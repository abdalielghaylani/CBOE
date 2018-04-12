VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsTableGroup"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F5010079"
'
Option Base 0

'local variable(s) to hold property value(s)
'##ModelId=3778F50102D2
Private mvarBASE_TABLE As String
'##ModelId=3778F5020111
Private mvarMOLECULE_TABLE As String
'##ModelId=3778F502028E
Private mvarTABLE_SQL_ORDER As String
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

'##ModelId=3778F81100D1
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

'##ModelId=3778F8100242
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub
'##ModelId=3778F503002C
Public Property Let Table_SQL_Order(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.TABLE_SQL_ORDER = 5
    mvarTABLE_SQL_ORDER = vData
End Property


'##ModelId=3778F503039E
Public Property Get Table_SQL_Order() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.TABLE_SQL_ORDER
    Table_SQL_Order = mvarTABLE_SQL_ORDER
End Property



'##ModelId=3778F5040196
Public Property Let MOLECULE_TABLE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.MOLECULE_TABLE = 5
    mvarMOLECULE_TABLE = vData
End Property


'##ModelId=3778F5050152
Public Property Get MOLECULE_TABLE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.MOLECULE_TABLE
    MOLECULE_TABLE = mvarMOLECULE_TABLE
End Property



'##ModelId=3778F5050350
Public Property Let Base_Table(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.BASE_TABLE = 5
    mvarBASE_TABLE = vData
End Property


'##ModelId=3778F5060398
Public Property Get Base_Table() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.BASE_TABLE
    Base_Table = mvarBASE_TABLE
End Property



