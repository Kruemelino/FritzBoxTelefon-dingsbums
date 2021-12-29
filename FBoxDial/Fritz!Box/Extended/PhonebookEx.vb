Public Class PhonebookEx
    Friend Property Phonebook As FBoxAPI.Phonebook

    Public Sub New(phonebook As FBoxAPI.Phonebook)
        _Phonebook = phonebook
    End Sub

    Friend Property ID As Integer
    Friend Property Rufsperren As Boolean = False
    Friend Property Name As String
#Region "Funktionen"

    ''' <summary>
    ''' Fügt den übergebenen Kontakt hinzu. 
    ''' Kontakte mit der selben ID werden entfernt (sollte beim Aktualisieren nur einer sein.
    ''' </summary>
    ''' <param name="Kontakt"></param>
    Friend Sub AddContact(Kontakt As FBoxAPI.Contact)
        With Phonebook.Contacts
            ' Kontakt hinzufügen
            .Add(Kontakt)
        End With
    End Sub

    ''' <summary>
    ''' Entfernt einen Kontakt aus der Liste.
    ''' </summary>
    ''' <param name="Kontakt">Der zu entfernende Kontakt.</param>
    Friend Sub DeleteKontakt(Kontakt As FBoxAPI.Contact)
        With Phonebook.Contacts
            ' Kontake entfernen
            .Remove(Kontakt)
        End With
    End Sub

    ''' <summary>
    ''' Entfernt eine Auflistung von Kontakten aus dem Telefonbuch
    ''' </summary>
    ''' <param name="RemoveKontakte">Liste der zu entfernenden Kontakte.</param>
    Friend Sub DeleteKontakte(RemoveKontakte As IEnumerable(Of FBoxAPI.Contact))
        With Phonebook.Contacts
            ' Kontake entfernen
            RemoveKontakte.AsParallel.ForAll(Sub(C) .Remove(C))
        End With
    End Sub

    ''' <summary>
    ''' Durchsucht die Kontakte nach der übergebenen Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Zu suchende Telefonnummer als Typ: <see cref="Telefonnummer"/></param>
    ''' <returns>Eine Auflistung aller infrage kommenden Kontakte.</returns>
    Friend Function FindbyNumber(TelNr As Telefonnummer) As IEnumerable(Of FBoxAPI.Contact)
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
    Friend Function FindbyNumber(TelNr As String) As IEnumerable(Of FBoxAPI.Contact)
        Return Phonebook.Contacts.Where(Function(K)
                                            ' interne Telefone sollen nicht duchsucht werden
                                            Return Not K.IstTelefon AndAlso K.Telephony.Numbers.Where(Function(N) TelNr.Equals(N.Number)).Any
                                        End Function)
    End Function

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

#End Region

End Class
