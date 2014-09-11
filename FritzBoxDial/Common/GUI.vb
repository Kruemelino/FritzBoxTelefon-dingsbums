#If OVer < 14 Then
Imports Microsoft.Office.Core
#End If
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
    Private C_WClient As Wählclient
    Private C_AnrMon As AnrufMonitor
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts
    Private C_FBox As FritzBox
    Private C_PopUp As Popup
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

    Friend Sub New(ByVal HelferKlasse As Helfer, _
           ByVal DataProviderKlasse As DataProvider, _
           ByVal Inverssuche As formRWSuche, _
           ByVal KontaktKlasse As Contacts, _
           ByVal PopUpKlasse As Popup)

        C_HF = HelferKlasse
        C_DP = DataProviderKlasse
        F_RWS = Inverssuche
        C_KF = KontaktKlasse
        C_PopUp = PopUpKlasse
    End Sub

#Region "Ribbon Inspector Office 2007 & Office 2010 & Office 2013" ' Ribbon Inspektorfenster
#If Not OVer = 11 Then
    Public Sub OnActionWählen(ByVal control As Office.IRibbonControl)
        WählenInspector()
    End Sub

    Public Sub OnActionKontakterstellen(ByVal control As Office.IRibbonControl)
        KontaktErstellen()
    End Sub

    Public Sub OnActionRWS(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        Select Case control.Tag
            Case "RWS11880"
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWS11880, Insp)
            Case "RWSDasOertliche"
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSDasOertliche, Insp)
            Case "RWSDasTelefonbuch"
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSDasTelefonbuch, Insp)
            Case "RWSTelSearch"
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWStelSearch, Insp)
            Case "RWSAlle"
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSAlle, Insp)
        End Select
    End Sub

    Public Function GroupVisible(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
#If OVer = 14 Then
        Dim ActiveExplorer As Outlook.Explorer
        Dim oapp As New Outlook.Application
        Dim anzeigen As Boolean
        ActiveExplorer = oapp.ActiveExplorer
        anzeigen = ActiveExplorer IsNot Nothing
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
                If CBool(InStr(olJournal.Body, "Tel.-Nr.: " & C_DP.P_Def_StringUnknown, CompareMethod.Text)) Then
                    Return False
                Else
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Private Function SetLabelJournal(ByVal control As Office.IRibbonControl) As String
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.JournalItem Then
            Dim olJournal As Outlook.JournalItem = CType(Insp.CurrentItem, Outlook.JournalItem)
            If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", CompareMethod.Text) = 0 Then
#If Not OVer = 15 Then
                Dim olLink As Outlook.Link = Nothing
                For Each olLink In olJournal.Links
                    Try
                        If TypeOf olLink.Item Is Outlook.ContactItem Then Return C_DP.P_CMB_Kontakt_Anzeigen
                        Exit For
                    Catch
                        Return C_DP.P_CMB_Kontakt_Erstellen
                    End Try
                Next
                C_HF.NAR(olLink) : olLink = Nothing
#End If
            Else
                Return C_DP.P_CMB_Kontakt_Erstellen
            End If
        End If
        Return C_DP.P_CMB_Kontakt_Erstellen
    End Function

    Private Function SetScreenTipJournal(ByVal control As Office.IRibbonControl) As String
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.JournalItem Then
            Dim olJournal As Outlook.JournalItem = CType(Insp.CurrentItem, Outlook.JournalItem)
            If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", CompareMethod.Text) = 0 Then
#If Not OVer = 15 Then
                Dim olLink As Outlook.Link = Nothing
                For Each olLink In olJournal.Links
                    Try
                        If TypeOf olLink.Item Is Outlook.ContactItem Then Return C_DP.P_CMB_Kontakt_Anzeigen_ToolTipp
                        Exit For
                    Catch
                        Return C_DP.P_CMB_Kontakt_Anzeigen_Error_ToolTipp
                    End Try
                Next
                C_HF.NAR(olLink) : olLink = Nothing
#End If
            Else
                Return C_DP.P_CMB_Kontakt_Erstellen_ToolTipp
            End If
        End If
        Return C_DP.P_CMB_Kontakt_Erstellen_ToolTipp
    End Function

    Public Sub OnActionNote(ByVal control As Office.IRibbonControl)
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        C_KF.AddNote(CType(Insp.CurrentItem, Outlook.ContactItem))
    End Sub


    ''' <summary>
    ''' Ermittelt das Label des Ribbon-Objektes ausgehend von der Ribbon-id für Inspektoren
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInspLabel(ByVal control As Office.IRibbonControl) As String
        Select Case control.Id
            Case "TabContact", "TabJournal", "TabReadMessage"
                GetInspLabel = C_DP.P_Def_Addin_LangName
            Case "Button_C1", "Button_M1", "Button_J1"
                GetInspLabel = C_DP.P_CMB_Dial
            Case "mnu_C01", "mnu_J01"
                GetInspLabel = C_DP.P_CMB_Insp_RWS
            Case "Button_J2"
                GetInspLabel = SetLabelJournal(control)
            Case "tButton_C1"
                GetInspLabel = C_DP.P_CMB_Insp_VIP
            Case "Button_C2"
                GetInspLabel = C_DP.P_CMB_Insp_Note
            Case "btn_C01", "btn_J01"
                GetInspLabel = C_DP.P_RWS11880_Name
            Case "btn_C02", "btn_J02"
                GetInspLabel = C_DP.P_RWSDasOertliche_Name
            Case "btn_C03", "btn_J03"
                GetInspLabel = C_DP.P_RWSDasTelefonbuch_Name
            Case "btn_C04", "btn_J04"
                GetInspLabel = C_DP.P_RWSTelSearch_Name
            Case "btn_C05", "btn_J05"
                GetInspLabel = C_DP.P_RWSAlle_Name
            Case Else
                GetInspLabel = C_DP.P_Def_ErrorMinusOne_String
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Inspektoren
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInspScreenTipp(ByVal control As Office.IRibbonControl) As String
        Select Case control.Id
            Case "Button_C1", "Button_M1", "Button_J1"
                GetInspScreenTipp = C_DP.P_CMB_Dial_ToolTipp
            Case "mnu_C01", "mnu_J01"
                GetInspScreenTipp = C_DP.P_CMB_Insp_RWS_ToolTipp
            Case "Button_J2"
                GetInspScreenTipp = SetScreenTipJournal(control)
            Case "tButton_C1"
                GetInspScreenTipp = GetScreenTipVIP(control)
            Case "Button_C2"
                GetInspScreenTipp = C_DP.P_CMB_Insp_Note_ToolTipp
            Case "btn_C01", "btn_J01"
                GetInspScreenTipp = C_DP.P_RWS_ToolTipp(C_DP.P_RWS11880_Link)
            Case "btn_C02", "btn_J02"
                GetInspScreenTipp = C_DP.P_RWS_ToolTipp(C_DP.P_RWSDasOertliche_Link)
            Case "btn_C03", "btn_J03"
                GetInspScreenTipp = C_DP.P_RWS_ToolTipp(C_DP.P_RWSDasTelefonbuch_Link)
            Case "btn_C04", "btn_J04"
                GetInspScreenTipp = C_DP.P_RWS_ToolTipp(C_DP.P_RWSTelSearch_Link)
            Case "btn_C05", "btn_J05"
                GetInspScreenTipp = C_DP.P_RWS_ToolTipp()
            Case Else
                GetInspScreenTipp = C_DP.P_Def_ErrorMinusOne_String
        End Select
    End Function


#End Region 'Ribbon Inspector

#Region "Ribbon Expector Office 2010 & Office 2013" 'Ribbon Explorer
#If oVer >= 14 Then
    Sub Ribbon_Load(ByVal Ribbon As Office.IRibbonUI)
        RibbonObjekt = Ribbon
    End Sub

    Public Function DynMenüfüllen(ByVal control As Office.IRibbonControl) As String

        Dim XMLListBaseNode As String

        Dim index As Integer
        'Dim i As Integer

        Dim Anrufer As String
        Dim TelNr As String
        Dim Zeit As String

        Dim LANodeNames As New ArrayList
        Dim LANodeValues As New ArrayList
        Dim xPathTeile As New ArrayList

        Dim RibbonListStrBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf & _
                                                                      "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)

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
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)
        With xPathTeile
            .Add(XMLListBaseNode)
            .Add("Eintrag")
        End With

        If Not XMLListBaseNode = C_DP.P_Def_NameListVIP Then
            For ID = index + 9 To index Step -1

                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))
                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))

                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                    Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                    Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))

                    GetButtonXMLString(RibbonListStrBuilder, _
                            CStr(ID Mod 10), _
                            CStr(IIf(Anrufer = C_DP.P_Def_ErrorMinusOne_String, TelNr, Anrufer)), _
                            XMLListBaseNode, _
                            C_DP.P_CMB_ToolTipp(Zeit, TelNr))
                    LANodeValues.Item(0) = C_DP.P_Def_ErrorMinusOne_String
                    LANodeValues.Item(1) = C_DP.P_Def_ErrorMinusOne_String
                    LANodeValues.Item(2) = C_DP.P_Def_ErrorMinusOne_String
                End If
            Next
        Else
            For ID = 0 To index - 1
                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                If Not Anrufer = C_DP.P_Def_ErrorMinusOne_String Then

                    GetButtonXMLString(RibbonListStrBuilder, _
                            CStr(ID Mod index), _
                            Anrufer, _
                            XMLListBaseNode, _
                            C_DP.P_Def_StringEmpty)

                    LANodeValues.Item(0) = C_DP.P_Def_ErrorMinusOne_String
                End If
            Next
        End If

        RibbonListStrBuilder.Append("</menu>")

        DynMenüfüllen = RibbonListStrBuilder.ToString
        LANodeNames = Nothing
        LANodeValues = Nothing
        xPathTeile = Nothing
    End Function

    Private Sub GetButtonXMLString(ByRef StrBuilder As StringBuilder, ByVal ID As String, ByVal Label As String, ByVal Tag As String, SuperTip As String)
        Dim Werte(3) As String

        Werte(0) = ID
        Werte(1) = Label
        Werte(2) = Tag
        Werte(3) = SuperTip
        ' Nicht zugelassene Zeichen der XML-Notifikation ersetzen.
        ' Zeichen	Notation in XML
        ' <	        &lt;    &#60;
        ' >	        &gt;    &#62;
        ' &	        &amp;   &#38; Zweimal anfügen, da es ansonsten ignoriert wird
        ' "	        &quot;  &#34;
        ' '	        &apos;  &#38;

        For i = LBound(Werte) To UBound(Werte)
            If Not Werte(i) = C_DP.P_Def_StringEmpty Then
                Werte(i) = Replace(Werte(i), "&", "&amp;&amp;", , , CompareMethod.Text)
                Werte(i) = Replace(Werte(i), "&amp;&amp;#", "&#", , , CompareMethod.Text) ' Deizmalcode wiederherstellen
                Werte(i) = Replace(Werte(i), "<", "&lt;", , , CompareMethod.Text)
                Werte(i) = Replace(Werte(i), ">", "&gt;", , , CompareMethod.Text)
                Werte(i) = Replace(Werte(i), Chr(34), "&quot;", , , CompareMethod.Text)
                Werte(i) = Replace(Werte(i), "'", "&apos;", , , CompareMethod.Text)
            End If
        Next

        With StrBuilder
            .Append("<button id=""button_" & Werte(0) & """ ")
            .Append("label=""" & Werte(1) & """ ")
            .Append("onAction=""OnActionListen"" ")
            .Append("tag=""" & Werte(2) & ";" & Werte(0) & """ ")
            If Not Werte(3) = C_DP.P_Def_StringEmpty Then
                .Append("supertip=""" & Werte(3) & """")
            End If
            .Append("/>" & vbCrLf)
        End With
    End Sub

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

        Return CBool(IIf(Not C_DP.Read(XMLListBaseNode, "Index", C_DP.P_Def_ErrorMinusOne_String) = C_DP.P_Def_ErrorMinusOne_String, True, False))
    End Function

    Public Function GetPressed(ByVal control As Office.IRibbonControl) As Boolean
        GetPressed = False
        If C_AnrMon IsNot Nothing Then
            GetPressed = C_AnrMon.AnrMonAktiv
        End If
    End Function

    Public Function GetImage(ByVal control As Office.IRibbonControl) As String
        GetImage = "PersonaStatusBusy"
        If C_AnrMon IsNot Nothing Then
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
        If RibbonObjekt IsNot Nothing Then
            RibbonObjekt.Invalidate()
        End If
    End Sub

    ''' <summary>
    ''' Ermittelt das Label des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetExplLabel(ByVal control As Office.IRibbonControl) As String
        Select Case control.Id
            Case "TabContact", "TabMail", "TabJournal", "TabCalendar"
                GetExplLabel = C_DP.P_Def_Addin_LangName
            Case "dynMwwdh_C", "dynMwwdh_M", "dynMwwdh_J", "dynMwwdh_K"
                GetExplLabel = C_DP.P_CMB_WWDH
            Case "dynMAnrListe_C", "dynMAnrListe_M", "dynMAnrListe_J", "dynMAnrListe_K"
                GetExplLabel = C_DP.P_CMB_CallBack
            Case "dynMVIPListe_C", "dynMVIPListe_M", "dynMVIPListe_J", "dynMVIPListe_K"
                GetExplLabel = C_DP.P_CMB_VIP
            Case "Button_C1", "Button_M1", "Button_J1", "Button_K1"
                GetExplLabel = C_DP.P_CMB_Dial
            Case "Button_C2", "Button_M2", "Button_J2", "Button_K2"
                GetExplLabel = C_DP.P_CMB_Direktwahl
            Case "btnSplit_C", "btnSplit_M", "btnSplit_J", "btnSplit_K"
                GetExplLabel = C_DP.P_CMB_AnrMon
            Case "AnrMonBtn_C2", "AnrMonBtn_M2", "AnrMonBtn_J2", "AnrMonBtn_K2"
                GetExplLabel = C_DP.P_CMB_AnrMonNeuStart
            Case "AnrMonBtn_C1", "AnrMonBtn_M1", "AnrMonBtn_J1", "AnrMonBtn_K1"
                GetExplLabel = C_DP.P_CMB_AnrMonAnzeigen
            Case "AnrMonBtn_C4", "AnrMonBtn_M4", "AnrMonBtn_J4", "AnrMonBtn_K4"
                GetExplLabel = C_DP.P_CMB_Journal
            Case "ContextMenuContactItem_B", "ContextMenuJournalItem_F", "ContextMenuMailItem_H"
                GetExplLabel = C_DP.P_CMB_ContextMenueItemCall
            Case "ContextMenuContactItem_C"
                GetExplLabel = C_DP.P_CMB_ContextMenueItemVIP
            Case Else
                GetExplLabel = C_DP.P_Def_ErrorMinusOne_String
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetExplScreenTipp(ByVal control As Office.IRibbonControl) As String
        Select Case control.Id
            Case "dynMwwdh_C", "dynMwwdh_M", "dynMwwdh_J", "dynMwwdh_K"
                GetExplScreenTipp = C_DP.P_CMB_WWDH_ToolTipp
            Case "dynMAnrListe_C", "dynMAnrListe_M", "dynMAnrListe_J", "dynMAnrListe_K"
                GetExplScreenTipp = C_DP.P_CMB_CallBack_ToolTipp
            Case "dynMVIPListe_C", "dynMVIPListe_M", "dynMVIPListe_J", "dynMVIPListe_K"
                GetExplScreenTipp = C_DP.P_CMB_VIP_ToolTipp
            Case "Button_C1", "Button_M1", "Button_J1", "Button_K1"
                GetExplScreenTipp = C_DP.P_CMB_Dial_ToolTipp
            Case "Button_C2", "Button_M2", "Button_J2", "Button_K2"
                GetExplScreenTipp = C_DP.P_CMB_Direktwahl_ToolTipp
            Case "btnSplit_C", "btnSplit_M", "btnSplit_J", "btnSplit_K"
                GetExplScreenTipp = C_DP.P_CMB_AnrMon_ToolTipp
            Case "AnrMonBtn_C2", "AnrMonBtn_M2", "AnrMonBtn_J2", "AnrMonBtn_K2"
                GetExplScreenTipp = C_DP.P_CMB_AnrMonNeuStart_ToolTipp
            Case "AnrMonBtn_C1", "AnrMonBtn_M1", "AnrMonBtn_J1", "AnrMonBtn_K1"
                GetExplScreenTipp = C_DP.P_CMB_AnrMonAnzeigen_ToolTipp
            Case "AnrMonBtn_C4", "AnrMonBtn_M4", "AnrMonBtn_J4", "AnrMonBtn_K4"
                GetExplScreenTipp = C_DP.P_CMB_Journal_ToolTipp
            Case "Einstellungen_C", "Einstellungen_M", "Einstellungen_J", "Einstellungen_K"
                GetExplScreenTipp = C_DP.P_CMB_Setup_ToolTipp
            Case Else
                GetExplScreenTipp = C_DP.P_Def_ErrorMinusOne_String
        End Select
    End Function

    Public Function GetVisibleAnrMonFKT(ByVal control As Office.IRibbonControl) As Boolean
        Return C_DP.P_CBUseAnrMon
    End Function

    Public Function GetEnabledJI(ByVal control As Office.IRibbonControl) As Boolean
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
#If OVer >= 14 Then
        RibbonObjekt.Invalidate()
#End If
    End Sub

    Public Function GetPressedVIP(ByVal control As Office.IRibbonControl) As Boolean
        GetPressedVIP = False
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim olContact As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)
            Return IsVIP(olContact)
        End If
    End Function

    Private Function GetScreenTipVIP(ByVal control As Office.IRibbonControl) As String
        GetScreenTipVIP = C_DP.P_Def_StringEmpty
        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim aktKontakt As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)
            If IsVIP(aktKontakt) Then
                GetScreenTipVIP = C_DP.P_CMB_VIP_Entfernen_ToolTipp
            Else
                'If CLng(C_DP.Read(C_DP.P_Def_NameListVIP, "Index", "0")) >= 10 Then
                '    GetScreenTipVIP = "Die VIP-Liste ist mit 10 Einträgen bereits voll."
                'Else
                GetScreenTipVIP = C_DP.P_CMB_VIP_Hinzufügen_ToolTipp
                'End If
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

        xPathTeile.Add(C_DP.P_Def_NameListVIP)
        xPathTeile.Add("Eintrag")
        xPathTeile.Add("[(KontaktID = """ & KontaktID & """ and StoreID = """ & StoreID & """)]")
        IsVIP = Not C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne_String) = C_DP.P_Def_ErrorMinusOne_String
        xPathTeile = Nothing
    End Function

    Friend Overloads Function AddVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        Dim Anrufer As String = Replace(aktKontakt.FullName & " (" & aktKontakt.CompanyName & ")", " ()", "")
        Dim Index As Integer = CInt(C_DP.Read(C_DP.P_Def_NameListVIP, "Index", "0"))
        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        xPathTeile.Add(C_DP.P_Def_NameListVIP)
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
            xPathTeile.Clear()
            xPathTeile.Add(.P_Def_NameListVIP)
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
        FillPopupItems(C_DP.P_Def_NameListVIP)
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
            .Add(C_DP.P_Def_NameListVIP)
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
                C_DP.Write(C_DP.P_Def_NameListVIP, "Index", CStr(Anzahl - 1))
            End If

        End With

#If OVer < 14 Then
        FillPopupItems(C_DP.P_Def_NameListVIP)
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
                .Name = C_DP.P_Def_Addin_KurzName
                .NameLocal = C_DP.P_Def_Addin_KurzName
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
    ByVal btnCaption As String, ByVal PosIndex As Integer, ByVal btnFaceId As Long, ByVal btnStyle As Office.MsoButtonStyle, _
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
                    .Style = btnStyle
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
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)
        With xPathTeile
            .Add(XMLListBaseNode)
            .Add("Eintrag")
        End With
        i = 1
        If Not XMLListBaseNode = C_DP.P_Def_NameListVIP Then
            For ID = index + 9 To index Step -1

                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))
                Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))

                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                    With cPopUp.Controls.Item(i)
                        If Anrufer = C_DP.P_Def_ErrorMinusOne_String Then .Caption = TelNr Else .Caption = Anrufer
                        .TooltipText = C_DP.P_CMB_ToolTipp(Zeit, TelNr)
                        .Parameter = CStr(ID Mod 10)
                        .Visible = True
                        .Tag = XMLListBaseNode & ";" & CStr(ID Mod 10)
                        i += 1
                    End With

                    'xPathTeile.RemoveAt(xPathTeile.Count - 1)
                    With LANodeValues
                        .Item(0) = (C_DP.P_Def_ErrorMinusOne_String)
                        .Item(1) = (C_DP.P_Def_ErrorMinusOne_String)
                        .Item(2) = (C_DP.P_Def_ErrorMinusOne_String)
                    End With
                End If
            Next
        Else
            For ID = 0 To 9

                C_DP.ReadXMLNode(xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID))
                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))

                If Not Anrufer = C_DP.P_Def_ErrorMinusOne_String And Not Anrufer = C_DP.P_Def_StringEmpty Then
                    With cPopUp.Controls.Item(i)
                        .Caption = Anrufer
                        .Parameter = CStr(ID Mod 10)
                        .Visible = True
                        .Tag = XMLListBaseNode & ";" & CStr(ID)
                        i += 1
                    End With
                    With LANodeValues
                        .Item(0) = (C_DP.P_Def_ErrorMinusOne_String)
                        .Item(1) = (C_DP.P_Def_ErrorMinusOne_String)
                        .Item(2) = (C_DP.P_Def_ErrorMinusOne_String)
                    End With

                Else
                    If cPopUp.Controls.Item(i) IsNot Nothing Then
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
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anrufmonitor").Visible = C_DP.P_CBSymbAnrMon
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anzeigen").Visible = C_DP.P_CBSymbAnrMon
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , C_DP.P_Def_NameListRING).Visible = C_DP.P_CBSymbAnrListe
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , C_DP.P_Def_NameListCALL).Visible = C_DP.P_CBSymbWwdh
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Journalimport").Visible = C_DP.P_CBSymbJournalimport
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "AnrMonNeuStart").Visible = C_DP.P_CBSymbAnrMonNeuStart
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , C_DP.P_Def_NameListVIP).Visible = C_DP.P_CBSymbVIP
        Catch : End Try
    End Sub

    Friend Sub SetAnrMonButton(ByVal EinAus As Boolean)
        bool_banrmon = EinAus
        bAnrMonTimer = C_HF.SetTimer(200)
    End Sub

    Private Sub bAnrMonTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles bAnrMonTimer.Elapsed
        If FritzBoxDialCommandBar IsNot Nothing Then
            Dim btnAnrMon As Office.CommandBarButton = Nothing
            Try
                btnAnrMon = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , "Anrufmonitor", , False), Office.CommandBarButton)
            Catch ex As Exception
                C_HF.LogFile("Fehler: btnAnrMon kann nicht gefunden werden.")
            End Try
            If btnAnrMon IsNot Nothing Then
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

        FritzBoxDialCommandBar = AddCmdBar(C_DP.P_Def_Addin_KurzName, True)

        eBtnWaehlen = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_Dial, 1, 568, MsoButtonStyle.msoButtonIconAndCaption, "Wählen", C_DP.P_CMB_Dial_ToolTipp)

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopWwdh, C_DP.P_CMB_WWDH, i, C_DP.P_Def_NameListCALL, C_DP.P_CMB_WWDH_ToolTipp)
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

        FillPopupItems(C_DP.P_Def_NameListCALL)
        ' Direktwahl
        ePopWwdh.Visible = C_DP.P_CBSymbWwdh
        ePopWwdh.Enabled = CommandBarPopupEnabled(ePopWwdh)
        eBtnDirektwahl = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_Direktwahl, i, 326, MsoButtonStyle.msoButtonIconAndCaption, "Direktwahl", C_DP.P_CMB_Direktwahl_ToolTipp)
        i += 1

        eBtnDirektwahl.Visible = C_DP.P_CBSymbDirekt
        ' Symbol Anrufmonitor & Anzeigen
        eBtnAnrMonitor = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_AnrMon, i, 815, MsoButtonStyle.msoButtonIconAndCaption, "Anrufmonitor", C_DP.P_CMB_AnrMon_ToolTipp) '815

        eBtnAnzeigen = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_AnrMonAnzeigen, i + 1, 682, MsoButtonStyle.msoButtonIconAndCaption, "Anzeigen", C_DP.P_CMB_AnrMonAnzeigen_ToolTipp)
        i += 2

        eBtnAnrMonitor.Visible = C_DP.P_CBSymbAnrMon
        eBtnAnzeigen.Visible = eBtnAnrMonitor.Visible

        eBtnAnrMonNeuStart = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_AnrMonNeuStart, i, 37, MsoButtonStyle.msoButtonIconAndCaption, "AnrMonNeuStart", C_DP.P_CMB_AnrMonNeuStart_ToolTipp)

        eBtnAnrMonNeuStart.Visible = C_DP.P_CBSymbAnrMonNeuStart

        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopAnr, C_DP.P_CMB_CallBack, i, C_DP.P_Def_NameListRING, C_DP.P_CMB_CallBack_ToolTipp)
        Try
            ePopAnr1 = AddPopupItems(ePopAnr, 1) : ePopAnr2 = AddPopupItems(ePopAnr, 2)
            ePopAnr3 = AddPopupItems(ePopAnr, 3) : ePopAnr4 = AddPopupItems(ePopAnr, 4)
            ePopAnr5 = AddPopupItems(ePopAnr, 5) : ePopAnr6 = AddPopupItems(ePopAnr, 6)
            ePopAnr7 = AddPopupItems(ePopAnr, 7) : ePopAnr8 = AddPopupItems(ePopAnr, 8)
            ePopAnr9 = AddPopupItems(ePopAnr, 9) : ePopAnr10 = AddPopupItems(ePopAnr, 10)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopAnr)")
        End Try
        FillPopupItems(C_DP.P_Def_NameListRING)
        ePopAnr.Visible = C_DP.P_CBSymbAnrListe
        ePopAnr.Enabled = CommandBarPopupEnabled(ePopAnr)
        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopVIP, C_DP.P_CMB_VIP, i, C_DP.P_Def_NameListVIP, C_DP.P_CMB_VIP_ToolTipp)
        Try
            ePopVIP1 = AddPopupItems(ePopVIP, 1) : ePopVIP2 = AddPopupItems(ePopVIP, 2)
            ePopVIP3 = AddPopupItems(ePopVIP, 3) : ePopVIP4 = AddPopupItems(ePopVIP, 4)
            ePopVIP5 = AddPopupItems(ePopVIP, 5) : ePopVIP6 = AddPopupItems(ePopVIP, 6)
            ePopVIP7 = AddPopupItems(ePopVIP, 7) : ePopVIP8 = AddPopupItems(ePopVIP, 8)
            ePopVIP9 = AddPopupItems(ePopVIP, 9) : ePopVIP10 = AddPopupItems(ePopVIP, 10)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopVIP)")
        End Try
        FillPopupItems(C_DP.P_Def_NameListVIP)
        i += 1
        ePopVIP.Visible = C_DP.P_CBSymbVIP
        ePopVIP.Enabled = CommandBarPopupEnabled(ePopVIP)

        eBtnJournalimport = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_Journal, i, 591, MsoButtonStyle.msoButtonIconAndCaption, "Journalimport", C_DP.P_CMB_Journal_ToolTipp)
        eBtnJournalimport.Visible = C_DP.P_CBSymbJournalimport
        i += 1
        eBtnEinstellungen = AddButtonsToCmb(FritzBoxDialCommandBar, C_DP.P_CMB_Setup, i, 548, MsoButtonStyle.msoButtonIconAndCaption, "Einstellungen", C_DP.P_CMB_Setup_ToolTipp)
        i += 1

        eBtnWaehlen.TooltipText = C_DP.P_CMB_Dial_ToolTipp
        ePopWwdh.TooltipText = C_DP.P_CMB_WWDH_ToolTipp
        eBtnAnrMonitor.TooltipText = C_DP.P_CMB_AnrMon_ToolTipp
        eBtnDirektwahl.TooltipText = C_DP.P_CMB_Direktwahl_ToolTipp
        eBtnAnzeigen.TooltipText = C_DP.P_CMB_AnrMonAnzeigen_ToolTipp
        eBtnAnrMonNeuStart.TooltipText = C_DP.P_CMB_AnrMonNeuStart_ToolTipp
        ePopAnr.TooltipText = C_DP.P_CMB_CallBack_ToolTipp
        ePopVIP.TooltipText = C_DP.P_CMB_VIP_ToolTipp
        eBtnJournalimport.TooltipText = C_DP.P_CMB_Journal_ToolTipp
        eBtnEinstellungen.TooltipText = C_DP.P_CMB_Setup_ToolTipp

    End Sub

    Private Function CommandBarPopupEnabled(ByVal control As Office.CommandBarPopup) As Boolean
        Dim XMLListBaseNode As String = C_DP.P_Def_ErrorMinusOne_String
        Dim xPathTeile As New ArrayList

        Select Case control.Tag
            Case C_DP.P_Def_NameListCALL
                XMLListBaseNode = C_DP.P_Def_NameListCALL
            Case C_DP.P_Def_NameListRING
                XMLListBaseNode = C_DP.P_Def_NameListRING
            Case C_DP.P_Def_NameListVIP ' "VIPListe"
                XMLListBaseNode = C_DP.P_Def_NameListVIP
        End Select

        If XMLListBaseNode = C_DP.P_Def_ErrorMinusOne_String Then
            CommandBarPopupEnabled = False
        Else
            CommandBarPopupEnabled = CBool(IIf(Not C_DP.Read(XMLListBaseNode, "Index", C_DP.P_Def_ErrorMinusOne_String) = C_DP.P_Def_ErrorMinusOne_String, True, False))
        End If
    End Function

#End If
#If OVer = 11 Then
    Sub InspectorSybolleisteErzeugen(ByVal Inspector As Outlook.Inspector, _
                                     ByRef iPopRWS As Office.CommandBarPopup, ByRef iBtnWwh As Office.CommandBarButton, _
                                     ByRef iBtnRwsDasOertliche As Office.CommandBarButton, ByRef iBtnRws11880 As Office.CommandBarButton, _
                                     ByRef iBtnRWSDasTelefonbuch As Office.CommandBarButton, ByRef iBtnRWStelSearch As Office.CommandBarButton, _
                                     ByRef iBtnRWSAlle As Office.CommandBarButton, _
                                     ByRef iBtnKontakterstellen As Office.CommandBarButton, ByRef iBtnVIP As Office.CommandBarButton)

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
                        If cmb.NameLocal = C_DP.P_Def_Addin_KurzName Then
                            cmbErstellen = False
                            Exit For
                        End If
                    Next
                End If
                If cmbErstellen Then
                    cmb = Inspector.CommandBars.Add(C_DP.P_Def_Addin_KurzName, Microsoft.Office.Core.MsoBarPosition.msoBarTop, , True)
                    With cmb
                        .NameLocal = C_DP.P_Def_Addin_KurzName
                        .Visible = True
                    End With
                    iBtnWwh = AddButtonsToCmb(cmb, C_DP.P_CMB_Dial, i, 568, MsoButtonStyle.msoButtonIconAndCaption, C_DP.P_Tag_Insp_Dial, C_DP.P_CMB_Dial_ToolTipp)
                    i += 1
                End If
            End If
            ' Kontakteinträge
            If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Or TypeOf Inspector.CurrentItem Is Outlook.JournalItem Then

                AddPopupsToExplorer(cmb, iPopRWS, C_DP.P_CMB_Insp_RWS, i, "RWS", C_DP.P_CMB_Insp_RWS_ToolTipp)
                i += 1
                iBtnRwsDasOertliche = AddPopupItems(iPopRWS, 1)
                iBtnRws11880 = AddPopupItems(iPopRWS, 2)
                iBtnRWSDasTelefonbuch = AddPopupItems(iPopRWS, 3)
                iBtnRWStelSearch = AddPopupItems(iPopRWS, 4)
                iBtnRWSAlle = AddPopupItems(iPopRWS, 5)

                Dim rwsNamen() As String = {C_DP.P_RWSDasOertliche_Name, _
                                            C_DP.P_RWS11880_Name, _
                                            C_DP.P_RWSDasTelefonbuch_Name, _
                                            C_DP.P_RWSTelSearch_Name, _
                                            C_DP.P_RWSAlle_Name}
                Dim rwsToolTipp() As String = {C_DP.P_RWS_ToolTipp(C_DP.P_RWSDasOertliche_Link), _
                                               C_DP.P_RWS_ToolTipp(C_DP.P_RWS11880_Link), _
                                               C_DP.P_RWS_ToolTipp(C_DP.P_RWSDasTelefonbuch_Link), _
                                               C_DP.P_RWS_ToolTipp(C_DP.P_RWSTelSearch_Link), _
                                               C_DP.P_RWS_ToolTipp()}

                For i = LBound(rwsNamen) To UBound(rwsNamen)
                    With iPopRWS.Controls.Item(i + 1)
                        .Tag = rwsNamen(i)
                        .Caption = rwsNamen(i)
                        .TooltipText = rwsToolTipp(i)
                        .Visible = True
                    End With
                Next
            End If
            If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Then
                iBtnVIP = AddButtonsToCmb(cmb, C_DP.P_CMB_Insp_VIP, i, 3710, MsoButtonStyle.msoButtonIconAndCaption, "VIP", C_DP.P_CMB_VIP_Hinzufügen_ToolTipp)
                Dim olKontact As Outlook.ContactItem = CType(Inspector.CurrentItem, Outlook.ContactItem)
                With iBtnVIP
                    If IsVIP(olKontact) Then
                        .State = Office.MsoButtonState.msoButtonDown
                        .TooltipText = C_DP.P_CMB_VIP_Entfernen_ToolTipp
                    Else
                        If CLng(C_DP.Read(C_DP.P_Def_NameListVIP, "Index", "0")) >= 10 Then
                            .TooltipText = C_DP.P_CMB_VIP_O11_Voll_ToolTipp
                            .Enabled = False
                        Else
                            .TooltipText = C_DP.P_CMB_VIP_Hinzufügen_ToolTipp
                        End If
                        .State = Office.MsoButtonState.msoButtonUp
                    End If
                    .Tag = C_DP.P_CMB_Insp_VIP
                    .Visible = C_DP.P_CBSymbVIP
                End With
            End If
            ' Journaleinträge
            If TypeOf Inspector.CurrentItem Is Outlook.JournalItem Then
                iBtnKontakterstellen = AddButtonsToCmb(cmb, _
                                                       C_DP.P_CMB_Kontakt_Erstellen, _
                                                       i, 1099, MsoButtonStyle.msoButtonIconAndCaption, _
                                                       C_DP.P_Tag_Insp_Kontakt, _
                                                       C_DP.P_CMB_Kontakt_Erstellen_ToolTipp)

                Dim olJournal As Outlook.JournalItem = CType(Inspector.CurrentItem, Outlook.JournalItem)
                If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", vbTextCompare) = 0 Then
                    Dim olLink As Outlook.Link = Nothing
                    For Each olLink In olJournal.Links
                        If TypeOf olLink.Item Is Outlook.ContactItem Then iBtnKontakterstellen.Caption = C_DP.P_CMB_Kontakt_Anzeigen
                        Exit For
                    Next
                    C_HF.NAR(olLink) : olLink = Nothing
                    iPopRWS.Enabled = True
                    iBtnWwh.Enabled = Not CBool(InStr(olJournal.Body, "Tel.-Nr.: " & C_DP.P_Def_StringUnknown, CompareMethod.Text))
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
        P_CallClient.Wählbox(Nothing, C_DP.P_Def_StringEmpty, C_DP.P_Def_StringEmpty, True)
    End Sub

    Friend Sub ÖffneEinstellungen()
        ThisAddIn.P_Config.ShowDialog()
    End Sub

    Friend Sub ÖffneJournalImport()
        Dim formjournalimort As New formJournalimport(C_AnrMon, C_HF, C_DP, True)
    End Sub

    Friend Sub ÖffneAnrMonAnzeigen()
        C_PopUp.AnrMonEinblenden(C_AnrMon.LetzterAnrufer)
    End Sub

    Friend Sub AnrMonNeustarten()
        C_AnrMon.AnrMonReStart()
    End Sub

    Friend Sub WählenExplorer()
        If C_OLI.OutlookApplication IsNot Nothing Then
            Dim ActiveExplorer As Outlook.Explorer = C_OLI.OutlookApplication.ActiveExplorer
            Dim oSel As Outlook.Selection = ActiveExplorer.Selection
            P_CallClient.WählboxStart(oSel)
            C_HF.NAR(oSel) : C_HF.NAR(ActiveExplorer)
            oSel = Nothing : ActiveExplorer = Nothing
        End If
    End Sub

#End Region

#Region "Inspector Button Click"
    ''' <summary>
    ''' Öffnet den Wähldialog.
    ''' </summary>
    ''' <remarks>Funktion wird für alle Office Versionen verwendet!</remarks>
    Friend Sub WählenInspector()
        P_CallClient.WählenAusInspector()
    End Sub

    ''' <summary>
    ''' Zeigt einen Kontakt aus einem Journal.
    ''' </summary>
    ''' <remarks>Funktion wird für alle Office Versionen verwendet!</remarks>
    Friend Sub KontaktErstellen()
        C_KF.ZeigeKontaktAusJournal()
    End Sub

#If OVer = 11 Then

    Friend Sub OnActionRWS(ByVal oInsp As Outlook.Inspector, ByVal RWS As RückwärtsSuchmaschine)

        Select Case RWS
            Case RückwärtsSuchmaschine.RWS11880
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWS11880, oInsp)
            Case RückwärtsSuchmaschine.RWSDasOertliche
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSDasOertliche, oInsp)
            Case RückwärtsSuchmaschine.RWSDasTelefonbuch
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSDasTelefonbuch, oInsp)
            Case RückwärtsSuchmaschine.RWStelSearch
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWStelSearch, oInsp)
            Case RückwärtsSuchmaschine.RWSAlle
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSAlle, oInsp)
        End Select
    End Sub

#End If
#End Region

#Region "RingCallList"

    Friend Overloads Sub UpdateList(ByVal ListName As String, _
                                    ByVal Anrufer As String, _
                                    ByVal TelNr As String, _
                                    ByVal Zeit As String, _
                                    ByVal StoreID As String, _
                                    ByVal KontaktID As String, _
                                    ByVal vCard As String)

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim index As Integer              ' Zählvariable

        index = CInt(C_DP.Read(ListName, "Index", "0"))

        xPathTeile.Add(ListName)
        xPathTeile.Add("Eintrag[@ID=""" & index - 1 & """]")
        xPathTeile.Add("TelNr")
        'With Telefonat

        If Not C_HF.TelNrVergleich(C_DP.Read(xPathTeile, "0"), TelNr) Then

            NodeNames.Add("Index")
            NodeValues.Add(CStr((index + 1) Mod 10))

            If Not Anrufer = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("Anrufer")
                NodeValues.Add(Anrufer)
            End If

            If Not TelNr = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("TelNr")
                NodeValues.Add(TelNr)
            End If

            If Not Zeit = Nothing Then
                NodeNames.Add("Zeit")
                NodeValues.Add(Zeit)
            End If

            If Not StoreID = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("StoreID")
                NodeValues.Add(StoreID)
            End If

            If Not KontaktID = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("KontaktID")
                NodeValues.Add(KontaktID)
            End If

            If Not vCard = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("vCard")
                NodeValues.Add(vCard)
            End If

            AttributeNames.Add("ID")
            AttributeValues.Add(CStr(index))

            With C_DP
                xPathTeile.Clear() 'RemoveRange(0, xPathTeile.Count)
                xPathTeile.Add(ListName)
                xPathTeile.Add("Index")
                .Write(xPathTeile, CStr((index + 1) Mod 10))
                xPathTeile.Remove("Index")
                .AppendNode(xPathTeile, .CreateXMLNode("Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
            End With
        Else
            ' Zeit anpassen
            If Not Zeit = Nothing Then
                xPathTeile.Item(xPathTeile.Count - 1) = "Zeit"
                C_DP.Write(xPathTeile, CStr(Zeit))
            End If
        End If
        'End With
        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
#If OVer > 12 Then
        RefreshRibbon()
#End If

    End Sub

    Friend Overloads Sub UpdateList(ByVal ListName As String, ByVal Telefonat As C_Telefonat)
        With Telefonat
            UpdateList(ListName, .Anrufer, .TelNr, CStr(.Zeit), .StoreID, .KontaktID, .vCard)
        End With
    End Sub
#End Region

End Class