Imports System.ComponentModel
Imports System.Resources

''' <summary>
''' Localize Enum Descriptions in WPF
''' <code>https://brianlagunas.com/localize-enum-descriptions-in-wpf/</code>
''' <code>https://github.com/brianlagunas/BindingEnumsInWpf</code>
''' </summary>
Public Class LocalizedDescriptionAttribute
    Inherits DescriptionAttribute

    Private Property ResManager As ResourceManager
    Private Property ResKey As String

    Public Sub New(resourceKey As String, resourceType As Type)
        ResManager = New ResourceManager(resourceType)
        ResKey = resourceKey
    End Sub

    Public Overrides ReadOnly Property Description As String
        Get
            Dim lDescription As String = ResManager.GetString(ResKey)
            Return If(String.IsNullOrWhiteSpace(lDescription), ResKey, lDescription)
        End Get
    End Property
End Class


