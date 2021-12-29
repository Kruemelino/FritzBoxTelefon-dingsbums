Imports System.ComponentModel
Imports System.Runtime.CompilerServices
''' <summary>
''' Implementation of <see cref="INotifyPropertyChanged" /> to simplify models.
''' </summary>
<DebuggerStepThrough>
Public MustInherit Class NotifyBase
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Checks if a property already matches a desired value. 
    ''' Sets the property And notifies listeners only when necessary.
    ''' </summary>
    ''' <typeparam name="T">Type of the property.</typeparam>
    ''' <param name="storage">Reference to a property with both getter And setter.</param>
    ''' <param name="value">Desired value for the property.</param>
    ''' <param name="propertyName">
    ''' Name of the property used to notify listeners.
    ''' This value Is optional And can be provided automatically when invoked from compilers that support CallerMemberName.    
    ''' </param>
    Protected Sub SetProperty(Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As String = Nothing)

        If Equals(storage, value) Then
        Else
            storage = value
            OnPropertyChanged(propertyName)
        End If

    End Sub

    ''' <summary>
    ''' Notifies listeners that a property value has changed.
    ''' </summary>
    ''' <param name="propertyName">
    ''' Name of the property used to notify listeners.
    ''' This value Is optional And can be provided automatically when invoked from compilers that support <see cref="CallerMemberNameAttribute" />.    '''     
    ''' </param>
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        Try
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        Catch
        End Try
    End Sub

End Class
