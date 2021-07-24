Imports System.Security
Imports System.Windows.Threading
Imports Microsoft.Office.Interop.Outlook

Friend Class OptionenService
    Implements IOptionenService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Grunddaten"
    Friend Function LadeFBoxUser(IPAdresse As String) As ObservableCollectionEx(Of FritzBoxXMLUser) Implements IOptionenService.LadeFBoxUser

        Dim UserList As New ObservableCollectionEx(Of FritzBoxXMLUser)
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(IPAdresse) Then
            Using FBoxTr064 As New SOAP.FritzBoxTR64(IPAdresse, Nothing)
                AddHandler FBoxTr064.Status, AddressOf SetStatus

                Dim XMLString As String = DfltStringEmpty
                Dim FritzBoxUsers As New FritzBoxXMLUserList

                If FBoxTr064.GetUserList(XMLString) AndAlso DeserializeXML(XMLString, False, FritzBoxUsers) Then
                    UserList.AddRange(FritzBoxUsers.UserListe)

                    RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(True))
                Else
                    RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(False))
                End If

                RemoveHandler FBoxTr064.Status, AddressOf SetStatus
            End Using
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        End If

        Return UserList
    End Function
#End Region

#Region "Import Telefoniedaten"
    Private Property FritzBoxDaten As Telefonie
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie)) Implements IOptionenService.Beendet
    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String)) Implements IOptionenService.Status

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

    Private Sub SetStatus(sender As Object, e As NotifyEventArgs(Of String))
        RaiseEvent Status(Me, e)
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
    Friend Event IndexStatus As EventHandler(Of NotifyEventArgs(Of Integer)) Implements IOptionenService.IndexStatus
    Friend Property CancelationPending As Boolean Implements IOptionenService.CancelationPending

    Public Function ZähleKontakte(olFolder As MAPIFolder) As Integer Implements IOptionenService.ZähleOutlookKontakte
        Return ZähleOutlookKontakte(olFolder)
    End Function

    Friend Sub Indexer(Ordner As MAPIFolder, IndexModus As Boolean, Unterordner As Boolean) Implements IOptionenService.Indexer

        For Each Item In Ordner.Items
            If CancelationPending Then Exit For

            If TypeOf Item Is ContactItem Then

                Dim aktKontakt As ContactItem = CType(Item, ContactItem)

                If IndexModus Then
                    IndiziereKontakt(aktKontakt)
                Else
                    DeIndiziereKontakt(aktKontakt)
                End If

                aktKontakt.Speichern

                aktKontakt.ReleaseComObject

            End If

            ' Erhöhe Wert für Progressbar
            RaiseEvent IndexStatus(Me, New NotifyEventArgs(Of Integer)(1))
        Next

        If Not IndexModus Then
            ' Entfernt alle Indizierungseinträge aus den Ordnern des Kontaktelementes.
            DeIndizierungOrdner(Ordner)
        End If

        NLogger.Info($"{If(IndexModus, "Indizierung", "Deindizierung")} des Ordners {Ordner.Name} ist abgeschlossen.")
    End Sub

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
    Public Async Function GetTellowsAccountData(XAuthToken As String) As Threading.Tasks.Task(Of TellowsPartnerInfo) Implements IOptionenService.GetTellowsAccountData
        Using tellows = New Tellows(XAuthToken)
            Return Await tellows.GetTellowsAccountInfo()
        End Using
    End Function

    Public Async Function GetTellowsLiveAPIData(TelNr As String, XAuthToken As String) As Threading.Tasks.Task(Of TellowsResponse) Implements IOptionenService.GetTellowsLiveAPIData
        Using Tel As New Telefonnummer With {.SetNummer = TelNr}
            If Tel.TellowsNummer.IsNotStringNothingOrEmpty Then
                Using tellows = New Tellows(XAuthToken)
                    Return Await tellows.GetTellowsLiveAPIData(Tel)
                End Using
            Else
                Return Nothing
            End If
        End Using
    End Function
#End Region

#Region "Test Login"
    Public Event BeendetLogin As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetLogin

    Friend Sub StartLoginTest(IPAdresse As String, User As String, Password As SecureString) Implements IOptionenService.StartLoginTest
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(IPAdresse) Then
            Using fboxTR064 As New SOAP.FritzBoxTR64(IPAdresse, New Net.NetworkCredential(User, Password))
                AddHandler fboxTR064.Status, AddressOf SetStatus

                Dim SessionID As String = DfltStringEmpty

                RaiseEvent BeendetLogin(Me, New NotifyEventArgs(Of Boolean)(fboxTR064.GetSessionID(SessionID)))

                RemoveHandler fboxTR064.Status, AddressOf SetStatus
            End Using
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        End If
    End Sub
#End Region

#Region "Test Kontaktsuche"
    Public Event BeendetKontaktsuche As EventHandler(Of NotifyEventArgs(Of Boolean)) Implements IOptionenService.BeendetKontaktsuche

    Friend Async Sub StartKontaktsucheTest(TelNr As String) Implements IOptionenService.StartKontaktsucheTest
        ' Ereignishandler hinzufügen
        AddHandler KontaktSucher.Beendet, AddressOf KontaktsucheTestBeendet
        AddHandler KontaktSucher.Status, AddressOf SetStatus

        Dim oc As ContactItem = Await KontaktSucheTaskDASL(New Telefonnummer With {.SetNummer = TelNr})
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
End Class
