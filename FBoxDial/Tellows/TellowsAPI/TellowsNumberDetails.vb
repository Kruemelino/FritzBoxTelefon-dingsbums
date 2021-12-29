Imports System.Xml.Serialization

<Serializable(), XmlType("numberDetails")> Public Class TellowsNumberDetails

    <XmlElement("name")> Public Property Name As String

    <XmlElement("category")> Public Property Category As String

    <XmlElement("street")> Public Property Street As String

    <XmlElement("zipcode")> Public Property Zipcode As String

    <XmlElement("city")> Public Property City As String

    <XmlElement("isCompany")> Public Property IsCompany As Boolean

End Class