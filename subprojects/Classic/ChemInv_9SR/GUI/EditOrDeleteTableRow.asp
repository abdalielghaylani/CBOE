<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%

Dim Conn
action = Request("action")
TableName = Request("TableName")
IDColumnName = Request("IDColumnName")
dbTableName = Request("dbTableName")
pkColumnName = Request("pkColumnName")

pkIDs = ""
'-- support multiple primary key columns
if instr(pkColumnName,",") > 0 then 
	arrPK = split(pkColumnName,",")
	for i = 0 to ubound(arrPK)
		currID = request(arrPK(i))
		pkIDs = pkIDs & left(currID,instr(currID,":")-1) & ":"
	next
	pkIDs = left(pkIDs, len(pkIDs)-1)
else
	currID = Request(pkColumnName)
	pkIDs = left(currID,instr(currID,":")-1)
end if
if action = "edit" then
	bEnabled = true
	actionText = "Edit this information."
elseif action = "create" then
	bEnabled = true
	actionText = "Enter information."
else
	bEnabled = false
	actionText = "Delete this row of information?"
end if

'Response.Write pkColumnName & ":" & pkIDs & ":" & Request("pkColumnName") & "<BR>"
'for each key in Request.QueryString
'	Response.Write key & "=" & request(key) & "<BR>"
'next

numericFields = " "
textFields = " "
dateFields = " "


fieldsNotShown = "RID,CREATOR,TIMESTAMP,TableName,dbTableName,action,pkColumnName,IDColumnName," & IDColumnName
if instr("inv_owners,inv_project_job_info",lcase(dbTableName)) = 0 then fieldsNotShown = fieldsNotShown & "," & pkColumnName

' build list of numeric, text, and date fields
For each oField in Request.QueryString
	if instr(fieldsNotShown,oField) = 0 then
		qsValue = Request(oField)
		'Response.Write oField & "=" & qsValue & "TEST<BR>"
		colPos = instr(qsValue,":")
		'fieldValue = left(qsValue,colPos - 1)
		fieldType = mid(qsValue,colPos + 1)
		select case lcase(fieldType)
			'numeric
			case "16","2","3","20","17","18","29","21","4","5","6","14","131","11","139","number"
				numericFields = numericFields & oField & ","
			'text
			case "12","8","129","200","201","130","202","203","varchar2","char"
				textFields = textFields & oField & ","
			'date
			case "7","133","date"
				dateFields = dateFields & oField & ","
		end select			
	end if
next

numericFields = trim(left(numericFields,len(numericFields)-1))
textFields = trim(left(textFields,len(textFields)-1))
dateFields = trim(left(dateFields,len(dateFields)-1))

'Response.Write numericFields & "=numericFields<BR>"
'Response.Write textFields & "=textFields<BR>"
'Response.Write dateFields & "=dateFields<BR>"
'Response.End

%>
<html>
<head>
	<title><%=Application("appTitle")%> -- Edit or Delete a Table Row</title>
	<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
	<SCRIPT LANGUAGE="">
	// Validates container attributes
	function ValidateRow(strMode){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		if (strMode.toLowerCase() == "edit" || strMode.toLowerCase() == "create") {
			<%
			arrNumericFields = split(numericFields,",")
			for i = 0 to ubound(arrNumericFields)
				field = arrNumericFields(i)
				Response.Write "if (document.form1." & field & ".value.length > 0) {"
				Response.Write "if (!isNumber(document.form1." & field & ".value)) {"
				Response.Write "errmsg = errmsg + ""- " & field & " must be a number.\r"";"
				Response.Write "bWriteError = true;}}"
			next		

			arrDateFields = split(dateFields,",")
			for i = 0 to ubound(arrDateFields)
				field = arrDateFields(i)
				Response.Write "if (document.form1." & field & ".value.length > 0) {"
				Response.Write "if (!isDate(document.form1." & field & ".value)) {"
				Response.Write "errmsg = errmsg + ""- " & field & " must be in MM/DD/YYYY format.\r"";"
				Response.Write "bWriteError = true;}}"
			next
			%>
		}
		// Report problems
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			if (strMode.toLowerCase() == "edit") {
				document.form1.action = "EditTableRow_action.asp";
				document.form1.submit();			
			}
			else if (strMode.toLowerCase() == "create") {
				document.form1.action = "CreateTableRow_action.asp";
				document.form1.submit();			
			}
			else {
				document.form1.action = "DeleteTableRow_action.asp";
				document.form1.submit();			
			}
		}
	}
	 
	</SCRIPT>	
</head>
<body>

<center>
<form name="form1" method="POST">
<INPUT TYPE="hidden" NAME="TableName" VALUE="<%=TableName%>">
<INPUT TYPE="hidden" NAME="dbTableName" VALUE="<%=dbTableName%>">
<INPUT TYPE="hidden" NAME="pkColumnName" VALUE="<%=pkColumnName%>">
<%
if action="create" then
	
	Response.Write "<INPUT TYPE=""hidden"" NAME=""IDColumnName"" VALUE=""" & IDColumnName & """>"
	if instr(TableName,"(") > 0 then
		IDValue = Request(IDColumnName)
		colPos = instr(IDValue,":")
		fieldValue = left(IDValue,colPos - 1)
		fieldType = mid(IDValue,colPos + 1)
		Response.Write "<INPUT TYPE=""hidden"" NAME=""IDValue"" VALUE=""" & fieldValue & """>"
	end if
end if

%>
<INPUT TYPE="hidden" NAME="pkIDs" VALUE="<%=pkIDs%>">
<INPUT TYPE="hidden" NAME="numericFields" VALUE="<%=numericFields%>">
<INPUT TYPE="hidden" NAME="textFields" VALUE="<%=textFields%>">
<INPUT TYPE="hidden" NAME="dateFields" VALUE="<%=dateFields%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=actionText%></span><br><br>
		</td>
	</tr>
	<%
	Response.Write BuildForm(bEnabled)
	%>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRow('<%=action%>'); return false;"><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21" id=image1 name=image1></a></td>
	</tr>	

</table>	
</form>
</center>
</body>
</html>
<%
Function BuildForm(bEnabled)
	if not bEnabled then
		enableString = "CLASS=""GrayedText"" READONLY"
	else
		enableString = ""
	end if
	theForm = ""
	For each oItem in Request.QueryString
		if instr(fieldsNotShown,oItem) = 0 then
			qsValue = Request(oItem)
			colPos = instr(qsValue,":")
			fieldValue = left(qsValue,colPos - 1)
			fieldType = mid(qsValue,colPos + 1)
			theForm = theForm & "<TR><TD ALIGN=""right"" NOWRAP>" & oItem & ":</TD><TD><INPUT TYPE=""text"" NAME=""" & oItem & """ VALUE=""" & fieldValue & """" & enableString & "></TD></TR>"
		end if
	next
	BuildForm = theForm
end function
%>