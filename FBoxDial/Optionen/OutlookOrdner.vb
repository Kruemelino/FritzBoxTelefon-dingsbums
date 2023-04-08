Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

Public Enum OutlookOrdnerVerwendung As Integer
    KontaktSuche = 1
    KontaktSpeichern = 2
    JournalSpeichern = 4
    TerminSpeichern = 8
    FBoxSync = 16
End Enum

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum SyncMode As Integer
    <LocalizedDescription("OutlookToFritzBox", GetType(resEnum))>
    OutlookToFritzBox = 1

    <LocalizedDescription("FritzBoxToOutlook", GetType(resEnum))>
    FritzBoxToOutlook = 2

End Enum

<Serializable()>
Public Class SyncOptions
    <XmlElement> Public Property FBoxSyncID As Integer = -1
    <XmlElement> Public Property FBoxSyncMode As SyncMode
    <XmlElement> Public Property FBoxCBSyncStartUp As Boolean = False

    <XmlIgnore> Friend ReadOnly Property ValidData As Boolean
        Get
            Return FBoxSyncID.AreDifferentTo(-1) And Not FBoxSyncMode = 0
        End Get
    End Property

End Class

<Serializable()>
Public Class OutlookOrdner
    Implements IDisposable
    Implements IEquatable(Of OutlookOrdner)

    Public Sub New()
        ' Nicht löschen. Wird zum deserialisieren benötigt: Parameterloser Konstruktor
    End Sub
    Public Sub New(OlFolder As Outlook.MAPIFolder)

        FolderID = OlFolder.EntryID
        StoreID = OlFolder.StoreID
        Name = OlFolder.Name

    End Sub

    Public Sub New(OlFolder As Outlook.MAPIFolder, Verwendung As OutlookOrdnerVerwendung)

        FolderID = OlFolder.EntryID
        StoreID = OlFolder.StoreID
        Name = OlFolder.Name
        Typ = Verwendung

    End Sub

    <XmlElement> Public Property FolderID As String
    <XmlElement> Public Property StoreID As String
    <XmlAttribute> Public Property Name As String

    <XmlElement> Public Property FBoxSyncOptions As SyncOptions = Nothing
    <XmlAttribute> Public Property Typ As OutlookOrdnerVerwendung
    <XmlIgnore> Friend ReadOnly Property Exists As Boolean
        Get
            Return GetOutlookFolder(FolderID, StoreID) IsNot Nothing
        End Get
    End Property
    <XmlIgnore> Friend Property MAPIFolder As Outlook.MAPIFolder
        Get
            Return GetOutlookFolder(FolderID, StoreID)
        End Get
        Set
            FolderID = Value.EntryID
            StoreID = Value.StoreID
        End Set
    End Property

    <XmlIgnore> Friend ReadOnly Property ItemsCount As Integer
        Get
            Return If(Exists, MAPIFolder.Items.Count, -1)
        End Get
    End Property

#Region "IEquatable Support"
    Public Overloads Function Equals(other As OutlookOrdner) As Boolean Implements IEquatable(Of OutlookOrdner).Equals
        If other Is Nothing Then Return False
        Return FolderID.IsEqual(other.FolderID) And StoreID.IsEqual(other.StoreID) And Typ.Equals(other.Typ)
    End Function

    Public Overloads Function Equals(other As Outlook.MAPIFolder, Verwendung As OutlookOrdnerVerwendung) As Boolean
        If other Is Nothing Then Return False
        Return Equals(other) And Typ.Equals(Verwendung)
    End Function

    Public Overloads Function Equals(other As Outlook.MAPIFolder) As Boolean
        If other Is Nothing Then Return False
        Return FolderID.IsEqual(other.EntryID) And StoreID.IsEqual(other.StoreID)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (FolderID, StoreID).GetHashCode()
    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ReleaseComObject(MAPIFolder)
            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    ' Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    Protected Overrides Sub Finalize()
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
