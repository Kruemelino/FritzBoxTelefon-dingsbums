Imports Microsoft.Office.Interop

Friend Class KontaktIndizierer
    Implements IDisposable
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
#Region "Kontaktindizierung"

    ''' <summary>
    ''' Indiziert einen Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der indiziert werden soll.</param>
    Friend Sub IndiziereKontakt(ByRef olKontakt As Outlook.ContactItem)

        With olKontakt

            Dim colArgs As Object()

            ' Lade alle Telefonnummern des Kontaktes
            ' Das Laden der Telefonnummern mittels PropertyAccessor ist nicht sinnvoll.
            ' Die Daten liegen darin erst nach dem Speichern des Kontaktes vor.
            '   colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNr), Object())
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
                    colArgs(i) = PDfltStringEmpty
                End If
            Next

            ' Lösche alle Indizierungsfelder
            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            ' Speichere die Nummern und nicht sichtbare Felder
            Try
                .PropertyAccessor.SetProperties(DASLTagTelNrIndex, colArgs)
            Catch ex As Exception
                NLogger.Error(ex, "Kontakt: {0}", olKontakt.FullNameAndCompany)
            End Try


            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())

            If Not .Saved Then
                .Save()
                NLogger.Info("Kontakt {0} wurde durch die Indizierung gespeichert.", olKontakt.FullNameAndCompany)
            End If
        End With
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der deindiziert werden soll.</param>
    ''' <remarks>Funktion wird in Teilen nicht benötigt, da mit aktuellen Programmversionen keine benutzerdefinierten Kontaktfelder erstellt werden.
    ''' Die Funktion dient zum bereinigen von Kontakten, die mit älteren Programmversionen indiziert wurden.</remarks>
    Friend Sub DeIndiziereKontakt(ByRef olKontakt As Outlook.ContactItem)
        ' Ab hier Code zum bereinigen, der alten Indizierungsspuren
        Dim UserEigenschaft As Outlook.UserProperty

        With olKontakt.UserProperties
            For Each UserProperty As String In PDfltUserProperties

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
        olKontakt.PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

        olKontakt.Save()
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus den Ordnern aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="Ordner">Der Ordner der deindiziert werden soll.</param>
    ''' <remarks>Funktion wird eigentlich nicht benötigt, da mit aktuellen Programmversionen keine benutzerdefinierten Kontaktfelder in Ordnern erstellt werden.
    ''' Die Funktion dient zum bereinigen von Ordner, die mit älteren Programmversionen indiziert wurden.</remarks>
    Friend Sub DeIndizierungOrdner(ByVal Ordner As Outlook.MAPIFolder)
        Try
            With Ordner.UserDefinedProperties
                For i = 1 To .Count
                    If PDfltUserProperties.Contains(.Item(1).Name) Then .Remove(1)
                Next
            End With
        Catch : End Try
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

#End Region

End Class
