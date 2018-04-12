<%
numLinks = 8
Dim arrLinks()
redim arrLinks(numLinks,6)
'-- 1st dimension is on/off
'-- 2nd dimension is "on" link href
'-- 3rd dimension is link text
'-- 4th dimension is dialog size
'-- 5th dimension is show "off" link switch
'-- 6th dimension is link category
'-- 7th dimension is link title
arrCategories = array("Location","Rack")


'-- set defaults
for i = 0 to ubound(arrLinks)
	'-- all links are off by default
	arrLinks(i,0) = 0
	'-- all dialog sizes are 1 by default
	arrLinks(i,3) = 1
next

'-- build the list of links
arrLinks(0,1) = "/cheminv/gui/NewLocation.asp"
arrLinks(0,2) = "New"
arrLinks(0,3) = "1"
arrLinks(0,5) = arrCategories(0)
arrLinks(0,6) = "Create New Location"

arrLinks(1,1) = "/cheminv/gui/NewLocation.asp?GetData=db"
arrLinks(1,2) = "Edit"
arrLinks(1,3) = "1"
arrLinks(1,5) = arrCategories(0)
arrLinks(1,6) = "Edit Selected Location"

arrLinks(2,1) = "/cheminv/gui/MoveLocation.asp"
arrLinks(2,2) = "Move"
arrLinks(2,3) = "1"
arrLinks(2,5) = arrCategories(0)
arrLinks(2,6) = "Move Selected Location"

arrLinks(3,1) = "/cheminv/gui/DeleteLocation.asp"
arrLinks(3,2) = "Delete"
arrLinks(3,3) = "1"
arrLinks(3,5) = arrCategories(0)
arrLinks(3,6) = "Delete Selected Location"

arrLinks(4,1) = "/cheminv/gui/NewLocation.asp?LocationType=rack"
arrLinks(4,2) = "New"
arrLinks(4,3) = "1"
arrLinks(4,5) = arrCategories(1)
arrLinks(4,6) = "Create New Rack"

arrLinks(5,1) = "/cheminv/gui/NewLocation.asp?LocationType=rack&amp;GetData=db"
arrLinks(5,2) = "Edit"
arrLinks(5,3) = "1"
arrLinks(5,5) = arrCategories(1)
arrLinks(5,6) = "Edit Selected Rack"

arrLinks(6,1) = "/cheminv/gui/MoveLocation.asp?LocationType=rack"
arrLinks(6,2) = "Move"
arrLinks(6,3) = "1"
arrLinks(6,5) = arrCategories(1)
arrLinks(6,6) = "Move Selected Rack"
	
arrLinks(7,1) = "/cheminv/gui/DeleteLocation.asp?LocationType=rack"
arrLinks(7,2) = "Delete"
arrLinks(7,3) = "1"
arrLinks(7,5) = arrCategories(1)
arrLinks(7,6) = "Delete Selected Rack"


'-- turn links on if appropriate
If Session("INV_CREATE_LOCATION" & dbkey) then
	arrLinks(0,0) = 1
end if

If Session("INV_EDIT_LOCATION" & dbkey) then
	arrLinks(1,0) = 1
end if

If Session("INV_MOVE_LOCATION" & dbkey) then
	arrLinks(2,0) = 1
end if

If Session("INV_DELETE_LOCATION" & dbkey) then
	arrLinks(3,0) = 1
end if

If Application("RACKS_ENABLED") then
	If Session("INV_CREATE_LOCATION" & dbkey) then
		arrLinks(4,0) = 1
	end if

	If Session("INV_EDIT_LOCATION" & dbkey) then
		arrLinks(5,0) = 1
	end if

	If Session("INV_MOVE_LOCATION" & dbkey) then
		arrLinks(6,0) = 1
	end if

	If Session("INV_DELETE_LOCATION" & dbkey) then
		arrLinks(7,0) = 1
	end if
end if

ShowMenuLinks(arrLinks)


sub ShowMenuLinks(byref arrLinks)

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
		border-left-color: #999999;
		border-top-color:#999999;
		border-bottom-color:#999999;
		border-right-color:#999999;
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
		width: 80px; /* width needed or else Opera goes nuts */
		border-width:1px 1px 0px 1px;
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
		height:20px;
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
		margin-top:16px;
		margin-left:-39px;
		background-color:cccccc;

	}

	.dropDownMenuControl {
		background-color:#cccccc; //D4D0C8
		border:1px solid #999;
		display:none;
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


<%
end sub


sub WritePullDownMenu(byref arrLinks, byref arrCategories)

	Response.Write "<div class=""dropDownMenuControl""><ul id=""nav"">"
	for	i=0 to ubound(arrCategories)
		Response.Write "<li style=""color:#006699;"">" & arrCategories(i) & "<ul>"
		WritePullDownMenuLinks arrLinks, arrCategories(i)
		Response.Write "</ul></li>"
	next
	Response.Write "</ul></div>"
	
end sub

sub WritePullDownMenuLinks(byref arrLinks, category)
	altTitle = ""
	for j = 0 to ubound(arrLinks)
		if arrLinks(j,6) <> "" then altTitle = arrLinks(j,6)
		if arrLinks(j,5) = category then
			if arrLinks(j,0) = 1 then
				Response.Write "<li><a href=""#"" title=""" & altTitle & """ onclick=""OpenDialog('" & arrLinks(j,1) & "', 'Diag', " & arrLinks(j,3) & "); return false"" class=""contextMenu"" onmouseover=""this.className = 'menuOn'"" onmouseout=""this.className = 'contextMenu';"" >" & arrLinks(j,2) & "</a></li>"
			elseif arrLinks(j,4) = 1 then
				Response.Write "<li><a href=""#"" title=""" & altTitle & """ class=""contextMenu"" style=""color:#808080;cursor: default;"">" & arrLinks(j,2) & "</A></li>"
			end if
		end if
	next

end sub

%>
