<%@ Language=VBScript %>
<%
Dim Conn
Dim Cmd
Dim RS

OrderStatusID = Request("OrderStatusID")
if OrderStatusID = "" then OrderStatusID = "1"
if OrderStatusID = "1" then
	disable1 = "disabled"
	DateLabel = "Date Created"
elseif OrderStatusID="2" then
	disable2 = "disabled"
	DateLabel = "Date Shipped"
elseif OrderStatusID="3" then
	disable3 = "disabled"
	DateLabel = "Date Shipped"
elseif OrderStatusID="4" then
	disable4 = "disabled"
	DateLabel = "Date Created"
else
	DateLabel="Date"
end if

%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Manage Orders</title>
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
		<td align="right" valign="top">
			<a class="MenuLink" target="_top" href="ManageOrders_frset.asp?OrderStatusID=1&amp;filter=true" <%=disable1%>>New</a> |
			<a class="MenuLink" target="_top" href="ManageOrders_frset.asp?OrderStatusID=2&amp;filter=true" <%=disable2%>>Shipped</a> |
			<a class="MenuLink" target="_top" href="ManageOrders_frset.asp?OrderStatusID=3&amp;filter=true" <%=disable3%>>Closed</a> |
			<a class="MenuLink" target="_top" href="ManageOrders_frset.asp?OrderStatusID=4&amp;filter=true" <%=disable4%>>Cancelled</a>
		</td>
	</tr>
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="ManageOrders_display.asp?OrderStatusID=<%=OrderStatusID%>" method="POST">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Manage Orders</b>		
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
						Ship to Name:
					</td>
					<td valign="top">
						<input type="text" name="ShipToName" size="15">
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
						<%ShowLocationPicker "document.form1", "DeliveryLocationID", "DeliveryLocationBarCode", "DeliveryLocationName", 10, 30, false%>
					</td>
					<th colspan="2" align="center" valign="top" nowrap>
						<%=DateLabel%>
					</th>
					
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						ContainerID: <%=GetBarcodeIcon()%>
					</td>
					<td valign="top">
						<input type="text" name="containerBarcode" value="<%=Request("containerBarcode")%>" size="8">
					</td>
					<td align="right" valign="top" nowrap>
						From Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "fromDate:form1:" , "DATE_PICKER:TEXT", "11")%>
						<!--<input type="text" name="fromDate" size="11" value><a href onclick="return PopUpDate(&quot;fromDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						&nbsp;
					</td>
					<td valign="top">
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
