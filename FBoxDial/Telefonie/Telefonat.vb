Imports System.Threading
Imports System.Windows
Imports System.Xml.Serialization
Imports Microsoft.Office.Interop
<Serializable()> Public Class Telefonat
    Inherits NotifyBase

    Implements IEquatable(Of Telefonat)
    Implements IDisposable

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"

    Private _ID As Integer
    <XmlAttribute> Public Property ID As Integer
        Get
            Return _ID
        End Get
        Set
            SetProperty(_ID, Value)
        End Set
    End Property

    Private _EigeneTelNr As Telefonnummer
    <XmlIgnore> Public Property EigeneTelNr As Telefonnummer
        Get
            Return _EigeneTelNr
        End Get
        Set
            SetProperty(_EigeneTelNr, Value)
        End Set
    End Property

    Private _OutEigeneTelNr As String
    <XmlElement> Public Property OutEigeneTelNr As String
        Get
            Return _OutEigeneTelNr
        End Get
        Set
            SetProperty(_OutEigeneTelNr, Value)
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

    <XmlIgnore> Public ReadOnly Property RINGGeräteString As String
        Get
            ' Funktioniert nicht!
            Return String.Join(", ", XMLData.PTelefonie.Telefoniegeräte.Where(Function(Tel)
                                                                                  Return Tel.StrEinTelNr.Contains(OutEigeneTelNr)
                                                                              End Function).Select(Function(Gerät) Gerät.Name).ToList())
        End Get
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

    Private _AnschlussID As String
    <XmlElement> Public Property AnschlussID As String
        Get
            Return _AnschlussID
        End Get
        Set
            SetProperty(_AnschlussID, Value)
        End Set
    End Property

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

    Private _FBTelBookKontakt As FritzBoxXMLKontakt
    <XmlElement> Public Property FBTelBookKontakt As FritzBoxXMLKontakt
        Get
            Return _FBTelBookKontakt
        End Get
        Set
            SetProperty(_FBTelBookKontakt, Value)
        End Set
    End Property

    Private _AnruferName As String
    <XmlElement> Public Property AnruferName As String
        Get
            Return _AnruferName
        End Get
        Set
            SetProperty(_AnruferName, Value)
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

    Private _OlKontakt As Outlook.ContactItem
    <XmlIgnore> Friend Property OlKontakt() As Outlook.ContactItem
        Get
            ' Ermittle den Outlook-Kontakt, falls dies noch nicht geschehen ist
            If _OlKontakt Is Nothing AndAlso (OutlookKontaktID.IsNotStringEmpty And OutlookStoreID.IsNotStringEmpty) Then
                _OlKontakt = GetOutlookKontakt(OutlookKontaktID, OutlookStoreID)
                NLogger.Debug($"Outlook Kontakt {_OlKontakt.FullNameAndCompany} aus EntryID und KontaktID ermittelt.")
            End If

            Return _OlKontakt
        End Get
        Set
            SetProperty(_OlKontakt, Value)
        End Set
    End Property
    '<XmlIgnore> Friend Property AnrMonPopUp As Popup
    <XmlIgnore> Friend Property AnrMonEingeblendet As Boolean = False
    <XmlIgnore> Friend Property StoppUhrEingeblendet As Boolean = False

    <XmlIgnore> Private Property PopUpAnrMonWPF As AnrMonWPF
    <XmlIgnore> Private Property PopupStoppUhrWPF As StoppUhrWPF


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
                        'GegenstelleTelNr = New Telefonnummer With {.SetNummer = "(03 52 08) 8 59-0"}

                        NrUnterdrückt = GegenstelleTelNr.Unbekannt

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
        Const Eingehend As Integer = 0
        Const Ausgehend As Integer = 1
    End Structure
#End Region

    Sub New()
        'Stop
    End Sub

#Region "Kontaktsuche"
    Friend Async Sub StarteKontaktsuche()

        ' Kontaktsuche in den Outlook-Kontakten
        OlKontakt = KontaktSuche(GegenstelleTelNr)

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
            End With
        End If

        ' Kontaktsuche in den Fritz!Box Telefonbüchern
        If XMLData.POptionen.CBKontaktSucheFritzBox Then
            If OlKontakt Is Nothing Then
                If ThisAddIn.PhoneBookXML IsNot Nothing Then
                    FBTelBookKontakt = ThisAddIn.PhoneBookXML.Find(GegenstelleTelNr)
                End If
            End If

            If FBTelBookKontakt IsNot Nothing Then
                If XMLData.POptionen.CBKErstellen Then
                    OlKontakt = ErstelleKontakt(OutlookKontaktID, OutlookStoreID, FBTelBookKontakt, GegenstelleTelNr, True)

                    With OlKontakt
                        AnruferName = .FullName
                        Firma = .CompanyName
                    End With
                Else
                    AnruferName = FBTelBookKontakt.Person.RealName
                End If
            End If
        End If

        ' Kontaktsuche über die Rückwärtssuche
        If FBTelBookKontakt Is Nothing And OlKontakt Is Nothing Then

            ' Eine Rückwärtssuche braucht nur dann gemacht werden, wennd die Länge der Telefonnummer aussreichend ist.
            ' Ggf. muss der Wert angepasst werden.
            If GegenstelleTelNr.Unformatiert.Length.IsLargerOrEqual(4) Then

                If XMLData.POptionen.CBRWS Then
                    VCard = Await StartRWS(GegenstelleTelNr, XMLData.POptionen.CBRWSIndex)

                    If VCard.IsNotStringEmpty Then
                        NLogger.Info($"Rückwärtssuche erfolgreich: {VCard}")
                        If XMLData.POptionen.CBKErstellen Then
                            OlKontakt = ErstelleKontakt(OutlookKontaktID, OutlookStoreID, VCard, GegenstelleTelNr, True)
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
                    End If
                End If
            End If
        End If

    End Sub
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

        If OlKontakt Is Nothing Then
            If VCard.IsNotStringNothingOrEmpty Then
                ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                OlKontakt = ErstelleKontakt(GegenstelleTelNr, False)
            Else
                'vCard gefunden
                OlKontakt = ErstelleKontakt(DfltStringEmpty, DfltStringEmpty, VCard, GegenstelleTelNr, False)
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
                Try
                    olJournal = CType(OutlookApp.CreateItem(Outlook.OlItemType.olJournalItem), Outlook.JournalItem)
                Catch ex As Exception
                    NLogger.Error(ex)
                End Try

                If olJournal IsNot Nothing Then
                    Dim tmpSubject As String

                    If Angenommen Then
                        If AnrufRichtung = AnrufRichtungen.Ausgehend Then
                            tmpSubject = DfltJournalTextAusgehend
                        Else
                            tmpSubject = DfltJournalTextEingehend
                        End If
                    Else 'Verpasst
                        If AnrufRichtung = AnrufRichtungen.Ausgehend Then
                            tmpSubject = DfltJournalTextNichtErfolgreich
                        Else
                            tmpSubject = DfltJournalTextVerpasst
                        End If
                    End If

                    With olJournal

                        .Subject = $"{tmpSubject} {AnruferName}{If(NrUnterdrückt, DfltStringEmpty, If(AnruferName.IsStringNothingOrEmpty, GegenstelleTelNr.Formatiert, String.Format(" ({0})", GegenstelleTelNr.Formatiert)))}"
                        .Duration = Dauer.GetLarger(31) \ 60
                        .Body = DfltJournalBody(If(NrUnterdrückt, DfltStringUnbekannt, GegenstelleTelNr.Formatiert), Angenommen, VCard)
                        .Start = ZeitBeginn
                        .Companies = Firma

                        ' Bei verpassten Anrufen ist TelGerät ggf. leer
                        .Categories = $"{If(TelGerät Is Nothing, "Verpasst", TelGerät.Name)};{String.Join("; ", DfltJournalDefCategories.ToArray)}"

                        ' Testweise: Speichern der EntryID und StoreID in Benutzerdefinierten Feldern
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
                        If olJournalFolder IsNot Nothing AndAlso olJournalFolder.MAPIFolder IsNot Nothing Then
                            .Move(olJournalFolder.MAPIFolder)
                            .Close(Outlook.OlInspectorClose.olDiscard)
                        Else
                            .Close(Outlook.OlInspectorClose.olSave)
                            '.Move(OutlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderJournal))
                        End If

                        NLogger.Info("Journaleintrag erstellt: {0}, {1}, {2}", .Start, .Subject, .Duration)

                    End With

                    olJournal.ReleaseComObject
                    ' Merke die Zeit
                    If XMLData.POptionen.LetzterJournalEintrag < Now Then XMLData.POptionen.LetzterJournalEintrag = Now
                    ' Merke die ID
                    XMLData.POptionen.LetzterJournalEintragID = Math.Max(XMLData.POptionen.LetzterJournalEintragID, ID)
                End If
            End If
        Else
            NLogger.Info(DfltJournalFehler)
        End If
    End Sub

#Region "Anrufmonitor"
    Private Sub AnrMonRING()
        Angenommen = False
        Beendet = False

        ' Starte die Kontaktsuche mit Hilfe eines Backgroundworkers, da ansonsten der Anrufmonitor erst eingeblendet wird, wenn der Kontakt ermittelt wurde
        ' Anrufername aus Kontakten und Rückwärtssuche ermitteln
        StarteKontaktsuche()

        '' Ermitteln der Gerätenammen der Telefone, die auf diese eigene Nummer reagieren
        'If RINGGeräte Is Nothing Then RINGGeräte = New ObservableCollectionEx(Of Telefoniegerät)
        'RINGGeräte.AddRange(XMLData.PTelefonie.Telefoniegeräte.Where(Function(Tel) Tel.StrEinTelNr IsNot Nothing AndAlso Tel.StrEinTelNr.Contains(EigeneTelNr.Unformatiert)))

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
        StarteKontaktsuche()

        ' Telefoniegerät ermitteln
        TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID.AreEqual(NebenstellenNummer))

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
        If StoppUhrEingeblendet And XMLData.POptionen.CBStoppUhrAusblenden Then
            PopupStoppUhrWPF.StarteAusblendTimer(XMLData.POptionen.TBStoppUhrAusblendverzögerung)
        End If

        If XMLData.POptionen.CBJournal Then ErstelleJournalEintrag()
    End Sub

    ''' <summary>
    ''' Das Closed-Event wird zweimal aufgerufen (Dispatcher.Invoke). Zählvariable zum Triggern der Aufrufe.
    ''' </summary>
    <XmlIgnore> Private Property IAnrMonClosed As Integer
    <XmlIgnore> Private Property StoppUhrClosed As Integer
    Friend Sub AnrMonEinblenden()

        ' Erstelle die Liste der aktuell eingeblendeten Anrufmonitorfenster, falls noch nicht geschehen
        If ThisAddIn.OffeneAnrMonWPF Is Nothing Then ThisAddIn.OffeneAnrMonWPF = New List(Of AnrMonWPF)

        ' Erstelle einen neues Popup
        PopUpAnrMonWPF = New AnrMonWPF

        ' Merke den aktuell offenen Inspektor
        KeepoInspActivated(False)

        With PopUpAnrMonWPF
            ' Übergib dieses Telefonat an das Viewmodel
            With CType(.DataContext, AnrMonViewModel)
                ' Übergib dieses Telefonat an das Viewmodel
                .AnrMonTelefonat = Me
            End With
            ' Zeige den ANrufmonitor an
            .Show()
        End With
        ' Zählvariable zum schließen des Anrufmonitors
        IAnrMonClosed = 0

        AnrMonEingeblendet = True

        ' Füge dieses Anruffenster der Liste eingeblendeten Anrufmonitorfenster hinzu
        ThisAddIn.OffeneAnrMonWPF.Add(PopUpAnrMonWPF)

        ' Fügen den Ereignishandler hinzu, der das Event für 'Geschlossen' verarbeitet
        AddHandler PopUpAnrMonWPF.Geschlossen, AddressOf PopupAnrMonGeschlossen

        KeepoInspActivated(True)
    End Sub

    Friend Sub StoppUhrEinblenden()

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
        ' Zählvariable zum schließen des Anrufmonitors
        StoppUhrClosed = 0

        StoppUhrEingeblendet = True

        ' Füge dieses Anruffenster der Liste eingeblendeten Anrufmonitorfenster hinzu
        ThisAddIn.OffeneStoppUhrWPF.Add(PopupStoppUhrWPF)

        ' Fügen den Ereignishandler hinzu, der das Event für 'Geschlossen' verarbeitet
        AddHandler PopupStoppUhrWPF.Geschlossen, AddressOf PopupStoppUhrGeschlossen

        KeepoInspActivated(True)


    End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des <see cref="PopUpAnrMonWPF"/> aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' Das Closed-Event wird zweimal aufgerufen (Dispatcher.Invoke). Zählvariable zum Triggern der Aufrufe.
    ''' </summary>
    Private Sub PopupAnrMonGeschlossen(sender As Object, e As EventArgs)
        ' Führe die Arbeiten nur beim ersten Aufruf des Closed-Event des Popups durch.
        If IAnrMonClosed.IsZero Then
            NLogger.Debug("Anruffenster geschlossen: {0}", AnruferName)
            IAnrMonClosed += 1
            AnrMonEingeblendet = False
            ' Entferne den Anrufmonitor von der Liste der offenen Popups
            ThisAddIn.OffeneAnrMonWPF.Remove(PopUpAnrMonWPF)

            PopUpAnrMonWPF = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Wird durch das Auslösen des Closed Ereignis des <see cref="PopupStoppUhrWPF"/> aufgerufen. Es werden ein paar Bereinigungsarbeiten durchgeführt. 
    ''' Das Closed-Event wird zweimal aufgerufen (Dispatcher.Invoke). Zählvariable zum Triggern der Aufrufe.
    ''' </summary>
    Private Sub PopupStoppUhrGeschlossen(sender As Object, e As EventArgs)
        ' Führe die Arbeiten nur beim ersten Aufruf des Closed-Event des Popups durch.
        If StoppUhrClosed.IsZero Then
            NLogger.Debug("Stoppuhr geschlossen: {0}", AnruferName)
            StoppUhrClosed += 1
            StoppUhrEingeblendet = False
            ' Entferne den Anrufmonitor von der Liste der offenen Popups
            ThisAddIn.OffeneStoppUhrWPF.Remove(PopupStoppUhrWPF)

            PopUpAnrMonWPF = Nothing
        End If
    End Sub

    Private Sub ShowAnrMon()
        Dim t = New Thread(Sub()
                               ' If Not VollBildAnwendungAktiv() Or XMLData.POptionen.CBAnrMonVollbildAnzeigen Then
                               If PopUpAnrMonWPF Is Nothing Then
                                   NLogger.Debug("Blende einen neuen Anrufmonitor ein")
                                   ' Blende einen neuen Anrufmonitor ein
                                   AnrMonEinblenden()

                                   While AnrMonEingeblendet
                                       Forms.Application.DoEvents()
                                       Thread.Sleep(100)
                                   End While
                               End If
                               ' End If
                           End Sub)

        t.SetApartmentState(ApartmentState.STA)
        t.Start()
    End Sub
    Private Sub ShowStoppUhr()
        Dim t = New Thread(Sub()
                               'If Not VollBildAnwendungAktiv() Or XMLData.POptionen.CBAnrMonVollbildAnzeigen Then
                               If PopupStoppUhrWPF Is Nothing Then
                                   NLogger.Debug("Blende einen neue StoppUhr ein")
                                   ' Blende einen neuen Anrufmonitor ein
                                   StoppUhrEinblenden()

                                   While StoppUhrEingeblendet
                                       Forms.Application.DoEvents()
                                       Thread.Sleep(100)
                                   End While
                               End If
                               'End If
                           End Sub)

        t.SetApartmentState(ApartmentState.STA)
        t.Start()
    End Sub
#End Region

#Region "RibbonXML"
    Friend Overloads Function CreateDynMenuButton(xDoc As Xml.XmlDocument, ID As Integer, Tag As String) As Xml.XmlElement
        Dim XButton As Xml.XmlElement
        Dim XAttribute As Xml.XmlAttribute

        XButton = xDoc.CreateElement("button", xDoc.DocumentElement.NamespaceURI)

        XAttribute = xDoc.CreateAttribute("id")
        XAttribute.Value = $"{Tag}_{ID}"
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("label")
        XAttribute.Value = If(AnruferName.IsNotStringNothingOrEmpty, AnruferName, GegenstelleTelNr?.Formatiert).XMLMaskiereZeichen
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("onAction")
        XAttribute.Value = "BtnOnAction"
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("tag")
        XAttribute.Value = Tag.XMLMaskiereZeichen
        XButton.Attributes.Append(XAttribute)

        XAttribute = xDoc.CreateAttribute("supertip")
        XAttribute.Value = $"Zeit: {ZeitBeginn}{Dflt1NeueZeile}Telefonnummer: {GegenstelleTelNr.Formatiert}"
        XButton.Attributes.Append(XAttribute)

        If Not Angenommen Then
            XAttribute = xDoc.CreateAttribute("imageMso")
            XAttribute.Value = "HighImportance"
            XButton.Attributes.Append(XAttribute)
        End If

        Return XButton
    End Function
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
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class


