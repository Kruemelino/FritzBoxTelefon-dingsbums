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
    ' Private ReadOnly Property Pfad As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltTellowsFileName)

    Public Sub New(Token As String)
        XAuthToken = Token
        ' Header für WebClient setzen
        Headers = New WebHeaderCollection From {{"X-Auth-Token", XAuthToken}}
    End Sub

    Public Sub New()
        Using Crypter As New Rijndael
            With Crypter
                XAuthToken = .SecureStringToMD5(.DecryptString(XMLData.POptionen.TBTellowsAPIKey, DfltTellowsDeCryptKey), Encoding.Default)
            End With
        End Using

        ' Header für WebClient setzen
        Headers = New WebHeaderCollection From {{"X-Auth-Token", XAuthToken}}
    End Sub

#Region "Basisfunktionen"

    Private Async Function GetTellowsResponseXML(UniformResourceIdentifier As Uri, Headers As WebHeaderCollection) As Task(Of TellowsResponse)
        Dim Response As New TellowsResponse
        ' Deserialisieren
        If Not DeserializeXML(Await DownloadStringTaskAsync(UniformResourceIdentifier, Encoding.UTF8, Headers), False, Response) Then
            NLogger.Error($"Die Tellows Abfrage zu '{UniformResourceIdentifier}' war nicht erfolgreich.")
        End If

        Return Response
    End Function

    Private Async Function GetTellowsResponseJSON(pfad As String, Optional Headers As WebHeaderCollection = Nothing) As Task(Of List(Of TellowsScoreListEntry))
        Return Await JSONDeserializeFromStringAsync(Of List(Of TellowsScoreListEntry))(Await DownloadStringTaskAsync(pfad, Encoding.UTF8, Headers))
    End Function

#End Region

    ''' <summary>
    ''' Führt eine Abfrage beim tellows zum Herunterladen der Account-Info bei tellows durch.
    ''' </summary>
    ''' <returns>Antwort von tellows als <see cref="TellowsPartnerInfo"/></returns>
    Friend Async Function GetTellowsAccountInfo() As Task(Of TellowsPartnerInfo)

        Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                       .Host = "www.tellows.de",
                                       .Path = "/api/getpartnerinfo",
                                       .Query = String.Join("&", {"xml=1", "country=de", "lang=de", "showcomments=10"})}

        Return (Await GetTellowsResponseXML(ub.Uri, Headers)).Partnerinfo

    End Function

    ''' <summary>
    ''' Führt eine Abfrage beim tellows über die LiveAPI durch.
    ''' </summary>
    ''' <param name="TelNr">Abzufragende Telefonnummer</param>
    ''' <returns>Antwort von tellows als <see cref="TellowsResponse"/></returns>
    Friend Async Function GetTellowsLiveAPIData(TelNr As Telefonnummer) As Task(Of TellowsResponse)

        NLogger.Debug($"Starte Apfrage via tellows LiveAPI für Nummer {TelNr.TellowsNummer}")

        Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                       .Host = "www.tellows.de",
                                       .Path = $"/basic/num/{TelNr.TellowsNummer}",
                                       .Query = "xml=1"}

        Return Await GetTellowsResponseXML(ub.Uri, Headers)

    End Function

    ''' <summary>
    ''' Führt eine Abfrage beim tellows zum herunterladen der ScoreList durch.
    ''' </summary>
    ''' <returns>Antwort von tellows als <see cref="List(Of TellowsScoreListEntry)"/></returns>
    Friend Async Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry))

        Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
                                       .Host = "www.tellows.de",
                                       .Path = "/stats/partnerscoredata",
                                       .Query = String.Join("&", {$"apikeyMd5={XAuthToken}",
                                                                   "json=1",
                                                                   "country=de",
                                                                   "lang=de",
                                                                   "minscore=1",
                                                                   "mincomments=3",
                                                                   "showcallername=1"})}

        Return Await GetTellowsResponseJSON(ub.Uri.AbsoluteUri, Headers)

    End Function

#Region "Herunterladen der ScoreList"
    'Friend Async Function LadeScoreList() As Task(Of List(Of TellowsScoreListEntry))
    '    NLogger.Debug($"Lade tellows ScoreList")
    '    If IO.File.Exists(Pfad) Then
    '        Return Await GetTellowsResponseJSON(Pfad)
    '        NLogger.Debug($"tellows ScoreList von Pfad '{Pfad}' geladen")
    '    Else
    '        Return Await GetTellowsScoreList()
    '        NLogger.Debug($"tellows ScoreList von tellows direkt geladen")
    '    End If
    'End Function
    '
    'Friend Async Function DownloadTellowsScoreList() As Task(Of Boolean)
    '
    '    If Not IO.File.Exists(Pfad) OrElse IO.File.GetLastWriteTime(Pfad).Subtract(Now).TotalHours.IsLargerOrEqual(24) Then
    '
    '        Dim ub As New UriBuilder With {.Scheme = Uri.UriSchemeHttps,
    '                                   .Host = "www.tellows.de",
    '                                   .Path = "/stats/partnerscoredata",
    '                                   .Query = String.Join("&", {$"apikeyMd5={XAuthToken}",
    '                                                               "json=1",
    '                                                               "country=de",
    '                                                               "lang=de",
    '                                                               "minscore=1",
    '                                                               "mincomments=3",
    '                                                               "showcallername=1"})}
    '
    '        Return Await DownloadToFileTaskAsync(ub.Uri, Pfad, Encoding.UTF8, Headers)
    '
    '    Else
    '        Return False
    '    End If
    'End Function
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
