<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved.%>

<%'SYAN modified 12/10/2003
'This is the UI for structure editing.

'This page does three things:
'1. UI
'2. When loaded by a post action fired in edit_structure.asp, take the base64 string, 
'	decrypt it and populate it in the control.

'3. On clicking OK, output base64 and post back to edit_structure.asp.
%>

<!--#include file="display_func_vbs.asp"-->

<html>

<head>

<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>
<script language="JavaScript">cd_includeWrapperFile("/cfserverasp/source/")</script>

<title>edit structure</title>

<%
dbkey = Application("appkey")
initial_CDP_file = Application("TempFileDirectoryHTTP" & dbkey) & "mt.cdx"

theData = Unescape(Request("structValue"))

'if base64 is from Net plugin we need do decode and re-encode so that it can be used as inline dataurl
theData = DecryptBase64cdx(dbkey, theData) 'this function is in display_func_vbs.asp
theData = escape(theData)
%>

<script language="javascript">
	okhelpstr = "update main window with structure"
	cancelhelpstr = "close window without updating main window with structure"
	clearhelpstr ="clear drawing area"
	
	theOpener = opener
	
	//timeout allows the applet to load otherwise you end up getting messages
	//indicating that the plugin has no properties
	window.onerror = null
	window.setTimeout("showVal()", 200)
	window.focus()
	
	//populate the control
	function showVal(){
		var t = unescape('<%=theData%>');
		if (theOpener.structure_transfer.length != 0 ){
			cd_putData("cdpstructure", "chemical/x-cdx", unescape('<%=theData%>'))
		}	
	}
	
	function clearStruc(){
		cd_clear("cdpstructure")
	}
	
	function getButtons() {
		outputval = ""
		buttonGifPath = '/cfserverasp/source/graphics/navbuttons/'; 
		cancelbutton = buttonGifPath + "Cancel.gif"
		okbutton = buttonGifPath + "ok.gif"
		clearbutton = buttonGifPath + "Clear_btn.gif"
		
		outputval = '<A HREF = "javascript:this.document.forms[&quot;'+ 'strucEdit' + '&quot;].elements[&quot;' + 'structVal' + '&quot;].value=cd_getData(&quot;' + 'cdpstructure' + '&quot;, &quot;' + 'chemical/x-cdx' + '&quot);this.document.forms[&quot;'+ 'strucEdit' + '&quot;].submit();" onMouseOver="window.status=&quot;' + okhelpstr + '&quot;; return true;"><IMG SRC="' +  okbutton + '" BORDER="0"></A>'
		outputval = outputval + '<A HREF = "javascript:clearStruc();" onMouseOver="window.status=&quot;' + clearhelpstr + '&quot;; return true;"><IMG SRC="' +  clearbutton + '" BORDER="0"></A>'
		outputval = outputval + '<A HREF = "javascript:window.close();" onMouseOver="window.status=&quot;' + cancelhelpstr + '&quot;; return true;"><IMG SRC="' +  cancelbutton + '" BORDER="0"></A>'
		document.write (outputval)
	}

</script>

</head>
<body>

<form name="strucEdit" method="post" action="edit_structure.asp">
	<input type="hidden" name="structVal" value="">
	<input type="hidden" name="postBack" value="true">
	
	<script language = "javascript">
		//this actually should be static
		getButtons();
	</script>

	<table border="1">
		<tr><td>
			<script language = "javascript">
				var embedString = "<embed name='cdpstructure' src='<%=initial_CDP_file%>' align='baseline' border='0' width='720' height='500' id='1' type='chemical/x-cdx' viewonly='false'>";
				cd_insertObjectStr(embedString);
			</script>
		</td></tr>
	</table>
</form>

</body>

</html>  
