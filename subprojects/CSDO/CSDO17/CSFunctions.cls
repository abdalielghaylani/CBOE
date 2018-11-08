VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSFunctions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private Const NumCSFunctionTypes = 10

Private moCol As Collection

Public Property Get Item(avIndexKey As Variant) As CSFunction
    Set Item = moCol(avIndexKey)
End Property

Public Property Get NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = moCol.[_NewEnum]
End Property

Public Property Get count() As Long
    count = moCol.count
End Property

Private Sub Class_Initialize()
    'creates the collection when this class is created
    Set moCol = New Collection
End Sub

Private Sub Class_Terminate()
    'destroy class objects when this class is terminated
    Set moCol = Nothing
End Sub

Friend Function Add(aType As CSFunctionType) As CSFunction
    ' Add a new project to the collection
    Dim toNewMember As CSFunction
        
    ' Add a new key to the collection
    Set toNewMember = New CSFunction
    With toNewMember
        .FunctionType = aType
    End With
    moCol.Add toNewMember, toNewMember.Name
    
    'return the object created
    Set Add = toNewMember
    Set toNewMember = Nothing
    Exit Function
End Function

Friend Sub MakeDefaultSet()
     ' fills collection with a complete set of functions
     Dim count As Long
     For count = 0 To NumCSFunctionTypes - 1
        Me.Add count
     Next
End Sub









