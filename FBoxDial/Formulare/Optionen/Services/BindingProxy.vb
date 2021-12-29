Imports System.Windows

''' <summary>
''' Quelle: Dirk Bahle: <code>https://www.codeproject.com/Articles/1224943/Advanced-WPF-TreeView-in-Csharp-VB-Net-Part-of-n</code>
''' Implements an XAML proxy which can be used to bind items (TreeViewItem, ListViewItem etc)
''' with a viewmodel that manages the collecrions.
''' 
''' Source: <code>http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-Is-Not-inherited/</code>
''' Issue: <code>http://stackoverflow.com/questions/9994241/mvvm-binding-command-to-contextmenu-item</code>
''' </summary>
Public Class BindingProxy
    Inherits Freezable

    ''' <summary>
    ''' Backing storage of the Data dependency property.
    '''
    ''' Gets/sets the data object this class Is forwarding to everyone
    ''' who has a reference to this object.
    ''' </summary>
    Public Shared ReadOnly DataProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(Data),
                                    GetType(Object),
                                    GetType(BindingProxy),
                                    New UIPropertyMetadata(Nothing))

    ''' <summary>
    ''' Gets/sets the data object this class Is forwarding to everyone
    ''' who has a reference to this object.
    ''' </summary>
    Public Property Data As Object
        Get
            Return CObj(GetValue(DataProperty))
        End Get

        Set(ByVal value As Object)
            SetValue(DataProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Overrides of Freezable
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function CreateInstanceCore() As Freezable
        Return New BindingProxy()
    End Function
End Class
