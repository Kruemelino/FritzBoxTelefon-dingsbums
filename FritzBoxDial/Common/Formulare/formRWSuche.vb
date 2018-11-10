Imports System.Windows.Forms

Friend Enum R�ckw�rtsSuchmaschine
    RWSDasOertliche = 0
    RWS11880 = 1
    RWSDasTelefonbuch = 2
    RWStelSearch = 3
    RWSAlle = 4
End Enum

Friend Class formRWSuche
    Private C_hf As Helfer
    Private C_KF As KontaktFunktionen
    Private C_DP As DataProvider
    Private C_XML As XML
    Private HTMLFehler As Boolean


    Public Sub New(ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As KontaktFunktionen, _
                   ByVal DataproviderKlasse As DataProvider, _
                   ByVal XMLKlasse As XML)
        ' Dieser Aufruf ist f�r den Windows Form-Designer erforderlich.
        InitializeComponent()
        C_hf = HelferKlasse
        C_KF = KontaktKlasse
        C_DP = DataproviderKlasse
        C_XML = XMLKlasse
        ' F�gen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    ''' <summary>
    ''' F�hrt eine R�ckw�rtssuche durch. Funktion wird durch den Anrufmonitor gestartet. Der RWSIndex wird zun�chst gepr�ft, danach
    ''' die ausgew�hlte RWS.
    ''' </summary>
    ''' <param name="Telefonat">Telefonat, das gepr�ft werden soll</param>
    ''' <returns>True, wenn gefunden. Neue Daten werden in dem Telefonat abgelegt.</returns>
    Friend Function AnrMonRWS(ByRef Telefonat As C_Telefonat) As Boolean
        AnrMonRWS = False
        With Telefonat

            Dim xPathTeile As New ArrayList

            If C_DP.P_CBRWSIndex Then
                .vCard = DataProvider.P_Def_ErrorMinusTwo_String
                ' RWS-Index �berpr�fen
                With xPathTeile
                    .Clear()
                    .Add("CBRWSIndex")
                    .Add("Eintrag[@ID=""" & Telefonat.TelNr & """]")
                End With
                .vCard = C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String)
            Else
                .vCard = DataProvider.P_Def_ErrorMinusOne_String
            End If
            ' Drei m�gliche R�ckgaben
            ' Fall 1: Eine fr�here RWS hat ein Ergebnis geliefert. R�ckgabe: g�ltige vCard
            ' Fall 2: Eine fr�here RWS hat kein Ergebnis geliefert. R�ckgabe: -2
            ' Fall 3: Es gibt keinen Eintrag. R�ckgabe: -1
            'ToDo:
            ' Fall 1: vCard �bernehmen
            ' Fall 2: keine erneute RWS durchf�hren
            ' Fall 3: RWS durchf�hren
            Select Case .vCard
                Case DataProvider.P_Def_ErrorMinusTwo_String ' Fall 2: Eine fr�here RWS hat kein Ergebnis geliefert.
                    '.vCard = DataProvider.P_Def_ErrorMinusTwo_String
                Case DataProvider.P_Def_ErrorMinusOne_String ' Fall 3: Es gibt keinen Eintrag.
                    '
                    Select Case CType(C_DP.P_ComboBoxRWS, R�ckw�rtsSuchmaschine) ' Fall 3: Es gibt keinen Eintrag.
                        Case R�ckw�rtsSuchmaschine.RWSDasOertliche
                            AnrMonRWS = RWSDasOertiche(.TelNr, .vCard)
                            'Case R�ckw�rtsSuchmaschine.RWS11880
                            '    AnrMonRWS = RWS11880(.TelNr, .vCard)
                            'Case R�ckw�rtsSuchmaschine.RWSDasTelefonbuch
                            '    AnrMonRWS = RWSDasTelefonbuch(.TelNr, .vCard)
                        Case R�ckw�rtsSuchmaschine.RWStelSearch
                            AnrMonRWS = RWStelsearch(.TelNr, .vCard)
                            'Case R�ckw�rtsSuchmaschine.RWSAlle
                            '    AnrMonRWS = RWSAlle(.TelNr, .vCard)
                    End Select
                    If C_DP.P_CBRWSIndex Then
                        xPathTeile.Item(xPathTeile.Count - 1) = "Eintrag"
                        C_XML.Write(C_DP.XMLDoc, xPathTeile, .vCard, "ID", .TelNr)
                    End If
                Case Else ' Fall 1: Eine fr�here RWS hat ein Ergebnis geliefert. 
                    AnrMonRWS = True
            End Select

        End With
    End Function

    ''' <summary>
    ''' Startet die R�ckw�rtssuche mit verschiedenen Suchmaschinen. 
    ''' </summary>
    ''' <param name="RWSAnbieter">Die zu verwendende R�ckw�rtssuchmaschine</param>
    ''' <param name="olInsp">Outlook Inspector Fenster</param>
    ''' <remarks>Funktioniert nur in Kontakt- und Journaleintr�gen</remarks>
    Public Sub R�ckw�rtssuche(ByVal RWSAnbieter As R�ckw�rtsSuchmaschine, ByVal olInsp As Outlook.Inspector)

        Dim i As Integer, iTelNr As Integer      ' Z�hlvariablen
        Dim TelNr As String    ' Telefonnummer des zu Suchenden
        Dim vCard As String = DataProvider.P_Def_LeerString    ' gefundene vCard
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
                        If Not alleTE(i) = DataProvider.P_Def_LeerString Then
                            iTelNr += 1
                            row(0) = CStr(iTelNr)
                            row(1) = DataProvider.P_Def_olTelNrTypen(i)
                            row(2) = alleTE(i)
                            Me.ListTel.Rows.Add(row)
                        End If
                    Next

                    Me.ListTel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                    Me.ListTel.ClearSelection()
                    Me.ButtonSuchen.Focus()
                    ' 'formRWSuche' modal einblenden (nach schlie�en des Fensters geht es hier weiter)
                    Me.ShowDialog()

                    If Me.ListTel.SelectedRows.Count > 0 Then
                        TelNr = Me.ListTel.SelectedRows.Item(0).Cells(2).Value.ToString ' TelNr aus Liste oder Eingabefeld entnehmen
                    Else
                        TelNr = Me.DirektTel.Text
                    End If
                    ' je nach 'Suchmaschine' Suche durchf�hren
                    If Not TelNr = DataProvider.P_Def_LeerString Then
                        Select Case RWSAnbieter
                            Case R�ckw�rtsSuchmaschine.RWSDasOertliche
                                rws = RWSDasOertiche(TelNr, vCard)
                                'Case R�ckw�rtsSuchmaschine.RWS11880
                                '    rws = RWS11880(TelNr, vCard)
                                'Case R�ckw�rtsSuchmaschine.RWSDasTelefonbuch
                                '    rws = RWSDasTelefonbuch(TelNr, vCard)
                            Case R�ckw�rtsSuchmaschine.RWStelSearch
                                rws = RWStelsearch(TelNr, vCard)
                                'Case R�ckw�rtsSuchmaschine.RWSAlle
                                '    rws = RWSAlle(TelNr, vCard)
                        End Select
                        If rws Then
                            ' wenn erfolgreich, dann Ergebnisse aus vCard in den Kontakt �bertragen
                            C_KF.vCard2Contact(vCard, oContact)
                            ' falls TelNr bei der R�ckw�rtssuche ge�ndert wurde, diese nummer als Zweitnummer eintragen
                            If Not C_hf.nurZiffern(.BusinessTelephoneNumber) = C_hf.nurZiffern(TelNr) And Not .BusinessTelephoneNumber = DataProvider.P_Def_LeerString Then
                                .Business2TelephoneNumber = C_hf.FormatTelNr(TelNr)
                            ElseIf Not C_hf.nurZiffern(.HomeTelephoneNumber) = C_hf.nurZiffern(TelNr) And Not .HomeTelephoneNumber = DataProvider.P_Def_LeerString Then
                                .Home2TelephoneNumber = C_hf.FormatTelNr(TelNr)
                            End If
                            .Body = "R�ckw�rtssuche erfolgreich" & vbCrLf & "Achtung! Unter Umst�nden werden vorhandene Daten �berschrieben. Wir �bernehmen keine Haftung f�r verloren gegangene Daten und f�r falsche Informationen, die die R�ckw�rtssuche liefert! Nutzung auf eigene Gefahr!" & vbCrLf & .Body
                        Else
                            .Body = "R�ckw�rtssuche nicht erfolgreich" & vbCrLf & .Body
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
                        ' je nach 'Suchmaschine' Suche durchf�hren
                        Select Case RWSAnbieter
                            Case R�ckw�rtsSuchmaschine.RWSDasOertliche
                                rws = RWSDasOertiche(TelNr, vCard)
                                'Case R�ckw�rtsSuchmaschine.RWS11880
                                '    rws = RWS11880(TelNr, vCard)
                                'Case R�ckw�rtsSuchmaschine.RWSDasTelefonbuch
                                '    rws = RWSDasTelefonbuch(TelNr, vCard)
                            Case R�ckw�rtsSuchmaschine.RWStelSearch
                                rws = RWStelsearch(TelNr, vCard)
                                'Case R�ckw�rtsSuchmaschine.RWSAlle
                                '    rws = RWSAlle(TelNr, vCard)
                        End Select
                        If rws Then
                            ' wenn erfolgreich, dann Ergebnis (vCard) dem .Body hinzuf�gen
                            .Body = .Body & vbCrLf & vbCrLf & vCard & vbCrLf
                        Else
                            .Body = .Body & vbCrLf & "R�ckw�rtssuche nicht erfolgreich: Es wurden keine Eintr�ge gefunden." & vbCrLf
                        End If
                    End If
                End With
                C_hf.NAR(olJournal) : olJournal = Nothing
            End If
        End If
    End Sub

    ' ''' <summary>
    ' ''' F�hrt die R�ckw�rtssuche �ber 'www.11880.com' durch.
    ' ''' </summary>
    ' ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ' ''' <param name="vCard">vCard falls was gefunden wurde als R�ckgabewert</param>
    ' ''' <returns>'true' wenn was gefunden wurde</returns>
    'Function RWS11880(ByRef TelNr As String, ByRef vCard As String) As Boolean

    '    RWS11880 = False

    '    Dim myurl As String         ' URL von 11880
    '    Dim EintragsID As String    ' Hilfsstring
    '    Dim tmpTelNr As String      ' Hilfsstring f�r TelNr
    '    Dim htmlRWS As String       ' Inhalt der Webseite
    '    Dim i As Integer            ' Z�hlvariable

    '    'Eindeutige Suchw�rter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
    '    Const SW1 As String = "<a class='micro_action vcf_enabled' rel='nofollow' href='"
    '    Const SW2 As String = "'"
    '    ' TelNr sichern, da sie unter Umst�nden ver�ndert wird
    '    vCard = DataProvider.P_Def_ErrorMinusTwo_String
    '    tmpTelNr = C_hf.nurZiffern(TelNr)
    '    ' Suche wird unter Umst�nden mehrfach durchgef�hrt, da auch Firmennummern gefunden werden sollen.
    '    ' Daf�r werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
    '    ' Schleife wird maximall drei mal durchlaufen
    '    i = 0
    '    If Not Strings.Left(tmpTelNr, 2) = "11" Then

    '        Do
    '            ' Webseite f�r R�ckw�rtssuche aufrufen und herunterladen
    '            myurl = "http://classic.11880.com/inverssuche/index/search?method=searchSimple&_dvform_posted=1&phoneNumber=" & tmpTelNr
    '            htmlRWS = C_hf.httpGET(myurl, System.Text.Encoding.Default, HTMLFehler)
    '            If Not HTMLFehler Then
    '                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text)  '" enfernen
    '                ' Link zum Herunterladen der vCard suchen
    '                EintragsID = C_hf.StringEntnehmen(htmlRWS, SW1, SW2)
    '                If Not EintragsID = DataProvider.P_Def_ErrorMinusOne_String Then
    '                    myurl = "http://classic.11880.com" & EintragsID
    '                    vCard = C_hf.httpGET(myurl, System.Text.Encoding.Default, HTMLFehler)
    '                    If HTMLFehler Then C_hf.LogFile("FBError (RWS11880): " & Err.Number & " - " & Err.Description & " - " & myurl)
    '                End If
    '                ' R�ckgabewert ermitteln
    '                If Strings.Left(vCard, Len(DataProvider.P_Def_Begin_vCard)) = DataProvider.P_Def_Begin_vCard Then
    '                    RWS11880 = True
    '                Else
    '                    vCard = DataProvider.P_Def_ErrorMinusTwo_String
    '                End If
    '                i = i + 1
    '                tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 1) & 0
    '            Else
    '                RWS11880 = False
    '                C_hf.LogFile("FBError (RWS11880): " & Err.Number & " - " & Err.Description & " - " & myurl)
    '            End If
    '        Loop Until RWS11880 Or i = 3 Or HTMLFehler

    '    End If
    '    ' Besonderheit bei '11880': Vor- und Nachname sind in vCard separat angegeben
    '    ' wenn kein Vorname vorhanden ist, dann "muss" es sich um eine Firma handeln
    '    ' dann wird der volle Name in der vCard (FN) in den Firmennamen (ORG) �bertragen
    '    If RWS11880 Then
    '        If InStr(1, ReadFromVCard(vCard, "N", ""), ";;;;", CompareMethod.Text) > 0 Then ''''''''''''' beim Debuggen aufpassen, dass auch das richtige Ergebnis ausgeworfen wird!!
    '            vCard = Replace(vCard, Chr(10) & "FN:", Chr(10) & "ORG:", , , CompareMethod.Text)
    '        End If
    '    End If
    'End Function

    ''' <summary>
    ''' F�hrt die R�ckw�rtssuche �ber 'www.dasoertliche.de' durch.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ''' <param name="vCard">vCard falls was gefunden wurde als R�ckgabewert</param>
    ''' <returns>'true' wenn was gefunden wurde</returns>
    Function RWSDasOertiche(ByRef TelNr As String, ByRef vCard As String) As Boolean

        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring f�r TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Z�hlvariable
        Dim baseurl As String

        RWSDasOertiche = False
        ' Webseite f�r R�ckw�rtssuche aufrufen und herunterladen
        vCard = DataProvider.P_Def_ErrorMinusTwo_String
        tmpTelNr = C_hf.nurZiffern(TelNr)
        ' Suche wird unter Umst�nden mehrfach durchgef�hrt, da auch Firmennummern gefunden werden sollen.
        ' Daf�r werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0

        baseurl = "https://www.dasoertliche.de?form_name="

        Do
            htmlRWS = C_hf.httpGET(baseurl & "search_nat&kw=" & tmpTelNr, Encoding.Default, HTMLFehler)
            If Not HTMLFehler Then
                If Not htmlRWS = DataProvider.P_Def_LeerString Then
                    htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen
                    ' Link zum Herunterladen der vCard suchen
                    EintragsID = C_hf.StringEntnehmen(htmlRWS, "dasoertliche.de/?id=", "&")
                    If Not EintragsID = DataProvider.P_Def_ErrorMinusOne_String Then

                        vCard = C_hf.httpGET(baseurl & "vcard&id=" & EintragsID, Encoding.Default, HTMLFehler)
                        If HTMLFehler Then C_hf.LogFile("FBError (RWSDasOertiche): " & Err.Number & " - " & Err.Description & " - " & baseurl & "vcard&id=" & EintragsID)
                    End If
                End If

                If Strings.Left(vCard, Len(DataProvider.P_Def_Begin_vCard)) = DataProvider.P_Def_Begin_vCard Then
                    RWSDasOertiche = True
                Else
                    vCard = DataProvider.P_Def_ErrorMinusTwo_String
                End If
                i = i + 1
                tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0
            Else
                RWSDasOertiche = False
                C_hf.LogFile("FBError (RWSDasOertiche): " & Err.Number & " - " & Err.Description)
            End If

        Loop Until RWSDasOertiche Or i = 3 Or HTMLFehler

    End Function

    ' ''' <summary>
    ' ''' F�hrt die R�ckw�rtssuche �ber 'www.dastelefonbuch.de' durch.
    ' ''' </summary>
    ' ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ' ''' <param name="vCard">vCard falls was gefunden wurde als R�ckgabewert</param>
    ' ''' <returns>'true' wenn was gefunden wurde</returns>
    'Function RWSDasTelefonbuch(ByRef TelNr As String, ByRef vCard As String) As Boolean
    '    ' f�hrt die R�ckw�rtssuche �ber 'www.dastelefonbuch.de' durch
    '    ' Parameter:  TelNr (String):  Telefonnummer des zu Suchenden
    '    '             vCard (String):  vCard falls was gefunden wurde (nur R�ckgabewert)
    '    ' R�ckgabewert (Boolean):      'true' wenn was gefunden wurde

    '    Dim myurl As String         ' URL von 11880
    '    Dim EintragsID As String    ' Hilfsstring
    '    Dim tmpTelNr As String      ' Hilfsstring f�r TelNr
    '    Dim htmlRWS As String       ' Inhalt der Webseite
    '    Dim i As Integer            ' Z�hlvariable

    '    'Eindeutige Suchw�rter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
    '    Const SW1 As String = "VCard?encurl="
    '    Const SW2 As String = "&"
    '    'Const SW3 As String = "'"

    '    RWSDasTelefonbuch = False
    '    ' Webseite f�r R�ckw�rtssuche aufrufen und herunterladen
    '    vCard = DataProvider.P_Def_ErrorMinusTwo_String
    '    tmpTelNr = C_hf.nurZiffern(TelNr)
    '    ' Suche wird unter Umst�nden mehrfach durchgef�hrt, da auch Firmennummern gefunden werden sollen.
    '    ' Daf�r werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
    '    ' Schleife wird maximall drei mal durchlaufen
    '    i = 0

    '    myurl = "http://www.dastelefonbuch.de/"
    '    Do
    '        htmlRWS = C_hf.httpGET(myurl & "?cmd=detail&kw=" & tmpTelNr, System.Text.Encoding.Default, False)
    '
    '        If Not HTMLFehler Then
    '            htmlRWS = Replace(htmlRWS, Chr(34), "'", , , CompareMethod.Text) '" enfernen
    '            ' Link zum Herunterladen der vCard suchen
    '            EintragsID = C_hf.StringEntnehmen(htmlRWS, SW1, SW2)
    '            If Not EintragsID = DataProvider.P_Def_ErrorMinusOne_String Then
    '                'myurl = C_hf.StringEntnehmen(htmlRWS, SW3, Sw1, True)
    '                vCard = C_hf.httpGET("http://www1.dastelefonbuch.de/" & SW1 & EintragsID, System.Text.Encoding.Default, HTMLFehler)
    '            End If
    '        Else
    '            C_hf.LogFile("FBError (RWSDasTelefonbuch): " & Err.Number & " - " & Err.Description & " - " & myurl)
    '        End If

    '        If Strings.Left(vCard, Len(DataProvider.P_Def_Begin_vCard)) = DataProvider.P_Def_Begin_vCard Then
    '            RWSDasTelefonbuch = True
    '        Else
    '            vCard = DataProvider.P_Def_ErrorMinusTwo_String
    '        End If
    '        i = i + 1
    '        tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0
    '    Loop Until RWSDasTelefonbuch Or i = 3

    'End Function

    ''' <summary>
    ''' F�hrt die R�ckw�rtssuche �ber 'www.telsearch.ch' durch.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ''' <param name="vCard">vCard falls was gefunden wurde als R�ckgabewert</param>
    ''' <returns>'true' wenn was gefunden wurde</returns>
    ''' <remarks>Nur f�r die Schweiz</remarks>
    Function RWStelsearch(ByRef TelNr As String, ByRef vCard As String) As Boolean

        Dim myurl As String         ' URL von 11880
        Dim EintragsID As String    ' Hilfsstring
        Dim tmpTelNr As String      ' Hilfsstring f�r TelNr
        Dim htmlRWS As String       ' Inhalt der Webseite
        Dim i As Integer            ' Z�hlvariable

        'Eindeutige Suchw�rter, nach denen die gesuchten Daten anfangen (ohne ", chr(09), chr(10) und chr(13)):
        Const SW1 As String = "<a href='/vCard/"
        Const SW2 As String = "'"

        RWStelsearch = False
        ' Vorwahl erkennen
        ' TelNr sichern, da sie unter Umst�nden ver�ndert wird
        vCard = DataProvider.P_Def_ErrorMinusTwo_String
        tmpTelNr = C_hf.nurZiffern(TelNr)
        ' Suche wird unter Umst�nden mehrfach durchgef�hrt, da auch Firmennummern gefunden werden sollen.
        ' Daf�r werden die letzten beiden Ziffern von TelNr durch '0' ersetzt und noch einmal gesucht.
        ' Schleife wird maximall drei mal durchlaufen
        i = 0
        Do
            ' Webseite f�r R�ckw�rtssuche aufrufen und herunterladen
            myurl = "http://tel.search.ch/result.html?name=&misc=&strasse=&ort=&kanton=&tel=" & tmpTelNr
            htmlRWS = C_hf.httpGET(myurl, Encoding.UTF8, HTMLFehler)
            If Not HTMLFehler Then
                htmlRWS = Replace(htmlRWS, Chr(34), "'", , , vbTextCompare) '" enfernen

                ' Link zum Herunterladen der vCard suchen
                EintragsID = C_hf.StringEntnehmen(htmlRWS, SW1, SW2)
                If Not EintragsID = DataProvider.P_Def_ErrorMinusOne_String Then
                    ' vCard herunterladen
                    myurl = Replace("http://tel.search.ch/vcard/" & EintragsID, "html", "vcf")
                    vCard = C_hf.httpGET(myurl, Encoding.UTF8, HTMLFehler)
                End If

                ' R�ckgabewert ermitteln
                If Strings.Left(vCard, Len(DataProvider.P_Def_Begin_vCard)) = DataProvider.P_Def_Begin_vCard Then
                    RWStelsearch = True
                Else
                    vCard = DataProvider.P_Def_ErrorMinusTwo_String
                End If
                i = i + 1
                tmpTelNr = Strings.Left(tmpTelNr, Len(tmpTelNr) - 2) & 0
            Else
                RWStelsearch = False
                C_hf.LogFile("FBError (RWStelsearch): " & Err.Number & " - " & Err.Description & " - " & myurl)
            End If
        Loop Until RWStelsearch Or i = 3 Or HTMLFehler

        ' Besonderheit bei 'telsearch': Vor- und Nachname sind in vCard separat angegeben
        ' wenn kein Vorname vorhanden ist, dann "muss" es sich um eine Firma handeln
        ' dann wird der volle Name in der vCard (FN) in den Firmennamen (ORG) �bertragen
        If RWStelsearch Then
            If InStr(1, ReadFromVCard(vCard, "N", ""), ";;;;", CompareMethod.Text) > 0 Then
                vCard = Replace(vCard, Chr(10) & "FN:", Chr(10) & "ORG:", , , vbTextCompare)
            End If
        End If

    End Function

    ' ''' <summary>
    ' ''' F�hrt die R�ckw�rtssuche mit allen vorhanden R�ckw�rtssuchmaschinen durch, so lange bis etwas gefunden wurde.
    ' ''' </summary>
    ' ''' <param name="TelNr">Telefonnummer des zu Suchenden</param>
    ' ''' <param name="vCard">vCard falls was gefunden wurde als R�ckgabewert</param>
    ' ''' <returns>'true' wenn was gefunden wurde</returns>
    'Function RWSAlle(ByRef TelNr As String, ByRef vCard As String) As Boolean

    '    RWSAlle = RWSDasOertiche(TelNr, vCard)

    '    'If Not RWSAlle Then
    '    '    RWSAlle = RWS11880(TelNr, vCard)
    '    'End If

    '    'If Not RWSAlle Then
    '    '    RWSAlle = RWSDasTelefonbuch(TelNr, vCard)
    '    'End If

    '    If Not RWSAlle Then
    '        RWSAlle = RWStelsearch(TelNr, vCard)
    '    End If
    'End Function

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