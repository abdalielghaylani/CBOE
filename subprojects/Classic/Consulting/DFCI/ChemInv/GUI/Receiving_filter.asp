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
	function addHiddenField(inputName, inputValue) {
	if (document.getElementById(inputName)) {
		document.getElementById(inputName).value=inputValue;
} else {
	var newinput = document.createElement('input');
	newinput.setAttribute("id",inputName);
	newinput.setAttribute("type","hidden");
	newinput.setAttribute("name",inputName);
	newinput.setAttribute("value",inputValue);
	document.form1.appendChild(newinput);
}

}

	function Validate(){
		var bWriteError = false;
		var errmsg = "";
		document.form1.barcodelist.value=document.form1.barcodelist.value + "|" + document.form1.barcode.value + "|";
		
		if (isNaN(document.form1.containerID.value))
		{
			bWriteError = true
			errmsg += "- Enter a valid Container ID(Internal).\n"	
		}
		if(errmsg!="")
		{
			errmsg ="Please fix the following problems:\r" + errmsg;
			alert(errmsg);
		}
		else{

			document.form1.barcode.focus();
			document.form1.submit();
			document.form1.barcode.value="";
			}
	}
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
    var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";


	function PopUpDate(strControl,fullsourcepath,formname)
{
	//SYAN added 12/15/2003 to fix CSBR-35466
	//append date_format to fullsourcepath. This fix is made this way to avoid 
	//change of function signature. Yuk.
	var quesPos = fullsourcepath.indexOf('?')
	var param;
	if (fullsourcepath.indexOf('?') > 0) {
		param = fullsourcepath.substring(quesPos + 1, fullsourcepath.length);
		fullsourcepath = fullsourcepath.substring(0, quesPos);
	}
	//End of SYAN modification
	
	var browserNetscape = "<%=strTrueFalse(detectNetscape())%>"
	var strURL = fullsourcepath + "?CTRL=" + strControl;
	if (formname !=null){
		var strCurDate = "document.forms[" + formname + "].elements[strControl].value";
	}
	else{
		var strCurDate = document.forms["cows_input_form"].elements[strControl].value;
	}
	if ((strCurDate != null) && (strCurDate != "undefined")) {
		if (strCurDate.length > 0){
			strURL += "&INIT=" + strCurDate;
		}
	}
	
	//SYAN added 12/15/2003 to fix CSBR-35466
	if (param != null) {
		strURL = strURL + '&' + param
	}
	//End of SYAN modification
	
	var windowDatePicker = ""
	if (windowDatePicker.name == null){
		if (browserNetscape.toLowerCase() == "true"){
			windowDatePicker = window.open(strURL,"dp","toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0," + "width=190,height=200");
			windowDatePicker.focus();
			}
		else{
			windowDatePicker = window.open(strURL,"dp","toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0," + "width=190,height=200,left=" + (window.event.screenX - 190) + ",top=" + (window.event.screenY + 20));
			windowDatePicker.focus();
		}
	}
	else{
		windowDatePicker.focus()
	}
	if (Version.indexOf("MSIE 5.0") != -1){
		date_picker_used = true
	}
	return false
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
			<!---<a class="MenuLink" target="_top" href="ManageSampleRequests_frset.asp?RequestStatusID=2&amp;filter=true" <%=disable1%>>New</a> |--->
		
		</td>
	</tr>
</table>
<br clear="all">
<form name="form1" target="DisplayFrame" action="Receiving_display.asp?requestType=<%=RequestType%>" method="POST">
<input TYPE="hidden" NAME="RequestStatusID" VALUE="<%=RequestStatusID%>">
<input TYPE="hidden" NAME="RequestTypeID" VALUE="<%=RequestTypeID%>">
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td colspan="4" align="Center">
			<b>Receive Ordered Containers</b>		
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
					
					</td>
					<td valign="top">
						<%'=ShowSelectBox2("CurrentUserIDList", "", "SELECT DISTINCT CS_SECURITY.People.User_ID AS Value, CS_SECURITY.People.Last_Name||DECODE(CS_SECURITY.People.First_Name, NULL, '', ', '||CS_SECURITY.People.First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(DisplayText) ASC", 40, RepeatString(18, "&nbsp;"), "")%>
					</td>
					<td align="right" valign="top" nowrap>
						NDC:
					</td>
					<td valign="top">
						<input type="text" name="CAS" size="20">
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Supplier Name:
					</td>
				<td valign="top">
						<%=ShowSelectBox2("SupplierID", "", "SELECT supplier_id AS Value, supplier_name AS DisplayText FROM inv_suppliers ORDER BY lower(DisplayText) ASC", 40, RepeatString(18, "&nbsp;"), "")%>
					</td>
					<td align="right" valign="top" nowrap>
						ACX ID:
					</td>
					<td valign="top">
						<input type="text" name="ACXID" size="20">
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Catalog Number:
					</td>
					<td valign="top">
						<input type="text" name="SupplierCatNum" size="20">
					</td>
					<th colspan="2" align="center" valign="top" nowrap>
						Date Created
					</th>
					
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Substance Name:
					</td>
					<td align="left">
						<input type="text" name="SubstanceName" size="43">
					</td>
					<td align="right" valign="top" nowrap>
						From Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "fromDate:form1:" , "DATE_PICKER:TEXT", "11")%>
					</td>
				</tr>
				<tr>
					<td align="right" valign="top" nowrap>
						Container ID(Internal):
					</td>
					<td valign="top">
						<input type="text" name="containerID" value="" size="8">
					</td>
					<td align="right" valign="top" nowrap>
						To Date:
					</td>
					<td valign="top">
						<%call ShowInputField("", "", "toDate:form1:" , "DATE_PICKER:TEXT", "11")%>
					</td>
				</tr>
					<tr>
	
					<td align="right" valign="top" nowrap>
						PO Number	
					</td>
					<td valign="top">
					<input type="text" name="PONumber" id="PONumber" onchange="" value="<%=Request("PONumber")%>" size="20">
					</td>
					</td>
					<td align="right" valign="top" nowrap>
						Barcode
					</td>
					<td valign="top">
						<input type="hidden" name="barcodelist" value="">
						<input type="text" name="barcode" onchange="Validate(); return false;" value="" size="20">
					</td>
	
				</tr>
			</table>
		</td>
		
		
		<td rowspan="3" valign="bottom" align="right">
			<input type="button" value="Reset" onclick="document.URL=<%="'" & request.servervariables("URL") & "?'"%>+'PONumber='+document.getElementById('PONumber').value" id="button1" name="button1">
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
