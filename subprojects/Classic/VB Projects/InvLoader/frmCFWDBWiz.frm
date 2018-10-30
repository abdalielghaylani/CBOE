VERSION 5.00
Object = "{D76D7128-4A96-11D3-BD95-D296DC2DD072}#1.0#0"; "Vsflex7.ocx"
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSCOMCTL.OCX"
Object = "{0AE5E57B-4690-4360-A55F-5C3BC18DB4CC}#1.0#0"; "ACTIVE~1.OCX"
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "COMDLG32.OCX"
Begin VB.Form frmCFWDBWiz 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Inventory Loader"
   ClientHeight    =   5880
   ClientLeft      =   5685
   ClientTop       =   4470
   ClientWidth     =   7440
   Icon            =   "frmCFWDBWiz.frx":0000
   LinkTopic       =   "Form1"
   LockControls    =   -1  'True
   MaxButton       =   0   'False
   ScaleHeight     =   5880
   ScaleWidth      =   7440
   Begin Active_Wizard.ActivePane ActivePane7 
      Height          =   4605
      Left            =   8520
      TabIndex        =   14
      Top             =   4920
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Setup New Plates"
      HeaderDescription=   "Please enter the information about the type of new plates you wish to create."
      WizardMode      =   1
      NextPane        =   8
      PaneOrder       =   7
      PreviousPane    =   6
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.ComboBox cmbLocation 
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   315
         Left            =   600
         Style           =   2  'Dropdown List
         TabIndex        =   55
         Top             =   840
         Width           =   5655
      End
      Begin VB.ComboBox cmbPlateType 
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   315
         Left            =   2160
         Style           =   2  'Dropdown List
         TabIndex        =   45
         Top             =   1920
         Width           =   4095
      End
      Begin VB.ComboBox cmbPlateFormat 
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   315
         Left            =   2160
         Style           =   2  'Dropdown List
         TabIndex        =   18
         Top             =   1440
         Width           =   4095
      End
      Begin VB.Label lblNumPlates 
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
         Left            =   2280
         TabIndex        =   23
         Top             =   2760
         Width           =   4095
      End
      Begin VB.Label Label22 
         Caption         =   "Put new plates in:"
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
         Left            =   600
         TabIndex        =   56
         Top             =   480
         Width           =   1935
      End
      Begin VB.Label Label15 
         Caption         =   "Plate Type:"
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
         Left            =   1200
         TabIndex        =   46
         Top             =   1920
         Width           =   1335
      End
      Begin VB.Label Label12 
         Caption         =   "Number of Plates:"
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
         Left            =   720
         TabIndex        =   22
         Top             =   2760
         Width           =   1335
      End
      Begin VB.Label lblNumCompounds 
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
         Left            =   2280
         TabIndex        =   21
         Top             =   2400
         Width           =   1815
      End
      Begin VB.Label Label11 
         Caption         =   "Number of Compounds:"
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
         TabIndex        =   20
         Top             =   2400
         Width           =   1935
      End
      Begin VB.Label Label10 
         Caption         =   "Plate Format:"
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
         Left            =   1080
         TabIndex        =   19
         Top             =   1440
         Width           =   975
      End
   End
   Begin Active_Wizard.ActivePane ActivePane5 
      Height          =   4605
      Left            =   9960
      TabIndex        =   86
      Top             =   600
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Update Selections"
      HeaderDescription=   "Select whether and how updates are performed"
      WizardMode      =   1
      NextPane        =   6
      PaneOrder       =   5
      PreviousPane    =   4
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.Frame fraUpdateOptions 
         Caption         =   "Update Options (Containers only)"
         Height          =   2775
         Left            =   480
         TabIndex        =   93
         Top             =   480
         Width           =   5295
         Begin VB.Frame frmCompoundUpdateOptions 
            Caption         =   "Compound Update Options"
            Height          =   1095
            Left            =   360
            TabIndex        =   98
            Top             =   1440
            Width           =   4335
            Begin VB.OptionButton optCompoundUpdate 
               Caption         =   "Update compounds including non-null to null"
               Height          =   195
               Index           =   2
               Left            =   240
               TabIndex        =   101
               Top             =   1080
               Visible         =   0   'False
               Width           =   3615
            End
            Begin VB.OptionButton optCompoundUpdate 
               Caption         =   "Update compounds to non-null values only"
               Height          =   255
               Index           =   1
               Left            =   240
               TabIndex        =   100
               Top             =   720
               Width           =   3615
            End
            Begin VB.OptionButton optCompoundUpdate 
               Caption         =   "No compound updates"
               Height          =   255
               Index           =   0
               Left            =   240
               TabIndex        =   99
               Top             =   360
               Value           =   -1  'True
               Width           =   3615
            End
         End
         Begin VB.OptionButton optUpdate 
            Caption         =   "Update including non-null to null"
            Height          =   255
            Index           =   2
            Left            =   600
            TabIndex        =   96
            Top             =   1080
            Width           =   3255
         End
         Begin VB.OptionButton optUpdate 
            Caption         =   "Update to non-null values only"
            Height          =   255
            Index           =   1
            Left            =   600
            TabIndex        =   95
            Top             =   720
            Width           =   3255
         End
         Begin VB.OptionButton optUpdate 
            Caption         =   "No updates "
            Height          =   255
            Index           =   0
            Left            =   600
            TabIndex        =   94
            Top             =   360
            Value           =   -1  'True
            Width           =   3255
         End
      End
      Begin VB.Label lblUpdateWarning 
         Caption         =   "Updated containers must be identified by either barcode or compound ID"
         Height          =   615
         Left            =   600
         TabIndex        =   97
         Top             =   3600
         Width           =   4575
      End
   End
   Begin Active_Wizard.ActivePane ActivePane6 
      Height          =   4605
      Left            =   9600
      TabIndex        =   13
      Top             =   600
      Width           =   7215
      _ExtentX        =   12726
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Select Fields"
      HeaderDescription=   "Please select the fields from the source database that contain the specified information. "
      WizardMode      =   1
      NextPane        =   7
      PaneOrder       =   6
      PreviousPane    =   5
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.CommandButton cmdSaveChemMappings 
         Caption         =   "Save"
         Height          =   375
         Left            =   1320
         TabIndex        =   77
         Top             =   3840
         Width           =   975
      End
      Begin VB.CommandButton cmdLoadChemMappings 
         Caption         =   "Load"
         Height          =   375
         Left            =   240
         TabIndex        =   76
         Top             =   3840
         Width           =   975
      End
      Begin VB.ComboBox cmbMolTable 
         Height          =   315
         Left            =   2640
         Style           =   2  'Dropdown List
         TabIndex        =   72
         Top             =   480
         Width           =   2175
      End
      Begin VSFlex7Ctl.VSFlexGrid grdFieldDict 
         Height          =   2775
         Left            =   240
         TabIndex        =   16
         Top             =   960
         Width           =   6735
         _cx             =   11880
         _cy             =   4895
         _ConvInfo       =   1
         Appearance      =   1
         BorderStyle     =   1
         Enabled         =   -1  'True
         BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         MousePointer    =   0
         BackColor       =   -2147483643
         ForeColor       =   -2147483640
         BackColorFixed  =   -2147483633
         ForeColorFixed  =   -2147483630
         BackColorSel    =   -2147483635
         ForeColorSel    =   -2147483634
         BackColorBkg    =   -2147483636
         BackColorAlternate=   -2147483643
         GridColor       =   -2147483633
         GridColorFixed  =   -2147483632
         TreeColor       =   -2147483632
         FloodColor      =   192
         SheetBorder     =   -2147483642
         FocusRect       =   1
         HighLight       =   1
         AllowSelection  =   -1  'True
         AllowBigSelection=   -1  'True
         AllowUserResizing=   0
         SelectionMode   =   0
         GridLines       =   1
         GridLinesFixed  =   2
         GridLineWidth   =   1
         Rows            =   50
         Cols            =   10
         FixedRows       =   1
         FixedCols       =   1
         RowHeightMin    =   0
         RowHeightMax    =   0
         ColWidthMin     =   0
         ColWidthMax     =   0
         ExtendLastCol   =   0   'False
         FormatString    =   ""
         ScrollTrack     =   0   'False
         ScrollBars      =   3
         ScrollTips      =   0   'False
         MergeCells      =   0
         MergeCompare    =   0
         AutoResize      =   -1  'True
         AutoSizeMode    =   0
         AutoSearch      =   0
         AutoSearchDelay =   2
         MultiTotals     =   -1  'True
         SubtotalPosition=   1
         OutlineBar      =   0
         OutlineCol      =   0
         Ellipsis        =   0
         ExplorerBar     =   0
         PicturesOver    =   0   'False
         FillStyle       =   0
         RightToLeft     =   0   'False
         PictureType     =   0
         TabBehavior     =   0
         OwnerDraw       =   0
         Editable        =   0
         ShowComboButton =   1
         WordWrap        =   0   'False
         TextStyle       =   0
         TextStyleFixed  =   0
         OleDragMode     =   0
         OleDropMode     =   0
         DataMode        =   0
         VirtualData     =   -1  'True
         DataMember      =   ""
         ComboSearch     =   3
         AutoSizeMouse   =   -1  'True
         FrozenRows      =   0
         FrozenCols      =   0
         AllowUserFreezing=   0
         BackColorFrozen =   0
         ForeColorFrozen =   0
         WallPaperAlignment=   9
      End
      Begin VB.Label Label28 
         Caption         =   "Select table:"
         Height          =   375
         Left            =   1440
         TabIndex        =   71
         Top             =   480
         Width           =   1575
      End
      Begin VB.Label Label9 
         Caption         =   "To use a default value, type it in the rightmost column."
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
         Left            =   240
         TabIndex        =   17
         Top             =   480
         Width           =   6255
      End
   End
   Begin Active_Wizard.ActivePane ActivePane11 
      Height          =   4605
      Left            =   600
      TabIndex        =   15
      Top             =   7200
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      IsLastPane      =   -1  'True
      HeaderTitle     =   "Ready to Import"
      WizardMode      =   1
      NextPane        =   11
      PaneOrder       =   11
      PreviousPane    =   10
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.Frame frmOverallProg 
         Caption         =   "Overall Progress"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   1335
         Left            =   240
         TabIndex        =   25
         Top             =   2280
         Width           =   6255
         Begin MSComctlLib.ProgressBar progOverall 
            Height          =   375
            Left            =   240
            TabIndex        =   26
            Top             =   720
            Width           =   5775
            _ExtentX        =   10186
            _ExtentY        =   661
            _Version        =   393216
            Appearance      =   1
         End
         Begin VB.Label lblOverallProg 
            Caption         =   "Initializing..."
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
            Left            =   240
            TabIndex        =   29
            Top             =   360
            Width           =   5775
         End
      End
      Begin VB.Frame frmPlateProg 
         Caption         =   "Plate Progress"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   1335
         Left            =   240
         TabIndex        =   24
         Top             =   480
         Width           =   6255
         Begin MSComctlLib.ProgressBar progPlate 
            Height          =   375
            Left            =   240
            TabIndex        =   27
            Top             =   720
            Width           =   5775
            _ExtentX        =   10186
            _ExtentY        =   661
            _Version        =   393216
            Appearance      =   1
         End
         Begin VB.Label lblPlateProg 
            Caption         =   "Initializing..."
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
            Left            =   240
            TabIndex        =   28
            Top             =   360
            Width           =   5295
         End
      End
      Begin VB.Label lblNotify 
         Caption         =   "Press Finish to begin importing the data."
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
         Left            =   240
         TabIndex        =   30
         Top             =   1920
         Width           =   6255
      End
   End
   Begin Active_Wizard.ActivePane ActivePane10 
      Height          =   4605
      Left            =   360
      TabIndex        =   67
      Top             =   6840
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483648
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Log Plate XML"
      HeaderDescription=   "You can choose to log the output for future reference."
      WizardMode      =   1
      NextPane        =   11
      PaneOrder       =   10
      PreviousPane    =   9
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.CheckBox chkStartNewLoad 
         Caption         =   "Start new load when finished"
         Height          =   255
         Left            =   600
         TabIndex        =   83
         Top             =   2040
         Width           =   2535
      End
      Begin VB.CheckBox chkOpenLog 
         Caption         =   "Open log file when finished"
         Enabled         =   0   'False
         Height          =   255
         Left            =   600
         TabIndex        =   75
         Top             =   1680
         Width           =   3135
      End
      Begin VB.CommandButton btnAdvancedOptions 
         Caption         =   "Advanced Options"
         Height          =   375
         Left            =   600
         TabIndex        =   74
         Top             =   2640
         Width           =   1935
      End
      Begin VB.TextBox txtLogLoc 
         Enabled         =   0   'False
         Height          =   285
         Left            =   600
         TabIndex        =   70
         Text            =   "C:\Program Files (x86)\PerkinElmer\InvLoader\Logs"
         Top             =   960
         Width           =   3735
      End
      Begin VB.CheckBox chkLog 
         Caption         =   "Check1"
         Height          =   255
         Left            =   600
         TabIndex        =   68
         Top             =   600
         Width           =   255
      End
      Begin VB.Label Label29 
         Caption         =   "Note: The file path must already exist."
         Height          =   255
         Left            =   600
         TabIndex        =   73
         Top             =   1320
         Width           =   3015
      End
      Begin VB.Label Label14 
         Caption         =   "Log data to the following location:"
         Height          =   255
         Left            =   840
         TabIndex        =   69
         Top             =   600
         Width           =   3495
      End
   End
   Begin Active_Wizard.ActivePane ActivePane4 
      Height          =   4605
      Left            =   9720
      TabIndex        =   82
      Top             =   6000
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Registration Integration"
      HeaderDescription=   "Select integration with Registration"
      WizardMode      =   1
      NextPane        =   5
      PaneOrder       =   4
      PreviousPane    =   3
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.Frame fraListAttributes 
         Caption         =   "Registration list attributes"
         Height          =   1575
         Left            =   360
         TabIndex        =   89
         Top             =   1200
         Width           =   5175
         Begin VB.ComboBox cmbListSeparator 
            Height          =   315
            Left            =   1200
            Style           =   2  'Dropdown List
            TabIndex        =   91
            Top             =   810
            Width           =   3495
         End
         Begin VB.CheckBox chkListAttributes 
            Caption         =   "Enable list attributes where defined"
            Height          =   375
            Left            =   360
            TabIndex        =   90
            Top             =   360
            Width           =   4095
         End
         Begin VB.Label lblSeparator 
            Caption         =   "Separator:"
            Height          =   255
            Left            =   360
            TabIndex        =   92
            Top             =   840
            Width           =   855
         End
      End
      Begin VB.CheckBox chkRegister 
         Caption         =   "Register the new compounds in the CambridgeSoft Chemical Registration System"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   372
         Left            =   360
         TabIndex        =   87
         Top             =   600
         Width           =   6015
      End
   End
   Begin Active_Wizard.ActivePane ActivePane3 
      Height          =   4605
      Left            =   9360
      TabIndex        =   9
      Top             =   5640
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Select Database"
      HeaderDescription=   "Select the structure database you wish to import."
      WizardMode      =   1
      NextPane        =   4
      PaneOrder       =   3
      PreviousPane    =   2
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.Frame Frame3 
         Caption         =   "Load Options"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   1815
         Left            =   960
         TabIndex        =   78
         Top             =   360
         Width           =   4695
         Begin VB.OptionButton optLoad 
            Caption         =   "Load Structures into Containers"
            Height          =   255
            Index           =   1
            Left            =   240
            TabIndex        =   81
            Top             =   840
            Width           =   4335
         End
         Begin VB.OptionButton optLoad 
            Caption         =   "Load Structures into Plates"
            Height          =   255
            Index           =   0
            Left            =   240
            TabIndex        =   80
            Top             =   360
            Value           =   -1  'True
            Width           =   4335
         End
         Begin VB.OptionButton optLoad 
            Caption         =   "Load Structures Only"
            Height          =   255
            Index           =   2
            Left            =   240
            TabIndex        =   79
            Top             =   1320
            Width           =   4215
         End
      End
      Begin VB.CommandButton cmdBrowse 
         Caption         =   "Browse..."
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   372
         Left            =   4440
         TabIndex        =   11
         Top             =   3240
         Width           =   1215
      End
      Begin VB.TextBox txtDB 
         Height          =   312
         Left            =   960
         TabIndex        =   10
         Top             =   2880
         Width           =   4695
      End
      Begin MSComDlg.CommonDialog CommonDialog1 
         Left            =   3600
         Top             =   3480
         _ExtentX        =   847
         _ExtentY        =   847
         _Version        =   393216
      End
      Begin VB.Label lblChemOffice 
         Height          =   255
         Left            =   960
         TabIndex        =   84
         Top             =   3840
         Width           =   5175
      End
      Begin VB.Label Label8 
         Caption         =   "Structure Database:"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   375
         Left            =   960
         TabIndex        =   12
         Top             =   2520
         Width           =   2055
      End
   End
   Begin Active_Wizard.ActivePane ActivePane2 
      Height          =   4605
      Left            =   8880
      TabIndex        =   57
      Top             =   5280
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Login to the Inventory Web Server"
      HeaderDescription=   "Enter the Server Name, User ID, and Password for the Inventory Web Server."
      WizardMode      =   1
      NextPane        =   3
      PaneOrder       =   2
      PreviousPane    =   1
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.CheckBox chkUseSSL 
         Alignment       =   1  'Right Justify
         Caption         =   "Use SSL (https:)"
         Height          =   375
         Left            =   1800
         TabIndex        =   85
         Top             =   2400
         Width           =   2535
      End
      Begin VB.TextBox txtServer 
         Height          =   375
         Left            =   2760
         TabIndex        =   59
         Top             =   744
         Width           =   1575
      End
      Begin VB.TextBox txtPassword 
         Height          =   375
         IMEMode         =   3  'DISABLE
         Left            =   2760
         PasswordChar    =   "*"
         TabIndex        =   63
         Top             =   1800
         Width           =   1575
      End
      Begin VB.TextBox txtUserID 
         Height          =   375
         Left            =   2760
         TabIndex        =   62
         Top             =   1272
         Width           =   1575
      End
      Begin VB.Label Label25 
         Caption         =   "Server:"
         Height          =   375
         Left            =   1800
         TabIndex        =   61
         Top             =   744
         Width           =   855
      End
      Begin VB.Label Label24 
         Caption         =   "Password:"
         Height          =   375
         Left            =   1800
         TabIndex        =   60
         Top             =   1800
         Width           =   855
      End
      Begin VB.Label Label23 
         Caption         =   "User ID:"
         Height          =   375
         Left            =   1800
         TabIndex        =   58
         Top             =   1272
         Width           =   855
      End
   End
   Begin Active_Wizard.ActivePane ActivePane9 
      Height          =   4605
      Left            =   240
      TabIndex        =   47
      Top             =   6480
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Barcoding"
      HeaderDescription=   "Please enter the barcode and group information for the new plates."
      WizardMode      =   1
      NextPane        =   10
      PaneOrder       =   9
      PreviousPane    =   8
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.ComboBox cmbPrefix 
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   315
         Index           =   1
         Left            =   1800
         TabIndex        =   53
         Text            =   "Combo1"
         Top             =   915
         Width           =   750
      End
      Begin VSFlex7Ctl.VSFlexGrid grdBarcodes 
         Height          =   2415
         Left            =   600
         TabIndex        =   50
         Top             =   1320
         Width           =   5295
         _cx             =   9340
         _cy             =   4260
         _ConvInfo       =   1
         Appearance      =   1
         BorderStyle     =   1
         Enabled         =   -1  'True
         BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         MousePointer    =   0
         BackColor       =   -2147483643
         ForeColor       =   -2147483640
         BackColorFixed  =   -2147483633
         ForeColorFixed  =   -2147483630
         BackColorSel    =   -2147483635
         ForeColorSel    =   -2147483634
         BackColorBkg    =   -2147483636
         BackColorAlternate=   -2147483643
         GridColor       =   -2147483633
         GridColorFixed  =   -2147483632
         TreeColor       =   -2147483632
         FloodColor      =   192
         SheetBorder     =   -2147483642
         FocusRect       =   1
         HighLight       =   1
         AllowSelection  =   -1  'True
         AllowBigSelection=   -1  'True
         AllowUserResizing=   0
         SelectionMode   =   0
         GridLines       =   1
         GridLinesFixed  =   2
         GridLineWidth   =   1
         Rows            =   50
         Cols            =   10
         FixedRows       =   1
         FixedCols       =   1
         RowHeightMin    =   0
         RowHeightMax    =   0
         ColWidthMin     =   0
         ColWidthMax     =   0
         ExtendLastCol   =   0   'False
         FormatString    =   ""
         ScrollTrack     =   0   'False
         ScrollBars      =   3
         ScrollTips      =   0   'False
         MergeCells      =   0
         MergeCompare    =   0
         AutoResize      =   -1  'True
         AutoSizeMode    =   0
         AutoSearch      =   0
         AutoSearchDelay =   2
         MultiTotals     =   -1  'True
         SubtotalPosition=   1
         OutlineBar      =   0
         OutlineCol      =   0
         Ellipsis        =   0
         ExplorerBar     =   0
         PicturesOver    =   0   'False
         FillStyle       =   0
         RightToLeft     =   0   'False
         PictureType     =   0
         TabBehavior     =   0
         OwnerDraw       =   0
         Editable        =   0
         ShowComboButton =   1
         WordWrap        =   0   'False
         TextStyle       =   0
         TextStyleFixed  =   0
         OleDragMode     =   0
         OleDropMode     =   0
         DataMode        =   0
         VirtualData     =   -1  'True
         DataMember      =   ""
         ComboSearch     =   3
         AutoSizeMouse   =   -1  'True
         FrozenRows      =   0
         FrozenCols      =   0
         AllowUserFreezing=   0
         BackColorFrozen =   0
         ForeColorFrozen =   0
         WallPaperAlignment=   9
      End
      Begin VB.Label Label19 
         Caption         =   "Barcode Prefix:"
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
         Left            =   600
         TabIndex        =   54
         Top             =   960
         Width           =   2655
      End
      Begin VB.Label Label17 
         Caption         =   "Please enter the barcode and group numbers for the new plates:"
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
         Left            =   240
         TabIndex        =   51
         Top             =   480
         Width           =   6135
      End
      Begin VB.Label Label21 
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
         Left            =   2280
         TabIndex        =   49
         Top             =   600
         Width           =   4095
      End
      Begin VB.Label Label20 
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
         Left            =   2160
         TabIndex        =   48
         Top             =   480
         Width           =   1815
      End
   End
   Begin Active_Wizard.ActivePane ActivePane1 
      Height          =   4365
      Left            =   8400
      TabIndex        =   1
      Top             =   4920
      Width           =   7215
      _ExtentX        =   12726
      _ExtentY        =   7699
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Import Database"
      HeaderDescription=   "This wizard will help you import a ChemFinder or Excel database of compounds."
      WizardMode      =   1
      NextPane        =   2
      PaneOrder       =   1
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.Label lblVersion 
         Alignment       =   1  'Right Justify
         Caption         =   "Version"
         Height          =   255
         Left            =   4320
         TabIndex        =   88
         Top             =   3960
         Width           =   2775
      End
      Begin VB.Label Label7 
         Caption         =   "Open the SDFile you wish to import"
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
         Left            =   720
         TabIndex        =   8
         Top             =   2040
         Width           =   5055
      End
      Begin VB.Label Label6 
         Caption         =   "When you are ready to begin, click Next."
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
         TabIndex        =   7
         Top             =   3480
         Width           =   5055
      End
      Begin VB.Label Label5 
         Caption         =   "Note where you saved the finished ChemFinder database."
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
         Left            =   720
         TabIndex        =   6
         Top             =   2760
         Width           =   5055
      End
      Begin VB.Label Label4 
         Caption         =   "Accept all the defaults that the wizard presents you"
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
         Left            =   720
         TabIndex        =   5
         Top             =   2400
         Width           =   5055
      End
      Begin VB.Label Label3 
         Caption         =   "Choose Import SDFile from the File Menu."
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
         Left            =   720
         TabIndex        =   4
         Top             =   1680
         Width           =   5055
      End
      Begin VB.Label Label2 
         Caption         =   "Open ChemFinder (Start | Programs | ChemOffice | ChemFinder)"
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
         Left            =   720
         TabIndex        =   3
         Top             =   1320
         Width           =   5055
      End
      Begin VB.Label Label1 
         Caption         =   "Filled in by frmCFWDBWiz.Initialize()"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   855
         Left            =   240
         TabIndex        =   2
         Top             =   360
         Width           =   6255
      End
   End
   Begin Active_Wizard.ActivePane ActivePane8 
      Height          =   4605
      Left            =   120
      TabIndex        =   31
      Top             =   6120
      Width           =   6735
      _ExtentX        =   11880
      _ExtentY        =   8123
      BackColor       =   -2147483633
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      HeaderTitle     =   "Setup New Plates"
      HeaderDescription=   "Please enter the information about the type of new plates you wish to create."
      WizardMode      =   1
      NextPane        =   9
      PaneOrder       =   8
      PreviousPane    =   7
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Tahoma"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Begin VB.ComboBox cmbLibrary 
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   315
         Left            =   1680
         Style           =   2  'Dropdown List
         TabIndex        =   39
         Top             =   480
         Width           =   4095
      End
      Begin VB.Frame Frame1 
         Caption         =   "Barcodes"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   1815
         Left            =   240
         TabIndex        =   38
         Top             =   960
         Width           =   5895
         Begin VB.ComboBox cmbBarcodeField 
            Height          =   315
            Left            =   2880
            TabIndex        =   66
            Text            =   "cmbBarcodeField"
            Top             =   1320
            Width           =   2895
         End
         Begin VB.OptionButton optBarcode 
            Caption         =   "Use mapped field:"
            Height          =   255
            Index           =   3
            Left            =   240
            TabIndex        =   65
            Top             =   1320
            Value           =   -1  'True
            Width           =   3200
         End
         Begin VB.ComboBox cmbBarcodeDesc 
            Height          =   315
            Left            =   2880
            TabIndex        =   64
            Text            =   "cmbBarcodeDesc"
            Top             =   240
            Width           =   2895
         End
         Begin VB.OptionButton optBarcode 
            Caption         =   "Select a Barcode Description:"
            BeginProperty Font 
               Name            =   "Tahoma"
               Size            =   8.25
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   195
            Index           =   0
            Left            =   240
            TabIndex        =   42
            Top             =   240
            Width           =   3255
         End
         Begin VB.ComboBox cmbPrefix 
            BeginProperty Font 
               Name            =   "Tahoma"
               Size            =   8.25
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   315
            Index           =   0
            Left            =   3720
            TabIndex        =   52
            Text            =   "Combo1"
            Top             =   600
            Width           =   750
         End
         Begin VB.OptionButton optBarcode 
            Caption         =   "Allow me to assign the barcode numbers"
            BeginProperty Font 
               Name            =   "Tahoma"
               Size            =   8.25
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   195
            Index           =   2
            Left            =   240
            TabIndex        =   44
            Top             =   960
            Width           =   3735
         End
         Begin VB.OptionButton optBarcode 
            Caption         =   "Assign ascending numbers, starting with:"
            BeginProperty Font 
               Name            =   "Tahoma"
               Size            =   8.25
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   195
            Index           =   1
            Left            =   360
            TabIndex        =   43
            Top             =   600
            Width           =   3975
         End
         Begin VB.TextBox txtBarcodeStart 
            BeginProperty Font 
               Name            =   "Tahoma"
               Size            =   8.25
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   285
            Left            =   4560
            TabIndex        =   41
            Top             =   600
            Width           =   1095
         End
      End
      Begin VB.Frame Frame2 
         Caption         =   "Grouping"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   1335
         Left            =   240
         TabIndex        =   32
         Top             =   2880
         Width           =   5895
         Begin VB.OptionButton optGrp 
            Caption         =   "Allow me to assign the group numbers"
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
            Index           =   1
            Left            =   240
            TabIndex        =   35
            Top             =   720
            Width           =   3735
         End
         Begin VB.OptionButton optGrp 
            Caption         =   "Assign ascending group names, starting with:"
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
            Index           =   0
            Left            =   240
            TabIndex        =   34
            Top             =   360
            Value           =   -1  'True
            Width           =   3855
         End
         Begin VB.TextBox txtGroupStart 
            BeginProperty Font 
               Name            =   "Tahoma"
               Size            =   8.25
               Charset         =   0
               Weight          =   400
               Underline       =   0   'False
               Italic          =   0   'False
               Strikethrough   =   0   'False
            EndProperty
            Height          =   285
            Left            =   4080
            TabIndex        =   33
            Top             =   360
            Width           =   1695
         End
      End
      Begin VB.Label Label13 
         Caption         =   "Assign to Library:"
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
         Left            =   240
         TabIndex        =   40
         Top             =   480
         Width           =   1335
      End
      Begin VB.Label Label18 
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
         Left            =   2160
         TabIndex        =   37
         Top             =   480
         Width           =   1815
      End
      Begin VB.Label Label16 
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
         Left            =   2160
         TabIndex        =   36
         Top             =   960
         Width           =   4095
      End
   End
   Begin Active_Wizard.ActiveWizard ActiveWizard1 
      Height          =   5895
      Left            =   -120
      TabIndex        =   0
      Top             =   -120
      Width           =   7455
      _ExtentX        =   13150
      _ExtentY        =   10398
      FinishMode      =   2
      BackButtonEnabled=   -1  'True
   End
End
Attribute VB_Name = "frmCFWDBWiz"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private moFieldXML As MSXML2.DOMDocument

Private mbGridLoading As Boolean
Private moDictFields As Dictionary
Private msaDictKeys As Variant
' Private moRegDict As Dictionary
' Private moCompoundDictFields As Dictionary 'kfdx
' Private moContainerDictFields As Dictionary 'kfdx
'Private moRegOptionFields As Dictionary
Private moLocationBarcodeMapping As Dictionary
Private moGroupSecurityMapping As Dictionary
Private moRegistrationNumberMapping As Dictionary
Private mbListAttributes As Boolean

Private msFile As String
Private mlPerPlate As Long
Private mlNumCompounds As Long
Private mlNumPlates As Long
Private mbFinished As Boolean
Private mlPlateType As Long
Private mlPlateFormat As Long
Private miLoadOption As LoadOptions
Private miUpdateMode As UpdateOptions
Private miUpdateCompoundMode As UpdateOptions
Private msTest As Integer

Private mlNewLocation As Long
Private mlLibrary As Long

Private moCFWImporter As DataImporter

'Private mbShowRegister As Boolean
Private mbRegAvailable As Boolean
Private mbRegister As Boolean
Private mbCustomBarcodes As Boolean
Private mbINV_CREATE_PLATE As Boolean
Private mbINV_MANAGE_SUBSTANCES As Boolean
Private mbINV_CREATE_CONTAINER As Boolean

' plate locator information
Private mbHaveWellCoords As Boolean
Private mbHavePlateCoords As Boolean
Private msWellCoordField As String
Private msPlateCoordField As String
Private msRowField As String
Private msColField As String
Private mbBarcodesFromFile As Boolean

Private moDictBarcodes As Dictionary
Private moDictGroups As Dictionary
Public LocationID As Long
Public bNoLocations As Boolean
Public bNoFormats As Boolean
Private sCoordinateFormat As String
Public bDebug As Boolean
Private sErrorMsg As String

Public moApplicationVariablesDict As Dictionary
Private xmlLogPath As String
Private msToLog As String
Public RLSStatus As String

' Dim oOwner() As OwnerClass

Private Enum LoadOptions
    ePlates = 0
    eContainers
    eStructures
End Enum

Private Enum UpdateOptions
    eUpdateNone = 0
    eUpdateNonNull
    eUpdateToNull
End Enum

Private Enum Panes
    ePaneIntro = 1
    ePaneLogin
    ePaneFile
    ePaneRegistration
    ePaneUpdate
    ePaneMapping
    ePaneFormat
    ePaneLibrary
    ePaneBarcodes
    ePaneLog
    ePaneFinish
End Enum

Public Enum EnumGridColumns
    eDisplayName = 0
    eDataType
    eMapping
    eValue
End Enum

Public Property Get NoInv() As Boolean
    NoInv = bNoLocations
End Property

Private Function InitDBField(ByVal Designator As String, _
                             ByVal Field As String, _
                             ByVal eFieldSubstClass As FieldSubstanceClass, _
                             ByVal Display As String, _
                             ByVal FieldType As FieldTypeEnum, _
                             ByVal Width As Integer, _
                             ByVal Required As Boolean, _
                             ByVal CanBeList As Boolean, _
                             ByVal PicklistName As String, _
                             ByVal InitValue As String, _
                             ByVal XMLParent As String, _
                             ByVal XMLPosition As String) As DBField
    Set InitDBField = New DBField
    InitDBField.Designator = Designator
    InitDBField.eFieldSubstClass = eFieldSubstClass
    InitDBField.FieldName = Field
    InitDBField.DisplayName = Display
    InitDBField.eFieldType = FieldType
    InitDBField.FieldWidth = Width
    InitDBField.Required = Required
    InitDBField.CanBeList = CanBeList
    InitDBField.PicklistName = PicklistName
    InitDBField.value = InitValue
    InitDBField.XMLParent = XMLParent
    InitDBField.XMLPosition = XMLPosition
End Function

Public Sub Initialize()
    Dim oDict As Dictionary, oDict2 As Dictionary, oDict3 As Dictionary
'    Dim oDict4 As Dictionary, oDict5 As Dictionary, oDict6 As Dictionary, oDict7 As Dictionary, oDict8 As Dictionary, oDict9 As Dictionary, oDict10 As Dictionary, oDict11 As Dictionary
    Dim response As String
    Dim aValuePairs, aVariable
    Dim i
    
    On Error GoTo Error    ' kfd2
    bNoLocations = False
    bNoFormats = False
    
    'kfd2
    LoadFieldXML msServerName, mbUseSSL
    'SetDBFields
        
    Set moCFWImporter = New DataImporter
    lblChemOffice.Caption = moCFWImporter.msChemOffice
    
    'Set oDict = DictionaryFromRecordset(ExecuteStatement("select location_id, location_name from inv_vw_plate_locations where location_id not in (select location_id from inv_vw_plate_grid_locations)"), "location_id", "location_name")
    Set oDict = DictionaryFromRecordset(ExecuteStatement(eStmtPlateLocations), "location_id", "location_name")
    Set oDict2 = DictionaryFromRecordset(ExecuteStatement(eStmtPlateFormats), 0, 0)
    Set oDict3 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerLocations), "location_id", "location_name_barcode")
    
    'There are no inventory plate locations.
    If oDict.count = 0 Then bNoLocations = True
    
    'There are no plate formats.
    If oDict2.count = 0 Then bNoFormats = True
    
    If Not bNoLocations And Not bNoFormats Then
        ' fill combos
        ComboFill cmbLocation, oDict
         
        ComboSelectItemData cmbLocation, LocationID
        ComboFill cmbPlateFormat, DictionaryFromRecordset(ExecuteStatement(eStmtPlateFormatFull), "plate_format_id", "plate_format_name")
        ComboFill cmbPlateType, DictionaryFromRecordset(ExecuteStatement(eStmtPlateTypes), "plate_type_id", "plate_type_name")
        ComboFill cmbLibrary, DictionaryFromRecordset(ExecuteStatement(eStmtLibraries), "enum_id", "enum_value")
        ComboFill cmbBarcodeDesc, DictionaryFromRecordset(ExecuteStatement(eStmtBarcodeDescs), "barcode_desc_id", "barcode_desc_name")
        Dim rs As Recordset
        Set rs = ExecuteStatement(eStmtBarcodePrefixes)
        ComboFill cmbPrefix(0), DictionaryFromRecordset(rs, 0, 1)
        ComboFill cmbPrefix(1), DictionaryFromRecordset(rs, 0, 1)
        ' Set rs = ExecuteStatement("select max(group_id_fk) from inv_plates")
        txtGroupStart = ""
        
        ' TODO -  fix this so based on prefix
        ' Set rs = ExecuteStatement("select max(barcode_number) from inv_barcode")
        txtBarcodeStart = ""
        
    End If
    
    response = UtilsInventory.GetApplicationVariables()
    aValuePairs = Split(response, "||")
    For i = 0 To UBound(aValuePairs)
        aVariable = Split(aValuePairs(i), "::")
        If (UBound(aVariable) = 1) Then
            moApplicationVariablesDict(UCase(aVariable(0))) = aVariable(1)
        End If
    Next
    
    'SYAN moved reg authentication to chkRegister_Click because it is slow 7/9/2004
    ''check if this user is authorized to register compounds
    'response = HTTPRequest("POST", msServerName, RegAPIFolder & "/reg_post_action.asp", "", "reg_method=reg_perm&USER_ID=" & msUserID & "&USER_PWD=" & msPassword & "&REG_PARAMETER=AUTHENTICATE")
    'If response = "user authenticated" Then
    '    'fill in reg combos
    '    FillRegOptionGrid
    'Else
    '    mbShowRegister = False
    'End If
    Exit Sub
Error:
    MsgBox "Failed to intitialize, possible connection failure", vbCritical
        'CSBR 148452 SJ Commenting the line to avoid runtime error
    'Err.Raise vbError + 1, , "Initialize error.  InvLoader exiting."
End Sub

Private Sub ActiveWizard1_Cancel()
    Me.Hide
    Me.Cls
    End
End Sub

Private Sub ActiveWizard1_Finish()
    
    If mbFinished Then
        Me.Hide
        Me.Cls
        End
        'Exit Sub
    End If

    ActiveWizard1.FinishButtonEnabled = False
    mlNewLocation = GetNewLocation
    ' disable cancel button
    ActiveWizard1.FinishMode = ShowFinishOnly
    
    LOG_ACTUAL_FILE_NAME = Mid(LOG_FILE_NAME, 1, InStr(LOG_FILE_NAME, ".") - 1) & Format(Now(), "mmddyyyy_hnn") & Mid(LOG_FILE_NAME, InStr(LOG_FILE_NAME, "."), Len(LOG_FILE_NAME))
    If chkLog.value = 1 Then
        LogAction "<INV_LOADER TIME_STAMP=""" & Now & """>", xmlLogPath
    End If

    ' set up progress report
    lblNotify.Visible = False
    frmOverallProg.Visible = True
    progOverall.Max = mlNumCompounds + 1
    progOverall.Min = 1
    
    
    If miLoadOption = eContainers Then
        progOverall.value = 1
        Call CreateContainerXML
    ElseIf miLoadOption = ePlates Then
        Call CreatePlateXML
    ElseIf miLoadOption = eStructures Then
        Call CreateSubstanceXML
    End If
    
    moCFWImporter.CloseDB   ' kfd
    
    If chkLog.value = 1 Then
        LogAction "</INV_LOADER>", xmlLogPath
        If chkOpenLog.value = 1 Then
            OpenLog xmlLogPath, Me.hWnd
        End If
    End If
        
    If chkStartNewLoad.value = 1 Then
        ActiveWizard1.FinishMode = ShowBackNext
        txtDB.Text = ""
        ActiveWizard1.ViewPane ePaneFile
    Else
        frmPlateProg.Visible = False
        frmOverallProg.Visible = False
        lblNotify.Caption = "Import done.  Press Finish to exit."
        lblNotify.Visible = True
        ActiveWizard1.FinishButtonEnabled = True
        mbFinished = True
    End If
End Sub


' This function will eventaully replace GetProperty.  It differs by accounting for picklist
' conversions
Private Function GetProperty2(ByVal PropertyId As String, ByRef rs As Recordset) As String
    Dim oField As DBField
    Dim LocaleDate As Date
    
    Set oField = moDictFields(PropertyId)

    If oField.SDFileField <> "" Then
        GetProperty2 = CnvString(rs(CLng(oField.SDFileField)).value, eDBtoVB)
    Else
        GetProperty2 = ""
    End If
    If GetProperty2 = "" Then
        GetProperty2 = oField.value
    End If
    
    If oField.PicklistName <> "" Then
        If Not oField.PickListLoaded Then
            Set oField.Picklist = GetFieldPicklistDict(oField)
        End If
        GetProperty2 = oField.Picklist(GetProperty2)
    End If
    If oField.eFieldType = eDate And GetProperty2 <> "" Then
        LocaleDate = Format(GetProperty2, moApplicationVariablesDict("DATE_FORMAT_STRING"))
        ' Now get the US/English equivalent since the webserver middle tier is expecting this format
        GetProperty2 = Month(LocaleDate) & "/" & Day(LocaleDate) & "/" & Year(LocaleDate)
    End If
End Function

' This function handles properties that can be lists.  It returns an array of property values
' in a variant
Private Function GetPropertyList(ByVal PropertyId As String, ByRef rs As Recordset) As Variant
    Dim oField As DBField
    Dim sValue As String
    Dim vValues As Variant
    Dim i As Integer
    Dim LocaleDate As Date
    
    Set oField = moDictFields(PropertyId)

    If oField.SDFileField <> "" Then
        sValue = CnvString(rs(CLng(oField.SDFileField)).value, eDBtoVB)
    Else
        sValue = ""
    End If
    If sValue = "" Then
        sValue = oField.value
        If oField.CanBeList Then
            vValues = Split(oField.value, "|")
        Else
            vValues = Array(sValue)
        End If
    Else
        If oField.CanBeList And mbListAttributes Then
            vValues = Split(sValue, msListSeparator)
        Else
            vValues = Array(sValue)
        End If
    End If
    
    
    
    For i = LBound(vValues) To UBound(vValues)
        sValue = vValues(i)
        If oField.PicklistName <> "" Then
            If Not oField.PickListLoaded Then
                Set oField.Picklist = GetFieldPicklistDict(oField)
            End If
            sValue = oField.Picklist(sValue)
        End If
        If oField.eFieldType = eDate Then
            If sValue <> "" Then
                LocaleDate = Format(sValue, moApplicationVariablesDict("DATE_FORMAT_STRING"))
                ' Now get the US/English equivalent since the webserver middle tier is expecting this format
                sValue = Month(LocaleDate) & "/" & Day(LocaleDate) & "/" & Year(LocaleDate)
            End If
        End If
        If sValue <> vValues(i) Then
            vValues(i) = sValue
        End If
    Next i
    GetPropertyList = vValues
End Function



Private Function GetProperty(ByVal PropertyId As String, ByRef rs As Recordset) As String
    Dim tfield As DBField
    Dim LocaleDate As Date
    Dim Property As String
    
    Set tfield = moDictFields(PropertyId)
'    If tfield.UseDefault Then
'        Property = tfield.value
'    Else
'        Property = CnvString(rs(CLng(tfield.SDFileField)).value, eDBtoVB)
'    End If
    If tfield.SDFileField <> "" Then
        Property = CnvString(rs(CLng(tfield.SDFileField)).value, eDBtoVB)
    Else
        Property = ""
    End If
    If Property = "" Then
        Property = tfield.value
    End If
    
    If tfield.eFieldType = eDate And Property <> "" And Not IsEmpty(Property) Then
        LocaleDate = Format(Property, moApplicationVariablesDict("DATE_FORMAT_STRING"))
        ' Now get the US/English equivalent since the webserver middle tier is expecting this format
        GetProperty = Month(LocaleDate) & "/" & Day(LocaleDate) & "/" & Year(LocaleDate)
    Else
        GetProperty = Property
    End If
End Function

Private Function HasProperty(ByVal PropertyId As String) As Boolean
    Dim tfield As DBField
'    Dim bReturn As Boolean
    
    Set tfield = moDictFields(PropertyId)

    If tfield.SDFileField <> "" Then
        HasProperty = True
    ElseIf Len(Trim(tfield.value)) > 0 Then
        HasProperty = True
    Else
        HasProperty = False
    End If
End Function

Private Sub ActiveWizard1_PaneChanged(ByVal OldPane As Integer, ByVal NewPane As Integer)
    Dim bWizStopped As Boolean
    Dim sErr As String: sErr = ""
    Dim bError, bFinish As Boolean
    Dim oAuthenticateXML As MSXML2.DOMDocument
        
    'CSBR-123147 jbattles 18-March-10
    'move this assignment to here because mbUseSSL needs to be set before Authentication is called
    mbUseSSL = (chkUseSSL = vbChecked)
    
    If OldPane = ePaneIntro And NewPane > OldPane Then
        ' get settings
        txtServer.Text = GetSetting(App.EXEName, "General", "Server", "")
        chkUseSSL.value = CheckboxValueFromText(GetSetting(App.EXEName, "General", "UseSSL", ""))
        txtUserID.Text = GetSetting(App.EXEName, "General", "UserID", "")
        txtServer.SetFocus
    
    ElseIf OldPane = ePaneLogin And NewPane > OldPane Then
        ' DoEvents
        Dim sAuthenticate As String
        Dim i As Integer
        Screen.MousePointer = vbHourglass
        Set oAuthenticateXML = New MSXML2.DOMDocument
        sAuthenticate = UtilsInventory.AuthenticateUser(txtServer.Text, txtUserID.Text, txtPassword.Text)
        Screen.MousePointer = vbDefault
        'WizStopNext ActiveWizard1, OldPane, sAuthenticate
        'MsgBox sAuthenticate
        
        If LCase(Left(sAuthenticate, 5)) = "error" Then
            sErr = sAuthenticate
            WizStopNext ActiveWizard1, OldPane, sErr
            bWizStopped = True
        Else
            oAuthenticateXML.loadXML (sAuthenticate)
            If oAuthenticateXML.selectNodes("ERRORS/ERROR").length > 0 Then
                For i = 0 To oAuthenticateXML.selectNodes("ERRORS/ERROR").length - 1
                    sErr = sErr & oAuthenticateXML.selectNodes("ERRORS/ERROR").Item(i).Text & Chr(13)
                Next
                WizStopNext ActiveWizard1, OldPane, sErr
                bWizStopped = True
            Else
                bError = False
                If oAuthenticateXML.selectNodes("//INV_CREATE_PLATE[.=1]").length > 0 Then
                    mbINV_CREATE_PLATE = True
                Else
                    mbINV_CREATE_PLATE = False
                End If
                
                If oAuthenticateXML.selectNodes("//INV_MANAGE_SUBSTANCES[.=1]").length > 0 Then
                    mbINV_MANAGE_SUBSTANCES = True
                Else
                    mbINV_MANAGE_SUBSTANCES = False
                End If
                
                If oAuthenticateXML.selectNodes("//INV_CREATE_CONTAINER[.=1]").length > 0 Then
                    mbINV_CREATE_CONTAINER = True
                Else
                    mbINV_CREATE_CONTAINER = False
                End If
                               
                msServerName = txtServer.Text
                msOraServiceName = oAuthenticateXML.documentElement.getAttribute("ORASERVICENAME")
                msInvSchemaName = oAuthenticateXML.documentElement.getAttribute("INVSCHEMANAME")
                msUserID = txtUserID.Text
                msPassword = txtPassword.Text
                'CSBR 160306 SJ To display correct info to the user in the case of insufficient cheminv privileges
                If Not mbINV_MANAGE_SUBSTANCES Then
                    sErr = "This user does not have the INV_MANAGE_SUBSTANCES privilege.  Please contact your system administrator."
                    bError = True
                End If
                If bError Then
                    WizStopNext ActiveWizard1, OldPane, sErr
                    bWizStopped = True
                    Exit Sub
                End If
                Initialize
                ' save servername and username for future use
                SaveSetting App.EXEName, "General", "Server", msServerName
                SaveSetting App.EXEName, "General", "UseSSL", BooleanToText(mbUseSSL)
                SaveSetting App.EXEName, "General", "UserID", msUserID
                               
                'If Not bError And Not mbINV_CREATE_PLATE And Not mbINV_CREATE_CONTAINER Then
                    'sErr = "This user does not have rights to create plates or containers.  Please contact your system administrator."
                    'bError = True
                'End If
                
                If bError Then
                    WizStopNext ActiveWizard1, OldPane, sErr
                    bWizStopped = True
                Else
                    optLoad(ePlates).Enabled = mbINV_CREATE_PLATE
                    optLoad(eContainers).Enabled = mbINV_CREATE_CONTAINER
                    optLoad(eStructures).Enabled = mbINV_MANAGE_SUBSTANCES
                    
                    If Not mbINV_CREATE_PLATE Then
                        If mbINV_CREATE_CONTAINER Then
                           optLoad(eContainers) = True
                        Else
                           optLoad(eStructures) = True
                        End If
                    End If
                    LookForRegistration
                End If
            End If      ' If oAuthenticateXML.selectNodes("ERRORS/ERROR").length > 0 Then
        End If      ' If LCase(Left(sAuthenticate, 5)) = "error" Then
    
    ElseIf OldPane = ePaneFile And NewPane > OldPane Then
        For i = ePlates To eStructures
            If optLoad(i) = True Then miLoadOption = i
        Next
        'CSBR-158781: Cannot login to invloader if there are no plate location in the system. When the user selects to upload plates only the checking is done for the required privileges.
        If miLoadOption = ePlates Then
            If Not bError And mbINV_CREATE_PLATE And (bNoLocations Or bNoFormats) Then
                sErr = "The inventory system is not configured properly for plates." & Chr(13)
                If bNoLocations Then sErr = sErr & "-There are no plate locations configured for the logged in user." & Chr(13)
                If bNoFormats Then sErr = sErr & "-There are no plate formats defined." & Chr(13)
                bError = True
            End If
        End If
        If bError Then
            WizStopNext ActiveWizard1, OldPane, sErr
            bWizStopped = True
        End If
        msFile = txtDB.Text
        If msFile = "" Then
            WizStopNext ActiveWizard1, OldPane, "Please specify a database to open."
            bWizStopped = True
        Else
            If moCFWImporter.Status = eNoMolServer Then
                WizStopNext ActiveWizard1, OldPane, "Could not open the specified database." & vbCrLf & "The server requires ChemOffice version 7.0 or above for this function to work." & vbCrLf & "Please contact your administrator."
                bWizStopped = True
            Else
                moCFWImporter.fileName = msFile
                ' open db
                If Not moCFWImporter.OpenDB Then
                    WizStopNext ActiveWizard1, OldPane, "Could not open the specified database."
                    bWizStopped = True
                End If
                
            End If
        End If
        'CSBR 135381 SJ For hiding the Register option step when the user selects the third upload option.
        If miLoadOption = eStructures Then
            mbRegAvailable = False
            LoadFields miLoadOption, eClassInvCompound
            FillDictGrid
            Label28.Visible = False
            cmbMolTable.Visible = False
            ActiveWizard1.ViewPane ePaneMapping
        End If
    
    ElseIf OldPane = ePaneRegistration And NewPane > OldPane Then
        mbListAttributes = CheckBoxBoolean(chkListAttributes)
        msListSeparator = "|"
        If mbListAttributes Then
            If cmbListSeparator.ListIndex < 0 Then
                WizStopNext ActiveWizard1, OldPane, "List separator must be specified"
                bWizStopped = True
            End If
            msListSeparator = Chr(cmbListSeparator.ItemData(cmbListSeparator.ListIndex))
        End If
        If miLoadOption <> eContainers Then
            ActiveWizard1.ViewPane ePaneMapping
            If NewPane <> ePaneLog Then bWizStopped = True
        End If
    
    ElseIf OldPane = ePaneUpdate And NewPane > OldPane Then
        If optUpdate(0) Then
            miUpdateMode = eUpdateNone
            miUpdateCompoundMode = eUpdateNone
        Else
            If optUpdate(1) Then
                miUpdateMode = eUpdateNonNull
            ElseIf optUpdate(2) Then
                miUpdateMode = eUpdateToNull
            End If
            If optCompoundUpdate(1) Then
                miUpdateCompoundMode = eUpdateNonNull
            ElseIf optCompoundUpdate(2) Then
                miUpdateCompoundMode = eUpdateToNull
            Else
                miUpdateCompoundMode = eUpdateNone
            End If
        End If
        
    ElseIf OldPane = ePaneMapping And NewPane > OldPane Then
    
        ' get values from table
        GetValuesFromTable
               
        ' check values to be sure we can go on
        sErr = CheckValues
        If sErr <> "" Then
            WizStopNext ActiveWizard1, OldPane, sErr
            bWizStopped = True
        Else
            If miLoadOption = ePlates Then
                lblNumCompounds = moCFWImporter.NumRecords
                UpdateNumPlates
                ActiveWizard1.ViewPane (ePaneFormat)
                If NewPane <> ePaneFormat Then bWizStopped = True
            ElseIf miLoadOption = eStructures Then
                mlNumCompounds = moCFWImporter.NumRecords
                ActiveWizard1.ViewPane ePaneLog
                If NewPane <> ePaneLog Then bWizStopped = True
            Else
                mlNumCompounds = moCFWImporter.NumRecords
                ActiveWizard1.ViewPane (ePaneLog)   'kfd2 skip container attributes for now
                If NewPane <> ePaneLog Then bWizStopped = True
            End If
        End If
   
    ElseIf OldPane = ePaneFormat And NewPane = ePaneFinish Then
        ActiveWizard1.FinishButtonEnabled = True
        frmOverallProg.Visible = False
        frmPlateProg.Visible = False
        lblNotify.Visible = True
    
    ElseIf OldPane = ePaneFormat And NewPane > OldPane And NewPane <> ePaneLog Then
        Dim vkey As Variant
        Dim bUseMappedField As Boolean
        mlPlateFormat = cmbPlateFormat.ItemData(cmbPlateFormat.ListIndex)
        mlPlateType = cmbPlateType.ItemData(cmbPlateType.ListIndex)
        'ComboFill cmbBarcodeField, moCFWImporter.FieldsDict
        'if a barcode was mapped don't allow it to be changed here
        
        'moDictFields(sPlateFld).SDFileField
        'cmbBarcodeField.se
        If mlPerPlate = 0 Then
            WizStopNext ActiveWizard1, OldPane, "You cannot import compounds into a plate format with no compound wells."
            bWizStopped = True
        Else
            frmOverallProg.Visible = False
            frmPlateProg.Visible = False
            lblNotify.Visible = True
        End If
    
    ElseIf OldPane = ePaneLibrary And NewPane > OldPane Then
        sErrorMsg = ""
        'Validate fields
        If optBarcode(1) Then
            If Len(Trim(txtBarcodeStart.Text)) = 0 Then
                bError = True
                sErrorMsg = sErrorMsg & "You must enter a Barcode starting value." & vbCrLf
            ElseIf Not IsNumeric(txtBarcodeStart.Text) Then
                bError = True
                sErrorMsg = sErrorMsg & "Barcode starting value must be a number." & vbCrLf
            End If
        End If
        If optGrp(0) Then
            If Len(Trim(txtGroupStart.Text)) > 0 Then
                If Not IsNumeric(txtGroupStart.Text) Then
                    bError = True
                    sErrorMsg = sErrorMsg & "Group starting value must be a number." & vbCrLf
                End If
            End If
        End If
        If bError Then
            WizStopNext ActiveWizard1, OldPane, sErrorMsg
            bWizStopped = True
        End If
        
        If Not bError Then
            If cmbLibrary.ListCount > 0 Then
                mlLibrary = cmbLibrary.ItemData(cmbLibrary.ListIndex)
            End If
            'WizStopNext ActiveWizard1, OldPane, optBarcode(0).Value
            If mbBarcodesFromFile Then
                FillBarcodeDictFromFile
            Else
                If optBarcode(0).value Then
                    
                ElseIf optBarcode(1).value Then
                    FillBarcodeDict CInt(txtBarcodeStart.Text)
                Else
                    FillBarcodeDict 0
                End If
            End If
            If optGrp(0).value Then
              'SYAN changed on 8/3/2004 to fix CSBR-46924
                'FillGroupDict CInt(txtGroupStart.Text)
                FillGroupDict txtGroupStart.Text
            Else
                FillGroupDict 0
            End If
            If optBarcode(2).value Or optGrp(1).value Then
                FillBarcodeGrid
                ActiveWizard1.ViewPane ePaneBarcodes
                If NewPane <> ePaneBarcodes Then bWizStopped = True
                mbCustomBarcodes = True
            Else
                ActiveWizard1.ViewPane ePaneLog
                If NewPane <> ePaneLog Then bWizStopped = True
                mbCustomBarcodes = False
            End If
        
        End If
    ElseIf OldPane = ePaneBarcodes And NewPane > OldPane Then
        GetBarcodes
        GetGroups
        If Not optBarcode(0).value Then   'not a barcode description
            sErr = CheckBarcodes
        Else
            sErr = ""
        End If
        If sErr <> "" Then
            WizStopNext ActiveWizard1, OldPane, sErr
            bWizStopped = True
        Else
            sErr = CheckGroups
            If sErr <> "" Then
                WizStopNext ActiveWizard1, OldPane, sErr
                bWizStopped = True
            Else
                ActiveWizard1.ViewPane ePaneLog
                If NewPane <> ePaneLog Then bWizStopped = True
            End If
        End If

    
    ElseIf OldPane = ePaneLog And NewPane > OldPane Then
        Dim oFS As FileSystemObject: Set oFS = New FileSystemObject
        If chkLog.value = 1 Then
            sErrorMsg = ""
            xmlLogPath = txtLogLoc.Text
            
            If Right(xmlLogPath, 1) <> "\" Then
                xmlLogPath = xmlLogPath & "\"
            End If
            
            If Not oFS.FolderExists(xmlLogPath) Then
                'oFS.CreateFolder (xmlLogPath)
                bError = True
                sErrorMsg = sErrorMsg & "This file path does not exist." & vbCrLf
            End If
            If bError Then
                WizStopNext ActiveWizard1, OldPane, sErrorMsg
                bWizStopped = True
            End If
        Else
            ' Need this for the debug XML files
            xmlLogPath = App.Path & "\Logs\"
            If (Not oFS.FolderExists(xmlLogPath)) Then
                oFS.CreateFolder (xmlLogPath)
            End If
        End If
        Set oFS = Nothing
        If miLoadOption = eStructures Or miLoadOption = eContainers Then
            ActiveWizard1.FinishButtonEnabled = True
            frmOverallProg.Visible = False
            frmPlateProg.Visible = False
            lblNotify.Visible = True
        End If
    
    ' Going backwards
    ElseIf OldPane = ePaneLog And NewPane < OldPane Then
        If miLoadOption = ePlates Then
            If optBarcode(2).value Or optGrp(1).value Then
                ActiveWizard1.ViewPane ePaneBarcodes
            Else
                ActiveWizard1.ViewPane ePaneBarcodes
            End If
        Else
            ActiveWizard1.ViewPane ePaneMapping         ' or ContainerAttributes if resurected, kfd2
        End If
        
    ElseIf OldPane = ePaneMapping And NewPane < OldPane Then
        If miLoadOption <> eContainers Then
            If Not mbRegAvailable Then
                ActiveWizard1.ViewPane ePaneFile
            Else
                ActiveWizard1.ViewPane ePaneRegistration
            End If
        End If
        
    ElseIf OldPane = ePaneUpdate And NewPane < OldPane Then
        If Not mbRegAvailable Then
            ActiveWizard1.ViewPane ePaneFile
        End If
        
    End If
    
    ' **********************************************************************
    ' Pane initialization code (in process of moving down from above, kfd2)
    If bWizStopped Then
        ' Do nothing if page transition aborted by WizStopNext
        
    ElseIf NewPane = ePaneRegistration And OldPane < ePaneRegistration Then
        If Not mbRegAvailable Then
            chkListAttributes = False
            If miLoadOption = eContainers Then
                ActiveWizard1.ViewPane ePaneUpdate
            Else
                If miLoadOption = ePlates Then 'CSBR 158779 SJ If the reg server name is absent in the invconfig.ini file.
                    Label28.Visible = False
                End If
                LoadFields miLoadOption, eClassInvCompound
                cmbMolTable.Visible = False
                FillDictGrid
                ActiveWizard1.ViewPane ePaneMapping
                optUpdate(0).value = True
            End If
        Else
            LoadSeparatorCombo
            fraListAttributes.Visible = CheckBoxBoolean(chkRegister.value)
        End If
        
    ElseIf NewPane = ePaneUpdate And OldPane < ePaneMapping Then
        If optUpdate(0) Or Not mbRegister Then
            frmCompoundUpdateOptions.Visible = False
        Else
            frmCompoundUpdateOptions.Visible = True
        End If
    
    ElseIf NewPane = ePaneMapping And OldPane < ePaneMapping Then
        If mbRegister Then
            LoadFields miLoadOption, eClassRegCompound
        Else
            LoadFields miLoadOption, eClassInvCompound
        End If
        If moCFWImporter.tableCount > 1 Then 'CSBR 164698 SJ
            Label28.Visible = True
            cmbMolTable.Visible = True
            cmbMolTable.Clear
            cmdLoadChemMappings.Visible = False
            cmdSaveChemMappings.Visible = False
                                
            Dim arrMolTable As Variant
            grdFieldDict.Visible = False
            Label9.Visible = False
            arrMolTable = Split(moCFWImporter.MolTableList, ",")
            For i = 0 To UBound(arrMolTable)
                cmbMolTable.AddItem arrMolTable(i)
            Next
            ActiveWizard1.NextButtonEnabled = False
        Else
            Label28.Visible = False
            cmbMolTable.Visible = False
            FillDictGrid
            ActiveWizard1.NextButtonEnabled = True
        End If
        
    ElseIf NewPane = ePaneLibrary And OldPane < ePaneLibrary Then
'        If (moDictFields("P_BARCODE").SDFileField >= 0 And moDictFields("P_BARCODE").SDFileField <> "") _
'          Or (moDictFields("P_BARCODE").UseDefault And moDictFields("P_BARCODE").value <> "") Then
        If HasProperty("P_BARCODE") Then
            cmbBarcodeField.Clear
            For Each vkey In moCFWImporter.FieldsDict
                If vkey <> 1000 Then
                    If moCFWImporter.FieldsDict(vkey) <> "" And Not IsEmpty(moCFWImporter.FieldsDict(vkey)) Then
                        cmbBarcodeField.AddItem moCFWImporter.FieldsDict(vkey)
                        cmbBarcodeField.ItemData(cmbBarcodeField.ListCount - 1) = vkey
                    End If
                End If
            Next
            bUseMappedField = ComboSelect(cmbBarcodeField, moCFWImporter.FieldsDict(moDictFields("P_BARCODE").SDFileField))
            optBarcode(3).value = True
            cmbBarcodeField.Enabled = False
            cmbBarcodeField.Visible = True
            cmbBarcodeDesc.Enabled = False
            cmbPrefix(0).Enabled = False
            txtBarcodeStart.Enabled = False
            optBarcode(0).Enabled = False
            optBarcode(1).Enabled = False
            optBarcode(2).Enabled = False
            optBarcode(3).Enabled = False
        Else
            optBarcode(3).Visible = False
            cmbBarcodeField.Visible = False
            optBarcode(0).value = True
            cmbBarcodeDesc.Enabled = True
            cmbPrefix(0).Enabled = True
            txtBarcodeStart.Enabled = True
            optBarcode(0).Enabled = True
            optBarcode(1).Enabled = True
            optBarcode(2).Enabled = True
            optBarcode(3).Enabled = True
        End If
    
    ' The following branch seems wrong, since the controls are on the previous pane!
    ElseIf NewPane = ePaneBarcodes And OldPane < ePaneBarcodes Then
        txtBarcodeStart.Visible = Not mbBarcodesFromFile
        If Not mbBarcodesFromFile Then
            optBarcode(1).Caption = "Assign ascending numbers, starting with:"
        Else
            optBarcode(1).Caption = "Use this barcode prefix:"
        End If
        optBarcode(2).Enabled = Not mbBarcodesFromFile
    
    End If
    
    ' enable finish button or not
    ActiveWizard1.FinishButtonEnabled = IIf(((NewPane = ePaneFinish) Or bFinish), True, False)
    'CSBR 129017
    'Comments    : Code fix to correct the issue mentioned in Note#11 of the CSBR.
    'Modified by : sjacob
    If ActiveWizard1.FinishButtonEnabled = True Then
        ActiveWizard1.FinishMode = ShowBackCancel
    End If
    Screen.MousePointer = vbNormal
End Sub

Private Sub LookForRegistration()
    Dim sResponse As String
    On Error GoTo Error
    mbRegAvailable = False
    sResponse = HTTPRequest("POST", msServerName, mbUseSSL, "cheminv/api/GetConfigInfo.asp", "", "ConfigKey=Reg_Server_Name")
    If LCase(sResponse) <> "null" Then
       mbRegAvailable = True
    End If
    Exit Sub
Error:
    MsgBox "Error checking Reg Server " & Err.Description, vbOKOnly, "Invloader"
End Sub

'kfd2
Private Sub LoadFieldXML(sServer As String, bIsSSL As Boolean)
    Dim sURL As String
    sURL = Protocol(bIsSSL) & msServerName & "/" & "cheminv/config/invloader.xml"
    Set moFieldXML = New MSXML2.DOMDocument
    moFieldXML.async = False
    moFieldXML.load sURL
End Sub

'kfd2
Private Sub LoadFields(load As LoadOptions, SubstClass As FieldSubstanceClass)
    Dim FieldNodes As MSXML2.IXMLDOMNodeList
    Dim FieldNode As MSXML2.IXMLDOMNode
    Dim Node As MSXML2.IXMLDOMNode
    Dim sLoadTarget As String
    Dim sTarget As String
    Dim eClass As FieldSubstanceClass
    Dim sDesignator As String
    Dim tfield As DBField
    Dim sValue As String
   
    Select Case load
        Case ePlates
            sLoadTarget = "PLATE"
        Case eContainers
            sLoadTarget = "CONTAINER"
        Case Else
            sLoadTarget = "COMPOUND"
    End Select
    
    Set moDictFields = New Dictionary
    
    glContainerLimit = DEFAULT_CONTAINER_LIMIT
    Set Node = moFieldXML.selectSingleNode("/invloader/defaults")
    If Not Node Is Nothing Then
        sValue = GetAttrValue(Node, "containerlimit")
        If IsNumeric(sValue) Then
            glContainerLimit = CLng(sValue)
        End If
    End If
        
    Set FieldNodes = moFieldXML.selectNodes("//fields/field")
    For Each FieldNode In FieldNodes
        sTarget = GetAttrValue(FieldNode, "target")
        If sTarget = sLoadTarget Then
            eClass = GetSubstClassENum(GetAttrValue(FieldNode, "class"))
            If eClass = eClassNeither Or eClass = SubstClass Then
                sDesignator = GetAttrValue(FieldNode, "designator")
                If Not (((sDesignator = "P_BATCHPROJECT" Or sDesignator = "C_BATCHPROJECT") And (RLSStatus = "Off" Or RLSStatus = "RegistryLevelProjects")) _
                        Or ((sDesignator = "P_PROJECT" Or sDesignator = "C_PROJECT") And (RLSStatus = "BatchLevelProjects"))) Then 'CSBR 167737 SJ Checks RLS setting and display\hide Project and BatchProject
                    Set tfield = InitDBField(sDesignator, _
                                       GetAttrValue(FieldNode, "field"), _
                                       eClass, _
                                       GetAttrValue(FieldNode, "display"), _
                                       GetTypeENum(GetAttrValue(FieldNode, "type")), _
                                       StrAsLong(GetAttrValue(FieldNode, "width")), _
                                       UCase(GetAttrValue(FieldNode, "required")) = "Y", _
                                       UCase(GetAttrValue(FieldNode, "canbelist")) = "Y", _
                                       GetAttrValue(FieldNode, "picklist"), _
                                       GetAttrValue(FieldNode, "default"), _
                                       GetAttrValue(FieldNode, "xmlparent"), _
                                       GetAttrValue(FieldNode, "xmlposition"))
                    moDictFields.Add GetAttrValue(FieldNode, "designator"), tfield
                End If
            End If
        End If
    Next FieldNode
        
    ' Special values for DUPACTION
'    Set rField = moRegOptionFields("R_DUPACTION")
'    rField.DefaultText = "Add New Batch to Registered Compound"
'    rField.DefaultValue = "NEW_BATCH"
'    rField.Picklist = GridBuildComboList2("USER_INPUT;NEW_BATCH;OVERRIDE;UNIQUE_DEL_TEMP", _
                                              "Add Duplicate to Temp Table;Add New Batch to Registered Compound;Add New Compound;Ignore Compound")
End Sub

Private Function GetAttrValue(Element As MSXML2.IXMLDOMElement, name As String) As String
    Dim Attr As MSXML2.IXMLDOMAttribute
    Set Attr = Element.getAttributeNode(name)
    If Not Attr Is Nothing Then
        GetAttrValue = Attr.value
    Else
        GetAttrValue = ""
    End If
End Function

Private Function GetSubstClassENum(sClass As String) As FieldSubstanceClass
    Select Case sClass
        Case "REG"
           GetSubstClassENum = FieldSubstanceClass.eClassRegCompound
        Case "INV"
           GetSubstClassENum = FieldSubstanceClass.eClassInvCompound
        Case ""
           GetSubstClassENum = FieldSubstanceClass.eClassNeither
        Case Else
            Err.Raise vbError + 1, , "GetSubstClassENum encountered unknown FieldSubstanceClass code: " + sClass
    End Select
End Function


Private Function GetTypeENum(sType As String) As FieldTypeEnum
    Select Case sType
        Case "eText"
           GetTypeENum = FieldTypeEnum.eText
        Case "eInteger"
           GetTypeENum = FieldTypeEnum.eInteger
        Case "eReal"
           GetTypeENum = FieldTypeEnum.eReal
        Case "eDate"
           GetTypeENum = FieldTypeEnum.eDate
        Case Else
            Err.Raise vbError + 1, , "GetTypeENum encountered unknown field type code"
    End Select
End Function

'kfd2
Private Function GetFieldPicklistDict(oField As DBField) As Dictionary
    Dim oRS As ADODB.Recordset
    On Error GoTo Error
    If Len(oField.PicklistName) > 0 Then
        If Not oField.PickListLoaded Then
            If Left(oField.PicklistName, 1) = "*" Then
                Set oField.Picklist = DictFromString(Mid(oField.PicklistName, 2))
            ElseIf oField.eFieldSubstClass = eClassRegCompound Then
                Set oField.Picklist = GetRegPicklist(oField.PicklistName)
            Else
                Set oRS = GetPicklistRS(oField.PicklistName)
                Set oField.Picklist = DictionaryFromRecordset(oRS, 0, 1)
            End If
            oField.PickListLoaded = True
        End If
    End If
    Set GetFieldPicklistDict = oField.Picklist
    Exit Function
Error:
    MsgBox "Failed to load picklist " & oField.PicklistName & ": " & Err.Description
    Set GetFieldPicklistDict = Nothing
End Function

Private Function DictFromString(s As String) As Dictionary
    Dim kv As Variant
    Dim key As String
    Dim isKey As Boolean
    
    Set DictFromString = New Dictionary
    isKey = True
    For Each kv In Split(s, "|")
        If isKey Then
            key = kv
            isKey = False
        Else
            DictFromString.Add key, kv
            isKey = True
        End If
    Next kv
End Function

Private Function CheckBarcodes() As String
    ' ensure barcodes are valid
    Dim vkey As Variant
    Dim sVal, sBarcodesToCheck, sSQL, sProblemBarcodes, sErrMsg As String
    Dim rsProblemBarcodes As ADODB.Recordset
    
    CheckBarcodes = ""
    sErrMsg = ""
    sProblemBarcodes = ""
    For Each vkey In moDictBarcodes
        sVal = moDictBarcodes(vkey)
        If sVal = "" Then
            sErrMsg = sErrMsg & "You must enter a value for all barcodes." & vbLf
            Exit For
        'ElseIf BarcodeGetId(moConn, ComboItemData(cmbPrefix(0)), sVal) <> NULL_AS_LONG Then
        Else
            'CheckBarcodes = "The barcode " & ComboItemString(cmbPrefix(0)) & "-" & sVal & " already exists.  Please choose another."
            'Exit For
            sBarcodesToCheck = sBarcodesToCheck & "'" & sVal & "',"
        End If
    Next
    sBarcodesToCheck = Left(sBarcodesToCheck, Len(sBarcodesToCheck) - 1)
    Set rsProblemBarcodes = ExecuteStatement(eStmtExistingPlates, sBarcodesToCheck)
    
    'SYAN modified on 7/26/2004 to fix CSBR-46513
    'While Not rsProblemBarcodes.EOF Or rsProblemBarcodes.BOF
    While Not rsProblemBarcodes.EOF And rsProblemBarcodes.BOF
    'End of SYAN modification
        sProblemBarcodes = sProblemBarcodes & rsProblemBarcodes("plate_barcode") & ","
        rsProblemBarcodes.MoveNext
    Wend
    If sProblemBarcodes <> "" Then sErrMsg = sErrMsg & "Duplicate barcodes: " & Left(sProblemBarcodes, Len(sProblemBarcodes) - 1) & "."
    CheckBarcodes = sErrMsg
End Function

Private Function CheckGroups() As String
    ' ensure groups are valid
    ' ensure barcodes are valid
    ' assume inv will take care of this
    Dim vkey As Variant
    Dim sVal As String
    CheckGroups = ""
'    For Each vkey In moDictGroups
'        sVal = moDictGroups(vkey)
'        If sVal = "" Then
'            CheckGroups = "You must enter a value for all groups."
'            Exit For
'        End If
'    Next
End Function

Private Sub GetBarcodes()
    Dim Prefix As String
    Dim key As Variant
    'b/c the cmbPrefixes are kept in synch doesn't matter which one I chooose
    'Prefix = cmbPrefix(0).ItemData(cmbPrefix(0).ListIndex)
    Prefix = cmbPrefix(0).Text
    If optBarcode(1) Then
        For Each key In moDictBarcodes
            moDictBarcodes(key) = Prefix & moDictBarcodes(key)
        Next
    ElseIf optBarcode(2) Then
        With grdBarcodes
            Dim i As Long
            For i = 1 To .Rows - 1
                moDictBarcodes(CStr(i)) = Prefix & CStr(.TextMatrix(i, 1))
            Next
        End With
    End If
End Sub

Private Sub GetGroups()
    With grdBarcodes
        Dim i As Long
        For i = 1 To .Rows - 1
            moDictGroups(CStr(i)) = .TextMatrix(i, 2)
        Next
    End With
End Sub

Private Sub FillBarcodeDictFromFile()
    ' IRL added 8/18/04
    ' get barcode numbers from file
    moDictBarcodes.RemoveAll
    Dim sSQL As String
    sSQL = "Select count(innerCount) as theCount from (Select count([" & moCFWImporter.FieldsDict(moDictFields("P_BARCODE").SDFileField) & "]) as innerCount from " & moCFWImporter.RealTableName() & " group by [" & moCFWImporter.FieldsDict(moDictFields("P_BARCODE").SDFileField) & "])"
    Dim rs As Recordset
    Set rs = moCFWImporter.Recordset2(sSQL)
    Dim i As Long: i = 1
    If RSHasRecords(rs) Then
        Do Until rs.EOF
            moDictBarcodes.Add CStr(i), rs(0).value
            rs.MoveNext
        Loop
    End If
    ' IRL end 8/18/04
End Sub

Private Sub FillBarcodeDict(ByVal Start As Integer)
    moDictBarcodes.RemoveAll
    Dim i As Long, Val As String
    For i = 1 To mlNumPlates
        If Start > 0 Then
            Val = Start + i - 1
        Else
            Val = i - 1
        End If
        moDictBarcodes.Add CStr(i), Val
    Next
End Sub

'SYAN modified on 8/3/2004 to fix CSBR-46924
Private Sub FillGroupDict(ByVal Start As Variant)
    If Start = "" Then
        Start = -1
    End If
    
    moDictGroups.RemoveAll
    Dim i As Long, Val As String
    For i = 1 To mlNumPlates
        If Start > 0 Then
            Val = Start + i - 1
        ElseIf Start = 0 Then
            Val = i - 1
        ElseIf Start = -1 Then
            Val = ""
        End If
        'moDictGroups.Add CStr(I), "G-" & Val
        moDictGroups.Add CStr(i), Val
    Next
End Sub

Private Sub UpdateNumPlates()
    Dim lLeftOver As Long
    Dim tempCmd As ADODB.Command
    Dim rsCount As ADODB.Recordset
    Dim sSQL As String
    ' Check the number of records for barcode,supplierplate number, and supplier barcode if they are used, it will tell you the number of plates
    Dim sPlateFld As String
    
    If IsNumeric(lblNumCompounds.Caption) Then
        If moDictFields("P_BARCODE").SDFileField = "" Then
            If moDictFields("P_SUPPLIERPLATENUMBER").SDFileField = "" Then
                If moDictFields("P_SUPPLIERBARCODE").SDFileField = "" Then
                    sPlateFld = ""
                Else
                    sPlateFld = "P_SUPPLIERBARCODE"
                End If
            Else
                sPlateFld = "P_SUPPLIERPLATENUMBER"
            End If
        Else
            sPlateFld = "P_BARCODE"
        End If
    
        'determine the number of plates if a barcode, supplier plate num, or supplier barcode is specified
        If sPlateFld <> "" Then
            ' IRL added [] symbols to handle field names with spaces 8/18/04
            sSQL = "Select count(innerCount) as theCount from (Select count([" & moCFWImporter.FieldsDict(moDictFields(sPlateFld).SDFileField) & "]) as innerCount from " & moCFWImporter.RealTableName() & " group by [" & moCFWImporter.FieldsDict(moDictFields(sPlateFld).SDFileField) & "])"
            ' IRL end 8/18/04
            'sSQL = "Select count(mol_id) as theCount from " & moCFWImporter.MolTable & " where mol_id in (select distinct mol_id from moltable)"
            Set rsCount = moCFWImporter.Recordset2(sSQL)
            mlNumPlates = rsCount("theCount")
        Else
            mlNumPlates = 0
        End If
    
        mlNumCompounds = CLng(lblNumCompounds.Caption)
        mlPerPlate = ExecuteStatement(eStmtNumberOfCompoundWells, cmbPlateFormat.ItemData(cmbPlateFormat.ListIndex))
        
        Dim lTest
        If mlPerPlate = 0 Then
            lblNumPlates.Caption = "The selected format has 0 compound wells."
        Else
            If mlNumPlates = 0 Then
                mlNumPlates = Ceiling(mlNumCompounds / mlPerPlate)
                lLeftOver = mlPerPlate - (mlNumCompounds Mod mlPerPlate)
                If lLeftOver = mlPerPlate Then lLeftOver = 0
                lblNumPlates.Caption = mlNumPlates & " plates, " & lLeftOver & " wells empty on last plate"
            ElseIf mlNumPlates > 0 Then
                lblNumPlates.Caption = mlNumPlates
            End If
        End If
    End If
End Sub

Private Function CheckPlateValues() As String
    
    Dim sErr As String: sErr = ""
    Dim vAns
    ' check the well indicator
    Dim tfield As DBField
    Dim wellfield, rowfield, colfield
    Dim barcodefield, suppbarcodefield, suppplatenumber
    Dim sPlateFld As String
    Dim rs As ADODB.Recordset
    Dim sBarcode As String
    Dim sBarcodeList As String
    Dim TempDict As Dictionary
    Dim sPrimaryKeys As String
    Dim sUnit As String
    Dim sReturn As String
    Dim vkey
    Dim arrBarcodes As Variant
    Dim arrBarcodePlateID As Variant
    Dim i As Long
    Dim sReturnVal As String
    Dim oField As DBField
    Dim sBatchProject As String
    Dim arrFragment As Variant
    Dim arrEquivalence As Variant
    Dim sFragments As String
    Dim sEquivalents As String
    Dim key As Variant
    Dim bInsertOnly As Boolean
    Dim sPrimeKey As String
    bInsertOnly = True
    sPrimeKey = ""
    Set TempDict = New Dictionary
    
    If mbRegAvailable And (Not mbRegister) Then  'kfd2 changed from Not mbShowRegister (need to consider mbRegister with Reg/Batch ID)
        'Debug.Print "moDictFields(eBatchNumber).value=" & moDictFields(eBatchNumber).value
        If HasProperty("P_REG") Then
            If Not HasProperty("P_BATCHNUMBER") Then
                sErr = "You must enter a batch number." & vbCrLf
                GoTo DoneChecking
            ElseIf moDictFields("P_BATCHNUMBER").value <> "" And Not IsNumeric(moDictFields("P_BATCHNUMBER").value) Then
                sErr = "Batch number must be a number." & vbCrLf
                GoTo DoneChecking
            Else
                sReturnVal = CheckRegValues(moDictFields, "P_REG", "P_BATCHNUMBER")
                If sReturnVal <> "" Then
                    sErr = sReturnVal
                    GoTo DoneChecking
                End If
            End If
        End If
    ElseIf (Not mbRegAvailable And Not mbRegister) Then 'CBOE-1365 SJ Checking if Inv-Reg integration is enabled.
        If HasProperty("P_REG") Then
            If Not HasProperty("P_BATCHNUMBER") Then
                sErr = "You must enter a batch number." & vbCrLf
                GoTo DoneChecking
            ElseIf moDictFields("P_BATCHNUMBER").value <> "" And Not IsNumeric(moDictFields("P_BATCHNUMBER").value) Then
                sErr = "Batch number must be a number." & vbCrLf
                GoTo DoneChecking
            Else
                sReturnVal = CheckRegValues(moDictFields, "P_REG", "P_BATCHNUMBER")
                If sReturnVal <> "" Then
                    sErr = sReturnVal
                    GoTo DoneChecking
                End If
            End If
        End If
    End If
    'CBOE-56 SJ Added code for displaying the message for selecting prefix if not selected.
    For Each key In moDictFields
        Set oField = moDictFields(key)
        sErr = CheckFieldDictValues(oField.Designator, bInsertOnly, sPrimeKey)
        If sErr <> "" Then
            GoTo DoneChecking
        End If
    Next key
    
    'CSBR 135264 SJ Checking if RLS is set to Batch Level.
    If RLSStatus = "BatchLevelProjects" Then
        Set oField = moDictFields("P_BATCHPROJECT")
        sBatchProject = moDictFields("P_BATCHPROJECT").SDFileField
        If oField.value = "" And Len(sBatchProject) = 0 Then
            sErr = "RLS is set to Batch Level." & vbLf
            sErr = sErr & "Please select a Batch Level Project."
            GoTo DoneChecking
        End If
    End If
    
    'CSBR 167423 SJ checking the no of fragments and equivalents.
    If mbRegister Then
        If moDictFields("P_FRAGMENT").SDFileField <> "" Then
            Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
            While Not rs.EOF
                sFragments = CnvString(rs(CLng(moDictFields("P_FRAGMENT").SDFileField)), eDBtoVB)
                If moDictFields("P_EQUIVALENTS").SDFileField <> "" Then
                    sEquivalents = CnvString(rs(CLng(moDictFields("P_EQUIVALENTS").SDFileField)), eDBtoVB)
                Else
                    Set oField = moDictFields("P_EQUIVALENTS")
                    sEquivalents = oField.value
                End If
                arrFragment = Split(Trim(sFragments), msListSeparator)
                arrEquivalence = Split(Trim(sEquivalents), msListSeparator)
                If UBound(arrFragment) <> UBound(arrEquivalence) Then
                    If UBound(arrFragment) < UBound(arrEquivalence) Then
                        sErr = "Fragment is missing" & vbLf & "Please enter a fragment"
                    Else
                        sErr = "Equivalents is missing" & vbLf & "Please enter a value for equivalents"
                    End If
                    GoTo DoneChecking
                End If
                rs.MoveNext
            Wend
        ElseIf moDictFields("P_EQUIVALENTS").SDFileField <> "" Then
            Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
            While Not rs.EOF
                sEquivalents = CnvString(rs(CLng(moDictFields("P_EQUIVALENTS").SDFileField)), eDBtoVB)
                If moDictFields("P_FRAGMENT").SDFileField <> "" Then
                    sFragments = CnvString(rs(CLng(moDictFields("P_FRAGMENT").SDFileField)), eDBtoVB)
                Else
                    Set oField = moDictFields("P_FRAGMENT")
                    sFragments = oField.value
                End If
                arrFragment = Split(sFragments, msListSeparator)
                arrEquivalence = Split(sEquivalents, msListSeparator)
                If UBound(arrFragment) <> UBound(arrEquivalence) Then
                    If UBound(arrFragment) < UBound(arrEquivalence) Then
                        sErr = "Fragment is missing" & vbLf & "Please enter a fragment"
                    Else
                        sErr = "Equivalents is missing" & vbLf & "Please enter a value for equivalents"
                    End If
                    GoTo DoneChecking
                End If
                rs.MoveNext
            Wend
        Else
            Set oField = moDictFields("P_FRAGMENT")
            sFragments = oField.value
            Set oField = moDictFields("P_EQUIVALENTS")
            sEquivalents = oField.value
            arrFragment = Split(sFragments, msListSeparator)
            arrEquivalence = Split(sEquivalents, msListSeparator)
            If UBound(arrFragment) <> UBound(arrEquivalence) Then
                If UBound(arrFragment) < UBound(arrEquivalence) Then
                    sErr = "Fragment is missing" & vbLf & "Please enter a fragment"
                Else
                    sErr = "Equivalents is missing" & vbLf & "Please enter a value for equivalents"
                End If
                GoTo DoneChecking
            End If
        End If
        sFragments = ""
        sEquivalents = ""
        Set oField = Nothing
        Set rs = Nothing
    End If
    
    wellfield = moDictFields("P_WELLCOORDINATES").SDFileField
    rowfield = moDictFields("P_ROW").SDFileField
    colfield = moDictFields("P_COL").SDFileField
    mbHaveWellCoords = False
    If moDictFields("P_BARCODE").SDFileField <> "" Then
        mbBarcodesFromFile = True
    Else
        mbBarcodesFromFile = False
    End If
    'If moDictFields(eqtyinit).Value = "" Then
        ' MsgBox serr, vbExclamation + vbOKOnly
    '    GoTo DoneChecking
    'End If
    If wellfield = "" And rowfield = "" And colfield = "" Then
        vAns = MsgBox("You have not selected any fields for well coordinate, row, or column.  Compounds will be placed in plates in the order that they appear in the file.  Is this OK?", vbYesNo + vbExclamation)
        If vAns = vbNo Then
            sErr = "Please select a field for well coordinate, or row and column."
            GoTo DoneChecking
        End If
    ElseIf wellfield = "" And ((rowfield = "" And colfield <> "") Or (rowfield <> "" And colfield = "")) Then
        vAns = MsgBox("You have not selected fields for both row and column.  Compounds will be placed in plates in the order that they appear in the file.  Is this OK?", vbYesNo + vbExclamation)
        If vAns = vbNo Then
            sErr = "Please select a field for well coordinate, or row and column."
            GoTo DoneChecking
        End If
    Else
        mbHaveWellCoords = True
        If wellfield <> "" Then
            msWellCoordField = "P_WELLCOORDINATES"
            msRowField = "P_ROW"
            msColField = "P_COL"
        Else
            msWellCoordField = "P_ROW"
        End If
    End If
    ' check plate indicator
    mbHavePlateCoords = False
    barcodefield = moDictFields("P_BARCODE").SDFileField
    suppbarcodefield = moDictFields("P_SUPPLIERBARCODE").SDFileField
    suppplatenumber = moDictFields("P_SUPPLIERPLATENUMBER").SDFileField
    If barcodefield = "" And suppbarcodefield = "" And suppplatenumber = "" Then
        vAns = MsgBox("You have not selected any fields for barcode, supplier barcode, or supplier plate number.  Well coordinate, row and column information will be ignored, and compounds will be placed in plates in the order that they appear in the file.  Is this OK?", vbYesNo + vbExclamation)
        If vAns = vbNo Then
            sErr = "Please select a field for barcode, supplier barcode, or supplier plate number."
            GoTo DoneChecking
        ElseIf vAns = vbYes Then
            mbHaveWellCoords = False
        End If
    Else
        mbHavePlateCoords = True
        If suppplatenumber <> "" Then
            msPlateCoordField = "P_SUPPLIERPLATENUMBER"
        ElseIf suppbarcodefield <> "" Then
            msPlateCoordField = "P_SUPPLIERBARCODE"
        Else
            msPlateCoordField = "P_BARCODE"
        End If
    End If
    
    If moDictFields("P_BARCODE").SDFileField = "" Then
        If moDictFields("P_SUPPLIERPLATENUMBER").SDFileField = "" Then
            sPlateFld = "P_SUPPLIERBARCODE"
        Else
            sPlateFld = "P_SUPPLIERPLATENUMBER"
        End If
    Else
        sPlateFld = "P_BARCODE"
    End If
    
    ' Check for duplicate barcodes in Inventory here, before the upload is attempted, so we can
    ' generate a warning and have the user fix the problem
    sBarcodeList = ""
    Set rs = moCFWImporter.DistinctRecordset(CnvLong(moDictFields(sPlateFld).SDFileField, eDBtoVB))
    While Not rs.EOF
        If Not IsNull(rs.fields(0).value) Then
            sBarcode = Trim(CStr(rs.fields(0).value))
            If Len(sBarcode) > 0 Then
                If Not TempDict.Exists(sBarcode) Then 'CSBR 159787 SJ duplicate checking
                    TempDict.Add sBarcode, sBarcode
                End If
            End If
        End If
        rs.MoveNext
    Wend
    
    For Each vkey In TempDict
        sBarcodeList = sBarcodeList & TempDict(vkey) & ","
    Next
    
    If Len(sBarcodeList) > 0 Then
        sPrimaryKeys = GetPrimaryKeyIDs("inv_plates", sBarcodeList)
        If sPrimaryKeys <> "NOT FOUND" Then
            arrBarcodes = Split(sPrimaryKeys, ",")
            If UBound(arrBarcodes) <= 5 Then
                sErr = "The following plate barcodes already exist within Inventory:" & vbLf & vbLf
                For i = 0 To UBound(arrBarcodes)
                    arrBarcodePlateID = Split(arrBarcodes(i), "=")
                    sErr = sErr & vbTab & arrBarcodePlateID(0) & vbLf
                Next
                sErr = sErr & vbLf & "These barcodes must be fixed before the upload can proceed."
            Else
                sErr = "Multiple plate barcodes from this database already exist within Inventory." & vbLf & vbLf
                sErr = sErr & "Please check to make sure the new barcodes are unique."
            End If
            GoTo DoneChecking
        End If
    End If

DoneChecking:
    CheckPlateValues = sErr
    
    Set TempDict = Nothing
End Function

Private Function LoadSeparatorCombo()
    Dim i As Integer
    With cmbListSeparator
        i = .ListIndex
        .Clear
        .AddItem ("Pipe (|) character")
        .ItemData(.ListCount - 1) = Asc("|")
        .AddItem ("Semicolon (;) character")
        .ItemData(.ListCount - 1) = Asc(";")
        .AddItem ("Comma (,) character")
        .ItemData(.ListCount - 1) = Asc(",")
        If i >= 0 And i < .ListCount Then
            .ListIndex = i
        Else
            .ListIndex = 0
        End If
    End With
End Function

Private Function CheckValues() As String
    Select Case miLoadOption
        Case ePlates
            CheckValues = CheckPlateValues()
        Case eStructures
        Case eContainers
            CheckValues = CheckContainerValues()
    End Select
End Function

Private Function CheckContainerValues() As String
    ' check the values that the user input
    Dim sErr As String: sErr = ""
    Dim key As Variant
    Dim rs As Recordset
    Dim sBarcode As String
    Dim sBarcodeList As String
    Dim TempDict As Dictionary
    Dim TempDict1 As Dictionary
    Dim sPrimaryKeys As String
    Dim sReturn As String
    Dim sCompoundID As String
    Dim lCompoundID As Long
    Dim sLocation As String
    Dim sLocationID As String
    Dim sLocationList As String
    Dim arrLocationFields As Variant
    Dim arrBarcodes As Variant
    Dim arrBarcodeContainerID As Variant
    Dim i As Integer
    Dim iLen As Integer
    Dim lListLen As Long
    Dim oField As DBField
    Dim bRSCheck As Boolean
    Dim bExists As Boolean
    Dim sValue As String
    Dim bInsertOnly As Boolean
    Dim sPrimeKey As String
    Dim bCheckDefault
    Dim sBatchProject As String
    Dim arrFragment As Variant
    Dim arrEquivalence As Variant
    Dim sFragments As String
    Dim sEquivalents As String
    
    ' Just in case the GUI didn't get this right
    If miUpdateMode = eUpdateNone Then miUpdateCompoundMode = eUpdateNone
    
    ' CSBR 160954 SJ Container barcode made mandatory in Update mode
    If miUpdateMode = eUpdateNonNull Or miUpdateMode = eUpdateToNull Then
        Set oField = moDictFields("C_CBARCODE")
        If moDictFields("C_CBARCODE").SDFileField = "" And oField.value = "" Then 'CSBR 167299 SJ
            sErr = "Container barcodes must be mapped in update mode."
            GoTo DoneChecking
        Else
            sBarcode = oField.value
            If sBarcode = "" Then
                Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
                sBarcode = CnvString(rs(CLng(moDictFields("C_CBARCODE").SDFileField)), eDBtoVB)
            End If
            sPrimaryKeys = GetPrimaryKeyIDs("inv_containers", Trim(UCase(sBarcode)))
            If sPrimaryKeys = "NOT FOUND" Then
                sErr = "The container barcode does not exist in Inventory."
                GoTo DoneChecking
            End If
        End If
        sBarcode = ""
        sPrimaryKeys = ""
        Set oField = Nothing
    End If
       
    ' Check Batch Number supplied with Reg Number
'    If ((Not moDictFields("C_REGNUMBER").UseDefault Or Len(moDictFields("C_REGNUMBER").value) <> 0) _
'      And (Len(moDictFields("C_BATCHNUMBERFK").value) = 0 And moDictFields("C_BATCHNUMBERFK").UseDefault)) Then
    If HasProperty("C_REGNUMBER") And Not HasProperty("C_BATCHNUMBERFK") Then
        sErr = "If you map Registration ID you must also map Batch Number." & vbCrLf
        GoTo DoneChecking
    End If
    
    ' Perform basic validity check on values
    bInsertOnly = (miUpdateMode = eUpdateNone)
    If bInsertOnly Then
        sPrimeKey = ""              ' No prime key needed for update
    Else
        sPrimeKey = "C_CBARCODE"     ' Prime key require for update-mode
    End If
    For Each key In moDictFields
        Set oField = moDictFields(key)
        sErr = CheckFieldDictValues(oField.Designator, bInsertOnly, sPrimeKey)
        If sErr <> "" Then
            GoTo DoneChecking
        End If
    Next key
    'CSBR 165570 SJ Checking for RLS status.
    If RLSStatus = "BatchLevelProjects" Then
        Set oField = moDictFields("C_BATCHPROJECT")
        sBatchProject = moDictFields("C_BATCHPROJECT").SDFileField
        If oField.value = "" And Len(sBatchProject) = 0 Then
            sErr = "RLS is set to Batch Level." & vbLf
            sErr = sErr & "Please select a Batch Level Project."
            GoTo DoneChecking
        End If
    End If
    'CSBR 167423 SJ checking the no of fragments and equivalents.
    If mbRegister Then
        If moDictFields("C_FRAGMENT").SDFileField <> "" Then
            Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
            While Not rs.EOF
                sFragments = CnvString(rs(CLng(moDictFields("C_FRAGMENT").SDFileField)), eDBtoVB)
                If moDictFields("C_EQUIVALENTS").SDFileField <> "" Then
                    sEquivalents = CnvString(rs(CLng(moDictFields("C_EQUIVALENTS").SDFileField)), eDBtoVB)
                Else
                    Set oField = moDictFields("C_EQUIVALENTS")
                    sEquivalents = oField.value
                End If
                arrFragment = Split(sFragments, msListSeparator)
                arrEquivalence = Split(sEquivalents, msListSeparator)
                If UBound(arrFragment) <> UBound(arrEquivalence) Then
                    If UBound(arrFragment) < UBound(arrEquivalence) Then
                        sErr = "Fragment is missing" & vbLf & "Please enter a fragment"
                    Else
                        sErr = "Equivalents is missing" & vbLf & "Please enter a value for equivalents"
                    End If
                    GoTo DoneChecking
                End If
                rs.MoveNext
            Wend
         ElseIf moDictFields("C_EQUIVALENTS").SDFileField <> "" Then
            Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
            While Not rs.EOF
                sEquivalents = CnvString(rs(CLng(moDictFields("C_EQUIVALENTS").SDFileField)), eDBtoVB)
                If moDictFields("P_FRAGMENT").SDFileField <> "" Then
                    sFragments = CnvString(rs(CLng(moDictFields("C_FRAGMENT").SDFileField)), eDBtoVB)
                Else
                    Set oField = moDictFields("C_FRAGMENT")
                    sFragments = oField.value
                End If
                arrFragment = Split(sFragments, msListSeparator)
                arrEquivalence = Split(sEquivalents, msListSeparator)
                If UBound(arrFragment) <> UBound(arrEquivalence) Then
                    If UBound(arrFragment) < UBound(arrEquivalence) Then
                        sErr = "Fragment is missing" & vbLf & "Please enter a fragment"
                    Else
                        sErr = "Equivalents is missing" & vbLf & "Please enter a value for equivalents"
                    End If
                    GoTo DoneChecking
                End If
                rs.MoveNext
            Wend
         Else
            Set oField = moDictFields("C_FRAGMENT")
            sFragments = oField.value
            Set oField = moDictFields("C_EQUIVALENTS")
            sEquivalents = oField.value
            arrFragment = Split(sFragments, msListSeparator)
            arrEquivalence = Split(sEquivalents, msListSeparator)
            If UBound(arrFragment) <> UBound(arrEquivalence) Then
                If UBound(arrFragment) < UBound(arrEquivalence) Then
                    sErr = "Fragment is missing" & vbLf & "Please enter a fragment"
                Else
                    sErr = "Equivalents is missing" & vbLf & "Please enter a value for equivalents"
                End If
                GoTo DoneChecking
            End If
        End If
        sFragments = ""
        sEquivalents = ""
        Set oField = Nothing
        Set rs = Nothing
    End If
    
    ' Check required values for the update to null cases.  The required value needs to be
    ' checked when it is NOT an update, so you need to look up the barcodes individually.
    If miUpdateMode = eUpdateToNull Or (mbRegister And miUpdateCompoundMode = eUpdateToNull) Then
        bRSCheck = False
        For Each key In moDictFields
            Set oField = moDictFields(key)
            If oField.Required And _
              ((oField.eFieldSubstClass = eClassNeither And miUpdateMode = eUpdateToNull) _
              Or (oField.eFieldSubstClass <> eClassNeither And miUpdateCompoundMode = eUpdateToNull)) Then
                bRSCheck = True
                Exit For
            End If
        Next key
        If bRSCheck Then
            Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
            While Not rs.EOF
            If moDictFields("C_CBARCODE").SDFileField <> "" Then
                sBarcode = CnvString(rs(CLng(moDictFields("C_CBARCODE").SDFileField)), eDBtoVB)
            Else
                sBarcode = CnvString(moDictFields("C_CBARCODE").value, eDBtoVB)
            End If
                sPrimaryKeys = GetPrimaryKeyIDs("inv_containers", sBarcode)
                bExists = sPrimaryKeys <> "NOT FOUND"
                If Not bExists Then
                    For Each key In moDictFields
                        Set oField = moDictFields(key)
                        If oField.Required And _
                          ((oField.eFieldSubstClass = eClassNeither And miUpdateMode = eUpdateToNull) _
                          Or (oField.eFieldSubstClass <> eClassNeither And miUpdateCompoundMode = eUpdateToNull)) Then
                            sValue = ""
                            If oField.SDFileField <> "" Then
                                sValue = CnvString(rs(CLng(oField.SDFileField)), eDBtoVB)
                            End If
                            If sValue = "" And oField.value = "" Then
                                sErr = "Missing required value for " + oField.DisplayName + " from input data"
                                GoTo DoneChecking
                            End If
                        End If
                    Next key
                End If
                rs.MoveNext
            Wend
        End If
    End If

    ' Check for duplicate barcodes within the user's database file
    If moDictFields("C_CBARCODE").SDFileField <> "" Then
        sBarcodeList = ""
        Set rs = moCFWImporter.DuplicateChecking(CnvLong(moDictFields("C_CBARCODE").SDFileField, eDBtoVB))
        If Not rs.EOF And Not rs.BOF Then
            While Not rs.EOF
                sBarcodeList = sBarcodeList & (rs.fields(0).value) & ","
                rs.MoveNext
            Wend
            
            If Len(sBarcodeList) > 0 Then
                arrBarcodes = Split(sBarcodeList, ",")
                If UBound(arrBarcodes) <= 5 Then
                    sErr = "The following container barcodes are duplicated in the source database:" & vbLf & vbLf
                    For i = 0 To UBound(arrBarcodes)
                        sErr = sErr & vbTab & arrBarcodes(i) & vbLf
                    Next
                    sErr = sErr & vbLf & "These barcodes must be fixed before the upload can proceed."
                Else
                    sErr = "Multiple container barcodes within this database are duplicates of each other." & vbLf & vbLf
                    sErr = sErr & "Please check to make sure every new barcode is unique."
                End If
                GoTo DoneChecking
            End If
        End If
        Set rs = Nothing
    End If
            
    ' Ensure the container barcodes don't already exist in update mode
    If miUpdateMode = eUpdateNone And moDictFields("C_CBARCODE").SDFileField <> "" Then
        sBarcodeList = ""
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(moDictFields("C_CBARCODE").SDFileField, eDBtoVB))
        If Not rs.EOF And Not rs.BOF Then
            sBarcodeList = ""
            lListLen = 0
            Do
                If Not rs.EOF Then
                    sBarcode = Trim(CStr(rs.fields(0).value))
                    iLen = Len(sBarcode)
                    If iLen > 0 Then
                        If lListLen > 0 Then
                            sBarcodeList = sBarcodeList & ","
                            lListLen = lListLen + 1
                        End If
                        sBarcodeList = sBarcodeList & sBarcode
                        lListLen = lListLen + iLen
                    End If
                 End If
                 If (rs.EOF Or lListLen > LIST_BYTE_LIMIT) And lListLen > 0 Then
                    sPrimaryKeys = GetPrimaryKeyIDs("inv_containers", sBarcodeList)
                    If sPrimaryKeys <> "NOT FOUND" Then
                        arrBarcodes = Split(sPrimaryKeys, ",")
                        If UBound(arrBarcodes) <= 5 Then
                            sErr = "The following container barcodes already exist within Inventory:" & vbLf & vbLf
                            For i = 0 To UBound(arrBarcodes)
                                arrBarcodeContainerID = Split(arrBarcodes(i), "=")
                                sErr = sErr & vbTab & arrBarcodeContainerID(0) & vbLf
                            Next
                            sErr = sErr & vbLf & "These barcodes must be fixed before the upload can proceed."
                        Else
                            sErr = "Multiple container barcodes from this database already exist within Inventory." & vbLf & vbLf
                            sErr = sErr & "Please check to make sure the new barcodes are unique."
                        End If
                        GoTo DoneChecking
                    End If
                    sBarcodeList = ""
                    lListLen = 0
                 End If
                 If Not rs.EOF Then rs.MoveNext
             Loop Until rs.EOF And lListLen = 0
        
        End If  ' If Not rs.EOF And Not rs.BOF Then
    End If
        
     ' Validate the location ID
    Set TempDict = New Dictionary
    Set TempDict1 = New Dictionary
    If moDictFields("C_LOCATIONBARCODE").SDFileField <> "" Then
        bCheckDefault = False
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(moDictFields("C_LOCATIONBARCODE").SDFileField, eDBtoVB))
        While Not rs.EOF
            sBarcode = Trim(rs.fields(0).value)
            If Len(sBarcode) > 0 Then
                sLocationID = GetLocationFromBarcode(sBarcode)
                If Len(sLocationID) = 0 Then
                    TempDict.Add sBarcode, sBarcode
                Else
                    ' Add this to our mapping dictionary so we can replace barcode with location ID
                    ' during the XML creation
                    arrLocationFields = Split(sLocationID, ",")
                    sLocationID = arrLocationFields(0)
                    If UBound(arrLocationFields) > 5 Then
                        If arrLocationFields(6) = 0 Then
                            TempDict1.Add sBarcode, sBarcode
                         Else
                            ' assign the Ownership same as of Parent Location
                            If Not moGroupSecurityMapping.Exists(sBarcode) Then
                                moGroupSecurityMapping.Add sBarcode, arrLocationFields(7)
                            End If
                        End If
                    End If
                    
                    If Not moLocationBarcodeMapping.Exists(sBarcode) Then
                        moLocationBarcodeMapping.Add sBarcode, sLocationID
                    End If
                End If
            Else
                bCheckDefault = True
            End If
            rs.MoveNext
        Wend
        rs.Close
        Set rs = Nothing
         
        If TempDict.count > 0 And TempDict.count <= 5 Then
            sErr = "The following location barcodes do not exist within Inventory:" & vbLf & vbLf
            For Each key In TempDict
                sErr = sErr & vbTab & key & vbLf
            Next
            sErr = sErr & vbLf & "These barcodes must be fixed before the upload can proceed."
            GoTo DoneChecking
        ElseIf TempDict.count > 5 Then
            sErr = "Multiple location barcodes from this database do not exist within Inventory." & vbLf & vbLf
            sErr = sErr & "These barcodes must be fixed before the upload can proceed."
            GoTo DoneChecking
       'Checking Group Authority
        ElseIf TempDict1.count > 0 And TempDict1.count <= 5 Then
            sErr = "You are not Location Admin of the following locations:" & vbLf & vbLf
            For Each key In TempDict1
                sErr = sErr & vbTab & key & vbLf
            Next
            sErr = sErr & vbLf & "Please enter valid locations before the upload can proceed."
            GoTo DoneChecking
        ElseIf TempDict.count > 5 Then
            sErr = "You are not part of Admin group for multiple locations." & vbLf & vbLf
            sErr = sErr & "Please enter valid locations before the upload can proceed."
            GoTo DoneChecking
        End If
    Else
        bCheckDefault = True
    End If
 TempDict1.RemoveAll
    If bCheckDefault Then
        sBarcode = Trim(moDictFields("C_LOCATIONBARCODE").value)
        If sBarcode <> "" Then
            sLocationID = GetLocationFromBarcode(sBarcode)
            If Len(sLocationID) = 0 Then
                sErr = "The location barcode '" & sBarcode & "' was not found in the Inventory database.  "
                sErr = sErr & "The location barcode must be fixed before the upload can proceed.  " & vbLf
                ' sErr = sErr & "Alternatively, you may leave the location barcode field blank and specify the location in the next step."
                GoTo DoneChecking
            Else
                ' Add this to our mapping dictionary so we can replace barcode with location ID
                ' during the XML creation
                ' GetLocationFromBarcode returns a string like "2281,L1041,Containers,0" if found
                 arrLocationFields = Split(sLocationID, ",")
                 ' Checking Group Authority
                 If UBound(arrLocationFields) > 5 Then
                    If arrLocationFields(6) = 0 Then
                        sErr = "You are not Location Admin of the following locations:" & vbLf & vbLf
                        sErr = "You are not Location Admin of the Location containing barcode '" & sBarcode & "'" & vbLf
                        sErr = sErr & "The location barcode must be fixed before the upload can proceed.  " & vbLf
                        GoTo DoneChecking
                    End If
                 End If
                 If UBound(arrLocationFields) > 5 Then
                    If Not moGroupSecurityMapping.Exists(sBarcode) Then
                        moGroupSecurityMapping.Add sBarcode, arrLocationFields(7)
                     End If
                 End If
                sLocationID = arrLocationFields(0)
                If Not moLocationBarcodeMapping.Exists(sBarcode) Then
                     moLocationBarcodeMapping.Add sBarcode, sLocationID
                End If
            End If
        End If
    End If
         
    ' Validate the compound ID
    TempDict.RemoveAll
    If moDictFields("C_COMPOUNDIDFK").SDFileField <> "" Then
        bCheckDefault = False
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(moDictFields("C_COMPOUNDIDFK").SDFileField, eDBtoVB))
        While Not rs.EOF
            sCompoundID = rs.fields(0).value
            If sCompoundID <> "" Then
                lCompoundID = CLng(sCompoundID)
                If lCompoundID >= -1 Then
                    sReturn = IsValidCompoundID(lCompoundID)
                    If InStr(LCase(sReturn), "error") > 0 Or Not IsNumeric(sReturn) Then
                        sErr = sReturn
                        GoTo DoneChecking
                    End If
                
                    If CLng(sReturn) = 0 Then
                        TempDict.Add sCompoundID, sCompoundID
                    End If
                End If
            Else
                bCheckDefault = True
            End If
            rs.MoveNext
        Wend
        rs.Close
        Set rs = Nothing
            
        If TempDict.count > 0 And TempDict.count <= 5 Then
            sErr = "The following compound IDs do not exist within Inventory:" & vbLf & vbLf
            For Each key In TempDict
                sErr = sErr & vbTab & key & vbLf
            Next
            sErr = sErr & vbLf & "These compound IDs must be fixed before the upload can proceed."
            GoTo DoneChecking
        ElseIf TempDict.count > 5 Then
            sErr = "Multiple compound IDs from this database do not exist within Inventory." & vbLf & vbLf
            sErr = sErr & "These compound IDs must be fixed before the upload can proceed."
            GoTo DoneChecking
        End If
    Else
        bCheckDefault = True
    End If
    
    If bCheckDefault Then
        sCompoundID = moDictFields("C_COMPOUNDIDFK").value
        If sCompoundID <> "" Then
            If IsNumeric(sCompoundID) Then
                lCompoundID = CLng(sCompoundID)
                ' -1 is the "Invalid substance assigned" compound.  This is part of some customer workflows (e.g. Cubist)
                If lCompoundID >= -1 Then
                    sReturn = IsValidCompoundID(lCompoundID)
                    If InStr(LCase(sReturn), "error") > 0 Or Not IsNumeric(sReturn) Then
                        sErr = sReturn
                        GoTo DoneChecking
                    End If
                
                    If CLng(sReturn) = 0 Then
                        sErr = "Compound ID " & lCompoundID & " was not found in the Inventory database.  " & vbLf
                        sErr = sErr & "The compound ID must be fixed before the upload can proceed."
                        GoTo DoneChecking
                   End If
                End If
            Else
                sErr = "Compound ID " & sCompoundID & " is not numeric.  "
                GoTo DoneChecking
            End If
        End If
    End If
    
    'Validating the Registration Number field
'    If (Not moDictFields("C_REGNUMBER").UseDefault Or Len(moDictFields("C_REGNUMBER").value) <> 0) And (Not moDictFields("C_BATCHNUMBERFK").UseDefault Or Len(moDictFields("C_BATCHNUMBERFK").value) <> 0) Then
     If HasProperty("C_REGNUMBER") And HasProperty("C_BATCHNUMBERFK") Then
        sReturn = CheckRegValues(moDictFields, "C_REGNUMBER", "C_BATCHNUMBERFK")
        If sReturn <> "" Then
            sErr = sReturn
        End If
        GoTo DoneChecking
    End If
DoneChecking:
    CheckContainerValues = sErr
    Set TempDict = Nothing
    
End Function

Function CheckRegValues(ByRef objdict As Dictionary, ByRef RegNumberField As Variant, ByRef BatchNumberField As Variant) As String
    Dim rs As Recordset
    Dim bRegNumberMapped As Boolean
    Dim bBatchNumberMapped As Boolean
    Dim sRegNumber, sBatchNumber, sArrRegNumber, sArrBatchNumber, count, sReturnVal, sRegId, sArrRegFields, limit, tempReg, tempbatch
    Dim TempDict As Dictionary
    Dim vkey
    
    Set TempDict = New Dictionary
    bRegNumberMapped = CastDBField(objdict(RegNumberField)).SDFileField <> ""
    bBatchNumberMapped = CastDBField(objdict(BatchNumberField)).SDFileField <> ""
    
    If Not bRegNumberMapped Then
         sRegNumber = Trim(objdict(RegNumberField).value)
         sArrRegNumber = Split(sRegNumber, ",")
     End If
     If Not bBatchNumberMapped Then
         sBatchNumber = Trim(objdict(BatchNumberField).value)
         sArrBatchNumber = Split(sBatchNumber, ",")
     End If
     
     If bRegNumberMapped Or bBatchNumberMapped Then
         Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
         'Set rs = moCFWImporter.DistinctRecordset(CnvLong(moDictFields(eRegNumber).SDFileField, eDBtoVB))
         count = 0
         While Not rs.EOF
             If bRegNumberMapped Then sRegNumber = sRegNumber & "," & Trim(GetProperty(RegNumberField, rs))
             If bBatchNumberMapped Then sBatchNumber = sBatchNumber & "," & Trim(GetProperty(BatchNumberField, rs))
             rs.MoveNext
         Wend
         If bRegNumberMapped Then sRegNumber = Mid(sRegNumber, 2, Len(sRegNumber))
         If bBatchNumberMapped Then sBatchNumber = Mid(sBatchNumber, 2, Len(sBatchNumber))
         rs.Close
         Set rs = Nothing
         If bRegNumberMapped Then
             sArrRegNumber = Split(sRegNumber, ",")
             sRegNumber = Null
         End If
         If bBatchNumberMapped Then
             sArrBatchNumber = Split(sBatchNumber, ",")
             sBatchNumber = Null
         End If
     End If
     If Not (IsNull(sRegNumber) Or IsNull(sBatchNumber)) Then
          sReturnVal = GetBatchInfo(CStr(sRegNumber), CLng(sBatchNumber))
          If Len(sReturnVal) = 0 Then
             CheckRegValues = "No compound found with registration number '" & sRegNumber & "' and batch number '" & sBatchNumber & "' in the Registration database.  "
             CheckRegValues = CheckRegValues & "The registration and batch numbers must be fixed before the upload can proceed.  " & vbLf
            ' GoTo DoneChecking
         Else
            If InStr(1, sReturnVal, "Error: ") > 0 Then
                CheckRegValues = sReturnVal
            Else
                'Get BatchInfo returns in following string format :-  regnumber,notebook, regpage, regscientist,  regamountunits,  regamount,regid, batchnumber, 'END' as End_Char
                sArrRegFields = Split(sReturnVal, ",")
                sRegId = sArrRegFields(6)
                If Not moRegistrationNumberMapping.Exists(sRegNumber) Then
                    moRegistrationNumberMapping.Add sRegNumber, sRegId
                End If
            End If
         End If
     Else
         If IsNull(sRegNumber) Then
             limit = UBound(sArrRegNumber)
         Else
             limit = UBound(sArrBatchNumber)
         End If
         For count = 0 To limit
             If IsNull(sRegNumber) Then
                 tempReg = sArrRegNumber(count)
             Else
                 tempReg = sRegNumber
             End If
             If IsNull(sBatchNumber) Then
                 tempbatch = sArrBatchNumber(count)
             Else
                 tempbatch = sBatchNumber
             End If
             If Len(tempReg) > 0 And Len(tempbatch) > 0 Then
                 sReturnVal = GetBatchInfo(CStr(tempReg), CLng(tempbatch))
                 
                 If (Len(sReturnVal) = 0 Or IsNull(sReturnVal) Or sReturnVal = "") Then
                     If Not TempDict.Exists(tempReg & "-" & tempbatch) Then
                         TempDict.Add tempReg & "-" & tempbatch, tempReg & "-" & tempbatch
                     End If
                 Else
                    If InStr(1, sReturnVal, "Error: ") > 0 Then
                        CheckRegValues = sReturnVal
                    Else
                        'Get BatchInfo returns in following string format :-  regnumber,notebook, regpage, regscientist,  regamountunits,  regamount,regid, batchnumber, 'END' as End_Char
                        sArrRegFields = Split(sReturnVal, ",")
                        sRegId = sArrRegFields(6)
                        If Not moRegistrationNumberMapping.Exists(tempReg) And Not IsNull(tempReg) Then
                           moRegistrationNumberMapping.Add tempReg, sRegId
                        End If
                    End If
                 End If
             ElseIf (tempbatch = "" Or IsNull(tempbatch)) And tempbatch <> "" Then
                  CheckRegValues = " A numerical Batch Number must be mapped for every Registration Number." & vbLf & vbLf
             End If
         Next
         If TempDict.count > 0 And TempDict.count <= 5 Then
             CheckRegValues = CheckRegValues & "The following registration and batch number combinations do not exist within registration:" & vbLf & vbLf
             For Each vkey In TempDict
                 CheckRegValues = CheckRegValues & vbTab & vkey & vbLf
             Next
             CheckRegValues = CheckRegValues & vbLf & "These registration and batch numbers must be fixed before the upload can proceed."
             'GoTo DoneChecking
         ElseIf TempDict.count > 5 Then
             CheckRegValues = "Multiple registration and batch number combinations from this database do not exist within registration." & vbLf & vbLf
             CheckRegValues = CheckRegValues & "These registration and batch numbers must be fixed before the upload can proceed."
             'GoTo DoneChecking
         End If
     End If
     Set TempDict = Nothing
End Function

' Check a single field for validity.  key is the designator for the field, bCheckRequired indicates whether
' to enforce required checks (not needed for update non-null, too complex for update null).  primekey forces
' the required check for a special field (barcode) needed for updates.
Function CheckFieldDictValues(key As String, bCheckRequired As Boolean, primekey As String) As String
    Dim oField As DBField
    Dim sErr As String
    Dim sValue As String
    Dim vValues As Variant
    Dim vValue As Variant
    Dim rs As Recordset
    Dim iRecord As Long
    Dim bCheckDefault As Boolean
    
'    On Error GoTo Error
    If Not moDictFields.Exists(key) Then
        CheckFieldDictValues = "Field with designator " + key + " not found in field dictionary"
        Exit Function
    End If
    Set oField = moDictFields(key)
    If oField Is Nothing Then
        CheckFieldDictValues = "Field with designator " + key + " had no DBField in field dictionary"
        Exit Function
    End If

    If oField.SDFileField <> "" Then
        bCheckDefault = False
        iRecord = 0
        Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
        While Not rs.EOF
            iRecord = iRecord + 1
            sValue = CnvString(rs(CLng(oField.SDFileField)).value, eDBtoVB)
            If sValue = "" Then
                bCheckDefault = True
            Else
                If oField.CanBeList And mbListAttributes Then
                    vValues = Split(sValue, msListSeparator)
                Else
                    vValues = Array(sValue)
                End If
                For Each vValue In vValues
                    sValue = CStr(vValue)
                    sErr = CheckFieldValue(oField, sValue, bCheckRequired Or oField.Designator = primekey)
                    If sErr <> "" Then
                        CheckFieldDictValues = "Field " + oField.DisplayName + " from input data, record " + CStr(iRecord) + ", " + sErr
                        rs.Close
                        Set rs = Nothing
                        Exit Function
                    End If
                Next vValue
            End If
            rs.MoveNext
        Wend
        rs.Close
        Set rs = Nothing
    Else
        bCheckDefault = True
    End If
    If bCheckDefault Then
        If oField.CanBeList Then
            vValues = Split(oField.value, "|")
        Else
            vValues = Array(oField.value)
        End If
        For Each vValue In vValues
            sErr = CheckFieldValue(oField, CStr(vValue), bCheckRequired)
            If sErr <> "" Then
                CheckFieldDictValues = "Default value for " + oField.DisplayName + " " + sErr
                Exit Function
            End If
        Next vValue
    End If
    CheckFieldDictValues = ""
    Exit Function
Error:
    CheckFieldDictValues = "(CheckFieldDictValues) " + Err.Description
    On Error Resume Next
    If Not rs Is Nothing Then
        rs.Close
        Set rs = Nothing
    End If
End Function

' kfd2
Function CheckFieldValue(oField As DBField, value As String, bCheckRequired As Boolean) As String
    If Len(value) > 0 Then
        If Len(oField.PicklistName) > 0 Then
            If Not GetFieldPicklistDict(oField).Exists(value) Then
                CheckFieldValue = "has value not in picklist " + oField.PicklistName + ": " + value
                Exit Function
            End If
        End If
        If oField.eFieldType = eText Then
            If oField.FieldWidth > 0 Then
                If Len(value) > oField.FieldWidth Then
                    CheckFieldValue = "has value longer than " + CStr(oField.FieldWidth) + " bytes: " + value
                    Exit Function
                End If
            End If
        ElseIf oField.eFieldType = eInteger Then
            If Not CheckLong(value) Then
                CheckFieldValue = "not a valid integer: " + value
                Exit Function
            End If
        ElseIf oField.eFieldType = eReal Then
            If Not CheckDouble(value) Then
                CheckFieldValue = "not a valid real number: " + value
                Exit Function
            End If			
            If InStr(1, CStr(value), ",") > 0  Then
                CheckFieldValue = "has wrong decimal operator: " + value
                Exit Function
            End If
        ElseIf oField.eFieldType = eDate Then
            If Not CheckDate(value) Then
                CheckFieldValue = "not a valid date: " + value
                Exit Function
            End If
        End If
    ElseIf oField.Required And bCheckRequired Then
        CheckFieldValue = "missing required value"
        Exit Function
    End If
    CheckFieldValue = ""
End Function

' kfd2, switched to keys
Private Sub GetValuesFromTable()
    ' get values from the grid
    Dim sMapping As String
    Dim MapDict As Dictionary
    
    Set MapDict = FlipDict(moCFWImporter.FieldsDict)
    
    With grdFieldDict
        Dim ik As Long
        Dim Row As Long
        Dim key As String
        For ik = LBound(msaDictKeys) To UBound(msaDictKeys)
            Row = ik + 1 - LBound(msaDictKeys)
            key = msaDictKeys(ik)
            sMapping = .TextMatrix(Row, eMapping)
            If MapDict.Exists(sMapping) Then
                sMapping = MapDict(sMapping)       ' Combo bug fix
            End If
            If sMapping = "1000" Then
                moDictFields(key).value = .TextMatrix(Row, eValue)
                moDictFields(key).SDFileField = ""
                'moDictFields(key).UseDefault = True
            Else
                moDictFields(key).SDFileField = sMapping
                'moDictFields(key).UseDefault = False
            End If
        Next
    End With
End Sub

Private Sub FillDictGrid()
    Dim i As Long
    Dim key As String
    With grdFieldDict
        .ExtendLastCol = True
        .Rows = 0
        .Cols = 4
        mbGridLoading = True
        .AddItem "Value" & vbTab & "Data Type" & vbTab & "Field Name" & vbTab & "Default"
        msaDictKeys = moDictFields.Keys
        For i = LBound(msaDictKeys) To UBound(msaDictKeys)
            key = msaDictKeys(i)
            .AddItem moDictFields(key).DisplayName & vbTab & GetFieldTypeAsString(moDictFields(key).eFieldType) & vbTab & "1000" & vbTab & moDictFields(key).value
        Next i
        .FixedRows = 1
        .FixedCols = 2
        .ColComboList(eMapping) = GridBuildComboList(moCFWImporter.FieldsDict)
        .AutoSize 0, 3
        mbGridLoading = False
    End With
End Sub

Private Sub FillBarcodeGrid()
    With grdBarcodes
        .Rows = 0
        .Cols = 3
        .AddItem "Plate Number" & vbTab & "Barcode" & vbTab & "Group"
        Dim i As Long
        For i = 1 To mlNumPlates
            .AddItem i & vbTab & moDictBarcodes(CStr(i)) & vbTab & moDictGroups(CStr(i))
        Next
        .FixedRows = 1
        .AutoSize 0, 2
    End With
End Sub


Function GetRegPicklist(sListName As String) As Dictionary
    Dim sXML As String
    Dim sMsg As String
    Dim sList As String
    
    sXML = HTTPRequest("POST", msServerName, mbUseSSL, RegApiFolder & "/GetPicklistXML.asp", "", "picklist=" & sListName & "&USRCODE=" & URLEncode(msUserID) & "&AUTHENTICATE=" & URLEncode(msPassword)) 'CSBR-156006: SJ
    Set GetRegPicklist = DictFromRegPicklistXML(sXML)
End Function

Private Sub btnAdvancedOptions_Click()
    With frmAdvancedOptions
        .Show vbModal, Me
    End With
End Sub

Private Sub chkLog_Click()
    If chkLog.value = 1 Then
        txtLogLoc.Enabled = True
        chkOpenLog.Enabled = True
    Else
        txtLogLoc.Enabled = False
        chkOpenLog.Enabled = False
        chkOpenLog.value = 0
    End If
End Sub

Private Sub chkRegister_Click()
    Dim originalCaption
    Dim URL As String
    Dim response As String
    Dim sArray
    
    If chkRegister.value = vbChecked Then
        
        originalCaption = chkRegister.Caption
        
        Screen.MousePointer = vbHourglass
        chkRegister.Caption = originalCaption + ".   Authenticating user..."
        
        'SYAN moved reg authentication here because it is slow 7/9/2004
        'Authenticate only when reg is needed.
        
        'check if this user is authorized to register compounds
        'response = HTTPRequest("POST", msServerName, RegApiFolder & "/reg_post_action.asp", "", "reg_method=reg_perm&USER_ID=" & msUserID & "&USER_PWD=" & msPassword & "&REG_PARAMETER=AUTHENTICATE")
        response = HTTPRequest("POST", msServerName, mbUseSSL, RegApiFolder & "/CheckRegisterPriv.asp", "", "USRCODE=" & URLEncode(msUserID) & "&AUTHENTICATE=" & URLEncode(msPassword)) 'CSBR-156006: SJ
        sArray = Split(response, "+") 'CSBR 135264 SJ Appending the RLS status to the response from  the server.
        If UBound(sArray) > 0 Then
            response = sArray(0)
            RLSStatus = sArray(1)
        End If
        
        Screen.MousePointer = vbDefault
        If response = "user authenticated" Then
            mbRegister = True
            fraListAttributes.Visible = True
        Else
            MsgBox ("The user does not have privileges to register compounds." & vbCrLf & vbCrLf & response)
            chkRegister.value = Bool2Checkbox(False)
            mbRegister = False
        End If
        
        chkRegister.Caption = originalCaption
        
    Else
        mbRegister = False
    End If
End Sub

Private Sub cmbMolTable_Click()
    If cmbMolTable <> "" Then
        moCFWImporter.MolTable = cmbMolTable
        moCFWImporter.SetUpDictionary
        If moCFWImporter.NumRecords > 0 Then
            FillDictGrid
            Label28.Visible = False
            cmbMolTable.Visible = False
            grdFieldDict.Visible = True
            cmdLoadChemMappings.Visible = True
            cmdSaveChemMappings.Visible = True
            Label9.Visible = True
            ActiveWizard1.NextButtonEnabled = True
        Else
            ActiveWizard1.NextButtonEnabled = False
            MsgBox "There are no records in the " & cmbMolTable & " table.  Please choose another table to use.", vbExclamation
        End If
    End If
End Sub

Private Sub cmbPlateFormat_Click()
    UpdateNumPlates
End Sub

Private Sub cmbPrefix_Change(Index As Integer)
    ' keep in sync
    Select Case Index
        Case 0
            If cmbPrefix(0).Text <> cmbPrefix(1).Text Then
                cmbPrefix(1).Text = cmbPrefix(0).Text
            End If
        Case 1
            If cmbPrefix(1).Text <> cmbPrefix(0).Text Then
                cmbPrefix(0).Text = cmbPrefix(1).Text
            End If
    End Select

End Sub

Private Sub cmbPrefix_Click(Index As Integer)
    ' keep in sync
    Select Case Index
        Case 0
            ComboSelectItemData cmbPrefix(1), ComboItemData(cmbPrefix(0))
        Case 1
            ComboSelectItemData cmbPrefix(0), ComboItemData(cmbPrefix(1))
    End Select

End Sub

Private Sub cmdLoadChemMappings_Click()
    Dim eMappingType As MappingType
    Dim NameToIndexMapping As Dictionary
    Dim i As Long
    
    Set NameToIndexMapping = New Dictionary
    
    With grdFieldDict
        Select Case miLoadOption
            Case ePlates
                eMappingType = eInventoryPlates
            Case eStructures
                eMappingType = eInventoryStructures
            Case eContainers
                eMappingType = eInventoryContainers
        End Select
        
        For i = 1 To UBound(msaDictKeys) - LBound(msaDictKeys) + 1
            NameToIndexMapping.Add .TextMatrix(i, eDisplayName), i
        Next
    End With
            
    Call UtilsMisc.LoadFieldMappings(grdFieldDict, moCFWImporter, eMappingType, NameToIndexMapping)
    Set NameToIndexMapping = Nothing
End Sub

Private Sub cmdSaveChemMappings_Click()
    Dim eMappingType As MappingType
    
    Select Case (miLoadOption)
        Case ePlates
            eMappingType = eInventoryPlates
        Case eStructures
            eMappingType = eInventoryStructures
        Case eContainers
            eMappingType = eInventoryContainers
    End Select
    
    Call UtilsMisc.SaveFieldMappings(grdFieldDict, moCFWImporter, eMappingType)
    
End Sub

Private Sub cmdBrowse_Click()
    'SYAN added to fix CSBR-46919
    Screen.MousePointer = vbHourglass
    'End of SYAN modification
    
    ' browse for datafile
    With CommonDialog1
        .Filter = moCFWImporter.OpenDatabaseFilter()
        .FilterIndex = glFilterIndex
        .ShowOpen
    End With
    
     'SYAN added to fix CSBR-46919
    Screen.MousePointer = vbNormal
    'End of SYAN modification

    If CommonDialog1.fileName <> "" Then
        moCFWImporter.SetFilterIndex (CommonDialog1.FilterIndex)
        msFile = CommonDialog1.fileName
        ' need library of text handling
        Dim oFS As FileSystemObject: Set oFS = New FileSystemObject
        If oFS.FileExists(msFile) Then
            txtDB.Text = msFile
            glFilterIndex = CommonDialog1.FilterIndex
            Call UtilsMisc.SetGlobals
        End If
    End If
End Sub

Private Sub Form_Load()
    Call UtilsMisc.GetGlobals
    Call UtilsMisc.GetAllFieldMappings
    
    ' set up grid
    ActiveWizard1.FinishButtonEnabled = False
    LocationID = NULL_AS_LONG
'    Set moDictFields = New Dictionary  'kfdx
'    Set moCompoundDictFields = New Dictionary
'    Set moContainerDictFields = New Dictionary
    Set moLocationBarcodeMapping = New Dictionary
    Set moGroupSecurityMapping = New Dictionary
    Set moRegistrationNumberMapping = New Dictionary
    ' SetDBFields  'kfd2, moved to Initialize
    GridFormatGeneric grdBarcodes
    GridFormatGeneric grdFieldDict
    grdBarcodes.Rows = 0
    ' other properties
    mlPerPlate = 0
    mbFinished = False
    Set moDictBarcodes = New Dictionary
    Set moDictGroups = New Dictionary
    Set moApplicationVariablesDict = New Dictionary
    bLoadMappingsFromXML = False
    
    Label1.Caption = "This wizard will import compounds from a ChemFinder or Excel database directly into Inventory Manager, a series of new plates, or new containers.  Most often, your compounds will start in an SDFile.  To convert this SDFile into a ChemFinder database, please do the following:"
    lblVersion.Caption = CStr(App.Major) + "." + CStr(App.Minor) + "." + CStr(App.Revision)
    
End Sub

Private Function GetNewLocation() As Long
    GetNewLocation = ComboItemData(cmbLocation)
End Function

Private Sub Form_Unload(Cancel As Integer)
    Set moDictFields = Nothing
    Set moLocationBarcodeMapping = Nothing
    Set moGroupSecurityMapping = Nothing
    Set moRegistrationNumberMapping = Nothing
    Set moDictBarcodes = Nothing
    Set moDictGroups = Nothing
End Sub

Private Sub grdBarcodes_ValidateEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
    ' only allow numeric values
    If Not IsNumeric(grdBarcodes.EditText) Then Beep: Cancel = True
End Sub


Private Sub GridFieldDict_ValueChanged(ByVal Row As Long, ByVal Col As Long)
    Dim oField As DBField
    Dim sValue As String
    Dim LongValue As Long
    Dim DoubleValue As Double
    Dim DateValue As Date
    Dim lError As Long
    Dim sErrMsg As String
    
    With grdFieldDict
        sValue = Trim(.TextMatrix(Row, Col))
        .TextMatrix(Row, Col) = sValue
        If Len(sValue) = 0 Then
            Exit Sub
        End If
                    
        sErrMsg = ""
            
        If moDictFields.Exists(Row) Then
            If TypeName(moDictFields(msaDictKeys(Row - 1))) = "DBField" Then
                Set oField = moDictFields(msaDictKeys(Row - 1))
                Select Case oField.eFieldType
                    Case eText
                        ' no restrictions
                    Case eInteger
                        On Error Resume Next
                        LongValue = CLng(sValue)
                        lError = Err.Number
                        On Error GoTo 0
                        .TextMatrix(Row, Col) = LongValue
                        If lError = 13 Then
                            sErrMsg = "Please enter an integer value for the " & oField.DisplayName & " field."
                        End If
                    Case eReal
                        On Error Resume Next
                        DoubleValue = CDbl(sValue)
                        lError = Err.Number
                        On Error GoTo 0
                        .TextMatrix(Row, Col) = DoubleValue
                        If lError = 13 Then
                            sErrMsg = "Please enter a numeric value for the " & oField.DisplayName & " field."
                        End If
                    Case eDate
                        On Error Resume Next
                        DateValue = Format(sValue, moApplicationVariablesDict("DATE_FORMAT_STRING"))
                        'CDate(sValue)
                        lError = Err.Number
                        On Error GoTo 0
                        .TextMatrix(Row, Col) = CStr(Format(DateValue, moApplicationVariablesDict("DATE_FORMAT_STRING")))
                        If lError = 13 Then
                            sErrMsg = "Please enter a valid date value for the " & oField.DisplayName & " field."
                        End If
                End Select
            End If
        End If
        
        If sErrMsg <> "" Then
            MsgBox sErrMsg, vbExclamation
            .Select Row, Col
            .EditCell
        End If
    End With
End Sub

Private Sub GridFieldDict_MappingChanged(ByVal Row As Long, ByVal Col As Long)
    Dim sMapping As String
    Dim FieldType
    Dim oField As DBField
    Dim sErrorMessage
    Dim sFieldPrefix As String
    Dim sFieldType As String
    Dim bError As Boolean
        
    If TypeName(moDictFields(msaDictKeys(Row - 1))) <> "DBField" Then
        Exit Sub
    End If
    Set oField = moDictFields(msaDictKeys(Row - 1))
    
    bError = False
        
    With grdFieldDict
        sMapping = .TextMatrix(Row, eMapping)
        If sMapping <> "1000" Then
            ' Only validate rows that correspond to database fields
            If Len(oField.FieldName) > 0 Then
                FieldType = moCFWImporter.GetFieldType(sMapping)
                
                Select Case oField.eFieldType
                    Case eText
                    Case eInteger
                        If FieldType <> adInteger And FieldType <> adSmallInt And FieldType <> adTinyInt And FieldType <> adUnsignedInt And FieldType <> adUnsignedSmallInt And FieldType <> adUnsignedTinyInt Then
                            bError = True
                            sFieldPrefix = "an "
                            sFieldType = "integer"
                        End If
                    Case eReal
                        If FieldType <> adSingle And FieldType <> adDouble And FieldType <> adDecimal And FieldType <> adInteger And FieldType <> adSmallInt And FieldType <> adTinyInt And FieldType <> adUnsignedInt And FieldType <> adUnsignedSmallInt And FieldType <> adUnsignedTinyInt Then
                            bError = True
                            sFieldPrefix = "a "
                            sFieldType = "decimal"
                        End If
                    Case eDate
                        If FieldType <> adDate And FieldType <> adDBDate And FieldType <> adDBTime And FieldType <> adDBTimeStamp Then
                            bError = True
                            sFieldPrefix = "a "
                            sFieldType = "date"
                        End If
                    Case Else
                End Select
            End If
        End If
    End With
    
    If bError Then
        sErrorMessage = "Warning: " & vbLf & vbLf
        sErrorMessage = sErrorMessage & oField.DisplayName & ", " & sFieldPrefix & sFieldType & " field, is being loaded from a " & GetADOFieldTypeString(CLng(FieldType)) & " field." & vbLf
        sErrorMessage = sErrorMessage & "This may cause the import to fail.  Please validate the data to ensure it contains only " & sFieldType & " records."
        MsgBox sErrorMessage, vbExclamation
    End If
End Sub

Private Sub grdFieldDict_StartEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
    Dim sKey As String
    Dim oField As DBField
    Dim oDict As Dictionary
    Dim sInitValue As String
    If Col = eValue Then
        sKey = msaDictKeys(Row - LBound(msaDictKeys) - 1)
        Set oField = moDictFields(sKey)
        If Not (oField Is Nothing) Then
            If oField.CanBeList Then
                If Len(oField.PicklistName) > 0 Then
                    Set oDict = GetFieldPicklistDict(oField)
                    If Not oDict Is Nothing Then
                        sInitValue = grdFieldDict.TextMatrix(Row, Col)
                        frmPickListField.Initialize sInitValue, oDict
                        frmPickListField.Show vbModal, Me
                        If Not frmPickListField.Cancel Then
                            grdFieldDict.TextMatrix(Row, Col) = frmPickListField.value
                        End If
                    End If
                Else
                    frmListField.Intialize grdFieldDict.TextMatrix(Row, Col)
                    frmListField.Show vbModal, Me
                    If Not frmListField.Cancel Then
                        grdFieldDict.TextMatrix(Row, Col) = frmListField.value
                    End If
                End If
                Cancel = True
            End If
        End If
    End If
End Sub

'kfd2
Private Sub grdFieldDict_BeforeEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
    Dim sKey As String
    Dim oField As DBField
    Dim oDict As Dictionary
    Dim sCombo As String
    If Col = eValue Then
        sCombo = ""
        sKey = msaDictKeys(Row - LBound(msaDictKeys) - 1)
        Set oField = moDictFields(sKey)
        If Not (oField Is Nothing) Then
            If Not oField.CanBeList Then
                If Len(oField.PicklistName) > 0 Then
                    Set oDict = GetFieldPicklistDict(oField)
                    If Not oDict Is Nothing Then
                        If oField.Required Then
                            sCombo = PicklistFromDictKeys(oDict, False, False)
                        Else
                            sCombo = PicklistFromDictKeys(oDict, False, True)
                        End If
                    End If
                End If
            End If
        End If
        grdFieldDict.ComboList = sCombo
    End If
End Sub
    
Private Sub grdFieldDict_CellChanged(ByVal Row As Long, ByVal Col As Long)
    If bLoadMappingsFromXML Or mbGridLoading Then
        Exit Sub
    End If
    
    Select Case (Col)
        Case eMapping
            GridFieldDict_MappingChanged Row, Col
        Case eValue
            GridFieldDict_ValueChanged Row, Col
    End Select
End Sub


Private Sub CreateContainerXML()
' kfd2, the 4 statements below were never used.  Convenient, since they have an error!
'        Dim lContainerFld As Long
'        If Not moContainerDictFields(eCBarcode).SDFileField = "" Then
'            lContainerFld = eBarcode
'        End If
      '  If chkLog.value = 1 Then
         '   LogAction "<INV_LOADER TIME_STAMP=""" & Now & """>", xmlLogPath
        'End If
        Dim i As Long: i = 0
        Dim key As Variant
        Dim XMLContainer As DOMDocument
        Dim oContainerNode As IXMLDOMElement
        Dim oOptionalNode As IXMLDOMElement
        Dim objPI As IXMLDOMProcessingInstruction
        Dim oRootNode As IXMLDOMElement
        Dim oNode As IXMLDOMElement
        Dim sUserID As String
        Dim sPassword As String
        Dim sServerName As String
        Dim oNode1 As IXMLDOMElement
        Dim j As Long, k As Long
        Dim sRegistrationNumber As String
        Dim qtyInitial, qtyUnitFK, Solvent, concentration, concUnitFK, weight, weightUnitFK As String
        ' get number of rows and columns
        Dim rs As Recordset
        Dim sBase64Cdx
        Dim sRet
        Dim RegID
        Dim bHasCompoundID As Boolean
        Dim vProperty As Variant
        Dim CompoundID As String
        Dim initialQty As String
        Dim qtyMax As String
        Dim sLocationBarcode As String
        Dim lContainerID As Long
        Dim lContainerCount As Long
        Dim lContainerLimit As Long
        Dim lFileCount As Long
        Dim bNewFile As Boolean
        Dim oField As DBField
        Dim sPrincipalID As Integer
        
        sUserID = txtUserID.Text
        sPassword = txtPassword.Text
        sServerName = txtServer.Text
        
        Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
        lContainerID = 1
        lFileCount = 0
        lContainerCount = 0
        bNewFile = True
        Do Until rs.EOF
            If bNewFile Then
                Set XMLContainer = New DOMDocument40
                With XMLContainer
                    Set objPI = XMLContainer.createProcessingInstruction("xml", "version=""1.0""")
                    .appendChild objPI
                    Set oRootNode = .createElement("CONTAINERS")
                     With oRootNode
                       '     .setAttribute "CSUSERNAME", sUserID
                       '     .setAttribute "CSUSERID", sPassword
                            .setAttribute "REGSERVER", sServerName
                            If miUpdateMode = eUpdateNonNull Then
                                .setAttribute "UPDATE", "NONNULL"
                            ElseIf miUpdateMode = eUpdateToNull Then
                                .setAttribute "UPDATE", "ALL"
                            End If
                            If mbRegister And (miUpdateMode <> eUpdateNone) Then
                                If miUpdateCompoundMode = eUpdateNonNull Then
                                    .setAttribute "UPDATECOMPOUNDS", "NONNULL"
                                ElseIf miUpdateCompoundMode = eUpdateToNull Then
                                    .setAttribute "UPDATECOMPOUNDS", "ALL"
                                Else  ' should not happen
                                    .setAttribute "UPDATECOMPOUNDS", "NONE"
                                End If
                            End If
                    End With
                    Set .documentElement = oRootNode
                End With
                bNewFile = False
            End If
                
            lblOverallProg.Caption = "Container " & progOverall.value - 1 & " of " & mlNumCompounds
            With XMLContainer
                Set oContainerNode = XMLContainer.createElement("CREATECONTAINER")
                oRootNode.appendChild oContainerNode
                 
                With oContainerNode
                    .setAttribute "ID", CStr(lContainerID)
                    bHasCompoundID = False
                    CompoundID = GetProperty2("C_COMPOUNDIDFK", rs)
                    If (CompoundID <> "" And Not IsNull(CompoundID)) Then
                        .setAttribute "COMPOUNDID", CompoundID
                        bHasCompoundID = True
                    End If
                     
                    If GetProperty("C_REGNUMBER", rs) <> "" And Not IsNull(GetProperty("C_REGNUMBER", rs)) And bHasCompoundID <> True Then
                        sRegistrationNumber = GetProperty("C_REGNUMBER", rs)
                        If moRegistrationNumberMapping.Exists(sRegistrationNumber) Then
                            RegID = moRegistrationNumberMapping.Item(sRegistrationNumber)
                        End If
                        .setAttribute "REGID", RegID
                        .setAttribute "BATCHNUMBER", GetProperty("C_BATCHNUMBERFK", rs)
                        bHasCompoundID = True
                    End If
                    
                    .setAttribute "UOMID", GetProperty2("C_UNITOFMEASIDFK", rs)
                    
                    initialQty = GetProperty2("C_QTYINITIAL", rs)
                    If initialQty = "" Then
                        initialQty = 0
                    End If
                    .setAttribute "INITIALQTY", initialQty
                    .setAttribute "QTYREMAINING", initialQty
                                          
                    sLocationBarcode = GetProperty("C_LOCATIONBARCODE", rs)
                    If sLocationBarcode <> "" Then
                        ' Go look up the actual location ID from the barcode.  This was populated
                        ' in CheckValues()
                        If moLocationBarcodeMapping.Exists(sLocationBarcode) Then
                            LocationID = moLocationBarcodeMapping.Item(sLocationBarcode)
                        Else
                            LocationID = GetLocationFromBarcode(sLocationBarcode)
                            moLocationBarcodeMapping.Add sLocationBarcode, LocationID
                        End If
                        .setAttribute "LOCATIONID", CStr(LocationID)
                    Else
                        .setAttribute "LOCATIONID", CStr(LocationID)
                    End If
                    
                    If moGroupSecurityMapping.Exists(sLocationBarcode) Then
                    If Not IsNull(moGroupSecurityMapping.Item(sLocationBarcode)) And moGroupSecurityMapping.Item(sLocationBarcode) <> "" Then
                        sPrincipalID = moGroupSecurityMapping.Item(sLocationBarcode)
                        .setAttribute "PRINCIPALID", CStr(sPrincipalID)
                        End If
                    End If
                    
                    .setAttribute "CONTAINERTYPEID", GetProperty2("C_CONTAINERTYPEIDFK", rs)
                    .setAttribute "CONTAINERSTATUSID", GetProperty2("C_CONTAINERSTATUSIDFK", rs)
                     
                    qtyMax = GetProperty("C_QTYMAX", rs)
                    If miUpdateMode = eUpdateNonNull Then
						If qtyMax <> "" And Not IsNull(qtyMax) Then
							.setAttribute "MAXQTY", qtyMax
						End If
					Else
						If qtyMax = "" Or IsNull(qtyMax) Then
							If initialQty = 0 Then
								qtyMax = 1
							Else
								qtyMax = initialQty
							End If
						End If
						.setAttribute "MAXQTY", qtyMax
					End If
                End With
                
                Set oOptionalNode = XMLContainer.createElement("OPTIONALPARAMS")
                oContainerNode.appendChild oOptionalNode
                 
                For Each key In moDictFields
                    Set oField = moDictFields(key)
                    If oField.eFieldSubstClass = eClassNeither And _
                      oField.XMLPosition = "ELEMENT" And _
                      oField.XMLParent = "OPTIONALPARAMS" Then
                        If oField.FieldName <> "" Then
                            Set oNode1 = XMLContainer.createElement(oField.FieldName)
                            oOptionalNode.appendChild oNode1
                            If oField.FieldName = "BARCODE" Then
                                oNode1.Text = UCase(GetProperty2(key, rs))
                            Else
                                oNode1.Text = GetProperty2(key, rs)
                            End If
                        End If
                    End If
                Next key
                
                Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "NUMCOPIES", "")
                oOptionalNode.appendChild oNode1
                oNode1.Text = "1"


             End With            ' With XMLContainer
             
            ' If we have a compound ID, then don't add the compound information... it already
            ' exists in Inventory
            If Not bHasCompoundID Then
                sBase64Cdx = moCFWImporter.Base64CDX(rs)
                If sBase64Cdx = "53 File not found" Or sBase64Cdx = "53+File+not+found" Then sBase64Cdx = ""
                 
                If mbRegister Then
                    If sBase64Cdx = "" Then sBase64Cdx = "No Structure"             ' Special processing for Registration (CSBR-134649)
                    Set oNode = XMLContainer.createElement("REGISTERSUBSTANCE")
                    oRootNode.appendChild oNode
                    With oNode
                        .setAttribute "PACKAGEID", CStr(lContainerID)
                        For Each key In moDictFields
                            Set oField = moDictFields(key)
                            If oField.eFieldSubstClass = eClassRegCompound Then
                                If oField.FieldName <> "" Then
                                    For Each vProperty In GetPropertyList(key, rs)
                                        'This test removes empty property lists
                                        If CStr(vProperty) <> "" Then
                                            ' Switched from attributes to elements to handle property lists nicely, kfd2
                                            ' .setAttribute oField.FieldName, vProperty
                                            Set oNode1 = XMLContainer.createElement(oField.FieldName)
                                            .appendChild oNode1
                                            oNode1.Text = CStr(vProperty)
                                        End If
                                    Next vProperty
                                End If
                        End If
                        Next key
                        ' Obsolete, replaced by element, below, kfd2
                        '.setAttribute "BASE64_CDX", URLEncode(sBase64Cdx)
                    End With
                    Set oNode1 = XMLContainer.createElement("structure")
                    oNode1.Text = sBase64Cdx
                    oNode.appendChild oNode1
                             
                Else    ' not mbRegister Then
                    Set oNode = XMLContainer.createElement("CREATESUBSTANCE")
                    oRootNode.appendChild oNode
                    With oNode
                        .setAttribute "PACKAGEID", CStr(lContainerID)
                        For Each key In moDictFields
                            Set oField = moDictFields(key)
                            If oField.eFieldSubstClass = eClassInvCompound And _
                              oField.XMLPosition = "ATTRIBUTE" Then
                                If oField.FieldName <> "" Then
                                    .setAttribute oField.FieldName, GetProperty2(key, rs)
                                End If
                            End If
                        Next key
                    End With
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "substanceName", "")
                    oNode1.Text = IIf(Len(GetProperty("C_CONTAINERSUBSTANCENAME", rs)) > 0, GetProperty("C_CONTAINERSUBSTANCENAME", rs), "No Name")
                    oNode.appendChild oNode1
                    
                    oNode.setAttribute "Type", "BASE64_CDX"
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "structure", "")
                    oNode1.Text = sBase64Cdx
                    oNode.appendChild oNode1
                End If
            End If
             
            progOverall.value = progOverall.value + 1
            progOverall.Refresh
            lblOverallProg.Caption = "Container " & progOverall.value - 1 & " of " & mlNumCompounds
            lblOverallProg.Refresh
             
            rs.MoveNext
            lContainerID = lContainerID + 1
            
            lContainerCount = lContainerCount + 1
            If lContainerCount >= glContainerLimit Or rs.EOF Then
                lblOverallProg.Caption = "Saving sending container data file " + CStr(lFileCount) + ".  This may take a few minutes.."
                modCloseBtn.EnableCloseButton Me.hWnd, False
                sRet = UtilsInventory.SendContainerXML(XMLContainer.xml, mbRegister)
                modCloseBtn.EnableCloseButton Me.hWnd, True
                If gbSaveContainerXML Then
                   XMLContainer.save (xmlLogPath & "DebugContainer_" + CStr(lFileCount) + ".xml")
                End If
                If chkLog.value = 1 Then
                   LogAction Replace(sRet, "<?xml version=""1.0""?>", ""), xmlLogPath
                End If
                lFileCount = lFileCount + 1
                lContainerCount = 0
                Set XMLContainer = Nothing
                bNewFile = True
            End If
        Loop
End Sub

Private Sub CreateSubstanceXML()
    Dim rs As Recordset
    Dim objPI As IXMLDOMProcessingInstruction
    Dim oRootNode As IXMLDOMElement
    Dim oNode As IXMLDOMElement
    Dim sBase64Cdx As String
    Dim sRet As String
    
    Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
    Dim XMLCompounds As DOMDocument
    Set XMLCompounds = New DOMDocument40
    With XMLCompounds
        Set objPI = .createProcessingInstruction("xml", "version=""1.0""")
        .appendChild objPI
        Set oRootNode = .createElement("COMPOUNDS")
        Set .documentElement = oRootNode
    End With
    progOverall.value = 1
    Do Until rs.EOF
'            i = i + 1
        
        ' add compound node
        Set oNode = XMLCompounds.createElement("COMPOUND")
        sBase64Cdx = moCFWImporter.Base64CDX(rs)
        
        With oNode
            .setAttribute "CAS", GetProperty("S_CCAS", rs)
            .setAttribute "ACX_ID", GetProperty("S_ACXID", rs)
            .setAttribute "SUBSTANCE_NAME", IIf(Len(GetProperty("S_SUBSTANCENAME", rs)) > 0, GetProperty("S_SUBSTANCENAME", rs), "No Name")
            .setAttribute "DENSITY", GetProperty("S_DENSITY", rs)
            .setAttribute "CLOGP", GetProperty("S_CCLOGP", rs)
            .setAttribute "ROTATABLE_BONDS", GetProperty("S_CROTATABLEBONDS", rs)
            .setAttribute "TOT_POL_SURF_AREA", GetProperty("S_CTOTPOLSURFAREA", rs)
            .setAttribute "HBOND_ACCEPTORS", GetProperty("S_HBONDACCEPTORS", rs)
            .setAttribute "HBOND_DONORS", GetProperty("S_HBONDDONORS", rs)
            .setAttribute "ALT_ID_1", GetProperty("S_ALTID1", rs) 'CSBR 135381 SJ Correcting the DB field names for the custom fields in Inv_Compounds table.
            .setAttribute "ALT_ID_2", GetProperty("S_ALTID2", rs)
            .setAttribute "ALT_ID_3", GetProperty("S_ALTID3", rs)
            .setAttribute "ALT_ID_4", GetProperty("S_ALTID4", rs)
            .setAttribute "ALT_ID_5", GetProperty("S_ALTID5", rs)
            If sBase64Cdx <> "" Then
                .setAttribute "BASE64_CDX", URLEncode(sBase64Cdx)
            End If
        End With
        oRootNode.appendChild oNode
                
        'update overal status bar
        progOverall.value = progOverall.value + 1
        progOverall.Refresh
        lblOverallProg.Caption = "Compound " & progOverall.value - 1 & " of " & mlNumCompounds
        
        rs.MoveNext
    Loop
    ' tell user we're busy updating
    lblOverallProg.Caption = "Saving to database..."
    DoEvents
            
    'XMLCompounds.save ("c:\debugPlate" & lPlateCount & ".xml")
    
    ' Disable the close button during the upload, as it will not accept a shutdown cleanly
    ' during the asynchronous HTTP post
    modCloseBtn.EnableCloseButton Me.hWnd, False
    sRet = UtilsInventory.SendSubstanceXML(XMLCompounds.xml)
    modCloseBtn.EnableCloseButton Me.hWnd, True
    
    If chkLog.value = 1 Then
        LogAction sRet, xmlLogPath
    End If
    
    If gbSaveCompoundXML Then
        XMLCompounds.save (xmlLogPath & "DebugCompounds.xml")
    End If
End Sub

Private Sub CreatePlateXML()
    ' run the import
    Dim l As Long, lCurrPlateId As Long
    Dim key As Variant
    Dim lPlateCount As Long: lPlateCount = 1
    Dim rs As Recordset
    Dim wellRS As Recordset
    Dim lGroup As Long, lBarcode As Long, sRegId As String
    Dim vCurrPlateId As Variant ' current plate
    Dim Rows As Long, Cols As Long, Row As Long, Col As Long
    Dim sCoord As String
    Dim lSolutionVolume As Long, lSolventVolume As Long
    Dim sVolume As String, sUnit As String
    Dim sUnituM As String, sUnituL As String, sUnitug As String
    Dim sSolventVolume As String
    vCurrPlateId = NULL_AS_LONG
    Dim tempLocationInfo As String
    Dim tempArrLocationFields As Variant
    Dim objPI As IXMLDOMProcessingInstruction
    Dim oRootNode As IXMLDOMElement
    Dim oNode As IXMLDOMElement
    Dim oNode1 As IXMLDOMElement
    Dim oField As DBField
    Dim vProperty As Variant
    Dim RegID, sRegistrationNumber
    
    sUnituM = UtilsInventory.LookUpValue("inv_units", "uM")
    sUnituL = UtilsInventory.LookUpValue("inv_units", "ul")
    sUnitug = UtilsInventory.LookUpValue("inv_units", "ug")
    
    If miLoadOption = ePlates Then
        frmPlateProg.Visible = True
        progPlate.Min = 1
        
        'progPlate.Max = mlPerPlate + 1
        'SYAN changed on 7/26/2004 to fix status bar issues
        If mlNumCompounds <= mlPerPlate Then
            progPlate.Max = mlNumCompounds + 1
        Else
            progPlate.Max = mlPerPlate + 1
        End If
        'End of SYAN modification
        
        progOverall.value = 1
        progPlate.value = 1
    End If
    
    l = 1
    Dim bNewPlate As Boolean: bNewPlate = True
    Dim bLoadCompound As Boolean
    Dim sBase64Cdx
    Dim sRet As String

    ' get the recordset from the importer.  Sort on the plate field so that
    ' our records are grouped by plate. Use barcode first, then supplierplate number, then
    ' supplier barcode
    Dim sPlateFld As String
    If moDictFields("P_BARCODE").SDFileField = "" Then
        If moDictFields("P_SUPPLIERPLATENUMBER").SDFileField = "" Then
            sPlateFld = "P_SUPPLIERBARCODE"
        Else
            sPlateFld = "P_SUPPLIERPLATENUMBER"
        End If
    Else
        sPlateFld = "P_BARCODE"
    End If
    ' if well field, use that, otherwise use row and col fields
    If moDictFields("P_WELLCOORDINATES").SDFileField = "" Then
        Set rs = moCFWImporter.Recordset(CnvLong(moDictFields(sPlateFld).SDFileField, eDBtoVB), _
                                        CnvLong(moDictFields("P_ROW").SDFileField, eDBtoVB), _
                                        CnvLong(moDictFields("P_COL").SDFileField, eDBtoVB))
    Else
        Set rs = moCFWImporter.Recordset(CnvLong(moDictFields(sPlateFld).SDFileField, eDBtoVB), _
                                        CnvLong(moDictFields("P_WELLCOORDINATES").SDFileField, eDBtoVB), _
                                        NULL_AS_LONG)
    End If
    
    'rs.save "c:\test.xml", adPersistXML
    If mbHavePlateCoords Then
        vCurrPlateId = GetProperty(msPlateCoordField, rs)
    End If
    Dim bUpdated As Boolean
    Dim i As Long: i = 0
    Dim XMLPlate As DOMDocument
    Dim oPlateNode As IXMLDOMElement
    Dim oRowNode As IXMLDOMElement
    Dim oColNode As IXMLDOMElement
    Dim oWellNode As IXMLDOMElement
    Dim j As Long, k As Long
    Dim qtyInitial, qtyUnitFK, Solvent, concentration, concUnitFK, weight, weightUnitFK As String
    ' get number of rows and columns
    Dim rs2 As Recordset
    Set rs2 = ExecuteStatement(eStmtPlateDimensions, mlPlateFormat)
    Rows = rs2("Row_count").value
    Cols = rs2("col_count").value
    Do Until rs.EOF
        i = i + 1
        
        If bNewPlate Then
            Row = 1
            Col = 0
            Set XMLPlate = New DOMDocument40
            l = 1
            bNewPlate = False
            bUpdated = False
            lblPlateProg.Caption = "Plate " & lPlateCount & " of " & mlNumPlates
            ' set plate information
            ' we could upload all plates in a single file, but we will upload
            ' individual plates for better error-handling and progress reporting.
            With XMLPlate
                Set objPI = XMLPlate.createProcessingInstruction("xml", "version=""1.0""")
                .appendChild objPI
                Set oRootNode = .createElement("PLATES")
                Set .documentElement = oRootNode
                Set oPlateNode = XMLPlate.createElement("PLATE")
                oRootNode.appendChild oPlateNode
                With oPlateNode
                    .setAttribute "PLATE_TYPE_ID_FK", mlPlateType
                    .setAttribute "PLATE_FORMAT_ID_FK", mlPlateFormat
                    .setAttribute "STATUS_ID_FK", 5
                    .setAttribute "LOCATION_ID_FK", mlNewLocation
                    'add Plate admin for group security, same as of Parent Location
                    tempLocationInfo = GetLocationFromID(mlNewLocation)
                    tempArrLocationFields = Split(tempLocationInfo, ",")
                    If UBound(tempArrLocationFields) > 5 Then
                        .setAttribute "PRINCIPAL_ID_FK", tempArrLocationFields(6)
                    End If
                    .setAttribute "LIBRARY_ID_FK", mlLibrary
                    .setAttribute "SUPPLIER", GetProperty("P_SUPPLIER", rs)
                    If GetProperty("P_SUPPLIERSHIPMENTDATE", rs) <> "" And Not IsEmpty(GetProperty("P_SUPPLIERSHIPMENTDATE", rs)) Then .setAttribute "SUPPLIER_SHIPMENT_DATE", GetProperty("P_SUPPLIERSHIPMENTDATE", rs)
                    .setAttribute "SUPPLIER_SHIPMENT_CODE", GetProperty("P_SUPPLIERSHIPMENTCODE", rs)
                    .setAttribute "SUPPLIER_SHIPMENT_NUMBER", GetProperty("P_SUPPLIERPLATENUMBER", rs)   ' Plate number is shipment number in the database
                    .setAttribute "SUPPLIER_BARCODE", GetProperty("P_SUPPLIERBARCODE", rs)
                    .setAttribute "GROUP_NAME", moDictGroups(CStr(lPlateCount))
                    'initial quantity is either the molar amount or the volume amount, if both then molar is chosen
                    qtyInitial = GetProperty("P_QTYINIT", rs) 'CSBR 167362 SJ Removing molar unit
                    If qtyInitial <> "" And Not IsEmpty(qtyInitial) Then
                        qtyUnitFK = GetProperty2("P_QTYINITUNIT", rs) 'CSBR 166677 SJ adding initial quantity unit
                    End If
                    .setAttribute "QTY_INITIAL", qtyInitial
                    .setAttribute "QTY_REMAINING", qtyInitial
                    .setAttribute "QTY_UNIT_FK", qtyUnitFK
                    .setAttribute "SOLVENT", GetProperty("P_SOLVENT", rs)
                    
                    sSolventVolume = GetProperty("P_SOLVENTVOLUME", rs)
                    If sSolventVolume = "" Or IsEmpty(sSolventVolume) Then sSolventVolume = "0"
                    .setAttribute "SOLVENT_VOLUME", sSolventVolume
                    
                    sSolventVolume = GetProperty("P_SOLVENTVOLUMEINITIAL", rs)
                    If sSolventVolume = "" Or IsEmpty(sSolventVolume) Then sSolventVolume = "0"
                    .setAttribute "SOLVENT_VOLUME_INITIAL", sSolventVolume
                    
                    sUnit = GetProperty2("P_SOLVENTVOLUMEUNIT", rs)
                    If sUnit = "" Or IsEmpty(sUnit) Then sUnit = sUnituL  ' Remove this when units dropdowns added for all plate units, kfd2
                    .setAttribute "SOLVENT_VOLUME_UNIT_ID_FK", sUnit
                    
                    concentration = GetProperty("P_CONCENTRATION", rs)
                    concUnitFK = GetProperty2("P_CONCENTRATIONUNIT", rs) 'CSBR 165436 SJ Added picklist for concentration unit.
                    sUnituM = concUnitFK
                    If concentration <> "" And Not IsEmpty(concentration) Then concUnitFK = sUnituM 'uM
                    .setAttribute "CONCENTRATION", concentration
                    .setAttribute "CONC_UNIT_FK", concUnitFK
                    weight = GetProperty("P_WEIGHT", rs)
                    weightUnitFK = GetProperty2("P_WEIGHTUNIT", rs) 'CSBR 167407 SJ Added picklist for weight unit.
                    sUnitug = weightUnitFK
                    If weight <> "" And Not IsEmpty(weight) Then weightUnitFK = sUnitug 'ug
                    .setAttribute "WEIGHT", weight
                    .setAttribute "WEIGHT_UNIT_FK", weightUnitFK
                    If mbBarcodesFromFile Then
                        .setAttribute "PLATE_BARCODE", GetProperty("P_BARCODE", rs)
                    Else
                        If (moDictFields("P_BARCODE").SDFileField = "" And moDictFields("P_BARCODE").value <> "") Then
                            .setAttribute "PLATE_BARCODE", moDictFields("P_BARCODE").value
                        Else
                            If optBarcode(0) Then
                                .setAttribute "BARCODE_DESC_ID", cmbBarcodeDesc.ItemData(cmbBarcodeDesc.ListIndex)
                            Else
                                .setAttribute "PLATE_BARCODE", moDictBarcodes(CStr(lPlateCount))
                            End If
                        End If
                    End If
                    .setAttribute "PLATE_NAME", GetProperty("P_PLATENAME", rs) ' kfd
                    .setAttribute "FIELD_1", GetProperty("P_PLATEFIELD1", rs)
                    .setAttribute "FIELD_2", GetProperty("P_PLATEFIELD2", rs)
                    .setAttribute "FIELD_3", GetProperty("P_PLATEFIELD3", rs)
                    .setAttribute "FIELD_4", GetProperty("P_PLATEFIELD4", rs)
                    .setAttribute "FIELD_5", GetProperty("P_PLATEFIELD5", rs)
                    .setAttribute "DATE_1", GetProperty("P_PLATEDATE1", rs)
                    .setAttribute "DATE_2", GetProperty("P_PLATEDATE2", rs)
                    'esuppliercompoundid
                End With        ' With oPlateNode
            End With            ' With XMLPlate
            
            'XMLPlate.save "c:\invloader_debug0.xml"
            ' get well format info for new plate
            Set wellRS = ExecuteStatement(eStmtWellFormats, mlPlateFormat)
            
            ' add row and column nodes based on the plate format
            For j = 1 To Rows
                Set oRowNode = XMLPlate.createElement("ROW")
                oRowNode.setAttribute "ID", j
                oPlateNode.appendChild oRowNode
                For k = 1 To Cols
                    Set oColNode = XMLPlate.createElement("COL")
                    oColNode.setAttribute "ID", k
                    oRowNode.appendChild oColNode
                    ' add well node, fill based on plate format
                    Set oWellNode = XMLPlate.createElement("WELL")
                    
                    wellRS.Filter = adFilterNone
                    wellRS.MoveFirst
                    On Error Resume Next
                    wellRS.Filter = "ROW_INDEX = " & j & " and COL_INDEX = " & k
                                                
                    oWellNode.setAttribute "WELL_FORMAT_ID_FK", wellRS("WELL_FORMAT_ID_FK").value
                    oWellNode.setAttribute "PLATE_FORMAT_ID_FK", wellRS("PLATE_FORMAT_ID_FK").value
                    oWellNode.setAttribute "GRID_POSITION_ID_FK", wellRS("GRID_POSITION_ID").value
                    oWellNode.setAttribute "CONCENTRATION", wellRS("CONCENTRATION").value
                    oWellNode.setAttribute "CONC_UNIT_FK", wellRS("CONC_UNIT_FK").value
                    oColNode.appendChild oWellNode
                Next
            Next
        End If      ' If bNewPlate Then
        
        ' Debug.Print XMLPlate.xml
        ' if we have well coordinates, find the appropriate well
        bLoadCompound = True
        If mbHaveWellCoords Then
            ' if we have well coordinates in one field, use that
            If msWellCoordField = "P_WELLCOORDINATES" Then
                sCoord = ConvertCoord(GetProperty("P_WELLCOORDINATES", rs))
                If sCoord = "INVALID" Then
                    MsgBox "There is a well coordinate error at record " & l & ". Please correct it and run the wizard again.", vbExclamation + vbOKOnly
                    Exit Do
                End If
            Else
                ' if we have row and col, use that
                Dim vRow, vCol
                vRow = GetProperty("P_ROW", rs)
                vCol = GetProperty("P_COL", rs)
                If IsNumeric(vRow) Then
                    ' convert numbers to letters
                    vRow = Chr(vRow + 64)
                End If
                sCoord = Trim(vRow) & "-" & vCol
            End If
            ' now change coordinate string to row and column
            If sCoordinateFormat = "" Then
                If InStr(sCoord, "-") > 0 Then
                    sCoordinateFormat = "Old"
                Else
                    sCoordinateFormat = "New"
                End If
            End If
            If sCoordinateFormat = "Old" Then
                Row = Asc(UCase(Split(sCoord, "-")(0))) - 64
                Col = Split(sCoord, "-")(1)
            ElseIf sCoordinateFormat = "New" Then
                Row = Asc(UCase(Left(sCoord, 1))) - 64
                Col = Right(sCoord, 2)
            End If
        Else
            ' if we don't have well coordinates, move to the next well
            Col = Col + 1
            If Col > Cols Then
                Col = 1
                Row = Row + 1
            End If
            
            ' If we've exceeded the physical plate dimensions, we won't find a well... set the boolean
            ' to true so we create the plate and move on
            If Row > Rows Then
                bNewPlate = True
                bLoadCompound = False
            End If
        End If
        
        If bLoadCompound Then
            ' get correct node
            Set oRowNode = oPlateNode.childNodes(Row - 1)
            Set oColNode = oRowNode.childNodes(Col - 1)
            Set oWellNode = oColNode.childNodes(0)
        End If
        
        ' 1 = compound wells - only put compounds in compound wells IF we don't have
        ' well coordinates
        If bLoadCompound And (oWellNode.getAttribute("WELL_FORMAT_ID_FK") = 1 Or mbHaveWellCoords) Then
             sRegId = ""
             sBase64Cdx = moCFWImporter.Base64CDX(rs)
             l = l + 1
             
             ' update well with new info
             lSolventVolume = 0
             'CSBR 167362 SJ Removed the molar unit
             qtyInitial = GetProperty("P_QTYINIT", rs)
             If qtyInitial <> "" And Not IsEmpty(qtyInitial) Then
                 qtyUnitFK = GetProperty2("P_QTYINITUNIT", rs) 'CSBR 166677 SJ adding initial quantity unit
             End If
             If concentration <> "" And Not IsEmpty(concentration) Then concUnitFK = sUnituM 'uM
             If weight <> "" And Not IsEmpty(weight) Then weightUnitFK = sUnitug 'ug
            
            With oWellNode
                .setAttribute "QTY_INITIAL", qtyInitial
                .setAttribute "QTY_REMAINING", qtyInitial
                .setAttribute "QTY_UNIT_FK", qtyUnitFK
                
                If HasProperty("P_WELLSOLVENT") Then
                    .setAttribute "SOLVENT", GetProperty("P_WELLSOLVENT", rs)
                Else
                    ' Use the plate solvent
                    .setAttribute "SOLVENT", GetProperty("P_SOLVENT", rs)
                End If
                
                If HasProperty("P_WELLSOLVENTVOLUME") Then
                    sVolume = GetProperty("P_WELLSOLVENTVOLUME", rs)
                Else
                    ' Use the plate solvent volume
                    sVolume = GetProperty("P_SOLVENTVOLUME", rs)
                End If
                If sVolume = "" Or IsEmpty(sVolume) Then
                    sVolume = "0"
                End If
                .setAttribute "SOLVENT_VOLUME", sVolume
                
                If HasProperty("P_SOLVENTVOLUMEINITIAL") Then
                    sVolume = GetProperty("P_SOLVENTVOLUMEINITIAL", rs)
                Else
                    ' Use the plate initial solvent volume
                    sVolume = GetProperty("P_SOLVENTVOLUMEINITIAL", rs)
                End If
                If sVolume = "" Or IsEmpty(sVolume) Then
                    sVolume = "0"
                End If
                .setAttribute "SOLVENT_VOLUME_INITIAL", sVolume
                
                If HasProperty("P_WELLSOLVENTVOLUMEUNIT") Then
                    sUnit = GetProperty2("P_WELLSOLVENTVOLUMEUNIT", rs) 'CSBR 167364 SJ To get the unit id of the selected unit in the dropdown
                Else
                    ' Use the plate solvent volume unit
                    sUnit = GetProperty2("P_SOLVENTVOLUMEUNIT", rs)
                End If
                If sUnit = "" Or IsEmpty(sUnit) Then    ' Remove this when all plate units use new dropdowns, kfd2
                    sUnit = sUnituL
                End If
                .setAttribute "SOLVENT_VOLUME_UNIT_ID_FK", sUnit

                
                '-- only update concentration if it is mapped, otherwise it will be set by the plate format
                If concentration <> "" And Not IsEmpty(concentration) Then
                    .setAttribute "CONCENTRATION", GetProperty("P_CONCENTRATION", rs)
                    .setAttribute "CONC_UNIT_FK", concUnitFK
                End If
                .setAttribute "WEIGHT", GetProperty("P_WEIGHT", rs)
                .setAttribute "WEIGHT_UNIT_FK", weightUnitFK
                .setAttribute "FIELD_1", GetProperty("P_WELLFIELD1", rs)
                .setAttribute "FIELD_2", GetProperty("P_WELLFIELD2", rs)
                .setAttribute "FIELD_3", GetProperty("P_WELLFIELD3", rs)
                .setAttribute "FIELD_4", GetProperty("P_WELLFIELD4", rs)
                .setAttribute "FIELD_5", GetProperty("P_WELLFIELD5", rs)
                .setAttribute "DATE_1", GetProperty("P_WELLDATE1", rs)
                .setAttribute "DATE_2", GetProperty("P_WELLDATE2", rs)
                
            End With
            
            If mbRegister Then 'CSBR 165266 SJ For adding the reg attributes to the XML
                'set reg compound options
                If sBase64Cdx = "" Then sBase64Cdx = "No Structure"             ' Special processing for Registration (CSBR-134649)
                Set oNode = XMLPlate.createElement("COMPOUND")
                oRootNode.appendChild oNode
                With oNode   'CSBR 166533 SJ This code change is to change the xml structure so as to handle multiple fragments
                    For Each key In moDictFields
                        Set oField = moDictFields(key)
                        If oField.eFieldSubstClass = eClassRegCompound Then
                            If oField.FieldName <> "" Then
                                For Each vProperty In GetPropertyList(key, rs)
                                    If CStr(vProperty) <> "" Then
                                        Set oNode1 = XMLPlate.createElement(oField.FieldName)
                                        .appendChild oNode1
                                        oNode1.Text = CStr(vProperty)
                                    End If
                                Next vProperty
                            End If
                        End If
                    Next key
                End With
                Set oNode1 = XMLPlate.createElement("structure")
                oNode1.Text = sBase64Cdx
                oNode.appendChild oNode1
                oWellNode.appendChild oNode
            ElseIf Not mbRegister Then   ' add compound node
                If sBase64Cdx <> "" Then
                    Set oNode = XMLPlate.createElement("COMPOUND")
                    oRootNode.appendChild oNode
                    With oNode
                        For Each key In moDictFields
                            Set oField = moDictFields(key)
                            If oField.eFieldSubstClass = eClassInvCompound And _
                              oField.XMLPosition = "ATTRIBUTE" Then
                                If oField.FieldName <> "" And oField.FieldName = "SUBSTANCE_NAME" Then
                                    .setAttribute oField.FieldName, IIf(Len(GetProperty2(key, rs)) > 0, GetProperty2(key, rs), "No Name")
                                ElseIf oField.FieldName <> "" Then
                                    .setAttribute oField.FieldName, GetProperty2(key, rs)
                                End If
                            End If
                        Next key
                    End With
                    oNode.setAttribute "BASE64_CDX", URLEncode(sBase64Cdx)
                    Set oNode1 = XMLPlate.createNode(NODE_ELEMENT, "structure", "")
                    oNode1.Text = sBase64Cdx
                    oNode.appendChild oNode1
                    oWellNode.appendChild oNode
                Else
                    'SMathur added to fix CSBR-58823
                    If GetProperty("P_REG", rs) <> "" And Not IsNull(GetProperty("P_REG", rs)) Then
                        sRegistrationNumber = GetProperty("P_REG", rs)
                        If moRegistrationNumberMapping.Exists(sRegistrationNumber) Then
                            RegID = moRegistrationNumberMapping.Item(sRegistrationNumber)
                        End If
                        oWellNode.setAttribute "REG_ID_FK", RegID
                        oWellNode.setAttribute "BATCH_NUMBER_FK", GetProperty("P_BATCHNUMBER", rs)
                    End If
                     'End of SMathur's Modifications
                End If
                
'                    Dim rsRegID As ADODB.Recordset
'                    'Set rsRegID = ExecuteStatement("SELECT reg_id FROM regdb.reg_numbers WHERE reg_number = '" & GetProperty(ereg, rs) & "'")
'                    Set rsRegID = ExecuteStatement(eStmtRegID, GetProperty(ereg, rs))
'
'                    'SYAN added the if statement to fix CSBR-46069 7/12/2004
'                    If rsRegID.BOF And rsRegID.EOF Then 'no reg number matched
'
'                    Else
'                        oWellNode.setAttribute "REG_ID_FK", rsRegID("reg_id")
'                        oWellNode.setAttribute "BATCH_NUMBER_FK", GetProperty(eBatchNumber, rs)
'                    End If
'                    'End of SYAN modification

                
            End If
            
            rs.MoveNext
            ' update progress indicators
            
            If miLoadOption = ePlates Then progPlate.value = progPlate.value + 1
            progOverall.value = progOverall.value + 1
            progOverall.Refresh
            If miLoadOption = ePlates Then progPlate.Refresh
            lblOverallProg.Caption = "Compound " & progOverall.value - 1 & " of " & mlNumCompounds
            
            'LogAction "progPlate.value = " & progPlate.value & vbLf, xmlLogPath
            'LogAction "progPlate.Max = " & progPlate.Max & vbLf, xmlLogPath
            'LogAction "mlNumCompounds = " & mlNumCompounds & vbLf, xmlLogPath
            'LogAction "mbHavePlateCoords = " & CStr(mbHavePlateCoords) & vbLf, xmlLogPath
            'LogAction "mlPerPlate = " & mlPerPlate & vbLf, xmlLogPath
            'LogAction "l = " & l & vbLf, xmlLogPath
            'LogAction "rs.EOF = " & CStr(rs.EOF) & vbLf, xmlLogPath
            'LogAction "mbHaveWellCoords = " & CStr(mbHaveWellCoords) & vbLf, xmlLogPath
             
            'SYAN added on 7/26/2004 to have better progress bar
            If progPlate.value = progPlate.Max Then
                progPlate.Visible = False
            End If
            'End of SYAN modification
            
            DoEvents
            ' if we have plate coordinates, check to see if we have come across a new plate
            If mbHavePlateCoords Then
                If Not rs.EOF Then
                    If vCurrPlateId <> GetProperty(msPlateCoordField, rs) Then
                        bNewPlate = True
                        vCurrPlateId = GetProperty(msPlateCoordField, rs)
                    End If
                End If
            ' if we're just stepping through, use well count
            ElseIf l > mlPerPlate Then
                bNewPlate = True
            End If
       Else         ' If oWellNode.getAttribute("WELL_FORMAT_ID_FK") = 1 Or mbHaveWellCoords Then
       
       ' ToDo:  This doesn't appear to ever be possible.  The first if statement
       ' has "If oWellNode.getAttribute("WELL_FORMAT_ID_FK") = 1 Or mbHaveWellCoords Then"
       ' If mbHaveWellCoords is true, then the first if will pull it in.  How then will
       ' this section ever get called?
         If mbHaveWellCoords Then
            ' notify the user that they're trying to put a compound
            ' where the well format is not compound
            MsgBox "The chosen plate format does not have the 'Compound' well format at position " & sCoord & _
                    ".  Please correct either the plate format or the input file and run the wizard again.", vbExclamation + vbOKOnly
            Exit Do
          'ElseIf Row = Rows And Col = Cols Then 'check to see if this is an empty well at the end of a plate
          '  bNewPlate = True
         End If
       End If   ' If oWellNode.getAttribute("WELL_FORMAT_ID_FK") = 1 Or mbHaveWellCoords Then
       
       ' if we're at the end of the plate or the recordset, then commit
        If bNewPlate Or rs.EOF Then
            ' tell user we're busy updating
            
            'lblPlateProg.Caption = "Saving to database..."
            'SYAN changed on 7/26/2004 to have better progress report
            lblPlateProg.Caption = "Saving plate " & lPlateCount & " of " & mlNumPlates & " to database. This could take a few minutes..."
            lblOverallProg.Caption = "Creating plate " & lPlateCount & " of " & mlNumPlates & ". This could take a few minutes..."
            'End of SYAN modification
            
            DoEvents
            ' save plates to db
            ' Update plateRS
            ' save new compounds to db
            ' send xml file to API page
            If gbSavePlateXML Then
                XMLPlate.save (xmlLogPath & "DebugPlate_" & lPlateCount & ".xml")
            End If
            
            ' Disable the close button during the upload, as it will not accept a shutdown cleanly
            ' during the asynchronous HTTP post
            modCloseBtn.EnableCloseButton Me.hWnd, False
            sRet = UtilsInventory.SendPlateXML(XMLPlate.xml, mbRegister)
            modCloseBtn.EnableCloseButton Me.hWnd, True
            
            lPlateCount = lPlateCount + 1
            If chkLog.value = 1 Then
                LogAction sRet, xmlLogPath
            End If
            ' Update cpdRs, UDL:=UDL_LOB
            ' save wells to db
            ' Update wellRS
            bUpdated = True
            ' reset progress bar
            progPlate.value = 1
            
            'SYAN added on 7/26/2004 to have better progress bar
            progPlate.Visible = True
            'End of SYAN modification
            
            ' get new plate id
            lGroup = lGroup + 1
            lBarcode = lBarcode + 1
        End If      ' If bNewPlate Or rs.EOF Then
        
        ' if we are just stepping through the wells one by one, then just move next
        ' otherwise we'll find the correct well next time
'        If Not mbHaveWellCoords Then
'            wellRS.MoveNext
'        End If
    Loop    ' Do Until rs.EOF

    Set wellRS = Nothing
    Set rs = Nothing

End Sub


Public Function CheckDate(sValue) As Boolean
    Dim LocaleDate As Date
    On Error GoTo Error
    CheckDate = False
    LocaleDate = Format(sValue, moApplicationVariablesDict("DATE_FORMAT_STRING"))
    CheckDate = True
    Exit Function
Error:
End Function


Private Sub optLoad_Click(Index As Integer)
    'CSBR 135381 SJ
    If Not mbRegAvailable Then
        mbRegAvailable = True
    End If
End Sub

Private Sub optUpdate_Click(Index As Integer)
    frmCompoundUpdateOptions.Visible = (Index <> 0 And mbRegister)
End Sub


