''' <summary>
''' Eventhandler, der den Sender ordentlich typisiert übermittelt
''' </summary>
Public Delegate Sub EventHandlerEx(Of T0)(Sender As T0)

Public Class NotifyEventArgs(Of T) : Inherits EventArgs
    Public ReadOnly Value As T
    Public Sub New(Value As T)
        MyBase.New()
        Me.Value = Value
    End Sub
End Class
