<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
'FormData = Request.Form & Credentials
QueryString = ""
ValuePairs = ""

numericFields = Request("numericFields")
textFields = Request("textFields")
dateFields = Request("dateFields")
For each field in Request.Form
	if InStr("TableName,dbTableName,numericFields,textFields,dateFields,pkColumnName,pkID", field) = 0 then
		if len(Request(field)) = 0  then
			ValuePairs = ValuePairs & field & "%3Dnull::"
		else
			value = Request(field)
			if instr(numericFields,field) > 0 then
				ValuePairs = ValuePairs & field & "%3D" & value & "::"
			elseif instr(textFields,field) > 0 then
				ValuePairs = ValuePairs & field & "%3D'" & Server.URLEncode(value) & "'::"
			elseif instr(dateFields,field) > 0 then
				ValuePairs = ValuePairs & field & "%3D TO_DATE('" & value & "','MM-DD-YYYY')::"
			end if
		end if
		'Response.Write field
		'ValuePairs = "test"
	end if
next
tableName = Request("dbTableName")
pkIDs = Request("pkIDs")
QueryString = QueryString & "TableName=" & tableName
QueryString = QueryString & "&pkColumnName=" & Request("pkColumnName")
select case lcase(tableName)
	'string pk
	case "inv_owners"
		arrPK = split(pkIDs,",")
		pkList = ""
		for i = 0 to ubound(arrPk)
			pkList = pkList & "'" & arrPK(i) & "',"
		next
		pkList = left(pkList,(len(pkList)-1))
	'numeric pk
	case else
		pkList = pkIDs
end select
QueryString = QueryString & "&pkIDs=" & pkList
QueryString = QueryString & "&ValuePairs=" & Left(ValuePairs, len(ValuePairs) - 2)
'Response.Write "<BR>" & QueryString
'Response.End
QueryString = QueryString & Credentials

'Update the containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdateTable.asp", "ChemInv", QueryString)
out = httpResponse

'Response.Write out
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update a Table</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(out) then
				if Clng(out) = 0 then
					theAction = "Success"
				Else
					theAction = "WriteAPIError"
				End if
			Elseif InStr(Left(out,18),"|") then
				containerList = out
				out = left(out,InStr(out,"|")-1)
				theAction = "SelectContainer"	
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "Success"
					Response.Write "<script language=javascript>"
					Response.Write	"if (opener){opener.location = 'manageTables.asp?TableName=" & Request("TableName") & "'; opener.focus();}"
					Response.Write "</script>"
					Response.write "<Script language=javascript>window.close();</script>"		
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>Table could not be updated.</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
