Imports System.Xml
Imports System.Timers

Public Class MyXML
    Private XMLDoc As XmlDocument
    Private sDateiPfad As String

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
            .Interval = 30 * 60 * 1000  ' 30 Minuten
            .Start()
        End With
    End Sub
    Public Overloads Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal sDefault As String) As String
        With XMLDoc
            Read = sDefault
            If Not DerEintrag = vbNullString Then
                If IsNumeric(Left(DerEintrag, 1)) Then DerEintrag = "ID" & DerEintrag
                Try
                    If Not .SelectSingleNode("//" & DieSektion & "//" & DerEintrag) Is Nothing Then
                        Read = .SelectSingleNode("//" & DieSektion).Item(DerEintrag).InnerText()
                    Else
                        Read = sDefault
                    End If
                Catch
                End Try
            End If
        End With
    End Function


    Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean

        With XMLDoc
            If IsNumeric(Left(DerEintrag, 1)) Then DerEintrag = "ID" & DerEintrag
            If Not .SelectSingleNode("//" & DieSektion & "//" & DerEintrag) Is Nothing Then
                .SelectSingleNode("//" & DieSektion).Item(DerEintrag).InnerText() = Value
            Else
                Dim xmlEintrag As XmlElement
                Dim xmlText As XmlText
                If .SelectSingleNode("//" & DieSektion) Is Nothing Then .DocumentElement.AppendChild(.CreateElement(DieSektion))
                xmlEintrag = .CreateElement(DerEintrag)
                xmlText = .CreateTextNode(Value)
                xmlEintrag.AppendChild(xmlText)
                .DocumentElement.Item(DieSektion).AppendChild(xmlEintrag)
                .SelectSingleNode("//" & DieSektion).Item(DerEintrag).InnerText() = Value
            End If
            If SpeichereDatei Then .Save(sDateiPfad)
        End With
        Return True
    End Function

    Function ReadTelNr(ByVal DieSektion As String) As String
        Dim tmpnodelist As XmlNodeList
        Dim stmp As String = vbNullString
        ReadTelNr = ";"
        With XMLDoc
            If Not .SelectSingleNode("//" & DieSektion) Is Nothing Then
                tmpnodelist = .SelectNodes("//" & DieSektion & "//*[starts-with(name(.), ""POTS"") or starts-with(name(.), ""MSN"") or (starts-with(name(.), ""SIP"") and not (starts-with(name(.), ""SIPID"")))]")
                If Not tmpnodelist.Count = 0 Then
                    For Each temxmlnode As XmlNode In tmpnodelist
                        stmp += temxmlnode.InnerText & ";"
                    Next
                    ReadTelNr = Left(stmp, Len(stmp) - 1)
                End If
            End If
        End With
    End Function

    Sub Delete(ByVal DieSektion As String)
        With XMLDoc
            If Not .SelectSingleNode("//" & DieSektion) Is Nothing Then
                .SelectSingleNode("//" & DieSektion).RemoveAll()
            End If
        End With
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

    Private Sub tSpeichern_Elapsed(sender As Object, e As ElapsedEventArgs) Handles tSpeichern.Elapsed
        XMLDoc.Save(sDateiPfad)
    End Sub
End Class
