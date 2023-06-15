Imports Microsoft.Office.Interop.Outlook

Public Class OutlookItemWrapper
    Implements IEquatable(Of OutlookItemWrapper)

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property OlItem As Object

    Friend ReadOnly Property Name As String

    Private ReadOnly Property EntryID As String
    Private ReadOnly Property StoreID As String

    Friend Sub New(Item As Object)
        _OlItem = Item

        Select Case True
            Case TypeOf Item Is ContactItem
                InitContactItem(CType(Item, ContactItem))
        End Select
    End Sub

#Region "ContactItem"
    Private Sub InitContactItem(Item As ContactItem)
        With Item

            _Name = .FullNameAndCompanyWithoutLineBreak
            _EntryID = .EntryID
            _StoreID = .StoreID

            AddHandler .BeforeDelete, AddressOf ContactItem_BeforeDelete
            AddHandler .Write, AddressOf ContactItem_Write

        End With
    End Sub

    Private Sub ContactItem_Write(ByRef Cancel As Boolean)

        With CType(OlItem, ContactItem)
            NLogger.Debug($"Speichern des Kontaktes '{ .CompanyAndFullName.RemoveLineBreaks}' wurde registriert.")

            ' Synchronisieren
            .SyncKontakt(.ParentFolder, False)

            ' Indizieren
            .IndiziereKontakt(.ParentFolder)
        End With
    End Sub

    Private Sub ContactItem_BeforeDelete(Item As Object, ByRef Cancel As Boolean)
        If TypeOf Item Is ContactItem Then

            With CType(Item, ContactItem)
                NLogger.Debug($"Löschen des Kontaktes '{ .CompanyAndFullName.RemoveLineBreaks}' wurde registriert.")

                ContactItem_RemoveHandler(.Self)

                .SyncDelete()
            End With
        End If
    End Sub

    Private Sub ContactItem_RemoveHandler(Item As ContactItem)
        With Item
            RemoveHandler .BeforeDelete, AddressOf ContactItem_BeforeDelete
            RemoveHandler .Write, AddressOf ContactItem_Write
        End With
    End Sub

#End Region

#Region "Finalize"
    Friend Sub Auflösen()
        Select Case True
            Case TypeOf OlItem Is ContactItem
                With CType(OlItem, ContactItem)
                    ContactItem_RemoveHandler(.Self)
                End With

        End Select

        MyBase.Finalize()
    End Sub

    Protected Overrides Sub Finalize()
        Auflösen()
        MyBase.Finalize()
    End Sub

#End Region

    Public Overloads Function Equals(other As OutlookItemWrapper) As Boolean Implements IEquatable(Of OutlookItemWrapper).Equals
        Return EntryID.IsEqual(other.EntryID) AndAlso StoreID.IsEqual(other.StoreID)
    End Function
End Class