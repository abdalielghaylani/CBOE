<%@ EnableSessionState=True Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
if not Session("IsFirstRequest") then 
	StoreASPSessionID()
else
	Session("IsFirstRequest") = false
	Response.Write("<html><body onload=""window.location.reload()"">&nbsp;<form name=""form1""></form></body></html>")	
end if

CSUserName=ucase(Session("UserName" & "cheminv")) 
CSUserID= Session("UserID" & "cheminv")

if CSUserName = "" or CSUserID = "" then
	response.write "<BR>Access Error:<BR> Credentials not found.<BR>  Please inform the system administrator."
	response.end
end if
%>

<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Recieve Ordered Containers</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
	function Validate(){
		var bWriteError = false;
		var errmsg = "";

	//	if (isNaN(document.form1.containerID.value))
	//	{
	//		bWriteError = true
	//		errmsg += "- Enter a valid Container ID(Internal).\n"	
	//	}
	//	if(errmsg!="")
	//	{
	//		errmsg ="Please fix the following problems:\r" + errmsg;
	//		alert(errmsg);
	//	}
	//	else{

			document.form1.submit();
//			}
	}
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
    var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";



</script>
</head>
<body>

<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
	<tr>
		<td valign="top" align="left">
			<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
		</td>
		<td align="right" valign="top">
			<!---<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=2&amp;filter=true" <%=disable1%>>New</a> |--->
		
		</td>
	</tr>
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="parlevels_display.asp?requestType=<%=RequestType%>" method="POST">
<input TYPE="hidden" NAME="RequestStatusID" VALUE="<%=RequestStatusID%>">
<input TYPE="hidden" NAME="RequestTypeID" VALUE="<%=RequestTypeID%>">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Manage Par Levels</b>		
		</td>
	</tr>	
	<tr>
		<td rowspan="3" valign="top">
			<table>
				<tr>
					<th colspan="4" align="center">
						Filter Criteria					
					</th>
				</tr>
				
				<tr>
					<td align="right" valign="top" nowrap>
						NDC:
					</td>
					<td valign="top">
						<input type="text" name="NDC" value="" size="12">
					</td>
					<%= ShowPickList2("Location", "iLocationId", "","select location_barcode || ' ' || location_name  as displaytext, location_id as value from inv_locations where location_type_id_fk>=1000 order by location_name",25," ","","")%>
				
				</tr>

			</table>
		</td>
		
		
		<td rowspan="3" valign="bottom" align="right">
			<input type="button" value="Reset" onclick="top.location.reload()" id="button1" name="button1">
			<input type="submit" value="Filter" id="submit1" name="submit1" onclick="Validate(); return false;">
			<br>
			<a href="#" onclick="top.window.close()"><image src="../graphics/close_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a
		</td>
	</tr>
</table>
</form>
<% 
CurrentUserIDListName = Request("CurrentUserIDListName")
CurrentUserIDList = Request("CurrentUserIDList")
if CurrentUserIDListName = "" then CurrentUserIDListName = CurrentUserIDList
if CurrentUserIDList <> "" then%>
<script language="javascript">
	document.form1.CurrentUserIDList.options[document.form1.CurrentUserIDList.options.length]= new Option("<%=CurrentUserIDListName%>","<%=CurrentUserIDList%>");
	document.form1.CurrentUserIDList.selectedIndex = document.form1.CurrentUserIDList.options.length - 1;
</script>
<%end if%>
</body>
</html>
