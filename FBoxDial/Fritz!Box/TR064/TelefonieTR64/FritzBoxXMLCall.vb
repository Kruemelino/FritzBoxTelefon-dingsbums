Imports System.Xml.Serialization
<Serializable(), XmlType("Call")> Public Class FritzBoxXMLCall
    Inherits NotifyBase

#Region "Fritz!Box Eigenschaften"

    Private _ID As Integer
    ''' <summary>
    ''' Unique ID per call. 
    ''' </summary>
    <XmlElement("Id", GetType(Integer))> Public Property ID As Integer
        Get
            Return _ID
        End Get
        Set
            SetProperty(_ID, Value)
        End Set
    End Property

    Private _Type As Integer
    ''' <summary>
    ''' 1 incoming,
    ''' 2 missed,
    ''' 3 outgoing,
    ''' 9 active incoming,
    ''' 10 rejected incoming,
    ''' 11 active outgoing 
    ''' </summary>
    <XmlElement("Type", GetType(Integer))> Public Property Type As Integer
        Get
            Return _Type
        End Get
        Set
            SetProperty(_Type, Value)
        End Set
    End Property

    Private _Called As String
    ''' <summary>
    ''' Number or name of called party 
    ''' </summary>
    <XmlElement("Called")> Public Property Called As String
        Get
            Return _Called
        End Get
        Set
            SetProperty(_Called, Value)
        End Set
    End Property

    Private _Caller As String
    ''' <summary>
    ''' Number of calling party 
    ''' </summary>
    <XmlElement("Caller")> Public Property Caller As String
        Get
            Return _Caller
        End Get
        Set
            SetProperty(_Caller, Value)
        End Set
    End Property

    Private _CalledNumber As String
    ''' <summary>
    ''' Own Number of called party (incoming call)
    ''' </summary>
    <XmlElement("CalledNumber")> Public Property CalledNumber As String
        Get
            Return _CalledNumber
        End Get
        Set
            SetProperty(_CalledNumber, Value)
        End Set
    End Property

    Private _CallerNumber As String
    ''' <summary>
    ''' Own Number of called party (outgoing call) 
    ''' </summary>
    <XmlElement("CallerNumber")> Public Property CallerNumber As String
        Get
            Return _CallerNumber
        End Get
        Set
            SetProperty(_CallerNumber, Value)
        End Set
    End Property

    Private _Name As String
    ''' <summary>
    ''' Name of called/ called party (outgoing/ incoming call) 
    ''' </summary>
    <XmlElement("Name")> Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _Numbertype As String
    ''' <summary>
    ''' pots, isdn, sip, umts, '' 
    ''' </summary>
    <XmlElement("Numbertype")> Public Property Numbertype As String
        Get
            Return _Numbertype
        End Get
        Set
            SetProperty(_Numbertype, Value)
        End Set
    End Property

    Private _Device As String
    ''' <summary>
    ''' Name of used telephone port. 
    ''' </summary>
    <XmlElement("Device")> Public Property Device As String
        Get
            Return _Device
        End Get
        Set
            SetProperty(_Device, Value)
        End Set
    End Property

    Private _Port As Integer
    ''' <summary>
    ''' Number of telephone port. 
    ''' </summary>
    ''' <remarks>    
    ''' To differ between voice calls, fax calls and TAM calls use the Port value.
    ''' E.g. if port equals 5 it Is a fax call. If port equals 6 Or port in in the rage of 40 to 49 it Is a TAM call.
    ''' </remarks>
    <XmlElement("Port")> Public Property Port As Integer
        Get
            Return _Port
        End Get
        Set
            SetProperty(_Port, Value)
        End Set
    End Property

    Private _XMLDate As String
    ''' <summary>
    ''' 31.07.12 12:03
    ''' </summary>
    <XmlElement("Date")> Public Property XMLDate As String
        Get
            Return _XMLDate
        End Get
        Set
            SetProperty(_XMLDate, Value)
        End Set
    End Property

    Private _XMLDuration As String
    ''' <summary>
    ''' hh:mm (minutes rounded up) 
    ''' </summary>
    <XmlElement("Duration")> Public Property XMLDuration As String
        Get
            Return _XMLDuration
        End Get
        Set
            SetProperty(_XMLDuration, Value)
        End Set
    End Property

    Private _Count As String
    <XmlElement("Count")> Public Property Count As String
        Get
            Return _Count
        End Get
        Set
            SetProperty(_Count, Value)
        End Set
    End Property

    Private _Path As String
    ''' <summary>
    '''  A call list may contain URLs for telephone answering machine messages or fax messages.
    '''  The content can be downloaded ising the protocol, hostname and port with the path URL.<br/>
    '''  An example is described here:<br/>
    '''  Protocol: https
    '''  Hostname: fritz.box
    '''  Port: 49443
    '''  path URL :  /download.lua?path=/var/media/ftp/USB/FRITZ/voicebox/rec/rec.0.000
    '''  The combination of<br/>
    '''  Protocoll + :// + Hostname + : + Port + path URL<br/>
    '''  will be the complete URL<br/>
    '''  https://fritz.box:49443/download.lua?path=/var/media/ftp/USB/FRITZ/voicebox/rec/rec.0.000<br/>
    '''  Please note, that this URL might require authentication. 
    ''' </summary>
    ''' <returns>URL path to TAM or FAX file.</returns>
    <XmlElement("Path")> Public Property Path As String
        Get
            Return _Path
        End Get
        Set
            SetProperty(_Path, Value)
        End Set
    End Property

#End Region

#Region "Eigene Eigenschaften"
    Private _Export As Boolean
    <XmlIgnore> Public Property Export As Boolean
        Get
            Return _Export
        End Get
        Set
            SetProperty(_Export, Value)
        End Set
    End Property

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

    <XmlIgnore> Public ReadOnly Property EigeneNummer As String
        Get
            Return String.Format("{0}{1}", CalledNumber, CallerNumber)
        End Get
    End Property

    <XmlIgnore> Public ReadOnly Property Datum As Date
        Get
            Return CDate(XMLDate.ToString)
        End Get
    End Property
    <XmlIgnore> Public ReadOnly Property Duration As TimeSpan
        Get
            With CDate(XMLDuration)
                Return New TimeSpan(.Hour, .Minute, .Second)
            End With
        End Get
    End Property
#End Region

    Friend Function ErstelleTelefonat() As Telefonat

        If Type.IsLessOrEqual(3) Then

            Dim tmpTelefonat As New Telefonat With {.Import = True,
                                                    .ID = ID,
                                                    .ZeitBeginn = Datum}
            Dim tmpTelNr As Telefonnummer

            With tmpTelefonat

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
                    .NrUnterdrückt = .GegenstelleTelNr.Unterdrückt

                    ' Ring-List
                    If XMLData.POptionen.CBAnrListeUpdateCallLists Then
                        ' RING-Liste initialisieren, falls erforderlich
                        If XMLData.PTelListen.RINGListe Is Nothing Then XMLData.PTelListen.RINGListe = New List(Of Telefonat)
                        ' Eintrag anfügen
                        XMLData.PTelListen.RINGListe.Insert(tmpTelefonat)
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
                        If XMLData.PTelListen.CALLListe Is Nothing Then XMLData.PTelListen.CALLListe = New List(Of Telefonat)
                        ' Eintrag anfügen
                        XMLData.PTelListen.CALLListe.Insert(tmpTelefonat)
                    End If
                End If

                If Type.AreEqual(1) Or Type.AreEqual(2) Or Type.AreEqual(3) Then
                    '.Aktiv = False
                    ' Anrufer ermitteln
                    If Name.IsNotStringNothingOrEmpty Then .AnruferName = Name

                    If .GegenstelleTelNr IsNot Nothing AndAlso Not .GegenstelleTelNr.Unterdrückt Then
                        .Kontaktsuche()
                    End If
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
