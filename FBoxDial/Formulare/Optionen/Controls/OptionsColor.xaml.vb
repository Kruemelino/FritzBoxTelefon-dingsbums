Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input

Partial Public Class OptionsColor
    Inherits UserControl

#Region "PrimaryColor"
    Public Property PrimaryColor As Media.Color
        Get
            Return CType(GetValue(PrimaryColorProperty), Media.Color)
        End Get
        Set
            SetValue(PrimaryColorProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly PrimaryColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(PrimaryColor),
                                                                                                    GetType(Media.Color),
                                                                                                    GetType(OptionsColor),
                                                                                                    New PropertyMetadata(Media.Colors.Black))
#End Region

#Region "SecondaryColor"
    Public Property SecondaryColor As Media.Color
        Get
            Return CType(GetValue(SecondaryColorProperty), Media.Color)
        End Get
        Set
            SetValue(SecondaryColorProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly SecondaryColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SecondaryColor),
                                                                                                      GetType(Media.Color),
                                                                                                      GetType(OptionsColor),
                                                                                                      New PropertyMetadata(Media.Colors.Black))
#End Region

#Region "ShowAlpha"
    Public Property ShowAlpha As Boolean
        Get
            Return CBool(GetValue(ShowAlphaProperty))
        End Get
        Set
            SetValue(ShowAlphaProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly ShowAlphaProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowAlpha),
                                                                                                 GetType(Boolean),
                                                                                                 GetType(OptionsColor),
                                                                                                 New PropertyMetadata(False))
#End Region



    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub TogglePopupButton_MouseLeave(sender As Object, e As MouseEventArgs) Handles TogglePopupButton.MouseLeave
        ToggledPopup.StaysOpen = False
    End Sub

    Private Sub TogglePopupButton_MouseEnter(sender As Object, e As MouseEventArgs) Handles TogglePopupButton.MouseEnter
        ToggledPopup.StaysOpen = True
    End Sub
End Class