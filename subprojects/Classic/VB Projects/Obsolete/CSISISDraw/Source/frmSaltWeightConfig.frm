VERSION 5.00
Begin VB.Form frmSaltWeightConfig 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Form1"
   ClientHeight    =   1800
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   5370
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1800
   ScaleWidth      =   5370
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  'CenterScreen
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   2760
      TabIndex        =   4
      Top             =   1320
      Width           =   1215
   End
   Begin VB.CommandButton CancelButton 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   375
      Left            =   4080
      TabIndex        =   3
      Top             =   1320
      Width           =   1215
   End
   Begin VB.CheckBox mNewSolvateCodeCheckBox 
      Caption         =   "Create New Solvate Code From Identify Dialog Box"
      Height          =   255
      Left            =   120
      TabIndex        =   2
      Top             =   840
      Width           =   5175
   End
   Begin VB.CheckBox mNewSaltCodeCheckBox 
      Caption         =   "Create New Salt Code From Identify Dialog Box"
      Height          =   255
      Left            =   120
      TabIndex        =   1
      Top             =   480
      Width           =   5295
   End
   Begin VB.CheckBox mComputeNameCheckBox 
      Caption         =   "Compute Name from Structure (Struct-to-Name License Required)"
      Height          =   255
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   5175
   End
End
Attribute VB_Name = "frmSaltWeightConfig"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private mIsOK As Boolean

Private Sub CancelButton_Click()
    Me.Hide
End Sub

Private Sub Form_Load()
    SetProperFont OKButton.Font
    SetProperFont CancelButton.Font
    SetProperFont mComputeNameCheckBox.Font
    SetProperFont mNewSaltCodeCheckBox.Font
    SetProperFont mNewSolvateCodeCheckBox.Font
End Sub

Private Sub OKButton_Click()
    mIsOK = True
    Me.Hide
End Sub

Public Property Get IsOK() As Boolean
    IsOK = mIsOK
End Property

Public Sub Initialize(ByVal ComputeName As Boolean, ByVal NewSaltCode As Boolean, ByVal NewSolvateCode As Boolean)
    mComputeNameCheckBox.Value = IIf(ComputeName, 1, 0)
    mNewSaltCodeCheckBox.Value = IIf(NewSaltCode, 1, 0)
    mNewSolvateCodeCheckBox.Value = IIf(NewSolvateCode, 1, 0)
End Sub

Public Property Get NewSaltCode() As Boolean
    NewSaltCode = mNewSaltCodeCheckBox.Value <> 0
End Property

Public Property Get NewSolvateCode() As Boolean
    NewSolvateCode = mNewSolvateCodeCheckBox.Value <> 0
End Property

Public Property Get ComputeName() As Boolean
    ComputeName = mComputeNameCheckBox.Value <> 0
End Property
