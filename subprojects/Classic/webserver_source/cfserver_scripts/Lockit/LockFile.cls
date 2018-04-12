VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LockFile"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Dim LockTime As Double
Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)



Sub LockTheFile()
Dim LockSemaphor As New WWWLock
LockSemaphor.LockTheFile
End Sub




Public Sub UnlockTheFile()
Dim LockSemaphor As New WWWLock
    LockSemaphor.UnlockTheFile
End Sub


Public Sub WaitASec()
    Sleep (10)
End Sub


