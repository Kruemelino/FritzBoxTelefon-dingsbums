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

            'ReadXMLTelefonbuch(XMLAdressbuch)

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
        Dim ds As New DataSet
        Telefonbuch.Save(xmlStream)
        xmlStream.Position = 0

        xmlFile = XmlReader.Create(xmlStream, New XmlReaderSettings())

        ds.ReadXml(xmlFile)

        Me.DGVAdressbuch.AutoGenerateColumns = False

        Me.DGVAdressbuch.DataSource = ds.Tables(0)

        Me.DGVAdressbuch.ReadOnly = False
        Me.DGVAdressbuch.RowHeadersVisible = False
        Me.DGVAdressbuch.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
        Me.DGVAdressbuch.Enabled = True

        Me.DGVAdressbuch.Update()
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
        TransTelBook.LoadXml("<TrnsAdrBk/>")
        '<?xml version=""1.0"" encoding=""UTF-8""?>
        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim XMLTelBuchEintraege As XmlNodeList
        Dim sTMP As string

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
            .Add("TelNr_home_TelNr")
            .Add("TelNr_home_Prio")
            .Add("TelNr_work_TelNr")
            .Add("TelNr_work_Prio")
            .Add("TelNr_mobile_TelNr")
            .Add("TelNr_mobile_Prio")
            .Add("TelNr_fax_work_TelNr")
            .Add("TelNr_fax_work_Prio")
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
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
        End With
        With AttributeValues
            .Clear()
            '.Add(C_DP.P_Def_StringEmpty)
            '.Add(C_DP.P_Def_StringEmpty)
        End With

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")
        Dim i As Integer
        For Each XMLTelefonbuchEintrag As XmlNode In XMLTelBuchEintraege
            NodeValues.Item(NodeNames.IndexOf("TelNr_home_Prio")) = False
            NodeValues.Item(NodeNames.IndexOf("TelNr_mobile_Prio")) = False
            NodeValues.Item(NodeNames.IndexOf("TelNr_work_Prio")) = False
            NodeValues.Item(NodeNames.IndexOf("TelNr_fax_work_Prio")) = False
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
                            sTMP = "TelNr_" & XMLTelNr.GetAttribute("type")
                            NodeValues.Item(NodeNames.IndexOf(sTMP & "_TelNr")) = XMLTelNr.InnerText
                            NodeValues.Item(NodeNames.IndexOf(sTMP & "_Prio")) = (XMLTelNr.InnerText IsNot C_DP.P_Def_StringEmpty) And CBool(XMLTelNr.GetAttribute("prio"))
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

    'Private Function ReadXMLTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As S_Adressbuch
    '    Dim Adressbuch As New S_Adressbuch
    '    Dim XMLTelBuchEintraege As XmlNodeList
    '    Dim AdressbuchEintrag As S_AdressbuchEintrag
    '    Dim aktEintragsListe As New List(Of S_AdressbuchEintrag)


    '    Dim i As Integer = 0

    '    XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")

    '    For Each XMLTelefonbuchEintrag As XmlNode In XMLTelBuchEintraege
    '        AdressbuchEintrag = New S_AdressbuchEintrag
    '        i += 1
    '        AdressbuchEintrag.ID = i

    '        For Each XMLEintragWerte As XmlElement In XMLTelefonbuchEintrag.ChildNodes
    '            Select Case XMLEintragWerte.Name
    '                Case "category"
    '                    If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then AdressbuchEintrag.Category = CBool(XMLEintragWerte.InnerText)
    '                Case "mod_time"
    '                    AdressbuchEintrag.Mod_Time = XMLEintragWerte.InnerText
    '                Case "uniqueid"
    '                    AdressbuchEintrag.Uniqueid = XMLEintragWerte.InnerText
    '                Case "person"
    '                    For Each XMLEintragPerson As XmlElement In XMLEintragWerte.ChildNodes
    '                        Select Case XMLEintragPerson.Name
    '                            Case "realName"
    '                                AdressbuchEintrag.RealName = XMLEintragPerson.InnerText
    '                                'Case "imageURL"
    '                        End Select
    '                    Next
    '                Case "telephony"
    '                    For Each XMLTelNr As XmlElement In XMLEintragWerte

    '                        If XMLTelNr.HasAttribute("type") Then
    '                            Select Case XMLTelNr.GetAttribute("type")
    '                                Case TelNrType.home.ToString
    '                                    AdressbuchEintrag.TelNr_Home_TelNr = XMLTelNr.InnerText
    '                                    If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Home_ID = CInt(XMLTelNr.GetAttribute("id"))
    '                                    If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Home_Prio = CBool(XMLTelNr.GetAttribute("prio"))
    '                                Case TelNrType.work.ToString
    '                                    AdressbuchEintrag.TelNr_Work_TelNr = XMLTelNr.InnerText
    '                                    If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Work_ID = CInt(XMLTelNr.GetAttribute("id"))
    '                                    If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Work_Prio = CBool(XMLTelNr.GetAttribute("prio"))
    '                                Case TelNrType.mobile.ToString
    '                                    AdressbuchEintrag.TelNr_Mobil_TelNr = XMLTelNr.InnerText
    '                                    If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Mobil_ID = CInt(XMLTelNr.GetAttribute("id"))
    '                                    If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Mobil_Prio = CBool(XMLTelNr.GetAttribute("prio"))
    '                                Case TelNrType.fax_work.ToString
    '                                    AdressbuchEintrag.TelNr_Fax_TelNr = XMLTelNr.InnerText
    '                                    If XMLTelNr.HasAttribute("id") Then AdressbuchEintrag.TelNr_Fax_ID = CInt(XMLTelNr.GetAttribute("id"))
    '                                    If XMLTelNr.HasAttribute("prio") Then AdressbuchEintrag.TelNr_Fax_Prio = CBool(XMLTelNr.GetAttribute("prio"))
    '                            End Select
    '                        End If
    '                    Next
    '                Case "services"

    '                    For Each XMLEMail As XmlElement In XMLEintragWerte
    '                        AdressbuchEintrag.EMail = XMLEMail.InnerText

    '                        If XMLEMail.HasAttribute("id") Then AdressbuchEintrag.E_Mail_ID = CInt(XMLEMail.GetAttribute("id"))

    '                        If XMLEMail.HasAttribute("classifier") Then AdressbuchEintrag.Classifier = XMLEMail.GetAttribute("classifier")
    '                    Next
    '                    'Case "setup"
    '                    '    For Each XMLEintragPerson As XmlElement In XMLEintragWerte.ChildNodes
    '                    '        Select Case XMLEintragPerson.Name
    '                    '            Case "ringTone"
    '                    '            Case "ringVolume"
    '                    '        End Select
    '                    '    Next
    '            End Select
    '        Next
    '        aktEintragsListe.Add(AdressbuchEintrag)
    '    Next
    '    Adressbuch.EintragsListe = aktEintragsListe
    '    Return Adressbuch
    'End Function
#End Region

    'Private Sub DGVAdressbuch_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)

    '    ' Von hinten durch die Brust ins Auge!

    '    Dim ChangedData As String = Me.DGVAdressbuch_orig.Columns(e.ColumnIndex).DataPropertyName
    '    Dim EntryuID As String = CStr(Me.DGVAdressbuch_orig.Rows(e.RowIndex).Cells("AdrBk_uniqueid").Value)
    '    Dim ChangedCell As DataGridViewCell = Me.DGVAdressbuch_orig.Rows(e.RowIndex).Cells(e.ColumnIndex)

    '    Dim newList As List(Of S_AdressbuchEintrag)
    '    Dim Eintrag As S_AdressbuchEintrag

    '    newList = CType(BS.DataSource, List(Of S_AdressbuchEintrag))
    '    If ChangedCell.Value IsNot ChangedCell.EditedFormattedValue Then
    '        Eintrag = newList.Find(Function(tmp) tmp.Uniqueid = EntryuID)
    '        newList.RemoveAt(e.RowIndex)
    '        Select Case ChangedData
    '            Case "Category" ' VIP
    '                Eintrag.Category = CBool(ChangedCell.EditedFormattedValue)
    '            Case "Classifier"
    '                Eintrag.Classifier = CStr(ChangedCell.EditedFormattedValue)
    '            Case "EMail"
    '                Eintrag.EMail = CStr(ChangedCell.EditedFormattedValue)
    '            Case "RealName"
    '                Eintrag.RealName = CStr(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Home_Prio"
    '                Eintrag.TelNr_Home_Prio = CBool(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Home_TelNr"
    '                Eintrag.TelNr_Home_TelNr = CStr(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Mobil_Prio"
    '                Eintrag.TelNr_Mobil_Prio = CBool(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Mobil_TelNr"
    '                Eintrag.TelNr_Mobil_TelNr = CStr(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Work_Prio"
    '                Eintrag.TelNr_Work_Prio = CBool(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Work_TelNr"
    '                Eintrag.TelNr_Work_TelNr = CStr(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Fax_Prio"
    '                Eintrag.TelNr_Fax_Prio = CBool(ChangedCell.EditedFormattedValue)
    '            Case "TelNr_Fax_TelNr"
    '                Eintrag.TelNr_Fax_TelNr = CStr(ChangedCell.EditedFormattedValue)
    '        End Select
    '        newList.Insert(e.RowIndex, Eintrag)

    '        Me.BS.ResetBindings(False)
    '    End If

    'End Sub

    'Private Sub Eintrag_Add_Click(sender As Object, e As EventArgs) Handles TSMI_Add.Click
    '    Dim newList As List(Of S_AdressbuchEintrag)
    '    Dim Eintrag As New S_AdressbuchEintrag
    '    newList = CType(BS.DataSource, List(Of S_AdressbuchEintrag))
    '    Eintrag.ID = newList.Count + 1
    '    Eintrag.Uniqueid = CStr(CInt(newList.Max(Function(tmp) tmp.Uniqueid)) + 1)

    '    newList.Add(Eintrag)
    '    Me.BS.ResetBindings(False)
    'End Sub

    'Private Sub Eintrag_Delete_Click(sender As Object, e As EventArgs) Handles TSMI_Delete.Click

    '    Dim newList As List(Of S_AdressbuchEintrag)
    '    newList = CType(BS.DataSource, List(Of S_AdressbuchEintrag))

    '    For Each Eintrag As DataGridViewRow In Me.DGVAdressbuch_orig.SelectedRows
    '        newList.Remove(newList.Find(Function(tmp) tmp.Uniqueid Is Eintrag.Cells("AdrBk_uniqueid").Value))
    '    Next
    '    Me.BS.ResetBindings(False)
    'End Sub
End Class
