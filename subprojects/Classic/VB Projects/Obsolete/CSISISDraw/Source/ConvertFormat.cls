VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ConvertFormat"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Public Function ConvertFormat(vStructIn As Variant, inputFormat As String, outputFormat As String) _
                    As Variant
    Dim lSkId As Long
    Dim strFile As String
    Dim bFile() As Byte
    Dim strRxnfile As String
    Dim skcStructure() As Byte
    
    If goObjlib Is Nothing Then
        Set goObjlib = CreateObject("isisbase32")
    End If
    
    ' Load structure into ISIS.
    If inputFormat = "rxn" Then
        strRxnfile = vStructIn
        lSkId = lGetSkidFromRxn(strRxnfile)
    ElseIf inputFormat = "mol" Then
        strRxnfile = vStructIn
        lSkId = lGetSkidFromMol(strRxnfile)
    ElseIf inputFormat = "skc" Then
        skcStructure = vStructIn
        lSkId = lGetSkidFromSkc(skcStructure)
    Else
        ' error condtition - file type not found.
    End If

    ' Get structure from ISIS.
    If outputFormat = "rxn" Then
        strFile = GetRxnFileString(lSkId)
        ConvertFormat = strFile
    ElseIf outputFormat = "mol" Then
        strFile = GetMolFileString(lSkId)
        ConvertFormat = strFile
    ElseIf outputFormat = "skc" Then
        bFile = GetSkcFileByteArray(lSkId)
        ConvertFormat = bFile
    Else
        ' error condtition - file type not found.
    End If
    
    goObjlib.DeleteSketch lSkId 'otherwise ISIS memory leak !!
    Set goObjlib = Nothing
End Function


