Imports System.Timers
Imports System.IO.Path
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Windows.Forms

Public Class Popup
    Implements IDisposable

    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts

    Private PopUpAnrMonList As New List(Of PopUpAnrMon)
    'Private C_AnrMon As AnrufMonitor
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

    Private CompContainer As New System.ComponentModel.Container()
    Private WithEvents AnrMonContextMenuStrip As New ContextMenuStrip(CompContainer)
    Private ToolStripMenuItemKontaktöffnen As New ToolStripMenuItem()
    Private ToolStripMenuItemRückruf As New ToolStripMenuItem()
    Private ToolStripMenuItemKopieren As New ToolStripMenuItem()
    Private WithEvents TimerAktualisieren As System.Timers.Timer
    Private WithEvents PopUpAnrufMonitor As PopUpAnrMon

    Public Property AnrmonClosed() As Boolean
        Get
            Return V_AnrmonClosed
        End Get
        Set(ByVal value As Boolean)
            V_AnrmonClosed = value
        End Set
    End Property

    Public Property PfadKontaktBild() As String
        Get
            Return V_PfadKontaktBild
        End Get
        Set(ByVal value As String)
            V_PfadKontaktBild = value
        End Set
    End Property

    Private UpdateForm As Boolean
    Private V_PfadKontaktBild As String
    Private V_AnrmonClosed As Boolean

    Private Sub AnrMonInitializeComponent(ByVal ThisPopUpAnrMon As PopUpAnrMon)
        '
        'ContextMenuStrip
        '
        Me.AnrMonContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemKontaktöffnen, Me.ToolStripMenuItemRückruf, Me.ToolStripMenuItemKopieren})
        Me.AnrMonContextMenuStrip.Name = "AnrMonContextMenuStrip"
        Me.AnrMonContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.AnrMonContextMenuStrip.Size = New System.Drawing.Size(222, 70)
        '
        'ToolStripMenuItemKontaktöffnen
        '
        Me.ToolStripMenuItemKontaktöffnen.Image = Global.FritzBoxDial.My.Resources.Bild4
        Me.ToolStripMenuItemKontaktöffnen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItemKontaktöffnen.Name = "ToolStripMenuItemKontaktöffnen"
        Me.ToolStripMenuItemKontaktöffnen.Size = New System.Drawing.Size(221, 22)
        Me.ToolStripMenuItemKontaktöffnen.Text = C_DP.P_AnrMon_PopUp_ToolStripMenuItemKontaktöffnen '"Kontakt öffnen"
        '
        'ToolStripMenuItemRückruf
        '
        Me.ToolStripMenuItemRückruf.Image = Global.FritzBoxDial.My.Resources.Bild2
        Me.ToolStripMenuItemRückruf.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItemRückruf.Name = "ToolStripMenuItemRückruf"
        Me.ToolStripMenuItemRückruf.Size = New System.Drawing.Size(221, 22)
        Me.ToolStripMenuItemRückruf.Text = C_DP.P_AnrMon_PopUp_ToolStripMenuItemRückruf '"Rückruf"
        '
        'ToolStripMenuItemKopieren
        '
        Me.ToolStripMenuItemKopieren.Image = Global.FritzBoxDial.My.Resources.Bild5
        Me.ToolStripMenuItemKopieren.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItemKopieren.Name = "ToolStripMenuItemKopieren"
        Me.ToolStripMenuItemKopieren.Size = New System.Drawing.Size(221, 22)
        Me.ToolStripMenuItemKopieren.Text = C_DP.P_AnrMon_PopUp_ToolStripMenuItemKopieren '"In Zwischenablage kopieren"
        '
        'PopUpAnrMon
        '
        With ThisPopUpAnrMon

            .AnrName = "Anrufername"
            .AutoAusblenden = False
            .BorderColor = System.Drawing.SystemColors.WindowText
            .ButtonHoverColor = System.Drawing.Color.Orange
            .ContentFont = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            .Firma = "Firmenname"
            .HeaderColor = System.Drawing.SystemColors.ControlDarkDark
            .Image = Nothing
            .ImagePosition = New System.Drawing.Point(12, 32)
            .ImageSize = New System.Drawing.Size(48, 48)
            .LinkHoverColor = System.Drawing.SystemColors.Highlight
            .OptionsButton = True
            .OptionsMenu = Me.AnrMonContextMenuStrip
            .PositionsKorrektur = New System.Drawing.Size(0, 0)
            .Size = New System.Drawing.Size(400, 100)
            .TelName = "Telefonname"
            .TelNr = "01156 +49 (0815) 0123456789"
            .TelNrFont = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            .TextPadding = New System.Windows.Forms.Padding(5)
            .TitleColor = System.Drawing.SystemColors.ControlText
            .TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            .Uhrzeit = "07.09.09 12:00:00"

        End With
    End Sub

    Private Sub AnrMonausfüllen(ByVal ThisPopUpAnrMon As PopUpAnrMon, ByVal Telefonat As C_Telefonat)
        With ThisPopUpAnrMon

            If Telefonat.TelNr = C_DP.P_Def_StringUnknown Then
                With .OptionsMenu
                    .Items("ToolStripMenuItemRückruf").Enabled = False ' kein Rückruf im Fall 1
                    .Items("ToolStripMenuItemKopieren").Enabled = False ' in dem Fall sinnlos
                    .Items("ToolStripMenuItemKontaktöffnen").Text = "Einen neuen Kontakt erstellen"
                End With
            End If
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
                If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
            End If

            .Firma = Telefonat.Companies
        End With
    End Sub

    Friend Sub AnrMonEinblenden(ByVal Aktualisieren As Boolean, ByVal Telefonat As C_Telefonat)
        Dim ThisPopUpAnrMon As New PopUpAnrMon

        AnrMonInitializeComponent(ThisPopUpAnrMon)

        UpdateForm = Aktualisieren

        PopUpAnrMonList.Add(ThisPopUpAnrMon)

        AnrMonausfüllen(ThisPopUpAnrMon, Telefonat)

        AnrmonClosed = False

        If UpdateForm Then
            TimerAktualisieren = C_hf.SetTimer(100)
            If TimerAktualisieren Is Nothing Then
                C_hf.LogFile("formAnrMon_New: TimerNeuStart nicht gestartet")
            End If
        End If

        C_OLI.KeepoInspActivated(False)

        With ThisPopUpAnrMon
            .ShowDelay = C_DP.P_TBEnblDauer * 1000
            .AutoAusblenden = C_DP.P_CBAutoClose
            .PositionsKorrektur = New Drawing.Size(C_DP.P_TBAnrMonX, C_DP.P_TBAnrMonY)
            .EffektMove = C_DP.P_CBAnrMonMove
            .EffektTransparenz = C_DP.P_CBAnrMonTransp
            .Startpunkt = CType(C_DP.P_CBoxAnrMonStartPosition, FritzBoxDial.PopUpAnrMon.eStartPosition) 'FritzBoxDial.PopUpAnrMon.eStartPosition.BottomRight
            .MoveDirecktion = CType(C_DP.P_CBoxAnrMonMoveDirection, FritzBoxDial.PopUpAnrMon.eMoveDirection) 'FritzBoxDial.PopUpAnrMon.eMoveDirection.X
            .EffektMoveGeschwindigkeit = 44 - C_DP.P_TBAnrMonMoveGeschwindigkeit * 4
            .Popup()
        End With

        AddHandler ThisPopUpAnrMon.Close, AddressOf PopUpAnrMon_Close
        AddHandler ThisPopUpAnrMon.Closed, AddressOf PopUpAnrMon_Closed
        AddHandler ThisPopUpAnrMon.LinkClick, AddressOf ToolStripMenuItemKontaktöffnen_Click
        AddHandler ThisPopUpAnrMon.ToolStripMenuItemClicked, AddressOf ToolStripMenuItem_Clicked

        C_OLI.KeepoInspActivated(True)

    End Sub

    Private Sub TimerAktualisieren_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerAktualisieren.Elapsed
        Dim VergleichString As String = PopUpAnrufMonitor.AnrName
        'AnrMonausfüllen()
        If Not VergleichString = PopUpAnrufMonitor.AnrName Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
    End Sub

    Private Sub PopUpAnrMon_Close(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles PopUpAnrufMonitor.Close
        CType(sender, PopUpAnrMon).Hide()
    End Sub

    Private Sub PopUpAnrMon_Closed(ByVal sender As Object, ByVal e As System.EventArgs)
        If (Not PfadKontaktBild = C_DP.P_Def_StringEmpty AndAlso System.IO.File.Exists(PfadKontaktBild)) Then
            C_KF.DelKontaktBild(PfadKontaktBild)
        End If

        AnrmonClosed = True
        If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
    End Sub

    Private Sub ToolStripMenuItem_Clicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
        ' Todo: Möglichkeit finden, wie auf das Telefonat, welches zu dem PopUp gehört, zugreifen
        Select Case e.ClickedItem.Name
            Case ToolStripMenuItemKontaktöffnen.Name
                ' blendet den Kontakteintrag des Anrufers ein
                ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt
            Case ToolStripMenuItemRückruf.Name
                ' Ruft den Kontakt zurück
            Case ToolStripMenuItemKopieren.Name
                '    With PopUpAnrufMonitor
                '        My.Computer.Clipboard.SetText(.AnrName & CStr(IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")")))
                '    End With
        End Select

        'ThisAddIn.P_WClient.Rueckruf(C_AnrMon.LetzterAnrufer)
    End Sub

    Private Sub ToolStripMenuItemKontaktöffnen_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles ToolStripMenuItemKontaktöffnen.Click, PopUpAnrufMonitor.LinkClick
        '    ' blendet den Kontakteintrag des Anrufers ein
        '    ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt
        '    'With C_AnrMon.LetzterAnrufer
        '    '    If Not .KontaktID = C_DP.P_Def_ErrorMinusOne_String And Not .StoreID = C_DP.P_Def_ErrorMinusOne_String Then
        '    '        .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
        '    '    End If
        '    '    If Not .olContact Is Nothing Then
        '    '        .olContact.Display()
        '    '    Else
        '    '        C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False).Display()
        '    '    End If
        '    'End With
    End Sub
#End Region

#Region "Stoppuhr"
    Private WithEvents PopUpStoppUhr As New FritzBoxDial.PopUpStoppUhr

    Private V_StUhrClosed As Boolean
    Private V_Position As System.Drawing.Point

    Friend Property StUhrClosed() As Boolean
        Get
            Return V_StUhrClosed
        End Get
        Set(ByVal value As Boolean)
            V_StUhrClosed = value
        End Set
    End Property

    Friend Property Position() As System.Drawing.Point
        Get
            Return V_Position
        End Get
        Set(ByVal value As System.Drawing.Point)
            V_Position = value
        End Set
    End Property

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
    Friend Sub ZeigeStoppUhr(ByVal Anrufer As String, _
                             ByVal ZeitStart As String, _
                             ByVal sRichtung As String, _
                             ByVal WarteZeit As Integer, _
                             ByVal PositionStart As System.Drawing.Point, _
                             ByVal sMSN As String)

        With PopUpStoppUhr
            .ContentFont = New Font("Segoe UI", 18)
            .TitleFont = New Font("Segoe UI", 9)
            .Size = New Size(250, 100)

            .Anruf = Anrufer
            .StartZeit = ZeitStart
            .WarteZeit = WarteZeit
            .StartPosition = PositionStart
            .StoppuhrStart()
            .Richtung = sRichtung
            .Popup()
            .MSN = sMSN
        End With

    End Sub

    ''' <summary>
    ''' Hält die StoppUhr an
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Stopp()
        PopUpStoppUhr.StoppuhrStopp()
    End Sub

    ''' <summary>
    ''' Blendet die StoppUr aus.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Stoppuhr_Close() Handles PopUpStoppUhr.Close
        Position = PopUpStoppUhr.StartPosition
        StUhrClosed = True
        Me.Finalize()
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
