Imports Office = Microsoft.Office.Core
Imports System.IO.Path
Imports System.Runtime.InteropServices
Public Class OutlookInterface
    Private KontaktFunktionen As Contacts
    Private OInsp As Outlook.Inspector
    Private hf As Helfer
    Private Dateipfad As String

    Public Sub New(ByVal KontaktKlasse As Contacts, ByVal Helferklasse As Helfer, ByVal inipfad As String)
        hf = Helferklasse
        KontaktFunktionen = KontaktKlasse
        Dateipfad = inipfad
    End Sub

    Function GetOutlook() As Outlook.Application
        Try
            GetOutlook = ThisAddIn.oApp
        Catch ex As Exception
            hf.FBDB_MsgBox("Fehler bei Erhalten von Outlook", MsgBoxStyle.Critical, "GetOutlook")
            GetOutlook = Nothing
        End Try
    End Function

    Friend Function ErstelleJournalItem(ByVal Subject As String, _
                                   ByVal Duration As Integer, _
                                   ByVal Body As String, _
                                   ByVal Start As Date, _
                                   ByVal Companies As String, _
                                   ByVal Categories As String, _
                                   Optional ByVal KontaktID As String = vbNullString, _
                                   Optional ByVal StoreID As String = vbNullString) As Boolean 'As Outlook.JournalItem

        ErstelleJournalItem = Nothing
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim oApp As Outlook.Application = GetOutlook()
        If Not oApp Is Nothing Then
            Try
                olJournal = CType(oApp.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
            Catch ex As Exception
                hf.LogFile("ErstelleJournalItem: " & ex.Message)
            End Try
            If Not olJournal Is Nothing Then
                With olJournal
                    .Subject = Subject
                    .Duration = Duration
                    .Body = Body
                    .Start = Start
                    .Companies = Companies
                    .Categories = Categories
                    If (Not (KontaktID = vbNullString Or StoreID = vbNullString)) And Not Left(KontaktID, 2) = "-1" Then
                        .Links.Add(CType(oApp.GetNamespace("MAPI").GetItemFromID(KontaktID, StoreID), Outlook.ContactItem))
                    End If
                    .Save()
                    .Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olSave)
                End With
                olJournal = Nothing
            End If
        Else
            hf.LogFile("Journaleintrag konnte nicht erstellt werden.")
        End If
        oApp = Nothing
    End Function

    Friend Sub KontaktInformation(ByRef KontaktID As String, _
                                  ByRef StoreID As String, _
                                  Optional ByRef FullName As String = vbNullString, _
                                  Optional ByRef CompanyName As String = vbNullString, _
                                  Optional ByRef HomeAddress As String = vbNullString, _
                                  Optional ByRef BusinessAddress As String = vbNullString)

        Dim Kontakt As Outlook.ContactItem = Nothing
        Dim oApp As Outlook.Application = GetOutlook()
        If Not oApp Is Nothing Then
            Try
                Kontakt = CType(oApp.GetNamespace("MAPI").GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
            Catch ex As Exception
                hf.LogFile("KontaktInformation: " & ex.Message)
            End Try
            If Not Kontakt Is Nothing Then
                With Kontakt
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                    FullName = .FullName
                    CompanyName = .CompanyName
                    HomeAddress = .HomeAddress
                    BusinessAddress = .BusinessAddress
                End With
                Kontakt = Nothing
            End If
        Else
            hf.LogFile("Kontaktinformationen konnten nicht ermittelt werden.")
        End If
        hf.NAR(Kontakt)
        oApp = Nothing
    End Sub

    Friend Function KontaktBild(ByRef KontaktID As String, ByRef StoreID As String) As String
        Dim Kontakt As Outlook.ContactItem = Nothing
        KontaktBild = vbNullString
        Dim oApp As Outlook.Application = GetOutlook()
        If Not oApp Is Nothing Then
            Try
                Kontakt = CType(oApp.GetNamespace("MAPI").GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
            Catch ex As Exception
                hf.LogFile("KontaktBild: " & ex.Message)
            End Try
            If Not Kontakt Is Nothing Then
                With Kontakt
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                    With .Attachments
                        If Not .Item("ContactPicture.jpg") Is Nothing Then
                            KontaktBild = GetTempPath() & GetRandomFileName()
                            KontaktBild = Left(KontaktBild, Len(KontaktBild) - 3) & "jpg"
                            .Item("ContactPicture.jpg").SaveAsFile(KontaktBild)
                        End If
                    End With
                End With
                Kontakt = Nothing
            End If
        Else
            hf.LogFile("Kontaktbild konnte nicht geladen werden.")
        End If
        hf.NAR(Kontakt)
        oApp = Nothing
    End Function

    Friend Function StarteKontaktSuche(ByRef KontaktID As String, _
                                  ByRef StoreID As String, _
                                  ByVal alleOrdner As Boolean, _
                                  ByRef TelNr As String, _
                                  ByVal Absender As String, _
                                  ByVal LandesVW As String) As Boolean
        Dim oApp As Outlook.Application = GetOutlook()
        If Not oApp Is Nothing Then
            Dim olNamespace As Outlook.NameSpace = oApp.GetNamespace("MAPI")
            Dim Ergebnis As Outlook.ContactItem          ' Auswertung für Findekontakt
            Dim olfolder As Outlook.MAPIFolder
            StarteKontaktSuche = False
            If alleOrdner Then
                olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
                Ergebnis = KontaktFunktionen.FindeKontakt(TelNr, Absender, LandesVW, olfolder, Nothing)
            Else
                Ergebnis = KontaktFunktionen.FindeKontakt(TelNr, Absender, LandesVW, Nothing, olNamespace)
            End If
            If Not Ergebnis Is Nothing Then
                StarteKontaktSuche = True
                With Ergebnis
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                End With
            End If
            Ergebnis = Nothing
            olfolder = Nothing
            olNamespace = Nothing
        Else
            hf.LogFile("Kontaktsuche konnte nicht gestartet werden.")
            StarteKontaktSuche = False
        End If
        oApp = Nothing
    End Function

    Friend Function NeuEmail(ByRef tmpFile As String, ByRef BodyString As String) As Boolean
        Dim olMail As Outlook.MailItem = Nothing
        Dim oApp As Outlook.Application = GetOutlook()
        If Not oApp Is Nothing Then
            Try
                olMail = CType(oApp.CreateItem(Outlook.OlItemType.olMailItem), Outlook.MailItem)
            Catch ex As Exception
                hf.LogFile("ErstelleJournalItem: " & ex.Message)
            End Try
            If Not olMail Is Nothing Then
                With olMail
                    .Attachments.Add(tmpFile)
                    .Attachments.Add(Dateipfad)
                    Try
                        .Attachments.Add(hf.Dateipfade(Dateipfad, "LogDatei"))
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
            hf.LogFile("E-Mail konnte nicht erstellt werden.")
        End If
        oApp = Nothing
        Return True
    End Function

#Region "Fenster"
    Friend Sub InspectorVerschieben(ByVal r As Boolean)
        ActiveFensterIsOutlook()
        Dim oApp As Outlook.Application = GetOutlook()
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
                Catch ex As Exception

                End Try
            End If
        Else
            hf.LogFile("Inspectorfenster konnte nicht verschoben werden.")
        End If
        oApp = Nothing
    End Sub

    'Aktive Fenster ermitteln & Handle des Aktiven Fenster ermitteln
    Private Function ActiveFensterIsOutlook() As Boolean
        'Fenster Name des Fensters mit Fokus ermitteln: OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow)
        'Fenster Name des Outlook Fenster ermitteln: OutlookSecurity.GetWindowText(OutlookSecurity.FindWindow("rctrl_renwnd32", vbNullString))
        Return OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow) = OutlookSecurity.GetWindowText(OutlookSecurity.FindWindow("rctrl_renwnd32", vbNullString))
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
                    hf.LogFile("Eine aktive Vollbildanwendung wurde detektiert.")
                End If
            End If
        End If
    End Function
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

Public NotInheritable Class OutlookSecurity

    Public Shared ReadOnly Property GetForegroundWindow() As IntPtr
        Get
            Return SafeNativeMethods.GetForegroundWindow()
        End Get
    End Property
    Public Shared ReadOnly Property GetWindowText(ByVal hwnd As IntPtr) As String
        Get
            Dim lpString As String = Space(255)
            Dim l As IntPtr = SafeNativeMethods.GetWindowText(hwnd, lpString, Len(lpString))
            Return Left(lpString, CInt(l))
        End Get
    End Property
    Public Shared ReadOnly Property FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
        Get
            Return SafeNativeMethods.FindWindow(lpClassName, lpWindowName)
        End Get
    End Property
    Public Shared ReadOnly Property GetShellWindow() As IntPtr
        Get
            Return SafeNativeMethods.GetShellWindow()
        End Get
    End Property
    Public Shared ReadOnly Property GetDesktopWindow() As IntPtr
        Get
            Return SafeNativeMethods.GetDesktopWindow()
        End Get
    End Property
    Public Shared ReadOnly Property GetWindowRect(ByVal hwnd As IntPtr) As RECT
        Get
            Dim lpRect As RECT
            SafeNativeMethods.GetWindowRect(hwnd, lpRect)
            Return lpRect
        End Get
    End Property
End Class


