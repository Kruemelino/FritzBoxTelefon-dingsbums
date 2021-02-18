''' <summary>
''' Eventhandler, der den Sender ordentlich typisiert übermittelt
''' https://activevb.de/tipps/vbnettipps/tipp0149.html
''' </summary>
Public Delegate Sub EventHandlerEx(Of T)(Sender As T)

Public Class NotifyEventArgs(Of T) : Inherits EventArgs
    Public ReadOnly Value As T
    Public Sub New(Value As T)
        MyBase.New()
        Me.Value = Value
    End Sub
End Class
