//Globals
/*CSBR# 139460 
 Purpose: To change servername to include portnumber information
*/
var serverName = window.location.host; //End of CHange
var serverType = String(window.location);
serverType = serverType.substring(0, serverType.indexOf(serverName));

// force any dialog windows to close when the opener closes
// I made the DilaogWindow object global to the page and
// added an onunload handler
var DialogWindow

window.onunload = function () { CloseDialog(); }

function CloseDialog() {
	if (DialogWindow == "object") DialogWindow.close();
}

///////////////////////////////////////////////////////////////////////////////
// Opens up a dialog box
// Type 1 is standard dialog used for funcitons like Move Container, ChangeQty etc
// Type 2 is the larger dialog used for Create/Edit Container and Substance selector
// Type 3 is the location browser dialog used from the Browse link
// The size and positions of the popups has been optimized to look Ok even at 800 X 600 resolution
function OpenDialog(url, name, type) {
	WindowDef_1 = "height=530, width= 530px, top=50, left=0";
	WindowDef_2 = "height=580, width= 850px, top=0, left=0";
	WindowDef_3 = "height=450, width= 300px, top=50, left=540";
	WindowDef_4 = "height=450, width= 550px, top=50, left=200";
	WindowDef_5 = "height=600, width= 800px, top=0, left=100";
	WindowDef_6 = "height=450, width= 550px, top=50, left=540";
	var WindowDef = eval("WindowDef_" + type);
	var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable," + WindowDef;
	DialogWindow = window.open(url, name, attribs);
	return DialogWindow;
}



//////////////////////////////////////////////////////////////////////////////////////////
// Generic Cookie Reader Function
// Returns empty string if cookie not found
function ReadCookie(cookiename) {
	var allcookies = unescape(document.cookie);      //JHS picking up fix from which was in app_js CSBR 148189 SJ Resolving the location id issue in the search page
	var pos = allcookies.indexOf(cookiename + "=");
	if (pos != -1) {
		var start = pos + cookiename.length + 1;
		var end = allcookies.indexOf(";", start);
		if (end == -1) {
			end = allcookies.length;
		}
		var cookiestr = allcookies.substring(start, end); //CSBR 148189
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
function WriteCookie(name, value) {
	nextyear = new Date();
	nextyear.setFullYear(nextyear.getFullYear() + 1);
	thecookie = name + "=" + value + "; expires=" + nextyear.toGMTString();
	document.cookie = thecookie;
	//alert("Saved cookie:\n" + thecookie);
}


////////////////////////////////////////////////////////////////////////////
// Opens a tree dialog to pick a location
// elm1 receives the location id of the chosen location
// elm2 receives the location name of the chosen locaiton
// dl is the location to which the tree should be opened to
function PickLocation(id, dl, islocationSearch, formelm, elm1, elm2, elm3) {
	var ls = ""
	if (islocationSearch) ls = "MaybeLocSearch=true&";
	if (formelm) ls += "formelm=" + formelm + "&";
	if (elm1) ls += "elm1=" + elm1 + "&";
	if (elm2) ls += "elm2=" + elm2 + "&";
	if (elm3) ls += "elm3=" + elm3 + "&";
	if (readCookie2("COESSO") != "") {
		ls += "ticket=" + readCookie2("COESSO") + "&";
	}
	OpenDialog('/cheminv/cheminv/BrowseTree.asp?' + ls + 'LocationPickerID=' + id + '&TreeID=2&ClearNodes=1&GotoNode=' + dl + '&Node=' + dl + '&sNode=' + dl + '&Exp=Y#' + dl, 'NewLocDiag', 6);
}
////////////////////////////////////////////////////////////////////////////
// This an enhancement of above function with an extra parameter isMultiSelectRacks
// which is used whether browsetree will show MultiSelect Racks or Not
function PickLocation1(id, dl, islocationSearch, formelm, elm1, elm2, elm3, isMultiSelectRacks) {
	var ls = ""
	if (islocationSearch) ls = "MaybeLocSearch=true&";
	if (formelm) ls += "formelm=" + formelm + "&";
	if (elm1) ls += "elm1=" + elm1 + "&";
	if (elm2) ls += "elm2=" + elm2 + "&";
	if (elm3) ls += "elm3=" + elm3 + "&";
	ls += "isMultiSelectRacks=" + isMultiSelectRacks + "&";
	OpenDialog('/cheminv/cheminv/BrowseTree.asp?' + ls + 'LocationPickerID=' + id + '&TreeID=2&ClearNodes=1&GotoNode=' + dl + '&Node=' + dl + '&sNode=' + dl + '&Exp=Y#' + dl, 'NewLocDiag', 6);
}
////////////////////////////////////////////////////////////////////////////
// Opens a tree dialog to pick a plate map location
// elm1 receives the location id of the chosen location
// elm2 receives the location name of the chosen locaiton
// dl is the location to which the tree should be opened to
function PickPlateMapLocation(id, dl, islocationSearch, formelm, elm1, elm2, elm3) {
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
function DoSelectSubstance(NewCompoundID, NewCompoundName) {
	var url = document.URL;
	var loc = document.location;
	var test = url.indexOf('CreateSubstance3.asp');
	if (test > -1) {
		document.location = "confirmCompoundSelection.asp?NewCompoundID=" + NewCompoundID
		return false;
	}
	var SN = "";
	//if (this.document.cows_input_form.SubstanceName){
	//	SN = this.document.cows_input_form.SubstanceName.value;
	//}
	if (top.opener.document.form1.iCompoundID) {
		if (top.opener.document.form1.iCompoundID) top.opener.document.form1.iCompoundID.value = NewCompoundID;
		if (top.opener.document.form1.NewCompoundID) top.opener.document.form1.NewCompoundID.value = NewCompoundID;
		if (top.opener.document.form1.iContainerName.value.length == 0) top.opener.document.form1.iContainerName.value = NewCompoundName;
		if (top.opener.document.form1.RegBatchID) top.opener.document.form1.RegBatchID.value = "";
		if (top.opener.document.form1.NewRegID) top.opener.document.form1.NewRegID.value = "";
		if (top.opener.document.form1.iRegID) top.opener.document.form1.iRegID.value = "";
		if (top.opener.document.form1.iBatchNumber) top.opener.document.form1.iBatchNumber.value = "";
		if (top.opener.document.form1.icompound_id_fk) top.opener.document.form1.icompound_id_fk.value = NewCompoundID;
	}
	if (top.opener.document.form1.isSubstanceTab) { top.opener.location.href = top.opener.location.pathname + "?newCompoundID=" + NewCompoundID };
	top.opener.focus();
	top.close();
}

///////////////////////////////////////////////////////////////////////
//	Updates EditorCreateWindow with a newly selected CompoundID
//
//	
function AssignRegCompound(RegID, BatchNumber, RegNumber) {
	var SN = "";
	var REGBATCHID = "";
	if (this.document.cows_input_form.SubstanceName) {
		SN = this.document.cows_input_form.SubstanceName.value;
	}
	if (top.opener.document.form1.RegBatchID) {
		top.opener.document.form1.iCompoundID.value = "";
		top.opener.document.form1.NewCompoundID.value = "";

		// getting RegbBatchID
		var CsUserName = document.forms[0].tempCsUserName.value;
		var CsUserID = document.forms[0].tempCsUserID.value;
		var strURL = serverType + serverName + "/cheminv/api/GetRegistrationAttributes.asp?Action=GETREGBATCHID&regid=" + RegID + "&BatchNumber=" + BatchNumber + "&tempCsUserName=" + CsUserName + "&tempCsUserID=" + CsUserID;
		var httpResponse = JsHTTPGet(strURL);
		if (httpResponse.length == 0) {
			REGBATCHID = RegNumber + "-" + BatchNumber;
		}
		else {
			REGBATCHID = httpResponse;
		}

		top.opener.document.form1.RegBatchID.value = REGBATCHID;

		if ((SN.length > 0) && (top.opener.document.form1.iContainerName.value.length == 0)) {
			top.opener.document.form1.iContainerName.value = SN;
		}
		top.opener.GetRegIDFromRegNum(REGBATCHID)
	}

	if (top.opener.document.form1.isSubstanceTab) { top.opener.location.href = top.opener.location.pathname + "?newRegID=" + RegID + "&newBatchNumber=" + BatchNumber };
	top.opener.focus();
	top.close();
}
//////////////////////////////////////////////////////////////////////
// Update Location Picker from a Location Barcode
//
function UpdateLocationPickerFromBarCode(barCode, formelm, elm1, elm2, elm3) {
	var CsUserName = document.getElementById("tempCSUserName").value;
	var CsUserID = document.getElementById("tempCsUserID").value;
	var strURL = serverType + serverName + "/cheminv/api/GetLocationFromBarcode.asp?LocationBarcode=" + barCode + "&tempCsUserName=" + CsUserName + "&tempCsUserID=" + CsUserID;
	var httpResponse = JsHTTPGet(strURL)
	//alert("'" + httpResponse + "'")
	if (httpResponse.length == 0) {
		formelm[elm1].value = "";
		formelm[elm2].value = "";
		formelm[elm3].value = "";
		formelm["isRack"].value = "0";
		alert("Could not a find a location for barcode= " + barCode);
	}
	else {
		var temp = httpResponse.split(",");
		formelm[elm1].value = temp[0];
		formelm[elm2].value = temp[1];
		formelm[elm3].value = temp[2];
		if (document.getElementById('LocationPathSpan') != null) {
			document.getElementById('LocationPathSpan').title = temp[4];
		}
		if (formelm["isRack"]) {
			formelm["isRack"].value = temp[3];
		}
		if (formelm["isPublic"]) {
			formelm["isPublic"].value = temp[5];
			ChangeOwnerShipVisiblity();
		}
		if (typeof SetLocationSQL == "function") SetLocationSQL(temp[0]);
	}
}

//////////////////////////////////////////////////////////////////////
//	Update location picker from a Location ID
//	
function UpdateLocationPickerFromID(LocationID, formelm, elm1, elm2, elm3) {
	if (LocationID == "") return;
	if (String(LocationID).indexOf(',') > 0) {
		UpdateLocationPickerFromIDs(LocationID, formelm, elm1, elm2, elm3);
	}
	else {
		var randomnumber = Math.floor(Math.random() * 11);
		var strURL = serverType + serverName + "/cheminv/api/GetLocationFromID.asp?LocationID=" + LocationID + "&randomnumber=" + randomnumber;
		var httpResponse = JsHTTPGet(strURL)
		if (httpResponse.length == 0) {
			formelm[elm1].value = "";
			formelm[elm2].value = "";
			formelm[elm3].value = "";
			formelm["isRack"].value = "0";
			alert("Invalid location id= " + LocationID);
		}
		else {
			var temp = httpResponse.split(",");
			if (formelm[elm1]) {
				formelm[elm1].value = temp[0];
			}
			if (formelm[elm2]) {
				formelm[elm2].value = temp[1];
			}
			if (formelm[elm3]) {
				formelm[elm3].value = temp[2];
			}
			if (formelm["isRack"]) {
				formelm["isRack"].value = temp[3];
			}
			if (document.getElementById('LocationPathSpan') != null) {
				document.getElementById('LocationPathSpan').title = temp[4];
			}
			if (formelm["isPublic"]) {
				formelm["isPublic"].value = temp[4];
				ChangeOwnerShipVisiblity();
			}
			if (typeof SetLocationSQL == "function") {
				SetLocationSQL(LocationID);
			}
		}
	}
}


//////////////////////////////////////////////////////////////////////
//	Update location picker from a Location ID
//	
function UpdateLocationPickerFromIDs(pLocationIDs, formelm, elm1, elm2, elm3) {
	if (pLocationIDs == "") return;
	var LocationIDs = String(pLocationIDs);
	var strURL;
	var httpResponse;
	var arrLocations = LocationIDs.split(",");;

	var invalidIds = "";

	for (i = 0; i < arrLocations.length; i++) {
		strURL = serverType + serverName + "/cheminv/api/GetLocationFromID.asp?LocationID=" + arrLocations[i];
		httpResponse = JsHTTPGet(strURL)
		if (httpResponse.length == 0) {
			invalidIds = invalidIds + arrLocations[i] + ",";
		}
	}

	if (invalidIds.length > 0) {
		formelm[elm1].value = "";
		formelm[elm2].value = "";
		formelm[elm3].value = "";
		alert("Invalid location ids: " + invalidIds);
	}
	else {
		if (formelm[elm1]) {
			formelm[elm1].value = LocationIDs;
		}
		if (formelm[elm2]) {
			formelm[elm2].value = "MultiSelect";
		}
		if (formelm[elm3]) {
			formelm[elm3].value = "Multiple Racks Selected";
		}
		if (formelm["isRack"]) {
			formelm["isRack"].value = 1;
		}

		if (typeof SetLocationSQL == "function") {
			SetLocationSQL(LocationIDs);
		}
	}
}


//////////////////////////////////////////////////////////////////////
//	Update location picker from a Location ID
//	
function ValidateLocationIDType(LocationID) {
	if (LocationID == "") return;
	var strURL = serverType + serverName + "/cheminv/api/GetLocationFromID.asp?LocationID=" + LocationID;
	var httpResponse = JsHTTPGet(strURL)
	//alert(httpResponse)
	if (httpResponse.length == 0) {
		alert("Invalid location id= " + LocationID);
	}
	else {
		var temp = httpResponse.split(",");
		var isRack = temp[3];
		if (parseInt(isRack) == 1) {
			return 1;
		} else {
			return 0;
		}
	}
}


function GetFirstOpenRackPosition(LocationID) {
	if (LocationID == "") return;
	var strURL = serverType + serverName + "/cheminv/api/GetFirstOpenRackPosition.asp?LocationID=" + LocationID;
	var httpResponse = JsHTTPGet(strURL)
	//alert(httpResponse)
	if (httpResponse.length == 0) {
		alert("Invalid location id= " + LocationID);
	}
	else {
		return httpResponse;
	}
}


function IsKnownContainerBarcode(ContainerBarcode) {
	var strURL = serverType + serverName + "/cheminv/api/GetKeyContainerAttributes.asp?ContainerBarcode='" + ContainerBarcode + "'";
	var httpResponse = JsHTTPGet(strURL)
	//alert("JSHTTP= " + httpResponse)
	if (httpResponse.length == 0) {
		return 0;
	}
	else {
		return 1;
	}
}

function CheckDuplicateBarcode(barcodes, barcodeType) {
	var strURL = serverType + serverName + "/cheminv/api/CheckDuplicateBarcode.asp?barcodes=" + barcodes + "&barcodeType=" + barcodeType;
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);
	if (httpResponse.length == 0) {
		return 0;
	}
	else {
		return 1;
	}
}

function IsValidCompoundID(compoundID, bAlert) {
	var strURL = serverType + serverName + "/cheminv/api/IsValidCompoundID.asp?compoundID=" + compoundID;
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);
	if (httpResponse == 1) {
		return 1;
	}
	else {
		if (bAlert == true) {
			alert("Invalid Compound ID.");
		}
		return 0;
	}
}

function IsChildGridLocation(locationId) {
	var strURL = serverType + serverName + "/cheminv/api/IsChildGridLocation.asp?LocationID=" + locationId;
	var httpResponse = JsHTTPGet(strURL);
	if (httpResponse == 1) {
		return 1;
	}
	else {
		return 0;
	}
}

function IsPlateTypeAllowed(PlateId, LocationID, PlateTypeID, IsPlateMap) {
	if (LocationID != '') {
		var strURL = serverType + serverName + "/cheminv/api/IsPlateTypeAllowed.asp?plateID=" + PlateId + "&LocationID=" + LocationID + "&PlateTypeID=" + PlateTypeID + "&IsPlateMap=" + IsPlateMap;
		var httpResponse = JsHTTPGet(strURL);
		if (httpResponse == 1) {
			return 1;
		}
		else {
			return 0;
		}
	}

}

function AreEnoughRackPositions(numAssets, locationIds) {
	var strURL = serverType + serverName + "/cheminv/api/NumberOpenPositions.asp?locationIds=" + locationIds;
	var numOpen = JsHTTPGet(strURL);

	if (parseInt(numAssets) > parseInt(numOpen)) {
		return false;
	}
	else {
		return true;
	}
}
function GetRegIDFromRegNum(RegBatchID) {
	if (RegBatchID.length > 0) {
		var CsUserName = document.form1.tempCsUserName.value;
		var CsUserID = document.form1.tempCsUserID.value;
		var strURL = serverType + serverName + "/cheminv/api/GetRegistrationAttributes.asp?Action=GETREGATTRIBUTES&regbatchid=" + encodeURIComponent(RegBatchID) + "&tempCsUserName=" + CsUserName + "&tempCsUserID=" + CsUserID;
		var httpResponse = JsHTTPGet(strURL)
		if (httpResponse.length == 0) {
			document.form1.RegBatchID.value = ""
			document.form1.iRegID.value = ""
			document.form1.iBatchNumber.value = ""
			document.form1.NewRegID.value = ""
			document.form1.NewBatchNumber.value = ""
			alert("Unable to retrieve batch data for RegBatchID= " + RegBatchID);
		}
		else {
			var temp = httpResponse.split(",");
			document.form1.RegBatchID.value = RegBatchID;
			document.form1.iRegID.value = temp[6];
			document.form1.iBatchNumber.value = temp[7];
			document.form1.NewRegID.value = temp[6];
			document.form1.NewBatchNumber.value = temp[7];
		}
	}
}

function GetRegIDFromRegNumValidateOnly(RegBatchID) {
	if (RegBatchID.length > 0) {
		var CsUserName = document.form1.tempCsUserName.value;
		var CsUserID = document.form1.tempCsUserID.value;
		var strURL = serverType + serverName + "/cheminv/api/GetRegistrationAttributes.asp?Action=GETREGATTRIBUTES&regbatchid=" + encodeURIComponent(RegBatchID) + "&tempCsUserName=" + CsUserName + "&tempCsUserID=" + CsUserID;
		var httpResponse = JsHTTPGet(strURL)
		if (httpResponse.length == 0) {
			document.form1.RegNumFinalValidate.value = 1;
			document.form1.RegNumFinalValidateMsg.value = "Unable to to retrieve information for RegBatchID =" + RegBatchID;
		}
	}

}

function IsValidCompoundID(compoundID, bAlert) {
	var strURL = serverType + serverName + "/cheminv/api/IsValidCompoundID.asp?compoundID=" + compoundID;
	var httpResponse = JsHTTPGet(strURL);
	//alert("JSHTTP= " + httpResponse);
	if (httpResponse == 1) {
		return 1;
	}
	else {
		if (bAlert == true) {
			alert("Invalid Compound ID.");
		}
		return 0;
	}

}

function ValidateBarcode(barcode) {
	var Cnt = 0;
	var strURL = serverType + serverName + "/cheminv/api/ValidateBarcode.asp?barcode=" + barcode;
	var httpResponse = JsHTTPGet(strURL);
	var temp = httpResponse.split(",");
	var Cnt = temp[0];
	return Cnt;
}

function ValidateUniqueName(nametype, name) {
	var Cnt = 0;
	var strURL = serverType + serverName + "/cheminv/api/ValidateUniqueName.asp?nametype=" + nametype + "&name=" + name;
	var httpResponse = JsHTTPGet(strURL);
	var temp = httpResponse.split(",");
	var Cnt = temp[0];
	return Cnt;
}

function GetOpenRackPositons(rackid, startpos) {
	var strURL = serverType + serverName + "/cheminv/api/GetOpenRackPositions.asp?RackID=" + rackid + "&StartPos=" + startpos;
	var httpResponse = JsHTTPGet(strURL);
	return httpResponse;
}

function UpdateBatchInfo(RegID, BatchNum) {

	var strURL = serverType + serverName + "/cheminv/api/GetBatchInfo.asp?RegID=" + RegID + "&BatchNumber=" + BatchNum + "&UserID=" + tempCSUserName + "&Password=" + tempCSUserID;; //CBOE-1428 SJ	
	var httpResponse = JsHTTPGet(strURL)

	//alert(httpResponse)
	if (httpResponse.length == 0) {
		document.all["RegNum_" + RegID].innerHTML = "";
		document.all["NoteBookName_" + RegID].innerHTML = "";
		document.all["NoteBookPage_" + RegID].innerHTML = "";
		document.all["Chemist_" + RegID].innerHTML = ""
		document.all["UOMID_" + RegID].innerHTML = ""
		document.form1["ContainerSize_" + RegID].value = ""
		document.form1["InitialAmount_" + RegID].value = ""
		alert("Unable to retrieve batch data for " + RegID + "-" + BatchNum);
	}
	else {
		var temp = httpResponse.split(",");
		var tempAmount = temp[5].split(" ");
		document.all["RegNum_" + RegID].innerHTML = Cnv2nbs(temp[0]);
		document.all["NoteBookName_" + RegID].innerHTML = Cnv2nbs(temp[1]);
		document.all["NoteBookPage_" + RegID].innerHTML = Cnv2nbs(temp[2]);
		document.all["Chemist_" + RegID].innerHTML = Cnv2nbs(temp[3]);
		document.form1["UOMID_" + RegID].value = Cnv2nbs(temp[4]);
		document.form1["ContainerSize_" + RegID].value = tempAmount[0];
		//document.form1["InitialAmount_" + RegID].value = document.form1["ContainerSize_" + RegID].value;
		document.form1["InitialAmount_" + RegID].value = (tempAmount[0]);
	}

}

function UpdateBatchInfo2(RegID, BatchNum) {
	var strURL = serverType + serverName + "/cheminv/api/GetBatchInfo.asp?RegID=" + RegID + "&BatchNumber=" + BatchNum;
	var httpResponse = JsHTTPGet(strURL);
	//alert(httpResponse)
	if (httpResponse.length == 0) {
		document.all["RegNum_" + RegID + "_" + BatchNum].innerHTML = "";
		document.all["NoteBookName_" + RegID + "_" + BatchNum].innerHTML = "";
		document.all["NoteBookPage_" + RegID + "_" + BatchNum].innerHTML = "";
		document.all["Chemist_" + RegID + "_" + BatchNum].innerHTML = ""
		document.all["UOMID_" + RegID + "_" + BatchNum].innerHTML = ""
		document.form1["ContainerSize_" + RegID + "_" + BatchNum].value = ""
		document.form1["InitialAmount_" + RegID + "_" + BatchNum].value = ""
		alert("Unable to retrieve batch data for " + RegID + "-" + BatchNum);
	}
	else {
		var temp = httpResponse.split(",");
		document.all["RegNum_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[0]);
		document.all["NoteBookName_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[1]);
		document.all["NoteBookPage_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[2]);
		document.all["Chemist_" + RegID + "_" + BatchNum].innerHTML = Cnv2nbs(temp[3]);
		document.all["UOMID_" + RegID + "_" + BatchNum].value = Cnv2nbs(temp[4]);
		document.form1["ContainerSize_" + RegID + "_" + BatchNum].value = ConvertToNum(Cnv2nbs(temp[5]));
		document.form1["InitialAmount_" + RegID + "_" + BatchNum].value = document.form1["ContainerSize_" + RegID + "_" + BatchNum].value;
	}
}
function ConvertToNum(val) {
	val = parseInt(val);
	if (isNaN(val)) return "";
	return val
}


function Cnv2nbs(val) {
	if (val == "") {
		return "&nbsp;";
	}
	else {
		return val;
	}
}
function trim(str) {
	return str.replace(/^\s*|\s*$/g, "");
}

function readCookie2(name) {
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') c = c.substring(1, c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
	}
	return null;
}

/////////////////////////////////////////////////////////////////////
//	GetHTTP Content using msxml
//	
function JsHTTPGet(strURL) {
	var objXML = new ActiveXObject("Msxml2.XMLHTTP");
	var separator = strURL.indexOf('?') < 0 ? '?' : '&';
	objXML.open("GET", strURL + separator + "Ticket=" + readCookie2("COESSO"), false);
	objXML.send();
	strResponse = objXML.responseText;
	return strResponse;
}

/////////////////////////////////////////////////////////////////////
//	Get XML Content via msxml HTTP
//	Returns the xmlhttp object
function JsXMLHTTP(strMethod, strURL, strData) {
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
function URLEncode(strURL) {
	//not fully implemented
	strURL = strURL.replace(/\+/g, "%2B");
	strURL = strURL.replace(/\-/g, "%2D");
	return strURL
}

/////////////////////////////////////////////////////////////////////
//	Changes CSS elements for a passed class
/////////////////////////////////////////////////////////////////////
function AlterCSS(theClass, element, value) {
	var cssRules;
	if (document.all) {
		cssRules = 'rules';
	}
	else if (document.getElementById) {
		cssRules = 'cssRules';
	}
	for (var i = 0; i < document.styleSheets.length; i++) {
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
function changeContent(docElement, text) {
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
function selectRackDefaults() {
	var selectedRackIndex = 0;

	if (document.form1.SelectRackBySearch) {
		if (document.form1.SelectRackBySearch.checked) {
			rackField = document.form1.SuggestedRackID;
			//alert("here");
		} else {
			rackField = document.form1.RackID;
		}
	} else {
		rackField = document.form1.RackID;
	}

	//alert(rackField.selectedIndex);
	if (rackField.selectedIndex != -1) {
		//alert("here: " + document.form1.RackID.selectedIndex);
		selectedRackIndex = rackField.selectedIndex;
	}
	var defRackInfo = "";
	rackField.options[selectedRackIndex].selected = true;
	if (rackField.options[selectedRackIndex].value != 'NULL') {
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
		if (document.form1.LocationName && document.form1.AssignToRack.checked == true && document.form1.LocationName.length == 0) {
			document.form1.LocationName.value = arrDefRackInfo[1];
		}

	}
}

function IsWholeNumber(sText) {
	var ValidChars = "0123456789";
	var IsNumber = true;
	var Char;


	for (i = 0; i < sText.length && IsNumber == true; i++) {
		Char = sText.charAt(i);
		if (ValidChars.indexOf(Char) == -1) {
			IsNumber = false;
		}
	}
	return IsNumber;

}

function checkAll(field) {
	for (i = 0; i < field.length; i++)
		field[i].checked = true;
}

function uncheckAll(field) {
	for (i = 0; i < field.length; i++)
		field[i].checked = false;
}


function addToList(listField, newText, newValue) {
	if ((newValue == "") || (newText == "")) {
		alert("You cannot add blank values");
	} else {
		var len = listField.length++;
		listField.options[len].value = newValue;
		listField.options[len].text = newText;
		listField.selectedIndex = len;
	}
}

function removeFromList(listField) {
	if (listField.length == -1) {
		//alert("There are no values which can be removed!");
	} else {
		var selected = listField.selectedIndex;
		if (selected == -1) {
			alert("You must select an entry to be removed!");
		} else {
			var replaceTextArray = new Array(listField.length - 1);
			var replaceValueArray = new Array(listField.length - 1);
			for (var i = 0; i < listField.length; i++) {
				// Put everything except the selected one into the array
				if (i < selected) { replaceTextArray[i] = listField.options[i].text; }
				if (i > selected) { replaceTextArray[i - 1] = listField.options[i].text; }
				if (i < selected) { replaceValueArray[i] = listField.options[i].value; }
				if (i > selected) { replaceValueArray[i - 1] = listField.options[i].value; }
			}
			listField.length = replaceTextArray.length;
			for (i = 0; i < replaceTextArray.length; i++) {
				listField.options[i].value = replaceValueArray[i];
				listField.options[i].text = replaceTextArray[i];
			}
		}
	}
}

function SetToFirstControl() {
	var bFound = false;

	// For each form
	for (iForm = 0; iForm < document.forms.length; iForm++) {
		// For each element in each form
		for (iElement = 0; iElement < document.forms[iForm].length; iElement++) {
			// If it's not a hidden element
			if (document.forms[iForm][iElement].type != "hidden") {
				// and it's not disabled
				if (document.forms[iForm][iElement].disabled != true) {
					try {
						//set the focus to it
						document.forms[iForm][iElement].focus();
						bFound = true;
					}
					catch (er) { }
				}
			}
			// If found in this element, stop looking
			if (bFound)
				break;
		}
		// If found in this form, stop looking
		if (bFound)
			break;
	}
}
function GetSumofValues(ValueList) {
	var strURL = serverType + serverName + "/cheminv/gui/GetSumofValues.asp?valuelist=" + ValueList;
	var httpResponse = JsHTTPGet(strURL)
	return httpResponse;
}

function isUniqueBarcode(Barcode) {
	var strURL = serverType + serverName + "/cheminv/api/isUniqueBarcode.asp?barcode=" + Barcode;
	var httpResponse = JsHTTPGet(strURL);
	if (httpResponse == 0) {
		return true;
	}
	else {
		return false;
	}
}

function isaRackLocation(LocationID) {
	if (LocationID == "") return;
	var strURL = serverType + serverName + "/cheminv/api/getlocationfromid.asp?LocationID=" + LocationID;
	var httpResponse = JsHTTPGet(strURL)
	if (httpResponse.length == 0) {
		alert("Invalid location id= " + LocationID);
	}
	else {
		var temp = httpResponse.split(",");
		var isRack = temp[3];

		if (parseInt(isRack) == 1) {
			return 1;
		} else {
			return 0;
		}
	}
}

//fill the ownership dropdown list
function getList(Listname, index) {
	var GroupList = Listname;
	var lst = document.getElementById("Ownershiplst");
	var GroupValue = "";
	var txt = "";
	if (GroupList != null && GroupList.length != 0 && lst != null) {
		var arr_Group = GroupList.split("|");
		if (lst.options) {
			lst.options.length = 0;
		}
		for (i = 0; i < arr_Group.length; i++) {
			GroupValue = arr_Group[i];
			txt = GroupValue.split(",");
			for (j = 0; j < txt.length - 1; j++) {
				var newOption = document.createElement("OPTION");
				lst.options.add(newOption);
				newOption.value = txt[j];
				newOption.innerText = txt[j + 1];
				if (index != '') {
					if (index == txt[j]) {
						newOption.selected = true;
					}
				}

				j++;
			}
		}
	}
}

//////////////////////////////////////////////////////////////////////
//	Get Authorized location 
//	
function GetAuthorizedLocation(LocationID, tempCsUserName, tempCsUserID) {
	if (LocationID == "") return;
	if (LocationID.indexOf(',') != -1) {
		var arrLocations = LocationID.split(",");
		var invalidIds = ""
		for (i = 0; i < arrLocations.length; i++) {
			strURL = "http://" + serverName + "/cheminv/api/GetAuthorizedLocation.asp?LocationID=" + arrLocations[i] + "&tempCsUserName=" + tempCsUserName + "&tempCsUserID=" + tempCsUserID;
			var httpResponse = JsHTTPGet(strURL)
			if (httpResponse == 0) return httpResponse;
		}
	}
	else {
		var strURL = "http://" + serverName + "/cheminv/api/GetAuthorizedLocation.asp?LocationID=" + LocationID + "&tempCsUserName=" + tempCsUserName + "&tempCsUserID=" + tempCsUserID;
		var httpResponse = JsHTTPGet(strURL)
	}
	return httpResponse;

}

//////////////////////////////////////////////////////////////////////
//	Get Container Authority 
/////////////////////////////////////////////////////////////////////
function GetContainerAuthority(barcode, username) {
	var username = document.form1.tempCsUserName.value;
	var strURL = serverType + serverName + "/cheminv/api/CheckContainerOwernership.asp?Barcode=" + barcode + "&username=" + username;
	var httpResponse = JsHTTPGet(strURL);
	return httpResponse;
}
//////////////////////////////////////////////////////////////////////
//	Set Location Owner info
/////////////////////////////////////////////////////////////////////
function SetOwnerInfo(objectType) {
	var locationIDVal;
	var tempuserID;
	var principalID = '';
	//get the Location id value
	if (document.form1.iDeliveryLocationID) {
		locationIDVal = document.form1.iDeliveryLocationID.value;
	}
	else if (document.form1.iLocationID) {
		locationIDVal = document.form1.iLocationID.value;
	}
	else if (document.form1.LocationID) {
		locationIDVal = document.form1.LocationID.value;
	}
	else if (document.form1.ParentID) {
		locationIDVal = document.form1.ParentID.value;
	}
	else {
		locationIDVal = document.form1.iLocation_ID_FK.value;
	}

	principalID = GetPrincipalID(locationIDVal, 'location');
	if (principalID != 0) {

		document.form1.LocationAdmin.value = principalID;
	}
	else {
		document.form1.LocationAdmin.value = '';
	}

	//load parent container
	if (objectType == 'container') {
		principalID = GetPrincipalID(document.form1.ContainerID.value, 'container');
	}

	//load parent plate
	if (objectType == 'plate') {
		principalID = GetPrincipalID(document.form1.iPlateIDs.value, 'plate');
	}

	//get theuser id value
	if (document.form1.tempCsUserName) {
		tempuserID = document.form1.tempCsUserName.value;
	}
	else {
		tempuserID = document.getElementById("tempCsUserName").value;
	}

	if (objectType == 'location' || objectType == 'container' || objectType == 'plate') {
		if (principalID != 0 && principalID.indexOf(",") < 0) {
			FillOwnerDropDown(principalID, objectType);
		}
		else {
			if (document.getElementById("Group_cb")) {
				document.getElementById("Group_cb").checked = false;
			}
			if (document.getElementById("User_cb")) {
				document.getElementById("User_cb").checked = false;
			}
			if (document.getElementById("Ownershiplst")) {
				document.getElementById("Ownershiplst").options.length = 0;
			}
			document.form1.LocationAdmin.value = '';
		}
	}
	else if (objectType == 'user') {
		var tempString = "|" + document.getElementById("OwnerShipUserList").value;
		if (tempString.indexOf("|" + principalID + ",") >= 0) { // user is owner
			FillOwnerDropDown(principalID, 'location');
		}
		else {
			FillOwnerDropDown(GetPrincipalID(tempuserID, 'user'), 'user');
		}
	}
	else {
		tempString = "|" + document.getElementById("OwnerShipGroupList").value;
		if (tempString.indexOf("|" + principalID + ",") >= 0) { // Group is owner
			FillOwnerDropDown(principalID, 'group');
		}
		else {
			FillOwnerDropDown(0, 'group');
		}
	}
	if (document.getElementById("Ownershiplst")) {
		setPrincipalID(document.getElementById("Ownershiplst"));
	}
}

// fill the ownership dropdown list based on the criteria passed
function FillOwnerDropDown(principalID, objectType) {
	var type = principalID;
	if (principalID > 0) {
		var tempString = "|" + document.getElementById("OwnerShipUserList").value;
		if (tempString.indexOf("|" + type + ",") >= 0) {
			getList(document.getElementById("OwnerShipUserList").value, type);
			if (document.getElementById("User_cb"))
				document.getElementById("User_cb").checked = true;
		}
		else {
			getList(document.getElementById("OwnerShipGroupList").value, type);
			if (document.getElementById("Group_cb"))
				document.getElementById("Group_cb").checked = true;
		}
	}
	else {
		if (objectType == 'user') {

			getList(document.getElementById("OwnerShipUserList").value, null);
		}
		else {
			getList(document.getElementById("OwnerShipGroupList").value, null);
		}
	}
}
//function will return principal id(required for Ownership). 
//For object type location, container or palte it will return princial id of the owner. for objectType user it will return user's object id.
function GetPrincipalID(objectID, objectType) {
	var returnStr = '';

	if (objectID.indexOf(",") >= 0) {
		var arrTemp = objectID.split(",");
		for (var i = 0; i < arrTemp.length; i++) {
			var strURL = serverType + serverName + "/cheminv/api/GetPrincipalID.asp?objectID=" + arrTemp[i] + "&objectType=" + objectType;
			var httpResponse = JsHTTPGet(strURL);
			if (returnStr == '')
				returnStr = httpResponse;
			else
				returnStr += ',' + httpResponse;
		}
		return returnStr;
	}
	else {
		var strURL = serverType + serverName + "/cheminv/api/GetPrincipalID.asp?objectID=" + objectID + "&objectType=" + objectType;
		var httpResponse = JsHTTPGet(strURL);
		return httpResponse;
	}
}

//////////////////////////////////////////////////////////////////////
//	Get Valid location 
//	
function GetValidLocation(LocationID, Targettype, TargettypeID) {
	if (LocationID == "") return;

	var strURL = serverType + serverName + "/cheminv/api/GetValidLocation.asp?LocationID=" + LocationID + "&TargetType=" + Targettype + "&TargetTypeID=" + TargettypeID;
	var httpResponse = JsHTTPGet(strURL)
	return httpResponse;

}

/////////////////////////////////////////////////////////////////////
// Get Parent or child Locations of a location
////////////////////////////////////////////////////////////////////
function GetParentOrChildLocations(LocationID, Direction) {
	var strURL = serverType + serverName + "/cheminv/API/GetParentOrChildLocations.asp?LocationID=" + LocationID + "&Direction=" + Direction;
	var httpResponse = JsHTTPGet(strURL);
	if (httpResponse.length == 0) {
		return '';
	}
	else {
		return httpResponse;
	}
}
/////////////////////////////////////////////////////////////////////
//Set RLS Sessions
////////////////////////////////////////////////////////////////////
function SetRLSSessions(locationList, roleID) {
	var strURL = serverType + serverName + "/cheminv/GUI/SetRLSExcludedLocSession.asp?locationList=" + locationList + "&roleID=" + roleID
	var httpResponse = JsHTTPGet(strURL);
}

function onkeydown_handler() {
	switch (event.keyCode) {
		case 112: // 'F1'
			document.onhelp = function () { return (false); }
			window.onhelp = function () { return (false); }
			event.returnValue = false;
			event.keyCode = 0;
			window.open('/cheminv/help/default.htm');
			return false;
			break;
	}
}
if (navigator.userAgent.toLowerCase().indexOf('msie') != -1) {
	document.attachEvent("onkeydown", onkeydown_handler);
}
else {
	document.addEventListener("keydown", onkeydown_handler);
}
