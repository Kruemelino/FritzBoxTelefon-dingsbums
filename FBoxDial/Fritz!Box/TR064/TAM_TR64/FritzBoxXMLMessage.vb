Imports System.Xml.Serialization

Namespace TR064
    <Serializable(), XmlType("Message")> Public Class FritzBoxXMLMessage
        Inherits NotifyBase

        Private _Called As String
        ''' <summary>
        ''' Number of called party 
        ''' </summary>
        <XmlElement("Called", GetType(String))> Public Property ID As String
            Get
                Return _Called
            End Get
            Set
                SetProperty(_Called, Value)
            End Set
        End Property

        Private _Date As String
        ''' <summary>
        ''' 31.07.12 12:03
        ''' </summary>
        <XmlElement("Date", GetType(String))> Public Property [Date] As String
            Get
                Return _Date
            End Get
            Set
                SetProperty(_Date, Value)
            End Set
        End Property

        Private _Duration As String
        ''' <summary>
        ''' hh:mm (minutes rounded up)
        ''' </summary>
        <XmlElement("Duration", GetType(String))> Public Property Duration As String
            Get
                Return _Duration
            End Get
            Set
                SetProperty(_Duration, Value)
            End Set
        End Property

        Private _Inbook As Boolean
        ''' <summary>
        ''' 0 not in a phone book,
        ''' 1 stored in a phone book
        ''' </summary>
        <XmlElement("Inbook", GetType(Boolean))> Public Property Inbook As Boolean
            Get
                Return _Inbook
            End Get
            Set
                SetProperty(_Inbook, Value)
            End Set
        End Property

        Private _Index As Integer
        ''' <summary>
        ''' Message index (ID), smallest value is 0. It grows with the number of messages.
        ''' Deleting a message don´t change the index of other messages, so the index is not a continuous counter.
        ''' </summary>
        <XmlElement("Index", GetType(Integer))> Public Property Index As Integer
            Get
                Return _Index
            End Get
            Set
                SetProperty(_Index, Value)
            End Set
        End Property

        Private _Name As String
        ''' <summary>
        ''' Name of Called number 
        ''' </summary>
        <XmlElement("Name", GetType(String))> Public Property Name As String
            Get
                Return _Name
            End Get
            Set
                SetProperty(_Name, Value)
            End Set
        End Property

        Private _New As Boolean
        ''' <summary>
        ''' 0 message is new,
        ''' 1 message has been marked 
        ''' </summary>
        <XmlElement("New", GetType(Boolean))> Public Property [New] As Boolean
            Get
                Return _New
            End Get
            Set
                SetProperty(_New, Value)
            End Set
        End Property

        Private _Number As String
        ''' <summary>
        ''' Own number
        ''' </summary>
        <XmlElement("Number", GetType(String))> Public Property Number As String
            Get
                Return _Number
            End Get
            Set
                SetProperty(_Number, Value)
            End Set
        End Property

        Private _Path As String
        ''' <summary>
        ''' URL path to TAM file
        ''' </summary>
        <XmlElement("Path", GetType(String))> Public Property Path As String
            Get
                Return _Path
            End Get
            Set
                SetProperty(_Path, Value)
            End Set
        End Property

        Private _Tam As Integer
        ''' <summary>
        ''' TAM index
        ''' </summary>
        <XmlElement("Tam", GetType(Integer))> Public Property Tam As Integer
            Get
                Return _Tam
            End Get
            Set
                SetProperty(_Tam, Value)
            End Set
        End Property

        <XmlIgnore> Friend ReadOnly Property CompleteURL(Optional SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID) As String
            Get
                If SessionID.AreEqual(FritzBoxDefault.DfltFritzBoxSessionID) Then
                    ' Wird bei Anzeige im Anrufmonitor benötigt.
                    If Ping(XMLData.POptionen.ValidFBAdr) Then
                        Using fbtr064 As New TR064.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
                            fbtr064.Deviceconfig.GetSessionID(SessionID)
                            ' Session ID erhalten, ansonsten DfltFritzBoxSessionID
                        End Using
                    End If
                End If

                Return If(SessionID.AreNotEqual(FritzBoxDefault.DfltFritzBoxSessionID), $"https://{XMLData.POptionen.ValidFBAdr}:{TR064.DfltTR064PortSSL}{Path}&{SessionID}", DfltStringEmpty)
            End Get

        End Property
    End Class

End Namespace
