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

    Public Async Function KontaktSuche(Wort As String) As Task(Of List(Of ContactItem)) Implements IDataKontaktsuche.KontaktSuche

        If SuchTask IsNot Nothing AndAlso Not SuchTask.IsCompleted Then
            NLogger.Trace($"SuchTask abgebrochen: Alters Wort: {AltesWort} Neues Wort: {Wort}")
            ' Brich den aktuellenn Suchtask ab
            TokenSource.Cancel()
        End If
        AltesWort = Wort

        ' Erstelle eine neue Abbruchtoken
        TokenSource = New CancellationTokenSource
        CT = tokenSource.Token

        NLogger.Trace($"SuchTask gestartet: Neues Wort: {Wort}")

        SuchTask = Task.Run(Function() KontaktSucheNameField(Wort, False, ct), ct)
        Return Await SuchTask
    End Function

    Public Sub DialContact(olContact As ContactItem) Implements IDataKontaktsuche.DialContact
        Dim FBoxDial As New FritzBoxWählClient
        FBoxDial.WählboxStart(olContact)
    End Sub

End Class
