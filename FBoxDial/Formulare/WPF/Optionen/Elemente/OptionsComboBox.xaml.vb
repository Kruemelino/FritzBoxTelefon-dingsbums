Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsComboBox
    Inherits UserControl

#Region "Label"

    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set(ByVal value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "Value"

    Public Property Value As String
        Get
            Return CStr(GetValue(ValueProperty))
        End Get
        Set(ByVal value As String)
            SetValue(ValueProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("Value", GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "Items"

    Public Property Items As IEnumerable(Of String)
        Get
            Return CType(GetValue(ValueProperty), IEnumerable(Of String))
        End Get
        Set(ByVal value As IEnumerable(Of String))
            SetValue(ValueProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ItemsProperty As DependencyProperty = DependencyProperty.Register("Items", GetType(IEnumerable(Of String)), GetType(OptionsComboBox), New PropertyMetadata(Nothing))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class