<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%

Dim Conn
Dim RS
Dim bDebugPrint
Dim SelectContainer

bDebugPrint = false

'-- store the aspID so core can kill the session
StoreASPSessionID()	

'Prevent page from being cached
Response.ExpiresAbsolute = Now()

'-- set the return action so that the cancel button will go to the home page
Session("returnaction") = ""

' Kill session if requested
if Request.QueryString("killsession")= 1 then 
	Session.Abandon
	Response.Write "Session Abandoned"
	Response.End
End if
'Read QueryString parameters
TreeID = Request.QueryString("TreeID")
CompoundID = Request.QueryString("CompoundID")
ClearNodes = Request("ClearNodes")
RemoveNode = Request.QueryString("RemoveNode")
sNode = Request.QueryString("sNode")
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

lStyle = Request.QueryString("style")
NodeTarget = Request.QueryString("NodeTarget")
NodeURL = Request.QueryString("NodeURL")

If lStyle = "" Then lStyle = 7
If  GotoNode= "" OR GotoNode = "0" then
	Session("CurrentLocationID")= 0
	'GotoNode = "0"
	bGotoNode = False
Else
	bGotoNode = True
End if 


Icon_clsd = "icon_clsdfold.gif" 
Icon_open = "icon_openfold.gif"
Icon_clsd2 = "icon_clsdfold2.gif"
Icon_open2 = "icon_openfold2.gif"
%>
<HTML>
<HEAD>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<STYLE>
	A {Text-Decoration: none;}
	.TreeView {color:#000000; font-size:8pt; font-family: arial}
	A.TreeView:LINK {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
	A.TreeView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
	A.TreeView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
</STYLE>
<SCRIPT LANGUAGE=JAVASCRIPT>
	window.focus()
	var PrevFolderImage
	var PrevLink
	var icon_clsd = "<%=icon_clsd%>";
	var icon_open = "<%=icon_open%>";
	var icon_clsd2 = "<%=icon_clsd2%>";
	var icon_open2 = "<%=icon_open2%>";
	var sNode = <%=sNode%>
	
	function OpenFolder(locationID)
	{
		var ename = "e" +  locationID
		var elm
			elm = document.anchors(ename);
		
		elm.style.color = "black"
		elm.style.fontWeight = "bold"
		CurrFolderImage = elm.firstChild 
		if (elm.firstChild.src.indexOf(icon_clsd) > 0){
			elm.firstChild.src = "/ChemInv/images/treeview/" + icon_open;
		}
		else if (elm.firstChild.src.indexOf(icon_open) == 0){
			elm.firstChild.src = "/ChemInv/images/treeview/" + icon_open2;
		}
		if ((typeof(PrevFolderImage)== "object") && (PrevFolderImage!=CurrFolderImage))
		{
			if (PrevFolderImage.src.indexOf(icon_open) > 0){
				PrevFolderImage.src = "/ChemInv/images/treeview/" + icon_clsd;
			}
			else {
				PrevFolderImage.src = "/ChemInv/images/treeview/" + icon_clsd2;
			}

			//PrevFolderImage.src = "/ChemInv/images/treeview/<%=icon_clsd%>";
			PrevLink.style.color = "#4682b4";
			PrevLink.style.fontWeight = ""
		}
		PrevLink = elm;
		PrevFolderImage = elm.firstChild;
	}

	function preloadImages() { 
	  var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
	    var i,j=d.MM_p.length,a=preloadImages.arguments; for(i=0; i<a.length; i++)
	    if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
	}	
</SCRIPT>
</HEAD>
<BODY TOPMARGIN=0 LEFTMARGIN=5 BGCOLOR="#FFFFFF" onload="preloadImages('/ChemInv/images/treeview/' + icon_clsd,'/ChemInv/images/treeview/' + icon_open,'/ChemInv/images/treeview/' + icon_open2,'/ChemInv/images/treeview/' + icon_clsd2);">

<TABLE border=0 width=100%>
	<TR>
		<TD align=right>
			<A CLASS="Treeview" ID="refresh" HREF="BrowseTree.asp?ClearNodes=1&TreeID=<%=TreeID%>&MaybeLocSearch=<%=MaybeLocSearch%>&formelm=<%=formelm%>&elm1=<%=elm1%>&elm2=<%=elm2%>&elm3=<%=elm3%>&LocationPickerID=<%=LocationPickerID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&GotoNode=0" title="Refresh the contents of the location tree">Refresh</A> 
			|
			<A CLASS="Treeview" HREF="BrowseTree.asp?ClearNodes=0&TreeID=<%=TreeID%>&NodeURL=<%=NodeURL%>&NodeTarget=<%=NodeTarget%>&GotoNode=<%=Session("DefaultLocation")%>&sNode=<%=Session("DefaultLocation")%>&Exp=Y#<%=Session("DefaultLocation")%>" title="Open the tree at your default location">Home</A>
			<%if TreeID = 1 then%>
			|
			<A CLASS="Treeview" HREF="Set Default Location" onclick="OpenDialog('/ChemInv/GUI/SetDefaultLocation.asp', 'Diag', 1); return false"  title="Make the selected location your default location">Make Default</A> 
			<%End if%>
		</TD>
	</TR>
</TABLE>
<%
Set TreeView = Server.CreateObject("VASPTV.ASPTreeView")

TreeView.Class = "TreeView"
TreeView.Style = clng(lStyle)
TreeView.LineStyle = 1
TreeView.ImagePath = "../images/treeview"
TreeView.AutoScrolling = True
TreeView.SingleSelect = False
TreeView.QueryString = ""
TreeView.LicenseKey = "8712-0DFC-5CEB"
TreeView.QueryString = "ClearNodes=0" & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&TreeID=" & TreeID & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&MaybeLocSearch=" & Request.QueryString("MaybeLocSearch")

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
'Response.Write "<script language=javascript>alert('OpenNodes=" & Session("TreeViewOpenNodes1") & "');</script>"



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
			LayerSQL = "SELECT Location_Name, Location_Barcode, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, Location_Description, Location_id AS ID, Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates FROM inv_Locations WHERE level " & pLevel & " CONNECT BY prior Location_id = Parent_id AND level " & pLevel & " START WITH Location_id IN (SELECT Location_id from inv_Locations where Parent_id " & pNodeID & ") ORDER BY level, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then UPPER(location_name) when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUV()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then UPPER(substr(location_name, 1, instr(location_name, ' ', -1))) else UPPER(location_name) end, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then 1000000 when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUV()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then to_number(substr(location_name, instr(location_name, ' ', -1) + 1)) else 1000000 end"
		else
			LayerSQL = "SELECT Location_Name, Location_Barcode, (SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk) AS LocationTypeName, Location_Description, Location_id AS ID, Parent_id, (SELECT count(*) from inv_allowed_ptypes where location_id_fk = inv_Locations.Location_ID) AS AllowedPlates FROM inv_Locations WHERE location_type_id_fk not in (select location_type_id from inv_location_types where location_type_name = 'Plate Map') AND level " & pLevel & " CONNECT BY prior Location_id = Parent_id AND level " & pLevel & " START WITH Location_id IN (SELECT Location_id from inv_Locations where Parent_id " & pNodeID & ") ORDER BY level, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then UPPER(location_name) when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUV()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then UPPER(substr(location_name, 1, instr(location_name, ' ', -1))) else UPPER(location_name) end, case when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(location_name) then 1000000 when length(substr(location_name, instr(location_name, ' ', -1) + 1)) = length(translate(UPPER(substr(location_name, instr(location_name, ' ', -1) + 1)), '0123456789ABCDEFGHIJLKMNOPQRSTUV()~`!@#$%^&*-_=+{}[]\|;:<>,./?', '0123456789')) then to_number(substr(location_name, instr(location_name, ' ', -1) + 1)) else 1000000 end"
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
		'Response.Write pSQL & "<BR>"
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
						Set Nodex = TreeView.Nodes.Add(ParentID,4,id,Left(RS("Location_Name"),200), Icon_clsd, Icon_clsd, , , RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value)			
					end if
					Nodex.URL = NodeURL & "?LocationID=" & Nodex.Key & "&LocationName=" & Server.URLEncode(Nodex.Text)
					Nodex.Target = NodeTarget
					if TreeID = 2 or TreeID = 3 then MaybeCloseWindow = "window.close()"
					if Request.QueryString("MaybeLocSearch")<> "" then MaybeLocSearch = "opener.SetLocationSQL('" & Nodex.key & "')" 
					if len(formelm) > 0 then
						if TreeID = 3 then
							if RS("LocationTypeName") = "Plate Map" then
								Nodex.DHTML = "onclick=""opener.UpdateLocationPickerFromID('"  & Nodex.Key & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & "; " & MaybeCloseWindow & "; return false;"""
							else
								Nodex.DHTML = "onclick=""alert('This is not a valid plate map location.');return false;"""						
							end if
						else
							Nodex.DHTML = "onclick=""opener.UpdateLocationPickerFromID('"  & Nodex.Key & "',opener." & formelm & ",'" & elm1 & "','" & elm2 & "','" & elm3 & "'); " & MaybeLocSearch & "; " & MaybeCloseWindow & "; return false;"""
						end if
					Else
						Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=OpenFolder(" & Nodex.Key & ");" & MaybeCloseWindow
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
	response.write "<Script language=javascript>"
	Response.Write "var Aelm = document.anchors(""refresh""); "
	Response.Write "if (Aelm){"
	Response.Write "Aelm.click();}</script>"
elseif bGotoNode AND TreeID = 1 then
	response.write "<Script language=javascript>"
	Response.Write "var Aelm = document.anchors(""e" & Request.QueryString("Gotonode") & """); "
	Response.Write "if (Aelm){"
	'Response.Write "document.all.e" & Request.QueryString("Gotonode") & ".href = document.all.e" & Request.QueryString("Gotonode") & ".href" & "+ '&SelectContainer=" & SelectContainer & "&SelectWell=" & SelectWell & "';" 
	'Response.Write "document.all.e" & Request.QueryString("Gotonode") & ".href = document.all.e" & Request.QueryString("Gotonode") & ".href" & "+ '&SelectContainer=" & SelectContainer & "#" & SelectContainer & "';"
	Response.Write "document.all.e" & Request.QueryString("Gotonode") & ".href = document.all.e" & Request.QueryString("Gotonode") & ".href" & "+ '&SelectContainer=" & SelectContainer & "';"  
	Response.Write "Aelm.click();}</script>"
	'Response.Write "alert(document.all.e" & Request.QueryString("Gotonode") & ".href);}</script>"
Else
	if TreeID = 1 then response.write "<Script language=javascript>var Aelm = document.anchors(""e0""); if (Aelm) {Aelm.click();}</script>"
End if

%>
</BODY>
</HTML>
