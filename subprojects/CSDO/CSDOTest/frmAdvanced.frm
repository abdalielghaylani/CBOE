VERSION 5.00
Begin VB.Form frmAdvanced 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Advanced Connection Options"
   ClientHeight    =   4875
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   5130
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   4875
   ScaleWidth      =   5130
   StartUpPosition =   1  'CenterOwner
   Begin VB.CommandButton cmdCancel 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   345
      Left            =   3705
      TabIndex        =   21
      Top             =   4410
      Width           =   1215
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   345
      Left            =   2325
      TabIndex        =   20
      Top             =   4410
      Width           =   1215
   End
   Begin VB.TextBox txtOSP 
      Height          =   300
      Left            =   2265
      TabIndex        =   14
      Top             =   3870
      Width           =   2655
   End
   Begin VB.TextBox txtTTBN 
      Height          =   300
      Left            =   2265
      TabIndex        =   13
      Top             =   3464
      Width           =   2655
   End
   Begin VB.TextBox txtADSN 
      Height          =   300
      Left            =   2265
      TabIndex        =   12
      Top             =   3061
      Width           =   2655
   End
   Begin VB.TextBox txtAPWD 
      Height          =   300
      Left            =   2265
      TabIndex        =   11
      Top             =   2658
      Width           =   2655
   End
   Begin VB.TextBox txtAUID 
      Height          =   300
      Left            =   2265
      TabIndex        =   10
      Top             =   2255
      Width           =   2655
   End
   Begin VB.TextBox txtWD 
      Height          =   300
      Left            =   2265
      TabIndex        =   9
      Top             =   1852
      Width           =   2655
   End
   Begin VB.TextBox txtJT 
      Height          =   300
      Left            =   2265
      TabIndex        =   4
      Top             =   613
      Width           =   2655
   End
   Begin VB.ComboBox cmbLJM 
      Height          =   315
      Left            =   2265
      Style           =   2  'Dropdown List
      TabIndex        =   3
      Top             =   1434
      Width           =   2655
   End
   Begin VB.ComboBox cmbSJM 
      Height          =   315
      Left            =   2265
      Style           =   2  'Dropdown List
      TabIndex        =   2
      Top             =   1016
      Width           =   2655
   End
   Begin VB.ComboBox cmbStoreType 
      Height          =   315
      Left            =   2265
      Style           =   2  'Dropdown List
      TabIndex        =   0
      Top             =   195
      Width           =   2655
   End
   Begin VB.Label Label10 
      Caption         =   "Oracle SQLLDR Path:"
      Height          =   240
      Left            =   390
      TabIndex        =   19
      Top             =   3900
      Width           =   1620
   End
   Begin VB.Label Label9 
      Caption         =   "Temp Table Base Name:"
      Height          =   240
      Left            =   195
      TabIndex        =   18
      Top             =   3495
      Width           =   1815
   End
   Begin VB.Label Label8 
      Caption         =   "Admin DSN:"
      Height          =   240
      Left            =   1095
      TabIndex        =   17
      Top             =   3090
      Width           =   915
   End
   Begin VB.Label Label7 
      Caption         =   "Admin PWD:"
      Height          =   240
      Left            =   1050
      TabIndex        =   16
      Top             =   2685
      Width           =   960
   End
   Begin VB.Label Label6 
      Caption         =   "Admin UID:"
      Height          =   240
      Left            =   1155
      TabIndex        =   15
      Top             =   2280
      Width           =   855
   End
   Begin VB.Label Label5 
      Caption         =   "Working Directory:"
      Height          =   240
      Left            =   645
      TabIndex        =   8
      Top             =   1875
      Width           =   1365
   End
   Begin VB.Label Label4 
      Caption         =   "Large Join Method:"
      Height          =   240
      Left            =   630
      TabIndex        =   7
      Top             =   1470
      Width           =   1380
   End
   Begin VB.Label Label3 
      Caption         =   "Small Join Method:"
      Height          =   240
      Left            =   675
      TabIndex        =   6
      Top             =   1065
      Width           =   1335
   End
   Begin VB.Label Label2 
      Caption         =   "Large Join Threshold:"
      Height          =   240
      Left            =   450
      TabIndex        =   5
      Top             =   660
      Width           =   1560
   End
   Begin VB.Label Label1 
      Caption         =   "Relational Store Type:"
      Height          =   240
      Left            =   375
      TabIndex        =   1
      Top             =   255
      Width           =   1635
   End
End
Attribute VB_Name = "frmAdvanced"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Public isOK As Boolean

Public Sub clear(defConn As CSDO.Connection)
    isOK = False
    With cmbStoreType
        .clear
        .AddItem "Unknown", 0
        .AddItem "MS Access", 1
        .AddItem "Oracle", 2
        .AddItem "SQL Server", 3
    End With
    With cmbLJM
        .clear
        .AddItem "Choose Best", 0
        .AddItem "Chem First, Use MOL_ID String", 1
        .AddItem "Chem First, Use Temp Table", 2
        .AddItem "Chem First, Use Oracle SQLLDR", 3
        .AddItem "Relational First"
    End With
    With cmbSJM
        .clear
        .AddItem "Choose Best", 0
        .AddItem "Chem First, Use MOL_ID String", 1
        .AddItem "Chem First, Use Temp Table", 2
        .AddItem "Chem First, Use Oracle SQLLDR", 3
        .AddItem "Relational First"
    End With
    With defConn.ADOOptions
        cmbStoreType.ListIndex = .StoreType
        cmbLJM.ListIndex = .LargeJoinMethod
        cmbSJM.ListIndex = .SmallJoinMethod
        txtJT.Text = .SmallToLargeJoinThreshold
        txtWD.Text = .ScratchDir
        txtAUID.Text = .AdminUID
        txtAPWD.Text = .AdminPWD
        txtADSN.Text = .AdminConnStr
        txtTTBN.Text = .TempTableBaseName
        txtOSP.Text = .ORASQLLDRPath
    End With
End Sub

Private Sub cmdOK_Click()
    isOK = True
    Me.Hide
End Sub

Private Sub cmdCancel_Click()
    isOK = False
    Me.Hide
End Sub
