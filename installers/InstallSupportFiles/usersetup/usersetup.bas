Attribute VB_Name = "Module1"

Option Explicit

Private Sub Main()
Dim Parameters() As String
Dim UserName As String
Dim AppPwd As String
Dim Domain As String
Dim Desc As String
Dim FullName As String


Parameters() = GetCommandLine(5)
UserName = Parameters(1)
AppPwd = Parameters(2)
Domain = Parameters(3)
Desc = Parameters(4)
FullName = Parameters(5)
On Error Resume Next

CreateUser UserName, Domain, FullName, AppPwd, Desc

If Err.Number <> 0 Then
    If Err.Number = -2147022672 Then
     Dim returnMessage As String
        returnMessage = CreateUserNewPassword(UserName, Domain, FullName, AppPwd, Desc)
        If Trim(returnMessage) <> "" Then
            msgBox ("User account already exists. " & returnMessage)
        End If
    Else
        msgBox (Err.Description)
    End If
Else
    SetUserFlags UserName, Domain, 0, False, True, "False", "False"
End If
          
'testuser;testuser2;alchemy;my description;no needfull nmae

End Sub

Function CreateUserNewPassword(strUser, strDomain, strFullname, strPassword, strDesc) As String
    Dim oComputer As IADsComputer
    Dim oUser As IADsUser
    Dim userPasswordOld As String
    Dim userPasswordNew As String
    Set oComputer = GetObject("WinNT://" & strDomain)
    
    Set oUser = GetObject("WinNT://" & strDomain & "/" & strUser & ",user")
    userPasswordOld = strPassword
    userPasswordNew = strPassword
    'if the password hasn't changed then the following line will not through an error.
    'if an error does occur this means the user needs to be warned that the password for the existing user has changed
    Dim user_passed As Boolean
    Dim pwd_message As String
    user_passed = False
    Do Until user_passed = True
        On Error Resume Next
        oUser.ChangePassword userPasswordOld, userPasswordNew
        If Err.Number <> 0 Then
            Dim msgBoxR
            msgBoxR = msgBox("The user you entered already exists, but the password you entered is different. Update user with new password?", vbYesNo, "Account Exists")
        'response is yes
            If msgBoxR = 6 Then
                On Error Resume Next
                Call oUser.SetPassword(userPasswordNew)
                pwd_message = "Password changed. "
                user_passed = True
            End If
               'response is no
            If msgBoxR = 7 Then
                user_passed = True
                pwd_message = "Password was not changed. "
            End If
        Else
            user_passed = True
            pwd_message = ""
        End If
    Loop
   
    'msgBox strUser & " User account created."
    On Error Resume Next
    Dim admin_added As Integer
    Dim admin_message As String
    admin_added = AddUserToGroupExistingUser(strDomain, strUser, "Administrators")
    If admin_added = 1 Then
        admin_message = "User added to Admininstrators Group. "
    Else
        admin_message = ""
    End If
        
    Dim final_user_message As String
    If Trim(pwd_message) <> "" Or Trim(admin_message) <> "" Then
        final_user_message = pwd_message & " " & admin_message
    Else
        final_user_message = ""
    End If
    Set oUser = Nothing
    Set oComputer = Nothing
    CreateUserNewPassword = final_user_message
End Function

Sub CreateUser(strUser, strDomain, strFullname, strPassword, strDesc)
    Dim oComputer As IADsComputer
    Dim oUser As IADsUser

    Set oComputer = GetObject("WinNT://" & strDomain)
    Set oUser = oComputer.Create("User", strUser)
    oUser.FullName = strFullname
    oUser.Description = strDesc
   
    Call oUser.SetPassword(strPassword)
    oUser.SetInfo
    'msgBox strUser & " User account created."
    On Error Resume Next
    AddUserToGroup strDomain, strUser, "Administrators"
    
    oUser.SetInfo
    Set oUser = Nothing
    Set oComputer = Nothing
End Sub

Sub SetUserFlags(strUser, strDomain, strPassexpires, bNochange, bNoexpire, strDisable, strLocked)
    Dim User As IADsUser
   

    Set User = GetObject("WinNT://" & strDomain & "/" & strUser & ",user")
  
    
    User.Put "PasswordExpired", strPassexpires
    User.AccountDisabled = strDisable
    If bNochange Then
      User.Put "UserFlags", User.Get("UserFlags") Or &H40
    End If
    If bNoexpire Then
      User.Put "Userflags", User.Get("UserFlags") Or &H10000
    End If
    User.IsAccountLocked = strLocked
    User.SetInfo
    Set User = Nothing
End Sub


Sub AddUserToGroup(strDomain, strUsername, strGroupname)
    Dim User As IADsUser
    Dim Group As IADsGroup
    On Error Resume Next
    Set User = GetObject("WinNT://" & strDomain & "/" & strUsername & ",user")
    Set Group = GetObject("WinNT://" & strDomain & "/" & strGroupname & ",group")
    Group.Add (User.ADsPath)
    Group.SetInfo
    Set User = Nothing
    Set Group = Nothing
End Sub

Function AddUserToGroupExistingUser(strDomain, strUsername, strGroupname) As Integer
    Dim User As IADsUser
    Dim Group As IADsGroup
    On Error Resume Next
    Set User = GetObject("WinNT://" & strDomain & "/" & strUsername & ",user")
    Set Group = GetObject("WinNT://" & strDomain & "/" & strGroupname & ",group")
    Group.Add (User.ADsPath)
    Group.SetInfo
    Dim adminAdded As Integer
    If Err.Number <> 0 Then
        adminAdded = 0
    Else
        adminAdded = 1
    End If
    Set User = Nothing
    Set Group = Nothing
    AddUserToGroupExistingUser = adminAdded
End Function

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

