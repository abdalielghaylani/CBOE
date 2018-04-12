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
			top.opener.document.form1.RegID.value= "";
			top.opener.document.form1.BatchNumber.value="";
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
		var SN = "";
		if (this.document.cows_input_form.SubstanceName){
			SN = this.document.cows_input_form.SubstanceName.value;
		}
		if (top.opener.document.form1.RegBatchID){
			top.opener.document.form1.iCompoundID.value="";
			top.opener.document.form1.NewCompoundID.value="";
			top.opener.document.form1.RegBatchID.value=RegNumber + "-" + BatchNumber;
			if (SN.length > 0) {
				top.opener.document.form1.iContainerName.value=SN;
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
		alert("Could not a find a location for barcode= " + barCode);
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
	if (httpResponse.length == 0){
		formelm[elm1].value = "";
		formelm[elm2].value = "";
		formelm[elm3].value = "";
		alert("Invalid location id= " + LocationID);
	}
	else{
		var temp = httpResponse.split(",");
		if (formelm[elm1]){
			formelm[elm1].value = temp[0];
		}
		if (formelm[elm2]){
			formelm[elm2].value = temp[1];
		}
		if (formelm[elm3]){
			formelm[elm3].value = temp[2];
		}
		var isRack = temp[3];
		if (typeof SetLocationSQL == "function"){
			SetLocationSQL(LocationID);	
		}
	}
}


//////////////////////////////////////////////////////////////////////
//	Update location picker from a Location ID
//	
function ValidateLocationIDType(LocationID){
	if (LocationID =="") return;
	var strURL = "http://" + serverName + "/cheminv/api/GetLocationFromID.asp?LocationID=" + LocationID;	
	var httpResponse = JsHTTPGet(strURL) 
	//alert(httpResponse)
	if (httpResponse.length == 0){
		alert("Invalid location id= " + LocationID);
	}
	else{
		var temp = httpResponse.split(",");
		var isRack = temp[3];
		if (parseInt(isRack)==1){
			return 1;
		} else {
			return 0;
		}
	}
}


function GetFirstOpenRackPosition(LocationID){
	if (LocationID =="") return;
	var strURL = "http://" + serverName + "/cheminv/api/GetFirstOpenRackPosition.asp?LocationID=" + LocationID;	
	var httpResponse = JsHTTPGet(strURL) 
	//alert(httpResponse)
	if (httpResponse.length == 0){
		alert("Invalid location id= " + LocationID);
	}
	else{
		return httpResponse;
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

function GetRegIDFromRegNum(RegBatchID){
	if (RegBatchID.length > 0) {
		//var temp = RegBatchId.split("-");
		var dash = RegBatchID.lastIndexOf("-");
		if (dash == -1){
			alert("Registry Batch ID should be entered as RegNumber-BatchNumber");
			return false;
		}
		var RegNumber = RegBatchID.substring(0, dash);
		var BatchNumber = RegBatchID.substring(dash+1,RegBatchID.length);
		if (BatchNumber == "") BatchNumber = 0 
		if (!isNumber(BatchNumber)) {
			alert("The batch number is invalid");
			document.form1.RegBatchID.value = ""
			document.form1.iRegID.value = ""
			document.form1.iBatchNumber.value = ""
			document.form1.NewRegID.value = ""
			document.form1.NewBatchNumber.value = ""
		} else {
			var strURL = "http://" + serverName + "/cheminv/api/GetBatchInfo.asp?RegNumber=" + RegNumber + "&BatchNumber=" + BatchNumber;
			var httpResponse = JsHTTPGet(strURL)
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
	} else {
		document.form1.RegBatchID.value = ""
		document.form1.iRegID.value = ""
		document.form1.iBatchNumber.value = ""
		document.form1.NewRegID.value = ""
		document.form1.NewBatchNumber.value = ""
	}

}

function GetRegIDFromRegNumValidateOnly(RegBatchID){

	if (RegBatchID.length > 0) {
		//var temp = RegBatchId.split("-");
		var dash = RegBatchID.lastIndexOf("-");
		if (dash == -1){
			//alert("Registry Batch ID should be entered as RegNumber-BatchNumber");
			document.form1.RegNumFinalValidate.value = 1;
			document.form1.RegNumFinalValidateMsg.value = "Registry Batch ID should be entered as RegNumber-BatchNumber";
			return false;
		}
		var RegNumber = RegBatchID.substring(0, dash);
		var BatchNumber = RegBatchID.substring(dash+1,RegBatchID.length);
		if (!isNumber(BatchNumber)) {
			//alert("The batch number is invalid");
			document.form1.RegNumFinalValidate.value = 1;
			document.form1.RegNumFinalValidateMsg.value = "The batch number is invalid";
			document.form1.RegBatchID.value = ""
			document.form1.iRegID.value = ""
			document.form1.iBatchNumber.value = ""
			document.form1.NewRegID.value = ""
			document.form1.NewBatchNumber.value = ""
		} else {
			if (BatchNumber == "") BatchNumber = 0 
			var strURL = "http://" + serverName + "/cheminv/api/GetBatchInfo.asp?RegNumber=" + RegNumber + "&BatchNumber=" + BatchNumber;
			var httpResponse = JsHTTPGet(strURL)
			if (httpResponse.length == 0){
				document.form1.RegBatchID.value = ""
				document.form1.iRegID.value = ""
				document.form1.iBatchNumber.value = ""
				document.form1.NewRegID.value = ""
				document.form1.NewBatchNumber.value = ""
				document.form1.RegNumFinalValidate.value = 1;
				document.form1.RegNumFinalValidateMsg.value = "Unable to retrieve batch data for Reg Number= " + RegNumber + " and Batch Number= " + BatchNumber + "\r\rRegistry Batch ID should be entered as RegNumber-BatchNumber";
				//alert();
			}
		}
	} else {
		document.form1.RegBatchID.value = ""
		document.form1.iRegID.value = ""
		document.form1.iBatchNumber.value = ""
		document.form1.NewRegID.value = ""
		document.form1.NewBatchNumber.value = ""
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

function ValidateBarcode(barcode){
	var Cnt = 0;
	var strURL = "http://" + serverName + "/cheminv/api/ValidateBarcode.asp?barcode=" + barcode;
	var httpResponse = JsHTTPGet(strURL);
	var temp = httpResponse.split(",");
	var Cnt = temp[0];
	return Cnt;
}

function ValidateUniqueName(nametype, name){
	var Cnt = 0;
	var strURL = "http://" + serverName + "/cheminv/api/ValidateUniqueName.asp?nametype=" + nametype + "&name=" + name;
	var httpResponse = JsHTTPGet(strURL);
	var temp = httpResponse.split(",");
	var Cnt = temp[0];
	return Cnt;
}

function GetOpenRackPositons(rackid,startpos){
	var strURL = "http://" + serverName + "/cheminv/api/GetOpenRackPositions.asp?RackID=" + rackid + "&StartPos=" + startpos;
	var httpResponse = JsHTTPGet(strURL);
	return httpResponse;
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

/////////////////////////////////////////////////////////////////////
//	Changes CSS elements for a passed class
/////////////////////////////////////////////////////////////////////
function AlterCSS(theClass,element,value) {
 var cssRules;
 if (document.all) {
  cssRules = 'rules';
 }
 else if (document.getElementById) {
  cssRules = 'cssRules';
 }
 for (var i = 0; i < document.styleSheets.length; i++){
  for (var j = 0; j < document.styleSheets[i][cssRules].length; j++) {
   if (document.styleSheets[i][cssRules][j].selectorText == theClass) {
    document.styleSheets[i][cssRules][j].style[element] = value;
   }
  }
 }	
}
	
/////////////////////////////////////////////////////////////////////
//	Changes inner HTML for element
/////////////////////////////////////////////////////////////////////
function changeContent(docElement,text) {
    if (document.all)
        docElement.innerHTML = text;
    else if (document.layers) {
        docElement.document.open();
        docElement.document.write(text);
        docElement.document.close();
    }
}


/////////////////////////////////////////////////////////////////////
//	Selects and displays default RackID and Rack position
/////////////////////////////////////////////////////////////////////
function selectRackDefaults(){
	var selectedRackIndex = 0;

	if (document.form1.SelectRackBySearch) {
		if (document.form1.SelectRackBySearch.checked) {
			rackField = document.form1.SuggestedRackID;
			//alert("here");
		}else{
			rackField = document.form1.RackID;
		}
	} else {
		rackField = document.form1.RackID;
	}

	//alert(rackField.selectedIndex);
	if(rackField.selectedIndex != -1){
		//alert("here: " + document.form1.RackID.selectedIndex);
		selectedRackIndex = rackField.selectedIndex;
	}
	var defRackInfo = "";
	rackField.options[selectedRackIndex].selected = true;
	if(rackField.options[selectedRackIndex].value != 'NULL'){
		//eval("defRackInfo = document.form1.Rack" + document.form1.RackID.options[selectedRackIndex].value + "Def.value;");
		defRackInfo = GetFirstOpenRackPosition(rackField.options[selectedRackIndex].value);
		var arrDefRackInfo = defRackInfo.split("::");
		var strRackInfo = rackField.options[selectedRackIndex].text;
		var arrRackInfo = strRackInfo.split("::");
		document.form1.iRackGridID.value = arrDefRackInfo[0];
		document.form1.iRackGridName.value = arrDefRackInfo[1] + " in " + arrRackInfo[0];
		if (document.form1.RackGridList) {
			document.form1.RackGridList.value = arrDefRackInfo[0];
		}
		if (document.form1.LocationName && document.form1.AssignToRack.checked == true && document.form1.LocationName.length == 0){
			document.form1.LocationName.value = arrDefRackInfo[1];
		}
		
	}
}

function IsWholeNumber(sText)
{
   var ValidChars = "0123456789";
   var IsNumber=true;
   var Char;

 
   for (i = 0; i < sText.length && IsNumber == true; i++) 
      { 
      Char = sText.charAt(i); 
      if (ValidChars.indexOf(Char) == -1) 
         {
         IsNumber = false;
         }
      }
   return IsNumber;
   
}

function checkAll(field)
{
for (i = 0; i < field.length; i++)
	field[i].checked = true ;
}

function uncheckAll(field)
{
for (i = 0; i < field.length; i++)
	field[i].checked = false ;
}

   
function addToList(listField, newText, newValue) {
   if ( ( newValue == "" ) || ( newText == "" ) ) {
      alert("You cannot add blank values");
   } else {
      var len = listField.length++;
      listField.options[len].value = newValue;
      listField.options[len].text = newText;
      listField.selectedIndex = len;
   }
}

function removeFromList(listField) {
   if ( listField.length == -1) {
      //alert("There are no values which can be removed!");
   } else {
      var selected = listField.selectedIndex;
      if (selected == -1) {
         alert("You must select an entry to be removed!");
      } else {
         var replaceTextArray = new Array(listField.length-1);
         var replaceValueArray = new Array(listField.length-1);
         for (var i = 0; i < listField.length; i++) {
            // Put everything except the selected one into the array
            if ( i < selected) { replaceTextArray[i] = listField.options[i].text; }
            if ( i > selected ) { replaceTextArray[i-1] = listField.options[i].text; }
            if ( i < selected) { replaceValueArray[i] = listField.options[i].value; }
            if ( i > selected ) { replaceValueArray[i-1] = listField.options[i].value; }
         }
         listField.length = replaceTextArray.length;
         for (i = 0; i < replaceTextArray.length; i++) {
            listField.options[i].value = replaceValueArray[i];
            listField.options[i].text = replaceTextArray[i];
         }
      }
   } 
}

