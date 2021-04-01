Public Class AnrMonService
    Implements IAnrMonService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Sub BlockNumbers(TelNr As Telefonnummer) Implements IAnrMonService.BlockNumbers

        Dim Sperreintrag As New FritzBoxXMLKontakt
        With Sperreintrag
            .Person.RealName = TelNr.Unformatiert
            .Telefonie.Nummern.Add(New FritzBoxXMLNummer With {.Nummer = TelNr.Unformatiert})
        End With


        Threading.Tasks.Task.Run(Sub()
                                     If AddToCallBarring(Sperreintrag) Then
                                         NLogger.Info($"Die Nummer '{TelNr.Unformatiert}' wurde der Sperrliste hinzugefügt.")
                                     Else
                                         NLogger.Warn($"Die Nummer '{TelNr.Unformatiert}' wurde nicht der Sperrliste hinzugefügt.")
                                     End If
                                 End Sub)

    End Sub
End Class
