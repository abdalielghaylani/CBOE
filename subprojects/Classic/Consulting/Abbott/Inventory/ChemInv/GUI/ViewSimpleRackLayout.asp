<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<%
Dim Conn
Dim Cmd
Dim RS

dbkey = "ChemInv"
LocationID = Request("LocationID")
RackID = Request("RackID")
ContainerID = Request("ContainerID")
RackPath = Request("RackPath")
caption = RackPath
Summary = Request("Summary")

if ContainerID <> "" then
	RackDisplayMsg = "Selected Rack Position"
elseif caption <> "" then
	RackDisplayMsg = caption
elseif Summary <> "" then
	RackDisplayMsg = Summary
else
	RackDisplayMsg = ""
end if

Call GetInvConnection()
Response.Write "<div align=""left""><img src=""/cheminv/graphics/cheminventory_banner.gif"" border=""0""></div>"
Response.Write("<div align=""center""><table bgcolor=""#d3d3d3"" width=""100%""><tr><td align=""center""><span class=""GuiFeedback"">" & caption & "</span></td></tr></table><br />")

%>
<div style="margin:10px; text-align: left;">
<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('/cheminv/cheminv/columnPicker2.asp?ArrayID=1&amp;showRackSummary=true', 'CCDiag', 4); return false">Column Chooser</a>
</div>
<%
Response.write(DisplaySimpleRack(RackID,ContainerID,RackDisplayMsg))
Response.Write("<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>")
Response.Write("&nbsp;<a href=""javascript:window.close()""><img src=""/cheminv/graphics/sq_btn/close_dialog_btn.gif"" border=""0""></a>")
Response.Write("</div>")

%>

<html>
<head>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="javascript" src="/cheminv/choosecss.js"></script>
<script language="javascript" src="/cheminv/gui/refreshgui.js"></script>


</head>
<body>

</body>
</html>
