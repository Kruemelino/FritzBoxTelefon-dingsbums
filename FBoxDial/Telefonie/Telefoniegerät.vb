Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie

<Serializable()>
Public Class Telefoniegerät
    Implements IEquatable(Of Telefoniegerät)

    Public Sub New()

    End Sub

#Region "Eigenschaften"
    <XmlElement> Public Property Name As String
    <XmlElement> Public Property StrEinTelNr As List(Of String)
    <XmlElement> Public Property StrAusTelNr As String
    <XmlAttribute> Public Property Dialport As Integer
    <XmlAttribute> Public Property AnrMonID As Integer
    <XmlAttribute> Public Property UPnPDialport As String
    <XmlAttribute> Public Property StdTelefon As Boolean
    <XmlAttribute> Public Property IsFax As Boolean
    <XmlAttribute> Public Property IsPhoner As Boolean
    <XmlAttribute> Public Property ZuletztGenutzt As Boolean
    <XmlAttribute> Public Property TelTyp As TelTypen
    '<XmlAttribute> Friend Property ZeitEingehend As Integer
    '<XmlAttribute> Friend Property ZeitAusgehend As Integer
    '<XmlIgnore> Friend Property EinTelNr As List(Of Telefonnummer)
    '<XmlIgnore> Friend Property AusTelNr As Telefonnummer
#End Region

#Region "Equals"
    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Telefoniegerät))
    End Function

    Public Overloads Function Equals(other As Telefoniegerät) As Boolean Implements IEquatable(Of Telefoniegerät).Equals
        Return other IsNot Nothing AndAlso
               Name = other.Name AndAlso
               Dialport = other.Dialport AndAlso
               StdTelefon = other.StdTelefon AndAlso
               TelTyp.CompareTo(other.TelTyp).IsZero
    End Function
#End Region
End Class

