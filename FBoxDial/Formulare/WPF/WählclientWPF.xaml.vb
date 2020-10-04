Imports System.Drawing
Imports System.Threading
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Markup
Public Class WählclientWPF
    Inherits Window

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' init number list
        Dim Liste As New TelefonnummernListe
        Liste.FillDummyData()

        ' set list to datacontext
        With CType(DataContext, DialNumberListViewModel)
            .DialNumberList = Liste.Einträge
            .Name = Liste.Name
        End With


        ' Skalinierung
        'Height *= ScaleFaktor.Height
        'Width *= ScaleFaktor.Width
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
        CType(DataContext, Telefonat).ZeigeKontakt()
    End Sub
#End Region

End Class
