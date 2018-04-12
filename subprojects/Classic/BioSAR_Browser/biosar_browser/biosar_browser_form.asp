<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%
dbkey = request("dbname")
formgroup = request("formgroup")
formmode = request("formmode")
if UCase(Request.QueryString("output_type")) ="FRAMETAB" then
	output_type = "FRAMETAB"
elseif 	Session("CurrentMode" & dbkey & formgroup)= "FRAMETAB" and UCase(request.QueryString("output_type")) = "" then
	output_type = "FRAMETAB"
	Session("CurrentMode" & dbkey & formgroup)= "FRAMETAB"
else	
	output_type = "FRAMES"
end if
Qstr = Request.QueryString
%>
<html>
<head>
<title>Frameset</title>
</head>
<%if UCase(output_type) = "FRAMES" then%>
	<frameset FRAMEBORDER="YES" FRAMESPACING="0" cols="400,*">

		<frame name="mainFrame" BORDER="1"  MARGINWIDTH="0" MARGINHEIGHT="0" SRC="/biosar_browser/biosar_browser/biosar_browser_form_left.asp?<%=Qstr%>" scrolling="AUTO">
		<frame name="rightFrame" BORDER="1" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="/biosar_browser/biosar_browser/biosar_browser_form_right.asp?<%=Qstr%>"   scrolling="AUTO">
	</frameset>
		<noframes>
	  <p>This page uses frames, but your browser doesn't support them.</p>
	  </body>
	  </noframes>
</frameset>

<%elseif UCase(output_type) = "FRAMETAB" then%>
	<frameset FRAMEBORDER="YES" FRAMESPACING="0" cols="400,*">
		
		<frame name="mainFrame" BORDER="1"  MARGINWIDTH="0" MARGINHEIGHT="0" SRC="/biosar_browser/biosar_browser/biosar_browser_form_left.asp?<%=Qstr%>" scrolling="AUTO">
		<frame name="rightFrame" BORDER="1" MARGINWIDTH="0" MARGINHEIGHT="0" SRC="/biosar_browser/biosar_browser/biosar_browser_form_righttabs.asp?<%=Qstr%>"   scrolling="AUTO">
	</frameset>
		<noframes>
	  <p>This page uses frames, but your browser doesn't support them.</p>
	  </body>
	  </noframes>
</frameset>

<%else%>

	<frameset FRAMEBORDER="NO" FRAMESPACING="0" cols="600,*">

		<frame name="mainFrame" BORDER="1"  MARGINWIDTH="0" MARGINHEIGHT="0" SRC="/biosar_browser/biosar_browser/biosar_browser_form_left.asp?<%=Qstr%>" scrolling="AUTO">
	</frameset>
		<noframes>
	  <p>This page uses frames, but your browser doesn't support them.</p>
	  </body>
	  </noframes>
</frameset>


<%end if%>
</html>