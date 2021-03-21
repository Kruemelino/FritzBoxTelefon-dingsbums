Imports System.ComponentModel
Imports System.Xml.Serialization

Friend Structure EMailType
    Friend Addresse As String

    Friend OutlookTyp As OutlookEMailType
End Structure

Friend Enum OutlookEMailType
    SMTP
    EX
End Enum


Public Structure TelNrType
    Public Property TelNrType As OutlookNrType

    Friend ReadOnly Property XML As XMLTelNrTyp
        Get
            Select Case TelNrType
                Case OutlookNrType.CarTelephoneNumber, OutlookNrType.HomeTelephoneNumber, OutlookNrType.Home2TelephoneNumber, OutlookNrType.ISDNNumber, OutlookNrType.TTYTDDTelephoneNumber, OutlookNrType.OtherTelephoneNumber
                    Return XMLTelNrTyp.home
                Case OutlookNrType.MobileTelephoneNumber, OutlookNrType.PagerNumber, OutlookNrType.RadioTelephoneNumber
                    Return XMLTelNrTyp.mobile
                Case OutlookNrType.AssistantTelephoneNumber, OutlookNrType.BusinessTelephoneNumber, OutlookNrType.Business2TelephoneNumber, OutlookNrType.CallbackTelephoneNumber, OutlookNrType.CompanyMainTelephoneNumber, OutlookNrType.PrimaryTelephoneNumber
                    Return XMLTelNrTyp.work
                Case OutlookNrType.BusinessFaxNumber, OutlookNrType.HomeFaxNumber, OutlookNrType.OtherFaxNumber, OutlookNrType.TelexNumber
                    Return XMLTelNrTyp.fax_work
                Case Else
                    Return XMLTelNrTyp.notset
            End Select

        End Get
    End Property

End Structure

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum OutlookNrType
    <LocalizedDescription("AssistantTelephoneNumber", GetType(resEnum))>
    AssistantTelephoneNumber

    <LocalizedDescription("BusinessTelephoneNumber", GetType(resEnum))>
    BusinessTelephoneNumber

    <LocalizedDescription("Business2TelephoneNumber", GetType(resEnum))>
    Business2TelephoneNumber

    <LocalizedDescription("CallbackTelephoneNumber", GetType(resEnum))>
    CallbackTelephoneNumber

    <LocalizedDescription("CarTelephoneNumber", GetType(resEnum))>
    CarTelephoneNumber

    <LocalizedDescription("CompanyMainTelephoneNumber", GetType(resEnum))>
    CompanyMainTelephoneNumber

    <LocalizedDescription("HomeTelephoneNumber", GetType(resEnum))>
    HomeTelephoneNumber

    <LocalizedDescription("Home2TelephoneNumber", GetType(resEnum))>
    Home2TelephoneNumber

    <LocalizedDescription("ISDNNumber", GetType(resEnum))>
    ISDNNumber

    <LocalizedDescription("MobileTelephoneNumber", GetType(resEnum))>
    MobileTelephoneNumber

    <LocalizedDescription("OtherTelephoneNumber", GetType(resEnum))>
    OtherTelephoneNumber

    <LocalizedDescription("PagerNumber", GetType(resEnum))>
    PagerNumber

    <LocalizedDescription("PrimaryTelephoneNumber", GetType(resEnum))>
    PrimaryTelephoneNumber

    <LocalizedDescription("RadioTelephoneNumber", GetType(resEnum))>
    RadioTelephoneNumber

    <LocalizedDescription("BusinessFaxNumber", GetType(resEnum))>
    BusinessFaxNumber

    <LocalizedDescription("HomeFaxNumber", GetType(resEnum))>
    HomeFaxNumber

    <LocalizedDescription("OtherFaxNumber", GetType(resEnum))>
    OtherFaxNumber

    <LocalizedDescription("TelexNumber", GetType(resEnum))>
    TelexNumber

    <LocalizedDescription("TTYTDDTelephoneNumber", GetType(resEnum))>
    TTYTDDTelephoneNumber
End Enum

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum XMLTelNrTyp
    <LocalizedDescription("notset", GetType(resEnum))>
    <XmlEnum("")> notset

    <LocalizedDescription("intern", GetType(resEnum))>
    <XmlEnum("intern")> intern

    <LocalizedDescription("work", GetType(resEnum))>
    <XmlEnum("work")> work

    <LocalizedDescription("home", GetType(resEnum))>
    <XmlEnum("home")> home

    <LocalizedDescription("mobile", GetType(resEnum))>
    <XmlEnum("mobile")> mobile

    <LocalizedDescription("fax_work", GetType(resEnum))>
    <XmlEnum("fax_work")> fax_work

    <LocalizedDescription("memo", GetType(resEnum))>
    <XmlEnum("memo")> memo

    <LocalizedDescription("other", GetType(resEnum))>
    <XmlEnum("other")> other

    ' Das AVM Telefonbuch nimmt es mit der Groß- und Kleinschreibung nicht so genau.
    ' Für die XML - Deserialsierung ist dies aber extrem wichtig.

End Enum

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum XMLEMailTyp
    <LocalizedDescription("Sonstige", GetType(resEnum))>
    <XmlEnum("")> notset

    <LocalizedDescription("private", GetType(resEnum))>
    <XmlEnum("private")> [private]

    <LocalizedDescription("work", GetType(resEnum))>
    <XmlEnum("work")> work

    ' Das AVM Telefonbuch nimmt es mit der Groß- und Kleinschreibung nicht so genau.
    ' Für die XML - Deserialsierung ist dies aber extrem wichtig.

End Enum

