Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Xml.Serialization
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

    <XmlIgnore> Friend ReadOnly Property AnrMonExInfo As String
        Get
            If Firma.IsNotStringNothingOrEmpty Then
                ' Gib die Firmeninformation zurück
                Return Firma
            Else
                If Not GegenstelleTelNr.Unterdrückt Then
                    Return $"{GegenstelleTelNr.Location}" & If(GegenstelleTelNr.IstInland, DfltStringEmpty, $" ({Localize.Länder.ResourceManager.GetString(GegenstelleTelNr.AreaCode)})")
                Else
                    ' Gib ein leeren String zurück
                    Return DfltStringEmpty
                End If
            End If
        End Get
    End Property

#End Region

#Region "Boolean"
    Private _Beendet As Boolean
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

    Private _Angenommen As Boolean
    <XmlAttribute> Public Property Angenommen As Boolean
        Get
            Return _Angenommen
        End Get
        Set
            SetProperty(_Angenommen, Value)
        End Set
    End Property

    Private _Abgelehnt As Boolean
    <XmlAttribute> Public Property Blockiert As Boolean
        Get
            Return _Abgelehnt
        End Get
        Set
            SetProperty(_Abgelehnt, Value)
        End Set
    End Property
    <XmlIgnore> Friend ReadOnly Property AnruferUnbekannt As Boolean
        Get
            Return OlKontakt Is Nothing And FBTelBookKontakt Is Nothing
        End Get
    End Property

    ''' <summary>
    ''' Gibt an, ob die Daten zu dem Telefonat nachträglich aus der Anrufliste importiert werden.
    ''' </summary>
    <XmlIgnore> Friend Property Import As Boolean = False
    <XmlIgnore> Friend Property AnruferErmittelt As Boolean = False
    <XmlIgnore> Friend Property AnrMonEingeblendet As Boolean = False
    <XmlIgnore> Friend Property StoppUhrEingeblendet As Boolean = False
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

    Private _FBTelBookKontakt As FBoxAPI.Contact
    <XmlElement> Public Property FBTelBookKontakt As FBoxAPI.Contact
        Get
            Return _FBTelBookKontakt
        End Get
        Set
            SetProperty(_FBTelBookKontakt, Value)
        End Set
    End Property

    Private _OlKontakt As Outlook.ContactItem
    <XmlIgnore> Friend Property OlKontakt() As Outlook.ContactItem
        Get
            ' Ermittle den Outlook-Kontakt, falls dies noch nicht geschehen ist
            If _OlKontakt Is Nothing AndAlso (OutlookKontaktID.IsNotStringEmpty And OutlookStoreID.IsNotStringEmpty) Then
                _OlKontakt = GetOutlookKontakt(OutlookKontaktID, OutlookStoreID)
                NLogger.Debug($"Outlook Kontakt {_OlKontakt?.FullNameAndCompany} aus EntryID und KontaktID ermittelt.")
            End If

            Return _OlKontakt
        End Get
        Set
            SetProperty(_OlKontakt, Value)
        End Set
    End Property

    <XmlElement> Public ReadOnly Property NameGegenstelle As String
        Get
            Return If(AnruferName.IsNotStringNothingOrEmpty, AnruferName, If(NrUnterdrückt, Localize.LocAnrMon.strNrUnterdrückt, GegenstelleTelNr.Formatiert))
        End Get
    End Property

    <XmlIgnore> Private Property PopUpAnrMonWPF As AnrMonWPF

    <XmlIgnore> Private Property PopupStoppUhrWPF As StoppUhrWPF

    Private _TellowsErgebnis As TellowsResponse
    <XmlElement> Public Property TellowsErgebnis() As TellowsResponse
        Get
            Return _TellowsErgebnis
        End Get
        Set
            SetProperty(_TellowsErgebnis, Value)
        End Set
    End Property

    Private _ScoreListEntry As TellowsScoreListEntry
    <XmlElement> Public Property ScoreListEntry() As TellowsScoreListEntry
        Get
            Return _ScoreListEntry
        End Get
        Set
            SetProperty(_ScoreListEntry, Value)
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
                            EigeneTelNr = New Telefonnummer With {.SetNummer = FBStatus(i), .EigeneNummer = True}
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

                    Case 4 ' Eingehende (anrufende) Telefonnummer
                        EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr(FBStatus(i))
                        ' Wert für Serialisierung in separater Eigenschaft ablegen
                        If EigeneTelNr Is Nothing Then
                            NLogger.Warn($"Eigene Telefonnummer für {FBStatus(i)} konnte nicht ermittelt werden.")
                            EigeneTelNr = New Telefonnummer With {.SetNummer = FBStatus(i), .EigeneNummer = True}
                        End If

                        OutEigeneTelNr = EigeneTelNr.Unformatiert

                    Case 5 ' Gewählte (ausgehende) Telefonnummer
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
                        GegenstelleTelNr = New Telefonnummer With {.SetNummer = FBStatus(i)}
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

                    If ThisAddIn.PhoneBookXML Is Nothing Then 'OrElse ThisAddIn.PhoneBookXML.NurHeaderDaten Then
                        ' Wenn die Telefonbücher noch nicht heruntergeladen wurden, oder nur die Namen bekannt sind (Header-Daten),
                        ' Dann lade die Telefonbücher herunter
                        NLogger.Debug($"Die Telefonbücher sind für die Kontaktsuche nicht bereit. Beginne sie herunterzuladen...")
                        Using FBoxTR064 = New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
                            ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeTelefonbücher(FBoxTR064)
                        End Using
                    End If

                    ' Wenn die Telefonbücher immer noch nicht zur Verfügung stehen, brich an dieser Stelle ab
                    If ThisAddIn.PhoneBookXML IsNot Nothing Then 'AndAlso Not ThisAddIn.PhoneBookXML.NurHeaderDaten Then
                        FBTelBookKontakt = Telefonbücher.Find(ThisAddIn.PhoneBookXML, GegenstelleTelNr)
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
                            TellowsErgebnis = Await tellows.GetTellowsLiveAPIData(GegenstelleTelNr)
                            If TellowsErgebnis IsNot Nothing Then

                                If TellowsErgebnis.Score.AreEqual(5) And TellowsErgebnis.Comments.IsLess(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                                    ' Verwirf das Ergebnis. Es gibt keinen Eintrag bei tellows
                                    TellowsErgebnis = Nothing
                                    NLogger.Debug($"Kein Eintrag bei tellows für Telefonnummer '{GegenstelleTelNr.TellowsNummer}' gefunden.")
                                Else
                                    ' Verarbeite das Tellows Ergebnis
                                    With TellowsErgebnis

                                        AnruferName = String.Join(", ", .CallerNames)
                                        Firma = String.Join(", ", .CallerTypes.Select(Function(CT) $"{CT.Name} ({CT.Count})"))

                                        NLogger.Debug($"Eintrag bei tellows für Telefonnummer '{GegenstelleTelNr.TellowsNummer}' mit Score { .Score} gefunden.")

                                        If XMLData.POptionen.CBTellowsAutoFBBlockList AndAlso .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAutoScoreFBBlockList) Then
                                            ' Sperrlisteintrag erzeugen
                                            AddToCallBarring(New List(Of String) From { .Number}, AnruferName)

                                        End If

                                    End With

                                    AnruferErmittelt = True
                                End If

                            End If
                        End Using
                    Else
                        ' Auswertung bei importieren Anrufen via CallListAPI
                        If ThisAddIn.TellowsScoreList IsNot Nothing Then
                            ScoreListEntry = ThisAddIn.TellowsScoreList.Find(Function(Eintrag) GegenstelleTelNr.Equals(Eintrag.Number))
                            If ScoreListEntry IsNot Nothing Then
                                With ScoreListEntry
                                    NLogger.Debug($"Eintrag in der tellows-ScoreList für Telefonnummer '{GegenstelleTelNr.TellowsNummer}' mit Score { .Score} gefunden.")

                                    AnruferName = .CallerName
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

                    If VCard.IsNotStringEmpty Then

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

        If OlKontakt Is Nothing AndAlso OutlookKontaktID.IsNotStringNothingOrEmpty And OutlookStoreID.IsNotStringNothingOrEmpty Then
            ' Verknüpfe den Kontakt
            OlKontakt = GetOutlookKontakt(OutlookKontaktID, OutlookStoreID)
        End If

        If OlKontakt Is Nothing AndAlso FBTelBookKontakt IsNot Nothing Then
            OlKontakt = ErstelleKontakt(FBTelBookKontakt, GegenstelleTelNr, False)
        End If

        If OlKontakt Is Nothing Then
            If VCard.IsNotStringNothingOrEmpty Then
                ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                OlKontakt = ErstelleKontakt(GegenstelleTelNr, False)
            Else
                'vCard gefunden
                OlKontakt = ErstelleKontakt(VCard, GegenstelleTelNr, False)
            End If
        End If

        If OlKontakt IsNot Nothing Then OlKontakt.Display()

    End Sub

    ''' <summary>
    ''' Ruft die Gegenstellentelefonnummer an
    ''' </summary>
    Friend Sub Rückruf()
        Dim WählClient As New FritzBoxWählClient
        WählClient.WählboxStart(Me)
    End Sub

    Friend Sub ErstelleJournalEintrag()

        Dim OutlookApp As Outlook.Application = ThisAddIn.OutookApplication
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim olJournalFolder As OutlookOrdner

        If OutlookApp IsNot Nothing Then
            ' Journalimport nur dann, wenn Nummer überwacht wird
            If XMLData.POptionen.CBJournal And EigeneTelNr.Überwacht Then

                ' Erzeuge Journaleinträge für blockierte Anrufe nur, wenn es der Nutzer will
                If Not Blockiert OrElse XMLData.POptionen.CBJournalBlockNr Then
                    Try
                        ' Erstelle ein Journaleintrag im STandard-Ordner.
                        olJournal = CType(OutlookApp.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
                    Catch ex As Exception
                        NLogger.Error(ex)
                    End Try

                    If olJournal IsNot Nothing Then
                        Dim tmpSubject As String

                        If Not Blockiert Then
                            If Angenommen Then
                                tmpSubject = If(AnrufRichtung = AnrufRichtungen.Ausgehend, Localize.LocAnrMon.strJournalAusgehend, Localize.LocAnrMon.strJournalEingehend)
                            Else 'Verpasst
                                tmpSubject = If(AnrufRichtung = AnrufRichtungen.Ausgehend, Localize.LocAnrMon.strJournalNichterfolgreich, Localize.LocAnrMon.strJournalVerpasst)
                            End If
                        Else
                            tmpSubject = Localize.LocAnrMon.strJournalBlockiert
                        End If

                        With olJournal

                            .Subject = $"{tmpSubject} {AnruferName}{If(NrUnterdrückt, DfltStringEmpty, If(AnruferName.IsStringNothingOrEmpty, GegenstelleTelNr.Formatiert, $" ({GegenstelleTelNr.Formatiert})"))}"
                            .Duration = Dauer.GetLarger(31) \ 60
                            .Body = $"{Localize.LocAnrMon.strJournalBodyStart} {If(NrUnterdrückt, Localize.LocAnrMon.strNrUnterdrückt, GegenstelleTelNr.Formatiert)}{Dflt1NeueZeile}Status: {If(Angenommen, DfltStringEmpty, "nicht ")}angenommen{Dflt2NeueZeile}{VCard}"
                            .Start = ZeitBeginn
                            .Companies = Firma

                            ' Bei verpassten Anrufen ist TelGerät ggf. leer
                            .Categories = $"{If(TelGerät Is Nothing, Localize.LocAnrMon.strJournalCatVerpasst, TelGerät.Name)};{String.Join("; ", DfltJournalDefCategories.ToArray)}"

                            ' Speichern der EntryID und StoreID in Benutzerdefinierten Feldern
                            If OlKontakt IsNot Nothing Then

                                Dim colArgs(1) As Object
                                colArgs(0) = OlKontakt.EntryID
                                colArgs(1) = OlKontakt.StoreID

                                For i As Integer = 0 To 1
                                    .PropertyAccessor.SetProperty(DASLTagJournal(i).ToString, colArgs(i))
                                Next

                                ' Funktioniert aus irgendeinem dummen Grund nicht. Die EntryID wird nicht übertragen.
                                '.PropertyAccessor.SetProperties(DASLTagJournal, colArgs)
                            End If

                            ' Speicherort wählen
                            olJournalFolder = XMLData.POptionen.OutlookOrdner.Find(OutlookOrdnerVerwendung.JournalSpeichern)

                            If olJournalFolder IsNot Nothing AndAlso olJournalFolder.MAPIFolder IsNot Nothing AndAlso
                            Not olJournalFolder.Equals(OutlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderJournal)) Then
                                ' Verschiebe den Journaleintrag in den ausgewählten Ordner
                                ' Damit wird der Kontakt gleichzeitig im Zielordner gespeichert.
                                .Move(olJournalFolder.MAPIFolder)
                                ' Verwerfe diesen Journaleintrag
                                .Close(Outlook.OlInspectorClose.olDiscard)

                                NLogger.Info($"Journaleintrag im Ordner {olJournalFolder.Name} (Store: {olJournalFolder.MAPIFolder.Store.DisplayName}) erstellt: { .Start}, { .Subject}, { .Duration}")
                            Else
                                ' Speicher den Journaleintrag im Standard-Ordner
                                .Close(Outlook.OlInspectorClose.olSave)
                                With OutlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderJournal)
                                    NLogger.Info($"Journaleintrag im Standardordner { .Name} (Store: { .Store.DisplayName}) erstellt: { olJournal.Start}, { olJournal.Subject}, { olJournal.Duration}")
                                End With
                            End If

                        End With

                        ReleaseComObject(olJournal)
                    End If
                Else
                    NLogger.Info($"Journaleintrag für blockierte Nummer '{NameGegenstelle}' nicht erzeugt.")
                End If
            End If
        Else
            NLogger.Info(Localize.LocAnrMon.strJournalFehler)
        End If
    End Sub

    Friend Sub UpdateRingCallList()

        If XMLData.POptionen.CBAnrListeUpdateCallLists Then
            If Not Blockiert OrElse XMLData.POptionen.CBJournalBlockNr Then
                If AnrufRichtung = AnrufRichtungen.Eingehend Then
                    ' RING-Liste initialisieren, falls erforderlich
                    If XMLData.PTelListen.RINGListe Is Nothing Then XMLData.PTelListen.RINGListe = New List(Of Telefonat)
                    ' Eintrag anfügen
                    XMLData.PTelListen.RINGListe.Insert(Me)
                Else
                    ' CALL-Liste initialisieren, falls erforderlich
                    If XMLData.PTelListen.CALLListe Is Nothing Then XMLData.PTelListen.CALLListe = New List(Of Telefonat)
                    ' Eintrag anfügen
                    XMLData.PTelListen.CALLListe.Insert(Me)
                End If
            End If
        End If
    End Sub

#Region "Anrufmonitor"
    Private Sub AnrMonRING()
        Angenommen = False
        Beendet = False

        ' Starte die Kontaktsuche mit Hilfe asynchroner Routinen, da ansonsten der Anrufmonitor erst eingeblendet wird, wenn der Kontakt ermittelt wurde
        ' Anrufername aus Kontakten und Rückwärtssuche ermitteln
        KontaktSuche()

        ' Anrufmonitor einblenden, wenn Bedingungen erfüllt 
        If EigeneTelNr.Überwacht Then ShowAnrMon()

        ' RING-Liste initialisieren, falls erforderlich
        If XMLData.PTelListen.RINGListe Is Nothing Then XMLData.PTelListen.RINGListe = New List(Of Telefonat)

        ' Telefonat in erste Positon der RING-Liste speichern
        XMLData.PTelListen.RINGListe.Insert(Me)

    End Sub

    Private Sub AnrMonCALL()
        Angenommen = False
        Beendet = False

        ' Anrufername aus Kontakten und Rückwärtssuche ermitteln
        KontaktSuche()

        ' Telefoniegerät ermitteln
        TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID.AreEqual(NebenstellenNummer))

        ' Eigene Nummer prüfen. Mittels Wählpräfix (*10X#, *11X#, *12X#) kann die ausgehende Telefonnummer beeinflusst werden. 
        ' Der Anrufmonitor gibt jedoch weiterhin die eigentlich genutzte eigene Nummer für diese Nebenstelle wieder.
        ' Unterschieden werden kann nur per AnschlussID
        ' 03.03.21 15:48:18;CALL;1;4;123456;0049987654321#;SIP3; *124# 654321
        ' 03.03.21 16:35:54;CALL;1;4;123456;0049987654321#;SIP1; *122# 123456

        ' Bei Rufumleitungen im Modus Automatisch wird die umgeleitete eingehende Nummer als eigene rausgehende Nummer verwendet.
        ' TODO: Anfangen von Rufumleitungen.
        If AnschlussID.IsNotStringNothingOrEmpty Then
            Dim tmpTel As Telefonnummer = XMLData.PTelefonie.GetEigeneTelNr(AnschlussID)
            If Not EigeneTelNr.Equals(tmpTel) Then
                ' Eintrag ins Log
                NLogger.Debug($"Eigene Telefonnummer '{EigeneTelNr.Unformatiert}' mit '{tmpTel.Unformatiert}' überschrieben ({AnschlussID}).")

                ' Telefonnummer ersetzen
                EigeneTelNr = tmpTel
            End If
        End If

        ' CALL-Liste initialisieren, falls erforderlich
        If XMLData.PTelListen.CALLListe Is Nothing Then XMLData.PTelListen.CALLListe = New List(Of Telefonat)

        ' Telefonat in erste Positon der CALL-Liste speichern
        XMLData.PTelListen.CALLListe.Insert(Me)

    End Sub

    Private Sub AnrMonCONNECT()
        Angenommen = True

        ' Telefoniegerät ermitteln
        TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID.AreEqual(NebenstellenNummer))

        ' Stoppuhr einblenden, wenn Bedingungen erfüllt 
        If XMLData.POptionen.CBStoppUhrEinblenden Then ShowStoppUhr()

    End Sub

    Private Sub AnrMonDISCONNECT()
        Beendet = True

        ' Stoppuhr ausblenden, wenn dies in den Einstellungen gesetzt ist
        If StoppUhrEingeblendet And XMLData.POptionen.CBStoppUhrAusblenden Then PopupStoppUhrWPF.StarteAusblendTimer()

        If XMLData.POptionen.CBJournal Then ErstelleJournalEintrag()
    End Sub

    Friend Sub AnrMonEinblenden()
        ' Zeige den Anrufmonitor nur an, wenn gerade nicht schon eingeblendet.
        If Not AnrMonEingeblendet Then

            ' Erstelle die Liste der aktuell eingeblendeten Anrufmonitorfenster, falls noch nicht geschehen
            If ThisAddIn.OffeneAnrMonWPF Is Nothing Then ThisAddIn.OffeneAnrMonWPF = New List(Of AnrMonWPF)

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
            End With

            AnrMonEingeblendet = True

            ' Füge dieses Anruffenster der Liste eingeblendeten Anrufmonitorfenster hinzu
            ThisAddIn.OffeneAnrMonWPF.Add(PopUpAnrMonWPF)
            ' Fügen den Ereignishandler hinzu, der das Event für 'Geschlossen' verarbeitet
            AddHandler PopUpAnrMonWPF.Geschlossen, AddressOf PopupAnrMonGeschlossen

        End If
    End Sub

    Friend Sub StoppUhrEinblenden()
        If Not StoppUhrEingeblendet Then
            ' Erstelle die Liste der aktuell eingeblendeten Anrufmonitorfenster, falls noch nicht geschehen
            If ThisAddIn.OffeneStoppUhrWPF Is Nothing Then ThisAddIn.OffeneStoppUhrWPF = New List(Of StoppUhrWPF)

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
            ThisAddIn.OffeneStoppUhrWPF.Add(PopupStoppUhrWPF)

            ' Fügen den Ereignishandler hinzu, der das Event für 'Geschlossen' verarbeitet
            AddHandler PopupStoppUhrWPF.Geschlossen, AddressOf PopupStoppUhrGeschlossen

            KeepoInspActivated(True)
        End If
    End Sub

    Private Sub PopupAnrMonGeschlossen(sender As Object, e As EventArgs)

        AnrMonEingeblendet = False

        ' Entferne den Anrufmonitor von der Liste der offenen Popups
        ThisAddIn.OffeneAnrMonWPF.Remove(PopUpAnrMonWPF)
        NLogger.Debug($"Anruffenster geschlossen: {AnruferName}: Noch {ThisAddIn.OffeneAnrMonWPF.Count} offene Anrufmonitor")

        PopUpAnrMonWPF = Nothing
    End Sub

    Private Sub PopupStoppUhrGeschlossen(sender As Object, e As EventArgs)

        StoppUhrEingeblendet = False
        ' Entferne die Stoppuhr von der Liste der offenen Popups
        ThisAddIn.OffeneStoppUhrWPF.Remove(PopupStoppUhrWPF)
        NLogger.Debug($"Stoppuhr geschlossen: {AnruferName}: Noch {ThisAddIn.OffeneStoppUhrWPF.Count} offene Stoppuhren")

        PopupStoppUhrWPF = Nothing
    End Sub
    Public Function StartSTATask(Of T)(func As Func(Of T)) As Task(Of T)
        Dim tcs = New TaskCompletionSource(Of T)()
        Dim thread As New Thread(Sub()
                                     Try
                                         tcs.SetResult(func())
                                     Catch e As Exception
                                         tcs.SetException(e)
                                     End Try
                                 End Sub)
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        Return tcs.Task
    End Function

    Private Async Sub ShowAnrMon()

        Await StartSTATask(Function() As Boolean
                               If PopUpAnrMonWPF Is Nothing Then
                                   NLogger.Debug("Blende einen neuen Anrufmonitor ein")
                                   ' Blende einen neuen Anrufmonitor ein
                                   AnrMonEinblenden()

                                   While AnrMonEingeblendet
                                       Forms.Application.DoEvents()
                                       Thread.Sleep(100)
                                   End While
                               End If
                               Return False
                           End Function)
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

#Region "Equals, CompareTo"
    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Telefonat))
    End Function

    Public Overloads Function Equals(other As Telefonat) As Boolean Implements IEquatable(Of Telefonat).Equals
        Return other IsNot Nothing AndAlso
               EigeneTelNr Is other.EigeneTelNr AndAlso
               GegenstelleTelNr Is other.GegenstelleTelNr AndAlso
               ZeitBeginn = ZeitBeginn AndAlso
               ZeitEnde.CompareTo(other.ZeitEnde).IsZero
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


