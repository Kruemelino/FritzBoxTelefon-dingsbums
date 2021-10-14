Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsTextBlock
    Inherits UserControl
#Region "Label"

    ''' <summary>
    ''' Gets or sets the Label which is displayed next to the field
    ''' </summary>
    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set
            SetValue(LabelProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(Label), GetType(String), GetType(OptionsTextBlock), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class