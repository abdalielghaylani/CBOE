<script language="javascript">
//standard vars - do not edit
var AppKey = "<%=Application("AppKey")%>"

//!DGB! Change to accomodate additional frame inside main frame
// if there is a frame called maiFrame inside main it now becomes the MainWindow
var MainWindow
if (!top.main.mainFrame){
	MainWindow = <%=Application("mainwindow")%>
}
else{
	MainWindow = top.main.mainFrame
}

var commit_type = "<%=commit_type%>"
var formmode =  "<%=formmode%>"
var formgroupflag = "<%=formgroupflag%>"
var uniqueid = "<%=baseid%>"



//getCustomButtons allows you to add buttons to the standard cows navbar.
//add calls to standard getButton function in navbar_js.js as shown below:
//outputval = outputval +  getButton("new_search")
// in app_js.js in a similar as below where submit_request would be a function in app_js.js
//outputval = '<a href="javascript:MainWindow.submit_request()">'
//outputval = outputval + '<img SRC="/<%=Application("AppKey")%>/graphics/add_user_btn.gif" BORDER="0"></a>'
function getCustomButtons(){
	outputval=""
	if ((formmode.toLowerCase() == "search") && (formgroup.toLowerCase() == "substances_form_group")){
		//addition of add compound button to take you to the add screen.
		//outputval = outputval + getButton("new_record", "", "/cheminv/graphics/add_mode.gif")
		outputval = '<a target="_top" href="/cheminv/gui/CreateOrEditSubstance.asp?ManageMode=1">' + '<img SRC="/<%=Application("AppKey")%>/graphics/add_mode.gif" BORDER="0"></a>'
	}
	
	if((formmode.toLowerCase() == "edit") && (formgroup.toLowerCase() == "substances_form_group")){
		//outputval = outputval + getButton("edit_record", "", "/cheminv/graphics/edit_mode.gif")
		outputval = '<a target="_top" href="/cheminv/gui/CreateOrEditSubstance.asp?ManageMode=1&action=edit&CompoundID='+ MainWindow.nav_variables.BaseID.value +'">' + '<img SRC="/<%=Application("AppKey")%>/graphics/edit_mode.gif" BORDER="0"></a>'
		outputval += '<a target="_top" href="/cheminv/gui/DeleteSubstance.asp?ManageMode=1&CompoundID='+ MainWindow.nav_variables.BaseID.value +'">' + '<img SRC="/<%=Application("AppKey")%>/graphics/delete_record_btn.gif" BORDER="0"></a>'
	}
	if((formmode.toLowerCase() == "list")&& (formgroup.toLowerCase() == "substances_form_group")){
		outputval = '<a target="_top" href="/cheminv/gui/CreateOrEditSubstance.asp?ManageMode=1">' + '<img SRC="/<%=Application("AppKey")%>/graphics/add_mode.gif" BORDER="0"></a>'
	}
	if(formmode.toLowerCase() == "add_record"){
	}
	if(formmode.toLowerCase() == "edit_record"){
		outputval = outputval + getButton("update_record")
		outputval = outputval + getButton("delete_record")
		outputval = outputval + getButton("cancel_edit")
	}
	return outputval
}






function getAddRecordButtons(){
var outputval = ""
	outputval = outputval + getButton("add_record")
	outputval = outputval + getButton("clear_form")
	outputval = outputval + getButton("new_search", "substances_form_group", "/<%=Application("AppKey")%>/graphics/search_mode.gif")
return outputval

}

	
</script> 


