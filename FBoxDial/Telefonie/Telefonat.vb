Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie
Imports Microsoft.Office.Interop

<Serializable()> Public Class Telefonat
    Inherits NotifyBase

    Implements IEquatable(Of Telefonat)
    Implements IDisposable

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
#Region "Integer"
    Private _ID As Integer
    <XmlAttribute> Public Property ID As Integer
        Get
            Return _ID
        End Get
        Set
            SetProperty(_ID, Value)
        End Set
    End Property

    Private _NebenstellenNummer As Integer

    <XmlElement> Public Property NebenstellenNummer As Integer
        Get
            Return _NebenstellenNummer
        End Get
        Set
            SetProperty(_NebenstellenNummer, Value)
        End Set
    End Property

    Private _Dauer As Integer
    <XmlElement> Public Property Dauer As Integer
        Get
            Return _Dauer
        End Get
        Set
            SetProperty(_Dauer, Value)
        End Set
    End Property

    Private _AnrufRichtung As Integer
    <XmlElement> Public Property AnrufRichtung As Integer
        Get
            Return _AnrufRichtung
        End Get
        Set
            SetProperty(_AnrufRichtung, Value)
        End Set
    End Property

    Private _AnzahlAnrufe As Integer = 1
    <XmlIgnore> Public Property AnzahlAnrufe As Integer
        Get
            Return _AnzahlAnrufe
        End Get
        Set
            SetProperty(_AnzahlAnrufe, Value)
        End Set
    End Property
#End Region

#Region "String"
    Private _OutEigeneTelNr As String
    <XmlElement> Public Property OutEigeneTelNr As String
        Get
            Return _OutEigeneTelNr
        End Get
        Set
            SetProperty(_OutEigeneTelNr, Value)
        End Set
    End Property

    Private _AnschlussID As String
    <XmlElement> Public Property AnschlussID As String
        Get
            Return _AnschlussID
        End Get
        Set
            SetProperty(_AnschlussID, Value)
        End Set
    End Property

    Private _OutlookKontaktID As String
    <XmlElement> Public Property OutlookKontaktID As String
        Get
            Return _OutlookKontaktID
        End Get
        Set
            SetProperty(_OutlookKontaktID, Value)
        End Set
    End Property

    Private _OutlookStoreID As String
    <XmlElement> Public Property OutlookStoreID As String
        Get
            Return _OutlookStoreID
        End Get
        Set
            SetProperty(_OutlookStoreID, Value)
        End Set
    End Property

    Private _VCard As String
    <XmlElement> Public Property VCard As String
        Get
            Return _VCard
        End Get
        Set
            SetProperty(_VCard, Value)
        End Set
    End Property

    Private _AnruferName As String
    <XmlElement> Public Property AnruferName As String
        Get
            Return _AnruferName
        End Get
        Set
            SetProperty(_AnruferName, Value)

            OnPropertyChanged(NameOf(NameGegenstelle))
        End Set
    End Property

    Private _Firma As String
    <XmlElement> Public Property Firma As String
        Get
            Return _Firma
        End Get
        Set
            SetProperty(_Firma, Value)
        End Set
    End Property

    Private _TAMMessagePath As String
    <XmlElement> Public Property TAMMessagePath As String
        Get
            Return _TAMMessagePath
        End Get
        Set
            SetProperty(_TAMMessagePath, Value)
        End Set
    End Property

    <XmlIgnore> Friend ReadOnly Property GegenstellenNummerLocation As String
        Get
            If Not GegenstelleTelNr.Unterdrückt Then
                Return $"{GegenstelleTelNr.Location}" & If(GegenstelleTelNr.IstInland, String.Empty, $" ({Localize.Länder.ResourceManager.GetString(GegenstelleTelNr.AreaCode)})")
            Else
                ' Gib ein leeren String zurück
                Return String.Empty
            End If
        End Get
    End Property

#End Region

#Region "Boolean"
    Private _Beendet As Boolean = False
    <XmlIgnore> Public Property Beendet As Boolean
        Get
            Return _Beendet
        End Get
        Set
            SetProperty(_Beendet, Value)
        End Set
    End Property

    Private _NrUnterdrückt As Boolean
    <XmlAttribute> Public Property NrUnterdrückt As Boolean
        Get
            Return _NrUnterdrückt
        End Get
        Set
            SetProperty(_NrUnterdrückt, Value)
        End Set
    End Property

    Private _Angenommen As Boolean = False
    <XmlAttribute> Public Property Angenommen As Boolean
        Get
            Return _Angenommen
        End Get
        Set
            SetProperty(_Angenommen, Value)
        End Set
    End Property

    Private _Blockiert As Boolean = False
    ''' <summary>
    ''' Angabe, ob dieses Telefonat von der Fritz!Box blockiert wurde.
    ''' </summary>
    <XmlAttribute> Public Property Blockiert As Boolean
        Get
            Return _Blockiert
        End Get
        Set
            SetProperty(_Blockiert, Value)
        End Set
    End Property
    <XmlIgnore> Friend ReadOnly Property AnruferUnbekannt As Boolean
        Get
            Return OlKontakt Is Nothing And FBTelBookKontakt Is Nothing
        End Get
    End Property

    Private _Import As Boolean = False
    ''' <summary>
    ''' Gibt an, ob die Daten zu dem Telefonat nachträglich aus der Anrufliste importiert werden.
    ''' </summary>
    <XmlIgnore> Friend Property Import As Boolean
        Set
            _Import = Value
        End Set
        Get
            Return _Import OrElse ID.IsLarger(10)
        End Get
    End Property

    ''' <summary>
    ''' Interne Rufweiterleitung.
    ''' </summary>
    <XmlIgnore> Friend Property Intern As Boolean = False

    ''' <summary>
    ''' Angabe, ob Informationen zur Gegenstelle ermittelt wurden
    ''' </summary>
    <XmlIgnore> Friend Property AnruferErmittelt As Boolean = False

    ''' <summary>
    ''' Angabe, ob der Anrufmonitor zu diesem Telefonat aktuell eingeblendet ist
    ''' </summary>
    <XmlIgnore> Friend Property AnrMonEingeblendet As Boolean = False

    ''' <summary>
    ''' Angabe, ob die Stoppuhr zu diesem Telefonat aktuell eingeblendet ist
    ''' </summary>
    <XmlIgnore> Friend Property StoppUhrEingeblendet As Boolean = False

    ''' <summary>
    ''' Flag, ob das Ausblenden durch einen Timer gestartet werden soll.
    ''' </summary>
    ''' <returns></returns>
    <XmlIgnore> Friend Property AnrMonStartHideTimer As Boolean = False

    ''' <summary>
    ''' Angabe ob das Telefonat für die Auswertung relevant ist. Maßgebend ist die Entscheidung, des Nutzers,
    ''' ob die eigene Telefonnummer überwacht werden soll und, ob die Gegenstelle nicht auf der Fritz!Box blockiert wird.
    ''' </summary>
    <XmlIgnore> Friend ReadOnly Property IstRelevant As Boolean
        Get
            Return EigeneTelNr.EigeneNummerInfo IsNot Nothing AndAlso EigeneTelNr.EigeneNummerInfo.Überwacht AndAlso (Not Blockiert OrElse (XMLData.POptionen.CBJournalBlockNr Or XMLData.POptionen.CBAnrMonBlockNr))

            '' Für Debugzewecke
            'If EigeneTelNr.EigeneNummerInfo IsNot Nothing Then
            '    If EigeneTelNr.EigeneNummerInfo.Überwacht Then
            '        NLogger.Trace($"Die eigene Telefonnummer '{EigeneTelNr.Unformatiert}' wird überwacht.")

            '        If Not Blockiert Then
            '            NLogger.Trace($"Die Gegenstelle '{GegenstelleTelNr.Unformatiert}' wird durch die Fritz!Box nicht blockiert.")
            '            Return True
            '        Else
            '            ' Das Telefonat wird blockiert
            '            NLogger.Trace($"Die Gegenstelle '{GegenstelleTelNr.Unformatiert}' wird durch die Fritz!Box  blockiert.")
            '            ' Sofern die Einstellungen gesetzt sind, dass blockierte Nummern dennoch erfasst werden sollen...
            '            NLogger.Trace($"Einstellungen: CBJournalBlockNr={XMLData.POptionen.CBJournalBlockNr} CBAnrMonBlockNr={XMLData.POptionen.CBAnrMonBlockNr}")
            '            Return XMLData.POptionen.CBJournalBlockNr Or XMLData.POptionen.CBAnrMonBlockNr
            '        End If
            '    Else
            '        NLogger.Trace($"Die eigene Telefonnummer '{EigeneTelNr.Unformatiert}' wird nicht überwacht.")
            '    End If
            'Else
            '    NLogger.Trace($"Datenstz für eigene Nummer '{EigeneTelNr.Unformatiert}' nicht vorhanden (Nothing).")
            'End If

            'Return False
        End Get
    End Property

    ''' <summary>
    ''' Rufumleitungen (Parallelruf) werden mittels NebenstellenID 3 (Durchwahl/CallThrough) verarbeitet.
    ''' </summary>
    <XmlIgnore> Friend ReadOnly Property Rufweiterleitung As Boolean
        Get
            Return NebenstellenNummer = AnrMonTelIDBase.CallThrough
        End Get
    End Property
#End Region

#Region "Date"
    Private _ZeitBeginn As Date
    <XmlElement> Public Property ZeitBeginn As Date
        Get
            Return _ZeitBeginn
        End Get
        Set
            SetProperty(_ZeitBeginn, Value)
        End Set
    End Property

    Private _ZeitVerbunden As Date
    <XmlElement> Public Property ZeitVerbunden As Date
        Get
            Return _ZeitVerbunden
        End Get
        Set
            SetProperty(_ZeitVerbunden, Value)
        End Set
    End Property

    Private _ZeitEnde As Date
    <XmlElement> Public Property ZeitEnde As Date
        Get
            Return _ZeitEnde
        End Get
        Set
            SetProperty(_ZeitEnde, Value)
        End Set
    End Property
#End Region

#Region "Objekte"
    Private _EigeneTelNr As Telefonnummer
    <XmlIgnore> Public Property EigeneTelNr As Telefonnummer
        Get
            Return _EigeneTelNr
        End Get
        Set
            SetProperty(_EigeneTelNr, Value)
        End Set
    End Property

    Private _GegenstelleTelNr As Telefonnummer
    <XmlElement> Public Property GegenstelleTelNr As Telefonnummer
        Get
            Return _GegenstelleTelNr
        End Get
        Set
            SetProperty(_GegenstelleTelNr, Value)
        End Set
    End Property

    Private _TelGerät As Telefoniegerät
    <XmlIgnore> Public Property TelGerät As Telefoniegerät
        Get
            Return _TelGerät
        End Get
        Set
            SetProperty(_TelGerät, Value)
        End Set
    End Property

    Private _FBTelBookKontakt As FBoxAPI.Contact = Nothing
    <XmlElement> Public Property FBTelBookKontakt As FBoxAPI.Contact
        Get
            Return _FBTelBookKontakt
        End Get
        Set
            SetProperty(_FBTelBookKontakt, Value)
        End Set
    End Property

    Private _OlKontakt As Outlook.ContactItem = Nothing
    <XmlIgnore> Friend Property OlKontakt() As Outlook.ContactItem
        Get
            ' Ermittle den Outlook-Kontakt, falls dies noch nicht geschehen ist
            Try
                ' Versuche auf den Kontakt zuzugreifen. Ansonsten gibt es einen Fehler.
                If _OlKontakt IsNot Nothing Then Dim tmp As String = _OlKontakt.EntryID

            Catch ex As Exception
                _OlKontakt = Nothing
                NLogger.Warn(ex)
            Finally

                If _OlKontakt Is Nothing AndAlso (OutlookKontaktID.IsNotStringNothingOrEmpty And OutlookStoreID.IsNotStringNothingOrEmpty) Then
                    _OlKontakt = GetOutlookKontakt(OutlookKontaktID, OutlookStoreID)
                End If

            End Try
            Return _OlKontakt
        End Get
        Set
            SetProperty(_OlKontakt, Value)
        End Set
    End Property

    Private WithEvents OlKontakt_wEvents As Outlook.ContactItem

    <XmlElement> Public ReadOnly Property NameGegenstelle As String
        Get
            Return If(AnruferName.IsNotStringNothingOrEmpty, AnruferName, If(NrUnterdrückt, Localize.LocAnrMon.strNrUnterdrückt, GegenstelleTelNr.Formatiert))
        End Get
    End Property

    <XmlIgnore> Private Property PopUpAnrMonWPF As AnrMonWPF

    <XmlIgnore> Private Property PopupStoppUhrWPF As StoppUhrWPF

    Private _TellowsResult As ITellowsResult
    <XmlIgnore> Public Property TellowsResult As ITellowsResult
        Get
            Return _TellowsResult
        End Get
        Set
            SetProperty(_TellowsResult, Value)
        End Set
    End Property

#End Region

    ''' <summary>
    '''         0        ; 1  ;2;    3     ;  4   ; 5  ; 6
    ''' 23.06.18 13:20:24;RING;1;0123456789;987654;SIP4;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonRING As String()
        Set(FBStatus As String())
            AnrufRichtung = AnrufRichtungen.Eingehend

            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitBeginn = CDate(FBStatus(i))

                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = FBStatus(i).ToInt

                    Case 3 ' Eingehende (anrufende) Telefonnummer
                        GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}

                        NrUnterdrückt = GegenstelleTelNr.Unterdrückt

                    Case 4 ' Eigene (angerufene) Telefonnummer, MSN

                        EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr(FBStatus(i))
                        ' Wert für Serialisierung in separater Eigenschaft ablegen
                        If EigeneTelNr Is Nothing Then
                            NLogger.Warn($"Eigene Telefonnummer für {FBStatus(i)} konnte nicht ermittelt werden.")
                            EigeneTelNr = New Telefonnummer With {.SetNummer = FBStatus(i),
                                                                  .EigeneNummerInfo = New EigeneNrInfo With {.Überwacht = True}}
                        End If

                        OutEigeneTelNr = EigeneTelNr.Unformatiert

                    Case 5 ' Anschluss, SIP...
                        AnschlussID = FBStatus(i)

                End Select
            Next
            AnrMonRING()
        End Set
    End Property

    ''' <summary>
    '''         0        ; 1  ;2;3;  4   ;    5     ; 6  ; 7
    ''' 23.06.18 13:20:24;CALL;3;4;987654;0123456789;SIP0;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonCALL As String()
        Set(FBStatus As String())
            AnrufRichtung = AnrufRichtungen.Ausgehend

            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitBeginn = CDate(FBStatus(i))

                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = FBStatus(i).ToInt

                    Case 3 ' Nebenstellennummer, eindeutige Zuordnung des Telefons
                        NebenstellenNummer = CInt(FBStatus(i))

                    Case 4 ' Eigene Ausgehende Telefonnummer. Kann bei Rufweiterleitungen eine externe Nummer sein.
                        EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr(FBStatus(i))

                        If EigeneTelNr Is Nothing Then

                            EigeneTelNr = New Telefonnummer With {.SetNummer = FBStatus(i),
                                                                  .EigeneNummerInfo = New EigeneNrInfo With {.Überwacht = True}}

                            ' Rufweiterleitung wurde festgestellt
                            If Rufweiterleitung Then
                                NLogger.Info($"Rufweiterleitung für {EigeneTelNr.Unformatiert} erfasst.")
                            Else
                                NLogger.Warn($"Eigene Telefonnummer für {FBStatus(i)} konnte nicht ermittelt werden.")
                            End If

                        End If

                        ' Wert für Serialisierung in separater Eigenschaft ablegen
                        OutEigeneTelNr = EigeneTelNr.Unformatiert

                    Case 5 ' Gewählte Telefonnummer
                        ' Dies kann auch eine interne Nebenstellennummer sein:
                        ' 01.05.22 10:18:04;CALL;2;4;987654;62;SIP4;
                        GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}

                    Case 6
                        AnschlussID = FBStatus(i)

                End Select
            Next

            AnrMonCALL()
        End Set
    End Property

    ''' <summary>
    '''         0        ;   1   ;2;3 ;    4     ; 5 
    ''' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonCONNECT As String()
        Set(FBStatus As String())
            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitVerbunden = CDate(FBStatus(i))
                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = FBStatus(i).ToInt
                    Case 3 ' Nebenstellennummer, eindeutige Zuordnung des Telefons
                        NebenstellenNummer = CInt(FBStatus(i))
                    Case 4 ' Gewählte Telefonnummer (CALL) bzw. eingehende Telefonnummer (RING)
                        If Not GegenstelleTelNr.Equals(FBStatus(i)) Then GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}
                End Select
            Next

            AnrMonCONNECT()
        End Set
    End Property

    ''' <summary>
    '''         0        ;   1      ;2;3; 4
    ''' 23.06.18 13:20:52;DISCONNECT;1;9;
    ''' </summary>
    <XmlIgnore> Friend WriteOnly Property SetAnrMonDISCONNECT As String()
        Set(FBStatus As String())
            For i = LBound(FBStatus) To UBound(FBStatus)
                Select Case i
                    Case 0 ' Uhrzeit des Telefonates - Startzeit
                        ZeitEnde = CDate(FBStatus(i))
                    Case 2 ' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
                        ID = FBStatus(i).ToInt
                    Case 3 ' Dauer des Telefonates
                        Dauer = CInt(FBStatus(i))
                End Select
            Next

            AnrMonDISCONNECT()
        End Set
    End Property
#End Region

#Region "Structures"
    Friend Structure AnrufRichtungen
        Friend Const Eingehend As Integer = 0
        Friend Const Ausgehend As Integer = 1
    End Structure
#End Region

    Friend Sub New()
        'Stop
    End Sub

#Region "Kontaktsuche"
    ''' <summary>
    ''' Führt die Kontaktsuche durch.
    ''' 1. Outlook-Kontakte
    ''' 2. Fritz!Box Telefonbücher
    ''' 3. Tellows
    ''' 4. Rückwärtssuche
    ''' </summary>
    Friend Async Sub KontaktSuche()
        Await KontaktSucheTask()
    End Sub

    Friend Async Function KontaktSucheTask() As Task

        ' Führe keine Kontaktsuche durch, wenn die Nummer unterdrückt ist
        If Not NrUnterdrückt Then

            ' Kontaktsuche in den Outlook-Kontakten
            OlKontakt = Await KontaktSucheTelNr(GegenstelleTelNr)

            If OlKontakt IsNot Nothing Then
                ' Ein Kontakt wurde gefunden
                With OlKontakt
                    ' Anrufernamen ermitteln
                    AnruferName = .FullName

                    ' Firma aus Kontaktdaten ermitteln
                    Firma = .CompanyName

                    ' KontaktID und StoreID speichern
                    OutlookKontaktID = .EntryID
                    OutlookStoreID = .StoreID

                    ' Log-Eintrag erzeugen
                    NLogger.Debug($"Kontakt '{AnruferName}' für Telefonnummer '{GegenstelleTelNr.Unformatiert}' in Outlook-Kontakten gefunden.")

                    ' Flag setzen, dass Kontaktinformationen für Gegenstelle ermittelt wurden
                    AnruferErmittelt = True
                End With
            End If

            ' Kontaktsuche in den Fritz!Box Telefonbüchern
            If Not AnruferErmittelt Then
                If XMLData.POptionen.CBKontaktSucheFritzBox Then

                    If Globals.ThisAddIn.PhoneBookXML Is Nothing OrElse Globals.ThisAddIn.PhoneBookXML.Where(Function(b) b.NurName).Any Then
                        ' Wenn die Telefonbücher noch nicht heruntergeladen wurden, oder nur die Namen bekannt sind, dann lade die Telefonbücher herunter.
                        NLogger.Debug($"Die Telefonbücher sind für die Kontaktsuche nicht bereit. Beginne sie herunterzuladen...")
                        Globals.ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeTelefonbücher()
                    End If

                    ' Wenn die Telefonbücher immer noch nicht zur Verfügung stehen, brich an dieser Stelle ab
                    If Globals.ThisAddIn.PhoneBookXML IsNot Nothing AndAlso Not Globals.ThisAddIn.PhoneBookXML.Where(Function(b) b.NurName).Any Then
                        FBTelBookKontakt = Telefonbücher.Find(Globals.ThisAddIn.PhoneBookXML, GegenstelleTelNr)
                    Else
                        NLogger.Warn("Kontaktsuche in Fritz!Box Telefonbüchern ist nicht möglich: Telefonbücher sind nicht heruntergeladen worden.")
                    End If

                    If FBTelBookKontakt IsNot Nothing Then
                        If XMLData.POptionen.CBKErstellen Then
                            OlKontakt = ErstelleKontakt(FBTelBookKontakt, GegenstelleTelNr, True)

                            With OlKontakt
                                AnruferName = .FullName
                                Firma = .CompanyName
                            End With
                        Else
                            AnruferName = FBTelBookKontakt.Person.RealName
                        End If

                        ' Log-Eintrag erzeugen
                        NLogger.Debug($"Kontakt '{AnruferName}' für Telefonnummer '{GegenstelleTelNr.Unformatiert}' im Fritz!Box Telefonbuch gefunden.")

                        ' Flag setzen, dass Kontaktinformationen für Gegenstelle ermittelt wurden
                        AnruferErmittelt = True
                    End If
                End If
            End If

            ' Kontaktsuche über die Rückwärtssuche
            If Not AnruferErmittelt Then

                ' Starte die Suche mit Tellows, wenn der User das wünscht, und nur bei eingehenden Telefonaten
                If XMLData.POptionen.CBTellows AndAlso AnrufRichtung = AnrufRichtungen.Eingehend And GegenstelleTelNr.TellowsNummer.Length.IsLargerOrEqual(4) Then
                    ' Auswertung bei aktiven Anrufen via LiveAPI
                    If Not Import Then
                        Using tellows = New Tellows()

                            ' Führe eine Suche via LiveAPI durch
                            TellowsResult = Await tellows.GetTellowsLiveAPIData(GegenstelleTelNr)
                            If TellowsResult IsNot Nothing Then

                                If TellowsResult.Score.AreEqual(5) And TellowsResult.Comments.IsLess(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                                    ' Verwirf das Ergebnis. Es gibt keinen Eintrag bei tellows
                                    TellowsResult = Nothing
                                    NLogger.Debug($"Kein Eintrag bei tellows für Telefonnummer '{GegenstelleTelNr.TellowsNummer}' gefunden.")
                                Else
                                    ' Verarbeite das Tellows Ergebnis
                                    With TellowsResult

                                        NLogger.Debug($"Eintrag bei tellows für Telefonnummer '{GegenstelleTelNr.TellowsNummer}' mit Score { .Score} gefunden.")

                                        ' Ergebnisse werden nur eingeblendet, wenn die Nutzereingaben passen
                                        If .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinScore) And .Comments.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                                            AnruferName = .CallerName
                                            Firma = .CallerType
                                        End If

                                        If XMLData.POptionen.CBTellowsAutoFBBlockList AndAlso .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAutoScoreFBBlockList) Then
                                            ' Sperrlisteintrag erzeugen
                                            AddToCallBarring(New List(Of String) From { .Number}, .CallerName)
                                        End If
                                    End With
                                    AnruferErmittelt = True
                                End If

                            End If
                        End Using
                    Else
                        ' Auswertung bei importieren Anrufen via CallListAPI
                        If Globals.ThisAddIn.TellowsScoreList IsNot Nothing Then
                            TellowsResult = Globals.ThisAddIn.TellowsScoreList.Find(Function(Eintrag) GegenstelleTelNr.Equals(Eintrag.Number))
                            If TellowsResult IsNot Nothing Then
                                With TellowsResult
                                    NLogger.Debug($"Eintrag in der tellows-ScoreList für Telefonnummer '{GegenstelleTelNr.TellowsNummer}' mit Score { .Score} gefunden.")

                                    AnruferName = .CallerName
                                    Firma = .CallerType

                                    If XMLData.POptionen.CBTellowsAutoFBBlockList AndAlso .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAutoScoreFBBlockList) Then
                                        ' Sperrlisteintrag erzeugen
                                        AddToCallBarring(New List(Of String) From { .Number}, AnruferName)
                                    End If
                                End With
                                AnruferErmittelt = True
                            End If
                        End If
                    End If
                End If
            End If

            If Not AnruferErmittelt Then
                ' Eine Rückwärtssuche braucht nur dann gemacht werden, wenn die Länge der Telefonnummer aussreichend ist.
                ' Ggf. muss der Wert angepasst werden.
                If XMLData.POptionen.CBRWS AndAlso GegenstelleTelNr.Unformatiert.Length.IsLargerOrEqual(4) Then

                    VCard = Await StartRWS(GegenstelleTelNr, XMLData.POptionen.CBRWSIndex)

                    If VCard.IsNotStringNothingOrEmpty Then

                        NLogger.Info($"Rückwärtssuche für '{GegenstelleTelNr.Unformatiert}' erfolgreich: {VCard}")

                        If XMLData.POptionen.CBKErstellen Then
                            OlKontakt = ErstelleKontakt(VCard, GegenstelleTelNr, True)
                            With OlKontakt
                                AnruferName = .FullName
                                Firma = .CompanyName
                            End With

                        Else
                            With MixERP.Net.VCards.Deserializer.GetVCard(VCard)
                                AnruferName = .FormattedName
                                Firma = .Organization
                            End With
                        End If

                        NLogger.Debug($"Kontakt '{AnruferName}' für Telefonnummer '{GegenstelleTelNr.Unformatiert}' per Rückwärtssuche gefunden.")

                    End If
                End If

            End If

            ' Zeige Kontakt
            If XMLData.POptionen.CBAnrMonZeigeKontakt Then ZeigeKontakt()
        Else
            AnruferName = Localize.LocAnrMon.strNrUnterdrückt
        End If
    End Function
#End Region

    ''' <summary>
    ''' Funktion, welche das öffnen des hinterlegten Kontaktes anstößt
    ''' </summary>
    Friend Sub ZeigeKontakt()
        ' Es gibt mehrere Varianten zu beachten.
        ' 1. Ein Kontakte ist in der Eigenschaft hinterlegt.
        ' 2. Es ist kein Kontakt hinterlegt, jedoch KontaktID und StoreID
        ' 3. Es ist eine vCard hinterlegt
        ' 4. Es ist nur eine Nummer hinterlegt
        ' 5. Es ist nichts hinterlegt.

        If OlKontakt IsNot Nothing Then
            ' Blende den Kontakt ein
            OlKontakt.Display()

        ElseIf OutlookKontaktID.IsNotStringNothingOrEmpty And OutlookStoreID.IsNotStringNothingOrEmpty Then
            ' Ermittle den bestehenden Outlook-Kontakt
            OlKontakt = GetOutlookKontakt(OutlookKontaktID, OutlookStoreID)

            ' Blende den Kontakt ein
            If OlKontakt IsNot Nothing Then OlKontakt.Display()
        Else
            ' ein Kontaktitem, welches eingeblendet werden kann muss erst erzeugt werden
            If FBTelBookKontakt IsNot Nothing Then

                ' Es gibt einen Kontakt aus dem Fritz!Box Telefonbuch.
                OlKontakt_wEvents = ErstelleKontakt(FBTelBookKontakt, GegenstelleTelNr, False)

            ElseIf VCard.IsNotStringNothingOrEmpty Then
                ' eine vCard ist verfügbar
                OlKontakt_wEvents = ErstelleKontakt(VCard, GegenstelleTelNr, False)
            Else
                ' eine Telefonnummer ist als einige Information vorhanden
                OlKontakt_wEvents = ErstelleKontakt(GegenstelleTelNr, False)
            End If

            ' Füge einen Ereignishandler hinzu, der das Speichern dieses temporären Kontaktes überwacht
            AddHandler OlKontakt_wEvents.Write, AddressOf EOlKontakt_wEvents_Write

            ' Blende den temporäten Kontakt ein
            If OlKontakt_wEvents IsNot Nothing Then OlKontakt_wEvents.Display()

        End If

    End Sub

    Private Sub EOlKontakt_wEvents_Write(ByRef Cancel As Boolean)

        With OlKontakt_wEvents

            ' Entferne den Ereignishandler
            RemoveHandler .Write, AddressOf EOlKontakt_wEvents_Write

            ' Merke Kontakt und StoreID
            OutlookStoreID = .StoreID
            OutlookKontaktID = .EntryID

        End With
    End Sub

    ''' <summary>
    ''' Ruft die Gegenstellentelefonnummer an
    ''' </summary>
    Friend Sub Rückruf()
        ' Wenn man aus dem Anrufmonitor auf Rückruf klickt, öffnet sich der Wähldialog.
        ' Wenn man dann den Anrufmonitor schließt, stürzt Outlook ab. Fix für #60
        With Globals.ThisAddIn.WPFApplication.Dispatcher

            .Invoke(Sub()
                        ' Neuen Wählclient generieren
                        ' Finde das existierende Fenster, oder generiere ein neues
                        With New FritzBoxWählClient
                            .WählboxStart(Me)
                        End With

                    End Sub)
        End With

    End Sub

    ''' <summary>
    ''' Schließt den Anrufmonitor und denn Eintrag im MissedCallPande
    ''' </summary>
    Friend Sub CloseAnrMonAndCallPane()
        ' Schließe den vorhandenen Anrufmonitor, falls vorhanden
        If XMLData.POptionen.CBAnrMonCloseReDial Then
            ' MissedCallPane entfernen
            Globals.ThisAddIn.ExplorerWrappers.Values.ToList.ForEach(Sub(ew) ew.RemoveMissedCall(Me))

            ' Sofortiges Ausblenden des Anrufmonitors
            If PopUpAnrMonWPF IsNot Nothing Then PopUpAnrMonWPF.StarteAusblendTimer(TimeSpan.Zero)
        End If
    End Sub

    ''' <summary>
    ''' Wird durch die Auswertung der Anrufliste aufgerufen. Erstellt Jounaleinträge und aktualisiert die Listen.
    ''' </summary>
    Friend Sub SetUpOlLists(UpdateCallPane As Boolean)
        If IstRelevant Then
            ' Erstelle einen Journaleintrag
            ErstelleJournalEintrag()

            ' Anruflisten aktualisieren
            UpdateRingCallList()

            ' CallPane ergänzen
            If UpdateCallPane Then SetMissedCallPane()
        Else
            NLogger.Debug($"Anruf {ID} wurde nicht importiert.")
        End If

        ' Die Auswertung ist abgeschlossen. Merke dir die ID dieses Eintrages, wenn er größer/neuer ist
        XMLData.POptionen.FBoxCallListLastImportedID = ID.GetLarger(XMLData.POptionen.FBoxCallListLastImportedID)
    End Sub

    ''' <summary>
    ''' Routine zum erstellen eines Outlook Journaleintrages.
    ''' </summary>
    Friend Sub ErstelleJournalEintrag()

        If XMLData.POptionen.CBJournal Then

            If Globals.ThisAddIn.Application IsNot Nothing Then

                Dim olJournal As Outlook.JournalItem = Nothing
                Dim olJournalFolder As OutlookOrdner

                Try
                    ' Erstelle ein Journaleintrag im Standard-Ordner.
                    olJournal = CType(Globals.ThisAddIn.Application.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
                Catch ex As Exception
                    NLogger.Error(ex)
                End Try

                If olJournal IsNot Nothing Then
                    Dim tmpSubject As String

                    If Blockiert Then
                        tmpSubject = Localize.LocAnrMon.strJournalBlockiert
                    Else
                        If Rufweiterleitung Then
                            ' Eine Rufweiterleitung wurde erfasst
                            tmpSubject = Localize.LocAnrMon.strJournalRufweiterleitung
                        Else
                            If Angenommen Then
                                tmpSubject = If(AnrufRichtung = AnrufRichtungen.Ausgehend, Localize.LocAnrMon.strJournalAusgehend, Localize.LocAnrMon.strJournalEingehend)
                            Else 'Verpasst
                                tmpSubject = If(AnrufRichtung = AnrufRichtungen.Ausgehend, Localize.LocAnrMon.strJournalNichterfolgreich, Localize.LocAnrMon.strJournalVerpasst)
                            End If
                        End If
                    End If

                    With olJournal

                        .Subject = $"{tmpSubject} {If(Rufweiterleitung, $"{EigeneTelNr.Formatiert} zu ", String.Empty)}{AnruferName}{If(NrUnterdrückt, String.Empty, If(AnruferName.IsStringNothingOrEmpty, GegenstelleTelNr.Formatiert, $" ({GegenstelleTelNr.Formatiert})"))}"
                        .Duration = Dauer.GetLarger(31) \ 60
                        .Body = $"{Localize.LocAnrMon.strJournalBodyStart} {If(NrUnterdrückt, Localize.LocAnrMon.strNrUnterdrückt, GegenstelleTelNr.Formatiert)}{vbCrLf}Status: {If(Angenommen, String.Empty, "nicht ")}angenommen{vbCrLf & vbCrLf}{VCard}"
                        .Start = ZeitBeginn
                        .Companies = Firma

                        ' Bei verpassten Anrufen ist TelGerät ggf. leer
                        .Categories = $"{If(TelGerät Is Nothing, Localize.LocAnrMon.strJournalCatVerpasst, TelGerät.Name)};{String.Join("; ", DfltOlItemCategories.ToArray)}"

                        ' Speichern der EntryID und StoreID in benutzerdefinierten Feldern
                        If OlKontakt IsNot Nothing Then

                            Dim colArgs(1) As Object
                            colArgs(0) = OlKontakt.EntryID
                            colArgs(1) = OlKontakt.StoreID

                            For i As Integer = 0 To 1
                                .PropertyAccessor.SetProperty(DASLTagOlItem(i).ToString, colArgs(i))
                            Next

                            ' Funktioniert aus irgendeinem dummen Grund nicht. Die EntryID wird nicht übertragen.
                            '.PropertyAccessor.SetProperties(DASLTagOlItem, colArgs)
                        End If

                        ' Speicherort wählen
                        olJournalFolder = XMLData.POptionen.OutlookOrdner.Find(OutlookOrdnerVerwendung.JournalSpeichern)

                        If olJournalFolder IsNot Nothing AndAlso olJournalFolder.MAPIFolder IsNot Nothing AndAlso
                        Not olJournalFolder.Equals(Globals.ThisAddIn.Application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderJournal)) Then
                            ' Verschiebe den Journaleintrag in den ausgewählten Ordner
                            ' Damit wird der Journaleintrag gleichzeitig im Zielordner gespeichert.
                            .Move(olJournalFolder.MAPIFolder)
                            ' Verwerfe diesen Journaleintrag
                            .Close(Outlook.OlInspectorClose.olDiscard)

                            NLogger.Info($"Journaleintrag im Ordner {olJournalFolder.Name} (Store: {olJournalFolder.MAPIFolder.Store.DisplayName}) erstellt: { .Start}, { .Subject}, { .Duration}")
                        Else
                            ' Speicher den Journaleintrag im Standard-Ordner
                            .Close(Outlook.OlInspectorClose.olSave)
                            With Globals.ThisAddIn.Application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderJournal)
                                NLogger.Info($"Journaleintrag im Standardordner { .Name} (Store: { .Store.DisplayName}) erstellt: { olJournal.Start}, { olJournal.Subject}, { olJournal.Duration}")
                            End With
                        End If

                        ' Merke die Zeit
                        UpdateTimeAnrList()

                    End With

                    ReleaseComObject(olJournal)
                End If

            Else
                NLogger.Warn(Localize.LocAnrMon.strJournalFehler)
            End If

        End If
    End Sub

    Friend Sub ErstelleErinnerungEintrag()
        If Globals.ThisAddIn.Application IsNot Nothing Then
            Dim olAppointment As Outlook.AppointmentItem = Nothing
            Dim olAppointmentFolder As OutlookOrdner

            Try
                ' Erstelle einen Termin im Standard-Ordner.
                olAppointment = CType(Globals.ThisAddIn.Application.CreateItem(Outlook.OlItemType.olAppointmentItem), Outlook.AppointmentItem)
            Catch ex As Exception
                NLogger.Error(ex)
            End Try

            If olAppointment IsNot Nothing Then

                With olAppointment
                    .Subject = $"{Localize.resCommon.strAppointmentSubject} {NameGegenstelle}"
                    .Categories = $"{String.Join("; ", DfltOlItemCategories.ToArray)}"
                    .Start = Now.AddMinutes(XMLData.POptionen.TBAppointmentOffset)
                    .Duration = XMLData.POptionen.TBAppointmentDauer
                    .Importance = Outlook.OlImportance.olImportanceHigh

                    .ReminderSet = True
                    .ReminderMinutesBeforeStart = XMLData.POptionen.TBAppointmentReminder

                    .Body = $"{Localize.LocAnrMon.strJournalBodyStart} {If(NrUnterdrückt, Localize.LocAnrMon.strNrUnterdrückt, GegenstelleTelNr.Formatiert)}{vbCrLf}Status: {If(Angenommen, String.Empty, "nicht ")}angenommen{vbCrLf & vbCrLf}{VCard}"

                    ' Speichern der EntryID und StoreID in benutzerdefinierten Feldern
                    If OlKontakt IsNot Nothing Then

                        Dim colArgs(1) As Object
                        colArgs(0) = OlKontakt.EntryID
                        colArgs(1) = OlKontakt.StoreID

                        ' TODO: OlKontakt ist aufgelößt wenn Wählclient vorher genutzt.
                        For i As Integer = 0 To 1
                            .PropertyAccessor.SetProperty(DASLTagOlItem(i).ToString, colArgs(i))
                        Next

                        ' Funktioniert aus irgendeinem dummen Grund nicht. Die EntryID wird nicht übertragen.
                        '.PropertyAccessor.SetProperties(DASLTagOlItem, colArgs)
                    End If

                    ' Speicherort wählen
                    olAppointmentFolder = XMLData.POptionen.OutlookOrdner.Find(OutlookOrdnerVerwendung.TerminSpeichern)

                    If olAppointmentFolder IsNot Nothing AndAlso olAppointmentFolder.MAPIFolder IsNot Nothing AndAlso
                        Not olAppointmentFolder.Equals(Globals.ThisAddIn.Application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar)) Then

                        ' Die Erinnerung wird erst übernommen, wenn der Termin gespeichert wurde.
                        .Save()
                        ' Verschiebe den Kalendereintrag in den ausgewählten Ordner
                        ' Damit wird der Termin gleichzeitig im Zielordner gespeichert.

                        .Move(olAppointmentFolder.MAPIFolder)
                        ' Verwerfe diesen Termin
                        .Close(Outlook.OlInspectorClose.olDiscard)

                        NLogger.Info($"Anruferinnerung im Kalender {olAppointmentFolder.Name} (Store: {olAppointmentFolder.MAPIFolder.Store.DisplayName}) erstellt: { .Start}, { .Subject}, { .Duration}")
                    Else
                        ' Speicher den Termin im Standard-Ordner
                        .Close(Outlook.OlInspectorClose.olSave)
                        With Globals.ThisAddIn.Application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar)
                            NLogger.Info($"Anruferinnerung im Standardordner { .Name} (Store: { .Store.DisplayName}) erstellt: { olAppointment.Start}, { olAppointment.Subject}, { olAppointment.Duration}")
                        End With
                    End If

                    If XMLData.POptionen.CBAppointmentDisplay Then .Display()

                End With

                ReleaseComObject(olAppointment)
            End If
        Else
            NLogger.Warn(Localize.resCommon.strAppointmentError)
        End If
    End Sub
    ''' <summary>
    ''' Routine zum Aktualisieren der Wahlwiederholungs- und Rückrufliste. Das Telefonat wird in die entsprechende Liste aufgenommen.
    ''' </summary>
    Friend Sub UpdateRingCallList()

        ' Nicht bei Rufweiterleitungen durchführen
        If XMLData.POptionen.CBAnrListeUpdateCallLists And Not Rufweiterleitung Then
            ' Überprüfe, ob eigene Nummer überhaupt überwacht wird            ' 
            If AnrufRichtung = AnrufRichtungen.Eingehend Then
                ' RING-Liste initialisieren, falls erforderlich
                If XMLData.PTelListen.RINGListe Is Nothing Then XMLData.PTelListen.RINGListe = New List(Of Telefonat)
                ' Telefonat in erste Positon der RING-Liste speichern
                XMLData.PTelListen.RINGListe.Insert(Me)
            Else
                ' CALL-Liste initialisieren, falls erforderlich
                If XMLData.PTelListen.CALLListe Is Nothing Then XMLData.PTelListen.CALLListe = New List(Of Telefonat)
                ' Telefonat in erste Positon der CALL-Liste speichern
                XMLData.PTelListen.CALLListe.Insert(Me)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Routine zum Aktualisieren des Outlook Seitenfensters. Das Telefonat wird in die Liste aufgenommen.
    ''' </summary>
    Private Sub SetMissedCallPane()
        If XMLData.POptionen.CBShowMissedCallPane Then
            If Not Angenommen And AnrufRichtung = AnrufRichtungen.Eingehend Then
                ' Schleife durch jeden offenen Explorer
                Globals.ThisAddIn.ExplorerWrappers.Values.ToList.ForEach(Sub(ew) ew.AddMissedCall(Me))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Routine zur Ermittlung der aufgenommenen Nachricht auf einem der eingerichteten Fritz!Box Anrufbeantworter.
    ''' </summary>
    Private Async Sub GetTAMMessage()
        ' Wenn der Fritz!Box Anrufbeantworter rangegangen ist, liegt eine Nachricht ggf. vor. Anhand der Gegenstellennummer, der eigenen Nummer und der Anrufzeit wird der Eintrag ermittelt.

        ' Überrpüfung, ob ein Anrufbeantworter rangegangen ist und dessen AnrMonID größer oder gleich 40 ist
        If TelGerät?.TelTyp = DfltWerteTelefonie.TelTypen.TAM AndAlso TelGerät?.AnrMonID.IsLargerOrEqual(DfltWerteTelefonie.AnrMonTelIDBase.TAM) Then
            ' lade die MessageList herunter, Ermittle anhand der ID den relevanten Anrufbeantworter

            Dim TAM_ID As Integer = TelGerät.AnrMonID - DfltWerteTelefonie.AnrMonTelIDBase.TAM
            With Await GetTAMMessages(TAM_ID)
                ' Im Fehlerfall ist die Liste leer.
                ' Es wird verglichen:
                ' A: Gegenstellennummer mit der Nummer des anrufenden
                ' B: Eigene Nummer, welche Angerufen wurde
                ' C: Zeit des Verbindens: Die Zeit wird ohne Sekundenangabe übergeben. Daher wird diese rausgerechnet
                Dim MList As IEnumerable(Of FBoxAPI.Message) = .Where(Function(M) GegenstelleTelNr.Equals(M.Number) AndAlso
                                                                                  EigeneTelNr.Equals(M.Called) AndAlso
                                                                                  ZeitVerbunden.AddSeconds(-ZeitVerbunden.Second).Equals(CDate(M.Date)))
                If MList.Any Then
                    If MList.Count.AreEqual(1) Then
                        With MList.First
                            ' Wenn Messages gefunden wurden...
                            NLogger.Debug($"Anrufbeantworter ({TAM_ID}): Benachrichtigung gefunden ({ .Index}): { .Date}, { .Number}, { .Path}")

                            ' Merke den Pfad zur Audiodatei
                            TAMMessagePath = .Path
                        End With
                    Else
                        NLogger.Warn($"Es wurden mehr als eine passende TAM Benachrichtigung gefunden.")
                    End If

                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' <para>Ermittelt das Telefoniegerät, mit dem das Telefonat geführt wird. Dies ist nur bei CALL und CONNECT möglich. 
    ''' Leider kann das Telefon nicht in allen Fällen ermittelt werden.</para>
    ''' </summary>
    ''' <param name="AnrListDeviceName">Name des Gerätes aus der Anrufliste der Fritz!Box</param>
    ''' <see href="link">https://freetz-ng.github.io/freetz-ng/make/callmonitor.html#ereignis-informationen-f%C3%BCr-aktionen</see>
    Friend Sub SetTelefoniegerät(Optional AnrListDeviceName As String = "")

        Select Case NebenstellenNummer
            Case -1 ' Tritt bei der Auswertung der Anrufliste auf. Ein Ermitteln des Gerätes ist nicht möglich.

            Case AnrMonTelIDBase.CallThrough ' 3
                TelGerät = New Telefoniegerät With {.Name = "Rufweiterleitung",
                                                    .AnrMonID = AnrMonTelIDBase.CallThrough,
                                                    .TelTyp = TelTypen.CallThrough}

            Case AnrMonTelIDBase.S0 ' 4
                ' ISDN S0 Geräte können nicht anhand der Nebenstellennummer ermittelt werden, da hier immer die 4 übermittelt wird.
                TelGerät = New Telefoniegerät With {.Name = "ISDN/S0",
                                                    .AnrMonID = AnrMonTelIDBase.S0,
                                                    .TelTyp = TelTypen.ISDN}

            Case AnrMonTelIDBase.OldTAM ' 6
                ' Aus der Dokumentation der Anrufliste:
                ' If port equals 6 or port in in the rage of 40 to 49 it is a TAM call.
                TelGerät = New Telefoniegerät With {.Name = "Anrufbeantworter",
                                                    .AnrMonID = AnrMonTelIDBase.OldTAM,
                                                    .TelTyp = TelTypen.TAM}

            Case AnrMonTelIDBase.DataFON1 ' 32
                TelGerät = New Telefoniegerät With {.Name = "Data FON1",
                                                    .AnrMonID = AnrMonTelIDBase.DataFON1,
                                                    .TelTyp = TelTypen.DATA}

            Case AnrMonTelIDBase.DataFON2 ' 33
                TelGerät = New Telefoniegerät With {.Name = "Data FON2",
                                                    .AnrMonID = AnrMonTelIDBase.DataFON2,
                                                    .TelTyp = TelTypen.DATA}

            Case AnrMonTelIDBase.DataFON3 ' 34
                TelGerät = New Telefoniegerät With {.Name = "Data FON3",
                                                    .AnrMonID = AnrMonTelIDBase.DataFON3,
                                                    .TelTyp = TelTypen.DATA}

            Case AnrMonTelIDBase.DataS0 ' 36
                TelGerät = New Telefoniegerät With {.Name = "Data S0",
                                                    .AnrMonID = AnrMonTelIDBase.DataS0,
                                                    .TelTyp = TelTypen.DATA}

            Case AnrMonTelIDBase.DataPC ' 37
                TelGerät = New Telefoniegerät With {.Name = "Data PC",
                                                    .AnrMonID = AnrMonTelIDBase.DataPC,
                                                    .TelTyp = TelTypen.DATA}

            Case Else
                ' FON, DECT, IP, TAM, interner Faxempfang
                ' Ermittle die Daten des genutzen Telefons aus der Liste aller bekannten Telefone.
                TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID.AreEqual(NebenstellenNummer))
        End Select

        ' Fallback: Versuche das Gerät anhand des Namens zu ermitteln (Nur bei Auwertung der Anrufliste möglich)
        If TelGerät Is Nothing AndAlso AnrListDeviceName.IsNotStringNothingOrEmpty Then
            TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.Name.IsEqual(AnrListDeviceName))
        End If

        ' Logeintrag
        If TelGerät Is Nothing Then
            NLogger.Warn($"Telefoniegerät nicht ermittelt: NebenstellenNummer: {NebenstellenNummer}; AnrListDeviceName: '{AnrListDeviceName}'")
        Else
            NLogger.Debug($"Telefoniegerät ermittelt: {TelGerät.Name} (NebenstellenNummer: {NebenstellenNummer})")
        End If
    End Sub

#Region "Anrufmonitor"
    Private Sub AnrMonRING()
        ' prüfe, ob die anrufende Nummer auf der Rufsperre der Fritz!Box steht
        If EigeneTelNr.EigeneNummerInfo.Überwacht Then Blockiert = IsFBoxBlocked(GegenstelleTelNr)

        'Abweisen()

        If IstRelevant Then
            ' Starte die Kontaktsuche mit Hilfe asynchroner Routinen, da ansonsten der Anrufmonitor erst eingeblendet wird, wenn der Kontakt ermittelt wurde
            ' Anrufername aus Kontakten und Rückwärtssuche ermitteln
            KontaktSuche()

            ' Anrufmonitor einblenden,
            ShowAnrMon()

            ' Aktualisiere die Rückrufliste
            UpdateRingCallList()

        End If

    End Sub

    'Private Sub Abweisen()
    '    NLogger.Debug($"Rufabweisung")
    '    Dim PhoneName As String = String.Empty
    '    With Globals.ThisAddIn.FBoxTR064
    '        If .Ready Then
    '            If .X_voip.DialGetConfig(PhoneName) Then
    '                ' Setze auf das Telefon, auf dass das Telefonat umgeleitet werden soll
    '                .X_voip.DialSetConfig("ISDN: TelefonTest")

    '                ' Hole das Telefonat ran
    '                .X_voip.DialNumber("**062")

    '                ' Setze auf das Telefon, was ursprünglich eingesetzt war.
    '                .X_voip.DialSetConfig(PhoneName)
    '            End If
    '        End If
    '    End With
    'End Sub

    Private Sub AnrMonCALL()

        If IstRelevant Then
            ' Anrufername aus Kontakten und Rückwärtssuche ermitteln, sofern es sich nicht um eine Weiterleitung handelt.
            If Not Intern And Not Rufweiterleitung Then KontaktSuche()

            ' 01.05.22 10:18:04;CALL;2;4;987654;62;SIP4;

            ' Telefoniegerät ermitteln
            SetTelefoniegerät()

            ' Eigene Nummer prüfen. Mittels Wählpräfix (*10X#, *11X#, *12X#) kann die ausgehende Telefonnummer beeinflusst werden. 
            ' Der Anrufmonitor gibt jedoch weiterhin die eigentlich genutzte eigene Nummer für diese Nebenstelle wieder.
            ' Unterschieden werden kann nur per AnschlussID (letzer Datenwert SIP... etc)
            ' 03.03.21 15:48:18;CALL;1;4;123456;0049987654321#;SIP3; *124# 654321
            ' 03.03.21 16:35:54;CALL;1;4;123456;0049987654321#;SIP1; *122# 123456

            If Rufweiterleitung Then
                ' Bei Rufumleitungen im Modus Automatisch wird die umgeleitete eingehende Nummer als eigene rausgehende Nummer verwendet.
                ' Muss hier was gemacht werden?
            Else
                If AnschlussID.IsNotStringNothingOrEmpty Then

                    Dim AnschlussIDTelNr As Telefonnummer = XMLData.PTelefonie.GetEigeneTelNr(AnschlussID)

                    If Not EigeneTelNr.Equals(AnschlussIDTelNr) Then
                        ' Eintrag ins Log
                        NLogger.Debug($"Übermittelte eigene Telefonnummer '{EigeneTelNr.Unformatiert}' mit '{AnschlussIDTelNr.Unformatiert}' überschrieben ({AnschlussID}).")

                        ' Telefonnummer ersetzen
                        EigeneTelNr = AnschlussIDTelNr
                    End If
                End If
            End If

            ' Aktualisiere die Wahlwiederholungsliste
            UpdateRingCallList()
        End If

    End Sub

    Private Sub AnrMonCONNECT()

        If IstRelevant Then
            ' Telefoniegerät ermitteln
            SetTelefoniegerät()

            ' Setze Flag, dass das Telefonat angenommen wurde.
            If TelGerät Is Nothing Then
                NLogger.Warn($"Telefoniegerät für Anruf {ID} mit AnrMonID {NebenstellenNummer} nicht gefunden.")
                ' Eine überprüfung, ob es sich um ein TAM handelt, kann nicht durchgeführt werden.
                Angenommen = True
            Else
                ' Angenommen wird nicht auf True gesetzt, wenn ein TAM rangegangen ist und dies gemäß Einstellungen als verpasst gekennzeichnet werden soll
                Angenommen = Not (TelGerät.IsTAM AndAlso XMLData.POptionen.CBIsTAMMissed)
            End If

            ' Anrufmonitor ausblenden einleiten, falls dies beim CONNECT geschehen soll
            If XMLData.POptionen.CBAutoClose And XMLData.POptionen.CBAnrMonHideCONNECT Then
                ' Ausblenden nur Starten, wenn der Anrufbeaantworter nicht rangegangen ist.
                ' Es kann sein, dass das Gerät nicht ermittelt wurde. Dann starte das Ausblenden trotzdem
                AnrMonStartHideTimer = TelGerät Is Nothing OrElse Not (TelGerät.IsTAM AndAlso XMLData.POptionen.CBIsTAMMissed)
            End If

            ' Stoppuhr einblenden, wenn Bedingungen erfüllt 
            If XMLData.POptionen.CBStoppUhrEinblenden Then ShowStoppUhr()
        End If

    End Sub

    Private Sub AnrMonDISCONNECT()
        Beendet = True

        If IstRelevant Then
            ' Stoppuhr ausblenden, wenn dies in den Einstellungen gesetzt ist
            If StoppUhrEingeblendet And XMLData.POptionen.CBStoppUhrAusblenden Then PopupStoppUhrWPF.StarteAusblendTimer(TimeSpan.FromSeconds(XMLData.POptionen.TBStoppUhrAusblendverzögerung))

            ' CallListPane füllen, wenn es sich um einen eingehenden Anruf handelt.
            SetMissedCallPane()

            ' Journaleintrag
            ErstelleJournalEintrag()

            ' Ermittle die aufgenommene Benachrichtigung des Anrufbeantwortes
            GetTAMMessage()
        End If

    End Sub

#Region "Anrufmonitor-Fenster"
    Friend Sub AnrMonEinblenden()
        ' Zeige den Anrufmonitor nur an, wenn gerade nicht schon eingeblendet.
        If Not AnrMonEingeblendet Then

            ' Erstelle die Liste der aktuell eingeblendeten Anrufmonitorfenster, falls noch nicht geschehen
            If Globals.ThisAddIn.OffeneAnrMonWPF Is Nothing Then Globals.ThisAddIn.OffeneAnrMonWPF = New List(Of AnrMonWPF)

            ' Erstelle einen neues Popup
            PopUpAnrMonWPF = New AnrMonWPF

            With PopUpAnrMonWPF
                ' Übergib dieses Telefonat an das Viewmodel
                With CType(.DataContext, AnrMonViewModel)
                    ' Übergib den Dispatcher des Views an das Viewmodel
                    .Instance = PopUpAnrMonWPF.Dispatcher

                    ' Übergib dieses Telefonat an das Viewmodel
                    .AnrMonTelefonat = Me
                End With

                ' Zeige den Anrufmonitor an
                .Show()

                ' Anrufmonitor ausblenden einleiten, falls dies beim RING geschehen soll
                If XMLData.POptionen.CBAutoClose And Not XMLData.POptionen.CBAnrMonHideCONNECT Then
                    NLogger.Debug("Starte Timer für Ausblenden des Anrufmonitors beim RING...")
                    .StarteAusblendTimer(TimeSpan.FromSeconds(XMLData.POptionen.TBEnblDauer))
                End If
            End With

            AnrMonEingeblendet = True

            ' Füge dieses Anruffenster der Liste eingeblendeten Anrufmonitorfenster hinzu
            Globals.ThisAddIn.OffeneAnrMonWPF.Add(PopUpAnrMonWPF)
            ' Fügen den Ereignishandler hinzu, der das Event für 'Geschlossen' verarbeitet
            AddHandler PopUpAnrMonWPF.Geschlossen, AddressOf PopupAnrMonGeschlossen

        End If
    End Sub

    Private Sub PopupAnrMonGeschlossen(sender As Object, e As EventArgs)

        AnrMonEingeblendet = False

        ' Entferne den Anrufmonitor von der Liste der offenen Popups
        Globals.ThisAddIn.OffeneAnrMonWPF.Remove(PopUpAnrMonWPF)
        NLogger.Debug($"Anruffenster geschlossen: {NameGegenstelle}: Noch {Globals.ThisAddIn.OffeneAnrMonWPF.Count} offene Anrufmonitor")

        PopUpAnrMonWPF = Nothing
    End Sub

    Private Async Sub ShowAnrMon()

        ' Erstelle die Liste der aktuell eingeblendeten Anrufmonitorfenster, falls noch nicht geschehen
        If Globals.ThisAddIn.OffeneAnrMonWPF Is Nothing Then Globals.ThisAddIn.OffeneAnrMonWPF = New List(Of AnrMonWPF)

        ' Prüfe, ob bereits ein Anrufmonitor für diese eingehende Telefonnummer eingeblendet ist
        ' Etwas kompliziert, da die Formulare in jeweils eigenen Threads vorgehalten werden
        ' Die Gegenstellennummer und die Eigene Nummer müssen identisch sein.
        Dim AnrMonList = Globals.ThisAddIn.OffeneAnrMonWPF.Select(Function(AM) AM.Dispatcher.Invoke(
                                                                  Function() CType(AM.DataContext, AnrMonViewModel).AnrMonTelefonat)).Where(
                                                                  Function(T) T.GegenstelleTelNr.Equals(GegenstelleTelNr) And T.EigeneTelNr.Equals(EigeneTelNr))

        If XMLData.POptionen.CBAnrMonHideMultipleCall AndAlso (Not XMLData.POptionen.CBAutoClose Or XMLData.POptionen.CBAnrMonHideCONNECT) AndAlso AnrMonList.Any Then

            Try
                ' Aktualisiere den bestehenden Anrufmonitor 
                NLogger.Debug($"Es ist bereits {AnrMonList.Count} Anrufmonitor für die Nummer {GegenstelleTelNr.Unformatiert} vorhanden.")

                With AnrMonList.First
                    ' Setze den Zähler hoch
                    .AnzahlAnrufe += 1
                    ' Aktualisiere die Zeit
                    .ZeitBeginn = ZeitBeginn

                End With
            Catch ex As Exception
                NLogger.Warn(ex, "Bestehender Anrufmonitor wird erneut eingeblendet")
                ShowAnrMon()
            End Try

        Else
            Await StartSTATask(Function() As Boolean
                                   If PopUpAnrMonWPF Is Nothing Then
                                       NLogger.Debug("Blende einen neuen Anrufmonitor ein")
                                       ' Blende einen neuen Anrufmonitor ein
                                       AnrMonEinblenden()

                                       While AnrMonEingeblendet
                                           AnrMonControl()

                                           Forms.Application.DoEvents()
                                           Thread.Sleep(100)
                                       End While
                                   End If
                                   Return False
                               End Function)
        End If

        AnrMonList = Nothing
    End Sub

    Private Sub AnrMonControl()

        ' AnrMonStartHideTimer wird durch die CONNECT auf True gesetzt. Sobald das der Fall ist, wird der AusblendTimer gestartet. 
        If AnrMonStartHideTimer Then
            NLogger.Debug("Starte Timer für Ausblenden des Anrufmonitors beim CONNECT...")
            PopUpAnrMonWPF?.StarteAusblendTimer(TimeSpan.FromSeconds(XMLData.POptionen.TBEnblDauer))

            ' Setze AnrMonStartHideTimer, damit der Timer nur einmal gestartet wird.
            AnrMonStartHideTimer = False
        End If

    End Sub

#End Region

#Region "Stoppuhr-Fenster"
    Friend Sub StoppUhrEinblenden()
        If Not StoppUhrEingeblendet Then
            ' Erstelle die Liste der aktuell eingeblendeten Anrufmonitorfenster, falls noch nicht geschehen
            If Globals.ThisAddIn.OffeneStoppUhrWPF Is Nothing Then Globals.ThisAddIn.OffeneStoppUhrWPF = New List(Of StoppUhrWPF)

            ' Erstelle einen neues Popup
            PopupStoppUhrWPF = New StoppUhrWPF

            ' Merke den aktuell offenen Inspektor
            KeepoInspActivated(False)

            With PopupStoppUhrWPF
                ' Übergib dieses Telefonat an das Viewmodel
                With CType(.DataContext, StoppUhrViewModel)
                    ' Übergib dieses Telefonat an das Viewmodel
                    .StoppUhrTelefonat = Me
                End With
                ' Zeige den ANrufmonitor an
                .Show()
            End With

            StoppUhrEingeblendet = True

            ' Füge dieses Anruffenster der Liste eingeblendeten Anrufmonitorfenster hinzu
            Globals.ThisAddIn.OffeneStoppUhrWPF.Add(PopupStoppUhrWPF)

            ' Fügen den Ereignishandler hinzu, der das Event für 'Geschlossen' verarbeitet
            AddHandler PopupStoppUhrWPF.Geschlossen, AddressOf PopupStoppUhrGeschlossen

            KeepoInspActivated(True)
        End If
    End Sub

    Private Sub PopupStoppUhrGeschlossen(sender As Object, e As EventArgs)

        StoppUhrEingeblendet = False
        ' Entferne die Stoppuhr von der Liste der offenen Popups
        Globals.ThisAddIn.OffeneStoppUhrWPF.Remove(PopupStoppUhrWPF)
        NLogger.Debug($"Stoppuhr geschlossen: {NameGegenstelle}: Noch {Globals.ThisAddIn.OffeneStoppUhrWPF.Count} offene Stoppuhren")

        PopupStoppUhrWPF = Nothing
    End Sub

    Private Async Sub ShowStoppUhr()

        Await StartSTATask(Function() As Boolean
                               If PopupStoppUhrWPF Is Nothing Then
                                   NLogger.Debug("Blende einen neue StoppUhr ein")
                                   ' Blende einen neuen Anrufmonitor ein
                                   StoppUhrEinblenden()

                                   While StoppUhrEingeblendet
                                       Forms.Application.DoEvents()
                                       Thread.Sleep(100)
                                   End While
                               End If
                               Return False
                           End Function)
    End Sub

#End Region

#End Region

#Region "Equals"
    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Telefonat))
    End Function

    Public Overloads Function Equals(other As Telefonat) As Boolean Implements IEquatable(Of Telefonat).Equals

        ' Starte den Vergleich nur, wenn das andere Telefonat nicht Nothing ist
        If other IsNot Nothing Then

            ' Es kann sein, dass Eigene Telefonnummer Nothing ist. Tritt bei den Wahlwiederholungslisten auf.
            If EigeneTelNr Is Nothing AndAlso OutEigeneTelNr.IsNotStringNothingOrEmpty Then
                ' Setze die eigene Nummer
                EigeneTelNr = New Telefonnummer With {.SetNummer = OutEigeneTelNr}
            End If

            ' Die Telefonnummern und die Zeiten müssen grundsätzlich gleich sein
            If EigeneTelNr IsNot Nothing AndAlso
               EigeneTelNr.Equals(other.EigeneTelNr) AndAlso GegenstelleTelNr.Equals(other.GegenstelleTelNr) AndAlso
               ZeitBeginn.IsSameAs(other.ZeitBeginn) AndAlso ZeitEnde.IsSameAs(other.ZeitEnde) Then

                ' Falls beide Telefonate importiert wurden, dann vergleiche zusätzlich die IDs. 
                ' Bei Live-Telefonaten kommt die ID aus dem Anrufmonitor und entspricht der Anzahl aktuell geführter Telefonate (-1: 0-10)
                If Import And other.Import Then
                    Return ID.AreEqual(other.ID)
                Else
                    Return True
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ReleaseComObject(OlKontakt)
            OlKontakt = Nothing
            ' nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    Protected Overrides Sub Finalize()
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class