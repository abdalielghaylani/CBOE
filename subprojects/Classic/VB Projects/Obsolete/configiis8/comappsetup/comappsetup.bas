Attribute VB_Name = "Module1"

Option Explicit

Private Sub Main()
Dim Parameters() As String
Dim AppName As String
Dim DllName As String
Dim AppDesc As String
Dim AppID As String
Dim DllPath As String
Dim UserName As String
Dim Password As String

'InstCreateComApps = 0

Parameters() = GetCommandLine(7)
AppName = Parameters(1)
DllName = Parameters(2)
AppDesc = Parameters(3)
AppID = Parameters(4)
DllPath = Parameters(5)
UserName = Parameters(6)
On Error Resume Next
Password = Trim(Parameters(7))
If Err.Number = 9 Then
    Password = ""
End If

'csdo8
CreateCOMApplication AppName, AppID, AppDesc, UserName, Password
AddComponentToCOMApplication AppName, DllPath, DllName

'csdo8;csdo8.dll;csdo app;{577BCAB9AD9B45A3806909C6A5F9AA6F};C:\Program Files\ChemOffice2002\Common\DLLs\8.0\csdo8.dll;liverwurst;exit
End Sub



Sub CreateCOMApplication(AppName, AppID, AppDesc, Identity, Password)
    Dim cat As COMAdminCatalog
    Set cat = CreateObject("COMAdmin.COMAdminCatalog.1")
    Dim comappCat As COMAdminCatalogCollection
    Set comappCat = cat.GetCollection("Applications")
    Dim comapp As COMAdminCatalogObject
    DeleteCOMApplication AppName
    Set comapp = comappCat.Add
    On Error Resume Next
  
    comapp.Value("Name") = AppName
    comapp.Value("Description") = AppDesc
    comappCat.SaveChanges
    'Activate as Server App
    comapp.Value("Activation") = 1  ' COMAdminActivationLocal = 1
    comappCat.SaveChanges
    comapp.Value("RunForever") = True
    comapp.Value("ApplicationAccessChecksEnabled") = 0
    comappCat.SaveChanges
    comapp.Value("Identity") = Identity
    comapp.Value("Password") = Password
    comappCat.SaveChanges
    
    comapp.Value("ID") = AppID
    comappCat.SaveChanges
    If Err.Number <> 0 Then
   
        app.LogEvent "- CreateCOMApplication Error: " & AppName & ": " & Err.Source & " " & Err.Number & " " & Err.Description
    End If
End Sub

Sub AddComponentToCOMApplication(AppName, DllPath, DllName)
    Dim cat As COMAdminCatalog
    Set cat = CreateObject("COMAdmin.COMAdminCatalog.1")
    cat.InstallComponent AppName, DllPath & "\" & DllName, "", ""
    If Err.Number <> 0 Then
        app.LogEvent "- AddComponentToCOMApplication Error: " & AppName & ": " & Err.Source & " " & Err.Number & " " & Err.Description
    End If
End Sub

Sub DeleteCOMApplication(AppName)
  Dim i
  Dim cat As COMAdminCatalog
  Set cat = CreateObject("COMAdmin.COMAdminCatalog.1")
  Dim apps As COMAdminCatalogCollection
  Set apps = cat.GetCollection("Applications")
  apps.Populate
  ' Enumerate through applications looking for AppName.
  Dim app As COMAdminCatalogObject
  i = 0
  For Each app In apps
    If UCase(app.Name) = UCase(AppName) Then
      apps.Remove i
      apps.SaveChanges
      Exit Sub
    End If
    i = i + 1
  Next
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

