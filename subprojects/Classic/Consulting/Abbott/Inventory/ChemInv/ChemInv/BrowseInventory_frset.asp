<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim RS
'if Not Session("UserValidated" & "cheminv") = 1 then
'	response.redirect "../login.asp?dbname=cheminv&formgroup=base_form_group&perform_validate=0"
'end if
Qstr = Request.QueryString


GotoNode = Request.QueryString("GotoNode")
ClearNodes = Request.QueryString("ClearNodes")
if ClearNodes = "" then ClearNodes = "1"
SelectContainer = Request.QueryString("SelectContainer")
SelectWell = Request.QueryString("SelectWell")
if Session("DefaultLocation") = "" then Session("DefaultLocation")= GetUserProperty(Session("UserNameCheminv"),"INVDefLoc")
If Session("DefaultLocation")="" OR IsNULL(Session("DefaultLocation"))  then Session("DefaultLocation")= 0	
if ClearNodes = "0" then
	OpenNodes = Session("TreeViewOpenNodes1")
Else
	OpenNodes = ""	
End if


If Len(GotoNode) = 0 then 
	GotoNode = Session("CurrentLocationID")
	If Len(GotoNode) = 0 then GotoNode = Session("DefaultLocation")
End if

If Len(SelectContainer) = 0 then 
	SelectContainer = Session("CurrentContainerID")
	If Len(SelectContainer) = 0 then SelectContainer = 0
End if

'BrowseTreeQS = "ClearNodes=" & ClearNodes & "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" & GotoNode & "&node=0&sNode=" & GotoNode & "&SelectContainer=" & SelectContainer & "&SelectWell=" & SelectWell & OpenNodes & "&Exp=Y#" & GotoNode
BrowseTreeQS = "ClearNodes=" & ClearNodes & "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" & GotoNode & "&sNode=" & GotoNode & "&SelectContainer=" & SelectContainer & "&SelectWell=" & SelectWell & OpenNodes & "&Exp=Y#" & GotoNode

%>
<html>
<head>
<title><%=Application("appTitle")%></title>
<meta name="GENERATOR" content="Microsoft FrontPage 4.0">
<meta name="ProgId" content="FrontPage.Editor.Document">
</head>

<frameset rows="97,*">
	<frameset cols="*,1">
		<frame name="bannerFrame" src="browsebannerFrame.asp" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
		<frame name="HiddenFrame" src="about:blank" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="no">
	</frameset>
	<frameset cols="250,*">
	  <frame name="TreeFrame" target="TreeFrame" src="BrowseTree.asp?<%=BrowseTreeQS%>">
	  <frameset rows="225,*">
		<frame name="ListFrame" src="BuildList.asp?SelectContainerID=<%=SelectContainer%>" MARGINWIDTH="0" MARGINHEIGHT="0">
		<frame name="TabFrame" src="SelectContainerMsg.asp" MARGINWIDTH="0" MARGINHEIGHT="0">
	  </frameset>
</frameset>  
  <body>

  <p>This page uses frames, but your browser doesn't support them.</p>

  </body>
  </noframes>
</frameset>

</html>
