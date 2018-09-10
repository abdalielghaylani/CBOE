<%@  language="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/xml_source/RS2HTML.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/cASPString.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
Dim rsPlate

if Session("viewWellPlateFilter") = "" then
	viewWellPlateFilter = "wellformat"
else
	viewWellPlateFilter = Session("viewWellPlateFilter")
end if

dbkey = "ChemInv"
plateID = Request("PlateID")
compoundID = Request("CompoundID")
refresh = Request("refresh")
if refresh = "True" then
	Session("sPlateTab") = ""
end if
Call GetInvConnection()
SelectWell = Session("SelectWell")
if lcase(SelectWell) = "undefined" then Session("SelectWell") = 0
Set machine = Server.CreateObject("WScript.Network")
ComputerName = machine.ComputerName
clientName = Cstr(ComputerName)
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetPlateAttributes.asp"-->
<html>
<head>
    <script language="javascript" type="text/javascript" src="/cheminv/utils.js"></script>
    <script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
    <script language="javascript" type="text/javascript" src="/cheminv/gui/refreshGUI.js"></script>
    <script language="javascript" type="text/javascript">
<!--Hide JavaScript
    // Posts the form when a tab is clicked
    function postDataFunction(sTab) {
        //document.form1.action = "ViewPlate.asp?TB=" + sTab
        document.form1.action = "ViewPlateFrame.asp?TB=" + sTab + "&PlateID=<%=plateID%>&WellCriterion=<%=wellCriterion%>&GetData=db"
        document.form1.submit()
    }

    // move the menu over to the right so it doesn't render below the CD control
    //AlterCSS('.firstList','margin','0px 0px 0px 100px');

    //-->
    </script>
    <style type="text/css">
        .firstList {
            margin: 0px 0px 0px 100px;
        }
        /* IE10 does not detect AlterCSS, hence added this style */
    </style>
</head>
<body>
    <%if Application("UseCustomTabFrameLinks") then%>
    <!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/plate_tab_frame_links.asp"-->
    <%else%>
    <!--#INCLUDE VIRTUAL = "/cheminv/gui/plate_tab_frame_links.asp"-->
    <%end if%>
    <div style="border-bottom-color: #CFD8E6; border-left-color: #fff; border-top-color: #fff; border-style: solid; border-left-width: 0px; border-bottom-width: 2px; border-right-width: 0px;">
        <!--#INCLUDE VIRTUAL = "/cheminv/gui/PlateViewTabs.asp"-->
    </div>
    <script language="javascript" type="text/javascript">
        //alert(parent.parent.TreeFrame);
        //alert(parent.parent.parent.TreeFrame);
        //alert(parent.parent.parent.parent.TreeFrame);
        //alert(parent);
        //alert(parent.parent);
        //alert(parent.name);
        //alert(parent.parent.name);
        //alert(parent.parent.parent);
        //alert(parent.parent.parent.parent);
    </script>
    <form name="form1" action="echo.asp" xaction="NewLocation_action.asp" method="POST">
        <input type="hidden" name="PlateID" value="<%=plateID%>">
        <%
sPlateTab = Session("sPlateTab")
Select Case sPlateTab
	Case "Summary"

        %>
        <!--#INCLUDE VIRTUAL = "/cheminv/gui/GetPlateAttributes.asp"-->
        <%		
		'Response.Write SQL
		Set oTemplate = Server.CreateObject("MSXML2.FreeThreadedDOMDocument")
		oTemplate.load(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/ViewPlate_Summary.xml"))
		Set mainTable = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/TABLE_ELEMENT")
		'Response.Write mainTable.xml & "=xml<BR>"
		'Set newNode = oTemplate.createNode(1, "FIELD", "")

        ' show group security info
        if Application("ENABLE_OWNERSHIP") = "TRUE" then		    
    		Set currentUserNode = oTemplate.selectSingleNode("/DOCUMENT/DISPLAY/TABLE_ELEMENT/FIELD[last()]")
			Set newNode = CreateFieldNode(oTemplate, "OWNERSHIP_NAME", "GrayedText", null, null,null, "Plate Admin:", "RightAlign", 1, null, null, 1, "#OWNERSHIP_NAME#")
        	Set currNode = mainTable.appendChild(newNode)
            Set newNode = nothing
		end if
		For each key in custom_plate_fields_dict
			Set newNode = CreateFieldNode(oTemplate, ucase(key), "GrayedText", null, null, null, custom_plate_fields_dict.Item(key) & ":", "RightAlign", 1, null, null, 1, "#" & ucase(key) & "#")
			Set currNode = mainTable.insertBefore(newNode,null)
		Next 
	

		'Set currNode = mainTable.insertBefore(newNode,null)

		HTML = RS2HTML(RS,oTemplate,null,null,null,null,null,null,null)
		'create parent plate links
		if len(Session("plParent_Plate_ID_FK")) > 0  then
			arrParentPlateID = split(Session("plParent_Plate_ID_FK"),",")
			arrParentBarcode = split(Session("plParent_Plate_Barcode"),",")
			arrParentLocationID = split(Session("plParent_Plate_Location_ID"),",")
			for i = 0 to ubound(arrParentPlateID)
				'parentLinks = parentLinks & "<span id=""Parent Plate:"" title=""""><A CLASS=""MenuLink"" target=""_Blank"" HREF=""#"" TITLE=""Parent Plate"" ONCLICK=""SelectLocationNode(0," & arrParentLocationID(i) & ", 0, '" & TreeViewOpenNodes1 & "'," & arrParentPlateID(i) & ",1);"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"">" & arrParentBarcode(i) & "</a></span>&nbsp;<BR>"
				 parentLinks = parentLinks & "<span id=""Parent Plate:"" title=""""><A id=""myLink"& i+1 &""" CLASS=""MenuLink"" HREF=""#"" TITLE=""Parent Plate"" ONCLICK=""SelectLocationNode3(0," & arrParentLocationID(i) & ", 0, '" & TreeViewOpenNodes1 & "'," & arrParentPlateID(i) & ",1," & i+1 & ");"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"">" & arrParentBarcode(i) & "</a></span>&nbsp;<BR>"
			next
		end if
		Response.Write "<table cellpadding=""0"" cellspacing=""0"" border=""0""><tr>"
		Response.Write "<td>"
		Response.Write replace(HTML,"PARENTPLATEREPLACEMENT",parentLinks)
		Response.Write "</td>"
        %>
        <td valign="bottom">
            <table border="0">
                <tr>
                    <td>
                        <%
						GetURLs PlateID, "inv_plates", "plate_id", "", "", "", "Plate Links"
                        %>
                    <td>
                </tr>
                <tr>
                    <td>
                        <!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/custom_plate_links.asp"-->
                        <td>
                </tr>
            </table>
        </td>
        <%		
		Response.Write "</tr></table>"		
	Case "PlateViewer"
		if Application("RegServerName") <> "NULL" then
			displayFields = "WellFormat,Name,CAS,MW,RegBatchID,Weight_String,Concentration_String,Solvent"
		else
			displayFields = "WellFormat,Name,CAS,MW,Weight_String,Concentration_String,Solvent"
		End if
        %>
        <!--#INCLUDE VIRTUAL = "/cheminv/gui/writePlateXMLIsland.asp"-->
        <%        
dim fs,tfile
set fs=Server.CreateObject("Scripting.FileSystemObject")
if fs.FileExists(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/" & ComputerName & ".xml")) then
   fs.DeleteFile(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/" & ComputerName & ".xml"))
end if
set tfile=fs.CreateTextFile(Server.MapPath("/" & Application("AppKey") & "/config/xml_templates/" & ComputerName & ".xml"), True)
tfile.Write(xmlHtml)
tfile.close
set tfile=nothing
set fs=nothing
        %>
        <div id="plateViewer" style="top: 60; left: 10; visibility: visible; z-index: -1">
            <table style="table-layout: fixed; border-collapse: collapse; width:98%;" cellspacing="0" cellpadding="1" bordercolor="#666666" id="tbl" datasrc datafld="name" border="1">
                <col width="30">
                <%
		For i=0 to NumCols-1
			'To fix CSBR-152262: Increase the cell size based on its content; Also making this configurable
			if lcase(Application("UseDefaultPlateViewerCellSize")) = "false" then
				Response.Write "<col width=""" & (cellWidth * 4) + fieldLength & """>"
			else
				Response.Write "<col width=""" & cellWidth & """>"
			end if
		Next
                %>
                <thead>
                    <th align="center" class="listCaption">
                        <a href="#" onclick="document.all.hiddenSelector.style.visibility = 'visible';" title="Click to select displayed value">
                            <img src="../graphics/desc_arrow.gif" border="0" width="12" height="6"></a>
                        <a id="hiddenWellSelector" target="wellJSFrame"></a>
                        <div id="hiddenSelector" style="position: Absolute; top: 0; left: 0; visibility: hidden;">
                            <select id="cboField" size="7" onchange="loadXMLDoc();">
                                <option value></option>
                                <option value="wellformat">Well Format</option>
                                <option value="name">Cell Name</option>
                                <option value="cas">CAS Number</option>
                                <option value="mw">MW</option>
                                <%if Application("RegServerName") <> "NULL" then%>
                                <option value="regbatchid">Reg Batch ID</option>
                                <!--<option VALUE="supplier_compound_id">Supplier ID</option>-->
                                <%End if%>
                                <option value="weight_string">Weight</option>
                                <option value="concentration_string">Concentration</option>
                                <option value="solvent">Solvent</option>
                            </select>
                        </div>
                    </th>
                    <%
		For i=0 to NumCols-1
			Response.Write "<th class=""listCaption"">" & colName_arr(0,i) & "</th>" & vblf
		Next
                    %>
                </thead>
                <%	
		Set objXMLDoc = Server.CreateObject("Microsoft.XMLDOM") 
		objXMLDoc.async = False 
		objXMLDoc.Loadxml(xmlHtml)
		Set Root = objXMLDoc.documentElement
		Set NodeList = Root.getElementsByTagName(viewWellPlateFilter)
			Response.Write "<tbody>"	
		For Each xmlItem In NodeList
		    rowname = xmlItem.childNodes(0).text	
			Response.Write "<tr height=""20""><th><span>" & rowname & "</span></th>"
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
        </div>
        <script language="javascript">
            //hide the wait gif

            document.all.waitGIF.style.display = "none";
            //open the selected well if there is one
            //SelectWell = top.ListFrame.document.all.SelectWell.value;
            SelectWell = "<%=iif(Session("SelectWell")="","0",Session("SelectWell"))%>";
            if (SelectWell != "0") {

                var hiddenWellSelector = document.getElementById("hiddenWellSelector");
                hiddenWellSelector.href =	"ViewWellFrame.asp?wellID=" + SelectWell + "&filter=" + '<%=viewWellPlateFilter%>'; 	
                //alert(hiddenWellSelector.href);
                hiddenWellSelector.click();
            }
            function viewWell(well_id){
                alert(well_id);
            }
            document.all.cboField.options[1].selected = true;            
        </script>

        <script language="javascript" type="text/javascript">
            var path = "/ChemInv/config/xml_templates/";
            var client = "<%=clientname%>";
            var columns = "<%=NumCols%>";
            var xmlfile = client + ".xml";
            var fullpath = path + xmlfile;
            var table = document.getElementById("tbl");    

            function loadXMLDoc() {
                document.all.hiddenSelector.style.visibility = "hidden";
                //document.all.cboField.options[1].selected = false;
                // Read the xml from the file        
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
            function BindTable(xml) 
            {
                table.removeChild(table.getElementsByTagName("tbody")[0]);
                var tbody = document.createElement("tbody");

                var plateFilter = document.getElementById("cboField").value;
                wellFilter = plateFilter;
                var RootNode = "";
                RootNode = xml.getElementsByTagName(plateFilter);
                //Read the xml values and create a html string with body,tr,th..
                for (i = 0; i <= RootNode.length - 1; i++)
                { 
                    var tr = document.createElement("tr");
                    rowname = RootNode[i].getElementsByTagName("rowname")[0].childNodes[0].nodeValue;
                    var th = document.createElement("th");
                    th.height = 20;
                    var thtxt = document.createTextNode(rowname);
                    th.appendChild(thtxt);
                    tr.appendChild(th);                    
                    for (j = 1; j <= columns; j++)
                    {
                        col = "col" + j;
                        name = RootNode[i].getElementsByTagName(col)[0].childNodes[0].nodeValue;
                        var td = document.createElement("td");
                        td.align = "center";
                        td.valign = "center";
                        var div = document.createElement("div");
                        div.className = "col" + j;
                        div.innerHTML = name;
                        td.appendChild(div);
                        tr.appendChild(td);
                    }
                    tbody.appendChild(tr);
                }
                table.appendChild(tbody);
            }
        </script>

        <!--<br><br><table>	<tr>		<td>			<table DATASRC="#xmlDoc" DATAFLD="customer" style="table-layout:fixed" BORDER>  <col width="150">  				<col width="150">  				<thead>					<th>NAME</th>					<th>ID</th>				</thead>  				<tr>    					<td><span DATAFLD="name"></span></td>    					<td><span DATAFLD="custID"></span></td>  				</tr>			</table>		</td>		<td>			<table DATASRC="#xmlDoc" DATAFLD="item" style="table-layout:fixed" BORDER>  				<col width="150"><col width="150">  				<thead>					<th>ITEM</th>					<th>PRICE</th>				</thead>  				<tr>    					<td><span DATAFLD="name"></span></td>    					<td><span DATAFLD="price"></span></td>  				</tr>			</table>		</td>	</tr></table>-->
        <!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/custom_plate_tab_cases.asp"-->
        <%end select%>
    </form>
</body>
</html>
