Imports FBoxDial.DfltWerteTelefonie
Imports FBoxDial.FritzBoxDefault
Imports System.Threading.Tasks
Public Class FritzBoxData
    Private Shared Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Public Sub New()
        If XMLData IsNot Nothing Then

            ' Gültige IP-Adresse für die Fritz!Box ablegen
            XMLData.POptionen.PValidFBAdr = ValidIP(XMLData.POptionen.PTBFBAdr)

        End If
    End Sub

#Region "Telefonnummern, Telefonnamen"
    Friend Async Sub FritzBoxDatenJSON()

        Dim SessionID As String = GetSessionID

        Dim FBoxJSON As New JSON
        Dim TelQuery As New List(Of String)
        Dim FritzBoxJSONTelNr1 As FritzBoxJSONTelNrT1 = Nothing
        Dim FritzBoxJSONTelefone1 As FritzBoxJSONTelefone1 = Nothing
        Dim FritzBoxJSONTelefone2 As FritzBoxJSONTelefone2 = Nothing

        Dim tmpTelNr As Telefonnummer
        Dim tmpTelefon As Telefoniegerät
        Dim tmpTelNrList As TelNrList

        Dim tmpStrArr As String()
        Dim QueryAntwort As String
        Dim tmpTelData As New Telefonie

        ' Boolean, der das Abbrechen des Einlesens signalisiert
        Dim Fortfahren As Boolean = True

        Using fbQuery As New FritzBoxQuery
            With TelQuery
                '.Add(  P_Query_FB_LKZPrefix)
                .Add(P_Query_FB_LKZ)
                '.Add(P_Query_FB_OKZPrefix)
                .Add(P_Query_FB_OKZ)
                QueryAntwort = Await fbQuery.FritzBoxQuery(SessionID, TelQuery)

                ' Überprüfung, ob Verbindung zur Fritz!Box besteht.

                Fortfahren = QueryAntwort.AreNotEqual("Gegenstelle nicht erreichbar!")
                If Fortfahren Then
                    With FBoxJSON.GetLocalValues(QueryAntwort)
                        XMLData.POptionen.PTBOrtsKZ = .OKZ
                        XMLData.POptionen.PTBLandesKZ = .LKZ
                        NLogger.Debug("Kennzahlen: {0}; {1}", .OKZ, .LKZ)
                    End With
                Else
                    NLogger.Error("Einlesen der Telefondaten: Gegenstelle nicht erreichbar!")
                End If
                .Clear()
            End With

            If Fortfahren Then

                With TelQuery
                    ' POTS Nummer
                    .Add(P_Query_FB_POTS)
                    ' Mobilnummer
                    .Add(P_Query_FB_Mobile)

                    ' FON-Name
                    For i = 0 To 2
                        .Add(P_Query_FB_FON(i))
                    Next

                    For i = 0 To 9
                        ' Anrufbeantworter-Nummern
                        .Add(P_Query_FB_TAM(i))
                        ' Fax-Nummern
                        .Add(P_Query_FB_FAX(i))
                        ' Klassische analoge MSN
                        .Add(P_Query_FB_MSN(i))
                        ' VoIP-Nummern
                        .Add(P_Query_FB_VOIP(i))
                    Next

                    ' SIP-Nummern
                    .Add(P_Query_FB_SIP)

                    ' Führt das Fritz!Box Query aus und gibt die ersten Daten der Telefonnummern zurück
                    QueryAntwort = Await fbQuery.FritzBoxQuery(SessionID, TelQuery)

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

                                tmpTelNr = tmpTelData.AddNewTelNrStr(tmpStrArr(idx))

                                With tmpTelNr
                                    .ID0 = idx
                                    .EigeneNummer = True
                                    Select Case jdx
                                        Case 1
                                            .Typ.Add(TelTypen.MSN)
                                        Case 2
                                            .Typ.Add(TelTypen.TAM)
                                        Case 3
                                            .Typ.Add(TelTypen.FAX)
                                    End Select
                                End With
                                NLogger.Debug("Telefonnummer: {0}; {1}; {2}; {3}", tmpTelNr.Typ, tmpTelNr.ID0, tmpTelNr.ID1, tmpTelNr.Unformatiert)
                            End If
                        Next
                    Next

                    ' Verarbeite Telefonnummern: SIP
                    For Each SIPi As SIPEntry In FritzBoxJSONTelNr1.SIP.Where(Function(SIPNr) CBool(SIPNr.Activated))
                        tmpTelNr = tmpTelData.AddNewTelNrStr(SIPi.Displayname)
                        With tmpTelNr
                            .SIPNode = SIPi.Node.ToUpper
                            .ID0 = SIPi.ID.ToInt
                            .EigeneNummer = True
                            .Typ.Add(TelTypen.SIP)
                            NLogger.Debug("Telefonnummer: {0}; {1}; {2}; {3}", tmpTelNr.Typ, tmpTelNr.ID0, tmpTelNr.ID1, tmpTelNr.Unformatiert)
                        End With
                    Next

                    ' Verarbeite Telefonnummern: POTS
                    If .POTS.IsNotStringEmpty Then
                        tmpTelNr = tmpTelData.AddNewTelNrStr(FritzBoxJSONTelNr1.POTS)
                        With tmpTelNr
                            .EigeneNummer = True
                            .Typ.Add(TelTypen.POTS)
                            NLogger.Debug("Telefonnummer: {0}; {1}; {2}; {3}", tmpTelNr.Typ, tmpTelNr.ID0, tmpTelNr.ID1, tmpTelNr.Unformatiert)
                        End With
                    End If

                    ' Verarbeite Telefonnummern: Mobil
                    If .Mobile.IsNotStringEmpty Then
                        tmpTelNr = tmpTelData.AddNewTelNrStr(FritzBoxJSONTelNr1.Mobile)
                        With tmpTelNr
                            .EigeneNummer = True
                            .Typ.Add(TelTypen.Mobil)
                            NLogger.Debug("Telefonnummer: {0}; {1}; {2}; {3}", tmpTelNr.Typ, tmpTelNr.ID0, tmpTelNr.ID1, tmpTelNr.Unformatiert)
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
                                        TelQuery.Add(P_Query_FB_MSN_TelNrList(idx, jdx))
                                    Case 1
                                        TelQuery.Add(P_Query_FB_VOIP_TelNrList(idx, jdx))
                                End Select
                            Next

                            ' Pro Gerät erfolgt eine Abfrage an die Fritz!Box
                            QueryAntwort = Await fbQuery.FritzBoxQuery(SessionID, TelQuery)
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
                                            tmpTelNr = tmpTelData.AddNewTelNrStr(tmpTelNrList.Item(jdx))
                                            With tmpTelNr
                                                .EigeneNummer = True
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
                    .Add(P_Query_FB_FON_List)       ' FON
                    .Add(P_Query_FB_DECT_List)      ' DECT (Teil1)
                    .Add(P_Query_FB_VOIP_List)      ' IP-Telefoen
                    .Add(P_Query_FB_TAM_List)       ' TAM

                    For idx = 1 To 8
                        .Add(P_Query_FB_S0("Name", idx))
                    Next
                End With 'TelQuery

                QueryAntwort = Await fbQuery.FritzBoxQuery(SessionID, TelQuery)
                FritzBoxJSONTelefone1 = FBoxJSON.GetSecondValues(QueryAntwort)

                With FritzBoxJSONTelefone1
                    TelQuery.Clear()
                    For idx = LBound(.S0NameList) To UBound(.S0NameList)
                        If .S0NameList(idx).IsNotStringEmpty Then
                            TelQuery.Add(P_Query_FB_S0("Number", idx + 1))
                            TelQuery.Add(P_Query_FB_S0("Type", idx + 1))
                        End If
                    Next

                    For idx = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                        If FritzBoxJSONTelefone1.DECT(idx).Intern.IsNotStringEmpty Then
                            TelQuery.Add(P_Query_FB_DECT_RingOnAllMSNs(idx))
                            TelQuery.Add(P_Query_FB_DECT_NrList(idx))
                        End If
                    Next

                    TelQuery.Add(P_Query_FB_FaxMailActive)
                    TelQuery.Add(P_Query_FB_MobileName)

                End With

                QueryAntwort = Await fbQuery.FritzBoxQuery(SessionID, TelQuery)
            End If
        End Using

        If Fortfahren Then
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
                        tmpTelefon.UPnPDialport = String.Format("FON{0}: {1}", tmpTelefon.Dialport, tmpTelefon.Name)

                        Dim j As Integer = idx ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results
                        tmpTelefon.StrEinTelNr = New List(Of String)
                        For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.ID0.AreEqual(j) And Nummern.Typ.Contains(TelTypen.MSN))
                            tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                        Next
                        NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)
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
                        tmpTelefon.UPnPDialport = String.Format("DECT: {0}", tmpTelefon.Name)

                        If FritzBoxJSONTelefone2.DECTRingOnAllMSNs(idx).AreEqual("1") Then
                            tmpTelefon.StrEinTelNr = New List(Of String)
                            For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.Distinct
                                tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                            Next
                        Else
                            For Each aktDECTNr As DECTNr In FritzBoxJSONTelefone2.DECTTelNr(idx)
                                If aktDECTNr.Number.IsNotStringEmpty Then
                                    tmpTelefon.StrEinTelNr = New List(Of String)
                                    For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Equals(aktDECTNr.Number))
                                        tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                                    Next
                                End If
                            Next
                        End If
                        NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)

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
                        For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.ID1.AreEqual(.Node.RegExReplace("\D", PDfltStringEmpty).ToInt) And Nummern.Typ.Contains(TelTypen.IP))
                            tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                        Next
                        NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)
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
                    tmpTelefon.UPnPDialport = String.Format("ISDN: {0}", tmpTelefon.Name)

                    Dim j = idx ' Vermeide Fehler: BC42324 Using the iteration variable in a lambda expression may have unexpected results

                    tmpTelefon.StrEinTelNr = New List(Of String)
                    For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummern) Nummern.Equals(FritzBoxJSONTelefone2.S0NumberList(j)))
                        tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                    Next
                    tmpTelData.Telefoniegeräte.Add(tmpTelefon)
                End If
            Next
            If tmpTelData.Telefoniegeräte.Find(Function(Telefon) Telefon.TelTyp = TelTypen.S0 Or Telefon.TelTyp = TelTypen.DECT) IsNot Nothing Then
                tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.S0,
                                                  .Dialport = DialPortBase.S0,
                                                  .AnrMonID = .Dialport,
                                                  .Name = "ISDN- und Schnurlostelefone",
                                                  .UPnPDialport = "ISDN und Schnurlostelefone"}           '

                NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)
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
                            For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.Distinct
                                tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                            Next
                        Else
                            For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Typ.Contains(TelTypen.TAM) And Nummer.ID0.AreEqual(j))
                                tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                            Next
                        End If
                        NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)
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
                For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Typ.Contains(TelTypen.FAX))
                    tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                Next
                NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)
                tmpTelData.Telefoniegeräte.Add(tmpTelefon)
            End If

            ' Verarbeitung der Telefone: Mobil
            If tmpTelData.Telefonnummern.Find(Function(Nummer) Nummer.Typ.Contains(TelTypen.Mobil)) IsNot Nothing Then
                tmpTelefon = New Telefoniegerät With {.TelTyp = TelTypen.Mobil,
                                                      .Dialport = PDfltMobilDialPort,
                                                      .AnrMonID = .Dialport,
                                                      .Name = FritzBoxJSONTelefone2.MobileName,
                                                      .IsFax = False}

                tmpTelefon.StrEinTelNr = New List(Of String)
                For Each TelNr As Telefonnummer In tmpTelData.Telefonnummern.FindAll(Function(Nummer) Nummer.Typ.Contains(TelTypen.Mobil))
                    tmpTelefon.StrEinTelNr.Add(TelNr.Unformatiert)
                Next
                NLogger.Debug("Telefon: {0}; {1}; {2}; {3}", tmpTelefon.AnrMonID, tmpTelefon.Dialport, tmpTelefon.UPnPDialport, tmpTelefon.Name)
                tmpTelData.Telefoniegeräte.Add(tmpTelefon)
            End If

            XMLData.PTelefonie = tmpTelData
        End If
        ' Aufräumen
        TelQuery.Clear()

    End Sub
#End Region
End Class