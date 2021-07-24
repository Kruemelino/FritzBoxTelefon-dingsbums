Imports System.Threading.Tasks
Imports Microsoft.Office.Interop

Friend Module KontaktSucher

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String))
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Boolean))

#Region "Kontaktsuche DASL in Ordnerauswahl"
    Friend Async Function KontaktSucheTaskDASL(TelNr As Telefonnummer) As Task(Of Outlook.ContactItem)
        Dim Filter As List(Of String)
        Dim sFilter As String
        Dim olKontakt As Outlook.ContactItem = Nothing

        If ThisAddIn.OutookApplication IsNot Nothing Then

            If TelNr IsNot Nothing Then
                PushStatus(LogLevel.Debug, $"Kontaktsuche für {TelNr.Unformatiert} gestartet")

                ' Filter zusammenstellen
                Filter = New List(Of String)

                For Each DASLTag As String In DASLTagTelNrIndex.ToList
                    Filter.Add($"{DASLTag}/0x0000001f = '{TelNr.Unformatiert}'")
                Next
                sFilter = $"@SQL={String.Join(" OR ", Filter)}"

                Dim OrdnerListe As List(Of OutlookOrdner) = XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)

                ' Füge den Standardkontaktordner hinzu, falls keine anderen Ordner definiert wurden.
                If Not OrdnerListe.Any Then
                    OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(Outlook.OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
                End If

                Dim TaskList As New List(Of Task(Of Outlook.ContactItem))

                ' Erzeuge eine Liste der Ordner, die der Nutzer ausgewählt hat
                Dim MAPIFolderList As List(Of Outlook.MAPIFolder) = OrdnerListe.Select(Function(S) S.MAPIFolder).ToList

                ' Füge die Unterordner hinzu
                If XMLData.POptionen.CBSucheUnterordner Then
                    PushStatus(LogLevel.Debug, "Ermittle Unterordner...")
                    AddChildFolders(MAPIFolderList, Outlook.OlItemType.olContactItem)
                End If

                PushStatus(LogLevel.Debug, $"Starte die Kontaktsuche in {MAPIFolderList.Count} Ordnern.")
                ' Führe die Kontaktsuche aus.
                For Each MapiFolder In MAPIFolderList

                    TaskList.Add(Task.Run(Function() As Outlook.ContactItem
                                              PushStatus(LogLevel.Debug, $"Kontaktsuche in MAPIFolder '{MapiFolder.Name}' gestartet")
                                              Return FindeAnruferKontaktAuswahl(MapiFolder, sFilter)
                                          End Function))
                Next

                While TaskList.Any 'And olKontakt Is Nothing
                    Dim t = Await Task.WhenAny(TaskList)

                    If t.Result IsNot Nothing Then
                        If olKontakt Is Nothing Then olKontakt = t.Result
                        PushStatus(LogLevel.Debug, $"Kontaktsuche erfolgreich: '{olKontakt.FullName}' in '{olKontakt.ParentFolder.Name}' gefunden.")
                    End If

                    TaskList.Remove(t)
                End While

            End If
        End If

        PushStatus(LogLevel.Debug, $"Kontaktsuche beendet.")
        RaiseEvent Beendet(Nothing, New NotifyEventArgs(Of Boolean)(olKontakt IsNot Nothing))

        Return olKontakt
    End Function

    Private Function FindeAnruferKontaktAuswahl(Ordner As Outlook.MAPIFolder, sFilter As String) As Outlook.ContactItem

        Dim olKontakt As Outlook.ContactItem = Nothing

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

            Dim oTable As Outlook.Table
            ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
            ' Erstellung der Datentabelle
            oTable = Ordner.GetTable(sFilter)
            ' Festlegung der Spalten. Zunächst werden alle Spalten entfernt
            With oTable.Columns
                .RemoveAll()
                .Add("EntryID")
            End With

            If Not oTable.EndOfTable Then
                olKontakt = GetOutlookKontakt(oTable.GetNextRow("EntryID").ToString, Ordner.StoreID)
                NLogger.Debug($"DASL Table erfolgreich: {olKontakt.FullNameAndCompany} in {Ordner.Name}")
            End If
            ReleaseComObject(oTable)
        End If

        Return olKontakt
    End Function '(FindeKontakt)

    ''' <summary>
    ''' Gibt eine Statusmeldung (<paramref name="StatusMessage"/>) als Event aus. Gleichzeitig wird in das Log mit vorgegebenem <paramref name="Level"/> geschrieben.
    ''' </summary>
    ''' <param name="Level">NLog LogLevel</param>
    ''' <param name="StatusMessage">Die auszugebende Statusmeldung.</param>
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Nothing, New NotifyEventArgs(Of String)(StatusMessage))
    End Sub
#End Region

#Region "Absendersuche E-Mail"

    ''' <summary>
    ''' Funktion die die Suche mit einer E-Mail durchführt.
    ''' </summary>
    ''' <param name="SMTPAdresse">Mail-Addresse, die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als Outlook.ContactItem.</returns>
    Friend Function KontaktSuche(SMTPAdresse As EMailType) As Outlook.ContactItem

        If SMTPAdresse.Addresse.IsNotStringEmpty Then
            ' Empfänger generieren
            With ThisAddIn.OutookApplication.Session.CreateRecipient(SMTPAdresse.Addresse)
                .Resolve()
                Return .AddressEntry.GetContact
            End With
        Else
            Return Nothing
        End If
    End Function

    Friend Function KontaktSucheExchangeUser(SMTPAdresse As EMailType) As Outlook.ExchangeUser

        If SMTPAdresse.Addresse.IsNotStringEmpty Then
            ' Empfänger generieren
            With ThisAddIn.OutookApplication.Session.CreateRecipient(SMTPAdresse.Addresse)
                .Resolve()
                Return .AddressEntry.GetExchangeUser
            End With
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Funktion die die Suche mit einer Kontaktkarte durchführt.
    ''' </summary>
    ''' <param name="Kontaktkarte">Kontaktkarte (ContactCard), die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als Outlook.ContactItem</returns>
    Friend Function KontaktSuche(Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As Outlook.ContactItem

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSuche(New EMailType With {.Addresse = Kontaktkarte.Address, .OutlookTyp = OutlookEMailType.SMTP})

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As Outlook.AddressEntry = ThisAddIn.OutookApplication.Session.GetAddressEntryFromID(Kontaktkarte.Address)

                    If Adresseintrag?.AddressEntryUserType = Outlook.OlAddressEntryUserType.olOutlookContactAddressEntry Then
                        Return Adresseintrag.GetContact
                    Else
                        Return Nothing
                    End If

                    ReleaseComObject(Adresseintrag)

                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If
        ReleaseComObject(Kontaktkarte)

    End Function

    ''' <summary>
    ''' Funktion die die Suche mit einer Kontaktkarte durchführt.
    ''' </summary>
    ''' <param name="Kontaktkarte">Kontaktkarte (ContactCard), die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als Outlook.ExchangeUser</returns>
    Friend Function KontaktSucheExchangeUser(Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As Outlook.ExchangeUser

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSucheExchangeUser(New EMailType With {.Addresse = Kontaktkarte.Address, .OutlookTyp = OutlookEMailType.EX})

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As Outlook.AddressEntry = ThisAddIn.OutookApplication.Session.GetAddressEntryFromID(Kontaktkarte.Address)

                    Select Case Adresseintrag?.AddressEntryUserType
                        Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
                            Return Adresseintrag.GetExchangeUser()
                        Case Else
                            Return Nothing
                    End Select

                    ReleaseComObject(Adresseintrag)
                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If
        ReleaseComObject(Kontaktkarte)
    End Function

#End Region

End Module
