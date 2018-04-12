VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "StoicCalculator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' ##MODULE_SUMMARY A class used to analyze the contents of a chemical reaction and calculate values in _
stoichiometry tables.

Private Const kNameRow = 0
Private Const kFormulaRow = 1
Private Const kMWRow = 2
Private Const kFormulaMassRow = 3
Private Const kMolesRow = 4
Private Const kEquivalentsRow = 5
Private Const kLoadingRow = 6
Private Const kSampleMassRow = 7
Private Const kReactantMassRow = 8
Private Const kLimitingRow = 9
Private Const kPercentWeightRow = 10
Private Const kMolarityRow = 11
Private Const kVolumeRow = 12
Private Const kDensityRow = 13
Private Const kNumReactantRows = 14

Private Const kActualMassRow = 7
Private Const kTheoreticalAmountRow = 8
Private Const kActualMolesRow = 9
Private Const kYieldRow = 10
Private Const kPurityRow = 11
Private Const kNumProductRows = 12

Private mReactantsCtl As CTableCtl
Private mProductsCtl As CTableCtl

' ## Details() is the Data Sgroup field 'DETAILS' for each component.
Dim mOldDetails() As String
Dim mNewDetails() As String

' ## Stoichiometry() is the Data Sgroup field 'STOICHIOMETRY' for each component.
Dim mOldStoichiometry() As String
Dim mNewStoichiometry() As String
    
Public Sub Initialize(ByVal ReactionCtl As CChemicalStructureCtl, _
                      ByVal ReactantsCtl As CTableCtl, _
                      ByVal ProductsCtl As CTableCtl, _
                      ByVal SolventsCtl As CTableCtl)
    Set mReactantsCtl = ReactantsCtl
    Set mProductsCtl = ProductsCtl
End Sub

Private Function IsCalculatedCell(ByVal TableCell As TableCell) As Boolean
Attribute IsCalculatedCell.VB_Description = "Does this cell need to have its value calculated?"
    ' Does this cell need to have its value calculated?
    
    If (TableCell Is Nothing) Then
        IsCalculatedCell = False
    ElseIf (IsCellEdited(TableCell)) Then
        IsCalculatedCell = False
    Else
        IsCalculatedCell = True
    End If
End Function

Private Sub CheckLimitingRow(ByVal oLimitingRow As TableProperty)
    ' Can't analyze structure without a limiting row.
    If (oLimitingRow Is Nothing) Then
        Err.Raise vbObjectError + 513, Description:="the Limiting property is not present"
    End If
End Sub

Private Sub FindReactantProperties(ByVal reactantsTS As TableSection, tps() As TableProperty)
    Set tps(kNameRow) = reactantsTS.TablePropertyByName("Name")
    Set tps(kFormulaRow) = reactantsTS.TablePropertyByName("Molecular Formula")
    
    Set tps(kLimitingRow) = reactantsTS.TablePropertyByName("Limiting?")
    
    ' Handle either the name Sample Mass or the name Amount for backward compatibility
    Set tps(kSampleMassRow) = reactantsTS.TablePropertyByName("Sample Mass")
    If (tps(kSampleMassRow) Is Nothing) Then Set tps(kSampleMassRow) = reactantsTS.TablePropertyByName("Amount")
    
    Set tps(kReactantMassRow) = reactantsTS.TablePropertyByName("Reactant Mass")
    
    Set tps(kMWRow) = reactantsTS.TablePropertyByName("Molecular Weight")
    Set tps(kFormulaMassRow) = reactantsTS.TablePropertyByName("Formula Mass")
    Set tps(kPercentWeightRow) = reactantsTS.TablePropertyByName("% by Weight")
    
    ' Handle either the name Moles or the name Millimols for backward compatibility
    Set tps(kMolesRow) = reactantsTS.TablePropertyByName("Moles")
    If (tps(kMolesRow) Is Nothing) Then Set tps(kMolesRow) = reactantsTS.TablePropertyByName("Millimols")
    
    Set tps(kEquivalentsRow) = reactantsTS.TablePropertyByName("Equivalents")
    Set tps(kMolarityRow) = reactantsTS.TablePropertyByName("Molarity")
    Set tps(kVolumeRow) = reactantsTS.TablePropertyByName("Volume")
    Set tps(kDensityRow) = reactantsTS.TablePropertyByName("Density")
    Set tps(kLoadingRow) = reactantsTS.TablePropertyByName("Loading")
End Sub

Private Sub FindProductProperties(ByVal productsTS As TableSection, pps() As TableProperty)
    With productsTS
        Set pps(kNameRow) = .TablePropertyByName("Name")
        Set pps(kFormulaRow) = .TablePropertyByName("Molecular Formula")
        
        Set pps(kActualMassRow) = .TablePropertyByName("Actual Mass")
        If (pps(kActualMassRow) Is Nothing) Then Set pps(kActualMassRow) = .TablePropertyByName("Actual Amount")
        If (pps(kActualMassRow) Is Nothing) Then Set pps(kActualMassRow) = .TablePropertyByName("Actual")
        
        Set pps(kYieldRow) = .TablePropertyByName("% Yield")
        Set pps(kPurityRow) = .TablePropertyByName("% Purity")
        Set pps(kMWRow) = .TablePropertyByName("Molecular Weight")
        Set pps(kFormulaMassRow) = .TablePropertyByName("Formula Mass")
        
        ' Handle either the name "Theoretical Amount" or "Expected" for backward compatibility
        Set pps(kTheoreticalAmountRow) = .TablePropertyByName("Theoretical Mass")
        If (pps(kTheoreticalAmountRow) Is Nothing) Then Set pps(kTheoreticalAmountRow) = .TablePropertyByName("Theoretical Amount")
        If (pps(kTheoreticalAmountRow) Is Nothing) Then Set pps(kTheoreticalAmountRow) = .TablePropertyByName("Expected")
        
        ' Handle either the name Moles or the name Millimols for backward compatibility
        Set pps(kMolesRow) = .TablePropertyByName("Theoretical Moles")
        If (pps(kMolesRow) Is Nothing) Then Set pps(kMolesRow) = .TablePropertyByName("Moles")
        If (pps(kMolesRow) Is Nothing) Then Set pps(kMolesRow) = .TablePropertyByName("Millimols")
        
        Set pps(kActualMolesRow) = .TablePropertyByName("Actual Moles")
        Set pps(kEquivalentsRow) = .TablePropertyByName("Equivalents")
        Set pps(kLoadingRow) = .TablePropertyByName("Loading")
    End With
End Sub

Private Property Get SigNumberFormat(ByVal d As Double, ByVal sigDig As Integer) As String
    ' return the format for the number d so that it has sigDig significant digits

    Dim absD As Double
    Dim tFormat As String
    
    Debug.Assert sigDig > 0
    
    absD = Abs(d)
    
    If (absD = 0#) Then
        If (sigDig = 0) Then
            tFormat = "0.0"
        Else
            tFormat = "0." & String(sigDig - 1, "0")
        End If
    ElseIf (absD < 1#) Then
        tFormat = "0."
        While (absD < 1#)
            tFormat = tFormat & "0"
            absD = absD * 10#
        Wend
        tFormat = tFormat & String(sigDig - 1, "0")
    Else
        tFormat = "0"
        While (absD > 10#)
            tFormat = tFormat & "0"
            absD = absD / 10#
        Wend
        If (Len(tFormat) < sigDig) Then
            tFormat = tFormat & "." & String(sigDig - Len(tFormat), "0")
        End If
    End If
    
    SigNumberFormat = tFormat
End Property

Private Function IsCellEdited(ByVal tc As TableCell, Optional ByVal isPositive As Boolean) As Boolean
Attribute IsCellEdited.VB_Description = "return true if the cell has been specified by the user."
    ' ## Return true if the cell has been specified by the user.
    ' ##PARAM isPositive True if the resulting value must be a positive integer for the cell to be usable in other calculations
    If (tc Is Nothing) Then
        IsCellEdited = False
    ElseIf (tc.Tag <> "entered") Then
        IsCellEdited = False
    ElseIf (isPositive And tc.MinValue <= 0) Then
        IsCellEdited = False
    Else
        IsCellEdited = True
    End If
End Function

Private Sub InitializeCell(ByVal CTableCtl As CTableCtl, ByVal tr As TableRow, ByVal tp As TableProperty, ByVal d As Double, Optional ByVal Format As String, Optional ByVal unitString)
Attribute InitializeCell.VB_Description = "If the cell is calculated, then initialize it."
    ' If the cell is calculated, then initialize it.
    Dim tc As TableCell
    
    If (Not tp Is Nothing) Then
        Set tc = tr.TableCell(tp)
        If (IsCalculatedCell(tc)) Then
            If (tc.MinValue <> d Or tc.MaxValue <> d) Then
                CTableCtl.TableCtl.SetNumericTableCellValue tc, d, Format, unitString
            End If
        End If
    End If
End Sub

Private Sub LoadCompound(ByVal frm As frmEditStructure)
    ' ## Load the structure referenced by this table row.
    ' ##REMARKS The structure is either the first chemical structure field for this row, or, if there is none, the first chemical structure in the section.
    Dim ts As TableSection
    Dim tp As TableProperty
    Dim tc As TableCell
    Dim sec As ENFramework9.Section
    Dim i As Long
    Dim f As ENFramework9.Field
    Dim sc As SectionCell
    Dim cs As ChemicalStructure
    Dim foundOne As Boolean
    
    foundOne = False
    Set sec = mReactantsCtl.TableCtl.TableSection.SectionCell.Section
    For i = 1 To sec.SectionType.Fields.Count
        Set f = sec.SectionType.Fields(i)
        If (TypeOf f.Tag Is ChemicalStructureFactory) Then
            Set sc = sec.SectionCell(f)
            Set cs = sc.Tag
            If (Len(cs.CDX) = 0) Then
                Err.Raise vbObjectError + 513, "LoadCompound", "the " & sc.ENObject.Description(kENDescriptionInCollection) & " has no chemical structure"
            End If
            frm.CDXStructure = cs.CDX
            foundOne = True
            Exit For
        End If
    Next i
    If (Not foundOne) Then
        Err.Raise vbObjectError + 513, "LoadCompound", "the " & sec.ENObject.Description(kENDescriptionInCollection) & " has no chemical structure"
    End If
End Sub

Private Function GetGroup(ByVal frm As frmEditStructure, ByVal ID As Long) As ChemDrawControl9Ctl.Group
    Dim i As Long
    Dim gs As ChemDrawControl9Ctl.Groups
    
    Set gs = frm.mCDXDrawing.Groups
    For i = 1 To gs.Count
        If (gs.Item(i).ID = ID) Then
            Set GetGroup = gs.Item(i)
            Exit Function
        End If
    Next i
End Function

Private Sub SetRowAttributes(ByVal frm As frmEditStructure, ByVal curRow As TableRow, ByVal GroupID As Long)
    curRow.setAttribute "ID", CStr(GroupID)
    Dim parentID As Long
    Dim saltCodeID As Long
    Dim solvateCodeID As Long
    Dim parentCoefficient As Long
    Dim saltCodeCoefficient As Long
    Dim solvateCodeCoefficient As Long
    Dim masterGroup As ChemDrawControl9Ctl.Group
    Dim g As ChemDrawControl9Ctl.Group
    
    Set masterGroup = GetGroup(frm, GroupID)
    
    If (masterGroup.Groups.Count = 0) Then
        parentID = masterGroup.ID
        parentCoefficient = 1
        saltCodeID = 0
        saltCodeCoefficient = 0
        solvateCodeID = 0
        solvateCodeCoefficient = 0
    Else
        Dim i As Long
        
        parentCoefficient = 0
        saltCodeCoefficient = 0
        solvateCodeCoefficient = 0
        
        ' TODO: Instead of going through this group in order, do it left-to-right if necessary.
        For i = 1 To masterGroup.Groups.Count
            Set g = masterGroup.Groups.Item(i)
            If (g.Groups.Count > 0) Then
                ' Do nothing
            ElseIf (parentID = 0) Then
                parentID = g.ID
                parentCoefficient = 1
            ElseIf (g.Objects.Formula = GetGroup(frm, parentID).Objects.Formula) Then
                parentCoefficient = parentCoefficient + 1
            ElseIf (saltCodeID = 0) Then
                saltCodeID = g.ID
                saltCodeCoefficient = 1
            ElseIf (g.Objects.Formula = GetGroup(frm, saltCodeID).Objects.Formula) Then
                saltCodeCoefficient = saltCodeCoefficient + 1
            ElseIf (solvateCodeID = 0) Then
                solvateCodeID = g.ID
                solvateCodeCoefficient = 1
            ElseIf (g.Objects.Formula = GetGroup(frm, solvateCodeID).Objects.Formula) Then
                solvateCodeCoefficient = solvateCodeCoefficient + 1
            End If
        Next i
    End If
    
    If (parentID = 0) Then
        curRow.setAttribute "parentID", ""
        curRow.setAttribute "parentCoefficient", ""
    Else
        curRow.setAttribute "parentID", CStr(parentID)
        curRow.setAttribute "parentCoefficient", CStr(parentCoefficient)
    End If
    
    If (saltCodeID = 0) Then
        curRow.setAttribute "saltID", ""
        curRow.setAttribute "saltCoefficient", ""
    Else
        curRow.setAttribute "saltID", CStr(saltCodeID)
        curRow.setAttribute "saltCoefficient", CStr(saltCodeCoefficient)
    End If
    
    If (solvateCodeID = 0) Then
        curRow.setAttribute "solvateID", ""
        curRow.setAttribute "solvateCoefficient", ""
    Else
        curRow.setAttribute "solvateID", CStr(solvateCodeID)
        curRow.setAttribute "solvateCoefficient", CStr(solvateCodeCoefficient)
    End If
End Sub

Private Function ComputeGroupCodes(ByVal frm As frmEditStructure, ByVal Data As String, _
                    ByRef DrawnReactantsCodeArray() As String, ByRef NumReactants As Long, _
                    ByRef DrawnProductsCodeArray() As String, ByRef NumProducts As Long) As Scripting.Dictionary
    ' ## Return a dictionary of the codes for each group in the data.
    Dim groupCodes As Scripting.Dictionary
    Dim rcs() As ReactionComponent
    Dim i As Long
    Dim Reactant As Long
    Dim Product As Long
    
    Set groupCodes = New Scripting.Dictionary
    Set ComputeGroupCodes = groupCodes
    
    If (Len(Data) > 0) Then
        frm.CDXStructure = Data
        rcs = frm.ExtractComponents
        If (UBound(rcs) > 0) Then
            For i = LBound(rcs) To UBound(rcs)
                If rcs(i).IsReactant Then
                    NumReactants = NumReactants + 1
                ElseIf rcs(i).IsProduct Then
                    NumProducts = NumProducts + 1
                End If
            Next i
            ReDim DrawnReactantsCodeArray(NumReactants)
            ReDim DrawnProductsCodeArray(NumProducts)
            Reactant = 0
            Product = 0
            For i = LBound(rcs) To UBound(rcs)
                If (rcs(i).IsReactant Or rcs(i).IsProduct) Then
                    groupCodes.Add rcs(i).ID, rcs(i).Code
                    If rcs(i).IsReactant Then
                        Reactant = Reactant + 1
                        DrawnReactantsCodeArray(Reactant) = rcs(i).Code
                    ElseIf rcs(i).IsProduct Then
                        Product = Product + 1
                        DrawnProductsCodeArray(Product) = rcs(i).Code
                    End If
                End If
            Next i
        End If
    End If
End Function

' ## Details() is the Data Sgroup field 'DETAILS' for each component.
' ## Stoichiometry() is the Data Sgroup field 'STOICHIOMETRY' for each component.
' ## (concatenated for each salt fragment)
Private Sub GetDataSgroupData(ByVal frm As frmEditStructure, ByVal Structure As String, _
                                ByRef Details() As String, ByRef Stoichiometry() As String)
    Dim NReactants As Long
    Dim NProducts As Long
    Dim dsd() As DataSgroupData
    Dim i As Long
    Dim mi As Long
    Dim MWReactant1 As Double
    Dim MWProduct1 As Double
    
    If (Len(Structure) > 0) Then
        frm.CDXStructure = Structure
         
        ' Extract Data Sgroups From the stored structure.
        dsd = ExtractDataSgroupsFromChemDraw(frm.mCDXDrawing, NReactants, NProducts, MWReactant1, MWProduct1)
        If UBound(dsd) > 0 Then
            For i = LBound(dsd) To UBound(dsd)
                mi = dsd(i).MoleculeIndex
                If (StrComp(dsd(i).FieldName, "DETAILS", vbTextCompare) = 0) Then
                    If (mi <= UBound(Details)) Then
                        Details(mi) = dsd(i).Data 'instr
                    End If
                End If
                If (StrComp(dsd(i).FieldName, "STOICHIOMETRY", vbTextCompare) = 0) Then
                    ' Note that this depends on the order of salt fragments always being the same, which is the case.
                    If (mi <= UBound(Stoichiometry)) Then
                        Stoichiometry(mi) = Stoichiometry(mi) & dsd(i).Data
                    End If
                End If
            Next i
        End If
    End If
End Sub

'DataSgroupsAreIdentical(OldIndex, NewIndex, ReactantsFlag, numOldDrawnReactants, numNewDrawnReactants)

Private Function DataSgroupsAreIdentical(ByVal OldIndex As Long, ByVal NewIndex As Long, _
        ByVal ReactantsFlag As Boolean, ByVal numOldDrawnReactants As Long, ByVal numNewDrawnReactants As Long) As Boolean
                
    Dim OldDrawnProductsIndex As Long ' Molecule index
    Dim NewDrawnProductsIndex As Long ' Molecule index

    DataSgroupsAreIdentical = False ' until proven True
    If ReactantsFlag Then
        ' Test Reactants for identity.
        If (mNewDetails(NewIndex) = mOldDetails(OldIndex)) _
            And (mNewStoichiometry(NewIndex) = mOldStoichiometry(OldIndex)) Then
            DataSgroupsAreIdentical = True
        End If
    Else
        ' Test Products for identity.
        OldDrawnProductsIndex = OldIndex + numOldDrawnReactants
        NewDrawnProductsIndex = NewIndex + numNewDrawnReactants
        If (mNewDetails(NewDrawnProductsIndex) = mOldDetails(OldDrawnProductsIndex)) _
            And (mNewStoichiometry(NewDrawnProductsIndex) = mOldStoichiometry(OldDrawnProductsIndex)) Then
            DataSgroupsAreIdentical = True
        End If
    End If
    
End Function


' Workaround to type mismatch:
Public Sub AnalyzeComponents(csc As Object, mOldData As String, ByVal NewData As String)
'Public Sub AnalyzeComponents(csc As ENStandardCtl9.ChemicalStructureCtl, mOldData As String, ByVal NewData As String)
'Public Sub AnalyzeComponents(rcs() As ENStandardCtl9.ReactionComponent, mOldData As String, ByVal NewData As String)
    Dim i As Long
    Dim j As Long
    Dim k As Long
    Dim oldID As Long
    Dim reactantsTS As TableSection
    Dim productsTS As TableSection
    Dim curRow As TableRow
    Dim tps(kNameRow To kNumReactantRows - 1) As TableProperty
    Dim pps(kNameRow To kNumProductRows - 1) As TableProperty
    Dim limitingRow As Long
    Dim removeCol As New VBA.Collection
    Dim tc As TableCell
    Dim clearEnteredValues As Boolean
    Dim clearValues As Boolean
    Dim frm As frmEditStructure
    Dim oldCodes As Scripting.Dictionary
    Dim newCodes As Scripting.Dictionary
    
    Dim numOldDrawnReactants As Long
    Dim oldDrawnReactantsCodeArray() As String
    Dim numOldDrawnProducts As Long
    Dim oldDrawnProductsCodeArray() As String
    
    Dim numNewDrawnReactants As Long
    Dim newDrawnReactantsCodeArray() As String
    Dim numNewDrawnProducts As Long
    Dim newDrawnProductsCodeArray() As String
    Dim Reactant As Long
    Dim Product As Long
    Dim DrawnReactantsCount As Long
    Dim DrawnProductsCount As Long
    Dim tp As TableProperty
    
    ' Workaround to type mismatch:
    Dim rcs() As ENStandardCtl9.ReactionComponent
    rcs = csc.ExtractComponents
    ' End workaround
    
    Set reactantsTS = mReactantsCtl.TableCtl.TableSection
    FindReactantProperties reactantsTS, tps
    
    CheckLimitingRow tps(kLimitingRow)
    
    Set productsTS = mProductsCtl.TableCtl.TableSection
    FindProductProperties productsTS, pps
    
    ' New for ISIS/Draw version: Compute CodeArrays.
    Set frm = New frmEditStructure
    ReDim oldDrawnReactantsCodeArray(0)
    ReDim oldDrawnProductsCodeArray(0)
    Set oldCodes = ComputeGroupCodes(frm, mOldData, oldDrawnReactantsCodeArray, numOldDrawnReactants, _
                                                    oldDrawnProductsCodeArray, numOldDrawnProducts)
    
    numNewDrawnReactants = 0
    numNewDrawnProducts = 0
    ReDim newDrawnReactantsCodeArray(0)
    ReDim newDrawnProductsCodeArray(0)
    
    Reactant = 0
    Product = 0

    ' New code that uses canonical codes:
    ' Don't use the rcs().Code values that were computed by csc.ExtractComponents(), above, because those are SMILES strings.
    Set newCodes = ComputeGroupCodes(frm, NewData, newDrawnReactantsCodeArray, numNewDrawnReactants, _
                                                    newDrawnProductsCodeArray, numNewDrawnProducts)
    
    ' New code that uses canonical codes:
    ' Put the computed canonical codes back into rcs().
    If (UBound(rcs) > 0) Then
        For i = LBound(rcs) To UBound(rcs)
            If rcs(i).IsReactant Then
                Reactant = Reactant + 1
                rcs(i).Code = newDrawnReactantsCodeArray(Reactant)
            ElseIf rcs(i).IsProduct Then
                Product = Product + 1
                rcs(i).Code = newDrawnProductsCodeArray(Product)
            End If
        Next i
    End If
    
    ' New for ISIS/Draw version: Use Data Sgroup Data -- it's chemically significant.
    If (numOldDrawnReactants + numOldDrawnProducts) > 0 Then
        ReDim mOldDetails(1 To numOldDrawnReactants + numOldDrawnProducts)
        ReDim mOldStoichiometry(1 To numOldDrawnReactants + numOldDrawnProducts) ' Structure STOICHIOMETRY
    Else
        ReDim mOldDetails(0 To 0)
        ReDim mOldStoichiometry(0 To 0)
    End If
    If (numNewDrawnReactants + numNewDrawnProducts) > 0 Then
        ReDim mNewDetails(1 To numNewDrawnReactants + numNewDrawnProducts) ' Structure DETAILS
        ReDim mNewStoichiometry(1 To numNewDrawnReactants + numNewDrawnProducts)
    Else
        ReDim mNewDetails(0 To 0)
        ReDim mNewStoichiometry(0 To 0)
    End If
    
    GetDataSgroupData frm, mOldData, mOldDetails, mOldStoichiometry
    GetDataSgroupData frm, NewData, mNewDetails, mNewStoichiometry
    
    ' Remove all of the items that have a tag of "Parsed"
    DrawnReactantsCount = 0
    For i = 1 To reactantsTS.CountRows
        Set curRow = reactantsTS.TableRow(i)
        ' The 'Parsed is just for historical reasons.
        If (curRow.getAttribute("ID") = "Parsed") Then
            removeCol.Add i
        ElseIf (curRow.getAttribute("ID") <> "") Then ' The ID is null is the reactant isn't drawn.
            If (LBound(rcs) = 0) Then
                ' This case is returned if the cdxml is not a reaction.
                removeCol.Add i
            Else
                DrawnReactantsCount = DrawnReactantsCount + 1
                If (DrawnReactantsCount <= UBound(oldDrawnReactantsCodeArray)) Then
                    ' For ISIS/Draw, add this attribute.  We need it later on in this routine.
                    ' Assert: DrawnReactantsCount <= UBound(oldDrawnReactantsCodeArray)
                    curRow.setAttribute "Code", oldDrawnReactantsCodeArray(DrawnReactantsCount)
                    'oldID = CLng(curRow.getAttribute("ID"))
                    For j = 1 To numNewDrawnReactants
                    'For j = LBound(rcs) To UBound(rcs)
                         ' In the case where the same structure is found, don't remove the row.
                        If newDrawnReactantsCodeArray(j) = oldDrawnReactantsCodeArray(DrawnReactantsCount) And _
                            DataSgroupsAreIdentical(DrawnReactantsCount, j, True, numOldDrawnReactants, numNewDrawnReactants) Then
                                'Core code: If (rcs(j).ID = oldID) Then ' In the case where a match is found, don't remove the row.
                            Exit For
                        End If
                    Next j
                    If (j > numNewDrawnReactants) Then ' In the case where no match was found, remove the row.
                    'If (j > UBound(rcs)) Then ' In the case where no match was found, remove the row.
                        removeCol.Add i
                    ElseIf (newDrawnReactantsCodeArray(j) <> oldDrawnReactantsCodeArray(DrawnReactantsCount)) And _
                       Not DataSgroupsAreIdentical(DrawnReactantsCount, j, True, numOldDrawnReactants, numNewDrawnReactants) Then ' Remove row if no structure matches.
                          'Core code: ElseIf (rcs(j).ID <> oldID) Then
                        removeCol.Add i
                    ' For ISIS/Draw, remove this as we need to go on position.
                    'ElseIf (Not rcs(j).IsReactant) Then ' If this item is now a product, then move it.
                    '    removeCol.Add i
                    End If
                Else
                    ' Fix bug CSBR-59295.
                    removeCol.Add i
                    ' Old code: MsgBox "DrawnReactantsCount assertion violation", vbOKOnly, "StoicCalculator Error"
                End If
            End If
        End If
    Next i
    
    If (removeCol.Count > 0) Then
        ' Step backwards through the rows to be removed and remove them in consecutive bunches.
        j = 1
        limitingRow = removeCol(removeCol.Count)
        For i = removeCol.Count - 1 To 1 Step -1
            If (removeCol(i) = limitingRow - j) Then
                j = j + 1
            Else
                mReactantsCtl.TableCtl.RemoveRows limitingRow - j + 1, j, False
                j = 1
                limitingRow = removeCol(i)
            End If
        Next i
        
        mReactantsCtl.TableCtl.RemoveRows limitingRow - j + 1, j, False
        Set removeCol = New VBA.Collection  ' Recycle the collection
    End If
    
    For i = 1 To productsTS.CountRows
        Set curRow = productsTS.TableRow(i)
        If (curRow.getAttribute("ID") = "Parsed") Then
            removeCol.Add i
        ElseIf (curRow.getAttribute("ID") <> "") Then
            If (LBound(rcs) = 0) Then
                ' This case is returned if the cdxml is not a reaction.
                removeCol.Add i
            Else
                DrawnProductsCount = DrawnProductsCount + 1
                ' Assert: DrawnProductsCount <= UBound(oldDrawnProductsCodeArray)
                If (DrawnProductsCount <= UBound(oldDrawnProductsCodeArray)) Then
                    ' For ISIS/Draw, add this attribute.  We need it later on in this routine.
                    curRow.setAttribute "Code", oldDrawnProductsCodeArray(DrawnProductsCount)
                    'oldID = CLng(curRow.getAttribute("ID"))
                    For j = 1 To numNewDrawnProducts
                    'For j = LBound(rcs) To UBound(rcs)
                         ' In the case where the same structure is found, don't remove the row.
                        If newDrawnProductsCodeArray(j) = oldDrawnProductsCodeArray(DrawnProductsCount) And _
                          DataSgroupsAreIdentical(DrawnProductsCount, j, False, numOldDrawnReactants, numNewDrawnReactants) Then
                        'If (rcs(j).ID = oldID) Then
                            Exit For
                        End If
                    Next j
                    If (j > numNewDrawnProducts) Then
                        removeCol.Add i
                    ElseIf (newDrawnProductsCodeArray(j) <> oldDrawnProductsCodeArray(DrawnProductsCount)) And _
                      Not DataSgroupsAreIdentical(DrawnProductsCount, j, False, numOldDrawnReactants, numNewDrawnReactants) Then ' Remove row if no structure matches.
                    'ElseIf (rcs(j).ID <> oldID) Then
                        removeCol.Add i
                    'ElseIf (Not rcs(j).IsProduct) Then ' If this item is now a Product, then move it.
                    '    removeCol.Add i
                    End If
                End If
            End If
        End If
    Next i
    
    If (removeCol.Count > 0) Then
        ' Step backwards through the rows to be removed and remove them in consecutive bunches.
        j = 1
        limitingRow = removeCol(removeCol.Count)
        For i = removeCol.Count - 1 To 1 Step -1
            If (removeCol(i) = limitingRow - j) Then
                j = j + 1
            Else
                mProductsCtl.TableCtl.RemoveRows limitingRow - j + 1, j, False
                j = 1
                limitingRow = removeCol(i)
            End If
        Next i
        
        mProductsCtl.TableCtl.RemoveRows limitingRow - j + 1, j, False
        Set removeCol = New VBA.Collection  ' Recycle the collection
    End If
    
    limitingRow = 0
    If (Not tps(kLimitingRow) Is Nothing) Then
        For i = 1 To reactantsTS.CountRows
            Set curRow = reactantsTS.TableRow(i)
            If (XMLBoolean(curRow.TableCell(tps(kLimitingRow)).value)) Then
                limitingRow = i
                Exit For
            End If
        Next i
    End If
    
    ' set the limiting reactant to the first row.
    If (limitingRow = 0) Then
        If (reactantsTS.CountRows > 0) Then
            Set curRow = reactantsTS.TableRow(1)
            
            ' Support "Yes" for historical table properties that are text, not boolean
            mReactantsCtl.TableCtl.SetTableCellValue curRow.TableCell(tps(kLimitingRow)), _
                IIf(tps(kLimitingRow).PropertyTypeName = "Boolean", XMLString(True), "Yes")
            limitingRow = 1
        End If
    End If
    
    ' Make sure there is a reaction to interpret before continuing
    If (LBound(rcs) = 0) Then
        Exit Sub
    End If
    
    Dim newReactantPosition As Long
    Dim newProductPosition As Long
    
    On Error GoTo CatchError
    
    ' ISIS/Draw version - This was moved up.
    'Set frm = New frmEditStructure
    'Set oldCodes = ComputeGroupCodes(frm, mOldData)
    
    LoadCompound frm
    newReactantPosition = 0
    newProductPosition = 0
    For i = LBound(rcs) To UBound(rcs)
        If (rcs(i).IsReactant) Then
        
            ' Record the new position of this item.
            newReactantPosition = newReactantPosition + 1
            With mReactantsCtl.TableCtl
                ' Find a row with the tag equal to CStr(i)
                Set curRow = Nothing
                For j = newReactantPosition To reactantsTS.CountRows
                    ' ISIS/Draw version:  Test for a matching structure.
                    If (reactantsTS.TableRow(j).getAttribute("Code") = rcs(i).Code) Then
                    'If (reactantsTS.TableRow(j).getAttribute("index") = CStr(i) Or _  '
                    '    reactantsTS.TableRow(j).getAttribute("ID") = CStr(rcs(i).ID)) Then
                        While (j > newReactantPosition)
                            .MoveRowCommand j, -1
                            j = j - 1
                        Wend
                        Set curRow = reactantsTS.TableRow(j)
                        Exit For
                    End If
                Next j
                If (curRow Is Nothing) Then
'                    If reactantsTS.CountRows >= numNewDrawnReactants Then
'                        .RemoveRowCommand
'                    End If
                    Set curRow = .InsertRowTask(newReactantPosition)
                    
                    If (Not tps(kLimitingRow) Is Nothing) Then
                        ' If there is already a table row with limiting? == true, then make this no, otherwise make it yes.
                        If (limitingRow = 0) Then
                            .SetTableCellValue curRow.TableCell(tps(kLimitingRow)), _
                                IIf(tps(kLimitingRow).PropertyTypeName = "Boolean", XMLString(True), "Yes")
                            limitingRow = curRow.Index
                        Else
                            .SetTableCellValue curRow.TableCell(tps(kLimitingRow)), _
                                IIf(tps(kLimitingRow).PropertyTypeName = "Boolean", XMLString(False), "No")
                        End If
                    End If
                End If
                
                If (curRow.getAttribute("ID") <> CStr(rcs(i).ID)) Then
                    SetRowAttributes frm, curRow, rcs(i).ID
                    clearEnteredValues = True
                Else
                    clearEnteredValues = False
                End If
                                
            End With
        
        ElseIf rcs(i).IsProduct Then
            newProductPosition = newProductPosition + 1
            With mProductsCtl.TableCtl
                Set curRow = Nothing
                For j = newProductPosition To productsTS.CountRows
                    ' ISIS/Draw version:  Test for a matching structure.
                    If (productsTS.TableRow(j).getAttribute("Code") = rcs(i).Code) Then
                    'If (productsTS.TableRow(j).getAttribute("index") = CStr(i) Or _
                    '    productsTS.TableRow(j).getAttribute("ID") = CStr(rcs(i).ID)) Then
                        While (j > newProductPosition)
                            .MoveRowCommand j, -1
                            j = j - 1
                        Wend
                        Set curRow = productsTS.TableRow(j)
                        Exit For
                    End If
                Next j
                If (curRow Is Nothing) Then
                    Set curRow = .InsertRowTask(newProductPosition)
                End If
                
                If (curRow.getAttribute("ID") <> CStr(rcs(i).ID)) Then
                    SetRowAttributes frm, curRow, rcs(i).ID
                    clearEnteredValues = True
                Else
                    clearEnteredValues = False
                End If
            End With
        End If
    Next i
        
    For j = 1 To reactantsTS.CountRows
        If (reactantsTS.TableRow(j).getAttribute("index") <> "") Then
            mReactantsCtl.TableCtl.SetBestRowSpacing j
        End If
        ' Delete "code" lest it overflow the Tag column in the database.
        reactantsTS.TableRow(j).setAttribute "Code", ""
    Next j
    For j = 1 To productsTS.CountRows
        If (productsTS.TableRow(j).getAttribute("index") <> "") Then
            mProductsCtl.TableCtl.SetBestRowSpacing j
        End If
        ' Delete "code" lest it overflow the Tag column in the database.
        productsTS.TableRow(j).setAttribute "Code", ""
    Next j
    
CatchError:
    Dim errNumber As Long
    Dim errDesc As String
    Dim errSource As String
    
    errNumber = Err.Number
    errDesc = Err.Description
    errSource = Err.Source
    
    If (Not frm Is Nothing) Then
        Unload frm
        Set frm = Nothing
    End If
    
    If (errNumber <> 0) Then
        Err.Raise errNumber, errSource, errDesc
    End If
End Sub

