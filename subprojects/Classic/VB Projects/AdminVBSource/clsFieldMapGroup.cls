VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsFieldMapGroup"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F49F035E"
'
Option Base 0

'local variable(s) to hold property value(s)
'##ModelId=3778F4A001C4
Private mvarSTRUC_FIELD_MAP As String
'##ModelId=3778F4A00369
Private mvarMW_FIELD_MAP As String
'##ModelId=3778F4A100F4
Private mvarFORMULA_FIELD_MAP As String
'##ModelId=3778F4A10270
Private mvarNUM_MAP_ENTRIES As String
'##ModelId=3778F4A200E1
Private mvarOTHER_FIELD_MAP1 As String
Private mvarOTHER_FIELD_MAP2 As String
Private mvarOTHER_FIELD_MAP3 As String
Private mvarOTHER_FIELD_MAP4 As String
Private mvarOTHER_FIELD_MAP5 As String
Private mvarOTHER_FIELD_MAP6 As String

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

'##ModelId=3778F80402A9
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

'##ModelId=3778F804001E
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub
'##ModelId=3778F4A20286
Public Property Let OTHER_FIELD_MAP6(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.OTHER_FIELD_MAP6 = 5
    mvarOTHER_FIELD_MAP6 = vData
End Property


'##ModelId=3778F4A30223
Public Property Get OTHER_FIELD_MAP6() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.OTHER_FIELD_MAP1
    OTHER_FIELD_MAP6 = mvarOTHER_FIELD_MAP6
End Property
'##ModelId=3778F4A20286
Public Property Let OTHER_FIELD_MAP5(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.OTHER_FIELD_MAP3 = 5
    mvarOTHER_FIELD_MAP5 = vData
End Property


'##ModelId=3778F4A30223
Public Property Get OTHER_FIELD_MAP5() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.OTHER_FIELD_MAP1
    OTHER_FIELD_MAP5 = mvarOTHER_FIELD_MAP5
End Property
'##ModelId=3778F4A20286
Public Property Let OTHER_FIELD_MAP4(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.OTHER_FIELD_MAP3 = 5
    mvarOTHER_FIELD_MAP4 = vData
End Property


'##ModelId=3778F4A30223
Public Property Get OTHER_FIELD_MAP4() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.OTHER_FIELD_MAP1
    OTHER_FIELD_MAP4 = mvarOTHER_FIELD_MAP4
End Property
'##ModelId=3778F4A20286
Public Property Let OTHER_FIELD_MAP3(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.OTHER_FIELD_MAP3 = 5
    mvarOTHER_FIELD_MAP3 = vData
End Property


'##ModelId=3778F4A30223
Public Property Get OTHER_FIELD_MAP3() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.OTHER_FIELD_MAP1
    OTHER_FIELD_MAP3 = mvarOTHER_FIELD_MAP3
End Property
'##ModelId=3778F4A20286
Public Property Let OTHER_FIELD_MAP1(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.OTHER_FIELD_MAP1 = 5
    mvarOTHER_FIELD_MAP1 = vData
End Property


'##ModelId=3778F4A30223
Public Property Get OTHER_FIELD_MAP1() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.OTHER_FIELD_MAP1
    OTHER_FIELD_MAP1 = mvarOTHER_FIELD_MAP1
End Property
'##ModelId=3778F4A20286
Public Property Let OTHER_FIELD_MAP2(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.OTHER_FIELD_MAP2 = 5
    mvarOTHER_FIELD_MAP2 = vData
End Property


'##ModelId=3778F4A30223
Public Property Get OTHER_FIELD_MAP2() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.OTHER_FIELD_MAP2
    OTHER_FIELD_MAP2 = mvarOTHER_FIELD_MAP2
End Property
'##ModelId=3778F4A40026
Public Property Let NUM_MAP_ENTRIES(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.NUM_MAP_ENTRIES = 5
    mvarNUM_MAP_ENTRIES = vData
End Property


'##ModelId=3778F4A403E7
Public Property Get NUM_MAP_ENTRIES() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.NUM_MAP_ENTRIES
    NUM_MAP_ENTRIES = mvarNUM_MAP_ENTRIES
End Property



'##ModelId=3778F4A50208
Public Property Let FORMULA_FIELD_MAP(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.FORMULA_FIELD = 5
    mvarFORMULA_FIELD_MAP = vData
End Property


'##ModelId=3778F4A60277
Public Property Get FORMULA_FIELD_MAP() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.FORMULA_FIELD
    FORMULA_FIELD_MAP = mvarFORMULA_FIELD_MAP
End Property



'##ModelId=3778F4A700B6
Public Property Let MW_FIELD_MAP(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.MW_FIELD_MAP = 5
    mvarMW_FIELD_MAP = vData
End Property


'##ModelId=3778F4A801C6
Public Property Get MW_FIELD_MAP() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.MW_FIELD_MAP
    MW_FIELD_MAP = mvarMW_FIELD_MAP
End Property



'##ModelId=3778F4A90023
Public Property Let STRUC_FIELD_MAP(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUC_FIELD_MAP = 5
    mvarSTRUC_FIELD_MAP = vData
End Property


'##ModelId=3778F4AA00A6
Public Property Get STRUC_FIELD_MAP() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUC_FIELD_MAP
    STRUC_FIELD_MAP = mvarSTRUC_FIELD_MAP
End Property



