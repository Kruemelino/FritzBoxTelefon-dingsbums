Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsHinweis
    Inherits UserControl
#Region "Label"

    ''' <summary>
    ''' Gets or sets the Label which is displayed next to the field
    ''' </summary>
    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set(ByVal value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(OptionsHinweis), New PropertyMetadata(""))

#End Region

#Region "Tooltipp"

    ''' <summary>
    ''' Gets or sets the Value which is being displayed
    ''' </summary>
    Public Property ToolTipp As String
        Get
            Return CStr(GetValue(ToolTippProperty))
        End Get
        Set(ByVal value As String)
            SetValue(ToolTippProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly ToolTippProperty As DependencyProperty = DependencyProperty.Register("ToolTipp", GetType(String), GetType(OptionsHinweis), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class