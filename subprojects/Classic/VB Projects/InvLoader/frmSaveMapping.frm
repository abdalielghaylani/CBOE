VERSION 5.00
Begin VB.Form frmSaveMapping 
   Caption         =   "Save Mapping"
   ClientHeight    =   1875
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   4680
   Icon            =   "frmSaveMapping.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   1875
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin VB.ComboBox cmbName 
      Height          =   315
      ItemData        =   "frmSaveMapping.frx":0442
      Left            =   120
      List            =   "frmSaveMapping.frx":0444
      Sorted          =   -1  'True
      TabIndex        =   3
      Top             =   480
      Width           =   4455
   End
   Begin VB.CommandButton cmdCancelSaveMapping 
      Caption         =   "Cancel"
      Height          =   375
      Left            =   3360
      TabIndex        =   2
      Top             =   1200
      Width           =   1215
   End
   Begin VB.CommandButton cmdSaveMapping 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   2040
      TabIndex        =   1
      Top             =   1200
      Width           =   1215
   End
   Begin VB.Label Label1 
      Caption         =   "Please enter a name for this new mapping:"
      Height          =   255
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   4095
   End
End
Attribute VB_Name = "frmSaveMapping"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public Cancel As Boolean
Public sMappingName As String

Private Sub cmdCancelSaveMapping_Click()
    Cancel = True
    Me.Hide
End Sub

Private Sub cmdSaveMapping_Click()
    Dim bValid As Boolean
    Dim sMessage As String
    Dim sChar As String
    Dim sName As String
    Dim mResult As VbMsgBoxResult
    Dim i As Integer
    bValid = True
    
    sName = Trim(cmbName.Text)
    
    If gFieldMappings.Exists(LCase(sName)) Then
        mResult = MsgBox("This will overwrite the existing mapping file.  Click OK to proceed.", vbOKCancel)
        If (mResult = vbCancel) Then
            Exit Sub
        End If
    End If
       
    If Len(sName) = 0 Then
        sMessage = "Please enter a name for this mapping."
        bValid = False
    End If
    
    For i = 1 To Len(sName)
        sChar = Mid(sName, i, 1)
        If InStr("\/:*?""<>|", sChar) > 0 Then
            sMessage = "Name must not contain these characters:" & vbLf & vbLf & "\ / : * ? "" < > |"
            bValid = False
        End If
    Next i
    
    If bValid Then
        Cancel = False
        sMappingName = sName
        Me.Hide
    Else
        MsgBox sMessage, vbExclamation
    End If
End Sub

Private Sub Form_Activate()
    cmbName.Text = ""
    cmbName.SetFocus
End Sub

Public Sub Initialize()
    Dim oItem
    Me.Move (Screen.Width - Me.Width) / 2, (Screen.Height - Me.Height) / 2
    cmbName.Clear
    For Each oItem In gFieldMappings.Items
        cmbName.AddItem (oItem.sName)
    Next
End Sub
