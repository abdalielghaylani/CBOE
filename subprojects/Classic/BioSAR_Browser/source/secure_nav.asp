
<!--#INCLUDE FILE = "../source/security_flags.asp"-->

<script language="javascript">
//Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
//standard vars - do not edit
var AppKey = "<%=Application("AppKey")%>"
//!DGB! Change to accomodate additional frame inside main frame
	// if there is a frame called mainFrame inside the main it now becomes the
var MainWindow
if (!<%=Application("mainwindow")%>.mainFrame){
	MainWindow = <%=Application("mainwindow")%>
}
else{
	MainWindow = <%=Application("mainwindow")%>.mainFrame
}

var commit_type = "<%=commit_type%>"
var formmode =  "<%=formmode%>"
var formgroupflag = "<%=formgroupflag%>"
var uniqueid = "<%=baseid%>"

//security variables from <%=Application("AppKey")%> & "_privileges_table".
// use these variables to turn buttons on and off in the navbar.
//add those needed based on this applicaitons privilege table fields. 
var Edit_People_Table = "<%=Session("Edit_People_Table" & dbkey)%>"
var Edit_Sites_Table = "<%=Session("Edit_People_Table" & dbkey)%>"

//getCustomButtons allows you to add buttons to the standard cows navbar.
//add calls to standard getButton function in navbar_js.js as shown below:
//outputval = outputval +  getButton("new_search")
// in app_js.js in a similar as below where submit_request would be a function in app_js.js
//outputval = '<a href="javascript:MainWindow.submit_request()">'
//outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
function getCustomButtons(){

	outputval=""
	if(formmode.toLowerCase() == "search"){
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;restore_query&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/restore_query_btn.gif" BORDER="0"></a>'
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;history&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/history_btn.gif" BORDER="0"></a>'
	}
	if(formmode.toLowerCase() == "edit"){
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;save_query&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/save_query_btn.gif" BORDER="0"></a>'
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;restore_query&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/restore_query_btn.gif" BORDER="0"></a>'
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;history&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/history_btn.gif" BORDER="0"></a>'

	}
	if(formmode.toLowerCase() == "list"){
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;save_query&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/save_query_btn.gif" BORDER="0"></a>'
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;restore_query&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/restore_query_btn.gif" BORDER="0"></a>'
		//outputval = outputval + '<a href="javascript:MainWindow.doManageQueries(&quot;history&quot;,MainWindow.dbname, MainWindow.formgroup)"><img SRC="/<%=Application("AppKey")%>/graphics/history_btn.gif" BORDER="0"></a>'
		
	}
	if(formmode.toLowerCase() == "add_record"){
	}
	if(formmode.toLowerCase() == "edit_record"){
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
		outputval = outputval +  getButton("new_search","","/chem_reg/graphics/manage_users_btn.gif")
	}
	if(formmode.toLowerCase() == "add_record"){
		table_name = "<%=Session("table_name")%>"
		
		outputval = outputval + getButton("add_record")
		outputval = outputval + getButton("cancel_edit")
		//outputval = outputval + getButton("clear_form")
		//outputval = outputval + getButton("restore_last")	
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
		outputval = outputval + ' SRC="/<%=Application("AppKey")%>/graphics/update_user_btn.gif" BORDER="0"></a>'
		
		//this code is in admin_utils_js.js.
		outputval = outputval + '<a href="javascript:MainWindow.doDeleteUser(MainWindow.user_name)"><img SRC="/<%=Application("AppKey")%>/graphics/delete_user_btn.gif" BORDER="0"></a>'
		outputval = outputval + '<a href="<%=Session("ListLocation" & dbkey & formgroup)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search","","/chem_reg/graphics/manage_users_btn.gif")
	}
	if(formmode.toLowerCase() == "add_record"){
		
		outputval = '<a href="javascript:MainWindow.submit_request(&quot;add_user&quot;,&quot;user_name:User Name,password:Password,password_confirmed:Password Confirm,roles_hidden:Role&quot;)">'
		outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
		outputval = outputval + '<a href="<%=Session("ListLocation" & dbkey & formgroup)%>" target = "main"><img SRC="/<%=Application("AppKey")%>/graphics/cancel_edit_btn.gif" BORDER="0"></a>'
		
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
		outputval = outputval + ' SRC="/<%=Application("AppKey")%>/graphics/update_role_btn.gif" BORDER="0"></a>'
		//this code is in admin_utils_js
		if (bDELETE_ROLES_TABLE.toLowerCase() == "true"){
			outputval = outputval + '<a href="javascript:MainWindow.doDeleteRole()"><img SRC="/<%=Application("AppKey")%>/graphics/delete_role_btn.gif" BORDER="0"></a>'
		}
		outputval = outputval + getButton("cancel_edit")
	}
	if(formmode.toLowerCase() == "list"){
		outputval = outputval +  getButton("new_search","","/chem_reg/graphics/manage_users_btn.gif")
	}
	if(formmode.toLowerCase() == "add_record"){
		//this code is in admin_utils_js
		outputval = '<a href="javascript:MainWindow.doAddRole(MainWindow.role_name)">'
		outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_new_role_btn.gif" BORDER="0"></a>'
		outputval = outputval + getButton("cancel_edit")
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
	
</script> 


