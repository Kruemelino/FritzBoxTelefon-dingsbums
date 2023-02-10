Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop
Imports FBoxDial.RibbonData
<Runtime.InteropServices.ComVisible(True)> Public Class OutlookRibbons
    Implements IRibbonExtensibility

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Ribbon Grundlagen für Outlook (ab 2010)"
    Private Property RibbonObjekt As IRibbonUI
    Sub Ribbon_Load(Ribbon As IRibbonUI)
        RibbonObjekt = Ribbon
    End Sub

    ''' <summary>
    ''' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
    ''' </summary>
    Friend Sub Invalidate()
        RibbonObjekt?.Invalidate()
    End Sub

    ''' <summary>
    ''' Lädt das XML-Markup aus einer XML-Anpassungsdatei oder aus XML-Markup, das in die Prozedur eingebettet ist, mit der die Menüband-Benutzeroberfläche angepasst wird.
    ''' </summary>
    Public Function GetCustomUI(ribbonID As String) As String Implements IRibbonExtensibility.GetCustomUI

        Select Case ribbonID
            Case "Microsoft.Outlook.Explorer"
                GetCustomUI = My.Resources.RibbonExplorer
            Case "Microsoft.Outlook.Mail.Read"
                GetCustomUI = My.Resources.RibbonInspectorMailRead
            Case "Microsoft.Outlook.Journal"
                GetCustomUI = My.Resources.RibbonInspectorJournal
            Case "Microsoft.Outlook.Appointment"
                GetCustomUI = My.Resources.RibbonInspectorAppointment
            Case "Microsoft.Outlook.Contact"
                GetCustomUI = My.Resources.RibbonInspectorKontakt
            Case "Microsoft.Mso.IMLayerUI"
                GetCustomUI = My.Resources.RibbonIMLayerUI
            Case Else
                GetCustomUI = String.Empty
        End Select

        If GetCustomUI.IsNotStringNothingOrEmpty Then NLogger.Trace($"RibbonXML für {ribbonID}:{vbCrLf}{GetCustomUI}")
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
                Invalidate()
            Catch ex As Exception
                NLogger.Error(ex)
            End Try

        End If
    End Sub

#End Region

#Region "Ribbon Office Inspektor" ' Ribbon Inspektorfenster
    ''' <summary>
    ''' Funktion ermittelt anhand des Controls und dessen Context das Outlook-Item.
    ''' </summary>
    ''' <param name="control">Das Control, von dem das Outlook-Item ermittelt werden soll.</param>
    ''' <returns>Das Outlook-Item</returns>
    Private Function GetOutlookItem(Of T)(control As IRibbonControl) As T
        Select Case True
            Case TypeOf control.Context Is Outlook.Selection
                Return CType(CType(control.Context, Outlook.Selection).Item(1), T)

            Case TypeOf control.Context Is Outlook.Inspector
                Return CType(CType(control.Context, Outlook.Inspector).CurrentItem, T)

            Case Else
                Return Nothing

        End Select
    End Function

    ''' <summary>
    ''' Gibt das Label des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>"Kontakt Anzeigen", wenn Link im JournalItem zu einem ContactItem führt. Ansonsten "Kontakt Erstellen"</returns>
    Public Function GetItemLabelJournal(control As IRibbonControl) As String
        Return RibbonContent(GetOutlookItem(Of Outlook.JournalItem)(control), control.Id, Typ.Label)
    End Function

    ''' <summary>
    ''' Gibt das ScreenTip des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    ''' <returns>Den entsprechenden ScreenTip, wenn Link im JournalItem zu einem ContactItem führt.</returns>
    Public Function GetItemScreenTipJournal(control As IRibbonControl) As String
        Return RibbonContent(GetOutlookItem(Of Outlook.JournalItem)(control), control.Id, Typ.ScreenTipp)
    End Function

    ''' <summary>
    ''' Gibt das ImageMso des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    ''' <returns>Den entsprechenden ImageMso, wenn Link im JournalItem zu einem ContactItem führt. </returns>
    Public Function GetItemImageMsoJournal(control As IRibbonControl) As String
        Return RibbonContent(GetOutlookItem(Of Outlook.JournalItem)(control), control.Id, Typ.ImageMso)
    End Function

    ''' <summary>
    ''' Gibt das Label des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das Ribbon Control</param>
    ''' <returns>"Kontakt Anzeigen", wenn Link im JournalItem zu einem ContactItem führt. Ansonsten "Kontakt Erstellen"</returns>
    Public Function GetItemLabelAppointment(control As IRibbonControl) As String
        Return RibbonContent(GetOutlookItem(Of Outlook.AppointmentItem)(control), control.Id, Typ.Label)
    End Function

    ''' <summary>
    ''' Gibt das ScreenTip des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    ''' <returns>Den entsprechenden ScreenTip, wenn Link im JournalItem zu einem ContactItem führt.</returns>
    Public Function GetItemScreenTipAppointment(control As IRibbonControl) As String
        Return RibbonContent(GetOutlookItem(Of Outlook.AppointmentItem)(control), control.Id, Typ.ScreenTipp)
    End Function

    ''' <summary>
    ''' Gibt das ImageMso des Buttons "Kontakt Erstellen" bzw. "Kontakt Anzeigen" zurück. 
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    ''' <returns>Den entsprechenden ImageMso, wenn Link im JournalItem zu einem ContactItem führt. </returns>
    Public Function GetItemImageMsoAppointment(control As IRibbonControl) As String
        Return RibbonContent(GetOutlookItem(Of Outlook.AppointmentItem)(control), control.Id, Typ.ImageMso)
    End Function

    ''' <summary>
    ''' Gibt zurück, ob das Wählen möglich ist.
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    ''' <returns>Boolean</returns>
    Public Function DialEnabled(control As IRibbonControl) As Boolean
        Select Case True

            Case TypeOf control.Context Is Outlook.Selection

                Return EnableDial(CType(control.Context, Outlook.Selection))

            Case TypeOf control.Context Is Outlook.Explorer

                Return EnableDial(CType(control.Context, Outlook.Explorer))

            Case TypeOf control.Context Is Outlook.Inspector

                Return EnableDial(CType(control.Context, Outlook.Inspector))

            Case TypeOf control.Context Is IMsoContactCard

                Return EnableDial(CType(control.Context, IMsoContactCard))

            Case Else

                Return False

        End Select
        Return False
    End Function

    Public Function CallPaneEnabled(control As IRibbonControl) As Boolean
        If XMLData.POptionen.CBShowMissedCallPane Then
            With Globals.ThisAddIn.ExplorerWrappers(CType(control.Context, Outlook.Explorer))
                Return Not .PaneVisible AndAlso .PaneItemsAny
            End With
        Else
            Return False
        End If
    End Function

    Public Function GroupVisible(control As IRibbonControl) As Boolean
        Return VisibilityGroup(control.Context)
    End Function

    ''' <summary>
    ''' Die Ribbons der Inspectoren sollen nur eingeblendet werden, wenn ein Explorer vorhanden ist.
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    Public Function ShowInspectorRibbon(control As IRibbonControl) As Boolean
        Return InspectorGroupVisible(CType(control.Context, Outlook.Inspector))
    End Function

    Public Function GetItemScreenTipVIP(control As IRibbonControl) As String
        Return VIPRibbonContent(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem), control.Id, Typ.ScreenTipp)
    End Function
    Public Function GetItemImageMsoVIP(control As IRibbonControl) As String
        Dim oKontakt As Outlook.ContactItem = Nothing

        Select Case True
            Case TypeOf control.Context Is Outlook.Selection
                With CType(control.Context, Outlook.Selection)
                    If TypeOf .Item(1) Is Outlook.ContactItem Then
                        oKontakt = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
                    End If
                End With

            Case TypeOf control.Context Is Outlook.Inspector
                oKontakt = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)

        End Select

        Return VIPRibbonContent(oKontakt, control.Id, Typ.ImageMso)

        ReleaseComObject(oKontakt)
    End Function

    ''' <summary>
    ''' Gibt das ImageMso zurück, entsprechend dem Zustand des Anrufmonitors
    ''' </summary>
    ''' <param name="control">Das zugehörige Ribbon Control.</param>
    ''' <returns>ImageMso</returns>
    Public Function GetItemImageMsoAnrMon(control As IRibbonControl) As String
        Return AnrMonRibbonContent(control.Id, Typ.ImageMso)
    End Function

    Public Function GetVisibleContextMenuMultipleItems(control As IRibbonControl) As Boolean

        If TypeOf control.Context Is Outlook.Selection Then

            With CType(control.Context, Outlook.Selection)
                Return .OfType(Of Outlook.ContactItem).Any

            End With

        End If
        Return False
    End Function

#End Region

#Region "Ribbon: Label, ScreenTipp, ImageMso, OnAction"
#Disable Warning IDE0060 ' Nicht verwendete Parameter entfernen
    Public Function GetPressed(control As IRibbonControl) As Boolean
        Return GetPressedAnrMon()
    End Function
#Enable Warning IDE0060 ' Nicht verwendete Parameter entfernen

    ''' <summary>
    ''' Ermittelt das Label des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemLabel(control As IRibbonControl) As String
        Return GetRibbonContent(control.Id, Typ.Label)
    End Function

    ''' <summary>
    ''' Ermittelt das ScreenTipp des Ribbon-Objektes ausgehend von der Ribbon-id für Explorer
    ''' </summary>
    ''' <param name="control"></param>
    Public Function GetItemScreenTipp(control As IRibbonControl) As String
        Return GetRibbonContent(control.Id, Typ.ScreenTipp)
    End Function

    ''' <summary>
    ''' Ermittelt das Icon (ImageMSO) des Ribbon-Objektes ausgehend von der Ribbon-id
    ''' </summary>
    ''' <param name="control">Die id des Ribbon Controls</param>
    ''' <returns>Bezeichnung des ImageMso</returns>
    Public Function GetItemImageMso(control As IRibbonControl) As String
        Return GetRibbonContent(control.Id, Typ.ImageMso)
    End Function

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem Explorer-Button hinterlegt ist.
    ''' </summary>
    ''' <param name="control">Die Object, was im Ribbon angeklickt wurde.</param>
    Public Sub BtnOnAction(control As IRibbonControl)
        GetRibbonAction(Of String, String)(control.Id, control.Tag, Nothing)
        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem Inspector-Button hinterlegt ist.
    ''' </summary>
    ''' <param name="control">Die Object, was im Ribbon angeklickt wurde.</param>>
    Public Sub BtnOnActionI(control As IRibbonControl)
        GetRibbonAction(control.Id, CType(control.Context, Outlook.Inspector), control.Tag)
        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem Button im Kontextmenü auf einer Kontaktkarte hinterlegt ist.
    ''' </summary>
    ''' <param name="control">Die Object, was im Ribbon angeklickt wurde.</param>>
    Public Sub BtnOnActionCC(control As IRibbonControl)
        GetRibbonAction(control.Id, CType(control.Context, IMsoContactCard), control.Tag)
        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub

    Public Sub BtnOnActionS(control As IRibbonControl)
        GetRibbonAction(control.Id, CType(control.Context, Outlook.Selection), control.Tag)
        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub

    ''' <summary>
    ''' Ruft die jeweilige Funktion auf, die dem ToogleButton hinterlegt ist.
    ''' </summary>
    ''' <param name="control">ToogleButton</param>
    ''' <param name="pressed">Zustand des ToogleButtons</param>
    ''' <remarks>Eine reine Weiterleitung auf die Standard-OnAction Funktion</remarks>
    Public Sub BtnOnToggleButtonAction(control As IRibbonControl, pressed As Boolean)
        GetRibbonAction(Of Boolean, String)(control.Id, pressed, Nothing)
    End Sub

    Public Sub BtnOnActionRWS(control As IRibbonControl)
        GetRibbonAction(control.Id, CType(control.Context, Outlook.Inspector), control.Tag)
        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub
#End Region

#Region "DynamicMenu"
#Region "CALL RING VIP"
    Public Function DynMenuEnabled(control As IRibbonControl) As Boolean
        Return ListCRVEnabled(control.Id)
    End Function

    ''' <summary>
    ''' Lädt ein XML-String, der in das DynamicMenu geladen wird
    ''' </summary>
    ''' <param name="control">Das Ribbon-Control, für das das das DynamicMenu verwendet werden soll.</param>
    Public Function FillDynamicMenu(control As IRibbonControl) As String
        Return GetDynamicMenu(control.Id)
    End Function
#End Region

#Region "RWS"
    Public Function FillDynamicMenuRWS(control As IRibbonControl) As String
        Return GetDynamicMenuRWS(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem), control.Id)
    End Function
#End Region

#Region "Telefonbücher"
    Public Function FillDynamicMenuTelBk(control As IRibbonControl) As String
        Return GetDynamicMenuTelBk(control.Id)
    End Function
#End Region

#Region "Index-Test"
    Public Function FillDynamicMenuIndex(control As IRibbonControl) As String
        Return GetDynamicMenuIndexTest(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem), control.Id)
    End Function

#Disable Warning IDE0060 ' Nicht verwendete Parameter entfernen
    Public Function IndexTestVisible(control As IRibbonControl) As Boolean
        Return VisibilityIndexTest()
    End Function
#Enable Warning IDE0060 ' Nicht verwendete Parameter entfernen
#End Region
#End Region

#Region "VIP-Ribbon"
    ''' <summary>
    ''' Ermittelt den hinterlegten Outlook-Kontakt und setzt diesen auf die VIP-Liste oder entfernt ihn.
    ''' </summary>
    ''' <param name="control">Das Control als ToggleButton.</param>
    ''' <param name="isPressed">Dieser Callback gibt an ob der Toggle Button gepresst angezeigt werden soll.</param>
    Public Sub ToggleBtnOnActionVIP(control As IRibbonControl, ByRef isPressed As Boolean)

        Dim oKontakt As Outlook.ContactItem = Nothing

        Select Case True
            Case TypeOf control.Context Is Outlook.Selection
                oKontakt = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)

            Case TypeOf control.Context Is Outlook.Inspector
                oKontakt = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)

        End Select

        GetRibbonAction(Of Outlook.ContactItem, String)(control.Id, oKontakt, Nothing)

        isPressed = oKontakt.IsVIP

        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub

    ''' <summary>
    ''' Dieser Callback gibt an ob der Toggle Button gepresst angezeigt werden soll. 
    ''' </summary>
    ''' <param name="control">Das Control als ToggleButton.</param>
    ''' <returns>Boolean</returns>
    Public Function TBtnPressedVIP(control As IRibbonControl) As Boolean
        Dim oKontakt As Outlook.ContactItem = Nothing

        Select Case True
            Case TypeOf control.Context Is Outlook.Selection

                With CType(control.Context, Outlook.Selection)
                    If TypeOf .Item(1) Is Outlook.ContactItem Then
                        oKontakt = CType(CType(control.Context, Outlook.Selection).Item(1), Outlook.ContactItem)
                    End If
                End With

            Case TypeOf control.Context Is Outlook.Inspector
                oKontakt = CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem)

        End Select

        Return GetPressedVIP(oKontakt)
    End Function

#End Region

#Region "Telefonbücher"
    Public Sub BtnOnActionBk(control As IRibbonControl)

        Dim oKontakte As New List(Of Outlook.ContactItem)

        Select Case True
            Case TypeOf control.Context Is Outlook.Selection

                With CType(control.Context, Outlook.Selection)
                    oKontakte.AddRange(.OfType(Of Outlook.ContactItem))
                End With


            Case TypeOf control.Context Is Outlook.Inspector
                oKontakte.Add(CType(CType(control.Context, Outlook.Inspector).CurrentItem, Outlook.ContactItem))

        End Select

        GetRibbonAction(control.Id, oKontakte, control.Tag)

        ' Macht die zwischengespeicherten Werte für alle Steuerelemente der Menüband-Benutzeroberfläche ungültig.
        ' Zeichne Ribbon neu
        Invalidate()
    End Sub

#End Region

End Class