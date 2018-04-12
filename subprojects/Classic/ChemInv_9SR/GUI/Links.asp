<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
URLID = Request("URLID")
FK_value = Request("FK_value")
FK_name = Request("FK_name")
Table_name = Request("Table_name")
URLType = Request("URLType")
URLhref = Request("URLhref")
Image_source = Request("Image_source")
Link_text = Request("Link_text")

isDelete = false
If URLID = "" then
	Caption = "Add a Link"
	formAction = "action=create"
Else
	if Lcase(Request("action")) = "delete" then
		Caption = "Are you sure you want to delete this link?"
		formAction = "action=delete"
		isDelete = true
		disabled = " onfocus=""blur()"" "
	Else
		Caption = "Edit this link"
		formAction = "action=edit"
		disbled = ""
	End if
End if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create/Edit/Delete an Inventory Link</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--
	window.focus();
	function ValidateSynonyms(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r\r";
		
		if (document.form1.URLhref){
			if (document.form1.URLhref.value.length == 0) {
				errmsg = errmsg + "- URL is required.\r";
				bWriteError = true;
			}
			if (document.form1.URLID.value.length == 0) {
				URLPath = document.form1.URLhref.value;
				if (URLPath.indexOf('mailto:') == -1 && URLPath.indexOf('https://') == -1 && URLPath.indexOf('http://') == -1 && URLPath.indexOf('ftp://') == -1 && URLPath.indexOf('\\\\') == -1) {
					errmsg = errmsg + "- The URL is not valid.\rPlease use a valid web ( http://url/ or https://url/), ftp ( ftp://url/ ), email ( mailto: ) or server path protocol ( \\\\servername\\filename\\ ).\r";
					bWriteError = true;
				}
			}
		}
		/*
		if (document.form1.URLType){
			if (document.form1.URLType.value.length == 0) {
				errmsg = errmsg + "- URL Type is required.\r";
				bWriteError = true;
			}
		}
		*/
		if (document.form1.Link_text){
			if (document.form1.Link_text.value.length == 0) {
				errmsg = errmsg + "- Link Text is required.\r";
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
//-->
</script>
</head>
<body onload="if (!document.form1.URLhref.disabled) document.form1.URLhref.focus()">
<center>
<form name="form1" action="Links_action.asp?<%=formAction%>" method="POST">
<input TYPE="Hidden" Name="URLID" value="<%=URLID%>">
<input Name="Table_Name" TYPE="hidden" VALUE="<%=Table_Name%>">
<input Name="FK_Name" TYPE="hidden" VALUE="<%=FK_Name%>">
<input Name="FK_Value" TYPE="hidden" VALUE="<%=FK_value%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	<!---	<tr>		<td align="right" nowrap>			<span class="required">Table Name:</span>		</td>		<td>			<input tabIndex="1" Name="Table_Name" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=Table_Name%>" <%=disabled%>>		</td>	</tr>	<tr>		<td align="right" nowrap>			<span class="required">FK Name:</span>		</td>		<td>			<input tabIndex="1" Name="FK_Name" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=FK_Name%>" <%=disabled%>>		</td>	</tr>	<tr>		<td align="right" nowrap>			<span class="required">FK Value:</span>		</td>		<td>			<input tabIndex="1" Name="FK_Value" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=FK_value%>" <%=disabled%>>		</td>	</tr>	-->
	<tr>
		<td align="right" valign="top" nowrap>
			<span class="required">URL:</span>
		</td>
		<td>
			<input tabIndex="1" Name="URLhref" TYPE="tetx" SIZE="60" Maxlength="255" VALUE="<%=URLhref%>" <%=disabled%>><br />
			<font size="1">Please use a valid web ( http://url/ or https://url/), ftp ( ftp://url/ ), email ( mailto: ) or server path protocal ( \\servername\filename\ ).</font>
		</td>
	</tr>
	<!--
	<tr>
		<td align="right" nowrap>
			<span class="required">URL Type:</span>
		</td>
		<td>
			<input tabIndex="1" Name="URLType" TYPE="tetx" SIZE="30" Maxlength="255" VALUE="<%=URLType%>" <%=disabled%>>
		</td>
	</tr>
	-->
	<tr>
		<td align="right" nowrap>
			<span class="required">Link Text:</span>
		</td>
		<td>
			<input tabIndex="1" Name="Link_text" TYPE="tetx" SIZE="30" Maxlength="255" VALUE="<%=Link_text%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Image Source:
		</td>
		<td>
			<input tabIndex="1" Name="Image_source" TYPE="tetx" SIZE="30" Maxlength="255" VALUE="<%=Image_source%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateSynonyms(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</center>
</body>
</html>
