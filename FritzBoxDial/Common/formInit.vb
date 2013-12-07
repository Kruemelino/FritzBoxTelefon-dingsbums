Friend Class formInit
    ' Klassen
    Private C_XML As MyXML
    Private C_Helfer As Helfer
    Private C_Crypt As Rijndael
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_AnrMon As AnrufMonitor
    Private C_FBox As FritzBox
    Private C_Kontakt As Contacts
    Private C_RWS As formRWSuche
    Private C_WählClient As Wählclient
    Private C_Phoner As PhonerInterface
    Private C_Config As formCfg
    Private F_JournalImport As formJournalimport
    'Strings
    Private DateiPfad As String
    Private SID As String


    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        ' Pfad zur Einstellungsdatei ermitteln
        DateiPfad = GetSetting("FritzBox", "Optionen", "TBxml", "-1")
        If Not IO.File.Exists(DateiPfad) Then DateiPfad = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Fritz!Box Telefon-dingsbums\FritzOutlook.xml"

        ' Klasse zum IO-der INI-Struktiur erstellen
        C_XML = New MyXML(DateiPfad)

        ' Klasse für Verschlüsselung erstellen
        C_Crypt = New Rijndael

        ' Klasse für Helferfunktionen erstellen
        C_Helfer = New Helfer(DateiPfad, C_XML, C_Crypt)

        ' Klasse für die Kontakte generieren
        C_Kontakt = New Contacts(C_XML, C_Helfer)

        ' Klasse für die Rückwärtssuche generieren
        C_RWS = New formRWSuche(C_Helfer, C_Kontakt)

        ' Klasse für die OutlookInterface generieren
        C_OlI = New OutlookInterface(C_Kontakt, C_Helfer, DateiPfad)

        ' Klasse für das PhonerInterface generieren
        C_Phoner = New PhonerInterface(C_Helfer, C_XML, C_Crypt)

        If PrüfeAddin() Then

            ' Wenn PrüfeAddin mit Dialog (Usereingaben) abgeschlossen wurde, exsistiert C_FBox schon 
            If C_FBox Is Nothing Then C_FBox = New FritzBox(C_XML, C_Helfer, C_Crypt)
            ThisAddIn.P_FritzBox = C_FBox

            C_GUI = New GraphicalUserInterface(C_Helfer, C_XML, C_Crypt, DateiPfad, C_RWS, C_Kontakt, C_Phoner)


            C_WählClient = New Wählclient(C_XML, C_Helfer, C_Kontakt, C_GUI, C_OlI, C_FBox, C_Phoner)
            ThisAddIn.P_WClient = C_WählClient

            C_AnrMon = New AnrufMonitor(C_RWS, C_XML, C_Helfer, C_Kontakt, C_GUI, C_OlI, C_FBox.P_FBAddr)
            ThisAddIn.P_AnrMon = C_AnrMon

            C_Config = New formCfg(C_GUI, C_XML, C_Helfer, C_Crypt, C_AnrMon, C_FBox, C_OlI, C_Kontakt, C_Phoner)
            ThisAddIn.P_Config = C_Config

            With C_GUI
                .P_AnrufMonitor = C_AnrMon
                .P_OlInterface = C_OlI
                .P_WählKlient = C_WählClient
                .P_FritzBox = C_FBox
            End With

            ThisAddIn.P_GUI = C_GUI
            ThisAddIn.P_Dateipfad = DateiPfad
            ThisAddIn.P_XML = C_XML
            ThisAddIn.P_hf = C_Helfer
            ThisAddIn.P_KontaktFunktionen = C_Kontakt

            If C_XML.P_CBJImport And C_XML.P_CBUseAnrMon Then F_JournalImport = New formJournalimport(C_AnrMon, C_Helfer, C_XML, False)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Function PrüfeAddin() As Boolean
        Dim Rückgabe As Boolean = False

        If C_XML.P_TBPasswort = vbNullString Or C_XML.P_TBVorwahl = vbNullString Or GetSetting("FritzBox", "Optionen", "Zugang", "-1") = "-1" Then
            Rückgabe = False
            Me.ShowDialog()
            Rückgabe = True 'PrüfeAddin()
        Else
            Rückgabe = True
        End If
        Return Rückgabe

    End Function

    Private Sub BFBAdr_Click(sender As Object, e As EventArgs) Handles BFBAdr.Click
        Dim tmpstr As String = Me.TBFritzBoxAdr.Text
        If C_Helfer.Ping(tmpstr) Or Me.CBForceFBAddr.Checked Then
            Me.TBFritzBoxAdr.Text = tmpstr
            If Not InStr(C_Helfer.httpRead("http://" & tmpstr & "/login_sid.lua", System.Text.Encoding.UTF8, Nothing), "<SID>0000000000000000</SID>", CompareMethod.Text) = 0 Then
                C_XML.P_TBFBAdr = tmpstr
                C_XML.P_CBForceFBAddr = Me.CBForceFBAddr.Checked
                Me.TBFBPW.Enabled = True
                Me.TBFBUser.Enabled = True
                Me.LabelFBUser.Enabled = True
                Me.LFBPW.Enabled = True
                Me.TBFritzBoxAdr.Enabled = False
                Me.BFBAdr.Enabled = False
                Me.LFBAdr.Enabled = False
                Me.CBForceFBAddr.Enabled = False
                Me.LMessage.Text = "Eine Fritz!Box unter der IP " & tmpstr & " gefunden."
            Else
                Me.LMessage.Text = "Keine Fritz!Box unter der angegebenen IP gefunden."
            End If
        Else
            Me.CBForceFBAddr.Enabled = True
            Me.TBFritzBoxAdr.Text = "192.168.178.1"
            tmpstr = Me.TBFritzBoxAdr.Text
            Me.LMessage.Text = "Keine Gegenstelle unter der angegebenen IP gefunden."
        End If
    End Sub

    Private Sub BFBPW_Click(sender As Object, e As EventArgs) Handles BFBPW.Click
        Dim fw550 As Boolean
        C_FBox = New FritzBox(C_XML, C_Helfer, C_Crypt)
        C_XML.P_TBBenutzer = Me.TBFBUser.Text
        C_XML.P_TBPasswort = C_Crypt.EncryptString128Bit(Me.TBFBPW.Text, "Fritz!Box Script")
        SaveSetting("FritzBox", "Optionen", "Zugang", "Fritz!Box Script")
        C_Helfer.KeyChange()
        SID = C_FBox.FBLogIn(fw550)
        If Not SID = C_FBox.P_DefaultSID Then
            Me.TBFBPW.Enabled = False
            Me.LFBPW.Enabled = False
            Me.BFBPW.Enabled = False
            Me.TBFBUser.Enabled = False
            Me.LabelFBUser.Enabled = False
            Me.LVorwahl.Enabled = True
            Me.LLandesvorwahl.Enabled = True
            Me.TBVorwahl.Enabled = True
            Me.TBLandesvorwahl.Enabled = True
            Me.LMessage.Text = "Das Anmelden an der Fritz!Box war erfolgreich."
        Else
            Me.LMessage.Text = "Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich."
        End If
    End Sub

    Private Sub TBFBPW_TextChanged(sender As Object, e As EventArgs) Handles TBFBPW.TextChanged
        Me.BFBPW.Enabled = Not Me.TBFBPW.Text.Length = 0
    End Sub

    Private Sub BtELeINLESEN_Click(sender As Object, e As EventArgs) Handles BTelEinlesen.Click
        Me.LVorwahl.Enabled = False
        Me.LLandesvorwahl.Enabled = False
        Me.TBVorwahl.Enabled = False
        Me.TBLandesvorwahl.Enabled = False
        Me.BTelEinlesen.Text = "Bitte warten..."
        Me.BTelEinlesen.Enabled = False

        C_XML.P_TBVorwahl = Me.TBVorwahl.Text
        C_XML.P_TBLandesVW = Me.TBLandesvorwahl.Text
        C_FBox.P_SpeichereDaten = True
        C_FBox.FritzBoxDaten()

        Me.CLBTelNr.Enabled = True
        Me.LTelListe.Enabled = True

        CLBtelnrAusfüllen()
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As EventArgs) Handles TBVorwahl.TextChanged, TBLandesvorwahl.TextChanged
        Me.BTelEinlesen.Enabled = (Not Me.TBVorwahl.Text.Length = 0) And (Not Me.TBLandesvorwahl.Text.Length = 0)
    End Sub

    Sub CLBtelnrAusfüllen()
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*[starts-with(name(.), ""POTS"") or starts-with(name(.), ""MSN"") or starts-with(name(.), ""SIP"")]")

            Dim TelNrString() As String = Split("Alle Telefonnummern;" & C_XML.Read(xPathTeile, ""), ";", , CompareMethod.Text)
            TelNrString = (From x In TelNrString Select x Distinct).ToArray 'Doppelte entfernen
            TelNrString = (From x In TelNrString Where Not x Like "" Select x).ToArray ' Leere entfernen
            Me.CLBTelNr.Items.Clear()

            For Each TelNr In TelNrString
                Me.CLBTelNr.Items.Add(TelNr)
            Next
        End With
        Me.CLBTelNr.SetItemChecked(0, Me.CLBTelNr.CheckedItems.Count = Me.CLBTelNr.Items.Count - 1)
    End Sub

    Private Sub CLBTelNr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CLBTelNr.SelectedIndexChanged

        Dim alle As Boolean = True
        With Me.CLBTelNr
            Select Case .SelectedIndex
                Case 0
                    For i = 1 To .Items.Count - 1
                        .SetItemChecked(i, .GetItemChecked(0))
                    Next
                Case 1 To .Items.Count - 1
                    For i = 1 To .Items.Count - 1
                        If .GetItemChecked(i) = False Then
                            alle = False
                            Exit For
                        End If
                    Next
                    .SetItemChecked(0, alle)
            End Select
        End With
        Me.BFertigstellen.Enabled = Not CLBTelNr.CheckedItems.Count = 0
    End Sub

    Private Sub BFertigstellen_Click(sender As Object, e As EventArgs) Handles BFertigstellen.Click
        Dim CheckTelNr As Windows.Forms.CheckedListBox.CheckedItemCollection = Me.CLBTelNr.CheckedItems
        If CheckTelNr.Count = 0 Then
            For i = 0 To Me.CLBTelNr.Items.Count - 1
                Me.CLBTelNr.SetItemChecked(i, True)
            Next
            CheckTelNr = Me.CLBTelNr.CheckedItems
        End If

        Dim xPathTeile As New ArrayList
        Dim tmpTeile As String = vbNullString
        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")

            For i = 1 To Me.CLBTelNr.Items.Count - 1
                tmpTeile += ". = " & """" & Me.CLBTelNr.Items(i).ToString & """" & " or "
            Next
            tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
            .Add("[" & tmpTeile & "]")
            C_XML.WriteAttribute(xPathTeile, "Checked", "0")
            tmpTeile = vbNullString
            For i = 0 To CheckTelNr.Count - 1
                tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
            Next
            tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
            .Item(.Count - 1) = "[" & tmpTeile & "]"
            C_XML.WriteAttribute(xPathTeile, "Checked", "1")
        End With

        Me.LMessage.Text = "Fertig"
        Me.BSchließen.Enabled = True
    End Sub

    Private Sub BSchließen_Click(sender As Object, e As EventArgs) Handles BSchließen.Click

        Me.Dispose()
        Me.Close()

    End Sub

#Region "Fritz!Box Tests"

    'Private Function FritzBoxVorhanden(IPAddresse As String) As Boolean

    '    If C_Helfer.Ping(IPAddresse) Then Return True
    '    If Not InStr(C_Helfer.httpRead("http://" & IPAddresse & "/login_sid.lua", System.Text.Encoding.UTF8), "<SID>0000000000000000</SID>", CompareMethod.Text) = 0 Then Return True
    '    C_Helfer.LogFile("Es konnte keine Fritz!Box im Netzwerk unter der Adresse """ & IPAddresse & """ gefunden werden.")
    '    Return False
    'End Function

#End Region
End Class