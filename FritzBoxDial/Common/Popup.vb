#Region "Imports"
Imports System.Timers
Imports System.IO.Path
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.ComponentModel
#End Region

Public Class Popup
    Implements IDisposable
#Region "Eigene Klassen"
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts
#End Region

#Region "BackgroundWorker"
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
    Private WithEvents BWStoppuhrEinblenden As BackgroundWorker
#End Region

#Region "Eigene Variablen für Anrufmonitor"
    Private V_PfadKontaktBild As String
    Private V_AnrmonClosed As Boolean
    Private UpdateForm As Boolean

    Private WithEvents TimerAktualisieren As System.Timers.Timer

    Private WithEvents PopUpAnrufMonitor As F_AnrMon
    Friend TelefonatsListe As New List(Of C_Telefonat)
    Friend AnrMonListe As New List(Of F_AnrMon)
#End Region

#Region "Eigene Properties für Anrufmonitor"
    Friend Property AnrmonClosed() As Boolean
        Get
            Return V_AnrmonClosed
        End Get
        Set(ByVal value As Boolean)
            V_AnrmonClosed = value
        End Set
    End Property

    Friend Property PfadKontaktBild() As String
        Get
            Return V_PfadKontaktBild
        End Get
        Set(ByVal value As String)
            V_PfadKontaktBild = value
        End Set
    End Property
#End Region

#Region "Eigene Variablen für Stoppuhr"
    Private WithEvents PopUpStoppUhr As F_StoppUhr
#End Region

    ' Track whether Dispose has been called.
    Private disposed As Boolean = False

    Friend Sub New(ByVal DataProviderKlasse As DataProvider, _
                     ByVal HelferKlasse As Helfer, _
                     ByVal OutlInter As OutlookInterface, _
                     ByVal KontaktFunktionen As Contacts)

        C_hf = HelferKlasse
        C_DP = DataProviderKlasse
        C_OLI = OutlInter
        C_KF = KontaktFunktionen
    End Sub

#Region "Anrufmonitor"

    ''' <summary>
    ''' Überträgt die Informationen aus dem Telefonat in das entsprechende PopUpFenster. 
    ''' </summary>
    ''' <param name="ThisPopUpAnrMon">PopUpFenster</param>
    ''' <param name="Telefonat">telefonat, das angezeigt werden soll.</param>
    ''' <remarks></remarks>
    Private Sub AnrMonausfüllen(ByVal ThisPopUpAnrMon As F_AnrMon, ByVal Telefonat As C_Telefonat)
        With ThisPopUpAnrMon
            With .OptionsMenu
                With .Items("ToolStripMenuItemRückruf")
                    .Text = C_DP.P_AnrMon_PopUp_ToolStripMenuItemRückruf
                    .Image = Global.FritzBoxDial.My.Resources.Bild2
                    .Enabled = Not Telefonat.TelNr = C_DP.P_Def_StringUnknown ' kein Rückruf
                End With
                With .Items("ToolStripMenuItemKopieren")
                    .Text = C_DP.P_AnrMon_PopUp_ToolStripMenuItemKopieren
                    .Image = Global.FritzBoxDial.My.Resources.Bild5
                    .Enabled = Not Telefonat.TelNr = C_DP.P_Def_StringUnknown ' in dem Fall sinnlos
                End With
                With .Items("ToolStripMenuItemKontaktöffnen")
                    .Text = CStr(IIf(Telefonat.TelNr = C_DP.P_Def_StringUnknown, _
                                C_DP.P_AnrMon_PopUp_ToolStripMenuItemKontaktErstellen, _
                                C_DP.P_AnrMon_PopUp_ToolStripMenuItemKontaktöffnen))
                    .Image = Global.FritzBoxDial.My.Resources.Bild4
                End With
            End With

            ' Uhrzeit des Telefonates eintragen
            .Uhrzeit = Telefonat.Zeit.ToString
            ' Telefonnamen eintragen
            .TelName = Telefonat.TelName & CStr(IIf(C_DP.P_CBShowMSN, " (" & Telefonat.MSN & ")", C_DP.P_Def_StringEmpty))

            ' Kontakt einblenden wenn in Outlook gefunden
            With Telefonat
                If .olContact Is Nothing Then
                    ''kontakt erstellen, wenn vcard vorhanden
                    'If Not .vCard = C_DP.P_Def_StringEmpty Then
                    '    .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    'End If
                Else
                    'Kontaktbild ermitteln
                    If C_DP.P_CBAnrMonContactImage Then
                        PfadKontaktBild = C_KF.KontaktBild(.olContact)
                        If Not PfadKontaktBild = C_DP.P_Def_StringEmpty Then
                            Using fs As New IO.FileStream(PfadKontaktBild, IO.FileMode.Open)
                                ThisPopUpAnrMon.Image = Image.FromStream(fs)
                            End Using

                            ' Seitenverhältnisse anpassen
                            ThisPopUpAnrMon.ImageSize = New Size(ThisPopUpAnrMon.ImageSize.Width, CInt((ThisPopUpAnrMon.ImageSize.Width * ThisPopUpAnrMon.Image.Size.Height) / ThisPopUpAnrMon.Image.Size.Width))
                        End If
                    End If
                End If
            End With

            If Telefonat.Anrufer = C_DP.P_Def_StringEmpty Then
                .TelNr = C_DP.P_Def_StringEmpty
                If Telefonat.TelNr = C_DP.P_Def_StringEmpty Then
                    .AnrName = C_DP.P_Def_StringUnknown
                Else
                    .AnrName = Telefonat.TelNr
                End If
            Else
                .TelNr = Telefonat.TelNr
                .AnrName = Telefonat.Anrufer
                If TimerAktualisieren IsNot Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
            End If

            .Firma = Telefonat.Companies
        End With
    End Sub

    ''' <summary>
    ''' Initiale Routine zum Einblenden eines Anrufmonitorfensters.
    ''' </summary>
    ''' <param name="Telefonat">Telefonalt, aus dem die Informationen gelesen werden sollen.</param>
    ''' <param name="Aktualisieren">Gibt an, ob ein Aktualisierungs-Timer gestartet werden soll. </param>
    ''' <remarks>Timer: Bei jedem Durchlauf wird geschaut, ob neuere Informationen im Telefonat enthalten sind.</remarks>
    Friend Overloads Sub AnrMonEinblenden(ByVal Telefonat As C_Telefonat, ByVal Aktualisieren As Boolean)
        Dim ThisPopUpAnrMon As New F_AnrMon
        Dim TelinList As Boolean = False

        UpdateForm = Aktualisieren

        Telefonat.PopupAnrMon = ThisPopUpAnrMon

        AnrMonausfüllen(ThisPopUpAnrMon, Telefonat)

        AnrmonClosed = False

        If Aktualisieren Then TimerAktualisieren = C_hf.SetTimer(500)

        C_OLI.KeepoInspActivated(False)

        AnrMonListe.Add(ThisPopUpAnrMon)

        With ThisPopUpAnrMon
            .ShowDelay = C_DP.P_TBEnblDauer * 1000
            .AutoAusblenden = C_DP.P_CBAutoClose
            .PositionsKorrektur = New Drawing.Size(C_DP.P_TBAnrMonX, C_DP.P_TBAnrMonY)
            .EffektMove = C_DP.P_CBAnrMonMove
            .EffektTransparenz = C_DP.P_CBAnrMonTransp
            .Startpunkt = CType(C_DP.P_CBoxAnrMonStartPosition, FritzBoxDial.F_AnrMon.eStartPosition)
            .MoveDirecktion = CType(C_DP.P_CBoxAnrMonMoveDirection, FritzBoxDial.F_AnrMon.eMoveDirection)
            .EffektMoveGeschwindigkeit = 44 - C_DP.P_TBAnrMonMoveGeschwindigkeit * 4
            .Popup()
        End With

        AddHandler ThisPopUpAnrMon.Close, AddressOf PopUpAnrMon_Close
        AddHandler ThisPopUpAnrMon.Closed, AddressOf PopupAnrMon_Closed
        AddHandler ThisPopUpAnrMon.LinkClick, AddressOf ToolStripMenuItemKontaktöffnen_Click
        AddHandler ThisPopUpAnrMon.ToolStripMenuItemClicked, AddressOf ToolStripMenuItem_Clicked

        C_OLI.KeepoInspActivated(True)

    End Sub

    ''' <summary>
    ''' Startet den BackgroundWorker für das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="Telefonat">Telefonat, das angezeigt wird</param>
    ''' <remarks></remarks>
    Friend Overloads Sub AnrMonEinblenden(ByVal Telefonat As C_Telefonat)
        BWAnrMonEinblenden = New BackgroundWorker
        With BWAnrMonEinblenden
            .WorkerSupportsCancellation = False
            .WorkerReportsProgress = False
            .RunWorkerAsync(argument:=Telefonat)
        End With
    End Sub

    ''' <summary>
    ''' Abarbeitung des BackgroundWorkers für das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim Telefonat As C_Telefonat = CType(e.Argument, C_Telefonat)
        AnrMonEinblenden(Telefonat, True)
        Do
            Windows.Forms.Application.DoEvents()
        Loop Until Telefonat.PopupAnrMon Is Nothing
    End Sub

    ''' <summary>
    ''' Gibt BackgroundWorkers frei. (Dispose)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BWAnrMonEinblenden_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWAnrMonEinblenden.RunWorkerCompleted
        BWAnrMonEinblenden.Dispose()
        BWAnrMonEinblenden = Nothing
    End Sub

    Private Sub TimerAktualisieren_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerAktualisieren.Elapsed
        For Each tmpTelefonat As C_Telefonat In TelefonatsListe
            If tmpTelefonat.PopupAnrMon IsNot Nothing Then
                AnrMonausfüllen(tmpTelefonat.PopupAnrMon, tmpTelefonat)
            End If
        Next
    End Sub

    Private Sub PopUpAnrMon_Close(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles PopUpAnrufMonitor.Close
        CType(sender, F_AnrMon).Hide()
    End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des PopupAnrMon aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PopupAnrMon_Closed(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Telefonat As C_Telefonat = TelefonatsListe.Find(Function(JE) JE.PopupAnrMon Is CType(sender, F_AnrMon))

        AnrMonListe.Remove(CType(sender, F_AnrMon))
        AnrmonClosed = True

        If Not PfadKontaktBild = C_DP.P_Def_StringEmpty AndAlso System.IO.File.Exists(PfadKontaktBild) Then C_KF.DelKontaktBild(PfadKontaktBild)

        If TimerAktualisieren IsNot Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)

        ' Prüfen ob Anrufmonitor in der Telefonliste vorhanden ist
        ' Ja: Dort Löschen
        ' Nein: Nichts unternehmen
        If Telefonat IsNot Nothing Then Telefonat.PopupAnrMon = Nothing
        'Try
        '    TelefonatsListe.Find(Function(JE) JE.PopupAnrMon Is CType(sender, F_AnrMon)).PopupAnrMon = Nothing
        'Catch : End Try
    End Sub

    Private Sub ToolStripMenuItem_Clicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)

        Dim tmpPopUpAnrMon As F_AnrMon = CType(sender, F_AnrMon)
        Dim tmpTelefonat As C_Telefonat = TelefonatsListe.Find(Function(JE) JE.PopupAnrMon Is tmpPopUpAnrMon)
        ' Todo: Möglichkeit finden, wie auf das Telefonat, welches zu dem PopUp gehört, zugreifen

        If tmpTelefonat IsNot Nothing Then

            Select Case e.ClickedItem.Name

                Case "ToolStripMenuItemKontaktöffnen" 'tmpPopUpAnrMon.OptionsMenu.Items("ToolStripMenuItemKontaktöffnen").Name
                    AnruferAnzeigen(tmpTelefonat)
                Case "ToolStripMenuItemRückruf" 'tmpPopUpAnrMon.OptionsMenu.Items("ToolStripMenuItemRückruf").Name
                    ' Ruft den Kontakt zurück
                    ThisAddIn.P_WClient.Rueckruf(tmpTelefonat)
                Case "ToolStripMenuItemKopieren" 'tmpPopUpAnrMon.OptionsMenu.Items("ToolStripMenuItemKopieren").Name
                    With tmpPopUpAnrMon
                        My.Computer.Clipboard.SetText(.AnrName & CStr(IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")")))
                    End With
            End Select

        End If
    End Sub

    Private Sub ToolStripMenuItemKontaktöffnen_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        AnruferAnzeigen(TelefonatsListe.Find(Function(JE) JE.PopupAnrMon Is CType(sender, F_AnrMon)))
    End Sub

    ''' <summary>
    ''' Blendet den Kontakteintrag des Anrufers ein.
    ''' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt
    ''' </summary>
    ''' <param name="tmpTelefonat">Telefonat, das angezeigt wird</param>
    ''' <remarks></remarks>
    Private Sub AnruferAnzeigen(ByVal tmpTelefonat As C_Telefonat)

        With tmpTelefonat
            If Not .KontaktID = C_DP.P_Def_ErrorMinusOne_String And Not .StoreID = C_DP.P_Def_ErrorMinusOne_String Then
                .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
            End If
            If .olContact IsNot Nothing Then
                .olContact.Display()
            Else
                C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False).Display()
            End If
        End With
    End Sub

#End Region

#Region "Stoppuhr"

    Friend Sub StoppuhrEinblenden(ByVal Telefonat As C_Telefonat)
        BWStoppuhrEinblenden = New BackgroundWorker
        With BWStoppuhrEinblenden
            .WorkerSupportsCancellation = True
            .WorkerReportsProgress = False
            .RunWorkerAsync(argument:=Telefonat)
        End With
    End Sub

    ''' <summary>
    ''' Blendet das Formular der StoppUhr ein
    ''' </summary>
    ''' <param name="Anrufer">Name bzw. Telefonnummer des Anrufers oder Angerufenen</param>
    ''' <param name="ZeitStart">Zeitpunkt des Telefonatstartes</param>
    ''' <param name="sRichtung">Eingehendes oder Ausgehendes Telefonat</param>
    ''' <param name="WarteZeit">Wartezeit, nach dem Telefonat bis die Stoppuhr automatisch ausgeblendet wird.</param>
    ''' <param name="PositionStart">Bildschirmposition</param>
    ''' <param name="sMSN">Eigene MSN</param>
    ''' <remarks></remarks>
    Private Function ErzeugePopUpStoppuhr(ByVal Anrufer As String, _
                             ByVal ZeitStart As String, _
                             ByVal sRichtung As String, _
                             ByVal WarteZeit As Integer, _
                             ByVal PositionStart As System.Drawing.Point, _
                             ByVal sMSN As String) As F_StoppUhr

        ErzeugePopUpStoppuhr = New F_StoppUhr
        With ErzeugePopUpStoppuhr
            .Anruf = Anrufer
            .StartZeit = ZeitStart
            .WarteZeit = WarteZeit
            .StartPosition = PositionStart
            .StoppuhrStart()
            .Richtung = sRichtung
            .Popup()
            .MSN = sMSN
        End With
        AddHandler ErzeugePopUpStoppuhr.Close, AddressOf PopUpStoppuhr_Close

    End Function

    Private Sub BWStoppuhrEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWStoppuhrEinblenden.DoWork
        Dim Telefonat As C_Telefonat = CType(e.Argument, C_Telefonat)
        Dim WarteZeit As Integer
        Dim Richtung As String
        Dim AnrName As String
        Dim StartZeit As String
        Dim Beendet As Boolean = False
        Dim Abbruch As Boolean
        Dim StartPosition As System.Drawing.Point
        Dim ScreensX As Integer = 0
        Dim ScreensY As Integer = 0

        If C_DP.P_CBStoppUhrAusblenden Then
            WarteZeit = C_DP.P_TBStoppUhr
        Else
            WarteZeit = -1
        End If

        StartPosition = New System.Drawing.Point(C_DP.P_CBStoppUhrX, C_DP.P_CBStoppUhrY)
        For Each Bildschirm In Windows.Forms.Screen.AllScreens
            ScreensX += Bildschirm.Bounds.Size.Width
            ScreensY += Bildschirm.Bounds.Size.Height
        Next
        With StartPosition
            If .X > ScreensX Or .Y > ScreensY Then
                .X = CInt((Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 100) / 2)
                .Y = CInt((Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 50) / 2)
            End If
        End With

        Richtung = "Anruf " & CStr(IIf(Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend, "von", "zu")) & ":"
        AnrName = CStr(IIf(Telefonat.Anrufer = C_DP.P_Def_StringEmpty, Telefonat.TelNr, Telefonat.Anrufer))
        StartZeit = String.Format("{0:00}:{1:00}:{2:00}", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second)
        Abbruch = False

        Telefonat.PopupStoppuhr = ErzeugePopUpStoppuhr(AnrName, StartZeit, Richtung, WarteZeit, StartPosition, Telefonat.MSN)
        C_hf.LogFile(C_DP.P_AnrMon_Log_StoppUhrStart1(AnrName)) '"Stoppuhr gestartet - ID: " & ID & ", Anruf: " & .Anruf)
        BWStoppuhrEinblenden.WorkerSupportsCancellation = True

        Do
            Windows.Forms.Application.DoEvents()
        Loop Until Telefonat.PopupStoppuhr Is Nothing

    End Sub

    ''' <summary>
    ''' Blendet die StoppUhr aus.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopUpStoppuhr_Close(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim thisPopupStoppuhr As F_StoppUhr = CType(sender, F_StoppUhr)

        C_DP.P_CBStoppUhrX = thisPopupStoppuhr.StartPosition.X
        C_DP.P_CBStoppUhrY = thisPopupStoppuhr.StartPosition.Y
        Try
            TelefonatsListe.Find(Function(JE) JE.PopupStoppuhr Is thisPopupStoppuhr).PopupStoppuhr = Nothing
        Catch : End Try
        thisPopupStoppuhr = Nothing
    End Sub

    Private Sub BWStoppuhrEinblenden_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWStoppuhrEinblenden.RunWorkerCompleted
        BWStoppuhrEinblenden.Dispose()
    End Sub
#End Region

#Region "Dispose"
    ' Implement IDisposable.
    ' Do not make this method virtual.
    ' A derived class should not be able to override this method.
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        ' This object will be cleaned up by the Dispose method.
        ' Therefore, you should call GC.SupressFinalize to
        ' take this object off the finalization queue 
        ' and prevent finalization code for this object
        ' from executing a second time.
        GC.SuppressFinalize(Me)
    End Sub

    ' Dispose(bool disposing) executes in two distinct scenarios.
    ' If disposing equals true, the method has been called directly
    ' or indirectly by a user's code. Managed and unmanaged resources
    ' can be disposed.
    ' If disposing equals false, the method has been called by the 
    ' runtime from inside the finalizer and you should not reference 
    ' other objects. Only unmanaged resources can be disposed.
    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
        ' Check to see if Dispose has already been called.
        If Not Me.disposed Then
            ' If disposing equals true, dispose all managed 
            ' and unmanaged resources.
            If disposing Then
                ' Dispose managed resources.
                'ToolStripMenuItemKontaktöffnen.Dispose()
                'ToolStripMenuItemRückruf.Dispose()
                'ToolStripMenuItemKopieren.Dispose()
                'AnrMonContextMenuStrip.Dispose()
                'CompContainer.Dispose()
                'TimerAktualisieren.Dispose()
                'PopUpAnrMon.Dispose()
                'PopUpStoppUhr.Dispose()
            End If

            ' Call the appropriate methods to clean up 
            ' unmanaged resources here.
            ' If disposing is false, 
            ' only the following code is executed.
            'CloseHandle(handle)
            'handle = IntPtr.Zero

            ' Note disposing has been done.
            disposed = True

        End If
    End Sub
#End Region


End Class
