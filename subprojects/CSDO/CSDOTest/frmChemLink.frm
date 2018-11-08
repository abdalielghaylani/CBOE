VERSION 5.00
Begin VB.Form frmChemLink 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Add ChemLink"
   ClientHeight    =   1605
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   4185
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1605
   ScaleWidth      =   4185
   StartUpPosition =   1  'CenterOwner
   Begin VB.CommandButton cmdCancel 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   405
      Left            =   2850
      TabIndex        =   3
      Top             =   1080
      Width           =   1215
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   405
      Left            =   1470
      TabIndex        =   2
      Top             =   1080
      Width           =   1215
   End
   Begin VB.TextBox txtMolID 
      Height          =   330
      Left            =   1470
      TabIndex        =   1
      Top             =   615
      Width           =   2565
   End
   Begin VB.TextBox txtRelTable 
      Height          =   330
      Left            =   1470
      TabIndex        =   0
      Top             =   135
      Width           =   2565
   End
   Begin VB.Label Label2 
      Caption         =   "Relational Table:"
      Height          =   255
      Left            =   180
      TabIndex        =   5
      Top             =   180
      Width           =   1215
   End
   Begin VB.Label Label1 
      Caption         =   "Molecule ID Field:"
      Height          =   300
      Left            =   75
      TabIndex        =   4
      Top             =   630
      Width           =   1365
   End
End
Attribute VB_Name = "frmChemLink"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Public isOK As Boolean

Public Sub clear()
    txtMolID.Text = ""
    txtRelTable.Text = ""
    isOK = False
End Sub

Private Sub cmdCancel_Click()
    isOK = False
    Me.Hide
End Sub

Private Sub cmdOK_Click()
    If txtMolID.Text = "" Or txtRelTable.Text = "" Then
        MsgBox "Please enter a value for both fields."
    Else
        isOK = True
        Me.Hide
    End If
End Sub


