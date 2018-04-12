<%@ LANGUAGE=VBScript %>
<%
dim action
Session("isSearchMode") = True
'If isEmpty(Session("ExcludeChecked")) then Session("ExcludeChecked") = ""

%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
		<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
		<script type="text/javascript" language="JavaScript">
		
			// hide the search link
			if (!top.bannerFrame.document.all.searchLink){
			}
			else{
				top.bannerFrame.document.all.searchLink.style.visibility = "hidden";
			}
			
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
			
			function display(id, str){
				if (NS){
					with (document[id].document){
						open();
						write(str);
						close();
					}
				}
				else{
					if (document.all[id]) document.all[id].innerHTML = str;
				}	
			}
			
			function SetLocationSQL(LocationID){
				var sql = "";
				var tableName
				if (formgroup == "plates_form_group" || formgroup == "plates_np_form_group"){
					tableName = "inv_plates.location_id_fk";
				}
				else{
					tableName = "inv_containers.Location_ID_FK";
				}
				if (LocationID.length > 0){
					if (document.cows_input_form.searchSubLocations.checked){
						sql =  " " + tableName + " IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID = " + LocationID + ")";
					}
					else{
						sql = " " + tableName + " = " + LocationID; 
					}
				}						
				if (document.cows_input_form.SpecialLocationList.value.length > 0){
					if (document.cows_input_form.ExcludeSpecialLocations.checked){
						if (sql.length > 0) sql += " AND ";
						sql += tableName + " NOT IN (SELECT Location_ID from inv_Locations CONNECT BY prior Location_ID = Parent_ID START WITH Location_ID IN (" + document.cows_input_form.SpecialLocationList.value + "))";
					}				
				}
				
				//alert(sql)
				document.cows_input_form[tableName].value = sql;
			}
			
			
			// Posts the form when a tab is clicked
			function SwitchTab(sTab, formgroup) {
				var url = "cheminv_input_form.asp?TB=" + sTab + "&formgroup=" + formgroup + "&formmode=search&dbname=cheminv"; 
				this.location.href = url;
			}
				
		</script>
		<title><%=Application("appTitle")%> -- Structure Query Form</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<!--#INCLUDE FILE="../source/secure_nav.asp"-->
		<!--#INCLUDE FILE="../source/app_js.js"-->
		<!--#INCLUDE FILE="../source/app_vbs.asp"-->
		<!--#INCLUDE VIRTUAL="/cheminv/gui/guiutils.asp"-->
		<!--#INCLUDE VIRTUAL="/cheminv/api/apiutils.asp"-->
	<script LANGUAGE="javascript">
		// This flag tells the onload function called from recordset_footer to process the AfterOnLoad() function
		DoAfterOnLoad = true;
			
		function AfterOnLoad(){
			if (document.cows_input_form["inv_compounds.CAS"]){
			document.cows_input_form["inv_compounds.CAS"].focus();
			}
		}
	</script>

	</head>
	<body bgcolor="#FFFFFF">
<%
	'-- set the return action so that the cancel button comes back to this page	
	Session("returnaction") = formgroup
	Session("formgroup") = 	formgroup
    Session("isBatchSearch") = "" 	
%>
	<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/SearchFormTabs.asp"-->
	<% 
		Dim Conn
		invSpecialLocs = GetUserProperty(Session("UserNameCheminv"), "invSpecialLocs")
		if formgroup = "base_form_group" then checked = "Checked" %>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
        <!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
dateFormatString = Application("DATE_FORMAT_STRING")
Select Case lCase(sTab)
	Case "advanced"
%>
		<table border="0" width="500" cellpadding="0" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td valign="top">
				<!-----COL 1 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.CAS", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.ACX_ID", "0","30"%>
						</td>
					</tr>
					<%if Application("RegServerName") <> "NULL" then%>
		      		<tr>
		        		<td>
							<span title="(e.g. AB-00001)">Reg Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.RegNumber", "0","30"%>
						</td>
		      		</tr>
		      		<%end if%>
		      		<tr>
		        		<td>
							<span title="Numerical value">Purity:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Purity", "0","12"%>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Purity_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Concentration:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Concentration", "0","12"%>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Conc_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Density:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Density", "0","12"%>
							<%= ShowSelectBox2("inv_containers.unit_of_Density_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 6 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. HPLC, ACS, Ultra Pure)">Grade:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Grade", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Size <span id="UOMtext1"></span>:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Qty_Max", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Must be a number">Container Cost($):<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Cost", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="Use a barcode reader or the browse link to select the location to search">Location ID: <%=GetBarcodeIcon()%></span><br>
							<%ShowLocationPicker "document.cows_input_form", "tempLocation_ID", "LocationBarCode", "LocationName", 10, 16, true%> 
							<font size="1"><input Type="CheckBox" name="searchSubLocations"  onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)" <%if Application("DefaultSearchSublocations") then %>checked <%end if %>>Search Sublocations</font>         
							<br>
							<input type="hidden" name="SpecialLocationList" value="<%=invSpecialLocs%>">
							<font size="1"><input Type="CheckBox" name="ExcludeSpecialLocations" value="checked" <%if Application("DefaultExcludeSpecialLocations") then %>checked <%end if %> onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)">Exclude <a class="menuLink" href="#" onclick="OpenDialog('SpecialLocationSelector.asp', 'LocSelectDiag', 1); return false">Special</a> Locations</font>         
							<input type="hidden" name="inv_containers.Location_ID_FK" value>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 2 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="One or more Container Barcode Id values separated by commas">Container ID:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Barcode", "0","30"%>            
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="One or more Container ID values separated by commas">Container ID (internal):&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_ID", "0","30"%>     
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Descriptive name of the container">Container Name:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Pick an option from the list">Container Type:</span><br>
							<%= ShowSelectBox2("inv_containers.Container_Type_ID_FK", "", "SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_container_types ORDER BY lower(Container_Type_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Container Status:</span><br>
							<%= ShowSelectBox2("inv_containers.Container_Status_ID_FK", "", "SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_container_status ORDER BY lower(Container_Status_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Unit of Measure:</span><br>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Meas_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(Unit_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Qty Remaining<span id="UOMtext2"></span>:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Qty_Remaining", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Qty Available<span id="UOMtext3"></span>:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Qty_Available", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="One or more Location Barcode values separated by commas">Location Barcode:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Locations.Location_Barcode", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Numerical value">Compound ID:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Compound_ID_FK", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Restrict the search to find only duplicates compounds">Compound Type:<br>
							<SELECT name="inv_compounds.Conflicting_Fields">
								<Option value="" size="1"><%=RepeatString(43, "&nbsp;")%>
								<Option value="*">Duplicate Compounds 
							</SELECT>
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="Supplier's catalog number">Catalog Number:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Supplier_CatNum", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Pick an option from the list">Supplier:</span><br>
							<%= ShowSelectBox2("inv_containers.Supplier_ID_FK", "", "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Supplier's lot number">Lot Number:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Lot_Num", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Expiration Date:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Date_Expires", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="User that placed the order for a container">Ordered By:<br>
							<%= ShowSelectBox2("inv_containers.Ordered_By_ID_FK", "", "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Date Ordered:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Date_Ordered", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Date Received:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Date_Received", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Number">PO Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.PO_Number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Line Number">PO Line Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.po_line_number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Line Number">Requisition Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.REQ_Number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="User ID of container owner">Owner:<br>
							<%= ShowSelectBox2("inv_containers.Owner_ID_FK", "", "SELECT owner_id AS Value, description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners ORDER BY lower(description) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="User ID of current user responsible for the container">Current User:<br>
							<%= ShowSelectBox2("inv_containers.Current_User_ID_FK", "", "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
					<tr>
						<td valign="bottom" align="right">
							<%ShowSearchButton()%>&nbsp;
						</td>
					</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 4 -------->
				<table border="0">
				<%
					For each key in custom_fields_dict
						Response.Write "<TR><td>" & vblf
						Response.Write "<span title="""">"
						Response.Write custom_fields_dict.Item(Key) & ":</span><br>"
						if inStr(Key, "DATE_") then
							ShowInputField dbkey, formgroup, "inv_containers." & lcase(Key), "DATE_PICKER:8","27"
						else
							ShowInputField dbkey, formgroup, "inv_containers." & lcase(key), "0","30"  
						end if
						Response.Write "</td></TR>" & vblf 
					Next 
				%>
				</table>
			</td>
			<td valign="top">
				<!-----COL 5 -------->
				<table border="0">
				<%
					For each key in alt_ids_dict
					    strLabel = alt_ids_dict.Item(Key) & ":"   ' WJC Millennium
					    bCustom = (left(strLabel, 1) = "*")
					    if bCustom then strLabel = mid(strLabel,2)
						Response.Write "<TR><td>" & vblf
						if bCustom then
						    response.Write ShowCustomPicklist(Key,"")
						else
						    Response.Write "<span title="""">"
						    Response.Write strLabel & "</span><br>"
						    ShowInputField dbkey, formgroup, key, "0","30"  
                        end if
						Response.Write "</td></TR>" & vblf 
					Next 
				%>
				</table>
			</td>
		  </tr>
		</table>
<%
	Case "simple"
%>
	<table border="0" width="500" cellpadding="10" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td valign="top">
				<!-----COL 1 -------->
				<table border="0">
		      		<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.CAS", "0","30"%>                     
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="One or more Container Barcode Id values separated by commas">Container ID:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Barcode", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="One or more Location Barcode values separated by commas">Location Barcode:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Locations.Location_Barcode", "0","30"%>
						</td>
					</tr>
					
		      		<tr>
			        	<td>
							<span title="Supplier's catalog number">Catalog Number:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Supplier_CatNum", "0","30"%>	        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Purchase Order Number">PO Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.PO_Number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Line Number">PO Line Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.po_line_number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
					</tr>
		      		<tr>
			        	<td>
							
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 2 -------->
				<table border="0">
				    <tr>
			        	<td>
							<span title="Numerical Value">Compound ID:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Compound_ID_FK", "0","50"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Compounds.Substance_Name", "0","50"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Substance Synonym:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_synonyms.Substance_Name", "0","50"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Descriptive name of the container">Container Name:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Name", "0","50"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Container Comments">Container Comments:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Comments", "0","50"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Use a barcode reader or the browse link to select the location to search">Location ID: <%=GetBarcodeIcon()%></span><br>
							<%
							ShowLocationPicker "document.cows_input_form", "tempLocation_ID", "LocationBarCode", "LocationName", 10, 37, true%> 
							<font size="1"><input Type="CheckBox" name="searchSubLocations"  onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)" <%if Application("DefaultSearchSublocations") then %>checked <%end if %>>Search Sublocations</font>
							<br>
							<input type="hidden" name="SpecialLocationList" value="<%=invSpecialLocs%>">
							<font size="1"><input Type="CheckBox" name="ExcludeSpecialLocations" value="checked" <%if Application("DefaultExcludeSpecialLocations") then %>checked <%end if %> onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)" >Exclude <a class="menuLink" href="#" onclick="OpenDialog('SpecialLocationSelector.asp', 'LocSelectDiag', 1); return false">Special</a> Locations</font>         
							<input type="hidden" name="inv_containers.Location_ID_FK">
						</td>
		      		</tr>
		      		<tr>
		        		<td align="right">
		        			<%ShowSearchButton()%>       
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							        
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				<table border="0">
					<tr>
			        	<td>
							        
						</td>
		      		</tr>
					<tr>
			        	<td>
							
						</td>
		      		</tr>
					<tr>
			        	<td>
							 
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		    	</table>
			</td>
		  </tr>
		</table>
		
<%
	Case "substructure"
%>
		<table border="0" cellpadding="0" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td>
				<table border="0">
					<tr>
						<td>
							<%=getGetSearchOptions(dbkey, formgroup, "inv_compounds.Structure", "AllOptions", "SelectList")%>
						</td>
						<td valign="bottom" align="right">
							<%ShowSearchButton()%>&nbsp;
						</td>
					</tr>
					<tr>
			        	<td valign="top" align="left" colspan="2">
							<%ShowStrucInputField  dbkey, formgroup, "inv_compounds.Structure", "1",300, 300, "AllOptions", ""%>
						</td>
		      		</tr>
					<tr>
						<td colspan="2" align="left">
							<input Type="checkbox" onclick="top.navbar.location.reload()" <%=Checked%> name="checkbox1">Group results by chemical structure
						</td>
					</tr>
				</table>			
			</td>
			<td valign="top">
				<!-----COL 1 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Compounds.Substance_Name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.CAS", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.ACX_ID", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. C8H3Cl3O, C1-5O&lt;3F0-1)">Molecular Formula:<br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.Formula", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="&lt;120, 120-130, &gt;250">MolWeight Range:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.MolWeight", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Purity:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Purity", "0","12"%>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Purity_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Concentration:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Concentration", "0","12"%>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Conc_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. HPLC, ACS, Ultra Pure)">Grade:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Grade", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Use a barcode reader or the browse link to select the location to search">Location ID: <%=GetBarcodeIcon()%></span><br>
							<%ShowLocationPicker "document.cows_input_form", "tempLocation_ID", "LocationBarCode", "LocationName", 10, 17, true%> 
							<BR>
							<font size="1"><input Type="CheckBox" name="searchSubLocations" onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)" <%if Application("DefaultSearchSublocations") then %>checked <%end if %>>Search Sublocations</font>         
							<br>
							<input type="hidden" name="SpecialLocationList" value="<%=invSpecialLocs%>">
							<font size="1"><input Type="CheckBox" name="ExcludeSpecialLocations" value="checked" <%if Application("DefaultExcludeSpecialLocations") then %>checked <%end if %> onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)">Exclude <a class="menuLink" href="#" onclick="OpenDialog('SpecialLocationSelector.asp', 'LocSelectDiag', 1); return false">Special</a> Locations</font>         
							<input type="hidden" name="inv_containers.Location_ID_FK">
						</td>
		      		</tr>
		      		<tr>
		        		<td>
						</td>
		      		</tr>
		      		
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 2 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="One or more Container Barcode Id values separated by commas">Container ID:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Barcode", "0","30"%>   
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="One or more Container ID values separated by commas">Container ID (internal):&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_ID", "0","30"%>   
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Descriptive name of the container">Container Name:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Pick an option from the list">Container Type:</span><br>
							<%= ShowSelectBox2("inv_containers.Container_Type_ID_FK", "", "SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_container_types ORDER BY lower(Container_Type_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Container Status:</span><br>
							<%= ShowSelectBox2("inv_containers.Container_Status_ID_FK", "", "SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_container_status ORDER BY lower(Container_Status_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Unit of Measure:</span><br>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Meas_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(Unit_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Size <span id="UOMtext1"></span>:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Qty_Max", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Qty Remain <span id="UOMtext2"></span>:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Qty_Remaining", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Qty Available <span id="UOMtext3"></span>:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Qty_Available", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="One or more Location Barcode values separated by commas">Location Barcode:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_Locations.Location_Barcode", "0","30"%>        
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="Supplier's catalog number">Catalog Number:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Supplier_CatNum", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Pick an option from the list">Supplier:</span><br>
							<%= ShowSelectBox2("inv_containers.Supplier_ID_FK", "", "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Expiration Date:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Date_Expires", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Date Ordered:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Date_Ordered", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Date Received:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Date_Received", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Number">PO Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.PO_Number", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Supplier's lot number">Lot Number:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Lot_Num", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Must be a number">Container Cost($):<br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Container_Cost", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="User ID of container owner">Owner:<br>
							<%= ShowSelectBox2("inv_containers.Owner_ID_FK", "", "SELECT owner_id AS Value, description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners ORDER BY lower(description) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="User ID of current user responsible for the container">Current User:<br>
							<%= ShowSelectBox2("inv_containers.Current_User_ID_FK", "", "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		    	</table>
			</td>
		  </tr>
		</table>

<%
	Case "global"
	'Pass credentials to chemreg
	Session("UserName" & "invreg") = Session("UserName" & "cheminv") 
	Session("UserID" & "invreg") = Session("UserID" & "cheminv") 
	'Pass Credentials to ACXOracle
	Dim IsACXORacle
	Dim conArray 
	conArray = Application("base_connectioninvacx")
	if isArray(conArray) then
	    IsACXOracle = (Ucase(conArray(6))="ORACLE")
	    if IsACXOracle then
	        Session("UserName" & "invacx") = Session("UserName" & "cheminv") 
	        Session("UserID" & "invacx") = Session("UserID" & "cheminv") 
	    end if
	end if    
	'set container id depending on .ini setting
	If lcase(Application("RegContainersOnly")) = "true" then
		globalRegContainerID = ">0"	
	else
		globalRegContainerID = ""
	end if
%>
<table border="0" width="500" cellpadding="0" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td>
			<%if session("isCDP") = "TRUE" then%>
				<table border="0">
					<tr>
						<td>
							<%=getGetSearchOptions(dbkey, formgroup, "MolTable.Structure", "AllOptions", "SelectList")%>
						</td>
						<td valign="bottom" align="right">
							<%ShowSearchButton()%>&nbsp;
						</td>
					</tr>
					<tr>
			        	<td valign="top" align="left" colspan="2">
							<%ShowStrucInputField  dbkey, formgroup, "MolTable.Structure", "1",300, 300, "AllOptions", ""%>
						</td>
		      		</tr>
				</table>			
			<%end if%>
			</td>
			<td valign="top">
				<BR><BR>
				<!-----COL 1 -------->
				<table border="0">
		      		<tr>
			        	<td>
							<span title="(eg. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "Synonym.Name", "0","30"%>        
							<input type="hidden" name="inv_synonyms.Synonym_ID" value="<>0" />
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "MolTable.CAS", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<%ShowInputField dbkey, formgroup, "MolTable.ACX_ID", "0","30"%>
						</td>
					</tr>
					<!--dgb added Catalog Number to global search--->
					<tr>
		        		<td>
							<span title="vendor's catalog number)">Catalog Number:<br>
							<%ShowInputField dbkey, formgroup, "Product.CatalogNum", "0","30"%>
						</td>
					</tr>
					<tr>
		        		<td>
							<span title="(e.g. C8H3Cl3O, C1-5O&lt;3F0-1)">Molecular Formula:<br>
							<%ShowInputField dbkey, formgroup, "MolTable.Formula", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="&lt;120, 120-130, &gt;250">MolWeight Range:</span><br>
							<%ShowInputField dbkey, formgroup, "MolTable.MolWeight", "0","30"%>
						</td>
		      		</tr>
		      		<%if Application("RegServerName") <> "NULL" then%>
		      		<tr>
		        		<td>
							<span title="Registry Number">Registry Number:<br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Reg_Number", "0","30"%>
						</td>
					</tr>
					<tr>
		        		<td>
							<span title="Registry Number">Registry Sequence:<br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Sequence_Number", "0","30"%>
						</td>
					</tr>
					<tr>
		        		<td>
							<span title="Registry Alternate Identifiers (ie. CAS, Chemical name...)">Reg Alternate IDs:<br>
							<%ShowInputField dbkey, formgroup, "Alt_IDS.Identifier", "0","30"%>
						</td>
					</tr>
					<%End if%>
		      		<tr>
		        		<td>
							<!---The following field ensures that reg searches return only substances with an associated container--->
							<INPUT type="hidden" name="inv_containers.Container_ID" value="<%=globalRegContainerID%>">
							<!---Note that the container_id field is mapped invacx.ini onto substance.csnum field.  
							This prevents a join from Access to Oracle.  TODO: Remap for Oracle version of ACX--->
							
							<!---The following field ensures that acx searches return only substances with an associated acx product--->
							<INPUT type="hidden" name="MolTable.HasProducts" value="<>0">
							<!---The HasProducts field is only needed for ACX searches.  In cheminv.ini and chemreg.ini we need to map it to
							a field that will never be zero so that the value "<> 0" clause will not eliminate rows from the search.  
							In particular we use container_id in ChemInv and Reg_ID in ChemReg which are guaranteed to be non zero--->
						</td>
					</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		
		    	</table>
			</td>
			<td valign="top">
				<BR><BR>
				<table border="0">
					<%
					For each key in alt_ids_dict
					    strLabel = alt_ids_dict.Item(Key) & ":"   ' WJC Millennium
					    bCustom = (left(strLabel, 1) = "*")
					    if bCustom then strLabel = mid(strLabel,2)
						Response.Write "<TR><td>" & vblf
						if bCustom then
						    response.Write ShowCustomPicklist(Key,"")
						else
						    Response.Write "<span title="""">"
						    Response.Write strLabel & "</span><br>"
						    ShowInputField dbkey, formgroup, "MolTable." &  Replace(key, "inv_compounds.",""), "0","30"  
                        end if
						Response.Write "</td></TR>" & vblf 
					Next 
					%>
				</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				<table border="0">
					<tr>
			        	<td>
							
						</td>
		      		</tr>
					<tr>
			        	<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
					</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		    	</table>
			</td>
		  </tr>
		</table>

<%
	Case "batches"
%>
		<!--<input type="hidden" name="return_location" value="/cheminv/cheminv/batches_result_list.asp?formgroup=batches_form_group&dbname=cheminv">-->
		<table border="0" cellpadding="0" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
		  	<td>
				<%if session("isCDP") = "TRUE" then%>
				<table border="0">
					<tr>
						<td>
							<%=getGetSearchOptions(dbkey, formgroup, "inv_compounds_alias.Structure", "AllOptions", "SelectList")%>
						</td>
						<td valign="bottom" align="right">
							<%'ShowSearchButton()%>&nbsp;
						</td>
					</tr>
					<tr>
			        	<td valign="top" align="left" colspan="2">
							<%ShowStrucInputField  dbkey, formgroup, "inv_compounds_alias.Structure", "1",300, 300, "AllOptions", ""%>
						</td>
		      		</tr>
				</table>			
				<% end if %>
			</td>

		  	<td valign="top">
				<table>
		      		<tr>
			        	<td>
							<span title="One or more Container Barcode values separated by commas">Container ID:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers_alias.barcode", "0","30"%>            
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Batch Id">Batch Id:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_container_batches.batch_id", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Batch Status:</span><br>
							<%= ShowSelectBox2("inv_container_batches.batch_status_id_fk", "", "SELECT Batch_Status_ID AS Value, Batch_Status_Name AS DisplayText FROM inv_batch_status ORDER BY lower(Batch_Status_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
					<tr>
		        		<td>
							<span title="Minimum Stock Threshold">Minimum Stock Threshold:<br>
							<%ShowInputField dbkey, formgroup, "inv_container_batches.minimum_stock_threshold", "0","30"%>
						</td>
					</tr>

					<% Call GetInvConnection()
					SQL = "SELECT FIELD_NAME,DISPLAY_NAME FROM " & Application("CHEMINV_USERNAME") & ".inv_container_batch_fields"
					Set Cmd = GetCommand(Conn, SQL, adCmdText)
					Set RS = Cmd.Execute
					count=1
					if not (rs.eof and rs.bof) then
					do while not rs.eof %>
					<tr>
		        		<td>
							<span title="<%=RS("DISPLAY_NAME")%>"><%=RS("DISPLAY_NAME")%>:<br>
							<%ShowInputField dbkey, formgroup, "inv_container_batches.batch_field_" & count & "", "0","30"%>
						</td>
					</tr>
					<% count=count+1
					RS.MoveNext
					Loop
					End If%> 					
					<tr>
						<%
							For each key in custom_batch_property_fields_dict
								Response.Write "<TR><td>" & vblf
								Response.Write "<span title="""">"
								Response.Write custom_batch_property_fields_dict.Item(Key) & ":</span><br>"
								if inStr(Key, "DATE_") then
									ShowInputField dbkey, formgroup, "inv_container_batches." & lcase(Key), "DATE_PICKER:8","27"
								else
									ShowInputField dbkey, formgroup, "inv_container_batches." & lcase(key), "0","30"  
								end if
								Response.Write "</td></TR>" & vblf 
							Next 
						%>
		      		<tr>
						<td valign="bottom" align="right">
							<%ShowSearchButton()%>&nbsp;
						</td>
		      		</tr>
		      	</table>
		     </td>

<!--
			<td>
				<%if session("isCDP") = "TRUE" then%>
				<table border="0">
					<tr>
						<td>
							<%=getGetSearchOptions(dbkey, formgroup, "inv_vw_reg_structures.BASE64_CDX", "AllOptions", "SelectList")%>
						</td>
						<td valign="bottom" align="right">
							<%ShowSearchButton()%>&nbsp;
						</td>
					</tr>
					<tr>
			        	<td valign="top" align="left" colspan="2">
							<%ShowStrucInputField  dbkey, formgroup, "inv_vw_reg_structures.BASE64_CDX", "1",300, 300, "AllOptions", ""%>
						</td>
		      		</tr>
				</table>			
				<% end if %>
			</td>
			<td valign="top">
-->
				<!-----COL 1 -------->
<!--
				<table border="0">
					<%if Application("RegServerName") <> "NULL" then%>
		      		<tr>
		        		<td>
							<span title="(e.g. AB-00001)">A-Code:<br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.RegNumber", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Batch Number">Lot Number:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.BatchNumber", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Batch Number">Project:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.RegProject", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Reagent Name:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.RegName", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="">Source:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.RegSource", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="">Dispensing Approver:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.Approver1", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="">Backup Approver:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.Approver2", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="">Species:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.Species", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="">Production Cell Line:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.Cell_Line", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="">Recognized Antigen:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.Antigen", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="CAS Registry #">CAS Registry #:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches.regcas", "0","30"%>        
						</td>
		      		</tr>
		      		<%end if%>
-->
		      		<!--
					<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.CAS", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.ACX_ID", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. C8H3Cl3O, C1-5O&lt;3F0-1)">Molecular Formula:<br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.Formula", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="&lt;120, 120-130, &gt;250">MolWeight Range:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.MolWeight", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Concentration:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers.Concentration", "0","12"%>
							<%= ShowSelectBox2("inv_containers.Unit_Of_Conc_ID_FK", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		-->

		    	</table>
			</td>
			<td valign="top">
				<!-----COL 2 -------->
				&nbsp;
				<table>
<!--
		      		<tr>
			        	<td>
							<span title="Container Name">Container Name:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_containers_alias.Container_Name", "0","30"%>        
						</td>
		      		</tr>
-->
		      	</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				&nbsp;
			</td>
		  </tr>
		</table>


<%
	Case "plate"
%>
		<table border="0" cellpadding="0" cellspacing="0">
		  	<tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td>
				<%if Session("isCDP") = "TRUE" then%>
				<table border="0">
					<tr>
						<td>
							<%=getGetSearchOptions(dbkey, formgroup, "well_compounds_alias.Structure", "AllOptions", "SelectList")%>
						</td>
						<td valign="bottom" align="right">
							<%ShowSearchButton()%>&nbsp;
						</td>
					</tr>
					<tr>
			        	<td valign="top" align="left" colspan="2">
			        		<input type="hidden" name="inv_wells_alias.well_id">
							<%ShowStrucInputField  dbkey, formgroup, "well_compounds_alias.Structure", "1",300, 300, "AllOptions", ""%>
						</td>
		      		</tr>
					<tr>
						<td colspan="2" align="left">
							<input Type="checkbox" onclick="top.navbar.location.reload()" <%=Checked%> name="checkbox1">Group results by chemical structure
						</td>
					</tr> 
				</table>			
				<%end if%>
			</td>
			<td valign="top">
				<!-----COL 1 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "well_compounds_alias.substance_name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "well_compounds_alias.cas", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:<br>
							<%ShowInputField dbkey, formgroup, "well_compounds_alias.acx_id", "0","30"%>
						</td>
					</tr>
					<%if Application("RegServerName") <> "NULL" then%>
		      		<tr>
		        		<td>
							<span title="(e.g. AB-00001)">Reg Number:<br>
							<%ShowInputField dbkey, formgroup, "inv_vw_reg_batches_alias.regnumber", "0","30"%>
						</td>
		      		</tr>
		      		<%end if%>
		      		<tr>
		        		<td>
							<span title="(e.g. C8H3Cl3O, C1-5O&lt;3F0-1)">Molecular Formula:<br>
							<%ShowInputField dbkey, formgroup, "well_compounds_alias.Formula", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="&lt;120, 120-130, &gt;250">MolWeight Range:</span><br>
							<%ShowInputField dbkey, formgroup, "well_compounds_alias.MolWeight", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Purity:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.purity", "0","12"%>
							<%= ShowSelectBox2("inv_plates.purity_unit_fk", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Concentration:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.concentration", "0","12"%>
							<%= ShowSelectBox2("inv_plates.conc_unit_fk", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. HPLC, ACS, Ultra Pure)">Solvent:</span><br>
							<%= ShowSelectBox2("inv_plates.plate_solvent_id_fk", "", "SELECT solvent_id AS Value, solvent_name AS DisplayText FROM inv_solvents ORDER BY lower(Solvent_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Use a barcode reader or the browse link to select the location to search">Location ID: <%=GetBarcodeIcon()%></span><br>
							<%ShowLocationPicker "document.cows_input_form", "tempLocation_ID", "LocationBarCode", "LocationName", 10, 17, true%> 
							<br>
							<font size="1"><input Type="CheckBox" name="searchSubLocations" onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)" <%if Application("DefaultSearchSublocations") then %>checked <%end if %>>Search Sublocations</font>         
							<br>
							<input type="hidden" name="SpecialLocationList" value="<%=invSpecialLocs%>">
							<font size="1"><input Type="CheckBox" name="ExcludeSpecialLocations" value="checked" <%if Application("DefaultExcludeSpecialLocations") then %>checked <%end if %> onclick="SetLocationSQL(document.cows_input_form.tempLocation_ID.value)">Exclude <a class="menuLink" href="#" onclick="OpenDialog('SpecialLocationSelector.asp', 'LocSelectDiag', 1); return false">Special</a> Locations</font>         
							<input type="hidden" name="inv_plates.location_id_fk">
						</td>
		      		</tr>
		      		<tr>
		        		<td>
						</td>
		      		</tr>
		      		
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 2 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Plate Barcode:<%=GetBarcodeIcon()%>&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.plate_barcode", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Number">Plate Map:<br>
							<%= ShowSelectBox2("inv_plates.plate_map_id_fk", "", "SELECT plate_id AS Value, plate_name AS DisplayText FROM inv_plates WHERE is_plate_map = 1 ORDER BY lower(plate_name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="One or more Plate ID values separated by commas">Plate ID (internal):&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.plate_id", "0","30"%>   
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Descriptive name of the plate">Plate Name:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.plate_name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Pick an option from the list">Plate Type:</span><br>
							<%= ShowSelectBox2("inv_plates.plate_type_id_fk", "", "SELECT Plate_Type_ID AS Value, Plate_Type_Name AS DisplayText FROM inv_plate_types ORDER BY lower(Plate_Type_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Plate Status:</span><br>
							<%= ShowSelectBox2("inv_plates.status_id_fk", "", "SELECT Enum_ID AS Value, Enum_value AS DisplayText FROM inv_enumeration WHERE Eset_id_fk = 2 ORDER BY lower(Enum_value) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Pick an option from the list">Unit of Measure:</span><br>
							<%= ShowSelectBox2("inv_plates.qty_unit_fk", "", "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(Unit_Name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Qty Remaining <span id="UOMtext2"></span>:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.qty_remaining", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Qty Initial <span id="UOMtext3"></span>:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.qty_initial", "0","30"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Numerical value">Molar Amount:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.molar_amount", "0","12"%>
							<%= ShowSelectBox2("inv_plates.molar_unit_fk", "", "SELECT UNIT_ID AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY lower(Unit_Abreviation) ASC", 16, RepeatString(20, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							&nbsp;
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 3 -------->
				<table border="0">
					<tr>
			        	<td>
							<span title="Supplier's name">Supplier Name:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.supplier", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Supplier's shipment code">Supplier Shipment Code:&nbsp;</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.supplier_shipment_code", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="Supplier's shipment number">Supplier Shipment Number:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_plates.supplier_shipment_number", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Supplier Shipment Date:<br>
							<%ShowInputField dbkey, formgroup, "inv_plates.supplier_shipment_date", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="<%=dateFormatString%>">Date Created:<br>
							<%ShowInputField dbkey, formgroup, "inv_plates.date_created", "DATE_PICKER:8","27"%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Purchase Order Number">Plate Format:<br>
							<%= ShowSelectBox2("inv_plates.plate_format_id_fk", "", "SELECT plate_format_id AS Value, plate_format_name AS DisplayText FROM inv_plate_format ORDER BY lower(plate_format_name) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
			        	<td>
							<span title="Supplier's lot number">Library:</span><br>
							<%= ShowSelectBox2("inv_plates.library_id_fk", "", "SELECT Enum_ID AS Value, Enum_value AS DisplayText FROM inv_enumeration WHERE Eset_id_fk = 5 ORDER BY lower(Enum_value) ASC", 30, RepeatString(43, "&nbsp;"), "")%>
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Must be a number">Group Name:<br>
							<%ShowInputField dbkey, formgroup, "inv_plates.group_name", "0","30"%>
						</td>
					</tr>
		      		<tr>
		        		<td>
							<span title="Freeze/Thaw Cycles">Freeze/Thaw Cycles:<br>
							<%ShowInputField dbkey, formgroup, "inv_plates.ft_cycles", "0","30"%>						</td>
		      		</tr>
		      		<tr>
		        		<td>
							
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<!-----COL 4 -------->
				<table border="0">
				<%
					For each key in custom_plate_fields_dict
						Response.Write "<TR><td>" & vblf
						Response.Write "<span title="""">"
						Response.Write custom_plate_fields_dict.Item(Key) & ":</span><br>"
						if inStr(Ucase(Key), "DATE_") then
							ShowInputField dbkey, formgroup, "inv_plates." & lcase(Key), "DATE_PICKER:8","27"
						else
							ShowInputField dbkey, formgroup, "inv_plates." & lcase(key), "0","30"  
						end if
						Response.Write "</td></TR>" & vblf 
					Next 
				%>
				</table>
			</td>
		  </tr>
		</table>

	<!--#INCLUDE VIRTUAL = "/cheminv/custom/cheminv/custom_searchTab_cases.asp"-->
		
<%End Select%>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
	<script language="JavaScript">
		function SetSearchCriteria(){
			var chkfg;
			var unchkfg;
			
			if ((formgroup == "plates_form_group") || (formgroup == "plate_compounds_form_group")){
				chkfg = "plate_compounds_form_group";
				unchkfg = "plates_form_group";
				document.cows_input_form.checkbox1.checked ? document.cows_input_form["inv_wells_alias.well_id"].value = ">0" : document.cows_input_form["inv_wells_alias.well_id"].value = "";
			}
			else if ((formgroup == "batches_form_group")){
				chkfg = "batches_form_group";
				unchkfg = "batches_form_group";
				//document.cows_input_form.checkbox1.checked ? document.cows_input_form["inv_container_batches.batch_id"].value = ">0" : document.cows_input_form["inv_container_batches.batch_id"].value = "";
			}
			else{
				chkfg = "base_form_group";
				unchkfg = "containers_form_group";	
			}
			getAction("<%=action%>", document.cows_input_form.checkbox1.checked ? chkfg : unchkfg);
		}
		
		var uomElm = "inv_containers.Unit_Of_Meas_ID_FK";
		//alert(formgroup)
		if (document.cows_input_form["inv_plates.qty_unit_fk"]) uomElm = "inv_plates.qty_unit_fk";
		if (document.cows_input_form[uomElm]) {
			document.cows_input_form[uomElm].onchange = function(){var unitName = "(<font size=1>" + this.options(this.options.selectedIndex).innerText + "</font>)"; display("UOMtext1", unitName );display("UOMtext2", unitName );display("UOMtext3", unitName )};	
		}
		
		
		
		if (MainWindow.document.cows_input_form.checkbox1){
			//alert(document.cows_input_form.checkbox1.checked ? chkfg : unchkfg)
			document.cows_input_form.onsubmit = function(){SetSearchCriteria(); return false;}
		}
		else{
			//DJP: this doesn't need to be set b/c core will do it and this is resetting what core is doing
			//document.cows_input_form.onsubmit = function(){getAction("search"); return false;}
		}
		 <%'if Session("ExcludeChecked") <> "" then%>
		//if (document.cows_input_form["ExcludeSpecialLocations"]) {
		//	document.cows_input_form["ExcludeSpecialLocations"].click();	
		//}
		<%'end if%>-->
		
	</script>
	
	</body>
</html>
