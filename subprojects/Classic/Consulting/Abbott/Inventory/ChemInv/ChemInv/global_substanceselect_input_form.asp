<%@ LANGUAGE=VBScript %>
<%
'Pass credentials to chemreg
Session("UserName" & "invreg") = Session("UserName" & "ChemInv") 
Session("UserID" & "invreg") = Session("UserID" & "ChemInv") 
%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script language="JavaScript">
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
			// hide the search link
			
			if (!top.bannerFrame.document.all.searchSubstancesLink) {
			}
			else{
			//top.bannerFrame.document.all.searchLink.style.visibility = "hidden";
			top.bannerFrame.document.all.searchSubstancesLink.style.visibility = "visible";
			top.bannerFrame.document.all.SearchSubs.innerHTML= "Search Containers"
			top.bannerFrame.document.all.SearchSubs.title = "Search chemical inventory containers"; 
			top.bannerFrame.document.all.SearchSubs.href = "../inputtoggle.asp?formgroup=base_form_group&dataaction=db&dbname=cheminv";
			
			}
			
			
			function SwitchFormGroup(bchecked)
			{
				if (bchecked)
				{
					top.location.href = "../inputtoggle.asp?formgroup=base_form_group&dbname=cheminv"
				}
				else
				{
					top.location.href = "../inputtoggle.asp?formgroup=containers_form_group&dbname=cheminv"
				}
			}
			
		</script>
		<title><%=Application("appTitle")%> -- Structure Query Form</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<!--#INCLUDE FILE="../source/secure_nav.asp"-->
		<!--#INCLUDE FILE="../source/app_js.js"-->
		<!--#INCLUDE FILE="../source/app_vbs.asp"-->
		<!--#INCLUDE VIRTUAL="/cheminv/gui/guiutils.asp"-->
	</head>
	<body bgcolor="#FFFFFF">
	<% 
	if formgroup = "base_form_group" then checked = "Checked" %>
		<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan="2">
				<span class="GuiFeedback">Search for a substance to associate to a container.</span><br><br>
			</td>
		  </tr>
		</table>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
		<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td valign="top">
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
			<% end if %>
			</td>
			<td valign="top">
				<br>
				<table border="0">
					<tr>
			        	<td>
							<span title="(eg. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "Synonym.Name", "0","30"%>        
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
							<span title="A Number">A-Code:<br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Reg_Number", "0","30"%>
						</td>
					</tr>
					<tr>
		        		<td>
							<span title="Lot Number">Lot Number:<br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Batch_Number", "0","30"%>
						</td>
					</tr>
		      		<tr>
			        	<td>
							<span title="Project">Project:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Reg_Project", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(e.g. Acetonitrile)">Reagent Name:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.regname", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title>Source:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.RegSource", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title>Dispensing Approver:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Approver1", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title>Backup Approver:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Approver2", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title>Species:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Species", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title>Production Cell Line:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Cell_Line", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title>Recognized Antigen:</span><br>
							<%ShowInputField dbkey, formgroup, "Reg_Numbers.Antigen", "0","30"%>        
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
		    	</table>
			</td>
			<td valign="top">
				<br>
				<table border="0">
					<%
					For each key in alt_ids_dict
						Response.Write "<TR><td>" & vblf
						Response.Write "<span title="""">"
						Response.Write alt_ids_dict.Item(Key) & ":</span><br>"
						ShowInputField dbkey, formgroup, "MolTable." &  Replace(key, "inv_compounds.",""), "0","30"  
						Response.Write "</td></TR>" & vblf 
					Next 
					%>
				</table>
			</td>
		  </tr>
		</table>
		<input Type="Image" Src="/cheminv/graphics/pixel.gif" id="Image1" name="Image1">
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
	<script language="JavaScript">
	// Only select default db
	var globalDBs = "<%=Application("GlobalSearchDBs")%>";
	var defSearchDB = "<%=Application("DEFAULT_SEARCH_DATABASE")%>";
	<% 
	'-- Check default DB if defined in INVCONGIF.INI
	if not isEmpty(Application("DEFAULT_SEARCH_DATABASE")) and instr(Application("GlobalSearchDBs"),Application("DEFAULT_SEARCH_DATABASE")) > 0 then
		response.write("document.cows_input_form." & Application("DEFAULT_SEARCH_DATABASE") & ".checked = true;")
		arrGlobalDBs = split(Application("GlobalSearchDBs"),",")
		for i = 0 to ubound(arrGlobalDBs)
			if arrGlobalDBs(i) <> Application("DEFAULT_SEARCH_DATABASE") then
				response.write("document.cows_input_form." & arrGlobalDBs(i) & ".checked=false;")
			end if
		next
	end if 
	%>
	
	document.cows_input_form.onsubmit = function(){getAction("search","global_substanceselect_form_group"); return false;}
	</script>
	</body>
</html>
