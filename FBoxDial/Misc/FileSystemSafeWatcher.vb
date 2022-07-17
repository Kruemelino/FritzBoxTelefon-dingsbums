Imports System.IO
Imports System.Timers
Imports System.Collections
Imports System.ComponentModel

' https://github.com/melenaos/FileSystemSafeWatcher

<DebuggerStepThrough>
Friend Class DelayedEvent
    Private ReadOnly _args As FileSystemEventArgs
    Private _delayed As Boolean

    Public Sub New(args As FileSystemEventArgs)
        _delayed = False
        _args = args
    End Sub

    Public ReadOnly Property Args As FileSystemEventArgs
        Get
            Return _args
        End Get
    End Property

    Public Property Delayed As Boolean
        Get
            Return _delayed
        End Get
        Set(ByVal value As Boolean)
            _delayed = value
        End Set
    End Property

    Public Overridable Function IsDuplicate(ByVal obj As Object) As Boolean
        Dim delayedEvent As DelayedEvent = TryCast(obj, DelayedEvent)
        If delayedEvent Is Nothing Then Return False
        Dim eO1 As FileSystemEventArgs = _args
        Dim reO1 As RenamedEventArgs = TryCast(_args, RenamedEventArgs)
        Dim eO2 As FileSystemEventArgs = delayedEvent._args
        Dim reO2 As RenamedEventArgs = TryCast(delayedEvent._args, RenamedEventArgs)

        Return (eO1 IsNot Nothing AndAlso eO2 IsNot Nothing AndAlso eO1.ChangeType = eO2.ChangeType AndAlso eO1.FullPath = eO2.FullPath AndAlso eO1.Name = eO2.Name AndAlso ((reO1 Is Nothing AndAlso reO2 Is Nothing) OrElse (reO1 IsNot Nothing AndAlso reO2 IsNot Nothing AndAlso reO1.OldFullPath = reO2.OldFullPath AndAlso reO1.OldName = reO2.OldName))) OrElse (eO1 IsNot Nothing AndAlso eO2 IsNot Nothing AndAlso eO1.ChangeType = WatcherChangeTypes.Created AndAlso eO2.ChangeType = WatcherChangeTypes.Changed AndAlso eO1.FullPath = eO2.FullPath AndAlso eO1.Name = eO2.Name)
    End Function
End Class

Friend Class FileSystemSafeWatcher
    Implements IDisposable

    Private ReadOnly _fileSystemWatcher As FileSystemWatcher
    Private ReadOnly _enterThread As New Object()
    Private _events As ArrayList
    Private _serverTimer As Timer
    Private _consolidationInterval As Integer = 100
    Private disposedValue As Boolean

    Public Sub New()
        _fileSystemWatcher = New FileSystemWatcher()
        Initialize()
    End Sub

    Public Sub New(path As String)
        _fileSystemWatcher = New FileSystemWatcher(path)
        Initialize()
    End Sub

    Public Sub New(path As String, filter As String)
        _fileSystemWatcher = New FileSystemWatcher(path, filter)
        Initialize()
    End Sub

    Public Property EnableRaisingEvents As Boolean
        Get
            Return _fileSystemWatcher.EnableRaisingEvents
        End Get
        Set
            _fileSystemWatcher.EnableRaisingEvents = Value

            If Value Then
                _serverTimer.Start()
            Else
                _serverTimer.[Stop]()
                _events.Clear()
            End If
        End Set
    End Property

    Public Property Filter As String
        Get
            Return _fileSystemWatcher.Filter
        End Get
        Set
            _fileSystemWatcher.Filter = Value
        End Set
    End Property

    Public Property IncludeSubdirectories As Boolean
        Get
            Return _fileSystemWatcher.IncludeSubdirectories
        End Get
        Set
            _fileSystemWatcher.IncludeSubdirectories = Value
        End Set
    End Property

    Public Property InternalBufferSize As Integer
        Get
            Return _fileSystemWatcher.InternalBufferSize
        End Get
        Set
            _fileSystemWatcher.InternalBufferSize = Value
        End Set
    End Property

    Public Property NotifyFilter As NotifyFilters
        Get
            Return _fileSystemWatcher.NotifyFilter
        End Get
        Set
            _fileSystemWatcher.NotifyFilter = Value
        End Set
    End Property

    Public Property Path As String
        Get
            Return _fileSystemWatcher.Path
        End Get
        Set
            _fileSystemWatcher.Path = Value
        End Set
    End Property

    Public Property SynchronizingObject As ISynchronizeInvoke

    Public Event Changed As FileSystemEventHandler
    Public Event Created As FileSystemEventHandler
    Public Event Deleted As FileSystemEventHandler
    Public Event [Error] As ErrorEventHandler
    Public Event Renamed As RenamedEventHandler

    Public Sub BeginInit()
        _fileSystemWatcher.BeginInit()
    End Sub

    Public Sub EndInit()
        _fileSystemWatcher.EndInit()
    End Sub

    Protected Sub OnChanged(e As FileSystemEventArgs)
        RaiseEvent Changed(Me, e)
    End Sub

    Protected Sub OnCreated(e As FileSystemEventArgs)
        RaiseEvent Created(Me, e)
    End Sub

    Protected Sub OnDeleted(e As FileSystemEventArgs)
        RaiseEvent Deleted(Me, e)
    End Sub

    Protected Sub OnError(e As ErrorEventArgs)
        RaiseEvent Error(Me, e)
    End Sub

    Protected Sub OnRenamed(e As RenamedEventArgs)
        RaiseEvent Renamed(Me, e)
    End Sub

    Public Function WaitForChanged(changeType As WatcherChangeTypes) As WaitForChangedResult
        Throw New NotImplementedException()
    End Function

    Public Function WaitForChanged(changeType As WatcherChangeTypes, timeout As Integer) As WaitForChangedResult
        Throw New NotImplementedException()
    End Function


    Private Sub Initialize()
        _events = ArrayList.Synchronized(New ArrayList(32))
        AddHandler _fileSystemWatcher.Changed, New FileSystemEventHandler(AddressOf FileSystemEventHandler)
        AddHandler _fileSystemWatcher.Created, New FileSystemEventHandler(AddressOf FileSystemEventHandler)
        AddHandler _fileSystemWatcher.Deleted, New FileSystemEventHandler(AddressOf FileSystemEventHandler)
        AddHandler _fileSystemWatcher.[Error], New ErrorEventHandler(AddressOf ErrorEventHandler)
        AddHandler _fileSystemWatcher.Renamed, New RenamedEventHandler(AddressOf RenamedEventHandler)

        _serverTimer = New Timer(_consolidationInterval)
        AddHandler _serverTimer.Elapsed, New ElapsedEventHandler(AddressOf ElapsedEventHandler)

        _serverTimer.AutoReset = True
        _serverTimer.Enabled = _fileSystemWatcher.EnableRaisingEvents
    End Sub

    Private Sub FileSystemEventHandler(sender As Object, e As FileSystemEventArgs)
        _events.Add(New DelayedEvent(e))
    End Sub

    Private Sub ErrorEventHandler(sender As Object, e As ErrorEventArgs)
        OnError(e)
    End Sub

    Private Sub RenamedEventHandler(sender As Object, e As RenamedEventArgs)
        _events.Add(New DelayedEvent(e))
    End Sub

    Private Sub ElapsedEventHandler(sender As Object, e As ElapsedEventArgs)
        ' We don't fire the events inside the lock. We will queue them here until
        ' the code exits the locks.
        Dim eventsToBeFired As Queue = Nothing

        If Threading.Monitor.TryEnter(_enterThread) Then
            ' Only one thread at a time is processing the events
            Try
                eventsToBeFired = New Queue(32)

                ' Lock the collection while processing the events
                SyncLock _events.SyncRoot
                    Dim current As DelayedEvent

                    For i As Integer = 0 To _events.Count - 1
                        If i.IsLess(_events.Count) Then
                            current = TryCast(_events(i), DelayedEvent)

                            If current.Delayed Then
                                ' This event has been delayed already so we can fire it
                                ' We just need to remove any duplicates

                                For j As Integer = i + 1 To _events.Count - 1
                                    ' Removing later duplicates
                                    If j.IsLess(_events.Count) AndAlso current.IsDuplicate(_events(j)) Then
                                        _events.RemoveAt(j)
                                        j -= 1 ' Don't skip next event
                                    End If
                                Next

                                ' check if the file has been completely copied (can be opened for read)
                                Dim [raiseEvent] As Boolean = True

                                If current.Args.ChangeType = WatcherChangeTypes.Created OrElse current.Args.ChangeType = WatcherChangeTypes.Changed Then
                                    Dim stream As FileStream = Nothing

                                    Try
                                        stream = File.Open(current.Args.FullPath, FileMode.Open, FileAccess.Read, FileShare.None)
                                        ' If this succeeds, the file is finished
                                    Catch __unusedIOException1__ As IOException
                                        [raiseEvent] = False
                                    Finally
                                        If stream IsNot Nothing Then stream.Close()
                                    End Try
                                End If

                                If [raiseEvent] Then
                                    ' Add the event to the list of events to be fired
                                    eventsToBeFired.Enqueue(current)
                                    ' Remove it from the current list
                                    _events.RemoveAt(i)
                                    i -= 1 ' Don't skip next event
                                End If
                            Else
                                ' This event was not delayed yet, so we will delay processing
                                ' this event for at least one timer interval
                                current.Delayed = True
                            End If

                        End If
                    Next
                End SyncLock

            Finally
                Threading.Monitor.[Exit](_enterThread)
            End Try
            ' Else - this timer event was skipped, processing will happen during the next timer event
        End If
        ' Now fire all the events if any events are in eventsToBeFired
        RaiseEvents(eventsToBeFired)
    End Sub

    Public Property ConsolidationInterval As Integer
        Get
            Return _consolidationInterval
        End Get
        Set
            _consolidationInterval = Value
            _serverTimer.Interval = Value
        End Set
    End Property

    Protected Sub RaiseEvents(deQueue As Queue)
        If (deQueue IsNot Nothing) AndAlso (deQueue.Count > 0) Then
            Dim de As DelayedEvent

            While deQueue.Count > 0
                de = TryCast(deQueue.Dequeue(), DelayedEvent)

                Select Case de.Args.ChangeType
                    Case WatcherChangeTypes.Changed
                        OnChanged(de.Args)
                    Case WatcherChangeTypes.Created
                        OnCreated(de.Args)
                    Case WatcherChangeTypes.Deleted
                        OnDeleted(de.Args)
                    Case WatcherChangeTypes.Renamed
                        OnRenamed(TryCast(de.Args, RenamedEventArgs))
                End Select
            End While
        End If
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            If _fileSystemWatcher IsNot Nothing Then _fileSystemWatcher.Dispose()
            If _serverTimer IsNot Nothing Then _serverTimer.Dispose()

            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

