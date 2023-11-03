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

                                  ' Wirf alle doppelten Einträge raus
                                  TelefonieListen.DistictList()

                                  Return Einträge
                              End Function, ct)

    End Function
#End Region

    ''' <summary>
    ''' Erstellt ein Telefonat-Objekt aus einem Eintrag der Fritz!Box Anrufliste
    ''' </summary>
    ''' <param name="Anruf">Der Anruf, der umgewandelt werden soll.</param>
    Friend Async Function ErstelleTelefonat(Anruf As FBoxAPI.Call) As Task(Of Telefonat)

        ' TODO: Temporäres Ausgeben des aktuellen Anrufes 
        Dim r As String = String.Empty
        If XmlSerializeToString(Anruf, r) Then NLogger.Debug($"Aktueller Anruf: {r}")

        If Anruf.Type.IsLessOrEqual(3) Or Anruf.Type.AreEqual(10) Then

            Dim tmpTelefonat As New Telefonat With {.Import = True,
                                                    .ID = Anruf.ID,
                                                    .NebenstellenNummer = Anruf.Port,
                                                    .ZeitBeginn = CDate(Anruf.Date.ToString)}

            With tmpTelefonat

                If Anruf.Type.AreEqual(1) Or Anruf.Type.AreEqual(3) Then ' incoming, outgoing
                    ' Es wird auch nach dem Namen des Gerätes gesucht, wenn über den Port nichts gefunden wurde.
                    .SetTelefoniegerät(Anruf.Device)

                    ' Umwandlung von "hh:mm" in Sekundenwert
                    With CDate(Anruf.Duration)
                        tmpTelefonat.Dauer = New TimeSpan(.Hour, .Minute, .Second).TotalSeconds.ToInt
                    End With

                    ' Das Flag, ob ein Telefonat angenommen wurde, wird anhand der Dauer festgelegt.
                    .Angenommen = .Dauer.IsNotZero

                    ' Falls es sich um ein TAM handelt: Setze Flag, dass das Telefonat nicht angenommen wurde.
                    If .TelGerät IsNot Nothing AndAlso .TelGerät.IsTAM AndAlso XMLData.POptionen.CBIsTAMMissed Then
                        .Angenommen = True
                    End If

                End If

                If Anruf.Type.AreEqual(1) Or Anruf.Type.AreEqual(2) Or Anruf.Type.AreEqual(10) Then ' incoming, missed, rejected
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Eingehend

                    ' Own Number of called party (incoming call)
                    .EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr(Anruf.CalledNumber)

                    ' Falls keine Nummer übereinstimmt, dann setze den tmpTelNr
                    If .EigeneTelNr Is Nothing Then
                        NLogger.Warn($"Eigene Nummer '{Anruf.CalledNumber}' ist nicht bekannt (ID: { .ID}).")
                        .EigeneTelNr = New Telefonnummer With {.SetNummer = Anruf.CalledNumber}
                    End If

                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert

                    ' Number of calling party 
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = Anruf.Caller}
                    .NrUnterdrückt = .GegenstelleTelNr.Unterdrückt

                    ' Merke den Pfad zur TAM-Message bzw. FAX nachricht
                    .TAMMessagePath = Anruf.Path

                End If

                If Anruf.Type.AreEqual(3) Then 'outgoing
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Ausgehend
                    ' Own Number of called party (outgoing call) 
                    .EigeneTelNr = XMLData.PTelefonie.GetEigeneTelNr(Anruf.CallerNumber)

                    ' Falls keine Nummer übereinstimmt, dann setze den tmpTelNr
                    If .EigeneTelNr Is Nothing Then
                        NLogger.Warn($"Eigene Nummer '{Anruf.CallerNumber}' ist nicht bekannt (ID: { .ID}).")
                        .EigeneTelNr = New Telefonnummer With {.SetNummer = Anruf.CallerNumber}
                    End If

                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert

                    ' Number or name of called party  
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = Anruf.Called}

                End If

                ' Anrufer ermitteln
                If Anruf.Name.IsNotStringNothingOrEmpty Then .AnruferName = Anruf.Name

                If .GegenstelleTelNr IsNot Nothing AndAlso Not .GegenstelleTelNr.Unterdrückt Then
                    Await .KontaktSucheTask(False)
                End If

                If Anruf.Type.AreEqual(2) Or Anruf.Type.AreEqual(10) Then .Angenommen = False ' missed, rejected

                If Anruf.Type.AreEqual(10) Then .Blockiert = True ' rejected

                If Anruf.Type.AreEqual(9) Or Anruf.Type.AreEqual(10) Or Anruf.Type.AreEqual(11) Then
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
