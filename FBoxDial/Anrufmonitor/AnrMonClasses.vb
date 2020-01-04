
Friend Structure AnrMonRING
    ''' <summary>
    ''' Uhrzeit
    ''' </summary>
    Friend Property Zeit As Date

    ''' <summary>
    ''' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' </summary>
    Friend Property ID As Integer

    ''' <summary>
    ''' Eingehende Telefonnummer
    ''' </summary>
    Friend Property EingehendeNummer As Telefonnummer

    ''' <summary>
    ''' Angerufene eigene Telefonnummer, MSN
    ''' </summary>
    Friend Property EigeneNummer As Telefonnummer

    ''' <summary>
    ''' Anschluss, SIP...
    ''' </summary>
    Friend Property Anschluss As String
End Structure

Friend Structure AnrMonCALL
    ''' <summary>
    ''' Uhrzeit
    ''' </summary>
    Friend Property Zeit As Date

    ''' <summary>
    ''' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' </summary>
    Friend Property ID As Integer

    ''' <summary>
    ''' Nebenstellennummer, eindeutige Zuordnung des Telefons
    ''' </summary>
    Friend Property NebenstellenNummer As Integer

    ''' <summary>
    ''' die gewählte Rufnummer
    ''' </summary>
    Friend Property TelNr As Telefonnummer
End Structure

Friend Structure AnrMonCONNECT
    ''' <summary>
    ''' Uhrzeit
    ''' </summary>
    Friend Property Zeit As Date

    ''' <summary>
    ''' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' </summary>
    Friend Property ID As Integer

    ''' <summary>
    ''' Nebenstellennummer, eindeutige Zuordnung des Telefons
    ''' </summary>
    Friend Property NebenstellenNummer As Integer

    ''' <summary>
    ''' Gewählte Nummer Telefonnummer bzw. eingehende Telefonnummer
    ''' </summary>
    Friend Property TelNr As Telefonnummer
End Structure

Friend Structure AnrMonDISCONNECT
    ''' <summary>
    ''' Uhrzeit
    ''' </summary>
    Friend Property Zeit As Date

    ''' <summary>
    ''' Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' </summary>
    Friend Property ID As Integer

        ''' <summary>
        ''' Dauer des Telefonates
        ''' </summary>
        Friend Property Dauer As Integer
    End Structure