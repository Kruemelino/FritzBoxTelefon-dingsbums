Imports Microsoft.Office.Interop
Imports MixERP.Net.VCards
Friend Module VCard

    ''' <summary>
    ''' Fügt die Informationen einer vCard in ein Kontaktelement ein.
    ''' </summary>
    ''' <param name="vCard">Quelle: Die vCard, die eingelesen werden soll.</param>
    ''' <param name="Kontakt">Ziel: (Rückgabe) Der Kontakt in den die Informationen der vCard geschrieben werden als<c>Outlook.ContactItem</c></param>
    ''' ''' <remarks>https://www.ietf.org/rfc/rfc2426.txt</remarks>
    Friend Sub DeserializevCard(vCard As String, ByRef Kontakt As Outlook.ContactItem)

        With Deserializer.GetVCard(vCard)
            ' insert Name
            Kontakt.FirstName = .FirstName
            Kontakt.LastName = .LastName
            Kontakt.Title = .Title
            Kontakt.Suffix = .Suffix
            Kontakt.NickName = .NickName
            ' insert Jobtitle and Companny
            Kontakt.JobTitle = .Title
            Kontakt.CompanyName = .Organization
            ' insert Telephone Numbers
            For Each vCardTelephone As Models.Telephone In .Telephones
                Using tmpTelNr As New Telefonnummer
                    tmpTelNr.SetNummer = vCardTelephone.Number

                    Select Case vCardTelephone.Type
                        Case Types.TelephoneType.Bbs
                            ' bulletin board system telephone number
                        Case Types.TelephoneType.Car
                            Kontakt.CarTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Cell
                            ' cellular telephone number
                            Kontakt.MobileTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Fax
                            ' facsimile telephone number
                            Kontakt.BusinessFaxNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Home
                            ' telephone number associated with a residence
                            Kontakt.HomeTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Isdn
                            ' ISDN service telephone number
                            Kontakt.ISDNNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Message
                            ' telephone number has voice messaging support
                            Kontakt.CallbackTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Modem
                            ' MODEM connected telephone number
                            Kontakt.Home2TelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Pager
                            ' paging device telephone number
                            Kontakt.PagerNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Personal
                            ' personal communication services telephone number
                            Kontakt.OtherTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Preferred
                            ' preferred-use telephone number
                            Kontakt.PrimaryTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Video
                            ' video conferencing telephone number
                            Kontakt.Business2TelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Voice
                            ' voice telephone number
                            Kontakt.CompanyMainTelephoneNumber = tmpTelNr.Formatiert
                        Case Types.TelephoneType.Work
                            ' telephone number associated with a place of work
                            Kontakt.BusinessTelephoneNumber = tmpTelNr.Formatiert
                    End Select
                End Using
            Next
            ' insert Birthday
            If .BirthDay IsNot Nothing Then
                Kontakt.Birthday = CDate(.BirthDay)
            End If

            ' insert addresses
            For Each vCardAddress As Models.Address In .Addresses
                Select Case vCardAddress.Type
                    Case Types.AddressType.Home
                        Kontakt.HomeAddressCity = vCardAddress.Locality
                        Kontakt.HomeAddressCountry = vCardAddress.Country
                        Kontakt.HomeAddressStreet = vCardAddress.Street
                        Kontakt.HomeAddressState = vCardAddress.Region
                        Kontakt.HomeAddressPostalCode = vCardAddress.PostalCode
                        Kontakt.HomeAddressPostOfficeBox = vCardAddress.PoBox

                    Case Types.AddressType.Work
                        Kontakt.BusinessAddressCity = vCardAddress.Locality
                        Kontakt.BusinessAddressCountry = vCardAddress.Country
                        Kontakt.BusinessAddressStreet = vCardAddress.Street
                        Kontakt.BusinessAddressState = vCardAddress.Region
                        Kontakt.BusinessAddressPostalCode = vCardAddress.PostalCode
                        Kontakt.BusinessAddressPostOfficeBox = vCardAddress.PoBox
                End Select
            Next
            ' insert email-addresses
            If .Emails IsNot Nothing Then
                For Each vCardEMail As Models.Email In .Emails
                    If Kontakt.Email1Address.IsStringNothingOrEmpty Then Kontakt.Email1Address = vCardEMail.EmailAddress
                    If Kontakt.Email2Address.IsStringNothingOrEmpty Then Kontakt.Email2Address = vCardEMail.EmailAddress
                    If Kontakt.Email3Address.IsStringNothingOrEmpty Then Kontakt.Email3Address = vCardEMail.EmailAddress
                Next
            End If
            ' insert URL
            If .Url IsNot Nothing Then
                Kontakt.WebPage = .Url.OriginalString
            End If

        End With

    End Sub

End Module