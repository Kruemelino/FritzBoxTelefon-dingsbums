Imports Microsoft.Office.Interop
Imports MixERP.Net.VCards.Types

Friend Module KontaktSucher

    Friend ReadOnly Property PDfltContactFolder() As Outlook.MAPIFolder = ThisAddIn.POutookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
    Private Property NLogger As NLog.Logger = LogManager.GetCurrentClassLogger

    Friend Function KontaktSuche(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
        NLogger.Debug("Kontaktsuche gestartet")

        'If XMLData.POptionen.PCBUseLegacyUserProp Then
        '    Return KontaktSucheUserProp(TelNr)
        'Else
        '    'Return KontaktSucheDASL(TelNr)
        Return KontaktSucheAuswahlDASL(TelNr)
        'End If
    End Function

#Region "Kontaktsuche DASL in Ordnerauswahl"
    Friend Function KontaktSucheAuswahlDASL(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
        Dim Filter As List(Of String)
        Dim sFilter As String
        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Integer

        If ThisAddIn.POutookApplication IsNot Nothing Then

            If TelNr IsNot Nothing Then
                ' Filter zusammenstellen
                Filter = New List(Of String)

                For Each DASLTag As String In DASLTagTelNrIndex.ToList
                    Filter.Add($"{DASLTag}/0x0000001f = '{TelNr.Unformatiert}'")
                Next
                sFilter = $"@SQL={String.Join(" OR ", Filter)}"

                With XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)
                    If?.Any Then
                        Dim Ordner As OutlookOrdner
                        iOrdner = 0
                        Do While (iOrdner.IsLess(.Count)) And (olKontakt Is Nothing)
                            Ordner = .Item(iOrdner)

                            ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
                            olKontakt = FindeAnruferKontaktAuswahl(Ordner.MAPIFolder, sFilter)

                            ' Rekursive Suche der Unterordner
                            If olKontakt Is Nothing And XMLData.POptionen.PCBSucheUnterordner Then
                                For Each Unterordner As Outlook.MAPIFolder In Ordner.MAPIFolder.Folders
                                    olKontakt = FindeAnruferKontaktAuswahl(Unterordner, sFilter)
                                    Unterordner.ReleaseComObject
                                Next
                            End If
                            iOrdner += 1
                        Loop
                    End If
                End With

            End If
        End If
        Return olKontakt
    End Function

    Private Function FindeAnruferKontaktAuswahl(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem

        Dim olKontakt As Outlook.ContactItem = Nothing

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

            If XMLData.POptionen.PCBUseLegacySearch Then
                olKontakt = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
                If olKontakt IsNot Nothing Then
                    NLogger.Debug("DASL Search erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
                End If
            Else
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
                    NLogger.Debug("DASL Table erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
                End If
                oTable.ReleaseComObject
            End If
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
    Friend Function KontaktSuche(ByVal SMTPAdresse As String) As Outlook.ContactItem

        If SMTPAdresse.IsNotStringEmpty Then
            ' Empfänger generieren
            With ThisAddIn.POutookApplication.Session.CreateRecipient(SMTPAdresse)
                .Resolve()
                Return .AddressEntry.GetContact
            End With
        Else
            Return Nothing
        End If
    End Function

    Friend Function KontaktSucheExchangeUser(ByVal SMTPAdresse As String) As Outlook.ExchangeUser

        If SMTPAdresse.IsNotStringEmpty Then
            ' Empfänger generieren
            With ThisAddIn.POutookApplication.Session.CreateRecipient(SMTPAdresse)
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
    ''' <returns>Den gefundenen Kontakt als Outlook.ContactItem.</returns>
    Friend Function KontaktSuche(ByVal Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As Outlook.ContactItem

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSuche(Kontaktkarte.Address)

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As Outlook.AddressEntry = ThisAddIn.POutookApplication.Session.GetAddressEntryFromID(Kontaktkarte.Address)

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
    ''' <returns>Den gefundenen Kontakt als Outlook.ExchangeUser.</returns>
    Friend Function KontaktSucheExchangeUser(ByVal Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As Outlook.ExchangeUser

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSucheExchangeUser(Kontaktkarte.Address)

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As Outlook.AddressEntry = ThisAddIn.POutookApplication.Session.GetAddressEntryFromID(Kontaktkarte.Address)

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

    '#Region "Kontaktsuche Table DASL"
    '    ''' <summary>
    '    ''' Startet die Kontaktsuche mit einer Telefonnummer über DASL.
    '    ''' </summary>
    '    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    '    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    '    Friend Function KontaktSucheDASL(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
    '        Dim Filter As List(Of String)

    '        Dim retOlKontakt As Outlook.ContactItem = Nothing

    '        If ThisAddIn.POutookApplication IsNot Nothing Then

    '            If TelNr IsNot Nothing Then
    '                ' Filter zusammenstellen
    '                Filter = New List(Of String)

    '                For Each DASLTag As String In DASLTagTelNrIndex.ToList
    '                    Filter.Add($"{DASLTag}/0x0000001f = '{TelNr.Unformatiert}'")
    '                Next

    '                retOlKontakt = FindeAnruferKontakt($"@SQL={String.Join(" OR ", Filter)}")
    '            End If

    '        End If
    '        Return retOlKontakt
    '    End Function

    '    ''' <summary>
    '    ''' Überladene Funktion die die Suche mit einer Telefonnummer durchführt. Start ist hier der <c>Outlook.NameSpace.</c>
    '    ''' </summary>
    '    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    '    Private Function FindeAnruferKontakt(ByVal sFilter As String) As Outlook.ContactItem
    '        Dim iStore As Integer
    '        Dim olStore As Outlook.Store = Nothing

    '        Dim KontaktGefunden As Outlook.ContactItem = Nothing

    '        ' Indiziere jeden Kontaktordner durch alle Stores rekursiv 
    '        iStore = 1
    '        Do While (iStore.IsLessOrEqual(ThisAddIn.POutookApplication.Session.Stores.Count)) And KontaktGefunden Is Nothing
    '            olStore = ThisAddIn.POutookApplication.Session.Stores.Item(iStore)
    '            ' Kein Suchen in Exchange
    '            'If olStore.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange Then
    '            KontaktGefunden = FindeAnruferKontakt(olStore.GetRootFolder, sFilter)
    '            'End If
    '            iStore += 1
    '        Loop
    '        olStore.ReleaseComObject

    '        Return KontaktGefunden
    '    End Function

    '    ''' <summary>
    '    ''' Überladene Funktion die die Suche mit einer Telefonnummer in einem Outlookordner durchführt. 
    '    ''' </summary>
    '    ''' <param name="Ordner">Outlookordner in dem die Suche durchgeführt wird.</param>
    '    ''' <param name="sFilter">Der Filter, mit dem die Suche nach dem Kontakt durchgeführt werden soll.</param>
    '    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem.</c></returns>
    '    Private Function FindeAnruferKontakt(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem

    '        Dim olKontakt As Outlook.ContactItem = Nothing
    '        Dim iOrdner As Integer    ' Zählvariable für den aktuellen Ordner
    '        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

    '            If XMLData.POptionen.PCBUseLegacySearch Then
    '                olKontakt = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
    '                If olKontakt IsNot Nothing Then
    '                    NLogger.Debug("DASL Search erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
    '                End If
    '            Else
    '                Dim oTable As Outlook.Table
    '                ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
    '                ' Erstellung der Datentabelle
    '                oTable = Ordner.GetTable(sFilter)
    '                ' Festlegung der Spalten. Zunächst werden alle Spalten entfernt
    '                With oTable.Columns
    '                    .RemoveAll()
    '                    .Add("EntryID")
    '                End With

    '                If Not oTable.EndOfTable Then
    '                    olKontakt = GetOutlookKontakt(oTable.GetNextRow("EntryID").ToString, Ordner.StoreID)
    '                    NLogger.Debug("DASL Table erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
    '                End If
    '                oTable.ReleaseComObject
    '            End If
    '        End If

    '        ' Unterordner werden rekursiv durchsucht
    '        iOrdner = 1
    '        Do While (iOrdner.IsLessOrEqual(Ordner.Folders.Count)) And (olKontakt Is Nothing)
    '            olKontakt = FindeAnruferKontakt(Ordner.Folders.Item(iOrdner), sFilter)
    '            iOrdner += 1
    '        Loop
    '        Return olKontakt
    '    End Function '(FindeKontakt)

    '#End Region



    '#Region "KontaktSuche Table UserProperties"
    '    ''' <summary>
    '    ''' Startet die Kontaktsuche mit einer Telefonnummer über Userproperties.
    '    ''' </summary>
    '    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    '    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    '    Friend Function KontaktSucheUserProp(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
    '        Dim Filter As List(Of String)

    '        Dim retOlKontakt As Outlook.ContactItem = Nothing

    '        If ThisAddIn.POutookApplication IsNot Nothing Then

    '            If TelNr IsNot Nothing Then
    '                ' Filter zusammenstellen
    '                Filter = New List(Of String)

    '                For Each UserPropTag As String In PDfltUserProperties.ToList
    '                    Filter.Add($"{UserPropTag}/0x0000001f = '{TelNr.Unformatiert}'")
    '                Next

    '                retOlKontakt = KontaktSucheUserProp($"@SQL={String.Join(" OR ", Filter)}")

    '            End If

    '        End If
    '        Return retOlKontakt
    '    End Function
    '    Private Function KontaktSucheUserProp(ByVal sFilter As String) As Outlook.ContactItem
    '        Dim iStore As Integer
    '        Dim olStore As Outlook.Store = Nothing

    '        Dim KontaktGefunden As Outlook.ContactItem = Nothing

    '        ' Indiziere jeden Kontaktordner durch alle Stores rekursiv 
    '        iStore = 1
    '        Do While (iStore.IsLessOrEqual(ThisAddIn.POutookApplication.Session.Stores.Count)) And KontaktGefunden Is Nothing
    '            olStore = ThisAddIn.POutookApplication.Session.Stores.Item(iStore)
    '            ' Kein Suchen in Exchange
    '            'If olStore.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange Then
    '            KontaktGefunden = KontaktSucheUserProp(olStore.GetRootFolder, sFilter)
    '            'End If
    '            iStore += 1
    '        Loop
    '        olStore.ReleaseComObject

    '        Return KontaktGefunden
    '    End Function
    '    Private Function KontaktSucheUserProp(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem

    '        Dim olKontakt As Outlook.ContactItem = Nothing
    '        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

    '        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
    '            If XMLData.POptionen.PCBUseLegacySearch Then
    '                olKontakt = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
    '                If olKontakt IsNot Nothing Then
    '                    NLogger.Debug("UserProperties Search erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
    '                End If
    '            Else
    '                Dim oTable As Outlook.Table

    '                ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
    '                ' Erstellung der Datentabelle
    '                oTable = Ordner.GetTable(sFilter)

    '                ' Festlegung der Spalten. Zunächst werden alle Spalten entfernt
    '                With oTable.Columns
    '                    .RemoveAll()
    '                    .Add("EntryID")
    '                End With

    '                If Not oTable.EndOfTable Then
    '                    olKontakt = GetOutlookKontakt(oTable.GetNextRow("EntryID").ToString, Ordner.StoreID)
    '                    NLogger.Debug("UserProperties Table erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
    '                End If

    '                oTable.ReleaseComObject
    '            End If
    '        End If

    '        ' Unterordner werden rekursiv durchsucht
    '        iOrdner = 1
    '        Do While (iOrdner <= Ordner.Folders.Count) And (olKontakt Is Nothing)
    '            olKontakt = KontaktSucheUserProp(Ordner.Folders.Item(iOrdner), sFilter)
    '            iOrdner += 1
    '        Loop
    '        Return olKontakt
    '    End Function '(FindeKontakt)

    '#End Region

End Module
