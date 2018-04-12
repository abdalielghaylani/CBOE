<script language="JavaScript">
	if (window.name == "mainFrame") top.main.location.href = this.document.location.href;
</script> 
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
  <frame name="mainFrame" src="ResultsTree.asp?<%=Qstr%>">
  <frameset rows="230,*">
	<frame name="ListFrame" src="BuildList.asp?<%=Qstr%>" MARGINWIDTH="0" MARGINHEIGHT="0">
	<frame name="TabFrame" src="BuildTabs.asp?<%=Qstr%>" MARGINWIDTH="0" MARGINHEIGHT="0">
  </frameset>
  <noframes>
  <body>

  <p>This page uses frames, but your browser doesn't support them.</p>

  </body>
  </noframes>
</frameset>

</html>
