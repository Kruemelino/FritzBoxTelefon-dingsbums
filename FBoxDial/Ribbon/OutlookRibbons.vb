Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop
Imports System.Xml

<Runtime.InteropServices.ComVisible(True)> Public Class OutlookRibbons
    Implements IRibbonExtensibility
    Private Property NLogger As NLog.Logger = LogManager.GetCurrentClassLogger

    Private Property DfltWerte As DefaultRibbonWerte

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
        If RibbonObjekt IsNot Nothing Then
            Try
                RibbonObjekt.Invalidate()
            Catch ex As Exception
                NLogger.Error(ex)
            End Try

        End If
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
    Public Function GetLabelJournal(ByVal control As IRibbonControl) As String
        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)

        If olJournal IsNot Nothing Then
            With olJournal
                GetLabelJournal = If(GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object())) Is Nothing, GetRibbonWert(control.Id, "Label"), GetRibbonWert("Anzeigen" & control.Id, "Label"))
            End With
        Else
            GetLabelJournal = GetRibbonWert(control.Id, "Label")
        End If
        olJournal.ReleaseComObject
    End Function

    ''' <summary>
    ''' Gibt das ScreenTip des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>Den entsprechenden ScreenTip, wenn Link im JournalItem zu einem ContactItem führt. Ansonsten den anderen. Falls Link ins Leere führt, dann wird Fehlermeldung ausgegeben.</returns>
    ''' <remarks>Funktioniert nur unter Office 2010, da Microsoft die Links aus Journalitems in nachfolgenden Office Versionen entfernt hat.</remarks>
    Public Function GetScreenTipJournal(ByVal control As IRibbonControl) As String
        Dim olJournal As Outlook.JournalItem = CheckJournalInspector(control)

        If olJournal IsNot Nothing Then
            With olJournal
                GetScreenTipJournal = If(GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object())) Is Nothing, GetRibbonWert(control.Id, "ScreenTipp"), GetRibbonWert("Anzeigen" & control.Id, "ScreenTipp"))
            End With
        Else
            GetScreenTipJournal = GetRibbonWert(control.Id, "ScreenTipp")
        End If
        olJournal.ReleaseComObject
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

    Public Function GetScreenTipVIP(ByVal control As IRibbonControl) As String
        Return If(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem).IsVIP, GetRibbonWert(control.Id & "Remove", "ScreenTipp"), GetRibbonWert(control.Id & "Add", "ScreenTipp"))
    End Function

    <CodeAnalysis.SuppressMessage("Style", "IDE0060:Nicht verwendete Parameter entfernen", Justification:="Parameter wird benötigt, da ansonsten Ribbon nicht korrekt verarbeitet wird.")>
    Public Function GetItemImageMsoAnrMon(ByVal control As IRibbonControl) As String
        Return If(ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso ThisAddIn.PAnrufmonitor.Aktiv, "PersonaStatusOnline", "PersonaStatusOffline")
    End Function


#End Region 'Ribbon Inspector

#Region "Ribbon Behandlung für Outlook 2010 bis 2019"

#Region "Ribbon: Label, ScreenTipp, ImageMso, OnAction"

    Private Function GetRibbonWert(ByVal Key As String, ByVal Typ As String) As String
        Dim tmpPropertyInfo As Reflection.PropertyInfo

        tmpPropertyInfo = Array.Find(DfltWerte.GetType.GetProperties, Function(PropertyInfo As Reflection.PropertyInfo) PropertyInfo.Name.AreEqual(String.Format("P{0}{1}", Typ, Key.Split("_")(0))))

        If tmpPropertyInfo IsNot Nothing Then
            Return tmpPropertyInfo.GetValue(DfltWerte).ToString()
        Else
            NLogger.Warn("Kann control.Id {0} für {1} nicht auswerten.", Key, Typ)
            If Typ.AreEqual("ImageMso") Then
                Return "TraceError"
            Else
                Return PDfltStrErrorMinusOne
            End If
        End If
    End Function


    ''' <summary>
    ''' Ermittelt das Label des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemLabel(ByVal control As IRibbonControl) As String
        Return GetRibbonWert(control.Id, "Label")
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemScreenTipp(ByVal control As IRibbonControl) As String
        Return GetRibbonWert(control.Id, "ScreenTipp")
    End Function

    ''' <summary>
    ''' Ermittelt das Icon (ImageMSO) des Ribbon-Objektes ausgehend von der Ribbon-id
    ''' </summary>
    ''' <param name="control">Die id des Ribbon Controls</param>
    ''' <returns>Bezeichnung des ImageMso</returns>
    Public Function GetItemImageMso(ByVal control As IRibbonControl) As String
        Return GetRibbonWert(control.Id, "ImageMso")
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
                OnAction(TaskToDo.DialIMLayer, control)
            Case "btnDirektwahl"
                OnAction(TaskToDo.DialDirekt)
            Case PDfltNameListRING, PDfltNameListCALL, PDfltNameListVIP
                OnActionListen(control)
            Case "dynListDel"
                ClearInListe(control)
            Case "btnAnrMonIO"
                OnAction(TaskToDo.AnrMonAnAus)
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
            Case "btnFBTelBch"
                OnAction(TaskToDo.FBoxTelBücher)
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
        AnrMonAnAus         ' Explorer: Anrufmonitor Starten/Stoppen
        DialExplorer        ' Explorer: Klassischen Wähldialog über das ausgewählte Objekt öffnen
        DialDirekt          ' Explorer: Direktwahl öffnen
        FBoxTelBücher       ' Explorer: Fritz!Box Telefonbücher
        DialInspector       ' Inspector: Wähldialog öffnen 
        DialIMLayer         ' IMLayer: Wähldialog öffnen 
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
                Using FormConfig As New FormCfg
                    FormConfig.ShowDialog()
                End Using

            Case TaskToDo.ShowAnrMon
                Dim PopUpAnrMon As New Popup

                If XMLData.PTelefonie.RINGListe.Einträge.Count.IsNotZero Then
                    PopUpAnrMon.AnrMonEinblenden(XMLData.PTelefonie.RINGListe.Einträge.Item(0))
                Else
                    PopUpAnrMon.AnrMonEinblenden(Nothing)
                End If

            Case TaskToDo.DialDirekt
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart()
            Case TaskToDo.DialExplorer
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(ThisAddIn.POutookApplication.ActiveExplorer.Selection)
            Case TaskToDo.OpenJournalimport
                Dim AnrListImport As New FormAnrList
                AnrListImport.Show()
            Case TaskToDo.AnrMonAnAus
                If ThisAddIn.PAnrufmonitor Is Nothing Then ThisAddIn.PAnrufmonitor = New Anrufmonitor
                ThisAddIn.PAnrufmonitor.StartStopAnrMon()
            Case TaskToDo.FBoxTelBücher
                Dim FBoxTeleBuch As New FormTelefonbücher
                FBoxTeleBuch.Show()
        End Select
    End Sub
    ''' <summary>
    ''' Steuert die aufzurufende Funktion aus Inspektorfenstern anhand der Übergebenen <c>Aufgabe</c>.
    ''' </summary>
    ''' <param name="Aufgabe"></param>
    ''' <param name="OutlookInspector"></param>
    Private Sub OnAction(ByVal Aufgabe As TaskToDo, ByVal OutlookInspector As Outlook.Inspector)
        Select Case Aufgabe
            Case TaskToDo.DialInspector
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(OutlookInspector)
            Case TaskToDo.CreateContact
                ZeigeKontaktAusInspector(OutlookInspector)
            Case TaskToDo.StartRWS
                ' Journal
                If TypeOf OutlookInspector.CurrentItem Is Outlook.JournalItem Then
                    StartJournalRWS(CType(OutlookInspector.CurrentItem, Outlook.JournalItem))
                End If
                ' Kontakt
                If TypeOf OutlookInspector.CurrentItem Is Outlook.ContactItem Then

                End If
        End Select
    End Sub

    Private Sub OnAction(ByVal Aufgabe As TaskToDo, ByVal OutlookSelection As Outlook.Selection)
        Select Case Aufgabe
            Case TaskToDo.CreateContact
                ZeigeKontaktAusSelection(OutlookSelection)
        End Select
    End Sub

    Private Sub OnAction(ByVal Aufgabe As TaskToDo, ByVal control As IRibbonControl)
        Select Case Aufgabe
            Case TaskToDo.DialIMLayer
                Dim card As IMsoContactCard = TryCast(control.Context, IMsoContactCard)
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(TryCast(control.Context, IMsoContactCard))
        End Select
    End Sub
    ''' <summary>
    ''' Behandelt das Ereignis, welches beim Klick auf die PopUp-Items ausgelöst wird.
    ''' Funktion würd für alle Office Versionen benötigt.
    ''' </summary>
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
        If XMLData IsNot Nothing Then
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
        Else
            Return False
        End If
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

#End Region

End Class