﻿Imports System.Windows.Markup
''' <summary>
''' A Better Way to Data Bind Enums in WPF
''' <code>https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/</code> 
''' <code>https://github.com/brianlagunas/BindingEnumsInWpf</code> 
''' </summary>
Public Class EnumBindingSourceExtension
    Inherits MarkupExtension
    Private _enumType As Type

    Public Property EnumType As Type
        Get
            Return _enumType
        End Get
        Set

            If Value IsNot _enumType Then

                If Value IsNot Nothing Then
                    Dim enumType As Type = If(Nullable.GetUnderlyingType(Value), Value)
                    If Not enumType.IsEnum Then Throw New ArgumentException("Type must be for an Enum.")
                End If

                _enumType = Value
            End If
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(enumType As Type)
        Me.EnumType = enumType
    End Sub

    Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
        If _enumType Is Nothing Then Throw New InvalidOperationException("The EnumType must be specified.")

        Dim actualEnumType As Type = If(Nullable.GetUnderlyingType(_enumType), _enumType)
        Dim enumValues As Array = [Enum].GetValues(actualEnumType)

        If actualEnumType = _enumType Then Return enumValues

        Dim tempArray As Array = Array.CreateInstance(actualEnumType, enumValues.Length + 1)
        enumValues.CopyTo(tempArray, 1)

        Return tempArray
    End Function
End Class