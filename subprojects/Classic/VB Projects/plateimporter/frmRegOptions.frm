VERSION 5.00
Object = "{D76D7128-4A96-11D3-BD95-D296DC2DD072}#1.0#0"; "Vsflex7.ocx"
Begin VB.Form frmRegOptions 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "Registration Options"
   ClientHeight    =   3810
   ClientLeft      =   2760
   ClientTop       =   3750
   ClientWidth     =   7965
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   3810
   ScaleWidth      =   7965
   ShowInTaskbar   =   0   'False
   StartUpPosition =   1  'CenterOwner
   Begin VB.CommandButton cmdSaveRegMapping 
      Caption         =   "Save"
      Height          =   375
      Left            =   1320
      TabIndex        =   5
      Top             =   3240
      Width           =   975
   End
   Begin VB.CommandButton cmdLoadRegMapping 
      Caption         =   "Load"
      Height          =   375
      Left            =   240
      TabIndex        =   4
      Top             =   3240
      Width           =   975
   End
   Begin VB.CommandButton CancelButton 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   375
      Left            =   6480
      TabIndex        =   1
      Top             =   3240
      Width           =   1215
   End
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   5160
      TabIndex        =   0
      Top             =   3240
      Width           =   1215
   End
   Begin VSFlex7Ctl.VSFlexGrid grdRegDict 
      Height          =   2772
      Left            =   240
      TabIndex        =   3
      Top             =   360
      Width           =   7452
      _cx             =   13144
      _cy             =   4890
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
      Height          =   372
      Left            =   240
      TabIndex        =   2
      Top             =   120
      Width           =   6252
   End
End
Attribute VB_Name = "frmRegOptions"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Option Explicit

Public Cancel As Boolean
Public moCFWImporter As DataImporter
Private moDictFields As Dictionary

Private Enum EnumRegDBFields
    ePRODUCER = 1
    eNOTEBOOK_PAGE
    eNOTEBOOK_TEXT
    eBATCH_COMMENT
    eCHEMICAL_NAME
    eSYNONYM_R
    eLIT_REF
    ePREPARATION
    eSTORAGE_REQ_AND_WARNINGS
    eSPECTRUM_ID
    eCAS_NUMBER
    eRNO_NUMBER
    eFEMA_GRAS_NUMBER
    eGROUP_CODE
    eBP
    eMP
    eH1NMR
    eC13NMR
    eMS
    eIR
    eGC
    ePHYSICAL_FORM
    eCOLOR
    eFLASHPOINT
    eHPLC
    eOPTICAL_ROTATION
    eREFRACTIVE_INDEX
    eCREATION_DATE
    eSTRUCTURE_COMMENTS_TXT
    eENTRY_DATE
    eLAST_MOD_DATE
    eSALT_NAME
    eSALT_MW
    eSALT_EQUIVALENTS
    eSOLVATE_ID
    eSOLVATE_NAME
    eSOLVATE_MW
    eSOLVATE_EQUIVALENTS
    eFORMULA_WEIGHT
    eBATCH_FORMULA
    eSOURCE
    eVENDOR_NAME
    eVENDOR_ID
    eAMOUNT_UNITS
    ePURITY
    ePURITY_COMMENTS 'CSBR 133538 Modified by : sjacob
    eLC_UV_MS
    eCHN_COMBUSTION
    eUV_SPECTRUM
    eAPPEARANCE
    eLOGD
    eSOLUBILITY
    eCOLLABORATOR_ID
    ePRODUCT_TYPE
    eCHIRAL
    eCLOGP
    eH_BOND_DONORS
    eH_BOND_ACCEPTORS
    eMW_TEXT
    eMF_TEXT
    eAMOUNT
    eDUPLICATE
    eFIELD_1
    eFIELD_2
    eFIELD_3
    eFIELD_4
    eFIELD_5
    eFIELD_6
    eFIELD_7
    eFIELD_8
    eFIELD_9
    eFIELD_10
    eTXT_CMPD_FIELD_1
    eTXT_CMPD_FIELD_2
    eTXT_CMPD_FIELD_3
    eTXT_CMPD_FIELD_4
    eINT_BATCH_FIELD_1
    eINT_BATCH_FIELD_2
    eINT_BATCH_FIELD_3
    eINT_BATCH_FIELD_4
    eINT_BATCH_FIELD_5
    eINT_BATCH_FIELD_6
    eINT_CMPD_FIELD_1
    eINT_CMPD_FIELD_2
    eINT_CMPD_FIELD_3
    eINT_CMPD_FIELD_4
    eREAL_BATCH_FIELD_1
    eREAL_BATCH_FIELD_2
    eREAL_BATCH_FIELD_3
    eREAL_BATCH_FIELD_4
    eREAL_BATCH_FIELD_5
    eREAL_BATCH_FIELD_6
    eREAL_CMPD_FIELD_1
    eREAL_CMPD_FIELD_2
    eREAL_CMPD_FIELD_3
    eREAL_CMPD_FIELD_4
    eDATE_BATCH_FIELD_1
    eDATE_BATCH_FIELD_2
    eDATE_BATCH_FIELD_3
    eDATE_BATCH_FIELD_4
    eDATE_BATCH_FIELD_5
    eDATE_BATCH_FIELD_6
    eDATE_CMPD_FIELD_1
    eDATE_CMPD_FIELD_2
    eDATE_CMPD_FIELD_3
    eDATE_CMPD_FIELD_4
    eLEGACY_REG_NUMBER
    eRegDBFieldCount ' must be last
End Enum

Public Property Get RegDict() As Dictionary
    Set RegDict = moDictFields
End Property

Public Property Set RegDict(ByRef rhs As Dictionary)
    Set moDictFields = rhs
End Property

Public Sub Initialize()
    TransFerData DT_TO_CONTROLS
End Sub

Private Sub FillDictGrid()
    Dim vkey
    With grdRegDict
        .Editable = flexEDKbdMouse
        .ExtendLastCol = True
        .Rows = 0
        .Cols = 4
        .AddItem "Value" & vbTab & "Data Type" & vbTab & "Field Name" & vbTab & "Default"
        For Each vkey In moDictFields
            If TypeName(moDictFields(vkey)) = "DBField" Then
                If moDictFields(vkey).UseDefault Then
                    .AddItem moDictFields(vkey).DisplayName & vbTab & GetFieldTypeAsString(moDictFields(vkey).eFieldType) & vbTab & "1000" & vbTab & moDictFields(vkey).value
                Else
                    .AddItem moDictFields(vkey).DisplayName & vbTab & GetFieldTypeAsString(moDictFields(vkey).eFieldType) & vbTab & moDictFields(vkey).SDFileField & vbTab & ""
                End If
            End If
        Next
        .FixedRows = 1
        .FixedCols = 2
        .ColComboList(eMapping) = GridBuildComboList(moCFWImporter.FieldsDict)
        .AutoSize 0, 3
    End With
End Sub

Private Function InitField(ByVal Table As String, ByVal Field As String, ByVal Display As String, ByVal FieldType As FieldTypeEnum) As DBField
    Set InitField = New DBField
    InitField.TableName = Table
    InitField.FieldName = Field
    InitField.DisplayName = Display
    InitField.eFieldType = FieldType
    InitField.UseDefault = True
End Function

Private Sub SetDBFields()
    ' set up dictionary to allow db fields
    If moDictFields Is Nothing Then
        Set moDictFields = New Dictionary
    End If
    Dim i As Long
    Dim tfield As DBField
    
    For i = 1 To eRegDBFieldCount - 1
        Select Case i
            Case ePRODUCER
                Set tfield = InitField("TEMPORARY_STRUCTURES", "PRODUCER", "Producer", eText)
            Case eNOTEBOOK_PAGE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "NOTEBOOK_PAGE", "Notebook Page", eText)
            Case eNOTEBOOK_TEXT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "NOTEBOOK_TEXT", "Notebook Text", eText)
            Case eBATCH_COMMENT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "BATCH_COMMENT", "Batch Comment", eText)
            Case eCHEMICAL_NAME
                Set tfield = InitField("TEMPORARY_STRUCTURES", "CHEMICAL_NAME", "Chemical Name", eText)
            Case eSYNONYM_R
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SYNONYM_R", "Synonym", eText)
            Case eLIT_REF
                Set tfield = InitField("TEMPORARY_STRUCTURES", "LIT_REF", "Literature Reference", eText)
            Case ePREPARATION
                Set tfield = InitField("TEMPORARY_STRUCTURES", "PREPARATION", "Preparation", eText)
            Case eSTORAGE_REQ_AND_WARNINGS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "STORAGE_REQ_AND_WARNINGS", "Storage Req and Warnings", eText)
            Case eSPECTRUM_ID
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SPECTRUM_ID", "Spectrum ID", eInteger)
            Case eCAS_NUMBER
                Set tfield = InitField("TEMPORARY_STRUCTURES", "CAS_NUMBER", "CAS Number", eText)
            Case eRNO_NUMBER
                Set tfield = InitField("TEMPORARY_STRUCTURES", "RNO_NUMBER", "RNO Number", eText)
            Case eFEMA_GRAS_NUMBER
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FEMA_GRAS_NUMBER", "FEMA GRAS Number", eText)
            Case eGROUP_CODE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "GROUP_CODE", "Group Code", eText)
            Case eBP
                Set tfield = InitField("TEMPORARY_STRUCTURES", "BP", "Boiling Point", eText)
            Case eMP
                Set tfield = InitField("TEMPORARY_STRUCTURES", "MP", "Melting Point", eText)
            Case eH1NMR
                Set tfield = InitField("TEMPORARY_STRUCTURES", "H1NMR", "H1NMR", eText)
            Case eC13NMR
                Set tfield = InitField("TEMPORARY_STRUCTURES", "C13NMR", "C13NMR", eText)
            Case eMS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "MS", "MS", eText)
            Case eIR
                Set tfield = InitField("TEMPORARY_STRUCTURES", "IR", "IR", eText)
            Case eGC
                Set tfield = InitField("TEMPORARY_STRUCTURES", "GC", "GC", eText)
            Case ePHYSICAL_FORM
                Set tfield = InitField("TEMPORARY_STRUCTURES", "PHYSICAL_FORM", "Physical Form", eText)
            Case eCOLOR
                Set tfield = InitField("TEMPORARY_STRUCTURES", "COLOR", "Color", eText)
            Case eFLASHPOINT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FLASHPOINT", "Flashpoint", eText)
            Case eHPLC
                Set tfield = InitField("TEMPORARY_STRUCTURES", "HPLC", "HPLC", eText)
            Case eOPTICAL_ROTATION
                Set tfield = InitField("TEMPORARY_STRUCTURES", "OPTICAL_ROTATION", "Optical Rotation", eText)
            Case eREFRACTIVE_INDEX
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REFRACTIVE_INDEX", "Refractive Index", eText)
            Case eCREATION_DATE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "CREATION_DATE", "Creation Date", eDate)
            Case eSTRUCTURE_COMMENTS_TXT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "STRUCTURE_COMMENTS_TXT", "Structure Comments Text", eText)
            Case eENTRY_DATE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "ENTRY_DATE", "Entry Date", eDate)
            Case eLAST_MOD_DATE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "LAST_MOD_DATE", "Last Modified Date", eDate)
            Case eSALT_NAME
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SALT_NAME", "Salt Name", eText)
            Case eSALT_MW
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SALT_MW", "Salt MW", eReal)
            Case eSALT_EQUIVALENTS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SALT_EQUIVALENTS", "Salt Equivalents", eReal)
            Case eSOLVATE_ID
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SOLVATE_ID", "Solvate ID", eInteger)
            Case eSOLVATE_NAME
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SOLVATE_NAME", "Solvate Name", eText)
            Case eSOLVATE_MW
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SOLVATE_MW", "Solvate MW", eReal)
            Case eSOLVATE_EQUIVALENTS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SOLVATE_EQUIVALENTS", "Solvate Equivalents", eReal)
            Case eFORMULA_WEIGHT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FORMULA_WEIGHT", "Formula Weight", eReal)
            Case eBATCH_FORMULA
                Set tfield = InitField("TEMPORARY_STRUCTURES", "BATCH_FORMULA", "Batch Formula", eText)
            Case eSOURCE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SOURCE", "Source", eText)
            Case eVENDOR_NAME
                Set tfield = InitField("TEMPORARY_STRUCTURES", "VENDOR_NAME", "Vendor Name", eText)
            Case eVENDOR_ID
                Set tfield = InitField("TEMPORARY_STRUCTURES", "VENDOR_ID", "Vendor ID", eText)
            Case eAMOUNT_UNITS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "AMOUNT_UNITS", "Amount Units", eText)
            Case ePURITY
                Set tfield = InitField("TEMPORARY_STRUCTURES", "PURITY", "Purity", eReal)
            'CSBR        : 133538
            'Modified by : sjacob
            'Comments    : Added field to enter the purity comments in Registration Options
            'Start of change
            Case ePURITY_COMMENTS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "PURITY_COMMENTS", "Purity_Comments", eText)
            'End of change
            Case eLC_UV_MS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "LC_UV_MS", "LC_UV_MS", eText)
            Case eCHN_COMBUSTION
                Set tfield = InitField("TEMPORARY_STRUCTURES", "CHN_COMBUSTION", "CHN Combustion", eText)
            Case eUV_SPECTRUM
                Set tfield = InitField("TEMPORARY_STRUCTURES", "UV_SPECTRUM", "UV Spectrum", eText)
            Case eAPPEARANCE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "APPEARANCE", "Appearance", eText)
            Case eLOGD
                Set tfield = InitField("TEMPORARY_STRUCTURES", "LOGD", "LogD", eReal)
            Case eSOLUBILITY
                Set tfield = InitField("TEMPORARY_STRUCTURES", "SOLUBILITY", "Solubility", eText)
            Case eCOLLABORATOR_ID
                Set tfield = InitField("TEMPORARY_STRUCTURES", "COLLABORATOR_ID", "Collaborator ID", eText)
            Case ePRODUCT_TYPE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "PRODUCT_TYPE", "Product Type", eText)
            Case eCHIRAL
                Set tfield = InitField("TEMPORARY_STRUCTURES", "CHIRAL", "Chiral", eText)
            Case eCLOGP
                Set tfield = InitField("TEMPORARY_STRUCTURES", "CLOGP", "CLogP", eReal)
            Case eH_BOND_DONORS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "H_BOND_DONORS", "H Bond Donors", eInteger)
            Case eH_BOND_ACCEPTORS
                Set tfield = InitField("TEMPORARY_STRUCTURES", "H_BOND_ACCEPTORS", "H Bond Acceptors", eInteger)
            Case eMW_TEXT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "MW_TEXT", "MW Text", eText)
            Case eMF_TEXT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "MF_TEXT", "MF Text", eText)
            Case eAMOUNT
                Set tfield = InitField("TEMPORARY_STRUCTURES", "AMOUNT", "Amount", eText)
            Case eDUPLICATE
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DUPLICATE", "Duplicate", eText)
            Case eFIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_1", "Field 1", eText)
            Case eFIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_2", "Field 2", eText)
            Case eFIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_3", "Field 3", eText)
            Case eFIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_4", "Field 4", eText)
            Case eFIELD_5
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_5", "Field 5", eText)
            Case eFIELD_6
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_6", "Field 6", eText)
            Case eFIELD_7
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_7", "Field 7", eText)
            Case eFIELD_8
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_8", "Field 8", eText)
            Case eFIELD_9
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_9", "Field 9", eText)
            Case eFIELD_10
                Set tfield = InitField("TEMPORARY_STRUCTURES", "FIELD_10", "Field 10", eText)
            Case eTXT_CMPD_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "TXT_CMPD_FIELD_1", "Compound Text Field 1", eText)
            Case eTXT_CMPD_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "TXT_CMPD_FIELD_2", "Compound Text Field 2", eText)
            Case eTXT_CMPD_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "TXT_CMPD_FIELD_3", "Compound Text Field 3", eText)
            Case eTXT_CMPD_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "TXT_CMPD_FIELD_4", "Compound Text Field 4", eText)
            Case eINT_BATCH_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_BATCH_FIELD_1", "Batch Integer Field 1", eInteger)
            Case eINT_BATCH_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_BATCH_FIELD_2", "Batch Integer Field 2", eInteger)
            Case eINT_BATCH_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_BATCH_FIELD_3", "Batch Integer Field 3", eInteger)
            Case eINT_BATCH_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_BATCH_FIELD_4", "Batch Integer Field 4", eInteger)
            Case eINT_BATCH_FIELD_5
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_BATCH_FIELD_5", "Batch Integer Field 5", eInteger)
            Case eINT_BATCH_FIELD_6
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_BATCH_FIELD_6", "Batch Integer Field 6", eInteger)
            Case eINT_CMPD_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_CMPD_FIELD_1", "Compound Integer Field 1", eInteger)
            Case eINT_CMPD_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_CMPD_FIELD_2", "Compound Integer Field 2", eInteger)
            Case eINT_CMPD_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_CMPD_FIELD_3", "Compound Integer Field 3", eInteger)
            Case eINT_CMPD_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "INT_CMPD_FIELD_4", "Compound Integer Field 4", eInteger)
            Case eREAL_BATCH_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_BATCH_FIELD_1", "Batch Decimal Field 1", eReal)
            Case eREAL_BATCH_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_BATCH_FIELD_2", "Batch Decimal Field 2", eReal)
            Case eREAL_BATCH_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_BATCH_FIELD_3", "Batch Decimal Field 3", eReal)
            Case eREAL_BATCH_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_BATCH_FIELD_4", "Batch Decimal Field 4", eReal)
            Case eREAL_BATCH_FIELD_5
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_BATCH_FIELD_5", "Batch Decimal Field 5", eReal)
            Case eREAL_BATCH_FIELD_6
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_BATCH_FIELD_6", "Batch Decimal Field 6", eReal)
            Case eREAL_CMPD_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_CMPD_FIELD_1", "Compound Decimal Field 1", eReal)
            Case eREAL_CMPD_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_CMPD_FIELD_2", "Compound Decimal Field 2", eReal)
            Case eREAL_CMPD_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_CMPD_FIELD_3", "Compound Decimal Field 3", eReal)
            Case eREAL_CMPD_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "REAL_CMPD_FIELD_4", "Compound Decimal Field 4", eReal)
            Case eDATE_BATCH_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_BATCH_FIELD_1", "Batch Date Field 1", eDate)
            Case eDATE_BATCH_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_BATCH_FIELD_2", "Batch Date Field 2", eDate)
            Case eDATE_BATCH_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_BATCH_FIELD_3", "Batch Date Field 3", eDate)
            Case eDATE_BATCH_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_BATCH_FIELD_4", "Batch Date Field 4", eDate)
            Case eDATE_BATCH_FIELD_5
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_BATCH_FIELD_5", "Batch Date Field 5", eDate)
            Case eDATE_BATCH_FIELD_6
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_BATCH_FIELD_6", "Batch Date Field 6", eDate)
            Case eDATE_CMPD_FIELD_1
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_CMPD_FIELD_1", "Compound Date Field 1", eDate)
            Case eDATE_CMPD_FIELD_2
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_CMPD_FIELD_2", "Compound Date Field 2", eDate)
            Case eDATE_CMPD_FIELD_3
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_CMPD_FIELD_3", "Compound Date Field 3", eDate)
            Case eDATE_CMPD_FIELD_4
                Set tfield = InitField("TEMPORARY_STRUCTURES", "DATE_CMPD_FIELD_4", "Compound Date Field 4", eDate)
            Case eLEGACY_REG_NUMBER
                Set tfield = InitField("TEMPORARY_STRUCTURES", "LEGACY_REG_NUMBER", "Legacy Reg Number", eText)
            Case Else
                Set tfield = Nothing
        End Select
        moDictFields.Add i, tfield
    Next
End Sub

Private Sub GetValuesFromTable()
    ' get values from the grid
    With grdRegDict
        Dim i As Long, s As String
        For i = 1 To .Rows - 1
            s = .TextMatrix(i, eDisplayName)
            If .TextMatrix(i, eMapping) = "1000" Then
                moDictFields(i).value = .TextMatrix(i, eValue)
                moDictFields(i).UseDefault = True
            Else
                moDictFields(i).SDFileField = .TextMatrix(i, eMapping)
                moDictFields(i).UseDefault = False
            End If
        Next
    End With
End Sub


Private Sub TransFerData(ByVal dir As Long)
    If dir = DT_TO_CONTROLS Then
        FillDictGrid
    Else ' DT_FROM_CONTROLS
        GetValuesFromTable
    End If
End Sub

Private Sub CancelButton_Click()
    Cancel = True
    Me.Hide
End Sub

Private Sub cmdLoadRegMapping_Click()
    Dim NameToIndexMapping As Dictionary
    Set NameToIndexMapping = New Dictionary
    Dim i As Long
    
    For i = 1 To eRegDBFieldCount - 1
        NameToIndexMapping.Add grdRegDict.TextMatrix(i, eDisplayName), i
    Next
    
    Call UtilsMisc.LoadFieldMappings(grdRegDict, moCFWImporter, eRegistration, NameToIndexMapping)
    Set NameToIndexMapping = Nothing
End Sub

Private Sub cmdSaveRegMapping_Click()
    Call UtilsMisc.SaveFieldMappings(grdRegDict, moCFWImporter, eRegistration)
End Sub

Private Sub Form_Load()
    SetDBFields
    bLoadMappingsFromXML = False
End Sub

Private Sub GridRegDict_ValueChanged(ByVal Row As Long, ByVal Col As Long)
    Dim sValue As String
    Dim LongValue As Long
    Dim DoubleValue As Double
    Dim DateValue As Date
    Dim lError As Long
    Dim sErrMsg As String
    
    sValue = Trim(grdRegDict.TextMatrix(Row, Col))
    grdRegDict.TextMatrix(Row, Col) = sValue
    If Len(sValue) = 0 Then
        Exit Sub
    End If
    
    sErrMsg = ""
        
    If moDictFields.Exists(Row) Then
        If TypeName(moDictFields(Row)) = "DBField" Then
            Select Case moDictFields(Row).eFieldType
                Case eText
                    ' no restrictions
                Case eInteger
                    On Error Resume Next
                    LongValue = CLng(sValue)
                    lError = Err.Number
                    On Error GoTo 0
                    grdRegDict.TextMatrix(Row, Col) = LongValue
                    If lError = 13 Then
                        sErrMsg = "Please enter an integer value for the " & moDictFields(Row).DisplayName & " field."
                    End If
                Case eReal
                    On Error Resume Next
                    DoubleValue = CDbl(sValue)
                    lError = Err.Number
                    On Error GoTo 0
                    grdRegDict.TextMatrix(Row, Col) = DoubleValue
                    If lError = 13 Then
                        sErrMsg = "Please enter a numeric value for the " & moDictFields(Row).DisplayName & " field."
                    End If
                Case eDate
                    On Error Resume Next
                    DateValue = Format(sValue, frmCFWDBWiz.moApplicationVariablesDict("DATE_FORMAT_STRING"))
                    lError = Err.Number
                    On Error GoTo 0
                    grdRegDict.TextMatrix(Row, Col) = CStr(Format(DateValue, frmCFWDBWiz.moApplicationVariablesDict("DATE_FORMAT_STRING")))
                    If lError = 13 Then
                        sErrMsg = "Please enter a valid date value for the " & moDictFields(Row).DisplayName & " field."
                    End If
            End Select
        End If
    End If
    
    If sErrMsg <> "" Then
        MsgBox sErrMsg, vbExclamation
        grdRegDict.Select Row, Col
        grdRegDict.EditCell
    End If

End Sub

Private Sub GridRegDict_MappingChanged(ByVal Row As Long, ByVal Col As Long)
    Dim sMapping As String
    Dim FieldType
    Dim sErrorMessage
    Dim sFieldPrefix As String
    Dim sFieldType As String
    Dim bError As Boolean
    
    If TypeName(moDictFields(Row)) <> "DBField" Then
        Exit Sub
    End If
    
    bError = False
        
    With grdRegDict
        sMapping = .TextMatrix(Row, eMapping)
        If sMapping <> "1000" Then
            ' Only validate rows that correspond to database fields
            If Len(moDictFields(Row).FieldName) > 0 Then
                FieldType = moCFWImporter.GetFieldType(sMapping)
                
                Select Case moDictFields(Row).eFieldType
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
                    Case Else
                End Select
            End If
        End If
    End With
    
    If bError Then
        sErrorMessage = "Warning: " & vbLf & vbLf
        sErrorMessage = sErrorMessage & moDictFields(Row).DisplayName & ", " & sFieldPrefix & sFieldType & " field, is being loaded from a ChemFinder " & GetADOFieldTypeString(CLng(FieldType)) & " field." & vbLf
        sErrorMessage = sErrorMessage & "This may cause the import to fail.  Please validate the data to ensure it contains only " & sFieldType & " records."
        '"click on Validate to verify the data will load properly."
        MsgBox sErrorMessage, vbExclamation
    End If
End Sub


Private Sub grdRegDict_CellChanged(ByVal Row As Long, ByVal Col As Long)
    If bLoadMappingsFromXML Then
        Exit Sub
    End If
    
    Select Case (Col)
        Case eMapping
            GridRegDict_MappingChanged Row, Col
        Case eValue
            GridRegDict_ValueChanged Row, Col
    End Select
    
    
End Sub


Private Sub OKButton_Click()
    Cancel = False
    TransFerData DT_FROM_CONTROLS
    Me.Hide
End Sub
