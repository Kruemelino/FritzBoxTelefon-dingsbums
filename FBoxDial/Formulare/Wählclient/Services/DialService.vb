Imports System.Threading.Tasks

Public Class DialService
    Implements IDialService

    Friend ReadOnly Property GetMobil As Boolean Implements IDialService.GetMobil
        Get
            Return XMLData.POptionen.CBCheckMobil
        End Get
    End Property

    Friend ReadOnly Property GetCLIR As Boolean Implements IDialService.GetCLIR
        Get
            Return XMLData.POptionen.CBCLIR
        End Get
    End Property

    Friend Function GetDialabePhones() As IEnumerable(Of Telefoniegerät) Implements IDialService.GetDialabePhones
        If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then
            Return XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) TG.IsDialable)

        Else
            Return Nothing

        End If

    End Function

    Friend Function GetSelectedPhone() As Telefoniegerät Implements IDialService.GetSelectedPhone
        If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then

            If XMLData.PTelefonie.Telefoniegeräte.Exists(Function(TG) TG.StdTelefon) Then
                ' Ausgewähltes Standardgerät
                GetSelectedPhone = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.StdTelefon)

            Else
                ' Wenn kein Standard-Gerät in den Einstellungen festgelegt wurde, dann nimm das zuletzt genutzte Telefon
                GetSelectedPhone = XMLData.PTelefonie.Telefoniegeräte.Find(Function(TG) TG.ZuletztGenutzt)

            End If
        Else

            Return Nothing
        End If
    End Function

    Friend Function GetLastTelNr() As IEnumerable(Of Telefonnummer) Implements IDialService.GetLastTelNr
        If XMLData.PTelListen.CALLListe IsNot Nothing AndAlso XMLData.PTelListen.CALLListe.Any Then
            Return XMLData.PTelListen.GetTelNrList(XMLData.PTelListen.CALLListe)
        Else
            Return Nothing
        End If
    End Function

    Friend Async Function DialNumber(Wählclient As FritzBoxWählClient,
                               TelNr As Telefonnummer,
                               Telefon As Telefoniegerät,
                               CLIR As Boolean, Abbruch As Boolean) As Task(Of Boolean) Implements IDialService.DialNumber

        ' Start den Wählvorgang
        Return Await Wählclient.DialTelNr(TelNr, Telefon, CLIR, Abbruch)

    End Function

End Class
