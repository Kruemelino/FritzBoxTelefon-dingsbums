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

#Region "PrimaryHeader"
    Public Property PrimaryHeader As String
        Get
            Return CType(GetValue(PrimaryHeaderProperty), String)
        End Get
        Set
            SetValue(PrimaryHeaderProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly PrimaryHeaderProperty As DependencyProperty = DependencyProperty.Register(NameOf(PrimaryHeader),
                                                                                                    GetType(String),
                                                                                                    GetType(OptionsColor),
                                                                                                    New PropertyMetadata(String.Empty))
#End Region

#Region "SecondaryHeader"
    Public Property SecondaryHeader As String
        Get
            Return CType(GetValue(SecondaryHeaderProperty), String)
        End Get
        Set
            SetValue(SecondaryHeaderProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly SecondaryHeaderProperty As DependencyProperty = DependencyProperty.Register(NameOf(SecondaryHeader),
                                                                                                    GetType(String),
                                                                                                    GetType(OptionsColor),
                                                                                                    New PropertyMetadata(String.Empty))
#End Region

    Public Sub New()
        InitializeComponent()
    End Sub

End Class