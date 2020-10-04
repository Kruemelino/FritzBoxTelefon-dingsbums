Imports System.Drawing
Imports System.Threading
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media.Imaging
Imports Microsoft.Office.Interop

Public Class WählclientWPF
    Inherits Window

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Lade initale Daten
        SetTelefonDaten()

    End Sub


    Friend Sub SetOutlookKontakt(ByVal oContact As Outlook.ContactItem)

        With CType(DataContext, WählClientViewModel)
            .OKontakt = oContact
            .DialNumberList.AddRange(GetKontaktTelNrList(oContact))
            .Name = PWählClientFormText($"{oContact.FullName}{If(oContact.CompanyName.IsNotStringEmpty, $" ({oContact.CompanyName})", PDfltStringEmpty)}")
            ' Kontaktbild anzeigen
            Dim BildPfad As String

            BildPfad = KontaktBild(oContact)

            If BildPfad.IsNotStringEmpty Then
                ' Bild einblenden
                AnrBild.Visibility = Visibility.Visible
                ' Kontaktbild laden
                .Kontaktbild = New BitmapImage
                With .Kontaktbild
                    .BeginInit()
                    .CacheOption = BitmapCacheOption.OnLoad
                    .UriSource = New Uri(BildPfad)
                    .EndInit()
                End With
                'Lösche das Kontaktbild 
                DelKontaktBild(BildPfad)
            End If
        End With
    End Sub

    Friend Sub SetOutlookKontakt(ByVal oExchangeUser As Outlook.ExchangeUser)
        With CType(DataContext, WählClientViewModel)
            .OExchangeNutzer = oExchangeUser
            .DialNumberList.AddRange(GetKontaktTelNrList(oExchangeUser))
            .Name = PWählClientFormText($"{oExchangeUser.Name}{If(oExchangeUser.CompanyName.IsNotStringEmpty, $" ({oExchangeUser.CompanyName})", PDfltStringEmpty)}")
        End With

    End Sub

    Friend Sub SetTelefonnummer(ByVal TelNr As Telefonnummer)
        ' Die Telefonnummer dem Datenobjekt zuweisen
        With CType(DataContext, WählClientViewModel)
            .DialNumberList.Add(TelNr)
            .Name = PWählClientFormText(TelNr.Formatiert)
        End With
    End Sub

    Private Sub SetTelefonDaten()
        With CType(DataContext, WählClientViewModel)
            ' Schreibe alle geeigneten Telefone rein (kein Fax, keine IP-Telefonie, keine AB)
            If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then
                ' Nur FON, DECT, S0 und Phoner, wenn Phoner aktiv
                .DialDeviceList.AddRange(XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) Not TG.IsFax And (TG.TelTyp = DfltWerteTelefonie.TelTypen.FON Or TG.TelTyp = DfltWerteTelefonie.TelTypen.DECT Or TG.TelTyp = DfltWerteTelefonie.TelTypen.S0)).ToList)
                ' XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) Not TG.IsFax And (TG.TelTyp = DfltWerteTelefonie.TelTypen.FON Or TG.TelTyp = DfltWerteTelefonie.TelTypen.DECT Or TG.TelTyp = DfltWerteTelefonie.TelTypen.S0 Or (TG.IsPhoner And PhonerApp IsNot Nothing))).ToList

                ' Ausgewähltes Standardgerät
                .StdGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.StdTelefon)
                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das zuletzt genutzte Telefon
                If .StdGerät Is Nothing Then .StdGerät = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.ZuletztGenutzt)
                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das erste in der Liste
                If .StdGerät Is Nothing And .DialDeviceList.Count.IsNotZero Then .StdGerät = .DialDeviceList.First
            End If
        End With
    End Sub



#Region "Eigenschaften"
    Private Property ScaleFaktor As SizeF = GetScaling()

#End Region

#Region "Botton"
    Private Sub BOptionen_MouseEnter(sender As Object, e As MouseEventArgs)
        OptionPopup.StaysOpen = True
    End Sub

    Private Sub BOptionen_MouseLeave(sender As Object, e As MouseEventArgs)
        OptionPopup.StaysOpen = False
    End Sub

    Private Sub BContact_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub DGNummern_LoadingRow(sender As Object, e As Controls.DataGridRowEventArgs) Handles DGNummern.LoadingRow
        e.Row.Header = e.Row.GetIndex
    End Sub

    Private Sub DGNummern_SelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs) Handles DGNummern.SelectionChanged

    End Sub
#End Region

End Class
