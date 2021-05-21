Imports System.Xml.Serialization

Namespace SOAP
    <Serializable()>
    Public Class Device
        Implements IDisposable

        <XmlElement("deviceType")> Public Property DeviceType As String
        <XmlElement("friendlyName")> Public Property FriendlyName As String
        <XmlElement("manufacturer")> Public Property Manufacturer As String
        <XmlElement("manufacturerURL")> Public Property ManufacturerURL As String
        <XmlElement("modelDescription")> Public Property ModelDescription As String
        <XmlElement("modelName")> Public Property ModelName As String
        <XmlElement("modelNumber")> Public Property ModelNumber As String
        <XmlElement("modelURL")> Public Property Display As String
        <XmlElement("UDN")> Public Property UDN As String
        <XmlArray("iconList")> <XmlArrayItem("icon")> Public Property IconList As List(Of Icon)
        <XmlArray("serviceList")> <XmlArrayItem("service")> Public Property ServiceList As List(Of Service)
        <XmlArray("deviceList")> <XmlArrayItem("device")> Public Property DeviceList As List(Of Device)
        <XmlElement("presentationURL")> Public Property PresentationURL As String

#Region "IDisposable Support"
        Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                ' nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
                IconList?.Clear()
                ServiceList.Clear()
                DeviceList?.ForEach(Sub(D) D.Dispose())
            End If
            disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
            Dispose(True)
        End Sub
#End Region
    End Class
End Namespace

