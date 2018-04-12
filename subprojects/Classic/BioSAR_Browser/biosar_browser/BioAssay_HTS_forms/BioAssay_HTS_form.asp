<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%
'output_type = request("output_type")
'if output_type = "" then 
	output_type = "FRAMES_CHECK_TABLES"
'end if
Qstr = Request.QueryString
'if not instr(Qstr, "output_type=" & output_type)> 0 then
	'Qstr = Qstr & "&output_type=" & output_type
'end if
result_form = "bioassay_hts_form"
%>
<html>
<head>
<title>Frameset</title>
</head>
<%if UCase(output_type) = "FRAMES_CHECK_TABLES" then
'response.Write result_form & "_left.asp?" & Qstr %>
	<frameset FRAMEBORDER="YES" FRAMESPACING="0" cols="400,*">

		<frame name="mainFrame" BORDER="1"  MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=result_form%>_left.asp?<%=Qstr%>" scrolling="AUTO">
		<frame name="rightFrame" BORDER="1" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=result_form%>_right.asp?<%=Qstr%>"   scrolling="AUTO">
	</frameset>
		<noframes>
	  <p>This page uses frames, but your browser doesn't support them.</p>
	  </body>
	  </noframes>
</frameset>

<%else%>

	<frameset FRAMEBORDER="NO" FRAMESPACING="0" cols="600,*">

		<frame name="mainFrame" BORDER="1"  MARGINWIDTH="0" MARGINHEIGHT="0" SRC="<%=result_form%>_left.asp?<%=Qstr%>" scrolling="AUTO">
	</frameset>
		<noframes>
	  <p>This page uses frames, but your browser doesn't support them.</p>
	  </body>
	  </noframes>
</frameset>


<%end if%>
</html>