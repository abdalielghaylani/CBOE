<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("appkey") & "/logged_out.asp"
end if%>
<html>

<head>
	<title>Search/Refine Form</title>
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<LINK REL='stylesheet' TYPE='text/css' HREF='/<%=Application("appkey")%>/styles.css'>
</head>

<body <%=Application("BODY_BACKGROUND")%>>
<%

'write out help steps if using wizard
'Response.Write "a" & session("wizard") & "a"
wizardStep = session("wizard")
Select Case lCase( wizardStep )
	case "wiz_add_deg_1"%>
		<P>Adding a Degradation compound.
		<br>There are 3 steps needed to add a degradant
		<ol>
			<li class=highlightcopy>Select Parent compound
				<ul>
					<li>You may use the form below to search for a parent compound
					<li>You may add a new parent compound. <a class="highlightlink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=AddParent_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=parent_full_commit&amp;record_added=false" target="_top">GO ></a>
				</ul>
			<li>Select Experiment
			<li>Add Degradant
		</ol>
	<%case "wiz_add_exp_1"%>
	<P>Adding an Experiment.
		<ol>
			<li class=highlightcopy>Select Parent compound
				<ul>
					<li>You may use the form below to search for a parent compound
					<li>You may add a new parent compound. <a class="highlightlink" href="/<%=Application( "appkey" )%>/default.asp?formgroup=AddParent_form_group&amp;dbname=<%=dbkey%>&amp;dataaction=parent_full_commit&amp;record_added=false" target="_top">GO ></a>
				</ul>
			<li >Add Experiment
		</ol>
	<%case "wiz_edit_par_1"%>
	<P>Edit a Parent Compound.
		<ol>
			<li class=highlightcopy>Select Parent compound
				<ul>
					<li>Use the form below to search for a parent compound
				</ul>
		</ol>
	<%case "wiz_edit_exp_1"%>
	<P>Editing an Experiment.
		<ol>
			<li class=highlightcopy>Select Parent compound
				<ul>
					<li>You may use the form below to search for a parent compound
				</ul>
			<li >Select Experiment
		</ol>
	<%case "wiz_edit_deg_1"%>
	<P>Editing a Degradation compound.
		<br>There are 3 steps needed to edit a degradant
		<ol>
			<li class=highlightcopy>Select Parent compound
				<ul>
					<li>You may use the form below to search for a parent compound
				</ul>
			<li>Select Experiment
			<li>Select Degradant
		</ol>
	<%case else
		session("wizard") = ""
end select
%>
 
<table  border = "3">
<tr>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
</tr>
</table>


<input type = "hidden" name = "order_by" value = "D3_PARENTS.GENERIC_NAME">
<input type = "hidden" name = "sort_direction" value = "ASC">
<%
' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' Make and fill a session variable to be used for getting back to this display
' with the proper querystring parameter settings.
Session( "ReturnToD3Search" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' Set variables for the height and width of the structure fields.
' This makes it easier to change them (no searching around in the code).
heightStructArea = 340
widthStructArea = 340
on error resume next
Set connBase = GetNewConnection( dbkey, formgroup, "base_connection" )
%>
<!-- ==================== -
<br>Session( "LoginRequired" & dbkey ) = "<%=Session( "LoginRequired" & dbkey )%>"
<br>Session( "UserValidated" & dbkey ) = "<%=Session( "UserValidated" & dbkey )%>"
 ==================== -->
<table  border = "0">
	<!--
	I have related things vertically close: molecular weight is above formula.  If I just
	put molecular weight in the first cells of a row and the formula in the first cells of
	the next row, tabbing from molecular weight will take you to the _other thing in its row_
	before going to the formula.  I want tabbing to go down one column, then down the next.
	To do this I am putting all the elements of a column (well, really a title/field column
	pair) in a table and all the elements of the other column set in another column.
	(Yes, I could use TABINDEX _if I were making the form myself_.  Since COWS is making
	the input items, I have to resort to this method.)
	-->
	<tr>
		<td>
			<!-- The table for the left column set. -->
			
			<table  border = "0"  bordercolor = "brown">

				<tr>
					<td  valign = "top"  align = "right">
						<strong>Generic name</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_PARENTS.GENERIC_NAME", 0, "24"%>
					</td>	
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Trade name</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_PARENTS.TRADE_NAME", 0, "24"%>
					</td>	
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Common/Other name</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_PARENTS.COMMON_NAME", 0, "24"%>
					</td>	
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Compound ID</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_PARENTS.COMPOUND_NUMBER", 0, "24"%>
					</td>	
				</tr>
				<tr>
					<td valign=top align=right>
						<strong>Functional Group</strong>
					</td>
					<td valign= "top">
						<%

						all_fgroups_list = ListAllFGroupsOrdered(dbkey, formgroup, connBase, "Deg_FGroup_Key", "Deg_FGroup_text", "D3_FGroups.Deg_FGroup_text")
						ShowLookUpList dbkey, formgroup, BaseRS, "D3_DEGSFGROUP.DEG_FGROUP_ID", all_fgroups_list, "", "", 0, true, "value", "0"
						%>
						
						
					</td>
				</tr>
			</table>
		</td>

		<td  valign = "top">
			<!-- The table for the right column set. -->
			<table  border = "0"  bordercolor = "purple">
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Parent Formula</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_BASE64.Formula", 4, "24"%>
					</td>
				</tr>

				<tr>
					<td  valign = "top"  align = "right">
						<strong>Parent Molecular&nbsp;weight</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_BASE64.MolWeight", 0, "24"%>
					</td>
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Change in Molecular&nbsp;weight</strong>
					</td>
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_DEGS.MW_AMT_CHG", 0, "24"%>
					</td>
				</tr>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Salt</strong>
					</td>
<!-- -->
					<td>
<%

						sSaltsList = GetSaltsListForDropDownBox( connBase )
'						Response.Write( "sSaltsList = [ " & sSaltsList & " ]" )
						ShowLookUpList dbkey, formgroup, BaseRS, "D3_PARENTS.SALT", sSaltsList, "", "", 0, true, "value", "0"
						
%>
					</td>
<!-- -->
<!-- -
					<td  valign = "top">
						<%ShowInputField dbkey, formgroup, "D3_PARENTS.SALT", 0, "24"%>
					</td>
-- -->
				</tr>

				<tr>
					<td  valign = "top"  align = "right">
						<strong>Conditions</strong>
					</td>
					<td>
<%
						on error resume next
						
						sCondsList = GetDegCondsList( connBase )
'						Response.Write( "sCondsList = [ " & sCondsList & " ]" )
						ShowLookUpList dbkey, formgroup, BaseRS, "D3_EXPTS.DEG_COND_FK", sCondsList, "", "", 0, true, "value", "0"
						
%>
					</td>
				</tr>
				<% if CBool(Session( "dd_Edit_Records" & dbkey )) = True then %>
				<tr>
					<td  valign = "top"  align = "right">
						<strong>Status</strong>
					</td>
					<td>
<%
						on error resume next
						
						sStatusList = GetStatusList( connBase )
'						Response.Write( "sCondsList = [ " & sCondsList & " ]" )
						ShowLookUpList dbkey, formgroup, BaseRS, "D3_PARENTS.STATUS", sStatusList, "", "", 0, true, "value", "0"
						
%>

					</td>
				</tr>
				<%else %>
				<tr>
					<td colspan=2><input type=hidden name=status value=20001></td>
				</tr>
				<%end if%>
				
			</td>
		</table>
	</tr>

	<tr>
		<!-- The subtable for parent compound structure searches. -->
		<td  valign = "top">
			<table  border = "1">
				<tr>
					<td  align = "center">
						<strong>Parent</strong>
					</td>
				</tr>
				<tr>
					<td>
						<% ShowStrucInputField  dbkey, formgroup, "D3_BASE64.Structure", "5", "340", widthStructArea, "AllOptions", "SelectList" %>
						<% 'ShowStrucInputField  dbkey, formgroup, "D3_PARENTS.Structure", "5", "340", widthStructArea, "AllOptions", "SelectList" %>

						<% 'ShowStrucInputField  dbkey, formgroup, "D3_BASE64.BASE64_CDX", "5", "340", widthStructArea, "AllOptions", "SelectList" %>
					</td>
				</tr>
			</table>
		</td>

		<!-- The subtable for degradant compound structure searches. -->
		<td  valign = "top">
			<table  border = "1">
				<tr>
					<td  align = "center">
						<strong>Degradant</strong>
					</td>
				</tr>
				<tr>
					<td>
						<% 'ShowStrucInputField  dbkey, formgroup, "D3_DEGS.Structure", "6", widthStructArea, heightStructArea, "AllOptions", "SelectList" %>
						<% ShowStrucInputField  dbkey, formgroup, "D3_DEG_BASE64.Structure", "6", widthStructArea, heightStructArea, "AllOptions", "SelectList" %>
					</td>
				</tr>
			</table>
		</td>
	</tr>

</table>
<%
connBase.Close
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
