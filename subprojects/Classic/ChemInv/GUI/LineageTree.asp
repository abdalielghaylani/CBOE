<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

assetType = Request("assetType")
selectedID = Request("selectedID")
selectedRootID = Request("selectedRootID")
refresh = Request("refresh")
selectedNodeID = ""

if assetType = "container" then
	Icon_clsd = "flask_closed_icon_16.gif" 
	Icon_open = "flask_open_icon_16.gif"
	IsPlateView=0
elseif assetType = "plate" then 
	Icon_clsd = "plate_icon_16.gif" 
	Icon_open = "plate_icon_16.gif"
	IsPlateView=1
elseif assetType = "well" then
	Icon_clsd = "plate_icon_16.gif" 
	Icon_open = "plate_icon_16.gif"
	IsPlateView=0
end if


%>
<HTML>
<HEAD>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
var assetType = '<%=assetType%>';
var selectedID = '<%=selectedID%>';
function ChangeRoot(value)
{
	//alert(value);
	document.location = 'LineageTree.asp?assetType=' + assetType +  '&selectedID=' + selectedID + '&selectedRootID=' + value + '&refresh=1';
}

</script>
<STYLE>
	A {Text-Decoration: none;}
	.TreeView {color:#000000; font-size:8pt; font-family: arial}
	A.TreeView:LINK {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
	A.TreeView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
	A.TreeView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
</STYLE>
</HEAD>
<BODY TOPMARGIN=0 LEFTMARGIN=5 BGCOLOR="#FFFFFF" >
<table align="center">
<tr>
<%
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".GUIUtils.GetRootNodes(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_SELECTEDID",131, 1, 0, selectedID)
Cmd.Parameters.Append Cmd.CreateParameter("P_ASSETTYPE",200, 1, 30, assetType)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set rsRoot = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	
	rootCount = 0
	if not rsRoot.EOF then
		while (not rsRoot.EOF)
			if rootCount = 0 then firstRootID = rsRoot("id")
			rootCount = rootCount + 1
			rsRoot.moveNext
		wend
		rsRoot.moveFirst
		if isEmpty(selectedRootID) or selectedRootID="" then selectedRootID = firstRootID
	else
		if isEmpty(selectedRootID) or selectedRootID="" then selectedRootID = selectedID
	end if		
	
	if rootCount > 1 then 
		Response.Write "<td valign=""top""><select name=""root"" onchange=""ChangeRoot(this.value);"">"
		while (not rsRoot.EOF)
			selectText = ""
			if cstr(rsRoot("id")) = cstr(selectedRootID) then selectText = "SELECTED"
			Response.Write "<option value=""" & rsRoot("id") & """ " & selectText & ">" & rsRoot("displayText")
			rsRoot.moveNext
		wend 
		Response.Write "</select><td>"
	end if

	' Create the TreeView Component.
	Set TreeView = Server.CreateObject("VASPTV.ASPTreeView")
	' Tells the component which stylesheet to use.
	TreeView.Class = "TreeView"
	TreeView.Style = 7
	TreeView.LineStyle = 1
	TreeView.ImagePath = "../images/treeview"
	TreeView.AutoScrolling = True
	TreeView.SingleSelect = False
	TreeView.QueryString = "assetType=" & assetType & "&selectedID=" & selectedID & "&selectedRootID=" & selectedRootID
	TreeView.LicenseKey = "8712-0DFC-5CEB"

	rootID = selectedRootID
	'call PopulateNodeLayer("NULL","<=3", assetType)
	call BuildTree(assetType)
	Response.Write "<td>"
	TreeView.Show
	Response.Write "</td>"
	set TreeView = nothing
end if

'on error resume next
'Response.Write TreeView.Nodes.Count
'source = TreeView.Style

 

'Sub BuildTree(pSQL)
Sub BuildTree(assetType)

	If refresh = "1" or IsEmpty(Session("LineageNodes")) Then	
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".GUIUtils.GetLineage(?,?)}", adCmdText)	
		Cmd.Parameters.Append Cmd.CreateParameter("P_SELECTEDID",131, 1, 0, rootID)
		Cmd.Parameters.Append Cmd.CreateParameter("P_ASSETTYPE",200, 1, 30, assetType)
		if bDebugPrint then 
			For each p in Cmd.Parameters
				Response.Write p.name & " = " & p.value & "<BR>"
			Next	
			'Response.End
		end if
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If (RS.EOF AND RS.BOF) then
			Response.Write ("<center><table><TR><TD align=center colspan=6><span class=""GUIFeedback"">No container requests found for this container.</Span></TD></tr></table></center><BR><BR>")
		end if
	
		rootNodes = ""
		i = 0
		Do While Not RS.EOF
			i = i + 1
			tempID = RS("id")
			id = mid(tempID, 2, len(tempID))
			dbParentID = RS("Parent_ID")
			if isNull(dbParentID) then 
				ParentID = null
				rootNodes = rootNodes & "," & id
			elseif cstr(dbParentID) = id then
				ParentID = null
				rootNodes = rootNodes & "," & id
			elseif instr(rootNodes, cstr(dbParentID)) > 0  then ' = rootID) then
				'-- if this is a leaf directly below the root node modify the parent id and id from the RS
				ParentID = cstr(dbParentID)
				'id = cstr(dbParentID) & "_" & id
				'Response.Write "<BR>" & tempID & ":" & dbParentID & ":" & selectedID
				'Response.Write (dbParentID = selectedID) & "<BR>"
			else
				tempParentID = id
				'-- get to the ParentID by pruning the current leaf
				lastUnderscore = instrrev(tempParentID, "_") - 1
				'Response.Write "<BR>" & lastUnderscore & ":" & RS("Parent_ID")
				if isNull(dbParentID) then
					ParentID = null
				else
					ParentID = left(tempParentID,lastUnderscore) 
				end if
			end if
			'ParentID = RS("Parent_ID")
			
			if bDebugPrint then
				Response.Write "<BR>" & i & ". " & id & "-" & ParentID & "-" & RS("barcode") & "-" & RS("child_id")
			Else
				'stop
				'Response.Write "<BR>i=" & i & "<BR>id=" & id & "<BR>parentID=" & ParentID & "<BR>barcode=" & RS("barcode") & "<BR>childPlateID=" & RS("child_plate_id_fk")
				'Response.Write "<BR>" & i & ". " & id & "-" & ParentID & "-" & RS("barcode") & "-" & RS("child_plate_id_fk")
				'on error resume next			
				' Add a Node to the Tree.
				id = rootID & "_" & id
				if not isNull(parentID) then
					parentID = rootID & "_" & parentID
				end if
				pk = RS("pk")
				bSelected = false
				if selectedID = cstr(pk) then
					selectedNodeID = id
					bSelected = true
				end if
				'Response.Write id & "<BR>"
				' Syntax: TREEVIEW.NODES.ADD ([Relative],[Relationship],[Key],[Text],[Image],[ExpandedImage],[URL],[Target],[ToolTipText])
				Set Nodex = TreeView.Nodes.Add(ParentID,4,id,Left(RS("barcode"),200), Icon_clsd, Icon_open, , , RS("locationPath"))			
				'Nodex.URL = NodeURL & "?LocationID=" & Nodex.Key & "&LocationName=" & Server.URLEncode(Nodex.Text)
				Nodex.URL = "#"
				if bSelected then 
					Nodex.Bold = true
					Nodex.ForeColor = "red"
				
				end if
				Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=""opener.SelectLocationNode4(0," & RS("Location_id_fk") & ", 0, '" & TreeViewOpenNodes1 & "'," & RS("pk") & "," & IsPlateView & ");window.close();"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"""
				'Nodex.Target = NodeTarget
			End if
			Set Nodex = nothing
			RS.MoveNext 
		Loop
	
		RS.Close
		Conn.Close
		Set RS = Nothing
		Set Conn = Nothing
		Set Session("LineageNodes") = TreeView.Nodes
	else
		Set TreeView.Nodes = Session("LineageNodes")	
	end if
End Sub

		'Response.Write pk & ":" & selectedNodeId & "<BR>"

'Open the node for the selected item
if isEmpty(Request("Exp")) then
	response.write "<Script language=javascript>"
	'Response.Write "var Aelm = document.anchors(""e" & selectedNodeID & """); "
	'Response.Write "if (Aelm){"
	Response.Write "document.location = document.location + ""&sNode=" & selectedNodeID & "&Exp=Y#" & selectedNodeID & """;"
	Response.Write "</script>"
	
	'Response.Write "alert(document.location);</script>"
	'Response.Write "Aelm.click();}</script>"
end if
%>

</tr>
</table>

</BODY>
</HTML>
