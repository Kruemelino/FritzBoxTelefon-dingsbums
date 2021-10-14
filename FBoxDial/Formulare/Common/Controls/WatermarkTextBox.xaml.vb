Imports System.Windows
Imports System.Windows.Controls

Public Class WatermarkTextBox
    Inherits UserControl

#Region "Label"
    Public Property Text As String
        Get
            Return CStr(GetValue(TextProperty))
        End Get
        Set
            SetValue(TextProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(WatermarkTextBox), New PropertyMetadata(""))

#End Region

#Region "Watermark"

    Public Property Watermark As String
        Get
            Return CStr(GetValue(WatermarkProperty))
        End Get
        Set
            SetValue(WatermarkProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly WatermarkProperty As DependencyProperty = DependencyProperty.Register(NameOf(Watermark), GetType(String), GetType(WatermarkTextBox), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
        WatermarkTextBox.DataContext = Me
    End Sub
End Class
