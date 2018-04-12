/////////////////////////////////////////////////////////////
//           Create Shopping Cart WDDX Recordset
//
function MakeWDDXShopCart(){
ShopcartWRS= new WddxRecordset;
ShopcartWRS.addColumn('prodid');
ShopcartWRS.addColumn('vendorcode');
ShopcartWRS.addColumn('vendorname');
ShopcartWRS.addColumn('catnum');
ShopcartWRS.addColumn('catnum2');
ShopcartWRS.addColumn('qty');
ShopcartWRS.addColumn('description');
ShopcartWRS.addColumn('unitmeas');
ShopcartWRS.addColumn('unitprice');
ShopcartWRS.addColumn('listprice');
ShopcartWRS.addColumn('totalprice');
ShopcartWRS.addColumn('cas');
ShopcartWRS.addColumn('strucid');
ShopcartWRS.addColumn('csymbol');
ShopcartWRS.addColumn('exprt');
}

//////////////////////////////////////////////////////////////
//     Save Shoppping Cart to Database
//
function WriteShopCartToDb(){
	
	document.form1.ShoppingCartWDDXPacket.value = SerializeShopCart();
	//alert(document.form1.ShoppingCartWDDXPacket.value)
	document.form1.SaveCart.value = "true";
	document.form1.submit();
}

//////////////////////////////////////////////////////////////
//     Save Shoppping Cart.  Will save to either cookie or DB
//
function WriteShopCart(){
	bWriteShopCartToDb ? WriteShopCartToDb()  : WriteShopCartCookie();
}

//////////////////////////////////////////////////////////////
//     Save Shoppping Cart to Cookie
//
function WriteShopCartCookie(){
var thecookie;
var A="";
	if (ShopcartWRS.getRowCount() != 0){
			for (var Row in ShopcartWRS.qty)
				{
					A += ShopcartWRS.prodid[Row]+ "!";
					A += ShopcartWRS.vendorcode[Row]+ "!";
					A += ShopcartWRS.vendorname[Row]+ "!";
					A += ShopcartWRS.catnum[Row]+ "!";
					A += ShopcartWRS.qty[Row]+ "!";
					A += ShopcartWRS.description[Row]+ "!";
					A += ShopcartWRS.unitmeas[Row]+ "!";
					A += ShopcartWRS.unitprice[Row]+ "!";
					A += ShopcartWRS.listprice[Row]+ "!";
					A += ShopcartWRS.totalprice[Row]+ "!";
					A += ShopcartWRS.cas[Row]+ "!";
					A += ShopcartWRS.strucid[Row]+ "!";
					A += ShopcartWRS.csymbol[Row]+ "!";
					A += ShopcartWRS.exprt[Row]+ "@"
				} 
			A = A.substring(0,A.length-1);
			A = escape(A);
	}
		
nextday = new Date();
nextday.setDate(nextday.getDate() +1);
thecookie ="ACXShopCart="+ A + "; expires=" + nextday.toGMTString();
document.cookie =  thecookie; 
//alert("Saved cookie:\n" + thecookie);
}

////////////////////////////////////////////////////////////
//       Read the shopping cart from Cookie
//
function ReadShopCartCookie(){
var allcookies = document.cookie;
//alert(allcookies);
var  pos = allcookies.indexOf("ACXShopCart=");
	if (pos != -1){
		var start = pos+12;
		var end = allcookies.indexOf(";",start);
		if (end == -1) {end=allcookies.length;}
		var ShopCartString = allcookies.substring(start,end);
		ShopCartString = unescape(ShopCartString);
	}
	else{ShopCartString= "";}
//alert("Read cookie:\n " + ShopCartString);
return ShopCartString;
}

//////////////////////////////////////////////////////////////
//       Delete all items from Shopping Cart
//
function ClearShopCart(){
var cend= ShopcartWRS.getRowCount();
for (var jj=cend-1; jj >-1 ;jj--){
if (theMainFrame.prodshown){
theMainFrame.ClearBoxes(jj);
}
ShopcartWRS.deleteRow(jj);
} 
}

///////////////////////////////////////////////////////////////
//        Create Shopping Cart WDDX Recordset 
//
function FillWDDXShopCart(){
	bWriteShopCartToDb ? FillWDDXShopCartFromDB(): FillWDDXShopCartFromCookie();
}

///////////////////////////////////////////////////////////////
//        Create Shopping Cart WDDX Recordset from DB
//
function FillWDDXShopCartFromDB(){
	
	if (ShoppingCartWddxPacket == ""){
		MakeWDDXShopCart();
	}
	else{
		DeserializeShopCart(ShoppingCartWddxPacket);
	}
}

///////////////////////////////////////////////////////////////
//        Create Shopping Cart WDDX Recordset from Cookie values
//
function FillWDDXShopCartFromCookie(){
MakeWDDXShopCart();
var ShopCartString = ReadShopCartCookie();
	if (ShopCartString == ""){return false;}
	else{ 
		lineitemArray = ShopCartString.split("@")
			 for (var Row in lineitemArray){
				 	 ShopcartWRS.addRows(1);
				 	 fieldArray = lineitemArray[Row].split("!");
					 ShopcartWRS.prodid[Row] = fieldArray[0];
					 ShopcartWRS.vendorcode[Row]= fieldArray[1];
					 ShopcartWRS.vendorname[Row]= fieldArray[2];
					 ShopcartWRS.catnum[Row]= fieldArray[3];
					 ShopcartWRS.catnum2[Row] = " ";
					 ShopcartWRS.qty[Row]= fieldArray[4];
					 ShopcartWRS.description[Row]= fieldArray[5];
					 ShopcartWRS.unitmeas[Row]= fieldArray[6];
					 ShopcartWRS.unitprice[Row]= fieldArray[7];
					 ShopcartWRS.listprice[Row]= fieldArray[8];
					 ShopcartWRS.totalprice[Row]= fieldArray[9];
					 ShopcartWRS.cas[Row]= fieldArray[10];
					 ShopcartWRS.strucid[Row]= fieldArray[11];
					 ShopcartWRS.csymbol[Row]= fieldArray[12];
					 ShopcartWRS.exprt[Row]= fieldArray[13];
			 }
	}		 
}

///////////////////////////////////////////////////////////////////////////////
//             Add items to Shopping Cart WDDX Recordset
//			   Can be called from other windows (not just the shopping cart window)
function AddtoShopcartWRSRemote(qty,packid,supid,supname,catnum,descript,size,price,listprice,csymbol, cas, strucid){
	var itemexists=0;
	var currow=1;
	var i=0;
	//check if item exists in basket
	if (ShopcartWRS.getRowCount()> 0){ 
		for (i=0; i<ShopcartWRS.getRowCount(); i++){
			if (ShopcartWRS.prodid[i] == packid){
				qty= parseInt(ShopcartWRS.qty[i]) + qty;
				ShopcartWRS.qty[i] = qty ; itemexists=1; currow = i;
				break;
			}
		}
	}
	// since item is not in the cart, create a new item in ShopcartWRS
	if (!itemexists) {
		ShopcartWRS.addRows(1);
		var currow = ShopcartWRS.getRowCount()-1; 
		ShopcartWRS.prodid[currow] = packid;
		ShopcartWRS.vendorcode[currow] = supid;
		ShopcartWRS.vendorname[currow] = UnEscapeQuotes(supname);
		ShopcartWRS.qty[currow] = parseInt(qty);
		ShopcartWRS.description[currow] = UnEscapeQuotes(descript);
		ShopcartWRS.catnum[currow] = UnEscapeQuotes(catnum);
		ShopcartWRS.catnum2[currow] = " ";
		ShopcartWRS.unitmeas[currow] = UnEscapeQuotes(size);
		ShopcartWRS.unitprice[currow] = price;
		ShopcartWRS.listprice[currow] = listprice;
		ShopcartWRS.cas[currow] = cas;
		ShopcartWRS.strucid[currow] = strucid;
		ShopcartWRS.totalprice[currow] = listprice.valueOf() * parseInt(qty);
		ShopcartWRS.csymbol[currow] = csymbol;
		ShopcartWRS.exprt[currow] = false;
		WriteShopCart();
	}
}

///////////////////////////////////////////////////////////////////////////////
//             Add items to Shopping Cart WDDX Recordset
//
function AddtoShopcartWRS(cas, strucid, packid,supid,supname,catnum,descript,size,price,listprice,csymbol,type){
var itemexists=0;
var currow=1;
var i=0;
//check if item exists in basket
if (ShopcartWRS.getRowCount()> 0){ 
	for (i=0; i<ShopcartWRS.getRowCount(); i++){
		if (ShopcartWRS.prodid[i] == packid){
			// type=1 means check box was clicked
			if (type ==1 ){
				qty=ShopcartWRS.qty[i]+1;
			}
			{checkbox = eval("theDisplayFrame.document.dispform.orderedcheck_" + packid + ".checked");
				if (checkbox){
					qty=1;
				}
				else{
					qty='';
				}
		}
			// type=2 means a number was entered into input box
			if (type ==2) {
				qty = parseFloat(eval("theDisplayFrame.document.dispform.ordered_" + packid + ".value"));
			}
			ShopcartWRS.qty[i] = qty ; itemexists=1; currow = i;
			theMainFrame.AsignBoxVal(i);
			break;
	}
		}

}
// since item is not in the cart, create a new item in ShopcartWRS
if (!itemexists) {
		// type=1 means check box was clicked
		if (type ==1 ){qty=1;}  
		// type=2 means a number was entered into input box
		if (type ==2) {
			qty = eval("theDisplayFrame.document.dispform.ordered_" + packid + ".value");
		} 
		ShopcartWRS.addRows(1);
		var currow = ShopcartWRS.getRowCount()-1; 
		ShopcartWRS.prodid[currow] = packid;
		ShopcartWRS.vendorcode[currow] = supid;
		ShopcartWRS.vendorname[currow] = UnEscapeQuotes(supname);
		ShopcartWRS.qty[currow] = parseInt(qty);
		ShopcartWRS.description[currow] = UnEscapeQuotes(descript);
		ShopcartWRS.catnum[currow] = UnEscapeQuotes(catnum);
		ShopcartWRS.catnum2[currow] = " ";
		ShopcartWRS.unitmeas[currow] = UnEscapeQuotes(size);
		ShopcartWRS.unitprice[currow] = price;
		ShopcartWRS.listprice[currow] = listprice;
		ShopcartWRS.totalprice[currow] = listprice.valueOf() * parseInt(qty);
		ShopcartWRS.cas[currow] = cas;
		ShopcartWRS.strucid[currow] = strucid;
		ShopcartWRS.csymbol[currow] = csymbol;
		ShopcartWRS.exprt[currow] = false;
		theMainFrame.AsignBoxVal(currow);}
		WriteShopCart();
}

////////////////////////////////////////////////////////////////
//          Serialize the Shopping Cart to send to ChemACX.Com
//
function SerializeShopCart(){
var ShoppinglistAsWDDX;
MyWddxserializer = new WddxSerializer();
ShoppinglistAsWDDX = MyWddxserializer.serialize(ShopcartWRS);
return(ShoppinglistAsWDDX)
}

////////////////////////////////////////////////////////////////
//          Deserialize the Shopping Cart WDDX packet
//
function DeserializeShopCart(wddxPacket){
var MyWddxDeserializer = new WddxDeserializer();
ShopcartWRS = MyWddxDeserializer.deserialize(wddxPacket);
}


///////////////////////////////////////////////////////////
//       Open the shopping cart pop up window
//
function ViewCart(){
window.open('/chemacx/chemacx/shopcart_frset.html','ChemACX_Cart','toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1,width=750,height=550,left=60,top=60,xpos=60,ypos=60');
return false
}

//////////////////////////////////////////////////////////////
//		Removes single and double quotes from string
//
function EscapeQuotes(thestring){
	if (thestring == null) {
		escapedstring = "";
	}
	else{	
		escapedstring = thestring.replace(/'/gi,"@@").replace(/"/gi,"~~");
	}
return escapedstring;
}

//////////////////////////////////////////////////////////////
//		Regenerates single and double quotes from string
//
function UnEscapeQuotes(thestring){
unescapedstring = thestring.replace(/@@/gi,"'").replace(/~~/gi,String.fromCharCode(34));
return unescapedstring
}

// Turn a floating point number into price with two decimal places

			