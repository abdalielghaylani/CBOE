<SCRIPT LANGUAGE=vbscript RUNAT=Server>
If IsEmpty(Request.QueryString("manage_hitlist_mode")) then
	server.transfer "/cfserverasp/source/manage_queries_dialog.asp"
Else
	server.transfer "/cfserverasp/source/manage_hitlists_dialog.asp"
End if



</SCRIPT>
