Imports System.Security
Imports System.Runtime.InteropServices

Public Structure RECT
    Dim left As Long
    Dim top As Long
    Dim right As Long
    Dim bottom As Long
End Structure

<SuppressUnmanagedCodeSecurityAttribute()> _
Friend NotInheritable Class UnsafeNativeMethods
    Friend Delegate Function EnumCallBackDelegate(ByVal hwnd As IntPtr, ByVal lParam As Integer) As IntPtr
    Private Sub New()
    End Sub

    ''' <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
    ''' <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation. </returns>
    <DllImport("user32.dll", EntryPoint:="GetForegroundWindow", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="GetWindowText", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetWindowText(ByVal hwnd As IntPtr, ByVal lpString As String, ByVal cch As Integer) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="EnumChildWindows", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function EnumChildWindows(ByVal hWndParent As IntPtr, ByVal lpEnumFunc As EnumCallBackDelegate, ByVal lParam As Integer) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="GetWindowRect", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindowEx", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function FindWindowEx(ByVal parentHandle As IntPtr, ByVal childAfter As IntPtr, ByVal lclassName As String, ByVal windowTitle As String) As IntPtr
    End Function

    <DllImport("UxTheme.dll", EntryPoint:="IsThemeActive", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function IsThemeActive() As Boolean
    End Function

    <DllImport("user32.dll", EntryPoint:="GetShellWindow", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetShellWindow() As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="GetDesktopWindow", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetDesktopWindow() As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="SetFocus", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function SetFocus(ByVal hwnd As IntPtr) As Long
    End Function

    <DllImport("user32.dll", EntryPoint:="SendMessage", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Long, ByVal lParam As Long) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="SendMessage", CharSet:=CharSet.Unicode)> _
    Friend Shared Function SendMessage(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
    End Function

    <DllImport("user32.dll", EntryPoint:="SendMessage", CharSet:=CharSet.Unicode)> _
    Friend Shared Function SendMessage(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, <MarshalAs(UnmanagedType.LPWStr)> ByVal lParam As System.Text.StringBuilder) As Integer
    End Function

    <DllImport("user32.dll", EntryPoint:="SendMessage", CharSet:=CharSet.Unicode)> _
    Friend Shared Function SendMessage(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As String) As IntPtr
    End Function

End Class

Public NotInheritable Class OutlookSecurity

    Public Shared ReadOnly Property GetForegroundWindow() As IntPtr
        Get
            Return UnsafeNativeMethods.GetForegroundWindow()
        End Get
    End Property
    Public Shared ReadOnly Property GetWindowText(ByVal hwnd As IntPtr) As String
        Get
            Dim lpString As String = Space(255)
            Dim l As IntPtr = UnsafeNativeMethods.GetWindowText(hwnd, lpString, Len(lpString))
            Return Left(lpString, CInt(l))
        End Get
    End Property
    Public Shared ReadOnly Property FindWindowEX(ByVal hWndParent As IntPtr, ByVal hWndChildAfter As IntPtr, ByVal lpszClass As String, ByVal lpszWindow As String) As IntPtr
        Get
            Return UnsafeNativeMethods.FindWindowEx(hWndParent, hWndChildAfter, lpszClass, lpszWindow)
        End Get
    End Property
    Public Shared ReadOnly Property GetShellWindow() As IntPtr
        Get
            Return UnsafeNativeMethods.GetShellWindow()
        End Get
    End Property
    Public Shared ReadOnly Property GetDesktopWindow() As IntPtr
        Get
            Return UnsafeNativeMethods.GetDesktopWindow()
        End Get
    End Property
    Public Shared ReadOnly Property GetWindowRect(ByVal hwnd As IntPtr) As RECT
        Get
            Dim lpRect As RECT
            UnsafeNativeMethods.GetWindowRect(hwnd, lpRect)
            Return lpRect
        End Get
    End Property
    Public Shared ReadOnly Property SetFocus(ByVal hwnd As IntPtr) As Long
        Get
            Return UnsafeNativeMethods.SetFocus(hwnd)
        End Get
    End Property
    Public Shared ReadOnly Property IsThemeActive() As Boolean
        Get
            Return UnsafeNativeMethods.IsThemeActive()
        End Get
    End Property
    Public Overloads Shared ReadOnly Property SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Long, ByVal lParam As Long) As IntPtr
        Get
            Return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam)
        End Get
    End Property
    Public Overloads Shared ReadOnly Property SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As String) As IntPtr
        Get
            Return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam)
        End Get
    End Property
End Class
