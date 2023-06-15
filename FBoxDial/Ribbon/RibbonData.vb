Imports System.Reflection
Imports System.Xml
Imports FBoxDial.Localize
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop

Namespace RibbonData
    ''' <summary>
    ''' Routinen zur Behandlung aller Ribbon-Informationen
    ''' </summary>
    Friend Module RibbonData
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

        Private Const NamespaceURI As String = "http://schemas.microsoft.com/office/2009/07/customui"

        Friend Enum Typ
            Label
            ScreenTipp
            ImageMso
        End Enum

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

            ' NLogger.Trace($"Daten ('{DatenTyp}') für '{Key}': {retVal}")
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
        ''' Einblenden der Einstellungen. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
        ''' </summary>
        Private Sub Einstellungen()
            ' Blendet ein neues Einstellungsfenster ein
            AddWindow(Of OptionenWPF)(False)
        End Sub

        ''' <summary>
        ''' Einblenden des Anrufmonitors. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
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
        ''' Einblenden verpasster Anrufe. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
        ''' </summary>
        Private Sub ShowMissedCalls()
            Globals.ThisAddIn.ExplorerWrappers.Values.ToList.ForEach(Sub(ew) ew.ShowCallListPane())
        End Sub

        ''' <summary>
        ''' Einblenden der Direktwahl. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
        ''' </summary>
        Private Sub DirectCall()
            ' Neuen Wählclient generieren
            ' Finde das existierende Fenster, oder generiere ein neues
            With New FritzBoxWählClient
                .WählboxStart()
            End With
        End Sub

        ''' <summary>
        ''' Einblenden der Kontaktwahl. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
        ''' </summary>
        Private Sub Dial()
            ' Neuen Wählclient generieren
            ' Finde das existierende Fenster, oder generiere ein neues
            With New FritzBoxWählClient
                .WählboxStart(Globals.ThisAddIn.Application.ActiveExplorer.Selection)
            End With
        End Sub

        ''' <summary>
        ''' Einblenden der Kontaktwahl aus Inspektorfenster. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
        ''' </summary>
        Private Sub Dial(OutlookInspector As Outlook.Inspector)
            ' Neuen Wählclient generieren
            ' Finde das existierende Fenster, oder generiere ein neues
            With New FritzBoxWählClient
                .WählboxStart(OutlookInspector)
            End With
        End Sub

        ''' <summary>
        ''' Einblenden der Kontaktwahl aus dem Inspektorfenster einer ContactCard. (Routine wird über <see cref="MethodInfo.Invoke"/> eingeblendet)
        ''' </summary>
        Private Sub Dial(ContactCard As IMsoContactCard)
            ' Neuen Wählclient generieren
            ' Finde das existierende Fenster, oder generiere ein neues
            With New FritzBoxWählClient
                .WählboxStart(ContactCard)
            End With
        End Sub

        ''' <summary>
        ''' Behandelt das Klicken auf reguläre Einträge der Call, Ring und VIP Liste
        ''' </summary>
        ''' <param name="Tag">Identifikation des Listeneintrages: RingList_0</param>
        Private Sub ListCRV(Tag As String)

            Dim ID As String() = Tag.Split("_")

            With New FritzBoxWählClient

                If ID.First.Equals(My.Resources.strDfltNameListVIP) Then
                    .WählboxStart(XMLData.PTelListen.VIPListe.Item(ID.Last.ToInt))
                Else
                    ' Ermittle die Wahlwiederholungs- bzw. Rückrufliste mittels Reflection aus dem übergebenen Namen
                    .WählboxStart(CType(XMLData.PTelListen.GetType().GetProperty(ID.First).GetValue(XMLData.PTelListen), List(Of Telefonat)).Item(ID.Last.ToInt))
                End If
            End With

        End Sub

        ''' <summary>
        ''' Erstellt einen Rückruftermin
        ''' </summary>
        ''' <param name="Tag">Identifikation des Listeneintrages: RingList_0</param>
        Private Sub SceduleButtonCRV(Tag As String)
            XMLData.PTelListen.CreateAppointment(Tag)
        End Sub

        ''' <summary>
        ''' Entfernt einen Eintrag aus der Liste
        ''' </summary>
        ''' <param name="Tag">Identifikation des Listeneintrages: RingList_0</param>
        Private Sub DeleteEntryButtonCRV(Tag As String)
            XMLData.PTelListen.ClearListEntry(Tag)
        End Sub

        ''' <summary>
        ''' Einblenden der Fritz!Box Daten
        ''' </summary>
        Private Sub FritzBoxData()
            AddWindow(Of FBoxDataWPF)(False).Show()
        End Sub

        Private Sub Search()
            AddWindow(Of KontaktsucheWPF)(False)
        End Sub

        ''' <summary>
        ''' Ein- und Ausschalten des Anrufmonitors
        ''' </summary>
        Private Sub CallMonitor(pressed As Boolean)
            ' Wenn der Anrufmonor Nothing ist, dann initiiere ihn
            If Globals.ThisAddIn.PAnrufmonitor Is Nothing Then Globals.ThisAddIn.PAnrufmonitor = New Anrufmonitor
            ' Wenn der Anrufmonitor aktiv ist, dann trenne ihn, ansonsten starte ihn
            With Globals.ThisAddIn.PAnrufmonitor
                If .Aktiv Then
                    .Stopp()
                Else
                    .Start()
                End If
            End With
        End Sub

        ''' <summary>
        ''' Entfernt alle Einträge aus der Liste.
        ''' </summary>
        ''' <param name="Parameter">Identifikation der Liste</param>
        Private Sub DynListDel(Parameter As String)
            XMLData.PTelListen.ClearList(Parameter.RegExRemove("^.*_"))
        End Sub

        Private Sub Contact(OutlookInspector As Outlook.Inspector)
            ZeigeKontaktAusInspector(OutlookInspector)
        End Sub

        Private Sub Contact(OutlookSelection As Outlook.Selection)
            With OutlookSelection
                If .Count.IsNotZero Then
                    Select Case True
                        Case TypeOf .Item(1) Is Outlook.AppointmentItem Or
                             TypeOf .Item(1) Is Outlook.JournalItem

                            ZeigeKontaktAusOutlookItem(.Item(1))

                        Case Else

                    End Select
                End If

            End With

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
                Case TypeOf OutlookInspector.CurrentItem Is Outlook.JournalItem Or
                     TypeOf OutlookInspector.CurrentItem Is Outlook.AppointmentItem

                    StartOlItemRWS(OutlookInspector.CurrentItem)

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
            Telefonbücher.AddToCallBarring(OutlookContactItems)

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
                    Case My.Resources.strDfltNameListCALL
                        Return XMLData.PTelListen.CALLListe IsNot Nothing AndAlso XMLData.PTelListen.CALLListe.Any

                    Case My.Resources.strDfltNameListRING
                        Return XMLData.PTelListen.RINGListe IsNot Nothing AndAlso XMLData.PTelListen.RINGListe.Any

                    Case My.Resources.strDfltNameListVIP
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

                            Case TypeOf .CurrentItem Is Outlook.AppointmentItem

                                ' Rekursiver Aufruf
                                Return EnableDial(CType(.CurrentItem, Outlook.AppointmentItem))
                        End Select
                    End With

                Case TypeOf Context Is Outlook.Selection

                    With CType(Context, Outlook.Selection)
                        If .Count.IsNotZero Then
                            Select Case True
                                Case TypeOf .Item(1) Is Outlook.MailItem

                                    ' Durch den Else-Zweig wird die E-Mail geöffnet und auf gelesen gesetzt.
                                    ' Die Mail wird insbesondere beim verzögerten Versenden nicht mehr versendet und bleibt im Postausgang liegen. 
                                    If XMLData.POptionen.CBDisableMailCheck Then
                                        Return True
                                    Else
                                        Dim MailItem As Outlook.MailItem = CType(.Item(1), Outlook.MailItem)

                                        Dim MailAdr As EMailType = GetSenderSMTPAddress(MailItem)

                                        ' Rekursiver Aufruf
                                        If MailAdr.OutlookTyp = OutlookEMailType.SMTP Then

                                            ' ContactItem
                                            Return EnableDial(KontaktSuche(MailAdr))
                                        Else

                                            ' ExchangeUser
                                            Return EnableDial(KontaktSucheExchangeUser(MailAdr))
                                        End If
                                    End If

                                Case TypeOf .Item(1) Is Outlook.ContactItem

                                    ' Rekursiver Aufruf
                                    Return EnableDial(CType(.Item(1), Outlook.ContactItem))


                                Case TypeOf .Item(1) Is Outlook.JournalItem

                                    ' Rekursiver Aufruf
                                    Return EnableDial(CType(.Item(1), Outlook.JournalItem))

                                Case TypeOf .Item(1) Is Outlook.AppointmentItem

                                    ' Rekursiver Aufruf
                                    Return EnableDial(CType(.Item(1), Outlook.AppointmentItem))
                            End Select
                        End If
                    End With

                Case TypeOf Context Is IMsoContactCard

                    ' Es gibt zwei Möglichkeiten:
                    ' A: Ein klassischer Kontakt ist hinterlegt
                    ' B: Ein Exchange-User existiert.

                    ' Rekursiver Aufruf
                    Return EnableDial(KontaktSuche(CType(Context, IMsoContactCard))) OrElse
                           EnableDial(KontaktSucheExchangeUser(CType(Context, IMsoContactCard)))

                Case TypeOf Context Is Outlook.ContactItem
                    ' Ermittelt, ob der Kontakt angerufen werden kann

                    ' Hat der Kontakt Telefonnummern?
                    Return CType(Context, Outlook.ContactItem).HatKontaktTelefonnummern(False)

                Case TypeOf Context Is Outlook.JournalItem
                    ' Ermittelt, ob dem Journaleintrag ein Kontakt hinterlegt ist, oder eine vCard, oder eine Telefonnummer

                    With CType(Context, Outlook.JournalItem)
                        Return .Body IsNot Nothing AndAlso Not .Body.StartsWith(String.Format($"{Localize.LocAnrMon.strJournalBodyStart} {Localize.LocAnrMon.strNrUnterdrückt}"))
                    End With

                Case TypeOf Context Is Outlook.AppointmentItem
                    ' Ermittelt, ob dem Termin ein Kontakt hinterlegt ist, oder eine vCard, oder eine Telefonnummer

                    With CType(Context, Outlook.AppointmentItem)
                        Return .Body IsNot Nothing AndAlso Not .Body.StartsWith(String.Format($"{Localize.LocAnrMon.strJournalBodyStart} {Localize.LocAnrMon.strNrUnterdrückt}"))
                    End With

                Case TypeOf Context Is Outlook.ExchangeUser
                    ' Ermittelt, ob der Kontakt angerufen werden kann
                    ' Hat der Kontakt Telefonnummern?
                    Return CType(Context, Outlook.ExchangeUser).HatKontaktTelefonnummern

            End Select

            ' Rückgabe für den Rest
            Return False
        End Function

        Friend Function IsFBoxAPIConnected() As Boolean

            If Globals.ThisAddIn.FBoxTR064 Is Nothing Then
                Return False
            Else
                Return Globals.ThisAddIn.FBoxTR064.Ready
            End If

        End Function

#End Region

#Region "Control Visible"
        Friend Function VisibilityGroup(Of T)(Context As T) As Boolean

            Select Case True
                Case TypeOf Context Is Outlook.Explorer
                    ' Werte die Selection des Explorer aus
                    With CType(Context, Outlook.Explorer)
                        ' Rekursiver Aufruf
                        Try
                            Return VisibilityGroup(.Selection)
                        Catch ' ex As Runtime.InteropServices.COMException
                            ' https://social.msdn.microsoft.com/Forums/en-US/1d6aa6df-53db-42d6-946d-130e642ddacb/comexception-when-checking-activeexplorerselection?forum=outlookdev
                            NLogger.Debug("Outlook mit 'Outlook Heute' gestartet.")
                            Return False
                        End Try

                    End With
                Case TypeOf Context Is Outlook.Selection

                    With CType(Context, Outlook.Selection)
                        If .Count.IsNotZero Then
                            Select Case True
                                Case TypeOf .Item(1) Is Outlook.AppointmentItem Or
                                     TypeOf .Item(1) Is Outlook.JournalItem

                                    ' Rekursiver Aufruf
                                    Return VisibilityGroup(.Item(1))

                                Case TypeOf .Item(1) Is Outlook.ContactItem
                                    Return True
                                Case Else
                                    Return False
                            End Select
                        Else
                            Return False
                        End If
                    End With
                Case TypeOf Context Is Outlook.AppointmentItem
                    Return CheckInspector(CType(Context, Outlook.AppointmentItem))

                Case TypeOf Context Is Outlook.JournalItem
                    Return CheckInspector(CType(Context, Outlook.JournalItem))

                Case Else
                    Return False
            End Select

        End Function
#End Region

#Region "Ribbon Pressed"
        Friend Function GetPressedAnrMon() As Boolean
            Return Globals.ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso Globals.ThisAddIn.PAnrufmonitor.Aktiv
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
            Return GetRibbonContent($"{If(Globals.ThisAddIn.PAnrufmonitor IsNot Nothing AndAlso Globals.ThisAddIn.PAnrufmonitor.Aktiv, "Online", "Offline")}{Key}", DatenTyp)
        End Function

#End Region

#Region "Journal"
        Friend Function RibbonContent(Of T)(OlItem As T, Key As String, DatenTyp As Typ) As String
            Dim olKontakt As Outlook.ContactItem
            Select Case True
                Case TypeOf OlItem Is Outlook.AppointmentItem
                    With CType(OlItem, Outlook.AppointmentItem)
                        ' Outlook-Kontakt ermitteln
                        olKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagOlItem), Object()))
                    End With

                Case TypeOf OlItem Is Outlook.JournalItem
                    With CType(OlItem, Outlook.JournalItem)
                        ' Outlook-Kontakt ermitteln
                        olKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagOlItem), Object()))
                    End With

                Case Else
                    olKontakt = Nothing
            End Select

            Return GetRibbonContent($"{If(olKontakt Is Nothing, "Create", "Show")}{Key}", DatenTyp)

            ReleaseComObject(olKontakt)

        End Function

        Friend Function InspectorGroupVisible(OutlookInspector As Outlook.Inspector) As Boolean

            ' Soll ausgeblendet werden, wenn kein Explorer vorhanden ist oder das JournalItem nicht vom Addin ist
            If Globals.ThisAddIn.Application.ActiveExplorer Is Nothing Then
                Return False
                NLogger.Debug("Kein Explorer")
            Else
                Select Case True
                    Case TypeOf OutlookInspector.CurrentItem Is Outlook.JournalItem
                        Return CheckInspector(CType(OutlookInspector.CurrentItem, Outlook.JournalItem))

                    Case TypeOf OutlookInspector.CurrentItem Is Outlook.AppointmentItem
                        Return CheckInspector(CType(OutlookInspector.CurrentItem, Outlook.AppointmentItem))

                    Case Else
                        Return True
                End Select
            End If

        End Function

        ''' <summary>
        ''' Gibt zurück, ob das JournalItem, von diesem Addin erstellt wurde. Dazu wird die Kategorie geprüft.
        ''' </summary>
        ''' <param name="olItem">Das zugehörige Ribbon Control.</param>
        ''' <returns>True, wenn JournalItem, von diesem Addin erstellt wurde. Ansonsten False</returns>
        Friend Function CheckInspector(olItem As Outlook.JournalItem) As Boolean

            ' Bei Journal nur wenn Kategorien korrekt
            ' Wenn Journal keine Kategorie enthält, dann ist es kein vom Addin erzeugtes JournalItem
            Return CheckCategories(olItem.Categories)

        End Function

        ''' <summary>
        ''' Gibt zurück, ob der Termin, von diesem Addin erstellt wurde. Dazu wird die Kategorie geprüft.
        ''' </summary>
        ''' <param name="olItem">Das zugehörige Ribbon Control.</param>
        ''' <returns>True, wenn Termin, von diesem Addin erstellt wurde. Ansonsten False</returns>
        Friend Function CheckInspector(olItem As Outlook.AppointmentItem) As Boolean

            ' Bei Termin nur wenn Kategorien korrekt
            ' Wenn Termin keine Kategorie enthält, dann ist es kein vom Addin erzeugtes JournalItem
            Return CheckCategories(olItem.Categories)

        End Function

        Private Function CheckCategories(Categories As String) As Boolean
            Return Categories IsNot Nothing AndAlso Categories.Contains(String.Join("; ", DfltOlItemCategories.ToArray))
        End Function
#End Region

#Region "Rückwärtssuche"
        Friend Function GetDynamicMenuRWS(Kontakt As Outlook.ContactItem, ListName As String) As String

            Dim XDynaMenu As New XmlDocument
            Dim ListofTelefonnummer As List(Of Telefonnummer)

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", NamespaceURI)))

                ' Ermittle alle Telefonnummern des Kontaktes
                ListofTelefonnummer = Kontakt.GetTelNrList(False)

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

#Region "Index-Test"

        Friend Function VisibilityIndexTest() As Boolean

            Return XMLData.POptionen.CBShowIndexEntries

        End Function

        Private Sub IndexContact(OutlookInspector As Outlook.Inspector, ListName As String)

            If ListName.IsNotStringNothingOrEmpty Then ' Abfrage eigentlich unnötig
                If TypeOf OutlookInspector.CurrentItem Is Outlook.ContactItem Then

                    CType(OutlookInspector.CurrentItem, Outlook.ContactItem).IndiziereKontakt
                End If
            End If

        End Sub

        ''' <summary>
        ''' Erstelle die Liste der entsprechenden Indizierungseinträge des <paramref name="oContact"/> ausgehend vom <paramref name="ListName"/>.
        ''' </summary>
        ''' <param name="oContact">Der aktuelle Kontakt</param>
        ''' <param name="ListName">Eindeutige Bezeichnung der Liste</param>
        ''' <returns>XML-Dokument als String</returns>
        Friend Function GetDynamicMenuIndexTest(oContact As Outlook.ContactItem, ListName As String) As String

            Dim XDynaMenu As New XmlDocument

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", NamespaceURI)))

                ' Wenn der Ordner für die Kontaktsuche verwendet werden soll, dann ergänze die Einträge
                If oContact.Parent IsNot Nothing AndAlso CType(oContact.Parent, Outlook.MAPIFolder).OrdnerAusgewählt(OutlookOrdnerVerwendung.KontaktSuche) Then
                    ' Button für das manuelle Indizieren des Kontaktes
                    ' Füge den Löschbutton und einen Seperator hinzu
                    .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, $"IndexContact_{ListName}", "I"))
                    .DocumentElement.AppendChild(CreateDynMenuSeperator(XDynaMenu))

                    ' Einzelnen Indexeinträge
                    For Each Eintrag In oContact.GetIndexList
                        .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, ListName, Eintrag))
                    Next
                Else
                    ' Fehlermeldung
                    .DocumentElement.AppendChild(CreateDisabledDynMenuButton(XDynaMenu, ListName, "IndexError"))
                End If

            End With

            NLogger.Trace($"{ListName}: {XDynaMenu.OuterXml}")

            Return XDynaMenu.InnerXml
        End Function

        ''' <summary>
        ''' Erstelle die Liste der entsprechenden Syncronisierungsdaten des <paramref name="oContact"/> ausgehend vom <paramref name="ListName"/>.
        ''' </summary>
        ''' <param name="oContact">Der aktuelle Kontakt</param>
        ''' <param name="ListName">Eindeutige Bezeichnung der Liste</param>
        ''' <returns>XML-Dokument als String</returns>
        Friend Function GetDynamicMenuSyncTest(oContact As Outlook.ContactItem, ListName As String) As String

            Dim XDynaMenu As New XmlDocument

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", NamespaceURI)))

                ' Wenn der Ordner für die Kontaktsuche verwendet werden soll, dann ergänze die Einträge
                If oContact.Parent IsNot Nothing AndAlso CType(oContact.Parent, Outlook.MAPIFolder).OrdnerAusgewählt(OutlookOrdnerVerwendung.FBoxSync) Then
                    ' Button für das manuelle Synchronisieren des Kontaktes
                    ' Füge den Löschbutton und einen Seperator hinzu
                    .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, $"SyncContact_{ListName}", "I"))
                    .DocumentElement.AppendChild(CreateDynMenuSeperator(XDynaMenu))

                    ' Einzelnen Indexeinträge
                    For Each Eintrag In oContact.GetUniqueID
                        .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, ListName, Eintrag.Key,
                                                                         $"BuchID: {Eintrag.Key}, uID: {Eintrag.Value}",
                                                                         $"Mod_Time: {oContact.GetFBoxModTime(Eintrag.Key.ToInt, Eintrag.Value.ToInt)}"))

                    Next
                Else
                    ' Fehlermeldung
                    .DocumentElement.AppendChild(CreateDisabledDynMenuButton(XDynaMenu, ListName, "SyncError"))
                End If

            End With

            NLogger.Trace($"{ListName}: {XDynaMenu.OuterXml}")

            Return XDynaMenu.InnerXml
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, ID As String, KVP As KeyValuePair(Of String, String)) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = $"{ID}_{KVP.Key}"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("enabled")
            XAttribute.Value = $"false"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("label")
            XAttribute.Value = $"{resEnum.ResourceManager.GetString(KVP.Key)}: {KVP.Value}".Trim.XMLMaskiereZeichen
            XButton.Attributes.Append(XAttribute)

            Return XButton
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, ID As String, Key As String, Label As String, Screentip As String) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = $"{ID}_{Key}"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("enabled")
            XAttribute.Value = $"false"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("label")
            XAttribute.Value = Label.Trim.XMLMaskiereZeichen
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("screentip")
            XAttribute.Value = Screentip.Trim.XMLMaskiereZeichen
            XButton.Attributes.Append(XAttribute)

            Return XButton
        End Function

        Private Function CreateDisabledDynMenuButton(xDoc As XmlDocument, ID As String, Label As String) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = $"{ID}_{Label}"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("enabled")
            XAttribute.Value = $"false"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("label")
            XAttribute.Value = $"{resRibbon.ResourceManager.GetString($"{Label}_Label")}".Trim.XMLMaskiereZeichen
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("screentip")
            XAttribute.Value = $"{resRibbon.ResourceManager.GetString($"{Label}_ScreenTipp")}".Trim.XMLMaskiereZeichen
            XButton.Attributes.Append(XAttribute)

            Return XButton
        End Function
#End Region

#Region "Synchronisationstest"
        Private Sub SyncContact(OutlookInspector As Outlook.Inspector, ListName As String)

            If ListName.IsNotStringNothingOrEmpty Then ' Abfrage eigentlich unnötig
                If TypeOf OutlookInspector.CurrentItem Is Outlook.ContactItem Then
                    With CType(OutlookInspector.CurrentItem, Outlook.ContactItem)
                        .Self.SyncKontakt(.ParentFolder, True)
                    End With
                End If
            End If

        End Sub
#End Region

#Region "Telefonbücher"
        ''' <summary>
        ''' Erstelle die Liste mit den vorhandenen Telefonbüchern ausgehend vom <paramref name="ListName"/>.
        ''' </summary>
        ''' <param name="ListName">Name der Liste</param>
        ''' <returns>XML-Dokument als String</returns>
        Friend Function GetDynamicMenuTelBk(ListName As String) As String

            Dim XDynaMenu As New XmlDocument

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", NamespaceURI)))

                ' Ermittle die verfügbaren Quellen für die Telefonbuchnamen
                If Globals.ThisAddIn.PhoneBookXML IsNot Nothing Then
                    ' Trage die einzelnen Bücher ein
                    For Each Buch As PhonebookEx In Globals.ThisAddIn.PhoneBookXML.Where(Function(d) Not d.IsDAV)
                        .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, Buch.Phonebook.Name, Buch.ID, Buch.CallBarringBook, ListName))
                    Next
                Else
                    NLogger.Warn($"Telefonbücher sind nicht bekannt.")
                End If

            End With

            NLogger.Trace($"{ListName}: {XDynaMenu.OuterXml}")

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
        ''' <param name="ListName">Eindeutige Bezeichnung der Liste</param>
        ''' <returns>XML-Dokument als String</returns>
        Friend Function GetDynamicMenu(ListName As String) As String
            Dim XDynaMenu As New XmlDocument

            ListName = ListName.RegExRemove("_.*")

            With XDynaMenu
                ' Füge die XMLDeclaration und das Wurzelelement einschl. Namespace hinzu
                .InsertBefore(.CreateXmlDeclaration("1.0", "UTF-8", Nothing), .AppendChild(.CreateElement("menu", NamespaceURI)))

                ' Füge den Löschbutton und einen Seperator hinzu
                .DocumentElement.AppendChild(CreateDynMenuButton(XDynaMenu, $"DynListDel_{ListName}"))
                .DocumentElement.AppendChild(CreateDynMenuSeperator(XDynaMenu))

                Select Case ListName
                    Case My.Resources.strDfltNameListCALL

                        For Each TelFt As Telefonat In XMLData.PTelListen.CALLListe.Where(Function(Tf) Not Tf.NrUnterdrückt)
                            .DocumentElement.AppendChild(CreateDynMenuSplitButton(XDynaMenu, TelFt, XMLData.PTelListen.CALLListe.IndexOf(TelFt), ListName))
                        Next

                    Case My.Resources.strDfltNameListRING

                        For Each TelFt As Telefonat In XMLData.PTelListen.RINGListe.Where(Function(Tf) Not Tf.NrUnterdrückt)
                            .DocumentElement.AppendChild(CreateDynMenuSplitButton(XDynaMenu, TelFt, XMLData.PTelListen.RINGListe.IndexOf(TelFt), ListName))
                        Next

                    Case My.Resources.strDfltNameListVIP

                        For Each VIP As VIPEntry In XMLData.PTelListen.VIPListe
                            .DocumentElement.AppendChild(CreateDynMenuSplitButton(XDynaMenu, VIP, XMLData.PTelListen.VIPListe.IndexOf(VIP), ListName))
                        Next
                End Select

            End With

            NLogger.Trace($"{ListName}: {XDynaMenu.OuterXml}")

            Return XDynaMenu.InnerXml
        End Function

        Private Function CreateDynMenuButton(xDoc As XmlDocument, ID As String, Optional OnActionSuffix As String = "") As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute
            XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

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
            XAttribute.Value = $"BtnOnAction{OnActionSuffix}"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("getImage")
            XAttribute.Value = "GetItemImageMso"
            XButton.Attributes.Append(XAttribute)

            XAttribute = xDoc.CreateAttribute("getScreentip")
            XAttribute.Value = "GetItemScreenTipp"
            XButton.Attributes.Append(XAttribute)

            Return XButton

        End Function

        ''' <summary>
        ''' Erstellt einen SplitButton für das DynamicMenu auf Basis eines Telefonates (RING/CALL -Liste)
        ''' </summary>
        ''' <param name="xDoc">Das Ribbon XML Dokument</param>
        ''' <param name="Tlfnt">Das gegenständliche Telefonat</param>
        ''' <param name="ID">Eine ID, die auf den Index des Telefonates in der Liste verweist.</param>
        ''' <param name="Tag">Eine Zeichenfolge, die auf das ursprüngliche Listenelement verweist.</param>
        ''' <returns>Ein XmlElement, welches in das finale Ribbon-XML eingefügt wird.</returns>
        Private Function CreateDynMenuSplitButton(xDoc As XmlDocument, Tlfnt As Telefonat, ID As Integer, Tag As String) As XmlElement
            Dim XAttribute As XmlAttribute
            Dim XSplitButton As XmlElement = xDoc.CreateElement("splitButton", xDoc.DocumentElement.NamespaceURI)

            ' ID des SplitButtons
            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = $"SplitButtonCRV_{ID}"
            XSplitButton.Attributes.Append(XAttribute)

            XSplitButton.AppendChild(CreateDynMenuButton(xDoc, Tlfnt, ID, Tag))

            With xDoc.CreateElement("menu", xDoc.DocumentElement.NamespaceURI)
                ' ID des SplitButtons-Menues
                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"SplitButtonMenuCRV_{ID}"
                .Attributes.Append(XAttribute)

                ' Erzeuge die Buttons für das Menu
                .AppendChild(CreateDynMenuSplitMenueButton(xDoc, "SceduleButtonCRV", ID, Tag)) ' Terminerstellung
                .AppendChild(CreateDynMenuSplitMenueButton(xDoc, "DeleteEntryButtonCRV", ID, Tag)) ' Eintrag löschem

                ' Füge das Menu hinzu
                XSplitButton.AppendChild(.Clone)

            End With

            Return XSplitButton
        End Function

        ''' <summary>
        ''' Erstellt einen SplitButton für das DynamicMenu auf Basis eines VIP-Eintrages
        ''' </summary>
        ''' <param name="xDoc">Das Ribbon XML Dokument</param>
        ''' <param name="VIP">Der gegenständliche VIP-Eintrag</param>
        ''' <param name="ID">Eine ID, die auf den Index des VIP-Eintrages in der Liste verweist.</param>
        ''' <param name="Tag">Eine Zeichenfolge, die auf das ursprüngliche Listenelement verweist.</param>
        ''' <returns>Ein XmlElement, welches in das finale Ribbon-XML eingefügt wird.</returns>
        Private Function CreateDynMenuSplitButton(xDoc As XmlDocument, VIP As VIPEntry, ID As Integer, Tag As String) As XmlElement
            Dim XAttribute As XmlAttribute
            Dim XSplitButton As XmlElement = xDoc.CreateElement("splitButton", xDoc.DocumentElement.NamespaceURI)

            ' ID des SplitButtons
            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = $"SplitButtonCRV_{ID}"
            XSplitButton.Attributes.Append(XAttribute)

            XSplitButton.AppendChild(CreateDynMenuButton(xDoc, VIP, ID, Tag))

            With xDoc.CreateElement("menu", xDoc.DocumentElement.NamespaceURI)
                ' ID des SplitButtons-Menues
                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"SplitButtonMenuCRV_{ID}"
                .Attributes.Append(XAttribute)

                ' Erzeuge die Buttons für das Menu
                .AppendChild(CreateDynMenuSplitMenueButton(xDoc, "DeleteEntryButtonCRV", ID, Tag)) ' Eintrag löschem

                ' Füge das Menu hinzu
                XSplitButton.AppendChild(.Clone)

            End With

            Return XSplitButton
        End Function

        ''' <summary>
        ''' Erstellt einen Button für den SplitButton auf Basis eines Telefonates (RING/CALL -Liste)
        ''' </summary>
        ''' <param name="xDoc">Das Ribbon XML Dokument</param>
        ''' <param name="Tlfnt">Das gegenständliche Telefonat</param>
        ''' <param name="ID">Eine ID, die auf den Index des Telefonates in der Liste verweist.</param>
        ''' <param name="Tag">Eine Zeichenfolge, die auf das ursprüngliche Listenelement verweist.</param>
        ''' <returns>Ein XmlElement, welches in das finale Split-Button-XML eingefügt wird.</returns>
        Private Function CreateDynMenuButton(xDoc As XmlDocument, Tlfnt As Telefonat, ID As Integer, Tag As String) As XmlElement
            Dim XButton As XmlElement
            Dim XAttribute As XmlAttribute

            With Tlfnt

                XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

                ' ID des Buttons
                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"ListCRV_{ID}"
                XButton.Attributes.Append(XAttribute)

                ' angezeigte Text
                XAttribute = xDoc.CreateAttribute("label")
                XAttribute.Value = .NameGegenstelle.XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)

                ' Action des Bottons
                XAttribute = xDoc.CreateAttribute("onAction")
                XAttribute.Value = "BtnOnAction"
                XButton.Attributes.Append(XAttribute)

                ' Tag des Buttons
                XAttribute = xDoc.CreateAttribute("tag")
                XAttribute.Value = $"{Tag}_{ID}".XMLMaskiereZeichen
                XButton.Attributes.Append(XAttribute)

                ' Supertipp des Buttons
                XAttribute = xDoc.CreateAttribute("supertip")
                XAttribute.Value = $"{Localize.resCommon.strTime}: { .ZeitBeginn}{vbCrLf}"
                XAttribute.Value += $"{Localize.resCommon.strTelNr}: { .GegenstelleTelNr.Formatiert}"

                ' Sofern nachfolgende Informationen vorliegen, füge diese dem Supertipp hinzu.
                If .GegenstelleTelNr.AreaCode.IsNotStringNothingOrEmpty Then XAttribute.Value += $"{vbCrLf}{Localize.resCommon.strArea}: {Localize.Länder.ResourceManager.GetString(.GegenstelleTelNr.AreaCode)}"
                If .GegenstelleTelNr.Location.IsNotStringNothingOrEmpty Then XAttribute.Value += $"{vbCrLf}{Localize.resCommon.strLocation}: { .GegenstelleTelNr.Location}"

                XButton.Attributes.Append(XAttribute)

                ' Icon für verpasstes Telefonat
                If Not .Angenommen Then
                    XAttribute = xDoc.CreateAttribute("getImage")
                    XAttribute.Value = "GetItemImageMso"
                    XButton.Attributes.Append(XAttribute)
                End If

            End With

            Return XButton
        End Function

        ''' <summary>
        ''' Erstellt einen Button für den SplitButton auf Basis eines VIP-Eintrages
        ''' </summary>
        ''' <param name="xDoc">Das Ribbon XML Dokument</param>
        ''' <param name="VIP">Der gegenständliche VIP-Eintrag</param>
        ''' <param name="ID">Eine ID, die auf den Index des VIP-Eintrages in der Liste verweist.</param>
        ''' <param name="Tag">Eine Zeichenfolge, die eindeutig den Typ des Eintrages entspricht. </param>
        ''' <returns>Ein XmlElement, welches in das finale Split-Button-XML eingefügt wird.</returns>
        Friend Function CreateDynMenuButton(xDoc As XmlDocument, VIP As VIPEntry, ID As Integer, Tag As String) As XmlElement
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
                    XAttribute.Value = $"{ .OlContact.FullName}{If(.OlContact.CompanyName.IsNotStringNothingOrEmpty, String.Format(" ({0})", .OlContact.CompanyName), String.Empty)}".XMLMaskiereZeichen
                Else
                    XAttribute.Value = String.Format(Localize.resRibbon.VIPListe_Deleted, VIP.Name.XMLMaskiereZeichen)
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

        ''' <summary>
        ''' Erstellt einen Menu-Button für das Menue-Element des SplitButtons
        ''' </summary>
        ''' <param name="xDoc">Das Ribbon XML Dokument</param>
        ''' <param name="ExID">Eine Zeichenfolge, die eindeutig den Typ des Eintrages entspricht. Diese Zeichenfolge wird genutzt um dynamisch das Label, ScreenTip, ImageMSO festzulegen.</param>
        ''' <param name="ID">Eine ID, die auf den Index des VIP-Eintrages in der Liste verweist.</param>
        ''' <param name="Tag">Eine Zeichenfolge, die auf das ursprüngliche Listenelement verweist.</param>
        ''' <returns>Ein XmlElement, welches in das finale Split-Menue-XML eingefügt wird.</returns>
        Private Function CreateDynMenuSplitMenueButton(xDoc As XmlDocument, ExID As String, ID As Integer, Tag As String) As XmlElement
            Dim XAttribute As XmlAttribute

            ' Erzeuge die Buttons für das Menu
            Dim MenueButton As XmlElement = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)
            With MenueButton
                ' ID des ScheduleButtons
                XAttribute = xDoc.CreateAttribute("id")
                XAttribute.Value = $"{ExID}_{ID}"
                .Attributes.Append(XAttribute)

                ' angezeigter Text
                XAttribute = xDoc.CreateAttribute("getLabel")
                XAttribute.Value = "GetItemLabel"
                .Attributes.Append(XAttribute)

                ' ScreenTipp
                XAttribute = xDoc.CreateAttribute("getScreentip")
                XAttribute.Value = "GetItemScreenTipp"
                .Attributes.Append(XAttribute)

                ' Tag des Buttons
                XAttribute = xDoc.CreateAttribute("tag")
                XAttribute.Value = $"{Tag}_{ID}".XMLMaskiereZeichen
                .Attributes.Append(XAttribute)

                ' Image des Bottons
                XAttribute = xDoc.CreateAttribute("getImage")
                XAttribute.Value = "GetItemImageMso"
                .Attributes.Append(XAttribute)

                ' Action des Bottons
                XAttribute = xDoc.CreateAttribute("onAction")
                XAttribute.Value = "BtnOnAction"
                .Attributes.Append(XAttribute)
            End With

            Return MenueButton
        End Function

        Private Function CreateDynMenuSeperator(xDoc As XmlDocument) As XmlElement
            Dim XSeperator As XmlElement
            Dim XAttribute As XmlAttribute

            XSeperator = xDoc.CreateElement("menuSeparator", xDoc.DocumentElement.NamespaceURI)

            XAttribute = xDoc.CreateAttribute("id")
            XAttribute.Value = "separator"
            XSeperator.Attributes.Append(XAttribute)

            Return XSeperator
        End Function
#End Region
    End Module
End Namespace