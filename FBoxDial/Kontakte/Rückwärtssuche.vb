Imports System.Threading.Tasks

Public Module Rückwärtssuche
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String))
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Boolean))

    Friend Async Function StartRWS(TelNr As Telefonnummer, RWSIndex As Boolean) As Task(Of String)
        Dim vCard As String = DfltStringEmpty
        Dim RWSIndexEintrag As RWSIndexEntry

        NLogger.Debug($"Starte Kontaktsuche per Rückwärtssuche für Telefonnummer '{TelNr.Unformatiert}'.")

        If RWSIndex Then
            ' Prüfe ob im RWSIndex ein Eintrag vorhanden ist
            If XMLData.PTelListen.RWSIndex IsNot Nothing Then

                RWSIndexEintrag = XMLData.PTelListen.RWSIndex.Find(Function(RWSEntry) TelNr.Equals(RWSEntry.TelNr))
                If RWSIndexEintrag IsNot Nothing AndAlso RWSIndexEintrag.VCard IsNot Nothing AndAlso RWSIndexEintrag.VCard.IsNotStringEmpty Then
                    vCard = RWSIndexEintrag.VCard
                End If
            End If
        End If

        If vCard.IsStringEmpty Then
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
        Dim VCard As String = DfltStringEmpty
        Dim Gefunden As Boolean = False

        ' Webseite für Rückwärtssuche aufrufen und herunterladen
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximal drei mal durchlaufen
        i = 0

        baseurl = "https://www.dasoertliche.de?form_name="

        tmpTelNr = TelNr.Unformatiert
        Do
            PushStatus(LogLevel.Debug, $"Start RWS{i}: {baseurl}search_inv&ph={tmpTelNr}")

            htmlRWS = Await HTTPAsyncGet($"{baseurl}search_inv&ph={tmpTelNr}", Encoding.Default)

            If htmlRWS.IsNotStringEmpty Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen
                ' Link zum Herunterladen der vCard suchen
                EintragsID = htmlRWS.GetSubString("form_name=detail&amp;action=58&amp;page=78&amp;context=11&amp;id=", "&")
                If EintragsID.IsNotErrorString Then
                    PushStatus(LogLevel.Debug, $"Link vCard: {baseurl}vcard&id={EintragsID}")
                    VCard = Await HTTPAsyncGet($"{baseurl}vcard&id={EintragsID}", Encoding.Default)
                End If
            End If

            If VCard.StartsWith(DfltBegin_vCard) Then
                Gefunden = True
                PushStatus(LogLevel.Debug, VCard)
            Else
                VCard = DfltStringEmpty
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
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Nothing, New NotifyEventArgs(Of String)(StatusMessage))
    End Sub
End Module
