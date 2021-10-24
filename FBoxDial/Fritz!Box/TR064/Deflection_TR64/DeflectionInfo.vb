Imports System.Xml.Serialization

Namespace TR064
    <Serializable> Public Class DeflectionInfo
        Inherits NotifyBase

        Private _DeflectionId As Integer
        <XmlElement("DeflectionId")> Public Property DeflectionId As Integer
            Get
                Return _DeflectionId
            End Get
            Set
                SetProperty(_DeflectionId, Value)
            End Set
        End Property

        Private _Enable As Boolean
        <XmlElement("Enable")> Public Property Enable As Boolean
            Get
                Return _Enable
            End Get
            Set
                SetProperty(_Enable, Value)
            End Set
        End Property

        Private _Type As TypeEnum
        <XmlElement("Type")> Public Property Type As TypeEnum
            Get
                Return _Type
            End Get
            Set
                SetProperty(_Type, Value)
            End Set
        End Property

        Private _Number As String
        <XmlElement("Number")> Public Property Number As String
            Get
                Return _Number
            End Get
            Set
                SetProperty(_Number, Value)
            End Set
        End Property

        Private _DeflectionToNumber As String
        <XmlElement("DeflectionToNumber")> Public Property DeflectionToNumber As String
            Get
                Return _DeflectionToNumber
            End Get
            Set
                SetProperty(_DeflectionToNumber, Value)
            End Set
        End Property

        Private _Mode As ModeEnum
        <XmlElement("Mode")> Public Property Mode As ModeEnum
            Get
                Return _Mode
            End Get
            Set
                SetProperty(_Mode, Value)
            End Set
        End Property

        Private _Outgoing As String
        <XmlElement("Outgoing")> Public Property Outgoing As String
            Get
                Return _Outgoing
            End Get
            Set
                SetProperty(_Outgoing, Value)
            End Set
        End Property

        ''' <summary>
        ''' Only valid if Type==fromPB
        ''' </summary>
        Private _PhonebookID As String
        <XmlElement("PhonebookID")> Public Property PhonebookID As String
            Get
                Return _PhonebookID
            End Get
            Set
                SetProperty(_PhonebookID, Value)
            End Set
        End Property
    End Class

End Namespace