Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLCall
    ''' <summary>
    ''' Unique ID per call. 
    ''' </summary>
    <XmlElement("Id", GetType(Integer))> Public Property ID As Integer
    ''' <summary>
    ''' 1 incoming,
    ''' 2 missed,
    ''' 3 outgoing,
    ''' 9 active incoming,
    ''' 10 rejected incoming,
    ''' 11 active outgoing 
    ''' </summary>
    <XmlElement("Type", GetType(Integer))> Public Property Type As Integer

    ''' <summary>
    ''' Number or name of called party 
    ''' </summary>
    <XmlElement("Called")> Public Property Called As String

    ''' <summary>
    ''' Number of calling party 
    ''' </summary>
    <XmlElement("Caller")> Public Property Caller As String

    ''' <summary>
    ''' Gibt die Gegenstellennummer (ferne Nummer, NICHT die eigene Nummer) zurück.
    ''' Es wird je nach Telefonatstyp <c>Type</c> unterschschieden werden.    ''' 
    ''' </summary>
    <XmlIgnore> Public ReadOnly Property Gegenstelle As String
        Get
            Select Case Type
                Case 1, 2, 9, 10
                    Return Caller
                Case Else '3, 11
                    Return Called
            End Select
        End Get
    End Property

    ''' <summary>
    ''' Own Number of called party (incoming call)
    ''' </summary>
    <XmlElement("CalledNumber")> Public Property CalledNumber As String

    ''' <summary>
    ''' Own Number of called party (outgoing call) 
    ''' </summary>
    <XmlElement("CallerNumber")> Public Property CallerNumber As String

    <XmlIgnore> Public ReadOnly Property EigeneNummer As String
        Get
            Return String.Format("{0}{1}", CalledNumber, CallerNumber)
        End Get
    End Property

    ''' <summary>
    ''' Name of called/ called party (outgoing/ incoming call) 
    ''' </summary>
    <XmlElement("Name")> Public Property Name As String

    ''' <summary>
    ''' pots, isdn, sip, umts, '' 
    ''' </summary>
    <XmlElement("Numbertype")> Public Property Numbertype As String

    ''' <summary>
    ''' Name of used telephone port. 
    ''' </summary>
    <XmlElement("Device")> Public Property Device As String

    ''' <summary>
    ''' Number of telephone port. 
    ''' </summary>
    ''' <remarks>    
    ''' To differ between voice calls, fax calls and TAM calls use the Port value.
    ''' E.g. if port equals 5 it Is a fax call. If port equals 6 Or port in in the rage of 40 to 49 it Is a TAM call.
    ''' </remarks>
    <XmlElement("Port")> Public Property Port As Integer

    ''' <summary>
    ''' 31.07.12 12:03
    ''' </summary>
    <XmlElement("Date")> Public Property XMLDate As String
    <XmlIgnore> Public ReadOnly Property Datum As Date
        Get
            Return CDate(XMLDate.ToString)
        End Get
    End Property

    ''' <summary>
    ''' hh:mm (minutes rounded up) 
    ''' </summary>
    <XmlElement("Duration")> Public Property XMLDuration As String
    <XmlIgnore> Public ReadOnly Property Duration As TimeSpan
        Get
            With CDate(XMLDuration)
                Return New TimeSpan(.Hour, .Minute, .Second)
            End With
        End Get
    End Property
    <XmlElement("Count")> Public Property Count As String

    ''' <summary>
    '''  URL path to TAM or FAX file. 
    ''' </summary>
    <XmlElement("Path")> Public Property Path As String

    <XmlIgnore> Public Property Check As Boolean

    Friend Function ErstelleTelefonat() As Telefonat

        If Type.IsLessOrEqual(3) Then

            Dim tmpTelefonat As New Telefonat
            Dim tmpTelNr As Telefonnummer

            With tmpTelefonat
                .ID = ID
                .ZeitBeginn = Datum

                If Type.AreEqual(1) Or Type.AreEqual(3) Then ' incoming, outgoing
                    ' Testweise wird auch nach dem Namen des Gerätes gesucht, wenn über den Port nichts gefunden wurde.
                    .TelGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.AnrMonID = Port OrElse TG.Name = Device)
                    ' Umwandlung von "hh:mm" in Sekundenwert
                    .Dauer = Duration.TotalSeconds.ToInt

                    .Angenommen = .Dauer.IsNotZero
                End If

                If Type.AreEqual(1) Or Type.AreEqual(2) Then ' incoming, missed
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Eingehend
                    ' Own Number of called party (incoming call)
                    tmpTelNr = New Telefonnummer With {.SetNummer = CalledNumber}
                    .EigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(Tel) Tel.Equals(tmpTelNr))
                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert
                    ' Number of calling party 
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = Caller}
                    .NrUnterdrückt = .GegenstelleTelNr.Unbekannt
                    ' Ring-List
                    If XMLData.POptionen.CBAnrListeUpdateCallLists Then
                        ' RING-Liste initialisieren, falls erforderlich
                        If XMLData.PTelefonie.RINGListe Is Nothing Then XMLData.PTelefonie.RINGListe = New List(Of Telefonat)
                        ' Eintrag anfügen
                        XMLData.PTelefonie.RINGListe.Insert(tmpTelefonat)
                    End If
                End If

                If Type.AreEqual(3) Then 'outgoing
                    .AnrufRichtung = Telefonat.AnrufRichtungen.Ausgehend
                    ' Own Number of called party (outgoing call) 
                    tmpTelNr = New Telefonnummer With {.SetNummer = CallerNumber}
                    .EigeneTelNr = XMLData.PTelefonie.Telefonnummern.Find(Function(Tel) Tel.Equals(tmpTelNr))
                    ' Wert für Serialisierung in separater Eigenschaft ablegen
                    .OutEigeneTelNr = .EigeneTelNr.Unformatiert
                    ' Number or name of called party  
                    .GegenstelleTelNr = New Telefonnummer With {.SetNummer = Called}
                    ' Call-List
                    If XMLData.POptionen.CBAnrListeUpdateCallLists Then
                        ' CALL-Liste initialisieren, falls erforderlich
                        If XMLData.PTelefonie.CALLListe Is Nothing Then XMLData.PTelefonie.CALLListe = New List(Of Telefonat)

                        ' Eintrag anfügen
                        XMLData.PTelefonie.CALLListe.Insert(tmpTelefonat)
                    End If
                End If

                If Type.AreEqual(1) Or Type.AreEqual(2) Or Type.AreEqual(3) Then
                    .Aktiv = False
                    ' Anrufer ermitteln
                    If Name.IsNotStringNothingOrEmpty Then .Anrufer = Name

                    If .GegenstelleTelNr IsNot Nothing AndAlso Not .GegenstelleTelNr.Unbekannt Then .StarteKontaktsuche()
                End If

                If Type.AreEqual(2) Then .Angenommen = False ' missed

                If Type.AreEqual(9) Or Type.AreEqual(10) Or Type.AreEqual(11) Then
                    ' 9 active incoming,
                    ' 10 rejected incoming,
                    ' 11 active outgoing 

                    ' Hier könnte mal erfasst werden, was mit aktiven Gesprächen geschehen soll. 
                End If
            End With

            Return tmpTelefonat
        Else
            Return Nothing
        End If

    End Function


End Class
