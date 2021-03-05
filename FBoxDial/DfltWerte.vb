Imports System.Collections.ObjectModel

Public NotInheritable Class DfltWerteAllgemein

    Public Shared ReadOnly Property DfltOptions As String = "Optionen"
    Public Shared ReadOnly Property DfltDeCryptKey As String = "ZugangV5"

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
    ''' String: unbekannt
    ''' </summary>
    Public Shared ReadOnly Property DfltStringUnbekannt() As String = "unbekannt"

    '''' <summary>
    '''' 0000000000000000
    '''' </summary>
    'Public Shared ReadOnly Property PDfltSessionID() As String = "0000000000000000"

    ''' <summary>
    ''' Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)
    ''' </summary>
    Public Shared ReadOnly Property DfltHeader_UserAgent() As String = "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"

    ''' <summary>
    ''' text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
    ''' </summary>
    Public Shared ReadOnly Property DfltHeader_Accept() As String = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"

    ''' <summary>
    ''' 2000
    ''' </summary>
    Public Shared ReadOnly Property DfltReStartIntervall() As Integer = 2000

    ''' <summary>
    ''' 15
    ''' </summary>
    Public Shared ReadOnly Property DfltTryMaxRestart() As Integer = 15

    '''' <summary>
    '''' [-&gt;]
    '''' </summary>
    'Public Shared ReadOnly Property PDfltAnrMonDirection_Call() As String = "[->]"

    '''' <summary>
    '''' [&lt;-]
    '''' </summary>
    'Public Shared ReadOnly Property PDfltAnrMonDirection_Ring() As String = "[<-]"

    '''' <summary>
    '''' [&lt;&gt;]
    '''' </summary>
    'Public Shared ReadOnly Property PDfltAnrMonDirection_Default() As String = "[<>]"

    ''' <summary>
    ''' FBDB-AnrMonDirection
    ''' </summary>
    Public Shared ReadOnly Property DfltAnrMonDirection_UserProperty_Name() As String = "FBDB-AnrMonDirection"

    ''' <summary>
    ''' FBDB-AnrMonZeit
    ''' </summary>
    Public Shared ReadOnly Property DfltAnrMonDirection_UserProperty_Zeit() As String = "FBDB-AnrMonZeit"

    ''' <summary>
    ''' FBDB_Note_Table
    ''' </summary>
    Public Shared ReadOnly Property DfltNote_Table() As String = "FBDB_Note_Table"

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

    ''' <summary>
    ''' FritzOutlookV5.log
    ''' </summary>
    ''' <value>FritzOutlookV5.log</value>
    ''' <returns>FritzOutlookV5.log</returns>
    ''' <remarks>Wird mit Ressource "PDfltAddin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property DfltLogFileName() As String = $"{My.Resources.strDefShortName}.log"

    ''' <summary>
    ''' Gibt den default Dialport für Mobilgeräte an. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>99</returns>
    Public Shared ReadOnly Property DfltMobilDialPort() As Integer = 99

    Public Shared ReadOnly Property DfltDirectorySeparatorChar() As String = IO.Path.DirectorySeparatorChar

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


    'Friend Shared ReadOnly Property DASLTagTelNr As Object()
    '    Get
    '        Dim tmpDASLTag(18) As Object
    '        tmpDASLTag(0) = "urn:schemas:contacts:secretaryphone" ' .AssistantTelephoneNumber
    '        tmpDASLTag(1) = "urn:schemas:contacts:officetelephonenumber" ' .BusinessTelephoneNumber
    '        tmpDASLTag(2) = "urn:schemas:contacts:office2telephonenumber" ' .Business2TelephoneNumber
    '        tmpDASLTag(3) = "urn:schemas:contacts:callbackphone" ' .CallbackTelephoneNumber
    '        tmpDASLTag(4) = "urn:schemas:contacts:othermobile" ' .CarTelephoneNumber
    '        tmpDASLTag(5) = "urn:schemas:contacts:organizationmainphone" ' .CompanyMainTelephoneNumber
    '        tmpDASLTag(6) = "urn:schemas:contacts:homePhone" ' .HomeTelephoneNumber
    '        tmpDASLTag(7) = "urn:schemas:contacts:homePhone2" ' .Home2TelephoneNumber
    '        tmpDASLTag(8) = "urn:schemas:contacts:internationalisdnnumber" ' .ISDNNumber
    '        tmpDASLTag(9) = "http://schemas.microsoft.com/mapi/proptag/0x3a1c001f" ' .MobileTelephoneNumber
    '        tmpDASLTag(10) = "urn:schemas:contacts:otherTelephone" ' .OtherTelephoneNumber
    '        tmpDASLTag(11) = "urn:schemas:contacts:pager" ' .PagerNumber
    '        tmpDASLTag(12) = "http://schemas.microsoft.com/mapi/proptag/0x3a1a001f" ' .PrimaryTelephoneNumber
    '        tmpDASLTag(13) = "http://schemas.microsoft.com/mapi/proptag/0x3a1d001f" ' .RadioTelephoneNumber
    '        tmpDASLTag(14) = "urn:schemas:contacts:facsimiletelephonenumber" ' .BusinessFaxNumber
    '        tmpDASLTag(15) = "urn:schemas:contacts:homefax" ' .HomeFaxNumber
    '        tmpDASLTag(16) = "urn:schemas:contacts:otherfax" ' .OtherFaxNumber
    '        tmpDASLTag(17) = "urn:schemas:contacts:telexnumber" ' .TelexNumber
    '        tmpDASLTag(18) = "urn:schemas:contacts:ttytddphone" ' .TTYTDDTelephoneNumber

    '        Return tmpDASLTag
    '    End Get
    'End Property

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

    Public Shared ReadOnly Property DfltolTelNrTypen() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String) From {
                "Assistent",
                "Geschäftlich",
                "Geschäftlich 2",
                "Rückmeldung",
                "Auto",
                "Firma",
                "Privat",
                "Privat 2",
                "ISDN",
                "Mobiltelefon",
                "Weitere",
                "Pager",
                "Haupttelefon",
                "Funkruf",
                "Fax geschäftl.",
                "Fax privat",
                "Weiteres Fax",
                "Telex",
                "Texttelefon"
            }

            Return New ReadOnlyCollection(Of String)(tmp)
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

#Region "Literale Rückwärtssuche"
    Public Shared ReadOnly Property RWSTest(TelNr As String, Ergebnis As String) As String
        Get
            Return $"Die Rückwärtssuche mit der Nummer {TelNr} brachte folgendes Ergebnis:{Dflt2NeueZeile}{Ergebnis}"
        End Get
    End Property
    Public Shared ReadOnly Property RWSTestKeinEintrag() As String
        Get
            Return "Kein Eintrag gefunden."
        End Get
    End Property

#End Region

#Region "Literale Warnungen"
    ''' <summary>
    ''' Die Zweifaktor-Authentifizierung der Fritz!Box ist aktiv. Diese Sicherheitsfunktion muss deaktiviert werden, damit das Wählen mit dem ausgewählten Telefon möglich ist.
    ''' In der Fritz!Box:
    ''' System / FRITZ!Box - Benutzer / Anmeldung im Heimnetz
    ''' Entfernen Sie den Haken 'Ausführung bestimmter Einstellungen und Funktionen zusätzlich bestätigen.
    ''' </summary>
    Public Shared ReadOnly Property Warnung2FA As String = $"Die Zweifaktor-Authentifizierung der Fritz!Box ist aktiv. Diese Sicherheitsfunktion muss deaktiviert werden, damit das Wählen mit dem ausgewählten Telefon möglich ist.{Dflt2NeueZeile}In der Fritz!Box:{Dflt1NeueZeile}System / FRITZ!Box - Benutzer / Anmeldung im Heimnetz{Dflt1NeueZeile}Entfernen Sie den Haken 'Ausführung bestimmter Einstellungen und Funktionen zusätzlich bestätigen.'"

#End Region

#Region "Literale Journal"
    ''' <summary>
    ''' Fritz!Box Anrufmonitor
    ''' </summary>
    Public Shared ReadOnly Property DfltJournalKategorie As String = "Fritz!Box Anrufmonitor"

    Public Shared ReadOnly Property DfltJournalDefCategories() As ReadOnlyCollection(Of String)
        Get
            Return New ReadOnlyCollection(Of String)({DfltJournalKategorie, "Telefonanrufe"})
        End Get
    End Property

    ''' <summary>
    ''' Journaleintrag konnte nicht erstellt werden.
    ''' </summary>
    Public Shared ReadOnly Property DfltJournalFehler As String = "Journaleintrag konnte nicht erstellt werden."

    Public Shared ReadOnly Property DfltJournalTextEingehend() As String = "Eingehender Anruf von"
    Public Shared ReadOnly Property DfltJournalTextAusgehend() As String = "Ausgehender Anruf zu"
    Public Shared ReadOnly Property DfltJournalTextVerpasst() As String = "Verpasster Anruf von"
    Public Shared ReadOnly Property DfltJournalTextNichtErfolgreich() As String = "Nicht erfolgreicher Anruf zu"

    Public Shared ReadOnly Property DfltJournalRWSFehler As String = "Rückwärtssuche nicht erfolgreich: Es wurden keine Einträge gefunden."

    '''' <summary>
    '''' Kontaktdaten:
    '''' </summary>
    'Public Shared ReadOnly Property PDfltJournalTextKontaktdaten As String = "Kontaktdaten:"

    ''' <summary>
    ''' Kontaktdaten (vCard):
    ''' </summary>
    Public Shared ReadOnly Property DfltJournalTextKontaktvCard As String = "Kontaktdaten (vCard):"

    ''' <summary>
    ''' Tel.-Nr.:
    ''' </summary>
    Public Shared ReadOnly Property PfltJournalBodyStart As String = "Tel.-Nr.:"

    ''' <summary>
    ''' Tel.-Nr.: TelNr Status: (nicht) angenommen    
    ''' </summary>
    ''' <param name="TelNr">Tekefonnummer</param>
    ''' <param name="Angenommen">Boolean, ob das Telefon angenommen wurde oder nicht</param>
    Public Shared ReadOnly Property DfltJournalBody(TelNr As String, Angenommen As Boolean, vCard As String) As String
        Get
            Return $"{PfltJournalBodyStart} {TelNr}{Dflt1NeueZeile}Status: {If(Angenommen, DfltStringEmpty, "nicht ")}angenommen{Dflt2NeueZeile}{vCard}"
        End Get
    End Property
#End Region

#Region "Literale Telefonbücher"
    Public Shared ReadOnly Property DfltTelBNameNeuBuch As String = "Name für das neue Telefonbuch:"

    Public Shared ReadOnly Property DfltTelBFrageLöschen(TB_Name As String, TB_ID As String) As String
        Get
            Return $"Soll das Telefonbuch {TB_Name} ({TB_ID}) von der Fritz!Box gelöscht werden?"
        End Get
    End Property
    Public Shared ReadOnly Property DfltTelBFrageLöschenID0(TB_Name As String, TB_ID As String) As String
        Get
            Return $"Soll das Telefonbuch {TB_Name} mit der ID {TB_ID} kann nicht gelöscht werden. Stattdessen werden alle Einträge entfernt. Fortfahren?"
        End Get
    End Property
#End Region

#Region "DatagridView"
    Public Shared ReadOnly Property PfltCheckBackColor As Drawing.Color = Drawing.Color.LightGreen
#End Region

#Region "Literale Phoner"
    ''' <summary>
    ''' Das Phoner-Passwort ist falsch!
    ''' </summary>
    Public Shared ReadOnly Property PhonerPasswortFalsch As String = "Das Phoner-Passwort ist falsch."
    ''' <summary>
    ''' Die Phoner-Verson ist zu alt!"
    ''' </summary>
    Public Shared ReadOnly Property PhonerZuAlt As String = "Die Phoner-Verson ist zu alt."
    ''' <summary>   
    ''' Zu dem Datenstrom können keine Daten gesendet werden!
    ''' </summary>
    Public Shared ReadOnly Property PhonerReadonly As String = "Zu dem Datenstrom können keine Daten gesendet werden."
    ''' <summary>   
    ''' Phoner oder PhonerLite ist nicht bereit!
    ''' </summary>
    Public Shared ReadOnly Property PhonerNichtBereit As String = "Phoner ist nicht bereit."
    ''' <summary>
    ''' Telefonnummer <paramref name="Dialcode"/> erfolgreich an Phoner übermittelt
    ''' </summary>
    ''' <param name="Dialcode">Der übermittelte Dialcode</param>
    ''' <returns></returns>
    Public Shared ReadOnly Property SoftPhoneErfolgreich(Dialcode As String, Softphone As String) As String
        Get
            Return $"Telefonnummer {Dialcode} erfolgreich an {Softphone} übermittelt."
        End Get
    End Property
    ''' <summary>
    ''' Abbruch des Rufaufbaues erfolgreich übermittelt.
    ''' </summary>
    Public Shared ReadOnly Property SoftPhoneAbbruch As String = "Abbruch des Rufaufbaues erfolgreich übermittelt."
#End Region

#Region "Literale MicroSIP"
    Public Shared ReadOnly Property MicroSIPBereit As String = "MicroSIP ist bereit."
    Public Shared ReadOnly Property MicroSIPNichtBereit As String = "MicroSIP ist nicht bereit."
    Public Shared ReadOnly Property MicroSIPgestartet(Pfad As String) As String
        Get
            Return $"Pfad zu MicroSIP ermittelt: {Pfad}"
        End Get
    End Property
    Public Shared ReadOnly Property MicroSIPgestartet As String
        Get
            Return "MicroSIP gestartet"
        End Get
    End Property


#End Region
End Class

