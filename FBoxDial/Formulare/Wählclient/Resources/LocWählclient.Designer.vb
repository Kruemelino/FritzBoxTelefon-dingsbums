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
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),  _
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
        '''  Sucht eine lokalisierte Zeichenfolge, die Abbrechen ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strCancel() As String
            Get
                Return ResourceManager.GetString("strCancel", resourceCulture)
            End Get
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
        '''  Sucht eine lokalisierte Zeichenfolge, die Es muss entweder ein Kontakt, eine eingegangene E-Mail-Adresse oder ein Journaleintrag ausgewählt sein. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strErrorAuswahl() As String
            Get
                Return ResourceManager.GetString("strErrorAuswahl", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Es ist kein Kontakt mit der E-Mail-Adresse {0} vorhanden. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strErrorMail() As String
            Get
                Return ResourceManager.GetString("strErrorMail", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die {0} ist nicht bereit... ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strErrorSoftphoneNotReady() As String
            Get
                Return ResourceManager.GetString("strErrorSoftphoneNotReady", resourceCulture)
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
        '''  Sucht eine lokalisierte Zeichenfolge, die PhonerLite ist bereit. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strPhonerLiteBereit() As String
            Get
                Return ResourceManager.GetString("strPhonerLiteBereit", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die PhonerLite gestartet ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strPhonerLitegestartet() As String
            Get
                Return ResourceManager.GetString("strPhonerLitegestartet", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Das Phoner-Passwort ist falsch. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strPhonerPasswortFalsch() As String
            Get
                Return ResourceManager.GetString("strPhonerPasswortFalsch", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Zu Phoner können keine Daten gesendet werden. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strPhonerReadonly() As String
            Get
                Return ResourceManager.GetString("strPhonerReadonly", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Die Phoner-Verson ist zu alt. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strPhonerZuAlt() As String
            Get
                Return ResourceManager.GetString("strPhonerZuAlt", resourceCulture)
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
        '''  Sucht eine lokalisierte Zeichenfolge, die Abbruch des Rufaufbaues erfolgreich übermittelt. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strSoftPhoneAbbruch() As String
            Get
                Return ResourceManager.GetString("strSoftPhoneAbbruch", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Das Softphone &apos;{0}&apos; ist bereit. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strSoftPhoneBereit() As String
            Get
                Return ResourceManager.GetString("strSoftPhoneBereit", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Telefonnummer {0} erfolgreich an {1} übermittelt. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strSoftPhoneErfolgreich() As String
            Get
                Return ResourceManager.GetString("strSoftPhoneErfolgreich", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Das Softphone &apos;{0}&apos; gestartet. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strSoftPhoneGestartet() As String
            Get
                Return ResourceManager.GetString("strSoftPhoneGestartet", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Das Softphone &apos;{0}&apos; ist nicht bereit. ähnelt.
        '''</summary>
        Public Shared ReadOnly Property strSoftPhoneNichtBereit() As String
            Get
                Return ResourceManager.GetString("strSoftPhoneNichtBereit", resourceCulture)
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
