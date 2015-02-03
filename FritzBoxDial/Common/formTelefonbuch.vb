Imports System.IO
Imports System.Xml
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing

Public Class formTelefonbuch

#Region "Deklarationen"
    Private C_FB As FritzBoxDial.FritzBox
    Private C_DP As FritzBoxDial.DataProvider
    Private C_KF As FritzBoxDial.Contacts
    Private C_XML As FritzBoxDial.XML
    Private C_hf As FritzBoxDial.Helfer
    Private C_GUI As FritzBoxDial.GraphicalUserInterface
    Private C_OLI As FritzBoxDial.OutlookInterface

    Friend DS As DataSet

    Private StatusText As String

    Dim F_TBDTV As formTBDTV
    Dim F_TBControl As formTBControl
    WithEvents F_DnD As MyDndForm
#End Region

#Region "Delegaten"
    Private Delegate Sub DelgTSSLTelefonbuch(ByVal StatusText As String)
    Private Delegate Sub DelgCBoxFBTelbuch(ByVal TelefonbuchListe As String())
    Private Delegate Sub DelgDGVTelefonbuch(ByVal AdrBk As XmlDocument)
#End Region

#Region "BackgroundWorker"
    Private WithEvents BackgroundWorkerLadeTelefonbuchListe As BackgroundWorker
    Private WithEvents BackgroundWorkerImport As BackgroundWorker
    Private WithEvents BackgroundWorkerExport As BackgroundWorker
#End Region

    Public Sub New(ByVal XMLKlasse As XML, _
                   ByVal FritzBoxKlasse As FritzBox, _
                   ByVal DataProviderKlasse As DataProvider, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal Helferklasse As Helfer, _
                   ByVal GUIKlasse As GraphicalUserInterface, _
                   ByVal OLIKlasse As OutlookInterface)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_FB = FritzBoxKlasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse
        C_XML = XMLKlasse
        C_hf = Helferklasse
        C_GUI = GUIKlasse
        C_OLI = OLIKlasse

        F_TBControl = New formTBControl(Me)
        With F_TBControl
            .MdiParent = Me
            .Dock = DockStyle.Right
            .Width = 200
            .Show()
        End With

        F_Dnd = New MyDndForm
        With F_DnD
            .MdiParent = Me
            .Dock = DockStyle.Bottom
            .Height = 100
            .Show()
        End With

        F_TBDTV = New formTBDTV(Me)
        With F_TBDTV
            .MdiParent = Me
            .Dock = DockStyle.Fill
            With .DGVTelefonbuch
                .RowHeadersVisible = False
                With .Columns
                    .Item("Adrbk_ID").Visible = False
                    .Item("AdrBk_uniqueid").Visible = False
                    .Item("AdrBk_Mod_Time").Visible = False
                End With
            End With
            .Show()
        End With

        FillDGVTelefonbuch(GetEmptyTelbook)

        BackgroundWorkerLadeTelefonbuchListe = New BackgroundWorker
        BackgroundWorkerLadeTelefonbuchListe.RunWorkerAsync()

        Me.Show()
        SetStatusText("Formular geöffnet, leeres Telefonbuch geladen")

    End Sub

#Region "Vorlage: Telefonbuchformate"

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
    '               <number type="fax_work" prio="0" id="2">0123456789</number>
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
    '	    <TelNr_Vanity>STRING</TelNr_Vanity>
    '	    <setup />
    '    </AdrBk>
    '</TrnsAdrBk>
#End Region

#Region "Telefonbuch Interaktionen"
    ''' <summary>
    ''' Wandelt das Telefonbuch der Fritz!Box in ein XMLFile um, welches als DataSource für das DataGridView verwendet wird. Dabei werden nur relevante Daten übernommen.
    ''' </summary>
    ''' <param name="XMLTelefonbuch">Das umzuwandelnde Telefonbuch</param>
    ''' <returns>Das umgewandelte Telefonbuch</returns>
    ''' <remarks>Das umgewandelte Telefonbuch dient als interne Datenquelle und wird nie ausgegeben.</remarks>
    Private Function GetDGVTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As XmlDocument
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

        F_TBControl.TBAdrbuchName.Text = XMLTelefonbuch.DocumentElement.Item("phonebook").GetAttribute("name")

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")
        If XMLTelBuchEintraege.Count > 0 Then
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
                                        sTMP2 = F_TBDTV.Adrbk_Prio.Items(1).ToString
                                    Case "mobile"
                                        sTMP2 = F_TBDTV.Adrbk_Prio.Items(2).ToString
                                    Case "fax_work"
                                        sTMP2 = F_TBDTV.Adrbk_Prio.Items(3).ToString
                                    Case Else '"home"
                                        sTMP2 = F_TBDTV.Adrbk_Prio.Items(0).ToString
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
        Else
            ' Das Telefonbuch enthält keine Einträge
 
        End If
        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
        XMLTelBuchEintraege = Nothing

        Return TransTelBook
    End Function

    ''' <summary>
    ''' Erstellt ein Telefonbuch aus der DataSource des DataGridView, welches in die Fritz!Box importiert werden kann.
    ''' </summary>
    ''' <returns>Das fertige XML-Dokument.</returns>
    ''' <remarks>Ausgabegröße</remarks>
    Private Function GetFritzBoxTelefonbuch() As XmlDocument
        Dim FBoxAdrBook As New XmlDocument

        Dim BaseXmlNode As XmlNode
        Dim tmpXmlNode As XmlNode

        Dim TelNrPrio As String
        Dim TelNrkwV As String
        Dim TelNrQuickDial As String
        Dim TelNrVanity As String

        Dim TelNr(3) As String
        Dim TelNrName() As String = {"home", "mobile", "work", "fax_work"}
        Dim i As Integer

        ' Neues Telefonbuch erstellen
        FBoxAdrBook.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><phonebooks><phonebook/></phonebooks>")
        ' Basisknoten festlegen
        BaseXmlNode = FBoxAdrBook.Item("phonebooks").Item("phonebook")

        ' TelefonbuchName festlegen
        If F_TBControl.TBAdrbuchName.Text IsNot C_DP.P_Def_StringEmpty Then
            BaseXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("name")).Value = F_TBControl.TBAdrbuchName.Text
            ' Prüfen:
            'BaseXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("owner")).Value = "1"
        End If

        For Each DR As DataRow In DS.Tables(0).Rows
            'For Each AdressbookEntrie As XmlNode In AdressbookEntries

            ' <contact>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "contact", C_XML.P_Def_StringEmpty)
            BaseXmlNode = BaseXmlNode.AppendChild(tmpXmlNode)

            ' <category>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "category", C_XML.P_Def_StringEmpty)
            tmpXmlNode.InnerText = CStr(IIf(CBool(DR.Item("Category").ToString = "True"), 1, 0))
            BaseXmlNode.AppendChild(tmpXmlNode)

            ' <person>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "person", C_XML.P_Def_StringEmpty)
            BaseXmlNode = BaseXmlNode.AppendChild(tmpXmlNode)

            ' <realName>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "realName", C_XML.P_Def_StringEmpty)
            tmpXmlNode.InnerText = CStr(DR.Item("RealName"))
            BaseXmlNode.AppendChild(tmpXmlNode)

            ' <imageURL> nicht Implementiert
            ' tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "imageURL", C_XML.P_Def_StringEmpty)
            ' tmpXmlNode.InnerText = AdressbookEntrie.Item("imageURL").InnerText
            ' BaseXmlNode.AppendChild(tmpXmlNode)

            ' Eine Ebene zurück auf <contact>
            BaseXmlNode = BaseXmlNode.ParentNode

            ' <telephony>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "telephony", C_XML.P_Def_StringEmpty)
            BaseXmlNode = BaseXmlNode.AppendChild(tmpXmlNode)

            ' Abhier etwas komplizierter
            ' Telefonnummern ermitteln

            TelNrPrio = CStr(DR.Item("TelNr_Prio"))
            TelNrkwV = CStr(DR.Item("TelNr_kwV"))
            TelNrQuickDial = CStr(DR.Item("TelNr_Kurzwahl"))
            TelNrVanity = CStr(DR.Item("TelNr_Vanity"))

            TelNr(0) = C_hf.nurZiffern(CStr(DR.Item("TelNr_home_TelNr")))     ' Home
            TelNr(1) = C_hf.nurZiffern(CStr(DR.Item("TelNr_mobile_TelNr")))  ' Mobil
            TelNr(2) = C_hf.nurZiffern(CStr(DR.Item("TelNr_work_TelNr")))  ' Work
            TelNr(3) = C_hf.nurZiffern(CStr(DR.Item("TelNr_fax_work_TelNr"))) ' Fax
            ' Counter auf 0
            i = 0
            ' <number>
            For j = 0 To 3
                If TelNr(j) IsNot C_DP.P_Def_StringEmpty Then
                    i += 1
                    tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "number", C_XML.P_Def_StringEmpty)
                    tmpXmlNode.InnerText = TelNr(j)

                    ' Attribut id
                    tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("id")).Value = CStr(i - 1)

                    ' Attribut prio
                    tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("prio")).Value = CStr(IIf(TelNrPrio = F_TBDTV.Adrbk_Prio.Items(j).ToString, 1, 0))

                    ' Attribut type
                    tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("type")).Value = TelNrName(j)

                    If TelNrkwV = F_TBDTV.AdrBk_KwV.Items(j).ToString Then
                        ' Attribut quickdial
                        If TelNrQuickDial IsNot C_DP.P_Def_StringEmpty Then
                            tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("quickdial")).Value = TelNrQuickDial
                        End If
                        ' Attribut vanity
                        If TelNrVanity IsNot C_DP.P_Def_StringEmpty Then
                            tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("vanity")).Value = TelNrVanity
                        End If
                    End If
                    BaseXmlNode.AppendChild(tmpXmlNode)
                End If
            Next
            ' Attribut nid
            BaseXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("nid")).Value = CStr(i)

            ' Eine Ebene zurück auf <contact>
            BaseXmlNode = BaseXmlNode.ParentNode

            ' Counter auf 0
            i = 0

            ' <services>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "services", C_XML.P_Def_StringEmpty)
            BaseXmlNode = BaseXmlNode.AppendChild(tmpXmlNode)

            ' <email>
            If DR.Item("EMail") IsNot C_DP.P_Def_StringEmpty Then
                i += 1
                tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "email", C_XML.P_Def_StringEmpty)
                tmpXmlNode.InnerText = CStr(DR.Item("EMail"))
                tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("classifier")).Value = "private"
                tmpXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("id")).Value = CStr(i - 1)
                BaseXmlNode.AppendChild(tmpXmlNode)
                ' Attribut nid
                BaseXmlNode.Attributes.Append(FBoxAdrBook.CreateAttribute("nid")).Value = CStr(i)
            End If
            ' Eine Ebene zurück auf <contact>
            BaseXmlNode = BaseXmlNode.ParentNode

            ' <setup>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "setup", C_XML.P_Def_StringEmpty)
            BaseXmlNode.AppendChild(tmpXmlNode)

            '<mod_time>
            tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "mod_time", C_XML.P_Def_StringEmpty)
            tmpXmlNode.InnerText = CStr(DR.Item("mod_time"))
            BaseXmlNode.AppendChild(tmpXmlNode)

            ''<uniqueid> ' Wird von FB überschrieben 
            'tmpXmlNode = FBoxAdrBook.CreateNode(XmlNodeType.Element, "uniqueid", C_XML.P_Def_StringEmpty)
            'tmpXmlNode.InnerText = CStr(DR.Item("uniqueid"))
            'BaseXmlNode.AppendChild(tmpXmlNode)

            ' Eine Ebene zurück auf <phonebook>
            BaseXmlNode = BaseXmlNode.ParentNode

            ' Done
        Next
        Return FBoxAdrBook
    End Function

    ''' <summary>
    ''' Erstellt ein leeres Telefonbuch, welches als DataSource für das DataGridView verwendet wird.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
            .Add(C_DP.P_Def_StringEmpty) ' id
            .Add(C_DP.P_Def_StringEmpty) ' uniqueid
            .Add("False")                ' category
            .Add(C_DP.P_Def_StringEmpty) ' mod_time
            .Add(C_DP.P_Def_StringEmpty) ' RealName
            .Add(C_DP.P_Def_StringEmpty) ' EMail
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_Prio
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_kwV
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_Kurzwahl
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_Vanity
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_home_TelNr
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_work_TelNr
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_mobile_TelNr
            .Add(C_DP.P_Def_StringEmpty) ' TelNr_fax_work_TelNr
            .Add(C_DP.P_Def_StringEmpty) ' setup
            'For Each Wert As String In NodeNames
            '    .Add(C_DP.P_Def_StringEmpty)
            'Next
        End With
        'NodeValues.Item(NodeNames.IndexOf("uniqueid")) = "1"
        TransTelBook.Item("TrnsAdrBk").AppendChild(C_XML.CreateXMLNode(TransTelBook, "AdrBk", NodeNames, NodeValues, AttributeNames, AttributeValues))

        Return TransTelBook
    End Function

    ''' <summary>
    ''' Übergibt das umgewandelte Telefonbuch an das DatagridView
    ''' </summary>
    ''' <param name="TransformiertesTelefonbuch"></param>
    ''' <remarks></remarks>
    Private Sub FillDGVTelefonbuch(ByVal TransformiertesTelefonbuch As XmlDocument)
        Dim xmlStream As MemoryStream = New MemoryStream()
        Dim xmlFile As XmlReader

        If TransformiertesTelefonbuch.GetElementsByTagName("AdrBk").Count > 0 Then
            TransformiertesTelefonbuch.Save(xmlStream)
        Else
            GetEmptyTelbook.Save(xmlStream)
        End If

        xmlStream.Position = 0

        xmlFile = XmlReader.Create(xmlStream, New XmlReaderSettings())

        DS = New DataSet
        DS.ReadXml(xmlFile)
        With F_TBDTV.DGVTelefonbuch
            .AutoGenerateColumns = False
            .DataSource = DS.Tables.Item(0)
            .ReadOnly = False
            .RowHeadersVisible = False
            .DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            .Enabled = True
            .Update()
        End With

        'DS.Tables.Item(0).Columns("uniqueid").Unique = True
        xmlStream.Close()

        AddHandler F_TBDTV.DGVTelefonbuch.CellValueChanged, AddressOf DGVTelefonbuch_CellValueChanged
        AddHandler F_TBDTV.DGVTelefonbuch.DataError, AddressOf DGVTelefonbuch_DataError
    End Sub

    Private Sub ADDTelefonbuchEintrag(ByVal DragDropString() As String)
        'Name;Position;Firma;Speichern unter;Land/Region;Abteilung;Telefon geschäftlich;Fax geschäftl.;Telefon privat;Mobiltelefon;E-Mail;Kategorien;

        '.Add("id")
        '.Add("uniqueid")
        '.Add("category")
        '.Add("mod_time")
        '.Add("RealName")
        '.Add("EMail")
        '.Add("TelNr_Prio")
        '.Add("TelNr_kwV")
        '.Add("TelNr_Kurzwahl")
        '.Add("TelNr_Vanity")
        '.Add("TelNr_home_TelNr")
        '.Add("TelNr_work_TelNr")
        '.Add("TelNr_mobile_TelNr")
        '.Add("TelNr_fax_work_TelNr")

        Dim BaseArray() As String
        Dim DatenArray() As String
        BaseArray = Split(DragDropString(0), ";", , CompareMethod.Text)
        Dim neueZeile As DataRow
        For i = 1 To DragDropString.Count - 1
            DatenArray = Split(DragDropString(i), ";", , CompareMethod.Text)
            neueZeile = DS.Tables.Item(0).NewRow()
            neueZeile.Item("RealName") = DatenArray(Array.IndexOf(BaseArray, "Name"))
            neueZeile.Item("EMail") = DatenArray(Array.IndexOf(BaseArray, "E-Mail"))

            neueZeile.Item("TelNr_Prio") = F_TBDTV.Adrbk_Prio.Items(0).ToString

            neueZeile.Item("TelNr_home_TelNr") = C_hf.nurZiffern(DatenArray(Array.IndexOf(BaseArray, "Telefon privat")))
            neueZeile.Item("TelNr_work_TelNr") = C_hf.nurZiffern(DatenArray(Array.IndexOf(BaseArray, "Telefon geschäftlich")))
            neueZeile.Item("TelNr_mobile_TelNr") = C_hf.nurZiffern(DatenArray(Array.IndexOf(BaseArray, "Mobiltelefon")))
            neueZeile.Item("TelNr_fax_work_TelNr") = C_hf.nurZiffern(DatenArray(Array.IndexOf(BaseArray, "Fax geschäftl.")))
            DS.Tables.Item(0).Rows.Add(neueZeile)
        Next

    End Sub

    Friend Sub DGVTelefonbuch_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)

        SetStatusText("Eintrag " & CStr(DS.Tables.Item(0).Rows(e.RowIndex).Item("RealName")) & " geändert.")
        ' Datum ändern
        Dim UTime As Integer = C_hf.GetUnixTime
        DS.Tables.Item(0).Rows(e.RowIndex).Item("mod_time") = UTime

    End Sub

#End Region

#Region "Button_Click"
    Private Sub ÖffnenToolStripButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ÖffnenToolStripButton.Click
        Dim myStream As Stream = Nothing
        Dim myStreamReader As StreamReader
        Dim XMLTelefonbuch As New XmlDocument()
        With OFDAdressdbuch
            .Filter = "XML Telefonbuch (*.xml)|*.xml|Alle Dateien (*.*)|*.*"
            .FilterIndex = 1
            .RestoreDirectory = True

            If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                Try
                    myStream = .OpenFile()
                    If (myStream IsNot Nothing) Then
                        myStreamReader = New StreamReader(myStream)
                        XMLTelefonbuch.LoadXml(myStreamReader.ReadToEnd)
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
            FillDGVTelefonbuch(GetDGVTelefonbuch(XMLTelefonbuch))
            myStream = Nothing
            myStreamReader = Nothing
            XMLTelefonbuch = Nothing
        End With
    End Sub

    Private Overloads Sub Eintrag_Add_Click(sender As Object, e As EventArgs) Handles TSMI_Add.Click
        Eintrag_Add_Click()
    End Sub
    Friend Overloads Sub Eintrag_Add_Click()
        DS.Tables.Item(0).Rows.Add()
    End Sub

    Private Overloads Sub Eintrag_Delete_Click(sender As Object, e As EventArgs) Handles TSMI_Delete.Click
        Eintrag_Delete_Click()
    End Sub

    Friend Overloads Sub Eintrag_Delete_Click()
        With F_TBDTV.DGVTelefonbuch
            If .SelectedRows.Count > 0 Then
                For I As Integer = .SelectedRows.Count - 1 To 0 Step -1
                    .Rows.RemoveAt(.SelectedRows(I).Index)
                Next
                SetStatusText("Eintrag / Einträge gelöscht.")
            End If
        End With
    End Sub

    Private Sub SpeicheFBTelefonbuch(sender As Object, e As EventArgs) Handles SpeichernToolStripButton.Click

        Dim myStreamWriter As StreamWriter
        Dim myStringBuilder As New StringBuilder
        Dim myStringWriter As New StringWriter(myStringBuilder)
        Dim myXmlTextWriter As New XmlTextWriter(myStringWriter)

        With myXmlTextWriter
            .Formatting = Formatting.Indented
            .IndentChar = ControlChars.Tab
            .Indentation = 1
        End With
        GetFritzBoxTelefonbuch.WriteContentTo(myXmlTextWriter)

        With SFDTelefonbuch
            .Filter = "XML Telefonbuch (*.xml)|*.xml" '"XML Telefonbuch (*.xml)|*.xml|Alle Dateien (*.*)|*.*"
            .FilterIndex = 1
            .RestoreDirectory = True

            If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                Try
                    myStreamWriter = File.CreateText(.FileName)
                    With myStreamWriter
                        .Write(myStringBuilder)
                        .Flush()
                        .Close()
                    End With
                Catch Ex As Exception
                    C_hf.FBDB_MsgBox("Das Speichern ist nicht möglich:" & C_DP.P_Def_NeueZeile & C_DP.P_Def_NeueZeile & Ex.Message, MsgBoxStyle.Critical, "SpeicheFBTelefonbuch")
                Finally
                    ' Check this again, since we need to make sure we didn't throw an exception on open.
                    myStreamWriter = Nothing
                    myStringBuilder = Nothing
                    myStringWriter = Nothing
                    myXmlTextWriter = Nothing
                End Try
            End If
        End With

    End Sub

#Region "Import Export"

    Private Sub ImportToolStrip_Click(ByVal sender As Object, e As EventArgs) Handles ImportToolStrip.Click
        Dim TelBuch As String

        BackgroundWorkerImport = New BackgroundWorker
        If F_TBControl.CBoxFBTelbuch.SelectedItem IsNot Nothing AndAlso CStr(F_TBControl.CBoxFBTelbuch.SelectedItem).Contains(": ") Then
            TelBuch = CStr(F_TBControl.CBoxFBTelbuch.SelectedItem)
        Else
            TelBuch = "0: Telefonbuch"
        End If

        With BackgroundWorkerImport
            .WorkerReportsProgress = False
            .WorkerSupportsCancellation = False
            .RunWorkerAsync(TelBuch)
        End With
    End Sub

    Private Sub ExportToolStripButton_Click(sender As Object, e As EventArgs) Handles ExportToolStripButton.Click

        Dim TelBuch As String = "0: Telefonbuch"
        Dim tmpStatusText As String

        If CStr(F_TBControl.CBoxFBTelbuch.SelectedItem).Contains(": ") Then TelBuch = CStr(F_TBControl.CBoxFBTelbuch.SelectedItem)

        If C_hf.FBDB_MsgBox("Soll dieses Telefonbuch in die Fritz!Box exportiert werden? Falls dieses Telefonbuch bereits vorhanden ist, wird es überschrieben.", _
                            MsgBoxStyle.YesNo, "ExportToolStripButton_Click") = MsgBoxResult.Yes Then
            BackgroundWorkerExport = New BackgroundWorker
            With BackgroundWorkerExport
                .WorkerReportsProgress = False
                .WorkerSupportsCancellation = False
                .RunWorkerAsync(TelBuch)
            End With

            tmpStatusText = "Telefonbuch wird zur Fritz!Box exportiert... (bitte warten)"
        Else
            tmpStatusText = "Telefonbuch nicht zur Fritz!Box exportiert."
        End If
        SetStatusText(tmpStatusText)
    End Sub

#End Region
#End Region

#Region "Behandlung Delegaten"
    Private Sub SetStatusText(ByVal StatusText As String)
        If Me.InvokeRequired Then
            Dim D As New DelgTSSLTelefonbuch(AddressOf SetStatusText)
            Me.Invoke(D, StatusText)
        Else
            Me.TSSLTelefonbuch.Text = StatusText
        End If
    End Sub

    Private Sub SetDBVTelefonbuch(ByVal AdrBk As XmlDocument)
        If Me.InvokeRequired Then
            Dim D As New DelgDGVTelefonbuch(AddressOf SetDBVTelefonbuch)
            Me.Invoke(D, AdrBk)
        Else
            FillDGVTelefonbuch(GetDGVTelefonbuch(AdrBk))
        End If
    End Sub

    Private Sub SetDelgCBoxFBTelbuch(ByVal FBTelBkList As String())
        If Me.InvokeRequired Then
            Dim D As New DelgCBoxFBTelbuch(AddressOf SetDelgCBoxFBTelbuch)
            Me.Invoke(D, FBTelBkList)
        Else
            With F_TBControl.CBoxFBTelbuch
                .Items.AddRange(FBTelBkList)
                .SelectedIndex = 0
            End With
        End If
    End Sub

#End Region

#Region "Behandlung BackgroundWorker"

    Private Sub BackgroundWorkerImport_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorkerImport.DoWork
        Dim BookID As String = "0"
        Dim BookName As String = "Telefonbuch"
        Dim TelBuch As String() = Split(CStr(e.Argument), ": ", , CompareMethod.Text)

        If IsNumeric(TelBuch(0)) Then BookID = TelBuch(0)
        BookName = TelBuch(1)
        BookName = Replace(BookName, vbLf, "", , , CompareMethod.Text)
        SetStatusText("Importvorgang des Telefonbuchs " & BookID & " (" & BookName & ") von der Fritz!Box gestartet... (bitte warten)")
        e.Result = C_FB.DownloadAddressbook(BookID, BookName)
    End Sub

    Private Sub BackgroundWorkerImport_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorkerImport.RunWorkerCompleted
        SetDBVTelefonbuch(CType(e.Result, XmlDocument))
        SetStatusText("Telefonbuch von der Fritz!Box erfolgreich importiert.")
    End Sub

    Private Sub BackgroundWorkerExport_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorkerExport.DoWork
        Dim myStringBuilder As New StringBuilder
        Dim myStringWriter As New StringWriter(myStringBuilder)
        Dim myXmlTextWriter As New XmlTextWriter(myStringWriter)
        Dim BookID As String = "0"
        Dim sXML As String

        With myXmlTextWriter
            .Formatting = Formatting.Indented
            .IndentChar = ControlChars.Tab
            .Indentation = 1
        End With

        GetFritzBoxTelefonbuch.WriteContentTo(myXmlTextWriter)
        sXML = myStringBuilder.ToString()

        Dim TelBuch As String() = Split(CStr(e.Argument), ": ", , CompareMethod.Text)

        If IsNumeric(TelBuch(0)) Then BookID = TelBuch(0)

        SetStatusText("Exportvorgang des Telefonbuchs " & BookID & " zu der Fritz!Box gestartet... (bitte warten)")

        C_FB.UploadAddressbook(BookID, sXML)
    End Sub

    Private Sub BackgroundWorkerExport_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorkerExport.RunWorkerCompleted
        SetStatusText("Telefonbuch in die Fritz!Box exportiert. Bitte Prüfen Sie, ob der Vorgang erfolgreich war.")
    End Sub

    Private Sub BackgroundWorkerLadeTelefonbuchListe_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorkerLadeTelefonbuchListe.DoWork
        SetStatusText("Ermittlung vorhandener Telefonbucher von der Fritz!Box gestartet... (bitte warten)")
        e.Result = C_FB.GetTelefonbuchListe()
    End Sub

    Private Sub BackgroundWorkerLadeTelefonbuchListe_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorkerLadeTelefonbuchListe.RunWorkerCompleted
        SetDelgCBoxFBTelbuch(CType(e.Result, String()))
        SetStatusText("Ermittlung vorhandener Telefonbücher Fritz!Box erfolgreich abgeschlossen.")
    End Sub

#End Region

    Private Sub DGVTelefonbuch_DataError(sender As Object, e As DataGridViewDataErrorEventArgs)
        Throw New NotImplementedException
    End Sub

    Private Sub BTest_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim NewMDIChild As New MyDndForm()
        'Set the Parent Form of the Child window.
        NewMDIChild.MdiParent = Me
        'Display the new form.
        NewMDIChild.Show()

    End Sub

#Region "DnD"

#End Region

End Class

