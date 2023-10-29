Imports System.Windows.Controls

Public Class FocusedEditableComboBox
    Inherits ComboBox

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()
        With CType(GetTemplateChild("PART_EditableTextBox"), TextBox)

            ' Nicht schön, aber damit kann der Text Zentriert werden.
            .TextAlignment = Windows.TextAlignment.Center

            ' Setze den Fokus auf die TextBox
            .Focus()
        End With

    End Sub

End Class