Imports System.IO
Imports System.Xml
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.ComponentModel

Public Class formAdressbuch

    Private C_FB As FritzBox
    Private C_DP As DataProvider
    Private C_KF As Contacts
    Private C_XML As XML
    Private tmp As String
    Private BS As BindingSource

    Private XMLAdressbuch As XmlDocument
    Public Sub New(ByVal XMLKlasse As XML, ByVal FritzBoxKlasse As FritzBox, ByVal DataProviderKlasse As DataProvider, KontaktKlasse As Contacts)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_FB = FritzBoxKlasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse
        C_XML = XMLKlasse

        'Me.DGVAdressbuch.Columns.Item("Adrbk_ID").Visible = False
        'Me.DGVAdressbuch.Columns.Item("Uniqueid").Visible = False
        'Me.DGVAdressbuch.Columns.Item("AdrBk_Mod_Time").Visible = False
        Me.Show()
    End Sub

    Private Sub ÖffnenToolStripButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ÖffnenToolStripButton.Click
        Dim myStream As Stream = Nothing
        Dim myStreamReader As StreamReader
        XMLAdressbuch = New XmlDocument()
        With OFDAdressdbuch
            .Filter = "XML Adressbuch (*.xml)|*.xml|Alle Dateien (*.*)|*.*"
            .FilterIndex = 1
            .RestoreDirectory = True

            If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                Try
                    myStream = .OpenFile()
                    If (myStream IsNot Nothing) Then
                        myStreamReader = New StreamReader(myStream)

                        XMLAdressbuch.LoadXml(myStreamReader.ReadToEnd)


                        myStreamReader.Close()
                    End If
                Catch Ex As Exception

                Finally
                    ' Check this again, since we need to make sure we didn't throw an exception on open.
                    If (myStream IsNot Nothing) Then
                        myStream.Close()
                    End If
                End Try
            End If

            ReadXMLTelefonbuch(XMLAdressbuch)
            'FillDGV(XMLAdressbuch, "person")             ' Test

        End With
    End Sub

    Private Sub ImportToolStrip_Click(ByVal sender As Object, e As EventArgs) Handles ImportToolStrip.Click
        Dim XMLImportiertesAdressbuch As XmlDocument
        Dim ImportiertesAdressbuch As S_Adressbuch

        XMLImportiertesAdressbuch = C_FB.DownloadAddressbook("0", "Telefonbuch")
        ImportiertesAdressbuch = ReadXMLTelefonbuch(XMLImportiertesAdressbuch)
        FillDGVAdressbuch(ImportiertesAdressbuch)
    End Sub

    Private Sub FillDGVAdressbuch(ByVal Telefonbuch As S_Adressbuch)

        Me.BS = New BindingSource
        Me.BS.DataSource = Telefonbuch.EintragsListe

        Me.DGVAdressbuch.AutoGenerateColumns = False
        Me.DGVAdressbuch.DataSource = BS
        Me.DGVAdressbuch.ReadOnly = False
        Me.DGVAdressbuch.RowHeadersVisible = False
        Me.DGVAdressbuch.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
        Me.DGVAdressbuch.Enabled = True
        AddHandler DGVAdressbuch.CellValueChanged, AddressOf DGVAdressbuch_CellValueChanged
    End Sub

    'nid --> Anzahl
    '<phonebooks>
    '    <phonebook>
    '        <contact>
    '	        <category>0</category>
    '	        <person>
    '		        <realName>Vorname Nachname</realName>,
    '               <imageURL>file:///var/InternerSpeicher/FRITZ/fonpix/1302284103-0.jpg</imageURL>
    '	        </person>
    '	        <telephony nid="3">
    '		        <number type="home" prio="1" id="0">0123456789</number>
    '		        <number type="mobile" prio="0" id="1">0123456789</number>
    '		        <number type="work" prio="0" id="2">0123456789</number>
    '	        </telephony>
    '	        <services nid="1">
    '		        <email classifier="private" id="0">vorname.nachname@online.de</email>
    '	        </services>
    '	        <setup>
    '		        <ringTone/>
    '		        <ringVolume/>
    '            </setup>
    '	        <mod_time>1416252727</mod_time>
    '	        <uniqueid>28</uniqueid>
    '        </contact>
    '    <phonebook>
    '<phonebooks>
#Region "Telefonbuch Datenstruktur"

    Private Enum TelNrType
        home = 0
        mobile = 1
        work = 2
        fax_work = 3
    End Enum

    Private Structure S_Adressbuch
        Public EintragsListe As List(Of S_AdressbuchEintrag)
    End Structure

    Private Structure S_AdressbuchEintrag
        Private _ID As Integer
        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Private _category As Boolean
        Public Property Category() As Boolean
            Get
                Return _category
            End Get
            Set(ByVal value As Boolean)
                _category = value
            End Set
        End Property

        Private _RealName As String
        Public Property RealName() As String
            Get
                Return _RealName
            End Get
            Set(ByVal value As String)
                _RealName = value
            End Set
        End Property

        Private _Mod_Time As String
        Public Property Mod_Time() As String
            Get
                Return _Mod_Time
            End Get
            Set(ByVal value As String)
                _Mod_Time = value
            End Set
        End Property

        Private _uniqueid As String
        Public Property Uniqueid() As String
            Get
                Return _uniqueid
            End Get
            Set(ByVal value As String)
                _uniqueid = value
            End Set
        End Property

#Region "Telefonnummern Home"
        Private _TelNr_Home_ID As Integer
        Friend Property TelNr_Home_ID() As Integer
            Get
                Return _TelNr_Home_ID
            End Get
            Set(ByVal value As Integer)
                _TelNr_Home_ID = value
            End Set
        End Property

        Private _TelNr_Home_prio As Boolean
        Public Property TelNr_Home_Prio() As Boolean
            Get
                Return _TelNr_Home_prio
            End Get
            Set(ByVal value As Boolean)
                _TelNr_Home_prio = value
            End Set
        End Property

        Private _TelNr_Home_TelNr As String
        Public Property TelNr_Home_TelNr() As String
            Get
                Return _TelNr_Home_TelNr
            End Get
            Set(ByVal value As String)
                _TelNr_Home_TelNr = value
            End Set
        End Property
#End Region

#Region "Telefonnummern Work"
        Private _TelNr_Work_ID As Integer
        Public Property TelNr_Work_ID() As Integer
            Get
                Return _TelNr_Work_ID
            End Get
            Set(ByVal value As Integer)
                _TelNr_Work_ID = value
            End Set
        End Property

        Private _TelNr_Work_prio As Boolean
        Public Property TelNr_Work_Prio() As Boolean
            Get
                Return _TelNr_Work_prio
            End Get
            Set(ByVal value As Boolean)
                _TelNr_Work_prio = value
            End Set
        End Property

        Private _TelNr_Work_TelNr As String
        Public Property TelNr_Work_TelNr() As String
            Get
                Return _TelNr_Work_TelNr
            End Get
            Set(ByVal value As String)
                _TelNr_Work_TelNr = value
            End Set
        End Property
#End Region

#Region "Telefonnummern Mobil"
        Private _TelNr_Mobil_ID As Integer
        Public Property TelNr_Mobil_ID() As Integer
            Get
                Return _TelNr_Mobil_ID
            End Get
            Set(ByVal value As Integer)
                _TelNr_Mobil_ID = value
            End Set
        End Property

        Private _TelNr_Mobil_prio As Boolean
        Public Property TelNr_Mobil_Prio() As Boolean
            Get
                Return _TelNr_Mobil_prio
            End Get
            Set(ByVal value As Boolean)
                _TelNr_Mobil_prio = value
            End Set
        End Property

        Private _TelNr_Mobil_TelNr As String
        Public Property TelNr_Mobil_TelNr() As String
            Get
                Return _TelNr_Mobil_TelNr
            End Get
            Set(ByVal value As String)
                _TelNr_Mobil_TelNr = value
            End Set
        End Property
#End Region

#Region "Telefonnummern Fax"
        Private _TelNr_Fax_ID As Integer
        Public Property TelNr_Fax_ID() As Integer
            Get
                Return _TelNr_Fax_ID
            End Get
            Set(ByVal value As Integer)
                _TelNr_Fax_ID = value
            End Set
        End Property

        Private _TelNr_Fax_prio As Boolean
        Public Property TelNr_Fax_Prio() As Boolean
            Get
                Return _TelNr_Fax_prio
            End Get
            Set(ByVal value As Boolean)
                _TelNr_Fax_prio = value
            End Set
        End Property

        Private _TelNr_Fax_TelNr As String
        Public Property TelNr_Fax_TelNr() As String
            Get
                Return _TelNr_Fax_TelNr
            End Get
            Set(ByVal value As String)
                _TelNr_Fax_TelNr = value
            End Set
        End Property
#End Region

#Region "E-Mail"
        Private _E_Mail_ID As Integer
        Public Property E_Mail_ID() As Integer
            Get
                Return _E_Mail_ID
            End Get
            Set(ByVal value As Integer)
                _E_Mail_ID = value
            End Set
        End Property

        Private _classifier As String
        Public Property Classifier() As String
            Get
                Return _classifier
            End Get
            Set(ByVal value As String)
                _classifier = value
            End Set
        End Property

        Private _EMail As String
        Public Property EMail() As String
            Get
                Return _EMail
            End Get
            Set(ByVal value As String)
                _EMail = value
            End Set
        End Property
#End Region

    End Structure

#End Region

#Region "Telefonbuch Interaktionen"
    Private Function ReadXMLTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As S_Adressbuch
        Dim Adressbuch As New S_Adressbuch
        Dim XMLTelBuchEintraege As XmlNodeList
        Dim AdressbuchEintrag As S_AdressbuchEintrag
        Dim aktEintragsListe As New List(Of S_AdressbuchEintrag)


        Dim i As Integer = 0

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")

        For Each XMLTelefonbuchEintrag As XmlNode In XMLTelBuchEintraege
            AdressbuchEintrag = New S_AdressbuchEintrag
            i += 1
            AdressbuchEintrag.ID = i

            For Each XMLEintragWerte As XmlElement In XMLTelefonbuchEintrag.ChildNodes
                Select Case XMLEintragWerte.Name
                    Case "category"
                        If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then AdressbuchEintrag.Category = CBool(XMLEintragWerte.InnerText)
                    Case "mod_time"
                        AdressbuchEintrag.Mod_Time = XMLEintragWerte.InnerText
                    Case "uniqueid"
                        AdressbuchEintrag.Uniqueid = XMLEintragWerte.InnerText
                    Case "person"
                        For Each XMLEintragPerson As XmlElement In XMLEintragWerte.ChildNodes
                            Select Case XMLEintragPerson.Name
                                Case "realName"
                                    AdressbuchEintrag.RealName = XMLEintragPerson.InnerText
                                    'Case "imageURL"
                            End Select
                        Next
                    Case "telephony"
                        For Each XMLTelNr As XmlElement In XMLEintragWerte

                            If XMLTelNr.HasAttribute("type") Then
                                Select Case XMLTelNr.GetAttribute("type")
                                    Case TelNrType.home.ToString
                                        AdressbuchEintrag.TelNr_Home_TelNr = XMLTelNr.InnerText
                                        If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Home_ID = CInt(XMLTelNr.GetAttribute("id"))
                                        If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Home_Prio = CBool(XMLTelNr.GetAttribute("prio"))
                                    Case TelNrType.work.ToString
                                        AdressbuchEintrag.TelNr_Work_TelNr = XMLTelNr.InnerText
                                        If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Work_ID = CInt(XMLTelNr.GetAttribute("id"))
                                        If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Work_Prio = CBool(XMLTelNr.GetAttribute("prio"))
                                    Case TelNrType.mobile.ToString
                                        AdressbuchEintrag.TelNr_Mobil_TelNr = XMLTelNr.InnerText
                                        If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Mobil_ID = CInt(XMLTelNr.GetAttribute("id"))
                                        If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Mobil_Prio = CBool(XMLTelNr.GetAttribute("prio"))
                                    Case TelNrType.fax_work.ToString
                                        AdressbuchEintrag.TelNr_Fax_TelNr = XMLTelNr.InnerText
                                        If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Fax_ID = CInt(XMLTelNr.GetAttribute("id"))
                                        If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Fax_Prio = CBool(XMLTelNr.GetAttribute("prio"))
                                End Select
                            End If
                        Next
                    Case "services"

                        For Each XMLEMail As XmlElement In XMLEintragWerte
                            AdressbuchEintrag.EMail = XMLEMail.InnerText

                            If XMLEMail.HasAttribute("id") Then AdressbuchEintrag.E_Mail_ID = CInt(XMLEMail.GetAttribute("id"))

                            If XMLEMail.HasAttribute("classifier") Then AdressbuchEintrag.Classifier = XMLEMail.GetAttribute("classifier")
                        Next
                        'Case "setup"
                        '    For Each XMLEintragPerson As XmlElement In XMLEintragWerte.ChildNodes
                        '        Select Case XMLEintragPerson.Name
                        '            Case "ringTone"
                        '            Case "ringVolume"
                        '        End Select
                        '    Next
                End Select
            Next
            aktEintragsListe.Add(AdressbuchEintrag)
        Next
        Adressbuch.EintragsListe = aktEintragsListe
        Return Adressbuch
    End Function
#End Region

    Private Sub DGVAdressbuch_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)

        ' Von hinten durch die Brust ins Auge!

        Dim ChangedData As String = Me.DGVAdressbuch.Columns(e.ColumnIndex).DataPropertyName
        Dim EntryuID As String = CStr(Me.DGVAdressbuch.Rows(e.RowIndex).Cells("AdrBk_uniqueid").Value)
        Dim ChangedCell As DataGridViewCell = Me.DGVAdressbuch.Rows(e.RowIndex).Cells(e.ColumnIndex)

        Dim newList As List(Of S_AdressbuchEintrag)
        Dim Eintrag As S_AdressbuchEintrag

        newList = CType(BS.DataSource, List(Of S_AdressbuchEintrag))
        If ChangedCell.Value IsNot ChangedCell.EditedFormattedValue Then
            Eintrag = newList.Find(Function(tmp) tmp.Uniqueid = EntryuID)
            newList.RemoveAt(e.RowIndex)
            Select Case ChangedData
                Case "Category" ' VIP
                    Eintrag.Category = CBool(ChangedCell.EditedFormattedValue)
                Case "Classifier"
                    Eintrag.Classifier = CStr(ChangedCell.EditedFormattedValue)
                Case "EMail"
                    Eintrag.EMail = CStr(ChangedCell.EditedFormattedValue)
                Case "RealName"
                    Eintrag.RealName = CStr(ChangedCell.EditedFormattedValue)
                Case "TelNr_Home_Prio"
                    Eintrag.TelNr_Home_Prio = CBool(ChangedCell.EditedFormattedValue)
                Case "TelNr_Home_TelNr"
                    Eintrag.TelNr_Home_TelNr = CStr(ChangedCell.EditedFormattedValue)
                Case "TelNr_Mobil_Prio"
                    Eintrag.TelNr_Mobil_Prio = CBool(ChangedCell.EditedFormattedValue)
                Case "TelNr_Mobil_TelNr"
                    Eintrag.TelNr_Mobil_TelNr = CStr(ChangedCell.EditedFormattedValue)
                Case "TelNr_Work_Prio"
                    Eintrag.TelNr_Work_Prio = CBool(ChangedCell.EditedFormattedValue)
                Case "TelNr_Work_TelNr"
                    Eintrag.TelNr_Work_TelNr = CStr(ChangedCell.EditedFormattedValue)
                Case "TelNr_Fax_Prio"
                    Eintrag.TelNr_Fax_Prio = CBool(ChangedCell.EditedFormattedValue)
                Case "TelNr_Fax_TelNr"
                    Eintrag.TelNr_Fax_TelNr = CStr(ChangedCell.EditedFormattedValue)
            End Select
            newList.Insert(e.RowIndex, Eintrag)

            Me.BS.ResetBindings(False)
        End If

    End Sub


    Private Sub BTest_Click(sender As Object, e As EventArgs) Handles BTest.Click

    End Sub

    Private Sub DGVAdressbuch_DataSourceChanged(sender As Object, e As EventArgs) Handles DGVAdressbuch.DataSourceChanged
        'Stop
    End Sub

    Private Sub Eintrag_Add_Click(sender As Object, e As EventArgs) Handles TSMI_Add.Click
        Dim newList As List(Of S_AdressbuchEintrag)
        Dim Eintrag As New S_AdressbuchEintrag
        newList = CType(BS.DataSource, List(Of S_AdressbuchEintrag))
        Eintrag.ID = newList.Count + 1
        Eintrag.Uniqueid = CStr(CInt(newList.Max(Function(tmp) tmp.Uniqueid)) + 1)

        newList.Add(Eintrag)
        Me.BS.ResetBindings(False)
    End Sub
End Class
