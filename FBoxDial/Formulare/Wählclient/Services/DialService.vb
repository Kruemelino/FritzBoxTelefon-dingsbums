Public Class DialService
    Implements IDialService

    ''' <summary>
    ''' Gibt an, ob bei einer Mobilnummer der Nutzer zunächt gefragt werden soll.
    ''' </summary>
    ''' <returns>Boolean</returns>
    Friend ReadOnly Property GetMobil As Boolean Implements IDialService.GetMobil
        Get
            Return XMLData.POptionen.CBCheckMobil
        End Get
    End Property

    ''' <summary>
    ''' Gibt an, ob bei die ausghende Telefonnummer unterdrückt werden soll.
    ''' </summary>
    ''' <returns>Boolean</returns>
    Friend ReadOnly Property GetCLIR As Boolean Implements IDialService.GetCLIR
        Get
            Return XMLData.POptionen.CBCLIR
        End Get
    End Property


    ''' <summary>
    ''' Ermittelt alle wählbaren Telefone:
    ''' <list type="bullet">
    ''' <item>FON</item>
    ''' <item>DECT</item>
    ''' <item>ISDN/S0</item>
    ''' <item>Phoner/MicroSIP</item>
    ''' </list>
    ''' </summary>
    ''' <returns>Auflistung von Telefoniegeräten</returns>
    Friend Function GetDialabePhones() As IEnumerable(Of Telefoniegerät) Implements IDialService.GetDialabePhones
        If XMLData.PTelefonie.Telefoniegeräte IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte.Any Then
            Return XMLData.PTelefonie.Telefoniegeräte.Where(Function(TG) TG.IsDialable)

        Else
            Return Nothing

        End If

    End Function

    ''' <summary>
    ''' Ermittelt das zu letzt genutzte Telefon, oder gibt das gewählte Standard-Telefon zurück.
    ''' </summary>
    ''' <returns>Telefoniegerät</returns>
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

    ''' <summary>
    ''' Gibt die Wahlwiederholungsliste zurück
    ''' </summary>
    ''' <returns>Auflistung von Telefonnummern</returns>
    Friend Function GetLastTelNr() As IEnumerable(Of Telefonnummer) Implements IDialService.GetLastTelNr
        If XMLData.PTelListen.CALLListe IsNot Nothing AndAlso XMLData.PTelListen.CALLListe.Any Then
            Return XMLData.PTelListen.GetTelNrList(XMLData.PTelListen.CALLListe)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Startet das Anwählen der übergebenen Telefonnummer auf dem ausgewählten Telefon.
    ''' </summary>
    ''' <param name="Wählclient">Der Wählclient, der das Telefonat initiieren soll.</param>
    ''' <param name="TelNr">Die zu wählende Telefonnummer</param>
    ''' <param name="Telefon">Das Telefon, über das gewählt werden soll</param>
    ''' <param name="CLIR">Rufnummernunterdrückung</param>
    ''' <param name="Abbruch">Angabe, ob der Wählvorgang abgebrochen werden soll</param>
    ''' <returns>Boolean, ob erfolgreich</returns>
    Friend Function DialNumber(Wählclient As FritzBoxWählClient,
                               TelNr As Telefonnummer,
                               Telefon As Telefoniegerät,
                               CLIR As Boolean, Abbruch As Boolean) As Boolean Implements IDialService.DialNumber

        ' Start den Wählvorgang
        Return Wählclient.DialTelNr(TelNr, Telefon, CLIR, Abbruch)

    End Function

End Class
