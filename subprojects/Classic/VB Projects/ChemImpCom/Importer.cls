VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Importer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

Option Explicit

' Win32 declares and constants for "synchronous" shell processing
Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long
Private Declare Function WaitForSingleObject Lib "kernel32" (ByVal hHandle As Long, ByVal dwMilliseconds As Long) As Long
Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Long) As Long
Private Const INFINITE = &HFFFF
Private Const SYNCHRONIZE = &H100000

Public Sub SyncShell(exePath As String)
    Dim taskId As Long
    taskId = Shell(exePath, 0)
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

Public Sub Import(userName As String, password As String, oraServer As String, fromFilePath, fromFileName As String, toTable As String)
    ChDir fromFilePath
    SyncShell "ChemImp.exe " & userName & "/" & password & "@" & oraServer & " file=" & Chr(34) & fromFilePath & "\" & fromFileName & Chr(34) & " table=" & toTable & " MOLS_ONLY=Y NO_PROMPT=Y APPEND=N LOG=chemimp.out"
End Sub

Public Sub Import2(userName As String, password As String, oraServer As String, fromFilePath, fromFileName As String, toTable As String)
    ChDir fromFilePath
    SyncShell "ChemImp.exe " & userName & "/" & password & "@" & oraServer & " file=" & Chr(34) & fromFilePath & "\" & fromFileName & Chr(34) & " table=" & toTable & " MOLS_ONLY=N NO_PROMPT=Y APPEND=N LOG=chemimp.out"
End Sub
