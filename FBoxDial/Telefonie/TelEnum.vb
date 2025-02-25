﻿Imports System.ComponentModel
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
    Friend Property XML As FBoxAPI.TelNrTypEnum
        Get
            Select Case TelNrType
                Case OutlookNrType.CarTelephoneNumber, OutlookNrType.HomeTelephoneNumber, OutlookNrType.Home2TelephoneNumber, OutlookNrType.ISDNNumber, OutlookNrType.TTYTDDTelephoneNumber, OutlookNrType.OtherTelephoneNumber
                    Return FBoxAPI.TelNrTypEnum.home
                Case OutlookNrType.MobileTelephoneNumber, OutlookNrType.PagerNumber, OutlookNrType.RadioTelephoneNumber
                    Return FBoxAPI.TelNrTypEnum.mobile
                Case OutlookNrType.AssistantTelephoneNumber, OutlookNrType.BusinessTelephoneNumber, OutlookNrType.Business2TelephoneNumber, OutlookNrType.CallbackTelephoneNumber, OutlookNrType.CompanyMainTelephoneNumber, OutlookNrType.PrimaryTelephoneNumber
                    Return FBoxAPI.TelNrTypEnum.work
                Case OutlookNrType.BusinessFaxNumber, OutlookNrType.HomeFaxNumber, OutlookNrType.OtherFaxNumber, OutlookNrType.TelexNumber
                    Return FBoxAPI.TelNrTypEnum.fax_work
                Case Else
                    Return FBoxAPI.TelNrTypEnum.notset
            End Select

        End Get
        Set
            Select Case Value
                Case FBoxAPI.TelNrTypEnum.notset, FBoxAPI.TelNrTypEnum.other, FBoxAPI.TelNrTypEnum.intern
                    TelNrType = OutlookNrType.OtherFaxNumber
                Case FBoxAPI.TelNrTypEnum.work
                    TelNrType = OutlookNrType.BusinessTelephoneNumber
                Case FBoxAPI.TelNrTypEnum.home
                    TelNrType = OutlookNrType.HomeTelephoneNumber
                Case FBoxAPI.TelNrTypEnum.mobile
                    TelNrType = OutlookNrType.MobileTelephoneNumber
                Case FBoxAPI.TelNrTypEnum.fax_work
                    TelNrType = OutlookNrType.HomeFaxNumber
                Case FBoxAPI.TelNrTypEnum.memo
                    TelNrType = OutlookNrType.CallbackTelephoneNumber
            End Select
        End Set
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

    <LocalizedDescription("notset", GetType(resEnum))>
    <XmlEnum("")> notset

    <LocalizedDescription("private", GetType(resEnum))>
    <XmlEnum("private")> [private]

    <LocalizedDescription("work", GetType(resEnum))>
    <XmlEnum("work")> work

    <LocalizedDescription("other", GetType(resEnum))>
    <XmlEnum("other")> other

    ' Das AVM Telefonbuch nimmt es mit der Groß- und Kleinschreibung nicht so genau.
    ' Für die XML - Deserialsierung ist dies aber extrem wichtig.

End Enum

''' <summary>
''' Fritz!Box Deflection
''' </summary>
<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum ModeEnum
    ''' <summary>
    ''' Deflect if a bell blockade is activ
    ''' </summary>
    <LocalizedDescription("BellBlockade", GetType(resEnum))>
    <XmlEnum> eBellBlockade

    ''' <summary>
    ''' Busy
    ''' </summary>
    <LocalizedDescription("Busy", GetType(resEnum))>
    <XmlEnum> eBusy

    ''' <summary>
    ''' Deflect with a delay
    ''' </summary>
    <LocalizedDescription("Delayed", GetType(resEnum))>
    <XmlEnum> eDelayed

    ''' <summary>
    ''' Deflect if busy or with a delay
    ''' </summary>
    <LocalizedDescription("DelayedOrBusy", GetType(resEnum))>
    <XmlEnum> eDelayedOrBusy

    ''' <summary>
    ''' Direct call
    ''' </summary>
    <LocalizedDescription("DirectCall", GetType(resEnum))>
    <XmlEnum> eDirectCall

    ''' <summary>
    ''' Deflect immediately
    ''' </summary>
    <LocalizedDescription("Immediately", GetType(resEnum))>
    <XmlEnum> eImmediately

    ''' <summary>
    ''' Deflect with a long delay
    ''' </summary>
    <LocalizedDescription("LongDelayed", GetType(resEnum))>
    <XmlEnum> eLongDelayed

    ''' <summary>
    ''' Do not signal this call
    ''' </summary>
    <LocalizedDescription("NoSignal", GetType(resEnum))>
    <XmlEnum> eNoSignal

    ''' <summary>
    ''' Deflect disabled
    ''' </summary>
    <LocalizedDescription("Off", GetType(resEnum))>
    <XmlEnum> eOff

    ''' <summary>
    ''' Parallel call
    ''' </summary>
    <LocalizedDescription("ParallelCall", GetType(resEnum))>
    <XmlEnum> eParallelCall

    ''' <summary>
    ''' Deflect with a short delay
    ''' </summary>
    <LocalizedDescription("ShortDelayed", GetType(resEnum))>
    <XmlEnum> eShortDelayed

    ''' <summary>
    ''' Mode unknown
    ''' </summary>
    <LocalizedDescription("Unknown", GetType(resEnum))>
    <XmlEnum> eUnknown

    ''' <summary>
    ''' VIP
    ''' </summary>
    <LocalizedDescription("VIP", GetType(resEnum))>
    <XmlEnum> eVIP
End Enum

''' <summary>
''' Fritz!Box Deflection
''' </summary>
<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum TypeEnum
    ''' <summary>
    ''' Phone port 1 is selected
    ''' </summary>
    <LocalizedDescription("fon1", GetType(resEnum))>
    <XmlEnum> fon1 = 1

    ''' <summary>
    ''' Phone port 2 is selected
    ''' </summary>
    <LocalizedDescription("fon2", GetType(resEnum))>
    <XmlEnum> fon2 = 2

    ''' <summary>
    ''' Phone port 3 is selected
    ''' </summary>
    <LocalizedDescription("fon3", GetType(resEnum))>
    <XmlEnum> fon3 = 3

    ''' <summary>
    ''' Phone port 4 is selected
    ''' </summary>
    <LocalizedDescription("fon4", GetType(resEnum))>
    <XmlEnum> fon4 = 4

    ''' <summary>
    ''' From all
    ''' </summary>
    <LocalizedDescription("fromAll", GetType(resEnum))>
    <XmlEnum> fromAll = 5

    ''' <summary>
    ''' From a anonymous call 
    ''' </summary>
    <LocalizedDescription("fromAnonymous", GetType(resEnum))>
    <XmlEnum> fromAnonymous = 6

    ''' <summary>
    ''' The caller is not in the phonebook 
    ''' </summary>
    <LocalizedDescription("fromNotInPhonebook", GetType(resEnum))>
    <XmlEnum> fromNotInPhonebook = 7

    ''' <summary>
    ''' Call not from a VIP (obsolate from Version 37)
    ''' </summary>
    <LocalizedDescription("fromNotVIP", GetType(resEnum))>
    <Obsolete("Obsolate from Version 37")> <XmlEnum> fromNotVIP = 8

    ''' <summary>
    ''' Specific Number 
    ''' </summary>
    <LocalizedDescription("fromNumber", GetType(resEnum))>
    <XmlEnum> fromNumber = 9

    ''' <summary>
    ''' The caller is in the phonebook
    ''' </summary>
    <LocalizedDescription("fromPB", GetType(resEnum))>
    <XmlEnum> fromPB = 10

    ''' <summary>
    ''' Call from a VIP
    ''' </summary>
    <LocalizedDescription("fromVIP", GetType(resEnum))>
    <XmlEnum> fromVIP = 11

    ''' <summary>
    ''' To Any
    ''' </summary>
    <LocalizedDescription("toAny", GetType(resEnum))>
    <XmlEnum> toAny = 12

    ''' <summary>
    ''' To MSN
    ''' </summary>
    <LocalizedDescription("toMSN", GetType(resEnum))>
    <XmlEnum> toMSN = 13

    ''' <summary>
    ''' To POTS
    ''' </summary>
    <LocalizedDescription("toPOTS", GetType(resEnum))>
    <XmlEnum> toPOTS = 14

    ''' <summary>
    ''' To VoIP
    ''' </summary>
    <LocalizedDescription("toVoIP", GetType(resEnum))>
    <XmlEnum> toVoIP = 15

    ''' <summary>
    ''' Type unknown
    ''' </summary>
    <LocalizedDescription("Unknown", GetType(resEnum))>
    <XmlEnum> unknown = 16
End Enum

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum TypeEnumSIP
    <LocalizedDescription("eAllCalls", GetType(resEnum))>
    <XmlEnum> eAllCalls

    <LocalizedDescription("eGSM", GetType(resEnum))>
    <XmlEnum> eGSM

    <LocalizedDescription("eISDN", GetType(resEnum))>
    <XmlEnum> eISDN

    <LocalizedDescription("eNone", GetType(resEnum))>
    <XmlEnum> eNone

    <LocalizedDescription("ePOTS", GetType(resEnum))>
    <XmlEnum> ePOTS

    <LocalizedDescription("eVoIP", GetType(resEnum))>
    <XmlEnum> eVoIP
End Enum

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum IPPhoneConnectorType
    <LocalizedDescription("IPConnNone", GetType(resEnum))>
    <XmlEnum> None

    <LocalizedDescription("IPConnPhoner", GetType(resEnum))>
    <XmlEnum> Phoner

    <LocalizedDescription("IPConnCMD", GetType(resEnum))>
    <XmlEnum> CMD

    <LocalizedDescription("IPConnURI", GetType(resEnum))>
    <XmlEnum> URI
End Enum

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum SoftPhones
    <LocalizedDescription("SoftPhonePhonerLite", GetType(resEnum))> PhonerLite
    <LocalizedDescription("SoftPhoneMicroSIP", GetType(resEnum))> MicroSIP
    <LocalizedDescription("SoftPhonePhoneSuite", GetType(resEnum))> PhoneSuite
    <LocalizedDescription("SoftPhoneOther", GetType(resEnum))> Anderes
End Enum
