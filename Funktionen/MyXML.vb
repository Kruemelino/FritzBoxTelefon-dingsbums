Imports System.Xml
Imports System.Timers

Public Class MyXML
    Private XMLDoc As XmlDocument
    Private sDateiPfad As String

    Private Const Speicherintervall As Double = 15 'in Minuten
    Private Const RootName As String = "FritzOutlookXML"
    Private WithEvents tSpeichern As Timer

    Public Sub New(ByVal DateiPfad As String)
        sDateiPfad = DateiPfad
        XMLDoc = New XmlDocument()
        With My.Computer.FileSystem
            If .FileExists(sDateiPfad) And .GetFileInfo(sDateiPfad).Extension.ToString = ".xml" Then
                XMLDoc.Load(sDateiPfad)
            Else
                XMLDoc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><" & RootName & "/>")
            End If
        End With
        RemoveJournalNodes()
        tSpeichern = New Timer
        With tSpeichern
            .Interval = TimeSpan.FromMinutes(Speicherintervall).TotalMilliseconds  ' 30 Minuten
            .Start()
        End With
    End Sub

    Protected Overrides Sub Finalize()
        XMLDoc.Save(sDateiPfad)
        XMLDoc = Nothing
        If Not tSpeichern Is Nothing Then
            tSpeichern.Stop()
            tSpeichern.Dispose()
            tSpeichern = Nothing
        End If

        MyBase.Finalize()
    End Sub

#Region "Read"
    Public Overloads Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal sDefault As String) As String
        Dim StrArr As New ArrayList
        With StrArr
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Read(StrArr, sDefault)
    End Function

    Public Overloads Function Read(ByVal StrArr As ArrayList, ByVal sDefault As String) As String
        Read = sDefault

        Dim xPath As String = CreateXPath(StrArr)
        StrArr = Nothing
        If Not XMLDoc.SelectSingleNode(xPath) Is Nothing Then
            Read = XMLDoc.SelectSingleNode(xPath).InnerText
        End If

    End Function
#End Region

#Region "Write"
    Public Overloads Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean
        Dim StrArr As New ArrayList
        With StrArr
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Write(StrArr, Value, SpeichereDatei)
    End Function

    Public Overloads Function Write(ByVal ZielDaten As ArrayList, ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean
        Dim StrArr As New ArrayList
        Dim sTmpXPath As String = vbNullString
        Dim xPath As String

        xPath = CreateXPath(ZielDaten)
        With XMLDoc
            If Not .SelectSingleNode(xPath) Is Nothing Then
                .SelectSingleNode(xPath).InnerText() = Value
            Else
                For Each sNodeName As String In ZielDaten
                    If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
                    StrArr.Add(sNodeName)
                    xPath = CreateXPath(StrArr)
                    If .SelectSingleNode(xPath) Is Nothing Then
                        .SelectSingleNode(sTmpXPath).AppendChild(.CreateElement(sNodeName))
                    End If
                    sTmpXPath = xPath
                Next
                Write(ZielDaten, Value, SpeichereDatei)
            End If
            If SpeichereDatei Then SpeichereXMLDatei()
        End With
        StrArr = Nothing
        Return True
    End Function
#End Region

#Region "Löschen"

    Public Overloads Function Delete(ByVal DieSektion As String) As Boolean
        Dim StrArr As New ArrayList
        StrArr.Add(DieSektion)
        Return Delete(StrArr)
    End Function

    Public Overloads Function Delete(ByVal alStrArr As ArrayList) As Boolean

        Dim xPath As String = CreateXPath(alStrArr)
        With XMLDoc
            If Not .SelectSingleNode(xPath) Is Nothing Then
                .SelectSingleNode(xPath).RemoveAll()
            End If
        End With
        alStrArr = Nothing
        Return True
    End Function

#End Region

#Region "Speichern"
    Sub SpeichereXMLDatei()
        XMLDoc.Save(sDateiPfad)
    End Sub

    Private Sub tSpeichern_Elapsed(sender As Object, e As ElapsedEventArgs) Handles tSpeichern.Elapsed
        SpeichereXMLDatei()
    End Sub
#End Region

#Region "Stuff"
    Function ReadAllTelNr(ByVal DieSektion As String) As String
        Dim tmpNodeList As XmlNodeList
        Dim StrArr As New ArrayList
        Dim xPath As String

        ReadAllTelNr = ";"

        With XMLDoc
            StrArr.Add(DieSektion)
            xPath = CreateXPath(StrArr)
            If Not .SelectSingleNode(xPath) Is Nothing Then
                StrArr.Add("*[starts-with(name(.), ""POTS"") or starts-with(name(.), ""MSN"") or (starts-with(name(.), ""SIP"") and not (starts-with(name(.), ""SIPID"")))]")
                xPath = CreateXPath(StrArr)
                tmpNodeList = .SelectNodes(xPath)

                If Not tmpNodeList.Count = 0 Then
                    For Each tmpXmlNode As XmlNode In tmpNodeList
                        If Not tmpXmlNode.InnerText = vbNullString Then ReadAllTelNr += tmpXmlNode.InnerText & ";"
                    Next
                    ReadAllTelNr = Left(ReadAllTelNr, Len(ReadAllTelNr) - 1)
                End If
            End If
        End With
    End Function

    Private Sub RemoveJournalNodes()
        Dim tmpNodeSchließZeit As XmlNode
        Dim tmpJournalRootNode As XmlNode
        Dim StrArr As New ArrayList
        Dim xPath As String

        With XMLDoc
            StrArr.Add("Journal")
            StrArr.Add("SchließZeit")
            xPath = CreateXPath(StrArr)
            tmpNodeSchließZeit = .SelectSingleNode(xPath)
            StrArr.Remove("SchließZeit")
            xPath = CreateXPath(StrArr)
            tmpJournalRootNode = .SelectSingleNode(xPath)
            tmpJournalRootNode.RemoveAll()

            If Not tmpNodeSchließZeit Is Nothing Then
                tmpJournalRootNode.AppendChild(tmpNodeSchließZeit)
            End If
        End With
    End Sub

    Function CreateXPath(ByVal xPathElements As ArrayList) As String
        If Not xPathElements.Item(0).ToString = XMLDoc.DocumentElement.Name Then xPathElements.Insert(0, XMLDoc.DocumentElement.Name)
        CreateXPath = "/" & Join(xPathElements.ToArray(), "/")
    End Function

    Function GetXMLDateiPfad() As String
        Return sDateiPfad
    End Function
#End Region

End Class
