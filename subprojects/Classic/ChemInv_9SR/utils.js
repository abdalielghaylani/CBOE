//Globals
var serverName = window.location.hostname;

// force any dialog windows to close when the opener closes
// I made the DilaogWindow object global to the page and
// added an onunload handler
var DialogWindow
 
window.onunload = function(){CloseDialog();}

function CloseDialog(){
	if (DialogWindow) DialogWindow.close();
}

///////////////////////////////////////////////////////////////////////////////
// Opens up a dialog box
// Type 1 is standard dialog used for funcitons like Move Container, ChangeQty etc
// Type 2 is the larger dialog used for Create/Edit Container and Substance selector
// Type 3 is the location browser dialog used from the Browse link
// The size and positions of the popups has been optimized to look Ok even at 800 X 600 resolution
function OpenDialog(url, name, type)
{
	WindowDef_1 = "height=530, width= 530px, top=50, left=0";
	WindowDef_2 = "height=580, width= 850px, top=0, left=0";
	WindowDef_3 = "height=450, width= 300px, top=50, left=540";
	WindowDef_4 = "height=450, width= 550px, top=50, left=200";
	WindowDef_5 = "height=600, width= 800px, top=0, left=100";		
	var WindowDef = eval("WindowDef_" + type);
	var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable," + WindowDef;
	DialogWindow = window.open(url,name,attribs);
	return DialogWindow;
}



//////////////////////////////////////////////////////////////////////////////////////////
// Generic Cookie Reader Function
// Returns empty string if cookie not found
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
		//alert(out);
		return out;
	}
	else { 
		var out = "";
		return out;
	}
	
}

//////////////////////////////////////////////////////////////////////////////////////////
// Generic Cookie Writer Function
// Cookie expires in 1 year
function WriteCookie(name, value){
nextyear = new Date();
nextyear.setFullYear(nextyear.getFullYear() +1);
thecookie = name + "=" + value + "; expires=" + nextyear.toGMTString();
document.cookie =  thecookie; 
//alert("Saved cookie:\n" + thecookie);
}


////////////////////////////////////////////////////////////////////////////
// Opens a tree dialog to pick a location
// elm1 receives the location id of the chosen location
// elm2 receives the location name of the chosen locaiton
// dl is the location to which the tree should be opened to
	function PickLocation(id, dl, islocationSearch, formelm, elm1, elm2, elm3){
		var ls = "" 
		if (islocationSearch) ls = "MaybeLocSearch=true&";		
		if (formelm) ls += "formelm=" + formelm + "&";
		if (elm1) ls += "elm1=" + elm1 + "&"; 
		if (elm2) ls += "elm2=" + elm2 + "&";
		if (elm3) ls += "elm3=" + elm3 + "&";
		OpenDialog('/cheminv/cheminv/BrowseTree.asp?' + ls + 'LocationPickerID=' + id + '&TreeID=2&ClearNodes=1&GotoNode=' + dl + '&Node=' + dl + '&sNode=' + dl + '&Exp=Y#' + dl, 'NewLocDiag', 3);
	}


////////////////////////////////////////////////////////////////////////////
// Opens a tree dialog to pick a plate map location
// elm1 receives the location id of the chosen location
// elm2 receives the location name of the chosen locaiton
// dl is the location to which the tree should be opened to
	function PickPlateMapLocation(id, dl, islocationSearch, formelm, elm1, elm2, elm3){
		var ls = "" 
		if (islocationSearch) ls = "MaybeLocSearch=true&";		
		if (formelm) ls += "formelm=" + formelm + "&";
		if (elm1) ls += "elm1=" + elm1 + "&"; 
		if (elm2) ls += "elm2=" + elm2 + "&";
		if (elm3) ls += "elm3=" + elm3 + "&";
		OpenDialog('/cheminv/cheminv/BrowseTree.asp?' + ls + 'LocationPickerID=' + id + '&TreeID=3&ClearNodes=1&GotoNode=' + dl + '&Node=' + dl + '&sNode=' + dl + '&Exp=Y#' + dl, 'NewLocDiag', 3);
	}
	
///////////////////////////////////////////////////////////////////////
//	Updates EditorCreateWindow with a newly selected CompoundID
//
//	
function DoSelectSubstance(NewCompoundID, NewCompoundName){
		var SN = "";
		//if (this.document.cows_input_form.SubstanceName){
		//	SN = this.document.cows_input_form.SubstanceName.value;
		//}
		if (top.opener.document.form1.iCompoundID){
			top.opener.document.form1.iCompoundID.value=NewCompoundID;
			top.opener.document.form1.NewCompoundID.value=NewCompoundID;
			if (top.opener.document.form1.iContainerName.value.length == 0) top.opener.document.form1.iContainerName.value = NewCompoundName;
			if (top.opener.document.form1.RegBatchID) top.opener.document.form1.RegBatchID.value= "";
			top.opener.document.form1.iRegID.value= "";
			top.opener.document.form1.iBatchNumber.value="";
			}
		if (top.opener.document.form1.isSubstanceTab) {top.opener.location.href= top.opener.location.pathname + "?newCompoundID=" + NewCompoundID}; 
		top.opener.focus();
		top.close();
	}
	
///////////////////////////////////////////////////////////////////////
//	Updates EditorCreateWindow with a newly selected CompoundID
//
//	
function AssignRegCompound(RegID, BatchNumber, RegNumber){
		//alert(RegID + " " +  BatchNumber + " " + RegNumber)
		var SN = "";
		if (this.document.cows_input_form.SubstanceName){
			SN = this.document.cows_input_form.SubstanceName.value;
		}
		if (top.opener.document.form1.RegBatchID){
			top.opener.document.form1.iCompoundID.value="";
			top.opener.document.form1.NewCompoundID.value="";
			top.opener.document.form1.RegBatchID.value= RegNumber + "-" + BatchNumber;
			if (SN.length > 0) {
			top.opener.document.form1.iContainerName.value= SN;
			}
			top.opener.GetRegIDFromRegNum(RegNumber + "-" + BatchNumber)
			}
		
		if (top.opener.document.form1.isSubstanceTab) {top.opener.location.href= top.opener.location.pathname + "?newRegID=" + RegID + "&newBatchNumber=" + BatchNumber}; 
		top.opener.focus();
		top.close();
	}


	
//////////////////////////////////////////////////////////////////////
// Update Location Picker from a Location Barcode
//
function UpdateLocationPickerFromBarCode(barCode, formelm, elm1, elm2, elm3){
	var strURL = "http://" + serverName + "/cheminv/api/GetLocationFromBarcode.asp?LocationBarcode=" + barCode;
	var httpResponse = JsHTTPGet(strURL) 
	//alert(httpResponse)
	if (httpResponse.length == 0){
		formelm[elm1].value = "";
		formelm[elm2].value = "";
		formelm[elm3].value = "";
		alert("Could not find a location for barcode= " + barCode);
	}
	else{
		var temp = httpResponse.split(",");
		formelm[elm1].value = temp[0];
		formelm[elm2].value = temp[1];
		formelm[elm3].value = temp[2];
		if (typeof SetLocationSQL == "function") SetLocationSQL(temp[0]);	
	}
}

//////////////////////////////////////////////////////////////////////
//	Update location picker from a Location ID
//	
function UpdateLocationPickerFromID(LocationID, formelm, elm1, elm2, elm3){
	if (LocationID =="") return;
	var strURL = "http://" + serverName + "/cheminv/api/GetLocationFromID.asp?LocationID=" + LocationID;	
	var httpResponse = JsHTTPGet(strURL) 
	//alert(httpResponse)
	if (httpResponse.length == 0){
		formelm[elm1].value = "";
		formelm[elm2].value = "";
		formelm[elm3].value = "";
		alert("Invalid location id= " + LocationID);
	}
	else{
		var temp = httpResponse.split(",");
		formelm[elm1].value = temp[0];
		formelm[elm2].value = temp[1];
		formelm[elm3].value = temp[2];
		if (typeof SetLocationSQL == "function") SetLocationSQL(LocationID);	
	}
}


function IsKnownContainerBarcode(ContainerBarcode){
	var strURL = "http://" + serverName + "/cheminv/api/GetKeyContainerAttributes.asp?ContainerBarcode='" + ContainerBarcode + "'"; 	
	var httpResponse = JsHTTPGet(strURL)
	//alert("JSHTTP= " + httpResponse)
	if (httpResponse.length == 0){
		return 0;
	}
	else{
		return 1;
	} 
}

function CheckDuplicateBarcode(barcodes, barcodeType){
	var strURL = "http://" + serverName + "/cheminv/api/CheckDuplicateBarcode.asp?barcodes=" + barcodes + "&barcodeType=" + barcodeType; 	
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);
	if (httpResponse.length == 0){
		return 0;
	}
	else{
		return 1;
	} 
}

function IsValidCompoundID(compoundID, bAlert)
{
	var strURL = "http://" + serverName + "/cheminv/api/IsValidCompoundID.asp?compoundID=" + compoundID; 	
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);
	if (httpResponse == 1){
		return 1;
	}
	else{
		if(bAlert == true) {
			alert("Invalid Compound ID.");
		}
		return 0;
	} 

}

function GetRegIDFromRegNum(RegBatchID){

	//var temp = RegBatchId.split("-");
	var dash = RegBatchID.lastIndexOf("-");
	if (dash == -1){
		alert("Registry Batch ID should be entered as RegNumber-BatchNumber");
		return false;
	}
	var RegNumber = RegBatchID.substring(0, dash);
	var BatchNumber = RegBatchID.substring(dash+1,RegBatchID.length);
	//alert("RegNumber=" + RegNumber)
	//alert("Batchnumber=" + BatchNumber)
	if (BatchNumber == "") BatchNumber = 0 
	var strURL = "http://" + serverName + "/cheminv/api/GetBatchInfo.asp?RegNumber=" + RegNumber + "&BatchNumber=" + BatchNumber; 	
	var httpResponse = JsHTTPGet(strURL)
	//alert(httpResponse)
	if (httpResponse.length == 0){
		document.form1.RegBatchID.value = ""
		document.form1.iRegID.value = ""
		document.form1.iBatchNumber.value = ""
		document.form1.NewRegID.value = ""
		document.form1.NewBatchNumber.value = ""
		alert("Unable to retrieve batch data for Reg Number= " + RegNumber + " and Batch Number= " + BatchNumber + "\r\rRegistry Batch ID should be entered as RegNumber-BatchNumber");
	}
	else{
		var temp = httpResponse.split(",");
		document.form1.RegBatchID.value = RegBatchID;
		document.form1.iRegID.value = temp[6];
		document.form1.iBatchNumber.value = temp[7];
		document.form1.NewRegID.value = temp[6];
		document.form1.NewBatchNumber.value = temp[7];
	} 
}

function UpdateBatchInfo(RegID, BatchNum){
	var strURL = "http://" + serverName + "/cheminv/api/GetBatchInfo.asp?RegID=" + RegID + "&BatchNumber=" + BatchNum;	
	var httpResponse = JsHTTPGet(strURL) 
	//alert(httpResponse)
	if (httpResponse.length == 0){
		document.all["RegNum_" + RegID].innerHTML = "";
		document.all["NoteBookName_" + RegID].innerHTML = "";
		document.all["NoteBookPage_" + RegID].innerHTML = "";
		document.all["Chemist_" + RegID].innerHTML = ""
		document.all["UOMID_" + RegID].innerHTML = ""
		document.form1["ContainerSize_" + RegID].value = ""
		document.form1["InitialAmount_" + RegID].value = ""
		alert("Unable to retrieve batch data for " + RegID + "-" + BatchNum);
	}
	else{
		var temp = httpResponse.split(",");
		
		document.all["RegNum_" + RegID].innerHTML = Cnv2nbs(temp[0]);
		document.all["NoteBookName_" + RegID].innerHTML = Cnv2nbs(temp[1]);
		document.all["NoteBookPage_" + RegID].innerHTML = Cnv2nbs(temp[2]);
		document.all["Chemist_" + RegID].innerHTML = Cnv2nbs(temp[3]);
		document.form1["UOMID_" + RegID].value = ConvertReg2InvUnits(Cnv2nbs(temp[4]));
		document.form1["ContainerSize_" + RegID].value = ConvertToNum(Cnv2nbs(temp[5]));
		document.form1["InitialAmount_" + RegID].value = document.form1["ContainerSize_" + RegID].value;
	}

}

function UpdateBatchInfo2(RegID, BatchNum){
	var strURL = "http://" + serverName + "/cheminv/api/GetBatchInfo.asp?RegID=" + RegID + "&BatchNumber=" + BatchNum;	
	var httpResponse = JsHTTPGet(strURL);
	//alert(httpResponse)
	if (httpResponse.length == 0){
		document.all["RegNum_" + RegID + "_" + BatchNum].innerHTML = "";
		document.all["NoteBookName_" + RegID + "_" + BatchNum].innerHTML = "";
		document.all["NoteBookPage_" + RegID + "_" + BatchNum].innerHTML = "";
		document.all["Chemist_" + RegID + "_" + BatchNum].innerHTML = ""
		document.all["UOMID_" + RegID + "_" + BatchNum].innerHTML = ""
		document.form1["ContainerSize_" + RegID + "_" + BatchNum].value = ""
		document.form1["InitialAmount_" + RegID + "_" + BatchNum].value = ""
		alert("Unable to retrieve batch data for " + RegID + "-" + BatchNum);
	}
	else{
		var temp = httpResponse.split(",");
		document.all["RegNum_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[0]);
		document.all["NoteBookName_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[1]);
		document.all["NoteBookPage_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[2]);
		document.all["Chemist_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[3]);
		document.all["UOMID_" + RegID + "_" + BatchNum].value = ConvertReg2InvUnits(Cnv2nbs(temp[4]));
		document.form1["ContainerSize_" + RegID + "_" + BatchNum].value = ConvertToNum(Cnv2nbs(temp[5]));
		document.form1["InitialAmount_" + RegID + "_" + BatchNum].value = document.form1["ContainerSize_" + RegID + "_" + BatchNum].value;
	}
}
function ConvertToNum(val){
	val = parseInt(val);
	if (isNaN(val)) return "";
	return val
}

function ConvertReg2InvUnits(val){
	if (val == "g") return 5;
	if (val == "mg") return 6;
	if (val == "kg") return 7;
	if (val == "mL") return 1;
	if (val == "L") return 2;
	if (val == "ug") return 8;
	return "";
}

function Cnv2nbs(val){
	if (val == "") {
	  return "&nbsp;";
	}
	else{
		return val;
	}  
}

function trim(str)
{
   return str.replace(/^\s*|\s*$/g,"");
}

/////////////////////////////////////////////////////////////////////
//	GetHTTP Content using msxml
//	
function JsHTTPGet(strURL){
	var objXML = new ActiveXObject("Msxml2.XMLHTTP"); 
	objXML.open("GET", strURL, false);
	objXML.send(); 
	strResponse = objXML.responseText;
	return strResponse;
}

/////////////////////////////////////////////////////////////////////
//	Get XML Content via msxml HTTP
//	Returns the xmlhttp object
function JsXMLHTTP(strMethod, strURL, strData){
	var objXmlHttp = new ActiveXObject("Msxml2.XMLHTTP"); 
	objXmlHttp.open(strMethod, strURL, false);
	objXmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	objXmlHttp.setRequestHeader("User-Agent", "ChemInv")
	objXmlHttp.send(strData); 
	return objXmlHttp;
}

/////////////////////////////////////////////////////////////////////
//	URL Encode a string
//	Returns the encoded string
function URLEncode(strURL){
	//not fully implemented
	strURL = strURL.replace(/\+/g,"%2B");
	strURL = strURL.replace(/\-/g,"%2D");
	return strURL
}

		