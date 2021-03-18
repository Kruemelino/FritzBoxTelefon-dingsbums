Imports Microsoft.Office.Interop

Friend Module KontaktSucher

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Function KontaktSuche(TelNr As Telefonnummer) As Outlook.ContactItem
        NLogger.Debug($"Starte Kontaktsuche in den Outlook Kontakten für Telefonnummer '{TelNr.Unformatiert}'.")

        Return KontaktSucheAuswahlDASL(TelNr)

    End Function

#Region "Kontaktsuche DASL in Ordnerauswahl"
    Friend Function KontaktSucheAuswahlDASL(TelNr As Telefonnummer) As Outlook.ContactItem
        Dim Filter As List(Of String)
        Dim sFilter As String
        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Integer

        If ThisAddIn.OutookApplication IsNot Nothing Then

            If TelNr IsNot Nothing Then
                ' Filter zusammenstellen
                Filter = New List(Of String)

                For Each DASLTag As String In DASLTagTelNrIndex.ToList
                    Filter.Add($"{DASLTag}/0x0000001f = '{TelNr.Unformatiert}'")
                Next
                sFilter = $"@SQL={String.Join(" OR ", Filter)}"


                With XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)
                    ' Füge den Standardkontaktordner hinzu, falls keine anderen Ordner definiert wurden.
                    If Not .Any Then
                        .Add(New OutlookOrdner(XMLData.POptionen.OutlookOrdner.GetDefaultMAPIFolder(Outlook.OlDefaultFolders.olFolderContacts),
                                               OutlookOrdnerVerwendung.KontaktSuche))
                    End If

                    Dim Ordner As OutlookOrdner
                    iOrdner = 0
                    Do While (iOrdner.IsLess(.Count)) And (olKontakt Is Nothing)
                        Ordner = .Item(iOrdner)

                        ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
                        olKontakt = FindeAnruferKontaktAuswahl(Ordner.MAPIFolder, sFilter)

                        ' Rekursive Suche der Unterordner
                        If olKontakt Is Nothing And XMLData.POptionen.CBSucheUnterordner Then
                            For Each Unterordner As Outlook.MAPIFolder In Ordner.MAPIFolder.Folders
                                olKontakt = FindeAnruferKontaktAuswahl(Unterordner, sFilter)
                                Unterordner.ReleaseComObject
                            Next
                        End If
                        iOrdner += 1
                    Loop

                End With

            End If
        End If
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
            oTable.ReleaseComObject
        End If


        Return olKontakt
    End Function '(FindeKontakt)
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

                    Adresseintrag.ReleaseComObject

                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If
        Kontaktkarte.ReleaseComObject

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

                    Adresseintrag.ReleaseComObject
                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If
        Kontaktkarte.ReleaseComObject
    End Function

#End Region

End Module
