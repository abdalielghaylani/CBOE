<SCRIPT LANGUAGE=vbscript RUNAT=Server>
If IsEmpty(Request.QueryString("manage_template_mode")) then
	server.transfer "/chemacx/export/export_hits_dialog.asp"
Else
	server.transfer "/chemacx/export/export_hits_templates_dialog.asp"
End if



</SCRIPT>