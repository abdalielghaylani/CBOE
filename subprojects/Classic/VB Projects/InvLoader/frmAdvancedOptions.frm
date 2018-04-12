VERSION 5.00
Begin VB.Form frmAdvancedOptions 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Options"
   ClientHeight    =   3825
   ClientLeft      =   2565
   ClientTop       =   1500
   ClientWidth     =   5190
   Icon            =   "frmAdvancedOptions.frx":0000
   KeyPreview      =   -1  'True
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3825
   ScaleWidth      =   5190
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  'CenterScreen
   Begin VB.CheckBox chkDoNotSendXML 
      Caption         =   "Do not send XML (testing only, setting not retained)"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   255
      Left            =   360
      TabIndex        =   12
      Top             =   2640
      Width           =   4455
   End
   Begin VB.Frame fraSample1 
      Caption         =   "Options"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   2385
      Left            =   240
      TabIndex        =   8
      Top             =   120
      Width           =   4695
      Begin VB.TextBox txtHTTPTimeout 
         Height          =   285
         Left            =   3240
         TabIndex        =   14
         Top             =   345
         Width           =   855
      End
      Begin VB.TextBox txtContainerLimit 
         Height          =   285
         Left            =   3240
         TabIndex        =   13
         Top             =   705
         Width           =   855
      End
      Begin VB.CheckBox chkSavePlateXML 
         Caption         =   "Save Plate XML for troubleshooting"
         Height          =   375
         Left            =   120
         TabIndex        =   11
         Top             =   1800
         Width           =   4335
      End
      Begin VB.CheckBox chkSaveCompoundXML 
         Caption         =   "Save Compound XML for troubleshooting"
         Height          =   375
         Left            =   120
         TabIndex        =   10
         Top             =   1440
         Width           =   4335
      End
      Begin VB.CheckBox chkSaveContainerXML 
         Caption         =   "Save Container XML for troubleshooting"
         Height          =   375
         Left            =   120
         TabIndex        =   9
         Top             =   1080
         Width           =   4335
      End
      Begin VB.Label Label1 
         Caption         =   "HTTP Server Timeout (in seconds):"
         Height          =   255
         Left            =   120
         TabIndex        =   16
         Top             =   360
         Width           =   2775
      End
      Begin VB.Label Label2 
         Caption         =   "Upload Chunk Size"
         Height          =   255
         Left            =   120
         TabIndex        =   15
         Top             =   720
         Width           =   2775
      End
   End
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   3780
      Index           =   3
      Left            =   -20000
      ScaleHeight     =   3780
      ScaleWidth      =   5685
      TabIndex        =   4
      TabStop         =   0   'False
      Top             =   480
      Width           =   5685
      Begin VB.Frame fraSample4 
         Caption         =   "Sample 4"
         Height          =   1785
         Left            =   2100
         TabIndex        =   7
         Top             =   840
         Width           =   2055
      End
   End
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   3780
      Index           =   2
      Left            =   -20000
      ScaleHeight     =   3780
      ScaleWidth      =   5685
      TabIndex        =   3
      TabStop         =   0   'False
      Top             =   480
      Width           =   5685
      Begin VB.Frame fraSample3 
         Caption         =   "Sample 3"
         Height          =   1785
         Left            =   1545
         TabIndex        =   6
         Top             =   675
         Width           =   2055
      End
   End
   Begin VB.PictureBox picOptions 
      BorderStyle     =   0  'None
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   3780
      Index           =   1
      Left            =   -20000
      ScaleHeight     =   3780
      ScaleWidth      =   5685
      TabIndex        =   2
      TabStop         =   0   'False
      Top             =   480
      Width           =   5685
      Begin VB.Frame fraSample2 
         Caption         =   "Sample 2"
         Height          =   1785
         Left            =   645
         TabIndex        =   5
         Top             =   300
         Width           =   2055
      End
   End
   Begin VB.CommandButton cmdCancel 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   3840
      TabIndex        =   1
      Top             =   3255
      Width           =   1095
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      BeginProperty Font 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   375
      Left            =   2610
      TabIndex        =   0
      Top             =   3255
      Width           =   1095
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
    If Validate() Then
        glHTTPTimeout = CLng(txtHTTPTimeout.Text)
        gbSavePlateXML = chkSavePlateXML.value
        gbSaveCompoundXML = chkSaveCompoundXML.value
        gbSaveContainerXML = chkSaveContainerXML.value
        Call SetGlobals
        glContainerLimit = CLng(txtContainerLimit.Text)
        Unload Me
    End If
End Sub

Private Function Validate() As Boolean
    Validate = False
    If Not CheckLong(txtHTTPTimeout.Text) Then
        MsgBox ("Timeout must be a number")
        txtHTTPTimeout.SetFocus
    ElseIf CLng(txtHTTPTimeout.Text) < 10 Then
        MsgBox ("Timeout must be at least 10")
        txtHTTPTimeout.SetFocus
    ElseIf Not CheckLong(txtContainerLimit.Text) Then
        MsgBox ("Container Limit must be a number")
        txtContainerLimit.SetFocus
    Else
        Validate = True
    End If
End Function

Private Sub Form_Load()
    txtHTTPTimeout.Text = glHTTPTimeout
    txtContainerLimit.Text = glContainerLimit
    chkDoNotSendXML.Visible = False
    
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

Private Sub txtHTTPTimeout_LostFocus()
    If Not IsNumeric(txtHTTPTimeout.Text) Then
        txtHTTPTimeout.Text = DEFAULT_HTTPTIMEOUT
    Else
        If CLng(txtHTTPTimeout.Text) < 10 Then
            txtHTTPTimeout.Text = 10
        End If
    End If
End Sub
