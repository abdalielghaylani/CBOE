VERSION 5.00
Begin VB.Form Form1 
   AutoRedraw      =   -1  'True
   Caption         =   "LockTime"
   ClientHeight    =   450
   ClientLeft      =   1065
   ClientTop       =   945
   ClientWidth     =   2220
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   PaletteMode     =   1  'UseZOrder
   ScaleHeight     =   450
   ScaleWidth      =   2220
   Begin VB.CommandButton Command3 
      Height          =   255
      Left            =   1800
      TabIndex        =   5
      Top             =   480
      Width           =   375
   End
   Begin VB.TextBox Text3 
      Height          =   285
      Left            =   1800
      TabIndex        =   4
      Text            =   "Text3"
      Top             =   120
      Width           =   375
   End
   Begin VB.CommandButton Command2 
      Caption         =   "Unlock NOW"
      Height          =   255
      Left            =   120
      TabIndex        =   3
      Top             =   840
      Width           =   1575
   End
   Begin VB.CommandButton Command1 
      Height          =   255
      Left            =   120
      TabIndex        =   2
      Top             =   480
      Width           =   255
   End
   Begin VB.TextBox Text2 
      Height          =   285
      Left            =   480
      TabIndex        =   1
      Text            =   "Text2"
      Top             =   480
      Width           =   1215
   End
   Begin VB.TextBox Text1 
      Height          =   285
      Left            =   120
      TabIndex        =   0
      Text            =   "Text1"
      Top             =   120
      Width           =   1575
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Command1_Click()
Form1.Text2 = Timer
End Sub


Private Sub Command2_Click()
Form1.Text1 = ""
LockTime = 99999
End Sub


Private Sub Command3_Click()
Form1.Text3 = "0"
WaitCount = 0
End Sub


