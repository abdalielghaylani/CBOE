<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/RS2HTML.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
Dim rsRack
dbkey = "ChemInv"
LocationID = Request("LocationID")
RackIDList = Request("RackIDList")
PosRequired = Request("PosRequired")
refresh = Request("refresh")
ActionType = Request("ActionType")
IsMulti = Request("IsMulti")

if IsMulti = "" then IsMulti = "false"
if PosRequired = "" then PosRequired = 1

Call GetInvConnection()
displayFields = "Empty,Icon,Barcode"

'-- Set default selected view of Rack
if Session("viewRackFilter") = "" then
	viewRackFilter = "icon"
else
	viewRackFilter = Session("viewRackFilter")
end if

'strSQL = "select gl.location_id from inv_vw_grid_location gl where parent_id=2203 and gl.location_id not in (select location_id_fk from inv_containers where location_id_fk=gl.location_id) and gl.location_id not in (select location_id_fk from inv_plates where location_id_fk=gl.location_id) and gl.location_id not in (select parent_id from inv_locations where parent_id=gl.location_id) order by gl.row_index,gl.col_index"
'Response.write GetListFromSQL(strSQL)

Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRID(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, LocationID)
'-- CSBR ID:131045
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Modified to get the length of server name; it was previously hard coded to 30
'-- Date: 27/09/2010
Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, len(Application("RegServerName")), Application("RegServerName"))
'-- End of Change #131045#
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
	'Response.Write currRow & ":test"
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
				Title = "Select rack grid location " & GridID
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

%>
<html>
<head>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="javascript" src="/cheminv/choosecss.js"></script>
<script language="javascript" src="/cheminv/gui/refreshgui.js"></script>

<style type="text/css">
.bar	    {background-color:#fff;}
.roll	    {background-color:#c4df9b;}
}   
</style>

</head>
<body>

<form name="form1" action="echo.asp" xaction="NewLocation_action.asp" method="POST">
<input type="hidden" name="LocationID" value="<%=LocationID%>">
<input type="hidden" name="RackIDList" value="<%=Server.URLEncode(RackIDList)%>">
<div ID="waitGIF" ALIGN="center"><img src="<%=Application("ANIMATED_GIF_PATH")%>" WIDTH="130" HEIGHT="100" BORDER></div>
<%=xmlHtml%>
<script language="vbscript" runat="server">
'Function WrapRackContents(FldName, GridID, GridBarcode, GridType, strText, Title, Length, isSelected)
Function WrapRackContents(FldName, GridID, GridName, GridBarcode, GridType, strText, Title, Length, isSelected)
	Dim str
	if (strText = "") OR IsNull(strText) then strText = ""
	strText2 = "<a hfref=#>" & strText 
	if ActionType = "select" then
		if GridType <> "" then
			bgColor = "dedede"
		else
			bgColor = "transparent"
		end if
	end if
	str = "<span style=""width:100%; background:" & bgColor & ";"""
	if (len(strText) > Length) AND (strText <> "&nbsp;") AND (Title <> "&nbsp;") then 
		str = str & " title=""" & Title & """>"
		if ActionType = "select" then
			if GridType <> "" then
				str = str & strText
			else
				str = str & "<a class=""MenuLink"" href=""View Container Details" & GridID & """ onclick=""SelectContainer(" & GridID & "); return false;"">" & strText & "</a>"
			end if
		end if
	else
		if instr(GridName,"'") > 0 then
			GridName = replace(GridName,"'","\'")
		end if
		str = str & "><a class=""MenuLink"" href=""#"" title=""" & Title & """ onclick=""selectGrid(" & GridID & ",'" & GridName & "')"">" & strText & "</a>"
	end if
	
	str = str & "</span>"
	WrapRackContents = str
End function
</script>

<div id="titleBox" style="position:Absolute;top:5px;left:5px;visibility:visible;z-index=1">
<% 

'-- Handle counting and display of elements in grid
if cInt(cntRacksInRack) > 0 then cntRacksInRack = cntRacksInRack/3
if cInt(cntPlatesInRack) > 0 then cntPlatesInRack = cntPlatesInRack/3
if cInt(cntContainersInRack) > 0 then cntContainersInRack = cntContainersInRack/3
if cInt(cntRacksInRack) = 1 then strRacksInRack = cntRacksInRack & " Rack" else strRacksInRack = cntRacksInRack & " Racks" end if
if cInt(cntPlatesInRack) = 1 then strPlatesInRack = cntPlatesInRack & " Plate" else strPlatesInRack = cntPlatesInRack & " Plates" end if
if cInt(cntContainersInRack) = 1 then strContainersInRack = cntContainersInRack & " Container" else strContainersInRack = cntContainersInRack & " Containers" end if
Response.write(renderBoxHeader("<img src=""/cheminv/images/treeview/rack_closed.gif"">","Select a position from this rack: " & Request("LocationName"),"(" & strRacksInRack & ")&nbsp;(" & strPlatesInRack & ")&nbsp;(" & strContainersInRack & ")","631px"))
'Response.write(renderBoxHeader("<img src=""/cheminv/images/treeview/rack_open.gif"">","Select a position for this rack","","631px")) 

%>
</div>
<div id="rackSelectList" style="position:Absolute;top:20px;left:5px;visibility:visible;z-index=1">
<%

'-- If user multiselects Racks, display those Racks, otherwise display all Racks
if IsMulti then 
	strCriteria = " and l.location_id in (" & RackIDList & ")"
else
	strCriteria = ""
end if
strSQL = "select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) as DisplayText from inv_grid_storage s, inv_locations l where s.location_id_fk = l.location_id and l.collapse_child_nodes=1 " & strCriteria & " order by DisplayText"
'Response.Write(strSQL)
'Response.End
rackSelectHtml = "<br /><table><tr>" & ShowPickList2("<strong>Select Rack:</strong>", "RackID", LocationID,strSQL,100,"","","refreshRack()") & "</tr></table>"
Response.Write(rackSelectHtml)

%>
</div>
<div id="rackViewer" style="position:Absolute;top:70px;left:5px;visibility:visible;z-index=1">
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
			<div id="hiddenSelector" style="POSITION:Absolute;top:0;left:0;visibility:hidden;z-index=2">
			<select id="cboField" size="7">	
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
<div align="right" style="margin-top:10px;"><a href="#" onclick="top.opener.focus();window.close(); return false;"><img SRC="../graphics/sq_btn/cancel_dialog_btn.gif" border="0" WIDTH="53" HEIGHT="21"></a></div>
</div>

<script LANGUAGE="javascript">

function selectGrid(GridID,GridName) {
	var isMulti = "<%=IsMulti%>";

	if (top.opener.document.form1.SelectRackBySearch) {
		if (top.opener.document.form1.SelectRackBySearch.checked) {
			rackField = top.opener.document.form1.SuggestedRackID;
		}
	} else {
		rackField = top.opener.document.form1.RackID;
	}
	if (top.opener.document.form1.SelectRackByLocation) {
		if (top.opener.document.form1.SelectRackByLocation.checked) {
			rackField = top.opener.document.form1.RackID;
		}
	}else {
		rackField = top.opener.document.form1.RackID;
	}
	//alert(rackField);
	if (isMulti=="false"){
	
		// SINGLE Rack position update
		top.opener.document.form1.iRackGridID.value=GridID; 
		top.opener.document.form1.iRackGridName.value=GridName; 
		top.opener.focus(); 
		rackField.selectedIndex=document.form1.RackID.selectedIndex;
		window.close();
		
	} else {
	
		// MULTI Rack position updates
		var RackIDList = "<%=RackIDList%>";		
		var PosRequired = <%=PosRequired%>;
		var PosRequiredList = "";
		var CurrRackID = document.form1.RackID.options[document.form1.RackID.selectedIndex].value;
		var tmpRackIDList = RackIDList.replace(CurrRackID,'');
		
		// Get open positions for selected Rack starting position
		var tmpOpenRackList = GetOpenRackPositons(CurrRackID,GridID);
		var tmpArrOpenRackList = tmpOpenRackList.split(",")
		for (j=0;j<tmpArrOpenRackList.length;j++){
			// Populate list of required Rack Grid ID's
			if (PosRequired > 0){
				if (PosRequiredList.length > 0){
					PosRequiredList = PosRequiredList + ",";
				}
				PosRequiredList = PosRequiredList + tmpArrOpenRackList[j];
				PosRequired--;
			}else{
				break;
			}
		}

		// Get open positions for remaining Racks and fill # required positions
		var arrRackIDList = tmpRackIDList.split(",");
		if (PosRequired > 0){
			for (i=0;i<arrRackIDList.length;i++){
				if (arrRackIDList[i].length > 0){
					tmpOpenRackList = GetOpenRackPositons(arrRackIDList[i],0);
					tmpArrOpenRackList = tmpOpenRackList.split(",")
					for (k=0;k<tmpArrOpenRackList.length;k++){
						// Populate list of required Rack Grid ID's
						if (PosRequired > 0){
							if (PosRequiredList.length > 0){
								PosRequiredList = PosRequiredList + ",";
							}
							PosRequiredList = PosRequiredList + tmpArrOpenRackList[k];
							PosRequired--;
						}else{
							break;
						}
					}
				}
			}
		}
		
		if (PosRequired > 0){
			alert("There are not enough open Rack positions to fill your request.\rPlease choose a different starting position.");
		}else{
			// Populate form fields in previous window
			top.opener.document.form1.iRackGridID.value=GridID; 
			top.opener.document.form1.iRackGridName.value=GridName + " in " + document.form1.RackID.options[document.form1.RackID.selectedIndex].text; 
			top.opener.document.form1.RackGridList.value=PosRequiredList;
			if (top.opener.document.form1.LocationName && top.opener.document.form1.LocationName.value.length == 0){
				top.opener.document.form1.LocationName.value = GridName;
			}
			top.opener.focus(); 
			RackIDList = RackIDList.split(",");
			
			// Highlight selected Racks from previous window
			for(l=0;l<rackField.length;l++){
				var tmpTopRackItem = rackField.options[l].value;
				for (m=0;m<RackIDList.length;m++){
					if (tmpTopRackItem == RackIDList[m]){
						rackField.options[l].selected=true;
					}
				}	
			}
			window.close();
		}

	}
}

function refreshRack() {
	var RackIDList = "<%=RackIDList%>";
	var index = document.form1.RackID.selectedIndex;
	var selectedText = document.form1.RackID.options[index].text;
	var selectedValue = document.form1.RackID.options[index].value;
	var pageUrl = 'ViewRackLayout.asp?ActionType=select&PosRequired=<%=PosRequired%>&IsMulti=<%=IsMulti%>&LocationID='+selectedValue+'&LocationName='+selectedText+'&RackIDList='+RackIDList;
	window.location=(pageUrl);
}

function SelectPlate(elm, PlateID, locationID, Refresh){
	//var ImageOpen = "<%=ImagePath%>/<%=Icon_Open%>";
	//var ImageClsd = "<%=ImagePath%>/<%=Icon_Clsd%>";
	//OpenFolder(elm, ImageOpen, ImageClsd);
	if (parent.TabFrame) parent.TabFrame.location.href = "/cheminv/gui/ViewPlate.asp?PlateID=" + PlateID + "&refresh=" + Refresh;
}

// Hide the wait gif
document.all.waitGIF.style.display = "none";

// Open the selected rack if there is one
SelectRack = "<%=Session("SelectRack")%>";

if (SelectRack != "0") {
	var hiddenRackSelector = document.anchors("hiddenRackSelector");
	hiddenRackSelector.click();
}

document.all.cboField.options[1].selected = true;
tbl.dataFld = "<%=viewRackFilter%>";
tbl.dataSrc = "#xmlDoc"; 
</script>

<script for="cboField" event="onchange">
  tbl.dataSrc = ""; // unbind the table

  // set the binding to the requested field
  tbl.dataFld = this.options(this.selectedIndex).value;

  tbl.dataSrc = "#xmlDoc"; // rebind the table
  document.all.hiddenSelector.style.visibility = 'hidden';
  wellFilter = tbl.dataFld;

</script>

</body>
</html>
