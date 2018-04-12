<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE FILE = "../source/app_vbs.asp"-->

<%
Dim log
Dim tempid

'stop

tempid = request("tempid")

Set RegConn = getRegConn("reg", "base_form_group")

		
Set logRS = Server.CreateObject("adodb.recordset")
set cmd = server.CreateObject("adodb.command")
	
cmd.commandtype = adCmdText
cmd.ActiveConnection = RegConn
sql = "Select log from temporary_structures where temp_compound_id=?"
cmd.CommandText = sql
cmd.Parameters.Append cmd.CreateParameter("pTempID", 5, 1, 0, tempid) 
	
logRS.Open cmd
cmd.Parameters.delete "pTempID"

log = logRS("log")
log = replace(log, "!!!!!", "<br><br>")

Response.Write log



%>
