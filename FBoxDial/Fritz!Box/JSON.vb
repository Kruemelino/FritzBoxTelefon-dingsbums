Imports Newtonsoft.Json
#Region "Definition Klassen"
Public Class TAMEntry
    <JsonProperty("_Node")> Public Property Node() As String
    Public Property Active() As String
    Public Property Name() As String
    Public Property Display() As String
    Public Property MSNBitmap() As String
    Public Property NumNewMessages() As String
    Public Property NumOldMessages() As String
End Class

Public Class SIPEntry
    <JsonProperty("_Node")> Public Property Node() As String
    Public Property Activated() As String
    Public Property Displayname() As String
    Public Property ID() As String
    Public Property Gui_readonly() As String
    Public Property Webui_trunk_id() As String
End Class

Public Class MSNEntry
    <JsonProperty("_Node")> Public Property Node() As String
    Public Property Name() As String
    Public Property Fax() As String
    Public Property GroupCall() As String
    Public Property AllIncomingCalls() As String
    Public Property OutDialing() As String
End Class

Public Class VOIPEntry
    <JsonProperty("_Node")> Public Property Node() As String
    Public Property Enabled() As String
    Public Property Name() As String
    Public Property RingOnAllMSNs() As String
End Class

Friend Class FoncontrolUserList
    <JsonProperty("_Node")> Public Property Node() As String
    Friend Property Name() As String
    Friend Property Type() As String
    Friend Property Intern() As String
    Friend Property Id() As String
End Class

Public Class DECTNr
    Public Property Number() As String
End Class

Public Class DECTEntry
    <JsonProperty("_Node")> Public Property Node() As String
    Public Property Name() As String
    Public Property Intern() As String
    Public Property Id() As String
End Class

Public Class FritzBoxJSONTelefone1
    Public Property S0Name1() As String
    Public Property S0Name2() As String
    Public Property S0Name3() As String
    Public Property S0Name4() As String
    Public Property S0Name5() As String
    Public Property S0Name6() As String
    Public Property S0Name7() As String
    Public Property S0Name8() As String
    Public Property TAM As TAMEntry()
    Public Property DECT As DECTEntry()
    Public Property FON As MSNEntry()
    Public Property VOIP As VOIPEntry()

    Public ReadOnly Property S0NameList As String()
        Get
            Dim tmp() As String = {S0Name1, S0Name2, S0Name3, S0Name4, S0Name5, S0Name6, S0Name7, S0Name8}
            Return tmp
        End Get
    End Property
End Class

Public Class FritzBoxJSONTelefone2
#Region "S0"
    Public Property S0Number1() As String
    Public Property S0Number2() As String
    Public Property S0Number3() As String
    Public Property S0Number4() As String
    Public Property S0Number5() As String
    Public Property S0Number6() As String
    Public Property S0Number7() As String
    Public Property S0Number8() As String
    Public Property S0Type1() As String
    Public Property S0Type2() As String
    Public Property S0Type3() As String
    Public Property S0Type4() As String
    Public Property S0Type5() As String
    Public Property S0Type6() As String
    Public Property S0Type7() As String
    Public Property S0Type8() As String

    Public ReadOnly Property S0NumberList As String()
        Get
            Dim tmp() As String = {S0Number1, S0Number2, S0Number3, S0Number4, S0Number5, S0Number6, S0Number7, S0Number8}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property S0TypeList As String()
        Get
            Dim tmp() As String = {S0Type1, S0Type2, S0Type3, S0Type4, S0Type5, S0Type6, S0Type7, S0Type8}
            Return tmp
        End Get
    End Property
#End Region

#Region "DECT"
    Public Property DECT0Nr() As DECTNr()

    Public Property DECT1Nr() As DECTNr()

    Public Property DECT2Nr() As DECTNr()

    Public Property DECT3Nr() As DECTNr()

    Public Property DECT4Nr() As DECTNr()

    Public Property DECT5Nr() As DECTNr()

    Public Property DECT6Nr() As DECTNr()

    Public Property DECT7Nr() As DECTNr()

    Public Property DECT8Nr() As DECTNr()

    Public Property DECT9Nr() As DECTNr()

    Public Property DECT10Nr() As DECTNr()

    Public Property DECT11Nr() As DECTNr()

    Public Property DECT0RingOnAllMSNs() As String

    Public Property DECT1RingOnAllMSNs() As String

    Public Property DECT2RingOnAllMSNs() As String

    Public Property DECT3RingOnAllMSNs() As String

    Public Property DECT4RingOnAllMSNs() As String

    Public Property DECT5RingOnAllMSNs() As String

    Public Property DECT6RingOnAllMSNs() As String

    Public Property DECT7RingOnAllMSNs() As String

    Public Property DECT8RingOnAllMSNs() As String

    Public Property DECT9RingOnAllMSNs() As String

    Public Property DECT10RingOnAllMSNs() As String

    Public Property DECT11RingOnAllMSNs() As String

    Public ReadOnly Property DECTRingOnAllMSNs As String()
        Get
            Dim tmp() As String = {DECT0RingOnAllMSNs, DECT1RingOnAllMSNs, DECT2RingOnAllMSNs, DECT3RingOnAllMSNs, DECT4RingOnAllMSNs, DECT5RingOnAllMSNs, DECT6RingOnAllMSNs, DECT7RingOnAllMSNs, DECT8RingOnAllMSNs, DECT9RingOnAllMSNs, DECT10RingOnAllMSNs, DECT11RingOnAllMSNs}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property DECTTelNr As DECTNr()()
        Get
            Dim tmp As DECTNr()() = {DECT0Nr(), DECT1Nr(), DECT2Nr(), DECT3Nr(), DECT4Nr(), DECT5Nr(), DECT6Nr(), DECT7Nr(), DECT8Nr(), DECT9Nr(), DECT10Nr(), DECT11Nr()}
            Return tmp
        End Get
    End Property
#End Region
    Public Property FaxMailActive() As String
    Public Property MobileName() As String
End Class

Public Class FritzBoxJSONTelNrT1
    Public Property POTS() As String
    Public Property Mobile() As String
    Public Property Port0Name() As String
    Public Property Port1Name() As String
    Public Property Port2Name() As String
    Public Property TAM0() As String
    Public Property FAX0() As String
    Public Property MSN0() As String
    Public Property VOIP0Enabled() As String
    Public Property TAM1() As String
    Public Property FAX1() As String
    Public Property MSN1() As String
    Public Property VOIP1Enabled() As String
    Public Property TAM2() As String
    Public Property FAX2() As String
    Public Property MSN2() As String
    Public Property VOIP2Enabled() As String
    Public Property TAM3() As String
    Public Property FAX3() As String
    Public Property MSN3() As String
    Public Property VOIP3Enabled() As String
    Public Property TAM4() As String
    Public Property FAX4() As String
    Public Property MSN4() As String
    Public Property VOIP4Enabled() As String
    Public Property TAM5() As String
    Public Property FAX5() As String
    Public Property MSN5() As String
    Public Property VOIP5Enabled() As String
    Public Property TAM6() As String
    Public Property FAX6() As String
    Public Property MSN6() As String
    Public Property VOIP6Enabled() As String
    Public Property TAM7() As String
    Public Property FAX7() As String
    Public Property MSN7() As String
    Public Property VOIP7Enabled() As String
    Public Property TAM8() As String
    Public Property FAX8() As String
    Public Property MSN8() As String
    Public Property VOIP8Enabled() As String
    Public Property TAM9() As String
    Public Property FAX9() As String
    Public Property MSN9() As String
    Public Property VOIP9Enabled() As String
    Public Property SIP() As SIPEntry()

    Public ReadOnly Property TAMList As String()
        Get
            Dim tmp() As String = {TAM0, TAM1, TAM2, TAM3, TAM4, TAM5, TAM6, TAM7, TAM8, TAM9}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property MSNList As String()
        Get
            Dim tmp() As String = {MSN0, MSN1, MSN2, MSN3, MSN4, MSN5, MSN6, MSN7, MSN8, MSN9}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property FAXList As String()
        Get
            Dim tmp() As String = {FAX0, FAX1, FAX2, FAX3, FAX4, FAX5, FAX6, FAX7, FAX8, FAX9}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property MSNPortEnabled As String()
        Get
            Dim tmp() As String = {Port0Name, Port1Name, Port2Name}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property VOIPPortEnabled As String()
        Get
            Dim tmp() As String = {VOIP0Enabled, VOIP1Enabled, VOIP2Enabled, VOIP3Enabled, VOIP4Enabled, VOIP5Enabled, VOIP6Enabled, VOIP7Enabled, VOIP8Enabled, VOIP9Enabled}
            Return tmp
        End Get
    End Property
End Class

''' <summary>
''' Klasse einer Liste mit 10 Telefonnummernfelder
''' </summary>
Public Class TelNrList
    Public Property TelNr0() As String
    Public Property TelNr1() As String
    Public Property TelNr2() As String
    Public Property TelNr3() As String
    Public Property TelNr4() As String
    Public Property TelNr5() As String
    Public Property TelNr6() As String
    Public Property TelNr7() As String
    Public Property TelNr8() As String
    Public Property TelNr9() As String
    ''' <summary>
    ''' Gibt die Telefonnummern als Array zurück. Leere Felder und doppelte Werte werden nicht heraus gefiltert.
    ''' </summary>
    ''' <returns>String-Array</returns>
    Public Function ToArray() As String()
        Dim tmp() As String = {TelNr0, TelNr1, TelNr2, TelNr3, TelNr4, TelNr5, TelNr6, TelNr7, TelNr8, TelNr9}
        Return tmp
    End Function

    ''' <summary>
    ''' Gibt die Telefonnummern als Array zurück. Leere Felder und doppelte Werte werden heraus gefiltert.
    ''' </summary>
    ''' <returns>String-Array</returns>
    Public Function ToDistinctArray() As String()
        ' Doppelte entfernen
        ToDistinctArray = (From x In ToArray() Select x Distinct).ToArray
        ' Leere entfernen
        ToDistinctArray = (From x In ToDistinctArray Where Not x Like DfltStringEmpty Select x).ToArray
    End Function

    ''' <summary>
    ''' Gibt den niedrigsten verfügbaren Feldindex für die angegebene Dimension eines Arrays zurück.
    ''' </summary>
    ''' <returns>Integer. Der niedrigste Wert, den der Feldindex für die angegebene Dimension enthalten kann. 
    ''' LBound gibt stets 0 (null) zurück, sofern Array initialisiert wurde, auch wenn das Array keine Elemente enthält, beispielsweise wenn es eine Zeichenfolge mit der Länge 0 (null) ist.
    ''' Wenn Array den Wert Nothing hat, löst LBound eine ArgumentNullException-Ausnahme aus.</returns>
    Public ReadOnly Property LBound() As Integer
        Get
            Return ToArray.GetLowerBound(0)
        End Get
    End Property

    ''' <summary>
    ''' Gibt den höchsten verfügbaren Feldindex für die angegebene Dimension eines Arrays zurück.
    ''' </summary>
    ''' <returns>Integer. Der höchste Wert, den der Feldindex für die angegebene Dimension enthalten kann. Wenn Array nur ein Element enthält, gibt UBound 0 (null) zurück. Enthält Array keine Elemente, z. B. wenn es sich um eine Zeichenfolge mit der Länge 0 (null) handelt, dann gibt UBound -1 zurück.</returns>
    Public ReadOnly Property UBound() As Integer
        Get
            Return ToArray.GetUpperBound(0)
        End Get
    End Property

    Public Property Item(idx As Integer) As String
        Get
            Select Case idx
                Case 0
                    Item = TelNr0
                Case 1
                    Item = TelNr1
                Case 2
                    Item = TelNr2
                Case 3
                    Item = TelNr3
                Case 4
                    Item = TelNr4
                Case 5
                    Item = TelNr5
                Case 6
                    Item = TelNr6
                Case 7
                    Item = TelNr7
                Case 8
                    Item = TelNr8
                Case 9
                    Item = TelNr9
                Case Else
                    Item = DfltStringEmpty
            End Select
            Return Item
        End Get
        Set
            Select Case idx
                Case 0
                    TelNr0 = Value
                Case 1
                    TelNr1 = Value
                Case 2
                    TelNr2 = Value
                Case 3
                    TelNr3 = Value
                Case 4
                    TelNr4 = Value
                Case 5
                    TelNr5 = Value
                Case 6
                    TelNr6 = Value
                Case 7
                    TelNr7 = Value
                Case 8
                    TelNr8 = Value
                Case 9
                    TelNr9 = Value
            End Select
        End Set
    End Property


End Class

''' <summary>
''' Klasse für den Upload von Kontakten
''' </summary>
Public Class Tomark
End Class

''' <summary>
''' Klasse für den Upload von Kontakten
''' </summary>
Public Class FritzBoxJSONUploadResult
    Public Property Tomark() As Tomark()
    Public Property Validate() As String
    Public Property Result() As String
    Public Property Ok() As Boolean
End Class
#End Region

Public Class JSON
    Implements IDisposable

    Public Function GetFirstValues(strJSON As String) As FritzBoxJSONTelNrT1
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelNrT1)(strJSON)
    End Function

    Public Function GetSecondValues(strJSON As String) As FritzBoxJSONTelefone1
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelefone1)(strJSON)
    End Function

    Public Function GetThirdValues(strJSON As String) As FritzBoxJSONTelefone2
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelefone2)(strJSON)
    End Function

    Public Function GetTelNrListJSON(strJSON As String) As TelNrList
        Return JsonConvert.DeserializeObject(Of TelNrList)(strJSON)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

