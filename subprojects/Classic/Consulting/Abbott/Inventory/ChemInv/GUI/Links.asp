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
LinkType = Request("LinkType")

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
		if (document.form1.URLhrefInput){
			if (!document.form1.URLCurr) {
				if (document.form1.URLhrefInput.value.length == 0) {
					errmsg = errmsg + "- URL is required.\r";
					bWriteError = true;
				}else{
					document.form1.URLhref.value = document.form1.URLhrefInput.value;
				}
			}else{
				if (document.form1.URLhrefInput.value.length == 0){
					document.form1.URLhref.value = document.form1.URLCurr.value;
				}else{
					document.form1.URLhref.value = document.form1.URLhrefInput.value;
				}
			}
			if (document.form1.URLID.value.length == 0) {
				URLPath = document.form1.URLhref.value;
				/*
				alert(URLPath.indexOf('http://'));
				if (URLPath.indexOf('mailto:') == -1 && URLPath.indexOf('https://') == -1 && URLPath.indexOf('http://') == -1 && URLPath.indexOf('ftp://') == -1 && URLPath.indexOf('\\\\') == -1) {
					errmsg = errmsg + "- The URL is not valid.\rPlease use a valid web ( http://url/ or https://url/), ftp ( ftp://url/ ), email ( mailto: ) or server path protocal ( \\servername\filename\ ).\r";
					bWriteError = true;
				}
				*/
				if ((URLPath.indexOf('\\\\') == -1 && URLPath.indexOf(':\\') == -1) || (URLPath.indexOf('http') >= 0)) {
					errmsg = errmsg + "- The file path is not a valid server path ( \\\\servername\\filename\\ ).\r";
					bWriteError = true;
				}
			}
		}
		if (document.form1.Link_text){
			if (document.form1.Link_text.value.length == 0) {
				errmsg = errmsg + "- Link Text is required.\r";
				bWriteError = true;
			}
		}	
		//bWriteError = true;	
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
<body onload="if (!document.form1.URLhrefInput.disabled) document.form1.URLhrefInput.focus()">
<center>
<form name="form1" action="Links_action.asp?<%=formAction%>" method="POST">
<input TYPE="Hidden" Name="URLID" value="<%=URLID%>">
<input Name="Table_Name" TYPE="hidden" VALUE="<%=Table_Name%>">
<input Name="FK_Name" TYPE="hidden" VALUE="<%=FK_Name%>">
<input Name="FK_Value" TYPE="hidden" VALUE="<%=FK_value%>">
<input name="URLhref" type="hidden" value />
<input name="LinkType" type="hidden" value="<%=LinkType%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	<!---	<tr>		<td align="right" nowrap>			<span class="required">Table Name:</span>		</td>		<td>			<input tabIndex="1" Name="Table_Name" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=Table_Name%>" <%=disabled%>>		</td>	</tr>	<tr>		<td align="right" nowrap>			<span class="required">FK Name:</span>		</td>		<td>			<input tabIndex="1" Name="FK_Name" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=FK_Name%>" <%=disabled%>>		</td>	</tr>	<tr>		<td align="right" nowrap>			<span class="required">FK Value:</span>		</td>		<td>			<input tabIndex="1" Name="FK_Value" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=FK_value%>" <%=disabled%>>		</td>	</tr>	-->
	<!--	<tr>		<td align="right" valign="top" nowrap>			<span class="required">URL:</span>		</td>		<td>			<input tabIndex="1" Name="URLhref" TYPE="text" SIZE="60" Maxlength="255" VALUE="<%=URLhref%>" <%=disabled%>><br />			<font size="1">Please use a valid web ( http://url/ or https://url/), ftp ( ftp://url/ ), email ( mailto: ) or server path protocal ( \\servername\filename\ ).</font>		</td>	</tr>	-->
<% if URLID <> "" and Lcase(Request("action")) <> "delete" then %>
	<tr>
		<td align="right" valign="top" nowrap>
			<span class="required">Current URL:</span>
		</td>
		<td>
			<input Name="URLCurr" TYPE="text" SIZE="50" Maxlength="255" VALUE="<%=URLhref%>" style="background-color:#d3d3d3;" readonly>
		</td>
	</tr>

<% end if %>
	<tr>
		<td align="right" valign="top" nowrap>
			<% if URLID = "" then %>
				<span class="required">URL:</span>
			<% else %>
				URL:
			<% end if %>
		</td>
		<td>
<% if Lcase(Request("action")) = "delete" then %>
			<input Name="URLhrefInput" TYPE="text" SIZE="50" Maxlength="255" VALUE="<%=URLhref%>" <%=disabled%>><br />
<% else %>
			<input Name="URLhrefInput" TYPE="FILE" SIZE="50" Maxlength="255" VALUE <%=disabled%>><br />
			<font size="1">Please use a valid server path.</font>
<% end if %>
		</td>
	</tr>
	<!--	<tr>		<td align="right" nowrap>			<span class="required">URL Type:</span>		</td>		<td>			<input tabIndex="1" Name="URLType" TYPE="tetx" SIZE="30" Maxlength="255" VALUE="<%=URLType%>" <%=disabled%>>		</td>	</tr>	-->
	<tr>
		<td align="right" nowrap>
			<span class="required">Link Text:</span>
		</td>
		<td>
			<input tabIndex="1" Name="Link_text" TYPE="text" SIZE="30" Maxlength="255" VALUE="<%=Link_text%>" <%=disabled%>>
		</td>
	</tr>
	<% if LinkType = "Batch" then %>
	<tr>
		<%=ShowPickList("<span class=""required"">Document Type:</span>", "URLType", URLType,"Select enum_value as Value, enum_value as DisplayText from inv_enumeration where eset_id_fk = 9 order by enum_value")%>
	</tr>
	<% else %>
	<tr>
		<td align="right" nowrap>Image Source:</td>
		<td><input tabIndex="1" Name="Image_source" TYPE="text" SIZE="30" Maxlength="255" VALUE="<%=Image_source%>" <%=disabled%>></td>
	</tr>
	<% end if %>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="ValidateSynonyms(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>	
</form>
</center>
</body>
</html>
