Imports System.Reflection

Friend Module FboxConverter

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    <Obsolete> Friend Function FBoxConvertTo(Of T1, T2)(TR064Obj As T1) As T2

        Dim FBoxAddinObj As T2 = Activator.CreateInstance(Of T2)

        ' Schleife durch alle Properties dieser Klasse
        For Each TR064ObjPropertyInfo As PropertyInfo In GetType(T1).GetProperties
            ' Suche das passende Property in den Optionen
            Dim FBoxAddinPropertyInfo As PropertyInfo = Array.Find(GetType(T2).GetProperties, Function(ItemPropertyInfo As PropertyInfo) ItemPropertyInfo.Name.AreEqual(TR064ObjPropertyInfo.Name))

            If FBoxAddinPropertyInfo IsNot Nothing Then
                Try
                    If FBoxAddinPropertyInfo.CanWrite AndAlso (FBoxAddinPropertyInfo.PropertyType.IsPrimitive Or FBoxAddinPropertyInfo.PropertyType = GetType(String) Or FBoxAddinPropertyInfo.PropertyType = GetType(Decimal)) Then
                        FBoxAddinPropertyInfo.SetValue(FBoxAddinObj, TR064ObjPropertyInfo.GetValue(TR064Obj))
                        NLogger.Trace($"Feld {FBoxAddinPropertyInfo.Name} mit Wert '{FBoxAddinPropertyInfo.GetValue(FBoxAddinObj)}' geladen.")
                    End If
                Catch ex As Exception
                    NLogger.Error(ex, $"Fehler beim Laden des Feldes {TR064ObjPropertyInfo.Name}.")

                End Try
            End If
        Next
        Return FBoxAddinObj
    End Function

End Module
