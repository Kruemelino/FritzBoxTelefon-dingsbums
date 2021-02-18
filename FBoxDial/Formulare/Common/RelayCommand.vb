Imports System.Windows.Input

''' <summary>
''' Diese Klasse implementiert das ICommand Interface, so muss man nicht in jeder Klasse eines ViewModel alles selbst implementieren.
''' Einfach eine Command wie folgt instanzieren:
''' MyCommand = New RelayCommand(AddressOf MyCommand_Execute, AddressOf MyCommand_CanExecute)
''' Quelle: Nofear23m https://www.vb-paradise.de/index.php/Thread/128963-Command-s-in-WPF/?postID=1116045#post1116045
''' </summary>
<DebuggerStepThrough>
Public Class RelayCommand : Implements ICommand
#Region "Fields"
    ReadOnly _execute As Action(Of Object)
    ReadOnly _canExecute As Predicate(Of Object)
#End Region
#Region "Constructors"
    ''' <summary>
    ''' Erstellt einen neuen Command welcher NUR Executed werden kann.
    ''' </summary>
    ''' <param name="execute">The execution logic.</param>
    ''' <remarks></remarks>
    Public Sub New(execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub
    ''' <summary>
    ''' Erstellt einen neuen Command welcher sowohl die Execute als auch die CanExecute Logik beinhaltet.
    ''' </summary>
    ''' <param name="execute">Die Logik für Execute.</param>
    ''' <param name="canExecute">Die Logik für CanExecute.</param>
    ''' <remarks></remarks>
    Public Sub New(execute As Action(Of Object), canExecute As Predicate(Of Object))
        If execute Is Nothing Then
            Throw New ArgumentNullException(NameOf(execute))
        End If
        _execute = execute
        _canExecute = canExecute
    End Sub
#End Region

#Region "ICommand Members"
    ''' <summary>
    ''' Setzt die CanExecute-Methode des ICommand-Interfaces auf True oder False
    ''' </summary>
    ''' <param name="parameter"></param>
    ''' <returns>Gibt zurück ob die Aktion ausgeführt werden kann oder nicht</returns>
    ''' <remarks>
    ''' Benutzt DebuggerStepThrough from System.Diagnostics
    ''' Der Debugger überspringt diese Prozedur also, es sei den es wird explizit ein Haltepunkt gesetzt.
    ''' </remarks>

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return _canExecute Is Nothing OrElse _canExecute(parameter)
    End Function

    ''' <summary>
    ''' Event welches geworfen wird wenn die Propertie CanExecuteChanged sich ändert.
    ''' </summary>
    ''' <remarks></remarks>
    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        AddHandler(value As EventHandler)
            If _canExecute IsNot Nothing Then
                AddHandler CommandManager.RequerySuggested, value
            End If
        End AddHandler

        RemoveHandler(value As EventHandler)
            If _canExecute IsNot Nothing Then
                RemoveHandler CommandManager.RequerySuggested, value
            End If
        End RemoveHandler

        RaiseEvent(sender As Object, e As EventArgs)
        End RaiseEvent
    End Event

    ''' <summary>
    ''' Führt die Prozedur Execute des ICommand.Execute aus
    ''' </summary>
    ''' <param name="parameter"></param>
    ''' <remarks></remarks>
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute(parameter)
    End Sub
#End Region
End Class


''' <summary>
''' https://www.vb-paradise.de/index.php/Thread/128963-Command-s-in-WPF/?postID=1116044#post1116044
''' </summary>
<DebuggerStepThrough>
Public Class RelayCommand(Of T)
    Implements ICommand

#Region "Fields"
    Private ReadOnly _execute As Action(Of T)
    Private ReadOnly _canExecute As Func(Of T, Boolean)
#End Region

#Region "Constructors"
    Public Sub New(execute As Action(Of T))
        Me.New(execute, Nothing)
    End Sub

    Public Sub New(execute As Action(Of T), canExecute As Func(Of T, Boolean))
        If execute Is Nothing Then
            Throw New ArgumentNullException(NameOf(execute))
        End If

        _execute = execute
        _canExecute = canExecute
    End Sub
#End Region

#Region "ICommand Members"
    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        AddHandler(value As EventHandler)
            If _canExecute IsNot Nothing Then
                AddHandler CommandManager.RequerySuggested, value
            End If
        End AddHandler
        RemoveHandler(value As EventHandler)
            If _canExecute IsNot Nothing Then
                RemoveHandler CommandManager.RequerySuggested, value
            End If
        End RemoveHandler

        RaiseEvent(sender As Object, e As EventArgs)
            'This is the RaiseEvent block
            CommandManager.InvalidateRequerySuggested()
        End RaiseEvent
    End Event

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If _canExecute Is Nothing Then
            Return True
        Else
            Return _canExecute(CType(parameter, T))
        End If
    End Function

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute(CType(parameter, T))
    End Sub

    Public Sub OnCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub
#End Region
End Class