<%
dbkey = "drugdeg"


Response.Expires = -1
Dim homeXml
Dim oSections, oSection
Dim fpath, sectionName, sectionCaption, sectionPrivilege

' Cache the xml representation of the home page in Applcation scope
if isObject(Application("homeXml")) then 
	Set homeXml = Application("homeXml")
else	
	Set homeXml = Server.CreateObject("Microsoft.FreeThreadedXMLDOM")
	homeXml.async = false
	on error resume next
	fpath = server.MapPath("/cs_security/config/home.xml")
	homeXml.load(fpath)
	'Check for parser errors
	If homeXml.parseError.errorCode <> 0 Then 
		Application("homeXml") = ""
		Response.Write reportParseError(homeXml.parseError)
	else  
		Set Application("homeXml") = homeXml
	end if
End if	
%>

<%
' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
if Session("MustChangePassword") then response.redirect "/cs_security/password.asp?dbkey=cs_security"
Dim vid_debug
vid_debug = false		
bINIfound = true
Set INIVAR= Server.CreateObject("cowsUtils.cowsini")
fullpath = Request.ServerVariables("path_translated")
fullpath = replace(lcase(fullpath),"\cs_security", "")
fullpath_array = split(fullpath,"\",-1)
last_element_name = fullpath_array(Ubound(fullpath_array))
chemoffice_ini_path=replace(fullpath,last_element_name, "config\chemoffice.ini")
if vid_debug then chemoffice_ini_path = "c:\inetpub\wwwroot\chemoffice\config\chemoffice.ini"
'Response.Write chemoffice_ini_path
'Response.end
loaded_apps = INIVAR.VBGetPrivateProfileString("GLOBALS", "APPNAME", Trim(chemoffice_ini_path))
if UCase(loaded_apps) = "INIEMPTY" then
	bINIfound = False
	MessageText =  "<b>" & chemoffice_ini_path & " is missing or empty.  No application links can be displayed." & "</b>"
end if
loaded_apps_array = Split(loaded_apps,",",-1)
Set INIVAR = Nothing

dbkey = "cs_security"

'When ChemACX is not in the privtable list then the link appears for everyone.
if inStr(1,Application("PrivTableList"),"CHEMACX_PRIVILEGES")<= 0 then Session("BROWSE_ACX" & dbkey) = True
if Not Session("DD_SEARCH" & "drugdeg") = true then
	response.redirect "/cs_security/login.asp?forceManualLogin=1"
	
end if

'move to correct page based on login
'Set DataConn = GetConnection(dbkey, formgroup, basetable)
'privTable= Application("PRIV_TABLE_NAME")
'roles_id_str = GetGrantedRoles(dbkey, DataConn,privTable)
'Response.Write roles_id_str
'if instr(1,"DRUGDEG_ADMINISTRATOR",roles_id_str) > 0 then

'end if

%>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->

<%
' Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved
dbkey = "cs_security"
if Session("MustChangePassword") then response.redirect "/cs_security/password.asp?dbkey=cs_security"
if Not Session("DD_SEARCH" & "drugdeg") = true then response.redirect "/cs_security/login.asp?forceManualLogin=1"

'When ChemACX is not in the privtable list then the link appears for everyone.
if inStr(1,Application("PrivTableList"),"CHEMACX_PRIVILEGES")<= 0 then Session("BROWSE_ACX" & dbkey) = True
%>

<!-- #include virtual="/cs_security/variables.asp" -->
<!-- #include virtual="/cs_security/functions.asp" -->

<%
PAGE_URL = "CambridgeSoft ChemOffice Enterprise Ultra"
PAGE_COLOR = color_blue
PAGE_APP = "header_" & color_blue & "_coent.gif"
TOP_NAV = "<a href=""/cfserveradmin/AdminSource/webeditor.asp""><b>Administrative Tools</b></a>&nbsp;|&nbsp;<a href=""http://www.cambridgesoft.com/services/topics.cfm?FID=6"" target=""_blank""><b>Technical Notes</b></a>"
%>

<!-- #include virtual="/cs_security/header.asp" -->

<table border="0" width="100%" cellpadding="0" style="border-collapse: collapse"><tr><td align="right">
<%
CS_SEC_UserName = UCase(Request.Cookies("CS_SEC_UserName"))
If Len(CS_SEC_UserName) > 0 Then Response.write "Current login: " & CS_SEC_UserName & "&nbsp;&nbsp;"
dbkey = "drugdeg"
%>
	<a href="/cs_security/login.asp?ClearCookies=true"><img border="0" SRC="/cs_security/graphics/button/btn_logoff.gif" alt="Log Off"></a>
</td></tr></table><br>
<form name="form1">
<table border="0" width="100%" style="border-collapse: collapse">
	<tr>
	<%
	Set oSections = homeXml.documentElement.selectNodes("section")
	i=0
	For each oSection in oSections
		i= i + 1
		isEven = false
		sectionName =  oSection.getAttribute("name")
		sectionCaption = oSection.getAttribute("caption")
		sectionPrivilege = oSection.getAttribute("privilege")
		
		bSectionPriv = CheckPrivilege(sectionPrivilege)
		
		Response.Write "<td valign=""top"" width=""50%"">"
		Response.Write renderBoxBegin (sectionCaption,"")
		
		if bSectionPriv then 
			Set oLines = oSection.selectNodes("line")			
			For each oLine in oLines
				linePrivilege = oLine.getAttribute("privilege")
				
				bLinePrivilege = CheckPrivilege(linePrivilege)
				if bLinePrivilege then
					Response.write renderLineCaption(oLine.getAttribute("caption"), "LineCaption")
					Set oLinks = oLine.selectNodes("link")
					linkCount = oLinks.length
					j = 0 
					For each oLink in oLinks
							
						addPipe = false
						linkDisplay = oLink.getAttribute("display")
						linkUrl = oLink.getAttribute("url")
						linkTip = oLink.getAttribute("tip")
						linkPopup = oLink.getAttribute("popup")
						linkPrivilege = oLink.getAttribute("privilege")
							
						bLinkPrivilege = CheckPrivilege(linkPrivilege)
							
						if bLinkPrivilege then
							j = j + 1
							Response.write RenderLink(linkDisplay, linkTip, linkUrl,linkPopUp,  j > 1, "HomeLink")
						End if		
					Next
					Response.Write "<BR/>"
				end if
			Next
			Set oCOiniApps = oSection.selectNodes("chemofficeiniApps")
			For each oCOiniAppType in oCOiniApps
				appType = oCOiniAppType.getAttribute("apptype")
				linkListCaption = oCOiniAppType.getAttribute("caption")
				useDropdownThreshold = oCOiniAppType.getAttribute("useDropdownThreshold")
				if isNull(useDropdownThreshold) then useDropdownThreshold = 0
					
				GetApps appType, linkListCaption, useDropdownThreshold
			Next  
		end if	
		Response.Write renderBoxEnd 
		if (i mod 2) = 0 then isEven = true
		
		if isEven then 
			Response.Write "</td></tr><tr><td colspan=""2"">&nbsp;</td></tr>"
		else
			Response.Write "</td><td width=""20"">&nbsp;&nbsp;&nbsp;&nbsp;</td>"
		end if
	Next
		if not isEven then Response.Write "<td valign=""top"" width=""50%"">&nbsp;</td></tr><tr><td colspan=""2"">&nbsp;</td></tr>"
	%>
</table>
</form>

<br/>
<div align="center">
	<a href="/cs_security/login.asp?ClearCookies=true"><img border="0" SRC="/cs_security/graphics/button/btn_logoff.gif" alt="Log Off"></a>
</div>
<%'DumpCookies%>

<!-- #include virtual="/cs_security/footer.asp" -->
