<%@ Language=VBScript %>


<!-- #INCLUDE FILE="utilities.asp" -->
<!-- #INCLUDE FILE="sql_statements.asp" -->

<%
	Select Case Request("FunctionToExecute")
		Case "DeleteFromReports"
			DeleteFromReports Request("formgroup_id")
	End Select
%>