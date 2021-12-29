Imports System.Windows
Imports System.Windows.Controls
''' <summary>
''' https://docs.microsoft.com/de-de/dotnet/api/system.windows.controls.contentcontrol.contenttemplateselector?view=net-5.0
''' </summary>
Public Class OptionenTemplateSelector
    Inherits DataTemplateSelector

    Public Property DirectDialTemplate As DataTemplate
    Public Property ContactDialTemplate As DataTemplate

    Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate

        ' Nothing can be passed by IDE designer
        If (item Is Nothing) Then Return Nothing

        If TypeOf item Is DirectDialViewModel Then
            Return DirectDialTemplate

        Else
            Return ContactDialTemplate

        End If

    End Function 'SelectTemplate
End Class
