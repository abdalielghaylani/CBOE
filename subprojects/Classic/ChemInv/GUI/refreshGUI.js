//Globals
/*CSBR# 139460 
 Purpose: To change servername to include portnumber information
*/
var serverName = window.location.host; //End of the change
var serverType = String(window.location);
serverType = serverType.substring(0, serverType.indexOf(serverName));

/////////////////////////////////////////////////////////////////////
//	GetHTTP Content using msxml
//	
function RefreshJsHTTPGet(strURL) {
	var objXML;
	if (window.XMLHttpRequest) {
		//IE7+ and all other browsers
		objXML = new window.XMLHttpRequest();
	} else {
		//IE6-
		objXML = new ActiveXObject("Msxml2.XMLHTTP");
	}
	objXML.open("GET", strURL, false);
	objXML.send();
	strResponse = objXML.responseText;
	return strResponse;
}


function SelectLocationNode(bClearNodes, tempLocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView) {

	// get the appropriate location id to refresh to
	var strURL = serverType + serverName + "/cheminv/api/GetRefreshLocationId.asp?locationId=" + tempLocationID;
	var LocationID = RefreshJsHTTPGet(strURL)

	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID + "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer;
	if (RemoveLocationID) {
		url += "&RemoveNode=" + RemoveLocationID;
	}

	if (openNodes) {
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;

	if (opener) {
		var thetop = opener.parent;
	} else {
		var thetop = parent;
	}
	if (IsPlateView) {
		thetop = thetop.parent;
	}
	//if (thetop.name == "main"){
	if (thetop.mainFrame) {
		// Tree is in search results mode
		var theTreeFrame = thetop.mainFrame;
		if (theTreeFrame) theTreeFrame.location.reload();
	} else if (thetop.TreeFrame) {
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash
		theTreeFrame.location.href = url;
		if (tempURL == url) {
			if (theTreeFrame) theTreeFrame.location.reload();
		}
	} else {
		opener.location.reload();
	}
}
//SMathur- Linage display in search mode. Now it will show the container/plate in new window if you are in search mode.
function SelectLocationNode4(bClearNodes, tempLocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView) {
	// get the appropriate location id to refresh to
	var strURL = "http://" + serverName + "/cheminv/api/GetRefreshLocationId.asp?locationId=" + tempLocationID;
	var LocationID = RefreshJsHTTPGet(strURL)

	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID + "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer;
	if (RemoveLocationID) {
		url += "&RemoveNode=" + RemoveLocationID;
	}

	if (openNodes) {
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;

	if (opener) {
		var thetop = opener.parent;
	} else {
		var thetop = parent;
	}
	if (IsPlateView) {
		thetop = thetop.parent;
	}
	//if (thetop.name == "main"){
	if (thetop.mainFrame) {
		// Tree is in search results mode
		window.open(serverType + serverName + "/cheminv/cheminv/BrowseInventory_frset.asp?Clearnodes=0&treeid=1&nodeTarget=listFrame&nodeURL=Buildlist.asp&gotonode=" + LocationID + "&sNode=" + LocationID + "&selectContainer=" + SelectContainer + "&SelectWell=&Exp=Y");
	} else if (thetop.TreeFrame) {
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash
		theTreeFrame.location.href = url;
		if (tempURL == url) {
			if (theTreeFrame) theTreeFrame.location.reload();
		}
	} else {
		opener.location.reload();
	}
}

//PerentPlate display in search mode. 
function SelectLocationNode3(bClearNodes, tempLocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView, counter) {

	// get the appropriate location id to refresh to
	var strURL = "http://" + serverName + "/cheminv/api/GetRefreshLocationId.asp?locationId=" + tempLocationID;
	var LocationID = RefreshJsHTTPGet(strURL)

	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID + "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer;
	if (RemoveLocationID) {
		url += "&RemoveNode=" + RemoveLocationID;
	}

	if (openNodes) {
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;

	if (opener) {
		var thetop = opener.parent;
	} else {
		var thetop = parent;
	}
	if (IsPlateView) {
		thetop = thetop.parent;
	}
	//if (thetop.name == "main"){
	if (thetop.mainFrame) {
		// Tree is in search results mode
		var LinkID = "mylink" + counter;
		document.getElementById(LinkID).href = serverType + serverName + "/cheminv/cheminv/BrowseInventory_frset.asp?Clearnodes=0&treeid=1&nodeTarget=listFrame&nodeURL=Buildlist.asp&gotonode=" + LocationID + "&sNode=" + LocationID + "&selectContainer=" + SelectContainer + "&SelectWell=&Exp=Y"
		document.getElementById(LinkID).target = "_blank";
		//var theTreeFrame =  thetop.mainFrame;
		//if (theTreeFrame) theTreeFrame.location.reload();
	} else if (thetop.TreeFrame) {
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash
		theTreeFrame.location.href = url;
		if (tempURL == url) {
			if (theTreeFrame) theTreeFrame.location.reload();
		}
	} else {
		opener.location.reload();
	}
}
//adds wellID 
function SelectLocationNode2(bClearNodes, LocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView, SelectWell) {

	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID + "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer + "&SelectWell=" + SelectWell;
	if (RemoveLocationID) {
		url += "&RemoveNode=" + RemoveLocationID;
	}

	if (openNodes) {
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;

	if (opener) {
		var thetop = opener.parent;
	} else {
		var thetop = parent;
	}
	if (IsPlateView) {
		thetop = thetop.parent.parent.parent;
	}
	if (SelectWell > 0) {
		thetop = thetop.parent.parent;
	}
	//if (thetop.name == "main"){
	if (thetop.mainFrame) {
		// Tree is in search results mode
		//var theTreeFrame =  thetop.mainFrame;
		//if (theTreeFrame) theTreeFrame.location.reload();
		var theTreeFrame = thetop.main;
		if (theTreeFrame) theTreeFrame.location.reload();
	} else if (thetop.TreeFrame) {
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash
		theTreeFrame.location.href = url;

		if (tempURL == url) {
			if (theTreeFrame) {
				theTreeFrame.location.reload();
			}

		}
	} else {
		opener.location.reload();
	}
}

function SelectLocation(LocationID, LocationName, ContainerID) {
	url = "/cheminv/cheminv/BuildList.asp?LocationID=" + LocationID;
	if (LocationName) {
		url += "&LocationName=" + LocationName.replace("\\\\", "\\");
	}
	if (ContainerID) {
		url += "&SelectContainer=" + ContainerID;
	}

	if (opener) {
		var thetop = opener.parent;
	} else {
		var thetop = parent;
	}
	//if (thetop.ListFrame) thetop.ListFrame.location.href = url;
	SelectLocationNode(0, LocationID, 0);
}



function SelectContainer(ContainerID) {
	url = "/cheminv/gui/viewContainer.asp?getdata=db";
	if (ContainerID) {
		url += "&ContainerID=" + ContainerID;
	}
	if (opener) {
		var thetop = opener.parent;
	} else {
		var thetop = parent;
	}
	if (thetop.TabFrame) thetop.TabFrame.location.href = url;
}