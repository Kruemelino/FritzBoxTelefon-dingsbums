Imports System.Windows.Forms
''' <summary>
''' https://stackoverflow.com/questions/57604675/get-keyboard-input-for-word-process-in-vsto-addin
''' https://stackoverflow.com/a/10257266
''' </summary>
Friend NotInheritable Class KeyboardHooking

    Private Shared ReadOnly Property Proc As LowLevelKeyboardProc = AddressOf HookCallback
    Private Shared Property HookID As IntPtr = IntPtr.Zero

    Private Const WH_KEYBOARD As Integer = 2
    Private Const HC_ACTION As Integer = 0

    Private Shared Property Shift As Boolean
    Private Shared Property Control As Boolean

    Friend Shared Sub SetHook(useShift As Boolean, useCtrl As Boolean)
        Shift = useShift
        Control = useCtrl

        If HookID = IntPtr.Zero Then
            ' Ignore this compiler warning, as SetWindowsHookEx doesn't work with ManagedThreadId
#Disable Warning BC40000 ' Typ oder Element ist veraltet
            HookID = UnSaveMethods.SetWindowsHookEx(WH_KEYBOARD, Proc, IntPtr.Zero, CUInt(AppDomain.GetCurrentThreadId()))
#Enable Warning BC40000 ' Typ oder Element ist veraltet
        End If

    End Sub

    Friend Shared Sub ReleaseHook()
        If Not HookID = IntPtr.Zero Then
            Dim b As Boolean = UnSaveMethods.UnhookWindowsHookEx(HookID)
        End If
        HookID = IntPtr.Zero
    End Sub


    'Note that the custom code goes in this method the rest of the class stays the same.
    'It will trap if BOTH keys are pressed down.
    Private Shared Function HookCallback(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As Integer
        If nCode < 0 Then
            Return CInt(UnSaveMethods.CallNextHookEx(HookID, nCode, wParam, lParam))
        Else

            If nCode = HC_ACTION Then
                Dim keyData As Keys = CType(wParam, Keys)

                Dim ModifierKey As Boolean

                If Shift And Control Then ModifierKey = IsKeyDown(Keys.ShiftKey) AndAlso IsKeyDown(Keys.ControlKey)

                If Shift And Not Control Then ModifierKey = IsKeyDown(Keys.ShiftKey) AndAlso Not IsKeyDown(Keys.ControlKey)

                If Not Shift And Control Then ModifierKey = Not IsKeyDown(Keys.ShiftKey) AndAlso IsKeyDown(Keys.ControlKey)

                If Not Shift And Not Control Then ModifierKey = Not IsKeyDown(Keys.ShiftKey) AndAlso Not IsKeyDown(Keys.ControlKey)

                ' Prüfe, ob die Modifier-Keys und die definierte Taste gedrückt wurden
                If IsKeyDown(keyData) And keyData = Keys.F2 And ModifierKey Then AddWindow(Of KontaktsucheWPF)(False)

            End If

            Return CInt(UnSaveMethods.CallNextHookEx(_HookID, nCode, wParam, lParam))
        End If
    End Function

    Private Shared Function IsKeyDown(keys As Keys) As Boolean
        Return (UnSaveMethods.GetKeyState(keys) And &H8000) = &H8000
    End Function
End Class
