<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
action = Request("action")
%>
<HTML>
<HEAD>
<TITLE>Set CFW Trace Level</TITLE>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
</HEAD>
<BODY>
<center>
<%
if action = "set" then

	webserver = Request("webserver")
	app = Request("application")
	scope = Request("scope")
	level = Request("level")
	
	URL = "http://" & webserver & "/" & app & "/SetCFWTraceLevel.asp"
	QS = "level=" & level & "&scope=" & scope
	URL = URL  & "?" & QS
	'Set oXMLHTTP = Server.CreateObject("Msxml2.ServerXMLHTTP.4.0")
	'oXMLHTTP.open "POST", URL, False
	'oXMLHTTP.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	'oXMLHTTP.setRequestHeader "User-Agent", "ChemInv"
	'oXMLHTTP.send QS
	'StatusCode = oXMLHTTP.status
	'If StatusCode <> "200" then
	'	httpResponse = oXMLHTTP.responseText
		'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
	'Else
	'	httpResponse = oXMLHTTP.responseText
	'End If

	'out2 = httpResponse
end if
%>
<form name="form1" action="#" method="post">
<input type="hidden" name="action" value="set">
<table border="0">
	<%if action="set" then%>
	<tr height="25">
		<td align="right" class="guiUtils">URL:</td>
		<td class="guiUtils"><a href="<%=URL%>"><%=URL%></a></td>
	</tr>	
	<%end if%>
	<tr height="25">
		<td align="right" nowrap>Webserver Name:</td>
		<td>
			<INPUT type="text" name="webserver">
		</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap>Application:</td>
		<td>
			<SELECT NAME="application">
				<OPTION VALUE="cheminv">Inventory Manager</OPTION>
			</SELECT>
		</td>
	</tr>	
	<tr height="25">
		<td align="right" nowrap>Trace Level:</td>
		<td>
			<SELECT NAME="level">
				<OPTION VALUE="0">0</OPTION>
				<OPTION VALUE="5">5</OPTION>
				<OPTION VALUE="10">10</OPTION>
				<OPTION VALUE="15">15</OPTION>
				<OPTION VALUE="20">20</OPTION>
			</SELECT>
		</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap>Scope:</td>
		<td>
			<SELECT NAME="scope">
				<OPTION VALUE="session">Session</OPTION>
				<OPTION VALUE="application">Application</OPTION>
			</SELECT>
		</td>
	</tr>	
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="form1.submit(); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
<P>&nbsp;</P>
</center>
</BODY>
</HTML>
