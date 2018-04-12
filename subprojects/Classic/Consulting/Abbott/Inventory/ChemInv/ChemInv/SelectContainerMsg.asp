<%@ Language=VBScript %>
<%
entity = Request.QueryString("entity")
if entity = "" then entity = "container"

%>
<HTML>
<HEAD>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<TITLE></TITLE>
</HEAD>
<BODY>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE NOWRAP>
			<span class="GuiFeedback">
			<%
			if entity = "none" then
				Response.Write "There are no containers to select."
			else
				Response.Write	"Select a " & entity & " from the top frame."
			end if
			%>
			</span>
		</TD>
	</TR>
</TABLE>
</BODY>
</HTML>


