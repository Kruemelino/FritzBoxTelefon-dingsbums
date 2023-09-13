Public Class EqualityComparer

    Implements IEqualityComparer(Of Telefonnummer)
    Implements IEqualityComparer(Of Telefonat)
    Implements IEqualityComparer(Of VIPEntry)

    Private disposedValue As Boolean
#Region "Telefonnummer"
    Public Overloads Function Equals(x As Telefonnummer, y As Telefonnummer) As Boolean Implements IEqualityComparer(Of Telefonnummer).Equals
        Return x.Equals(y)
    End Function

    Public Overloads Function GetHashCode(obj As Telefonnummer) As Integer Implements IEqualityComparer(Of Telefonnummer).GetHashCode

        ' Check whether the object is null.
        If obj Is Nothing Then Return 0

        Return If(obj.Unformatiert Is Nothing, 0, obj.Unformatiert.GetHashCode())
    End Function
#End Region

#Region "Telefonat"
    Public Overloads Function Equals(x As Telefonat, y As Telefonat) As Boolean Implements IEqualityComparer(Of Telefonat).Equals
        Return x.Equals(y)
    End Function

    Public Overloads Function GetHashCode(obj As Telefonat) As Integer Implements IEqualityComparer(Of Telefonat).GetHashCode

        ' Check whether the object is null.
        If obj Is Nothing Then Return 0

        Return If(obj.AnruferName Is Nothing, 0, obj.AnruferName.GetHashCode())
    End Function
#End Region

#Region "VIPEntry"
    Public Overloads Function Equals(x As VIPEntry, y As VIPEntry) As Boolean Implements IEqualityComparer(Of VIPEntry).Equals
        Return x.Equals(y)
    End Function

    Public Overloads Function GetHashCode(obj As VIPEntry) As Integer Implements IEqualityComparer(Of VIPEntry).GetHashCode

        ' Check whether the object is null.
        If obj Is Nothing Then Return 0

        Return If(obj.EntryID Is Nothing, 0, obj.EntryID.GetHashCode())
    End Function

#End Region

End Class
