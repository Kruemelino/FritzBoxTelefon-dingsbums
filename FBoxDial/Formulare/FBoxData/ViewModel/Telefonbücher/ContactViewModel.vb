Public Class ContactViewModel
    Inherits NotifyBase

#Region "Models"
    Public Property Kontakt As FBoxAPI.Contact
    Public Property KontaktKlone As FBoxAPI.Contact
#End Region

#Region "ViewModel"
    Public Property Person As PersonViewModel
    Public Property Telefonie As TelephonyViewModel
#End Region

#Region "Eigenschaften"
    Private _Kategorie As Integer
    ''' <summary>
    ''' Wichtige Person = 1, Optional, VIP == 1
    ''' </summary>
    Public Property Kategorie As Integer

        Get
            Return _Kategorie
        End Get
        Set
            SetProperty(_Kategorie, Value)
            Kontakt.Category = Value
        End Set
    End Property

    Private _Uniqueid As Integer

    Public Sub New(contact As FBoxAPI.Contact)
        _Kontakt = contact
        If Kontakt IsNot Nothing Then
            With Kontakt
                ' Setze Felder
                Kategorie = .Category
                Uniqueid = .Uniqueid

                Person = New PersonViewModel(.Person)
                Telefonie = New TelephonyViewModel(.Telephony)
            End With
        End If
    End Sub

    ''' <summary>
    ''' Unique ID for a single contact (new since 2013-04-20) 
    ''' </summary> 
    Public Property Uniqueid As Integer
        Get
            Return _Uniqueid
        End Get
        Set
            SetProperty(_Uniqueid, Value)
            Kontakt.Uniqueid = Value
        End Set
    End Property

#End Region
End Class