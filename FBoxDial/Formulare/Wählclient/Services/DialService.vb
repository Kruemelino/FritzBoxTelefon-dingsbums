Imports System.Threading.Tasks

Public Class DialService
    Implements IDialService

    Private Property Wählclient As FritzBoxWählClient

    Friend Sub New(wc As FritzBoxWählClient)
        _Wählclient = wc
    End Sub

    Private Sub UpdateTheme() Implements IDialService.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub

    Private ReadOnly Property GetMobil As Boolean Implements IDialService.GetMobil
        Get
            Return XMLData.POptionen.CBCheckMobil
        End Get
    End Property

    Private ReadOnly Property GetCLIR As Boolean Implements IDialService.GetCLIR
        Get
            Return XMLData.POptionen.CBCLIR
        End Get
    End Property

    Private Function GetDialabePhones() As IEnumerable(Of Telefoniegerät) Implements IDialService.GetDialabePhones
        If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then
            Return XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) TG.IsDialable)
        Else
            Return Nothing
        End If

    End Function

    Private Function GetSelectedPhone() As Telefoniegerät Implements IDialService.GetSelectedPhone
        If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then

            ' Ausgewähltes Standardgerät
            GetSelectedPhone = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.StdTelefon)

            If GetSelectedPhone Is Nothing Then
                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das zuletzt genutzte Telefon
                GetSelectedPhone = XMLData.PTelefonie.GetTelefonByID(XMLData.POptionen.UsedTelefonID)
            End If

        Else
            Return Nothing
        End If
    End Function

    Private Function GetLastTelNr() As IEnumerable(Of Telefonnummer) Implements IDialService.GetLastTelNr
        If XMLData.PTelListen.CALLListe IsNot Nothing AndAlso XMLData.PTelListen.CALLListe.Any Then
            Return XMLData.PTelListen.GetTelNrList(XMLData.PTelListen.CALLListe)
        Else
            Return Nothing
        End If
    End Function

    Private Async Function DialTelNr(TelNr As Telefonnummer,
                                     Telefon As Telefoniegerät,
                                     CLIR As Boolean, Abbruch As Boolean) As Task(Of Boolean) Implements IDialService.DialTelNr

        ' Start den Wählvorgang
        Return Await Wählclient.DialTelNr(TelNr, Telefon, CLIR, Abbruch)

    End Function

End Class
