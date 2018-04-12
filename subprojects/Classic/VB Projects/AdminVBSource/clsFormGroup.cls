VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsFormGroup"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F4AC006D"
'
Option Base 0

'local variable(s) to hold property value(s)
'##ModelId=3778F4AC02BC
Private mvarINPUT_FORM_PATH As String
'##ModelId=3778F4AD001F
Private mvarINPUT_FORM_MODE As String
'##ModelId=3778F4AD0191
Private mvarRESULT_FORM_PATH As String
'##ModelId=3778F4AD0318
Private mvarRESULT_FORM_MODE As String
'##ModelId=3778F4AE00B6
Private mvarPLUGIN_VALUE As String
'##ModelId=3778F4AE0265
Private mvarSTRUCTURE_FIELDS As String
'##ModelId=3778F4AF002B
Private mvarMW_FIELDS As String
'##ModelId=3778F4AF01EE
Private mvarFORMULA_FIELDS As String
'##ModelId=3778F4B00041
Private mvarSEARCHABLE_ADO_FIELDS As String
'##ModelId=3778F4B00236
Private mvarREQUIRED_FIELDS As String
'##ModelId=3778F4B10042
Private mvarSDFILE_FIELDS As String
'##ModelId=3778F4B1024B
Private mvarFIELD_MAP_GROUP As String
'##ModelId=3778F4B20076
Private mvarFORM_GROUP_FLAG As String
'##ModelId=3778F4B202A7
Private mvarTABLE_GROUP As String
'##ModelId=3778F4B300F0
Private mvarNUM_LIST_VIEW As String
'##ModelId=3778F4B30334

Private mvarTABLE_ORDER_FULL_COMMIT As String
'##ModelId=3778F4B50034
Private mvarTABLE_ORDER_PARTIAL_COMMIT As String
'##ModelId=3778F4B502B5
Private mvarREG_COMMIT_BASETABLE As String
'##ModelId=3778F4B6014E
Private mvarREG_COMMIT_MOLTABLE As String
'##ModelId=3778F4B70005
Private mvarREG_TEMP_BASETABLE As String
'##ModelId=3778F4B702B8
Private mvarREG_TEMP_MOLTABLE As String
Private mvarGroupName As String
Private mvarINDEX_DB As String




'##ModelId=3778F499037D
Public Property Get INDEX_DB() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.INDEX_DB
    INDEX_DB = mvarINDEX_DB
End Property


'##ModelId=3778F49A01A8
Public Property Let INDEX_DB(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.INDEX_DB = 5
    mvarINDEX_DB = vData
End Property



'##ModelId=3778F499037D
Public Property Get GroupName() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GroupName
    GroupName = mvarGroupName
End Property


'##ModelId=3778F49A01A8
Public Property Let GroupName(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GroupName = 5
    mvarGroupName = vData
End Property

'##ModelId=3778F80A02A8
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

'##ModelId=3778F809015C
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub

'##ModelId=3778F4B80183
Public Property Let REG_TEMP_MOLTABLE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.REG_TEMP_MOLTABLE  = 5
   mvarREG_TEMP_MOLTABLE = vData
End Property


'##ModelId=3778F4B902D9
Public Property Get REG_TEMP_MOLTABLE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.REG_TEMP_MOLTABLE
    REG_TEMP_MOLTABLE = mvarREG_TEMP_MOLTABLE
End Property

'##ModelId=3778F4BA0294
Public Property Let REG_TEMP_BASETABLE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.REG_COMMIT_MOLTABLE  = 5
   mvarREG_TEMP_BASETABLE = vData
End Property


'##ModelId=3778F4BC0034
Public Property Get REG_TEMP_BASETABLE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.REG_TEMP_BASETABLE
    REG_TEMP_BASETABLE = mvarREG_TEMP_BASETABLE
End Property

'##ModelId=3778F4BC032D
Public Property Let REG_COMMIT_MOLTABLE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.REG_COMMIT_MOLTABLE  = 5
    mvarREG_COMMIT_MOLTABLE = vData
End Property


'##ModelId=3778F4BE016E
Public Property Get REG_COMMIT_MOLTABLE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.REG_COMMIT_MOLTABLE
    REG_COMMIT_MOLTABLE = mvarREG_COMMIT_MOLTABLE
End Property
'##ModelId=3778F4BF00C5
Public Property Let REG_COMMIT_BASETABLE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.REG_COMMIT_BASETABLE  = 5
    mvarREG_COMMIT_BASETABLE = vData
End Property


'##ModelId=3778F4C003A1
Public Property Get REG_COMMIT_BASETABLE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.REG_COMMIT_BASETABLE
    REG_COMMIT_BASETABLE = mvarREG_COMMIT_BASETABLE
End Property




'##ModelId=3778F4C10302
Public Property Let TABLE_ORDER_PARTIAL_COMMIT(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TABLE_ORDER_FULL_COMMIT  = 5
    mvarTABLE_ORDER_PARTIAL_COMMIT = vData
End Property


'##ModelId=3778F4C30157
Public Property Get TABLE_ORDER_PARTIAL_COMMIT() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_ORDER_FULL_COMMIT
    TABLE_ORDER_PARTIAL_COMMIT = mvarTABLE_ORDER_PARTIAL_COMMIT
End Property




'##ModelId=3778F4C401BC
Public Property Let TABLE_ORDER_FULL_COMMIT(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TABLE_ORDER_FULL_COMMIT  = 5
    mvarTABLE_ORDER_FULL_COMMIT = vData
End Property


'##ModelId=3778F4C6004D
Public Property Get TABLE_ORDER_FULL_COMMIT() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_ORDER_FULL_COMMIT
    TABLE_ORDER_FULL_COMMIT = mvarTABLE_ORDER_FULL_COMMIT
End Property

'##ModelId=3778F4C603D2
Public Property Let GLOBAL_SEARCH_BASE_DB(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.GLOBAL_SEARCH_BASE_DB = 5
    mvarGLOBAL_SEARCH_BASE_DB = vData
End Property


'##ModelId=3778F4C80302
Public Property Get GLOBAL_SEARCH_BASE_DB() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.GLOBAL_SEARCH_BASE_DB
    GLOBAL_SEARCH_BASE_DB = mvarGLOBAL_SEARCH_BASE_DB
End Property



'##ModelId=3778F4C902BE
Public Property Let NUM_LIST_VIEW(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.NUM_LIST_VIEW = 5
    mvarNUM_LIST_VIEW = vData
End Property


'##ModelId=3778F4CB01B2
Public Property Get NUM_LIST_VIEW() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.NUM_LIST_VIEW
    NUM_LIST_VIEW = mvarNUM_LIST_VIEW
End Property



'##ModelId=3778F4CC018C
Public Property Let TABLE_GROUP(ByVal vData As String)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.TABLE_GROUP = Form1
    mvarTABLE_GROUP = vData
End Property


'##ModelId=3778F4CE00BC
Public Property Get TABLE_GROUP() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_GROUP
    TABLE_GROUP = mvarTABLE_GROUP
End Property



'##ModelId=3778F4CF00B4
Public Property Let FORM_GROUP_FLAG(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.FORM_GROUP_FLAG = 5
    mvarFORM_GROUP_FLAG = vData
End Property


'##ModelId=3778F4D10048
Public Property Get FORM_GROUP_FLAG() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FORM_GROUP_FLAG
    FORM_GROUP_FLAG = mvarFORM_GROUP_FLAG
End Property


'##ModelId=3778F4D20144
Public Property Let FIELD_MAP_GROUP(ByVal vData As String)
'used when assigning an Object to the property, on the left side of a Set statement.
'Syntax: Set x.FIELD_MAP_GROUP = Form1
    mvarFIELD_MAP_GROUP = vData
End Property


'##ModelId=3778F4D4010B
Public Property Get FIELD_MAP_GROUP() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FIELD_MAP_GROUP
    FIELD_MAP_GROUP = mvarFIELD_MAP_GROUP
End Property



'##ModelId=3778F4D5015D
Public Property Let SDFILE_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SDFILE_FIELDS = 5
    mvarSDFILE_FIELDS = vData
End Property


'##ModelId=3778F4D701BA
Public Property Get SDFILE_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SDFILE_FIELDS
    SDFILE_FIELDS = mvarSDFILE_FIELDS
End Property



'##ModelId=3778F4D80305
Public Property Let REQUIRED_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.REQUIRED_FIELDS = 5
    mvarREQUIRED_FIELDS = vData
End Property


'##ModelId=3778F4DA0326
Public Property Get REQUIRED_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.REQUIRED_FIELDS
    REQUIRED_FIELDS = mvarREQUIRED_FIELDS
End Property



'##ModelId=3778F4DC0094
Public Property Let SEARCHABLE_ADO_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SEARCHABLE_ADO_FIELDS = 5
    mvarSEARCHABLE_ADO_FIELDS = vData
End Property


'##ModelId=3778F4DE00E7
Public Property Get SEARCHABLE_ADO_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SEARCHABLE_ADO_FIELDS
    SEARCHABLE_ADO_FIELDS = mvarSEARCHABLE_ADO_FIELDS
End Property



'##ModelId=3778F4DF0193
Public Property Let FORMULA_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.FORMULA_FIELDS = 5
    mvarFORMULA_FIELDS = vData
End Property


'##ModelId=3778F4E102AE
Public Property Get FORMULA_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FORMULA_FIELDS
    FORMULA_FIELDS = mvarFORMULA_FIELDS
End Property



'##ModelId=3778F4E20382
Public Property Let MW_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.MW_FIELDS = 5
    mvarMW_FIELDS = vData
End Property


'##ModelId=3778F4E5008D
Public Property Get MW_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.MW_FIELDS
    MW_FIELDS = mvarMW_FIELDS
End Property



'##ModelId=3778F4E6017F
Public Property Let STRUCTURE_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUCTURE_FIELDS = 5
    mvarSTRUCTURE_FIELDS = vData
End Property


'##ModelId=3778F4E8029A
Public Property Get STRUCTURE_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUCTURE_FIELDS
    STRUCTURE_FIELDS = mvarSTRUCTURE_FIELDS
End Property



'##ModelId=3778F4EA0080
Public Property Let PLUGIN_VALUE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.PLUGIN_VALUE = 5
    mvarPLUGIN_VALUE = vData
End Property


'##ModelId=3778F4EC01C4
Public Property Get PLUGIN_VALUE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.PLUGIN_VALUE
    PLUGIN_VALUE = mvarPLUGIN_VALUE
End Property



'##ModelId=3778F4ED02E8
Public Property Let RESULT_FORM_MODE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.RESULT_FORM_MODE = 5
    mvarRESULT_FORM_MODE = vData
End Property


'##ModelId=3778F4F001B6
Public Property Get RESULT_FORM_MODE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.RESULT_FORM_MODE
    RESULT_FORM_MODE = mvarRESULT_FORM_MODE
End Property



'##ModelId=3778F4F1031F
Public Property Let RESULT_FORM_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.RESULT_FORM_PATH = 5
    mvarRESULT_FORM_PATH = vData
End Property


'##ModelId=3778F4F401D9
Public Property Get RESULT_FORM_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.RESULT_FORM_PATH
    RESULT_FORM_PATH = mvarRESULT_FORM_PATH
End Property



'##ModelId=3778F4F50361
Public Property Let INPUT_FORM_MODE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.INPUT_FORM_MODE = 5
    mvarINPUT_FORM_MODE = vData
End Property


'##ModelId=3778F4F8024D
Public Property Get INPUT_FORM_MODE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.INPUT_FORM_MODE
    INPUT_FORM_MODE = mvarINPUT_FORM_MODE
End Property



'##ModelId=3778F4F903D5
Public Property Let INPUT_FORM_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.INPUT_FORM_PATH = 5
    mvarINPUT_FORM_PATH = vData
End Property


'##ModelId=3778F4FC0221
Public Property Get INPUT_FORM_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.INPUT_FORM_PATH
    INPUT_FORM_PATH = mvarINPUT_FORM_PATH
End Property



