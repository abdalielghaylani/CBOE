<%
Qstr = Request.QueryString
%>
<html>
<head>
<title>New Page 2</title>
<meta name="GENERATOR" content="Microsoft FrontPage 4.0">
<meta name="ProgId" content="FrontPage.Editor.Document">
</head>

<frameset cols="250,*">
  <frame name="TreeFrame" target="ListFrame" src="ResultsTree.asp?<%=Qstr%>">
  <frameset rows="200,*">
	<frame name="ListFrame" src="BuildList.asp?<%=Qstr%>">
	<frame name="TabFrame" src="BuildTabs.asp?<%=Qstr%>">
  </frameset>
  <noframes>
  <body>

  <p>This page uses frames, but your browser doesn't support them.</p>

  </body>
  </noframes>
</frameset>

</html>
