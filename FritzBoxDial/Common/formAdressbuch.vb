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
        FillDGVAdressbuch(GetEmptyTelbook)
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
            ' Auswahl, je nach Datei
            FillDGVAdressbuch(TransformFritzBoxTelefonbuch(XMLAdressbuch))

        End With
    End Sub

    Private Sub ImportToolStrip_Click(ByVal sender As Object, e As EventArgs) Handles ImportToolStrip.Click
        Dim XMLImportiertesAdressbuch As XmlDocument

        XMLImportiertesAdressbuch = C_FB.DownloadAddressbook("0", "Telefonbuch")
        TransformFritzBoxTelefonbuch(XMLImportiertesAdressbuch)

        FillDGVAdressbuch(TransformFritzBoxTelefonbuch(XMLImportiertesAdressbuch))
    End Sub

    Private Sub FillDGVAdressbuch(ByVal TransformiertesTelefonbuch As XmlDocument)
        Dim xmlStream As MemoryStream = New MemoryStream()
        Dim xmlFile As XmlReader

        TransformiertesTelefonbuch.Save(xmlStream)
        xmlStream.Position = 0

        xmlFile = XmlReader.Create(xmlStream, New XmlReaderSettings())

        DS = New DataSet
        DS.ReadXml(xmlFile)
        With Me.DGVAdressbuch
            .AutoGenerateColumns = False
            .DataSource = DS.Tables.Item(0)
            .ReadOnly = False
            .RowHeadersVisible = False
            .DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            .Enabled = True
            .Update()
        End With

        DS.Tables.Item(0).Columns("uniqueid").Unique = True
        xmlStream.Close()
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
    '		        <number type="home" prio="1" quickdial="1" vanity="STRING" id="0">0123456789</number>
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

    '<TrnsAdrBk>
    '    <AdrBk>
    '	    <id>3</id>
    '	    <uniqueid>27</uniqueid>
    '	    <category>False</category>
    '	    <mod_time>1417437894</mod_time>
    '	    <RealName>Vorname Nachname</RealName>
    '	    <EMail>vorname.nachname@online.de</EMail>
    '	    <TelNr_Prio>Privat</TelNr_Prio>
    '	    <TelNr_home_TelNr>0123456789</TelNr_home_TelNr>
    '	    <TelNr_work_TelNr />
    '	    <TelNr_mobile_TelNr>0123456789</TelNr_mobile_TelNr>
    '	    <TelNr_fax_work_TelNr />
    '	    <TelNr_kwV>Privat</TelNr_kwV>
    '	    <TelNr_Kurzwahl>1</TelNr_Kurzwahl>
    '	    <TelNr_Vanity>BMI</TelNr_Vanity>
    '	    <setup />
    '    </AdrBk>
    '</TrnsAdrBk>
#Region "Telefonbuch Interaktionen"
    Private Function TransformFritzBoxTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As XmlDocument
        Dim TransTelBook As New XmlDocument
        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim XMLTelBuchEintraege As XmlNodeList
        Dim sTMP1 As String
        Dim sTMP2 As String
        Dim i As Integer
        TransTelBook.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><TrnsAdrBk/>")

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
            .Add("EMail")
            .Add("TelNr_Prio")
            .Add("TelNr_kwV")
            .Add("TelNr_Kurzwahl")
            .Add("TelNr_Vanity")
            .Add("TelNr_home_TelNr")
            .Add("TelNr_work_TelNr")
            .Add("TelNr_mobile_TelNr")
            .Add("TelNr_fax_work_TelNr")
            .Add("setup")
        End With
        With AttributeNames
            .Clear()
            '.Add("Fax")
            '.Add("Dialport")
        End With

        'With AttributeValues
        '    .Clear()
        '    '.Add(C_DP.P_Def_StringEmpty)
        '    '.Add(C_DP.P_Def_StringEmpty)
        'End With

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")

        For Each XMLTelefonbuchEintrag As XmlNode In XMLTelBuchEintraege
            With NodeValues
                .Clear()
                For Each Wert As String In NodeNames
                    .Add(C_DP.P_Def_StringEmpty)
                Next
            End With
            i += 1
            NodeValues.Item(NodeNames.IndexOf("id")) = i
            For Each XMLEintragWerte As XmlElement In XMLTelefonbuchEintrag.ChildNodes
                Select Case XMLEintragWerte.Name
                    Case "category"
                        NodeValues.Item(NodeNames.IndexOf(XMLEintragWerte.Name)) = CStr(IIf(XMLEintragWerte.InnerText = "1", True, False))
                    Case "person"
                        NodeValues.Item(NodeNames.IndexOf("RealName")) = XMLEintragWerte.Item("realName").InnerText
                    Case "telephony"
                        For Each XMLTelNr As XmlElement In XMLEintragWerte.ChildNodes
                            sTMP1 = XMLTelNr.GetAttribute("type")
                            NodeValues.Item(NodeNames.IndexOf("TelNr_" & sTMP1 & "_TelNr")) = XMLTelNr.InnerText
                            Select Case sTMP1
                                Case "work"
                                    sTMP2 = Me.Adrbk_Prio.Items(1).ToString
                                Case "mobile"
                                    sTMP2 = Me.Adrbk_Prio.Items(2).ToString
                                Case "fax_work"
                                    sTMP2 = Me.Adrbk_Prio.Items(3).ToString
                                Case Else '"home"
                                    sTMP2 = Me.Adrbk_Prio.Items(0).ToString
                            End Select

                            If XMLTelNr.GetAttribute("prio") = "1" Then
                                NodeValues.Item(NodeNames.IndexOf("TelNr_Prio")) = sTMP2
                            End If
                            If XMLTelNr.GetAttribute("quickdial") IsNot C_DP.P_Def_StringEmpty Or XMLTelNr.GetAttribute("vanity") IsNot C_DP.P_Def_StringEmpty Then
                                NodeValues.Item(NodeNames.IndexOf("TelNr_kwV")) = sTMP2
                                NodeValues.Item(NodeNames.IndexOf("TelNr_Kurzwahl")) = XMLTelNr.GetAttribute("quickdial")
                                NodeValues.Item(NodeNames.IndexOf("TelNr_Vanity")) = XMLTelNr.GetAttribute("vanity")
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

        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
        XMLTelBuchEintraege = Nothing

        Return TransTelBook
    End Function

    Private Function GetEmptyTelbook() As XmlDocument
        Dim TransTelBook As New XmlDocument
        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        TransTelBook.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><TrnsAdrBk/>")

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
            .Add("EMail")
            .Add("TelNr_Prio")
            .Add("TelNr_kwV")
            .Add("TelNr_Kurzwahl")
            .Add("TelNr_Vanity")
            .Add("TelNr_home_TelNr")
            .Add("TelNr_work_TelNr")
            .Add("TelNr_mobile_TelNr")
            .Add("TelNr_fax_work_TelNr")
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
        NodeValues.Item(NodeNames.IndexOf("uniqueid")) = "1"
        TransTelBook.Item("TrnsAdrBk").AppendChild(C_XML.CreateXMLNode(TransTelBook, "AdrBk", NodeNames, NodeValues, AttributeNames, AttributeValues))

        Return TransTelBook
    End Function

    Private Function GetFritzBoxTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As XmlDocument
        Dim TransTelBook As New XmlDocument
        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim XMLTelBuchEintraege As XmlNodeList

        Dim XMLTelBuchEintrag As XmlNode

        Dim XMLTelBuchRealName As XmlNode = Nothing
        Dim XMLTelNrHome As XmlNode = Nothing
        Dim XMLTelNrWork As XmlNode = Nothing
        Dim XMLTelNrFax As XmlNode = Nothing
        Dim XMLTelNrMobil As XmlNode = Nothing
        Dim XMLTelBuchEMAIL As XmlNode = Nothing

        Dim prio As String
        Dim TelNr_kwV As String
        Dim quickdial As String
        Dim vanity As String

        Dim i As Integer

        TransTelBook.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><phonebooks><phonebook/></phonebooks>")

        With xPathTeile
            .Clear()
            .Add("phonebook")
        End With

        With NodeNames
            .Clear()
            .Add("category")
            .Add("person")
            .Add("telephony")
            .Add("services")
            .Add("setup")
            .Add("mod_time")
            .Add("uniqueid")
        End With
        With AttributeNames
            .Clear()
            '.Add("Fax")
            '.Add("Dialport")
        End With

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("AdrBk")

        For Each XMLTelefonbuchEintrag As XmlNode In XMLTelBuchEintraege

            With NodeValues
                .Clear()
                For Each Wert As String In NodeNames
                    .Add(C_DP.P_Def_StringEmpty)
                Next
            End With
            prio = C_DP.P_Def_StringEmpty
            TelNr_kwV = C_DP.P_Def_StringEmpty
            quickdial = C_DP.P_Def_StringEmpty
            vanity = C_DP.P_Def_StringEmpty

            XMLTelNrHome = Nothing
            XMLTelNrWork = Nothing
            XMLTelNrMobil = Nothing
            XMLTelNrFax = Nothing
            XMLTelBuchEMAIL = Nothing
            i += 1

            For Each XMLEintragWerte As XmlElement In XMLTelefonbuchEintrag.ChildNodes
                Select Case XMLEintragWerte.Name
                    Case "category", "uniqueid", "mod_time"
                        NodeValues.Item(NodeNames.IndexOf(XMLEintragWerte.Name)) = XMLEintragWerte.InnerText
                    Case "TelNr_Prio"
                        prio = XMLEintragWerte.InnerText
                    Case "TelNr_kwV"
                        TelNr_kwV = XMLEintragWerte.InnerText
                    Case "TelNr_Kurzwahl"
                        quickdial = XMLEintragWerte.InnerText
                    Case "TelNr_Vanity"
                        vanity = XMLEintragWerte.InnerText

                    Case "RealName"
                        XMLTelBuchRealName = TransTelBook.CreateNode(XmlNodeType.Element, "realName", C_XML.P_Def_StringEmpty)
                        XMLTelBuchRealName.InnerText = XMLEintragWerte.InnerText

                    Case "TelNr_home_TelNr"
                        If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then
                            XMLTelNrHome = TransTelBook.CreateNode(XmlNodeType.Element, "number", C_XML.P_Def_StringEmpty)
                            XMLTelNrHome.InnerText = XMLEintragWerte.InnerText

                            XMLTelNrHome.Attributes.Append(TransTelBook.CreateAttribute("type")).Value = "home"
                            XMLTelNrHome.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "0"
                        End If
                    Case "TelNr_work_TelNr"
                        If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then
                            XMLTelNrWork = TransTelBook.CreateNode(XmlNodeType.Element, "number", C_XML.P_Def_StringEmpty)
                            XMLTelNrWork.InnerText = XMLEintragWerte.InnerText

                            XMLTelNrWork.Attributes.Append(TransTelBook.CreateAttribute("type")).Value = "work"
                            XMLTelNrWork.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "0"
                        End If
                    Case "TelNr_mobile_TelNr"
                        If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then
                            XMLTelNrMobil = TransTelBook.CreateNode(XmlNodeType.Element, "number", C_XML.P_Def_StringEmpty)
                            XMLTelNrMobil.InnerText = XMLEintragWerte.InnerText

                            XMLTelNrMobil.Attributes.Append(TransTelBook.CreateAttribute("type")).Value = "mobile"
                            XMLTelNrMobil.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "0"
                        End If
                    Case "TelNr_fax_work_TelNr"
                        If XMLEintragWerte.InnerText IsNot C_DP.P_Def_StringEmpty Then
                            XMLTelNrFax = TransTelBook.CreateNode(XmlNodeType.Element, "number", C_XML.P_Def_StringEmpty)
                            XMLTelNrFax.InnerText = XMLEintragWerte.InnerText

                            XMLTelNrFax.Attributes.Append(TransTelBook.CreateAttribute("type")).Value = "fax_work"
                            XMLTelNrFax.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "0"
                        End If
                    Case "EMail"
                        XMLTelBuchEMAIL = TransTelBook.CreateNode(XmlNodeType.Element, "email", C_XML.P_Def_StringEmpty)
                        XMLTelBuchEMAIL.InnerText = XMLEintragWerte.InnerText

                        XMLTelBuchEMAIL.Attributes.Append(TransTelBook.CreateAttribute("classifier")).Value = "private"
                        XMLTelBuchEMAIL.Attributes.Append(TransTelBook.CreateAttribute("id")).Value = "0"
                        'Case "setup"
                    Case Else
                        ' Do Nothing
                End Select
            Next
            XMLTelBuchEintrag = C_XML.CreateXMLNode(TransTelBook, "contact", NodeNames, NodeValues, AttributeNames, AttributeValues)

            'Name Hinzufügen
            If XMLTelBuchRealName IsNot Nothing Then XMLTelBuchEintrag.Item("person").AppendChild(XMLTelBuchRealName)

            'Telefonnummer Hinzufügen
            Select Case TelNr_kwV
                Case Me.AdrBk_KwV.Items(0).ToString '"home"
                    XMLTelNrHome.Attributes.Append(TransTelBook.CreateAttribute("quickdial")).Value = quickdial
                    XMLTelNrHome.Attributes.Append(TransTelBook.CreateAttribute("vanity")).Value = vanity
                Case Me.AdrBk_KwV.Items(1).ToString ' "work"
                    XMLTelNrWork.Attributes.Append(TransTelBook.CreateAttribute("quickdial")).Value = quickdial
                    XMLTelNrWork.Attributes.Append(TransTelBook.CreateAttribute("vanity")).Value = vanity
                Case Me.AdrBk_KwV.Items(2).ToString ' "mobile"
                    XMLTelNrMobil.Attributes.Append(TransTelBook.CreateAttribute("quickdial")).Value = quickdial
                    XMLTelNrMobil.Attributes.Append(TransTelBook.CreateAttribute("vanity")).Value = vanity
                Case Me.AdrBk_KwV.Items(3).ToString ' "fax_work"
                    XMLTelNrFax.Attributes.Append(TransTelBook.CreateAttribute("quickdial")).Value = quickdial
                    XMLTelNrFax.Attributes.Append(TransTelBook.CreateAttribute("vanity")).Value = vanity
                Case Else
                    ' Do Nothing
            End Select

            Select Case prio
                Case Me.AdrBk_KwV.Items(0).ToString '"home"
                    XMLTelNrHome.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "1"
                Case Me.AdrBk_KwV.Items(1).ToString ' "work"
                    XMLTelNrWork.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "1"
                Case Me.AdrBk_KwV.Items(2).ToString ' "mobile"
                    XMLTelNrMobil.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "1"
                Case Me.AdrBk_KwV.Items(3).ToString ' "fax_work"
                    XMLTelNrFax.Attributes.Append(TransTelBook.CreateAttribute("prio")).Value = "1"
                Case Else
                    ' Do Nothing
            End Select

            If XMLTelNrHome IsNot Nothing Then XMLTelBuchEintrag.Item("telephony").AppendChild(XMLTelNrHome)
            If XMLTelNrWork IsNot Nothing Then XMLTelBuchEintrag.Item("telephony").AppendChild(XMLTelNrWork)
            If XMLTelNrMobil IsNot Nothing Then XMLTelBuchEintrag.Item("telephony").AppendChild(XMLTelNrMobil)
            If XMLTelNrFax IsNot Nothing Then XMLTelBuchEintrag.Item("telephony").AppendChild(XMLTelNrFax)

            ' E-Mail hinzufügen
            If XMLTelBuchEMAIL IsNot Nothing Then XMLTelBuchEintrag.Item("services").AppendChild(XMLTelBuchEMAIL)

            TransTelBook.Item("phonebooks").Item("phonebook").AppendChild(XMLTelBuchEintrag)
        Next
        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
        XMLTelBuchEintraege = Nothing

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
        Dim uID As String = GetUniqueID()
        DS.Tables.Item(0).Rows.Add.Item("uniqueid") = uID
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

    End Sub

    Private Sub DGVAdressbuch_DragDrop(sender As Object, e As DragEventArgs) Handles DGVAdressbuch.DragDrop

    End Sub

    Private Sub DGVAdressbuch_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles DGVAdressbuch.DataError
        'Throw New NotImplementedException
    End Sub

    Private Sub DGVAdressbuch_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DGVAdressbuch.CellValueChanged
        ' Datum ändern
        ' C.hf.GetUnixTime
    End Sub

    Private Function GetUniqueID() As String
        Dim rmp As Integer = 0

        For Each DR As DataRow In DS.Tables.Item(0).Rows
            If IsNumeric(DR.Item("uniqueid")) AndAlso rmp < CInt(DR.Item("uniqueid")) Then rmp = CInt(DR.Item(1))
        Next
        Return CStr(rmp + 1)
    End Function

    Private Sub SpeichernToolStripButton_Click(sender As Object, e As EventArgs) Handles SpeichernToolStripButton.Click
        GetFritzBoxTelefonbuch(GenerateXML(DS))
    End Sub
End Class
