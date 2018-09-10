<script language = "javascript">
//Copyright 1998-2001 CambridgeSoft Corporation All Rights Reserved%>
//PURPOSE OF FILE: TO add custom javascript functions to an applciation
//All form files generated by the wizard have a #INCLUDE for this file. Add the #INCLUDE to form files
//that you might add to the application.

function getButtons_np(){
	var outputval='<table><tr><td nobrake><a href="<%=Application("AppPathHTTP")%>/<%=Application("NPinputForm")%>" target="_top"><img src="<%=Application("NavButtonGifPath")%>new_search_btn.gif" width="71" height="20" alt="" border="0"></a>';
	outputval += (MainWindow.getButton("log_off")) + "</td></tr></table>";
return outputval
}

///////////////////////////////////////////////////////////////////////////////
// Opens up a dialog box
// The size and positions of the popups has been optimized to look Ok even at 800 X 600 resolution
function OpenDialog(url, name, type)
{
	WindowDef_1 = "height=530, width= 530px, top=50, left=0";
	WindowDef_2 = "height=580, width= 850px, top=0, left=0";
	WindowDef_3 = "height=450, width= 300px, top=50, left=540";		
	var WindowDef = eval("WindowDef_" + type);
	var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable," + WindowDef;
	var DialogWindow = window.open(url,name,attribs);
	return DialogWindow;
}


//////////////////////////////////////////////////////////////////////////////////////////
// Displays the zoom button and calls ACX_doStructureZoom() when clicked
//
function ACX_getStrucZoomBtn(fullSrucFieldName, BaseID, structDataObjName, gifWidth, gifHeight){
	var outputval = ""
	var buttonGifPath = button_gif_path + "zoom_btn.gif"
	var params = "&quot;" + fullSrucFieldName + "&quot;," + BaseID + ",&quot;" + structDataObjName + "&quot;" 
	
	if(typeof gifWidth != "undefined"){
		params +=  "," + gifWidth + "," + gifHeight	  
	}
	outputval = '<A HREF ="Show Structure in larger window" onclick="ACX_doStructureZoom(' + params + ');return false;"><IMG SRC="' +  buttonGifPath + '" BORDER="0"></A>'
	document.write (outputval)
}

//////////////////////////////////////////////////////////////////////////////////////////
// Pops up a window with zoom_structure.asp in it
//
function ACX_doStructureZoom(fullSrucFieldName, BaseID, structDataObjName, gifWidth, gifHeight){
	var z = ""
	var attribs = 'width=550,height=600,left=60,top=60,xpos=60,ypos=60,status=yes,resizable=yes';
	var url = app_Path + "/zoom_structure.asp?baseid="+ BaseID + "&dbname=" + dbname + "&fullSrucFieldName=" + fullSrucFieldName + "&structDataObjName=" + structDataObjName;
	
	if (typeof gifWidth != "undefined"){
		url += "&gifWidth=" + gifWidth + "&gifHeight=" + gifHeight;
	}
	
	if (z.name == null){
		z = window.open(url,"zoom_structure", attribs);
		z.name = "zoom_structure"
	}
	else{
		z.focus()
	}
}

//////////////////////////////////////////////////////////////////////////////////////////
// Generic Cookie Reader Function
//
function ReadCookie(cookiename){
		var allcookies = document.cookie;
		var  pos = allcookies.indexOf(cookiename + "=");
		if (pos != -1){
			var start = pos + cookiename.length + 1;
			var end = allcookies.indexOf(";",start);  
			if (end == -1){
				end= allcookies.length;
			}
			var cookiestr = unescape(allcookies.substring(start,end));
			var out = cookiestr;
			return out;
		}
		else { 
			var out = "";
			return out;
		}
	}

//////////////////////////////////////////////////////////////////////////////////////////
// Popup the vendor selection interface
//
	function editFavoriteVendorsList(){
		<%if Application("supplierSelector")="supplier_select_cb.asp" then %>
			PopupWindow = window.open("<%=Application("supplierSelector")%>?listType=1","ChemACX_SupplierSelect",'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=500,height=700,xpos=250,ypos=10,left=250,top=10');
		<%Elseif Application("supplierSelector")="supplier_select.asp" then %>
			PopupWindow = window.open("supplier_select.asp?listType=1","ChemACX_SupplierSelect",'toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=780,height=550,xpos=20,ypos=20,left=20,top=20');
		<%End if%>
	}

	
//////////////////////////////////////////////////////////////////////////////////////////
// Write the selected vendor list to a cookie
//	
	function writeLimitCookie(vboolean,flag){
		limitflag = flag;
		nextyear = new Date();
		nextyear.setFullYear(nextyear.getFullYear() +1);
		var wholecookie= "acxlimitsearch=" + vboolean + "; expires=" + nextyear.toGMTString() 
		document.cookie =  wholecookie;
		if (vboolean == 0) {
			document.cows_input_form["Product.SupplierID"].value="";
		}
		if (vboolean == 1) {
			var supslist = ReadCookie("acxprefsuplist").replace(/:/gi,",");
			document.cows_input_form["Product.SupplierID"].value=supslist;
		}
		if ((vboolean == 1) && (ReadCookie("acxprefsuplist")== "")){
			editFavoriteVendorsList();
		}
	}

// Function to display the total hits from search
function getTotalHits(){
	var outputval = ""
	if (formmode.toLowerCase() != "search"){
		var outputval = '<IMG SRC="' + button_gif_path + 'total_hits.gif">&nbsp;'
		if (more_available == "True"){
			outputval = outputval + '<%=Session("hitlistRecordCount" & dbkey)%>&nbsp;<BR>'
		}
		else
		{
			outputval = outputval + totalrecords + '&nbsp;<BR>';
		}
	}
	else{
		var outputval = '<IMG SRC="' + button_gif_path + 'totalrecords_txt.gif">&nbsp;'
		outputval = outputval + MainWindow.db_record_count + '&nbsp;<BR>';
	}
	return outputval
}

// Custom Function to start search
function ACXSearch(){
		var action = "search";
		var actiontemp = getActionTemp(action)
		var dbname = getDBName(action)
		setCookie("PagingMove" + MainWindow.dbname + MainWindow.formgroup ,"first_record",1)
		getStructureFields()
		getFormulaFields()
		getMolWeightFields()
		getRelationalFields()
		missingfields = getSearchStrategy()
		if (needToAuthenticate){
			MainWindow.document.cows_input_form.action = "../postrelay.asp?dataaction=search&formmode=list" + "&formgroup=" + formgroup + "&dbname=" + dbname 
		}
		else{
			MainWindow.document.cows_input_form.action = actiontemp + "?formmode=" + formmode + "&formgroup=" + formgroup + "&dataaction=" + action + "&dbname=" + dbname
		}
		MainWindow.document.cows_input_form.submit();	
}

//Prints the most important frame/s
function ACXPrintCurrentPage(){
	if (MainWindow.formmode == "edit"){ 
	MainWindow.parent.focus();
	}
	else{
	MainWindow.focus();
	}
	window.print();
}

function doStructureCopy(structDataObjName, isDialog) {
    var isNotCopied = true;
    if(chemdrawjs) {
        var base64_cdx = (isDialog) ? opener.document.getElementById(structDataObjName).value : document.getElementById(structDataObjName).value;
        var b64 = base64_cdx.replace(new RegExp('<br>', 'g'), '');
        chemdrawjs.loadB64CDX(b64);
        var cdxml = chemdrawjs.getCDXML();
        if (cdxml != '') {
            var textField = document.createElement('textarea');    
            document.body.appendChild(textField);
            textField.innerText = cdxml;
            textField.select();
            document.execCommand('copy');
            textField.remove(); 
            isNotCopied = false;
        }
    } 
    if (isNotCopied) {
        alert('Structure is still loading, try again in a few seconds!');
    }
}

function doStructureCopyIndividual() {
    var isNotCopied = true;
    if(chemdrawjs) {
        var cdxml = chemdrawjs.getCDXML();
        if (cdxml != '') {
            var textField = document.createElement('textarea');    
            document.body.appendChild(textField);
            textField.innerText = cdxml;
            textField.select();
            document.execCommand('copy');
            textField.remove();
            isNotCopied = false;
        }   
    } 
    if (isNotCopied) {
        alert('Structure is still loading, try again in a few seconds!');
    }
}

</script>



