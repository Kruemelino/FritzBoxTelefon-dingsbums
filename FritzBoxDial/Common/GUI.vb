Imports System.Collections.Generic

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
                File = GetResourceText("FritzBoxDial.RibbonInspectorMailRead.xml")
            Case "Microsoft.Outlook.Journal"
                File = GetResourceText("FritzBoxDial.RibbonInspectorJournal.xml")
            Case "Microsoft.Outlook.Contact"
                File = GetResourceText("FritzBoxDial.RibbonInspectorKontakt.xml")
            Case Else
                File = DataProvider.P_Def_LeerString
        End Select
#If OVer = 12 Then
        If Not File = DataProvider.P_Def_LeerString Then
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
    Private C_XML As XML
    Private C_HF As Helfer
    Private C_DP As DataProvider
    Private C_KF As KontaktFunktionen
#End Region

#Region "Eigene Formulare"
    Private F_RWS As formRWSuche
#End Region

#Region "Properies"
    Private C_WClient As Wählclient
    Friend Property P_CallClient() As Wählclient
        Get
            Return C_WClient
        End Get
        Set(ByVal value As Wählclient)
            C_WClient = value
        End Set
    End Property

    Private C_AnrMon As AnrufMonitor
    Friend Property P_AnrufMonitor() As AnrufMonitor
        Get
            Return C_AnrMon
        End Get
        Set(ByVal value As AnrufMonitor)
            C_AnrMon = value
        End Set
    End Property

    Private C_OLI As OutlookInterface
    Public Property P_OlInterface() As OutlookInterface
        Get
            Return C_OLI
        End Get
        Set(ByVal value As OutlookInterface)
            C_OLI = value
        End Set
    End Property

    Private C_FBox As FritzBox
    Public Property P_FritzBox() As FritzBox
        Get
            Return C_FBox
        End Get
        Set(ByVal value As FritzBox)
            C_FBox = value
        End Set
    End Property

    Private C_PopUp As Popup
    Public Property P_PopUp() As Popup
        Get
            Return C_PopUp
        End Get
        Set(ByVal value As Popup)
            C_PopUp = value
        End Set
    End Property

    Private F_Cfg As formCfg
    Public Property P_Config() As formCfg
        Get
            Return F_Cfg
        End Get
        Set(ByVal value As formCfg)
            F_Cfg = value
        End Set
    End Property

    Private F_AnrList As formImportAnrList
    Public Property P_AnrList() As formImportAnrList
        Get
            Return F_AnrList
        End Get
        Set(ByVal value As formImportAnrList)
            F_AnrList = value
        End Set
    End Property
#End Region

    Friend Sub New(ByVal HelferKlasse As Helfer, _
           ByVal DataProviderKlasse As DataProvider, _
           ByVal Inverssuche As formRWSuche, _
           ByVal KontaktKlasse As KontaktFunktionen, _
           ByVal PopUpKlasse As Popup, _
           ByVal XMLKlasse As XML)

        C_HF = HelferKlasse
        C_DP = DataProviderKlasse
        F_RWS = Inverssuche
        C_KF = KontaktKlasse
        C_PopUp = PopUpKlasse
        C_XML = XMLKlasse
    End Sub

#Region "Ribbon Behandlung für Outlook 2007 bis 2013"
#If Not OVer = 11 Then
#Region "Ribbon Inspector Office 2007 & Office 2010 & Office 2013" ' Ribbon Inspektorfenster

    ''' <summary>
    ''' Gibt zurück, ob das JournalItem, von diesem Addin erstellt wurde. Dazu wird die Kategorie geprüft.
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>True, wenn JournalItem, von diesem Addin erstellt wurde. Ansonsten False</returns>
    Private Function CheckJournalInspector(ByVal control As Microsoft.Office.Core.IRibbonControl) As Outlook.JournalItem
        CheckJournalInspector = Nothing

        Dim oInsp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim olLink As Outlook.Link = Nothing

        With C_HF
            If TypeOf oInsp.CurrentItem Is Outlook.JournalItem Then
                olJournal = CType(oInsp.CurrentItem, Outlook.JournalItem)

                ' Bei Journal nur wenn Kategorien korrekt
                ' Wenn Journal keine Kategorie enthält, dann ist es kein vom Addin erzeugtes JournalItem
                If olJournal.Categories IsNot Nothing AndAlso olJournal.Categories.Contains(String.Join("; ", DataProvider.P_AnrMon_Journal_Def_Categories)) Then
                    CheckJournalInspector = olJournal
                End If
            End If
            .NAR(olJournal) : olJournal = Nothing
            .NAR(olLink) : olLink = Nothing
            .NAR(oInsp) : oInsp = Nothing
        End With
    End Function

    ''' <summary>
    ''' Gibt zurück, ob das Journal eine gültige Telefonnummer enthält
    ''' </summary>
    ''' <param name="control"></param>
    Public Function EnableBtnJournal(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        EnableBtnJournal = False

        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)

        If olJournal IsNot Nothing Then
            EnableBtnJournal = Not olJournal.Body.Contains(DataProvider.P_AnrMon_AnrMonDISCONNECT_JournalTelNr & DataProvider.P_Def_StringUnknown)
        End If

    End Function

    ''' <summary>
    ''' Gibt das Label des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>"Kontakt Anzeigen", wenn Link im JournalItem zu einem ContactItem führt. Ansonsten "Kontakt Erstellen"</returns>
    ''' <remarks>Funktioniert nur unter Office 2010, da Microsoft die Links aus Journalitems in nachfolgenden Office Versionen entfernt hat.</remarks>
    Private Function SetLabelJournal(ByVal control As Office.IRibbonControl) As String
        SetLabelJournal = DataProvider.P_CMB_Kontakt_Erstellen

#If Not OVer = 15 Then
        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)
        If olJournal IsNot Nothing Then
            For Each olLink As Outlook.Link In olJournal.Links
                ' Catch tritt ein, wenn der Kontakt nicht mehr verfügbar ist.
                Try
                    If TypeOf olLink.Item Is Outlook.ContactItem Then
                        SetLabelJournal = DataProvider.P_CMB_Kontakt_Anzeigen
                        Exit For
                    End If
                Catch : End Try
            Next
        End If
#End If
    End Function

    ''' <summary>
    ''' Gibt das ScreenTip des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>Den entsprechenden ScreenTip, wenn Link im JournalItem zu einem ContactItem führt. Ansonsten den anderen. Falls Link ins Leere führt, dann wird Fehlermeldung ausgegeben.</returns>
    ''' <remarks>Funktioniert nur unter Office 2010, da Microsoft die Links aus Journalitems in nachfolgenden Office Versionen entfernt hat.</remarks>
    Private Function SetScreenTipJournal(ByVal control As Office.IRibbonControl) As String
        SetScreenTipJournal = DataProvider.P_CMB_Kontakt_Erstellen_ToolTipp

#If Not OVer = 15 Then
        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)
        If olJournal IsNot Nothing Then
            For Each olLink As Outlook.Link In olJournal.Links
                ' Catch tritt ein, wenn der Kontakt nicht mehr verfügbar ist.
                Try
                    If TypeOf olLink.Item Is Outlook.ContactItem Then
                        SetScreenTipJournal = DataProvider.P_CMB_Kontakt_Anzeigen_ToolTipp
                        Exit For
                    End If
                Catch
                    SetScreenTipJournal = DataProvider.P_CMB_Kontakt_Anzeigen_Error_ToolTipp
                End Try
            Next
        End If
#End If

    End Function

    ''' <summary>
    ''' Die Ribbons der Inspectoren sollen nur eingeblendet werden, wenn ein Explorer vorhanden ist.
    ''' </summary>
    ''' <param name="control"></param>
    Public Function ShowInspectorRibbon(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        ShowInspectorRibbon = False

        ' Einblendenm wenn Explorer vorhanden ist
        ShowInspectorRibbon = (New Outlook.Application).ActiveExplorer IsNot Nothing

        ' Extra Prüfung bei JournalItem
        If TypeOf CType(control.Context, Outlook.Inspector).CurrentItem Is Outlook.JournalItem Then
            ShowInspectorRibbon = CheckJournalInspector(control) IsNot Nothing
        End If
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
        Dim Anrufer As String
        Dim TelNr As String
        Dim Zeit As String
        Dim Verpasst As Boolean = False

        Dim LANodeNames As New ArrayList
        Dim LANodeValues As New ArrayList
        Dim xPathTeile As New ArrayList

        Dim RibbonListStrBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf & _
                                                                      "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)

        Select Case Mid(control.Id, 1, Len(control.Id) - 2)
            Case "dynMWwdListe"
                XMLListBaseNode = DataProvider.P_Def_NameListCALL '"CallList"
            Case "dynMAnrListe"
                XMLListBaseNode = DataProvider.P_Def_NameListRING '"RingList"
            Case Else '"dynMVIPListe"
                XMLListBaseNode = DataProvider.P_Def_NameListVIP '"VIPList"
        End Select

        index = CInt(C_XML.Read(C_DP.XMLDoc, XMLListBaseNode, "Index", "0"))

        LANodeNames.Add("Anrufer")
        LANodeNames.Add("TelNr")
        LANodeNames.Add("Zeit")
        LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        ' Signalisierung verpasster Anrufe
        If XMLListBaseNode = DataProvider.P_Def_NameListRING Then
            LANodeNames.Add("Verpasst")
            LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        End If

        With xPathTeile
            .Add(XMLListBaseNode)
            .Add("Eintrag")
        End With

        With RibbonListStrBuilder
            .Append("<button id=""dynListDel_" & XMLListBaseNode & """ getLabel=""GetItemLabel"" onAction=""BtnOnAction"" getImage=""GetItemImageMso"" />" & vbCrLf)
            .Append("<menuSeparator id=""separator"" />" & vbCrLf)
        End With

        If Not XMLListBaseNode = DataProvider.P_Def_NameListVIP Then

            For ID = index + 9 To index Step -1

                C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))
                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))

                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                    Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                    Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))
                    If XMLListBaseNode = DataProvider.P_Def_NameListRING Then Verpasst = CBool(LANodeValues.Item(LANodeNames.IndexOf("Verpasst")))

                    GetButtonXMLString(RibbonListStrBuilder, _
                            CStr(ID Mod 10), _
                            CStr(IIf(Anrufer = DataProvider.P_Def_ErrorMinusOne_String, TelNr, Anrufer)), _
                            XMLListBaseNode, _
                            DataProvider.P_CMB_ToolTipp(Zeit, TelNr), _
                            CStr(IIf(Verpasst, "HighImportance", DataProvider.P_Def_LeerString)))

                    LANodeValues.Item(0) = DataProvider.P_Def_ErrorMinusOne_String
                    LANodeValues.Item(1) = DataProvider.P_Def_ErrorMinusOne_String
                    LANodeValues.Item(2) = DataProvider.P_Def_ErrorMinusOne_String
                End If
            Next
        Else
            For ID = 0 To index
                C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                If Not Anrufer = DataProvider.P_Def_ErrorMinusOne_String Then

                    GetButtonXMLString(RibbonListStrBuilder, _
                            CStr(ID), _
                            Anrufer, _
                            XMLListBaseNode, _
                            DataProvider.P_Def_LeerString, _
                            DataProvider.P_Def_LeerString)

                    LANodeValues.Item(0) = DataProvider.P_Def_ErrorMinusOne_String
                End If
            Next
        End If

        RibbonListStrBuilder.Append("</menu>")

        DynMenüfüllen = RibbonListStrBuilder.ToString
        RibbonListStrBuilder.Clear()
        RibbonListStrBuilder = Nothing
        LANodeNames = Nothing
        LANodeValues = Nothing
        xPathTeile = Nothing
    End Function

    Private Sub GetButtonXMLString(ByRef StrBuilder As StringBuilder, ByVal ID As String, ByVal Label As String, ByVal Tag As String, ByVal SuperTip As String, ByVal ImageMSO As String)
        Dim Werte(4) As String

        Werte(0) = ID
        Werte(1) = Label
        Werte(2) = Tag
        Werte(3) = SuperTip
        Werte(4) = ImageMSO

        ' Nicht zugelassene Zeichen der XML-Notifikation ersetzen.
        ' Zeichen	Notation in XML
        ' <	        &lt;    &#60;
        ' >	        &gt;    &#62;
        ' &	        &amp;   &#38; Zweimal anfügen, da es ansonsten ignoriert wird
        ' "	        &quot;  &#34;
        ' '	        &apos;  &#38;

        For i = LBound(Werte) To UBound(Werte)
            If Not Werte(i) = DataProvider.P_Def_LeerString Then
                Werte(i) = Werte(i).Replace("&", "&amp;&amp;").Replace("&amp;&amp;#", "&#").Replace("<", "&lt;").Replace(">", "&gt;").Replace(Chr(34), "&quot;").Replace("'", "&apos;")
            End If
        Next

        With StrBuilder
            .Append("<button id=""dynMListe_" & Werte(0) & """ label=""" & Werte(1) & """ onAction=""BtnOnAction"" tag=""" & Werte(2) & ";" & Werte(0) & """ ")

            If Not Werte(3) = DataProvider.P_Def_LeerString Then .Append("supertip=""" & Werte(3) & """ ")
            If Not Werte(4) = DataProvider.P_Def_LeerString Then .Append("imageMso=""" & Werte(4) & """ ")

            .Append("/>" & vbCrLf)
        End With
    End Sub

    Public Function DynMenüEnabled(ByVal control As Office.IRibbonControl) As Boolean
        Dim XMLListBaseNode As String
        Dim xPathTeile As New ArrayList

        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case "dynMWwdListe"
                XMLListBaseNode = DataProvider.P_Def_NameListCALL '"CallList"
            Case "dynMAnrListe"
                XMLListBaseNode = DataProvider.P_Def_NameListRING '"RingList"
            Case Else '"dynMVIPListe"
                XMLListBaseNode = DataProvider.P_Def_NameListVIP '"VIPList"
        End Select

        Return CBool(IIf(Not C_XML.Read(C_DP.XMLDoc, XMLListBaseNode, "Index", DataProvider.P_Def_ErrorMinusOne_String) = DataProvider.P_Def_ErrorMinusOne_String, True, False))
    End Function

    Public Function GetPressed(ByVal control As Office.IRibbonControl) As Boolean
        GetPressed = False
        If C_AnrMon IsNot Nothing Then
            GetPressed = C_AnrMon.AnrMonAktiv
        End If
    End Function

    Public Function UseAnrMon(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return C_DP.P_CBUseAnrMon
    End Function

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

    Public Function GetVisibleAnrMonFKT(ByVal control As Office.IRibbonControl) As Boolean
        Return C_DP.P_CBUseAnrMon
    End Function

    Public Function GetEnabledJI(ByVal control As Office.IRibbonControl) As Boolean
        Return C_DP.P_CBJournal
    End Function

    ''' <summary>
    ''' Die Ribbons der Explorer sollen nur eingeblendet werden, wenn ... (momentan immer :) )
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns>True</returns>
    Public Function ShowExplorerRibbon(ByVal control As Microsoft.Office.Core.IRibbonControl) As Boolean
        Return True
    End Function

#End If
#End Region 'Ribbon Explorer

#Region "Ribbon: Label, ScreenTipp, ImageMso, OnAction"
    ''' <summary>
    ''' Ermittelt das Label des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemLabel(ByVal control As Office.IRibbonControl) As String
        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case "Tab"
                Return DataProvider.P_Def_Addin_LangName
            Case "btnDialExpl", "btnDialInsp"
                Return DataProvider.P_CMB_Dial
            Case "btnDirektwahl"
                Return DataProvider.P_CMB_Direktwahl
            Case "dynMWwdListe"
                Return DataProvider.P_CMB_WWDH
            Case "dynMAnrListe"
                Return DataProvider.P_CMB_CallBack
            Case "dynMVIPListe"
                Return DataProvider.P_CMB_VIP
            Case "btnAnrMonIO"
                Return DataProvider.P_CMB_AnrMon
            Case "dynListDel"
                Return DataProvider.P_CMB_ClearList
            Case "btnAnrMonRestart"
                Return DataProvider.P_CMB_AnrMonNeuStart
            Case "btnAnrMonShow"
                Return DataProvider.P_CMB_AnrMonAnzeigen
            Case "btnAnrMonJI"
                Return DataProvider.P_CMB_Journal
            Case "Einstellungen"
                Return DataProvider.P_CMB_Setup
            Case "cbtnDial" ' ContextMenu Dial
                Return DataProvider.P_CMB_ContextMenueItemCall
            Case "ctbtnVIP" ' ContextMenu Dial
                Return DataProvider.P_CMB_ContextMenueItemVIP
            Case "cbtnUpload" ' ContextMenu Upload
                Return DataProvider.P_CMB_ContextMenueItemUpload
            Case "mnuRWS"
                Return DataProvider.P_CMB_Insp_RWS
            Case "btnRWS01"
                Return DataProvider.P_RWS11880_Name
            Case "btnRWS02"
                Return DataProvider.P_RWSDasOertliche_Name
            Case "btnRWS03"
                Return DataProvider.P_RWSDasTelefonbuch_Name
            Case "btnRWS04"
                Return DataProvider.P_RWSTelSearch_Name
            Case "btnRWS05"
                Return DataProvider.P_RWSAlle_Name
            Case "btnAddContact"
                Return SetLabelJournal(control)
            Case "btnNote"
                Return DataProvider.P_CMB_Insp_Note
            Case "tbtnVIP"
                Return DataProvider.P_CMB_Insp_VIP
            Case "btnUpload"
                Return DataProvider.P_CMB_Insp_Upload
            Case Else
                Return DataProvider.P_Def_ErrorMinusOne_String
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemScreenTipp(ByVal control As Office.IRibbonControl) As String
        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case "btnDialExpl", "btnDialInsp"
                Return DataProvider.P_CMB_Dial_ToolTipp
            Case "btnDirektwahl"
                Return DataProvider.P_CMB_Direktwahl_ToolTipp
            Case "dynMWwdListe"
                Return DataProvider.P_CMB_WWDH_ToolTipp
            Case "dynMAnrListe"
                Return DataProvider.P_CMB_CallBack_ToolTipp
            Case "dynMVIPListe"
                Return DataProvider.P_CMB_VIP_ToolTipp
            Case "btnAnrMonIO"
                Return DataProvider.P_CMB_AnrMon_ToolTipp
            Case "btnAnrMonRestart"
                Return DataProvider.P_CMB_AnrMonNeuStart_ToolTipp
            Case "btnAnrMonShow"
                Return DataProvider.P_CMB_AnrMonAnzeigen_ToolTipp()
            Case "btnAnrMonJI"
                Return DataProvider.P_CMB_Journal_ToolTipp
            Case "Einstellungen"
                Return DataProvider.P_CMB_Setup_ToolTipp
            Case "mnuRWS"
                Return DataProvider.P_CMB_Insp_RWS_ToolTipp
            Case "btnRWS01"
                Return DataProvider.P_RWS_ToolTipp(DataProvider.P_RWS11880_Link)
            Case "btnRWS02"
                Return DataProvider.P_RWS_ToolTipp(DataProvider.P_RWSDasOertliche_Link)
            Case "btnRWS03"
                Return DataProvider.P_RWS_ToolTipp(DataProvider.P_RWSDasTelefonbuch_Link)
            Case "btnRWS04"
                Return DataProvider.P_RWS_ToolTipp(DataProvider.P_RWSTelSearch_Link)
            Case "btnRWS05"
                Return DataProvider.P_RWS_ToolTipp()
            Case "btnAddContact"
                Return SetScreenTipJournal(control)
            Case "btnNote"
                Return DataProvider.P_CMB_Insp_Note_ToolTipp
            Case "tbtnVIP"
                Return CStr(IIf(IsVIP(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)), DataProvider.P_CMB_VIP_Entfernen_ToolTipp, DataProvider.P_CMB_VIP_Hinzufügen_ToolTipp))
            Case "btnUpload"
                Return DataProvider.P_CMB_Insp_UploadKontakt_ToolTipp()
            Case Else
                Return DataProvider.P_Def_ErrorMinusOne_String
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das Icon (ImageMSO) des Ribbon-Objektes ausgehend von der Ribbon-id
    ''' </summary>
    ''' <param name="control">Die id des Ribbon Controls</param>
    ''' <returns>Bezeichnung des ImageMso</returns>
    ''' <remarks>http://soltechs.net/customui/</remarks>
    Public Function GetItemImageMso(ByVal control As Office.IRibbonControl) As String

        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case "btnDialExpl", "btnDialInsp"
                Return "AutoDial"
            Case "btnDirektwahl"
                Return "SlidesPerPage9Slides"
            Case "dynMWwdListe"
                Return "RecurrenceEdit"
            Case "dynMAnrListe"
                Return "DirectRepliesTo"
            Case "dynMVIPListe", "tbtnVIP"
                Return "Pushpin"
            Case "dynListDel"
                Return "ToolDelete"
            Case "btnAnrMonIO"
                GetItemImageMso = "PersonaStatusBusy"
                If C_AnrMon IsNot Nothing Then
                    If C_AnrMon.AnrMonAktiv Then
                        GetItemImageMso = "PersonaStatusOnline"
                    Else
                        If Not C_AnrMon.AnrMonError Then GetItemImageMso = "PersonaStatusOffline"
                    End If
                End If
            Case "btnAnrMonRestart"
                Return "RecurrenceEdit"
            Case "btnAnrMonShow"
                Return "ClipArtInsert"
            Case "btnAnrMonJI"
                Return "NewJournalEntry"
            Case "btnUpload"
                Return "DistributionListAddNewMember"
            Case "mnuRWS" ' Inspector
                Return "CheckNames"
            Case "btnAddContact" ' Inspector
                Return "RecordsSaveAsOutlookContact"
            Case "btnNote" ' Inspector
                Return "ShowNotesPage"
            Case Else
                Return DataProvider.P_Def_ErrorMinusOne_String
        End Select

    End Function

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem Button hinterlegt ist.
    ''' </summary>
    ''' <param name="control">Die id des Ribbon Controls</param>
    Public Sub BtnOnAction(ByVal control As Office.IRibbonControl)
        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case "btnDialExpl", "cbtnDial"
                OnAction(TaskToDo.DialExplorer)
            Case "btnDialInsp"
                OnAction(TaskToDo.DialInspector)
            Case "btnDirektwahl"
                OnAction(TaskToDo.DialDirect)
            Case "dynMListe" ',"dynMWwdListe", "dynMAnrListe", "dynMVIPListe"
                OnActionListen(control.Tag)
            Case "dynListDel"
                ClearInListe(control.Id)
            Case "btnAnrMonIO"
                C_AnrMon.AnrMonStartStopp()
            Case "btnAnrMonRestart"
                OnAction(TaskToDo.RestartAnrMon)
            Case "btnAnrMonShow"
                OnAction(TaskToDo.ShowAnrMon)
            Case "btnAnrMonJI"
                OnAction(TaskToDo.OpenJournalimport)
            Case "Einstellungen"
                OnAction(TaskToDo.OpenConfig)
            Case "cbtnUpload"
                Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
                C_FBox.UploadKontaktToFritzBox(oKontakt, IsVIP(oKontakt))
            Case "btnUpload"
                Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)
                C_FBox.UploadKontaktToFritzBox(oKontakt, IsVIP(oKontakt))
            Case "btnRWS01" ' RWS11880
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWS11880, CType(control.Context, Outlook.Inspector))
            Case "btnRWS02" ' RWSDasOertliche
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSDasOertliche, CType(control.Context, Outlook.Inspector))
            Case "btnRWS03" ' RWSTelSearch
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSDasTelefonbuch, CType(control.Context, Outlook.Inspector))
            Case "btnRWS04" ' RWSTelSearch
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWStelSearch, CType(control.Context, Outlook.Inspector))
            Case "btnRWS05" ' RWSAlle
                F_RWS.Rückwärtssuche(RückwärtsSuchmaschine.RWSAlle, CType(control.Context, Outlook.Inspector))
            Case "btnAddContact"
                OnAction(TaskToDo.CreateContact)
            Case "btnNote"
                C_KF.AddNote(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem))
        End Select
    End Sub

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem ToogleButton hinterlegt ist.
    ''' </summary>
    ''' <param name="control">ToogleButton</param>
    ''' <param name="pressed">Zustand des ToogleButtons</param>
    ''' <remarks>Eine reine Weiterleitung auf die Standard-OnAction Funktion</remarks>
    Public Sub BtnOnToggleButtonAction(ByVal control As Office.IRibbonControl, ByVal pressed As Boolean)
        BtnOnAction(control)
    End Sub
#End Region

#Region "VIP-Ribbon"
    Public Sub tBtnOnAction(ByVal control As Office.IRibbonControl, ByVal pressed As Boolean)
        Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)

        If IsVIP(oKontakt) Then
            RemoveVIP(oKontakt.EntryID, CType(oKontakt.Parent, Outlook.MAPIFolder).StoreID)
        Else
            AddVIP(oKontakt)
        End If
        C_HF.NAR(oKontakt)
        oKontakt = Nothing
        ' Fehler unter Office 2007
#If OVer >= 14 Then
        RibbonObjekt.Invalidate()
#End If
    End Sub

    Public Function CtBtnPressedVIP(ByVal control As Office.IRibbonControl) As Boolean
        CtBtnPressedVIP = False
        Dim oKontact As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)

        CtBtnPressedVIP = IsVIP(oKontact)

        C_HF.NAR(oKontact)
        oKontact = Nothing
    End Function

    Public Function tBtnPressedVIP(ByVal control As Office.IRibbonControl) As Boolean
        tBtnPressedVIP = False

        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)

        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim olContact As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)

            tBtnPressedVIP = IsVIP(olContact)

            C_HF.NAR(olContact)
            olContact = Nothing
        End If
    End Function

#End Region

#End If
#End Region

#Region "VIP-Generell"
    Friend Function IsVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        IsVIP = False

        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID
        Dim xPathTeile As New ArrayList

        xPathTeile.Add(DataProvider.P_Def_NameListVIP)
        xPathTeile.Add("Eintrag")
        xPathTeile.Add("[(KontaktID = """ & KontaktID & """ and StoreID = """ & StoreID & """)]")
        IsVIP = Not C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String) = DataProvider.P_Def_ErrorMinusOne_String
        xPathTeile = Nothing
    End Function

    Friend Overloads Function AddVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        Dim Anrufer As String = Replace(aktKontakt.FullName & " (" & aktKontakt.CompanyName & ")", " ()", "")
        Dim Index As Integer = CInt(C_XML.Read(C_DP.XMLDoc, DataProvider.P_Def_NameListVIP, "Index", "0"))
        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        xPathTeile.Add(DataProvider.P_Def_NameListVIP)
        xPathTeile.Add("ID[@ID=""" & Index & """]")

        If Not Anrufer = DataProvider.P_Def_LeerString Then
            NodeNames.Add("Anrufer")
            NodeValues.Add(Anrufer)
        End If

        If Not StoreID = DataProvider.P_Def_LeerString Then
            NodeNames.Add("StoreID")
            NodeValues.Add(StoreID)
        End If

        If Not KontaktID = DataProvider.P_Def_LeerString Then
            NodeNames.Add("KontaktID")
            NodeValues.Add(KontaktID)
        End If

        AttributeNames.Add("ID")
        AttributeValues.Add(CStr(Index))

        With C_DP
            xPathTeile.Clear()
            xPathTeile.Add(DataProvider.P_Def_NameListVIP)
            xPathTeile.Add("Index")
            C_XML.Write(.XMLDoc, xPathTeile, CStr(Index + 1))
            xPathTeile.Remove("Index")
            C_XML.AppendNode(.XMLDoc, xPathTeile, C_XML.CreateXMLNode(.XMLDoc, "Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
            .SpeichereXMLDatei()
        End With
        NodeNames = Nothing
        NodeValues = Nothing
        xPathTeile = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
#If OVer < 14 Then
        FillPopupItems(DataProvider.P_Def_NameListVIP)
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
            .Add(DataProvider.P_Def_NameListVIP)
            .Add("Index")
            Anzahl = CInt(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0"))
            ' Index Speichern
            .Item(.Count - 1) = "Eintrag"
            .Add("[(KontaktID = """ & KontaktID & """ and StoreID = """ & StoreID & """)]")
            .Add("Index")
            Index = CInt(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0"))
            ' Knoten löschen
            .Remove("Index")
            C_XML.Delete(C_DP.XMLDoc, xPathTeile)
            ' schleife durch jeden anderen Knoten und <Index> und Attribut ändern
            For i = Index + 1 To Anzahl - 1
                .Item(.Count - 1) = "[@ID=""" & i & """]"
                C_XML.WriteAttribute(C_DP.XMLDoc, xPathTeile, "ID", CStr(i - 1))
            Next
            'neue Anzahl (index) schreiben oder löschen
            .Remove(.Item(.Count - 1))
            .Remove("Eintrag")
            If C_XML.SubNoteCount(C_DP.XMLDoc, xPathTeile) = 1 Then
                .Add("Index")
                C_XML.Delete(C_DP.XMLDoc, xPathTeile)
            Else
                C_XML.Write(C_DP.XMLDoc, DataProvider.P_Def_NameListVIP, "Index", CStr(Anzahl - 1))
            End If

        End With

#If OVer < 14 Then
        FillPopupItems(DataProvider.P_Def_NameListVIP)
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
                .Name = DataProvider.P_Def_Addin_KurzName
                .NameLocal = DataProvider.P_Def_Addin_KurzName
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
                                    ByVal btnCaption As String, _
                                    ByVal PosIndex As Integer, _
                                    ByVal btnFaceId As Integer, _
                                    ByVal btnStyle As Office.MsoButtonStyle, _
                                    ByVal btnTag As String, _
                                    ByVal btnToolTip As String) As Office.CommandBarButton

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
                    .FaceId = btnFaceId
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
        If btnPopup.Controls.Count > Index Then
            Return Nothing
        Else
            Dim btn As Office.CommandBarButton = CType(btnPopup.Controls.Add(Office.MsoControlType.msoControlButton, , , , True), Office.CommandBarButton)
            btn.Visible = False 'erst mal verstecken, da wir nicht wissen ob da ein Wert drin ist.
            Return btn
        End If
    End Function

    Friend Sub FillPopupItems(ByRef XMLListBaseNode As String)
        ' XMLListBaseNode erlaubt: CallList, RingList, VIPListe

        Dim cPopUp As Office.CommandBarPopup = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , XMLListBaseNode, , False), Office.CommandBarPopup)
        Dim index As Integer
        Dim Anrufer As String
        Dim TelNr As String
        Dim Zeit As String
        Dim Verpasst As Boolean

        Dim LANodeNames As New ArrayList
        Dim LANodeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim i As Integer

        index = CInt(C_XML.Read(C_DP.XMLDoc, XMLListBaseNode, "Index", "0"))

        LANodeNames.Add("Anrufer")
        LANodeNames.Add("TelNr")
        LANodeNames.Add("Zeit")

        LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        With xPathTeile
            .Add(XMLListBaseNode)
            .Add("Eintrag")
        End With

        ' Signalisierung verpasster Anrufe
        If XMLListBaseNode = DataProvider.P_Def_NameListRING Then
            LANodeNames.Add("Verpasst")
            LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        End If

        i = 1
        With CType(cPopUp.Controls.Item(1), Office.CommandBarButton)
            .Caption = DataProvider.P_CMB_ClearList
            .Style = MsoButtonStyle.msoButtonIconAndCaption
            .FaceId = 1786
            .Visible = True
            .Tag = DataProvider.P_CMB_eDynListDel_Tag & "_" & XMLListBaseNode
        End With

        If Not XMLListBaseNode = DataProvider.P_Def_NameListVIP Then
            For ID = index + 9 To index Step -1

                C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod 10))

                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))
                Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))
                If XMLListBaseNode = DataProvider.P_Def_NameListRING Then Verpasst = CBool(LANodeValues.Item(LANodeNames.IndexOf("Verpasst")))

                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then

                    With CType(cPopUp.Controls.Item(i + 1), Office.CommandBarButton)
                        If Anrufer = DataProvider.P_Def_ErrorMinusOne_String Then .Caption = TelNr Else .Caption = Anrufer
                        .Style = MsoButtonStyle.msoButtonIconAndCaption
                        .TooltipText = DataProvider.P_CMB_ToolTipp(Zeit, TelNr)
                        .Parameter = CStr(ID Mod 10)
                        .Visible = True
                        .Tag = XMLListBaseNode & ";" & CStr(ID Mod 10)
                        .BeginGroup = CBool(IIf(i = 1, True, False))
                        .FaceId = CInt(IIf(Verpasst, 964, 0))
                        i += 1
                    End With

                    With LANodeValues
                        .Item(0) = (DataProvider.P_Def_ErrorMinusOne_String)
                        .Item(1) = (DataProvider.P_Def_ErrorMinusOne_String)
                        .Item(2) = (DataProvider.P_Def_ErrorMinusOne_String)
                    End With
                End If
            Next
        Else
            For ID = 0 To 9

                C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID))
                Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))

                If Not Anrufer = DataProvider.P_Def_ErrorMinusOne_String And Not Anrufer = DataProvider.P_Def_LeerString Then
                    With CType(cPopUp.Controls.Item(i + 1), Office.CommandBarButton)
                        .Caption = Anrufer
                        .Parameter = CStr(ID Mod 10)
                        .Visible = True
                        .Tag = XMLListBaseNode & ";" & CStr(ID)
                        .BeginGroup = CBool(IIf(i = 1, True, False))
                        i += 1
                    End With
                    With LANodeValues
                        .Item(0) = (DataProvider.P_Def_ErrorMinusOne_String)
                        .Item(1) = (DataProvider.P_Def_ErrorMinusOne_String)
                        .Item(2) = (DataProvider.P_Def_ErrorMinusOne_String)
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
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , DataProvider.P_CMB_eBtnDirektwahl_Tag).Visible = C_DP.P_CBSymbDirekt
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , DataProvider.P_CMB_eBtnAnrMon_Tag).Visible = C_DP.P_CBSymbAnrMon
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , DataProvider.P_CMB_eBtnAnzeigen_Tag).Visible = C_DP.P_CBSymbAnrMon
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , DataProvider.P_Def_NameListRING).Visible = C_DP.P_CBSymbAnrListe
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , DataProvider.P_Def_NameListCALL).Visible = C_DP.P_CBSymbWwdh
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , DataProvider.P_CMB_eBtnJournalimport_Tag).Visible = C_DP.P_CBSymbJournalimport
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , DataProvider.P_CMB_eBtnAnrMonNeuStart_Tag).Visible = C_DP.P_CBSymbAnrMonNeuStart
        Catch : End Try
        Try
            FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlPopup, , DataProvider.P_Def_NameListVIP).Visible = C_DP.P_CBSymbVIP
        Catch : End Try
    End Sub

    Friend Sub SetAnrMonButton()
        bool_banrmon = C_AnrMon.AnrMonAktiv
        bAnrMonTimer = C_HF.SetTimer(200)
    End Sub

    Private Sub bAnrMonTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles bAnrMonTimer.Elapsed
        If FritzBoxDialCommandBar IsNot Nothing Then
            Dim btnAnrMon As Office.CommandBarButton = Nothing
            Try
                btnAnrMon = CType(FritzBoxDialCommandBar.FindControl(Office.MsoControlType.msoControlButton, , DataProvider.P_CMB_eBtnAnrMon_Tag, , False), Office.CommandBarButton)
            Catch ex As Exception
                C_HF.LogFile("Fehler: " & DataProvider.P_CMB_eBtnAnrMon_Tag & " kann nicht gefunden werden (" & ex.Message & ").")
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
            Else
                C_HF.LogFile("Fehler: " & DataProvider.P_CMB_eBtnAnrMon_Tag & " kann nicht gefunden werden (btnAnrMon is Nothing).")
            End If

            C_HF.KillTimer(bAnrMonTimer)
            btnAnrMon = Nothing
        End If
    End Sub

    Sub SymbolleisteErzeugen(ByRef eBtnWaehlen As Office.CommandBarButton, _
                             ByRef eBtnDirektwahl As Office.CommandBarButton, _
                             ByRef eBtnAnrMon As Office.CommandBarButton, _
                             ByRef eBtnAnzeigen As Office.CommandBarButton, _
                             ByRef eBtnAnrMonNeuStart As Office.CommandBarButton, _
                             ByRef eBtnJournalimport As Office.CommandBarButton, _
                             ByRef eBtnEinstellungen As Office.CommandBarButton, _
                             ByRef ePopWwdh As Office.CommandBarPopup, _
                             ByRef ePopAnr As Office.CommandBarPopup, _
                             ByRef ePopVIP As Office.CommandBarPopup, _
                             ByRef ePopWwdhDel As Office.CommandBarButton, _
                             ByRef ePopWwdh01 As Office.CommandBarButton, ByRef ePopWwdh02 As Office.CommandBarButton, ByRef ePopWwdh03 As Office.CommandBarButton, _
                             ByRef ePopWwdh04 As Office.CommandBarButton, ByRef ePopWwdh05 As Office.CommandBarButton, ByRef ePopWwdh06 As Office.CommandBarButton, _
                             ByRef ePopWwdh07 As Office.CommandBarButton, ByRef ePopWwdh08 As Office.CommandBarButton, ByRef ePopWwdh09 As Office.CommandBarButton, _
                             ByRef ePopWwdh10 As Office.CommandBarButton, _
                             ByRef ePopAnrDel As Office.CommandBarButton, _
                             ByRef ePopAnr01 As Office.CommandBarButton, ByRef ePopAnr02 As Office.CommandBarButton, ByRef ePopAnr03 As Office.CommandBarButton, _
                             ByRef ePopAnr04 As Office.CommandBarButton, ByRef ePopAnr05 As Office.CommandBarButton, ByRef ePopAnr06 As Office.CommandBarButton, _
                             ByRef ePopAnr07 As Office.CommandBarButton, ByRef ePopAnr08 As Office.CommandBarButton, ByRef ePopAnr09 As Office.CommandBarButton, _
                             ByRef ePopAnr10 As Office.CommandBarButton, _
                             ByRef ePopVIPDel As Office.CommandBarButton, _
                             ByRef ePopVIP01 As Office.CommandBarButton, ByRef ePopVIP02 As Office.CommandBarButton, ByRef ePopVIP03 As Office.CommandBarButton, _
                             ByRef ePopVIP04 As Office.CommandBarButton, ByRef ePopVIP05 As Office.CommandBarButton, ByRef ePopVIP06 As Office.CommandBarButton, _
                             ByRef ePopVIP07 As Office.CommandBarButton, ByRef ePopVIP08 As Office.CommandBarButton, ByRef ePopVIP09 As Office.CommandBarButton, _
                             ByRef ePopVIP10 As Office.CommandBarButton)

        Dim i As Integer = 2

        FritzBoxDialCommandBar = AddCmdBar(DataProvider.P_Def_Addin_KurzName, True)

        eBtnWaehlen = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_Dial, 1, 568, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnWaehlen_Tag, DataProvider.P_CMB_Dial_ToolTipp)

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopWwdh, DataProvider.P_CMB_WWDH, i, DataProvider.P_Def_NameListCALL, DataProvider.P_CMB_WWDH_ToolTipp)
        i += 1

        Try
            ePopWwdhDel = AddPopupItems(ePopWwdh, 1)
            ePopWwdh01 = AddPopupItems(ePopWwdh, 2) : ePopWwdh02 = AddPopupItems(ePopWwdh, 3) : ePopWwdh03 = AddPopupItems(ePopWwdh, 4)
            ePopWwdh04 = AddPopupItems(ePopWwdh, 5) : ePopWwdh05 = AddPopupItems(ePopWwdh, 6) : ePopWwdh06 = AddPopupItems(ePopWwdh, 7)
            ePopWwdh07 = AddPopupItems(ePopWwdh, 8) : ePopWwdh08 = AddPopupItems(ePopWwdh, 9) : ePopWwdh09 = AddPopupItems(ePopWwdh, 10)
            ePopWwdh10 = AddPopupItems(ePopWwdh, 11)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopWwdh)")
        End Try

        FillPopupItems(DataProvider.P_Def_NameListCALL)
        ' Direktwahl
        ePopWwdh.Visible = C_DP.P_CBSymbWwdh
        ePopWwdh.Enabled = CommandBarPopupEnabled(ePopWwdh)
        eBtnDirektwahl = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_Direktwahl, i, 326, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnDirektwahl_Tag, DataProvider.P_CMB_Direktwahl_ToolTipp)
        i += 1

        eBtnDirektwahl.Visible = C_DP.P_CBSymbDirekt
        ' Symbol Anrufmonitor & Anzeigen
        eBtnAnrMon = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_AnrMon, i, 815, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnAnrMon_Tag, DataProvider.P_CMB_AnrMon_ToolTipp) '815

        eBtnAnzeigen = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_AnrMonAnzeigen, i + 1, 682, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnAnzeigen_Tag, DataProvider.P_CMB_AnrMonAnzeigen_ToolTipp)
        i += 2

        eBtnAnrMon.Visible = C_DP.P_CBSymbAnrMon
        eBtnAnzeigen.Visible = eBtnAnrMon.Visible

        eBtnAnrMonNeuStart = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_AnrMonNeuStart, i, 37, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnAnrMonNeuStart_Tag, DataProvider.P_CMB_AnrMonNeuStart_ToolTipp)

        eBtnAnrMonNeuStart.Visible = C_DP.P_CBSymbAnrMonNeuStart

        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopAnr, DataProvider.P_CMB_CallBack, i, DataProvider.P_Def_NameListRING, DataProvider.P_CMB_CallBack_ToolTipp)

        Try
            ePopAnrDel = AddPopupItems(ePopAnr, 1)
            ePopAnr01 = AddPopupItems(ePopAnr, 2) : ePopAnr02 = AddPopupItems(ePopAnr, 3) : ePopAnr03 = AddPopupItems(ePopAnr, 4)
            ePopAnr04 = AddPopupItems(ePopAnr, 5) : ePopAnr05 = AddPopupItems(ePopAnr, 6) : ePopAnr06 = AddPopupItems(ePopAnr, 7)
            ePopAnr07 = AddPopupItems(ePopAnr, 8) : ePopAnr08 = AddPopupItems(ePopAnr, 9) : ePopAnr09 = AddPopupItems(ePopAnr, 10)
            ePopAnr10 = AddPopupItems(ePopAnr, 11)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopAnr)")
        End Try

        FillPopupItems(DataProvider.P_Def_NameListRING)
        ePopAnr.Visible = C_DP.P_CBSymbAnrListe
        ePopAnr.Enabled = CommandBarPopupEnabled(ePopAnr)
        i += 1

        AddPopupsToExplorer(FritzBoxDialCommandBar, ePopVIP, DataProvider.P_CMB_VIP, i, DataProvider.P_Def_NameListVIP, DataProvider.P_CMB_VIP_ToolTipp)

        Try
            ePopVIPDel = AddPopupItems(ePopVIP, 1)
            ePopVIP01 = AddPopupItems(ePopVIP, 2) : ePopVIP02 = AddPopupItems(ePopVIP, 3) : ePopVIP03 = AddPopupItems(ePopVIP, 4)
            ePopVIP04 = AddPopupItems(ePopVIP, 5) : ePopVIP05 = AddPopupItems(ePopVIP, 6) : ePopVIP06 = AddPopupItems(ePopVIP, 7)
            ePopVIP07 = AddPopupItems(ePopVIP, 8) : ePopVIP08 = AddPopupItems(ePopVIP, 9) : ePopVIP09 = AddPopupItems(ePopVIP, 10)
            ePopVIP10 = AddPopupItems(ePopVIP, 11)
        Catch ex As Exception
            C_HF.FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "ThisAddIn_Startup (ePopVIP)")
        End Try

        FillPopupItems(DataProvider.P_Def_NameListVIP)
        i += 1
        ePopVIP.Visible = C_DP.P_CBSymbVIP
        ePopVIP.Enabled = CommandBarPopupEnabled(ePopVIP)

        eBtnJournalimport = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_Journal, i, 591, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnJournalimport_Tag, DataProvider.P_CMB_Journal_ToolTipp)
        eBtnJournalimport.Visible = C_DP.P_CBSymbJournalimport
        i += 1
        eBtnEinstellungen = AddButtonsToCmb(FritzBoxDialCommandBar, DataProvider.P_CMB_Setup, i, 548, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_CMB_eBtnEinstellungen_Tag, DataProvider.P_CMB_Setup_ToolTipp)
        i += 1

        eBtnWaehlen.TooltipText = DataProvider.P_CMB_Dial_ToolTipp
        ePopWwdh.TooltipText = DataProvider.P_CMB_WWDH_ToolTipp
        eBtnAnrMon.TooltipText = DataProvider.P_CMB_AnrMon_ToolTipp
        eBtnDirektwahl.TooltipText = DataProvider.P_CMB_Direktwahl_ToolTipp
        eBtnAnzeigen.TooltipText = DataProvider.P_CMB_AnrMonAnzeigen_ToolTipp
        eBtnAnrMonNeuStart.TooltipText = DataProvider.P_CMB_AnrMonNeuStart_ToolTipp
        ePopAnr.TooltipText = DataProvider.P_CMB_CallBack_ToolTipp
        ePopVIP.TooltipText = DataProvider.P_CMB_VIP_ToolTipp
        eBtnJournalimport.TooltipText = DataProvider.P_CMB_Journal_ToolTipp
        eBtnEinstellungen.TooltipText = DataProvider.P_CMB_Setup_ToolTipp

    End Sub

    Private Function CommandBarPopupEnabled(ByVal control As Office.CommandBarPopup) As Boolean
        Dim XMLListBaseNode As String = DataProvider.P_Def_ErrorMinusOne_String
        Dim xPathTeile As New ArrayList

        Select Case control.Tag
            Case DataProvider.P_Def_NameListCALL
                XMLListBaseNode = DataProvider.P_Def_NameListCALL
            Case DataProvider.P_Def_NameListRING
                XMLListBaseNode = DataProvider.P_Def_NameListRING
            Case DataProvider.P_Def_NameListVIP ' "VIPListe"
                XMLListBaseNode = DataProvider.P_Def_NameListVIP
        End Select

        If XMLListBaseNode = DataProvider.P_Def_ErrorMinusOne_String Then
            CommandBarPopupEnabled = False
        Else
            CommandBarPopupEnabled = CBool(IIf(Not C_XML.Read(C_DP.XMLDoc, XMLListBaseNode, "Index", DataProvider.P_Def_ErrorMinusOne_String) = DataProvider.P_Def_ErrorMinusOne_String, True, False))
        End If
    End Function

#End If
#If OVer = 11 Then
    Sub InspectorSybolleisteErzeugen(ByVal Inspector As Outlook.Inspector, _
                                     ByRef iPopRWS As Office.CommandBarPopup, _
                                     ByRef iBtnDial As Office.CommandBarButton, _
                                     ByRef iBtnRwsDasOertliche As Office.CommandBarButton, _
                                     ByRef iBtnRws11880 As Office.CommandBarButton, _
                                     ByRef iBtnRWSDasTelefonbuch As Office.CommandBarButton, _
                                     ByRef iBtnRWStelSearch As Office.CommandBarButton, _
                                     ByRef iBtnRWSAlle As Office.CommandBarButton, _
                                     ByRef iBtnKontakterstellen As Office.CommandBarButton, _
                                     ByRef iBtnVIP As Office.CommandBarButton, _
                                     ByRef iBtnUpload As Office.CommandBarButton)

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
                        If cmb.NameLocal = DataProvider.P_Def_Addin_KurzName Then
                            cmbErstellen = False
                            Exit For
                        End If
                    Next
                End If
                If cmbErstellen Then
                    cmb = Inspector.CommandBars.Add(DataProvider.P_Def_Addin_KurzName, Microsoft.Office.Core.MsoBarPosition.msoBarTop, , True)
                    With cmb
                        .NameLocal = DataProvider.P_Def_Addin_KurzName
                        .Visible = True
                    End With
                    iBtnDial = AddButtonsToCmb(cmb, DataProvider.P_CMB_Dial, i, 568, MsoButtonStyle.msoButtonIconAndCaption, DataProvider.P_Tag_Insp_Dial, DataProvider.P_CMB_Dial_ToolTipp)
                    i += 1
                End If
            End If
            ' Kontakteinträge
            If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Or TypeOf Inspector.CurrentItem Is Outlook.JournalItem Then

                AddPopupsToExplorer(cmb, iPopRWS, DataProvider.P_CMB_Insp_RWS, i, "RWS", DataProvider.P_CMB_Insp_RWS_ToolTipp)
                i += 1
                iBtnRwsDasOertliche = AddPopupItems(iPopRWS, 1)
                iBtnRws11880 = AddPopupItems(iPopRWS, 2)
                iBtnRWSDasTelefonbuch = AddPopupItems(iPopRWS, 3)
                iBtnRWStelSearch = AddPopupItems(iPopRWS, 4)
                iBtnRWSAlle = AddPopupItems(iPopRWS, 5)

                Dim rwsNamen() As String = {DataProvider.P_RWSDasOertliche_Name, _
                                            DataProvider.P_RWS11880_Name, _
                                            DataProvider.P_RWSDasTelefonbuch_Name, _
                                            DataProvider.P_RWSTelSearch_Name, _
                                            DataProvider.P_RWSAlle_Name}
                Dim rwsToolTipp() As String = {DataProvider.P_RWS_ToolTipp(DataProvider.P_RWSDasOertliche_Link), _
                                               DataProvider.P_RWS_ToolTipp(DataProvider.P_RWS11880_Link), _
                                               DataProvider.P_RWS_ToolTipp(DataProvider.P_RWSDasTelefonbuch_Link), _
                                               DataProvider.P_RWS_ToolTipp(DataProvider.P_RWSTelSearch_Link), _
                                               DataProvider.P_RWS_ToolTipp()}

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
                iBtnVIP = AddButtonsToCmb(cmb, DataProvider.P_CMB_Insp_VIP, i, 3710, MsoButtonStyle.msoButtonIconAndCaption, "VIP", DataProvider.P_CMB_VIP_Hinzufügen_ToolTipp)
                i += 1
                Dim olKontakt As Outlook.ContactItem = CType(Inspector.CurrentItem, Outlook.ContactItem)
                With iBtnVIP
                    If IsVIP(olKontakt) Then
                        .State = Office.MsoButtonState.msoButtonDown
                        .TooltipText = DataProvider.P_CMB_VIP_Entfernen_ToolTipp
                    Else
                        If CLng(C_XML.Read(C_DP.XMLDoc, DataProvider.P_Def_NameListVIP, "Index", "0")) >= 10 Then
                            .TooltipText = DataProvider.P_CMB_VIP_O11_Voll_ToolTipp
                            .Enabled = False
                        Else
                            .TooltipText = DataProvider.P_CMB_VIP_Hinzufügen_ToolTipp
                        End If
                        .State = Office.MsoButtonState.msoButtonUp
                    End If
                    .Tag = DataProvider.P_CMB_Insp_VIP
                    .Visible = C_DP.P_CBSymbVIP
                End With
                ' Upload
                iBtnUpload = AddButtonsToCmb(cmb, DataProvider.P_CMB_Insp_Upload, i, 3732, MsoButtonStyle.msoButtonIconAndCaption, "Upload", DataProvider.P_CMB_Insp_UploadKontakt_ToolTipp)
                i += 1
            End If
            ' Journaleinträge
            If TypeOf Inspector.CurrentItem Is Outlook.JournalItem Then
                iBtnKontakterstellen = AddButtonsToCmb(cmb, _
                                                       DataProvider.P_CMB_Kontakt_Erstellen, _
                                                       i, 1099, MsoButtonStyle.msoButtonIconAndCaption, _
                                                       DataProvider.P_Tag_Insp_Kontakt, _
                                                       DataProvider.P_CMB_Kontakt_Erstellen_ToolTipp)
                i += 1
                Dim olJournal As Outlook.JournalItem = CType(Inspector.CurrentItem, Outlook.JournalItem)
                If Not InStr(1, olJournal.Categories, "FritzBox Anrufmonitor; Telefonanrufe", vbTextCompare) = 0 Then
                    Dim olLink As Outlook.Link = Nothing
                    For Each olLink In olJournal.Links
                        If TypeOf olLink.Item Is Outlook.ContactItem Then iBtnKontakterstellen.Caption = DataProvider.P_CMB_Kontakt_Anzeigen
                        Exit For
                    Next
                    C_HF.NAR(olLink) : olLink = Nothing
                    iPopRWS.Enabled = True
                    iBtnDial.Enabled = Not CBool(InStr(olJournal.Body, "Tel.-Nr.: " & DataProvider.P_Def_StringUnknown, CompareMethod.Text))
                    iBtnKontakterstellen.Enabled = True
                Else
                    cmb.Delete()
                End If
            End If
        End If
    End Sub
#End If
#End Region

#Region "Explorer Button Click"
    ''' <summary>
    ''' Mögliche Anwendungen, die durch den klick auf ein Button/Ribbon ausgelöst werden können.
    ''' Warum, die Englisch sind? Keine Ahnung.
    ''' </summary>
    Friend Enum TaskToDo
        OpenConfig          ' Explorer: Einstellung Öffnen
        OpenJournalimport   ' Explorer: Journalimport öffnen
        ShowAnrMon          ' Explorer: Letzten Anrufer anzeigen
        RestartAnrMon       ' Explorer: Anrufmonitor neu starten
        DialExplorer        ' Explorer: Klassischen Wähldialog über das ausgewählte Objekt öffnen
        DialDirect          ' Explorer: Direktwahl öffnen
        DialInspector       ' Inspector: Wähldialog öffnen 
        CreateContact       ' Inspector: Journal, Kontakt erstellen
    End Enum

    ''' <summary>
    ''' Steuert die aufzurufende Funktion anhand der Übergebenen <c>Aufgabe</c>
    ''' </summary>
    ''' <param name="Aufgabe">Übergabe Wert, der bestimmt, was getan werden soll.</param>
    Friend Sub OnAction(ByVal Aufgabe As TaskToDo)
        Select Case Aufgabe
            Case TaskToDo.DialDirect
                P_CallClient.Wählbox(Nothing, DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, True)
            Case TaskToDo.DialExplorer
                If C_OLI.OutlookApplication IsNot Nothing Then
                    P_CallClient.WählboxStart(C_OLI.OutlookApplication.ActiveExplorer.Selection)
                End If
            Case TaskToDo.OpenConfig
                P_Config.ShowDialog()
            Case TaskToDo.OpenJournalimport
                If Not P_AnrList Is Nothing Then
                    P_AnrList = New formImportAnrList(C_FBox, C_AnrMon, C_HF, C_DP, C_XML)
                End If
                P_AnrList.StartAuswertung(True)
            Case TaskToDo.RestartAnrMon
                C_AnrMon.AnrMonReStart()
            Case TaskToDo.ShowAnrMon
                C_PopUp.AnrMonEinblenden(C_AnrMon.LetzterAnrufer)
            Case TaskToDo.DialInspector
                P_CallClient.WählenAusInspector()
            Case TaskToDo.CreateContact
                C_KF.ZeigeKontaktAusJournal()
        End Select
    End Sub

#End Region

#Region "Inspector Button Click"
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
                                    ByVal vCard As String, _
                                    ByVal Verpasst As Boolean)

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim index As Integer              ' Zählvariable

        index = CInt(C_XML.Read(C_DP.XMLDoc, ListName, "Index", "0"))

        xPathTeile.Add(ListName)
        xPathTeile.Add("Eintrag[@ID=""" & index - 1 & """]")
        xPathTeile.Add("TelNr")

        If Not C_HF.TelNrVergleich(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0"), TelNr) Then

            NodeNames.Add("Index")
            NodeValues.Add(CStr((index + 1) Mod 10))

            If Not Anrufer = DataProvider.P_Def_LeerString Then
                NodeNames.Add("Anrufer")
                NodeValues.Add(Anrufer)
            End If

            If Not TelNr = DataProvider.P_Def_LeerString Then
                NodeNames.Add("TelNr")
                NodeValues.Add(TelNr)
            End If

            If Not Zeit = Nothing Then
                NodeNames.Add("Zeit")
                NodeValues.Add(Zeit)
            End If

            If Not StoreID = DataProvider.P_Def_LeerString Then
                NodeNames.Add("StoreID")
                NodeValues.Add(StoreID)
            End If

            If Not KontaktID = DataProvider.P_Def_LeerString Then
                NodeNames.Add("KontaktID")
                NodeValues.Add(KontaktID)
            End If

            If Not vCard = DataProvider.P_Def_LeerString Then
                NodeNames.Add("vCard")
                NodeValues.Add(vCard)
            End If

            AttributeNames.Add("ID")
            AttributeValues.Add(CStr(index))

            With C_DP
                xPathTeile.Clear() 'RemoveRange(0, xPathTeile.Count)
                xPathTeile.Add(ListName)
                xPathTeile.Add("Index")
                C_XML.Write(.XMLDoc, xPathTeile, CStr((index + 1) Mod 10))
                xPathTeile.Remove("Index")
                C_XML.AppendNode(.XMLDoc, xPathTeile, C_XML.CreateXMLNode(.XMLDoc, "Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
            End With
        Else
            ' Zeit anpassen
            If Not Zeit = Nothing Then
                xPathTeile.Item(xPathTeile.Count - 1) = "Zeit"
                C_XML.Write(C_DP.XMLDoc, xPathTeile, CStr(Zeit))
            End If

            ' Verpasst Status setzen
            xPathTeile.Item(xPathTeile.Count - 1) = "Verpasst"
            C_XML.Write(C_DP.XMLDoc, xPathTeile, CStr(Verpasst))
        End If

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
            UpdateList(ListName, .Anrufer, .TelNr, CStr(.Zeit), .StoreID, .KontaktID, .vCard, .Verpasst)
        End With
    End Sub

    ''' <summary>
    ''' Behandelt das Ereignis, welches beim Klick auf die PopUp-Items ausgelöst wird.
    ''' Funktion würd für alle Offeice Versionen benötigt.
    ''' </summary>
    ''' <param name="ControlTag">Tag des Control Items in der folgenden Form: ####List;ID</param>
    Friend Sub OnActionListen(ByVal ControlTag As String)
        Dim oContact As Outlook.ContactItem
        Dim Telefonat As String() = Split(ControlTag, ";", , CompareMethod.Text)
        ' KontaktID, StoreID, TelNr ermitteln
        Dim KontaktID As String
        Dim StoreID As String
        Dim TelNr As String
        'Dim Verpasst As Boolean
        Dim vCard As String
        Dim ListNodeNames As New ArrayList
        Dim ListNodeValues As New ArrayList
        Dim xPathTeile As New ArrayList

        ' TelNr
        ListNodeNames.Add("TelNr")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        ' Anrufer
        ListNodeNames.Add("Anrufer")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        ' StoreID
        ListNodeNames.Add("StoreID")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        ' KontaktID
        ListNodeNames.Add("KontaktID")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String & ";")

        ' vCard
        ListNodeNames.Add("vCard")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String & ";")

        ' Verpasst
        ListNodeNames.Add("Verpasst")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String & ";")

        With xPathTeile
            .Add(Telefonat(0))
            .Add("Eintrag")
        End With
        C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, ListNodeNames, ListNodeValues, "ID", Telefonat(1))

        TelNr = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("TelNr")))
        KontaktID = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("KontaktID")))
        StoreID = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("StoreID")))
        vCard = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("vCard")))
        'Verpasst = CBool(ListNodeValues.Item(ListNodeNames.IndexOf("Verpasst")))

        If Not StoreID = DataProvider.P_Def_ErrorMinusOne_String Then
            'If Not KontaktID = DataProvider.P_Def_ErrorMinusOne And Not StoreID = DataProvider.P_Def_ErrorMinusOne Then
            oContact = C_KF.GetOutlookKontakt(KontaktID, StoreID)
            If oContact Is Nothing Then
                Select Case Telefonat(0)
                    Case DataProvider.P_Def_NameListVIP
                        If C_HF.FBDB_MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben. Soll der zugehörige VIP-Eintrag entfernt werden?", MsgBoxStyle.YesNo, "OnActionListen") = MsgBoxResult.Yes Then
                            RemoveVIP(KontaktID, StoreID)
                        End If
                    Case Else
                        C_HF.FBDB_MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben.", MsgBoxStyle.Critical, "OnActionListen")
                End Select
            End If
        Else
            oContact = Nothing
        End If

        ' Verpasst-Marker auf false setzen
        With xPathTeile
            .Add("[@ID=""" & Telefonat(1) & """]")
            .Add("Verpasst")
        End With
        C_XML.Write(C_DP.XMLDoc, xPathTeile, "False")

        C_WClient.Wählbox(oContact, TelNr, vCard, False) '.TooltipText = TelNr. - .Caption = evtl. vorh. Name.
    End Sub


    ''' <summary>
    ''' Löscht die gesammte gewählte Liste aus der XML
    ''' </summary>
    ''' <param name="ControlID">ID der Liste</param>
    ''' <remarks></remarks>
    Friend Sub ClearInListe(ByVal ControlID As String)
        Dim xPathTeile As New ArrayList
        Dim Eintrag() As String = Split(ControlID, "_", , CompareMethod.Text)
        Dim NameListe As String = DataProvider.P_Def_StringNull

        Select Case Eintrag(1)
            Case "RingList"
                NameListe = DataProvider.P_CMB_CallBack
            Case "CallList"
                NameListe = DataProvider.P_CMB_WWDH
            Case "VIPList"
                NameListe = DataProvider.P_CMB_VIP
        End Select

        xPathTeile.Clear()
        xPathTeile.Add(Eintrag(1)) 'Liste

        If Not NameListe = DataProvider.P_Def_StringNull Then
            If UBound(Eintrag) = 2 Then
                xPathTeile.Add("Eintrag[@ID=""" & Eintrag(2) & """]")
                C_HF.LogFile("Die Eintrag mit ID" & Eintrag(2) & " der Liste " & NameListe & " wurde gelöscht.")
            Else
                C_HF.LogFile("Die Liste " & NameListe & " wurde gelöscht.")
            End If

            C_XML.Delete(C_DP.XMLDoc, xPathTeile)

#If OVer < 14 Then
            FillPopupItems(Eintrag(1))
#Else
            RefreshRibbon()
#End If
        End If
    End Sub
#End Region

End Class