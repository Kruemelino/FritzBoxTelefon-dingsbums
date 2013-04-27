Public Class formIndizierung
    Private WithEvents BGWIndexer As New System.ComponentModel.BackgroundWorker
    Private Delegate Sub DelgSetProgressbar()
    Private ini As New InI
    Private HelferFunktionen As Helfer
    Private DateiPfad As String
    Private Anzahl As Integer = 0
    Private StatusWert As Integer
    Private Dauer As TimeSpan
    Private Startzeit As Date
    Private KontaktName As String

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Sub New(ByVal IniPath As String, ByVal iniKlasse As InI, ByVal HelferKlasse As Helfer)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        DateiPfad = IniPath
        ini = iniKlasse
        HelferFunktionen = HelferKlasse
        Me.Show()
        If CBool(ini.Read(DateiPfad, "Optionen", "CheckBoxIndexAutoStart", "False")) Then
            Me.CheckBoxIndexAutoStart.Checked = True
            Start()
        End If

    End Sub

    Sub Start()
        Startzeit = Date.Now
        Me.ProgressBarIndex.Value = 0

        Me.LabelAnzahl.Text = "Status: 0/" & CStr(Me.ProgressBarIndex.Maximum)
        Me.ButtonSchließen.Enabled = False
        Me.ButtonAbbrechen.Enabled = True
        Me.ButtonStart.Enabled = False
        Me.LabelAnzahl.Text = "Status: Bitte Warten!"
        With BGWIndexer
            .WorkerSupportsCancellation = True
            .WorkerReportsProgress = True
            .RunWorkerAsync()
        End With

    End Sub

#Region "Vorbereitung"
    Function ErmittleKontaktanzahl() As Boolean
        ErmittleKontaktanzahl = True
        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder
        Dim LandesVW As String = ini.Read(DateiPfad, "Optionen", "TBLandesVW", "0049")
        Anzahl = 0
        olNamespace = ThisAddIn.oApp.GetNamespace("MAPI")

        If ini.Read(DateiPfad, "Optionen", "CBKHO", "True") = "True" Then
            olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            ZähleKontakte(olfolder)
        Else
            ZähleKontakte(, olNamespace)
        End If
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbarMax)
            Invoke(D)
        Else
            SetProgressbarMax()
        End If
    End Function
    Friend Function ZähleKontakte(Optional ByVal Ordner As Outlook.MAPIFolder = Nothing, _
                             Optional ByVal NamensRaum As Outlook.NameSpace = Nothing) As Integer

        ZähleKontakte = 0
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        Dim alleTE(13) As String  ' alle TelNr/Email eines Kontakts
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                ZähleKontakte(NamensRaum.Folders.Item(j))
                j = j + 1
            Loop
            aktKontakt = Nothing
            Return 0
        End If

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            'Debug.Print(Ordner.Name, Ordner.Items.Count)
            Anzahl += Ordner.Items.Count
        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count)
            ZähleKontakte(Ordner.Folders.Item(iOrdner))
            iOrdner = iOrdner + 1
        Loop

        aktKontakt = Nothing
    End Function
#End Region

    Friend Function KontaktIndexer(ByVal LandesVW As String, Optional ByVal Ordner As Outlook.MAPIFolder = Nothing, _
                              Optional ByVal NamensRaum As Outlook.NameSpace = Nothing) As Boolean
        KontaktIndexer = False
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        Dim item As Object      ' aktuelles Element
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        Dim alleTE(16) As String  ' alle TelNr/Email eines Kontakts
        Dim FeldNamen() As String = Split("FBDB-AssistantTelephoneNumber;FBDB-BusinessTelephoneNumber;FBDB-Business2TelephoneNumber;FBDB-CallbackTelephoneNumber;FBDB-CarTelephoneNumber;FBDB-CompanyMainTelephoneNumber;FBDB-HomeTelephoneNumber;FBDB-Home2TelephoneNumber;FBDB-ISDNNumber;FBDB-MobileTelephoneNumber;FBDB-OtherTelephoneNumber;FBDB-PagerNumber;FBDB-PrimaryTelephoneNumber;FBDB-RadioTelephoneNumber;FBDB-BusinessFaxNumber;FBDB-HomeFaxNumber;FBDB-OtherFaxNumber", ";", , CompareMethod.Text)
        Dim Index As String = ""
        Dim Elemente As Outlook.Items
        Dim speichern As Boolean = False
        Dim tempTelNr As String
        Dim i As Integer = 0
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                KontaktIndexer(LandesVW, NamensRaum.Folders.Item(j))
                j = j + 1
            Loop
            aktKontakt = Nothing
            Return True
        End If
        If BGWIndexer.CancellationPending Then Exit Function
        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
#If Not OVer = 11 Then
            For Each BenutzerdefFeld In FeldNamen
                With Ordner.UserDefinedProperties
                    If .Find(BenutzerdefFeld) Is Nothing Then
                        .Add(BenutzerdefFeld, Outlook.OlUserPropertyType.olText)
                    End If
                End With
            Next
#End If
            'MsgBox(Ordner.FolderPath)
            For Each item In Ordner.Items
                ' nur Kontakte werden durchsucht
                If TypeOf item Is Outlook.ContactItem Then
                    aktKontakt = CType(item, Outlook.ContactItem)
                    With aktKontakt
                        KontaktName = " (" & .FullNameAndCompany & ")"
                        BGWIndexer.ReportProgress(1)
                        alleTE(0) = .AssistantTelephoneNumber
                        alleTE(1) = .BusinessTelephoneNumber
                        alleTE(2) = .Business2TelephoneNumber
                        alleTE(3) = .CallbackTelephoneNumber
                        alleTE(4) = .CarTelephoneNumber
                        alleTE(5) = .CompanyMainTelephoneNumber
                        alleTE(6) = .HomeTelephoneNumber
                        alleTE(7) = .Home2TelephoneNumber
                        alleTE(8) = .ISDNNumber
                        alleTE(9) = .MobileTelephoneNumber
                        alleTE(10) = .OtherTelephoneNumber
                        alleTE(11) = .PagerNumber
                        alleTE(12) = .PrimaryTelephoneNumber
                        alleTE(13) = .RadioTelephoneNumber
                        alleTE(14) = .BusinessFaxNumber
                        alleTE(15) = .HomeFaxNumber
                        alleTE(16) = .OtherFaxNumber

                        For i = LBound(alleTE) To UBound(alleTE)
                            If Not alleTE(i) = vbNullString Then ' Fall: Telefonnummer vorhanden
                                If .UserProperties.Find(FeldNamen(i)) Is Nothing Then
                                    .UserProperties.Add(FeldNamen(i), Outlook.OlUserPropertyType.olText, True)
                                    speichern = True
                                End If
                                tempTelNr = HelferFunktionen.nurZiffern(alleTE(i), LandesVW)
                                If Not CStr(.UserProperties.Find(FeldNamen(i)).Value) = tempTelNr Then
                                    .UserProperties.Find(FeldNamen(i)).Value = tempTelNr
                                    speichern = True
                                End If
                            ElseIf Not .UserProperties.Find(FeldNamen(i)) Is Nothing Then ' Fall:Index vorhanden, Telefonnummer nicht
                                .UserProperties.Find(FeldNamen(i)).Delete()
                                speichern = True
                            End If
                        Next
                        If BGWIndexer.CancellationPending Then Exit For

                        If speichern Then .Save()
                    End With
                Else
                    BGWIndexer.ReportProgress(1)
                End If
                HelferFunktionen.NAR(item)
                Windows.Forms.Application.DoEvents()
            Next 'Item
            Elemente = Nothing
        End If
        If BGWIndexer.CancellationPending Then Exit Function
        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count)
            KontaktIndexer(LandesVW, Ordner.Folders.Item(iOrdner))
            iOrdner = iOrdner + 1
        Loop
        aktKontakt = Nothing

    End Function

#Region "Delegate"
    Private Sub SetProgressbar()
        With Me.ProgressBarIndex
            .Value += StatusWert
            Me.LabelAnzahl.Text = "Status: " & .Value & "/" & CStr(.Maximum) & KontaktName
        End With
    End Sub
    Private Sub SetProgressbarToMax()
        With Me.ProgressBarIndex
            .Value = .Maximum
        End With
        Me.ButtonSchließen.Enabled = True
        Me.ButtonStart.Enabled = True
        Me.ButtonAbbrechen.Enabled = False
    End Sub
    Private Sub SetProgressbarMax()
        Me.ProgressBarIndex.Maximum = Anzahl
    End Sub
#End Region
#Region "Backroundworker"

    Private Sub BGWIndexer_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BGWIndexer.DoWork
        ErmittleKontaktanzahl()

        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder
        Dim LandesVW As String = ini.Read(DateiPfad, "Optionen", "TBLandesVW", "0049")

        olNamespace = ThisAddIn.oApp.GetNamespace("MAPI")

        If ini.Read(DateiPfad, "Optionen", "CBKHO", "True") = "True" Then
            olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            KontaktIndexer(LandesVW, olfolder)
        Else
            KontaktIndexer(LandesVW, , olNamespace)
        End If
    End Sub
    Private Sub BGWIndexer_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BGWIndexer.ProgressChanged
        StatusWert = e.ProgressPercentage
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbar)
            Invoke(D)
        Else
            SetProgressbar()
        End If
    End Sub
    Private Sub BGWIndexer_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BGWIndexer.RunWorkerCompleted
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbarToMax)
            Invoke(D)
        Else
            SetProgressbarToMax()
        End If

        Dauer = Date.Now - Startzeit
        'MsgBox(Anzahl & ": " & Dauer.TotalMilliseconds & " ms")
        ini.Write(DateiPfad, "Optionen", "LLetzteIndizierung", CStr(Date.Now))
        HelferFunktionen.LogFile("Indizierung abgeschlossen: " & Anzahl & " Kontakte in " & Dauer.TotalMilliseconds & " ms")
        If CBool(ini.Read(DateiPfad, "Optionen", "CheckBoxIndexAutoStart", "False")) Then
            ini.Write(DateiPfad, "Optionen", "CheckBoxIndexAutoStart", CStr(Me.CheckBoxIndexAutoStart.Checked))
            Me.Close()
        End If

    End Sub
#End Region
#Region "Button"
    Private Sub ButtonSchließen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSchließen.Click
        ini.Write(DateiPfad, "Optionen", "CheckBoxIndexAutoStart", CStr(Me.CheckBoxIndexAutoStart.Checked))
        Me.Close()
    End Sub

    Private Sub ButtonAbbrechen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAbbrechen.Click
        If HelferFunktionen.FBDB_MsgBox("Das Abbrechen des Indizierungsvorgangs wird nicht empfohlen. Sind Sie sicher?", MsgBoxStyle.YesNo, "Indizierung: ButtonAbbrechen_Click") = MsgBoxResult.Yes Then
            BGWIndexer.CancelAsync()
            Me.ProgressBarIndex.Value = 0
            Me.LabelAnzahl.Text = "Status: Abgebrochen"
            Me.ButtonAbbrechen.Enabled = False
            Me.ButtonSchließen.Enabled = True
            Me.ButtonStart.Enabled = True
        End If
    End Sub
    Private Sub ButtonStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonStart.Click
        Start()
    End Sub
#End Region
End Class
