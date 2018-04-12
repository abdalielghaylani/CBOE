Attribute VB_Name = "Module1"

Option Explicit
'This project sets permissions on the "core" of WebServer only, the application specific
'portions have been commented out.  --BEF 11/30/00

Private Sub Main()
' set standard machine values
Dim Path, thePath As String
Dim LenPath, thePathCO As String
Dim theOrigBool, theFinalBool As Boolean
' The below lines were commented out so that long path names, with spaces
' could be passed into the program on the command line. --BEF 1/19/01
''''''Path = GetCommandLine(1)
'the path now comes in from installer as the path to wwwroot\chemoffice\, with
'the final \ --BEF 12/4/00
'''''thePath = Path(1)

thePath = Command()
'--LJB Added creation of chemoffice directory as VDIR. ALl base properties are inherited from this directory rather then the default web site
'FOR DEBUGGING comment out above line and the following line:
'thePath = "C:\Inetpub\wwwroot\chemoffice\"

LenPath = Len(thePath)
thePathCO = Left(thePath, LenPath - 11)
CreateCODir thePathCO, "ChemOffice"
theOrigBool = AlterRootWritePermissions(True)
CreateSourceDir thePath & "webserver_source\", "CFServerasp", "False"
CreateSourceDir thePath & "webserver_source\", "CFServerasp", "False"

'CreateSourceDir thePath & "webserver_source\", "cfserver_scripts", "True"

SetDefaultDir
SetMimeType
CreateAppDir thePath & "webserver_source\", "True", "CFServerADSI", "True"
CreateAppDir thePath & "webserver_source\", "True", "CFServerAdmin", "True"
CreateAppDir thePath, "True", "sample", "False"

'CreateAppDir thePath, "True", "all_reactions_data", "False"
'CreateAppDir thePath, "True", "derwent", "False"
'CreateAppDir thePath, "True", "chem_reg", "False"
'CreateAppDir thePath, "True", "chemacx", "False"
'CreateAppDir thePath, "True", "chemindex", "False"
'CreateAppDir thePath, "True", "chemrxn", "False"
'CreateAppDir thePath, "True", "chemmsdx", "False"
'CreateAppDir thePath, "True", "chemacxsc", "False"

' remove the cdx app mapping so we can use it.
CreateTempDir thePath, "cfwtemp"

SetAppMaps thePath, "cfwtemp"
'reset permissions to original state
theFinalBool = AlterRootWritePermissions(theOrigBool)

'Create isolated applications from apps previously created.  This is a work-around for problems with ADSI v.1.0

CreateAppIso "CFServerAdmin"
CreateAppIso "sample"
'CreateAppIso "all_reactions_data"
'CreateAppIso "derwent"
'CreateAppIso "chemacx"
'CreateAppIso "chem_reg"
'CreateAppIso "chemrxn"
'CreateAppIso "chemindex"
'CreateAppIso "chemmsdx"
'CreateAppIso "chemmsdxsc"

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
    
    Set MimeMapObj = Nothing
    
End Sub


Sub CreateAppNotIso(thePath, DirName)

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
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.AccessRead = False
    vDir.AccessWrite = False
    vDir.AccessScript = True
    vDir.Path = thePath & DirName
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub

Sub CreateSourceDir(thePath, DirName, ExFlag)

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
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.Path = thePath & DirName
    vDir.AppFriendlyName = DirName
    vDir.AccessRead = True
    vDir.AccessWrite = False
    If ExFlag = True Then
        vDir.AccessExecute = True
    Else
        vDir.AccessScript = True
    End If
    
    
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub

Sub CreateCODir(thePath, DirName)



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
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.Path = thePath & DirName
    vDir.AppFriendlyName = DirName
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub
Sub CreateAppDir(thePath, theRoot, DirName, DirAuth)

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
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.AppCreate (False)
    vDir.SetInfo
    vDir.AppFriendlyName = DirName
    vDir.AspScriptFileCacheSize = 200
    vDir.AspScriptTimeout = 600
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    If theRoot = True Then
        vDir.Path = thePath & DirName
    Else
        vDir.Path = thePath & DirName
    End If
    vDir.EnableDefaultDoc = True
    vDir.DefaultDoc = "Default.asp"
    vDir.AspExceptionCatchEnable = False
    vDir.AppAllowClientDebug = False
    vDir.AppAllowDebugging = False
    If DirAuth = True Then
        vDir.AuthNTLM = False
        vDir.AuthAnonymous = True
        vDir.AnonymousUserName = "Administrator"
        vDir.AuthBasic = False
    Else
        vDir.AuthNTLM = False
    End If
    
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub
'Settings for CFWTemp
Sub CreateTempDir(thePath, DirName)
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
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.Path = thePath & DirName
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    vDir.AppCreate (True)
    vDir.SetInfo
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
         
End Sub
     
Sub SetAppMaps(thePath, DirName)
    Dim j As Integer
    Dim vList, count
    Dim IISObject As Object
    Dim vDir As Object
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath & "/ROOT" & "/" & DirName
    Set vDir = GetObject(strPath)
    On Error Resume Next
    vList = vDir.GetEx("ScriptMaps")
    count = UBound(vList)
    ReDim Preserve vList(count - 1)
    
    For j = 0 To count - 1
        If InStr(vList(j), ".cdx") > 0 Then
            vList(j) = ".cmw,C:\WINNT\System32\inetsrv\asp.dll,1,PUT,DELETE"
        End If
    Next j
    vDir.PutEx 2, "ScriptMaps", vList
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
    
End Sub

Sub CreateAppIso(DirName)

    Dim strMachineName, strObjectPath, strPath As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    Dim vDirObj As Object
    'create the base iis object and connect ot the IIS metabase
     Set vDirObj = GetObject(strPath & "/ROOT/" & DirName)
    vDirObj.AppCreate (False)
    vDirObj.SetInfo
    
    Set vDirObj = Nothing
    
End Sub
Function GetCommandLine(Optional MaxArgs)
    'Declare variables.
    Dim C, CmdLine, CmdLnLen, InArg, i, NumArgs
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

