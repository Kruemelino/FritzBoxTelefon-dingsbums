Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.Office.Interop.Outlook

<Serializable(), XmlType("contact")> Public Class FritzBoxXMLKontakt
    Inherits NotifyBase

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger


    Public Sub New()
        Person = New FritzBoxXMLPerson
        Telefonie = New FritzBoxXMLTelefonie
    End Sub

    Private _Kategorie As Integer
    ''' <summary>
    ''' Wichtige Person = 1
    ''' </summary>
    <XmlElement("category")> Public Property Kategorie As Integer
        Get
            Return _Kategorie
        End Get
        Set
            SetProperty(_Kategorie, Value)
        End Set
    End Property

    Private _Person As FritzBoxXMLPerson
    <XmlElement("person")> Public Property Person As FritzBoxXMLPerson
        Get
            Return _Person
        End Get
        Set
            SetProperty(_Person, Value)
        End Set
    End Property

    Private _Uniqueid As Integer
    ''' <summary>
    ''' Unique ID for a single contact (new since 2013-04-20) 
    ''' </summary> 
    <XmlElement("uniqueid")> Public Property Uniqueid As Integer
        Get
            Return _Uniqueid
        End Get
        Set
            SetProperty(_Uniqueid, Value)
        End Set
    End Property

    Private _Telefonie As FritzBoxXMLTelefonie
    <XmlElement("telephony")> Public Property Telefonie As FritzBoxXMLTelefonie
        Get
            Return _Telefonie
        End Get
        Set
            SetProperty(_Telefonie, Value)
        End Set
    End Property

    <XmlIgnore> Public ReadOnly Property IstTelefon As Boolean
        Get
            If Telefonie IsNot Nothing Then
                If Telefonie.Nummern IsNot Nothing AndAlso Telefonie.Nummern.Any Then
                    Return Telefonie.Nummern.Where(Function(N) N.Typ = XMLTelNrTyp.intern Or N.Nummer.StartsWith("*")).Any
                End If
            End If

            Return False
        End Get
    End Property
    '
    Friend Sub XMLKontaktOutlook(ByRef Kontakt As ContactItem)
        ' Werte übeführen
        With Kontakt
            ' Name
            .FullName = Person.RealName
            ' E-Mail Adressen (Es gibt in Outlook maximal 3 E-Mail Adressen)
            For i = 1 To Math.Min(Telefonie.Emails.Count, 3)
                Select Case i
                    Case 1
                        .Email1Address = Telefonie.Emails.Item(i - 1).EMail
                    Case 2
                        .Email2Address = Telefonie.Emails.Item(i - 1).EMail
                    Case 3
                        .Email3Address = Telefonie.Emails.Item(i - 1).EMail
                End Select
            Next
            ' Telefonnummern
            For Each TelNr As FritzBoxXMLNummer In Telefonie.Nummern
                Using tmpTelNr As New Telefonnummer With {.SetNummer = TelNr.Nummer}
                    ' Zuordnung zu den Kategorien                    
                    ' Type = "home":    .CarTelephoneNumber, .HomeTelephoneNumber, .Home2TelephoneNumber, .ISDNNumber, .TTYTDDTelephoneNumber, .OtherTelephoneNumber                           
                    ' Type = "mobile":  .MobileTelephoneNumber, .PagerNumber, .RadioTelephoneNumber
                    ' Type = "work":    .AssistantTelephoneNumber, .BusinessTelephoneNumber, .Business2TelephoneNumber, .CallbackTelephoneNumber, .CompanyMainTelephoneNumber, .PrimaryTelephoneNumber
                    ' Type = "fax_work: .BusinessFaxNumber, .HomeFaxNumber, .OtherFaxNumber, .TelexNumber
                    Select Case TelNr.Typ
                        Case XMLTelNrTyp.home
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
                        Case XMLTelNrTyp.mobile
                            If .MobileTelephoneNumber.IsStringNothingOrEmpty Then
                                .MobileTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PagerNumber.IsStringNothingOrEmpty Then
                                .PagerNumber = tmpTelNr.Formatiert
                            ElseIf .RadioTelephoneNumber.IsStringNothingOrEmpty Then
                                .RadioTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case XMLTelNrTyp.work
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
                        Case XMLTelNrTyp.fax_work
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
            XmlSerializeToString(Me, .Body)

        End With
    End Sub

    Friend Function GetXMLKontakt() As String
        Dim XMLKontakt As String = DfltStringEmpty

        XmlSerializeToString(Me, XMLKontakt)
        NLogger.Debug($"Kontakt {Person.RealName} serialisiert: {XMLKontakt}")

        Return XMLKontakt
    End Function
End Class
