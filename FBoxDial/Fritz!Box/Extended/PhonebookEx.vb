Imports System.Threading.Tasks

Public Class PhonebookEx
    Friend Property Phonebook As FBoxAPI.Phonebook

    Public Sub New()

    End Sub

    Public Sub New(phonebook As FBoxAPI.Phonebook)
        _Phonebook = phonebook
    End Sub

    Friend Property ID As Integer

    ' Der Telefonbuchname wird in der Combobox für Kontaktsynchronisation verwendet
    Public ReadOnly Property Name As String
        Get
            Return Phonebook.Name
        End Get
    End Property

    ''' <summary>
    ''' Angabe, ob es sich um das interne Rufsperrentelefonbuch handelt (ID 258, CallBarring).
    ''' </summary>
    Friend Property CallBarringBook As Boolean = False

    ''' <summary>
    ''' Angabe, ob das Telefonbuch als Rufsperre genutzt wird.
    ''' </summary>
    Friend Property Rufsperre As Boolean = False

    ''' <summary>
    ''' Angabe, ob das Telefonbuch via DAV synchronisiert wird.
    ''' </summary>
    Friend ReadOnly Property IsDAV As Boolean
        Get
            Return Phonebook.Owner.ToInt.IsInRange(240, 257)
        End Get
    End Property

#Region "Funktionen"

    Friend Function GetContact(EntryID As Integer) As FBoxAPI.Contact
        Return Phonebook.Contacts.Find(Function(K) K.Uniqueid.AreEqual(EntryID))
    End Function

    ''' <summary>
    ''' Durchsucht die Kontakte nach der übergebenen Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Zu suchende Telefonnummer als Typ: <see cref="Telefonnummer"/></param>
    ''' <returns>Eine Auflistung aller infrage kommenden Kontakte.</returns>
    Friend Function GetContact(TelNr As Telefonnummer) As IEnumerable(Of FBoxAPI.Contact)
        Return Phonebook.Contacts.Where(Function(K)
                                            ' interne Telefone sollen nicht duchsucht werden
                                            Return Not K.IstTelefon AndAlso K.Telephony.Numbers.Where(Function(N) TelNr.Equals(N.Number)).Any
                                        End Function)
    End Function

    ''' <summary>
    ''' Durchsucht die Kontakte nach der übergebenen Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Zu suchende Telefonnummer als Typ: <see cref="String"/></param>
    ''' <returns>Eine Auflistung aller infrage kommenden Kontakte.</returns>
    Friend Function GetContact(TelNr As String) As IEnumerable(Of FBoxAPI.Contact)
        Return Phonebook.Contacts.Where(Function(K)
                                            ' interne Telefone sollen nicht duchsucht werden
                                            Return Not K.IstTelefon AndAlso K.Telephony.Numbers.Where(Function(N) TelNr.Equals(N.Number)).Any
                                        End Function)
    End Function

    Friend Function GetContacts() As IEnumerable(Of FBoxAPI.Contact)
        ' interne Telefone sollen nicht duchsucht werden
        Return Phonebook.Contacts.Where(Function(K) Not K.IstTelefon)
    End Function

    ''' <summary>
    ''' Fügt den übergebenen Kontakt hinzu. 
    ''' Kontakte mit der selben ID werden entfernt (sollte beim Aktualisieren nur einer sein.)
    ''' </summary>
    ''' <param name="Kontakt"></param>
    Friend Sub AddContact(Kontakt As FBoxAPI.Contact)
        If Kontakt IsNot Nothing Then
            With Phonebook.Contacts
                ' Entferne alle Kontakte mit der selben UniqueID (sollte nur einen oder keinen geben)
                .RemoveAll(Function(E) E.Uniqueid.AreEqual(Kontakt.Uniqueid))

                ' Kontakt hinzufügen
                .Add(Kontakt)
            End With
        End If
    End Sub

    ''' <summary>
    ''' Entfernt einen Kontakt aus der Liste.
    ''' </summary>
    ''' <param name="Kontakt">Der zu entfernende Kontakt.</param>
    Friend Sub DeleteKontakt(Kontakt As FBoxAPI.Contact)
        If Kontakt IsNot Nothing Then
            With Phonebook.Contacts
                ' Kontake entfernen
                .Remove(Kontakt)
            End With
        End If
    End Sub

    ''' <summary>
    ''' Entfernt eine Auflistung von Kontakten aus dem Telefonbuch
    ''' </summary>
    ''' <param name="Kontakte">Liste der zu entfernenden Kontakte.</param>
    Friend Sub DeleteKontakte(Kontakte As IEnumerable(Of FBoxAPI.Contact))
        With Phonebook.Contacts
            ' Kontake entfernen
            Kontakte.AsParallel.ForAll(Sub(C) .Remove(C))
        End With
    End Sub

    ''' <summary>
    ''' Gibt an, ob das Telefonbuch einen Kontakt mit der gesuchten Telefonnummer enthält.
    ''' </summary>
    ''' <param name="TelNr">Zu suchende Telefonnummer</param>
    Friend Function ContainsNumber(TelNr As Telefonnummer) As Boolean
        Return Phonebook.Contacts.Where(Function(K)
                                            ' interne Telefone sollen nicht duchsucht werden
                                            Return Not K.IstTelefon AndAlso K.Telephony.Numbers.Where(Function(N) TelNr.Equals(N.Number)).Any
                                        End Function).Any
    End Function

    'Friend Async Sub ExtendContacts()

    '    Dim tmpList As New Dictionary(Of Integer, FBoxAPI.Contact)

    '    For Each Contact As FBoxAPI.Contact In Phonebook.Contacts.Where(Function(C) Not C.IstTelefon)
    '        ' Lade den Kontakt erneut mittels GetPhonebookEntryUID herunter und überschreibe ihn.
    '        tmpList.Add(Phonebook.Contacts.IndexOf(Contact), Await Telefonbücher.GetTelefonbuchEintrag(ID, Contact.Uniqueid))
    '    Next

    '    For Each ExtendedContact As KeyValuePair(Of Integer, FBoxAPI.Contact) In tmpList
    '        ' Entferne den alten Eintrag
    '        Phonebook.Contacts.RemoveAt(ExtendedContact.Key)

    '        Phonebook.Contacts.Insert(ExtendedContact.Key, ExtendedContact.Value)
    '    Next

    'End Sub

#End Region

End Class
