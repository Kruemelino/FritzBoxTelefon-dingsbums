Imports System.Security
Imports System.Runtime.InteropServices

Public Structure RECT
    Dim left As Long
    Dim top As Long
    Dim right As Long
    Dim bottom As Long
End Structure

<SuppressUnmanagedCodeSecurityAttribute()> Friend NotInheritable Class SafeNativeMethods
    ' formConfig
    Public Declare Function IsThemeActive Lib "UxTheme.dll" () As Boolean

    'OutlookInterface
    Friend Declare Function GetForegroundWindow Lib "user32" Alias "GetForegroundWindow" () As IntPtr
    Friend Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As IntPtr, ByVal lpString As String, ByVal cch As Integer) As IntPtr
    Friend Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
    Friend Declare Function GetShellWindow Lib "user32" () As IntPtr
    Friend Declare Function GetDesktopWindow Lib "user32" () As IntPtr
    Friend Declare Function GetWindowRect Lib "User32" (ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Int32
End Class