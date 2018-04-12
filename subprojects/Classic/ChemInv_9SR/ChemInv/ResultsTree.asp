<%@ Language=VBScript %>
<%

dbname= Request.QueryString("dbname")
Select Case lcase(dbname)
	Case "invacx"
		color = "_red"
	Case "invreg"
		color = "_blue"
	Case "cheminv"
		color = ""
End select
%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script language="JavaScript">
	var MarkedHitsbuttonname = "show_marked<%=color%>";
	var ShowLastbuttonname = "show_last_list<%=color%>";
	var ClearHitsbuttonname = "clear_marked<%=color%>";

	if  (top.main.location.href.indexOf("<%=Application("appkey")%>_form_frset.asp") == -1){
		top.main.location.href = "<%=Application("appkey")%>_form_frset.asp?<%=request.QueryString%>";
	}
	theMainFrame = top.main.mainFrame;
</script> 
<%
Dim Conn
Dim RS
Dim LocationCount
Dim anchorIndex
Dim DBResultListBrake

' Add a line brake between the database names
DBResultListBrake = "<BR>"

'Prevent page from being cached
Response.ExpiresAbsolute = Now()

' Kill session if requested
if Request.QueryString("killsession")= 1 then 
	Session.Abandon
	Response.Write "Session Abandoned"
	Response.End
End if

CompoundID = Request.QueryString("CompoundID")
LocationID = Request.QueryString("LocationID")
ContainerID = Request.QueryString("ContainerID")
GotoNode = Request.QueryString("GotoNode") 
hitListID = Request.QueryString("hitListID")
ClearNodes = Request("ClearNodes")
lStyle = Request.QueryString("style")
If lStyle = "" Then lStyle = 5
Icon_clsd = "icon_clsdfold.gif" 
Icon_open = "icon_openfold.gif"
formgroup = Request.QueryString("formgroup")
if (formgroup = "plates_form_group") OR (formgroup = "plate_compounds_form_group") then
	showInList = "&showInList=plates"
	tableName = "inv_plates"
else
	showInList = "&showInList=containers"
	tableName = "inv_containers" 
end if
BuildListURL = "BuildList.asp"
%>
<HTML>
<HEAD>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<STYLE>
A {Text-Decoration: none;}
.TreeView {color:#000000; font-size:8pt; font-family: arial}
A.TreeView:LINK {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
A.TreeView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
A.TreeView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
</STYLE>
<SCRIPT LANGUAGE=JAVASCRIPT>
	<!--
	var PrevFolderImage
	var PrevLink
	var sNode = <%=sNode%>
	
	function OpenFolder(locationID){
		
		if (document.anchors(1)){
			var ename = "e" +  locationID
			var elm
			var elm2
			if (locationID == 0){
				elm = document.anchors(0);
				elm2 = document.anchors(1);
			}
			else{
				elm = document.anchors(ename);
				elm2 = elm;
			}
			elm2.style.color = "black"
			elm2.style.fontWeight = "bold"
			CurrFolderImage = elm.firstChild 
			elm.firstChild.src = "/ChemInv/images/treeview/<%=icon_open%>";
			if ((typeof(PrevFolderImage)== "object") && (PrevFolderImage!=CurrFolderImage))
			{
				PrevFolderImage.src = "/ChemInv/images/treeview/<%=icon_clsd%>";
				PrevLink.style.color = "#4682b4";
				PrevLink.style.fontWeight = ""
			}
		
			PrevLink = elm2;
			PrevFolderImage = elm.firstChild;
		}	
	}
	-->
</SCRIPT>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->

</HEAD>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<BODY TOPMARGIN=5 LEFTMARGIN=5 BGCOLOR="#FFFFFF">
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%

If formgroup = "base_form_group" OR formgroup="gs_form_group" OR formgroup="plate_compounds_form_group" then
	
	Select Case lcase(dbname)
		Case "invreg" 
			IdName = "RegID"
			SearchType = "ByRegID"
			fieldName = "Reg_ID_FK"
		Case "invacx"
			IdName = "ACXID"
			' need to translate CsNum into ACX_ID
			GetACXConnection()
			SQLQuery = "SELECT ACX_ID FROM substance WHERE CsNum = " & BaseID
			set RS = Conn.Execute(SQLQuery)
			BaseID = RS("ACX_ID")
			RS.Close
			set RS = nothing
			set Conn= nothing
			SearchType = "ByACXID"
			fieldName = "ACX_ID"	
		Case "cheminv" 
			IdName = "CompoundID"
			SearchType = "ByCompoundID"
			fieldName = "Compound_ID_FK"
			if (formgroup = "plate_compounds_form_group") then SearchType = "PlatesByCompoundID"
	End Select
Elseif InStr(1,formgroup, "containers_") > 0 then
	IdName = "ContainerID"
	ContainerID = BaseID
	SearchType = "ByContainerID"
Elseif InStr(1,formgroup, "plates_") > 0 then
	IdName = "PlateID"
	plateID = BaseID
	SearchType = "PlatesByPlateID"	
End If

Set TreeView = Server.CreateObject("VASPTV.ASPTreeView")

TreeView.Class = "TreeView"
TreeView.Style = clng(lStyle)
TreeView.LineStyle = 1
TreeView.ImagePath = "../images/treeview"
TreeView.AutoScrolling = True
TreeView.SingleSelect = False
TreeView.QueryString = ""
TreeView.LicenseKey = "8712-0DFC-5CEB"
TreeView.QueryString = "formgroup=" & formgroup & "&formmode=" & Request.QueryString("formmode") & "&dbname="  & Request.QueryString("dbname")& "&PagingMove=" & Request.QueryString("PagingMove") & "&BaseRecordCount=" & Request.QueryString("BaseRecordCount")

Set Nodex = TreeView.Nodes.Add( ,,0 , "Search results", Icon_clsd, Icon_clsd, , "ListFrame", "Containers matching your search found at all locations")
Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=OpenFolder(" & Nodex.Key &")"

Call BuildResultsTree(SearchType)
'Response.Write SearchType & "<BR>"
TreeView.Show
If TreeView.Nodes.Count = 1 then
	Response.Write "<span class=""GUIFeedback""><center><BR>There are no locations to display.</center></span>"
	Response.Write "<SCRIPT language=javascript>parent.ListFrame.location.href = '/cheminv/cheminv/BuildList.asp'; parent.TabFrame.location.href = '/cheminv/cheminv/SelectContainerMsg.asp';</SCRIPT>"
	Session("CurrentLocationID") = 0
	Session("CurrentConatinerID") = 0
End if
Set TreeView = Nothing

%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
<%
' It's nice to show where containers matching the search criteria are located within the location tree.
' What we are after is the full path to each end location where containers are found.
' Getting the path is not hard, but organizing all the nodes in hirarchical order and 
' eliminating redundant nodes gets a bit more tricky.
' RS will contain the hirarchically ordered set of nodes needed to depict the paths to the end locations.
' End locations are easily identified by their level in the connect by clause and hyperlinked.
' For generality the set of nodes is calculated starting from four different imput criteria (SearchTypes)
Sub BuildResultsTree(ByVal pSearchType)
		Call GetInvConnection()
		Select Case pSearchType
			Case "ByCompoundID","ByRegID"
				anchorIndex = 1
				sSQL = "SELECT Max(Location_barcode) AS Location_barcode, Max(Location_description) AS Location_Description, Max(inv_locations.Parent_ID) As parent_id, inv_locations.Location_ID As Id, Max(inv_locations.Location_Name) AS Location_Name, Max((SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk)) AS LocationTypeName, Min(Level) AS MaxLevel FROM inv_locations CONNECT BY Location_id = prior Parent_id START WITH Location_id IN (SELECT DISTINCT inv_locations.Location_ID FROM inv_locations, inv_containers WHERE inv_locations.Location_ID = inv_containers.Location_ID_FK AND inv_containers." & fieldName & "=" & BaseID & ") GROUP BY inv_locations.Location_ID HAVING Max(inv_locations.Parent_ID) IS NOT NULL ORDER BY Max(Level) DESC"
			Case "ByACXID"
				anchorIndex = 1
				sSQL = "SELECT Max(Location_barcode) AS Location_barcode, Max(Location_description) AS Location_Description, Max(inv_locations.Parent_ID) As parent_id, inv_locations.Location_ID As Id, Max(inv_locations.Location_Name) AS Location_Name, Max((SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk)) AS LocationTypeName, Min(Level) AS MaxLevel FROM inv_locations CONNECT BY Location_id = prior Parent_id START WITH Location_id IN (SELECT DISTINCT inv_locations.Location_ID FROM inv_locations, inv_containers, inv_compounds WHERE inv_locations.Location_ID = inv_containers.Location_ID_FK AND inv_containers.compound_id_fk = inv_compounds.compound_id AND inv_compounds." & fieldName & "='" & BaseID & "') GROUP BY inv_locations.Location_ID HAVING Max(inv_locations.Parent_ID) IS NOT NULL ORDER BY Max(Level) DESC"
			Case "ByContainerID"
				anchorIndex = "document.anchors.length-1"
				sSQL = "SELECT Max(Location_barcode) AS Location_barcode, Max(Location_description) AS Location_Description,  Max(inv_locations.Parent_ID) As parent_id, inv_locations.Location_ID As Id, Max(inv_locations.Location_Name) AS Location_Name, Max((SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk)) AS LocationTypeName, Min(Level) AS MaxLevel FROM inv_locations CONNECT BY Location_id = prior Parent_id START WITH Location_id IN (SELECT DISTINCT inv_locations.Location_ID FROM inv_locations, inv_containers WHERE inv_locations.Location_ID = inv_containers.Location_ID_FK AND inv_containers.Container_ID=" & ContainerID & ") GROUP BY inv_locations.Location_ID  ORDER BY Max(Level) DESC"
			Case "PlatesByCompoundID"
				anchorIndex = 1
				sSQL = "SELECT Max(Location_barcode) AS Location_barcode, Max(Location_description) AS Location_Description, Max(inv_locations.Parent_ID) As parent_id, inv_locations.Location_ID As Id, Max(inv_locations.Location_Name) AS Location_Name, Max((SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk)) AS LocationTypeName, Min(Level) AS MaxLevel FROM inv_locations CONNECT BY Location_id = prior Parent_id START WITH Location_id IN (SELECT DISTINCT inv_locations.Location_ID FROM inv_locations, inv_wells, inv_plates WHERE inv_locations.Location_ID = inv_plates.Location_ID_FK AND inv_plates.plate_id = inv_wells.plate_id_fk AND inv_wells.Compound_ID_FK=" & BaseID & ") GROUP BY inv_locations.Location_ID HAVING Max(inv_locations.Parent_ID) IS NOT NULL ORDER BY Max(Level) DESC"
			Case "PlatesByPlateID"
				anchorIndex = "document.anchors.length-1"
				sSQL = "SELECT Max(Location_barcode) AS Location_barcode, Max(Location_description) AS Location_Description,  Max(inv_locations.Parent_ID) As parent_id, inv_locations.Location_ID As Id, Max(inv_locations.Location_Name) AS Location_Name, Max((SELECT Location_Type_Name FROM inv_Location_Types WHERE Location_Type_ID = inv_Locations.Location_Type_ID_fk)) AS LocationTypeName, Min(Level) AS MaxLevel FROM inv_locations CONNECT BY Location_id = prior Parent_id START WITH Location_id IN (SELECT DISTINCT inv_locations.Location_ID FROM inv_locations, inv_plates WHERE inv_locations.Location_ID = inv_plates.Location_ID_FK AND inv_plates.plate_ID=" & plateID & ") GROUP BY inv_locations.Location_ID  ORDER BY Max(Level) DESC"
		End Select
		'Response.Write sSQL 
		'Response.end
		Set RS = Conn.Execute(sSQL)
		' Build the results tree from RS
		Do While Not RS.EOF
			level = RS("MaxLevel")
			'Response.Write RS("ID") & "-" & RS("parent_id") & "-" & RS("Location_Name").value & "-" & RS("MaxLevel").value & "<BR>"
			on error resume next
			Set Nodex = TreeView.Nodes.Add(RS("parent_id").value,4,RS("id").value,RS("Location_Name").value, Icon_clsd, Icon_clsd, , "ListFrame", "Containers matching your search found at: " & vblf & RS("LocationTypeName") & vblf & RS("Location_barcode").value & vblf & RS("Location_Description").value)
			Nodex.EnsureVisible
			if (CLng(level) = 1) then
				LocationCount = LocationCount + 1
				URL = BuildListURL & "?" & IdName & "=" & BaseID & "&" & Request.QueryString & "&LocationCount=1" & showInList' & "&LocationID=" & RS("id").value
				if pSearchType = "ByCompoundID" OR  pSearchType = "ByRegID" OR  pSearchType = "ByACXID" then URL = URL & "&LocationName=" & Nodex.Text & "&LocationID=" & Nodex.Key
				Nodex.URL = URL 
				Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=OpenFolder(" & Nodex.Key &")"
			End if			
			RS.MoveNext 
		Loop
		if pSearchType = "ByCompoundID" OR  pSearchType = "PlatesByCompoundID" OR pSearchType = "ByRegID" OR  pSearchType = "ByACXID" then
			TreeView.Nodes("0").URL = BuildListURL & "?" & IdName & "=" & BaseID & "&LocationCount=" & LocationCount & "&" & Request.QueryString & showInList
		End if
		if (ExpNode = "Y") then Session("PrevExpandedNodesList") = Session("PrevExpandedNodesList")& pNodeID & ":" 
		RS.Close
		Conn.Close
		Set RS = Nothing
		Set DB = Nothing
End Sub

%>
<SCRIPT LANGUAGE=javascript>
	if ((document.anchors("<%="e" & Session("CurrentLocationID")%>")) && (document.anchors("<%="e" & Session("CurrentLocationID")%>").href.length > 0)) {
		document.anchors("<%="e" & Session("CurrentLocationID")%>").click();
	}
	else if (document.anchors(<%=anchorIndex%>)){
		document.anchors(<%=anchorIndex%>).click();
	}
</SCRIPT>

</BODY>
</HTML>