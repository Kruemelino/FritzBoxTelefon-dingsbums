Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class Popup
    Implements IDisposable

#Region "BackgroundWorker"
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
#End Region

#Region "Properties"
    Friend Property PfadKontaktBild As String
    Friend Property UpdateForm As Boolean
    Private WithEvents PopUpAnrufMonitor As FormAnrMon
#End Region

#Region "Anrufmonitor"

    Private Sub AnrMonausfüllen(ByVal ThisPopUpAnrMon As FormAnrMon, ByVal TelFt As Telefonat)
        With ThisPopUpAnrMon
            If TelFt IsNot Nothing Then
                ' Uhrzeit des Telefonates eintragen
                .Uhrzeit = TelFt.ZeitBeginn

                ' Telefonnamen eintragen

                If TelFt.RINGGeräte Is Nothing Then
                    ' Ermitteln der Gerätenammen der Telefone, die auf diese eigene Nummer reagieren
                    TelFt.RINGGeräte = XMLData.PTelefonie.Telefoniegeräte.FindAll(Function(Tel) Tel.StrEinTelNr.Contains(TelFt.OutEigeneTelNr))
                End If

                For Each TelGerät As Telefoniegerät In TelFt.RINGGeräte
                    .TelName = String.Format("{0}, {1}", .TelName, TelGerät.Name)
                Next

                If TelFt.NrUnterdrückt Then
                    ' Die Nummer wurde unterdrückt
                    .TelNr = PDfltStringEmpty
                    .Firma = PDfltStringEmpty
                    .AnrName = PDfltStringUnbekannt
                Else
                    If TelFt.Anrufer IsNot Nothing Then
                        ' Kontaktinformationen wurden gefunden
                        .AnrName = TelFt.Anrufer
                        .TelNr = TelFt.GegenstelleTelNr.Formatiert
                        .Firma = TelFt.Firma
                    Else
                        ' Kontaktinformationen wurden nicht gefunden
                        .AnrName = TelFt.GegenstelleTelNr.Formatiert
                        .TelNr = PDfltStringEmpty
                        .Firma = PDfltStringEmpty
                    End If
                End If

                If XMLData.POptionen.PCBAnrMonContactImage AndAlso TelFt.OlContact IsNot Nothing Then
                    ' Kontaktbild ermitteln

                    Dim ImgPath As String = KontaktBild(TelFt.OlContact)

                    If ImgPath.IsNotStringEmpty Then
                        Using fs As New IO.FileStream(ImgPath, IO.FileMode.Open)
                            .Image = Image.FromStream(fs)
                        End Using
                        DelKontaktBild(ImgPath)
                    End If

                End If

                .AutoAusblenden = XMLData.POptionen.PCBAutoClose

                With .OptionsMenu
                    With .Items("ToolStripMenuItemRückruf")
                        .Text = PAnrMonPopUpToolStripMenuItemRückruf
                        .Image = My.Resources.CallTo
                        .Enabled = Not TelFt.NrUnterdrückt
                    End With
                    With .Items("ToolStripMenuItemKopieren")
                        .Text = PAnrMonPopUpToolStripMenuItemKopieren
                        .Image = My.Resources.Copy
                        .Enabled = Not TelFt.NrUnterdrückt
                    End With
                    With .Items("ToolStripMenuItemKontaktöffnen")
                        .Text = If(TelFt.NrUnterdrückt, PAnrMonPopUpToolStripMenuItemKontaktErstellen, PAnrMonPopUpToolStripMenuItemKontaktöffnen)
                        .Image = My.Resources.ContactCard
                    End With
                End With
            Else
                ' Uhrzeit des Telefonates eintragen
                .Uhrzeit = Now
                ' Telefonnamen eintragen
                .TelName = "Gerät"
                .AnrName = "Test: Name"
                .Firma = "Test: Firma"
                .TelNr = "Test: +49 (123) 4567890"
            End If
        End With
    End Sub

    ''' <summary>
    ''' Startet den BackgroundWorker für das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="tmpTelefonat">Telefonat, das angezeigt wird</param>
    Friend Sub AnrMonEinblenden(ByVal tmpTelefonat As Telefonat)
        BWAnrMonEinblenden = New BackgroundWorker
        With BWAnrMonEinblenden
            .WorkerSupportsCancellation = False
            .WorkerReportsProgress = False
            .RunWorkerAsync(tmpTelefonat)
        End With
    End Sub

    Friend Sub UpdateAnrMon(ByVal tmpTelefonat As Telefonat)
        AnrMonausfüllen(PopUpAnrufMonitor, tmpTelefonat)
    End Sub

    ''' <summary>
    ''' Abarbeitung des BackgroundWorkers für das Einblenden des Anrufmonitors
    ''' </summary>
    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim Telefonat As Telefonat = CType(e.Argument, Telefonat)
        Dim RemoveTelFromList As Boolean = False
        'Dim ThisPopUpAnrMon As New FormAnrMon
        Dim TelinList As Boolean = False

        PopUpAnrufMonitor = New FormAnrMon
        AnrMonausfüllen(PopUpAnrufMonitor, Telefonat)

        KeepoInspActivated(False)

        PopUpAnrufMonitor.Popup()

        AddHandler PopUpAnrufMonitor.Close, AddressOf PopUpAnrMon_Close
        AddHandler PopUpAnrufMonitor.Closed, AddressOf PopupAnrMon_Closed

        'AddHandler PopUpAnrufMonitor.LinkClick, AddressOf ToolStripMenuItemKontaktöffnen_Click
        'AddHandler PopUpAnrufMonitor.ToolStripMenuItemClicked, AddressOf ToolStripMenuItem_Clicked

        KeepoInspActivated(True)

        Do
            PopUpAnrufMonitor.TmAnimation_Tick()
            ' Steuerung der Wartezeit des Threads
            Application.DoEvents()
        Loop Until PopUpAnrufMonitor.FromCloseed

    End Sub

    ''' <summary>
    ''' Gibt BackgroundWorkers frei. (Dispose)
    ''' </summary>
    Private Sub BWAnrMonEinblenden_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWAnrMonEinblenden.RunWorkerCompleted
        BWAnrMonEinblenden.Dispose()
        BWAnrMonEinblenden = Nothing
    End Sub

    Private Sub PopUpAnrMon_Close(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, FormAnrMon).Hide()
    End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des PopupAnrMon aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' </summary>
    Private Sub PopupAnrMon_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles PopUpAnrufMonitor.Closed

        If PfadKontaktBild.IsNotStringEmpty AndAlso IO.File.Exists(PfadKontaktBild) Then
            DelKontaktBild(PfadKontaktBild)
        End If
    End Sub

#End Region

#Region "Dispose"
    ' Track whether Dispose has been called.
    Private disposed As Boolean = False
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
