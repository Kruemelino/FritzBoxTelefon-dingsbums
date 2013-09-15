Imports System.Xml

Public Class MyXML
    Private XMLDoc As XmlDocument
    Private sDateiPfad As String
    Public Sub New(ByVal DateiPfad As String)
        sDateiPfad = DateiPfad
        XMLDoc = New XmlDocument()
        With My.Computer.FileSystem
            If .FileExists(sDateiPfad) And .GetFileInfo(sDateiPfad).Extension = "xml" Then
                XMLDoc.Load(sDateiPfad)
            Else
                XMLDoc.LoadXml("<FritzOutlookXML/>")
            End If
        End With

    End Sub

    Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Def As String) As String
        With XMLDoc
            If Not .InnerXml.Contains(DieSektion) Then .DocumentElement.AppendChild(.CreateElement(DieSektion))
            If .SelectSingleNode("//" & DieSektion).InnerXml.Contains(DerEintrag) Then
                Read = .SelectSingleNode("//" & DieSektion).Item(DerEintrag).InnerText()
            Else
                Read = Def
            End If
        End With
    End Function

    Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String) As Boolean
        With XMLDoc
            If Not .InnerXml.Contains(DieSektion) Then .DocumentElement.AppendChild(.CreateElement(DieSektion))
            If .SelectSingleNode("//" & DieSektion).InnerXml.Contains(DerEintrag) Then
                .SelectSingleNode("//" & DieSektion).Item(DerEintrag).InnerText() = Value
            Else
                Dim xmlEintrag As XmlElement
                Dim xmlText As XmlText

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

    Protected Overrides Sub Finalize()
        XMLDoc.Save(sDateiPfad)
        XMLDoc = Nothing

        MyBase.Finalize()
    End Sub
End Class
