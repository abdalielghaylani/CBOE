<%@ Language=VBScript %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SendTo = Application("POST_MARKED_SEND_TO_PAGE")
'"http://DevSite2/cheminv/gui/ImportFromChemReg.asp"
%>
<HTML>
<HEAD>
</HEAD>
<BODY onload="form1.submit()">
	<Form Name="form1" method="POST" action="<%=SendTo%>">
		<input type="Hidden" name="RegIDList" value="<%=Session("MarkedHitsregbase_form_group")%>">
		<input type="Hidden" name="CSUserName" value="<%=Session("UserName" & "reg")%>">
		<input type="Hidden" name="CSUserID" value="<%=Session("UserID" & "reg")%>">
	</Form>
</BODY>
</HTML>
