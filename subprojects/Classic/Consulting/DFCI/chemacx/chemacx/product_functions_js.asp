<Script language="JavaScript">
//////////////////////////////////////////////////////////////////////////////////////
// populates all boxes with values from the shopcart object 
function InitCheckBoxes(){
	if (theDisplayFrame.document.dispform)
	for (Row in top.bannerFrame.ShopcartWRS.qty) AsignBoxVal(Row);
}

////////////////////////////////////////////////////////////////////////////////////////
// Fills in input box and check box values to agree with top.bannerFrame.ShopcartWRS
function AsignBoxVal(n){
	// get the box associated with the nth element of the shopping cart
	box = eval("theDisplayFrame.document.dispform.ordered_" +top.bannerFrame.ShopcartWRS.prodid[n] );
	// fill the value if the input box is in the document
	if (box){
		box.value = top.bannerFrame.ShopcartWRS.qty[n];
	// check the box if the shopcart item is still not blank
	if (box.value){
		checkbox = eval("theDisplayFrame.document.dispform.orderedcheck_" + top.bannerFrame.ShopcartWRS.prodid[n]);
		checkbox.checked =true;
	}
	else{ 
		top.bannerFrame.ShopcartWRS.deleteRow(n);}
	}
}

////////////////////////////////////////////////////////////////////////////
// clear the box associated with the nth element of the shopping cart
function ClearBoxes(n){
	box = eval("theDisplayFrame.document.dispform.ordered_" +top.bannerFrame.ShopcartWRS.prodid[n] );
	// clear the value if the input box is in the document
	if (box){
		box.value = '';
		checkbox = eval("theDisplayFrame.document.dispform.orderedcheck_" + top.bannerFrame.ShopcartWRS.prodid[n]);
		checkbox.checked =false;
	}
}

////////////////////////////////////////////////////////////////////////
// Clear the boxes of all elements associeted with the shopping cart
function ClearAllBoxes(){
	var cend= top.bannerFrame.ShopcartWRS.getRowCount();
	for (var j=0; j < cend ;j++){
		ClearBoxes(j);
	}
}

///////////////////////////////////////////////////////////////////////////////// open the product detail popup window
// Open the Synonyms popup window
function openSynWindow(leftPos,topPos,csNum,recordNum){
	var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=300,height=200,screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos
	var SynWindow = window.open("Synlookup.asp?CsNum=" + csNum + "&recordNum=" + recordNum,"Synonyms_Window",attribs);
}

///////////////////////////////////////////////////////////////////////////////// open the product detail popup window
// Open the catalog info popup window
function Popup(prodid,strucid, isPlugin){
	var attribs = 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=600,height=650,left=260,top=60,xpos=260,ypos=60';
 	var PopupWindow = window.open("chemacx_prod_detail.asp?isPlugin=" + isPlugin + "&formgroup=" + formgroup + "&dbname=chemacx&prodid=" + prodid +"&strucid="+ strucid ,"ChemACX_ProductDetail",attribs);
	return false;
}

///////////////////////////////////////////////////////////////////////////////// open the vendor detail popup window
// Open the vendor contact info popup window
function PopupV(supid){
	var attribs = 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=600,height=500,left=60,top=60,xpos=60,ypos=60';
 	var PopupWindow = window.open("chemacx_vendor_detail.asp?supid=" + supid, "ChemACX_VendorDetail",attribs);
}

///////////////////////////////////////////////////////////////////////////////////////
// Fill the supplier selection box from the contents of a client side Wddx packet
function FillSelectFromWDDXPacket(WddxString){
	Des = new WddxDeserializer();
	Obj = Des.deserialize(WddxString);
	var ispacket = WddxString.indexOf("</wddxPacket>")
	if (ispacket == -1){
		if (noSuppliersFound){
			var idValue = "NS"
		}
		else{
			idValue = "CD"
		}
		Obj = new WddxRecordset();
		Obj.addColumn("SupplierName");
		Obj.addColumn("SupplierID");
		Obj.setField(0,"SUPPLIERNAME", WddxString)
		Obj.setField(0,"SUPPLIERID", idValue)
	}
	FillSelect(document.cows_input_form.supplierList,Obj);
}

///////////////////////////////////////////////////////////////////////////////////////
// Fill the supplier selection box from the contents of a client side Wddx recordset
function FillSelect(selobj,WRSobj) {   
     var firstOptionText;
	 if (document.cows_input_form.selectedList.value == 1){
	 	firstOptionText = "<%=tab1OptionText%>";
	 }
	 else if (document.cows_input_form.selectedList.value == 2){
	 	firstOptionText = "<%=tab2OptionText%>"; 
	 }
	 else if (document.cows_input_form.selectedList.value == 3){
	 	firstOptionText = "<%=tab3OptionText%>";
	 }  
	 else{
	 	firstOptionText = "---NO VENDORS FOUND---";
	 } 
	 selobj.options.length = 0; 
	 document.cows_input_form.NewOpt = new Option;
     document.cows_input_form.NewOpt.text = firstOptionText ;
	 selobj.options[selobj.options.length] = document.cows_input_form.NewOpt;
	 document.cows_input_form.NewOpt.value ="";
	 for (var i = 0; i < WRSobj.getRowCount(); i++){ 
	 	document.cows_input_form.NewOpt = new Option;
        document.cows_input_form.NewOpt.value = WRSobj.getField(i,"SUPPLIERID");
		document.cows_input_form.NewOpt.text = WRSobj.getField(i,"SUPPLIERNAME");
        selobj.options[selobj.options.length] = document.cows_input_form.NewOpt;		     
	 }
}

////////////////////////////////////////////////////////////////////////////////////
// highlight a supplier type tab dimming all others
function toggleTabs(tabnumber){
	window.document.images.tab1.src = tab1_off.src
	<%if bLimitSearch = "0" OR blimitSearch = "" then%>
		window.document.images.tab2.src = tab2_off.src
		window.document.images.tab3.src = tab3_off.src
	<%End if%>	
	thetab= "tab" + tabnumber;
	if (tabnumber){
		window.document.images[thetab].src = eval(thetab + "_on.src");
	}
}

///////////////////////////////////////////////////////////////////////////////////////
// Build a comma delimeted list from values selected from select object
function ListFromSelect(selobj){
	var list="";
	numselected = 0 //clear global
	for (var index=0; index< selobj.options.length; index++){
		if (selobj.options[index].selected){ list += selobj.options[index].value + ","
			numselected++
		}
	}
	list= list.substr(0,list.length-1);
	return list
}

/////////////////////////////////////////////////////////////////////////
// Toggles between compact and catalog views
function ToggleProductView(supplierList){
	if (productViewFunction == "DisplayBySupplierID_compact"){
		productViewFunction = "DisplayBySupplierID";
		DisplayBySupplierID(ListFromSelect(supplierList))
	}
	else{
		productViewFunction = "DisplayBySupplierID_compact";
		DisplayBySupplierID_compact(ListFromSelect(supplierList))
	}
}

/////////////////////////////////////////////////////////////////////////////////////////
// Display selected supplier's products in catalog view
function DisplayBySupplierID(supplierIDList){
	if (supplierIDList == "") return false;
	if (document.cows_input_form.supplierList.selectedIndex == -1){
		alert('Please select one or more suppliers from the list');
		return false;
	}
	else if (numselected >5){
		alert("Please select a maximum of 5 suppliers at a time");
		return false;
	}
	else{
		theDisplayFrame.document.open();
		var out= "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head></head><body><BR><BR><h3 align=center>LOADING PRODUCT DATA...</h3></body></html>";
		theDisplayFrame.document.write(out);
		theDisplayFrame.document.close();
		prodshown = true;
		theDisplayFrame.document.open();
		
		if (supplierIDList == "CD"){
			out = headerText() + PackageDataNotDisplayedNotice();
		}
		else if (supplierIDList == "NS") {
			out = headerText() + noSuppliersNotice();
			theDisplayFrame.document.write(out);
		} 
		else{
			out = headerText() 
			//theDisplayFrame.document.write(out);
			theDisplayFrame.document.write(out + '<form name=dispform><table cellpadding=3 cellspacing=5 border=0>');
			var rowArray = ConvertSupplierIDtoRowArray(supplierIDList);
			var BaseID = "<%=BaseID%>";
			if (BaseID == ""){ BaseID=0 }
			
			for (var index =0; index < rowArray.length; index++){
				theDisplayFrame.document.write('<tr><td  align="left">');
				ProdWRSRow = rowArray[index];
				ACXProd_Display(ProdWRSRow, BaseID , <%=Application("addToCartWithoutPrice")%>  , <%=Application("showListPrice")%>);	
				theDisplayFrame.document.write('</td></TR>');
			}
			theDisplayFrame.document.write('</table></form>');
		}
		theDisplayFrame.document.write('</body></html>');
		theDisplayFrame.document.close();
	}
}

/////////////////////////////////////////////////////////////////////////////////////////
// Display selected supplier's products in compact view
function DisplayBySupplierID_compact(supplierIDList){
	if (document.cows_input_form.supplierList.selectedIndex == -1){
		alert('Please select one or more suppliers from the list');
		return false;
	}
	else if (numselected >10){
		alert("Please select a maximum of 10 suppliers at a time");
		return false;
	}
	else{
		theDisplayFrame.document.open();
		var out= "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head></head><body><BR><BR><h3 align=center>LOADING PRODUCT DATA...</h3></body></html>";
		theDisplayFrame.document.write(out);
		theDisplayFrame.document.close();
		prodshown = true;
		theDisplayFrame.document.open();
	
		if (supplierIDList == "CD"){
			out = headerText() + PackageDataNotDisplayedNotice();
		}
		else if (supplierIDList == "NS") {
			out = headerText() + noSuppliersNotice();
		} 
		else{
			out = headerText() 
			theDisplayFrame.document.write(out);
			
			theDisplayFrame.document.write('<form name=dispform>');
			compact_Header(<%=BaseID%>);
			var rowArray = ConvertSupplierIDtoRowArray(supplierIDList);
			for (var index =0; index < rowArray.length; index++){
				var ProdWRSRow = rowArray[index];
				ACXProd_Display_compact(ProdWRSRow, <%=BaseID%>,  <%=Application("addToCartWithoutPrice")%>, <%=Application("showListPrice")%>);
			}		
			theDisplayFrame.document.write('</table></form>');
		}
		theDisplayFrame.document.write('</body></html>');
		theDisplayFrame.document.close();
	}
}

///////////////////////////////////////////////////////////////////////////////////////
//  Display all products in compact view
function DisplayAllCompact(){
	theDisplayFrame.document.open();
	var out= "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"><html><head></head><body><BR><BR><h3 align=center>LOADING PRODUCT DATA...</h3></body></html>";
	theDisplayFrame.document.write(out);
	theDisplayFrame.document.close();

	theDisplayFrame.document.open();

	out = headerText() 
	theDisplayFrame.document.write(out);
	theDisplayFrame.document.write('<form name=dispform><center>');
	ACXProd_Display_compact(<%=BaseID%>);
	theDisplayFrame.document.write('</center></form>');
	theDisplayFrame.document.write('</body></html>');
	theDisplayFrame.document.close();
}


/////////////////////////////////////////////////////////////////////////////////////
// Display Substance name and number of products and suppliers found
function headerText(){
	if (numProducts != 1){ prodPlural= "S";} else {prodPlural= "";} 
	if (numSuppliers != 1){ supPlural= "S";} else {supPlural= "";} 
	var out;
	out = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">"
    out += "<html><head>";
	//out += '<scr' + 'ipt LANGUAGE="javascript" src="/chemacx/Choosecss.js"></scr' + 'ipt>';
	if (navigator.appName == "Netscape"){
		out +='<LINK REL=STYLESHEET HREF="/chemacx/chemacx_ns.css" TYPE="text/css">';
	}
	else{
		out +='<LINK REL=STYLESHEET HREF="/chemacx/chemacx_ie.css" TYPE="text/css">';
	}
	out += "</head>";
	out += "<body bgcolor=white onload=\"if (document.dispform)top.main.mainFrame.InitCheckBoxes();\">"
	var substanceName = "<%=Replace(firstsyn, Chr(13)&Chr(10)," ")%>";
	substanceName = substanceName.toUpperCase();
	if (substanceName.length){
		out += "<table cellpadding=0 cellspacing=0 border=0><tr><td><table border=1 cellpadding=0 cellspacing=0 bgcolor=#FFFFD6 width=350><tr><td nowrap><font face=verdana><strong>"  
		out += insertIntoString(substanceName,50,"<BR>")  
		out += "</strong></font></td></tr></table></td></tr></table>";
		out += "<BR>";
	}
	out += "<table border=0><tr><td>"; 
	out += "<h5 align=center>";
	out += "<font face=arial>FOUND " + numSuppliers + " SUPPLIER" + supPlural + " SELLING " + numProducts + " PRODUCT"+ prodPlural;
	out += "<%if usepricerobot = True then response.write "<font type=arial size=1><BR>PRICING FROM ChemACX.Com</font>" else response.write ""%>";
	out += " </font></h5> ";
	out += "</td></tr></table>";
	//alert(out);
	return out;
}

/////////////////////////////////////////////////////////////////////////////////////
// Prompt user to select a supplier from the list
function SelectSuppliersNotice(){
	prodshown = false;
	theDisplayFrame.document.open();
	var out= headerText()
	out += "<table border=1 width=350><tr><td align=center><h5>"; 
	out += "<P><font face=Arial>TO COMPARE PRICES AND CATALOG DATA PLEASE CHOOSE ONE OR MORE SUPPLIERS FROM THE SELECTION LIST TO THE LEFT</P>You can select multiple suppliers by using<br> Ctrl-Click (PC) or Cmnd-Click (Mac)</font>";
	out += "</h5></td></tr></table></body></html>";
	theDisplayFrame.document.write(out);
	theDisplayFrame.document.close();
}

///////////////////////////////////////////////////////////////////////////////////
// Display ChemACX CD-ROM Notice
function PackageDataNotDisplayedNotice(){
theDisplayFrame.document.open();
var out= "";
out += "<table border=1 width=350><tr><td align=center><h5>"; 
out += "<P><font face=Arial>THIS SUBSTANCE IS COMMERCIALLY AVAILABLE BUT IS ONLY LISTED ON THE SUBCRIPTION VERSION OF THE ChemACX DATABASE</P><P>Please contact <a href='mailto:sales@cambridgesoft.com'>sales@cambridgesoft.com</a></P></body></html></font>";
out += "</h5></td></tr></table>";
theDisplayFrame.document.write(out);
theDisplayFrame.document.close();
return out;
}

///////////////////////////////////////////////////////////////////////////////////
// Display ChemACX CD-ROM Notice
function noSuppliersNotice(){
	theDisplayFrame.document.open();
	var out= "";
	out += "<table border=1 width=350><tr><td align=center><h5>"; 
	out += "<P><font face=Arial>NO SUPPLIERS FOUND FOR THIS SUBSTANCE.  </P><P>Please contact <a href='mailto:sales@cambridgesoft.com'>sales@cambridgesoft.com</a></P></body></html></font>";
	out += "</h5></td></tr></table>";
	theDisplayFrame.document.write(out);
	theDisplayFrame.document.close();
	return out;
}
	
////////////////////////////////////////////////////////////////////////
//  Inserts a delimiter string into a target string every chunksize characters
//  Used, for example to force a wrap by adding a break every n characters
function insertIntoString(str,chunksize,delimiter){
var start =0;
var newstr = "";
	for (i=1; i< str.length/chunksize; i++){
	chunk = str.substr(start,chunksize) + delimiter;
	newstr += chunk;
	start += chunksize;
	}
	newstr += str.substr(start,str.length-start);
	return newstr;
}
</script>