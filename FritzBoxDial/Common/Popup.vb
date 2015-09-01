#Region "Imports"
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
    Private C_KF As KontaktFunktionen
    Private C_WClient As Wählclient
#End Region

#Region "BackgroundWorker"
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
    Private WithEvents BWStoppUhrEinblenden As BackgroundWorker
#End Region

#Region "Eigene Variablen für Anrufmonitor"
    Private _PfadKontaktBild As String
    Private UpdateForm As Boolean

    Private WithEvents PopUpAnrufMonitor As F_AnrMon
    Friend TelefonatsListe As New List(Of C_Telefonat)
    Friend AnrMonListe As New List(Of F_AnrMon)
    Friend StoppuhrListe As New List(Of F_StoppUhr)
#End Region

#Region "Eigene Properties für Anrufmonitor"
    Friend Property PfadKontaktBild() As String
        Get
            Return _PfadKontaktBild
        End Get
        Set(ByVal value As String)
            _PfadKontaktBild = value
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
                     ByVal KontaktFunktionen As KontaktFunktionen, _
                     ByVal WählClientKlasse As Wählclient)

        C_hf = HelferKlasse
        C_DP = DataProviderKlasse
        C_OLI = OutlInter
        C_KF = KontaktFunktionen
        C_WClient = WählClientKlasse
    End Sub

#Region "Anrufmonitor"

    ''' <summary>
    ''' Überträgt die Informationen aus dem Telefonat in das entsprechende PopUpFenster. 
    ''' </summary>
    ''' <param name="ThisPopUpAnrMon">PopUpFenster</param>
    ''' <param name="Telefonat">telefonat, das angezeigt werden soll.</param>
    Private Sub AnrMonausfüllen(ByVal ThisPopUpAnrMon As F_AnrMon, ByVal Telefonat As C_Telefonat)
        With ThisPopUpAnrMon
            With .OptionsMenu
                With .Items("ToolStripMenuItemRückruf")
                    .Text = DataProvider.P_AnrMon_PopUp_ToolStripMenuItemRückruf
                    .Image = Global.FritzBoxDial.My.Resources.IMG_Telefon
                    .Enabled = Not Telefonat.TelNr = DataProvider.P_Def_StringUnknown ' kein Rückruf
                End With
                With .Items("ToolStripMenuItemKopieren")
                    .Text = DataProvider.P_AnrMon_PopUp_ToolStripMenuItemKopieren
                    .Image = Global.FritzBoxDial.My.Resources.IMG_Copy
                    .Enabled = Not Telefonat.TelNr = DataProvider.P_Def_StringUnknown ' in dem Fall sinnlos
                End With
                With .Items("ToolStripMenuItemKontaktöffnen")
                    .Text = C_hf.IIf(Telefonat.TelNr = DataProvider.P_Def_StringUnknown, _
                                DataProvider.P_AnrMon_PopUp_ToolStripMenuItemKontaktErstellen, _
                                DataProvider.P_AnrMon_PopUp_ToolStripMenuItemKontaktöffnen)
                    .Image = Global.FritzBoxDial.My.Resources.IMG_Kontakt_Aktiv
                End With
            End With

            ' Uhrzeit des Telefonates eintragen
            .Uhrzeit = Telefonat.Zeit.ToString
            ' Telefonnamen eintragen
            .TelName = Telefonat.TelName & C_hf.IIf(C_DP.P_CBShowMSN, " (" & Telefonat.MSN & ")", DataProvider.P_Def_LeerString)

            ' Kontakt einblenden wenn in Outlook gefunden
            With Telefonat
                If .olContact Is Nothing Then
                    ''kontakt erstellen, wenn vcard vorhanden
                    'If Not .vCard = DataProvider.P_Def_StringEmpty Then
                    '    .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    'End If
                Else
                    'Kontaktbild ermitteln
                    If C_DP.P_CBAnrMonContactImage Then
                        PfadKontaktBild = C_KF.KontaktBild(.olContact)
                        If Not PfadKontaktBild = DataProvider.P_Def_LeerString Then
                            Using fs As New IO.FileStream(PfadKontaktBild, IO.FileMode.Open)
                                ThisPopUpAnrMon.Image = Image.FromStream(fs)
                            End Using

                            ' Seitenverhältnisse anpassen
                            ThisPopUpAnrMon.ImageSize = New Size(ThisPopUpAnrMon.ImageSize.Width, CInt((ThisPopUpAnrMon.ImageSize.Width * ThisPopUpAnrMon.Image.Size.Height) / ThisPopUpAnrMon.Image.Size.Width))
                        End If
                    End If
                End If
            End With

            If Telefonat.Anrufer = DataProvider.P_Def_LeerString Then
                .TelNr = DataProvider.P_Def_LeerString
                If Telefonat.TelNr = DataProvider.P_Def_LeerString Then
                    'unterdrückte Nummer
                    .AnrName = DataProvider.P_Def_StringUnknown
                Else
                    'unbekannte, aber nicht unterdrückte Nummer
                    .AnrName = Telefonat.TelNr
                End If
            Else
                .TelNr = Telefonat.TelNr
                .AnrName = Telefonat.Anrufer
            End If

            .Firma = Telefonat.Firma
        End With
    End Sub

    ''' <summary>
    ''' Startet den BackgroundWorker für das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="tmpTelefonat">Telefonat, das angezeigt wird</param>
    Friend Sub AnrMonEinblenden(ByVal tmpTelefonat As C_Telefonat)
        BWAnrMonEinblenden = New BackgroundWorker
        With BWAnrMonEinblenden
            .WorkerSupportsCancellation = False
            .WorkerReportsProgress = False
            .RunWorkerAsync(argument:=tmpTelefonat)
        End With
    End Sub

    Friend Sub UpdateAnrMon(ByVal tmpTelefonat As C_Telefonat)
        AnrMonausfüllen(tmpTelefonat.PopupAnrMon, tmpTelefonat)
    End Sub

    ''' <summary>
    ''' Abarbeitung des BackgroundWorkers für das Einblenden des Anrufmonitors
    ''' </summary>
    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim Telefonat As C_Telefonat = CType(e.Argument, C_Telefonat)
        Dim RemoveTelFromList As Boolean = False
        Dim ThisPopUpAnrMon As New F_AnrMon
        Dim TelinList As Boolean = False

        ' Überprüfe ob Anrufmonitor für dieses Telefonat bereits angezeigt wird
        If Telefonat.PopupAnrMon Is Nothing Then
            Telefonat.PopupAnrMon = ThisPopUpAnrMon

            If Not TelefonatsListe.Exists(Function(fAM) fAM Is Telefonat) Then
                TelefonatsListe.Add(Telefonat)
                RemoveTelFromList = True
            End If

            AnrMonausfüllen(ThisPopUpAnrMon, Telefonat)

            C_OLI.KeepoInspActivated(False)

            AnrMonListe.Add(ThisPopUpAnrMon)

            With ThisPopUpAnrMon
                .ShowDelay = C_DP.P_TBEnblDauer * 1000
                .AutoAusblenden = C_DP.P_CBAutoClose And Telefonat.AnrMonAusblenden
                .PositionsKorrektur = New Drawing.Size(C_DP.P_TBAnrMonX, C_DP.P_TBAnrMonY)
                .EffektMove = C_DP.P_CBAnrMonMove
                .EffektTransparenz = C_DP.P_CBAnrMonTransp
                .Startpunkt = CType(C_DP.P_CBoxAnrMonStartPosition, F_AnrMon.eStartPosition)
                .MoveDirection = CType(C_DP.P_CBoxAnrMonMoveDirection, F_AnrMon.eMoveDirection)
                .AnzAnrMon = AnrMonListe.Count - 1
                .Popup()
            End With

            AddHandler ThisPopUpAnrMon.Close, AddressOf PopUpAnrMon_Close
            AddHandler ThisPopUpAnrMon.Closed, AddressOf PopupAnrMon_Closed
            AddHandler ThisPopUpAnrMon.LinkClick, AddressOf ToolStripMenuItemKontaktöffnen_Click
            AddHandler ThisPopUpAnrMon.ToolStripMenuItemClicked, AddressOf ToolStripMenuItem_Clicked

            C_OLI.KeepoInspActivated(True)

            Do
                ' Steuerung der Einblendgeschwindigkeit mit der Wartezeit des Threads
                'C_hf.ThreadSleep(40 + -1 * C_DP.P_TBAnrMonMoveGeschwindigkeit)
                Telefonat.PopupAnrMon.tmAnimation_Tick()
                C_hf.ThreadSleep(5 + 10 - C_DP.P_TBAnrMonMoveGeschwindigkeit)
                Windows.Forms.Application.DoEvents()
            Loop Until Telefonat.PopupAnrMon Is Nothing Or Not AnrMonListe.Exists(Function(AM) AM Is Telefonat.PopupAnrMon)

            If RemoveTelFromList Then TelefonatsListe.Remove(Telefonat)
        End If
    End Sub

    ''' <summary>
    ''' Gibt BackgroundWorkers frei. (Dispose)
    ''' </summary>
    Private Sub BWAnrMonEinblenden_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWAnrMonEinblenden.RunWorkerCompleted
        BWAnrMonEinblenden.Dispose()
        BWAnrMonEinblenden = Nothing
    End Sub

    Private Sub PopUpAnrMon_Close(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles PopUpAnrufMonitor.Close
        CType(sender, F_AnrMon).Hide()
    End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des PopupAnrMon aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' </summary>
    Private Sub PopupAnrMon_Closed(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim tmpPopupAnrMon As F_AnrMon = CType(sender, F_AnrMon)
        Dim tmpTelefonat As C_Telefonat

        ' Entferne Anrufmonitor aus der Anrufmonitorliste
        AnrMonListe.Remove(tmpPopupAnrMon)

        If TelefonatsListe.Count > 0 Then
            tmpTelefonat = TelefonatsListe.Find(Function(JE) JE.PopupAnrMon Is tmpPopupAnrMon)
            If tmpTelefonat IsNot Nothing Then
                ' Entferne Anrufmonitor aus dem dazugehörigen Telefonat
                With tmpTelefonat
                    .PopupAnrMon = Nothing
                    ' Wenn das Telefonat beendet wurde und keine Stoppuhr eingeblendet ist
                    If .Beendet And .PopupStoppuhr Is Nothing Then
                        ' dann entferne Telefonat aus Liste
                        C_hf.LogFile("PopupAnrMon_Closed: Telefonat " & .ID & ":" & .TelNr & " aus der Liste entfernt.")
                        TelefonatsListe.Remove(tmpTelefonat)
                    End If
                End With
            End If
        End If
        If Not PfadKontaktBild = DataProvider.P_Def_LeerString AndAlso System.IO.File.Exists(PfadKontaktBild) Then C_KF.DelKontaktBild(PfadKontaktBild)

        tmpPopupAnrMon = Nothing
        tmpTelefonat = Nothing
    End Sub

    Private Sub ToolStripMenuItem_Clicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)

        Dim tmpPopUpAnrMon As F_AnrMon = CType(sender, F_AnrMon)
        Dim tmpTelefonat As C_Telefonat = TelefonatsListe.Find(Function(JE) JE.PopupAnrMon Is tmpPopUpAnrMon)

        If tmpTelefonat IsNot Nothing Then

            Select Case e.ClickedItem.Name

                Case "ToolStripMenuItemKontaktöffnen"
                    AnruferAnzeigen(tmpTelefonat)
                Case "ToolStripMenuItemRückruf"
                    ' Ruft den Kontakt zurück
                    C_WClient.Rueckruf(tmpTelefonat)
                Case "ToolStripMenuItemKopieren"
                    Dim thrd As New Threading.Thread(AddressOf ClipboardSetText)
                    thrd.SetApartmentState(Threading.ApartmentState.STA)
                    With tmpPopUpAnrMon
                        thrd.Start(.AnrName & C_hf.IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")"))
                    End With
            End Select

        End If
    End Sub

    Private Sub ClipboardSetText(ByVal Text As Object)
        If Threading.Thread.CurrentThread.GetApartmentState = Threading.ApartmentState.STA Then
            Clipboard.SetText(CStr(Text))
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
    Private Sub AnruferAnzeigen(ByVal tmpTelefonat As C_Telefonat)

        With tmpTelefonat
            If Not .KontaktID = DataProvider.P_Def_ErrorMinusOne_String And Not .StoreID = DataProvider.P_Def_ErrorMinusOne_String Then
                .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
            End If
            If .olContact IsNot Nothing Then
                Try
                    .olContact.Display()
                Catch ex As System.Runtime.InteropServices.COMException
                    C_hf.MsgBox(DataProvider.P_Fehler_Kontakt_Anzeigen(ex.Message), MsgBoxStyle.Critical, "AnruferAnzeigen")
                End Try
            Else
                C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False).Display()
            End If
        End With
    End Sub

#End Region

#Region "Stoppuhr"
    ''' <summary>
    ''' Startet den Hintergrundprozess, der die Stoppuhr einblendet.
    ''' </summary>
    ''' <param name="Telefonat"></param>
    Friend Sub StoppuhrEinblenden(ByVal Telefonat As C_Telefonat)
        BWStoppUhrEinblenden = New BackgroundWorker
        With BWStoppUhrEinblenden
            .WorkerSupportsCancellation = False
            .WorkerReportsProgress = False
            .RunWorkerAsync(argument:=Telefonat)
        End With
    End Sub

    ''' <summary>
    ''' Hintergrundprozess der Stoppuhr
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub BWStoppUhrEinblenden_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWStoppUhrEinblenden.DoWork
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
        Dim thisPopupStoppuhr As New F_StoppUhr

        If C_DP.P_CBStoppUhrAusblenden Then
            WarteZeit = C_DP.P_TBStoppUhr
        Else
            WarteZeit = DataProvider.P_Def_ErrorMinusOne_Integer
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

        Richtung = "Anruf " & C_hf.IIf(Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend, "von", "zu") & ":"
        AnrName = C_hf.IIf(Telefonat.Anrufer = DataProvider.P_Def_LeerString, Telefonat.TelNr, Telefonat.Anrufer)
        StartZeit = String.Format("{0:00}:{1:00}:{2:00}", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second)
        Abbruch = False

        StoppuhrListe.Add(thisPopupStoppuhr)
        With thisPopupStoppuhr
            .Anruf = AnrName
            .StartZeit = StartZeit
            .WarteZeit = WarteZeit
            .StartPosition = StartPosition
            .StoppuhrStart()
            .Richtung = Richtung
            .Popup()
            .MSN = Telefonat.MSN
        End With

        Telefonat.PopupStoppuhr = thisPopupStoppuhr
        C_hf.LogFile(DataProvider.P_AnrMon_Log_StoppUhrStart1(AnrName))

        AddHandler thisPopupStoppuhr.Close, AddressOf PopUpStoppuhr_Close

        Do
            Windows.Forms.Application.DoEvents()
            C_hf.ThreadSleep(40)
        Loop Until Telefonat.PopupStoppuhr Is Nothing Or Not StoppuhrListe.Exists(Function(SU) SU Is Telefonat.PopupStoppuhr)

    End Sub

    ''' <summary>
    ''' Gibt BackgroundWorkers frei. (Dispose)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub BWStoppUhrEinblenden_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWStoppUhrEinblenden.RunWorkerCompleted
        BWStoppUhrEinblenden.Dispose()
        BWStoppUhrEinblenden = Nothing
    End Sub

    ''' <summary>
    ''' Blendet die StoppUhr aus.
    ''' </summary>
    Private Sub PopUpStoppuhr_Close(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim tmpPopupStoppuhr As F_StoppUhr = CType(sender, F_StoppUhr)
        Dim tmpTelefonat As C_Telefonat

        With tmpPopupStoppuhr.StartPosition
            C_DP.P_CBStoppUhrX = .X
            C_DP.P_CBStoppUhrY = .Y
        End With

        ' Entferne Stoppuhr aus der Stoppuhrliste
        StoppuhrListe.Remove(tmpPopupStoppuhr)

        If TelefonatsListe.Count > 0 Then
            tmpTelefonat = TelefonatsListe.Find(Function(JE) JE.PopupStoppuhr Is tmpPopupStoppuhr)
            If tmpTelefonat IsNot Nothing Then
                ' Entferne Anrufmonitor aus dem dazugehörigen Telefonat
                With tmpTelefonat
                    .PopupStoppuhr = Nothing
                    ' Wenn das Telefonat beendet wurde und kein Anrufmonitor eingeblendet ist
                    If .Beendet And .PopupAnrMon Is Nothing Then
                        ' dann entferne Telefonat aus Liste
                        C_hf.LogFile("PopUpStoppuhr_Close: Telefonat " & .ID & ":" & .TelNr & " aus der Liste entfernt.")
                        TelefonatsListe.Remove(tmpTelefonat)
                    End If
                End With
            End If
        End If

        tmpPopupStoppuhr = Nothing
        tmpTelefonat = Nothing
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
