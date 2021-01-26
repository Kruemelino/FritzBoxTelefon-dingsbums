Imports System.Threading.Tasks
Imports System.Collections
Imports System.Runtime.CompilerServices

Friend Module FritzBoxTelefonbuch
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Async Function LadeFritzBoxTelefonbücher() As Task(Of FritzBoxXMLTelefonbücher)
        Dim OutPutData As Hashtable
        Dim InPutData As Hashtable
        Dim PhoneBookXML As FritzBoxXMLTelefonbücher


        Using fboxSOAP As New FritzBoxTR64
            ' Ermittle alle verfügbaren Telefonbücher
            OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "GetPhonebookList")

            If OutPutData.ContainsKey("NewPhonebookList") Then
                ' Initialiesiere die Gesamtliste 
                Dim tmpTelefonbücher As New FritzBoxXMLTelefonbücher With {.Telefonbuch = New List(Of FritzBoxXMLTelefonbuch)}

                ' Ermittle alle Telefonbuchdaten und starte die Verarbeitung in einer Schleife
                For Each TelefonbuchID As String In OutPutData.Item("NewPhonebookList").ToString.Split(",")
                    InPutData = New Hashtable From {{"NewPhonebookID", TelefonbuchID}}
                    OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "GetPhonebook", InPutData)
                    If OutPutData.ContainsKey("NewPhonebookURL") Then
                        NLogger.Debug($"Telefonbuch {TelefonbuchID} heruntergeladen: {OutPutData.Item("NewPhonebookURL")}")
                        ' Deserialisiere das Telefonbuch
                        PhoneBookXML = Await DeserializeObjectAsyc(Of FritzBoxXMLTelefonbücher)(OutPutData.Item("NewPhonebookURL").ToString())
                        ' Setze die ID
                        PhoneBookXML.Telefonbuch.ForEach(Sub(r) r.ID = TelefonbuchID)
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

#Region "Aktionen für Telefonbuch"
    ''' <summary>
    ''' Erstellt ein neues Telefonbuch.
    ''' </summary>
    ''' <param name="TelefonbuchName">Übergabe des neuen Namens des Telefonbuches.</param>
    ''' <returns>XML-Telefonbuch</returns>
    Friend Async Function ErstelleTelefonbuch(TelefonbuchName As String) As Task(Of FritzBoxXMLTelefonbücher)
        Using fboxSOAP As New FritzBoxTR64
            With fboxSOAP
                ' Hole die aktuelle Liste an Telefonbüchern
                Dim TelListeA As String() = .TelefonbuchListe
                ' Erstelle ein neues Telefonbuch
                .ErstelleNeuesTelefonbuch(TelefonbuchName)
                ' Ermittle die neue ID des Telefonbuches
                Dim TelListeb As String() = .TelefonbuchListe

                If TelListeA.Count.AreDifferentTo(TelListeb.Count) Then
                    Dim TelListeC As List(Of String)
                    TelListeC = TelListeb.Except(TelListeA).ToList
                    TelefonbuchName = TelListeC.First
                End If

                ' Lade das Telefonbuch 
                Return Await .Telefonbuch(TelefonbuchName)
            End With
        End Using
    End Function

    Friend Sub LöscheTelefonbuch(TelefonbuchID As Integer)
        Using fboxSOAP As New FritzBoxTR64
            With fboxSOAP
                .LöscheTelefonbuch(TelefonbuchID)
            End With
        End Using
    End Sub

    <Extension> Private Function TelefonbuchListe(fboxSOAP As FritzBoxTR64) As String()
        Dim OutPutData As Hashtable

        ' Ermittle alle verfügbaren Telefonbücher
        OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "GetPhonebookList")

        If OutPutData.ContainsKey("NewPhonebookList") Then

            Return OutPutData.Item("NewPhonebookList").ToString.Split(",")
        Else
            Return Nothing
        End If

    End Function

    <Extension> Private Async Function Telefonbuch(fboxSOAP As FritzBoxTR64, TelefonbuchID As String) As Task(Of FritzBoxXMLTelefonbücher)
        Dim OutPutData As Hashtable
        Dim InPutData As Hashtable
        Dim PhoneBookXML As FritzBoxXMLTelefonbücher

        InPutData = New Hashtable From {{"NewPhonebookID", TelefonbuchID}}
        OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "GetPhonebook", InPutData)
        If OutPutData.ContainsKey("NewPhonebookURL") Then
            ' Deserialisiere das Telefonbuch
            PhoneBookXML = Await DeserializeObjectAsyc(Of FritzBoxXMLTelefonbücher)(OutPutData.Item("NewPhonebookURL").ToString())
            ' Setze die ID
            PhoneBookXML.Telefonbuch.ForEach(Sub(r) r.ID = TelefonbuchID)
            Return PhoneBookXML
        Else
            Return Nothing
        End If
    End Function


    ''' <summary>
    ''' Erstellt ein neues Telfonbuch
    ''' </summary>
    ''' <param name="TelefonbuchName">Der Name des Telefonbuches</param>
    <Extension> Private Function ErstelleNeuesTelefonbuch(fboxSOAP As FritzBoxTR64, TelefonbuchName As String) As Boolean
        Dim OutPutData As Hashtable
        Dim InPutData As Hashtable

        If TelefonbuchName.IsNotStringEmpty Then

            InPutData = New Hashtable From {{"NewPhonebookName", TelefonbuchName}, {"NewPhonebookExtraID", DfltStringEmpty}}
            OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "AddPhonebook", InPutData)

            ' Return code   Description         Related argument
            ' 402           Invalid arguments   Any
            ' 820           Internal Error
            If OutPutData.ContainsKey("Error") Then
                NLogger.Error(OutPutData.Item("Error"))
                Return False
            Else
                Return True
            End If
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Löscht das mit der <c>TelefonbuchID</c> angegebene Telefonbuch.
    ''' </summary>
    ''' <param name="TelefonbuchID">Number for a single phonebook.</param>
    ''' <remarks>The default phonebook (PhonebookID = 0) is not deletable, but therefore, each entry will
    ''' be deleted And the phonebook will be empty afterwards.</remarks>
    ''' <returns></returns>
    <Extension> Private Function LöscheTelefonbuch(fboxSOAP As FritzBoxTR64, TelefonbuchID As Integer) As Boolean
        Dim OutPutData As Hashtable
        Dim InPutData As Hashtable

        InPutData = New Hashtable From {{"NewPhonebookID", TelefonbuchID}, {"NewPhonebookExtraID", DfltStringEmpty}}
        OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "DeletePhonebook", InPutData)

        ' Return code   Description         Related argument
        ' 402           Invalid arguments   Any
        ' 713           Invalid array index Any input parameter
        ' 820           Internal Error
        If OutPutData.ContainsKey("Error") Then
            NLogger.Error(OutPutData.Item("Error"))
            Return False
        Else
            Return True
        End If

    End Function
#End Region

#Region "Aktionen für Telefonbucheinträge"
    ''' <summary>
    ''' Erstellt oder aktualisiert einen Telefonbucheintrag im mit der <c>TelefonbuchID</c> angegebene Telefonbuch.
    ''' </summary>
    ''' <param name="TelefonbuchID">Number for a single phonebook.</param>
    ''' <param name="XMLDaten">XML document with a single entry. </param>
    ''' <returns>The action returns the unique ID of the new or changed entry.</returns>
    Friend Function UpdateTelefonbucheintrag(TelefonbuchID As UInteger, XMLDaten As String) As Integer
        Dim OutPutData As Hashtable
        Dim InPutData As Hashtable

        If XMLDaten.IsNotStringEmpty Then
            Using fboxSOAP As New FritzBoxTR64

                ' SetPhonebookEntryUID
                ' Add a new or change an existing entry in a telephone book using the unique ID of the entry.
                ' Add new entry:
                '   set phonebook ID and XML entry data structure (without the unique ID tag)
                ' Change existing entry:
                '   set phonebook ID and XML entry data structure with the unique ID tag (e.g. <uniqueid>28</uniqueid>)
                ' The action returns the unique ID of the new or changed entry.

                InPutData = New Hashtable From {
                                                    {"NewPhonebookID", TelefonbuchID},
                                                    {"NewPhonebookEntryData", XMLDaten}
                                               }
                OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "SetPhonebookEntryUID", InPutData)

                ' Return code   Description         Related argument
                ' 402           Invalid arguments   Any
                ' 600           Argument invalid    PhonebookID
                ' 713           Invalid array index PhonebookID
                ' 820           Internal Error

                If OutPutData.ContainsKey("NewPhonebookEntryUniqueID") Then

                    Return CInt(OutPutData.Item("NewPhonebookEntryUniqueID"))
                Else
                    NLogger.Error("UpdateTelefonbucheintrag: {0}", OutPutData.Item("Error").ToString)
                    Return DfltIntErrorMinusOne
                End If
            End Using
        Else
            Return DfltIntErrorMinusOne
        End If
    End Function
    ''' <summary>
    ''' Delete an existing telephone book entry using the unique ID from the entry.
    ''' </summary>
    ''' <param name="TelefonbuchID">>Number for a single phonebook.</param>
    ''' <param name="UniqueID">Eindeutige ID des Kontaktes</param>
    Friend Function LöscheTelefonbucheintrag(TelefonbuchID As UInteger, UniqueID As Integer) As Boolean
        Dim OutPutData As Hashtable
        Dim InPutData As Hashtable

        Using fboxSOAP As New FritzBoxTR64

            InPutData = New Hashtable From {
                                                {"NewPhonebookID", TelefonbuchID},
                                                {"NewPhonebookEntryUniqueID", UniqueID}
                                           }
            OutPutData = fboxSOAP.TR064Start(Tr064Files.x_contactSCPD, "DeletePhonebookEntryUID", InPutData)

            ' Return code   Description         Related argument
            ' 402           Invalid arguments   Any
            ' 600           Argument invalid    PhonebookID
            ' 713           Invalid array index PhonebookID
            ' 820           Internal Error

            If OutPutData.ContainsKey("Error") Then
                NLogger.Error(OutPutData.Item("Error"))
                Return False
            Else
                Return True
            End If
        End Using

    End Function
#End Region
End Module

