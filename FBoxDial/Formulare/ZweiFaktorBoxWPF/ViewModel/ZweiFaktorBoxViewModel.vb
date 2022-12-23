Imports System.Windows.Media
Imports System.Windows.Threading

Public Class ZweiFaktorBoxViewModel
    Inherits NotifyBase
    Implements IViewModelBase
    Private Property DatenService As IZweiFAService
    Public Property Instance As Dispatcher Implements IViewModelBase.Instance

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
#End Region

#Region "Eigenschaften"
    Private _BackgroundColor As String = CType(Globals.ThisAddIn.WPFApplication.FindResource("BackgroundColor"), SolidColorBrush).Color.ToString()
    Public Property BackgroundColor As String
        Get
            Return _BackgroundColor
        End Get
        Set
            SetProperty(_BackgroundColor, Value)
        End Set
    End Property

    Private _ForeColor As String = CType(Globals.ThisAddIn.WPFApplication.FindResource("ControlDefaultForeground"), SolidColorBrush).Color.ToString()
    Public Property ForeColor As String
        Get
            Return _ForeColor
        End Get
        Set
            SetProperty(_ForeColor, Value)
        End Set
    End Property

    Private _Methods As String
    Public Property Methods As String
        Get
            Return _Methods
        End Get
        Set
            SetProperty(_Methods, Value)
        End Set
    End Property
#End Region

#Region "SetProperties"
    Friend WriteOnly Property SetMethods As String
        Set
            Methods = Value
        End Set
    End Property

#End Region

    Public Sub New(ds As IZweiFAService)
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelAuth)

        ' Interface
        _DatenService = ds
        ' Theme
        DatenService.UpdateTheme()
    End Sub

#Region "ICommand Callback"
    Private Sub CancelAuth(o As Object)
        DatenService.CancelAuth()
    End Sub
#End Region
End Class
