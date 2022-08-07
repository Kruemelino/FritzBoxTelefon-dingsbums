Imports System.Threading.Tasks
Imports System.Windows.Media.Imaging

Public Class AnrMonService
    Implements IAnrMonService

    Private Property SoundPlayer As SoundPlayerEx
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String)) Implements IAnrMonService.SoundFinished

#Region "Styling"
    Public Sub GetColors(ByRef BackgroundColor As String, ByRef ForeColor As String, TelNr As Telefonnummer, IsStoppUhr As Boolean) Implements IAnrMonService.GetColors

        ' 1. Allgemeine Farben, die der Nutzer für alle Fenster festgelegt hat.
        If XMLData.POptionen.CBSetAnrMonBColor Then
            ' Unterscheidung zwischen Stoppuhr und Anrufmonitor/CallPane
            BackgroundColor = If(IsStoppUhr, XMLData.POptionen.TBStoppUhrBColorHex, XMLData.POptionen.TBAnrMonBColorHex)
            ForeColor = If(IsStoppUhr, XMLData.POptionen.TBStoppUhrFColorHex, XMLData.POptionen.TBAnrMonFColorHex)
        End If

        ' 2. Überschreibe die Farbdefinition je eigener Nummer
        If TelNr IsNot Nothing AndAlso TelNr.EigeneNummerInfo IsNot Nothing Then

            ' Hintergrundfarbe
            If TelNr.EigeneNummerInfo.CBSetBackgroundColorByNumber Then
                BackgroundColor = TelNr.EigeneNummerInfo.TBBackgoundColorHex
            End If

            ' Schriftfarbe
            If TelNr.EigeneNummerInfo.CBSetForegroundColorByNumber Then
                ForeColor = TelNr.EigeneNummerInfo.TBForegoundColorHex
            End If
        End If

        ' TODO 3. Farbdefinition nach VIP

    End Sub
#End Region

#Region "MissedCallPane"
    Public Sub RemoveMissedCall(MissedCall As MissedCallViewModel) Implements IAnrMonService.RemoveMissedCall
        For Each Explorer In Globals.ThisAddIn.ExplorerWrappers.Values
            With Explorer.CallListPaneVM.MissedCallList
                ' Finde alle passenden Einträge und entferne diese
                NLogger.Debug($"Verpasster Anruf {MissedCall.VerpasstesTelefonat.NameGegenstelle} ({MissedCall.Zeit}) wird aus dem CallPane des entfernt.")

                .RemoveRange(Explorer.CallListPaneVM.MissedCallList.Where(Function(C) C.VerpasstesTelefonat.Equals(MissedCall.VerpasstesTelefonat)))
                ' Schließe das Pane, wenn gewünscht
                If Not .Any And XMLData.POptionen.CBCloseEmptyCallPane Then Explorer.HideCallListPane()
            End With
        Next
    End Sub
#End Region

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

    Public Function GetEigeneTelNr(TelNr As String) As Telefonnummer Implements IAnrMonService.GetEigeneTelNr
        GetEigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(T) T.Equals(TelNr))

        If GetEigeneTelNr Is Nothing Then
            ' Fehlerfall, wenn Nummer nicht in den eingelesenen Daten gefunden wurde
            NLogger.Warn($"Die eigene Telefonnummer {TelNr} wurde in den eingelesenen Daten nicht gefunden.")
            ' Setze neues Datenobjekt
            GetEigeneTelNr = New Telefonnummer With {.SetNummer = TelNr}
        End If
    End Function

#Region "TAM Messages"
    Public Sub PlayMessage(MessageURL As String) Implements IAnrMonService.PlayMessage
        NLogger.Debug($"Anrufbeantworternachricht via TAM für Eintrag: {MessageURL}")

        PlayRecord(MessageURL)
    End Sub

    Public Sub StoppMessage(MessageURL As String) Implements IAnrMonService.StoppMessage
        StoppRecord(MessageURL)
    End Sub

    Public Function CompleteURL(PathSegment As String) As String Implements IAnrMonService.CompleteURL
        Return FritzBoxDefault.CompleteURL(PathSegment)
    End Function

#Region "SoundPlayer"
    Private Sub PlayRecord(Pfad As String)

        If Not Pfad.Contains(FritzBoxDefault.DfltFritzBoxSessionID) Then

            If SoundPlayer Is Nothing Then
                SoundPlayer = New SoundPlayerEx()
                AddHandler SoundPlayer.SoundFinished, AddressOf SoundPlayer_SoundFinished

            End If

            With SoundPlayer
                If .PlayingAsync Then .Stop()

                .LocationURL = Pfad
                .PlayAsync()

            End With
        Else
            NLogger.Warn($"TAM Message kann nicht heruntergeladen werden: {Pfad} ")
        End If
    End Sub

    Private Sub StoppRecord(Pfad As String)
        If SoundPlayer IsNot Nothing Then
            With SoundPlayer
                If .PlayingAsync Then .Stop()
            End With
        End If
    End Sub

    Private Sub SoundPlayer_SoundFinished(sender As Object, e As NotifyEventArgs(Of String))

        RaiseEvent SoundFinished(Me, e)

        SoundPlayer.LocationURL = String.Empty
    End Sub
#End Region
#End Region
End Class
