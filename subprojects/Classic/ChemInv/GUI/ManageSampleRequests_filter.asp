<%@ Language=VBScript %>
<%
Dim Conn
Dim Cmd
Dim RS

RequestTypeID = Request("RequestTypeID")
RequestStatusID = Request("RequestStatusID")

if RequestTypeID = "" then RequestTypeID = "2"
if RequestStatusID = "" then RequestStatusID = "3"
if RequestStatusID = "2" then
	disable1 = "disabled"
elseif RequestStatusID = "8" then
	disable7 = "disabled"
elseif RequestStatusID = "3" then
	disable2 = "disabled"
elseif RequestStatusID = "4" then
	disable3 = "disabled"
elseif RequestStatusID = "5" then
	disable4 = "disabled"
elseif RequestStatusID = "6" then
	disable5 = "disabled"
elseif RequestStatusID = "7" then
	disable6 = "disabled"	
elseif RequestStatusID = "0" then
	disable = "disabled"	
end if

%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->

<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Manage Sample Requests</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script language="JavaScript">
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
    var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";

	function Validate(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";

		if (document.form1.fromDate.value.length != 0) {
			if (!isDate(document.form1.fromDate.value)) {
				errmsg = errmsg + "- Entry must be in date using the format 'mm/dd/yyyy'\r";
				bWriteError = true;
			}
		}
		if (document.form1.toDate.value.length != 0) {
			if (!isDate(document.form1.toDate.value)) {
				errmsg = errmsg + "- Entry must be in date using the format 'mm/dd/yyyy'\r";
				bWriteError = true;
			}
		}
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}

</script>
</head>
<body>

<table border="0" cellspacing="0" cellpadding="2" width="700" align="left">
	<tr>
		<td valign="top" align="left">
			<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
		</td>
		<td align="right" valign="top" nowrap>
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=2&amp;filter=true" <%=disable1%>>New</a> |
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=8&amp;filter=true" <%=disable7%>>Pending</a> |
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=3&amp;filter=true" <%=disable2%>>Approved</a> |
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=4&amp;filter=true" <%=disable3%>>Declined</a> |
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=5&amp;filter=true" <%=disable4%>>Filled</a> |
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=6&amp;filter=true" <%=disable5%>>Closed</a> |
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=7&amp;filter=true" <%=disable6%>>Cancelled</a> | 
			<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=0&amp;filter=true" <%=disable%>>All</a>
		</td>
	</tr>
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="ManageSampleRequests_display.asp?requestType=<%=RequestType%>" method="POST">
<input TYPE="hidden" NAME="RequestStatusID" VALUE="<%=RequestStatusID%>">
<input TYPE="hidden" NAME="RequestTypeID" VALUE="<%=RequestTypeID%>">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Manage Sample Requests</b>		
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
						Requested For:
					</td>
					<td valign="top">
						<%=ShowSelectBox2("userID", "", "SELECT DISTINCT CS_SECURITY.People.User_ID AS Value, CS_SECURITY.People.Last_Name||DECODE(CS_SECURITY.People.First_Name, NULL, '', ', '||CS_SECURITY.People.First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(DisplayText) ASC", 40, RepeatString(18, "&nbsp;"), "")%>
					</td>
					<td align="right" valign="top" nowrap>
						&nbsp;
					</td>
					<th colspan="2" align="center" valign="top" nowrap>
						Request Date
					</th>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Delivery Location: <%=GetBarcodeIcon()%>
					</td>
					<td valign="top">
						<%ShowLocationPicker "document.form1", "DeliverToLocationID", "DeliverToLocationBarCode", "DeliverToLocationName", 10, 30, false%>
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
						&nbsp;
					</td>
					<td align="left">
						&nbsp;
					</td>
					<td align="right" valign="top" nowrap>
						To Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "toDate:form1:" , "DATE_PICKER:TEXT", "11")%>
						<!--<input type="text" name="toDate" size="11" value><a href onclick="return PopUpDate(&quot;toDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						&nbsp;
					</td>
					<td valign="top">
						&nbsp;
					</td>
				</tr>
			</table>
		</td>
		
		
		<td rowspan="3" valign="bottom" align="center">
		    <a class="MenuLink" HREF="#" onclick="top.location.reload(); return false;"><img src="/cheminv/graphics/sq_btn/reset.gif" alt="Click to reset the filter" border=0 /></a><br />
			<a class="MenuLink" HREF="#" onclick="Validate();; return false;"><img src="/cheminv/graphics/sq_btn/filter.gif" alt="Click to filter the display" border=0 /></a><br />			
			<%=GetCloseButton()%>
		</td>
	</tr>
</table>
</form>
</body>
</html>
