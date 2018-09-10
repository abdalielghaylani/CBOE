<%
'========================================================================
' MODULE:    cASPString.asp
' AUTHOR:    www.u229.no
' CREATED:   May 2006
'========================================================================
' COMMENT: A fast string class for classic ASP.               
'========================================================================
' ROUTINES:

' - Public Property Get NumberOfStrings()
' - Public Property Get NumberOfBytes()
' - Public Property Get NumberOfCharacters()
' - Private Sub Class_Initialize()
' - Public Sub Append(sNewString)
' - Public Function ToString()
' - Public Sub Reset()
'========================================================================


'========================================================================
Class ASPString
'========================================================================

'// MODULE VARIABLES
Dim m_sArr            '// Array holding the strings.
Dim m_lResize        '// Factor for rezising the array.
Dim m_lStrings       '// Number of strings appended.

'// PROPERTIES
Public Property Get NumberOfStrings()
    NumberOfStrings = m_lStrings
End Property

Public Property Get NumberOfBytes()
    NumberOfBytes = LenB(Join(m_sArr, ""))
End Property

Public Property Get NumberOfCharacters()
    NumberOfCharacters = Len(Join(m_sArr, ""))
End Property

'------------------------------------------------------------------------------------------------------------
' Comment: Initialize default values.
'------------------------------------------------------------------------------------------------------------
Private Sub Class_Initialize()
    m_lResize = CLng(50)
    m_lStrings = CLng(0)
    ReDim m_sArr(m_lResize)
End Sub

'------------------------------------------------------------------------------------------------------------
' Comment: Add a new string to the string array.
'------------------------------------------------------------------------------------------------------------
Public Sub Append(sNewString)

    If Len(sNewString & "") = 0 Then Exit Sub
    
    '// If we have filled the array, resize it.
    If m_lStrings > UBound(m_sArr) Then ReDim Preserve m_sArr(UBound(m_sArr) + m_lResize)

    '// Append the new string to the next unused position in the array.
    m_sArr(m_lStrings) = sNewString
    m_lStrings = (m_lStrings + 1)
End Sub

'------------------------------------------------------------------------------------------------------------
' Comment: Return the strings as one big string.
'------------------------------------------------------------------------------------------------------------
Public Function ToString()
    ToString = Join(m_sArr, "")
End Function

'------------------------------------------------------------------------------------------------------------
' Comment: Reset everything.
'------------------------------------------------------------------------------------------------------------
Public Sub Reset()
    Class_Initialize
End Sub

'========================================================================
End Class 
'========================================================================
%>