Attribute VB_Name = "IsisUtils"
Option Explicit

Public gENWindowName As String

Private Const GW_HWNDNEXT = 2  '\2 = next window handle
Private Const GW_HWNDFIRST = 0  '\0 = topmost window handle

Private Declare Function GetWindowText Lib "user32" Alias _
"GetWindowTextA" (ByVal hWnd As Long, ByVal lpString As String, _
ByVal cch As Long) As Long

Private Declare Function GetWindowTextLength Lib "user32" _
  Alias "GetWindowTextLengthA" (ByVal hWnd As Long) As Long

Private Declare Function GetWindow Lib "user32" _
  (ByVal hWnd As Long, ByVal wCmd As Long) _
  As Long

Private Declare Function GetActiveWindow Lib "user32" _
  () As Long

'\\ FUNCTION: WINDOWEXISTS
'\\ This function checks if a window exists by searching for
'\\ (part of) the window title.
'\\ strCheckWindow = (part of) window-title

' It returns the full window title.

Public Function WindowExists(ByVal strCheckWindow As String, _
    ByVal ExactMatch As Boolean, ByRef TitleBarString As String) As Boolean

    Dim CurrWnd As Long, Length As Long
    Dim WindowName As String
    
    WindowExists = False
    CurrWnd = GetActiveWindow()
    Do While (CurrWnd <> 0)
      Length = GetWindowTextLength(CurrWnd)
      WindowName = Space$(Length + 1)
      Length = GetWindowText(CurrWnd, WindowName, Length + 1)
      WindowName = Left$(WindowName, Len(WindowName) - 1)
      If (Length > 0) Then
        If ExactMatch = True Then
          If (WindowName = strCheckWindow) Then
            WindowExists = True
            Exit Do
          End If
        Else
          If InStr(WindowName, strCheckWindow) Then
            WindowExists = True
            Exit Do
          End If
        End If
      End If
      CurrWnd = GetWindow(CurrWnd, GW_HWNDNEXT)
      DoEvents
    Loop
    If WindowExists Then
        TitleBarString = WindowName
    End If
End Function

