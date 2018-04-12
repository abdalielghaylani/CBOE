
<script language = "javascript">
//Copyright 1998-2001 CambridgeSoft Corporation All Rights Reserved%>

function loadHelperFrame(theID){
	top.frames["helper"].location.replace(app_Path + "/helper.asp?dbname=" + "<%=dbkey%>" + "&formgroup=" + formgroup+ "&unique_id=" + theID + "&action=" + "get_chemist" + "&formode=" + formmode.toLowerCase())	
}

function loadHelperFrame2(theID){
	top.frames["helper"].location.replace(app_Path + "/helper.asp?dbname=" + "<%=dbkey%>" + "&formgroup=" + formgroup+ "&unique_id=" + theID + "&action=" + "get_producer"+ "&formode=" +  formmode.toLowerCase())	
}


function checkCompoundType(fullfieldname){
	var theSelectBox = eval('MainWindow.document.forms[0].elements["' + fullfieldname + '"].options[MainWindow.document.forms[0].elements["'+ fullfieldname + '"].selectedIndex]')
	setStrucReq(theSelectBox.value)
}

function setStrucReq(theValue){
	var newstring = ""
	if (theValue == 2){
		reqfieldsarray=required_fields.split(",")
		for(i=0;i<reqfieldsarray.length;i++){
			testcase = reqfieldsarray[i].toLowerCase()
			if(testcase.indexOf(".structure")==-1){
				if(newstring == ""){
					newstring = reqfieldsarray[i]
				}
				else
				{
					newstring = newstring + "," + reqfieldsarray[i]
					
				}
			}
		}
	}

	else{
		newstring = document.forms[0].elements['orig_required_fields'].value
	}
	required_fields = newstring
}
</script>



