Imports Microsoft.Office.Interop.Outlook
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks

Friend Module KontaktSync
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Synchronisiert einen Kontaktordner <paramref name="OutlookOrdner"/> mit einem Fritz!Box Telefonbuch (<paramref name="FBoxTBuch"/>)
    ''' </summary>
    ''' <param name="OutlookOrdner">Der zu synchrinisierende Outlook Ordner</param>
    ''' <param name="FBoxTBuch">Das zu synchrinisierende Fritz!Box Telefonbuch</param>
    ''' <param name="Modus">Der Synchronisationsmodus. Hier wird festgelegt, in welche Richtung die Daten bei Änderungen verschoben werden.</param>
    ''' <param name="ct">CancellationToken zum Abbruch der Routine</param>
    ''' <param name="Progress">Anbieter für Statusupdates</param>
    ''' <returns></returns>
    Friend Async Function Synchronisierer(OutlookOrdner As MAPIFolder, FBoxTBuch As PhonebookEx, Modus As SyncMode, ct As CancellationToken, Progress As IProgress(Of String)) As Task(Of Integer)

        Dim VerarbeiteteKontakte As Integer = 0

        Dim TaskList As New List(Of Task(Of String))

        Dim FBKontakte As New List(Of FBoxAPI.Contact)
        FBKontakte.AddRange(FBoxTBuch.GetContacts)

        ' Schleife durch jedes Element dieses Ordners. 
        For Each Item In OutlookOrdner.Items

            Select Case True
                ' Unterscheidung je nach Datentyp
                Case TypeOf Item Is ContactItem

                    Dim aktKontakt As ContactItem = CType(Item, ContactItem)

                    ' Synchronisiere Kontakt
                    With aktKontakt
                        Dim uID As Integer = .GetUniqueID(FBoxTBuch.ID)
                        If uID.AreEqual(-1) Then
                            Progress?.Report($"Kontakt '{ .FullName}' auf der Fritz!Box erzeugt ...")
                            ' Es gibt keinen Kontakt auf der Fritz!Box
                            TaskList.Add(Task.Run(Function() Telefonbücher.SetTelefonbuchEintrag(FBoxTBuch.ID, aktKontakt)))
                        Else
                            ' Es gibt einen Kontakt im Fritz!Box Telefonbuch
                            Dim FBoxKontakt As FBoxAPI.Contact = FBoxTBuch.GetContact(uID)
                            ' Gibt es in dem Telefonbuch einen Kontakt mit der ID
                            If FBoxKontakt IsNot Nothing Then
                                ' Ja ... Abgleich
                                If Not .IsEqual(FBoxKontakt) Then
                                    Select Case Modus
                                        Case SyncMode.OutlookToFritzBox
                                            Progress?.Report($"Kontakt '{ .FullName}' auf der Fritz!Box überschrieben (uID {FBoxKontakt.Uniqueid}) ...")

                                            ' Kontakt auf der Fritz!Box ersetzen
                                            TaskList.Add(Task.Run(Function() Telefonbücher.SetTelefonbuchEintrag(FBoxTBuch.ID, aktKontakt)))

                                        Case SyncMode.FritzBoxToOutlook
                                            Progress?.Report($"Kontakt '{ .FullName}' in Outlook überschrieben (uID {FBoxKontakt.Uniqueid}) ...")

                                            TaskList.Add(Task.Run(Function() ÜberschreibeKontakt(aktKontakt, FBoxKontakt)))

                                    End Select

                                End If
                            Else
                                ' Nein ... Kontakt wurde auf der Fritz!Box gelöscht?
                                Progress?.Report($"Kontakt '{ .FullName}' im Outlook gelöscht ...")
                                .Delete()

                            End If

                            ' Entferne den Kontakt aus dem heruntergeladenen Telefonbuch
                            FBKontakte.Remove(FBoxKontakt)
                        End If

                        ' Erhöhe Wert für Progressbar und schreibe einen Status
                        Progress?.Report($"Kontakt '{ .FullName}' abgeschlossen ...")
                    End With

                    aktKontakt = Nothing

                Case TypeOf Item Is AddressList ' Adressliste
                    With CType(Item, AddressList)
                        Progress?.Report($"Adressliste '{ .Name}' übergangen ...")
                    End With

                Case TypeOf Item Is DistListItem ' Verteilerliste
                    With CType(Item, DistListItem)
                        Progress?.Report($"Verteilerliste '{ .DLName}' übergangen ...")
                    End With

                Case Else
                    Progress?.Report($"Unbekanntes Objekt übergangen ...")

            End Select

            ' Die einzelnen Vorgänge müssen nacheinander erfolgen, da es sonst zu einer WebException kommt: Die zugrunde liegende Verbindung wurde geschlossen: Für den geschützten SSL/TLS-Kanal konnte keine Vertrauensstellung hergestellt werden.
            If TaskList.Any Then Await TaskList.Last

            ReleaseComObject(Item)
            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For

            VerarbeiteteKontakte += 1
        Next

        ' Alle Kontakte, welche jetzt noch im Telefonbuch sind, müssen im Outlook angelegt werden
        For Each FBoxKontakt In FBKontakte
            Select Case Modus
                Case SyncMode.OutlookToFritzBox
                    With FBoxKontakt
                        Telefonbücher.DeleteTelefonbuchEintrag(FBoxTBuch.ID, .Uniqueid)
                        Progress?.Report($"Kontakt '{ .Person.RealName}' auf der Fritz!Box gelöscht (uID { .Uniqueid}) ...")
                    End With

                Case SyncMode.FritzBoxToOutlook

                    With ErstelleKontakt(FBoxKontakt, OutlookOrdner, FBoxTBuch.ID)
                        Progress?.Report($"Kontakt '{ .FullName}' in Outlook erzeugt (uID {FBoxKontakt.Uniqueid}) ...")
                    End With

            End Select

            VerarbeiteteKontakte += 1
        Next

        Return VerarbeiteteKontakte
    End Function

    ''' <summary>
    ''' Synchronisiert einen einzelnen Outlook Kontakt <paramref name="olContact"/> mit einem Fritz!Box Telefonbuch.
    ''' </summary>
    ''' <param name="olContact">Der zu synchrinisierende Outlook Kontakt</param>
    ''' <param name="olFolder">Der Ordner in dem sich der zu synchrinisierende Outlook Kontakt befindet</param>
    <Extension> Friend Async Sub Synchronisierer(olContact As ContactItem, olFolder As MAPIFolder)

        Dim olOrdner As OutlookOrdner = XMLData.POptionen.OutlookOrdner.Find(olFolder, OutlookOrdnerVerwendung.FBoxSync)

        If olOrdner?.FBoxSyncOptions?.ValidData IsNot Nothing Then

            ' Synchronisiere Kontakt

            With olContact
                Dim uID As Integer = .GetUniqueID(olOrdner.FBoxSyncOptions.FBoxSyncID)
                If uID.AreEqual(-1) Then
                    NLogger.Info($"Kontakt '{ .FullName}' auf der Fritz!Box erzeugt ...")
                    ' Es gibt keinen Kontakt auf der Fritz!Box
                    Telefonbücher.SetTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, olContact)
                Else
                    ' Es gibt einen Kontakt im Fritz!Box Telefonbuch
                    Dim FBoxKontakt As FBoxAPI.Contact = Await Telefonbücher.GetTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, uID)
                    ' Gibt es in dem Telefonbuch einen Kontakt mit der ID
                    If FBoxKontakt IsNot Nothing Then
                        ' Ja ... Abgleich
                        If Not .IsEqual(FBoxKontakt) Then

                            NLogger.Info($"Kontakt '{ .FullName}' auf der Fritz!Box überschrieben (uID {FBoxKontakt.Uniqueid}) ...")

                            ' Kontakt auf der Fritz!Box ersetzen
                            Telefonbücher.SetTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, olContact)

                        End If
                    Else
                        ' Nein ... Kontakt wurde auf der Fritz!Box gelöscht?
                        NLogger.Info($"Kontakt '{ .FullName}' im Outlook gelöscht ...")
                        .Delete()

                    End If

                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' Löscht einen einzelnen olContact auf dem Fritz!Box Telefonbuch, wenn der Kontakt im Outlook gelöscht wurde.
    ''' </summary>
    ''' <param name="olContact">Der zu löschende Kontakt.</param>
    <Extension> Friend Sub SyncDelete(olContact As ContactItem)
        Dim olOrdner As OutlookOrdner = XMLData.POptionen.OutlookOrdner.Find(olContact.ParentFolder, OutlookOrdnerVerwendung.FBoxSync)

        If olOrdner?.FBoxSyncOptions?.ValidData IsNot Nothing Then

            ' Lösche Kontakt auf der Fritz!Box (wenn vorhanden)

            With olContact
                Dim uID As Integer = .GetUniqueID(olOrdner.FBoxSyncOptions.FBoxSyncID)
                If Not uID.AreEqual(-1) Then
                    ' Es gibt einen Kontakt im Fritz!Box Telefonbuch
                    Telefonbücher.DeleteTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, uID)
                    NLogger.Info($"Kontakt '{olContact.FullName}' auf der Fritz!Box gelöscht (uID { uID}) ...")
                End If
            End With
        End If
    End Sub

    Friend Async Sub StartAutoSync()
        Dim progressIndicator = New Progress(Of String)(Sub(status)
                                                            NLogger.Info(status)
                                                        End Sub)

        Dim TaskList As New List(Of Task(Of Integer))

        For Each Ordner In XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.FBoxSync).Where(Function(O) (O.FBoxSyncOptions.FBoxCBSyncStartUp))

            Dim FBoxTelefonbuch As PhonebookEx = Globals.ThisAddIn.PhoneBookXML.Where(Function(TB) TB.ID.AreEqual(Ordner.FBoxSyncOptions.FBoxSyncID)).First
            If FBoxTelefonbuch IsNot Nothing Then

                If Not FBoxTelefonbuch.NurName OrElse Not Await FBoxTelefonbuch.UpdatePhonebook() Then
                    NLogger.Info($"Starte die automatische Syncronisation des Outlook-Ordners {Ordner.Name} mit {FBoxTelefonbuch.Name}")

                    TaskList.Add(Task.Run(Function() Synchronisierer(Ordner.MAPIFolder, FBoxTelefonbuch, SyncMode.FritzBoxToOutlook, Nothing, progressIndicator)))
                End If
            End If

            ' Die einzelnen Vorgänge müssen nacheinander erfolgen, da es sonst zu einer WebException kommt: Die zugrunde liegende Verbindung wurde geschlossen: Für den geschützten SSL/TLS-Kanal konnte keine Vertrauensstellung hergestellt werden.
            If TaskList.Any Then Await TaskList.Last
        Next
        NLogger.Info($"Automatische Syncronisation abgeschlossen: {(Await Task.WhenAll(TaskList)).Sum}")

    End Sub

End Module
