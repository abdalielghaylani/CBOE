VERSION 5.00
Begin VB.Form frmQuery 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Create ChemSQL String"
   ClientHeight    =   3465
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   6615
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3465
   ScaleWidth      =   6615
   StartUpPosition =   1  'CenterOwner
   Begin VB.CommandButton Command3 
      Caption         =   "Close"
      Default         =   -1  'True
      Height          =   390
      Left            =   3405
      TabIndex        =   13
      Top             =   2985
      Width           =   1185
   End
   Begin VB.CommandButton Command2 
      Caption         =   "&Copy"
      Height          =   390
      Left            =   1995
      TabIndex        =   12
      Top             =   2985
      Width           =   1185
   End
   Begin VB.TextBox txtSql 
      Height          =   705
      Left            =   195
      TabIndex        =   10
      Top             =   2175
      Width           =   6255
   End
   Begin VB.Frame Frame2 
      Caption         =   "Data Transfer"
      Height          =   1605
      Left            =   3435
      TabIndex        =   6
      Top             =   150
      Width           =   3015
      Begin VB.TextBox txtDisk 
         Height          =   315
         Left            =   555
         TabIndex        =   9
         Top             =   1125
         Width           =   2265
      End
      Begin VB.OptionButton optDT 
         Caption         =   "Use Disk File:"
         Height          =   345
         Index           =   1
         Left            =   270
         TabIndex        =   8
         Top             =   705
         Width           =   2145
      End
      Begin VB.OptionButton optDT 
         Caption         =   "Use Base64 CDX"
         Height          =   345
         Index           =   0
         Left            =   255
         TabIndex        =   7
         Top             =   345
         Width           =   2145
      End
   End
   Begin VB.Frame Frame1 
      Caption         =   "Query Type"
      Height          =   1590
      Left            =   210
      TabIndex        =   0
      Top             =   165
      Width           =   3000
      Begin VB.OptionButton optQT 
         Caption         =   "No Query"
         Height          =   210
         Index           =   3
         Left            =   180
         TabIndex        =   14
         Top             =   300
         Width           =   1965
      End
      Begin VB.TextBox txtSim 
         Height          =   300
         Left            =   2175
         TabIndex        =   4
         Text            =   "90"
         Top             =   1125
         Width           =   435
      End
      Begin VB.OptionButton optQT 
         Caption         =   "Similarity Greater Than:"
         Height          =   210
         Index           =   2
         Left            =   180
         TabIndex        =   3
         Top             =   1185
         Width           =   1965
      End
      Begin VB.OptionButton optQT 
         Caption         =   "Exact Structure"
         Height          =   210
         Index           =   1
         Left            =   180
         TabIndex        =   2
         Top             =   895
         Width           =   1965
      End
      Begin VB.OptionButton optQT 
         Caption         =   "SubStructure"
         Height          =   210
         Index           =   0
         Left            =   180
         TabIndex        =   1
         Top             =   605
         Width           =   1965
      End
      Begin VB.Label Label1 
         Caption         =   "%"
         Height          =   225
         Left            =   2655
         TabIndex        =   5
         Top             =   1185
         Width           =   240
      End
   End
   Begin VB.Label Label2 
      Caption         =   "ChemSQL String:"
      Height          =   255
      Left            =   180
      TabIndex        =   11
      Top             =   1890
      Width           =   1515
   End
End
Attribute VB_Name = "frmQuery"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Public mol As CSMol
Public base64str As String
Public currSqlType As Integer
Public currDTType As Integer

Public Sub clear(argMol As CSMol)
    Set mol = argMol
    txtDisk = GetTmpPath & "temp.cdx"
    txtSim = "90"
    optQT(3).Value = True
    optDT(1).Value = True
    currSqlType = 3
    currDTType = 1
    txtSql = MakeSQLString(3, 1)
End Sub

Private Function MakeSQLString(sqlType As Integer, dtType As Integer) As String
    Dim txt As String

    Select Case sqlType
        Case 3
            txt = "STRUCT("
        Case 0
            txt = "SUBSTRUCT(STRUCTURE, STRUCT("
        Case 1
            txt = "EXACTSTRUCT(STRUCTURE, STRUCT("
        Case 2
            txt = "SIMILARITY(STRUCTURE, STRUCT("
    End Select
    Select Case dtType
        Case 0
            txt = txt & "BASE64CDX, "
            mol.WriteMol GetTmpPath & "temp.cdx"
            txt = txt & "'" & Encode(GetTmpPath & "temp.cdx") & "')"
        Case 1
            txt = txt & "STRUCTFILE, "
            txt = txt & "'" & txtDisk & "')"
    End Select
    Select Case sqlType
        Case 0, 1
            txt = txt & ") > 0"
        Case 2
            txt = txt & ") > " & txtSim
    End Select
    MakeSQLString = txt
End Function

Private Sub Command2_click()
    Clipboard.SetText txtSql.Text
End Sub

Private Sub Command3_Click()
    If optDT(1).Value = True Then
        mol.WriteMol txtDisk
    End If
    Me.Hide
End Sub

Private Sub optDT_Click(Index As Integer)
    currDTType = Index
    If currDTType = 0 Then
        txtDisk.Enabled = False
    Else
        txtDisk.Enabled = True
    End If
    txtSql = MakeSQLString(currSqlType, currDTType)
End Sub

Private Sub optQT_Click(Index As Integer)
    currSqlType = Index
    txtSql = MakeSQLString(currSqlType, currDTType)
End Sub


Private Sub txtDisk_Change()
    txtSql = MakeSQLString(currSqlType, currDTType)
End Sub
