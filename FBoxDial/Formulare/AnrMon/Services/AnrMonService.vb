Imports System.Threading.Tasks
Imports System.Windows.Media.Imaging

Public Class AnrMonService
    Implements IAnrMonService

    Private Sub UpdateTheme() Implements IAnrMonService.UpdateTheme
        'OfficeColors.UpdateTheme()
    End Sub

    Private Sub BlockNumber(TelNr As Telefonnummer) Implements IAnrMonService.BlockNumber
        AddNrToBlockList(TelNr)
    End Sub

    Private Async Function LadeBild(AnrMonTelefonat As Telefonat) As Task(Of BitmapImage) Implements IAnrMonService.LadeBild

        With AnrMonTelefonat
            ' Lade das Kontaktbild, wenn a) Option gesetzt ist oder b) ein TellowsErgebnis vorliegt und das Bild noch nicht geladen wurde
            If XMLData.POptionen.CBAnrMonContactImage Or .TellowsResult IsNot Nothing Then

                ' Setze das Kontaktbild, falls ein Outlookkontakt verfügbar ist.
                If .OlKontakt IsNot Nothing Then Return .OlKontakt.KontaktBildEx

                ' Setze das Kontaktbild, falls ein Eintrag aus einem Fritz!Box Telefonbuch verfügbar ist.
                If .FBTelBookKontakt IsNot Nothing Then Return Await .FBTelBookKontakt.KontaktBild

                ' Setze das Kontaktbild, falls ein Eintrag aus tellows verfügbar ist.
                If .TellowsResult IsNot Nothing Then
                    With .TellowsResult
                        ' Wenn der Mindestscore erreicht wurde und die Mindestanzahl an Kommentaren, dann Zeige die Informationen an
                        If .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinScore) And .Comments.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                            ' tellows Score Icon 
                            Return New BitmapImage(New Uri($"pack://application:,,,/{My.Resources.strDefLongName};component/Tellows/Resources/score{ .Score}.png", UriKind.Absolute))

                        End If
                    End With
                End If

            End If
        End With
        Return Nothing
    End Function
End Class
