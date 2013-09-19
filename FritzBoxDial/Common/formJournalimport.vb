Imports System.Threading

Public Class formJournalimport
    Private WithEvents DownloadAnrListe As New System.ComponentModel.BackgroundWorker ' Background Worker zum Runterladen der Anrufliste
    Private WithEvents BGAnrListeAuswerten As New System.ComponentModel.BackgroundWorker
    Private Delegate Sub DelgSetProgressbar()
    Private Delegate Sub DelgSetButtonHerunterladen()
    Private C_XML As MyXML
    Private CSVArg As Argument
    Private AnrMon As AnrufMonitor
    Private hf As Helfer
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
        hf = HelferKlasse
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
        e.Result = ThisAddIn.fBox.DownloadAnrListe
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
        Dim TelName() As String  ' Name des Telefons
        Dim Dauer As String  ' Dauer des Telefonats
        Dim vFBStatus As String()  ' generierter Status-String
        Dim Startzeit As Date    ' Letzter Journalimports
        Dim Endzeit As Date    ' Ende des Journalimports
        Dim i, j, a, b As Integer    ' Zählvariable
        Dim Nebenstellen As String()
        Dim Übergabewerte As Argument = CType(e.Argument, Argument)
        Dim AnrListe As String()

        DownloadAnrListe.Dispose()
        With Übergabewerte
            Startzeit = .StartZeit
            Endzeit = .EndZeit
        End With
        Dim Vorwahl As String = C_XML.Read("Optionen", "TBVorwahl", "")
        Dim checkstring As String = C_XML.Read("Telefone", "CLBTelNr", "-1") ' Enthällt alle MSN, auf die reakiert werden soll
        Dim StartZeile As Integer ' Zeile der csv, die das Erste zu importierenden Telefonat enthält
        Dim EndZeile As Integer = -1 ' Zeile der csv, die das Letzte zu importierenden Telefonat enthält
        Dim Anzahl As Integer = -1 ' Anzahl der zu importierenden Telefonate


        Nebenstellen = Split("1,2,3,5,51,52,53,54,55,56,57,58,50,600,601,602,603,604,60,61,62,63,64,65,66,67,68,69", ",", , CompareMethod.Text) ',20,21,22,23,24,25,26,27,28,29
        ThisAddIn.fBox.FBLogout(SID)


        'Dim myurl As String = "D:\Makro\Arbeitsverzeichnis\quelldateien\Maik\FRITZ!Box_Anrufliste.csv"
        'CSVAnrliste = hf.httpRead(myurl, System.Text.Encoding.UTF8)

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

                        If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, Vorwahl), Split(checkstring, ";", , CompareMethod.Text)) Then
                            b += 1
                            i = 0
                            NSN = -1

                            For Each NebenstellenNr In Nebenstellen
                                TelName = Split(C_XML.Read("Telefone", CStr(NebenstellenNr), "-1;;"), ";", , CompareMethod.Text)
                                If Not Nebenstelle = vbNullString Then
                                    If TelName(2) = Nebenstelle Then NSN = CInt(NebenstellenNr)
                                Else
                                    If TelName(1) = MSN Then NSN = CInt(NebenstellenNr)
                                End If
                                If Not NSN = -1 Then
                                    If NSN < 4 Then NSN -= 1
                                    Exit For
                                End If
                            Next

                            Select Case CInt(AnrTyp)
                                Case 1 ' eingehender Anruf: angenommen
                                    vFBStatus = Split(AnrZeit & ";RING;25;" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonRING(vFBStatus, False, False)
                                    vFBStatus = Split(AnrZeit & ";CONNECT;25;" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonCONNECT(vFBStatus, False)
                                Case 2 ' eingehender Anruf: nicht angenommen
                                    vFBStatus = Split(AnrZeit & ";RING;25;" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonRING(vFBStatus, False, False)
                                Case 3, 4 ' ausgehender Anruf
                                    vFBStatus = Split(AnrZeit & ";CALL;25;0;" & MSN & ";" & AnrTelNr & ";;", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonCALL(vFBStatus, False)
                                    vFBStatus = Split(AnrZeit & ";CONNECT;25;" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
                                    AnrMon.AnrMonCONNECT(vFBStatus, False)
                            End Select
                            If Abbruch Then Exit For
                            vFBStatus = Split(AnrZeit & ";DISCONNECT;25;" & Dauer & ";", ";", , CompareMethod.Text)
                            AnrMon.AnrMonDISCONNECT(vFBStatus, False)
                        End If
                        If anzeigen Then BGAnrListeAuswerten.ReportProgress(a * 100 \ Anzahl)
                        a += 1
                    Next
                End If
                ' Registry zurückschreiben
                C_XML.Write("Journal", "SchließZeit", CStr(System.DateTime.Now.AddMinutes(1)))
                hf.LogFile("Aus der 'FRITZ!Box_Anrufliste.csv' " & IIf(b = 1, "wurde " & b & " Journaleintag", "wurden " & b & " Journaleintäge").ToString & " importiert.")
            Else
                hf.LogFile("Auswertung von 'Anrufliste.csv' wurde abgebrochen.")
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