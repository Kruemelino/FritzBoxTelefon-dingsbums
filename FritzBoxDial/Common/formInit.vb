Public Class formInit
    ' Klassen
    'Private C_ini As InI
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
    Private C_JournalXML As JournalXML

    Private WithEvents emc As New EventMulticaster

    'Strings
    Private DateiPfad As String
    Private SID As String


    Public Sub New()
        Dim UseAnrMon As Boolean

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

        ' Klasse für das Journal-XML generieren
        C_JournalXML = New JournalXML(DateiPfad, C_Helfer)

        If PrüfeAddin() Then
            UseAnrMon = CBool(C_XML.Read("Optionen", "CBUseAnrMon", "True"))

            ' Wenn PrüfeAddin mit Dialog (Usereingaben) abgeschlossen wurde, exsistiert C_FBox schon 
            If C_FBox Is Nothing Then C_FBox = New FritzBox(C_XML, C_Helfer, C_Crypt, False, emc)

            C_GUI = New GraphicalUserInterface(C_Helfer, C_XML, C_Crypt, DateiPfad, C_WählClient, C_RWS, C_AnrMon, C_Kontakt, C_FBox, C_OlI, C_Phoner)

            C_WählClient = New Wählclient(C_XML, C_Helfer, C_Kontakt, C_GUI, C_OlI, C_FBox, C_Phoner)

            C_AnrMon = New AnrufMonitor(C_RWS, UseAnrMon, C_XML, C_Helfer, C_Kontakt, C_GUI, C_OlI, C_JournalXML, C_FBox.GetFBAddr)

            C_GUI.SetOAWOF(C_WählClient, C_AnrMon, C_FBox, C_OlI)

            ThisAddIn.Dateipfad = DateiPfad
            ThisAddIn.XML = C_XML
            ThisAddIn.Crypt = C_Crypt
            ThisAddIn.hf = C_Helfer
            ThisAddIn.KontaktFunktionen = C_Kontakt
            ThisAddIn.RWSSuche = C_RWS
            ThisAddIn.OlI = C_OlI
            ThisAddIn.fBox = C_FBox
            ThisAddIn.WClient = C_WählClient
            ThisAddIn.AnrMon = C_AnrMon
            ThisAddIn.GUI = C_GUI
            ThisAddIn.Phoner = C_Phoner
            ThisAddIn.UseAnrMon = UseAnrMon

            If CBool(C_XML.Read("Optionen", "CBJImport", CStr(False))) And UseAnrMon And CBool(C_XML.Read("Optionen", "CBForceFBAddr", "False")) Then
                Dim formjournalimort As New formJournalimport(C_AnrMon, C_Helfer, C_XML, False)
            End If
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Function PrüfeAddin() As Boolean
        Dim Rückgabe As Boolean = False
        Dim TMPStr(4) As String

        TMPStr(0) = C_XML.Read("Optionen", "TBLandesVW", "-1")
        TMPStr(1) = C_XML.Read("Optionen", "TBVorwahl", "-1")
        TMPStr(3) = C_XML.Read("Optionen", "TBPasswort", "-1")
        TMPStr(4) = GetSetting("FritzBox", "Optionen", "Zugang", "-1")

        If C_Helfer.IsOneOf("-1", TMPStr) Then
            Rückgabe = False
            Me.ShowDialog()
            Rückgabe = True 'PrüfeAddin()
        Else
            'Me.ShowDialog()
            Rückgabe = True
        End If
        Return Rückgabe

    End Function

    Private Sub BFBAdr_Click(sender As Object, e As EventArgs) Handles BFBAdr.Click
        Dim tmpstr As String = Me.TBFritzBoxAdr.Text
        If C_Helfer.Ping(tmpstr) Or Me.CBForceFBAddr.Checked Then
            Me.TBFritzBoxAdr.Text = tmpstr
            If Not InStr(C_Helfer.httpRead("http://" & tmpstr & "/login_sid.lua", System.Text.Encoding.UTF8, Nothing), "<SID>0000000000000000</SID>", CompareMethod.Text) = 0 Then
                C_XML.Write("Optionen", "TBFBAdr", tmpstr)
                C_XML.Write("Optionen", "CBForceFBAddr", CStr(Me.CBForceFBAddr.Checked))
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
        C_FBox = New FritzBox(C_XML, C_Helfer, C_Crypt, False, emc)
        C_XML.Write("Optionen", "TBBenutzer", Me.TBFBUser.Text)
        C_XML.Write("Optionen", "TBPasswort", C_Crypt.EncryptString128Bit(Me.TBFBPW.Text, "Fritz!Box Script"))
        SaveSetting("FritzBox", "Optionen", "Zugang", "Fritz!Box Script")
        C_Helfer.KeyChange()
        SID = C_FBox.FBLogin(fw550)
        If Not SID = C_FBox.sDefaultSID Then
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

    Private Sub BtELeINLESEN_Click(sender As Object, e As EventArgs) Handles BtELeINLESEN.Click
        Me.LVorwahl.Enabled = False
        Me.LLandesvorwahl.Enabled = False
        Me.TBVorwahl.Enabled = False
        Me.TBLandesvorwahl.Enabled = False
        Me.BtELeINLESEN.Text = "Bitte warten..."
        Me.BtELeINLESEN.Enabled = False

        C_XML.Write("Optionen", "TBVorwahl", Me.TBVorwahl.Text)
        C_XML.Write("Optionen", "TBLandesvorwahl", Me.TBLandesvorwahl.Text)
        C_FBox.bRausschreiben = False
        C_FBox.FritzBoxDaten()

        Me.CLBTelNr.Enabled = True
        Me.LTelListe.Enabled = True

        CLBtelnrAusfüllen()
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As EventArgs) Handles TBVorwahl.TextChanged, TBLandesvorwahl.TextChanged
        Me.BtELeINLESEN.Enabled = (Not Me.TBVorwahl.Text.Length = 0) And (Not Me.TBLandesvorwahl.Text.Length = 0)
    End Sub

    Sub CLBtelnrAusfüllen()
        Dim TelNrString As String = "Alle Telefonnummern;" & C_XML.ReadTelNr("Telefone")
        Dim CheckString() As String = Split(C_XML.Read("Telefone", "CLBTelNr", ";"), ";", , CompareMethod.Text)

        Dim res = From x In Split(TelNrString, ";", , CompareMethod.Text) Select x Distinct 'Doppelte entfernen
        res = (From x In res Where Not x Like "" Select x).ToArray ' Leere entfernen
        Me.CLBTelNr.Items.Clear()
        Dim alle As Boolean = True

        For Each TelNr In res
            Me.CLBTelNr.Items.Add(TelNr)
            If IsNumeric(TelNr) Then
                If C_Helfer.IsOneOf(TelNr, CheckString) Then
                    Me.CLBTelNr.SetItemChecked(Me.CLBTelNr.Items.Count - 1, True)
                Else
                    alle = False
                End If
            End If
        Next
        Me.CLBTelNr.SetItemChecked(0, alle)
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
        Dim checkstring As String = vbNullString
        Dim checkitemcoll As Windows.Forms.CheckedListBox.CheckedItemCollection = Me.CLBTelNr.CheckedItems
        If checkitemcoll.Count = 0 Then
            For i = 0 To Me.CLBTelNr.Items.Count - 1
                Me.CLBTelNr.SetItemChecked(i, True)
            Next
            checkitemcoll = Me.CLBTelNr.CheckedItems
        End If
        For Each el As String In checkitemcoll
            If Not el = "Alle Telefonnummern" And Not C_Helfer.IsOneOf(el, Split(checkstring, ";", , CompareMethod.Text)) Then
                checkstring += el & ";"
            End If
        Next
        If Strings.Right(checkstring, 1) = ";" Then checkstring = Strings.Left(checkstring, Len(checkstring) - 1)

        C_XML.Write("Telefone", "CLBTelNr", checkstring)
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