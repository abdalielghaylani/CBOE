Attribute VB_Name = "Module1"

Option Explicit
'This project sets permissions on the "core" of WebServer only, the application specific
'portions have been commented out.  --BEF 11/30/00

Private Sub Main()
' set standard machine values
Dim AppName, Username, password, Path, thePath As String
Dim Parameters() As String
Dim LenPath, thePathCO, theACLPath As String
Dim theOrigBool, theFinalBool As Boolean
' The below lines were commented out so that long path names, with spaces
' could be passed into the program on the command line. --BEF 1/19/01
''''''Path = GetCommandLine(1)
'the path now comes in from installer as the path to wwwroot\chemoffice\, with
'the final \ --BEF 12/4/00
'''''thePath = Path(1)
On Error Resume Next
Parameters() = GetCommandLine(5)
thePath = Trim(Parameters(1))
'App.LogEvent "thePath:" & thePath

AppName = Trim(Parameters(2))

'App.LogEvent "AppName:" & AppName

On Error Resume Next
Username = Trim(Parameters(3))

'App.LogEvent "Username:" & Username
If Err.Number = 9 Or Trim(UCase(Username)) = "NULL" Then
    Username = ""
End If

password = Trim(Parameters(4))
'App.LogEvent "password:" & password

If Err.Number = 9 Or Trim(UCase(password)) = "NULL" Then
    password = ""
End If

theACLPath = Trim(Parameters(5))
'App.LogEvent "ACLPath:" & theACLPath
'--LJB Added creation of chemoffice directory as VDIR. ALl base properties are inherited from this directory rather then the default web site
'FOR DEBUGGING comment out above line and the following line:
'thePath = "C:\Inetpub\wwwroot\chemoffice\"
Dim lastChar
lastChar = Right$(theACLPath, 1)
If Not lastChar = Chr(92) Then
    theACLPath = theACLPath & Chr(92)
End If
theACLPath = Replace(theACLPath, "\\", "\")

Dim lastChar2
lastChar2 = Right$(thePath, 1)
If Not lastChar2 = Chr(92) Then
    thePath = thePath & Chr(92)
End If

theOrigBool = AlterRootWritePermissions(True)
CreateAppDir thePath, "True", AppName, Username, password
CreateAppIso AppName
theFinalBool = AlterRootWritePermissions(theOrigBool)

On Error Resume Next

SetSubRootPermissions theACLPath, thePath, AppName

'reset all files under chemoffice so they are not read only
SyncShell "Attrib.exe -R """ & thePath & AppName & Chr(92) & "*.*""" & " /S /D"

'chemoffice_data dire
Dim CODataDirPath, temppath
temppath = Split(UCase(thePath), "INETPUB", -1)
CODataDirPath = temppath(0) & "chemoffice_data"
setRootPermissions theACLPath, CODataDirPath
SetSubRootPermissions theACLPath, CODataDirPath, AppName
SyncShell "Attrib.exe -R """ & CODataDirPath & Chr(92) & "*.*""" & " /S /D"

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
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    Else
        Call vRoot.Delete("IIsWebVirtualDir", dirName)
        vRoot.SetInfo
        Set vDir = vRoot.Create("IIsWebVirtualDir", dirName)
    End If
    
    vDir.AccessRead = False
    vDir.AccessWrite = False
    vDir.AccessScript = True
    vDir.Path = thePath & dirName
    vDir.AspEnableParentPaths = True
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
    On Error Resume Next
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
    vDir.AspEnableParentPaths = True
    If theRoot = True Then
        vDir.Path = thePath & dirName
    Else
        vDir.Path = thePath & dirName
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
   'App.LogEvent "theCommandLine:" & CmdLine

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


Sub SetSubRootPermissions(theACLPath, thePath, dirName)

Dim execpath, theAppPath, theCommand, thequotedpath As String
'execpath = thePath & "chemoffice\webserver_source\cfserverasp\utilities\"
theAppPath = thePath & "chemoffice\" & dirName
theCommand = "-on """ & theAppPath & """ -ot file -actn clear -clr ""dacl"" -actn rstchldrn -rst ""dacl"" -actn setprot -op ""dacl:io"" -rec cont_obj"
thequotedpath = """" & theACLPath & "SetACL.exe" & """"
SyncShell thequotedpath & " " & theCommand

End Sub


Sub setRootPermissions(theACLPath, thePath)

Dim execpath, chemofficedatapath, theCommand, thequotedpath As String
'execpath = thePath & "chemoffice\webserver_source\cfserverasp\utilities\"
chemofficedatapath = thePath
theCommand = "-on """ & chemofficedatapath & """ -ot file -actn clear -clr ""dacl"" -actn rstchldrn -rst ""dacl"" -actn ace -ace ""n:authenticated users;p:full"" -actn setprot -op ""dacl:p_nc"" -rec cont_obj"
thequotedpath = """" & theACLPath & "SetACL.exe" & """"
SyncShell thequotedpath & " " & theCommand

End Sub
