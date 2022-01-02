Public Interface IDialService
    Sub UpdateTheme()

#Region "GetData"
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
    Function GetDialabePhones() As IEnumerable(Of Telefoniegerät)

    ''' <summary>
    ''' Ermittelt das zu letzt genutzte Telefon, oder gibt das gewählte Standard-Telefon zurück.
    ''' </summary>
    ''' <returns>Telefoniegerät</returns>
    Function GetSelectedPhone() As Telefoniegerät

    ''' <summary>
    ''' Gibt die Wahlwiederholungsliste zurück
    ''' </summary>
    ''' <returns>Auflistung von Telefonnummern</returns>
    Function GetLastTelNr() As IEnumerable(Of Telefonnummer)

    ''' <summary>
    ''' Gibt an, ob bei die ausghende Telefonnummer unterdrückt werden soll.
    ''' </summary>
    ''' <returns>Boolean</returns>
    ReadOnly Property GetCLIR() As Boolean

    ''' <summary>
    ''' Gibt an, ob bei einer Mobilnummer der Nutzer zunächt gefragt werden soll.
    ''' </summary>
    ''' <returns>Boolean</returns>
    ReadOnly Property GetMobil() As Boolean

#End Region
    ''' <summary>
    ''' Startet das Anwählen der übergebenen Telefonnummer auf dem ausgewählten Telefon.
    ''' </summary>
    ''' <param name="TelNr">Die zu wählende Telefonnummer</param>
    ''' <param name="Telefon">Das Telefon, über das gewählt werden soll</param>
    ''' <param name="CLIR">Rufnummernunterdrückung</param>
    ''' <param name="Abbruch">Angabe, ob der Wählvorgang abgebrochen werden soll</param>
    ''' <returns>Boolean, ob erfolgreich</returns>
    Function DialTelNr(TelNr As Telefonnummer,
                       Telefon As Telefoniegerät,
                       CLIR As Boolean, Abbruch As Boolean) As Threading.Tasks.Task(Of Boolean)

End Interface
