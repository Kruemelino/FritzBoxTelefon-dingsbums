Imports System.Threading

Friend Class formJournalimport
    Private WithEvents DownloadAnrListe As New System.ComponentModel.BackgroundWorker ' Background Worker zum Runterladen der Anrufliste
    Private WithEvents BGAnrListeAuswerten As New System.ComponentModel.BackgroundWorker
    Private Delegate Sub DelgSetProgressbar()
    Private Delegate Sub DelgSetButtonHerunterladen()
    Private C_XML As MyXML
    Private CSVArg As Argument
    Private AnrMon As AnrufMonitor
    Private C_hf As Helfer
    Private Abbruch As Boolean
    Private anzeigen As Boolean
    Private CSVAnrliste As String
    Private StatusWert As Integer
    Private SID As String

    Structure Argument
        Dim StartZeit As Date
        Dim EndZeit As Date
    End Structure

    Public Sub New(ByVal AnrMonKlasse As AnrufMonitor, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal XMLKlasse As MyXML, _
                   ByVal FormShow As Boolean)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_XML = XMLKlasse
        C_hf = HelferKlasse
        AnrMon = AnrMonKlasse

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
        StartZeit = CDate(C_XML.Read("Journal", "SchließZeit", CStr(System.DateTime.Now)))
        Me.StartDatum.Value = StartZeit
        Me.StartZeit.Value = StartZeit
        Me.EndDatum.Value = System.DateTime.Now
        Me.EndZeit.Value = System.DateTime.Now
    End Sub

#Region " Herunterladen"
    Private Sub DownloadAnrListe_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles DownloadAnrListe.DoWork
        e.Result = ThisAddIn.P_FritzBox.DownloadAnrListe
    End Sub


    Private Sub DownloadAnrListe_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles DownloadAnrListe.RunWorkerCompleted
        Dim Übergabe As Argument
        CSVAnrliste = CStr(e.Result)
        If Me.InvokeRequired Then
            Dim D As New DelgSetButtonHerunterladen(AddressOf ButtonEnable)
            Invoke(D)
        Else
            Me.ButtonHerunterladen.Enabled = True
        End If

        With Übergabe
            .StartZeit = CDate(C_XML.Read("Journal", "SchließZeit", CStr(System.DateTime.Now)))
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

        Dim aktZeile As String()  ' aktuell bearbeitete Zeile
        Dim AnrTyp As String  ' Typ des Anrufs
        Dim AnrZeit As String  ' Zeitpunkt des Anrufs
        Dim AnrTelNr As String  ' Name und TelNr des Telefonpartners
        Dim Nebenstelle As String  ' verwendete Nebenstelle
        Dim MSN As String  ' verwendete MSN
        Dim NSN As Integer  ' verwendete Nebenstellennummer
        Dim Dauer As String  ' Dauer des Telefonats
        Dim vFBStatus As String()  ' generierter Status-String
        Dim Startzeit As Date    ' Letzter Journalimports
        Dim Endzeit As Date    ' Ende des Journalimports
        Dim i, j, a, b As Integer    ' Zählvariable
        Dim Übergabewerte As Argument = CType(e.Argument, Argument)
        Dim AnrListe As String()
        Dim xPathTeile As New ArrayList

        DownloadAnrListe.Dispose()
        With Übergabewerte
            Startzeit = .StartZeit
            Endzeit = .EndZeit
        End With
        Dim StartZeile As Integer ' Zeile der csv, die das Erste zu importierenden Telefonat enthält
        Dim EndZeile As Integer = -1 ' Zeile der csv, die das Letzte zu importierenden Telefonat enthält
        Dim Anzahl As Integer = -1 ' Anzahl der zu importierenden Telefonate
        ThisAddIn.P_FritzBox.FBLogOut(SID)

        If InStr(CSVAnrliste, "!DOCTYPE", CompareMethod.Text) = 0 And Not CSVAnrliste Is vbNullString Then

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
                If CStr(AnrListe.GetValue(j + 1)) = vbNullString Then
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
                Anzahl = EndZeile - StartZeile + 1
                If Anzahl > 0 Then

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
                        If MSN = "Festnetz" Then MSN = C_XML.Read("Telefone", "POTS", "-1")
                        ' MSN von dem "Internet: " bereinigen
                        If Not MSN = String.Empty Then MSN = Replace(MSN, "Internet: ", String.Empty)

                        With xPathTeile
                            .Clear()
                            .Add("Telefone")
                            .Add("Nummern")
                            .Add("*")
                            .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(MSN, C_XML.P_TBVorwahl) & """]")
                            .Add("@Checked")
                        End With

                        If C_hf.IsOneOf("1", Split(C_XML.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Then
                            b += 1
                            i = 0
                            NSN = -1
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
                                            .Add("Telefon") 'Select Case Nebenstelle
                                            .Add("[TelName = """ & Nebenstelle & """]")
                                            'End Select
                                            .Add("@Dialport")
                                            NSN = CInt(C_XML.Read(xPathTeile, "-1"))
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
                                    vFBStatus = Split(AnrZeit & ";RING;25;" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonRING(vFBStatus, False)
                                    vFBStatus = Split(AnrZeit & ";CONNECT;25;" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonCONNECT(vFBStatus)
                                Case 2 ' eingehender Anruf: nicht angenommen
                                    vFBStatus = Split(AnrZeit & ";RING;25;" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonRING(vFBStatus, False)
                                Case 3, 4 ' ausgehender Anruf
                                    vFBStatus = Split(AnrZeit & ";CALL;25;0;" & MSN & ";" & AnrTelNr & ";;", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonCALL(vFBStatus)
                                    vFBStatus = Split(AnrZeit & ";CONNECT;25;" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonCONNECT(vFBStatus)
                            End Select
                            If Abbruch Then Exit For
                            vFBStatus = Split(AnrZeit & ";DISCONNECT;25;" & Dauer & ";", ";", , CompareMethod.Text)
                            AnrMon.AnrMonDISCONNECT(vFBStatus)
                        End If
                        If anzeigen Then BGAnrListeAuswerten.ReportProgress(a * 100 \ Anzahl)
                        a += 1
                    Next
                End If
                ' Registry zurückschreiben
                C_XML.Write("Journal", "SchließZeit", CStr(System.DateTime.Now.AddMinutes(1)), True)
                C_hf.LogFile("Aus der 'FRITZ!Box_Anrufliste.csv' " & IIf(b = 1, "wurde " & b & " Journaleintag", "wurden " & b & " Journaleintäge").ToString & " importiert.")
            Else
                C_hf.LogFile("Auswertung von 'Anrufliste.csv' wurde abgebrochen.")
            End If
            If anzeigen Then BGAnrListeAuswerten.ReportProgress(100)
            BGAnrListeAuswerten.Dispose()
        End If
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
        Me.lblBG1Percent.Text = StatusWert & " %"
        Me.Text = StatusWert & " %  - Journalimport"
        If StatusWert = 100 Then Me.ButtonStart.Enabled = True
    End Sub

    Private Sub BGAnrListeAuswerten_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGAnrListeAuswerten.RunWorkerCompleted
        BGAnrListeAuswerten.Dispose()
        DownloadAnrListe.Dispose()
    End Sub
#End Region
#Region " Button"
    Private Sub ButtonSchließen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSchließen.Click
        Me.Hide()
    End Sub
    Private Sub ButtonStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonStart.Click
        Dim Übergabe As Argument
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