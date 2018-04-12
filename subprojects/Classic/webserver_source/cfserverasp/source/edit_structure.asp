<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved.%>

<%'SYAN modified 12/10/2003
'This is the middle tier between main window and the structure-edit form window.
'This layer is needed because Net ActiveX control encrypt its base64 output and could not 
'read it's own output without decryption(CSBR-35737).
'Hence the encrypted output Net base64 string needs to be posted to server and decrypted.

'This page does two things:
'1. When triggered open by the main window, it takes the net base64 from main window as a javascript variable,
'	put it in a hidden field of a form, post it to edit_structure_form.asp, 
'	where the base64 gets decrypted and populated to a control.

'2. User edit the structure at edit_structure_form.asp, on clicking OK, the edited structure
'	gets put in a hidden field of a form, post back to this page. This page takes it and decrypt 
'	it to a valid base64, hand it back to main window as a javascript variable, close itself, 
'	and hand the focus back to main window.
%>

<!--#include file="display_func_vbs.asp"-->

<html>
<head>
<script language="javascript">
	function updateMain(b64) {
		fullfieldname = opener.structure_transfer_name
		opener.structure_transfer = b64
		opener.focus()
		opener.doneStrucEdit(fullfieldname)
		window.close()
	}
</script>
</head>

<body>

<%
dbkey = Application("appkey")
structValFromForm = Unescape(request("structVal")) 'this is posted by edit_structure_form.asp%>

<%if request("postBack") <> "true" then 'this page is called by main window%>
	<%'post base64 to edit_structure_form.asp for decrypt and populate%>
	<form name="editStructForm" method="post" action="edit_structure_form.asp">
		<input type="hidden" name="structValue" value="">
	</form>
	
	<script language="javascript">
		var theData = opener.structure_transfer;
		theData = escape(theData);
		this.document.forms["editStructForm"].elements["structValue"].value = theData;
		this.document.forms["editStructForm"].submit();
	</script>
<%else	'this page is called by edit_structure_form.asp%>
	<%'decrypt the base64 and pass back to main%>
	<%structValFromForm = DecryptBase64cdx(dbkey, structValFromForm) 'this function is in display_func_vbs.asp%>
	<%structValFromForm = escape(structValFromForm)%>
	
	<script language="javascript">
		updateMain(unescape('<%=structValFromForm%>'));
	</script>
<%end if%>

</body>
</html>
