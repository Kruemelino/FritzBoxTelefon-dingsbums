Imports System.Windows.Forms
''' <summary>
''' https://www.codeproject.com/Articles/333864/Flexible-List-Control
''' </summary>
Friend Class TelBuchListControl
    Friend Event ItemClick(sender As Object, Index As Integer)

    Private Property LastSelected As TelBuchListControlItem = Nothing

    Public Event ContextMenuClick(sender As Object, e As ToolStripItemClickedEventArgs, TB As FritzBoxXMLTelefonbuch)

    Friend Sub AddTelefonbuch(TelBuch As FritzBoxXMLTelefonbuch)
        Dim TelListControl As New TelBuchListControlItem With {
            .Name = $"TelBuch_{flpListBox.Controls.Count + 1}",
            .Margin = New Padding(0),
            .ScaleFaktor = GetScaling(),
            .Telefonbuch = TelBuch
        }
        '.TelBuchName = TelBuch.Name,
        '    .Anzahl = TelBuch.Kontakte.Count,
        '    .Besitzer = TelBuch.Owner,
        AddHandler TelListControl.SelectionChanged, AddressOf SelectionChanged
        AddHandler TelListControl.Click, AddressOf ItemClicked
        AddHandler TelListControl.ContextMenuClicked, AddressOf ContextMenuClicked

        flpListBox.Controls.Add(TelListControl)
        SetupAnchors()
    End Sub

    Friend Sub Remove(TelBuch As FritzBoxXMLTelefonbuch)

        ' grab which control is being removed
        For Each ctrl As TelBuchListControlItem In flpListBox.Controls
            If ctrl.Telefonbuch.Equals(TelBuch) Then
                flpListBox.Controls.Remove(ctrl)

                RemoveHandler ctrl.SelectionChanged, AddressOf SelectionChanged
                RemoveHandler ctrl.Click, AddressOf ItemClicked
                RemoveHandler ctrl.ContextMenuClicked, AddressOf ContextMenuClicked

                ' now dispose off properly
                ctrl.Dispose()

                Exit For
            End If
        Next
        ' remove the event hook
        SetupAnchors()
    End Sub

    Friend Sub Clear()
        Do Until flpListBox.Controls.Count.IsZero
            Dim c As Control = flpListBox.Controls(0)
            flpListBox.Controls.Remove(c)
            c.Dispose()
        Loop
    End Sub

    Private Sub SetupAnchors()
        If flpListBox.Controls.Count > 0 Then
            For i = 0 To flpListBox.Controls.Count - 1
                Dim c As Control = flpListBox.Controls(i)
                If i = 0 Then
                    ' Its the first control, all subsequent controls follow
                    ' the anchor behavior of this control.
                    c.Anchor = AnchorStyles.Left Or AnchorStyles.Top
                    c.Width = flpListBox.Width - SystemInformation.VerticalScrollBarWidth
                Else
                    ' It is not the first control. Set its anchor to
                    ' copy the width of the first control in the list.
                    c.Anchor = AnchorStyles.Left Or AnchorStyles.Right
                End If
            Next
        End If
    End Sub

    Private Sub FlpListBox_Layout(sender As Object, e As LayoutEventArgs) Handles flpListBox.Layout
        If flpListBox.Controls.Count.IsNotZero Then
            flpListBox.Controls(0).Width = flpListBox.Size.Width - If(flpListBox.VerticalScroll.Visible, SystemInformation.VerticalScrollBarWidth, 0)
            Refresh()
        End If
    End Sub

    Private Sub FlpListBox_Resize(sender As Object, e As EventArgs) Handles flpListBox.Resize

        If flpListBox.Controls.Count.IsNotZero Then
            flpListBox.Controls(0).Width = flpListBox.Width - If(flpListBox.VerticalScroll.Visible, SystemInformation.VerticalScrollBarWidth, 0)
            Refresh()
        End If
    End Sub

    Private Sub SelectionChanged(sender As Object)
        If LastSelected IsNot Nothing Then LastSelected.Selected = False
        LastSelected = CType(sender, TelBuchListControlItem)
        Refresh()
    End Sub

    Private Sub ItemClicked(sender As Object, e As EventArgs)
        RaiseEvent ItemClick(Me, flpListBox.Controls.IndexOfKey(CType(sender, TelBuchListControlItem).Name))
    End Sub
    Private Sub ContextMenuClicked(sender As Object, e As ToolStripItemClickedEventArgs, TB As FritzBoxXMLTelefonbuch)
        RaiseEvent ContextMenuClick(sender, e, TB)
    End Sub

End Class
