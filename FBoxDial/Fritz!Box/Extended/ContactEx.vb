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
                        Case FBoxAPI.TelNrTypEnum.home
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
                        Case FBoxAPI.TelNrTypEnum.mobile
                            If .MobileTelephoneNumber.IsStringNothingOrEmpty Then
                                .MobileTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PagerNumber.IsStringNothingOrEmpty Then
                                .PagerNumber = tmpTelNr.Formatiert
                            ElseIf .RadioTelephoneNumber.IsStringNothingOrEmpty Then
                                .RadioTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case FBoxAPI.TelNrTypEnum.work
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
                        Case FBoxAPI.TelNrTypEnum.fax_work
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
            If Not XMLData.POptionen.CBNoContactNotes Then
                XmlSerializeToString(Contact, .Body)
                .Body = String.Format(Localize.resCommon.strCreateContact, My.Resources.strDefLongName, Now, vbNewLine, .Body)
            End If

            If Contact.Person.ImageURL.IsNotStringNothingOrEmpty And HerunterladenKontaktBild IsNot Nothing Then
                ' Kontaktbild
                Dim Pfad As String = Await HerunterladenKontaktBild
                Dim Kontaktspeichern As Boolean = .Saved
                ' Kontaktbild hinzufügen
                Try
                    ' Füge das Kontaktbild hinzu. Der Kontakt muss danach neu gespeichert werden
                    .AddPicture(Pfad)
                    ' Wenn der Kontakt bereits gespeichert war, dann speichere ihn erneut.
                    If .Speichern Then NLogger.Debug($"Kontakt { .FullNameAndCompanyWithoutLineBreak} nach hinzufügen des Kontaktbildes gespeichert")
                Catch ex As System.Exception
                    NLogger.Warn(ex)
                Finally
                    ' Lösche das Bild auf dem Dateisystem
                    LöscheDatei(Pfad)
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

            Await Globals.ThisAddIn.FBoxTR064.HttpService.DownloadToFileSystem(New Uri(FritzBoxDefault.CompleteURL(Kontakt.Person.ImageURL)), Pfad)

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

    <Extension> Friend Function CompleteImageURL(Person As FBoxAPI.Person, SessionID As String) As String

        If SessionID.IsNotEqual(FritzBoxDefault.DfltFritzBoxSessionID) Then
            ' Bei DAV-Telefonbüchern ist dies der Fall. Das Herunterladen ist dann nicht möglich.
            If Not Person.ImageURL.StartsWith("/download.lua?path=https://localhost/") Then
                Return $"https://{XMLData.POptionen.ValidFBAdr}:{FritzBoxDefault.DfltTR064PortSSL}{Person.ImageURL}&{SessionID}"
            End If
        End If

        Return String.Empty
    End Function

    <Extension> Friend Function GetKontaktTelNrList(Contact As FBoxAPI.Contact) As List(Of Telefonnummer)

        Return Contact.Telephony.Numbers.Select(Function(TelNr) New Telefonnummer With {.SetNummer = TelNr.Number, .Typ = New TelNrType With {.XML = CType(TelNr.Type, FBoxAPI.TelNrTypEnum)}}).ToList

    End Function

    Friend Function CreateContact(InitName As String) As FBoxAPI.Contact

        Return New FBoxAPI.Contact With {.Person = New FBoxAPI.Person With {.RealName = InitName}, .Telephony = New FBoxAPI.Telephony}
    End Function

    Friend Async Function GetPersonImage(Link As String) As Task(Of Imaging.BitmapImage)

        If Link.IsNotStringNothingOrEmpty Then
            ' Setze den Pfad zum Bild zusammen
            Dim b As Byte() = {}
            NLogger.Debug($"Lade Kontaktbild von Pfad: ' {Link} '")
            ' Lade das Bild herunter
            b = Await Globals.ThisAddIn.FBoxTR064.HttpService.GetData(New Uri(Link))
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
