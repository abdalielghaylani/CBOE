VERSION 5.00
Begin VB.Form TestForm 
   Caption         =   "Form1"
   ClientHeight    =   3192
   ClientLeft      =   60
   ClientTop       =   348
   ClientWidth     =   4680
   LinkTopic       =   "Form1"
   ScaleHeight     =   3192
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton ImportBtn 
      Caption         =   "Import"
      Height          =   615
      Left            =   840
      TabIndex        =   0
      Top             =   1080
      Width           =   2055
   End
End
Attribute VB_Name = "TestForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub ImportBtn_Click()
    
    Dim imptr
    Dim retVal
    
    Set imptr = CreateObject("ChemImp.Importer")
    
    imptr.Import "regdb", "oracle", "sunnyora", "C:\Documents and Settings\Sunny Yan\Desktop\CS", "1 record.sdf", "TempTable"
    MsgBox "done"
End Sub
