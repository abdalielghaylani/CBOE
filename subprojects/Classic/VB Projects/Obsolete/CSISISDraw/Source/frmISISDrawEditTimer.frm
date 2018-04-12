VERSION 5.00
Begin VB.Form frmISISDrawEditTimer 
   Caption         =   "Form1"
   ClientHeight    =   3090
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   4680
   LinkTopic       =   "Form1"
   ScaleHeight     =   3090
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer Timer1 
      Left            =   960
      Top             =   1440
   End
End
Attribute VB_Name = "frmISISDrawEditTimer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' The calling object

Dim mISISDrawEdit As ISISDrawEdit
Dim mcdxml As String

Friend Sub Initialize(ByVal IDrawEdit As ISISDrawEdit, ByVal cdxml As String)
    Set mISISDrawEdit = IDrawEdit
    mcdxml = cdxml
End Sub

Private Sub Timer1_Timer()
    Timer1.Interval = 0 ' Fire just one time
    Call mISISDrawEdit.DoEdit(mcdxml)
End Sub

