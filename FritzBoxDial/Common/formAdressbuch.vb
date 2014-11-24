Imports System.IO
Imports System.Xml
Imports System.Windows.Forms
Imports System.Collections.Generic

Public Class formAdressbuch
    Private C_FB As FritzBox
    Private C_DP As DataProvider
    Private C_KF As Contacts
    Private C_XML As XML
    Private tmp As String

    Private XMLAdressbuch As XmlDocument
    Public Sub New(ByVal XMLKlasse As XML, ByVal FritzBoxKlasse As FritzBox, ByVal DataProviderKlasse As DataProvider, KontaktKlasse As Contacts)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_FB = FritzBoxKlasse
        C_DP = DataProviderKlasse
        C_KF = KontaktKlasse
        C_XML = XMLKlasse

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

    'Sub FillDGV(ByVal XMLDatenSatz As XmlDocument, ByVal Eintrag As String)
    '    Dim myStream As New MemoryStream()
    '    XMLDatenSatz.Save(myStream)
    '    myStream.Position = 0

    '    Me.DSAdressbuch.ReadXml(myStream, Data.XmlReadMode.Auto)

    '    If Me.DSAdressbuch.HasChanges Then
    '        With Me.DGVAdressbuch
    '            .AutoGenerateColumns = True
    '            column=New datagridviewcolumn
    '            .DataSource = Me.DSAdressbuch.Tables("contact")

    '        End With
    '    End If
    'End Sub

    Private Sub ImportToolStrip_Click(sender As Object, e As EventArgs) Handles ImportToolStrip.Click
        'FillDGV(C_FB.DownloadAddressbook("0", "Telefonbuch"), "person")
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
#Region "Daten"



    Friend Enum TelNrType
        home = 0
        mobile = 1
        work = 2
        fax_work = 3
    End Enum

    Friend Structure C_Adressbuch
        Friend EintragsListe As List(Of C_AdressbuchEintrag)
    End Structure

    Friend Structure C_Telefonnummer
        Private _ID As Integer
        Friend Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Private _prio As Integer
        Friend Property Prio() As Integer
            Get
                Return _prio
            End Get
            Set(ByVal value As Integer)
                _prio = value
            End Set
        End Property

        Private _type As TelNrType
        Friend Property Type() As TelNrType
            Get
                Return _type
            End Get
            Set(ByVal value As TelNrType)
                _type = value
            End Set
        End Property

        Private _TelNr As String
        Friend Property TelNr() As String
            Get
                Return _TelNr
            End Get
            Set(ByVal value As String)
                _TelNr = value
            End Set
        End Property
    End Structure

    Friend Structure C_EMail
        Private _ID As Integer
        Friend Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Private _classifier As String
        Friend Property Classifier() As String
            Get
                Return _classifier
            End Get
            Set(ByVal value As String)
                _classifier = value
            End Set
        End Property

        Private _EMail As String
        Friend Property EMail() As String
            Get
                Return _EMail
            End Get
            Set(ByVal value As String)
                _EMail = value
            End Set
        End Property
    End Structure

    Friend Structure C_AdressbuchEintrag

        Private _category As Integer
        Friend Property Category() As Integer
            Get
                Return _category
            End Get
            Set(ByVal value As Integer)
                _category = value
            End Set
        End Property

        Private _RealName As String
        Friend Property RealName() As String
            Get
                Return _RealName
            End Get
            Set(ByVal value As String)
                _RealName = value
            End Set
        End Property

        Friend TelNrListe As List(Of C_Telefonnummer)
        Friend EMailListe As List(Of C_EMail)

        Private _Mod_Time As Integer
        Friend Property Mod_Time() As Integer
            Get
                Return _Mod_Time
            End Get
            Set(ByVal value As Integer)
                _Mod_Time = value
            End Set
        End Property

        Private _uniqueid As Integer
        Friend Property uniqueid() As Integer
            Get
                Return _uniqueid
            End Get
            Set(ByVal value As Integer)
                _uniqueid = value
            End Set
        End Property
    End Structure

#End Region

    Public Function ReadXMLTelefonbuch(ByVal XMLTelefonbuch As XmlDocument) As String 'As C_Adressbuch
        Dim Adressbuch As New C_Adressbuch
        Dim XMLTelBuchEintraege As XmlNodeList
        Dim AdressbuchEintrag As C_AdressbuchEintrag
        Dim aktTelNr As C_Telefonnummer
        Dim aktEmail As C_EMail
        Dim aktTelNrListe As List(Of C_Telefonnummer)
        Dim aktEMailListe As List(Of C_EMail)
        Dim aktEintragsListe As New List(Of C_AdressbuchEintrag)

        XMLTelBuchEintraege = XMLTelefonbuch.GetElementsByTagName("contact")

        For Each XMLTelefonbucheintrag As XmlNode In XMLTelBuchEintraege
            AdressbuchEintrag = New C_AdressbuchEintrag

            AdressbuchEintrag.Category = CInt(XMLTelefonbucheintrag.Item("category").InnerText)
            AdressbuchEintrag.Mod_Time = CInt(XMLTelefonbucheintrag.Item("mod_time").InnerText)
            AdressbuchEintrag.uniqueid = CInt(XMLTelefonbucheintrag.Item("uniqueid").InnerText)

            AdressbuchEintrag.RealName = XMLTelefonbucheintrag.Item("person").Item("realName").InnerText

            aktTelNrListe = New List(Of C_Telefonnummer)

            For Each XMLTelNr As XmlElement In XMLTelefonbucheintrag.Item("telephony")
                aktTelNr = New C_Telefonnummer
                aktTelNr.TelNr = XMLTelNr.InnerText

                aktTelNr.ID = CInt(XMLTelNr.GetAttribute("id"))

                aktTelNr.Prio = CInt(XMLTelNr.GetAttribute("prio"))

                Select Case XMLTelNr.GetAttribute("type")
                    Case TelNrType.home.ToString
                        aktTelNr.Type = TelNrType.home
                    Case TelNrType.work.ToString
                        aktTelNr.Type = TelNrType.work
                    Case TelNrType.mobile.ToString
                        aktTelNr.Type = TelNrType.mobile
                    Case TelNrType.fax_work.ToString
                        aktTelNr.Type = TelNrType.fax_work
                End Select

                aktTelNrListe.Add(aktTelNr)
            Next
            AdressbuchEintrag.TelNrListe = aktTelNrListe

            aktEMailListe = New List(Of C_EMail)
            For Each XMLEMail As XmlElement In XMLTelefonbucheintrag.Item("services")
                aktEmail = New C_EMail
                aktEmail.EMail = XMLEMail.InnerText

                aktEmail.ID = CInt(XMLEMail.GetAttribute("id"))

                aktEmail.Classifier = XMLEMail.GetAttribute("classifier")
                aktEMailListe.Add(aktEmail)
            Next
            AdressbuchEintrag.EMailListe = aktEMailListe

            aktEintragsListe.Add(AdressbuchEintrag)
        Next
        Adressbuch.EintragsListe = aktEintragsListe
        Return "dummy"
    End Function

End Class
