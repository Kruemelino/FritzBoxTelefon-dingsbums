Imports System.Timers
Imports System.IO.Path

Public Class formAnrMon
    Private TelefonName As String
    Private aID As Integer
    Private C_XML As MyXML
    Private HelferFunktionen As Helfer
    Private TelNr As String              ' TelNr des Anrufers
    Private KontaktID As String              ' KontaktID des Anrufers
    Private StoreID As String
    Private MSN As String
    Private AnrMon As AnrufMonitor
    Public AnrmonClosed As Boolean
    Private OlI As OutlookInterface
    Private WithEvents TimerAktualisieren As Timer


    Public Sub New(ByVal iAnrufID As Integer, _
                   ByVal Aktualisieren As Boolean, _
                   ByVal XMLKlasse As MyXML, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal AnrufMon As AnrufMonitor, _
                   ByVal OutlInter As OutlookInterface)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        HelferFunktionen = HelferKlasse
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        'If ThisAddIn.Debug Then ThisAddIn.Diagnose.AddLine("formAnrMon aufgerufen")
        aID = iAnrufID
        C_XML = XMLKlasse
        OlI = OutlInter
        AnrMon = AnrufMon
        AnrMonausfüllen()
        AnrmonClosed = False


        Dim OInsp As Outlook.Inspector = Nothing
        If Aktualisieren Then
            TimerAktualisieren = HelferFunktionen.SetTimer(100)
            If TimerAktualisieren Is Nothing Then
                HelferFunktionen.LogFile("formAnrMon.New: TimerNeuStart nicht gestartet")
            End If
        End If
        OlI.InspectorVerschieben(True)

        With PopupNotifier
            .ShowDelay = CInt(C_XML.Read("Optionen", "TBEnblDauer", "10")) * 1000
            .AutoAusblenden = CBool(C_XML.Read("Optionen", "CBAutoClose", "True"))
            Dim FormVerschiebung As New Drawing.Size(CInt(C_XML.Read("Optionen", "TBAnrMonX", "0")), CInt(C_XML.Read("Optionen", "TBAnrMonY", "0")))
            .PositionsKorrektur = FormVerschiebung
            .EffektMove = CBool(C_XML.Read("Optionen", "CBAnrMonMove", "True"))
            .EffektTransparenz = CBool(C_XML.Read("Optionen", "CBAnrMonTransp", "True"))
            .EffektMoveGeschwindigkeit = CInt(C_XML.Read("Optionen", "TBAnrMonMoveGeschwindigkeit", "50"))
            .Popup()
        End With
        OlI.InspectorVerschieben(False)
    End Sub

    Sub AnrMonausfüllen()
        ' Diese Funktion nimmt Daten aus der Registry und öffnet 'formAnMon'.
        Dim AnrName As String              ' Name des Anrufers
        Dim Uhrzeit As String
        'Dim letzterAnrufer() As String = Split(C_XML.Read("letzterAnrufer", "letzterAnrufer" & aID, CStr(DateTime.Now) & ";;unbekannt;;-1;-1;"), ";", 6, CompareMethod.Text)
        'LA(0) = Zeit
        'LA(1) = Anrufer
        'LA(2) = TelNr
        'LA(3) = MSN
        'LA(4) = StoreID
        'LA(5) = KontaktID

        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("LetzterAnrufer")
            .Add("Eintrag[@ID = """ & aID & """]")
            .Add("Zeit")
            Uhrzeit = C_XML.Read(xPathTeile, CStr(DateTime.Now))

            .Item(.Count - 1) = "Anrufer"
            AnrName = C_XML.Read(xPathTeile, "")

            .Item(.Count - 1) = "TelNr"
            TelNr = C_XML.Read(xPathTeile, "unbekannt")

            .Item(.Count - 1) = "MSN"
            MSN = C_XML.Read(xPathTeile, "")

            .Item(.Count - 1) = "StoreID"
            StoreID = C_XML.Read(xPathTeile, "-1")

            .Item(.Count - 1) = "KontaktID"
            KontaktID = C_XML.Read(xPathTeile, "-1")
        End With

        TelefonName = AnrMon.TelefonName(MSN)
        With PopupNotifier
            If TelNr = "unbekannt" Then
                With .OptionsMenu
                    .Items("ToolStripMenuItemRückruf").Enabled = False ' kein Rückruf im Fall 1
                    .Items("ToolStripMenuItemKopieren").Enabled = False ' in dem Fall sinnlos
                    .Items("ToolStripMenuItemKontaktöffnen").Text = "Einen neuen Kontakt erstellen"
                End With
            End If
            ' Uhrzeit des Telefonates eintragen
            .Uhrzeit = Uhrzeit
            ' Telefonnamen eintragen
            .TelName = TelefonName & CStr(IIf(CBool(C_XML.Read("Optionen", "CBShowMSN", "False")), " (" & MSN & ")", vbNullString))

            If Not Strings.Left(KontaktID, 2) = "-1" Then
                If Not TimerAktualisieren Is Nothing Then HelferFunktionen.KillTimer(TimerAktualisieren)
                ' Kontakt einblenden wenn in Outlook gefunden
                Try
                    OlI.KontaktInformation(KontaktID, StoreID, PopupNotifier.AnrName, PopupNotifier.Firma)
                    If CBool(C_XML.Read("Optionen", "CBAnrMonContactImage", "True")) Then
                        Dim BildPfad = OlI.KontaktBild(KontaktID, StoreID)
                        If Not BildPfad Is vbNullString Then
                            PopupNotifier.Image = Drawing.Image.FromFile(BildPfad)
                            ' Seitenverhältnisse anpassen
                            Dim Bildgröße As New Drawing.Size(PopupNotifier.ImageSize.Width, CInt((PopupNotifier.ImageSize.Width * PopupNotifier.Image.Size.Height) / PopupNotifier.Image.Size.Width))
                            PopupNotifier.ImageSize = Bildgröße
                        End If
                    End If
                Catch ex As Exception
                    HelferFunktionen.LogFile("formAnrMon: Fehler beim Öffnen des Kontaktes " & AnrName & " (" & ex.Message & ")")
                    .Firma = ""
                    If AnrName = "" Then
                        .TelNr = ""
                        .AnrName = TelNr
                    Else
                        .TelNr = TelNr
                        .AnrName = AnrName
                    End If
                End Try

                .TelNr = TelNr
            Else
                .Firma = ""
                If AnrName = "" Then
                    .TelNr = ""
                    .AnrName = TelNr
                Else
                    .TelNr = TelNr
                    .AnrName = AnrName
                End If
            End If
        End With
    End Sub

    Private Sub PopupNotifier_Close() Handles PopupNotifier.Close
        PopupNotifier.Hide()
    End Sub

    Private Sub ToolStripMenuItemRückruf_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemRückruf.Click
        ThisAddIn.WClient.Rueckruf(aID)
    End Sub

    Private Sub ToolStripMenuItemKopieren_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemKopieren.Click
        With PopupNotifier
            My.Computer.Clipboard.SetText(.AnrName & CStr(IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")")))
        End With
    End Sub

    Private Sub PopupNotifier_Closed() Handles PopupNotifier.Closed
        AnrmonClosed = True
        If Not TimerAktualisieren Is Nothing Then HelferFunktionen.KillTimer(TimerAktualisieren)
    End Sub

    Private Sub ToolStripMenuItemKontaktöffnen_Click() Handles ToolStripMenuItemKontaktöffnen.Click, PopupNotifier.LinkClick
        ' blendet den Kontakteintrag des Anrufers ein
        ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt
        Dim Kontaktdaten(2) As String
        Kontaktdaten(0) = KontaktID
        Kontaktdaten(1) = StoreID
        Kontaktdaten(2) = TelNr
        ThisAddIn.WClient.ZeigeKontakt(Kontaktdaten)
    End Sub

    Private Sub TimerAktualisieren_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerAktualisieren.Elapsed
        Dim VergleichString As String = PopupNotifier.AnrName
        AnrMonausfüllen()
        If Not VergleichString = PopupNotifier.AnrName Then HelferFunktionen.KillTimer(TimerAktualisieren)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
