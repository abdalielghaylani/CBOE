Attribute VB_Name = "IObjSafeUtils"
'------------------------------------------------------------------------
'     Copyright © 1997 Microsoft Corporation. All rights reserved.
'You have a royalty-free right to use, modify, reproduce and distribute
'the Sample Application Files (and/or any modified version) in any way
'you find useful, provided that you agree that Microsoft has no warranty,
'     obligations or liability for any Sample Application Files.
'------------------------------------------------------------------------
Option Explicit

' ##MODULE_SUMMARY Utility routines for standard implementations of the IENObjectSafety interface.

'------------------------------------------------------------
' constant declares...
'------------------------------------------------------------
Private Const IID_IDispatch = "{00020400-0000-0000-C000-000000000046}"           ' IDispatch interface id
Private Const IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}"     ' IPersistStorage interface id
Private Const IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}"      ' IPersistStream interface id
Private Const IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}" ' IPersistPropertyBag interface id

' Option bit definitions for IObjectSafety
Private Const INTERFACESAFE_FOR_UNTRUSTED_CALLER = &H1   ' Caller of interface may be untrusted
Private Const INTERFACESAFE_FOR_UNTRUSTED_DATA = &H2     ' Data passed into interface may be untrusted

Private Const E_NOINTERFACE = &H80004002 ' No such interface supported
Private Const E_FAIL = &H80004005        ' Unspecified error

Private Const MAX_GUIDLEN = 40           ' This must be 40 for NT.

'------------------------------------------------------------
' API declares...
'------------------------------------------------------------
Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (pDest As Any, pSource As Any, ByVal ByteLen As Long)
Private Declare Function StringFromGUID2 Lib "ole32.dll" (rguid As Any, ByVal lpstrClsId As Long, ByVal cbMax As Integer) As Long

'------------------------------------------------------------
' UDT declares...
'------------------------------------------------------------
Private Type uGUID
    Data1 As Long
    Data2 As Integer
    Data3 As Integer
    Data4(7) As Byte
End Type

'------------------------------------------------------------
Private Function GetIIDFromPTR(ByVal riid As Long) As String
'------------------------------------------------------------
    Dim rc          As Long                                         ' function return code
    Dim rClsId      As uGUID                                        ' guid struct
    Dim bIID()      As Byte                                         ' byte array for interface id
'------------------------------------------------------------
    If (riid <> 0) Then                                             ' validate pointer to interface id
        CopyMemory rClsId, ByVal riid, Len(rClsId)                  ' copy interface guid to struct
        
        bIID = String$(MAX_GUIDLEN, 0)                              ' pre-allocate byte array
        rc = StringFromGUID2(rClsId, VarPtr(bIID(0)), MAX_GUIDLEN)  ' get clsid from guid struct
        rc = InStr(1, bIID, vbNullChar) - 1                         ' look for trailing null char.s
        GetIIDFromPTR = Left$(UCase(bIID), rc)                      ' trim extra nulls and convert to upper-case for comparison
    End If
'------------------------------------------------------------
End Function

'------------------------------------------------------------
Public Sub GetDefaultInterfaceSafetyOptions(ByVal riid As Long, pdwSupportedOptions As Long, pdwEnabledOptions As Long)
    Dim IID         As String                           ' interface id string
    
    ' set supported object safety features...
    pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER Or INTERFACESAFE_FOR_UNTRUSTED_DATA
                          
    IID = GetIIDFromPTR(riid)                           ' get interface id string from pointer
    
    Select Case IID                                     ' determine interface requesting settings
    Case IID_IDispatch                                  ' interface IDispatch.
        ' if this control is Safe For Initializing _
          then set the INTERFACESAFE_FOR_UNTRUSTED_DATA flag
        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA    ' set enabled feature flag
        Exit Sub                                        ' exit and return success
    Case IID_IPersistStorage, IID_IPersistStream, IID_IPersistPropertyBag
        ' if this control is Safe For Scripting or can be made safe for _
          scripting then set the INTERFACESAFE_FOR_UNTRUSTED_CALLER flag
        pdwEnabledOptions = pdwEnabledOptions Or INTERFACESAFE_FOR_UNTRUSTED_CALLER     ' set enabled feature flag
        Exit Sub                                        ' return success
    Case "{A6EF9860-C720-11D0-9337-00A0C90DCAA9}"
        ' this is sent during debugging.
        Exit Sub
    Case Else                                           ' unknown interface requested.
        Err.Raise E_NOINTERFACE                         ' safety options requested for interface are not supported.
    End Select
        
    Err.Raise E_FAIL                                    ' the safety setting for this interface isn't supported
'------------------------------------------------------------
End Sub
'------------------------------------------------------------

'------------------------------------------------------------
Public Sub SetDefaultInterfaceSafetyOptions(ByVal riid As Long, ByVal dwOptionsSetMask As Long, ByVal dwEnabledOptions As Long)
    Exit Sub    ' return success
End Sub
'------------------------------------------------------------



