Attribute VB_Name = "UtilsMisc"
Option Explicit
        
Public Const NULL_AS_LONG = -1

Public Const DT_TO_CONTROLS = 1
Public Const DT_FROM_CONTROLS = 2
Public Const INVALID_ROW_KEY = -1

Public Const LOG_FILE_NAME = "log_InvLoader.txt"
Public LOG_ACTUAL_FILE_NAME As String
Public Const MAPPINGS_FOLDER = "\Mappings\"
' These asp pages were moved from chem_reg/reg
Public Const RegApiFolder = "cheminv/reg"

Public Const LIST_BYTE_LIMIT = 12000

Public Const SW_SHOWNORMAL = 1
Public Const SW_SHOWMINIMIZED = 2
Public Const SW_SHOWMAXIMIZED = 3

Public gFieldMappings As Dictionary
Public glHTTPTimeout As Long
Public gbSavePlateXML As Boolean
Public gbSaveContainerXML As Boolean
Public gbSaveCompoundXML As Boolean
Public glContainerLimit As Long
Public Const DEFAULT_HTTPTIMEOUT = 1200
Public Const DEFAULT_CONTAINER_LIMIT = 300
Public glFilterIndex As Long
Public bLoadMappingsFromXML As Boolean

Public Enum EnumDataDirection
    eDBtoVB = 0
    eVBToDB
End Enum

Public Enum EnumMolServerVersion
    eNoMolServer = 0
    eMolServer7
    eMolServer8
    eMolServer9
    eMolServer10
    eMolServer11
    eMolServer12
    eMolServer14
	eMolServer15
	eMolServer17
	eMolServer18
End Enum

Public Enum PadType
    padLeft
    padCenter
    padRight
End Enum

Public Function IsNullLong(ByVal var As Variant) As Boolean
    IsNullLong = IIf(var = NULL_AS_LONG, True, False)
End Function

Public Function IsNothing(var As Variant) As Boolean
    IsNothing = (StrComp(TypeName(var), "Nothing", vbTextCompare) = 0)
End Function

Public Sub AddIfNew(ByRef oDict As Dictionary, ByVal key As String, ByVal value As Variant)
    '  add to a dictionary only if unique
    If Not oDict.Exists(key) Then
        oDict.Add key, value
    End If
End Sub

Public Function DictSafeGet(ByRef oDict As Dictionary, ByVal vkey As Variant)
    ' hopefully solve key problem
    Dim bNumeric As Boolean, bfound As Boolean
    bNumeric = IsNumeric(vkey)
    If oDict.Exists(vkey) Then
        If IsObject(oDict(vkey)) Then
            Set DictSafeGet = oDict(vkey)
            Exit Function
        Else
            DictSafeGet = oDict(vkey)
            Exit Function
        End If
    ElseIf bNumeric Then
        If oDict.Exists(CLng(vkey)) Then
            If IsObject(oDict(CLng(vkey))) Then
                Set DictSafeGet = oDict(CLng(vkey))
                Exit Function
            Else
                DictSafeGet = oDict(CLng(vkey))
                Exit Function
            End If
        End If
    End If
    If oDict.Exists(CStr(vkey)) Then
        If IsObject(oDict(CStr(vkey))) Then
            Set DictSafeGet = oDict(CStr(vkey))
            Exit Function
        Else
            DictSafeGet = oDict(CStr(vkey))
            Exit Function
        End If
    Else
        If oDict.count > 0 Then
            If IsObject(oDict(oDict.Keys(0))) Then
                Set DictSafeGet = Nothing
                Exit Function
            End If
        End If
    End If
End Function

Public Function DictionaryNullItems(ByRef oDict As Dictionary)
    Dim vkey
    For Each vkey In oDict.Keys
        oDict(vkey) = Null
    Next
End Function

Public Function DictionaryFromRecordset(ByRef aRS As ADODB.Recordset, ByVal KeyField As Variant, ByVal ItemField As Variant) As Dictionary
    ' used to build ColComboLists
    Dim oRet As Dictionary
    Set oRet = New Dictionary
    If RSHasRecords(aRS) Then
        aRS.MoveFirst
        Do Until aRS.EOF
            If Not IsNull(aRS.fields(KeyField).value) Then
                AddIfNew oRet, aRS.fields(KeyField).value, aRS.fields(ItemField).value
            End If
            aRS.MoveNext
        Loop
        aRS.MoveFirst
    End If
    Set DictionaryFromRecordset = oRet
End Function

Public Function DictionaryAppend(ByRef oDict1 As Dictionary, ByRef oDict2 As Dictionary) As Dictionary
    ' concatenate two dictionaries.
    Dim oRet As Dictionary: Set oRet = New Dictionary
    Dim vkey As Variant
    For Each vkey In oDict1
        oRet.Add vkey, oDict1(vkey)
    Next
    ' if keys already exist, they will not be overwritten
    For Each vkey In oDict2
        If Not oRet.Exists(vkey) Then
            oRet.Add vkey, oDict2(vkey)
        End If
    Next
    Set DictionaryAppend = oRet
End Function

Public Function DictionariesMatch(ByRef oDict1 As Dictionary, ByRef oDict2 As Dictionary) As Boolean
    Dim vkey, bRet As Boolean
    bRet = True
    ' check 1 -> 2
    For Each vkey In oDict1
        If oDict2.Exists(vkey) Then
            If oDict2(vkey) <> oDict1(vkey) Then
                bRet = False
                Exit For
            End If
        Else
            bRet = False
            Exit For
        End If
    Next
    ' check 2 -> 1
    If bRet Then
        For Each vkey In oDict2
            If oDict1.Exists(vkey) Then
                If oDict2(vkey) <> oDict1(vkey) Then
                    bRet = False
                    Exit For
                End If
            Else
                bRet = False
                Exit For
            End If
        Next
    End If
    DictionariesMatch = bRet
End Function

Public Function DictionarySubtract(ByRef oDict1 As Dictionary, ByRef oDict2 As Dictionary) As Dictionary
    ' subtract elements of dict 2 from dict 1, return new dict
    Dim oDict As Dictionary: Set oDict = DictionaryDuplicate(oDict1)
    Dim vkey
    If Not oDict2 Is Nothing Then
        For Each vkey In oDict2.Keys
            If oDict.Exists(vkey) Then
                oDict.Remove (vkey)
            End If
        Next
    End If
    Set DictionarySubtract = oDict
End Function

Public Function DictionaryDuplicate(ByRef oDict As Dictionary) As Dictionary
    ' return a new dictionary
    Dim oDictDup As Dictionary: Set oDictDup = New Dictionary
    Dim vkey
    For Each vkey In oDict.Keys
        oDictDup.Add vkey, oDict(vkey)
    Next
    Set DictionaryDuplicate = oDictDup
End Function



''
' Public Function CDGetStructurePicture(ByRef moCDApp As ChemDraw.Application, ByVal cdxData As String, Optional ByVal Format As String = "wmf", Optional ByVal Clean As Boolean = True) As IPictureDisp
'    ' return a picture of the structure.
'    Dim FilePath As String
'    FilePath = GetTmpPath + "\temp." & Format
'    ' moCDApp. = True
'    mCDXControl.Objects.Clear
'    If (Len(cdxData) > 1) Then
'        moCDApp.ActiveDocument.DataObject.GetData
'        mCDXControl.SourceURL = "data:chemical/x-cdx;base64," & cdxData
'        If Clean Then
'            mCDXControl.Objects.Clean
'        End If
'    End If
'    mCDXControl.DataEncoded = False
'    WriteBytes FilePath, mCDXControl.Data(Format)
'    Set GetStructurePicture = LoadPicture(FilePath)
'    Kill FilePath
' End Function

Public Function CnvLong(ByVal value As Variant, ByVal dir As EnumDataDirection) As Variant
    If dir = eDBtoVB Then
        If IsNull(value) Then
            CnvLong = NULL_AS_LONG
        Else
            If value = "" Then
                CnvLong = NULL_AS_LONG
            Else
                CnvLong = value
            End If
        End If
    ElseIf dir = eVBToDB Then
        If value = NULL_AS_LONG Or value = "" Then
            CnvLong = Null
        Else
            CnvLong = value
        End If
    End If
End Function

Public Function CnvNumeric(ByVal value As Variant, ByVal dir As EnumDataDirection) As Variant
    If dir = eDBtoVB Then
        If IsNull(value) Then
            CnvNumeric = 0
        Else
            CnvNumeric = Val(value)
        End If
    ElseIf dir = eVBToDB Then
        If value = "" Then
            CnvNumeric = Null
        Else
            CnvNumeric = Val(value)
        End If
    End If
End Function

Public Function CnvBool(ByVal value As Variant, ByVal dir As EnumDataDirection) As Variant
    ' convert values from varchar2(1) to booleans
    If dir = eDBtoVB Then
        If IsNull(value) Then
            CnvBool = False
        ElseIf CStr(value) = "0" Then
            CnvBool = False
        Else
            CnvBool = True
        End If
    ElseIf dir = eVBToDB Then
        If value = True Then
            CnvBool = "1"
        Else
            CnvBool = "0"
        End If
    End If
End Function

Public Function CnvBoolLong(ByVal value As Variant, ByVal dir As EnumDataDirection) As Variant
    ' convert values for use in checkboxes
    If dir = eDBtoVB Then
        If IsNull(value) Then
            CnvBoolLong = 0
        Else
            CnvBoolLong = value
        End If
    ElseIf dir = eVBToDB Then
        If value = NULL_AS_LONG Then
            CnvBoolLong = Null
        Else
            CnvBoolLong = value
        End If
    End If
End Function

Public Function CnvString(ByVal value As Variant, ByVal dir As EnumDataDirection) As Variant
    If dir = eDBtoVB Then
        If IsNull(value) Then
            CnvString = ""
        Else
            CnvString = value
        End If
    ElseIf dir = eVBToDB Then
        If value = "" Then
            CnvString = Null
        Else
            CnvString = value
        End If
    End If
End Function

Public Function Ceiling(ByVal X As Double, _
            Optional ByVal Factor As Double = 1) As Double
   Dim Temp As Double
     Temp = Fix(X * Factor)
     Ceiling = (Temp + IIf(X = Temp, 0, Sgn(X))) / Factor
End Function


Public Function Max(ByVal aArr As Variant) As Double
    Dim lMax As Double, l As Long
    Dim bFirst As Boolean: bFirst = True
    For l = LBound(aArr) To UBound(aArr)
        If bFirst Then
            lMax = aArr(l)
            bFirst = False
        Else
            If aArr(l) > lMax Then lMax = aArr(l)
        End If
    Next
    Max = lMax
End Function

Public Function Min(ByVal aArr As Variant) As Double
    Dim lMin As Double, l As Long
    Dim bFirst As Boolean: bFirst = True
    For l = LBound(aArr) To UBound(aArr)
        If bFirst Then
            lMin = aArr(l)
            bFirst = False
        Else
            If aArr(l) < lMin Then lMin = aArr(l)
        End If
    Next
    Min = lMin
End Function

Public Function RSHasRecords(ByRef oRS As ADODB.Recordset) As Boolean
    If oRS Is Nothing Then
        RSHasRecords = False
    Else
        If oRS.BOF And oRS.EOF Then
            RSHasRecords = False
        Else
'            If oRS.RecordCount = 0 Then
'                 RSHasRecords = False
'             Else
                RSHasRecords = True
'             End If
        End If
    End If
End Function

Public Sub DictAddOrReplace(ByRef oDict As Dictionary, ByVal key As Variant, ByVal Item As Variant)
    If Not IsNull(key) Then
        If Not oDict.Exists(CStr(key)) Then
            oDict.Add CStr(key), Item
        Else
            oDict(CStr(key)) = Item
        End If
    End If
End Sub

Public Sub RSDbg(ByRef rs As ADODB.Recordset)
    If Not RSHasRecords(rs) Then
        Debug.Print "No records."
    Else
        Dim LPos As Long
        LPos = rs.AbsolutePosition
        Debug.Print rs.RecordCount & " records:"
        Dim oField
        Dim sRet As String: sRet = "   "
        For Each oField In rs.fields
            sRet = sRet & oField.name & Space(Abs(20 - Len(oField.name)))
        Next
        Debug.Print sRet
        Debug.Print "----------------------------------------------------------------------------------------------------------"
        rs.MoveFirst
        Dim l2 As Long, sVal As String
        Do Until rs.EOF
            sRet = ""
            If rs.AbsolutePosition = LPos Then
                sRet = sRet & ">> "
            Else
                sRet = sRet & "   "
            End If
            For l2 = 0 To rs.fields.count - 1
                sVal = IIf(IsNull(rs(l2)), "NULL", rs(l2))
                sRet = sRet & sVal & Space(Abs(20 - Len(sVal)))
            Next
            Debug.Print sRet
            rs.MoveNext
        Loop
        If LPos >= 0 Then
            rs.AbsolutePosition = LPos
        End If
    End If
End Sub

Public Sub DictDbg(ByRef oDict As Dictionary)
    If oDict Is Nothing Then
        Debug.Print "Dictionary is Nothing."
    ElseIf oDict.count = 0 Then
        Debug.Print "Dictionary has no items."
    Else
        Debug.Print oDict.count & " items:"
        Dim vkey
        For Each vkey In oDict
            If IsObject(oDict(vkey)) Then
                Debug.Print vkey & Space(Abs(20 - Len(vkey))) & TypeName(oDict(vkey))
            ElseIf IsArray(oDict(vkey)) Then
                Debug.Print vkey & Space(Abs(20 - Len(vkey))) & "Array: " & UBound(oDict(vkey)) & " item(s)"
            Else
                Debug.Print vkey & Space(Abs(20 - Len(vkey))) & oDict(vkey)
            End If
        Next
    End If
End Sub

Public Function GetDate(Optional ByVal TheDate)
    ' avoids internationalization problems
    Dim dnow
    If IsMissing(TheDate) Then
        dnow = CDate(Now)
    Else
        dnow = TheDate
    End If
    GetDate = DateSerial(Year(dnow), Month(dnow), Day(dnow)) + TimeSerial(Hour(dnow), Minute(dnow), Second(dnow))
End Function

Public Function ConvertCoord(ByVal rhs As String, Optional ByVal OldFormat As Boolean = False) As String
    ' if OldFormat = true, will return "A-2", else returns new-style "A02"
    '' convert coordinate to standard format
    '--SYAN rewrite this to avoid error when rhs is badly formatted
    Dim sClean As String: sClean = ""
    Dim sTemp As String, Prefix As String, Number As String
    Dim iFirstNumPos As Long: iFirstNumPos = 0
    Dim iFirstNonZeroNumPos As Long: iFirstNonZeroNumPos = 0
    Dim bValid As Boolean
    Dim i As Long
    
    'Go through the string to get rid of non-alphabetic-numeric chars
    For i = 1 To Len(rhs)
        sTemp = Mid(rhs, i, 1)
        If IsAlpha(sTemp) = True Or IsNumeric(sTemp) Then
            sClean = sClean & sTemp
        End If
    Next
 
    'Go through the cleaned string to get the index of the first number
    For i = 1 To Len(sClean)
        sTemp = Mid(sClean, i, 1)
        If IsNumeric(sTemp) Then
            If iFirstNumPos = 0 Then
                iFirstNumPos = i
            End If
            If sTemp <> "0" Then
                iFirstNonZeroNumPos = i
                Exit For
            End If
        End If
    Next
    
    If iFirstNumPos > 1 Then 'number(s) found and not the first char of the string
        Prefix = Left(sClean, iFirstNumPos - 1) 'first part of the string split by first number
        Number = Right(sClean, Len(sClean) - iFirstNonZeroNumPos + 1) 'second part of the string split by first number
        If IsNumeric(Number) Then
            If Not OldFormat Then
                ConvertCoord = Prefix & Pad(Number, 2, padLeft, "0")
            Else
                ConvertCoord = Prefix & "-" & Number
            End If
        Else
            ConvertCoord = "INVALID"
        End If
    Else
        ConvertCoord = "INVALID"
    End If
    
End Function



Public Function ConvertCoordOLD(ByVal rhs As String) As String
    
    '--SYAN rewrite this to avoid error when rhs is badly formatted
    Dim sClean As String: sClean = ""
    Dim sTemp As String, Prefix As String, Number As String
    Dim iFirstNumPos As Long: iFirstNumPos = 0
    Dim bValid As Boolean
    Dim i As Long
    
    'Go through the string to get rid of non-alphabetic-numeric chars
    For i = 1 To Len(rhs)
        sTemp = Mid(rhs, i, 1)
        If IsAlpha(sTemp) = True Or IsNumeric(sTemp) Then
            sClean = sClean & sTemp
        End If
    Next

    'Go through the cleaned string to get the index of the first number
    For i = 1 To Len(sClean)
        sTemp = Mid(sClean, i, 1)
        If IsNumeric(sTemp) Then
            iFirstNumPos = i
            Exit For
        End If
    Next
    
    If iFirstNumPos > 1 Then 'number(s) found and not the first char of the string
        Prefix = Left(sClean, iFirstNumPos - 1) 'first part of the string split by first number
        Number = Right(sClean, Len(sClean) - iFirstNumPos + 1) 'second part of the string split by first number
        
        If IsNumeric(Number) Then
            ConvertCoordOLD = Prefix & "-" & Number
        Else
            ConvertCoordOLD = "INVALID"
        End If
    Else
        ConvertCoordOLD = "INVALID"
    End If
    
End Function
Public Function Pad(ByVal argString As String, ByVal TotalLength As Long, _
Optional ByVal direction As PadType = padCenter, _
Optional ByVal PadCharacter As String = " ") As String
'
' Pads a String using another string to the specified length (Memories of XBase ?)
'
 
If argString = "" Then Exit Function
 
If TotalLength <= Len(argString) Then
    Pad = Left$(argString, TotalLength)
    Exit Function
End If
 
PadCharacter = Left$(PadCharacter, 1)
 
If direction = padCenter Then
    PadCharacter = String$((TotalLength - Len(argString)) \ 2, PadCharacter)
Else
    PadCharacter = String$(TotalLength - Len(argString), PadCharacter)
End If
 
Select Case direction
Case padLeft
    Pad = PadCharacter & argString
    
Case padRight
    Pad = argString & PadCharacter
    
Case Else
    Pad = PadCharacter & argString & PadCharacter
    If Len(Pad) < TotalLength Then
        Pad = Space$(TotalLength - Len(Pad)) & Pad
    End If
End Select
End Function

'****************************************************************************************
'*  PURPOSE: Performs an HTTP request
'*  INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data
'*  OUTPUT: The raw http response from the server
'****************************************************************************************
Public Function HTTPRequest(pMethod, pHostName, bIsSSL As Boolean, pTarget, pUserAgent, pData) As String
    Dim oHTTP As MSXML2.XMLHTTP60
    Dim httpResponse, URL, StatusCode As String
    Screen.MousePointer = vbHourglass
    URL = Protocol(bIsSSL) & pHostName & "/" & pTarget
    ' This is the server safe version from MSXML3.
    Set oHTTP = New MSXML2.XMLHTTP60
    oHTTP.open pMethod, URL, False
    oHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
    oHTTP.setRequestHeader "User-Agent", pUserAgent
    oHTTP.send pData
    ' Print out the request status:
    StatusCode = oHTTP.Status

    If StatusCode <> "200" Then
        httpResponse = oHTTP.responseText
        'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
    Else
        httpResponse = oHTTP.responseText
    End If
    
    HTTPRequest = httpResponse
    Set oHTTP = Nothing
    Screen.MousePointer = vbDefault
End Function


'**********************************************************************
'****** LOGGING routines
'**********************************************************************
Public Sub LogAction(ByVal sInput As String, ByVal sPath As String)
        Dim fso As New FileSystemObject
        Dim txtStream As TextStream
        
        If sPath = "" Or IsNull(sPath) Then sPath = "c:\"
        
        Set txtStream = fso.OpenTextFile(sPath & "\" & LOG_ACTUAL_FILE_NAME, ForAppending, True, TristateTrue)
        'txtStream.WriteLine Now & ":" & sInput
        'txtStream.WriteLine " "
        txtStream.WriteLine sInput & " "
        txtStream.Close
End Sub

Public Sub OpenLog(ByVal sPath As String, ByVal hWnd As Long)
    Dim sFile As String
    
    If sPath = "" Or IsNull(sPath) Then sPath = "c:\"
    sFile = sPath & "\" & LOG_ACTUAL_FILE_NAME
    ShellExecute hWnd, "open", sFile, vbNullString, vbNullString, SW_SHOWNORMAL
End Sub

Public Sub GetGlobals()
    glHTTPTimeout = GetSetting(App.EXEName, "Advanced Options", "HTTPTimeout", DEFAULT_HTTPTIMEOUT)
    gbSavePlateXML = CBool(GetSetting(App.EXEName, "Advanced Options", "SavePlateXML", "False"))
    gbSaveCompoundXML = CBool(GetSetting(App.EXEName, "Advanced Options", "SaveCompoundXML", "False"))
    gbSaveContainerXML = CBool(GetSetting(App.EXEName, "Advanced Options", "SaveContainerXML", "False"))
    glFilterIndex = CLng(GetSetting(App.EXEName, "General", "FilterIndex", 0))
End Sub

Public Sub SetGlobals()
    Call SaveSetting(App.EXEName, "Advanced Options", "HTTPTimeout", CStr(glHTTPTimeout))
    Call SaveSetting(App.EXEName, "Advanced Options", "SavePlateXML", CStr(gbSavePlateXML))
    Call SaveSetting(App.EXEName, "Advanced Options", "SaveCompoundXML", CStr(gbSaveCompoundXML))
    Call SaveSetting(App.EXEName, "Advanced Options", "SaveContainerXML", CStr(gbSaveContainerXML))
    Call SaveSetting(App.EXEName, "General", "FilterIndex", CStr(glFilterIndex))
End Sub

Public Sub GetAllFieldMappings()
    Dim fso As New FileSystemObject
    Dim oMappingsFolder As Folder
    Dim oFile As File
    Dim oXMLFile As MSXML2.DOMDocument60
    Dim oParentNode As IXMLDOMNode
    Dim oNameAttrib As MSXML2.IXMLDOMNode
    Dim oFieldMapping As FieldMapping
    Dim sName As String
    
    Set gFieldMappings = New Dictionary
    
    ' Create if not existing already
    If (Not fso.FolderExists(App.Path & MAPPINGS_FOLDER)) Then
        fso.CreateFolder (App.Path & MAPPINGS_FOLDER)
        ' No need to go any further... we won't have any mappings there if we just created it
        Exit Sub
    End If
    
    Set oXMLFile = New MSXML2.DOMDocument60
        
    Set oMappingsFolder = fso.GetFolder(App.Path & MAPPINGS_FOLDER)
    For Each oFile In oMappingsFolder.Files
        Set oFieldMapping = New FieldMapping
        oFieldMapping.eMappingType = eNone
        
        If oXMLFile.load(oFile.Path) Then
            Set oParentNode = oXMLFile.documentElement
            If Not IsNothing(oParentNode) Then
                Set oNameAttrib = oParentNode.Attributes.getNamedItem("Name")
                If Not IsNothing(oNameAttrib) Then
                    sName = oNameAttrib.Text
                End If
                Set oNameAttrib = Nothing
                
                If oParentNode.nodeName = "RegDBMappings" Then
                    oFieldMapping.eMappingType = eRegistration
                ElseIf oParentNode.nodeName = "ChemInvPlateMappings" Then
                    oFieldMapping.eMappingType = eInventoryPlates
                ElseIf oParentNode.nodeName = "ChemInvStructureMappings" Then
                    oFieldMapping.eMappingType = eInventoryStructures
                ElseIf oParentNode.nodeName = "ChemInvContainerMappings" Then
                    oFieldMapping.eMappingType = eInventoryContainers
                End If
            End If
            Set oParentNode = Nothing
        End If
        
        If (oFieldMapping.eMappingType <> eNone) Then
            oFieldMapping.sName = sName
            oFieldMapping.sXMLFilePath = oFile.Path
            gFieldMappings.Add LCase(sName), oFieldMapping
        End If
        
        Set oFieldMapping = Nothing
    Next
    
    Set oXMLFile = Nothing
    Set oMappingsFolder = Nothing
    Set fso = Nothing
End Sub

Public Sub LoadFieldMappings(grdObject As VSFlexGrid, oCFWImporter As DataImporter, eMappingType As MappingType, NameToIndexMapping As Dictionary)
    Dim oMappingXML As MSXML2.DOMDocument60
    Dim oFieldNodes As MSXML2.IXMLDOMNodeList
    Dim oField As MSXML2.IXMLDOMNode
    Dim oParentNode As MSXML2.IXMLDOMNode
    Dim oVersionAttr As MSXML2.IXMLDOMNode
    Dim oFieldAttr As MSXML2.IXMLDOMAttribute
    Dim sMappingsPath As String
    Dim sXMLFilePath As String
    Dim sVersion As String
    Dim lIndex As Long
    Dim sMapping As String
    Dim sName As String
    Dim bUseDefault As Boolean
        
    On Error GoTo Error
    If GetMappingCount(eMappingType) = 0 Then
        MsgBox "There are no mappings for this load option currently available.", vbExclamation
        Exit Sub
    End If
    
    frmLoadMapping.eMappingType = eMappingType
    frmLoadMapping.Initialize
    frmLoadMapping.Show vbModal, frmCFWDBWiz
    If frmLoadMapping.Cancel Then
        Exit Sub
    End If
        
    sMappingsPath = App.Path & MAPPINGS_FOLDER
    sXMLFilePath = sMappingsPath & frmLoadMapping.sMappingName & ".xml"
    
    ' Set this so we don't get validation error messages when setting the mappings
    bLoadMappingsFromXML = True
        
    Set oMappingXML = New MSXML2.DOMDocument60
    If oMappingXML.load(sXMLFilePath) Then
    
        Set oParentNode = oMappingXML.documentElement
        Set oVersionAttr = oParentNode.Attributes.getNamedItem("Version")
        
        If Not IsNothing(oVersionAttr) Then
            sVersion = oVersionAttr.Text
            If sVersion = "1.0" Then
                Set oFieldNodes = oMappingXML.selectNodes("//Field")
                For Each oField In oFieldNodes
                    ' In case the XML is malformed, use this index value to avoid errors
                    lIndex = -1
                    
                    sName = ""
                    Set oFieldAttr = oField.Attributes.getNamedItem("Name")
                    If Not IsNothing(oFieldAttr) Then
                        sName = oFieldAttr.Text
                        If NameToIndexMapping.Exists(sName) Then
                            lIndex = NameToIndexMapping.Item(sName)
                        End If
                    End If
                    
                    sMapping = ""
                    Set oFieldAttr = oField.Attributes.getNamedItem("Mapping")
                    If Not IsNothing(oFieldAttr) Then
                        sMapping = oFieldAttr.Text
                    End If
                    
                    bUseDefault = True
                    Set oFieldAttr = oField.Attributes.getNamedItem("UseDefault")
                    If Not IsNothing(oFieldAttr) Then
                        If oFieldAttr.Text = "False" Then
                            bUseDefault = False
                        End If
                    End If
                    
                    If lIndex <> -1 And lIndex < grdObject.Rows Then
                        If bUseDefault Then
                            grdObject.TextMatrix(lIndex, eMapping) = "1000"
                        Else
                            grdObject.TextMatrix(lIndex, eMapping) = oCFWImporter.GetFieldMapping(sMapping)
                        End If
                        Set oFieldAttr = oField.Attributes.getNamedItem("Value")
                        If Not IsNothing(oFieldAttr) Then
                            grdObject.TextMatrix(lIndex, eValue) = oFieldAttr.Text
                        End If
                    End If
                    Set oFieldAttr = Nothing
                Next
                Set oField = Nothing
                Set oFieldNodes = Nothing
            Else
                MsgBox "The version of this mapping file is not supported." & vbLf & vbLf & sXMLFilePath, vbExclamation
            End If      ' If sVersion = "1.0" Then
        Else
            MsgBox "The specified XML file is malformed." & vbLf & vbLf & sXMLFilePath, vbCritical
        End If
    End If
    bLoadMappingsFromXML = False
    Exit Sub
Error:
    MsgBox "Error in LoadFileMappings" & vbCrLf & Err.Description, vbExclamation
    bLoadMappingsFromXML = False
End Sub

Public Sub SaveFieldMappings(grdObject As VSFlexGrid, oCFWImporter As DataImporter, eMappingType As MappingType)
    Dim fso As FileSystemObject
    Dim sMappingsPath As String
    Dim oMappingXML As MSXML2.DOMDocument60
    Dim oRootNode As MSXML2.IXMLDOMElement
    Dim oFieldNode As MSXML2.IXMLDOMElement
    Dim oFieldMapping As FieldMapping
    Dim i As Long
    Dim sName As String
    Dim sXMLPath As String
    
    Call frmSaveMapping.Initialize
    frmSaveMapping.Show vbModal, frmCFWDBWiz
    If frmSaveMapping.Cancel Then
        Exit Sub
    End If
            
    ' Check to see if the Mappings folder is there, if not, create it
    Set fso = New FileSystemObject
    sMappingsPath = App.Path & MAPPINGS_FOLDER
    
    If Not fso.FolderExists(sMappingsPath) Then
        fso.CreateFolder (sMappingsPath)
    End If
    
    Set oFieldMapping = New FieldMapping
    oFieldMapping.eMappingType = eMappingType
    Set oMappingXML = New MSXML2.DOMDocument60
    
    Select Case (eMappingType)
        Case eInventoryPlates
            Set oRootNode = oMappingXML.createElement("ChemInvPlateMappings")
        Case eInventoryStructures
            Set oRootNode = oMappingXML.createElement("ChemInvStructureMappings")
        Case eInventoryContainers
            Set oRootNode = oMappingXML.createElement("ChemInvContainerMappings")
        Case eRegistration
            Set oRootNode = oMappingXML.createElement("RegDBMappings")
    End Select
    
    oFieldMapping.sName = frmSaveMapping.sMappingName
    oRootNode.setAttribute "Name", frmSaveMapping.sMappingName
    oRootNode.setAttribute "Version", "1.0"
    oRootNode.setAttribute "CreateDate", CStr(Now)
    
    With grdObject
        For i = 1 To .Rows - 1
            sName = .TextMatrix(i, eDisplayName)
            Set oFieldNode = oMappingXML.createElement("Field")
            oFieldNode.setAttribute "Name", sName
            
            If .TextMatrix(i, eMapping) = "1000" Then
                ' This is "Use Default:".  Set value to whatever value they've typed in
                oFieldNode.setAttribute "UseDefault", "True"
                oFieldNode.setAttribute "Mapping", ""
                oFieldNode.setAttribute "Value", CStr(.TextMatrix(i, eValue))
            Else
                ' Get the actual SDFile field name.  Set value to empty string
                oFieldNode.setAttribute "UseDefault", "False"
                oFieldNode.setAttribute "Mapping", oCFWImporter.GetFieldName(CStr(.TextMatrix(i, eMapping)))
                oFieldNode.setAttribute "Value", ""
            End If
            oRootNode.appendChild oFieldNode
            Set oFieldNode = Nothing
        Next
    End With
    
    oMappingXML.appendChild oRootNode
    sXMLPath = sMappingsPath & frmSaveMapping.sMappingName & ".xml"
    oMappingXML.save (sXMLPath)
    oFieldMapping.sXMLFilePath = sXMLPath
    
    If gFieldMappings.Exists(LCase(frmSaveMapping.sMappingName)) Then
        gFieldMappings.Remove (LCase(frmSaveMapping.sMappingName))
    End If
    gFieldMappings.Add LCase(frmSaveMapping.sMappingName), oFieldMapping
    
    Set oFieldMapping = Nothing
    Set oRootNode = Nothing
    Set oMappingXML = Nothing
    Set fso = Nothing
End Sub

Function GetMappingCount(eMappingType As MappingType)
    Dim oItem
    Dim lCount As Long
    
    lCount = 0
    For Each oItem In gFieldMappings.Items
        If oItem.eMappingType = eMappingType Then
            lCount = lCount + 1
        End If
    Next

    GetMappingCount = lCount
End Function

Function GetFieldTypeAsString(eFieldType As FieldTypeEnum)
    Dim strType As String
    Select Case (eFieldType)
        Case eText
            strType = "Text"
        Case eInteger
            strType = "Integer"
        Case eReal
            strType = "Decimal"
        Case eDate
            strType = "Date"
        Case Else
            strType = "Unknown"
    End Select
    GetFieldTypeAsString = strType
End Function

Function GetADOFieldTypeString(DataTypeEnum As Long)
    Dim str As String
    Select Case DataTypeEnum
        Case adBigInt
            str = "integer"
        Case adBinary
            str = "binary"
        Case adBoolean
            str = "boolean"
        Case adBSTR
            str = "text"
        Case adChar
            str = "text"
        Case adCurrency
            str = "decimal"
        Case adDate
            str = "date"
        Case adDBDate
            str = "date"
        Case adDBTime
            str = "time"
        Case adDBTimeStamp
            str = "time"
        Case adDecimal
            str = "decimal"
        Case adDouble
            str = "decimal"
        Case adEmpty
            str = "empty"
        Case adError
            str = "integer"
        Case adFileTime
            str = "integer"
        Case adGUID
            str = "GUID"
        Case adInteger
            str = "integer"
        Case adLongVarBinary
            str = "integer"
        Case adLongVarChar
            str = "text"
        Case adLongVarWChar
            str = "text"
        Case adNumeric
            str = "decimal"
        Case adSingle
            str = "decimal"
        Case adSmallInt
            str = "integer"
        Case adTinyInt
            str = "integer"
        Case adUnsignedBigInt
            str = "integer"
        Case adUnsignedInt
            str = "integer"
        Case adUnsignedSmallInt
            str = "integer"
        Case adUnsignedTinyInt
            str = "integer"
        Case adVarBinary
            str = "binary"
        Case adVarChar
            str = "text"
        Case adVarNumeric
            str = "decimal"
        Case adVarWChar
            str = "text"
        Case adWChar
            str = "text"
        Case Else
            str = "unhandled"
    End Select
    GetADOFieldTypeString = str
End Function

Function RecordSetHasField(rs As ADODB.Recordset, sFieldName As String)
    Dim oField As ADODB.Field
    
    For Each oField In rs.fields
        If LCase(oField.name) = LCase(sFieldName) Then
            RecordSetHasField = True
            Exit Function
        End If
    Next
    RecordSetHasField = False
End Function

' [Tyler] This is testing for now; RTI may request to have a version that can load a file
' output by their barcode scanners and move/reconcile containers accordingly.  ProcessActionBatch2.asp
' has been modified for E10 to support a MOVECONTAINER tag, which will in turn call out to the
' MoveContainer Oracle function.  This tag expects container and location barcodes, as that will be the information
' entered into the scanner file.  MoveContainer, however, expects container and location ID values, which
' are not the same.  ProcessActionBatch2 then does a lookup of the barcodes to the corresponding ID values.

' In this workflow, it is assumed the scanner file will be opened just like the CFW, XLS, or CSV files.
' A separate radio button will be provided to change the process so that once the scanner file has
' been specified, it moves directly to the "Log XML" pane.  No field mappings or database settings
' need to be given; the scanner file itself will provide all of the information (just location and container
' barcodes, after all).
'Public Function CreateMoveContainerXML() As String
'
'    Dim i As Long: i = 0
'    Dim XMLContainer As DOMDocument
'    Dim oContainerNode As IXMLDOMElement
'    Dim objPI As IXMLDOMProcessingInstruction
'    Dim oRootNode As IXMLDOMElement
'    Dim oNode As IXMLDOMElement
'    Dim sRet
'    Dim key
'    Dim oDict As Dictionary
'    Set oDict = New Dictionary
'
'    With oDict
'        .Add "C1000", ""
'        .Add "C1001", ""
'        .Add "C1002", ""
'        .Add "C1003", ""
'        .Add "C1004", ""
'        .Add "C1005", ""
'        .Add "C1006", ""
'        .Add "C1007", ""
'        .Add "C1008", ""
'        .Add "C1010", ""
'        .Add "C1011", ""
'        .Add "C1040", ""
'        .Add "C1487", ""
'        .Add "C1021", ""
'        .Add "C1022", ""
'        .Add "C1020", ""
'        .Add "C1488", ""
'        .Add "C1501", ""
'    End With
'
'
'    Set XMLContainer = New DOMDocument40
'    With XMLContainer
'        Set objPI = XMLContainer.createProcessingInstruction("xml", "version=""1.0""")
'        .appendChild objPI
'        Set oRootNode = .createElement("CONTAINERS")
'        Set .documentElement = oRootNode
'
'        For Each key In oDict
'            Set oContainerNode = XMLContainer.createElement("MOVECONTAINER")
'            oRootNode.appendChild oContainerNode
'
'            With oContainerNode
'                .setAttribute "CONTAINER_BARCODE", key
'                .setAttribute "LOCATION_BARCODE", "L1040" '"L1021" '
'            End With
'        Next
'
'        .save "c:\MoveContainer.xml"
'    End With
'    sRet = UtilsInventory.CreateContainerXML(XMLContainer.xml, False)
'
'End Function


