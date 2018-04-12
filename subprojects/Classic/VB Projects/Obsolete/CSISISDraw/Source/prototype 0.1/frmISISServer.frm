VERSION 5.00
Begin VB.Form frmISISServer 
   Caption         =   "ISISServer"
   ClientHeight    =   3090
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   8430
   LinkTopic       =   "Form1"
   ScaleHeight     =   3090
   ScaleWidth      =   8430
   StartUpPosition =   3  'Windows Default
   Begin VB.OLE OLE1 
      Class           =   "ISISServer"
      Height          =   2775
      Left            =   120
      OleObjectBlob   =   "frmISISServer.frx":0000
      TabIndex        =   0
      Top             =   120
      Width           =   8175
   End
End
Attribute VB_Name = "frmISISServer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private mChemicalStructureCtl As ChemicalStructureCtl

Friend Sub Initialize(oChemicalStructureCtl As ChemicalStructureCtl)
    Set mChemicalStructureCtl = oChemicalStructureCtl
End Sub


Private Sub OLE1_Updated(Code As Integer)
    If Code = vbOLEClosed Then
        mChemicalStructureCtl.ClearAndLoad
    End If
End Sub

