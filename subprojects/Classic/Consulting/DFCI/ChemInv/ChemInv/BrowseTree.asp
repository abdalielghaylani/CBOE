<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%
Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint
Dim SelectContainer
response.Expires = 0
bDebugPrint = false
dbkey = "ChemInv"
'-- store the aspID so core can kill the session
'StoreASPSessionID()

'-- Prevent page from being cached
Response.ExpiresAbsolute = Now()

'-- set the return action so that the cancel button will go to the home page
Session("returnaction") = ""
'-- Set the isBatchSearch session variable so that it will show DHTML menu in the Container details page
Session("isBatchSearch") = "" 
'--set the return URL so that the it would not take you to the previously saved returnURL
Session("GUIReturnURL") = ""
Session("bManageMode")=0
'-- Kill session if requested
if Request.QueryString("killsession")= 1 then
	Session.Abandon
	Response.Write "Session Abandoned"
	Response.End
End if

'for each val in request.QueryString
    'response.Write val & "<BR/>"
'next

'-- Read QueryString parameters
TreeID = Request.QueryString("TreeID")
CompoundID = Request.QueryString("CompoundID")
ClearNodes = Request("ClearNodes")
RemoveNode = Request.QueryString("RemoveNode")
sNode = Request.QueryString("sNode")
if sNode="" then sNode=0
'openNodesList = Request.QueryString("Node")
ExpNode = Request.QueryString("Exp")
GotoNode = Request.QueryString("GotoNode")
SelectContainer = Request.QueryString("SelectContainer")
SelectWell = Request.QueryString("SelectWell")
Session("SelectWell") = SelectWell
formelm= Request("formelm")
elm1= Request("elm1")
elm2= Request("elm2")
elm3= Request("elm3")
bRefresh = false
multiSelect = Request("multiSelect")
clearMS = Request("clearMS")

if Request("isMultiSelectRacks")="false" or Request("isMultiSelectRacks")="0" then
    isMultiSelectRacks = "0"
else
    isMultiSelectRacks = "1"
end if
'-- set multiselect value
if isEmpty(multiSelect) or multiSelect = "" then
    multiSelect = Session("locationMS")
else
    Session("locationMS") = multiselect    
end if

if not isObject(Session("rackDict")) then
    SET Session("rackDict") = server.CreateObject("Scripting.Dictionary")
end if
set rackDict = Session("rackDict")

'-- clear the multiselect dictionary
if clearMS = "1" then
    rackDict.RemoveAll
    Set Session("rackDict") = rackDict
end if

lStyle = Request.QueryString("style")
NodeTarget = Request.QueryString("NodeTarget")
NodeURL = Request.QueryString("NodeURL")

If lStyle = "" Then lStyle = 7
If  GotoNode= "" OR (GotoNode = "0" and NodeURL = "") then  'Add  NodeURL =""to check its a pop up or Tree Frame
	Session("CurrentLocationID")= 0
	'GotoNode = "0"
	bGotoNode = False
Else
	bGotoNode = True
End if

'-- Build list of tree icons for JS icon refreshing
GetInvConnection()
    Set Cmd = nothing
    Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Racks.isRack", adCmdStoredProc)

    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",131, adparamreturnvalue, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID",131, 1, 0, sNode)
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Racks.isRack")

    checkRack = cbool(Cmd.Parameters("RETURN_VALUE"))
    if checkRack =true then
        checkRack =1
    else
        checkRack =0
    end if
sql_tree_icons = "select g.url_active,g.url_inactive from inv_graphics g, inv_graphic_types gt where g.graphic_type_id_fk = gt.graphic_type_id"
'Response.Write sql_tree_icons
'Response.End
Set RS_tree_icons = Conn.Execute(sql_tree_icons)
While NOT RS_tree_icons.EOF
	treeIconList = treeIconList & RS_tree_icons("url_active") & "::" & RS_tree_icons("url_inactive") & ","
    RS_tree_icons.MoveNext
Wend
RS_tree_icons.Close()

QS =  "isMultiSelectRacks=" & isMultiSelectRacks & "&ClearNodes=0" & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&TreeID=" & TreeID & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&MaybeLocSearch=" & Request.QueryString("MaybeLocSearch") & "&multiSelect=" & multiSelect
%>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en"lang="en">
<head>
<title></title>
<script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<style type="text/css">
	.treeitems{
		top:50px
	}
	.textmenu{
		top:20px
	}

    .divBorder{
        border-bottom-color:#fff; 
        border-left-color:#fff; 
        border-right-color:#fff;
        border-top-color:#CFD8E6; 
        border-style: solid; 
        border-bottom-width: 0px; 
        border-left-width: 0px; 
        border-right-width: 0px; 
        border-top-width: 2px;
    }

	.rackDetail{
        display:none;
	}
	.selectedRacks{
	    display:none;
	}
    .bar{background-color:#fff;}
    .roll{background-color:#c4df9b;}
}   
</style>
<script type="text/javascript" for="window" event="onscroll" language="jscript">
	slide();
</script>
<script type="text/javascript" language="javascript">

	window.focus()
	var PrevFolderImage
	var PrevLink
	var locationInfo;
	//alert(sNode);
	
	function OpenFolder(locationID,imgActive,imgInActive)
	{
		//alert(locationID + ":" + imgActive + ":" + imgInActive);
		var ename = "e" +  locationID;
		var elm = document.anchors(ename);
		elm.style.color = "black"
		elm.style.fontWeight = "bold"
		CurrFolderImage = elm.firstChild

		// Set the active image
		elm.firstChild.src = "/ChemInv/images/treeview/" + imgActive;

		// Set the closed image of the previous tree location
		// starting on second time through tree is rendered and only if images are different
		if ((typeof(PrevFolderImage)== "object") && (PrevFolderImage!=CurrFolderImage))
		{

			var treeIconList = "<%=treeIconList%>";
			var arrTreeIconList = treeIconList.split(",");
			var bShowDefault = true;
			for (i = 0; i < arrTreeIconList.length; i++){
				if (arrTreeIconList[i].length > 0) {
					var arrTemp = arrTreeIconList[i].split("::")
					if (PrevFolderImage.src.indexOf(arrTemp[0]) > 0){
						PrevFolderImage.src = "/ChemInv/images/treeview/"+arrTemp[1];
						bShowDefault = false;
						break;
					}
				}
			}
			if (bShowDefault) {
				PrevFolderImage.src = "/ChemInv/images/treeview/icon_clsdfold.gif";
			}
			PrevLink.style.color = "#4682b4";
			PrevLink.style.fontWeight = ""
		}

		// Sets current image for active location
		PrevLink = elm;
		PrevFolderImage = elm.firstChild;
	}

	function clickLocation(value, locationText) {
		//alert(serverName);
		var strURL = "http://" + serverName + "/cheminv/cheminv/BrowseTreeDict.asp?action=click&locationID=" + value + "&locationText=" + locationText;
		//alert(strURL);	
		var httpResponse = JsHTTPGet(strURL);
		//alert(httpResponse);
		//document.write(httpResponse);
		displaySelectedRackText(httpResponse);
	}
	
	function getRackDictionary() {
		var strURL = "http://" + serverName + "/cheminv/cheminv/BrowseTreeDict.asp";
		return(JsHTTPGet(strURL));
	}
	
	function displaySelectedRackText(newLocationInfo){
    	var sNode = <%=sNode%>
	    locationInfo = newLocationInfo;
        eval("var rackDict = new Array(" + locationInfo + ")");
        //alert("var rackDict = new Array(" + httpResponse + ")");
        var text = "<br/><b>" + (rackDict.length/2) + " Selected Racks</b><br/>";
        // the names are the 2nd half of the array
        //alert((rackDict.length/2));
        rackOrder = 1;
        for(i=(rackDict.length/2);i<rackDict.length;i++) {
            text = text + rackOrder + ". " + rackDict[i] + "<br/>";
            rackOrder = rackOrder + 1;
        }
        text = text + "<br/>Racks will be filled in this order."

		document.all("selectedRacks").innerHTML = text;
        AlterCSS('.selectedRacks','display','block');        
         // select the first rack
        if (rackDict.length > 0 && sNode!=rackDict[0] && <%=checkRack  %>  ) {
            var urlBeginning = "BrowseTree.asp?sNode=";
            var urlEnd = "&isRack=True&" + "<%=replace(QS,"ClearNodes=0","ClearNodes=1")%>";            
            document.location = urlBeginning + rackDict[0] + urlEnd;
        }        
	}
	
	// returns a comma-delimited list of the selected racks starting with the 2nd selected rack
	function getEndingLocationIds(){
        eval("var rackDict = new Array(" + locationInfo + ")");
        var list = "";
        if (rackDict.length > 2){
            for(i=1;i<(rackDict.length/2);i++) {
                list = list + rackDict[i] + ",";
            }
            list = list.substr(0,list.length-1);
        }
        return list;
	}
	// This function is removing the parentRack Id from the list of the selected rack location 
	function getEndingLocationIdsNew(locationId){
        eval("var rackDict = new Array(" + locationInfo + ")");
        var list = "";
        if (rackDict.length > 2){
            for(i=0;i<(rackDict.length/2);i++) {
                if(rackDict[i]!=locationId)
                {
                list = list + rackDict[i] + ",";
                }
            }
            list = list.substr(0,list.length-1);
        }
        return list;
	}
	

    function slide() {
	    with(document.body){
		    header.style.left=0;
		    header.style.top=scrollTop;
	    }
    }

	// When viewing browse tree as a dialog box hide the drop down menu
	<%if TreeID <> 1 then %>
	AlterCSS('.dropDownMenuControl','display','none')
	<%end if %>
    //AlterCSS('.textmenu','top','0')
   	//AlterCSS('.treeitems','top','25')

</script>
</head>
<body>
<% If Session("INV_CREATE_LOCATION" & dbkey) OR Session("INV_EDIT_LOCATION" & dbkey) or Session("INV_MOVE_LOCATION" & dbkey)  or Session("INV_DELETE_LOCATION" & dbkey) then %>
<div align="left" id="header" style="POSITION:Absolute;top:0;left:0;visibility:visible;z-index:3;">
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/tree_frame_links.asp"-->
</div>
<%end if 

'-- create tabs for all other trees except the main browser tree
if TreeID <> 1 then
%>
<div style="border-bottom-color:#CFD8E6; border-left-color:#fff; border-top-color:#fff; border-style: solid; border-left-width: 0px; border-bottom-width: 2px; border-right-width: 0px;">
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/TreeViewTabs.asp"-->
</div>
<%
end if

if Session("treeTab") = "Browse" or TreeID = 1 then
%>
<div id="textmenu" style="POSITION:Absolute;top:25;left:20;visibility:visible;z-index:-1;background-color=#ffffff;">
<table border="0" width="100%">
	<tr>
	    <td align="left" valign="top">
			<%if TreeID = 1 then %>
			<A CLASS="Treeview" HREF="BrowseTree.asp?isMultiSelectRacks=<%=isMultiSelectRacks%>&ClearNodes=0&TreeID=<%=TreeID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&GotoNode=<%=Session("DefaultLocation")%>&sNode=<%=Session("DefaultLocation")%>&Exp=Y#<%=Session("DefaultLocation")%>" title="Open the tree at your default location">*Default</A>
			<%elseif TreeID <> 1 then%>
    			<a class="Treeview" href="BrowseTree.asp?isMultiSelectRacks=<%=isMultiSelectRacks%>&ClearNodes=0&TreeID=<%=TreeID%>&MaybeLocSearch=<%=MaybeLocSearch%>&formelm=<%=formelm%>&elm1=<%=elm1%>&elm2=<%=elm2%>&elm3=<%=elm3%>&LocationPickerID=<%=LocationPickerID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&sNode=<%=Session("DefaultLocation")%>&Exp=Y#<%=Session("DefaultLocation")%>" title="Open the tree at your default location">*Default</a>
    		<% if isMultiSelectRacks <> "0" then %>    		    
			    <%if multiSelect = "1" then%>
			    |
                <a class="TreeView" href="BrowseTree.asp?isMultiSelectRacks=<%=isMultiSelectRacks%>&ClearNodes=0&TreeID=<%=TreeID%>&MaybeLocSearch=<%=MaybeLocSearch%>&formelm=<%=formelm%>&elm1=<%=elm1%>&elm2=<%=elm2%>&elm3=<%=elm3%>&LocationPickerID=<%=LocationPickerID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&multiSelect=1&clearMS=1&sNode=<%=sNode%>&Exp=Y" title="Clear Rack Selection">Clear All</a>
			    |
                <a class="TreeView" href="BrowseTree.asp?isMultiSelectRacks=<%=isMultiSelectRacks%>&ClearNodes=1&TreeID=<%=TreeID%>&MaybeLocSearch=<%=MaybeLocSearch%>&formelm=<%=formelm%>&elm1=<%=elm1%>&elm2=<%=elm2%>&elm3=<%=elm3%>&LocationPickerID=<%=LocationPickerID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&multiSelect=0&clearMS=1&sNode=<%=sNode%>&Exp=Y" title="Exit MultiSelect Mode">Cancel MultiSelect</a>
			    <%else%>
	    		|
                <a class="TreeView" href="BrowseTree.asp?isMultiSelectRacks=<%=isMultiSelectRacks%>&ClearNodes=1&TreeID=<%=TreeID%>&MaybeLocSearch=<%=MaybeLocSearch%>&formelm=<%=formelm%>&elm1=<%=elm1%>&elm2=<%=elm2%>&elm3=<%=elm3%>&LocationPickerID=<%=LocationPickerID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&multiSelect=1&sNode=<%=sNode%>&Exp=Y" title="Select Multiple Racks">MultiSelect Racks</a>
			    <%End if%>
			<%End if %>
			<%End if%>
	    </td>
		<td align=right valign="top">
			<A CLASS="Treeview" ID="refresh" HREF="BrowseTree.asp?isMultiSelectRacks=<%=isMultiSelectRacks%>&ClearNodes=1&TreeID=<%=TreeID%>&MaybeLocSearch=<%=MaybeLocSearch%>&formelm=<%=formelm%>&elm1=<%=elm1%>&elm2=<%=elm2%>&elm3=<%=elm3%>&LocationPickerID=<%=LocationPickerID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&GotoNode=0" title="Refresh the contents of the location tree"><img src="/cheminv/graphics/refresh_icon.gif" border="0"/>&nbsp;Refresh</A>
			<%if TreeID <> 1 then%>
			|
            <a CLASS="Treeview" href="#" onclick="top.opener.focus();window.close(); return false;"><img SRC="/ChemInv/graphics/close_icon.gif" border="0">&nbsp;Close</a>
			<%End if%>
		</td>
	</tr>
</table>
</div>

<div id="treeitems" style="POSITION:Absolute;top:45;left:20;visibility:visible;z-index:2">
<form name="locationTree" id="locationTree">
<% 
Set TreeView = Server.CreateObject("VASPTV.ASPTreeView")

TreeView.Class = "TreeView"
TreeView.Style = clng(lStyle)
TreeView.LineStyle = 1
TreeView.ImagePath = "/cheminv/images/treeview"
TreeView.AutoScrolling = True
TreeView.SingleSelect = False
TreeView.QueryString = ""
TreeView.LicenseKey = "8712-0DFC-5CEB"
TreeView.QueryString = "isMultiSelectRacks=" & isMultiSelectRacks & "&ClearNodes=0" & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&TreeID=" & TreeID & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&MaybeLocSearch=" & Request.QueryString("MaybeLocSearch")
'QS = "ClearNodes=0" & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&TreeID=" & TreeID & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&MaybeLocSearch=" & Request.QueryString("MaybeLocSearch") & "&multiSelect=" & multiSelect

If (IsEmpty(Session("TVNodes" & TreeID)) OR ClearNodes = 1) Then

	'-- Start with an empty tree
	Set Session("TVNodes" & TreeID)= Nothing
	Session("TreeViewOpenNodes" & TreeID) = ""
	Session("PrevExpandedNodesList" & TreeID) = ":"

	'-- Read the default inventory location
	if  bGotoNode then
		Call PopulateNodeLayer("NULL","<=5")
		Call GetNodes(GotoNode) ' Get the tree leading to the requested node
	Else
		sNode = "NULL"
		Call PopulateNodeLayer(sNode,"<=5") ' Add the first three layers of nodes
	End if
	'-- Save the current nodes collection in session var for reuse
	Set Session("TVNodes" & TreeID)= TreeView.Nodes
Elseif (ExpNode = "Y") then

	'-- Fetch the nodes collection from session var
	Set TreeView.Nodes = Session("TVNodes" & TreeID)
	if NOT IsEmpty(RemoveNode) then
		On Error resume next
		'Remove the node from the tree
		TreeView.Nodes.Remove(CStr(RemoveNode))
		'Remove the node from the open node list
		'Session("TreeViewOpenNodes" & TreeID) = Replace(Session("TreeViewOpenNodes" & TreeID), "&node=" & RemoveNode , "")
	End if
	if  bGotoNode OR GotoNode = "0" then
		Call GetNodes(GotoNode) ' Get the tree leading to the requested node
	Else
		Call PopulateNodeLayer(sNode,"<=4") ' Add the two layers below the selected node
	End if
	'-- Save the nodes collection in session var for reuse
	Set Session("TVNodes" & TreeID)= TreeView.Nodes
End if

If IsObject(Session("TVNodes" & TreeID)) then
	Set TreeView.Nodes = Session("TVNodes" & TreeID)
	If TreeView.Nodes.Count > 0 then
		TreeView.Nodes(1).EnsureVisible
		if TreeView.Nodes.Count > 1 then  TreeView.Nodes(2).EnsureVisible
		if NOT bDebugPrint then
			on error resume next
			TreeView.Show
			if err.number > 0 then bRefresh = true
		End if
	End if

	'-- Keep track of open nodes as URL string that can be appended when recalling the tree page
	ClickedNode = Request.QueryString("sNode")
	If Not IsEmpty(ClickedNode) then
		if InStr(1,Session("TreeViewOpenNodes" & TreeID), "&node=" & ClickedNode) > 0 then
			Session("TreeViewOpenNodes" & TreeID) = Replace(Session("TreeViewOpenNodes" & TreeID), "&node=" & ClickedNode , "")
		Else
			Session("TreeViewOpenNodes" & TreeID) = Session("TreeViewOpenNodes" & TreeID) & "&node=" & ClickedNode
		End if
	End if
	'Response.Write "[" & TreeID & "==TreeID<br>"
	'Response.Write "[" & Session("TVNodes" & TreeID) & "==Session(TVNodes)<BR>"
	'Response.Write "[" & Session("TreeViewOpenNodes1") & "==Session(TreeViewOpenNodes)<BR>" & vbcrlf
	'Response.Write "[" & ClickedNode & "==cn"
	Set TreeView = Nothing
End if

'-- Open the target node
response.Write chr(13)
if bRefresh then
	response.write "<script language=""javascript"">"
	Response.Write "var Aelm = document.anchors(""refresh""); "
	Response.Write "if (Aelm){"
	Response.Write "Aelm.click();}</script>"
elseif bGotoNode AND TreeID = 1 then
	response.write "<script language=""javascript"">"
	Response.Write "var Aelm = document.anchors(""e" & Gotonode & """); "
	Response.Write "if (Aelm){"
	Response.Write "document.all.e" & Gotonode & ".href = document.all.e" & Gotonode & ".href" & "+ '&SelectContainer=" & SelectContainer & "';"
	Response.Write "Aelm.click();}</script>"
Else
	if TreeID = 1 then response.write "<script language=""javascript""> var Aelm = document.anchors(""e0""); if (Aelm) {Aelm.click();}</script>"
End if
%>
</form>
<%
'-- displaying the rack starts here
isRack = cbool(Request("isRack"))
NumCols = 0
'-- get rack information
if isRack then
    if multiSelect = "1" then
        LocationID = request("sNode")
    else
        LocationID = sNode
    end if
    displayFields = "Empty,Icon,Barcode"
    '-- Set default selected view of Rack
    if Session("viewRackFilter") = "" then
	    viewRackFilter = "icon"
    else
	    viewRackFilter = Session("viewRackFilter")
    end if

    Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRID(?,?)}", adCmdText)	
    Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, LocationID)
    Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, 30, Application("RegServerName"))
    Cmd.Properties ("PLSQLRSet") = TRUE  
    Set RS = Server.CreateObject("ADODB.Recordset")
    RS.CursorLocation = aduseClient
    RS.LockType = adLockOptimistic
    RS.Open Cmd
    RS.ActiveConnection = Nothing
    RS.filter = "COL_INDEX=1"

    rowName_arr = RS.GetRows(adGetRowsRest, , "RowName")
    numRows = Ubound(rowName_arr,2) + 1 
    RS.filter = 0
    RS.Movefirst
    RS.filter = "ROW_INDEX=1"
    colName_arr = RS.GetRows(adGetRowsRest, , "ColName")
    NumCols = Ubound(colName_arr,2) + 1
    if NumCols > 15 then
	    cellWidth = 70
    else
	    cellWidth = 70
    end if
    cellWidthLucidaChars = cellWidth/6

    cntRacksInRack = 0
    cntPlatesInRack = 0
    cntContainersInRack = 0

    FldArray = split(lcase(displayFields),",")
    xmlHtml = ""
    xmlHtml = xmlHtml & "<xml ID=""xmlDoc""><rack>" & vbcrlf
    For currRow = 1 to numRows
	    For i = 0 to Ubound(FldArray)
		    FldName = FldArray(i)
		    RS.filter = 0
		    RS.Movefirst
		    RS.filter = "ROW_INDEX=" & currRow
		    rowName = RS("ROWNAME") 
		    xmlHtml = xmlHtml & "<" & FldName & ">" & vblf
		    xmlHtml = xmlHtml & "<rowname>" & rowname & "</rowname>"
		    rackCriterion = Request("RackCriterion")
		    if len(rackCriterion) > 0 then
			    key = left(rackCriterion,instr(rackCriterion,",")-1)
			    value = right(rackCriterion,len(rackCriterion) - instr(rackCriterion,","))
			    bCheckSelected = true
		    end if
		    While NOT RS.EOF
			    GridData = RS("grid_data").value
			    if isBlank(GridData) then
				    GridID = RS("location_id")
				    GridBarcode = RS("location_barcode")
				    GridType = ""
				    Title = "Select rack position " & GridBarcode
				    theValue = RS("name")
			    else
				    arrGridData = split(GridData,"::")
				    execute("cnt" & arrGridData(0) & "sInRack = cnt" & arrGridData(0) & "sInRack + 1")
				    GridID = arrGridData(1)
				    if isBlank(GridID) then GridID = RS("location_id")
				    GridBarcode = arrGridData(2)
				    GridType = lCase(arrGridData(0))
				    Title = "View Rack " & arrGridData(2)
				    if FldName = "barcode" then
					    theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & GridBarcode
				    else
					    theValue = "<img src=""" & arrGridData(3) & """ border=""0"">&nbsp;" & GridBarcode
				    end if
			    end if

			    isSelected = false
			    if bCheckSelected then
				    keyValue = RS(key).Value
				    if isNull(keyValue) then keyValue = ""	
				    if cstr(keyValue) = cstr(value) then isSelected = true
			    end if
    			
			    theValue = "<![CDATA[" & WrapRackContents(FldName, GridID, RS("name"), GridBarcode, GridType, theValue, Title, cellWidthLucidaChars, isSelected) & "]]>"
			    colIndex = RS("COL_INDEX")
			    xmlHtml = xmlHtml & "<col" & colIndex & ">" & theValue & "</col" & colIndex & ">" & vblf

			    RS.MoveNext		
		    Wend
		    xmlHtml = xmlHtml & "</" & FldName & ">" & vblf
	    Next
    Next
    xmlHtml = xmlHtml & "</rack></xml>"

response.Write xmlHtml
%>
<script type="text/javascript" language="javascript">
    function selectRackPosition(locationId){
        
        var locationIds = locationId;
        //var endingLocationIds = getEndingLocationIds();
        var endingLocationIds = getEndingLocationIdsNew(<%=LocationID%>);
        if (endingLocationIds.length > 0) {
            locationIds = locationIds + "," + endingLocationIds;
        }
        //alert(getEndingLocationIds());
        //alert(locationIds);        
        <%
        if TreeID = 2 or TreeID = 3 then MaybeCloseWindow = "window.close();"
        if multiSelect = "1" then
            response.write "opener.UpdateLocationPickerFromIDs(locationIds,opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & chr(13)
        else
            response.write "opener.UpdateLocationPickerFromID(locationIds,opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & chr(13)
        end if            
        response.write MaybeCloseWindow
         %>
    }
</script>
<% end if
if NodeURL<>"BuildList.asp" then
%>
<br />
<div id="selectedRacks" class="selectedRacks divBorder">
</div>
<%end if
if isRack then %>
<div id="sorttext" style="margin-left: -5px; float:left; position:Absolute;left:5px;visibility:visible;z-index=1;font-size:11px;">

Rack displayed by <strong>Icon</strong>. To change the display, click on the arrow.
</div>
<br />
<div id="rackDetail" class="rackDetail divBorder">
<br />
<table style="font-size:7pt; font-family: verdana; table-layout:fixed; border-collapse: collapse;" cellspacing="0" cellpadding="0" bordercolor="#999999" id="tbl" DATASRC DATAFLD="icon" border="1">
	<col width="30">
	<%
		For i=0 to NumCols-1
			Response.Write "<col width=""" & cellWidth & """>"
		Next
	%>
	<thead>
		<th align="center">
			<a href="#" onclick="document.all.hiddenSelector.style.visibility = 'visible';document.all.cboField.click()" title="Click to select displayed value"><img SRC="../graphics/desc_arrow.gif" border="0" WIDTH="12" HEIGHT="6"></a>
			<a id="hiddenRackSelector" target="rackJSFrame"></a>
			<div id="hiddenSelector" style="POSITION:Absolute;left=0;visibility:hidden;z-index:2">
			<select id="cboField" size="4">	
				<option value></option>
				<option value="icon">Icon</option>
				<option value="barcode">Barcode</option>
			</select>
			</div>
		</th>
	<%
		For i=0 to NumCols-1
			Response.Write "<th>" & colName_arr(0,i) & "</th>" & vblf
		Next
	%>
	</thead>
	<tr height="20" class="bar">
		<th><span DATAFLD="rowname"></span></th>
		<%
		For i=1 to NumCols
			Response.Write "<td class=""nav"" onmouseover=""className='roll'"" onmouseout=""className='nav'"" align=""center"" valign=""center""><div DATAFORMATAS=html DATAFLD=""col" & i &"""></div></td>" & vblf
		Next
		%>
	</tr>
</table>
<!--<div align="right" style="margin-top:10px;"><a href="#" onclick="top.opener.focus();window.close(); return false;"><img SRC="../graphics/sq_btn/cancel_dialog_btn.gif" border="0" WIDTH="53" HEIGHT="21"></a></div>-->
</div>

<script language="javascript">

    AlterCSS('.rackDetail','display','block');
    tbl.dataFld = "icon";
    tbl.dataSrc = "#xmlDoc"

</script>
<script for="cboField" event="onchange">
  var dispText = "Rack displayed by <strong>";
  dispText = dispText + this.options(this.selectedIndex).value + "</strong>. To change the display, click on the arrow.";
  changeContent(document.all.sorttext,dispText);
  
  tbl.dataSrc = ""; // unbind the table
  // Set the binding to the requested field
  tbl.dataFld = this.options(this.selectedIndex).value;

  tbl.dataSrc = "#xmlDoc"; // rebind the table
  document.all.hiddenSelector.style.visibility = 'hidden';
  wellFilter = tbl.dataFld;
</script>

<%
end if
elseif Session("treeTab") = "Search" then
%>
    <div id="Div1" style="POSITION:Absolute;top:25;left:20;visibility:visible;z-index:-1;background-color=#ffffff;">
    <table border="0" width="100%">
	    <tr>
		    <td align=right valign="top">
                <a CLASS="Treeview" href="#" onclick="top.opener.focus();window.close(); return false;"><img SRC="/ChemInv//graphics/close_icon.gif" border="0">&nbsp;Close</a>
		    </td>
	    </tr>
    </table>
    </div>

    <!--#INCLUDE VIRTUAL = "/cheminv/cheminv/TreeSearch.asp"-->
<%  
end if


%>
<script type="text/javascript" language="javascript">
	<%if multiSelect = "1" and NodeURL ="" then%> //Add  NodeURL =""to check its a pop up or Tree Frame
	//if in multiselect mode check the selected racks
	var rackDict = new Array(<%=dict2list(rackDict,null)%>);
	if(document.locationTree.LocationID){
		for(i=0;i<document.locationTree.LocationID.length;i++)
		  {
   		      //alert(document.locationTree.LocationID[i].value);
		      if(rackDict.length>0)   	    
		         for(j=0;j<rackDict.length;j++){	        
		             if (document.locationTree.LocationID[i].value == rackDict[j])
		                  document.locationTree.LocationID[i].checked = true;            
		           }	    
		        else	    
		        {
		            document.locationTree.LocationID[i].checked = false;   	    		        
		        }
		    }
	}
	//show the selected racks
    displaySelectedRackText(getRackDictionary());

    <%end if%>
</script>
</BODY>
</HTML>

<%

'****************************************************************************************
'*	PURPOSE:  Adds a new layer of nodes to the tree each time a plus is checked                                              *
'*  INPUT: 	  The node to add children to, pLevel <=2 for children and grandchildren,
'*            =2 for grandchildren
'*	OUTPUT:   Adds nodes to a preexisting tree
'****************************************************************************************
Sub PopulateNodeLayer(pNodeID, pLevel)
	Dim LayerSQL
	' Check the list of previously expanded nodes to avoid adding preexisting nodes to tree
	if InStr(1,Session("PrevExpandedNodesList" & TreeID),":" & pNodeID & ":") = 0 Then
		if pNodeID = "NULL" or pNodeID = "" then
			pNodeID = "IS NULL"
		Else
			pNodeID = "= " & pNodeID
		End if
		dbkey = Application("appkey")
		If Session("INV_MANAGE_PLATE_MAPS" & dbkey) then
			LayerSQL = "SELECT Location_Name, Location_Barcode, collapse_child_nodes, " & Application("oraschemaname") & ".racks.open_position_count(location_id) as open_position_count, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, (SELECT gr.url_active from inv_location_types lt, inv_graphics gr where lt.graphic_id_fk=gr.graphic_id(+) and lt.location_type_id=inv_Locations.Location_Type_ID_fk) AS Img_URL_active, (SELECT gr.url_inactive from inv_location_types lt, inv_graphics gr where lt.graphic_id_fk=gr.graphic_id(+) and lt.location_type_id=inv_Locations.Location_Type_ID_fk) AS Img_URL_inactive, Location_Description, Location_id AS ID, case when (select count((select l1.location_id from inv_locations l1 where l1.location_id in (select st.location_id_fk from inv_grid_storage st where st.grid_storage_id in (select ge.grid_storage_id_fk from inv_grid_element ge where ge.location_id_fk = inv_locations.Parent_id)) and collapse_child_nodes=1)) from dual) = 0 then Parent_id else (select l1.location_id from inv_locations l1 where l1.location_id in (select st.location_id_fk from inv_grid_storage st where st.grid_storage_id in (select ge.grid_storage_id_fk from inv_grid_element ge where ge.location_id_fk = inv_locations.Parent_id)) and collapse_child_nodes=1) end as Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates, (select grid_storage_id from inv_grid_storage where location_id_fk=location_id) as GridStorageID FROM inv_Locations WHERE level " & pLevel & " AND Location_id not in (Select Location_id from inv_locations where parent_id in (Select Location_ID from inv_locations where collapse_child_nodes=1) and collapse_child_nodes is null) CONNECT BY prior Location_id = Parent_id AND level " & pLevel & " START WITH Location_id IN (SELECT Location_id from inv_Locations where Parent_id " & pNodeID & ") ORDER BY level, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then UPPER(location_name) when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUVXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then UPPER(substr(location_name, 1, instr(location_name, ' ', -1))) else UPPER(location_name) end, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then 1000000 when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUVXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then to_number(substr(location_name, instr(location_name, ' ', -1) + 1)) else 1000000 end"
		else
			LayerSQL = "SELECT Location_Name, Location_Barcode, collapse_child_nodes, " & Application("oraschemaname") & ".racks.open_position_count(location_id) as open_position_count, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, (SELECT gr.url_active from inv_location_types lt, inv_graphics gr where lt.graphic_id_fk=gr.graphic_id(+) and lt.location_type_id=inv_Locations.Location_Type_ID_fk) AS Img_URL_active, (SELECT gr.url_inactive from inv_location_types lt, inv_graphics gr where lt.graphic_id_fk=gr.graphic_id(+) and lt.location_type_id=inv_Locations.Location_Type_ID_fk) AS Img_URL_inactive, Location_Description, Location_id AS ID, case when (select count((select l1.location_id from inv_locations l1 where l1.location_id in (select st.location_id_fk from inv_grid_storage st where st.grid_storage_id in (select ge.grid_storage_id_fk from inv_grid_element ge where ge.location_id_fk = inv_locations.Parent_id)) and collapse_child_nodes=1)) from dual) = 0 then Parent_id else (select l1.location_id from inv_locations l1 where l1.location_id in (select st.location_id_fk from inv_grid_storage st where st.grid_storage_id in (select ge.grid_storage_id_fk from inv_grid_element ge where ge.location_id_fk = inv_locations.Parent_id)) and collapse_child_nodes=1) end as Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates, (select grid_storage_id from inv_grid_storage where location_id_fk=location_id) as GridStorageID FROM inv_Locations WHERE location_type_id_fk not in (select location_type_id from inv_location_types where location_type_name = 'Plate Map') AND level " & pLevel & " AND Location_id not in (Select Location_id from inv_locations where parent_id in (Select Location_ID from inv_locations where collapse_child_nodes=1) and collapse_child_nodes is null) CONNECT BY prior Location_id = Parent_id AND level " & pLevel & " START WITH Location_id IN (SELECT Location_id from inv_Locations where Parent_id " & pNodeID & ") ORDER BY level, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then UPPER(location_name) when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUVXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then UPPER(substr(location_name, 1, instr(location_name, ' ', -1))) else UPPER(location_name) end, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then 1000000 when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUVXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then to_number(substr(location_name, instr(location_name, ' ', -1) + 1)) else 1000000 end"
		end if
		if bDebugPrint then
			Response.Write LayerSQL
			Response.End
		End	if
		Call BuildTree(LayerSQL)
		if (ExpNode = "Y") then Session("PrevExpandedNodesList" & TreeID) = Session("PrevExpandedNodesList" & TreeID)& pNodeID & ":"
	End if
End Sub

Sub BuildTree(pSQL)
		'Response.Write pSQL
		'Response.end
		Call GetInvConnection()
		Set RS = Conn.Execute(pSQL)
		i = 0
		Do While Not RS.EOF
			i = i + 1
			id = RS("id")
			ParentID = RS("Parent_ID")
			imgActive = RS("img_url_active")
			imgInActive = RS("img_url_inactive")
			if isBlank(imgActive) then imgActive = "icon_openfold.gif"
			if isBlank(imgInActive) then imgInActive = "icon_clsdfold.gif"

			if bDebugPrint then
				Response.Write "<br>" & i & ". " & id & "-" & ParentID & "-" & RS("Location_Name") & "-" & RS("LocationTypeName")
			Else
				on error resume next
				'-- INV_LOCATIONS.COLLAPSE_CHILD_NODES = 1 means that location is a Rack
				isRack = false
				if RS("collapse_child_nodes")=1 then isRack = true
				
				'-- If Rack, set showInList value
				showInList = ""
                displayName = Left(RS("Location_Name"),200)                
   				if isRack then 
				    showInList = "racks"
				    '-- if this is a browse location tree, then show the number of open positions for the rack
				    if TreeID = 2 then
   				        openPositionText = " (" & RS("open_position_count") & " open)"
   				        locationText = Left(RS("Location_Name"),200-(len(openPositionText))) & openPositionText
   		    		    if multiSelect = "1" then
   		    		        checkText = ""
   		    		        if rackDict.exists(cstr(id)) then checkText = " CHECKED"
                            displayName = "<INPUT TYPE=""CHECKBOX"" NAME=""LocationID"" VALUE=""" & id & """" & checkText & " onclick=""clickLocation(this.value, '" & locationText & "');"">" & locationText
       				    else
    				        openPositionText = " (" & RS("open_position_count") & " open)"
	    			        displayName = locationText
       				    end if
	    	        end if
   				    
                end if				    

				' Add a Node to the Tree.
				' Syntax: TREEVIEW.NODES.ADD ([Relative],[Relationship],[Key],[Text],[Image],[ExpandedImage],[URL],[Target],[ToolTipText])
				Set Nodex = TreeView.Nodes.Add(ParentID,4,id,displayName, imgInActive, imgInActive, , , RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value)

				Nodex.URL = NodeURL & "?LocationID=" & Nodex.Key & "&LocationName=" & Server.URLEncode(Nodex.Text) & "&showInList=" & showInList
				Nodex.Target = NodeTarget
				if TreeID = 2 or TreeID = 3 then MaybeCloseWindow = "window.close();"
				if Request.QueryString("MaybeLocSearch")<> "" then MaybeLocSearch = "opener.SetLocationSQL('" & Nodex.key & "');"
				if len(formelm) > 0 then
					if TreeID = 3 then
						if RS("LocationTypeName") = "Plate Map" then
							Nodex.DHTML = "onclick=""opener.UpdateLocationPickerFromID('"  & Nodex.Key & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & " " & MaybeCloseWindow & " return false;"""
						else
							Nodex.DHTML = "onclick=""alert('This is not a valid plate map location.');return false;"""
						end if
					else
					    if isRack then
	                        'Nodex.DHTML = "onclick=""alert('test');"""
	                        Nodex.URL = "BrowseTree.asp?sNode=" & Nodex.key & "&isRack=" & isRack& "&" & QS   
						else
    						Nodex.DHTML = "onclick=""opener.UpdateLocationPickerFromID('"  & Nodex.Key & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & "; " & MaybeCloseWindow & "; return false;"""
						end if
					end if
				Else
					Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=OpenFolder(" & Nodex.Key & ",""" & imgActive & """,""" & imgInActive & """);" & MaybeCloseWindow
				End if
			End if
			Set Nodex = nothing
			RS.MoveNext
		Loop

		RS.Close
		Conn.Close
		Set RS = Nothing
		Set Conn = Nothing
End Sub

Sub GetNodes(pNodeID)
	if InStr(1,Session("PrevExpandedNodesList" & TreeID),":" & pNodeID & ":") = 0 Then
		Dim PathSQL
		Dim NodesSQL
		PathSQL = "SELECT inv_Locations.Location_ID AS ID, (select pl.collapse_child_nodes from inv_vw_grid_location_lite ll, inv_locations pl where ll.location_id=inv_locations.location_id and pl.location_id = ll.parent_id) isInRack FROM inv_Locations WHERE inv_Locations.Parent_ID IS NOT NULL CONNECT BY inv_Locations.Location_ID = prior inv_Locations.Parent_ID START WITH inv_Locations.Location_ID=" & pNodeID & " ORDER BY LEVEL, UPPER(Location_Name)"
		Call GetInvConnection()
		Set RS = Conn.Execute(PathSQL)
		'Response.Write PathSQL
		if RS.EOF AND RS.BOF then
			'-- The requested node does not exist so reload at the root instead.
			Response.Write "<script language=""javascript"">document.location.href='BrowseTree.asp?isMultiSelectRacks=" & isMultiSelectRacks & "ClearNodes=1&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&sNode=0'</script>"
		Else
			Do While NOT RS.EOF
				if InStr(1,Session("TreeViewOpenNodes" & TreeID), "&node=" & RS("ID")) = 0 then
					'-- If Tree node is grid location, don't add as node
					if RS("isInRack") <> "1" then
						Session("TreeViewOpenNodes" & TreeID) = Session("TreeViewOpenNodes" & TreeID) & "&node=" & RS("ID")
					end if
				end if
				Session("PrevExpandedNodesList" & TreeID) = Session("PrevExpandedNodesList" & TreeID)& RS("ID") & ":"
				RS.MoveNext
			Loop
			RS.Close
			Conn.Close
			Set RS = Nothing
			Set Conn = Nothing
			NodesSQL = "SELECT Location_Name, Location_Barcode, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, (SELECT g.url_active from inv_location_types lt, inv_graphics g where lt.graphic_id_fk=g.graphic_id(+) and lt.location_type_id=inv_Locations.Location_Type_ID_fk) AS Img_URL_active, (SELECT g.url_inactive from inv_location_types lt, inv_graphics g where lt.graphic_id_fk=g.graphic_id(+) and lt.location_type_id=inv_Locations.Location_Type_ID_fk) AS Img_URL_inactive, Location_Description, Location_id AS ID, case when (select count((select l1.location_id from inv_locations l1 where l1.location_id in (select st.location_id_fk from inv_grid_storage st where st.grid_storage_id in (select ge.grid_storage_id_fk from inv_grid_element ge where ge.location_id_fk = inv_locations.Parent_id)) and collapse_child_nodes=1)) from dual) = 0 then Parent_id else (select l1.location_id from inv_locations l1 where l1.location_id in (select st.location_id_fk from inv_grid_storage st where st.grid_storage_id in (select ge.grid_storage_id_fk from inv_grid_element ge where ge.location_id_fk = inv_locations.Parent_id)) and collapse_child_nodes=1) end as Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates, collapse_child_nodes, (select grid_storage_id from inv_grid_storage where location_id_fk=location_id) as GridStorageID FROM inv_Locations WHERE level <=3 AND Location_id not in (Select Location_id from inv_locations where parent_id in (Select Location_ID from inv_locations where collapse_child_nodes=1) and collapse_child_nodes is null) CONNECT BY prior Location_id = Parent_id AND level <=3 START WITH Location_id IN (SELECT  Location_id  FROM inv_Locations  CONNECT BY inv_Locations.Location_ID = prior inv_Locations.Parent_ID START WITH inv_Locations.Location_ID=" & pNodeID & ")  ORDER BY LEVEL, UPPER(location_name)"
			'Response.Write NodesSQL
			'Response.end
			Call BuildTree(NodesSQL)
		end if
	end if
End Sub

Function WrapRackContents(FldName, GridID, GridName, GridBarcode, GridType, strText, Title, Length, isSelected)
	Dim str
	if (strText = "") OR IsNull(strText) then strText = ""
	strText2 = "<a hfref=#>" & strText 
	'if ActionType = "select" then
		if GridType <> "" then
			bgColor = "dedede"
		else
			bgColor = "transparent"
		end if
	'end if
	str = "<span style=""width:100%; background:" & bgColor & ";"""
	if (len(strText) > Length) AND (strText <> "&nbsp;") AND (Title <> "&nbsp;") then 
		str = str & " title=""" & Title & """>"
		'if ActionType = "select" then
			if GridType <> "" then
				str = str & strText
			else
				str = str & "<a class=""MenuLink"" href=""View Container Details" & GridID & """ onclick=""SelectContainer(" & GridID & "); return false;"">" & strText & "</a>"
			end if
		'end if
	else
		if instr(GridName,"'") > 0 then
			GridName = replace(GridName,"'","\'")
		end if
		'str = str & "><a class=""MenuLink"" href=""#"" title=""" & Title & """ onclick=""selectGrid(" & GridID & ",'" & GridName & "')"">" & strText & "</a>"
		'if Request.QueryString("MaybeLocSearch")<> "" then MaybeLocSearch = "opener.SetLocationSQL('" & Nodex.key & "');"
		if Request.QueryString("MaybeLocSearch")<> "" then MaybeLocSearch = "opener.SetLocationSQL('" & GridID & "');"
		if TreeID = 2 or TreeID = 3 then MaybeCloseWindow = "window.close();"
		'str = str & "><a class=""TreeView"" href=""#"" title=""" & Title & """ onclick=""opener.UpdateLocationPickerFromID('"  & GridID & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & " " & MaybeCloseWindow & " return false;"">" & strText & "</a>"
		str = str & "><a class=""TreeView"" href=""#"" title=""" & Title & """ onclick=""selectRackPosition(" & GridID & "); return false;"">" & strText & "</a>"
	end if
	
	str = str & "</span>"
	WrapRackContents = str
End function

 %>
