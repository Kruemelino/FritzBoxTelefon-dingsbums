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

Namespace My.Resources
    
    'Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    '-Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    'Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    'mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    '''<summary>
    '''  Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Public Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("FBoxDial.Resources", GetType(Resources).Assembly)
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
        Public Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        '''&lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot; onLoad=&quot;Ribbon_Load&quot;&gt;
        '''	&lt;ribbon startFromScratch=&quot;false&quot;&gt;
        '''		&lt;tabs&gt;
        '''			&lt;tab idMso=&quot;TabCalendar&quot;&gt;
        '''				&lt;group id=&quot;Tab_Calendar&quot; getLabel=&quot;GetItemLabel&quot; autoScale=&quot;false&quot; imageMso=&quot;AutoDial&quot;&gt;
        '''					&lt;splitButton id=&quot;spb1_K&quot; size=&quot;large&quot;&gt;
        '''						&lt;button id=&quot;Dial_K&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; onAction=&quot;BtnOnAction&quot; getScreentip=&quot;GetItemScreenTipp&quot; getEnabled=&quot;DialEnabled&quot;/&gt;
        '''			 [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonExplorer() As String
            Get
                Return ResourceManager.GetString("RibbonExplorer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot;&gt;
        '''  &lt;contextMenus&gt;
        '''    &lt;contextMenu idMso=&quot;ContextMenuContactCardRecipient&quot;&gt;      
        '''      &lt;menuSeparator id=&quot;Seperator_CMR1&quot;/&gt;
        '''      &lt;button id=&quot;Dial_CMR2&quot; getLabel=&quot;GetItemLabel&quot; imageMso=&quot;AutoDial&quot; onAction=&quot;BtnOnActionCC&quot; getEnabled=&quot;DialEnabled&quot;/&gt;
        '''    &lt;/contextMenu&gt;
        '''  &lt;/contextMenus&gt;
        '''&lt;/customUI&gt; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonIMLayerUI() As String
            Get
                Return ResourceManager.GetString("RibbonIMLayerUI", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        '''&lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot;&gt;
        '''  &lt;ribbon&gt;
        '''    &lt;tabs&gt;
        '''      &lt;tab idMso=&quot;TabAppointment&quot;&gt;
        '''        &lt;group id=&quot;Tab_Appointment&quot; getLabel=&quot;GetItemLabel&quot; getVisible=&quot;ShowInspectorRibbon&quot;&gt;
        '''          &lt;button id=&quot;Dial_A&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;normal&quot; getEnabled=&quot;DialEnabled&quot; onAction=&quot;BtnOnActionI&quot;/&gt;
        '''          &lt;button id=&quot;RWS_A&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;G [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonInspectorAppointment() As String
            Get
                Return ResourceManager.GetString("RibbonInspectorAppointment", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        '''&lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot;&gt;
        '''  &lt;ribbon&gt;
        '''    &lt;tabs&gt;
        '''      &lt;tab idMso=&quot;TabJournal&quot;&gt;
        '''        &lt;group id=&quot;Tab_Journal&quot; getLabel=&quot;GetItemLabel&quot; getVisible=&quot;ShowInspectorRibbon&quot;&gt;
        '''          &lt;button id=&quot;Dial_J&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;normal&quot; getEnabled=&quot;DialEnabled&quot; onAction=&quot;BtnOnActionI&quot;/&gt;
        '''          &lt;button id=&quot;RWS_J&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemIm [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonInspectorJournal() As String
            Get
                Return ResourceManager.GetString("RibbonInspectorJournal", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        '''&lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot;&gt;
        '''  &lt;ribbon&gt;
        '''    &lt;tabs&gt;
        '''      &lt;tab idMso=&quot;TabContact&quot;&gt;
        '''        &lt;group id=&quot;Tab_Contact&quot; getLabel=&quot;GetItemLabel&quot; getVisible=&quot;ShowInspectorRibbon&quot;&gt;
        '''          &lt;button id=&quot;Dial_C&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;large&quot; onAction=&quot;BtnOnActionI&quot; getEnabled=&quot;DialEnabled&quot;/&gt;
        '''          &lt;separator id=&quot;Seperator_C1&quot;/&gt;
        '''          &lt;dynamicMenu id=&quot;RWS_ [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonInspectorKontakt() As String
            Get
                Return ResourceManager.GetString("RibbonInspectorKontakt", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        '''&lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot;&gt;
        '''  &lt;ribbon&gt;
        '''    &lt;tabs&gt;
        '''      &lt;tab idMso=&quot;TabReadMessage&quot;&gt;
        '''        &lt;group id=&quot;Tab_ReadMessage&quot; getLabel=&quot;GetItemLabel&quot; getVisible=&quot;ShowInspectorRibbon&quot;&gt;
        '''          &lt;button id=&quot;Dial_M&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;large&quot; onAction=&quot;BtnOnActionI&quot; getEnabled=&quot;DialEnabled&quot;/&gt;
        '''        &lt;/group&gt;
        '''      &lt;/tab&gt;
        '''    &lt;/tabs&gt;
        '''  &lt;/ribbon&gt;
        '''&lt;/custo [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonInspectorMailRead() As String
            Get
                Return ResourceManager.GetString("RibbonInspectorMailRead", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Fritz!Box Telefon-dingsbums ähnelt.
        '''</summary>
        Public ReadOnly Property strDefLongName() As String
            Get
                Return ResourceManager.GetString("strDefLongName", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die FritzOutlookV5 ähnelt.
        '''</summary>
        Public ReadOnly Property strDefShortName() As String
            Get
                Return ResourceManager.GetString("strDefShortName", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die ZugangAuthTest ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltAuthTestDeCryptKey() As String
            Get
                Return ResourceManager.GetString("strDfltAuthTestDeCryptKey", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die ZugangV5 ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltDeCryptKey() As String
            Get
                Return ResourceManager.GetString("strDfltDeCryptKey", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die ZugangIPPhone ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltIPPhoneDeCryptKey() As String
            Get
                Return ResourceManager.GetString("strDfltIPPhoneDeCryptKey", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die CALLListe ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltNameListCALL() As String
            Get
                Return ResourceManager.GetString("strDfltNameListCALL", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die RINGListe ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltNameListRING() As String
            Get
                Return ResourceManager.GetString("strDfltNameListRING", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die VIPListe ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltNameListVIP() As String
            Get
                Return ResourceManager.GetString("strDfltNameListVIP", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die Optionen ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltOptions() As String
            Get
                Return ResourceManager.GetString("strDfltOptions", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die ZugangPhoner ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltPhonerDeCryptKey() As String
            Get
                Return ResourceManager.GetString("strDfltPhonerDeCryptKey", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die ZugangTellows ähnelt.
        '''</summary>
        Public ReadOnly Property strDfltTellowsDeCryptKey() As String
            Get
                Return ResourceManager.GetString("strDfltTellowsDeCryptKey", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die TelProt.txt ähnelt.
        '''</summary>
        Public ReadOnly Property strLinkProtFileName() As String
            Get
                Return ResourceManager.GetString("strLinkProtFileName", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot; standalone=&quot;yes&quot;?&gt;
        '''&lt;KZ&gt;
        '''	&lt;LKZ n=&quot;376&quot; Code=&quot;AD&quot;&gt;
        '''		&lt;ONKZ n=&quot;3&quot; Name=&quot;Mobile Phones&quot;/&gt;
        '''		&lt;ONKZ n=&quot;4&quot; Name=&quot;Mobile Phones&quot;/&gt;
        '''		&lt;ONKZ n=&quot;6&quot; Name=&quot;Mobile Phones&quot;/&gt;
        '''		&lt;ONKZ n=&quot;7&quot; Name=&quot;Andorra la Vella&quot;/&gt;
        '''		&lt;ONKZ n=&quot;8&quot; Name=&quot;Andorra la Vella&quot;/&gt;
        '''	&lt;/LKZ&gt;
        '''  &lt;LKZ n=&quot;971&quot; Code=&quot;AE&quot;&gt;
        '''    &lt;ONKZ n=&quot;2&quot; Name=&quot;Abu Dhabi&quot;/&gt;
        '''    &lt;ONKZ n=&quot;3&quot; Name=&quot;Al-Ain&quot;/&gt;
        '''    &lt;ONKZ n=&quot;4&quot; Name=&quot;Dubai&quot;/&gt;
        '''    &lt;ONKZ n=&quot;48&quot;/&gt;
        '''    &lt;ONKZ n=&quot;50&quot; Name=&quot;Etisalat&quot;/&gt;
        '''    &lt;ONKZ n=&quot;52&quot; Name=&quot;Du&quot;/&gt;
        '''     [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property Vorwahlen() As String
            Get
                Return ResourceManager.GetString("Vorwahlen", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
