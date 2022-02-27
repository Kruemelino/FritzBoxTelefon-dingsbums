Imports System.Security
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Threading
Imports Microsoft.Office.Interop.Outlook

Friend Class OptionenService
    Implements IOptionenService

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private _OutlookStoreRootFolder As IEnumerable(Of MAPIFolder)
    Private Function GetOutlookStoreRootFolder() As IEnumerable(Of MAPIFolder) Implements IOptionenService.GetOutlookStoreRootFolder
        ' Ermittle die Wurzelordner jedes Stores
        If _OutlookStoreRootFolder Is Nothing Then
            _OutlookStoreRootFolder = From S In Globals.ThisAddIn.Application.Session.Stores() Select CType(S, Store).GetRootFolder

            NLogger.Debug($"Outlook RootFolder erstmalig eingelesen: {_OutlookStoreRootFolder.Count}")
        End If

        Return _OutlookStoreRootFolder
    End Function

#Region "Design"
    Private Sub UpdateTheme() Implements IOptionenService.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub

    Private Sub ShowDesignTest() Implements IOptionenService.ShowDesignTest
        AddWindow(Of EasyWPFThemeLib.MainWindow)()
    End Sub

    Private Sub ToogleDesign() Implements IOptionenService.ToogleDesign
        ToogleTheme()
    End Sub
#End Region

#Region "Grunddaten"
    Private Function LadeFBoxUser(IPAdresse As String) As ObservableCollectionEx(Of FBoxAPI.User) Implements IOptionenService.LadeFBoxUser

        Dim UserList As New ObservableCollectionEx(Of FBoxAPI.User)
        Dim XMLString As String = String.Empty
        Dim FritzBoxUsers As New FBoxAPI.UserList

        If Globals.ThisAddIn.FBoxTR064.LANConfigSecurity.GetUserList(XMLString) AndAlso DeserializeXML(XMLString, False, FritzBoxUsers) Then
            UserList.AddRange(FritzBoxUsers.UserListe)

            'RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(True))
            NLogger.Trace($"Userliste ermittelt: {XMLString}")
        Else
            'RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(False))
            NLogger.Trace($"Userliste nicht ermittelt")
        End If

        Return UserList
    End Function
#End Region

#Region "Import Telefoniedaten"
    Private Property FritzBoxDaten As Telefonie

    Private Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie)) Implements IOptionenService.Beendet
    Private Event Status As EventHandler(Of String) Implements IOptionenService.Status
    Private Event FBoxAPIStatus As EventHandler(Of String)

    Private Sub StartImport() Implements IOptionenService.StartImport

        ' Neue Telefonie erstellen
        FritzBoxDaten = New Telefonie

        ' Ereignishandler hinzufügen
        AddHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        AddHandler FritzBoxDaten.Status, AddressOf SetStatus

        NLogger.Debug($"Einlesen der Telefoniedaten gestartet")
        ' Starte das Einlesen
        Dispatcher.CurrentDispatcher.BeginInvoke(Sub() If Ping(XMLData.POptionen.ValidFBAdr) Then FritzBoxDaten.GetFritzBoxDaten())

    End Sub

    Private Sub SetStatus(sender As Object, e As String)
        RaiseEvent Status(Me, e)
    End Sub

    Private Sub SetFBoxAPIStatus(sender As Object, e As FBoxAPI.NotifyEventArgs(Of FBoxAPI.LogMessage))
        NLogger.Log(LogLevel.FromOrdinal(e.Value.Level), e.Value.Message)
        SetStatus(sender, e.Value.Message)
    End Sub

    Private Sub FritzBoxDatenImportBeendet()

        ' Signalisiere, das beenden des Einlesens
        RaiseEvent Beendet(Me, New NotifyEventArgs(Of Telefonie)(FritzBoxDaten))

        ' Ereignishandler entfernen
        RemoveHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        RemoveHandler FritzBoxDaten.Status, AddressOf SetStatus

        NLogger.Debug($"Einlesen der Telefoniedaten beendet")
    End Sub

#End Region

#Region "Indizierung"
    Private Function ZähleKontakte(olFolders As List(Of MAPIFolder)) As Integer Implements IOptionenService.ZähleOutlookKontakte
        Dim retval As Integer = 0
        For Each olFolder In olFolders
            retval += olFolder.Items.Count
        Next
        Return retval
    End Function

    Private Function Indexer(olOrdner As MAPIFolder, IndexModus As Boolean, ct As CancellationToken, progress As IProgress(Of Integer)) As Integer

        Dim VerarbeiteteKontakte As Integer = 0

        For Each Item In olOrdner.Items

            If TypeOf Item Is ContactItem Then

                Dim aktKontakt As ContactItem = CType(Item, ContactItem)

                If IndexModus Then
                    IndiziereKontakt(aktKontakt)
                Else
                    DeIndiziereKontakt(aktKontakt)
                End If

                aktKontakt.Speichern

                ReleaseComObject(aktKontakt)

            End If
            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For

            ' Erhöhe Wert für Progressbar
            progress?.Report(1)

            VerarbeiteteKontakte += 1
        Next

        NLogger.Info($"{If(IndexModus, "Indizierung", "Deindizierung")} des Ordners {olOrdner.Name} ist abgeschlossen ({VerarbeiteteKontakte} Kontakte verarbeitet).")

        ReleaseComObject(olOrdner)
        Return VerarbeiteteKontakte
    End Function

    Private Async Function Indexer(OrdnerListe As List(Of MAPIFolder), IndexModus As Boolean, ct As CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IOptionenService.Indexer

        Dim IndexTasks As New List(Of Task(Of Integer))

        ' Verarbeite alle Ordner die der Kontaktsuche entsprechen
        For Each Ordner In OrdnerListe
            NLogger.Debug($"{If(IndexModus, "Indiziere", "Deindiziere")} Odner {Ordner.Name}")
            ' Starte das Indizieren
            IndexTasks.Add(Task.Run(Function()
                                        Return Indexer(Ordner, IndexModus, ct, progress)
                                    End Function, ct))

            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For
        Next

        ' Warte den Abschluss der Indizierung ab
        Return (Await Task.WhenAll(IndexTasks)).Sum

    End Function

#End Region

#Region "MicroSIP"
    Private Function GetMicroSIPExecutablePath() As String Implements IOptionenService.GetMicroSIPExecutablePath
        Using MicroSIP As New MicroSIP
            Return MicroSIP.MicroSIPPath
        End Using
    End Function
#End Region

#Region "Test Rückwärtssuche"
    Private Event BeendetRWS As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetRWS

    Private Async Sub StartRWSTest(TelNr As String) Implements IOptionenService.StartRWSTest

        ' Ereignishandler hinzufügen
        AddHandler Rückwärtssuche.Beendet, AddressOf RWSTestBeendet
        AddHandler Rückwärtssuche.Status, AddressOf SetStatus

        NLogger.Debug($"Test der Rückwärtssuche für '{TelNr}' gestartet")

        Await StartRWS(New Telefonnummer With {.SetNummer = TelNr}, False)

    End Sub

    Private Sub RWSTestBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        RaiseEvent BeendetRWS(Me, New NotifyEventArgs(Of Boolean)(e.Value))

        ' Ereignishandler hinzufügen
        RemoveHandler Rückwärtssuche.Beendet, AddressOf RWSTestBeendet
        RemoveHandler Rückwärtssuche.Status, AddressOf SetStatus

        NLogger.Debug($"Test der Rückwärtssuche beendet")
    End Sub
#End Region

#Region "Tellows"
    Private Async Function GetTellowsAccountData(XAuthToken As String) As Task(Of TellowsPartnerInfo) Implements IOptionenService.GetTellowsAccountData
        Using tellows = New Tellows(XAuthToken)
            Return Await tellows.GetTellowsAccountInfo()
        End Using
    End Function

    Private Async Function GetTellowsLiveAPIData(TelNr As String, XAuthToken As String) As Task(Of TellowsResponse) Implements IOptionenService.GetTellowsLiveAPIData
        If TelNr.IsNotStringNothingOrEmpty Then
            Using Tel As New Telefonnummer With {.SetNummer = TelNr}
                If Tel.TellowsNummer.IsNotStringNothingOrEmpty Then
                    Using tellows = New Tellows(XAuthToken)
                        Return Await tellows.GetTellowsLiveAPIData(Tel)
                    End Using
                Else
                    Return Nothing
                End If
            End Using
        Else
            Return Nothing
        End If
    End Function
#End Region

    '#Region "Test Login"
    '    Private Event BeendetLogin As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetLogin

    '    Private Sub StartLoginTest(IPAdresse As String, User As String, Password As SecureString) Implements IOptionenService.StartLoginTest

    '        Dim SessionID As String = String.Empty

    '        RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(Globals.ThisAddIn.FBoxTR064.Deviceconfig.GetSessionID(SessionID)))

    '    End Sub
    '#End Region

#Region "Test Kontaktsuche"
    Private Event BeendetKontaktsuche As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetKontaktsuche

    Private Async Sub StartKontaktsucheTest(TelNr As String) Implements IOptionenService.StartKontaktsucheTest
        ' Ereignishandler hinzufügen
        AddHandler KontaktSucher.Beendet, AddressOf KontaktsucheTestBeendet
        AddHandler KontaktSucher.Status, AddressOf SetStatus

        ' Führe eine Kontaktsuche durch
        Dim oc As ContactItem = Await KontaktSucheTelNr(New Telefonnummer With {.SetNummer = TelNr})
        ' Blende den Kontakt ein
        If oc IsNot Nothing Then oc.Display()
    End Sub

    Private Sub KontaktsucheTestBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        RaiseEvent BeendetKontaktsuche(Me, New NotifyEventArgs(Of Boolean)(e.Value))

        ' Ereignishandler hinzufügen
        RemoveHandler KontaktSucher.Beendet, AddressOf KontaktsucheTestBeendet
        RemoveHandler KontaktSucher.Status, AddressOf SetStatus

        NLogger.Debug($"Test der Kontaktsuche beendet")
    End Sub
#End Region

#Region "Test Anrufmonitor"
    Private Async Sub StartAnrMonTest(TelNr As String, CONNECT As Boolean, rnd As Boolean, rndOutlook As Boolean, rndFBox As Boolean, rndTellows As Boolean, clir As Boolean) Implements IOptionenService.StartAnrMonTest
        Dim RndGen As New Random()

        If TelNr.IsStringNothingOrEmpty Then

            If rnd Then
                ' Generiere eine zufällige Telefonnummer aus Deutschland
                Dim LKZ As Landeskennzahl = Globals.ThisAddIn.PVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(L) L.Landeskennzahl = "49")
                Dim OKZ As Integer = RndGen.Next(0, LKZ.Ortsnetzkennzahlen.Count)
                Dim Nr As Integer = RndGen.Next(9999, 9999999)

                TelNr = $"0{LKZ.Ortsnetzkennzahlen.Item(OKZ).Ortsnetzkennzahl}{Nr}"
            End If

            ' Telefonnummer aus Outlookkontakten
            If rndOutlook Then
                ' Ermittle einen Kontakt aus den durchsuchten Ordnern
                ' Ermittle Ordner
                Dim OrdnerListe As List(Of OutlookOrdner) = XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)

                ' Füge den Standardkontaktordner hinzu, falls keine anderen Ordner definiert wurden.
                If Not OrdnerListe.Any Then
                    OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
                End If

                ' Erstelle eine Liste aller Kontakte, welche durchsucht werden sollen
                Dim OLC As New List(Of ContactItem)
                OrdnerListe.ForEach(Sub(F) OLC.AddRange(F.MAPIFolder.Items.Cast(Of ContactItem)))

                ' Ermittle einen zufälligen Kontakt
                Dim C As ContactItem = OLC.Item(RndGen.Next(0, OLC.Count))

                ' Ermittle eine zufällige Telefonnummer des Kontaktes
                Dim NL = C.GetKontaktTelNrList
                If NL.Any Then
                    TelNr = NL.Item(RndGen.Next(0, NL.Count)).Unformatiert
                Else
                    TelNr = String.Empty
                End If

                OLC.ForEach(Sub(Co) ReleaseComObject(Co))
            End If

            ' Telefonnummer aus Fritz!Box Telefonbüchern
            If rndFBox Then
                If Globals.ThisAddIn.PhoneBookXML Is Nothing OrElse Globals.ThisAddIn.PhoneBookXML.First.Phonebook Is Nothing Then
                    ' Wenn die Telefonbücher noch nicht heruntergeladen wurden, oder nur die Namen bekannt sind (Header-Daten),
                    ' Dann lade die Telefonbücher herunter
                    NLogger.Debug($"Die Telefonbücher sind für die Kontaktsuche nicht bereit. Beginne sie herunterzuladen...")
                    Globals.ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeTelefonbücher()
                End If

                Dim FBC As New List(Of FBoxAPI.Contact)
                Globals.ThisAddIn.PhoneBookXML.ToList.ForEach(Sub(F) FBC.AddRange(F.Phonebook.Contacts.Where(Function(T) Not T.IstTelefon)))

                ' Ermittle einen zufälligen Kontakt
                Dim C As FBoxAPI.Contact = FBC.Item(RndGen.Next(0, FBC.Count))

                Dim NL = C.GetKontaktTelNrList
                TelNr = NL.Item(RndGen.Next(0, NL.Count)).Unformatiert
            End If

            ' Telefonnummer aus Tellows
            If rndTellows Then
                If Globals.ThisAddIn.TellowsScoreList Is Nothing Then
                    Using tellows As New Tellows
                        Globals.ThisAddIn.TellowsScoreList = Await tellows.LadeScoreList
                    End Using
                End If

                If Globals.ThisAddIn.TellowsScoreList IsNot Nothing Then
                    Using t As New Telefonnummer With {.SetNummer = Globals.ThisAddIn.TellowsScoreList.Item(RndGen.Next(0, Globals.ThisAddIn.TellowsScoreList.Count)).Number}
                        TelNr = t.Unformatiert
                    End Using
                End If
            End If

            ' Unterdrückte Telefonnummer
            If clir Then TelNr = ""
        Else
            ' Ermittle die unformatierte Nummer der Nutzereingabe
            Using t As New Telefonnummer With {.SetNummer = TelNr}
                TelNr = t.Unformatiert
            End Using
        End If

        ' Starte ein eingehendes Telefonat
        Dim AktivesTelefonat = New Telefonat With {.SetAnrMonRING = {Now.ToString("G"), "RING", "99", TelNr, XMLData.PTelefonie.Telefonnummern(RndGen.Next(0, XMLData.PTelefonie.Telefonnummern.Count)).Einwahl, "SIP4"}}

        ' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
        If CONNECT Then AktivesTelefonat.SetAnrMonCONNECT = {Now.ToString("G"), "CONNECT", "99", $"{XMLData.PTelefonie.Telefoniegeräte(RndGen.Next(0, XMLData.PTelefonie.Telefoniegeräte.Count)).AnrMonID}", TelNr}

        ' 23.06.18 13:20:52;DISCONNECT;1;9;
        AktivesTelefonat.SetAnrMonDISCONNECT = {Now.ToString("G"), "DISCONNECT", "99", "60"}

        RndGen = Nothing
    End Sub

#End Region
End Class
