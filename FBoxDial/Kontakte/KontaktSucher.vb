Imports Microsoft.Office.Interop

Friend Class KontaktSucher
    Implements IDisposable

    Private ReadOnly Property OutlookApp() As Outlook.Application = ThisAddIn.POutookApplication
    Friend ReadOnly Property PDfltContactFolder() As Outlook.MAPIFolder = OutlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)

    ''' <summary>
    ''' Startet die Kontaktsuche mit einer E-Mail oder einer Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    ''' <param name="EMailAdresse">E-Mail, die als Suchkriterium verwendet werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Friend Async Function KontaktSuche(ByVal TelNr As Telefonnummer, ByVal EMailAdresse As String) As Threading.Tasks.Task(Of Outlook.ContactItem)
        Dim JoinFilter As List(Of String)

        Dim retOlKontakt As Outlook.ContactItem = Nothing

        If OutlookApp IsNot Nothing Then

            If TelNr IsNot Nothing Then
                ' Filter zusammenstellen

                JoinFilter = New List(Of String)

                For Each DASLTag As String In DASLTagTelNrIndex.ToList
                    JoinFilter.Add(String.Format("{0}/0x0000001f = '{1}'", DASLTag, TelNr.Unformatiert))
                Next

                If XMLData.POptionen.PCBKontaktSucheHauptOrdner Then
                    retOlKontakt = Await FindeAnruferKontakt(PDfltContactFolder, String.Format("@SQL={0}", String.Join(" OR ", JoinFilter)))
                Else
                    retOlKontakt = Await FindeAnruferKontakt(OutlookApp.Session, String.Format("@SQL={0}", String.Join(" OR ", JoinFilter)))
                End If

            ElseIf EMailAdresse.IsNotStringEmpty Then
                retOlKontakt = FindeAbsenderKontakt(EMailAdresse)
            End If
        End If
        Return retOlKontakt
    End Function
    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer durchführt. Start ist hier der <c>Outlook.NameSpace.</c>
    ''' </summary>
    ''' <param name="NamensRaum">Startpunkt der Rekursiven Suche als <c>Outlook.NameSpace</c>.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Private Overloads Async Function FindeAnruferKontakt(ByVal NamensRaum As Outlook.NameSpace, ByVal sFilter As String) As Threading.Tasks.Task(Of Outlook.ContactItem)

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        '  Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        Dim j As Integer = 1
        Do While (j <= NamensRaum.Folders.Count) And (KontaktGefunden Is Nothing)
            KontaktGefunden = Await FindeAnruferKontakt(NamensRaum.Folders.Item(j), sFilter)
            j += 1
            Windows.Forms.Application.DoEvents()
        Loop
        Return KontaktGefunden
    End Function

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer in einem Outlookordner durchführt. 
    ''' </summary>
    ''' <param name="Ordner">Outlookordner in dem die Suche durchgeführt wird.</param>
    ''' <param name="sFilter">Der Filter, mit dem die Suche nach dem Kontakt durchgeführt werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem.</c></returns>
    Private Overloads Async Function FindeAnruferKontakt(ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Threading.Tasks.Task(Of Outlook.ContactItem)

        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner
        Dim oTable As Outlook.Table

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
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

        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count) And (olKontakt Is Nothing)
            olKontakt = Await FindeAnruferKontakt(Ordner.Folders.Item(iOrdner), sFilter)
            iOrdner += 1
            Windows.Forms.Application.DoEvents()
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

        With OutlookApp.Session.CreateRecipient(EMailAdresse)
            .Resolve()
            If .AddressEntry.GetContact() IsNot Nothing Then
                olKontakt = .AddressEntry.GetContact()
            ElseIf .AddressEntry.GetExchangeUser IsNot Nothing Then
                olKontakt = .AddressEntry.GetExchangeUser.GetContact()
            End If
        End With
        Return olKontakt
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
