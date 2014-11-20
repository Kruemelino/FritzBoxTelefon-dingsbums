Imports System.IO
Imports System.Xml
Imports System.Windows.Forms

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
        ' Me.Show()

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
End Class