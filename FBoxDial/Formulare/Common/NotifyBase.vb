Imports System.ComponentModel
Imports System.Runtime.CompilerServices
''' <summary>
''' Implementation of <see cref="INotifyPropertyChanged" /> to simplify models.
''' </summary>
Public MustInherit Class NotifyBase
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    '''     Checks if a property already matches a desired value. 
    '''     Sets the property And notifies listeners only when necessary.
    ''' </summary>
    ''' <typeparam name="T">Type of the property.</typeparam>
    ''' <param name="storage">Reference to a property with both getter And setter.</param>
    ''' <param name="value">Desired value for the property.</param>
    ''' <param name="propertyName">
    '''     Name of the property used to notify listeners.
    '''     This value Is optional And can be provided automatically when invoked from compilers that support CallerMemberName.    
    ''' </param>
    ''' <returns>
    '''     True if the value was changed, false if the existing value matched the sesired value.
    ''' </returns>
    Protected Function SetProperty(Of T)(ByRef storage As T, ByVal value As T, <CallerMemberName> ByVal Optional propertyName As String = Nothing) As Boolean

        If Equals(storage, value) Then
            Return False
        Else
            storage = value
            OnPropertyChanged(propertyName)
            Return True
        End If

    End Function

    Protected Function GetProperty(Of T)(ByVal storage As T, DefaultValue As T) As T

        If storage IsNot Nothing Then
            Return storage
        Else
            Return DefaultValue
        End If

    End Function

    ''' <summary>
    '''    Notifies listeners that a property value has changed.
    ''' </summary>
    ''' <param name="propertyName">
    '''     Name of the property used to notify listeners.
    '''     This value Is optional And can be provided automatically when invoked from compilers that support <see cref="CallerMemberNameAttribute" />.    '''     
    ''' </param>
    Protected Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
        Try
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        Catch
        End Try
    End Sub
End Class
