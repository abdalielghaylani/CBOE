<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<%
' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

bINIfound = true
Set INIVAR= Server.CreateObject("cowsUtils.cowsini")
fullpath = Request.ServerVariables("path_translated")
fullpath = replace(lcase(fullpath),"\cs_security", "")
fullpath_array = split(fullpath,"\",-1)
last_element_name = fullpath_array(Ubound(fullpath_array))
chemoffice_ini_path=replace(fullpath,last_element_name, "config\chemoffice.ini")
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
if Not Session("UserValidated" & dbKey) = 1 then
	response.redirect "login.asp?forceManualLogin=1"
end if
%>
<html>
<head>
<meta http-equiv="Content-Language" content="en-us">
<meta name="GENERATOR" content="Microsoft FrontPage 5.0">
<meta name="ProgId" content="FrontPage.Editor.Document">
<meta http-equiv="Content-Type" content="text/html; charset=windows-1252">
<title>CS Home</title>
<!--#include virtual="/cfserverasp/source/cs_security/cs_security_utils.js"-->
</head>
<body>
<!--#INCLUDE FILE = "header.asp"-->
<br><br><br>
<table border="0" width="934">
	<tr>
		<td width="912">
			<table border="1" style="border-collapse: collapse" bordercolor="#111111" width="100%" bordercolorlight="#EAEAEA">
			  <tr>
			    <td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber4">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov2.gif" width="23" height="22">
			        <font face="Arial" size="2"><b>Registration System</b></font></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%if Session("SEARCH_REG" & dbKey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<span class="Menulink">Search:</span> 
						<a class="HomeLink" href="/chem_reg/default.asp?formgroup=base_form_group&amp;dbname=reg">Registry</a>
						<%if Session("SEARCH_TEMP" & dbKey) then%>
						 | 
						<a class="HomeLink" href="/chem_reg/default.asp?formgroup=reg_ctrbt_commit_form_group&amp;dbname=reg">Temp</a>
						<%end if%>
			        </td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><span class="Menulink"><a class="HomeLink" href="/chem_reg/default.asp?formgroup=base_form_group&amp;dbname=reg&amp;dataaction=mainpage">Main menu</a></span>
			        </td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%if Session("ADD_COMPOUND_TEMP" & dbKey) OR Session("ADD_BATCH_TEMP" & dbKey)then %>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<span class="Menulink">Enter a new:</span>
						<%if Session("ADD_COMPOUND_TEMP" & dbKey) then%>
						<a class="HomeLink" href="/chem_reg/default.asp?formgroup=reg_ctrbt_form_group&amp;dbname=reg&amp;dataaction=full_commit&amp;record_added=false&amp;PostRelay=1"> Compound</a>
						<%End if%>
						<%if Session("ADD_COMPOUND_TEMP" & dbKey) AND Session("ADD_BATCH_TEMP" & dbKey)then %>
						 | 
						<%End if%>  
						<%if Session("ADD_BATCH_TEMP" & dbKey) then%>
						<a class="HomeLink" href="/chem_reg/default.asp?formgroup=batch_ctrbt_form_group&amp;dbname=reg&amp;dataaction=batch_commit&amp;record_added=false&amp;PostRelay=1">Batch</a>
						<%End if%>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%End if%>
			      <%if Session("REGISTER_TEMP" & dbKey) then%>
			 <!---<tr>			        					<td width="14%">&nbsp;</td>					<td width="63%"><span class="Menulink">Register an </span>			        <a class="HomeLink" href="c:\inetpub\wwwroot\chemoffice\chem_reg\chemloader.exe">SD File</a></td>			        <td width="103%">&nbsp;</td>			      </tr>-->
			      <%End if%>
			      <%end if%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			    </table>
			    </td>
			    <td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber5">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov3.gif" width="24" height="21"> <b>
			        <font face="Arial" size="2">Manage Security</font></b></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%if Session("CSS_CHANGE_PASSWORD" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
			        <a class="HomeLink" href="#" onclick="OpenDialog('/cs_security/cs_security/Password.asp?dbkey=cs_security', 'Mpwd', 2); return false">Change Password</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%End if%>
			      <%if Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
			        <a class="HomeLink" href="#" onclick="OpenDialog('/cs_security/cs_security/manageUsers.asp?dbkey=cs_security', 'Musers', 2); return false">Manage Users</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%End if%>
			      <%if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
			        <a class="HomeLink" href="#" onclick="OpenDialog('/cs_security/cs_security/manageRoles.asp?dbkey=cs_security', 'Mroles', 2); return false">Manage Roles</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			    </table>
			    </td>
			  </tr>
			  <tr>
			    <td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber5">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov3.gif" width="24" height="21"> <b>
			        <font face="Arial" size="2">Inventory Manager</font></b></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%For i = 0 to UBound(loaded_apps_array)
					'if true then
					if  UCase(loaded_apps_array(i)) = "CHEMACX" then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<span class="Menulink">ChemACX:</span> 
						<a class="HomeLink" href="/chemacx/inputtoggle.asp?formgroup=base_form_group&amp;dataaction=db&amp;dbname=chemacx">Search</a>
			        </td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if
			      Next%>
			      <%if Session("INV_BROWSE_ALL" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%" nowrap>
			        <span class="Menulink">Inventory:</span> 
			        <a class="HomeLink" href="/cheminv/cheminv/BrowseInventory_frset.asp?PostRelay=1">Browse</a>
			         | 
			        <a class="HomeLink" href="/cheminv/inputtoggle.asp?dataaction=db&amp;dbname=cheminv">Search</a> 
			        <%If Session("INV_DELETE_CONTAINER" & dbkey) OR Session("INV_MOVE_CONTAINER" & dbkey) OR Session("INV_CHECKOUT_CONTAINER" & dbkey) OR Session("INV_CHECKIN_CONTAINER" & dbkey) OR Session("INV_RETIRE_CONTAINER" & dbkey) then%>
					|
					<a class="HomeLink" HREF="/cheminv/gui/multiScan_frset.asp" target="_top">Scan</a>	
					<%end if%>
			        <%if Session("INV_MANAGE_SUBSTANCES" & dbkey) then%>
					|	
						<a class="HomeLink" href="/cheminv/inputtoggle.asp?formgroup=substances_form_group&amp;dataaction=db&amp;dbname=cheminv">Manage substances</a> 
			        <%end if%>
			        </td>
			        <td>&nbsp;</td>
			      </tr>
			      <%end if%>
			      <%if Session("INV_BROWSE_ALL" & dbkey) AND Session("SEARCH_REG" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
			        <a class="HomeLink" href="/ChemInv/default.asp?TB=Global&amp;formgroup=gs_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;Showbanner=True">Global Search</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			    </table>
			    </td>
			    <td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber7">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov5.gif" width="21" height="22"> <b>
			        <font face="Arial" size="2">Chemical Databases</font></b></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%For i = 0 to UBound(loaded_apps_array)
					'if true then
					if  UCase(loaded_apps_array(i)) = "CHEMACX" then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"> 
						<a class="HomeLink" href="/chemacx/inputtoggle.asp?formgroup=base_form_group&amp;dataaction=db&amp;dbname=chemacx">ChemACX</a>
			        </td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if
			      Next%>
			      <%For i = 0 to UBound(loaded_apps_array)
					if  UCase(loaded_apps_array(i)) = "THEMERCKINDEX" then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
			        <a class="HomeLink" href="/themerckindex/index.asp">The Merck Index</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if
			      Next%>
			      <%For i = 0 to UBound(loaded_apps_array)
					if  UCase(loaded_apps_array(i)) = "DERWENT" then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<a class="HomeLink" href="/derwent/default.asp?formgroup=base_form_group&amp;dbname=wdi">Derwent World Drug Index</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<a class="HomeLink" href="/derwent/default.asp?formgroup=base_form_group&amp;dbname=wda">Derwent World Drug Alerts</a></td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if
			      Next
			      %>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			    </table>
			    </td>
			  </tr>
			  <tr>
			    <td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber3">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov1.gif" width="23" height="22"> <b>
			        <font face="Arial" size="2">BioSAR Browser</font></b></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%if Session("SEARCH_USING_FORMGROUP" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<a class="HomeLink" href="/biosar_browser/BIOSAR_BROWSER/mainpage.asp?formgroup=base_form_group&amp;dbname=BIOSAR_BROWSER">Main Menu</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      <%if Session("ADD_USER_FORMGROUP" & dbkey) OR Session("EDIT_USER_FORMGROUP" & dbkey) OR Session("DELETE_USER_FORMGROUP" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<a class="HomeLink" href="/biosar_browser/default.asp?dataaction=form_management&amp;formgroup=base_form_group&amp;dbname=BIOSAR_BROWSER">Manage your Forms</a>
			        </td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      <%if Session("ADD_ADMIN_SCHEMA" & dbkey) OR Session("EDIT_ADMIN_SCHEMA" & dbkey) OR Session("DELETE_ADMIN_SCHEMA" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">
						<a class="HomeLink" href="/biosar_browser/default.asp?dataaction=schema_management&amp;formgroup=base_form_group&amp;dbname=BIOSAR_BROWSER">Schema Management</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			    </table>
			    </td>
			    <td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber6">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov4.gif" width="23" height="22"> <b>
			        <font face="Arial" size="2">E-Notebook</font></b></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			    </table>
			    </td>
			  </tr>
	
			  <tr>
				<td width="50%" valign="top">
			    <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber4">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov2.gif" width="23" height="22">
			        <font face="Arial" size="2"><b>Document Manager</b></font></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%if Session("SEARCH_DOCS" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/docmanager/docmanager/mainpage.asp">Main Page</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/docmanager/default.asp?formgroup=base_form_group&amp;dbname=docmanager&amp;dataaction=db">Search Documents</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      <%if Session("SUBMIT_DOCS" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/docmanager/docmanager/src/locatedocs.asp">Submit Documents</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      
				</table>
			    </td>
			    <td>
			     <table border="0" style="border-collapse: collapse" bordercolor="#111111" width="100%" id="AutoNumber4">
			      <tr>
			        <td width="77%" bgcolor="#EAEAEA" colspan="2">
			        <img border="0" src="images/discov2.gif" width="23" height="22">
			        <font face="Arial" size="2"><b>Drug Degradation</b></font></td>
			        <td width="123%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%">&nbsp;</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%if Session("DD_SEARCH" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/drugdeg/DRUGDEG/mainpage.asp?formgroup=base_form_group&amp;dbname=DRUGDEG&amp;MadeIn=DDDefault">Main Page</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/drugdeg/default.asp?formgroup=base_form_group&amp;dbname=DRUGDEG&amp;dataaction=db">Search</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
			      <%if Session("DD_ADD_RECORDS" & dbkey) or Session("DD_EDIT_RECORDS" & dbkey) or Session("DD_DELETE_RECORDS" & dbkey) then%>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/drugdeg/default.asp?formgroup=AddParent_form_group&amp;dbname=DRUGDEG&amp;dataaction=parent_full_commit&amp;record_added=false">Add and Edit Parents</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/drugdeg/default.asp?formgroup=Condition_form_group&amp;dbname=DRUGDEG&amp;dataaction=condition_list_admin&amp;record_added=false">Add and Edit Conditions</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <tr>
			        <td width="14%">&nbsp;</td>
			        <td width="63%"><a class="HomeLink" href="/drugdeg/default.asp?formgroup=Salt_form_group&amp;dbname=DRUGDEG&amp;dataaction=salt_list_admin&amp;record_added=false">Add and Edit Salts</a>
					</td>
			        <td width="103%">&nbsp;</td>
			      </tr>
			      <%end if%>
				</table>
			    </td>
			   </tr>
			  
			  
			</table>
		</td>
	</tr>
</table>
<br><br>
<div align="center">
	<a class="MenuLink" href="/cs_security/login.asp?ClearCookies=true"><img border="0" SRC="graphics/logoff.gif"></a>
</div>
<%'DumpCookies%>
</body>

</html>