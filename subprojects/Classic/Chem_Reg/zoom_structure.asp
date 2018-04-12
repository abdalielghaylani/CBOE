<%@ LANGUAGE=VBScript  %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<script language="Javascript">
var cd_plugin_threshold="<%=Application("CD_PLUGIN_THRESHOLD")%>"
</script>
<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>
<script language="JavaScript">  cd_includeWrapperFile("/cfserverasp/source/")</script>

<title>zoom structure</title>
<%dbkey = Request.QueryString("dbname")
initial_CDP_file = Application("TempFileDirectoryHTTP" & dbkey) & "mt.cdx"%>
<script language="javascript">
okhelpstr = "update main window with structure"
cancelhelpstr = "close window without updating main window with structure"
closehelpstr ="close window"
theOpener = opener
fullfieldname = "<%=request.querystring("fieldname")%>"
//timeout allows the applet to load otherwise you end up getting messages
//indicating that the plugin has no properties
	window.onerror = null
	window.setTimeout("showVal()",200)
	window.focus()

function showVal(){
	if (theOpener.structure_transfer.length!=0 ){
		cd_putData("cdpstructure", "chemical/x-cdx", theOpener.structure_transfer)
	}
}
	
function getButtons() {
	outputval = ""
	var buttonGifPath = theOpener.button_gif_path 
	closebutton = buttonGifPath + "close_btn.gif"
	outputval = '<A HREF = "javascript:window.close();" onMouseOver="window.status=&quot;' + closehelpstr + '&quot;; return true;"><IMG SRC="' +  closebutton + '" BORDER="0"></A>'
	document.write (outputval)

}
</script>
</head>

<body>

<form name="strucEdit" method="post" action>
  <script language="javascript">getButtons()</script>
<table border = "1"><tr><td>
	<script language = "javascript">
		var embedString = "<embed name='cdpstructure' src='<%=initial_CDP_file%>' align='baseline' border='0' width='720' height='500' id='1' type='chemical/x-cdx' shrinktofit='false' viewonly='true'>"
		cd_insertObjectStr(embedString)
	</script>
</td></tr></table>
</form>
</body>
</html>
