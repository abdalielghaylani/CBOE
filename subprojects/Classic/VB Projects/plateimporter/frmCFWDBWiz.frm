VERSION 5.00
Object = "{D76D7128-4A96-11D3-BD95-D296DC2DD072}#1.0#0"; "Vsflex7.ocx"
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSComCtl.ocx"
Object = "{0AE5E57B-4690-4360-A55F-5C3BC18DB4CC}#1.0#0"; "ActiveWizard.ocx"
Object = "{F9043C88-F6F2-101A-A3C9-08002B2F49FB}#1.2#0"; "ComDLG32.ocx"
Begin VB.Form frmCFWDBWiz 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Inventory Loader"
   ClientHeight    =   5865
   ClientLeft      =   5685
   ClientTop       =   4470
   ClientWidth     =   7425
   Icon            =   "frmCFWDBWiz.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   5865
   ScaleWidth      =   7425
   Begin Active_Wizard.ActivePane ActivePane2 
      Height          =   4605
      Left            =   9840
      TabIndex        =   59
      Top             =   7440
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
         TabIndex        =   112
         Top             =   2400
         Width           =   2535
      End
      Begin VB.TextBox txtServer 
         Height          =   375
         Left            =   2760
         TabIndex        =   61
         Top             =   744
         Width           =   1575
      End
      Begin VB.TextBox txtPassword 
         Height          =   375
         IMEMode         =   3  'DISABLE
         Left            =   2760
         PasswordChar    =   "*"
         TabIndex        =   65
         Top             =   1800
         Width           =   1575
      End
      Begin VB.TextBox txtUserID 
         Height          =   375
         Left            =   2760
         TabIndex        =   64
         Top             =   1272
         Width           =   1575
      End
      Begin VB.Label Label25 
         Caption         =   "Server:"
         Height          =   375
         Left            =   1800
         TabIndex        =   63
         Top             =   744
         Width           =   855
      End
      Begin VB.Label Label24 
         Caption         =   "Password:"
         Height          =   375
         Left            =   1800
         TabIndex        =   62
         Top             =   1800
         Width           =   855
      End
      Begin VB.Label Label23 
         Caption         =   "User ID:"
         Height          =   375
         Left            =   1800
         TabIndex        =   60
         Top             =   1272
         Width           =   855
      End
   End
   Begin Active_Wizard.ActivePane ActivePane3 
      Height          =   4605
      Left            =   8880
      TabIndex        =   9
      Top             =   120
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
         TabIndex        =   82
         Top             =   360
         Width           =   4695
         Begin VB.OptionButton optLoad 
            Caption         =   "Load Structures into Containers"
            Height          =   255
            Index           =   1
            Left            =   240
            TabIndex        =   85
            Top             =   840
            Width           =   4335
         End
         Begin VB.OptionButton optLoad 
            Caption         =   "Load Structures into Plates"
            Height          =   255
            Index           =   0
            Left            =   240
            TabIndex        =   84
            Top             =   360
            Value           =   -1  'True
            Width           =   4335
         End
         Begin VB.OptionButton optLoad 
            Caption         =   "Load Structures Only"
            Height          =   255
            Index           =   2
            Left            =   240
            TabIndex        =   83
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
         TabIndex        =   111
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
   Begin Active_Wizard.ActivePane ActivePane8 
      Height          =   4605
      Left            =   360
      TabIndex        =   47
      Top             =   7320
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
      HeaderTitle     =   "Registration Info"
      HeaderDescription=   "You can register the new compounds in a ChemReg system."
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
      Begin VB.CommandButton cmdRegisterOptions 
         Caption         =   "Registration Options..."
         Height          =   372
         Left            =   4080
         TabIndex        =   74
         Top             =   3480
         Visible         =   0   'False
         Width           =   2412
      End
      Begin VB.CheckBox chkRegister 
         Caption         =   "Register the new compounds in the PerkinElmer Chemical Registration System"
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
         Left            =   1080
         TabIndex        =   48
         Top             =   360
         Width           =   6015
      End
      Begin VSFlex7Ctl.VSFlexGrid grdRegOptions 
         Height          =   2535
         Left            =   600
         TabIndex        =   69
         Top             =   840
         Width           =   5895
         _cx             =   10398
         _cy             =   4471
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
   End
   Begin Active_Wizard.ActivePane ActivePane6 
      Height          =   4605
      Left            =   240
      TabIndex        =   31
      Top             =   6960
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
            TabIndex        =   68
            Text            =   "cmbBarcodeField"
            Top             =   1320
            Width           =   2895
         End
         Begin VB.OptionButton optBarcode 
            Caption         =   "Use mapped field:"
            Height          =   255
            Index           =   3
            Left            =   240
            TabIndex        =   67
            Top             =   1320
            Value           =   -1  'True
            Width           =   3200
         End
         Begin VB.ComboBox cmbBarcodeDesc 
            Height          =   315
            Left            =   2880
            TabIndex        =   66
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
            TabIndex        =   54
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
   Begin Active_Wizard.ActivePane ActivePane4 
      Height          =   4605
      Left            =   10560
      TabIndex        =   13
      Top             =   720
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
      Begin VB.CommandButton cmdSaveChemMappings 
         Caption         =   "Save"
         Height          =   375
         Left            =   1320
         TabIndex        =   81
         Top             =   3840
         Width           =   975
      End
      Begin VB.CommandButton cmdLoadChemMappings 
         Caption         =   "Load"
         Height          =   375
         Left            =   240
         TabIndex        =   80
         Top             =   3840
         Width           =   975
      End
      Begin VB.ComboBox cmbMolTable 
         Height          =   315
         Left            =   2640
         Style           =   2  'Dropdown List
         TabIndex        =   76
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
         TabIndex        =   75
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
   Begin Active_Wizard.ActivePane ActivePane10 
      Height          =   4605
      Left            =   0
      TabIndex        =   70
      Top             =   6600
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
      NextPane        =   10
      PaneOrder       =   9
      PreviousPane    =   8
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
         TabIndex        =   110
         Top             =   2040
         Width           =   2535
      End
      Begin VB.CheckBox chkOpenLog 
         Caption         =   "Open log file when finished"
         Enabled         =   0   'False
         Height          =   255
         Left            =   600
         TabIndex        =   79
         Top             =   1680
         Width           =   3135
      End
      Begin VB.CommandButton btnAdvancedOptions 
         Caption         =   "Advanced Options"
         Height          =   375
         Left            =   600
         TabIndex        =   78
         Top             =   2640
         Width           =   1935
      End
      Begin VB.TextBox txtLogLoc 
         Enabled         =   0   'False
         Height          =   285
         Left            =   600
         TabIndex        =   73
         Text            =   "C:\Program Files\Cambridgesoft\InvLoader\Logs"
         Top             =   960
         Width           =   3735
      End
      Begin VB.CheckBox chkLog 
         Caption         =   "Check1"
         Height          =   255
         Left            =   600
         TabIndex        =   71
         Top             =   600
         Width           =   255
      End
      Begin VB.Label Label29 
         Caption         =   "Note: The file path must already exist."
         Height          =   255
         Left            =   600
         TabIndex        =   77
         Top             =   1320
         Width           =   3015
      End
      Begin VB.Label Label14 
         Caption         =   "Log data to the following location:"
         Height          =   255
         Left            =   840
         TabIndex        =   72
         Top             =   600
         Width           =   3495
      End
   End
   Begin Active_Wizard.ActivePane ActivePane11 
      Height          =   4605
      Left            =   7560
      TabIndex        =   86
      Top             =   5760
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
      HeaderTitle     =   "Container Attributes"
      HeaderDescription=   "Select Values for following container attributes"
      WizardMode      =   1
      NextPane        =   8
      PaneOrder       =   11
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
      Begin VB.ComboBox cmbContainerUOD 
         Height          =   315
         Left            =   4800
         Style           =   2  'Dropdown List
         TabIndex        =   104
         Top             =   2400
         Width           =   1575
      End
      Begin VB.ComboBox cmbContainerUOP 
         Height          =   315
         Left            =   4800
         Style           =   2  'Dropdown List
         TabIndex        =   103
         Top             =   2880
         Width           =   1575
      End
      Begin VB.ComboBox cmbContainerBarcodeDesc 
         Height          =   315
         Left            =   1920
         Style           =   2  'Dropdown List
         TabIndex        =   101
         Top             =   3360
         Width           =   1575
      End
      Begin VB.ComboBox cmbContainerOwner 
         Height          =   315
         Left            =   4800
         Style           =   2  'Dropdown List
         TabIndex        =   99
         Top             =   3360
         Width           =   1575
      End
      Begin VB.ComboBox cmbContainerUOC 
         Height          =   315
         Left            =   1920
         Style           =   2  'Dropdown List
         TabIndex        =   97
         Top             =   2880
         Width           =   1575
      End
      Begin VB.ComboBox cmbContainerUOW 
         Height          =   315
         Left            =   1920
         Style           =   2  'Dropdown List
         TabIndex        =   95
         Top             =   2400
         Width           =   1575
      End
      Begin VB.ComboBox cmbContainerUOM 
         Height          =   315
         Left            =   2640
         Style           =   2  'Dropdown List
         TabIndex        =   93
         Top             =   1680
         Width           =   3135
      End
      Begin VB.ComboBox cmbContainerStatus 
         Height          =   315
         Left            =   2640
         Style           =   2  'Dropdown List
         TabIndex        =   91
         Top             =   1200
         Width           =   3135
      End
      Begin VB.ComboBox cmbContainerType 
         Height          =   315
         Left            =   2640
         Style           =   2  'Dropdown List
         TabIndex        =   89
         Top             =   720
         Width           =   3135
      End
      Begin VB.ComboBox cmbContainerLocations 
         Height          =   315
         Left            =   2640
         Style           =   2  'Dropdown List
         TabIndex        =   87
         Top             =   240
         Width           =   3135
      End
      Begin VB.Label lblNumContainers 
         AutoSize        =   -1  'True
         Caption         =   "Number of Containers:"
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
         Left            =   2040
         TabIndex        =   109
         Top             =   3960
         Width           =   1635
      End
      Begin VB.Label Label26 
         AutoSize        =   -1  'True
         Caption         =   "Number of Containers:"
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
         Left            =   240
         TabIndex        =   108
         Top             =   3960
         Width           =   1635
      End
      Begin VB.Label Label40 
         AutoSize        =   -1  'True
         Caption         =   "Unit of Density:"
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
         Left            =   3600
         TabIndex        =   106
         Top             =   2400
         Width           =   1125
      End
      Begin VB.Label Label39 
         AutoSize        =   -1  'True
         Caption         =   "Unit of Purity:"
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
         Left            =   3720
         TabIndex        =   105
         Top             =   2880
         Width           =   1005
      End
      Begin VB.Label Label38 
         AutoSize        =   -1  'True
         Caption         =   "Barcode Description:"
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
         Left            =   360
         TabIndex        =   102
         Top             =   3360
         Width           =   1485
      End
      Begin VB.Label Label37 
         AutoSize        =   -1  'True
         Caption         =   "Owner:"
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
         Left            =   4185
         TabIndex        =   100
         Top             =   3480
         Width           =   540
      End
      Begin VB.Label Label36 
         AutoSize        =   -1  'True
         Caption         =   "Unit of Concentration:"
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
         Left            =   240
         TabIndex        =   98
         Top             =   2880
         Width           =   1605
      End
      Begin VB.Label Label35 
         AutoSize        =   -1  'True
         Caption         =   "Unit of Weight:"
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
         Left            =   750
         TabIndex        =   96
         Top             =   2400
         Width           =   1095
      End
      Begin VB.Label Label34 
         AutoSize        =   -1  'True
         Caption         =   "Unit of Measure"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H000000C0&
         Height          =   195
         Left            =   1440
         TabIndex        =   94
         Top             =   1680
         Width           =   1140
      End
      Begin VB.Label Label33 
         AutoSize        =   -1  'True
         Caption         =   "Container Status:"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H000000C0&
         Height          =   195
         Left            =   1305
         TabIndex        =   92
         Top             =   1200
         Width           =   1275
      End
      Begin VB.Label Label32 
         AutoSize        =   -1  'True
         Caption         =   "ContainerType:"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H000000C0&
         Height          =   195
         Left            =   1455
         TabIndex        =   90
         Top             =   720
         Width           =   1125
      End
      Begin VB.Label Label31 
         AutoSize        =   -1  'True
         Caption         =   "Container Location:"
         BeginProperty Font 
            Name            =   "Tahoma"
            Size            =   8.25
            Charset         =   0
            Weight          =   400
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H000000C0&
         Height          =   195
         Left            =   1170
         TabIndex        =   88
         Top             =   240
         Width           =   1410
      End
   End
   Begin Active_Wizard.ActivePane ActivePane12 
      Height          =   1335
      Left            =   240
      TabIndex        =   107
      Top             =   360
      Width           =   1815
      _ExtentX        =   3201
      _ExtentY        =   2355
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      NextPane        =   10
      PaneOrder       =   12
      PreviousPane    =   11
      BeginProperty HeaderFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
   End
   Begin Active_Wizard.ActivePane ActivePane5 
      Height          =   4605
      Left            =   8880
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
         TabIndex        =   57
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
         TabIndex        =   58
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
   Begin Active_Wizard.ActivePane ActivePane7 
      Height          =   4605
      Left            =   7800
      TabIndex        =   49
      Top             =   240
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
         TabIndex        =   55
         Text            =   "Combo1"
         Top             =   915
         Width           =   750
      End
      Begin VSFlex7Ctl.VSFlexGrid grdBarcodes 
         Height          =   2415
         Left            =   600
         TabIndex        =   52
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
         TabIndex        =   56
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
         TabIndex        =   53
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
         TabIndex        =   51
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
         TabIndex        =   50
         Top             =   480
         Width           =   1815
      End
   End
   Begin Active_Wizard.ActivePane ActivePane1 
      Height          =   4605
      Left            =   4680
      TabIndex        =   1
      Top             =   7080
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
   Begin Active_Wizard.ActivePane ActivePane9 
      Height          =   4605
      Left            =   600
      TabIndex        =   15
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
      IsLastPane      =   -1  'True
      HeaderTitle     =   "Ready to Import"
      WizardMode      =   1
      NextPane        =   10
      PaneOrder       =   10
      PreviousPane    =   9
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

Private moDictFields As Dictionary
Private moRegDict As Dictionary
Private moCompoundDictFields As Dictionary
Private moContainerDictFields As Dictionary
Private moRegOptionFields As Dictionary
Private moLocationBarcodeMapping As Dictionary
Private moRegistrationNumberMapping As Dictionary


Private msFile As String
Private mlPerPlate As Long
Private mlNumCompounds As Long
Private mlNumPlates As Long
Private mbFinished As Boolean
Private mlPlateType As Long
Private mlPlateFormat As Long
Private miLoadOption As Integer
Private msTest As Integer

Private mlNewLocation As Long
Private mlLibrary As Long

Private moCFWImporter As DataImporter

Private mbShowRegister As Boolean
Private mbRegister As Boolean
Private mbCustomBarcodes As Boolean
Private mbINV_CREATE_PLATE As Boolean
Private mbINV_MANAGE_SUBSTANCES As Boolean
Private mbINV_CREATE_CONTAINER As Boolean

' plate locator information
Private mbHaveWellCoords As Boolean
Private mbHavePlateCoords As Boolean
Private mlWellCoordField As Long
Private mlPlateCoordField As Long
Private mlRowField As Long
Private mlColField As Long
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

Dim oOwner() As OwnerClass

Private Enum LoadOptions
    ePlates = 0
    eContainers
    eStructures
End Enum

Private Enum Panes
    ePaneIntro = 1
    ePaneLogin
    ePaneFile
    ePaneMapping
    ePaneFormat
    ePaneLibrary
    ePaneBarcodes
    ePaneReg
    ePaneLog
    ePaneFinish
    ePaneContainerAttributes
End Enum

Private Enum EnumDBFields
    eWellCoordinates = 1
    eRow
    eCol
    ereg
    eBatchNumber
    eBarcode
    ePlateName
    ename
    eCas
    eacx
    esupplier
    esupplierbarcode
    'CSBR-121010; jbattles 15-Mar-10
    'esuppliercompoundid
    esuppliershipmentdate
    esuppliershipmentcode
    esupplierplatenumber
    emolarinit
    eqtyinit
    eConcentration
    eWeight
    esolvent
    eSolventVolume
    eSolventVolumeInitial
    eSolventVolumeUnit
    eWellSolvent
    eWellSolventVolume
    eWellSolventVolumeInitial
    eWellSolventVolumeUnit
    eCLOGP
    eRotatableBonds
    eTotPolSurfArea
    ehbondacc
    ehbonddon
    ePlateField1
    ePlateField2
    ePlateField3
    ePlateField4
    ePlateField5
    ePlateDate1
    ePlateDate2
    eWellField1
    eWellField2
    eWellField3
    eWellField4
    eWellField5
    eWellDate1
    eWellDate2
    edbfieldcount ' must be last
End Enum

Private Enum EnumCompoundDBFields
    eCCas = 1
    eACXID
    eSubstanceName
    eDensity
    eCcLogP
    eCRotatableBonds
    eCTotPolSurfArea
    eHBondAcceptors
    eHBondDonors
    eAltID1
    eAltID2
    eAltID3
    eAltID4
    eAltID5
    eCreator
    eCompoundDBFieldCount    ' must be last
End Enum

Private Enum EnumRegOptions
    eDupAction = 1
    ePrefix
    eProject
    eStructComment
    'eCompound   CSBR 126433 : sjacob : 06.10.2010
    eNotebook
    eSalt
    eBatchProject
    eScientist
    eRegOptionCount     'must be last
End Enum

Public Enum EnumGridColumns
    eDisplayName = 0
    eDataType
    eMapping
    eValue
End Enum

Private Enum EnumContainerDBFields
    eCompoundIdFk = 1
    eRegNumber
    eBatchNumberFk
    eLocationBarcode
    eContainerName
    eContainerDescription
    eContainerComments
    eContainerTypeIdFk
    eContainerStatusIdFk
    eCBarcode
    eQtyMax
    eQtyInitial
    eUnitOfMeasIdFk
    eQtyMinstock
    eQtyMaxstock
    eContainerSubstanceName
    eContainerCAS
    eContainerACX
    eDateExpires
    ePURITY
    eUnitOfPurityIdFk
    eSolventIdFk
    eCConcentration
    eUnitOfConcIdFk
    eGrade
    eCWeight
    eTareWeight
    eFinalWght
    eNetWght
    eUnitOfWghtIdFk
    eCDensity
    eUnitOfDensityIdFk
    eStorageConditions
    eHandlingProcedures
    eSupplierIdFk
    eSupplierCatnum
    eDateProduced
    eDateOrdered
    eDateReceived
    eLotNum
    eContainerCost
    ePoNumber
    ePoLineNumber
    eReqNumber
    eOwnerIdFk
    eContainerCLOGP
    eContainerRotatableBonds
    eContainerTotPolSurfArea
    eContainerhbondacc
    eContainerhbonddon
    eField1
    eField2
    eField3
    eField4
    eField5
    eField6
    eField7
    eField8
    eField9
    eField10
    eDate1
    eDate2
    eDate3
    eDate4
    eDate5
    eContainerDBFieldCount    ' must be last
End Enum

Public Property Get NoInv() As Boolean
    NoInv = bNoLocations
End Property

Private Function InitField(ByVal Table As String, ByVal Field As String, ByVal Display As String, ByVal FieldType As FieldTypeEnum) As DBField
    Set InitField = New DBField
    InitField.TableName = Table
    InitField.FieldName = Field
    InitField.DisplayName = Display
    InitField.eFieldType = FieldType
End Function

Private Function InitRegField(ByVal DisplayName As String, ByVal AttributeName As String, ByVal PicklistName As String, _
  Optional ByVal DefaultText As String = "", Optional ByVal DefaultValue As String = "") As RegField
    Set InitRegField = New RegField
    InitRegField.DisplayName = DisplayName
    InitRegField.AttributeName = AttributeName
    InitRegField.PicklistName = PicklistName
    InitRegField.DefaultText = DefaultText
    InitRegField.DefaultValue = DefaultValue
    InitRegField.PickListLoaded = False
End Function

Private Sub SetDBFields()
    ' set up dictionary to allow db fields
    Dim i As Long
    Dim tfield As DBField
    Dim rField As RegField
    For i = 1 To edbfieldcount - 1
        Select Case i
            Case eWellCoordinates
                Set tfield = InitField("", "", "Well Coordinates", eText)
            Case eRow
                Set tfield = InitField("", "", "Row", eText)
            Case eCol
                Set tfield = InitField("", "", "Column", eText)
            Case ereg
                Set tfield = InitField("REG_NUMBERS", "REG_NUMBER", "Reg Number", eText)
            Case eBatchNumber
                Set tfield = InitField("BATCHES", "BATCH_NUMBER", "Batch Number", eInteger)
            Case eBarcode
                Set tfield = InitField("INV_PLATES", "PLATE_BARCODE", "Barcode", eText)
            Case ePlateName     ' kfd
                Set tfield = InitField("INV_PLATES", "PLATE_NAME", "Plate Name", eText)
            Case ename
                Set tfield = InitField("INV_COMPOUNDS", "SUBSTANCE_NAME", "Compound Name", eText)
            Case eCas
                Set tfield = InitField("INV_COMPOUNDS", "CAS", "CAS Number", eText)
            Case eacx
                Set tfield = InitField("INV_COMPOUNDS", "ACX_ID", "ChemACX ID", eText)
            Case esupplier
                Set tfield = InitField("INV_PLATES", "SUPPLIER", "Supplier", eText)
            Case esuppliershipmentdate
                Set tfield = InitField("INV_PLATES", "supplier_shipment_date", "Supplier Shipment Date", eDate)
            Case esupplierbarcode
                Set tfield = InitField("INV_PLATES", "supplier_barcode", "Supplier Barcode", eText)
            'CSBR-121010; Jbattles 15-Mar-10
            'Case esuppliercompoundid
            '    Set tfield = InitField("COMPOUND_MOLECULE", "TXT_CMPD_FIELD_4", "Supplier Compound ID", eText)
            Case esuppliershipmentcode
                Set tfield = InitField("INV_PLATES", "supplier_shipment_code", "Supplier Shipment Code", eText)
            Case esupplierplatenumber
                Set tfield = InitField("INV_PLATES", "supplier_shipment_number", "Supplier Plate Number", eInteger)
            Case emolarinit
                Set tfield = InitField("INV_WELLS", "molar_amount", "Initial Quantity (umol)", eReal)
            Case eqtyinit
                Set tfield = InitField("INV_WELLS", "qty_initial", "Initial Quantity (uL)", eReal)
                tfield.value = "0"
            Case eConcentration
                Set tfield = InitField("INV_WELLS", "concentration", "Concentration (uM)", eReal)
            Case eWeight
                Set tfield = InitField("INV_WELLS", "weight", "Weight (ug)", eReal)
            Case esolvent
                ' Note: this field is a text value that will get translated to the proper inv_solvents.solvent_id
                ' within API/CreatePlateXML.asp.  The API will create the solvent_id if it's not already found.
                Set tfield = InitField("INV_PLATES", "solvent", "Plate Solvent", eText)
            Case eSolventVolume
                Set tfield = InitField("INV_PLATES", "solvent_volume", "Plate Solvent Volume", eReal)
            Case eSolventVolumeInitial
                Set tfield = InitField("INV_PLATES", "solvent_volume_initial", "Plate Init. Solvent Vol.", eReal)
            Case eSolventVolumeUnit
                ' Note: this field is a text value that will get translated to the proper inv_unit.unit_id
                ' within API/CreatePlateXML.asp for setting of inv_plates.solvent_volume_unit_id_fk.  This
                ' value will not get created if it does not already exist; null will be inserted instead.
                Set tfield = InitField("INV_PLATES", "SOLVENT_VOLUME_UNIT_NAME", "Plate Solvent Unit", eText)
            Case eWellSolvent
                Set tfield = InitField("INV_WELLS", "solvent", "Well Solvent", eText)
            Case eWellSolventVolume
                Set tfield = InitField("INV_WELLS", "solvent_volume", "Well Solvent Volume", eReal)
            Case eWellSolventVolumeInitial
                Set tfield = InitField("INV_WELLS", "solvent_volume_initial", "Well Init. Solvent Vol.", eReal)
            Case eWellSolventVolumeUnit
                ' Note: this field is a text value that will get translated to the proper inv_unit.unit_id
                ' within API/CreatePlateXML.asp for setting of inv_wells.solvent_volume_unit_id_fk.   This
                ' value will not get created if it does not already exist; null will be inserted instead.
                Set tfield = InitField("INV_WELLS", "SOLVENT_VOLUME_UNIT_NAME", "Well Solvent Unit", eText)
            Case eCLOGP
                Set tfield = InitField("inv_compounds", "clogp", "cLogP", eReal)
            Case eRotatableBonds
                Set tfield = InitField("inv_compounds", "rotatable_bonds", "Rotatable Bonds", eInteger)
            Case eTotPolSurfArea
                Set tfield = InitField("inv_compounds", "tot_pol_surf_area", "Total Polar Surface Area", eReal)
            Case ehbondacc
                Set tfield = InitField("inv_compounds", "hbond_acceptors", "H-bond Acceptors", eInteger)
            Case ehbonddon
                Set tfield = InitField("inv_compounds", "hbond_donors", "H-bond Donors", eInteger)
            Case ePlateField1
                Set tfield = InitField("INV_PLATES", "field_1", "Plate Field 1", eText)
            Case ePlateField2
                Set tfield = InitField("INV_PLATES", "field_2", "Plate Field 2", eText)
            Case ePlateField3
                Set tfield = InitField("INV_PLATES", "field_3", "Plate Field 3", eText)
            Case ePlateField4
                Set tfield = InitField("INV_PLATES", "field_4", "Plate Field 4", eText)
            Case ePlateField5
                Set tfield = InitField("INV_PLATES", "field_5", "Plate Field 5", eText)
            Case ePlateDate1
                Set tfield = InitField("INV_PLATES", "date_1", "Plate Date 1", eDate)
            Case ePlateDate2
                Set tfield = InitField("INV_PLATES", "date_2", "Plate Date 2", eDate)
            Case eWellField1
                Set tfield = InitField("INV_WELLS", "field_1", "Well Field 1", eText)
            Case eWellField2
                Set tfield = InitField("INV_WELLS", "field_2", "Well Field 2", eText)
            Case eWellField3
                Set tfield = InitField("INV_WELLS", "field_3", "Well Field 3", eText)
            Case eWellField4
                Set tfield = InitField("INV_WELLS", "field_4", "Well Field 4", eText)
            Case eWellField5
                Set tfield = InitField("INV_WELLS", "field_5", "Well Field 5", eText)
            Case eWellDate1
                Set tfield = InitField("INV_WELLS", "date_1", "Well Date 1", eDate)
            Case eWellDate2
                Set tfield = InitField("INV_WELLS", "date_2", "Well Date 2", eDate)
            Case Else
        End Select
        moDictFields.Add i, tfield
    Next
    
    For i = 1 To eContainerDBFieldCount - 1
        Select Case i
            Case eCompoundIdFk
                Set tfield = InitField("inv_containers", "compound_id_fk", "Compound ID", eInteger)
            Case eRegNumber
                Set tfield = InitField("inv_containers", "reg_id_fk", "Registration Number", eText)
            Case eBatchNumberFk
                Set tfield = InitField("inv_containers", "batch_number_fk", "Batch Number", eInteger)
            Case eContainerName
                Set tfield = InitField("inv_containers", "container_name", "Container Name", eText)
            Case eContainerDescription
                Set tfield = InitField("inv_containers", "container_description", "Container Description", eText)
            Case eQtyMax
                Set tfield = InitField("inv_containers", "qty_max", "Container Size", eReal)
            Case eQtyInitial
                Set tfield = InitField("inv_containers", "qty_initial", "Initial Quantity", eReal)
            Case eQtyMinstock
                Set tfield = InitField("inv_containers", "qty_minstock", "Min Stock Quantity", eInteger)
            Case eQtyMaxstock
                Set tfield = InitField("inv_containers", "qty_maxstock", "Max Stock Quantity", eInteger)
            Case eLocationBarcode
                Set tfield = InitField("inv_containers", "LOCATION_ID_FK", "Location Barcode", eText)
            Case eDateExpires
                Set tfield = InitField("inv_containers", "date_expires", "Expiration Date", eDate)
            Case eContainerTypeIdFk
                Set tfield = InitField("inv_containers", "container_type_id_fk", "Container Type", eText)
            Case ePURITY
                Set tfield = InitField("inv_containers", "purity", "Purity", eReal)
            Case eSolventIdFk
                Set tfield = InitField("inv_containers", "solvent_id_fk", "Solvent ID", eText)
            Case eCConcentration
                Set tfield = InitField("inv_containers", "concentration", "Concentration", eReal)
            Case eUnitOfMeasIdFk
                Set tfield = InitField("inv_containers", "unit_of_meas_id_fk", "Unit Of Measure", eText)
            Case eUnitOfWghtIdFk
                Set tfield = InitField("inv_containers", "unit_of_wght_id_fk", "Unit Of Weight", eText)
            Case eUnitOfConcIdFk
                Set tfield = InitField("inv_containers", "unit_of_conc_id_fk", "Unit Of Conc.", eText)
            Case eGrade
                Set tfield = InitField("inv_containers", "grade", "Grade", eText)
            Case eCWeight
                Set tfield = InitField("inv_containers", "weight", "Weight", eText)
            Case eUnitOfPurityIdFk
                Set tfield = InitField("inv_containers", "unit_of_purity_id_fk", "Unit Of Purity", eText)
            Case eTareWeight
                Set tfield = InitField("inv_containers", "tare_weight", "Tare Weight", eReal)
            Case eOwnerIdFk
                Set tfield = InitField("inv_containers", "owner_id_fk", "Owner ID", eText)
            Case eContainerComments
                Set tfield = InitField("inv_containers", "container_comments", "Container Comments", eText)
            Case eStorageConditions
                Set tfield = InitField("inv_containers", "storage_conditions", "Storage Conditions", eText)
            Case eHandlingProcedures
                Set tfield = InitField("inv_containers", "handling_procedures", "Handling Procedures", eText)
            Case eDateOrdered
                Set tfield = InitField("inv_containers", "date_ordered", "Date Ordered", eDate)
            Case eDateReceived
                Set tfield = InitField("inv_containers", "date_received", "Date Received", eDate)
            Case eLotNum
                Set tfield = InitField("inv_containers", "lot_num", "Lot Num", eInteger)
            Case eContainerStatusIdFk
                Set tfield = InitField("inv_containers", "container_status_id_fk", "Container Status", eText)
            Case eFinalWght
                Set tfield = InitField("inv_containers", "final_wght", "Final Weight", eReal)
            Case eNetWght
                Set tfield = InitField("inv_containers", "net_wght", "Net Weight", eReal)
            Case eSupplierIdFk
                Set tfield = InitField("inv_containers", "supplier_id_fk", "Supplier", eText)
            Case eSupplierCatnum
                Set tfield = InitField("inv_containers", "supplier_catnum", "Supplier Catalog Num", eText)
            Case eDateProduced
                Set tfield = InitField("inv_containers", "date_produced", "Date Produced", eDate)
            Case eContainerCost
                Set tfield = InitField("inv_containers", "container_cost", "Container Cost", eReal)
            Case eCBarcode
                Set tfield = InitField("inv_containers", "barcode", "Barcode", eText)
            Case ePoNumber
                Set tfield = InitField("inv_containers", "po_number", "PO Number", eInteger)
            Case eReqNumber
                Set tfield = InitField("inv_containers", "req_number", "Req Number", eInteger)
            Case eCDensity
                Set tfield = InitField("inv_containers", "density", "Density", eReal)
            Case eUnitOfDensityIdFk
                Set tfield = InitField("inv_containers", "unit_of_density_id_fk", "Unit Of Density", eText)
            Case ePoLineNumber
                Set tfield = InitField("inv_containers", "po_line_number", "PO Line Number", eInteger)
            Case eContainerSubstanceName
                Set tfield = InitField("INV_COMPOUNDS", "SUBSTANCE_NAME", "Compound Name", eText)
            Case eContainerCAS
                Set tfield = InitField("INV_COMPOUNDS", "CAS", "CAS Number", eText)
            Case eContainerACX
                Set tfield = InitField("INV_COMPOUNDS", "ACX_ID", "ChemACX ID", eText)
            Case eContainerCLOGP
                Set tfield = InitField("inv_compounds", "clogp", "cLogP", eReal)
            Case eContainerRotatableBonds
                Set tfield = InitField("inv_compounds", "rotatable_bonds", "Rotatable Bonds", eInteger)
            Case eContainerTotPolSurfArea
                Set tfield = InitField("inv_compounds", "tot_pol_surf_area", "Total Polar Surface Area", eReal)
            Case eContainerhbondacc
                Set tfield = InitField("inv_compounds", "hbond_acceptors", "H-bond Acceptors", eInteger)
            Case eContainerhbonddon
                Set tfield = InitField("inv_compounds", "hbond_donors", "H-bond Donors", eInteger)
            Case eField1
                Set tfield = InitField("inv_containers", "field_1", "Field 1", eText)
            Case eField2
                Set tfield = InitField("inv_containers", "field_2", "Field 2", eText)
            Case eField3
                Set tfield = InitField("inv_containers", "field_3", "Field 3", eText)
            Case eField4
                Set tfield = InitField("inv_containers", "field_4", "Field 4", eText)
            Case eField5
                Set tfield = InitField("inv_containers", "field_5", "Field 5", eText)
            Case eField6
                Set tfield = InitField("inv_containers", "field_6", "Field 6", eText)
            Case eField7
                Set tfield = InitField("inv_containers", "field_7", "Field 7", eText)
            Case eField8
                Set tfield = InitField("inv_containers", "field_8", "Field 8", eText)
            Case eField9
                Set tfield = InitField("inv_containers", "field_9", "Field 9", eText)
            Case eField10
                Set tfield = InitField("inv_containers", "field_10", "Field 10", eText)
            Case eDate1
                Set tfield = InitField("inv_containers", "date_1", "Date 1", eDate)
            Case eDate2
                Set tfield = InitField("inv_containers", "date_2", "Date 2", eDate)
            Case eDate3
                Set tfield = InitField("inv_containers", "date_3", "Date 3", eDate)
            Case eDate4
                Set tfield = InitField("inv_containers", "date_4", "Date 4", eDate)
            Case eDate5
                Set tfield = InitField("inv_containers", "date_5", "Date 5", eDate)
            Case Else
        End Select
    moContainerDictFields.Add i, tfield
    Next
    
    For i = 1 To eCompoundDBFieldCount - 1
        Select Case i
            Case eCCas
                Set tfield = InitField("INV_COMPOUNDS", "CAS", "CAS", eText)
            Case eACXID
                Set tfield = InitField("INV_COMPOUNDS", "ACX_ID", "ACX ID", eText)
            Case eSubstanceName
                Set tfield = InitField("INV_COMPOUNDS", "SUBSTANCE_NAME", "Substance Name", eText)
            Case eDensity
                Set tfield = InitField("INV_COMPOUNDS", "DENSITY", "Density", eReal)
            Case eCcLogP
                Set tfield = InitField("INV_COMPOUNDS", "CLOGP", "cLogP", eReal)
            Case eCRotatableBonds
                Set tfield = InitField("INV_COMPOUNDS", "ROTATABLE_BONDS", "Rotatable Bonds", eInteger)
            Case eCTotPolSurfArea
                Set tfield = InitField("INV_COMPOUNDS", "TOT_POL_SURF_AREA", "Total Polar Surface Area", eReal)
            Case eHBondAcceptors
                Set tfield = InitField("INV_COMPOUNDS", "HBOND_ACCEPTORS", "H-bond Acceptors", eInteger)
            Case eHBondDonors
                Set tfield = InitField("INV_COMPOUNDS", "HBOND_DONORS", "H-bond Donors", eInteger)
            Case eAltID1
                Set tfield = InitField("INV_COMPOUNDS", "ALT_ID_1", "Alternative ID1", eText)
            Case eAltID2
                Set tfield = InitField("INV_COMPOUNDS", "ALT_ID_2", "Alternative ID2", eText)
            Case eAltID3
                Set tfield = InitField("INV_COMPOUNDS", "ALT_ID_3", "Alternative ID3", eText)
            Case eAltID4
                Set tfield = InitField("INV_COMPOUNDS", "ALT_ID_4", "Alternative ID4", eText)
            Case eAltID5
                Set tfield = InitField("INV_COMPOUNDS", "ALT_ID_5", "Alternative ID5", eText)
            Case eCreator
                Set tfield = InitField("INV_COMPOUNDS", "CREATOR", "Creator", eText)
            Case Else
        End Select
        moCompoundDictFields.Add i, tfield
    Next
    
    For i = 1 To eRegOptionCount - 1
        Select Case i
            Case eDupAction
                Set rField = InitRegField("Duplicate Action", "REGPARAMETER", "", "Add New Batch to Registered Compound", "NEW_BATCH")
                'CSBR ID : 124023 : sjacob
                'Comment : Removed the 'Add duplicate to temp table' from the dropdown list
                rField.Picklist = GridBuildComboList2("NEW_BATCH;OVERRIDE;UNIQUE_DEL_TEMP", _
                                                      "Add New Batch to Registered Compound;Add New Compound;Ignore Compound")
                rField.PickListLoaded = True
            Case ePrefix
                Set rField = InitRegField("Prefix", "SEQUENCE", "PREFIX")
            'CSBR ID : 124898 : sjacob
            'Comment : changed PROJECT to PROJECTID
            Case eProject
                Set rField = InitRegField("Project", "PROJECTID", "PROJECT")
            Case eStructComment
                Set rField = InitRegField("Struct/Stereochem Comment", "", "")
'             CSBR 126433 : sjacob : 06/10/2010
'            Start of code change
'            Case eCompound
'                Set rField = InitRegField("Compound Type", "COMPOUNDTYPE", "COMPOUNDTYPE")
'            End of code change
            Case eNotebook
                Set rField = InitRegField("Notebook", "NOTEBOOK", "NOTEBOOK")
            Case eSalt
                Set rField = InitRegField("Salt", "SALT", "SALT")
            Case eBatchProject
                Set rField = InitRegField("Batch Project", "BATCH_PROJECT", "PROJECT")
            'CSBR ID : 124898 : sjacob
            'Comment : changed SCIENTIST_ID to SCIENTIST_ID
            Case eScientist
                Set rField = InitRegField("Scientist", "SCIENTIST_ID", "ACTIVEPERSON")
            Case Else
        End Select
        moRegOptionFields.Add i, rField
    Next
End Sub

Public Sub Initialize()
    Dim oDict As Dictionary, oDict2 As Dictionary, oDict3 As Dictionary, oDict4 As Dictionary, oDict5 As Dictionary, oDict6 As Dictionary, oDict7 As Dictionary, oDict8 As Dictionary, oDict9 As Dictionary, oDict10 As Dictionary, oDict11 As Dictionary
    Dim response As String
    Dim aValuePairs, aVariable
    Dim i
    
    On Error GoTo Error
    bNoLocations = False
    bNoFormats = False
        
    Set moCFWImporter = New DataImporter
    lblChemOffice.Caption = moCFWImporter.msChemOffice
    
    'Set oDict = DictionaryFromRecordset(ExecuteStatement("select location_id, location_name from inv_vw_plate_locations where location_id not in (select location_id from inv_vw_plate_grid_locations)"), "location_id", "location_name")
    Set oDict = DictionaryFromRecordset(ExecuteStatement(eStmtPlateLocations), "location_id", "location_name")
    Set oDict2 = DictionaryFromRecordset(ExecuteStatement(eStmtPlateFormats), 0, 0)
    Set oDict3 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerLocations), "location_id", "location_name_barcode")
    Set oDict4 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerType), "CONTAINER_TYPE_ID", "CONTAINER_TYPE_NAME")
    Set oDict5 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerStatus), "CONTAINER_STATUS_ID", "CONTAINER_STATUS_NAME")
    Set oDict6 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerOwner), "value", "DisplayText")
    'UOM- Unit_type_ID=1
    Set oDict7 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerUnits, "1, 2, 4"), "value", "DisplayText")
    'UOW- Unit_type_ID=2
    Set oDict8 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerUnits, 2), "value", "DisplayText")
    'UOD -Unit_type_ID = 6
    Set oDict9 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerUnits, 6), "value", "DisplayText")
    'UOC -Unit_type_ID = 3
    Set oDict10 = DictionaryFromRecordset(ExecuteStatement(eStmtContainerUnits, 3), "value", "DisplayText")
    Set oDict11 = DictionaryFromRecordset(ExecuteStatement(eStmtBarcodeDescs), "barcode_desc_id", "barcode_desc_name")
    
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
        
        'update display names
        UpdateDisplayName "INV_COMPOUNDS", HTTPRequest("POST", msServerName, mbUseSSL, "cheminv/api/GetConfigInfo.asp", "", "ConfigKey=alt_ids")
        UpdateDisplayName "INV_PLATES", HTTPRequest("POST", msServerName, mbUseSSL, "cheminv/api/GetConfigInfo.asp", "", "ConfigKey=custom_plate_fields")
        UpdateDisplayName "INV_WELLS", HTTPRequest("POST", msServerName, mbUseSSL, "cheminv/api/GetConfigInfo.asp", "", "ConfigKey=custom_well_fields")
    End If
    
    ComboFill cmbContainerLocations, oDict3
    ComboFill cmbContainerType, oDict4
    ComboFill cmbContainerStatus, oDict5
    FillOwnerCombo cmbContainerOwner, oDict6
    ' ComboFill cmbContainerOwner, oDict6, False
    ComboFill cmbContainerUOM, oDict7
    ComboFill cmbContainerUOW, oDict8, False
    ComboFill cmbContainerUOD, oDict9, False
    ComboFill cmbContainerUOC, oDict10, False
    ComboFill cmbContainerUOP, oDict10, False
    ComboFill cmbContainerBarcodeDesc, oDict11, False
    
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
    Err.Raise vbError + 1, , "Failed to intitialize, possible connection failure: ", Err.Description
End Sub

Private Sub UpdateDisplayName(TableName As String, ConfigString As String)
    Dim fieldDict, reqFieldDict As Dictionary
    Dim vkey
    
    If ConfigString <> "" Then
        Set fieldDict = New Dictionary
        Set reqFieldDict = New Dictionary
        ConfigStringToDictionaries ConfigString, fieldDict, reqFieldDict
        For Each vkey In moDictFields
            If TypeName(moDictFields(vkey)) = "DBField" Then
                If moDictFields(vkey).TableName = TableName Then
                    If fieldDict.Exists(LCase(moDictFields(vkey).FieldName)) Then moDictFields(vkey).DisplayName = fieldDict(LCase(moDictFields(vkey).FieldName))
                End If
            End If
        Next
    End If
End Sub
Private Sub ConfigStringToDictionaries(ConfigString, ByRef fieldDict, ByRef reqFieldDict)
    Dim arrField, arrReqfield As Variant
    Dim bRequired As Boolean
    Dim tempStr, fName, fText, afterSemi As String
    Dim tempLen, colonPos, semiPos, i As Integer
    
    arrField = Split(ConfigString, ",")
    For i = 0 To UBound(arrField)
        bRequired = False
        tempStr = arrField(i)
        tempLen = Len(tempStr)
        colonPos = InStr(1, tempStr, ":")
        semiPos = InStr(1, tempStr, ";")
        fName = Left(tempStr, colonPos - 1)
        fText = Mid(tempStr, colonPos + 1, semiPos - colonPos - 1)
        afterSemi = Mid(tempStr, semiPos + 1, Len(tempStr) - semiPos)
        If InStr(1, afterSemi, "1") > 0 Then bRequired = True
        'lower case the key so I can match it up
        fieldDict.Add LCase(Trim(fName)), Trim(fText)
        If bRequired Then reqFieldDict.Add Trim(fName), Trim(fText)
    Next
    
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
    
    ' run the import
    Dim l As Long, lCurrPlateId As Long
    Dim lPlateCount As Long: lPlateCount = 1
    Dim rs As Recordset
    Dim cpdRs As Recordset
    Dim wellRS As Recordset
    Dim plateRS As Recordset
    Dim gridRS As Recordset
    Dim lGroup As Long, lBarcode As Long, sRegId As String
    Dim vCurrPlateId As Variant ' current plate
    Dim Rows As Long, Cols As Long, Row As Long, Col As Long
    Dim sCoord As String
    Dim lSolutionVolume As Long, lSolventVolume As Long
    Dim sVolume As String, sUnit As String
    Dim sUnituM As String, sUnituL As String, sUnitumol As String, sUnitug As String
    Dim sSolventVolume As String
    vCurrPlateId = NULL_AS_LONG
    
    Dim objPI As IXMLDOMProcessingInstruction
    Dim oRootNode As IXMLDOMElement
    Dim oNode As IXMLDOMElement
    Dim oNode1 As IXMLDOMElement
    
    sUnituM = UtilsInventory.LookUpValue("inv_units", "uM")
    sUnituL = UtilsInventory.LookUpValue("inv_units", "ul")
    sUnitumol = UtilsInventory.LookUpValue("inv_units", "umol")
    sUnitug = UtilsInventory.LookUpValue("inv_units", "ug")
    
    If chkLog.value = 1 Then
        LogAction "<INV_LOADER TIME_STAMP=""" & Now & """>", xmlLogPath
    End If

    ' set up progress report
    lblNotify.Visible = False
    frmOverallProg.Visible = True
    progOverall.Max = mlNumCompounds + 1
    progOverall.Min = 1
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
    
    If miLoadOption = eContainers Then
        progOverall.value = 1
        Dim ContainerXML
        Call CreateContainerXML
    ElseIf miLoadOption = ePlates Then
        ' get the recordset from the importer.  Sort on the plate field so that
        ' our records are grouped by plate. Use barcode first, then supplierplate number, then
        ' supplier barcode
        Dim lPlateFld As Long
        If moDictFields(eBarcode).SDFileField = "" Then
            If moDictFields(esupplierplatenumber).SDFileField = "" Then
                lPlateFld = esupplierbarcode
            Else
                lPlateFld = esupplierplatenumber
            End If
        Else
            lPlateFld = eBarcode
        End If
        ' if well field, use that, otherwise use row and col fields
        If moDictFields(eWellCoordinates).SDFileField = "" Then
            Set rs = moCFWImporter.Recordset(CnvLong(moDictFields(lPlateFld).SDFileField, eDBtoVB), _
                                            CnvLong(moDictFields(eRow).SDFileField, eDBtoVB), _
                                            CnvLong(moDictFields(eCol).SDFileField, eDBtoVB))
        Else
            Set rs = moCFWImporter.Recordset(CnvLong(moDictFields(lPlateFld).SDFileField, eDBtoVB), _
                                            CnvLong(moDictFields(eWellCoordinates).SDFileField, eDBtoVB), _
                                            NULL_AS_LONG)
        End If
        
        'rs.save "c:\test.xml", adPersistXML
        If mbHavePlateCoords Then
            vCurrPlateId = GetProperty(mlPlateCoordField, rs)
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
                Set XMLPlate = New DOMDocument60
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
                    If mbRegister Then
                    'set reg compound options
                        With oRootNode
                            For i = 1 To eRegOptionCount - 1
                                If moRegOptionFields(i).AttributeName <> "" Then
                                    .setAttribute moRegOptionFields(i).AttributeName, moRegOptionFields(i).value
                                End If
                            Next i
'                            .setAttribute "REGPARAMETER", moRegOptionFields(eDupAction).value
'                            .setAttribute "SEQUENCE", moRegOptionFields(ePrefix).value
'                            .setAttribute "PROJECT", moRegOptionFields(eProject).value
'                            .setAttribute "COMPOUND", moRegOptionFields(eCompound).value
'                            .setAttribute "NOTEBOOK", moRegOptionFields(eNotebook).value
'                            .setAttribute "SALT", moRegOptionFields(eSalt).value
'                            .setAttribute "BATCH_PROJECT", moRegOptionFields(eBatchProject).value
'                            .setAttribute "SCIENTIST", moRegOptionFields(eScientist).value
                        End With
                    End If
                    Set .documentElement = oRootNode
                    Set oPlateNode = XMLPlate.createElement("PLATE")
                    oRootNode.appendChild oPlateNode
                    With oPlateNode
                        .setAttribute "PLATE_TYPE_ID_FK", mlPlateType
                        .setAttribute "PLATE_FORMAT_ID_FK", mlPlateFormat
                        .setAttribute "STATUS_ID_FK", 5
                        .setAttribute "LOCATION_ID_FK", mlNewLocation
                        .setAttribute "LIBRARY_ID_FK", mlLibrary
                        .setAttribute "SUPPLIER", GetProperty(esupplier, rs)
                        If GetProperty(esuppliershipmentdate, rs) <> "" And Not IsEmpty(GetProperty(esuppliershipmentdate, rs)) Then .setAttribute "SUPPLIER_SHIPMENT_DATE", GetProperty(esuppliershipmentdate, rs)
                        .setAttribute "SUPPLIER_SHIPMENT_CODE", GetProperty(esuppliershipmentcode, rs)
                        .setAttribute "SUPPLIER_SHIPMENT_NUMBER", GetProperty(esupplierplatenumber, rs)
                        .setAttribute "SUPPLIER_BARCODE", GetProperty(esupplierbarcode, rs)
                        .setAttribute "GROUP_NAME", moDictGroups(CStr(lPlateCount))
                        'initial quantity is either the molar amount or the volume amount, if both then molar is chosen
                        qtyInitial = GetProperty(emolarinit, rs)
                        If qtyInitial = "" Or IsEmpty(qtyInitial) Then
                            qtyInitial = GetProperty(eqtyinit, rs)
                            If qtyInitial <> "" And Not IsEmpty(qtyInitial) Then
                                qtyUnitFK = sUnituL 'uL
                            End If
                        Else
                            qtyUnitFK = sUnitumol 'umol
                        End If
                        .setAttribute "QTY_INITIAL", qtyInitial
                        .setAttribute "QTY_REMAINING", qtyInitial
                        .setAttribute "QTY_UNIT_FK", qtyUnitFK
                        .setAttribute "SOLVENT", GetProperty(esolvent, rs)
                        
                        sSolventVolume = GetProperty(eSolventVolume, rs)
                        If sSolventVolume = "" Or IsEmpty(sSolventVolume) Then sSolventVolume = "0"
                        .setAttribute "SOLVENT_VOLUME", sSolventVolume
                        
                        sSolventVolume = GetProperty(eSolventVolumeInitial, rs)
                        If sSolventVolume = "" Or IsEmpty(sSolventVolume) Then sSolventVolume = "0"
                        .setAttribute "SOLVENT_VOLUME_INITIAL", sSolventVolume
                        
                        sUnit = GetProperty(eSolventVolumeUnit, rs)
                        If sUnit = "" Or IsEmpty(sUnit) Then sUnit = "ul"
                        .setAttribute "SOLVENT_VOLUME_UNIT_NAME", sUnit
                        
                        concentration = GetProperty(eConcentration, rs)
                        If concentration <> "" And Not IsEmpty(concentration) Then concUnitFK = sUnituM 'uM
                        .setAttribute "CONCENTRATION", concentration
                        .setAttribute "CONC_UNIT_FK", concUnitFK
                        weight = GetProperty(eWeight, rs)
                        If weight <> "" And Not IsEmpty(weight) Then weightUnitFK = sUnitug 'ug
                        .setAttribute "WEIGHT", weight
                        .setAttribute "WEIGHT_UNIT_FK", weightUnitFK
                        If mbBarcodesFromFile Then
                            .setAttribute "PLATE_BARCODE", GetProperty(eBarcode, rs)
                        Else
                            If (moDictFields(eBarcode).UseDefault And moDictFields(eBarcode).value <> "") Then
                                .setAttribute "PLATE_BARCODE", moDictFields(eBarcode).value
                            Else
                                If optBarcode(0) Then
                                    .setAttribute "BARCODE_DESC_ID", cmbBarcodeDesc.ItemData(cmbBarcodeDesc.ListIndex)
                                Else
                                    .setAttribute "PLATE_BARCODE", moDictBarcodes(CStr(lPlateCount))
                                End If
                            End If
                        End If
                        .setAttribute "PLATE_NAME", GetProperty(ePlateName, rs) ' kfd
                        .setAttribute "FIELD_1", GetProperty(ePlateField1, rs)
                        .setAttribute "FIELD_2", GetProperty(ePlateField2, rs)
                        .setAttribute "FIELD_3", GetProperty(ePlateField3, rs)
                        .setAttribute "FIELD_4", GetProperty(ePlateField4, rs)
                        .setAttribute "FIELD_5", GetProperty(ePlateField5, rs)
                        .setAttribute "DATE_1", GetProperty(ePlateDate1, rs)
                        .setAttribute "DATE_2", GetProperty(ePlateDate2, rs)
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
                If mlWellCoordField = eWellCoordinates Then
                    sCoord = ConvertCoord(GetProperty(eWellCoordinates, rs))
                    If sCoord = "INVALID" Then
                        MsgBox "There is a well coordinate error at record " & l & ". Please correct it and run the wizard again.", vbExclamation + vbOKOnly
                        Exit Do
                    End If
                Else
                    ' if we have row and col, use that
                    Dim vRow, vCol
                    vRow = GetProperty(eRow, rs)
                    vCol = GetProperty(eCol, rs)
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
                 qtyInitial = GetProperty(emolarinit, rs)
                 If qtyInitial = "" Or IsEmpty(qtyInitial) Then
                     qtyInitial = GetProperty(eqtyinit, rs)
                     If qtyInitial <> "" And Not IsEmpty(qtyInitial) Then
                         qtyUnitFK = sUnituL 'uL
                     End If
                 Else
                     qtyUnitFK = sUnitumol 'umol
                 End If
                 If concentration <> "" And Not IsEmpty(concentration) Then concUnitFK = sUnituM 'uM
                 If weight <> "" And Not IsEmpty(weight) Then weightUnitFK = sUnitug 'ug
                
                With oWellNode
                    .setAttribute "QTY_INITIAL", qtyInitial
                    .setAttribute "QTY_REMAINING", qtyInitial
                    .setAttribute "QTY_UNIT_FK", qtyUnitFK
                    
                    If HasProperty(eWellSolvent) Then
                        .setAttribute "SOLVENT", GetProperty(eWellSolvent, rs)
                    Else
                        ' Use the plate solvent
                        .setAttribute "SOLVENT", GetProperty(esolvent, rs)
                    End If
                    
                    If HasProperty(eWellSolventVolume) Then
                        sVolume = GetProperty(eWellSolventVolume, rs)
                    Else
                        ' Use the plate solvent volume
                        sVolume = GetProperty(eSolventVolume, rs)
                    End If
                    If sVolume = "" Or IsEmpty(sVolume) Then
                        sVolume = "0"
                    End If
                    .setAttribute "SOLVENT_VOLUME", sVolume
                    
                    If HasProperty(eWellSolventVolumeInitial) Then
                        sVolume = GetProperty(eWellSolventVolumeInitial, rs)
                    Else
                        ' Use the plate initial solvent volume
                        sVolume = GetProperty(eSolventVolumeInitial, rs)
                    End If
                    If sVolume = "" Or IsEmpty(sVolume) Then
                        sVolume = "0"
                    End If
                    .setAttribute "SOLVENT_VOLUME_INITIAL", sVolume
                    
                    If HasProperty(eWellSolventVolumeUnit) Then
                        sUnit = GetProperty(eWellSolventVolumeUnit, rs)
                    Else
                        ' Use the plate solvent volume unit
                        sUnit = GetProperty(eSolventVolumeUnit, rs)
                    End If
                    If sUnit = "" Or IsEmpty(sUnit) Then
                        sUnit = "ul"
                    End If
                    .setAttribute "SOLVENT_VOLUME_UNIT_NAME", sUnit
                    
                    '-- only update concentration if it is mapped, otherwise it will be set by the plate format
                    If concentration <> "" And Not IsEmpty(concentration) Then
                        .setAttribute "CONCENTRATION", GetProperty(eConcentration, rs)
                        .setAttribute "CONC_UNIT_FK", concUnitFK
                    End If
                    .setAttribute "WEIGHT", GetProperty(eWeight, rs)
                    .setAttribute "WEIGHT_UNIT_FK", weightUnitFK
                    .setAttribute "FIELD_1", GetProperty(eWellField1, rs)
                    .setAttribute "FIELD_2", GetProperty(eWellField2, rs)
                    .setAttribute "FIELD_3", GetProperty(eWellField3, rs)
                    .setAttribute "FIELD_4", GetProperty(eWellField4, rs)
                    .setAttribute "FIELD_5", GetProperty(eWellField5, rs)
                    .setAttribute "DATE_1", GetProperty(eWellDate1, rs)
                    .setAttribute "DATE_2", GetProperty(eWellDate2, rs)
                    
                End With
                
                If mbShowRegister Then
                    ' add compound node
                    If sBase64Cdx <> "" Then
                    Set oNode = XMLPlate.createElement("COMPOUND")
                    With oNode
                        .setAttribute "SUBSTANCE_NAME", IIf(Len(GetProperty(ename, rs)) > 0, GetProperty(ename, rs), "No Name")
                        .setAttribute "CAS", GetProperty(eCas, rs)
                        .setAttribute "ACX_ID", GetProperty(eacx, rs)
                        If sBase64Cdx <> "" Then
                            .setAttribute "BASE64_CDX", URLEncode(sBase64Cdx)
                        End If
                        .setAttribute "CLOGP", GetProperty(eCLOGP, rs)
                        .setAttribute "ROTATABLE_BONDS", GetProperty(eRotatableBonds, rs)
                        .setAttribute "TOT_POL_SURF_AREA", GetProperty(eTotPolSurfArea, rs)
                        .setAttribute "HBOND_ACCEPTORS", GetProperty(ehbondacc, rs)
                        .setAttribute "HBOND_DONORS", GetProperty(ehbonddon, rs)
                        '!supplier_id = GetProperty(esuppliercompoundid, rs)
                        ' fill in reg options
                        Dim vkey
                        If Not moRegDict Is Nothing Then
                            For Each vkey In moRegDict
                                If TypeName(moRegDict(vkey)) = "DBField" Then
                                    ' only add tag if there is a default other than "" ,or
                                    ' if we are using an SDFile field
                                    If (moRegDict(vkey).UseDefault And moRegDict(vkey).value <> "") _
                                        Or moRegDict(vkey).UseDefault = False Then
                                        .setAttribute moRegDict(vkey).FieldName, GetRegProperty(vkey, rs)
                                    End If
                                End If
                            Next
                        End If
                    End With
                    'kfd - add structure element without urlencoding
                    Set oNode1 = XMLPlate.createNode(NODE_ELEMENT, "structure", "")
                    oNode1.Text = sBase64Cdx
                    oNode.appendChild oNode1

                    oWellNode.appendChild oNode
                    End If
                Else
                
                    'SMathur added to fix CSBR-58823
                    Dim RegID, sRegistrationNumber
                    If GetProperty(ereg, rs) <> "" And Not IsNull(GetProperty(ereg, rs)) Then
                        sRegistrationNumber = GetProperty(ereg, rs)
                        If moRegistrationNumberMapping.Exists(sRegistrationNumber) Then
                            RegID = moRegistrationNumberMapping.Item(sRegistrationNumber)
                        End If
                        oWellNode.setAttribute "REG_ID_FK", RegID
                        oWellNode.setAttribute "BATCH_NUMBER_FK", GetProperty(eBatchNumber, rs)
                    End If
                    'End of SMathur's Modifications
                    
                    
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

                    
                End If      ' If mbShowRegister Then
                
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
                        If vCurrPlateId <> GetProperty(mlPlateCoordField, rs) Then
                            bNewPlate = True
                            vCurrPlateId = GetProperty(mlPlateCoordField, rs)
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
                sRet = UtilsInventory.CreatePlateXML(XMLPlate.xml, mbRegister)
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
    ElseIf miLoadOption = eStructures Then
        Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
        Dim XMLCompounds As DOMDocument
        Set XMLCompounds = New DOMDocument60
        With XMLCompounds
            Set objPI = .createProcessingInstruction("xml", "version=""1.0""")
            .appendChild objPI
            Set oRootNode = .createElement("COMPOUNDS")
            Set .documentElement = oRootNode
        End With
        progOverall.value = 1
        Do Until rs.EOF
            i = i + 1
            
            ' add compound node
            Set oNode = XMLCompounds.createElement("COMPOUND")
            sBase64Cdx = moCFWImporter.Base64CDX(rs)
            
            With oNode
                .setAttribute "CAS", GetProperty(eCCas, rs)
                .setAttribute "ACX_ID", GetProperty(eACXID, rs)
                .setAttribute "SUBSTANCE_NAME", IIf(Len(GetProperty(eSubstanceName, rs)) > 0, GetProperty(eSubstanceName, rs), "No Name")
                .setAttribute "DENSITY", GetProperty(eDensity, rs)
                .setAttribute "CLOGP", GetProperty(eCcLogP, rs)
                .setAttribute "ROTATABLE_BONDS", GetProperty(eCRotatableBonds, rs)
                .setAttribute "TOT_POL_SURF_AREA", GetProperty(eCTotPolSurfArea, rs)
                .setAttribute "HBOND_ACCEPTORS", GetProperty(eHBondAcceptors, rs)
                .setAttribute "HBOND_DONORS", GetProperty(eHBondDonors, rs)
                .setAttribute "ALT_ID1", GetProperty(eAltID1, rs)
                .setAttribute "ALT_ID2", GetProperty(eAltID2, rs)
                .setAttribute "ALT_ID3", GetProperty(eAltID3, rs)
                .setAttribute "ALT_ID4", GetProperty(eAltID4, rs)
                .setAttribute "ALT_ID5", GetProperty(eAltID5, rs)
                .setAttribute "CREATOR", GetProperty(eCreator, rs)
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
        If bNewPlate Or rs.EOF Then
            lPlateCount = lPlateCount + 1
            ' tell user we're busy updating
            lblOverallProg.Caption = "Saving to database..."
            DoEvents
                    
            'XMLCompounds.save ("c:\debugPlate" & lPlateCount & ".xml")
            
            ' Disable the close button during the upload, as it will not accept a shutdown cleanly
            ' during the asynchronous HTTP post
            modCloseBtn.EnableCloseButton Me.hWnd, False
            sRet = UtilsInventory.CreateSubstanceXML(XMLCompounds.xml)
            modCloseBtn.EnableCloseButton Me.hWnd, True
            
            If chkLog.value = 1 Then
                LogAction sRet, xmlLogPath
            End If
        End If
        
        If gbSaveCompoundXML Then
            XMLCompounds.save (xmlLogPath & "DebugCompounds.xml")
        End If
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

Private Function GetRegProperty(ByVal Property As Long, ByRef rs As Recordset) As String
    Dim tfield As DBField
    Dim RegProperty As String
    Dim LocaleDate As Date
    Set tfield = moRegDict(Property)
    
    If tfield.UseDefault Then
        RegProperty = tfield.value
    Else
        RegProperty = CnvString(rs(CLng(tfield.SDFileField)).value, eDBtoVB)
    End If
    
    If tfield.eFieldType = eDate And RegProperty <> "" And Not IsEmpty(RegProperty) Then
        LocaleDate = Format(RegProperty, moApplicationVariablesDict("DATE_FORMAT_STRING"))
        ' Now get the US/English equivalent since the webserver middle tier is expecting this format
        GetRegProperty = Month(LocaleDate) & "/" & Day(LocaleDate) & "/" & Year(LocaleDate)
    Else
        GetRegProperty = RegProperty
    End If
End Function

Private Function GetProperty(ByVal PropertyId As Long, ByRef rs As Recordset) As String
    Dim tfield As DBField
    Dim LocaleDate As Date
    Dim Property As String
    
    Select Case (miLoadOption)
        Case ePlates
            Set tfield = moDictFields(PropertyId)
        Case eStructures
            Set tfield = moCompoundDictFields(PropertyId)
        Case eContainers
            Set tfield = moContainerDictFields(PropertyId)
    End Select
    If tfield.UseDefault Then
        Property = tfield.value
    Else
        Property = CnvString(rs(CLng(tfield.SDFileField)).value, eDBtoVB)
    End If
    
    If tfield.eFieldType = eDate And Property <> "" And Not IsEmpty(Property) Then
        LocaleDate = Format(Property, moApplicationVariablesDict("DATE_FORMAT_STRING"))
        ' Now get the US/English equivalent since the webserver middle tier is expecting this format
        GetProperty = Month(LocaleDate) & "/" & Day(LocaleDate) & "/" & Year(LocaleDate)
    Else
        GetProperty = Property
    End If
End Function

Private Function HasProperty(ByVal PropertyId As Long) As String
    Dim tfield As DBField
    Dim bReturn As Boolean
    
    Select Case (miLoadOption)
        Case ePlates
            Set tfield = moDictFields(PropertyId)
        Case eStructures
            Set tfield = moCompoundDictFields(PropertyId)
        Case eContainers
            Set tfield = moContainerDictFields(PropertyId)
    End Select
    
    bReturn = False
    If tfield.UseDefault Then
        If Len(Trim(tfield.value)) > 0 Then
            bReturn = True
        End If
    Else
        bReturn = True
    End If
    HasProperty = bReturn
End Function

Private Sub ActiveWizard1_PaneChanged(ByVal OldPane As Integer, ByVal NewPane As Integer)
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
        Else
            oAuthenticateXML.loadXML (sAuthenticate)
            If oAuthenticateXML.selectNodes("ERRORS/ERROR").length > 0 Then
                For i = 0 To oAuthenticateXML.selectNodes("ERRORS/ERROR").length - 1
                    sErr = sErr & oAuthenticateXML.selectNodes("ERRORS/ERROR").Item(i).Text & Chr(13)
                Next
                WizStopNext ActiveWizard1, OldPane, sErr
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
                Initialize
                ' save servername and username for future use
                SaveSetting App.EXEName, "General", "Server", msServerName
                SaveSetting App.EXEName, "General", "UseSSL", BooleanToText(mbUseSSL)
                SaveSetting App.EXEName, "General", "UserID", msUserID
                
                If Not mbINV_MANAGE_SUBSTANCES Then
                    sErr = "This user does not have the INV_MANAGE_SUBSTANCES privilege.  Please contact your system administrator."
                    bError = True
                End If
                
                If Not bError And mbINV_CREATE_PLATE And (bNoLocations Or bNoFormats) Then
                    sErr = "The inventory system is not configured properly for plates." & Chr(13)
                    If bNoLocations Then sErr = sErr & "-There are no plate locations configured." & Chr(13)
                    If bNoFormats Then sErr = sErr & "-There are no plate formats defined." & Chr(13)
                    bError = True
                End If
                
                'If Not bError And Not mbINV_CREATE_PLATE And Not mbINV_CREATE_CONTAINER Then
                    'sErr = "This user does not have rights to create plates or containers.  Please contact your system administrator."
                    'bError = True
                'End If
                
                If bError Then
                    WizStopNext ActiveWizard1, OldPane, sErr
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
                End If
            End If      ' If oAuthenticateXML.selectNodes("ERRORS/ERROR").length > 0 Then
        End If      ' If LCase(Left(sAuthenticate, 5)) = "error" Then
    ElseIf OldPane = ePaneFile And NewPane > OldPane Then
        ' init
        For i = ePlates To eStructures
            If optLoad(i) = True Then miLoadOption = i
        Next
        msFile = txtDB.Text
        If msFile = "" Then
            WizStopNext ActiveWizard1, OldPane, "Please specify a database to open."
        Else
            If moCFWImporter.Status = eNoMolServer Then
                WizStopNext ActiveWizard1, OldPane, "Could not open the specified database." & vbCrLf & "The server requires ChemOffice version 7.0 or above for this function to work." & vbCrLf & "Please contact your administrator."
            Else
                moCFWImporter.fileName = msFile
                ' open db
                If Not moCFWImporter.OpenDB Then
                    WizStopNext ActiveWizard1, OldPane, "Could not open the specified database."
                End If
                
                If moCFWImporter.bSelectMolTable Then
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
                End If
            End If
        End If
    ElseIf OldPane = ePaneMapping And NewPane > OldPane Then
    
        ' get values from table
        GetValuesFromTable
        ' determine whether or not a registry id is specified
        mbShowRegister = True
        If (moDictFields(ereg).UseDefault And Len(moDictFields(ereg).value) > 0) Or (Not moDictFields(ereg).UseDefault And Len(moDictFields(ereg).SDFileField) > 0) Then mbShowRegister = False
      
        ' check values to be sure we can go on
        sErr = CheckValues
        If sErr <> "" Then
            WizStopNext ActiveWizard1, OldPane, sErr
        Else
            If miLoadOption = ePlates Then
                lblNumCompounds = moCFWImporter.NumRecords
                UpdateNumPlates
                txtBarcodeStart.Visible = Not mbBarcodesFromFile
                If Not mbBarcodesFromFile Then
                    optBarcode(1).Caption = "Assign ascending numbers, starting with:"
                Else
                    optBarcode(1).Caption = "Use this barcode prefix:"
                End If
                optBarcode(2).Enabled = Not mbBarcodesFromFile
            ElseIf miLoadOption = eStructures Then
                mlNumCompounds = moCFWImporter.NumRecords
                ActiveWizard1.ViewPane ePaneLog
                'bFinish = True
            Else
                mlNumCompounds = moCFWImporter.NumRecords
                ActiveWizard1.ViewPane ePaneContainerAttributes
                If (moContainerDictFields(eLocationBarcode).UseDefault And Len(moContainerDictFields(eLocationBarcode).value) <> 0) Or (Not moContainerDictFields(eLocationBarcode).UseDefault) Then
                   cmbContainerLocations.Enabled = False
                Else
                    cmbContainerLocations.Enabled = True
                End If
                If (moContainerDictFields(eContainerTypeIdFk).UseDefault And Len(moContainerDictFields(eContainerTypeIdFk).value) <> 0) Or (Not moContainerDictFields(eContainerTypeIdFk).UseDefault) Then
                   cmbContainerType.Enabled = False
                Else
                    cmbContainerType.Enabled = True
                End If
                If (moContainerDictFields(eContainerStatusIdFk).UseDefault And Len(moContainerDictFields(eContainerStatusIdFk).value) <> 0) Or (Not moContainerDictFields(eContainerStatusIdFk).UseDefault) Then
                   cmbContainerStatus.Enabled = False
                Else
                    cmbContainerStatus.Enabled = True
                End If
                If (moContainerDictFields(eUnitOfMeasIdFk).UseDefault And Len(moContainerDictFields(eUnitOfMeasIdFk).value) <> 0) Or (Not moContainerDictFields(eUnitOfMeasIdFk).UseDefault) Then
                   cmbContainerUOM.Enabled = False
                Else
                    cmbContainerUOM.Enabled = True
                End If
                If (moContainerDictFields(eUnitOfWghtIdFk).UseDefault And Len(moContainerDictFields(eUnitOfWghtIdFk).value) <> 0) Or (Not moContainerDictFields(eUnitOfWghtIdFk).UseDefault) Then
                   cmbContainerUOW.Enabled = False
                Else
                    cmbContainerUOW.Enabled = True
                End If
                If (moContainerDictFields(eUnitOfDensityIdFk).UseDefault And Len(moContainerDictFields(eUnitOfDensityIdFk).value) <> 0) Or (Not moContainerDictFields(eUnitOfDensityIdFk).UseDefault) Then
                   cmbContainerUOD.Enabled = False
                Else
                    cmbContainerUOD.Enabled = True
                End If
                If (moContainerDictFields(eUnitOfConcIdFk).UseDefault And Len(moContainerDictFields(eUnitOfConcIdFk).value) <> 0) Or (Not moContainerDictFields(eUnitOfConcIdFk).UseDefault) Then
                   cmbContainerUOC.Enabled = False
                Else
                    cmbContainerUOC.Enabled = True
                End If
                If (moContainerDictFields(eUnitOfPurityIdFk).UseDefault And Len(moContainerDictFields(eUnitOfPurityIdFk).value) <> 0) Or (Not moContainerDictFields(eUnitOfPurityIdFk).UseDefault) Then
                   cmbContainerUOP.Enabled = False
                Else
                    cmbContainerUOP.Enabled = True
                End If
                If (moContainerDictFields(eOwnerIdFk).UseDefault And Len(moContainerDictFields(eOwnerIdFk).value) <> 0) Or (Not moContainerDictFields(eOwnerIdFk).UseDefault) Then
                   cmbContainerOwner.Enabled = False
                Else
                    cmbContainerOwner.Enabled = True
                End If
                If (moContainerDictFields(eCBarcode).UseDefault And Len(moContainerDictFields(eCBarcode).value) <> 0) Or (Not moContainerDictFields(eCBarcode).UseDefault) Then
                   cmbContainerBarcodeDesc.Enabled = False
                Else
                    cmbContainerBarcodeDesc.Enabled = True
                End If
            End If
        End If
    ElseIf OldPane = ePaneMapping And NewPane < OldPane Then
        ' Need to close the DB connection, as the Excel driver keeps a lock on the file
        moCFWImporter.CloseDB
    ElseIf OldPane = ePaneFormat And NewPane = ePaneFinish Then
        ActiveWizard1.FinishButtonEnabled = True
        frmOverallProg.Visible = False
        frmPlateProg.Visible = False
        lblNotify.Visible = True
    
    ElseIf OldPane = ePaneFormat And NewPane > OldPane And NewPane <> ePaneLog And NewPane <> ePaneContainerAttributes Then
        Dim vkey As Variant
        Dim bUseMappedField As Boolean
        mlPlateFormat = cmbPlateFormat.ItemData(cmbPlateFormat.ListIndex)
        mlPlateType = cmbPlateType.ItemData(cmbPlateType.ListIndex)
        'ComboFill cmbBarcodeField, moCFWImporter.FieldsDict
        'if a barcode was mapped don't allow it to be changed here
        
        If (moDictFields(eBarcode).SDFileField >= 0 And moDictFields(eBarcode).SDFileField <> "") _
        Or (moDictFields(eBarcode).UseDefault And moDictFields(eBarcode).value <> "") Then
            cmbBarcodeField.Clear
            For Each vkey In moCFWImporter.FieldsDict
                If vkey <> 1000 Then
                    If moCFWImporter.FieldsDict(vkey) <> "" And Not IsEmpty(moCFWImporter.FieldsDict(vkey)) Then
                        cmbBarcodeField.AddItem moCFWImporter.FieldsDict(vkey)
                        cmbBarcodeField.ItemData(cmbBarcodeField.ListCount - 1) = vkey
                    End If
                End If
            Next
            bUseMappedField = ComboSelect(cmbBarcodeField, moCFWImporter.FieldsDict(moDictFields(eBarcode).SDFileField))
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
        'moDictFields(lPlateFld).SDFileField
        'cmbBarcodeField.se
        If mlPerPlate = 0 Then
            WizStopNext ActiveWizard1, OldPane, "You cannot import compounds into a plate format with no compound wells."
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
        If bError Then WizStopNext ActiveWizard1, OldPane, sErrorMsg
        
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
                mbCustomBarcodes = True
            Else
                'sErr = CheckBarcodes
                If sErr <> "" Then
                    WizStopNext ActiveWizard1, OldPane, sErr
                Else
                    If mbShowRegister Then
                        ActiveWizard1.ViewPane ePaneReg
                    Else
                        ActiveWizard1.ViewPane ePaneLog
                        mbRegister = False
                        'bFinish = True
                    End If
                    mbCustomBarcodes = False
                End If
            End If
        
        End If
    ElseIf OldPane = ePaneBarcodes And NewPane > OldPane And NewPane <> ePaneContainerAttributes Then
        GetBarcodes
        GetGroups
        If Not optBarcode(0).value Then   'not a barcode description
            sErr = CheckBarcodes
        Else
            sErr = ""
        End If
        If sErr <> "" Then
            WizStopNext ActiveWizard1, OldPane, sErr
        Else
            sErr = CheckGroups
            If sErr <> "" Then
                WizStopNext ActiveWizard1, OldPane, sErr
            Else
                If mbShowRegister Then
                   ActiveWizard1.ViewPane ePaneReg
                Else
                   ActiveWizard1.ViewPane ePaneLog
                   mbRegister = False
                   'bFinish = True
                End If
            End If
        End If
    ElseIf OldPane = ePaneReg And NewPane > OldPane Then
        Dim sAlertText
        sAlertText = ""
        
        ' save properties to local disk
        mbRegister = CheckBoxBoolean(chkRegister.value)
                
        If mbRegister Then
            sErrorMsg = ""
            'validate options
            If moRegOptionFields(eDupAction).value = "" Then
                bError = True
                sErrorMsg = sErrorMsg & "You must select a Duplicate Action." & vbCrLf
            End If
            If moRegOptionFields(ePrefix).value = "" Then
                bError = True
                sErrorMsg = sErrorMsg & "You must select a prefix." & vbCrLf
            End If
            If bError Then WizStopNext ActiveWizard1, OldPane, sErrorMsg
            
            ' alert about choosing 2 values for a field
            ' DJP removed fields mapped by dropdown from the 2nd form
            'If Not moRegDict Is Nothing Then
            '    If ((moRegDict("COMPOUND_TYPE").UseDefault And moRegDict("COMPOUND_TYPE").value <> "") Or moRegDict("COMPOUND_TYPE").UseDefault = False) _
            '        And (moRegOptionFields(eCompound) <> "") Then
            '            sAlertText = sAlertText & vbTab & "-Compound/Compound_Type" & vbLf
            '    End If
            '    If ((moRegDict("NOTEBOOK_NUMBER").UseDefault And moRegDict("NOTEBOOK_NUMBER").value <> "") Or moRegDict("NOTEBOOK_NUMBER").UseDefault = False) _
            '        And (moRegOptionFields(eNotebook) <> "") Then
            '            sAlertText = sAlertText & vbTab & "-Notebook/Notebook_Number" & vbLf
            '    End If
            '    If ((moRegDict("SALT_CODE").UseDefault And moRegDict("SALT_CODE").value <> "") Or moRegDict("SALT_CODE").UseDefault = False) _
            '        And (moRegOptionFields(eSalt) <> "") Then
            '            sAlertText = sAlertText & vbTab & "-Salt/Salt_Code" & vbLf
            '    End If
            'End If
            'If sAlertText <> "" Then MsgBox ("You have both selected a value for and mapped data to the following fields.  The mapped data will be used in lieu of the selected data." & vbLf & sAlertText)
        End If
    ElseIf OldPane = ePaneReg And NewPane < OldPane Then
        If mbCustomBarcodes Then
            ActiveWizard1.ViewPane ePaneBarcodes
        ElseIf miLoadOption = eStructures Then
            ActiveWizard1.ViewPane ePaneMapping
         ElseIf miLoadOption = eContainers Then
            ActiveWizard1.ViewPane ePaneContainerAttributes
        Else
            ActiveWizard1.ViewPane ePaneLibrary
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
            If bError Then WizStopNext ActiveWizard1, OldPane, sErrorMsg
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
    ElseIf OldPane = ePaneLog And NewPane < OldPane Then
        If miLoadOption = eStructures Then
            ActiveWizard1.ViewPane ePaneMapping
        End If
        
        If Not mbShowRegister Then
            ActiveWizard1.ViewPane ePaneBarcodes
        End If
   ' End If
   
    ElseIf OldPane = ePaneFormat And NewPane > OldPane And NewPane = ePaneContainerAttributes Then
        ActiveWizard1.NextButtonEnabled = True
        lblNumContainers.Caption = mlNumCompounds
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
    sSQL = "Select count(innerCount) as theCount from (Select count([" & moCFWImporter.FieldsDict(moDictFields(eBarcode).SDFileField) & "]) as innerCount from " & moCFWImporter.RealTableName() & " group by [" & moCFWImporter.FieldsDict(moDictFields(eBarcode).SDFileField) & "])"
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
    Dim lPlateFld As Long
    
    If IsNumeric(lblNumCompounds.Caption) Then
        If moDictFields(eBarcode).SDFileField = "" Then
            If moDictFields(esupplierplatenumber).SDFileField = "" Then
                If moDictFields(esupplierbarcode).SDFileField = "" Then
                    lPlateFld = -1
                Else
                    lPlateFld = esupplierbarcode
                End If
            Else
                lPlateFld = esupplierplatenumber
            End If
        Else
            lPlateFld = eBarcode
        End If
    
        'determine the number of plates if a barcode, supplier plate num, or supplier barcode is specified
        If lPlateFld >= 0 Then
            ' IRL added [] symbols to handle field names with spaces 8/18/04
            sSQL = "Select count(innerCount) as theCount from (Select count([" & moCFWImporter.FieldsDict(moDictFields(lPlateFld).SDFileField) & "]) as innerCount from " & moCFWImporter.RealTableName() & " group by [" & moCFWImporter.FieldsDict(moDictFields(lPlateFld).SDFileField) & "])"
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
    Dim lPlateFld As Long
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
    
    Set TempDict = New Dictionary
    
    If Not mbShowRegister Then
        'Debug.Print "moDictFields(eBatchNumber).value=" & moDictFields(eBatchNumber).value
        If (moDictFields(eBatchNumber).UseDefault And Len(moDictFields(eBatchNumber).value) = 0) Or (Not moDictFields(eBatchNumber).UseDefault And Len(moDictFields(eBatchNumber).SDFileField) = 0) Then
            sErr = "You must enter a batch number." & vbCrLf
            GoTo DoneChecking
        ElseIf moDictFields(eBatchNumber).UseDefault And Not IsNumeric(moDictFields(eBatchNumber).value) Then
            sErr = "Batch number must be a number." & vbCrLf
            GoTo DoneChecking
        Else
            sReturnVal = CheckRegValues(moDictFields, ereg, eBatchNumber)
            If sReturnVal <> "" Then
                sErr = sReturnVal
                GoTo DoneChecking
            End If
        End If
        
    End If

    wellfield = moDictFields(eWellCoordinates).SDFileField
    rowfield = moDictFields(eRow).SDFileField
    colfield = moDictFields(eCol).SDFileField
    mbHaveWellCoords = False
    If moDictFields(eBarcode).SDFileField <> "" Then
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
            mlWellCoordField = eWellCoordinates
            mlRowField = eRow
            mlColField = eCol
        Else
            mlWellCoordField = eRow
        End If
    End If
    ' check plate indicator
    mbHavePlateCoords = False
    barcodefield = moDictFields(eBarcode).SDFileField
    suppbarcodefield = moDictFields(esupplierbarcode).SDFileField
    suppplatenumber = moDictFields(esupplierplatenumber).SDFileField
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
            mlPlateCoordField = esupplierplatenumber
        ElseIf suppbarcodefield <> "" Then
            mlPlateCoordField = esupplierbarcode
        Else
            mlPlateCoordField = eBarcode
        End If
    End If
    
    If moDictFields(eBarcode).SDFileField = "" Then
        If moDictFields(esupplierplatenumber).SDFileField = "" Then
            lPlateFld = esupplierbarcode
        Else
            lPlateFld = esupplierplatenumber
        End If
    Else
        lPlateFld = eBarcode
    End If
    
    ' Check for duplicate barcodes in Inventory here, before the upload is attempted, so we can
    ' generate a warning and have the user fix the problem
    sBarcodeList = ""
    Set rs = moCFWImporter.DistinctRecordset(CnvLong(moDictFields(lPlateFld).SDFileField, eDBtoVB))
    While Not rs.EOF
        If Not IsNull(rs.fields(0).value) Then
            sBarcode = Trim(CStr(rs.fields(0).value))
            If Len(sBarcode) > 0 Then
                TempDict.Add sBarcode, sBarcode
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
    
    sErr = CheckTableValue(moDictFields, eSolventVolumeUnit, "Plate Solvent", "inv_units")
    If sErr <> "" Then
        GoTo DoneChecking
    End If
    
    sErr = CheckTableValue(moDictFields, eWellSolventVolumeUnit, "Well Solvent", "inv_units")
    If sErr <> "" Then
        GoTo DoneChecking
    End If

DoneChecking:
    CheckPlateValues = sErr
    
    Set TempDict = Nothing
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
    Dim vAns
    ' check the well indicator
    Dim tfield As DBField
    Dim wellfield, rowfield, colfield
    Dim lPlateFld As Long
    Dim rs As Recordset
    Dim sBarcode As String
    Dim sBarcodeList As String
    Dim TempDict As Dictionary
    Dim sPrimaryKeys As String
    Dim sUnit As String
    Dim sReturn As String
    Dim sCompoundID As String
    Dim lCompoundID As Long
    Dim sLocation As String
    Dim sLocationID As String
    Dim sLocationList As String
    Dim vkey
    Dim arrLocationFields As Variant
    Dim arrBarcodes As Variant
    Dim arrBarcodeContainerID As Variant
    Dim i As Integer
    Dim iLen As Integer
    Dim lListLen As Long
    Dim barcodefield, suppbarcodefield, suppplatenumber
    Dim sRegNumber, sBatchNumber, sArrRegNumber, sArrBatchNumber, count, sReturnVal, sRegId, sArrRegFields, limit, tempReg, tempbatch
    Dim SRegNumberMapped As Boolean
    Dim sBatchNumberMapped As Boolean
    
    Set TempDict = New Dictionary
    
    If ((Not moContainerDictFields(eRegNumber).UseDefault Or Len(moContainerDictFields(eRegNumber).value) <> 0) And (Len(moContainerDictFields(eBatchNumberFk).value) = 0 And moContainerDictFields(eBatchNumberFk).UseDefault)) Then
        sErr = "If you map Registration ID you must also map Batch Number." & vbCrLf
        GoTo DoneChecking
    End If
    
    ' Check for duplicate barcodes within the user's database file
    sBarcodeList = ""
    If Not moContainerDictFields(eCBarcode).UseDefault Then
        Set rs = moCFWImporter.DuplicateChecking(CnvLong(moContainerDictFields(eCBarcode).SDFileField, eDBtoVB))
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
        
        ' Ensure the container barcodes don't already exist
        ' kfd - changed to limit barcode list length
        sBarcodeList = ""
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(moContainerDictFields(eCBarcode).SDFileField, eDBtoVB))
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
             Loop Until rs.EOF
        
        End If  ' If Not rs.EOF And Not rs.BOF Then
    ElseIf moContainerDictFields(eCBarcode).UseDefault And Len(moContainerDictFields(eCBarcode).value) <> 0 Then
          sErr = "Container barcodes cannot be entered as default value."
          GoTo DoneChecking
    End If
    
    sErr = sErr & CheckTableValue(moContainerDictFields, eContainerTypeIdFk, "Container Type", "inv_container_types")
    sErr = sErr & CheckTableValue(moContainerDictFields, eContainerStatusIdFk, "Container Status", "inv_container_status")
    sErr = sErr & CheckTableValue(moContainerDictFields, eUnitOfMeasIdFk, "Unit Of Measure", "inv_units")
    'sErr = sErr & CheckTableValue(moContainerDictFields, eLocationBarcode, "Location", "inv_locations")
    sErr = sErr & CheckTableValue(moContainerDictFields, eUnitOfPurityIdFk, "Purity", "inv_units")
    sErr = sErr & CheckTableValue(moContainerDictFields, eSupplierIdFk, "Supplier ID", "inv_suppliers")
    sErr = sErr & CheckTableValue(moContainerDictFields, eOwnerIdFk, "Owner ID", "inv_owners")
    sErr = sErr & CheckTableValue(moContainerDictFields, eUnitOfConcIdFk, "Concentration", "inv_units")
    sErr = sErr & CheckTableValue(moContainerDictFields, eUnitOfWghtIdFk, "Weight", "inv_units")
    sErr = sErr & CheckTableValue(moContainerDictFields, eUnitOfDensityIdFk, "Density", "inv_units")
    
     ' Validate the location ID
    TempDict.RemoveAll
    If moContainerDictFields(eLocationBarcode).UseDefault Then
        sBarcode = Trim(moContainerDictFields(eLocationBarcode).value)
        If Not IsNull(sBarcode) And sBarcode <> "" Then
            sLocationID = GetLocationFromBarcode(sBarcode)
            If Len(sLocationID) = 0 Then
                sErr = "The location barcode '" & sBarcode & "' was not found in the Inventory database.  "
                sErr = sErr & "The location barcode must be fixed before the upload can proceed.  " & vbLf
                sErr = sErr & "Alternatively, you may leave the location barcode field blank and specify the location in the next step."
                GoTo DoneChecking
            Else
                ' Add this to our mapping dictionary so we can replace barcode with location ID
                ' during the XML creation
                ' GetLocationFromBarcode returns a string like "2281,L1041,Containers,0" if found
                arrLocationFields = Split(sLocationID, ",")
                sLocationID = arrLocationFields(0)
                If Not moLocationBarcodeMapping.Exists(sBarcode) Then
                     moLocationBarcodeMapping.Add sBarcode, sLocationID
                End If
            End If
        End If
    Else
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(moContainerDictFields(eLocationBarcode).SDFileField, eDBtoVB))
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
                    If Not moLocationBarcodeMapping.Exists(sBarcode) Then
                        moLocationBarcodeMapping.Add sBarcode, sLocationID
                    End If
                End If
            End If
            rs.MoveNext
        Wend
        rs.Close
        Set rs = Nothing
         
        If TempDict.count > 0 And TempDict.count <= 5 Then
            sErr = "The following location barcodes do not exist within Inventory:" & vbLf & vbLf
            For Each vkey In TempDict
                sErr = sErr & vbTab & vkey & vbLf
            Next
            sErr = sErr & vbLf & "These barcodes must be fixed before the upload can proceed."
            GoTo DoneChecking
        ElseIf TempDict.count > 5 Then
            sErr = "Multiple location barcodes from this database do not exist within Inventory." & vbLf & vbLf
            sErr = sErr & "These barcodes must be fixed before the upload can proceed."
            GoTo DoneChecking
        End If
    End If
         
    ' Validate the compound ID
    TempDict.RemoveAll
    If moContainerDictFields(eCompoundIdFk).UseDefault Then
        sCompoundID = moContainerDictFields(eCompoundIdFk).value
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
        End If
    Else
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(moContainerDictFields(eCompoundIdFk).SDFileField, eDBtoVB))
        While Not rs.EOF
            sCompoundID = rs.fields(0).value
            If IsNumeric(sCompoundID) Then
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
            End If
            rs.MoveNext
        Wend
        rs.Close
        Set rs = Nothing
            
        If TempDict.count > 0 And TempDict.count <= 5 Then
            sErr = "The following compound IDs do not exist within Inventory:" & vbLf & vbLf
            For Each vkey In TempDict
                sErr = sErr & vbTab & vkey & vbLf
            Next
            sErr = sErr & vbLf & "These compound IDs must be fixed before the upload can proceed."
            GoTo DoneChecking
        ElseIf TempDict.count > 5 Then
            sErr = "Multiple compound IDs from this database do not exist within Inventory." & vbLf & vbLf
            sErr = sErr & "These compound IDs must be fixed before the upload can proceed."
            GoTo DoneChecking
        End If
    End If
    
    'Validating the Registration Number field
    If (Not moContainerDictFields(eRegNumber).UseDefault Or Len(moContainerDictFields(eRegNumber).value) <> 0) And (Not moContainerDictFields(eBatchNumberFk).UseDefault Or Len(moContainerDictFields(eBatchNumberFk).value) <> 0) Then
        sReturnVal = CheckRegValues(moContainerDictFields, eRegNumber, eBatchNumberFk)
        If sReturnVal <> "" Then
            sErr = sReturnVal
        End If
        GoTo DoneChecking
    End If
DoneChecking:
    CheckContainerValues = sErr
    Set TempDict = Nothing
    
End Function
Function CheckRegValues(ByRef objdict As Dictionary, ByRef RegNumberField As Variant, ByRef BatchNumberField As Variant) As String
    Dim rs As Recordset
    Dim SRegNumberMapped As Boolean
    Dim sBatchNumberMapped As Boolean
    Dim sRegNumber, sBatchNumber, sArrRegNumber, sArrBatchNumber, count, sReturnVal, sRegId, sArrRegFields, limit, tempReg, tempbatch
    Dim TempDict As Dictionary
    Dim vkey
    
    Set TempDict = New Dictionary
    SRegNumberMapped = False
    sBatchNumberMapped = False
    TempDict.RemoveAll
     If objdict(RegNumberField).UseDefault Then
         sRegNumber = Trim(objdict(RegNumberField).value)
          sArrRegNumber = Split(sRegNumber, ",")
     Else
         SRegNumberMapped = True
     End If
     If objdict(BatchNumberField).UseDefault Then
         sBatchNumber = Trim(objdict(BatchNumberField).value)
         sArrBatchNumber = Split(sBatchNumber, ",")
     Else
         sBatchNumberMapped = True
     End If
    If SRegNumberMapped Or sBatchNumberMapped Then
         Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
         'Set rs = moCFWImporter.DistinctRecordset(CnvLong(moContainerDictFields(eRegNumber).SDFileField, eDBtoVB))
         count = 0
         While Not rs.EOF
             If SRegNumberMapped Then sRegNumber = sRegNumber & "," & Trim(GetProperty(RegNumberField, rs))
             If sBatchNumberMapped Then sBatchNumber = sBatchNumber & "," & Trim(GetProperty(BatchNumberField, rs))
             rs.MoveNext
         Wend
         If SRegNumberMapped Then sRegNumber = Mid(sRegNumber, 2, Len(sRegNumber))
         If sBatchNumberMapped Then sBatchNumber = Mid(sBatchNumber, 2, Len(sBatchNumber))
         rs.Close
         Set rs = Nothing
         If SRegNumberMapped Then
             sArrRegNumber = Split(sRegNumber, ",")
             sRegNumber = Null
         End If
         If sBatchNumberMapped Then
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
Function CheckTableValue(ByRef objdict As Dictionary, lIndex As Long, sDisplay As String, sTableName As String) As String
    Dim rs As ADODB.Recordset
    Dim sTableValue As String
    Dim sReturn As String
    Dim sResult As String
    Dim sTableValueList As String
    Dim i As Integer
    Dim aTableValues
        
    ' Check to ensure the units are valid
    sReturn = ""
    
    sTableValue = objdict(lIndex).SDFileField
    If sTableValue = "" Then
        sTableValue = objdict(lIndex).value
        If sTableValue <> "" Then
            sResult = UtilsInventory.LookUpValue(sTableName, sTableValue)
            If sResult = "NOT FOUND" Then
                Select Case (sTableName)
                Case "inv_units"
                    sReturn = "The " & sDisplay & " unit '" & sTableValue & "' does not exist in the Inventory database." & vbLf & vbLf
                    sReturn = sReturn & "Please correct the unit to match an existing value, or add the new unit to the database."
                Case "inv_container_types"
                    sReturn = "The " & sDisplay & " type '" & sTableValue & "' does not exist in the Inventory database." & vbLf & vbLf
                    sReturn = sReturn & "Please correct the container type to match an existing value, or add the new container type to the database."
                Case "inv_container_status"
                    sReturn = "The " & sDisplay & " status '" & sTableValue & "' does not exist in the Inventory database." & vbLf & vbLf
                    sReturn = sReturn & "Please correct the container status to match an existing value, or add the new container status to the database."
                Case "inv_suppliers"
                    sReturn = "The " & sDisplay & " '" & sTableValue & "' does not exist in the Inventory database." & vbLf & vbLf
                    sReturn = sReturn & "Please correct the supplier id to match an existing value."
                Case "inv_owners"
                    sReturn = "The " & sDisplay & " '" & sTableValue & "' does not exist in the Inventory database." & vbLf & vbLf
                    sReturn = sReturn & "Please correct the owner id to match an existing value, or add the new owner id to the database."
                End Select
            ElseIf InStr(1, LCase(sResult), "error") > 0 Then
                CheckTableValue = sResult
                Exit Function
            End If
        End If
    Else
        Set rs = moCFWImporter.DistinctRecordset(CnvLong(objdict(lIndex).SDFileField, eDBtoVB))
        While Not rs.EOF
            sTableValue = rs.fields(0).value
            If Len(sTableValue) > 0 Then
                sResult = UtilsInventory.LookUpValue(sTableName, sTableValue)
                If sResult = "NOT FOUND" Then
                    sTableValueList = sTableValueList & sTableValue & ","
                ElseIf InStr(1, LCase(sResult), "error") > 0 Then
                    CheckTableValue = sResult
                    Exit Function
                End If
            End If
            rs.MoveNext
        Wend
        rs.Close
        Set rs = Nothing
        
        If Len(sTableValueList) > 0 Then
            aTableValues = Split(sTableValueList, ",")
            Select Case (sTableName)
            
            Case "inv_units"
                If UBound(aTableValues) <= 5 Then
                    sReturn = "The following " & sDisplay & " units do not exist in the Inventory database:" & vbLf & vbLf
                    For i = 0 To UBound(aTableValues)
                        sReturn = sReturn & vbTab & aTableValues(i) & vbLf
                    Next
                    sReturn = sReturn & vbLf & "These units must be fixed in your database before the upload can proceed."
                Else
                    sReturn = "Multiple " & sDisplay & " units within the source database do not exist within Inventory." & vbLf & vbLf
                    sReturn = sReturn & "Please check to make sure that all specified units are valid."
                End If
            Case "inv_container_types"
                If UBound(aTableValues) <= 5 Then
                    sReturn = "The following " & sDisplay & " do not exist in the Inventory database:" & vbLf & vbLf
                    For i = 0 To UBound(aTableValues)
                        sReturn = sReturn & vbTab & aTableValues(i) & vbLf
                    Next
                    sReturn = sReturn & vbLf & "These container type must be fixed in your database before the upload can proceed."
                Else
                    sReturn = "Multiple " & sDisplay & " within the source database do not exist within Inventory." & vbLf & vbLf
                    sReturn = sReturn & "Please check to make sure that all specified container types are valid."
                End If
            Case "inv_container_status"
                   If UBound(aTableValues) <= 5 Then
                    sReturn = "The following " & sDisplay & " do not exist in the Inventory database:" & vbLf & vbLf
                    For i = 0 To UBound(aTableValues)
                        sReturn = sReturn & vbTab & aTableValues(i) & vbLf
                    Next
                    sReturn = sReturn & vbLf & "These container status must be fixed in your database before the upload can proceed."
                Else
                    sReturn = "Multiple " & sDisplay & " within the source database do not exist within Inventory." & vbLf & vbLf
                    sReturn = sReturn & "Please check to make sure that all specified container status are valid."
                End If
           Case "inv_suppliers"
                   If UBound(aTableValues) <= 5 Then
                    sReturn = "The following " & sDisplay & " do not exist in the Inventory database:" & vbLf & vbLf
                    For i = 0 To UBound(aTableValues)
                        sReturn = sReturn & vbTab & aTableValues(i) & vbLf
                    Next
                    sReturn = sReturn & vbLf & "These supplier must be fixed in your database before the upload can proceed."
                Else
                    sReturn = "Multiple " & sDisplay & " within the source database do not exist within Inventory." & vbLf & vbLf
                    sReturn = sReturn & "Please check to make sure that all specified supplier id are valid."
                End If
          Case "inv_owners"
               If UBound(aTableValues) <= 5 Then
                    sReturn = "The following " & sDisplay & " do not exist in the Inventory database:" & vbLf & vbLf
                    For i = 0 To UBound(aTableValues)
                        sReturn = sReturn & vbTab & aTableValues(i) & vbLf
                    Next
                    sReturn = sReturn & vbLf & "These owners must be fixed in your database before the upload can proceed."
                Else
                    sReturn = "Multiple " & sDisplay & " within the source database do not exist within Inventory." & vbLf & vbLf
                    sReturn = sReturn & "Please check to make sure that all specified owner id are valid."
                End If
           End Select
        End If
    End If
    
    CheckTableValue = sReturn
    
End Function


Private Sub GetValuesFromTable()
    ' get values from the grid
    Dim lUpperLimit As Long
    
    With grdFieldDict
        Dim i As Long
        Select Case miLoadOption
            Case ePlates
                lUpperLimit = edbfieldcount
                For i = 1 To lUpperLimit - 1
                    If .TextMatrix(i, eMapping) = "1000" Then
                        moDictFields(i).value = .TextMatrix(i, eValue)
                        moDictFields(i).SDFileField = ""
                        moDictFields(i).UseDefault = True
                    Else
                        moDictFields(i).SDFileField = .TextMatrix(i, eMapping)
                        moDictFields(i).UseDefault = False
                    End If
                Next
            Case eStructures
                lUpperLimit = eCompoundDBFieldCount
                For i = 1 To lUpperLimit - 1
                    If .TextMatrix(i, eMapping) = "1000" Then
                        moCompoundDictFields(i).value = .TextMatrix(i, eValue)
                        moCompoundDictFields(i).SDFileField = ""
                        moCompoundDictFields(i).UseDefault = True
                    Else
                        moCompoundDictFields(i).SDFileField = .TextMatrix(i, eMapping)
                        moCompoundDictFields(i).UseDefault = False
                    End If
                Next
            Case eContainers
                lUpperLimit = eContainerDBFieldCount
                For i = 1 To lUpperLimit - 1
                    If .TextMatrix(i, eMapping) = "1000" Then
                        moContainerDictFields(i).value = .TextMatrix(i, eValue)
                        moContainerDictFields(i).SDFileField = ""
                        moContainerDictFields(i).UseDefault = True
                    Else
                        moContainerDictFields(i).SDFileField = .TextMatrix(i, eMapping)
                        moContainerDictFields(i).UseDefault = False
                    End If
                Next
            End Select
    End With
End Sub

Private Sub FillDictGrid()
    Dim i As Long
    With grdFieldDict
        .ExtendLastCol = True
        .Rows = 0
        .Cols = 4
        .AddItem "Value" & vbTab & "Data Type" & vbTab & "Field Name" & vbTab & "Default"
        Select Case miLoadOption
            Case ePlates
                For i = 1 To edbfieldcount - 1
                    .AddItem moDictFields(i).DisplayName & vbTab & GetFieldTypeAsString(moDictFields(i).eFieldType) & vbTab & "1000" & vbTab & moDictFields(i).value
                Next
            Case eStructures
                For i = 1 To eCompoundDBFieldCount - 1
                    .AddItem moCompoundDictFields(i).DisplayName & vbTab & GetFieldTypeAsString(moCompoundDictFields(i).eFieldType) & vbTab & "1000" & vbTab & moCompoundDictFields(i).value
                Next
            Case eContainers
                For i = 1 To eContainerDBFieldCount - 1
                    .AddItem moContainerDictFields(i).DisplayName & vbTab & GetFieldTypeAsString(moContainerDictFields(i).eFieldType) & vbTab & "1000" & vbTab & moContainerDictFields(i).value
                Next
        End Select
        .FixedRows = 1
        .FixedCols = 2
        .ColComboList(eMapping) = GridBuildComboList(moCFWImporter.FieldsDict)
        .AutoSize 0, 3
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

Private Sub FillRegOptionGrid()
    Dim i As Long
    Dim rField As RegField
    Dim indexList As String
    Dim valueList As String
    Dim value As String
    
    On Error GoTo Error
    With grdRegOptions
        .ExtendLastCol = True
        .Rows = 0
        .Cols = 2
        .AddItem "Field Name" & vbTab & "Value"
        .FixedRows = 1
        
        For i = 1 To eRegOptionCount - 1
            Set rField = moRegOptionFields(i)
            .AddItem rField.DisplayName
            If rField.DefaultText <> "" Then
                .Cell(flexcpText, .Rows - 1, 1) = rField.DefaultText
                rField.value = rField.DefaultValue
            End If
        Next i
        .AutoSize 0, 1
    End With
    Exit Sub
Error:
    MsgBox "FillRegOptionGrid error: " & Err.Description
End Sub

Function GetRegPicklist(sListName As String, Optional bCombo As Boolean = False) As String
    Dim sXML As String
    Dim sMsg As String
    Dim sList As String
    
    On Error GoTo Error
    sXML = HTTPRequest("POST", msServerName, mbUseSSL, RegApiFolder & "/GetPicklistXML.asp", "", "picklist=" & sListName & "&USRCODE=" & msUserID & "&AUTHENTICATE=" & msPassword)
    sMsg = PicklistFromXML(sXML, sList, True)
    If sMsg <> "" Then
        MsgBox "Error building " & sListName & " picklist: " & sMsg
    End If
    GetRegPicklist = sList
    Exit Function
Error:
    MsgBox "Error getting " & sListName & " picklist: " & Err.Description
End Function

Private Sub btnAdvancedOptions_Click()
    With frmAdvancedOptions
        .Initialize
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
    
    If chkRegister.value Then
        
        originalCaption = chkRegister.Caption
        
        Screen.MousePointer = vbHourglass
        chkRegister.Caption = originalCaption + ".   Authenticating user..."
        
        'SYAN moved reg authentication here because it is slow 7/9/2004
        'Authenticate only when reg is needed.
        
        'check if this user is authorized to register compounds
'kfd        response = HTTPRequest("POST", msServerName, RegApiFolder & "/reg_post_action.asp", "", "reg_method=reg_perm&USER_ID=" & msUserID & "&USER_PWD=" & msPassword & "&REG_PARAMETER=AUTHENTICATE")
        response = HTTPRequest("POST", msServerName, mbUseSSL, RegApiFolder & "/CheckRegisterPriv.asp", "", "USRCODE=" & msUserID & "&AUTHENTICATE=" & msPassword)
'        response = HTTPRequest("POST", msServerName, "cheminv/Reg/CheckRegisterPriv.asp", "", "UserID=" & msUserID & "&Password=" & msPassword)
        
        Screen.MousePointer = vbDefault
        If response = "user authenticated" Then
            'check if reg is integrated with inv
            response = HTTPRequest("POST", msServerName, mbUseSSL, "cheminv/api/GetConfigInfo.asp", "", "ConfigKey=Reg_Server_Name")
            If LCase(response) <> "null" Then
                'fill in reg combos
                FillRegOptionGrid
                grdRegOptions.Visible = True
                cmdRegisterOptions.Visible = True
            Else
                MsgBox ("Inventory is not integrated with Registration.")
                mbShowRegister = False
                chkRegister.value = Bool2Checkbox(False)
            End If
        Else
            MsgBox ("The user does not have privileges to register compounds." & vbCrLf & vbCrLf & response)
            mbShowRegister = False
            chkRegister.value = Bool2Checkbox(False)
        End If
        
        chkRegister.Caption = originalCaption
        'End of SYAN modification
        
    Else
        grdRegOptions.Visible = False
        cmdRegisterOptions.Visible = False
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


Private Sub cmdRegisterOptions_Click()
    With frmRegOptions
        Set .moCFWImporter = moCFWImporter
        If Not moRegDict Is Nothing Then
            Set .RegDict = moRegDict
        End If
        .Initialize
        .Show vbModal, Me
        If Not .Cancel Then
            Set moRegDict = .RegDict
        End If
    End With
End Sub

Private Sub cmdLoadChemMappings_Click()
    Dim eMappingType As MappingType
    Dim NameToIndexMapping As Dictionary
    Dim lUpperLimit, i As Long
    
    Set NameToIndexMapping = New Dictionary
    
    With grdFieldDict
        Select Case miLoadOption
            Case ePlates
                eMappingType = eInventoryPlates
                lUpperLimit = edbfieldcount
            Case eStructures
                eMappingType = eInventoryStructures
                lUpperLimit = eCompoundDBFieldCount
            Case eContainers
                eMappingType = eInventoryContainers
                lUpperLimit = eContainerDBFieldCount
        End Select
        
        For i = 1 To lUpperLimit - 1
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


Private Sub Command1_Click()
    'Call UtilsMisc.CreateMoveContainerXML
End Sub

Private Sub Form_Load()
    Call UtilsMisc.GetGlobals
    Call UtilsMisc.GetAllFieldMappings
    
    ' set up grid
    ActiveWizard1.FinishButtonEnabled = False
    LocationID = NULL_AS_LONG
    Set moDictFields = New Dictionary
    Set moCompoundDictFields = New Dictionary
    Set moContainerDictFields = New Dictionary
    Set moRegOptionFields = New Dictionary
    Set moLocationBarcodeMapping = New Dictionary
    Set moRegistrationNumberMapping = New Dictionary
    SetDBFields
    GridFormatGeneric grdBarcodes
    GridFormatGeneric grdFieldDict
    GridFormatGeneric grdRegOptions
    grdRegOptions.Visible = False
    grdBarcodes.Rows = 0
    ' other properties
    mlPerPlate = 0
    mbFinished = False
    Set moDictBarcodes = New Dictionary
    Set moDictGroups = New Dictionary
    Set moApplicationVariablesDict = New Dictionary
    bLoadMappingsFromXML = False
    
    Label1.Caption = "This wizard will import compounds from a ChemFinder or Excel database directly into Inventory Manager, a series of new plates, or new containers.  Most often, your compounds will start in an SDFile.  To convert this SDFile into a ChemFinder database, please do the following:"
    
End Sub

Private Function GetNewLocation() As Long
    GetNewLocation = ComboItemData(cmbLocation)
'    Dim RS As Recordset
'    Set RS = ExecuteStatement("select * from inv_vw_enumerated_values where enum_value = 'Default Plate Location'")
'    GetNewLocation = RS("Value")
End Function

Private Sub Form_Unload(Cancel As Integer)
    Set moDictFields = Nothing
    Set moCompoundDictFields = Nothing
    Set moContainerDictFields = Nothing
    Set moLocationBarcodeMapping = Nothing
    Set moRegistrationNumberMapping = Nothing
    Set moDictBarcodes = Nothing
    Set moDictGroups = Nothing
End Sub

Private Sub grdBarcodes_ValidateEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
    ' only allow numeric values
    If Not IsNumeric(grdBarcodes.EditText) Then Beep: Cancel = True
End Sub


Private Sub GridFieldDict_ValueChanged(ByVal Row As Long, ByVal Col As Long)
    Dim lDict As Dictionary
    Dim sValue As String
    Dim LongValue As Long
    Dim DoubleValue As Double
    Dim DateValue As Date
    Dim lError As Long
    Dim sErrMsg As String
    
    With grdFieldDict
        Set lDict = Nothing
        sValue = Trim(.TextMatrix(Row, Col))
        .TextMatrix(Row, Col) = sValue
        If Len(sValue) = 0 Then
            Exit Sub
        End If
            
        Select Case miLoadOption
            Case ePlates
                Set lDict = moDictFields
            Case eStructures
                Set lDict = moCompoundDictFields
             Case eContainers
                Set lDict = moContainerDictFields
        End Select
        
        If IsNothing(lDict) Then
            ' Whoops... some added miLoadOption?
            Exit Sub
        End If
        
        sErrMsg = ""
            
        If lDict.Exists(Row) Then
            If TypeName(lDict(Row)) = "DBField" Then
                Select Case lDict(Row).eFieldType
                    Case eText
                        ' no restrictions
                    Case eInteger
                        On Error Resume Next
                        LongValue = CLng(sValue)
                        lError = Err.Number
                        On Error GoTo 0
                        .TextMatrix(Row, Col) = LongValue
                        If lError = 13 Then
                            sErrMsg = "Please enter an integer value for the " & lDict(Row).DisplayName & " field."
                        End If
                    Case eReal
                        On Error Resume Next
                        DoubleValue = CDbl(sValue)
                        lError = Err.Number
                        On Error GoTo 0
                        .TextMatrix(Row, Col) = DoubleValue
                        If lError = 13 Then
                            sErrMsg = "Please enter a numeric value for the " & lDict(Row).DisplayName & " field."
                        End If
                    Case eDate
                        On Error Resume Next
                        DateValue = Format(sValue, moApplicationVariablesDict("DATE_FORMAT_STRING"))
                        'CDate(sValue)
                        lError = Err.Number
                        On Error GoTo 0
                        .TextMatrix(Row, Col) = CStr(Format(DateValue, moApplicationVariablesDict("DATE_FORMAT_STRING")))
                        If lError = 13 Then
                            sErrMsg = "Please enter a valid date value for the " & lDict(Row).DisplayName & " field."
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
    Dim dict As Dictionary
    Dim FieldType
    Dim sErrorMessage
    Dim sFieldPrefix As String
    Dim sFieldType As String
    Dim bError As Boolean
        
    If miLoadOption = ePlates Then
        Set dict = moDictFields
    ElseIf miLoadOption = eStructures Then
        Set dict = moCompoundDictFields
     ElseIf miLoadOption = eContainers Then
        Set dict = moContainerDictFields
    End If
    If TypeName(dict(Row)) <> "DBField" Then
        Exit Sub
    End If
    
    bError = False
        
    With grdFieldDict
        sMapping = .TextMatrix(Row, eMapping)
        If sMapping <> "1000" Then
            ' Only validate rows that correspond to database fields
            If Len(dict(Row).FieldName) > 0 Then
                FieldType = moCFWImporter.GetFieldType(sMapping)
                
                Select Case dict(Row).eFieldType
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
        sErrorMessage = sErrorMessage & dict(Row).DisplayName & ", " & sFieldPrefix & sFieldType & " field, is being loaded from a " & GetADOFieldTypeString(CLng(FieldType)) & " field." & vbLf
        sErrorMessage = sErrorMessage & "This may cause the import to fail.  Please validate the data to ensure it contains only " & sFieldType & " records."
        MsgBox sErrorMessage, vbExclamation
    End If
End Sub

    
Private Sub grdFieldDict_CellChanged(ByVal Row As Long, ByVal Col As Long)
    If bLoadMappingsFromXML Then
        Exit Sub
    End If
    
    Select Case (Col)
        Case eMapping
            GridFieldDict_MappingChanged Row, Col
        Case eValue
            GridFieldDict_ValueChanged Row, Col
    End Select
End Sub


Private Sub grdRegOptions_AfterEdit(ByVal Row As Long, ByVal Col As Long)
    Dim sName As String
    Dim rField As RegField
    If Col = 1 Then
        sName = grdRegOptions.Cell(flexcpText, Row, 0)
        Set rField = RegFieldByName(sName)
        If Not (rField Is Nothing) Then
            If rField.Picklist <> "" Then
                rField.value = grdRegOptions.ComboData
            Else
                rField.value = grdRegOptions.value
            End If
        End If
    End If
End Sub

Private Sub grdRegOptions_BeforeEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
    Dim sName As String
    Dim rField As RegField
    If Col = 1 Then
        sName = grdRegOptions.Cell(flexcpText, Row, 0)
        Set rField = RegFieldByName(sName)
        If Not (rField Is Nothing) Then GetRegFieldPicklist rField
        grdRegOptions.ComboList = rField.Picklist
    End If
End Sub

Function RegFieldByName(sName As String) As RegField
    Dim i As Integer
    Dim rField As RegField
    For i = 1 To eRegOptionCount - 1
        Set rField = moRegOptionFields(i)
        If rField.DisplayName = sName Then
            Set RegFieldByName = rField
            Exit Function
        End If
    Next i
End Function

Private Sub GetRegFieldPicklist(rField As RegField)
    Dim sXML As String
    Dim sMsg As String
    Dim sList As String
    
    If Not rField.PickListLoaded Then
        If Len(rField.PicklistName) > 0 Then
            sXML = HTTPRequest("POST", msServerName, mbUseSSL, RegApiFolder & "/GetPicklistXML.asp", "", _
              "picklist=" & rField.PicklistName & "&USRCODE=" & msUserID & "&AUTHENTICATE=" & msPassword)
            sMsg = PicklistFromXML(sXML, sList, True)
            If sMsg = "" Then
                rField.Picklist = sList
            Else
                MsgBox "Error building " & rField.PicklistName & " picklist: " & sMsg
            End If
        End If
        rField.PickListLoaded = True
    End If
End Sub

Private Function CreateContainerXML() As String
        Dim bNewContainer As Boolean
        bNewContainer = True
        Dim lContainerFld As Long
        If Not moContainerDictFields(eCBarcode).SDFileField = "" Then
            lContainerFld = eBarcode
        End If
        If chkLog.value = 1 Then
            LogAction "<INV_LOADER TIME_STAMP=""" & Now & """>", xmlLogPath
        End If
        Dim bUpdated As Boolean
        Dim i As Long: i = 0
        Dim XMLContainer As DOMDocument
        Dim oContainerNode As IXMLDOMElement
        Dim oOptionalNode As IXMLDOMElement
        Dim objPI As IXMLDOMProcessingInstruction
        Dim oRootNode As IXMLDOMElement
        Dim oNode As IXMLDOMElement
        Dim msUserID, msPassword, ServerName
        msUserID = txtUserID.Text
        msPassword = txtPassword.Text
        ServerName = txtServer.Text
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
        Dim ContainerId, UOM, initialQty, LocationID, containerTypeID, containerStatusID, qtyMax, UOP, UOW, UOC, UOD, CompoundID, BarcodeDescId, OwnerID, SupplierID, SolventID
        Dim sLocationBarcode As String
        Set rs = moCFWImporter.Recordset2(moCFWImporter.SelectSTR)
        ContainerId = 1
         Set XMLContainer = New DOMDocument60
                With XMLContainer
                    Set objPI = XMLContainer.createProcessingInstruction("xml", "version=""1.0""")
                    .appendChild objPI
                    Set oRootNode = .createElement("CONTAINERS")
                     With oRootNode
                       '     .setAttribute "CSUSERNAME", msUserID
                       '     .setAttribute "CSUSERID", msPassword
                            .setAttribute "REGSERVER", ServerName
                    End With
                    Set .documentElement = oRootNode
                    End With
        Do Until rs.EOF
                
                bNewContainer = False
                bUpdated = False
                lblOverallProg.Caption = "Container " & progOverall.value - 1 & " of " & mlNumCompounds
                With XMLContainer
                    Set oContainerNode = XMLContainer.createElement("CREATECONTAINER")
                    oRootNode.appendChild oContainerNode
                    
                    With oContainerNode
                        .setAttribute "ID", ContainerId
                        bHasCompoundID = False
                        CompoundID = GetProperty(eCompoundIdFk, rs)
                        If (CompoundID <> "" And Not IsNull(CompoundID)) Then
                            .setAttribute "COMPOUNDID", CompoundID
                            bHasCompoundID = True
                        End If
                        
                        If GetProperty(eRegNumber, rs) <> "" And Not IsNull(GetProperty(eRegNumber, rs)) And bHasCompoundID <> True Then
                            sRegistrationNumber = GetProperty(eRegNumber, rs)
                            If moRegistrationNumberMapping.Exists(sRegistrationNumber) Then
                                RegID = moRegistrationNumberMapping.Item(sRegistrationNumber)
                            End If
                            .setAttribute "REGID", RegID
                            .setAttribute "BATCHNUMBER", GetProperty(eBatchNumberFk, rs)
                             bHasCompoundID = True
                        End If
                         UOM = GetProperty(eUnitOfMeasIdFk, rs)
                         If UOM <> "" And Not IsNull(UOM) Then
                            UOM = LookUpValue("INV_UNITS", CStr(UOM))
                            If Not IsNumeric(UOM) Then UOM = ""
                         End If
                         If cmbContainerUOM.Enabled Then
                            UOM = cmbContainerUOM.ItemData(cmbContainerUOM.ListIndex)
                         End If
                         .setAttribute "UOMID", UOM
                        initialQty = GetProperty(eQtyInitial, rs)
                        If initialQty = "" Or IsNull(initialQty) Then
                            initialQty = 0
                        End If
                        .setAttribute "INITIALQTY", initialQty
                        .setAttribute "QTYREMAINING", initialQty
                        
                        'If Not moLocationBarcodeMapping.Exists(sBarcode) Then
                                'moLocationBarcodeMapping.Add sBarcode, sLocationID
                            'End If
                        
                        If cmbContainerLocations.Enabled Then
                            LocationID = cmbContainerLocations.ItemData(cmbContainerLocations.ListIndex)
                        Else
                            sLocationBarcode = GetProperty(eLocationBarcode, rs)
                            ' Go look up the actual location ID from the barcode.  This was populated
                            ' in CheckValues()
                            If moLocationBarcodeMapping.Exists(sLocationBarcode) Then
                                LocationID = moLocationBarcodeMapping.Item(sLocationBarcode)
                            End If
                        End If
                        .setAttribute "LOCATIONID", LocationID
                        
                        containerTypeID = GetProperty(eContainerTypeIdFk, rs)
                        If containerTypeID <> "" And Not IsNull(containerTypeID) Then
                           containerTypeID = LookUpValue("INV_CONTAINER_TYPES", CStr(containerTypeID))
                           If Not IsNumeric(containerTypeID) Then containerTypeID = ""
                        End If
                        If cmbContainerType.Enabled Then
                            containerTypeID = cmbContainerType.ItemData(cmbContainerType.ListIndex)
                        End If
                        .setAttribute "CONTAINERTYPEID", containerTypeID
                        containerStatusID = GetProperty(eContainerStatusIdFk, rs)
                        If containerStatusID <> "" And Not IsNull(containerStatusID) Then
                           containerStatusID = LookUpValue("INV_CONTAINER_STATUS", CStr(containerStatusID))
                           If Not IsNumeric(containerStatusID) Then containerStatusID = ""
                        End If
                        If cmbContainerStatus.Enabled Then
                            containerStatusID = cmbContainerStatus.ItemData(cmbContainerStatus.ListIndex)
                        End If
                        .setAttribute "CONTAINERSTATUSID", containerStatusID
                        qtyMax = GetProperty(eQtyMax, rs)
                        If qtyMax = "" Or IsNull(qtyMax) Then
                            If initialQty = 0 Then
                                qtyMax = 1
                            Else
                                qtyMax = initialQty
                            End If
                        End If
                        .setAttribute "QTY_MAX", qtyMax
                    End With
                    Set oOptionalNode = XMLContainer.createElement("OPTIONALPARAMS")
                    oContainerNode.appendChild oOptionalNode
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "CONTAINERNAME", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eContainerName, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "CONTAINERDESC", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eContainerDescription, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "MINSTOCKQTY", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eQtyMinstock, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "MAXSTOCKQTY", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eQtyMaxstock, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "EXPDATE", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDateExpires, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATEORDERED", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDateOrdered, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATERECEIVED", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDateReceived, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "PURITY", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(ePURITY, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "SOLVENTIDFK", "")
                    SolventID = GetProperty(eSolventIdFk, rs)
                    If SolventID <> "" And Not IsNull(SolventID) Then
                       SolventID = LookUpValue("INV_SOLVENTS", CStr(SolventID))
                       If Not IsNumeric(SolventID) Then SolventID = ""
                    End If
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = SolventID
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "CONCENTRATION", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eCConcentration, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "GRADE", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eGrade, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "TAREWEIGHT", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eTareWeight, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FINALWEIGHT", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eFinalWght, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "NETWEIGHT", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eNetWght, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DENSITY", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eCDensity, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "COMMENTS", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eContainerComments, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "STORAGECONDITIONS", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eStorageConditions, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "HANDLINGPROCEDURES", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eHandlingProcedures, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "LOTNUM", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eLotNum, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "SUPPLIERID", "")
                    oOptionalNode.appendChild oNode1
                    SupplierID = GetProperty(eSupplierIdFk, rs)
                    If SupplierID <> "" And Not IsNull(SupplierID) Then
                       SupplierID = LookUpValue("INV_SUPPLIERS", CStr(SupplierID))
                       If Not IsNumeric(SupplierID) Then SupplierID = ""
                    End If
                    oNode1.Text = SupplierID
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "SUPPLIERCATNUM", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eSupplierCatnum, rs)
                    
                     Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATEPRODUCED", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDateProduced, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "CONTAINERCOST", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eContainerCost, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "BARCODE", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eCBarcode, rs)
                     
                     Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "PONUMBER", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(ePoNumber, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "POLINENUMBER", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(ePoLineNumber, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "REQNUMBER", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eReqNumber, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "NUMCOPIES", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = "1"
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_1", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField1, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_2", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField2, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_3", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField3, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_4", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField4, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_5", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField5, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_6", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField6, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_7", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField7, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_8", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField8, rs)
                   
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_9", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField9, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "FIELD_10", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eField10, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATE_1", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDate1, rs)
                        
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATE_2", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDate2, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATE_3", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDate3, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATE_4", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDate4, rs)
                    
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "DATE_5", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = GetProperty(eDate5, rs)
                      
                    UOP = GetProperty(eUnitOfPurityIdFk, rs)
                    If UOP <> "" And Not IsNull(UOP) Then
                            UOP = LookUpValue("INV_UNITS", CStr(UOP))
                            If Not IsNumeric(UOP) Then UOP = ""
                    End If
                    If cmbContainerUOP.ListIndex <> -1 And cmbContainerUOP.Enabled = True Then
                       UOP = cmbContainerUOP.ItemData(cmbContainerUOP.ListIndex)
                    End If
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "UOPID", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = UOP
                        
                    UOW = GetProperty(eUnitOfWghtIdFk, rs)
                    If UOW <> "" And Not IsNull(UOW) Then
                        UOW = LookUpValue("INV_UNITS", CStr(UOW))
                        If Not IsNumeric(UOW) Then UOW = ""
                    End If
                    If cmbContainerUOW.ListIndex <> -1 And cmbContainerUOW.Enabled = True Then
                        UOW = cmbContainerUOW.ItemData(cmbContainerUOW.ListIndex)
                    End If
                     Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "UOWID", "")
                     oOptionalNode.appendChild oNode1
                     oNode1.Text = UOW
                    UOC = GetProperty(eUnitOfConcIdFk, rs)
                    If UOC <> "" And Not IsNull(UOC) Then
                        UOC = LookUpValue("INV_UNITS", CStr(UOC))
                        If Not IsNumeric(UOC) Then UOC = ""
                    End If
                    If cmbContainerUOC.ListIndex <> -1 And cmbContainerUOC.Enabled = True Then
                       UOC = cmbContainerUOC.ItemData(cmbContainerUOC.ListIndex)
                    End If
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "UOCID", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = UOC
                        
                    UOD = GetProperty(eUnitOfDensityIdFk, rs)
                    If UOD <> "" And Not IsNull(UOD) Then
                        UOD = LookUpValue("INV_UNITS", CStr(UOD))
                        If Not IsNumeric(UOD) Then UOD = ""
                    End If
                    If cmbContainerUOD.ListIndex <> -1 And cmbContainerUOD.Enabled = True Then
                       UOD = cmbContainerUOD.ItemData(cmbContainerUOD.ListIndex)
                    End If
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "UODID", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = UOD
                      
                    If cmbContainerBarcodeDesc.ListIndex <> -1 And cmbContainerBarcodeDesc.Enabled = True Then
                       BarcodeDescId = cmbContainerBarcodeDesc.ItemData(cmbContainerBarcodeDesc.ListIndex)
                       Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "BARCODEDESCID", "")
                       oOptionalNode.appendChild oNode1
                       oNode1.Text = BarcodeDescId
                    End If
                    
                    OwnerID = GetProperty(eOwnerIdFk, rs)
                    If cmbContainerOwner.ListIndex <> -1 And cmbContainerOwner.Enabled = True Then
                       OwnerID = oOwner(cmbContainerOwner.ItemData(cmbContainerOwner.ListIndex)).PrimaryKey
                    End If
                    Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "OWNERID", "")
                    oOptionalNode.appendChild oNode1
                    oNode1.Text = OwnerID
                End With            ' With XMLContainer
                
                ' If we have a compound ID, then don't add the compound information... it already
                ' exists in Inventory
                If mbShowRegister And Not bHasCompoundID Then
                    sBase64Cdx = moCFWImporter.Base64CDX(rs)
                    If sBase64Cdx = "53 File not found" Or sBase64Cdx = "53+File+not+found" Then sBase64Cdx = ""
                    
                    If mbRegister Then
                        ' add compound node
                        If sBase64Cdx <> "" Then
                            Set oNode = XMLContainer.createElement("REGISTERSUBSTANCE")
                            oRootNode.appendChild oNode
                            With oNode
                                .setAttribute "PACKAGEID", ContainerId
                                For i = 1 To eRegOptionCount - 1
                                    If moRegOptionFields(i).AttributeName <> "" Then
                                    .setAttribute moRegOptionFields(i).AttributeName, moRegOptionFields(i).value
                                End If
                                Next i
'                               Replaced by loop above
'                                .setAttribute "REGPARAMETER", moRegOptionFields(eDupAction)
'                                .setAttribute "SEQUENCE", moRegOptionFields(ePrefix)
'                                .setAttribute "PROJECT", moRegOptionFields(eProject)
'                                .setAttribute "COMPOUND", moRegOptionFields(eCompound)
'                                .setAttribute "NOTEBOOK", moRegOptionFields(eNotebook)
'                                .setAttribute "SALT", moRegOptionFields(eSalt)
'                                .setAttribute "BATCH_PROJECT", moRegOptionFields(eBatchProject)
'                                .setAttribute "SCIENTIST", moRegOptionFields(eScientist)
'
                                .setAttribute "CHEMICAL_NAME", IIf(Len(GetProperty(eContainerSubstanceName, rs)) > 0, GetProperty(eContainerSubstanceName, rs), "No Name")
                                .setAttribute "CAS", GetProperty(eContainerCAS, rs)
                                .setAttribute "ACX_ID", GetProperty(eContainerACX, rs)
                                If sBase64Cdx <> "" Then
                                    .setAttribute "BASE64_CDX", URLEncode(sBase64Cdx)
                                End If
                                .setAttribute "CLOGP", GetProperty(eContainerCLOGP, rs)
                                .setAttribute "ROTATABLE_BONDS", GetProperty(eContainerRotatableBonds, rs)
                                .setAttribute "TOT_POL_SURF_AREA", GetProperty(eContainerTotPolSurfArea, rs)
                                .setAttribute "HBOND_ACCEPTORS", GetProperty(eContainerhbondacc, rs)
                                .setAttribute "HBOND_DONORS", GetProperty(eContainerhbonddon, rs)
                                ' fill in reg options
                                Dim vkey
                                If Not moRegDict Is Nothing Then
                                    For Each vkey In moRegDict
                                        If TypeName(moRegDict(vkey)) = "DBField" Then
                                            ' only add tag if there is a default other than "" ,or
                                            ' if we are using an SDFile field
                                            If (moRegDict(vkey).UseDefault And moRegDict(vkey).value <> "") _
                                                Or moRegDict(vkey).UseDefault = False Then
                                                .setAttribute moRegDict(vkey).FieldName, GetRegProperty(vkey, rs)
                                            End If
                                        End If
                                    Next
                                End If
                            End With
                            Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "structure", "")
                            oNode1.Text = sBase64Cdx
                            oNode.appendChild oNode1
                                
                       End If
                    Else    ' If mbRegister Then
                        If sBase64Cdx <> "" Then
                            Set oNode = XMLContainer.createElement("CREATESUBSTANCE")
                            oRootNode.appendChild oNode
                            With oNode
                                .setAttribute "PACKAGEID", ContainerId
                                .setAttribute "CAS", GetProperty(eContainerCAS, rs)
                                .setAttribute "ACX_ID", GetProperty(eContainerACX, rs)
                                .setAttribute "CLOGP", GetProperty(eContainerCLOGP, rs)
                                .setAttribute "ROTATABLE_BONDS", GetProperty(eContainerRotatableBonds, rs)
                                .setAttribute "TOT_POL_SURF_AREA", GetProperty(eContainerTotPolSurfArea, rs)
                                .setAttribute "HBOND_ACCEPTORS", GetProperty(eContainerhbondacc, rs)
                                .setAttribute "HBOND_DONORS", GetProperty(eContainerhbonddon, rs)
                                '!supplier_id = GetProperty(esuppliercompoundid, rs)
                                ' fill in reg options
                                '.setAttribute "BASE64_CDX", URLEncode(sBase64Cdx)
                            End With
                            Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "substanceName", "")
                            oNode1.Text = IIf(Len(GetProperty(eContainerSubstanceName, rs)) > 0, GetProperty(eContainerSubstanceName, rs), "No Name")
                            oNode.appendChild oNode1
                            oNode.setAttribute "Type", "BASE64_CDX"
                            
                            Set oNode1 = XMLContainer.createNode(NODE_ELEMENT, "structure", "")
                            oNode1.Text = sBase64Cdx
                            oNode.appendChild oNode1
                        End If
                    End If
                End If
                rs.MoveNext
                progOverall.value = progOverall.value + 1
                progOverall.Refresh
               ' XMLContainer.save "c:\invloader_debug0.xml"
                lblOverallProg.Caption = "Container " & progOverall.value - 1 & " of " & mlNumCompounds
               lblOverallProg.Refresh
                modCloseBtn.EnableCloseButton Me.hWnd, False
                modCloseBtn.EnableCloseButton Me.hWnd, True
                
                ContainerId = ContainerId + 1
        Loop
        lblOverallProg.Caption = "Saving Container data.This may take a few minutes.."
        modCloseBtn.EnableCloseButton Me.hWnd, False
        sRet = UtilsInventory.CreateContainerXML(XMLContainer.xml, mbRegister)
        modCloseBtn.EnableCloseButton Me.hWnd, True
        If gbSaveContainerXML Then
           XMLContainer.save (xmlLogPath & "DebugContainer.xml")
        End If
        If chkLog.value = 1 Then
           LogAction sRet, xmlLogPath
        End If
       ' XMLContainer.save "c:\invloader_debug1.xml"
End Function
Sub FillOwnerCombo(ByRef Combo As ComboBox, ByRef dict As Dictionary)
    Dim aAr As Variant
    
    If Not dict Is Nothing Then
        aAr = dict.Keys
        ReDim oOwner(LBound(aAr) To UBound(aAr)) As OwnerClass
        Combo.Clear
        Dim i As Integer
         For i = LBound(aAr) To UBound(aAr)
            Set oOwner(i) = New OwnerClass
            oOwner(i).PrimaryKey = aAr(i)
            oOwner(i).DisplayName = CnvString(dict.Item(aAr(i)), eDBtoVB)
            Combo.AddItem oOwner(i).DisplayName, Combo.ListCount    ' add at last position
            Combo.ItemData(Combo.ListCount - 1) = i
        Next
    End If
End Sub

