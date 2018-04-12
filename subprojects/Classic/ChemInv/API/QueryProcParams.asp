<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
ProcName = Request.QueryString("ProcName")
Dim Conn
Dim Cmd
GetInvConnection()
Set Cmd = Server.CreateObject("ADODB.Command")
Set Cmd.ActiveConnection = Conn
Cmd.CommandText = ProcName
Cmd.CommandType = adCmdStoredProc
Cmd.Parameters.Refresh
%>


<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
</HEAD>
<BODY>
<% 
if IsEmpty(ProcName) then 
	Response.Write "Usage: QueryProcParams.asp?ProcName=Text[&ShowCode=Boolean]"
	Response.End
End if
%>

<center>
<B><%= UCASE(ProcName) %><B>
<table border=1>
	<tr>
		<th> 
			Parameter Name
		</th>
		<th>
			Ado DataType
		</th>
		<th>
			Parameter creation code
		</th>
	</tr>
<%
Cmd.Parameters.Append Cmd.CreateParameter("Return_Value", adNumeric, adParamReturnValue, , NewLocationID)
lf = Chr(13)
For each p in Cmd.Parameters
	Response.Write "<TR><TD>" & p.Name & "</td>"
	Response.Write "<td>" & p.Type & "</td>"
	Response.Write "<TD>Cmd.Parameters.Append Cmd.CreateParameter(""" & p.Name & """," & p.Type & ", " & p.Direction & ", " & p.Size & ", #)" & "<BR>" & "Cmd.Parameters(""" & p.Name & """).NumericScale = " & p.NumericScale & "<BR>" & "Cmd.Parameters(""" & p.Name & """).Precision = " & p.Precision & "</td></tr>"
	codestr = codestr & "Cmd.Parameters.Append Cmd.CreateParameter(""" & p.Name & """," & p.Type & ", " & p.Direction & ", " & p.Size & ", #)" & lf & "Cmd.Parameters(""" & p.Name & """).NumericScale = " & p.NumericScale & lf &  "Cmd.Parameters(""" & p.Name & """).Precision = " & p.Precision & lf
Next
Response.Write "</table></center>"
if CBool(Request.QueryString("ShowCode")) then
	Response.Write "<PRE>" & codestr  & "</PRE>"
End if
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
%>

</BODY>
</HTML>

