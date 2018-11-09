VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSField"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Public Enum CSFieldType
    ' note these must be in order, starting with 0
    ' kCSFDMol_Id         ' TODO - deprecated
    kCSFDFormula
    kCSFDMolWt
    kCSFDSmiles
    kCSFDStructure
    kCSFDStructFile
    kCSFDBase64Cdx
    kCSFDRelational
    kCSFDUnknown
End Enum

Public Enum CSStrucSearchType
    kCSSSTSub
    kCSSSTExact
    kCSSSTSim
End Enum

Private m_Type As CSFieldType                       ' CSFieldType
Private m_DefaultName As String                     ' name to give field if user assigns none
Private m_AssignedName As String                    ' user assigned name
Private m_DefaultSize As Long                       ' default size of field
Private m_ADOType As Long                           ' ADO Type of field
Private m_ADOFieldAttrs As ADODB.FieldAttributeEnum ' ADO field attributes
Private m_Value As Variant                          ' value of the field
Private m_Comparator As String                      ' comparator (>, <, LIKE, =, AS)
Private m_StrucSearchType As String                 ' Structure Search Type
Private m_StrucPath As String                       ' Structure Path
Private m_StrucExt As String                        ' Structure Extension
Private m_Base64CDX As String                       ' Structure in Base64 CDX format
Private m_ChemSearchOptions As ChemSearchOptions    ' chemical search options

Public Property Get fieldtype() As CSFieldType
    fieldtype = m_Type
End Property

Friend Property Let fieldtype(aFieldType As CSFieldType)
    m_Type = aFieldType
    ' when assigning type, assign rest of things as well
    Select Case m_Type
'        Case kCSFDMol_Id
'            m_DefaultName = "MOL_ID"
'            m_DefaultSize = 100
'            m_ADOType = adDouble
'            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDFormula
            m_DefaultName = "FORMULA"
            m_DefaultSize = 100
            m_ADOType = adChar
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDMolWt
            m_DefaultName = "MOLWT"
            m_DefaultSize = 100
            m_ADOType = adDouble
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDSmiles
            m_DefaultName = "SMILES"
            m_DefaultSize = 1024
            m_ADOType = adChar
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDStructure
            m_DefaultName = "STRUCTURE"
            m_DefaultSize = -1
            m_ADOType = adBinary
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDStructFile
            m_DefaultName = "STRUCTFILE"
            m_DefaultSize = 1024
            m_ADOType = adChar
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDBase64Cdx
            m_DefaultName = "BASE64CDX"
            m_DefaultSize = 10000
            m_ADOType = adChar
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDRelational
            m_DefaultName = ""
            m_DefaultSize = -1
            m_ADOType = adChar
            m_ADOFieldAttrs = adFldUpdatable
        Case kCSFDUnknown
            m_DefaultName = ""
            m_DefaultSize = -1
            m_ADOType = adBinary
            m_ADOFieldAttrs = adFldUpdatable
    End Select
End Property

Public Property Get DefaultName() As String
    DefaultName = m_DefaultName
End Property

Friend Property Let DefaultName(aName As String)
    m_DefaultName = aName
    ' set up other properties by calling FieldType property
    Select Case m_DefaultName
'        Case "MOL_ID"
'            Me.fieldtype = kCSFDMol_Id
        Case "FORMULA"
            Me.fieldtype = kCSFDFormula
        Case "MOLWT"
            Me.fieldtype = kCSFDMolWt
        Case "SMILES"
            Me.fieldtype = kCSFDSmiles
        Case "STRUCTURE"
            Me.fieldtype = kCSFDStructure
        Case "STRUCTFILE"
            Me.fieldtype = kCSFDStructFile
        Case "BASE64CDX"
            Me.fieldtype = kCSFDBase64Cdx
        Case Else
            Me.fieldtype = kCSFDUnknown
    End Select
End Property

Public Property Get ADOType() As Long
    ADOType = m_ADOType
End Property

Public Property Get DefaultSize() As Long
    DefaultSize = m_DefaultSize
End Property

Public Property Get ADOFieldAttrs() As ADODB.FieldAttributeEnum
    ADOFieldAttrs = m_ADOFieldAttrs
End Property

Public Property Get AssignedName() As String
    If m_AssignedName <> "" Then
        AssignedName = m_AssignedName
    Else
        AssignedName = m_DefaultName
    End If
End Property

Friend Property Let AssignedName(aAssignedName As String)
    m_AssignedName = aAssignedName
End Property

Public Property Get Value() As Variant
    Value = m_Value
End Property

Friend Property Let Value(aValue As Variant)
    m_Value = aValue
End Property

Public Property Get Comparator() As String
    Comparator = m_Comparator
End Property

Friend Property Let Comparator(aComp As String)
    m_Comparator = aComp
End Property

Public Property Get StrucPath() As String
    StrucPath = m_StrucPath
End Property

Friend Property Let StrucPath(aPath As String)
    m_StrucPath = aPath
End Property

Public Property Get StrucExt() As String
    StrucExt = m_StrucExt
End Property

Friend Property Let StrucExt(aExt As String)
    m_StrucExt = aExt
End Property

Public Property Get Base64CDX() As String
    Base64CDX = m_Base64CDX
End Property

Friend Property Let Base64CDX(aStr As String)
    m_Base64CDX = aStr
End Property

Public Property Get StrucSearchType() As CSStrucSearchType
    StrucSearchType = m_StrucSearchType
End Property

Friend Property Let StrucSearchType(aType As CSStrucSearchType)
    m_StrucSearchType = aType
End Property

Public Property Get ChemSearchOptions() As ChemSearchOptions
    Set ChemSearchOptions = m_ChemSearchOptions
End Property

Public Property Set ChemSearchOptions(aChemSearchOptions As ChemSearchOptions)
    Set m_ChemSearchOptions = aChemSearchOptions
End Property

Private Sub Class_Initialize()
    m_Value = ""
End Sub

Friend Function CreateFromSQL(sql As String) As CSFieldType
    ' create from sql
    Dim arr1() As String
    Dim arr2() As String
    Dim arr3() As String
    Dim arr4() As String
    Dim temps1 As String
    Dim temps2 As String
    Dim temps3 As String
    Dim comp As String
    Dim MinMW, MaxMW As Double
    
    ' find comparator if it exists
    If InStr(sql, " IN ") Then
        comp = " IN "
    ElseIf InStr(sql, "=<") Then
        comp = "=<"
    ElseIf InStr(sql, ">=") Then
        comp = ">="
    ElseIf InStr(sql, "<") Then
        comp = "<"
    ElseIf InStr(sql, ">") Then
        comp = ">"
    ElseIf InStr(sql, "=") Then
        comp = "="
    ElseIf InStr(1, sql, " LIKE ", vbTextCompare) Then
        comp = " LIKE "
    ElseIf InStr(1, sql, " AS ", vbTextCompare) Then
        comp = " AS "
    End If
    m_Comparator = Trim(comp)
    
    If comp <> "" Then
        arr1 = Split(sql, comp, 2)
        arr4 = Split(Trim(arr1(0)), " ")
        If comp = " AS " Then
            ' in SELECT statement
            m_AssignedName = Trim(arr1(1))
        ElseIf comp = " IN " Then
            m_Value = Trim(arr1(1))
        Else
            ' in WHERE statement
            m_Value = Replace(Trim(arr1(1)), "'", "")
        End If
        temps1 = arr4(UBound(arr4))
    Else
        arr4 = Split(Trim(sql), " ")
        temps1 = arr4(UBound(arr4))
    End If
    
    arr2 = Split(temps1, ".")
    If UBound(arr2) = 0 Then
        ' case [FieldName]
        Me.DefaultName = Trim(arr2(0))
    Else
        ' case [TableName].[FieldName]
        Me.DefaultName = Trim(arr2(1))
    End If
    
    ' create molwt range if field is molwt type and
    ' equality is sought - this is code that is in
    ' CFW but was left out of MolServer
    ' DGB reimplemented using CFW Range Searching Alorithm
    If m_Type = kCSFDMolWt And m_Value <> "0" And m_Comparator = "=" And InStr(m_Value, "-") = 0 Then
        GetSearchRange m_Value, MinMW, MaxMW
        m_Value = CStr(MinMW) & " - " & CStr(MaxMW)
    End If
    
    CreateFromSQL = m_Type
End Function

    



'Function GetSigFigs derived from Cs_RealNum:ParseString C++ code used by CFW
'ParseString   Examines the provided string, which represents a real number, possibly      '
'       in exponential notation, and determines the number of significant figures present.  '
'       In the conventional simple and scientific representations of real numbers, there is '
'       one region within the string that represents the significant digits.  The position  '
'       of this region given by (start,finish).                                             '
'                                                                                           '
'       To determine this region:                                                           '
'           1)  Locate the decimal point.                                                   '
'           2)  Remove a leading minus or plus sign.                                        '
'                                                                                           '
'           3)  Remove leading zeroes.                                                      '
'           4)  Strip off everything including and following an exponent (E).               '
'           5)  If a decimal point is not present, strip off trailing zeroes.               '
'           6)  Count the number of characters left, ignoring any decimal point.            '
'           7)  Every number is considered to have *at least* one significant figure.       '
'                                                                                           '
'       Examples of "significant regions" are:                                              '
'               "-003.14E01"    -> "3.14",      3 sig figs.                                 '
'               "0.0300"        -> ".0300",     3 sig figs.                                 '
'               "3020"          -> "3020",      3 sig figs.                                 '
'               ".00E-7"        -> ".00",       1 sig figs.                                 '
'       Penned by HEH, 12/15/95.                                                            '
'       Ported by DGB 7/2003                                                                '
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Function GetSigFigs(src)
    Dim decPt, finish, exponent As String
    Dim Start, numSigFigs As Integer
    
    ' 1) Locate the decimal point.
    decPt = InStr(src, ".")
    If decPt >= 0 Then decPt = decPt - 1 ' -1 if not present
    ' 2-3) Remove leading zeroes, pluses, minuses (and periods for chuckles).
    Start = 0
   
    While Start < Len(src) And InStr("+-0.", Mid(src, Start + 1, 1))
        Start = Start + 1
    Wend

    finish = Len(src) - 1

    ' 4) Strip off an exponent and everything after.
    exponent = InStr(LCase(src), "e")
    If exponent >= 0 Then exponent = exponent - 1
    If exponent > 0 Then
        finish = exponent - 1
    End If
    ' 5) If a decimal point is not present, strip off trailing zeroes.
    If decPt = -1 Then
        While (finish > Start And Mid(src, finish + 1, 1) = "0")
            finish = finish - 1
        Wend

    End If
    ' 5.5) Handle the special case of ".00" (with or without a trailing exponential).
    If finish < Start Then
    
        Start = 0
        While Start < Len(src) And InStr("+-0", Mid(src, Start + 1, 1))
            Start = Start + 1
        Wend
        numSigFigs = 1
    Else

    '   6)  Count the number of characters left, ignoring any decimal point.
    '       start and finish are the first and last digits in the significant region.

        numSigFigs = finish - Start + 1

        If decPt > Start And decPt < finish Then
            numSigFigs = numSigFigs - 1
        End If
    End If

    ' 7) Every number has at least one significant figure.
    If (numSigFigs < 1) Then
        numSigFigs = 1
    End If
    GetSigFigs = numSigFigs
End Function

Sub GetSearchRange(src, ByRef loVal, ByRef hiVal)
    Dim v, figs, tol, delta As Double
    
    v = CDbl(src)
    figs = GetSigFigs(src)
    tol = SetToleranceFromSigFig(v, figs)
    delta = 0.5 * tol
    If delta > 1 Then delta = 0.5
    
    loVal = v - delta
    hiVal = v + delta
End Sub

Function SetToleranceFromSigFig(v, numsigfig)
        SetToleranceFromSigFig = 10 ^ (PowerOfMostSignificantFig(v) - numsigfig + 1)
End Function

Function PowerOfMostSignificantFig(val)
    PowerOfMostSignificantFig = Floor(Log10(Abs(val)))
End Function

Function Log10(X)
   Log10 = Log(X) / Log(10)
End Function

Function Floor(ByVal n)
    Dim iTmp, bErr
    On Error Resume Next
    n = CDbl(n)
    If Err Then bErr = True
    On Error GoTo 0
    If bErr Then Err.Raise 5000, "Floor Function", _
        "Input must be convertible to a sub-type of double"
    iTmp = Round(n)
    'test rounded value against the non rounded value
    'if greater, subtract 1
    If iTmp > n Then iTmp = iTmp - 1
    Floor = CInt(iTmp)
End Function
