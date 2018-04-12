<%@ LANGUAGE=VBScript  %>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
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
	window.setTimeout("showVal()",3000)
	window.focus()

function showVal(){
	document.applets["CDPStructure"].putData(0,theOpener.structure_transfer )
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
<table border="1">
    <tr>
      <td><embed src="<%=initial_CDP_file%>" align="baseline" border="0" width="720"
      height="500" id="1" name="CDX" viewonly="true" type="chemical/x-cdx"> </td>
    </tr>
  </table>
</form>
</body>
<applet code="camsoft.cdp.CDPHelperAppSimple.class" align="baseline" width="10" height="0"
name="CDPStructure" archive="camsoft.jar" id="Applet1">
  <param name="ID" value="1">
</applet>
</html>
