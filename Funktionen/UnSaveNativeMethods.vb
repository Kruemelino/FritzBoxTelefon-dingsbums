Imports System.Security
Imports System.Runtime.InteropServices

Public Structure RECT
    Dim left As Long
    Dim top As Long
    Dim right As Long
    Dim bottom As Long
End Structure
''' <summary>
''' The window sizing and positioning flags.
''' </summary>
''' <remarks></remarks>
<Flags> Public Enum SetWindowPosFlags As Int32
    ''' <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
    ''' the system posts the request to the thread that owns the window. This prevents the calling thread from 
    ''' blocking its execution while other threads process the request.</summary>
    ''' <remarks>SWProperyASYNCWINDOWPOS</remarks>
    SynchronousWindowPosition = &H4000

    ''' <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
    ''' <remarks>SWPropery_DefERERASE</remarks>
    DeferErase = &H2000

    ''' <summary>Draws a frame (defined in the window's class description) around the window.</summary>
    ''' <remarks>SWProperyDRAWFRAME</remarks>
    DrawFrame = &H20

    ''' <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
    ''' the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
    ''' is sent only when the window's size is being changed.</summary>
    ''' <remarks>SWProperyFRAMECHANGED</remarks>
    FrameChanged = &H20

    ''' <summary>Hides the window.</summary>
    ''' <remarks>SWProperyHIDEWINDOW</remarks>
    HideWindow = &H80

    ''' <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
    ''' top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
    ''' parameter).</summary>
    ''' <remarks>SWProperyNOACTIVATE</remarks>
    DoNotActivate = &H10

    ''' <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
    ''' contents of the client area are saved and copied back into the client area after the window is sized or 
    ''' repositioned.</summary>
    ''' <remarks>SWProperyNOCOPYBITS</remarks>
    DoNotCopyBits = &H100

    ''' <summary>Retains the current position (ignores X and Y parameters).</summary>
    ''' <remarks>SWProperyNOMOVE</remarks>
    IgnoreMove = &H2

    ''' <summary>Does not change the owner window's position in the Z order.</summary>
    ''' <remarks>SWProperyNOOWNERZORDER</remarks>
    DoNotChangeOwnerZOrder = &H200

    ''' <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
    ''' the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
    ''' window uncovered as a result of the window being moved. When this flag is set, the application must 
    ''' explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
    ''' <remarks>SWProperyNOREDRAW</remarks>
    DoNotRedraw = &H8

    ''' <summary>Same as the SWProperyNOOWNERZORDER flag.</summary>
    ''' <remarks>SWProperyNOREPOSITION</remarks>
    DoNotReposition = &H200 'Danke an Pikachu für den Tipp :)

    ''' <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
    ''' <remarks>SWProperyNOSENDCHANGING</remarks>
    DoNotSendChangingEvent = &H400

    ''' <summary>Retains the current size (ignores the cx and cy parameters).</summary>
    ''' <remarks>SWProperyNOSIZE</remarks>
    IgnoreResize = &H1

    ''' <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
    ''' <remarks>SWProperyNOZORDER</remarks>
    IgnoreZOrder = &H4

    ''' <summary>Displays the window.</summary>
    ''' <remarks>SWProperySHOWWINDOW</remarks>
    ShowWindow = &H40
End Enum

''' <summary>
''' The SetWindowPos hWndInsertAfter positioning flags.
''' </summary>
''' <remarks></remarks>
<Flags> Public Enum hWndInsertAfterFlags As Integer
    ''' <summary>
    ''' Places the window at the top of the Z order.
    ''' </summary>
    ''' <remarks></remarks>
    HWND_TOP = 0

    ''' <summary>
    ''' Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
    ''' </summary>
    ''' <remarks></remarks>
    HWND_BOTTOM = 1

    ''' <summary>
    ''' Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
    ''' </summary>
    ''' <remarks></remarks>
    HWND_NOTOPMOST = -2

    ''' <summary>
    ''' Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
    ''' </summary>
    ''' <remarks></remarks>
    HWND_TOPMOST = -1
End Enum

<SuppressUnmanagedCodeSecurityAttribute()> Friend NotInheritable Class UnsafeNativeMethods
    ''' <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
    ''' <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation. </returns>
    <DllImport("user32.dll", EntryPoint:="GetForegroundWindow", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="GetWindowText", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetWindowText(ByVal hwnd As IntPtr, ByVal lpString As String, ByVal cch As Int32) As Int32
    End Function

    <DllImport("user32.dll", EntryPoint:="EnumChildWindows", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function EnumChildWindows(ByVal hWndParent As IntPtr, ByVal lpEnumFunc As OutlookSecurity.EnumCallBackDelegate, ByVal lParam As IntPtr) As Int32
    End Function

    <DllImport("user32.dll", EntryPoint:="GetWindowRect", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindowEx", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function FindWindowEx(ByVal parentHandle As IntPtr, ByVal childAfter As IntPtr, ByVal lclassName As String, ByVal windowTitle As String) As IntPtr
    End Function

    <DllImport("UxTheme.dll", EntryPoint:="IsThemeActive", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function IsThemeActive() As <MarshalAs(UnmanagedType.Bool)> Boolean
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
    Friend Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Int32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="SendMessage", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function SendMessage(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As String) As IntPtr
    End Function

    '<DllImport("user32.dll", EntryPoint:="SendMessage", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    'Friend Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As Boolean
    'End Function

    '<DllImport("user32.dll", EntryPoint:="SendMessage", CharSet:=CharSet.Unicode)> _
    'Friend Shared Function SendMessage(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
    'End Function

    '<DllImport("user32.dll", EntryPoint:="SendMessage", CharSet:=CharSet.Unicode)> _
    'Friend Shared Function SendMessage(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, <MarshalAs(UnmanagedType.LPWStr)> ByVal lParam As System.Text.StringBuilder) As Integer
    'End Function

    <DllImport("user32.dll", EntryPoint:="SetWindowPos", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As hWndInsertAfterFlags, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    '<DllImport("user32.dll", SetLastError:=True)> _
    'Friend Shared Function BringWindowToTop(ByVal hwnd As IntPtr) As Boolean
    'End Function

    '<DllImport("user32.dll", SetLastError:=True)> _
    'Friend Shared Function GetActiveWindow() As IntPtr
    'End Function

    <DllImport("user32.dll", EntryPoint:="ReleaseCapture", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Friend Shared Function ReleaseCapture() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
End Class

Public NotInheritable Class OutlookSecurity
    Public Delegate Sub EnumCallBackDelegate(ByVal hwnd As IntPtr, ByVal lParam As Integer)

    ''' <summary>
    ''' Retrieves a handle to the foreground window (the window with which the user is currently working). 
    ''' The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
    ''' </summary>
    ''' <value>IntPtr</value>
    ''' <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property GetForegroundWindow() As IntPtr
        Get
            Return UnsafeNativeMethods.GetForegroundWindow()
        End Get
    End Property

    ''' <summary>
    ''' Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied. 
    ''' However, GetWindowText cannot retrieve the text of a control in another application.
    ''' </summary>
    ''' <param name="hwnd">A handle to the window or control containing the text.</param>
    ''' <value></value>
    ''' <returns>If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character.
    ''' If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.
    ''' This function cannot retrieve the text of an edit control in another application.</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property GetWindowText(ByVal hwnd As IntPtr) As String
        Get
            Dim lpString As String = Space(255)
            Return Left(lpString, UnsafeNativeMethods.GetWindowText(hwnd, lpString, Len(lpString)))
        End Get
    End Property

    ''' <summary>
    ''' Retrieves a handle to a window whose class name and window name match the specified strings. 
    ''' The function searches child windows, beginning with the one following the specified child window. This function does not perform a case-sensitive search.
    ''' </summary>
    ''' <param name="hWndParent">A handle to the parent window whose child windows are to be searched.
    ''' If hwndParent is NULL, the function uses the desktop window as the parent window. The function searches among windows that are child windows of the desktop.
    ''' If hwndParent is HWND_MESSAGE, the function searches all message-only windows.</param>
    ''' <param name="hWndChildAfter">A handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window.
    ''' If hwndChildAfter is NULL, the search begins with the first child window of hwndParent.
    ''' Note that if both hwndParent and hwndChildAfter are NULL, the function searches all top-level and message-only windows.</param>
    ''' <param name="lpszClass">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.
    ''' If lpszClass is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names, or it can be MAKEINTATOM(0x8000). In this latter case, 0x8000 is the atom for a menu class. For more information, see the Remarks section of this topic.</param>
    ''' <param name="lpszWindow">The window name (the window's title). If this parameter is NULL, all window names match.</param>
    ''' <value>IntPtr</value>
    ''' <returns>If the function succeeds, the return value is a handle to the window that has the specified class and window names.</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property FindWindowEX(ByVal hWndParent As IntPtr, ByVal hWndChildAfter As IntPtr, ByVal lpszClass As String, ByVal lpszWindow As String) As IntPtr
        Get
            Return UnsafeNativeMethods.FindWindowEx(hWndParent, hWndChildAfter, lpszClass, lpszWindow)
        End Get
    End Property

    ''' <summary>
    ''' Retrieves a handle to the Shell's desktop window.
    ''' </summary>
    ''' <value>IntPtr</value>
    ''' <returns>The return value is the handle of the Shell's desktop window. If no Shell process is present, the return value is NULL.</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property GetShellWindow() As IntPtr
        Get
            Return UnsafeNativeMethods.GetShellWindow()
        End Get
    End Property

    ''' <summary>
    ''' The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
    ''' </summary>
    ''' <returns>The GetDesktopWindow function returns a handle to the desktop window.</returns>
    Public Shared ReadOnly Property GetDesktopWindow As IntPtr
        Get
            Return UnsafeNativeMethods.GetDesktopWindow()
        End Get
    End Property

    ''' <summary>
    ''' Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    ''' </summary>
    ''' <param name="hwnd">A handle to the window.</param>
    ''' <value>A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</value>
    ''' <returns>If the function succeeds, the return value is nonzero.
    ''' If the function fails, the return value is zero.</returns>
    ''' <remarks>Retrieves the dimensions of the bounding rectangle of the specified window. 
    ''' The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    ''' The Win32 RECT is not binary compatible with System.Drawing.Rectangle.</remarks>
    Public Shared ReadOnly Property GetWindowRect(ByVal hwnd As IntPtr) As RECT
        Get
            Dim lpRect As RECT
            UnsafeNativeMethods.GetWindowRect(hwnd, lpRect)
            Return lpRect
        End Get
    End Property

    ''' <summary>
    ''' Sets the keyboard focus to the specified window. The window must be attached to the calling thread's message queue.
    ''' </summary>
    ''' <param name="hwnd">A handle to the window that will receive the keyboard input. If this parameter is NULL, keystrokes are ignored.</param>
    ''' <value></value>
    ''' <returns>If the function succeeds, the return value is the handle to the window that previously had the keyboard focus. If the hWnd parameter is invalid or the window is not attached to the calling thread's message queue, the return value is NULL. </returns>
    ''' <remarks>
    ''' The SetFocus function sends a WM_KILLFOCUS message to the window that loses the keyboard focus and a WM_SETFOCUS message to the window that receives the keyboard focus. It also activates either the window that receives the focus or the parent of the window that receives the focus.
    ''' If a window is active but does not have the focus, any key pressed will produce the WM_SYSCHAR, WM_SYSKEYDOWN, or WM_SYSKEYUP message. If the VK_MENU key is also pressed, the lParam parameter of the message will have bit 30 set. Otherwise, the messages produced do not have this bit set.
    ''' By using the AttachThreadInput function, a thread can attach its input processing to another thread. This allows a thread to call SetFocus to set the keyboard focus to a window attached to another thread's message queue.
    ''' </remarks>
    Public Shared ReadOnly Property SetFocus(ByVal hwnd As IntPtr) As Long
        Get
            Return UnsafeNativeMethods.SetFocus(hwnd)
        End Get
    End Property

    ''' <summary>
    ''' Tests if a visual style for the current application is active. 
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>
    ''' True, if a visual style is enabled, and windows with visual styles applied should call OpenThemeData to start using theme drawing services.
    ''' False, if a visual style is not enabled, and the window message handler does not need to make another call to IsThemeActive until it receives a WM_THEMECHANGED message.</returns>
    ''' <remarks>Do not call this function during DllMain or global objects contructors. This may cause invalid return values in Windows Vista and may cause Windows XP to become unstable.</remarks>
    Public Shared ReadOnly Property IsThemeActive() As Boolean
        Get
            Return UnsafeNativeMethods.IsThemeActive()
        End Get
    End Property

    ''' <summary>
    ''' Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
    ''' </summary>
    ''' <param name="hWnd">
    ''' A handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
    ''' Message sending is subject to UIPI. The thread of a process can send messages only to message queues of threads in processes of lesser or equal integrity level.</param>
    ''' <param name="Msg">The message to be sent.</param>
    ''' <param name="wParam">Additional message-specific information.</param>
    ''' <param name="lParam">Additional message-specific information.</param>
    ''' <value>IntPtr</value>
    ''' <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
    ''' <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/ms644950(v=vs.85).aspx</remarks>
    Public Overloads Shared ReadOnly Property SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Int32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        Get
            Return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam)
        End Get
    End Property
    ''' <summary>
    ''' Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
    ''' </summary>
    ''' <param name="hWnd">
    ''' A handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
    ''' Message sending is subject to UIPI. The thread of a process can send messages only to message queues of threads in processes of lesser or equal integrity level.</param>
    ''' <param name="Msg">The message to be sent.</param>
    ''' <param name="wParam">Additional message-specific information.</param>
    ''' <param name="lParam">Additional message-specific information.</param>
    ''' <value>IntPtr</value>
    ''' <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
    ''' <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/ms644950(v=vs.85).aspx</remarks>
    Public Overloads Shared ReadOnly Property SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As String) As IntPtr
        Get
            Return UnsafeNativeMethods.SendMessage(hWnd, Msg, wParam, lParam)
        End Get
    End Property


    ''' <summary>
    ''' Enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function. EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE.
    ''' </summary>
    ''' <param name="hWndParent">A handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows.</param>
    ''' <param name="lpEnumFunc">A pointer to an application-defined callback function.</param>
    ''' <param name="lParam">An application-defined value to be passed to the callback function.</param>
    ''' <value>IntPtr</value>
    ''' <returns></returns>
    ''' <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/ms633494(v=vs.85).aspx</remarks>
    Public Shared ReadOnly Property EnumChildWindows(ByVal hWndParent As IntPtr, ByVal lpEnumFunc As EnumCallBackDelegate, ByVal lParam As IntPtr) As Int32
        Get
            Return UnsafeNativeMethods.EnumChildWindows(hWndParent, lpEnumFunc, lParam)
        End Get
    End Property

    ''' <summary>
    ''' Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
    ''' </summary>
    ''' <param name="hWnd">A handle to the window.</param>
    ''' <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the hWndInsertAfterFlags.</param>
    ''' <param name="X">The new position of the left side of the window, in client coordinates.</param>
    ''' <param name="Y">The new position of the top of the window, in client coordinates.</param>
    ''' <param name="cx">The new width of the window, in pixels.</param>
    ''' <param name="cy">The new height of the window, in pixels.</param>
    ''' <param name="uFlags">The window sizing and positioning flags.</param>
    ''' <value>Boolean</value>
    ''' <returns>If the function succeeds, the return value is nonzero.</returns>
    ''' <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx</remarks>
    Public Shared ReadOnly Property SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As hWndInsertAfterFlags, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As Boolean
        Get
            Return UnsafeNativeMethods.SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags)
        End Get
    End Property

    'Public Shared ReadOnly Property BringWindowToTop(ByVal hwnd As IntPtr) As Boolean
    '    Get
    '        Return UnsafeNativeMethods.BringWindowToTop(hwnd)
    '    End Get
    'End Property
    'Public Shared ReadOnly Property GetActiveWindow() As IntPtr
    '    Get
    '        Return UnsafeNativeMethods.GetActiveWindow()
    '    End Get
    'End Property

    ''' <summary>
    ''' Releases the mouse capture from a window in the current thread and restores normal mouse input processing. 
    ''' A window that has captured the mouse receives all mouse input, regardless of the position of the cursor, 
    ''' except when a mouse button is clicked while the cursor hot spot is in the window of another thread.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ReleaseCapture() As Boolean
        Get
            Return UnsafeNativeMethods.ReleaseCapture()
        End Get
    End Property
End Class
