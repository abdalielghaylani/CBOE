<%
numLinks = 17
Dim arrLinks()
redim arrLinks(numLinks,5)
'-- 1st dimension is on/off
'-- 2nd dimension is "on" link href
'-- 3rd dimension is link text
'-- 4th dimension is dialog size
'-- 5th dimension is show "off" link switch
'-- 6th dimension is link category
arrCategories = array("Update","Create","Check In/Out","Request","Other")


'-- set defaults
for i = 0 to ubound(arrLinks)
	'-- all links are off by default
	arrLinks(i,0) = 0
	'-- all dialog sizes are 1 by default
	arrLinks(i,3) = 1
next

'-- build the list of links
arrLinks(0,1) = "/cheminv/gui/ChangeQty.asp?ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&UOMAbv=" & UOMAbv & "&TotalQtyReserved=" & TotalQtyReserved
arrLinks(0,2) = "Change Qty"
arrLinks(0,3) = "1"
arrLinks(0,5) = arrCategories(0)

arrLinks(1,1) = "/ChemInv/GUI/UpdateContainer.asp?editMode=single&action=dataEntry&containerFields=Container Status:container_status_id_fk&iContainerID=" & ContainerID
arrLinks(1,2) = "Change Status"
arrLinks(1,3) = "1"
arrLinks(1,5) = arrCategories(0)

arrLinks(2,1) = "/Cheminv/GUI/CheckOut.asp?action=out&ContainerID=" & ContainerID & "&LocationID=" &  LocationID & "&LocationName=" & Replace(LocationName,"'","\'")
arrLinks(2,2) = "Check Out"
arrLinks(2,3) = "1"
arrLinks(2,5) = arrCategories(2)

arrLinks(3,1) = "/Cheminv/GUI/CheckOut.asp?action=in&ContainerID=" & ContainerID & "&LocationID=" &  LocationID
arrLinks(3,2) = "Check In"
arrLinks(3,3) = "1"
arrLinks(3,5) = arrCategories(2)

arrLinks(4,1) = "/Cheminv/GUI/Request.asp?action=create&ContainerID=" & ContainerID & "&LocationID=" & LocationID & "&UOMAbv=" & UOMAbv & "&QtyRequired=" & QtyAvailable
arrLinks(4,2) = "Request Container"
arrLinks(4,3) = "1"
arrLinks(4,5) = arrCategories(3)

arrLinks(5,1) = "/Cheminv/GUI/RequestSample.asp?action=create&ContainerID=" & ContainerID & "&LocationID=" & LocationID & "&UOMAbv=" & UOMAbv & "&QtyRequired=" & QtyAvailable
arrLinks(5,2) = "Request Sample"
arrLinks(5,3) = "2"
arrLinks(5,4) = 1
arrLinks(5,5) = arrCategories(3)

arrLinks(6,1) = "/Cheminv/GUI/CreateOrEditContainer.asp?isCopy=true&getData=db&amp;ContainerID=" & ContainerID
arrLinks(6,2) = "Copy Container"
arrLinks(6,3) = "2"
arrLinks(6,5) = arrCategories(1)
	
arrLinks(7,1) = "/Cheminv/GUI/Certify.asp?ContainerID=" & ContainerID
arrLinks(7,2) = "Certify Container"
arrLinks(7,3) = "1"
arrLinks(7,4) = 1
arrLinks(7,5) = arrCategories(4)

arrLinks(8,1) = "/Cheminv/GUI/AllotContainer.asp?action=split&amp;ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&UOMAbv=" & UOMAbv
arrLinks(8,2) = "Split Container"
arrLinks(8,3) = "1"
arrLinks(8,5) = arrCategories(1)

arrLinks(9,1) = "/Cheminv/GUI/AllotContainer.asp?action=sample&amp;ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining & "&UOMAbv=" & UOMAbv
arrLinks(9,2) = "Create Samples"
arrLinks(9,3) = "1"
arrLinks(9,5) = arrCategories(1)

arrLinks(10,1) = "/Cheminv/GUI/MergeContainers.asp?ContainerID=" & ContainerID & "&ParentContainerID=" & ParentContainerID & "&Action=SelectIDs"
arrLinks(10,2) = "Merge Containers"
arrLinks(10,3) = "1"
arrLinks(10,5) = arrCategories(1)

arrLinks(11,1) = "/ChemInv/GUI/ReorderContainer.asp?getData=db&amp;ContainerID=" & ContainerID & "&SupplierID=" & SupplierID & "&CatNum=" & SupplierCatNum
arrLinks(11,2) = "Reorder Container"
arrLinks(11,3) = "2"
arrLinks(11,5) = arrCategories(1)

arrLinks(12,1) = "/ChemInv/GUI/CreateOrEditContainer.asp?isEdit=true&getData=db&amp;ContainerID=" & ContainerID
arrLinks(12,2) = "Edit Container"
arrLinks(12,3) = "2"
arrLinks(12,5) = arrCategories(0)

arrLinks(13,1) = "/Cheminv/GUI/MoveContainer.asp?ContainerID=" & ContainerID
arrLinks(13,2) = "Move Container"
arrLinks(13,3) = "1"
arrLinks(13,5) = arrCategories(0)

arrLinks(14,1) = "/Cheminv/GUI/RetireContainer.asp?ContainerID=" & ContainerID & "&QtyRemaining=" & QtyRemaining
arrLinks(14,2) = "Retire Container"
arrLinks(14,3) = "1"
arrLinks(14,5) = arrCategories(0)

arrLinks(15,1) = "/Cheminv/GUI/DeleteContainer.asp?ContainerID=" & ContainerID
arrLinks(15,2) = "Delete Container"
arrLinks(15,3) = "1"
arrLinks(15,5) = arrCategories(0)

arrLinks(16,1) = "/Cheminv/GUI/history.asp?containerid=" & ContainerID
arrLinks(16,2) = "History"
arrLinks(16,3) = "2"
arrLinks(16,5) = arrCategories(4)

arrLinks(17,1) = "/Cheminv/GUI/PrintLabel.asp?ContainerID=" & ContainerID
arrLinks(17,2) = "Print Label"
arrLinks(17,3) = "1"
arrLinks(17,5) = arrCategories(4)

'-- turn links on if appropriate
If Session("INV_CHANGEQTY_CONTAINER" & dbkey) then
	arrLinks(0,0) = 1
end if

If Session("INV_CHANGE_STATUS_CONTAINER" & dbkey) then
	arrLinks(1,0) = 1
end if

If Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	arrLinks(2,0) = 1
end if

If Session("INV_CHECKIN_CONTAINER" & dbkey) then
	arrLinks(3,0) = 1
end if

If Application("AllowRequests") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) AND IsNull(RequestID) AND ContainerStatusID = "1" then
	arrLinks(4,0) = 1
end if

If Application("ShowRequestSample") then
	if Session("INV_CHECKOUT_CONTAINER" & dbkey) then
		if cint(ContainerStatusID) = cint(Application("StatusApproved")) then
			arrLinks(5,0) = 1
		end if
	end if
else
	arrLinks(5,4) = 0
end if

If Session("INV_CREATE_CONTAINER" & dbkey) then
	arrLinks(6,0) = 1
End if

if Application("ShowCertify") then
	If Session("INV_CERTIFY_CONTAINER" & dbkey) then
		if IsEmpty(Session("DateCertified")) then
			arrLinks(7,0) = 1
		end if
	end if
else
	arrLinks(7,4) = 0
End if

If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSplit") then
	arrLinks(8,0) = 1
End if

If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSample") then
	arrLinks(9,0) = 1
End if

If Session("INV_CREATE_CONTAINER" & dbkey) or Application("ShowMerge") then
	if not isNull(ParentContainerID) and ParentContainerID <> "" then
		arrLinks(10,0) = 1
	end if
End if

If Session("INV_REORDER_CONTAINER" & dbkey) then			
	'Only show the "Reorder" link if this is not a registry compound, and if the location is not "Trash Can" (LocTypeID 502)
	If (Len(RegID) = 0) And (LocationType <> "502") Then
		arrLinks(11,0) = 1
	End If
End if

If Session("INV_EDIT_CONTAINER" & dbkey) then
	arrLinks(12,0) = 1
End if

If Session("INV_MOVE_CONTAINER" & dbkey) then
	arrLinks(13,0) = 1
End if

If Session("INV_RETIRE_CONTAINER" & dbkey) then
	arrLinks(14,0) = 1
End if

If Session("INV_DELETE_CONTAINER" & dbkey) then
	arrLinks(15,0) = 1
End if

If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then
	arrLinks(16,0) = 1
end if

If Session("INV_PRINT_LABEL_CONTAINER" & dbkey) then
	arrLinks(17,0) = 1
end if

if Application("ClassicLinks") = "TRUE" then
	ShowClassicLinks(arrLinks)
else
	ShowMenuLinks(arrLinks)
end if

sub ShowClassicLinks(byref arrLinks)
	Response.Write "<table border=""0"" cellspacing=""0"" cellpadding=""2"" width=""100%""><tr><td align=""right"" valign=""top"" nowrap>"
	isFirst = true
	for i = 0 to ubound(arrLinks)
		if arrLinks(i,0) = 1 then
			if not isFirst then Response.Write " | " 
			Response.Write "<a class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('" & arrLinks(i,1) & "', 'Diag', " & arrLinks(i,3) & "); return false"">" & arrLinks(i,2) & "</a>"
			isFirst = false
		elseif arrLinks(i,4) = 1 then
			if not isFirst then Response.Write " | " 
			Response.Write "<a class=""MenuLink"" disabled>" & arrLinks(i,2) & "</a>"
			isFirst = false
		end if
	next
	Response.Write "</td></tr></table>"
	
end sub

sub ShowMenuLinks(byref arrLinks)

'		/*background-color:D4D0C8; /* EBEBEB */*/

%>
<style>
	.WindowMenu {color:#000000; font-size:8pt; font-family: verdana}
	A.WindowMenu:LINK {text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
	A.WindowMenu:VISITED {text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
	A.WindowMenu:HOVER {text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: verdana}

	#nav, #nav ul { /* all lists */
		font-size:8pt;
		font-family: verdana;
		margin: 0;
		list-style: none;
		line-height: 1;
		border-style: solid;
		border-width: 1px;
		border-left-color: #ffffff;
		border-top-color:#D4D0C8;
		border-bottom-color:#D4D0C8;
		border-right-color:#D4D0C8;
		width:800px;
		height:20px;
	}
	#nav a {
		display: block;
		width:100px;
		color:006699;
	}
	#nav li { /* all list items */
		float: left;
		width: 40px; /* width needed or else Opera goes nuts */
		border-width:1px 0px 1px 1px;
		border-top-color:#999;
		border-bottom-color:#999;
		border-right-color:#999;
		line-height:20px;
		padding: 0px 0px 0px 10px;
	}
	#nav li:hover, #nav li.sfhover{background-color:#ffffff;border-color:#fff;}
	#nav li ul { /* second-level lists */
		position: absolute;
		width:100px;
		height:10px;
		position:absolute;
		left:-900px;
		margin:0px 0px 0px 0px;
		border: 1px solid #999;
	}
	#nav li li{
		line-height:20px;
		border:0px;
		border-style:solid;
		border-right-color:#333;
		margin:0px 0px 0px 0px;
		}
	#nav li ul a{width:100px; padding: 0px 0px 0px 0px;}
	#nav li:hover ul, #nav li.sfhover ul { /* lists nested under hovered list items */
		left: auto;
		background-color:D4D0C8;

	}
</style>

<STYLE>
#contextMenu {
	font-size:8pt;
	font-family: verdana;
	position: absolute;
	visibility: hidden;
	width: 350px;
	background-color: D4D0C8;
	border: 1px outset #999;
	padding: 5px 5px 5px 5px;
	line-height: 15pt
}

.A:contextMenu {
	color: 4682b4;
	text-decoration: none;
	cursor: default;
	width: 100%
  }

 .A:menuOn {
   color: white;
   text-decoration: none;
   background-color: darkblue;
   cursor: default;
   width: 100%
  }
</STYLE>

<SCRIPT>
var menu;
function showMenu (evt) {
  if (document.all) {

	if (parseInt(navigator.appVersion) > 3) {
	 if (navigator.appName=="Netscape") {
	  winW = window.innerWidth;
	  winH = window.innerHeight;
	 }
	 if (navigator.appName.indexOf("Microsoft")!=-1) {
	  winW = document.body.offsetWidth;
	  winH = document.body.offsetHeight;
	 }
	}

	varAdjHeight = 0;
	varAdjWidth = 0;
	if (event.clientY > (winH-195)) {
		varAdjHeight = event.clientY-195;
	} else {
		varAdjHeight = event.clientY;
	}
	if (event.clientX > (winW-346)) {
		varAdjWidth = event.clientX-346;
	} else {
		varAdjWidth = event.clientX;
	}
    document.all.contextMenu.style.pixelLeft = varAdjWidth;
    document.all.contextMenu.style.pixelTop = varAdjHeight;
    //document.all.contextMenu.style.pixelTop = event.clientX;
    //document.all.contextMenu.style.pixelTop = event.clientY;
    document.all.contextMenu.style.visibility = 'visible';
    return false;
  }
  else if (document.layers) {
    if (evt.which == 3) {
      document.contextMenu.left = evt.x;
      document.contextMenu.top = evt.y;
      document.contextMenu.onmouseout =
        function (evt) { this.visibility = 'hide'; };
      document.contextMenu.visibility = 'show';
      return false;
    }
  }
  return true;
}
if (document.all)
  document.oncontextmenu =showMenu;
if (document.layers) {
  document.captureEvents(Event.MOUSEDOWN);
  document.onmousedown = showMenu;
}
</SCRIPT>


<script type="text/javascript"><!--//--><![CDATA[//><!--

sfHover = function() {
	var sfEls = document.getElementById("nav").getElementsByTagName("LI");
	for (var i=0; i<sfEls.length; i++) {
		sfEls[i].onmouseover=function() {
			this.className+=" sfhover";
		}
		sfEls[i].onmouseout=function() {
			this.className=this.className.replace(new RegExp(" sfhover\\b"), "");
		}
	}
}
if (window.attachEvent) window.attachEvent("onload", sfHover);

//--><!]]></script>


<%WritePullDownMenu arrLinks, arrCategories%>
<%WriteContextMenu arrLinks, arrCategories%>

<%
end sub
for i=0 to ubound(arrLinks)
	'Response.Write arrLinks(i,0) & ":" & arrLinks(i,1) & ":" & arrLinks(i,2) & ":" & arrLinks(i,3) & ":" & arrLinks(i,4) & ":" & arrLinks(i,5) & "<BR>"
next

sub WriteContextMenu(byref arrLinks, byref arrCategories)
	
	Response.Write "<DIV ID=""contextMenu"" ONMOUSEOUT=""menu = this; this.tid = setTimeout ('menu.style.visibility = \'hidden\'', 20);"" ONMOUSEOVER=""clearTimeout(this.tid);"">"
	for i = 0 to ubound(arrCategories)	
		Response.Write "<strong>" & arrCategories(i) & "</strong><br />"
		WriteContextMenuLinks arrLinks, arrCategories(i)
		Response.Write "<br />"
	next
	Response.Write "</div>" & chr(13)

end sub

sub WriteContextMenuLinks(byref arrLinks, category)
	
	isFirst = true
	for j = 0 to ubound(arrLinks)
		if arrLinks(j,5) = category then
			if arrLinks(j,0) = 1 then
				if not isFirst then Response.Write "&nbsp;|&nbsp;" 
				Response.Write "<A HREF=""#" & arrLinks(j,2) & """ onclick=""OpenDialog('" & arrLinks(j,1) & "', 'Diag', " & arrLinks(j,3) & "); return false"" CLASS=""contextMenu"" ONMOUSEOVER=""this.className = 'menuOn'"" ONMOUSEOUT=""this.className = 'contextMenu';"" >" & arrLinks(j,2) & "</A>"
				isFirst = false					
			elseif arrLinks(j,4) = 1 then
				if not isFirst then Response.Write "&nbsp;|&nbsp;" 
				Response.Write "<A HREF=""#"" CLASS=""contextMenu"" style=""color:#808080;cursor: default;"">" & arrLinks(j,2) & "</A>"
				isFirst = false					
			end if
		end if
	next

end sub

sub WritePullDownMenu(byref arrLinks, byref arrCategories)

	Response.Write "<div style=""background-color:#D4D0C8;border:1px solid #999;""><ul id=""nav"">"
	for	i=0 to ubound(arrCategories)
		if i = 0 then
			Response.Write "<li style=""margin-left:100px;""><a href=""#"">" & arrCategories(i) & "</a><ul>"
		else
			Response.Write "<li><a href=""#"">" & arrCategories(i) & "</a><ul>"
		end if
		WritePullDownMenuLinks arrLinks, arrCategories(i)
		Response.Write "</ul></li>"
	next
	Response.Write "</ul></div>"
	
end sub

sub WritePullDownMenuLinks(byref arrLinks, category)

	for j = 0 to ubound(arrLinks)
		if arrLinks(j,5) = category then
			if arrLinks(j,0) = 1 then
				Response.Write "<li><A HREF=""#"" onclick=""OpenDialog('" & arrLinks(j,1) & "', 'Diag', " & arrLinks(j,3) & "); return false"" CLASS=""contextMenu"" ONMOUSEOVER=""this.className = 'menuOn'"" ONMOUSEOUT=""this.className = 'contextMenu';"" >" & arrLinks(j,2) & "</A></li>"
			elseif arrLinks(j,4) = 1 then
				Response.Write "<li><A HREF=""#"" CLASS=""contextMenu"" style=""color:#808080;cursor: default;"">" & arrLinks(j,2) & "</A></li>"
			end if
		end if
	next

end sub



isFirst = true
if false then
%>

<%If Session("INV_CHANGEQTY_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/cheminv/gui/ChangeQty.asp?ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>&TotalQtyReserved=<%=TotalQtyReserved%>', 'Diag', 1); return false">Change Qty</a>
<%end if%>

<%If Session("INV_CHANGE_STATUS_CONTAINER" & dbkey) then
	call CheckIfFirst()%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/ChemInv/GUI/UpdateContainer.asp?editMode=single&action=dataEntry&containerFields=Container Status:container_status_id_fk&iContainerID=<%=ContainerID%>', 'Diag', 1); return false">Change Status</a>
<%end if%>

<%If Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?action=out&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&LocationName=<%=Replace(LocationName,"'","\'")%>' , 'Diag', 1); return false">Check Out</a>
<%end if%>

<%If Session("INV_CHECKIN_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CheckOut.asp?action=in&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID, 'Diag', 1); return false">Check In</a>
<%end if%>

<%If Application("AllowRequests") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) AND IsNull(RequestID) AND ContainerStatusID = "1" then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=create&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&UOMAbv=<%=UOMAbv%>&QtyRequired=<%=QtyAvailable%>', 'Diag', 1); return false">Request Container</a>

<%end if%>

<%If Application("ShowRequestSample") AND Session("INV_CHECKOUT_CONTAINER" & dbkey) then
	call CheckIfFirst()
	if cint(ContainerStatusID) = cint(Application("StatusApproved")) then
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RequestSample.asp?action=create&ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&LocationID=' + LocationID + '&UOMAbv=<%=UOMAbv%>&QtyRequired=<%=QtyAvailable%>', 'Diag', 2); return false">Request Sample</a>
	<%else%>
	<a class="MenuLink" disabled>Request Sample</a>
	<%end if%>
<%end if%>


<%If Session("INV_CREATE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/CreateOrEditContainer.asp?isCopy=true&getData=db&amp;ContainerID=<%=ContainerID%>', 'Diag', 2); return false">Copy Container</a>
<%End if%>

<%If Session("INV_CERTIFY_CONTAINER" & dbkey) and Application("ShowCertify") then
	call CheckIfFirst()
	if not IsEmpty(Session("DateCertified")) then
%>			
		<a class="MenuLink" disabled>Certify Container</a>
	<%else%>	
		<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/Certify.asp?ContainerID=<%=ContainerID%>', 'Diag', 1); return false">Certify Container</a>
	<%end if%>
<%End if%>

<%If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSplit") then
	call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/AllotContainer.asp?action=split&amp;ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>', 'Diag', 1); return false">Split Container</a>
<%End if%>

<%If Session("INV_CREATE_CONTAINER" & dbkey) and Application("ShowSample") then
	call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/AllotContainer.asp?action=sample&amp;ContainerID=<%=ContainerID%>&QtyRemaining=<%=QtyRemaining%>&UOMAbv=<%=UOMAbv%>', 'Diag', 1); return false">Create Samples</a>
<%End if%>

<%If Session("INV_CREATE_CONTAINER" & dbkey) or Application("ShowMerge") then
	if not isNull(ParentContainerID) and ParentContainerID <> "" then
		call CheckIfFirst()
%>			
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MergeContainers.asp?ContainerID=<%=ContainerID%>&ParentContainerID=<%=ParentContainerID%>&Action=SelectIDs', 'Diag', 1); return false">Merge Containers</a>
<%
	end if
End if
%>

<%
If Session("INV_REORDER_CONTAINER" & dbkey) then			
	'Only show the "Reorder" link if this is not a registry compound, and if the location is not "Trash Can" (LocTypeID 502)
	If (Len(RegID) = 0) And (LocationType <> "502") Then
		call CheckIfFirst()
%>
	<a class="MenuLink" HREF="Reorder Container" onclick="OpenDialog('/ChemInv/GUI/ReorderContainer.asp?getData=db&amp;ContainerID=<%=ContainerID%>&SupplierID=<%=SupplierID%>&CatNum=<%=SupplierCatNum%>', 'Diag', 2); return false">Reorder Container</a>
<%
	End If
End if
%>

<%If Session("INV_EDIT_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>			
	<a class="MenuLink" id="EditContainerLnk" HREF="Edit inventory container" onclick="OpenDialog('../GUI/CreateOrEditContainer.asp?isEdit=true&getData=db&amp;ContainerID=<%=ContainerID%>', 'Diag', 2); return false">Edit Container</a>
<%End if%>

<%If Session("INV_MOVE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/MoveContainer.asp?ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName, 'Diag', 1); return false">Move Container</a>
<%End if%>

<%If Session("INV_RETIRE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/RetireContainer.asp?ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName + '&QtyRemaining=<%=QtyRemaining%>', 'Diag', 1); return false">Retire Container</a>
<%End if%>

<%If Session("INV_DELETE_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>	
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/DeleteContainer.asp?ContainerID=<%=ContainerID%>&ContainerName=' + ContainerName, 'Diag', 1); return false">Delete Container</a>		
<%End if%>

<%If Session("INV_VIEW_AUDIT_TRAIL" & dbkey) then
	call CheckIfFirst()
%>		
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/history.asp?containerid=<%=ContainerID%>', 'Diag', 2); return false">History</a>		
<%end if%>

<%If Session("INV_PRINT_LABEL_CONTAINER" & dbkey) then
	call CheckIfFirst()
%>
	<a class="MenuLink" HREF="#" onclick="OpenDialog('/Cheminv/GUI/PrintLabel.asp?ContainerID=<%=ContainerID%>', 'Diag', 1); return false">Print Label</a>		
<%end if%>

<%
end if
sub CheckIfFirst()
	if isFirst then
		isFirst = false
	else
		Response.Write " | "
	end if
end sub
%>