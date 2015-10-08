Imports System.Threading
Imports System.Xml

Public Class formImportAnrList
#Region "BackgroundWorker"
    Private WithEvents BWDownloadAnrListe As New System.ComponentModel.BackgroundWorker ' Background Worker zum Runterladen der Anrufliste
    Private WithEvents BWAnrListeAuswerten As New System.ComponentModel.BackgroundWorker
#End Region

#Region "Delegate"
    Private Delegate Sub DelgSetProgressbar()
    Private Delegate Sub DelgSetButtonHerunterladen()
#End Region

#Region "Eigene Klassen"
    Private C_FBox As FritzBox
    Private C_AnrMon As AnrufMonitor
    Private C_hf As Helfer
    Private C_DP As DataProvider
    Private C_XML As XML
#End Region

#Region "Structure"
    Private Structure AnrListData
        Friend StartZeit As Date
        Friend EndZeit As Date
    End Structure
#End Region

#Region "Eigene Variablen"
    Private Abbruch As Boolean
    Private Anzeigen As Boolean
    Private StatusWert As Integer
    Private SID As String
    Private EntryCount As Integer = -1
    Private CSVAnrListe As String
    'Private XMLAnrListe As XmlDocument
#End Region

    Friend Sub New(ByVal FritzBoxKlasse As FritzBox, _
                   ByVal AnrMonKlasse As AnrufMonitor, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal DataProviderKlasse As DataProvider, _
                   ByVal XMLKlasse As XML)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_FBox = FritzBoxKlasse
        C_DP = DataProviderKlasse
        C_hf = HelferKlasse
        C_AnrMon = AnrMonKlasse
        C_XML = XMLKlasse
    End Sub

    Private Sub formJournalimport_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.StartDatum.Value = C_DP.P_StatOLClosedZeit
        Me.StartZeit.Value = C_DP.P_StatOLClosedZeit
        Me.EndDatum.Value = System.DateTime.Now
        Me.EndZeit.Value = System.DateTime.Now
    End Sub

    Friend Sub StartAuswertung(ByVal ShowForm As Boolean)
        Abbruch = False
        Anzeigen = ShowForm
        If Anzeigen Then Me.Show() 'wenn gewollt
        With BWDownloadAnrListe
            .WorkerSupportsCancellation = True
            .RunWorkerAsync()
        End With
    End Sub
#Region "Herunterladen"
    Private Sub DownloadAnrListe_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWDownloadAnrListe.DoWork
        'If C_DP.P_RBFBComUPnP Then
        '  e.Result = C_FBox.DownloadAnrListeV2()
        'Else
        e.Result = C_FBox.DownloadAnrListeV1()
        'End If
    End Sub

    Private Sub DownloadAnrListe_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWDownloadAnrListe.RunWorkerCompleted
        Dim Übergabe As AnrListData

        If Me.InvokeRequired Then
            Dim D As New DelgSetButtonHerunterladen(AddressOf ButtonEnable)
            Invoke(D)
        Else
            Me.ButtonHerunterladen.Enabled = True
        End If

        'If C_DP.P_RBFBComUPnP Then
        '    CSVAnrListe = DataProvider.P_Def_LeerString
        '    XMLAnrListe = CType(e.Result, XmlDocument)
        'Else
        CSVAnrListe = CStr(e.Result)
        '    XMLAnrListe = Nothing
        'End If

        With Übergabe
            .StartZeit = C_DP.P_StatOLClosedZeit
            .EndZeit = System.DateTime.Now
        End With

        If Not Anzeigen Then
            With BWAnrListeAuswerten
                .WorkerReportsProgress = True
                .RunWorkerAsync(Übergabe)
            End With
        End If
    End Sub

    Sub ButtonEnable()
        Me.ButtonHerunterladen.Enabled = True
    End Sub
#End Region

#Region "Auswertung"
    Private Sub BGAnrListeAuswerten_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrListeAuswerten.DoWork
        'If C_DP.P_RBFBComUPnP Then
        '   JournalXML(CType(e.Argument, AnrListData))
        'Else
        JournalCSV(CType(e.Argument, AnrListData))
        'End If
    End Sub

    Private Sub BGAnrListeAuswerten_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BWAnrListeAuswerten.ProgressChanged
        StatusWert = e.ProgressPercentage
        If Me.InvokeRequired Then
            Dim D As New DelgSetProgressbar(AddressOf SetProgressbar)
            Invoke(D)
        End If
    End Sub

    Private Sub SetProgressbar()
        Me.ProgressBar1.Value = StatusWert
        Me.lblBG1Percent.Text = StatusWert & " % (" & StatusWert * EntryCount \ 100 & "/" & EntryCount & ")"
        Me.Text = "Journalimport - " & Me.lblBG1Percent.Text
        If StatusWert = 100 Then Me.ButtonStart.Enabled = True
    End Sub

    Private Sub BGAnrListeAuswerten_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWAnrListeAuswerten.RunWorkerCompleted
        BWAnrListeAuswerten.Dispose()
        BWDownloadAnrListe.Dispose()
    End Sub

    Private Sub JournalCSV(ByVal AnrListeData As AnrListData)

        Dim aktZeile As String()        ' aktuell bearbeitete Zeile
        Dim AnrZeit As String           ' Zeitpunkt des Anrufs
        Dim AnrID As String             ' ID des Anrufes
        Dim vFBStatus As String()       ' generierter Status-String
        Dim j, a, b As Integer          ' Zählvariable
        Dim AnrListe As String()
        Dim xPathTeile As New ArrayList
        Dim tmp() As String

        Dim CSVTelefonat As C_Telefonat

        BWDownloadAnrListe.Dispose()
        Dim StartZeile As Integer ' Zeile der csv, die das Erste zu importierenden Telefonat enthält
        Dim EndZeile As Integer = -1 ' Zeile der csv, die das Letzte zu importierenden Telefonat enthält

        C_FBox.FBLogout(SID)

        If InStr(CSVAnrListe, "!DOCTYPE", CompareMethod.Text) = 0 And Not CSVAnrListe = DataProvider.P_Def_LeerString Then

            CSVAnrListe = Strings.Left(CSVAnrListe, Len(CSVAnrListe) - 2) 'Datei endet mit zwei chr(10) -> abschneiden
            ' Datei wird zuerst in ein String-Array gelesen und dann ausgewertet.
            AnrListe = Split(CSVAnrListe, Chr(10), , CompareMethod.Text)
            If Not AnrListe.Length = 1 Then
                j = -1
                ' Ermittle Startzeile
                Do
                    j += 1
                Loop Until AnrListe.GetValue(j).ToString = "Typ;Datum;Name;Rufnummer;Nebenstelle;Eigene Rufnummer;Dauer" Or j = AnrListe.Length
                ' Ermittle die Position des Ersten und Letzten zu importierenden Telefonats

                StartZeile = j + 1
                If CStr(AnrListe.GetValue(j + 1)) = DataProvider.P_Def_LeerString Then
                    j += 1
                    StartZeile = j + 1
                End If

                Do
                    j += 1
                    AnrZeit = CStr(Split(CStr(AnrListe.GetValue(j)), ";", , CompareMethod.Text).GetValue(1)) & ":00"
                    If CDate(AnrZeit) < AnrListeData.StartZeit Then EndZeile = j - 1 ' AnrZeit nach Startzeit
                    If CDate(AnrZeit) > AnrListeData.EndZeit Then StartZeile = j + 1 ' AnrZeit vor Endzeit
                    Windows.Forms.Application.DoEvents()
                Loop Until CDate(AnrZeit) < AnrListeData.StartZeit Or j = AnrListe.Length - 1

                If j = AnrListe.Length - 1 Then EndZeile = AnrListe.Length - 1

                EntryCount = EndZeile - StartZeile + 1
                If EntryCount > 0 Then

                    b = 0 ' Anzahl der tatsächlich importierten Telefonate
                    a = 1
                    For j = EndZeile To StartZeile Step -1 ' Array wird Zeilenweise rückwärts durchlaufen
                        CSVTelefonat = Nothing
                        CSVTelefonat = New C_Telefonat
                        With CSVTelefonat
                            ' aktuelle Zeile wird ebenfalls in ein Array geteilt, damit ist ein direkter Zugriff möglich.
                            aktZeile = Split(CStr(AnrListe.GetValue(j)), ";", , CompareMethod.Text)

                            If Abbruch Then Exit For

                            .Verpasst = aktZeile(0) = "2"
                            .Angenommen = Not .Verpasst
                            .Zeit = CDate(aktZeile(1))
                            .TelNr = aktZeile(3)
                            .TelName = aktZeile(4)
                            .MSN = aktZeile(5)
                            ' Umrechnung der Dauer (h:m) in volle Sekunden
                            tmp = Split(aktZeile(6), ":")
                            .Dauer = CInt(tmp(0)) * 60 + CInt(tmp(1)) * 60

                            ' Bei analogen Anschlüssen steht "Festnetz" in MSN
                            If .MSN = "Festnetz" Then .MSN = C_XML.Read(C_DP.XMLDoc, "Telefone", "POTS", DataProvider.P_Def_ErrorMinusOne_String)
                            ' MSN von dem "Internet: " bereinigen
                            .MSN = Replace(.MSN, "Internet: ", String.Empty)

                            If C_DP.P_CLBTelNr.Contains(C_hf.EigeneVorwahlenEntfernen(.MSN)) Or DataProvider.P_Debug_AnrufSimulation Then
                                b += 1
                                .NSN = -1

                                If Not .Verpasst Then
                                    Select Case .TelName
                                        Case "Durchwahl"
                                            .NSN = 3
                                        Case "ISDN Gerät"
                                            .NSN = 4
                                        Case "Fax (intern/PC)"
                                            .NSN = 5
                                        Case "Data S0"
                                            .NSN = 36
                                        Case "Data PC"
                                            .NSN = 37
                                        Case Else
                                            With xPathTeile
                                                .Clear()
                                                .Add("Telefone")
                                                .Add("Telefone")
                                                .Add("*")
                                                .Add("Telefon")
                                                .Add("[TelName = """ & CSVTelefonat.TelName & """]")
                                                .Add("@Dialport")
                                            End With
                                            .NSN = CInt(C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String))
                                    End Select
                                End If

                                If Not .NSN = -1 Then
                                    'If NSN < 4 Then NSN -= 1
                                    Select Case .NSN
                                        Case 1 To 3
                                            .NSN -= 1
                                        Case 60 To 69 'DECT
                                            .NSN -= 50
                                    End Select
                                End If

                                AnrID = CStr(DataProvider.P_Def_AnrListIDOffset + b)
                                Select Case CInt(aktZeile(0))
                                    Case 1 ' eingehender Anruf: angenommen
                                        vFBStatus = Split(AnrZeit & ";RING;" & AnrID & ";" & .TelNr & ";" & .MSN & ";;", ";", , CompareMethod.Text)
                                        C_AnrMon.AnrMonRING(vFBStatus)
                                        vFBStatus = Split(AnrZeit & ";CONNECT;" & AnrID & ";" & .NSN & ";" & .TelNr & ";", ";", , CompareMethod.Text)
                                        C_AnrMon.AnrMonCONNECT(vFBStatus)
                                    Case 2 ' eingehender Anruf: nicht angenommen
                                        vFBStatus = Split(AnrZeit & ";RING;" & AnrID & ";" & .TelNr & ";" & .MSN & ";;", ";", , CompareMethod.Text)
                                        C_AnrMon.AnrMonRING(vFBStatus)
                                    Case 3, 4 ' ausgehender Anruf
                                        vFBStatus = Split(AnrZeit & ";CALL;" & AnrID & ";0;" & .MSN & ";" & .TelNr & ";;", ";", , CompareMethod.Text)
                                        C_AnrMon.AnrMonCALL(vFBStatus)
                                        vFBStatus = Split(AnrZeit & ";CONNECT;" & AnrID & ";" & .NSN & ";" & .TelNr & ";", ";", , CompareMethod.Text)
                                        C_AnrMon.AnrMonCONNECT(vFBStatus)
                                End Select

                                If Abbruch Then Exit For
                                vFBStatus = Split(AnrZeit & ";DISCONNECT;" & AnrID & ";" & .Dauer & ";", ";", , CompareMethod.Text)
                                C_AnrMon.AnrMonDISCONNECT(vFBStatus)

                            End If
                            If Anzeigen Then BWAnrListeAuswerten.ReportProgress(a * 100 \ EntryCount)
                            a += 1

                        End With
                    Next
                End If
                ' Registry zurückschreiben
                C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)
                C_hf.LogFile("Aus der 'FRITZ!Box_Anrufliste.csv' " & C_hf.IIf(b = 1, "wurde " & b & " Journaleintag", "wurden " & b & " Journaleintäge").ToString & " importiert.")
            Else
                C_hf.LogFile("Auswertung von 'Anrufliste.csv' wurde abgebrochen.")
            End If
            If Anzeigen Then BWAnrListeAuswerten.ReportProgress(100)
            BWAnrListeAuswerten.Dispose()
        End If
        CSVTelefonat = Nothing
    End Sub

    ' ''' <summary>
    ' ''' Importiert die Journaleinträge aus der XML-Datei
    ' ''' <c>
    ' '''   <root>
    ' '''  	<timestamp>123456</timestamp>
    ' '''  	<Call>
    ' '''  		<Id>123</Id>
    ' '''  		<Type>3</Type>
    ' '''  		<Called>0123456789</Called>
    ' '''  		<Caller>SIP: 98765</Caller>
    ' '''  		<Name>Max Mustermann</Name>
    ' '''  		<Numbertype/>
    ' '''  		<Device>Mobilteil 1</Device>
    ' '''  		<Port>10</Port>
    ' '''  		<Date>23.09.11 08:13</Date>
    ' '''  		<Duration>0:01</Duration>
    ' '''  		<Count/>
    ' '''  		<Path/>
    ' '''  	</Call>
    ' '''  	<Call>
    ' '''  		<Id>122</Id>
    ' '''  		<Type>1</Type>
    ' '''  		<Caller>012456789</Caller>
    ' '''  		<Called>56789</Called>
    ' '''  		<Name>Max Mustermann</Name>
    ' '''  		<Numbertype/>
    ' '''  		<Device>Anrufbeantworter 1</Device>
    ' '''  		<Port>40</Port>
    ' '''  		<Date>22.09.11 14:19</Date>
    ' '''  		<Duration>0:01</Duration>
    ' '''  		<Count/>
    ' '''  		<Path>/download.lua?path=/var/media/ftp/JetFlash-Transcend4GB-01/FRITZ/voicebox/rec/rec.0.000</Path>
    ' '''  	</Call>
    ' '''  </root>
    ' ''' </c>
    ' ''' </summary>
    ' ''' <param name="AnrListeData"></param>
    ' ''' <remarks></remarks>
    'Private Sub JournalXML(ByVal AnrListeData As AnrListData)

    '    Dim xPathTeile As New ArrayList
    '    Dim CallNodeList As XmlNodeList
    '    Dim CallNode As XmlNode
    '    Dim xPath As String
    '    Dim ImportXML As New XmlDocument
    '    Dim AnrTyp As String            ' Typ des Anrufs
    '    Dim AnrZeit As String           ' Zeitpunkt des Anrufs
    '    Dim AnrTelNr As String          ' Name und TelNr des Telefonpartners
    '    Dim AnrID As String             ' ID des Anrufes
    '    Dim Nebenstelle As String       ' verwendete Nebenstelle
    '    Dim MSN As String               ' verwendete MSN
    '    Dim NSN As Integer              ' verwendete Nebenstellennummer
    '    Dim Dauer As String             ' Dauer des Telefonats
    '    Dim a, b As Integer             ' Zählvariable
    '    Dim vFBStatus As String()       ' generierter Status-String

    '    ImportXML.InnerXml = "<root/>"


    '    xPathTeile.Add("Call")
    '    xPath = C_XML.CreateXPath(XMLAnrListe, xPathTeile)
    '    CallNodeList = XMLAnrListe.SelectNodes(xPath)

    '    For Each CallNodeListItem As XmlNode In CallNodeList
    '        If CallNodeListItem.NodeType = XmlNodeType.Element Then
    '            With CType(CallNodeListItem, XmlElement)
    '                If CDate(.Item("Date").InnerText) > AnrListeData.StartZeit And CDate(.Item("Date").InnerText) < AnrListeData.EndZeit Then
    '                    ImportXML.Item("root").AppendChild(ImportXML.ImportNode(CallNodeListItem, True))
    '                End If
    '            End With
    '        End If
    '    Next
    '    XMLAnrListe = Nothing

    '    xPath = C_XML.CreateXPath(ImportXML, xPathTeile)
    '    CallNodeList = ImportXML.SelectNodes(xPath)

    '    EntryCount = CallNodeList.Count
    '    a = 1
    '    For Each CallNodeListItem As XmlNode In CallNodeList
    '        If Abbruch Then Exit For
    '        If CallNodeListItem.NodeType = XmlNodeType.Element Then
    '            With CType(CallNodeListItem, XmlElement)
    '                AnrTyp = .Item("Type").InnerText
    '                AnrZeit = .Item("Date").InnerText & ":00"
    '                AnrTelNr = .Item("Called").InnerText
    '                Nebenstelle = .Item("Port").InnerText
    '                MSN = .Item("Caller").InnerText
    '                Dauer = .Item("Duration").InnerText
    '                NSN = CInt(.Item("Port").InnerText)
    '            End With

    '            '' Bei analogen Anschlüssen steht "Festnetz" in MSN
    '            'If MSN = "Festnetz" Then MSN = C_XML.Read(C_DP.XMLDoc, "Telefone", "POTS", DataProvider.P_Def_ErrorMinusOne_String)
    '            '' MSN von dem "Internet: " bereinigen
    '            'If Not MSN = String.Empty Then MSN = Replace(MSN, "Internet: ", String.Empty)

    '            If C_DP.P_CLBTelNr.Contains(C_hf.EigeneVorwahlenEntfernen(MSN)) Or DataProvider.P_Debug_AnrufSimulation Then
    '                b += 1
    '                NSN = -1
    '                AnrID = CStr(DataProvider.P_Def_AnrListIDOffset + b)

    '                Select Case CInt(AnrTyp)
    '                    Case 1 ' eingehender Anruf: angenommen
    '                        vFBStatus = Split(AnrZeit & ";RING;" & AnrID & ";" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
    '                        C_AnrMon.AnrMonRING(vFBStatus)
    '                        vFBStatus = Split(AnrZeit & ";CONNECT;" & AnrID & ";" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
    '                        C_AnrMon.AnrMonCONNECT(vFBStatus)
    '                    Case 2 ' eingehender Anruf: nicht angenommen
    '                        vFBStatus = Split(AnrZeit & ";RING;" & AnrID & ";" & AnrTelNr & ";" & MSN & ";;", ";", , CompareMethod.Text)
    '                        C_AnrMon.AnrMonRING(vFBStatus)
    '                    Case 3, 4 ' ausgehender Anruf
    '                        vFBStatus = Split(AnrZeit & ";CALL;" & AnrID & ";0;" & MSN & ";" & AnrTelNr & ";;", ";", , CompareMethod.Text)
    '                        C_AnrMon.AnrMonCALL(vFBStatus)
    '                        vFBStatus = Split(AnrZeit & ";CONNECT;" & AnrID & ";" & NSN & ";" & AnrTelNr & ";", ";", , CompareMethod.Text)
    '                        C_AnrMon.AnrMonCONNECT(vFBStatus)
    '                End Select
    '                If Abbruch Then Exit For
    '                vFBStatus = Split(AnrZeit & ";DISCONNECT;" & AnrID & ";" & Dauer & ";", ";", , CompareMethod.Text)
    '                C_AnrMon.AnrMonDISCONNECT(vFBStatus)
    '            End If

    '            Dauer = CStr((CLng(Strings.Left(Dauer, InStr(1, Dauer, ":", CompareMethod.Text) - 1)) * 60 + CLng(Mid(Dauer, InStr(1, Dauer, ":", CompareMethod.Text) + 1))) * 60)
    '        End If
    '        If Anzeigen Then BWAnrListeAuswerten.ReportProgress(a * 100 \ EntryCount)
    '        a += 1
    '    Next

    '    'Tag 		Type 		Description
    '    'Id 		Integer		Unique ID per call.
    '    'Type 		Integer 	1 incoming,	2 missed, 3 outgoing, 9 active incoming, 10 rejected incoming, 11 active outgoing
    '    'Called 	String 		Number of called party
    '    'Caller 	String 		Number of calling party
    '    'Name 		String 		Name of called/ called party (outgoing/ incoming call)
    '    'Numbertype String 		pots, isdn, sip, umts, ''
    '    'Device 	String 		Name of used telephone port.
    '    'Port 		String 		Number of telephone port.
    '    'Date 		Date-String	31.07.12 12:03
    '    'Duration 	String 		hh:mm (minutes rounded up)
    '    'Path 		String 		URL path to TAM or FAX file.
    'End Sub
#End Region

#Region "Button"
    Private Sub ButtonSchließen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSchließen.Click
        Me.Hide()
    End Sub

    Private Sub ButtonStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonStart.Click
        Dim Übergabe As AnrListData
        Abbruch = False
        Me.ButtonStart.Enabled = False
        Do While BWDownloadAnrListe.IsBusy
            Windows.Forms.Application.DoEvents()
        Loop
        StatusWert = 0
        SetProgressbar()
        BereichAuswertung.Enabled = True
        With Übergabe
            .StartZeit = CDate(Me.StartDatum.Text & " " & Me.StartZeit.Text)
            .EndZeit = CDate(Me.EndDatum.Text & " " & Me.EndZeit.Text)
        End With

        With BWAnrListeAuswerten
            .WorkerReportsProgress = True
            .RunWorkerAsync(Übergabe)
        End With
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        Abbruch = True
    End Sub
    Private Sub ButtonHerunterladen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonHerunterladen.Click
        Me.ButtonHerunterladen.Enabled = False
        With BWDownloadAnrListe
            .WorkerSupportsCancellation = True
            .RunWorkerAsync()
        End With
    End Sub
#End Region
End Class