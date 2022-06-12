Imports System.Xml.Serialization


<Serializable()>
Public Class IPTelefonDaten

    ''' <summary>
    ''' Angabe, ob dieses Telefon mit dem Softphone Phoner verbunden ist.
    ''' </summary>
    <XmlAttribute> Public Property IsPhoner As Boolean

    ''' <summary>
    ''' Angabe, ob dieses Telefon mit dem Softphone MicroSIP verbunden ist.
    ''' </summary>
    <XmlAttribute> Public Property IsMicroSIP As Boolean


    ' TODO: Wird für Telefoniegeräte benötigt
    ''' <summary>
    ''' Angabe, ob das Telefon den Softphones Phoner bzw. MicroSIP zugeordnet ist.
    ''' </summary>
    <XmlIgnore> Public ReadOnly Property IsSoftPhone As Boolean
        Get
            Return IsMicroSIP Or IsPhoner
        End Get
    End Property
End Class
