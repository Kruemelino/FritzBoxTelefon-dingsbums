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
        Set(value As String)
            SetProperty(_Status, value)
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            SetProperty(_Name, value)
        End Set
    End Property

    Private _CLIR As Boolean
    Public Property CLIR As Boolean
        Get
            Return _CLIR
        End Get
        Set(value As Boolean)
            SetProperty(_CLIR, value)
        End Set
    End Property

    Private _OKontakt As Outlook.ContactItem
    Public Property OKontakt As Outlook.ContactItem
        Get
            Return _OKontakt
        End Get
        Set(value As Outlook.ContactItem)
            SetProperty(_OKontakt, value)
        End Set
    End Property

    Private _OExchangeNutzer As Outlook.ExchangeUser
    Public Property OExchangeNutzer As Outlook.ExchangeUser
        Get
            Return _OExchangeNutzer
        End Get
        Set(value As Outlook.ExchangeUser)
            SetProperty(_OExchangeNutzer, value)
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
        Set(value As ObservableCollectionEx(Of Telefonnummer))
            SetProperty(_DialNumberList, value)
        End Set
    End Property

    Private _DialDirektWahlList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialDirektWahlList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialDirektWahlList
        End Get
        Set(value As ObservableCollectionEx(Of Telefonnummer))
            SetProperty(_DialDirektWahlList, value)
        End Set
    End Property

    Private _DialDeviceList As New ObservableCollectionEx(Of Telefoniegerät)
    Public Property DialDeviceList As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _DialDeviceList
        End Get
        Set(value As ObservableCollectionEx(Of Telefoniegerät))
            SetProperty(_DialDeviceList, value)
        End Set
    End Property

    Private _TelGerät As Telefoniegerät
    Public Property TelGerät As Telefoniegerät
        Get
            Return _TelGerät
        End Get
        Set(value As Telefoniegerät)
            SetProperty(_TelGerät, value)
        End Set
    End Property

    Private _Kontaktbild As BitmapImage
    Public Property Kontaktbild As BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set(value As BitmapImage)
            SetProperty(_Kontaktbild, value)
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
    Public Sub New()

    End Sub

    Public Sub New(FBWählclient As FritzBoxWählClient)
        Wählclient = FBWählclient
    End Sub

    ''' <summary>
    ''' Sammelt alle Kontaktdaten des Outlook-Kontaktes als <see cref="Outlook.ContactItem"/> zusammen.
    ''' </summary>
    ''' <param name="oContact">Outlook Kontakt, der eingeblendet werden soll.</param>
    Friend Sub SetOutlookKontakt(oContact As Outlook.ContactItem)


        ' Outlook Kontakt im ViewModel setzen
        OKontakt = oContact

        ' Telefonnummern des Kontaktes setzen 
        DialNumberList.AddRange(GetKontaktTelNrList(oContact))

        ' Kopfdaten setzen
        Name = WählClientFormText($"{oContact.FullName}{If(oContact.CompanyName.IsNotStringEmpty, $" ({oContact.CompanyName})", DfltStringEmpty)}")

        ' Kontaktbild anzeigen
        Dim BildPfad As String

        BildPfad = KontaktFunktionen.KontaktBild(oContact)

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

    End Sub

    ''' <summary>
    ''' Sammelt alle Kontaktdaten des Outlook-ExchangeNutzers als <see cref="Outlook.ExchangeUser"/> zusammen.
    ''' </summary>
    ''' <param name="oExchangeUser">Outlook-ExchangeNutzers, der eingeblendet werden soll.</param>
    Friend Sub SetOutlookKontakt(oExchangeUser As Outlook.ExchangeUser)

        ' Outlook ExchangeNutzer im ViewModel setzen
        OExchangeNutzer = oExchangeUser

        ' Telefonnummern des Kontaktes setzen 
        DialNumberList.AddRange(GetKontaktTelNrList(oExchangeUser))

        ' Kopfdaten setzen
        Name = WählClientFormText($"{oExchangeUser.Name}{If(oExchangeUser.CompanyName.IsNotStringEmpty, $" ({oExchangeUser.CompanyName})", DfltStringEmpty)}")

    End Sub

    Friend Sub SetTelefonnummer(TelNr As Telefonnummer)

        ' Telefonnummer setzen 
        DialNumberList.Add(TelNr)

        ' Kopfdaten setzen
        Name = WählClientFormText(TelNr.Formatiert)

    End Sub

    Friend Sub SetDirektwahl()

        ' Kopfdaten setzen
        Name = WählClientFormText("Direktwahl")

        ' Wahlwiederhohlung in Combobox schreiben
        If XMLData.PTelefonie.CALLListe IsNot Nothing AndAlso XMLData.PTelefonie.CALLListe.Any Then
            DialDirektWahlList.AddRange(XMLData.PTelefonie.GetTelNrList(XMLData.PTelefonie.CALLListe))
        End If

    End Sub
End Class


