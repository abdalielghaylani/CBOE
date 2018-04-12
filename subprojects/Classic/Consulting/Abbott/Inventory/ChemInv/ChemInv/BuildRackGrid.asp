
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/RS2HTML.asp"-->
<%
Dim rsRack
dbkey = "ChemInv"
LocationID = Request("LocationID")
refresh = Request("refresh")
errorMSG = "Duplicate items are contained in the following position(s). Please contact administrator and send the following message:<br /><br />"
displayErrorMSG = false
Call GetInvConnection()
displayFields = "Empty,Icon,Barcode,CellName,A-Code,Lot-Number"

'-- Set default selected view of Rack
if Session("viewRackFilter") = "" then
	viewRackFilter = "icon"
else
	viewRackFilter = Session("viewRackFilter")
end if

'-- Get list of containers that are duplicated in Rack grids
DuplicatesList = ""
DuplicatesList = GetDuplicateRackContainerID(LocationID)

'-- Get data to render Rack
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRID(?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, LocationID)
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Server.CreateObject("ADODB.Recordset")
RS.CursorLocation = aduseClient
RS.LockType = adLockOptimistic
RS.Open Cmd
RS.ActiveConnection = Nothing
RS.filter = "col_index=1"

rowName_arr = RS.GetRows(adGetRowsRest, , "RowName")

numRows = Ubound(rowName_arr,2) + 1 
RS.filter = 0
RS.Movefirst
RS.filter = "ROW_INDEX=1"
colName_arr = RS.GetRows(adGetRowsRest, , "ColName")
NumCols = Ubound(colName_arr,2) + 1
if NumCols > 15 then
	cellWidth = 80
else
	cellWidth = 950/NumCols
end if
cellWidthLucidaChars = cellWidth/6

FldArray = split(displayFields,",")
xmlHtml = ""
xmlHtml = xmlHtml & "<xml ID=""xmlDoc""><rack>" & vbcrlf

cntRacksInRack = 0
cntPlatesInRack = 0
cntContainersInRack = 0

cellSeqNaming = false
strRowNames = ""
For currRow = 1 to numRows
	For i = 0 to Ubound(FldArray)
		FldName = FldArray(i)
		RS.filter = 0
		RS.Movefirst
		RS.filter = "row_index=" & currRow
		rowName = RS("ROWNAME") 
		xmlHtml = xmlHtml & "<" & FldName & ">" & vblf
		xmlHtml = xmlHtml & "<rowname>" & rowname & "</rowname>"
		strRowNames = strRowNames & rowname & ","
		rackCriterion = Request("RackCriterion")
		if len(rackCriterion) > 0 then
			key = left(rackCriterion,instr(rackCriterion,",")-1)
			value = right(rackCriterion,len(rackCriterion) - instr(rackCriterion,","))
			bCheckSelected = true
		end if
		hasDups = false
		bReassignGridID = false
		While NOT RS.EOF
			if RS("cell_naming") = "1" then 
				cellSeqNaming = true
			end if
			GridData = RS("grid_data").value
			if isBlank(GridData) then
				GridID = ""
				GridBarcode = ""
				Title = RS("name")
				theValue = ""
			else
				arrGridData = split(GridData,"::")
				execute("cnt" & arrGridData(0) & "sInRack = cnt" & arrGridData(0) & "sInRack + 1")
				GridID = arrGridData(1)
				GridBarcode = arrGridData(2)
				GridElementName = arrGridData(4)
				if arrGridData(0) <> "Rack" then RegNumber = arrGridData(20)
				GridBatchField1 = arrGridData(9)
				GridBatchField2 = arrGridData(11)
				GridType = lCase(arrGridData(0))
				Title = "View " & arrGridData(0) & ":" & vblf & "--------------------" & vblf & "Location: " & RS("name") & vblf & arrGridData(0) & " Barcode: " & GridBarcode
				if GridElementName <> "" then Title = Title & vblf & "Name: " & GridElementName

				if GridType = "container" then
					duplicateElementIDs = ""
					if DuplicatesList <> "" then
						arrDuplicatesList = split(DuplicatesList,",")
						for l = 0 to ubound(arrDuplicatesList)
							arrDupValues = split(arrDuplicatesList(l),"::")
							dupContainerID = arrDupValues(0)
							dupRackGridID = arrDupValues(1)
							dupRackName = arrDupValues(2)
							if dupRackName = RS("name") then
								if duplicateElementIDs <> "" then
									duplicateElementIDs = duplicateElementIDs & "," & dupContainerID
								else
									duplicateElementIDs = duplicateElementIDs & dupContainerID
								end if
								bReassignGridID = true
							end if
						next
					end if
					if bReassignGridID then 
						GridID = duplicateElementIDs
						hasDups = true
					end if
					bReassignGridID = false
				end if
			
				if FldName = "Barcode" then
					theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & GridBarcode
				elseif FldName = "A-Code" then
					theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & RegNumber
				elseif FldName = "Lot-Number" then
					theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & GridBatchField2
				elseif FldName = "CellName" then
					theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & RS("name")
				else
					theValue = "<img src=""" & arrGridData(3) & """ border=""0"">&nbsp;"
					if GridType = "rack" then
						theValue = theValue & Left(GridElementName,10)
						if Len(GridElementName) > 10 then theValue = theValue & "..."
					else
						theValue = theValue & GridBarcode
					end if
				end if
			end if
			
			isSelected = false
			if bCheckSelected then
				keyValue = RS(key).Value
				if isNull(keyValue) then keyValue = ""	
				if cstr(keyValue) = cstr(value) then isSelected = true
			end if
			theValue = "<![CDATA[" & WrapRackContents(FldName, GridID, GridBarcode, GridType, theValue, Title, cellWidthLucidaChars, isSelected, hasDups, RS("name")) & "]]>"
			colIndex = RS("COL_INDEX")
			xmlHtml = xmlHtml & "<col" & colIndex & ">" & theValue & "</col" & colIndex & ">" & vblf

			hasDups = false

			RS.MoveNext		
		Wend
		xmlHtml = xmlHtml & "</" & FldName & ">" & vblf
	Next
Next
xmlHtml = xmlHtml & "</rack></xml>"

'-- If naming sequentially, remove rownames
if cellSeqNaming then
	arrRowNames = split(strRowNames,",")
	for i=0 to ubound(arrRowNames)
		xmlHtml = replace(xmlHtml,"<rowname>" & arrRowNames(i) & "</rowname>","<rowname></rowname>")
	next
end if
%>
<html>
<head>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="javascript" src="/cheminv/choosecss.js"></script>
<script language="javascript" src="/cheminv/gui/refreshGUI.js"></script>

<script language="javascript">
<!--
	lastSelectedGrid=0;
	function highlightGrid(GridID) {
		if (lastSelectedGrid > 0) { document.getElementById(lastSelectedGrid).className='filledGrid'; }
		changeClass(GridID,'selectedGrid');
		lastSelectedGrid = GridID;
	}

	function changeClass(ElementName,ClassName){
		document.getElementById(ElementName).className=ClassName;
	}
// -->
</script>


<style>
.errorGrid {
	width:100%;
	background: #CC0000;
}
.errorGrid a {
	color: #fff;
	font-weight: bold;
}

.filledGrid {
	width:100%;
	background:#eaeaff;
}
.emptyGrid {
	width:100%;
	background:#ffffff;
}
.selectedGrid {
	width:100%;
	background:#FFFF66;
}

</style>

</head>
<body>

<form name="form1" method="POST" target="TabFrame">
<input type="hidden" name="LocationID" value="<%=LocationID%>">
<input type="hidden" name="removeList">
<div id="waitGIF" align="center"><img src="/cfserverasp/source/graphics/processing_Ybvl_Ysh_grey.gif" WIDTH="130" HEIGHT="100" BORDER=""></div>
<%=xmlHtml%>
<script language=vbscript runat=server>
Function WrapRackContents(FldName, GridID, GridBarcode, GridType, strText, Title, Length, isSelected, hasDups, position)
	Dim str
	if (strText = "") OR IsNull(strText) then strText = ""
	strText2 = "<a hfref=#>" & strText 
	if hasDups = "True" then
		className="errorGrid"
		elementName="filledGridName"
		if instr(errorMSG,GridID) = 0 then
			errorMSG = errorMSG & " - Container ID's, " & GridID & ", are dupliated in position " & position & " of Rack " & LocationID & vbcrlf
		end if
		Title = replace(errorMSG,"<br />",vbcrlf)
		GridID = "Error"
		strText = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;Error"
		displayErrorMSG = true
	elseif GridID <> "" then
		className="filledGrid"
		elementName="filledGridName"
	else
		className="emptyGrid"
		elementName="emptyGridName"
	end if
	
	str = "<span id=""" & GridID & """ name=""" & elementName & """ class=""" & className & """ "
	str = str & " title=""" & Title & """>"
	if GridID <> "" then 
		if Session("bMultiSelect") and (GridType="plate" or GridType="container") then
			if hasDups = "True" then
				str = str & "<a href=""#"">" & strText & "</a>"
			else
				str = str & "<input type=""checkbox"" name=""selectChckBox""  value=""" & GridID & """ onclick=""Removals('" & GridID & "', this.checked);SelectMarked('" & GridType & "');"" />"
			end if
		end if 
		if hasDups = "True" then
			str = str & "<a href=""#"">" & strText & "</a>"
		elseif GridType = "rack" then
			str = str & "<a href=""#"" onclick=""SelectLocation(" & GridID & ", '', '')"">" & strText & "</a>"
		elseif GridType = "plate" then
			str = str & "<a href=""View Plate Details"" target=""TabFrame"" onclick=""SelectPlate(this," & GridID & ", " & LocationID & ",'True'); return false;"">" & strText & "</a>"
		elseif GridType = "container" then
			str = str & "<a href=""View Container Details " & GridBarcode & """ onclick=""SelectContainer(" & GridID & "); highlightGrid(" & GridID & "); return false; "">" & strText & "</a>"
		end if
	else
		str = str & "<font color=""#BABABA"">" & Title & "</font>"
	end if
	str = str & "</span>"
	WrapRackContents = str
End function
</script>

<div id="titleBox" style="position:Absolute;top:5px;left:5px;visibility:visible;z-index=1">
<%
'-- Handle counting and display of elements in grid
if cInt(cntRacksInRack) > 0 then cntRacksInRack = cntRacksInRack/6
if cInt(cntPlatesInRack) > 0 then cntPlatesInRack = cntPlatesInRack/6
if cInt(cntContainersInRack) > 0 then cntContainersInRack = cntContainersInRack/6
if cInt(cntRacksInRack) = 1 then strRacksInRack = cntRacksInRack & " Rack" else strRacksInRack = cntRacksInRack & " Racks" end if
if cInt(cntPlatesInRack) = 1 then strPlatesInRack = cntPlatesInRack & " Plate" else strPlatesInRack = cntPlatesInRack & " Plates" end if
if cInt(cntContainersInRack) = 1 then strContainersInRack = cntContainersInRack & " Container" else strContainersInRack = cntContainersInRack & " Containers" end if
Response.write(renderBoxHeader("<img src=""/cheminv/images/treeview/rack_open.gif"">",Request("LocationName"),"(" & strRacksInRack & ")&nbsp;(" & strPlatesInRack & ")&nbsp;(" & strContainersInRack & ")","631px"))

%>
</div>

<div style="position:Absolute;top:38px;left:5px;visibility:visible;z-index=1;font-size:11px;">&nbsp;
<% if Session("bMultiSelect") then %>
	<A CLASS="MenuLink" HREF="#" onclick="CheckAll(true); return false">Select All</A>
	|
	<A CLASS="MenuLink" HREF="#" onclick="CheckAll(false); return false">Clear All</A>
	|
	<a class="MenuLink" HREF="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame">Cancel MultiSelect</a>					
<% else %>
	<a href="BuildList.asp?view=3&multiSelect=1&LocationID=<%=Locationid%>&LocationName=<%=Request("LocationName")%>&showInList=racks">Multi-Select</a> | 
	<a href="Print Rack Summary" onclick="OpenDialog('/cheminv/gui/ViewSimpleRackLayout.asp?rackid=<%=LocationID%>&locationid=<%=LocationID%>&containerid=&RackPath=&Summary=(&nbsp;<%=strRacksInRack%>&nbsp;)&nbsp;(&nbsp;<%=strPlatesInRack%>&nbsp;)&nbsp;(&nbsp;<%=strContainersInRack%>&nbsp;)', 'Diag1', 2); return false;">Print Rack</a>&nbsp;&nbsp;&nbsp;&nbsp;
<% end if %>
</div>

<div id="sorttext" style="margin-left: 350px; float:left; position:Absolute;top:38px;left:5px;visibility:visible;z-index=1;font-size:11px;">
Rack displayed by <strong>Icon</strong>. To change the display, click on the arrow.</div>

<div id="rackViewer" style="position:Absolute;top:55px;left:5px;visibility:visible;z-index=1">
<table style="font-size:7pt; font-family: verdana; table-layout:fixed; border-collapse: collapse;" cellspacing="0" cellpadding="0" bordercolor="#999999" id="tbl" datasrc datafld="icon" border="1">
	<col width="30">
	<%
		For i=0 to NumCols-1
			Response.Write "<col width=""" & cellWidth & """>"
		Next
	%>
	<thead>
		<th align="center">
			<a href="#" onclick="document.all.hiddenSelector.style.visibility = 'visible';document.all.cboField.click()" title="Click to select displayed value"><img src="/cheminv/graphics/desc_arrow.gif" border="0" width="12" height="6"></a>
			<a id="hiddenRackSelector" target="rackJSFrame"></a>
			<div id="hiddenSelector" style="position:Absolute;top:0;left:0;visibility:hidden;z-index=2">
			<select id="cboField" size="7">	
				<option value></option>
				<option value="Icon">Icon</option>
				<option value="Barcode">Barcode</option>
				<option value="Cellname">Cell Name</option>
				<option value="A-Code">A-Code</option>
				<option value="Lot-Number">Lot Number</option>
			</select>
			</div>
		</th>
	<%
		For i=0 to NumCols-1
			Response.Write "<th>"
			if not cellSeqNaming then
				Response.write(colName_arr(0,i))
			end if
			Response.Write "</th>" & vblf
		Next
	%>
	</thead>
	<tr height="20">
		<th><span datafld="rowname"></span></th>
		<%
		For i=1 to NumCols
			Response.Write "<td align=""center"" valign=""center""><div class=""col" & i & """ DATAFORMATAS=html DATAFLD=""col" & i &"""></div></td>" & vblf
		Next
		%>
	</tr>
</table>
<% if displayErrorMSG then %>
<span style="font-size: 10px; display: block; margin-top: 3px; ">
	<div style="display: block; height: 15px; width: 15px; float: left; background: #CC0000;">&nbsp;</div>
<%=replace(errorMSG,"<br /><br />","<br />")%>
</span>
<% end if %>

</div>

<script language="javascript">

if (parent.TabFrame) parent.TabFrame.location.href = '/cheminv/cheminv/SelectContainerMsg.asp?entity=Rack grid location';

function SelectPlate(elm, PlateID, locationID, Refresh){
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

<% if SelectContainer <> "" and SelectContainer <> "0" then %>
<script language="javascript">
  SelectContainer(<%=SelectContainer%>);
</script>
<% end if %>

<SCRIPT LANGUAGE=javascript>
	TabFrameURL = "<%=Session("TabFrameURL")%>";
	if (TabFrameURL != "") {
		//refresh tab frame
		document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
		document.form1.submit();	
	}
	function SelectMarked(type){
	
		// Restrict batch Rack grid options to Containerse for now
		if (type == 'container') {
			var reml = document.form1.removeList;
			var len = reml.value.length
			if (reml.value.substring(len -1 , len) == ","){
				reml.value = reml.value.substring(0, len - 1);
			}
			document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
			elm = document.form1.selectChckBox;
			<%if bDisableChkBoxes then%>
				//MCD: added check for case where there's only one element
				if (elm.length){
					for (i=0; i < elm.length ; i++){
						elm[i].disabled = false;
					}
				}
				else{
					elm.disabled = false;
				}
			<%end if%>
			document.form1.submit();
			<%if bDisableChkBoxes then%>
				//MCD: added check for case where there's only one element
				if (elm.length){
					for (i=0; i < elm.length ; i++){
						elm[i].disabled = true;
					}
				}
				else{
					elm.disabled = true;
				}
			<%end if%>
			reml.value ="";
		} 
	}
			
	function CheckAll(bCheck){
		var cbObj = document.form1.selectChckBox;
						
		if (cbObj.length){
			for (i=0; i< cbObj.length; i++){
				if (cbObj[i].checked ^ bCheck){
					cbObj[i].checked = bCheck;
					if (!bCheck) Removals(cbObj[i].value, false);
				}
			}
		}
		else{
			if (cbObj.checked ^ bCheck){
					cbObj.checked = bCheck;
					if (!bCheck) Removals(cbObj.value, false);
				}
		}
		SelectMarked('container');	
	}
				
	function Removals(id, bRemove){
		var idc = id + ",";
		var reml = document.form1.removeList;
			
		if (bRemove){
			if (reml.value.indexOf(idc) >=0){
				reml.value = reml.value.replace(idc,"");
			}
		}
		else{
			if (reml.value.indexOf(idc) ==  -1){
				reml.value += id + ",";
			}
		}
		//alert(reml.value);				
	}
	function HappySound(){
		snd1.src='/cheminv/sounds/yes1.wav';
		return true;
	}
	
	function SadSound(){
		snd1.src='/cheminv/sounds/no1.wav';
	}
			
	function SadSound2(){
		snd1.src='/cheminv/sounds/no2.wav';
	}

</SCRIPT>

</body>
</html>
