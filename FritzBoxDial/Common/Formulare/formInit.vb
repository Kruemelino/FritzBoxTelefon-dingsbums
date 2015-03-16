﻿Imports System.Timers

Friend Class formInit
    Implements IDisposable

    ' Klassen
    Private C_DP As FritzBoxDial.DataProvider
    Private C_HF As FritzBoxDial.Helfer
    Private C_Crypt As FritzBoxDial.MyRijndael
    Private C_GUI As FritzBoxDial.GraphicalUserInterface
    Private C_OlI As FritzBoxDial.OutlookInterface
    Private C_AnrMon As FritzBoxDial.AnrufMonitor
    Private C_FBox As FritzBoxDial.FritzBox
    Private C_KF As FritzBoxDial.KontaktFunktionen
    Private C_RWS As FritzBoxDial.formRWSuche
    Private C_WählClient As FritzBoxDial.Wählclient
    Private C_Phoner As FritzBoxDial.PhonerInterface
    Private C_Config As FritzBoxDial.formCfg
    Private F_AnrListImport As FritzBoxDial.formImportAnrList
    Private C_PopUp As FritzBoxDial.Popup
    Private C_XML As FritzBoxDial.XML
    ' Strings
    Private SID As String
    ' Integer
    Private StandbyCounter As Integer
    ' Timer
    Private WithEvents TimerReStart As Timer
    ' Boolean
    Private ReStartError As Boolean

    Public Sub New(ByRef GUIKlasse As GraphicalUserInterface, _
                   ByRef KFKlasse As KontaktFunktionen, _
                   ByRef HFKlasse As Helfer, _
                   ByRef DPKlasse As DataProvider, _
                   ByRef AnrMonKlasse As AnrufMonitor, _
                   ByRef XMLKlasse As XML, _
                   ByRef FritzBoxKlasse As FritzBox)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        ' Klasse zum IO-der XML-Struktur erstellen
        C_XML = New XML()

        ' Klasse zum Bereitstellen von Daten erstellen
        C_DP = New DataProvider(C_XML)

        ' Klasse für Verschlüsselung erstellen
        C_Crypt = New MyRijndael(C_DP)

        ' Klasse für Helferfunktionen erstellen
        C_HF = New Helfer(C_DP, C_Crypt, C_XML)
        C_HF.LogFile(DataProvider.P_Def_Addin_LangName & " V" & ThisAddIn.Version & " gestartet.")

        ' Klasse für die Kontakte generieren
        C_KF = New KontaktFunktionen(C_DP, C_HF)

        ' Klasse für die Rückwärtssuche generieren
        C_RWS = New formRWSuche(C_HF, C_KF, C_DP, C_XML)

        ' Klasse für die OutlookInterface generieren
        C_OlI = New OutlookInterface(C_KF, C_HF, C_DP)

        ' Klasse für das PhonerInterface generieren
        C_Phoner = New PhonerInterface(C_HF, C_DP, C_Crypt)

        If PrüfeAddin() Then

            ' Klasse für die Interaktionen mit der Fritz!Box generieren
            ' Wenn PrüfeAddin mit Dialog (Usereingaben) abgeschlossen wurde, exsistiert C_FBox schon 
            If C_FBox Is Nothing Then C_FBox = New FritzBox(C_DP, C_HF, C_Crypt, C_XML)

            ' Klasse für das GraphicalUserInterface (GUI) generieren
            C_GUI = New GraphicalUserInterface(C_HF, C_DP, C_RWS, C_KF, C_PopUp, C_XML)

            ' Klasse für den AnrufMonitor generieren
            C_AnrMon = New AnrufMonitor(C_DP, C_RWS, C_HF, C_KF, C_GUI, C_OlI, C_PopUp, C_XML)

            ' Klasse für den Wählclient generieren
            C_WählClient = New Wählclient(C_DP, C_HF, C_KF, C_GUI, C_OlI, C_FBox, C_AnrMon, C_Phoner, C_XML)

            ' Klasse für das GraphicalUserInterface (GUI) generieren
            C_Config = New formCfg(C_GUI, C_DP, C_HF, C_Crypt, C_AnrMon, C_FBox, C_OlI, C_KF, C_Phoner, C_PopUp, C_XML)

            ' Klasse für das Popup-Fenster generieren
            C_PopUp = New Popup(C_DP, C_HF, C_OlI, C_KF, C_WählClient)

            ' Verschiedene Funktionen an die GraphicalUserInterface-Klasse übergeben
            With C_GUI
                .P_AnrufMonitor = C_AnrMon
                .P_OlInterface = C_OlI
                .P_CallClient = C_WählClient
                .P_FritzBox = C_FBox
                .P_PopUp = C_PopUp
                .P_Config = C_Config
            End With

            ' Verschiedene Funktionen an den AnrufMonitor-Klasse übergeben
            With C_AnrMon
                .P_PopUp = C_PopUp
            End With

            If C_DP.P_CBAutoAnrList And C_DP.P_CBUseAnrMon Then
                F_AnrListImport = New formImportAnrList(C_FBox, C_AnrMon, C_HF, C_DP, C_XML, False)
            End If

            ' Ab hier nur noch Debug-Code
            If DataProvider.P_Debug_AnrufSimulation Then
                F_AnrListImport = New formImportAnrList(C_FBox, C_AnrMon, C_HF, C_DP, C_XML, True)
            End If
        End If

        FritzBoxKlasse = C_FBox
        GUIKlasse = C_GUI
        KFKlasse = C_KF
        HFKlasse = C_HF
        DPKlasse = C_DP
        AnrMonKlasse = C_AnrMon
        XMLKlasse = C_XML
    End Sub

    Function PrüfeAddin() As Boolean
        Dim Rückgabe As Boolean = False

        If C_DP.P_TBPasswort = DataProvider.P_Def_StringEmpty Or _
            C_DP.P_TBVorwahl = DataProvider.P_Def_StringEmpty Or _
            C_DP.GetSettingsVBA("Zugang", DataProvider.P_Def_ErrorMinusOne_String) = DataProvider.P_Def_ErrorMinusOne_String Then

            Rückgabe = False
            Me.ShowDialog()
            Rückgabe = True
        Else
            Rückgabe = True
        End If
        Return Rückgabe

    End Function
#Region "Timer"

    Public Sub StandByReStart()

        If C_DP.P_CBAutoAnrList Or C_DP.P_CBAnrMonAuto Then
            If TimerReStart Is Nothing Then
                StandbyCounter = 1
                TimerReStart = C_HF.SetTimer(DataProvider.P_Def_ReStartIntervall)
            End If
        End If
    End Sub


    Private Sub TimerReStartStandBy_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerReStart.Elapsed
        If StandbyCounter < DataProvider.P_Def_TryMaxRestart Then
            If C_DP.P_CBForceFBAddr Then
                C_HF.httpGET("http://" & C_DP.P_TBFBAdr, C_HF.GetEncoding(C_DP.P_EncodeingFritzBox), ReStartError)
            Else
                ReStartError = Not C_HF.Ping(C_DP.P_TBFBAdr)
            End If

            If ReStartError Then
                ' Fehler! Verbindung zur Fritz!Box konnte nach Standby nicht wieder aufgebaut werden. Weitere Versuche werden folgen.
                C_HF.LogFile(DataProvider.P_AnrMon_Log_AnrMonTimer1)
                StandbyCounter += 1
            Else
                ' Erfolg! Verbindung zur Fritz!Box konnte nach Standby  wieder aufgebaut werden.
                C_HF.LogFile(DataProvider.P_AnrMon_Log_AnrMonTimer2)

                ' Beende Timer
                TimerReStart = C_HF.KillTimer(TimerReStart)

                ' Starte Anrufmonitor
                If C_DP.P_CBAnrMonAuto And C_DP.P_CBUseAnrMon Then
                    C_AnrMon.AnrMonStartStopp()
                End If

                ' Auswertung der Anrufliste anstoßen
                If C_DP.P_CBAutoAnrList Then
                    F_AnrListImport = New formImportAnrList(C_FBox, C_AnrMon, C_HF, C_DP, C_XML, False)
                End If

            End If
        Else
            ' Fehler! Verbindung zur Fritz!Box konnte nach final Standby nicht wieder aufgebaut werden.
            C_HF.LogFile(DataProvider.P_AnrMon_Log_AnrMonTimer3)
            TimerReStart = C_HF.KillTimer(TimerReStart)
        End If
    End Sub
#End Region


#Region "Formularfunktionen"
    Private Sub BFBAdr_Click(sender As Object, e As EventArgs) Handles BFBAdr.Click
        Dim FBIPAdresse As String = Me.TBFritzBoxAdr.Text
        If C_HF.Ping(FBIPAdresse) Or Me.CBForceFBAddr.Checked Then
            Me.TBFritzBoxAdr.Text = FBIPAdresse
            If Not InStr(C_HF.httpGET("http://" & FBIPAdresse & "/login_sid.lua", System.Text.Encoding.UTF8, Nothing), "<SID>" & DataProvider.P_Def_SessionID & "</SID>", CompareMethod.Text) = 0 Then
                Me.LMessage.Text = DataProvider.P_Init_FritzBox_Found(FBIPAdresse)
                C_DP.P_TBFBAdr = FBIPAdresse
                C_DP.P_CBForceFBAddr = Me.CBForceFBAddr.Checked
                Me.TBFBPW.Enabled = True
                Me.TBFBUser.Enabled = True
                Me.LabelFBUser.Enabled = True
                Me.LFBPW.Enabled = True
                Me.TBFritzBoxAdr.Enabled = False
                Me.BFBAdr.Enabled = False
                Me.LFBAdr.Enabled = False
                Me.CBForceFBAddr.Enabled = False
            Else
                Me.LMessage.Text = DataProvider.P_Init_FritzBox_NotFound
            End If
        Else
            Me.CBForceFBAddr.Enabled = True
            Me.TBFritzBoxAdr.Text = DataProvider.P_Def_FritzBoxIPAdress
            FBIPAdresse = Me.TBFritzBoxAdr.Text
            Me.LMessage.Text = DataProvider.P_Init_NotthingFound
        End If
    End Sub

    Private Sub BFBPW_Click(sender As Object, e As EventArgs) Handles BFBPW.Click
        Dim fw550 As Boolean
        C_FBox = New FritzBox(C_DP, C_HF, C_Crypt, C_XML)
        C_DP.P_TBBenutzer = Me.TBFBUser.Text
        C_DP.P_TBPasswort = C_Crypt.EncryptString128Bit(Me.TBFBPW.Text, DataProvider.P_Def_PassWordDecryptionKey)
        C_DP.SaveSettingsVBA("Zugang", DataProvider.P_Def_PassWordDecryptionKey)
        C_HF.KeyChange()
        SID = C_FBox.FBLogin(fw550)
        If Not SID = DataProvider.P_Def_SessionID Then
            Me.TBFBPW.Enabled = False
            Me.LFBPW.Enabled = False
            Me.BFBPW.Enabled = False
            Me.TBFBUser.Enabled = False
            Me.LabelFBUser.Enabled = False
            Me.LVorwahl.Enabled = True
            Me.LLandesvorwahl.Enabled = True
            Me.TBVorwahl.Enabled = True
            Me.TBLandesvorwahl.Enabled = True
            Me.LMessage.Text = DataProvider.P_Init_Login_Korrekt
        Else
            Me.LMessage.Text = DataProvider.P_Init_Login_Nicht_Korrekt
        End If
    End Sub

    Private Sub TBFBPW_TextChanged(sender As Object, e As EventArgs) Handles TBFBPW.TextChanged
        Me.BFBPW.Enabled = Not Me.TBFBPW.Text.Length = 0
    End Sub

    Private Sub BTelEinlesen_Click(sender As Object, e As EventArgs) Handles BTelEinlesen.Click
        Me.LVorwahl.Enabled = False
        Me.LLandesvorwahl.Enabled = False
        Me.TBVorwahl.Enabled = False
        Me.TBLandesvorwahl.Enabled = False
        Me.BTelEinlesen.Text = DataProvider.P_Def_Bitte_Warten
        Me.BTelEinlesen.Enabled = False

        C_DP.P_TBVorwahl = Me.TBVorwahl.Text
        C_DP.P_TBLandesVW = Me.TBLandesvorwahl.Text
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

            Dim TelNrString() As String = Split("Alle Telefonnummern;" & C_XML.Read(C_DP.XMLDoc, xPathTeile, ""), ";", , CompareMethod.Text)
            TelNrString = (From x In TelNrString Select x Distinct).ToArray 'Doppelte entfernen
            TelNrString = (From x In TelNrString Where Not x Like DataProvider.P_Def_StringEmpty Select x).ToArray ' Leere entfernen
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
        Dim tmpTeile As String = DataProvider.P_Def_StringEmpty
        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")

            For i = 1 To Me.CLBTelNr.Items.Count - 1
                tmpTeile += ". = " & """" & Me.CLBTelNr.Items(i).ToString & """" & " or "
            Next
            tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
            .Add("[" & tmpTeile & "]")
            C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "Checked", "0")
            tmpTeile = DataProvider.P_Def_StringEmpty
            For i = 0 To CheckTelNr.Count - 1
                tmpTeile += ". = " & """" & CheckTelNr.Item(i).ToString & """" & " or "
            Next
            tmpTeile = Strings.Left(tmpTeile, Len(tmpTeile) - Len(" or "))
            .Item(.Count - 1) = "[" & tmpTeile & "]"
            C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "Checked", "1")
        End With

        Me.LMessage.Text = "Fertig"
        Me.BSchließen.Enabled = True
    End Sub

    Private Sub BSchließen_Click(sender As Object, e As EventArgs) Handles BSchließen.Click
        Me.Close()
    End Sub
#End Region
End Class