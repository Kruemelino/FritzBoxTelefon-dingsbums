Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie

<Serializable()>
Public Class Telefoniegerät
    Inherits NotifyBase
    Implements IEquatable(Of Telefoniegerät)
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public Sub New()

    End Sub

#Region "Eigenschaften"
    <XmlElement> Public Property Name As String
    <XmlElement> Public Property StrEinTelNr As List(Of String)
    <XmlAttribute> Public Property Intern As Integer
    <XmlAttribute> Public Property AnrMonID As Integer
    <XmlAttribute> Public Property TR064Dialport As String
    <XmlAttribute> Public Property StdTelefon As Boolean
    <XmlAttribute> Public Property IsFax As Boolean
    <XmlAttribute> Public Property IsPhoner As Boolean
    <XmlAttribute> Public Property IsMicroSIP As Boolean
    <XmlAttribute> Public Property ZuletztGenutzt As Boolean
    <XmlAttribute> Public Property TelTyp As TelTypen
#End Region
    <XmlIgnore> Public ReadOnly Property IsSoftPhone As Boolean
        Get
            Return IsMicroSIP Or IsPhoner
        End Get
    End Property

    <XmlIgnore> Public ReadOnly Property IsIPPhone As Boolean
        Get
            Return TelTyp = TelTypen.IP
        End Get
    End Property

    <XmlIgnore> Public ReadOnly Property IsDialable As Boolean
        Get
            ' Kein Fax
            If IsFax Then Return False

            Select Case TelTyp

                Case TelTypen.DECT, TelTypen.FON, TelTypen.ISDN
                    Return True

                Case TelTypen.IP
                    Return IsPhoner Or IsMicroSIP

                Case Else
                    Return False
            End Select
        End Get
    End Property

    Friend ReadOnly Property GetDialPortFallback As String
        Get
            Select Case TelTyp
                Case TelTypen.FON
                    Return $"{TelTypen.FON}{AnrMonID + 1}: {Name}"
                Case TelTypen.DECT
                    Return $"{TelTypen.DECT}: {Name}"
                Case TelTypen.ISDN
                    Return $"{TelTypen.ISDN}: {Name}"
                Case Else
                    Return DfltStringEmpty
            End Select
        End Get
    End Property





#Region "Equals"
    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Telefoniegerät))
    End Function

    Public Overloads Function Equals(other As Telefoniegerät) As Boolean Implements IEquatable(Of Telefoniegerät).Equals
        Return other IsNot Nothing AndAlso
               Name = other.Name AndAlso
               StdTelefon = other.StdTelefon AndAlso
               TelTyp.CompareTo(other.TelTyp).IsZero
    End Function
#End Region
End Class

