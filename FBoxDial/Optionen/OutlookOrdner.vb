Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Microsoft.Office.Interop.Outlook

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

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public Sub New()
        ' Nicht löschen. Wird zum deserialisieren benötigt: Parameterloser Konstruktor
    End Sub
    Public Sub New(OlFolder As MAPIFolder)

        FolderID = OlFolder.EntryID
        StoreID = OlFolder.StoreID
        Name = OlFolder.Name

    End Sub

    Public Sub New(OlFolder As MAPIFolder, Verwendung As OutlookOrdnerVerwendung)

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
            Dim F As MAPIFolder = GetOutlookFolder(FolderID, StoreID)
            Try
                ' Es kann sein, dass auf den Ordner nicht zugegriffen werden kann. Dann kommt es im schlimmsten Fall zu einem Absturz
                Return F IsNot Nothing AndAlso F.EntryID.IsNotStringNothingOrEmpty
            Catch ex As System.Exception
                NLogger.Error(ex, $"Der Ordner '{Name}' ist vorhanden jedoch kann auf ihn nicht zugegriffen werden.")
                Return False
            End Try

        End Get
    End Property
    <XmlIgnore> Friend Property MAPIFolder As MAPIFolder
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

    Friend Function ContainsChildFolder(Ordner As MAPIFolder) As Boolean
        Return GetChildFolders(MAPIFolder, Ordner.DefaultItemType, Typ).Contains(New OutlookOrdner(Ordner, Typ))
    End Function

#Region "IEquatable Support"
    Public Overloads Function Equals(other As OutlookOrdner) As Boolean Implements IEquatable(Of OutlookOrdner).Equals
        If other Is Nothing Then Return False
        Return Typ.Equals(other.Typ) AndAlso (FolderID.IsEqual(other.FolderID) And StoreID.IsEqual(other.StoreID))
    End Function

    Public Overloads Function Equals(other As MAPIFolder, Verwendung As OutlookOrdnerVerwendung) As Boolean
        If other Is Nothing Then Return False
        Return Typ.Equals(Verwendung) AndAlso Equals(other)
    End Function

    Public Overloads Function Equals(other As MAPIFolder) As Boolean
        If other Is Nothing Then Return False
        Return FolderID.IsEqual(other.EntryID) And StoreID.IsEqual(other.StoreID)
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
