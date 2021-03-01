Imports System.Runtime.CompilerServices
Imports Microsoft.Office.Interop.Outlook

Public Module OutlookFolder
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

End Module
