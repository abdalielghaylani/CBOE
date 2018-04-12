<%@ Language=VBScript %>
<%
Dim Conn
Dim Cmd
Dim RS

%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->

<html>
<head>
<title><%=Application("appTitle")%> -- Manage Approvals</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script language="JavaScript">
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
	</tr>
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="ManageApprovals_display.asp" method="POST">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Manage Approvals</b>		
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
					<td align="right" valign="top">
						<span title="Pick an option from the list">Current user:</span>
					</td>
					<td>
						<%=ShowSelectBox2("CurrentUserID", UCASE(CurrentUserID), "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
					</td>
					<th colspan="2" align="center" valign="top" nowrap>
						Certification Date
					</th>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Current Location: <%=GetBarcodeIcon()%>
					</td>
					<td valign="top">
						<%ShowLocationPicker "document.form1", "CurrentLocationID", "CurrentLocationBarCode", "CurrentLocationName", 10, 30, false%>
					</td>
					<td align="right" valign="top" nowrap>
						From Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "fromDate:form1:" , "DATE_PICKER:TEXT", "11")%>
						<!--<input type="text" name="fromDate" size="11" value><a href onclick="return PopUpDate(&quot;fromDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						ContainerID: <%=GetBarcodeIcon()%>
					</td>
					<td valign="top">
						<input type="text" size="43" name="containerBarcode" value="<%=Request("containerBarcode")%>" size="8">
					</td>
					<td align="right" valign="top" nowrap>
						To Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "toDate:form1:" , "DATE_PICKER:TEXT", "11")%>
						<!--<input type="text" name="toDate" size="11" value><a href onclick="return PopUpDate(&quot;toDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
					</td>
				</tr>
			</table>
		</td>
		
		
		<td rowspan="3" valign="bottom" align="right">
			<input type="button" value="Reset" onclick="top.location.reload()" id="button1" name="button1">
			<input type="submit" value="Filter" id="submit1" name="submit1">
			<br>
			<%=GetCancelButton()%>
		</td>
	</tr>
</table>
</form>
</body>
</html>
