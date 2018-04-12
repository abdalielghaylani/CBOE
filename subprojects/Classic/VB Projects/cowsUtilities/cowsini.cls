VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "cowsini"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Attribute VB_Ext_KEY = "SavedWithClassBuilder" ,"Yes"
Attribute VB_Ext_KEY = "Top_Level" ,"Yes"
Option Explicit
'
' This first line is the declaration from win32api.txt
' Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, lpKeyName As Any, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Long, ByVal lpfilename As String) As Long
Private Declare Function GetPrivateProfileStringByKeyName& Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName$, ByVal lpszKey$, ByVal lpszDefault$, ByVal lpszReturnBuffer$, ByVal cchReturnBuffer&, ByVal lpszFile$)
Private Declare Function GetPrivateProfileStringKeys& Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName$, ByVal lpszKey&, ByVal lpszDefault$, ByVal lpszReturnBuffer$, ByVal cchReturnBuffer&, ByVal lpszFile$)
Private Declare Function GetPrivateProfileStringSections& Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName&, ByVal lpszKey&, ByVal lpszDefault$, ByVal lpszReturnBuffer$, ByVal cchReturnBuffer&, ByVal lpszFile$)
' This first line is the declaration from win32api.txt
' Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, lpKeyName As Any, lpString As Any, ByVal lplFileName As String) As Long
Private Declare Function WritePrivateProfileStringByKeyName& Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lplFileName As String)
Private Declare Function WritePrivateProfileStringToDeleteKey& Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As Long, ByVal lplFileName As String)
Private Declare Function WritePrivateProfileStringToDeleteSection& Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As Long, ByVal lpString As Long, ByVal lplFileName As String)


Public Function VBGetPrivateProfileString(section$, key$, File$) As String

    Dim KeyValue$
    Dim characters As Long
    If File$ = "cows.ini" Then
     File$ = GetNTPath() & "cows.ini"
    End If
    KeyValue$ = String$(1000, 0)
    characters = GetPrivateProfileStringByKeyName(section$, key$, "", KeyValue$, 1000, File$)
    If characters > 0 Then
        KeyValue$ = Left$(KeyValue$, characters)
    End If
    If characters = 0 Then
        KeyValue$ = "INIEmpty"
    End If
    VBGetPrivateProfileString = Trim(KeyValue$)
End Function

'
' Add a new key and value into the INI file section
'
Public Function VBWritePrivateProfileString(KeyValue$, section$, key$, File$)
    Dim success%
    
    ' Write the new key
    success% = WritePrivateProfileStringByKeyName(section$, key$, KeyValue$, File$)
    
    VBWritePrivateProfileString = success%

End Function

'
' Delete the selected key
'
Private Function VBDeletePrivateProfileString(section$, key$, File$)
    Dim success%
    ' Delete the key and section
    success% = WritePrivateProfileStringToDeleteKey(section$, key$, 0, File$)
    VBDeletePrivateProfileString = success%

End Function

Public Function GetNTPath()
    Dim EnvString As String
    EnvString = Environ("windir")   ' Get environment
End Function

