VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsDB"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Member0" ,"ADOConnGroups"
Attribute VB_Ext_KEY = "Member1" ,"ChemConnGroups"
Attribute VB_Ext_KEY = "Member2" ,"clsADOConnGroup"
Attribute VB_Ext_KEY = "Member3" ,"clsChemConnGroup"
Attribute VB_Ext_KEY = "Member4" ,"clsDBGlobals"
Attribute VB_Ext_KEY = "Member5" ,"clsFieldMapGroup"
Attribute VB_Ext_KEY = "Member6" ,"clsFormGroup"
Attribute VB_Ext_KEY = "Member7" ,"clsSubFormGroup"
Attribute VB_Ext_KEY = "Member8" ,"clsTableAlias"
Attribute VB_Ext_KEY = "Member9" ,"clsTableGroup"
Attribute VB_Ext_KEY = "Member10" ,"FieldMapGroups"
Attribute VB_Ext_KEY = "Member11" ,"FormGroups"
Attribute VB_Ext_KEY = "Member12" ,"SubFormGroups"
Attribute VB_Ext_KEY = "Member13" ,"TableAliases"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F557018C"
Private mvarABOUT_WINDOW As String
Private mvarDISPLAY_NAME As String
Private mvarMAXHITS As String
Private mvarMAIN_PAGE As String
Private mvarDB_RECORD_COUNT As String
Private mvarDB_TYPE As String
Private mvarDBName As String
Private mvarADOConnGroups As ADOConnGroups
Private mvarChemConnGroups As ChemConnGroups
Private mvarFieldMapGroups As FieldMapGroups
Private mvarFormGroups As FormGroups
Private mvarSubFormGroups As SubFormGroups
Private mvarTableAliases As TableAliases
Private mvarTableGroups As TableGroups
Private mvarSTRADOConnGroups As String
Private mvarSTRChemConnGroups As String
Private mvarSTRFieldMapGroups As String
Private mvarSTRFormGroups As String
Private mvarSTRSubFormGroups As String
Private mvarSTRTableAliases As String
Private mvarSTRTableGroups As String

Public Property Get STRTableGroups() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TableGroups
    STRTableGroups = mvarSTRTableGroups
End Property

Public Property Let STRTableGroups(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TableAliases  = 5
   STRTableGroups = vData
End Property
Public Property Get STRSubFormGroups() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FieldMapGroups
    STRSubFormGroups = mvarSTRSubFormGroups
End Property

Public Property Let STRSubFormGroups(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SubFormGroups  = 5
   STRSubFormGroups = vData
End Property
Public Property Get STRFormGroups() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FieldMapGroups
   STRFormGroups = mvarSTRFormGroups
End Property

Public Property Let STRFormGroups(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.FormGroups  = 5
   STRFormGroups = vData
End Property
Public Property Get STRFieldMapGroups() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FieldMapGroups
    STRFieldMapGroups = mvarSTRFieldMapGroups
End Property

Public Property Let STRFieldMapGroups(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.FieldMapGroups  = 5
   STRFieldMapGroups = vData
End Property


Public Property Get STRChemConnGroups() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DBName
    STRChemConnGroups = mvarSTRChemConnGroups
End Property

Public Property Let STRChemConnGroups(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ChemConnGroups  = 5
   mvarSTRChemConnGroups = vData
End Property
Public Property Get STRADOConnGroups() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DBName
    STRADOConnGroups = mvarSTRADOConnGroups
End Property

Public Property Let STRADOConnGroups(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DBName  = 5
   mvarSTRADOConnGroups = vData
End Property
Public Property Get dbName() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DBName
    dbName = mvarDBName
End Property

Public Property Let dbName(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DBName  = 5
   mvarDBName = vData
End Property


Public Property Get clsDBGlobals() As clsDBGlobals
    Set clsDBGlobals = mvarclsDBGlobals
End Property


Public Property Set clsDBGlobals(vData As clsDBGlobals)
    Set mvarclsDBGlobals = vData
End Property
Public Property Get TableGroups() As TableGroups
    If mvarTableGroups Is Nothing Then
        Set mvarTableGroups = New TableGroups
    End If


    Set TableGroups = mvarTableGroups
End Property

Public Property Set TableGroups(vData As TableGroups)
    Set mvarTableGroups = vData
End Property


Public Property Get TableAliases() As TableAliases
    If mvarTableAliases Is Nothing Then
        Set mvarTableAliases = New TableAliases
    End If


    Set TableAliases = mvarTableAliases
End Property


Public Property Set TableAliases(vData As TableAliases)
    Set mvarTableAliases = vData
End Property



Public Property Get SubFormGroups() As SubFormGroups
    If mvarSubFormGroups Is Nothing Then
        Set mvarSubFormGroups = New SubFormGroups
    End If


    Set SubFormGroups = mvarSubFormGroups
End Property


Public Property Set SubFormGroups(vData As SubFormGroups)
    Set mvarSubFormGroups = vData
End Property


Public Property Get FormGroups() As FormGroups
    If mvarFormGroups Is Nothing Then
        Set mvarFormGroups = New FormGroups
    End If


    Set FormGroups = mvarFormGroups
End Property


Public Property Set FormGroups(vData As FormGroups)
    Set mvarFormGroups = vData
End Property


Public Property Get FieldMapGroups() As FieldMapGroups
    If mvarFieldMapGroups Is Nothing Then
        Set mvarFieldMapGroups = New FieldMapGroups
    End If


    Set FieldMapGroups = mvarFieldMapGroups
End Property


Public Property Set FieldMapGroups(vData As FieldMapGroups)
    Set mvarFieldMapGroups = vData
End Property



Public Property Get ChemConnGroups() As ChemConnGroups
    If mvarChemConnGroups Is Nothing Then
        Set mvarChemConnGroups = New ChemConnGroups
    End If


    Set ChemConnGroups = mvarChemConnGroups
End Property


Public Property Set ChemConnGroups(vData As ChemConnGroups)
    Set mvarChemConnGroups = vData
End Property



Public Property Get ADOConnGroups() As ADOConnGroups
    If mvarADOConnGroups Is Nothing Then
        Set mvarADOConnGroups = New ADOConnGroups
    End If


    Set ADOConnGroups = mvarADOConnGroups
End Property


Public Property Set ADOConnGroups(vData As ADOConnGroups)
    Set mvarADOConnGroups = vData
End Property



'##ModelId=3778F5800130
Private Sub Class_Terminate()
ABOUT_WINDOW = ""
DISPLAY_NAME = ""
MAXHITS = ""
MAIN_PAGE = ""
DB_RECORD_COUNT = ""
DB_TYPE = ""

dbName = ""

Set ADOConnGroups = Nothing
Set ChemConnGroups = Nothing
Set FieldMapGroups = Nothing
Set FormGroups = Nothing
Set SubFormGroups = Nothing
Set TableAliases = Nothing
Set TableGroups = Nothing
End Sub

Public Property Set FIELD_MAP_GROUPS(ByVal vData As FieldMapGroups)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.FIELD_MAP_GROUPS = Form1
    Set mvarFIELD_MAP_GROUPS = vData
End Property


'##ModelId=3778F47C00FA
Public Property Get FIELD_MAP_GROUPS() As FieldMapGroups
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FIELD_MAP_GROUPS
    Set FIELD_MAP_GROUPS = mvarFIELD_MAP_GROUPS
End Property



'Public Property Set TABLE_GROUPS(ByVal vData As TableGroups)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.TABLE_GROUPS = Form1
   ' Set mvarTABLE_GROUPS = vData
'End Property


'Public Property Get TABLE_GROUPS() As TableGroups
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_GROUPS
    'Set TABLE_GROUPS = mvarTABLE_GROUPS
'End Property



'##ModelId=3778F47C02DB
Public Property Set Form_Groups(ByVal vData As FormGroups)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.FORM_GROUPS = Form1
    Set mvarFORM_GROUPS = vData
End Property


'##ModelId=3778F47D0282
Public Property Get Form_Groups() As FormGroups
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FORM_GROUPS
    Set Form_Groups = mvarFORM_GROUPS
End Property



'##ModelId=3778F47E008F
Public Property Set Chem_Connection_Names(ByVal vData As ChemConnGroups)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.CHEM_CONNECTION_NAMES = Form1
    Set mvarCHEM_CONNECTION_NAMES = vData
End Property


'##ModelId=3778F47F007D
Public Property Get Chem_Connection_Names() As ChemConnGroups
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CHEM_CONNECTION_NAMES
    Set Chem_Connection_Names = mvarCHEM_CONNECTION_NAMES
End Property



'##ModelId=3778F47F0299
Public Property Set ADO_Connection_Names(ByVal vData As ADOConnGroups)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.ADO_CONNECTION_NAMES = Form1
    Set mvarADO_CONNECTION_NAMES = vData
End Property


'##ModelId=3778F480031D
Public Property Get ADO_Connection_Names() As ADOConnGroups
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ADO_CONNECTION_NAMES
    Set ADO_Connection_Names = mvarADO_CONNECTION_NAMES
End Property



'##ModelId=3778F4810170
Public Property Set Subform_View_Names(ByVal vData As SubFormGroups)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.SUBFORM_VIEW_NAMES = Form1
    Set mvarSUBFORM_VIEW_NAMES = vData
End Property


'##ModelId=3778F48201B7
Public Property Get Subform_View_Names() As SubFormGroups
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SUBFORM_VIEW_NAMES
    Set Subform_View_Names = mvarSUBFORM_VIEW_NAMES
End Property



'##ModelId=3778F483001E
Public Property Set TABLE_ALIASES(ByVal vData As TableAliases)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.TABLE_ALIASES = Form1
    Set mvarTABLE_ALIASES = vData
End Property


'##ModelId=3778F4840098
Public Property Get TABLE_ALIASES() As TableAliases
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_ALIASES
    Set TABLE_ALIASES = mvarTABLE_ALIASES
End Property



'##ModelId=3778F484030F
Public Property Let DB_TYPE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DB_TYPE = 5
    mvarDB_TYPE = vData
End Property


'##ModelId=3778F48503BA
Public Property Get DB_TYPE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DB_TYPE
    DB_TYPE = mvarDB_TYPE
End Property



'##ModelId=3778F4860267
Public Property Let DB_RECORD_COUNT(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DB_RECORD_COUNT = 5
    mvarDB_RECORD_COUNT = vData
End Property


'##ModelId=3778F4870345
Public Property Get DB_RECORD_COUNT() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DB_RECORD_COUNT
    DB_RECORD_COUNT = mvarDB_RECORD_COUNT
End Property



'##ModelId=3778F4880210
Public Property Let MAIN_PAGE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.MAIN_PAGE = 5
    mvarMAIN_PAGE = vData
End Property


'##ModelId=3778F48A0014
Public Property Get MAIN_PAGE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.MAIN_PAGE
    MAIN_PAGE = mvarMAIN_PAGE
End Property



'##ModelId=3778F48A02DB
Public Property Let MAXHITS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.MAXHITS = 5
   mvarMAXHITS = vData
End Property


'##ModelId=3778F48C0049
Public Property Get MAXHITS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.MAXHITS
    MAXHITS = mvarMAXHITS
End Property



'##ModelId=3778F48C036A
Public Property Let DISPLAY_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DISPLAY_NAME = 5
    mvarDISPLAY_NAME = vData
End Property


'##ModelId=3778F48E0178
Public Property Get DISPLAY_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DISPLAY_NAME
    DISPLAY_NAME = mvarDISPLAY_NAME
End Property



'##ModelId=3778F48F00A8
Public Property Let ABOUT_WINDOW(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ABOUT_WINDOW = 5
    mvarABOUT_WINDOW = vData
End Property


'##ModelId=3778F4900352
Public Property Get ABOUT_WINDOW() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ABOUT_WINDOW
    ABOUT_WINDOW = mvarABOUT_WINDOW
End Property

Public Property Get STRTableAliases() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FieldMapGroups
    STRTableAliases = mvarSTRTableAliases
End Property

Public Property Let STRTableAliases(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TableAliases  = 5
   STRTableAliases = vData
End Property



