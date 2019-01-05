Friend Class C_Telefonat
    Friend Enum AnrufRichtung
        Eingehend = 1
        Ausgehend = 0
    End Enum

#Region "Properties"

    Friend Property ID() As Integer
    Friend Property Typ() As AnrufRichtung
    Friend Property Zeit() As Date
    Friend Property MSN() As String
    Friend Property RingTime() As Double
    Friend Property TelNr() As String
    Friend Property KontaktID() As String
    Friend Property StoreID() As String
    Friend Property Dauer() As Integer
    Friend Property NSN() As Integer
    Friend Property Subject() As String
    Friend Property Body() As String
    Friend Property Categories() As String
    Friend Property Firma() As String
    Friend Property OlContact() As Outlook.ContactItem
    Friend Property vCard() As String
    Friend Property Anrufer() As String
    Friend Property TelName() As String
    Friend Property Angenommen() As Boolean
    Friend Property PopupAnrMon() As F_AnrMon
    Friend Property PopupStoppuhr() As F_StoppUhr
    Friend Property Beendet() As Boolean
    Friend Property Online() As Boolean
    Friend Property Verpasst() As Boolean
    Friend Property AnrMonAusblenden() As Boolean
#End Region

End Class

Public Class OutlookInterface

#Region "Eigene Klassen"
    Private C_KF As KontaktFunktionen
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

    Public Sub New(ByVal KontaktKlasse As KontaktFunktionen, ByVal Helferklasse As Helfer, ByVal DataProviderKlasse As DataProvider)
        C_hf = Helferklasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse
        C_KF.C_OLI = Me
    End Sub

    Friend Function ErstelleJournalEintrag(Telefonat As C_Telefonat) As Boolean
        ErstelleJournalEintrag = Nothing
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication
        If oApp IsNot Nothing Then
            Try
                olJournal = CType(oApp.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
            Catch ex As Exception
                C_hf.LogFile("ErstelleJournalItem: " & ex.Message)
            End Try
            If olJournal IsNot Nothing Then
                With olJournal
                    .Subject = Telefonat.Subject
                    .Duration = Telefonat.Dauer
                    .Body = Telefonat.Body
                    .Start = Telefonat.Zeit
                    .Companies = Telefonat.Firma
                    .Categories = Telefonat.Categories

#If OVer = 14 Then
                    If (Not (Telefonat.KontaktID = DataProvider.P_Def_LeerString Or Telefonat.StoreID = DataProvider.P_Def_LeerString)) And Not _
                        Left(Telefonat.KontaktID, 2) = DataProvider.P_Def_ErrorMinusOne_String Then
                        .Links.Add(Telefonat.olContact)
                    End If
#End If
                    '.Save()
                    .Close(Outlook.OlInspectorClose.olSave)
                End With
                olJournal = Nothing
            End If
        Else
            C_hf.LogFile("Journaleintrag konnte nicht erstellt werden.")
        End If
        oApp = Nothing
    End Function

    Friend Function NeueEmail(ByRef BodyString As String) As Boolean
        Dim olMail As Outlook.MailItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication
        If oApp IsNot Nothing Then
            Try
                olMail = CType(oApp.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
            Catch ex As Exception
                C_hf.LogFile("NeueEmail: " & ex.Message)
            End Try

            If olMail IsNot Nothing Then
                With olMail
                    If C_DP.P_Debug_FBFile IsNot Nothing Then
                        For Each QueryFile As String In C_DP.P_Debug_FBFile
                            If My.Computer.FileSystem.FileExists(QueryFile) Then
                                .Attachments.Add(QueryFile)
                                My.Computer.FileSystem.DeleteFile(QueryFile)
                            End If

                        Next
                    End If
                    ' Einstellungsdatei anfügen
                    If My.Computer.FileSystem.FileExists(C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Config_FileName) Then .Attachments.Add(C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Config_FileName)
                    ' Logdatei anfügen, falls sie geschrieben wird
                    If My.Computer.FileSystem.FileExists(C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Log_FileName) Then
                        .Attachments.Add(C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Log_FileName)
                    Else
                        .Body = DataProvider.P_Def_EineNeueZeile & "Log wird nicht geschrieben." & DataProvider.P_Def_EineNeueZeile
                    End If

                    .Subject = "Einleseproblem der Telefone im " & DataProvider.P_Def_Addin_LangName
                    .Body = String.Concat(BodyString, "Outlook-Version: ", oApp.Version, DataProvider.P_Def_EineNeueZeile, DataProvider.P_Def_Addin_LangName, "-Version: ", ThisAddIn.Version, .Body)
                    .To = DataProvider.P_AddinKontaktMail
                    Try
                        .Display()
                    Catch ex As Runtime.InteropServices.COMException
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
            Regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Office\Common\UserInfo")
            UserName = Regkey.GetValue("UserName", "Name").ToString
            UserInitials = Regkey.GetValue("UserInitials", "Initialien").ToString
        Catch ex As Exception
            C_hf.LogFile("Fehler beim Zugriff auf die Registry (BenutzerInitialien): " & ex.Message)
        End Try
        If Regkey IsNot Nothing Then Regkey.Close()
        BenutzerInitialien = UserInitials
    End Function

    Friend Function GetSmtpAddress(ByVal card As Office.IMsoContactCard) As String
        If card.AddressType = Office.MsoContactCardAddressType.msoContactCardAddressTypeOutlook Then
            'Dim host As Outlook.Application = Globals.ThisAddIn.Application
            Dim ae As Outlook.AddressEntry = OutlookApplication.Session.GetAddressEntryFromID(card.Address)

            Select Case ae.AddressEntryUserType
                Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
                    Dim ex As Outlook.ExchangeUser = ae.GetExchangeUser()
                    Return ex.PrimarySmtpAddress
                Case Outlook.OlAddressEntryUserType.olOutlookContactAddressEntry
                    Return ae.Address
                Case Else
                    Throw New Exception("Valid address entry not found.")
            End Select
        Else
            Return card.Address
        End If
    End Function

#Region "Fenster"
    ''' <summary>
    ''' Sinn der Routine ist es einen aktiven Inspector wieder aktiv zu schalten, da der Anrufmonitor diesen deaktiviert.
    ''' Nachdem der Anrufmonitor eingeblendet wurde, muss der Inspector wieder aktiviert werden.
    ''' Zuvor müssen zwei Dinge geprüft werden:
    ''' 1. Hat ein Outlookfenster (Inspector) gerade den Focus: (.ActiveWindow Is .ActiveInspector)
    ''' 2. Ist das aktuell aktive Fenster der Inspector (OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow) = .ActiveInspector.Caption)
    ''' Um den ganzen vorgang abschließen zu können, wird der Inspector zwischengespeichert und nachdem der Anrufmonitor eingeblendet wurde wieder aktiviert.
    ''' </summary>
    ''' <param name="Activate">Gibt an, ob der Inspector aktiviert werden soll (true) oder ob er gespeichert werden soll (false)</param>
    Friend Sub KeepoInspActivated(ByVal Activate As Boolean)

        If OutlookApplication IsNot Nothing Then
            If Activate Then
                If OInsp IsNot Nothing Then
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
                If (AppBounds.Bottom - AppBounds.Top = screenBounds.Height) And (AppBounds.Right - AppBounds.Left = screenBounds.Width) Then
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
        'Dim TVImageList As Windows.Forms.ImageList
        Dim iOrdner As Integer = 1

        With TreeView
            .Nodes.Add("Kontaktordner")
        End With

        Do While iOrdner <= olNamespace.Folders.Count
            KontaktOrdnerInTreeView(olNamespace.Folders.Item(iOrdner), TreeView, TreeView.Nodes(0))
            iOrdner += 1
            Windows.Forms.Application.DoEvents()
        Loop
    End Sub

    Private Sub KontaktOrdnerInTreeView(ByVal Ordner As Outlook.MAPIFolder, ByVal TreeView As Windows.Forms.TreeView, ByVal BaseNode As Windows.Forms.TreeNode)
        Dim ChildNode As System.Windows.Forms.TreeNode
        Dim iOrdner As Integer = 1
        Dim SubFolder As Outlook.MAPIFolder

        Do While iOrdner <= Ordner.Folders.Count
            SubFolder = Ordner.Folders.Item(iOrdner)
            ChildNode = BaseNode
            If SubFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
                Try
                    ChildNode = BaseNode.Nodes.Add(SubFolder.EntryID & ";" & SubFolder.StoreID, SubFolder.Name, "Kontakt")
                    ChildNode.Tag = ChildNode.Name
                    If ChildNode.Level = 1 Then ChildNode.Text += " (" & Ordner.Name & ")"
                Catch ex As Exception
                    C_hf.LogFile("Auf den Ordner " & SubFolder.Name & " kann nicht zugegriffen werden.")
                    ChildNode = BaseNode
                End Try
            End If
            KontaktOrdnerInTreeView(SubFolder, TreeView, ChildNode)
            iOrdner += 1
            Windows.Forms.Application.DoEvents()
        Loop

        ' Umbau auf For Each, nach Hinweis voon jcc aus dem ippf 10.09.14
        'For Each SubFolder As Outlook.MAPIFolder In Ordner.Folders

        '    If SubFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
        '        Try
        '            ChildNode = BaseNode.Nodes.Add(SubFolder.EntryID & ";" & SubFolder.StoreID, SubFolder.Name, "Kontakt")
        '            ChildNode.Tag = ChildNode.Name 'SubFolder.EntryID & ";" & SubFolder.StoreID
        '        Catch ex As Exception
        '            C_hf.LogFile("Auf den Ordner " & SubFolder.Name & " kann nicht zugegriffen werden.")
        '            ChildNode = BaseNode
        '        End Try
        '    Else
        '        ChildNode = BaseNode
        '    End If

        '    KontaktOrdnerInTreeView(SubFolder, TreeView, ChildNode)
        'Next

    End Sub
#End Region

End Class