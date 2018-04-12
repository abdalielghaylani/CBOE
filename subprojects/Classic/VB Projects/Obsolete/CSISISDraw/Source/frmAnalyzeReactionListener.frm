VERSION 5.00
Begin VB.Form frmAnalyzeReactionListener 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Form1"
   ClientHeight    =   1545
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   4080
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1545
   ScaleWidth      =   4080
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  'CenterScreen
   Begin VB.CommandButton CancelButton 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   375
      Left            =   2760
      TabIndex        =   3
      Top             =   1080
      Width           =   1215
   End
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   1440
      TabIndex        =   2
      Top             =   1080
      Width           =   1215
   End
   Begin VB.ComboBox mReactantsFieldComboBox 
      Height          =   315
      Left            =   1440
      Style           =   2  'Dropdown List
      TabIndex        =   1
      Top             =   120
      Width           =   2535
   End
   Begin VB.ComboBox mProductsFieldComboBox 
      Height          =   315
      Left            =   1440
      Style           =   2  'Dropdown List
      TabIndex        =   0
      Top             =   600
      Width           =   2535
   End
   Begin VB.Label mReactantsFieldLabel 
      Alignment       =   1  'Right Justify
      Caption         =   "Reactants Field:"
      Height          =   255
      Left            =   120
      TabIndex        =   5
      Top             =   180
      Width           =   1215
   End
   Begin VB.Label mProductsFieldLabel 
      Alignment       =   1  'Right Justify
      Caption         =   "Products Field:"
      Height          =   255
      Left            =   120
      TabIndex        =   4
      Top             =   660
      Width           =   1215
   End
End
Attribute VB_Name = "frmAnalyzeReactionListener"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' ##MODULE_SUMMARY A frmReactionToolProperties form is used to specify the properties associated _
with a ReactionFormTool implementation.
' ##MODULE_REMARKS The frmReactionToolProperties form appears when processing the IENFormTool_Properties method _
of the ReactionFormTool class.

' ##IGNORE_REFERENCES mConnection

Private mIsOK As Boolean
Private mConnection As ENFramework9.Connection

Public Property Get IsOK() As Boolean
    IsOK = mIsOK
End Property

Public Property Get NewReactantsFieldKey() As Long
    If (mReactantsFieldComboBox.ListIndex = -1) Then
        NewReactantsFieldKey = 0
    Else
        NewReactantsFieldKey = mReactantsFieldComboBox.ItemData(mReactantsFieldComboBox.ListIndex)
    End If
End Property

Public Property Get NewProductsFieldKey() As Long
    If (mProductsFieldComboBox.ListIndex = -1) Then
        NewProductsFieldKey = 0
    Else
        NewProductsFieldKey = mProductsFieldComboBox.ItemData(mProductsFieldComboBox.ListIndex)
    End If
End Property

Friend Sub Initialize(ByVal FieldListener As IENFieldListener, ByVal SectionType As ENFramework9.SectionType, ByVal ReactantsFieldKey As Long, ByVal ProductsFieldKey As Long, ByVal Connection As ENFramework9.Connection)
    Me.Caption = FieldListener.Name & " Properties"
    Dim f As ENFramework9.Field
    Dim i As Long
    
    Set mConnection = Connection

    mReactantsFieldComboBox.Clear
    For i = 1 To SectionType.Fields.count
        Set f = SectionType.Fields.Item(i)
        If (f.FieldType.FactoryProgID = "ENStandard9.TableFactory") Then
            mReactantsFieldComboBox.AddItem f.Name
            mReactantsFieldComboBox.ItemData(mReactantsFieldComboBox.NewIndex) = f.Key
            If (f.Key = ReactantsFieldKey) Then
                mReactantsFieldComboBox.ListIndex = mReactantsFieldComboBox.NewIndex
            End If
        End If
    Next i

    mProductsFieldComboBox.Clear
    For i = 1 To SectionType.Fields.count
        Set f = SectionType.Fields.Item(i)
        If (f.FieldType.FactoryProgID = "ENStandard9.TableFactory") Then
            mProductsFieldComboBox.AddItem f.Name
            mProductsFieldComboBox.ItemData(mProductsFieldComboBox.NewIndex) = f.Key
            If (f.Key = ProductsFieldKey) Then
                mProductsFieldComboBox.ListIndex = mProductsFieldComboBox.NewIndex
            End If
        End If
    Next i
End Sub

Private Sub CancelButton_Click()
    mConnection.WriteLogStream "==>> Standard Add-ins: frmAnalyzeReactionListener.CancelButton_Click: Cancel Button Clicked"
    
    Me.Hide
End Sub

Private Sub Form_Load()
    SetProperFont mReactantsFieldComboBox.Font
    SetProperFont mProductsFieldComboBox.Font
    
    mIsOK = False
End Sub

Private Sub OKButton_Click()
    mConnection.WriteLogStream "==>> Standard Add-ins: frmAnalyzeReactionListener.OKButton_Click: OK Button Clicked"
    
    mIsOK = True
    Me.Hide
End Sub





