Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Forms

Friend Class formCfg
#Region "Eigene Klassen"
    Private C_XML As MyXML
    Private C_Crypt As Rijndael
    Private C_Helfer As Helfer
    Private C_Kontakte As Contacts
    Private C_Phoner As PhonerInterface
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_AnrMon As AnrufMonitor
    Private C_FBox As FritzBox
#End Region

#Region "BackgroundWorker"
    Private WithEvents BWTelefone As BackgroundWorker
    Private WithEvents BWIndexer As BackgroundWorker
#End Region

#Region "Delegaten"
    Private Delegate Sub DelgButtonTelEinl()
    Private Delegate Sub DelgSetLine()
    Private Delegate Sub DelgSetFillTelListe()
    Private Delegate Sub DelgStatistik()
    Private Delegate Sub DelgSetProgressbar()
#End Region

#Region "EventMulticaster"
    Private WithEvents emc As New EventMulticaster
#End Region

#Region "Eigene Variablen"
    Private tmpCheckString As String
    Private StatusWert As String
    Private KontaktName As String
    Private Anzahl As Integer = 0
    Private Startzeit As Date
    Private _StoppUhrAnzeigen As Boolean
    Private Dauer As TimeSpan
#End Region

    Public Sub New(ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal XMLKlasse As MyXML, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal CryptKlasse As Rijndael, _
                   ByVal AnrufMon As AnrufMonitor, _
                   ByVal fritzboxKlasse As FritzBox, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal kontaktklasse As Contacts, _
                   ByVal Phonerklasse As PhonerInterface)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        C_Helfer = HelferKlasse
        C_XML = XMLKlasse
        C_Crypt = CryptKlasse
        C_GUI = InterfacesKlasse
        C_OlI = OutlInter
        C_AnrMon = AnrufMon
        C_FBox = fritzboxKlasse
        C_Kontakte = kontaktklasse
        C_Phoner = Phonerklasse
        Me.LVersion.Text += ThisAddIn.Version
        With Me.ComboBoxRWS.Items
            .Add("11880.com")
            .Add("DasTelefonbuch.de")
            .Add("tel.search.ch")
            .Add("Alle")
        End With
    End Sub

    Private Sub UserForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.TBAnrMonMoveGeschwindigkeit.BackColor = CType(IIf(iTa.IsThemeActive, SystemColors.ControlLightLight, SystemColors.ControlLight), Color)
        Me.BAnrMonTest.Enabled = Not C_AnrMon Is Nothing
        Me.BTelefonliste.Enabled = Not C_FBox Is Nothing
        Me.FBDB_MP.SelectedIndex = 0
        'Me.StartPosition = FormStartPosition.CenterParent
        Ausfüllen()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "Ausfüllen"

    Private Sub Ausfüllen()
        Me.ToolTipFBDBConfig.SetToolTip(Me.ButtonXML, "Öffnet die Datei " & vbCrLf & C_XML.GetXMLDateiPfad)
#If OVer >= 14 Then
        If Not Me.FBDB_MP.TabPages.Item("PSymbolleiste") Is Nothing Then Me.FBDB_MP.TabPages.Remove(Me.FBDB_MP.TabPages.Item("PSymbolleiste"))
#End If
        ' Beim Einblenden die Werte aus der Registry einlesen
        ' Einstellungen für das Wählmakro laden
        Me.TBLandesVW.Text = C_XML.P_TBLandesVW
        Me.TBAmt.Text = C_XML.P_TBAmt
        Me.TBAmt.Text = CStr(IIf(Me.TBAmt.Text = "-1", "", Me.TBAmt.Text))
        Me.TBFBAdr.Text = C_XML.P_TBFBAdr

        Me.CBForceFBAddr.Checked = C_XML.P_CBForceFBAddr
        Me.TBBenutzer.Text = C_XML.P_TBBenutzer
        If Not Me.TBBenutzer.Text = vbNullString Then
            If C_XML.Read("Optionen", Me.TBBenutzer.Text, "2") = "0" Then
                Me.TBBenutzer.BackColor = Color.Red
                Me.ToolTipFBDBConfig.SetToolTip(Me.TBBenutzer, "Der Benutzer " & Me.TBBenutzer.Text & " hat keine ausreichenden Berechtigungen auf der Fritz!Box.")
            End If
        End If

        If Not Len(C_XML.P_TBPasswort) = 0 Then Me.TBPasswort.Text = "1234"
        Me.TBVorwahl.Text = C_XML.P_TBVorwahl
        Me.TBEnblDauer.Text = CStr(C_XML.P_TBEnblDauer)
        Me.CBAnrMonAuto.Checked = C_XML.P_CBAnrMonAuto
        Me.TBAnrMonX.Text = CStr(C_XML.P_TBAnrMonX)
        Me.TBAnrMonY.Text = CStr(C_XML.P_TBAnrMonY)
        Me.CBAnrMonMove.Checked = C_XML.P_CBAnrMonMove
        Me.CBAnrMonTransp.Checked = C_XML.P_CBAnrMonTransp
        Me.TBAnrMonMoveGeschwindigkeit.Value = C_XML.P_TBAnrMonMoveGeschwindigkeit
        Me.CBAnrMonContactImage.Checked = C_XML.P_CBAnrMonContactImage
        Me.CBIndexAus.Checked = C_XML.P_CBIndexAus
        Me.CBShowMSN.Checked = C_XML.P_CBShowMSN
        ' optionale allgemeine Einstellungen laden
        Me.CBAutoClose.Checked = C_XML.P_CBAutoClose
        Me.CBVoIPBuster.Checked = C_XML.P_CBVoIPBuster
        Me.CBCbCunterbinden.Checked = C_XML.P_CBCbCunterbinden
        Me.CBCallByCall.Checked = C_XML.P_CBCallByCall
        Me.CBDialPort.Checked = C_XML.P_CBDialPort
        Me.CBRueckwaertssuche.Checked = C_XML.P_CBRueckwaertssuche
        Me.CBKErstellen.Checked = C_XML.P_CBKErstellen
        Me.CBLogFile.Checked = C_XML.P_CBLogFile
#If OVer < 14 Then
        ' Einstellungen für die Symbolleiste laden
        Me.CBSymbWwdh.Checked = C_XML.P_CBSymbWwdh
        Me.CBSymbAnrMon.Checked = C_XML.P_CBSymbAnrMon
        Me.CBSymbAnrMonNeuStart.Checked = C_XML.P_CBSymbAnrMonNeuStart
        Me.CBSymbAnrListe.Checked = C_XML.P_CBSymbAnrListe
        Me.CBSymbDirekt.Checked = C_XML.P_CBSymbDirekt
        Me.CBSymbRWSuche.Checked = C_XML.P_CBSymbRWSuche
        Me.CBSymbVIP.Checked = C_XML.P_CBSymbVIP '
        Me.CBSymbJournalimport.Checked = C_XML.P_CBSymbJournalimport
#End If
        Me.CBJImport.Checked = C_XML.P_CBJImport
        ' Einstellungen füer die Rückwärtssuche laden
        Me.CBKHO.Checked = C_XML.P_CBKHO
        Me.CBRWSIndex.Checked = C_XML.P_CBRWSIndex

        Me.ComboBoxRWS.SelectedItem = Me.ComboBoxRWS.Items.Item(C_XML.P_CBoxRWSuche)
        If Not Me.CBRueckwaertssuche.Checked Then Me.ComboBoxRWS.Enabled = False
        ' Einstellungen für das Journal laden

        Me.CBJournal.Checked = C_XML.P_CBJournal
        Me.CBUseAnrMon.Checked = C_XML.P_CBUseAnrMon
        Me.CBCheckMobil.Checked = C_XML.P_CBCheckMobil

        Me.CBIndexAus.Enabled = Not Me.CBUseAnrMon.Checked
        Me.PanelAnrMon.Enabled = Me.CBUseAnrMon.Checked
        'StoppUhr
        Me.CBStoppUhrEinblenden.Checked = C_XML.P_CBStoppUhrEinblenden
        Me.CBStoppUhrAusblenden.Checked = C_XML.P_CBStoppUhrAusblenden
        Me.TBStoppUhr.Text = CStr(C_XML.P_TBStoppUhr)

        Me.CBStoppUhrAusblenden.Enabled = Me.CBStoppUhrEinblenden.Checked
        If Not Me.CBStoppUhrEinblenden.Checked Then Me.CBStoppUhrAusblenden.Checked = False
        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked

        'Telefonnummernformat
        Me.TBTelNrMaske.Text = C_XML.P_TBTelNrMaske
        Me.CBTelNrGruppieren.Checked = C_XML.P_CBTelNrGruppieren
        Me.CBintl.Checked = C_XML.P_CBintl
        Me.CBIgnoTelNrFormat.Checked = C_XML.P_CBIgnoTelNrFormat

#If OVer < 14 Then
        If Not Me.CBJournal.Checked Then Me.CBSymbJournalimport.Checked = False
        Me.CBSymbJournalimport.Enabled = Me.CBJournal.Checked
#End If
        'Phoner
        Dim PhonerVerfuegbar As Boolean = C_XML.P_PhonerVerfügbar
        Dim TelName() As String
        Me.PanelPhoner.Enabled = PhonerVerfuegbar
        If PhonerVerfuegbar Then
            Me.CBPhoner.Checked = C_XML.P_CBPhoner
        Else
            Me.CBPhoner.Checked = False
        End If
        Me.LabelPhoner.Text = Replace(Me.LabelPhoner.Text, " [nicht]", CStr(IIf(PhonerVerfuegbar, "", " nicht")), , , CompareMethod.Text)
        'Me.CBPhonerKeineFB.Checked = CBool(IIf(C_XML.Read("Phoner", "CBPhonerKeineFB", "False") = "True", True, False))
        'If Not Me.CBPhonerKeineFB.Checked Then
        For i = 20 To 29
            TelName = Split(C_XML.Read("Telefone", CStr(i), "-1;"), ";", , CompareMethod.Text)
            If Not TelName(0) = "-1" And Not TelName.Length = 2 Then
                Me.ComboBoxPhonerSIP.Items.Add(TelName(2))
            End If
        Next
        If Not Me.ComboBoxPhonerSIP.Items.Count = 0 Then
            Me.ComboBoxPhonerSIP.SelectedIndex = C_XML.P_ComboBoxPhonerSIP
        End If
        'Else
        'Me.ComboBoxPhonerSIP.SelectedIndex = 0
        'Me.ComboBoxPhonerSIP.Enabled = False
        'End If
        Me.CBPhonerAnrMon.Checked = C_XML.P_CBPhonerAnrMon
        If Not Len(C_XML.P_TBPhonerPasswort) = 0 Then Me.TBPhonerPasswort.Text = "1234"

        Dim PhonerInstalliert As Boolean = C_Phoner.PhonerReady()
        Me.PanelPhonerAktiv.BackColor = CType(IIf(PhonerInstalliert, Color.LightGreen, Color.Red), Color)
        Me.LabelPhoner.Text = "Phoner ist " & CStr(IIf(PhonerInstalliert, "", "nicht ")) & "aktiv."
        Me.PanelPhoner.Enabled = PhonerInstalliert
        C_XML.P_PhonerVerfügbar = PhonerInstalliert
        ' Tooltipp
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBVoIPBuster, "Mit dieser Einstellung wird die Landesvorwahl " & Me.TBLandesVW.Text & " immer mitgewählt.")

        FillLogTB()
        FillTelListe()
        CLBTelNrAusfüllen()
    End Sub
    ''' <summary>
    ''' Füllt die Telefonliste in den Einstellungen aus.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillTelListe()
        Dim Zeile As New ArrayList
        Dim Nebenstellen() As String
        Dim j As Integer
        Dim tmpein(3) As Double
        Dim xPathTeile As New ArrayList

        With xPathTeile
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("TelName")
        End With
        Nebenstellen = Split(C_XML.Read(xPathTeile, "-1;"), ";", , CompareMethod.Text)

        If Not Nebenstellen(0) = "-1" Then
            With Me.TelList
                .Rows.Clear()
                j = 0
                For Each Nebenstelle As String In Nebenstellen
                    j += 1
                    xPathTeile.Clear()

                    With xPathTeile
                        .Add("Telefone")
                        .Add("Telefone")
                        .Add("*")
                        .Add("Telefon")
                        .Add("[TelName = """ & Nebenstelle & """]")
                        .Add("@Standard")
                        Zeile.Add(CBool(C_XML.Read(xPathTeile, "False")))
                        Zeile.Add(CStr(j))
                        .Item(.Count - 1) = "@Dialport"
                        Zeile.Add(C_XML.Read(xPathTeile, "-1;")) 'Nebenstelle
                        .RemoveAt(.Count - 1)
                        Zeile.Add(C_XML.ReadElementName(xPathTeile, "-1;")) 'Telefontyp
                        Zeile.Add(Nebenstelle) ' TelName
                        .Add("TelNr")
                        Zeile.Add(Replace(C_XML.Read(xPathTeile, "-"), ";", ", ", , , CompareMethod.Text)) 'TelNr
                        .Item(.Count - 1) = "Eingehend"
                        Zeile.Add(C_XML.Read(xPathTeile, "0")) 'Eingehnd
                        tmpein(0) += CDbl(Zeile.Item(Zeile.Count - 1))
                        .Item(.Count - 1) = "Ausgehend"
                        Zeile.Add(C_XML.Read(xPathTeile, "0")) 'Ausgehnd
                        tmpein(1) += CDbl(Zeile.Item(Zeile.Count - 1))
                        Zeile.Add(CStr(CDbl(Zeile.Item(Zeile.Count - 2)) + CDbl(Zeile.Item(Zeile.Count - 1)))) 'Gesamt
                        tmpein(2) += CDbl(Zeile.Item(Zeile.Count - 1))
                        For i = Zeile.Count - 3 To Zeile.Count - 1
                            Zeile.Item(i) = C_Helfer.GetTimeInterval(CInt(Zeile.Item(i)))
                        Next
                    End With
                    .Rows.Add(Zeile.ToArray)
                    Zeile.Clear()
                Next
                Zeile.Add(False)
                Zeile.Add(vbNullString)
                Zeile.Add(vbNullString)
                Zeile.Add(vbNullString)
                Zeile.Add(vbNullString)
                Zeile.Add("Gesamt:")
                For i = 0 To 2
                    Zeile.Add(C_Helfer.GetTimeInterval(tmpein(i)))
                Next

                .Rows.Add(Zeile.ToArray)
            End With
        End If

        Me.TBAnderes.Text = C_XML.P_StatVerpasst & " verpasste Telefonate" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_XML.P_StatNichtErfolgreich & " nicht erfolgreiche Telefonate" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_XML.P_StatKontakt & " erstellte Kontakte" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_XML.P_StatJournal & " erstellte Journaleinträge" & vbCrLf
        Me.TBReset.Text = "Letzter Reset: " & C_XML.P_StatResetZeit
        Me.TBSchließZeit.Text = "Letzter Journaleintrag: " & C_XML.P_StatOLClosedZeit
        xPathTeile = Nothing
        Zeile = Nothing
    End Sub

    Private Sub CLBTelNrAusfüllen()
        Dim xPathTeile As New ArrayList
        Dim TelNrString() As String
        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*[starts-with(name(.), ""POTS"") or starts-with(name(.), ""MSN"") or starts-with(name(.), ""SIP"")]")

            TelNrString = Split("Alle Telefonnummern;" & C_XML.Read(xPathTeile, ""), ";", , CompareMethod.Text)

            TelNrString = (From x In TelNrString Select x Distinct).ToArray 'Doppelte entfernen
            TelNrString = (From x In TelNrString Where Not x Like "" Select x).ToArray ' Leere entfernen
            Me.CLBTelNr.Items.Clear()

            For Each TelNr In TelNrString
                Me.CLBTelNr.Items.Add(TelNr)
            Next
            'etwas unschön
            .Add("")
            For i = 1 To Me.CLBTelNr.Items.Count - 1
                .Item(.Count - 2) = "*[. = """ & Me.CLBTelNr.Items(i).ToString & """]"
                .Item(.Count - 1) = "@Checked"
                Me.CLBTelNr.SetItemChecked(i, C_Helfer.IsOneOf("1", Split(C_XML.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)))
            Next
        End With
        Me.CLBTelNr.SetItemChecked(0, Me.CLBTelNr.CheckedItems.Count = Me.CLBTelNr.Items.Count - 1)
    End Sub

#End Region

    Private Function Speichern() As Boolean
        Speichern = True
        Dim xPathTeile As New ArrayList
        Dim tmpTeile As String = vbNullString
        Dim CheckTelNr As CheckedListBox.CheckedItemCollection = Me.CLBTelNr.CheckedItems
        If CheckTelNr.Count = 0 Then
            For i = 0 To Me.CLBTelNr.Items.Count - 1
                Me.CLBTelNr.SetItemChecked(i, True)
            Next
            CheckTelNr = Me.CLBTelNr.CheckedItems
        End If
        If Me.CLBTelNr.Items.Count > 1 Then
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
        End If

        ' Sichert die Einstellungen und schließt das Fenster
        If (CInt(Me.TBEnblDauer.Text) < 4) Then Me.TBEnblDauer.Text = "4"
        With C_XML



            ' So ist es schön:
            C_FBox.P_FBAddr = Me.TBFBAdr.Text
            ' So nicht:
            ThisAddIn.P_AnrMon.P_FBAddr = Me.TBFBAdr.Text
            .P_CBForceFBAddr = Me.CBForceFBAddr.Checked


            If Me.TBBenutzer.Text = vbNullString Then
                With xPathTeile
                    .Clear()
                    .Add("Optionen")
                    .Add("TBBenutzer")
                End With
                C_XML.Delete(xPathTeile)
            Else
                .P_TBBenutzer = Me.TBBenutzer.Text
            End If
            If Not Me.TBPasswort.Text = "1234" Then
                .P_TBPasswort = C_Crypt.EncryptString128Bit(Me.TBPasswort.Text, "Fritz!Box Script")
                SaveSetting("FritzBox", "Optionen", "Zugang", "Fritz!Box Script")
                C_Helfer.KeyChange()
            End If
            ' StoppUhr
            If Not Me.TBStoppUhr.Text = vbNullString Then
                If CInt(Me.TBStoppUhr.Text) < 0 Then
                    Me.TBStoppUhr.Text = "10"
                End If
            Else
                Me.TBStoppUhr.Text = "10"
            End If

            .P_TBLandesVW = Me.TBLandesVW.Text
            .P_TBAmt = CStr(IIf(Me.TBAmt.Text = "", "-1", Me.TBAmt.Text))
            .P_TBFBAdr = Me.TBFBAdr.Text
            .P_TBVorwahl = Me.TBVorwahl.Text

            .P_TBAnrMonX = CInt(Me.TBAnrMonX.Text)
            .P_TBAnrMonY = CInt(Me.TBAnrMonY.Text)
            .P_CBLogFile = Me.CBLogFile.Checked
            .P_TBEnblDauer = CInt(Me.TBEnblDauer.Text)
            .P_CBAnrMonAuto = Me.CBAnrMonAuto.Checked
            .P_CBAutoClose = Me.CBAutoClose.Checked
            .P_CBAnrMonMove = Me.CBAnrMonMove.Checked
            .P_CBAnrMonTransp = Me.CBAnrMonTransp.Checked
            .P_CBAnrMonContactImage = Me.CBAnrMonContactImage.Checked
            .P_TBAnrMonMoveGeschwindigkeit = Me.TBAnrMonMoveGeschwindigkeit.Value
            .P_CBIndexAus = Me.CBIndexAus.Checked
            .P_CBShowMSN = Me.CBShowMSN.Checked
            .P_CBVoIPBuster = Me.CBVoIPBuster.Checked
            .P_CBDialPort = Me.CBDialPort.Checked
            .P_CBCbCunterbinden = Me.CBCbCunterbinden.Checked
            .P_CBCallByCall = Me.CBCallByCall.Checked
            .P_CBRueckwaertssuche = Me.CBRueckwaertssuche.Checked
            .P_CBKErstellen = Me.CBKErstellen.Checked
            .P_ComboBoxRWS = Me.ComboBoxRWS.SelectedIndex
            .P_CBKHO = Me.CBKHO.Checked
            .P_CBRWSIndex = Me.CBRWSIndex.Checked
            .P_CBJournal = Me.CBJournal.Checked
            .P_CBUseAnrMon = Me.CBUseAnrMon.Checked
            .P_CBJImport = Me.CBJImport.Checked
            .P_CBCheckMobil = Me.CBCheckMobil.Checked
            .P_CBStoppUhrEinblenden = Me.CBStoppUhrEinblenden.Checked
            .P_CBStoppUhrAusblenden = Me.CBStoppUhrAusblenden.Checked
            .P_TBStoppUhr = CInt(Me.TBStoppUhr.Text)
#If OVer < 14 Then
            .P_CBSymbWwdh = Me.CBSymbWwdh.Checked
            .P_CBSymbAnrMonNeuStart = Me.CBSymbAnrMonNeuStart.Checked
            .P_CBSymbAnrMon = Me.CBSymbAnrMon.Checked
            .P_CBSymbAnrListe = Me.CBSymbAnrListe.Checked
            .P_CBSymbDirekt = Me.CBSymbDirekt.Checked
            .P_CBSymbRWSuche = Me.CBSymbRWSuche.Checked
            .P_CBSymbJournalimport = Me.CBSymbJournalimport.Checked
            .P_CBSymbVIP = Me.CBSymbVIP.Checked
#End If
            If PrüfeMaske() Then .P_TBTelNrMaske = Me.TBTelNrMaske.Text
            .P_CBTelNrGruppieren = Me.CBTelNrGruppieren.Checked
            .P_CBintl = Me.CBintl.Checked
            .P_CBIgnoTelNrFormat = Me.CBIgnoTelNrFormat.Checked

            .P_CBPhoner = Me.CBPhoner.Checked

            .P_ComboBoxPhonerSIP = Me.ComboBoxPhonerSIP.SelectedIndex
            .P_CBPhonerAnrMon = Me.CBPhonerAnrMon.Checked


            ' Telefone
#If OVer < 14 Then
            C_GUI.SetVisibleButtons()
#End If
            With xPathTeile
                .Clear()
                .Add("Telefone")
                .Add("Telefone")
                .Add("*")
                .Add("Telefon")
                .Add(vbNullString)
                For i = 0 To TelList.Rows.Count - 2
                    .Item(.Count - 1) = "[@Dialport = """ & TelList.Rows(i).Cells(2).Value.ToString & """]"
                    C_XML.WriteAttribute(xPathTeile, "Standard", CStr(CBool(TelList.Rows(i).Cells(0).Value)))
                Next
            End With
            ' Phoner
            Dim TelName() As String
            Dim PhonerTelNameIndex As Integer = 0

            For i = 20 To 29
                TelName = Split(C_XML.Read("Telefone", CStr(i), "-1;;"), ";", , CompareMethod.Text)
                If Not TelName(0) = "-1" And Not ComboBoxPhonerSIP.SelectedItem Is Nothing And Not TelName.Length = 2 Then
                    If TelName(2) = ComboBoxPhonerSIP.SelectedItem.ToString Then
                        PhonerTelNameIndex = i
                        Exit For
                    End If
                End If
            Next
            .P_PhonerTelNameIndex = PhonerTelNameIndex
            'ThisAddIn.NutzePhonerOhneFritzBox = Me.CBPhonerKeineFB.Checked
            If Me.TBPhonerPasswort.Text = "" And Me.CBPhoner.Checked Then
                If C_Helfer.FBDB_MsgBox("Es wurde kein Passwort für Phoner eingegeben! Da Wählen über Phoner wird nicht funktionieren!", MsgBoxStyle.OkCancel, "Speichern") = MsgBoxResult.Cancel Then
                    Speichern = False
                End If
            End If

            If Me.CBPhoner.Checked Then
                If Not Me.TBPhonerPasswort.Text = "" Then
                    If Not Me.TBPhonerPasswort.Text = "1234" Then
                        .P_TBPhonerPasswort = C_Crypt.EncryptString128Bit(Me.TBPhonerPasswort.Text, "Fritz!Box Script")
                        SaveSetting("FritzBox", "Optionen", "ZugangPasswortPhoner", "Fritz!Box Script")
                        C_Helfer.KeyChange()
                    End If
                End If
            End If
            .SpeichereXMLDatei()
        End With
    End Function

#Region "Button Link"
    Private Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonZuruecksetzen.Click, _
                                                                                   ButtonOK.Click, _
                                                                                   ButtonAbbruch.Click, _
                                                                                   ButtonUebernehmen.Click, _
                                                                                   ButtonXML.Click, _
                                                                                   BAnrMonTest.Click, _
                                                                                   BIndizierungStart.Click, _
                                                                                   BIndizierungAbbrechen.Click, _
                                                                                   BZwischenablage.Click, _
                                                                                   BTelefonliste.Click, _
                                                                                   BTelefonDatei.Click, _
                                                                                   BStartDebug.Click, _
                                                                                   BResetStat.Click, _
                                                                                   BProbleme.Click, _
                                                                                   BStoppUhrAnzeigen.Click

        Select Case CType(sender, Windows.Forms.Button).Name
            Case "ButtonZuruecksetzen"
                ' Startwerte zurücksetzen
                ' Einstellungen für das Wählmakro zurücksetzen
                Me.TBLandesVW.Text = "0049"
                Me.TBAmt.Text = ""
                Me.CBCheckMobil.Checked = True

                ' Einstellungen für den Anrufmonitor zurücksetzen
                Me.TBEnblDauer.Text = "10"
                Me.TBAnrMonX.Text = "0"
                Me.TBAnrMonY.Text = "0"
                Me.CBAnrMonAuto.Checked = False
                Me.CBAutoClose.Checked = True
                Me.CBAnrMonMove.Checked = True
                Me.CBAnrMonTransp.Checked = True
                Me.CBAnrMonContactImage.Checked = True
                Me.CBShowMSN.Checked = False
                Me.TBAnrMonMoveGeschwindigkeit.Value = 5
                Me.CBIndexAus.Checked = False
                Me.CBIndexAus.Enabled = False
                ' optionale allgemeine Einstellungen zuruecksetzen
                Me.CBVoIPBuster.Checked = False
                Me.CBDialPort.Checked = False
                Me.CBCallByCall.Checked = False
                Me.CBCbCunterbinden.Checked = False
                Me.CBRueckwaertssuche.Checked = False
                Me.CBKErstellen.Checked = False
                Me.CBLogFile.Checked = False
                Me.CBForceFBAddr.Checked = False
#If OVer < 14 Then
                ' Einstellungen für die Symbolleiste zurücksetzen
                Me.CBSymbAnrMonNeuStart.Checked = False
                Me.CBSymbWwdh.Checked = True
                Me.CBSymbAnrMon.Checked = True
                Me.CBSymbAnrListe.Checked = True
                Me.CBSymbDirekt.Checked = True
                Me.CBSymbRWSuche.Checked = False
                Me.CBSymbJournalimport.Checked = False
#End If
                ' Einstellungen für die Rückwärtssuche zurücksetzen
                Me.ComboBoxRWS.Enabled = False
                Me.ComboBoxRWS.SelectedIndex = 0
                Me.CBRWSIndex.Checked = True
                ' Einstellungen für das Journal zurücksetzen
                Me.CBKHO.Checked = True
                Me.CBJournal.Checked = False
                Me.CBJImport.Checked = False
                Me.CBLogFile.Checked = True

                'StoppUhr
                Me.CBStoppUhrEinblenden.Checked = False
                Me.CBStoppUhrAusblenden.Checked = False
                Me.TBStoppUhr.Text = "10"

                'Telefonnummernformat
                Me.TBTelNrMaske.Text = "%L (%O) %N - %D"
                Me.CBTelNrGruppieren.Checked = True
                Me.CBintl.Checked = False
                Me.CBIgnoTelNrFormat.Checked = False
            Case "BTelefonliste"
                Dim xPathTeile As New ArrayList
                C_FBox.SetEventProvider(emc)
                Me.BTelefonliste.Enabled = False
                Me.BTelefonliste.Text = "Bitte warten..."
                Windows.Forms.Application.DoEvents()
                Speichern()

                BWTelefone = New BackgroundWorker
                With BWTelefone
                    .WorkerReportsProgress = False
                    .RunWorkerAsync(True)
                End With
            Case "ButtonOK"
                Dim formschließen As Boolean = Speichern()
                C_XML.P_CBUseAnrMon = Me.CBUseAnrMon.Checked
#If OVer >= 14 Then
                C_GUI.RefreshRibbon()
#End If
                If formschließen Then
                    Me.Hide()
                    'Dispose(True)
                End If
            Case "ButtonAbbruch"
                ' Schließt das Fenster
                Me.Hide()
                'Dispose(True)
            Case "ButtonUebernehmen"
                Speichern()
            Case "ButtonXML"
                System.Diagnostics.Process.Start(C_XML.GetXMLDateiPfad)
            Case "BAnrMonTest"
                Speichern()
                Dim ID As Integer = CInt(C_XML.Read("letzterAnrufer", "Letzter", CStr(0)))
                Dim forman As New formAnrMon(ID, False, C_XML, C_Helfer, C_AnrMon, C_OlI)
            Case "BZwischenablage"
                My.Computer.Clipboard.SetText(Me.TBDiagnose.Text)
            Case "BProbleme"
                Dim T As New Thread(AddressOf NewMail)
                T.Start()
                If C_Helfer.FBDB_MsgBox("Der Einstellungsdialog wird jetzt geschlossen. Danach werden alle erforderlichen Informationen gesammelt, was ein paar Sekunden dauern kann." & vbNewLine & _
                                                "Danach wird eine neue E-Mail geöffnet, die Sie bitte vervollständigen und absenden.", MsgBoxStyle.Information, "") = MsgBoxResult.Ok Then
                    Me.Close()
                End If
            Case "BStartDebug"
                Me.TBDiagnose.Text = vbNullString
                AddLine("Start")
                If Me.CBTelefonDatei.Checked Then
                    If System.IO.File.Exists(Me.TBTelefonDatei.Text) Then
                        If C_Helfer.FBDB_MsgBox("Sind Sie sicher was sie da tun? Das Testen einer fehlerhaften oder falschen Datei kann sehr unerfreulich enden.", _
                                                        MsgBoxStyle.YesNo, "Telefondatei testen") = vbYes Then
                            Me.TBTelefonDatei.Enabled = False
                        End If
                    Else
                        Me.CBTelefonDatei.Checked = False
                    End If
                End If
                C_FBox.SetEventProvider(emc)
                AddLine("Fritz!Box Klasse mit Verweis auf dieses Formular erstellt.")

                BWTelefone = New BackgroundWorker
                AddLine("BackgroundWorker erstellt.")
                With BWTelefone
                    .WorkerReportsProgress = True
                    .RunWorkerAsync(True)
                    AddLine("BackgroundWorker gestartet.")
                End With
                Me.TBTelefonDatei.Enabled = True
            Case "BTelefonDatei"
                Dim fDialg As New System.Windows.Forms.OpenFileDialog
                With fDialg
                    .Filter = "htm-Dateien (*.htm)| *.htm"
                    .Multiselect = False
                    .Title = "Fritz!Box Telefon-Datei auswählen"
                    .FilterIndex = 1
                    .RestoreDirectory = True
                    If .ShowDialog = Windows.Forms.DialogResult.OK Then
                        If System.IO.File.Exists(fDialg.FileName) Then
                            Me.TBTelefonDatei.Text = fDialg.FileName
                        Else
                            Me.TBTelefonDatei.Text = "Fehler!"
                        End If
                    End If
                End With
                fDialg = Nothing
            Case "BResetStat"
                Dim xPathTeile As New ArrayList
                C_XML.Delete("Statistik")
                With xPathTeile
                    .Add("Statistik")
                    .Add("ResetZeit")
                    C_XML.Write(xPathTeile, CStr(System.DateTime.Now))
                    .Clear()
                    .Add("Telefone")
                    .Add("Telefone")
                    .Add("*")
                    .Add("Telefon")
                    .Add("Eingehend")
                    C_XML.Write(xPathTeile, "0")
                    .Item(.Count - 1) = "Ausgehend"
                    C_XML.Write(xPathTeile, "0")
                End With
                FillTelListe()
                xPathTeile = Nothing
            Case "BIndizierungStart"
                StarteIndizierung()
            Case "BIndizierungAbbrechen"
                BWIndexer.CancelAsync()
                Me.BIndizierungAbbrechen.Enabled = False
                Me.BIndizierungStart.Enabled = True
            Case "BStoppUhrAnzeigen"
                Speichern()
                Dim Zeit As String
                Dim WarteZeit As Integer
                Dim Beendet As Boolean = False
                Dim StartPosition As System.Drawing.Point
                Dim x As Integer = 0
                Dim y As Integer = 0
                If C_XML.P_CBStoppUhrAusblenden Then
                    WarteZeit = CInt(Me.TBStoppUhr.Text)
                Else
                    WarteZeit = -1
                End If

                StartPosition = New System.Drawing.Point(C_XML.P_CBStoppUhrX, C_XML.P_CBStoppUhrY)
                For Each Bildschirm In Windows.Forms.Screen.AllScreens
                    x += Bildschirm.Bounds.Size.Width
                    y += Bildschirm.Bounds.Size.Height
                Next
                With StartPosition
                    If .X > x Or .Y > y Then
                        .X = CInt((Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 100) / 2)
                        .Y = CInt((Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 50) / 2)
                    End If
                End With
                With System.DateTime.Now
                    Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hour, .Minute, .Second)
                End With

                Dim frmStUhr As New formStoppUhr("Gegenstelle", Zeit, "Richtung:", WarteZeit, StartPosition, "Ihre MSN")
                Do Until frmStUhr.StUhrClosed
                    Thread.Sleep(20)
                    Windows.Forms.Application.DoEvents()
                Loop
                C_XML.P_CBStoppUhrX = frmStUhr.Position.X
                C_XML.P_CBStoppUhrY = frmStUhr.Position.Y
                frmStUhr = Nothing
        End Select
    End Sub

    Private Sub Link_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkHomepage.LinkClicked, _
                                                                                                                                LinkForum.LinkClicked, _
                                                                                                                                LinkEmail.LinkClicked, _
                                                                                                                                LinkLogFile.LinkClicked
        Select Case CType(sender, Windows.Forms.LinkLabel).Name
            Case "LinkEmail"
                Me.Close()
                System.Diagnostics.Process.Start("mailto:kruemelino@gert-michael.de")
            Case "LinkForum"
                System.Diagnostics.Process.Start("http://www.ip-phone-forum.de/showthread.php?t=237086")
            Case "LinkHomepage"
                System.Diagnostics.Process.Start("http://github.com/Kruemelino/FritzBoxTelefon-dingsbums")
            Case "LinkLogFile"
                System.Diagnostics.Process.Start(C_Helfer.Dateipfade("LogDatei"))
        End Select
    End Sub

#End Region

#Region "Änderungen"
    Private Sub ValueChanged(sender As Object, e As EventArgs) Handles _
                                                                        CBRueckwaertssuche.CheckedChanged, _
                                                                        CBCbCunterbinden.CheckedChanged, _
                                                                        CBAutoClose.CheckedChanged, _
                                                                        CBTelefonDatei.CheckedChanged, _
                                                                        CBJournal.CheckedChanged, _
                                                                        CBIndexAus.CheckedChanged, _
                                                                        CBUseAnrMon.CheckedChanged, _
                                                                        CBStoppUhrEinblenden.CheckedChanged, _
                                                                        CBStoppUhrAusblenden.CheckedChanged, _
                                                                        CBLogFile.CheckedChanged, _
                                                                        TBLandesVW.Leave, _
                                                                        TBVorwahl.TextChanged, _
                                                                        TBEnblDauer.TextChanged, _
                                                                        TBAnrMonX.TextChanged, _
                                                                        TBAnrMonY.TextChanged, _
                                                                        TBLandesVW.TextChanged, _
                                                                        TBTelNrMaske.Leave, _
                                                                        CLBTelNr.SelectedIndexChanged
        Select Case sender.GetType().Name
            Case "CheckBox"
                Select Case CType(sender, CheckBox).Name
                    Case "CBTelefonDatei"
                        Me.PTelefonDatei.Enabled = Me.CBTelefonDatei.Checked
                        If Not Me.CBTelefonDatei.Checked Then
                            Me.TBTelefonDatei.Text = vbNullString
                        End If
                    Case "CBRueckwaertssuche"
                        ' Combobox für Rückwärtssuchmaschinen je nach CheckBox für Rückwärtssuche ein- bzw. ausblenden
                        Me.ComboBoxRWS.Enabled = Me.CBRueckwaertssuche.Checked
                        Me.CBKErstellen.Checked = Me.CBRueckwaertssuche.Checked
                        Me.CBKErstellen.Enabled = Me.CBRueckwaertssuche.Checked
                        Me.CBRWSIndex.Enabled = Me.CBRueckwaertssuche.Checked
                        Me.CBRWSIndex.Checked = Me.CBRueckwaertssuche.Checked
                    Case "CBCbCunterbinden"
                        Me.CBCallByCall.Enabled = Not Me.CBCbCunterbinden.Checked
                        If Me.CBCbCunterbinden.Checked Then Me.CBCallByCall.Checked = False
                    Case "CBAutoClose"
                        Me.TBEnblDauer.Enabled = Me.CBAutoClose.Checked
                        Me.LEnblDauer.Enabled = Me.CBAutoClose.Checked
                    Case "CBJournal"
                        If Not Me.CBJournal.Checked Then Me.CBJImport.Checked = False
                        Me.CBJImport.Enabled = Me.CBJournal.Checked
                        'Me.GroupBoxStoppUhr.Enabled = Me.CBJournal.Checked
                        'If Not Me.CBJournal.Checked Then
                        '    Me.CBStoppUhrEinblenden.Checked = False
                        'End If
#If OVer < 14 Then
                If Not Me.CBJournal.Checked Then Me.CBSymbJournalimport.Checked = False
                Me.CBSymbJournalimport.Enabled = Me.CBJournal.Checked
#End If
                    Case "CBIndexAus"
                        Me.BIndizierungStart.Enabled = Not Me.CBIndexAus.Checked
                    Case "CBUseAnrMon"
                        Me.PanelAnrMon.Enabled = Me.CBUseAnrMon.Checked
                        Me.CBIndexAus.Enabled = Not Me.CBUseAnrMon.Checked
                        Me.GroupBoxStoppUhr.Enabled = Me.CBUseAnrMon.Checked

                        If Not Me.CBUseAnrMon.Checked Then
                            Me.CBStoppUhrEinblenden.Checked = False
                            Me.CBStoppUhrAusblenden.Checked = False
                        End If
                    Case "CBStoppUhrEinblenden"
                        Me.CBStoppUhrAusblenden.Enabled = Me.CBStoppUhrEinblenden.Checked
                        If Not Me.CBStoppUhrEinblenden.Checked Then Me.CBStoppUhrAusblenden.Checked = False
                        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked
                        Me.LabelStoppUhr.Enabled = Me.CBStoppUhrEinblenden.Checked
                    Case "CBStoppUhrAusblenden"
                        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked
                    Case "CBLogFile"
                        Me.GBLogging.Enabled = Me.CBLogFile.Checked
                End Select
            Case "TextBox"
                Select Case CType(sender, TextBox).Name
                    Case "TBLandesVW"
                        If Me.TBLandesVW.Text = "0049" Then
                            Me.CBRueckwaertssuche.Enabled = True
                            Me.CBKErstellen.Enabled = True
                            Me.ComboBoxRWS.Enabled = Me.CBRueckwaertssuche.Checked
                        Else
                            Me.CBRueckwaertssuche.Checked = False
                            Me.CBRueckwaertssuche.Enabled = False

                            Me.CBKErstellen.Enabled = False
                            Me.CBKErstellen.Checked = False
                            Me.ComboBoxRWS.Enabled = False
                        End If
                    Case "TBVorwahl"
                        C_Helfer.AcceptOnlyNumeric(Me.TBVorwahl.Text)
                    Case "TBEnblDauer"
                        C_Helfer.AcceptOnlyNumeric(Me.TBEnblDauer.Text)
                    Case "TBAnrMonX"
                        C_Helfer.AcceptOnlyNumeric(Me.TBAnrMonX.Text)
                    Case "TBAnrMonY"
                        C_Helfer.AcceptOnlyNumeric(Me.TBAnrMonY.Text)
                    Case "TBLandesVW"
                        Me.ToolTipFBDBConfig.SetToolTip(Me.CBVoIPBuster, "Mit dieser Einstellung wird die Landesvorwahl " & Me.TBLandesVW.Text & " immer mitgewählt.")
                    Case "TBTelNrMaske"
                        PrüfeMaske()
                End Select
            Case "CheckedListBox"
                Select Case CType(sender, CheckedListBox).Name
                    Case "CLBTelNr"
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
                End Select
        End Select
    End Sub

    Private Sub TelList_CellMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs)
        ' Sichersellen, dass nur ein Haken gesetzt ist.
        If TypeOf Me.TelList.CurrentCell Is Windows.Forms.DataGridViewCheckBoxCell Then
            Me.TelList.EndEdit()
            If Not Me.TelList.CurrentCell.Value Is Nothing Then
                Dim cellVal As Boolean = DirectCast(Me.TelList.CurrentCell.Value, Boolean)
                If cellVal Then
                    If Not Me.TelList.CurrentCell Is Me.TelList.Rows(Me.TelList.Rows.Count - 1).Cells(0) Then
                        For i = 0 To TelList.Rows.Count - 1
                            Me.TelList.Rows(i).Cells(0).Value = False
                        Next
                        If Not (Me.TelList.Rows(Me.TelList.CurrentCell.RowIndex).Cells(3).Value.ToString = "TAM" Or _
                             Me.TelList.Rows(Me.TelList.CurrentCell.RowIndex).Cells(3).Value.ToString = "FAX") Then Me.TelList.CurrentCell.Value = cellVal
                    Else
                        Me.TelList.CurrentCell.Value = False
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Helfer"
    Function PrüfeMaske() As Boolean
        ' "%L (%O) %N - %D"
        Dim pos(2) As String
        pos(0) = CStr(InStr(Me.TBTelNrMaske.Text, "%L", CompareMethod.Text))
        pos(1) = CStr(InStr(Me.TBTelNrMaske.Text, "%O", CompareMethod.Text))
        pos(2) = CStr(InStr(Me.TBTelNrMaske.Text, "%N", CompareMethod.Text))
        If C_Helfer.IsOneOf("0", pos) Then
            C_Helfer.FBDB_MsgBox("Achtung: Die Maske für die Telefonnummernformatierung ist nicht korrekt." & vbNewLine & _
                        "Prüfen Sie, ob folgende Zeichen in der Maske Enthalten sind: ""%L"", ""%V"" und ""%N"" (""%D"" kann wegelassen werden)!" & vbNewLine & _
                        "Beispiel: ""%L (%O) %N - %D""", MsgBoxStyle.Information, "Einstellungen")
            Return False
        End If
        Return True
    End Function

    Private Sub NewMail()
        Dim NeueFW As Boolean
        Dim SID As String = C_FBox.P_DefaultSID
        Dim URL As String
        Dim FBOX_ADR As String = C_XML.P_TBFBAdr

        Dim FBEncoding As System.Text.Encoding = System.Text.Encoding.UTF8
        Dim MailText As String
        Dim PfadTMPfile As String
        Dim tmpFileName As String
        Dim tmpFilePath As String
        Dim FBBenutzer As String
        Dim FBPasswort As String

        'C_FBox = Nothing
        'C_FBox = New FritzBox(C_XML, C_Helfer, C_Crypt)
        C_FBox.SetEventProvider(emc)
        Do While SID = C_FBox.P_DefaultSID
            FBBenutzer = InputBox("Geben Sie den Benutzernamen der Fritz!Box ein (Lassen Sie das Feld leer, falls Sie kein Benutzername benötigen.):")
            FBPasswort = InputBox("Geben Sie das Passwort der Fritz!Box ein:")
            If Len(FBPasswort) = 0 Then
                If C_Helfer.FBDB_MsgBox("Haben Sie das Passwort vergessen?", MsgBoxStyle.YesNo, "NewMail") = vbYes Then
                    Exit Sub
                End If
            End If
            SID = C_FBox.FBLogIn(NeueFW, FBBenutzer, FBPasswort)
        Loop

        If NeueFW Then
            URL = "http://" & FBOX_ADR & "/fon_num/fon_num_list.lua?sid=" & SID
        Else
            URL = "http://" & FBOX_ADR & "/cgi-bin/webcm?sid=" & SID & "&getpage=&var:lang=de&var:menu=fon&var:pagename=fondevices"
        End If
        MailText = C_Helfer.httpRead(URL, FBEncoding, Nothing)

        With My.Computer.FileSystem
            PfadTMPfile = .GetTempFileName()
            tmpFilePath = .GetFileInfo(PfadTMPfile).DirectoryName
            tmpFileName = Split(.GetFileInfo(PfadTMPfile).Name, ".", , CompareMethod.Text)(0) & "_Telefoniegeräte.htm"
            .RenameFile(PfadTMPfile, tmpFileName)
            PfadTMPfile = .GetFiles(tmpFilePath, FileIO.SearchOption.SearchTopLevelOnly, "*_Telefoniegeräte.htm")(0).ToString
            .WriteAllText(PfadTMPfile, MailText, False)
        End With
        C_OlI.NeuEmail(PfadTMPfile, C_XML.GetXMLDateiPfad, C_Helfer.GetInformationSystemFritzBox(FBOX_ADR))
    End Sub

    Public Function SetTelNrListe() As Boolean
        SetTelNrListe = False
        If Me.InvokeRequired Then
            Dim D As New DelgSetLine(AddressOf CLBTelNrAusfüllen)
            Invoke(D)
        Else
            CLBTelNrAusfüllen()
        End If
    End Function

    Public Function SetFillTelListe() As Boolean
        SetFillTelListe = False
        If Me.InvokeRequired Then
            Dim D As New DelgSetFillTelListe(AddressOf FillTelListe)
            Invoke(D)
        Else
            FillTelListe()
        End If
    End Function

    Private Sub TextChangedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles emc.GenericEvent
        StatusWert = DirectCast(sender, Control).Text
        AddLine(StatusWert)
    End Sub

    Public Function AddLine(ByVal Zeile As String) As Boolean
        AddLine = False
        StatusWert = Zeile
        If Me.InvokeRequired Then
            Dim D As New DelgSetLine(AddressOf setline)
            Invoke(D)
        Else
            setline()
        End If
    End Function

    Private Sub setline()
        Me.LTelStatus.Text = "Status: " & StatusWert
        With Me.TBDiagnose
            .Text += StatusWert & vbCrLf
            .SelectionStart = .Text.Length
            .ScrollToCaret()
        End With
    End Sub

#End Region

#Region "Kontaktindizierung"

    Sub StarteIndizierung()
        Startzeit = Date.Now
        BWIndexer = New BackgroundWorker
        Me.ProgressBarIndex.Value = 0
        Me.LabelAnzahl.Text = "Status: 0/" & CStr(Me.ProgressBarIndex.Maximum)
        Me.BIndizierungAbbrechen.Enabled = True
        Me.BIndizierungStart.Enabled = False
        Me.LabelAnzahl.Text = "Status: Bitte Warten!"
        With BWIndexer
            .WorkerSupportsCancellation = True
            .WorkerReportsProgress = True
            .RunWorkerAsync()
        End With

    End Sub

#Region "Vorbereitung"
    Private Function ErmittleKontaktanzahl() As Boolean
        ErmittleKontaktanzahl = True
        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder
        Dim LandesVW As String = Me.TBLandesVW.Text
        Anzahl = 0
        olNamespace = C_OlI.GetOutlook.GetNamespace("MAPI")

        If Me.CBKHO.Checked Then
            olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            ZähleKontakte(olfolder, Nothing)
        Else
            ZähleKontakte(Nothing, olNamespace)
        End If
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbarMax)
            Invoke(D)
        Else
            SetProgressbarMax()
        End If
    End Function

    Private Function ZähleKontakte(ByVal Ordner As Outlook.MAPIFolder, ByVal NamensRaum As Outlook.NameSpace) As Integer

        ZähleKontakte = 0
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        Dim alleTE(13) As String  ' alle TelNr/Email eines Kontakts
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                ZähleKontakte(NamensRaum.Folders.Item(j), Nothing)
                j = j + 1
            Loop
            aktKontakt = Nothing
            Return 0
        End If

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            'Debug.Print(Ordner.Name, Ordner.Items.Count)
            Anzahl += Ordner.Items.Count
        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count)
            ZähleKontakte(Ordner.Folders.Item(iOrdner), Nothing)
            iOrdner = iOrdner + 1
        Loop

        aktKontakt = Nothing
    End Function
#End Region

    Private Sub KontaktIndexer(ByVal LandesVW As String, Optional ByVal Ordner As Outlook.MAPIFolder = Nothing, Optional ByVal NamensRaum As Outlook.NameSpace = Nothing) 'as Boolean
        'KontaktIndexer = False
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        'Dim item As Object      ' aktuelles Element
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                KontaktIndexer(LandesVW, NamensRaum.Folders.Item(j))
                j = j + 1
            Loop
            aktKontakt = Nothing
            'Return True
        Else
            If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem And Not BWIndexer.CancellationPending Then
                'C_Kontakte.IndiziereOrdner(Ordner)
                For Each item In Ordner.Items
                    ' nur Kontakte werden durchsucht
                    If TypeOf item Is Outlook.ContactItem Then
                        aktKontakt = CType(item, Outlook.ContactItem)

                        'With aktKontakt
                        'KontaktName = " (" & aktKontakt.FullNameAndCompany & ")"
                        KontaktName = " (" & aktKontakt.FullName & ")"
                        C_Kontakte.IndiziereKontakt(aktKontakt, False)
                        BWIndexer.ReportProgress(1)
                        If BWIndexer.CancellationPending Then Exit For
                    Else
                        BWIndexer.ReportProgress(1)
                    End If
                    C_Helfer.NAR(item)
                    Windows.Forms.Application.DoEvents()
                Next 'Item
                'Elemente = Nothing
            End If

            ' Unterordner werden rekursiv durchsucht
            iOrdner = 1
            Do While (iOrdner <= Ordner.Folders.Count) And Not BWIndexer.CancellationPending
                KontaktIndexer(LandesVW, Ordner.Folders.Item(iOrdner))
                iOrdner = iOrdner + 1
            Loop
            aktKontakt = Nothing
        End If
    End Sub

    Private Sub KontaktDeIndexer(ByVal Ordner As Outlook.MAPIFolder, ByVal NamensRaum As Outlook.NameSpace) 'As Boolean

        'KontaktDeIndexer = False
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        'Dim item As Object      ' aktuelles Element
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                KontaktDeIndexer(NamensRaum.Folders.Item(j), Nothing)
                j = j + 1
            Loop
            aktKontakt = Nothing
            'Return True
        Else

            'If BWIndexer.CancellationPending Then Exit Function

            If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem And Not BWIndexer.CancellationPending Then
                For Each item In Ordner.Items
                    ' nur Kontakte werden durchsucht
                    If TypeOf item Is Outlook.ContactItem Then
                        aktKontakt = CType(item, Outlook.ContactItem)

                        'With aktKontakt
                        'KontaktName = " (" & aktKontakt.FullNameAndCompany & ")"
                        KontaktName = " (" & aktKontakt.FullName & ")"
                        C_Kontakte.DeIndizierungKontakt(aktKontakt, False)
                        BWIndexer.ReportProgress(-1)
                        If BWIndexer.CancellationPending Then Exit For
                    Else
                        BWIndexer.ReportProgress(-1)
                    End If
                    C_Helfer.NAR(item)
                    Windows.Forms.Application.DoEvents()
                Next 'Item
                C_Kontakte.DeIndizierungOrdner(Ordner)
            End If
            ' Unterordner werden rekursiv durchsucht
            iOrdner = 1
            Do While (iOrdner <= Ordner.Folders.Count) And Not BWIndexer.CancellationPending
                KontaktDeIndexer(Ordner.Folders.Item(iOrdner), Nothing)
                iOrdner = iOrdner + 1
            Loop
            aktKontakt = Nothing
        End If
    End Sub
#End Region

#Region "Logging"
    Sub FillLogTB()
        Dim LogDatei As String = C_Helfer.Dateipfade("LogDatei")

        If C_XML.P_CBLogFile Then
            If My.Computer.FileSystem.FileExists(LogDatei) Then
                Me.TBLogging.Text = My.Computer.FileSystem.OpenTextFileReader(LogDatei).ReadToEnd
            End If
        End If
        Me.LinkLogFile.Text = LogDatei
    End Sub

    Private Sub FBDB_MP_TabIndexChanged(sender As Object, e As EventArgs) Handles FBDB_MP.SelectedIndexChanged
        Me.Update()
        If Me.FBDB_MP.SelectedTab.Name = "PLogging" Then
            With Me.TBLogging
                .Focus()
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End With
        End If
    End Sub

    Private Sub BLogging_Click(sender As Object, e As EventArgs) Handles BLogging.Click
        With Me.TBLogging
            If .SelectedText = vbNullString Then
                My.Computer.Clipboard.SetText(.Text)
            Else
                My.Computer.Clipboard.SetText(.SelectedText)
            End If
        End With
    End Sub

#End Region

#Region "Delegate"
    Private Sub SetProgressbar()
        With Me.ProgressBarIndex
            .Value += CInt(StatusWert)
            Me.LabelAnzahl.Text = "Status: " & .Value & "/" & CStr(.Maximum) & KontaktName
        End With
    End Sub

    Private Sub SetProgressbarToMax()
        With Me.ProgressBarIndex
            If Me.RadioButtonErstelle.Checked And Not Me.RadioButtonEntfernen.Checked Then
                .Value = .Maximum
            ElseIf Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
                .Value = 0
            End If
        End With
        Me.BIndizierungStart.Enabled = True
        Me.BIndizierungAbbrechen.Enabled = False
    End Sub

    Private Sub SetProgressbarMax()
        Me.ProgressBarIndex.Maximum = Anzahl
    End Sub

    Private Sub DelBTelefonliste()
        If Me.InvokeRequired Then
            Dim D As New DelgButtonTelEinl(AddressOf DelBTelefonliste)
            Me.Invoke(D)
        Else
            Me.BTelefonliste.Text = "Telefone erneut einlesen"
            Me.BTelefonliste.Enabled = True
        End If
    End Sub

#End Region

#Region "BackGroundWorker - hHandle"
    Private Sub BWIndexer_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWIndexer.DoWork

        ErmittleKontaktanzahl()
        If Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
            StatusWert = Me.ProgressBarIndex.Maximum.ToString
            BWIndexer.ReportProgress(Me.ProgressBarIndex.Maximum)
        End If

        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder
        Dim LandesVW As String = Me.TBLandesVW.Text

        olNamespace = C_OlI.GetOutlook.GetNamespace("MAPI")

        If Me.CBKHO.Checked Then
            olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            If Me.RadioButtonErstelle.Checked Then
                KontaktIndexer(LandesVW, Ordner:=olfolder)
            ElseIf Me.RadioButtonEntfernen.Checked Then
                KontaktDeIndexer(olfolder, Nothing)
            End If
        Else
            If Me.RadioButtonErstelle.Checked Then
                KontaktIndexer(LandesVW, NamensRaum:=olNamespace)
            ElseIf Me.RadioButtonEntfernen.Checked Then
                KontaktDeIndexer(Nothing, olNamespace)
            End If
        End If
    End Sub

    Private Sub BWIndexer_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BWIndexer.ProgressChanged
        StatusWert = CStr(e.ProgressPercentage)
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbar)
            Invoke(D)
        Else
            SetProgressbar()
        End If
    End Sub

    Private Sub BWIndexer_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWIndexer.RunWorkerCompleted

        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbarToMax)
            Invoke(D)
        Else
            SetProgressbarToMax()
        End If
        BWIndexer.Dispose()
        Dauer = Date.Now - Startzeit
        If Me.RadioButtonErstelle.Checked And Not Me.RadioButtonEntfernen.Checked Then
            C_XML.P_LLetzteIndizierung = Date.Now
            C_Helfer.LogFile("Indizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        ElseIf Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
            C_Helfer.LogFile("Deindizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        End If
    End Sub

    Private Sub BWTelefone_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWTelefone.DoWork
        AddLine("Einlesen der Telefone gestartet.")
        C_FBox.P_SpeichereDaten = CBool(e.Argument)
        e.Result = CBool(e.Argument)
        If Me.TBTelefonDatei.Text = vbNullString Then
            C_FBox.FritzBoxDaten()
        Else
            C_FBox.FritzBoxDatenDebug(Me.TBTelefonDatei.Text)
        End If
    End Sub

    Private Sub BWTelefone_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWTelefone.RunWorkerCompleted
        AddLine("BackgroundWorker ist fertig.")
        Dim xPathTeile As New ArrayList
        Dim tmpTelefon As String

        'Statistik zurückschreiben
        With xPathTeile
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("[@Dialport = """ & """]")
            .Add("TelName")

            For Row = 0 To TelList.Rows.Count - 2
                .Item(.Count - 2) = "[@Dialport = """ & TelList.Rows(Row).Cells(2).Value.ToString & """]"
                .Item(.Count - 1) = "TelName"
                ' Prüfe ob Telefonname und Telefonnummer übereinstimmt
                tmpTelefon = C_XML.Read(xPathTeile, "-1")
                If Not tmpTelefon = "-1" Then
                    .Item(.Count - 1) = "TelNr"
                    If (TelList.Rows(Row).Cells(4).Value Is Nothing) Or (TelList.Rows(Row).Cells(5).Value Is Nothing) Then
                        If tmpTelefon = TelList.Rows(Row).Cells(4).Value.ToString And _
                            C_XML.Read(xPathTeile, "-1") = Replace(TelList.Rows(Row).Cells(5).Value.ToString, ", ", ";", , , CompareMethod.Text) Then
                            Dim Dauer As Date
                            .Item(.Count - 1) = "Eingehend"
                            Dauer = CDate(TelList.Rows(Row).Cells(6).Value.ToString())
                            C_XML.Write(xPathTeile, CStr((Dauer.Hour * 60 + Dauer.Minute) * 60 + Dauer.Second))
                            .Item(.Count - 1) = "Ausgehend"
                            Dauer = CDate(TelList.Rows(Row).Cells(7).Value.ToString())
                            C_XML.Write(xPathTeile, CStr((Dauer.Hour * 60 + Dauer.Minute) * 60 + Dauer.Second))
                        End If
                    End If
                End If
            Next

            'CLBTelNrAusfüllen setzen
            .Clear()
            Dim CheckTelNr As CheckedListBox.CheckedItemCollection = Me.CLBTelNr.CheckedItems
            If Not CheckTelNr.Count = 0 Then
                Dim tmpTeile As String = vbNullString
                .Add("Telefone")
                .Add("Nummern")
                .Add("*")
                For i = 0 To CheckTelNr.Count - 1
                    tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
                Next
                tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
                .Add("[" & tmpTeile & "]")
                C_XML.WriteAttribute(xPathTeile, "Checked", "1")
            End If
        End With

        SetTelNrListe()
        SetFillTelListe()
        DelBTelefonliste()
        BWTelefone = Nothing
        AddLine("BackgroundWorker wurde eliminiert.")
        If CBool(e.Result) Then AddLine("Das Einlesen der Telefone ist abgeschlossen.")
    End Sub
#End Region

#Region "Phoner"
    'Phoner
    'Private Sub CBKeineFB_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    If Me.CBPhonerKeineFB.Checked Then Me.CBJImport.Checked = False
    '    Me.CBJImport.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.ButtonTelefonliste.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.TBFBAdr.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.CBForceFBAddr.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.TBPasswort.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.lblTBPasswort.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.CBPhonerAnrMon.Checked = Me.CBPhonerKeineFB.Checked
    '    Me.CBPhonerAnrMon.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.ComboBoxPhonerSIP.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    Me.CBPhoner.Enabled = Not Me.CBPhonerKeineFB.Checked
    '    If Me.CBPhonerKeineFB.Checked Then
    '        Me.CBPhoner.Checked = True
    '        Me.ComboBoxPhonerSIP.SelectedIndex = 0
    '        Me.CLBTelNr.SetItemChecked(0, True)
    '        For i = 0 To TelList.Rows.Count - 1
    '            TelList.Rows(i).Cells(0).Value = False
    '        Next
    '    End If
    '    Me.CLBTelNr.Enabled = Not Me.CBPhonerKeineFB.Checked
    'End Sub

    Private Sub LinkPhoner_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkPhoner.LinkClicked
        System.Diagnostics.Process.Start("http://www.phoner.de/")
    End Sub

    Private Sub ButtonPhoner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonPhoner.Click
        Dim PhonerInstalliert As Boolean = C_Phoner.PhonerReady()
        Me.PanelPhonerAktiv.BackColor = CType(IIf(PhonerInstalliert, Color.LightGreen, Color.Red), Color)
        Me.LabelPhoner.Text = "Phoner ist " & CStr(IIf(PhonerInstalliert, "", "nicht ")) & "aktiv."
        Me.PanelPhoner.Enabled = PhonerInstalliert
        C_XML.P_PhonerVerfügbar = PhonerInstalliert
    End Sub

    Private Sub CBPhoner_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBPhoner.CheckedChanged
        Me.TBPhonerPasswort.Enabled = Me.CBPhoner.Checked
        Me.LPassworPhoner.Enabled = Me.CBPhoner.Checked
    End Sub
#End Region
End Class

Public NotInheritable Class iTa
    ' Callers do not require Unmanaged permission       
    Public Shared ReadOnly Property IsThemeActive() As Boolean
        Get
            ' No need to demand a permission in place of               
            ' UnmanagedCode as GetTickCount is considered               
            ' a safe method               
            Return SafeNativeMethods.IsThemeActive()
        End Get
    End Property

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class


