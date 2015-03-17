Imports System.Threading

Friend Class formImportAnrList
#Region "BackgroundWorker"
    Private WithEvents DownloadAnrListe As New System.ComponentModel.BackgroundWorker ' Background Worker zum Runterladen der Anrufliste
    Private WithEvents BGAnrListeAuswerten As New System.ComponentModel.BackgroundWorker
#End Region

#Region "Delegate"
    Private Delegate Sub DelgSetProgressbar()
    Private Delegate Sub DelgSetButtonHerunterladen()
#End Region

#Region "Eigene Klassen"
    Private C_FBox As FritzBox
    Private C_AnrMon As AnrufMonitor
    Private C_hf As Helfer
    Private C_DP As DataProvider
    Private C_XML As XML
#End Region

#Region "Structure"
    Private Structure ImportZeitraum
        Dim StartZeit As Date
        Dim EndZeit As Date
    End Structure
#End Region

#Region "Eigene Variablen"
    Private Abbruch As Boolean
    Private anzeigen As Boolean
    Private CSVAnrliste As String
    Private StatusWert As Integer
    Private SID As String
    Private EntryCount As Integer = -1 ' Anzahl der zu importierenden Telefonate
#End Region

    Friend Sub New(ByVal FritzBoxKlasse As FritzBox, _
                   ByVal AnrMonKlasse As AnrufMonitor, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal DataProviderKlasse As DataProvider, _
                   ByVal XMLKlasse As XML, _
                   ByVal FormShow As Boolean)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_FBox = FritzBoxKlasse
        C_DP = DataProviderKlasse
        C_hf = HelferKlasse
        C_AnrMon = AnrMonKlasse
        C_XML = XMLKlasse
        Abbruch = False
        anzeigen = FormShow
        If anzeigen Then Me.Show() 'wenn gewollt
        With DownloadAnrListe
            .WorkerSupportsCancellation = True
            .RunWorkerAsync()
        End With
    End Sub

    Private Sub formJournalimport_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim StartZeit As Date
        StartZeit = C_DP.P_StatOLClosedZeit
        Me.StartDatum.Value = StartZeit
        Me.StartZeit.Value = StartZeit
        Me.EndDatum.Value = System.DateTime.Now
        Me.EndZeit.Value = System.DateTime.Now
    End Sub

#Region " Herunterladen"
    Private Sub DownloadAnrListe_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles DownloadAnrListe.DoWork
        e.Result = C_FBox.DownloadAnrListe
    End Sub

    Private Sub DownloadAnrListe_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles DownloadAnrListe.RunWorkerCompleted
        Dim Übergabe As ImportZeitraum
        CSVAnrliste = CStr(e.Result)
        If Me.InvokeRequired Then
            Dim D As New DelgSetButtonHerunterladen(AddressOf ButtonEnable)
            Invoke(D)
        Else
            Me.ButtonHerunterladen.Enabled = True
        End If

        With Übergabe
            .StartZeit = C_DP.P_StatOLClosedZeit
            .EndZeit = System.DateTime.Now
        End With
        If Not anzeigen Then
            With BGAnrListeAuswerten
                .WorkerReportsProgress = True
                .RunWorkerAsync(Übergabe)
            End With
        End If

    End Sub

    Sub ButtonEnable()
        Me.ButtonHerunterladen.Enabled = True
    End Sub
#End Region

#Region " Auswertung"
    Private Sub BGAnrListeAuswerten_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGAnrListeAuswerten.DoWork
        JournalCSV(CType(e.Argument, ImportZeitraum))
    End Sub

    Private Sub BGAnrListeAuswerten_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BGAnrListeAuswerten.ProgressChanged
        StatusWert = e.ProgressPercentage
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbar)
            Invoke(D)
        End If
    End Sub

    Private Sub SetProgressbar()
        Me.ProgressBar1.Value = StatusWert
        Me.lblBG1Percent.Text = StatusWert & " % (" & StatusWert * EntryCount \ 100 & "/" & EntryCount & ")"
        Me.Text = "Journalimport - " & Me.lblBG1Percent.Text
        If StatusWert = 100 Then Me.ButtonStart.Enabled = True
    End Sub

    Private Sub BGAnrListeAuswerten_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGAnrListeAuswerten.RunWorkerCompleted
        BGAnrListeAuswerten.Dispose()
        DownloadAnrListe.Dispose()
    End Sub

    Private Sub JournalCSV(ByVal Zeitraum As ImportZeitraum)

        Dim aktZeile As String()        ' aktuell bearbeitete Zeile
        Dim AnrTyp As String            ' Typ des Anrufs
        Dim AnrZeit As String           ' Zeitpunkt des Anrufs
        Dim AnrTelNr As String          ' Name und TelNr des Telefonpartners
        Dim AnrID As String             ' ID des Anrufes
        Dim Nebenstelle As String       ' verwendete Nebenstelle
        Dim MSN As String               ' verwendete MSN
        Dim NSN As Integer              ' verwendete Nebenstellennummer
        Dim Dauer As String             ' Dauer des Telefonats
        Dim vFBStatus As String()       ' generierter Status-String
        Dim Startzeit As Date           ' Letzter Journalimports
        Dim Endzeit As Date             ' Ende des Journalimports
        Dim j, a, b As Integer          ' Zählvariable
        Dim AnrListe As String()
        Dim xPathTeile As New ArrayList

        DownloadAnrListe.Dispose()
        With Zeitraum
            Startzeit = .StartZeit
            Endzeit = .EndZeit
        End With
        Dim StartZeile As Integer ' Zeile der csv, die das Erste zu importierenden Telefonat enthält
        Dim EndZeile As Integer = -1 ' Zeile der csv, die das Letzte zu importierenden Telefonat enthält

        C_FBox.FBLogout(SID)

        If InStr(CSVAnrliste, "!DOCTYPE", CompareMethod.Text) = 0 And Not CSVAnrliste = DataProvider.P_Def_StringEmpty Then

            CSVAnrliste = Strings.Left(CSVAnrliste, Len(CSVAnrliste) - 2) 'Datei endet mit zwei chr(10) -> abschneiden
            ' Datei wird zuerst in ein String-Array gelesen und dann ausgewertet.
            AnrListe = Split(CSVAnrliste, Chr(10), , CompareMethod.Text)
            If Not AnrListe.Length = 1 Then
                j = -1
                ' Ermittle Startzeile
                Do
                    j += 1
                Loop Until AnrListe.GetValue(j).ToString = "Typ;Datum;Name;Rufnummer;Nebenstelle;Eigene Rufnummer;Dauer" Or j = AnrListe.Length
                ' Ermittle die Position des Ersten und Letzten zu importierenden Telefonats
                StartZeile = j + 1
                If CStr(AnrListe.GetValue(j + 1)) = DataProvider.P_Def_StringEmpty Then
                    j += 1
                    StartZeile = j + 1
                End If

                Do
                    j += 1
                    AnrZeit = CStr(Split(CStr(AnrListe.GetValue(j)), ";", , CompareMethod.Text).GetValue(1)) & ":00"
                    If CDate(AnrZeit) < Startzeit Then EndZeile = j - 1 ' AnrZeit nach  Startzeit
                    If CDate(AnrZeit) > Endzeit Then StartZeile = j + 1 ' AnrZeit vor Endzeit
                    Windows.Forms.Application.DoEvents()
                Loop Until CDate(AnrZeit) < Startzeit Or j = AnrListe.Length - 1
                If j = AnrListe.Length - 1 Then EndZeile = AnrListe.Length - 1
                EntryCount = EndZeile - StartZeile + 1
                If EntryCount > 0 Then

                    b = 0 ' Anzahl der tatsächlich importierten Telefonate
                    a = 1
                    For j = EndZeile To StartZeile Step -1 ' Array wird Zeilenweise rückwärts durchlaufen
                        If Abbruch Then Exit For
                        ' aktuelle Zeile wird ebenfalls in ein Array geteilt, damit ist ein direkter Zugriff möglich.
                        aktZeile = Split(CStr(AnrListe.GetValue(j)), ";", , CompareMethod.Text)

                        AnrTyp = CStr(aktZeile.GetValue(0))
                        AnrZeit = CStr(aktZeile.GetValue(1)) & ":00"
                        'AnrName = CStr(aktZeile.GetValue(2)) 'wird nicht benötigt
                        AnrTelNr = CStr(aktZeile.GetValue(3))
                        Nebenstelle = CStr(aktZeile.GetValue(4))
                        MSN = CStr(aktZeile.GetValue(5))
                        Dauer = CStr(aktZeile.GetValue(6))

                        Dauer = CStr((CLng(Strings.Left(Dauer, InStr(1, Dauer, ":", CompareMethod.Text) - 1)) * 60 + CLng(Mid(Dauer, InStr(1, Dauer, ":", CompareMethod.Text) + 1))) * 60)
                        ' Bei analogen Anschlüssen steht "Festnetz" in MSN
                        If MSN = "Festnetz" Then MSN = C_XML.Read(C_DP.XMLDoc, "Telefone", "POTS", DataProvider.P_Def_ErrorMinusOne_String)
                        ' MSN von dem "Internet: " bereinigen
                        If Not MSN = String.Empty Then MSN = Replace(MSN, "Internet: ", String.Empty)

                        If C_DP.P_CLBTelNr.Contains(C_hf.EigeneVorwahlenEntfernen(MSN)) Or DataProvider.P_Debug_AnrufSimulation Then
                            b += 1
                            NSN = -1
                            AnrID = Str(100 + b)
                            If Not AnrTyp = "2" Then
                                'Wird im Fall 2 nicht benötigt: Verpasster Anruf.
                                Select Case Nebenstelle
                                    Case "Durchwahl"
                                        NSN = 3
                                    Case "ISDN Gerät"
                                        NSN = 4
                                    Case "Fax (intern/PC)"
                                        NSN = 5
                                    Case "Data S0"
                                        NSN = 36
                                    Case "Data PC"
                                        NSN = 37
                                    Case Else
                                        With xPathTeile
                                            .Clear()
                                            .Add("Telefone")
                                            .Add("Telefone")
                                            .Add("*")
                                            .Add("Telefon")
                                            .Add("[TelName = """ & Nebenstelle & """]")
                                            .Add("@Dialport")
                                            NSN = CInt(C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String))
                                        End With
                                End Select
                            End If
                            If Not NSN = -1 Then
                                'If NSN < 4 Then NSN -= 1
                                Select Case NSN
                                    Case 1 To 3
                                        NSN -= 1
                                    Case 60 To 69 'DECT
                                        NSN -= 50
                                End Select
                            End If

                            Select Case CInt(AnrTyp)
                                Case 1 ' eingehender Anruf: angenommen
                                    vFBStatus = Split(AnrZeit & ";RING;" & AnrID & ";" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
                                    C_AnrMon.AnrMonRING(vFBStatus, DataProvider.P_Debug_AnrufSimulation)
                                    vFBStatus = Split(AnrZeit & ";CONNECT;" & AnrID & ";" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
                                    C_AnrMon.AnrMonCONNECT(vFBStatus, DataProvider.P_Debug_AnrufSimulation)
                                Case 2 ' eingehender Anruf: nicht angenommen
                                    vFBStatus = Split(AnrZeit & ";RING;" & AnrID & ";" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
                                    C_AnrMon.AnrMonRING(vFBStatus, DataProvider.P_Debug_AnrufSimulation)
                                Case 3, 4 ' ausgehender Anruf
                                    vFBStatus = Split(AnrZeit & ";CALL;" & AnrID & ";0;" & MSN & ";" & AnrTelNr & ";;", ";", , CompareMethod.Text)
                                    C_AnrMon.AnrMonCALL(vFBStatus, DataProvider.P_Debug_AnrufSimulation)
                                    vFBStatus = Split(AnrZeit & ";CONNECT;" & AnrID & ";" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
                                    C_AnrMon.AnrMonCONNECT(vFBStatus, DataProvider.P_Debug_AnrufSimulation)
                            End Select
                            If Abbruch Then Exit For
                            vFBStatus = Split(AnrZeit & ";DISCONNECT;" & AnrID & ";" & Dauer & ";", ";", , CompareMethod.Text)
                            C_AnrMon.AnrMonDISCONNECT(vFBStatus, DataProvider.P_Debug_AnrufSimulation)
                        End If
                        If anzeigen Then BGAnrListeAuswerten.ReportProgress(a * 100 \ EntryCount)
                        a += 1
                    Next
                End If
                ' Registry zurückschreiben
                C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)
                C_hf.LogFile("Aus der 'FRITZ!Box_Anrufliste.csv' " & IIf(b = 1, "wurde " & b & " Journaleintag", "wurden " & b & " Journaleintäge").ToString & " importiert.")
            Else
                C_hf.LogFile("Auswertung von 'Anrufliste.csv' wurde abgebrochen.")
            End If
            If anzeigen Then BGAnrListeAuswerten.ReportProgress(100)
            BGAnrListeAuswerten.Dispose()
        End If

    End Sub
#End Region

#Region " Button"
    Private Sub ButtonSchließen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSchließen.Click
        Me.Hide()
    End Sub
    Private Sub ButtonStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonStart.Click
        Dim Übergabe As ImportZeitraum
        Abbruch = False
        Me.ButtonStart.Enabled = False
        Do While DownloadAnrListe.IsBusy
            Windows.Forms.Application.DoEvents()
        Loop
        StatusWert = 0
        SetProgressbar()
        BereichAuswertung.Enabled = True
        If Not Len(CSVAnrliste) = 0 Then
            With Übergabe
                .StartZeit = CDate(Me.StartDatum.Text & " " & Me.StartZeit.Text)
                .EndZeit = CDate(Me.EndDatum.Text & " " & Me.EndZeit.Text)
            End With

            With BGAnrListeAuswerten
                .WorkerReportsProgress = True
                .RunWorkerAsync(Übergabe)
            End With
        End If
    End Sub
    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        Abbruch = True
    End Sub
    Private Sub ButtonHerunterladen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonHerunterladen.Click
        Me.ButtonHerunterladen.Enabled = False
        With DownloadAnrListe
            .WorkerSupportsCancellation = True
            .RunWorkerAsync()
        End With
    End Sub
#End Region
End Class