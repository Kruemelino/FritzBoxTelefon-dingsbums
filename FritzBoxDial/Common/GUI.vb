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
                File = C_DP.P_Def_StringEmpty
        End Select
#If OVer = 12 Then
        If Not File = C_DP.P_Def_StringEmpty Then
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

#Region "Eigene Klassen"
    Private C_HF As Helfer
    Private C_DP As DataProvider
    Private C_Crypt As Rijndael
    Private C_WClient As Wählclient
    Private C_AnrMon As AnrufMonitor
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts
    Private C_FBox As FritzBox
    Private C_Phoner As PhonerInterface
#End Region

#Region "Eigene Formulare"
    Private F_RWS As formRWSuche
#End Region

#Region "Properies"
    Friend Property P_CallClient() As Wählclient
        Get
            Return C_WClient
        End Get
        Set(ByVal value As Wählclient)
            C_WClient = value
        End Set
    End Property

    Friend Property P_AnrufMonitor() As AnrufMonitor
        Get
            Return C_AnrMon
        End Get
        Set(ByVal value As AnrufMonitor)
            C_AnrMon = value
        End Set
    End Property

    Public Property P_OlInterface() As OutlookInterface
        Get
            Return C_OLI
        End Get
        Set(ByVal value As OutlookInterface)
            C_OLI = value
        End Set
    End Property

    Public Property P_FritzBox() As FritzBox
        Get
            Return C_FBox
        End Get
        Set(ByVal value As FritzBox)
            C_FBox = value
        End Set
    End Property
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Friend Sub New(ByVal HelferKlasse As Helfer, _
               ByVal DataProviderKlasse As DataProvider, _
               ByVal CryptKlasse As Rijndael, _
               ByVal Inverssuche As formRWSuche, _
               ByVal KontaktKlasse As Contacts, _
               ByVal Phonerklasse As PhonerInterface)
        C_HF = HelferKlasse
        C_DP = DataProviderKlasse
        C_Crypt = CryptKlasse
        F_RWS = Inverssuche
        C_KF = KontaktKlasse
        C_Phoner = Phonerklasse
    End Sub

#Region "Ribbon Inspector Office 2007 & Office 2010 & Office 2013" ' Ribbon Inspektorfenster
#If Not OVer = 11 Then
    Public Sub OnActionWählen(ByVal control As Office.IRibbonControl)
        WählenInspector()
    End Sub

    Public Sub OnActionKontakterstellen(ByVal control As Office.IRibbonControl)
        KontaktErstellen()
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
        With C_HF
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
#If Not OVer = 15 Then
                Dim olLink As Outlook.Link = Nothing
                For Each olLink In olJournal.Links
                    Try
                        If TypeOf olLink.Item Is Outlook.ContactItem Then Return "Kontakt anzeigen"
                        Exit For
                    Catch
                        Return "Kontakt erstellen"
                    End Try
                Next
                C_HF.NAR(olLink) : olLink = Nothing
#End If
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
#If Not OVer = 15 Then
                Dim olLink As Outlook.Link = Nothing
                For Each olLink In olJournal.Links
                    Try
                        If TypeOf olLink.Item Is Outlook.ContactItem Then Return "Zeigt den Kontakt zu diesem Journaleintrag an"
                        Exit For
                    Catch
                        Return "Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag."
                    End Try
                Next
                C_HF.NAR(olLink) : olLink = Nothing
#End If
            Else
                Return "Erstellt einen Kontakt aus diesem Journaleintrag"
            End If
        End If
        Return "Erstellt einen Kontakt aus diesem Journaleintrag"
    End Function


    Public Sub OnActionNote(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        C_KF.AddNote(CType(Insp.CurrentItem, Outlook.ContactItem))
    End Sub
#End Region 'Ribbon Inspector

#Region "Ribbon Expector Office 2010 & Office 2013"
#If oVer >= 14 Then
    Sub Ribbon_Load(ByVal Ribbon As Office.IRibbonUI)
        RibbonObjekt = Ribbon
    End Sub

    Public Function DynMenüfüllen(ByVal control As Office.IRibbonControl) As String

        Dim XMLListBaseNode As String

        Dim index As Integer
        Dim i As Integer

        Dim Anrufer As String
        Dim TelNr As String
        Dim Zeit As String

        Dim LANodeNames As New ArrayList
        Dim LANodeValues As New ArrayList
        Dim xPathTeile As New ArrayList

        Dim MyStringBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf & "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)

        Select Case Mid(control.Id, 1, Len(control.Id) - 2)
            Case "dynMwwdh"
                XMLListBaseNode = C_DP.P_Def_NameListCALL '"CallList"
            Case "dynMAnrListe"
                XMLListBaseNode = C_DP.P_Def_NameListRING '"RingList"
            Case Else '"dynMVIPListe"
                XMLListBaseNode = C_DP.P_Def_NameListVIP '"VIPList"
        End Select

        index = CInt(C_DP.Read(XMLListBaseNode, "Index", "0"))

        LANodeNames.Add("Anrufer")
        LANodeNames.Add("TelNr")
        LANodeNames.Add("Zeit")
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne)
        With xPathTeile
            .Add(XMLListBaseNode)
            .Add("Eintrag")
        End With
        i = 1
        If Not XMLListBaseNode = "VIPListe" Then
            For ID = index + 9 To index Step -1

                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                Anrufer = Replace(Anrufer, "&", "&#38;&#38;", , , CompareMethod.Text)

                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))
                Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))

                If Not TelNr = C_DP.P_Def_ErrorMinusOne Then
                    MyStringBuilder.Append("<button id=""button_" & CStr(ID Mod 10) & """")
                    MyStringBuilder.Append(" label=""" & CStr(IIf(Anrufer = C_DP.P_Def_ErrorMinusOne, TelNr, Anrufer)) & """")  ''CStr(IIf(Anrufer = C_DP.P_Def_ErrorMinusOne, TelNr, Anrufer))
                    MyStringBuilder.Append(" onAction=""OnActionListen""")
                    MyStringBuilder.Append(" tag=""" & XMLListBaseNode & ";" & CStr(ID Mod 10) & """")
                    MyStringBuilder.Append(" supertip=""Zeit: " & Zeit & "&#13;Telefonnummer: " & TelNr & """")
                    MyStringBuilder.Append("/>" & vbCrLf)
                    i += 1
                    LANodeValues.Item(0) = (C_DP.P_Def_ErrorMinusOne)
                    LANodeValues.Item(1) = (C_DP.P_Def_ErrorMinusOne)
                    LANodeValues.Item(2) = (C_DP.P_Def_ErrorMinusOne)
                End If
            Next
        Else
            For ID = 0 To index - 1
                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                If Not Anrufer = C_DP.P_Def_ErrorMinusOne Then

                    MyStringBuilder.Append("<button id=""button_" & CStr(ID Mod index) & """")
                    MyStringBuilder.Append(" label=""" & CStr(Anrufer) & """")
                    MyStringBuilder.Append(" onAction=""OnActionListen""")
                    MyStringBuilder.Append(" tag=""VIPListe;" & CStr(ID) & """")
                    MyStringBuilder.Append("/>" & vbCrLf)

                    'xPathTeile.RemoveAt(xPathTeile.Count - 1)
                    LANodeValues.Item(0) = (C_DP.P_Def_ErrorMinusOne)
                End If
            Next
        End If

        MyStringBuilder.Append("</menu>")

        DynMenüfüllen = MyStringBuilder.ToString
        LANodeNames = Nothing
        LANodeValues = Nothing
        xPathTeile = Nothing
    End Function

    Public Function DynMenüEnabled(ByVal control As Office.IRibbonControl) As Boolean
        Dim XMLListBaseNode As String
        Dim xPathTeile As New ArrayList

        Select Case Mid(control.Id, 1, Len(control.Id) - 2)
            Case "dynMwwdh"
                XMLListBaseNode = C_DP.P_Def_NameListCALL '"CallList"
            Case "dynMAnrListe"
                XMLListBaseNode = C_DP.P_Def_NameListRING '"RingList"
            Case Else '"dynMVIPListe"
                XMLListBaseNode = C_DP.P_Def_NameListVIP '"VIPList"
        End Select

        Return CBool(IIf(Not C_DP.Read(XMLListBaseNode, "Index", C_DP.P_Def_ErrorMinusOne) = C_DP.P_Def_ErrorMinusOne, True, False))
    End Function

    Public Function GetPressed(ByVal control As Office.IRibbonControl) As Boolean
        GetPressed = False
        If Not C_AnrMon Is Nothing Then
            GetPressed = C_AnrMon.AnrMonAktiv
        End If
    End Function

    Public Function GetImage(ByVal control As Office.IRibbonControl) As String
        GetImage = "PersonaStatusBusy"
        If Not C_AnrMon Is Nothing Then
            If C_AnrMon.AnrMonAktiv Then
                GetImage = "PersonaStatusOnline"
            Else
                If Not C_AnrMon.AnrMonError Then GetImage = "PersonaStatusOffline"
            End If
        End If
    End Function

    Public Function UseAnrMon(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return C_DP.P_CBUseAnrMon
    End Function

    Public Function GetPressedKontextVIP(ByVal control As Office.IRibbonControl) As Boolean
        Dim oKontact As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
        GetPressedKontextVIP = IsVIP(oKontact)
        C_HF.NAR(oKontact)
        oKontact = Nothing
    End Function

    Public Sub OnActionKontextVIP(ByVal control As Office.IRibbonControl, ByVal pressed As Boolean)
        Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)

        If IsVIP(oKontakt) Then
            RemoveVIP(oKontakt.EntryID, CType(oKontakt.Parent, Outlook.MAPIFolder).StoreID)
        Else
            AddVIP(oKontakt)
        End If
        C_HF.NAR(oKontakt)
        oKontakt = Nothing

    End Sub

    Public Sub RefreshRibbon()
        If RibbonObjekt Is Nothing Then
            Dim i As Integer
            Do While RibbonObjekt Is Nothing And i < 100

                i += 1
                Windows.Forms.Application.DoEvents()
            Loop
        End If
        If Not RibbonObjekt Is Nothing Then
            RibbonObjekt.Invalidate()
        End If
    End Sub

    Public Function GetVisibleAnrMonFKT(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return C_DP.P_CBUseAnrMon
    End Function

    Public Function GetEnabledJI(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return C_DP.P_CBJournal
    End Function

    Public Sub OnActionDirektwahl(ByVal control As Office.IRibbonControl)
        WähleDirektwahl()
    End Sub

    Public Sub OnActionListen(ByVal control As Office.IRibbonControl)
        P_CallClient.OnActionListen(control.Tag)
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
        C_AnrMon.AnrMonStartStopp()
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
        ' Fehler unter Office 2007
        ' RibbonObjekt.Invalidate()
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
        GetScreenTipVIP = C_DP.P_Def_StringEmpty
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim aktKontakt As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)
            If IsVIP(aktKontakt) Then
                GetScreenTipVIP = "Entferne diesen Kontakt von der VIP-Liste."
            Else
                If CLng(C_DP.Read("VIPListe", "Anzahl", "0")) >= 10 Then
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
        Dim xPathTeile As New ArrayList

        xPathTeile.Add("VIPListe")
        xPathTeile.Add("Eintrag")
        xPathTeile.Add("[(KontaktID = """ & KontaktID & """ and StoreID = """ & StoreID & """)]")
        IsVIP = Not C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne) = C_DP.P_Def_ErrorMinusOne
        xPathTeile = Nothing
    End Function

    Friend Overloads Function AddVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        Dim Anrufer As String = Replace(aktKontakt.FullName & " (" & aktKontakt.CompanyName & ")", " ()", "")
        Dim Index As Integer = CInt(C_DP.Read("VIPListe", "Index", "0"))
        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        xPathTeile.Add("VIPListe")
        xPathTeile.Add("ID[@ID=""" & Index & """]")

        If Not Anrufer = C_DP.P_Def_StringEmpty Then
            NodeNames.Add("Anrufer")
            NodeValues.Add(Anrufer)
        End If

        If Not StoreID = C_DP.P_Def_StringEmpty Then
            NodeNames.Add("StoreID")
            NodeValues.Add(StoreID)
        End If

        If Not KontaktID = C_DP.P_Def_StringEmpty Then
            NodeNames.Add("KontaktID")
            NodeValues.Add(KontaktID)
        End If

        AttributeNames.Add("ID")
        AttributeValues.Add(CStr(Index))

        With C_DP
            xPathTeile.RemoveRange(0, xPathTeile.Count)
            xPathTeile.Add("VIPListe")
            xPathTeile.Add("Index")
            .Write(xPathTeile, CStr(Index + 1))
            xPathTeile.Remove("Index")
            .AppendNode(xPathTeile, .CreateXMLNode("Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
            .SpeichereXMLDatei()
        End With
        NodeNames = Nothing
        NodeValues = Nothing
        xPathTeile = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
#If OVer < 14 Then
        FillPopupItems("VIPListe")
#Else
        RefreshRibbon()
#End If
        Return True
    End Function

    Friend Overloads Function AddVIP(ByVal KontaktID As String, ByVal StoreID As String) As Boolean
        Dim oKontact As Outlook.ContactItem
        oKontact = Nothing

        Try
            oKontact = CType(CType(C_OLI.OutlookApplication.GetNamespace("MAPI"), Outlook.NameSpace).GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
        Catch : End Try

        Return AddVIP(oKontact)
    End Function

    Friend Function RemoveVIP(ByVal KontaktID As String, ByVal StoreID As String) As Boolean

        Dim xPathTeile As New ArrayList
        Dim Index As Integer
        Dim Anzahl As Integer
        Dim i As Integer

        With xPathTeile
            ' Anzahl Speichern
            .Add("VIPListe")
            .Add("Index")
            Anzahl = CInt(C_DP.Read(xPathTeile, "0"))
            ' Index Speichern
            .Item(.Count - 1) = "Eintrag"
            .Add("[(KontaktID = """ & KontaktID & """ and StoreID = """ & StoreID & """)]")
            .Add("Index")
            Index = CInt(C_DP.Read(xPathTeile, "0"))
            ' Knoten löschen
            .Remove("Index")
            C_DP.Delete(xPathTeile)
            ' schleife durch jeden anderen Knoten und <Index> und Attribut ändern
            For i = Index + 1 To Anzahl - 1
                .Item(.Count - 1) = "[@ID=""" & i & """]"
                C_DP.WriteAttribute(xPathTeile, "ID", CStr(i - 1))
            Next
            'neue Anzahl (index) schreiben oder löschen
            .Remove(.Item(.Count - 1))
            .Remove("Eintrag")
            If C_DP.SubNoteCount(xPathTeile) = 1 Then
                .Add("Index")
                C_DP.Delete(xPathTeile)
            Else
                C_DP.Write("VIPListe", "Index", CStr(Anzahl - 1))
            End If

        End With

#If OVer < 14 Then
        FillPopupItems("VIPListe")
#Else
        RefreshRibbon()
#End If
        xPathTeile = Nothing
        C_DP.SpeichereXMLDatei()
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
            oExp = C_OLI.OutlookApplication.ActiveExplorer
            olMBars = oExp.CommandBars
            For Each olMBar In olMBars
                If olMBar.Name = MenuName Then
                    With C_HF
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
            FritzBoxDialCommandBar = olMBar
            AddCmdBar = olMBar

            With C_HF
                .NAR(olMBars) : .NAR(oExp)
            End With

            olMBar = Nothing
            olMBars = Nothing
            oExp = Nothing

        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "AddCmdBar")
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
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "AddButtonsToCmb")
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
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "AddPopupsToExplorer")
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

    Friend Sub FillPopupItems(ByRef XMLListBaseNode As String)
        ' XMLListBaseNode erlaubt: CallList, RingList, VIPListe

        Dim cPopUp As Office.CommandBarPopup = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , XMLListBaseNode, , False), Office.CommandBarPopup)
        Dim index As Integer
        Dim Anrufer As String
        Dim TelNr As String
        Dim Zeit As String

        Dim LANodeNames As New ArrayList
        Dim LANodeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim i As Integer

        index = CInt(C_DP.Read(XMLListBaseNode, "Index", "0"))

        LANodeNames.Add("Anrufer")
        LANodeNames.Add("TelNr")
        LANodeNames.Add("Zeit")
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne)
        With xPathTeile
            .Add(XMLListBaseNode)
            .Add("Eintrag")
        End With
        i = 1
        If Not XMLListBaseNode = "VIPListe" Then
            For ID = index + 9 To index Step -1

                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))
                Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))

                If Not TelNr = C_DP.P_Def_ErrorMinusOne Then
                    With cPopUp.Controls.Item(i)
                        If Anrufer = C_DP.P_Def_StringEmpty Then .Caption = TelNr Else .Caption = Anrufer
                        .TooltipText = "Zeit: " & Zeit & Environment.NewLine & "Telefonnummer: " & TelNr
                        .Parameter = CStr(ID Mod 10)
                        .Visible = True
                        .Tag = XMLListBaseNode & ";" & CStr(ID Mod 10)
                        i += 1
                    End With

                    xPathTeile.RemoveAt(xPathTeile.Count - 1)
                    With LANodeValues
                        .Item(0) = (C_DP.P_Def_ErrorMinusOne)
                        .Item(1) = (C_DP.P_Def_ErrorMinusOne)
                        .Item(2) = (C_DP.P_Def_ErrorMinusOne)
                    End With
                End If
            Next
        Else
            For ID = 0 To 9

                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID))
                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))

                If Not Anrufer = C_DP.P_Def_ErrorMinusOne And Not Anrufer = C_DP.P_Def_StringEmpty Then
                    With cPopUp.Controls.Item(i)
                        .Caption = Anrufer
                        .Parameter = CStr(ID Mod 10)
                        .Visible = True
                        .Tag = "VIPListe;" & CStr(ID)
                        i += 1
                    End With
                    With LANodeValues
                        .Item(0) = (C_DP.P_Def_ErrorMinusOne)
                        .Item(1) = (C_DP.P_Def_ErrorMinusOne)
                        .Item(2) = (C_DP.P_Def_ErrorMinusOne)
                    End With

                Else
                    If Not cPopUp.Controls.Item(i) Is Nothing Then
                        cPopUp.Controls.Item(i).Visible = False
                    End If
                End If
            Next
        End If
        cPopUp.Enabled = CommandBarPopupEnabled(cPopUp)
    End Sub

    Friend Sub SetVisibleButtons()
        ' Einstellungen für die Symbolleiste speichern
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Direktwahl").Visible = C_DP.P_CBSymbDirekt
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anrufmonitor").Visible = C_DP.P_CBSymbAnrMon
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anzeigen").Visible = C_DP.P_CBSymbAnrMon
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , "AnrListe").Visible = C_DP.P_CBSymbAnrListe
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , "Wwdh").Visible = C_DP.P_CBSymbWwdh
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Journalimport").Visible = C_DP.P_CBSymbJournalimport
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "AnrMonNeuStart").Visible = C_DP.P_CBSymbAnrMonNeuStart
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , "VIPListe").Visible = C_DP.P_CBSymbVIP
        Catch : End Try
    End Sub

    Friend Sub SetAnrMonButton(ByVal EinAus As Boolean)
        bool_banrmon = EinAus
        bAnrMonTimer = C_HF.SetTimer(200)
    End Sub

    Private Sub bAnrMonTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles bAnrMonTimer.Elapsed
        If Not FritzBoxDialCommandBar Is Nothing Then
            Dim btnAnrMon As Office.CommandBarButton = Nothing
            Try
                btnAnrMon = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anrufmonitor", , False), Office.CommandBarButton)
            Catch ex As Exception
                C_HF.LogFile("Fehler: btnAnrMon kann nicht gefunden werden.")
            End Try
            If Not btnAnrMon Is Nothing Then
                Select Case bool_banrmon
                    Case True
                        btnAnrMon.State = Office.MsoButtonState.msoButtonDown
                        btnAnrMon.TooltipText = "Beendet den Anrufmonitor"
                    Case False
                        btnAnrMon.State = Office.MsoButtonState.msoButtonUp
                        btnAnrMon.TooltipText = "Startet den Anrufmonitor"
                End Select
            End If

            C_HF.KillTimer(bAnrMonTimer)
            btnAnrMon = Nothing
        End If
    End Sub

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
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopWwdh)")
        End Try

        FillPopupItems("Wwdh")
        ' Direktwahl
        ePopWwdh.Visible = C_DP.P_CBSymbWwdh
        ePopWwdh.Enabled = CommandBarPopupEnabled(ePopWwdh)
        eBtnDirektwahl = AddButtonsToCmb(FritzBoxDialCommandBar, "Direktwahl", i, 326, "IconandCaption", "Direktwahl", "Direktwahl")
        i += 1

        eBtnDirektwahl.Visible = C_DP.P_CBSymbDirekt
        ' Symbol Anrufmonitor & Anzeigen
        eBtnAnrMonitor = AddButtonsToCmb(FritzBoxDialCommandBar, "Anrufmonitor", i, 815, "IconandCaption", "Anrufmonitor", "Anrufmonitor starten oder stoppen") '815

        eBtnAnzeigen = AddButtonsToCmb(FritzBoxDialCommandBar, "Anzeigen", i + 1, 682, "IconandCaption", "Anzeigen", "Letzte Anrufe anzeigen")
        i += 2

        eBtnAnrMonitor.Visible = C_DP.P_CBSymbAnrMon
        eBtnAnzeigen.Visible = eBtnAnrMonitor.Visible

        eBtnAnrMonNeuStart = AddButtonsToCmb(FritzBoxDialCommandBar, "Anrufmonitor neustarten", i, 37, "IconandCaption", "AnrMonNeuStart", "")
        eBtnAnrMonNeuStart.TooltipText = "Startet den Anrufmonitor neu."
        eBtnAnrMonNeuStart.Visible = C_DP.P_CBSymbAnrMonNeuStart

        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopAnr, "Rückruf", i, "AnrListe", "Letze Anrufer zurückrufen")
        Try
            ePopAnr1 = AddPopupItems(ePopAnr, 1) : ePopAnr2 = AddPopupItems(ePopAnr, 2)
            ePopAnr3 = AddPopupItems(ePopAnr, 3) : ePopAnr4 = AddPopupItems(ePopAnr, 4)
            ePopAnr5 = AddPopupItems(ePopAnr, 5) : ePopAnr6 = AddPopupItems(ePopAnr, 6)
            ePopAnr7 = AddPopupItems(ePopAnr, 7) : ePopAnr8 = AddPopupItems(ePopAnr, 8)
            ePopAnr9 = AddPopupItems(ePopAnr, 9) : ePopAnr10 = AddPopupItems(ePopAnr, 10)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopAnr)")
        End Try
        FillPopupItems("AnrListe")
        ePopAnr.Visible = C_DP.P_CBSymbAnrListe
        ePopAnr.Enabled = CommandBarPopupEnabled(ePopAnr)
        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopVIP, "VIP", i, "VIPListe", "VIP anrufen")
        Try
            ePopVIP1 = AddPopupItems(ePopVIP, 1) : ePopVIP2 = AddPopupItems(ePopVIP, 2)
            ePopVIP3 = AddPopupItems(ePopVIP, 3) : ePopVIP4 = AddPopupItems(ePopVIP, 4)
            ePopVIP5 = AddPopupItems(ePopVIP, 5) : ePopVIP6 = AddPopupItems(ePopVIP, 6)
            ePopVIP7 = AddPopupItems(ePopVIP, 7) : ePopVIP8 = AddPopupItems(ePopVIP, 8)
            ePopVIP9 = AddPopupItems(ePopVIP, 9) : ePopVIP10 = AddPopupItems(ePopVIP, 10)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopVIP)")
        End Try
        FillPopupItems("VIPListe")
        i += 1
        ePopVIP.Visible = C_DP.P_CBSymbVIP
        ePopVIP.Enabled = CommandBarPopupEnabled(ePopVIP)

        eBtnJournalimport = AddButtonsToCmb(FritzBoxDialCommandBar, "Journalimport", i, 591, "IconandCaption", "Journalimport", "Importiert die Anrufliste der Fritz!Box als Journaleinträge")
        eBtnJournalimport.Visible = C_DP.P_CBSymbJournalimport
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

    Private Function CommandBarPopupEnabled(ByVal control As Office.CommandBarPopup) As Boolean
        Dim XMLListBaseNode As String
        Dim xPathTeile As New ArrayList

        Select Case control.Tag
            Case "Wwdh"
                XMLListBaseNode = C_DP.NameListCALL
            Case "AnrListe"
                XMLListBaseNode = C_DP.NameListRING
            Case Else ' "VIPListe"
                XMLListBaseNode = C_DP.NameListVIP
        End Select

        Return CBool(IIf(Not C_DP.Read(XMLListBaseNode, "Index", C_DP.P_Def_ErrorMinusOne) = C_DP.P_Def_ErrorMinusOne, True, False))
    End Function

#End If
#If OVer = 11 Then
    Sub InspectorSybolleisteErzeugen(ByVal Inspector As Outlook.Inspector, _
                                     ByRef iPopRWS As Office.CommandBarPopup, ByRef iBtnWwh As Office.CommandBarButton, _
                                     ByRef iBtnRws11880 As Office.CommandBarButton, ByRef iBtnRWSDasTelefonbuch As Office.CommandBarButton, _
                                     ByRef iBtnRWStelSearch As Office.CommandBarButton, ByRef iBtnRWSAlle As Office.CommandBarButton, _
                                     ByRef iBtnKontakterstellen As Office.CommandBarButton, ByRef iBtnVIP As Office.CommandBarButton, _
                                     ByRef iBtnNotiz As Office.CommandBarButton)

        Dim cmbs As Office.CommandBars = Inspector.CommandBars
        Dim cmb As Office.CommandBar = Nothing
        Dim cmbErstellen As Boolean = True
        Dim i As Integer = 1

        If C_DP.P_CBSymbRWSuche Then
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
                'iBtnRwsGoYellow = AddPopupItems(iPopRWS, 1)
                iBtnRws11880 = AddPopupItems(iPopRWS, 2)
                iBtnRWSDasTelefonbuch = AddPopupItems(iPopRWS, 3)
                iBtnRWStelSearch = AddPopupItems(iPopRWS, 4)
                iBtnRWSAlle = AddPopupItems(iPopRWS, 5)
                'Dim rwsNamen() As String = {"GoYellow", "11880", "DasTelefonbuch", "tel.search.ch", "Alle"}
                'Dim rwsToolTipp() As String = {"Rückwärtssuche mit 'www.goyellow.de'", "Rückwärtssuche mit 'www.11880.com'", "Rückwärtssuche mit 'www.dastelefonbuch.de'", "Rückwärtssuche mit 'tel.search.ch'", "Rückwärtssuche mit allen Anbietern."}

                Dim rwsNamen() As String = {"11880", "DasTelefonbuch", "tel.search.ch", "Alle"}
                Dim rwsToolTipp() As String = {"Rückwärtssuche mit 'www.11880.com'", "Rückwärtssuche mit 'www.dastelefonbuch.de'", "Rückwärtssuche mit 'tel.search.ch'", "Rückwärtssuche mit allen Anbietern."}
                For i = 0 To 3
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
                        If CLng(C_DP.Read("VIPListe", "Anzahl", "0")) >= 10 Then
                            .TooltipText = "Die VIP-Liste ist mit 10 Einträgen bereits voll."
                            .Enabled = False
                        Else
                            .TooltipText = "Füge diesen Kontakt der VIP-Liste hinzu."
                        End If
                        .State = Office.MsoButtonState.msoButtonUp
                    End If
                    .Visible = C_DP.P_CBSymbVIP
                End With
                'iBtnNotiz = AddButtonsToCmb(cmb, "Notiz", i, 2056, "IconandCaption", "Notiz", "Einen Notizeintrag hinzufügen")
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
                    C_HF.NAR(olLink) : olLink = Nothing
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
    Friend Sub WähleDirektwahl()
        P_CallClient.Wählbox(Nothing, C_DP.P_Def_StringEmpty, True, C_DP.P_Def_StringEmpty)
    End Sub

    Friend Sub ÖffneEinstellungen()
        ThisAddIn.P_Config.ShowDialog()
    End Sub

    Friend Sub ÖffneJournalImport()
        Dim formjournalimort As New formJournalimport(C_AnrMon, C_HF, C_DP, True)
    End Sub

    Friend Sub ÖffneAnrMonAnzeigen()
        Dim forman As New formAnrMon(False, C_DP, C_HF, C_AnrMon, C_OLI, C_KF)
    End Sub

    Friend Sub AnrMonNeustarten()
        C_AnrMon.AnrMonReStart()
    End Sub

    Friend Sub WählenExplorer()
        If Not C_OLI.OutlookApplication Is Nothing Then
            Dim ActiveExplorer As Outlook.Explorer = C_OLI.OutlookApplication.ActiveExplorer
            Dim oSel As Outlook.Selection = ActiveExplorer.Selection
            P_CallClient.WählboxStart(oSel)
            C_HF.NAR(oSel) : C_HF.NAR(ActiveExplorer)
            oSel = Nothing : ActiveExplorer = Nothing
        End If
    End Sub

#End Region

#Region "Inspector Button Click"
    Friend Sub WählenInspector()
        P_CallClient.WählenAusInspector()
    End Sub

    Friend Sub KontaktErstellen()
        C_KF.ZeigeKontaktAusJournal()
    End Sub

    Friend Sub RWS11880(ByVal insp As Outlook.Inspector)
        F_RWS.Rückwärtssuche(formRWSuche.Suchmaschine.RWS11880, insp)
    End Sub

    Friend Sub RWSDasTelefonbuch(ByVal insp As Outlook.Inspector)
        F_RWS.Rückwärtssuche(formRWSuche.Suchmaschine.RWSDasTelefonbuch, insp)
    End Sub

    Friend Sub RWSTelSearch(ByVal insp As Outlook.Inspector)
        F_RWS.Rückwärtssuche(formRWSuche.Suchmaschine.RWStelSearch, insp)
    End Sub

    Friend Sub RWSAlle(ByVal insp As Outlook.Inspector)
        F_RWS.Rückwärtssuche(formRWSuche.Suchmaschine.RWSAlle, insp)
    End Sub
#End Region

End Class