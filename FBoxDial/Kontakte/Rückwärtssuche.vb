Imports System.Net.Http
Imports System.Threading.Tasks

Public Module Rückwärtssuche
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Event Status As EventHandler(Of String)
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Boolean))

    Private Const httpClientKey As String = "rws"

    Friend Async Function StartRWS(TelNr As Telefonnummer, RWSIndex As Boolean) As Task(Of String)
        Dim vCard As String = String.Empty
        Dim RWSIndexEintrag As RWSIndexEntry

        NLogger.Debug($"Starte Kontaktsuche per Rückwärtssuche für Telefonnummer '{TelNr.Unformatiert}'.")

        If RWSIndex Then
            ' Prüfe ob im RWSIndex ein Eintrag vorhanden ist
            If XMLData.PTelListen.RWSIndex IsNot Nothing Then

                RWSIndexEintrag = XMLData.PTelListen.RWSIndex.Find(Function(RWSEntry) TelNr.Equals(RWSEntry.TelNr))
                If RWSIndexEintrag IsNot Nothing AndAlso RWSIndexEintrag.VCard IsNot Nothing AndAlso RWSIndexEintrag.VCard.IsNotStringNothingOrEmpty Then
                    vCard = RWSIndexEintrag.VCard
                End If
            End If
        End If

        If vCard.IsStringNothingOrEmpty Then
            vCard = Await RWSDasOertiche(TelNr)

            If RWSIndex Then
                RWSIndexEintrag = New RWSIndexEntry
                With RWSIndexEintrag
                    .Datum = Date.Now
                    .VCard = vCard
                    .TelNr = TelNr.Unformatiert
                End With

                ' RWS-Index-Liste initialisieren, falls erforderlich
                If XMLData.PTelListen.RWSIndex Is Nothing Then XMLData.PTelListen.RWSIndex = New List(Of RWSIndexEntry)

                XMLData.PTelListen.RWSIndex.Add(RWSIndexEintrag)
            End If
        End If
        Return vCard
    End Function

    ''' <summary>
    ''' Führt die Rückwärtssuche über 'www.dasoertliche.de' durch.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ''' <returns>'true' wenn was gefunden wurde</returns>
    Private Async Function RWSDasOertiche(TelNr As Telefonnummer) As Task(Of String)

        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring für TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Zählvariable
        Dim baseurl As String
        Dim VCard As String = String.Empty
        Dim Gefunden As Boolean = False

        ' Webseite für Rückwärtssuche aufrufen und herunterladen
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximal drei mal durchlaufen
        i = 0

        baseurl = "https://www.dasoertliche.de/?form_name="

        Globals.ThisAddIn.FBoxhttpClient.RegisterClient(httpClientKey, New HttpClientHandler)

        tmpTelNr = TelNr.Unformatiert

        Dim RequestMessage As HttpRequestMessage

        Do
            PushStatus(LogLevel.Debug, $"Start RWS{i}: {baseurl}search_inv&ph={tmpTelNr}")

            ' Fange Fehlermeldungen der Rückwärtssuche ab: Wenn die Nummer nicht gefunden wurde, dann wird ein Fehler zurückgeben.
            RequestMessage = New HttpRequestMessage With {.Method = HttpMethod.Get,
                                                          .RequestUri = New Uri($"{baseurl}search_inv&ph={tmpTelNr}")}

            htmlRWS = Await Globals.ThisAddIn.FBoxhttpClient.GetString(httpClientKey, RequestMessage, Encoding.UTF8)

            If htmlRWS.IsNotStringNothingOrEmpty Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen
                ' Aus dem Response muss die ID des Eintrages ermittelt werden. Es gibt mehrere Möglichkeiten
                EintragsID = htmlRWS.GetSubString("var handlerData =[['", "']];").Split("','").First

                If EintragsID.IsNotEqual("-1") Then
                    ' Link zum Herunterladen der vCard suchen
                    PushStatus(LogLevel.Debug, $"Link vCard: {baseurl}vcard&id={EintragsID}")

                    RequestMessage = New HttpRequestMessage With {.Method = HttpMethod.Get,
                                                                  .RequestUri = New Uri($"{baseurl}vcard&id={EintragsID}")}

                    VCard = Await Globals.ThisAddIn.FBoxhttpClient.GetString(httpClientKey, RequestMessage, Encoding.Default)
                Else
                    PushStatus(LogLevel.Warn, $"ID des Eintrages für {tmpTelNr} kann nicht ermittelt werden.")
                End If
            End If

            If VCard.StartsWith("BEGIN:VCARD") Then
                Gefunden = True
                PushStatus(LogLevel.Debug, VCard)
            Else
                VCard = String.Empty
            End If
            i += 1
            ' Ersetze die letzten beiden Zeichen durch eine Null.
            tmpTelNr = tmpTelNr.RegExReplace(".{2}$", "0")

        Loop Until Gefunden Or i = 3

        ' Event für das Beenden dieser Routine 
        RaiseEvent Beendet(Nothing, New NotifyEventArgs(Of Boolean)(Gefunden))

        Return VCard
    End Function

    ''' <summary>
    ''' Gibt eine Statusmeldung (<paramref name="StatusMessage"/>) als Event aus. Gleichzeitig wird in das Log mit vorgegebenem <paramref name="Level"/> geschrieben.
    ''' </summary>
    ''' <param name="Level">NLog LogLevel</param>
    ''' <param name="StatusMessage">Die auszugebende Statusmeldung.</param>
    <DebuggerStepThrough>
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Nothing, StatusMessage)
    End Sub
End Module
