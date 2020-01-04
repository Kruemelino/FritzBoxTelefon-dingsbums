Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

<Serializable()> Public Class VIPEntry
    '<XmlElement> Public Property VCard As String
    <XmlAttribute> Public Property Name As String
    <XmlElement> Public Property EntryID As String
    <XmlElement> Public Property StoreID As String
    <XmlIgnore> Public Property OlContact() As Outlook.ContactItem

#Region "RibbonXML"
    Friend Overloads Function CreateDynMenuButton(ByVal xDoc As Xml.XmlDocument, ByVal ID As Integer, ByVal Tag As String) As Xml.XmlElement
        Dim XButton As Xml.XmlElement
        Dim XAttribute As Xml.XmlAttribute

        XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

        XAttribute = xDoc.CreateAttribute("id")
        XAttribute.Value = String.Format("{0}_{1}", Tag, ID)
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("label")

        OlContact = GetOutlookKontakt(EntryID, StoreID)
        If OlContact IsNot Nothing Then
            XAttribute.Value = String.Format("{0}{1}", OlContact.FullName, If(OlContact.CompanyName.IsNotStringNothingOrEmpty, String.Format(" ({0})", OlContact.CompanyName), PDfltStringEmpty)).XMLMaskiereZeichen
        End If

        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("onAction")
        XAttribute.Value = "BtnOnAction"
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("tag")
        XAttribute.Value = Tag.XMLMaskiereZeichen
        XButton.Attributes.Append(XAttribute)

        Return XButton
    End Function
#End Region
End Class

