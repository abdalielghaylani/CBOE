<%@ Language=VBScript %>

<html>
<head>
<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: verdana}
</style>
<title><%=Application("appTitle")%> -- Export to File</title>
<SCRIPT LANGUAGE="JavaScript" SRC="/cheminv/utils.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
</head>
<body>

<table border="0" cellspacing="0" cellpadding="2" width="100%" align="center" valign="middle">
	<tr>
		<td colspan="2" align="center"> 
			<BR><BR><BR><BR><BR><BR><BR><BR><BR><BR>
			<span  class="GuiFeedback">Click to close.</span>
		</td>
	</tr>	
	<tr>
		<td colspan="2" align="center"> 
			<a HREF="#" onclick="parent.window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
</body>
</html>
