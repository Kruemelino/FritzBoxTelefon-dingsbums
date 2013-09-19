Imports System.Windows.Forms
Public Class formRWSuche
    Private hf As Helfer
    Private KontaktFunktionen As Contacts
    Private DateiPfad As String
    Private HTMLFehler As ErrObject

    Public Enum Suchmaschine
        RWSGoYellow = 0
        RWS11880 = 1
        RWSDasTelefonbuch = 2
        RWStelSearch = 3
        RWSAlle = 4
    End Enum
    Public Sub New(ByVal inipfad As String, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts)
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        DateiPfad = inipfad
        hf = HelferKlasse
        KontaktFunktionen = KontaktKlasse
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    Public Sub Rückwärtssuche(ByVal RWSAnbieter As Suchmaschine, ByVal olInsp As Outlook.Inspector)
        ' Startet die Rückwärtssuche mit verschiedenen Suchmaschinen
        ' funktioniert nur in Kontakt- und Journaleinträgen
        ' Parameter:  Suchmaschine (Integer):  Kennnummer der Suchmaschinen
        '               = 0: GoYellow
        '               = 1: 11880
        '               = 2: RWSDasTelefonbuch
        '               = 3: TelSearch.ch

        Dim alleTelNr(14) As String   ' alle im Kontakt enthaltenen Telefonnummern
        Dim alleNrTypen(14) As String   ' die Bezeichnungen der Telefonnummern
        Dim i As Integer, iTelNr As Integer      ' Zählvariablen
        Dim TelNr As String    ' Telefonnummer des zu Suchenden
        Dim vCard As String = String.Empty    ' gefundene vCard
        Dim rws As Boolean   ' 'true' wenn was gefunden wurde
        Dim row(2) As String


        If Not olInsp Is Nothing Then
            If TypeOf olInsp.CurrentItem Is Outlook.ContactItem Then
                Dim oContact As Outlook.ContactItem = CType(olInsp.CurrentItem, Outlook.ContactItem)
                With Me.ListTel
                    Do Until .RowCount = 0
                        .Rows.Remove(.Rows(0))
                    Loop
                End With
                With oContact ' ist aktuelles Fenster ein Kontakt?
                    iTelNr = 0
                    ' alle Telefonnummern in 'formRWSuche' eintragen
                    alleTelNr(0) = .AssistantTelephoneNumber
                    alleTelNr(1) = .BusinessTelephoneNumber
                    alleTelNr(2) = .Business2TelephoneNumber
                    alleTelNr(3) = .CallbackTelephoneNumber
                    alleTelNr(4) = .CarTelephoneNumber
                    alleTelNr(5) = .CompanyMainTelephoneNumber
                    alleTelNr(6) = .HomeTelephoneNumber
                    alleTelNr(7) = .Home2TelephoneNumber
                    alleTelNr(8) = .ISDNNumber
                    alleTelNr(9) = .MobileTelephoneNumber
                    alleTelNr(10) = .OtherTelephoneNumber
                    alleTelNr(11) = .PagerNumber
                    alleTelNr(12) = .PrimaryTelephoneNumber
                    alleTelNr(13) = .RadioTelephoneNumber
                    alleNrTypen = Split("Assistent,Geschäftlich,Geschäftlich2,Rückmeldung,Auto,Firma,Privat,Privat2,ISDN,Mobiltelefon,Weitere,Pager,Haupttelefon,Funkruf", ",", , CompareMethod.Text)
                    For i = LBound(alleTelNr) To UBound(alleTelNr)
                        If Not alleTelNr(i) = String.Empty Then ''''''''''''''' hier evtl. noch ein "or xxx = nothing?? rein?
                            iTelNr += 1
                            row(0) = CStr(iTelNr)
                            row(1) = alleNrTypen(i)
                            row(2) = alleTelNr(i)
                            Me.ListTel.Rows.Add(row)
                        End If
                    Next

                    Me.ListTel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                    Me.ListTel.ClearSelection()
                    Me.ButtonSuchen.Focus()
                    ' 'formRWSuche' modal einblenden (nach schließen des Fensters geht es hier weiter)
                    Me.ShowDialog()

                    If Me.ListTel.SelectedRows.Count > 0 Then
                        TelNr = Me.ListTel.SelectedRows.Item(0).Cells(2).Value.ToString ' TelNr aus Liste oder Eingabefeld entnehmen
                    Else
                        TelNr = Me.DirektTel.Text
                    End If
                    ' je nach 'Suchmaschine' Suche durchführen
                    If Not TelNr = "" Then
                        Select Case RWSAnbieter
                            Case Suchmaschine.RWSGoYellow
                                rws = RWSGoYellow(TelNr, vCard)
                            Case Suchmaschine.RWS11880
                                rws = RWS11880(TelNr, vCard)
                            Case Suchmaschine.RWSDasTelefonbuch
                                rws = RWSDasTelefonbuch(TelNr, vCard)
                            Case Suchmaschine.RWStelSearch
                                rws = RWStelsearch(TelNr, vCard)
                            Case Suchmaschine.RWSAlle
                                rws = RWSAlle(TelNr, vCard)
                        End Select
                        If rws Then
                            ' wenn erfolgreich, dann Ergebnisse aus vCard in den Kontakt übertragen
                            KontaktFunktionen.vCard2Contact(vCard, oContact)
                            ' falls TelNr bei der Rückwärtssuche geändert wurde, diese nummer als Zweitnummer eintragen
                            If Not hf.nurZiffern(.BusinessTelephoneNumber, "0049") = hf.nurZiffern(TelNr, "0049") And Not .BusinessTelephoneNumber = "" Then
                                .Business2TelephoneNumber = hf.formatTelNr(TelNr)
                            ElseIf Not hf.nurZiffern(.HomeTelephoneNumber, "0049") = hf.nurZiffern(TelNr, "0049") And Not .HomeTelephoneNumber = "" Then
                                .Home2TelephoneNumber = hf.formatTelNr(TelNr)
                            End If
                            .Body = "Rückwärtssuche erfolgreich" & vbCrLf & "Achtung! Unter Umständen werden vorhandene Daten überschrieben. Wir übernehmen keine Haftung für verloren gegangene Daten und für falsche Informationen, die die Rückwärtssuche liefert! Nutzung auf eigene Gefahr!" & vbCrLf & .Body
                        Else
                            .Body = "Rückwärtssuche nicht erfolgreich" & vbCrLf & .Body
                        End If
                    End If
                End With
            ElseIf TypeOf olInsp.CurrentItem Is Outlook.JournalItem Then
                ' ist aktuelles Fenster ein Journal?
                Dim olJournal As Outlook.JournalItem = CType(olInsp.CurrentItem, Outlook.JournalItem)
                With olJournal
                    If Not InStr(.Categories, "FritzBox Anrufmonitor") = 0 Then
                        ' wurde der Eintrag vom Anrufmonitor angelegt?
                        ' TelNr aus dem .Body entnehmen
                        TelNr = Mid(.Body, 11, InStr(1, .Body, vbNewLine) - 11)
                        ' je nach 'Suchmaschine' Suche durchführen
                        Select Case RWSAnbieter
                            Case Suchmaschine.RWSGoYellow
                                rws = RWSGoYellow(TelNr, vCard)
                            Case Suchmaschine.RWS11880
                                rws = RWS11880(TelNr, vCard)
                            Case Suchmaschine.RWSDasTelefonbuch
                                rws = RWSDasTelefonbuch(TelNr, vCard)
                            Case Suchmaschine.RWStelSearch
                                rws = RWStelsearch(TelNr, vCard)
                            Case Suchmaschine.RWSAlle
                                rws = RWSAlle(TelNr, vCard)
                        End Select
                        If rws Then
                            ' wenn erfolgreich, dann Ergebnis (vCard) dem .Body hinzufügen
                            .Body = .Body & vbCrLf & vbCrLf & vCard & vbCrLf
                        Else
                            .Body = .Body & vbCrLf & "Rückwärtssuche nicht erfolgreich: Es wurden keine Einträge gefunden." & vbCrLf
                        End If
                    End If
                End With
                hf.NAR(olJournal) : olJournal = Nothing
            End If
        End If
    End Sub

    Function RWS11880(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' führt die Rückwärtssuche über 'www.11880.com' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        RWS11880 = False

        Dim myurl As String             ' URL von 11880
        Dim temp As String             ' Hilfsstring
        Dim tempTelNr As String             ' Hilfsstring für TelNr
        Dim alleTelNr() As String             ' alle TelNr der gefundenen vCard
        Dim html11880 As String             ' Inhalt der Webseite
        Dim pos1, pos2, pos3 As Integer               ' Positionen in 'html11880'
        Dim i As Integer               ' Zählvariable

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SWVisitenkarte1 As String = "<a class='micro_action vcf_enabled' rel='nofollow' href='"
        Const SWVisitenkarte2 As String = "'"
        ' TelNr sichern, da sie unter Umständen verändert wird
        tempTelNr = hf.nurZiffern(TelNr, "0049")
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        If Not Strings.Left(tempTelNr, 2) = "11" Then

            Do
                ' Webseite für Rückwärtssuche aufrufen und herunterladen
                myurl = "http://www.11880.com/inverssuche/index/search?method=searchSimple&_dvform_posted=1&phoneNumber=" & tempTelNr
                html11880 = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                If HTMLFehler Is Nothing Then


                    html11880 = Replace(html11880, Chr(34), "'", , , CompareMethod.Text)  '" enfernen

                    On Error GoTo 0
                    ' Link zum Herunterladen der vCard suchen
                    pos1 = InStr(1, html11880, SWVisitenkarte1, CompareMethod.Text) + Len(SWVisitenkarte1)
                    If Not pos1 = Len(SWVisitenkarte1) Then 'wenn der Startpunkt gefunden werden konnte
                        pos2 = InStr(pos1, html11880, SWVisitenkarte2, CompareMethod.Text) 'Starten ab Startpunkt, suchen des Endpunkts
                        ' vCard herunterladen
                        myurl = "http://www.11880.com" & Mid(html11880, pos1, pos2 - pos1)
                        vCard = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                        If Not HTMLFehler Is Nothing Then
                            hf.LogFile("FBError (RWS11880): " & Err.Number & " - " & Err.Description & " - " & myurl)
                        End If
                    End If
                    ' Rückgabewert ermitteln
                    RWS11880 = Strings.Left(vCard, 11) = "BEGIN:VCARD"
                    i = i + 1
                    tempTelNr = Strings.Left(tempTelNr, Len(tempTelNr) - 1) & 0
                Else
                    hf.LogFile("FBError (RWS11880): " & Err.Number & " - " & Err.Description & " - " & myurl)
                End If
            Loop Until RWS11880 Or i = 3

        End If
        ' Besonderheit bei '11880': Vor- und Nachname sind in vCard separat angegeben
        ' wenn kein Vorname vorhanden ist, dann "muss" es sich um eine Firma handeln
        ' dann wird der volle Name in der vCard (FN) in den Firmennamen (ORG) übertragen
        If RWS11880 Then
            If InStr(1, ReadFromVCard(vCard, "N", ""), ";;;;", CompareMethod.Text) > 0 Then ''''''''''''' beim Debuggen aufpassen, dass auch das richtige Ergebnis ausgeworfen wird!!
                vCard = Replace(vCard, Chr(10) & "FN:", Chr(10) & "ORG:", , , CompareMethod.Text)
            End If

        End If
    End Function

    Function RWSGoYellow(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' führt die Rückwärtssuche über 'www.goyellow.de' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        RWSGoYellow = False

        Dim myurl As String             ' URL von 11880
        Dim temp As String             ' Hilfsstring
        Dim tempTelNr As String ' Hilfsstring für TelNr
        Dim htmlGoYellow As String             ' Inhalt der Webseite
        Dim pos, pos1, pos2 As Integer               ' Positionen in 'html11880'
        Dim i As Long               ' Zählvariable

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SWVisitenkarte1 As String = "<a title=Eine Visitenkarte"
        Const SWVisitenkarte2 As String = "href="
        ' Vorwahl erkennen
        ' TelNr sichern, da sie unter Umständen verändert wird
        tempTelNr = hf.nurZiffern(TelNr, "0049")
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        If Not Strings.Left(tempTelNr, 2) = "11" Then
            Do
                ' Webseite für Rückwärtssuche aufrufen und herunterladen
                myurl = "http://www.goyellow.de/inverssuche/?TEL=" & tempTelNr
                htmlGoYellow = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                If HTMLFehler Is Nothing Then
                    htmlGoYellow = Replace(htmlGoYellow, Chr(34), "", , , CompareMethod.Text) '" enfernen
                    pos = InStr(1, htmlGoYellow, "<a href=/upgrade?q=", CompareMethod.Text)
                    If Not pos = 0 Then
                        pos1 = InStr(pos, htmlGoYellow, " title", CompareMethod.Text)
                        myurl = "http://www.goyellow.de/upgrade?TEL=" & tempTelNr & "&q=" & Mid(htmlGoYellow, pos + 19, pos1 - pos - 19)
                        htmlGoYellow = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                        htmlGoYellow = Replace(htmlGoYellow, Chr(34), "", , , CompareMethod.Text) '" enfernen
                    End If

                    ' Link zum Herunterladen der vCard suchen
                    pos = InStr(1, htmlGoYellow, SWVisitenkarte1, CompareMethod.Text)
                    If Not pos = 0 Then
                        pos1 = InStr(pos, htmlGoYellow, SWVisitenkarte2) + Len(SWVisitenkarte2)
                        pos2 = InStr(pos1, htmlGoYellow, ">", CompareMethod.Text)
                        If Not pos1 = Len(SWVisitenkarte2) And Not pos2 = 0 Then
                            ' vCard herunterladen
                            myurl = "http://www.goyellow.de" & Mid(htmlGoYellow, pos1, pos2 - pos1)
                            vCard = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                        End If
                    End If
                    ' Rückgabewert ermitteln
                    RWSGoYellow = Strings.Left(vCard, 11) = "BEGIN:VCARD"
                    i = i + 1
                    tempTelNr = Strings.Left(tempTelNr, Len(tempTelNr) - 2) & 0
                Else
                    hf.LogFile("FBError (RWSGoYellow): " & Err.Number & " - " & Err.Description & " - " & myurl)
                    Exit Do
                End If
            Loop Until RWSGoYellow Or i = 3
        End If
        ' Bemerkungen und Webseiten aus vCard entfernen, da sie Werbung enthalten
        If RWSGoYellow Then
            pos1 = InStr(1, vCard, "URL", CompareMethod.Text)
            If Not pos1 = 0 Then
                pos2 = InStr(pos1, vCard, Chr(10), CompareMethod.Text)
                If Not pos2 = 0 Then temp = Mid(vCard, pos1, pos2 - pos1 + 1) Else temp = ""
                If Not InStr(1, vCard, "www.goyellow.de", CompareMethod.Text) = 0 Then vCard = Replace(vCard, temp, "", , , CompareMethod.Text)
            End If
            pos1 = InStr(1, vCard, "NOTE", CompareMethod.Text)
            If Not pos1 = 0 Then
                pos2 = InStr(pos1, vCard, Chr(10), CompareMethod.Text)
                If Not pos2 = 0 Then temp = Mid(vCard, pos1, pos2 - pos1 + 1) Else temp = ""
                If Not InStr(1, vCard, "www.goyellow.de", CompareMethod.Text) = 0 Then vCard = Replace(vCard, temp, "", , , CompareMethod.Text)
            End If
        End If
    End Function

    Function RWSDasTelefonbuch(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' führt die Rückwärtssuche über 'www.dastelefonbuch.de' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde
        Dim myurl As String             ' URL von 11880
        Dim formdata As String
        Dim tempTelNr As String             ' Hilfsstring für TelNr
        Dim html As String             ' Inhalt der Webseite

        Dim pos1, pos2 As Integer               ' Positionen in 'html11880'
        Dim i As Integer
        Dim Text As String

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SW1 As String = "<a title='Kontakt als V-Card herunterladen' href='"
        Const SW2 As String = "'"
        ' Webseite für Rückwärtssuche aufrufen und herunterladen
        vCard = ""
        tempTelNr = hf.nurZiffern(TelNr, "0049")
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        Do
            myurl = "http://www.dastelefonbuch.de/"
            formdata = "sp=0&aktion=23&kw=" & tempTelNr & "&ort=&cifav=0&s=a10000&stype=s&la=de&taoid=&cmd=search&ort_ok=0&vert_ok=0"

            Text = hf.httpWrite(myurl, formdata, System.Text.Encoding.Default)
            If Not Text = "" Then
                html = Replace(Text, Chr(34), "'", , , CompareMethod.Text) '" enfernen
                pos1 = InStr(1, html, SW1, CompareMethod.Text)
                If Not pos1 = 0 Then
                    pos1 = pos1 + Len(SW1)
                    pos2 = InStr(pos1, html, SW2, vbTextCompare)
                    myurl = Mid(html, pos1, pos2 - pos1)

                    'myurl = Replace(myurl, "dastelefonbuch.de", "dastelefonbuch.de/", , , vbTextCompare)
                    myurl = Replace(myurl, "&amp;", "&", , , vbTextCompare)
                    vCard = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                End If
            End If
            If Not HTMLFehler Is Nothing Then
                hf.LogFile("FBError (RWSDasTelefonbuch): " & Err.Number & " - " & Err.Description & " - " & myurl)
            End If
            RWSDasTelefonbuch = Strings.Left(vCard, 11) = "BEGIN:VCARD"
            i = i + 1
            tempTelNr = Strings.Left(tempTelNr, Len(tempTelNr) - 2) & 0
        Loop Until RWSDasTelefonbuch Or i = 3

    End Function

    Function RWStelsearch(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' Suchmaschienen Script für die Schweiz
        ' führt die Rückwärtssuche über 'www.telsearch.ch' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        Dim myurl As String             ' URL von telsearch
        'Dim temp As String             ' Hilfsstring
        Dim tempTelNr As String             ' Hilfsstring für TelNr
        Dim htmltelsearch As String             ' Inhalt der Webseite
        Dim pos1, pos2 As Integer               ' Positionen in 'htmltelsearch'
        Dim i As Long               ' Zählvariable

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SWVisitenkarte1 As String = "<a href=/vCard/"
        Const SWVisitenkarte2 As String = "title"

        ' Vorwahl erkennen
        ' TelNr sichern, da sie unter Umständen verändert wird
        tempTelNr = hf.nurZiffern(TelNr, "0041")
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        Do
            ' Webseite für Rückwärtssuche aufrufen und herunterladen
            myurl = "http://tel.search.ch/result.html?name=&misc=&strasse=&ort=&kanton=&tel=" & tempTelNr
            htmltelsearch = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
            If HTMLFehler Is Nothing Then
                htmltelsearch = Replace(htmltelsearch, Chr(34), "", , , vbTextCompare) '" enfernen

                ' Link zum Herunterladen der vCard suchen
                pos1 = InStr(1, htmltelsearch, SWVisitenkarte1, vbTextCompare)
                If Not pos1 = 0 Then
                    pos2 = InStr(pos1, htmltelsearch, SWVisitenkarte2, vbTextCompare)
                    If Not pos1 = Len(SWVisitenkarte2) And Not pos2 = 0 Then
                        ' vCard herunterladen
                        myurl = "http://tel.search.ch/" & Mid(htmltelsearch, pos1 + 9, pos2 - pos1 - 10)
                        myurl = Replace(myurl, "html", "vcf")
                        vCard = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                    End If
                End If
                ' Rückgabewert ermitteln
                RWStelsearch = Strings.Left(vCard, 11) = "BEGIN:VCARD"
                i = i + 1
                tempTelNr = Strings.Left(tempTelNr, Len(tempTelNr) - 2) & 0
            Else
                RWStelsearch = False
                hf.LogFile("FBError (RWStelsearch): " & Err.Number & " - " & Err.Description & " - " & myurl)
                Exit Do
            End If

        Loop Until RWStelsearch Or i = 3
        ' Besonderheit bei 'telsearch': Vor- und Nachname sind in vCard separat angegeben
        ' wenn kein Vorname vorhanden ist, dann "muss" es sich um eine Firma handeln
        ' dann wird der volle Name in der vCard (FN) in den Firmennamen (ORG) übertragen
        If RWStelsearch Then
            If InStr(1, ReadFromVCard(vCard, "N", ""), ";;;;", CompareMethod.Text) > 0 Then
                vCard = Replace(vCard, Chr(10) & "FN:", Chr(10) & "ORG:", , , vbTextCompare)
            End If
        End If

    End Function

    Function RWSAlle(ByRef TelNr As String, ByRef vCard As String) As Boolean
        RWSAlle = RWS11880(TelNr, vCard)
        If RWSAlle Then Exit Function
        RWSAlle = RWSGoYellow(TelNr, vCard)
        If RWSAlle Then Exit Function
        RWSAlle = RWSDasTelefonbuch(TelNr, vCard)
        If RWSAlle Then Exit Function
        RWSAlle = RWStelsearch(TelNr, vCard)
    End Function
#Region "Helfer"
    Private Sub DirektTel_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles DirektTel.GotFocus
        Me.ListTel.ClearSelection()
    End Sub

    Private Sub ButtonSuchen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSuchen.Click
        Me.Close()
    End Sub

    Private Sub ListTel_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListTel.SelectionChanged
        Me.ButtonSuchen.Focus()
    End Sub
#End Region
End Class