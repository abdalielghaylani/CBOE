Attribute VB_Name = "mWindow"
Option Explicit

Public Declare Function EnumWindows Lib "user32" (ByVal lpEnumFunc As Long, ByVal lparam As Long) As Long
Public Declare Function EnumChildWindows Lib "user32" (ByVal hWnd As Long, ByVal lpEnumFunc As Long, ByVal lparam As Long) As Long
Public Declare Function IsWindowVisible Lib "user32" (ByVal hWnd As Long) As Long
Public Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hWnd As Long, ByVal lpString As String, ByVal cch As Long) As Long
Public Declare Function GetWindowTextLength Lib "user32" Alias "GetWindowTextLengthA" (ByVal hWnd As Long) As Long
Public Declare Function BringWindowToTop Lib "user32" (ByVal hWnd As Long) As Long
Public Declare Function SetForegroundWindow Lib "user32" (ByVal hWnd As Long) As Long
Public Declare Function GetClassName Lib "user32" Alias "GetClassNameA" (ByVal hWnd As Long, ByVal lpClassName As String, ByVal nMaxCount As Long) As Long
Type RECT
    Left As Long
    Top As Long
    Right As Long
    Bottom As Long
End Type
Public Declare Function GetWindowRect Lib "user32" (ByVal hWnd As Long, lpRect As RECT) As Long
Public Const WM_COMMAND = &H111

Private m_cSink As IEnumWindowsSink
Private m_cChildSink As IEnumChildWindowsSink

' TOP-LEVEL windows

Public Function EnumWindowsProc( _
        ByVal hWnd As Long, _
        ByVal lparam As Long _
    ) As Long
Dim bStop As Boolean
    bStop = False
    m_cSink.EnumWindow hWnd, bStop
    If (bStop) Then
        EnumWindowsProc = 0
    Else
        EnumWindowsProc = 1
    End If
End Function

Public Function EnumerateWindows( _
        ByRef cSink As IEnumWindowsSink _
    ) As Boolean
    If Not (m_cSink Is Nothing) Then Exit Function
    Set m_cSink = cSink
    EnumWindows AddressOf EnumWindowsProc, cSink.Identifier
    Set m_cSink = Nothing
End Function

' CHILD Windows

Public Function EnumChildWindowsProc( _
        ByVal hWnd As Long, _
        ByVal lparam As Long _
    ) As Long
Dim bStop As Boolean
    bStop = False
    m_cChildSink.EnumChildWindow hWnd, bStop
    If (bStop) Then
        EnumChildWindowsProc = 0
    Else
        EnumChildWindowsProc = 1
    End If
End Function

Public Function EnumerateChildWindows( _
        ByRef cChildSink As IEnumChildWindowsSink _
    ) As Boolean
    If Not (m_cChildSink Is Nothing) Then Exit Function
    Set m_cChildSink = cChildSink
    EnumChildWindows cChildSink.ParenthWnd, AddressOf EnumChildWindowsProc, cChildSink.Identifier
    Set m_cChildSink = Nothing
End Function

Public Function WindowTitle(ByVal lHwnd As Long) As String
Dim lLen As Long
Dim sBuf As String

    ' Get the Window Title:
    lLen = GetWindowTextLength(lHwnd)
    If (lLen > 0) Then
        sBuf = String$(lLen + 1, 0)
        lLen = GetWindowText(lHwnd, sBuf, lLen + 1)
        WindowTitle = Left$(sBuf, lLen)
    End If
    
End Function

Public Function ClassName(ByVal lHwnd As Long) As String
Dim lLen As Long
Dim sBuf As String
    lLen = 260
    sBuf = String$(lLen, 0)
    lLen = GetClassName(lHwnd, sBuf, lLen)
    If (lLen <> 0) Then
        ClassName = Left$(sBuf, lLen)
    End If
End Function

Public Sub ActivateWindow(ByVal lHwnd As Long)
    SetForegroundWindow lHwnd
End Sub
