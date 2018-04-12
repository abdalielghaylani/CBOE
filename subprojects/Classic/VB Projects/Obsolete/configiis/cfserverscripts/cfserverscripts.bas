Attribute VB_Name = "Module1"

Option Explicit

Private Sub Main()

Dim Path, thePath As String

' Lines below commented out to allow for long path names in the placement of IIS
' 1/19/01 --BEF
'Path = GetCommandLine(1)
'thePath = Path(1)
thePath = Command()


CreateSourceDir thePath, "cfserver_scripts", "True"

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
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName) 'Name of virtual dir
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.Path = thePath 'Physical path to directory which DirName represents
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
        
        'Test for space or tab.
        If (C <> " " And C <> vbTab) Then
            'Neither space nor tab.
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
             'Found a space or tab.
             
             'Set InArg flag to False.
             InArg = False
         End If
    Next i
    'Resize array just enough to hold arguments.
    ReDim Preserve ArgArray(NumArgs)
    'Return Array in Function name.
    GetCommandLine = ArgArray()
End Function



