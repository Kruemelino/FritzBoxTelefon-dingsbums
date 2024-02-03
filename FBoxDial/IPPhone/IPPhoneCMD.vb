Public Module IPPhoneCMD

#Region "Eigenschaften"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property SoftPhoneReady(ProcessName As String) As Boolean
        Get
            Return Process.GetProcessesByName(ProcessName).Length.IsNotZero
        End Get
    End Property

#End Region

    Private Sub SoftPhoneStart(Connector As IIPPhoneConnector)
        NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneNichtBereit, Connector.Name))

        If Connector.ConnectionUriCall.IsNotStringNothingOrEmpty Then
            ' Starte Softphone
            Try
                Process.Start(Connector.ConnectionUriCall)

                NLogger.Info(String.Format(Localize.LocWählclient.strSoftPhoneGestartet, Connector.Name))
            Catch ex As ComponentModel.Win32Exception
                NLogger.Warn(ex)
            Catch ex As ObjectDisposedException
                NLogger.Warn(ex)
            Catch ex As IO.FileNotFoundException
                NLogger.Warn(ex)
            End Try

        End If
    End Sub

    ''' <summary>
    ''' Initiiert ein Telefonat über Softphone
    ''' </summary>
    ''' <param name="DialCode">Die zu wählende Nummer</param>
    ''' <param name="Hangup">Angabe, ob der Rufaufbau beendet werden soll.</param>
    Friend Function Dial(Connector As IIPPhoneConnector, DialCode As String, Hangup As Boolean) As Boolean

        Dial = False
        If Connector.Type = IPPhoneConnectorType.CMD Then
            If Not SoftPhoneReady(Connector.Name) Then SoftPhoneStart(Connector)

            If SoftPhoneReady(Connector.Name) Then
                ' Wählkommando senden
                If Hangup Then
                    ' Abbruch des Rufaufbaues mittels Parameter
                    Process.Start(Connector.ConnectionUriCall, Connector.CommandHangUp)

                    NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
                Else
                    ' Füge die Raute hinzu, falls gewünscht
                    DialCode += If(Connector.AppendSuffix, "#", String.Empty)

                    ' Aufbau des Telefonates mittels Parameter
                    Process.Start(Connector.ConnectionUriCall, Connector.CommandCallTo.Replace(Localize.LocOptionen.strIPPhoneCMDPlatzhalter, DialCode))

                    NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, Connector.Name))
                End If
                ' Gib Rückmeldung, damit Wählclient kein Fehler ausgibt
                Return True
            Else
                ' PhonerLite nicht verfügbar
                NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneNichtBereit, Connector.Name))
            End If

        End If
    End Function

End Module
