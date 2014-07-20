Friend Class C_Telefonat
    Friend Enum AnrufRichtung
        Eingehend = 1
        Ausgehend = 0
    End Enum

#Region "Eigene Private Variablen"
    Private _ID As Integer
    Private _Typ As AnrufRichtung
    Private _Zeit As Date
    Private _MSN As String
    Private _TelNr As String
    Private _KontaktID As String
    Private _StoreID As String
    Private _Dauer As Integer
    Private _NSN As Long
    Private _Subject As String
    Private _Body As String
    Private _Companies As String
    Private _Categories As String
    'Private WithEvents _olContact As Outlook.ContactItem
    Private _olContact As Outlook.ContactItem
    Private _vCard As String
    Private _Anrufer As String
    Private _TelName As String
    Private _frm_Popup As Popup
    Private _Angenommen As Boolean = False
#End Region

#Region "Properties"
    Friend Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property
    Friend Property Typ() As AnrufRichtung
        Get
            Return _Typ
        End Get
        Set(ByVal value As AnrufRichtung)
            _Typ = value
        End Set
    End Property
    Friend Property Zeit() As Date
        Get
            Return _Zeit
        End Get
        Set(ByVal value As Date)
            _Zeit = value
        End Set
    End Property
    Friend Property MSN() As String
        Get
            Return _MSN
        End Get
        Set(ByVal value As String)
            _MSN = value
        End Set
    End Property
    Friend Property TelNr() As String
        Get
            Return _TelNr
        End Get
        Set(ByVal value As String)
            _TelNr = value
        End Set
    End Property
    Friend Property KontaktID() As String
        Get
            Return _KontaktID
        End Get
        Set(ByVal value As String)
            _KontaktID = value
        End Set
    End Property
    Friend Property StoreID() As String
        Get
            Return _StoreID
        End Get
        Set(ByVal value As String)
            _StoreID = value
        End Set
    End Property
    Friend Property Dauer() As Integer
        Get
            Return _Dauer
        End Get
        Set(ByVal value As Integer)
            _Dauer = value
        End Set
    End Property
    Friend Property NSN() As Long
        Get
            Return _NSN
        End Get
        Set(ByVal value As Long)
            _NSN = value
        End Set
    End Property
    Friend Property Subject() As String
        Get
            Return _Subject
        End Get
        Set(ByVal value As String)
            _Subject = value
        End Set
    End Property
    Friend Property Body() As String
        Get
            Return _Body
        End Get
        Set(ByVal value As String)
            _Body = value
        End Set
    End Property
    Friend Property Categories() As String
        Get
            Return _Categories
        End Get
        Set(ByVal value As String)
            _Categories = value
        End Set
    End Property
    Friend Property Companies() As String
        Get
            Return _Companies
        End Get
        Set(ByVal value As String)
            _Companies = value
        End Set
    End Property
    Friend Property olContact() As Outlook.ContactItem
        Get
            Return _olContact
        End Get
        Set(ByVal value As Outlook.ContactItem)
            _olContact = value
        End Set
    End Property
    Friend Property vCard() As String
        Get
            Return _vCard
        End Get
        Set(ByVal value As String)
            _vCard = value
        End Set
    End Property
    Friend Property Anrufer() As String
        Get
            Return _Anrufer
        End Get
        Set(ByVal value As String)
            _Anrufer = value
        End Set
    End Property
    Friend Property TelName() As String
        Get
            Return _TelName
        End Get
        Set(ByVal value As String)
            _TelName = value
        End Set
    End Property
    Friend Property FormAnrMon() As Popup
        Get
            Return _frm_Popup
        End Get
        Set(ByVal value As Popup)
            _frm_Popup = value
        End Set
    End Property
    Friend Property Angenommen() As Boolean
        Get
            Return _Angenommen
        End Get
        Set(ByVal value As Boolean)
            _Angenommen = value
        End Set
    End Property
#End Region

End Class

Public Class OutlookInterface

#Region "Eigene Klassen"
    Private C_KF As Contacts
    Private C_hf As Helfer
    Private C_DP As DataProvider
#End Region

#Region "Globale Variablen"
    Private OInsp As Outlook.Inspector
#End Region

#Region "Properties"
    Friend ReadOnly Property OutlookApplication As Outlook.Application
        Get
            Return ThisAddIn.P_oApp
        End Get
    End Property
#End Region
    Public Sub New(ByVal KontaktKlasse As Contacts, ByVal Helferklasse As Helfer, ByVal DataProviderKlasse As DataProvider)
        C_hf = Helferklasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse
        C_KF.C_OLI = Me
    End Sub

    Friend Function ErstelleJournalEintrag(Telefonat As C_Telefonat) As Boolean
        ErstelleJournalEintrag = Nothing
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication
        If Not oApp Is Nothing Then
            Try
                olJournal = CType(oApp.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
            Catch ex As Exception
                C_hf.LogFile("ErstelleJournalItem: " & ex.Message)
            End Try
            If Not olJournal Is Nothing Then
                With olJournal
                    .Subject = Telefonat.Subject
                    .Duration = Telefonat.Dauer
                    .Body = Telefonat.Body
                    .Start = Telefonat.Zeit
                    .Companies = Telefonat.Companies
                    .Categories = Telefonat.Categories

#If Not OVer = 15 Then
                    If (Not (Telefonat.KontaktID = C_DP.P_Def_StringEmpty Or Telefonat.StoreID = C_DP.P_Def_StringEmpty)) And Not _
                        Left(Telefonat.KontaktID, 2) = C_DP.P_Def_ErrorMinusOne_String Then
                        .Links.Add(Telefonat.olContact)
                    End If
#End If
                    .Save()
                    '.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olSave)
                End With
                olJournal = Nothing
            End If
        Else
            C_hf.LogFile("Journaleintrag konnte nicht erstellt werden.")
        End If
        oApp = Nothing
    End Function

    Friend Function NeueEmail(ByRef tmpFile As String, ByRef XMLFile As String, ByRef BodyString As String) As Boolean
        Dim olMail As Outlook.MailItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication
        If Not oApp Is Nothing Then

            Try
                olMail = CType(oApp.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
            Catch ex As Exception
                C_hf.LogFile("NeueEmail: " & ex.Message)
            End Try

            If Not olMail Is Nothing Then
                With olMail
                    .Attachments.Add(tmpFile)
                    .Attachments.Add(XMLFile)
                    Try
                        .Attachments.Add(C_DP.P_Arbeitsverzeichnis & C_DP.P_Def_Log_FileName)
                    Catch
                        .Body = vbNewLine & "Log wird nicht geschrieben."
                    End Try

                    .Subject = "Einleseproblem der Telefone im Fritz!Box Telefon-dingsbums"
                    My.Computer.FileSystem.DeleteFile(tmpFile)
                    .Body = String.Concat( _
                        BodyString, _
                        "Outlook-Version: ", oApp.Version, vbNewLine, _
                        "Fritz!Box Telefon-dingsbums-Version: ", ThisAddIn.Version, .Body)
                    .To = "kruemelino@gert-michael.de"
                    Try
                        .Display()
                    Catch ex As System.Runtime.InteropServices.COMException
                        C_hf.LogFile("NeueEmail: Die E-Mail wurde erstellt, konnte jedoch nicht angezeigt werden.")
                        .Save()
                    End Try

                End With
                olMail = Nothing
            End If
        Else
            C_hf.LogFile("NeueEmail: E-Mail konnte nicht erstellt werden.")
        End If
        oApp = Nothing
        Return True
    End Function

    Friend Function BenutzerInitialien() As String
        Dim Regkey As Microsoft.Win32.RegistryKey = Nothing
        Dim UserInitials As String
        Dim UserName As String
        UserName = "Name"
        UserInitials = "Initialien"
        Try
            '64 Bit prüfen!
#If OVer = 11 Then
            'Regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Office\11.0\Common\UserInfo")
            'UserName = System.Text.Encoding.Unicode.GetString(CType(Regkey.GetValue("UserName"), Byte()))
            'UserInitials = System.Text.Encoding.Unicode.GetString(CType(Regkey.GetValue("UserInitials"), Byte()))
            UserInitials = "Name"
#Else
            Regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Office\Common\UserInfo")
            UserName = Regkey.GetValue("UserName", "Name").ToString
            UserInitials = Regkey.GetValue("UserInitials", "Initialien").ToString
#End If
        Catch ex As Exception
            C_hf.LogFile("Fehler beim Zugriff auf die Registry (BenutzerInitialien): " & ex.Message)
        End Try
        If Not Regkey Is Nothing Then Regkey.Close()
        BenutzerInitialien = UserInitials
    End Function

#Region "Fenster"
    ''' <summary>
    ''' Sinn der Routine ist es einen aktiven Inspector wieder aktiv zu schalten, da der Anrufmonitor diesen deaktiviert.
    ''' Nachdem der Anrufmonitor eingeblendet wurde, muss der Inspector wieder aktiviert werden.
    ''' Zuvor müssen zwei Dinge geprüft werden:
    ''' 1. Haut ein Outlookfenster (Inspector) gerade den Focus: (.ActiveWindow Is .ActiveInspector)
    ''' 2. Ist das aktuell aktive Fenster der Inspector (OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow) = .ActiveInspector.Caption)
    ''' 
    ''' Um den ganzen vorgang abschließen zu können, wird der Inspector zwischengespeichert und nachdem der Anrufmonitor eingeblendet wurde wieder aktiviert.
    ''' </summary>
    ''' <param name="Activate">Gibt an, ob der Inspector aktiviert werden soll (true) oder ob er gespeichert werden soll (false)</param>
    ''' <remarks></remarks>
    Friend Sub KeepoInspActivated(ByVal Activate As Boolean)

        If Not OutlookApplication Is Nothing Then
            If Activate Then
                If Not OInsp Is Nothing Then
                    If Not OInsp.WindowState = Outlook.OlWindowState.olMinimized Then
                        OInsp.Activate()
                        OInsp = Nothing
                    End If
                End If
            Else
                If OInsp Is Nothing Then
                    With OutlookApplication
                        If .ActiveWindow Is .ActiveInspector Then
                            If OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow) = .ActiveInspector.Caption Then
                                OInsp = .ActiveInspector()
                            End If
                        End If
                    End With
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Prüfft ob ein Vollbildanwendung aktiv ist.
    ''' </summary>
    ''' <returns>True, wenn Vollbildanwendung erkannt, fals wenn nicht</returns>
    ''' <remarks></remarks>
    Function VollBildAnwendungAktiv() As Boolean
        'Detect if the current app is running in full screen

        Dim AppBounds As RECT
        Dim screenBounds As System.Drawing.Rectangle
        Dim hWnd As IntPtr

        VollBildAnwendungAktiv = False

        'get the dimensions of the active window
        hWnd = OutlookSecurity.GetForegroundWindow()

        If Not hWnd = IntPtr.Zero Then
            ' Check we haven't picked up the desktop or the shell
            If Not (hWnd.Equals(OutlookSecurity.GetDesktopWindow) Or hWnd.Equals(OutlookSecurity.GetShellWindow)) Then
                AppBounds = OutlookSecurity.GetWindowRect(hWnd)
                'determine if window is fullscreen
                screenBounds = Windows.Forms.Screen.FromHandle(CType(hWnd, IntPtr)).Bounds
                If (AppBounds.bottom - AppBounds.top = screenBounds.Height) And (AppBounds.right - AppBounds.left = screenBounds.Width) Then
                    VollBildAnwendungAktiv = True
                    C_hf.LogFile("Eine aktive Vollbildanwendung wurde detektiert.")
                End If
            End If
        End If
    End Function
#End Region

#Region "TreeView"
    Friend Sub GetKontaktOrdnerInTreeView(ByVal TreeView As Windows.Forms.TreeView)
        Dim olNamespace As Outlook.NameSpace = OutlookApplication.GetNamespace("MAPI")
        Dim TVImageList As Windows.Forms.ImageList
        TVImageList = New Windows.Forms.ImageList
        TVImageList.Images.Add("Kontakt", My.Resources.Bild4_1)
        TVImageList.Images.Add("KontaktSel", My.Resources.Bild4_2)
        TreeView.ImageList = TVImageList
        TreeView.SelectedImageKey = "KontaktSel"
        KontaktOrdnerInTreeView(olNamespace, TreeView)
    End Sub
    Private Overloads Sub KontaktOrdnerInTreeView(ByVal NamensRaum As Outlook.NameSpace, ByVal TreeView As Windows.Forms.TreeView)
        TreeView.Nodes.Add("Kontaktordner")
        '  Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        Dim j As Integer = 1
        Do While (j <= NamensRaum.Folders.Count)
            KontaktOrdnerInTreeView(NamensRaum.Folders.Item(j), TreeView, TreeView.Nodes(0))
            j = j + 1
            Windows.Forms.Application.DoEvents()
        Loop
    End Sub
    Private Overloads Sub KontaktOrdnerInTreeView(ByVal Ordner As Outlook.MAPIFolder, ByVal TreeView As Windows.Forms.TreeView, ByVal BaseNode As Windows.Forms.TreeNode)
        Dim iOrdner As Integer
        Dim SubFolder As Outlook.MAPIFolder
        Dim ChildNode As System.Windows.Forms.TreeNode

        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count)
            SubFolder = Ordner.Folders.Item(iOrdner)
            ChildNode = BaseNode
            If SubFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
                ChildNode = BaseNode.Nodes.Add(SubFolder.EntryID & ";" & SubFolder.StoreID, SubFolder.Name, "Kontakt")
                ChildNode.Tag = SubFolder.EntryID & ";" & SubFolder.StoreID
            End If
            KontaktOrdnerInTreeView(SubFolder, TreeView, ChildNode)
            iOrdner = iOrdner + 1
            Windows.Forms.Application.DoEvents()
        Loop

    End Sub
#End Region
End Class

