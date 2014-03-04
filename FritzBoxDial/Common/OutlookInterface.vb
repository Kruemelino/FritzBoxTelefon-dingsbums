﻿Imports Office = Microsoft.Office.Core
Imports System.IO.Path
Imports System.Runtime.InteropServices

Friend Class C_Telefonat
    Friend Enum JournalTyp
        Eingehend = 1
        Ausgehend = 0
    End Enum

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
#End Region
End Class

Public Class OutlookInterface
    Private C_KF As Contacts
    Private C_hf As Helfer
    Private C_DP As DataProvider
    Private OInsp As Outlook.Inspector
    Friend ReadOnly Property OutlookApplication() As Outlook.Application
        Get
            Return ThisAddIn.P_oApp
        End Get
    End Property

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
        C_KF = KontaktKlasse
        C_DP = DataProviderKlasse

        C_KF.C_OLI = Me
    End Sub

    Friend Sub KontaktInformation(ByRef KontaktID As String, _
                                  ByRef StoreID As String, _
                                  Optional ByRef FullName As String = vbNullString, _
                                  Optional ByRef CompanyName As String = vbNullString, _
                                  Optional ByRef HomeAddress As String = vbNullString, _
                                  Optional ByRef BusinessAddress As String = vbNullString)

        Dim Kontakt As Outlook.ContactItem = Nothing
        Dim oApp As Outlook.Application = OutlookApplication()
        If Not oApp Is Nothing Then
            Try
                Kontakt = CType(oApp.GetNamespace("MAPI").GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
            Catch ex As Exception
                C_hf.LogFile("KontaktInformation: " & ex.Message)
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
            C_hf.LogFile("Kontaktinformationen konnten nicht ermittelt werden.")
        End If
        C_hf.NAR(Kontakt)
        oApp = Nothing
    End Sub

    Friend Function KontaktBild(ByRef KontaktID As String, ByRef StoreID As String) As String
        Dim Kontakt As Outlook.ContactItem = Nothing
        KontaktBild = C_DP.P_Def_StringEmpty
        Dim oApp As Outlook.Application = OutlookApplication()
        If Not oApp Is Nothing Then
            Try
                Kontakt = CType(oApp.GetNamespace("MAPI").GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
            Catch ex As Exception
                C_hf.LogFile("KontaktBild: " & ex.Message)
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
            C_hf.LogFile("Kontaktbild konnte nicht geladen werden.")
        End If
        C_hf.NAR(Kontakt)
        oApp = Nothing
    End Function

    Friend Function StarteKontaktSuche(ByRef KontaktID As String, _
                                  ByRef StoreID As String, _
                                  ByVal alleOrdner As Boolean, _
                                  ByRef TelNr As String, _
                                  ByVal Absender As String, _
                                  ByVal LandesVW As String) As Boolean
        Dim oApp As Outlook.Application = OutlookApplication()
        If Not oApp Is Nothing Then
            Dim olNamespace As Outlook.NameSpace = oApp.GetNamespace("MAPI")
            Dim Ergebnis As Outlook.ContactItem          ' Auswertung für Findekontakt
            StarteKontaktSuche = False
            If alleOrdner Then
                Ergebnis = C_KF.FindeKontakt(TelNr, Absender, LandesVW, olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts))
            Else
                Ergebnis = C_KF.FindeKontakt(TelNr, Absender, LandesVW, olNamespace)
            End If
            If Not Ergebnis Is Nothing Then
                StarteKontaktSuche = True
                With Ergebnis
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                End With
            End If
            Ergebnis = Nothing
            olNamespace = Nothing
        Else
            C_hf.LogFile("Kontaktsuche konnte nicht gestartet werden.")
            StarteKontaktSuche = False
        End If
        oApp = Nothing
    End Function

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

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

