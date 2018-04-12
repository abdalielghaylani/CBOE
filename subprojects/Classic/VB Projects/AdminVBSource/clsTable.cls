VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsTableAlias"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F5080142"
'
Option Base 0

'##ModelId=3778F5080373
Private mvarTABLE_ALIAS_NAME As String

'##ModelId=3778F50900D5
Private mvarTABLE_NAME As String
'##ModelId=3778F5090234
Private mvarRELATIONAL_FIELDS As String
'##ModelId=3778F509039C
Private mvarPRIMARY_KEY As String
'##ModelId=3778F50A013B
Private mvarSQL_SYNTAX As String
'##ModelId=3778F50A02CB
Private mvarSELECT_KEYWORD As String
'##ModelId=3778F50B007E
Private mvarSELECT_ADDITIONAL As String
'##ModelId=3778F50B0237
Private mvarSELECT_JOIN As String
'##ModelId=3778F50C00D9
Private mvarSELECT_LINKS As String
'##ModelId=3778F50C02B0
Private mvarINTER_TABLES As String
'##ModelId=3778F50D00B3
Private mvarADO_CONNECTION As String
'##ModelId=3778F50D02A8
Private mvarCHEM_CONNECTION As String
'##ModelId=3778F50E00C8
Private mvarSTRUC_FIELD_ID As String
'##ModelId=3778F50E02DB
Public theTableStuff As Collection
Private mvarGroupName As String

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

'##ModelId=3778F81500CD
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

'##ModelId=3778F814005D
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub

'##ModelId=3778F50F00DE
Public Property Let STRUC_FIELD_ID(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUC_FIELD = 5
    mvarSTRUC_FIELD_ID = vData
End Property


'##ModelId=3778F5100189
Public Property Get STRUC_FIELD_ID() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUC_FIELD
    STRUC_FIELD_ID = mvarSTRUC_FIELD_ID
End Property



'##ModelId=3778F51003E2
Public Property Let CHEM_CONNECTION(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.CHEM_CONNECTION = 5
    mvarCHEM_CONNECTION = vData
End Property


'##ModelId=3778F5120088
Public Property Get CHEM_CONNECTION() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CHEM_CONNECTION
    CHEM_CONNECTION = mvarCHEM_CONNECTION
End Property



'##ModelId=3778F5120309
Public Property Let ADO_CONNECTION(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.ADO_CONNECTION = 5
    mvarADO_CONNECTION = vData
End Property


'##ModelId=3778F51303D3
Public Property Get ADO_CONNECTION() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.ADO_CONNECTION
    ADO_CONNECTION = mvarADO_CONNECTION
End Property



'##ModelId=3778F5140280
Public Property Let INTER_TABLES(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.INTER_TABLES = 5
    mvarINTER_TABLES = vData
End Property


'##ModelId=3778F515037B
Public Property Get INTER_TABLES() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.INTER_TABLES
    INTER_TABLES = mvarINTER_TABLES
End Property



'##ModelId=3778F5160250
Public Property Let SELECT_LINKS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SELECT_LINKS = 5
    mvarSELECT_LINKS = vData
End Property


'##ModelId=3778F5170388
Public Property Get SELECT_LINKS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SELECT_LINKS
    SELECT_LINKS = mvarSELECT_LINKS
End Property



'##ModelId=3778F518027B
Public Property Let SELECT_JOIN(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SELECT_JOIN = 5
    mvarSELECT_JOIN = vData
End Property


'##ModelId=3778F51A00D0
Public Property Get SELECT_JOIN() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SELECT_JOIN
    SELECT_JOIN = mvarSELECT_JOIN
End Property



'##ModelId=3778F51A03C9
Public Property Let SELECT_ADDITIONAL(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SELECT_ADDITIONAL = 5
    mvarSELECT_ADDITIONAL = vData
End Property


'##ModelId=3778F51C0191
Public Property Get SELECT_ADDITIONAL() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SELECT_ADDITIONAL
    SELECT_ADDITIONAL = mvarSELECT_ADDITIONAL
End Property



'##ModelId=3778F51D00CA
Public Property Let SELECT_KEYWORD(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SELECT_KEYWORD = 5
    mvarSELECT_KEYWORD = vData
End Property


'##ModelId=3778F51E0310
Public Property Get SELECT_KEYWORD() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SELECT_KEYWORD
    SELECT_KEYWORD = mvarSELECT_KEYWORD
End Property



'##ModelId=3778F51F0267
Public Property Let SQL_SYNTAX(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.SQL_SYNTAX = 5
    mvarSQL_SYNTAX = vData
End Property


'##ModelId=3778F5210166
Public Property Get SQL_SYNTAX() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.SQL_SYNTAX
    SQL_SYNTAX = mvarSQL_SYNTAX
End Property



'##ModelId=3778F52200DB
Public Property Let PRIMARY_KEY(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.PRIMARY_KEY = 5
    mvarPRIMARY_KEY = vData
End Property


'##ModelId=3778F5230335
Public Property Get PRIMARY_KEY() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.PRIMARY_KEY
    PRIMARY_KEY = mvarPRIMARY_KEY
End Property



'##ModelId=3778F524039B
Public Property Let RELATIONAL_FIELDS(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.RELATIONAL_FIELDS = 5
    mvarRELATIONAL_FIELDS = vData
End Property


'##ModelId=3778F5260235
Public Property Get RELATIONAL_FIELDS() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.RELATIONAL_FIELDS
    RELATIONAL_FIELDS = mvarRELATIONAL_FIELDS
End Property



'##ModelId=3778F52701E7
Public Property Let TABLE_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TABLE_NAME = 5
    mvarTABLE_NAME = vData
End Property


'##ModelId=3778F5290121
Public Property Get TABLE_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_NAME
    TABLE_NAME = mvarTABLE_NAME
End Property

'##ModelId=3778F52A00FB
Public Property Let TABLE_ALIAS_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TABLE_ALIAS_NAME = 5
    mvarTABLE_ALIAS_NAME = vData
End Property


'##ModelId=3778F52C002B
Public Property Get TABLE_ALIAS_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_ALIAS_NAME
    TABLE_ALIAS_NAME = mvarTABLE_ALIAS_NAME
End Property



