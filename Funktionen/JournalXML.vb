Imports System.Xml

Public Class JournalXML
    Private Helfer As Helfer
    Private InIPfad As String
    Private PFAD As String

    Public Sub New(ByVal FilePfad As String, ByVal Helferklasse As Helfer)
        Helfer = Helferklasse
        InIPfad = FilePfad
        PFAD = Helfer.Dateipfade(InIPfad, "JournalXML")

        With My.Computer.FileSystem
            If .FileExists(PFAD) Then .DeleteFile(PFAD)
        End With
    End Sub

    Sub NeuerJI(ByVal ID As Integer, _
                ByVal Typ As String, _
                ByVal Zeit As String, _
                ByVal MSN As String, _
                ByVal TelNr As String, _
                ByVal KontaktID As String, _
                ByVal StoreID As String)

        Dim xml As New XmlDocument()
        Dim xmlKD As XmlElement
        Dim xmlTyp As XmlElement
        Dim xmlZeit As XmlElement
        Dim xmlMSN As XmlElement
        Dim xmlTelNr As XmlElement
        Dim xmlKontaktID As XmlElement
        Dim xmlStoreID As XmlElement

        Dim xmlText As XmlText

        With xml
            Try
                .Load(PFAD)
            Catch ex As Exception
                'File Exist?
                .LoadXml("<Journal/>")
            End Try

            xmlKD = .CreateElement("ID" & ID)

            xmlTyp = .CreateElement("Typ")
            xmlText = .CreateTextNode(Typ)
            xmlTyp.AppendChild(xmlText)
            xmlKD.AppendChild(xmlTyp)

            xmlZeit = .CreateElement("Zeit")
            xmlText = .CreateTextNode(Zeit)
            xmlZeit.AppendChild(xmlText)
            xmlKD.AppendChild(xmlZeit)

            xmlMSN = .CreateElement("MSN")
            xmlText = .CreateTextNode(MSN)
            xmlMSN.AppendChild(xmlText)
            xmlKD.AppendChild(xmlMSN)

            xmlTelNr = .CreateElement("TelNr")
            xmlText = .CreateTextNode(TelNr)
            xmlTelNr.AppendChild(xmlText)
            xmlKD.AppendChild(xmlTelNr)

            xmlKontaktID = .CreateElement("KontaktID")
            xmlText = .CreateTextNode(KontaktID)
            xmlKontaktID.AppendChild(xmlText)
            xmlKD.AppendChild(xmlKontaktID)

            xmlStoreID = .CreateElement("StoreID")
            xmlText = .CreateTextNode(StoreID)
            xmlStoreID.AppendChild(xmlText)
            xmlKD.AppendChild(xmlStoreID)

            .DocumentElement.AppendChild(xmlKD)
            .Save(PFAD)
        End With
        xml = Nothing
        xmlKontaktID = Nothing
        xmlText = Nothing
    End Sub

    Sub ZuJEhinzufügen(ByVal ID As Integer, _
                       ByVal Name As String, _
                       ByVal Value As String)

        Dim xml As New XmlDocument()
        Dim xmlKontaktID As XmlElement
        Dim xmlText As XmlText

        With xml
            .Load(PFAD)

            xmlKontaktID = .CreateElement(Name)
            xmlText = .CreateTextNode(Value)
            xmlKontaktID.AppendChild(xmlText)

            For i = 0 To .DocumentElement.ChildNodes.Count - 1
                If .DocumentElement.ChildNodes(i).Name = "ID" & ID Then
                    .DocumentElement.ChildNodes(i).AppendChild(xmlKontaktID)
                End If
            Next
            .Save(PFAD)
        End With
        xml = Nothing

    End Sub

    Sub JIauslesen(ByVal ID As Integer, _
                   ByRef NSN As String, _
                   ByRef Zeit As String, _
                   ByRef Typ As String, _
                   ByRef MSN As String, _
                   ByRef TelNr As String, _
                   ByRef StoreID As String, _
                   ByRef KontaktID As String)


        Dim xml As New Xml.XmlDocument()

        With xml
            'Try
            .Load(PFAD)
            For i = 0 To .DocumentElement.ChildNodes.Count - 1
                If .DocumentElement.ChildNodes(i).Name = "ID" & ID Then
                    For j = 0 To .DocumentElement.ChildNodes(i).ChildNodes.Count - 1
                        Select Case xml.DocumentElement.ChildNodes(i).ChildNodes(j).Name
                            Case "Typ"
                                Typ = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                            Case "NSN"
                                NSN = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                            Case "MSN"
                                MSN = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                            Case "Zeit"
                                Zeit = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                            Case "TelNr"
                                TelNr = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                            Case "KontaktID"
                                KontaktID = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                            Case "StoreID"
                                StoreID = .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                        End Select
                    Next
                End If
            Next
            'Catch ex As Exception
            '    Helfer.LogFile("XML-Datei nicht gefunden")
            'End Try

        End With
        xml = Nothing
    End Sub

    Function JEWertAuslesen(ByVal ID As Integer, ByVal Name As String) As String
        If My.Computer.FileSystem.FileExists(PFAD) Then
            JEWertAuslesen = vbNullString
            Dim xml As New Xml.XmlDocument()
            With xml
                .Load(PFAD)
                For i = 0 To .DocumentElement.ChildNodes.Count - 1
                    If .DocumentElement.ChildNodes(i).Name = "ID" & ID Then
                        For j = 0 To .DocumentElement.ChildNodes(i).ChildNodes.Count - 1
                            If xml.DocumentElement.ChildNodes(i).ChildNodes(j).Name = Name Then Return .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText
                        Next
                    End If
                Next
            End With
            xml = Nothing
        Else
            JEWertAuslesen = "-1"
        End If


    End Function

    Sub JEentfernen(ID As Integer)
        Dim node As XmlNode = Nothing
        Dim xml As New XmlDocument()
        With xml
            .Load(PFAD)
            For i = 0 To .DocumentElement.ChildNodes.Count - 1
                If .DocumentElement.ChildNodes(i).Name = "ID" & ID Then
                    node = .DocumentElement.ChildNodes(i)
                    Exit For
                End If
            Next
            If Not node Is Nothing Then .DocumentElement.RemoveChild(node)
            .Save(PFAD)
        End With
        xml = Nothing
    End Sub

    Sub JIÄndern(ByVal ID As Integer, _
                       ByVal Name As String, _
                       ByVal Value As String)

        Dim xml As New XmlDocument()
        Dim xmlKontaktID As XmlElement
        Dim xmlText As XmlText
        Dim abgeschlossen As Boolean = False

        With xml
            .Load(PFAD)

            xmlKontaktID = .CreateElement(Name)
            xmlText = .CreateTextNode(Value)
            xmlKontaktID.AppendChild(xmlText)


            For i = 0 To .DocumentElement.ChildNodes.Count - 1
                If .DocumentElement.ChildNodes(i).Name = "ID" & ID Then
                    For j = 0 To .DocumentElement.ChildNodes(i).ChildNodes.Count - 1
                        If xml.DocumentElement.ChildNodes(i).ChildNodes(j).Name = Name Then
                            .DocumentElement.ChildNodes(i).ChildNodes(j).InnerText = Value
                            abgeschlossen = True
                        End If
                        If abgeschlossen Then Exit For
                    Next
                End If
                If abgeschlossen Then Exit For
            Next
            .Save(PFAD)
        End With
        xml = Nothing
    End Sub

    Protected Overrides Sub Finalize()
        With My.Computer.FileSystem
            If .FileExists(PFAD) Then .DeleteFile(PFAD)
        End With
        MyBase.Finalize()
    End Sub
End Class
