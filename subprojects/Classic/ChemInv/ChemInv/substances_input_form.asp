<%@ LANGUAGE=VBScript %>
<%
Session("bManageMode") = true
if Session("isCDP") = "TRUE" then
	substanceFG = "substances_form_group"
else
	substanceFG = "substances_np_form_group"
end if
%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
        <SCRIPT LANGUAGE="javascript" src="<%=Application("CDJSUrl")%>"></SCRIPT>
		<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
		<script language="JavaScript">
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
			// hide the search link
			
			if (!top.bannerFrame.document.all.searchSubstancesLink) {
			}
			else{
			//top.bannerFrame.document.all.searchLink.style.visibility = "hidden";
			top.bannerFrame.document.all.searchSubstancesLink.style.visibility = "visible";
			top.bannerFrame.document.all.SearchSubs.innerHTML= "Search"
			top.bannerFrame.document.all.SearchSubs.title = "Search"; 
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
		<!--#INCLUDE VIRTUAL="/cheminv/api/apiutils.asp"-->
        <!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
	</head>
	<body bgcolor="#FFFFFF">
	<%
	'-- set the return action so that the cancel button comes back to this page
	Session("returnaction") = formgroup
	%>
	<% if formgroup = "base_form_group" then checked = "Checked" %>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
		<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan="3">
				<span class="GuiFeedback">Search for the inventory substance to select, view or edit.</span><br><br>
			</td>
		  </tr>
		  <tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td>
			  <%if Session("isCDP") = "TRUE" then%>
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
							<%ShowStrucInputField  dbkey, formgroup, "inv_compounds.Structure", "1",520, 300, "AllOptions", ""%>
						</td>
		      		</tr>
				</table>
				<%end if%>
			
			</td>
			<td valign="top">
				<table border="0">
					<tr>
			        	<td>
							<span title="(eg. Acetonitrile)">Substance Name:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.Substance_Name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(eg. 50-50-0)">CAS Registry#:</span><br>
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
							<span title="(ChemInv compound idenfier)">Compound ID:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.Compound_ID", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="Restrict the search to find only duplicates compounds">Compound Type:<br>
							<select name="inv_compounds.Conflicting_Fields">
								<option value size="1"><%=RepeatString(43, "&nbsp;")%>
								<option value="*">Duplicate Compounds 
							</select>
						</td>
		      		</tr>
		    	</table>
			</td>
			<td valign="top">
				<table border="0">
					<%
					'Set alt_IDDict = Session("Alt_IDDict")
					For each key in alt_ids_dict
					    strLabel = alt_ids_dict.Item(Key) & ":"   ' WJC Millennium
					    bCustom = (left(strLabel, 1) = "*")
					    if bCustom then strLabel = mid(strLabel,2)
						Response.Write "<TR><td>" & vblf
						if bCustom then
						    Dim Conn
						    response.Write ShowCustomPicklist(Key)
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
		
		<input Type="Image" Src="/cheminv/graphics/pixel.gif" id="Image1" name="Image1">
		<input type=hidden name="inv_compounds.reg_id_fk" value=" (inv_compounds.reg_id_fk is null) " />
		<input type=hidden name="inv_compounds.batch_number_fk" value="  (inv_compounds.batch_number_fk is null) " />
		
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
	<script language="JavaScript">
	    document.cows_input_form.onsubmit = function() { 
	        if (window.document.activeElement.className.indexOf("cdd-dialog-input") < 0) {
	            doSearch("search","<%=substanceFG%>"); 
	        }
	        return false;
	    }
	</script>

	</body>
</html>
