<%@ LANGUAGE="VBSCRIPT"%>
<%
theAppName = Request.QueryString("appname")
theTask = Request.QueryString("task")
if Trim(theTask) = "" then theTask = "ADD"
referrer = "/cfserveradmin/AdminSource/webeditor.asp"
Select Case UCase(theTask)

	case "ADD"
		thePath = GetCOWSPath & "\"
		CreateAppVirtualDir thePath, True, theAppName, False
		SetVirDirAppIso theAppName
		Response.Redirect referrer
	case "REMOVE"
		RemoveDir theAppName
		Response.Redirect referrer
End Select


Sub SetVirDirAppIso(DirName)
on error resume next
    Dim strMachineName, strObjectPath, strPath 
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line

    strPath = "IIS://" & strMachineName & "/" & strObjectPath
    Dim vDirObj 
    'create the base iis object and connect ot the IIS metabase
    Set vDirObj = GetObject(strPath & "/ROOT/" & DirName)
    vDirObj.AppCreate (False)
    
    ' vDirObj.AppEnable (True)
    ' vDirObj = vDirObj.AppWamClsid
    
    vDirObj.SetInfo
    
    Set vDirObj = Nothing
    
End Sub
Sub CreateAppVirtualDir(thePath, theRoot, DirName, DirAuth )
'on error resume next
    Dim IISObject 
    Dim vDir 
    Dim vRoot
    Dim strMachineName, strObjectPath, strPath, success 
    
    strMachineName = "localhost"
    strObjectPath = "W3SVC/1"
    ' get path from command line
    
    strPath = "IIS://" & strMachineName & "/" & strObjectPath
     On Error Resume Next
    Set IISObject = GetObject(strPath)
    Set vRoot = IISObject.GetObject("IIsWebVirtualDir", "Root")
   if err.number <> 0 then

		Response.Write "<h2><small>" & "Your ChemOffice Webserver Application was successfully created. However, the Web Wizard was unable" & " to create a virual directory for " & """" & DirName & """" & ".</h2></small>"
		Response.Write "<br>"
		Response.Write "<h3><small>" & "Before using the newly created ChemOffice Webserver Application, a virtual directory will need to be created manually."
		Response.Write "<br>"
		Response.Write "Please contact the Administrator for this computer and have a virtual directory created for wwwroot/chemoffice/" & DirName   & ".</h3></small>"
		Response.end
   end if
    Set vDir = vRoot.GetObject("IIsWebVirtualDir", DirName)
    If Not Err.Number = 0 Then
        Set vDir = vRoot.Create("IIsWebVirtualDir", DirName)
    End If
    vDir.AppFriendlyName = DirName
    vDir.AspScriptFileCacheSize = 200
    vDir.AspScriptTimeout = 600
    vDir.AccessRead = True
    vDir.AccessWrite = True
    vDir.AccessScript = True
    vDir.AspEnableParentPaths = True
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
    vDir.AuthNTLM = False
    If DirAuth = True Then
        vDir.AuthNTLM = False
        vDir.AuthAnonymous = True
        vDir.AnonymousUserName = "Administrator"
        vDir.AuthBasic = False
    End If
    vDir.AppCreate True
    
    vDir.SetInfo
    
    Set vDir = Nothing
    Set vRoot = Nothing
    Set IISObject = Nothing
End Sub

Function GetCOWSPath()
    Set INIVar = CreateObject("cowsUtils.cowsini")
    theDrive = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DRIVE", "cows.ini")
    theServerRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "SERVER_DIR", "cows.ini")
    theDocumentRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "DOC_ROOT", "cows.ini")
    theCowsRoot = INIVar.VBGetPrivateProfileString("GLOBALS", "COWS_ROOT", "cows.ini")
    theCOWSPath = theDrive & "\" & theServerRoot & "\" & theDocumentRoot & "\" & theCowsRoot
    GetCOWSPath = theCOWSPath
    Set INIVar = Nothing
End Function

Sub RemoveDir(DirName)

    Dim IISObject
  
    Dim vRoot
    Dim strMachineName, strObjectPath, strPath, success
    folderpath = server.MapPath("/" & dirname)
    
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
    
    deleteDir(folderpath)
    
End Sub

Sub deleteDir(folderpath)

	on error resume next
	Set theFile = CreateObject("Scripting.FileSystemObject")
	theFile.DeleteFolder folderpath,true
	Set theFile = nothing
end Sub
%>
