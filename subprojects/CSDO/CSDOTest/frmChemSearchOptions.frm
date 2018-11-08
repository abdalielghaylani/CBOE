VERSION 5.00
Begin VB.Form frmChemSearchOptions 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Advanced Chemical Search Options"
   ClientHeight    =   4470
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   4260
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   4470
   ScaleWidth      =   4260
   StartUpPosition =   1  'CenterOwner
   Begin VB.CommandButton Command2 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   360
      Left            =   2220
      TabIndex        =   9
      Top             =   3945
      Width           =   1215
   End
   Begin VB.CommandButton Command1 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   360
      Left            =   885
      TabIndex        =   8
      Top             =   3945
      Width           =   1215
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Extra fragments may be present in hit if reaction"
      Height          =   450
      Index           =   7
      Left            =   150
      TabIndex        =   7
      Top             =   3270
      Width           =   3900
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Match any charge on heteroatom"
      Height          =   450
      Index           =   6
      Left            =   150
      TabIndex        =   6
      Top             =   2820
      Width           =   3015
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Match any charge on carbon"
      Height          =   450
      Index           =   5
      Left            =   150
      TabIndex        =   5
      Top             =   2370
      Width           =   3015
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Extra fragments may be present in hit"
      Height          =   450
      Index           =   4
      Left            =   150
      TabIndex        =   4
      Top             =   1920
      Width           =   3015
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Hit must overlap reaction center"
      Height          =   450
      Index           =   3
      Left            =   150
      TabIndex        =   3
      Top             =   1485
      Width           =   3015
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Fragments may overlap"
      Height          =   450
      Index           =   2
      Left            =   150
      TabIndex        =   2
      Top             =   1035
      Width           =   3015
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Match double bond stereo"
      Height          =   450
      Index           =   1
      Left            =   150
      TabIndex        =   1
      Top             =   585
      Width           =   3015
   End
   Begin VB.CheckBox Check1 
      Caption         =   "Match tetrahedral stereo"
      Height          =   450
      Index           =   0
      Left            =   150
      TabIndex        =   0
      Top             =   135
      Width           =   3015
   End
End
Attribute VB_Name = "frmChemSearchOptions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Public isOK As Boolean

Public Sub clear(defConn As CSDO.Connection)
    isOK = False
    Dim i As Integer
    For i = 0 To 7
        Check1(i).Value = vbUnchecked
    Next
    With defConn.ChemSearchOptions
        If .StereoTetr Then Check1(0).Value = vbChecked
        If .StereoDB Then Check1(1).Value = vbChecked
        If .fragscanoverlap Then Check1(2).Value = vbChecked
        If .userxncenters Then Check1(3).Value = vbChecked
        If .extrafragsok Then Check1(4).Value = vbChecked
        If .findchargedcarbon Then Check1(5).Value = vbChecked
        If .findchargedhetero Then Check1(6).Value = vbChecked
        If .extrafragsokifrxn Then Check1(7).Value = vbChecked
    End With
End Sub

Private Sub Command1_Click()
    isOK = True
    Me.Hide
End Sub

Private Sub Command2_click()
    isOK = False
    Me.Hide
End Sub
