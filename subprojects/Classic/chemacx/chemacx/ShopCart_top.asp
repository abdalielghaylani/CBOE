<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
	<title>Untitled</title>
<!-- Include WDDX Javascript support-->
<SCRIPT LANGUAGE="JavaScript" SRC="wddx.js"></SCRIPT>
<SCRIPT LANGUAGE="JavaScript" SRC="wddxRsEx.js"></SCRIPT>
<script LANGUAGE=JAVASCRIPT>
<!--
function checkinteger(object)
    {
    //Returns true if value is a number or is NULL
    //otherwise returns false	
var object_value
object_value = object 
    if (object_value.length == 0)
        return true;

    //Returns true if value is an integer defined as
    //   having an optional leading + or -.
    //   otherwise containing only the characters 0-9.
	var decimal_format = ".";
	var check_char;

    //The first character can be + -  blank or a digit.
	check_char = object_value.indexOf(decimal_format)
    //Was it a decimal?
    if (check_char < 1)
	return checknumber(object_value);
    else
	
	return false;
    }

function checknumber(object_value)
    {
    //Returns true if value is a number or is NULL
    //otherwise returns false	

    if (object_value.length == 0)
        return true;

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
//-->
</script>

<script language="JavaScript1.1">
focus();

// Function to delete row from ShopCartWRS which lives in opener page, then re-display it 
  function DeleteRow(RowNum) { 
	if ((top.opener.productsShown) && (parent.opener.theMainFrame.prodshown)){
	parent.opener.theMainFrame.ClearBoxes(RowNum);
	}
	top.opener.ShopcartWRS.deleteRow(RowNum);
    DisplayCart();}


// Function to delete row from ShopCartWRS which lives in opener page, then re-display it 
  function DeleteAll() { 
	max = top.opener.ShopcartWRS.getRowCount()-1
	for (rRow = max; rRow >= 0;  rRow--){
		if ((top.opener.productsShown) && (parent.opener.theMainFrame.prodshown)){
			parent.opener.theMainFrame.ClearBoxes(rRow);
		}
		top.opener.ShopcartWRS.deleteRow(rRow);
	}
    DisplayCart();
  }
    
// Changes qty value and recalculates total
function Recalculate(RowNum){
qtybox= eval('parent.BottomFrame.document.cartform.qty_' + RowNum);
if (checkinteger(qtybox.value)){
if ((top.opener.productsShown) && (parent.opener.theMainFrame.prodshown)){
	//change the value in the text box
	box = eval("parent.opener.theDisplayFrame.document.dispform.ordered_" + top.opener.ShopcartWRS.prodid[RowNum]);
	if (box){
	box.value = parseInt(eval('parent.BottomFrame.document.cartform.qty_' + RowNum+ '.value'));
	}
}
// recalculate total
for (rRow = 0; rRow < top.opener.ShopcartWRS.getRowCount(); rRow++) {
top.opener.ShopcartWRS.qty[rRow]=  eval('parent.BottomFrame.document.cartform.qty_' + rRow+ '.value');
}
DisplayCart();
}
else
{
qtybox.value = top.opener.ShopcartWRS.qty[RowNum]
alert("Value must be an integer!");}
}

///////////////////////////////////////////////////////////////////////////////// Change the price of an item in the shopcart recordset
//
function ChangePrice(therow){
pbox= eval('parent.BottomFrame.document.cartform.price_' + therow);
if (checknumber(pbox.value)){
top.opener.ShopcartWRS.unitprice[therow]=  eval('parent.BottomFrame.document.cartform.price_' + therow+ '.value');
DisplayCart();}
else
{
pbox.value = top.opener.ShopcartWRS.unitprice[therow]
alert("Value must be a number!");
}
} 

// Toggle the value of the export field
function ToggleExport(cboxv,arow){
top.opener.ShopcartWRS.exprt[arow]= cboxv;
}

function Checkexprt(){
for (var rRow = 0; rRow < top.opener.ShopcartWRS.getRowCount(); rRow++) {
var thecbox = eval('parent.BottomFrame.document.cartform.exprt_' + rRow);
thecbox.checked = top.opener.ShopcartWRS.exprt[rRow];
}
}

function WinPrint(){
	if (navigator.userAgent.substring(8,9) > 3){
		if (navigator.appName == "Netscape"){
			parent.BottomFrame.print();}
		else if (navigator.appName == "Microsoft Internet Explorer"){
			parent.BottomFrame.focus();
			//parent.BottomFrame.WebBrowser1.ExecWB(6,1);
			parent.BottomFrame.print();
		}
	}
	else{
	alert("The print button does not work because your Navigator version is less than 4.0\n Try cntrl-P on the PC or command-P on a Mac");
	}
}

// Creates a CSV string from the Shopping Cart RS
function ToCSVString(theaction, delimiter){
var CSV = "";

if (delimiter == 1) 
{var dq = String.fromCharCode(34);
CSV = "ACXSUPID,SUPPLIER,QTY,UNIT,CATNUM,DESCRIPTION,UPRICE,SUBTOT"; 
CSV += String.fromCharCode(13,10);}
else
{var dq = "&quot;";}
for (var Row = 0; Row < top.opener.ShopcartWRS.getRowCount(); Row++)
{
var subtot= parseFloat(top.opener.ShopcartWRS.unitprice[Row]) * top.opener.ShopcartWRS.qty[Row];
if (isNaN(subtot)) {subtot='';}

//if (top.opener.ShopcartWRS.exprt[Row]){
CSV+= dq+ top.opener.ShopcartWRS.vendorcode[Row] + dq+ ",";
CSV+= dq+ top.opener.ShopcartWRS.vendorname[Row] + dq+ ",";
CSV+= dq+ top.opener.ShopcartWRS.qty[Row] + dq+ ",";
CSV+= dq+ top.opener.ShopcartWRS.unitmeas[Row] + dq+ ",";
CSV+= dq+ top.opener.ShopcartWRS.catnum[Row] + dq+ ",";
CSV+= dq+ top.opener.ShopcartWRS.description[Row] + dq+ ",";
CSV+= dq+ formatAsPrice(top.opener.ShopcartWRS.unitprice[Row]) + dq+ ",";
CSV+= dq+ subtot + dq; 
CSV+= ",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,";
CSV+= String.fromCharCode(13,10);}
//}
window.open('about:blank','CSVWindow','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=600,height=800,xpos=100,ypos=0,left=100,top=0')
parent.BottomFrame.document.cartform.action= theaction;
parent.BottomFrame.document.cartform.target= "CSVWindow";
parent.BottomFrame.document.cartform.CSVcart.value= CSV;
parent.BottomFrame.document.cartform.submit();
}

function PlaceACXOrder(){
	ShoppinglistAsWDDX = parent.opener.SerializeShopCart();
	//alert("WDDXContent= " + ShoppinglistAsWDDX);
	parent.BottomFrame.document.cartform.WDDXContent.value=ShoppinglistAsWDDX;
	parent.BottomFrame.document.cartform.action="http://sales.chemacx.com/chemsales/CS2_createorder.cfm";
	if ((parent.opener.name=='ControlFrame')  && (parent.opener.theMainFrame.prodshown)){
	parent.opener.ClearAllBoxes();
	}
	parent.BottomFrame.document.cartform.submit();
	
}


function RequestACXQuote(){
ShoppinglistAsWDDX = parent.opener.SerializeShopCart();
	//alert("WDDXContent= " + ShoppinglistAsWDDX);
	parent.BottomFrame.document.cartform.WDDXContent.value=ShoppinglistAsWDDX;
	parent.BottomFrame.document.cartform.action="http://sales.chemacx.com/chemsales/CS2_RequestQuote.cfm";
parent.BottomFrame.document.cartform.submit();
}

function Export_SiAl(){
	var numchecked=0;
	for (var Row = 0; Row < parent.opener.ShopcartWRS.getRowCount(); Row++){
		//var thecbox = eval('parent.BottomFrame.document.cartform.exprt_' + Row);
		var vcode= parent.opener.ShopcartWRS.vendorcode[Row];
		if ((vcode == 53)|| (vcode == 56)|| (vcode == 59)|| (vcode == 61)|| (vcode == 62)){
			numchecked ++
			//thecbox.checked = true;
			parent.opener.ShopcartWRS.exprt[Row] =  true;
		}
		else{
			//thecbox.checked = false;
			parent.opener.ShopcartWRS.exprt[Row]= false;
		}
	}
	if (numchecked == 0) {
	alert("No Sigma-Aldrich-Fluka items in shopping cart!");
	return false;
	}
	else{
	parent.BottomFrame.document.cartform.isSigma.value=1;
	ToCSVString("HeaderForm.asp");
	}
}


function ExportToExcel(){
var ShoppinglistAsWDDX = parent.opener.SerializeShopCart();
//alert("WDDXContent= " + ShoppinglistAsWDDX);
window.open('about:blank','ToXLWindow','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=600,height=600,xpos=100,ypos=0,left=100,top=0')

parent.BottomFrame.document.cartform.WDDXContent.value=ShoppinglistAsWDDX;
parent.BottomFrame.document.cartform.action= "XLHeaderForm.asp";
parent.BottomFrame.document.cartform.target= "ToXLWindow";
parent.BottomFrame.document.cartform.submit();
}

function Export_WORD(){
	var numchecked=0;
	for (var Row = 0; Row < parent.opener.ShopcartWRS.getRowCount(); Row++){
		//var thecbox = eval('parent.BottomFrame.document.cartform.exprt_' + Row);
			numchecked ++
			//thecbox.checked = true;
			parent.opener.ShopcartWRS.exprt[Row] =  true;
	}
	//parent.BottomFrame.document.cartform.isSigma.value=0;
	ToCSVString("HeaderForm.asp");
	}


function MailMergeHelp(){
window.open('mailmerge_instructions.asp','CSVWindow','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=0,width=650,height=450,xpos=50,ypos=50,left=50,top=50')
}


function SendToChemInv(){
	if(confirm("Shopping Cart will be cleared, Do you want to continue ?") == true) {
	packageIDList = "";
	qtyList = "";
	for (var Row = 0; Row < top.opener.ShopcartWRS.getRowCount(); Row++){	
		packageIDList += top.opener.ShopcartWRS.prodid[Row] + ",";
		qtyList += top.opener.ShopcartWRS.qty[Row] + ",";
	}
	packageIDList = packageIDList.substring(0,packageIDList.length-1);
	qtyList = qtyList.substring(0,qtyList.length-1);
	parent.BottomFrame.document.cartform.packageIDList.value = packageIDList;
	parent.BottomFrame.document.cartform.qtyList.value = qtyList;
	//alert(packageIDList);
	//alert(qtyList);
	parent.BottomFrame.document.cartform.action= "/cheminv/gui/ImportFromChemACX.asp?postrelay=1";
	parent.BottomFrame.document.cartform.target= "ToINVWindow";
	parent.BottomFrame.document.cartform.submit();
	DeleteAll();
}	
}



	
// Function to display the contents of cart in lower frame
function DisplayCart() {
    with (parent.BottomFrame.document) {
      
	  open();
      writeln('<body bgcolor=#FFFFFF>');
	  writeln('<img src="<%=Application("NavButtonGifPath")%>big_shopping_cart.gif" alt="" border="0" align="absmiddle">');
        // Begin the table and write the table header
        if (top.opener.ShopcartWRS.getRowCount() > 0) {
          writeln('<form name=cartform action="dummy" method="post" target="_top">');
		  writeln('<center><table align=center CELLSPACING="0" WIDTH="95%" border="1" ><tr>');
		  writeln('<td width="20" align="center"><small><strong>');
          writeln('ITEM');
          writeln('</small></strong></td>');
		  writeln('<td width="40"><small><strong>');
          writeln('QTY');
          writeln('</small></strong></td><td align=center><small><strong>');
          writeln('SIZE');
          writeln('</small></strong></td><td><small><strong>');
		  writeln('CAS');
		  writeln('</small></strong></td><td><small><strong>');
		  writeln('DESCRIPTION');
		  writeln('</small></strong></td><td><small><strong>');
		  writeln('VENDOR');
		  writeln('</small></strong></td><td><small><strong>');
		  writeln('CATALOG#');
		  writeln('</small></strong></td><td align=center><small><strong>');
          writeln('PRICE');
		  writeln('</small></strong></td><td align=center><small><strong>');
 		  writeln('SUBTOT');
		  writeln('</small></strong></td><td align=center><small><strong>');
          writeln('REMOVE?');
          writeln('</small></strong></td></tr>');
		 //writeln('<td width="30"><small><strong>');
          //writeln('EXPORT?');
         // writeln('</small></strong></td></tr>');
		// Write the data rows
				var ordertotal = 0;
				
				//alert(top.opener.ShopcartWRS.getRowCount());
		        for (var Row = 0; Row < top.opener.ShopcartWRS.getRowCount(); Row++) {
				currsym = top.opener.ShopcartWRS.csymbol[Row];
				
				 
				var subtot= parseFloat(top.opener.ShopcartWRS.unitprice[Row]) * parseInt(top.opener.ShopcartWRS.qty[Row]);
				
				if (isNaN(parseFloat(top.opener.ShopcartWRS.unitprice[Row]))){
					top.opener.ShopcartWRS.unitprice[Row]="NA";
				}
				
				if (isNaN(subtot)) {
					subtot="NA";
				}	
				else {
					subtot = formatAsPrice(subtot);
				}
				top.opener.ShopcartWRS.totalprice[Row]= subtot;
	          write( '<tr>' +
			   '<td align="center"><small><b>'+ (Row+1) + '.</b></small></td>' +
			   '<td><input size=3 name=qty_'+ Row +  ' value=' + top.opener.ShopcartWRS.qty[Row]  + ' onchange=" parent.TopFrame.Recalculate('+ Row +');return false;" ></td>' +
                 '<td align=center><small>'     + top.opener.ShopcartWRS.unitmeas[Row]+ '</small></td>' +
				 '<td><small>'     + top.opener.ShopcartWRS.cas[Row]+ '</small></td>' + 
				 '<td><small>'     + top.opener.ShopcartWRS.description[Row]+ '</small></td>' + 
				'<td><small>'     + top.opener.ShopcartWRS.vendorname[Row]+ '</small></td>' +
				'<td><small>'     + top.opener.ShopcartWRS.catnum[Row]+ '</small></td>');
				
				//if (top.opener.ShopcartWRS.unitprice[Row] == "NA"){
	            //    write('<td colspan=2 align="center"><small>&quot;to be quoted&quot;</small></td>');
				//}
				//else{
				theprice = formatAsPrice(top.opener.ShopcartWRS.unitprice[Row])   
				if (theprice != "NA") {
					theprice = "$" + theprice;
				}
				if (subtot != "NA"){
					 subtot = "$" + subtot;
				}
				 write('<td align=right><small>' + theprice + '</small> </td>' +   
					 '<td align=right><small>' +   subtot  + '</small></td>');  
				//}   
				//write('<td><input type=text name=price_' + Row +  ' size=7 onchange="parent.TopFrame.ChangePrice('+ Row +'); return false;" value='  + formatAsPrice(parent.opener.ShopcartWRS.unitprice[Row])   + ' ></td>');
				//write('<td align=right><small>$' +   subtot  + '</small></td>');  
                 write('<td width=50 align=center><small><a href="DeleteRow" onclick="parent.TopFrame.DeleteRow('+ Row +'); return false"><img border=0 src="<%=Application("NavButtonGifPath")%>remove_btn.gif" alt="Remove from cart" align=middle></a></small></td></tr>');
				//write('<td><input type=checkbox name=exprt_'+ Row  + ' onclick=" parent.TopFrame.ToggleExport(this.checked,'+ Row +');" ></td>' +
				 
				 //ordertotal = ordertotal + top.opener.ShopcartWRS.totalprice[Row];
				 
				 }
                //ordertotal = formatAsPrice(ordertotal);
				//write('<tr bordercolor=White><td colspan=6></td><td align="right"><small><strong>Total:</strong></small></td><td align="right" bordercolor="#4A5AA9"><small><strong>' + currsym + ordertotal + '</strong></small></td><td></td></tr>');
				write('</table><br>');
				write('<a href="Print" onclick="parent.TopFrame.DeleteAll(); return false"><img src="<%=Application("NavButtonGifPath")%>remove_all_btn.gif" alt="Remove all items from shopping cart" border="0"></a>');
				write('<a href="Print" onclick="parent.TopFrame.WinPrint(); return false"><img src="<%=Application("NavButtonGifPath")%>print_btn.gif" alt="Print the shopping cart" border="0"></a>');
				write('<a href="Close" onclick="parent.opener.focus(); parent.close(); return false"><img src="<%=Application("NavButtonGifPath")%>return_to_shopping_btn.gif" alt="Continue Shopping!" border="0"></a>');
				<% if Ucase(Application("orderAtACXcom")) = "TRUE" AND Session("Buy_ACXchemACX") then%>
				write('<A href="Order" onclick="parent.TopFrame.PlaceACXOrder(); parent.opener.ClearShopCart(); return false"><img src="<%=Application("NavButtonGifPath")%>create_order_btn.gif" alt="Order online from ChemACX.Com" border="0"></a>');
				<%end if%>
				<%if Ucase(Application("requestQuote")) = "TRUE" then%>
				write('<A href="RFQ" onclick="parent.TopFrame.RequestACXQuote(); return false"><img src="<%=Application("NavButtonGifPath")%>request_price_quote_btn.gif" alt="Request a Quote from ChemACX.Com" border="0"></a>');
				<%end if%>
				write('<input type=hidden name=CSVcart>');
				//write('<input type=hidden name=isSigma>');
				write('<input type=hidden name=WDDXContent>');
				write('<input type=hidden name=packageIDList>');
				write('<input type=hidden name=qtyList>');
				write('<input type=hidden name=acx_sessionID value="999999">');
				writeln('<input type="hidden" value="remote" name=demo>');
				writeln('<input type="hidden" value="<%=Application("CHEMACXCOM_CODE")%>" name=COWS_ID>');

				
				
				
				write('<br><BR><BR><BR>')
				write('<table><tr>')
				<% if Ucase(Application("exportToExcel")) = "TRUE" then%>
				write('<tr>')
				write('<td>')
				write('<a href="Export to MS Excel Template" onclick="parent.TopFrame.ExportToExcel(); return false"><img src="<%=Application("NavButtonGifPath")%>export_excel_btn.gif" alt="Export to Excel Template" border="0"></a>');
				write('</td>')
				write('</tr>')
				<%end if%>
				<% if Ucase(Application("exportToWord")) = "TRUE" then%>
				write('<tr>')
				write('<td>')
				write('<a href="Export to MS Word Template" onclick="parent.TopFrame.Export_WORD(); return false" ><img src="<%=Application("NavButtonGifPath")%>export_word_btn.gif" alt="Export to Word Template" border="0"></a>');
				write('</td>')
				write('</tr>')
				<% end if%>
				<% if Ucase(Application("SendToChemInv")) = "TRUE" then%>
				write('<tr>')
				write('<td>')
				write('<a href="Export to Chemical Inventory" onclick="parent.TopFrame.SendToChemInv(); return false" ><img src="<%=Application("NavButtonGifPath")%>sendtoinventorymgr.gif" alt="Send to Inventory" border="0"></a>');
				write('</td>')
				write('</tr>')
				<% end if%>
				//write('<tr><td align=center><input type=button value="Remove All" onclick=parent.TopFrame.DeleteAll()></td></tr>');
				write('</table>')
				writeln('</form></center>');
				//writeln('<OBJECT ID="WebBrowser1" WIDTH=0 HEIGHT=0 CLASSID="CLSID:8856F961-340A-11D0-A96B-00C04FD705A2">');
				}
				 
else {

        writeln('<P align=center><font face=arial size=4>Your shopping cart is empty.</font>');
		writeln('<P align=center>');
		write('<a href="Close" onclick="parent.opener.focus(); parent.close(); return false"><img src="<%=Application("NavButtonGifPath")%>return_to_shopping_btn.gif" alt="Continue Shopping!" border="0"></a>');
	
        }
		writeln('</body>');
      close();
    }
}  

// Turn a floating point number into price with two decimal places
function formatAsPrice(price) {
//alert("finish formatAsPrice()!");
  var cents = Math.floor((100*price)%100);
  var dollars = Math.floor(price);
  if (cents == 0)
    cents = "00";
  else if (cents < 10)
    cents = "0" + cents;
  if (isNaN(dollars)){
	out = "NA";
  }
  else{
	out = dollars + "." + cents;
  }
  return out;
}

</script>
</head>
<body bgcolor="#FFFFFF" onload="DisplayCart(); " onunload="if ((top.opener.productsShown) && (parent.opener.theMainFrame.prodshown)){for (Row in top.opener.ShopcartWRS.qty) parent.opener.theMainFrame.AsignBoxVal(Row)}">

</body>








