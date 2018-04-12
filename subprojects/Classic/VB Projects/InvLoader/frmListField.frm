VERSION 5.00
Begin VB.Form frmListField 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Select values for property"
   ClientHeight    =   3960
   ClientLeft      =   2760
   ClientTop       =   3750
   ClientWidth     =   6420
   BeginProperty Font 
      Name            =   "Tahoma"
      Size            =   8.25
      Charset         =   0
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3960
   ScaleWidth      =   6420
   ShowInTaskbar   =   0   'False
   Begin VB.CommandButton cbReplace 
      Caption         =   "Replace"
      Height          =   375
      Left            =   2280
      TabIndex        =   7
      Top             =   600
      Width           =   735
   End
   Begin VB.CommandButton cbRemove 
      Caption         =   "Remove"
      Height          =   375
      Left            =   5400
      TabIndex        =   6
      Top             =   600
      Width           =   735
   End
   Begin VB.CommandButton cbEdit 
      Caption         =   "Edit"
      Height          =   375
      Left            =   1200
      TabIndex        =   5
      Top             =   600
      Width           =   735
   End
   Begin VB.CommandButton cbAdd 
      Caption         =   "Add"
      Height          =   375
      Left            =   240
      TabIndex        =   4
      Top             =   600
      Width           =   735
   End
   Begin VB.TextBox txtValue 
      Height          =   375
      Left            =   240
      TabIndex        =   3
      Top             =   120
      Width           =   5895
   End
   Begin VB.ListBox lstValues 
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2205
      Left            =   240
      TabIndex        =   2
      Top             =   1080
      Width           =   5895
   End
   Begin VB.CommandButton CancelButton 
      Caption         =   "Cancel"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   4920
      TabIndex        =   1
      Top             =   3480
      Width           =   1215
   End
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   3480
      TabIndex        =   0
      Top             =   3480
      Width           =   1215
   End
End
Attribute VB_Name = "frmListField"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public Cancel As Boolean
Public Picklist As Dictionary
Public value As String

Private Sub CancelButton_Click()
    Cancel = True
    Me.Hide
End Sub

Public Sub Intialize(sValue As String)
    Dim asValues As Variant
    Dim vValue As Variant
    
    asValues = Split(sValue, msListSeparator)
    lstValues.Clear
    For Each vValue In asValues
        lstValues.AddItem vValue
    Next vValue
    SetButtons
End Sub

Private Sub cbAdd_Click()
    If Len(txtValue.Text) > 0 Then
        lstValues.AddItem (txtValue.Text)
        txtValue.Text = ""
    End If
    SetButtons
End Sub

Private Sub cbEdit_Click()
    If lstValues.ListIndex >= 0 Then
        txtValue.Text = lstValues.Text
    End If
    SetButtons
End Sub

Private Sub cbRemove_Click()
    Dim iValue
    If lstValues.ListIndex >= 0 Then
        iValue = lstValues.ListIndex
        lstValues.RemoveItem (lstValues.ListIndex)
        If lstValues.ListCount > 0 Then
            If iValue < lstValues.ListCount Then
                lstValues.ListIndex = iValue
            Else
                lstValues.ListIndex = lstValues.ListCount - 1
            End If
        End If
    End If
    SetButtons
End Sub

Private Sub cbReplace_Click()
    If lstValues.ListIndex >= 0 And Len(txtValue.Text) > 0 Then
        lstValues.list(lstValues.ListIndex) = txtValue.Text
    End If
    SetButtons
End Sub

Private Sub lstValues_Click()
    SetButtons
End Sub

Private Sub OKButton_Click()
    SetValue
    Cancel = False
    Me.Hide
End Sub

Private Sub SetButtons()
    cbEdit.Enabled = lstValues.ListIndex >= 0
    cbRemove.Enabled = lstValues.ListIndex >= 0
    cbReplace.Enabled = lstValues.ListIndex >= 0 And Len(txtValue.Text)
    cbAdd.Enabled = Len(txtValue.Text) > 0
End Sub

Private Sub SetValue()
    Dim i As Integer
    value = ""
    For i = 0 To lstValues.ListCount - 1
        If i > 0 Then value = value + msListSeparator
        value = value + lstValues.list(i)
    Next i
End Sub

Private Sub txtValue_Change()
    SetButtons
End Sub
