<% 
Dim rsRack
dbkey = "ChemInv"
LocationID = Request("LocationID")
refresh = Request("refresh")
errorMSG = "Duplicate items are contained in the following position(s). Please contact administrator and send the following message:<br /><br />"
displayErrorMSG = false
Call GetInvConnection()

if ucase(Application("RegServerName")) <> "NULL" then
    displayFields = "Empty,Icon,Barcode,CellName,RegNumber,BatchNumber"
else
    displayFields = "Empty,Icon,Barcode,CellName"
end if    
'Following Lines are Added to Fix Bugs 78538,78541,78536 
'Start
SQL = "SELECT Distinct l.row_index as RowIndex FROM inv_vw_grid_location l,	inv_locations p,inv_grid_storage s,	inv_grid_format f "&_
"WHERE l.parent_id = p.location_id	 AND l.parent_id = s.location_id_fk	 AND s.grid_format_id_fk = f.grid_format_id AND l.parent_id =? order by RowIndex"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("parent_id", adNumeric, 1, 0, LocationID)
Set RS = Cmd.Execute
if RS.EOF then
    response.Write "<BR><BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP><span class='GuiFeedback'>You do not have privilege to view this location.</span></TD></TR></TABLE>"
    Response.end
end if
rowName_arr = RS.GetRows(adGetRowsRest, , "RowIndex")
numRows = Ubound(rowName_arr,2) + 1 
Set RS=Nothing
SQL="select Distinct COL_NAME as ColName," &_
"(SELECT Max(l.col_index) as NumCols FROM inv_vw_grid_location l,inv_locations p,inv_grid_storage s,inv_grid_format f WHERE l.parent_id = p.location_id	 AND l.parent_id = s.location_id_fk	 AND s.grid_format_id_fk = f.grid_format_id AND l.parent_id =? ) as col_index " &_
"from inv_grid_storage,inv_grid_position where inv_grid_position.GRID_FORMAT_ID_FK=inv_grid_storage.GRID_FORMAT_ID_FK and Location_id_fk=? order by length(COL_NAME),col_name"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("parent_id", adNumeric, 1, 0, LocationID)
Cmd.Parameters.Append Cmd.CreateParameter("Location_id_fk", adNumeric, 1, 0, LocationID)
Set RS = Cmd.Execute
NumCols = Cint(RS("col_index"))
ReDim Prefix_arr(NumCols)
RS.MoveFirst
For i=0 to NumCols-1
	Prefix_arr(i)=RS("ColName")
	RS.MoveNext
Next 
Set RS=Nothing
'Finish

'-- Set default selected view of Rack
if Session("viewRackFilter") = "" then
	viewRackFilter = "Icon"
else
	viewRackFilter = Session("viewRackFilter")
end if

'-- Get list of containers that are duplicated in Rack grids
DuplicatesList = ""
DuplicatesList = GetDuplicateRackContainerID(LocationID)

'-- Get data to render Rack
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.DISPLAYRACKGRID(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_LOCATIONID",200, 1, 30, LocationID)
'-- CSBR ID:131045
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Modified to get the length of server name; it was previously hard coded to 30; in case when server name has more characters, its an error
'-- Date: 27/09/2010
Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, len(Application("RegServerName")), Application("RegServerName"))
'-- End of Change #131045#
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Server.CreateObject("ADODB.Recordset")
RS.CursorLocation = aduseClient
RS.LockType = adLockOptimistic
RS.Open Cmd
RS.ActiveConnection = Nothing
'Following Lines are commented to Fix Bugs 78538,78541
'RS.filter = "col_index=1"
'rowName_arr = RS.GetRows(adGetRowsRest, , "RowName")
'numRows = Ubound(rowName_arr,2) + 1 
'RS.filter = 0
'RS.Movefirst
'RS.filter = "ROW_INDEX=1"
'colName_arr = RS.GetRows(adGetRowsRest, , "ColName")
'NumCols = Ubound(colName_arr,2) + 1

if NumCols > 15 then
	cellWidth = 80
else
	cellWidth = 950/NumCols
end if
cellWidthLucidaChars = cellWidth/6

FldArray = split(displayFields,",")
numFields = ubound(FldArray) + 1
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
		RS.filter = "row_index=" & rowName_arr(0,currRow-1) 'To Fix Bugs 78538,78541,78536 
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
					if ucase(Application("RegServerName")) <> "NULL" then
					if arrGridData(0) <> "Rack" then RegNumber = arrGridData(20)
					if len(RegNumber)>10 and NumCols>4 then RegNumber= mid(RegNumber,1,6+10-iif(NumCols>10,10,NumCols))& "..."
					if arrGridData(0) <> "Rack" then BatchNumber = arrGridData(21)					
				end if
				GridBatchField1 = arrGridData(9)
				GridBatchField2 = arrGridData(11)
				GridType = lCase(arrGridData(0))
				Title = "View " & arrGridData(0) & ":" & vblf & "--------------------" & vblf & "Location: " & RS("name") & vblf & arrGridData(0) & " Barcode: " & GridBarcode
				if GridElementName <> "" then Title = Title & vblf & "Name: " & GridElementName
				if ucase(Application("RegServerName")) <> "NULL" then
				if len(arrGridData(20))>0 then Title = Title & vblf & "RegNumber: " & arrGridData(20)
				end if 
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
				elseif FldName = "RegNumber" then
					theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & RegNumber
				elseif FldName = "BatchNumber" then
					theValue = "<img src=""/cheminv/graphics/pixel.gif"" width=""1"" height=""16"" border=""0"">&nbsp;" & BatchNumber
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
Set machine = Server.CreateObject("WScript.Network")
ComputerName = machine.ComputerName
clientName = Cstr(ComputerName)
dim fs,tfile
set fs=Server.CreateObject("Scripting.FileSystemObject")
if fs.FileExists(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/" & ComputerName & ".xml")) then
   fs.DeleteFile(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/" & ComputerName & ".xml"))
end if
set tfile=fs.CreateTextFile(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/" & ComputerName & ".xml"), True)
tfile.WriteLine(xmlHtml)
tfile.close
set tfile=nothing
set fs=nothing
%>
<html>

<head>
    <script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
    <script type="text/javascript" language="javascript" src="/cheminv/choosecss.js"></script>
    <script type="text/javascript" language="javascript" src="/cheminv/gui/refreshGUI.js"></script>

    <script type="text/javascript" language="javascript">
            <!--
    lastSelectedGrid = 0;

    function highlightGrid(GridID) {
        if (lastSelectedGrid > 0) {
            document.getElementById(lastSelectedGrid).className = 'filledGrid';
        }
        changeClass(GridID, 'selectedGrid');
        lastSelectedGrid = GridID;
    }

    function changeClass(ElementName, ClassName) {
        document.getElementById(ElementName).className = ClassName;
    }
    //Adding this function for fixing bug CSBR-75908
    function RefreshLocationBar(LocationPath) {
        if (top.bannerFrame) {
            if (top.bannerFrame.LocationBar.LocationBox) {
                top.bannerFrame.LocationBar.LocationBox.value = LocationPath;
            }
        }
    }
    // -->
    </script>

</head>

<body>

    <form name="form1" method="POST" target="TabFrame">
        <input type="hidden" name="LocationID" value="<%=LocationID%>">
        <input type="hidden" name="removeList">
        <div id="waitGIF" align="center">
            <img src="<%=Application(" ANIMATED_GIF_PATH ")%>" width="130" height="100" border=""></div>
        <div style="display: none">
            <%=xmlHtml%>
        </div>

        <% 
' Added following lines for bug CSBR-75908
if Not IsEmpty(LocationID) then
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GETLOCATIONPATH", adCmdStoredProc)	
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 2000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pLocationID", adnumeric, 1, 0, LocationID)
	Cmd.Execute()
	
	LocationPath = Trim(Cmd.Parameters("RETURN_VALUE"))
	aLocations = split( LocationPath, "\" )
	LocationPath = ""
	for each strLocation in aLocations
	    ' Remove any extraneous location that has a forward slash plus numeric characters in its name.
	    ' Grid locations include a forward slash in the location_name field, followed by the grid
	    ' location number.  We don't want to exclude the possibility that the location name itself
	    ' has a forward slash, as the user may have chosen to use that character.
	    if( len( strLocation ) > 0 ) then
	        i = InStr( strLocation, "/" )	        
	        if( i > 0 ) then
	            if not IsNumeric( Mid( strLocation, i + 1 ) ) then
    	            LocationPath = LocationPath & strLocation & "\\"
    	        end if
    	    else
    	        LocationPath = LocationPath & strLocation & "\\"
	        end if
	    end if
	next
	Response.Write "<script language=javascript>RefreshLocationBar(""" & LocationPath & """);</script>"
	Session("CurrentLocationName") = LocationPath
End if
' 75908 end
        %>


        <script language="vbscript" runat="server">
Function WrapRackContents(FldName, GridID, GridBarcode, GridType, strText, Title, Length, isSelected, hasDups, position)
	Dim str
	if (strText = "") OR IsNull(strText) then strText = ""
	strText2 = "<a href=# class=""MenuLink"">" & strText 
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
				str = str & "<a href=""#"" class=""MenuLink"">" & strText & "</a>"
			else
			    if GridType = "container" then
    				str = str & "<input type=""checkbox"" name=""selectChckBox""  value=""" & GridID & """ onclick=""Removals('" & GridID & "', this.checked);SelectMarked('" & GridType & "');"" />"
			    elseif GridType = "plate" then
    				str = str & "<input type=""checkbox"" name=""selectPlateChckBox""  value=""" & GridID & """ onclick=""Removals('" & GridID & "', this.checked);SelectMarked('" & GridType & "');"" />"
			    end if
			end if
		end if 
		if hasDups = "True" then
			str = str & "<a href=""#"" class=""MenuLink"">" & strText & "</a>"
		elseif GridType = "rack" then
			str = str & "<a href=""#"" class=""MenuLink"" onclick=""SelectLocation(" & GridID & ", '', '')"">" & strText & "</a>"
		elseif GridType = "plate" then
			str = str & "<a href=""View Plate Details"" class=""MenuLink"" target=""TabFrame"" onclick=""SelectPlate(this," & GridID & ", " & LocationID & ",'True'); highlightGrid(" & GridID & "); return false;"">" & strText & "</a>"
		elseif GridType = "container" then
			str = str & "<a href=""View Container Details " & GridBarcode & """  class=""MenuLink"" onclick=""SelectContainer(" & GridID & "); highlightGrid(" & GridID & "); return false; "">" & strText & "</a>"
		end if
	else
		str = str & "<font color=""#BABABA"">" & Title & "</font>"
	end if
	str = str & "</span>"
	WrapRackContents = str
End function
        </script>

        <div id="titleBox" style="position: Absolute; top: 5px; left: 5px; visibility: visible; z-index=1">
            <%
'-- Handle counting and display of elements in grid
if cInt(cntRacksInRack) > 0 then cntRacksInRack = cntRacksInRack/numFields
if cInt(cntPlatesInRack) > 0 then cntPlatesInRack = cntPlatesInRack/numFields
if cInt(cntContainersInRack) > 0 then cntContainersInRack = cntContainersInRack/numFields
if cInt(cntRacksInRack) = 1 then strRacksInRack = cntRacksInRack & " Rack" else strRacksInRack = cntRacksInRack & " Racks" end if
if cInt(cntPlatesInRack) = 1 then strPlatesInRack = cntPlatesInRack & " Plate" else strPlatesInRack = cntPlatesInRack & " Plates" end if
if cInt(cntContainersInRack) = 1 then strContainersInRack = cntContainersInRack & " Container" else strContainersInRack = cntContainersInRack & " Containers" end if
'Response.write(renderBoxHeader("<img src=""/cheminv/images/treeview/rack_open.gif"">",Request("LocationName"),"(" & strRacksInRack & ")&nbsp;(" & strPlatesInRack & ")&nbsp;(" & strContainersInRack & ")","631px"))

            %>
        </div>

        <%
'-- show menu links
    if Session("bMultiSelect") then
        %>
        <div class="dropDownMenuControl">
            <ul id="nav">
                <li><a href="#" title="Select All" onclick="CheckAll(true); return false" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';">Select All</a></li>
                <li><a href="#" title="Clear All" onclick="CheckAll(false); return false" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';">Clear All</a></li>
                <li><a href="/cheminv/gui/<%=Session(" TabFrameURL ")%>?containerCount=0&clear=1&showInList=<%=showInList%>" target="TabFrame" title="Clear All" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';">Cancel MultiSelect</a></li>
            </ul>
        </div>

        <%
    else
        %>
        <div class="dropDownMenuControl">
            <ul id="nav">
                <li><a href="BuildList.asp?view=3&multiSelect=1&LocationID=<%=Locationid%>&LocationName=<%=Request(" LocationName ")%>&showInList=racks" title="Multi-Select" class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';">Multi-Select</a></li>
                <li><a href="Print Rack Summary" title="Print Rack Summary" onclick="OpenDialog('/cheminv/gui/ViewSimpleRackLayout.asp?rackid=<%=LocationID%>&locationid=<%=LocationID%>&containerid=&RackPath=&Summary=(&nbsp;<%=strRacksInRack%>&nbsp;)&nbsp;(&nbsp;<%=strPlatesInRack%>&nbsp;)&nbsp;(&nbsp;<%=strContainersInRack%>&nbsp;)', 'Diag1', 2); return false;"
                    class="contextMenu" onmouseover="this.className = 'menuOn'" onmouseout="this.className = 'contextMenu';">Print Rack</a></li>
            </ul>
        </div>
        <% end if %>

        <div id="contentsCount" style="margin-left: 5px; float: left; position: Absolute; top: 25px; left: 5px; visibility: visible; z-index=1;">
            <%
'-- Handle counting and display of elements in grid
numRacks = cInt(cntRacksInRack)
numContainers = cInt(cntContainersInRack)
numPlates = cInt(cntPlatesInRack)
'if numRacks > 0 then numRacks = numRacks/6
'if numPlates > 0 then numPlates = numPlates/6
'if numContainers > 0 then numContainers = numContainers/6
totalInRack = numRacks + numContainers + numPlates
if numRacks = 0 then
    strRacksInRack = ""
elseif numRacks = 1 then 
    strRacksInRack = cntRacksInRack & " Rack" 
else 
    strRacksInRack = cntRacksInRack & " Racks" 
end if
if numPlates = 0 then 
    strPlatesInRack = ""
elseif numPlates = 1 then 
    strPlatesInRack = cntPlatesInRack & " Plate" 
else 
    strPlatesInRack = cntPlatesInRack & " Plates" 
end if
if numContainers = 0 then 
    strContainersInRack = ""
elseif numContainers = 1 then 
    strContainersInRack = cntContainersInRack & " Container" 
else 
    strContainersInRack = cntContainersInRack & " Containers" 
end if

response.Write "<span class=""listCaption""><strong>" & Request("LocationName") & "&nbsp;</strong>" 
if totalInRack > 0 then response.Write "("
response.Write strRacksInRack
if numRacks > 0 and numPlates > 0 then response.Write "&nbsp;"
response.write strPlatesInRack 
if (numRacks > 0 or numPlates > 0) and numContainers > 0 then response.Write "&nbsp;"
response.write strContainersInRack
if totalInRack > 0 then response.Write ")"'
response.Write "</span>"
            %>
        </div>

        <div id="sorttext" style="margin-left: 350px; float: left; position: Absolute; top: 25px; left: 5px; visibility: visible; z-index=1; font-size: 11px;">
            Rack displayed by <strong>Icon</strong>. To change the display, click on the arrow.
        </div>

        <div id="rackViewer" style="position: Absolute; top: 44px; left: 5px; visibility: visible; z-index=1">
            <table style="font-size: 7pt; font-family: verdana; table-layout: fixed; border-collapse: collapse;" cellspacing="0" cellpadding="0" bordercolor="#999999" id="tbl" datasrc datafld="icon" border="1">
                <col width="30">
                <%
		For i=0 to NumCols-1
			Response.Write "<col width=""" & cellWidth & """>"
		Next
                %>
                <thead>
                    <th align="center">
                        <a href="#" onclick="document.all.hiddenSelector.style.visibility = 'visible';" title="Click to select displayed value">
                            <img src="/cheminv/graphics/desc_arrow.gif" border="0" width="12" height="6"></a>
                        <a id="hiddenRackSelector" target="rackJSFrame"></a>
                        <div id="hiddenSelector" style="position: Absolute; top: 0; left: 0; visibility: hidden; z-index=2">
                            <select id="cboField" size="7" onchange="loadXMLDoc();">
                                <option value></option>
                                <option value="Icon">Icon</option>
                                <option value="Barcode">Barcode</option>
                                <option value="Cellname">Cell Name</option>
                                <option value="RegNumber">Reg Number</option>
                                <option value="BatchNumber">Batch Number</option>
                            </select>
                        </div>
                    </th>
                    <%
		For i=0 to NumCols-1
			Response.Write "<th>"
			if not cellSeqNaming then
    		    Response.write(Prefix_arr(i))    		    
			end if
			Response.Write "</th>" & vblf
		Next
                    %>
                </thead>

                <%
		Set objXMLDoc = Server.CreateObject("Microsoft.XMLDOM") 
		objXMLDoc.async = False 
		objXMLDoc.Loadxml(xmlHtml)
		Set Root = objXMLDoc.documentElement		
		Set NodeList = Root.getElementsByTagName(viewRackFilter)
			Response.Write "<tbody>"	
		For Each xmlItem In NodeList	
			Response.Write "<tr height=""20""><th><span></span></th>"
				For i = 1 to NumCols step 1
					name = xmlItem.childNodes(i).text 
					Response.Write "<td align=""center"" valign=""center""><dIV class=""col" & i & """>" & name & "</div></td>" &vblf			
		Next
			    Next
		Set objXMLDoc = Nothing
                %>
                                            </tr>
                                            </tbody>
            </table>
            <% if displayErrorMSG then %>
            <span style="font-size: 10px; display: block; margin-top: 3px;">
                <div style="display: block; height: 15px; width: 15px; float: left; background: #CC0000;">&nbsp;</div>
                <%=replace(errorMSG,"<br /><br />","<br />")%>
            </span>
            <% end if %>
        </div>

        <script language="javascript">
            if (parent.TabFrame) parent.TabFrame.location.href = '/cheminv/cheminv/SelectContainerMsg.asp?entity=rack location';

            function SelectPlate(elm, PlateID, locationID, Refresh) {
                if (parent.TabFrame) parent.TabFrame.location.href = "/cheminv/gui/ViewPlate.asp?PlateID=" + PlateID + "&refresh=" + Refresh;
            }

            // Hide the wait gif
            document.all.waitGIF.style.display = "none";

            // Open the selected rack if there is one
            SelectRack = "<%=Session("SelectRack")%>";

            if (SelectRack != "0") {
                var hiddenRackSelector = document.getElementById("hiddenRackSelector");
                hiddenRackSelector.click();
            }
            document.all.cboField.options[1].selected = true;
        </script>

        <script language="javascript">
            var path = "/ChemInv/config/xml_templates/";
            var client = "<%=clientname%>";
            var columns = "<%=NumCols%>";
            var xmlfile = client + ".xml";
            var fullpath = path + xmlfile;
            var table = document.getElementById("tbl");
            function loadXMLDoc() {
                var selectedRack = document.getElementById("cboField").value;
                var dispText = "Rack displayed by <strong>";
                dispText = dispText + selectedRack + "</strong>. To change the display, click on the arrow.";
                document.getElementById("sorttext").innerHTML = dispText;
                document.all.hiddenSelector.style.visibility = 'hidden';
                changeContent(document.all.sorttext, dispText);                     
                if (window.XMLHttpRequest) {
                    xhttp = new XMLHttpRequest();
                } else {                    
                    xhttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                xhttp.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                        BindTable(xhttp.responseXML);
                    }
                };
                xhttp.open("GET", fullpath, true);
                xhttp.send();
            }
            function BindTable(xml) {
                html = "";    
                wellFilter = selectedRack;
                var RootNode = "";
                RootNode = xml.getElementsByTagName(selectedRack);                
                for (var k = RootNode.length; k > 0; k--) {
                    table.deleteRow(k);
                }                
                for (i = 0; i <= RootNode.length - 1; i++) {
                    rowname = RootNode[i].getElementsByTagName("rowname")[0].childNodes[0].nodeValue;
                    html = "<tbody><tr height='20'><th><span>" + rowname + "</span></th>";
                    for (j = 1; j <= columns; j++) {
                        col = "col" + j;
                        name = RootNode[i].getElementsByTagName(col)[0].childNodes[0].nodeValue;
                        html = html + "<td align='center' valign='center'><dIV class='col'" + j + ">" + name + "</div></td>";
                    }
                    html = html + "</tr></tbody>";                    
                    var row = table.insertRow(i + 1);
                    row.innerHTML = html;
                }
            }
        </script>

        <% 
'-- select the container/plate
if SelectContainer <> "" and SelectContainer <> "0" then 
	Response.Write "<script language=""javascript"" for=""window"" event=""onload"">"
	if ContainerCount = 1 OR CLng(SelectContainer)>0 then
		if CLng(SelectContainer)>0 then ContainerID = SelectContainer
		theLink = "document.getElementById(""" & SelectContainer & """).firstChild.click();"
		response.Write theLink
		'response.Write "alert(" & theLink & ");"
    end if
  Response.Write "</script>"
end if 
        %>

        <script language="javascript" type="text/javascript">
            TabFrameURL = "<%=Session("TabFrameURL")%>";
            if (TabFrameURL != "") {
                //refresh tab frame
                document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
                document.form1.submit();
            }

            function SelectMarked(type) {

                // Restrict batch Rack grid options to Containerse for now
                //if (type == 'container') {
                var reml = document.form1.removeList;
                var len = reml.value.length
                if (reml.value.substring(len - 1, len) == ",") {
                    reml.value = reml.value.substring(0, len - 1);
                }
                if (type == 'container') {
                    document.form1.action = "/Cheminv/GUI/multiselect.asp?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
                    elm = document.form1.selectChckBox;
                } else if (type == 'plate') {
                    document.form1.action = "/Cheminv/GUI/multiselect_plate.asp?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
                    elm = document.form1.selectPlateChckBox;
                }
                //document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>&showInList=<%=showInList%>&dicttype="+type;
                //elm = document.form1.selectChckBox;
                <%if bDisableChkBoxes then%>
                //MCD: added check for case where there's only one element
                if (elm.length) {
                    for (i = 0; i < elm.length; i++) {
                        elm[i].disabled = false;
                    }
                } else {
                    elm.disabled = false;
            }
            <%end if%>
            document.form1.submit();
            <%if bDisableChkBoxes then%>
            //MCD: added check for case where there's only one element
            if (elm.length) {
                for (i = 0; i < elm.length; i++) {
                    elm[i].disabled = true;
                }
            } else {
                                            elm.disabled = true;
            }
                                        <%end if%>
                                        reml.value = "";
            //} 
            }

            function CheckAll(bCheck) {
                var cbObj = document.form1.selectChckBox;
                var cbPlateObj = document.form1.selectPlateChckBox;
                if (cbObj != undefined) {
                    if (cbObj.length) {
                        for (i = 0; i < cbObj.length; i++) {
                            if (cbObj[i].checked ^ bCheck) {
                                cbObj[i].checked = bCheck;
                                if (!bCheck) Removals(cbObj[i].value, false);
                            }
                        }
                    } else {
                        if (cbObj.checked ^ bCheck) {
                            cbObj.checked = bCheck;
                            if (!bCheck) Removals(cbObj.value, false);
                        }
                    }
                    SelectMarked('container');
                }
                if (cbPlateObj != undefined) {
                    if (cbPlateObj.length) {
                        for (i = 0; i < cbPlateObj.length; i++) {
                            if (cbPlateObj[i].checked ^ bCheck) {
                                cbPlateObj[i].checked = bCheck;
                                if (!bCheck) Removals(cbPlateObj[i].value, false);
                            }
                        }
                    } else {
                        if (cbPlateObj.checked ^ bCheck) {
                            cbPlateObj.checked = bCheck;
                            if (!bCheck) Removals(cbPlateObj.value, false);
                        }
                    }
                    SelectMarked('plate');
                }
            }

            function Removals(id, bRemove) {
                var idc = id + ",";
                var reml = document.form1.removeList;

                if (bRemove) {
                    if (reml.value.indexOf(idc) >= 0) {
                        reml.value = reml.value.replace(idc, "");
                    }
                } else {
                    if (reml.value.indexOf(idc) == -1) {
                        reml.value += id + ",";
                    }
                }
                //alert(reml.value);				
            }
        </script>
</body>

</html>
