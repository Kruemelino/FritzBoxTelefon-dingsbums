﻿Imports System.Threading.Tasks

Friend Module FritzBoxAnrufliste
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste(FBoxTR064 As FBoxAPI.FritzBoxTR64) As Task(Of FBoxAPI.CallList)
        Dim Anrufliste As New FBoxAPI.CallList With {.Calls = New List(Of FBoxAPI.Call)}
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Dim Pfad As String = DfltStringEmpty

            ' Ermittle Pfad zur Anrufliste
            If FBoxTR064.Bereit AndAlso FBoxTR064.X_contact.GetCallList(Pfad) Then
                With Await DeserializeAsyncXML(Of FBoxAPI.CallList)(Pfad, True)
                    Anrufliste.Calls.AddRange(.Calls)
                    Anrufliste.Timestamp = .Timestamp
                End With

            Else
                NLogger.Warn("Pfad zur XML-Anrufliste konnte nicht ermittelt werden.")
            End If
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        End If
        Return Anrufliste
    End Function
#End Region

#Region "Anrufliste auswerten"
    Friend Async Function SetUpOutlookListen(Anrufliste As IEnumerable(Of FBoxAPI.Call), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
        Return Await Task.Run(Async Function()
                                  Dim Einträge As Integer = 0

                                  For Each Anruf In Anrufliste
                                      ' Journaleintrag erstellen
                                      Using t = Await ErstelleTelefonat(Anruf)
                                          If t IsNot Nothing Then t.SetUpOlLists()
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

            Dim tmpTelefonat As New Telefonat With {.Import = True, .ID = [Call].ID, .ZeitBeginn = CDate([Call].[Date].ToString)}
            Dim tmpTelNr As Telefonnummer

            With tmpTelefonat

                If [Call].Type.AreEqual(1) Or [Call].Type.AreEqual(3) Then ' incoming, outgoing
                    ' Testweise wird auch nach dem Namen des Gerätes gesucht, wenn über den Port nichts gefunden wurde.
                    .TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID = [Call].Port OrElse TG.Name = [Call].Device)
                    ' Umwandlung von "hh:mm" in Sekundenwert
                    With CDate([Call].Duration)
                        tmpTelefonat.Dauer = New TimeSpan(.Hour, .Minute, .Second).TotalSeconds.ToInt
                    End With

                    .Angenommen = .Dauer.IsNotZero
                End If

                If [Call].Type.AreEqual(1) Or [Call].Type.AreEqual(2) Or [Call].Type.AreEqual(10) Then ' incoming, missed, rejected
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Eingehend
                    ' Own Number of called party (incoming call)
                    tmpTelNr = New Telefonnummer With {.SetNummer = [Call].CalledNumber}
                    .EigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(Tel) Tel.Equals(tmpTelNr))
                    ' Falls keine Nummer übereinstimmt, dann setze den tmpTelNr
                    If .EigeneTelNr Is Nothing Then .EigeneTelNr = tmpTelNr
                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert

                    ' Number of calling party 
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = [Call].Caller}
                    .NrUnterdrückt = .GegenstelleTelNr.Unterdrückt

                End If

                If [Call].Type.AreEqual(3) Then 'outgoing
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Ausgehend
                    ' Own Number of called party (outgoing call) 
                    tmpTelNr = New Telefonnummer With {.SetNummer = [Call].CallerNumber}
                    .EigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(Tel) Tel.Equals(tmpTelNr))
                    ' Falls keine Nummer übereinstimmt, dann setze den tmpTelNr
                    If .EigeneTelNr Is Nothing Then .EigeneTelNr = tmpTelNr
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
