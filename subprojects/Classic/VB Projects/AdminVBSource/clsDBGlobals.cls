VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsDBGlobals"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F47700CB"
'
Option Base 0

Private mvarABOUT_WINDOW As String
Private mvarDISPLAY_NAME As String
Private mvarMAXHITS As String
Private mvarMAIN_PAGE As String
Private mvarDB_RECORD_COUNT As String
Private mvarDB_TYPE As String
Private mvarTABLE_ALIASES As TableAliases
Private mvarSUBFORM_VIEW_NAMES As SubFormGroups
Private mvarADO_CONNECTION_NAMES As ADOConnGroups
Private mvarCHEM_CONNECTION_NAMES As ChemConnGroups
Private mvarFORM_GROUPS As FormGroups
'Private mvarTABLE_GROUPS As TableGroups 'local copy
Private mvarFIELD_MAP_GROUPS As FieldMapGroups

Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
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



