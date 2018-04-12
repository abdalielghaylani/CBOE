<SCRIPT LANGUAGE=vbscript RUNAT=Server>
	Response.Redirect "/chemacx/chemacx/chemacx_action.asp?formgroup=base_form_group&dataaction=get_structure&dbname=chemacx&Table=Substance&Field=Structure&DisplayType=" & Request("datatype") & "&StrucID=" & Request("StrucId")
</SCRIPT>
