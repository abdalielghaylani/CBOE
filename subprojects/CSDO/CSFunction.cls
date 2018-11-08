VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CSFunction"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Public Enum CSFunctionType
    kCSFTBase64Cdx
    kCSFTStruct
    kCSFTStructFile
    kCSFTExactStruct
    kCSFTSubStruct
    kCSFTSimilarity
    kCSFTUnknown
End Enum

' function data
Private m_Type As CSFunctionType
Private m_Name As String
Private m_UseForInput As Boolean
Private m_UseForOutput As Boolean

' return field
Private m_ReturnField As CSField

Public Property Get FunctionType() As CSFunctionType
    FunctionType = m_Type
End Property

Friend Property Let FunctionType(aType As CSFunctionType)
    m_Type = aType
    ' set default attributes at this time
    Select Case m_Type
        Case kCSFTBase64Cdx
            m_Name = "BASE64CDX"
            m_UseForInput = False
            m_UseForOutput = True
        Case kCSFTStruct
            m_Name = "STRUCT"
            m_UseForInput = True
            m_UseForOutput = False
        Case kCSFTStructFile
            m_Name = "STRUCTFILE"
            m_UseForInput = False
            m_UseForOutput = True
        Case kCSFTExactStruct
            m_Name = "EXACTSTRUCT"
            m_UseForInput = True
            m_UseForOutput = False
        Case kCSFTSubStruct
            m_Name = "SUBSTRUCT"
            m_UseForInput = True
            m_UseForOutput = False
        Case kCSFTSimilarity
            m_Name = "SIMILARITY"
            m_UseForInput = True
            m_UseForOutput = False
        Case kCSFTUnknown
            m_Name = ""
            m_UseForInput = False
            m_UseForOutput = False
    End Select
End Property

Public Property Get Name() As String
    Name = m_Name
End Property

Friend Property Let Name(aName As String)
    m_Name = aName
    ' set default attributes at this time
    Select Case m_Name
        Case "BASE64CDX"
            Me.FunctionType = kCSFTBase64Cdx
        Case "STRUCT"
            Me.FunctionType = kCSFTStruct
        Case "STRUCTFILE"
            Me.FunctionType = kCSFTStructFile
        Case "EXACTSTRUCT"
            Me.FunctionType = kCSFTExactStruct
        Case "SUBSTRUCT"
            Me.FunctionType = kCSFTSubStruct
        Case "SIMILARITY"
            Me.FunctionType = kCSFTSimilarity
        Case Else
            Me.FunctionType = kCSFTUnknown
    End Select
End Property

Public Property Get UseForInput() As Boolean
    UseForInput = m_UseForInput
End Property

Public Property Get UseForOutput() As Boolean
    UseForOutput = m_UseForOutput
End Property

Public Property Get ReturnField() As CSField
    Set ReturnField = m_ReturnField
End Property

Private Sub Class_Initialize()
    Set m_ReturnField = New CSField
End Sub

Private Sub Class_Terminate()
    Set m_ReturnField = Nothing
End Sub

Friend Function CreateFromSQL(sql As String) As CSFunctionType
    ' Create function from SQL
    ' right now we only handle EXACTSTRUCT, SUBSTRUCT, SIMILARITY, BASE64CDX, and STRUCTFILE
    Dim arr1() As String
    Dim arr2() As String
    Dim arr3() As String
    Dim arr4() As String
    Dim arr5() As String
    Dim temps1 As String
    Dim temps2 As String
    Dim temps3 As String
    Dim rawMask As String

    arr5 = Split(sql, "(", 2)
    arr1 = Split(Trim(arr5(0)), " ")
    If UBound(arr1) > -1 Then
        Me.Name = Trim(arr1(UBound(arr1)))
    Else
        Me.Name = sql
    End If
    
    Select Case m_Type
        Case kCSFTExactStruct, kCSFTSubStruct, kCSFTSimilarity
            m_ReturnField.Comparator = ">"
            arr2 = Split(arr5(1), ">")
            m_ReturnField.Value = Trim(arr2(1))
            arr3 = Split(arr2(0), ",", 2)
            ' return full table.name qualifier in DefaultName field (table.STRUCTURE, usually)
            m_ReturnField.DefaultName = arr3(0)
            ' parse inner STRUCT function
            Dim innerStruct As CSFunction
            Set innerStruct = New CSFunction
            innerStruct.CreateFromSQL arr3(1)
            m_ReturnField.fieldtype = innerStruct.ReturnField.fieldtype
            m_ReturnField.StrucPath = innerStruct.ReturnField.StrucPath
            Set innerStruct = Nothing
            Select Case m_Type
                Case kCSFTExactStruct
                    m_ReturnField.StrucSearchType = kCSSSTExact
                Case kCSFTSubStruct
                    m_ReturnField.StrucSearchType = kCSSSTSub
                Case kCSFTSimilarity
                    m_ReturnField.StrucSearchType = kCSSSTSim
            End Select
        Case kCSFTStructFile
            arr2 = Split(arr5(1), " AS ")
            temps2 = ""
            If UBound(arr2) = 0 Then
                temps1 = Left(arr5(1), Len(arr5(1)) - 1)
            Else
                temps1 = Left(arr2(0), Len(arr2(0)) - 1)
                temps2 = arr2(1)
            End If
            arr3 = Split(temps1, ",")
            rawMask = Trim(Replace(arr3(1), "'", ""))
            m_ReturnField.StrucPath = Left(rawMask, Len(rawMask) - 4)
            m_ReturnField.StrucExt = Right(rawMask, 4)
            With m_ReturnField
                .fieldtype = kCSFDStructFile
                If temps2 <> "" Then
                    .AssignedName = temps2
                End If
            End With
        Case kCSFTStruct
            Dim format As CSFieldType
            
            arr2 = Split(arr5(1), ",")
            If UBound(arr2) = 0 Then
                ' old style, assumed to be a structfile
                temps1 = arr2(0)
                format = kCSFDStructFile
            Else
                ' check first arg of function for type
                temps1 = arr2(1)
                temps2 = arr2(0)
                If temps2 = "BASE64CDX" Then
                    format = kCSFDBase64Cdx
                Else
                    format = kCSFDStructFile
                End If
            End If
                
            Select Case format
                Case kCSFDBase64Cdx
                    ' want eventually to not use temp file
                    Dim tb As Boolean
                    Dim myPath As String
                    Dim b64cdx As String
                    myPath = GetTmpPath & "temp.cdx"
                    b64cdx = GetFromQuotes(temps1)
                    tb = Decode(b64cdx, myPath)
                    If tb Then
                        With m_ReturnField
                            .StrucPath = myPath
                            .StrucExt = Right(.StrucPath, 4)
                            .Base64CDX = b64cdx
                            .fieldtype = kCSFDStructure
                        End With
                    End If
                Case Else
                    With m_ReturnField
                        .StrucPath = GetFromQuotes(temps1)
                        .StrucExt = Right(.StrucPath, 4)
                        .fieldtype = kCSFDStructure
                    End With
            End Select
        Case kCSFTBase64Cdx
            With m_ReturnField
                arr2 = Split(arr5(1), "AS")
                temps2 = ""
                If UBound(arr2) = 0 Then
                    temps1 = Left(arr5(1), Len(arr5(1)) - 1)
                Else
                    temps1 = Left(arr2(0), Len(arr2(0)) - 1)
                    temps2 = arr2(1)
                End If
                .fieldtype = kCSFDBase64Cdx
                If temps2 <> "" Then
                    .AssignedName = temps2
                End If
            End With
        Case Else
    End Select
    
    CreateFromSQL = m_Type
End Function

Private Function GetFromQuotes(str As String) As String

    Dim fqp As Long
    Dim sqp As Long
    
    fqp = InStr(str, "'")
    sqp = InStr(fqp + 1, str, "'")
    
    GetFromQuotes = Mid(str, fqp + 1, (sqp - fqp) - 1)
    
End Function
