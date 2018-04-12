VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsADOConnGroup"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F467014A"
'
Option Base 0

'local variable(s) to hold property value(s)
'##ModelId=3778F4680047
Private mvarCONN_TYPE As String
'##ModelId=3778F46801B0
Private mvarCONNECTION_STRING As String
'##ModelId=3778F468034B
Private mvarDSN_NAME As String
'##ModelId=3778F468034B
Private mvarDB_PATH As String
'##ModelId=3778F468034B
Private mvarDB_DRIVER As String
'##ModelId=3778F468034B
Private mvarCONNECTION_TIMEOUT As String
'##ModelId=3778F4690107
Private mvarCOMMAND_TIMEOUT As String
'##ModelId=3778F46902C0
Private mvarCONNECTION_USERNAME As String
'##ModelId=3778F46A00A4
Private mvarCONNECTION_PASSWORD As String
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
'##ModelId=3778F499037D
Public Property Get DSN_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DSN_NAME
    DSN_NAME = mvarDSN_NAME
End Property


'##ModelId=3778F49A01A8
Public Property Let DSN_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DSN_NAME = 5
    mvarDSN_NAME = vData
End Property
Public Property Get DB_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DB_PATH
    DB_PATH = mvarDB_PATH
End Property


'##ModelId=3778F49A01A8
Public Property Let DB_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DB_PATH = 5
    mvarDB_PATH = vData
End Property

Public Property Get DB_DRIVER() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.DB_DRIVER
    DB_DRIVER = mvarDB_DRIVER
End Property


'##ModelId=3778F49A01A8
Public Property Let DB_DRIVER(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.DB_DRIVER = 5
    mvarDB_DRIVER = vData
End Property
'##ModelId=3778F7FA0038
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

'##ModelId=3778F7F90163
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub





'##ModelId=3778F46A0285
Public Property Let CONNECTION_USERNAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.CONNECTION_USERNAME = 5
    mvarCONNECTION_USERNAME = vData
End Property


'##ModelId=3778F46B02D7
Public Property Get CONNECTION_USERNAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CONNECTION_USERNAME
    CONNECTION_USERNAME = mvarCONNECTION_USERNAME
End Property



'##ModelId=3778F46C0148
Public Property Let COMMAND_TIMEOUT(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.COMMAND_TIMEOUT = 5
    mvarCOMMAND_TIMEOUT = vData
End Property


'##ModelId=3778F46D01E9
Public Property Get COMMAND_TIMEOUT() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.COMMAND_TIMEOUT
    COMMAND_TIMEOUT = mvarCOMMAND_TIMEOUT
End Property



'##ModelId=3778F46E0064
Public Property Let CONNECTION_TIMEOUT(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.CONNECTION_TIMEOUT = 5
    mvarCONNECTION_TIMEOUT = vData
End Property


'##ModelId=3778F46F0124
Public Property Get CONNECTION_TIMEOUT() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CONNECTION_TIMEOUT
    CONNECTION_TIMEOUT = mvarCONNECTION_TIMEOUT
End Property



'##ModelId=3778F46F03AF
Public Property Let CONNECTION_STRING(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.CONNECTION_STRING = 5
    mvarCONNECTION_STRING = vData
End Property


'##ModelId=3778F47100C3
Public Property Get CONNECTION_STRING() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CONNECTION_STRING
    CONNECTION_STRING = mvarCONNECTION_STRING
End Property





'##ModelId=3778F473018E
Public Property Get CONN_TYPE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CONN_TYPE
    CONN_TYPE = mvarCONN_TYPE
End Property

'##ModelId=3778F4720060
Public Property Let CONN_TYPE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.CONN_TYPE = 5
    mvarCONN_TYPE = vData
End Property

'##ModelId=3778F4740077
Public Property Let CONNECTION_PASSWORD(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.CONNECTION_PASSWORD = 5
    mvarCONNECTION_PASSWORD = vData
End Property


'##ModelId=3778F47501EB
Public Property Get CONNECTION_PASSWORD() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.CONNECTION_PASSWORD
    CONNECTION_PASSWORD = mvarCONNECTION_PASSWORD
End Property
