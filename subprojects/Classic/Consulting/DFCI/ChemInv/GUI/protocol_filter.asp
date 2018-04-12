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

		if (document.form1.Protocol.value=="" && document.form1.AddProtocol.value=="")
		{
			bWriteError = true
			errmsg += "- You must fill in either a protocol to search or a protocol to add.\n"	
		}
		if(errmsg!="")
		{
			errmsg ="Please fix the following problem:\r" + errmsg;
			alert(errmsg);
		}
		else{

			if (!document.form1.AddProtocol.value=="") {
		
			var tempCSUserName= "<%=Session("UserName" & "cheminv")%>";
			var tempCSUserID= "<%=Server.URLEncode(CryptVBS(lcase(Session("UserID" & "cheminv")),"protocol"))%>"
			var identifier = document.all["AddProtocol"].value;
			var startdate = document.all["AddStartDate"].value;
			var name = document.all["AddProtocolName"].value;
			var ncinum = document.all["AddNCINum"].value;
			var enddate = document.all["AddEndDate"].value;
			var sponsorid = document.all["SponsorID"].value;
			var httpResponse ="";
			msg="";
			var strURL = "http://" + serverName + "/cheminv/api/dfci_manageprotocol.asp?ProtocolIdentifier=" + identifier + "&StartDate=" + startdate + "&EndDate=" + enddate + "&ProtocolName=" + name + "&NCIProtocolNum=" + ncinum + "&SponsorID=" + sponsorid; 	
			strURL = strURL + "&tempCSUserId=" + tempCSUserID + "&tempCSUserName=" + tempCSUserName;
			//document.write(strURL);
			//alert(strURL);
			var httpResponse = JsHTTPGet(strURL);
			if (httpResponse >= 0){
			}
			else{
			if (httpResponse != "-9602")
				{
					alert("There was a problem adding the protocol. \n Please check the values and try again.");
				};
			}	



			document.form1.Protocol.value=document.form1.AddProtocol.value;
			}

			document.form1.submit();
			}
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
<form name="form1" target="DisplayFrame" action="protocol_display.asp" method="POST">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Protocols</b>		
		</td>
	</tr>	
	<tr>
		<td rowspan="3" valign="top">
			<table>
				<tr>
					<th colspan="4" align="center">
						Search Criteria					
					</th>
				</tr>
				
				<tr>
					<td align="right" valign="top" nowrap>
						Protocol:
					</td>
					<td valign="top">
						<input type="text" name="Protocol" value="" size="5" maxsize="5">
					</td>
					
				</tr>
				<tr>
					<td colspan="4" align="center" valign=bottom><BR><BR>
			<input type="button" value="Reset" onclick="top.location.reload()" id="button1" name="button1">
			<input type="submit" value="Filter" id="submit1" name="submit1" onclick="Validate(); return false;">
							
					</td>
				</tr>


			</table>
		</td>
		<td rowspan="3" valign="top">
			<table>
				<tr>
					<th colspan="4" align="center">
						Add new protocol					
					</th>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						DFCI Protocol Id
					</td>
					<td valign="top">
						<input type="text" name="AddProtocol" value="" size="10" maxsize="10">
					</td>
					<td valign="top"> NCI Protocol No.
					</td>
					<td><input type="text" name="AddNCINum" value="" size="20" maxsize="20"></td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Protocol Name
					</td>
					<td valign="top">
						<input type="text" name="AddProtocolName" value="" size="20" maxsize="100">
					</td>
	<td valign="top"> Sponsor ID
					</td>
					<td><input type="text" name="SponsorID" value="" size="20" maxsize="20"></td>

				</tr>
				
				<tr>
					<td align="right" valign="top" nowrap>
					Start Date:
					</td>
					<td valign="top">
						<input type="protStartDate" name="AddStartDate" value="" size="30" maxsize="50">
					</td>
					<td align="right" valign="top" nowrap>
					End Date:
					</td>
					<td valign="top">
						<input type="text" name="AddEndDate" value="" size="30" maxsize="50">
					</td>
		
				</tr>
				<tr> 
					<TD COLSPAN=4>
						<input type="button" value="Reset" onclick="top.location.reload()" id="button1" name="button1">
						<input type="submit" value="Add Protocol" id="submit1" name="submit1" onclick="Validate(); return false;">
					</TD>
						
				</TR>			
			</table>
		</td>
		
		
		<td rowspan="3" valign="bottom" align="right">
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
