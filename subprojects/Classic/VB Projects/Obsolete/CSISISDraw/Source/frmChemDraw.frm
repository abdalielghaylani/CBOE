VERSION 5.00
Object = "{BDA04258-9536-4779-A86A-49164A7741D7}#9.0#0"; "ChemDrawCtl9.dll"
Begin VB.Form frmChemDraw 
   Caption         =   "frmChemDraw"
   ClientHeight    =   3090
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   4680
   LinkTopic       =   "Form1"
   ScaleHeight     =   3090
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin ChemDrawControl9Ctl.ChemDrawCtl ChemDrawCtl1 
      Height          =   2895
      Left            =   960
      TabIndex        =   0
      Top             =   120
      Width           =   2895
      _cx             =   5106
      _cy             =   5106
      SourceURL       =   ""
      DataURL         =   ""
      ViewOnly        =   0   'False
      DontCache       =   0   'False
      ShowToolsWhenVisible=   0   'False
      AuthenticateURL =   ""
      DataEncoded     =   0   'False
      ShrinkToFit     =   -1  'True
      EnlargeToFit    =   0   'False
      WrapReactionsToFit=   0   'False
      RecenterWhenFitting=   -1  'True
      BorderColor     =   0
      BorderVisible   =   0   'False
      BorderWidth     =   1
      OpenDoesImport  =   0   'False
   End
End
Attribute VB_Name = "frmChemDraw"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' ##MODULE_SUMMARY A form used instantiate the ChemDraw control for format conversions, etc.

