<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
CompoundID = Request("CompoundID")
SynonymID = Request("SynonymID")
isDelete = false
If SynonymID = "" then
	Caption = "Add a Synonym"
	formAction = "action=create"
Else
	SubstanceName = Request("SubstanceName")
	if Lcase(Request("action")) = "delete" then
		Caption = "Are you sure you want to delete this synonym?"
		formAction = "action=delete"
		isDelete = true
		disabled = "disabled"
	Else
		Caption = "Edit this Synonym"
		formAction = "action=edit"
		disbled = ""
	End if
End if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create a New Synonym</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--
	window.focus();
	function ValidateSynonyms(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Quantity reserved is required
		if (document.form1.SubstanceName){
			if (document.form1.SubstanceName.value.length == 0) {
				errmsg = errmsg + "- Substance Name is required.\r";
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
<body onload="if (!document.form1.SubstanceName.disabled) document.form1.SubstanceName.focus()">
<center>
<form name="form1" xaction="echo.asp" action="Synonym_action.asp?<%=formAction%>" method="POST">
<input TYPE="Hidden" Name="SynonymID" value="<%=SynonymID%>">
<input TYPE="Hidden" Name="CompoundID" value="<%=CompoundID%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Synonym:
		</td>
		<td>
			<input tabIndex="1" Name="SubstanceName" TYPE="tetx" SIZE="60" Maxlength="255" VALUE="<%=SubstanceName%>" <%=disabled%>>
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
