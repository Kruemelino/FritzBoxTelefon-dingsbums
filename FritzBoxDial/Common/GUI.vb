<Runtime.InteropServices.ComVisible(True)> Public Class GraphicalUserInterface
#Region "Ribbon Grundlagen für Outlook 2007 bis 2013"
    Implements Office.IRibbonExtensibility

    Private RibbonObjekt As Office.IRibbonUI

    Public Function GetCustomUI(ByVal ribbonID As String) As String Implements Office.IRibbonExtensibility.GetCustomUI
        Dim File As String

        Select Case ribbonID
            Case "Microsoft.Outlook.Explorer"
                File = GetResourceText("FritzBoxDial.RibbonExplorer.xml")
            Case "Microsoft.Outlook.Mail.Read"
                File = GetResourceText("FritzBoxDial.RibbonInspectorMailRead.xml")
            Case "Microsoft.Outlook.Journal"
                File = GetResourceText("FritzBoxDial.RibbonInspectorJournal.xml")
            Case "Microsoft.Outlook.Contact"
                File = GetResourceText("FritzBoxDial.RibbonInspectorKontakt.xml")
            Case "Microsoft.Mso.IMLayerUI"
                File = GetResourceText("FritzBoxDial.RibbonIMLayerUI.xml")
            Case Else
                File = DataProvider.P_Def_LeerString
        End Select
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
#End Region

#Region "Eigene Klassen"
    Private C_XML As XML
    Private C_hf As Helfer
    Private C_DP As DataProvider
    Private C_KF As KontaktFunktionen
#End Region

#Region "Eigene Formulare"
    Private F_RWS As formRWSuche
    Friend Property P_CallClient() As Wählclient
    Friend Property P_AnrufMonitor() As AnrufMonitor
    Public Property P_OlInterface() As OutlookInterface
    Public Property P_FritzBox() As FritzBox
    Public Property P_PopUp() As Popup
    Public Property P_Config() As formCfg
    Public Property P_AnrList() As formImportAnrList
#End Region

    Friend Sub New(ByVal HelferKlasse As Helfer, ByVal DataProviderKlasse As DataProvider, ByVal Inverssuche As formRWSuche, ByVal KontaktKlasse As KontaktFunktionen, ByVal PopUpKlasse As Popup, ByVal XMLKlasse As XML)
        C_hf = HelferKlasse
        C_DP = DataProviderKlasse
        F_RWS = Inverssuche
        C_KF = KontaktKlasse
        P_PopUp = PopUpKlasse
        C_XML = XMLKlasse
    End Sub

#Region "Ribbon Behandlung für Outlook 2010 bis 2016"

#Region "Ribbon Inspector Office 2010 bis Office 2016" ' Ribbon Inspektorfenster

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

        With C_hf
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

#If OVer = 14 Then
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

#If OVer = 14 Then
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

#Region "Ribbon Expector Office 2010 bis Office 2019" 'Ribbon Explorer

    Sub Ribbon_Load(ByVal Ribbon As Office.IRibbonUI)
        RibbonObjekt = Ribbon
    End Sub

    Public Function FillDynamicMenu(ByVal control As Office.IRibbonControl) As String
        Dim XMLListBaseNode As String

        Dim index As Integer
        Dim Anrufer As String
        Dim TelNr As String
        Dim Zeit As String
        Dim Verpasst As Boolean = False

        Dim LANodeNames As ArrayList
        Dim LANodeValues As ArrayList
        Dim xPathTeile As ArrayList

        Dim RibbonListStrBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf &
                                                                      "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)

        Select Case Mid(control.Id, 1, Len(control.Id) - 2)
            Case DataProvider.P_Def_NameListCALL
                XMLListBaseNode = DataProvider.P_Def_NameListCALL '"CallList"
            Case DataProvider.P_Def_NameListRING
                XMLListBaseNode = DataProvider.P_Def_NameListRING '"RingList"
            Case Else 'DataProvider.P_Def_NameListVIP
                XMLListBaseNode = DataProvider.P_Def_NameListVIP '"VIPList"
        End Select

        index = CInt(C_XML.Read(C_DP.XMLDoc, XMLListBaseNode, "Index", "0"))

        LANodeNames = C_XML.XPathConcat("Anrufer", "TelNr", "Zeit")
        LANodeValues = C_XML.XPathConcat(DataProvider.P_Def_ErrorMinusOne_String, DataProvider.P_Def_ErrorMinusOne_String, DataProvider.P_Def_ErrorMinusOne_String)

        ' Signalisierung verpasster Anrufe
        If XMLListBaseNode = DataProvider.P_Def_NameListRING Then
            LANodeNames.Add("Verpasst")
            LANodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)
        End If

        xPathTeile = C_XML.XPathConcat(XMLListBaseNode, "Eintrag")

        With RibbonListStrBuilder
            .Append("<button id=""dynListDel_" & XMLListBaseNode & """ getLabel=""GetItemLabel"" onAction=""BtnOnAction"" getImage=""GetItemImageMso"" />" & vbCrLf)
            .Append("<menuSeparator id=""separator"" />" & vbCrLf)
        End With

        If Not XMLListBaseNode = DataProvider.P_Def_NameListVIP Then

            For ID = index + C_DP.P_TBNumEntryList - 1 To index Step -1

                C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, LANodeNames, LANodeValues, "ID", CStr(ID Mod C_DP.P_TBNumEntryList))
                TelNr = CStr(LANodeValues.Item(LANodeNames.IndexOf("TelNr")))

                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                    Anrufer = CStr(LANodeValues.Item(LANodeNames.IndexOf("Anrufer")))
                    Zeit = CStr(LANodeValues.Item(LANodeNames.IndexOf("Zeit")))
                    If XMLListBaseNode = DataProvider.P_Def_NameListRING Then Verpasst = CBool(LANodeValues.Item(LANodeNames.IndexOf("Verpasst")))

                    GetButtonXMLString(RibbonListStrBuilder,
                            CStr(ID Mod C_DP.P_TBNumEntryList),
                            C_hf.IIf(Anrufer = DataProvider.P_Def_ErrorMinusOne_String, TelNr, Anrufer),
                            XMLListBaseNode,
                            DataProvider.P_CMB_ToolTipp(Zeit, TelNr),
                            C_hf.IIf(Verpasst, "HighImportance", DataProvider.P_Def_LeerString))

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

                    GetButtonXMLString(RibbonListStrBuilder,
                            CStr(ID),
                            Anrufer,
                            XMLListBaseNode,
                            DataProvider.P_Def_LeerString,
                            DataProvider.P_Def_LeerString)

                    LANodeValues.Item(0) = DataProvider.P_Def_ErrorMinusOne_String
                End If
            Next
        End If

        RibbonListStrBuilder.Append("</menu>")

        FillDynamicMenu = RibbonListStrBuilder.ToString
        RibbonListStrBuilder.Clear()
        RibbonListStrBuilder = Nothing
        LANodeNames = Nothing
        LANodeValues = Nothing
        xPathTeile = Nothing
    End Function

    Public Function GetPhonebooks(ByVal control As Office.IRibbonControl) As String
        Dim TelBuchList As String() = P_FritzBox.GetTelefonbuchListe()
        Dim RibbonListStrBuilder As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf &
                                                                      "<menu xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">" & vbCrLf)

        With RibbonListStrBuilder
            For Each TelBuch As String In TelBuchList
                .Append("<button id=""" & Split(control.Id, "_",, CompareMethod.Text)(0) & "_" & Split(TelBuch, ";", , CompareMethod.Text)(0) & """ label=""" & Split(TelBuch, ";", , CompareMethod.Text)(1) & """ onAction=""BtnOnAction"" />" & vbCrLf)
            Next
            .Append("</menu>")
            GetPhonebooks = RibbonListStrBuilder.ToString
            .Clear()
        End With

        RibbonListStrBuilder = Nothing
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
            .Append("<button id=""" & Werte(2) & "_" & Werte(0) & """ label=""" & Werte(1) & """ onAction=""BtnOnAction"" tag=""" & Werte(2) & ";" & Werte(0) & """ ")

            If Not Werte(3) = DataProvider.P_Def_LeerString Then .Append("supertip=""" & Werte(3) & """ ")
            If Not Werte(4) = DataProvider.P_Def_LeerString Then .Append("imageMso=""" & Werte(4) & """ ")

            .Append("/>" & vbCrLf)
        End With
    End Sub

    Public Function DynMenüEnabled(ByVal control As Office.IRibbonControl) As Boolean
        Dim XMLListBaseNode As String

        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case DataProvider.P_Def_NameListCALL
                XMLListBaseNode = DataProvider.P_Def_NameListCALL '"CallList"
            Case DataProvider.P_Def_NameListRING
                XMLListBaseNode = DataProvider.P_Def_NameListRING '"RingList"
            Case Else 'DataProvider.P_Def_NameListVip
                XMLListBaseNode = DataProvider.P_Def_NameListVIP '"VIPList"
        End Select

        Return Not C_XML.Read(C_DP.XMLDoc, XMLListBaseNode, "Index", DataProvider.P_Def_ErrorMinusOne_String) = DataProvider.P_Def_ErrorMinusOne_String
    End Function

    Public Function GetPressed(ByVal control As Office.IRibbonControl) As Boolean
        GetPressed = False
        If P_AnrufMonitor IsNot Nothing Then GetPressed = P_AnrufMonitor.AnrMonAktiv
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

    ''' <summary>
    ''' Die Uploadfunktion im Kontextmenü. Bei SOAP soll das Telefonbuch auswählbar sein.
    ''' </summary>
    ''' <param name="control"></param>
    ''' <returns></returns>
    Public Function GetVisibleUploadFKT(ByVal control As Office.IRibbonControl) As Boolean
        GetVisibleUploadFKT = False
        Select Case Split(control.Id, "_",, CompareMethod.Text)(0)
            Case "cbtnUpload", "btnUpload"
                GetVisibleUploadFKT = Not C_DP.P_RBFBComUPnP
            Case "cdMUpload", "MUpload"
                GetVisibleUploadFKT = C_DP.P_RBFBComUPnP
        End Select
    End Function
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
            Case "CallList"
                Return DataProvider.P_CMB_WWDH
            Case "RingList"
                Return DataProvider.P_CMB_CallBack
            Case "VIPList"
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
            Case "cbtnDial", "rbtnDial" ' ContextMenu Dial
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
            Case "btnUpload", "cdMUpload", "MUpload"
                Return DataProvider.P_CMB_Insp_Upload
            Case Else
                C_hf.LogFile("GetItemLabel: Kann control.Id " & control.Id & " nicht auswerten.")
                Return DataProvider.P_Def_ErrorMinusOne_String
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemScreenTipp(ByVal control As Office.IRibbonControl) As String
        Select Case Split(control.Id, "_", 2, CompareMethod.Text)(0)
            Case "btnDialExpl", "btnDialInsp", "rbtnDial"
                Return DataProvider.P_CMB_Dial_ToolTipp
            Case "btnDirektwahl"
                Return DataProvider.P_CMB_Direktwahl_ToolTipp
            Case "CallList"
                Return DataProvider.P_CMB_WWDH_ToolTipp
            Case "RingList"
                Return DataProvider.P_CMB_CallBack_ToolTipp
            Case "VIPList"
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
                Return C_hf.IIf(IsVIP(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)), DataProvider.P_CMB_VIP_Entfernen_ToolTipp, DataProvider.P_CMB_VIP_Hinzufügen_ToolTipp)
            Case "btnUpload"
                Return DataProvider.P_CMB_Insp_UploadKontakt_ToolTipp()
            Case Else
                C_hf.LogFile("GetItemScreenTipp: Kann control.Id " & control.Id & " nicht auswerten.")
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
            Case "btnDialExpl", "btnDialInsp", "rbtnDial"
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
                If P_AnrufMonitor IsNot Nothing Then
                    If P_AnrufMonitor.AnrMonAktiv Then
                        GetItemImageMso = "PersonaStatusOnline"
                    Else
                        If Not P_AnrufMonitor.AnrMonError Then GetItemImageMso = "PersonaStatusOffline"
                    End If
                End If
            Case "btnAnrMonRestart"
                Return "RecurrenceEdit"
            Case "btnAnrMonShow"
                Return "ClipArtInsert"
            Case "btnAnrMonJI"
                Return "NewJournalEntry"
            Case "btnUpload", "MUpload"
                Return "DistributionListAddNewMember"
            Case "mnuRWS" ' Inspector
                Return "CheckNames"
            Case "btnAddContact" ' Inspector
                Return "RecordsSaveAsOutlookContact"
            Case "btnNote" ' Inspector
                Return "ShowNotesPage"
                'Case "CallList", "RingList", "VIPList"
                '    Return DataProvider.P_Def_LeerString
            Case Else
                C_hf.LogFile("GetItemImageMso: Kann control.Id " & control.Id & " nicht auswerten.")
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
            Case "rbtnDial"
                Try
                    Dim card As Office.IMsoContactCard = TryCast(control.Context, Office.IMsoContactCard)

                    If card IsNot Nothing Then
                        P_CallClient.WählenAusEMail(P_OlInterface.GetSmtpAddress(card))
                    Else
                        C_hf.LogFile("Unable to access contact card")
                    End If
                Catch ex As Exception
                    C_hf.LogFile(ex.Message)
                End Try
            Case "btnDirektwahl"
                OnAction(TaskToDo.DialDirect)
            Case DataProvider.P_Def_NameListCALL, DataProvider.P_Def_NameListRING, DataProvider.P_Def_NameListVIP
                OnActionListen(control.Tag)
            Case "dynListDel"
                ClearInListe(control.Id)
            Case "btnAnrMonIO"
                P_AnrufMonitor.AnrMonStartStopp()
            Case "btnAnrMonRestart"
                OnAction(TaskToDo.RestartAnrMon)
            Case "btnAnrMonShow"
                OnAction(TaskToDo.ShowAnrMon)
            Case "btnAnrMonJI"
                OnAction(TaskToDo.OpenJournalimport)
            Case "Einstellungen"
                OnAction(TaskToDo.OpenConfig)
            Case "cbtnUpload"  ' Kontextmenü
                Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
                P_FritzBox.UploadKontaktToFritzBox(oKontakt, IsVIP(oKontakt), "")
            Case "btnUpload"   ' Inspector
                Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)
                P_FritzBox.UploadKontaktToFritzBox(oKontakt, IsVIP(oKontakt), "")
            Case "cdMUpload"  ' Kontext + Telefonbuchauswahl
                Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
                P_FritzBox.UploadKontaktToFritzBox(oKontakt, IsVIP(oKontakt), Split(control.Id, "_", 2, CompareMethod.Text)(1))
            Case "MUpload"  ' Inspector + Telefonbuchauswahl
                Dim oKontakt As Outlook.ContactItem = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)
                P_FritzBox.UploadKontaktToFritzBox(oKontakt, IsVIP(oKontakt), Split(control.Id, "_", 2, CompareMethod.Text)(1))
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

        Dim oKontakt As Outlook.ContactItem = Nothing

        Select Case control.Id
            Case "ctbtnVIP" ' Kontext Menu
                oKontakt = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
            Case "tbtnVIP_C1"  ' Kontaktinspector 
                oKontakt = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)
        End Select

        If Not oKontakt Is Nothing Then
            If IsVIP(oKontakt) Then
                RemoveVIP(oKontakt.EntryID, CType(oKontakt.Parent, Outlook.MAPIFolder).StoreID)
            Else
                AddVIP(oKontakt)
            End If
            C_hf.NAR(oKontakt)
        End If

        oKontakt = Nothing
        ' Fehler unter Office 2007
        RibbonObjekt.Invalidate()

    End Sub

    Public Function CtBtnPressedVIP(ByVal control As Office.IRibbonControl) As Boolean
        CtBtnPressedVIP = False
        Dim oKontact As Outlook.ContactItem = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)

        CtBtnPressedVIP = IsVIP(oKontact)

        C_hf.NAR(oKontact)
        oKontact = Nothing
    End Function

    Public Function tBtnPressedVIP(ByVal control As Office.IRibbonControl) As Boolean
        tBtnPressedVIP = False

        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)

        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim olContact As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)

            tBtnPressedVIP = IsVIP(olContact)

            C_hf.NAR(olContact)
            olContact = Nothing
        End If
    End Function

#End Region

#End Region

#Region "VIP-Generell"
    Friend Function IsVIP(ByVal aktKontakt As Outlook.ContactItem) As Boolean
        IsVIP = False

        Dim KontaktID As String = aktKontakt.EntryID
        Dim StoreID As String = CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID
        Dim xPathTeile As ArrayList

        xPathTeile = C_XML.XPathConcat(DataProvider.P_Def_NameListVIP, "Eintrag", "[(KontaktID = """ & KontaktID & """ and StoreID = """ & StoreID & """)]")
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
        RefreshRibbon()
        Return True
    End Function

    Friend Overloads Function AddVIP(ByVal KontaktID As String, ByVal StoreID As String) As Boolean
        Dim oKontact As Outlook.ContactItem
        oKontact = Nothing

        Try
            oKontact = CType(CType(P_OlInterface.OutlookApplication.GetNamespace("MAPI"), Outlook.NameSpace).GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
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

        RefreshRibbon()

        xPathTeile = Nothing
        C_DP.SpeichereXMLDatei()
        Return True
    End Function
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
                If P_OlInterface.OutlookApplication IsNot Nothing Then
                    P_CallClient.WählboxStart(P_OlInterface.OutlookApplication.ActiveExplorer.Selection)
                End If
            Case TaskToDo.OpenConfig
                P_Config.ShowDialog()
            Case TaskToDo.OpenJournalimport
                If Not P_AnrList Is Nothing Then
                    P_AnrList = New formImportAnrList(P_FritzBox, P_AnrufMonitor, C_hf, C_DP, C_XML)
                End If
                P_AnrList.StartAuswertung(True)
            Case TaskToDo.RestartAnrMon
                P_AnrufMonitor.Restart(False)
            Case TaskToDo.ShowAnrMon
                P_PopUp.AnrMonEinblenden(P_AnrufMonitor.LetzterAnrufer)
            Case TaskToDo.DialInspector
                P_CallClient.WählenAusInspector()
            Case TaskToDo.CreateContact
                C_KF.ZeigeKontaktAusJournal()
        End Select
    End Sub

    'Private Function GetSmtpAddress(ByVal card As Office.IMsoContactCard) As String
    '    If card.AddressType = Office.MsoContactCardAddressType.msoContactCardAddressTypeOutlook Then
    '        Dim host As Outlook.Application = Globals.ThisAddIn.Application
    '        Dim ae As Outlook.AddressEntry = host.Session.GetAddressEntryFromID(card.Address)

    '        Select Case ae.AddressEntryUserType
    '            Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
    '                Dim ex As Outlook.ExchangeUser = ae.GetExchangeUser()
    '                Return ex.PrimarySmtpAddress
    '            Case Outlook.OlAddressEntryUserType.olOutlookContactAddressEntry
    '                Return ae.Address
    '            Case Else
    '                Throw New Exception("Valid address entry not found.")
    '        End Select
    '    Else
    '        Return card.Address
    '    End If
    'End Function
#End Region

#Region "RingCallList"
    Friend Overloads Sub UpdateList(ByVal ListName As String,
                                    ByVal Anrufer As String,
                                    ByVal TelNr As String,
                                    ByVal Zeit As String,
                                    ByVal StoreID As String,
                                    ByVal KontaktID As String,
                                    ByVal vCard As String,
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

        If Not C_hf.TelNrVergleich(C_XML.Read(C_DP.XMLDoc, xPathTeile, "0"), TelNr) Then

            NodeNames.Add("Index")
            NodeValues.Add(CStr((index + 1) Mod C_DP.P_TBNumEntryList))

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
                C_XML.Write(.XMLDoc, xPathTeile, CStr((index + 1) Mod C_DP.P_TBNumEntryList))
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

        RefreshRibbon()

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
                        If C_hf.MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben. Soll der zugehörige VIP-Eintrag entfernt werden?", MsgBoxStyle.YesNo, "OnActionListen") = MsgBoxResult.Yes Then
                            RemoveVIP(KontaktID, StoreID)
                        End If
                    Case Else
                        C_hf.MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben.", MsgBoxStyle.Critical, "OnActionListen")
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

        P_CallClient.Wählbox(oContact, TelNr, vCard, False) '.TooltipText = TelNr. - .Caption = evtl. vorh. Name.
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
            Case DataProvider.P_Def_NameListRING
                NameListe = DataProvider.P_CMB_CallBack
            Case DataProvider.P_Def_NameListCALL
                NameListe = DataProvider.P_CMB_WWDH
            Case DataProvider.P_Def_NameListVIP
                NameListe = DataProvider.P_CMB_VIP
        End Select

        xPathTeile.Clear()
        xPathTeile.Add(Eintrag(1)) 'Liste

        If Not NameListe = DataProvider.P_Def_StringNull Then
            If UBound(Eintrag) = 2 Then
                xPathTeile.Add("Eintrag[@ID=""" & Eintrag(2) & """]")
                C_hf.LogFile("Die Eintrag mit ID" & Eintrag(2) & " der Liste " & NameListe & " wurde gelöscht.")
            Else
                C_hf.LogFile("Die Liste " & NameListe & " wurde gelöscht.")
            End If

            C_XML.Delete(C_DP.XMLDoc, xPathTeile)

            RefreshRibbon()

        End If
    End Sub
#End Region

End Class