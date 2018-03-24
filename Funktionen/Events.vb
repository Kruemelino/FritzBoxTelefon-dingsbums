Public Interface IEventProvider
    Sub GenericHandler(ByVal sender As Object, ByVal e As EventArgs)
    'spezifischere Handler hier definieren
End Interface

<DebuggerStepThrough>
Public Class EventMulticaster
    Implements IEventProvider

    Public Event GenericEvent As EventHandler

    Public Sub GenericHandler(ByVal sender As Object, ByVal e As EventArgs) Implements IEventProvider.GenericHandler
        RaiseEvent GenericEvent(sender, e)
    End Sub
End Class