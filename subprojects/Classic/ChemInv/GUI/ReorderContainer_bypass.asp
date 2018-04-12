<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Session("CurrentLocationID")= 1	'OnOrder location, rather than Request("DeliveryLocationID") 
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reorder Container Results</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
	
	function CloseDialog()
	{
	    opener.top.location.href = '/cheminv/cheminv/BrowseInventory_frset.asp?ClearNodes=0';
	    window.close();
	}
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>					
			<SPAN class="GuiFeedback">Container has been reordered</SPAN>
			<br /><br />
            <a href="#HTML" onclick="javascript:CloseDialog();return(false);"><img src="/ChemInv/graphics/sq_btn/ok_dialog_btn.gif" border="0" /></a>						
		</TD>
	</TR>
</TABLE>
</Body>
</html>