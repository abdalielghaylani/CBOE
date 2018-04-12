Attribute VB_Name = "cfcode"

Option Explicit
'This project sets permissions on the "core" of WebServer only, the application specific
'portions have been commented out.  --BEF 11/30/00

Private Sub Main()
' set standard machine values
Dim Username, password, path, thePath As String
Dim Parameters() As String
Dim LenPath, thePathCO, theACLPath As String
Dim theOrigBool, theFinalBool As Boolean
' The below lines were commented out so that long path names, with spaces
' could be passed into the program on the command line. --BEF 1/19/01
''''''Path = GetCommandLine(1)
'the path now comes in from installer as the path to wwwroot\chemoffice\, with
'the final \ --BEF 12/4/00
'''''thePath = Path(1)

Parameters() = GetCommandLine(4)
thePath = Trim(Parameters(1))
On Error Resume Next
Username = Trim(Parameters(2))
If Err.Number = 9 Then
    Username = ""
End If

password = Trim(Parameters(3))
If Err.Number = 9 Then
    password = ""
End If
theACLPath = Trim(Parameters(4))

'--LJB Added creation of chemoffice directory as VDIR. ALl base properties are inherited from this directory rather then the default web site
'FOR DEBUGGING comment out above line and the following line:
'thePath = "C:\Inetpub\wwwroot\chemoffice\"

LenPath = Len(thePath)
On Error Resume Next
thePathCO = Left$(thePath, LenPath - 11)
CreateCODir thePathCO, "ChemOffice"
theOrigBool = AlterRootWritePermissions(True)
CreateSourceDir thePath & "webserver_source\", "CFServerasp", "False"

'CreateSourceDir thePath & "webserver_source\", "cfserver_scripts", "True"

SetDefaultDir
SetMimeType
CreateAppDir thePath & "webserver_source\", "True", "CFServerADSI", Username, password
CreateAppDir thePath & "webserver_source\", "True", "CFServerAdmin", Username, password
CreateAppDir thePath, "True", "sample", "", ""
CreateAppDir thePath, "True", "sample_sqlserver", "", ""
CreateAppDir thePath, "True", "sample_ora", "", ""
CreateAppDir thePath, "True", "cs_security", "", ""


' remove the cdx app mapping so we can use it.
CreateTempDir thePath, "cfwtemp"

SetAppMaps thePath, "cfwtemp"
'reset permissions to original state
theFinalBool = AlterRootWritePermissions(theOrigBool)

'Create isolated applications from apps previously created.  This is a work-around for problems with ADSI v.1.0

CreateAppIso "CFServerAdmin"

Dim lastChar
lastChar = Right$(theACLPath, 1)
If Not lastChar = Chr(92) Then
    theACLPath = theACLPath & Chr(92)
End If
Dim FullCOPath
FullCOPath = thePathCO & "ChemOffice\"
SyncShell "Attrib.exe -R """ & FullCOPath & "*.*""" & " /S /D"

setRootPermissions theACLPath, thePathCO

SetSubRootPermissions theACLPath, thePathCO, "cfwtemp"
SetSubRootPermissions theACLPath, thePathCO, "webserver_source"
SetSubRootPermissions theACLPath, thePathCO, "sample"
SetSubRootPermissions theACLPath, thePathCO, "sample_ora"
SetSubRootPermissions theACLPath, thePathCO, "sample_sqlserver"
SetSubRootPermissions theACLPath, thePathCO, "cs_security"

'reset all files under chemoffice so they are not read only
SyncShell "Attrib.exe -R """ & FullCOPath & "*.*""" & " /S /D"
End Sub


Sub SetDefaultDir()
    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath

    
    Set IISObject = GetObject(strPath)
    
    'Get root of Default Web Site
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    Dim temp
    temp = vRoot.DefaultDoc
    vRoot.EnableDefaultDoc = True
    If Not InStr(temp, "ChemOffice.asp") > 0 Then
        vRoot.DefaultDoc = "ChemOffice.asp," & temp
    End If
    vRoot.SetInfo
    Set vRoot = Nothing
    Set IISObject = Nothing

End Sub

Function AlterRootWritePermissions(theBool)
    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    Dim theOriginalBool As Boolean
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    Set IISObject = GetObject(strPath)
    'Get root of Default Web Site
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    theOriginalBool = vRoot.AccessWrite
    vRoot.AccessWrite = theBool
    vRoot.AspEnableParentPaths = True
    vRoot.SetInfo
    Set vRoot = Nothing
    Set IISObject = Nothing
    AlterRootWritePermissions = theOriginalBool
End Function

Sub SetMimeType()

    'Set the mime type cdx for the web site
     
    Dim MimeMapObj, aMimeMap, i, count
    Const ADS_PROPERTY_UPDATE = 2 ' Get the mimemap object
    Set MimeMapObj = GetObject("IIS://LocalHost/MimeMap")
    'Get the mappings from the MimeMap property
    aMimeMap = MimeMapObj.GetEx("MimeMap") ' Display the mappings
    count = UBound(aMimeMap) + 1
    ReDim Preserve aMimeMap(count)
    Set aMimeMap(count) = CreateObject("MimeMap")
    aMimeMap(count).Extension = ".cdx"
    aMimeMap(count).MimeType = "chemical/x-cdx"
    MimeMapObj.PutEx ADS_PROPERTY_UPDATE, "MimeMap", aMimeMap
    MimeMapObj.SetInfo
  
    aMimeMap = MimeMapObj.GetEx("MimeMap") ' Display the mappings
    count = UBound(aMimeMap) + 1
    ReDim Preserve aMimeMap(count)
    Set aMimeMap(count) = CreateObject("MimeMap")
    aMimeMap(count).Extension = ".sdf"
    aMimeMap(count).MimeType = "chemical/x-mdl-sdf"
    MimeMapObj.PutEx ADS_PROPERTY_UPDATE, "MimeMap", aMimeMap
    MimeMapObj.SetInfo
    
    Set MimeMapObj = GetObject("IIS://LocalHost/W3SVC/1/root")
    'Get the mappings from the MimeMap property
    aMimeMap = MimeMapObj.GetEx("MimeMap") ' Display the mappings
    count = UBound(aMimeMap) + 1
    ReDim Preserve aMimeMap(count)
    Set aMimeMap(count) = CreateObject("MimeMap")
    aMimeMap(count).Extension = ".cdx"
    aMimeMap(count).MimeType = "chemical/x-cdx"
    MimeMapObj.PutEx ADS_PROPERTY_UPDATE, "MimeMap", aMimeMap
    MimeMapObj.SetInfo
  
    aMimeMap = MimeMapObj.GetEx("MimeMap") ' Display the mappings
    count = UBound(aMimeMap) + 1
    ReDim Preserve aMimeMap(count)
    Set aMimeMap(count) = CreateObject("MimeMap")
    aMimeMap(count).Extension = ".sdf"
    aMimeMap(count).MimeType = "chemical/x-mdl-sdf"
    MimeMapObj.PutEx ADS_PROPERTY_UPDATE, "MimeMap", aMimeMap
    MimeMapObj.SetInfo
   
    
    Set MimeMapObj = Nothing
    
End Sub


Sub CreateAppNotIso(thePath, dirName)

    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    On Error Resume Next
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", dirName)
    If Not Err.Number = 0 Then
        Call vRoot.Delete("IIsWebVirtualDir", dirName)
        vRoot.SetInfo
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    End If
    vDir.AccessRead = False
    vDir.AccessWrite = False
    vDir.AccessScript = True
    vDir.path = thePath & dirName
    vDir.AspEnableParentPaths = True
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub

Sub CreateSourceDir(thePath, dirName, ExFlag)

    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    
    
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    On Error Resume Next
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", dirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    Else
        Call vRoot.Delete("IIsWebVirtualDir", dirName)
        vRoot.SetInfo
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    End If
    vDir.path = thePath & dirName
    vDir.AppFriendlyName = dirName
    vDir.AccessRead = True
    vDir.AccessWrite = False
    If ExFlag = True Then
        vDir.AccessExecute = True
    Else
        vDir.AccessScript = True
    End If
    vDir.AspEnableParentPaths = True
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub

Sub CreateCODir(thePath, dirName)



    Dim IISObject As IADs
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
   strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    On Error Resume Next
    
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", dirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    Else
        Call vRoot.Delete("IIsWebVirtualDir", dirName)
        vRoot.SetInfo
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    End If
    vDir.path = thePath & dirName
    vDir.AppFriendlyName = dirName
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    vRoot.AspEnableParentPaths = True
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
    


    
    
End Sub
Sub CreateAppDir(thePath, theRoot, dirName, Username, password)

    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    On Error Resume Next
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", dirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    Else
        Call vRoot.Delete("IIsWebVirtualDir", dirName)
        vRoot.SetInfo
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    End If
    vDir.AppCreate (False)
     vDir.AspEnableParentPaths = True
    vDir.SetInfo
    vDir.AppFriendlyName = dirName
    vDir.AspScriptFileCacheSize = 200
    vDir.AspScriptTimeout = 600
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    If theRoot = True Then
        vDir.path = thePath & dirName
    Else
        vDir.path = thePath & dirName
    End If
    vDir.EnableDefaultDoc = True
    vDir.DefaultDoc = "Default.asp"
    vDir.AspExceptionCatchEnable = False
    vDir.AppAllowClientDebug = False
    vDir.AppAllowDebugging = False
    If Len(Username) > 0 Then
        vDir.AuthNTLM = False
        vDir.AuthAnonymous = True
        vDir.AnonymousUserName = Username
        vDir.AnonymousUserPass = password
        vDir.AuthBasic = False
    Else
        vDir.AuthNTLM = False
    End If
    vDir.AspEnableParentPaths = True
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub
'Settings for CFWTemp
Sub CreateTempDir(thePath, dirName)
    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
     strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    On Error Resume Next
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", dirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    Else
        Call vRoot.Delete("IIsWebVirtualDir", dirName)
        vRoot.SetInfo
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    End If
    vDir.path = thePath & dirName
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    vDir.AppCreate (True)
    vDir.AspEnableParentPaths = True
    vDir.SetInfo
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
         
End Sub
     
Sub SetAppMaps(thePath, dirName)
    Dim j As Integer
    Dim vList, count
    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
     strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath & "/ROOT" & "/" & dirName
    Set vDir = GetObject(strPath)
    On Error Resume Next
    vList = vDir.GetEx("ScriptMaps")
    count = UBound(vList)
    ReDim Preserve vList(count - 1)
    
    For j = 0 To count - 1
        If InStr(vList(j), ".cdx") > 0 Then
            vList(j) = ".cmw,C:\WINNT\System32\inetsrv\asp.dll,1,PUT,DELETE"
        End If
    Next
    vDir.PutEx 2, "ScriptMaps", vList
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
    
End Sub

Sub CreateAppIso(dirName)

    Dim strMachineName, strObjectPath, strPath As String
    
     strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    Dim vDirObj
    'create the base iis object and connect ot the IIS metabase
     Set vDirObj = GetObject(strPath & "/ROOT/" & dirName)
    vDirObj.AppCreate (False)
    vDirObj.SetInfo
    
    Set vDirObj = Nothing
    
    
End Sub
Function GetCommandLine(Optional MaxArgs)
     'Declare variables.
    Dim C, CmdLine, CmdLnLen, InArg, i, NumArgs
    Dim ArgArray() As String
    'See if MaxArgs was provided.
    If IsMissing(MaxArgs) Then MaxArgs = 10
    'Make array of the correct size.
    ReDim ArgArray(MaxArgs)
    NumArgs = 0: InArg = False
    'Get command line arguments.
    CmdLine = Command()
    CmdLnLen = Len(CmdLine)
    'Go thru command line one character
    'at a time.
    For i = 1 To CmdLnLen
        C = Mid(CmdLine, i, 1)
        
'        'Test for space or tab.
'       If (C <> " " And C <> vbTab) Then
'           'Neither space nor tab.

       'Test for semi or tab.
        If (C <> ";" And C <> vbTab) Then
            'Neither semi nor tab.

            'Test if already in argument.
            If Not InArg Then
             'New argument begins.
             'Test for too many arguments.
             If NumArgs = MaxArgs Then Exit For
                 NumArgs = NumArgs + 1
                 InArg = True
             End If
             'Concatenate character to current argument.
             ArgArray(NumArgs) = ArgArray(NumArgs) & C
          Else
             'Found a semi or tab.
             
             'Set InArg flag to False.
             InArg = False
         End If
    Next i
    'Resize array just enough to hold arguments.
    ReDim Preserve ArgArray(NumArgs)
    'Return Array in Function name.
    GetCommandLine = ArgArray()

End Function



Sub setRootPermissions(theACLPath, thePath)

Dim execpath, chemofficepath, theCommand, thequotedpath As String
'execpath = thePath & "chemoffice\webserver_source\cfserverasp\utilities\"
chemofficepath = thePath & "chemoffice"
theCommand = "-on " & chemofficepath & " -ot file -actn clear -clr ""dacl"" -actn rstchldrn -rst ""dacl"" -actn ace -ace ""n:authenticated users;p:full"" -actn setprot -op ""dacl:p_nc"" -rec cont_obj"
thequotedpath = """" & theACLPath & "SetACL.exe" & """"
SyncShell thequotedpath & " " & theCommand

End Sub


Sub SetSubRootPermissions(theACLPath, thePath, dirName)

Dim execpath, theAppPath, theCommand, thequotedpath As String
'execpath = thePath & "chemoffice\webserver_source\cfserverasp\utilities\"
theAppPath = thePath & "chemoffice\" & dirName
theCommand = "-on """ & theAppPath & """ -ot file -actn clear -clr ""dacl"" -actn rstchldrn -rst ""dacl"" -actn setprot -op ""dacl:io"" -rec cont_obj"
thequotedpath = """" & theACLPath & "SetACL.exe" & """"
SyncShell thequotedpath & " " & theCommand

End Sub
