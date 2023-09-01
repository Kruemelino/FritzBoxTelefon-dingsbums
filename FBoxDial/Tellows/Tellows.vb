Imports System.IO
Imports System.Net.Http
Imports System.Threading.Tasks

Friend Class Tellows
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    ''' <summary>
    ''' MD5 Hash vom tellows APIKey
    ''' </summary>
    Private ReadOnly Property XAuthToken As String
    Private ReadOnly Property Pfad As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, $"{My.Resources.strDefShortName}.json")
    Private ReadOnly Property Ready As Boolean
        Get
            Return XAuthToken.IsNotStringNothingOrEmpty AndAlso XMLData.POptionen.TBTellowsApiKeyGütigBis.IsLaterAs(Now)
        End Get
    End Property

    Private Const httpClientKey As String = "tellows"

    Public Sub New(Optional Token As String = "")
        If Token.IsStringNothingOrEmpty Then
            Using Crypter As New Rijndael
                With Crypter
                    XAuthToken = .SecureStringToHash(.DecryptString(XMLData.POptionen.TBTellowsAPIKey, My.Resources.strDfltTellowsDeCryptKey),
                                                     Encoding.Default, Security.Cryptography.HashAlgorithmName.MD5.Name)
                End With
            End Using
        Else
            XAuthToken = Token
        End If

        ' httpClient für tellows registrieren
        Globals.ThisAddIn.FBoxhttpClient.RegisterClient(httpClientKey, New HttpClientHandler)

    End Sub

#Region "Basisfunktionen"

    Private Async Function GetTellowsResponseXML(UniformResourceIdentifier As Uri) As Task(Of TellowsResponse)
        Dim Response As New TellowsResponse

        Dim RequestMessage As New HttpRequestMessage With {.Method = HttpMethod.Get,
                                                           .RequestUri = UniformResourceIdentifier}

        RequestMessage.Headers.Add("X-Auth-Token", XAuthToken)

        Dim ResponseString As String = Await Globals.ThisAddIn.FBoxhttpClient.GetString(httpClientKey, RequestMessage, Encoding.UTF8)

        ' Deserialisieren
        If DeserializeXML(ResponseString, False, Response) Then
            NLogger.Debug(ResponseString)
        Else
            NLogger.Error($"Die Tellows Abfrage zu '{UniformResourceIdentifier}' war nicht erfolgreich.")
            Return New TellowsResponse
        End If

        Return Response
    End Function

#End Region

#Region "AccountInfo"
    ''' <summary>
    ''' Führt eine Abfrage beim tellows zum Herunterladen der Account-Info bei tellows durch.
    ''' </summary>
    ''' <returns>Antwort von tellows als <see cref="TellowsPartnerInfo"/></returns>
    Friend Async Function GetTellowsAccountInfo() As Task(Of TellowsPartnerInfo)
        If XAuthToken.IsNotStringNothingOrEmpty Then
            Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                           .Host = "www.tellows.de",
                                           .Path = "/api/getpartnerinfo",
                                           .Query = String.Join("&", {"xml=1"})}

            Return (Await GetTellowsResponseXML(ub.Uri)).Partnerinfo
        Else
            NLogger.Warn($"Abfrage der tellows Accountdaten nicht möglich, da kein gültiger API-Key eingegeben wurde.")
            Return New TellowsPartnerInfo With {.Info = "Kein tellows ApiKey vorhanden."}
        End If
    End Function

#End Region


    ''' <summary>
    ''' Führt eine Abfrage beim tellows über die LiveAPI durch.
    ''' </summary>
    ''' <param name="TelNr">Abzufragende Telefonnummer</param>
    ''' <returns>Antwort von tellows als <see cref="TellowsResponse"/></returns>
    Friend Async Function GetTellowsLiveAPIData(TelNr As Telefonnummer) As Task(Of TellowsResponse)
        If Ready Then
            NLogger.Info($"Starte Abfrage via tellows LiveAPI für Nummer {TelNr.TellowsNummer}")

            Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                           .Host = "www.tellows.de",
                                           .Path = $"/basic/num/{TelNr.TellowsNummer}",
                                           .Query = String.Join("&", {"xml=1", "country=de", "lang=de", "showcomments=10"})}

            Return Await GetTellowsResponseXML(ub.Uri)
        Else
            NLogger.Warn($"Abfrage via tellows LiveAPI für Nummer {TelNr.TellowsNummer} nicht möglich, da kein gültiger API-Key eingegeben wurde.")
            Return Nothing
        End If

    End Function

#Region "Herunterladen der ScoreList"
    ''' <summary>
    ''' Führt eine Abfrage beim tellows zum Herunterladen der ScoreList durch.
    ''' </summary>
    ''' <returns>Antwort von tellows als <see cref="List(Of TellowsScoreListEntry)"/></returns>
    Friend Async Function LadeScoreList() As Task(Of List(Of TellowsScoreListEntry))
        NLogger.Debug($"Lade tellows ScoreList")

        If Ready Then

            If Not File.Exists(Pfad) OrElse (Now.Subtract(File.GetLastWriteTime(Pfad)).TotalHours.IsLargerOrEqual(24) Or New FileInfo(Pfad).Length.IsZero) Then
                Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                               .Host = "www.tellows.de",
                                               .Path = "/stats/partnerscoredata",
                                               .Query = String.Join("&", {$"apikeyMd5={XAuthToken}",
                                                                           "json=1",
                                                                           "country=de",
                                                                           "lang=de",
                                                                           "minscore=1",
                                                                           "showprefixname=1",
                                                                          $"mincomments={XMLData.POptionen.CBTellowsAnrMonMinComments}",
                                                                           "showcallername=1"})}

                Dim TellowsList As List(Of TellowsScoreListEntry) = Await JSONDeserializeFromStreamAsync(Of List(Of TellowsScoreListEntry))(Await Globals.ThisAddIn.FBoxhttpClient.GetStream(httpClientKey, ub.Uri), True)

                If TellowsList IsNot Nothing Then
                    ' Speichere die TellowsDatei
                    JSONSerializeToFileAsync(Pfad, TellowsList, True)

                    ' Debug Meldung
                    NLogger.Debug($"tellows ScoreList von tellows direkt geladen und unter '{Pfad}' gespeichert.")

                    ' Rückgabe
                    Return TellowsList
                Else
                    NLogger.Error($"Die ScoreList von Tellows ' {ub.Uri} ' konnte nicht ermittelt werden.")

                    Return New List(Of TellowsScoreListEntry)
                End If

            Else
                ' Debug Meldung
                NLogger.Debug($"tellows ScoreList von Pfad '{Pfad}' geladen")

                Return Await JSONDeserializeFromFileAsync(Of List(Of TellowsScoreListEntry))(Pfad, True)
            End If

        Else
            NLogger.Warn($"Ein gültiger tellows API-Key wurde nicht eingegeben.")
            ' Gib eine leere Liste zurück
            Return New List(Of TellowsScoreListEntry)
        End If

    End Function

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
