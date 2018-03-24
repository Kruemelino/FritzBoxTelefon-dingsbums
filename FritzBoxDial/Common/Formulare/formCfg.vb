Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Public Class formCfg
#Region "Eigene Klassen"
    Private C_XML As XML
    Private C_DP As DataProvider
    Private C_Crypt As Rijndael
    Private C_hf As Helfer
    Private C_KF As KontaktFunktionen
    Private C_Phoner As PhonerInterface
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_AnrMon As AnrufMonitor
    Private C_FBox As FritzBox
    Private C_PopUp As Popup
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

    Friend Sub New(ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal DataProviderKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal CryptKlasse As Rijndael, _
                   ByVal AnrufMon As AnrufMonitor, _
                   ByVal fritzboxKlasse As FritzBox, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal kontaktklasse As KontaktFunktionen, _
                   ByVal Phonerklasse As PhonerInterface, _
                   ByVal Popupklasse As Popup, _
                   ByVal XMLKlasse As XML)

        InitializeComponent()

        C_hf = HelferKlasse
        C_DP = DataProviderKlasse
        C_Crypt = CryptKlasse
        C_GUI = InterfacesKlasse
        C_OlI = OutlInter
        C_AnrMon = AnrufMon
        C_FBox = fritzboxKlasse
        C_KF = kontaktklasse
        C_Phoner = Phonerklasse
        C_PopUp = Popupklasse
        C_XML = XMLKlasse

    End Sub

    Private Sub UserForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        TBAnrMonMoveGeschwindigkeit.BackColor = C_hf.IIf(OutlookSecurity.IsThemeActive, SystemColors.ControlLightLight, SystemColors.ControlLight)
        BAnrMonTest.Enabled = C_AnrMon IsNot Nothing
        BTelefonliste.Enabled = C_FBox IsNot Nothing
        FBDB_MP.SelectedIndex = 0
        Ausfüllen()
    End Sub

#Region "Ausfüllen"
    Private Sub Ausfüllen()
        With C_DP
            LVersion.Text = DataProvider.P_Def_Addin_LangName & " " & ThisAddIn.Version
            With ComboBoxRWS.Items
                .Clear()
                .Add(DataProvider.P_RWSDasOertliche_Name) '"DasÖrtliche"
                '.Add(DataProvider.P_RWS11880_Name) '"11880.com"
                '.Add(DataProvider.P_RWSDasTelefonbuch_Name) '"DasTelefonbuch.de"
                .Add(DataProvider.P_RWSTelSearch_Name) '"tel.search.ch"
                .Add(DataProvider.P_RWSAlle_Name) '"Alle Suchmaschinen"
            End With

            ToolTipFBDBConfig.SetToolTip(BXML, "Öffnet die Datei " & vbCrLf & .P_Arbeitsverzeichnis & DataProvider.P_Def_Config_FileName)

            ' Einstellungen für das Wählmakro laden
            TBLandesVW.Text = .P_TBLandesVW

            TBAmt.Text = C_hf.IIf(.P_TBAmt = DataProvider.P_Def_ErrorMinusOne_String, "", .P_TBAmt)
            TBFBAdr.Text = .P_TBFBAdr

            CBForceFBAddr.Checked = .P_CBForceFBAddr
            TBBenutzer.Text = .P_TBBenutzer

            If Not Len(.P_TBPasswort) = 0 Then TBPasswort.Text = "1234"
            TBVorwahl.Text = .P_TBVorwahl
            TBNumEntryList.Text = CStr(.P_TBNumEntryList)
            TBEnblDauer.Text = CStr(.P_TBEnblDauer)
            CBAnrMonAuto.Checked = .P_CBAnrMonAuto
            TBAnrBeantworterTimeout.Text = CStr(.P_TBAnrBeantworterTimeout)
            TBAnrMonX.Text = CStr(.P_TBAnrMonX)
            TBAnrMonY.Text = CStr(.P_TBAnrMonY)
            CBAnrMonMove.Checked = .P_CBAnrMonMove
            CBAnrMonTransp.Checked = .P_CBAnrMonTransp

            If .P_TBAnrMonMoveGeschwindigkeit < TBAnrMonMoveGeschwindigkeit.Minimum Or .P_TBAnrMonMoveGeschwindigkeit > TBAnrMonMoveGeschwindigkeit.Maximum Then
                .P_TBAnrMonMoveGeschwindigkeit = DataProvider.P_Def_TBAnrMonMoveGeschwindigkeit
            End If

            TBAnrMonMoveGeschwindigkeit.Value = .P_TBAnrMonMoveGeschwindigkeit
            CBoxAnrMonStartPosition.SelectedIndex = .P_CBoxAnrMonStartPosition
            CBoxAnrMonMoveDirection.SelectedIndex = .P_CBoxAnrMonMoveDirection
            CBAnrMonZeigeKontakt.Checked = .P_CBAnrMonZeigeKontakt
            CBAnrMonContactImage.Checked = .P_CBAnrMonContactImage
            CBIndexAus.Checked = .P_CBIndexAus
            CBShowMSN.Checked = .P_CBShowMSN
            ' optionale allgemeine Einstellungen laden
            CBAutoClose.Checked = .P_CBAutoClose
            CBAnrMonCloseAtDISSCONNECT.Checked = .P_CBAnrMonCloseAtDISSCONNECT
            CBVoIPBuster.Checked = .P_CBVoIPBuster
            CBCbCunterbinden.Checked = .P_CBCbCunterbinden
            CBCallByCall.Checked = .P_CBCallByCall
            CBDialPort.Checked = .P_CBDialPort
            CBRWS.Checked = .P_CBRWS
            CBKErstellen.Checked = .P_CBKErstellen
            CBLogFile.Checked = .P_CBLogFile
            CBAutoAnrList.Checked = .P_CBAutoAnrList
            ' Einstellungen füer die Rückwärtssuche laden
            CBKHO.Checked = .P_CBKHO
            CBRWSIndex.Checked = .P_CBRWSIndex

            ComboBoxRWS.SelectedItem = ComboBoxRWS.Items.Item(.P_ComboBoxRWS)
            If Not CBRWS.Checked Then ComboBoxRWS.Enabled = False
            ' Einstellungen für das Journal laden
            CBJournal.Checked = .P_CBJournal
            CBAnrListeUpdateJournal.Checked = .P_CBAnrListeUpdateJournal
            CBAnrListeUpdateCallLists.Checked = .P_CBAnrListeUpdateCallLists
            CBAnrListeShowAnrMon.Checked = .P_CBAnrListeShowAnrMon
            CBUseAnrMon.Checked = .P_CBUseAnrMon
            CBCheckMobil.Checked = .P_CBCheckMobil
            CBIndexAus.Enabled = Not CBUseAnrMon.Checked
            PanelAnrMon.Enabled = CBUseAnrMon.Checked
            If Not CBAutoAnrList.Checked Then
                CBAnrListeUpdateJournal.Checked = False
                CBAnrListeUpdateCallLists.Checked = False
                CBAnrListeShowAnrMon.Checked = False
                CBAnrListeUpdateJournal.Enabled = False
                CBAnrListeUpdateCallLists.Enabled = False
                CBAnrListeShowAnrMon.Enabled = False
            End If
            'StoppUhr
            CBStoppUhrEinblenden.Checked = .P_CBStoppUhrEinblenden
            CBStoppUhrAusblenden.Checked = .P_CBStoppUhrAusblenden
            TBStoppUhr.Text = CStr(.P_TBStoppUhr)
            CBStoppUhrIgnIntFax.Checked = .P_CBStoppUhrIgnIntFax
            If Not CBStoppUhrEinblenden.Checked Then CBStoppUhrAusblenden.Checked = False
            TBStoppUhr.Enabled = CBStoppUhrAusblenden.Checked And CBStoppUhrEinblenden.Checked
            LabelStoppUhr.Enabled = CBStoppUhrEinblenden.Checked
            CBStoppUhrAusblenden.Enabled = CBStoppUhrEinblenden.Checked
            CBStoppUhrIgnIntFax.Enabled = CBStoppUhrEinblenden.Checked
            'Telefonnummernformat
            TBTelNrMaske.Text = .P_TBTelNrMaske
            CBTelNrGruppieren.Checked = .P_CBTelNrGruppieren
            CBintl.Checked = .P_CBintl
            CBIgnoTelNrFormat.Checked = .P_CBIgnoTelNrFormat

            'Phoner
            Dim PhonerVerfuegbar As Boolean = .P_PhonerVerfügbar
            PanelPhoner.Enabled = PhonerVerfuegbar
            If PhonerVerfuegbar Then
                CBPhoner.Checked = .P_CBPhoner
            Else
                CBPhoner.Checked = False
            End If
            LabelPhoner.Text = Replace(LabelPhoner.Text, " [nicht]", C_hf.IIf(PhonerVerfuegbar, "", " nicht"), , , CompareMethod.Text)

            Dim xPathTeile As New ArrayList

            'Statistik zurückschreiben
            With xPathTeile
                .Add("Telefone")
                .Add("Telefone")
                .Add("*")
                .Add("Telefon")
                .Add("[@Dialport > 19 and @Dialport < 30]") ' Nur IP-Telefone
                .Add("TelName")
            End With

            ComboBoxPhonerSIP.DataSource = Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, "Phoner"), ";", , CompareMethod.Text)

            If Not ComboBoxPhonerSIP.Items.Count = 0 Then ComboBoxPhonerSIP.SelectedIndex = .P_ComboBoxPhonerSIP

            CBPhonerAnrMon.Checked = .P_CBPhonerAnrMon
            If Not Len(.P_TBPhonerPasswort) = 0 Then TBPhonerPasswort.Text = "1234"

            Dim PhonerInstalliert As Boolean = C_Phoner.PhonerReady()
            PanelPhonerAktiv.BackColor = C_hf.IIf(PhonerInstalliert, Color.LightGreen, Color.Red)
            LabelPhoner.Text = "Phoner ist " & C_hf.IIf(PhonerInstalliert, "", "nicht ") & "aktiv."
            PanelPhoner.Enabled = PhonerInstalliert
            .P_PhonerVerfügbar = PhonerInstalliert
            ' Notiz
            CBNote.Checked = .P_CBNote
            ' Fritz!Box Kommunikation
            RBFBComUPnP.Checked = .P_RBFBComUPnP
        End With
        'TreeView
        With TVOutlookContact
            .Enabled = False
            If .Nodes.Count > 0 Then .Nodes.Clear()
        End With

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
        Nebenstellen = Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String & ";"), ";", , CompareMethod.Text)

        If Not Nebenstellen(0) = DataProvider.P_Def_ErrorMinusOne_String Then
            With TelList
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

                        Zeile.Add(CBool(C_XML.Read(C_DP.XMLDoc, xPathTeile, "False")))
                        Zeile.Add(CStr(j))
                        .Item(.Count - 1) = "@Dialport"
                        Zeile.Add(C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String & ";")) 'Nebenstelle
                        .RemoveAt(.Count - 1)
                        Zeile.Add(C_XML.ReadElementName(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String & ";")) 'Telefontyp
                        Zeile.Add(Nebenstelle) ' TelName
                        .Add("TelNr")
                        Zeile.Add(Replace(C_XML.Read(C_DP.XMLDoc, xPathTeile, "-"), ";", ", ", , , CompareMethod.Text)) 'TelNr
                        .Item(.Count - 1) = "Eingehend"
                        Zeile.Add(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0")) 'Eingehnd
                        tmpein(0) += CDbl(Zeile.Item(Zeile.Count - 1))
                        .Item(.Count - 1) = "Ausgehend"
                        Zeile.Add(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0")) 'Ausgehnd
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
                Zeile.Add(DataProvider.P_Def_LeerString)
                Zeile.Add(DataProvider.P_Def_LeerString)
                Zeile.Add(DataProvider.P_Def_LeerString)
                Zeile.Add(DataProvider.P_Def_LeerString)
                Zeile.Add("Gesamt:")
                For i = 0 To 2
                    Zeile.Add(C_hf.GetTimeInterval(tmpein(i)))
                Next

                .Rows.Add(Zeile.ToArray)
            End With
        End If

        TBAnderes.Text = C_DP.P_StatVerpasst & " verpasste Telefonate" & vbCrLf
        TBAnderes.Text = TBAnderes.Text & C_DP.P_StatNichtErfolgreich & " nicht erfolgreiche Telefonate" & vbCrLf
        TBAnderes.Text = TBAnderes.Text & C_DP.P_StatKontakt & " erstellte Kontakte" & vbCrLf
        TBAnderes.Text = TBAnderes.Text & C_DP.P_StatJournal & " erstellte Journaleinträge" & vbCrLf
        TBReset.Text = "Letzter Reset: " & C_DP.P_StatResetZeit
        TBSchließZeit.Text = "Letzter Journaleintrag: " & C_DP.P_StatOLClosedZeit
        xPathTeile = Nothing
        Zeile = Nothing
    End Sub

    Private Sub CLBTelNrAusfüllen()
        Dim xPathTeile As New ArrayList
        Dim TelNrString() As String
        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*[starts-with(name(.), ""POTS"") or starts-with(name(.), ""MSN"") or starts-with(name(.), ""SIP"") or starts-with(name(.), ""Mobil"")]")

            TelNrString = Split("Alle Telefonnummern;" & C_XML.Read(C_DP.XMLDoc, xPathTeile, ""), ";", , CompareMethod.Text)

            TelNrString = C_hf.ClearStringArray(TelNrString, True, True, True)

            CLBTelNr.Items.Clear()

            For Each TelNr In TelNrString
                CLBTelNr.Items.Add(TelNr)
            Next
            'etwas unschön
            .Add("")
            For i = 1 To CLBTelNr.Items.Count - 1
                .Item(.Count - 2) = "*[. = """ & CLBTelNr.Items(i).ToString & """]"
                .Item(.Count - 1) = "@Checked"
                CLBTelNr.SetItemChecked(i, Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0;") & ";", ";", , CompareMethod.Text).Contains("1"))
                'Me.CLBTelNr.SetItemChecked(i, C_hf.IsOneOf("1", Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0;") & ";", ";", , CompareMethod.Text)))
            Next
        End With
        CLBTelNr.SetItemChecked(0, CLBTelNr.CheckedItems.Count = CLBTelNr.Items.Count - 1)
    End Sub
#End Region

    Private Function Speichern() As Boolean
        Speichern = True
        Dim xPathTeile As New ArrayList
        Dim tmpTeile As String = DataProvider.P_Def_LeerString
        Dim CheckTelNr As CheckedListBox.CheckedItemCollection = CLBTelNr.CheckedItems

        ' dieses Try-Catch ist erforderlich, da es beim Debuggen ab und zu unerklärlichen Zugriffs-Fehlern kommt.
        Try
            If CheckTelNr.Count = 0 Then
                For i = 0 To CLBTelNr.Items.Count - 1
                    CLBTelNr.SetItemChecked(i, True)
                Next
                CheckTelNr = CLBTelNr.CheckedItems
            End If
            If CLBTelNr.Items.Count > 1 Then
                With xPathTeile
                    .Add("Telefone")
                    .Add("Nummern")
                    .Add("*")
                    For i = 1 To CLBTelNr.Items.Count - 1
                        tmpTeile += ". = " & """" & CLBTelNr.Items(i).ToString & """" & " or "
                    Next
                    tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
                    .Add("[" & tmpTeile & "]")
                    C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "Checked", "0")
                    tmpTeile = DataProvider.P_Def_LeerString
                    For i = 0 To CheckTelNr.Count - 1
                        tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
                    Next
                    tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
                    .Item(.Count - 1) = "[" & tmpTeile & "]"
                    C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "Checked", "1")
                End With
            End If

            ' Sichert die Einstellungen und schließt das Fenster
            If (CInt(TBEnblDauer.Text) < 4) Then TBEnblDauer.Text = "4"
            With C_DP

                .P_CBForceFBAddr = CBForceFBAddr.Checked

                If TBBenutzer.Text = DataProvider.P_Def_LeerString Then
                    With xPathTeile
                        .Clear()
                        .Add("Optionen")
                        .Add("TBBenutzer")
                    End With
                    C_XML.Delete(C_DP.XMLDoc, xPathTeile)
                Else
                    .P_TBBenutzer = TBBenutzer.Text
                End If
                If Not TBPasswort.Text = "1234" Then
                    .P_TBPasswort = C_Crypt.EncryptString128Bit(TBPasswort.Text, DataProvider.P_Def_PassWordDecryptionKey)
                    C_DP.SaveSettingsVBA("Zugang", DataProvider.P_Def_PassWordDecryptionKey)
                    C_hf.KeyChange()
                End If
                ' StoppUhr
                If Not TBStoppUhr.Text = DataProvider.P_Def_LeerString Then
                    If CInt(TBStoppUhr.Text) < 0 Then
                        TBStoppUhr.Text = CStr(DataProvider.P_Def_TBStoppUhr)
                    End If
                Else
                    TBStoppUhr.Text = CStr(DataProvider.P_Def_TBStoppUhr)
                End If

                .P_TBLandesVW = TBLandesVW.Text
                .P_TBAmt = C_hf.IIf(TBAmt.Text = DataProvider.P_Def_LeerString, DataProvider.P_Def_ErrorMinusOne_String, TBAmt.Text)
                .P_TBFBAdr = TBFBAdr.Text
                .P_TBVorwahl = TBVorwahl.Text
                .P_TBAnrMonX = CInt(TBAnrMonX.Text)
                .P_TBAnrMonY = CInt(TBAnrMonY.Text)
                .P_CBLogFile = CBLogFile.Checked
                .P_TBEnblDauer = CInt(TBEnblDauer.Text)
                .P_CBAnrMonAuto = CBAnrMonAuto.Checked
                .P_CBAutoClose = CBAutoClose.Checked
                .P_CBAnrMonCloseAtDISSCONNECT = CBAnrMonCloseAtDISSCONNECT.Checked
                .P_CBAnrMonMove = CBAnrMonMove.Checked
                .P_CBAnrMonTransp = CBAnrMonTransp.Checked
                .P_TBAnrBeantworterTimeout = CInt(TBAnrBeantworterTimeout.Text)
                .P_CBAnrMonContactImage = CBAnrMonContactImage.Checked
                .P_TBAnrMonMoveGeschwindigkeit = TBAnrMonMoveGeschwindigkeit.Value
                .P_CBoxAnrMonMoveDirection = CBoxAnrMonMoveDirection.SelectedIndex
                .P_CBoxAnrMonStartPosition = CBoxAnrMonStartPosition.SelectedIndex
                .P_CBAnrMonZeigeKontakt = CBAnrMonZeigeKontakt.Checked
                .P_CBIndexAus = CBIndexAus.Checked
                .P_CBShowMSN = CBShowMSN.Checked
                .P_CBVoIPBuster = CBVoIPBuster.Checked
                .P_CBDialPort = CBDialPort.Checked
                .P_CBCbCunterbinden = CBCbCunterbinden.Checked
                .P_CBCallByCall = CBCallByCall.Checked
                .P_CBRWS = CBRWS.Checked
                .P_CBKErstellen = CBKErstellen.Checked
                .P_ComboBoxRWS = ComboBoxRWS.SelectedIndex
                .P_CBKHO = CBKHO.Checked
                .P_CBRWSIndex = CBRWSIndex.Checked
                .P_CBJournal = CBJournal.Checked
                .P_CBAnrListeUpdateJournal = CBAnrListeUpdateJournal.Checked
                .P_CBAnrListeUpdateCallLists = CBAnrListeUpdateCallLists.Checked
                .P_CBAnrListeShowAnrMon = CBAnrListeShowAnrMon.Checked
                .P_CBUseAnrMon = CBUseAnrMon.Checked
                .P_CBAutoAnrList = CBAutoAnrList.Checked
                .P_CBCheckMobil = CBCheckMobil.Checked
                .P_CBStoppUhrEinblenden = CBStoppUhrEinblenden.Checked
                .P_CBStoppUhrAusblenden = CBStoppUhrAusblenden.Checked
                .P_TBStoppUhr = CInt(TBStoppUhr.Text)
                .P_CBStoppUhrIgnIntFax = CBStoppUhrIgnIntFax.Checked
                If PrüfeMaske() Then .P_TBTelNrMaske = TBTelNrMaske.Text
                .P_CBTelNrGruppieren = CBTelNrGruppieren.Checked
                .P_CBintl = CBintl.Checked
                .P_CBIgnoTelNrFormat = CBIgnoTelNrFormat.Checked
                .P_CBPhoner = CBPhoner.Checked
                If ComboBoxPhonerSIP.Items.Count > 0 Then
                    .P_ComboBoxPhonerSIP = ComboBoxPhonerSIP.SelectedIndex
                End If

                .P_CBPhonerAnrMon = CBPhonerAnrMon.Checked
                ' Notiz
                .P_CBNote = CBNote.Checked
                ' Fritz!Box Kommunikation
                .P_RBFBComUPnP = RBFBComUPnP.Checked
                ' Telefone

                With xPathTeile
                    .Clear()
                    .Add("Telefone")
                    .Add("Telefone")
                    .Add("*")
                    .Add("Telefon")
                    .Add(DataProvider.P_Def_LeerString)
                    For i = 0 To TelList.Rows.Count - 2
                        .Item(.Count - 1) = "[@Dialport = """ & TelList.Rows(i).Cells(2).Value.ToString & """]"
                        C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "Standard", CStr(CBool(TelList.Rows(i).Cells(0).Value)))
                    Next
                End With

                With xPathTeile
                    .Clear()
                    .Add("Telefone")
                    .Add("Nummern")
                    .Add("*")
                    .Add("[@Checked=""1""]")
                End With

                '.P_CLBTelNr.Clear()
                .P_CLBTelNr = New Collection(Of String)(C_hf.ClearStringArray(Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String), ";", , CompareMethod.Text), False, True, False))

                ' Phoner
                If CBPhoner.Checked Then
                    With xPathTeile
                        .Clear()
                        .Add("Telefone")
                        .Add("Telefone")
                        .Add("*")
                        .Add("Telefon")
                        .Add("[@Dialport > 19 and @Dialport < 30]") ' Nur IP-Telefone
                        .Add("TelName")
                    End With

                    Dim TelNames As String()
                    TelNames = Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, "Phoner"), ";", , CompareMethod.Text)

                    For Each TelName As String In TelNames
                        xPathTeile.Item(xPathTeile.Count - 1) = "[TelName = """ & TelName & """]"
                        C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "PhonerPhone", CStr(TelName = ComboBoxPhonerSIP.SelectedItem.ToString))
                    Next

                    'ThisAddIn.NutzePhonerOhneFritzBox = Me.CBPhonerKeineFB.Checked
                    If TBPhonerPasswort.Text = DataProvider.P_Def_LeerString And CBPhoner.Checked Then
                        If C_hf.MsgBox("Es wurde kein Passwort für Phoner eingegeben! Da Wählen über Phoner wird nicht funktionieren!", MsgBoxStyle.OkCancel, "Speichern") = MsgBoxResult.Cancel Then
                            Speichern = False
                        End If
                    End If


                    If Not TBPhonerPasswort.Text = DataProvider.P_Def_LeerString Then
                        If Not TBPhonerPasswort.Text = "1234" Then
                            .P_TBPhonerPasswort = C_Crypt.EncryptString128Bit(TBPhonerPasswort.Text, DataProvider.P_Def_PassWordDecryptionKey)
                            C_DP.SaveSettingsVBA("ZugangPasswortPhoner", DataProvider.P_Def_PassWordDecryptionKey)
                            C_hf.KeyChange()
                        End If
                    End If
                End If

                If TVOutlookContact.SelectedNode IsNot Nothing Then
                    .P_TVKontaktOrdnerEntryID = Split(CStr(TVOutlookContact.SelectedNode.Tag), ";", , CompareMethod.Text)(0)
                    .P_TVKontaktOrdnerStoreID = Split(CStr(TVOutlookContact.SelectedNode.Tag), ";", , CompareMethod.Text)(1)
                Else
                    C_KF.GetOutlookFolder(.P_TVKontaktOrdnerEntryID, .P_TVKontaktOrdnerStoreID)
                End If

                ' Anruflisten
                If Not TBNumEntryList.Text = DataProvider.P_Def_LeerString Then
                    If CInt(TBNumEntryList.Text) < 1 Then
                        TBNumEntryList.Text = CStr(DataProvider.P_Def_TBNumEntryList)
                    End If
                Else
                    TBNumEntryList.Text = CStr(DataProvider.P_Def_TBNumEntryList)
                End If

                If CInt(TBNumEntryList.Text) < .P_TBNumEntryList Then
                    ' Lösche  CallList
                    C_XML.Delete(C_DP.XMLDoc, DataProvider.P_Def_NameListCALL)
                    ' Lösche  RingList
                    C_XML.Delete(C_DP.XMLDoc, DataProvider.P_Def_NameListRING)
                End If
                .P_TBNumEntryList = CInt(TBNumEntryList.Text)

                .SpeichereXMLDatei()
                C_DP.P_ValidFBAdr = C_hf.ValidIP(C_DP.P_TBFBAdr)
            End With

        Catch ex As InvalidOperationException
            C_hf.LogFile("Fehler beim Speichern: " & ex.Message)
        Catch ex As Exception
            C_hf.MsgBox("Fehler beim Speichern: " & ex.Message, MsgBoxStyle.Critical, "formCfg Sepeichern")
        End Try
    End Function

#Region "Button Link"
    Private Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BReset.Click, BOK.Click, BAbbruch.Click, BApply.Click, BXML.Click, BAnrMonTest.Click, BIndizierungStart.Click, BIndizierungAbbrechen.Click, BZwischenablage.Click, BTelefonliste.Click, BStartDebug.Click, BResetStat.Click, BProbleme.Click, BStoppUhrAnzeigen.Click, BArbeitsverzeichnis.Click, BRWSTest.Click, BTestLogin.Click, BManLoad.Click

        Select Case CType(sender, Windows.Forms.Button).Name
            Case "BReset"
                ' Startwerte zurücksetzen
                TBLandesVW.Text = DataProvider.P_Def_TBLandesVW
                TBAmt.Text = DataProvider.P_Def_LeerString
                CBCheckMobil.Checked = DataProvider.P_Def_CBCheckMobil
                TBNumEntryList.Text = CStr(DataProvider.P_Def_TBNumEntryList)
                ' Einstellungen für den Anrufmonitor zurücksetzen
                TBEnblDauer.Text = CStr(DataProvider.P_Def_TBEnblDauer)
                TBAnrMonX.Text = CStr(DataProvider.P_Def_TBAnrMonX)
                TBAnrMonY.Text = CStr(DataProvider.P_Def_TBAnrMonY)
                CBAnrMonAuto.Checked = DataProvider.P_Def_CBAnrMonAuto
                TBAnrBeantworterTimeout.Text = CStr(DataProvider.P_Def_TBAnrBeantworterTimeout)
                CBAutoClose.Checked = DataProvider.P_Def_CBAutoClose
                CBAnrMonMove.Checked = DataProvider.P_Def_CBAnrMonMove
                CBAnrMonTransp.Checked = DataProvider.P_Def_CBAnrMonTransp
                CBAnrMonContactImage.Checked = DataProvider.P_Def_CBAnrMonContactImage
                CBShowMSN.Checked = DataProvider.P_Def_CBShowMSN
                TBAnrMonMoveGeschwindigkeit.Value = DataProvider.P_Def_TBAnrMonMoveGeschwindigkeit
                CBoxAnrMonMoveDirection.SelectedIndex = DataProvider.P_Def_CBoxAnrMonMoveDirection
                CBoxAnrMonStartPosition.SelectedIndex = DataProvider.P_Def_CBoxAnrMonStartPosition
                CBAnrMonZeigeKontakt.Checked = DataProvider.P_Def_CBAnrMonZeigeKontakt
                CBIndexAus.Checked = DataProvider.P_Def_CBIndexAus
                ' optionale allgemeine Einstellungen zuruecksetzen
                CBVoIPBuster.Checked = DataProvider.P_Def_CBVoIPBuster
                CBDialPort.Checked = DataProvider.P_Def_CBDialPort
                CBCallByCall.Checked = DataProvider.P_Def_CBCallByCall
                CBCbCunterbinden.Checked = DataProvider.P_Def_CBCbCunterbinden
                CBKErstellen.Checked = DataProvider.P_Def_CBKErstellen
                CBLogFile.Checked = DataProvider.P_Def_CBLogFile
                CBForceFBAddr.Checked = DataProvider.P_Def_CBForceFBAddr

                ' Einstellungen für die Kontaktsuche zurücksetzen
                CBRWS.Checked = DataProvider.P_Def_CBRWS
                ComboBoxRWS.Enabled = DataProvider.P_Def_CBRWS
                ComboBoxRWS.SelectedIndex = DataProvider.P_Def_ComboBoxRWS
                CBRWSIndex.Checked = DataProvider.P_Def_CBRWSIndex
                CBKHO.Checked = DataProvider.P_Def_CBKHO
                ' Einstellungen für das Journal zurücksetzen
                CBJournal.Checked = DataProvider.P_Def_CBJournal
                CBAutoAnrList.Checked = DataProvider.P_Def_CBJImport
                CBAnrListeUpdateJournal.Checked = DataProvider.P_Def_CBAnrListeUpdateJournal
                CBAnrListeUpdateCallLists.Checked = DataProvider.P_Def_CBAnrListeUpdateCallLists
                CBAnrListeShowAnrMon.Checked = DataProvider.P_Def_CBAnrListeShowAnrMon
                CBLogFile.Checked = DataProvider.P_Def_CBLogFile
                ' StoppUhr
                CBStoppUhrEinblenden.Checked = DataProvider.P_Def_CBStoppUhrEinblenden
                CBStoppUhrAusblenden.Checked = DataProvider.P_Def_CBStoppUhrAusblenden
                TBStoppUhr.Text = CStr(DataProvider.P_Def_TBStoppUhr)
                CBStoppUhrIgnIntFax.Checked = DataProvider.P_Def_CBStoppUhrIgnIntFax
                ' Telefonnummernformat
                TBTelNrMaske.Text = DataProvider.P_Def_TBTelNrMaske
                CBTelNrGruppieren.Checked = DataProvider.P_Def_CBTelNrGruppieren
                CBintl.Checked = DataProvider.P_Def_CBintl
                CBIgnoTelNrFormat.Checked = DataProvider.P_Def_CBIgnoTelNrFormat
                ' Notiz
                CBNote.Checked = DataProvider.P_Def_CBNote
                ' Fritz!Box Kommunikation
                RBFBComUPnP.Checked = DataProvider.P_Def_RBFBComUPnP
                C_hf.LogFile("Einstellungen zurückgesetzt")
            Case "BTelefonliste"
                C_FBox.SetEventProvider(emc)
                BTelefonliste.Enabled = False
                BTelefonliste.Text = "Bitte warten..."
                Windows.Forms.Application.DoEvents()
                Speichern()

                BWTelefone = New BackgroundWorker
                With BWTelefone
                    .WorkerReportsProgress = False
                    .RunWorkerAsync(True)
                End With
            Case "BOK"
                Dim formschließen As Boolean = Speichern()
                C_DP.P_CBUseAnrMon = CBUseAnrMon.Checked
                C_GUI.RefreshRibbon()

                If formschließen Then Hide()
            Case "BAbbruch"
                ' Schließt das Fenster
                Hide()
            Case "BApply"
                Speichern()
            Case "BXML"
                Process.Start(C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Config_FileName)
            Case "BAnrMonTest"
                Speichern()
                C_PopUp.AnrMonEinblenden(C_AnrMon.LetzterAnrufer)

            Case "BZwischenablage"
                If Not TBDiagnose.Text = DataProvider.P_Def_LeerString Then
                    My.Computer.Clipboard.SetText(TBDiagnose.Text)
                End If
            Case "BProbleme"
                Dim T As New Thread(AddressOf NeueMail)

                If C_hf.MsgBox("Der Einstellungsdialog wird jetzt geschlossen. Danach werden alle erforderlichen Informationen gesammelt, was ein paar Sekunden dauern kann." & vbNewLine & "Danach wird eine neue E-Mail geöffnet, die Sie bitte vervollständigen und absenden.", MsgBoxStyle.OkCancel, "") = MsgBoxResult.Ok Then
                    T.Start()
                    Close()
                Else
                    T = Nothing
                End If
            Case "BStartDebug"
                TBDiagnose.Text = DataProvider.P_Def_LeerString
                AddLine("Start")
                C_FBox.SetEventProvider(emc)
                AddLine("Fritz!Box Typ: " & C_FBox.P_FritzBoxTyp)
                AddLine("Fritz!Box Firmware: " & C_FBox.P_FritzBoxFirmware)

                BWTelefone = New BackgroundWorker
                AddLine("BackgroundWorker erstellt.")
                With BWTelefone
                    .WorkerReportsProgress = True
                    .RunWorkerAsync(DataProvider.P_Debug_ImportTelefone)
                    AddLine("BackgroundWorker gestartet.")
                End With

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
                    C_XML.Delete(C_DP.XMLDoc, xPathTeile)
                    .Item(.Count - 1) = "Ausgehend"
                    C_XML.Delete(C_DP.XMLDoc, xPathTeile)
                End With
                C_DP.SpeichereXMLDatei()
                Ausfüllen()
                xPathTeile = Nothing
            Case "BIndizierungStart"
                StarteIndizierung()
            Case "BIndizierungAbbrechen"
                BWIndexer.CancelAsync()
                BIndizierungAbbrechen.Enabled = False
                BIndizierungStart.Enabled = True
            Case "BStoppUhrAnzeigen"
                Speichern()
                C_PopUp.StoppuhrEinblenden(C_AnrMon.LetzterAnrufer)
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
                            ToolTipFBDBConfig.SetToolTip(BXML, "Öffnet die Datei " & vbCrLf & C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Config_FileName)
                            C_DP.SpeichereXMLDatei()
                        End If
                    End If
                End With
            Case "BRWSTest"
                Dim TelNr As String = TBRWSTest.Text
                If IsNumeric(TelNr) Then
                    Dim F_RWS As New formRWSuche(C_hf, C_KF, C_DP, C_XML)
                    Dim rws As Boolean
                    Dim vCard As String = DataProvider.P_Def_LeerString

                    Select Case CType(ComboBoxRWS.SelectedIndex, RückwärtsSuchmaschine)
                        Case RückwärtsSuchmaschine.RWSDasOertliche
                            rws = F_RWS.RWSDasOertiche(TelNr, vCard)
                            'Case RückwärtsSuchmaschine.RWS11880
                            '    rws = F_RWS.RWS11880(TelNr, vCard)
                            'Case RückwärtsSuchmaschine.RWSDasTelefonbuch
                            '    rws = F_RWS.RWSDasTelefonbuch(TelNr, vCard)
                        Case RückwärtsSuchmaschine.RWStelSearch
                            rws = F_RWS.RWStelsearch(TelNr, vCard)
                            'Case RückwärtsSuchmaschine.RWSAlle
                            '    rws = F_RWS.RWSAlle(TelNr, vCard)
                    End Select

                    C_hf.MsgBox("Die Rückwärtssuche mit der Nummer """ & TelNr & """ brachte mit der Suchmaschine """ & ComboBoxRWS.SelectedItem.ToString() & """ " & C_hf.IIf(rws, "folgendes Ergebnis:" & DataProvider.P_Def_EineNeueZeile & DataProvider.P_Def_EineNeueZeile & vCard, "kein Ergebnis."), MsgBoxStyle.Information, "Test der Rückwärtssuche " & ComboBoxRWS.SelectedItem.ToString())
                Else
                    C_hf.MsgBox("Die Telefonnummer """ & TelNr & """ ist ungültig (Test abgebrochen).", MsgBoxStyle.Exclamation, "Test der Rückwärtssuche")
                End If
            Case "BTestLogin"
                Dim SID As String
                If TBPasswort.Text = "1234" Then
                    SID = C_FBox.FBLogin()
                Else
                    SID = C_FBox.FBLogin(TBBenutzer.Text, TBPasswort.Text)
                End If

                If SID = DataProvider.P_Def_SessionID Then
                    BTestLogin.Text = "Fehler!"
                Else
                    BTestLogin.Text = "OK!"
                End If
            Case "BManLoad"
                Using FBD As New FolderBrowserDialog
                    With FBD
                        .Description = "Wähle den Ordner aus, in dem sich die Konfigurationsdateien befinden"
                        .ShowNewFolderButton = False
                        If .ShowDialog() = DialogResult.OK Then
                            C_DP.P_Debug_PfadKonfig = .SelectedPath
                            AddLine("Pfad: " & C_DP.P_Debug_PfadKonfig)
                        End If
                    End With
                End Using
        End Select
    End Sub

    Private Sub Link_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkHomepage.LinkClicked, LinkForum.LinkClicked, LinkEmail.LinkClicked, LinkLogFile.LinkClicked
        Select Case CType(sender, Windows.Forms.LinkLabel).Name
            Case "LinkEmail"
                Close()
                System.Diagnostics.Process.Start("mailto:kruemelino@gert-michael.de")
            Case "LinkForum"
                System.Diagnostics.Process.Start("http://www.ip-phone-forum.de/showthread.php?t=237086")
            Case "LinkHomepage"
                System.Diagnostics.Process.Start("http://github.com/Kruemelino/FritzBoxTelefon-dingsbums")
            Case "LinkLogFile"
                System.Diagnostics.Process.Start(C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Log_FileName)
        End Select
    End Sub

#End Region

#Region "Änderungen"
    Private Sub ValueChanged(sender As Object, e As EventArgs) Handles CBRWS.CheckedChanged, CBCbCunterbinden.CheckedChanged, CBAutoClose.CheckedChanged, CBIndexAus.CheckedChanged, CBUseAnrMon.CheckedChanged, CBAnrMonMove.CheckedChanged, CBStoppUhrEinblenden.CheckedChanged, CBStoppUhrAusblenden.CheckedChanged, CBLogFile.CheckedChanged, TBEnblDauer.TextChanged, TBAnrMonX.TextChanged, TBAnrMonY.TextChanged, TBTelNrMaske.Leave, CLBTelNr.SelectedIndexChanged, TBRWSTest.TextChanged, TBBenutzer.TextChanged, TBPasswort.TextChanged, CBAnrMonCloseAtDISSCONNECT.CheckedChanged, CBJournal.CheckedChanged, CBAnrListeShowAnrMon.CheckedChanged, CBAutoAnrList.CheckedChanged, TBNumEntryList.TextChanged

        Select Case sender.GetType().Name
            Case "CheckBox"
                Select Case CType(sender, CheckBox).Name
                    Case "CBRWS"
                        ' Combobox für Rückwärtssuchmaschinen je nach CheckBox für Rückwärtssuche ein- bzw. ausblenden
                        ComboBoxRWS.Enabled = CBRWS.Checked
                        CBKErstellen.Checked = CBRWS.Checked
                        CBKErstellen.Enabled = CBRWS.Checked
                        CBRWSIndex.Enabled = CBRWS.Checked
                        CBRWSIndex.Checked = CBRWS.Checked
                        LRWSTest.Enabled = CBRWS.Checked
                        TBRWSTest.Enabled = CBRWS.Checked
                        'Me.BRWSTest.Enabled = Me.CBRWS.Checked
                    Case "CBCbCunterbinden"
                        CBCallByCall.Enabled = Not CBCbCunterbinden.Checked
                        If CBCbCunterbinden.Checked Then CBCallByCall.Checked = False
                    Case "CBAutoClose"
                        TBEnblDauer.Enabled = CBAutoClose.Checked
                        LEnblDauer.Enabled = CBAutoClose.Checked
                        CBAnrMonCloseAtDISSCONNECT.Checked = False
                        CBAnrMonCloseAtDISSCONNECT.Enabled = Not CBAutoClose.Checked
                    Case "CBJournal"
                        If Not CBJournal.Checked Then CBAnrListeUpdateJournal.Checked = False
                        CBAnrListeUpdateJournal.Enabled = CBJournal.Checked
                    Case "CBAutoAnrList"
                        If Not CBAutoAnrList.Checked Then
                            CBAnrListeUpdateJournal.Checked = False
                            CBAnrListeUpdateCallLists.Checked = False
                            CBAnrListeShowAnrMon.Checked = False
                        End If
                        CBAnrListeUpdateJournal.Enabled = CBAutoAnrList.Checked
                        CBAnrListeUpdateCallLists.Enabled = CBAutoAnrList.Checked
                        CBAnrListeShowAnrMon.Enabled = CBAutoAnrList.Checked
                    Case "CBAnrListeShowAnrMon"
                        TBAnrBeantworterTimeout.Enabled = CBAnrListeShowAnrMon.Checked
                        LAnrBeantworterTimeout.Enabled = CBAnrListeShowAnrMon.Checked
                    Case "CBIndexAus"
                        BIndizierungStart.Enabled = Not CBIndexAus.Checked
                    Case "CBUseAnrMon"
                        PanelAnrMon.Enabled = CBUseAnrMon.Checked
                        CBIndexAus.Enabled = Not CBUseAnrMon.Checked
                        GroupBoxStoppUhr.Enabled = CBUseAnrMon.Checked

                        If Not CBUseAnrMon.Checked Then
                            CBStoppUhrEinblenden.Checked = False
                            CBStoppUhrAusblenden.Checked = False
                        End If
                    Case "CBStoppUhrEinblenden"
                        CBStoppUhrAusblenden.Enabled = CBStoppUhrEinblenden.Checked
                        If Not CBStoppUhrEinblenden.Checked Then CBStoppUhrAusblenden.Checked = False
                        TBStoppUhr.Enabled = CBStoppUhrAusblenden.Checked And CBStoppUhrEinblenden.Checked
                        LabelStoppUhr.Enabled = CBStoppUhrEinblenden.Checked
                        CBStoppUhrIgnIntFax.Enabled = CBStoppUhrEinblenden.Checked
                    Case "CBStoppUhrAusblenden"
                        TBStoppUhr.Enabled = CBStoppUhrAusblenden.Checked And CBStoppUhrEinblenden.Checked
                    Case "CBLogFile"
                        GBLogging.Enabled = CBLogFile.Checked
                    Case "CBAnrMonMove"
                        CBoxAnrMonMoveDirection.Enabled = CBAnrMonMove.Checked
                        LAnrMonMoveDirection.Enabled = CBAnrMonMove.Checked
                End Select
            Case "TextBox"
                Select Case CType(sender, TextBox).Name
                    Case "TBLandesVW"
                        If TBLandesVW.Text = DataProvider.P_Def_TBLandesVW Then
                            CBRWS.Enabled = True
                            CBKErstellen.Enabled = True
                            ComboBoxRWS.Enabled = CBRWS.Checked
                        Else
                            CBRWS.Checked = False
                            CBRWS.Enabled = False

                            CBKErstellen.Enabled = False
                            CBKErstellen.Checked = False
                            ComboBoxRWS.Enabled = False
                        End If
                    Case "TBVorwahl"
                        TBVorwahl.Text = C_hf.AcceptOnlyNumeric(TBVorwahl.Text)
                    Case "TBEnblDauer"
                        TBEnblDauer.Text = C_hf.AcceptOnlyNumeric(TBEnblDauer.Text)
                    Case "TBAnrMonX"
                        TBAnrMonX.Text = C_hf.AcceptOnlyNumeric(TBAnrMonX.Text)
                    Case "TBAnrMonY"
                        TBAnrMonY.Text = C_hf.AcceptOnlyNumeric(TBAnrMonY.Text)
                    Case "TBNumEntryList"
                        TBNumEntryList.Text = C_hf.AcceptOnlyNumeric(TBNumEntryList.Text)
                        If TBNumEntryList.Text = DataProvider.P_Def_LeerString Or TBNumEntryList.Text < "1" Then TBNumEntryList.Text = CStr(DataProvider.P_Def_TBNumEntryList)
                        If CInt(TBNumEntryList.Text) < C_DP.P_TBNumEntryList Then
                            TBNumEntryList.ForeColor = Color.Red
                        Else
                            TBNumEntryList.ForeColor = SystemColors.WindowText
                        End If
                    Case "TBLandesVW"
                        ToolTipFBDBConfig.SetToolTip(CBVoIPBuster, "Mit dieser Einstellung wird die Landesvorwahl " & TBLandesVW.Text & " immer mitgewählt.")
                    Case "TBTelNrMaske"
                        PrüfeMaske()
                    Case "TBRWSTest"
                        TBRWSTest.Text = C_hf.AcceptOnlyNumeric(TBRWSTest.Text)
                        BRWSTest.Enabled = Len(C_hf.nurZiffern(TBRWSTest.Text)) > 0
                    Case "TBBenutzer", "TBPasswort"
                        BTestLogin.Text = "Test"
                    Case "TBAnrBeantworterTimeout"
                        TBAnrBeantworterTimeout.Text = C_hf.AcceptOnlyNumeric(TBAnrBeantworterTimeout.Text)
                End Select
            Case "CheckedListBox"
                Select Case CType(sender, CheckedListBox).Name
                    Case "CLBTelNr"
                        Dim alle As Boolean = True
                        With CLBTelNr
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
        If TypeOf TelList.CurrentCell Is Windows.Forms.DataGridViewCheckBoxCell Then
            TelList.EndEdit()
            If TelList.CurrentCell.Value IsNot Nothing Then
                Dim cellVal As Boolean = DirectCast(TelList.CurrentCell.Value, Boolean)
                If cellVal Then
                    If Not TelList.CurrentCell Is TelList.Rows(TelList.Rows.Count - 1).Cells(0) Then
                        For i = 0 To TelList.Rows.Count - 1
                            TelList.Rows(i).Cells(0).Value = False
                        Next
                        If Not (TelList.Rows(TelList.CurrentCell.RowIndex).Cells(3).Value.ToString = "TAM" Or TelList.Rows(TelList.CurrentCell.RowIndex).Cells(3).Value.ToString = "FAX") Then TelList.CurrentCell.Value = cellVal
                    Else
                        TelList.CurrentCell.Value = False
                    End If
                End If
            End If
        End If
    End Sub
#End Region

#Region "Helfer"
    Private Function PrüfeMaske() As Boolean
        ' "%L (%O) %N - %D"
        Dim pos(2) As String
        pos(0) = CStr(InStr(TBTelNrMaske.Text, "%L", CompareMethod.Text))
        pos(1) = CStr(InStr(TBTelNrMaske.Text, "%O", CompareMethod.Text))
        pos(2) = CStr(InStr(TBTelNrMaske.Text, "%N", CompareMethod.Text))
        If pos.Contains("0") Then
            'If C_hf.IsOneOf("0", pos) Then
            C_hf.MsgBox("Achtung: Die Maske für die Telefonnummernformatierung ist nicht korrekt." & vbNewLine & "Prüfen Sie, ob folgende Zeichen in der Maske enthalten sind: ""%L"", ""%V"" und ""%N"" (""%D"" kann wegelassen werden)!" & vbNewLine & "Beispiel: ""%L (%O) %N - %D""", MsgBoxStyle.Information, "Einstellungen")
            Return False
        End If
        Return True
    End Function

    Private Sub NeueMail()
        C_FBox.FritzBoxDaten(True, True)

        C_OlI.NeueEmail(DataProvider.P_FritzBox_Info(C_FBox.P_FritzBoxTyp, C_FBox.P_FritzBoxFirmware))
    End Sub
    Private Function SetTelNrListe() As Boolean
        SetTelNrListe = False
        If InvokeRequired Then
            Dim D As New DelgSetLine(AddressOf CLBTelNrAusfüllen)
            Invoke(D)
        Else
            CLBTelNrAusfüllen()
        End If
    End Function

    Private Function SetFillTelListe() As Boolean
        SetFillTelListe = False
        If InvokeRequired Then
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

    Private Function AddLine(ByVal Zeile As String) As Boolean
        AddLine = False
        StatusWert = Zeile
        If InvokeRequired Then
            Dim D As New DelgSetLine(AddressOf setline)
            Invoke(D)
        Else
            setline()
        End If
    End Function

    Private Sub setline()
        LTelStatus.Text = "Status: " & StatusWert
        With TBDiagnose
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
        ProgressBarIndex.Value = 0
        LabelAnzahl.Text = "Status: 0/" & CStr(ProgressBarIndex.Maximum)
        BIndizierungAbbrechen.Enabled = True
        BIndizierungStart.Enabled = False
        LabelAnzahl.Text = "Status: Bitte Warten!"
        With BWIndexer
            .WorkerSupportsCancellation = True
            .WorkerReportsProgress = True
            .RunWorkerAsync()
        End With

    End Sub

#Region "Indizierung - Vorbereitung"
    Private Function ErmittleKontaktanzahl() As Boolean
        ErmittleKontaktanzahl = True
        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder

        Anzahl = 0
        olNamespace = C_OlI.OutlookApplication.GetNamespace("MAPI")

        If CBKHO.Checked Then
            olfolder = C_KF.P_DefContactFolder
            ZähleKontakte(olfolder, Nothing)
        Else
            ZähleKontakte(Nothing, olNamespace)
        End If
        If InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbarMax)
            Invoke(D)
        Else
            SetProgressbarMax()
        End If
    End Function

    Private Function ZähleKontakte(ByVal Ordner As Outlook.MAPIFolder, ByVal NamensRaum As Outlook.NameSpace) As Integer

        ZähleKontakte = 0
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If NamensRaum IsNot Nothing Then
            For Each olFolder As Outlook.MAPIFolder In NamensRaum.Folders
                ZähleKontakte(olFolder, Nothing)
            Next
            Return 0
        End If

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            'Debug.Print(Ordner.Name, Ordner.Items.Count)
            Anzahl += Ordner.Items.Count
        End If

        ' Unterordner werden rekursiv durchsucht
        For Each olFolder As Outlook.MAPIFolder In Ordner.Folders
            ZähleKontakte(olFolder, Nothing)
        Next
    End Function
#End Region

    Private Sub KontaktIndexer(ByVal KorrNumbers As Boolean, Optional ByVal Ordner As Outlook.MAPIFolder = Nothing, Optional ByVal NamensRaum As Outlook.NameSpace = Nothing) 'as Boolean
        'KontaktIndexer = False
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        'Dim item As Object      ' aktuelles Element
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If NamensRaum IsNot Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                KontaktIndexer(CBTelFormKorr.Checked, CType(NamensRaum.Folders.Item(j), Outlook.MAPIFolder))
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
                        If KorrNumbers Then C_KF.KontaktFormatTelNr(aktKontakt)
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
                KontaktIndexer(CBTelFormKorr.Checked, CType(Ordner.Folders.Item(iOrdner), Outlook.MAPIFolder))
                iOrdner = iOrdner + 1
            Loop
            aktKontakt = Nothing
        End If
    End Sub

    Private Overloads Sub KontaktDeIndexer(ByVal NamensRaum As Outlook.NameSpace) 'As Boolean

        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If NamensRaum IsNot Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                KontaktDeIndexer(CType(NamensRaum.Folders.Item(j), Outlook.MAPIFolder))
                j = j + 1
            Loop
            aktKontakt = Nothing
            'Return True
        End If
    End Sub

    Private Overloads Sub KontaktDeIndexer(ByVal Ordner As Outlook.MAPIFolder) 'As Boolean

        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt

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
            KontaktDeIndexer(Ordner.Folders.Item(iOrdner))
            iOrdner = iOrdner + 1
        Loop
        aktKontakt = Nothing
    End Sub
#End Region

#Region "Logging"
    Sub FillLogTB()
        Dim LogDatei As String = C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Log_FileName

        If C_DP.P_CBLogFile Then
            If My.Computer.FileSystem.FileExists(LogDatei) Then
                TBLogging.Text = My.Computer.FileSystem.OpenTextFileReader(LogDatei).ReadToEnd
            End If
        End If
        LinkLogFile.Text = LogDatei
    End Sub

    Private Sub FBDB_MP_TabIndexChanged(sender As Object, e As EventArgs) Handles FBDB_MP.SelectedIndexChanged
        Update()
        If FBDB_MP.SelectedTab.Name = "PLogging" Then
            With TBLogging
                .Focus()
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End With
        End If
    End Sub

    Private Sub BLogging_Click(sender As Object, e As EventArgs) Handles BLogging.Click
        With TBLogging
            If .SelectedText = DataProvider.P_Def_LeerString Then
                My.Computer.Clipboard.SetText(.Text)
            Else
                My.Computer.Clipboard.SetText(.SelectedText)
            End If
        End With
    End Sub

#End Region

#Region "Delegate"
    Private Sub SetProgressbar()
        With ProgressBarIndex
            .Value += CInt(StatusWert)
            LabelAnzahl.Text = "Status: " & .Value & "/" & CStr(.Maximum) & KontaktName
        End With
    End Sub

    Private Sub SetProgressbarToMax()
        With ProgressBarIndex
            If RadioButtonErstelle.Checked And Not RadioButtonEntfernen.Checked Then
                .Value = .Maximum
            ElseIf RadioButtonEntfernen.Checked And Not RadioButtonErstelle.Checked Then
                .Value = 0
            End If
        End With
        BIndizierungStart.Enabled = True
        BIndizierungAbbrechen.Enabled = False
    End Sub

    Private Sub SetProgressbarMax()
        ProgressBarIndex.Maximum = Anzahl
    End Sub

    Private Sub DelBTelefonliste()
        If InvokeRequired Then
            Dim D As New DelgButtonTelEinl(AddressOf DelBTelefonliste)
            Invoke(D)
        Else
            BTelefonliste.Text = "Telefone erneut einlesen"
            BTelefonliste.Enabled = True
        End If
    End Sub

    Private Sub DelSetTreeView()
        If InvokeRequired Then
            Dim D As New DelgButtonTelEinl(AddressOf DelSetTreeView)
            Invoke(D)
        Else
            Dim tmpNode As TreeNode()
            C_OlI.GetKontaktOrdnerInTreeView(TVOutlookContact)
            With TVOutlookContact
                tmpNode = .Nodes.Find(C_DP.P_TVKontaktOrdnerEntryID & ";" & C_DP.P_TVKontaktOrdnerStoreID, True)
                If Not tmpNode.Length = 0 Then
                    .SelectedNode = tmpNode(0)
                    .SelectedNode.Checked = True
                End If
                .ExpandAll()
                .Enabled = True
            End With
        End If
    End Sub

#End Region

#Region "BackGroundWorker - Handle"
    Private Sub BWIndexer_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWIndexer.DoWork

        ErmittleKontaktanzahl()
        If RadioButtonEntfernen.Checked And Not RadioButtonErstelle.Checked Then
            StatusWert = ProgressBarIndex.Maximum.ToString
            BWIndexer.ReportProgress(ProgressBarIndex.Maximum)
        End If

        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder

        olNamespace = C_OlI.OutlookApplication.GetNamespace("MAPI")

        If CBKHO.Checked Then
            olfolder = C_KF.P_DefContactFolder
            If RadioButtonErstelle.Checked Then
                KontaktIndexer(CBTelFormKorr.Checked, Ordner:=olfolder)
            ElseIf RadioButtonEntfernen.Checked Then
                KontaktDeIndexer(olfolder)
            End If
        Else
            If RadioButtonErstelle.Checked Then
                KontaktIndexer(CBTelFormKorr.Checked, NamensRaum:=olNamespace)
            ElseIf RadioButtonEntfernen.Checked Then
                KontaktDeIndexer(olNamespace)
            End If
        End If
    End Sub

    Private Sub BWIndexer_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles BWIndexer.ProgressChanged
        StatusWert = CStr(e.ProgressPercentage)
        If InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbar)
            Invoke(D)
        Else
            SetProgressbar()
        End If
    End Sub

    Private Sub BWIndexer_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWIndexer.RunWorkerCompleted

        If InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbarToMax)
            Invoke(D)
        Else
            SetProgressbarToMax()
        End If
        BWIndexer.Dispose()
        Dauer = Date.Now - Startzeit
        If RadioButtonErstelle.Checked And Not RadioButtonEntfernen.Checked Then
            C_DP.P_LLetzteIndizierung = Date.Now
            C_hf.LogFile("Indizierung abgeschlossen: " & ProgressBarIndex.Value & " von " & Anzahl & " Kontakten in " & Dauer.TotalSeconds & " s")
        ElseIf RadioButtonEntfernen.Checked And Not RadioButtonErstelle.Checked Then
            C_hf.LogFile("Deindizierung abgeschlossen: " & ProgressBarIndex.Value & " von " & Anzahl & " Kontakten in " & Dauer.TotalSeconds & " s")
        End If
    End Sub

    Private Sub BWTelefone_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWTelefone.DoWork
        AddLine("Einlesen der Telefone gestartet.")

        C_FBox.P_SpeichereDaten = CBool(e.Argument)
        e.Result = CBool(e.Argument)
        C_FBox.FritzBoxDaten(False, C_DP.P_Debug_PfadKonfig = DataProvider.P_Def_LeerString)
    End Sub

    Private Sub BWTelefone_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWTelefone.RunWorkerCompleted
        AddLine("BackgroundWorker zum Einlesen der Telefone ist fertig.")
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
        End With

        For Row = 0 To TelList.Rows.Count - 2
            xPathTeile.Item(xPathTeile.Count - 2) = "[@Dialport = """ & TelList.Rows(Row).Cells(2).Value.ToString & """]"
            xPathTeile.Item(xPathTeile.Count - 1) = "TelName"
            ' Prüfe ob Telefonname und Telefonnummer übereinstimmt
            tmpTelefon = C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String)
            If Not tmpTelefon = DataProvider.P_Def_ErrorMinusOne_String Then
                xPathTeile.Item(xPathTeile.Count - 1) = "TelNr"
                If Not ((TelList.Rows(Row).Cells(4).Value Is Nothing) Or (TelList.Rows(Row).Cells(5).Value Is Nothing)) Then
                    If tmpTelefon = TelList.Rows(Row).Cells(4).Value.ToString And C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String) = Replace(TelList.Rows(Row).Cells(5).Value.ToString, ", ", ";", , , CompareMethod.Text) Then
                        If C_XML.GetProperXPath(C_DP.XMLDoc, xPathTeile) Then
                            Dim Dauer As Date
                            xPathTeile.Item(xPathTeile.Count - 1) = "Eingehend"
                            Dauer = CDate(TelList.Rows(Row).Cells(6).Value.ToString())
                            C_XML.Write(C_DP.XMLDoc, xPathTeile, CStr((Dauer.Hour * 60 + Dauer.Minute) * 60 + Dauer.Second))
                            xPathTeile.Item(xPathTeile.Count - 1) = "Ausgehend"
                            Dauer = CDate(TelList.Rows(Row).Cells(7).Value.ToString())
                            C_XML.Write(C_DP.XMLDoc, xPathTeile, CStr((Dauer.Hour * 60 + Dauer.Minute) * 60 + Dauer.Second))
                        End If
                    End If
                End If
            End If
        Next

        With xPathTeile
            'CLBTelNrAusfüllen setzen
            .Clear()
            Dim CheckTelNr As CheckedListBox.CheckedItemCollection = CLBTelNr.CheckedItems
            If Not CheckTelNr.Count = 0 Then
                Dim tmpTeile As String = DataProvider.P_Def_LeerString
                .Add("Telefone")
                .Add("Nummern")
                .Add("*")
                For i = 0 To CheckTelNr.Count - 1
                    tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
                Next
                tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
                .Add("[" & tmpTeile & "]")
                C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "Checked", "1")
            End If
        End With

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("[@Dialport > 19 and @Dialport < 30]") ' Nur IP-Telefone
            .Add("TelName")
        End With

        ComboBoxPhonerSIP.DataSource = Split(C_XML.Read(C_DP.XMLDoc, xPathTeile, "Phoner"), ";", , CompareMethod.Text)

        AddLine("Speichere Einstellungen")
        Speichern()
        AddLine("Fülle Telefonnummernliste in den Einstellungen")
        SetTelNrListe()
        AddLine("Fülle Telefonliste in den Einstellungen")
        SetFillTelListe()
        AddLine("Setze Button Label und räume auf")
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
        PanelPhonerAktiv.BackColor = C_hf.IIf(PhonerInstalliert, Color.LightGreen, Color.Red)
        LabelPhoner.Text = "Phoner ist " & C_hf.IIf(PhonerInstalliert, "", "nicht ") & "aktiv."
        PanelPhoner.Enabled = PhonerInstalliert
        C_DP.P_PhonerVerfügbar = PhonerInstalliert
    End Sub

    Private Sub CBPhoner_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBPhoner.CheckedChanged
        TBPhonerPasswort.Enabled = CBPhoner.Checked
        LPassworPhoner.Enabled = CBPhoner.Checked
    End Sub


#End Region

End Class


