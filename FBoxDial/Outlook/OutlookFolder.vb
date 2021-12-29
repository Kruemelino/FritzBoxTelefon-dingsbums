Imports System.Runtime.CompilerServices
Imports Microsoft.Office.Interop.Outlook

Public Module OutlookFolder
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    ''' <summary>
    ''' Prüft, ob der Outlook-Ordner für die gewünschte Verwendung ausgewählt wurde.
    ''' Falls der Nutzer keinen Ordner in den Einstellungen gewählt hat, wird der Standard-Ordner verwendet.
    ''' </summary>
    ''' <param name="Ordner"></param>
    ''' <param name="Verwendung"></param>
    ''' <returns></returns>
    <Extension> Public Function OrdnerAusgewählt(Ordner As MAPIFolder, Verwendung As OutlookOrdnerVerwendung) As Boolean

        Return XMLData.POptionen.OutlookOrdner.OrdnerAusgewählt(Ordner, Verwendung)

    End Function

    ''' <summary>
    ''' Dekrementiert den Verweiszähler des dem angegebenen COM-Objekt zugeordneten angegebenen Runtime Callable Wrapper (RCW)
    ''' </summary>
    ''' <param name="COMObject">Das freizugebende COM-Objekt.</param>
    Friend Sub ReleaseComObject(Of T)(COMObject As T)
        If COMObject IsNot Nothing Then
            Try
                Runtime.InteropServices.Marshal.ReleaseComObject(COMObject)
            Catch ex As ArgumentException
                NLogger.Error(ex, $"COM-Object ist kein gültiges COM-Objekt: {COMObject}")
            End Try
        End If
    End Sub
End Module
