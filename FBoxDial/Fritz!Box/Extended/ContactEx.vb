Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports System.Windows.Media
Imports Microsoft.Office.Interop.Outlook
Friend Module ContactEx
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    <Extension> Friend Function IstTelefon(Contact As FBoxAPI.Contact) As Boolean

        If Contact.Telephony IsNot Nothing Then
            If Contact.Telephony.Numbers IsNot Nothing AndAlso Contact.Telephony.Numbers.Any Then
                Return Contact.Telephony.Numbers.Where(Function(N) N.Type = XMLTelNrTyp.intern Or N.Number.StartsWith("*")).Any
            End If
        End If

        Return False

    End Function

    <Extension> Friend Async Sub XMLKontaktOutlook(Contact As FBoxAPI.Contact, Kontakt As ContactItem)
        Dim HerunterladenKontaktBild As Task(Of String) = Nothing
        ' Werte übeführen
        With Kontakt
            ' Name
            .FullName = Contact.Person.RealName

            ' Kontaktbild asynchron herunterladen
            If Contact.Person.ImageURL.IsNotStringNothingOrEmpty Then HerunterladenKontaktBild = KontaktBildPfad(Contact)

            ' E-Mail Adressen (Es gibt in Outlook maximal 3 E-Mail Adressen)
            For i = 1 To Math.Min(Contact.Telephony.Emails.Count, 3)
                Select Case i
                    Case 1
                        .Email1Address = Contact.Telephony.Emails.Item(i - 1).EMail
                    Case 2
                        .Email2Address = Contact.Telephony.Emails.Item(i - 1).EMail
                    Case 3
                        .Email3Address = Contact.Telephony.Emails.Item(i - 1).EMail
                    Case Else
                        Exit Select
                End Select
            Next

            ' Telefonnummern
            For Each TelNr In Contact.Telephony.Numbers
                Using tmpTelNr As New Telefonnummer With {.SetNummer = TelNr.Number}
                    ' Zuordnung zu den Kategorien                    
                    ' Type = "home":    .CarTelephoneNumber, .HomeTelephoneNumber, .Home2TelephoneNumber, .ISDNNumber, .TTYTDDTelephoneNumber, .OtherTelephoneNumber                           
                    ' Type = "mobile":  .MobileTelephoneNumber, .PagerNumber, .RadioTelephoneNumber
                    ' Type = "work":    .AssistantTelephoneNumber, .BusinessTelephoneNumber, .Business2TelephoneNumber, .CallbackTelephoneNumber, .CompanyMainTelephoneNumber, .PrimaryTelephoneNumber
                    ' Type = "fax_work: .BusinessFaxNumber, .HomeFaxNumber, .OtherFaxNumber, .TelexNumber
                    Select Case TelNr.Type
                        Case FBoxAPI.TelNrTyp.home
                            If .HomeTelephoneNumber.IsStringNothingOrEmpty Then
                                .HomeTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .Home2TelephoneNumber.IsStringNothingOrEmpty Then
                                .Home2TelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CarTelephoneNumber.IsStringNothingOrEmpty Then
                                .CarTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .OtherTelephoneNumber.IsStringNothingOrEmpty Then
                                .OtherTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .ISDNNumber.IsStringNothingOrEmpty Then
                                .ISDNNumber = tmpTelNr.Formatiert
                            ElseIf .TTYTDDTelephoneNumber.IsStringNothingOrEmpty Then
                                .TTYTDDTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case FBoxAPI.TelNrTyp.mobile
                            If .MobileTelephoneNumber.IsStringNothingOrEmpty Then
                                .MobileTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PagerNumber.IsStringNothingOrEmpty Then
                                .PagerNumber = tmpTelNr.Formatiert
                            ElseIf .RadioTelephoneNumber.IsStringNothingOrEmpty Then
                                .RadioTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case FBoxAPI.TelNrTyp.work
                            If .BusinessTelephoneNumber.IsStringNothingOrEmpty Then
                                .BusinessTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .Business2TelephoneNumber.IsStringNothingOrEmpty Then
                                .Business2TelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .AssistantTelephoneNumber.IsStringNothingOrEmpty Then
                                .AssistantTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CallbackTelephoneNumber.IsStringNothingOrEmpty Then
                                .CallbackTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CompanyMainTelephoneNumber.IsStringNothingOrEmpty Then
                                .CompanyMainTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PrimaryTelephoneNumber.IsStringNothingOrEmpty Then
                                .PrimaryTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case FBoxAPI.TelNrTyp.fax_work
                            If .BusinessFaxNumber.IsStringNothingOrEmpty Then
                                .BusinessFaxNumber = tmpTelNr.Formatiert
                            ElseIf .HomeFaxNumber.IsStringNothingOrEmpty Then
                                .HomeFaxNumber = tmpTelNr.Formatiert
                            ElseIf .OtherFaxNumber.IsStringNothingOrEmpty Then
                                .OtherFaxNumber = tmpTelNr.Formatiert
                            ElseIf .TelexNumber.IsStringNothingOrEmpty Then
                                .TelexNumber = tmpTelNr.Formatiert
                            End If
                    End Select
                End Using
            Next
            ' Body
            If Not XMLData.POptionen.CBNoContactNotes Then XmlSerializeToString(Contact, .Body)

            If Contact.Person.ImageURL.IsNotStringNothingOrEmpty And HerunterladenKontaktBild IsNot Nothing Then
                ' Kontaktbild
                Dim Pfad As String = Await HerunterladenKontaktBild
                Dim Kontaktspeichern As Boolean = .Saved
                ' Kontaktbild hinzufügen
                Try
                    ' Füge das Kontaktbild hinzu. Der Kontakt muss danach neu gespeichert werden
                    .AddPicture(Pfad)
                    ' Wenn der Kontakt bereits gespeichert war, dann speichere ihn erneut.
                    If .Speichern Then NLogger.Debug($"Kontakt { .FullNameAndCompany} nach hinzufügen des Kontaktbildes gespeichert")
                Catch ex As System.Exception
                    NLogger.Warn(ex)
                Finally
                    ' Lösche das Bild auf dem Dateisystem
                    DelKontaktBild(Pfad)
                End Try
            End If

        End With
    End Sub

    <Extension> Friend Async Function KontaktBild(Contact As FBoxAPI.Contact) As Task(Of Imaging.BitmapImage)
        If Contact IsNot Nothing Then
            With Contact
                If .Person.ImageURL.IsNotStringNothingOrEmpty Then
                    ' Bild in das Datenobjekt laden und abschließend löschen
                    Return KontaktBildEx(Await .KontaktBildPfad)
                End If
            End With
        End If
        Return Nothing
    End Function

    <Extension> Friend Async Function KontaktBildPfad(Kontakt As FBoxAPI.Contact) As Task(Of String)
        Dim Pfad As String = String.Empty
        If Kontakt IsNot Nothing Then
            Pfad = $"{Path.GetTempPath}{Path.GetRandomFileName}" '.RegExReplace(".{3}$", "jpg")

            Await DownloadToFileTaskAsync(New Uri(CompleteImageURL(Kontakt.Person)), Pfad)

            NLogger.Debug($"Bild des Kontaktes {Kontakt.Person.RealName} unter Pfad {Pfad} gespeichert.")
        End If
        Return Pfad
    End Function

    <Extension> Friend Function GetXMLKontakt(Kontakt As FBoxAPI.Contact) As String
        Dim XMLKontakt As String = String.Empty

        XmlSerializeToString(Kontakt, XMLKontakt)

        NLogger.Trace($"Kontakt {Kontakt.Person.RealName} serialisiert: {XMLKontakt}")

        Return XMLKontakt
    End Function

    <Extension> Friend Function CompleteImageURL(Person As FBoxAPI.Person) As String
        Dim SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID

        ' Wird bei Anzeige im Anrufmonitor benötigt.
        'If Ping(XMLData.POptionen.ValidFBAdr) Then
        Using fbtr064 As New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, XMLData.POptionen.TBNetworkTimeout, FritzBoxDefault.Anmeldeinformationen)
            fbtr064.Deviceconfig.GetSessionID(SessionID)
            ' Session ID erhalten, ansonsten DfltFritzBoxSessionID
        End Using
        'End If

        Return If(SessionID.IsNotEqual(FritzBoxDefault.DfltFritzBoxSessionID), $"https://{XMLData.POptionen.ValidFBAdr}:{FritzBoxDefault.DfltTR064PortSSL}{Person.ImageURL}&{SessionID}", String.Empty)
    End Function

    <Extension> Friend Function CompleteImageURL(Person As FBoxAPI.Person, SessionID As String) As String
        Return If(SessionID.IsNotEqual(FritzBoxDefault.DfltFritzBoxSessionID), $"https://{XMLData.POptionen.ValidFBAdr}:{FritzBoxDefault.DfltTR064PortSSL}{Person.ImageURL}&{SessionID}", String.Empty)
    End Function

    <Extension> Friend Function GetKontaktTelNrList(Contact As FBoxAPI.Contact) As List(Of Telefonnummer)

        Return Contact.Telephony.Numbers.Select(Function(TelNr) New Telefonnummer With {.SetNummer = TelNr.Number, .Typ = New TelNrType With {.XML = CType(TelNr.Type, FBoxAPI.TelNrTyp)}}).ToList

    End Function

    Friend Function CreateContact(InitName As String) As FBoxAPI.Contact

        Return New FBoxAPI.Contact With {.Person = New FBoxAPI.Person With {.RealName = InitName},
                                         .Telephony = New FBoxAPI.Telephony With {.Numbers = New List(Of FBoxAPI.Number),
                                                                                  .Emails = New List(Of FBoxAPI.Email)}}
    End Function

    Friend Async Function GetPersonImage(Link As String) As Task(Of Imaging.BitmapImage)

        If Link.IsNotStringNothingOrEmpty Then
            ' Setze den Pfad zum Bild zusammen
            Dim b As Byte() = {}
            NLogger.Debug($"Lade Kontaktbild von Pfad: ' {Link} '")
            ' Lade das Bild herunter
            b = Await DownloadDataTaskAsync(New Uri(Link))
            If b.Any Then
                Dim biImg As New Imaging.BitmapImage()
                Dim ms As New MemoryStream(b)

                With biImg
                    .BeginInit()
                    .StreamSource = ms
                    .EndInit()
                End With

                Return biImg
            End If
        End If

        Return Nothing
    End Function

End Module
