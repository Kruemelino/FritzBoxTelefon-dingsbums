Imports System.Collections.ObjectModel

Public NotInheritable Class DfltWerteTelefonie
    ''' <summary>
    ''' Verkehrsausscheidungsziffer "00"
    ''' </summary>
    Friend Shared ReadOnly Property PDfltVAZ As String = "00"
    Friend Shared ReadOnly Property PDfltAmt As String = "0"
    Friend Shared ReadOnly Property PDfltMaskeNANP As String = "%L (%O) %N-%D"

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
        FON = 1
        Fax = 5
        DECT = 10
        IP = 20
        TAM = 40
        S0 = 50
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
        FON = 1
        DECT = 2
        IP = 4
        ISDN = 8
        FAX = 16
        Mobil = 32
        POTS = 64
        MSN = 128
        TAM = 256
        SIP = 512
    End Enum
#End Region

End Class