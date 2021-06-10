Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsPasswordBox
    Inherits UserControl

#Region "Label"

    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set
            SetValue(LabelProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Label dependency property
    ''' </summary>
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(Label), GetType(String), GetType(OptionsPasswordBox), New PropertyMetadata(""))

#End Region

#Region "Value"

    ''' <summary>
    ''' Gets or sets the Value which is being displayed
    ''' </summary>
    Public Property Value As String
        Get
            Return CStr(GetValue(ValueProperty))
        End Get
        Set
            SetValue(ValueProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the Value dependency property
    ''' </summary>
    Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(Value), GetType(String), GetType(OptionsPasswordBox), New PropertyMetadata(""))

#End Region

#Region "DeCryptKey"

    ''' <summary>
    ''' Gets or sets the DeCryptKey which is being displayed
    ''' </summary>
    Public Property DeCryptKey As String
        Get
            Return CStr(GetValue(DeCryptKeyProperty))
        End Get
        Set
            SetValue(DeCryptKeyProperty, Value)
        End Set
    End Property

    ''' <summary>
    ''' Identified the DeCryptKey dependency property
    ''' </summary>
    Public Shared ReadOnly DeCryptKeyProperty As DependencyProperty = DependencyProperty.Register(NameOf(DeCryptKey), GetType(String), GetType(OptionsPasswordBox), New PropertyMetadata(""))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub PwBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles PwBox.GotFocus
        PwBox.Clear()
    End Sub

    Private Sub PwBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles PwBox.LostFocus

        If PwBox.Password.IsStringNothingOrEmpty Then
            PwBox.Password = "1234"
        Else
            If PwBox.Password.AreNotEqual("1234") Then
                Using Crypt As New Rijndael
                    Value = Crypt.EncryptString(PwBox.Password, DeCryptKey)
                End Using
            End If
        End If
    End Sub
End Class