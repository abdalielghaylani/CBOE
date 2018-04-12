VERSION 5.00
Begin VB.Form frmAddSalt 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Molecular Formula"
   ClientHeight    =   1890
   ClientLeft      =   30
   ClientTop       =   420
   ClientWidth     =   5550
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1890
   ScaleWidth      =   5550
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  'CenterScreen
   Begin VB.ComboBox mAdditive1 
      Height          =   315
      Left            =   1680
      Sorted          =   -1  'True
      TabIndex        =   1
      Text            =   "mAdditive1"
      Top             =   600
      Width           =   3015
   End
   Begin VB.TextBox mCoefficient1 
      Height          =   288
      Left            =   840
      TabIndex        =   0
      Top             =   600
      Width           =   732
   End
   Begin VB.CommandButton mCancelButton 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   375
      Left            =   2940
      TabIndex        =   3
      Top             =   1320
      Width           =   1335
   End
   Begin VB.CommandButton mOKButton 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   1380
      TabIndex        =   2
      Top             =   1320
      Width           =   1335
   End
   Begin VB.Label mSaltCodeLabel 
      Caption         =   "Salt Code"
      Height          =   255
      Left            =   1680
      TabIndex        =   5
      Top             =   360
      Width           =   1095
   End
   Begin VB.Label mCoefficientsLabel 
      Caption         =   "Coefficient"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   840
      TabIndex        =   4
      Top             =   360
      Width           =   855
   End
End
Attribute VB_Name = "frmAddSalt"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private mIsOK As Boolean

Private mConnection As ENFramework9.Connection

Private Function TextIndex(cb As ComboBox)
    ' ## Insert the specified salt code into the list.
    Dim num As Long
    Dim mid As Long
    Dim tbl As Long
    Dim Name As String
    
    num = cb.ListCount
    tbl = 0
    Name = Trim(cb.Text)
    
    While num > 1
        mid = num / 2
        If (StrComp(Name, cb.List(tbl + mid), vbTextCompare) < 0) Then
            num = mid
        Else
            tbl = tbl + mid
            num = num - mid
        End If
    Wend
    
    If (num = 1) Then
        If (StrComp(Name, cb.List(tbl), vbTextCompare) > 0) Then
            tbl = tbl + 1
        End If
    End If
    
    If (tbl >= cb.ListCount) Then
        TextIndex = -1
    ElseIf (cb.List(tbl) = Name) Then
        TextIndex = tbl
    Else
        TextIndex = -1
    End If
End Function

Private Sub CheckOKEnabled()
    If (mAdditive1.ListIndex = -1) Then
        If (TextIndex(mAdditive1) = -1) Then
            mOKButton.Enabled = False
            Exit Sub
        End If
    End If
    
    If (mAdditive1.Text = "") Then
        If (mCoefficient1.Text <> "") Then
            mOKButton.Enabled = False
            Exit Sub
        End If
    Else
        If (Len(mCoefficient1.Text) > 0 And Not IsNumeric(mCoefficient1.Text)) Then
            mOKButton.Enabled = False
            Exit Sub
        End If
    End If
    
    mOKButton.Enabled = True
End Sub

Public Sub Initialize(ByVal Connection As ENFramework9.Connection)
    Set mConnection = Connection
    
    RefreshSaltCodeList ""
    
    CheckOKEnabled
End Sub

Private Sub Form_Load()
    SetProperFont mCoefficient1.Font
    SetProperFont mAdditive1.Font
End Sub

Private Sub mAdditive1_Change()
    CheckOKEnabled
End Sub

Private Sub mAdditive1_Click()
    CheckOKEnabled
End Sub

Private Sub mCancelButton_Click()
    Me.Hide
End Sub

Private Sub mCoefficient1_Change()
    CheckOKEnabled
End Sub

Private Sub mCoefficient1_GotFocus()
    SubclassTextbox mCoefficient1.hWnd
End Sub

Private Sub mCoefficient1_LostFocus()
    UnSubclassTextbox mCoefficient1.hWnd
End Sub

Private Property Get SectionNames(ByVal CollectionTypeName As String) As Scripting.Dictionary
    Dim i As Long
    Dim ct As CollectionType
    
    Set ct = mConnection.CollectionTypes(Name:=CollectionTypeName)
    For i = 1 To ct.CollectionListeners.Count
        If (ct.CollectionListeners.Item(i).StorageProgID = "ENStandard9.SectionListCListener") Then
            Set SectionNames = ct.CollectionListeners.Item(i).Tag.object.SectionNames
            Exit Property
        End If
    Next i
    
    Set SectionNames = New Scripting.Dictionary
End Property

Private Sub RefreshSaltCodeList(ByVal oldSaltName As String)
    Dim SaltCodeSecs As Scripting.Dictionary
    Dim i As Long
    
    Set SaltCodeSecs = SectionNames("Salt Codes")
    
    mAdditive1.Clear
    
    For i = LBound(SaltCodeSecs.Keys) To UBound(SaltCodeSecs.Keys)
        mAdditive1.AddItem SaltCodeSecs.Keys(i)
        If (mAdditive1.List(mAdditive1.NewIndex) = oldSaltName) Then
            mAdditive1.ListIndex = mAdditive1.NewIndex
        End If
    Next i

    If (mAdditive1.ListIndex < 0 And mAdditive1.ListCount > 0) Then
        mAdditive1.ListIndex = 0
    End If
    mAdditive1.Text = mAdditive1.List(mAdditive1.ListIndex)
End Sub

Private Sub mOKButton_Click()
    mIsOK = True
    Me.Hide
End Sub

Friend Property Get IsOK() As Boolean
    ' ## True if the user clicked the OK button.
    IsOK = mIsOK
End Property

Friend Property Get NewSaltCoefficient() As String
    If (Len(mCoefficient1.Text) = 0) Then
        NewSaltCoefficient = "1"
    Else
        NewSaltCoefficient = mCoefficient1.Text
    End If
End Property

Friend Property Get NewSaltSection() As ENFramework9.Section
    Dim SaltCodeSecs As Scripting.Dictionary
    Dim i As Long
    
    Set SaltCodeSecs = SectionNames("Salt Codes")
    Set NewSaltSection = SaltCodeSecs(mAdditive1.Text)
End Property

