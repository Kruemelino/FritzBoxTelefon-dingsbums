Imports System.Threading
Imports System.Threading.Tasks

Friend Module STATask
    Public Function StartSTATask(Of T)(func As Func(Of T)) As Task(Of T)
        Dim tcs = New TaskCompletionSource(Of T)()
        Dim thread As New Thread(Sub()
                                     Try
                                         tcs.SetResult(func())
                                     Catch e As Exception
                                         tcs.SetException(e)
                                     End Try
                                 End Sub)
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        Return tcs.Task
    End Function
End Module
