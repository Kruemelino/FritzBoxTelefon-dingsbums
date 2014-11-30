Imports System.IO
Imports System.Xml
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data

Public Class formAdressbuch

    Private C_FB As FritzBox
    Private C_DP As DataProvider
    Private C_KF As Contacts
    Private C_XML As XML
    Private tmp As String
    'Private BS As BindingSource
    Private DS As DataSet

    Private XMLAdressbuch As XmlDocument
    Public Sub New(ByVal XMLKlasse As XML, ByVal FritzBoxKlasse As FritzBox, ByVal DataProviderKlasse As DataProvider, KontaktKlasse As Contacts)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_FB = FritzBoxKlasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse
        C_XML = XMLKlasse

        Me.DGVAdressbuch.RowHeadersVisible = False
        'Me.DGVAdressbuch.Columns.Item("Adrbk_ID").Visible = False
        'Me.DGVAdressbuch.Columns.Item("AdrBk_uniqueid").Visible = False
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

            FillDGVAdressbuch(TransformTelefonbuch(XMLAdressbuch))

        End With
    End Sub

    Private Sub ImportToolStrip_Click(ByVal sender As Object, e As EventArgs) Handles ImportToolStrip.Click
        Dim XMLImportiertesAdressbuch As XmlDocument

        XMLImportiertesAdressbuch = C_FB.DownloadAddressbook("0", "Telefonbuch")
        TransformTelefonbuch(XMLImportiertesAdressbuch)

        FillDGVAdressbuch(TransformTelefonbuch(XMLImportiertesAdressbuch))
    End Sub

    Private Sub FillDGVAdressbuch(ByVal Telefonbuch As XmlDocument)
        Dim xmlStream As MemoryStream = New MemoryStream()
        Dim xmlFile As XmlReader

        Telefonbuch.Save(xmlStream)
        xmlStream.Position = 0

        xmlFile = XmlReader.Create(xmlStream, New XmlReaderSettings())

        DS = New DataSet
        ds.ReadXml(xmlFile)
        With Me.DGVAdressbuch
            .AutoGenerateColumns = False
            .DataSource = DS.Tables.Item(0)
            .ReadOnly = False
            .RowHeadersVisible = False
            .DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            .Enabled = True
            .Update()
        End With
        xmlStream.Close()
        'AddHandler DGVAdressbuch.CellValueChanged, AddressOf DGVAdressbuch_CellValueChanged
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

#Region "Telefonbuch Interaktionen"
    Private Function TransformTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As XmlDocument
        Dim TransTelBook As New XmlDocument
        TransTelBook.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><TrnsAdrBk/>")
        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim XMLTelBuchEintraege As XmlNodeList
        Dim sTMP As String

        With xPathTeile
            .Clear()
            .Add("AdrBk")
        End With
        With NodeNames
            .Clear()
            .Add("id")
            .Add("uniqueid")
            .Add("category")
            .Add("mod_time")
            .Add("RealName")
            .Add("TelNr_Prio")
            .Add("TelNr_home_TelNr")
            .Add("TelNr_work_TelNr")
            .Add("TelNr_mobile_TelNr")
            .Add("TelNr_fax_work_TelNr")
            .Add("EMail")
            .Add("setup")
        End With
        With AttributeNames
            .Clear()
            '.Add("Fax")
            '.Add("Dialport")
        End With
        With NodeValues
            .Clear()
            For Each Wert As String In NodeNames
                .Add(C_DP.P_Def_StringEmpty)
            Next
        End With
        With AttributeValues
            .Clear()
            '.Add(C_DP.P_Def_StringEmpty)
            '.Add(C_DP.P_Def_StringEmpty)
        End With

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")
        Dim i As Integer
        For Each XMLTelefonbuchEintrag As XmlNode In XMLTelBuchEintraege
            i += 1
            NodeValues.Item(NodeNames.IndexOf("id")) = i
            For Each XMLEintragWerte As XmlElement In XMLTelefonbuchEintrag.ChildNodes
                Select Case XMLEintragWerte.Name
                    Case "category"
                        If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then
                            NodeValues.Item(NodeNames.IndexOf(XMLEintragWerte.Name)) = CBool(XMLEintragWerte.InnerText)
                        End If
                    Case "person"
                        NodeValues.Item(NodeNames.IndexOf("RealName")) = XMLEintragWerte.Item("realName").InnerText
                    Case "telephony"
                        For Each XMLTelNr As XmlElement In XMLEintragWerte.ChildNodes
                            sTMP = XMLTelNr.GetAttribute("type")
                            NodeValues.Item(NodeNames.IndexOf("TelNr_" & sTMP & "_TelNr")) = XMLTelNr.InnerText
                            If XMLTelNr.GetAttribute("prio") = "1" Then
                                Select Case sTMP
                                    Case "home"
                                        NodeValues.Item(NodeNames.IndexOf("TelNr_Prio")) = Me.Adrbk_Prio.Items(0).ToString
                                    Case "work"
                                        NodeValues.Item(NodeNames.IndexOf("TelNr_Prio")) = Me.Adrbk_Prio.Items(1).ToString
                                    Case "mobile"
                                        NodeValues.Item(NodeNames.IndexOf("TelNr_Prio")) = Me.Adrbk_Prio.Items(2).ToString
                                    Case "fax_work"
                                        NodeValues.Item(NodeNames.IndexOf("TelNr_Prio")) = Me.Adrbk_Prio.Items(3).ToString
                                End Select
                            End If
                        Next
                    Case "services"
                        For Each XMLEMail As XmlElement In XMLEintragWerte
                            NodeValues.Item(NodeNames.IndexOf("EMail")) = XMLEMail.InnerText
                        Next
                    Case Else
                        NodeValues.Item(NodeNames.IndexOf(XMLEintragWerte.Name)) = XMLEintragWerte.InnerText
                End Select

            Next

            TransTelBook.Item("TrnsAdrBk").AppendChild(C_XML.CreateXMLNode(TransTelBook, "AdrBk", NodeNames, NodeValues, AttributeNames, AttributeValues))
        Next

        Return TransTelBook
    End Function

    Private Function GenerateXML(ByVal Datensatz As DataSet) As XmlDocument
        Dim SW As New StringWriter()
        Dim Telefonbuch As New XmlDocument

        Datensatz.WriteXml(SW)
        Telefonbuch.LoadXml(SW.ToString)

        Return Telefonbuch
    End Function
#End Region

    Private Sub Eintrag_Add_Click(sender As Object, e As EventArgs) Handles TSMI_Add.Click, BAdd.Click
        DS.Tables.Item(0).Rows.Add()
    End Sub

    Private Sub Eintrag_Delete_Click(sender As Object, e As EventArgs) Handles TSMI_Delete.Click, BDel.Click
        With Me.DGVAdressbuch
            If .SelectedRows.Count > 0 Then
                For I As Integer = .SelectedRows.Count - 1 To 0 Step -1
                    .Rows.RemoveAt(.SelectedRows(I).Index)
                Next
            End If
        End With
    End Sub

    Private Sub BTest_Click(sender As Object, e As EventArgs) Handles BTest.Click
        GenerateXML(DS)
    End Sub

    Private Sub DGVAdressbuch_DragDrop(sender As Object, e As DragEventArgs) Handles DGVAdressbuch.DragDrop

    End Sub
End Class
