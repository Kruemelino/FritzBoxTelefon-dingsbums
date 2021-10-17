Imports System.Threading.Tasks
Imports Microsoft.Office.Interop

Public Interface IDataKontaktsuche
    Function KontaktSuche(Text As String) As Task(Of List(Of Outlook.ContactItem))
    Function KontaktSuche2(Text As String) As List(Of Outlook.ContactItem)
    Sub DialContact(olContact As Outlook.ContactItem)
End Interface
