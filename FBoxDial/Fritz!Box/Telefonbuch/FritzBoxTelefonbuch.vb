Imports System.Threading.Tasks

Friend Module FritzBoxTelefonbuch

    Friend Async Function LadeFritzBoxTelefonbücher() As Task(Of FritzBoxXMLTelefonbücher)
        Dim OutPutData As Collections.Hashtable
        Dim InPutData As Collections.Hashtable
        Dim PhoneBookXML As FritzBoxXMLTelefonbücher


        Using fboxSOAP As New FritzBoxServices
            ' Ermittle alle verfügbaren Telefonbücher
            OutPutData = fboxSOAP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebookList")

            If OutPutData.ContainsKey("NewPhonebookList") Then
                ' Initialiesiere die Gesamtliste 
                Dim tmpTelefonbücher As New FritzBoxXMLTelefonbücher With {.Telefonbuch = New List(Of FritzBoxXMLTelefonbuch)}

                ' Ermittle alle Telefonbuchdaten und starte die Verarbeitung in einer Schleife
                For Each PhonebookID In OutPutData.Item("NewPhonebookList").ToString.Split(",")
                    InPutData = New Collections.Hashtable From {{"NewPhonebookID", PhonebookID}}
                    OutPutData = fboxSOAP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebook", InPutData)
                    If OutPutData.ContainsKey("NewPhonebookURL") Then
                        ' Deserialisiere das Telefonbuch
                        PhoneBookXML = Await DeserializeObject(Of FritzBoxXMLTelefonbücher)(OutPutData.Item("NewPhonebookURL").ToString())
                        ' Füge die Telefonbücher zusammen
                        tmpTelefonbücher.Telefonbuch.AddRange(PhoneBookXML.Telefonbuch)
                    End If
                Next
                Return tmpTelefonbücher
            Else
                Return Nothing
            End If
        End Using
    End Function

End Module

