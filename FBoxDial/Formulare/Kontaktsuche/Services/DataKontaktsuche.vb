Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Public Class DataKontaktsuche
    Implements IDataKontaktsuche
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property TokenSource As CancellationTokenSource
    Private Property CT As CancellationToken
    Private Property SuchTask As Task(Of List(Of ContactItem))
    Private AltesWort As String = ""

    Private Async Function KontaktSuche(Wort As String) As Task(Of List(Of ContactItem)) Implements IDataKontaktsuche.KontaktSuche

        If SuchTask IsNot Nothing AndAlso Not SuchTask.IsCompleted Then
            NLogger.Trace($"SuchTask abgebrochen: Alters Wort: {AltesWort} Neues Wort: {Wort}")
            ' Brich den aktuellenn Suchtask ab
            TokenSource.Cancel()
        End If
        AltesWort = Wort

        ' Erstelle eine neue Abbruchtoken
        TokenSource = New CancellationTokenSource
        CT = TokenSource.Token

        NLogger.Trace($"SuchTask gestartet: Neues Wort: {Wort}")

        SuchTask = Task.Run(Function() KontaktSucheNameField(Wort, False, CT), CT)
        Return Await SuchTask
    End Function

    Private Sub DialContact(olContact As ContactItem) Implements IDataKontaktsuche.DialContact
        Dim FBoxDial As New FritzBoxWählClient
        FBoxDial.WählboxStart(olContact)
    End Sub
    Private Sub UpdateTheme() Implements IDataKontaktsuche.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub
End Class
