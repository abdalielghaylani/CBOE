VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSCOMCTL.OCX"
Begin VB.Form frmAdvancedOptions 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Options"
   ClientHeight    =   3840
   ClientLeft      =   2565
   ClientTop       =   1500
   ClientWidth     =   5325
   Icon            =   "frmAdvancedOptions.frx":0000
   KeyPreview      =   -1  'True
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3840
   ScaleWidth      =   5325
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  'CenterScreen
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      Height          =   3780
      Index           =   3
      Left            =   -20000
      ScaleHeight     =   3780
      ScaleWidth      =   5685
      TabIndex        =   6
      TabStop         =   0   'False
      Top             =   480
      Width           =   5685
      Begin VB.Frame fraSample4 
         Caption         =   "Sample 4"
         Height          =   1785
         Left            =   2100
         TabIndex        =   9
         Top             =   840
         Width           =   2055
      End
   End
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      Height          =   3780
      Index           =   2
      Left            =   -20000
      ScaleHeight     =   3780
      ScaleWidth      =   5685
      TabIndex        =   5
      TabStop         =   0   'False
      Top             =   480
      Width           =   5685
      Begin VB.Frame fraSample3 
         Caption         =   "Sample 3"
         Height          =   1785
         Left            =   1545
         TabIndex        =   8
         Top             =   675
         Width           =   2055
      End
   End
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      Height          =   3780
      Index           =   1
      Left            =   -20000
      ScaleHeight     =   3780
      ScaleWidth      =   5685
      TabIndex        =   4
      TabStop         =   0   'False
      Top             =   480
      Width           =   5685
      Begin VB.Frame fraSample2 
         Caption         =   "Sample 2"
         Height          =   1785
         Left            =   645
         TabIndex        =   7
         Top             =   300
         Width           =   2055
      End
   End
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      Height          =   2460
      Index           =   0
      Left            =   120
      ScaleHeight     =   2460
      ScaleWidth      =   4845
      TabIndex        =   3
      TabStop         =   0   'False
      Top             =   480
      Width           =   4845
      Begin VB.Frame fraSample1 
         Height          =   2145
         Left            =   120
         TabIndex        =   10
         Top             =   15
         Width           =   4695
         Begin VB.CheckBox chkSaveContainerXML 
            Caption         =   "Save Container XML for troubleshooting"
            Height          =   375
            Left            =   120
            TabIndex        =   15
            Top             =   1200
            Width           =   4335
         End
         Begin VB.CheckBox chkSaveCompoundXML 
            Caption         =   "Save Compound XML for troubleshooting"
            Height          =   375
            Left            =   120
            TabIndex        =   14
            Top             =   1560
            Width           =   4335
         End
         Begin VB.CheckBox chkSavePlateXML 
            Caption         =   "Save Plate XML for troubleshooting"
            Height          =   375
            Left            =   120
            TabIndex        =   13
            Top             =   840
            Width           =   4335
         End
         Begin VB.TextBox txtHTTPTimeout 
            Height          =   285
            Left            =   3600
            TabIndex        =   11
            Top             =   480
            Width           =   855
         End
         Begin VB.Label Label1 
            Caption         =   "HTTP Server Timeout (in seconds):"
            Height          =   255
            Left            =   120
            TabIndex        =   12
            Top             =   480
            Width           =   2775
         End
      End
   End
   Begin VB.CommandButton cmdCancel 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   375
      Left            =   4080
      TabIndex        =   2
      Top             =   3255
      Width           =   1095
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Height          =   375
      Left            =   2850
      TabIndex        =   1
      Top             =   3255
      Width           =   1095
   End
   Begin MSComctlLib.TabStrip tbsOptions 
      Height          =   2925
      Left            =   105
      TabIndex        =   0
      Top             =   120
      Width           =   5055
      _ExtentX        =   8916
      _ExtentY        =   5159
      _Version        =   393216
      BeginProperty Tabs {1EFB6598-857C-11D1-B16A-00C0F0283628} 
         NumTabs         =   1
         BeginProperty Tab1 {1EFB659A-857C-11D1-B16A-00C0F0283628} 
            Caption         =   "API"
            Key             =   "Group1"
            Object.ToolTipText     =   "Set HTTP server API settings"
            ImageVarType    =   2
         EndProperty
      EndProperty
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
   End
End
Attribute VB_Name = "frmAdvancedOptions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub cmdCancel_Click()
    Unload Me
End Sub

Private Sub cmdOK_Click()
    glHTTPTimeout = CLng(txtHTTPTimeout.Text)
    gbSavePlateXML = chkSavePlateXML.value
    gbSaveCompoundXML = chkSaveCompoundXML.value
    gbSaveContainerXML = chkSaveContainerXML.value
    Call SetGlobals
    Unload Me
End Sub

Private Sub Form_KeyDown(KeyCode As Integer, Shift As Integer)
    Dim i As Integer
    'handle ctrl+tab to move to the next tab
    If Shift = vbCtrlMask And KeyCode = vbKeyTab Then
        i = tbsOptions.SelectedItem.Index
        If i = tbsOptions.Tabs.count Then
            'last tab so we need to wrap to tab 1
            Set tbsOptions.SelectedItem = tbsOptions.Tabs(1)
        Else
            'increment the tab
            Set tbsOptions.SelectedItem = tbsOptions.Tabs(i + 1)
        End If
    End If
End Sub

Private Sub Form_Load()
    txtHTTPTimeout.Text = glHTTPTimeout
    If gbSavePlateXML Then
        chkSavePlateXML.value = 1
    Else
        chkSavePlateXML.value = 0
    End If
    If gbSaveCompoundXML Then
        chkSaveCompoundXML.value = 1
    Else
        chkSaveCompoundXML.value = 0
    End If
    If gbSaveContainerXML Then
        chkSaveContainerXML.value = 1
    Else
        chkSaveContainerXML.value = 0
    End If
    'center the form
    Me.Move (Screen.Width - Me.Width) / 2, (Screen.Height - Me.Height) / 2
    
End Sub

Private Sub tbsOptions_Click()
    
    Dim i As Integer
    'show and enable the selected tab's controls
    'and hide and disable all others
    For i = 0 To tbsOptions.Tabs.count - 1
        If i = tbsOptions.SelectedItem.Index - 1 Then
            picOptions(i).Left = 210
            picOptions(i).Enabled = True
        Else
            picOptions(i).Left = -20000
            picOptions(i).Enabled = False
        End If
    Next
    
End Sub


Public Sub Initialize()
    
End Sub


Private Sub txtHTTPTimeout_LostFocus()
    If Not IsNumeric(txtHTTPTimeout.Text) Then
        txtHTTPTimeout.Text = DEFAULT_HTTPTIMEOUT
    Else
        If CLng(txtHTTPTimeout.Text) < 10 Then
            txtHTTPTimeout.Text = 10
        End If
    End If
End Sub
