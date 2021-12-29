Imports System.Windows.Forms
''' <summary>
''' https://stackoverflow.com/questions/57604675/get-keyboard-input-for-word-process-in-vsto-addin
''' </summary>
Friend Class KeyboardHooking

    Private Shared ReadOnly _proc As LowLevelKeyboardProc = AddressOf HookCallback
    Private Shared _hookID As IntPtr = IntPtr.Zero

    'declare the mouse hook constant.
    'For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
    Private Const WH_KEYBOARD As Integer = 2 ' mouse
    Private Const HC_ACTION As Integer = 0

    Friend Shared Sub SetHook()
        ' Ignore this compiler warning, as SetWindowsHookEx doesn't work with ManagedThreadId
#Disable Warning BC40000 ' Typ oder Element ist veraltet
        _hookID = UnSaveMethods.SetWindowsHookEx(WH_KEYBOARD, _proc, IntPtr.Zero, CUInt(AppDomain.GetCurrentThreadId()))
#Enable Warning BC40000 ' Typ oder Element ist veraltet
    End Sub

    Friend Shared Sub ReleaseHook()
        Dim b = UnSaveMethods.UnhookWindowsHookEx(_hookID)
    End Sub


    'Note that the custom code goes in this method the rest of the class stays the same.
    'It will trap if BOTH keys are pressed down.
    Private Shared Function HookCallback(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As Integer
        If nCode < 0 Then
            Return CInt(UnSaveMethods.CallNextHookEx(_hookID, nCode, wParam, lParam))
        Else

            If nCode = HC_ACTION Then
                Dim keyData As Keys = CType(wParam, Keys)

                If IsKeyDown(Keys.F2) Then
                    AddWindow(Of KontaktsucheWPF)()
                End If

            End If

            Return CInt(UnSaveMethods.CallNextHookEx(_hookID, nCode, wParam, lParam))
        End If
    End Function

    Private Shared Function IsKeyDown(keys As Keys) As Boolean
        Return (UnSaveMethods.GetKeyState(keys) And &H8000) = &H8000
    End Function
End Class
