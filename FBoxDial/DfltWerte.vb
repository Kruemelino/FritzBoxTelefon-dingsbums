Imports System.Collections.ObjectModel

Public NotInheritable Class DfltWerteAllgemein

#Region "Global Default Value Properties"
    ''' <summary>
    ''' Eon leerer String
    ''' </summary>
    Public Shared ReadOnly Property PDfltStringEmpty As String = String.Empty
    ''' <summary>
    ''' Leerzeichen Chr(32), " "
    ''' </summary>
    Public Shared ReadOnly Property PDfltStringLeerzeichen As String = Chr(32)
    ''' <summary>
    ''' -1 als String
    ''' Default Fehler
    ''' </summary>
    Public Shared ReadOnly Property PDfltStrErrorMinusOne() As String = "-1"

    ''' <summary>
    ''' -1 als Integer
    ''' </summary>
    Public Shared ReadOnly Property PDfltIntErrorMinusOne() As Integer = -1

    ''' <summary>
    ''' -2 als String
    ''' </summary>
    Public Shared ReadOnly Property PDfltStrErrorMinusTwo() As String = "-2"

    ''' <summary>
    ''' vbCrLf
    ''' </summary>
    Public Shared ReadOnly Property PDflt1NeueZeile() As String = vbCrLf

    ''' <summary>
    ''' vbCrLf &amp; vbCrLf
    ''' </summary>
    Public Shared ReadOnly Property PDflt2NeueZeile() As String = PDflt1NeueZeile & PDflt1NeueZeile

    ''' <summary>
    ''' String: unbekannt
    ''' </summary>
    Public Shared ReadOnly Property PDfltStringUnbekannt() As String = "unbekannt"

    ''' <summary>
    ''' Der Zahlenwert NULL <code>"0"</code> als String.
    ''' </summary>
    Public Shared ReadOnly Property PDfltStringNull() As String = "0"

    ''' <summary>
    ''' 0000000000000000
    ''' </summary>
    Public Shared ReadOnly Property PDfltSessionID() As String = "0000000000000000"

    ''' <summary>
    ''' Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)
    ''' </summary>
    Public Shared ReadOnly Property PDfltHeader_UserAgent() As String = "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"

    ''' <summary>
    ''' application/x-www-form-urlencoded
    ''' </summary>
    Public Shared ReadOnly Property PDfltHeader_ContentType() As String = "application/x-www-form-urlencoded"

    ''' <summary>
    ''' text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
    ''' </summary>
    Public Shared ReadOnly Property PDfltHeader_Accept() As String = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"

    ''' <summary>
    ''' 3000
    ''' </summary>
    Public Shared ReadOnly Property PDfltReStartIntervall() As Integer = 2000

    ''' <summary>
    ''' 15
    ''' </summary>
    Public Shared ReadOnly Property PDfltTryMaxRestart() As Integer = 15

    ''' <summary>
    ''' [-&gt;]
    ''' </summary>
    Public Shared ReadOnly Property PDfltAnrMonDirection_Call() As String = "[->]"

    ''' <summary>
    ''' [&lt;-]
    ''' </summary>
    Public Shared ReadOnly Property PDfltAnrMonDirection_Ring() As String = "[<-]"

    ''' <summary>
    ''' [&lt;&gt;]
    ''' </summary>
    Public Shared ReadOnly Property PDfltAnrMonDirection_Default() As String = "[<>]"

    ''' <summary>
    ''' FBDB-AnrMonDirection
    ''' </summary>
    Public Shared ReadOnly Property PDfltAnrMonDirection_UserProperty_Name() As String = "FBDB-AnrMonDirection"

    ''' <summary>
    ''' FBDB-AnrMonZeit
    ''' </summary>
    Public Shared ReadOnly Property PDfltAnrMonDirection_UserProperty_Zeit() As String = "FBDB-AnrMonZeit"

    ''' <summary>
    ''' FBDB_Note_Table
    ''' </summary>
    Public Shared ReadOnly Property PDfltNote_Table() As String = "FBDB_Note_Table"

    ''' <summary>
    ''' BEGIN:VCARD
    ''' </summary>
    Public Shared ReadOnly Property PDfltBegin_vCard() As String = "BEGIN:VCARD"

    ''' <summary>
    ''' END:VCARD
    ''' </summary>
    Public Shared ReadOnly Property PDfltEnd_vCard() As String = "END:VCARD"

    ''' <summary>
    ''' CallList
    ''' </summary>
    ''' <value>CallList</value>
    ''' <returns>CallList</returns>
    Public Shared ReadOnly Property PDfltNameListCALL() As String = "CallList"

    ''' <summary>
    ''' RingList
    ''' </summary>
    ''' <value>RingList</value>
    ''' <returns>RingList</returns>
    Public Shared ReadOnly Property PDfltNameListRING() As String = "RingList"

    ''' <summary>
    ''' VIPList
    ''' </summary>
    ''' <value>VIPList</value>
    ''' <returns>VIPList</returns>
    Public Shared ReadOnly Property PDfltNameListVIP() As String = "VIPList"

    ''' <summary>
    ''' Fritz!Box Telefon-dingsbums
    ''' </summary>
    ''' <value>Fritz!Box Telefon-dingsbums</value>
    ''' <returns>Fritz!Box Telefon-dingsbums</returns>
    Public Shared ReadOnly Property PDfltAddin_LangName() As String = "Fritz!Box Telefon-dingsbums"

    ''' <summary>
    ''' FritzOutlook
    ''' </summary>
    ''' <value>FritzOutlook</value>
    ''' <returns>FritzOutlook</returns>
    Public Shared ReadOnly Property PDfltAddin_KurzName() As String = "FritzOutlook"

    ''' <summary>
    ''' FritzOutlook.xml
    ''' </summary>
    ''' <value>FritzOutlook.xml</value>
    ''' <returns>FritzOutlook.xml</returns>
    ''' <remarks>Wird mit "PDfltAddin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property PDfltConfig_FileName() As String = PDfltAddin_KurzName & ".xml"

    ''' <summary>
    ''' FritzOutlook.log
    ''' </summary>
    ''' <value>FritzOutlook.log</value>
    ''' <returns>FritzOutlook.log</returns>
    ''' <remarks>Wird mit "PDfltAddin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property PDfltLog_FileName() As String = PDfltAddin_KurzName & ".log"

    ''' <summary>
    ''' Gibt den Zeitraum in MINUTEN an, nachdem geprüft werden soll, ob der Anrufmonitor noch aktiv ist. 
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall in MINUTEN</returns>
    Public Shared ReadOnly Property PDfltCheckAnrMonIntervall() As Integer = 1

    ''' <summary>
    ''' Gibt den default Dialport für Mobilgeräte an. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>99</returns>
    Public Shared ReadOnly Property PDfltMobilDialPort() As Integer = 99

    ''' <summary>
    ''' Der Offset der bei der Auswertung der Anrufliste der AnrufID gegeben wird
    ''' </summary>
    ''' <value>100</value>
    Public Shared ReadOnly Property PDfltAnrListIDOffset() As Integer = 100

    Public Shared ReadOnly Property PDfltDirectorySeparatorChar() As String = IO.Path.DirectorySeparatorChar

    Public Shared ReadOnly Property PDfltAddInPath() As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & PDfltDirectorySeparatorChar & PDfltAddin_LangName & PDfltDirectorySeparatorChar

    Friend Shared ReadOnly Property DfltErrorvalue As Integer = -2147221233

    Private Shared ReadOnly Property PDfltSchema As String = "http://schemas.microsoft.com/mapi/string/{FFF40745-D92F-4C11-9E14-92701F001EB3}/"

    Private Shared ReadOnly Property PDfltSchemaUserProperties As String = "http://schemas.microsoft.com/mapi/string/{00020329-0000-0000-C000-000000000046}/"

    Friend Shared ReadOnly Property DASLTagJournal As Object()
        Get
            Dim tmpDASLTag(1) As Object
            tmpDASLTag(0) = PDfltSchema & "FBDB-ContactEntryID"
            tmpDASLTag(1) = PDfltSchema & "FBDB-ContactStoreID"
            Return tmpDASLTag
        End Get
    End Property

    ''' <summary>
    ''' Ein Array, welches den Namen der UserProperties, die die unformatierte Telefonnummer enthält.
    ''' </summary>
    ''' <value>String-Array</value>
    ''' <returns>String-Array</returns>
    Public Shared ReadOnly Property PDfltUserProperties() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String) From {
                PDfltSchemaUserProperties & "FBDB-AssistantTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-BusinessTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-Business2TelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-CallbackTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-CarTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-CompanyMainTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-HomeTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-Home2TelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-ISDNNumber",
                PDfltSchemaUserProperties & "FBDB-MobileTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-OtherTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-PagerNumber",
                PDfltSchemaUserProperties & "FBDB-PrimaryTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-RadioTelephoneNumber",
                PDfltSchemaUserProperties & "FBDB-BusinessFaxNumber",
                PDfltSchemaUserProperties & "FBDB-HomeFaxNumber",
                PDfltSchemaUserProperties & "FBDB-OtherFaxNumber",
                PDfltSchemaUserProperties & "FBDB-Telex",
                PDfltSchemaUserProperties & "FBDB-TTYTDDTelephoneNumber"
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
            tmpDASLTag(0) = PDfltSchema & "FBDB-AssistantTelephoneNumber"
            tmpDASLTag(1) = PDfltSchema & "FBDB-BusinessTelephoneNumber"
            tmpDASLTag(2) = PDfltSchema & "FBDB-Business2TelephoneNumber"
            tmpDASLTag(3) = PDfltSchema & "FBDB-CallbackTelephoneNumber"
            tmpDASLTag(4) = PDfltSchema & "FBDB-CarTelephoneNumber"
            tmpDASLTag(5) = PDfltSchema & "FBDB-CompanyMainTelephoneNumber"
            tmpDASLTag(6) = PDfltSchema & "FBDB-HomeTelephoneNumber"
            tmpDASLTag(7) = PDfltSchema & "FBDB-Home2TelephoneNumber"
            tmpDASLTag(8) = PDfltSchema & "FBDB-ISDNNumber"
            tmpDASLTag(9) = PDfltSchema & "FBDB-MobileTelephoneNumber"
            tmpDASLTag(10) = PDfltSchema & "FBDB-OtherTelephoneNumber"
            tmpDASLTag(11) = PDfltSchema & "FBDB-PagerNumber"
            tmpDASLTag(12) = PDfltSchema & "FBDB-PrimaryTelephoneNumber"
            tmpDASLTag(13) = PDfltSchema & "FBDB-RadioTelephoneNumber"
            tmpDASLTag(14) = PDfltSchema & "FBDB-BusinessFaxNumber"
            tmpDASLTag(15) = PDfltSchema & "FBDB-HomeFaxNumber"
            tmpDASLTag(16) = PDfltSchema & "FBDB-OtherFaxNumber"
            tmpDASLTag(17) = PDfltSchema & "FBDB-Telex"
            tmpDASLTag(18) = PDfltSchema & "FBDB-TTYTDDTelephoneNumber"
            Return tmpDASLTag
        End Get
    End Property

    Public Shared ReadOnly Property PDfltolTelNrTypen() As ReadOnlyCollection(Of String)
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

    ''' <summary>
    ''' Keine Ahnung wozu? FBDB-Save
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property PDfltUserPropertyIndex As String = "FBDB-Save"

#Region "Journal"
    Public Shared ReadOnly Property PDfltJournal_Text_Eingehend As String = "Eingehender Anruf von "
    Public Shared ReadOnly Property PDfltJournal_Text_Ausgehend As String = "Ausgehender Anruf zu "
    Public Shared ReadOnly Property PDfltJournal_Text_Verpasst As String = "Verpasster Anruf von "
    Public Shared ReadOnly Property PDfltJournal_Text_NichtErfolgreich As String = "Nicht erfolgreicher Anruf zu "
#End Region

#End Region

#Region "Literale Anrufmonitor PopUp"
    ''' <summary>
    ''' Kontakt öffnen
    ''' </summary>
    ''' <value>Kontakt öffnen</value>
    ''' <returns>Kontakt öffnen</returns>
    Public Shared ReadOnly Property PAnrMonPopUpToolStripMenuItemKontaktöffnen As String = "Kontakt öffnen"

    ''' <summary>
    ''' Kontakt erstellen
    ''' </summary>
    ''' <value>Kontakt erstellen</value>
    ''' <returns>Kontakt erstellen</returns>
    Public Shared ReadOnly Property PAnrMonPopUpToolStripMenuItemKontaktErstellen As String = "Kontakt erstellen"

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    ''' <value>Rückruf</value>
    ''' <returns>Rückruf</returns>
    Public Shared ReadOnly Property PAnrMonPopUpToolStripMenuItemRückruf As String = "Rückruf"

    ''' <summary>
    ''' In Zwischenablage kopieren
    ''' </summary>
    ''' <value>In Zwischenablage kopieren</value>
    ''' <returns>In Zwischenablage kopieren</returns>
    Public Shared ReadOnly Property PAnrMonPopUpToolStripMenuItemKopieren As String = "In Zwischenablage kopieren"
#End Region

#Region "Literale WählClient"
    ''' <summary>
    ''' Anruf: <paramref name="Kontakt"/>
    ''' </summary>
    ''' <param name="Kontakt">Die Kontaktdaten des anzurzfenden Kontaktes</param>
    Public Shared ReadOnly Property PWählClientFormText(ByVal Kontakt As String) As String
        Get
            Return String.Format("Anruf: {0}", Kontakt)
        End Get
    End Property

    ''' <summary>
    ''' Es ist kein Kontakt mit der E-Mail-Adresse <paramref name="EMailAdresse"/> vorhanden!
    ''' </summary>
    ''' <param name="EMailAdresse">Die Adresse der ausgewählten E-Mail</param>
    Public Shared ReadOnly Property PWählClientEMailunbekannt(ByVal EMailAdresse As String) As String
        Get
            Return String.Format("Es ist kein Kontakt mit der E-Mail-Adresse {0} vorhanden!", EMailAdresse)
        End Get
    End Property

    ''' <summary>
    ''' Es muss entweder ein Kontakt, eine eingegangene E-Mail-Adresse oder ein Journaleintrag ausgewählt sein!
    ''' </summary>
    Public Shared ReadOnly Property PWählClientAuswahlFalsch As String = "Es muss entweder ein Kontakt, eine eingegangene E-Mail-Adresse oder ein Journaleintrag ausgewählt sein!"

    ''' <summary>
    ''' Sie sind dabei eine Mobilnummer anzurufen. Fortsetzen?
    ''' </summary>
    Public Shared ReadOnly Property PWählClientFrageMobil As String = "Sie sind dabei eine Mobilnummer anzurufen. Fortsetzen?"

    ''' <summary>
    ''' Fehler
    ''' </summary>
    Public Shared ReadOnly Property PWählClientDialFehler As String = "Fehler"

    ''' <summary>
    ''' Bitte warten
    ''' </summary>
    Public Shared ReadOnly Property PWählClientBitteWarten As String = "Bitte warten"

    ''' <summary>
    ''' Jetzt abheben
    ''' </summary>
    Public Shared ReadOnly Property PWählClientJetztAbheben As String = "Jetzt abheben"

    ''' <summary>
    ''' Abgebrochen
    ''' </summary>
    Public Shared ReadOnly Property PWählClientDialHangUp As String = String.Format("Abgebrochen", PDflt1NeueZeile)

    ''' <summary>
    ''' Dialcode: <paramref name="DialCode"/>>
    ''' </summary>
    ''' <param name="DialCode"></param>
    Public Shared ReadOnly Property PWählClientStatusWählClient(ByVal DialCode As String) As String
        Get
            Return String.Format("Dialcode: {0}", DialCode)
        End Get
    End Property

    ''' <summary>
    ''' Wählclient SOAPDial: <paramref name="DialCode"/> über <paramref name="TelGerät"/>
    ''' </summary>
    ''' <param name="DialCode"></param>
    ''' <param name="TelGerät"></param>
    ''' <returns></returns>
    Public Shared ReadOnly Property PWählClientLogDial(ByVal DialCode As String, TelGerät As String) As String
        Get
            Return String.Format("Wählclient SOAPDial: {0} über {1}", DialCode, TelGerät)
        End Get
    End Property

    ''' <summary>
    ''' Lade Telefoniegeräte...
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusLadeGeräte As String = "Lade Telefoniegeräte..."

    ''' <summary>
    ''' Lade Telefonnummern des Kontaktes...
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusLadeKontaktTelNr As String = "Lade Telefonnummern des Kontaktes..."

    ''' <summary>
    ''' Lade Telefonnummer...
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusLadeTelNr As String = "Lade Telefonnummer..."

    ''' <summary>
    ''' Ausgewählt: <paramref name="StrTelNr"/>
    ''' </summary>
    ''' <param name="StrTelNr"></param>
    Public Shared ReadOnly Property PWählClientStatusTelNrAuswahl(ByVal StrTelNr As String) As String
        Get
            Return String.Format("Ausgewählt: {0}", StrTelNr)
        End Get
    End Property

    ''' <summary>
    ''' "Anruf wird vorbereitet...
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusVorbereitung As String = String.Format("Anruf wird vorbereitet...", PDflt1NeueZeile)

    ''' <summary>
    ''' 
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusAbbruch As String = String.Format("Abbruch wird vorbereitet...", PDflt1NeueZeile)

    ''' <summary>
    ''' <paramref name="Sender"/>: <paramref name="Meldung"/> <paramref name="Wert"/>
    ''' </summary>
    ''' <param name="Sender">Die Funktion die den den Status setzt</param>
    ''' <param name="Meldung">Der Meldungstext</param>
    ''' <param name="Wert">Ein gesetzer Wert</param>
    Public Shared ReadOnly Property PWählClientDialStatus(ByVal Sender As String, ByVal Meldung As String, ByVal Wert As String) As String
        Get
            Return String.Format("{0}: {1} {2}", Sender, Meldung, Wert)
        End Get
    End Property

    ''' <summary>
    ''' "Ändere Dialport auf
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusDialPort() As String = "Ändere Dialport auf"

    ''' <summary>
    ''' Der SOAP-Dialport konnte nicht geändert werden:
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusSOAPDialPortFehler As String = "Der SOAP-Dialport konnte nicht geändert werden:"
#End Region

#Region "Warnungen"
    ''' <summary>
    ''' Die Zweifaktor-Authentifizierung der Fritz!Box ist aktiv. Diese Sicherheitsfunktion muss deaktiviert werden, damit das Wählen mit dem ausgewählten Telefon möglich ist.
    ''' In der Fritz!Box:
    ''' System / FRITZ!Box - Benutzer / Anmeldung im Heimnetz
    ''' Entfernen Sie den Haken 'Ausführung bestimmter Einstellungen und Funktionen zusätzlich bestätigen.
    ''' </summary>
    Public Shared ReadOnly Property PWarnung2FA As String = String.Format("Die Zweifaktor-Authentifizierung der Fritz!Box ist aktiv. Diese Sicherheitsfunktion muss deaktiviert werden, damit das Wählen mit dem ausgewählten Telefon möglich ist.{0}In der Fritz!Box:{1}System / FRITZ!Box - Benutzer / Anmeldung im Heimnetz{1}Entfernen Sie den Haken 'Ausführung bestimmter Einstellungen und Funktionen zusätzlich bestätigen.'", PDflt2NeueZeile, PDflt1NeueZeile)

#End Region

#Region "Journal"
    ''' <summary>
    ''' FritzBox Anrufmonitor
    ''' </summary>
    Public Shared ReadOnly Property PDfltJournalKategorie As String = "Fritz!Box Anrufmonitor"

    Public Shared ReadOnly Property PDfltJournalDefCategories() As ReadOnlyCollection(Of String)
        Get
            Return New ReadOnlyCollection(Of String)({PDfltJournalKategorie, "Telefonanrufe"})
        End Get
    End Property

    ''' <summary>
    ''' Journaleintrag konnte nicht erstellt werden.
    ''' </summary>
    Public Shared ReadOnly Property PDfltJournalFehler As String = "Journaleintrag konnte nicht erstellt werden."

    Public Shared ReadOnly Property PDfltJournalTextEingehend() As String = "Eingehender Anruf von"
    Public Shared ReadOnly Property PDfltJournalTextAusgehend() As String = "Ausgehender Anruf zu"
    Public Shared ReadOnly Property PDfltJournalTextVerpasst() As String = "Verpasster Anruf von"
    Public Shared ReadOnly Property PDfltJournalTextNichtErfolgreich() As String = "Nicht erfolgreicher Anruf zu"

    Public Shared ReadOnly Property PDfltJournalRWSFehler As String = "Rückwärtssuche nicht erfolgreich: Es wurden keine Einträge gefunden."

    '''' <summary>
    '''' Kontaktdaten:
    '''' </summary>
    'Public Shared ReadOnly Property PDfltJournalTextKontaktdaten As String = "Kontaktdaten:"

    ''' <summary>
    ''' Kontaktdaten (vCard):
    ''' </summary>
    Public Shared ReadOnly Property PDfltJournalTextKontaktvCard As String = "Kontaktdaten (vCard):"

    ''' <summary>
    ''' Tel.-Nr.:
    ''' </summary>
    Public Shared ReadOnly Property PDfltJournalBodyStart As String = "Tel.-Nr.:"

    ''' <summary>
    ''' Tel.-Nr.: TelNr Status: (nicht) angenommen    
    ''' </summary>
    ''' <param name="TelNr">Tekefonnummer</param>
    ''' <param name="Angenommen">Boolean, ob das Telefon angenommen wurde oder nicht</param>
    Public Shared ReadOnly Property PDfltJournalBody(ByVal TelNr As String, ByVal Angenommen As Boolean, ByVal vCard As String) As String
        Get
            Return String.Format("{5} {2}{0}Status: {3}angenommen{1}{4}", PDflt1NeueZeile, PDflt2NeueZeile, TelNr, If(Angenommen, PDfltStringEmpty, "nicht "), vCard, PDfltJournalBodyStart)
        End Get
    End Property
#End Region

End Class

