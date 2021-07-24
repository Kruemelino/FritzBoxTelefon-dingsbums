Imports System.Collections.ObjectModel

Public NotInheritable Class DfltWerteAllgemein

    Public Shared ReadOnly Property DfltOptions As String = "Optionen"
    Public Shared ReadOnly Property DfltDeCryptKey As String = "ZugangV5"
    Public Shared ReadOnly Property DfltPhonerDeCryptKey As String = "ZugangPhoner"
    Public Shared ReadOnly Property DfltTellowsDeCryptKey As String = "ZugangTellows"

#Region "Global Default Value Properties"
    ''' <summary>
    ''' Ein leerer String
    ''' </summary>
    Public Shared ReadOnly Property DfltStringEmpty As String = String.Empty
    ''' <summary>
    ''' Leerzeichen Chr(32), " "
    ''' </summary>
    Public Shared ReadOnly Property DfltStringLeerzeichen As String = Chr(32)
    ''' <summary>
    ''' -1 als String
    ''' Default Fehler
    ''' </summary>
    Public Shared ReadOnly Property DfltStrErrorMinusOne() As String = "-1"

    ''' <summary>
    ''' vbCrLf
    ''' </summary>
    Public Shared ReadOnly Property Dflt1NeueZeile() As String = vbCrLf

    ''' <summary>
    ''' vbCrLf &amp; vbCrLf
    ''' </summary>
    Public Shared ReadOnly Property Dflt2NeueZeile() As String = Dflt1NeueZeile & Dflt1NeueZeile

    ''' <summary>
    ''' 2000
    ''' </summary>
    Public Shared ReadOnly Property DfltReStartIntervall() As Integer = 2000

    ''' <summary>
    ''' 15
    ''' </summary>
    Public Shared ReadOnly Property DfltTryMaxRestart() As Integer = 15

    ''' <summary>
    ''' BEGIN:VCARD
    ''' </summary>
    Public Shared ReadOnly Property DfltBegin_vCard() As String = "BEGIN:VCARD"

    ''' <summary>
    ''' END:VCARD
    ''' </summary>
    Public Shared ReadOnly Property DfltEnd_vCard() As String = "END:VCARD"

    ''' <summary>
    ''' CallList
    ''' </summary>
    ''' <value>CallList</value>
    ''' <returns>CallList</returns>
    Public Shared ReadOnly Property DfltNameListCALL() As String = "CallList"

    ''' <summary>
    ''' RingList
    ''' </summary>
    ''' <value>RingList</value>
    ''' <returns>RingList</returns>
    Public Shared ReadOnly Property DfltNameListRING() As String = "RingList"

    ''' <summary>
    ''' VIPList
    ''' </summary>
    ''' <value>VIPList</value>
    ''' <returns>VIPList</returns>
    Public Shared ReadOnly Property DfltNameListVIP() As String = "VIPList"

    ''' <summary>
    ''' FritzOutlookV5.xml
    ''' </summary>
    ''' <returns>FritzOutlookV5.xml</returns>
    ''' <remarks>Wird mit der Ressource "strDefShortName" erstellt.</remarks>
    Public Shared ReadOnly Property DfltConfigFileName() As String = $"{My.Resources.strDefShortName}.xml"

    '''' <summary>
    '''' FritzOutlookV5.json
    '''' </summary>
    '''' <remarks>Wird mit Ressource "strDefShortName" erstellt.</remarks>
    Public Shared ReadOnly Property DfltTellowsFileName() As String = $"{My.Resources.strDefShortName}.json"

    ''' <summary>
    ''' FritzOutlookV5.log
    ''' </summary>
    ''' <remarks>Wird mit Ressource "strDefShortName" erstellt.</remarks>
    Public Shared ReadOnly Property DfltLogFileName() As String = $"{My.Resources.strDefShortName}.log"

    ''' <summary>
    ''' FritzOutlookV5.{#}.log
    ''' </summary>
    ''' <remarks>Wird mit Ressource "strDefShortName" erstellt.</remarks>
    Public Shared ReadOnly Property DfltLogArchiveFileName() As String = $"{My.Resources.strDefShortName}.{{#}}.log"

    Friend Shared ReadOnly Property DfltErrorvalue As Integer = -2147221233

    Private Shared ReadOnly Property DfltDASLSchema As String = "http://schemas.microsoft.com/mapi/string/{FFF40745-D92F-4C11-9E14-92701F001EB3}/"

    Private Shared ReadOnly Property DfltDASLSchemaUserProperties As String = "http://schemas.microsoft.com/mapi/string/{00020329-0000-0000-C000-000000000046}/"

    Friend Shared ReadOnly Property DfltDASLSMTPAdress As String = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E"

    Friend Shared ReadOnly Property DASLTagJournal As Object()
        Get
            Dim tmpDASLTag(1) As Object
            tmpDASLTag(0) = DfltDASLSchema & "FBDB-ContactEntryID"
            tmpDASLTag(1) = DfltDASLSchema & "FBDB-ContactStoreID"
            Return tmpDASLTag
        End Get
    End Property

    Friend Shared ReadOnly Property DASLTagFBTelBuch As Object()
        Get
            Dim tmpDASLTag(1) As Object
            tmpDASLTag(0) = DfltDASLSchema & "FBDB-PhonebookID"
            tmpDASLTag(1) = DfltDASLSchema & "FBDB-PhonebookEntryID"
            Return tmpDASLTag
        End Get
    End Property

    ''' <summary>
    ''' Ein Array, welches den Namen der UserProperties, die die unformatierte Telefonnummer enthält.
    ''' </summary>
    ''' <value>String-Array</value>
    ''' <returns>String-Array</returns>
    Public Shared ReadOnly Property DfltUserProperties() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String) From {
                DfltDASLSchemaUserProperties & "FBDB-AssistantTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-BusinessTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-Business2TelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-CallbackTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-CarTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-CompanyMainTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-HomeTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-Home2TelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-ISDNNumber",
                DfltDASLSchemaUserProperties & "FBDB-MobileTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-OtherTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-PagerNumber",
                DfltDASLSchemaUserProperties & "FBDB-PrimaryTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-RadioTelephoneNumber",
                DfltDASLSchemaUserProperties & "FBDB-BusinessFaxNumber",
                DfltDASLSchemaUserProperties & "FBDB-HomeFaxNumber",
                DfltDASLSchemaUserProperties & "FBDB-OtherFaxNumber",
                DfltDASLSchemaUserProperties & "FBDB-Telex",
                DfltDASLSchemaUserProperties & "FBDB-TTYTDDTelephoneNumber"
            }

            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    Friend Shared ReadOnly Property DASLTagTelNrIndex As Object()
        Get
            Dim tmpDASLTag(18) As Object
            tmpDASLTag(0) = DfltDASLSchema & "FBDB-AssistantTelephoneNumber"
            tmpDASLTag(1) = DfltDASLSchema & "FBDB-BusinessTelephoneNumber"
            tmpDASLTag(2) = DfltDASLSchema & "FBDB-Business2TelephoneNumber"
            tmpDASLTag(3) = DfltDASLSchema & "FBDB-CallbackTelephoneNumber"
            tmpDASLTag(4) = DfltDASLSchema & "FBDB-CarTelephoneNumber"
            tmpDASLTag(5) = DfltDASLSchema & "FBDB-CompanyMainTelephoneNumber"
            tmpDASLTag(6) = DfltDASLSchema & "FBDB-HomeTelephoneNumber"
            tmpDASLTag(7) = DfltDASLSchema & "FBDB-Home2TelephoneNumber"
            tmpDASLTag(8) = DfltDASLSchema & "FBDB-ISDNNumber"
            tmpDASLTag(9) = DfltDASLSchema & "FBDB-MobileTelephoneNumber"
            tmpDASLTag(10) = DfltDASLSchema & "FBDB-OtherTelephoneNumber"
            tmpDASLTag(11) = DfltDASLSchema & "FBDB-PagerNumber"
            tmpDASLTag(12) = DfltDASLSchema & "FBDB-PrimaryTelephoneNumber"
            tmpDASLTag(13) = DfltDASLSchema & "FBDB-RadioTelephoneNumber"
            tmpDASLTag(14) = DfltDASLSchema & "FBDB-BusinessFaxNumber"
            tmpDASLTag(15) = DfltDASLSchema & "FBDB-HomeFaxNumber"
            tmpDASLTag(16) = DfltDASLSchema & "FBDB-OtherFaxNumber"
            tmpDASLTag(17) = DfltDASLSchema & "FBDB-Telex"
            tmpDASLTag(18) = DfltDASLSchema & "FBDB-TTYTDDTelephoneNumber"
            Return tmpDASLTag
        End Get
    End Property

#End Region

#Region "Literale Anrufmonitor PopUp"
    ''' <summary>
    ''' Kontakt öffnen
    ''' </summary>
    ''' <value>Kontakt öffnen</value>
    ''' <returns>Kontakt öffnen</returns>
    Public Shared ReadOnly Property AnrMonPopUpToolStripMenuItemKontaktöffnen As String = "Kontakt öffnen"

    ''' <summary>
    ''' Kontakt erstellen
    ''' </summary>
    ''' <value>Kontakt erstellen</value>
    ''' <returns>Kontakt erstellen</returns>
    Public Shared ReadOnly Property AnrMonPopUpToolStripMenuItemKontaktErstellen As String = "Kontakt erstellen"

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    ''' <value>Rückruf</value>
    ''' <returns>Rückruf</returns>
    Public Shared ReadOnly Property AnrMonPopUpToolStripMenuItemRückruf As String = "Rückruf"

    ''' <summary>
    ''' In Zwischenablage kopieren
    ''' </summary>
    ''' <value>In Zwischenablage kopieren</value>
    ''' <returns>In Zwischenablage kopieren</returns>
    Public Shared ReadOnly Property AnrMonPopUpToolStripMenuItemKopieren As String = "In Zwischenablage kopieren"
#End Region

#Region "Literale WählClient"
    ''' <summary>
    ''' Anruf: <paramref name="Kontakt"/>
    ''' </summary>
    ''' <param name="Kontakt">Die Kontaktdaten des anzurufenden Kontaktes</param>
    <Obsolete> Public Shared ReadOnly Property WählClientFormText(Kontakt As String) As String
        Get
            Return $"Anruf: {Kontakt}"
        End Get
    End Property

    ''' <summary>
    ''' Es ist kein Kontakt mit der E-Mail-Adresse <paramref name="EMailAdresse"/> vorhanden!
    ''' </summary>
    ''' <param name="EMailAdresse">Die Adresse der ausgewählten E-Mail</param>
    Public Shared ReadOnly Property WählClientEMailunbekannt(EMailAdresse As String) As String
        Get
            Return $"Es ist kein Kontakt mit der E-Mail-Adresse {EMailAdresse} vorhanden!"
        End Get
    End Property

    ''' <summary>
    ''' Es muss entweder ein Kontakt, eine eingegangene E-Mail-Adresse oder ein Journaleintrag ausgewählt sein!
    ''' </summary>
    Public Shared ReadOnly Property WählClientAuswahlFalsch As String = "Es muss entweder ein Kontakt, eine eingegangene E-Mail-Adresse oder ein Journaleintrag ausgewählt sein!"

    ''' <summary>
    ''' Sie sind dabei eine Mobilnummer anzurufen. Fortsetzen?
    ''' </summary>
    ''' <param name="MobilNr">Die Mobilnummer als formatierte Zeichenfolge</param>
    Public Shared ReadOnly Property WählClientFrageMobil(MobilNr As String) As String
        Get
            Return $"Sie sind dabei eine Mobilnummer ({MobilNr}) anzurufen. Fortsetzen?"
        End Get
    End Property

    ''' <summary>
    ''' Fehler
    ''' </summary>
    Public Shared ReadOnly Property WählClientDialFehler As String = "Fehler"

    ''' <summary>
    ''' Bitte warten
    ''' </summary>
    Public Shared ReadOnly Property WählClientBitteWarten As String = "Bitte warten"

    ''' <summary>
    ''' Jetzt abheben
    ''' </summary>
    Public Shared ReadOnly Property WählClientJetztAbheben As String = "Jetzt abheben"

    ''' <summary>
    ''' Abgebrochen
    ''' </summary>
    Public Shared ReadOnly Property WählClientDialHangUp As String = $"Abgebrochen"

    ''' <summary>
    ''' Dialcode: <paramref name="DialCode"/>>
    ''' </summary>
    ''' <param name="DialCode"></param>
    Public Shared ReadOnly Property WählClientStatusWählClient(DialCode As String) As String
        Get
            Return $"Dialcode: {DialCode}"
        End Get
    End Property

    ''' <summary>
    ''' Lade Telefoniegeräte...
    ''' </summary>
    Public Shared ReadOnly Property WählClientStatusLadeGeräte As String = "Lade Telefoniegeräte..."
    Public Shared ReadOnly Property WählClientStatusLetztesGerät As String = "Setze letztes Gerät..."
    Public Shared ReadOnly Property WählClientStatus1Gerät As String = "Setze 1. Gerät in Liste..."
    Public Shared ReadOnly Property WählClientStatusFehlerGerät As String = "Es konnte kein Gerät geladen werden..."
    Public Shared ReadOnly Property WählClientSoftPhoneInaktiv(Softphone As String) As String
        Get
            Return $"{Softphone} ist nicht bereit..."
        End Get
    End Property
    ''' <summary>
    ''' Lade Telefonnummern des Kontaktes...
    ''' </summary>
    Public Shared ReadOnly Property WählClientStatusLadeKontaktTelNr As String = "Lade Telefonnummern des Kontaktes..."

    ''' <summary>
    ''' Lade Telefonnummer...
    ''' </summary>
    Public Shared ReadOnly Property WählClientStatusLadeTelNr As String = "Lade Telefonnummer..."

    ''' <summary>
    ''' Ausgewählt: <paramref name="StrTelNr"/>
    ''' </summary>
    ''' <param name="StrTelNr"></param>
    Public Shared ReadOnly Property WählClientStatusTelNrAuswahl(StrTelNr As String) As String
        Get
            Return $"Ausgewählt: {StrTelNr}"
        End Get
    End Property

    ''' <summary>
    ''' "Anruf wird vorbereitet...
    ''' </summary>
    Public Shared ReadOnly Property WählClientStatusVorbereitung As String = $"Anruf wird vorbereitet...{Dflt1NeueZeile}"

    ''' <summary>
    ''' 
    ''' </summary>
    Public Shared ReadOnly Property WählClientStatusAbbruch As String = $"Anruf wird abgebrochen...{Dflt1NeueZeile}"


#End Region

#Region "Literale Journal"

    Public Shared ReadOnly Property DfltJournalDefCategories() As ReadOnlyCollection(Of String)
        Get
            Return New ReadOnlyCollection(Of String)({Localize.LocAnrMon.strJournalCatDefault, Localize.LocAnrMon.strJournalCatCalls})
        End Get
    End Property

    ''' <summary>
    ''' Tel.-Nr.: TelNr Status: (nicht) angenommen    
    ''' </summary>
    ''' <param name="TelNr">Tekefonnummer</param>
    ''' <param name="Angenommen">Boolean, ob das Telefon angenommen wurde oder nicht</param>
    Public Shared ReadOnly Property DfltJournalBody(TelNr As String, Angenommen As Boolean, vCard As String) As String
        Get
            Return $"{Localize.LocAnrMon.strJournalBodyStart} {TelNr}{Dflt1NeueZeile}Status: {If(Angenommen, DfltStringEmpty, "nicht ")}angenommen{Dflt2NeueZeile}{vCard}"
        End Get
    End Property
#End Region

End Class

