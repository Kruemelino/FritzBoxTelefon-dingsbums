Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsComboBox
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

    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register(NameOf(Label), GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "DisplayMemberPath"

    Public Property DisplayMemberPath As String
        Get
            Return CStr(GetValue(DisplayMemberPathProperty))
        End Get
        Set
            SetValue(DisplayMemberPathProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly DisplayMemberPathProperty As DependencyProperty = DependencyProperty.Register(NameOf(DisplayMemberPath), GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "SelectedValuePath"

    Public Property SelectedValuePath As String
        Get
            Return CStr(GetValue(SelectedValuePathProperty))
        End Get
        Set
            SetValue(SelectedValuePathProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly SelectedValuePathProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedValuePath), GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "SelectedValue"

    Public Property SelectedValue As Object
        Get
            Return GetValue(SelectedValueProperty)
        End Get
        Set
            SetValue(SelectedValueProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly SelectedValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedValue), GetType(Object), GetType(OptionsComboBox), New PropertyMetadata(Nothing))

#End Region

#Region "ItemsSource"

    Public Property ItemsSource As Object
        Get
            Return GetValue(ItemsSourceProperty)
        End Get
        Set
            SetValue(ItemsSourceProperty, Value)
        End Set
    End Property

    Public Shared ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.Register(NameOf(ItemsSource), GetType(Object), GetType(OptionsComboBox), New PropertyMetadata(Nothing))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class