Attribute VB_Name = "LanguageUtils"
Option Explicit

' ##MODULE_SUMMARY Utility routines for supporting localized text in forms and user controls.

Private Declare Function GetUserDefaultLangID% Lib "kernel32" ()
Private Declare Function GetUserDefaultLCID Lib "kernel32" () As Long

Private Const LANGID_JAPANESE = &H411
Public Const CHARSET_JAPANESE = 128

Private Const DEFAULT_CHARSET = 1
Private Const SYMBOL_CHARSET = 2
Private Const SHIFTJIS_CHARSET = 128
Private Const HANGEUL_CHARSET = 129
Private Const CHINESEBIG5_CHARSET = 136
Private Const CHINESESIMPLIFIED_CHARSET = 134

Public Function IsLangJapanese() As Boolean
    IsLangJapanese = (GetUserDefaultLangID() = LANGID_JAPANESE)
End Function

Public Sub SetProperFont(obj As StdFont, Optional ByVal FontBold As Boolean = False, Optional ByVal SizeIncrement As Integer = 0)
    On Error GoTo ErrorSetProperFont
    Select Case GetUserDefaultLCID
    Case &H404 ' Traditional Chinese
        obj.Charset = CHINESEBIG5_CHARSET
        obj.Name = ChrW(&H65B0) + ChrW(&H7D30) + ChrW(&H660E) _
         + ChrW(&H9AD4)   'New Ming-Li
        obj.Size = 9 + SizeIncrement
    Case &H411 ' Japan
        obj.Charset = SHIFTJIS_CHARSET
        obj.Name = ChrW(&HFF2D) + ChrW(&HFF33) + ChrW(&H20) + _
         ChrW(&HFF30) + ChrW(&H30B4) + ChrW(&H30B7) + ChrW(&H30C3) + _
         ChrW(&H30AF)
        obj.Size = 9 + SizeIncrement
    Case &H412 'Korea UserLCID
        obj.Charset = HANGEUL_CHARSET
        obj.Name = ChrW(&HAD74) + ChrW(&HB9BC)
        obj.Size = 9 + SizeIncrement
    Case &H804 ' Simplified Chinese
        obj.Charset = CHINESESIMPLIFIED_CHARSET
        obj.Name = ChrW(&H5B8B) + ChrW(&H4F53)
        obj.Size = 9 + SizeIncrement
    Case Else   ' The other countries
        obj.Charset = DEFAULT_CHARSET
        obj.Name = ""   ' Get the default UI font.
        obj.Size = 8 + SizeIncrement
    End Select
    
    obj.Bold = FontBold
    Exit Sub
ErrorSetProperFont:
    Err.Number = Err
End Sub

Public Function ISEngSet() As Boolean
On Error GoTo ErrorGetLCID
    Dim LCID As Long
    ISEngSet = True
    LCID = GetUserDefaultLCID
    Select Case LCID
       Case 1033, 2057, 3081, 4105, 5129, 6153, 7177, 8201, 9225, 10249, 11273
       Case Else
        ISEngSet = False
    End Select
Exit Function
ErrorGetLCID:
    Err.Number = Err
End Function




