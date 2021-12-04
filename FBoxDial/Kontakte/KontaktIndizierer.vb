Imports Microsoft.Office.Interop.Outlook

Friend Module KontaktIndizierer
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Kontaktindizierung"

    ''' <summary>
    ''' Indiziert oder deindiziert ein Kontaktelement, ne nach dem, ob der Ordner für die Kontaktsuche ausgewählt wurde
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der indiziert werden soll.</param>
    ''' <param name="olOrdner">Der Ordner in dem Der Kontakt gespeichert werden soll.</param>
    ''' <param name="RCO">Angabe, ob der indizierte Kontakte freigegeben werden soll. <see cref="ReleaseComObject"/></param>
    Friend Sub IndiziereKontakt(olKontakt As ContactItem, olOrdner As MAPIFolder, RCO As Boolean)

        ' Wird der Zielordner für, die Kontaktsuche verwendet?
        If olOrdner.OrdnerAusgewählt(OutlookOrdnerVerwendung.KontaktSuche) Then
            ' Indiziere den Kontakt
            IndiziereKontakt(olKontakt)

        Else
            ' Deindiziere den Kontakt
            DeIndiziereKontakt(olKontakt)

        End If

        If RCO Then ReleaseComObject(olKontakt)
        ReleaseComObject(olOrdner)
    End Sub

    ''' <summary>
    ''' Indiziert ein Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der indiziert werden soll.</param>
    Friend Sub IndiziereKontakt(olKontakt As ContactItem)

        With olKontakt

            Dim colArgs As Object()
            ' Lade alle Telefonnummern des Kontaktes
            ' Das Laden der Telefonnummern mittels PropertyAccessor ist nicht sinnvoll.
            ' Die Daten liegen darin erst nach dem Speichern des Kontaktes vor.
            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNr), Object())
            ' Die Telefonnummern werden stattdessen aus den Eigenschaften des Kontaktes direkt ausgelesen.
            colArgs = .GetTelNrArray

            ' Entferne alle Formatierungen der Telefonnummgern
            For i = LBound(colArgs) To UBound(colArgs)
                If colArgs(i) IsNot Nothing Then
                    'If TypeOf colArgs(i) IsNot Integer Then
                    If colArgs(i).ToString.IsNotStringNothingOrEmpty Then
                        Using tempTelNr = New Telefonnummer() With {.SetNummer = colArgs(i).ToString}
                            colArgs(i) = tempTelNr.Unformatiert
                        End Using
                    End If
                Else
                    colArgs(i) = DfltStringEmpty
                End If
            Next

            ' Lösche alle Indizierungsfelder
            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            ' Speichere die Nummern und nicht sichtbare Felder
            Try
                .PropertyAccessor.SetProperties(DASLTagTelNrIndex, colArgs)
            Catch ex As System.Exception
                NLogger.Error(ex, $"Kontakt: { .FullNameAndCompany}")
            End Try

            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())

            If .Speichern Then NLogger.Debug($"Kontakt { .FullNameAndCompany} gespeichert")

        End With
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der deindiziert werden soll.</param>
    ''' <remarks>Funktion wird in Teilen nicht benötigt, da mit aktuellen Programmversionen keine benutzerdefinierten Kontaktfelder erstellt werden.
    ''' Die Funktion dient zum bereinigen von Kontakten, die mit älteren Programmversionen indiziert wurden.</remarks>
    Friend Sub DeIndiziereKontakt(olKontakt As ContactItem)
        ' Ab hier Code zum bereinigen, der alten Indizierungsspuren
        Dim UserEigenschaft As UserProperty
        With olKontakt
            With .UserProperties
                For Each UserProperty As String In DfltUserProperties

                    Try
                        UserEigenschaft = .Find(UserProperty)
                    Catch
                        UserEigenschaft = Nothing
                    End Try

                    If UserEigenschaft IsNot Nothing Then UserEigenschaft.Delete()
                    UserEigenschaft = Nothing
                Next
            End With
            ' Ab hier neu
            ' Lösche alle Indizierungsfelder

            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            If .Speichern Then NLogger.Debug($"Kontakt { .FullNameAndCompany} gespeichert")
        End With
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus den Ordnern aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="Ordner">Der Ordner der deindiziert werden soll.</param>
    ''' <remarks>Funktion wird eigentlich nicht benötigt, da mit aktuellen Programmversionen keine benutzerdefinierten Kontaktfelder in Ordnern erstellt werden.
    ''' Die Funktion dient zum bereinigen von Ordner, die mit älteren Programmversionen indiziert wurden.</remarks>
    Friend Sub DeIndizierungOrdner(Ordner As MAPIFolder)
        Try
            With Ordner.UserDefinedProperties
                For i = 1 To .Count
                    If DfltUserProperties.Contains(.Item(1).Name) Then .Remove(1)
                Next
            End With
        Catch : End Try
    End Sub

#End Region

End Module
