﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Dieser Code wurde von einem Tool generiert.
'     Laufzeitversion:4.0.30319.42000
'
'     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
'     der Code erneut generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace Localize
    
    'Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    '-Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    'Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    'mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    '''<summary>
    '''  Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Public Class LocWählclient
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("FBoxDial.LocWählclient", GetType(LocWählclient).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        '''  Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Rufnummer unterdrücken ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strCLIR() As String
            Get
                Return ResourceManager.GetString("strCLIR", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Verbinden über... ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strConnectTo() As String
            Get
                Return ResourceManager.GetString("strConnectTo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Wählen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strDial() As String
            Get
                Return ResourceManager.GetString("strDial", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Direktwahl ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strDirect() As String
            Get
                Return ResourceManager.GetString("strDirect", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Direktwahl - Geben Sie die zu wählende Telefonnummer direkt ein. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strDirectDial() As String
            Get
                Return ResourceManager.GetString("strDirectDial", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Anruf: {0} ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strHeader() As String
            Get
                Return ResourceManager.GetString("strHeader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Sie sind dabei eine Mobilnummer ({0}) anzurufen. Fortsetzen? ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strQMobil() As String
            Get
                Return ResourceManager.GetString("strQMobil", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Status ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strStatus() As String
            Get
                Return ResourceManager.GetString("strStatus", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Fehler ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strStatusError() As String
            Get
                Return ResourceManager.GetString("strStatusError", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Abgebrochen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strStatusHangUp() As String
            Get
                Return ResourceManager.GetString("strStatusHangUp", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Jetzt Abheben ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strStatusPickUp() As String
            Get
                Return ResourceManager.GetString("strStatusPickUp", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Bitte warten ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strStatusWait() As String
            Get
                Return ResourceManager.GetString("strStatusWait", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
