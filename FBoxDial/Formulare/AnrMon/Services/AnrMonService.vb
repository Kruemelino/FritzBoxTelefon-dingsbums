Imports System.Threading.Tasks
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class AnrMonService
    Implements IAnrMonService

    Private Property SoundPlayer As SoundPlayerEx
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String)) Implements IAnrMonService.SoundFinished

#Region "Styling"
    Public Sub GetColors(ByRef BackgroundColor As String, ByRef ForeColor As String, TelNr As Telefonnummer, IsStoppUhr As Boolean, IsVIP As Boolean) Implements IAnrMonService.GetColors

        ' 0. Lade die Default-Farbwerte

        ' 0.1 Hintergrund
        BackgroundColor = CType(Globals.ThisAddIn.WPFApplication.FindResource("BackgroundColor"), SolidColorBrush).Color.ToString
        ' 0.2 Schriftfarbe
        ForeColor = CType(Globals.ThisAddIn.WPFApplication.FindResource("ControlDefaultForeground"), SolidColorBrush).Color.ToString

        ' 1. Allgemeine Farben, die der Nutzer für alle Fenster festgelegt hat.
        ' Unterscheidung zwischen Stoppuhr und Anrufmonitor/CallPane
        If IsStoppUhr Then
            ' Farbdefinition für Stoppuhr laden
            With XMLData.POptionen.Farbdefinitionen.Find(Function(FD) FD.Kontext.Equals(Localize.LocOptionen.strStoppuhr))
                ' 1.1 Hintergrund
                If .CBSetBackgroundColor Then BackgroundColor = .TBBackgoundColor

                ' 1.2 Schriftfarbe
                If .CBSetForegroundColor Then ForeColor = .TBForegoundColor
            End With
        Else
            ' Farbdefinition für Anrufmonitor laden
            With XMLData.POptionen.Farbdefinitionen.Find(Function(FD) FD.Kontext.Equals(Localize.LocOptionen.strAnrMon))
                ' 1.1 Hintergrund
                If .CBSetBackgroundColor Then BackgroundColor = .TBBackgoundColor

                ' 1.2 Schriftfarbe
                If .CBSetForegroundColor Then ForeColor = .TBForegoundColor
            End With
        End If

        ' 2. Überschreibe die Farbdefinition je eigener Nummer
        If TelNr IsNot Nothing AndAlso TelNr.EigeneNummerInfo IsNot Nothing AndAlso TelNr.EigeneNummerInfo.Farben IsNot Nothing Then
            With TelNr.EigeneNummerInfo.Farben
                ' 2.1 Hintergrund
                If .CBSetBackgroundColor Then BackgroundColor = .TBBackgoundColor

                ' 2.2 Schriftfarbe
                If .CBSetForegroundColor Then ForeColor = .TBForegoundColor
            End With
        Else
            NLogger.Warn($"Farbdefinition für Nummer {TelNr?.Einwahl} nicht gefunden.")
        End If

        ' 3. Farbdefinition nach VIP
        If IsVIP Then
            With XMLData.POptionen.Farbdefinitionen.Find(Function(FD) FD.Kontext.Equals(Localize.LocOptionen.strVIP))
                ' 3.1 Hintergrund
                If .CBSetBackgroundColor Then BackgroundColor = .TBBackgoundColor

                ' 3.2 Schriftfarbe
                If .CBSetForegroundColor Then ForeColor = .TBForegoundColor
            End With
        End If

    End Sub
#End Region

#Region "MissedCallPane"
    Public Sub RemoveMissedCall(MissedCall As MissedCallViewModel) Implements IAnrMonService.RemoveMissedCall
        Globals.ThisAddIn.ExplorerWrappers.Values.ToList.ForEach(Sub(ew) ew.RemoveMissedCall(MissedCall.VerpasstesTelefonat))
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
