Attribute VB_Name = "SyncProcessing"
Option Explicit

' Win32 declares and constants for "synchronous" shell processing
Public Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long
Public Declare Function WaitForSingleObject Lib "kernel32" (ByVal hHandle As Long, ByVal dwMilliseconds As Long) As Long
Public Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
Private Const INFINITE = &HFFFF
Private Const SYNCHRONIZE = &H100000

Public Sub SyncShell(exePath As String)
    Dim taskId As Long
    taskId = Shell(exePath, vbHide)
    WaitForTerm taskId
End Sub

Private Sub WaitForTerm(pid&)
    Dim phnd&
    phnd = OpenProcess(SYNCHRONIZE, 0&, pid)
    If phnd <> 0 Then
        Call WaitForSingleObject(phnd, INFINITE)
        Call CloseHandle(phnd)
    End If
End Sub



