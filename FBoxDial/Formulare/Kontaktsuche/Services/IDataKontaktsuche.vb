Imports System.Threading.Tasks
Imports Microsoft.Office.Interop

Public Interface IDataKontaktsuche
    Function KontaktSuche(Text As String) As Task(Of List(Of Outlook.ContactItem))
    Sub DialContact(olContact As Outlook.ContactItem)
    Sub UpdateTheme()
End Interface
