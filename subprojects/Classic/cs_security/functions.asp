<%
Dim listNum
Dim bCS_SECURITY
listNum = 0

bINIfound = true
'Create cowsini object
Set INIVAR= Server.CreateObject("cowsUtils.cowsini")
' Get chemoffice.ini path
chemoffice_ini_path = Server.MapPath("/chemoffice/config/chemoffice.ini")
on error resume next
coe_version = INIVAR.VBGetPrivateProfileString("GLOBALS", "CHEMOFFICE_ENTERPRISE_VERSION", Trim(chemoffice_ini_path))
if Ucase(coe_version) = "INIEMPTY" then coe_version = "?.?.?"

' Read installed apps
loaded_apps = INIVAR.VBGetPrivateProfileString("GLOBALS", "APPNAME", Trim(chemoffice_ini_path))
wizard_apps = INIVAR.VBGetPrivateProfileString("GLOBALS", "WIZARD_APPNAMES", Trim(chemoffice_ini_path))

bCS_SECURITY = INIVAR.VBGetPrivateProfileString("GLOBALS", "CS_SECURITY", Trim(chemoffice_ini_path))
if bCS_SECURITY = "INIEMPTY" then bCS_SECURITY = 0

if loaded_apps <> "" and wizard_apps <> "" then
	all_apps = loaded_apps & "," & wizard_apps
else 
	if not loaded_apps <> "" and wizard_apps <> "" then
		all_apps =  wizard_apps
	else 
		if loaded_apps <> "" and not wizard_apps <> "" then
			all_apps =  loaded_apps
		end if
	end if
end if

if UCase(loaded_apps) = "INIEMPTY" then
	bINIfound = False
	Response.Write  "<b>" & chemoffice_ini_path & " is missing or empty.  No application links can be displayed." & "</b>"
	Response.end
end if
loaded_apps_array = Split(all_apps,",",-1)


FUNCTION renderBoxBegin (title, additional)
	renderBoxBegin = "" & vbcrlf  & vbcrlf &_
"<!-- ------------------------------------------------------------------------------- -->" & vbcrlf &_
"<!-- SECTION BOX: " & uCase(title) & " -->" & vbcrlf &_
"<!-- ------------------------------------------------------------------------------- -->" & vbcrlf &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"">" & vbcrlf &_
"	<tr>" & vbcrlf &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/hl.gif"" width=""15"" height=""22""></td>" & vbcrlf  &_
"		<td bgcolor=""#E1E1E1"" width=""100%""><strong>" & title & "</strong>&nbsp;&nbsp;&nbsp;&nbsp;" & additional & "</td>"  & vbcrlf &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/hr.gif"" width=""15"" height=""22""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf  &_
"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">" & vbcrlf  &_
"	<tr>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/tl.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td width=""100%""><img border=""0"" src=""/cs_security/graphics/pixel.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/tr.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf  &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"" width=""100%"">" & vbcrlf  &_
"	<tr>" & vbcrlf  &_
"		<td background=""/cs_security/graphics/box/ml.gif""><img border=""0"" src=""/cs_security/graphics/box/ml.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td width=""100%"" valign=""top"">" & vbcrlf 
END FUNCTION

FUNCTION renderBoxEnd()
	renderBoxEnd = "		" & vbcrlf  &_
"	</td>" & vbcrlf  &_
"		<td background=""/cs_security/graphics/box/mr.gif""><img border=""0"" src=""/cs_security/graphics/box/mr.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>" & vbcrlf  &_
"<table border=""0"" cellpadding=""0"" style=""border-collapse: collapse"" width=""100%"">" & vbcrlf  &_
"	<tr>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/bl.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"		<td background=""/cs_security/graphics/box/bm.gif"" width=""100%"">" & vbcrlf  &_
"		<img border=""0"" src=""/cs_security/graphics/box/bm.gif"" width=""5"" height=""15""></td>" & vbcrlf  &_
"		<td><img border=""0"" src=""/cs_security/graphics/box/br.gif"" width=""15"" height=""15""></td>" & vbcrlf  &_
"	</tr>" & vbcrlf  &_
"</table>"
END FUNCTION

Function RenderLineCaption(caption, cssclass)
	Dim out
	
	if IsNull(caption) then 
		out = ""
	else
		out = "<span class=""" & cssclass & """>" & caption & "</span>"
	End if
	RenderLineCaption = out
End function

Function RenderLink(display, tip, url, popup, addPipe, cssclass)
	Dim out
	
	href = "href='" & url & "'"
	target = ""
	onclick = ""
	Select Case lcase(popup)
		Case "dialog"
			href = "href='#'"
			onclick = "onclick=""OpenDialog('" & url & "', '" & replace(display," ","_") & "', 2); return false"""
		Case "browser"
			target = "target='_new'"
	End Select
	if addPipe then out = " | "
	out = out &  "<a class=""" & cssclass & """ "  & href & " " & onclick &  " title=""" & tip & """ " & target & " >" & display & "</a>"
	RenderLink = out
End function 

function CheckPrivilege(privs)
	Dim out, i
			
	if IsNull(privs) then 
		out = true
	else	
		if InStr(1, privs, ":") > 0 then
			arr = Split(privs,":")
			for i = 0 to Ubound(arr,1)
				temp = Session(arr(i) & dbkey)
				if i > 0 then 
					out = out OR temp
				else
					out = temp
				end if		 	
			next
		else 
			out = Session(privs & dbkey)
		end if
	end if
	CheckPrivilege = out
End function

Sub GetApps(app_type, caption, dropdownThreshold)
	Dim linkDic 
	Set linkDic = Server.CreateObject("Scripting.Dictionary")
	
	For j = 0 to UBound(loaded_apps_array)
		APP_NAME = loaded_apps_array(j)
		bShow = false
		SECTION_NAME =  INIVAR.VBGetPrivateProfileString(CStr(APP_NAME), "SECTION", Trim(chemoffice_ini_path))
				
		if UCase(Trim(SECTION_NAME))=UCase(Trim(app_type)) then
			APP_DBNAMES =  INIVAR.VBGetPrivateProfileString(CStr(APP_NAME), "DBNAMES", Trim(chemoffice_ini_path))
			GLOBAL_SEARCH= INIVAR.VBGetPrivateProfileString(CStr(loaded_apps_array(j)), "GLOBAL_SEARCH", Trim(chemoffice_ini_path))
			if GLOBAL_SEARCH ="1"  then
				GLOBAL_SEARCH_DISPLAY_NAME= INIVAR.VBGetPrivateProfileString(CStr(loaded_apps_array(j)), "GLOBAL_SEARCH_DISPLAY_NAME", Trim(chemoffice_ini_path))
				GLOBAL_SEARCH_START_PATH= INIVAR.VBGetPrivateProfileString(CStr(loaded_apps_array(j)), "GLOBAL_SEARCH_START_PATH", Trim(chemoffice_ini_path))
				if not linkDic.Exists(Trim(GLOBAL_SEARCH_DISPLAY_NAME)) then linkDic.Add Trim(GLOBAL_SEARCH_DISPLAY_NAME),GLOBAL_SEARCH_START_PATH 		
			end if
			temp = split(APP_DBNAMES, ",", -1)
			for k = 0 to UBound(temp)
				DBNAME = temp(k)
				DBNAME_START_PATH=INIVAR.VBGetPrivateProfileString(CStr(APP_NAME), CStr(DBNAME & "_START_PATH"), Trim(chemoffice_ini_path))
				DBNAME_DISPLAY_NAME=INIVAR.VBGetPrivateProfileString(CStr(APP_NAME), CStr(DBNAME & "_DISPLAY_NAME"), Trim(chemoffice_ini_path)) 
				if not linkDic.Exists(Trim(DBNAME_DISPLAY_NAME))and Trim(DBNAME_DISPLAY_NAME) <> "ChemACX" then linkDic.Add Trim(DBNAME_DISPLAY_NAME),DBNAME_START_PATH 		
			next
		end if
	next
	renderLinkList linkDic, caption, dropdownThreshold
	Set linkDic = Nothing
end sub

Sub renderLinkList (byref dic , caption, dropdownThreshold)
	Dim listName
	listNum = listNum +1
	listName = "list" & Cstr(listNum)
	
	if  dic.count >= CInt(dropdownThreshold) then
		Response.Write "<table border=0 width=""100%""><tr><td width=""20%"" align=right>"
		Response.Write "<span class=""linkListCaption"" >" & caption & "</span>"
		Response.Write "</td>"
		Response.Write"<td align=left>"
		Response.Write "<select name=""" & listName & """>" & vbcrlf
		Response.Write "<Option value=#>Select a database ----&gt;</option>"
		for each link in dic
			Response.Write "<option value=""" & dic.Item(link) & """>" & link & "</option>" & vbcrlf
		Next
		Response.write "</select>" & vbcrlf
		Response.Write "<a class=""HomeLink"" href=""#"" onclick=""document.location.href=document.form1." & listName & ".options[document.form1." & listName & ".options.selectedIndex].value;return false"">&nbsp;Go</a>"
		Response.Write "</td></tr></table><BR/>"
	else
		for each link in dic
			Response.Write "<a class=""HomeLink"" href=""" & dic.Item(link) & """>" & link & "</a><BR/>" & vbcrlf
		Next
	end if
End sub

%>

<script language="javascript" runat="server">
// Parse error formatting function
function reportParseError(error)
{
  var s = "";
  for (var i=1; i<error.linepos; i++) {
    s += " ";
  }
  r = "<font face=Verdana size=2><font size=4>XML Error loading '" + 
      error.url + "'</font>" +
      "<P><B>" + error.reason + 
      "</B></P></font>";
  if (error.line > 0)
    r += "<font size=3><XMP>" +
    "at line " + error.line + ", character " + error.linepos +
    "\n" + error.srcText +
    "\n" + s + "^" +
    "</XMP></font>";
  return r;
}

// Runtime error formatting function
function reportRuntimeError(exception)
{
  return "<font face=Verdana size=2><font size=4>XSL Runtime Error</font>" +
      "<P><B>" + exception.description + "</B></P></font>";
}
</script>