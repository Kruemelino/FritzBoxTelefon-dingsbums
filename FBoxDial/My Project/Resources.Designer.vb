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
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0"),  _
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
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property Add() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Add", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property call_made() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("call_made", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property call_missed() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("call_missed", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property call_received() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("call_received", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property CallTo() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("CallTo", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property Cancel() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Cancel", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property Download() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Download", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property Remove() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Remove", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        '''&lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot; onLoad=&quot;Ribbon_Load&quot;&gt;
        '''  &lt;ribbon startFromScratch=&quot;false&quot;&gt;
        '''    &lt;tabs&gt;
        '''      &lt;tab idMso=&quot;TabCalendar&quot;&gt;
        '''        &lt;group id=&quot;Tab_Calendar&quot; getLabel=&quot;GetItemLabel&quot; autoScale=&quot;false&quot; imageMso=&quot;AutoDial&quot;&gt;
        '''          &lt;splitButton id=&quot;spb1_K&quot; size=&quot;large&quot;&gt;
        '''            &lt;button id=&quot;btnDialExpl_K&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; onAction=&quot;BtnOnAction&quot;  getScreentip=&quot;GetItemScreenTipp&quot;/&gt;        ''' [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonExplorer() As String
            Get
                Return ResourceManager.GetString("RibbonExplorer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;customUI xmlns=&quot;http://schemas.microsoft.com/office/2009/07/customui&quot; onLoad=&quot;ribbonLoaded_&quot; loadImage=&quot;getImages&quot;&gt;
        '''  &lt;contextMenus&gt;
        '''    &lt;contextMenu idMso=&quot;ContextMenuContactCardRecipient&quot;&gt;      
        '''      &lt;menuSeparator id=&quot;Seperator_CMR3&quot;/&gt;
        '''      &lt;button id=&quot;rbtnDial_CMR&quot; getLabel=&quot;GetItemLabel&quot; imageMso=&quot;AutoDial&quot; onAction=&quot;BtnOnAction&quot;/&gt;
        '''    &lt;/contextMenu&gt;
        '''  &lt;/contextMenus&gt;
        '''&lt;/customUI&gt;
        '''
        ''' ähnelt.
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
        '''      &lt;tab idMso=&quot;TabJournal&quot;&gt;
        '''        &lt;group id=&quot;Tab_Journal&quot; getLabel=&quot;GetItemLabel&quot; getVisible=&quot;ShowInspectorRibbon&quot;&gt;
        '''          &lt;button id=&quot;btnDialInsp_J&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;large&quot; getEnabled=&quot;EnableBtnJournal&quot; onAction=&quot;BtnOnActionI&quot;/&gt;
        '''          &lt;separator id=&quot;Seperator_J1&quot;/&gt;
        '''          &lt;button i [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
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
        '''          &lt;button id=&quot;btnDialInsp_C&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;large&quot; onAction=&quot;BtnOnActionI&quot;/&gt;
        '''          &lt;separator id=&quot;Seperator_C1&quot;/&gt;
        '''          &lt;dynamicMenu id=&quot;btnRWS_C&quot; tag=&quot;RWSDasO [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
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
        '''          &lt;button id=&quot;btnDialInsp_M&quot; getLabel=&quot;GetItemLabel&quot; getImage=&quot;GetItemImageMso&quot; getScreentip=&quot;GetItemScreenTipp&quot; size=&quot;large&quot; onAction=&quot;BtnOnActionI&quot;/&gt;
        '''        &lt;/group&gt;
        '''      &lt;/tab&gt;
        '''    &lt;/tabs&gt;
        '''  &lt;/ribbon&gt;
        '''&lt;/customUI&gt;
        '''
        ''' ähnelt.
        '''</summary>
        Public ReadOnly Property RibbonInspectorMailRead() As String
            Get
                Return ResourceManager.GetString("RibbonInspectorMailRead", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property Save() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Save", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
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
        '''  Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        '''</summary>
        Public ReadOnly Property Upload() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Upload", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die &lt;KZ&gt;
        '''  &lt;LKZ n=&quot;1&quot;&gt;
        '''    &lt;ONKZ n=&quot;1809&quot;/&gt;
        '''    &lt;ONKZ n=&quot;201&quot;/&gt;
        '''    &lt;ONKZ n=&quot;202&quot;/&gt;
        '''    &lt;ONKZ n=&quot;203&quot;/&gt;
        '''    &lt;ONKZ n=&quot;204&quot;/&gt;
        '''    &lt;ONKZ n=&quot;205&quot;/&gt;
        '''    &lt;ONKZ n=&quot;206&quot;/&gt;
        '''    &lt;ONKZ n=&quot;207&quot;/&gt;
        '''    &lt;ONKZ n=&quot;208&quot;/&gt;
        '''    &lt;ONKZ n=&quot;209&quot;/&gt;
        '''    &lt;ONKZ n=&quot;210&quot;/&gt;
        '''    &lt;ONKZ n=&quot;212&quot;/&gt;
        '''    &lt;ONKZ n=&quot;213&quot;/&gt;
        '''    &lt;ONKZ n=&quot;214&quot;/&gt;
        '''    &lt;ONKZ n=&quot;215&quot;/&gt;
        '''    &lt;ONKZ n=&quot;216&quot;/&gt;
        '''    &lt;ONKZ n=&quot;217&quot;/&gt;
        '''    &lt;ONKZ n=&quot;226&quot;/&gt;
        '''    &lt;ONKZ n=&quot;236&quot;/&gt;
        '''    &lt;ONKZ n=&quot;242&quot;/&gt;
        '''    &lt;ONKZ n=&quot;246&quot;/&gt;
        '''    &lt;ONKZ n=&quot;249&quot;/&gt;
        '''    &lt;ONKZ n=&quot;250&quot;/&gt;
        '''    &lt;ON [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Public ReadOnly Property Vorwahlen() As String
            Get
                Return ResourceManager.GetString("Vorwahlen", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
