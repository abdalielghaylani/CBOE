VERSION 5.00
Begin VB.Form frmCSTimer 
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
Attribute VB_Name = "frmCSTimer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' The calling object

Dim mISISListener As ISISDrawCSListener

Friend Sub Initialize(ByVal ISISListener As ISISDrawCSListener)
    Set mISISListener = ISISListener
End Sub

Private Sub Timer1_Timer()
    Timer1.Interval = 0 ' Fire just one time
' Old code: from before use of ISISDrawEdit
'    mISISListener.DoEdit
End Sub

