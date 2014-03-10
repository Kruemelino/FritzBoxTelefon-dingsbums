Friend Class C_Telefonat
    Friend Enum JournalTyp
        Eingehend = 1
        Ausgehend = 0
    End Enum

#Region "Eigene Private Variablen"
    Private _ID As Integer
    Private _Typ As JournalTyp
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
    Private _olContact As Outlook.ContactItem
    Private _vCard As String
    Private _Anrufer As String
    Private _TelName As String
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
    Friend Property Typ() As JournalTyp
        Get
            Return _Typ
        End Get
        Set(ByVal value As JournalTyp)
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
    Friend ReadOnly Property OutlookApplication() As Outlook.Application
        Get
            Return ThisAddIn.P_oApp
        End Get
    End Property
#End Region

    Friend Function ErstelleJournalEintrag(Telefonat As C_Telefonat) As Boolean
        ErstelleJournalEintrag = Nothing
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication()
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
                    If (Not (Telefonat.KontaktID = C_DP.P_Def_StringEmpty Or Telefonat.StoreID = C_DP.P_Def_StringEmpty)) And Not Left(Telefonat.KontaktID, 2) = C_DP.P_Def_ErrorMinusOne Then
                        Try
                            .Links.Add(CType(oApp.GetNamespace("MAPI").GetItemFromID(Telefonat.KontaktID, Telefonat.StoreID), Outlook.ContactItem))
                        Catch ex As Exception
                            C_hf.LogFile("Fehler (ErstelleJournalEintrag): Kann eingebetteten Link zum Kontakt nicht erstellen: " & ex.Message)
                        End Try
                    End If
#End If
                    .Save()
                    .Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olSave)
                End With
                olJournal = Nothing
            End If
        Else
            C_hf.LogFile("Journaleintrag konnte nicht erstellt werden.")
        End If
        oApp = Nothing
    End Function

    Public Sub New(ByVal KontaktKlasse As Contacts, ByVal Helferklasse As Helfer, ByVal DataProviderKlasse As DataProvider, ByVal inipfad As String)
        C_hf = Helferklasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse

        C_KF.C_OLI = Me
    End Sub

    Friend Function NeuEmail(ByRef tmpFile As String, ByRef XMLFile As String, ByRef BodyString As String) As Boolean
        Dim olMail As Outlook.MailItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication()
        If Not oApp Is Nothing Then
            Try
                olMail = CType(oApp.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
            Catch ex As Exception
                C_hf.LogFile("ErstelleJournalItem: " & ex.Message)
            End Try
            If Not olMail Is Nothing Then
                With olMail
                    .Attachments.Add(tmpFile)
                    .Attachments.Add(XMLFile)
                    Try
                        .Attachments.Add(C_hf.Dateipfade("LogDatei"))
                    Catch ex As Exception
                        .Body = vbNewLine & "Log wird nicht geschrieben."
                    End Try

                    .Subject = "Einleseproblem der Telefone im Fritz!Box Telefon-dingsbums"
                    My.Computer.FileSystem.DeleteFile(tmpFile)
                    .Body = String.Concat( _
                        BodyString, _
                        "Outlook-Version: ", oApp.Version, vbNewLine, _
                        "Fritz!Box Telefon-dingsbums-Version: ", ThisAddIn.Version, .Body)
                    .To = "kruemelino@gert-michael.de"
                    .Display()
                End With
                olMail = Nothing
            End If
        Else
            C_hf.LogFile("E-Mail konnte nicht erstellt werden.")
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
    Friend Sub InspectorVerschieben(ByVal r As Boolean)
        Dim oApp As Outlook.Application = OutlookApplication()
        If Not oApp Is Nothing Then
            If r Then
                If ActiveFensterIsOutlook() And oApp.ActiveWindow Is oApp.ActiveInspector Then
                    OInsp = oApp.ActiveInspector()
                End If
            Else
                Try
                    If Not OInsp Is Nothing Then
                        If Not OInsp.WindowState = Outlook.OlWindowState.olMinimized Then
                            OInsp.Activate()
                            OInsp = Nothing
                        End If
                    End If
                Catch : End Try
            End If
        Else
            C_hf.LogFile("Inspectorfenster konnte nicht verschoben werden.")
        End If
        oApp = Nothing
    End Sub

    'Aktive Fenster ermitteln & Handle des Aktiven Fenster ermitteln
    Private Function ActiveFensterIsOutlook() As Boolean
        'Fenster Name des Fensters mit Fokus ermitteln: OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow)
        'Fenster Name des Outlook Fenster ermitteln: OutlookSecurity.FindWindowEX(IntPtr.Zero, IntPtr.Zero, "rctrl_renwnd32", C_DP.P_Def_StringEmpty)
        Return OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow) = OutlookSecurity.GetWindowText(OutlookSecurity.FindWindowEX(IntPtr.Zero, IntPtr.Zero, "rctrl_renwnd32", C_DP.P_Def_StringEmpty))
    End Function

    Function VollBildAnwendungAktiv() As Boolean
        Dim desktopHandle As IntPtr = OutlookSecurity.GetDesktopWindow()
        Dim shellHandle As IntPtr = OutlookSecurity.GetShellWindow()
        'Detect if the current app is running in full screen

        Dim AppBounds As RECT
        Dim screenBounds As System.Drawing.Rectangle
        Dim hWnd As IntPtr

        VollBildAnwendungAktiv = False

        'get the dimensions of the active window
        hWnd = OutlookSecurity.GetForegroundWindow()
        If Not hWnd = IntPtr.Zero Then
            ' Check we haven't picked up the desktop or the shell
            If Not (hWnd.Equals(desktopHandle) Or hWnd.Equals(shellHandle)) Then
                AppBounds = OutlookSecurity.GetWindowRect(hWnd)
                'Return String.Concat(irect.top, ";", irect.bottom, ";", irect.left, ";", irect.right)
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
    Friend Overloads Sub KontaktOrdnerInTreeView(ByVal TreeView As Windows.Forms.TreeView)
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
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

