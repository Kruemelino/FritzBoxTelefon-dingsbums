Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsPasswordBox
    Inherits UserControl
#Region "Label"

    ''' <summary>
    ''' Gets or sets the Label which is displayed next to the field
    ''' </summary>
    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set(ByVal value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(OptionsPasswordBox), New PropertyMetadata(""))

#End Region

#Region "Value"

    ''' <summary>
    ''' Gets or sets the Value which is being displayed
    ''' </summary>
    Public Property Value As String
        Get
            Return CStr(GetValue(ValueProperty))
        End Get
        Set(ByVal value As String)
            SetValue(ValueProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("Value", GetType(String), GetType(OptionsPasswordBox), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
        ' Eventhandler erst nach dem Initialisieren hinzufügen
        'AddHandler PwBox.PasswordChanged, AddressOf PwBox_PasswordChanged
    End Sub

    Private Sub PwBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles PwBox.GotFocus
        PwBox.Clear()
    End Sub

    Private Sub PwBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles PwBox.LostFocus

        If PwBox.Password.IsStringNothingOrEmpty Then
            PwBox.Password = "1234"
        Else
            If PwBox.Password.AreNotEqual("1234") Then
                Using Crypt As Rijndael = New Rijndael
                    Value = Crypt.EncryptString128Bit(PwBox.Password, DefaultWerte.DfltDeCryptKey)
                End Using
            End If
        End If


    End Sub
End Class