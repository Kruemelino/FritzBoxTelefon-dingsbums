Imports System.Drawing
Imports System.Windows.Forms

Public Class Popup
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private WithEvents PopupWPF As AnrMonWPF
    Friend Property Eingeblendet As Boolean = False
    Friend Property TelFnt As Telefonat

#Region "Anrufmonitor"
    ''' <summary>
    ''' Startet das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="tmpTelefonat">Telefonat, das angezeigt wird</param>
    Friend Sub AnrMonEinblenden(ByVal tmpTelefonat As Telefonat)

        PopupWPF = New AnrMonWPF

        If ThisAddIn.OffenePopUps Is Nothing Then ThisAddIn.OffenePopUps = New List(Of Popup)

        'AnrMonausfüllen(PopUpAnrufMonitor, tmpTelefonat)
        TelFnt = tmpTelefonat

        KeepoInspActivated(False)

        'PopUpAnrufMonitor.Popup()
        PopupWPF.Popup(tmpTelefonat)
        Eingeblendet = True
        ThisAddIn.OffenePopUps.Add(Me)

        'AddHandler PopUpAnrufMonitor.Schließen, AddressOf PopUpAnrMon_Close
        AddHandler PopupWPF.Geschlossen, AddressOf PopupAnrMon_Closed

        'AddHandler PopUpAnrufMonitor.LinkClick, AddressOf AnrMonLink_Click
        'AddHandler PopUpAnrufMonitor.ToolStripMenuItemClicked, AddressOf AnrMonToolStripMenuItem_Clicked

        KeepoInspActivated(True)
    End Sub

    Friend Sub UpdateAnrMon(ByVal tmpTelefonat As Telefonat)
        PopupWPF?.Update(tmpTelefonat)
    End Sub

    Friend Sub AnrMonAusblenden()
        PopupWPF?.Close()
    End Sub

    'Private Sub PopUpAnrMon_Close(ByVal sender As Object, ByVal e As EventArgs)
    '    CType(sender, FormAnrMon).Close()
    'End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des PopupAnrMon aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' </summary>
    Private Sub PopupAnrMon_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles PopupWPF.Geschlossen
        NLogger.Debug("Anruffenster geschlossen: {0}", TelFnt.Anrufer)
        Eingeblendet = False
        ' Entferne den Anrufmonitor von der Liste der offenen Popups
        ThisAddIn.OffenePopUps.Remove(Me)

    End Sub

    Private Sub AnrMonLink_Click(ByVal sender As Object, ByVal e As EventArgs)
        If TelFnt IsNot Nothing Then
            TelFnt.ZeigeKontakt()
        End If
    End Sub

    Private Sub AnrMonToolStripMenuItem_Clicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs)

        If TelFnt IsNot Nothing Then
            Select Case e.ClickedItem.Name
                Case "ToolStripMenuItemKontaktöffnen"
                    TelFnt.ZeigeKontakt()

                Case "ToolStripMenuItemRückruf"
                    TelFnt.Rückruf()

                Case "ToolStripMenuItemKopieren"
                    My.Computer.Clipboard.SetText(TelFnt.GegenstelleTelNr.Formatiert)

            End Select
        End If
    End Sub


#End Region



End Class
