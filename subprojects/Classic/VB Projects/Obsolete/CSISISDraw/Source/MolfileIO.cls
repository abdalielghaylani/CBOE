VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "MolfileIO"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

' # Molfile IO routines for Organon E-Notebook

' # This function accepts cdxml structure data from E-Notebook, and
' # converts it to a mol file.

Dim mfrm As frmMDLbox

Public Function ConvertCdxmlToMolfileWithDataSgroups(ByVal cdxml As String) As String
    Dim frm As New frmEditStructure
    Dim lSkId As Long
    Dim dsd() As DataSgroupData
    Dim Molfile As String
    Dim Sketch As Variant
    Dim bSkcFile() As Byte
    Dim NReactants As Long
    Dim NProducts As Long
    Dim MWReactant1 As Double
    Dim MWProduct1 As Double
    
    If goObjlib Is Nothing Then
        Set goObjlib = CreateObject("isisbase32")
    End If
    
    With frm.mCDXDrawing
    
        .DataEncoded = False
        ' Load the structure into ChemDraw.
        If (Len(cdxml) > 0) Then
            .SourceURL = "data:chemical/x-cdx;base64," & cdxml
        End If
        dsd = ExtractDataSgroupsFromChemDraw(frm.mCDXDrawing, NReactants, NProducts, MWReactant1, MWProduct1) ' Get Data Sgroup data of ChemDraw Object Tags.
        Sketch = .Data("skc")           ' Use ChemDraw to convert structure to a "skc" 'file'.
        bSkcFile = Sketch               ' Cast Variant to Byte Array
        lSkId = lGetSkidFromSkc(bSkcFile) ' Use the MDL ADK to read the skc file into a sketch.
        If lSkId = 0 Then
            Call MsgBox("ISISlib failed to read skc file.", vbOKOnly, "MolfileIO error")
        End If
        Call ConvertSaltSubsketchesToMols(lSkId)     ' Work around CSBR-57006.
        Call RemoveColors(lSkId)  ' Colors cause V3000 mol files to be written.
        Call AddDataSgroupsToISISSketch(lSkId, dsd)  ' Create Data Sgroups with the MDL ADK.
        Molfile = GetMolFileString(lSkId)              ' Use MDL ADK to produce a mol file with Data Sgroups.
        goObjlib.DeleteSketch (lSkId)                  ' Avoid a memory leak.
        ConvertCdxmlToMolfileWithDataSgroups = Molfile
        Call WarnV3000(Molfile, False) ' Fix CSBR-59756.
        
    End With
    
    Set frm = Nothing
End Function

' # This function accepts mol file structure data from, say, a Reagent Selector or Registration system, and
' # converts it to a cdxml file.

Public Function ConvertMolfileToCdxmlWithDataSgroups(ByVal Molfile As String) As Variant ' a CDXML string
    Dim frm As New frmEditStructure
    Dim lSkId As Long
    Dim dsd() As DataSgroupData
    Dim Sketch As Variant
    Dim oldEncoded As Boolean
    Dim bStruct() As Byte
    Dim NReactants As Long
    Dim NProducts As Long
    
    Call WarnV3000(Molfile, True) ' Fix CSBR-59756.
    If goObjlib Is Nothing Then
        Set goObjlib = CreateObject("isisbase32")
    End If
    
    With frm.mCDXDrawing
    
        .DataEncoded = False
        lSkId = lGetSkidFromMol(Molfile) ' Use the MDL ADK to read the mol file into a sketch.
        If lSkId <> 0 Then
            Call goObjlib.LayoutChiralFlag(lSkId)  ' Put Chiral Flag in a good place.
            Call GroupFragments(lSkId)       ' Group the fragments.
            dsd = ExtractDataSgroupsFromISISSketch(lSkId)  ' Read the Data Sgroups from the sketch.
            ' Set display of implicit hydrogens to to ChemDraw (and Organon) default. (CSBR-58484)
            ' TODO: Get this configuration information from somewhere?
            Call goObjlib.SetSketchDisplayHydrogens(lSkId, HYDROGENS_HETERO)
            Sketch = GetSkcFileByteArray(lSkId)            ' Get a "skc" 'file' from the MDL ADK.
            goObjlib.DeleteSketch (lSkId)                  ' Avoid a memory leak.
            
            .Objects.Clear                                 ' Read the data and settings into ChemDraw.
            oldEncoded = .DataEncoded
            .DataEncoded = False
            bStruct = Sketch ' Convert to byte array.
            ' Load data and settings into ChemDraw.
            .SourceURL = "data:chemical/x-mdl-isis;base64," & Base64Encode(bStruct)
            .DataEncoded = oldEncoded
            
            ' Convert the Data Sgroups into ChemDraw ObjectTags.
            Call SaveDataSgroupsInChemDraw(frm.mCDXDrawing, dsd, NReactants, NProducts)
            ConvertMolfileToCdxmlWithDataSgroups = .Data("text/xml")  ' Get the cdxml from ChemDraw.
            If ConvertMolfileToCdxmlWithDataSgroups = "" Then
                Call MsgBox("ChemDraw returned a null file - check ChemDraw acrivation.", vbOKOnly, "MolfileIO error")
            End If
        Else
            Call MsgBox("ISISlib failed to read mol file.", vbOKOnly, "MolfileIO error")
            ConvertMolfileToCdxmlWithDataSgroups = .Data("text/xml")  ' Get a null cdxml from ChemDraw.
        End If
    End With
    
    Set frm = Nothing
End Function

Private Sub ConvertSaltSubsketchesToMols(ByRef lSkId As Long)
    Dim i As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim TotalSkObjs As Long

    ' Convert Subsketches to mols.
    With goObjlib
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
    End With
End Sub

Private Sub GroupFragments(ByRef lSkId As Long)
    Dim i As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim TotalSkObjs As Long
    Dim SaveSketchMode As Long
    Dim NFragments As Long
    Dim oldEncoded As Boolean
    Dim bStruct() As Byte
    
    With goObjlib
        SaveSketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        Call .SetSketchMode(lSkId, SKMODE_MOL)
        
        TotalSkObjs = .GetTotalSkObjs(lSkId)
        
        ' Count the mol sketch objects.
        NFragments = 0
        For i = 1 To TotalSkObjs
            skObjId = .GetSkObjId(lSkId, i)
            skObjType = .GetSkObjType(skObjId)
            If skObjType = SKOBJTYPE_MOL Then
                NFragments = NFragments + 1
            End If
        Next i
        
        If NFragments > 1 Then
            ' Group all the mol sketch objects.
            For i = 1 To TotalSkObjs
                skObjId = .GetSkObjId(lSkId, i)
                skObjType = .GetSkObjType(skObjId)
                If skObjType = SKOBJTYPE_MOL Then
                    Call .SelectSkObj(skObjId)
                End If
            Next i
            Call .GroupSelectedMolObj(lSkId)
            Call .DeselectAllSkObj(lSkId)
        End If
        
        Call .SetSketchMode(lSkId, SaveSketchMode)
    End With
End Sub

Private Sub RemoveColors(lSkId As Long)
    Dim i As Long
    Dim skObjId As Long
    Dim skObjType As Long
    Dim TotalSkObjs As Long
    Dim SaveSketchMode As Long
    Dim NFragments As Long
    Dim atomID As Long
    Dim Red As Long
    Dim Green As Long
    Dim Blue As Long
    
    With goObjlib
        SaveSketchMode = .GetSketchMode(lSkId) ' SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        Call .SetSketchMode(lSkId, SKMODE_CHEM)
        
        For i = 1 To .GetTotalAtoms(lSkId)
            skObjId = .GetAtomId(lSkId, i)
            Call .SetSkObjPenColor(skObjId, 0, 0, 0)
        Next i
        For i = 1 To .GetTotalBonds(lSkId)
            skObjId = .GetBondId(lSkId, i)
            Call .SetSkObjPenColor(skObjId, 0, 0, 0)
        Next i
        
        Call .SetSketchMode(lSkId, SaveSketchMode)
    End With
End Sub

Private Sub WarnV3000(Molfile As String, bInput As Boolean)
    If (InStr(1, Molfile, "V3000") <> 0) Then
        If (bInput) Then
            MsgBox "A V3000 mol file has been received.", vbOKOnly, "ISIS/Draw mol file I/O Warning"
        Else
            MsgBox "A V3000 mol file has been generated.", vbOKOnly, "ISIS/Draw mol file I/O Warning"
        End If
    End If
End Sub


' # Below, we instantiate the mdlbox control.  This is necessary to work around an error
' # in the development environment if we run more than once.
' # The error is:
' #    Error number -2147023782, "Automation error -
' #    A dynamic link library (DLL) initialization routine failed."
' # This occurs when attempting:
' #    Set goObjlib = CreateObject("isisbase32")

Private Sub Class_Initialize()
    Set mfrm = New frmMDLbox
    Load mfrm
End Sub

Private Sub Class_Terminate()
    Unload mfrm
    Set mfrm = Nothing
End Sub
