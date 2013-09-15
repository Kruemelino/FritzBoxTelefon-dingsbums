Imports System.Text

Public Class FormMain
    Private C_ini As InI
    Private C_Helfer As Helfer
    Private C_Crypt As Rijndael
    Private C_FBox As FritzBox

    Private WithEvents emc As New EventMulticaster
    Private FBFehler As ErrObject
    Private FBEncoding As System.Text.Encoding = Encoding.UTF8
    Private Delegate Sub DelgSetLine()

    Private StatusWert As String
    Private DateiPfad As String
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        DateiPfad = GetSetting("FritzBox", "Optionen", "TBini", "-1")
        If Not IO.File.Exists(DateiPfad) Then DateiPfad = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Fritz!Box Telefon-dingsbums\FritzOutlook.ini"

        ' Klasse zum IO-der INI-Struktiur erstellen
        C_ini = New InI(DateiPfad)

        ' Klasse für Verschlüsselung erstellen
        C_Crypt = New Rijndael

        ' Klasse für Helferfunktionen erstellen
        C_Helfer = New Helfer(DateiPfad, C_ini, C_Crypt)

        C_FBox = New FritzBox(DateiPfad, C_ini, C_Helfer, C_Crypt, False, emc)
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        'MsgBox("Hello World")

        Dim Passwort As String

        Me.TBLandesVW.Text = C_ini.Read("Optionen", "TBLandesVW", "0049")
        'Me.TBFBAdr.Text = C_ini.Read("Optionen", "TBFBAdr", "fritz.box")
        Me.TBBenutzer.Text = C_ini.Read("Optionen", "TBBenutzer", vbNullString)
        Passwort = C_ini.Read("Optionen", "TBPasswort", "")
        If Not Len(Passwort) = 0 Then
            Me.TBPasswort.Text = "1234"
        End If
        Me.TBVorwahl.Text = C_ini.Read("Optionen", "TBVorwahl", "")
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
        C_ini.Write(DateiPfad, "Optionen", "TBLandesVW", Me.TBLandesVW.Text)
        C_ini.Write(DateiPfad, "Optionen", "TBBenutzer", Me.TBBenutzer.Text)
        If Not Me.TBPasswort.Text = "1234" Then
            C_ini.Write(DateiPfad, "Optionen", "TBPasswort", C_Crypt.EncryptString128Bit(Me.TBPasswort.Text, "Fritz!Box Script"))
            SaveSetting("FritzBox", "Optionen", "Zugang", "Fritz!Box Script")
            C_Helfer.KeyChange(DateiPfad)
        End If
        C_ini.Write(DateiPfad, "Optionen", "TBVorwahl", Me.TBVorwahl.Text)

        C_FBox.bRausschreiben = True
        C_FBox.FritzBoxDaten()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim FW550 As Boolean
        Dim sSID As String
        Dim sLink As String
        Dim tempstring As String

        'sSID = C_FBox.sDefaultSID
        sSID = C_FBox.FBLogin(FW550)
        If Not sSID = C_FBox.sDefaultSID Then
            If FW550 Then
                sLink = "http://fritz.box/fon_num/fon_num_list.lua?sid=" & sSID
            Else
                sLink = "http://fritz.box/cgi-bin/webcm?sid=" & sSID & "&getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices"
            End If

            tempstring = C_Helfer.httpRead(sLink, FBEncoding, FBFehler)
            Me.TBTelefonie.Text = tempstring
        End If
    End Sub
End Class

