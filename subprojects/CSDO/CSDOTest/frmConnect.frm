VERSION 5.00
Begin VB.Form frmConnect 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "New Connection"
   ClientHeight    =   4440
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   5355
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   4440
   ScaleWidth      =   5355
   StartUpPosition =   1  'CenterOwner
   Begin VB.CommandButton cmdDelete 
      Caption         =   "De&lete ChemLink"
      Height          =   375
      Left            =   3015
      TabIndex        =   5
      Top             =   3345
      Width           =   2190
   End
   Begin VB.CommandButton cmdAdd 
      Caption         =   "A&dd ChemLink"
      Height          =   375
      Left            =   3015
      TabIndex        =   4
      Top             =   2880
      Width           =   2190
   End
   Begin VB.ListBox lstChemLinks 
      Height          =   840
      Left            =   135
      TabIndex        =   3
      Top             =   2865
      Width           =   2730
   End
   Begin VB.CommandButton cmdCancel 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   345
      Left            =   3510
      TabIndex        =   7
      Top             =   3945
      Width           =   1710
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   345
      Left            =   1680
      TabIndex        =   6
      Top             =   3945
      Width           =   1710
   End
   Begin VB.TextBox txtMolString 
      Height          =   300
      Left            =   135
      TabIndex        =   2
      Text            =   "D:\chemacxdb\6500.mst"
      Top             =   2055
      Width           =   5040
   End
   Begin VB.TextBox txtIP 
      Height          =   300
      Left            =   135
      TabIndex        =   0
      Top             =   435
      Width           =   5040
   End
   Begin VB.TextBox txtADO 
      Height          =   330
      Left            =   135
      TabIndex        =   1
      Text            =   "DSN=chem_reg;uid=t5_supervising_chemical_admin;pwd=t5_supervising_chemical_admin;"
      Top             =   1200
      Width           =   5040
   End
   Begin VB.Label Label3 
      Caption         =   "ChemLinks:"
      Height          =   255
      Left            =   150
      TabIndex        =   11
      Top             =   2580
      Width           =   1215
   End
   Begin VB.Label Label5 
      Caption         =   "MolServer Connection String:"
      Height          =   240
      Left            =   135
      TabIndex        =   10
      Top             =   1740
      Width           =   3120
   End
   Begin VB.Label Label2 
      Caption         =   "ADO Connection string:"
      Height          =   345
      Left            =   135
      TabIndex        =   9
      Top             =   945
      Width           =   4740
   End
   Begin VB.Label Label1 
      Caption         =   "Server IP address (leave blank to use this machine):"
      Height          =   300
      Left            =   135
      TabIndex        =   8
      Top             =   105
      Width           =   4560
   End
End
Attribute VB_Name = "frmConnect"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Public fileName As String
Public csConnection As CSDO.Connection
Public isOK As Boolean

Public Sub clear(conn As CSDO.Connection)
    Set csConnection = conn
    isOK = False
    txtADO.Text = conn.ADOConnString
    txtIP.Text = ""
On Error Resume Next ' might not have chemlinks yet...
    txtMolString.Text = conn.CSChemLinks.Item(1).MolConnString
    lstChemLinks.clear
    Dim cl As CSChemLink
    For Each cl In conn.CSChemLinks
        lstChemLinks.AddItem cl.RelTable & "." & cl.RelField
    Next
End Sub

Private Sub cmdAdd_Click()
    With frmChemLink
        .clear
        .Show vbModal, Me
        If .isOK Then
            lstChemLinks.AddItem .txtRelTable.Text & "." & .txtMolID.Text
        End If
    End With
End Sub

Private Sub cmdCancel_Click()
    isOK = False
    Me.Hide
End Sub

Private Sub cmdDelete_Click()
    If lstChemLinks.ListIndex <> -1 Then
        lstChemLinks.RemoveItem lstChemLinks.ListIndex
    End If
End Sub

Private Sub cmdOK_Click()
    If txtADO.Text = "" Or txtMolString.Text = "" Then
        MsgBox "You must enter data for MolString and ADO String."
    Else
        isOK = True
        Me.Hide
    End If
End Sub



