Friend Interface IOptionenService

#Region "Import Telefoniedaten"
    Event Status As EventHandler(Of NotifyEventArgs(Of String))
    Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie))
    Sub StartImport()
#End Region

#Region "Outlook Ordner"

#End Region

End Interface
