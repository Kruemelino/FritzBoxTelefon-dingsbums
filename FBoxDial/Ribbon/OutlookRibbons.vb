Imports Microsoft.Office.Core
Imports FBoxDial.DefaultRibbon
Imports Microsoft.Office.Interop
Imports System.Xml

<Runtime.InteropServices.ComVisible(True)> Public Class OutlookRibbons
    Implements IRibbonExtensibility

    Private ReadOnly Property OutlookApp() As Outlook.Application
        Get
            Return ThisAddIn.POutookApplication
        End Get
    End Property
    Public Sub New()
        'dim DefaultRibbon = New DefaultRibbon
    End Sub

#Region "Ribbon Grundlagen für Outlook 2010 bis 2019"
    Private Property RibbonObjekt As IRibbonUI
    Sub Ribbon_Load(ByVal Ribbon As IRibbonUI)
        RibbonObjekt = Ribbon
    End Sub

    Public Function GetCustomUI(ByVal ribbonID As String) As String Implements IRibbonExtensibility.GetCustomUI
        Dim File As String

        Select Case ribbonID
            Case "Microsoft.Outlook.Explorer"
                File = My.Resources.RibbonExplorer
            Case "Microsoft.Outlook.Mail.Read"
                File = My.Resources.RibbonInspectorMailRead
            Case "Microsoft.Outlook.Journal"
                File = My.Resources.RibbonInspectorJournal
            Case "Microsoft.Outlook.Contact"
                File = My.Resources.RibbonInspectorKontakt
            Case "Microsoft.Mso.IMLayerUI"
                File = My.Resources.RibbonIMLayerUI
            Case Else
                File = PDfltStringEmpty
        End Select
        Return File
    End Function

    Public Sub RefreshRibbon()
        If RibbonObjekt Is Nothing Then
            Dim i As Integer
            Do While RibbonObjekt Is Nothing And i.IsLess(100)
                i += 1
                Windows.Forms.Application.DoEvents()
            Loop
        End If
        If RibbonObjekt IsNot Nothing Then RibbonObjekt.Invalidate()

    End Sub
#End Region

#Region "Ribbon Explorer  Office 2010 bis Office 2019" 'Ribbon Explorer
    Public Function GetPressed(ByVal control As IRibbonControl) As Boolean
        Select Case control.Id.Split("_")(0)
            Case "btnAnrMonIO"
                Return ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso ThisAddIn.PAnrufmonitor.Aktiv
            Case Else
                Return False
        End Select
    End Function

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem ToogleButton hinterlegt ist.
    ''' </summary>
    ''' <param name="control">ToogleButton</param>
    ''' <param name="pressed">Zustand des ToogleButtons</param>
    ''' <remarks>Eine reine Weiterleitung auf die Standard-OnAction Funktion</remarks>
    <CodeAnalysis.SuppressMessage("Stil", "IDE0060:Nicht verwendete Parameter entfernen", Justification:="Der Parameter wird für die korrekte Verarbeitung der Ribbons benötigt")>
    Public Sub BtnOnToggleButtonAction(ByVal control As IRibbonControl, ByVal pressed As Boolean)
        BtnOnAction(control)
    End Sub

#End Region 'Ribbon Explorer

#Region "Ribbon Inspector Office 2010 bis Office 2019" ' Ribbon Inspektorfenster

    ''' <summary>
    ''' Gibt zurück, ob das JournalItem, von diesem Addin erstellt wurde. Dazu wird die Kategorie geprüft.
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>True, wenn JournalItem, von diesem Addin erstellt wurde. Ansonsten False</returns>
    Private Function CheckJournalInspector(ByVal control As IRibbonControl) As Outlook.JournalItem
        CheckJournalInspector = Nothing

        Dim olJournal As Outlook.JournalItem = Nothing

        If TypeOf control.Context Is Outlook.Selection Then
            olJournal = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.JournalItem)
        End If

        If TypeOf control.Context Is Outlook.Inspector Then
            olJournal = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.JournalItem)
        End If

        ' Bei Journal nur wenn Kategorien korrekt
        ' Wenn Journal keine Kategorie enthält, dann ist es kein vom Addin erzeugtes JournalItem
        If olJournal.Categories IsNot Nothing AndAlso olJournal.Categories.Contains(String.Join("; ", PDfltJournalDefCategories.ToArray)) Then
            CheckJournalInspector = olJournal
        End If

    End Function

    ''' <summary>
    ''' Gibt zurück, ob das Journal eine gültige Telefonnummer enthält
    ''' </summary>
    ''' <param name="control"></param>
    Public Function EnableBtnJournal(ByVal control As IRibbonControl) As Boolean
        EnableBtnJournal = False

        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)

        If olJournal IsNot Nothing Then
            EnableBtnJournal = Not olJournal.Body.StartsWith(String.Format("{0} {1}", PDfltJournalBodyStart, PDfltStringUnbekannt))
        End If
    End Function

    ''' <summary>
    ''' Gibt das Label des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>"Kontakt Anzeigen", wenn Link im JournalItem zu einem ContactItem führt. Ansonsten "Kontakt Erstellen"</returns>
    ''' <remarks>Funktioniert nur unter Office 2010, da Microsoft die Links aus Journalitems in nachfolgenden Office Versionen entfernt hat.</remarks>
    Private Function SetLabelJournal(ByVal control As IRibbonControl) As String

        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)
        If olJournal IsNot Nothing Then
            With olJournal
                Return If(GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object())) Is Nothing, P_CMB_Kontakt_Erstellen, P_CMB_Kontakt_Anzeigen)
            End With
        End If
        Return P_CMB_Kontakt_Erstellen
    End Function

    ''' <summary>
    ''' Gibt das ScreenTip des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>Den entsprechenden ScreenTip, wenn Link im JournalItem zu einem ContactItem führt. Ansonsten den anderen. Falls Link ins Leere führt, dann wird Fehlermeldung ausgegeben.</returns>
    ''' <remarks>Funktioniert nur unter Office 2010, da Microsoft die Links aus Journalitems in nachfolgenden Office Versionen entfernt hat.</remarks>
    Private Function SetScreenTipJournal(ByVal control As IRibbonControl) As String

        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)
        If olJournal IsNot Nothing Then
            With olJournal
                Return If(GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object())) Is Nothing, P_CMB_Kontakt_Erstellen_ToolTipp, P_CMB_Kontakt_Anzeigen_ToolTipp)
            End With
        End If

        Return P_CMB_Kontakt_Erstellen_ToolTipp
    End Function

    ''' <summary>
    ''' Die Ribbons der Inspectoren sollen nur eingeblendet werden, wenn ein Explorer vorhanden ist.
    ''' </summary>
    ''' <param name="control"></param>
    Public Function ShowInspectorRibbon(ByVal control As IRibbonControl) As Boolean
        ShowInspectorRibbon = False

        ' Einblendenm wenn Explorer vorhanden ist
        ShowInspectorRibbon = (New Outlook.Application).ActiveExplorer IsNot Nothing

        ' Extra Prüfung bei JournalItem
        If TypeOf CType(control.Context, Outlook.Inspector).CurrentItem Is Outlook.JournalItem Then
            ShowInspectorRibbon = CheckJournalInspector(control) IsNot Nothing
        End If
    End Function

#End Region 'Ribbon Inspector

#Region "Ribbon Behandlung für Outlook 2010 bis 2019"

#Region "Ribbon: Label, ScreenTipp, ImageMso, OnAction"
    ''' <summary>
    ''' Ermittelt das Label des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemLabel(ByVal control As IRibbonControl) As String
        Select Case control.Id.Split("_")(0)
            Case "Tab"
                Return PDfltAddin_LangName
            Case "btnDialExpl", "btnDialInsp"
                Return P_CMB_Dial
            Case "btnDirektwahl"
                Return P_CMB_Direktwahl
            Case "CallList"
                Return P_CMB_WWDH
            Case "RingList"
                Return P_CMB_CallBack
            Case "VIPList"
                Return P_CMB_VIP
            Case "btnAnrMonIO"
                Return P_CMB_AnrMon
            Case "dynListDel"
                Return P_CMB_ClearList
            Case "btnAnrMonRestart"
                Return P_CMB_AnrMonNeuStart
            Case "btnAnrMonShow"
                Return P_CMB_AnrMonAnzeigen
            Case "btnAnrMonJI"
                Return P_CMB_Journal
            Case "Einstellungen"
                Return P_CMB_Setup
            Case "cbtnDial", "rbtnDial" ' ContextMenu Dial
                Return P_CMB_ContextMenueItemCall
            Case "ctbtnVIP" ' ContextMenu VIP
                Return P_CMB_ContextMenueItemVIP
            Case "cbtnUpload" ' ContextMenu Upload
                Return P_CMB_ContextMenueItemUpload
            Case "btnRWS"
                Return P_RWS_Name
            Case "btnAddContact", "cbtnAddContact"
                Return SetLabelJournal(control)
            Case "btnNote"
                Return P_CMB_Insp_Note
            Case "tbtnVIP"
                Return P_CMB_Insp_VIP
            Case "btnUpload", "cdMUpload", "MUpload"
                Return P_CMB_Insp_Upload
            Case Else
                LogFile("GetItemLabel: Kann control.Id " & control.Id & " nicht auswerten.")
                Return PDfltStrErrorMinusOne
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemScreenTipp(ByVal control As IRibbonControl) As String
        Select Case control.Id.Split("_").First
            Case "btnDialExpl", "btnDialInsp", "rbtnDial"
                Return P_CMB_Dial_ToolTipp
            Case "btnDirektwahl"
                Return P_CMB_Direktwahl_ToolTipp
            Case "CallList"
                Return P_CMB_WWDH_ToolTipp
            Case "RingList"
                Return P_CMB_CallBack_ToolTipp
            Case "VIPList"
                Return P_CMB_VIP_ToolTipp
            Case "btnAnrMonIO"
                Return P_CMB_AnrMon_ToolTipp
            Case "btnAnrMonRestart"
                Return P_CMB_AnrMonNeuStart_ToolTipp
            Case "btnAnrMonShow"
                Return P_CMB_AnrMonAnzeigen_ToolTipp()
            Case "btnAnrMonJI"
                Return P_CMB_Journal_ToolTipp
            Case "Einstellungen"
                Return P_CMB_Setup_ToolTipp
            Case "btnRWS"
                Return P_CMB_Insp_RWS_ToolTipp
            Case "btnAddContact"
                Return SetScreenTipJournal(control)
            Case "btnNote"
                Return P_CMB_Insp_Note_ToolTipp
            Case "TbtnVIP"
                Return If(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem).IsVIP, P_CMB_VIP_Entfernen_ToolTipp, P_CMB_VIP_Hinzufügen_ToolTipp)
            Case "btnUpload"
                Return P_CMB_Insp_UploadKontakt_ToolTipp()
            Case Else
                LogFile("GetItemScreenTipp: Kann control.Id " & control.Id & " nicht auswerten.")
                Return PDfltStrErrorMinusOne
        End Select
    End Function

    ''' <summary>
    ''' Ermittelt das Icon (ImageMSO) des Ribbon-Objektes ausgehend von der Ribbon-id
    ''' </summary>
    ''' <param name="control">Die id des Ribbon Controls</param>
    ''' <returns>Bezeichnung des ImageMso</returns>
    ''' <remarks>http://soltechs.net/customui/</remarks>
    Public Function GetItemImageMso(ByVal control As IRibbonControl) As String

        Select Case control.Id.Split("_").First
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
                'Return "PersonaStatusBusy"
                Return If(ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso ThisAddIn.PAnrufmonitor.Aktiv, "PersonaStatusOnline", "PersonaStatusOffline")
            Case "btnAnrMonRestart"
                Return "RecurrenceEdit"
            Case "btnAnrMonShow"
                Return "ClipArtInsert"
            Case "btnAnrMonJI"
                Return "NewJournalEntry"
            Case "btnUpload", "MUpload"
                Return "DistributionListAddNewMember"
            Case "btnRWS" ' Inspector
                Return "CheckNames"
            Case "btnAddContact", "cbtnAddContact" ' Inspector, ContextMenü
                Return "RecordsSaveAsOutlookContact"
            Case "btnNote" ' Inspector
                Return "ShowNotesPage"
            Case "CallList", "RingList", "VIPList"
                Return PDfltStringEmpty
            Case Else
                LogFile("GetItemImageMso: Kann control.Id " & control.Id & " nicht auswerten.")
                Return "TraceError"
        End Select

    End Function

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem Button hinterlegt ist.
    ''' </summary>
    ''' <param name="control">Die id des Ribbon Controls</param>
    Public Sub BtnOnAction(ByVal control As IRibbonControl)
        Select Case control.Id.Split("_").First
            Case "btnDialExpl"
                OnAction(TaskToDo.DialExplorer)
            Case "rbtnDial"

            Case "btnDirektwahl"
                OnAction(TaskToDo.DialDirekt)
            Case PDfltNameListRING, PDfltNameListCALL, PDfltNameListVIP
                OnActionListen(control)
            Case "dynListDel"
                ClearInListe(control)
            Case "btnAnrMonIO"
                OnAction(TaskToDo.AnrMonAnAus)
            Case "btnAnrMonRestart"
                OnAction(TaskToDo.RestartAnrMon)
            Case "btnAnrMonShow"
                OnAction(TaskToDo.ShowAnrMon)
            Case "btnAnrMonJI"
                OnAction(TaskToDo.OpenJournalimport)
            Case "Einstellungen"
                OnAction(TaskToDo.OpenConfig)
            Case "cbtnUpload"  ' Kontextmenü

            Case "cdMUpload"  ' Kontext + Telefonbuchauswahl

            Case "cbtnAddContact"
                OnAction(TaskToDo.CreateContact)
        End Select
    End Sub


#End Region

#Region "Explorer Button Click"
    ''' <summary>
    ''' Mögliche Anwendungen, die durch den Klick auf ein Button/Ribbon ausgelöst werden können.
    ''' Warum, die Englisch sind? Keine Ahnung.
    ''' </summary>
    Private Enum TaskToDo
        OpenConfig          ' Explorer: Einstellung Öffnen
        OpenJournalimport   ' Explorer: Journalimport öffnen
        ShowAnrMon          ' Explorer: Letzten Anrufer anzeigen
        RestartAnrMon       ' Explorer: Anrufmonitor neu starten
        AnrMonAnAus         ' Explorer: Anrufmonitor Starten/Stoppen
        DialExplorer        ' Explorer: Klassischen Wähldialog über das ausgewählte Objekt öffnen
        DialDirekt          ' Explorer: Direktwahl öffnen
        DialInspector       ' Inspector: Wähldialog öffnen 
        CreateContact       ' Inspector: Journal, Kontakt erstellen
        StartRWS            ' Inspector: Rückwärtssuche starten
    End Enum

    ''' <summary>
    ''' Steuert die aufzurufende Funktion anhand der Übergebenen <c>Aufgabe</c>
    ''' </summary>
    ''' <param name="Aufgabe">Übergabe Wert, der bestimmt, was getan werden soll.</param>
    Private Sub OnAction(ByVal Aufgabe As TaskToDo)
        Select Case Aufgabe
            Case TaskToDo.OpenConfig ' Einstellungsdialog
                Dim FormConfig As New FormCfg
                FormConfig.ShowDialog()
            Case TaskToDo.ShowAnrMon
                Dim PopUpAnrMon As New Popup

                If XMLData.PTelefonie.RINGListe.Einträge.Count.IsNotZero Then
                    PopUpAnrMon.AnrMonEinblenden(XMLData.PTelefonie.RINGListe.Einträge.Item(0))
                Else
                    PopUpAnrMon.AnrMonEinblenden(Nothing)
                End If

            Case TaskToDo.DialDirekt
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(Nothing, True)
            Case TaskToDo.DialExplorer
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(OutlookApp.ActiveExplorer.Selection, False)
            Case TaskToDo.OpenJournalimport
                Dim AnrListImport As New FormAnrList
                AnrListImport.Show()
            Case TaskToDo.RestartAnrMon

            Case TaskToDo.AnrMonAnAus
                If ThisAddIn.PAnrufmonitor Is Nothing Then ThisAddIn.PAnrufmonitor = New Anrufmonitor
                ThisAddIn.PAnrufmonitor.StartStopAnrMon()

        End Select
    End Sub

    Private Sub OnAction(ByVal Aufgabe As TaskToDo, ByVal OutlookInspector As Outlook.Inspector)
        Select Case Aufgabe
            Case TaskToDo.DialInspector
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(OutlookInspector)
            Case TaskToDo.CreateContact
                ZeigeKontaktAusInspector(OutlookInspector)
            Case TaskToDo.StartRWS
                If TypeOf OutlookInspector.CurrentItem Is Outlook.JournalItem Then
                    StartJournalRWS(CType(OutlookInspector.CurrentItem, Outlook.JournalItem))
                End If
                If TypeOf OutlookInspector.CurrentItem Is Outlook.ContactItem Then

                End If
        End Select
    End Sub

    Private Sub OnAction(ByVal Aufgabe As TaskToDo, ByVal OutlookSelection As Outlook.Selection)
        Select Case Aufgabe
            Case TaskToDo.DialInspector

            Case TaskToDo.CreateContact
                ZeigeKontaktAusSelection(OutlookSelection)
        End Select
    End Sub
    ''' <summary>
    ''' Behandelt das Ereignis, welches beim Klick auf die PopUp-Items ausgelöst wird.
    ''' Funktion würd für alle Office Versionen benötigt.
    ''' </summary>
    ''' <param name="control"></param>
    Private Sub OnActionListen(ByVal control As IRibbonControl)
        Dim tmpTelefonat As Telefonat
        Dim tmpVIPEintrag As VIPEntry
        If control.Tag.AreEqual(PDfltNameListVIP) Then
            tmpVIPEintrag = XMLData.PTelefonie.VIPListe.Einträge.Item(control.Id.Split("_")(1).ToInt)
            If tmpVIPEintrag IsNot Nothing Then
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(tmpVIPEintrag)
            End If
        Else
            tmpTelefonat = If(control.Tag.AreEqual(PDfltNameListCALL), XMLData.PTelefonie.CALLListe.Einträge, XMLData.PTelefonie.RINGListe.Einträge).Item(control.Id.Split("_")(1).ToInt)
            ' Ermittle das Telefonat aus der Liste
            If tmpTelefonat IsNot Nothing Then
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(tmpTelefonat)
            End If
        End If
    End Sub
    Private Sub ClearInListe(ByVal control As IRibbonControl)

        Select Case control.Tag
            Case PDfltNameListCALL
                XMLData.PTelefonie.CALLListe.Einträge.Clear()
            Case PDfltNameListRING
                XMLData.PTelefonie.RINGListe.Einträge.Clear()
            Case PDfltNameListVIP
                XMLData.PTelefonie.VIPListe.Einträge.Clear()
        End Select

        RefreshRibbon()
    End Sub

#End Region

#Region "Inspector Button Click"
    Public Sub BtnOnActionI(ByVal control As IRibbonControl)
        Dim oInsp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)
        If oInsp IsNot Nothing Then
            Select Case control.Id.Split("_").First
                Case "btnDialInsp"
                    OnAction(TaskToDo.DialInspector, oInsp)
                Case "btnUpload"   ' Inspector

                Case "MUpload"  ' Inspector + Telefonbuchauswahl

                Case "btnRWS" ' Rückwärtssuche
                    OnAction(TaskToDo.StartRWS, oInsp)
                Case "btnNote"

                Case "btnAddContact"
                    OnAction(TaskToDo.CreateContact, oInsp)
            End Select
        End If
    End Sub
#End Region

#Region "ContextMenü Button Click"
    Public Sub BtnOnActionCM(ByVal control As IRibbonControl)
        Dim oSel As Outlook.Selection = CType(control.Context, Outlook.Selection)
        If oSel IsNot Nothing Then
            Select Case control.Id.Split("_").First
                Case "cbtnDial"
                    OnAction(TaskToDo.DialExplorer)
                Case "cbtnAddContact"
                    OnAction(TaskToDo.CreateContact, oSel)
            End Select
        End If
    End Sub
#End Region

    Public Function GetVisibleUploadFKT() As Boolean
        Return False
    End Function

    Public Function GetVisibleRWS() As Boolean
        Return False
    End Function

#Region "DynamicMenu"


    Public Function DynMenuEnabled(ByVal control As IRibbonControl) As Boolean

        Select Case Left(control.Id, Len(control.Id) - 2)
            Case PDfltNameListCALL
                Return XMLData.PTelefonie.CALLListe IsNot Nothing AndAlso XMLData.PTelefonie.CALLListe.Einträge.Any
            Case PDfltNameListRING
                Return XMLData.PTelefonie.RINGListe IsNot Nothing AndAlso XMLData.PTelefonie.RINGListe.Einträge.Any
            Case PDfltNameListVIP
                Return XMLData.PTelefonie.VIPListe IsNot Nothing AndAlso XMLData.PTelefonie.VIPListe.Einträge.Any
            Case Else
                Return False
        End Select

    End Function

    Private Overloads Function CreateDynMenuButton(ByVal xDoc As XmlDocument, ByVal ID As String) As XmlElement
        Dim XButton As XmlElement
        Dim XAttribute As XmlAttribute
        XButton = xDoc.CreateElement("button", "http://schemas.microsoft.com/office/2009/07/customui")

        XAttribute = xDoc.CreateAttribute("id")
        XAttribute.Value = ID
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("tag")
        XAttribute.Value = ID.Split("_").Last
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("getLabel")
        XAttribute.Value = "GetItemLabel"
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("onAction")
        XAttribute.Value = "BtnOnAction"
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("getImage")
        XAttribute.Value = "GetItemImageMso"
        XButton.Attributes.Append(XAttribute)

        Return XButton
    End Function

    Private Function CreateDynMenuSeperator(ByVal xDoc As XmlDocument) As XmlElement
        Dim XSeperator As XmlElement
        Dim XAttribute As XmlAttribute

        XSeperator = xDoc.CreateElement("menuSeparator", "http://schemas.microsoft.com/office/2009/07/customui")

        XAttribute = xDoc.CreateAttribute("id")
        XAttribute.Value = "separator"
        XSeperator.Attributes.Append(XAttribute)

        Return XSeperator
    End Function

    ''' <summary>
    ''' Generiert ein XML-String, der indas DynamicMenu geladen wird
    ''' </summary>
    ''' <param name="control">Das Ribbon-Control, für das das das DynamicMenu verwendet werden soll.</param>
    ''' <returns></returns>
    Public Function FillDynamicMenu(ByVal control As IRibbonControl) As String

        Dim ListName As String = Left(control.Id, Len(control.Id) - 2)
        Dim ListofTelefonate As List(Of Telefonat)
        Dim XDynaMenu As New XmlDocument

        With XDynaMenu
            ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
            .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", "http://schemas.microsoft.com/office/2009/07/customui")))

            ' Füge den Löschbutton und einen Seperator hinzu
            .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, String.Format("dynListDel_{0}", ListName)))
            .DocumentElement.AppendChild(CreateDynMenuSeperator(XDynaMenu))

            If ListName.AreEqual(PDfltNameListCALL) Or ListName.AreEqual(PDfltNameListRING) Then
                If ListName.AreEqual(PDfltNameListCALL) Then
                    ListofTelefonate = XMLData.PTelefonie.CALLListe.Einträge
                Else
                    ListofTelefonate = XMLData.PTelefonie.RINGListe.Einträge
                End If

                For Each TelFt As Telefonat In ListofTelefonate
                    .DocumentElement.AppendChild(TelFt.CreateDynMenuButton(XDynaMenu, ListofTelefonate.IndexOf(TelFt), ListName))
                Next
            ElseIf ListName.AreEqual(PDfltNameListVIP) Then
                For Each VIP As VIPEntry In XMLData.PTelefonie.VIPListe.Einträge
                    .DocumentElement.AppendChild(VIP.CreateDynMenuButton(XDynaMenu, XMLData.PTelefonie.VIPListe.Einträge.IndexOf(VIP), ListName))
                Next
            End If
        End With

        Return XDynaMenu.InnerXml
    End Function
#End Region

#Region "VIP-Ribbon"
    <CodeAnalysis.SuppressMessage("Stil", "IDE0060:Nicht verwendete Parameter entfernen", Justification:="Der Parameter isPressed wird für die korrekte Verarbeitung der Ribbons benötigt")>
    Public Sub TBtnOnAction(ByVal control As IRibbonControl, ByRef isPressed As Boolean)

        Dim oKontakt As Outlook.ContactItem = Nothing

        Select Case control.Id
            Case "ctbtnVIP" ' Kontext Menu
                oKontakt = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
            Case "tbtnVIP_C" ' Kontaktinspector 
                oKontakt = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)
        End Select

        If Not oKontakt Is Nothing Then

            If oKontakt.IsVIP Then
                oKontakt.RemoveVIP
            Else
                oKontakt.AddVIP
            End If
            oKontakt.ReleaseComObject
        End If

        ' Fehler unter Office 2007
        RibbonObjekt.Invalidate()
    End Sub

    Public Function CtBtnPressedVIP(ByVal control As IRibbonControl) As Boolean
        Return CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem).IsVIP
    End Function

    Public Function TBtnPressedVIP(ByVal control As IRibbonControl) As Boolean
        TBtnPressedVIP = False

        Dim Insp As Outlook.Inspector = CType(control.Context, Outlook.Inspector)

        If TypeOf Insp.CurrentItem Is Outlook.ContactItem Then
            Dim olContact As Outlook.ContactItem = CType(Insp.CurrentItem, Outlook.ContactItem)

            TBtnPressedVIP = IsVIP(olContact)

            olContact.ReleaseComObject
        End If
    End Function

#End Region

#Region "Explorer Button Click"
    '    ''' <summary>
    '    ''' Mögliche Anwendungen, die durch den klick auf ein Button/Ribbon ausgelöst werden können.
    '    ''' Warum, die Englisch sind? Keine Ahnung.
    '    ''' </summary>
    '    Friend Enum TaskToDo
    '        OpenConfig          ' Explorer: Einstellung Öffnen
    '        OpenJournalimport   ' Explorer: Journalimport öffnen
    '        ShowAnrMon          ' Explorer: Letzten Anrufer anzeigen
    '        RestartAnrMon       ' Explorer: Anrufmonitor neu starten
    '        DialExplorer        ' Explorer: Klassischen Wähldialog über das ausgewählte Objekt öffnen
    '        DialDirect          ' Explorer: Direktwahl öffnen
    '        DialInspector       ' Inspector: Wähldialog öffnen 
    '        CreateContact       ' Inspector: Journal, Kontakt erstellen
    '    End Enum

    '    ''' <summary>
    '    ''' Steuert die aufzurufende Funktion anhand der Übergebenen <c>Aufgabe</c>
    '    ''' </summary>
    '    ''' <param name="Aufgabe">Übergabe Wert, der bestimmt, was getan werden soll.</param>
    '    Friend Sub OnAction(ByVal Aufgabe As TaskToDo)
    '        Select Case Aufgabe
    '            Case TaskToDo.DialDirect
    '                P_CallClient.Wählbox(Nothing, DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, True)
    '            Case TaskToDo.DialExplorer
    '                If P_OlInterface.OutlookApplication IsNot Nothing Then
    '                    P_CallClient.WählboxStart(P_OlInterface.OutlookApplication.ActiveExplorer.Selection)
    '                End If
    '            Case TaskToDo.OpenConfig
    '                P_Config.ShowDialog()
    '            Case TaskToDo.OpenJournalimport
    '                If Not P_AnrList Is Nothing Then
    '                    P_AnrList = New formImportAnrList(P_FritzBox, P_AnrufMonitor, C_hf, C_DP, C_XML)
    '                End If
    '                P_AnrList.StartAuswertung(True)
    '            Case TaskToDo.RestartAnrMon
    '                P_AnrufMonitor.Restart(False)
    '            Case TaskToDo.ShowAnrMon
    '                P_PopUp.AnrMonEinblenden(P_AnrufMonitor.LetzterAnrufer)
    '            Case TaskToDo.DialInspector
    '                P_CallClient.WählenAusInspector()
    '            Case TaskToDo.CreateContact
    '                C_KF.ZeigeKontaktAusJournal()
    '        End Select
    '    End Sub

    '    'Private Function GetSmtpAddress(ByVal card As Office.IMsoContactCard) As String
    '    '    If card.AddressType = Office.MsoContactCardAddressType.msoContactCardAddressTypeOutlook Then
    '    '        Dim host As Outlook.Application = Globals.ThisAddIn.Application
    '    '        Dim ae As Outlook.AddressEntry = host.Session.GetAddressEntryFromID(card.Address)

    '    '        Select Case ae.AddressEntryUserType
    '    '            Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
    '    '                Dim ex As Outlook.ExchangeUser = ae.GetExchangeUser()
    '    '                Return ex.PrimarySmtpAddress
    '    '            Case Outlook.OlAddressEntryUserType.olOutlookContactAddressEntry
    '    '                Return ae.Address
    '    '            Case Else
    '    '                Throw New Exception("Valid address entry not found.")
    '    '        End Select
    '    '    Else
    '    '        Return card.Address
    '    '    End If
    '    'End Function
#End Region
#End Region
End Class