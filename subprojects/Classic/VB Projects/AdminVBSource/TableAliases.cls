VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "TableAliases"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "Collection" ,"clsTable"
Attribute VB_Ext_KEY = "Member0" ,"TableGroups"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F52E022D"
'
Option Base 0

'local variable to hold collection
'##ModelId=3778F52F0292
Private mCol As Collection
Private mvarTableGroups As TableGroups


Public Property Get TableGroups() As TableGroups
    If mvarTableGroups Is Nothing Then
        Set mvarTableGroups = New TableGroups
    End If


    Set TableGroups = mvarTableGroups
End Property


Public Property Set TableGroups(vData As TableGroups)
    Set mvarTableGroups = vData
End Property

'##ModelId=3778F52F03DD
Public Function Add(ByVal objNewMember As clsTableAlias, Optional ByVal Key As String) As Variant
    'create a new object
    'Dim objNewMember As clsTableAlias
   ' Set objNewMember = New clsTableAlias


    'set the properties passed into the method
    If Len(Key) = 0 Then
        mCol.Add objNewMember
    Else
        mCol.Add objNewMember, Key
    End If


    'return the object created
    Set Add = objNewMember
    Set objNewMember = Nothing


End Function

'##ModelId=3778F53003B6
Public Property Get Item(vntIndexKey As Variant) As clsTableAlias
Attribute Item.VB_UserMemId = 0
    'used when referencing an element in the collection
    'vntIndexKey contains either the Index or Key to the collection,
    'this is why it is declared as a Variant
    'Syntax: Set foo = x.Item(xyz) or Set foo = x.Item(5)
  Set Item = mCol(vntIndexKey)
End Property



'##ModelId=3778F53102DB
Public Property Get Count() As Long
    'used when retrieving the number of elements in the
    'collection. Syntax: Debug.Print x.Count
    Count = mCol.Count
End Property


'##ModelId=3778F532017E
Public Sub Remove(vntIndexKey As Variant)
    'used when removing an element from the collection
    'vntIndexKey contains either the Index or Key, which is why
    'it is declared as a Variant
    'Syntax: x.Remove(xyz)


    mCol.Remove vntIndexKey
End Sub


'##ModelId=3778F533000D
Public Property Get NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
    'this property allows you to enumerate
    'this collection with the For...Each syntax
    Set NewEnum = mCol.[_NewEnum]
End Property


'##ModelId=3778F53301D0
Private Sub Class_Initialize()
    'creates the collection when this class is created
    Set mCol = New Collection
End Sub


'##ModelId=3778F53302F2
Private Sub Class_Terminate()
  Set mvarTableGroups = Nothing
    'destroys collection when this class is terminated
    Set mCol = Nothing
End Sub

