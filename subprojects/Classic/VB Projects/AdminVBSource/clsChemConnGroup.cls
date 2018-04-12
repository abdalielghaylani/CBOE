VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsChemConnGroup"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F49601AC"
'
Option Base 0

'local variable(s) to hold property value(s)
'##ModelId=3778F4970045

Private mvarSTRUC_ENGINE As String
'##ModelId=3778F49701EA
Private mvarSTRUC_FORM_NAME As String
'##ModelId=3778F498001E
Private mvarSTRUC_DB_PATH As String
'##ModelId=3778F49801C3
Private mvarSTRUC_TABLE_NAME As String
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
'##ModelId=3778F8010381
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub

'##ModelId=3778F80100EC
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub
'##ModelId=3778F499037D
Public Property Get STRUC_TABLE_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUC_TABLE_NAME
    STRUC_TABLE_NAME = mvarSTRUC_TABLE_NAME
End Property
Public Property Let STRUC_TABLE_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUC_TABLE_NAME = 5
    mvarSTRUC_TABLE_NAME = vData
End Property

Public Property Let STRUC_DB_PATH(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUC_DB_PATH = 5
    mvarSTRUC_DB_PATH = vData
End Property



'##ModelId=3778F49B01B3
Public Property Get STRUC_DB_PATH() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUC_DB_PATH
    STRUC_DB_PATH = mvarSTRUC_DB_PATH
End Property



'##ModelId=3778F49B03DA
Public Property Let STRUC_FORM_NAME(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUC_FORM_NAME = 5
    mvarSTRUC_FORM_NAME = vData
End Property


'##ModelId=3778F49D003A
Public Property Get STRUC_FORM_NAME() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUC_FORM_NAME
    STRUC_FORM_NAME = mvarSTRUC_FORM_NAME
End Property



'##ModelId=3778F49D027E
Public Property Let STRUC_ENGINE(ByVal vData As String)
'used when assigning a value to the property, on the left side of an assignment.
'Syntax: X.STRUC_ENGINE = 5
    mvarSTRUC_ENGINE = vData
End Property


'##ModelId=3778F49E02F8
Public Property Get STRUC_ENGINE() As String
'used when retrieving value of a property, on the right side of an assignment.
'Syntax: Debug.Print X.STRUC_ENGINE
    STRUC_ENGINE = mvarSTRUC_ENGINE
End Property



