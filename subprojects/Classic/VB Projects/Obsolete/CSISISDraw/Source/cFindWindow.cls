VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "cFindWIndow"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

Private Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As Long, lpdwProcessId As Long) As Long
Private Declare Function TerminateProcess Lib "kernel32" (ByVal hProcess As Long, ByVal uExitCode As Long) As Long
Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long

Implements IEnumWindowsSink
Implements IEnumChildWindowsSink

Private m_sTitleToFInd As String
Private m_sClassToFind As String
Private m_sChildTitleToFind As String
Private m_lHwndFound As Long
Private m_lChildHwndFound As Long
Private m_lParenthWnd As Long

Public Property Let TitleToFind(ByVal sTitle As String)
    m_sTitleToFInd = sTitle
    m_lHwndFound = 0
End Property

Public Property Get TitleToFind() As String
    TitleToFind = m_sTitleToFInd
End Property

Public Property Let ChildTitleToFind(ByVal sTitle As String)
    m_sChildTitleToFind = sTitle
    m_lChildHwndFound = 0
End Property

Public Property Get ChildTitleToFind() As String
    TitleToFind = m_sChildTitleToFind
End Property

Public Property Let ClassToFind(ByVal sClass As String)
    m_sClassToFind = sClass
    m_lHwndFound = 0
End Property
Public Property Get ClassToFind() As String
    ClassToFind = m_sClassToFind
End Property

Public Property Get hWndFound() As Long
    hWndFound = m_lHwndFound
End Property

Private Function pbTitleFound(ByVal lHwnd As Long) As Boolean
Dim sWinTitle As String
    sWinTitle = WindowTitle(lHwnd)
    If (pbTextMatch(m_sTitleToFInd, sWinTitle)) Then
        pbTitleFound = True
    End If
End Function

Private Function pbChildTitleFound(ByVal lHwnd As Long) As Boolean
Dim sWinTitle As String
    sWinTitle = WindowTitle(lHwnd)
    ' Fix bug CSBR-56406.  Add second test so we can find no windows in this case.
    If (pbTextMatch(m_sChildTitleToFind, sWinTitle) And (Len(sWinTitle) > 0)) Then
        pbChildTitleFound = True
    End If
End Function

Private Function pbTextMatch(ByVal sMatch As String, ByVal sText As String) As Boolean
Dim lLen As Long
    ' sMatch is the text to match, sText is the Window title:
    If Left$(sMatch, 1) = "%" And Right$(sMatch, 1) = "%" Then
        'Debug.Print sText, sMatch
        If (Len(sMatch) <= 2) Then
            pbTextMatch = True
        Else
            If (InStr(sText, mid$(sMatch, 2, Len(sMatch) - 2)) <> 0) Then
                pbTextMatch = True
            End If
        End If
    ElseIf Left$(sMatch, 1) = "%" Then
        If (Len(sMatch) <= 0) Then
            pbTextMatch = True
        Else
            lLen = Len(sMatch) - 1
            If (lLen <= Len(sText)) Then
                If InStr(sText, mid$(sMatch, 2)) <> 0 Then
                    pbTextMatch = True
                End If
            End If
        End If
    ElseIf Right$(sMatch, 1) = "%" Then
        lLen = Len(sMatch) - 1
        If (Len(sMatch) <= 0) Then
            pbTextMatch = True
        Else
            If (lLen <= Len(sText)) Then
                If InStr(sText, Left$(sMatch, lLen)) <> 0 Then
                    pbTextMatch = True
                End If
            End If
        End If
    Else
        If (sMatch = sText) Then
            pbTextMatch = True
        End If
    End If
End Function

Public Function FindWindow() As Boolean
    m_lHwndFound = 0
    EnumerateWindows Me
    FindWindow = (m_lHwndFound <> 0)
End Function

Public Function FindChildWindow() As Boolean
    m_lChildHwndFound = 0
    EnumerateChildWindows Me
    FindChildWindow = (m_lChildHwndFound <> 0)
End Function

Private Function pbClassFound(ByVal lHwnd As Long) As Boolean
Dim sClass As String
    sClass = ClassName(lHwnd)
    If (pbTextMatch(m_sClassToFind, sClass)) Then
        pbClassFound = True
    End If
End Function

' Top-level windows

Private Sub IEnumWindowsSink_EnumWindow(ByVal hWnd As Long, bStop As Boolean)
Dim bFound As Boolean
Dim bTitleCheckFalse As Boolean
Dim threadId As Long
Dim processId As Long
Dim bSuccess As Boolean
Dim hProcess As Long

    If (IsWindowVisible(hWnd)) Then
        If Len(m_sTitleToFInd) > 0 Then
            bFound = pbTitleFound(hWnd)
            If Not (bFound) Then
                bTitleCheckFalse = True
            End If
        End If
        If Len(m_sClassToFind) > 0 And Not (bTitleCheckFalse) Then
            bFound = pbClassFound(hWnd)
        End If
        If Len(m_sChildTitleToFind) > 0 Then
            ' Find process child windows.
            bFound = pbClassFound(hWnd)
            If (bFound) Then
                m_lParenthWnd = hWnd
                bFound = FindChildWindow
                ' Fix bug CSBR-56046.  If there's an empty ISIS/Draw window left, kill it.
                If ((Not bFound) And (m_sChildTitleToFind = "%")) Then
                    threadId = GetWindowThreadProcessId(hWnd, processId)
                    hProcess = OpenProcess(1, True, processId) '  (PROCESS_TERMINATE = 1)
                    bSuccess = TerminateProcess(hProcess, 1)
                End If
            End If
        End If
    End If
    If (bFound) Then
        m_lHwndFound = hWnd
    End If
    bStop = bFound

End Sub

' CHILD windows

Private Property Get IEnumChildWindowsSink_ParenthWnd() As Long
    IEnumChildWindowsSink_ParenthWnd = m_lParenthWnd
End Property

Private Sub IEnumChildWindowsSink_EnumChildWindow(ByVal hWnd As Long, bStop As Boolean)
Dim bFound As Boolean
Dim bTitleCheckFalse As Boolean
    If (IsWindowVisible(hWnd)) Then
        If Len(m_sChildTitleToFind) > 0 Then
            bFound = pbChildTitleFound(hWnd)
            If Not (bFound) Then
                bTitleCheckFalse = True
            End If
        End If
    End If
    If (bFound) Then
        m_lChildHwndFound = hWnd
    End If
    bStop = bFound

End Sub

Private Property Get IEnumWindowsSink_Identifier() As Long
    IEnumWindowsSink_Identifier = Rnd * 1000000000
End Property

Private Property Get IEnumChildWindowsSink_Identifier() As Long
    IEnumChildWindowsSink_Identifier = Rnd * 1000000000
End Property

