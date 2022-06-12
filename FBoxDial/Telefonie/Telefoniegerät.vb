Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie

<Serializable()>
Public Class Telefoniegerät
    Inherits NotifyBase
    Implements IEquatable(Of Telefoniegerät)

    Public Sub New()

    End Sub

#Region "Eigenschaften"
    ''' <summary>
    ''' Name des Telefones
    ''' </summary>
    <XmlElement> Public Property Name As String

    ''' <summary>
    ''' Liste aller Telefonnummern, auf die das Telefon reagiert.
    ''' </summary>
    <XmlElement> Public Property StrEinTelNr As List(Of String)

    ''' <summary>
    ''' Interne ID des Telefones
    ''' </summary>
    <XmlAttribute> Public Property ID As Integer

    ''' <summary>
    ''' Interne Kurzwahl des Telefones
    ''' </summary>
    <XmlAttribute> Public Property Kurzwahl As Integer

    ''' <summary>
    ''' Interne ID des Telefones, die durch den Anrufmonitor genutzt wird.
    ''' </summary>
    <XmlAttribute> Public Property AnrMonID As Integer

    ''' <summary>
    ''' Dialport für die Wählhilfe via TR-064
    ''' </summary>
    <XmlAttribute> Public Property TR064Dialport As String

    ''' <summary>
    ''' Angabe, ob es sich um das Standardtelefon der Wählhilfe handelt
    ''' </summary>
    <XmlAttribute> Public Property StdTelefon As Boolean

    ''' <summary>
    ''' Angabe, ob es sich um ein Fax handelt
    ''' </summary>
    <XmlAttribute> Public Property IsFax As Boolean

    ''' <summary>
    ''' Angabe, ob dieses Telefon bei der Wählhilfe zuletzt genutzt wurde.
    ''' </summary>
    <XmlAttribute> Public Property ZuletztGenutzt As Boolean

    Private _IsExternalTAM As Boolean
    ''' <summary>
    ''' Nutzereingabe, ob dieses Telefon ein externer Anrufbeantworter ist. Dies ist nur für FON möglich
    ''' </summary>
    <XmlAttribute> Public Property IsExternalTAM As Boolean
        Set
            SetProperty(_IsExternalTAM, Value)

            OnPropertyChanged(NameOf(IsDialable))
        End Set
        Get
            Return _IsExternalTAM
        End Get
    End Property

    ''' <summary>
    ''' Typ des Telefones
    ''' </summary>
    <XmlAttribute> Public Property TelTyp As TelTypen
#End Region

    ''' <summary>
    ''' Angabe, ob es sich um ein IP Telefon handelt
    ''' </summary>
    ''' <remarks>Wird nur für die Darstellung ein dem Einstellungs-View benötigt.</remarks>
    <XmlIgnore> Public ReadOnly Property IsIPPhone As Boolean
        Get
            Return TelTyp = TelTypen.IP
        End Get
    End Property

    ''' <summary>
    ''' Angabe, ob das Gerät ein interner oder externer Anrufbeantworter ist
    ''' </summary>
    <XmlIgnore> Public ReadOnly Property IsTAM As Boolean
        Get
            Return TelTyp = TelTypen.TAM Or IsExternalTAM
        End Get
    End Property

    ''' <summary>
    ''' Angabe, ob das Telefon über die Wählhilfe steuerbar ist. FAX und TAM werden nicht berücksichtigt.
    ''' </summary>
    <XmlIgnore> Public ReadOnly Property IsDialable As Boolean
        Get
            ' Kein Fax oder externer TAM
            If IsFax Or IsExternalTAM Then Return False

            Select Case TelTyp

                Case TelTypen.DECT, TelTypen.FON, TelTypen.ISDN
                    Return True

                Case TelTypen.IP
                    ' Gibt es einen Connector
                    Return XMLData.PTelefonie.IPTelefone.Exists(Function(C) C.ConnectedPhoneID.AreEqual(ID))

                Case Else
                    Return False
            End Select
        End Get
    End Property

    ''' <summary>
    ''' Gibt den Rückfallwert für den TR-064 Dialportes des Telefones zurück.<br/>
    ''' Der Wert wird anhand Erfahrungswerte zusammengesetzt.
    ''' </summary>
    Friend ReadOnly Property GetDialPortFallback As String
        Get
            Select Case TelTyp
                Case TelTypen.FON
                    Return $"{TelTypen.FON}{ID}: {Name}"
                Case TelTypen.DECT
                    Return $"{TelTypen.DECT}: {Name}"
                Case TelTypen.ISDN
                    Return $"{TelTypen.ISDN}: {Name}"
                Case Else
                    Return String.Empty
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

