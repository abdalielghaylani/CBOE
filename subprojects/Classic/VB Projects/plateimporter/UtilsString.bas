Attribute VB_Name = "UtilsString"
Option Explicit

' utilities for strings - note that vbflexgrid comes with a good string package,
' and component one elastic comes with an awk implementation

Public Function StrMatch(str1 As String, str2 As String) As Boolean
    StrMatch = (StrComp(str1, str2, vbTextCompare) = 0)
End Function

Public Function StrSingleQuote(ByVal str As String)
    StrSingleQuote = "'" & str & "'"
End Function

Public Function StrPluralize(intCount As Integer, strSingular As String, Optional strPlural As String) As String
    If intCount = 1 Then
        StrPluralize = strSingular
    ElseIf strSingular = strPlural Then ' hack for making plurals disappear; enables the indefinite article
        StrPluralize = ""
    ElseIf IsMissing(strPlural) Then
        StrPluralize = strSingular & "s"
    ElseIf Len(strPlural) = 0 Then
        StrPluralize = strSingular & "s"
    Else
       StrPluralize = strPlural
    End If
End Function

Public Function StrAsDouble(ByVal str As String) As Double
    Dim lRet As Double
    If str = "" Or Not IsNumeric(str) Or str = "0" Then
        lRet = 0
    Else
        lRet = CDbl(str)
    End If
    StrAsDouble = lRet
End Function

Public Function StrAsLong(ByVal str As String) As Long
    Dim lRet As Long
    If str = "" Or Not IsNumeric(str) Or str = "0" Then
        lRet = NULL_AS_LONG
    Else
        lRet = CLng(str)
    End If
    StrAsLong = lRet
End Function

Public Function IsSpaces(ByVal str As String) As Boolean
    ' TODO - boy this is a hack
    ' return true if string is nothing but spaces
    If str = " " Or str = "  " Or str = "   " Or str = "    " Or str = "       " Then
        IsSpaces = True
    End If
End Function

Public Function StrSetChr(ByVal rhs As String, ByVal char As String, ByVal Index As Long) As String
    ' replaces the character at the given position
    StrSetChr = Left(rhs, Index - 1) & char & Right(rhs, Len(rhs) - Index)
End Function

Public Function Oracleize(ByVal rhs As String) As String
    ' remove quotes and spaces, misc chars

    ' uppercase everything
    Oracleize = UCase(rhs)
        
    Oracleize = Replace(Oracleize, " ", "_")
    Oracleize = Replace(Oracleize, "%", "PCT")
    Oracleize = Replace(Oracleize, "+", "POS")
    Oracleize = Replace(Oracleize, "-", "NEG")
    Oracleize = Replace(Oracleize, "/", "DIV")
    Oracleize = Replace(Oracleize, ".", "_")
    
    ' now remove any other non-alphabetic, non-numeric, and non-underscore characters
    Dim I As Integer, sCopy As String, sChar As String
    sCopy = Oracleize
    Oracleize = ""
    Dim lCount As Long: lCount = 0
    For I = 1 To Len(sCopy)
        sChar = Mid(sCopy, I, 1)
        If (Asc(sChar) > 64 And Asc(sChar) < 91) Or (Asc(sChar) > 47 And Asc(sChar) < 58) Or Asc(sChar) = 95 Then
            If lCount = 0 And Not (Asc(sChar) > 64 And Asc(sChar) < 91) Then
                ' make sure first character is alphabetic
                Oracleize = Oracleize & "N"
            End If
            Oracleize = Oracleize & sChar
            lCount = lCount + 1
        End If
    Next
End Function

Public Function IsAlphabetic(ByVal sChar As String) As Boolean
    ' return true sChar is A-Z or a-z
    Dim lcode As Long: lcode = Asc(sChar)
    If (lcode > 64 And lcode < 91) Or (lcode > 96 And lcode < 122) Then
        IsAlphabetic = True
    End If
End Function

Public Function OracleDateString(ByVal vDate) As String
    Dim dDt As Date, stime As String, sdate As String
    dDt = CDate(vDate)
    stime = Format(dDt, "hh:mm:ss")
    sdate = Format(dDt, "mm/dd/yyyy")
    OracleDateString = "to_date('" & sdate & " " & stime & "','mm/dd/yyyy hh24:mi:ss')"
End Function

Public Function IsAlpha(ByVal rhs As String) As Boolean
    IsAlpha = rhs Like "[A-Za-z]"
End Function

Public Function SQLQuote(ByVal rhs As Variant) As String
If IsNumeric(rhs) Then
        SQLQuote = rhs
    Else
        SQLQuote = "'" & rhs & "'"
    End If
End Function

