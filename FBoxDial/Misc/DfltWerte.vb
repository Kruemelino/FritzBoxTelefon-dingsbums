Imports System.Collections.ObjectModel

Public NotInheritable Class DfltWerteAllgemein

#Region "Global Default Value Properties"
    Friend Shared ReadOnly Property DfltDASLSchema As String = "http://schemas.microsoft.com/mapi/string/{FFF40745-D92F-4C11-9E14-92701F001EB3}/"

    Friend Shared ReadOnly Property DASLTagOlItem As Object() = {$"{DfltDASLSchema}FBDB-ContactEntryID", $"{DfltDASLSchema}FBDB-ContactStoreID"}.ToArray

#End Region

#Region "Literale Journal"

    Public Shared ReadOnly Property DfltOlItemCategories() As ReadOnlyCollection(Of String)
        Get
            Return New ReadOnlyCollection(Of String)({Localize.LocAnrMon.strJournalCatDefault, Localize.LocAnrMon.strJournalCatCalls})
        End Get
    End Property
#End Region

End Class

