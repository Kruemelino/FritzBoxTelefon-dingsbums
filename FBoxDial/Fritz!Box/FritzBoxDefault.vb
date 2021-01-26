Imports System.Xml.Schema

Friend NotInheritable Class FritzBoxDefault

    Friend Shared ReadOnly Property DfltFritzBoxAdress() As String = "fritz.box"
    Friend Shared ReadOnly Property DfltFritzBoxUser As String = "admin"
    Friend Shared ReadOnly Property DfltFritzBoxIPAdress As String = "192.168.178.1"
    Friend Shared ReadOnly Property DfltFritzBoxSessionID As String = "0000000000000000"
    Friend Shared ReadOnly Property DfltTelCodeActivateFritzBoxCallMonitor() As String = "#96*5*"
    Friend Shared ReadOnly Property DfltFBAnrMonPort() As Integer = 1012
    Friend Shared ReadOnly Property DfltCodePageFritzBox() As Integer = 65001
    Friend Shared ReadOnly Property DfltFritzBoxName As String = "Fritz!Box"


#Region "Properties Fritz!Box Links"
    Friend Shared ReadOnly Property FBLinkBasis() As String
        Get
            If XMLData.POptionen.ValidFBAdr.IsNotStringEmpty Then
                XMLData.POptionen.ValidFBAdr = ValidIP(XMLData.POptionen.TBFBAdr)
            End If
            Return "http://" & XMLData.POptionen.ValidFBAdr
        End Get
    End Property
#End Region

#Region "Fritz!Box SOAP/TR64"
    Friend Shared ReadOnly Property DfltTR064Port() As Integer = 49000
    Friend Shared ReadOnly Property DfltTR064PortSSL() As Integer = 49443
    ''' <summary>
    ''' nameSpace URL: http://schemas.xmlsoap.org/soap/envelope/
    ''' </summary>
    Friend Shared ReadOnly Property DfltTR064RequestNameSpaceEnvelope() As String = "http://schemas.xmlsoap.org/soap/envelope/"
    ''' <summary>
    ''' nameSpace URL: http://schemas.xmlsoap.org/soap/encoding/
    ''' </summary>
    Friend Shared ReadOnly Property DfltTR064RequestNameSpaceEncoding() As String = "http://schemas.xmlsoap.org/soap/encoding/"
    Friend Shared ReadOnly Property DfltSOAPRequestSchema() As XmlSchema
        Get
            Dim XMLSOAPSchema As New XmlSchema

            With XMLSOAPSchema.Namespaces
                .Add("s", DfltTR064RequestNameSpaceEnvelope)
                .Add("u", DfltTR064RequestNameSpaceEncoding)
            End With

            Return XMLSOAPSchema
        End Get
    End Property
#End Region

#Region "Fritz!Box Telefonbuch"

    Friend Shared ReadOnly Property DfltTelBuchTelTyp() As List(Of KeyValuePair(Of String, String))
        Get
            Dim values As New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)("home", "Privat"),
                New KeyValuePair(Of String, String)("work", "Arbeit"),
                New KeyValuePair(Of String, String)("intern", "Intern"),
                New KeyValuePair(Of String, String)("fax", "Fax Privat"),
                New KeyValuePair(Of String, String)("fax_work", "Fax Arbeit"),
                New KeyValuePair(Of String, String)("mobile", "Mobil"),
                New KeyValuePair(Of String, String)("memo", "Memo")
            }
            Return values
        End Get
    End Property


#End Region
End Class
