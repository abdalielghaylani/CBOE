VERSION 5.00
Begin VB.Form frmLoadMapping 
   Caption         =   "Load Mapping"
   ClientHeight    =   2430
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   4740
   Icon            =   "frmLoadMapping.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   2430
   ScaleWidth      =   4740
   StartUpPosition =   3  'Windows Default
   Begin VB.ComboBox cmbName 
      Height          =   315
      ItemData        =   "frmLoadMapping.frx":0442
      Left            =   120
      List            =   "frmLoadMapping.frx":0444
      Sorted          =   -1  'True
      Style           =   2  'Dropdown List
      TabIndex        =   2
      Top             =   480
      Width           =   4455
   End
   Begin VB.CommandButton cmdSaveMapping 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   1920
      TabIndex        =   1
      Top             =   1800
      Width           =   1215
   End
   Begin VB.CommandButton cmdCancelSaveMapping 
      Caption         =   "Cancel"
      Height          =   375
      Left            =   3240
      TabIndex        =   0
      Top             =   1800
      Width           =   1215
   End
   Begin VB.Label Label2 
      Caption         =   "*Note: loading a saved mapping template will overwrite any field mappings you have currently selected."
      Height          =   495
      Left            =   120
      TabIndex        =   4
      Top             =   960
      Width           =   4455
   End
   Begin VB.Label Label1 
      Caption         =   "Please select a mapping to load:"
      Height          =   255
      Left            =   120
      TabIndex        =   3
      Top             =   120
      Width           =   4095
   End
End
Attribute VB_Name = "frmLoadMapping"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Public eMappingType As MappingType
Public Cancel As Boolean
Public sMappingName As String

Private Sub cmdCancelSaveMapping_Click()
    Cancel = True
    Me.Hide
End Sub

Private Sub cmdSaveMapping_Click()
    Cancel = False
    sMappingName = cmbName.Text
    Me.Hide
End Sub

Public Sub Initialize()
    Me.Move (Screen.Width - Me.Width) / 2, (Screen.Height - Me.Height) / 2
    Dim oItem
    cmbName.Clear
    For Each oItem In gFieldMappings.Items
        If oItem.eMappingType = eMappingType Then
            cmbName.AddItem (oItem.sName)
        End If
    Next
End Sub
