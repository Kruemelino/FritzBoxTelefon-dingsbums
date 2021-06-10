Imports Microsoft.Office.Interop.Outlook

Friend Interface IOptionenService

#Region "Grunddaten"
    Function LadeFBoxUser(IPAdresse As String) As ObservableCollectionEx(Of FritzBoxXMLUser)
#End Region

#Region "Import Telefoniedaten"
    Event Status As EventHandler(Of NotifyEventArgs(Of String))
    Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie))
    Sub StartImport()
#End Region

#Region "Indizierung Ordner"
    Event IndexStatus As EventHandler(Of NotifyEventArgs(Of Integer))
    Property CancelationPending As Boolean
    ''' <summary>
    ''' Startet die Indizierung des Ordners
    ''' </summary>
    ''' <param name="Ordner">Outlook-Ordner, der indiziert werden soll</param>
    ''' <param name="IndexModus">Modus: true indizieren, false deindizieren</param>
    ''' <param name="Unterordner">Angabe, ob Unterordner einbezogen werden sollen</param>
    Sub Indexer(Ordner As MAPIFolder, IndexModus As Boolean, Unterordner As Boolean)
    Function ZähleOutlookKontakte(olFolder As MAPIFolder) As Integer
#End Region

#Region "Test Rückwärtssuche"
    Event BeendetRWS As EventHandler(Of NotifyEventArgs(Of Boolean))
    Sub StartRWSTest(TelNr As String)
#End Region
End Interface
