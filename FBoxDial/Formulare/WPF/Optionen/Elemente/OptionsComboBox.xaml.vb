Imports System.Windows
Imports System.Windows.Controls

Partial Public Class OptionsComboBox
    Inherits UserControl

#Region "Label"

    Public Property Label As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set(value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "DisplayMemberPath"

    Public Property DisplayMemberPath As String
        Get
            Return CStr(GetValue(DisplayMemberPathProperty))
        End Get
        Set(value As String)
            SetValue(DisplayMemberPathProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DisplayMemberPathProperty As DependencyProperty = DependencyProperty.Register("DisplayMemberPath", GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "SelectedValuePath"

    Public Property SelectedValuePath As String
        Get
            Return CStr(GetValue(SelectedValuePathProperty))
        End Get
        Set(value As String)
            SetValue(SelectedValuePathProperty, value)
        End Set
    End Property

    Public Shared ReadOnly SelectedValuePathProperty As DependencyProperty = DependencyProperty.Register("SelectedValuePath", GetType(String), GetType(OptionsComboBox), New PropertyMetadata(""))

#End Region

#Region "SelectedValue"

    Public Property SelectedValue As Object
        Get
            Return GetValue(SelectedValueProperty)
        End Get
        Set(value As Object)
            SetValue(SelectedValueProperty, value)
        End Set
    End Property

    Public Shared ReadOnly SelectedValueProperty As DependencyProperty = DependencyProperty.Register("SelectedValue", GetType(Object), GetType(OptionsComboBox), New PropertyMetadata(Nothing))

#End Region

#Region "ItemsSource"

    Public Property ItemsSource As Object
        Get
            Return GetValue(ItemsSourceProperty)
        End Get
        Set(value As Object)
            SetValue(ItemsSourceProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.Register("ItemsSource", GetType(Object), GetType(OptionsComboBox), New PropertyMetadata(Nothing))

#End Region

    Public Sub New()
        InitializeComponent()
    End Sub
End Class