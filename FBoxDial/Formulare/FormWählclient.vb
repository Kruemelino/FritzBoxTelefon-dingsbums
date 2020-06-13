Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Net.NetworkInformation
Imports System.Timers
Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Public Class FormWählclient
    Implements IDisposable

#Region "Properties"
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property OKontakt As Outlook.ContactItem
    Private Property OExchangeNutzer As Outlook.ExchangeUser
    Private Property PKontaktbild As Bitmap

    Private Property PhonerApp As Phoner
#End Region

#Region "Delegaten"
    Private Delegate Sub DlgFormWählClient()
    Private Delegate Sub DlgStatus(ByVal StatusText As String)
#End Region

#Region "WithEvents"
    Private WithEvents WählClient As FritzBoxWählClient
    Private WithEvents TimerSchließen As Timers.Timer
#End Region

    Public Sub New(ByVal WClient As FritzBoxWählClient)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        WählClient = WClient

        LStatus.Text = PDfltStringEmpty

        ' Initiere Phoner, wenn erforderlich
        If XMLData.POptionen.PCBPhoner Then
            PhonerApp = New Phoner
            If Not PhonerApp.PhonerReady Then
                WählClient_SetStatus(PWählClientPhonerInaktiv)
                PhonerApp.Dispose()
                PhonerApp = Nothing
            End If
        End If

        ' Lade die Telefone
        SetTelefonDaten()
    End Sub

    Private Sub SetTelefonDaten()
        'Dim StdTel As Telefoniegerät

        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeGeräte)
        ' Leere das Control
        ComboBoxFon.Items.Clear()

        ' schreibe alle geeigneten Telefone rein (kein Fax, keine IP-Telefonie, keine AB)
        With ComboBoxFon
            .DataBindings.Clear()
            If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then
                .DisplayMember = NameOf(Telefoniegerät.Name)
                .ValueMember = NameOf(Telefoniegerät.UPnPDialport)
                ' Nur FON, DECT, S0 und Phoner, wenn Phoner aktiv
                .DataSource = XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) Not TG.IsFax And (TG.TelTyp = DfltWerteTelefonie.TelTypen.FON Or TG.TelTyp = DfltWerteTelefonie.TelTypen.DECT Or TG.TelTyp = DfltWerteTelefonie.TelTypen.S0 Or (TG.IsPhoner And PhonerApp IsNot Nothing))).ToList

                ' Ausgewähltes Standardgerät
                .SelectedItem = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.StdTelefon)
                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das zuletzt genutzte Telefon
                If .SelectedItem Is Nothing Then
                    WählClient_SetStatus(PWählClientStatusLetztesGerät)
                    .SelectedItem = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.ZuletztGenutzt)
                End If
                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das erste in der liste
                If .SelectedItem Is Nothing Then
                    WählClient_SetStatus(PWählClientStatus1Gerät)
                    .SelectedIndex = 0

                End If
            Else
                WählClient_SetStatus(PWählClientStatusFehlerGerät)
            End If
        End With

        CBCLIR.Checked = XMLData.POptionen.PCBCLIR
    End Sub


    Friend Sub SetOutlookKontakt(ByVal oContact As Outlook.ContactItem)
        Dim ImgPath As String
        ' Outlook Kontakt sichern
        OKontakt = oContact
        ButtonZeigeKontakt.Enabled = True
        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeKontaktTelNr)
        ' Kopf Schreiben
        Text = PWählClientFormText($"{oContact.FullName}{If(oContact.CompanyName.IsNotStringEmpty, $" ({oContact.CompanyName})", PDfltStringEmpty)}")
        ' Direktwahl deaktivieren
        With PanelDirektwahl
            .Enabled = False
            .Visible = False
        End With
        ' DataGridView auf Sollgröße maximieren 
        With PanelKontaktwahl
            .Height = PanelDirektwahl.Top + PanelDirektwahl.Height
        End With

        ' DGV-Füllem
        With dgvKontaktNr

            .AddTextColumn("Nr", "Nr.", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), 25)
            .AddTextColumn("Typ", "Typ", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), 200)
            .AddTextColumn("TelNr", "Telefonnummern", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), DataGridViewAutoSizeColumnMode.Fill)

            .DataSource = FillDatatable(oContact)
        End With

        ' Kontaktbild anzeigen

        ImgPath = KontaktBild(oContact)

        If ImgPath.IsNotStringEmpty Then
            Dim orgImage As Image
            Using fs As New IO.FileStream(ImgPath, IO.FileMode.Open)
                orgImage = Image.FromStream(fs)
            End Using
            DelKontaktBild(ImgPath)

            With New Size(PicBoxKontaktBild.Width, CInt((PicBoxKontaktBild.Width * orgImage.Size.Height) / orgImage.Size.Width))
                PKontaktbild = New Bitmap(.Width, .Height)

                Using g As Graphics = Graphics.FromImage(PKontaktbild)
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.DrawImage(orgImage, 0, 0, .Width, .Height)
                End Using
            End With
            PicBoxKontaktBild.Image = PKontaktbild
        Else
            PicBoxKontaktBild.Visible = False
        End If

    End Sub

    Friend Sub SetOutlookKontakt(ByVal oExchangeUser As Outlook.ExchangeUser)
        'Dim ImgPath As String
        ' Outlook Kontakt sichern
        OExchangeNutzer = oExchangeUser
        ButtonZeigeKontakt.Enabled = True
        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeKontaktTelNr)
        ' Kopf Schreiben
        Text = PWählClientFormText($"{oExchangeUser.Name}{If(oExchangeUser.CompanyName.IsNotStringEmpty, $" ({oExchangeUser.CompanyName})", PDfltStringEmpty)}")
        ' Direktwahl deaktivieren
        With PanelDirektwahl
            .Enabled = False
            .Visible = False
        End With
        ' DataGridView auf Sollgröße maximieren 
        With PanelKontaktwahl
            .Height = PanelDirektwahl.Top + PanelDirektwahl.Height
        End With

        ' DGV-Füllem
        With dgvKontaktNr

            .AddTextColumn("Nr", "Nr.", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), 25)
            .AddTextColumn("Typ", "Typ", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), 200)
            .AddTextColumn("TelNr", "Telefonnummern", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), DataGridViewAutoSizeColumnMode.Fill)

            .DataSource = FillDatatable(oExchangeUser)
        End With

        ' Kontaktbild anzeigen

        'ImgPath = KontaktBild(oContact)

        'If ImgPath.IsNotStringEmpty Then
        '    Dim orgImage As Image
        '    Using fs As New IO.FileStream(ImgPath, IO.FileMode.Open)
        '        orgImage = Image.FromStream(fs)
        '    End Using
        '    DelKontaktBild(ImgPath)

        '    With New Size(PicBoxKontaktBild.Width, CInt((PicBoxKontaktBild.Width * orgImage.Size.Height) / orgImage.Size.Width))
        '        PKontaktbild = New Bitmap(.Width, .Height)

        '        Using g As Graphics = Graphics.FromImage(PKontaktbild)
        '            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        '            g.DrawImage(orgImage, 0, 0, .Width, .Height)
        '        End Using
        '    End With
        '    PicBoxKontaktBild.Image = PKontaktbild
        'Else
        '    PicBoxKontaktBild.Visible = False
        'End If

    End Sub

    Friend Sub SetTelefonnummer(ByVal TelNr As Telefonnummer)
        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeTelNr)
        ' Kopf Schreiben
        Text = PWählClientFormText(TelNr.Formatiert)
        ' Direktwahl deaktivieren
        With PanelDirektwahl
            .Enabled = False
            .Visible = False
        End With
        ' DataGridView auf Sollgröße maximieren 
        With PanelKontaktwahl
            .Height = PanelDirektwahl.Top + PanelDirektwahl.Height
        End With
        ' DGV-Füllem
        With dgvKontaktNr
            .AddTextColumn("Nr", "Nr.", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), 25)
            '.AddTextColumn("Typ", "Typ", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), 200)
            .AddTextColumn("TelNr", "Telefonnummern", DataGridViewContentAlignment.MiddleLeft, GetType(Integer), DataGridViewAutoSizeColumnMode.Fill)

            .DataSource = FillDatatable(TelNr)
        End With
    End Sub

    Friend Sub SetDirektwahl()
        ' Kopf Schreiben
        Text = PWählClientFormText("Direktwahl")
        ' DatagridView deaktivieren
        With dgvKontaktNr
            .Enabled = False
            .Visible = False
        End With
        ' Panel auf Sollgröße maximieren
        PanelDirektwahl.Top = dgvKontaktNr.Top

        ' Wahlwiederhohlung in Combobox schreiben
        With CBoxDirektwahl
            .DataBindings.Clear()
            If XMLData.PTelefonie.CALLListe IsNot Nothing AndAlso XMLData.PTelefonie.CALLListe.Any Then
                .DisplayMember = NameOf(Telefonnummer.Unformatiert)
                .ValueMember = NameOf(Telefonnummer.Unformatiert)
                .DataSource = GetTelNrList(XMLData.PTelefonie.CALLListe)
            End If
            .SelectedItem = Nothing
        End With
    End Sub

    ''' <summary>
    ''' Gibt die zuletzt gewählten Telefonnummern der Wahlwiederholungsliste zurück
    ''' </summary>
    ''' <param name="Telefonate">Wahlwiederhohlungsliste</param>
    ''' <returns>Liste der Telefonnummern</returns>
    Private Function GetTelNrList(ByVal Telefonate As List(Of Telefonat)) As List(Of Telefonnummer)
        GetTelNrList = New List(Of Telefonnummer)
        For Each Tel As Telefonat In Telefonate
            GetTelNrList.Add(Tel.GegenstelleTelNr)
        Next
    End Function


#Region "DataTable"
    Private Overloads Function FillDatatable(ByVal oContact As Outlook.ContactItem) As WählClientDataTable
        Dim tmpDataRow As WählClientDataRow
        Dim tmpListofTelNr As List(Of Telefonnummer)

        FillDatatable = New WählClientDataTable

        With FillDatatable
            ' Spalten hinzufügen
            .Columns.Add("Nr", GetType(Integer))
            .Columns.Add("Typ", GetType(String))
            .Columns.Add("TelNr", GetType(String))
            ' Zeilen hinzufügen
            tmpListofTelNr = GetKontaktTelNrList(oContact)
            For Each TelNr As Telefonnummer In tmpListofTelNr
                tmpDataRow = CType(.Rows.Add(tmpListofTelNr.IndexOf(TelNr) + 1, TelNr.OutlookTyp, TelNr.Formatiert), WählClientDataRow)
                tmpDataRow.TelNr = TelNr
            Next
        End With
    End Function
    Private Overloads Function FillDatatable(ByVal oExchangeNutzer As Outlook.ExchangeUser) As WählClientDataTable
        Dim tmpDataRow As WählClientDataRow
        Dim tmpListofTelNr As List(Of Telefonnummer)

        FillDatatable = New WählClientDataTable

        With FillDatatable
            ' Spalten hinzufügen
            .Columns.Add("Nr", GetType(Integer))
            .Columns.Add("Typ", GetType(String))
            .Columns.Add("TelNr", GetType(String))
            ' Zeilen hinzufügen
            tmpListofTelNr = GetKontaktTelNrList(oExchangeNutzer)
            For Each TelNr As Telefonnummer In tmpListofTelNr
                tmpDataRow = CType(.Rows.Add(tmpListofTelNr.IndexOf(TelNr) + 1, TelNr.OutlookTyp, TelNr.Formatiert), WählClientDataRow)
                tmpDataRow.TelNr = TelNr
            Next
        End With
    End Function

    Private Overloads Function FillDatatable(ByVal TelNr As Telefonnummer) As WählClientDataTable
        Dim tmpDataColumn As DataColumn
        Dim tmpDataRow As WählClientDataRow

        FillDatatable = New WählClientDataTable

        With FillDatatable
            ' Spalten hinzufügen
            tmpDataColumn = .Columns.Add("Nr", GetType(Integer))
            tmpDataColumn = .Columns.Add("TelNr", GetType(String))
            ' Zeilen hinzufügen
            tmpDataRow = CType(.Rows.Add(1, TelNr.Formatiert), WählClientDataRow)
            tmpDataRow.TelNr = TelNr
        End With

    End Function
#End Region
    Private Sub DgvKontaktNr_SelectionChanged(sender As Object, e As EventArgs)
        Dim tmpDataRow As WählClientDataRow

        ' Prüfung ob es sich bei der gewählten nummer um eine Mobilnummer handelt.
        If dgvKontaktNr.SelectedRows.Count.IsNotZero Then
            tmpDataRow = CType(CType(dgvKontaktNr.SelectedRows(0).DataBoundItem, DataRowView).Row, WählClientDataRow)

            If tmpDataRow.TelNr IsNot Nothing Then
                DialTelNr(tmpDataRow.TelNr, False)
            End If
        End If
    End Sub

    Private Sub DialTelNr(TelNr As Telefonnummer, ByVal AufbauAbbrechen As Boolean)

        Dim DialCode As String
        Dim TelGerät As Telefoniegerät
        Dim Erfolreich As Boolean

        BCancelCall.Visible = True
        BCancelCall.Focus()
        ComboBoxFon.Enabled = False
        dgvKontaktNr.Enabled = False

        TelGerät = CType(ComboBoxFon.SelectedItem, Telefoniegerät)

        If TelGerät IsNot Nothing Then

            WählClient_SetStatus(PWählClientStatusTelNrAuswahl(TelNr.Formatiert))
            If Not TelNr.IstMobilnummer OrElse (XMLData.POptionen.PCBCheckMobil AndAlso MsgBox(PWählClientFrageMobil, MsgBoxStyle.YesNo, "Fritz!Box Wählclient") = vbYes) Then
                If AufbauAbbrechen Then
                    DialCode = PDfltStringEmpty
                    WählClient_SetStatus(PWählClientStatusAbbruch)
                Else
                    LStatus.Text = PWählClientBitteWarten : WählClient_SetStatus(PWählClientStatusVorbereitung)
                    ' Entferne 1x # am Ende
                    DialCode = TelNr.Unformatiert.RegExRemove("#{1}$")

                    ' Füge VAZ und LKZ hinzu, wenn gewünscht
                    If XMLData.POptionen.PCBForceDialLKZ Then DialCode = DialCode.RegExReplace("^0(?=[1-9])", DfltWerteTelefonie.PDfltVAZ & TelNr.Landeskennzahl)

                    ' Rufnummerunterdrückung
                    DialCode = $"{If(CBCLIR.Checked, "*31#", PDfltStringEmpty)}{XMLData.POptionen.PTBAmt}{DialCode}#"

                    WählClient_SetStatus(PWählClientStatusWählClient(DialCode))
                    NLogger.Info("Wählclient SOAPDial: {0} über {1}", DialCode, TelGerät.Name)
                End If

                If TelGerät.IsPhoner Then
                    ' Telefonat an Phoner übergeben
                    NLogger.Info("Wählclient an Phoner: {0} über {1}", DialCode, TelGerät.Name)
                    Erfolreich = PhonerApp.DialPhoner(DialCode, AufbauAbbrechen)
                Else
                    ' Telefonat üper SOAP an Fritz!Box weiterreichen
                    Erfolreich = WählClient.SOAPDial(DialCode, TelGerät, AufbauAbbrechen)
                End If

                ' Ergebnis auswerten 
                If Erfolreich Then
                    If AufbauAbbrechen Then
                        LStatus.Text = PWählClientDialHangUp
                    Else
                        LStatus.Text = PWählClientJetztAbheben
                    End If

                    ' Abbruch-Button aktivieren, wenn Anruf abgebrochen
                    BCancelCall.Enabled = Not AufbauAbbrechen
                    ' Einstellungen (Welcher Anschluss, CLIR...) speichern
                    XMLData.POptionen.PCBCLIR = CBCLIR.Checked
                    ' Standard-Gerät speichern

                    If Not TelGerät.ZuletztGenutzt Then
                        ' Entferne das Flag bei allen anderen Geräten
                        ' (eigentlich reicht es, das Flag bei dem einen Gerät zu entfernen. Sicher ist sicher.
                        XMLData.PTelefonie.Telefoniegeräte.ForEach(Sub(TE) TE.ZuletztGenutzt = False)
                        ' Flag setzen
                        TelGerät.ZuletztGenutzt = True
                    End If
                    ' Timer zum automatischen Schließen des Fensters starten
                    If XMLData.POptionen.PCBCloseWClient Then TimerSchließen = SetTimer(XMLData.POptionen.PTBWClientEnblDauer * 1000)
                Else
                    LStatus.Text = PWählClientDialFehler
                End If

            End If
        Else
            LStatus.Text = PWählClientDialFehler
        End If
    End Sub

    Private Sub FormWählclient_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        AddHandler dgvKontaktNr.SelectionChanged, AddressOf DgvKontaktNr_SelectionChanged
    End Sub

    Private Sub DgvKontaktNr_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles dgvKontaktNr.DataBindingComplete
        With dgvKontaktNr
            .CurrentCell = Nothing
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .ClearSelection()
        End With
    End Sub

    Private Sub TimerSchließen_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TimerSchließen.Elapsed
        TimerSchließen = KillTimer(TimerSchließen)
        AutoClose()
    End Sub
    Private Sub AutoClose()
        If InvokeRequired Then
            Invoke(New DlgFormWählClient(AddressOf AutoClose))
        Else
            Close()
            Dispose(True)
        End If
    End Sub

#Region "Status"
    Private Sub WählClient_SetStatus(Status As String) Handles WählClient.SetStatus
        With TBStatus
            If .InvokeRequired Then
                .Invoke(New DlgStatus(AddressOf WählClient_SetStatus), Status)
            Else
                .AppendText(String.Format("{0}{1}", If(.Text.IsStringEmpty, PDfltStringEmpty, PDflt1NeueZeile), Status))
                NLogger.Debug(Status)
            End If
        End With
    End Sub
#End Region

    Private Sub Button_Click(sender As Object, e As EventArgs) Handles BWählenDirektwahl.Click,
                                                                       BCancelCall.Click,
                                                                       BVIP.Click,
                                                                       ButtonZeigeKontakt.Click,
                                                                       BSchließen.Click
        Select Case CType(sender, Button).Name
            Case BWählenDirektwahl.Name
                Using tmpTelNr As New Telefonnummer With {.SetNummer = CBoxDirektwahl.Text}
                    DialTelNr(tmpTelNr, False)
                End Using

            Case BCancelCall.Name
                Using tmpTelNr As New Telefonnummer
                    DialTelNr(tmpTelNr, True)
                End Using

                If Not TimerSchließen Is Nothing Then TimerSchließen.Stop()
                dgvKontaktNr.ClearSelection() ' Ein erneutes Wählen ermöglichen
            Case BVIP.Name
            Case ButtonZeigeKontakt.Name
                OKontakt?.Display()
                OExchangeNutzer?.Details()

            Case BSchließen.Name
                Close()

        End Select
    End Sub

    Private Sub FormWählclient_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If PKontaktbild IsNot Nothing Then PKontaktbild.Dispose()
        Dispose(True)
    End Sub
End Class