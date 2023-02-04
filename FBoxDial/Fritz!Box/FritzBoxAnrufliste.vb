Imports System.Threading.Tasks

Friend Module FritzBoxAnrufliste
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste(Optional ID As Integer = 0, Optional TimeStamp As Integer = 0) As Task(Of FBoxAPI.CallList)
        ' Ermittle Pfad zur Anrufliste
        If Globals.ThisAddIn.FBoxTR064?.Ready Then
            Dim Anrufliste = Await Globals.ThisAddIn.FBoxTR064.X_contact.GetCallList(id:=ID, timestamp:=TimeStamp)

            ' CallList TimeStamp merken (0 ist ungültig)
            If Anrufliste.TimeStamp.IsNotZero Then XMLData.POptionen.FBoxCallListTimeStamp = Anrufliste.TimeStamp

            Return Anrufliste
        Else
            NLogger.Warn("Pfad zur XML-Anrufliste konnte nicht ermittelt werden.")
            Return New FBoxAPI.CallList
        End If
    End Function
#End Region

#Region "Anrufliste auswerten"
    ''' <summary>
    ''' Manueller Import der Telefonate aus der Anrufliste
    ''' </summary>
    Friend Async Function SetUpOutlookListen(Anrufliste As IEnumerable(Of FBoxAPI.Call), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
        Return Await Task.Run(Async Function()
                                  Dim Einträge As Integer = 0

                                  For Each Anruf In Anrufliste
                                      ' Journaleintrag erstellen
                                      Using t = Await ErstelleTelefonat(Anruf)
                                          If t IsNot Nothing Then t.SetUpOlLists(False)
                                      End Using

                                      ' Zählvariable hochsetzen
                                      Einträge += 1

                                      ' Status weitergeben
                                      progress.Report(1)

                                      ' Abbruch überwachen
                                      If ct.IsCancellationRequested Then Exit For
                                  Next

                                  Return Einträge
                              End Function, ct)

    End Function
#End Region

    Friend Async Function ErstelleTelefonat([Call] As FBoxAPI.Call) As Task(Of Telefonat)

        If [Call].Type.IsLessOrEqual(3) Or [Call].Type.AreEqual(10) Then

            Dim tmpTelefonat As New Telefonat With {.Import = True,
                                                    .ID = [Call].ID,
                                                    .NebenstellenNummer = [Call].Port,
                                                    .ZeitBeginn = CDate([Call].[Date].ToString)}

            With tmpTelefonat

                If [Call].Type.AreEqual(1) Or [Call].Type.AreEqual(3) Then ' incoming, outgoing
                    ' Es wird auch nach dem Namen des Gerätes gesucht, wenn über den Port nichts gefunden wurde.
                    .SetTelefoniegerät([Call].Device)

                    ' Umwandlung von "hh:mm" in Sekundenwert
                    With CDate([Call].Duration)
                        tmpTelefonat.Dauer = New TimeSpan(.Hour, .Minute, .Second).TotalSeconds.ToInt
                    End With

                    ' Das Flag, ob ein Telefonat angenommen wurde, wird anhand der Dauer festgelegt.
                    .Angenommen = .Dauer.IsNotZero

                    ' Falls es sich um ein TAM handelt: Setze Flag, dass das Telefonat nicht angenommen wurde.
                    If .TelGerät IsNot Nothing AndAlso .TelGerät.IsTAM AndAlso XMLData.POptionen.CBIsTAMMissed Then
                        .Angenommen = True
                    End If

                End If

                If [Call].Type.AreEqual(1) Or [Call].Type.AreEqual(2) Or [Call].Type.AreEqual(10) Then ' incoming, missed, rejected
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Eingehend

                    ' Own Number of called party (incoming call)
                    .EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr([Call].CalledNumber)

                    ' Falls keine Nummer übereinstimmt, dann setze den tmpTelNr
                    If .EigeneTelNr Is Nothing Then
                        NLogger.Warn($"Eigene Nummer '{[Call].CalledNumber}' ist nicht bekannt (ID: { .ID}).")
                        .EigeneTelNr = New Telefonnummer With {.SetNummer = [Call].CalledNumber}
                    End If

                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert

                    ' Number of calling party 
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = [Call].Caller}
                    .NrUnterdrückt = .GegenstelleTelNr.Unterdrückt

                    ' Merke den Pfad zur TAM-Message bzw. FAX nachricht
                    .TAMMessagePath = [Call].Path

                End If

                If [Call].Type.AreEqual(3) Then 'outgoing
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Ausgehend
                    ' Own Number of called party (outgoing call) 
                    .EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr([Call].CallerNumber)

                    ' Falls keine Nummer übereinstimmt, dann setze den tmpTelNr
                    If .EigeneTelNr Is Nothing Then
                        NLogger.Warn($"Eigene Nummer '{[Call].CallerNumber}' ist nicht bekannt (ID: { .ID}).")
                        .EigeneTelNr = New Telefonnummer With {.SetNummer = [Call].CallerNumber}
                    End If

                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert

                    ' Number or name of called party  
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = [Call].Called}

                End If

                ' Anrufer ermitteln
                If [Call].Name.IsNotStringNothingOrEmpty Then .AnruferName = [Call].Name

                If .GegenstelleTelNr IsNot Nothing AndAlso Not .GegenstelleTelNr.Unterdrückt Then
                    Await .KontaktSucheTask()
                End If

                If [Call].Type.AreEqual(2) Or [Call].Type.AreEqual(10) Then .Angenommen = False ' missed, rejected

                If [Call].Type.AreEqual(10) Then .Blockiert = True ' rejected

                If [Call].Type.AreEqual(9) Or [Call].Type.AreEqual(10) Or [Call].Type.AreEqual(11) Then
                    ' 9 active incoming,
                    ' 11 active outgoing 

                    ' Hier könnte mal erfasst werden, was mit aktiven Gesprächen geschehen soll. 
                End If
            End With

            Return tmpTelefonat
        Else
            Return Nothing
        End If
    End Function

End Module
