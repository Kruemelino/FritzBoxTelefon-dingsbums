Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsTextBox
    Inherits UserControl
#Region "Label"

    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set
            SetValue(LabelProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(Label), GetType(String), GetType(OptionsTextBox), New PropertyMetadata(""))

#End Region

#Region "Value"
    Public Property Value As String
        Get
            Return CStr(GetValue(ValueProperty))
        End Get
        Set
            SetValue(ValueProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(Value), GetType(String), GetType(OptionsTextBox), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class