Attribute VB_Name = "Module1"

Option Explicit
'This project sets permissions on the "core" of WebServer only, the application specific
'portions have been commented out.  --BEF 11/30/00

Private Sub Main()
' set standard machine values
Dim theDir As String
Dim Parameters() As String

Parameters() = GetCommandLine(1)
theDir = Trim(Parameters(1))
If UCase(theDir) = "CORE" Then
    RemoveDir "sample"
    RemoveDir "sample_ora"
    RemoveDir "sample_sqlserver"
    RemoveDir "cs_security"
    RemoveDir "chemoffice"
    RemoveDir "cfserverasp"
    RemoveDir "cfserveradmin"
    RemoveDir "cfserveradsi"
    RemoveDir "cfwtemp"
    ResetDefaultDir

Else
    RemoveDir theDir
End If


End Sub
Sub ResetDefaultDir()
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
   
    vRoot.EnableDefaultDoc = True
    vRoot.DefaultDoc = "default.asp"
    vRoot.SetInfo
    Set vRoot = Nothing
    Set IISObject = Nothing

End Sub



Sub RemoveDir(DirName)

    Dim IISObject As Object
  
    Dim vRoot As Object
    Dim strMachineName, strObjectPath, strPath, success As String
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    
    
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
    On Error Resume Next
    vRoot.Delete "IIsWebVirtualDir", DirName
    vRoot.SetInfo
    Set vRoot = Nothing
    Set IISObject = Nothing
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



