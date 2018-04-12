'This is a shared file.  Modifying functions in this file may cause problems for various application installers

	Dim oTextArea
	Dim fso
	Dim currPath
	Dim goodsPath
	Dim rc
	Dim oATO
	Dim WshShell
	Dim LocalMachineName
	Dim sys32Path
	Dim oSysFolder
	Dim oNet
	
Const ForReading = 1 
Const ForWriting = 2

' ================================================
' === CREATE OBJECTS =========================
' ================================================	  	
Sub BuildObjs()
	Set fso = CreateObject("Scripting.FileSystemObject")
	Set oATO = CreateObject("wshAPIToolkitObject.ucATO") 
	Set WshShell = CreateObject("Wscript.Shell")
	Set oNet = CreateObject("WScript.NetWork") 
End sub


	
' ================================================
' === SUBROUTINES FOLLOW =========================
' ================================================	

Sub CleanUp()
	Set fso = Nothing
	Set oATO = Nothing
	Set WshShell = Nothing
	Set oNet = Nothing
	SetCursor("")
End Sub

Sub InitObjs()
	Set oTextArea = document.all.myTextArea
	ClearTextArea()
	BuildObjs()
	currPath = fso.GetAbsolutePathName(".")
	goodsPath = currPath & "\goods"
	LocalMachineName = oNet.ComputerName
	Set oSysFolder = fso.GetSpecialFolder(1)
	sys32Path = oSysFolder.Path
End Sub

Sub StartInstall()
	continueInstall = true
	InitFormValueGlobals() 'This function is in the hta file
	if continueInstall then
		'msgbox "Begin Install"
		InitObjs()
		ValidateUser()
	else
		msgbox "After you fix the errors that were alerted, click on ""Install"" again to begin installation."
		CleanUp()
	end if
End Sub

Sub ValidateUser()

	if CheckUserExists(LocalMachineName, ServiceAccountUserName) then
		if CheckUserPassword(LocalMachineName, ServiceAccountUserName, ServiceAccountPassword) then
			DoInstall()
		else
			msgbox "Invalid service account password" 
			document.form1.ServiceAccountPassword.value = ""
			CleanUp()
		End if
	Else
		msgbox "'" & ServiceAccountUserName & "' is not a valid local Windows user account"
		CleanUp()
	End if
End Sub

Sub SetCursor(str)
	str = lcase(str)
	if str = "wait" then
		doc1.style.cursor= str
		'doc1.disabled=true
	Else
		doc1.style.cursor= str
		'doc1.disabled=false
	End if 
End sub

Function OpenCreateUserDialog()
	Dim newUSer
	Dim temp_arr
	sfeatures = ""
	window.showModalDialog "CreateUser.html", window, ""
	CreateWindowsUser = false
End Function

Sub RegisterCOMObject(fileName)	
	WriteMsgToTextArea "+ Registering " & fileName & "..."
	' dll must located in the system path!	
	' Use wsh API Toolkit to register dlls
	on error resume next
	rc = oATO.CallApi(Cstr(fileName),"DllRegisterServer")
	if err.number <> 0 then
		WriteMsgToTextArea "- RegisterCOMObject Error: " & fileName & ": " & err.Source & " " & err.number & " " & err.Description  
	end if
End Sub
	
Sub CopyFolder(fromPath,toPath)
	on error resume next  
	fso.CopyFolder FromPath, toPath, true
	ChangeFileAttributes toPath, 0, 1, true
	if err.number > 0 then
		WriteMsgToTextArea "- Error in CopyFolder: " & fromPath & " to " & toPath & vbcrlf & "- Error Info: " & err.Source & " " & err.number & " " & err.Description
	Else
		WriteMsgToTextArea  "+ Copied folder " & fromPath & " to " & toPath
	End if
End Sub

Sub CopyFile(fromPath, toPath)
	Dim file
	on error resume next
	fso.CopyFile fromPath, toPath, true
	
	if err.number > 0 then
		WriteMsgToTextArea "- Error in CopyFile: " & fromPath & " to " & toPath & vbcrlf & "- Error Info: " & err.Source & " " & err.number & " " & err.Description
	Else
		WriteMsgToTextArea  "+ Copied file " & fromPath & " to " & toPath
	End if
	file = fso.GetFile(toPath)
	' Remove read only flag
	file.Attributes = file.Attributes - 1
End sub

Function ChangeFileAttributes(strPath, lngSetAttr, lngRemoveAttr, blnRecursive)
   
   ' This function takes a directory path, a value specifying file
   ' attributes to be set, a value specifying file attributes to be
   ' removed, and a flag that indicates whether it should be called
   ' recursively. It returns True unless an error occurs.
   
   Dim fso     
   Dim fdrFolder      
   Dim fdrSubFolder   
   Dim filFile
   
   
   Set fso= CreateObject("Scripting.Filesystemobject")
   
   On Error Resume Next
   ' Get folder.
   Set fdrFolder = fso.GetFolder(strPath)
   If Err <> 0 Then
      ' Incorrect path.
	  WriteMsgToTextArea "- Error in ChangeFileAttributes: " & toPath & vbcrlf & "- Error Info: " & err.Source & " " & err.number & " " & err.Description	
      err.clear
      ChangeFileAttributes = False
      Exit function	
   End If
   
   
   ' If caller passed in attribute to set, set for all.
   If lngSetAttr Then
      For Each filFile In fdrFolder.Files
            filFile.Attributes = filFile.Attributes Or lngSetAttr
      Next
   End If
   
   ' If caller passed in attribute to remove, remove for all.
   If lngRemoveAttr Then
      For Each filFile In fdrFolder.Files
            filFile.Attributes = filFile.Attributes - lngRemoveAttr
      Next
   End If
   
   ' If caller has set blnRecursive argument to True, then call
   ' function recursively.
   If blnRecursive Then
      ' Loop through subfolders.
      For Each fdrSubFolder In fdrFolder.SubFolders
         ' Call function with subfolder path.
         ChangeFileAttributes fdrSubFolder.Path, lngSetAttr, lngRemoveAttr, True
      Next
   End If
   ChangeFileAttributes = True
End Function

Sub CreateFolder(thePath)
	on error resume next  
	fso.CreateFolder thePath
	if err.number > 0 then
		if err.number <> 58 then
			WriteMsgToTextArea "- Error in CreateFolder: " & err.Source & " " & err.number & " " & err.Description
		else
			WriteMsgToTextArea "+ Folder " & thePath & " alredy exists."
		end if
	Else
		WriteMsgToTextArea  "+ Created folder " & thePath 
	End if
End Sub
	
Sub WriteMsgToTextArea(str)

	oTextArea.value = oTextArea.value & str & vbcrlf
	for l = 1 to 100000
	next
End sub
	
Sub ClearTextArea()
	oTextArea.InnerHTML = ""
End sub

Sub CreateCOWSApp(thePath, AppName)
	theOrigBool = AlterRootWritePermissions(True)
	CreateAppDir thePath, "True", AppName, "False"
	CreateAppIso AppName
	theFinalBool = AlterRootWritePermissions(theOrigBool)
	WriteMsgToTextArea  "+ Created " & AppName & " IIS application."
End Sub

Function AlterRootWritePermissions(theBool)
    Dim IISObject
    Dim vDir
    Dim vRoot
    Dim strMachineName
    Dim strObjectPath
    Dim strPath
    Dim success
    Dim theOriginalBool
	    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
	    
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
    Dim IISObject
    Dim vDir
    Dim vRoot
    Dim strMachineName
    Dim strObjectPath
    Dim strPath
    Dim success
	    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
	    
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
    Dim strMachineName
    Dim strObjectPath
    Dim strPath
	    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
	    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    Dim vDirObj
    'create the base iis object and connect ot the IIS metabase
    Set vDirObj = GetObject(strPath & "/ROOT/" & DirName)
    vDirObj.AppCreate (False)
    vDirObj.SetInfo
	    
    Set vDirObj = Nothing
End Sub
		
Sub CreateDSN_MSORA(DataSourceName, ServiceName, UserID)
	Dim lResult
	Dim hKeyHandle
	Dim ODBCRoot
	Dim NewDSNRoot
		
	DriverPath = Sys32Path & "\msorcl32.dll"
	Const DriverName = "Microsoft ODBC for Oracle" 
	
	ODBCRoot = "HKLM\SOFTWARE\ODBC\ODBC.INI\"
	NewDSNRoot = ODBCRoot & DataSourceName
		
	' Make a new DSN Key
	WshShell.RegWrite ODBCRoot, DataSourceName
				
	'Set the values of the new DSN key.
	WshShell.RegWrite NewDSNRoot & "\BufferSize", "65535", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\Description", "", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\Driver", DriverPath, "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\DSN", DataSourceName, "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\GuessTheColDef", "0", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\PWD", "", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\Remarks","0", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\SERVER",ServiceName, "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\StdDayOfWeek","1", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\StripTrailingZero","0", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\SynonymColumns", "1", "REG_SZ"
	WshShell.RegWrite NewDSNRoot & "\UID",UserID, "REG_SZ"
		
	'List the new DSN in the ODBC Manager.
	WshShell.RegWrite ODBCRoot & "\ODBC Data Sources\" & DataSourceName , DriverName, "REG_SZ"
		
	WriteMsgToTextArea "+ " & DataSourceName & " DSN created."
End Sub	

Sub REReplaceInFile(PathSpec, theReplacement, theREPattern, fFormat)
    Dim theString
    Dim ColFiles
    Dim iFile
    Dim FolderPath
    Dim inExt
    Dim fnExt
    Dim inLength
    Dim inlastSlash
    Dim indot
    Dim fnLength
    Dim fndot
    Dim Rts
    Dim Wts

    inLength = Len(PathSpec)
    inlastSlash = InStr(1, StrReverse(PathSpec), "\")
    indot = InStr(1, PathSpec, ".")
    FolderPath = Left(PathSpec, inLength - inlastSlash + 1)
    inExt = Mid(PathSpec, indot, inLength - indot + 1)
    If inlastSlash > 0 Then
        inFileName = Mid(PathSpec, (inLength - inlastSlash) + 2, indot - (inLength - inlastSlash) + 2)
    Else
        inFileName = PathSpec
    End If
    
    If InStr(1, inFileName, "*") > 0 Then
        Set ColFiles = GetFilesCollection(FolderPath)
        For Each File In ColFiles
            fnLenght = Len(File.Name)
            fndot = InStr(1, File.Name, ".")
            fnExt = Mid(File.Name, fndot, fnLenght - fndot + 1)
            If fnExt = inExt Then
                'on error resume next
                Set Rts = File.OpenAsTextStream(ForReading, fFormat)
              
if err.number > 0 then
					WriteMsgToTextArea "Error in REReplaceInFile for " & inFileName
					WriteMsgToTextArea  err.Source & " " & err.number & " " & err.Description  
					err.clear
					Exit sub
				End if
                theString = Rts.ReadAll()
msgbox theString
                Rts.Close
                Set Wts = File.OpenAsTextStream(ForWriting, fFormat)
                Wts.Write (REReplace(theString, theReplacement, theREPattern, True))
                Wts.Close
            End If
        Next
    Else
		on error resume next

        Set Rts = fso.OpenTextFile(PathSpec, ForReading, False, fFormat)
        
	if err.number > 0 then
			WriteMsgToTextArea "Error in REReplaceInFile for " & inFileName
			WriteMsgToTextArea  err.Source & " " & err.number & " " & err.Description  
			err.clear
			Exit sub
        End if
        theString = Rts.ReadAll()
        Rts.Close
        
        Set Wts = fso.OpenTextFile(PathSpec, ForWriting, True, fFormat)
        Wts.Write (REReplace(theString, theReplacement, theREPattern, True))
        Wts.Close
        Set Rts = Nothing
        Set Wts = Nothing
    End If
    WriteMsgToTextArea "+ Replacements made in file/s " & inFileName
End Sub

Function REReplace(pString, pReplacement, pREPattern, bIsGlobal)
    Dim oRE
    
    Set oRE = New RegExp
    oRE.Global = bIsGlobal
    oRE.Pattern = pREPattern
    REReplace = oRE.Replace(pString, pReplacement)
End Function

Function GetFilesCollection(strFolderPath)
    Dim oFolder
    
    Set oFolder = fso.GetFolder(strFolderPath)
    Set GetFilesCollection = oFolder.Files
    Set oFolder = Nothing
End Function

Sub CreateUser(strUser,strDomain, strFullname,strPassword,strDesc)
    Dim oComputer
    Dim oUser

    Set oComputer = Getobject("WinNT://" & strDomain)
    Set oUser = oComputer.create("User",strUser)
	oUser.fullname = strFullname
	oUser.Description = strDesc
	call oUser.SetPassword(strPassword)
	oUser.setinfo
	'msgBox strUser & " User account created."
	 
	Set oUser = nothing
	Set oComputer = nothing
End sub

Sub SetUserFlags(strUser,strDomain,strPassexpires, bNochange, bNoexpire, strDisable, strLocked)
    Dim User
    Dim Flags

    Set User = Getobject("WinNT://" & strDomain & "/" & strUser & ",user")
    Flags = User.Get("UserFlags")
	
    User.put "PasswordExpired", strPassexpires
    User.Accountdisabled = strDisable
    if bNochange then 
      User.put "UserFlags", Flags OR &H00040
    End if
    If bNoexpire then
      User.put "Userflags", flags OR &H10000
    end if
    User.IsAccountLocked = strLocked
	user.setinfo
	Set User = nothing
End sub

Sub CreateCOMApplication(AppName, AppID, AppDesc, Identity, Password)
	Dim cat 'As COMAdminCatalog
	Set cat = CreateObject("COMAdmin.COMAdminCatalog.1")
	Dim apps 'As COMAdminCatalogCollection
	Set apps = cat.GetCollection("Applications")
	Dim app 'As COMAdminCatalogObject
	DeleteCOMApplication AppName
	Set app = apps.Add
	app.Value("ID") = AppID
	app.Value("Name") = AppName
	app.Value("Description") = AppDesc
	'Activate as Server App
	app.Value("Activation") = 1  ' COMAdminActivationLocal = 1
	app.Value("Identity") = Identity
	app.Value("Password") = Password
	app.Value("RunForever") = True
	apps.SaveChanges
	WriteMsgToTextArea "+ " & AppName & " COM+ application created and set to run under " & Identity & " acount."
End sub

Sub AddComponentToCOMApplication(AppName, DllPath, DllName)
	Dim cat 'As COMAdminCatalog
	Set cat = CreateObject("COMAdmin.COMAdminCatalog.1")
	cat.InstallComponent AppName, DllPath & "\" & DllName, "", ""
	WriteMsgToTextArea "+ Added " & DllName & " to " & AppName & " COM+ application"
End Sub

Sub DeleteCOMApplication(AppName)
  Dim i	
  Dim cat 'As COMAdminCatalog
  Set cat = CreateObject("COMAdmin.COMAdminCatalog.1")
  Dim apps 'As COMAdminCatalogCollection
  Set apps = cat.GetCollection("Applications")
  apps.Populate
  ' Enumerate through applications looking for AppName.
  Dim app 'As COMAdminCatalogObject
  i = 0
  For Each app In apps
    If app.Name = AppName Then
      apps.Remove i 
      apps.SaveChanges
      Exit Sub
    End If
    i = i + 1
  Next
End Sub

Sub AddUserToGroup(strDomain,strUsername,strGroupname)
      Dim User
      Dim Group

      Set User = GetObject("WinNT://" & strDomain & "/" & strUsername & ",user")
      Set Group = GetObject("WinNT://" & strDomain & "/" & strGroupname & ",group")
      Group.Add(User.ADsPath)
    Group.Setinfo
	'msgbox "User " & strUserName & " added to " & strGroupName & " Group."
    Set User = nothing
    Set Group = nothing
End Sub

Function CheckUserExists (strDomain, strUser)
    Dim User
    on error resume next
    Set User = GetObject("WinNT://" & strDomain & "/" & strUser & ",user")
	if Err.Number = &H800708AD then
		'User not found
		CheckUserExists = false
    Else			
		CheckUserExists =  true	
    End if  
End function



Function CheckUserPassword (strDomain, strUser, strPassword)
    Dim User
    on error resume next
    Set User = GetObject("WinNT://" & strDomain & "/" & strUser & ",user")
	User.ChangePassword strPassword, strPassword
	
	if err.number = &H80070056 then 
		CheckUserPassword =false
	Else
		CheckUserPassword  = true
	End if  
End function

Function AuthenticateUser(strDomain, strUser, strPassword)
	if NOT CheckUserExists(strDomain, strUser) then
		AuthenticateUser = false
		exit function
	Else
		if NOT CheckUserPassword(strDomain, strUser, strPassword) then
			AuthenticateUser = false
			exit function
		Else
			AuthenticateUser = true
		End if
	End if 
End function

Sub AddAppToChemOfficeIni(AppName)
	REReplaceInFile  COWSRoot & "\config\chemoffice.ini", "$1", "(=|,){1}" & AppName & "[,]*", vbUseDefault
	REReplaceInFile  COWSRoot & "\config\chemoffice.ini", "AppName=" & AppName & "," , "AppName=", vbUseDefault
End sub

Sub CheckError
	
	Dim strMessage, errRecord

	If Err = 0 Then Exit Sub
	strMessage = Err.Source & " " & Hex(Err) & ": " & Err.Description

	WScript.Echo strMessage
	WScript.Quit 1
End Sub

