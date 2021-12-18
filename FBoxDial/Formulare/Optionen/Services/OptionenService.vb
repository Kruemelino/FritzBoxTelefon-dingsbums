Imports System.Security
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Threading
Imports Microsoft.Office.Interop.Outlook

Friend Class OptionenService
    Implements IOptionenService

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Grunddaten"
    Friend Function LadeFBoxUser(IPAdresse As String) As ObservableCollectionEx(Of FBoxAPI.User) Implements IOptionenService.LadeFBoxUser

        Dim UserList As New ObservableCollectionEx(Of FBoxAPI.User)
        '' Prüfe, ob Fritz!Box verfügbar
        'If Ping(IPAdresse) Then
        Using FBoxTr064 As New FBoxAPI.FritzBoxTR64(IPAdresse, XMLData.POptionen.TBNetworkTimeout, Nothing)
            AddHandler FBoxTr064.Status, AddressOf SetFBoxAPIStatus

            Dim XMLString As String = String.Empty
            Dim FritzBoxUsers As New FBoxAPI.UserList

            If FBoxTr064.LANConfigSecurity.GetUserList(XMLString) AndAlso DeserializeXML(XMLString, False, FritzBoxUsers) Then
                UserList.AddRange(FritzBoxUsers.UserListe)

                RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(True))
            Else
                RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(False))
            End If

            RemoveHandler FBoxTr064.Status, AddressOf SetFBoxAPIStatus

        End Using
        'Else
        '    NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        'End If

        Return UserList
    End Function


#End Region

#Region "Import Telefoniedaten"
    Private Property FritzBoxDaten As Telefonie
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie)) Implements IOptionenService.Beendet
    Friend Event Status As EventHandler(Of String) Implements IOptionenService.Status
    Friend Event FBoxAPIStatus As EventHandler(Of String)

    Friend Sub StartImport() Implements IOptionenService.StartImport

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
    Public Function ZähleKontakte(olFolders As List(Of MAPIFolder)) As Integer Implements IOptionenService.ZähleOutlookKontakte
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

    Friend Async Function Indexer(OrdnerListe As List(Of MAPIFolder), IndexModus As Boolean, ct As CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IOptionenService.Indexer

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
    Public Function GetMicroSIPExecutablePath() As String Implements IOptionenService.GetMicroSIPExecutablePath
        Using MicroSIP As New MicroSIP
            Return MicroSIP.MicroSIPPath
        End Using
    End Function
#End Region

#Region "Test Rückwärtssuche"
    Friend Event BeendetRWS As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetRWS

    Friend Async Sub StartRWSTest(TelNr As String) Implements IOptionenService.StartRWSTest

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
    Public Async Function GetTellowsAccountData(XAuthToken As String) As Task(Of TellowsPartnerInfo) Implements IOptionenService.GetTellowsAccountData
        Using tellows = New Tellows(XAuthToken)
            Return Await tellows.GetTellowsAccountInfo()
        End Using
    End Function

    Public Async Function GetTellowsLiveAPIData(TelNr As String, XAuthToken As String) As Task(Of TellowsResponse) Implements IOptionenService.GetTellowsLiveAPIData
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

#Region "Test Login"
    Public Event BeendetLogin As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetLogin

    Friend Sub StartLoginTest(IPAdresse As String, User As String, Password As SecureString) Implements IOptionenService.StartLoginTest
        '' Prüfe, ob Fritz!Box verfügbar
        'If Ping(IPAdresse) Then
        Using fboxTR064 As New FBoxAPI.FritzBoxTR64(IPAdresse, XMLData.POptionen.TBNetworkTimeout, New Net.NetworkCredential(User, Password))
            AddHandler fboxTR064.Status, AddressOf SetFBoxAPIStatus

            Dim SessionID As String = String.Empty

            RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(fboxTR064.Deviceconfig.GetSessionID(SessionID)))

            RemoveHandler fboxTR064.Status, AddressOf SetFBoxAPIStatus
        End Using
        'Else
        '    NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        'End If
    End Sub
#End Region

#Region "Test Kontaktsuche"
    Public Event BeendetKontaktsuche As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetKontaktsuche

    Friend Async Sub StartKontaktsucheTest(TelNr As String) Implements IOptionenService.StartKontaktsucheTest
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
    Public Sub StartAnrMonTest(TelNr As String, CONNECT As Boolean) Implements IOptionenService.StartAnrMonTest
        Dim rnd As New Random()

        Dim D As Landeskennzahl = ThisAddIn.PVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(LKZ) LKZ.Landeskennzahl = "49")
        Dim i As Integer = rnd.Next(0, D.Ortsnetzkennzahlen.Count)
        Dim N As Integer = rnd.Next(9999, 9999999)

        If TelNr.IsStringNothingOrEmpty Then
            TelNr = $"0{D.Ortsnetzkennzahlen.Item(i).Ortsnetzkennzahl}{N}"
        Else
            Using t As New Telefonnummer With {.SetNummer = TelNr}
                TelNr = t.Unformatiert
            End Using
        End If

        Dim AktivesTelefonat = New Telefonat With {.SetAnrMonRING = {"23.06.18 13:20:24", "RING", "1", TelNr, XMLData.PTelefonie.Telefonnummern(rnd.Next(0, XMLData.PTelefonie.Telefonnummern.Count)).Einwahl, "SIP4"}}

        ' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
        If CONNECT Then AktivesTelefonat.SetAnrMonCONNECT = {"23.06.18 13:20:52", "CONNECT", "1", $"{XMLData.PTelefonie.Telefoniegeräte(rnd.Next(0, XMLData.PTelefonie.Telefoniegeräte.Count)).AnrMonID}", TelNr}

        ' 23.06.18 13:20:52;DISCONNECT;1;9;
        AktivesTelefonat.SetAnrMonDISCONNECT = {"23.06.18 13:20:44", "DISCONNECT", "1", "9"}
    End Sub
#End Region
End Class
