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
    Private Property ScaleFaktor As SizeF
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
        ScaleFaktor = GetScaling()

        Me.LStatus.Text = PDfltStringEmpty

        SetTelefonDaten()
    End Sub

    Private Sub SetTelefonDaten()
        Dim StdTel As Telefoniegerät

        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeGeräte)
        ' Leere das Control
        Me.ComboBoxFon.Items.Clear()
        For Each TelGerät As Telefoniegerät In XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) Not TG.IsFax)
            Me.ComboBoxFon.Items.Add(TelGerät.Name)
        Next

        ' Ausgewähltes Stamdardgerät
        StdTel = XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) TG.StdTelefon).FirstOrDefault
        If StdTel Is Nothing Then
            If XMLData.POptionen.PTelAnschluss Is Nothing OrElse XMLData.POptionen.PTelAnschluss.IsStringEmpty OrElse Me.ComboBoxFon.Items.Contains(XMLData.POptionen.PTelAnschluss) Then
                Me.ComboBoxFon.SelectedIndex = 0
            Else
                Me.ComboBoxFon.SelectedIndex = Me.ComboBoxFon.Items.IndexOf(XMLData.POptionen.PTelAnschluss)
                WählClient_SetStatus(PWählClientStatusLetztesGerät)
            End If
        Else
            WählClient_SetStatus(PWählClientStatusStandardGerät)
            Me.ComboBoxFon.SelectedIndex = Me.ComboBoxFon.Items.IndexOf(StdTel.Name)
        End If
        Me.CBCLIR.Checked = XMLData.POptionen.PCBCLIR
    End Sub

    Friend Sub SetOutlookKontakt(ByVal oContact As Outlook.ContactItem)
        Dim ImgPath As String
        ' Outlook Kontakt sichern
        OKontakt = oContact
        Me.ButtonZeigeKontakt.Enabled = True
        ' Status schreiben
        WählClient_SetStatus(PWählClientStatusLadeKontaktTelNr)
        ' Kopf Schreiben
        Me.Text = PWählClientFormText(String.Format("{0}{1}", oContact.FullName, If(oContact.CompanyName.IsNotStringNothingOrEmpty, String.Format(" ({0})", oContact.CompanyName), PDfltStringEmpty)))
        ' Direktwahl deaktivieren
        With Me.PanelDirektwahl
            .Enabled = False
            .Visible = False
        End With
        ' DataGridView auf Sollgröße maximieren 
        With Me.PanelKontaktwahl
            .Height = Me.PanelDirektwahl.Top + Me.PanelDirektwahl.Height
        End With
        ' DGV-Füllem
        With Me.dgvKontaktNr
            .EnableDoubleBuffered(True)
            .AllowUserToAddRows = False
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .RowHeadersVisible = False
            .DataSource = FillDatatable(oContact)

            With .Columns.Item("Nr")
                .HeaderText = "Nr."
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet
                .Width = CInt(25 * ScaleFaktor.Width)
            End With
            With .Columns.Item("Typ")
                .HeaderText = "Typ"
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet
                .Width = CInt(200 * ScaleFaktor.Width)
            End With
            With .Columns.Item("TelNr")
                .HeaderText = "Telefonnummern"
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End With
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
        Me.Text = PWählClientFormText(TelNr.Formatiert)
        ' Direktwahl deaktivieren
        With Me.PanelDirektwahl
            .Enabled = False
            .Visible = False
        End With
        ' DataGridView auf Sollgröße maximieren 
        With Me.PanelKontaktwahl
            .Height = Me.PanelDirektwahl.Top + Me.PanelDirektwahl.Height
        End With
        ' DGV-Füllem
        With Me.dgvKontaktNr
            .EnableDoubleBuffered(True)
            .AllowUserToAddRows = False
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            .RowHeadersVisible = False
            .DataSource = FillDatatable(TelNr)

            With .Columns.Item("Nr")
                .HeaderText = "Nr."
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet
                .Width = CInt(25 * ScaleFaktor.Width)
            End With
            With .Columns.Item("TelNr")
                .HeaderText = "Telefonnummer"
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End With
        End With
    End Sub

    Friend Sub SetDirektwahl()
        ' Kopf Schreiben
        Me.Text = PWählClientFormText("Direktwahl")
        ' DatagridView deaktivieren
        With Me.dgvKontaktNr
            .Enabled = False
            .Visible = False
        End With
        ' Panel auf Sollgröße maximieren
        With Me.PanelDirektwahl
            .Top = Me.dgvKontaktNr.Top
        End With
    End Sub

#Region "DataTable"
    Private Overloads Function FillDatatable(ByVal oContact As Outlook.ContactItem) As WählClientDataTable
        Dim tmpDataColumn As DataColumn
        Dim tmpDataRow As WählClientDataRow

        FillDatatable = New WählClientDataTable

        Dim tmpListofTelNr As List(Of Telefonnummer)

        With FillDatatable
            ' Spalten hinzufügen
            tmpDataColumn = .Columns.Add("Nr", GetType(Integer))
            tmpDataColumn = .Columns.Add("Typ", GetType(String))
            tmpDataColumn = .Columns.Add("TelNr", GetType(String))
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
        Me.BCancelCall.Visible = True
        Me.BCancelCall.Focus()
        Me.ComboBoxFon.Enabled = False
        Me.dgvKontaktNr.Enabled = False

        Dim tmpDataRow As WählClientDataRow

        ' Prüfung ob es sich bei der gewählten nummer um eine Mobilnummer handelt.
        If Me.dgvKontaktNr.SelectedRows.Count.IsNotZero Then
            tmpDataRow = CType(CType(Me.dgvKontaktNr.SelectedRows(0).DataBoundItem, DataRowView).Row, WählClientDataRow)

            If tmpDataRow.TelNr IsNot Nothing Then
                DialTelNr(tmpDataRow.TelNr, False)
            End If
        End If
    End Sub

    Private Sub DialTelNr(TelNr As Telefonnummer, ByVal AufbauAbbrechen As Boolean)

        Dim DialCode As String

        WählClient_SetStatus(PWählClientStatusTelNrAuswahl(TelNr.Formatiert))
        If Not TelNr.IstMobilnummer OrElse (XMLData.POptionen.PCBCheckMobil AndAlso MsgBox(PWählClientFrageMobil, MsgBoxStyle.YesNo, "Fritz!Box Wählclient") = vbYes) Then
            If AufbauAbbrechen Then
                DialCode = PDfltStringEmpty
                WählClient_SetStatus(PWählClientStatusAbbruch)
            Else
                Me.LStatus.Text = PWählClientBitteWarten : WählClient_SetStatus(PWählClientStatusVorbereitung)

                DialCode = TelNr.Unformatiert
                If XMLData.POptionen.PCBForceDialLKZ Then DialCode = DialCode.RegExReplace("^0(?=[1-9])", DfltWerteTelefonie.PDfltPreLandesKZ & TelNr.Landeskennzahl)

                DialCode = String.Format("{2}{1}{0}{3}", DialCode, XMLData.POptionen.PTBAmt, If(Me.CBCLIR.Checked, "*31#", PDfltStringEmpty), "#")

                WählClient_SetStatus(PWählClientStatusWählClient(DialCode))
                NLogger.Info("Wählclient SOAPDial: {0} über {1}", DialCode, CStr(Me.ComboBoxFon.SelectedItem))
            End If

            If WählClient.SOAPDial(DialCode, XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.Name.AreEqual(CStr(Me.ComboBoxFon.SelectedItem))), AufbauAbbrechen) Then
                If AufbauAbbrechen Then
                    Me.LStatus.Text = PWählClientDialHangUp
                Else
                    Me.LStatus.Text = PWählClientJetztAbheben
                End If
            Else
                Me.LStatus.Text = PWählClientDialFehler
            End If

            ' Einstellungen (Welcher Anschluss, CLIR...) speichern
            XMLData.POptionen.PCBCLIR = Me.CBCLIR.Checked
            XMLData.POptionen.PTelAnschluss = ComboBoxFon.SelectedText
            ' Timer zum automatischen Schließen des Fensters starten
            If XMLData.POptionen.PCBAutoClose Then TimerSchließen = SetTimer(XMLData.POptionen.PTBEnblDauer * 1000)
            Me.BCancelCall.Enabled = True
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
        If Me.InvokeRequired Then
            Me.Invoke(New DlgFormWählClient(AddressOf AutoClose))
        Else
            Me.Close()
            Me.Dispose(True)
        End If
    End Sub

#Region "Status"
    Private Sub WählClient_SetStatus(Status As String) Handles WählClient.SetStatus
        With Me.TBStatus
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
            Case Me.BWählenDirektwahl.Name
                Using tmpTelNr As New Telefonnummer With {.SetNummer = Me.TBDirektwahl.Text}
                    DialTelNr(tmpTelNr, False)
                End Using
            Case Me.BCancelCall.Name
                Using tmpTelNr As New Telefonnummer
                    DialTelNr(tmpTelNr, True)
                End Using
                If Not TimerSchließen Is Nothing Then TimerSchließen.Stop()
                dgvKontaktNr.ClearSelection() ' Ein erneutes Wählen ermöglichen
            Case Me.BVIP.Name
            Case Me.ButtonZeigeKontakt.Name
                OKontakt.Display()
            Case Me.BSchließen.Name
                Me.Close()
        End Select
    End Sub

    Private Sub FormWählclient_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If PKontaktbild IsNot Nothing Then PKontaktbild.Dispose()
        Me.Dispose(True)
    End Sub
End Class