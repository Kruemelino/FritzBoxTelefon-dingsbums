Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Public Class DataKontaktsuche
    Implements IDataKontaktsuche
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property TokenSource As CancellationTokenSource
    Private Property CT As CancellationToken
    Private Property SuchTask As Task(Of List(Of ContactItem))
    Private Property AltesWort As String = String.Empty

    Private Async Function KontaktSuche(Wort As String) As Task(Of List(Of ContactItem)) Implements IDataKontaktsuche.KontaktSuche

        If Wort.Length.IsLargerOrEqual(XMLData.POptionen.TBFormSearchMinLength) Then

            If SuchTask IsNot Nothing AndAlso Not SuchTask.IsCompleted Then
                NLogger.Trace($"SuchTask abgebrochen: Alters Wort: {AltesWort} Neues Wort: {Wort}")
                ' Brich den aktuellen Suchtask ab
                TokenSource.Cancel()
            End If
            AltesWort = Wort

            ' Erstelle eine neue Abbruchtoken
            TokenSource = New CancellationTokenSource
            CT = TokenSource.Token

            NLogger.Trace($"SuchTask gestartet: Neues Wort: {Wort}")

            SuchTask = Task.Run(Function() KontaktSucheNameField(Wort, False, CT), CT)

            Return Await SuchTask
        Else
            Return Await Task.Run(Function() New List(Of ContactItem)) ' Rückgabe: eine leere Liste.
        End If
    End Function

    Private Sub DialContact(olContact As ContactItem) Implements IDataKontaktsuche.DialContact
        Dim FBoxDial As New FritzBoxWählClient
        FBoxDial.WählboxStart(olContact)
    End Sub
    Private Sub UpdateTheme() Implements IDataKontaktsuche.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub
End Class
