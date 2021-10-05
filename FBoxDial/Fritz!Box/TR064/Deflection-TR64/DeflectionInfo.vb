Imports System.Xml.Serialization


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

Public Enum ModeEnum
    ''' <summary>
    ''' Deflect if a bell blockade is activ
    ''' </summary>
    eBellBlockade

    ''' <summary>
    ''' Busy
    ''' </summary>
    eBusy

    ''' <summary>
    ''' Deflect with a delay
    ''' </summary>
    eDelayed

    ''' <summary>
    ''' Deflect if busy or with a delay
    ''' </summary>
    eDelayedOrBusy

    ''' <summary>
    ''' Direct call
    ''' </summary>
    eDirectCall

    ''' <summary>
    ''' Deflect immediately
    ''' </summary>
    eImmediately

    ''' <summary>
    ''' Deflect with a long delay
    ''' </summary>
    eLongDelayed

    ''' <summary>
    ''' Do not signal this call
    ''' </summary>
    eNoSignal

    ''' <summary>
    ''' Deflect disabled
    ''' </summary>
    eOff

    ''' <summary>
    ''' Parallel call
    ''' </summary>
    eParallelCall

    ''' <summary>
    ''' Deflect with a short delay
    ''' </summary>
    eShortDelayed

    ''' <summary>
    ''' Mode unknown
    ''' </summary>
    eUnknown

    ''' <summary>
    ''' VIP
    ''' </summary>
    eVIP
End Enum

Public Enum TypeEnum
    ''' <summary>
    ''' Phone port 1 is selected
    ''' </summary>
    fon1

    ''' <summary>
    ''' Phone port 2 is selected
    ''' </summary>
    fon2

    ''' <summary>
    ''' Phone port 3 is selected
    ''' </summary>
    fon3

    ''' <summary>
    ''' Phone port 4 is selected
    ''' </summary>
    fon4

    ''' <summary>
    ''' From all
    ''' </summary>
    fromAll

    ''' <summary>
    ''' From a anonymous call 
    ''' </summary>
    fromAnonymous

    ''' <summary>
    ''' Call not from a VIP 
    ''' </summary>
    fromNotVIP

    ''' <summary>
    ''' Specific Number 
    ''' </summary>
    fromNumber

    ''' <summary>
    ''' The caller is in the phonebook
    ''' </summary>
    fromPB

    ''' <summary>
    ''' Call from a VIP
    ''' </summary>
    fromVIP

    ''' <summary>
    ''' To Any
    ''' </summary>
    toAny

    ''' <summary>
    ''' To MSN
    ''' </summary>
    toMSN

    ''' <summary>
    ''' To POTS
    ''' </summary>
    toPOTS

    ''' <summary>
    ''' To VoIP
    ''' </summary>
    toVoIP

    ''' <summary>
    ''' Type unknown
    ''' </summary>
    unknown
End Enum