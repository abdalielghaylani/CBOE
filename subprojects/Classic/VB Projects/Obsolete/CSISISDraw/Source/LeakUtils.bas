Attribute VB_Name = "LeakUtils"
Option Explicit

Private Declare Function GetCurrentProcessId Lib "kernel32.dll" () As Long

Public Sub InitializeLeakChecking()
    ' Ignore errors that are thrown if there are not settings to delete.
    On Error Resume Next
    DeleteSetting "ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID)
    Err.Clear
End Sub

Public Sub RecordInitialize(ByVal ClassName As String)
'    Dim ClassMetrics() As String
'    ClassMetrics = Split(GetSetting("ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID), ClassName, "0|0|0"), "|")
'
'    ' first is the number of open items.
'    ' second is the number of allocated items
'    ' third is the maximum number of allocated items
'    ClassMetrics(0) = CStr(CLng(ClassMetrics(0)) + 1)
'    ClassMetrics(1) = CStr(CLng(ClassMetrics(1)) + 1)
'    If (CLng(ClassMetrics(2)) < CLng(ClassMetrics(0))) Then
'        ClassMetrics(2) = ClassMetrics(0)
'    End If
'
'    On Error Resume Next
'    SaveSetting "ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID), ClassName, Join(ClassMetrics, "|")
'    Err.Clear
End Sub

Public Sub RecordTerminate(ByVal ClassName As String)
'    Dim ClassMetrics() As String
'    ClassMetrics = Split(GetSetting("ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID), ClassName, "0|0|0"), "|")
'
'    ClassMetrics(0) = CStr(CLng(ClassMetrics(0)) - 1)
'    On Error Resume Next
'    SaveSetting "ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID), ClassName, Join(ClassMetrics, "|")
'    Err.Clear
End Sub

Public Sub PrintLeaks(ByVal Caption As String)
'    Dim Leaks() As String
'    Dim MySettings As Variant
'    Dim i As Integer
'    Dim foundOne As Boolean
'
'    MySettings = GetAllSettings("ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID))
'    foundOne = False
'    For i = LBound(MySettings, 1) To UBound(MySettings, 1)
'        If (Left(MySettings(i, 1), 2) <> "0|") Then
'            If (Not foundOne) Then
'                Debug.Print "*** Leaks: " & Caption
'                App.LogEvent "*** Leaks: " & Caption
'                foundOne = True
'            End If
'            Debug.Print MySettings(i, 0), MySettings(i, 1)
'            App.LogEvent MySettings(i, 0) & ": " & MySettings(i, 1), vbLogEventTypeWarning
'        End If
'    Next i
End Sub

Public Sub TerminateLeakChecking()
    ' Ignore errors that are thrown if there are not settings to delete.
    On Error Resume Next
    DeleteSetting "ENLeakCheck", CStr(GetCurrentProcessId()) & ":" & CStr(App.ThreadID)
    Err.Clear
End Sub

