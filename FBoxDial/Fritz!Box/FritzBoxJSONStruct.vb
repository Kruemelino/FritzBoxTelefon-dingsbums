﻿Imports Newtonsoft.Json

#Region "FON"
Friend Structure MSNEntry
    <JsonProperty("_Node")> Public Property Node As String
    Public Property AllIncomingCalls As Boolean
    Public Property Name As String
    Public Property Fax As Boolean
End Structure

Friend Structure FBoxFONNr
    Public Property MSN0 As String
    Public Property MSN1 As String
    Public Property MSN2 As String
    Public Property MSN3 As String
    Public Property MSN4 As String
    Public Property MSN5 As String
    Public Property MSN6 As String
    Public Property MSN7 As String
    Public Property MSN8 As String
    Public Property MSN9 As String

    Friend ReadOnly Property MSNList As List(Of String)
        Get
            Return {MSN0, MSN1, MSN2, MSN3, MSN4, MSN5, MSN6, MSN7, MSN8, MSN9}.ToList
        End Get
    End Property

End Structure
#End Region

#Region "DECT"
Friend Structure DECTEntry
    Public Property Name As String
    Public Property Intern As String
    Public Property Id As Integer
End Structure

Friend Structure DECTNr
    Public Property Number As String
End Structure

Friend Structure FBoxDECTNr
    Public Property DECTNr As List(Of DECTNr)
    Public Property DECTRingOnAllMSNs As Boolean

End Structure
#End Region

#Region "S0"
Friend Structure FBoxS0
    Public Property S0Name As String
    Public Property S0Number As String
    Public Property S0Type As String
End Structure
#End Region

#Region "FaxMail, Mobil"
Friend Structure FaxMailMobil
    Public Property FaxMailActive As Boolean
    Public Property MobileName As String
    Public Property Mobile As String
End Structure

Friend Structure FBoxFaxNr
    Public Property FAX0 As String
    Public Property FAX1 As String
    Public Property FAX2 As String
    Public Property FAX3 As String
    Public Property FAX4 As String
    Public Property FAX5 As String
    Public Property FAX6 As String
    Public Property FAX7 As String
    Public Property FAX8 As String
    Public Property FAX9 As String

    Friend ReadOnly Property FAXList As List(Of String)
        Get
            Return {FAX0, FAX1, FAX2, FAX3, FAX4, FAX5, FAX6, FAX7, FAX8, FAX9}.ToList
        End Get
    End Property

End Structure

#End Region

