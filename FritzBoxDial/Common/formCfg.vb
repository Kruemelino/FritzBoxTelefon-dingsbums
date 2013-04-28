Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports System.Threading

Public Class formCfg
    Private C_ini As InI
    Private C_Crypt As Rijndael
    Private C_Helfer As Helfer
    Private C_Kontakte As Contacts
    Private GUI As GraphicalUserInterface
    Private OlI As OutlookInterface
    Private AnrMon As AnrufMonitor
    Private FBox As FritzBox

    Private WithEvents BWTelefone As BackgroundWorker
    Private WithEvents BWIndexer As BackgroundWorker

    Private Dateipfad As String
    Private tmpCheckString As String
    Private StatusWert As String
    Private KontaktName As String
    Private Anzahl As Integer = 0
    Private Dauer As TimeSpan
    Private Startzeit As Date

    Private Delegate Sub DelgButtonTelEinl()
    Private Delegate Sub DelgSetLine()
    Private Delegate Sub DelgStatistik()
    Private Delegate Sub DelgSetProgressbar()

    Public Sub New(ByVal FilePath As String, _
                   ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal iniKlasse As InI, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal CryptKlasse As Rijndael, _
                   ByVal AnrufMon As AnrufMonitor, _
                   ByVal fritzboxKlasse As FritzBox, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal kontaktklasse As Contacts)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        Dateipfad = FilePath
        C_Helfer = HelferKlasse
        C_ini = iniKlasse
        C_Crypt = CryptKlasse
        GUI = InterfacesKlasse
        OlI = OutlInter
        AnrMon = AnrufMon
        FBox = fritzboxKlasse
        C_Kontakte = kontaktklasse
    End Sub

    Private Sub UserForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.TBAnrMonMoveGeschwindigkeit.BackColor = CType(IIf(iTa.IsThemeActive, SystemColors.ControlLightLight, SystemColors.ControlLight), Color)
        Me.ButtonTesten.Enabled = Not AnrMon Is Nothing
        Me.ButtonTelefonliste.Enabled = Not FBox Is Nothing
        Ausfüllen()
    End Sub

    Private Sub Ausfüllen()

        'Dim IP As String
        Dim Passwort As String
#If OVer >= 14 Then
        If Not Me.FBDB_MP.TabPages.Item("PSymbolleiste") Is Nothing Then
            Me.FBDB_MP.TabPages.Remove(Me.FBDB_MP.TabPages.Item("PSymbolleiste"))
        End If
#End If
        ' Beim Einblenden die Werte aus der Registry einlesen
        Me.Label7.Text += ThisAddIn.Version
        ' Einstellungen für das Wählmakro laden
        Me.TBLandesVW.Text = C_ini.Read(Dateipfad, "Optionen", "TBLandesVW", "0049")
        Me.TBAmt.Text = C_ini.Read(Dateipfad, "Optionen", "TBAmt", "")
        Me.TBFBAdr.Text = C_ini.Read(Dateipfad, "Optionen", "TBFBAdr", "fritz.box")
        Me.CBForceFBAddr.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBForceFBAddr", "False") = "True", True, False))
        Passwort = C_ini.Read(Dateipfad, "Optionen", "TBPasswort", "")
        If Not Len(Passwort) = 0 Then
            Me.TBPasswort.Text = "1234"
        End If
        Me.TBVorwahl.Text = C_ini.Read(Dateipfad, "Optionen", "TBVorwahl", "")
        CLBtelnrAusfüllen()
        Me.TBEnblDauer.Text = CStr(CInt(C_ini.Read(Dateipfad, "Optionen", "TBEnblDauer", "10")))
        Me.CBAnrMonAuto.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBAnrMonAuto", "False"))
        Me.TBAnrMonX.Text = C_ini.Read(Dateipfad, "Optionen", "TBAnrMonX", "0")
        Me.TBAnrMonY.Text = C_ini.Read(Dateipfad, "Optionen", "TBAnrMonY", "0")
        Me.CBAnrMonMove.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBAnrMonMove", "True") = "True", True, False))
        Me.CBAnrMonTransp.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBAnrMonTransp", "True") = "True", True, False))
        Me.TBAnrMonMoveGeschwindigkeit.Value = CInt((100 - CDbl(C_ini.Read(Dateipfad, "Optionen", "TBAnrMonMoveGeschwindigkeit", "50"))) / 10)
        Me.CBAnrMonContactImage.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBAnrMonContactImage", "True") = "True", True, False))
        Me.CBIndexAus.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBIndexAus", "False"))
        Me.CBShowMSN.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBShowMSN", "False"))
        ' optionale allgemeine Einstellungen laden
        Me.CBAutoClose.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBAutoClose", "True") = "True", True, False))
        Me.CBVoIPBuster.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBVoIPBuster", "False") = "True", True, False))
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBVoIPBuster, "Mit dieser Einstellung wird die Landesvorwahl " & Me.TBLandesVW.Text & " immer mitgewählt.")
        Me.CBCbCunterbinden.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBCbCunterbinden", "False") = "True", True, False))
        Me.CBCallByCall.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBCallByCall", "False") = "True", True, False))
        Me.CBDialPort.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBDialPort", "False") = "True", True, False))
        Me.CBRückwärtssuche.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBRückwärtssuche", "False") = "True", True, False))
        Me.CBKErstellen.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBKErstellen", "False") = "True", True, False))
        Me.CBLogFile.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBLogFile", "False") = "True", True, False))
#If OVer < 14 Then
        ' Einstellungen für die Symbolleiste laden
        Me.CBSymbWwdh.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbWwdh", "True") = "True", True, False))
        Me.CBSymbAnrMon.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbAnrMon", "True") = "True", True, False))
        Me.CBSymbAnrMonNeuStart.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbAnrMonNeuStart", "False") = "True", True, False))
        Me.CBSymbAnrListe.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbAnrListe", "True") = "True", True, False))
        Me.CBSymbDirekt.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbDirekt", "True") = "True", True, False))
        Me.CBSymbRWSuche.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbRWSuche", "True") = "True", True, False))
        Me.CBSymbVIP.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbVIP", "False") = "True", True, False))
        Me.CBSymbJournalimport.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBSymbJournalimport", "False") = "True", True, False))
#End If
        Me.CBJImport.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBJImport", "False") = "True", True, False))
        ' Einstellungen für die Rückwärtssuche laden
        Me.CBKHO.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBKHO", "True") = "True", True, False))
        Me.CBRWSIndex.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBRWSIndex", "True") = "True", True, False))
        With Me.ComboBoxRWS.Items
            .Add("GoYellow.de")
            .Add("11880.com")
            .Add("DasTelefonbuch.de")
            .Add("tel.search.ch")
            .Add("Alle")
        End With

        Me.ComboBoxRWS.SelectedItem = Me.ComboBoxRWS.Items.Item(CInt(C_ini.Read(Dateipfad, "Optionen", "CBoxRWSuche", "0")))
        If Not Me.CBRückwärtssuche.Checked Then Me.ComboBoxRWS.Enabled = False
        ' Einstellungen für das Journal laden
        Me.CBJournal.Checked = CBool(IIf(C_ini.Read(Dateipfad, "Optionen", "CBJournal", "False") = "True", True, False))

        Statistik()
        With C_Helfer
            Me.ButtonIndexDateiöffnen.Enabled = My.Computer.FileSystem.FileExists(.Dateipfade(Dateipfad, "KontaktIndex"))
            Me.ButtonLog.Enabled = My.Computer.FileSystem.FileExists(.Dateipfade(Dateipfad, "LogDatei"))
            Me.ButtonListen.Enabled = My.Computer.FileSystem.FileExists(.Dateipfade(Dateipfad, "Listen"))
        End With

        Me.CBUseAnrMon.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBUseAnrMon", "True"))
        Me.CBIndexAus.Enabled = Not Me.CBUseAnrMon.Checked
        Me.PanelAnrMon.Enabled = Me.CBUseAnrMon.Checked
        Me.CBCheckMobil.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBCheckMobil", "True"))

        'StoppUhr
        Me.CBStoppUhrEinblenden.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBStoppUhrEinblenden", "False"))
        Me.CBStoppUhrAusblenden.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBStoppUhrAusblenden", "False"))
        Me.TBStoppUhr.Text = C_ini.Read(Dateipfad, "Optionen", "TBStoppUhr", "10")

        Me.CBStoppUhrAusblenden.Enabled = Me.CBStoppUhrEinblenden.Checked
        If Not Me.CBStoppUhrEinblenden.Checked Then Me.CBStoppUhrAusblenden.Checked = False
        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked

        'Telefonnummernformat
        Me.TBTelNrMaske.Text = C_ini.Read(Dateipfad, "Optionen", "TBTelNrMaske", "%L (%O) %N - %D")
        Me.CBTelNrGruppieren.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBTelNrGruppieren", "True"))
        Me.CBintl.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBintl", "False"))
        Me.CBIgnoTelNrFormat.Checked = CBool(C_ini.Read(Dateipfad, "Optionen", "CBIgnoTelNrFormat", "False"))
#If OVer < 14 Then
        If Not Me.CBJournal.Checked Then Me.CBSymbJournalimport.Checked = False
        Me.CBSymbJournalimport.Enabled = Me.CBJournal.Checked
#End If
    End Sub

    Private Sub Statistik()
        Dim row(Me.TelList.ColumnCount) As String
        Dim Nebenstellen() As String
        Dim j As Integer
        Dim TelName() As String
        Dim TelAnzahl As String
        With Me.TelList
            For j = 0 To .RowCount - 1
                .Rows.RemoveAt(0)
            Next
        End With
        Dim StandardTelefon As String = C_ini.Read(Dateipfad, "Telefone", "CBStandardTelefon", "-1")
        Nebenstellen = Split(C_ini.Read(Dateipfad, "Telefone", "EingerichteteTelefone", "1,2,3,5,51,52,53,54,55,56,57,58,50,60,61,62,63,64,65,66,67,68,69,20,21,22,23,24,25,26,27,28,29,5,600,601,602,603,604"), ";", , CompareMethod.Text)
        TelAnzahl = C_ini.Read(Dateipfad, "Telefone", "Anzahl", "-1")
        If Not TelAnzahl = "-1" Then
            With Me.TelList
                j = 0
                For Each Nebenstelle In Nebenstellen
                    TelName = Split(C_ini.Read(Dateipfad, "Telefone", Nebenstelle, "-1;"), ";", , CompareMethod.Text)
                    If Not TelName(0) = "-1" And Not TelName.Length = 2 Then
                        j += 1
                        row(1) = CStr(j) ' Zählvariable
                        row(2) = Nebenstelle
                        row(3) = TelName(2) ' TelName
                        row(4) = Telefontyp(CInt(Nebenstelle))
                        row(5) = Replace(TelName(1), "_", ", ", , , CompareMethod.Text) ' Eingehnd
                        row(6) = Replace(TelName(0), "_", ", ", , , CompareMethod.Text) ' Ausgehnd
                        row(7) = GetTimeInterval(CInt(C_ini.Read(Dateipfad, "Statistik", TelName(0) & "ein", "0")))
                        row(8) = GetTimeInterval(CInt(C_ini.Read(Dateipfad, "Statistik", TelName(0) & "aus", "0")))
                        row(9) = GetTimeInterval(CInt(C_ini.Read(Dateipfad, "Statistik", TelName(0) & "ein", "0")) + CInt(C_ini.Read(Dateipfad, "Statistik", TelName(0) & "aus", "0")))
                        .Rows.Add(row)
                        If Not StandardTelefon = "-1" And StandardTelefon = row(2) Then .Rows(.RowCount - 1).Cells(0).Value = True
                    End If
                Next


                row(1) = Nothing
                row(2) = Nothing
                row(3) = Nothing
                row(4) = Nothing
                row(5) = "Summe:"
                row(6) = GetTimeInterval(CInt(C_ini.Read(Dateipfad, "Statistik", "eingehend", "0")))
                row(7) = GetTimeInterval(CInt(C_ini.Read(Dateipfad, "Statistik", "ausgehend", "0")))
                row(8) = GetTimeInterval(CInt(C_ini.Read(Dateipfad, "Statistik", "eingehend", "0")) + CInt(C_ini.Read(Dateipfad, "Statistik", "ausgehend", "0")))
                .Rows.Add(row)
            End With
        End If

        If C_ini.Read(Dateipfad, "Statistik", "ResetZeit", "-1") = "-1" Then
            C_ini.Write(Dateipfad, "Statistik", "ResetZeit", CStr(System.DateTime.Now))
        End If
        Me.TBAnderes.Text = C_ini.Read(Dateipfad, "Statistik", "Verpasst", "0") & " verpasste Telefonate" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_ini.Read(Dateipfad, "Statistik", "Nichterfolgreich", "0") & " nicht erfolgreiche Telefonate" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_ini.Read(Dateipfad, "Statistik", "Kontakt", "0") & " erstellte Kontakte" & vbCrLf
        Me.TBAnderes.Text = Me.TBAnderes.Text & C_ini.Read(Dateipfad, "Statistik", "Journal", "0") & " erstellte Journaleinträge" & vbCrLf
        Me.TBReset.Text = "Letzter Reset: " & C_ini.Read(Dateipfad, "Statistik", "ResetZeit", "Noch nicht festgelegt")
        Me.TBSchließZeit.Text = "Letzter Journaleintrag: " & C_ini.Read(Dateipfad, "Journal", "SchließZeit", "Noch nicht festgelegt")

    End Sub

    Private Function Telefontyp(ByVal Nebenstelle As Integer) As String
        Select Case Nebenstelle
            Case 1 To 3
                Return "FON" & Nebenstelle
            Case 5
                Return "Fax"
            Case 50 To 59
                Return "S0"
            Case 60 To 65
                Return "DECT"
            Case 20 To 29
                Return "IP"
            Case 600 To 604
                Return "AB"
            Case Else
                Return "?"
        End Select
    End Function

    Private Function Speichern() As Boolean
        Speichern = True

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

        C_ini.Write(Dateipfad, "Telefone", "CLBTelNr", checkstring)
        'C_ini.Write(Dateipfad, "Optionen", "CBAutoUpdate", CStr(Me.CBAutoUpdate.Checked))
        ' Sichert die Einstellungen und schließt das Fenster
        If (CInt(Me.TBEnblDauer.Text) < 4) Then Me.TBEnblDauer.Text = "4"
        SaveSetting("FritzBox", "Optionen", "TBini", Dateipfad)
        C_ini.Write(Dateipfad, "Optionen", "TBLandesVW", Me.TBLandesVW.Text)
        C_ini.Write(Dateipfad, "Optionen", "TBAmt", Me.TBAmt.Text)
        C_ini.Write(Dateipfad, "Optionen", "TBFBAdr", Me.TBFBAdr.Text)
        C_ini.Write(Dateipfad, "Optionen", "CBForceFBAddr", CStr(Me.CBForceFBAddr.Checked))
        C_ini.Write(Dateipfad, "Optionen", "TBAnrMonX", Me.TBAnrMonX.Text)
        C_ini.Write(Dateipfad, "Optionen", "TBAnrMonY", Me.TBAnrMonY.Text)
        If Not Me.TBPasswort.Text = "1234" Then
            C_ini.Write(Dateipfad, "Optionen", "TBPasswort", C_Crypt.EncryptString128Bit(Me.TBPasswort.Text, "Fritz!Box Script"))
            SaveSetting("FritzBox", "Optionen", "Zugang", "Fritz!Box Script")
            C_Helfer.KeyÄnderung(Dateipfad)
        End If
        C_ini.Write(Dateipfad, "Optionen", "TBVorwahl", Me.TBVorwahl.Text)
        C_ini.Write(Dateipfad, "Optionen", "CBLogFile", CStr(Me.CBLogFile.Checked))
        ' Einstellungen für den Anrufmonitor speichern
        C_ini.Write(Dateipfad, "Optionen", "TBEnblDauer", CStr(Int(CDbl(Me.TBEnblDauer.Text))))
        C_ini.Write(Dateipfad, "Optionen", "CBAnrMonAuto", CStr(Me.CBAnrMonAuto.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBAutoClose", CStr(Me.CBAutoClose.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBAnrMonMove", CStr(Me.CBAnrMonMove.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBAnrMonTransp", CStr(Me.CBAnrMonTransp.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBAnrMonContactImage", CStr(Me.CBAnrMonContactImage.Checked))
        C_ini.Write(Dateipfad, "Optionen", "TBAnrMonMoveGeschwindigkeit", CStr((10 - Me.TBAnrMonMoveGeschwindigkeit.Value) * 10))
        C_ini.Write(Dateipfad, "Optionen", "CBIndexAus", CStr(Me.CBIndexAus.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBShowMSN", CStr(Me.CBShowMSN.Checked))
        ' optionale allgemeine Einstellungen speichern
        C_ini.Write(Dateipfad, "Optionen", "CBVoIPBuster", CStr(Me.CBVoIPBuster.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBDialPort", CStr(Me.CBDialPort.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBCbCunterbinden", CStr(Me.CBCbCunterbinden.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBCallByCall", CStr(Me.CBCallByCall.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBRückwärtssuche", CStr(Me.CBRückwärtssuche.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBKErstellen", CStr(Me.CBKErstellen.Checked))
        ' Einstellungen für die Rückwärtssuche speichern
        C_ini.Write(Dateipfad, "Optionen", "CBoxRWSuche", CStr(Me.ComboBoxRWS.SelectedIndex))
        C_ini.Write(Dateipfad, "Optionen", "CBKHO", CStr(Me.CBKHO.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBRWSIndex", CStr(Me.CBRWSIndex.Checked))
        ' Einstellungen für das Journal speichern
        C_ini.Write(Dateipfad, "Optionen", "CBJournal", CStr(Me.CBJournal.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBJImport", CStr(Me.CBJImport.Checked))
        ' NEU
        C_ini.Write(Dateipfad, "Optionen", "CBUseAnrMon", CStr(Me.CBUseAnrMon.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBCheckMobil", CStr(Me.CBCheckMobil.Checked))
        ' StoppUhr
        C_ini.Write(Dateipfad, "Optionen", "CBStoppUhrEinblenden", CStr(Me.CBStoppUhrEinblenden.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBStoppUhrAusblenden", CStr(Me.CBStoppUhrAusblenden.Checked))
        If Not Me.TBStoppUhr.Text = vbNullString Then
            If CInt(Me.TBStoppUhr.Text) < 0 Then
                Me.TBStoppUhr.Text = "10"
            End If
        Else
            Me.TBStoppUhr.Text = "10"
        End If
#If OVer < 14 Then
        C_ini.Write(Dateipfad, "Optionen", "CBSymbWwdh", CStr(Me.CBSymbWwdh.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbAnrMonNeuStart", CStr(Me.CBSymbAnrMonNeuStart.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbAnrMon", CStr(Me.CBSymbAnrMon.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbAnrListe", CStr(Me.CBSymbAnrListe.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbDirekt", CStr(Me.CBSymbDirekt.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbRWSuche", CStr(Me.CBSymbRWSuche.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbJournalimport", CStr(Me.CBSymbJournalimport.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBSymbVIP", CStr(Me.CBSymbVIP.Checked))
#End If

        C_ini.Write(Dateipfad, "Optionen", "TBStoppUhr", Me.TBStoppUhr.Text)
        'Telefonnummernformat
        If PrüfeMaske() Then
            C_ini.Write(Dateipfad, "Optionen", "TBTelNrMaske", Me.TBTelNrMaske.Text)
        End If
        C_ini.Write(Dateipfad, "Optionen", "CBTelNrGruppieren", CStr(Me.CBTelNrGruppieren.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBintl", CStr(Me.CBintl.Checked))
        C_ini.Write(Dateipfad, "Optionen", "CBIgnoTelNrFormat", CStr(Me.CBIgnoTelNrFormat.Checked))
        ' Telefone
#If OVer < 14 Then
        GUI.SetVisibleButtons()
#End If
        For i = 0 To TelList.Rows.Count - 1
            If CBool(TelList.Rows(i).Cells(0).Value) Then
                C_ini.Write(Dateipfad, "Telefone", "CBStandardTelefon", CStr(TelList.Rows(i).Cells(2).Value))
                Exit Function
            End If
        Next
        C_ini.Write(Dateipfad, "Telefone", "CBStandardTelefon", CStr(-1))
    End Function
#Region "Helfer"
    Private Function GetTimeInterval(ByVal nSeks As Int32) As String
        'http://www.vbarchiv.net/faq/date_sectotime.php
        Dim h As Int32, m As Int32
        h = nSeks \ 3600
        nSeks = nSeks Mod 3600
        m = nSeks \ 60
        nSeks = nSeks Mod 60
        Return Format(h, "00") & ":" & Format(m, "00") & ":" & Format(nSeks, "00")
    End Function

    Private Function AcceptOnlyNumeric(ByVal sTxt As String) As String
        If sTxt = String.Empty Then Return String.Empty
        If Mid(sTxt, Len(sTxt), 1) Like "[0-9]" = False Then ''''''''''''''''' prüfen, ob die Funktion auch richtig auf Nummern überprüft!!
            Return Mid(sTxt, 1, Len(sTxt) - 1)
        End If
        Return sTxt
    End Function

    Sub CLBtelnrAusfüllen()
        Dim iniTelefonEinträge() As String = C_ini.ReadSection(Dateipfad, "Telefone")
        Dim TelNrString As String = "Alle Telefonnummern"
        Dim TelEintrag() As String
        Dim CheckString(1) As String
        For Each Eintrag In iniTelefonEinträge
            TelEintrag = Split(Eintrag, "=", 2, CompareMethod.Text)
            If Not TelEintrag.Length = 1 Then
                Select Case True
                    Case TelEintrag(0).Contains("SIP") And Not TelEintrag(0).Contains("SIPID") _
                        Or TelEintrag(0).Contains("MSN") _
                        Or TelEintrag(0).Contains("POTS")
                        TelNrString += ";" & C_Helfer.OrtsVorwahlEntfernen(TelEintrag(1), Me.TBVorwahl.Text)
                    Case TelEintrag(0).Contains("CLBTelNr")
                        CheckString = Split(TelEintrag(1), ";", , CompareMethod.Text)
                End Select
            End If
        Next

        Dim Inhalt = From x In Split(TelNrString, ";", , CompareMethod.Text) Select x Distinct 'Doppelte entfernen
        Inhalt = (From x In Inhalt Where Not x Like "" Select x).ToArray ' Leere entfernem

        Me.CLBTelNr.Items.Clear()
        Dim alle As Boolean = True

        For Each TelNr In Inhalt
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
#End Region

#Region "Button Link"
    Private Sub ButtonZuruecksetzen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonZuruecksetzen.Click
        ' Startwerte zurücksetzen
        ' Einstellungen für das Wählmakro zurücksetzen
        Me.TBLandesVW.Text = "0049"
        Me.TBAmt.Text = ""
        'Me.TBFBAdr.Text = "fritz.box"
        'Me.TBPasswort.Text = ""
        'Me.TBVorwahl.Text = ""
        Me.CBCheckMobil.Checked = True
        'Me.CBAutoUpdate.Checked = False
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
        ' optionale allgemeine Einstellungen zurücksetzen
        Me.CBVoIPBuster.Checked = False
        Me.CBDialPort.Checked = False
        Me.CBCallByCall.Checked = False
        Me.CBCbCunterbinden.Checked = False
        Me.CBRückwärtssuche.Checked = False
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
    End Sub

    Private Sub ButtonTelefonliste_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonTelefonliste.Click

        tmpCheckString = C_ini.Read(Dateipfad, "Telefone", "CLBTelNr", "-1")
        Me.ButtonTelefonliste.Enabled = False
        Me.ButtonTelefonliste.Text = "Bitte warten..."
        Windows.Forms.Application.DoEvents()
        Speichern()
        Try
            BWTelefone = New BackgroundWorker

            With BWTelefone
                .RunWorkerAsync()
            End With
            ' Telefon-Daten in die .ini schreiben
        Catch ex As Exception
            C_Helfer.FBDB_MsgBox("Das Einlesen der Telefone war nicht erfolgreich!" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Optionen, ButtonTelefonListe")
        End Try
    End Sub

    Private Sub ButtonBereinigung_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonBereinigung.Click
        ' Wartung ini-Datei bereinigen
        Dim SchließZeit As String

        If C_Helfer.FBDB_MsgBox("Sind Sie sicher, das Sie die ausgewählten Bereiche aus der Einstellungsdatei löschen wollen?", MsgBoxStyle.YesNo, "ButtonBereinigung") = MsgBoxResult.Yes Then
            If Me.CBWJournal.Checked Then C_ini.Write(Dateipfad, "Journal", vbNullString, "")
            If Me.CBWOptionen.Checked Then
                SchließZeit = C_ini.Read(Dateipfad, "Journal", "SchließZeit", CStr(System.DateTime.Now))
                C_ini.Write(Dateipfad, "Optionen", vbNullString, "")
                C_ini.Write(Dateipfad, "Optionen", "SchließZeit", SchließZeit)
            End If
            If Me.CBWWwdh.Checked Then C_ini.Write(C_Helfer.Dateipfade(Dateipfad, "Listen"), "Wwdh", vbNullString, "")
            If Me.CBWRR.Checked Then C_ini.Write(C_Helfer.Dateipfade(Dateipfad, "Listen"), "AnrListe", vbNullString, "")
            If Me.CBWTelefone.Checked Then
                C_ini.Write(Dateipfad, "Telefone", vbNullString, "")
                C_Helfer.FBDB_MsgBox("Die Telefondaten wurden aus der ini-Datei gelöscht. Bitte lesen Sie diese wieder ein", MsgBoxStyle.Information, "ButtonBereinigung")
            End If

            If Me.CBWStatistik.Checked Then C_ini.Write(Dateipfad, "Statistik", vbNullString, "")
            If Me.CBWletzterAnrufer.Checked Then C_ini.Write(C_Helfer.Dateipfade(Dateipfad, "Listen"), "letzterAnrufer", vbNullString, "")
        End If

    End Sub

    Private Sub ButtonReset_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonReset.Click
        If C_Helfer.FBDB_MsgBox("Sind Sie sicher, dass Sie die Statistik unwiederruflich löschen wollen?", MsgBoxStyle.YesNo, "ButtonReset") = MsgBoxResult.Yes Then
            C_ini.Write(Dateipfad, "Statistik", vbNullString, "")
            C_ini.Write(Dateipfad, "Statistik", "ResetZeit", CStr(System.DateTime.Now))
            Statistik()
        End If
    End Sub

    Private Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonOK.Click
        Dim formschließen As Boolean = Speichern()
        ThisAddIn.UseAnrMon = Me.CBUseAnrMon.Checked
#If OVer >= 14 Then
        GUI.InvalidateControlAnrMon()
#End If
        If formschließen Then
            Dispose(True)
        End If
    End Sub

    Private Sub ButtonAbbruch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonAbbruch.Click
        ' Schließt das Fenster
        Dispose(True)
    End Sub

    Private Sub ButtonÜbernehmen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonÜbernehmen.Click
        Speichern()
    End Sub

    Private Sub BINIImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BINIImport.Click
        'Dim DateiPfad As String = String.Empty
        Dim fDialg As New System.Windows.Forms.OpenFileDialog
        fDialg.Filter = "ini-Dateien (*.ini)| *.ini"
        fDialg.Multiselect = False
        fDialg.Title = "Fritz!Box Ini-Datei auswählen"
        fDialg.FilterIndex = 1
        fDialg.RestoreDirectory = True
        If fDialg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dateipfad = fDialg.FileName
            If Not Len(Dateipfad) = 0 Then
                If Not C_ini.Read(Dateipfad, "Optionen", "TBVorwahl", "-1") = "-1" Or _
                        C_ini.Read(Dateipfad, "Optionen", "TBFBAdr", "-1") = "-1" Then
                    SaveSetting("FritzBox", "Optionen", "TBini", Dateipfad)
                    Ausfüllen()
                    C_Helfer.FBDB_MsgBox("Nach dem Import der Einstellungsdatei wird ein Neustart von Outlook empfohlen.", MsgBoxStyle.Information, "BINIImport")
                Else
                    C_Helfer.FBDB_MsgBox("Ungültige Einstellungsdatei! Der Import wird sicherheitshalber abgebrochen.", MsgBoxStyle.Critical, "BINIImport")
                End If
            End If
        End If
        fDialg = Nothing
    End Sub

    Private Sub ButtonINI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonINI.Click
        System.Diagnostics.Process.Start(Dateipfad)
    End Sub

    Private Sub ButtonIndexDateiöffnen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonIndexDateiöffnen.Click
        System.Diagnostics.Process.Start(C_Helfer.Dateipfade(Dateipfad, "KontaktIndex"))
    End Sub

    Private Sub ButtonLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLog.Click
        System.Diagnostics.Process.Start(C_Helfer.Dateipfade(Dateipfad, "LogDatei"))
    End Sub

    Private Sub ButtonListen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonListen.Click
        System.Diagnostics.Process.Start(C_Helfer.Dateipfade(Dateipfad, "Listen"))
    End Sub

    Private Sub Link_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkHomepage.LinkClicked, LinkForum.LinkClicked, LinkEmail.LinkClicked

        If sender Is Me.LinkEmail Then
            Me.Close()
            System.Diagnostics.Process.Start("mailto:kruemelino@gert-michael.de")
        ElseIf sender Is Me.LinkForum Then
            System.Diagnostics.Process.Start("http://www.ip-phone-forum.de/showthread.php?t=237086")
        ElseIf sender Is Me.LinkHomepage Then
            System.Diagnostics.Process.Start("http://github.com/Kruemelino/FritzBoxTelefon-dingsbums")
        End If

    End Sub

    Private Sub ButtonTesten_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonTesten.Click
        Speichern()
        Dim ID As Integer = CInt(C_ini.Read(C_Helfer.Dateipfade(Dateipfad, "Listen"), "letzterAnrufer", "Letzter", CStr(0)))
        Dim forman As New formAnrMon(Dateipfad, ID, False, C_ini, C_Helfer, AnrMon, OlI)
    End Sub

    Private Sub BZwischenablage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BZwischenablage.Click
        My.Computer.Clipboard.SetText(Me.TBDiagnose.Text)
    End Sub
#End Region

#Region "Änderungen"
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CBTelefonDatei.CheckedChanged
        Me.PTelefonDatei.Enabled = Me.CBTelefonDatei.Checked
        If Not Me.CBTelefonDatei.Checked Then
            Me.TBTelefonDatei.Text = vbNullString
        End If
    End Sub

    Private Sub CBRückwärtssuche_Change(ByVal sender As Object, ByVal e As EventArgs) Handles CBRückwärtssuche.CheckedChanged
        ' Combobox für Rückwärtssuchmaschinen je nach CheckBox für Rückwärtssuche ein- bzw. ausblenden
        Me.ComboBoxRWS.Enabled = Me.CBRückwärtssuche.Checked
        Me.CBKErstellen.Checked = Me.CBRückwärtssuche.Checked
        Me.CBKErstellen.Enabled = Me.CBRückwärtssuche.Checked
        Me.CBRWSIndex.Enabled = Me.CBRückwärtssuche.Checked
        Me.CBRWSIndex.Checked = Me.CBRückwärtssuche.Checked
    End Sub

    Private Sub CBCbCunterbinden_Change(ByVal sender As Object, ByVal e As EventArgs)
        Me.CBCallByCall.Enabled = Not Me.CBCbCunterbinden.Checked
        If Me.CBCbCunterbinden.Checked Then Me.CBCallByCall.Checked = False
    End Sub

    Private Sub TBLandesVW_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles TBLandesVW.Leave
        If Me.TBLandesVW.Text = "0049" Then
            Me.CBRückwärtssuche.Enabled = True

            Me.CBKErstellen.Enabled = True
            Me.ComboBoxRWS.Enabled = Me.CBRückwärtssuche.Checked
        Else
            Me.CBRückwärtssuche.Checked = False
            Me.CBRückwärtssuche.Enabled = False

            Me.CBKErstellen.Enabled = False
            Me.CBKErstellen.Checked = False
            Me.ComboBoxRWS.Enabled = False
        End If
    End Sub

    Private Sub TBVorwahl_Change(ByVal sender As Object, ByVal e As EventArgs) Handles TBVorwahl.TextChanged
        AcceptOnlyNumeric(TBVorwahl.Text)
    End Sub

    Private Sub TBEnblDauer_Change(ByVal sender As Object, ByVal e As EventArgs) Handles TBEnblDauer.TextChanged
        AcceptOnlyNumeric(TBEnblDauer.Text)
    End Sub

    Private Sub TBAnrMonX_Change(ByVal sender As Object, ByVal e As EventArgs) Handles TBAnrMonX.TextChanged
        AcceptOnlyNumeric(TBAnrMonX.Text)
    End Sub

    Private Sub TBAnrMonY_Change(ByVal sender As Object, ByVal e As EventArgs) Handles TBAnrMonY.TextChanged
        AcceptOnlyNumeric(TBAnrMonY.Text)
    End Sub

    Private Sub CBAutoClose_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBAutoClose.CheckedChanged
        Me.TBEnblDauer.Enabled = Me.CBAutoClose.Checked
        Me.Label15.Enabled = Me.CBAutoClose.Checked
    End Sub

    Private Sub CBWKomplett_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBWKomplett.CheckedChanged
        Me.CBWJournal.Checked = Me.CBWKomplett.Checked
        Me.CBWOptionen.Checked = Me.CBWKomplett.Checked
        Me.CBWWwdh.Checked = Me.CBWKomplett.Checked
        Me.CBWRR.Checked = Me.CBWKomplett.Checked
        Me.CBWStatistik.Checked = Me.CBWKomplett.Checked
        Me.CBWletzterAnrufer.Checked = Me.CBWKomplett.Checked
        Me.CBWTelefone.Checked = Me.CBWKomplett.Checked
    End Sub

    Private Sub CBJournal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBJournal.CheckedChanged

        If Not Me.CBJournal.Checked Then Me.CBJImport.Checked = False
        Me.CBJImport.Enabled = Me.CBJournal.Checked
#If OVer < 14 Then
            If Not Me.CBJournal.Checked Then Me.CBSymbJournalimport.Checked = False
            Me.CBSymbJournalimport.Enabled = Me.CBJournal.Checked
#End If
    End Sub

    Private Sub CBIndexAus_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBIndexAus.CheckedChanged
        Me.ButtonIndizierungStart.Enabled = Not Me.CBIndexAus.Checked
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
    End Sub

    Private Sub CBUseAnrMon_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBUseAnrMon.CheckedChanged
        Me.PanelAnrMon.Enabled = Me.CBUseAnrMon.Checked
        Me.CBIndexAus.Enabled = Not Me.CBUseAnrMon.Checked
        Me.GroupBoxStoppUhr.Enabled = Me.CBUseAnrMon.Checked

        If Not Me.CBUseAnrMon.Checked Then
            Me.CBStoppUhrEinblenden.Checked = False
            Me.CBStoppUhrAusblenden.Checked = False
        End If

    End Sub

    Private Sub TelList_CellMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles TelList.CellMouseUp
        If TypeOf TelList.CurrentCell Is Windows.Forms.DataGridViewCheckBoxCell Then
            TelList.EndEdit()
            If Not TelList.CurrentCell.Value Is Nothing Then
                Dim cellVal As Boolean = DirectCast(TelList.CurrentCell.Value, Boolean)
                If cellVal Then
                    If Not TelList.CurrentCell Is TelList.Rows(TelList.Rows.Count - 1).Cells(0) Then
                        For i = 0 To TelList.Rows.Count - 1
                            TelList.Rows(i).Cells(0).Value = False
                        Next
                        If Not TelList.Rows(TelList.CurrentCell.RowIndex).Cells(3).Value Is "AB" Then TelList.CurrentCell.Value = cellVal
                    Else
                        TelList.CurrentCell.Value = False
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub TBLandesVW_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TBLandesVW.TextChanged
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBVoIPBuster, "Mit dieser Einstellung wird die Landesvorwahl " & Me.TBLandesVW.Text & " immer mitgewählt.")
    End Sub

    Private Sub CBStoppUhrEinblenden_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBStoppUhrEinblenden.CheckedChanged
        Me.CBStoppUhrAusblenden.Enabled = Me.CBStoppUhrEinblenden.Checked
        If Not Me.CBStoppUhrEinblenden.Checked Then Me.CBStoppUhrAusblenden.Checked = False
        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked
        Me.LabelStoppUhr.Enabled = Me.CBStoppUhrEinblenden.Checked
    End Sub

    Private Sub CBStoppUhrAusblenden_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBStoppUhrAusblenden.CheckedChanged
        Me.TBStoppUhr.Enabled = Me.CBStoppUhrAusblenden.Checked And Me.CBStoppUhrEinblenden.Checked
    End Sub

    Private Sub TBTelNrMaske_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles TBTelNrMaske.Leave
        PrüfeMaske()
    End Sub
#End Region

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

    Function NSN(ByVal Nebenstelle As String, ByVal MSN As String) As Integer
        NSN = -1
        Dim Nebenstellen As String()
        Dim Telname As String()
        Nebenstellen = Split("1,2,3,5,51,52,53,54,55,56,57,58,50,60,61,62,63,64,65,66,67,68,69,20,21,22,23,24,25,26,27,28,29", ",", , CompareMethod.Text) 'AB nicht durchsuchen 600,601,602,603,604

        For Each NebenstellenNr In Nebenstellen
            Telname = Split(C_ini.Read(Dateipfad, "Telefone", CStr(NebenstellenNr), "-1;;"), ";", , CompareMethod.Text)
            If Not Nebenstelle = vbNullString Then
                If Telname(2) = Nebenstelle Then NSN = CInt(NebenstellenNr)
            Else
                If Telname(1) = MSN Then NSN = CInt(NebenstellenNr)
            End If
            If Not NSN = -1 Then
                Select Case NSN
                    Case 0 To 3
                        NSN -= 1
                    Case 60 To 69 'DECT
                        NSN -= 50
                End Select
            End If
        Next
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "Telefone"

    Private Sub BProbleme_Click(sender As Object, e As EventArgs) Handles BProbleme.Click
        Dim T As New Thread(AddressOf NewMail)
        T.Start()
        If C_Helfer.FBDB_MsgBox("Der Einstellungsdialog wird jetzt geschlossen. Danach werden alle erforderlichen Informationen gesammelt, was ein paar Sekunden dauern kann." & vbNewLine & _
                                        "Danach wird eine neue E-Mail geöffnet, die Sie bitte vervollständigen und absenden.", MsgBoxStyle.Information, "") = MsgBoxResult.Ok Then
            Me.Close()
        End If
    End Sub

    Private Sub NewMail()
        Dim NeueFW As Boolean
        Dim SID As String = FBox.DefaultSID
        Dim URL As String
        Dim FBOX_ADR As String = C_ini.Read(Dateipfad, "Optionen", "TBFBAdr", "fritz.box")

        Dim FBEncoding As System.Text.Encoding = System.Text.Encoding.UTF8
        Dim MailText As String
        Dim PfadTMPfile As String
        Dim tmpFileName As String
        Dim tmpFilePath As String
        Dim FBPasswort As String

        FBox = Nothing
        FBox = New FritzBox(Dateipfad, C_ini, C_Helfer, C_Crypt, False, Me)

        Do While SID = FBox.DefaultSID
            FBPasswort = InputBox("Geben Sie das Passwort zur Fritz!Box ein:")
            If Len(FBPasswort) = 0 Then
                If C_Helfer.FBDB_MsgBox("Haben Sie das Passwort vergessen?", MsgBoxStyle.YesNo, "NewMail") = vbYes Then
                    Exit Sub
                End If
            End If
            SID = FBox.FBLogin(NeueFW, FBPasswort)
        Loop

        If NeueFW Then
            URL = "http://" & FBOX_ADR & "/fon_num/fon_num_list.lua?sid=" & SID
        Else
            URL = "http://" & FBOX_ADR & "/cgi-bin/webcm?getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices&sid=" & SID
        End If
        MailText = C_Helfer.httpRead(URL, FBEncoding)

        With My.Computer.FileSystem
            PfadTMPfile = .GetTempFileName()
            tmpFilePath = .GetFileInfo(PfadTMPfile).DirectoryName
            tmpFileName = Split(.GetFileInfo(PfadTMPfile).Name, ".", , CompareMethod.Text)(0) & "_Telefoniegeräte.htm"
            .RenameFile(PfadTMPfile, tmpFileName)
            PfadTMPfile = .GetFiles(tmpFilePath, FileIO.SearchOption.SearchTopLevelOnly, "*_Telefoniegeräte.htm")(0).ToString
            .WriteAllText(PfadTMPfile, MailText, False)
        End With
        OlI.NeuEmail(PfadTMPfile, C_Helfer.GetInformationSystemFritzBox)
    End Sub

    Private Sub BWTelefone_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWTelefone.DoWork
        AddLine("Einlesen der Telefone gestartet.")
        FBox.FritzBoxDaten()
    End Sub

    Private Sub BWTelefone_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWTelefone.RunWorkerCompleted
        C_ini.Write(Dateipfad, "Telefone", "CLBTelNr", tmpCheckString)
        AddLine("BackgroundWorker ist fertig.")
        CLBtelnrAusfüllen()
        SetStatistik()
        Statistik()
        DelBTelefonliste()
        C_Helfer.FBDB_MsgBox("Das erneute Einlesen der Telefone ist abgeschlossen.", MsgBoxStyle.Information, "ButtonTelefonliste")
        BWTelefone = Nothing
        AddLine("BackgroundWorker wurde eliminiert.")
    End Sub

    Sub DelBTelefonliste()
        If Me.InvokeRequired Then
            Dim D As New DelgButtonTelEinl(AddressOf DelBTelefonliste)
            Me.Invoke(D)
        Else
            Me.ButtonTelefonliste.Text = "Telefone erneut einlesen"
            Me.ButtonTelefonliste.Enabled = True
        End If
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

    Public Function SetStatistik() As Boolean
        SetStatistik = False
        If Me.InvokeRequired Then
            Dim D As New DelgSetLine(AddressOf CLBtelnrAusfüllen)
            Invoke(D)
        Else
            CLBtelnrAusfüllen()
        End If
    End Function

    Private Sub setline()
        With Me.TBDiagnose
            .Text += StatusWert & vbCrLf
            .SelectionStart = .Text.Length
            .ScrollToCaret()
        End With
    End Sub

    Private Sub BStart2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BStart2.Click
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
        FBox = Nothing
        FBox = New FritzBox(Dateipfad, C_ini, C_Helfer, C_Crypt, False, Me)
        AddLine("Fritz!Box Klasse mit Verweis auf dieses Formular erstellt.")
        tmpCheckString = C_ini.Read(Dateipfad, "Telefone", "CLBTelNr", "-1")
        Try
            BWTelefone = New BackgroundWorker
            BWTelefone.WorkerReportsProgress = True
            AddLine("BackgroundWorker erstellt.")
            BWTelefone.RunWorkerAsync()
            AddLine("BackgroundWorker gestartet.")
        Catch ex As Exception
            AddLine("BackgroundWorker FEHLER: " & ex.Message)
        End Try
        Me.TBTelefonDatei.Enabled = True

    End Sub

    Private Sub BTelefonDatei_Click(sender As Object, e As EventArgs) Handles BTelefonDatei.Click
        Dim fDialg As New System.Windows.Forms.OpenFileDialog
        fDialg.Filter = "htm-Dateien (*.htm)| *.htm"
        fDialg.Multiselect = False
        fDialg.Title = "Fritz!Box Telefon-Datei auswählen"
        fDialg.FilterIndex = 1
        fDialg.RestoreDirectory = True
        If fDialg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If System.IO.File.Exists(fDialg.FileName) Then
                Me.TBTelefonDatei.Text = fDialg.FileName
            Else
                Me.TBTelefonDatei.Text = "Fehler!"
            End If
        End If
        fDialg = Nothing
    End Sub
#End Region

#Region "Kontaktindizierung"

    Sub StarteIndizierung()
        Startzeit = Date.Now
        BWIndexer = New BackgroundWorker
        Me.ProgressBarIndex.Value = 0
        Me.LabelAnzahl.Text = "Status: 0/" & CStr(Me.ProgressBarIndex.Maximum)
        Me.ButtonIndizierungAbbrechen.Enabled = True
        Me.ButtonIndizierungStart.Enabled = False
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
        olNamespace = OlI.GetOutlook.GetNamespace("MAPI")

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
                        KontaktName = " (" & aktKontakt.FullNameAndCompany & ")"
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
                        KontaktName = " (" & aktKontakt.FullNameAndCompany & ")"
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
        Me.ButtonIndizierungStart.Enabled = True
        Me.ButtonIndizierungAbbrechen.Enabled = False
    End Sub
    Private Sub SetProgressbarMax()
        Me.ProgressBarIndex.Maximum = Anzahl
    End Sub
#End Region
#Region "Backroundworker"
    Private Sub BWIndexer_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWIndexer.DoWork

        ErmittleKontaktanzahl()
        If Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
            StatusWert = Me.ProgressBarIndex.Maximum.ToString
            BWIndexer.ReportProgress(Me.ProgressBarIndex.Maximum)
        End If

        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder
        Dim LandesVW As String = Me.TBLandesVW.Text

        olNamespace = OlI.GetOutlook.GetNamespace("MAPI")

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
            C_ini.Write(Dateipfad, "Optionen", "LLetzteIndizierung", CStr(Date.Now))
            C_Helfer.LogFile("Indizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        ElseIf Me.RadioButtonEntfernen.Checked And Not Me.RadioButtonErstelle.Checked Then
            C_Helfer.LogFile("Deindizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        End If
    End Sub
#End Region
#Region "Button"
    Private Sub ButtonStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonIndizierungStart.Click
        StarteIndizierung()
    End Sub

    Private Sub ButtonIndizierungAbbrechen_Click(sender As Object, e As EventArgs) Handles ButtonIndizierungAbbrechen.Click
        BWIndexer.CancelAsync()
        Me.ButtonIndizierungAbbrechen.Enabled = False
        Me.ButtonIndizierungStart.Enabled = True
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


