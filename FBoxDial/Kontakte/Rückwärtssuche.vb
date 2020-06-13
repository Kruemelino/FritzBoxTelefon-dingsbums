Imports System.Threading.Tasks

Public Module Rückwärtssuche

    Friend Async Function StartRWS(ByVal TelNr As Telefonnummer, ByVal RWSIndex As Boolean) As Task(Of String)
        Dim vCard As String = PDfltStringEmpty
        Dim RWSIndexEintrag As RWSIndexEntry

        If RWSIndex Then
            ' Prüfe ob im RWSIndex ein Eintrag vorhanden ist
            If XMLData.PTelefonie.RWSIndex IsNot Nothing Then

                RWSIndexEintrag = XMLData.PTelefonie.RWSIndex.Find(Function(RWSEntry) TelNr.Equals(RWSEntry.TelNr))
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
                If XMLData.PTelefonie.RWSIndex Is Nothing Then
                    XMLData.PTelefonie.RWSIndex = New List(Of RWSIndexEntry)
                End If

                XMLData.PTelefonie.RWSIndex.Add(RWSIndexEintrag)
            End If
        End If
        Return vCard
    End Function

    ''' <summary>
    ''' Führt die Rückwärtssuche über 'www.dasoertliche.de' durch.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ''' <returns>'true' wenn was gefunden wurde</returns>
    Private Async Function RWSDasOertiche(ByVal TelNr As Telefonnummer) As Task(Of String)

        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring für TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Zählvariable
        Dim baseurl As String
        Dim VCard As String = PDfltStringEmpty
        Dim Gefunden As Boolean = False

        ' Webseite für Rückwärtssuche aufrufen und herunterladen
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0

        baseurl = "https://www.dasoertliche.de?form_name="

        tmpTelNr = TelNr.Unformatiert
        Do
            htmlRWS = Await HTTPGet(String.Format("{0}search_nat&kw={1}", baseurl, tmpTelNr), Encoding.Default)

            If htmlRWS.IsNotStringEmpty Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen

                ' Link zum Herunterladen der vCard suchen

                EintragsID = htmlRWS.GetSubString("dasoertliche.de/?id=", "&")
                If EintragsID.IsNotErrorString Then
                    VCard = Await HTTPGet(baseurl & "vcard&id=" & EintragsID, Encoding.Default)
                End If
            End If

            If VCard.StartsWith(PDfltBegin_vCard) Then
                Gefunden = True
            Else
                VCard = PDfltStringEmpty
            End If
            i += 1
            tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0

        Loop Until Gefunden Or i = 3

        Threading.Thread.Sleep(2000)
        Return VCard
    End Function

End Module
