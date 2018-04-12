VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ReactionComponent"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' ## An object that contains several public member variables used for analyzing reactions.

Public MolecularWeight As Double
Public Formula As String
Public RepeatCount As Long
Public IsProduct As Boolean
Public IsReactant As Boolean
Public IsSolvent As Boolean
Public ID As Long
Public Top As Double
Public Bottom As Double
Public Left As Double
Public Right As Double

' A canonical code for this reaction component. Since ChemDraw doesn't generate canonical codes, use SMILES.
Public Code As String

