﻿Imports System.Xml.Serialization
Imports System.Threading.Tasks

Imports FBoxDial.DfltWerteTelefonie
Imports FBoxDial.FritzBoxDefault

<Serializable()> Public Class Telefonie
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
    <XmlElement("Telefonnummer")> Public Property Telefonnummern As List(Of Telefonnummer)
    <XmlElement("Telefoniegerät")> Public Property Telefoniegeräte As List(Of Telefoniegerät)

    ''' <summary>
    ''' Ortskennzahl des Telefonanschlusses. Wird automatisch ermittelt. Kann in den Einstellungen überschrieben werden.
    ''' </summary>
    <XmlElement("TBOrtsKZ")> Public Property OKZ As String

    ''' <summary>
    ''' Landeskennzahl der Telefonanschlusses. Wird automatisch ermittelt. Kann in den Einstellungen überschrieben werden.
    ''' </summary>
    <XmlElement("TBLandesKZ")> Public Property LKZ As String
#End Region

#Region "Events"
    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String))
    Friend Event Beendet()
#End Region

    Public Sub New()
        Telefonnummern = New List(Of Telefonnummer)
        Telefoniegeräte = New List(Of Telefoniegerät)

    End Sub

    ''' <summary>
    ''' Gibt die Telefonnummer zurück, die der übergebenen Zeichenfolge entspricht
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer als Zeichenfolge</param>
    ''' <returns>Telefonnummer</returns>
    Friend Function GetNummer(TelNr As String) As Telefonnummer
        Return Telefonnummern.Find(Function(Tel) Tel.Equals(TelNr))
    End Function

    Friend Function GetKennzahlen() As Boolean
        Using fbSOAP As New FritzBoxTR64
            With fbSOAP
                Return .GetVoIPCommonCountryCode(LKZ) AndAlso .GetVoIPCommonAreaCode(OKZ)
            End With
        End Using
    End Function

#Region "Import Telefoniedaten der Fritz!Box"

    Friend Async Sub GetFritzBoxDaten()

        Dim SessionID As String = DfltFritzBoxSessionID

        ' Starte die TR-064 Schnittstelle zur Fritz!Box
        Using fbtr064 As New FritzBoxTR64

            With fbtr064
                ' Ermittle die SessionID für Fritz!Box Query
                If .GetSessionID(SessionID) Then

                    ' Ermittle die Landeskennzahl (LKZ) und die Ortskennzahl (OKZ)
                    If .GetVoIPCommonCountryCode(LKZ) And .GetVoIPCommonAreaCode(OKZ) Then

                        PushStatus(LogLevel.Debug, $"Kennzahlen: {LKZ}; {OKZ}")

                        ' Lade Telefonnummern via TR-064 
                        Dim NummernListe As SIPTelNrList = Nothing
                        ' Füge die Nummer zu den eigenen Nummern hinzu
                        If .GetNumbers(NummernListe) Then NummernListe.TelNrList.ForEach(Sub(S) AddEigeneTelNr(S))

                        ' Lade SIP Clients via TR-064 
                        Dim SIPClientList As SIPClientList = Nothing
                        If .GetSIPClients(SIPClientList) Then
                            ' Werte alle SIP Clients aus
                            For Each SIPClient In SIPClientList.SIPClientList

                                Dim Telefon As New Telefoniegerät With {.Name = SIPClient.PhoneName,
                                                .TelTyp = DfltWerteTelefonie.TelTypen.IP,
                                                .AnrMonID = AnrMonTelIDBase.IP + SIPClient.ClientIndex,
                                                .StrEinTelNr = New List(Of String),
                                                .Intern = SIPClient.InternalNumber}
                                With Telefon

                                    ' Füge die eigehenden Telefonnummern hinzu
                                    SIPClient.InComingNumbers.ForEach(Sub(TeLNr) .StrEinTelNr.Add(AddEigeneTelNr(TeLNr.Number).Einwahl))

                                    PushStatus(LogLevel.Debug, $"Telefon { .TelTyp}: { .AnrMonID}; { .Name}; { .Intern}")
                                End With

                                ' Telefon der Liste von Geräten hinzufügen
                                Telefoniegeräte.Add(Telefon)

                                NLogger.Debug($"Test Dialport Fallback IP-Telefon: '{Telefon.GetDialPortFallback}'")
                            Next
                        End If

                        ' Lade Anrufbeantworter, TAM (telephone answering machine) via TR-064 
                        Dim ABListe As TAMList = Nothing
                        If .GetTAMList(ABListe) Then
                            ' Werte alle TAMs aus.
                            For Each AB In ABListe.TAMListe.Where(Function(TAM) TAM.Enable)

                                Dim Telefon As New Telefoniegerät With {.Name = AB.Name,
                                                .TelTyp = TelTypen.TAM,
                                                .StrEinTelNr = New List(Of String),
                                                .Intern = InternBase.TAM + AB.Index}

                                ' Ermittle die Nummer, auf den der AB reagiert.
                                Dim TelNrArray As String() = {}
                                If .GetTAMInfo(TelNrArray, AB.Index) Then
                                    If TelNrArray.Length.IsZero Then
                                        ' Empty string represents all numbers.
                                        Telefonnummern.ForEach(Sub(TelNr) Telefon.StrEinTelNr.Add(TelNr.Einwahl))

                                    Else
                                        ' Comma (,) separated list represents specific phone numbers.
                                        For Each TelNr In TelNrArray
                                            Telefon.StrEinTelNr.Add(AddEigeneTelNr(TelNr).Einwahl)
                                        Next

                                    End If
                                End If
                                PushStatus(LogLevel.Debug, $"Telefon { Telefon.TelTyp}: { Telefon.AnrMonID}; { Telefon.Name}; { Telefon.Intern}")
                                ' Telefon der Liste von Geräten hinzufügen
                                Telefoniegeräte.Add(Telefon)

                                NLogger.Debug($"Test Dialport Fallback TAM: '{Telefon.GetDialPortFallback}'")
                            Next
                        End If

                        ' Asynchroner Task für das Einlesen der FON-Geräte via Query
                        Dim TaskFON As Task(Of List(Of Telefoniegerät)) = GetFON(SessionID)

                        ' Asynchroner Task für das Einlesen der DECT-Geräte via Query
                        Dim TaskDECT As Task(Of List(Of Telefoniegerät)) = GetDECT(SessionID)

                        ' Asynchroner Task für das Einlesen der S0-Geräte via Query
                        Dim TaskS0 As Task(Of List(Of Telefoniegerät)) = GetS0(SessionID)

                        ' Asynchroner Task für das Einlesen der Mobil-Geräte sowie integrieten Faxempfang via Query
                        Dim TaskMobilFax As Task(Of List(Of Telefoniegerät)) = GetFaxMailMobil(SessionID)

                        ' Füge die ermittelten FON-Geräte hinzu
                        Telefoniegeräte.AddRange(Await TaskFON)

                        ' Füge die ermittelten DECT-Geräte hinzu 
                        Telefoniegeräte.AddRange(Await TaskDECT)

                        ' Füge die ermittelten S0-Geräte hinzu 
                        Telefoniegeräte.AddRange(Await TaskS0)

                        ' Füge die ermittelten Mobil-Geräte sowie integrieten Faxempfang hinzu
                        Telefoniegeräte.AddRange(Await TaskMobilFax)

                        ' ISDN/DECT Rundruf, falls S0 oder DECT Geräte verfügbar 
                        Telefoniegeräte.Add(GetRundruf)

                        ' Ermittle TR-064 Phoneports
                        ' Für die Fritz!Box Wählhilfe nutzbare Telefone ermitteln
                        Dim WählhilfeTelefone As List(Of Telefoniegerät) = Telefoniegeräte.FindAll(Function(Telefon) Telefon.TelTyp = TelTypen.FON Or Telefon.TelTyp = TelTypen.DECT Or Telefon.TelTyp = TelTypen.ISDN)
                        If WählhilfeTelefone.Any Then
                            ' Ermittle alle Phoneports via X_AVM-DE_GetPhonePort
                            ' X_AVM-DE_PhoneName Empty string to disable feature to dial a number.
                            ' Examples:
                            ' FON1: Telefon
                            ' FON2: Telefon
                            ' ISDN: ISDN/ DECT Rundruf
                            ' DECT: Mobilteil 1 

                            ' Schleife durch alle wählbaren Telefone
                            For i = 1 To WählhilfeTelefone.Count
                                Dim Phoneport As String = DfltStringEmpty
                                If .GetPhonePort(Phoneport, i) Then
                                    ' Erfolgreich ermittelt
                                    Dim Telefon As Telefoniegerät = WählhilfeTelefone.Find(Function(Tel) Phoneport.EndsWith(Tel.Name))
                                    If Telefon IsNot Nothing Then
                                        With Telefon
                                            .TR064Dialport = Phoneport
                                            PushStatus(LogLevel.Debug, $"Setze Phoneport für Telefon { .Name} ({ .TelTyp}): { .TR064Dialport}")
                                        End With
                                    End If
                                End If
                            Next
                        End If

                        ' Setze neue Telefoniedaten in die XML Daten
                        XMLData.PTelefonie = Me

                        ' Aufräumen
                        PushStatus(LogLevel.Info, $"Einlesen der Telefoniedaten abgeschlossen...")
                        RaiseEvent Beendet()
                    Else

                        PushStatus(LogLevel.Error, $"Fehler Einlesen nicht erfolgreich")
                    End If

                End If
            End With

        End Using

    End Sub

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven FON-Geräte mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven FON-Geräte.</returns>
    Private Async Function GetFON(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim QueryAntwort As String = DfltStringEmpty
        Dim FONList As New List(Of Telefoniegerät)

        NLogger.Debug("GetFON - Start")

        ' Frage alle angeschlossenen und aktiven DECT Telefone ab.
        TelQuery.Add("FON=telcfg:settings/MSN/Port/list(Name,Fax,AllIncomingCalls)")
        ' Führe Abfrage aus
        QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)

        With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FBoxFON)(QueryAntwort)
            ' Wenn es eine interne Nummer gibt, sind die DECT-Geräte aktiv
            For Each FONTelefon In .FON.Where(Function(F) F.Name.IsNotStringNothingOrEmpty)
                ' Dimensioniere ein neues Telefon und setze Daten
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.FON,
                                                        .Name = FONTelefon.Name,
                                                        .Intern = FONTelefon.Node.RegExRemove("^\D*").ToInt,
                                                        .AnrMonID = AnrMonTelIDBase.FON + .Intern,
                                                        .StrEinTelNr = New List(Of String)}

                ' Abfrageliste leeren
                TelQuery.Clear()
                ' Frage ab, auf welche Nummern das Telefon reagiert.
                For i As Integer = 0 To 9
                    TelQuery.Add($"MSN{i}=telcfg:settings/MSN/{FONTelefon.Node}/MSN{i}")
                Next

                ' Führe Abfrage aus
                QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)

                With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FBoxFONNr)(QueryAntwort)

                    ' Veraarbeite alle Nummer des FON-Telefones
                    If CBool(FONTelefon.AllIncomingCalls) Then
                        ' Weise dem Telefon alle bekannten Nummern zu
                        For Each TelNr In Telefonnummern.Distinct
                            Telefon.StrEinTelNr.Add(TelNr.Einwahl)
                        Next
                    Else
                        ' Verarbeite die angegebenen Nummern
                        For Each FONTelNr In .MSNList.Where(Function(M) M.IsNotStringNothingOrEmpty)

                            Telefon.StrEinTelNr.Add(GetTelNr(FONTelNr)?.Einwahl)
                        Next
                    End If

                    PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.Intern}")
                    FONList.Add(Telefon)

                    NLogger.Debug($"Test Dialport Fallback FON: '{Telefon.GetDialPortFallback}'")
                End With
            Next
        End With
        NLogger.Debug($"GetFON - Ende ({FONList.Count})")
        Return FONList
    End Function

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven DECT-Geräte mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven DECT-Geräte.</returns>
    Private Async Function GetDECT(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim QueryAntwort As String = DfltStringEmpty
        Dim DECTList As New List(Of Telefoniegerät)

        NLogger.Debug("GetDECT - Start")

        ' Frage alle angeschlossenen und aktiven DECT Telefone ab.
        TelQuery.Add("DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)")
        ' Führe Abfrage aus
        QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)

        With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FBoxDECT)(QueryAntwort)
            ' Wenn es eine interne Nummer gibt, sind die DECT-Geräte aktiv
            For Each DECTTelefon In .DECT.Where(Function(D) D.Intern.IsNotStringNothingOrEmpty)
                ' Dimensioniere ein neues Telefon und setze Daten
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.DECT,
                                                        .Name = DECTTelefon.Name,
                                                        .Intern = DECTTelefon.Intern.ToInt,
                                                        .IsFax = False,
                                                        .AnrMonID = AnrMonTelIDBase.DECT + InternBase.DECT - .Intern,
                                                        .StrEinTelNr = New List(Of String)}
                ' Abfrageliste leeren
                TelQuery.Clear()
                ' Frage ab, ob das Telefon auf alle Nummern reagieren soll.
                TelQuery.Add($"DECTRingOnAllMSNs=telcfg:settings/Foncontrol/User{DECTTelefon.Id}/RingOnAllMSNs")
                ' Frage ab, auf welche Nummern das Telefon  reagiert.
                TelQuery.Add($"DECTNr=telcfg:settings/Foncontrol/User{DECTTelefon.Id}/MSN/list(Number)")
                ' Führe Abfrage aus
                QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)

                With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FBoxDECTNr)(QueryAntwort)

                    ' Veraarbeite alle Nummer des DECT-Telefones
                    If CBool(.DECTRingOnAllMSNs) Then
                        ' Weise dem Telefon alle bekannten Nummern zu
                        For Each TelNr In Telefonnummern.Distinct
                            Telefon.StrEinTelNr.Add(TelNr.Einwahl)
                        Next
                    Else
                        ' Verarbeite die angegebenen Nummern
                        For Each DECTelNr In .DECTNr.Where(Function(T) T.Number.IsNotStringNothingOrEmpty)

                            Telefon.StrEinTelNr.Add(GetTelNr(DECTelNr.Number)?.Einwahl)
                        Next
                    End If

                    PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.Intern}")
                    DECTList.Add(Telefon)

                    NLogger.Debug($"Test Dialport Fallback DECT: '{Telefon.GetDialPortFallback}'")
                End With
            Next
        End With

        NLogger.Debug($"GetDECT - Ende ({DECTList.Count})")
        Return DECTList
    End Function

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven S0-Geräte mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven S0-Geräte.</returns>
    Private Async Function GetS0(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim QueryAntwort As String = DfltStringEmpty
        Dim S0List As New List(Of Telefoniegerät)

        NLogger.Debug("GetS0 - Start")

        For idx = 1 To 8
            With TelQuery
                ' Abfrageliste leeren
                .Clear()
                ' Abfrage nach Gerätenamen
                TelQuery.Add($"S0Name=telcfg:settings/NTHotDialList/Name{idx}")
                ' Abfrage nach Nummer (intern?)
                TelQuery.Add($"S0Number=telcfg:settings/NTHotDialList/Number{idx}")
                ' ABfrage nach Typ. Wird momentan nicht verwendet
                'TelQuery.Add($"S0Type=telcfg:settings/NTHotDialList/Type{idx}")
            End With
            ' Frage alle möglichen S0 Telefone ab (1-8). 
            ' Führe Abfrage aus
            QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)

            With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FBoxS0)(QueryAntwort)
                ' Wenn es einen Namen gibt, sind die S0-Geräte aktiv
                If .S0Name.IsNotStringNothingOrEmpty Then

                    ' Dimensioniere ein neues Telefon und setze Daten
                    Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.ISDN,
                                                            .AnrMonID = AnrMonTelIDBase.S0 + idx,
                                                            .Intern = InternBase.S0 + idx,
                                                            .StrEinTelNr = New List(Of String)}

                    Telefon.Name = .S0Name
                    If Telefon.Intern.AreDifferentTo(.S0Number.ToInt) Then
                        Telefon.StrEinTelNr.Add(GetTelNr(.S0Number)?.Einwahl)
                    End If

                    PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.Intern}")
                    S0List.Add(Telefon)

                    NLogger.Debug($"Test Dialport Fallback S0: '{Telefon.GetDialPortFallback}'")
                End If
            End With
        Next

        NLogger.Debug($"GetS0 - Ende ({S0List.Count})")

        Return S0List
    End Function

    ''' <summary>
    ''' Liest alle angeschlossenen und aktiven Mobil-Geräte sowie den internen Faxempfang mittels Query aus der Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Gültige Session ID</param>
    ''' <returns>Liste aller angeschlossenen und aktiven Mobil-Geräte und dem internen Faxempfang.</returns>
    Private Async Function GetFaxMailMobil(SessionID As String) As Task(Of List(Of Telefoniegerät))
        Dim TelQuery As New List(Of String)
        Dim QueryAntwort As String = DfltStringEmpty
        Dim TelList As New List(Of Telefoniegerät)

        NLogger.Debug("GetFaxMailMobil - Start")

        With TelQuery
            .Add($"FaxMailActive=telcfg:settings/FaxMailActive")
            .Add($"MobileName=telcfg:settings/Mobile/Name")
            .Add($"Mobile=telcfg:settings/Mobile/MSN")
        End With

        ' Führe Abfrage aus
        QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)
        With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FaxMailMobil)(QueryAntwort)
            ' Verarbeite Mobilgerät, wenn es eine Mobilnummer gibt.
            If .Mobile.IsNotStringNothingOrEmpty Then
                Dim Telefon As New Telefoniegerät With {.TelTyp = TelTypen.Mobil,
                                                        .AnrMonID = AnrMonTelIDBase.Mobil,
                                                        .StrEinTelNr = New List(Of String)}

                Telefon.StrEinTelNr.Add(GetTelNr(.Mobile)?.Einwahl)
                PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.Intern}")
                TelList.Add(Telefon)

                NLogger.Debug($"Test Dialport Fallback Mobile: '{Telefon.GetDialPortFallback}'")
            End If

            ' Verarbeite internen Faxempfang (FaxMail)
            If .FaxMailActive.IsNotStringNothingOrEmpty Then
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
                QueryAntwort = Await FritzBoxAsyncQuery(SessionID, TelQuery)

                With Newtonsoft.Json.JsonConvert.DeserializeObject(Of FBoxFaxNr)(QueryAntwort)
                    For Each FaxTelNr In .FAXList.Where(Function(M) M.IsNotStringNothingOrEmpty)

                        Telefon.StrEinTelNr.Add(GetTelNr(FaxTelNr)?.Einwahl)
                    Next
                End With

                PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.Intern}")
                TelList.Add(Telefon)

                NLogger.Debug($"Test Dialport Fallback FaxMail: '{Telefon.GetDialPortFallback}'")
            End If
        End With

        NLogger.Debug($"GetFaxMailMobil - Ende ({TelList.Count})")

        Return TelList
    End Function

    ''' <summary>
    ''' Erstellt den ISDN/DECT Rundruf, sofern DECT oder S0 Geräte vorhanden sind.
    ''' </summary>
    ''' <returns></returns>
    Private Function GetRundruf() As Telefoniegerät
        Dim Telefon As Telefoniegerät = Nothing
        ' Verarbeitung des Telefons: ISDN/DECT Rundruf
        If Telefoniegeräte.Find(Function(T) T.TelTyp = TelTypen.ISDN Or T.TelTyp = TelTypen.DECT) IsNot Nothing Then
            Telefon = New Telefoniegerät With {.TelTyp = TelTypen.ISDN,
                                                       .AnrMonID = AnrMonTelIDBase.S0,
                                                       .Name = "ISDN/DECT Rundruf",
                                                       .Intern = InternBase.S0}

            PushStatus(LogLevel.Debug, $"Telefon {Telefon.TelTyp}: {Telefon.AnrMonID}; {Telefon.Name}; {Telefon.Intern}")
        End If
        Return Telefon
    End Function

#End Region

#Region "Helferfunktionen"
    ''' <summary>
    ''' Fügt eine neue eigene Telefonnummer hinzu, falls sie noch nicht exisiert, und gieb sie zurück.
    ''' Falls die Nummer schon in der Liste enthalten ist, gib diese zurück.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer als Zeichenfolge</param>
    ''' <returns>Telefonnummer</returns>
    Private Function AddEigeneTelNr(TelNr As String) As Telefonnummer

        AddEigeneTelNr = Telefonnummern.Find(Function(Nummer) Nummer.Equals(TelNr))

        If AddEigeneTelNr Is Nothing Then
            AddEigeneTelNr = New Telefonnummer With {.EigeneNummer = True, .Ortskennzahl = OKZ, .Landeskennzahl = LKZ, .SetNummer = TelNr}
            Telefonnummern.Add(AddEigeneTelNr)
        End If
    End Function

    Private Function AddEigeneTelNr(TelNr As SIPTelNr) As Telefonnummer

        AddEigeneTelNr = Telefonnummern.Find(Function(Nummer) Nummer.Equals(TelNr.Number))

        If AddEigeneTelNr Is Nothing Then
            AddEigeneTelNr = New Telefonnummer With {.EigeneNummer = True,
                                                     .Überwacht = True,
                                                     .Ortskennzahl = OKZ,
                                                     .Landeskennzahl = LKZ,
                                                     .SIP = TelNr.Index,
                                                     .SetNummer = TelNr.Number}
            Telefonnummern.Add(AddEigeneTelNr)
        Else
            AddEigeneTelNr.SIP = TelNr.Index
        End If

        With TelNr
            PushStatus(LogLevel.Debug, $"X_AVM-DE_GetNumbers({ .Index}): { .Number}, { .Type}")
        End With
    End Function

    ''' <summary>
    ''' Ermittelt eine eigene bekannte Telefonnummer anhand einer Zeichenfolge. SIP0 etc. wird erfasst.
    ''' </summary>
    ''' <param name="TelNr">Die zu ermittelnde Telefonnummer</param>
    ''' <returns></returns>
    Private Function GetTelNr(TelNr As String) As Telefonnummer

        If TelNr.IsRegExMatch("^SIP\d") Then
            Return Telefonnummern.Find(Function(T) T.SIP.AreEqual(TelNr.RegExRemove("^SIP").ToInt))
        Else
            Return Telefonnummern.Find(Function(T) T.Equals(TelNr))
        End If

    End Function


    ''' <summary>
    ''' Führt die Abfrage zur Fritz!Box aus.
    ''' </summary>
    ''' <param name="SessionID">Die gültige SessionID</param>
    ''' <param name="Abfrage">Die auszuführende Abfrage.</param>
    ''' <returns></returns>
    Private Async Function FritzBoxAsyncQuery(SessionID As String, Abfrage As List(Of String)) As Task(Of String)
        Return Await HTTPAsyncGet($"{FBLinkBasis}/query.lua?{SessionID}&{String.Join("&", Abfrage.ToArray)}", Encoding.GetEncoding(DfltCodePageFritzBox))
    End Function
    Private Function FritzBoxQuery(SessionID As String, Abfrage As List(Of String)) As String
        Return HTTPGet($"{FBLinkBasis}/query.lua?{SessionID}&{String.Join("&", Abfrage.ToArray)}", Encoding.GetEncoding(DfltCodePageFritzBox))
    End Function

    ''' <summary>
    ''' Gibt eine Statusmeldung (<paramref name="StatusMessage"/>) als Event aus. Gleichzeitig wird in das Log mit vorgegebenem <paramref name="Level"/> geschrieben.
    ''' </summary>
    ''' <param name="Level">NLog LogLevel</param>
    ''' <param name="StatusMessage">Die auszugebende Statusmeldung.</param>
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Me, New NotifyEventArgs(Of String)(StatusMessage))
    End Sub
#End Region

End Class
