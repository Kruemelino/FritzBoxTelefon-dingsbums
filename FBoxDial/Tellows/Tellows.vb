Imports System.Net
Imports System.Threading.Tasks

Friend Class Tellows
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    ''' <summary>
    ''' MD5 Hash vom tellows APIKey
    ''' </summary>
    Private ReadOnly Property XAuthToken As String
    Private ReadOnly Property Headers As WebHeaderCollection
    Private ReadOnly Property Pfad As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, $"{My.Resources.strDefShortName}.json")
    Private ReadOnly Property Ready As Boolean
        Get
            Return XAuthToken.IsNotStringNothingOrEmpty
        End Get
    End Property

    Private Const NotAuthorized As String = "NOT AUTHORIZED REQUEST - API Key not valid"

    Public Sub New(Token As String)
        XAuthToken = Token
        ' Header für WebClient setzen
        Headers = New WebHeaderCollection From {{"X-Auth-Token", XAuthToken}}
    End Sub

    Public Sub New()
        Using Crypter As New Rijndael
            With Crypter
                XAuthToken = .SecureStringToMD5(.DecryptString(XMLData.POptionen.TBTellowsAPIKey, My.Resources.strDfltTellowsDeCryptKey), Encoding.Default)
            End With
        End Using

        ' Header für WebClient setzen
        Headers = New WebHeaderCollection From {{"X-Auth-Token", XAuthToken}}
    End Sub

#Region "Basisfunktionen"

    Private Async Function GetTellowsResponseXML(UniformResourceIdentifier As Uri, Headers As WebHeaderCollection) As Task(Of TellowsResponse)
        Dim Response As New TellowsResponse
        ' Deserialisieren
        If Not DeserializeXML(Await DownloadStringTaskAsync(UniformResourceIdentifier, ZeichenCodierung:=Encoding.UTF8, Headers:=Headers), False, Response) Then
            NLogger.Error($"Die Tellows Abfrage zu '{UniformResourceIdentifier}' war nicht erfolgreich.")
            Return Nothing
        End If

        Return Response
    End Function

    Private Async Function GetTellowsResponseJSON(Pfad As Uri, Optional Headers As WebHeaderCollection = Nothing) As Task(Of List(Of TellowsScoreListEntry))

        Dim TellowsResponse As String = Await DownloadStringTaskAsync(Pfad, ZeichenCodierung:=Encoding.UTF8, Headers:=Headers)

        If TellowsResponse.Contains(NotAuthorized) Then
            NLogger.Warn($"Abfrage der tellows Accountdaten nicht möglich, da kein gültiger API-Key eingegeben wurde.")
            Return Nothing
        Else
            Return Await JSONDeserializeFromStringAsync(Of List(Of TellowsScoreListEntry))(TellowsResponse)
        End If

    End Function

#End Region

    ''' <summary>
    ''' Führt eine Abfrage beim tellows zum Herunterladen der Account-Info bei tellows durch.
    ''' </summary>
    ''' <returns>Antwort von tellows als <see cref="TellowsPartnerInfo"/></returns>
    Friend Async Function GetTellowsAccountInfo() As Task(Of TellowsPartnerInfo)
        If Ready Then
            Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                           .Host = "www.tellows.de",
                                           .Path = "/api/getpartnerinfo",
                                           .Query = String.Join("&", {"xml=1"})}

            Return (Await GetTellowsResponseXML(ub.Uri, Headers)).Partnerinfo
        Else
            NLogger.Warn($"Abfrage der tellows Accountdaten nicht möglich, da kein API-Key eingegeben wurde.")
            Return New TellowsPartnerInfo With {.Info = "Kein tellows ApiKey vorhanden."}
        End If
    End Function

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

            Return Await GetTellowsResponseXML(ub.Uri, Headers)
        Else
            NLogger.Warn($"Abfrage via tellows LiveAPI für Nummer {TelNr.TellowsNummer} nicht möglich, da kein API-Key eingegeben wurde.")
            Return Nothing
        End If

    End Function

#Region "Herunterladen der ScoreList"
    Friend Async Function LadeScoreList() As Task(Of List(Of TellowsScoreListEntry))
        NLogger.Debug($"Lade tellows ScoreList")

        If Ready Then
            If Await DownloadTellowsScoreList() Then
                ' Die Datei wurde neu heruntergeladen
                NLogger.Debug($"tellows ScoreList von tellows direkt geladen und unter '{Pfad}' gespeichert.")

            Else
                ' Die Datei ist aktuell und kann verwendet werden
                NLogger.Debug($"tellows ScoreList von Pfad '{Pfad}' geladen")
            End If

            ' Lade die Daten aus der Datei
            Return Await GetTellowsResponseJSON(New Uri(Pfad))
        Else
            NLogger.Warn($"Ein tellows API-Key wurde nicht eingegeben.")
            ' Gib eine leere Liste zurück
            Return New List(Of TellowsScoreListEntry)
        End If

    End Function

    ''' <summary>
    ''' Führt eine Abfrage beim tellows zum Herunterladen der ScoreList durch.
    ''' </summary>
    ''' <returns>Antwort von tellows als <see cref="List(Of TellowsScoreListEntry)"/></returns>
    Private Async Function DownloadTellowsScoreList() As Task(Of Boolean)

        If Not IO.File.Exists(Pfad) OrElse (Now.Subtract(IO.File.GetLastWriteTime(Pfad)).TotalHours.IsLargerOrEqual(24) Or New IO.FileInfo(Pfad).Length.IsZero) Then
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

            Return Await DownloadToFileTaskAsync(ub.Uri, Pfad, Encoding.UTF8, Headers)
        Else
            Return False
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
