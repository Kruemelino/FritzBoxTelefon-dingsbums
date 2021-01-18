Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsCheckBox
    Inherits UserControl
#Region "Label DP"

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
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(OptionsCheckBox), New PropertyMetadata(""))

#End Region

#Region "Label DP"

    ''' <summary>
    ''' Gets or sets the Value which is being displayed
    ''' </summary>
    Public Property Value As Boolean
        Get
            Return CBool(GetValue(ValueProperty))
        End Get
        Set
            SetValue(ValueProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("Value", GetType(Boolean), GetType(OptionsCheckBox), New PropertyMetadata(False))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class