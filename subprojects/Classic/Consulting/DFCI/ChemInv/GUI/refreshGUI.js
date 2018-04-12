//Globals
var serverName = window.location.hostname;

/////////////////////////////////////////////////////////////////////
//	GetHTTP Content using msxml
//	
function RefreshJsHTTPGet(strURL){
	var objXML = new ActiveXObject("Msxml2.XMLHTTP"); 
	objXML.open("GET", strURL, false);
	objXML.send(); 
	strResponse = objXML.responseText;
	return strResponse;
}


function SelectLocationNode(bClearNodes, tempLocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView){

	// get the appropriate location id to refresh to
	var strURL = "http://" + serverName + "/cheminv/api/GetRefreshLocationId.asp?locationId=" + tempLocationID;
	var LocationID = RefreshJsHTTPGet(strURL) 

    var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID +  "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer;
	if (RemoveLocationID){
		url += "&RemoveNode=" + RemoveLocationID;
	}

	if (openNodes){
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;
	
	if (opener){
		var thetop =  opener.parent;
	}
	else{
		var thetop = parent;
	}
	if (IsPlateView) {
		thetop = thetop.parent;
	}
	//if (thetop.name == "main"){
	if (thetop.mainFrame) {
		// Tree is in search results mode
		var theTreeFrame =  thetop.mainFrame;
		if (theTreeFrame) theTreeFrame.location.reload();
	}
	else if (thetop.TreeFrame){
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash 
		theTreeFrame.location.href = url;
		if (tempURL == url){
			if (theTreeFrame) theTreeFrame.location.reload();
		}
	}
	else{
		opener.location.reload();
	}
}

//adds wellID 
function SelectLocationNode2(bClearNodes, LocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView, SelectWell){

	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID +  "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer + "&SelectWell=" + SelectWell;
	if (RemoveLocationID){
		url += "&RemoveNode=" + RemoveLocationID;
	}
	
	if (openNodes){
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;
	
	if (opener){
		var thetop =  opener.parent;
	}
	else{
		var thetop = parent;
	}
	if (IsPlateView) {
		thetop = thetop.parent.parent.parent;
	}
	if (SelectWell>0) {
		thetop = thetop.parent.parent;
	}
	//if (thetop.name == "main"){
	if(thetop.mainFrame) {
		// Tree is in search results mode
		//var theTreeFrame =  thetop.mainFrame;
		//if (theTreeFrame) theTreeFrame.location.reload();
		var theTreeFrame =  thetop.main;
		if (theTreeFrame) theTreeFrame.location.reload();
	}
	else if (thetop.TreeFrame){
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash 
		theTreeFrame.location.href = url;

		if (tempURL == url){
			if (theTreeFrame) {
				theTreeFrame.location.reload();
			}
			
		}
	}
	else{
		opener.location.reload();
	}
}

function SelectLocation(LocationID, LocationName, ContainerID){
	url = "/cheminv/cheminv/BuildList.asp?LocationID=" + LocationID;
	if (LocationName){
		url += "&LocationName=" + LocationName.replace("\\\\", "\\");
	}
	if (ContainerID){
		url += "&SelectContainer=" + ContainerID;
	}
	
	if (opener){
		var thetop =  opener.parent;
	}
	else{
		var thetop = parent;
	}
	//if (thetop.ListFrame) thetop.ListFrame.location.href = url;
	SelectLocationNode(0, LocationID, 0);
}



function SelectContainer(ContainerID){
	url = "/cheminv/gui/viewContainer.asp?getdata=db";
	if (ContainerID){
		url += "&ContainerID=" + ContainerID;
	}
	if (opener){
		var thetop =  opener.parent;
	}
	else{
		var thetop = parent;
	}
	if (thetop.TabFrame) thetop.TabFrame.location.href =  url;
}

