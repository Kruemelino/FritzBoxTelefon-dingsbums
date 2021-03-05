Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' https://gist.github.com/sachintha81/7b56aa5704409e055b8bbd33a27c9482
''' </summary>
Public Class Messenger
    Private Shared ReadOnly CreationLock As Object = New Object()
    Private Shared ReadOnly Dictionary As ConcurrentDictionary(Of MessengerKey, Object) = New ConcurrentDictionary(Of MessengerKey, Object)()

#Region "Default property"

    Private Shared _instance As Messenger


    ''' <summary>
    ''' Gets the single instance of the Messenger.
    ''' </summary>
    Public Shared ReadOnly Property [Default] As Messenger
        Get

            If _instance Is Nothing Then

                SyncLock CreationLock

                    If _instance Is Nothing Then
                        _instance = New Messenger()
                    End If
                End SyncLock
            End If

            Return _instance
        End Get
    End Property


#End Region

    ''' <summary>
    ''' Initializes a new instance of the Messenger class.
    ''' </summary>
    Private Sub New()
    End Sub


    ''' <summary>
    ''' Registers a recipient for a type of message T. The action parameter will be executed
    ''' when a corresponding message is sent.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="recipient"></param>
    ''' <param name="action"></param>
    Public Sub Register(Of T)(recipient As Object, action As Action(Of T))
        Register(recipient, action, Nothing)
    End Sub


    ''' <summary>
    ''' Registers a recipient for a type of message T and a matching context. The action parameter will be executed
    ''' when a corresponding message is sent.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="recipient"></param>
    ''' <param name="action"></param>
    ''' <param name="context"></param>
    Public Sub Register(Of T)(recipient As Object, action As Action(Of T), context As Object)
        Dim key = New MessengerKey(recipient, context)
        Dictionary.TryAdd(key, action)
    End Sub


    ''' <summary>
    ''' Unregisters a messenger recipient completely. After this method is executed, the recipient will
    ''' no longer receive any messages.
    ''' </summary>
    ''' <param name="recipient"></param>
    Public Sub Unregister(recipient As Object)
        Unregister(recipient, Nothing)
    End Sub


    ''' <summary>
    ''' Unregisters a messenger recipient with a matching context completely. After this method is executed, the recipient will
    ''' no longer receive any messages.
    ''' </summary>
    ''' <param name="recipient"></param>
    ''' <param name="context"></param>
    Public Sub Unregister(recipient As Object, context As Object)
        Dim key = New MessengerKey(recipient, context)
        Dictionary.TryRemove(key, Nothing)
    End Sub


    ''' <summary>
    ''' Sends a message to registered recipients. The message will reach all recipients that are
    ''' registered for this message type.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="message"></param>
    Public Sub Send(Of T)(message As T)
        Send(message, Nothing)
    End Sub


    ''' <summary>
    ''' Sends a message to registered recipients. The message will reach all recipients that are
    ''' registered for this message type and matching context.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="message"></param>
    ''' <param name="context"></param>
    Public Sub Send(Of T)(message As T, context As Object)
        Dim result As IEnumerable(Of KeyValuePair(Of MessengerKey, Object))

        If context Is Nothing Then
            ' Get all recipients where the context is null.
            result = From r In Dictionary Where r.Key.Context Is Nothing Select r
        Else
            ' Get all recipients where the context is matching.
            result = From r In Dictionary Where r.Key.Context IsNot Nothing AndAlso r.Key.Context.Equals(context) Select r
        End If

        For Each action In result.[Select](Function(x) x.Value).OfType(Of Action(Of T))()
            ' Send the message to all recipients.
            action(message)
        Next
    End Sub

    Protected Class MessengerKey
        Private _Recipient As Object, _Context As Object

        Public Property Recipient As Object
            Get
                Return _Recipient
            End Get
            Private Set(value As Object)
                _Recipient = value
            End Set
        End Property

        Public Property Context As Object
            Get
                Return _Context
            End Get
            Private Set(value As Object)
                _Context = value
            End Set
        End Property


        ''' <summary>
        ''' Initializes a new instance of the MessengerKey class.
        ''' </summary>
        ''' <param name="recipient"></param>
        ''' <param name="context"></param>
        Public Sub New(recipient As Object, context As Object)
            Me.Recipient = recipient
            Me.Context = context
        End Sub


        ''' <summary>
        ''' Determines whether the specified MessengerKey is equal to the current MessengerKey.
        ''' </summary>
        ''' <param name="other"></param>
        ''' <returns></returns>
        Protected Overloads Function Equals(other As MessengerKey) As Boolean
            Return Equals(Recipient, other.Recipient) AndAlso Equals(Context, other.Context)
        End Function


        ''' <summary>
        ''' Determines whether the specified MessengerKey is equal to the current MessengerKey.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Overrides Function Equals(obj As Object) As Boolean
            If obj Is Nothing Then Return False
            If ReferenceEquals(Me, obj) Then Return True
            If obj.GetType() IsNot [GetType]() Then Return False
            Return Equals(CType(obj, MessengerKey))
        End Function


        ''' <summary>
        ''' Serves as a hash function for a particular type. 
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function GetHashCode() As Integer
            '  BEGIN TODO : Visual Basic does Not support checked statements!
            Return ((If(Recipient IsNot Nothing, Recipient.GetHashCode(), 0)) * 397) Xor (If(Context IsNot Nothing, Context.GetHashCode(), 0))
            ' End TODO : Visual Basic does Not support checked statements!
        End Function
    End Class
End Class
