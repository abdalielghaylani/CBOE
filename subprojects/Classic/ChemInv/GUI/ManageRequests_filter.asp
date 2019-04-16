<%@ Language=VBScript %>
<%
Dim Conn
Dim Cmd
Dim RS

RequestType = Ucase(request("RequestType"))
if RequestType = "" then RequestType = "PENDING"
if RequestType = "NEW" then
	disable1 = "disabled"
elseif RequestType = "PENDING" then
	disable1 = "disabled"
else
	disable3 = "disabled"
end if

%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Manage Requests</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->

<script language="JavaScript">

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

<table border="0" cellspacing="0" cellpadding="2" width="600" align="left">
	<tr>
		<td valign="top" align="left">
			<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
		</td>
		<td align="right" valign="top">
			<a class="MenuLink" target="_top" href="ManageRequests_frset.asp?filter=true" <%=disable1%>>New</a> |
			<!--<a class="MenuLink" target="_top" href="ManageRequests_frset.asp?RequestType=PENDING&amp;filter=true" <%=disable2%>>Pending Requests</a> |-->
			<a class="MenuLink" target="_top" href="ManageRequests_frset.asp?RequestType=CLOSED&amp;filter=true" <%=disable3%>>Closed Requests</a>
		</td>
	</tr>
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="ManageRequests_display.asp?requestType=<%=RequestType%>" method="POST">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Manage Requests</b>		
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
						Requested by:
					</td>
					<td valign="top">
						<%=ShowSelectBox2("userID", "", "SELECT DISTINCT CS_SECURITY.People.User_ID AS Value, CS_SECURITY.People.Last_Name||DECODE(CS_SECURITY.People.First_Name, NULL, '', ', '||CS_SECURITY.People.First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(DisplayText) ASC", 40, RepeatString(18, "&nbsp;"), "")%>
					</td>
					<td align="right" valign="top" nowrap>
						&nbsp;
					</td>
					<td valign="top">
						&nbsp;
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Current Location: <%=GetBarcodeIcon()%>
					</td>
					<td valign="top">
						<%ShowLocationPicker "document.form1", "CurrentLocationID", "CurrentLocationBarCode", "CurrentLocationName", 10, 30, false%>
					</td>
					<td align="right" valign="top" nowrap>
						&nbsp;
					</td>
					<td valign="top">
						&nbsp;
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Delivery Location: <%=GetBarcodeIcon()%>
					</td>
					<td valign="top">
						<%ShowLocationPicker "document.form1", "DeliverToLocationID", "DeliverToLocationBarCode", "DeliverToLocationName", 10, 30, false%>
					</td>
					<th colspan="2" align="center" valign="top" nowrap>
						Request Date
					</th>
					
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Comments:
					</td>
					<td align="left">
						<input type="text" name="RequestComments" size="43">
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
						<input type="text" name="containerBarcode" value="<%=Request("containerBarcode")%>" size="8">
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
			<input type="submit" value="Filter" id="submit1" name="submit1" onclick="Validate(); return false;">
			<br>
			<%=GetCloseButton()%>
		</td>
	</tr>
</table>
</form>
</body>
</html>
