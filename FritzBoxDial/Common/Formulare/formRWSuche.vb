Imports System.Windows.Forms
Friend Enum RückwärtsSuchmaschine
    RWSDasOertliche = 0
    RWS11880 = 1
    RWSDasTelefonbuch = 2
    RWStelSearch = 3
    RWSAlle = 4
End Enum

Friend Class formRWSuche
    Private C_hf As Helfer
    Private C_KF As Contacts
    Private C_DP As DataProvider
    Private C_XML As XML
    Private HTMLFehler As Boolean


    Public Sub New(ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal DataproviderKlasse As DataProvider, _
                   ByVal XMLKlasse As XML)
        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        C_hf = HelferKlasse
        C_KF = KontaktKlasse
        C_DP = DataproviderKlasse
        C_XML = XMLKlasse
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    ''' <summary>
    ''' Führt eine Rückwärtssuche durch. Funktion wird durch den Anrufmonitor gestartet. Der RWSIndex wird zunächst geprüft, danach
    ''' die ausgewählte RWS.
    ''' </summary>
    ''' <param name="Telefonat">Telefonat, das geprüft werden soll</param>
    ''' <returns>True, wenn gefunden. Neue Daten werden in dem Telefonat abgelegt.</returns>
    ''' <remarks></remarks>
    Friend Function AnrMonRWS(ByRef Telefonat As C_Telefonat) As Boolean
        AnrMonRWS = False
        With Telefonat

            Dim xPathTeile As New ArrayList

            If C_DP.P_CBRWSIndex Then
                .vCard = C_DP.P_Def_ErrorMinusTwo_String
                ' RWS-Index überprüfen
                With xPathTeile
                    .Clear()
                    .Add("CBRWSIndex")
                    .Add("Eintrag[@ID=""" & Telefonat.TelNr & """]")
                End With
                .vCard = C_XML.Read(C_DP.XMLDoc, xPathTeile, C_DP.P_Def_ErrorMinusOne_String)
            Else
                .vCard = C_DP.P_Def_ErrorMinusOne_String
            End If
            ' Drei mögliche Rückgaben
            ' Fall 1: Eine frühere RWS hat ein Ergebnis geliefert. Rückgabe: gültige vCard
            ' Fall 2: Eine frühere RWS hat kein Ergebnis geliefert. Rückgabe: -2
            ' Fall 3: Es gibt keinen Eintrag. Rückgabe: -1
            'ToDo:
            ' Fall 1: vCard übernehmen
            ' Fall 2: keine erneute RWS durchführen
            ' Fall 3: RWS durchführen
            Select Case .vCard
                Case C_DP.P_Def_ErrorMinusTwo_String ' Fall 2: Eine frühere RWS hat kein Ergebnis geliefert.
                    '.vCard = C_DP.P_Def_ErrorMinusTwo_String
                Case C_DP.P_Def_ErrorMinusOne_String ' Fall 3: Es gibt keinen Eintrag.
                    '
                    Select Case CType(C_DP.P_ComboBoxRWS, RückwärtsSuchmaschine) ' Fall 3: Es gibt keinen Eintrag.
                        Case RückwärtsSuchmaschine.RWSDasOertliche
                            AnrMonRWS = RWSDasOertiche(.TelNr, .vCard)
                        Case RückwärtsSuchmaschine.RWS11880
                            AnrMonRWS = RWS11880(.TelNr, .vCard)
                        Case RückwärtsSuchmaschine.RWSDasTelefonbuch
                            AnrMonRWS = RWSDasTelefonbuch(.TelNr, .vCard)
                        Case RückwärtsSuchmaschine.RWStelSearch
                            AnrMonRWS = RWStelsearch(.TelNr, .vCard)
                        Case RückwärtsSuchmaschine.RWSAlle
                            AnrMonRWS = RWSAlle(.TelNr, .vCard)
                    End Select
                    If C_DP.P_CBRWSIndex Then
                        xPathTeile.Item(xPathTeile.Count - 1) = "Eintrag"
                        C_XML.Write(C_DP.XMLDoc, xPathTeile, .vCard, "ID", .TelNr)
                    End If
                Case Else ' Fall 1: Eine frühere RWS hat ein Ergebnis geliefert. 
                    AnrMonRWS = True
            End Select

        End With
    End Function

    Public Sub Rückwärtssuche(ByVal RWSAnbieter As RückwärtsSuchmaschine, ByVal olInsp As Outlook.Inspector)
        ' Startet die Rückwärtssuche mit verschiedenen Suchmaschinen
        ' funktioniert nur in Kontakt- und Journaleinträgen
        ' Parameter:  Suchmaschine (Integer):  Kennnummer der Suchmaschinen
        '               = 0: GoYellow
        '               = 1: 11880
        '               = 2: RWSDasTelefonbuch
        '               = 3: TelSearch.ch

        Dim i As Integer, iTelNr As Integer      ' Zählvariablen
        Dim TelNr As String    ' Telefonnummer des zu Suchenden
        Dim vCard As String = C_DP.P_Def_StringEmpty    ' gefundene vCard
        Dim rws As Boolean   ' 'true' wenn was gefunden wurde
        Dim row(2) As String


        If olInsp IsNot Nothing Then
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


                    Dim alleTE() As String = {.AssistantTelephoneNumber, _
                                              .BusinessTelephoneNumber, _
                                              .Business2TelephoneNumber, _
                                              .CallbackTelephoneNumber, _
                                              .CarTelephoneNumber, _
                                              .CompanyMainTelephoneNumber, _
                                              .HomeTelephoneNumber, _
                                              .Home2TelephoneNumber, _
                                              .ISDNNumber, _
                                              .MobileTelephoneNumber, _
                                              .OtherTelephoneNumber, _
                                              .PagerNumber, _
                                              .PrimaryTelephoneNumber, _
                                              .RadioTelephoneNumber, _
                                              .BusinessFaxNumber, _
                                              .HomeFaxNumber, _
                                              .OtherFaxNumber, _
                                              .TelexNumber, _
                                              .TTYTDDTelephoneNumber}

                    For i = LBound(alleTE) To UBound(alleTE)
                        If Not alleTE(i) = C_DP.P_Def_StringEmpty Then
                            iTelNr += 1
                            row(0) = CStr(iTelNr)
                            row(1) = C_DP.P_Def_olTelNrTypen(i)
                            row(2) = alleTE(i)
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
                    If Not TelNr = C_DP.P_Def_StringEmpty Then
                        Select Case RWSAnbieter
                            Case RückwärtsSuchmaschine.RWSDasOertliche
                                rws = RWSDasOertiche(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWS11880
                                rws = RWS11880(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWSDasTelefonbuch
                                rws = RWSDasTelefonbuch(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWStelSearch
                                rws = RWStelsearch(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWSAlle
                                rws = RWSAlle(TelNr, vCard)
                        End Select
                        If rws Then
                            ' wenn erfolgreich, dann Ergebnisse aus vCard in den Kontakt übertragen
                            C_KF.vCard2Contact(vCard, oContact)
                            ' falls TelNr bei der Rückwärtssuche geändert wurde, diese nummer als Zweitnummer eintragen
                            If Not C_hf.nurZiffern(.BusinessTelephoneNumber) = C_hf.nurZiffern(TelNr) And Not .BusinessTelephoneNumber = C_DP.P_Def_StringEmpty Then
                                .Business2TelephoneNumber = C_hf.formatTelNr(TelNr)
                            ElseIf Not C_hf.nurZiffern(.HomeTelephoneNumber) = C_hf.nurZiffern(TelNr) And Not .HomeTelephoneNumber = C_DP.P_Def_StringEmpty Then
                                .Home2TelephoneNumber = C_hf.formatTelNr(TelNr)
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
                            Case RückwärtsSuchmaschine.RWSDasOertliche
                                rws = RWSDasOertiche(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWS11880
                                rws = RWS11880(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWSDasTelefonbuch
                                rws = RWSDasTelefonbuch(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWStelSearch
                                rws = RWStelsearch(TelNr, vCard)
                            Case RückwärtsSuchmaschine.RWSAlle
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
                C_hf.NAR(olJournal) : olJournal = Nothing
            End If
        End If
    End Sub

    Function RWS11880(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' führt die Rückwärtssuche über 'www.11880.com' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        RWS11880 = False

        Dim myurl As String         ' URL von 11880
        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring für TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Zählvariable

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SW1 As String = "<a class='micro_action vcf_enabled' rel='nofollow' href='"
        Const SW2 As String = "'"
        ' TelNr sichern, da sie unter Umständen verändert wird
        vCard = C_DP.P_Def_ErrorMinusTwo_String
        tmpTelNr = C_hf.nurZiffern(TelNr)
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        If Not Strings.Left(tmpTelNr, 2) = "11" Then

            Do
                ' Webseite für Rückwärtssuche aufrufen und herunterladen
                myurl = "http://classic.11880.com/inverssuche/index/search?method=searchSimple&_dvform_posted=1&phoneNumber=" & tmpTelNr
                htmlRWS = C_hf.httpGET(myurl, System.Text.Encoding.Default, HTMLFehler)
                If Not HTMLFehler Then
                    htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text)  '" enfernen
                    ' Link zum Herunterladen der vCard suchen
                    EintragsID = C_hf.StringEntnehmen(htmlRWS, SW1, SW2)
                    If Not EintragsID = C_DP.P_Def_ErrorMinusOne_String Then
                        myurl = "http://classic.11880.com" & EintragsID
                        vCard = C_hf.httpGET(myurl, System.Text.Encoding.Default, HTMLFehler)
                        If HTMLFehler Then C_hf.LogFile("FBError (RWS11880): " & Err.Number & " - " & Err.Description & " - " & myurl)
                    End If
                    ' Rückgabewert ermitteln
                    If Strings.Left(vCard, Len(C_DP.P_Def_Begin_vCard)) = C_DP.P_Def_Begin_vCard Then
                        RWS11880 = True
                    Else
                        vCard = C_DP.P_Def_ErrorMinusTwo_String
                    End If
                    i = i + 1
                    tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 1) & 0
                Else
                    C_hf.LogFile("FBError (RWS11880): " & Err.Number & " - " & Err.Description & " - " & myurl)
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

    Function RWSDasOertiche(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' führt die Rückwärtssuche über 'www.dastelefonbuch.de' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring für TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Zählvariable
        Dim baseurl As String

        RWSDasOertiche = False
        ' Webseite für Rückwärtssuche aufrufen und herunterladen
        vCard = C_DP.P_Def_ErrorMinusTwo_String
        tmpTelNr = C_hf.nurZiffern(TelNr)
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0

        baseurl = "http://www.dasoertliche.de?form_name="

        Do
            htmlRWS = C_hf.httpGET(baseurl & "search_nat&kw=" & tmpTelNr, System.Text.Encoding.Default, False)

            If Not htmlRWS = C_DP.P_Def_StringEmpty Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen
                ' Link zum Herunterladen der vCard suchen
                EintragsID = C_hf.StringEntnehmen(htmlRWS, "dasoertliche.de/?id=", "&")
                If Not EintragsID = C_DP.P_Def_ErrorMinusOne_String Then
                    vCard = C_hf.httpGET(baseurl & "vcard&id=" & EintragsID, System.Text.Encoding.Default, HTMLFehler)
                End If
            End If
            If HTMLFehler Then C_hf.LogFile("FBError (RWSDasOertiche): " & Err.Number & " - " & Err.Description)
            If Strings.Left(vCard, Len(C_DP.P_Def_Begin_vCard)) = C_DP.P_Def_Begin_vCard Then
                RWSDasOertiche = True
            Else
                vCard = C_DP.P_Def_ErrorMinusTwo_String
            End If
            i = i + 1
            tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0
        Loop Until RWSDasOertiche Or i = 3

    End Function

    Function RWSDasTelefonbuch(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' führt die Rückwärtssuche über 'www.dastelefonbuch.de' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        Dim myurl As String         ' URL von 11880
        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring für TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Zählvariable

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SW1 As String = "VCard?encurl="
        Const SW2 As String = "&"
        'Const SW3 As String = "'"

        RWSDasTelefonbuch = False
        ' Webseite für Rückwärtssuche aufrufen und herunterladen
        vCard = C_DP.P_Def_ErrorMinusTwo_String
        tmpTelNr = C_hf.nurZiffern(TelNr)
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0

        myurl = "http://www.dastelefonbuch.de/"
        Do
            htmlRWS = C_hf.httpGET(myurl & "?cmd=detail&kw=" & tmpTelNr, System.Text.Encoding.Default, False)

            If Not htmlRWS = C_DP.P_Def_StringEmpty Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen
                ' Link zum Herunterladen der vCard suchen
                EintragsID = C_hf.StringEntnehmen(htmlRWS, SW1, SW2)
                If Not EintragsID = C_DP.P_Def_ErrorMinusOne_String Then
                    'myurl = C_hf.StringEntnehmen(htmlRWS, SW3, Sw1, True)
                    vCard = C_hf.httpGET("http://www1.dastelefonbuch.de/" & SW1 & EintragsID, System.Text.Encoding.Default, HTMLFehler)
                End If
            End If
            If HTMLFehler Then C_hf.LogFile("FBError (RWSDasTelefonbuch): " & Err.Number & " - " & Err.Description & " - " & myurl)
            If Strings.Left(vCard, Len(C_DP.P_Def_Begin_vCard)) = C_DP.P_Def_Begin_vCard Then
                RWSDasTelefonbuch = True
            Else
                vCard = C_DP.P_Def_ErrorMinusTwo_String
            End If
            i = i + 1
            tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0
        Loop Until RWSDasTelefonbuch Or i = 3

    End Function

    Function RWStelsearch(ByRef TelNr As String, ByRef vCard As String) As Boolean
        ' Suchmaschienen Script für die Schweiz
        ' führt die Rückwärtssuche über 'www.telsearch.ch' durch
        ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
        '             vCard (String):  vCard falls was gefunden wurde (nur Rückgabewert)
        ' Rückgabewert (Boolean):      'true' wenn was gefunden wurde

        Dim myurl As String         ' URL von 11880
        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring für TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Zählvariable

        'Eindeutige Suchwörter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SW1 As String = "<a href='/vCard/"
        Const SW2 As String = "'"

        RWStelsearch = False
        ' Vorwahl erkennen
        ' TelNr sichern, da sie unter Umständen verändert wird
        vCard = C_DP.P_Def_ErrorMinusTwo_String
        tmpTelNr = C_hf.nurZiffern(TelNr)
        ' Suche wird unter Umständen mehrfach durchgeführt, da auch Firmennummern gefunden werden sollen.
        ' Dafür werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        Do
            ' Webseite für Rückwärtssuche aufrufen und herunterladen
            myurl = "http://tel.search.ch/result.html?name=&misc=&strasse=&ort=&kanton=&tel=" & tmpTelNr
            htmlRWS = C_hf.httpGET(myurl, System.Text.Encoding.UTF8, HTMLFehler)
            If Not HTMLFehler Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , vbTextCompare) '" enfernen

                ' Link zum Herunterladen der vCard suchen
                EintragsID = C_hf.StringEntnehmen(htmlRWS, SW1, SW2)
                If Not EintragsID = C_DP.P_Def_ErrorMinusOne_String Then
                    ' vCard herunterladen
                    myurl = Replace("http://tel.search.ch/vcard/" & EintragsID, "html", "vcf")
                    vCard = C_hf.httpGET(myurl, System.Text.Encoding.UTF8, HTMLFehler)
                End If

                ' Rückgabewert ermitteln
                If Strings.Left(vCard, Len(C_DP.P_Def_Begin_vCard)) = C_DP.P_Def_Begin_vCard Then
                    RWStelsearch = True
                Else
                    vCard = C_DP.P_Def_ErrorMinusTwo_String
                End If
                i = i + 1
                tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0
            Else
                RWStelsearch = False
                C_hf.LogFile("FBError (RWStelsearch): " & Err.Number & " - " & Err.Description & " - " & myurl)
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
        RWSAlle = RWSDasOertiche(TelNr, vCard)
        If RWSAlle Then Exit Function
        RWSAlle = RWS11880(TelNr, vCard)
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