<script language="javascript">
//standard vars - do not edit
var AppKey = "<%=Application("AppKey")%>"
MainWindow = <%=Application("mainwindow")%>
var commit_type = "<%=commit_type%>"
var formmode =  "<%=formmode%>"
var formgroupflag = "<%=formgroupflag%>"
var uniqueid = "<%=baseid%>"

//security variables from <%=Application("AppKey")%> & "_privileges_table".
// use these variables to turn buttons on and off in the navbar.
//add those needed based on this applicaitons privilege table fields. 
var Edit_People_Table = "<%=Session("Edit_People_Table" & dbkey)%>"
var Edit_Sites_Table = "<%=Session("Edit_People_Table" & dbkey)%>"
var Submit_Docs = "<%=Session("Submit_Docs" & dbkey)%>"
var BatchLoad_Docs = "<%=Session("BatchLoad_Docs" & dbkey)%>"
var View_History = "<%=Session("View_History" & dbkey)%>"

//getCustomButtons allows you to add buttons to the standard cows navbar.
//add calls to standard getButton function in navbar_js.js as shown below:
//outputval = outputval +  getButton("new_search")
// in app_js.js in a similar as below where submit_request would be a function in app_js.js
//outputval = '<a href="javascript:MainWindow.submit_request()">'
//outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
/*function getCustomButtons(){
	outputval=""
	if(formmode.toLowerCase() == "search"){
	}
	if(formmode.toLowerCase() == "edit"){
	}
	if(formmode.toLowerCase() == "list"){
	}
	if(formmode.toLowerCase() == "add_record"){
	}
	if(formmode.toLowerCase() == "edit_record"){
	}
	return outputval
}*/


//standard for cs_security - do not edit unless you are adding tables that you will manage in this applicaiton

function  getRegManageTablesButtons(){
	outputval = ""
  //display approrpriate buttons depending on security flags
	if(formmode.toLowerCase() == "search"){
	}
	if(formmode.toLowerCase() == "edit"){
		outputval = outputval +  getButton("new_search")
		outputval = outputval +  getButton("edit_search")
	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval + getButton("new_search", "", "/docmanager/graphics/manage_users_btn.gif")
	}
	if(formmode.toLowerCase() == "add_record"){
		table_name = "<%=Session("table_name")%>"
		
		outputval = outputval + getButton("add_record")
		//outputval = outputval + getButton("cancel_edit")
		outputval = outputval + '<a href="<%=Session("ListLocation" & dbkey)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
		//outputval = outputval + getButton("clear_form")
		//outputval = outputval + getButton("restore_last")	
	}
	
	if(formmode.toLowerCase() == "edit_record"){
		table_name = "<%=Session("table_name")%>"
		if ((table_name.toLowerCase() == "people") && (Edit_People_Table.toLowerCase()=="true")){
			outputval = outputval + getButton("update_record")
			//outputval = outputval + getButton("cancel_edit")
			outputval = outputval + '<a href="<%=Session("ListLocation" & dbkey & formgroup)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
			outputval = outputval + getButton("delete_record")
		}
		if ((table_name.toLowerCase() == "sites") && (Edit_Sites_Table.toLowerCase()=="true")){
			outputval = outputval + getButton("update_record")
			outputval = outputval + getButton("cancel_edit")
			outputval = outputval + getButton("delete_record")
		}
			
	}

return outputval

}

//standard for cs_security - do not edit
function  getRegManageUsersButtons(){
	outputval = ""
  //display appropriate buttons depending on security flags
	if(formmode.toLowerCase() == "search"){
	}
	if(formmode.toLowerCase() == "edit_record"){
		outputval = '<a href="javascript:MainWindow.doUpdateUser(MainWindow.orig_current_list, MainWindow.orig_user_password, MainWindow.user_name)"><img'
		outputval = outputval + ' SRC="/<%=Application("AppKey")%>/graphics/update_user_btn.gif" BORDER="0"></a>'
		
		//this code is in admin_utils_js.js.
		outputval = outputval + '<a href="javascript:MainWindow.doDeleteUser(MainWindow.user_name)"><img SRC="/<%=Application("AppKey")%>/graphics/delete_user_btn.gif" BORDER="0"></a>'
		outputval = outputval + '<a href="<%=Session("listlocation" & dbkey & formgroup)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
	}
	if (formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search", "", "/docmanager/graphics/manage_users_btn.gif")
	}
	if (formmode.toLowerCase() == "add_record"){
		
		outputval = '<a href="javascript:MainWindow.submit_request(&quot;add_user&quot;,&quot;user_name:User Name,password:Password,password_confirmed:Password Confirm,roles_hidden:Role&quot;)">'
		outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
		outputval = outputval + '<a href="<%=Session("listlocation" & dbkey & formgroup)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
	
	}
	//alert(outputval);
return outputval

}
//standard for cs_security - do not edit
function  getRegManageRolesButtons(){
	outputval = ""
  //display approrpriate buttons depending on security flags
	if(formmode.toLowerCase() == "search"){
	}
	if(formmode.toLowerCase() == "edit_record"){
		outputval = '<a href="javascript:MainWindow.doUpdateRole()"><img'
		outputval = outputval + ' SRC="/<%=Application("AppKey")%>/graphics/update_role_btn.gif" BORDER="0"></a>'
		//this code is in admin_utils_js
		//outputval = outputval + '<a href="javascript:MainWindow.doDeleteRole()"><img SRC="/<%=Application("AppKey")%>/graphics/delete_role_btn.gif" BORDER="0"></a>'
		outputval = outputval + getButton("cancel_edit")
	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search", "", "/docmanager/graphics/manage_users_btn.gif")
	}
	if(formmode.toLowerCase() == "add_record"){
		//this code is in admin_utils_js
		outputval = '<a href="javascript:MainWindow.doAddRole(MainWindow.role_name)">'
		outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_new_role_btn.gif" BORDER="0"></a>'
		outputval = outputval + getButton("cancel_edit")
	}
	//alert(outputval);
return outputval


}

function getCustomButtons(excludeButtonList){
	var outputval = ""
	
	if (!excludeButtonList){
		excludeButtonList = ""
	}
	else{	
		excludeButtonList = excludeButtonList.toLowerCase()
	}
	
	if (formmode.toLowerCase() == "search"){
		if (excludeButtonList.indexOf("search") == -1) outputval = outputval + getButton("search");
		if (excludeButtonList.indexOf("clear_form") == -1) outputval = outputval + getButton("clear_form");
		//if (excludeButtonList.indexOf("restore_last") == -1) outputval = outputval + getButton("restore_last")
		if (excludeButtonList.indexOf("retrieve_all") == -1) outputval = outputval + getButton("retrieve_all");
	}	
	
	if (formmode.toLowerCase() == "refine"){			
		if (excludeButtonList.indexOf("apply") == -1) outputval = outputval + getButton("apply");
		if (excludeButtonList.indexOf("clear_form") == -1) outputval = outputval + getButton("clear_form");
		if (excludeButtonList.indexOf("new_search") == -1) outputval = outputval + getButton("new_search");
	}
	
	if (formmode.toLowerCase() == "list"){
		outputval = outputval + getButton("new_search");
		
		
		if (refine_mode== ""){
			if (excludeButtonList.indexOf("edit_query") == -1) outputval = outputval + getButton("edit_query");
		}

		if ((fEmptyRecordset != "True")||(get_next_count > 0)){
			if (excludeButtonList.indexOf("refine") == -1) outputval = outputval + getButton("refine"); 
			/*if((formgroupflag != "REG_TEMP")&&(formgroupflag != "REG_COMMIT")){
				if (excludeButtonList.indexOf("mark_all_records") == -1) outputval = outputval + getButton("mark_all_records");
			}*/
		}

		if (refine_mode== "refine"){		
			if (excludeButtonList.indexOf("undo_refine") == -1) outputval = outputval + getButton("undo_refine");
		}
		
		outputval = ReplaceSubString(outputval, "getAction", "getAction_Docmanager", 0)
				
		/*if((db_type != "RXN") && (formgroupflag !="REG_COMMIT") && (formgroupflag !="REG_TEMP")&& (fEmptyRecordset == "False")){
			if (excludeButtonList.indexOf("export_hits") == -1) outputval = outputval + getButton("export_hits");
		}*/	
	}
	
	if (formmode.toLowerCase() == "no_nav_view") {
		if (excludeButtonList.indexOf("return_to_details") == -1) outputval = outputval + getButton("return_to_details");		
	}
	
	if (formmode.toLowerCase() == "edit") {	
		if (excludeButtonList.indexOf("edit_query") == -1) outputval = outputval + getButton("new_search");
		if (refine_mode== ""){
			if (excludeButtonList.indexOf("edit_query") == -1) outputval = outputval + getButton("edit_query");
		}
		
		if ((fEmptyRecordset != "True")||(get_next_count > 0)) {
			if (excludeButtonList.indexOf("refine") == -1) outputval = outputval + getButton("refine");
		
			/*if((formgroupflag != "REG_TEMP")&&(formgroupflag != "REG_COMMIT")){
				if (excludeButtonList.indexOf("mark_all_records") == -1) outputval = outputval + getButton("mark_all_records");
			}*/
		}
		if (refine_mode != ""){		
			
			if (excludeButtonList.indexOf("undo_refine") == -1) outputval = outputval + getButton("undo_refine");
		
		}
		/*if((db_type != "RXN") && (formgroupflag !="REG_COMMIT") && (formgroupflag !="REG_TEMP")&& (fEmptyRecordset == "False")){
			if (excludeButtonList.indexOf("export_hits") == -1) outputval = outputval + getButton("export_hits");
		}*/
		
		if(formgroupflag =="ADD_RECORD"){
			if (excludeButtonList.indexOf("update_record") == -1) outputval = outputval + getButton("update_record");
			if (excludeButtonList.indexOf("delete_record") == -1) outputval = outputval + getButton("delete_record");
		}		
	}
	
	if (formmode.toLowerCase() == "edit_record") {
		if (excludeButtonList.indexOf("update_record") == -1) outputval = outputval + getButton("update_record");
		if (excludeButtonList.indexOf("cancel_edit") == -1) outputval = outputval + getButton("cancel_edit");
		if (excludeButtonList.indexOf("delete_record") == -1) outputval = outputval + getButton("delete_record");
	}
	
	if (formmode.toLowerCase() == "add_record"){
		if (excludeButtonList.indexOf("add_record") == -1) outputval = outputval + getButton("add_record");
		if (excludeButtonList.indexOf("cancel_edit") == -1) outputval = outputval + getButton("cancel_edit");
		if (excludeButtonList.indexOf("clear_form") == -1) outputval = outputval + getButton("clear_form");
		if (excludeButtonList.indexOf("restore_last") == -1) outputval = outputval + getButton("restore_last");
	}
	
	if (formmode.toLowerCase() == "add_compounds"){
		if(formgroupflag=="ADD_RECORD"){
			if (excludeButtonList.indexOf("update_record") == -1) outputval = outputval + getButton("update_record");
			if (excludeButtonList.indexOf("cancel_edit") == -1) outputval = outputval + getButton("cancel_edit");
			if (excludeButtonList.indexOf("clear_form") == -1) outputval = outputval + getButton("clear_form");
			if (excludeButtonList.indexOf("restore_last") == -1) outputval = outputval + getButton("restore_last");
		}
	}	
return outputval
}

function  getRegManagePasswordsButtons(){
	var outputval = ""
  //display approrpriate buttons depending on security flags
	if(formmode.toLowerCase() == "search"){
	}
	if(formmode.toLowerCase() == "edit_record"){
		outputval = '<a href="javascript:MainWindow.doUpdateUserPwd(MainWindow.orig_user_password, MainWindow.user_name)"><img'
		outputval = outputval + ' SRC="/<%=Application("AppKey")%>/graphics/update_user_btn.gif" BORDER="0"></a>'

		//outputval = outputval + getButton("cancel_edit")

	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search")
	}
	if(formmode.toLowerCase() == "add_record"){
		
		outputval = '<a href="javascript:MainWindow.submit_request(&quot;add_user&quot;,&quot;user_name:User Name,password:Password,password_confirmed:Password Confirm,roles_hidden:Role&quot;)">'
		outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
		outputval = outputval + '<a href="<%=Session("ListLocation" & dbkey & formgroup)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
   		
   		//outputval = outputval + getButton("cancel_edit")

	}
	
return outputval
}

//Added to fix CSBR-41876 SYAN 3/12/2004
//This replaces all the occurrances of 'find' in a string.	
function ReplaceSubString(s, find, replace, start) {
	var matchStart;
	var firstHalf;
	var secondHalf;
	var replaceEnd;
	var result;
	
	result = s;
	replaceEnd = 0;
	
	matchStart = result.indexOf(find, replaceEnd);
	
	while (matchStart >= replaceEnd) {
		
		firstHalf = result.slice(0, matchStart);
		secondHalf = result.slice(matchStart + find.length, result.length);
		result = firstHalf.concat(replace, secondHalf);
		replaceEnd = matchStart + replace.length;
		
		matchStart = result.indexOf(find, replaceEnd);
	
	}
	
	return result;		
}

//To fix CSBR-41876 SYAN 3/12/2004
//This function is called when "New Query", "Edit Query", "Refine" buttons are clicked, overriding the core getAction function in navbar_js.js.
//This is only necessary when download is clicked on list view. The next button you click right after download will
//throw error if use core functions. It is caused by redirecting to download.asp hence lose track of the main frame info.
//Esentially MainWindow.document.cows_input_form is replaced with MainWindow.cows_input_form.
//This is not a good way of fixing it but the changes in core would be risky at this point.
//This does not fix all the buttons when clicked immediately after download. To fix all, more functions need to be moved
//to app level, which I am reluctant to do. Hopefully the rest of it is not significant enough to be release-stopping.
function getAction_Docmanager(action, formgroup_override){
	
	var actiontemp = getActionTemp(action)
	var dbname = getDBName(action)
	
	if (action == "edit_query"){
		MainWindow.cows_input_form.action = actiontemp + "?formgroup=" + formgroup + "&dataaction=" + action + "&dbname=" + dbname+ "&dbsearchnames=" + dbsearchnames + "&refine_mode=" + refine_mode
		MainWindow.cows_input_form.submit();
	}
		
	if (action == "refine"){
		if (no_refine_dialog!=true){
			MainWindow.cows_input_form.action = actiontemp + "?formgroup=" + formgroup + "&dataaction=" + action + "&dbname=" + dbname+ "&dbsearchnames=" + dbsearchnames
			var w = ""
			if (w.name == null){
			var w = window.open(refine_path + "?dbname=" + dbname + "&totalrecords=" + MainWindow.base_records_found,"refine_options","width=400,height=200,scrollbars=no,status=no,resizable=no");
			w.focus()}
			else{
			w.focus()}
		}
		else{
			MainWindow.cows_input_form.action = actiontemp + "?formgroup=" + formgroup + "&dataaction=" + action + "&dbname=" + dbname+ "&dbsearchnames=" + dbsearchnames
			MainWindow.cows_input_form.RefineType.value = "partial_refine"
			MainWindow.cows_input_form.submit();
		}
	}
	
	if (action == "undo_refine"){
		MainWindow.cows_input_form.action = actiontemp + "?formgroup=" + formgroup + "&dataaction=" + action + "&dbname=" + dbname+ "&dbsearchnames=" + dbsearchnames
		MainWindow.cows_input_form.submit();
	}
	if (action == "new_search"){
		MainWindow.cows_input_form.action = actiontemp + "?formgroup=" + formgroup + "&dataaction=" + action + "&dbname=" + dbname+ "&dbsearchnames=" + dbsearchnames
		MainWindow.cows_input_form.submit();
	}
	
	return true
}
</script> 


