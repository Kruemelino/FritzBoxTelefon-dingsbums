Imports System.Drawing
Imports System.ComponentModel 'BackgroundWorker
Imports System.Threading
Imports System.Windows.Forms

Friend Class formCfg
#Region "Eigene Klassen"
    Private C_DP As DataProvider
    Private C_Crypt As MyRijndael
    Private C_hf As Helfer
    Private C_KF As Contacts
    Private C_Phoner As PhonerInterface
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_AnrMon As AnrufMonitor
    Private C_FBox As FritzBox
#End Region

#Region "BackgroundWorker"
    Private WithEvents BWTelefone As BackgroundWorker
    Private WithEvents BWIndexer As BackgroundWorker
    Private WithEvents BWTreeView As BackgroundWorker
#End Region

#Region "Delegaten"
    Private Delegate Sub DelgButtonTelEinl()
    Private Delegate Sub DelgSetLine()
    Private Delegate Sub DelgSetFillTelListe()
    Private Delegate Sub DelgStatistik()
    Private Delegate Sub DelgSetProgressbar()
    Private Delegate Sub DelgSetTreeView()
#End Region

#Region "EventMulticaster"
    Private WithEvents emc As New EventMulticaster
#End Region

#Region "Eigene Variablen"
    Private StatusWert As String
    Private KontaktName As String
    Private Anzahl As Integer = 0
    Private Startzeit As Date
    Private Dauer As TimeSpan
#End Region

    Public Sub New(ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal XMLKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal CryptKlasse As MyRijndael, _
                   ByVal AnrufMon As AnrufMonitor, _
                   ByVal fritzboxKlasse As FritzBox, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal kontaktklasse As Contacts, _
                   ByVal Phonerklasse As PhonerInterface)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        C_hf = HelferKlasse
        C_DP = XMLKlasse
        C_Crypt = CryptKlasse
        C_GUI = InterfacesKlasse
        C_OlI = OutlInter
        C_AnrMon = AnrufMon
        C_FBox = fritzboxKlasse
        C_KF = kontaktklasse
        C_Phoner = Phonerklasse
        Me.LVersion.Text += ThisAddIn.Version
        With Me.ComboBoxRWS.Items
            .Add("DasÖrtliche.de")
            .Add("11880.com")
            .Add("DasTelefonbuch.de")
            .Add("tel.search.ch")
            .Add("Alle Suchmaschinen")
        End With
    End Sub

    Private Sub UserForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.TBAnrMonMoveGeschwindigkeit.BackColor = CType(IIf(OutlookSecurity.IsThemeActive, SystemColors.ControlLightLight, SystemColors.ControlLight), Color)
        Me.BAnrMonTest.Enabled = Not C_AnrMon Is Nothing
        Me.BTelefonliste.Enabled = Not C_FBox Is Nothing
        Me.FBDB_MP.SelectedIndex = 0
        Ausfüllen()
    End Sub

#Region "Ausfüllen"

    Private Sub Ausfüllen()
        Me.ToolTipFBDBConfig.SetToolTip(Me.BXML, "Öffnet die Datei " & vbCrLf & C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Config_FileName)
#If OVer >= 14 Then
        If Not Me.FBDB_MP.TabPages.Item("PSymbolleiste") Is Nothing Then Me.FBDB_MP.TabPages.Remove(Me.FBDB_MP.TabPages.Item("PSymbolleiste"))
#End If
        ' Beim Einblenden die Werte aus der Registry einlesen
        ' Einstellungen für das Wählmakro laden
        Me.TBLandesVW.Text = C_DP.P_TBLandesVW

        Me.TBAmt.Text = CStr(IIf(C_DP.P_TBAmt = C_DP.P_Def_ErrorMinusOne_String, "", C_DP.P_TBAmt))
        Me.TBFBAdr.Text = C_DP.P_TBFBAdr

        Me.CBForceFBAddr.Checked = C_DP.P_CBForceFBAddr
        Me.TBBenutzer.Text = C_DP.P_TBBenutzer
        If Not Me.TBBenutzer.Text = C_DP.P_Def_StringEmpty Then
            If C_DP.Read("Optionen", Me.TBBenutzer.Text, "2") = "0" Then
                Me.TBBenutzer.BackColor = Color.Red
                Me.ToolTipFBDBConfig.SetToolTip(Me.TBBenutzer, "Der Benutzer " & Me.TBBenutzer.Text & " hat keine ausreichenden Berechtigungen auf der Fritz!Box.")
            End If
        End If

        If Not Len(C_DP.P_TBPasswort) = 0 Then Me.TBPasswort.Text = "1234"
        Me.TBVorwahl.Text = C_DP.P_TBVorwahl
        Me.TBEnblDauer.Text = CStr(C_DP.P_TBEnblDauer)
        Me.CBAnrMonAuto.Checked = C_DP.P_CBAnrMonAuto
        Me.TBAnrMonX.Text = CStr(C_DP.P_TBAnrMonX)
        Me.TBAnrMonY.Text = CStr(C_DP.P_TBAnrMonY)
        Me.CBAnrMonMove.Checked = C_DP.P_CBAnrMonMove
        Me.CBAnrMonTransp.Checked = C_DP.P_CBAnrMonTransp
        Me.TBAnrMonMoveGeschwindigkeit.Value = C_DP.P_TBAnrMonMoveGeschwindigkeit
        Me.CBoxAnrMonStartPosition.SelectedIndex = C_DP.P_CBoxAnrMonStartPosition
        Me.CBoxAnrMonMoveDirection.SelectedIndex = C_DP.P_CBoxAnrMonMoveDirection
        Me.CBAnrMonZeigeKontakt.Checked = C_DP.P_CBAnrMonZeigeKontakt
        Me.CBAnrMonContactImage.Checked = C_DP.P_CBAnrMonContactImage
        Me.CBIndexAus.Checked = C_DP.P_CBIndexAus
        Me.CBShowMSN.Checked = C_DP.P_CBShowMSN
        ' optionale allgemeine Einstellungen laden
        Me.CBAutoClose.Checked = C_DP.P_CBAutoClose
        Me.CBVoIPBuster.Checked = C_DP.P_CBVoIPBuster
        Me.CBCbCunterbinden.Checked = C_DP.P_CBCbCunterbinden
        Me.CBCallByCall.Checked = C_DP.P_CBCallByCall
        Me.CBDialPort.Checked = C_DP.P_CBDialPort
        Me.CBRWS.Checked = C_DP.P_CBRWS
        Me.CBKErstellen.Checked = C_DP.P_CBKErstellen
        Me.CBLogFile.Checked = C_DP.P_CBLogFile
#If OVer < 14 Then
        ' Einstellungen für die Symbolleiste laden
        Me.CBSymbWwdh.Checked = C_DP.P_CBSymbWwdh
        Me.CBSymbAnrMon.Checked = C_DP.P_CBSymbAnrMon
        Me.CBSymbAnrMonNeuStart.Checked = C_DP.P_CBSymbAnrMonNeuStart
        Me.CBSymbAnrListe.Checked = C_DP.P_CBSymbAnrListe
        Me.CBSymbDirekt.Checked = C_DP.P_CBSymbDirekt
        Me.CBSymbRWSuche.Checked = C_DP.P_CBSymbRWSuche
        Me.CBSymbVIP.Checked = C_DP.P_CBSymbVIP '
        Me.CBSymbJournalimport.Checked = C_DP.P_CBSymbJournalimport
#End If
        Me.CBJImport.Checked = C_DP.P_CBJImport
        ' Einstellungen füer die Rückwärtssuche laden
        Me.CBKHO.Checked = C_DP.P_CBKHO
        Me.CBRWSIndex.Checked = C_DP.P_CBRWSIndex

        Me.ComboBoxRWS.SelectedItem = Me.ComboBoxRWS.Items.Item(C_DP.P_ComboBoxRWS)
        If Not Me.CBRWS.Checked Then Me.ComboBoxRWS.Enabled = False
        ' Einstellungen für das Journal laden

        Me.CBJournal.Checked = C_DP.P_CBJournal
        Me.CBUseAnrMon.Checked = C_DP.P_CBUseAnrMon
        Me.CBCheckMobil.Checked = C_DP.P_CBCheckMobil

        Me.CBIndexAus.Enabled = Not Me.CBUseAnrMon.Checked
        Me.PanelAnrMon.Enabled = Me.CBUseAnrMon.Checked
        'StoppUhr
        Me.CBStoppUhrEinblenden.Checked = C_DP.P_CBStoppUhrEinblenden
        Me.CBStoppUhrAusblenden.Checked = C_DP.P_CBStoppUhrAusblenden
        Me.TBStoppUhr.Text = CStr(C_DP.P_TBStoppUhr)

        Me.CBStoppUhrAusblenden.Enabled = Me.CBStoppUhrEinblenden.Checked
        If Not Me.CBStoppUhrEinblenden.Checked Then Me.CBStoppUhrAusblenden.Checked = False
        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked

        'Telefonnummernformat
        Me.TBTelNrMaske.Text = C_DP.P_TBTelNrMaske
        Me.CBTelNrGruppieren.Checked = C_DP.P_CBTelNrGruppieren
        Me.CBintl.Checked = C_DP.P_CBintl
        Me.CBIgnoTelNrFormat.Checked = C_DP.P_CBIgnoTelNrFormat

#If OVer < 14 Then
        If Not Me.CBJournal.Checked Then Me.CBSymbJournalimport.Checked = False
        Me.CBSymbJournalimport.Enabled = Me.CBJournal.Checked
#End If
        'Phoner
        Dim PhonerVerfuegbar As Boolean = C_DP.P_PhonerVerfügbar
        Me.PanelPhoner.Enabled = PhonerVerfuegbar
        If PhonerVerfuegbar Then
            Me.CBPhoner.Checked = C_DP.P_CBPhoner
        Else
            Me.CBPhoner.Checked = False
        End If
        Me.LabelPhoner.Text = Replace(Me.LabelPhoner.Text, " [nicht]", CStr(IIf(PhonerVerfuegbar, "", " nicht")), , , CompareMethod.Text)
        'Me.CBPhonerKeineFB.Checked = CBool(IIf(C_DP.Read("Phoner", "CBPhonerKeineFB", "False") = "True", True, False))
        'If Not Me.CBPhonerKeineFB.Checked Then

        Dim xPathTeile As New ArrayList
        Dim tmpTelefon As String

        'Statistik zurückschreiben
        With xPathTeile
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("[@Dialport > 19 and @Dialport < 30]") ' Nur IP-Telefone
            .Add("TelName")
        End With

        tmpTelefon = C_DP.Read(xPathTeile, "Phoner")
        If InStr(tmpTelefon, ";", CompareMethod.Text) = 0 Then
            Me.ComboBoxPhonerSIP.Items.Add(tmpTelefon)
        Else
            Me.ComboBoxPhonerSIP.DataSource = Split(tmpTelefon, ";", , CompareMethod.Text)
        End If

        If Not Me.ComboBoxPhonerSIP.Items.Count = 0 Then
            Me.ComboBoxPhonerSIP.SelectedIndex = C_DP.P_ComboBoxPhonerSIP
        End If


        'Else
        'Me.ComboBoxPhonerSIP.SelectedIndex = 0
        'Me.ComboBoxPhonerSIP.Enabled = False
        'End If
        Me.CBPhonerAnrMon.Checked = C_DP.P_CBPhonerAnrMon
        If Not Len(C_DP.P_TBPhonerPasswort) = 0 Then Me.TBPhonerPasswort.Text = "1234"

        Dim PhonerInstalliert As Boolean = C_Phoner.PhonerReady()
        Me.PanelPhonerAktiv.BackColor = CType(IIf(PhonerInstalliert, Color.LightGreen, Color.Red), Color)
        Me.LabelPhoner.Text = "Phoner ist " & CStr(IIf(PhonerInstalliert, "", "nicht ")) & "aktiv."
        Me.PanelPhoner.Enabled = PhonerInstalliert
        C_DP.P_PhonerVerfügbar = PhonerInstalliert
        ' Notiz
        Me.CBNote.Checked = C_DP.P_CBNote
        'TreeView
        Me.TVOutlookContact.Enabled = False
        If Me.TVOutlookContact.Nodes.Count > 0 Then Me.TVOutlookContact.Nodes.Clear()
        BWTreeView = New BackgroundWorker
        With BWTreeView
            .WorkerReportsProgress = False
            .RunWorkerAsync(True)
        End With

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
        Nebenstellen = Split(C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne_String & ";"), ";", , CompareMethod.Text)

        If Not Nebenstellen(0) = C_DP.P_Def_ErrorMinusOne_String Then
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
                        Zeile.Add(CBool(C_DP.Read(xPathTeile, "False")))
                        Zeile.Add(CStr(j))
                        .Item(.Count - 1) = "@Dialport"
                        Zeile.Add(C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne_String & ";")) 'Nebenstelle
                        .RemoveAt(.Count - 1)
                        Zeile.Add(C_DP.ReadElementName(xPathTeile, C_DP.P_Def_ErrorMinusOne_String & ";")) 'Telefontyp
                        Zeile.Add(Nebenstelle) ' TelName
                        .Add("TelNr")
                        Zeile.Add(Replace(C_DP.Read(xPathTeile, "-"), ";", ", ", , , CompareMethod.Text)) 'TelNr
                        .Item(.Count - 1) = "Eingehend"
                        Zeile.Add(C_DP.Read(xPathTeile, "0")) 'Eingehnd
                        tmpein(0) += CDbl(Zeile.Item(Zeile.Count - 1))
                        .Item(.Count - 1) = "Ausgehend"
                        Zeile.Add(C_DP.Read(xPathTeile, "0")) 'Ausgehnd
                        tmpein(1) += CDbl(Zeile.Item(Zeile.Count - 1))
                        Zeile.Add(CStr(CDbl(Zeile.Item(Zeile.Count - 2)) + CDbl(Zeile.Item(Zeile.Count - 1)))) 'Gesamt
                        tmpein(2) += CDbl(Zeile.Item(Zeile.Count - 1))
                        For i = Zeile.Count - 3 To Zeile.Count - 1
                            Zeile.Item(i) = C_hf.GetTimeInterval(CInt(Zeile.Item(i)))
                        Next
                    End With
                    .Rows.Add(Zeile.ToArray)
                    Zeile.Clear()
                Next
                Zeile.Add(False)
                Zeile.Add(C_DP.P_Def_StringEmpty)
                Zeile.Add(C_DP.P_Def_StringEmpty)
                Zeile.Add(C_DP.P_Def_StringEmpty)
                Zeile.Add(C_DP.P_Def_StringEmpty)
                Zeile.Add("Gesamt:")
                For i = 0 To 2
                    Zeile.Add(C_hf.GetTimeInterval(tmpein(i)))
                Next

                .Rows.Add(Zeile.ToArray)
            End With
        End If

        Me.TBAnderes.Text = C_DP.P_StatVerpasst & " verpasste Telefonate" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_DP.P_StatNichtErfolgreich & " nicht erfolgreiche Telefonate" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_DP.P_StatKontakt & " erstellte Kontakte" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_DP.P_StatJournal & " erstellte Journaleinträge" & vbCrLf
        Me.TBReset.Text = "Letzter Reset: " & C_DP.P_StatResetZeit
        Me.TBSchließZeit.Text = "Letzter Journaleintrag: " & C_DP.P_StatOLClosedZeit
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

            TelNrString = Split("Alle Telefonnummern;" & C_DP.Read(xPathTeile, ""), ";", , CompareMethod.Text)

            TelNrString = (From x In TelNrString Select x Distinct).ToArray 'Doppelte entfernen
            TelNrString = (From x In TelNrString Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray ' Leere entfernen
            Me.CLBTelNr.Items.Clear()

            For Each TelNr In TelNrString
                Me.CLBTelNr.Items.Add(TelNr)
            Next
            'etwas unschön
            .Add("")
            For i = 1 To Me.CLBTelNr.Items.Count - 1
                .Item(.Count - 2) = "*[. = """ & Me.CLBTelNr.Items(i).ToString & """]"
                .Item(.Count - 1) = "@Checked"
                Me.CLBTelNr.SetItemChecked(i, C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)))
            Next
        End With
        Me.CLBTelNr.SetItemChecked(0, Me.CLBTelNr.CheckedItems.Count = Me.CLBTelNr.Items.Count - 1)
    End Sub

#End Region

    Private Function Speichern() As Boolean
        Speichern = True
        Dim xPathTeile As New ArrayList
        Dim tmpTeile As String = C_DP.P_Def_StringEmpty
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
                C_DP.WriteAttribute(xPathTeile, "Checked", "0")
                tmpTeile = C_DP.P_Def_StringEmpty
                For i = 0 To CheckTelNr.Count - 1
                    tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
                Next
                tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
                .Item(.Count - 1) = "[" & tmpTeile & "]"
                C_DP.WriteAttribute(xPathTeile, "Checked", "1")
            End With
        End If

        ' Sichert die Einstellungen und schließt das Fenster
        If (CInt(Me.TBEnblDauer.Text) < 4) Then Me.TBEnblDauer.Text = "4"
        With C_DP

            .P_CBForceFBAddr = Me.CBForceFBAddr.Checked

            If Me.TBBenutzer.Text = C_DP.P_Def_StringEmpty Then
                With xPathTeile
                    .Clear()
                    .Add("Optionen")
                    .Add("TBBenutzer")
                End With
                C_DP.Delete(xPathTeile)
            Else
                .P_TBBenutzer = Me.TBBenutzer.Text
            End If
            If Not Me.TBPasswort.Text = "1234" Then
                .P_TBPasswort = C_Crypt.EncryptString128Bit(Me.TBPasswort.Text, C_DP.P_Def_PassWordDecryptionKey)
                C_DP.SaveSettingsVBA("Zugang", C_DP.P_Def_PassWordDecryptionKey)
                C_hf.KeyChange()
            End If
            ' StoppUhr
            If Not Me.TBStoppUhr.Text = C_DP.P_Def_StringEmpty Then
                If CInt(Me.TBStoppUhr.Text) < 0 Then
                    Me.TBStoppUhr.Text = "10"
                End If
            Else
                Me.TBStoppUhr.Text = "10"
            End If

            .P_TBLandesVW = Me.TBLandesVW.Text
            .P_TBAmt = CStr(IIf(Me.TBAmt.Text = C_DP.P_Def_StringEmpty, C_DP.P_Def_ErrorMinusOne_String, Me.TBAmt.Text))
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
            .P_CBoxAnrMonMoveDirection = Me.CBoxAnrMonMoveDirection.SelectedIndex
            .P_CBoxAnrMonStartPosition = Me.CBoxAnrMonStartPosition.SelectedIndex
            .P_CBAnrMonZeigeKontakt = Me.CBAnrMonZeigeKontakt.Checked
            .P_CBIndexAus = Me.CBIndexAus.Checked
            .P_CBShowMSN = Me.CBShowMSN.Checked
            .P_CBVoIPBuster = Me.CBVoIPBuster.Checked
            .P_CBDialPort = Me.CBDialPort.Checked
            .P_CBCbCunterbinden = Me.CBCbCunterbinden.Checked
            .P_CBCallByCall = Me.CBCallByCall.Checked
            .P_CBRWS = Me.CBRWS.Checked
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
            ' Notiz
            .P_CBNote = Me.CBNote.Checked

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
                .Add(C_DP.P_Def_StringEmpty)
                For i = 0 To TelList.Rows.Count - 2
                    .Item(.Count - 1) = "[@Dialport = """ & TelList.Rows(i).Cells(2).Value.ToString & """]"
                    C_DP.WriteAttribute(xPathTeile, "Standard", CStr(CBool(TelList.Rows(i).Cells(0).Value)))
                Next
            End With

            With xPathTeile
                .Clear()
                .Add("Telefone")
                .Add("Nummern")
                .Add("*")
                .Add("[@Checked=""1""]")
            End With
            .P_CLBTelNr = (From x In Split(.Read(xPathTeile, .P_Def_ErrorMinusOne_String), ";", , CompareMethod.Text) Select x Distinct).ToArray

            ' Phoner
            Dim TelName() As String
            Dim PhonerTelNameIndex As Integer = 0

            For i = 20 To 29
                TelName = Split(C_DP.Read("Telefone", CStr(i), "-1;;"), ";", , CompareMethod.Text)
                If Not TelName(0) = C_DP.P_Def_ErrorMinusOne_String And Not ComboBoxPhonerSIP.SelectedItem Is Nothing And Not TelName.Length = 2 Then
                    If TelName(2) = ComboBoxPhonerSIP.SelectedItem.ToString Then
                        PhonerTelNameIndex = i
                        Exit For
                    End If
                End If
            Next
            .P_PhonerTelNameIndex = PhonerTelNameIndex
            'ThisAddIn.NutzePhonerOhneFritzBox = Me.CBPhonerKeineFB.Checked
            If Me.TBPhonerPasswort.Text = C_DP.P_Def_StringEmpty And Me.CBPhoner.Checked Then
                If C_hf.FBDB_MsgBox("Es wurde kein Passwort für Phoner eingegeben! Da Wählen über Phoner wird nicht funktionieren!", MsgBoxStyle.OkCancel, "Speichern") = MsgBoxResult.Cancel Then
                    Speichern = False
                End If
            End If

            If Me.CBPhoner.Checked Then
                If Not Me.TBPhonerPasswort.Text = C_DP.P_Def_StringEmpty Then
                    If Not Me.TBPhonerPasswort.Text = "1234" Then
                        .P_TBPhonerPasswort = C_Crypt.EncryptString128Bit(Me.TBPhonerPasswort.Text, C_DP.P_Def_PassWordDecryptionKey)
                        C_DP.SaveSettingsVBA("ZugangPasswortPhoner", C_DP.P_Def_PassWordDecryptionKey)
                        C_hf.KeyChange()
                    End If
                End If
            End If
            If Not Me.TVOutlookContact.SelectedNode Is Nothing Then
                .P_TVKontaktOrdnerEntryID = Split(CStr(Me.TVOutlookContact.SelectedNode.Tag), ";", , CompareMethod.Text)(0)
                .P_TVKontaktOrdnerStoreID = Split(CStr(Me.TVOutlookContact.SelectedNode.Tag), ";", , CompareMethod.Text)(1)
            Else
                C_KF.GetOutlookFolder(.P_TVKontaktOrdnerEntryID, .P_TVKontaktOrdnerStoreID)
            End If

            .SpeichereXMLDatei()
            C_DP.P_ValidFBAdr = C_hf.ValidIP(C_DP.P_TBFBAdr)
        End With
    End Function

#Region "Button Link"
    Private Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BReset.Click, _
                                                                                   BOK.Click, _
                                                                                   BAbbruch.Click, _
                                                                                   BApply.Click, _
                                                                                   BXML.Click, _
                                                                                   BAnrMonTest.Click, _
                                                                                   BIndizierungStart.Click, _
                                                                                   BIndizierungAbbrechen.Click, _
                                                                                   BZwischenablage.Click, _
                                                                                   BTelefonliste.Click, _
                                                                                   BTelefonDatei.Click, _
                                                                                   BStartDebug.Click, _
                                                                                   BResetStat.Click, _
                                                                                   BProbleme.Click, _
                                                                                   BStoppUhrAnzeigen.Click, _
                                                                                   BArbeitsverzeichnis.Click, _
                                                                                   BRWSTest.Click, _
                                                                                   BTestLogin.Click

        Select Case CType(sender, Windows.Forms.Button).Name
            Case "BReset"
                ' Startwerte zurücksetzen
                ' Einstellungen für das Wählmakro zurücksetzen
                With C_DP
                    Me.TBLandesVW.Text = .P_Def_TBLandesVW
                    Me.TBAmt.Text = .P_Def_StringEmpty
                    Me.CBCheckMobil.Checked = .P_Def_CBCheckMobil

                    ' Einstellungen für den Anrufmonitor zurücksetzen
                    Me.TBEnblDauer.Text = CStr(.P_Def_TBEnblDauer)
                    Me.TBAnrMonX.Text = CStr(.P_Def_TBAnrMonX)
                    Me.TBAnrMonY.Text = CStr(.P_Def_TBAnrMonY)
                    Me.CBAnrMonAuto.Checked = .P_Def_CBAnrMonAuto
                    Me.CBAutoClose.Checked = .P_Def_CBAutoClose
                    Me.CBAnrMonMove.Checked = .P_Def_CBAnrMonMove
                    Me.CBAnrMonTransp.Checked = .P_Def_CBAnrMonTransp
                    Me.CBAnrMonContactImage.Checked = .P_Def_CBAnrMonContactImage
                    Me.CBShowMSN.Checked = .P_Def_CBShowMSN
                    Me.TBAnrMonMoveGeschwindigkeit.Value = .P_Def_TBAnrMonMoveGeschwindigkeit
                    Me.CBoxAnrMonMoveDirection.SelectedIndex = .P_Def_CBoxAnrMonMoveDirection
                    Me.CBoxAnrMonStartPosition.SelectedIndex = .P_Def_CBoxAnrMonStartPosition
                    Me.CBAnrMonZeigeKontakt.Checked = .P_Def_CBAnrMonZeigeKontakt
                    Me.CBIndexAus.Checked = .P_Def_CBIndexAus
                    ' optionale allgemeine Einstellungen zuruecksetzen
                    Me.CBVoIPBuster.Checked = .P_Def_CBVoIPBuster
                    Me.CBDialPort.Checked = .P_Def_CBDialPort
                    Me.CBCallByCall.Checked = .P_Def_CBCallByCall
                    Me.CBCbCunterbinden.Checked = .P_Def_CBCbCunterbinden
                    Me.CBKErstellen.Checked = .P_Def_CBKErstellen
                    Me.CBLogFile.Checked = .P_Def_CBLogFile
                    Me.CBForceFBAddr.Checked = .P_Def_CBForceFBAddr
#If OVer < 14 Then
                    ' Einstellungen für die Symbolleiste zurücksetzen
                    Me.CBSymbAnrMonNeuStart.Checked = .P_Def_CBSymbAnrMonNeuStart
                    Me.CBSymbWwdh.Checked = .P_Def_CBSymbWwdh
                    Me.CBSymbAnrMon.Checked = .P_Def_CBSymbAnrMon
                    Me.CBSymbAnrListe.Checked = .P_Def_CBSymbAnrListe
                    Me.CBSymbDirekt.Checked = .P_Def_CBSymbDirekt
                    Me.CBSymbRWSuche.Checked = .P_Def_CBSymbRWSuche
                    Me.CBSymbJournalimport.Checked = .P_Def_CBSymbJournalimport
#End If
                    ' Einstellungen für die Rückwärtssuche zurücksetzen
                    Me.CBRWS.Checked = .P_Def_CBRWS
                    Me.ComboBoxRWS.Enabled = .P_Def_CBRWS
                    Me.ComboBoxRWS.SelectedIndex = .P_Def_ComboBoxRWS
                    Me.CBRWSIndex.Checked = .P_Def_CBRWSIndex
                    ' Einstellungen für das Journal zurücksetzen
                    Me.CBKHO.Checked = .P_Def_CBKHO
                    Me.CBJournal.Checked = .P_Def_CBJournal
                    Me.CBJImport.Checked = .P_Def_CBJImport
                    Me.CBLogFile.Checked = .P_Def_CBLogFile

                    'StoppUhr
                    Me.CBStoppUhrEinblenden.Checked = .P_Def_CBStoppUhrEinblenden
                    Me.CBStoppUhrAusblenden.Checked = .P_Def_CBStoppUhrAusblenden
                    Me.TBStoppUhr.Text = CStr(.P_Def_TBStoppUhr)

                    'Telefonnummernformat
                    Me.TBTelNrMaske.Text = .P_Def_TBTelNrMaske
                    Me.CBTelNrGruppieren.Checked = .P_Def_CBTelNrGruppieren
                    Me.CBintl.Checked = .P_Def_CBintl
                    Me.CBIgnoTelNrFormat.Checked = .P_Def_CBIgnoTelNrFormat
                    'Notiz
                    Me.CBNote.Checked = C_DP.P_Def_CBNote
                End With
                C_hf.LogFile("Einstellungen zurückgesetzt")
            Case "BTelefonliste"
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
            Case "BOK"
                Dim formschließen As Boolean = Speichern()
                C_DP.P_CBUseAnrMon = Me.CBUseAnrMon.Checked
#If OVer >= 14 Then
                C_GUI.RefreshRibbon()
#End If
                If formschließen Then Me.Hide()
            Case "BAbbruch"
                ' Schließt das Fenster
                Me.Hide()
            Case "BApply"
                Speichern()
            Case "BXML"
                System.Diagnostics.Process.Start(C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Config_FileName)
            Case "BAnrMonTest"
                Speichern()
                Using F_AnrMon As New Popup
                    F_AnrMon.Start(False, C_DP, C_hf, C_AnrMon, C_OlI, C_KF)
                End Using
            Case "BZwischenablage"
                My.Computer.Clipboard.SetText(Me.TBDiagnose.Text)
            Case "BProbleme"
                Dim T As New Thread(AddressOf NeueMail)

                If C_hf.FBDB_MsgBox("Der Einstellungsdialog wird jetzt geschlossen. Danach werden alle erforderlichen Informationen gesammelt, was ein paar Sekunden dauern kann." & vbNewLine & _
                                                "Danach wird eine neue E-Mail geöffnet, die Sie bitte vervollständigen und absenden.", MsgBoxStyle.OkCancel, "") = MsgBoxResult.Ok Then
                    T.Start()
                    Me.Close()
                Else
                    T = Nothing
                End If
            Case "BStartDebug"
                Me.TBDiagnose.Text = C_DP.P_Def_StringEmpty
                AddLine("Start")
                If Me.CBTelefonDatei.Checked Then
                    If System.IO.File.Exists(Me.TBTelefonDatei.Text) Then
                        If C_hf.FBDB_MsgBox("Sind Sie sicher was sie da tun? Das Einlesen einer fehlerhaften oder falschen Datei wird sehr unerfreulich enden.", _
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
                    .RunWorkerAsync(C_DP.P_Debug_ImportTelefone)
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

                C_DP.P_StatResetZeit = System.DateTime.Now
                C_DP.P_StatVerpasst = 0
                C_DP.P_StatNichtErfolgreich = 0
                C_DP.P_StatKontakt = 0
                C_DP.P_StatJournal = 0
                C_DP.P_StatOLClosedZeit = System.DateTime.Now

                Dim xPathTeile As New ArrayList
                With xPathTeile
                    .Clear()
                    .Add("Telefone")
                    .Add("Telefone")
                    .Add("*")
                    .Add("Telefon")
                    .Add("Eingehend")
                    C_DP.Delete(xPathTeile)
                    .Item(.Count - 1) = "Ausgehend"
                    C_DP.Delete(xPathTeile)
                End With
                C_DP.SpeichereXMLDatei()
                Ausfüllen()
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
                Dim StartPosition As System.Drawing.Point
                Dim x As Integer = 0
                Dim y As Integer = 0

                If C_DP.P_CBStoppUhrAusblenden Then
                    WarteZeit = CInt(Me.TBStoppUhr.Text)
                Else
                    WarteZeit = -1
                End If

                StartPosition = New System.Drawing.Point(C_DP.P_CBStoppUhrX, C_DP.P_CBStoppUhrY)
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

                Dim frmStUhr As New Popup
                frmStUhr.ZeigeStoppUhr("Gegenstelle", Zeit, "Richtung:", WarteZeit, StartPosition, "Ihre MSN")

                Do Until frmStUhr.StUhrClosed
                    C_hf.ThreadSleep(20)
                    Windows.Forms.Application.DoEvents()
                Loop

                C_DP.P_CBStoppUhrX = frmStUhr.Position.X
                C_DP.P_CBStoppUhrY = frmStUhr.Position.Y
                frmStUhr = Nothing
            Case "BArbeitsverzeichnis"
                Dim fDialg As New System.Windows.Forms.FolderBrowserDialog
                With fDialg
                    .ShowNewFolderButton = True
                    .SelectedPath = C_DP.P_Arbeitsverzeichnis
                    .Description = "Wählen Sie das neue Arbeitsverzeichnis aus!"
                    If .ShowDialog = Windows.Forms.DialogResult.OK Then
                        If Not C_DP.P_Arbeitsverzeichnis = .SelectedPath Then
                            C_hf.LogFile("Arbeitsverzeichnis von " & C_DP.P_Arbeitsverzeichnis & " auf " & .SelectedPath & "\ geändert.")
                            C_DP.P_Arbeitsverzeichnis = .SelectedPath & "\"
                            Me.ToolTipFBDBConfig.SetToolTip(Me.BXML, "Öffnet die Datei " & vbCrLf & C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Config_FileName)
                            C_DP.SpeichereXMLDatei()
                        End If
                    End If
                End With
            Case "BRWSTest"
                Dim TelNr As String = Me.TBRWSTest.Text
                If IsNumeric(TelNr) Then
                    Dim F_RWS As New formRWSuche(C_hf, C_KF, C_DP)
                    Dim rws As Boolean
                    Dim vCard As String = C_DP.P_Def_StringEmpty

                    Select Case CType(Me.ComboBoxRWS.SelectedIndex, RückwärtsSuchmaschine)
                        Case RückwärtsSuchmaschine.RWSDasOertliche
                            rws = F_RWS.RWSDasOertiche(TelNr, vCard)
                        Case RückwärtsSuchmaschine.RWS11880
                            rws = F_RWS.RWS11880(TelNr, vCard)
                        Case RückwärtsSuchmaschine.RWSDasTelefonbuch
                            rws = F_RWS.RWSDasTelefonbuch(TelNr, vCard)
                        Case RückwärtsSuchmaschine.RWStelSearch
                            rws = F_RWS.RWStelsearch(TelNr, vCard)
                        Case RückwärtsSuchmaschine.RWSAlle
                            rws = F_RWS.RWSAlle(TelNr, vCard)
                    End Select

                    C_hf.FBDB_MsgBox("Die Rückwärtssuche mit der Nummer """ & TelNr & """ brachte mit der Suchmaschine """ & Me.ComboBoxRWS.SelectedItem.ToString() & """ " & _
                                    CStr(IIf(rws, "folgendes Ergebnis:" & C_DP.P_Def_NeueZeile & C_DP.P_Def_NeueZeile & vCard, "kein Ergebnis.")), MsgBoxStyle.Information, _
                                    "Test der Rückwärtssuche " & Me.ComboBoxRWS.SelectedItem.ToString())
                Else
                    C_hf.FBDB_MsgBox("Die Telefonnummer """ & TelNr & """ ist ungültig (Test abgebrochen).", MsgBoxStyle.Exclamation, "Test der Rückwärtssuche")
                End If
            Case "BTestLogin"
                If C_FBox.FBLogin(True, Me.TBBenutzer.Text, Me.TBPasswort.Text) = C_DP.P_Def_SessionID Then
                    Me.BTestLogin.Text = "Fehler!"
                Else
                    Me.BTestLogin.Text = "OK!"
                End If
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
                System.Diagnostics.Process.Start(C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Log_FileName)
        End Select
    End Sub

#End Region

#Region "Änderungen"
    Private Sub ValueChanged(sender As Object, e As EventArgs) Handles _
                                                                        CBRWS.CheckedChanged, _
                                                                        CBCbCunterbinden.CheckedChanged, _
                                                                        CBAutoClose.CheckedChanged, _
                                                                        CBTelefonDatei.CheckedChanged, _
                                                                        CBJournal.CheckedChanged, _
                                                                        CBIndexAus.CheckedChanged, _
                                                                        CBUseAnrMon.CheckedChanged, _
                                                                        CBAnrMonMove.CheckedChanged, _
                                                                        CBStoppUhrEinblenden.CheckedChanged, _
                                                                        CBStoppUhrAusblenden.CheckedChanged, _
                                                                        CBLogFile.CheckedChanged, _
                                                                        TBEnblDauer.TextChanged, _
                                                                        TBAnrMonX.TextChanged, _
                                                                        TBAnrMonY.TextChanged, _
                                                                        TBTelNrMaske.Leave, _
                                                                        CLBTelNr.SelectedIndexChanged, _
                                                                        TBRWSTest.TextChanged, _
                                                                        TBBenutzer.TextChanged, _
                                                                        TBPasswort.TextChanged

        Select Case sender.GetType().Name
            Case "CheckBox"
                Select Case CType(sender, CheckBox).Name
                    Case "CBTelefonDatei"
                        Me.PTelefonDatei.Enabled = Me.CBTelefonDatei.Checked
                        If Not Me.CBTelefonDatei.Checked Then
                            Me.TBTelefonDatei.Text = C_DP.P_Def_StringEmpty
                        End If
                    Case "CBRWS"
                        ' Combobox für Rückwärtssuchmaschinen je nach CheckBox für Rückwärtssuche ein- bzw. ausblenden
                        Me.ComboBoxRWS.Enabled = Me.CBRWS.Checked
                        Me.CBKErstellen.Checked = Me.CBRWS.Checked
                        Me.CBKErstellen.Enabled = Me.CBRWS.Checked
                        Me.CBRWSIndex.Enabled = Me.CBRWS.Checked
                        Me.CBRWSIndex.Checked = Me.CBRWS.Checked
                        Me.LRWSTest.Enabled = Me.CBRWS.Checked
                        Me.TBRWSTest.Enabled = Me.CBRWS.Checked
                        'Me.BRWSTest.Enabled = Me.CBRWS.Checked
                    Case "CBCbCunterbinden"
                        Me.CBCallByCall.Enabled = Not Me.CBCbCunterbinden.Checked
                        If Me.CBCbCunterbinden.Checked Then Me.CBCallByCall.Checked = False
                    Case "CBAutoClose"
                        Me.TBEnblDauer.Enabled = Me.CBAutoClose.Checked
                        Me.LEnblDauer.Enabled = Me.CBAutoClose.Checked
                    Case "CBJournal"
                        If Not Me.CBJournal.Checked Then Me.CBJImport.Checked = False
                        Me.CBJImport.Enabled = Me.CBJournal.Checked
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
                    Case "CBAnrMonMove"
                        Me.CBoxAnrMonMoveDirection.Enabled = Me.CBAnrMonMove.Checked
                        Me.LAnrMonMoveDirection.Enabled = Me.CBAnrMonMove.Checked
                End Select
            Case "TextBox"
                Select Case CType(sender, TextBox).Name
                    Case "TBLandesVW"
                        If Me.TBLandesVW.Text = C_DP.P_Def_TBLandesVW Then
                            Me.CBRWS.Enabled = True
                            Me.CBKErstellen.Enabled = True
                            Me.ComboBoxRWS.Enabled = Me.CBRWS.Checked
                        Else
                            Me.CBRWS.Checked = False
                            Me.CBRWS.Enabled = False

                            Me.CBKErstellen.Enabled = False
                            Me.CBKErstellen.Checked = False
                            Me.ComboBoxRWS.Enabled = False
                        End If
                    Case "TBVorwahl"
                        Me.TBVorwahl.Text = C_hf.AcceptOnlyNumeric(Me.TBVorwahl.Text)
                    Case "TBEnblDauer"
                        Me.TBEnblDauer.Text = C_hf.AcceptOnlyNumeric(Me.TBEnblDauer.Text)
                    Case "TBAnrMonX"
                        Me.TBAnrMonX.Text = C_hf.AcceptOnlyNumeric(Me.TBAnrMonX.Text)
                    Case "TBAnrMonY"
                        Me.TBAnrMonY.Text = C_hf.AcceptOnlyNumeric(Me.TBAnrMonY.Text)
                    Case "TBLandesVW"
                        Me.ToolTipFBDBConfig.SetToolTip(Me.CBVoIPBuster, "Mit dieser Einstellung wird die Landesvorwahl " & Me.TBLandesVW.Text & " immer mitgewählt.")
                    Case "TBTelNrMaske"
                        PrüfeMaske()
                    Case "TBRWSTest"
                        Me.TBRWSTest.Text = C_hf.AcceptOnlyNumeric(Me.TBRWSTest.Text)
                        Me.BRWSTest.Enabled = Len(C_hf.nurZiffern(Me.TBRWSTest.Text)) > 0
                    Case "TBBenutzer", "TBPasswort"
                        Me.BTestLogin.Text = "Test"
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
            Case "ComboBox"
                'Select Case CType(sender, ComboBox).Name
                '    Case ""

                'End Select
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
        If C_hf.IsOneOf("0", pos) Then
            C_hf.FBDB_MsgBox("Achtung: Die Maske für die Telefonnummernformatierung ist nicht korrekt." & vbNewLine & _
                        "Prüfen Sie, ob folgende Zeichen in der Maske Enthalten sind: ""%L"", ""%V"" und ""%N"" (""%D"" kann wegelassen werden)!" & vbNewLine & _
                        "Beispiel: ""%L (%O) %N - %D""", MsgBoxStyle.Information, "Einstellungen")
            Return False
        End If
        Return True
    End Function

    Private Sub NeueMail()
        Dim NeueFW As Boolean
        Dim sSID As String = C_DP.P_Def_SessionID
        Dim URL As String
        Dim FBEncoding As System.Text.Encoding = System.Text.Encoding.UTF8
        Dim MailText As String
        Dim PfadTMPfile As String
        Dim tmpFileName As String
        Dim tmpFilePath As String
        Dim FBBenutzer As String
        Dim FBPasswort As String

        C_FBox.SetEventProvider(emc)

        Do While sSID = C_DP.P_Def_SessionID
            FBBenutzer = InputBox("Geben Sie den Benutzernamen der Fritz!Box ein (Lassen Sie das Feld leer, falls Sie kein Benutzername benötigen.):")
            FBPasswort = InputBox("Geben Sie das Passwort der Fritz!Box ein:")
            If Len(FBPasswort) = 0 Then
                If C_hf.FBDB_MsgBox("Abbrechen?", MsgBoxStyle.YesNo, "NewMail") = vbYes Then
                    Exit Sub
                End If
            End If
            sSID = C_FBox.FBLogin(NeueFW, FBBenutzer, FBPasswort)
        Loop

        If NeueFW Then
            URL = "http://" & C_DP.P_ValidFBAdr & "/fon_num/fon_num_list.lua?sid=" & sSID
        Else
            URL = "http://" & C_DP.P_ValidFBAdr & "/cgi-bin/webcm?sid=" & sSID & "&getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices"
        End If

        MailText = C_hf.httpGET(URL, FBEncoding, Nothing)

        With My.Computer.FileSystem
            PfadTMPfile = .GetTempFileName()
            tmpFilePath = .GetFileInfo(PfadTMPfile).DirectoryName
            tmpFileName = Split(.GetFileInfo(PfadTMPfile).Name, ".", , CompareMethod.Text)(0) & "_Telefoniegeräte.htm"
            .RenameFile(PfadTMPfile, tmpFileName)
            PfadTMPfile = .GetFiles(tmpFilePath, FileIO.SearchOption.SearchTopLevelOnly, "*_Telefoniegeräte.htm")(0).ToString
            .WriteAllText(PfadTMPfile, MailText, False)
        End With
        C_OlI.NeueEmail(PfadTMPfile, C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Config_FileName, C_FBox.GetInformationSystemFritzBox())
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

        Anzahl = 0
        olNamespace = C_OlI.OutlookApplication.GetNamespace("MAPI")

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

        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                ZähleKontakte(NamensRaum.Folders.Item(j), Nothing)
                j = j + 1
            Loop
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
                        KontaktName = " (" & aktKontakt.FullName & ")"
                        C_KF.IndiziereKontakt(aktKontakt)
                        aktKontakt.Save()
                        BWIndexer.ReportProgress(1)
                        If BWIndexer.CancellationPending Then Exit For
                    Else
                        BWIndexer.ReportProgress(1)
                    End If
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
                        C_KF.DeIndizierungKontakt(aktKontakt)
                        BWIndexer.ReportProgress(-1)
                        If BWIndexer.CancellationPending Then Exit For
                    Else
                        BWIndexer.ReportProgress(-1)
                    End If
                    C_hf.NAR(item)
                    Windows.Forms.Application.DoEvents()
                Next 'Item
                C_KF.DeIndizierungOrdner(Ordner)
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
        Dim LogDatei As String = C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Log_FileName

        If C_DP.P_CBLogFile Then
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
            If .SelectedText = C_DP.P_Def_StringEmpty Then
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

    Private Sub DelSetTreeView()
        If Me.InvokeRequired Then
            Dim D As New DelgButtonTelEinl(AddressOf DelSetTreeView)
            Me.Invoke(D)
        Else
            Dim tmpNode As TreeNode()

            C_OlI.GetKontaktOrdnerInTreeView(Me.TVOutlookContact)
            With Me.TVOutlookContact
                .ExpandAll()
                tmpNode = .Nodes.Find(C_DP.P_TVKontaktOrdnerEntryID & ";" & C_DP.P_TVKontaktOrdnerStoreID, True)
                If Not tmpNode.Length = 0 Then
                    .SelectedNode = tmpNode(0)
                End If
                .Enabled = True
            End With

        End If
    End Sub

#End Region

#Region "BackGroundWorker - Handle"
    Private Sub BWIndexer_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWIndexer.DoWork

        ErmittleKontaktanzahl()
        If Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
            StatusWert = Me.ProgressBarIndex.Maximum.ToString
            BWIndexer.ReportProgress(Me.ProgressBarIndex.Maximum)
        End If

        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder

        olNamespace = C_OlI.OutlookApplication.GetNamespace("MAPI")

        If Me.CBKHO.Checked Then
            olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            If Me.RadioButtonErstelle.Checked Then
                KontaktIndexer(C_DP.P_TBLandesVW, Ordner:=olfolder)
            ElseIf Me.RadioButtonEntfernen.Checked Then
                KontaktDeIndexer(olfolder, Nothing)
            End If
        Else
            If Me.RadioButtonErstelle.Checked Then
                KontaktIndexer(C_DP.P_TBLandesVW, NamensRaum:=olNamespace)
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
            C_DP.P_LLetzteIndizierung = Date.Now
            C_hf.LogFile("Indizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        ElseIf Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
            C_hf.LogFile("Deindizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        End If
    End Sub

    Private Sub BWTelefone_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWTelefone.DoWork
        AddLine("Einlesen der Telefone gestartet.")
        C_FBox.P_SpeichereDaten = CBool(e.Argument)
        e.Result = CBool(e.Argument)
        If Me.TBTelefonDatei.Text = C_DP.P_Def_StringEmpty Then
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
                tmpTelefon = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne_String)
                If Not tmpTelefon = C_DP.P_Def_ErrorMinusOne_String Then
                    .Item(.Count - 1) = "TelNr"
                    If Not ((TelList.Rows(Row).Cells(4).Value Is Nothing) Or (TelList.Rows(Row).Cells(5).Value Is Nothing)) Then
                        If tmpTelefon = TelList.Rows(Row).Cells(4).Value.ToString And _
                            C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne_String) = Replace(TelList.Rows(Row).Cells(5).Value.ToString, ", ", ";", , , CompareMethod.Text) Then
                            Dim Dauer As Date
                            .Item(.Count - 1) = "Eingehend"
                            Dauer = CDate(TelList.Rows(Row).Cells(6).Value.ToString())
                            C_DP.Write(xPathTeile, CStr((Dauer.Hour * 60 + Dauer.Minute) * 60 + Dauer.Second))
                            .Item(.Count - 1) = "Ausgehend"
                            Dauer = CDate(TelList.Rows(Row).Cells(7).Value.ToString())
                            C_DP.Write(xPathTeile, CStr((Dauer.Hour * 60 + Dauer.Minute) * 60 + Dauer.Second))
                        End If
                    End If
                End If
            Next

            'CLBTelNrAusfüllen setzen
            .Clear()
            Dim CheckTelNr As CheckedListBox.CheckedItemCollection = Me.CLBTelNr.CheckedItems
            If Not CheckTelNr.Count = 0 Then
                Dim tmpTeile As String = C_DP.P_Def_StringEmpty
                .Add("Telefone")
                .Add("Nummern")
                .Add("*")
                For i = 0 To CheckTelNr.Count - 1
                    tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
                Next
                tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
                .Add("[" & tmpTeile & "]")
                C_DP.WriteAttribute(xPathTeile, "Checked", "1")
            End If
        End With

        SetTelNrListe()
        SetFillTelListe()
        DelBTelefonliste()
        BWTelefone = Nothing
        AddLine("BackgroundWorker wurde eliminiert.")
        If CBool(e.Result) Then AddLine("Das Einlesen der Telefone ist abgeschlossen.")
    End Sub

    Private Sub BWTreeView_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWTreeView.DoWork
        DelSetTreeView()
    End Sub

    Private Sub BWTreeView_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWTreeView.RunWorkerCompleted
        BWTreeView = Nothing
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

    Private Sub ButtonPhoner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BPhoner.Click
        Dim PhonerInstalliert As Boolean = C_Phoner.PhonerReady()
        Me.PanelPhonerAktiv.BackColor = CType(IIf(PhonerInstalliert, Color.LightGreen, Color.Red), Color)
        Me.LabelPhoner.Text = "Phoner ist " & CStr(IIf(PhonerInstalliert, "", "nicht ")) & "aktiv."
        Me.PanelPhoner.Enabled = PhonerInstalliert
        C_DP.P_PhonerVerfügbar = PhonerInstalliert
    End Sub

    Private Sub CBPhoner_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBPhoner.CheckedChanged
        Me.TBPhonerPasswort.Enabled = Me.CBPhoner.Checked
        Me.LPassworPhoner.Enabled = Me.CBPhoner.Checked
    End Sub
#End Region


End Class


