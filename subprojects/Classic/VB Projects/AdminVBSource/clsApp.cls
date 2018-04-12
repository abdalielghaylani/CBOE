VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsApp"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Member0" ,"clsAppGlobals"
Attribute VB_Ext_KEY = "Member1" ,"DBs"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F5830044"
'
Option Base 0

'##ModelId=3778F583037A
Private mvarclsAppGlobals As clsAppGlobals
'##ModelId=3778F58303CA
Private mvarDBs As DBs


'##ModelId=3778F5840385
Public Property Get DBs() As DBs
    If mvarDBs Is Nothing Then
        Set mvarDBs = New DBs
    End If


    Set DBs = mvarDBs
End Property


'##ModelId=3778F584001E
Public Property Set DBs(vData As DBs)
    Set mvarDBs = vData
End Property



'##ModelId=3778F585011A
Private Sub Class_Initialize()
    'create the mclsAppGlobals object when the clsApp class is created
    Set mvarclsAppGlobals = New clsAppGlobals
End Sub


'##ModelId=3778F5860215
Public Property Get clsAppGlobals() As clsAppGlobals
    Set clsAppGlobals = mvarclsAppGlobals
End Property


'##ModelId=3778F5850214
Public Property Set clsAppGlobals(vData As clsAppGlobals)
    Set mvarclsAppGlobals = vData
End Property
'##ModelId=3778F58603CE
Private Sub Class_Terminate()
  Set mvarDBs = Nothing
    Set mvarclsAppGlobals = Nothing
End Sub



