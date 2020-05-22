Imports System.Drawing
Imports System.Windows.Forms

Public Class Popup
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private WithEvents PopUpAnrufMonitor As FormAnrMon
    Friend Eingeblendet As Boolean = False
    Friend TelFnt As Telefonat
    Friend Property PfadKontaktBild As String

#Region "Anrufmonitor"
    Private Sub AnrMonausfüllen(ByVal ThisPopUpAnrMon As FormAnrMon, ByVal tTelFnt As Telefonat)

        ' If ThisPopUpAnrMon IsNot Nothing Then

        With ThisPopUpAnrMon
            If tTelFnt IsNot Nothing Then
                ' Telefonat setzen
                TelFnt = tTelFnt

                ' Uhrzeit des Telefonates eintragen
                .Uhrzeit = TelFnt.ZeitBeginn

                ' Telefonnamen eintragen

                If TelFnt.RINGGeräte Is Nothing Then
                    ' Ermitteln der Gerätenammen der Telefone, die auf diese eigene Nummer reagieren
                    TelFnt.RINGGeräte = XMLData.PTelefonie.Telefoniegeräte.FindAll(Function(Tel) Tel.StrEinTelNr.Contains(TelFnt.OutEigeneTelNr))
                End If

                For Each TelGerät As Telefoniegerät In TelFnt.RINGGeräte
                    .TelName = String.Format("{0}, {1}", .TelName, TelGerät.Name)
                Next

                If TelFnt.NrUnterdrückt Then
                    ' Die Nummer wurde unterdrückt
                    .TelNr = PDfltStringEmpty
                    .Firma = PDfltStringEmpty
                    .AnrName = PDfltStringUnbekannt
                Else
                    If TelFnt.Anrufer IsNot Nothing Then
                        ' Kontaktinformationen wurden gefunden
                        .AnrName = TelFnt.Anrufer
                        .TelNr = TelFnt.GegenstelleTelNr.Formatiert
                        .Firma = TelFnt.Firma
                    Else
                        ' Kontaktinformationen wurden nicht gefunden
                        .AnrName = TelFnt.GegenstelleTelNr.Formatiert
                        .TelNr = PDfltStringEmpty
                        .Firma = PDfltStringEmpty
                    End If
                End If

                If XMLData.POptionen.PCBAnrMonContactImage AndAlso TelFnt.OlKontakt IsNot Nothing Then
                    ' Kontaktbild ermitteln

                    Dim ImgPath As String = KontaktBild(TelFnt.OlKontakt)

                    If ImgPath.IsNotStringEmpty Then
                        Using fs As New IO.FileStream(ImgPath, IO.FileMode.Open)
                            .Image = Image.FromStream(fs)
                        End Using
                        DelKontaktBild(ImgPath)
                    End If

                End If

                .AutoAusblenden = XMLData.POptionen.PCBAutoClose
                .Anzeigedauer = XMLData.POptionen.PTBEnblDauer * 1000
                .AnzAnrMon = ThisAddIn.OffenePopUps.Count

                With .OptionsMenu
                    With .Items("ToolStripMenuItemRückruf")
                        .Text = PAnrMonPopUpToolStripMenuItemRückruf
                        .Image = My.Resources.CallTo
                        .Enabled = Not TelFnt.NrUnterdrückt
                    End With
                    With .Items("ToolStripMenuItemKopieren")
                        .Text = PAnrMonPopUpToolStripMenuItemKopieren
                        .Image = My.Resources.Copy
                        .Enabled = Not TelFnt.NrUnterdrückt
                    End With
                    With .Items("ToolStripMenuItemKontaktöffnen")
                        .Text = If(TelFnt.NrUnterdrückt, PAnrMonPopUpToolStripMenuItemKontaktErstellen, PAnrMonPopUpToolStripMenuItemKontaktöffnen)
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

        ' End If
    End Sub

    ''' <summary>
    ''' Startet das Einblenden des Anrufmonitors
    ''' </summary>
    ''' <param name="tmpTelefonat">Telefonat, das angezeigt wird</param>
    Friend Sub AnrMonEinblenden(ByVal tmpTelefonat As Telefonat)

        PopUpAnrufMonitor = New FormAnrMon

        If ThisAddIn.OffenePopUps Is Nothing Then ThisAddIn.OffenePopUps = New List(Of Popup)

        ThisAddIn.OffenePopUps.Add(Me)

        AnrMonausfüllen(PopUpAnrufMonitor, tmpTelefonat)

        KeepoInspActivated(False)

        PopUpAnrufMonitor.Popup()
        Eingeblendet = True

        AddHandler PopUpAnrufMonitor.Schließen, AddressOf PopUpAnrMon_Close
        AddHandler PopUpAnrufMonitor.Geschlossen, AddressOf PopupAnrMon_Closed

        AddHandler PopUpAnrufMonitor.LinkClick, AddressOf AnrMonLink_Click
        AddHandler PopUpAnrufMonitor.ToolStripMenuItemClicked, AddressOf AnrMonToolStripMenuItem_Clicked

        KeepoInspActivated(True)
    End Sub

    Friend Sub UpdateAnrMon(ByVal tmpTelefonat As Telefonat)
        If PopUpAnrufMonitor IsNot Nothing Then
            AnrMonausfüllen(PopUpAnrufMonitor, tmpTelefonat)
            ' Neu Zeichnen
            PopUpAnrufMonitor.Invalidate()
        End If
    End Sub

    Friend Sub AnrMonAusblenden()
        If PopUpAnrufMonitor IsNot Nothing Then PopUpAnrufMonitor.Close()
    End Sub

    Private Sub PopUpAnrMon_Close(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, FormAnrMon).Close()
    End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des PopupAnrMon aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' </summary>
    Private Sub PopupAnrMon_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles PopUpAnrufMonitor.Geschlossen
        NLogger.Debug("Anruffenster geschlossen: {0}", TelFnt.Anrufer)
        Eingeblendet = False
        ' Entferne den Anrufmonitor von der Liste der offenen Popups
        ThisAddIn.OffenePopUps.Remove(Me)
        ' Lösche das Kontaktbild
        If PfadKontaktBild.IsNotStringEmpty AndAlso IO.File.Exists(PfadKontaktBild) Then DelKontaktBild(PfadKontaktBild)
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
