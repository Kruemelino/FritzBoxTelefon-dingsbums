Imports System.Windows.Threading
Friend Class OptionenService
    Implements IOptionenService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private Property FritzBoxDaten As Telefonie
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie)) Implements IOptionenService.Beendet
    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String)) Implements IOptionenService.Status


    Friend Sub StartImport() Implements IOptionenService.StartImport

        ' Neue Telefonie erstellen
        FritzBoxDaten = New Telefonie

        ' Ereignishandler hinzufügen
        AddHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        AddHandler FritzBoxDaten.Status, AddressOf FritzBoxDatenStatus

        NLogger.Debug($"Einlesen der Telefoniedaten gestartet")

        Dispatcher.CurrentDispatcher.BeginInvoke(Sub()
                                                     ' Starte das Einlesen
                                                     If Ping(XMLData.POptionen.ValidFBAdr) Then
                                                         FritzBoxDaten.GetFritzBoxDaten()
                                                     End If
                                                 End Sub)


    End Sub

    Private Sub FritzBoxDatenStatus(sender As Object, e As NotifyEventArgs(Of String))
        RaiseEvent Status(Me, e)
    End Sub

    Private Sub FritzBoxDatenImportBeendet()

        ' Signalisiere, das beenden des Einlesens
        RaiseEvent Beendet(Me, New NotifyEventArgs(Of Telefonie)(FritzBoxDaten))

        ' Ereignishandler entfernen
        RemoveHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        RemoveHandler FritzBoxDaten.Status, AddressOf FritzBoxDatenStatus

        NLogger.Debug($"Einlesen der Telefoniedaten beendet")
    End Sub
End Class
