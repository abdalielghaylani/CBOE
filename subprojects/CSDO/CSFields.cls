VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSFields"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
    Option Explicit

Private moCol As Collection

Private Const NumCSFieldTypes = 8                    ' number of defined CSField Types

Public Property Get Item(avIndexKey As Variant) As CSField
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

Friend Function Add(aCSFieldType As CSFieldType) As CSField
    ' Add a new project to the collection
    Dim toNewMember As CSField
        
    ' Add a new key to the collection
    Set toNewMember = New CSField
    With toNewMember
        .fieldtype = aCSFieldType
    End With
    moCol.Add toNewMember, toNewMember.DefaultName
    
    'return the object created
    Set Add = toNewMember
    Set toNewMember = Nothing
    Exit Function
        
End Function

Public Function AddFromExisting(aCSField As CSField) As CSField
    Dim toNewMember As CSField
        
    ' Add a new key to the collection
    Set toNewMember = aCSField
    moCol.Add toNewMember, toNewMember.DefaultName
    
    'return the object created
    Set AddFromExisting = toNewMember
    Set toNewMember = Nothing
End Function

Friend Sub MakeDefaultSet()
     ' fills collection with a complete set of fields
     Dim count As Long
     For count = 0 To NumCSFieldTypes - 1
        Add count
     Next
End Sub

Public Function Clear() As Boolean
    Do Until moCol.count = 0
        moCol.Remove (1)
    Loop
End Function










