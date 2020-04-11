Imports Microsoft.Office.Interop

Friend Module KontaktSucher

    Friend ReadOnly Property PDfltContactFolder() As Outlook.MAPIFolder = ThisAddIn.POutookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Friend Function KontaktSuche(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
        NLogger.Debug("Kontaktsuche gestartet")

        If XMLData.POptionen.PCBUseLegacyUserProp Then
            Return KontaktSucheUserProp(TelNr)
        Else
            'Return KontaktSucheDASL(TelNr)
            Return KontaktSucheAuswahlDASL(TelNr)
        End If
    End Function

#Region "KontaktSuche Table DASL"
    ''' <summary>
    ''' Startet die Kontaktsuche mit einer Telefonnummer über DASL.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Friend Function KontaktSucheDASL(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
        Dim Filter As List(Of String)

        Dim retOlKontakt As Outlook.ContactItem = Nothing

        If ThisAddIn.POutookApplication IsNot Nothing Then

            If TelNr IsNot Nothing Then
                ' Filter zusammenstellen
                Filter = New List(Of String)

                For Each DASLTag As String In DASLTagTelNrIndex.ToList
                    Filter.Add($"{DASLTag}/0x0000001f = '{TelNr.Unformatiert}'")
                Next

                retOlKontakt = FindeAnruferKontakt($"@SQL={String.Join(" OR ", Filter)}")
            End If

        End If
        Return retOlKontakt
    End Function

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer durchführt. Start ist hier der <c>Outlook.NameSpace.</c>
    ''' </summary>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Private Function FindeAnruferKontakt(ByVal sFilter As String) As Outlook.ContactItem
        Dim iStore As Integer
        Dim olStore As Outlook.Store = Nothing

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        ' Indiziere jeden Kontaktordner durch alle Stores rekursiv 
        iStore = 1
        Do While (iStore.IsLessOrEqual(ThisAddIn.POutookApplication.Session.Stores.Count)) And KontaktGefunden Is Nothing
            olStore = ThisAddIn.POutookApplication.Session.Stores.Item(iStore)
            ' Kein Suchen in Exchange
            'If olStore.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange Then
            KontaktGefunden = FindeAnruferKontakt(olStore.GetRootFolder, sFilter)
            'End If
            iStore += 1
        Loop
        olStore.ReleaseComObject

        Return KontaktGefunden
    End Function

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer in einem Outlookordner durchführt. 
    ''' </summary>
    ''' <param name="Ordner">Outlookordner in dem die Suche durchgeführt wird.</param>
    ''' <param name="sFilter">Der Filter, mit dem die Suche nach dem Kontakt durchgeführt werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem.</c></returns>
    Private Function FindeAnruferKontakt(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem
        NLogger.Debug("DASL im Ordner: {0}", Ordner.Name)
        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Integer    ' Zählvariable für den aktuellen Ordner

        'If Ordner.Store.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange AndAlso
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

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner.IsLessOrEqual(Ordner.Folders.Count)) And (olKontakt Is Nothing)
            olKontakt = FindeAnruferKontakt(Ordner.Folders.Item(iOrdner), sFilter)
            iOrdner += 1
        Loop
        Return olKontakt
    End Function '(FindeKontakt)
#End Region

#Region "KontaktSuche Table UserProperties"
    ''' <summary>
    ''' Startet die Kontaktsuche mit einer Telefonnummer über Userproperties.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Friend Function KontaktSucheUserProp(ByVal TelNr As Telefonnummer) As Outlook.ContactItem
        Dim Filter As List(Of String)

        Dim retOlKontakt As Outlook.ContactItem = Nothing

        If ThisAddIn.POutookApplication IsNot Nothing Then

            If TelNr IsNot Nothing Then
                ' Filter zusammenstellen
                Filter = New List(Of String)

                For Each UserPropTag As String In PDfltUserProperties.ToList
                    Filter.Add($"{UserPropTag}/0x0000001f = '{TelNr.Unformatiert}'")
                Next

                retOlKontakt = KontaktSucheUserProp($"@SQL={String.Join(" OR ", Filter)}")

            End If

        End If
        Return retOlKontakt
    End Function
    Private Function KontaktSucheUserProp(ByVal sFilter As String) As Outlook.ContactItem
        Dim iStore As Integer
        Dim olStore As Outlook.Store = Nothing

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        ' Indiziere jeden Kontaktordner durch alle Stores rekursiv 
        iStore = 1
        Do While (iStore.IsLessOrEqual(ThisAddIn.POutookApplication.Session.Stores.Count)) And KontaktGefunden Is Nothing
            olStore = ThisAddIn.POutookApplication.Session.Stores.Item(iStore)
            ' Kein Suchen in Exchange
            'If olStore.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange Then
            KontaktGefunden = KontaktSucheUserProp(olStore.GetRootFolder, sFilter)
            'End If
            iStore += 1
        Loop
        olStore.ReleaseComObject

        Return KontaktGefunden
    End Function

    Private Function KontaktSucheUserProp(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem

        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner


        'If Ordner.Store.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange AndAlso
        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            If XMLData.POptionen.PCBUseLegacySearch Then
                olKontakt = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
                If olKontakt IsNot Nothing Then
                    NLogger.Debug("UserProperties Search erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
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
                    NLogger.Debug("UserProperties Table erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
                End If

                oTable.ReleaseComObject
            End If
        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count) And (olKontakt Is Nothing)
            olKontakt = KontaktSucheUserProp(Ordner.Folders.Item(iOrdner), sFilter)
            iOrdner += 1
        Loop
        Return olKontakt
    End Function '(FindeKontakt)


#End Region

#Region "Absendersuche E-Mail"
    ''' <summary>
    ''' Startet die Kontaktsuche mit einer E-Mail.
    ''' </summary>
    ''' <param name="EMailAdresse">E-Mail, die als Suchkriterium verwendet werden soll.</param>
    ''' <returns></returns>
    Friend Function KontaktSuche(ByVal EMailAdresse As String) As Outlook.ContactItem
        If EMailAdresse.IsNotStringEmpty Then
            Return FindeAbsenderKontakt(EMailAdresse)
        Else
            Return Nothing
        End If

    End Function


    ''' <summary>
    ''' Funktion die die Suche mit einer E-Mail-Adresse durchführt.
    ''' </summary>
    ''' <param name="EMailAdresse">E-Mail-Adresse, die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Private Function FindeAbsenderKontakt(ByVal EMailAdresse As String) As Outlook.ContactItem

        Dim olKontakt As Outlook.ContactItem = Nothing

        With ThisAddIn.POutookApplication.Session.CreateRecipient(EMailAdresse)
            .Resolve()
            If .AddressEntry.GetContact() IsNot Nothing Then
                olKontakt = .AddressEntry.GetContact()
            ElseIf .AddressEntry.GetExchangeUser IsNot Nothing Then
                olKontakt = .AddressEntry.GetExchangeUser.GetContact()
            End If
        End With
        Return olKontakt
    End Function
#End Region

#Region "Suche in Ordnerauswahl"
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

                With XMLData.POptionen.IndizerteOrdner
                    ' Kontaktsuche in allen vom Nutzer ausgewählten Ordnern
                    If .OrdnerListe.Any Then
                        Dim Ordner As IndizerterOrdner
                        iOrdner = 0
                        Do While (iOrdner.IsLess(.OrdnerListe.Count)) And (olKontakt Is Nothing)
                            Ordner = .OrdnerListe.Item(iOrdner)
                            Dim olFolder As Outlook.MAPIFolder
                            ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
                            ' Erstellung der Datentabelle
                            olFolder = GetOutlookFolder(Ordner.FolderID, Ordner.StoreID)

                            If XMLData.POptionen.PCBUseLegacySearch Then
                                olKontakt = CType(olFolder.Items.Find(sFilter), Outlook.ContactItem)
                                If olKontakt IsNot Nothing Then
                                    NLogger.Debug("DASL Search Auswahl erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
                                End If
                            Else
                                If olFolder IsNot Nothing Then
                                    Dim oTable As Outlook.Table = olFolder.GetTable(sFilter)

                                    ' Festlegung der Spalten. Zunächst werden alle Spalten entfernt
                                    With oTable.Columns
                                        .RemoveAll()
                                        .Add("EntryID")
                                    End With

                                    If Not oTable.EndOfTable Then
                                        olKontakt = GetOutlookKontakt(oTable.GetNextRow("EntryID").ToString, Ordner.StoreID)
                                        NLogger.Debug("DASL Table Auswahl erfolgreich: {0} in {1}", olKontakt.FullNameAndCompany, Ordner.Name)
                                    End If
                                    oTable.ReleaseComObject
                                End If
                            End If
                            olFolder.ReleaseComObject
                            'Next
                            iOrdner += 1
                        Loop
                    End If
                End With
            End If
        End If
        Return olKontakt
    End Function
#End Region


End Module
