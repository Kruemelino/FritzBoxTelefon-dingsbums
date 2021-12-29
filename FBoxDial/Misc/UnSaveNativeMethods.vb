Imports System.Security
Imports System.Runtime.InteropServices

Friend Delegate Function LowLevelKeyboardProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As Integer

''' <summary>
''' The window sizing and positioning flags.
''' </summary>
''' <remarks></remarks>
<Flags> Friend Enum SetWindowPosFlags As Integer
    ''' <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
    ''' the system posts the request to the thread that owns the window. This prevents the calling thread from 
    ''' blocking its execution while other threads process the request.</summary>
    ''' <remarks>SWP_ASYNCWINDOWPOS</remarks>
    SynchronousWindowPosition = &H4000

    ''' <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
    ''' <remarks>SWP_DEFERERASE</remarks>
    DeferErase = &H2000

    ''' <summary>Draws a frame (defined in the window's class description) around the window.</summary>
    ''' <remarks>SWP_DRAWFRAME</remarks>
    DrawFrame = &H20

    ''' <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
    ''' the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
    ''' is sent only when the window's size is being changed.</summary>
    ''' <remarks>SWP_FRAMECHANGED</remarks>
    FrameChanged = &H20

    ''' <summary>Hides the window.</summary>
    ''' <remarks>SWP_HIDEWINDOW</remarks>
    HideWindow = &H80

    ''' <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
    ''' top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
    ''' parameter).</summary>
    ''' <remarks>SWP_NOACTIVATE</remarks>
    DoNotActivate = &H10

    ''' <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
    ''' contents of the client area are saved and copied back into the client area after the window is sized or 
    ''' repositioned.</summary>
    ''' <remarks>SWP_NOCOPYBITS</remarks>
    DoNotCopyBits = &H100

    ''' <summary>Retains the current position (ignores X and Y parameters).</summary>
    ''' <remarks>SWP_NOMOVE</remarks>
    IgnoreMove = &H2

    ''' <summary>Does not change the owner window's position in the Z order.</summary>
    ''' <remarks>SWP_NOOWNERZORDER</remarks>
    DoNotChangeOwnerZOrder = &H200

    ''' <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
    ''' the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
    ''' window uncovered as a result of the window being moved. When this flag is set, the application must 
    ''' explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
    ''' <remarks>SWP_NOREDRAW</remarks>
    DoNotRedraw = &H8

    ''' <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
    ''' <remarks>SWP_NOREPOSITION</remarks>
    DoNotReposition = &H200 'Danke an Pikachu für den Tipp :)

    ''' <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
    ''' <remarks>SWP_NOSENDCHANGING</remarks>
    DoNotSendChangingEvent = &H400

    ''' <summary>Retains the current size (ignores the cx and cy parameters).</summary>
    ''' <remarks>SWP_NOSIZE</remarks>
    IgnoreResize = &H1

    ''' <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
    ''' <remarks>SWP_NOZORDER</remarks>
    IgnoreZOrder = &H4

    ''' <summary>Displays the window.</summary>
    ''' <remarks>SWP_SHOWWINDOW</remarks>
    ShowWindow = &H40
End Enum

''' <summary>
''' The SetWindowPos hWndInsertAfter positioning flags.
''' </summary>
''' <remarks></remarks>
<Flags> Friend Enum HWndInsertAfterFlags As Integer
    ''' <summary>
    ''' Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
    ''' </summary>
    HWND_BOTTOM = 1

    ''' <summary>
    ''' Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
    ''' </summary>
    HWND_NOTOPMOST = -2

    ''' <summary>
    ''' Places the window at the top of the Z order. (Equal to HWND_TOP)
    ''' </summary>
    None = 0

    ''' <summary>
    ''' Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
    ''' </summary>
    HWND_TOPMOST = -1
End Enum

<SuppressUnmanagedCodeSecurity()> Friend NotInheritable Class UnsafeNativeMethods
    ''' <summary>
    '''     Retrieves a handle to the foreground window (the window with which the user Is currently working). The system
    '''     assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
    '''     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633505%28v=vs.85%29.aspx for more information.</para>
    ''' </summary>
    ''' <returns>
    '''     C++ ( Type: Type: HWND )<br /> The return value Is a handle to the foreground window. The foreground window
    '''     can be NULL in certain circumstances, such as when a window Is losing activation.
    ''' </returns>
    <DllImport("user32.dll", EntryPoint:="GetForegroundWindow", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Friend Shared Function GetForegroundWindow() As IntPtr
    End Function

    ''' <summary>
    '''     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a
    '''     control, the text of the control Is copied. However, GetWindowText cannot retrieve the text of a control in another
    '''     application.
    '''     <para>
    '''     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520%28v=vs.85%29.aspx  for more
    '''     information
    '''     </para>
    ''' </summary>
    ''' <param name="hWnd">
    '''     C++ ( hWnd [in]. Type: HWND )<br />A <see cref="IntPtr" /> handle to the window Or control containing the text.
    ''' </param>
    ''' <param name="lpString">
    '''     C++ (lpString [out]. Type: LPTSTR )<br />The <see cref="StringBuilder" /> buffer that will receive the text. If
    '''     the string Is as long Or longer than the buffer, the string Is truncated And terminated with a null character.
    ''' </param>
    ''' <param name="cch">
    '''     C++ ( cch [in]. Type: int )<br /> Should be equivalent to
    '''     <see cref="StringBuilder.Length" /> after call returns. The <see cref="int" /> maximum number of characters to copy
    '''     to the buffer, including the null character. If the text exceeds this limit, it Is truncated.
    ''' </param>
    ''' <returns>
    '''     If the function succeeds, the return value Is the length, in characters, of the copied string, Not including
    '''     the terminating null character. If the window has no title bar Or text, if the title bar Is empty, Or if the window
    '''     Or control handle Is invalid, the return value Is zero. To get extended error information, call GetLastError.<br />
    '''     This function cannot retrieve the text of an edit control in another application.
    ''' </returns>
    ''' <remarks>
    '''     If the target window Is owned by the current process, GetWindowText causes a WM_GETTEXT message to be sent to the
    '''     specified window Or control. If the target window Is owned by another process And has a caption, GetWindowText
    '''     retrieves the window caption text. If the window does Not have a caption, the return value Is a null string. This
    '''     behavior Is by design. It allows applications to call GetWindowText without becoming unresponsive if the process
    '''     that owns the target window Is Not responding. However, if the target window Is Not responding And it belongs to
    '''     the calling application, GetWindowText will cause the calling application to become unresponsive. To retrieve the
    '''     text of a control in another process, send a WM_GETTEXT message directly instead of calling GetWindowText.<br />For
    '''     an example go to
    '''     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">
    '''     Sending a
    '''     Message.
    '''     </see>
    ''' </remarks>
    <DllImport("user32.dll", EntryPoint:="GetWindowText", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Friend Shared Function GetWindowText(hwnd As IntPtr, lpString As String, cch As Int32) As Int32
    End Function

    '<DllImport("user32.dll", EntryPoint:="GetWindowRect", SetLastError:=True, CharSet:=CharSet.Unicode)>
    'Friend Shared Function GetWindowRect(hWnd As IntPtr, ByRef lpRect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
    'End Function

    '''' <summary>
    ''''     Retrieves a handle to the Shell's desktop window.
    ''''     <para>
    ''''     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633512%28v=vs.85%29.aspx for more
    ''''     information
    ''''     </para>
    '''' </summary>
    '''' <returns>
    ''''     C++ (Type: HWND )<br />The return value Is the handle of the Shell's desktop window. If no Shell process is
    ''''     present, the return value Is NULL.
    '''' </returns>
    '<DllImport("user32.dll", EntryPoint:="GetShellWindow", SetLastError:=True, CharSet:=CharSet.Unicode)>
    'Friend Shared Function GetShellWindow() As IntPtr
    'End Function

    '''' <summary>
    '''' The GetDesktopWindow function returns a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
    '''' </summary>
    '<DllImport("user32.dll", EntryPoint:="GetDesktopWindow", SetLastError:=True, CharSet:=CharSet.Unicode)>
    'Friend Shared Function GetDesktopWindow() As IntPtr
    'End Function

    ''' <summary>
    '''     Changes the size, position, And Z order of a child, pop-up, Or top-level window. These windows are ordered
    '''     according to their appearance on the screen. The topmost window receives the highest rank And Is the first window
    '''     in the Z order.
    '''     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx for more information.</para>
    ''' </summary>
    ''' <param name="hWnd">C++ (hWnd [in]. Type: HWND )<br />A handle to the window.</param>
    ''' <param name="hWndInsertAfter">
    '''     C++ ( hWndInsertAfter [in, optional]. Type: HWND )<br />A handle to the window to precede the positioned window in
    '''     the Z order. This parameter must be a window handle Or one of the following values.
    '''     <list type="table">
    '''     <itemheader>
    '''         <term>HWND placement</term><description>Window to precede placement</description>
    '''     </itemheader>
    '''     <item>
    '''         <term>HWND_BOTTOM ((HWND)1)</term>
    '''         <description>
    '''         Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost
    '''         window, the window loses its topmost status And Is placed at the bottom of all other windows.
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>HWND_NOTOPMOST ((HWND)-2)</term>
    '''         <description>
    '''         Places the window above all non-topmost windows (that Is, behind all topmost windows). This
    '''         flag has no effect if the window Is already a non-topmost window.
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>HWND_TOP ((HWND)0)</term><description>Places the window at the top of the Z order.</description>
    '''     </item>
    '''     <item>
    '''         <term>HWND_TOPMOST ((HWND)-1)</term>
    '''         <description>
    '''         Places the window above all non-topmost windows. The window maintains its topmost position
    '''         even when it Is deactivated.
    '''         </description>
    '''     </item>
    '''     </list>
    '''     <para>For more information about how this parameter Is used, see the following Remarks section.</para>
    ''' </param>
    ''' <param name="X">C++ ( X [in]. Type: int )<br />The New position of the left side of the window, in client coordinates.</param>
    ''' <param name="Y">C++ ( Y [in]. Type: int )<br />The New position of the top of the window, in client coordinates.</param>
    ''' <param name="cx">C++ ( cx [in]. Type: int )<br />The New width of the window, in pixels.</param>
    ''' <param name="cy">C++ ( cy [in]. Type: int )<br />The New height of the window, in pixels.</param>
    ''' <param name="uFlags">
    '''     C++ ( uFlags [in]. Type: UINT )<br />The window sizing And positioning flags. This parameter can be a combination
    '''     of the following values.
    '''     <list type="table">
    '''     <itemheader>
    '''         <term>HWND sizing And positioning flags</term>
    '''         <description>Where to place And size window. Can be a combination of any</description>
    '''     </itemheader>
    '''     <item>
    '''         <term>SWP_ASYNCWINDOWPOS (0x4000)</term>
    '''         <description>
    '''         If the calling thread And the thread that owns the window are attached to different input
    '''         queues, the system posts the request to the thread that owns the window. This prevents the calling
    '''         thread from blocking its execution while other threads process the request.
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_DEFERERASE (0x2000)</term>
    '''         <description>Prevents generation of the WM_SYNCPAINT message. </description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_DRAWFRAME (0x0020)</term>
    '''         <description>Draws a frame (defined in the window's class description) around the window.</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_FRAMECHANGED (0x0020)</term>
    '''         <description>
    '''         Applies New frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message
    '''         to the window, even if the window's size is not being changed. If this flag is not specified,
    '''         WM_NCCALCSIZE Is sent only when the window's size is being changed
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_HIDEWINDOW (0x0080)</term><description>Hides the window.</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOACTIVATE (0x0010)</term>
    '''         <description>
    '''         Does Not activate the window. If this flag Is Not set, the window Is activated And moved to
    '''         the top of either the topmost Or non-topmost group (depending on the setting of the hWndInsertAfter
    '''         parameter).
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOCOPYBITS (0x0100)</term>
    '''         <description>
    '''         Discards the entire contents of the client area. If this flag Is Not specified, the valid
    '''         contents of the client area are saved And copied back into the client area after the window Is sized Or
    '''         repositioned.
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOMOVE (0x0002)</term>
    '''         <description>Retains the current position (ignores X And Y parameters).</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOOWNERZORDER (0x0200)</term>
    '''         <description>Does Not change the owner window's position in the Z order.</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOREDRAW (0x0008)</term>
    '''         <description>
    '''         Does Not redraw changes. If this flag Is set, no repainting of any kind occurs. This applies
    '''         to the client area, the nonclient area (including the title bar And scroll bars), And any part of the
    '''         parent window uncovered as a result of the window being moved. When this flag Is set, the application
    '''         must explicitly invalidate Or redraw any parts of the window And parent window that need redrawing.
    '''         </description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOREPOSITION (0x0200)</term><description>Same as the SWP_NOOWNERZORDER flag.</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOSENDCHANGING (0x0400)</term>
    '''         <description>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOSIZE (0x0001)</term>
    '''         <description>Retains the current size (ignores the cx And cy parameters).</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_NOZORDER (0x0004)</term>
    '''         <description>Retains the current Z order (ignores the hWndInsertAfter parameter).</description>
    '''     </item>
    '''     <item>
    '''         <term>SWP_SHOWWINDOW (0x0040)</term><description>Displays the window.</description>
    '''     </item>
    '''     </list>
    ''' </param>
    ''' <returns><c>true</c> Or nonzero if the function succeeds, <c>false</c> Or zero otherwise Or if function fails.</returns>
    ''' <remarks>
    '''     <para>
    '''         As part of the Vista re-architecture, all services were moved off the interactive desktop into Session 0.
    '''         hwnd And window manager operations are only effective inside a session And cross-session attempts to manipulate
    '''         the hwnd will fail. For more information, see The Windows Vista Developer Story: Application Compatibility
    '''         Cookbook.
    '''     </para>
    '''     <para>
    '''         If you have changed certain window data using SetWindowLong, you must call SetWindowPos for the changes to
    '''         take effect. Use the following combination for uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
    '''         SWP_FRAMECHANGED.
    '''     </para>
    '''     <para>
    '''         A window can be made a topmost window either by setting the hWndInsertAfter parameter to HWND_TOPMOST And
    '''         ensuring that the SWP_NOZORDER flag Is Not set, Or by setting a window's position in the Z order so that it is
    '''         above any existing topmost windows. When a non-topmost window Is made topmost, its owned windows are also made
    '''         topmost. Its owners, however, are Not changed.
    '''     </para>
    '''     <para>
    '''         If neither the SWP_NOACTIVATE nor SWP_NOZORDER flag Is specified (that Is, when the application requests that
    '''         a window be simultaneously activated And its position in the Z order changed), the value specified in
    '''         hWndInsertAfter Is used only in the following circumstances.
    '''     </para>
    '''         <list type="bullet">
    '''         <item>Neither the HWND_TOPMOST nor HWND_NOTOPMOST flag Is specified in hWndInsertAfter. </item>
    '''         <item>The window identified by hWnd Is Not the active window. </item>
    '''     </list>
    '''     <para>
    '''         An application cannot activate an inactive window without also bringing it to the top of the Z order.
    '''         Applications can change an activated window's position in the Z order without restrictions, or it can activate
    '''         a window And then move it to the top of the topmost Or non-topmost windows.
    '''     </para>
    '''     <para>
    '''         If a topmost window Is repositioned to the bottom (HWND_BOTTOM) of the Z order Or after any non-topmost
    '''         window, it Is no longer topmost. When a topmost window Is made non-topmost, its owners And its owned windows
    '''         are also made non-topmost windows.
    '''     </para>
    '''     <para>
    '''         A non-topmost window can own a topmost window, but the reverse cannot occur. Any window (for example, a
    '''         dialog box) owned by a topmost window Is itself made a topmost window, to ensure that all owned windows stay
    '''         above their owner.
    '''     </para>
    '''     <para>
    '''         If an application Is Not in the foreground, And should be in the foreground, it must call the
    '''         SetForegroundWindow function.
    '''     </para>
    '''     <para>
    '''         To use SetWindowPos to bring a window to the top, the process that owns the window must have
    '''         SetForegroundWindow permission.
    '''     </para>
    ''' </remarks>
    <DllImport("user32.dll", EntryPoint:="SetWindowPos", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Friend Shared Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As HWndInsertAfterFlags, X As Integer, Y As Integer, cx As Integer, cy As Integer, uFlags As SetWindowPosFlags) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Friend Shared Function SetWindowsHookEx(idHook As Integer, lpfn As LowLevelKeyboardProc, hMod As IntPtr, ByVal dwThreadId As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function UnhookWindowsHookEx(hhk As IntPtr) As Boolean
    End Function

    ''' <summary>
    '''     Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this
    '''     function either before Or after processing the hook information.
    '''     <para>
    '''     See [ https://msdn.microsoft.com/en-us/library/windows/desktop/ms644974%28v=vs.85%29.aspx ] for more
    '''     information.
    '''     </para>
    ''' </summary>
    ''' <param name="hhk">C++ ( hhk [in, optional]. Type: HHOOK )<br />This parameter Is ignored. </param>
    ''' <param name="nCode">
    '''     C++ ( nCode [in]. Type: int )<br />The hook code passed to the current hook procedure. The next
    '''     hook procedure uses this code to determine how to process the hook information.
    ''' </param>
    ''' <param name="wParam">
    '''     C++ ( wParam [in]. Type: WPARAM )<br />The wParam value passed to the current hook procedure. The
    '''     meaning of this parameter depends on the type of hook associated with the current hook chain.
    ''' </param>
    ''' <param name="lParam">
    '''     C++ ( lParam [in]. Type: LPARAM )<br />The lParam value passed to the current hook procedure. The
    '''     meaning of this parameter depends on the type of hook associated with the current hook chain.
    ''' </param>
    ''' <returns>
    '''     C++ ( Type: LRESULT )<br />This value Is returned by the next hook procedure in the chain. The current hook
    '''     procedure must also return this value. The meaning of the return value depends on the hook type. For more
    '''     information, see the descriptions of the individual hook procedures.
    ''' </returns>
    ''' <remarks>
    '''     <para>
    '''     Hook procedures are installed in chains for particular hook types. <see cref="CallNextHookEx" /> calls the
    '''     next hook in the chain.
    '''     </para>
    '''     <para>
    '''     Calling CallNextHookEx Is optional, but it Is highly recommended; otherwise, other applications that have
    '''     installed hooks will Not receive hook notifications And may behave incorrectly as a result. You should call
    '''     <see cref="CallNextHookEx" /> unless you absolutely need to prevent the notification from being seen by other
    '''     applications.
    '''     </para>
    ''' </remarks>
    <DllImport("user32.dll")>
    Friend Shared Function CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function GetKeyState(nVirtKey As Integer) As Short
    End Function
End Class

Public NotInheritable Class UnSaveMethods

    ''' <summary>
    ''' Retrieves a handle to the foreground window (the window with which the user is currently working). 
    ''' The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
    ''' </summary>
    ''' <value>IntPtr</value>
    ''' <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
    Public Shared ReadOnly Property GetForegroundWindow() As IntPtr = UnsafeNativeMethods.GetForegroundWindow()

    ''' <summary>
    ''' Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied. 
    ''' However, GetWindowText cannot retrieve the text of a control in another application.
    ''' </summary>
    ''' <param name="hwnd">A handle to the window or control containing the text.</param>
    ''' <returns>If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character.
    ''' If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.
    ''' This function cannot retrieve the text of an edit control in another application.</returns>
    Public Shared ReadOnly Property GetWindowText(hwnd As IntPtr) As String
        Get
            Dim lpString As String = Space(255)
            Return Left(lpString, UnsafeNativeMethods.GetWindowText(hwnd, lpString, Len(lpString)))
        End Get
    End Property

    '''' <summary>
    '''' Retrieves a handle to the Shell's desktop window.
    '''' </summary>
    '''' <value>IntPtr</value>
    '''' <returns>The return value is the handle of the Shell's desktop window. If no Shell process is present, the return value is NULL.</returns>
    'Public Shared ReadOnly Property GetShellWindow() As IntPtr = UnsafeNativeMethods.GetShellWindow()

    '''' <summary>
    '''' The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
    '''' </summary>
    '''' <returns>The GetDesktopWindow function returns a handle to the desktop window.</returns>
    'Public Shared ReadOnly Property GetDesktopWindow As IntPtr = UnsafeNativeMethods.GetDesktopWindow()

    '''' <summary>
    '''' Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    '''' </summary>
    '''' <param name="hwnd">A handle to the window.</param>
    '''' <value>A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</value>
    '''' <returns>If the function succeeds, the return value is nonzero.
    '''' If the function fails, the return value is zero.</returns>
    '''' <remarks>Retrieves the dimensions of the bounding rectangle of the specified window. 
    '''' The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    '''' The Win32 RECT is not binary compatible with System.Drawing.Rectangle.</remarks>
    'Public Shared ReadOnly Property GetWindowRect(hwnd As IntPtr) As RECT
    '    Get
    '        Dim lpRect As RECT
    '        UnsafeNativeMethods.GetWindowRect(hwnd, lpRect)
    '        Return lpRect
    '    End Get
    'End Property

    Friend Shared ReadOnly Property CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        Get
            Return UnsafeNativeMethods.CallNextHookEx(hhk, nCode, wParam, lParam)
        End Get
    End Property

    Friend Shared ReadOnly Property UnhookWindowsHookEx(hhk As IntPtr) As Boolean
        Get
            Return UnsafeNativeMethods.UnhookWindowsHookEx(hhk)
        End Get
    End Property

    Friend Shared ReadOnly Property SetWindowsHookEx(idHook As Integer, lpfn As LowLevelKeyboardProc, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
        Get
            Return UnsafeNativeMethods.SetWindowsHookEx(idHook, lpfn, hMod, dwThreadId)
        End Get
    End Property

    Friend Shared ReadOnly Property GetKeyState(nVirtKey As Integer) As Short
        Get
            Return UnsafeNativeMethods.GetKeyState(nVirtKey)
        End Get
    End Property
End Class

