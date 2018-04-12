VERSION 5.00
Object = "{BDA04258-9536-4779-A86A-49164A7741D7}#9.0#0"; "ChemDrawCtl9.dll"
Begin VB.Form frmEditStructure 
   Caption         =   "Edit Structure"
   ClientHeight    =   4560
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   4800
   ControlBox      =   0   'False
   LinkTopic       =   "Form1"
   ScaleHeight     =   4560
   ScaleWidth      =   4800
   StartUpPosition =   2  'CenterScreen
   Begin ChemDrawControl9Ctl.ChemDrawCtl mCDXDrawing 
      Height          =   3855
      Left            =   0
      TabIndex        =   2
      Top             =   0
      Width           =   4695
      _cx             =   8281
      _cy             =   6800
      SourceURL       =   ""
      DataURL         =   ""
      ViewOnly        =   0   'False
      DontCache       =   0   'False
      ShowToolsWhenVisible=   0   'False
      AuthenticateURL =   ""
      DataEncoded     =   -1  'True
      ShrinkToFit     =   -1  'True
      EnlargeToFit    =   0   'False
      WrapReactionsToFit=   0   'False
      RecenterWhenFitting=   0   'False
      BorderColor     =   0
      BorderVisible   =   0   'False
      BorderWidth     =   1
      OpenDoesImport  =   -1  'True
   End
   Begin VB.CommandButton CancelButton 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   375
      Left            =   3480
      TabIndex        =   1
      Top             =   4080
      Width           =   1215
   End
   Begin VB.CommandButton OKButton 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   2160
      TabIndex        =   0
      Top             =   4080
      Width           =   1215
   End
End
Attribute VB_Name = "frmEditStructure"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

' ##MODULE_SUMMARY A form used to edit a chemical structure associated with a table cell.

' ##MODULE_REMARKS A frmEditStructure is also used to calculating the molecular formula, _
molecular weight or a picture of a chemical structure using the ChemDraw Active-X control _
contained within.

' ##IGNORE_REFERENCES mConnection

Public OK As Boolean
Private mButtonTopSpacing As Single
Private mButtonRightSpacing As Single
'Private mConnection As ENFramework9.Connection

Private Sub CancelButton_Click()
    'mConnection.WriteLogStream "==>> Standard Add-ins: frmEditStructure.CancelButton_Click: Cancel Button Clicked"
    
    OK = False
    mCDXDrawing.ShowToolsWhenVisible = False
    Me.Hide
End Sub

Private Sub Form_Activate()
    ' In some unknown conditions, the SetFocus command generates Error #5. These errors
    ' seem to be harmless and can be ignored. This is a hack.
    On Error Resume Next
    mCDXDrawing.SetFocus
    mCDXDrawing.ShowToolsWhenVisible = True
End Sub

Private Sub mCDXDrawing_DataChanged()
    OKButton.Enabled = True
End Sub

Private Sub OKButton_Click()
    'mConnection.WriteLogStream "==>> Standard Add-ins: frmEditStructure.OKButton_Click: OK Button Clicked"
    
    OK = True
    mCDXDrawing.ShowToolsWhenVisible = False
    Me.Hide
End Sub

Public Property Get CDXStructure() As String
    If (InStr(mCDXDrawing.Version, " Net") > 0) Then
        Err.Raise vbObjectError + 513, "ChemicalStructureCtl.IENFieldCtl_SectionCell", "chemical structures can not be saved using the ChemDraw Net Control. Please activate ChemDraw using a serial number for ChemDraw Pro Control"
    End If
    
    CDXStructure = mCDXDrawing.Data("cdxml")
End Property

Friend Property Get StructurePicture(Optional ByVal Extension As String = "bmp", Optional ByVal GraphicsOutputBorder As Long = 1) As IPictureDisp
    Dim FilePath As String
    
    FilePath = GetTempFile(Extension)
    
    WriteStructurePicture FilePath, Extension, GraphicsOutputBorder
        
    Set StructurePicture = LoadPicture(FilePath)
    Kill FilePath
End Property

Friend Sub WriteStructurePicture(ByVal FilePath As String, Optional ByVal Extension As String = "bmp", Optional ByVal GraphicsOutputBorder As Long = 1)
    Dim oldEncoded As Boolean
    Dim oldAnitaliasedGIFs As Boolean
    Dim oldGraphicsOutputBorder As Long
    
    oldEncoded = mCDXDrawing.DataEncoded
    oldAnitaliasedGIFs = mCDXDrawing.Application.Preferences.AntialiasedGIFs
    mCDXDrawing.DataEncoded = False
    mCDXDrawing.Application.Preferences.AntialiasedGIFs = False
    oldGraphicsOutputBorder = mCDXDrawing.Application.Preferences.GraphicsOutputBorder
    mCDXDrawing.Application.Preferences.GraphicsOutputBorder = GraphicsOutputBorder
    WriteBytes FilePath, mCDXDrawing.Data(Extension)
    mCDXDrawing.DataEncoded = oldEncoded
    mCDXDrawing.Application.Preferences.AntialiasedGIFs = oldAnitaliasedGIFs
    mCDXDrawing.Application.Preferences.GraphicsOutputBorder = oldGraphicsOutputBorder
End Sub

Friend Property Get Formula() As String
    Formula = mCDXDrawing.Objects.Formula
End Property

Friend Property Get ExactMass() As Double
    ExactMass = mCDXDrawing.Objects.ExactMass
End Property

Friend Property Get MolecularWeight() As Double
    MolecularWeight = mCDXDrawing.Objects.MolecularWeight
End Property

Friend Sub Initialize() 'ByVal Connection As ENFramework9.Connection)
    'Set mConnection = Connection
End Sub

Public Property Let CDXStructure(aData As String)
    Dim oldEncoded As Boolean
    
    oldEncoded = mCDXDrawing.DataEncoded
    mCDXDrawing.DataEncoded = True
    
    mCDXDrawing.Objects.Clear
    If (Len(aData) > 0) Then
        mCDXDrawing.SourceURL = "data:chemical/x-cdx;base64," & aData
    End If
    mCDXDrawing.DataEncoded = oldEncoded
    
    mCDXDrawing.DataChanged = False
    OKButton.Enabled = False
End Property

Public Property Get ReactionSchemes() As ChemDrawControl9Ctl.ReactionSchemes
    Set ReactionSchemes = mCDXDrawing.ReactionSchemes
End Property

Public Property Let IsEditable(ByVal aData As Boolean)
    mCDXDrawing.ViewOnly = Not aData
End Property

Public Function ExtractComponents() As ReactionComponent()
    Dim retval() As ReactionComponent
    
    Dim rs As ChemDrawControl9Ctl.ReactionScheme
    Dim rStep As ChemDrawControl9Ctl.ReactionStep
    Dim g As ChemDrawControl9Ctl.Group
    Dim iReactants%, iProducts%, iIndex%
    Dim iSolvents As Long
    Dim j As Long

    For Each rs In ReactionSchemes
        iReactants = iReactants + rs.Reactants.Count
        iProducts = iProducts + rs.Products.Count
        For Each rStep In rs.ReactionSteps
            iSolvents = iSolvents + rStep.GroupsBelowArrow.Count
        Next rStep
    Next rs
    
    If iReactants > 0 Or iProducts > 0 Or iSolvents > 0 Then
        ReDim retval(1 To iReactants + iProducts + iSolvents)
        
        iIndex = 1
        For Each rs In ReactionSchemes
            For Each g In rs.Reactants
                j = iIndex - 1
                Do While (j >= 1)
                    If (retval(j).Top >= g.Bounds.Bottom) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    ElseIf (retval(j).Bottom <= g.Bounds.Top) Then
                        Exit Do
                    ElseIf (retval(j).Left >= g.Bounds.Right) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    ElseIf (retval(j).Right <= g.Bounds.Left) Then
                        Exit Do
                    ElseIf (retval(j).Left + retval(j).Right >= g.Bounds.Left + g.Bounds.Right) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    Else
                        Exit Do
                    End If
                Loop
                j = j + 1
                With g.Objects
                    Set retval(j) = New ReactionComponent
                    retval(j).MolecularWeight = .MolecularWeight / 1000# ' Convert to SI units (kg/mol)
                    retval(j).Formula = .Formula
                    retval(j).IsReactant = True
                    retval(j).IsProduct = False
                    retval(j).IsSolvent = False
                    retval(j).ID = g.ID
                    retval(j).Top = g.Bounds.Top
                    retval(j).Bottom = g.Bounds.Bottom
                    retval(j).Right = g.Bounds.Right
                    retval(j).Left = g.Bounds.Left
                    retval(j).code = CanonicalCode(g)
                    ' Old code:  retval(j).Code = g.Objects.Data("chemical/x-smiles")
                End With
                iIndex = iIndex + 1
            Next g
            
            For Each g In rs.Products
                j = iIndex - 1
                Do While (j >= 1)
                    If (Not retval(j).IsProduct) Then
                        Exit Do
                    ElseIf (retval(j).Top >= g.Bounds.Bottom) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    ElseIf (retval(j).Bottom <= g.Bounds.Top) Then
                        Exit Do
                    ElseIf (retval(j).Left >= g.Bounds.Right) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    ElseIf (retval(j).Right <= g.Bounds.Left) Then
                        Exit Do
                    ElseIf (retval(j).Left + retval(j).Right >= g.Bounds.Left + g.Bounds.Right) Then
                        Set retval(j + 1) = retval(j)
                        j = j - 1
                    Else
                        Exit Do
                    End If
                Loop
                j = j + 1
                With g.Objects
                    Set retval(j) = New ReactionComponent
                    retval(j).MolecularWeight = .MolecularWeight / 1000# ' Convert to SI units (kg/mol)
                    retval(j).Formula = .Formula
                    retval(j).IsReactant = False
                    retval(j).IsProduct = True
                    retval(j).IsSolvent = False
                    retval(j).ID = g.ID
                    retval(j).Top = g.Bounds.Top
                    retval(j).Bottom = g.Bounds.Bottom
                    retval(j).Right = g.Bounds.Right
                    retval(j).Left = g.Bounds.Left
                    retval(j).code = CanonicalCode(g)
                    ' Old code:  retval(j).Code = g.Objects.Data("chemical/x-smiles")
                End With
                iIndex = iIndex + 1
            Next g
        Next rs
        
        For Each rs In ReactionSchemes
            For Each rStep In rs.ReactionSteps
                For Each g In rStep.GroupsBelowArrow
                    j = iIndex - 1
                    Do While (j >= 1)
                        If (Not retval(j).IsSolvent) Then
                            Exit Do
                        ElseIf (retval(j).Top >= g.Bounds.Bottom) Then
                            Set retval(j + 1) = retval(j)
                            j = j - 1
                        ElseIf (retval(j).Bottom <= g.Bounds.Top) Then
                            Exit Do
                        ElseIf (retval(j).Left >= g.Bounds.Right) Then
                            Set retval(j + 1) = retval(j)
                            j = j - 1
                        ElseIf (retval(j).Right <= g.Bounds.Left) Then
                            Exit Do
                        ElseIf (retval(j).Left + retval(j).Right >= g.Bounds.Left + g.Bounds.Right) Then
                            Set retval(j + 1) = retval(j)
                            j = j - 1
                        Else
                            Exit Do
                        End If
                    Loop
                    j = j + 1
                    With g.Objects
                        Set retval(j) = New ReactionComponent
                        retval(j).MolecularWeight = .MolecularWeight / 1000# ' Convert to SI units (kg/mol)
                        retval(j).Formula = .Formula
                        retval(j).IsReactant = False
                        retval(j).IsProduct = False
                        retval(j).IsSolvent = True
                        retval(j).ID = g.ID
                        retval(j).Top = g.Bounds.Top
                        retval(j).Bottom = g.Bounds.Bottom
                        retval(j).Right = g.Bounds.Right
                        retval(j).Left = g.Bounds.Left
                        retval(j).code = CanonicalCode(g)
                        ' Old code:  retval(j).Code = g.Objects.Data("chemical/x-smiles")
                    End With
                    iIndex = iIndex + 1
                Next g
            Next rStep
        Next rs
    Else
        ReDim retval(0, 0)
        ' Err.Raise vbObjectError + 513, , "the drawing can not be interpreted as a reaction"
    End If
    
    ExtractComponents = retval
End Function

Public Function ExtractFragment(ByVal ID As Long) As String
    Dim g As ChemDrawControl9Ctl.Group

    For Each g In mCDXDrawing.Groups
        If (g.ID = ID) Then
            ExtractFragment = g.Objects.Data("cdxml")
            Exit Function
        End If
    Next g
    
    ExtractFragment = ""
End Function

Private Sub Form_Load()
    mButtonTopSpacing = Me.ScaleHeight - OKButton.Top
    mButtonRightSpacing = Me.ScaleWidth - CancelButton.Width - CancelButton.Left
End Sub

Private Sub Form_Resize()
    'mConnection.WriteLogStream "==>> Standard Add-ins: frmEditStructure.Form_Resize: Form Resized"
    
    OKButton.Top = Me.ScaleHeight - mButtonTopSpacing
    CancelButton.Top = OKButton.Top
    CancelButton.Left = Me.ScaleWidth - mButtonRightSpacing - CancelButton.Width
    OKButton.Left = CancelButton.Left - mButtonRightSpacing - OKButton.Width
    
    If (Me.ScaleWidth - (2 * mCDXDrawing.Left) > 0) Then
        mCDXDrawing.Width = Me.ScaleWidth - (2 * mCDXDrawing.Left)
    Else
        mCDXDrawing.Width = 0
    End If
    
    If (OKButton.Top - mCDXDrawing.Top - mButtonTopSpacing + OKButton.Height > 0) Then
        mCDXDrawing.Height = OKButton.Top - mCDXDrawing.Top - mButtonTopSpacing + OKButton.Height
    Else
        mCDXDrawing.Height = 0
    End If
End Sub


