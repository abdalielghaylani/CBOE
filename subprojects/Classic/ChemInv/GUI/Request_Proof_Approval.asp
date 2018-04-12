<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<html>
<head>
<title><%=Application("appTitle")%> -- Request Samples from an Inventory Container</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript">
	function fileFormatValid() {
		var filePath = document.submitForm.file.value;
		var fileValid = true;
		var fileFormat;
		fileFormat = filePath.substr(filePath.length - 4 , 4);
		fileFormat2 = filePath.substr(filePath.length - 5 , 5);
		if (fileFormat.toLowerCase() != '.doc' 
			&& fileFormat.toLowerCase() != '.txt'
			&& fileFormat.toLowerCase() != '.xls'
			&& fileFormat.toLowerCase() != '.pdf'
			&& fileFormat.toLowerCase() != '.ppt'
			&& fileFormat2.toLowerCase() != '.docx'
			&& fileFormat2.toLowerCase() != '.xlsx'
			&& fileFormat2.toLowerCase() != '.pptx') {
			alert('Supported file formats are: *.doc *.docx *.txt *.xls *.xlsx *.ppt *pptx *.pdf');
			fileValid = false;
		}
		return fileValid;
	}
	
	function submitDoc() {
	      var filePath = document.submitForm.file.value;
	      var fileName;
	      fileName=filePath.substr(filePath.lastIndexOf("\\",filePath)+1,filePath.length);
	      window.opener.form1.proof_file.value =filePath.substr(filePath.lastIndexOf("\\",filePath)+1,filePath.length); 
	      window.opener.form1.fileDeleted.value = "0";
	      window.opener.form1.proof_approval.value = document.submitForm.file.value;
	      document.submitForm.action="../API/uploadDocs.asp?toSubmit=true";
		  document.submitForm.submit();
	}
</script>

</head>
<body">
<center>
<form method="post" name="submitForm" ENCTYPE="multipart/form-data">
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">Attach Proof of Approval</span><br/><br/>
		</td>
	</tr>
	
    <tr>
	    <td>
	        <span>Document: </span>
	    </td>	
    	
	    <td align="right">
		    <input type="file" size="44" name="file" accept="image/jpeg">
	    </td>
    					
	    <td>
		    <input type="button" name="submitIt" value="Submit Now" onclick="javascript:if (fileFormatValid() == true) {submitDoc();}">
	    </td>
    </tr>
	<tr>
	    <td><br/></td>	    
	</tr>
</table>	
</form>
</center>
</body>
</html>
