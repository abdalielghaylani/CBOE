VERSION 5.00
Begin VB.Form frmPickListField 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Select values for property"
   ClientHeight    =   3495
   ClientLeft      =   2760
   ClientTop       =   3750
   ClientWidth     =   7125
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3495
   ScaleWidth      =   7125
   ShowInTaskbar   =   0   'False
   Begin VB.CommandButton cbRemove 
      Caption         =   "<"
      Height          =   375
      Left            =   3360
      TabIndex        =   5
      Top             =   1200
      Width           =   375
   End
   Begin VB.CommandButton cbAdd 
      Caption         =   ">"
      Height          =   375
      Left            =   3360
      TabIndex        =   4
      Top             =   600
      Width           =   375
   End
   Begin VB.ListBox lstValues 
      Height          =   2205
      Left            =   3960
      TabIndex        =   3
      Top             =   360
      Width           =   2895
   End
   Begin VB.ListBox lstPicks 
      Height          =   2205
      Left            =   240
      TabIndex        =   2
      Top             =   360
      Width           =   2895
   End
   Begin VB.CommandButton CancelButton 
      Caption         =   "Cancel"
      Height          =   375
      Left            =   5640
      TabIndex        =   1
      Top             =   3000
      Width           =   1215
   End
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      Height          =   375
      Left            =   4080
      TabIndex        =   0
      Top             =   3000
      Width           =   1215
   End
End
Attribute VB_Name = "frmPickListField"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public Cancel As Boolean
Public value As String

Private Picklist As Dictionary
Private Selection As Dictionary

Private Sub CancelButton_Click()
    Cancel = True
    Me.Hide
End Sub

Public Sub Initialize(sValue As String, oPicklist As Dictionary)
    Dim asValues As Variant
    Dim vValue As Variant
    
    Set Picklist = oPicklist
    InitializeSelection
    asValues = Split(sValue, msListSeparator)
    lstValues.Clear
    For Each vValue In asValues
        If Selection.Exists(vValue) Then
            Selection.Item(vValue) = 1
        End If
    Next vValue
    DisplaySelection
End Sub

Private Sub cbAdd_Click()
    AddValue
End Sub

Private Sub cbRemove_Click()
    RemoveValue
End Sub

Private Sub OKButton_Click()
    Cancel = False
    SetValue
    Me.Hide
End Sub

Private Sub InitializeSelection()
    Dim key As Variant
    Set Selection = New Dictionary
    For Each key In Picklist
        Selection.Add key, 0
    Next key
End Sub
        
Private Sub DisplaySelection()
    Dim key As Variant
    Dim iValues As Long
    Dim iPicks As Long
    
    iValues = lstValues.ListIndex
    iPicks = lstPicks.ListIndex
    lstValues.Clear
    lstPicks.Clear
    For Each key In Selection
        If (Selection.Item(key) <> 0) Then
            lstValues.AddItem key
        Else
            lstPicks.AddItem key
        End If
    Next key
    
    If iValues < lstValues.ListCount Then
        lstValues.ListIndex = iValues
    ElseIf lstValues.ListCount > 0 Then
        lstValues.ListIndex = lstValues.ListCount - 1
    End If
    If iPicks < lstPicks.ListCount Then
        lstPicks.ListIndex = iPicks
    ElseIf lstPicks.ListCount > 0 Then
        lstPicks.ListIndex = lstPicks.ListCount - 1
    End If
    
End Sub

Private Sub SetValue()
    Dim i As Integer
    value = ""
    For i = 0 To lstValues.ListCount - 1
        If i > 0 Then value = value + msListSeparator
        value = value + lstValues.list(i)
    Next i
End Sub

Private Sub AddValue()
    If lstPicks.ListIndex >= 0 Then
        Selection.Item(lstPicks.Text) = 1
    End If
    DisplaySelection
End Sub

Private Sub RemoveValue()
    If lstValues.ListIndex >= 0 Then
        Selection.Item(lstValues.Text) = 0
    End If
    DisplaySelection
End Sub



