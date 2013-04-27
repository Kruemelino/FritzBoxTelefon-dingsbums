Imports System.Collections
Imports Office = Microsoft.Office.Core
'Imports System.Threading

<Runtime.InteropServices.ComVisible(True)> Public Class GraphicalUserInterface
#Region "Ribbon Grundlagen für Outlook 2007 bis 2013"
#If Not OVer = 11 Then
    Implements Office.IRibbonExtensibility
    Private RibbonObjekt As Office.IRibbonUI
    Public Function GetCustomUI(ByVal ribbonID As String) As String Implements Office.IRibbonExtensibility.GetCustomUI
        Dim File As String
        Select Case ribbonID
#If OVer >= 14 Then
            Case "Microsoft.Outlook.Explorer"
                File = GetResourceText("FritzBoxDial.RibbonExplorer.xml")
#End If
            Case "Microsoft.Outlook.Mail.Read"
                File = GetResourceText("FritzBoxDial.RibbonMailRead.xml")
            Case "Microsoft.Outlook.Journal"
                File = GetResourceText("FritzBoxDial.RibbonJournal.xml")
            Case "Microsoft.Outlook.Contact"
                File = GetResourceText("FritzBoxDial.RibbonKontakt.xml")
            Case Else
                File = vbNullString
        End Select
#If OVer = 12 Then
        If Not File = vbNullString Then
            File = Replace(File, "http://schemas.microsoft.com/office/2009/07/customui", "http://schemas.microsoft.com/office/2006/01/customui", , 1, CompareMethod.Text)
        End If
#End If
        Return File
    End Function

    Private Shared Function GetResourceText(ByVal resourceName As String) As String
        Dim asm As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
        Dim resourceNames() As String = asm.GetManifestResourceNames()
        For i As Integer = 0 To resourceNames.Length - 1
            If String.Compare(resourceName, resourceNames(i), StringComparison.OrdinalIgnoreCase) = 0 Then
                Using resourceReader As IO.StreamReader = New IO.StreamReader(asm.GetManifestResourceStream(resourceNames(i)))
                    If resourceReader IsNot Nothing Then
                        Return resourceReader.ReadToEnd()
                    End If
                End Using
            End If
        Next
        Return Nothing
    End Function
#End If
#End Region

#Region "Commandbar Grundlagen für Outlook 2003 & 2007"
#If OVer < 14 Then
    Private FritzBoxDialCommandBar As Office.CommandBar
    Private WithEvents bAnrMonTimer As Timers.Timer
    Private bool_banrmon As Boolean
#End If


#End Region

#Region "Ribbon Grundlagen für Outlook 2010 & 2013"
#If OVer >= 14 Then
    Friend bolAnrMonAktiv As Boolean
#End If
#End Region

    Private HelferFunktionen As Helfer
    Private ini As InI
    Private Crypt As Rijndael
    Private Dateipfad As String
    Private Callclient As Wählclient
    Private RWSSuche As formRWSuche
    Private AnrMon As AnrufMonitor
    Private OlI As OutlookInterface
    Private KontaktFunktionen As Contacts
    Private fbox As FritzBox


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Friend Sub New(ByVal HelferKlasse As Helfer, _
                   ByVal iniKlasse As InI, _
                   ByVal CryptKlasse As Rijndael, _
                   ByVal iniPfad As String, _
                   ByVal Wclient As Wählclient, _
                   ByVal Inverssuche As formRWSuche, _
                   ByVal AnMonitor As AnrufMonitor, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal FritzBoxKlasse As FritzBox, _
                   ByVal OutlInter As OutlookInterface)

        HelferFunktionen = HelferKlasse
        ini = iniKlasse
        Crypt = CryptKlasse
        Dateipfad = iniPfad
        Callclient = Wclient
        RWSSuche = Inverssuche
        AnrMon = AnMonitor
        KontaktFunktionen = KontaktKlasse
        fbox = FritzBoxKlasse
        OlI = OutlInter
    End Sub

    Friend Sub SetOAWOF(ByVal Wclient As Wählclient, ByVal AnMonitor As AnrufMonitor, ByVal FritzBoxKlasse As FritzBox, OutlInter As OutlookInterface)
        Callclient = Wclient
        AnrMon = AnMonitor
        OlI = OutlInter
        fbox = FritzBoxKlasse
    End Sub

#Region "Office 2007 & Office 2010" ' Ribbon Inspektorfenster
#If Not OVer = 11 Then
    Public Sub OnActionWählen(ByVal control As Office.IRibbonControl)
        WählenInspector()
    End Sub

    Public Sub OnActionKontakterstellen(ByVal control As Office.IRibbonControl)
        KontaktErstellen()
    End Sub

    Public Sub OnActionRWSGoYellow(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        RWSGoYellow(Insp)
    End Sub

    Public Sub OnActionRWS11880(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        RWS11880(Insp)
    End Sub

    Public Sub OnActionRWSDasTelefonbuch(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        RWSDasTelefonbuch(Insp)
    End Sub

    Public Sub OnActionRWSTelSearch(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        RWSTelSearch(Insp)
    End Sub

    Public Sub OnActionRWSAlle(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        RWSAlle(Insp)
    End Sub

    Public Function GroupVisible(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
#If OVer = 14 Then
        Dim ActiveExplorer As Outlook.Explorer
        Dim oapp As New Outlook.Application
        Dim anzeigen As Boolean
        ActiveExplorer = oapp.ActiveExplorer
        anzeigen = Not ActiveExplorer Is Nothing
        With HelferFunktionen
            .NAR(ActiveExplorer)
            .NAR(oapp)
        End With
        Return anzeigen
#Else
        Return True
#End If
    End Function

    Public Function ButtonEnable(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.JournalItem Then
            Dim olJournal As Outlook.JournalItem = CType(Insp.CurrentItem, Outlook.JournalItem)
            If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", CompareMethod.Text) = 0 Then
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function

    Public Function ButtonEnableW(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.JournalItem Then
            Dim olJournal As Outlook.JournalItem = CType(Insp.CurrentItem, Outlook.JournalItem)
            If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", CompareMethod.Text) = 0 Then
                If CBool(InStr(olJournal.Body, "Tel.-Nr.: unbekannt", CompareMethod.Text)) Then
                    Return False
                Else
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Public Function SetLabelJournal(ByVal control As Microsoft.Office.Core.IRibbonControl) As String
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.JournalItem Then
            Dim olJournal As Outlook.JournalItem = CType(Insp.CurrentItem, Outlook.JournalItem)
            If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", CompareMethod.Text) = 0 Then
                Dim olLink As Outlook.Link = Nothing
                For Each olLink In olJournal.Links
                    Try
                        If TypeOf olLink.Item Is Outlook.ContactItem Then Return "Kontakt anzeigen"
                        Exit For
                    Catch
                        Return "Kontakt erstellen"
                    End Try
                Next
                HelferFunktionen.NAR(olLink) : olLink = Nothing
            Else
                Return "Kontakt erstellen"
            End If
        End If
        Return "Kontakt erstellen"
    End Function

    Public Function SetScreenTipJournal(ByVal control As Microsoft.Office.Core.IRibbonControl) As String
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.JournalItem Then
            Dim olJournal As Outlook.JournalItem = CType(Insp.CurrentItem, Outlook.JournalItem)
            If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", CompareMethod.Text) = 0 Then
                Dim olLink As Outlook.Link = Nothing
                For Each olLink In olJournal.Links
                    Try
                        If TypeOf olLink.Item Is Outlook.ContactItem Then Return "Zeigt den Kontakt zu diesem Journaleintrag an"
                        Exit For
                    Catch
                        Return "Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag."
                    End Try
                Next
                HelferFunktionen.NAR(olLink) : olLink = Nothing
            Else
                Return "Erstellt einen Kontakt aus diesem Journaleintrag"
            End If
        End If
        Return "Erstellt einen Kontakt aus diesem Journaleintrag"
    End Function
#End Region 'Ribbon Inspector

#Region "Office 2010/2013"
#If over >= 14 Then
    Sub Ribbon_Load(ByVal Ribbon As Office.IRibbonUI)
        RibbonObjekt = Ribbon
    End Sub

    Public Function DynMenüfüllen(ByVal control As Office.IRibbonControl) As String
        Dim ListPath As String = HelferFunktionen.Dateipfade(GetSetting("FritzBox", "Optionen", "TBini", "-1"), "Listen")
        Dim IniParam As String
        Dim index As Integer


        Dim AnrName As String
        Dim j, i As Integer
        Dim Einträge(9) As String
        Dim Eintrag As String()
        Dim MyStringBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf & "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)


        Select Case Mid(control.Id, 1, Len(control.Id) - 2)
            Case "dynMwwdh"
                IniParam = "Wwdh"
            Case "dynMAnrListe"
                IniParam = "AnrListe"
            Case Else
                IniParam = vbNullString
        End Select
        index = CInt(ini.Read(ListPath, IniParam, "Index", "0"))
        For i = 0 To 9
            Einträge(i) = ini.Read(ListPath, IniParam, IniParam & "Eintrag " & i, "")
        Next
        i = 1
        For j = index + 9 To index Step -1
            Eintrag = Split(Einträge(j Mod 10), ";", 5, CompareMethod.Text)
            ' Eintrag(0) Anrufername 
            ' Eintrag(1) TelNr 
            ' Eintrag(2) Zeit
            ' Eintrag(3) Nummer in der Liste
            ' Eintrag(4) StoreID, 
            ' Eintrag(5) KontaktID

            If Not Eintrag.Length = 1 Then
                If Not Eintrag(1) = "" Or IniParam = "VIPListe" Then
                    AnrName = Replace(Eintrag(0), "&", "&#38;&#38;", , , CompareMethod.Text)

                    MyStringBuilder.Append("<button id=""button_" & CStr(j Mod 10) & """")
                    MyStringBuilder.Append(" label=""" & CStr(IIf(AnrName = "", Eintrag(1), AnrName)) & """")
                    MyStringBuilder.Append(" onAction=""OnActionAnrListen""")
                    MyStringBuilder.Append(" tag=""" & IniParam & ";" & CStr(j Mod 10) & """")
                    MyStringBuilder.Append(" supertip=""Zeit: " & Eintrag(2) & "&#13;Telefonnummer: " & Eintrag(1) & """")
                    MyStringBuilder.Append("/>" & vbCrLf)
                    i += 1
                End If
            End If
        Next
        MyStringBuilder.Append("</menu>")

        Return MyStringBuilder.ToString
    End Function

    Public Function DynMenüfüllenVIP(ByVal control As Office.IRibbonControl) As String
        Dim ListPath As String = HelferFunktionen.Dateipfade(GetSetting("FritzBox", "Optionen", "TBini", "-1"), "Listen")

        Dim Anzahl As Integer = CInt(ini.Read(ListPath, "VIPListe", "Anzahl", "0"))


        Dim AnrName As String
        Dim j, i As Integer
        Dim Einträge(Anzahl) As String
        Dim Eintrag As String()
        Dim MyStringBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf & "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)


        For i = 0 To Anzahl - 1
            Einträge(i) = ini.Read(ListPath, "VIPListe", "VIPListeEintrag " & i, "")
        Next
        i = 1
        For j = 0 To Anzahl - 1
            Eintrag = Split(Einträge(j), ";", 5, CompareMethod.Text)
            ' Eintrag(0) Anrufername 
            ' Eintrag(1) vbNullString
            ' Eintrag(2) Nummer in der Liste
            ' Eintrag(3) StoreID, 
            ' Eintrag(2) KontaktID

            If Not Eintrag.Length = 1 Then

                AnrName = Replace(Eintrag(0), "&", "&#38;&#38;", , , CompareMethod.Text)

                MyStringBuilder.Append("<button id=""button_" & CStr(j Mod Anzahl) & """")
                MyStringBuilder.Append(" label=""" & CStr(AnrName) & """")
                MyStringBuilder.Append(" onAction=""OnActionAnrListen""")
                MyStringBuilder.Append(" tag=""VIPListe;" & CStr(j) & """")
                MyStringBuilder.Append("/>" & vbCrLf)

                i += 1
            End If
        Next
        MyStringBuilder.Append("</menu>")

        Return MyStringBuilder.ToString
    End Function

    Public Function GetPressed(ByVal control As Office.IRibbonControl) As Boolean
        If Not ThisAddIn.AnrMon Is Nothing Then
            Return ThisAddIn.AnrMon.AnrMonAktiv
        End If
        Return False
    End Function

    Public Function GetImage(ByVal control As Office.IRibbonControl) As String
        GetImage = "PersonaStatusBusy"
        If Not AnrMon Is Nothing Then
            If AnrMon.AnrMonAktiv Then
                GetImage = "PersonaStatusOnline"
            Else
                If Not AnrMon.AnrMonError Then
                    GetImage = "PersonaStatusOffline"
                End If
            End If
        End If
    End Function

    Public Function UseAnrMon(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return ThisAddIn.UseAnrMon
    End Function


    Public Function GetPressedKontextVIP(ByVal control As Office.IRibbonControl) As Boolean
        Dim oKontact As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
        GetPressedKontextVIP = IsVIP(oKontact)
        HelferFunktionen.NAR(oKontact)
        oKontact = Nothing
    End Function

    Public Sub OnActionKontextVIP(ByVal control As Office.IRibbonControl, ByVal pressed As Boolean)
        Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)

        If IsVIP(oKontakt) Then
            RemoveVIP(oKontakt.EntryID, CType(oKontakt.Parent, Outlook.MAPIFolder).StoreID)
        Else
            AddVIP(oKontakt)
        End If
        HelferFunktionen.NAR(oKontakt)
        oKontakt = Nothing

    End Sub

    Public Sub InvalidateControlAnrMon()
        If RibbonObjekt Is Nothing Then
            Dim i As Integer
            Do While RibbonObjekt Is Nothing And i < 100
                ' Thread.Sleep(50)
                i += 1
                Windows.Forms.Application.DoEvents()
            Loop
        End If
        If Not RibbonObjekt Is Nothing Then
            RibbonObjekt.Invalidate()
        End If
    End Sub

    Public Function GetVisibleAnrMonFKT(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBUseAnrMon", "True") = "True", True, False))
    End Function
    Public Function GetEnabledJI(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBJournal", "False") = "True", True, False))
    End Function
    ' Ab Hier Rückrufe von Buttons
    Public Sub OnActionDirektwahl(ByVal control As Office.IRibbonControl)
        ÖffneDirektwahl()
    End Sub

    Public Sub OnActionAnrListen(ByVal control As Office.IRibbonControl)
        KlickListen(control.Tag)
    End Sub

    Public Sub OnActionEinstellungen(ByVal control As Office.IRibbonControl)
        ÖffneEinstellungen()
    End Sub

    Public Sub OnActionJournalImport(ByVal control As Office.IRibbonControl)
        ÖffneJournalImport()
    End Sub

    Public Sub OnActionAnrMonAnzeigen(ByVal control As Office.IRibbonControl)
        ÖffneAnrMonAnzeigen()
    End Sub

    Public Sub OnActionAnrMonNeustarten(ByVal control As Office.IRibbonControl)
        AnrMonNeustarten()
    End Sub

    Public Sub OnActionWählenExplorer(ByVal control As Office.IRibbonControl)
        WählenExplorer()
    End Sub

    Public Sub OnActionAnrMonAnAus(ByVal control As Office.IRibbonControl, ByVal pressed As Boolean)
        bolAnrMonAktiv = AnrMon.AnrMonAnAus()
        RibbonObjekt.InvalidateControl(control.Id)
    End Sub

    Public Sub ContextCall(ByVal control As Office.IRibbonControl)
        WählenExplorer()
    End Sub
#End If
#End Region 'Ribbon Explorer

#Region "VIP-Ribbon"
    Public Sub OnActionInspVIP(ByVal control As Office.IRibbonControl, ByVal pressed As Boolean)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim aktKontakt As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)
            If IsVIP(aktKontakt) Then
                RemoveVIP(aktKontakt.EntryID, CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID)
            Else
                AddVIP(aktKontakt)
            End If
        End If
        RibbonObjekt.Invalidate()
    End Sub

    Public Function GetPressedVIP(ByVal control As Office.IRibbonControl) As Boolean
        GetPressedVIP = False
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim olContact As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)
            Return IsVIP(olContact)
        End If
    End Function

    Public Function GetScreenTipVIP(ByVal control As Microsoft.Office.Core.IRibbonControl) As String
        GetScreenTipVIP = vbNullString
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim aktKontakt As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)
            If IsVIP(aktKontakt) Then
                GetScreenTipVIP = "Entferne diesen Kontakt von der VIP-Liste."
            Else
                If CLng(ini.Read(Dateipfad, "VIPListe", "Anzahl", "0")) >= 10 Then
                    GetScreenTipVIP = "Die VIP-Liste ist mit 10 Einträgen bereits voll."
                Else
                    GetScreenTipVIP = "Füge diesen Kontakt der VIP-Liste hinzu."
                End If
            End If
        End If
    End Function
#End If
#End Region

#Region "VIP-Generell"
    Friend Function IsVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        IsVIP = False

        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID
        Dim Eintrag() As String
        Dim ListenPfad As String = HelferFunktionen.Dateipfade(Dateipfad, "Listen")
        Dim i As Integer = 0
        Do
            Eintrag = Split(ini.Read(ListenPfad, "VIPListe", "VIPListeEintrag " & i, ";"), ";", , CompareMethod.Text)
            If Eintrag.Length > 2 Then IsVIP = (Eintrag(5) = KontaktID And Eintrag(4) = StoreID)
            i += 1
#If OVer < 14 Then
        Loop Until i = 10 Or IsVIP
#Else
        Loop Until Eintrag.Length = 2 Or IsVIP
#End If

    End Function

    Friend Function AddVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        Dim ListenPfad As String = HelferFunktionen.Dateipfade(Dateipfad, "Listen")
        Dim Anrufer As String = Replace(aktKontakt.FullName & " (" & aktKontakt.CompanyName & ")", " ()", "")
        Dim Anzahl As Long = CLng(ini.Read(ListenPfad, "VIPListe", "Anzahl", "0"))
        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID
        Dim StrArr() As String = {Anrufer, vbNullString, vbNullString, CStr(Anzahl), StoreID, KontaktID}
        ini.Write(ListenPfad, "VIPListe", "VIPListeEintrag " & Anzahl, Join(StrArr, ";"))
        ini.Write(ListenPfad, "VIPListe", "Anzahl", CStr(Anzahl + 1))
#If OVer < 14 Then
        FillPopupItemsVIP()
#End If
        Return True
    End Function

    Friend Function RemoveVIP(ByVal EntryID As String, ByVal StoreID As String) As Boolean
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim Eintrag() As String
        Dim tempEintrag As String
        Dim Einträge(9) As String
        Dim alle As Boolean = False
        Dim myArray As New ArrayList
        Dim ListenPfad As String = HelferFunktionen.Dateipfade(Dateipfad, "Listen")
        Do
            tempEintrag = ini.Read(ListenPfad, "VIPListe", "VIPListeEintrag " & i, "")
            If tempEintrag = "" Then
                alle = True
            Else
                myArray.Add(tempEintrag)
            End If
            i += 1
            Windows.Forms.Application.DoEvents()
        Loop Until alle

        i = 1
        ini.Write(ListenPfad, "VIPListe", vbNullString, vbNullString)
        For i = 1 To myArray.Count
            Eintrag = Split(CStr(myArray(i - 1)), ";", , CompareMethod.Text)
            If Not (Eintrag(5) = EntryID And Eintrag(4) = StoreID) Then
                Eintrag(2) = CStr(j)
                ini.Write(ListenPfad, "VIPListe", "VIPListeEintrag " & j, Join(Eintrag, ";"))
                j += 1
            End If
        Next
        ini.Write(ListenPfad, "VIPListe", "Anzahl", CStr(j))
#If OVer < 14 Then
        FillPopupItemsVIP()
#End If
        Return True
    End Function
#End Region

#Region "Commandbar für Office 2003 & 2007"
#If OVer < 14 Then
    Friend Function AddCmdBar(ByVal MenuName As String, ByVal visible As Boolean) As Office.CommandBar

        AddCmdBar = Nothing
        Try
            'Ab hier für Menüeintrag
            Dim oExp As Outlook.Explorer
            Dim olMBars As Office.CommandBars
            Dim olMBar As Office.CommandBar = Nothing
            oExp = OlI.GetOutlook.ActiveExplorer
            olMBars = oExp.CommandBars
            For Each olMBar In olMBars
                If olMBar.Name = MenuName Then
                    With HelferFunktionen
                        .NAR(olMBar)
                        .NAR(olMBars)
                        .NAR(oExp)
                    End With

                    olMBar = Nothing
                    olMBars = Nothing
                    oExp = Nothing
                    Return olMBar   ' wenn die Fritz CommandBar schon vorhanden ist, 
                End If
            Next

            olMBar = oExp.CommandBars.Add(, , , True)
            With olMBar
                .Name = "FritzBox"
                .NameLocal = "FritzBox"
                .Visible = visible
                .Position = Office.MsoBarPosition.msoBarTop
            End With
            FritzBoxDialCommandbar = olMBar
            AddCmdBar = olMBar

            With HelferFunktionen
                .NAR(olMBars) : .NAR(oExp)
            End With

            olMBar = Nothing
            olMBars = Nothing
            oExp = Nothing

        Catch ex As Exception
            HelferFunktionen.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "AddCmdBar")
        End Try

    End Function

    Friend Function AddButtonsToCmb(ByVal cmdBar As Office.CommandBar, _
    ByVal btnCaption As String, ByVal PosIndex As Integer, ByVal btnFaceId As Long, ByVal btnStyle As String, _
    ByVal btnTag As String, ByVal btnToolTip As String) As Office.CommandBarButton
        Dim cbBtn As Office.CommandBarControl
        Dim cBtn As Office.CommandBarButton
        AddButtonsToCmb = Nothing ' Default Return-Wert

        Try
            cbBtn = cmdBar.FindControl(Office.MsoControlType.msoControlButton, , btnTag) 'Haben wir bereits einen solchen Knopf?
            If cbBtn Is Nothing Then ' Wenn nein, erstelle einen neuen.
                'korrekten Index ermitteln, falls vorherige Add's fehlgeschlagen sein sollten
                If cmdBar.Controls.Count < PosIndex Then PosIndex = cmdBar.Controls.Count + 1
                cBtn = CType(cmdBar.Controls.Add(Office.MsoControlType.msoControlButton, , , PosIndex, True), Office.CommandBarButton)
                With cBtn
                    .BeginGroup = True
                    .FaceId = CInt(btnFaceId)
                    Select Case btnStyle
                        Case "IconandCaption"
                            .Style = Office.MsoButtonStyle.msoButtonIconAndCaption
                        Case "Icon"
                            .Style = Office.MsoButtonStyle.msoButtonIcon
                        Case "Caption"
                            .Style = Office.MsoButtonStyle.msoButtonCaption
                    End Select
                    .Caption = btnCaption
                    .Tag = btnTag
                    .Visible = True
                End With
                Return cBtn
            End If
            cBtn = CType(cbBtn, Office.CommandBarButton)
            Return cBtn
        Catch ex As Exception
            HelferFunktionen.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "AddButtonsToCmb")
        End Try
    End Function

    Friend Sub AddPopupsToExplorer(ByRef cmdBar As Office.CommandBar, ByRef cbPopup As Office.CommandBarPopup, _
    ByVal btnCaption As String, ByVal PosIndex As Integer, ByVal btnTag As String, _
    ByVal btnTooltipText As String)

        Try
            cbPopup = CType(cmdBar.FindControl(Office.MsoControlType.msoControlPopup, , btnTag, , False), Office.CommandBarPopup)
            If cbPopup Is Nothing Then
                cbPopup = CType(cmdBar.Controls.Add(Office.MsoControlType.msoControlPopup, , , PosIndex, True), Office.CommandBarPopup)
                With cbPopup
                    .BeginGroup = True
                    .Caption = btnCaption
                    .Tag = btnTag
                    .Visible = True
                    .TooltipText = btnTooltipText
                End With
            End If
        Catch ex As Exception
            HelferFunktionen.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "AddPopupsToExplorer")
        End Try

    End Sub

    Friend Function AddPopupItems(ByRef btnPopup As Office.CommandBarPopup, _
                                  ByVal Index As Int32) As Office.CommandBarButton
        Try
            If btnPopup.Controls.Count > Index Then
                Throw New Exception("Button already exists.")
            Else
                Dim btn As Office.CommandBarButton = CType(btnPopup.Controls.Add(Office.MsoControlType.msoControlButton, , , , True), Office.CommandBarButton)
                btn.Visible = False 'erst mal verstecken, da wir nicht wissen ob da ein Wert drin ist.
                Return btn
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function

    Friend Sub FillPopupItems(ByRef btnPopup As String)
        ' btnPopuo erlaubt: AnrListe, Wwdh
        Dim cPopUp As Office.CommandBarPopup = CType(FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlPopup, , btnPopup, , False), Office.CommandBarPopup)
        Dim ListenPfad As String = HelferFunktionen.Dateipfade(Dateipfad, "Listen")
        Dim index As Integer = CInt(ini.Read(ListenPfad, btnPopup, "Index", "0"))


        Dim AnrName, TelNr As String
        Dim j, i As Integer
        Dim Einträge(9) As String
        Dim Eintrag As String()

        For i = 0 To 9
            Einträge(i) = ini.Read(ListenPfad, btnPopup, btnPopup & "Eintrag " & i, "")
        Next

        i = 1
        For j = index + 9 To index Step -1
            Eintrag = Split(Einträge(j Mod 10), ";", 5, CompareMethod.Text)
            ' Eintrag(0) Anrufername 
            ' Eintrag(1) TelNr 
            ' Eintrag(2) Zeit
            ' Eintrag(3) Nummer in der Liste
            ' Eintrag(4) StoreID, 
            ' Eintrag(5) KontaktID

            If Not Eintrag.Length = 1 Then
                If Not Eintrag(1) = "" Then
                    With cPopUp.Controls.Item(i)
                        AnrName = Replace(Eintrag(0), "&", "&&", , , CompareMethod.Text)
                        TelNr = Eintrag(1)
                        If AnrName = "" Then .Caption = TelNr Else .Caption = AnrName
                        .TooltipText = "Zeit: " & Eintrag(2) & Environment.NewLine & "Telefonnummer: " & TelNr
                        .Parameter = CStr(j Mod 10)
                        .Visible = True
                        .Tag = btnPopup & ";" & CStr(j Mod 10)
                        i += 1
                    End With
                End If
            End If
        Next

    End Sub

    Friend Sub FillPopupItemsVIP()
        ' 26.04.11 14.44
        Dim btnPopup As Office.CommandBarPopup = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , "VIPListe"), Office.CommandBarPopup)
        Dim ListPath As String = HelferFunktionen.Dateipfade(GetSetting("FritzBox", "Optionen", "TBini", "-1"), "Listen")
        Dim ini As New Ini

        Dim j, i As Integer
        Dim Einträge(9) As String
        Dim Eintrag As String()

        For i = 0 To 9
            Einträge(i) = ini.Read(ListPath, "VIPListe", "VIPListeEintrag " & i, "")
        Next
        i = 1
        For j = 0 To 9
            Eintrag = Split(Einträge(j), ";", 5, CompareMethod.Text)
            ' Eintrag(0) Anrufername 
            ' Eintrag(1) vbNullString 
            ' Eintrag(2) Nummer in der Liste
            ' Eintrag(3) StoreID, 
            ' Eintrag(4) KontaktID

            If Not Eintrag.Length = 1 Then
                If Not Eintrag(0) = "" Then
                    With btnPopup.Controls.Item(i)
                        .Caption = Replace(Eintrag(0), "&", "&&", , , CompareMethod.Text)
                        .Parameter = CStr(j Mod 10)
                        .Visible = True
                        .Tag = "VIPListe;" & CStr(j)
                        i += 1
                    End With
                End If
            Else
                If Not btnPopup.Controls.Item(i) Is Nothing Then
                    btnPopup.Controls.Item(i).Visible = False
                End If

            End If
        Next
    End Sub

    Friend Sub SetVisibleButtons()
        ' Einstellungen für die Symbolleiste speichern
        Try
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlButton, , "Direktwahl").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbDirekt", "True") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlButton, , "Anrufmonitor").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbAnrMon", "True") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlButton, , "Anzeigen").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbAnrMon", "True") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlPopup, , "AnrListe").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbAnrListe", "True") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlPopup, , "Wwdh").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbWwdh", "True") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlButton, , "Journalimport").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbJournalimport", "False") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlButton, , "AnrMonNeuStart").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbAnrMonNeuStart", "False") = "True", True, False))
            FritzBoxDialCommandbar.FindControl(Office.MsoControlType.msoControlPopup, , "VIPListe").Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbVIP", "False") = "True", True, False))
        Catch ex As Exception

        End Try
    End Sub

    Friend Sub SetAnrMonButton(ByVal EinAus As Boolean)
        bool_banrmon = EinAus
        bAnrMonTimer = New Timers.Timer
        With bAnrMonTimer
            .Interval = 200
            .Enabled = True
            .Start()
        End With
    End Sub

    Private Sub bAnrMonTimer_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles bAnrMonTimer.Disposed
        bAnrMonTimer.Close()
    End Sub

    Private Sub bAnrMonTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles bAnrMonTimer.Elapsed
        If Not FritzBoxDialCommandBar Is Nothing Then
            Dim btnAnrMon As Office.CommandBarButton = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anrufmonitor", , False), Office.CommandBarButton)
            Select Case bool_banrmon
                Case True
                    btnAnrMon.State = Office.MsoButtonState.msoButtonDown
                    btnAnrMon.TooltipText = "Beendet den Anrufmonitor"
                Case False
                    btnAnrMon.State = Office.MsoButtonState.msoButtonUp
                    btnAnrMon.TooltipText = "Startet den Anrufmonitor"
            End Select
            bAnrMonTimer.Stop()
            bAnrMonTimer.Dispose()
            btnAnrMon = Nothing
        End If
    End Sub

#End If
#End Region

#Region "Symbolleiste erzeugen"
#If OVer < 14 Then
    Sub SymbolleisteErzeugen(ByRef ePopWwdh As Office.CommandBarPopup, ByRef ePopAnr As Office.CommandBarPopup, ByRef ePopVIP As Office.CommandBarPopup, _
                             ByRef eBtnWaehlen As Office.CommandBarButton, ByRef eBtnDirektwahl As Office.CommandBarButton, ByRef eBtnAnrMonitor As Office.CommandBarButton, _
                             ByRef eBtnAnzeigen As Office.CommandBarButton, ByRef eBtnAnrMonNeuStart As Office.CommandBarButton, ByRef eBtnJournalimport As Office.CommandBarButton, ByRef eBtnEinstellungen As Office.CommandBarButton, _
                             ByRef ePopWwdh1 As Office.CommandBarButton, ByRef ePopWwdh2 As Office.CommandBarButton, ByRef ePopWwdh3 As Office.CommandBarButton, ByRef ePopWwdh4 As Office.CommandBarButton, _
                             ByRef ePopWwdh5 As Office.CommandBarButton, ByRef ePopWwdh6 As Office.CommandBarButton, ByRef ePopWwdh7 As Office.CommandBarButton, ByRef ePopWwdh8 As Office.CommandBarButton, _
                             ByRef ePopWwdh9 As Office.CommandBarButton, ByRef ePopWwdh10 As Office.CommandBarButton, _
                             ByRef ePopAnr1 As Office.CommandBarButton, ByRef ePopAnr2 As Office.CommandBarButton, ByRef ePopAnr3 As Office.CommandBarButton, ByRef ePopAnr4 As Office.CommandBarButton, _
                             ByRef ePopAnr5 As Office.CommandBarButton, ByRef ePopAnr6 As Office.CommandBarButton, ByRef ePopAnr7 As Office.CommandBarButton, ByRef ePopAnr8 As Office.CommandBarButton, _
                             ByRef ePopAnr9 As Office.CommandBarButton, ByRef ePopAnr10 As Office.CommandBarButton, _
                             ByRef ePopVIP1 As Office.CommandBarButton, ByRef ePopVIP2 As Office.CommandBarButton, ByRef ePopVIP3 As Office.CommandBarButton, ByRef ePopVIP4 As Office.CommandBarButton, _
                             ByRef ePopVIP5 As Office.CommandBarButton, ByRef ePopVIP6 As Office.CommandBarButton, ByRef ePopVIP7 As Office.CommandBarButton, ByRef ePopVIP8 As Office.CommandBarButton, _
                             ByRef ePopVIP9 As Office.CommandBarButton, ByRef ePopVIP10 As Office.CommandBarButton)
        Dim i As Integer = 2

        FritzBoxDialCommandBar = AddCmdBar("FritzBoxDial", True)

        eBtnWaehlen = AddButtonsToCmb(FritzBoxDialCommandBar, "Wählen", 1, 568, "IconandCaption", "Wählen", "Wählen")

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopWwdh, "Wahlwiederholung", i, "Wwdh", "Letzte Anrufe wiederholen")
        i += 1
        Try
            ePopWwdh1 = AddPopupItems(ePopWwdh, 1) : ePopWwdh2 = AddPopupItems(ePopWwdh, 2)
            ePopWwdh3 = AddPopupItems(ePopWwdh, 3) : ePopWwdh4 = AddPopupItems(ePopWwdh, 4)
            ePopWwdh5 = AddPopupItems(ePopWwdh, 5) : ePopWwdh6 = AddPopupItems(ePopWwdh, 6)
            ePopWwdh7 = AddPopupItems(ePopWwdh, 7) : ePopWwdh8 = AddPopupItems(ePopWwdh, 8)
            ePopWwdh9 = AddPopupItems(ePopWwdh, 9) : ePopWwdh10 = AddPopupItems(ePopWwdh, 10)
        Catch ex As Exception
            HelferFunktionen.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopWwdh)")
        End Try

        FillPopupItems("Wwdh")
        ' Direktwahl
        ePopWwdh.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbWwdh", "True"))
        eBtnDirektwahl = AddButtonsToCmb(FritzBoxDialCommandBar, "Direktwahl", i, 326, "IconandCaption", "Direktwahl", "Direktwahl")
        i += 1

        eBtnDirektwahl.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbDirekt", "False"))
        ' Symbol Anrufmonitor & Anzeigen
        eBtnAnrMonitor = AddButtonsToCmb(FritzBoxDialCommandBar, "Anrufmonitor", i, 815, "IconandCaption", "Anrufmonitor", "Anrufmonitor starten oder stoppen") '815

        eBtnAnzeigen = AddButtonsToCmb(FritzBoxDialCommandBar, "Anzeigen", i + 1, 682, "IconandCaption", "Anzeigen", "Letzte Anrufe anzeigen")
        i += 2

        eBtnAnrMonitor.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbAnrMon", "True"))
        eBtnAnzeigen.Visible = eBtnAnrMonitor.Visible

        eBtnAnrMonNeuStart = AddButtonsToCmb(FritzBoxDialCommandBar, "Anrufmonitor neustarten", i, 37, "IconandCaption", "AnrMonNeuStart", "")
        eBtnAnrMonNeuStart.TooltipText = "Startet den Anrufmonitor neu."
        eBtnAnrMonNeuStart.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbAnrMonNeuStart", "False"))

        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopAnr, "Rückruf", i, "AnrListe", "Letze Anrufer zurückrufen")
        Try
            ePopAnr1 = AddPopupItems(ePopAnr, 1) : ePopAnr2 = AddPopupItems(ePopAnr, 2)
            ePopAnr3 = AddPopupItems(ePopAnr, 3) : ePopAnr4 = AddPopupItems(ePopAnr, 4)
            ePopAnr5 = AddPopupItems(ePopAnr, 5) : ePopAnr6 = AddPopupItems(ePopAnr, 6)
            ePopAnr7 = AddPopupItems(ePopAnr, 7) : ePopAnr8 = AddPopupItems(ePopAnr, 8)
            ePopAnr9 = AddPopupItems(ePopAnr, 9) : ePopAnr10 = AddPopupItems(ePopAnr, 10)
        Catch ex As Exception
            HelferFunktionen.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopAnr)")
        End Try
        FillPopupItems("AnrListe")
        ePopAnr.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbAnrListe", "True"))
        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopVIP, "VIP", i, "VIPListe", "VIP anrufen")
        Try
            ePopVIP1 = AddPopupItems(ePopVIP, 1) : ePopVIP2 = AddPopupItems(ePopVIP, 2)
            ePopVIP3 = AddPopupItems(ePopVIP, 3) : ePopVIP4 = AddPopupItems(ePopVIP, 4)
            ePopVIP5 = AddPopupItems(ePopVIP, 5) : ePopVIP6 = AddPopupItems(ePopVIP, 6)
            ePopVIP7 = AddPopupItems(ePopVIP, 7) : ePopVIP8 = AddPopupItems(ePopVIP, 8)
            ePopVIP9 = AddPopupItems(ePopVIP, 9) : ePopVIP10 = AddPopupItems(ePopVIP, 10)
        Catch ex As Exception
            HelferFunktionen.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopVIP)")
        End Try
        FillPopupItemsVIP()
        i += 1
        ePopVIP.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbVIP", "True"))

        eBtnJournalimport = AddButtonsToCmb(FritzBoxDialCommandBar, "Journalimport", i, 591, "IconandCaption", "Journalimport", "Importiert die Anrufliste der Fritz!Box als Journaleinträge")
        eBtnJournalimport.Visible = CBool(ini.Read(Dateipfad, "Optionen", "CBSymbJournalimport", "False"))
        i += 1
        eBtnEinstellungen = AddButtonsToCmb(FritzBoxDialCommandBar, "Einstellungen", i, 548, "IconandCaption", "Einstellungen", "Fritz!Box Einstellungen")
        i += 1

        eBtnWaehlen.TooltipText = "Öffnet den Wahldialog um das ausgewählte Element anzurufen."
        ePopWwdh.TooltipText = "Öffnet den Wahldialog für die Wahlwiederholung."
        eBtnAnrMonitor.TooltipText = "Startet den Anrufmonitor."
        eBtnDirektwahl.TooltipText = "Öffnet den Wahldialog für die Diarektwahl"
        eBtnAnzeigen.TooltipText = "Zeigt den letzten Anruf an."
        eBtnAnrMonNeuStart.TooltipText = "Startet den Anrufmonitor neu."
        ePopAnr.TooltipText = "Öffnet den Wahldialog für den Rückruf."
        ePopVIP.TooltipText = "Öffnet den Wahldialog um einen VIP anzurufen."
        eBtnJournalimport.TooltipText = "Importiert die Anrufliste der Fritz!Box als Journaleinträge."
        eBtnEinstellungen.TooltipText = "Öffnet die Fritz!Box Telefon-dingsbums Einstellungen."

    End Sub
#End If
#If OVer = 11 Then
    Sub InspectorSybolleisteErzeugen(ByVal Inspector As Outlook.Inspector, _
                                     ByRef iPopRWS As Office.CommandBarPopup, ByRef iBtnWwh As Office.CommandBarButton, ByRef iBtnRwsGoYellow As Office.CommandBarButton, _
                                     ByRef iBtnRws11880 As Office.CommandBarButton, ByRef iBtnRWSDasTelefonbuch As Office.CommandBarButton, ByRef iBtnRWStelSearch As Office.CommandBarButton, _
                                     ByRef iBtnRWSAlle As Office.CommandBarButton, ByRef iBtnKontakterstellen As Office.CommandBarButton, ByRef iBtnVIP As Office.CommandBarButton)

        Dim cmbs As Office.CommandBars = Inspector.CommandBars
        Dim cmb As Office.CommandBar = Nothing
        Dim cmbErstellen As Boolean = True
        Dim i As Integer = 1

        If CBool(ini.Read(Dateipfad, "Optionen", "CBSymbRWSuche")) = True Then
            If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Or _
            TypeOf Inspector.CurrentItem Is Outlook.JournalItem Or _
            TypeOf Inspector.CurrentItem Is Outlook.MailItem Then

                ' Wenn die Leiste nicht gefunden werden konnte, dann füge sie hinzu.
                If TypeOf Inspector.CurrentItem Is Outlook.MailItem Then
                    For Each cmb In cmbs
                        If cmb.NameLocal = "FritzBoxDial" Then
                            cmbErstellen = False
                            Exit For
                        End If
                    Next
                End If
                If cmbErstellen Then
                    cmb = Inspector.CommandBars.Add("FritzBoxDial", Microsoft.Office.Core.MsoBarPosition.msoBarTop, , True)
                    With cmb
                        .NameLocal = "FritzBoxDial"
                        .Visible = True
                    End With
                    iBtnWwh = AddButtonsToCmb(cmb, "Wählen", i, 568, "IconandCaption", "Wählen2", "FritzBox Wählclient für Outlook")
                    i += 1
                End If
            End If
            ' Kontakteinträge
            If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Or TypeOf Inspector.CurrentItem Is Outlook.JournalItem Then

                AddPopupsToExplorer(cmb, iPopRWS, "Rückwärtssuche", i, "RWS", "Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche.")
                i += 1
                iBtnRwsGoYellow = AddPopupItems(iPopRWS, 1)
                iBtnRws11880 = AddPopupItems(iPopRWS, 2)
                iBtnRWSDasTelefonbuch = AddPopupItems(iPopRWS, 3)
                iBtnRWStelSearch = AddPopupItems(iPopRWS, 4)
                iBtnRWSAlle = AddPopupItems(iPopRWS, 5)

                Dim rwsNamen() As String = {"GoYellow", "11880", "DasTelefonbuch", "tel.search.ch", "Alle"}
                Dim rwsToolTipp() As String = {"Rückwärtssuche mit 'www.goyellow.de'", "Rückwärtssuche mit 'www.11880.com'", "Rückwärtssuche mit 'www.dastelefonbuch.de'", "Rückwärtssuche mit 'tel.search.ch'", "Rückwärtssuche mit allen Anbietern."}
                For i = 0 To 4
                    With iPopRWS.Controls.Item(i + 1)
                        .Caption = rwsNamen(i)
                        .TooltipText = rwsToolTipp(i)
                        .Visible = True
                    End With
                Next
            End If
            If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Then
                iBtnVIP = AddButtonsToCmb(cmb, "VIP", i, 3710, "IconandCaption", "VIP", "Füge diesen Kontakt der VIP-Liste hinzu.")
                Dim olKontact As Outlook.ContactItem = CType(Inspector.CurrentItem, Outlook.ContactItem)
                With iBtnVIP
                    If IsVIP(olKontact) Then
                        .State = Office.MsoButtonState.msoButtonDown
                    Else
                        If CLng(ini.Read(HelferFunktionen.Dateipfade(Dateipfad, "Listen"), "VIPListe", "Anzahl", "0")) >= 10 Then
                            .TooltipText = "Die VIP-Liste ist mit 10 Einträgen bereits voll."
                            .Enabled = False
                        Else
                            .TooltipText = "Füge diesen Kontakt der VIP-Liste hinzu."
                        End If
                        .State = Office.MsoButtonState.msoButtonUp
                    End If
                    .Visible = CBool(IIf(ini.Read(Dateipfad, "Optionen", "CBSymbVIP", "False") = "True", True, False))
                End With
            End If
            ' Journaleinträge
            If TypeOf Inspector.CurrentItem Is Outlook.JournalItem Then
                iBtnKontakterstellen = AddButtonsToCmb(cmb, "Kontakt erstellen", i, 1099, "IconandCaption", "Kontakterstellen", "Erstellt einen Kontakt aus einem Journaleintrag")
                Dim olJournal As Outlook.JournalItem = CType(Inspector.CurrentItem, Outlook.JournalItem)
                If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", vbTextCompare) = 0 Then
                    Dim olLink As Outlook.Link = Nothing
                    For Each olLink In olJournal.Links
                        If TypeOf olLink.Item Is Outlook.ContactItem Then iBtnKontakterstellen.Caption = "Kontakt anzeigen"
                        Exit For
                    Next
                    HelferFunktionen.NAR(olLink) : olLink = Nothing
                    iPopRWS.Enabled = True
                    iBtnWwh.Enabled = Not CBool(InStr(olJournal.Body, "Tel.-Nr.: unbekannt", CompareMethod.Text))
                    iBtnKontakterstellen.Enabled = True
                Else
                    cmb.Delete()
                End If
            End If
        End If
    End Sub
#End If
#End Region 'für Office 2003 und 2007

#Region "Explorer Button Click"
    Friend Sub ÖffneDirektwahl()
        Callclient.Wählbox(Nothing, "", True)
    End Sub

    Friend Sub ÖffneEinstellungen()
        Dim formConfig As New formCfg(Dateipfad, Me, ini, HelferFunktionen, Crypt, AnrMon, fbox, OlI, KontaktFunktionen)
        formConfig.ShowDialog()
        Dateipfad = GetSetting("FritzBox", "Optionen", "TBini", "-1")
    End Sub

    Friend Sub ÖffneJournalImport()
        Dim formjournalimort As New formJournalimport(Dateipfad, AnrMon, HelferFunktionen, ini, True)
    End Sub

    Friend Sub ÖffneAnrMonAnzeigen()
        Dim ID As Integer = CInt(ini.Read(HelferFunktionen.Dateipfade(Dateipfad, "Listen"), "letzterAnrufer", "Letzter", CStr(0)))
        Dim forman As New formAnrMon(Dateipfad, ID, False, ini, HelferFunktionen, AnrMon, OlI)
    End Sub

    Friend Sub AnrMonNeustarten()
        AnrMon.AnrMonReStart()
    End Sub

    Friend Sub KlickListen(ByVal controlTag As String)
        Callclient.OnActionAnrListen(controlTag)
    End Sub

    Friend Sub WählenExplorer()
        Dim olApp As Outlook.Application = ThisAddIn.oApp
        If Not olApp Is Nothing Then
            Dim ActiveExplorer As Outlook.Explorer = olApp.ActiveExplorer
            Dim oSel As Outlook.Selection = ActiveExplorer.Selection
            Callclient.WählboxStart(oSel)
            HelferFunktionen.NAR(oSel) : HelferFunktionen.NAR(ActiveExplorer)
            oSel = Nothing : ActiveExplorer = Nothing
        End If
    End Sub
#End Region

#Region "Inspector Button Click"
    Friend Sub WählenInspector()
        Callclient.WählenAusInspector()
    End Sub

    Friend Sub KontaktErstellen()
        KontaktFunktionen.KontaktErstellen()
    End Sub

    Friend Sub RWSGoYellow(ByVal insp As Outlook.Inspector)
        RWSSuche.Rückwärtssuche(formRWSuche.Suchmaschine.RWSGoYellow, insp)
    End Sub

    Friend Sub RWS11880(ByVal insp As Outlook.Inspector)
        RWSSuche.Rückwärtssuche(formRWSuche.Suchmaschine.RWS11880, insp)
    End Sub

    Friend Sub RWSDasTelefonbuch(ByVal insp As Outlook.Inspector)
        RWSSuche.Rückwärtssuche(formRWSuche.Suchmaschine.RWSDasTelefonbuch, insp)
    End Sub

    Friend Sub RWSTelSearch(ByVal insp As Outlook.Inspector)
        RWSSuche.Rückwärtssuche(formRWSuche.Suchmaschine.RWStelSearch, insp)
    End Sub

    Friend Sub RWSAlle(ByVal insp As Outlook.Inspector)
        RWSSuche.Rückwärtssuche(formRWSuche.Suchmaschine.RWSAlle, insp)
    End Sub
#End Region


End Class