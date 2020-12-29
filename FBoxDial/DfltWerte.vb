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

    '''' <summary>
    '''' 0000000000000000
    '''' </summary>
    'Public Shared ReadOnly Property PDfltSessionID() As String = "0000000000000000"

    ''' <summary>
    ''' Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)
    ''' </summary>
    Public Shared ReadOnly Property PDfltHeader_UserAgent() As String = "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"

    '''' <summary>
    '''' application/x-www-form-urlencoded
    '''' </summary>
    'Public Shared ReadOnly Property PDfltHeader_ContentType() As String = "application/x-www-form-urlencoded"

    ''' <summary>
    ''' text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
    ''' </summary>
    Public Shared ReadOnly Property PDfltHeader_Accept() As String = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"

    ''' <summary>
    ''' 2000
    ''' </summary>
    Public Shared ReadOnly Property PDfltReStartIntervall() As Integer = 2000

    ''' <summary>
    ''' 15
    ''' </summary>
    Public Shared ReadOnly Property PDfltTryMaxRestart() As Integer = 15

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
    ''' FritzOutlookV5.xml
    ''' </summary>
    ''' <returns>FritzOutlookV5.xml</returns>
    ''' <remarks>Wird mit der Ressource "strDefShortName" erstellt.</remarks>
    Public Shared ReadOnly Property PDfltConfig_FileName() As String = $"{Localize.resCommon.strDefShortName}.xml"

    ''' <summary>
    ''' FritzOutlookV5.log
    ''' </summary>
    ''' <value>FritzOutlookV5.log</value>
    ''' <returns>FritzOutlookV5.log</returns>
    ''' <remarks>Wird mit Ressource "PDfltAddin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property PDfltLog_FileName() As String = $"{Localize.resCommon.strDefShortName}.log"

    ''' <summary>
    ''' ${date:format=dd.MM.yyyy HH\:mm\:ss.fff}|${level}|${logger}|${callsite:includeNamespace=false:className=false:methodName=true:cleanNamesOfAnonymousDelegates=true:cleanNamesOfAsyncContinuations=true}|${callsite-linenumber}|${message}
    ''' </summary>
    ''' <returns>${date:format=dd.MM.yyyy HH\:mm\:ss.fff}|${level}|${logger}|${callsite:includeNamespace=false:className=false:methodName=true:cleanNamesOfAnonymousDelegates=true:cleanNamesOfAsyncContinuations=true}|${callsite-linenumber}|${message}</returns>
    Public Shared ReadOnly Property PDfltNLog_LayoutText() As String = "${date:format=dd.MM.yyyy HH\:mm\:ss.fff}|${level}|${logger}|${callsite:includeNamespace=false:className=false:methodName=true:cleanNamesOfAnonymousDelegates=true:cleanNamesOfAsyncContinuations=true}|${callsite-linenumber}|${message}"

    ''' <summary>
    ''' Gibt den default Dialport für Mobilgeräte an. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>99</returns>
    Public Shared ReadOnly Property PDfltMobilDialPort() As Integer = 99

    Public Shared ReadOnly Property PDfltDirectorySeparatorChar() As String = IO.Path.DirectorySeparatorChar

    Public Shared ReadOnly Property PDfltAddInPath() As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & PDfltDirectorySeparatorChar & Localize.resCommon.strDefLongName & PDfltDirectorySeparatorChar

    Friend Shared ReadOnly Property DfltErrorvalue As Integer = -2147221233

    Private Shared ReadOnly Property PDfltDASLSchema As String = "http://schemas.microsoft.com/mapi/string/{FFF40745-D92F-4C11-9E14-92701F001EB3}/"

    Private Shared ReadOnly Property PDfltDASLSchemaUserProperties As String = "http://schemas.microsoft.com/mapi/string/{00020329-0000-0000-C000-000000000046}/"

    Friend Shared ReadOnly Property PDfltDASLSMTPAdress As String = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E"

    Friend Shared ReadOnly Property DASLTagJournal As Object()
        Get
            Dim tmpDASLTag(1) As Object
            tmpDASLTag(0) = PDfltDASLSchema & "FBDB-ContactEntryID"
            tmpDASLTag(1) = PDfltDASLSchema & "FBDB-ContactStoreID"
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
                PDfltDASLSchemaUserProperties & "FBDB-AssistantTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-BusinessTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-Business2TelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-CallbackTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-CarTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-CompanyMainTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-HomeTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-Home2TelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-ISDNNumber",
                PDfltDASLSchemaUserProperties & "FBDB-MobileTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-OtherTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-PagerNumber",
                PDfltDASLSchemaUserProperties & "FBDB-PrimaryTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-RadioTelephoneNumber",
                PDfltDASLSchemaUserProperties & "FBDB-BusinessFaxNumber",
                PDfltDASLSchemaUserProperties & "FBDB-HomeFaxNumber",
                PDfltDASLSchemaUserProperties & "FBDB-OtherFaxNumber",
                PDfltDASLSchemaUserProperties & "FBDB-Telex",
                PDfltDASLSchemaUserProperties & "FBDB-TTYTDDTelephoneNumber"
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
            tmpDASLTag(0) = PDfltDASLSchema & "FBDB-AssistantTelephoneNumber"
            tmpDASLTag(1) = PDfltDASLSchema & "FBDB-BusinessTelephoneNumber"
            tmpDASLTag(2) = PDfltDASLSchema & "FBDB-Business2TelephoneNumber"
            tmpDASLTag(3) = PDfltDASLSchema & "FBDB-CallbackTelephoneNumber"
            tmpDASLTag(4) = PDfltDASLSchema & "FBDB-CarTelephoneNumber"
            tmpDASLTag(5) = PDfltDASLSchema & "FBDB-CompanyMainTelephoneNumber"
            tmpDASLTag(6) = PDfltDASLSchema & "FBDB-HomeTelephoneNumber"
            tmpDASLTag(7) = PDfltDASLSchema & "FBDB-Home2TelephoneNumber"
            tmpDASLTag(8) = PDfltDASLSchema & "FBDB-ISDNNumber"
            tmpDASLTag(9) = PDfltDASLSchema & "FBDB-MobileTelephoneNumber"
            tmpDASLTag(10) = PDfltDASLSchema & "FBDB-OtherTelephoneNumber"
            tmpDASLTag(11) = PDfltDASLSchema & "FBDB-PagerNumber"
            tmpDASLTag(12) = PDfltDASLSchema & "FBDB-PrimaryTelephoneNumber"
            tmpDASLTag(13) = PDfltDASLSchema & "FBDB-RadioTelephoneNumber"
            tmpDASLTag(14) = PDfltDASLSchema & "FBDB-BusinessFaxNumber"
            tmpDASLTag(15) = PDfltDASLSchema & "FBDB-HomeFaxNumber"
            tmpDASLTag(16) = PDfltDASLSchema & "FBDB-OtherFaxNumber"
            tmpDASLTag(17) = PDfltDASLSchema & "FBDB-Telex"
            tmpDASLTag(18) = PDfltDASLSchema & "FBDB-TTYTDDTelephoneNumber"
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

    '''' <summary>
    '''' Keine Ahnung wozu? FBDB-Save
    '''' </summary>
    '''' <returns></returns>
    'Public Shared ReadOnly Property PDfltUserPropertyIndex As String = "FBDB-Save"

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
    ''' <param name="Kontakt">Die Kontaktdaten des anzurufenden Kontaktes</param>
    Public Shared ReadOnly Property PWählClientFormText(ByVal Kontakt As String) As String
        Get
            Return $"Anruf: {Kontakt}"
        End Get
    End Property

    ''' <summary>
    ''' Es ist kein Kontakt mit der E-Mail-Adresse <paramref name="EMailAdresse"/> vorhanden!
    ''' </summary>
    ''' <param name="EMailAdresse">Die Adresse der ausgewählten E-Mail</param>
    Public Shared ReadOnly Property PWählClientEMailunbekannt(ByVal EMailAdresse As String) As String
        Get
            Return $"Es ist kein Kontakt mit der E-Mail-Adresse {EMailAdresse} vorhanden!"
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
    Public Shared ReadOnly Property PWählClientDialHangUp As String = $"Abgebrochen"

    ''' <summary>
    ''' Dialcode: <paramref name="DialCode"/>>
    ''' </summary>
    ''' <param name="DialCode"></param>
    Public Shared ReadOnly Property PWählClientStatusWählClient(ByVal DialCode As String) As String
        Get
            Return $"Dialcode: {DialCode}"
        End Get
    End Property

    ''' <summary>
    ''' Lade Telefoniegeräte...
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusLadeGeräte As String = "Lade Telefoniegeräte..."
    Public Shared ReadOnly Property PWählClientStatusLetztesGerät As String = "Setze letztes Gerät..."
    Public Shared ReadOnly Property PWählClientStatus1Gerät As String = "Setze 1. Gerät in Liste..."
    Public Shared ReadOnly Property PWählClientStatusFehlerGerät As String = "Es konnte kein Gerät geladen werden..."
    Public Shared ReadOnly Property PWählClientPhonerInaktiv As String = "Phoner ist nicht bereit..."
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
            Return $"Ausgewählt: {StrTelNr}"
        End Get
    End Property

    ''' <summary>
    ''' "Anruf wird vorbereitet...
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusVorbereitung As String = $"Anruf wird vorbereitet...{PDflt1NeueZeile}"

    ''' <summary>
    ''' 
    ''' </summary>
    Public Shared ReadOnly Property PWählClientStatusAbbruch As String = $"Anruf wird abgebrochen...{PDflt1NeueZeile}"

    ''' <summary>
    ''' <paramref name="Sender"/>: <paramref name="Meldung"/> <paramref name="Wert"/>
    ''' </summary>
    ''' <param name="Sender">Die Funktion die den den Status setzt</param>
    ''' <param name="Meldung">Der Meldungstext</param>
    ''' <param name="Wert">Ein gesetzer Wert</param>
    Public Shared ReadOnly Property PWählClientDialStatus(ByVal Sender As String, ByVal Meldung As String, ByVal Wert As String) As String
        Get
            Return $"{Sender}: {Meldung} {Wert}"
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

#Region "Literale Rückwärtssuche"
    Public Shared ReadOnly Property PRWSTest(ByVal TelNr As String, ByVal Ergebnis As String) As String
        Get
            Return $"Die Rückwärtssuche mit der Nummer {TelNr} brachte folgendes Ergebnis:{PDflt2NeueZeile}{Ergebnis}"
        End Get
    End Property
    Public Shared ReadOnly Property PRWSTestKeinEintrag() As String
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
    Public Shared ReadOnly Property PWarnung2FA As String = $"Die Zweifaktor-Authentifizierung der Fritz!Box ist aktiv. Diese Sicherheitsfunktion muss deaktiviert werden, damit das Wählen mit dem ausgewählten Telefon möglich ist.{PDflt2NeueZeile}In der Fritz!Box:{PDflt1NeueZeile}System / FRITZ!Box - Benutzer / Anmeldung im Heimnetz{PDflt1NeueZeile}Entfernen Sie den Haken 'Ausführung bestimmter Einstellungen und Funktionen zusätzlich bestätigen.'"

#End Region

#Region "Literale Journal"
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
            Return $"{PDfltJournalBodyStart} {TelNr}{PDflt1NeueZeile}Status: {If(Angenommen, PDfltStringEmpty, "nicht ")}angenommen{PDflt2NeueZeile}{vCard}"
        End Get
    End Property
#End Region

#Region "Literale Telefonbücher"
    Public Shared ReadOnly Property PDfltTelBNameNeuBuch As String = "Name für das neue Telefonbuch:"

    Public Shared ReadOnly Property PDfltTelBFrageLöschen(ByVal TB_Name As String, ByVal TB_ID As String) As String
        Get
            Return $"Soll das Telefonbuch {TB_Name} ({TB_ID}) von der Fritz!Box gelöscht werden?"
        End Get
    End Property
    Public Shared ReadOnly Property PDfltTelBFrageLöschenID0(ByVal TB_Name As String, ByVal TB_ID As String) As String
        Get
            Return $"Soll das Telefonbuch {TB_Name} mit der ID {TB_ID} kann nicht gelöscht werden. Stattdessen werden alle Einträge entfernt. Fortfahren?"
        End Get
    End Property
#End Region

#Region "DatagridView"
    Public Shared ReadOnly Property PDfltCheckBackColor As Drawing.Color = Drawing.Color.LightGreen
#End Region

#Region "Literale Phoner"
    ''' <summary>
    ''' Das Phoner-Passwort ist falsch!
    ''' </summary>
    Public Shared ReadOnly Property PPhonerPasswowrtFalsch As String = "Das Phoner-Passwort ist falsch!"
    ''' <summary>
    ''' Die Phoner-Verson ist zu alt!"
    ''' </summary>
    Public Shared ReadOnly Property PPhonerZuAlt As String = "Die Phoner-Verson ist zu alt!"
    ''' <summary>   
    ''' Zu dem Datenstrom können keine Daten gesendet werden!
    ''' </summary>
    Public Shared ReadOnly Property PPhonerReadonly As String = "Zu dem Datenstrom können keine Daten gesendet werden!"
    ''' <summary>   
    ''' Phoner oder PhonerLite ist nicht bereit!
    ''' </summary>
    Public Shared ReadOnly Property PPhonerNichtBereit As String = "Phoner oder PhonerLite ist nicht bereit!"
    ''' <summary>
    ''' Telefonnummer <paramref name="Dialcode"/> erfolgreich an Phoner übermittelt
    ''' </summary>
    ''' <param name="Dialcode">Der übermittelte Dialcode</param>
    ''' <returns></returns>
    Public Shared ReadOnly Property PPhonerErfolgreich(ByVal Dialcode As String) As String
        Get
            Return $"Telefonnummer {Dialcode} erfolgreich an Phoner übermittelt."
        End Get
    End Property
    ''' <summary>
    ''' Abbruch des Rufaufbaues erfolgreich übermittelt.
    ''' </summary>
    Public Shared ReadOnly Property PPhonerAbbruch As String = "Abbruch des Rufaufbaues erfolgreich übermittelt."
#End Region

End Class

