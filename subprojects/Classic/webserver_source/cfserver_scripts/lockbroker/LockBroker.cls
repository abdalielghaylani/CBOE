VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "WWWLock"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit
'Public Name As String
'Private datCreated As Date

Public Property Get Created() As Date
'Created = datCreated
End Property
Public Property Get LockTheFile() As Boolean
Dim starttime As Double

Debug.Print LockTime
If LockTime <> 99999 Then
    starttime = Timer
    If ((starttime - LockTime) > 60) Or _
        ((starttime - LockTime < 0) And starttime > 60) Then
        ' process is stuck -- go ahead anyway
        LockTime = starttime
        Form1.Text1 = LockTime
        LockTheFile = True
    Else
        ' already locked
        LockTheFile = False
    End If
Else
    LockTime = Timer
    Form1.Text1 = LockTime
    LockTheFile = True
End If

End Property
Public Property Get AddWaitCount() As Boolean

If WaitCount < 10 Then
    WaitCount = WaitCount + 1
    AddWaitCount = True
Else
    AddWaitCount = False
End If
Form1.Text3 = WaitCount

End Property
Public Sub UnlockTheFile()
Form1.Text1 = ""
LockTime = 99999
End Sub
Public Sub SubWaitCount()
If WaitCount > 0 Then WaitCount = WaitCount - 1
Form1.Text3 = WaitCount

End Sub
Public Sub WaitASec()
Dim starttime As Double

starttime = Timer
While (Timer - starttime < 1)
    DoEvents
Wend

End Sub
