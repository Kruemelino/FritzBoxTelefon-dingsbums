Imports System.Reflection
Imports System.Xml
Imports Microsoft.Office.Interop

Namespace RibbonData
    ''' <summary>
    ''' Routinen zur Behandlung aller Ribbon-Informationen
    ''' </summary>
    Friend Module RibbonData
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Hilfsroutinen"
        Friend Enum Typ
            Label
            ScreenTipp
            ImageMso
        End Enum

        Private Sub Window_Closed(sender As Object, e As EventArgs)

            ' Window der Variable zuweisen
            Dim Window As Windows.Window = CType(sender, Windows.Window)
            ' Ereignishandler entfernen
            RemoveHandler Window.Closed, AddressOf Window_Closed
            ' Window aus der Liste entfernen
            ThisAddIn.AddinWindows.Remove(Window)

            NLogger.Debug("Fenster aus der Gesamtliste entfernt.")
        End Sub


#End Region

        ''' <summary>
        ''' Ermittelt den anzuzeigenden Content der einzelnen Schaltflächen (Label, ScreenTipp und ImageMso).
        ''' </summary>
        ''' <param name="Key">Schlüssel zur Identifikation des Steuerelementes.</param>
        ''' <param name="DatenTyp">Festlegung der spezifischen Daten für das Steuerlement.></param>
        ''' <returns>Zeichenfolge (String) der angeforderten Daten</returns>
        Friend Function GetRibbonContent(Key As String, DatenTyp As Typ) As String
            ' Der Key enthält eine Information zu dem Control, für das die Daten ermittelt werden sollen
            ' Separiert wird mit Umterstich "_"
            ' {ControlID}_{TabID}
            ' Die TabID ist irrlevant (Alles hinter dem _ muss entfernt werden
            ' In der Lokalisierungsdatei sind die Textbausteine 
            ' {ControlID}_{Typ} aufgebaut

            Dim KeyRes As String = $"{Key.RegExRemove("_.*")}_{DatenTyp}"
            Dim retVal As String = "Fehler"

            Select Case DatenTyp
                Case Typ.Label, Typ.ScreenTipp
                    retVal = Localize.resRibbon.ResourceManager.GetString(KeyRes)

                Case Typ.ImageMso
                    retVal = resImageMso.ResourceManager.GetString(KeyRes)

            End Select

            NLogger.Trace($"Daten ('{DatenTyp}') für '{Key}': {retVal}")
            Return retVal
        End Function

        ''' <summary>
        ''' Ermittle anhandes Keys die vorgesehene Routine mittels Reflection.
        ''' </summary>
        ''' <typeparam name="T1">Typ des Parameters</typeparam>
        ''' <param name="Key">Identifikation der Schaltfläche</param>
        ''' <param name="Parameter1">Zu übergebender Parameter</param>
        ''' <param name="Parameter2">Zu übergebender Parameter</param>
        Friend Sub GetRibbonAction(Of T1, T2)(Key As String, Parameter1 As T1, Parameter2 As T2)
            ' Der Key enthält eine Information zu dem Control, für das die Daten ermittelt werden sollen
            ' Separiert wird mit Umterstich "_"
            ' {ControlID}_{TabID}
            ' Die TabID ist irlevant (Alles hinter dem _ muss entfernt werden
            Dim mInfo As MethodInfo
            Dim KeyRes As String = Key.RegExRemove("_.*")

            Dim TypeArray() As Type
            Dim ParameterArray() As Object

            ' Ermittle das Array der übergebenen Typen
            If Parameter1 Is Nothing And Parameter2 Is Nothing Then
                ' A: Kein Parameter übergeben
                TypeArray = {}

                ' ParameterArray füllen
                ParameterArray = Nothing
            Else

                If Parameter1 IsNot Nothing And Parameter2 IsNot Nothing Then
                    ' C: Beide Parameter sind übergeben
                    TypeArray = {GetType(T1), GetType(T2)}

                    ' ParameterArray füllen
                    ParameterArray = {Parameter1, Parameter2}
                Else

                    If Parameter1 IsNot Nothing And Parameter2 Is Nothing Then
                        TypeArray = {GetType(T1)}

                        ' ParameterArray füllen
                        ParameterArray = {Parameter1}
                    Else
                        TypeArray = {GetType(T2)}

                        ' ParameterArray füllen
                        ParameterArray = {Parameter2}
                    End If

                End If
            End If

            ' Ermittle anhandes Keys die vorgesehene Routine mittels Reflection
            Try
                ' Sucht nach der angegebenen Methode, deren Parameter den angegebenen Argumenttypen und -modifizierern entsprechen, und 
                ' verwendet dabei die angegebenen Bindungseinschränkungen und die angegebene Aufrufkonvention.
                mInfo = GetType(RibbonData).GetMethod(KeyRes,
                                                      BindingFlags.NonPublic Or BindingFlags.Static,
                                                      Nothing,
                                                      TypeArray,
                                                      Nothing)

            Catch ex As Exception
                NLogger.Error(ex, $"GetMethod({KeyRes})")

                mInfo = Nothing
            End Try

            If mInfo IsNot Nothing Then
                Try
                    ' Hat die Zielfunktion Parameter
                    If mInfo.GetParameters.Any Then
                        ' Starte die Routine mit Parameter
                        mInfo.Invoke(Nothing, ParameterArray)
                    Else
                        ' Starte die Routine ohne Parameter
                        mInfo.Invoke(Nothing, Nothing)

                    End If

                    NLogger.Debug($"Routine '{mInfo.Name}' gestartet ({KeyRes}).")

                Catch ex As Exception
                    NLogger.Error(ex, "Invoke")

                End Try
            Else
                NLogger.Warn($"Routine für '{Key}' nicht gefunden.")
            End If

        End Sub

#Region "Ribbon Action"
        ''' <summary>
        ''' Einblenden der Einstellungen. (Routine wird über Reflection.Invoke eingeblendet)
        ''' </summary>
        Private Sub Einstellungen()
            ' Blendet ein neues Einstellungsfenster ein
            Dim AddinFenster As OptionenWPF = CType(ThisAddIn.AddinWindows.Find(Function(Window) TypeOf Window Is OptionenWPF), OptionenWPF)

            If AddinFenster Is Nothing Then
                ' Neues Window generieren
                AddinFenster = New OptionenWPF
                ' Ereignishandler hinzufügen
                AddHandler AddinFenster.Closed, AddressOf Window_Closed
                ' Window in die Liste aufnehmen
                ThisAddIn.AddinWindows.Add(AddinFenster)
            Else
                AddinFenster.Activate()
            End If
        End Sub

        ''' <summary>
        ''' Einblenden des Anrufmonitors. (Routine wird über Reflection.Invoke eingeblendet)
        ''' </summary>
        Private Sub ShowCallMonitor()
            If XMLData.PTelListen.RINGListe.Count.IsNotZero Then
                XMLData.PTelListen.RINGListe.Item(0).AnrMonEinblenden()
            Else
                Using tmptelfnt As New Telefonat With {.AnruferName = My.Resources.strDefLongName, .GegenstelleTelNr = New Telefonnummer With {.SetNummer = "0123456789"}, .ZeitBeginn = Now}
                    tmptelfnt.AnrMonEinblenden()
                End Using
            End If
        End Sub

        ''' <summary>
        ''' Einblenden der Direktwahl. (Routine wird über Reflection.Invoke eingeblendet)
        ''' </summary>
        Private Sub DirectCall()
            Dim AddinFenster As WählclientWPF = CType(ThisAddIn.AddinWindows.Find(Function(Window) TypeOf Window Is WählclientWPF), WählclientWPF)

            If AddinFenster Is Nothing Then
                ' Neuen Wählclient generieren
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart()
                ' Fenster zuweisen
                AddinFenster = WählClient.WPFWindow
                ' Ereignishandler hinzufügen
                AddHandler AddinFenster.Closed, AddressOf Window_Closed
                ' Window in die Liste aufnehmen
                ThisAddIn.AddinWindows.Add(AddinFenster)
            Else
                AddinFenster.Activate()
            End If
        End Sub

        ''' <summary>
        ''' Einblenden der Kontaktwahl. (Routine wird über Reflection.Invoke eingeblendet)
        ''' </summary>
        Private Sub Dial()
            Dim AddinFenster As WählclientWPF = CType(ThisAddIn.AddinWindows.Find(Function(Window) TypeOf Window Is WählclientWPF), WählclientWPF)

            If AddinFenster Is Nothing Then
                ' Neuen Wählclient generieren
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(ThisAddIn.OutookApplication.ActiveExplorer.Selection)
                ' Fenster zuweisen
                AddinFenster = WählClient.WPFWindow

                If AddinFenster IsNot Nothing Then
                    ' Ereignishandler hinzufügen
                    AddHandler AddinFenster.Closed, AddressOf Window_Closed
                    ' Window in die Liste aufnehmen
                    ThisAddIn.AddinWindows.Add(AddinFenster)
                End If
            Else
                AddinFenster.Activate()
            End If
        End Sub

        ''' <summary>
        ''' Einblenden der Kontaktwahl aus Inspektorfenster. (Routine wird über Reflection.Invoke eingeblendet)
        ''' </summary>
        ''' <param name="OutlookInspector"></param>
        Private Sub Dial(OutlookInspector As Outlook.Inspector)
            Dim AddinFenster As WählclientWPF = CType(ThisAddIn.AddinWindows.Find(Function(Window) TypeOf Window Is WählclientWPF), WählclientWPF)

            If AddinFenster Is Nothing Then
                ' Neuen Wählclient generieren
                Dim WählClient As New FritzBoxWählClient
                WählClient.WählboxStart(OutlookInspector)
                ' Fenster zuweisen
                AddinFenster = WählClient.WPFWindow

                If AddinFenster IsNot Nothing Then
                    ' Ereignishandler hinzufügen
                    AddHandler AddinFenster.Closed, AddressOf Window_Closed
                    ' Window in die Liste aufnehmen
                    ThisAddIn.AddinWindows.Add(AddinFenster)
                End If
            Else
                AddinFenster.Activate()
            End If
        End Sub

        ''' <summary>
        ''' Einblenden der Fritz!Box Anrufliste.
        ''' </summary>
        Private Sub JournalImport()
            Dim AnrListImportWPF As New AnrListWPF
            AnrListImportWPF.Show()
        End Sub

        ''' <summary>
        ''' Einblenden der Fritz!Box Telefonbuchverwaltung. (Routine wird über Reflection.Invoke eingeblendet)
        ''' </summary>
        Private Sub PhoneBooks()
            Dim fbt As New FBKontakBuch
            fbt.Show()
        End Sub

        ''' <summary>
        ''' Ein- und Ausschalten des Anrufmonitors
        ''' </summary>
        Private Sub CallMonitor(pressed As Boolean)
            ' Wenn der Anrufmonor Nothing ist, dann initiiere ihn
            If ThisAddIn.PAnrufmonitor Is Nothing Then ThisAddIn.PAnrufmonitor = New Anrufmonitor
            ' Wenn der Anrufmonitor aktiv ist, dann trenne ihn, ansonsten starte ihn
            With ThisAddIn.PAnrufmonitor
                If .Aktiv Then
                    .StoppAnrMon()
                Else
                    .StartAnrMon()
                End If
            End With
        End Sub

        ''' <summary>
        ''' Entfernt alle Einträge aus der Liste.
        ''' </summary>
        ''' <param name="Parameter">Identifikation der Liste</param>
        Private Sub DynListDel(Parameter As String)

            Dim KeyDelete As String = Parameter.RegExRemove("^.*_")

            NLogger.Info($"Liste {KeyDelete} gelöscht.")

            Select Case KeyDelete
                Case DfltNameListCALL
                    XMLData.PTelListen.CALLListe.Clear()

                Case DfltNameListRING
                    XMLData.PTelListen.RINGListe.Clear()

                Case DfltNameListVIP
                    XMLData.PTelListen.VIPListe.Clear()

            End Select
        End Sub

        ''' <summary>
        ''' Bahandelt das Klicken auf reguläre Einträge der Call, Ring und VIP Liste
        ''' </summary>
        ''' <param name="Tag">Identifikation des Listeneintrages</param>
        Private Sub ListCRV(Tag As String)
            ' RingList_0
            Dim ID As String() = Tag.Split("_")
            Dim WählClient As New FritzBoxWählClient

            ' Ermittle die zugehörige Liste
            Select Case ID(0)
                Case DfltNameListCALL
                    Dim Liste As List(Of Telefonat) = XMLData.PTelListen.CALLListe

                    WählClient.WählboxStart(Liste(ID(1).ToInt))

                Case DfltNameListRING
                    Dim Liste As List(Of Telefonat) = XMLData.PTelListen.RINGListe

                    WählClient.WählboxStart(Liste(ID(1).ToInt))

                Case DfltNameListVIP
                    Dim Liste As List(Of VIPEntry) = XMLData.PTelListen.VIPListe

                    WählClient.WählboxStart(Liste(ID(1).ToInt))

                Case Else

            End Select

        End Sub

        Private Sub Contact(OutlookInspector As Outlook.Inspector)
            ZeigeKontaktAusInspector(OutlookInspector)
        End Sub

        ''' <summary>
        ''' Setzt den übergebenen Outlook Kontakt auf die VIP-Liste oder entfernt diesen.
        ''' </summary>
        ''' <param name="OutlookContactItem">Der zu verarbeitende Kontakt.</param>
        Private Sub VIP(OutlookContactItem As Outlook.ContactItem)
            If OutlookContactItem IsNot Nothing Then OutlookContactItem.ToggleVIP
        End Sub

        ''' <summary>
        ''' Führt die Rückwärtssuche bei Journaleinträgen aus Inspector aus.
        ''' </summary>
        ''' <param name="OutlookInspector">Der zu verarbeitende Inspector.</param>
        Private Sub RWS(OutlookInspector As Outlook.Inspector, Tag As String)

            Select Case True
                Case TypeOf OutlookInspector.CurrentItem Is Outlook.JournalItem
                    StartJournalRWS(CType(OutlookInspector.CurrentItem, Outlook.JournalItem))

                Case TypeOf OutlookInspector.CurrentItem Is Outlook.ContactItem
                    StartKontaktRWS(CType(OutlookInspector.CurrentItem, Outlook.ContactItem), New Telefonnummer With {.SetNummer = Tag})

            End Select

        End Sub

        ''' <summary>
        ''' Lädt den übergebenen Kontakte in die Fritz!Box hoch
        ''' </summary>
        ''' <param name="OutlookContactItems"></param>
        ''' <param name="BookID"></param>
        Private Sub UploadBk(OutlookContactItems As IEnumerable(Of Outlook.ContactItem), BookID As String)

            NLogger.Debug($"Füge {OutlookContactItems.Count} Einträge zum Telefonbuch (ID{BookID}) hinzu.")

            ' Lädt die Kontakte in das Telefonbuch hoch
            Telefonbücher.SetTelefonbuchEintrag(BookID.ToInt, OutlookContactItems)
        End Sub

        ''' <summary>
        ''' Lädt den übergebenen Kontakte in die Fritz!Box Sperrliste hoch
        ''' </summary>
        ''' <param name="OutlookContactItems"></param>
        ''' <param name="BookID"></param>
        Private Sub UploadSl(OutlookContactItems As IEnumerable(Of Outlook.ContactItem), BookID As String)

            NLogger.Debug($"Füge {OutlookContactItems.Count} Einträge zur Sperrliste (ID{BookID}) hinzu.")

            ' Lädt die Kontakte in das Telefonbuch der Rufsperre hoch
            AddToCallBarring(OutlookContactItems)

        End Sub


#End Region

#Region "Control Enabled"
        ''' <summary>
        ''' Gibt an, ob die Liste eingeschaltet ist. 
        ''' </summary>
        ''' <param name="Tag">Indentifikation der Liste</param>
        ''' <returns>Boolean</returns>
        Friend Function ListCRVEnabled(Tag As String) As Boolean
            Dim ID As String() = Tag.Split("_")

            If XMLData IsNot Nothing Then
                Select Case ID(0)
                    Case DfltNameListCALL
                        Return XMLData.PTelListen.CALLListe IsNot Nothing AndAlso XMLData.PTelListen.CALLListe.Any

                    Case DfltNameListRING
                        Return XMLData.PTelListen.RINGListe IsNot Nothing AndAlso XMLData.PTelListen.RINGListe.Any

                    Case DfltNameListVIP
                        Return XMLData.PTelListen.VIPListe IsNot Nothing AndAlso XMLData.PTelListen.VIPListe.Any

                    Case Else
                        Return False
                End Select
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' Rekursive Funktion, um den Dial-Button zu aktivieren.
        ''' </summary>
        ''' <typeparam name="T">Typ des übergebenen Outlook Element</typeparam>
        ''' <param name="Context"></param>
        ''' <returns>Übergebenes Outlook Element</returns>
        Friend Function EnableDial(Of T)(Context As T) As Boolean

            Select Case True
                Case TypeOf Context Is Outlook.Explorer
                    ' Werte die Selection des Explorer aus

                    With CType(Context, Outlook.Explorer)
                        ' Rekursiver Aufruf
                        Try
                            Return EnableDial(.Selection)
                        Catch ' ex As Runtime.InteropServices.COMException
                            ' https://social.msdn.microsoft.com/Forums/en-US/1d6aa6df-53db-42d6-946d-130e642ddacb/comexception-when-checking-activeexplorerselection?forum=outlookdev
                            NLogger.Debug("Outlook mit 'Outlook Heute' gestartet.")
                            Return False
                        End Try

                    End With

                Case TypeOf Context Is Outlook.Inspector
                    With CType(Context, Outlook.Inspector)
                        Select Case True
                            Case TypeOf .CurrentItem Is Outlook.ContactItem

                                ' Rekursiver Aufruf
                                Return EnableDial(CType(.CurrentItem, Outlook.ContactItem))

                            Case TypeOf .CurrentItem Is Outlook.MailItem

                                Dim MailAdr As EMailType = GetSenderSMTPAddress(CType(.CurrentItem, Outlook.MailItem))

                                ' Rekursiver Aufruf
                                If MailAdr.OutlookTyp = OutlookEMailType.SMTP Then

                                    ' ContactItem
                                    Return EnableDial(KontaktSuche(MailAdr))
                                Else

                                    ' ExchangeUser
                                    Return EnableDial(KontaktSucheExchangeUser(MailAdr))
                                End If

                            Case TypeOf .CurrentItem Is Outlook.JournalItem

                                ' Rekursiver Aufruf
                                Return EnableDial(CType(.CurrentItem, Outlook.JournalItem))

                        End Select
                    End With

                Case TypeOf Context Is Outlook.Selection
                    With CType(Context, Outlook.Selection)

                        If .Count.IsNotZero Then
                            Select Case True
                                Case TypeOf .Item(1) Is Outlook.ContactItem

                                    ' Rekursiver Aufruf
                                    Return EnableDial(CType(.Item(1), Outlook.ContactItem))

                                Case TypeOf .Item(1) Is Outlook.MailItem
                                    Dim MailAdr As EMailType = GetSenderSMTPAddress(CType(.Item(1), Outlook.MailItem))

                                    ' Rekursiver Aufruf
                                    If MailAdr.OutlookTyp = OutlookEMailType.SMTP Then

                                        ' ContactItem
                                        Return EnableDial(KontaktSuche(MailAdr))
                                    Else

                                        ' ExchangeUser
                                        Return EnableDial(KontaktSucheExchangeUser(MailAdr))
                                    End If

                                Case TypeOf .Item(1) Is Outlook.JournalItem

                                    ' Rekursiver Aufruf
                                    Return EnableDial(CType(.Item(1), Outlook.JournalItem))

                            End Select
                        End If

                    End With

                Case TypeOf Context Is Microsoft.Office.Core.IMsoContactCard

                    ' Es gibt zwei Möglichkeiten:
                    ' A: Ein klassischer Kontakt ist hinterlegt
                    ' B: Ein Exchange-User existiert.

                    ' Rekursiver Aufruf
                    Return EnableDial(KontaktSuche(CType(Context, Microsoft.Office.Core.IMsoContactCard))) OrElse
                           EnableDial(KontaktSucheExchangeUser(CType(Context, Microsoft.Office.Core.IMsoContactCard)))

                Case TypeOf Context Is Outlook.ContactItem

                    ' Ermittelt, ob der Kontakt angerufen werden kann
                    With CType(Context, Outlook.ContactItem)
                        ' Hat der Kontakt Telefonnummern?
                        Return .GetTelNrArray.Any
                    End With

                Case TypeOf Context Is Outlook.JournalItem
                    ' Ermittelt, ob dem Journaleintrag ein Kontakthinterlegt ist, oder eine vCard, oder eine Telefonnummer
                    With CType(Context, Outlook.JournalItem)
                        Return Not .Body.StartsWith(String.Format($"{Localize.LocAnrMon.strJournalBodyStart} {Localize.LocAnrMon.strNrUnterdrückt}"))
                    End With

                Case TypeOf Context Is Outlook.ExchangeUser
                    ' Ermittelt, ob der Kontakt angerufen werden kann
                    ' Hat der Kontakt Telefonnummern?
                    Return GetKontaktTelNrList(CType(Context, Outlook.ExchangeUser)).Any

            End Select

            ' Rückgabe für den Rest
            Return False
        End Function

#End Region

#Region "Ribbon Pressed"
        Friend Function GetPressedAnrMon() As Boolean
            Return ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso ThisAddIn.PAnrufmonitor.Aktiv
        End Function

        Friend Function GetPressedVIP(Kontakt As Outlook.ContactItem) As Boolean
            Return Kontakt.IsVIP
        End Function
#End Region

#Region "VIP"
        Friend Function VIPRibbonContent(Kontakt As Outlook.ContactItem, Key As String, DatenTyp As Typ) As String
            Return GetRibbonContent($"{If(Kontakt.IsVIP, "Remove", "Add")}{Key}", DatenTyp)
        End Function

#End Region

#Region "Anrufmonitor"
        Friend Function AnrMonRibbonContent(Key As String, DatenTyp As Typ) As String
            Return GetRibbonContent($"{If(ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso ThisAddIn.PAnrufmonitor.Aktiv, "Online", "Offline")}{Key}", DatenTyp)
        End Function

#End Region

#Region "Journal"
        Friend Function JournalRibbonContent(Journal As Outlook.JournalItem, Key As String, DatenTyp As Typ) As String
            Return GetRibbonContent($"{If(GetOutlookKontakt(CType(Journal.PropertyAccessor.GetProperties(DASLTagJournal), Object())) Is Nothing, "Create", "Show")}{Key}", DatenTyp)
        End Function

        Friend Function InspectorGroupVisible(OutlookInspector As Outlook.Inspector) As Boolean

            ' Soll ausgeblendet werden, wenn Kein Explorer vorhanden ist
            ' oder das JournalItem nicht vom Addin ist

            If (New Outlook.Application).ActiveExplorer Is Nothing Then
                Return False
            Else
                If TypeOf OutlookInspector.CurrentItem Is Outlook.JournalItem Then
                    Return CheckJournalInspector(CType(OutlookInspector.CurrentItem, Outlook.JournalItem))
                Else
                    Return True
                End If
            End If

        End Function

        ''' <summary>
        ''' Gibt zurück, ob das JournalItem, von diesem Addin erstellt wurde. Dazu wird die Kategorie geprüft.
        ''' </summary>
        ''' <param name="OutlookJournal">Das zugehörige Ribbon Control.</param>
        ''' <returns>True, wenn JournalItem, von diesem Addin erstellt wurde. Ansonsten False</returns>
        Friend Function CheckJournalInspector(OutlookJournal As Outlook.JournalItem) As Boolean

            ' Bei Journal nur wenn Kategorien korrekt
            ' Wenn Journal keine Kategorie enthält, dann ist es kein vom Addin erzeugtes JournalItem
            Return OutlookJournal.Categories IsNot Nothing AndAlso OutlookJournal.Categories.Contains(String.Join("; ", DfltJournalDefCategories.ToArray))

        End Function

#End Region

#Region "Rückwärtssuche"
        Friend Function GetDynamicMenuRWS(Kontakt As Outlook.ContactItem, ListName As String) As String

            Dim XDynaMenu As New XmlDocument
            Dim ListofTelefonnummer As List(Of Telefonnummer)

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", "http://schemas.microsoft.com/office/2009/07/customui")))

                ' Ermittle alle Telefonnummern des Kontaktes
                ListofTelefonnummer = GetKontaktTelNrList(Kontakt)

                For Each TelNr In ListofTelefonnummer
                    .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, TelNr, ListofTelefonnummer.IndexOf(TelNr), ListName))
                Next
            End With

            Return XDynaMenu.InnerXml
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, TelNr As Telefonnummer, ID As Integer, Tag As String) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute
            With TelNr
                XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"{Tag}_{ID}"
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("label")
                XAttribute.Value = .Formatiert.XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("onAction")
                XAttribute.Value = "BtnOnActionRWS"
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("tag")
                XAttribute.Value = .Unformatiert.XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)
            End With

            Return XButton
        End Function

#End Region

#Region "Telefonbücher"
        Friend Function GetDynamicMenuTelBk(ListName As String) As String

            Dim XDynaMenu As New XmlDocument

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", "http://schemas.microsoft.com/office/2009/07/customui")))

                ' Ermittle die verfügbaren Quellen für die Telefonbuchnamen
                Dim TelBk As FritzBoxXMLTelefonbücher

                If ThisAddIn.PhoneBookXML Is Nothing Then
                    TelBk = Telefonbücher.LadeHeaderFritzBoxTelefonbücher
                Else
                    TelBk = ThisAddIn.PhoneBookXML
                End If

                ' Trage die einzelnen Bücher ein
                For Each Buch In TelBk.Telefonbücher
                    .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, Buch.Name, Buch.ID, Buch.Rufsperren, ListName))
                Next
            End With

            Return XDynaMenu.InnerXml
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, TelefonbuchName As String, BuchID As Integer, Sperrliste As Boolean, Tag As String) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = $"{Tag}{If(Sperrliste, "Sl", "Bk")}_{BuchID}"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("tag")
            XAttribute.Value = $"{BuchID}"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("label")
            XAttribute.Value = TelefonbuchName
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("onAction")
            XAttribute.Value = "BtnOnActionBk"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("getImage")
            XAttribute.Value = "GetItemImageMso"
            XButton.Attributes.Append(XAttribute)

            Return XButton
        End Function
#End Region

#Region "Listen für Wahlwiederholung, Rückruf, VIP"
        ''' <summary>
        ''' Erstelle die Liste der entsprechenden Einträge ausgehend vom <paramref name="ListName"/>.
        ''' </summary>
        ''' <param name="ListName">Name der Liste</param>
        ''' <returns></returns>
        Friend Function GetDynamicMenu(ListName As String) As String
            Dim ListevonTelefonaten As List(Of Telefonat)
            Dim XDynaMenu As New XmlDocument

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", "http://schemas.microsoft.com/office/2009/07/customui")))

                ' Füge den Löschbutton und einen Seperator hinzu
                .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, $"DynListDel_{ListName}"))
                .DocumentElement.AppendChild(CreateDynMenuSeperator(XDynaMenu))

                If ListName.AreEqual(DfltNameListCALL) Or ListName.AreEqual(DfltNameListRING) Then
                    ListevonTelefonaten = If(ListName.AreEqual(DfltNameListCALL), XMLData.PTelListen.CALLListe, XMLData.PTelListen.RINGListe)

                    For Each TelFt As Telefonat In ListevonTelefonaten.Where(Function(Tf) Not Tf.NrUnterdrückt)
                        .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, TelFt, ListevonTelefonaten.IndexOf(TelFt), ListName))
                    Next

                ElseIf ListName.AreEqual(DfltNameListVIP) Then

                    For Each VIP As VIPEntry In XMLData.PTelListen.VIPListe
                        .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, VIP, XMLData.PTelListen.VIPListe.IndexOf(VIP), ListName))
                    Next

                End If
            End With

            Return XDynaMenu.InnerXml
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, ID As String) As XmlElement
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

            XAttribute = xDoc.CreateAttribute("getScreentip")
            XAttribute.Value = "GetItemScreenTipp"
            XButton.Attributes.Append(XAttribute)

            Return XButton
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, Tlfnt As Telefonat, ID As Integer, Tag As String) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            With Tlfnt

                XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"ListCRV_{ID}"
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("label")
                XAttribute.Value = .NameGegenstelle.XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("onAction")
                XAttribute.Value = "BtnOnAction"
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("tag")
                XAttribute.Value = $"{Tag}_{ID}".XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("supertip")
                XAttribute.Value = $"{Localize.resCommon.strTime}: { .ZeitBeginn}{Dflt1NeueZeile}"
                XAttribute.Value += $"{Localize.resCommon.strTelNr}: { .GegenstelleTelNr.Formatiert}"

                If .GegenstelleTelNr.AreaCode.IsNotStringNothingOrEmpty Then XAttribute.Value += $"{Dflt1NeueZeile}{Localize.resCommon.strArea}: {Localize.Länder.ResourceManager.GetString(.GegenstelleTelNr.AreaCode)}"
                If .GegenstelleTelNr.Location.IsNotStringNothingOrEmpty Then XAttribute.Value += $"{Dflt1NeueZeile}{Localize.resCommon.strLocation}: { .GegenstelleTelNr.Location}"

                XButton.Attributes.Append(XAttribute)

                If Not .Angenommen Then
                    XAttribute = xDoc.CreateAttribute("getImage")
                    XAttribute.Value = "GetItemImageMso"
                    XButton.Attributes.Append(XAttribute)
                End If

            End With

            Return XButton
        End Function

        Friend Function CreateDynMenuButton(xDoc As XmlDocument, VIP As VIPEntry, ID As Integer, Tag As String) As Xml.XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            With VIP

                XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"ListCRV_{ID}"
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("label")

                .OlContact = GetOutlookKontakt(.EntryID, .StoreID)
                If .OlContact IsNot Nothing Then
                    XAttribute.Value = $"{ .OlContact.FullName}{If(.OlContact.CompanyName.IsNotStringNothingOrEmpty, String.Format(" ({0})", .OlContact.CompanyName), DfltStringEmpty)}".XMLMaskiereZeichen
                End If

                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("onAction")
                XAttribute.Value = "BtnOnAction"
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("tag")
                XAttribute.Value = $"{Tag}_{ID}".XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)

                XAttribute = xDoc.CreateAttribute("imageMso")
                XAttribute.Value = resImageMso.RemoveVIP_ImageMso
                XButton.Attributes.Append(XAttribute)
            End With

            Return XButton
        End Function

        Private Function CreateDynMenuSeperator(xDoc As XmlDocument) As XmlElement
            Dim XSeperator As XmlElement
            Dim XAttribute As XmlAttribute

            XSeperator = xDoc.CreateElement("menuSeparator", "http://schemas.microsoft.com/office/2009/07/customui")

            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = "separator"
            XSeperator.Attributes.Append(XAttribute)

            Return XSeperator
        End Function
#End Region
    End Module
End Namespace