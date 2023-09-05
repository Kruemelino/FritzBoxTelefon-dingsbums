Imports System.Xml.Serialization
Imports System.Threading.Tasks
Imports FBoxDial.DfltWerteTelefonie

<Serializable()> Public Class Telefonie
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
    <XmlElement("Telefonnummer")> Public Property Telefonnummern As List(Of Telefonnummer)
    <XmlElement("Telefoniegerät")> Public Property Telefoniegeräte As List(Of Telefoniegerät)
    <XmlElement("IPPhoneConnector")> Public Property IPTelefone As List(Of IPPhoneConnector)

    ''' <summary>
    ''' Ortskennzahl des Telefonanschlusses. Wird automatisch ermittelt. Kann in den Einstellungen überschrieben werden.
    ''' </summary>
    <XmlElement("TBOrtsKZ")> Public Property OKZ As String = String.Empty

    ''' <summary>
    ''' Landeskennzahl der Telefonanschlusses. Wird automatisch ermittelt. Kann in den Einstellungen überschrieben werden.
    ''' </summary>
    <XmlElement("TBLandesKZ")> Public Property LKZ As String = "49"
#End Region

#Region "Events"
    Friend Event Status As EventHandler(Of String)
    Friend Event Beendet()
#End Region

    Public Sub New()
        Telefonnummern = New List(Of Telefonnummer)
        Telefoniegeräte = New List(Of Telefoniegerät)

    End Sub

#Region "Import Telefoniedaten der Fritz!Box"

    Friend Async Sub GetFritzBoxDaten()

        With Globals.ThisAddIn.FBoxTR064

            Dim SessionID As String = String.Empty

            ' Ermittle die SessionID für Fritz!Box Query
            If .Deviceconfig.GetSessionID(SessionID) Then

                ' Ermittle die Landeskennzahl (LKZ) und die Ortskennzahl (OKZ)
                If .X_voip.GetVoIPCommonCountryCode(LKZ) And .X_voip.GetVoIPCommonAreaCode(OKZ) Then

                    If LKZ.IsStringNothingOrEmpty Then
                        LKZ = If(XMLData.PTelefonie.LKZ.IsStringNothingOrEmpty, "49", XMLData.PTelefonie.LKZ)
                        PushStatus(LogLevel.Warn, $"Landeskennzahl konnte nicht ermittelt werden (Setze Wert aus Einstellungen: '{LKZ}').")
                    End If

                    If OKZ.IsStringNothingOrEmpty Then PushStatus(LogLevel.Warn, $"Ortskennzahl konnte nicht ermittelt werden.")

                    PushStatus(LogLevel.Debug, $"Kennzahlen: {LKZ}; {OKZ}")

                    ' Lade Telefonnummern via TR-064 
                    Dim NummernListe As FBoxAPI.SIPTelNrList = Nothing
                    ' Füge die Nummer zu den eigenen Nummern hinzu
                    If .X_voip.GetNumbers(NummernListe) Then NummernListe.TelNrList.ForEach(Sub(S) AddEigeneTelNr(S.Number, S.Index))

                    ' Lade SIP Clients via TR-064 (IP-Telefone)
                    Dim SIPList As FBoxAPI.SIPClientList = Nothing
                    If .X_voip.GetClients(SIPList) Then

                        ' Werte alle SIP Clients aus
                        For Each SIPClient In SIPList.SIPClients

                            Dim Telefon As New Telefoniegerät With {.ID = SIPClient.ClientIndex,
                                                                    .Name = SIPClient.PhoneName,
                                                                    .TelTyp = TelTypen.IP,
                                                                    .AnrMonID = AnrMonTelIDBase.IP + SIPClient.ClientIndex,
                                                                    .StrEinTelNr = New List(Of String),
                                                                    .Kurzwahl = SIPClient.InternalNumber}
                            With Telefon

                                If SIPClient.InComingNumbers.First.Type = FBoxAPI.SIPTypeEnum.eAllCalls Then
                                    ' füge alle bekannten Nummern hinzu
                                    Telefonnummern.ForEach(Sub(TelNr) .StrEinTelNr.Add(TelNr.Einwahl))
                                Else
                                    ' Füge die angegebenen eigehenden Telefonnummern hinzu
                                    SIPClient.InComingNumbers.ForEach(Sub(T) .StrEinTelNr.Add(AddEigeneTelNr(T.Number, T.Index).Einwahl))
                                End If

                                PushStatus(LogLevel.Debug, $"Telefon { .TelTyp}: { .AnrMonID}; { .Name}; { .ID}")
                            End With

                            ' Telefon der Liste von Geräten hinzufügen
                            Telefoniegeräte.Add(Telefon)
                        Next
                    End If

                    ' Lade Anrufbeantworter, TAM (telephone answering machine) via TR-064 
                    Dim ABListe As FBoxAPI.TAMList = Await .X_tam.GetList
                    ' Werte alle TAMs aus, welche in der Fritz!Box sichtbar sind.
                    For Each AB In ABListe.Items.Where(Function(T) T.Display)

                        Dim Telefon As New Telefoniegerät With {.ID = AB.Index,
                                                                .Name = AB.Name,
                                                                .TelTyp = TelTypen.TAM,
                                                                .StrEinTelNr = New List(Of String),
                                                                .Kurzwahl = InternBase.TAM + AB.Index,
                                                                .AnrMonID = AnrMonTelIDBase.TAM + AB.Index}

                        ' Ermittle die Nummer, auf den der AB reagiert.
                        Dim TAMInfo As New FBoxAPI.TAMInfo
                        If .X_tam.GetTAMInfo(TAMInfo, AB.Index) Then
                            If TAMInfo.PhoneNumbers.Length.AreEqual(1) AndAlso TAMInfo.PhoneNumbers.First.IsStringNothingOrEmpty Then
                                ' Empty string represents all numbers.
                                Telefonnummern.ForEach(Sub(TelNr) Telefon.StrEinTelNr.Add(TelNr.Einwahl))

                            Else
                                ' Comma (,) separated list represents specific phone numbers.
                                For Each T In TAMInfo.PhoneNumbers
                                    Telefon.StrEinTelNr.Add(GetEigeneTelNr(T)?.Einwahl)
                                Next

                            End If
                        End If
                        PushStatus(LogLevel.Debug, $"Telefon { Telefon.TelTyp}: { Telefon.AnrMonID}; { Telefon.Name}; { Telefon.ID}")
                        ' Telefon der Liste von Geräten hinzufügen
                        Telefoniegeräte.Add(Telefon)
                    Next

#Region "ALT: Nicht alle werden erkannt"
                    '' Asynchroner Task für das Einlesen der FON-Geräte via Query
                    'Dim TaskFON As Task(Of List(Of Telefoniegerät)) = GetFON(SessionID)

                    '' Asynchroner Task für das Einlesen der DECT-Geräte via Query
                    'Dim TaskDECT As Task(Of List(Of Telefoniegerät)) = GetDECT(SessionID)

                    '' Asynchroner Task für das Einlesen der S0-Geräte via Query
                    'Dim TaskS0 As Task(Of List(Of Telefoniegerät)) = GetS0(SessionID)

                    '' Asynchroner Task für das Einlesen der Mobil-Geräte sowie integrieten Faxempfang via Query
                    'Dim TaskMobilFax As Task(Of List(Of Telefoniegerät)) = GetFaxMailMobil(SessionID)

                    '' Füge die ermittelten FON-Geräte hinzu
                    'Telefoniegeräte.AddRange(Await TaskFON)

                    '' Füge die ermittelten DECT-Geräte hinzu 
                    'Telefoniegeräte.AddRange(Await TaskDECT)

                    '' Füge die ermittelten S0-Geräte hinzu 
                    'Telefoniegeräte.AddRange(Await TaskS0)

                    '' Füge die ermittelten Mobil-Geräte sowie integrieten Faxempfang hinzu
                    'Telefoniegeräte.AddRange(Await TaskMobilFax)
#End Region

                    ' Füge die ermittelten FON-Geräte hinzu
                    Telefoniegeräte.AddRange(Await GetFON(SessionID))

                    ' Füge die ermittelten DECT-Geräte hinzu 
                    Telefoniegeräte.AddRange(Await GetDECT(SessionID))

                    ' Füge die ermittelten S0-Geräte hinzu 
                    Telefoniegeräte.AddRange(Await GetS0(SessionID))

                    ' Füge die ermittelten Mobil-Geräte sowie integrieten Faxempfang hinzu
                    Telefoniegeräte.AddRange(Await GetFaxMailMobil(SessionID))

                    ' Ermittle TR-064 Phoneports
                    ' Für die Fritz!Box Wählhilfe nutzbare Telefone ermitteln
                    Dim WählhilfeTelefone As List(Of Telefoniegerät) = Telefoniegeräte.FindAll(Function(Telefon) Telefon.IsFBoxDialable)
                    If WählhilfeTelefone.Any Then
                        ' Ermittle alle Phoneports via X_AVM-DE_GetPhonePort
                        ' X_AVM-DE_PhoneName Empty string to disable feature to dial a number.
                        ' Examples:
                        ' FON1: Telefon
                        ' FON2: Telefon
                        ' ISDN: ISDN/DECT Rundruf
                        ' DECT: Mobilteil 1 

                        ' Schleife durch alle wählbaren Telefone
                        For i = 1 To WählhilfeTelefone.Count
                            Dim Phoneport As String = String.Empty
                            If .X_voip.GetPhonePort(Phoneport, i) Then
                                ' Erfolgreich ermittelt
                                Dim Telefon As Telefoniegerät = WählhilfeTelefone.Find(Function(Tel) Phoneport.EndsWith(Tel.Name))
                                If Telefon IsNot Nothing Then
                                    With Telefon
                                        .TR064Dialport = Phoneport
                                        PushStatus(LogLevel.Debug, $"Setze Phoneport für Telefon { .Name} ({ .TelTyp}): '{ .TR064Dialport}'; Rückfallwert: '{ .GetDialPortFallback}'")
                                    End With
                                End If
                            End If
                        Next
                    End If

                    ' Aufräumen
                    PushStatus(LogLevel.Info, $"Einlesen der Telefoniedaten abgeschlossen...")

                Else

                    PushStatus(LogLevel.Error, $"Fehler: Einlesen nicht möglich: Landes- bzw. Ortsnetzkennzahlen konnten nicht abgerufen werden.")
                End If
            Else

                PushStatus(LogLevel.Error, $"Fehler: Login nicht möglich.")
            End If
        End With

        RaiseEvent Beendet()

    End Sub

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven FON-Geräte mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven FON-Geräte.</returns>
    Private Async Function GetFON(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim FONList As New List(Of Telefoniegerät)


        NLogger.Trace("GetFON - Start")

        ' Frage alle angeschlossenen und aktiven FON Telefone ab.
        TelQuery.Add("FON=telcfg:settings/MSN/Port/list(Name,Fax,AllIncomingCalls)")
        ' TelQuery.Add("FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing,MSN)")

        ' Führe Abfrage aus
        Dim MSNList As FBoxFON = Await JSONDeserializeFromStreamAsync(Of FBoxFON)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))

        If MSNList IsNot Nothing Then
            ' Wenn es eine interne Nummer gibt, sind die DECT-Geräte aktiv
            For Each FONTelefon In MSNList.FON.Where(Function(F) F.Name.IsNotStringNothingOrEmpty)
                ' Dimensioniere ein neues Telefon und setze Daten
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.FON,
                                                        .Name = FONTelefon.Name,
                                                        .ID = FONTelefon.Node.RegExRemove("^\D*").ToInt,
                                                        .Kurzwahl = .ID,
                                                        .AnrMonID = AnrMonTelIDBase.FON + .ID,
                                                        .StrEinTelNr = New List(Of String),
                                                        .IsFax = FONTelefon.Fax}

                ' Abfrageliste leeren
                TelQuery.Clear()
                ' Frage ab, auf welche Nummern das Telefon reagiert.
                For i As Integer = 0 To 9
                    TelQuery.Add($"MSN{i}=telcfg:settings/MSN/{FONTelefon.Node}/MSN{i}")
                Next

                ' Führe Abfrage aus
                Dim FONNr As FBoxFONNr = Await JSONDeserializeFromStreamAsync(Of FBoxFONNr)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))

                If FONNr IsNot Nothing Then

                    ' Verarbeite alle Nummer des FON-Telefones
                    If FONTelefon.AllIncomingCalls Then
                        ' Weise dem Telefon alle bekannten Nummern zu
                        Telefonnummern.ForEach(Sub(TelNr) Telefon.StrEinTelNr.Add(TelNr.Einwahl))

                    Else
                        ' Verarbeite die angegebenen Nummern
                        For Each FONTelNr In FONNr.MSNList.Where(Function(M) M.IsNotStringNothingOrEmpty)
                            Telefon.StrEinTelNr.Add(GetEigeneTelNr(FONTelNr)?.Einwahl)
                        Next
                    End If

                    PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.ID}")
                    FONList.Add(Telefon)

                End If
            Next
        End If

        NLogger.Trace($"GetFON - Ende ({FONList.Count})")
        Return FONList
    End Function

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven DECT-Geräte mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven DECT-Geräte.</returns>
    Private Async Function GetDECT(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim DECTList As New List(Of Telefoniegerät)

        NLogger.Trace("GetDECT - Start")

        ' Frage alle angeschlossenen und aktiven DECT Telefone ab.
        TelQuery.Add("DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)")

        Dim DECTTelList As FBoxDECT = Await JSONDeserializeFromStreamAsync(Of FBoxDECT)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))

        ' Führe Abfrage aus
        If DECTTelList IsNot Nothing Then
            ' Wenn es eine interne Nummer gibt, sind die DECT-Geräte aktiv
            For Each DECTTelefon In DECTTelList.DECT.Where(Function(D) D.Intern.IsNotStringNothingOrEmpty)
                ' Dimensioniere ein neues Telefon und setze Daten
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.DECT,
                                                        .Name = DECTTelefon.Name,
                                                        .ID = DECTTelefon.Intern.ToInt,
                                                        .Kurzwahl = .ID,
                                                        .IsFax = False,
                                                        .AnrMonID = .ID - InternBase.DECT + AnrMonTelIDBase.DECT,
                                                        .StrEinTelNr = New List(Of String)}
                ' Abfrageliste leeren
                TelQuery.Clear()
                ' Frage ab, ob das Telefon auf alle Nummern reagieren soll.
                TelQuery.Add($"DECTRingOnAllMSNs=telcfg:settings/Foncontrol/User{DECTTelefon.Id}/RingOnAllMSNs")
                ' Frage ab, auf welche Nummern das Telefon  reagiert.
                TelQuery.Add($"DECTNr=telcfg:settings/Foncontrol/User{DECTTelefon.Id}/MSN/list(Number)")

                ' Führe Abfrage aus
                Dim DECTNr As FBoxDECTNr = Await JSONDeserializeFromStreamAsync(Of FBoxDECTNr)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))
                If DECTNr IsNot Nothing Then
                    ' Veraarbeite alle Nummer des DECT-Telefones
                    If DECTNr.DECTRingOnAllMSNs Then
                        ' Weise dem Telefon alle bekannten Nummern zu
                        For Each TelNr In Telefonnummern.Distinct(New Telefonnummer)
                            Telefon.StrEinTelNr.Add(TelNr.Einwahl)
                        Next
                    Else
                        ' Verarbeite die angegebenen Nummern
                        For Each DECTelNr In DECTNr.DECTNr.Where(Function(T) T.Number.IsNotStringNothingOrEmpty)

                            Telefon.StrEinTelNr.Add(GetEigeneTelNr(DECTelNr.Number)?.Einwahl)
                        Next
                    End If

                    PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.ID}")
                    DECTList.Add(Telefon)
                End If
            Next
        End If

        NLogger.Trace($"GetDECT - Ende ({DECTList.Count})")
        Return DECTList
    End Function

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven S0-Geräte mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven S0-Geräte.</returns>
    Private Async Function GetS0(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim S0List As New List(Of Telefoniegerät)

        NLogger.Trace("GetS0 - Start")

        ' Frage alle möglichen S0 Telefone ab (1-8).
        ' 0 Ist der ISDN/DECT Rundruf
        For idx = 0 To 8
            With TelQuery
                ' Abfrageliste leeren
                .Clear()
                ' Abfrage nach Gerätenamen
                TelQuery.Add($"S0Name=telcfg:settings/NTHotDialList/Name{idx}")
                ' Abfrage nach Nummer (intern?)
                TelQuery.Add($"S0Number=telcfg:settings/NTHotDialList/Number{idx}")
                ' Abfrage nach Typ. Wird momentan nicht verwendet
                'TelQuery.Add($"S0Type=telcfg:settings/NTHotDialList/Type{idx}")
            End With

            ' Führe Abfrage aus
            Dim S0Tel As FBoxS0 = Await JSONDeserializeFromStreamAsync(Of FBoxS0)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))

            If S0Tel IsNot Nothing Then
                ' Wenn es einen Namen gibt, sind die S0-Geräte aktiv
                If S0Tel.S0Name.IsNotStringNothingOrEmpty Then

                    ' Dimensioniere ein neues Telefon und setze Daten
                    Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.ISDN,
                                                            .AnrMonID = AnrMonTelIDBase.S0,
                                                            .ID = InternBase.S0 + idx,
                                                            .Kurzwahl = .ID,
                                                            .StrEinTelNr = New List(Of String),
                                                            .Name = S0Tel.S0Name}

                    If Telefon.ID.AreDifferentTo(S0Tel.S0Number.ToInt) Then
                        Telefon.StrEinTelNr.Add(GetEigeneTelNr(S0Tel.S0Number)?.Einwahl)
                    End If

                    PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.ID}")
                    S0List.Add(Telefon)

                End If
            End If
        Next

        NLogger.Trace($"GetS0 - Ende ({S0List.Count})")

        Return S0List
    End Function

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven Mobil-Geräte sowie den internen Faxempfang mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven Mobil-Geräte und dem internen Faxempfang.</returns>
    Private Async Function GetFaxMailMobil(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim TelList As New List(Of Telefoniegerät)

        NLogger.Trace("GetFaxMailMobil - Start")

        With TelQuery
            .Add($"FaxMailActive=telcfg:settings/FaxMailActive")
            .Add($"MobileName=telcfg:settings/Mobile/Name")
            .Add($"Mobile=telcfg:settings/Mobile/MSN")
        End With

        ' Führe Abfrage aus
        Dim MailMobilTel As FaxMailMobil = Await JSONDeserializeFromStreamAsync(Of FaxMailMobil)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))

        If MailMobilTel IsNot Nothing Then
            ' Verarbeite Mobilgerät, wenn es eine Mobilnummer gibt.
            If MailMobilTel.Mobile.IsNotStringNothingOrEmpty Then
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.Mobil,
                                                        .AnrMonID = AnrMonTelIDBase.Mobil,
                                                        .StrEinTelNr = New List(Of String)}

                Telefon.StrEinTelNr.Add(GetEigeneTelNr(MailMobilTel.Mobile)?.Einwahl)
                PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.ID}")
                TelList.Add(Telefon)

            End If

            ' Verarbeite internen Faxempfang (FaxMail)
            If MailMobilTel.FaxMailActive Then
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.FAX,
                                                        .AnrMonID = AnrMonTelIDBase.Fax,
                                                        .Name = "Faxempfang",
                                                        .IsFax = True,
                                                        .StrEinTelNr = New List(Of String)}
                ' Fax-Nummern ermitteln
                With TelQuery
                    ' Abfrageliste leeren
                    .Clear()

                    For i = 0 To 9
                        ' Fax-Nummern
                        .Add($"FAX{i}=telcfg:settings/FaxMSN{i}")
                    Next
                End With

                ' Führe Abfrage aus
                Dim FaxNr As FBoxFaxNr = Await JSONDeserializeFromStreamAsync(Of FBoxFaxNr)(Await Globals.ThisAddIn.FBoxTR064.HttpService.GetLuaResponseStream(SessionID, TelQuery))
                If FaxNr IsNot Nothing Then
                    For Each FaxTelNr In FaxNr.FAXList.Where(Function(M) M.IsNotStringNothingOrEmpty)

                        Telefon.StrEinTelNr.Add(GetEigeneTelNr(FaxTelNr)?.Einwahl)
                    Next
                End If

                PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.ID}")
                TelList.Add(Telefon)

            End If
        End If

        NLogger.Trace($"GetFaxMailMobil - Ende ({TelList.Count})")

        Return TelList
    End Function

#End Region

#Region "Eigenschaften zur Datenausgabe"
    <XmlIgnore> Friend ReadOnly Property GetTelNrByID(ID As Integer) As Telefonnummer
        Get
            Return Telefonnummern.Find(Function(T) T.EigeneNummerInfo.SIP.AreEqual(ID))
        End Get
    End Property

    <XmlIgnore> Friend ReadOnly Property GetTelefonByID(ID As Integer) As Telefoniegerät
        Get
            Return Telefoniegeräte.Find(Function(T) T.ID.AreEqual(ID))
        End Get
    End Property

    <XmlIgnore> Friend ReadOnly Property GetIPTelefonByID(ID As Integer) As IPPhoneConnector
        Get
            Return IPTelefone.Find(Function(T) T.ConnectedPhoneID.AreEqual(ID))
        End Get
    End Property
#End Region

#Region "Helferfunktionen"

    ''' <summary>
    ''' Ermittelt eine eigene bekannte Telefonnummer anhand einer Zeichenfolge. SIP0 etc. wird erfasst.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer als Zeichenfolge</param>
    ''' <returns>Telefonnummer</returns>
    Friend Function GetEigeneTelNr(TelNr As String) As Telefonnummer
        If TelNr.IsRegExMatch("^SIP\d") Then
            Return GetTelNrByID(TelNr.RegExRemove("^SIP").ToInt)
        Else
            ' Standardvergleich
            Return Telefonnummern.Find(Function(Tel) Tel.Equals(TelNr))
        End If
    End Function

    ''' <summary>
    ''' Fügt eine neue eigene Telefonnummer hinzu, falls sie noch nicht exisiert, und gib sie zurück.
    ''' Falls die Nummer schon in der Liste enthalten ist, gib diese zurück.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer als Zeichenfolge</param>
    ''' <returns>Telefonnummer</returns>
    Private Function AddEigeneTelNr(TelNr As String, ID As String) As Telefonnummer

        AddEigeneTelNr = GetEigeneTelNr(TelNr)

        If AddEigeneTelNr Is Nothing Then
            ' Es ist wichtig, dass die LKZ und die OKZ in jedem Fall übergeben werden. Führe daher das SetNummer zuletzt aus.
            AddEigeneTelNr = New Telefonnummer With {.Ortskennzahl = OKZ,
                                                     .Landeskennzahl = LKZ,
                                                     .EigeneNummerInfo = New EigeneNrInfo With {.Überwacht = True,
                                                                                                .Farben = New Farbdefinition,
                                                                                                .SIP = ID.ToInt},
                                                     .SetNummer = TelNr}

            ' Ermittle aus den bereits bekannten Nummern, damit die benutzerdefinierten Einstellungen (Überwachung, Farbe) behalten werden.
            If XMLData.PTelefonie?.Telefonnummern IsNot Nothing Then

                ' Suche die Telefonnummer
                Dim AlteNummer As Telefonnummer = XMLData.PTelefonie.Telefonnummern.Find(Function(T) T.Equals(AddEigeneTelNr))

                ' Wenn keine Nummer gefunden wurde, dann unternimm nichts
                If AlteNummer IsNot Nothing AndAlso AlteNummer.EigeneNummerInfo IsNot Nothing Then
                    ' Überschreibe die Daten der eigenen Nummer
                    AddEigeneTelNr.EigeneNummerInfo.Überwacht = AlteNummer.EigeneNummerInfo.Überwacht

                    If AlteNummer.EigeneNummerInfo.Farben IsNot Nothing Then
                        AddEigeneTelNr.EigeneNummerInfo.Farben = AlteNummer.EigeneNummerInfo.Farben
                    End If

                End If

            End If

            Telefonnummern.Add(AddEigeneTelNr)
            PushStatus(LogLevel.Debug, $"Telefonnummern: '{TelNr}' ({ID}); F: '{AddEigeneTelNr.Formatiert}'; U: '{AddEigeneTelNr.Unformatiert}'")
        End If
    End Function

    ''' <summary>
    ''' Gibt eine Statusmeldung (<paramref name="StatusMessage"/>) als Event aus. Gleichzeitig wird in das Log mit vorgegebenem <paramref name="Level"/> geschrieben.
    ''' </summary>
    ''' <param name="Level">NLog LogLevel</param>
    ''' <param name="StatusMessage">Die auszugebende Statusmeldung.</param>
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Me, StatusMessage)
    End Sub

#End Region

End Class
