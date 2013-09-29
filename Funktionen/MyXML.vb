Imports System.Xml
Imports System.Timers

Public Class MyXML
    Private XMLDoc As XmlDocument
    Private sDateiPfad As String

    Private Const Speicherintervall As Double = 30 'in Minuten

    Private WithEvents tSpeichern As Timer

    Public Sub New(ByVal DateiPfad As String)
        sDateiPfad = DateiPfad
        XMLDoc = New XmlDocument()
        With My.Computer.FileSystem
            If .FileExists(sDateiPfad) And .GetFileInfo(sDateiPfad).Extension.ToString = ".xml" Then
                XMLDoc.Load(sDateiPfad)
            Else
                XMLDoc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><FritzOutlookXML/>")
            End If
        End With
        tSpeichern = New Timer
        With tSpeichern
            .Interval = TimeSpan.FromMinutes(Speicherintervall).TotalMilliseconds  ' 30 Minuten
            .Start()
        End With
    End Sub
#Region "Read"
    Public Overloads Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal sDefault As String) As String
        Return Read(New String() {DieSektion, DerEintrag}, sDefault)
    End Function
    Public Overloads Function Read(ByVal ZielDaten As String(), ByVal sDefault As String) As String
        Read = sDefault
        Dim StrArr As New ArrayList
        For Each sNodeName As String In ZielDaten
            If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
            StrArr.Add(sNodeName)
        Next

        Dim xPath As String = CreateXPath(StrArr)
        StrArr = Nothing
        If Not XMLDoc.SelectSingleNode(xPath) Is Nothing Then
            Read = XMLDoc.SelectSingleNode(xPath).InnerText
        Else
            Read = sDefault
        End If

    End Function
#End Region
#Region "Write"
    Public Overloads Function Write(ByVal ZielDaten As String(), ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean
        Dim StrArr As New ArrayList
        Dim sTmpXPath As String = vbNullString
        Dim xPath As String
        For Each sNodeName As String In ZielDaten
            If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
            StrArr.Add(sNodeName)
        Next
        xPath = CreateXPath(StrArr)
        With XMLDoc
            If Not .SelectSingleNode(xPath) Is Nothing Then
                .SelectSingleNode(xPath).InnerText() = Value
            Else
                StrArr.RemoveRange(0, StrArr.Count)
                For Each sNodeName As String In ZielDaten
                    If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
                    StrArr.Add(sNodeName)
                    xPath = CreateXPath(StrArr)
                    If .SelectSingleNode(xPath) Is Nothing Then
                        .SelectSingleNode(sTmpXPath).AppendChild(.CreateElement(sNodeName))
                    Else
                        sTmpXPath = xPath
                    End If
                Next
                Write(ZielDaten, Value, SpeichereDatei)
            End If
            If SpeichereDatei Then SpeichereXMLDatei()
        End With
        StrArr = Nothing
        Return True
    End Function

    Public Overloads Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean
        Return Write(New String() {DieSektion, DerEintrag}, Value, SpeichereDatei)
    End Function
#End Region

    Function ReadAllTelNr(ByVal DieSektion As String) As String
        Dim tmpNodeList As XmlNodeList
        Dim StrArr As New ArrayList
        Dim stmp As String = vbNullString
        ReadAllTelNr = ";"

        With XMLDoc
            StrArr.Add(DieSektion)
            Dim xPath As String = CreateXPath(StrArr)
            If Not .SelectSingleNode(xPath) Is Nothing Then
                StrArr.Add("*[starts-with(name(.), ""POTS"") or starts-with(name(.), ""MSN"") or (starts-with(name(.), ""SIP"") and not (starts-with(name(.), ""SIPID"")))]")
                xPath = CreateXPath(StrArr)
                tmpNodeList = .SelectNodes(xPath)

                If Not tmpNodeList.Count = 0 Then
                    For Each tmpXmlNode As XmlNode In tmpNodeList
                        If Not tmpXmlNode.InnerText = vbNullString Then stmp += tmpXmlNode.InnerText & ";"
                    Next
                    ReadAllTelNr = Left(stmp, Len(stmp) - 1)
                End If
            End If
        End With
    End Function
    Function CreateXPath(ByVal Pfad As ArrayList) As String
        Pfad.Insert(0, XMLDoc.DocumentElement.Name)
        Return "/" & Join(Pfad.ToArray(), "/")
    End Function

    Sub Delete(ByVal DieSektion As String)
        Dim StrArr As New ArrayList
        StrArr.Add(DieSektion)
        Dim xPath As String = CreateXPath(StrArr)
        With XMLDoc
            If Not .SelectSingleNode(xPath) Is Nothing Then
                .SelectSingleNode(xPath).RemoveAll()
            End If
        End With
        StrArr = Nothing
    End Sub

    Function GetXMLDateiPfad() As String
        Return sDateiPfad
    End Function

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

    Sub SpeichereXMLDatei()
        XMLDoc.Save(sDateiPfad)
    End Sub

    Private Sub tSpeichern_Elapsed(sender As Object, e As ElapsedEventArgs) Handles tSpeichern.Elapsed
        SpeichereXMLDatei()
    End Sub
End Class
