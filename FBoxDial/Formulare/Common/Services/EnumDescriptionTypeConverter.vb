Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection

''' <summary>
''' A Better Way to Data Bind Enums in WPF
''' <code>https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/</code> 
''' <code>https://github.com/brianlagunas/BindingEnumsInWpf</code> 
''' </summary>
Public Class EnumDescriptionTypeConverter
    Inherits EnumConverter

    Public Sub New(type As Type)
        MyBase.New(type)
    End Sub

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As CultureInfo, value As Object, destinationType As Type) As Object
        If destinationType Is GetType(String) Then

            If value IsNot Nothing Then
                Dim fi As FieldInfo = value.GetType().GetField(value.ToString())

                If fi IsNot Nothing Then
                    Dim attributes = CType(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())
                    Return If(attributes.Length > 0 AndAlso Not String.IsNullOrEmpty(attributes(0).Description), attributes(0).Description, value.ToString())
                End If
            End If

            Return String.Empty
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function
End Class
