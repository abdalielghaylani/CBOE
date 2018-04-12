VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 1  'Persistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "FormGroups"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder6" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"No"
Attribute VB_Ext_KEY = "Collection" ,"clsFormGroup"
Attribute VB_Ext_KEY = "Member0" ,"clsFormGroup"
Attribute VB_Ext_KEY = "RVB_UniqueId" ,"3778F549020E"
'
Option Base 0

'local variable to hold collection
'##ModelId=3778F54A0350
Private mCol As Collection
'##ModelId=3778F54B00C6
Public Function Add(ByVal objNewMember As clsFormGroup, Optional ByVal Key As String) As Variant
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

'##ModelId=3778F54C00B3
Public Property Get Item(vntIndexKey As Variant) As clsFormGroup
    'used when referencing an element in the collection
    'vntIndexKey contains either the Index or Key to the collection,
    'this is why it is declared as a Variant
    'Syntax: Set foo = x.Item(xyz) or Set foo = x.Item(5)
  Set Item = mCol(vntIndexKey)
End Property



'##ModelId=3778F54C03DF
Public Property Get Count() As Long
    'used when retrieving the number of elements in the
    'collection. Syntax: Debug.Print x.Count
    Count = mCol.Count
End Property


'##ModelId=3778F54D01B9
Public Sub Remove(vntIndexKey As Variant)
    'used when removing an element from the collection
    'vntIndexKey contains either the Index or Key, which is why
    'it is declared as a Variant
    'Syntax: x.Remove(xyz)


    mCol.Remove vntIndexKey
End Sub


'##ModelId=3778F54E00C0
Public Property Get NewEnum() As IUnknown
    'this property allows you to enumerate
    'this collection with the For...Each syntax
    Set NewEnum = mCol.[_NewEnum]
End Property


'##ModelId=3778F54E02F1
Private Sub Class_Initialize()
    'creates the collection when this class is created
    Set mCol = New Collection
End Sub


'##ModelId=3778F54F0036
Private Sub Class_Terminate()
    'destroys collection when this class is terminated
    Set mCol = Nothing
End Sub


