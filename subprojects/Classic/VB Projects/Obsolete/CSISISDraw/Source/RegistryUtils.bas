Attribute VB_Name = "RegistryUtils"
' ## Registry Key Utilities - from VBhelper.com

Option Explicit

Private Const HKEY_CLASSES_ROOT = &H80000000
Private Const HKEY_CURRENT_CONFIG = &H80000005
Private Const HKEY_CURRENT_USER = &H80000001
Private Const HKEY_LOCAL_MACHINE = &H80000002
Private Const HKEY_USERS = &H80000003

Private Declare Function RegOpenKeyEx Lib "advapi32.dll" Alias "RegOpenKeyExA" (ByVal hKey As Long, ByVal lpSubKey As String, ByVal ulOptions As Long, ByVal samDesired As Long, phkResult As Long) As Long
Private Declare Function RegQueryValueEx Lib "advapi32.dll" Alias "RegQueryValueExA" (ByVal hKey As Long, ByVal lpValueName As String, ByVal lpReserved As Long, lpType As Long, lpData As Any, lpcbData As Long) As Long
Private Declare Function RegCloseKey Lib "advapi32.dll" (ByVal hKey As Long) As Long

Private Const ERROR_SUCCESS = 0&

Private Const STANDARD_RIGHTS_ALL = &H1F0000
Private Const KEY_QUERY_VALUE = &H1
Private Const KEY_SET_VALUE = &H2
Private Const KEY_CREATE_SUB_KEY = &H4
Private Const KEY_ENUMERATE_SUB_KEYS = &H8
Private Const KEY_NOTIFY = &H10
Private Const KEY_CREATE_LINK = &H20
Private Const SYNCHRONIZE = &H100000
Private Const KEY_ALL_ACCESS = _
    ((STANDARD_RIGHTS_ALL Or _
    KEY_QUERY_VALUE Or _
    KEY_SET_VALUE Or _
    KEY_CREATE_SUB_KEY Or _
    KEY_ENUMERATE_SUB_KEYS Or _
    KEY_NOTIFY Or KEY_CREATE_LINK) And _
    (Not SYNCHRONIZE))

' Return a registry key value.
Function GetRegKeyValue(ByVal root As Long, ByVal key_name As String, ByVal subkey_name As String) As String
Dim hKey As Long
Dim value As String
Dim length As Long
Dim value_type As Long

    ' Open the key.
    If RegOpenKeyEx(root, key_name, _
        0&, KEY_QUERY_VALUE, hKey) <> ERROR_SUCCESS _
    Then
        MsgBox "Error opening key."
        Exit Function
    End If

    ' Get the subkey's size.
    If RegQueryValueEx(hKey, subkey_name, _
        0&, value_type, ByVal 0&, length) _
            <> ERROR_SUCCESS _
    Then
        MsgBox "Error getting subkey length."
    End If

    ' Get the subkey's value.
    value = Space$(length)
    If RegQueryValueEx(hKey, subkey_name, _
        0&, value_type, ByVal value, length) _
            <> ERROR_SUCCESS _
    Then
        MsgBox "Error getting subkey value."
    Else
        ' Remove the trailing null character.
        GetRegKeyValue = Left$(value, length - 1)
    End If

    ' Close the key.
    If RegCloseKey(hKey) <> ERROR_SUCCESS Then
        MsgBox "Error closing key."
    End If
End Function


