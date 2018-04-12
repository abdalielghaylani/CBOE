function formatAsPrice(price) {
  if (isPositiveNumber(price)){
	var cents = Math.floor((100*price)%100);
	var dollars = Math.floor(price);
	if (cents == 0)
	  cents = "00";
	else if (cents < 10)
	  cents = "0" + cents;
	return dollars + "." + cents;
  }
  else{
	return "NA";
  }
}

//validate Postive number
function isPositiveNumber(value){
	if (isNumber(value)){
		if (value > 0) return true;
	}
	return false;
}

// Validate numerical attribute
	function isNumber(object_value){
	    //Returns true if value is a number or is NULL
	    //otherwise returns false	
        if (object_value == null) return true;
	    if (object_value.length == 0)return true;

	    //Returns true if value is a number defined as
	    //   having an optional leading + or -.
	    //   having at most 1 decimal point.
	    //   otherwise containing only the characters 0-9.
		var start_format = " .+-0123456789";
		var number_format = " .0123456789";
		var check_char;
		var decimal = false;
		var trailing_blank = false;
		var digits = false;

	    //The first character can be + - .  blank or a digit.
		check_char = start_format.indexOf(object_value.charAt(0))
	    //Was it a decimal?
		if (check_char == 1)
		    decimal = true;
		else if (check_char < 1)
			return false;
	        
		//Remaining characters can be only . or a digit, but only one decimal.
		for (var i = 1; i < object_value.length; i++)
		{
			check_char = number_format.indexOf(object_value.charAt(i))
			if (check_char < 0)
				return false;
			else if (check_char == 1)
			{
				if (decimal)		// Second decimal.
					return false;
				else
					decimal = true;
			}
			else if (check_char == 0)
			{
				if (decimal || digits)	
					trailing_blank = true;
	        // ignore leading blanks

			}
		        else if (trailing_blank)
				return false;
			else
				digits = true;
		}	
	    //All tests passed, so...
	    return true
	}
	
	
// check if a field is empty
function isEmpty(inputVal){
	if(inputVal == "" || inputVal == null){
		return true
	}
	return false
}


function ConvertSupplierIDtoRowArray(supplierIDList){
supplierIDList= supplierIDList.toString();
var supplierIDArray = supplierIDList.split(",");
var rowArray = new Array();
var newindex= 0;
	for (var Row=0; Row< ProdWRS.getRowCount(); Row++){
		for (var id=0; id< supplierIDArray.length; id++){
			if (ProdWRS.supplierid[Row]== supplierIDArray[id]){
				rowArray[newindex] = Row;
				newindex += 1;
			}
		}
	}
return rowArray;
}

function ToggleCheckbox(cbelement){
cbelement.checked = (cbelement.checked ? false : true);
}


function ACXProd_Display(Row,strucid,allwaysshow,showListPrice){
var thesupname;
var theprodname;
var thesize;
var proddesc;
if ((ProdWRS.proddescrip[Row] == "") || (ProdWRS.proddescrip[Row] == null)){
proddesc = ""
}
else{
proddesc = ProdWRS.proddescrip[Row];
}
var df = theDisplayFrame;
df.document.write('<input type=hidden name=strucid value=' + strucid +'>'); 
 df.document.write('			<table cellpadding=1 cellspacing=0 border=0>');
 df.document.write('				<tr>');
 df.document.write('					<td valign=bottom>');		
 df.document.write('						<font face=Arial size=2><strong>');
 df.document.write(								ProdWRS.prodname[Row].toUpperCase());    
 df.document.write('						</strong></font>');
 if (ShowMSDSLinks){
	df.document.write(' | ');
	if ((ProdWRS.hasmsds[Row])){
		var QS="";
		var CN = ProdWRS.catalognum[Row];
		var SID = ProdWRS.supplierid[Row];
		
		if ((SID != "0") && (SID != "")){
			 if (QS != "") QS +=  "&amp;";
			 QS += msdxSupplierIDKeywd + "=" + SID;	
		}
			 
		if (CN != ""){
			if (QS != "") QS += "&amp;";
			QS += msdxSupplierCatNumKeywd + "=" + CN;
		}
		
		df.document.write('<a href="' + msdxURL + '?' + QS + '" class="MenuLink" target="_new" title="' + msdxLinkTitle + '">' +  msdxLinkText +'</a>');	    
	}
	else{
	   df.document.write('<a href="#" disabled title="No materials safety data available for this specific package.  Safety data from similar substances may be availalbe by clicking the MSDS link next to the structure/CAS" class="MenuLink" onclick="return false;">' +  msdxLinkText + '</a>')	
	}
 }
 df.document.write('					</td>');
 df.document.write('				</tr>');
 df.document.write('				<tr>');
 df.document.write('					<td>');
 df.document.write('						<table cellpadding=0 cellspacing=0 border=1>');
 df.document.write('							<tr>');
 df.document.write('								<td colspan=3>');
 df.document.write('									<font face=Arial size=2>');
 df.document.write(										proddesc); 
 df.document.write('									</font>');
 df.document.write('								</td>');
 df.document.write('							</tr>');
 df.document.write('							<tr>');
 df.document.write('								<td>');
 df.document.write('									<font face=Arial size=2><strong>');
 df.document.write( 										ProdWRS.name[Row]);
 df.document.write('									<a onmouseover="status=\'View vendor contact information\';return true" onmouseout="status=\'\';return true" href="#" onclick="top.main.mainFrame.PopupV(' + ProdWRS.supplierid[Row] + ');return false">');
 df.document.write('									<img src=/ChemACX/graphics/vendor_info.gif border=0 align=absmiddle></a>');
 df.document.write('									</strong></font>');
 df.document.write('								</td>');
 df.document.write('								<td align=center>');
 df.document.write('									<font face=Arial size=2>');
 df.document.write(' 										Cat#:&nbsp;');
 df.document.write(											ProdWRS.catalognum[Row]);
 df.document.write('									</font>');
 df.document.write('								</td>');
 df.document.write('								<td align=right>');
 df.document.write('								<a onmouseover="status=\'View vendor catalog details\';return true" onmouseout="status=\'\';return true" href="JavaScript: void top.main.mainFrame.Popup(' + ProdWRS.productid[Row] + ',' + strucid + ',\'' + isPlugin + '\');">');
 df.document.write('								<img src=/ChemACX/graphics/catalog_details.gif border=0 alt=Catalog_Details align=absmiddle></a>');
 df.document.write('								</td>');
 df.document.write('							</tr>');
 df.document.write('						</table>');
 df.document.write('					</td>');
 df.document.write('				</tr>');
 df.document.write('			</table>');

	if (allwaysshow){
	showaddbuttons = 1;
	}
	else{
	showaddbuttons = ProdWRS.iswww[Row];
	}

var productid = ProdWRS.productid[Row];
if(eval("typeof PricesWRS_" + productid) == "undefined") {
	tempRS = "";
}
else{
	tempRS = eval("PricesWRS_" + productid);
}

var numpacks; 
if (!tempRS == ""){
	numpacks= tempRS.getRowCount();
}
else {
	numpacks=0;
}

	
 df.document.write('			<table border=1 cellpadding=2 cellspacing=0>');
 df.document.write('				<tr>');
 df.document.write('					<td align=center >');

if (numpacks) {
 df.document.write('						<small>CatalogNum</small>');
 df.document.write('					</td>');
 df.document.write('					<td align=center>');
 df.document.write('						<small>Size</small>');
 df.document.write('					</td>');

if (showaddbuttons != 0){
 
 if (showListPrice){
  df.document.write('					<td>');
  df.document.write('						<small>List Price</small>');
  df.document.write('                   </td>');	
 } 

 df.document.write('					<td align=center>');
 df.document.write('						<small>Price</small>');
 df.document.write('					</td>');
 
 df.document.write('					<td align=right>');
 df.document.write('						<a onmouseover="status=\'View items currently in your shopping cart\';return true" onmouseout="status=\'\';return true" href="Javascript:void top.bannerFrame.ViewCart();"><img src=/ChemACX/graphics/view_shopping_cart.gif border=0></a>');
 df.document.write('					</td>');
}
} // end if for numpacks
else
df.document.write('							No package data available');
df.document.write('						</td>'); 
 df.document.write('				</tr>'); 



for (Prow=0; Prow < numpacks; Prow++){
	if ((tempRS.catalog2num[Prow] == "")||(tempRS.catalog2num[Prow] == null)){
		var orderingnumber =  ProdWRS.catalognum[Row]
	}
	else {
		orderingnumber =  tempRS.catalog2num[Prow]
	}

 df.document.write('				<tr>');
 df.document.write('					<td align=left>');
 df.document.write('						<small>');

if  (orderingnumber == "") {
 df.document.write('						&nbsp;');
}
else{ 
 df.document.write( 						orderingnumber);
} 
 df.document.write('						</small>');
 df.document.write('					</td>');
 df.document.write('					<td align=right>');
 df.document.write('						<small>');
if (tempRS.size[Prow] == ""){
 df.document.write('						&nbsp;');										
}
else{
 df.document.write(							tempRS.size[Prow]);
} 
 df.document.write('						</small>');
 df.document.write('					</td>');


thesupname = top.bannerFrame.EscapeQuotes(ProdWRS.name[Row]);
theprodname =  top.bannerFrame.EscapeQuotes(ProdWRS.prodname[Row]);
thesize = 	top.bannerFrame.EscapeQuotes(tempRS.size[Prow]);
thecatnum = top.bannerFrame.EscapeQuotes(orderingnumber);
var theprice

if (showListPrice){
theprice = formatAsPrice(tempRS.listprice[Prow])
}
else{
theprice = formatAsPrice(tempRS.price[Prow])
} 

var scpayload =  tempRS.cas[Prow] +"','"+ tempRS.strucid[Prow]+"','"+ tempRS.packageid[Prow] +"','"+ ProdWRS.supplierid[Row] +"','"+ thesupname +"','"+ thecatnum +"','"+ theprodname +"','"+ thesize +"','"+ formatAsPrice(tempRS.price[Prow]) +"','"+ theprice +"','"+ tempRS.csymbol[Prow];

if (showaddbuttons!= 0) {
 if (showListPrice){
 df.document.write('					<td align=right>');
 df.document.write('						<small>');

if ((tempRS.price[Prow] == "")||(tempRS.price[Prow] == null)){
 df.document.write('							<div align=center>&nbsp;</div>');	
 	}
	else{
 if (isNumber(theprice)) df.document.write('							$');
  
 if (showListPrice){
 df.document.write('							<strike>');
 }
 df.document.write(								 formatAsPrice(tempRS.listprice[Prow]));
 	} 
 if (showListPrice){
 df.document.write('							</strike>');
 }

 df.document.write('						</small>');	
 df.document.write('					</td>');
 }

df.document.write('					<td align=right>');
df.document.write('						<small>');
if ((tempRS.price[Prow] == "")||(tempRS.price[Prow] == null)){
 df.document.write('							<div align=center>&nbsp;</div>');	
 }
 else{
if (isNumber(theprice)) df.document.write('						$');
df.document.write(					 	formatAsPrice(tempRS.price[Prow]));
 }
df.document.write('						</small>');	
 df.document.write('					</td>');

	 df.document.write('				<td align=left valign=middle>');
	 df.document.write('						<table border=0 cellspacing=0 cellpadding=0>');
	 df.document.write('							<tr>');
	 df.document.write('								<td align=center valign=middle>');
	 df.document.write('									<input align=absmiddle type=checkbox  name=orderedcheck_');
	 df.document.write(										tempRS.packageid[Prow]);
	 df.document.write('									 onclick="top.bannerFrame.AddtoShopcartWRS(\'' + scpayload +  '\',\'1\')">');
	 df.document.write('								</td>');
	 df.document.write('								<td align=center valign=middle>');
	 df.document.write('									<ax onmouseover="status=\'Add this item to your shopping cart\';return true" onmouseout="status=\'\';return true" href="Javascript:void top.main.mainFrame.ToggleCheckbox(document.dispform.orderedcheck_' + tempRS.packageid[Prow] + ');void top.bannerFrame.AddtoShopcartWRS(\'' + scpayload +  '\',\'1\')"><img src=/ChemACX/graphics/Add_to_cart_sm_btn.gif border=0 align=absmiddle align=absmiddle></ax>');
	 df.document.write('								</td>');
	 df.document.write('								<td align=right valign=middle>');
	 df.document.write('									<font face=Arial size=1><b>&nbsp;QTY:</b></font>');
	 df.document.write('								</td>');
	 df.document.write('								<td align=right valign=middle>');
	 df.document.write('									<input type=text size=3 name=ordered_' + tempRS.packageid[Prow]);
	// df.document.write(' onfocus=blur() align=top>');
 	df.document.write(' onkeyup="if (this.value > 0) {top.bannerFrame.AddtoShopcartWRS(\'' + scpayload +  '\',\'2\'); return false} else{cb = orderedcheck_' + tempRS.packageid[Prow] + ';if (cb.checked) cb.click();}" align=top>');
	 df.document.write('								</td>');
	 df.document.write('							</tr>');
	 df.document.write('						</table>');
	 df.document.write('				</td>');
}  // end if iswww
	 df.document.write('			</tr>');
} //loop back to next package
	 df.document.write('		</table>');
return(true);
}

function compact_Header(strucid){
var df = theDisplayFrame;
df.document.write('<input type=hidden name=strucid value="+ strucid +">') 
 df.document.write('<table align=left border=1>')
 df.document.write('	<tr>')
 df.document.write('		<th>')		
 df.document.write('			<font face=Arial size=2>')
 df.document.write('			SUP')    
 df.document.write('			</font>')
 df.document.write('		</th>')
 df.document.write('		<th>')		
 df.document.write('			<font face=Arial size=2>')
 df.document.write('			CAT#')    
 df.document.write('			</font>')
 df.document.write('		</th>')
 df.document.write('		<th>')
 df.document.write('			<font face=Arial size=2>')
 df.document.write('			SIZE') 
 df.document.write('			</font>')
 df.document.write('		</th>')
	 df.document.write('		<th>')
	 df.document.write('			<font face=Arial size=2>')
	 df.document.write('			PRICE') 
	 df.document.write('			</font>')
	 df.document.write('		</th>')
	 df.document.write('		<th align=center>')
	 df.document.write('			<a href=View contents of shopping cart')
	 df.document.write('			 onclick="Javascript:top.bannerFrame.ViewCart(); return false"><img src=/ChemACX/graphics/view_shopping_cart.gif border=0></a>')
	 df.document.write('		</th>')
 df.document.write('	</tr>')

}

function ACXProd_Display_compact(Row,strucid,allwaysshow,showListPrice){
var df = theDisplayFrame;
var all = ProdWRS.getRowCount();
var thesupname;
var theprodname;
var thesize;

if (allwaysshow){
	showaddbuttons = 1;
}
else{
	showaddbuttons = ProdWRS.iswww[Row];
}

	var productid = ProdWRS.productid[Row];
		if(eval("typeof PricesWRS_" + productid) == "undefined") {
			tempRS = "";
		}
		else{
			tempRS = eval("PricesWRS_" + productid);
		}
		
	var numpacks; 
	if (!tempRS == ""){
		numpacks= tempRS.getRowCount();
	}
	else {
		numpacks=0;
	}
	
	for (Prow=0; Prow < numpacks; Prow++){
		if ((tempRS.catalog2num[Prow] == "")||(tempRS.catalog2num[Prow] == null)){
			var orderingnumber =  ProdWRS.catalognum[Row]
		}
		else {
			orderingnumber =  tempRS.catalog2num[Prow]
		}


		 df.document.write('	<tr>')
		 df.document.write('		<td>')		
		 df.document.write('			<font face=Arial size=2>')
		 df.document.write('				<a href="void 0" onclick="top.main.mainFrame.DisplayBySupplierID(' +ProdWRS.supplierid[Row]+ ');return false;">' + ProdWRS.name[Row]+ '</a>');   
		 df.document.write('			</font>')
		 df.document.write('		</td>')	
		 df.document.write('		<td align=left>')
		 df.document.write('			<small>')
		
		if  (orderingnumber == "") {
		 df.document.write('			&nbsp;')
		}
		else{ 
		 df.document.write( 			orderingnumber);
		} 
		 df.document.write('			</small>')
		 df.document.write('		</td>')
		 df.document.write('		<td align=right>')
		 df.document.write('			<small>')
		if (tempRS.size[Prow] == ""){
		 df.document.write('			&nbsp;')
		}
		else{
		df.document.write( 			tempRS.size[Prow]);
		} 
		
		 df.document.write('			</small>')
		 df.document.write('		</td>')

		thesupname = top.bannerFrame.EscapeQuotes(ProdWRS.name[Row]);
		theprodname =  top.bannerFrame.EscapeQuotes(ProdWRS.prodname[Row]);
		thesize = 	top.bannerFrame.EscapeQuotes(tempRS.size[Prow]);
		thecatnum = top.bannerFrame.EscapeQuotes(orderingnumber);
		var theprice;	
		if (showListPrice){
			theprice = formatAsPrice(tempRS.listprice[Prow])
		}
		else{
			theprice = formatAsPrice(tempRS.price[Prow])
		} 
		
		
		var scpayload =  tempRS.cas[Prow] +"','"+ tempRS.strucid[Prow]+"','"+ tempRS.packageid[Prow] +"','"+ ProdWRS.supplierid[Row] +"','"+ thesupname +"','"+ thecatnum +"','"+ theprodname +"','"+ thesize +"','"+ formatAsPrice(tempRS.price[Prow]) +"','"+ theprice +"','"+ tempRS.csymbol[Prow];
		//if (ProdWRS.iswww[Row]){
		 df.document.write('		<td align=right>')
		 df.document.write('			<small>')
		
			//if ((tempRS.price[Prow] == "")||(tempRS.price[Prow] == null)){
		 //df.document.write('			n/a')	
		 //	}
		//	else{
		if (isNumber(theprice)) df.document.write('			$');
		 df.document.write(				theprice);
		 //	} 
		 df.document.write('			</small>')	
		 df.document.write('		</td>')
		
		if (showaddbuttons){ 
		 df.document.write('		<td align=right valign=middle>')
		 df.document.write('				<table border=0 cellspacing=0 cellpadding=0>')
		 df.document.write('					<tr>')
		 df.document.write('						<td align=center valign=middle>')
		 df.document.write('							<input type=checkbox  name=orderedcheck_')
		 df.document.write( 								tempRS.packageid[Prow]);
		 df.document.write('							 onclick="top.bannerFrame.AddtoShopcartWRS(\'' + scpayload +  '\',\'1\')">')
		 df.document.write('						</td>')
		 df.document.write('						<td align=right valign=middle>')
		 //df.document.write('							<a href="Javascript:document.dispform.orderedcheck_' + tempRS.packageid[Prow]+ '.click()">Add to cart</a>')
		 df.document.write('							<img src=/ChemACX/graphics/Add_to_cart_sm_btn.gif border=0 align=absmiddle align=absmiddle>')
		 df.document.write('						</td>')
		 df.document.write('						<td align=right valign=middle>')
		 df.document.write('							<font face=Arial size=1><b>&nbsp;QTY:</b></font>')
		 df.document.write('						</td>')
		 df.document.write('						<td align=right valign=middle>')
		 df.document.write('							<input type=text size=3 name=ordered_' + tempRS.packageid[Prow]);
		 df.document.write('							 onkeyup="if (this.value > 0) {top.bannerFrame.AddtoShopcartWRS(\'' + scpayload +  '\',\'2\'); return false} else{cb = orderedcheck_' + tempRS.packageid[Prow] + ';if (cb.checked) cb.click();}" align=top>'); 
		 df.document.write('						</td>')
		 df.document.write('					</tr>')
		 df.document.write('				</table>')
		
		 df.document.write('		</td>')
		}
	else{
	 df.document.write('														<td align=left align=center>')
	 df.document.write('															we can\'t sell it!')
	 df.document.write('														</td>')
	}
		
		}
		 df.document.write('	</tr>')
	//}
	
	if (!numpacks){	
		 df.document.write('	<tr>')
		 df.document.write('		<td>')		
		 df.document.write('			<font face=Arial size=2>')
		 		 df.document.write('				<a href="void 0" onclick="top.main.mainFrame.DisplayBySupplierID(' +ProdWRS.supplierid[Row]+ ');return false;">' + ProdWRS.name[Row]+ '</a>');     
		 df.document.write('			</font>')
		 df.document.write('		</td>')	
		 df.document.write('		<td colspan=4 align=center>')
		 df.document.write('  			No Package data available')
		 df.document.write('       </td>')
		 df.document.write('	</tr>')
	}

return(true);
}		



