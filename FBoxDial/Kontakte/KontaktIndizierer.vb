Imports Microsoft.Office.Interop.Outlook

Friend Module KontaktIndizierer
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property DASLTagTelNrIndex As Object() = GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) $"{DfltDASLSchema}FBDB-{P.Name}").ToArray
    Private Const ProppertyAccessorError As String = "-2147221233"
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

            NLogger.Trace($"Indizierung des Kontaktes { .FullNameAndCompany} gestartet.")

            Dim colArgs As Object()
            ' Lade alle Telefonnummern des Kontaktes
            ' Das Laden der Telefonnummern mittels PropertyAccessor ist nicht sinnvoll.
            ' Die Daten liegen darin erst nach dem Speichern des Kontaktes vor.
            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())
            ' Die Telefonnummern werden stattdessen aus den Eigenschaften des Kontaktes direkt ausgelesen.

            ' Entferne alle Formatierungen der Telefonnummern
            colArgs = .GetTelNrArray.Select(Of Object)(Function(N) If(N IsNot Nothing, New Telefonnummer() With {.SetNummer = N.ToString}.Unformatiert, String.Empty)).ToArray

            ' Lösche alle Indizierungsfelder
            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            ' Speichere die Nummern und nicht sichtbare Felder
            Try
                .PropertyAccessor.SetProperties(DASLTagTelNrIndex, colArgs)
            Catch ex As System.Exception
                NLogger.Error(ex, $"Kontakt: { .FullNameAndCompany}")
            End Try

            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())

            If .Speichern Then NLogger.Debug($"Indizierung des Kontaktes { .FullNameAndCompany.RemoveLineBreaks} abgeschlossen.")

        End With
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der deindiziert werden soll.</param>
    Friend Sub DeIndiziereKontakt(olKontakt As ContactItem)

        With olKontakt
            ' Lösche alle Indizierungsfelder
            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            If .Speichern Then NLogger.Debug($"Deindizierung des Kontaktes { .FullNameAndCompany.RemoveLineBreaks} abgeschlossen.")
        End With
    End Sub

    ''' <summary>
    ''' Erstellt ein Dictionary aller indizierten Telefonnummern. Key ist die englisch-sprachige Bezeichnung des Eintrages.
    ''' </summary>
    ''' <param name="olKontakt">Aktueller Kontakt</param>
    ''' <returns>Dictionary aller indizierten Telefonnummern</returns>
    Friend Function GetIndexList(olKontakt As ContactItem) As Dictionary(Of String, String)
        With olKontakt
            Dim colArgs As Object() = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())
            Dim Text As List(Of String) = GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) P.Name).ToList

            ' Stellt eine Zuordnung zwichen der Nummernbezeichnung und dem Key sowie der Nummer und des Values her.
            ' Im zweiten schritt werden alle elemente rausgefiltert, die leer sind.
            Return Text.ToDictionary(Function(i) Text(Text.IndexOf(i)), Function(i) colArgs(Text.IndexOf(i)).ToString) _
                       .Where(Function(i) i.Value.IsNotStringNothingOrEmpty AndAlso i.Value.IsNotEqual(ProppertyAccessorError)) _
                       .ToDictionary(Function(i) i.Key, Function(i) i.Value)
        End With
    End Function

#End Region

End Module
