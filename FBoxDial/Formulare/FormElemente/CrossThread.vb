Imports System.Windows.Forms

''' <summary>
''' Stellt Methoden bereit, mit denen ein beliebiger Methoden-Aufruf mit bis zu 3 Argumenten
''' in einen Nebenthread verlegt werden kann, bzw. aus einem Nebenthread in den Hauptthread
''' Quelle (ErfinderDesRades): https://www.vb-paradise.de/index.php/Thread/61948-VersuchsChat-mit-leistungsf%C3%A4higem-Server
''' </summary>
Public Class CrossThread

    Public Shared Sub RunAsync(Of T1, T2, T3)(Action As Action(Of T1, T2, T3), Arg1 As T1, Arg2 As T2, Arg3 As T3)
        ' Aufruf von Action.EndInvoke() gewährleisten, indem er als Callback-Argument mitgegeben wird
        Action.BeginInvoke(Arg1, Arg2, Arg3, AddressOf Action.EndInvoke, Nothing)
    End Sub
    Public Shared Sub RunAsync(Of T1, T2)(Action As Action(Of T1, T2), Arg1 As T1, Arg2 As T2)
        Action.BeginInvoke(Arg1, Arg2, AddressOf Action.EndInvoke, Nothing)
    End Sub
    Public Shared Sub RunAsync(Of T1)(Action As Action(Of T1), Arg1 As T1)
        Action.BeginInvoke(Arg1, AddressOf Action.EndInvoke, Nothing)
    End Sub
    Public Shared Sub RunAsync(Action As System.Action)
        Action.BeginInvoke(AddressOf Action.EndInvoke, Nothing)
    End Sub

    Private Shared Function GuiCrossInvoke(Action As [Delegate], ParamArray Args() As Object) As Boolean
        GuiCrossInvoke = False
        'wenn kein Form mehr da ist, so tun, als ob das Invoking ausgeführt wäre
        If Application.OpenForms.Count.IsZero Then Return True

        If Application.OpenForms(0).InvokeRequired Then
            Application.OpenForms(0).BeginInvoke(Action, Args)
            Return True
        End If
    End Function

    Public Shared Sub RunGui(Of T1, T2, T3)(Action As Action(Of T1, T2, T3), Arg1 As T1, Arg2 As T2, Arg3 As T3)
        'falls Invoking nicht erforderlich, die Action direkt ausführen
        If Not GuiCrossInvoke(Action, Arg1, Arg2, Arg3) Then Action(Arg1, Arg2, Arg3)
    End Sub
    Public Shared Sub RunGui(Of T1, T2)(Action As Action(Of T1, T2), Arg1 As T1, Arg2 As T2)
        If Not GuiCrossInvoke(Action, Arg1, Arg2) Then Action(Arg1, Arg2)
    End Sub
    Public Shared Sub RunGui(Of T1)(Action As Action(Of T1), Arg1 As T1)
        If Not GuiCrossInvoke(Action, Arg1) Then Action(Arg1)
    End Sub
    Public Shared Sub RunGui(Action As System.Action)
        If Not GuiCrossInvoke(Action) Then Action()
    End Sub

End Class
