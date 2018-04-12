Attribute VB_Name = "ISISListenerCommon"
Option Explicit

' Code common to the ISIS/Draw listeners

Const mTagName As String = "StoredDataSgroup"
Const mMaxDataSgroups As Long = 5
Const mObjectTagDataItems As Long = 5

Private Declare Function RegisterClipboardFormat Lib "user32" Alias "RegisterClipboardFormatA" (ByVal lpString As String) As Long

Function GetTotalMolWt(ByVal molId As Long, ByVal skObjId As Long) As Double
    Dim MolWtString As String
    Dim tArray() As String
    Dim i As Long
    Dim wt As Double
    Dim lSkId3 As Long
    Dim NumFragments As Long
    Dim skObjId3 As Long
    Dim skObjType As Long
    Dim molId3 As Long
    Dim MolWt As Double
    Dim molwtdd As String
    Dim SaveSketchMode As Long
    Dim Formula As String
    Dim Formuladd As String

    GetTotalMolWt = 0#
    With goObjlib
'        lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
'        If lSkId3 <> 0 Then
'            SaveSketchMode = .GetSketchMode(lSkId3) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
'            Call .SetSketchMode(lSkId3, SKMODE_CHEM) ' Look at all atoms.
            GetTotalMolWt = .GetMolWeight(molId)
            molwtdd = .GetMolWeightDotDisconnect(molId)
            Formula = .GetMolFormula(molId)
            Formuladd = .GetMolFormulaDotDisconnect(molId)
            
            ' Check for an abbreviated structure.
            If (((GetTotalMolWt = 0#) And (Len(Formula) = 0)) And ((Len(molwtdd) > 0) And (IsNumeric(molwtdd)) And (Len(Formuladd) > 0))) Then
                GetTotalMolWt = val(molwtdd)
            End If
            
            i = 0
'            If (Not IsMultiFragment(lSkId3)) Then
'                GetTotalMolWt = .GetMolWeight(molId)
'            Else
'                NumFragments = .GetTotalSkObjs(lSkId3) ' Number of fragments.
'                If (NumFragments > 1) Then
'                    For i = 1 To NumFragments
'                        skObjId3 = .GetSkObjId(lSkId3, i)
'                        skObjType = .GetSkObjType(skObjId3)
'                        If skObjType = SKOBJTYPE_MOL Then
'                            molId3 = .GetMolIdOfSkObj(skObjId3)
'                            MolWt = .GetMolWeight(molId3)
'                            GetTotalMolWt = GetTotalMolWt + MolWt
'                        End If
'                    Next i
'                End If
'            End If
'            Call .SetSketchMode(lSkId3, SaveSketchMode)
'            .DeleteSketch (lSkId3)
'        End If
    End With

' Old code:
'    MolWtString = goObjlib.GetMolWeightDotDisconnect(molId)
'    tArray = Split(MolWtString, " ", -1, vbTextCompare)
'    wt = 0#
'    For i = LBound(tArray) To UBound(tArray)
'        wt = wt + val(tArray(i))
'    Next i
'    GetTotalMolWt = wt
End Function

' Find the bounding box of a structure that has DataSgroups, exclusive of the Data Sgroups,
' by using the atom positions.  This is necessary because if we use .GetSkObjBox(), the
' box that gets returned includes the Data Sgroups, which is not what we want.
Private Sub GetBoxFromAtoms(ByVal molId As Long, ByRef Left As Double, _
              ByRef Top As Double, ByRef Right As Double, ByRef Bottom As Double, _
              ByVal skObjId As Long)
    Dim TotalAtoms As Long
    Dim atomID As Long
    Dim i As Long
    Dim j As Long
    Dim x As Double
    Dim y As Double
    Dim z As Double
    Dim MolWeightDD As String
    Dim lSkId3 As Long
    Dim NumSaltFragments As Long
    Dim skObjId3 As Long
    Dim skObjType As Long
    Dim molId3 As Long
    Dim lSkId4 As Long
    Dim skObjId4 As Long
    Dim TotalSkObjs As Long


    Left = 0# ' Initialize output parameters.
    Top = 0#
    Right = 0#
    Bottom = 0#

    With goObjlib
        'SaveSketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101

        lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
        lSkId4 = .CopySketch(lSkId3)
        If lSkId4 <> 0 Then
            ' For a single atom.
            TotalSkObjs = .GetTotalSkObjs(lSkId4)
            For i = 1 To TotalSkObjs
                skObjId4 = .GetSkObjId(lSkId4, i)
                skObjType = .GetSkObjType(skObjId4) 'SKOBJTYPE_GROUP = 10, SKOBJTYPE_ATOM = 33
                If (skObjType = SKOBJTYPE_ATOM) Then
                    TotalAtoms = .GetTotalAtoms(lSkId4)
                    For j = 1 To TotalAtoms
                        atomID = .GetAtomId(lSkId4, j)
                        Call .GetAtomCoords(atomID, x, y, z)
                        If j = 1 Then
                            Left = x
                            Right = x
                            Top = y
                            Bottom = y
                        Else
                            If x < Left Then Left = x
                            If x > Right Then Right = x
                            If y < Top Then Top = y
                            If y > Bottom Then Bottom = y
                        End If
                    Next j
                End If
            Next i

            ' for a MOL
            Call .SetSketchMode(lSkId4, SKMODE_CHEM) 'Get all the atoms.
            TotalAtoms = .GetTotalAtoms(lSkId4)
            For i = 1 To TotalAtoms
                atomID = .GetAtomId(lSkId4, i)
                Call .GetAtomCoords(atomID, x, y, z)
                If i = 1 Then
                    Left = x
                    Right = x
                    Top = y
                    Bottom = y
                Else
                    If x < Left Then Left = x
                    If x > Right Then Right = x
                    If y < Top Then Top = y
                    If y > Bottom Then Bottom = y
                End If
            Next i

            Call .SetSketchMode(lSkId4, SKMODE_MOL)
            .DeleteSketch lSkId4
            .DeleteSketch lSkId3
        End If
        
'        If molId <> 0 Then
'            MolWeightDD = .GetMolWeightDotDisconnect(molId)
'            If InStr(1, MolWeightDD, " ", vbTextCompare) = 0 Then
'                ' This is not a salt.
'                TotalAtoms = .GetTotalAtoms(molId)
'                For i = 1 To TotalAtoms
'                    atomID = .GetAtomId(molId, i)
'                    Call .GetAtomCoords(atomID, x, y, z)
'                    If i = 1 Then
'                        Left = x
'                        Right = x
'                        Top = y
'                        Bottom = y
'                    Else
'                        If x < Left Then Left = x
'                        If x > Right Then Right = x
'                        If y < Top Then Top = y
'                        If y > Top Then Bottom = y
'                    End If
'                Next i
'            Else
'                ' This is a salt.
'                lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
'                If lSkId3 <> 0 Then
'                    NumSaltFragments = .GetTotalSkObjs(lSkId3) ' Number of fragments.
'
'                    For j = 1 To NumSaltFragments
'                        skObjId3 = .GetSkObjId(lSkId3, j)
'                        skObjType = .GetSkObjType(skObjId3)
'                        If (skObjType = SKOBJTYPE_MOL) Then
'                            molId3 = .GetMolIdOfSkObj(skObjId3)
'                            TotalAtoms = .GetTotalAtoms(molId3)
'                            For i = 1 To TotalAtoms
'                                atomID = .GetAtomId(molId3, i)
'                                Call .GetAtomCoords(atomID, x, y, z)
'                                If ((Left = 0#) And (Right = 0#)) Then ' Test for first time.
'                                    Left = x
'                                    Right = x
'                                    Top = y
'                                    Bottom = y
'                                Else
'                                    If x < Left Then Left = x
'                                    If x > Right Then Right = x
'                                    If y < Top Then Top = y
'                                    If y > Bottom Then Bottom = y
'                                End If
'                            Next i
'                        ElseIf (skObjType = SKOBJTYPE_ATOM) Then
'                            molId3 = .GetMolIdOfSkObj(skObjId3)
'                            atomID = .GetAtomId(molId3, 1)
'                            If (atomID <> 0) Then
'                                Call .GetAtomCoords(atomID, x, y, z)
'                                If ((Left = 0#) And (Right = 0#)) Then ' Test for first time.
'                                    Left = x
'                                    Right = x
'                                    Top = y
'                                    Bottom = y
'                                Else
'                                    If x < Left Then Left = x
'                                    If x > Right Then Right = x
'                                    If y < Top Then Top = y
'                                    If y > Bottom Then Bottom = y
'                                End If
'                            End If
'                        End If
'                    Next j
'                End If
'                .DeleteSketch (lSkId3)
'            End If
'            ' If Left = Right = 0 then this process has failed, so fall back to GetSkObjBox.
'            If ((Left = Right) Or (Top = Bottom)) Then
'                Call .GetSkObjBox(skObjId, Left, Top, Right, Bottom)
'            End If
'        End If
    End With
End Sub

Private Sub GetSGroups(ByVal molId As Long, ByVal MoleculeIndex As Long, _
            ByVal SaltFragmentIndex As Long, ByRef DataSgroupIndex As Long, _
            ByRef dsd() As DataSgroupData, ByVal skObjId As Long, GroupTheFragments As Boolean)
    Dim i As Long
    Dim NumDataSGroups As Long
    Dim dataSgroupId As Long
    Dim Left As Double
    Dim Top As Double
    Dim Right As Double
    Dim Bottom As Double
    Dim bSuccess As Boolean
    Dim sgLeft As Double
    Dim sgTop As Double
    Dim Width As Double
    Dim Height As Double
    Dim col As Long
    Dim vData As Variant
    Dim FieldType As String
    Dim lSkId3 As Long
    Dim NumSaltFragments As Long
    Dim NumBasisObjects As Long
    Dim TotalAtoms As Long
    Dim BasisAtoms As Long
    Dim DataSgroupDeleted As Boolean
    Dim atomID As Long
    Dim aliasExists As Boolean
    Dim aliasStr As String
    Dim MW As Double
    
    With goObjlib
        DataSgroupDeleted = False
        TotalAtoms = .GetTotalAtoms(molId)
        NumDataSGroups = .GetTotalDataSgroups(molId)
        For i = NumDataSGroups To 1 Step -1 ' Go backwards as we are deleting them.
            dataSgroupId = .GetDataSgroupID(molId, i)
            ' We are only getting Data Sgroups that span all the atoms, because if it's a salt,
            ' there may be some that we want to get later and associate with the salt fragments.
            col = 1
            NumBasisObjects = .CollectDataSgroupBasisObjects(dataSgroupId, col)
            BasisAtoms = .GetTotalAtomsInMolCollection(molId, col)
            If (BasisAtoms = TotalAtoms) Then
                DataSgroupIndex = DataSgroupIndex + 1
                dsd(DataSgroupIndex).MoleculeIndex = MoleculeIndex
                If (BasisAtoms > 1) Then
                    MW = GetTotalMolWt(molId, skObjId)
                Else
                    atomID = .GetAtomId(molId, 1)
                    aliasExists = .GetAtomAlias(atomID, aliasStr)
                    If (Not aliasExists) Then
                        MW = GetTotalMolWt(molId, skObjId)
                    Else
                        ' Note: ISISlib does not supply the correct mass for an alias that is not an
                        '       expanded residue.  It supplies the MW of CH4!
                        MW = -1# ' A negative mass is a flag to denote that we don't know the mass.
                                  ' It will be used to skip the MW check in PutSGroupDataToObjectTags().
                                  ' (fix to bug CSBR-60350)
                    End If
                End If
                dsd(DataSgroupIndex).MolecularWeight = MW
                dsd(DataSgroupIndex).FieldName = .GetDataSgroupFieldName(dataSgroupId)
                dsd(DataSgroupIndex).Data = .GetDataSgroupData(dataSgroupId)
                dsd(DataSgroupIndex).TagAlignment = .GetDataSgroupTagAlignment(dataSgroupId)
                bSuccess = .GetDataSgroupCoordinates(dataSgroupId, sgLeft, sgTop)
                If Not bSuccess Then
                    Call MsgBox("GetDataSgroupCoordinates failed.", vbOKOnly, "ISIS/Draw Interface")
                End If
                ' We no longer want this.  If ChemDraw reads it, it will place a text label,
                '  which will duplicate the visible Object Tags we place.
                bSuccess = .DeleteDataSgroup(dataSgroupId)
                If Not bSuccess Then
                    Call MsgBox("DeleteDataSgroup failed.", vbOKOnly, "ISIS/Draw Interface")
                Else
                    DataSgroupDeleted = True
                End If
                If TotalAtoms > 1 Then
                    Call GetBoxFromAtoms(molId, Left, Top, Right, Bottom, skObjId)
                Else
                    ' We can't use GetBoxFromAtoms because atoms are just points.
                    Call .GetSkObjBox(skObjId, Left, Top, Right, Bottom)
                End If
                If ((Left = Right) Or (Top = Bottom)) Then
                    'Call MsgBox("Data Sgroup coordinates computation from atoms was unsuccessful.", vbOKOnly, "ISIS/Draw Interface")
                    Call .GetSkObjBox(skObjId, Left, Top, Right, Bottom)
                End If
                
                ' dsd coordinates are in a molecule-relative coordinate system.
                '   The origin is the upper left of the molecule, and
                '   the point (1,1) is the lower right of the molecule.
                Width = Right - Left
                If Width > 0 Then
                    dsd(DataSgroupIndex).Left = (sgLeft - Left) / Width
                Else
                    dsd(DataSgroupIndex).Left = 0#
                End If
                Height = Bottom - Top
                If Height > 0 Then
                    dsd(DataSgroupIndex).Top = (sgTop - Top) / Height
                Else
                    dsd(DataSgroupIndex).Top = 0#
                End If
                dsd(DataSgroupIndex).SaltFragmentIndex = SaltFragmentIndex
            End If
        Next i
        If ((SaltFragmentIndex = 0) And DataSgroupDeleted And GroupTheFragments) Then
            ' By now the Data Sgroup has been deleted.
            ' Check for a salt spanned by a Data Sgroup (SSDS).
            lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
            If lSkId3 <> 0 Then
                Call .SetSketchMode(lSkId3, SKMODE_MOL)
                NumSaltFragments = .GetTotalSkObjs(lSkId3) ' Number of fragments.
                If NumSaltFragments > 1 Then
                    ' Group these fragments -- the Data Sgroup grouped them into a single molecule,
                    ' but E-Notebook requires them to be grouped fragments.
                    ' No need to do the following line of code. What we have done above does it.  They are now grouped.
                    'Call GroupFragments(lSkId3)
                End If
                .DeleteSketch lSkId3
            End If
        End If
    End With
End Sub


' Note:  The reason that we aren't using the ISIS reaction APIs here due to a bug we in the
'  ISIS intrinsics (reported to MDL) that causes the reactants and products to be grouped if
'  we use those APIs.
Function ExtractDataSgroupsFromISISSketch(ByVal lSkId As Long) As DataSgroupData()
    Dim i As Long
    Dim j As Long
    Dim k As Long
    Dim NumRxnMolecules As Long
    Dim DataSgroupIndex As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim molId As Long
    Dim TotalNumDataSGroups As Long
    Dim IsReaction As Boolean
    Dim SketchMode As Long
    Dim bSuccess As Boolean
    Dim dsd() As DataSgroupData
    Dim dataSgroupId As Long
    Dim TotalSkObjs As Long
    Dim bfirst As Boolean
    Dim Left As Double
    Dim Right As Double
    Dim Top As Double
    Dim Bottom As Double
    Dim LastLeft As Double
    Dim LeftMolecule As Long
    Dim LeftMax As Double
    Dim foundOne As Boolean
    Dim Count As Long
    Dim Found() As Boolean
    Dim SaltFragmentFound() As Boolean
    Dim MoleculeIndex As Long
    Dim lSkId2 As Long
    Dim lSkId3 As Long
    Dim MFDD As String
    Dim TotalSkObjs3 As Long
    Dim NumSaltFragments As Long
    Dim SaltFragmentIndex As Long
    Dim TotalAtoms As Long
    Dim MolFormula As String
    Dim MolWeight As Double
    Dim MolWeightDD As String
    Dim GroupTheFragments As Boolean
    Dim MakeCopy As Boolean
    Dim bDummy As Boolean
    Dim TopMax As Double
    Dim m As Long
    Dim TopMolecule As Long
    Dim Left1 As Double
    Dim FoundTopOne As Boolean
    Dim molId3 As Long

    ReDim dsd(0 To 0)
    With goObjlib
        SketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        
        ' Find the total number of Data Sgroups so we can dimension our array.
        lSkId2 = .CopySketch(lSkId)
        .SetSketchMode lSkId2, SKMODE_CHEM
        TotalSkObjs = .GetTotalSkObjs(lSkId2)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
        TotalNumDataSGroups = 0
        For i = 1 To TotalSkObjs
            skObjId = .GetSkObjId(lSkId2, i)
            skObjType = .GetSkObjType(skObjId)
            If (skObjType = SKOBJTYPE_DATASGROUP) Then
                TotalNumDataSGroups = TotalNumDataSGroups + 1
            End If
        Next i
        .DeleteSketch lSkId2
        
        .SetSketchMode lSkId, SKMODE_MOL
        TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
        
        If TotalNumDataSGroups >= 1 Then
            ReDim dsd(1 To TotalNumDataSGroups)
        
            If TotalSkObjs > 0 Then
                ReDim Found(1 To TotalSkObjs) As Boolean
            End If
            DataSgroupIndex = 0
            MoleculeIndex = 0
            For k = 1 To TotalSkObjs
                ' Find the next leftmost sketch object.
                LeftMax = 1000000#
                LeftMolecule = 0
                foundOne = False
                For i = 1 To TotalSkObjs
                    skObjId = .GetSkObjId(lSkId, i)
                    skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                    If (skObjType = SKOBJTYPE_MOL) Then
                        If (Found(i) = False) Then
                            molId = .GetMolIdOfSkObj(skObjId) ' for debugging...
                            MolFormula = .GetMolFormulaDotDisconnect(molId) ' for debugging...
                            Call GetBoxFromAtoms(molId, Left, Top, Right, Bottom, skObjId)
                            If (Left <= LeftMax + 0.0005) Then
                                ' Are there others the same X position?  If so, pick the highest.
                                TopMax = 1000000#
                                TopMolecule = 0
                                FoundTopOne = False
                                For m = 1 To TotalSkObjs
                                    skObjId = .GetSkObjId(lSkId, m)
                                    skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                                    If (skObjType = SKOBJTYPE_MOL) Then
                                        If (Found(m) = False) Then
                                            molId = .GetMolIdOfSkObj(skObjId) ' for debugging...
                                            MolFormula = .GetMolFormulaDotDisconnect(molId) ' for debugging...
                                            Call GetBoxFromAtoms(molId, Left1, Top, Right, Bottom, skObjId)
                                            If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                If (Top < TopMax) Then
                                                    TopMax = Top
                                                    TopMolecule = m
                                                    FoundTopOne = True
                                                End If
                                            End If
                                        End If
                                    End If
                                Next m
                                If FoundTopOne Then
                                    LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                Else
                                    LeftMolecule = i
                                End If
                                LeftMax = Left
                                foundOne = True
                            End If
                        End If
                    End If
                Next i
                
                If foundOne Then
                    Found(LeftMolecule) = True
                    ' Create the Data Sgroups.
                    skObjId = .GetSkObjId(lSkId, LeftMolecule)
                    skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                    If (skObjType = SKOBJTYPE_MOL) Then
                        MoleculeIndex = MoleculeIndex + 1
                        'Get Data Sgroups.
                        molId = .GetMolIdOfSkObj(skObjId)
                        TotalAtoms = .GetTotalAtoms(molId)
                        MolFormula = .GetMolFormulaDotDisconnect(molId)
                        MolWeight = GetTotalMolWt(molId, skObjId)
                        MolWeightDD = .GetMolWeightDotDisconnect(molId)
                        ' Test for a salt.
                        If InStr(1, MolWeightDD, " ", vbTextCompare) = 0 Then
                            ' This is not a salt, or it is a salt spanned by a Data Sgroup.
                            ' Get any Data Sgroup data.
                            SaltFragmentIndex = 0
                            
                            lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
                            If lSkId3 <> 0 Then
                                GroupTheFragments = False
                                MakeCopy = True ' Necessary so we don't change grouping, for simple non-SDS case
                                If (IsMultiFragment(lSkId3, MakeCopy)) Then
                                    ' This is a salt spanned by a Data Sgroup.
                                    MakeCopy = False
                                    bDummy = IsMultiFragment(lSkId3, MakeCopy) ' Call again to make a mol-group for this case.
                                    NumSaltFragments = .GetTotalSkObjs(lSkId3) ' Number of fragments.
                                    If NumSaltFragments = 1 Then ' This should always be 1.
                                        skObjId = .GetSkObjId(lSkId3, 1)
                                        skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                                        If (skObjType = SKOBJTYPE_MOL) Then
                                            'Get Data Sgroups.
                                            molId = .GetMolIdOfSkObj(skObjId)
                                            TotalAtoms = .GetTotalAtoms(molId)
                                            MolFormula = .GetMolFormulaDotDisconnect(molId)
                                            MolWeight = GetTotalMolWt(molId, skObjId)
                                            MolWeightDD = .GetMolWeightDotDisconnect(molId)
                                        End If
                                        GroupTheFragments = True
                                    Else
                                        Call MsgBox("Salt Fragment assertion failure", vbOKOnly, "ISIS/Draw Interface")
                                    End If
                                End If
                                Call GetSGroups(molId, MoleculeIndex, SaltFragmentIndex, _
                                                DataSgroupIndex, dsd, skObjId, GroupTheFragments)
                                .DeleteSketch lSkId3
                            End If
                        ' Old code: Else
                        End If
                        ' Debug:
                        'Call .WriteTGFFile("C:\after grouping SSDG5.tgf", lSkId)
                        MolWeightDD = .GetMolWeightDotDisconnect(molId)
                        If InStr(1, MolWeightDD, " ", vbTextCompare) <> 0 Then
                            ' This is a salt.
                            lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
                            If lSkId3 <> 0 Then
                                NumSaltFragments = .GetTotalSkObjs(lSkId3) ' Number of fragments.
                                
                                If NumSaltFragments > 0 Then
                                    ReDim SaltFragmentFound(1 To NumSaltFragments) As Boolean
                                End If
                                SaltFragmentIndex = 0
                                For j = 1 To NumSaltFragments
                                    ' Find the next leftmost sketch object.
                                    LeftMax = 1000000#
                                    LeftMolecule = 0
                                    foundOne = False
                                    For i = 1 To NumSaltFragments
                                        skObjId = .GetSkObjId(lSkId3, i)
                                        skObjType = .GetSkObjType(skObjId)
                                        If ((skObjType = SKOBJTYPE_MOL) Or (skObjType = SKOBJTYPE_ATOM)) Then
                                            If (SaltFragmentFound(i) = False) Then
                                                molId3 = .GetMolIdOfSkObj(skObjId) ' for debugging...
                                                MolFormula = .GetMolFormulaDotDisconnect(molId3) ' for debugging...
                                                Call GetBoxFromAtoms(molId3, Left, Top, Right, Bottom, skObjId)
                                                If (Left <= LeftMax + 0.0005) Then
                                                    ' Are there others the same X position?  If so, pick the highest.
                                                    TopMax = 1000000#
                                                    TopMolecule = 0
                                                    FoundTopOne = False
                                                    For m = 1 To NumSaltFragments
                                                        skObjId = .GetSkObjId(lSkId3, m)
                                                        skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                                                        If (skObjType = SKOBJTYPE_MOL) Then
                                                            If (SaltFragmentFound(m) = False) Then
                                                                molId3 = .GetMolIdOfSkObj(skObjId) ' for debugging...
                                                                MolFormula = .GetMolFormulaDotDisconnect(molId3) ' for debugging...
                                                                Call GetBoxFromAtoms(molId3, Left1, Top, Right, Bottom, skObjId)
                                                                If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                                    If (Top < TopMax) Then
                                                                        TopMax = Top
                                                                        TopMolecule = m
                                                                        FoundTopOne = True
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    Next m
                                                    If FoundTopOne Then
                                                        LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                                    Else
                                                        LeftMolecule = i
                                                    End If
                                                    LeftMax = Left
                                                    foundOne = True
                                                End If
                                            End If
                                        End If
                                    Next i
                                    
                                    If foundOne Then
                                        SaltFragmentFound(LeftMolecule) = True
                                        skObjId = .GetSkObjId(lSkId3, LeftMolecule)
                                        skObjType = .GetSkObjType(skObjId)
                                        If (skObjType = SKOBJTYPE_MOL) Then ' SKOBJTYPE_ATOM = 33
                                            SaltFragmentIndex = SaltFragmentIndex + 1
                                            molId3 = .GetMolIdOfSkObj(skObjId)
                                            TotalAtoms = .GetTotalAtoms(molId3)
                                            MolFormula = .GetMolFormulaDotDisconnect(molId3)
                                            MolWeight = .GetMolWeight(molId3)
                                            ' This is a salt -- get any Data Sgroup data.
                                            Call GetSGroups(molId3, MoleculeIndex, SaltFragmentIndex, _
                                                      DataSgroupIndex, dsd, skObjId, False)
                                        End If
                                    End If
                                Next j
                                .DeleteSketch lSkId3
                            End If
                        End If
                    End If
                End If
            Next k
        End If
    End With
    ExtractDataSgroupsFromISISSketch = dsd()
End Function

Private Sub PopulateCollection(ByVal molId As Long, ByVal skObjId As Long, ByVal col As Long, ByVal lSkId As Long)
    Dim lSkId3 As Long
    Dim lSkId4 As Long
    Dim NumSaltFragments As Long
    Dim NumFragments As Long
    Dim i As Long
    Dim j As Long
    Dim skObjId3 As Long
    Dim skObjId4 As Long
    Dim skObjType As Long
    Dim SaveSketchMode As Long
    Dim atomID As Long
    Dim molId3 As Long
    Dim dataSgroupId As Long
    Dim TotalAtoms As Long

    ' You can't create any molIds in the code below.  I think that breaks the association between the col
    ' and the molId of the SDS.
    With goObjlib
        SaveSketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        
        lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
        If lSkId3 <> 0 Then
            Call .SetSketchMode(lSkId3, SKMODE_CHEM) 'Get all the atoms.  (The ones that aren't sons of
                                                    '  the SDS are rejected.)
            Call .ClearMolCollection(lSkId, col)
            For j = 1 To .GetTotalAtoms(lSkId3)
                atomID = .GetAtomId(lSkId3, j)
                Call .AddAtomToMolCollection(atomID, col)
            Next j
            
            Call .SetSketchMode(lSkId, SKMODE_MOL) ' This is necessary to collapse any residues.
            
            For j = 1 To .GetTotalAtoms(lSkId3)    ' This picks up any atom abbreviations.
                atomID = .GetAtomId(lSkId3, j)
                Call .AddAtomToMolCollection(atomID, col)
            Next j
            .DeleteSketch lSkId3
        End If
    End With
End Sub


Private Sub PutSGroups(ByVal molId As Long, ByVal MolCount As Long, ByVal SaltFragmentIndex As Long, _
                                 ByRef dsd() As DataSgroupData, ByVal skObjId As Long, ByVal lSkId As Long)
    Dim i As Long
    Dim j As Long
    Dim ISISDrawMolWt As Double
    Dim ChemDrawMolWt As Double
    Dim col As Long
    Dim dataSgroupId As Long
    Dim bSuccess As Boolean
    Dim FieldName As String
    Dim Left As Double
    Dim Right As Double
    Dim Top As Double
    Dim Bottom As Double
    Dim sgLeft As Double
    Dim sgTop As Double
    Dim Width As Double
    Dim Height As Double
    Dim NewX As Double
    Dim NewY As Double
    Dim ErrorMessage As String
    Dim vData As Variant
    Dim FieldType As Long
    Dim TotalAtoms As Long
    Dim MolWeightDD As String
    Dim atomID As Long
    Dim skObjType As Long
    Dim TotalBonds As Long
    
    With goObjlib
        ISISDrawMolWt = GetTotalMolWt(molId, skObjId)
        For i = LBound(dsd) To UBound(dsd)
            If ((MolCount = dsd(i).MoleculeIndex) And _
                  (dsd(i).SaltFragmentIndex = SaltFragmentIndex)) Then
                ' Sanity check: Test to see if molecular weight is the same:
                ChemDrawMolWt = dsd(i).MolecularWeight
                If Abs(ISISDrawMolWt - ChemDrawMolWt) > 0.1 Then ' They should be equal to this tolerance.
                    ' Can't call Err.Raise here, so...
                    If (.GetMolFormula(molId) = "") Then
                        ErrorMessage = "Molecular weight test:" & _
                            " ChemDraw MW=" & CStr(ChemDrawMolWt) _
                            & ", ISIS MW=" & CStr(ISISDrawMolWt) & "." _
                            & "  Check structure."
                    Else
                        ErrorMessage = "Molecular weight test for " _
                            & .GetMolFormula(molId) & ": ChemDraw MW=" & CStr(ChemDrawMolWt) _
                            & ", ISIS MW=" & CStr(ISISDrawMolWt) & "." _
                            & "  Check structure."
                    End If
                    Call MsgBox(ErrorMessage, vbOKOnly, "ISIS/Draw Interface Warning")
                    'Err.Raise vbObjectError + 513, Description:="ISIS/Draw Interface: Data Sgroup molecular weight test failed going to ISIS"
                End If
                
                ' Compute Data Sgroup coordinates.
                If .GetTotalAtoms(molId) > 1 Then
                    Call GetBoxFromAtoms(molId, Left, Top, Right, Bottom, skObjId)
                Else
                    Call .GetSkObjBox(skObjId, Left, Top, Right, Bottom)
                End If
                If ((Left = Right) Or (Top = Bottom)) Then
                    'Call MsgBox("Data Sgroup coordinates computation from atoms was unsuccessful.", vbOKOnly, "ISIS/Draw Interface")
                    Call .GetSkObjBox(skObjId, Left, Top, Right, Bottom)
                End If
                
                ' dsd coordinates are in a molecule-relative coordinate system.
                '   The origin is the upper left of the molecule, and
                '   the point (1,1) is the lower right of the molecule.
                Width = Right - Left
                NewX = Left + dsd(i).Left * Width

                Height = Bottom - Top
                NewY = Top + dsd(i).Top * Height
                ' Note: We add a "fudge factor" of 60.  This was found emperically to position
                '  The Data Sgroup in the same place every time.  The value of this might be
                ' sensitive to the font used (in development it was Arial 9, using Organon's
                ' standard stylesheet.)  If this drifts overly, the user can always reposition it.
                
                ' Add Data Sgroup to molecule
                col = 1
                ' Check for a salt spanned by a Data Sgroup (SSDS).
                MolWeightDD = .GetMolWeightDotDisconnect(molId)
                If ((SaltFragmentIndex = 0) And (InStr(1, MolWeightDD, " ", vbTextCompare) > 0)) Then
                    ' This is a salt spanned by a Data Sgroup (SSDS).
                    Call PopulateCollection(molId, skObjId, col, lSkId)
                Else
                    Call .AddAllBondsToMolCollection(molId, col)
                    Call .AddAllAtomsToMolCollection(molId, col) ' Not neccessary as AddAllBonds does this
                End If
                
                TotalAtoms = .GetTotalAtomsInMolCollection(molId, col)
                dataSgroupId = .CreateDataSgroup(molId, col, dsd(i).FieldName, dsd(i).Data, NewX, NewY + 60)
                If dataSgroupId > 0 Then
                    vData = dsd(i).Data
                    If IsNumeric(vData) Then
                        FieldType = DATASGROUP_FIELD_TYPE_NUMERIC
                    Else
                        FieldType = DATASGROUP_FIELD_TYPE_TEXT
                    End If
                    bSuccess = .SetDataSgroupFieldType(dataSgroupId, FieldType)
                    If Not bSuccess Then
                        Call MsgBox("ISIS intrinsic SetDataSgroupFieldType failure", vbOKOnly, "ISIS/Draw Interface")
                    End If
                Else
                    Call MsgBox("ISIS intrinsic CreateDataSgroup failure", vbOKOnly, "ISIS/Draw Interface")
                End If
            End If
        Next i
    End With
End Sub


Function AddDataSgroupsToISISSketch(ByVal lSkId As Long, ByRef dsd() As DataSgroupData)
    Dim molId As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim i As Long
    Dim j As Long
    Dim k As Long
    Dim TotalSkObjs As Long
    Dim MolWeight As Double
    Dim MolCount As Long
    Dim ISISDrawMolWt As Double
    Dim ChemDrawMolWt As Double
    Dim TotalAtoms As Long
    Dim col As Long
    Dim dataSgroupId As Long
    Dim bSuccess As Boolean
    Dim Left As Double
    Dim Right As Double
    Dim Top As Double
    Dim Bottom As Double
    Dim LastLeft As Double
    Dim bfirst As Boolean
    Dim LeftMolecule As Long
    Dim LeftMax As Double
    Dim foundOne As Boolean
    Dim Count As Long
    Dim Found() As Boolean
    Dim NumSaltFragments As Long
    Dim SaltFragmentIndex As Long
    Dim MolWeightDD As String
    Dim lSkId3 As Long
    Dim Formula As String
    Dim molId3 As Long
    Dim skObjId3 As Long
    Dim MolFormula As String
    Dim TopMax As Double
    Dim m As Long
    Dim TopMolecule As Long
    Dim Left1 As Double
    Dim FoundTopOne As Boolean
    
    On Error GoTo NULLDSD
    If UBound(dsd) > 0 Then
    On Error GoTo 0
        With goObjlib
        
            TotalSkObjs = .GetTotalSkObjs(lSkId)  'Types: 12 = SKOBJTYPE_MOL, 9=SKOBJTYPE_TEXT, 14 = SKOBJTYPE_RXNARROW
            MolCount = 0
            If TotalSkObjs > 0 Then
                ReDim Found(1 To TotalSkObjs) As Boolean
            
                For k = 1 To TotalSkObjs
                    ' Find the next leftmost molecule.
                    LeftMax = 1000000#
                    LeftMolecule = 0
                    foundOne = False
                    For i = 1 To TotalSkObjs
                        skObjId = .GetSkObjId(lSkId, i)
                        skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                        If (skObjType = SKOBJTYPE_MOL) Then
                            molId = .GetMolIdOfSkObj(skObjId) ' debug
                            Formula = .GetMolFormulaDotDisconnect(molId) ' debug
                            If (Found(i) = False) Then
                                Call GetBoxFromAtoms(molId, Left, Top, Right, Bottom, skObjId)
                                If (Left <= LeftMax + 0.0005) Then
                                    ' Are there others the same X position?  If so, pick the highest.
                                    TopMax = 1000000#
                                    TopMolecule = 0
                                    FoundTopOne = False
                                    For m = 1 To TotalSkObjs
                                        skObjId = .GetSkObjId(lSkId, m)
                                        skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                                        If (skObjType = SKOBJTYPE_MOL) Then
                                            If (Found(m) = False) Then
                                                molId = .GetMolIdOfSkObj(skObjId) ' for debugging...
                                                MolFormula = .GetMolFormulaDotDisconnect(molId) ' for debugging...
                                                Call GetBoxFromAtoms(molId, Left1, Top, Right, Bottom, skObjId)
                                                If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                    If (Top < TopMax) Then
                                                        TopMax = Top
                                                        TopMolecule = m
                                                        FoundTopOne = True
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next m
                                    If FoundTopOne Then
                                        LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                    Else
                                        LeftMolecule = i
                                    End If
                                    LeftMax = Left
                                    foundOne = True
                                End If
                            End If
                        End If
                    Next i
                    If foundOne Then
                        Found(LeftMolecule) = True
                        ' Create the molecule Data Sgroups, if there are any.
                        skObjId = .GetSkObjId(lSkId, LeftMolecule)
                        skObjType = .GetSkObjType(skObjId) 'SKOBJTYPE_GROUP = 10
                        If (skObjType = SKOBJTYPE_MOL) Then
                            MolCount = MolCount + 1
                            molId = .GetMolIdOfSkObj(skObjId)
                            Formula = .GetMolFormulaDotDisconnect(molId) ' debug
                            
                            ' Test for a salt.
                            MolWeightDD = .GetMolWeightDotDisconnect(molId)
                            If InStr(1, MolWeightDD, " ", vbTextCompare) > 0 Then
                                ' This is a salt.
                                lSkId3 = .GetSketchIdOfGroup(skObjId) ' Drill down inside this group.
                                If lSkId3 <> 0 Then
                                    NumSaltFragments = .GetTotalSkObjs(lSkId3) ' Number of fragments.
                                    
                                    If NumSaltFragments > 0 Then
                                        ReDim SaltFragmentFound(1 To NumSaltFragments) As Boolean
                                    End If
                                    SaltFragmentIndex = 0
                                    For j = 1 To NumSaltFragments
                                        ' Find the next leftmost sketch object.
                                        LeftMax = 1000000#
                                        LeftMolecule = 0
                                        foundOne = False
                                        For i = 1 To NumSaltFragments
                                            skObjId3 = .GetSkObjId(lSkId3, i)
                                            skObjType = .GetSkObjType(skObjId3)
                                            If ((skObjType = SKOBJTYPE_MOL) Or (skObjType = SKOBJTYPE_ATOM)) Then
                                                If (SaltFragmentFound(i) = False) Then
                                                    molId3 = .GetMolIdOfSkObj(skObjId3) ' for debugging...
                                                    MolFormula = .GetMolFormulaDotDisconnect(molId3) ' for debugging...
                                                    Call GetBoxFromAtoms(molId3, Left, Top, Right, Bottom, skObjId3)
                                                    If (Left <= LeftMax + 0.0005) Then
                                                        ' Are there others the same X position?  If so, pick the highest.
                                                        TopMax = 1000000#
                                                        TopMolecule = 0
                                                        FoundTopOne = False
                                                        For m = 1 To NumSaltFragments
                                                            skObjId3 = .GetSkObjId(lSkId3, m)
                                                            skObjType = .GetSkObjType(skObjId3) 'SKOBJTYPE_GROUP = 10
                                                            If (skObjType = SKOBJTYPE_MOL) Then
                                                                If (SaltFragmentFound(m) = False) Then
                                                                    molId3 = .GetMolIdOfSkObj(skObjId3) ' for debugging...
                                                                    MolFormula = .GetMolFormulaDotDisconnect(molId3) ' for debugging...
                                                                    Call GetBoxFromAtoms(molId3, Left1, Top, Right, Bottom, skObjId3)
                                                                    If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                                        If (Top < TopMax) Then
                                                                            TopMax = Top
                                                                            TopMolecule = m
                                                                            FoundTopOne = True
                                                                        End If
                                                                    End If
                                                                End If
                                                            End If
                                                        Next m
                                                        If FoundTopOne Then
                                                            LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                                        Else
                                                            LeftMolecule = i
                                                        End If
                                                        LeftMax = Left
                                                        foundOne = True
                                                    End If
                                                End If
                                            End If
                                        Next i
                                        
                                        If foundOne Then
                                            SaltFragmentFound(LeftMolecule) = True
                                            SaltFragmentIndex = SaltFragmentIndex + 1
                                            skObjId3 = .GetSkObjId(lSkId3, LeftMolecule)
                                            molId3 = .GetMolIdOfSkObj(skObjId3)
                                            Formula = .GetMolFormulaDotDisconnect(molId3) ' debug
                                                                              
                                            Call PutSGroups(molId3, MolCount, SaltFragmentIndex, dsd, skObjId3, lSkId)
                                            
                                        End If
                                    Next j
                                    .DeleteSketch (lSkId3)
                                End If
                            End If
                            
'                            ' Look for top-level Data Sgroups.
                            SaltFragmentIndex = 0
                            Call PutSGroups(molId, MolCount, SaltFragmentIndex, dsd, skObjId, lSkId)
                            
                        End If
                    Else
                        Exit For
                    End If
                Next k
            End If
        End With
    End If
NULLDSD:
End Function


Private Sub GetSGroupDataFromObjectTags(ByVal g As ChemDrawControl9Ctl.Group, _
                ByRef ChemDrawCtl As Object, ByRef TotalNumDataSGroups As Long, _
                ByVal MoleculeIndex As Long, ByVal SaltFragmentIndex As Long, _
                ByRef dsd() As DataSgroupData)
    Dim ot As ChemDrawControl9Ctl.ObjectTag
    Dim tString As String
    Dim tArray() As String
    Dim Ngroups As Long
    Dim i As Long
    Dim j As Long
    Dim otName As String
    Dim NumFragments As Long
    
    With ChemDrawCtl
        ' Find Data Sgroup data in ObjectTags.
        For i = 1 To mMaxDataSgroups
            otName = mTagName & CStr(i)
            Set ot = g.GetObjectTag(otName)
            If Not ot Is Nothing Then
                tString = ot.StringValue
                tArray = Split(tString, "|", -1, vbTextCompare)
                
                If mObjectTagDataItems - 1 = UBound(tArray) Then ' If not, Object tag is not well formed - ignore it.
                    TotalNumDataSGroups = TotalNumDataSGroups + 1
                    If (TotalNumDataSGroups = 1) Then
                        ReDim dsd(1 To 1) ' (first one)
                    Else
                        ReDim Preserve dsd(1 To TotalNumDataSGroups)
                    End If
                    ' StringValue format: String data delimited by vertical bars:
                    '   <data for each data sgroup> = <Field>|<Data>|<Left>|<Top>|<TagAlignment>
                    dsd(TotalNumDataSGroups).FieldName = tArray(0)
                    dsd(TotalNumDataSGroups).Data = tArray(1)
                    dsd(TotalNumDataSGroups).Left = tArray(2)
                    dsd(TotalNumDataSGroups).Top = tArray(3)
                    dsd(TotalNumDataSGroups).TagAlignment = tArray(4)
                    ' Supply values not stored in the Object tag:
                    dsd(TotalNumDataSGroups).MolecularWeight = g.Objects.MolecularWeight ' For sanity check test.
                    dsd(TotalNumDataSGroups).MoleculeIndex = MoleculeIndex
                    dsd(TotalNumDataSGroups).SaltFragmentIndex = SaltFragmentIndex
        
                End If
            End If
            Set ot = Nothing
        Next i
    End With
End Sub


'Function ExtractDataSgroupsFromChemDraw(ByRef ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl) As DataSgroupData()
Function ExtractDataSgroupsFromChemDraw(ByRef ChemDrawCtl As Object, _
           ByRef NReactants As Long, ByRef NProducts As Long, _
           ByRef MWReactant1 As Double, ByRef MWProduct1 As Double) As DataSgroupData()

    Const NumDataSgroupProperties = 5
    Dim MoleculeIndex As Long
    Dim g As ChemDrawControl9Ctl.Group
    Dim gLeft As ChemDrawControl9Ctl.Group
    Dim gFound As ChemDrawControl9Ctl.Group
    Dim ReactionsCount As Integer
    Dim ot As ChemDrawControl9Ctl.ObjectTag
    Dim tCount As Long
    Dim i As Long
    Dim j As Long
    Dim k As Long
    Dim l As Long
    Dim tString As String
    Dim ISISDrawMolWt As Double
    Dim ChemDrawMolWt As Double
    Dim Count As Long
    Dim tArray() As String
    Dim TotalNumDataSGroups As Long
    Dim Ngroups As Long
    Dim dsd() As DataSgroupData
    Dim oMolecule As ChemDrawControl9Ctl.Objects
    Dim LeftMax As Double
    Dim LeftMolecule As Long
    Dim foundOne As Boolean
    Dim Found() As Boolean
    Dim gSalt As ChemDrawControl9Ctl.Group
    Dim gSaltFound As ChemDrawControl9Ctl.Group
    Dim gParent As ChemDrawControl9Ctl.Group
    Dim SaltFragmentIndex As Long
    Dim SaltDataSgroups As Boolean
    Dim SaltFragmentFound() As Boolean
    Dim NMolecules As Long
    Dim Formula As String
    Dim NumArrows As Long
    Dim ArrowRightEnd As Double
    Dim GroupMidpointX As Double
    Dim TopMax As Double
    Dim m As Long
    Dim TopMolecule As Long
    Dim Left1 As Double
    Dim FoundTopOne As Boolean
    Dim gTop As ChemDrawControl9Ctl.Group
    Dim gTopFound As ChemDrawControl9Ctl.Group
    Dim Left As Double
    Dim Right As Double
    Dim Top As Double
    Dim Bottom As Double

    
    ReDim dsd(0 To 0)
    With ChemDrawCtl
        ReactionsCount = .ReactionSchemes.Count
        If ReactionsCount > 0 Then
            NumArrows = .ReactionSchemes(1).Arrows.Count
            If NumArrows > 0 Then
                If NumArrows > 1 Then
                    MsgBox "Error - only one arrow is supported per reaction.", vbOKOnly, "ISIS/Draw Interface"
                Else
                    ArrowRightEnd = .ReactionSchemes(1).Arrows(1).Start.x
                End If
            End If
        End If
        
        TotalNumDataSGroups = 0
        MoleculeIndex = 0
        NReactants = 0
        NProducts = 0
        NMolecules = 0
        MWReactant1 = 0#  ' Molecular weight of reactants and products...
        MWProduct1 = 0#
        
        ' Count the molecules.
        For Each g In .Groups
            If (IsMolecule(g)) Then
                ' This is a molecule.
                NMolecules = NMolecules + 1
            End If
        Next g
        ReDim Found(NMolecules) As Boolean
            
        For Each g In .Groups
            If (IsMolecule(g)) Then
                ' This is a molecule.
                MoleculeIndex = MoleculeIndex + 1
                ' Find the next leftmost molecule.
                LeftMax = 1000000#
                LeftMolecule = 0
                foundOne = False
                i = 0
                For Each gLeft In .Groups
                    If (IsMolecule(gLeft)) Then
                        Formula = gLeft.Objects.Formula ' for debugging
                        ' This is a molecule.
                        i = i + 1
                        If Not Found(i) Then
                            Call GetBoxFromAtomsOT(gLeft, Left, Top, Right, Bottom)
                            If (Left <= LeftMax + 0.0005) Then
                                ' Are there others the same X position?  If so, pick the highest.
                                TopMax = 1000000#
                                TopMolecule = 0
                                FoundTopOne = False
                                m = 0
                                For Each gTop In .Groups
                                    If (IsMolecule(gTop)) Then
                                        ' This is a molecule.
                                        Formula = gTop.Objects.Formula ' for debugging
                                        m = m + 1
                                        If (Found(m) = False) Then
                                            Call GetBoxFromAtomsOT(gTop, Left1, Top, Right, Bottom)
                                            If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                If (Top < TopMax) Then
                                                    TopMax = Top
                                                    TopMolecule = m
                                                    FoundTopOne = True
                                                    Set gTopFound = gTop
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                                If FoundTopOne Then
                                    LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                    Set gFound = gTopFound
                                Else
                                    LeftMolecule = i
                                    Set gFound = gLeft
                                End If
                                LeftMax = Left
                                foundOne = True
                            End If
                        End If
                    End If
                Next
                If foundOne Then
                    Found(LeftMolecule) = True
                    Formula = gFound.Objects.Formula ' for debugging
                    If NumArrows > 0 Then
                        Call GetBoxFromAtomsOT(gFound, Left, Top, Right, Bottom)
                        GroupMidpointX = (Right + Left) / 2#
                        If (GroupMidpointX < ArrowRightEnd) Then
                            NReactants = NReactants + 1
                            If (NReactants = 1) Then
                                MWReactant1 = gFound.Objects.MolecularWeight
                            End If
                        Else
                            NProducts = NProducts + 1
                            If (NProducts = 1) Then
                                MWProduct1 = gFound.Objects.MolecularWeight
                            End If
                        End If
                    End If
                    
                    ' Retrieve Data Sgroup data in ObjectTags.
                    Formula = gFound.Objects.Formula ' for debugging
                    Call GetSGroupDataFromObjectTags(gFound, ChemDrawCtl, TotalNumDataSGroups, MoleculeIndex, _
                             0, dsd)
                    
                    ' Check for a salt
                    If gFound.Groups.Count > 1 Then
                        
                        ' This is a Salt that contains Data Sgroups, or a group that contains
                        ' salts that contain data Sgroups.
                        
                        ' See if we need to drill down one level.
                        ' Is there only one of the contained groups that is a child of this one?
                        i = 0
                        For Each gSalt In gFound.Groups
                            If (gFound.ID = gSalt.Group.ID) Then
                                i = i + 1
                                Set gParent = gSalt
                            End If
                        Next gSalt
                        If i = 1 Then
                            ' Yes, there is only one of the contained groups that is a child of this one.
                            ' Now, is there at least one child of this gParent?
                            If (gParent.Groups.Count > 0) Then
                                Set gFound = gParent ' Drill down one level.
                            End If
                        End If
                        
                        ReDim SaltFragmentFound(gFound.Groups.Count) As Boolean
                        SaltFragmentIndex = 0
                        For Each gSalt In gFound.Groups
                            SaltFragmentIndex = SaltFragmentIndex + 1
                            ' Find the next leftmost Salt Fragment.
                            LeftMax = 1000000#
                            LeftMolecule = 0
                            foundOne = False
                            i = 0
                            For Each gLeft In gFound.Groups
                                i = i + 1
                                If Not SaltFragmentFound(i) Then
                                    Call GetBoxFromAtomsOT(gLeft, Left, Top, Right, Bottom)
                                    If (Left <= LeftMax + 0.0005) Then
                                        ' Are there others the same X position?  If so, pick the highest.
                                        TopMax = 1000000#
                                        TopMolecule = 0
                                        FoundTopOne = False
                                        m = 0
                                        For Each gTop In gFound.Groups
                                            m = m + 1
                                            If (SaltFragmentFound(m) = False) Then
                                                Call GetBoxFromAtomsOT(gTop, Left1, Top, Right, Bottom)
                                                If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                    If (Top < TopMax) Then
                                                        TopMax = Top
                                                        TopMolecule = m
                                                        FoundTopOne = True
                                                        Set gTopFound = gTop
                                                    End If
                                                End If
                                            End If
                                        Next
                                        If FoundTopOne Then
                                            LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                            Set gSaltFound = gTopFound
                                        Else
                                            LeftMolecule = i
                                            Set gSaltFound = gLeft
                                        End If
                                        LeftMax = Left
                                        foundOne = True
                                    End If
                                End If
                            Next
                                
                            If foundOne Then
                                SaltFragmentFound(LeftMolecule) = True
                                Formula = gSaltFound.Objects.Formula ' for debugging
                                
                                ' Retrieve Data Sgroup data in ObjectTags.
                                Call GetSGroupDataFromObjectTags(gSaltFound, ChemDrawCtl, TotalNumDataSGroups, _
                                MoleculeIndex, SaltFragmentIndex, dsd)
                    
                            End If
                        Next gSalt
                    End If
                End If
            End If
        Next g
    End With
    ExtractDataSgroupsFromChemDraw = dsd()
End Function

Private Sub PutSGroupDataToObjectTags(ByVal g As ChemDrawControl9Ctl.Group, ByVal NGroupsMolecule As Long, _
            ByVal TotalNumDataSGroups As Long, ByVal MoleculeIndex As Long, ByVal SaltFragmentIndex As Long, _
            ByRef dsd() As DataSgroupData, ByRef cdc As Object)
    Dim i As Long
    Dim j As Long
    Dim ot As ChemDrawControl9Ctl.ObjectTag
    Dim otVis As ChemDrawControl9Ctl.ObjectTag
    Dim ISISDrawMolWt As Double
    Dim ChemDrawMolWt As Double
    Dim otName As String
    Dim tString As String
    Dim DX As Double
    Dim DY As Double
    Dim NewX As Double
    Dim NewY As Double
    Dim OldX As Double
    Dim OldY As Double
    Dim Height As Double
    Dim Width As Double
    Dim ErrorMessage As String
    Dim LeftX As Double
    Dim LeftY As Double
    Dim LeftMostAtom As Long
    Dim a As ChemDrawControl9Ctl.Atom
    Dim pt As ChemDrawControl9Ctl.Point
    Dim Left As Double
    Dim Top As Double
    Dim Right As Double
    Dim Bottom As Double
    
    j = 0
    For i = 1 To TotalNumDataSGroups ' Loop through all the Data Sgroup data to find the ones that are for us.
        If ((MoleculeIndex = dsd(i).MoleculeIndex) And _
            (SaltFragmentIndex = dsd(i).SaltFragmentIndex)) Then
            
            ' Molecule and Salt fragment match - make an Object Tag.
            j = j + 1
            otName = mTagName & CStr(j)
            ' Sanity check: Test to see if molecular weight is the same:
            ISISDrawMolWt = dsd(i).MolecularWeight
            ChemDrawMolWt = g.Objects.MolecularWeight
            
            If (ISISDrawMolWt > 0#) Then ' Fix bug CSBR-60350 (see companion code in GetSGroups().)
                If Abs(ISISDrawMolWt - ChemDrawMolWt) > 0.1 Then ' They should be equal to this tolerance.
                                ' Can't call Err.Raise here, so...
                    If (g.Objects.Formula = "") Then
                        ErrorMessage = "Molecular weight test:" _
                            & " ISIS MW=" & CStr(ISISDrawMolWt) _
                            & ", ChemDraw MW=" & CStr(ChemDrawMolWt) & "." _
                            & "  Check structure."
                    Else
                        ErrorMessage = "Molecular weight test for " _
                            & g.Objects.Formula & ": ISIS MW=" & CStr(ISISDrawMolWt) _
                            & ", ChemDraw MW=" & CStr(ChemDrawMolWt) & "." _
                            & "  Check structure."
                    End If
                    Call MsgBox(ErrorMessage, vbOKOnly, "ISIS/Draw Interface Warning")
                    'Err.Raise vbObjectError + 513, Description:="ISIS/Draw Interface: Data Sgroup molecular weight test failed going to ChemDraw"
                End If
            End If
            
            Call g.MakeObjectTag(otName, ot, True) ' Create the Object Tag.
            ' StringValue format: String data delimited by vertical bars:
            '   <Field>|<Data>|<Left>|<Top>|<TagAlignment>
            tString = dsd(i).FieldName & "|"
            tString = tString & dsd(i).Data & "|"
            tString = tString & CStr(Round(dsd(i).Left, 3)) & "|"
            tString = tString & CStr(Round(dsd(i).Top, 3)) & "|"
            tString = tString & CStr(dsd(i).TagAlignment)
            ot.StringValue = tString
            
            If Not ot Is Nothing Then

'                ' Set up visible Object Tag caption.

'                ' dsd coordinates are in a molecule-relative coordinate system.
'                '   The origin is the upper left of the molecule, and
'                '   the point (1,1) is the lower right of the molecule.
                If g.Atoms.Count > 1 Then
                    Call GetBoxFromAtomsOT(g, Left, Top, Right, Bottom)
                Else
                    Left = g.Bounds.Left / 20#
                    Top = g.Bounds.Top / 20#
                    Right = g.Bounds.Right / 20#
                    Bottom = g.Bounds.Bottom / 20#
                End If
                Width = Right - Left
                NewX = Left + dsd(i).Left * Width

                Height = Bottom - Top
                NewY = Top + dsd(i).Top * Height
                
                ' Make another one to work around CSBR-56268.  Since relative positioning of
                ' Object tags on atoms *does* work, we pick an atom, and position our object tag on it.
                'Pick the leftmost atom.
                For j = 1 To g.Atoms.Count
                    If j = 1 Then
                        LeftMostAtom = j
                        LeftX = g.Atoms(j).Position.x
                        LeftY = g.Atoms(j).Position.y
                    Else
                        If g.Atoms(j).Position.x < LeftX Then
                            LeftMostAtom = j
                            LeftX = g.Atoms(j).Position.x
                            LeftY = g.Atoms(j).Position.y
                        End If
                    End If
                Next j
                Set a = g.Atoms(LeftMostAtom)
                Call a.MakeObjectTag(otName & "Visible", otVis, True)
                otVis.PositioningType = kCDPositioningTypeOffset
                otVis.Caption.Color = 1
                otVis.Caption.Text = dsd(i).Data
                otVis.Caption.Settings.CaptionJustification = kCDJustificationLeft
                
                Dim GroupWidth As Double
                Dim otWidth As Double
                Dim otHeight As Double
                ' 20 is a scale factor to take into account different ChemDraw units used.
                ' (suggested by Coh)
                ' (Per JBrecher, the different units are bugs.)
                ' The ot.Bounds.Right is to right-justify the text.
                otWidth = (otVis.Bounds.Left - otVis.Bounds.Right) / 20#
                otHeight = (otVis.Bounds.Bottom - otVis.Bounds.Top) / 20#
                
                Set pt = otVis.PositioningOffset
                pt.x = NewX - LeftX
                pt.y = NewY - LeftY + otHeight / 2 + 5
                pt.z = 0
                otVis.PositioningOffset = pt
            End If
        End If
    Next i
End Sub

Function IsMolecule(g As ChemDrawControl9Ctl.Group) As Boolean
        If (((g.Group.ID = 0) And (g.Objects.Formula <> "") And _
                (g.Objects.MolecularWeight > 0)) Or _
            (((g.Group.ID = 0) And (g.Objects.Formula = Chr(149)) And _
                (g.Objects.MolecularWeight = 0)))) Then
            IsMolecule = True
        Else
            IsMolecule = False
        End If
End Function


'Sub SaveDataSgroupsInChemDraw(ByRef ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl, ByRef dsd() As DataSgroupData)
Sub SaveDataSgroupsInChemDraw(ByRef ChemDrawCtl As Object, ByRef dsd() As DataSgroupData, _
                                ByRef NReactants As Long, ByRef NProducts As Long)
    Dim MoleculeIndex As Long
    Dim g As ChemDrawControl9Ctl.Group
    Dim gLeft As ChemDrawControl9Ctl.Group
    Dim gFound As ChemDrawControl9Ctl.Group
    Dim gSaltFound As ChemDrawControl9Ctl.Group
    Dim ReactionsCount As Integer
    Dim ot As ChemDrawControl9Ctl.ObjectTag
    Dim tCount As Long
    Dim i As Long
    Dim j As Long
    Dim k As Long
    Dim tString As String
    Dim ISISDrawMolWt As Double
    Dim ChemDrawMolWt As Double
    Dim Count As Long
    Dim TotalNumDataSGroups As Long
    Dim NGroupsMolecule As Long
    Dim NSaltGroupsMolecule As Long
    Dim oMolecule As ChemDrawControl9Ctl.Objects
    Dim LeftMax As Double
    Dim LeftMolecule As Long
    Dim foundOne As Boolean
    Dim Found() As Boolean
    Dim gSalt As ChemDrawControl9Ctl.Group
    Dim SaltFragmentIndex As Long
    Dim SaltDataSgroups As Boolean
    Dim NMolecules As Long
    Dim gCount As Long
    Dim gParent As ChemDrawControl9Ctl.Group
    Dim NumArrows As Long
    Dim ArrowRightEnd As Double
    Dim GroupMidpointX As Double
    Dim Formula As String
    Dim TopMax As Double
    Dim m As Long
    Dim TopMolecule As Long
    Dim Left1 As Double
    Dim FoundTopOne As Boolean
    Dim gTop As ChemDrawControl9Ctl.Group
    Dim gTopFound As ChemDrawControl9Ctl.Group
    Dim Left As Double
    Dim Right As Double
    Dim Top As Double
    Dim Bottom As Double
    
    TotalNumDataSGroups = UBound(dsd)
    'If TotalNumDataSGroups > 0 Then
        With ChemDrawCtl
            ReactionsCount = .ReactionSchemes.Count
            If ReactionsCount > 0 Then
                NumArrows = .ReactionSchemes(1).Arrows.Count
                If NumArrows > 0 Then
                    If NumArrows > 1 Then
                        'MsgBox "Error - only one arrow is supported per reaction.", vbOKOnly, "ISIS/Draw Interface"
                    Else
                        ArrowRightEnd = .ReactionSchemes(1).Arrows(1).Start.x
                    End If
                End If
            End If
                
            ' Count the molecules.
            For Each g In .Groups
                ' The second test using Chr(149) is for an Organon "No Structure"
                If (IsMolecule(g)) Then
                    ' This is a molecule.
                    NMolecules = NMolecules + 1
                End If
            Next g
            ReDim Found(NMolecules) As Boolean
            
            MoleculeIndex = 0
            NReactants = 0
            NProducts = 0
                
            For Each g In .Groups
                If (IsMolecule(g)) Then
                    ' This is a molecule.
                    MoleculeIndex = MoleculeIndex + 1
                    ' Find the next leftmost molecule.
                    LeftMax = 1000000#
                    LeftMolecule = 0
                    foundOne = False
                    i = 0
                    For Each gLeft In .Groups
                        If (IsMolecule(gLeft)) Then
                            ' This is a molecule.
                            i = i + 1
                            If Not Found(i) Then
                                Call GetBoxFromAtomsOT(gLeft, Left, Top, Right, Bottom)
                                If (Left <= LeftMax + 0.0005) Then
                                    ' Are there others the same X position?  If so, pick the highest.
                                    TopMax = 1000000#
                                    TopMolecule = 0
                                    FoundTopOne = False
                                    m = 0
                                    For Each gTop In .Groups
                                        If (IsMolecule(gTop)) Then
                                            m = m + 1
                                            If (Found(m) = False) Then
                                                Call GetBoxFromAtomsOT(gTop, Left1, Top, Right, Bottom)
                                                If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                    If (Top < TopMax) Then
                                                        TopMax = Top
                                                        TopMolecule = m
                                                        FoundTopOne = True
                                                        Set gTopFound = gTop
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next
                                    If FoundTopOne Then
                                        LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                        Set gFound = gTopFound
                                    Else
                                        LeftMolecule = i
                                        Set gFound = gLeft
                                    End If
                                    LeftMax = Left
                                    foundOne = True
                                End If
                            End If
                        End If
                    Next gLeft
                    If foundOne Then
                        Found(LeftMolecule) = True
                        Formula = gFound.Objects.Formula ' for debugging
                        
                        If NumArrows > 0 Then
                            Call GetBoxFromAtomsOT(gFound, Left, Top, Right, Bottom)
                            GroupMidpointX = (Right + Left) / 2#
                            If (GroupMidpointX < ArrowRightEnd) Then
                                NReactants = NReactants + 1
                            Else
                                NProducts = NProducts + 1
                            End If
                        End If
                        
                        ' Put Data Sgroup(s) on.
                        ' Count the number of them for this molecule.
                        NGroupsMolecule = 0
                        For i = 1 To TotalNumDataSGroups
                            If ((MoleculeIndex = dsd(i).MoleculeIndex) And _
                                 (dsd(i).SaltFragmentIndex = 0)) Then
                                NGroupsMolecule = NGroupsMolecule + 1
                            End If
                        Next i
                        
                        If (NGroupsMolecule > 0) Then
                            Call PutSGroupDataToObjectTags(gFound, NGroupsMolecule, TotalNumDataSGroups, _
                                    MoleculeIndex, 0, dsd, ChemDrawCtl)
                        End If
                        
                        ' Are there salt Data Sgroups for this molecule?
                        SaltDataSgroups = False
                        For i = 1 To TotalNumDataSGroups
                            If ((MoleculeIndex = dsd(i).MoleculeIndex) And _
                                (dsd(i).SaltFragmentIndex > 0)) Then
                                SaltDataSgroups = True
                            End If
                        Next i
                        If SaltDataSgroups Then
                            ' This is a Salt that contains Data Sgroups, or a group that contains
                            ' salts that contain data Sgroups.
                            
                            ' See if we need to drill down one level.
                            ' Is there only one of the contained groups that is a child of this one?
                            i = 0
                            For Each gSalt In gFound.Groups
                                If (gFound.ID = gSalt.Group.ID) Then
                                    i = i + 1
                                    Set gParent = gSalt
                                End If
                            Next gSalt
                            If i = 1 Then
                                ' Yes, there is only one of the contained groups that is a child of this one.
                                ' Now, is there at least one child of this gParent?
                                If (gParent.Groups.Count > 0) Then
                                    Set gFound = gParent ' Drill down one level.
                                End If
                            End If
                            
                            ReDim SaltFragmentFound(gFound.Groups.Count) As Boolean
                            SaltFragmentIndex = 0
                            For Each gSalt In gFound.Groups
                                SaltFragmentIndex = SaltFragmentIndex + 1
                                ' Find the next leftmost Salt Fragment.
                                LeftMax = 1000000#
                                LeftMolecule = 0
                                foundOne = False
                                i = 0
                                For Each gLeft In gFound.Groups
                                    i = i + 1
                                    If Not SaltFragmentFound(i) Then
                                        Call GetBoxFromAtomsOT(gLeft, Left, Top, Right, Bottom)
                                        If (Left <= LeftMax + 0.0005) Then
                                            ' Are there others the same X position?  If so, pick the highest.
                                            TopMax = 1000000#
                                            TopMolecule = 0
                                            FoundTopOne = False
                                            m = 0
                                            For Each gTop In gFound.Groups
                                                m = m + 1
                                                If (SaltFragmentFound(m) = False) Then
                                                    Call GetBoxFromAtomsOT(gTop, Left1, Top, Right, Bottom)
                                                    If (Abs(Left - Left1) <= 0.001) Then ' Are they at the same X position?
                                                        If (Top < TopMax) Then
                                                            TopMax = Top
                                                            TopMolecule = m
                                                            FoundTopOne = True
                                                            Set gTopFound = gTop
                                                        End If
                                                    End If
                                                End If
                                            Next
                                            If FoundTopOne Then
                                                LeftMolecule = TopMolecule ' (Actually top molecule of the left molecules that are at the same X.)
                                                Set gSaltFound = gTopFound
                                            Else
                                                LeftMolecule = i
                                                Set gSaltFound = gLeft
                                            End If
                                            LeftMax = Left
                                            foundOne = True
                                        End If
                                    End If
                                Next
                                
                                If foundOne Then
                                    SaltFragmentFound(LeftMolecule) = True
                                    
                                    Call PutSGroupDataToObjectTags(gSaltFound, NSaltGroupsMolecule, TotalNumDataSGroups, _
                                                         MoleculeIndex, SaltFragmentIndex, dsd, ChemDrawCtl)
                                End If
                            Next gSalt
                        End If
                    End If
                End If
            Next g
        End With
    'End If
End Sub

'Sub RemoveIdentical(ByRef ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl)
'Sub RemoveIdentical(ByRef ChemDrawCtl As Object, ByVal FieldCtlContainer As ENFramework9.IFieldCtlContainer, _
'                       DeleteRows As Boolean)
'    Dim i As Long
'    Dim j As Long
'    Dim k As Long
'    Dim Smiles As String
'    Dim g As ChemDrawControl9Ctl.Group
'    Dim g1 As ChemDrawControl9Ctl.Group
'    Dim ReactionsCount As Integer
'    Dim Count As Long
'    Dim Cleared As Boolean
'    Dim AnyCleared As Boolean
'    Dim NumReactants As Long
'    Dim NumProducts As Long
'    Dim NReactants As Long
'    Dim NProducts As Long
'    Dim frm As frmEditStructure
'    Dim sCanonicalCode As String
'    Dim ThisSection As ENFramework9.Section
'    Dim ReactantsField As ENFramework9.Field
'    Dim ReactantsTableSection As ENStandard9.TableSection
'    Dim ReactantsTableCtl As ENStandardCtl9.CTableCtl
'    Dim ProductsField As ENFramework9.Field
'    Dim ProductsTableSection As ENStandard9.TableSection
'    Dim ProductsTableCtl As ENStandardCtl9.CTableCtl
'    Dim tr As ENStandard9.TableRow
'    Dim tc As ENStandard9.TableCell
'    Dim GroupID As Long
'    Dim MWReactant1 As Double
'    Dim MWProduct1 As Double
'
'    ' ## Details() is the Data Sgroup field 'DETAILS' for each component.
'    Dim Details() As String
'
'    ' ## Stoichiometry() is the Data Sgroup field 'STOICHIOMETRY' for each component.
'    Dim Stoichiometry() As String
'
'    Dim dsd() As DataSgroupData
'
'    If (Right(FieldCtlContainer.SelectedSection.SectionType.Name, 8) <> "Reaction") Then
'        Call MsgBox("Error - Section type should begin with 'Reaction'.", vbOKCancel, "ISIS/Draw interface")
'        Exit Sub
'    End If
'
'    If DeleteRows Then
'        Set ReactantsField = FieldCtlContainer.SelectedSection.SectionType.FieldByName("Reactants")
'        Set ReactantsTableSection = FieldCtlContainer.SelectedSection.SectionCell(ReactantsField).Tag
'        Set ReactantsTableCtl = FieldCtlContainer.Control(ReactantsField).object.CTableCtl
'        Set ProductsField = FieldCtlContainer.SelectedSection.SectionType.FieldByName("Products")
'        Set ProductsTableSection = FieldCtlContainer.SelectedSection.SectionCell(ProductsField).Tag
'        Set ProductsTableCtl = FieldCtlContainer.Control(ProductsField).object.CTableCtl
'    End If
'
'    With ChemDrawCtl
'
'        AnyCleared = False
'        ReactionsCount = .ReactionSchemes.Count
'        If ReactionsCount > 0 Then
'
'            ' Use Data Sgroup Data -- it's chemically significant.
'            NumReactants = .ReactionSchemes(1).Reactants.Count
'            NumProducts = .ReactionSchemes(1).Products.Count
'            If (NumReactants + NumProducts) > 0 Then
'                ReDim Details(1 To NumReactants + NumProducts) ' Structure DETAILS
'                ReDim Stoichiometry(1 To NumReactants + NumProducts)
'            End If
'            ' Extract Data Sgroups From the stored structure.
'            dsd = ExtractDataSgroupsFromChemDraw(ChemDrawCtl, NReactants, NProducts, MWReactant1, MWProduct1)
'            If UBound(dsd) > 0 Then
'                For i = LBound(dsd) To UBound(dsd)
'                    If (StrComp(dsd(i).FieldName, "DETAILS", vbTextCompare) = 0) Then
'                        Details(dsd(i).MoleculeIndex) = dsd(i).Data
'                    End If
'                    If (StrComp(dsd(i).FieldName, "STOICHIOMETRY", vbTextCompare) = 0) Then
'                        ' Note that this depends on the order of salt fragments always being the same, which is the case.
'                        Stoichiometry(dsd(i).MoleculeIndex) = Stoichiometry(dsd(i).MoleculeIndex) & dsd(i).Data
'                    End If
'                Next i
'            End If
'
'            Do
'                Cleared = False
''                Count = .ReactionSchemes(1).Reactants.Count
'                i = 0
'                ReactionsCount = .ReactionSchemes.Count
'                If ReactionsCount > 0 Then
'                For Each g In .ReactionSchemes(1).Reactants
'                    i = i + 1
'                        sCanonicalCode = CanonicalCode(g)
'                        ' Old code: Smiles = g.Objects.Data("chemical/x-smiles")
'
'                    ' Look for an identical reactant.
'                    j = 0
'                    For Each g1 In .ReactionSchemes(1).Reactants
'                        j = j + 1
'                        If i < j Then
'                                If (sCanonicalCode = CanonicalCode(g1)) Then
'                                If (DataSgroupsAreIdentical1(i, j, True, NumReactants, Details, Stoichiometry)) Then
'                                    ' Identical molecule found.
'                                    If DeleteRows Then
'                                        ' Delete the corresponding row in the grid.  Use fragment ID to find it.
'                                        For k = 1 To ReactantsTableSection.CountRows
'                                            Set tr = ReactantsTableSection.TableRow(k)
'                                            If (IsNumeric(tr.getAttribute("ID"))) Then
'                                                GroupID = val(tr.getAttribute("ID"))
'                                                If (GroupID = g.ID) Then
'                                                    ' We found the row to delete.
'                                                    ReactantsTableCtl.TableCtl.RemoveRows k, 1, False
'                                                    Exit For
'                                                End If
'                                            End If
'                                        Next k
'                                    End If
'                                    g.Objects.Clear
'                                    Cleared = True
'                                    AnyCleared = True
'                                End If
'                            End If
'                        End If
'                    Next g1
'                Next g
'                End If
'            Loop Until Cleared = False
'            Do
'                Cleared = False
''                Count = .ReactionSchemes(1).Products.Count
'                i = 0
'                ReactionsCount = .ReactionSchemes.Count
'                If ReactionsCount > 0 Then
'                For Each g In .ReactionSchemes(1).Products
'                    i = i + 1
'                        sCanonicalCode = CanonicalCode(g)
'                        ' Old code: Smiles = g.Objects.Data("chemical/x-smiles")
'                    ' Look for an identical reactant.
'                    j = 0
'                    For Each g1 In .ReactionSchemes(1).Products
'                        j = j + 1
'                        If i < j Then
'                                If (sCanonicalCode = CanonicalCode(g1)) Then
'                                If (DataSgroupsAreIdentical1(i, j, False, NumReactants, Details, Stoichiometry)) Then
'                                    ' Identical molecule found.
'                                    If DeleteRows Then
'                                        ' Delete the corresponding row in the grid.  Use fragment ID to find it.
'                                        For k = 1 To ProductsTableSection.CountRows
'                                            Set tr = ProductsTableSection.TableRow(k)
'                                            If (IsNumeric(tr.getAttribute("ID"))) Then
'                                                GroupID = val(tr.getAttribute("ID"))
'                                                If (GroupID = g.ID) Then
'                                                    ' We found the row to delete.
'                                                    ProductsTableCtl.TableCtl.RemoveRows k, 1, False
'                                                    Exit For
'                                                End If
'                                            End If
'                                        Next k
'                                    End If
'                                    g.Objects.Clear
'                                    Cleared = True
'                                    AnyCleared = True
'                                End If
'                            End If
'                        End If
'                    Next g1
'                Next g
'                End If
'            Loop Until Cleared = False
'        End If
'    End With
'    If AnyCleared Then
'        MsgBox "Duplicate reactants and/or products were removed.", vbInformation, "ISIS/Draw Integration"
'    End If
'End Sub

' Work around CSBR-58415 and it's associated ChemDraw bug CSBR-58650.  Do this in mdlStructure_DataFromStructureEdit().
'Sub RemovePlusSigns(ByRef ChemDrawCtl As Object)
'
'    With ChemDrawCtl
'        ' Remove all plus signs.
'        Dim ReactionsCount As Integer
'        Dim Count As Long
'
'        ReactionsCount = .ReactionSchemes.Count
'        If ReactionsCount > 0 Then
'            For Count = .Captions.Count To 1 Step -1
'                If .Captions(Count).Text = "+" Then
'                    .Captions(Count).Delete
'                End If
'            Next Count
'        End If
'    End With
'End Sub

' Old code:
'' Insert plus signs, making use of a feature of ChemDraw that puts them
''   between all reactants and products via the Clean method if any of them overlap.
''Sub InsertPlusSign(ByRef X as double,byref Y as double, ByRef ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl)
'Sub InsertPlusSign(ByRef x As Double, ByRef y As Double, ByRef ChemDrawCtl As Object, _
'                    OnArrow As Boolean)
'    Dim oCaption As ChemDrawControl9Ctl.Caption
'    Dim oPos As Point
'    Dim DX As Double
'    Dim DY As Double
'    Dim OldX As Double
'    Dim OldY As Double
'    Dim ArrowAdjust As Double
'
'    With ChemDrawCtl
'
'        Set oCaption = .MakeCaption
'        oCaption.Text = "+"
'        oCaption.Settings.CaptionFont = "Arial"
'        oCaption.Settings.CaptionSize = 5
'        OldX = oCaption.Position.x
'        OldY = oCaption.Position.y
'
'        DX = x - OldX
'        DY = y - OldY
'
'        If OnArrow Then
'            ' Bump the plus sign to the left.
'            'ArrowAdjust = .selection.Objects.Width  ' * 4#
'        Else
'            'ArrowAdjust = .selection.Objects.Width
'        End If
'
'        .Objects.Unselect
'         oCaption.Selected = True
'        .selection.Objects.Move DX - ArrowAdjust, _
'                                DY + .selection.Objects.Width / 2#
'    End With
'End Sub
'
''Sub InsertPlusSigns(ByRef ChemDrawCtl As ChemDrawControl9Ctl.ChemDrawCtl)
'Sub InsertPlusSigns(ByRef ChemDrawCtl As Object)
'    Dim ReactionsCount As Integer
'    Dim LeftMax As Double
'    Dim LeftCoord As Double
'    Dim LeftMolecule As Long
'    Dim LeftMax2 As Double
'    Dim LeftMolecule2 As Long
'    Dim Found() As Boolean
'    Dim OnArrow() As Boolean
'    Dim foundOne As Boolean
'    Dim FoundTwo As Boolean
'    Dim CDG As ChemDrawControl9Ctl.Group
'    Dim Xloc As Double
'    Dim Yloc As Double
'    Dim DeltaX As Double
'    Dim DeltaY As Double
'    Dim i As Long
'    Dim j As Long
'    Dim Count As Long
'    Dim Moved As Boolean
'    Dim oCaption As ChemDrawControl9Ctl.Caption
'    Dim oPos As ChemDrawControl9Ctl.Point
'    Dim oMolecule As ChemDrawControl9Ctl.Objects
'    Dim oMolecule2 As ChemDrawControl9Ctl.Objects
'    Dim NumArrows As Long
'    Dim CenterX As Double
'    Dim oGroup As ChemDrawControl9Ctl.Group
'    Dim CaptionAlreadyExists As Boolean
'
'    With ChemDrawCtl
'    '.ReactionSchemes(1).ReactionSteps(1).GroupsAboveArrow.count
'        Moved = False
'        ReactionsCount = .ReactionSchemes.Count
'        If ReactionsCount > 0 Then
'            'The form that launches this form should make sure that there is only one reaction
'            Count = .ReactionSchemes(1).Reactants.Count
'            ReDim Found(Count) As Boolean
'            ReDim OnArrow(Count) As Boolean
'            If Count > 1 Then
'                ' Find which reactants are on top of or below the arrow.
'                '.ReactionSchemes(1).Arrows(1).Bounds.BottomLeft.X
'
'                NumArrows = .ReactionSchemes(1).Arrows.Count
'                If NumArrows > 0 Then
'                    If NumArrows > 1 Then
'                        MsgBox "Error - only one arrow is supported per reaction.", vbOKOnly, "ISIS/Draw Interface"
'                        'Err.Raise vbObjectError + 513, Description:="ISIS/Draw Interface: Only one arrow is supported per reaction."
'                    End If
'                    For i = 1 To Count
'                        CenterX = (.ReactionSchemes(1).Reactants(i).Objects.Left + _
'                                   .ReactionSchemes(1).Reactants(i).Objects.Right) / 2#
'                        If CenterX > .ReactionSchemes(1).Arrows(1).End.x Then
'                            OnArrow(i) = True
'                        End If
'                    Next i
'                End If
'                ' Find the leftmost molecule.
'                LeftMax = 10000
'                LeftMolecule = 0 ' This means there is none.
'                foundOne = False
'                For i = 1 To Count
'                    If Not OnArrow(i) Then
'                        LeftCoord = .ReactionSchemes(1).Reactants(i).Objects.Left
'                        If LeftCoord < LeftMax Then
'                            LeftMax = LeftCoord
'                            LeftMolecule = i
'                            Set oMolecule = .ReactionSchemes(1).Reactants(i).Objects
'                            foundOne = True
'                        End If
'                    End If
'                Next
'                If foundOne Then
'                    Found(LeftMolecule) = True
'                    For j = 1 To Count
'                        If Not OnArrow(j) Then
'                            ' Find the next most leftmost molecule.
'                            LeftMax2 = 10000
'                            LeftMolecule2 = 0
'                            FoundTwo = False
'                            For i = 1 To Count
'                                If Not Found(i) And Not OnArrow(i) Then
'                                    LeftCoord = .ReactionSchemes(1).Reactants(i).Objects.Left
'                                    If LeftCoord < LeftMax2 Then
'                                        LeftMax2 = LeftCoord
'                                        LeftMolecule2 = i
'                                        Set oMolecule2 = .ReactionSchemes(1).Reactants(i).Objects
'                                        FoundTwo = True
'                                    End If
'                                End If
'                            Next i
'                            If FoundTwo Then
'                                Found(LeftMolecule2) = True
'                                ' See if there already is a plus sign.
'                                CaptionAlreadyExists = False
'                                For Each oCaption In .Captions
'                                    If oCaption.Text = "+" And (oCaption.Position.x > oMolecule.Right) _
'                                      And (oCaption.Position.x < oMolecule2.Left) Then
'                                        CaptionAlreadyExists = True
'                                    End If
'                                Next oCaption
'                                If Not CaptionAlreadyExists Then
'                                ' Insert a plus sign between the molecules.
'                                    Xloc = (oMolecule.Right + oMolecule2.Left) / 2# '- (oMolecule.Right - oMolecule2.Left) / 4#
'                                Yloc = (oMolecule2.Top + oMolecule2.Bottom) / 2#
'                                Call InsertPlusSign(Xloc, Yloc, ChemDrawCtl, False)
'                                End If
'
'                                 ' Old method (does not support reactants above and below the arrow)
'                                 'get the delta - enough to overlap them by 1/4 of the leftmost's width
'                                 'DeltaX = LeftMax + .ReactionSchemes(1).Reactants(LeftMolecule).Objects.Width * 0.75 _
'                                            - LeftMax2
'                                 'DeltaY = 0#
'
'                                 'move the leftmost molecule
'                                 '.ReactionSchemes(1).Reactants(LeftMolecule2).Objects.Move DeltaX, DeltaY
'                                 'Moved = True
'                            End If
'                            ' Shift right.
'                            Set oMolecule = oMolecule2
'                        End If
'                    Next j
'                End If
'                ' Now process the reactants that are above or below the arrow.
'                For i = 1 To Count
'                    If OnArrow(i) Then
'                        Set oMolecule = .ReactionSchemes(1).Reactants(i).Objects
'                        ' See if there already is a plus sign.
'                        CaptionAlreadyExists = False
'                        For Each oCaption In .Captions
'                            If oCaption.Text = "+" And (oCaption.Position.y < oMolecule.Bottom) _
'                              And (oCaption.Position.y > oMolecule.Top) Then
'                                CaptionAlreadyExists = True
'                            End If
'                        Next oCaption
'                        If Not CaptionAlreadyExists Then
'                        ' Insert a plus sign between the molecules.
'                            Xloc = oMolecule.Left - (oMolecule.Right - oMolecule.Left)
'                        Yloc = (oMolecule.Top + oMolecule.Bottom) / 2#
'                        Call InsertPlusSign(Xloc, Yloc, ChemDrawCtl, True)
'                    End If
'                    End If
'                Next i
'            End If
'
'            Count = .ReactionSchemes(1).Products.Count
'            ReDim Found(Count) As Boolean
'            ReDim OnArrow(Count) As Boolean
'            If Count > 1 Then
'                ' Find the leftmost molecule.
'                LeftMax = 10000
'                LeftMolecule = 0 ' This means there is none.
'                foundOne = False
'                For i = 1 To Count
'                    If Not OnArrow(i) Then
'                        LeftCoord = .ReactionSchemes(1).Products(i).Objects.Left
'                        If LeftCoord < LeftMax Then
'                            LeftMax = LeftCoord
'                            LeftMolecule = i
'                            Set oMolecule = .ReactionSchemes(1).Products(i).Objects
'                            foundOne = True
'                        End If
'                    End If
'                Next
'                If foundOne Then
'                    Found(LeftMolecule) = True
'                    For j = 1 To Count
'                        If Not OnArrow(j) Then
'                            ' Find the next most leftmost molecule.
'                            LeftMax2 = 10000
'                            LeftMolecule2 = 0
'                            FoundTwo = False
'                            For i = 1 To Count
'                                If Not Found(i) And Not OnArrow(i) Then
'                                    LeftCoord = .ReactionSchemes(1).Products(i).Objects.Left
'                                    If LeftCoord < LeftMax2 Then
'                                        LeftMax2 = LeftCoord
'                                        LeftMolecule2 = i
'                                        Set oMolecule2 = .ReactionSchemes(1).Products(i).Objects
'                                        FoundTwo = True
'                                    End If
'                                End If
'                            Next i
'                            If FoundTwo Then
'                                Found(LeftMolecule2) = True
'                                ' See if there already is a plus sign.
'                                CaptionAlreadyExists = False
'                                For Each oCaption In .Captions
'                                    If oCaption.Text = "+" And (oCaption.Position.x > oMolecule.Right) _
'                                      And (oCaption.Position.x < oMolecule2.Left) Then
'                                        CaptionAlreadyExists = True
'                                    End If
'                                Next oCaption
'                                If Not CaptionAlreadyExists Then
'                                ' Insert a plus sign between the molecules.
'                                    Xloc = (oMolecule.Right + oMolecule2.Left) / 2# ' - (oMolecule.Right - oMolecule2.Left) / 4#
'                                Yloc = (oMolecule2.Top + oMolecule2.Bottom) / 2#
'                                Call InsertPlusSign(Xloc, Yloc, ChemDrawCtl, False)
'                            End If
'                            End If
'                            ' Shift right.
'                            Set oMolecule = oMolecule2
'                        End If
'                    Next j
'                End If
'            End If
'
''            Count = .ReactionSchemes(1).Products.Count
''            ReDim Found(Count) As Boolean
''            If Moved = False And Count > 1 Then
''                ' Find the leftmost molecule.
''                LeftMax = 10000
''                LeftMolecule = 0 ' This means there is none.
''                For i = 1 To Count
''                    LeftCoord = .ReactionSchemes(1).Products(i).Objects.Left
''                    If LeftCoord < LeftMax Then
''                        LeftMax = LeftCoord
''                        LeftMolecule = i
''                        Found(i) = True
''                    End If
''                Next
''                ' Put a plus sign between all reactant pairs.
''                If LeftMolecule <> 0 Then
''                    ' Find the next most leftmost molecule.
''                    LeftMax2 = 10000
''                    LeftMolecule2 = 0
''                    For i = 1 To Count
''                        If Not Found(i) Then
''                            LeftCoord = .ReactionSchemes(1).Products(i).Objects.Left
''                            If LeftCoord < LeftMax2 Then
''                                LeftMax2 = LeftCoord
''                                LeftMolecule2 = i
''                                Found(i) = True
''                            End If
''                        End If
''                    Next i
''                    If LeftMolecule2 <> 0 Then
''                        ' Insert a plus sign.
''                        Set oCaption = .MakeCaption
''                        oCaption.Text = "+"
''                        Set oPos = oCaption.Position
''                        oPos.X = LeftMax2 - 10
''                        oPos.Y = (.ReactionSchemes(1).Products(LeftMolecule2).Objects.Top + _
''                                    .ReactionSchemes(1).Products(LeftMolecule2).Objects.Bottom) / 2#
''
''' Old method:
'''                         'get the delta - enough to overlap them by 1/4 of the leftmost's width
'''                         DeltaX = LeftMax + .ReactionSchemes(1).Products(LeftMolecule).Objects.Width * 0.75 _
'''                                    - LeftMax2
'''                         DeltaY = 0#
'''
'''                         'move the leftmost molecule
'''                         .ReactionSchemes(1).Products(LeftMolecule2).Objects.Move DeltaX, DeltaY
'''                         Moved = True
''                    End If
''                End If
''            End If
''            'Remove any overlaps
''            If Moved Then
''                .Objects.Clean ' This repositions the molecules and inserts plus signs.
''            End If
'        End If
'    End With
'End Sub

'DataSgroupsAreIdentical1(i, j, ReactantsFlag, numReactants, Details, Stoichiometry)

Function DataSgroupsAreIdentical1(ByVal OldIndex As Long, ByVal NewIndex As Long, _
        ByVal ReactantsFlag As Boolean, ByVal NumReactants As Long, _
        ByRef Details() As String, ByRef Stoichiometry() As String) As Boolean
                
    Dim OldDrawnProductsIndex As Long ' Molecule index
    Dim NewDrawnProductsIndex As Long ' Molecule index

    DataSgroupsAreIdentical1 = False ' until proven True
    If ReactantsFlag Then
        ' Test Reactants for identity.
        If (Details(NewIndex) = Details(OldIndex)) _
            And (Stoichiometry(NewIndex) = Stoichiometry(OldIndex)) Then
            DataSgroupsAreIdentical1 = True
        End If
    Else
        ' Test Products for identity.
        OldDrawnProductsIndex = OldIndex + NumReactants
        NewDrawnProductsIndex = NewIndex + NumReactants
        If (Details(NewDrawnProductsIndex) = Details(OldDrawnProductsIndex)) _
            And (Stoichiometry(NewDrawnProductsIndex) = Stoichiometry(OldDrawnProductsIndex)) Then
            DataSgroupsAreIdentical1 = True
        End If
    End If
    
End Function

' Example code from ISIS ADK:
Private Function IsMultiFragment(skId3 As Long, MakeCopy As Boolean) As Boolean
    Dim isis As Object, TotAtoms As Long, CollAtoms
    Dim SaveSketchMode As Long
    Dim skId As Long
    
    With goObjlib
        If MakeCopy Then
            skId = .CopySketch(skId3)
        Else
            skId = skId3
        End If
        SaveSketchMode = .GetSketchMode(skId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        Call .SetSketchMode(skId, SKMODE_CHEM)
        TotAtoms = .GetTotalAtoms(skId)
        If TotAtoms = 0 Then
            IsMultiFragment = False
            Exit Function
        End If
        Call .ClearMolCollection(skId, 1)
        Call .AddAtomToMolCollection(.GetAtomId(skId, 1), 1)
        Do
            CollAtoms = .GetTotalAtomsInMolCollection(skId, 1)
            Call .CollectAlpha(skId, 1, True, True)
        Loop Until .GetTotalAtomsInMolCollection(skId, 1) = _
        CollAtoms
        If CollAtoms < TotAtoms Then
            IsMultiFragment = True
        Else
            IsMultiFragment = False
        End If
        Call .SetSketchMode(skId, SaveSketchMode)
        If MakeCopy Then
            .DeleteSketch (skId)
        End If
    End With
End Function

Sub DebugWriteFiles(ByVal NameRoot As String, cdxml As String)
    Dim fso As FileSystemObject
    Dim os As Scripting.TextStream
    Dim FileWriteSuccess As Boolean
    Dim FileName As String
    Dim Path As String
    Dim FileCount As Long
    
    Path = "c:\IDdebug\"
    
    On Error GoTo CatchError
    If FileCount = 0 Then
        ' If this file is not found, we have an error and don't do anything.
        Open Path & "count.txt" For Input As #1
            Input #1, FileCount
        Close #1
        ' Increment count.
        FileCount = FileCount + 1
        Open Path & "count.txt" For Output As #1
            Print #1, FileCount
        Close #1
    End If
    FileName = NameRoot & " " & CStr(FileCount)
    
    Set fso = New FileSystemObject
    Set os = fso.OpenTextFile(Path & FileName & ".cdxml", ForWriting, True)
    os.Write (cdxml)
    os.Close
    
CatchError:
End Sub

Function CanonicalCode(ByVal g As ChemDrawControl9Ctl.Group)
    Dim molServerMol As MolServer9.Molecule
    Dim clipFormat As Long

    clipFormat = RegisterClipboardFormat("ChemDraw Interchange Format")

    Set molServerMol = New MolServer9.Molecule

    molServerMol.DataObject.SetData g.Objects.DataObject.GetData(clipFormat), clipFormat
    CanonicalCode = molServerMol.CanonicalCode()

    Set molServerMol = Nothing
End Function

