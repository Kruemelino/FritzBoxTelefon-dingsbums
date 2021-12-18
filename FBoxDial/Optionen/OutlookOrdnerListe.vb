Imports System.Xml.Serialization
Imports Microsoft.Office.Interop.Outlook

<Serializable()>
Public Class OutlookOrdnerListe
    <XmlElement("Ordner")> Public Property OrdnerListe As List(Of OutlookOrdner)

    Public Sub New()
        OrdnerListe = New List(Of OutlookOrdner)
    End Sub

    Friend Sub Add(OutlookOrdner As OutlookOrdner)
        If Not Exists(OutlookOrdner.MAPIFolder, OutlookOrdner.Typ) Then OrdnerListe.Add(OutlookOrdner)
    End Sub

    Friend Sub AddRange(ListeOutlookOrdner As List(Of OutlookOrdner))
        OrdnerListe.AddRange(ListeOutlookOrdner)
    End Sub

    Friend Function Exists(Verwendung As OutlookOrdnerVerwendung) As Boolean
        Return OrdnerListe.Exists(Function(fldr) fldr.Typ = Verwendung)
    End Function

    Friend Function Exists(MAPIFolder As MAPIFolder, Verwendung As OutlookOrdnerVerwendung) As Boolean
        Return OrdnerListe.Exists(Function(fldr) fldr.MAPIFolder.AreEqual(MAPIFolder) And fldr.Typ = Verwendung)
    End Function

    Friend Function Find(Verwendung As OutlookOrdnerVerwendung) As OutlookOrdner
        Return OrdnerListe.Find(Function(fldr) fldr.Typ = Verwendung)
    End Function

    Friend Function Find(StoreID As String, FolderID As String, Verwendung As OutlookOrdnerVerwendung) As OutlookOrdner
        Return OrdnerListe.Find(Function(fldr) fldr.FolderID.IsEqual(FolderID) And fldr.StoreID.IsEqual(StoreID) And fldr.Typ = Verwendung)
    End Function

    Friend Function FindAll(Verwendung As OutlookOrdnerVerwendung) As List(Of OutlookOrdner)
        Return OrdnerListe.FindAll(Function(fldr) fldr.Typ = Verwendung)
    End Function

    Friend Sub RemoveAll(Verwendung As OutlookOrdnerVerwendung)
        OrdnerListe.RemoveAll(Function(OlFldr) OlFldr.Typ = Verwendung)
    End Sub

    Friend Sub ClearNotExisting()
        OrdnerListe.RemoveAll(Function(OlFldr) Not OlFldr.Exists)
    End Sub

    Friend Function Remove(Folder As OutlookOrdner) As Boolean
        Return OrdnerListe.Remove(Folder)
    End Function

    ''' <summary>
    ''' Prüft, ob der Outlook-Ordner für die gewünschte Verwendung ausgewählt wurde.
    ''' Falls der Nutzer keinen Ordner in den Einstellungen gewählt hat, wird der Standard-Ordner verwendet.
    ''' </summary>
    ''' <param name="Verwendung"></param>
    ''' <returns></returns>
    Public Function GetMAPIFolder(Verwendung As OutlookOrdnerVerwendung) As MAPIFolder
        ' Ist der Order für die gewählte Verwendung vom User ausgewählt?

        If Exists(Verwendung) Then
            Return Find(Verwendung).MAPIFolder
        Else
            Select Case Verwendung
                ' Journaleinträge
                Case OutlookOrdnerVerwendung.JournalSpeichern
                    Return GetDefaultMAPIFolder(OlDefaultFolders.olFolderJournal)

                ' Kontakte
                Case OutlookOrdnerVerwendung.KontaktSpeichern, OutlookOrdnerVerwendung.KontaktSuche
                    Return GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts)

                Case Else
                    Return Nothing

            End Select
        End If

    End Function

    Public Function OrdnerAusgewählt(Ordner As MAPIFolder, Verwendung As OutlookOrdnerVerwendung) As Boolean

        ' Gibt es überhaupt Ordner für die gewählte Verwendung
        If FindAll(Verwendung).Any Then
            ' Ist der Order für die gewählte Verwendung vom User ausgewählt?
            Return Exists(Ordner, Verwendung)
        Else
            ' Fallback
            ' Ist der Order der Standard-Ordner für Journal oder Kontakt?

            Select Case Verwendung
                ' Journaleinträge
                Case OutlookOrdnerVerwendung.JournalSpeichern
                    Return Ordner.AreEqual(GetDefaultMAPIFolder(OlDefaultFolders.olFolderJournal))

                ' Kontakte
                Case OutlookOrdnerVerwendung.KontaktSpeichern, OutlookOrdnerVerwendung.KontaktSuche
                    Return Ordner.AreEqual(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts))

                Case Else
                    Return False

            End Select
        End If
    End Function

End Class
