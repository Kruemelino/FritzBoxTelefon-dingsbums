Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Public Class DataKontaktsuche
    Implements IDataKontaktsuche

    Private Property SuchTask As Task(Of List(Of ContactItem))

    Public Async Function KontaktSuche(Text As String) As Task(Of List(Of ContactItem)) Implements IDataKontaktsuche.KontaktSuche

        If SuchTask Is Nothing OrElse SuchTask.IsCompleted Then
            SuchTask = KontaktSucheNameField(Text, False)
        Else
            Await SuchTask
            SuchTask = KontaktSucheNameField(Text, False)
        End If

        Return Await SuchTask
    End Function

    Public Sub DialContact(olContact As ContactItem) Implements IDataKontaktsuche.DialContact
        Dim FBoxDial As New FritzBoxWählClient
        FBoxDial.WählboxStart(olContact)
    End Sub
End Class
