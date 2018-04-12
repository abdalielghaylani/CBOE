Attribute VB_Name = "BooleanUtils"
Option Explicit

Public Function XMLString(ByVal b As Boolean) As String
    ' ## Convert the specified string into a boolean that can be used as an attribute value
    ' ##REMARKS This function is not sensitive to internationalization. The reverse function is XMLBoolean.
    If b Then
        XMLString = "true"
    Else
        XMLString = "false"
    End If
End Function

Public Function XMLBoolean(ByVal s As String) As Boolean
    If (LCase(s) = "true" Or LCase(s) = "yes") Then
        XMLBoolean = True
    ElseIf (LCase(s) = "false" Or LCase(s) = "no") Then
        XMLBoolean = False
    ElseIf (Len(s) = 0) Then
        XMLBoolean = False
    ElseIf (IsNumeric(s)) Then
        XMLBoolean = (Val(s) <> 0)
    Else
        Err.Raise 13, "XMLBoolean", "the string """ & s & """ is not a recognized boolean"
    End If
End Function
