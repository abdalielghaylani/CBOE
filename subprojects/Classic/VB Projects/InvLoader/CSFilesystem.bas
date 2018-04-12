Attribute VB_Name = "Filesystem"
Option Explicit

' CS Common routines for temporary directories, text files

Declare Function GetTempPath Lib "kernel32" Alias _
"GetTempPathA" (ByVal nBufferLength As Long, ByVal _
lpBuffer As String) As Long

Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" _
                   (ByVal hWnd As Long, ByVal lpszOp As String, _
                    ByVal lpszFile As String, ByVal lpszParams As String, _
                    ByVal LpszDir As String, ByVal FsShowCmd As Long) _
                    As Long



Private Declare Function GetDesktopWindow Lib "user32" () As Long

Const SW_SHOWNORMAL = 1
Const SE_ERR_FNF = 2&
Const SE_ERR_PNF = 3&
Const SE_ERR_ACCESSDENIED = 5&
Const SE_ERR_OOM = 8&
Const SE_ERR_DLLNOTFOUND = 32&
Const SE_ERR_SHARE = 26&
Const SE_ERR_ASSOCINCOMPLETE = 27&
Const SE_ERR_DDETIMEOUT = 28&
Const SE_ERR_DDEFAIL = 29&
Const SE_ERR_DDEBUSY = 30&
Const SE_ERR_NOASSOC = 31&
Const ERROR_BAD_FORMAT = 11&
Public Const MAX_PATH = 1024


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

Public Sub ReadByteArray(ByVal fName As String, anArray() As Byte, ByVal BytesToRead As Long)
    ' ## Set the bytes in an array to the contents of a file.
    ' ##PARAM Fname The path to the file from which to read the array of bytes.
    ' ##PARAM anArray A byte array containing the data to be retrieved in the specified file.
    ' ##PARAM BytesToRead The number of bytes within the file to read.
   Dim fHandle As Long
   Dim fSuccess As Long
   Dim sTest As String
   Dim lBytesRead As Long

   'Get a handle to a file Fname.
   fHandle = CreateFile(fName, GENERIC_READ, _
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
        Err.Raise vbObjectError + 513, Description:="An error occurred while opening a file for reading (" & fName & ")"
    End If
End Sub

Public Sub WriteByteArray(ByVal fName As String, anArray() As Byte)
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
   fHandle = CreateFile(fName, GENERIC_WRITE Or GENERIC_READ, _
                        0, 0, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0)
   'Here you should test to see if you get a file handle or not.
   'CreateFile returns INVALID_HANDLE_VALUE if it fails.
   If fHandle <> INVALID_HANDLE_VALUE Then
      fSuccess = WriteFile(fHandle, anArray(LBound(anArray)), _
                           BytesToWrite, lBytesWritten, 0)
      'Check to see if you were successful writing the data
      If fSuccess <> 0 Then
         'Flush the file buffers to force writing of the data.
         fSuccess = FlushFileBuffers(fHandle)
         'Close the file.
         fSuccess = CloseHandle(fHandle)
      End If
    Else
        Err.Raise vbObjectError + 513, Description:="An error occurred while opening a file for writing (" & fName & ")"
   End If
End Sub


Public Function StartURL(ByVal URL As String) As Long
    Dim Scr_hDC As Long
    Scr_hDC = GetDesktopWindow()

    Dim iret As Long
    ' open URL into the default internet browser
    iret = ShellExecute(Scr_hDC, vbNullString, _
    URL, vbNullString, _
    "c:\", SW_SHOWNORMAL)

End Function

Public Function StartDoc(DocName As String) As Long
    Dim Scr_hDC As Long
    Scr_hDC = GetDesktopWindow()
    StartDoc = ShellExecute(Scr_hDC, "Open", DocName, _
    "", "C:\", SW_SHOWNORMAL)
End Function

Public Function GetTmpPath()
Dim strFolder As String
Dim lngResult As Long

strFolder = String(MAX_PATH, 0)
lngResult = GetTempPath(MAX_PATH, strFolder)
If lngResult <> 0 Then
  GetTmpPath = Left(strFolder, InStr(strFolder, _
  Chr(0)) - 1)
Else
  GetTmpPath = ""
End If

End Function

' read contents of a text file
Public Function ReadTextFromFile(fileName As String) As String
    ' read contents of file into var
    Dim fileNum As Long
    fileNum = FreeFile
    Open fileName For Input As #fileNum
    ReadTextFromFile = Input(LOF(1), #fileNum)
    Close #fileNum
End Function

' save a text file to disk
Public Function SaveTextToFile(fileName As String, contents As String) As Boolean
    Dim fileNum As Long
    fileNum = FreeFile
    Open fileName For Output As #fileNum
    Print #fileNum, contents
    Close #fileNum
    SaveTextToFile = True
End Function

' save a text file to disk
Public Function AppendTextToFile(fileName As String, contents As String) As Boolean
    Dim fileNum As Long
    fileNum = FreeFile
    Open fileName For Append As #fileNum
    Print #fileNum, contents
    Close #fileNum
    AppendTextToFile = True
End Function


Public Function DeleteFile(fileName As String) As Boolean
On Error Resume Next
    Kill fileName
    DeleteFile = True
End Function


' FILESYSTEMUTILS.BAS
' Various utilities to deal with the file system.

' Assigns a file's properties the current date and time
Public Function FileDateStampString(Optional ByVal DateToStamp As Variant, _
                                    Optional ByVal UseYear As Boolean = True, _
                                    Optional ByVal UseMonth As Boolean = True, _
                                    Optional ByVal UseDay As Boolean = True, _
                                    Optional ByVal UseTime As Boolean = True) As String
    
    Dim sTemp As String
    
    If UseYear Then
        sTemp = sTemp & "YYYY"
    End If
    If UseMonth Then
        sTemp = sTemp & "_" & "MM"
    End If
    If UseDay Then
        sTemp = sTemp & "_" & "DD"
    End If
    If UseTime Then
        sTemp = sTemp & "-hh_mm_ss-AMPM"
    End If
    If IsMissing(DateToStamp) Then
        DateToStamp = Now
    End If
    FileDateStampString = Format(DateToStamp, sTemp)

End Function

' Retrieves an array of strings describing the names of the files in a directory
Public Function GetFilesInDir(ByVal Path As String, ByVal Ext As String) As String()

    Dim ret() As String
    ReDim ret(0)
    Dim sFile As String
    Dim i As Long: i = 0
    Dim sDir As String
    sDir = AddTrailingSlash(Path)
    sFile = dir(sDir & "*." & Ext)
    
    Do While sFile <> ""
        ReDim Preserve ret(i)
        ret(i) = sDir & sFile
        i = i + 1
        sFile = dir
    Loop
    
    GetFilesInDir = ret
    Exit Function

End Function

' Checks to see if a file exists; returns true if it does
Public Function FileExists(ByVal File As String) As Boolean
On Error GoTo ErrorHandler
    If dir(File) <> "" Then
        FileExists = True
    Else
        FileExists = False
    End If
    Exit Function
ErrorHandler:
    FileExists = False
End Function

' Checks to see if a directory exists; returns true if it does
Public Function DirExists(ByVal Path As String) As Boolean

On Error GoTo ErrorHandler

    If dir(Path, vbDirectory) <> "" Then
        DirExists = True
    Else
        DirExists = False
    End If
    
    Exit Function

ErrorHandler:
    DirExists = False

End Function

' Creates a directory
Public Sub MakeDir(ByVal Path As String)

    MkDir Path

End Sub

' Returns the extension of a file
Public Function Ext(ByVal Path As String) As String

    Dim lDot As Long
    lDot = InStrRev(Path, ".")
    
    If lDot > 0 Then
        Ext = Right(Path, Len(Path) - lDot)
    Else
        Ext = Path
    End If

End Function

' Changes the extension of a file
Public Function ChExt(ByVal Path As String, ByVal NewExt As String) As String

    Dim lDot As Long

    lDot = InStrRev(Path, ".")

    If NewExt = "" Then
        ChExt = Path
    ElseIf lDot > 0 Then
        ChExt = Left(Path, lDot - 1) & "." & NewExt
    Else
        ChExt = Path
    End If

End Function

' Returns the directory path of a path, dropping any file name
Public Function Directory(ByVal Path As String) As String

    Dim iSlash As Long

    iSlash = InStrRev(Path, "\")
'    Directory = IIf(iSlash > 0, Left(Path, iSlash - 1), Path)
    If iSlash > 0 Then
        Directory = Left(Path, iSlash - 1)
    Else
        Directory = Path
    End If

End Function

' Returns the file name of a path, dropping the directory and the extension
Public Function FileNameNoExt(ByVal Path As String) As String

    Dim sFn As String
    Dim sRet As String
    Dim lDot As Long

    sFn = fileName(Path)
    lDot = InStrRev(sFn, ".")
    
    If lDot > 0 Then
        sRet = Left(sFn, lDot - 1)
    Else
        sRet = sFn
    End If
    
    FileNameNoExt = sRet

End Function

' Returns the file name of a path, dropping the directory
Public Function fileName(ByVal Path As String) As String

    Dim lSlash As Long

    lSlash = InStrRev(Path, "\")
    'Filename = IIf(lSlash > 0, Right(Path, Len(Path) - lSlash), Path)
    If lSlash > 0 Then
        fileName = Right(Path, Len(Path) - lSlash)
    Else
        fileName = Path
    End If

End Function

' Moves the file
Public Function MoveFile(ByVal OldPath As String, _
                         ByVal NewDir As String, _
                         Optional ByVal EraseOld As Boolean = True) As String
                            
On Error GoTo HandleError
'    If Not FileExists(OldPath) Then
'        MoveFile = "The source file:  " & OldPath & " does not exist."
'        Exit Function
'    End If
'    If GetAttr(OldPath) = vbDirectory Then
'        MoveFile = "The source file:  " & OldPath & " is not a file."
'        Exit Function
'    End If
'    If Not FileExists(AddTrailingSlash(NewDir)) Then
'        MoveFile = "The destination directory:  " & NewDir & " does not exist."
'        Exit Function
'    End If
    Dim sNewPath As String
    sNewPath = AddTrailingSlash(NewDir) & fileName(OldPath)
    FileCopy OldPath, sNewPath
    If EraseOld Then
        ' only erase if no errors occurred while copying to new location
        If Err.Number = 0 Then
            SetAttr OldPath, vbNormal
            Kill OldPath
        End If
    End If
    Exit Function
    
HandleError:
    MoveFile = "An unknown error occurred while copying " & OldPath & " to " & NewDir
    
End Function

' Behaves similarly to the function above; moves all files in a directory one by one, and then
' moves the directory
Public Function MoveDir(ByVal OldDir As String, _
                        ByVal NewDir As String, _
                        Optional ByVal EraseOld As Boolean = True) As String

    MoveDir = ""

On Error GoTo HandleError

    Dim sNewPath As String
    sNewPath = AddTrailingSlash(NewDir) & fileName(OldDir)
    Name OldDir As sNewPath
    Exit Function

HandleError:
    MoveDir = "An unknown error occurred while copying " & OldDir & " to " & NewDir

End Function

' Adds a trailing backslash to the end of a path string
Public Function AddTrailingSlash(ByVal Path As String) As String

    If Right(Path, 1) <> "\" Then
        Path = Path & "\"
    End If

    AddTrailingSlash = Path

End Function

' Removes a trailing backslash from the end of a path string
Public Function RemTrailingSlash(ByVal Path As String) As String

    If Right(Path, 1) = "\" Then
        Path = Left(Path, Len(Path) - 1)
    End If

    RemTrailingSlash = Path

End Function


Public Sub WriteBytes(FilePath As String, destData As Variant)
    Dim fileNum As Long
    Dim newData() As Byte
    Dim i As Long
    
    fileNum = FreeFile
    Open FilePath For Binary As fileNum
    newData = destData
   
    For i = 0 To UBound(newData)
        Put #fileNum, , newData(i)
    Next i
    
    Close fileNum
End Sub


