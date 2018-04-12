Attribute VB_Name = "Module1"

Option Explicit

Private Sub Main()

Dim Parameter, thePath, AppName As String
Dim theOrigBool, theFinalBool As Boolean

Parameter = GetCommandLine(2)

'LJB added sub for change write permissions on root directory temporarily so that apps can be configured.
'this is a safety measrue for win2K insallation where setting permissions of sub directories has proven
'to be tricky and very dependent on the initial settings of the default dir.

'for debugging use the following an comment out the above.
AppName = Parameter(1)
thePath = Parameter(2)
'AppName = "Sample"
'thePath = "C:\inetpub\wwwroot\chemoffice\"

theOrigBool = AlterRootWritePermissions(True)
CreateAppDir thePath, "True", AppName, "False"
CreateAppIso AppName
theFinalBool = AlterRootWritePermissions(theOrigBool)

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
    vDir.AppFriendlyName = DirName
    vDir.AspScriptFileCacheSize = 200
    vDir.AspScriptTimeout = 1200
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    If theRoot = True Then
        vDir.Path = thePath
    Else
        vDir.Path = thePath
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
    vDir.AppCreate (True)
    
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
'    'Declare variables.
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

'   Commented out, using second section to search for Semi or Tab. --BEF 1/19/01
'        'Test for space or tab.
'        If (C <> " " And C <> vbTab) Then
'            'Neither space nor tab.
'            'Test if already in argument.
'            If Not InArg Then
'             'New argument begins.
'             'Test for too many arguments.
'             If NumArgs = MaxArgs Then Exit For
'                 NumArgs = NumArgs + 1
'                 InArg = True
'             End If
'             'Concatenate character to current argument.
'             ArgArray(NumArgs) = ArgArray(NumArgs) & C
'             Else
'             'Found a space or tab.
'
'             'Set InArg flag to False.
'             InArg = False
'         End If
'    Next i
'    'Resize array just enough to hold arguments.
'    ReDim Preserve ArgArray(NumArgs)
'    'Return Array in Function name.
'    GetCommandLine = ArgArray()


        'Test for semicolon or tab.
        If (C <> ";" And C <> vbTab) Then
            'Neither semicolon nor tab.
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
             'Found a semicolon or tab.

             'Set InArg flag to False.
             InArg = False
         End If
    Next i
    'Resize array just enough to hold arguments.
    ReDim Preserve ArgArray(NumArgs)
    'Return Array in Function name.
    GetCommandLine = ArgArray()

End Function

