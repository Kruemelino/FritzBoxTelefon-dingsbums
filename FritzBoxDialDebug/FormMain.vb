Imports System.Text

Public Class FormMain
    Private C_DP As DataProvider
    Private C_Helfer As Helfer
    Private C_Crypt As MyRijndael
    Private C_FBox As FritzBox

    Private WithEvents emc As New EventMulticaster
    Private FBFehler As Boolean
    Private FBEncoding As System.Text.Encoding = Encoding.UTF8
    Private Delegate Sub DelgSetLine()

    Private StatusWert As String

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Klasse zum IO-der INI-Struktiur erstellen
        C_DP = New DataProvider()

        ' Klasse für Verschlüsselung erstellen
        C_Crypt = New MyRijndael

        ' Klasse für Helferfunktionen erstellen
        C_Helfer = New Helfer(C_DP, C_Crypt)

        C_FBox = New FritzBox(C_DP, C_Helfer, C_Crypt)
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        With C_DP
            Me.TBLandesVW.Text = .ProperyTBLandesVW
            Me.TBBenutzer.Text = .ProperyTBBenutzer
            If Not Len(.ProperyTBPasswort) = 0 Then Me.TBPasswort.Text = "1234"
            Me.TBVorwahl.Text = .ProperyTBVorwahl
        End With
    End Sub

    Public Function AddLine(ByVal Zeile As String) As Boolean
        AddLine = False
        StatusWert = Zeile
        If Me.InvokeRequired Then
            Dim D As New DelgSetLine(AddressOf setline)
            Invoke(D)
        Else
            setline()
        End If
    End Function

    Private Sub TextChangedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles emc.GenericEvent
        StatusWert = DirectCast(sender, Control).Text
        AddLine(StatusWert)
    End Sub

    Private Sub setline()
        With Me.TBDiagnose
            .Text += StatusWert & vbCrLf
            .SelectionStart = .Text.Length
            .ScrollToCaret()
        End With
    End Sub

    Private Sub BStart_Click(sender As Object, e As EventArgs) Handles BStart.Click
        C_DP.ProperyTBLandesVW = Me.TBLandesVW.Text
        C_DP.ProperyTBBenutzer = Me.TBBenutzer.Text
        C_DP.ProperyTBVorwahl = Me.TBVorwahl.Text
        If Not Me.TBPasswort.Text = "1234" Then
            C_DP.ProperyTBPasswort = C_Crypt.EncryptString128Bit(Me.TBPasswort.Text, C_DP.Propery_Def_PassWordDecryptionKey)
            C_DP.SaveSettingsVBA("Zugang", C_DP.Propery_Def_PassWordDecryptionKey)
            C_Helfer.KeyChange()
        End If
        With C_FBox
            .SetEventProvider(emc)
            .ProperySpeichereDaten = False
            .FritzBoxDaten()
        End With
    End Sub

    Private Sub BHerunterladen_Click(sender As Object, e As EventArgs) Handles BHerunterladen.Click
        Dim FW550 As Boolean
        Dim sSID As String
        Dim sLink As String
        Dim tempstring As String

        sSID = C_FBox.FBLogIn(FW550)
        If Not sSID = C_DP.Propery_Def_SessionID Then
            If FW550 Then
                sLink = "http://fritz.box/fon_num/fon_num_list.lua?sid=" & sSID
            Else
                sLink = "http://fritz.box/cgi-bin/webcm?sid=" & sSID & "&getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices"
            End If

            tempstring = C_Helfer.httpGET(sLink, FBEncoding, FBFehler)
            Me.TBTelefonie.Text = tempstring
        End If
    End Sub
End Class

