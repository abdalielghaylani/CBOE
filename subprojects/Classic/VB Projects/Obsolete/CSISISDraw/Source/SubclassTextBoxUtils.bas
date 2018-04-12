Attribute VB_Name = "SubclassTextboxUtils"
Option Explicit
'The code is designed to intercept Windows messages being sent to the textbox and to get the IME text content when it lost focus
'IMEs(Input Method Editor) are required for many Asian languages in order to input characters from the keyboard

Private Declare Function ImmGetContext Lib "imm32.dll" (ByVal hwnd As Long) As Long
Private Declare Function ImmNotifyIME Lib "imm32.dll" (ByVal himc As Long, ByVal dwAction As Long, ByVal dwIndex As Long, ByVal dwValue As Long) As Long
Private Const NI_COMPOSITIONSTR = &H15
Private Const CPS_COMPLETE = &H1
Private Const WM_KILLFOCUS = &H8

Declare Function CallWindowProc Lib "user32" Alias "CallWindowProcA" _
           (ByVal lpPrevWndFunc As Long, _
            ByVal hwnd As Long, _
            ByVal Msg As Long, _
            ByVal wParam As Long, _
            ByVal lParam As Long) As Long

Declare Function SetWindowLong Lib "user32" Alias "SetWindowLongA" _
       (ByVal hwnd As Long, _
        ByVal nIndex As Long, _
        ByVal dwNewLong As Long) As Long

Private Const GWL_WNDPROC = -4

' A set of window procedures that are keyed by hwnd. Normally, there will only be one item
' in the list, but, if a modal dialog appears, there may be more than one.
Private lpPrevWndProcs As New Scripting.Dictionary

Public Sub SubclassTextbox(ByVal hwnd As Long)
    Debug.Assert Not lpPrevWndProcs.Exists(hwnd)
    lpPrevWndProcs.Add hwnd, SetWindowLong(hwnd, GWL_WNDPROC, AddressOf WindowProc)
End Sub

Public Sub UnSubclassTextbox(ByVal hwnd As Long)
    Dim lngReturnValue As Long
    
    If (lpPrevWndProcs.Exists(hwnd)) Then
        lngReturnValue = SetWindowLong(hwnd, GWL_WNDPROC, lpPrevWndProcs(hwnd))
        lpPrevWndProcs.Remove hwnd
    End If
End Sub

Function WindowProc(ByVal hw As Long, _
                    ByVal uMsg As Long, _
                    ByVal wParam As Long, _
                    ByVal lParam As Long) As Long
   
    If uMsg = WM_KILLFOCUS Then
         Dim himc As Long
         himc = ImmGetContext(hw) 'retrieves the input context associated with the specified window
         Dim retval As Long
         retval = ImmNotifyIME(himc, NI_COMPOSITIONSTR, CPS_COMPLETE, 0) 'notifies the IME about changes to the status of the input context
     End If
     
     WindowProc = CallWindowProc(lpPrevWndProcs(hw), hw, _
                                       uMsg, wParam, lParam)
    
End Function



