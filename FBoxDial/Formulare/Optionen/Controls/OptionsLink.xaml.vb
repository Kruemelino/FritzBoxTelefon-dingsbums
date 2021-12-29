Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsLink
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
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(Label), GetType(String), GetType(OptionsLink), New PropertyMetadata(""))

#End Region

#Region "LinkLabel"

    ''' <summary>
    ''' Gets or sets the Value which is being displayed
    ''' </summary>
    Public Property LinkLabel As String
        Get
            Return CStr(GetValue(LinkLabelProperty))
        End Get
        Set
            SetValue(LinkLabelProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly LinkLabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(LinkLabel), GetType(String), GetType(OptionsLink), New PropertyMetadata(""))

#End Region

#Region "Value"

    ''' <summary>
    ''' Gets or sets the Value which is being displayed
    ''' </summary>
    Public Property URL As String
        Get
            Return CStr(GetValue(URLProperty))
        End Get
        Set
            SetValue(URLProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly URLProperty As DependencyProperty = DependencyProperty.Register(NameOf(URL), GetType(String), GetType(OptionsLink), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Hyperlink_RequestNavigate(sender As Object, e As Navigation.RequestNavigateEventArgs)
        Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        e.Handled = True
    End Sub
End Class