VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsSubFormGroup"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F500014A"
'
Option Base 0
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

'##ModelId=3778F80F022D
Private Sub Class_Terminate()
    #If DebugMode Then
    'the class is being destroyed
    Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " is terminating"
    #End If
End Sub


'##ModelId=3778F80F0088
Private Sub Class_Initialize()
    #If DebugMode Then
        'get the next available class ID, and print out
        'that the class was created successfully
        mlClassDebugID = GetNextClassDebugID()
        Debug.Print "'" & TypeName(Me) & "' instance " & CStr(mlClassDebugID) & " created"
    #End If
End Sub
