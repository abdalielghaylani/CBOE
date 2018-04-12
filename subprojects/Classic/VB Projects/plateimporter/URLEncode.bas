Attribute VB_Name = "mURLEncode"
Option Explicit

Private m_SafeChar(0 To 255) As Boolean
' Set m_SafeChar(i) = True for characters that
' do not need protection.
Private Sub SetSafeChars()
Static done_before As Boolean
Dim I As Long

    If done_before Then Exit Sub
    done_before = True

    For I = 0 To 47
        m_SafeChar(I) = False
    Next I
    For I = 48 To 57
        m_SafeChar(I) = True
    Next I
    For I = 58 To 64
        m_SafeChar(I) = False
    Next I
    For I = 65 To 90
        m_SafeChar(I) = True
    Next I
    For I = 91 To 96
        m_SafeChar(I) = False
    Next I
    For I = 97 To 122
        m_SafeChar(I) = True
    Next I
    For I = 123 To 255
        m_SafeChar(I) = False
    Next I
End Sub
' Return a URL safe encoding of txt.
Public Function URLEncode(ByVal txt As String) As String
Dim I As Long
Dim ch As String
Dim ch_asc As Long
Dim result As String

    SetSafeChars

    result = ""
    For I = 1 To Len(txt)
        ' Translate the next character.
        ch = Mid$(txt, I, 1)
        ch_asc = Asc(ch)
        If ch_asc = vbKeySpace Then
            ' Use a plus.
            result = result & "+"
        ElseIf m_SafeChar(ch_asc) Then
            ' Use the character.
            result = result & ch
        Else
            ' Convert the character to hex.
            result = result & "%" & Right$("0" & Hex$(ch_asc), 2)
        End If
    Next I

    URLEncode = result
End Function


