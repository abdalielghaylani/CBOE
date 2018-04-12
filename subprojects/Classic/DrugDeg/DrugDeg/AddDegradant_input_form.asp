<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved%>
<%if session("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<title>Add degradant compound</title>
<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<%
record_added=Request.QueryString("record_added")
commit_type = "full_commit"
formmode = Request("formmode")


' Set up a session variable for this as the current page displayed.
Session( "ReturnToCurrentPage" & dbkey ) = Session( "CurrentLocation" & dbkey & formgroup )

' start add_record information additions to input page

' Change this to "Duplicate_Search_No_Add" for duplicate search where duplicate structures are
' not added or "Duplicate_Search_Add" if you want to check for duplicates but still add them.
' The duplicate id's in either case are stored in Session("duplicates_found" & dbkey & formgroup)
' which you can look at and report after response is returned.
add_record_action = "Duplicate_Search_Add"

'   Add comma delimited list of tables (or single table if only one).  Table addition will
' cascade based on this if more than one table is entered.  The links between tables used
' are those defined for the table in the ini file.
'LJB 11/17 we are only adding to drugdeg_degs via the core addrecord code. The addition to drugdegs_base64 is done in a fashion that avoids the need to alias the table in the drugdegs.ini file.
add_order = "DRUGDEG_DEGS"

'   If you want to override or append the default return location (which for this form is this
' form) then add information to this field. Session( "CurrentLocation" & dbkey & formgroup ) is
' the standard return location. You can append a "&myfield=myvalue" to this to have it returned
' in the querystring.
return_location_overrride = Session( "ReturnToExperimentDetails" & dbkey )

' Set variables for the height and width of the structure field.
' This makes it easier to change them (no searching around in the code).
heightStructArea = 400
widthStructArea = 400

' Get the key for the experiment for which we are adding a degradant.
keyExpt = Request.QueryString( "keyexpt" )
if not keyExpt <> "" then
	keyExpt = Request("DRUGDEG_DEGS.DEG_EXPT_FK")
end if
Set connBase = GetNewConnection( dbkey, formgroup, "base_connection" )
%>
<script language="javascript">
	<!-- Hide from older browsers
	MainWindow.commit_type = "<%=commit_type%>"
	// End Script hiding -->
</script>

<body  <%=Application("BODY_BACKGROUND")%>>

<%'add some helper text for the wizard
wizardStep = session("wizard")

select case lcase(wizardStep)
	case "wiz_add_deg_2"
	session("wizard") = "wiz_add_deg_3"
	%>
	<P>Adding a Degradation compound.
	<ol>
		<li>Select Parent compound
				
		<li>Add Experiment
				
		<li class=highlightcopy>Add Degradant
			<ul>
				<li>Input the Degradant information
			</ul>
	</ol>
	<P><A class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/Experiment_details_form.asp?dbname=drugdeg&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=keyExpt%>">Return to Experiment Details</a>
	<%case "wiz_add_deg_3"
		session("wizard") = "wiz_add_deg_4"%>
	<p>You have added a new degradation product. What would you like to do now?
	<p><span class="heading">Add Another Degradant to this experiment</span>
		<br>You may use the form below to add another degradant to this experiment.
	<P><A class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/Experiment_details_form.asp?dbname=drugdeg&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=keyExpt%>">Return to Experiment Details</a>
	<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
		<br>Return here if you would like to add something else
			
	<%case "wiz_add_exp_2"
	'they came from the final add expirement page
		session("wizard") = "wiz_add_deg_3"
		%>
		<P>Adding a Degradation compound.
		<ol>
			<li>Select Parent compound
					
			<li>Add Experiment
					
			<li class=highlightcopy>Add Degradant
				<ul>
					<li>Input the Degradant information
				</ul>
		</ol>
		<P><A class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/Experiment_details_form.asp?dbname=drugdeg&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=keyExpt%>">Return to Experiment Details</a>
	<%case "wiz_add_exp_3"
		session("wizard") = "wiz_add_deg_3" %>
		Your expirement has been added. What would you like to do now?
		<p><span class="heading">Add a Degradant to this experiment</span>
			<br>You may use the form below to add a degradant to this experiment.
		<P><A class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/Experiment_details_form.asp?dbname=drugdeg&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=keyExpt%>">Return to Experiment Details</a>
		<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
			<br>Return to the main page if you would like to add something else
	<%case "wiz_add_par_2"
		session("wizard") = "wiz_add_deg_3" %>
		<p><span class="heading">Add a Degradant to this experiment</span>
			<br>You may use the form below to add a degradant to this experiment.
		<P><A class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/Experiment_details_form.asp?dbname=drugdeg&formgroup=AddExperiment_form_group&formmode=edit&formgroupflag_override=Experiment_details&keyexpt=<%=keyExpt%>">Return to Experiment Details</a>
		<P><a class="headinglink" href="/<%=Application( "appkey" )%>/drugdeg/mainpage.asp?dbname=<%=dbkey%>" target="_top">Return to main page</a>
			<br>Return to the main page if you would like to add something else
<%end select%>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<input  type = "hidden"  name = "add_order"  value ="<%=add_order%>">
<input  type = "hidden"  name = "add_record_action"  value = "<%=add_record_action%>">	
<input  type = "hidden"  name = "commit_type"  value = "<%=commit_type%>">	
<input  type = "hidden"  name = "return_location_overrride"  value = "<%=return_location_overrride%>">	
<input  type = "hidden"  name = "DRUGDEG_BASE64.BASE64_CDX"  value = "">
<input  type = "hidden"  name = "DRUGDEG_DEGS.DEG_EXPT_FK"  value = "<%=keyExpt%>">
<input type=hidden name="parent_rgroup" value="<%=Request("parent_rgroup")%>">



<%if record_added = "true" then%>
<script language="javascript">
		alert("Your record was added to the temporary table")
</script>
<%end if%>
<%
all_fgroups_list = ListAllFGroupsOrdered(dbkey, formgroup, connBase, "Deg_FGroup_Key", "Deg_FGroup_text", "DRUGDEG_FGroups.Deg_FGroup_text")

%>
<script language="javascript">
	w_fgroup_list = "<%=all_fgroups_list%>"
	w_current_fgroup_list = ""
</script>

<table  border = 0>
	<tr>
		<td  valign = "top">
			<% 'ShowStrucInputField  dbkey, formgroup, "DRUGDEG_DEGS.Structure", "5", widthStructArea, heightStructArea, "Exact", "SelectList" %>
			<% ShowStrucInputField  dbkey, formgroup, "DRUGDEG_BASE64.Structure", "5", widthStructArea, heightStructArea, "Exact", "SelectList" %>
		</td>


		<td  valign = "top">
			
			<table  border = 0  color = "red">

				<tr>
					<td  valign = "top">
						<strong>Compound Number</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<% ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.COMPOUND_NUMBER", 0, "30" %>
					</td>
				</tr>
				<tr>
					<td  height = 20>
					</td>
				</tr>			
				<tr>
					<td  valign = "top">
						<strong>Name</strong>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<% ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.ALIAS", 0, "30" %>
					</td>
				</tr>
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<strong>Molecular weight Change</strong><br>
						<font size="-1">Please enter a molecular weight change<br>
						if the parent or degradant contain an<br>
						R-Group. If no number is entered, we will<br>
						calculate the change.</font>
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<table>
							<tr>
								<!-- <td valign=middle>
								<%
									'on error resume next
									
									'sMWDirectionList = "1:+,2:-"
									
									'ShowLookUpList dbkey, formgroup, BaseRS, "DRUGDEG_DEGS.MW_DIR_CHG", sMWDirectionList, "", "", 0, false, "value", "1"
									
								%>
								</td> -->
								<td valign=middle><% ShowInputField dbkey, formgroup, "DRUGDEG_DEGS.MW_AMT_CHG", 0, "30" %></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td  height = 20>
					</td>
				</tr>
				<%if all_fgroups_list <> "" then%>
				<tr>
					<td  valign = "top">
						<strong>Functional group(s) of parent involved<br>in degradation reaction</strong><br>
						(to add multiple groups, hold down<br>
						the control key when selecting)
					</td>
				</tr>
				<tr>
					<td  valign = "top">
						<select name="fgroups" width = "220" size = "6" ID="fgroups" onchange="updateHiddenFGroup()" multiple>
						</select>
						<input type = "hidden" value = "" name = "current_fgroups_hidden" ID="current_fgroups_hidden">
					</td>
				</tr>
				<%else%>
				<input type = "hidden" value = "" name = "current_fgroups_hidden" ID="current_fgroups_hidden">
				<%end if%>
			</table>
		</td>
	</tr>
</table>
<script>
<%if all_fgroups_list <> "" then%>
//window.onload = function(){loadSelect()}
//	function loadSelect(){
		fill_deg_fgroup_list();

//	}
<%end if%>
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
