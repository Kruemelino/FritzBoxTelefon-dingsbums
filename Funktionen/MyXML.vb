Imports System.Xml

Public Class MyXML
    Private XMLDoc As XmlDocument
    Private sDateiPfad As String
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
    End Sub

    Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Def As String) As String
        With XMLDoc
            If IsNumeric(Left(DerEintrag, 1)) Then DerEintrag = "ID" & DerEintrag
            If Not .SelectSingleNode("//" & DieSektion & "//" & DerEintrag) Is Nothing Then
                Read = .SelectSingleNode("//" & DieSektion).Item(DerEintrag).InnerText()
            Else
                Read = Def
            End If
        End With
    End Function

    Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String) As Boolean

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
            .Save(sDateiPfad)
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

    Protected Overrides Sub Finalize()
        XMLDoc.Save(sDateiPfad)
        XMLDoc = Nothing

        MyBase.Finalize()
    End Sub
End Class
