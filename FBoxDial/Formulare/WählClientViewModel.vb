Imports System.Windows
Imports System.Windows.Media.Imaging
Imports Microsoft.Office.Interop

Public Class WählClientViewModel
    Inherits NotifyBase

#Region "Felder"
    Private _Status As String
    Public Property Status As String
        Get
            Return _Status
        End Get
        Set
            SetProperty(_Status, Value)
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _CLIR As Boolean
    Public Property CLIR As Boolean
        Get
            Return _CLIR
        End Get
        Set
            SetProperty(_CLIR, Value)
        End Set
    End Property

    Private _CheckMobil As Boolean
    Public Property CheckMobil As Boolean
        Get
            Return _CheckMobil
        End Get
        Set
            SetProperty(_CheckMobil, Value)
        End Set
    End Property

    Private _IsCancelEnabled As Boolean
    Public Property IsCancelEnabled As Boolean
        Get
            Return _IsCancelEnabled
        End Get
        Set
            SetProperty(_IsCancelEnabled, Value)
        End Set
    End Property

    Private _IsDirektWahl As Boolean
    Public Property IsDirektWahl As Boolean
        Get
            Return _IsDirektWahl
        End Get
        Set
            SetProperty(_IsDirektWahl, Value)
        End Set
    End Property

    Private _OKontakt As Outlook.ContactItem
    Public Property OKontakt As Outlook.ContactItem
        Get
            Return _OKontakt
        End Get
        Set
            SetProperty(_OKontakt, Value)
        End Set
    End Property

    Private _OExchangeNutzer As Outlook.ExchangeUser
    Public Property OExchangeNutzer As Outlook.ExchangeUser
        Get
            Return _OExchangeNutzer
        End Get
        Set
            SetProperty(_OExchangeNutzer, Value)
        End Set
    End Property

    ''' <summary>
    ''' Returns Or sets a list as Telefonnummern             
    ''' </summary>
    Private _DialNumberList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialNumberList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialNumberList
        End Get
        Set
            SetProperty(_DialNumberList, Value)
        End Set
    End Property

    Private _DialDirektWahlList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialDirektWahlList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialDirektWahlList
        End Get
        Set
            SetProperty(_DialDirektWahlList, Value)
        End Set
    End Property

    Private _DialDeviceList As New ObservableCollectionEx(Of Telefoniegerät)
    Public Property DialDeviceList As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _DialDeviceList
        End Get
        Set
            SetProperty(_DialDeviceList, Value)
        End Set
    End Property

    Private _TelGerät As Telefoniegerät
    Public Property TelGerät As Telefoniegerät
        Get
            Return _TelGerät
        End Get
        Set
            SetProperty(_TelGerät, Value)
        End Set
    End Property

    Private _Kontaktbild As BitmapImage
    Public Property Kontaktbild As BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set
            SetProperty(_Kontaktbild, Value)
        End Set
    End Property

    Public ReadOnly Property KontaktbildVisibility As Visibility
        Get
            If Kontaktbild Is Nothing Then
                Return Visibility.Collapsed
            Else
                Return Visibility.Visible
            End If

        End Get
    End Property

    Private _TelNr As Telefonnummer
    ''' <summary>
    ''' Telefonnummer, die gewählt werden soll.
    ''' </summary>
    Public Property TelNr As Telefonnummer
        Get
            Return _TelNr
        End Get
        Set
            SetProperty(_TelNr, Value)
        End Set
    End Property
#End Region

    Friend Property Wählclient As FritzBoxWählClient

    ''' <summary>
    ''' Sammelt alle Kontaktdaten des Outlook-Kontaktes als <see cref="Outlook.ContactItem"/> zusammen.
    ''' </summary>
    Friend WriteOnly Property OutlookKontakt As Outlook.ContactItem
        Set
            ' Direktwahl Flag setzen
            IsDirektWahl = False

            ' Outlook Kontakt im ViewModel setzen
            OKontakt = Value

            ' Telefonnummern des Kontaktes setzen 
            DialNumberList.AddRange(GetKontaktTelNrList(Value))

            ' Kopfdaten setzen
            Name = WählClientFormText($"{Value.FullName}{If(Value.CompanyName.IsNotStringEmpty, $" ({Value.CompanyName})", DfltStringEmpty)}")

            ' Kontaktbild anzeigen
            Dim BildPfad As String

            BildPfad = KontaktFunktionen.KontaktBild(Value)

            If BildPfad.IsNotStringEmpty Then
                ' Kontaktbild laden
                Kontaktbild = New BitmapImage
                With Kontaktbild
                    .BeginInit()
                    .CacheOption = BitmapCacheOption.OnLoad
                    .UriSource = New Uri(BildPfad)
                    .EndInit()
                End With
                'Lösche das Kontaktbild 
                DelKontaktBild(BildPfad)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Sammelt alle Kontaktdaten des Outlook-ExchangeNutzers als <see cref="Outlook.ExchangeUser"/> zusammen.
    ''' </summary>
    Friend WriteOnly Property ExchangeKontakt As Outlook.ExchangeUser
        Set
            ' Direktwahl Flag setzen
            IsDirektWahl = False

            ' Outlook ExchangeNutzer im ViewModel setzen
            OExchangeNutzer = Value

            ' Telefonnummern des Kontaktes setzen 
            DialNumberList.AddRange(GetKontaktTelNrList(Value))

            ' Kopfdaten setzen
            Name = WählClientFormText($"{Value.Name}{If(Value.CompanyName.IsNotStringEmpty, $" ({Value.CompanyName})", DfltStringEmpty)}")
        End Set
    End Property

    Friend WriteOnly Property Telefonnummer As Telefonnummer
        Set
            ' Direktwahl Flag setzen
            IsDirektWahl = False

            ' Telefonnummer setzen 
            DialNumberList.Add(Value)

            ' Kopfdaten setzen
            Name = WählClientFormText(Value.Formatiert)
        End Set
    End Property

    Friend WriteOnly Property SetDirektwahl As Boolean
        Set
            ' Direktwahl Flag setzen
            IsDirektWahl = Value
            ' Kopfdaten setzen
            Name = WählClientFormText("Direktwahl")

            ' Wahlwiederhohlung in Combobox schreiben
            If XMLData.PTelListen.CALLListe IsNot Nothing AndAlso XMLData.PTelListen.CALLListe.Any Then
                DialDirektWahlList.AddRange(XMLData.PTelListen.GetTelNrList(XMLData.PTelListen.CALLListe))
            End If
        End Set

    End Property
End Class


