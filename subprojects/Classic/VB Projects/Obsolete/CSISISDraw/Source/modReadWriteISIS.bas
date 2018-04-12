Attribute VB_Name = "modReadWriteISIS"
Option Explicit

'
'     ISIS FILE READ AND WRITE
'

Function lGetSkidFromRxn(strRxnfile As String) As Long
    Dim lBufId As Long
    Dim lSkId As Long
    Dim lTextFile As Long
    Dim strRetVal As String
    Dim lIndex As Long
    
    lBufId = goObjlib.CreateBuffer(Len(strRxnfile))
    Call goObjlib.OpenVirtualTextFile(lBufId, Len(strRxnfile), "r", lTextFile)
    For lIndex = 1 To Len(strRxnfile)
        Call goObjlib.SetByteInBuffer(lBufId, lIndex, Asc(mid$(strRxnfile, lIndex, 1)))
    Next lIndex
    lGetSkidFromRxn = INVALID_ID
    lGetSkidFromRxn = goObjlib.ReadRxnFile(lTextFile)
    goObjlib.DeleteBuffer lBufId
    Call goObjlib.CloseVirtualFile(lTextFile)
End Function

Function GetRxnFileString(lSkId As Long) As String
    Dim lBufId As Long
    Dim lTextFile As Long
    Dim lIndex As Long
    Dim strRetVal As String
    Const RET_BUF_SIZE = 100000
    Dim IgnoredFlag As Boolean
    
    lBufId = goObjlib.CreateBuffer(RET_BUF_SIZE)
    'If lBufId = INVALID_ID Then Err.Raise ERR_IMOL_NO_MEMORY_FOR_BUFFER, Source, "CreateBuffer returned false for length " & RET_BUF_SIZE
    goObjlib.OpenVirtualTextFile lBufId, RET_BUF_SIZE, "w", lTextFile
    'If lTextFile = INVALID_ID Then Err.Raise ERR_IMOL_VIRTUALTEXTFILE, Source, "Virtual Text File would not open to save MolFile"
        If goObjlib.WriteRxnFile(lTextFile, lSkId, IgnoredFlag) Then
            goObjlib.CloseVirtualFile lTextFile
        strRetVal = ""
        'If lBufId <= 0 Then Err.Raise ERR_IDB_INVALID_BUFFER, Source, "Invalid buffer to get string. Bufferid = " & lBufId
        For lIndex = 1 To goObjlib.GetBufferTextSize(lBufId) - 1
            strRetVal = strRetVal & Chr$(goObjlib.GetByteFromBuffer(lBufId, lIndex))
        Next lIndex
        goObjlib.DeleteBuffer lBufId
    Else
        'Err.Raise ERR_IMOL_WRITE_VMOLFILE, Source, "Mol File could not be written in Virtual Text File (ID=" & lTextFile & ")"
        goObjlib.CloseVirtualFile lTextFile
    End If
    GetRxnFileString = strRetVal
End Function

'
'     MOL FILE READ AND WRITE
'

Function lGetSkidFromMol(strMolfile As String) As Long
    Dim lBufId As Long
    Dim lSkId As Long
    Dim lTextFile As Long
    Dim strRetVal As String
    Dim lIndex As Long
    
    lBufId = goObjlib.CreateBuffer(Len(strMolfile))
    Call goObjlib.OpenVirtualTextFile(lBufId, Len(strMolfile), "r", lTextFile)
    For lIndex = 1 To Len(strMolfile)
        Call goObjlib.SetByteInBuffer(lBufId, lIndex, Asc(mid$(strMolfile, lIndex, 1)))
    Next lIndex
    lGetSkidFromMol = INVALID_ID
    lGetSkidFromMol = goObjlib.ReadMolFile(lTextFile)
    goObjlib.DeleteBuffer lBufId
    Call goObjlib.CloseVirtualFile(lTextFile)
End Function

Function GetMolFileString(lSkId As Long) As String
    Dim lBufId As Long
    Dim lTextFile As Long
    Dim lIndex As Long
    Dim strRetVal As String
    Const RET_BUF_SIZE = 100000
    
    lBufId = goObjlib.CreateBuffer(RET_BUF_SIZE)
    'If lBufId = INVALID_ID Then Err.Raise ERR_IMOL_NO_MEMORY_FOR_BUFFER, Source, "CreateBuffer returned false for length " & RET_BUF_SIZE
    goObjlib.OpenVirtualTextFile lBufId, RET_BUF_SIZE, "w", lTextFile
    'If lTextFile = INVALID_ID Then Err.Raise ERR_IMOL_VIRTUALTEXTFILE, Source, "Virtual Text File would not open to save MolFile"
        If goObjlib.WriteMolFile(lTextFile, lSkId) Then
            goObjlib.CloseVirtualFile lTextFile
        strRetVal = ""
        'If lBufId <= 0 Then Err.Raise ERR_IDB_INVALID_BUFFER, Source, "Invalid buffer to get string. Bufferid = " & lBufId
        For lIndex = 1 To goObjlib.GetBufferTextSize(lBufId) - 1
            strRetVal = strRetVal & Chr$(goObjlib.GetByteFromBuffer(lBufId, lIndex))
        Next lIndex
        goObjlib.DeleteBuffer lBufId
    Else
        'Err.Raise ERR_IMOL_WRITE_VMOLFILE, Source, "Mol File could not be written in Virtual Text File (ID=" & lTextFile & ")"
        goObjlib.CloseVirtualFile lTextFile
    End If
    GetMolFileString = strRetVal
End Function

'
'     START SKC FILE READ AND WRITE, END MOL FILE READ AND WRITE
'

Function lGetSkidFromSkc(bSkcFile() As Byte) As Long
    Dim lBufId As Long
    Dim lSkId As Long
    Dim lTextFile As Long
    Dim strRetVal As String
    Dim lIndex As Long
    Dim ldataSgroups As Long
    Dim i As Long
    Dim SGroupId As Long
    Dim bSuccess As Boolean
    Dim dLeft As Double
    Dim dTop As Double
    Dim SketchMode As Long
    
    With goObjlib
        ' we can't go this as lSkId is null. - .SetSketchMode lSkId, SKMODE_MOL ' The only mode that works with ChemDraw.
        lBufId = .CreateBuffer(UBound(bSkcFile) + 1)
        For lIndex = 0 To UBound(bSkcFile)
            Call .SetByteInBuffer(lBufId, lIndex + 1, bSkcFile(lIndex))
        Next lIndex
        lGetSkidFromSkc = INVALID_ID
        lGetSkidFromSkc = .ReadSketchFromBuffer(lBufId)
        .DeleteBuffer lBufId
    '                             SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
        SketchMode = .GetSketchMode(lGetSkidFromSkc) ' It's SKMODE_MOL - surprisingly
        .SetSketchMode lSkId, SKMODE_MOL ' The only mode that works with ChemDraw.
            ' Test code: TGF files are written differently for each Sketch Mode.
            'lGetSkidFromSkc = .ReadTGFFile("c:\grouped by mix rxn 2.tgf") ' SSDS
            'lGetSkidFromSkc = .ReadTGFFile("c:\data extracted from ChemDraw after double-click - with fix3.tgf")
            'lGetSkidFromSkc = .ReadSketchFile("c:\CaCl2b.skc")
            'lGetSkidFromSkc = .ReadSketchFile("c:\molsalt.skc")
            'lGetSkidFromSkc = .ReadSketchFile("c:\r1p.skc")
            '.SetSketchMode lGetSkidFromSkc, SKMODE_CHEM ' The only mode that works with ChemDraw.
            'Call .WriteTGFFile("C:\test2.tgf", lGetSkidFromSkc)
    End With

End Function

Function GetSkcFileByteArray(lSkId As Long) As Byte()
    Dim lBufId As Long
    Dim lTextFile As Long
    Dim lIndex As Long
    Dim strRetVal As String
    Const RET_BUF_SIZE = 100000
    Dim LongByte As Variant
    Dim bStruct() As Byte

    '                             SKMODE_MOL = 1003 , SKMODE_SKETCH = 1102, SKMODE_CHEM = 1101
    goObjlib.SetSketchMode lSkId, SKMODE_MOL ' The only mode that works with ChemDraw.
    ' Test code:
    '    Call goObjlib.WriteTGFFile("C:\test27.tgf", lSkId)
    lBufId = goObjlib.CreateBuffer(RET_BUF_SIZE)
    'If lBufId = INVALID_ID Then Err.Raise ERR_IMOL_NO_MEMORY_FOR_BUFFER, Source, "CreateBuffer returned false for length " & RET_BUF_SIZE
        If (Not goObjlib.WriteSketchToBuffer(lBufId, lSkId)) Then
        Else
        End If

        'If lBufId <= 0 Then Err.Raise ERR_IDB_INVALID_BUFFER, Source, "Invalid buffer to get string. Bufferid = " & lBufId
        ReDim bStruct(1 To RET_BUF_SIZE) ' FAP TODO - find the actual length.
        For lIndex = 1 To RET_BUF_SIZE ' FAP TODO - find the actual length.
            bStruct(lIndex) = goObjlib.GetByteFromBuffer(lBufId, lIndex)
        Next lIndex
        goObjlib.DeleteBuffer lBufId
    GetSkcFileByteArray = bStruct
End Function

' Utility code for testing:

Private Function GetStructureFromFile2(FilePath As String) As Byte()
    'TEMP:  Read the data in from the file
    Dim NumBytes As Long
    Dim i As Long
    Dim fileNum As Integer
    Dim NewData() As Byte

    fileNum = FreeFile
    Open FilePath For Binary As fileNum
    NumBytes = LOF(fileNum)
    ReDim NewData(1 To NumBytes)

    For i = 1 To NumBytes
        Get #fileNum, , NewData(i)
    Next i

    Close fileNum
    'TEMP ends

    GetStructureFromFile2 = NewData
End Function

Private Function GetStructureFromFile(FilePath As String) As String
    Dim MolLine As String
    Dim Molfile As String
    Dim i As Long
    
    Molfile = ""
        If FilePath <> "" Then
        Open FilePath For Input As #2
        On Error GoTo ENDFILE
        For i = 1 To 10000
            Line Input #2, MolLine
            Molfile = Molfile + MolLine + vbCr + vbLf
            GoTo SKIP
ENDFILE:
            Exit For
SKIP:
        Next i
        Close #2
    End If
    GetStructureFromFile = Molfile
End Function



