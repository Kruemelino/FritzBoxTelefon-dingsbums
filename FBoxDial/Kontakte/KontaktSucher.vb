Imports Microsoft.Office.Interop

Friend Module KontaktSucher

    Friend ReadOnly Property PDfltContactFolder() As Outlook.MAPIFolder = ThisAddIn.POutookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)

    ''' <summary>
    ''' Startet die Kontaktsuche mit einer Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Friend Async Function KontaktSuche(ByVal TelNr As Telefonnummer) As Threading.Tasks.Task(Of Outlook.ContactItem)
        Dim JoinFilter As List(Of String)

        Dim retOlKontakt As Outlook.ContactItem = Nothing

        If ThisAddIn.POutookApplication IsNot Nothing Then

            If TelNr IsNot Nothing Then
                ' Filter zusammenstellen

                JoinFilter = New List(Of String)

                For Each DASLTag As String In DASLTagTelNrIndex.ToList
                    JoinFilter.Add(String.Format("{0}/0x0000001f = '{1}'", DASLTag, TelNr.Unformatiert))
                Next

                If XMLData.POptionen.PCBKontaktSucheHauptOrdner Then
                    retOlKontakt = Await FindeAnruferKontakt(PDfltContactFolder, String.Format("@SQL={0}", String.Join(" OR ", JoinFilter)))
                Else
                    retOlKontakt = Await FindeAnruferKontakt(String.Format("@SQL={0}", String.Join(" OR ", JoinFilter)))
                End If
            End If
        End If
        Return retOlKontakt
    End Function
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
    ''' Überladene Funktion die die Suche mit einer Telefonnummer durchführt. Start ist hier der <c>Outlook.NameSpace.</c>
    ''' </summary>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Private Async Function FindeAnruferKontakt(ByVal sFilter As String) As Threading.Tasks.Task(Of Outlook.ContactItem)
        Dim iStore As Integer
        Dim olStore As Outlook.Store = Nothing

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        ' Indiziere jeden Kontaktordner durch alle Stores rekursiv 
        iStore = 1
        Do While (iStore.IsLessOrEqual(ThisAddIn.POutookApplication.Session.Stores.Count)) And KontaktGefunden Is Nothing
            olStore = ThisAddIn.POutookApplication.Session.Stores.Item(iStore)
            ' Kein Suchen in Exchange
            If olStore.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange Then
                KontaktGefunden = Await FindeAnruferKontakt(olStore.GetRootFolder, sFilter)
            End If
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
    Private Async Function FindeAnruferKontakt(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Threading.Tasks.Task(Of Outlook.ContactItem)

        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner


        If Ordner.Store.ExchangeStoreType = Outlook.OlExchangeStoreType.olNotExchange AndAlso
           Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

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
            End If

            oTable.ReleaseComObject
        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count) And (olKontakt Is Nothing)
            olKontakt = Await FindeAnruferKontakt(Ordner.Folders.Item(iOrdner), sFilter)
            iOrdner += 1
        Loop
        Return olKontakt
    End Function '(FindeKontakt)

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

End Module
