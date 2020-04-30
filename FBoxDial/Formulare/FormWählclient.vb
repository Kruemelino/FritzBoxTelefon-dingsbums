Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Timers
Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Public Class FormWählclient
    Implements IDisposable

#Region "Properties"
    Private Shared Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private Property OKontakt As Outlook.ContactItem
    Private Property PKontaktbild As Bitmap
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

        ' Lade die Telefone
        SetTelefonDaten()
    End Sub

    Private Sub SetTelefonDaten()
        Dim StdTel As Telefoniegerät

        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeGeräte)
        ' Leere das Control
        ComboBoxFon.Items.Clear()
        For Each TelGerät As Telefoniegerät In XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) Not TG.IsFax)
            ComboBoxFon.Items.Add(TelGerät.Name)
        Next

        ' Ausgewähltes Stamdardgerät
        StdTel = XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) TG.StdTelefon).FirstOrDefault
        If StdTel Is Nothing Then
            If XMLData.POptionen.PTelAnschluss Is Nothing OrElse XMLData.POptionen.PTelAnschluss.IsStringEmpty OrElse ComboBoxFon.Items.Contains(XMLData.POptionen.PTelAnschluss) Then
                ComboBoxFon.SelectedIndex = 0
            Else
                ComboBoxFon.SelectedIndex = ComboBoxFon.Items.IndexOf(XMLData.POptionen.PTelAnschluss)
                WählClient_SetStatus(PWählClientStatusLetztesGerät)
            End If
        Else
            WählClient_SetStatus(PWählClientStatusStandardGerät)
            ComboBoxFon.SelectedIndex = ComboBoxFon.Items.IndexOf(StdTel.Name)
        End If
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
        Text = PWählClientFormText(String.Format("{0}{1}", oContact.FullName, If(oContact.CompanyName.IsNotStringNothingOrEmpty, String.Format(" ({0})", oContact.CompanyName), PDfltStringEmpty)))
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
            .DisplayMember = NameOf(Telefonnummer.Unformatiert)
            .ValueMember = NameOf(Telefonnummer.Unformatiert)
            .DataSource = GetTelNrList(XMLData.PTelefonie.CALLListe?.Einträge)
            .SelectedItem = Nothing
        End With
    End Sub

    Private Function GetTelNrList(ByVal Telefonate As List(Of Telefonat)) As List(Of Telefonnummer)
        GetTelNrList = New List(Of Telefonnummer)
        For Each Tel As Telefonat In Telefonate
            GetTelNrList.Add(Tel.GegenstelleTelNr)
        Next
    End Function


#Region "DataTable"
    Private Overloads Function FillDatatable(ByVal oContact As Outlook.ContactItem) As WählClientDataTable
        'Dim tmpDataColumn As DataColumn
        Dim tmpDataRow As WählClientDataRow

        FillDatatable = New WählClientDataTable

        Dim tmpListofTelNr As List(Of Telefonnummer)

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

        BCancelCall.Visible = True
        BCancelCall.Focus()
        ComboBoxFon.Enabled = False
        dgvKontaktNr.Enabled = False

        WählClient_SetStatus(PWählClientStatusTelNrAuswahl(TelNr.Formatiert))
        If Not TelNr.IstMobilnummer OrElse (XMLData.POptionen.PCBCheckMobil AndAlso MsgBox(PWählClientFrageMobil, MsgBoxStyle.YesNo, "Fritz!Box Wählclient") = vbYes) Then
            If AufbauAbbrechen Then
                DialCode = PDfltStringEmpty
                WählClient_SetStatus(PWählClientStatusAbbruch)
            Else
                LStatus.Text = PWählClientBitteWarten : WählClient_SetStatus(PWählClientStatusVorbereitung)

                DialCode = TelNr.Unformatiert.RegExReplace("#{1}$", PDfltStringEmpty)
                If XMLData.POptionen.PCBForceDialLKZ Then DialCode = DialCode.RegExReplace("^0(?=[1-9])", DfltWerteTelefonie.PDfltVAZ & TelNr.Landeskennzahl)

                DialCode = $"{If(CBCLIR.Checked, "*31#", PDfltStringEmpty)}{XMLData.POptionen.PTBAmt}{DialCode}#"

                WählClient_SetStatus(PWählClientStatusWählClient(DialCode))
                NLogger.Info("Wählclient SOAPDial: {0} über {1}", DialCode, CStr(ComboBoxFon.SelectedItem))
            End If

            If WählClient.SOAPDial(DialCode, XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.Name.AreEqual(CStr(ComboBoxFon.SelectedItem))), AufbauAbbrechen) Then
                If AufbauAbbrechen Then
                    LStatus.Text = PWählClientDialHangUp
                Else
                    LStatus.Text = PWählClientJetztAbheben
                End If
            Else
                LStatus.Text = PWählClientDialFehler
            End If

            ' Einstellungen (Welcher Anschluss, CLIR...) speichern
            XMLData.POptionen.PCBCLIR = CBCLIR.Checked
            XMLData.POptionen.PTelAnschluss = ComboBoxFon.SelectedText
            ' Timer zum automatischen Schließen des Fensters starten
            If XMLData.POptionen.PCBCloseWClient Then TimerSchließen = SetTimer(XMLData.POptionen.PTBWClientEnblDauer * 1000)
            BCancelCall.Enabled = True
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
                OKontakt.Display()

            Case BSchließen.Name
                Close()

        End Select
    End Sub

    Private Sub FormWählclient_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If PKontaktbild IsNot Nothing Then PKontaktbild.Dispose()
        Dispose(True)
    End Sub
End Class