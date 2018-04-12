VERSION 5.00
Object = "{2FA1F512-E754-101B-8BA0-0020AF04BB6D}#2.30#0"; "mdlbox.ocx"
Begin VB.Form frmStructure 
   Caption         =   "Form1"
   ClientHeight    =   4230
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   5520
   Icon            =   "frmStructure.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   4230
   ScaleWidth      =   5520
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer Timer1 
      Left            =   360
      Top             =   3240
   End
   Begin MDLBoxLib.MDLbox mdlStructure 
      Height          =   2895
      Left            =   0
      TabIndex        =   0
      Top             =   0
      Width           =   3975
      _Version        =   131102
      _ExtentX        =   7011
      _ExtentY        =   5106
      _StockProps     =   41
      BackColor       =   -2147483643
      BorderStyle     =   1
      Hydrogens       =   1
      BeginProperty MoleculeFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "Arial"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      EnableDataEdit  =   -1  'True
      AutoPaste       =   0   'False
      Name            =   "mdlStructure"
      ScrollBarWidth  =   16
      ApplicationName =   "MDL Form Box Control"
      ContainerName   =   ""
      ShowSequenceBonds=   -1  'True
   End
End
Attribute VB_Name = "frmStructure"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Public Event EditReady(ByVal vStruct As Variant, ByRef DataSgroups() As DataSgroupData)
Private mOutputFormat As String

Private mFileCount As Long

'## This structure stores the properties of a Data Sgroup.

Public Sub Initialize(ByVal ApplicationName As String)
    Dim TitleBar As String
    Dim TitleBarFound As String
    
    With Me.mdlStructure
        '.FieldType = FIELDTYPE_STRUCT       ' Required to make mdlbox act as structure container
        .DataOrQuery = DISPLAY_QUERY         ' Probably DISPLAY_DATA would have been OK too, but is not really relevant here
        .AutoPaste = False                  ' Required to get the mdlStructure_DataFromStructureEdit event
        
        ' This allows the icon to appear in ISIS/Draw. Normally this needs to be the same
        ' as App.Title, but because this is a dll, it must be the same as that of the calling
        ' application.
        
        ' The title bar of the E-Notebook container application has to contain this string:
        TitleBar = "Internet Explorer" ' This works for IE.
        If WindowExists(TitleBar, False, TitleBarFound) Then
            .ApplicationName = TitleBarFound
            gENWindowName = TitleBarFound
        Else
            TitleBar = "CambridgeSoft E-Notebook" ' This works for the ENClientRemote client.
            If WindowExists(TitleBar, False, TitleBarFound) Then
                .ApplicationName = TitleBarFound
                gENWindowName = TitleBarFound
            Else
                TitleBar = "E-Notebook" ' This works for the Internet Explorer client (which includes this string in it's title bar).
                If WindowExists(TitleBar, False, TitleBarFound) Then
                    .ApplicationName = TitleBarFound
                    gENWindowName = TitleBarFound
                End If
            End If
        End If
    End With
End Sub

Friend Function EditStructure(vStruct As Variant, inputFormat As String, outputFormat As String, _
                        dsd() As DataSgroupData, ByVal NReactantsCD As Long, ByVal NProductsCD As Long, _
                        ByVal MWReactant1 As Double, ByVal MWProduct1 As Double)
    Dim lSkId As Long
    Dim strRxnfile As String
    Dim skcStructure() As Byte
    Dim lSkId1 As Long
    
    If goObjlib Is Nothing Then
        Set goObjlib = CreateObject("isisbase32")
    End If
    
    mOutputFormat = outputFormat
    If inputFormat = "rxn" Then
        mdlStructure.FieldType = FIELDTYPE_RXN
        strRxnfile = vStruct
        mdlStructure.Data = lGetSkidFromRxn(strRxnfile)
    ElseIf inputFormat = "mol" Then
        mdlStructure.FieldType = FIELDTYPE_STRUCT
        strRxnfile = vStruct
        mdlStructure.Data = lGetSkidFromMol(strRxnfile)
    ElseIf inputFormat = "skc" Then
        mdlStructure.FieldType = FIELDTYPE_SKETCH
        skcStructure = vStruct
        'Call DebugWriteFiles("Before Edit - file written by ChemDraw", 0, vStruct)
        lSkId1 = lGetSkidFromSkc(skcStructure)
        Call DebugWriteFiles("Before Edit 1 - before removing plus signs", lSkId1)
        RemoveAllPlusSigns (lSkId1)
        Call DebugWriteFiles("Before Edit 2 - before loading mdlbox", lSkId1)
        mdlStructure.Data = lSkId1
    Else
        ' error condtition - file type not found.
    End If

    lSkId = mdlStructure.Data
    Call DebugWriteFiles("Before Edit 3 - read back from mdlbox", lSkId)
        
'    ' Ungroup top-level objects.
'    Call DebugWriteFiles("Before Edit - before Ungroup", lSkId)
'
    Dim SaveSketchMode As Long
    Dim TotalSkObjs As Long
    Dim TotalSkObjs1 As Long
    Dim NumRxnMolecules As Long
    Dim i As Long
    Dim k As Long
    Dim sj As Long
    Dim molId As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim TotalAtoms As Long
    Dim MolFormula As String
    Dim MolWeight As Double
    Dim MolWeightDD As String
    Dim ArrowSkObjID As Long
    Dim ArrowEndType As Long
    Dim ArrowStyle As Long
    Dim ArrowLeft As Double
    Dim ArrowTop As Double
    Dim ArrowRight As Double
    Dim ArrowBottom As Double
    Dim ArrowPresent As Boolean
    Dim MolsToUngroup(1 To 10) As Long
    Dim NumMolsToUngroup As Long
    Dim Left As Double
    Dim Top As Double
    Dim Right As Double
    Dim Bottom As Double
    Dim NISISReactants As Long
    Dim NISISProducts As Long
    Dim ReactantMolId As Long
    Dim ProductMolId As Long
    Dim ReactantSkObjId As Long
    Dim ProductSkObjId As Long
    Dim lSkId2 As Long
    Dim N As Long
    Dim IsReaction As Boolean
    Dim NoSgroupsFound As Boolean
    Dim NMoleculesCD As Long ' Number of molecules, as found in ChemDraw.
    Dim NMoleculesID As Long ' Number of molecules, as found in the ISIS/Draw sketch.
    Dim MW As Double
    Dim MWdifference As Double

    With goObjlib
        SaveSketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        Call .SetSketchMode(lSkId, SKMODE_MOL)
        
        ' Code to deal with the bug in the isislib.dll that causes reactants and products to be spontaneously grouped:
        
        ' Get arrow location and style (we may need to put it back (as ungrouping can cause it to vanish!)).
        TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
        For i = 1 To TotalSkObjs 'TotalSkObjs
            skObjId = .GetSkObjId(lSkId, i)
            skObjType = .GetSkObjType(skObjId)
            If skObjType = SKOBJTYPE_RXNARROW Then
                ArrowSkObjID = skObjId
                ArrowEndType = .GetSkObjArrowEndPoints(skObjId)
                ArrowStyle = .GetSkObjArrowStyle(skObjId)
                Call .GetSkObjBox(skObjId, ArrowLeft, ArrowTop, ArrowRight, ArrowBottom)
            End If
        Next i
            
        ' Get the number of molecules.
        NMoleculesCD = NReactantsCD + NProductsCD ' as found in ChemDraw...
        ' (Note: NReactantsCD and NProductsCD are computed according to whether the molecule X
        '  centers are left or right of the righthand end of the arrow.)
        
        TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW, SKOBJTYPE_GROUP = 10
        NMoleculesID = 0
        For i = 1 To TotalSkObjs 'TotalSkObjs
            skObjId = .GetSkObjId(lSkId, i)
            skObjType = .GetSkObjType(skObjId)
            If ((skObjType = SKOBJTYPE_MOL) Or (skObjType = SKOBJTYPE_GROUP)) Then
                NMoleculesID = NMoleculesID + 1  ' as found in ISIS/Draw...
            End If
        Next i
        
        ' Test for spontaneous grouping.
        If (NMoleculesCD > NMoleculesID) Then
            ' We have it:  spontaneous grouping
            ' Note that the simple case, where the rection is A + B + C -> D + E + F, and the grouping is of
            ' {A, B, and C} and {D, E, and F) is not the only case.
            ' If there are reactants above or below the arrow, they can get grouped into either the product
            '   group or the reactant group (and we don't know ISIS's rules for that).
        
            IsReaction = False
            If .IsSketchReaction(lSkId) Then
                IsReaction = True
                
                ' Find number of reactants and products (according to ISIS/Draw's parser).
                NISISReactants = 0
                NISISProducts = 0
                NumRxnMolecules = .GetRxnComponentTotal(lSkId)
                For i = 1 To NumRxnMolecules
                    If (.GetRxnComponentType(lSkId, i) = RXN_REACTANT) Then 'RXN_REACTANT=1 or RXN_PRODUCT=2
                        NISISReactants = NISISReactants + 1
                        ReactantSkObjId = .GetRxnComponentSkObjId(lSkId, i)
                        ReactantMolId = .GetMolIdOfSkObj(ReactantSkObjId)
                    End If
                    If (.GetRxnComponentType(lSkId, i) = RXN_PRODUCT) Then
                        NISISProducts = NISISProducts + 1
                        ProductSkObjId = .GetRxnComponentSkObjId(lSkId, i)
                        ProductMolId = .GetMolIdOfSkObj(ProductSkObjId)
                    End If
                Next i
                If (NISISReactants = 1) Then
                    ' We don't want to ungroup a salt molecule.  Thus this test:  If (NReactantsCD = 1) and the
                    ' molecular weight of reactants is the same in ID and CD, then we don't ungroup it.
                    
                    ' Note that in some cases salts at this point SKOBJTYPE_GROUP (they are converted to SKOBJTYPE_MOL below),
                    ' so that protects them here.
                    
                    MW = GetTotalMolWt(ReactantMolId, ReactantSkObjId)
                    MWdifference = Abs(MW - MWReactant1)
                    If (Not ((NReactantsCD = 1) And (MWdifference < 1#))) Then
                        ' This is the case where the isislib has grouped our reactants, because they don't have plus signs.
                        ' Ungroup them.
                        skObjType = .GetSkObjType(ReactantSkObjId) 'SKOBJTYPE_GROUP = 10
                        If skObjType = SKOBJTYPE_MOL Then
                            Call .SelectSkObj(ReactantSkObjId)
                            Call .UnGroupSelectedMolObj(lSkId)
                            Call .DeselectAllSkObj(lSkId)
                        End If
                    End If
                End If
                If (NISISProducts = 1) Then
                    ' We don't want to ungroup a salt molecule.  Thus this test:  If (NReactantsCD = 1) and the
                    ' molecular weight of reactants is the same in ID and CD, then we don't ungroup it.
                    MW = GetTotalMolWt(ProductMolId, ProductSkObjId)
                    MWdifference = Abs(MW - MWProduct1)
                    If (Not ((NProductsCD = 1) And (MWdifference < 1#))) Then
                        ' This is the case where the isislib has grouped our reactants, because they don't have plus signs.
                        ' Ungroup them.
                        skObjType = .GetSkObjType(ProductSkObjId) 'SKOBJTYPE_GROUP = 10
                        If skObjType = SKOBJTYPE_MOL Then
                            Call .SelectSkObj(ProductSkObjId)
                            Call .UnGroupSelectedMolObj(lSkId)
                            Call .DeselectAllSkObj(lSkId)
                        End If
                    End If
                End If
            End If ' .IsSketchReaction(lSkId)
        End If ' (NMoleculesCD > NMoleculesID)
                       
             ' Old code that doesn't use Reaction APIs:
'            TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
'            NumMolsToUngroup = 0
'            For i = 1 To TotalSkObjs 'TotalSkObjs
'                skObjId = .GetSkObjId(lSkId, i)
'                skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
'                If skObjType = SKOBJTYPE_MOL Then
'                    molId = .GetMolIdOfSkObj(skObjId)
'                    ' Scale in a standard way.
'                    'Call .LayoutMol(molId, 0#)
'                    totalAtoms = .GetTotalAtoms(molId)
'                    MolFormula = .GetMolFormula(molId)
'                    MolWeight = .GetMolWeight(molId)
'                    MolWeightDD = .GetMolWeightDotDisconnect(molId)
'                    ' Ungroup MOL if it contains more than one fragment.
'                    If InStr(1, MolWeightDD, " ", vbTextCompare) <> 0 Then
'                        NumMolsToUngroup = NumMolsToUngroup + 1
'                        MolsToUngroup(NumMolsToUngroup) = skObjId
'                    End If
'                End If
'                If skObjType = SKOBJTYPE_RXNARROW Then
'                    ArrowSkObjID = skObjId
'                    ArrowEndType = .GetSkObjArrowEndPoints(skObjId)
'                    ArrowStyle = .GetSkObjArrowStyle(skObjId)
'                    Call .GetSkObjBox(skObjId, ArrowLeft, ArrowTop, ArrowRight, ArrowBottom)
'                End If
'            Next i
'
'            For i = 1 To NumMolsToUngroup
'                Call .SelectSkObj(MolsToUngroup(i))
'                Call .UnGroupSelectedMolObj(lSkId)
'                Call .DeselectAllSkObj(lSkId)
'            Next i

            ' Convert Subsketches to mols.
            Do
                TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
                For i = 1 To TotalSkObjs 'TotalSkObjs
                    skObjId = .GetSkObjId(lSkId, i)
                    skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                    If skObjType = SKOBJTYPE_GROUP Then
                        Call .SelectSkObj(skObjId)
                        Call .UngroupSelectedSkObj(lSkId)
                        skObjId = .GroupSelectedMolObj(lSkId)
                        Call .DeselectAllSkObj(lSkId)
                    End If
                Next i
                
                ' Check to see if there are any subsketches that didn't get converted to mols (found to happen in testing).
                NoSgroupsFound = True
                For i = 1 To TotalSkObjs 'TotalSkObjs
                    skObjId = .GetSkObjId(lSkId, i)
                    skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                    If skObjType = SKOBJTYPE_GROUP Then
                        NoSgroupsFound = False
                        Exit For
                    End If
                Next i
            Loop Until NoSgroupsFound
            Call DebugWriteFiles("Before edit 3.5 - After converting subsketches to mols", lSkId)
        
        If IsReaction Then
            ' Do we still have a reaction arrow?  If not, put it back in.
            ArrowPresent = False
            TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
            For i = 1 To TotalSkObjs 'TotalSkObjs
                skObjId = .GetSkObjId(lSkId, i)
                skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                If skObjType = SKOBJTYPE_RXNARROW Then
                    ArrowPresent = True
                End If
            Next i
            If Not ArrowPresent Then
                ' Recreate arrow.
                ArrowSkObjID = .CreateLine(lSkId, ArrowLeft, ArrowTop, ArrowRight, ArrowBottom)
                Call .SetSkObjArrowEndPoints(ArrowSkObjID, ArrowEndType)
                Call .SetSkObjArrowStyle(ArrowSkObjID, ArrowStyle)
            End If
            
        End If ' IsReaction
        
        ' Move sketch to upper right corner of box.
        Call .GetSketchBoundingBox(lSkId, Left, Top, Right, Bottom)
        ' Partial fix to CSBR-57318:
        Call .MoveSketch(lSkId, -Left, -Top + (Bottom - Top) / 2)
        ' Old code: Call .MoveSketch(lSkId, -Left + (Right - Left) / 8, -Top + (Bottom - Top) / 2)
        
        ' Clean up the molecules - creates problems -- see below.
'        'If .IsSketchReaction(lSkId) Then
'            TotalSkObjs = .GetTotalSkObjs(lSkId)
'            NumMolsToUngroup = 0
'            For i = 1 To TotalSkObjs 'TotalSkObjs
'                skObjId = .GetSkObjId(lSkId, i)
'                skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
'                If skObjType = SKOBJTYPE_MOL Then
'                    molId = .GetMolIdOfSkObj(skObjId)
'                    Call .LayoutMol(molId, 0.7)
'                End If
'            Next i
'        'End If

'        Not useful as it removes sepatates the reactants above and below the arrow from the arrow.
'        If .IsSketchReaction(lSkId) Then
'            Call .LayoutRxn(lSkId, True, 0#, 0#, 0#, 0#, 0#)
'        End If

        'Call .SetSketchDisplayAtomNumbers(lSkId, ATOM_NUMBERS_OFF)
        Call .SetSketchMode(lSkId, SaveSketchMode)
    End With

' OLD CODE:
'            NumRxnMolecules = .GetRxnComponentTotal(lSkId)
'            For i = 1 To NumRxnMolecules
'                If ((.GetRxnComponentType(lSkId, i) = RXN_REACTANT) _
'                    Or (.GetRxnComponentType(lSkId, i) = RXN_PRODUCT)) Then 'RXN_REACTANT=1 or RXN_PRODUCT=2
'                    skObjId = .GetRxnComponentSkObjId(lSkId, i)
'                    molId = .GetMolIdOfSkObj(skObjId)
'                    TotalAtoms = .GetTotalAtoms(molId)
'                    MolFormula = .GetMolFormula(molId)
'                    MolWeight = .GetMolWeight(molId)
'                    MolWeightDD = .GetMolWeightDotDisconnect(molId)
'                    Call .SelectSkObj(skObjId)
'                    Call .UnGroupSelectedMolObj(lSkId)
'                    Call .DeselectAllSkObj(lSkId)
'                End If
'            Next i

'                    TotalSkObjs1 = .GetTotalSkObjs(sj)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
'                    For k = 1 To TotalSkObjs1
'                        skObjId = .GetSkObjId(sj, k)
'                        skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
'                    '.GetTotalAtomsInMolCollection(skObjId,1)
'                    Next k
                    'If skObjType = SKOBJTYPE_ATOM Then 'SKOBJTYPE_ATOM = 33
                    '    sj = .GetAtomNumber(skObjId)
                    '.SelectSkObj (skObjId)
                    'End If
    ' Add Data Sgroups To Sketch.
    Call DebugWriteFiles("Before edit 4 - before Add Data Sgroups To Sketch", lSkId)
    Call AddDataSgroupsToISISSketch(lSkId, dsd)
    
    Call DebugWriteFiles("Before edit 5 - after Add Data Sgroups To Sketch", lSkId)
    
    Call goObjlib.LayoutChiralFlag(lSkId)  ' Put Chiral Flag in a good place.
    Call DebugWriteFiles("Before edit 6 - after Layout Chiral Flag - before EditData", lSkId)
    
    If Not mdlStructure.EditData(lSkId) Then
        MsgBox "Cannot start ISIS/Draw", vbApplicationModal + vbExclamation + vbOKOnly, "ISIS/Draw error"
        ' From a dll, this should probably not be a message box
    End If
    goObjlib.DeleteSketch lSkId 'otherwise ISIS memory leak !!
    goObjlib.DeleteSketch lSkId1 'otherwise ISIS memory leak !!
End Function

Private Sub mdlStructure_DataFromStructureEdit(ByVal lSkId As Long)
    Dim strFile As String
    Dim bFile() As Byte
    Dim vStruct As Variant
    Dim molId As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim DataSgroups() As DataSgroupData

    'Call DebugWriteFiles("After edit 1 - Before scaling molecules", lSkId) ' for debugging
    'RemoveAllPlusSigns (lSkId)
    'DeleteIdenticalStructures (lSkId)
    
'    ' Set each molecule's standard bond length.
'    With goObjlib
'        TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
'        For i = 1 To TotalSkObjs 'TotalSkObjs
'            skObjId = .GetSkObjId(lSkId, i)
'            skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
'            If skObjType = SKOBJTYPE_MOL Then
'                molId = .GetMolIdOfSkObj(skObjId)
'                ' The following line of code has no effect???
'                'Call .SetDefaultMolFont(Arial, 2400, FONT_PLAIN)
'                Call .LayoutMol(molId, 0#)
'            End If
'        Next i
'        Call .SetSketchMode(lSkId, SKMODE_MOL)
'    End With

'    Note:  Don't do this.  While this works to shut down ISIS/Draw, it does so violently, and generates a GPF error.
'    mdlStructure.CancelEditData ' Fix bug CSBR-57570.  Forces ISIS/Draw to close if it hasn't already.

    ' Fix bug CSBR-57570.  If we get here before ISIS/Draw has closed, we may be in another thread which needs its own object pointer.
    If goObjlib Is Nothing Then
        Set goObjlib = CreateObject("isisbase32")
    End If
    
    Call DebugWriteFiles("After edit 2 - Before extracting Data Sgroups", lSkId) ' for debugging
    DataSgroups = ExtractDataSgroupsFromISISSketch(lSkId)
    Call DebugWriteFiles("After edit 3 - After extracting Data Sgroups", lSkId) ' for debugging
    RemoveAllPlusSigns (lSkId)
    Call DebugWriteFiles("After edit 4 - Before extracting sketch", lSkId) ' for debugging
    
    If mOutputFormat = "rxn" Then
        strFile = GetRxnFileString(lSkId)
        vStruct = strFile
    ElseIf mOutputFormat = "mol" Then
        strFile = GetMolFileString(lSkId)
        vStruct = strFile
    ElseIf mOutputFormat = "skc" Then
        bFile = GetSkcFileByteArray(lSkId) ' This function sets the SKC_MODE.
        vStruct = bFile
    Else
        ' error condtition - file type not found.
    End If
    
'    Call DebugWriteFiles("After edit 3 - Before extracting Data Sgroups", lSkId) ' for debugging
'    DataSgroups = ExtractDataSgroupsFromISISSketch(lSkId)
'    Call DebugWriteFiles("After edit 4 - After extracting Data Sgroups", lSkId) ' for debugging

'    Dim i As Long
'    Open "c:\IDdebug\" & "SkcFileByteArray" & CStr(mFileCount) & ".txt" For Output As #1
'    For i = LBound(bFile) To UBound(bFile)
'        Print #1, bFile(i)
'    Next i
'    Close #1
    
    goObjlib.DeleteSketch lSkId 'otherwise ISIS memory leak !!
    Set goObjlib = Nothing
    
    RaiseEvent EditReady(vStruct, DataSgroups) ' Send structure back to the ISISDrawCSListener.
End Sub

Private Sub RemoveAllPlusSigns(ByVal s As Long)
    Dim SaveSketchMode As Long
    Dim TotalSkObjs As Long
    Dim TotalSkObjs1 As Long
    Dim NumRxnMolecules As Long
    Dim i As Long
    Dim k As Long
    Dim sj As Long
    Dim mj As Long
    Dim skObjId As Long
    Dim skObjType As Long

    With goObjlib
        SaveSketchMode = .GetSketchMode(s) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        Call .SetSketchMode(s, SKMODE_MOL)
            TotalSkObjs = .GetTotalSkObjs(s)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
            For i = TotalSkObjs To 1 Step -1 'TotalSkObjs
                skObjId = .GetSkObjId(s, i)
                skObjType = .GetSkObjType(skObjId)
                If skObjType = SKOBJTYPE_TEXT And .GetSkObjText(skObjId) = "+" Then
                     On Error Resume Next ' Ignore error for this statement.
                     .DeleteSkObj (skObjId)
                     On Error GoTo 0
                End If
            Next i
        Call .SetSketchMode(s, SaveSketchMode)
    End With
End Sub

' No longer used but kept here for possible future use.
'Private Sub DeleteIdenticalStructures(ByVal s As Long)
'    Dim TotalSkObjs As Long
'    Dim skObjType As Long
'    Dim Success As Boolean
'    Dim i As Long
'    Dim j As Long
'    Dim k As Long
'    Dim skObjId As Long
'    Dim IsReaction As Boolean
'    Dim NumRxnMolecules As Long
'    Dim SaveSketchMode As Long
'    Dim IsReactant As Boolean
'    Dim molId As Long
'    Dim RxnComponentType As Long
'    Dim si As Long
'    Dim sj As Long
'    Dim AreEqual As Boolean
'    Dim mi As Long
'    Dim mj As Long
'    Dim lleft As Double
'    Dim ttop As Double
'    Dim rright As Double
'    Dim bbottom As Double
'    Dim lleft1 As Double
'    Dim ttop1 As Double
'    Dim rright1 As Double
'    Dim bbottom1 As Double
'    Dim ClosestPlusskObjId As Long
'    Dim DistanceLeft As Double
'    Dim ProductsAndReactants As Long
'    Dim AnyRemoval As Boolean
'
'    With goObjlib
'
'        SaveSketchMode = .GetSketchMode(s) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
'        Call .SetSketchMode(s, SKMODE_MOL) ' This mode is required by the Rxn APIs we are using below.
'        If .IsSketchReaction(s) Then
'            AnyRemoval = False
'            For ProductsAndReactants = RXN_REACTANT To RXN_PRODUCT
'                Do
'                    TotalSkObjs = .GetTotalSkObjs(s)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
'                    NumRxnMolecules = .GetRxnComponentTotal(s)
'                    AreEqual = False ' Becomes true if a duplicate molecule is found
'                    For i = 1 To NumRxnMolecules
'                        If .GetRxnComponentType(s, i) = ProductsAndReactants Then
'                            si = .GetRxnComponentSkObjId(s, i)
'                            mi = .GetMolIdOfSkObj(si)
'                            ' Look for an identical reactant to delete.
'                            For j = i + 1 To NumRxnMolecules
'                                If .GetRxnComponentType(s, j) = ProductsAndReactants Then
'                                    sj = .GetRxnComponentSkObjId(s, j)
'                                    mj = .GetMolIdOfSkObj(sj)
'                                    AreEqual = .AreMoleculesEqual(mi, mj)
'                                    If AreEqual Then
'                                        ' Determine position
'                                        Call .GetSkObjBox(sj, lleft, ttop, rright, bbottom)
'                                        .DeleteSkObj (sj)
'                                        ' Now delete closest plus sign to the left.
'                                        ClosestPlusskObjId = 0
'                                        DistanceLeft = 100000
'                                        For k = 1 To TotalSkObjs
'                                            skObjId = .GetSkObjId(s, k)
'                                            skObjType = .GetSkObjType(skObjId)
'                                            If skObjType = SKOBJTYPE_TEXT And .GetSkObjText(skObjId) = "+" Then
'                                                Call .GetSkObjBox(skObjId, lleft1, ttop1, rright1, bbottom1)
'                                                ' The first line is to get the closest to the left.
'                                                If lleft - rright1 > 0 And lleft - rright1 < DistanceLeft Then
'                                                    DistanceLeft = lleft - rright1
'                                                    ClosestPlusskObjId = skObjId
'                                                End If
'                                            End If
'                                        Next k
'                                        If ClosestPlusskObjId > 0 Then
'                                            .DeleteSkObj (ClosestPlusskObjId)
'                                        End If
'                                    End If
'                                End If
'                            Next j
'                        End If
'                        If AreEqual Then
'                            AnyRemoval = True
'                            Exit For
'                        End If
'                    Next i
'                Loop Until Not AreEqual
'            Next ProductsAndReactants
'        End If
'        Call .SetSketchMode(s, SaveSketchMode)
'        If AnyRemoval Then
'            MsgBox "Duplicate reactants and/or products were removed.", vbInformation, "ISIS/Draw Integration"
'        End If
'    End With
'
'                                                ' The first line is to get the closest to the left.
'                                                ' The second two lines are to select properly for the components
'                                                '    that are above and below the arrow.
'                                                'If lleft - rright1 > 0 And lleft - rright1 < DistanceLeft _
'                                                'And Abs((ttop + bbottom) / 2# - (ttop1 + bbottom1) / 2#) < _
'                                                '   (bbottom - ttop) * 1# _
'                                                    Then
'
'
'End Sub

Private Sub DebugWriteFiles(ByVal NameRoot As String, lSkId As Long, _
                                            Optional ByRef bStruct As Variant = "")
    Dim fso As FileSystemObject
    Dim FileWriteSuccess As Boolean
    Dim FileName As String
    Dim Path As String
    
    Path = "c:\IDdebug\"
    
    On Error GoTo CatchError
    If mFileCount = 0 Then
        ' If this file is not found, we have an error and don't do anything.
        Open Path & "count.txt" For Input As #1
            Input #1, mFileCount
        Close #1
        ' Increment count.
        mFileCount = mFileCount + 1
        Open Path & "count.txt" For Output As #1
            Print #1, mFileCount
        Close #1
    End If
    FileName = NameRoot & " " & CStr(mFileCount)
    
    With goObjlib
        '.SetSketchMode lSkId, SKMODE_MOL ' SKMODE_MOL or SKMODE_SKETCH or SKMODE_CHEM
        'FileWriteSuccess = .WriteSketchFile(Path & fileName & ".skc", lSkId)
    If NameRoot = "Before Edit - file written by ChemDraw" Then
        Call PutStructureToFile(Path & FileName & ".tgf", bStruct)
    Else
            FileWriteSuccess = .WriteTGFFile(Path & FileName & ".tgf", lSkId)
    End If
    
'        ' Put top-level sketch IDs to a file.
'        Dim TotalSkObjs As Long
'        Dim skObjId As Long
'        Dim skObjType As Long
'        Dim molId As Long
'        Dim MolFormula As String
'        Dim fileNum As Integer
'        Dim i As Long
'        Dim SaveSketchMode As Long
'
'        SaveSketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
'        Call .SetSketchMode(lSkId, SKMODE_MOL) ' This mode is required by the Rxn APIs we are using below.
'        TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
'        If TotalSkObjs > 0 Then
'            fileNum = FreeFile
'            Open Path & FileName & ".txt" For Output As fileNum
'            Print #1, "TotalSkObjs = "; TotalSkObjs
'            For i = 1 To TotalSkObjs
'                skObjId = .GetSkObjId(lSkId, i)
'                skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
'                If (skObjType = SKOBJTYPE_MOL) Then
'                    molId = .GetMolIdOfSkObj(skObjId) ' for debugging...
'                    MolFormula = .GetMolFormulaDotDisconnect(molId) ' for debugging...
'                Else
'                    MolFormula = "<not mol>"
'                End If
'                Print #1, "skObjId, MolFormula = "; skObjId; "   "; MolFormula
'            Next i
'            Close fileNum
'        End If
    End With
    
CatchError:
End Sub

Private Sub PutStructureToFile(ByVal FilePath As String, ByRef vStruct As Variant)
    'TEMP:  Read the data in from the file
    Dim NumBytes As Long
    Dim i As Long
    Dim fileNum As Integer
    Dim bStruct() As Byte

    bStruct = vStruct
    fileNum = FreeFile
    Open FilePath For Binary As fileNum
    NumBytes = UBound(bStruct)

    For i = 1 To NumBytes
        Put #fileNum, , bStruct(i)
    Next i

    Close fileNum
    'TEMP ends
End Sub

