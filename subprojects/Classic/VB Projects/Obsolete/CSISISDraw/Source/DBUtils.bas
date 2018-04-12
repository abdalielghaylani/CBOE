Attribute VB_Name = "DBUtils"
Option Explicit

' ##MODULE_SUMMARY A set of utilities for managing data within databases.

Const BLOCK_SIZE = 16384#
Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (pDest As Any, pSource As Any, ByVal ByteLen As Long)

Public Const GENERIC_WRITE = &H40000000
Public Const GENERIC_READ = &H80000000
Const FILE_ATTRIBUTE_NORMAL = &H80
Const CREATE_ALWAYS = 2
Const OPEN_ALWAYS = 4
Const INVALID_HANDLE_VALUE = -1

Declare Function ReadFile Lib "kernel32" (ByVal hFile As Long, _
   lpBuffer As Any, ByVal nNumberOfBytesToRead As Long, _
   lpNumberOfBytesRead As Long, ByVal lpOverlapped As Long) As Long

Private Declare Function CloseHandle Lib "kernel32" ( _
  ByVal hObject As Long) As Long

Private Declare Function WriteFile Lib "kernel32" ( _
  ByVal hFile As Long, lpBuffer As Any, _
  ByVal nNumberOfBytesToWrite As Long, _
  lpNumberOfBytesWritten As Long, ByVal lpOverlapped As Long) As Long

Private Declare Function CreateFile Lib "kernel32" _
  Alias "CreateFileA" (ByVal lpFileName As String, _
  ByVal dwDesiredAccess As Long, ByVal dwShareMode As Long, _
  ByVal lpSecurityAttributes As Long, _
  ByVal dwCreationDisposition As Long, _
  ByVal dwFlagsAndAttributes As Long, ByVal hTemplateFile As Long) _
  As Long

Declare Function FlushFileBuffers Lib "kernel32" ( _
  ByVal hFile As Long) As Long

Public Sub ReadByteArray(ByVal Fname As String, anArray() As Byte, ByVal BytesToRead As Long)
    ' ## Set the bytes in an array to the contents of a file.
    ' ##PARAM Fname The path to the file from which to read the array of bytes.
    ' ##PARAM anArray A byte array containing the data to be retrieved in the specified file.
    ' ##PARAM BytesToRead The number of bytes within the file to read.
   Dim fHandle As Long
   Dim fSuccess As Long
   Dim sTest As String
   Dim lBytesRead As Long

   'Get a handle to a file Fname.
   fHandle = CreateFile(Fname, GENERIC_READ, _
                        0, 0, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0)
   'Here you should test to see if you get a file handle or not.
   'CreateFile returns INVALID_HANDLE_VALUE if it fails.
   If fHandle <> INVALID_HANDLE_VALUE Then
      fSuccess = ReadFile(fHandle, anArray(LBound(anArray)), _
                          BytesToRead, lBytesRead, 0)
      'ReadFile returns a non-zero value if it is successful.
      'Now you just close the file.
      fSuccess = CloseHandle(fHandle)
    Else
        Dim fso As New FileSystemObject
        
        If (Not fso.FileExists(Fname)) Then
            Err.Raise vbObjectError + 513, "DBUtils.ReadByteArray", Description:="the file """ & Fname & """ does not exist"
        Else
            Err.Raise vbObjectError + 513, "DBUtils.ReadByteArray", Description:="the file """ & Fname & """ is open by another program and must be closed before you can continue"
        End If
    End If
End Sub

Public Sub WriteByteArray(ByVal Fname As String, anArray() As Byte)
    ' ## Set the contents of a file to the bytes in the specified array.
    ' ##PARAM Fname The path to the file in which to write the array of bytes.
    ' ##PARAM anArray A byte array containing the data to be stored in the specified file.
   Dim fHandle As Long
   Dim fSuccess As Long
   Dim sTest As String
   Dim lBytesWritten As Long
   Dim BytesToWrite As Long
   'Get the length of data to write
   BytesToWrite = (UBound(anArray) - LBound(anArray) + 1) * LenB(anArray(LBound(anArray)))
   
   'Get a handle to a file Fname.
   fHandle = CreateFile(Fname, GENERIC_WRITE Or GENERIC_READ, _
                        0, 0, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0)
   'Here you should test to see if you get a file handle or not.
   'CreateFile returns INVALID_HANDLE_VALUE if it fails.
   If fHandle <> INVALID_HANDLE_VALUE Then
        If (BytesToWrite > 0) Then
         
      fSuccess = WriteFile(fHandle, anArray(LBound(anArray)), _
                           BytesToWrite, lBytesWritten, 0)
        Else
            fSuccess = WriteFile(fHandle, sTest, BytesToWrite, lBytesWritten, 0)
         End If
      'Check to see if you were successful writing the data
      If fSuccess <> 0 Then
         'Flush the file buffers to force writing of the data.
         fSuccess = FlushFileBuffers(fHandle)
         'Close the file.
         fSuccess = CloseHandle(fHandle)
      End If
    Else
        Err.Raise vbObjectError + 513, Description:="An error occurred while opening a file for writing (" & Fname & ")"
   End If
End Sub

Public Sub SetBlobFieldValue(theField As ADODB.Field, theCDX() As Byte)
Attribute SetBlobFieldValue.VB_Description = "Set the value of a blob field by chunking the data out in small blocks."
    ' ## Set the value of a blob field by chunking the data out in small blocks.
    ' ##PARAM theField The field whose data is to be set.
    ' ##PARAM theCDX A byte array containing the data to be stored in the blob field.
    
    Dim FileSize, CharsRead As Long
    Dim theChunk(BLOCK_SIZE - 1) As Byte
    Dim i As Long
    Dim srcOffset As Long
    Dim lastChunk() As Byte
    
    theField.AppendChunk theCDX
    
    'FileSize = UBound(theCDX) - LBound(theCDX) + 1
    'CharsRead = 0
    
    'Do While FileSize > CharsRead
    '    srcOffset = CharsRead + LBound(theCDX)
    '    If FileSize - CharsRead < BLOCK_SIZE Then
    '        ReDim lastChunk(FileSize - CharsRead - 1)
    '
    '        For i = 0 To UBound(lastChunk)
    '            lastChunk(i) = theCDX(i + srcOffset)
    '        Next i
    '        CharsRead = FileSize
    '        theField.AppendChunk lastChunk
    '    Else
    '        CopyMemory theChunk(0), theCDX(srcOffset), BLOCK_SIZE
    '        CharsRead = CharsRead + BLOCK_SIZE
    '        theField.AppendChunk theChunk
    '    End If
    'Loop
End Sub

Public Sub SetBlobFieldString(theField As ADODB.Field, ByVal theString As String)
Attribute SetBlobFieldString.VB_Description = "Set the value of a blob field to the contents of the specified Unicode string."
    ' ## Set the value of a blob field to the contents of the specified Unicode string.
    ' ##PARAM theField The field whose data is to be set.
    ' ##PARAM theString A Unicode string whose data is to be stored in the field.
    Dim b() As Byte
    'Dim i As Long
    ReDim b(0 To LenB(theString) - 1)
    CopyMemory b(0), ByVal StrPtr(theString), LenB(theString)
    'For i = 1 To LenB(theString)
    '    b(i) = AscB(MidB(theString, i))
    'Next i
    
    SetBlobFieldValue theField, b
End Sub

Public Function NonNull(ByVal s As Variant) As String
Attribute NonNull.VB_Description = "this function converts a null object to an empty length string and other objects"
    ' ## Return the specified argument as a string, handling Null as a special case.
    ' ##PARAM s The string argument to be returned.
    ' ##REMARKS If s represents a null object, then return an empty string, otherwise _
    return the specified string.
    
    If IsNull(s) Then
        NonNull = ""
    Else
        NonNull = s
    End If
End Function

Public Function NonNullBoolean(ByVal s As Variant) As Boolean
    ' ## Return the specified argument as a number, handling Null as a special case.
    ' ##PARAM s The string argument to be returned.
    ' ##REMARKS If s represents a null object, then return false, otherwise _
    return the specified number cast to a Long.
    If (IsNull(s)) Then
        NonNullBoolean = False
    ElseIf (Len(CStr(s)) = 0) Then
        NonNullBoolean = False
    Else
        NonNullBoolean = CBool(s)
    End If
End Function

Public Function NonNullNumber(ByVal s As Variant) As Long
    ' ## Return the specified argument as a number, handling Null as a special case.
    ' ##PARAM s The string argument to be returned.
    ' ##REMARKS If s represents a null object, then return 0, otherwise _
    return the specified number cast to a Long.
    If (IsNull(s)) Then
        NonNullNumber = 0
    ElseIf (Len(CStr(s)) = 0) Then
        NonNullNumber = 0
    Else
        NonNullNumber = CLng(s)
    End If
End Function

Public Function NonNullDouble(ByVal s As Variant) As Double
    ' ## Return the specified argument as a double, handling Null as a special case.
    ' ##PARAM s The string argument to be returned.
    ' ##REMARKS If s represents a null object, then return 0, otherwise _
    return the specified number cast to a Long.
    If (IsNull(s)) Then
        NonNullDouble = 0
    Else
        NonNullDouble = CDbl(s)
    End If
End Function

Public Function DoubleEquals(ByVal d1 As Double, ByVal d2 As Double) As Boolean
    ' Return True if the two doubles have equivalent values, given their magnitude.
    If (d1 = d2) Then
        DoubleEquals = True
    ElseIf (d1 = 0) Then
        DoubleEquals = Log(Abs(d2)) <= -22
    Else
        DoubleEquals = Log(Abs(d1 - d2)) <= Log(Abs(d1)) - 22   ' (e ** 22 is approx 3584912846.13159)
    End If
End Function

Public Sub SetForeignKeyField(ByVal theField As ADODB.Field, ByVal Key As Long)
Attribute SetForeignKeyField.VB_Description = "Set the value of the key field to the specified key, or null if the key is 0."
    ' ## Set the value of the key field to the specified key, or null if the key is 0.
    ' ##PARAM theField The field whose data is to be set.
    ' ##PARAM Key The new value for the field.
    If (Key = 0) Then
        theField.Value = Null
    Else
        theField.Value = Key
    End If
End Sub

Public Function ForeignKeyValue(ByVal Key As Long) As String
Attribute ForeignKeyValue.VB_Description = "Use this value for a foreign key embedded in an insert or update statement."
    ' ## Return a string that can be embedded within a SQL statement representing a number that might be NULL.
    ' ##REMARKS Use this value for a foreign key embedded in an insert or update statement.
    If (Key = 0) Then
        ForeignKeyValue = "NULL"
    Else
        ForeignKeyValue = CStr(Key)
    End If
End Function

Public Function XMLEncode(ByVal Src As String) As String
    ' ## Return an xml encoded version of this string.
    ' ##PARAM Src The string to be encoded.
    
    Dim Target As String
    Dim i As Long
    Dim startChar As Long
    Dim lastChar As Long
    Dim midAsc As Long
    
    startChar = 1
    i = 1
    lastChar = Len(Src)
    While (i <= lastChar)
        midAsc = AscW(Mid(Src, i, 1))
        If ((midAsc <> 9 And midAsc <> 10 And midAsc <> 13 And midAsc < 32) Or midAsc > 126) Then
            If (i > startChar) Then
                Target = Target & Mid(Src, startChar, i - startChar) & "&#x" & Hex(midAsc) & ";"
            Else
                Target = Target & "&#x" & Hex(midAsc) & ";"
            End If
            startChar = i + 1
        End If
        i = i + 1
    Wend
    
    If (i > startChar) Then
        Target = Target & Mid(Src, startChar)
    End If
    
    XMLEncode = Target
End Function

Public Sub AppendXML(ByVal N As IXMLDOMElement, ByRef XML As String)
    ' ## Append the XML in the specified string to the specified node.
    Dim tempDoc As New DOMDocument
    Dim nodeRun As IXMLDOMNode
    Dim targetRun As IXMLDOMNode
    Dim newTargetRun As IXMLDOMNode
    Dim nodeAttribute As IXMLDOMAttribute
    Dim targetAttribute As IXMLDOMAttribute
    
    tempDoc.preserveWhiteSpace = False
    tempDoc.loadXML XML
    With N.ownerDocument
        Set nodeRun = tempDoc.FirstChild
        Set targetRun = N
        Do While (Not nodeRun Is Nothing)
            Set newTargetRun = targetRun.appendChild(.createElement(nodeRun.nodeName))
            newTargetRun.Text = nodeRun.Text
            For Each nodeAttribute In nodeRun.Attributes
                Set targetAttribute = newTargetRun.Attributes.setNamedItem(.createAttribute(nodeAttribute.nodeName))
                targetAttribute.nodeValue = nodeAttribute.nodeValue
            Next nodeAttribute
            
            ' Go depth first
            If (Not nodeRun.FirstChild Is Nothing) Then
                Set targetRun = newTargetRun
                Set nodeRun = nodeRun.FirstChild
            ElseIf (Not nodeRun.nextSibling Is Nothing) Then
                Set nodeRun = nodeRun.nextSibling
            Else
                Do While (Not nodeRun.parentNode Is Nothing)
                    If (Not nodeRun.parentNode.nextSibling Is Nothing) Then
                        Exit Do
                    End If
                    Set nodeRun = nodeRun.parentNode
                    Set targetRun = targetRun.parentNode
                Loop
                If (nodeRun.parentNode Is Nothing) Then
                    Set nodeRun = Nothing ' we are done.
                Else
                    Set nodeRun = nodeRun.parentNode.nextSibling
                    Set targetRun = targetRun.parentNode
                End If
            End If
        Loop
    End With
End Sub

Public Function ParseIntermediaQuery(ByVal SourceQuery As String, Optional ByVal IsAdvancedSearch As Boolean = True) As String
    ' ## Return a string that is a legal intermedia text query based on the specified source string.
    ' ##PARAM SourceQuery The user specified query.
    ' ##RETURNS A legal value for an intermedia query.
    Dim s As String
    
    ' Remove whitespace
    s = VBA.Trim(SourceQuery)
    ' Strip off any leading '='
    If (VBA.Left(s, 1) = "=") Then
        s = VBA.Mid(s, 2)
    End If
    ' Remove whitespace
    s = VBA.Trim(s)
    
    If (IsAdvancedSearch) Then
        ParseIntermediaQuery = s
    Else
        ' Escape any "\" or "}" characters with a backslash and mark the text as literal.
        ParseIntermediaQuery = "{" & Replace(Replace(s, "\", "\\"), "}", "\}") & "}"
    End If

End Function
