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
    Private C_KF As Contacts
#End Region

#Region "BackgroundWorker"
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
#End Region

#Region "Eigene Variablen für Anrufmonitor"
    Private V_PfadKontaktBild As String
    Private V_AnrmonClosed As Boolean
    Private UpdateForm As Boolean

    Private WithEvents PopUpAnrufMonitor As F_AnrMon
    Friend TelefonatsListe As New List(Of C_Telefonat)
    Friend AnrMonListe As New List(Of F_AnrMon)
    Friend StoppuhrListe As New List(Of F_StoppUhr)
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
                    'unterdrückte Nummer
                    .AnrName = C_DP.P_Def_StringUnknown
                Else
                    'unbekannte, aber nicht unterdrückte Nummer
                    .AnrName = Telefonat.TelNr
                End If
            Else
                .TelNr = Telefonat.TelNr
                .AnrName = Telefonat.Anrufer
            End If

            .Firma = Telefonat.Companies
        End With
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

    Friend Sub UpdateAnrMon(ByVal tmpTelefonat As C_Telefonat)
        AnrMonausfüllen(tmpTelefonat.PopupAnrMon, tmpTelefonat)
    End Sub

    ''' <summary>
    ''' Abarbeitung des BackgroundWorkers für das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim Telefonat As C_Telefonat = CType(e.Argument, C_Telefonat)
        Dim RemoveTelFromList As Boolean = False
        Dim ThisPopUpAnrMon As New F_AnrMon
        Dim TelinList As Boolean = False

        'UpdateForm = Aktualisieren

        ' Überprüfe ob Anrufmonitor für dieses Telefonat bereits angezeigt wird
        If Telefonat.PopupAnrMon Is Nothing Then
            Telefonat.PopupAnrMon = ThisPopUpAnrMon

            If Not TelefonatsListe.Exists(Function(fAM) fAM Is Telefonat) Then
                TelefonatsListe.Add(Telefonat)
                RemoveTelFromList = True
            End If

            AnrMonausfüllen(ThisPopUpAnrMon, Telefonat)

            AnrmonClosed = False

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

            Do
                ' Steuerung der Einblendgeschwindigkeit mit der Wartezeit des Threads
                '10: Schnell 
                '20: Normal
                '30: Langsam
                'C_DP.P_TBAnrMonMoveGeschwindigkeit
                C_hf.ThreadSleep(20 + -1 * C_DP.P_TBAnrMonMoveGeschwindigkeit)
                Telefonat.PopupAnrMon.tmAnimation_Tick()
                Windows.Forms.Application.DoEvents()
            Loop Until Telefonat.PopupAnrMon Is Nothing Or Not AnrMonListe.Exists(Function(AM) AM Is Telefonat.PopupAnrMon)

            If RemoveTelFromList Then TelefonatsListe.Remove(Telefonat)
            C_hf.LogFile("BWAnrMonEinblenden.DoWork: Schleife verlassen")

        End If
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

        If tmpTelefonat IsNot Nothing Then

            Select Case e.ClickedItem.Name

                Case "ToolStripMenuItemKontaktöffnen" 'tmpPopUpAnrMon.OptionsMenu.Items("ToolStripMenuItemKontaktöffnen").Name
                    AnruferAnzeigen(tmpTelefonat)
                Case "ToolStripMenuItemRückruf" 'tmpPopUpAnrMon.OptionsMenu.Items("ToolStripMenuItemRückruf").Name
                    ' Ruft den Kontakt zurück
                    ThisAddIn.P_WClient.Rueckruf(tmpTelefonat)
                Case "ToolStripMenuItemKopieren" 'tmpPopUpAnrMon.OptionsMenu.Items("ToolStripMenuItemKopieren").Name
                    Dim thrd As New Threading.Thread(AddressOf ClipboardSetText)
                    thrd.SetApartmentState(Threading.ApartmentState.STA)
                    With tmpPopUpAnrMon
                        thrd.Start(.AnrName & CStr(IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")")))
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
    ''' <remarks></remarks>
    Private Sub AnruferAnzeigen(ByVal tmpTelefonat As C_Telefonat)

        With tmpTelefonat
            If Not .KontaktID = C_DP.P_Def_ErrorMinusOne_String And Not .StoreID = C_DP.P_Def_ErrorMinusOne_String Then
                .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
            End If
            If .olContact IsNot Nothing Then
                Try
                    .olContact.Display()
                Catch ex As System.Runtime.InteropServices.COMException
                    C_hf.FBDB_MsgBox(C_DP.P_Fehler_Kontakt_Anzeigen(ex.Message), MsgBoxStyle.Critical, "AnruferAnzeigen")
                End Try
            Else
                C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False).Display()
            End If
        End With
    End Sub

#End Region

#Region "Stoppuhr"

    Friend Sub StoppuhrEinblenden(ByVal Telefonat As C_Telefonat)
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
        C_hf.LogFile(C_DP.P_AnrMon_Log_StoppUhrStart1(AnrName))

        AddHandler thisPopupStoppuhr.Close, AddressOf PopUpStoppuhr_Close
    End Sub

    ''' <summary>
    ''' Blendet die StoppUhr aus.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopUpStoppuhr_Close(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim thisPopupStoppuhr As F_StoppUhr = CType(sender, F_StoppUhr)

        C_DP.P_CBStoppUhrX = thisPopupStoppuhr.StartPosition.X
        C_DP.P_CBStoppUhrY = thisPopupStoppuhr.StartPosition.Y
        StoppuhrListe.Remove(thisPopupStoppuhr)
        If TelefonatsListe.Count > 0 Then
            Try
                TelefonatsListe.Find(Function(JE) JE.PopupStoppuhr Is thisPopupStoppuhr).PopupStoppuhr = Nothing
            Catch : End Try
        End If
        thisPopupStoppuhr = Nothing
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
