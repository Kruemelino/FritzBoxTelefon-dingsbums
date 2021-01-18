Imports System.Collections
Imports FBoxDial.DfltWerteTelefonie
Imports FBoxDial.FritzBoxDefault

Public Class FritzBoxData
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String))
    Friend Event Beendet()
    Public Sub New()
        If XMLData IsNot Nothing Then

            ' Gültige IP-Adresse für die Fritz!Box ablegen
            XMLData.POptionen.ValidFBAdr = ValidIP(XMLData.POptionen.TBFBAdr)

        End If
    End Sub

    ''' <summary>
    ''' Gibt eine Statusmeldung (<paramref name="StatusMessage"/>) als Event aus. Gleichzeitig wird in das Log mit vorgegebenem <paramref name="Level"/> geschrieben.
    ''' </summary>
    ''' <param name="Level">NLog LogLevel</param>
    ''' <param name="StatusMessage">Die auszugebende Statusmeldung.</param>
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Me, New NotifyEventArgs(Of String)(StatusMessage))
    End Sub

#Region "Telefonnummern, Telefonnamen"
    Friend Async Sub GetFritzBoxDaten()

        Dim SessionID As String

        Dim FBoxJSON As New JSON
        Dim TelQuery As New List(Of String)
        Dim PhonePorts = New List(Of String)
        Dim FritzBoxJSONTelNr1 As FritzBoxJSONTelNrT1 = Nothing
        Dim FritzBoxJSONTelefone1 As FritzBoxJSONTelefone1 = Nothing
        Dim FritzBoxJSONTelefone2 As FritzBoxJSONTelefone2 = Nothing

        Dim tmpTelNr As Telefonnummer
        Dim tmpTelefon As Telefoniegerät
        Dim tmpTelNrList As TelNrList

        Dim tmpStrArr As String()
        Dim QueryAntwort As String = DfltStringEmpty
        Dim tmpTelData As New Telefonie

#Region "uPnP Phone Port, LKZ, OKZ"

        Dim InPutData As New Hashtable
        Dim OutPutData As Hashtable

        Using fbSOAP As New FritzBoxSOAP
            ' Hole die SessionID für Fritz!Box Query
            OutPutData = fbSOAP.Start(KnownSOAPFile.deviceconfigSCPD, "X_AVM-DE_CreateUrlSID")
            If OutPutData.ContainsKey("NewX_AVM-DE_UrlSID") Then
                NLogger.Debug(OutPutData.Item("NewX_AVM-DE_UrlSID").ToString)
                SessionID = OutPutData.Item("NewX_AVM-DE_UrlSID").ToString
            Else
                SessionID = DfltFritzBoxSessionID
                PushStatus(LogLevel.Error, "Einlesen der Telefondaten: Gegenstelle nicht erreichbar!")
            End If

            If SessionID.AreNotEqual(DfltFritzBoxSessionID) Then

                Dim i As Integer = 1
                ' Ermittle alle Phoneports via X_AVM-DE_GetPhonePort
                ' X_AVM-DE_PhoneName Empty string to disable feature to dial a number.
                ' Examples:
                ' FON1: Telefon
                ' FON2: Telefon
                ' ISDN: ISDN/ DECT Rundruf
                ' DECT: Mobilteil 1 
                InPutData.Add("NewIndex", i)

                Do
                    InPutData.Item("NewIndex") = i
                    OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_GetPhonePort", InPutData)
                    If Not OutPutData.Contains("Error") Then
                        i += 1
                        PhonePorts.Add(OutPutData.Item("NewX_AVM-DE_PhoneName").ToString())
                        PushStatus(LogLevel.Debug, $"Phoneport: {PhonePorts.Last}")
                    End If
                Loop Until OutPutData.Contains("Error")

                ' Landeskennzahl ermitteln: X_AVM-DE_GetVoIPCommonCountryCode
                InPutData.Clear()
                OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_GetVoIPCommonCountryCode")
                tmpTelData.LKZ = OutPutData.Item("NewX_AVM-DE_LKZ").ToString()

                ' Ortskennzahl ermitteln: X_AVM-DE_GetVoIPCommonAreaCode
                InPutData.Clear()
                OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_GetVoIPCommonAreaCode")
                tmpTelData.OKZ = OutPutData.Item("NewX_AVM-DE_OKZ").ToString()

                PushStatus(LogLevel.Debug, $"Kennzahlen: {tmpTelData.LKZ}; {tmpTelData.OKZ}")

                '' Ermittle alle Telefonnummern: X_AVM-DE_GetNumbers
                'InPutData.Clear()
                'OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_GetNumbers")

                'If OutPutData.ContainsKey("Error") Then
                '    NLogger.Error("XML-Telefonnummern konnte nicht heruntergeladen werden.")

                'Else
                '    If OutPutData.ContainsKey("NewNumberList") Then
                '        ' Deserialisiere die Anrufliste

                '        Dim xTel As FritzBoxXMLTelNrList = XmlDeserializeFromString(Of FritzBoxXMLTelNrList)(OutPutData.Item("NewNumberList").ToString())

                '        For Each TelNr In xTel.TelNr
                '            NLogger.Debug($"SOAP TelNr: [{TelNr.Index}] {TelNr.Number} - {TelNr.Type} - {TelNr.Name}")

                '        Next

                '    Else
                '        NLogger.Warn("XML-Telefonnummern konnte nicht heruntergeladen werden.")

                '    End If
                'End If

                ''Numbers to be returned:
                ''• ISDN
                ''• VoIP
                ''• GSM
                ''• POTS
                ''• no number
                ''The used values for Type are: 
                ''• eAllCalls
                ''• eGSM
                ''• eISDN
                ''• eNone
                ''• ePOTS
                ''• eVoIP
            End If
        End Using

#End Region

#Region "JSON Query Telefonnummern"

        If SessionID.AreNotEqual(DfltFritzBoxSessionID) Then

            With TelQuery
                ' POTS Nummer
                .Add(FBoxQueryPOTS)
                ' Mobilnummer
                .Add(FBoxQueryMobile)

                ' FON-Name
                For i = 0 To 2
                    .Add(FBoxQueryFON(i))
                Next

                For i = 0 To 9
                    ' Anrufbeantworter-Nummern
                    .Add(FBoxQueryTAM(i))
                    ' Fax-Nummern
                    .Add(FBoxQueryFAX(i))
                    ' Klassische analoge MSN
                    .Add(FBoxQueryMSN(i))
                    ' VoIP-Nummern
                    .Add(FBoxQueryVOIP(i))
                Next

                ' SIP-Nummern
                .Add(FBoxQuerySIP)

                ' Führt das Fritz!Box Query aus und gibt die ersten Daten der Telefonnummern zurück
                QueryAntwort = Await FritzBoxQuery(SessionID, TelQuery)

                FritzBoxJSONTelNr1 = FBoxJSON.GetFirstValues(QueryAntwort)

                .Clear()
            End With

            With FritzBoxJSONTelNr1
                ' Verarbeite Telefonnummern: MSN, TAM, FAX
                For jdx = 1 To 3
                    Select Case jdx
                        Case 1
                            ' Verarbeite MSN-Nummern
                            tmpStrArr = .MSNList
                        Case 2
                            ' Verarbeite TAM-Nummern (Anrufbeantworter)
                            tmpStrArr = .TAMList
                        Case 3
                            ' Verarbeite FAX-Nummern
                            tmpStrArr = .FAXList
                        Case Else
                            ReDim tmpStrArr(-1)
                    End Select

                    For idx = LBound(tmpStrArr) To UBound(tmpStrArr)
                        If tmpStrArr(idx).IsNotStringEmpty Then

                            tmpTelNr = tmpTelData.AddEigeneTelNr(tmpStrArr(idx))

                            With tmpTelNr
                                .ID0 = idx
                                .Überwacht = True
                                Select Case jdx
                                    Case 1
                                        .Typ.Add(TelTypen.MSN)
                                    Case 2
                                        .Typ.Add(TelTypen.TAM)
                                    Case 3
                                        .Typ.Add(TelTypen.FAX)
                                End Select
                            End With
                            PushStatus(LogLevel.Debug, $"Telefonnummer: {String.Join(", ", tmpTelNr.Typ.ToArray)}; {tmpTelNr.ID0}; {tmpTelNr.ID1}; {tmpTelNr.Unformatiert}")
                        End If
                    Next
                Next

                ' Verarbeite Telefonnummern: SIP
                For Each SIPi As SIPEntry In FritzBoxJSONTelNr1.SIP.Where(Function(SIPNr) CBool(SIPNr.Activated))
                    tmpTelNr = tmpTelData.AddEigeneTelNr(SIPi.Displayname)
                    With tmpTelNr
                        .SIPNode = SIPi.Node.ToUpper
                        .ID0 = SIPi.ID.ToInt
                        .Überwacht = True
                        .Typ.Add(TelTypen.SIP)
                        PushStatus(LogLevel.Debug, $"Telefonnummer: {String.Join(", ", tmpTelNr.Typ.ToArray)}; {tmpTelNr.ID0}; {tmpTelNr.ID1}; {tmpTelNr.Unformatiert}")
                    End With
                Next

                ' Verarbeite Telefonnummern: POTS
                If .POTS.IsNotStringEmpty Then
                    tmpTelNr = tmpTelData.AddEigeneTelNr(FritzBoxJSONTelNr1.POTS)
                    With tmpTelNr
                        .Überwacht = True
                        .Typ.Add(TelTypen.POTS)
                        PushStatus(LogLevel.Debug, $"Telefonnummer: {String.Join(", ", tmpTelNr.Typ.ToArray)}; {tmpTelNr.ID0}; {tmpTelNr.ID1}; {tmpTelNr.Unformatiert}")
                    End With
                End If

                ' Verarbeite Telefonnummern: Mobil
                If .Mobile.IsNotStringEmpty Then
                    tmpTelNr = tmpTelData.AddEigeneTelNr(FritzBoxJSONTelNr1.Mobile)
                    With tmpTelNr
                        .Überwacht = True
                        .Typ.Add(TelTypen.Mobil)
                        PushStatus(LogLevel.Debug, $"Telefonnummer: {String.Join(", ", tmpTelNr.Typ.ToArray)}; {tmpTelNr.ID0}; {tmpTelNr.ID1}; {tmpTelNr.Unformatiert}")
                    End With
                End If
            End With

            ' Verarbeite Telefonnummern über die angeschlossenen Geräte
            For kdx = 0 To 1
                Select Case kdx
                    Case 0
                        tmpStrArr = FritzBoxJSONTelNr1.MSNPortEnabled
                    Case 1
                        tmpStrArr = FritzBoxJSONTelNr1.VOIPPortEnabled
                    Case Else
                        ReDim tmpStrArr(-1)
                End Select

                For idx = LBound(tmpStrArr) To UBound(tmpStrArr)
                    If (kdx.IsZero And tmpStrArr(idx).IsNotStringEmpty) OrElse (kdx.AreEqual(1) And tmpStrArr(idx).AreEqual("1")) Then

                        ' Füge alle 10 möglichen zugeordneten Nummern hinzu
                        TelQuery.Clear()
                        For jdx = 0 To 9
                            Select Case kdx
                                Case 0
                                    TelQuery.Add(FBoxQueryMSNTelNrList(idx, jdx))
                                Case 1
                                    TelQuery.Add(FBoxQueryVOIPTelNrList(idx, jdx))
                            End Select
                        Next

                        ' Pro Gerät erfolgt eine Abfrage an die Fritz!Box
                        QueryAntwort = Await FritzBoxQuery(SessionID, TelQuery)
                        tmpTelNrList = FBoxJSON.GetTelNrListJSON(QueryAntwort)

                        With tmpTelNrList
                            For jdx = .LBound To .UBound
                                If .Item(jdx).IsNotStringEmpty Then
                                    ' Überprüfe ob die übergebene Teefonnummer eine SIP-Zeichenfolge entspricht: SIP0, SIP1 etc
                                    If .Item(jdx).IsRegExMatch("^SIP\d+$", RegularExpressions.RegexOptions.IgnoreCase) Then
                                        ' Finde die Telefonnummer anhand der SIP-Node
                                        Dim j As Integer = jdx ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results
                                        tmpTelNr = tmpTelData.Telefonnummern.Find(Function(Nummern) Nummern.SIPNode.AreEqual(.Item(j)) And Nummern.Typ.Contains(TelTypen.SIP))
                                    Else
                                        tmpTelNr = tmpTelData.AddEigeneTelNr(tmpTelNrList.Item(jdx))
                                        With tmpTelNr
                                            .ID0 = jdx
                                            .ID1 = idx
                                        End With
                                    End If

                                    Select Case kdx
                                        Case 0
                                            tmpTelNr.Typ.Add(TelTypen.MSN)
                                        Case 1
                                            tmpTelNr.Typ.Add(TelTypen.IP)
                                    End Select
                                End If
                            Next
                        End With
                    End If
                Next
            Next

            With TelQuery
                .Clear()
                .Add(FBoxQueryFONList)       ' FON
                .Add(FBoxQueryDECTList)      ' DECT (Teil1)
                .Add(FBoxQueryVOIP)          ' IP-Telefoen
                .Add(FBoxQueryTAMList)       ' TAM

                For idx = 1 To 8
                    .Add(FBoxQueryS0("Name", idx))
                Next
            End With 'TelQuery

            QueryAntwort = Await FritzBoxQuery(SessionID, TelQuery)
            FritzBoxJSONTelefone1 = FBoxJSON.GetSecondValues(QueryAntwort)

            With FritzBoxJSONTelefone1
                TelQuery.Clear()
                For idx = LBound(.S0NameList) To UBound(.S0NameList)
                    If .S0NameList(idx).IsNotStringEmpty Then
                        TelQuery.Add(FBoxQueryS0("Number", idx + 1))
                        TelQuery.Add(FBoxQueryS0("Type", idx + 1))
                    End If
                Next

                For idx = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                    If FritzBoxJSONTelefone1.DECT(idx).Intern.IsNotStringEmpty Then
                        TelQuery.Add(FBoxQueryDECTRingOnAllMSNs(idx))
                        TelQuery.Add(FBoxQueryDECTNrList(idx))
                    End If
                Next

                TelQuery.Add(FBoxQueryFaxMailActive)
                TelQuery.Add(FBoxQueryMobileName)

            End With

            QueryAntwort = Await FritzBoxQuery(SessionID, TelQuery)

#End Region
#Region "JSON Query Telefoniegeräte"

            FritzBoxJSONTelefone2 = FBoxJSON.GetThirdValues(QueryAntwort)

            ' Verarbeitung der Telefone: FON
            For idx = LBound(FritzBoxJSONTelefone1.FON) To UBound(FritzBoxJSONTelefone1.FON)
                With FritzBoxJSONTelefone1.FON(idx)
                    If .Name.IsNotStringEmpty Then
                        tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.FON,
                                                              .Dialport = DialPortBase.FON + idx,
                                                              .AnrMonID = idx}
                        tmpTelefon.IsFax = CBool(.Fax)
                        tmpTelefon.Name = .Name
                        tmpTelefon.UPnPDialport = PhonePorts.Find(Function(PhonePort) PhonePort.StartsWith("FON") AndAlso PhonePort.EndsWith(tmpTelefon.Name))
                        ' Fallback
                        If tmpTelefon.UPnPDialport.IsNotStringNothingOrEmpty Then tmpTelefon.SetUPnPDialportFallback()

                        Dim j As Integer = idx ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results

                        tmpTelefon.StrEinTelNr = New List(Of String)

                        For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.ID0.AreEqual(j) And Nummern.Typ.Contains(TelTypen.MSN))
                            tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                        Next

                        PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                        tmpTelData.Telefoniegeräte.Add(tmpTelefon)
                    End If
                End With
            Next

            ' Verarbeitung der Telefone: DECT
            For idx = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                With FritzBoxJSONTelefone1.DECT(idx)

                    If .Name.IsNotStringEmpty Then

                        tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.DECT,
                                                              .IsFax = False}

                        tmpTelefon.AnrMonID = AnrMonTelIDBase.DECT + CInt(Right(.Intern, 1))
                        tmpTelefon.Dialport = DialPortBase.DECT + CInt(Right(.Intern, 1))
                        tmpTelefon.Name = .Name
                        tmpTelefon.UPnPDialport = PhonePorts.Find(Function(PhonePort) PhonePort.StartsWith("DECT") AndAlso PhonePort.EndsWith(tmpTelefon.Name))
                        ' Fallback
                        If tmpTelefon.UPnPDialport.IsNotStringNothingOrEmpty Then tmpTelefon.SetUPnPDialportFallback()

                        If FritzBoxJSONTelefone2.DECTRingOnAllMSNs(idx).AreEqual("1") Then
                            tmpTelefon.StrEinTelNr = New List(Of String)
                            For Each TelNr In tmpTelData.Telefonnummern.Distinct
                                tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                            Next
                        Else
                            For Each aktDECTNr In FritzBoxJSONTelefone2.DECTTelNr(idx)
                                If aktDECTNr.Number.IsNotStringEmpty Then
                                    tmpTelefon.StrEinTelNr = New List(Of String)

                                    For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Equals(aktDECTNr.Number))
                                        tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                                    Next
                                End If
                            Next
                        End If

                        PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                        tmpTelData.Telefoniegeräte.Add(tmpTelefon)
                    End If
                End With
            Next

            ' Verarbeitung der Telefone: IP-Telefone
            For idx = LBound(FritzBoxJSONTelefone1.VOIP) To UBound(FritzBoxJSONTelefone1.VOIP)
                With FritzBoxJSONTelefone1.VOIP(idx)
                    If .Enabled.AreEqual("1") Then
                        tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.IP,
                                                              .Dialport = DialPortBase.IP + idx}
                        tmpTelefon.Name = .Name
                        tmpTelefon.StrEinTelNr = New List(Of String)

                        For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.ID1.AreEqual(.Node.RegExRemove("\D").ToInt) And Nummern.Typ.Contains(TelTypen.IP))
                            tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                        Next

                        PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                        tmpTelData.Telefoniegeräte.Add(tmpTelefon)
                    End If
                End With
            Next

            ' Verarbeitung der Telefone: S0
            For idx = 0 To 7
                If FritzBoxJSONTelefone1.S0NameList(idx).IsNotStringEmpty And FritzBoxJSONTelefone2.S0NumberList(idx).IsNotStringEmpty Then
                    tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.S0,
                                                          .Dialport = DialPortBase.S0 + idx + 1,
                                                          .AnrMonID = .Dialport}

                    tmpTelefon.Name = FritzBoxJSONTelefone1.S0NameList(idx)
                    tmpTelefon.UPnPDialport = PhonePorts.Find(Function(PhonePort) PhonePort.StartsWith("ISDN") AndAlso PhonePort.EndsWith(tmpTelefon.Name))
                    ' Fallback
                    If tmpTelefon.UPnPDialport.IsNotStringNothingOrEmpty Then tmpTelefon.SetUPnPDialportFallback()

                    Dim j = idx ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results

                    tmpTelefon.StrEinTelNr = New List(Of String)
                    For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.Equals(FritzBoxJSONTelefone2.S0NumberList(j)))
                        tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                    Next
                    tmpTelData.Telefoniegeräte.Add(tmpTelefon)
                End If
            Next

            If tmpTelData.Telefoniegeräte.Find(Function(Telefon) Telefon.TelTyp = TelTypen.S0 Or Telefon.TelTyp = TelTypen.DECT) IsNot Nothing Then
                tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.S0,
                                                  .Dialport = DialPortBase.S0,
                                                  .AnrMonID = .Dialport,
                                                  .Name = "ISDN/DECT Rundruf"}

                tmpTelefon.UPnPDialport = PhonePorts.Find(Function(PhonePort) PhonePort.StartsWith("ISDN") AndAlso PhonePort.EndsWith(tmpTelefon.Name)) '
                ' Fallback
                If tmpTelefon.UPnPDialport.IsNotStringNothingOrEmpty Then tmpTelefon.SetUPnPDialportFallback()

                PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                tmpTelData.Telefoniegeräte.Add(tmpTelefon)
            End If

            ' Verarbeitung der Telefone: TAM, Anrufbeantworter
            For idx = LBound(FritzBoxJSONTelefone1.TAM) To UBound(FritzBoxJSONTelefone1.TAM)
                With FritzBoxJSONTelefone1.TAM(idx)
                    If .Active.AreEqual("1") Then
                        Dim j As Integer = idx ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results
                        tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.TAM,
                                                              .Dialport = DialPortBase.TAM + j}
                        tmpTelefon.Name = .Name

                        tmpTelefon.StrEinTelNr = New List(Of String)
                        If tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.Typ.Contains(TelTypen.TAM)).Count.IsZero Then
                            For Each TelNr In tmpTelData.Telefonnummern.Distinct
                                tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                            Next
                        Else
                            For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Typ.Contains(TelTypen.TAM) And Nummer.ID0.AreEqual(j))
                                tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                            Next
                        End If

                        PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                        tmpTelData.Telefoniegeräte.Add(tmpTelefon)
                    End If
                End With
            Next

            ' Verarbeitung der Telefone: integrierter Faxempfang
            If FritzBoxJSONTelefone2.FaxMailActive.AreNotEqual("0") Then
                tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.FAX,
                                                      .Dialport = DialPortBase.Fax,
                                                      .AnrMonID = .Dialport,
                                                      .Name = "Faxempfang",
                                                      .IsFax = True}

                tmpTelefon.StrEinTelNr = New List(Of String)

                For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Typ.Contains(TelTypen.FAX))
                    tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                Next

                PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                tmpTelData.Telefoniegeräte.Add(tmpTelefon)
            End If

            ' Verarbeitung der Telefone: Mobil
            If tmpTelData.Telefonnummern.Find(Function(Nummer) Nummer.Typ.Contains(TelTypen.Mobil)) IsNot Nothing Then
                tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.Mobil,
                                                      .Dialport = DfltMobilDialPort,
                                                      .AnrMonID = .Dialport,
                                                      .Name = FritzBoxJSONTelefone2.MobileName,
                                                      .IsFax = False}

                tmpTelefon.StrEinTelNr = New List(Of String)
                For Each TelNr In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Typ.Contains(TelTypen.Mobil))
                    tmpTelefon.StrEinTelNr.Add(TelNr.Einwahl)
                Next

                PushStatus(LogLevel.Debug, $"Telefon: {tmpTelefon.AnrMonID}; {tmpTelefon.Dialport}; {tmpTelefon.UPnPDialport}; {tmpTelefon.Name}")
                tmpTelData.Telefoniegeräte.Add(tmpTelefon)
            End If

            XMLData.PTelefonie = tmpTelData
        End If
#End Region

        ' Aufräumen
        TelQuery.Clear()
        PushStatus(LogLevel.Debug, $"Einlesen der Telefoniedaten abgeschlossen...")
        RaiseEvent Beendet()
    End Sub
#End Region

#Region "Fritz!Box Query"
    Private Async Function FritzBoxQuery(SessionID As String, Abfrage As List(Of String)) As Threading.Tasks.Task(Of String)
        Return Await HTTPGet($"{FBLinkBasis}/query.lua?{SessionID}&{String.Join("&", Abfrage.ToArray)}", Encoding.GetEncoding(DfltCodePageFritzBox))
    End Function
#End Region
End Class