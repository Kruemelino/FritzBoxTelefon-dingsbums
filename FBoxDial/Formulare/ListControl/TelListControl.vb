Imports System.Windows.Forms
''' <summary>
''' https://www.codeproject.com/Articles/333864/Flexible-List-Control
''' </summary>
Friend Class TelBuchListControl
    Friend Event ItemClick(sender As Object, Index As Integer)
    Friend Property Telefonbuch As FritzBoxXMLTelefonbuch
    Private Property LastSelected As TelBuchListControlItem = Nothing

    Friend Sub AddTelefonbuch(TelBuch As FritzBoxXMLTelefonbuch)
        Telefonbuch = TelBuch
        Dim TelListControl As New TelBuchListControlItem With {
            .Name = String.Format("TelBuch", flpListBox.Controls.Count + 1),
            .Margin = New Padding(0),
            .TelBuchName = Telefonbuch.Name,
            .Anzahl = Telefonbuch.Kontakte.Count,
            .Besitzer = Telefonbuch.Owner,
            .ScaleFaktor = GetScaling()
        }

        AddHandler TelListControl.SelectionChanged, AddressOf SelectionChanged
        AddHandler TelListControl.Click, AddressOf ItemClicked

        flpListBox.Controls.Add(TelListControl)
        SetupAnchors()
    End Sub

    Friend Sub Remove(name As String)
        ' grab which control is being removed
        Dim TelListControl As TelBuchListControlItem = CType(flpListBox.Controls(name), TelBuchListControlItem)
        flpListBox.Controls.Remove(TelListControl)
        ' remove the event hook
        RemoveHandler TelListControl.SelectionChanged, AddressOf SelectionChanged
        RemoveHandler TelListControl.Click, AddressOf ItemClicked
        ' now dispose off properly
        TelListControl.Dispose()
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
End Class
