<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim RS
Dim Cmd

stepValue = Request("stepValue")
if isEmpty(stepValue) then stepValue = "1"
roleID = Request("roleID")
ClearNodes = Request("ClearNodes")
RemoveNode = Request.QueryString("RemoveNode")
sNode = Request.QueryString("sNode")
ExpNode = Request.QueryString("Exp")
GotoNode = Request.QueryString("GotoNode")
TreeID = "5"
refresh = request("refresh")
'Response.Write Request.QueryString & "<BR>"

if stepValue = "1" then
	stepText = "Select a Role."
	formAction = "#"
elseif stepValue = "2" then
	stepText = "Select the locations the role will be EXCLUDED from accessing."
	formAction = "ManageRoleLocations_action.asp"
	if ClearNodes = "" then ClearNodes=1
end if

Icon_clsd = "icon_clsdfold.gif" 
Icon_open = "icon_openfold.gif"
Icon_clsd2 = "icon_clsdfold2.gif"
Icon_open2 = "icon_openfold2.gif"
        
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Role Locations</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<style>
	A {Text-Decoration: none;}
	.TreeView {color:#000000; font-size:8pt; font-family: arial}
	A.TreeView:LINK {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
	A.TreeView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
	A.TreeView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
</style>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	var totalLocations="";
	window.focus();
	function Validate(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";

		// role is required
		if (document.form1.roleID.value.length == 0) {
			errmsg = errmsg + "- Role is required.\r";
			bWriteError = true;
		}
        var locationsArr=totalLocations.split(",");	          
	    for(i=0;i<locationsArr.length;i++)
	    {
	        if (locationsArr[i]=="0"){
                errmsg = errmsg + "- Root location can not be excluded.\r";
			    bWriteError = true;   
			    break;
	        }
	        if ((locationsArr[i]=="1") || (locationsArr[i]=="2") || (locationsArr[i]=="3") || (locationsArr[i]=="4")){
                errmsg = errmsg + "- System locations can not be excluded.\r";
			    bWriteError = true;   
			    break;
	        }	        
	    }	    
		// set form action according to which step this is
		document.form1.action = '<%=formAction%>';
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.selectedLocationID.value=totalLocations;
			document.form1.submit();
		}
	}
	
    function clickLocation(theObject) {
        var value = theObject.value;
        var strURL = serverType + serverName + "/cheminv/GUI/ManageRoleLocationsDict.asp?locationID=" + value;
        var httpResponse = JsHTTPGet(strURL);
        var temptotalLocations = "," + totalLocations + ",";
        var parentSelected = false;
        if (totalLocations != '' && temptotalLocations.indexOf("," + value + ",") != -1 && !theObject.checked) {
            var parentList = GetParentOrChildLocations(value, 'parent');
            if (parentList != '') {
                var arrParentList = parentList.split(",");
                for (i = 0; i < arrParentList.length; i++) {
                    if (temptotalLocations.indexOf("," + arrParentList[i] + ",") != -1) {
                        parentSelected = true;
                        break;
                    }
                }
            }

        }
        // select the child location
        if (theObject.checked) {
            if (document.form1.selectedLocationID.value == "")
                document.form1.selectedLocationID.value = value;
            else
                document.form1.selectedLocationID.value = document.form1.selectedLocationID.value + ',' + value;
            var childList = GetParentOrChildLocations(value, 'child');
            var numLocations = document.form1.LocationID.length;
            var arrLocation = childList.split(",");
            var i;
            var j;
            for (i = 0; i < numLocations; i++) {
                // determine if a value is a member of a list of values
                for (j = 0; j < arrLocation.length; j++) {
                    if (arrLocation[j] == document.form1.LocationID[i].value) {
                        if (!document.form1.LocationID[i].checked) {
                            document.form1.LocationID[i].checked = true;
                            selecetedLocations(document.form1.LocationID[i].value);
                            document.form1.selectedLocationID.value = document.form1.selectedLocationID.value + "," + arrLocation[j];
                        }
                    }
                }
            }

        }
        else {
            var allSelectedLocations = ',' + document.form1.selectedLocationID.value + ',';
            allSelectedLocations = allSelectedLocations.replace("," + value + ",", ","); //remove the unselected location
            var childList = GetParentOrChildLocations(value, 'child');
            var newChildList = "," + childList + ",";
            if (childList != "") {
                var arrLocation = childList.split(",");
                var j;
                var k;
                for (j = 0; j < arrLocation.length; j++) {
                    var numLocations = document.form1.LocationID.length;
                    for (k = 0; k < numLocations; k++) {
                        if (arrLocation[j] == document.form1.LocationID[k].value) {
                            if (document.form1.LocationID[k].checked) {
                                newChildList = newChildList.replace("," + arrLocation[j] + ",", ",");
                                break;
                            }
                        }
                    }

                }
                if ("," + childList + "," == newChildList) // parent location is collapsed
                {
                    var arrLocation = childList.split(",");
                    for (j = 0; j < arrLocation.length; j++) {
                        allSelectedLocations = allSelectedLocations.replace("," + arrLocation[j] + ",", ",");
                    }
                }


            }
            document.form1.selectedLocationID.value = allSelectedLocations.substr(1, allSelectedLocations.length - 2);
        }
        if (!parentSelected) {
            SetRLSSessions(document.form1.selectedLocationID.value, document.form1.roleID.value);
            selecetedLocations(value);
        }
        else {
            alert('You cannot unselect a child location');
            theObject.checked = true;
        }
    }
    

	//Change Function Definition to fix 78536
	function selecetedLocations(value) {
        var flag=false;
        var i;   
	    var locationsArr=totalLocations.split(",");
	    totalLocations="";
	    for(i=0;i<locationsArr.length;i++)
	    {	        
	        if (value==locationsArr[i]){
	            flag=true;	            
	        }
	        else{
	        totalLocations=locationsArr[i] + "," + totalLocations;
	        }
	    }	    
        totalLocations=totalLocations.substr(0,totalLocations.length-1);        
	    if(!flag){
	       if (totalLocations.length>0){
	            totalLocations=totalLocations + "," + value;
	       }
           else{
                totalLocations=value;
           }
        }    
        
    }

    // This function will be used to click the child locations on page load
    function SelectChildLocation() {
        var selectedLocations = document.form1.selectedLocationID.value;
        var arrloc = selectedLocations.split(",");
        var ProcessedLocations = '';
        var i;
        for (i = 0; i < arrloc.length; i++) {
            var LocationID = arrloc[i];
            if (ProcessedLocations.indexOf("," + LocationID + ",") == -1) {
                ProcessedLocations = ProcessedLocations + LocationID + ',';
                var childList = GetParentOrChildLocations(LocationID, 'child');
                if (childList != "") {
                    ProcessedLocations = ProcessedLocations + childList + ',';
                    var numLocations = document.form1.LocationID.length;
                    var arrLocation = childList.split(",");
                    var k;
                    var j;
                    // determine if a value is a member of a list of values
                    for (j = 0; j < arrLocation.length; j++) {
                        for (k = 0; k < numLocations; k++) {
                            if (arrLocation[j] == document.form1.LocationID[k].value) {
                                if (!document.form1.LocationID[k].checked) {
                                    document.form1.LocationID[k].checked = true;
                                    selecetedLocations(document.form1.LocationID[k].value);
                                    document.form1.selectedLocationID.value = document.form1.selectedLocationID.value + "," + arrLocation[j];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        SetRLSSessions(document.form1.selectedLocationID.value, document.form1.roleID.value);
    }
-->
</script>


</head>
<body TOPMARGIN="0" LEFTMARGIN="5" BGCOLOR="#FFFFFF">
<br><br>
<form NAME="form1" METHOD="POST">
<input TYPE="hidden" NAME="stepValue" VALUE="2">
<input TYPE="hidden" NAME="selectedLocationID" value="">
<input TYPE="hidden" NAME="refresh" VALUE="<%=refresh%>">
<input TYPE="hidden" NAME="propagate" VALUE="1">
<table border="0" align="center">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback"><%=stepText%></span><br /><br />
		<table bgcolor="#e1e1e1" width="100%"><tr><td align="center">
			Step <strong><%=stepValue%></strong> of 2
		</td></tr></table><br /><br />
	</td></tr>
	<tr>
		<td align="right" valign="top">
			<span class="required">Role:</span>
		</td>
<%if stepValue = "1" then%>
		<td>
			<%=ShowSelectBox2("roleID", "", "SELECT role_id AS Value, role_name AS DisplayText FROM CS_SECURITY.security_roles, privilege_tables WHERE privilege_table_int_id = privilege_table_id AND privilege_table_name = 'CHEMINV_PRIVILEGES' ORDER BY role_name ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
	</tr>
<%
elseif stepValue = "2" then

Call GetInvConnection()
'-- get role name
Call GetInvCommand("SELECT role_name FROM cs_security.security_roles WHERE role_id = ?", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PRoleID",131, 1, 0, roleID)
Set rs = Cmd.Execute
roleName = rs("role_name")
set rs = nothing
		
if refresh = "1" then		
	'-- Get list of currently excluded locations
	Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".ManageRLS.GetRoleLocations(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PRoleID",131, 1, 0, roleID)
	if bDebugPrint then
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
		Response.End
	Else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set rs = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
	end if

	'Set rs = Conn.Execute(sql)
	excludedLocationIDs = " "
	Set excludedDict = server.CreateObject("Scripting.Dictionary")
	While NOT rs.EOF
		excludedDict.Add trim(cstr(rs("location_id_fk"))), cstr(rs("role_id_fk"))
		excludedLocationIDs = excludedLocationIDs & rs("location_id_fk") & ","
		rs.MoveNext
	Wend
	excludedLocationIDs = left(excludedLocationIDs,len(excludedLocationIDs)-1)
	rs.Close()
	Set rs = Nothing
	
	'-- put the dict into a session var
	Set Session("excludedDict") = excludedDict
	'Response.Write excludedLocationIDs
	'Response.Write cstr(excludedDict.Item("1002")) & "test2<BR>"
	'Response.Write excludedDict.Item("1") & "test2<BR>"
else
	Set excludedDict = Session("excludedDict")
end if

'for each key in excludedDict
'	Response.Write key & "<BR>"
'next
'Response.Write Dict2List(excludedDict,2)

		%>
		<td>
			<input TYPE="hidden" NAME="roleID" VALUE="<%=roleID%>">
			<input TYPE="text" NAME="roleName" VALUE="<%=roleName%>" SIZE="25" CLASS="GrayedText" READONLY style="text-align:left;">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
			<span class="required">Excluded Locations:</span>
		</td>
	<td>
<%
' Create the TreeView Component.
Set TreeView = Server.CreateObject("VASPTV.ASPTreeView")

' Tells the component which stylesheet to use.
TreeView.Class = "TreeView"

TreeView.Style = 7
TreeView.LineStyle = 1
TreeView.ImagePath = "../images/treeview"
TreeView.AutoScrolling = True
TreeView.SingleSelect = False
TreeView.QueryString = ""
TreeView.LicenseKey = "8712-0DFC-5CEB"
'TreeView.QueryString = "ClearNodes=0" & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&TreeID=" & TreeID & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&MaybeLocSearch=" & Request.QueryString("MaybeLocSearch")
TreeView.QueryString = "ClearNodes=0" & "&stepValue=2&roleID=" & roleID

If (IsEmpty(Session("TVNodes" & TreeID)) OR ClearNodes = 1) Then
	'Start with an empty tree
	Set Session("TVNodes" & TreeID)= Nothing
	Session("TreeViewOpenNodes" & TreeID) = ""
	Session("PrevExpandedNodesList" & TreeID) = ":"
	'Read the default inventory location
	
	if  bGotoNode then
		Call PopulateNodeLayer("NULL","<=3")
		Call GetNodes(GotoNode) ' Get the tree leading to the requested node
	Else
		sNode = "NULL"
		Call PopulateNodeLayer(sNode,"<=3") ' Add the first three layers of nodes
	End if
	' Save the current nodes collection in session var for reuse
	Set Session("TVNodes" & TreeID)= TreeView.Nodes 
Elseif (ExpNode = "Y") then
	' Fetch the nodes collection from session var
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
		Call PopulateNodeLayer(sNode,"<=2") ' Add the two layers below the selected node
	End if
	'Save the nodes collection in session var for reuse
	Set Session("TVNodes" & TreeID)= TreeView.Nodes 
End if

If IsObject(Session("TVNodes" & TreeID)) then
	'Response.Write("##" & TreeID)
	Set TreeView.Nodes = Session("TVNodes" & TreeID)
	If TreeView.Nodes.Count > 0 then
		TreeView.Nodes(1).EnsureVisible
		if TreeView.Nodes.Count > 1 then  TreeView.Nodes(2).EnsureVisible
		if NOT bDebugPrint then
			'on error resume next
			TreeView.Show
			if err.number > 0 then bRefresh = true
		End if
	End if
	
	' Keep track of open nodes as URL string that can be appended when recalling the tree page
	ClickedNode = Request.QueryString("sNode")
	If Not IsEmpty(ClickedNode) then
		if InStr(1,Session("TreeViewOpenNodes" & TreeID), "&node=" & ClickedNode) > 0 then
			Session("TreeViewOpenNodes" & TreeID) = Replace(Session("TreeViewOpenNodes" & TreeID), "&node=" & ClickedNode & "&" , "&")
		Else
			Session("TreeViewOpenNodes" & TreeID) = Session("TreeViewOpenNodes" & TreeID) & "&node=" & ClickedNode
		End if
	End if
	'Response.Write TreeID & "=TreeID<BR>"
	'Response.Write Session("TVNodes" & TreeID) & "=Session(TVNodes)<BR>"
	'Response.Write Session("TreeViewOpenNodes1") & "=Session(TreeViewOpenNodes)<BR>"
	'Response.Write ClickedNode & "=cn"
	Set TreeView = Nothing
End if


'call PopulateNodeLayer("NULL","<=3")
'on error resume next
'Response.Write TreeView.Nodes.Count
'source = TreeView.Style


 
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
		if pNodeID = "NULL" then
			pNodeID = "IS NULL"
		Else 
			pNodeID = "= " & pNodeID
		End if
		dbkey = Application("appkey")
		If Session("INV_MANAGE_PLATE_MAPS" & dbkey) then		
			LayerSQL = "SELECT Location_Name, Location_Barcode, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, Location_Description, Location_id AS ID, Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates FROM inv_Locations WHERE level " & pLevel & " CONNECT BY prior Location_id = Parent_id AND level " & pLevel & " START WITH Location_id IN (SELECT Location_id from inv_Locations where Parent_id " & pNodeID & ") ORDER BY level, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then UPPER(location_name) when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUV()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then UPPER(substr(location_name, 1, instr(location_name, ' ', -1))) else UPPER(location_name) end, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then 1000000 when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUVXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then to_number(substr(location_name, instr(location_name, ' ', -1) + 1)) else 1000000 end"
		else
			LayerSQL = "SELECT Location_Name, Location_Barcode, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, Location_Description, Location_id AS ID, Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates FROM inv_Locations WHERE location_type_id_fk not in (select location_type_id from inv_location_types where location_type_name = 'Plate Map') AND level " & pLevel & " CONNECT BY prior Location_id = Parent_id AND level " & pLevel & " START WITH Location_id IN (SELECT Location_id from inv_Locations where Parent_id " & pNodeID & ") ORDER BY level, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then UPPER(location_name) when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUV()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then UPPER(substr(location_name, 1, instr(location_name, ' ', -1))) else UPPER(location_name) end, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then 1000000 when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUVXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then to_number(substr(location_name, instr(location_name, ' ', -1) + 1)) else 1000000 end"
		end if
		if bDbugPrint then
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
			bShowPlates = 0
			
			if CInt(RS("AllowedPlates").value) > 0 then bShowPlates = 1 
			if bDebugPrint then
				Response.Write "<BR>" & i & ". " & id & "-" & ParentID & "-" & RS("Location_Name") & "-" & RS("LocationTypeName")
			Else
				on error resume next			
				' Add a Node to the Tree.
				' Syntax: TREEVIEW.NODES.ADD ([Relative],[Relationship],[Key],[Text],[Image],[ExpandedImage],[URL],[Target],[ToolTipText])
				If RS("LocationTypeName") = "Plate Map" then
					Set Nodex = TreeView.Nodes.Add(ParentID,4,id,Left(RS("Location_Name"),200), Icon_clsd2, Icon_clsd2, , , RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value)			
				else
					'Set Nodex = TreeView.Nodes.Add("R", 4,"C", "<INPUT TYPE=""CHECKBOX"" id=1 name=1>Components","icon_cpack.gif")
					'Set Nodex = TreeView.Nodes.Add("R", 4,Left(RS("Location_Name"),200),"<INPUT TYPE=""CHECKBOX"" id=1 name=1>" & RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value, Icon_clsd)
					'chkString = ""
					'Response.Write cstr(id) & "<BR>"
					'if excludedDict.Exists(cstr(id)) then chkString = "CHECKED"
					'if excludedDict.Exists(cstr(id)) then Response.Write "test"
					'Response.Write excludedDict.Exists("1002") & "<BR>"
					Set Nodex = TreeView.Nodes.Add(ParentID,4,id,"<INPUT TYPE=""CHECKBOX"" NAME=""LocationID"" VALUE=""" & id & """ onclick=""clickLocation(this);"">" &Left(RS("Location_Name"),200), Icon_clsd, Icon_clsd, , , RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value)
					'Set Nodex = TreeView.Nodes.Add("R",4,id,Left(RS("Location_Name"),200), Icon_clsd, Icon_clsd, , , RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value)						
				end if
				'Nodex.URL = NodeURL & "?LocationID=" & Nodex.Key & "&LocationName=" & Server.URLEncode(Nodex.Text) & "&stepValue=2&userID=" & userID
				'Nodex.Target = NodeTarget
				'if TreeID = 2 or TreeID = 3 then MaybeCloseWindow = "window.close()"
				'if Request.QueryString("MaybeLocSearch")<> "" then MaybeLocSearch = "opener.SetLocationSQL('" & Nodex.key & "')" 
				'if len(formelm) > 0 then
				'	if TreeID = 3 then
				'		if RS("LocationTypeName") = "Plate Map" then
				'			Nodex.DHTML = "onclick=""opener.UpdateLocationPickerFromID('"  & Nodex.Key & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & "; " & MaybeCloseWindow & "; return false;"""
				'		else
				'			Nodex.DHTML = "onclick=""alert('This is not a valid plate map location.');return false;"""						
				'		end if
				'	else
				'		Nodex.DHTML = "onclick=""opener.UpdateLocationPickerFromID('"  & Nodex.Key & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & "; " & MaybeCloseWindow & "; return false;"""
				'	end if
				'Else
				'	Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=OpenFolder(" & Nodex.Key & ");" & MaybeCloseWindow
				'End if
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
		PathSQL = "SELECT inv_Locations.Location_ID AS ID FROM inv_Locations WHERE inv_Locations.Parent_ID IS NOT NULL CONNECT BY inv_Locations.Location_ID = prior inv_Locations.Parent_ID START WITH inv_Locations.Location_ID=" & pNodeID & " ORDER BY LEVEL, UPPER(Location_Name)"
		Call GetInvConnection()
		Set RS = Conn.Execute(PathSQL)
		if RS.EOF AND RS.BOF then
			' The requested node does not exist so reload at the root instead.
			Response.Write "<SCRIPT language=Javascript>document.location.href='BrowseTree.asp?ClearNodes=1&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&sNode=0'</script>"
		Else
			Do While NOt RS.EOF
				Session("TreeViewOpenNodes" & TreeID) = Session("TreeViewOpenNodes" & TreeID) & "&node=" & RS("ID") 
				Session("PrevExpandedNodesList" & TreeID) = Session("PrevExpandedNodesList" & TreeID)& RS("ID") & ":"
				RS.MoveNext 
			Loop
			RS.Close
			Conn.Close
			Set RS = Nothing
			Set Conn = Nothing
			NodesSQL = "SELECT Location_Name, Location_Barcode, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, Location_Description, Location_id AS ID, Parent_id,(SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates  FROM inv_Locations WHERE level <=3 CONNECT BY prior Location_id = Parent_id AND level <=3 START WITH Location_id IN (SELECT  Location_id  FROM inv_Locations  CONNECT BY inv_Locations.Location_ID = prior inv_Locations.Parent_ID START WITH inv_Locations.Location_ID=" & pNodeID & ")  ORDER BY LEVEL, UPPER(location_name)"	
			'Response.Write NodesSQL
			'Response.end
			Call BuildTree(NodesSQL)
		end if		
	end if
End Sub

'Open the target node
if bRefresh then
	response.write "<Script language=javascript>" & vbCrLf
	Response.Write "var Aelm = document.anchors.namedItem(""refresh""); " & vbCrLf
	Response.Write "if (Aelm){" & vbCrLf
	Response.Write "Aelm.click();}</script>" & vbCrLf
elseif bGotoNode AND TreeID = 1 then
	response.write "<Script language=javascript>" & vbCrLf
	Response.Write "var Aelm = document.anchors.namedItem(""e" & Request.QueryString("Gotonode") & """); " & vbCrLf
	Response.Write "if (Aelm){" & vbCrLf
	'Response.Write "document.all.e" & Request.QueryString("Gotonode") & ".href = document.all.e" & Request.QueryString("Gotonode") & ".href" & "+ '&SelectContainer=" & SelectContainer & "&SelectWell=" & SelectWell & "';" 
	'Response.Write "document.all.e" & Request.QueryString("Gotonode") & ".href = document.all.e" & Request.QueryString("Gotonode") & ".href" & "+ '&SelectContainer=" & SelectContainer & "#" & SelectContainer & "';"
	Response.Write "document.all.e" & Request.QueryString("Gotonode") & ".href = document.all.e" & Request.QueryString("Gotonode") & ".href" & "+ '&SelectContainer=" & SelectContainer & "';"   & vbCrLf
	Response.Write "Aelm.click();}</script>" & vbCrLf
	'Response.Write "alert(document.all.e" & Request.QueryString("Gotonode") & ".href);}</script>"
Else
	if TreeID = 1 then 
		response.write "<Script language=javascript>" & vbCrLf
		Response.Write "var Aelm = document.anchors.namedItem(""e0""); " & vbCrLf
		Response.Write "if (Aelm) {" & vbCrLf
		Response.Write "Aelm.click();}</script>"
	End if
End if
%>
<script language="javascript">
    var locationList = '<%response.write Dict2List(excludedDict,null)%>';
    var arrLocation = locationList.split(",");
    var numLocations = document.form1.LocationID.length;
    totalLocations = locationList;
    //check each location to see if its in the location list
    var i;
    var j;
    for (i = 0; i < numLocations; i++) {
        // determine if a value is a member of a list of values
        for (j = 0; j < arrLocation.length; j++) {
            if (arrLocation[j] == document.form1.LocationID[i].value)
                document.form1.LocationID[i].checked = true;
        }
    }
    // select the child locations
    document.form1.selectedLocationID.value = '<%=Dict2List(excludedDict, null) %>';
    SelectChildLocation();
</script>
		</td>
	</tr>
<%end if%>
	<tr><td colspan="2" align="right">
		<%if stepValue = "2" then%>
			<a HREF="#" onclick="window.location='ManageRoleLocations.asp?refresh=1'; return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;
			<a HREF="#" onclick="history.go(-1);"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a>
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
            <br /><span class="GuiFeedback">Please note: </span> On selecting a location, all child location will also be excluded.
		<%else%>
			<a HREF="#" onclick="window.location='menu.asp'; return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/btn_next_61.gif" border="0"></a>
		<%end if%>
	</td></tr>
</form>
</body>
</html>
