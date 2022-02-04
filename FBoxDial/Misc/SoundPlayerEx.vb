Imports System.ComponentModel
Imports System.Media
Imports System.Threading
Imports System.Threading.Tasks
Public Class SoundPlayerEx
    Inherits SoundPlayer

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property TokenSource As CancellationTokenSource

    Private Property TmpFilePath As String
    Friend Property LocationURL As String
    Friend Property PlayingAsync As Boolean = False

    Friend Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String))

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub PlayAsync()

        TokenSource = New CancellationTokenSource
        Dim CT As CancellationToken = TokenSource.Token

        If LocationURL.IsNotStringNothingOrEmpty Then

            PlayingAsync = True

            Task.Run(Async Function()
                         ' Merke die URL, da sie beim Wechsel der Soundfile überschrieben wird
                         Dim URL As String = LocationURL
                         TmpFilePath = $"{IO.Path.GetTempPath}{IO.Path.GetRandomFileName}"
                         ' Lade die Datei in eine temporäre Datei
                         If Await DownloadToFileTaskAsync(New Uri(URL), TmpFilePath) Then
                             Try
                                 ' Ermittle die Länge der Datei. 
                                 Dim stopAt As Date = Date.Now.AddMilliseconds(UnSaveMethods.GetWAVDuration(TmpFilePath))

                                 ' Setze die Soundquelle auf die heruntergeladene Datei
                                 SoundLocation = TmpFilePath

                                 ' Spiele die Datei ab
                                 Play()

                                 LöscheDatei(TmpFilePath)
                                 ' Schleife um den Task am Leben zu lassen
                                 While Date.Now < stopAt And Not CT.IsCancellationRequested
                                     Task.Delay(10).Wait()
                                 End While

                                 ' Beendet die laufende Wiedergabe
                                 MyBase.[Stop]()
                             Finally
                                 OnSoundFinished(URL)
                             End Try
                         End If

                     End Function, CT)
        End If

    End Sub

    Public Overloads Sub [Stop]()
        If PlayingAsync Then
            TokenSource.Cancel()
        Else
            MyBase.[Stop]()
        End If
    End Sub

    Protected Overridable Sub OnSoundFinished(Pfad As String)
        NLogger.Debug($"Wiedergabe der Datei {Pfad} abgeschlossen.")

        ' Setze Flag auf False
        PlayingAsync = False

        ' Event auslösen
        RaiseEvent SoundFinished(Me, New NotifyEventArgs(Of String)(Pfad))
    End Sub

End Class
