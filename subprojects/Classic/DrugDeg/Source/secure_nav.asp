<script language="javascript">
// standard vars - do not edit
var appkey = "<%=Application("appkey")%>"
MainWindow = <%=Application("mainwindow")%>

var commit_type = "<%=commit_type%>"
var formmode = "<%=lcase(formmode)%>"
var formgroupflag = "<%=formgroupflag%>"
var uniqueid = "<%=baseid%>"
var Edit_This_Record = "True"
// security variables from the table, <%=Application( "appkey" )%> & "_privileges".
// CAP 7 Aug 2001: or, security variables from the table named in the PRIV_TABLE_NAME
//     line of cfserver.ini?
// use these variables to turn buttons on and off in the navbar.
// add those needed based on this application's privilege table fields. 
var Edit_People_Table = "<%=Session( "Edit_People_Table" & dbkey )%>"
var Edit_Sites_Table = "<%=Session( "Edit_People_Table" & dbkey )%>"
var Edit_Users_Table = "<%=Session( "Edit_Users_Table" & dbkey )%>"
var Edit_Records = "<%=Session( "dd_Edit_Records" & dbkey )%>"
var Add_Records = "<%=Session( "dd_Add_Records" & dbkey )%>"
var Search_Records = "<%=Session( "dd_Search" & dbkey )%>"
var Delete_Records = "<%=Session( "dd_Delete_Records" & dbkey )%>"


// getCustomButtons allows you to add buttons to the standard cows navbar.
// add calls to standard getButton function in navbar_js.js as shown below:
// outputval = outputval + getButton("new_search")
// in app_js.js in a similar as below where submit_request would be a function in app_js.js
// outputval = '<a href="javascript:MainWindow.submit_request()">'
// outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/add_user_btn.gif"  border = 0></a>'
function getCustomButtons(){
	var outputval=""

	if ( "search" == formmode.toLowerCase() ){
	}
	else if ( "edit" == formmode.toLowerCase() ){
		// Make sure the user is authorized to edit.
		// Edit_This_Record set in Parent_details_form.asp
		if ( "True" == Edit_Records && ("True" == Edit_This_Record || "" == Edit_This_Record) ) {
			outputval = getButton( "edit_record" )
		}
	}
	else if ( "list" == formmode.toLowerCase() ){
	}
	else if ( "add_record" == formmode.toLowerCase() ){
	}
	else if ( "edit_record" == formmode.toLowerCase() ){
	}
	return outputval
}


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
		outputval = outputval +  getButton("new_search")
	}
	if(formmode.toLowerCase() == "add_record"){
		table_name = "<%=Session("table_name")%>"
		outputval = outputval + getButton("add_record")
		outputval = outputval + getButton("cancel_edit")
	}
	if(formmode.toLowerCase() == "edit_record"){
		table_name = "<%=Session("table_name")%>"
		if ((table_name.toLowerCase() == "people") && (Edit_People_Table.toLowerCase()=="true")){
			outputval = outputval + getButton("update_record")
			outputval = outputval + getButton("cancel_edit")
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
		outputval = outputval + ' SRC="/<%=Application("appkey")%>/graphics/update_user_btn.gif"  border = 0></a>'
		
		//this code is in admin_utils_js.js.
		outputval = outputval + '<a href="javascript:MainWindow.doDeleteUser(MainWindow.user_name)"><img SRC="/<%=Application("appkey")%>/graphics/delete_user_btn.gif"  border = 0></a>'
		outputval = outputval + '<a href="<%=Session( "ListLocation" & dbkey & formgroup )%>" target = "main"><img SRC="/<%=Application("appkey")%>/graphics/cancel_edit_btn.gif"  border = 0></a>'
	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search")
	}
	if(formmode.toLowerCase() == "add_record"){
		outputval = '<a href="javascript:MainWindow.submit_request(&quot;add_user&quot;,&quot;user_name:User Name,password:Password,password_confirmed:Password Confirm,roles_hidden:Role&quot;)">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/add_user_btn.gif"  border = 0></a>'
		outputval = outputval + '<a href="<%=Session( "ListLocation" & dbkey & formgroup )%>" target = "main"><img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
	}
	
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
		outputval = outputval + ' SRC="/<%=Application("appkey")%>/graphics/update_role_btn.gif"  border = 0></a>'
		//this code is in admin_utils_js
		//outputval = outputval + '<a href="javascript:MainWindow.doDeleteRole()"><img SRC="/<%=Application("appkey")%>/graphics/delete_role_btn.gif"  border = 0></a>'
		outputval = outputval + getButton("cancel_edit")
	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search")
	}
	if(formmode.toLowerCase() == "add_record"){
		//this code is in admin_utils_js
		outputval = '<a href="javascript:MainWindow.doAddRole(MainWindow.role_name)">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/add_new_role_btn.gif"  border = 0></a>'
		outputval = outputval + getButton("cancel_edit")
	}
	
	return outputval
}

function getAddCompoundsButtons(){
	var outputval = ""

	//outputval = outputval + getButton("add_record")
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'
	var returnLoc = '<%=Session( "ReturnToDetails" & dbkey )%>'
	outputval = outputval +  '<a href="JavaScript:MainWindow.location.replace(&quot;'+returnLoc+'&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
	outputval = outputval + getButton("clear_form")

	return outputval
}


function getExperimentDetailsButtons(){
	var outputval = ""

	if ( "edit_record" == formmode.toLowerCase() ) {
		var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
		outputval =  outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Cancel the edit, changing nothing&quot;; return true;">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
	}
	else {
		var returnLoc = '<%=Session( "ReturnToParentDetails" & dbkey )%>'
		outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Return to the parent of this experiment&quot;; return true;">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_ReturnToParent.gif"  border = 0></a>'
		// Make sure the user is authorized to edit.
		// Edit_This_Record set in Parent_details_form.asp
		if ( "True" == Edit_Records && ("True" == Edit_This_Record || "" == Edit_This_Record) ) {
			outputval = outputval + getButton( "edit_record" )
		}
	}

	return outputval
}

function getDegradantDetailsButtons(){
	var outputval = ""

	if ( "edit_record" == formmode.toLowerCase() ) {
		
		var returnLoc = '<%=Session( "ReturnToDegradantDetails" & dbkey )%>'
		outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Cancel the edit, changing nothing&quot;; return true;">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
	}
	else {
		var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
		outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Return to the experiment which produced this compound&quot;; return true;">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_ReturnToExperiment.gif"  border = 0></a>'
		// Make sure the user is authorized to edit.
		if ( "True" == Edit_Records ) {
			outputval = outputval + getButton( "edit_record" )
		}
	}

	return outputval
}

function getMechanismDetailsButtons(){
	var outputval = ""

	if ( "edit_record" == formmode.toLowerCase() ) {
		var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
		outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Cancel the edit, changing nothing&quot;; return true;">'
//		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_LogOff_big.gif"  border = 0></a>'
	}
	else {
		var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
		outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Return to the experiment view&quot;; return true;">'
		outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_ReturnToExperiment.gif"  border = 0></a>'
		// Make sure the user is authorized to edit.
		if ( "True" == Edit_Records ) {
			outputval = outputval + getButton( "edit_record" )
		}
	}

	return outputval
}

function getAddParentButtons(){
	var outputval = ""

	outputval = '<a href="javascript:MainWindow.add_drugdeg()"  onMouseOver="status=&quot;Add the data to the database&quot;; return true;"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'
	outputval = outputval + getButton( "clear_form" )
	outputval = outputval + '<a href="/<%=Application( "AppKey" )%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top  onMouseOver="status=&quot;Cancel, adding nothing to the database&quot;; return true;"><img src="/<%=Application( "appkey" )%>/graphics/Button_Cancel.gif" border="0"></a>'

	return outputval
}

function getAddExperimentButtons(){
	
	var returnLoc = '<%=Session( "ReturnToParentDetails" & dbkey )%>'
	var outputval = ""

	outputval = '<a href="javascript:MainWindow.add_drugdeg()"  onMouseOver="status=&quot;Add the data to the database&quot;; return true;"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + getButton("clear_form")

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Cancel, adding nothing to the database&quot;; return true;">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	return outputval
}

function getAddDegradantButtons(){
	var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton("add_record")
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + getButton("clear_form")

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)"  onMouseOver="status=&quot;Cancel, adding nothing to the database&quot;; return true;">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	return outputval
}

function getAddMechanismButtons(){
	var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton("add_record")
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;'+returnLoc+'&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	outputval = outputval + getButton("clear_form")

	return outputval
}

function getAddDocLinkButtons(){
	var returnLoc = '<%=Session( "ReturnToParentDetails" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton("add_record")
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;'+returnLoc+'&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	outputval = outputval + getButton("clear_form")

	return outputval
}

function getConditionAdminButtons() {
	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function getModifyConditionTextButtons() {
	var returnLoc = '<%=Session( "ReturnToConditionListAdmin" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_Update.gif"  border = 0></a>'

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function getFunctionalGroupAdminButtons() {
	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function getModifyFunctionalGroupTextButtons() {
	var returnLoc = '<%=Session( "ReturnToFunctionalGroupListAdmin" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_Update.gif"  border = 0></a>'

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function getStatusAdminButtons() {
	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function getModifyStatusTextButtons() {
	var returnLoc = '<%=Session( "ReturnToStatusListAdmin" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_Update.gif"  border = 0></a>'

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function getSaltAdminButtons(){
	var outputval = ""
	//outputval = outputval + getButton("add_record")
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_AddRecord.gif"  border = 0></a>'

	outputval = outputval + getButton("clear_form")

	return outputval
}

function getModifySaltNameButtons() {
	var returnLoc = '<%=Session( "ReturnToSaltListAdmin" & dbkey )%>'

	var outputval = ""
	//outputval = outputval + getButton( "add_record" )
	outputval = '<a href="javascript:MainWindow.add_drugdeg()"><img SRC="/<%=Application("appkey")%>/graphics/Button_Update.gif"  border = 0></a>'

	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'

	outputval = outputval + getButton( "clear_form" )

	return outputval
}

function  getRegManagePasswordsButtons(){
	var outputval = ""

	//display approrpriate buttons depending on security flags
	if(formmode.toLowerCase() == "search"){
	}
	else if(formmode.toLowerCase() == "edit_record"){
		outputval = '<a href="javascript:MainWindow.doUpdateUserPwd(MainWindow.orig_user_password, MainWindow.user_name)"><img'
		outputval = outputval + ' SRC="/<%=Application("AppKey")%>/graphics/update_user_btn.gif" BORDER="0"></a>'
		//outputval = outputval + getButton("cancel_edit")
	}
	else if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search")
	}
	else if(formmode.toLowerCase() == "add_record"){
		outputval = '<a href="javascript:MainWindow.submit_request(&quot;add_user&quot;,&quot;user_name:User Name,password:Password,password_confirmed:Password Confirm,roles_hidden:Role&quot;)">'
		outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
		outputval = outputval + '<a href="<%=Session( "ListLocation" & dbkey & formgroup )%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
   		//outputval = outputval + getButton("cancel_edit")
	}

	return outputval
}

//JHS added this function
function getModifyExperimentButtons(){

	var returnLoc = '<%=Session("ReturnToExperimentDetails" & dbkey)%>'
	var outputval = ""

	outputval = '<a href="javascript:MainWindow.drugdegUpdateRecord()"><img SRC="/<%=Application("appkey")%>/graphics/Button_Update.gif"  border = 0></a>'	
	
	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
	var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=experiment&MadeIn=DDNavbar'
	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete this experiment&quot;; return true;">'
	outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
	//document.write( outputval )

	return outputval
}


//JHS added this function
function getModifyParentButtons(){

	var returnLoc = '<%=Session("ReturnToParentDetails" & dbkey)%>'
	var outputval = ""

	outputval = '<a href="javascript:MainWindow.drugdegUpdateRecord()"><img SRC="/<%=Application("appkey")%>/graphics/Button_Update.gif"  border = 0></a>'	
	
	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'
	outputval = outputval + '<img SRC="/<%=Application("appkey")%>/graphics/Button_Cancel.gif"  border = 0></a>'
	//var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=experiment&MadeIn=DDNavbar'
	//outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete this experiment&quot;; return true;">'
	//outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
	
	
	var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=parent&MadeIn=DDNavbar'
	outputval = outputval + '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete the parent compound&quot;; return true;">'
	outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'


	return outputval
}


</script> 
