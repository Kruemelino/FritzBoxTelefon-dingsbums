Imports System.Collections.ObjectModel

Public NotInheritable Class DfltWerteTelefonie
    ''' <summary>
    ''' Verkehrsausscheidungsziffer "00"
    ''' </summary>
    Friend Shared ReadOnly Property PDfltVAZ As String = "00"
    Friend Shared ReadOnly Property PDfltAmt As String = "0"

    ''' <summary>
    ''' Italienische Mobilvorwahlen
    ''' </summary>
    Friend Shared ReadOnly Property PDfltMobilIt() As ReadOnlyCollection(Of String)
        Get
            Return New ReadOnlyCollection(Of String)(New List(Of String) From {
                "330", "331", "332", "333", "334", "335", "336", "337", "338", "339",
                "360", "361", "362", "363", "364", "365", "366", "367", "368", "390",
                "391", "392", "393", "340", "341", "342", "343", "344", "345", "346",
                "347", "348", "349", "380", "381", "382", "383", "384", "385", "386",
                "387", "388", "389", "320", "321", "322", "323", "324", "325", "326",
                "327", "328", "329"})
        End Get
    End Property

#Region "Enumeration"
    Friend Enum AnrMonTelIDBase As Integer
        ''' <summary>
        ''' Anrufmonitor: 0, 1, 2 für FON1, FON2, FON3
        ''' </summary>
        FON = 0

        ''' <summary>
        ''' Durchwahl
        ''' </summary>
        Durchwahl = 3

        ''' <summary>
        ''' S0 ISDN Telefon
        ''' </summary>
        S0 = 4

        ''' <summary>
        ''' PC/Fax, internes Fax
        ''' </summary>
        Fax = 5

        ''' <summary>
        ''' Aus der Dokumentation der Anrufliste:<br/>
        ''' If port equals 6 or port in in the rage of 40 to 49 it is a TAM call.
        ''' </summary>
        OldTAM = 6

        ''' <summary>
        ''' Anrufmonitor: 10, 11, 12, 13, 14, 15
        ''' </summary>
        DECT = 10

        ''' <summary>
        ''' Anrufmonitor für IP-Telefone: 20, 21, 22, 23, 24, 25, ...
        ''' </summary>
        IP = 20

        ''' <summary>
        ''' Anrufmonitor für integrierten Anrufbeantworter: 40, 41, 42, ..., 49
        ''' </summary>
        TAM = 40

        ''' <summary>
        ''' Data S0
        ''' </summary>
        DataS0 = 36

        ''' <summary>
        ''' Data PC
        ''' </summary>
        DataPC = 37

        Mobil = 99
    End Enum

    Friend Enum InternBase As Integer
        FON = 0
        DECT = 610
        IP = 620
        S0 = 50
        TAM = 600
    End Enum

    Public Enum TelTypen As Integer
        FON
        DECT
        IP
        ISDN
        FAX
        Mobil
        POTS
        TAM
        DATA
        CallThrough
    End Enum
#End Region

End Class