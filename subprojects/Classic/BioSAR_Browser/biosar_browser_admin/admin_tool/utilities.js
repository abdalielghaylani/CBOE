<script language="javascript">


function add_child_table_list(id){
	var childtablelist= MainWindow.document.forms["admin_form"].elements["child_table_list"].value
	if (childtablelist.length>0){
		childtablelist = childtablelist + "," + id
	}else{
		childtablelist=id
	}
	MainWindow.document.forms["admin_form"].elements["child_table_list"].value=childtablelist

}



function fill_lists(){
	
	
	if (MainWindow.document.forms["security_form"]){
		if (MainWindow.document.forms["security_form"].elements["can_make_public"]){		
			fillAvailList()
			fillCurrentList()
			updateHidden()
		}
	}
}

function updateHidden(){
	var can_make_public = MainWindow.document.forms["security_form"].elements["can_make_public"].value
		if (can_make_public==1){
			theavailroles = MainWindow.document.forms["admin_form"].elements["roles_hidden"]
			thecurrentroles = MainWindow.document.forms["admin_form"].elements["current_roles_hidden"]
	
			ispublic = MainWindow.document.forms["security_form"].elements["is_public"]
			if (thecurrentroles.value == "") {
				ispublic.value = ""
			} else {
				ispublic.value = "true"
			}
		}
}

function fillAvailList(){

	var can_make_public = MainWindow.document.forms["security_form"].elements["can_make_public"].value
	if (can_make_public==1){
		temp = w_avail_list.split(",")
	
		MainWindow.document.forms["security_form"].elements["roles"].length = 0
		for (i=0;i<temp.length;i++){
			MainWindow.document.forms["security_form"].elements["roles"].options[i] = new Option(temp[i])
		}
		MainWindow.document.forms["admin_form"].elements["roles_hidden"].value = w_avail_list

	}
}

function fillCurrentList(){
	var can_make_public = MainWindow.document.forms["security_form"].elements["can_make_public"].value
	if (can_make_public==1){
	temp = w_current_list.split(",")
	
	MainWindow.document.forms["security_form"].elements["current_roles"].length = 0
	for (i=0;i<temp.length;i++){
		MainWindow.document.forms["security_form"].elements["current_roles"].options[i] = new Option(temp[i])
	}
	MainWindow.document.forms["admin_form"].elements["current_roles_hidden"].value = w_current_list
	
	}
}





function removeFromList(theItem, theList){
	//242
	var thenewlist= ""
	if (theList.length > 0){
		temp = theList.split(",")
		for (i=0;i<temp.length;i++){
			if (temp[i].toLowerCase() != theItem.toLowerCase()){
				if (thenewlist.length >0){
					thenewlist = thenewlist + "," + temp[i]
				}
				else{
				
					thenewlist = temp[i]
				}
			}
		}
	}
	else{
		thenewlist = ""
	}
return thenewlist
}

function addToList(theItem, theList){
	var thenewlist= ""
	var ok_to_add = true
	
	if (theList.length >0 ){
		temp = theList.split(",")
		for (i=0;i<temp.length;i++){
			if (temp[i].toLowerCase() == theItem.toLowerCase()){
				ok_to_add = false
			}
		}
		if(ok_to_add == true){
			//SYAN commented the condition checking out
			//if (thenewlist.length == 0){
			//		thenewlist = theItem
			//}
			//else{
				thenewlist = theList + "," + theItem
			//}
			
		}
		else{
			thenewlist = ""
		}
	}
	else{
		thenewlist = theItem
	}

	return thenewlist
}



function doFormAction(action){


	var elm = MainWindow.document.forms[0].elements["form_id"]
	
	var form_id = "&formgroup_id=" + elm.options[elm.selectedIndex].value
	var action_path
	var action_string
	var base_path
	if (action == "restore"){
		action_path="/biosar_browser/biosar_browser/biosar_browser_action.asp?dataaction=db&dbname=biosar_browser&bypass_ini=true&formgroup=" + elm.options[elm.selectedIndex].value 
		override_submit = false
	}
	if (action=="edit"){
		base_path="/biosar_browser/biosar_browser_admin/admin_tool/user_tables.asp?dbname=biosar_browser&formgroup=base_form_group"
		action_string = "&action=" + action
		action_path = base_path + action_string + form_id
		override_submit = false
	}
	if (action=="duplicate_form"){
		base_path="/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group"
		action_string = "&action=" + action
		action_path = base_path + action_string + form_id
		override_submit = false
	}
	if (action=="do_rename"){
		var form_info = escape(elm.options[elm.selectedIndex].text)
		base_path="/biosar_browser/biosar_browser_admin/admin_tool/user_rename_form.asp?dbname=biosar_browser&formgroup=base_form_group&form_info=" + form_info
		action_string = "&action=" + action
		action_path = base_path + action_string + form_id
		override_submit = false
	}

	if (action=="delete_form"){
		base_path="/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group"
		if (confirm("You are about to permanently delete this form.  Deletion cannot be undone.  Continue?")){
			override_submit = false
		}else{
			override_submit = true
		}
		action_string = "&action=really_delete_form"
		action_path = base_path + action_string + form_id
		
	}
	
	if (action=="refresh_warning"){
		base_path="/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group"
		action_string = "&action=" + action
		action_path = base_path + action_string + form_id
		override_submit = false
	}
	
	if (action=="refresh"){
		base_path="/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group"
		action_string = "&action=" + action
		action_path = base_path + action_string + form_id
		override_submit = false
	}
	
	if(override_submit == false){
	
		MainWindow.document.forms[0].action = action_path
		MainWindow.document.forms[0].target = "main"
		MainWindow.document.forms[0].submit()
	}
	
}

function doSubmitAndRestore(){
	//setIntegrationCookie("bioviz")
	//setIntegrationCookie("excel")
	MainWindow.document.admin_form.submit()

}

function doSubmitAndRestore2(){
	//setIntegrationCookie("bioviz")
	//setIntegrationCookie("excel")
	doUpdateTemplates()

}

function doSwitchSchema(){
	var newownerkey = MainWindow.document.admin_form.schema_selector.options[MainWindow.document.admin_form.schema_selector.selectedIndex].value;
	var currownerkey = "<%=request("user_name")%>";
	var currentURL = MainWindow.location.href
	var newURL=""
	if (currentURL.indexOf("user_name") != -1) {
		 newURL = MainWindow.document.location.href.replace("&user_name=" + currownerkey + "&" ,"&user_name=" + newownerkey + "&")
	}
	else{
		 newURL = MainWindow.location.href.replace("&dbname=", "&user_name=" + newownerkey + "&dbname=" )
	}
	MainWindow.document.location.href = newURL;
}
function doSwitchLookupSchema(){
	var newownerkey = MainWindow.document.admin_form.schema_selector.options[MainWindow.document.admin_form.schema_selector.selectedIndex].value;
	var currownerkey = "<%=request("lookup_owner")%>";
	var currentURL = MainWindow.location.href
	var newURL=""
	if (currentURL.indexOf("lookup_owner") != -1) {
		 newURL = MainWindow.document.location.href.replace("&lookup_owner=" + currownerkey + "&" ,"&lookup_owner=" + newownerkey + "&")
	}
	else{
		 newURL = MainWindow.location.href.replace("&dbname=", "&lookup_owner=" + newownerkey + "&dbname=" )
	}
	MainWindow.document.location.href = newURL;
}

function doSwitchBaseTableSchema(){
	var newownerkey = MainWindow.document.admin_form.schema_selector.options[MainWindow.document.admin_form.schema_selector.selectedIndex].value;
	var currownerkey = "<%=request("lookup_owner")%>";
	var currentURL = MainWindow.location.href
	var newURL=""
	
	if (currentURL.indexOf("lookup_owner") != -1) {
		 newURL = MainWindow.document.location.href.replace("&lookup_owner=" + currownerkey + "&" ,"&lookup_owner=" + newownerkey + "&")
	}
	else{
		 newURL = MainWindow.location.href.replace("&formgroup=", "&lookup_owner=" + newownerkey + "&formgroup=" )
	}
	
	MainWindow.document.location.href = newURL;
}



function doSwitchSchemaTables(){
	var newtablekey = MainWindow.document.admin_form.table_selector.options[document.admin_form.table_selector.selectedIndex].value;
	var currtablekey = "<%=request("table_id")%>";
	
	var currentURL = MainWindow.location.href
	var newURL=""
	if (currentURL.indexOf("table_id") != -1) {
		 newURL = MainWindow.document.location.href.replace("&table_id=" + currtablekey + "&" ,"&table_id=" + newtablekey + "&")
	}
	else{
		 newURL = MainWindow.location.href.replace("&dbname=", "&table_id=" + newtablekey + "&dbname=" )
	}

	MainWindow.document.location.href = newURL;
}

function doSwitchSchemaLookupTables(){
	var newtablekey = MainWindow.document.admin_form.table_selector.options[document.admin_form.table_selector.selectedIndex].value;
	var currtablekey = "<%=request("lookup_table")%>";
	
	var currentURL = MainWindow.location.href
	var newURL=""
	if (currentURL.indexOf("lookup_owner") == -1) {
		var newownerkey = MainWindow.document.admin_form.schema_selector.options[MainWindow.document.admin_form.schema_selector.selectedIndex].value;
		newURL = MainWindow.document.location.href.replace("&dbname=", "&lookup_owner=" + newownerkey + "&dbname=" )
	}else{
		newURL = currentURL
	}
	if (newURL.indexOf("lookup_table") != -1) {
		 newURL = newURL.replace("&lookup_table=" + currtablekey + "&" ,"&lookup_table=" + newtablekey + "&")
	}
	else{
		 newURL = newURL.replace("&lookup_owner=", "&lookup_table=" + newtablekey + "&lookup_owner=" )
	}
	
	MainWindow.document.location.href = newURL;
}

function doSwitchSchemaBaseTableTables(){
	var newtablekey = MainWindow.document.admin_form.table_selector.options[document.admin_form.table_selector.selectedIndex].value;
	var currtablekey = "<%=request("lookup_table")%>";
	
	var currentURL = MainWindow.location.href
	var newURL=""
	if (currentURL.indexOf("lookup_owner") == -1) {
		var newownerkey = MainWindow.document.admin_form.schema_selector.options[MainWindow.document.admin_form.schema_selector.selectedIndex].value;
		newURL = MainWindow.document.location.href.replace("&dbname=", "&lookup_owner=" + newownerkey + "&dbname=" )
	}else{
		newURL = currentURL
	}
	if (newURL.indexOf("lookup_table") != -1) {
		 newURL = newURL.replace("&lookup_table=" + currtablekey + "&" ,"&lookup_table=" + newtablekey + "&")
	}
	else{
		 newURL = newURL.replace("&lookup_owner=", "&lookup_table=" + newtablekey + "&lookup_owner=" )
	}
	
	MainWindow.document.location.href = newURL;
}

function doGoToTable(owner_name,table_id){
	var action_path = "/biosar_browser/biosar_browser_admin/admin_tool/admin_table_view.asp?formgroup=base_form_group&user_name=" + ownerkey + "&dbname=biosar_browser&table_id=" + tablekey
	MainWindow.document.admin_form.action=action_path
	MainWindow.document.admin_form.submit()
}

function doSwitchFormGroupID(dup_only_form_groups){
	var newfgkey = MainWindow.document.forms[0].form_id.options[MainWindow.document.forms[0].form_id.selectedIndex].value;
	var currfgkey = "<%=request("formgroup_id")%>";
	var currform_restrict = "<%=request("form_restrict")%>";
	var curraction = "<%=request("action")%>";
	
	if (newfgkey == 0){
		MainWindow.document.location.href = "/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&formmode=admin";
		return;
	} ;
	
	var newURL = ""
	//var dup_only_form_groups = "<%=dup_only_form_groups%>"
	var bDupOnly=false
	var currentURL = MainWindow.location.href
	
	if (currentURL.indexOf("formgroup_id") != -1){
	
		newURL = currentURL.replace("&formgroup_id=" + currfgkey ,"&formgroup_id=" + newfgkey)
	
	}else{
		if (currentURL.indexOf("&formgroup") != -1){
			newURL = currentURL.replace("&formgroup=", "&formgroup_id=" + newfgkey + "&formgroup=" )
		}else{
			if (newURL.indexOf("?formgroup=") != -1){
					newURL = currentURL.replace("?formgroup=", "?formgroup_id=" + newfgkey + "&formgroup=" )
			}
			else{
				newURL = currentURL.replace("&dbname=", "&formgroup_id=" + newfgkey + "&dbname=" )
			}
		}
	}
	
	if (dup_only_form_groups.length > 0){
	
		restrict_array = dup_only_form_groups.split(",")
		for (i=0;i<restrict_array.length;i++){
			if(newfgkey == restrict_array[i]){
			
				bDupOnly = true
				
			}
		}
	}
	if (bDupOnly == true){
		if (newURL.indexOf("form_restrict") != -1){
		
			newURL = newURL.replace("&form_restrict=" + currform_restrict ,"&form_restrict=" + "dup_only" )
		}
		else
		{	if (newURL.indexOf("&formgroup=") != -1){
				newURL = newURL.replace("&formgroup=", "&form_restrict=" + "dup_only" + "&formgroup=" )
			}else{
				if (newURL.indexOf("?formgroup=") != -1){
					newURL = newURL.replace("?formgroup=", "?form_restrict=" + "dup_only" + "&formgroup=" )
				}
			}
		}
		
	}
	else{
		if (newURL.indexOf("form_restrict") != -1){
			if (newURL.indexOf("&form_restrict=") != -1){
				newURL = newURL.replace("&form_restrict=" + currform_restrict , "" )
			}else{
				if (newURL.indexOf("?form_restrict=") != -1){
						newURL = newURL.replace("?form_restrict=" + currform_restrict ,"?" )
						newURL = newURL.replace("?&", "?")
				}
			}
		}
	}
	if (newURL.indexOf("action") != -1){
		newURL = newURL.replace("&action=" + curraction + "&" , "&")
		newURL = newURL.replace("&action=" + curraction , "")
	}
	
	MainWindow.document.location.href = newURL;
	
}

function doUpdateRoles(){
	var can_make_public = MainWindow.document.forms["admin_form"].elements["can_make_public"].value
	if (can_make_public==1){
		setIntegrationCookie("bioviz")
		setIntegrationCookie("excel")
		MainWindow.document.forms["admin_form"].current_roles_hidden.value = w_current_list
		MainWindow.document.forms["admin_form"].submit()
	}else{
		
		MainWindow.doUpdateTemplates()
	}
	

}
function doUpdateTemplates(){
	setTempIntegrationCookie("bioviz")
	setTempIntegrationCookie("excel")
	MainWindow.document.forms["template_form"].submit()

}

//jhs 4/23/2007 adding so that you can trim off if it is only blanks
//fixes CSBR-78871
function trimAll(sString) {
	while (sString.substring(0,1) == ' ')	{
		sString = sString.substring(1, sString.length);
	}
	while (sString.substring(sString.length-1, sString.length) == ' '){
		sString = sString.substring(0,sString.length-1);
	}
	return sString;
}

function checkLength(theFieldName, message){
	var bSubmit
	bSubmit = false
	var theField = MainWindow.document.admin_form.elements[theFieldName]
	
	if (trimAll(theField.value) !=""){
		bSubmit=true
	}
	
	if(bSubmit == false){
		alert("Please enter a " + message + ".")
	}
	return bSubmit
}

function checkNameDescPass(){
	var bSubmit
	var bSubmit2
	bSubmit = checkLength("new_desc", "description")
	bSubmit2 = checkLength("new_password","password")
	if (bSubmit==true && bSubmit2==true){
		MainWindow.document.admin_form.submit()
	}
}

function checkNameDescRename(){
	var bSubmit
	var bSubmit2
	bSubmit = checkLength("new_name","name")
	bSubmit2 = checkLength("new_desc", "description")
	if (bSubmit==true && bSubmit2==true){
		MainWindow.document.admin_form.submit()
	}
}
function checkNameDesc(){
	var bSubmit
	var bSubmit2
	bSubmit = checkLength("formgroup_name","name")
	bSubmit2 = checkLength("formgroup_description", "description")
	if (bSubmit==true && bSubmit2==true){
		MainWindow.document.admin_form.submit()
	}
}

function ShowHideDiv(divId)
{
   var menuObj = "div" + divId;
   var imgObj = "img" + divId;
   if (document.all.item(menuObj).className == "SHOW") {
      document.all.item(menuObj).className = "HIDE";
      document.all.item(imgObj).src = "right.gif"
   }
   else {
      document.all.item(menuObj).className = "SHOW";
      document.all.item(imgObj).src = "down2.gif"
   }
}
function basetableWarning(){
		if (confirm("If you change the base table you will lose all assoiciated child table information.  Continue?")){
			MainWindow.document.location.href="user_choose_base_table.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=request("formgroup_id")%>&curr_id=<%=lBaseTableid%>"		
		}else{
		//return false
		}

}


function noFieldsWarning(){

		if (confirm("You have not selected any fields to display on the form <%=removeIllegalChars(request("new_name"))%>. Edit the form now?")){
			MainWindow.document.location.href="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=request("formgroup_id")%>"
		}else{
		return false
		}

}

function noBaseTableFieldsWarning(){
		if (alert("Please select at least one field for display in the base table")){
			MainWindow.document.location.href="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=request("formgroup_id")%>"
		}else{
		MainWindow.document.location.href="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=request("formgroup_id")%>"
		}

}

function confirmDeleteForm(){
		if (confirm("You are about to permanently delete this form.  Deletion cannot be undone. Continue?")){
			MainWindow.document.location.href="user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&action=really_delete_form&formgroup_id=<%=request("formgroup_id")%>"
		}else{
		return false
		}

}

function formErrors(){
alert("Errors occured while creating this form. Please delete form and recreate.")


}

function schemaErrors(){
alert("Permissions on some selected tables could not be granted to the specified user(s). Check to make sure that the schema password(s) is/are correct in Schema Management for schema(s) <%=sSchemaErrs%>")
}


function buildLeftMultiSelectBox(Name, thesize){
	document.write ('<select multiple size = "' + thesize + '" name="' + Name + '" onChange="checkAddValues()">')
	document.write ('</select>')
	//document.write ('<input type="hidden" value=" " name="' + Name + '_hidden">')
	document.write ('<input type="hidden" value=" " name="items_to_add">')
}

function buildRightMultiSelectBox(Name, thesize){
	document.write ('<select multiple size = "' + thesize + '" name="' + Name + '" onChange="checkRemoveValues()">')
	document.write ('</select>')
	//document.write ('<input type="hidden" value="<%' +'=user_roles_list' + '%>" name="' + Name + '_hidden">')
	document.write ('<input type="hidden" value=" " name="items_to_remove">')
}

function checkRemoveValues(){
	removeItemtextobj= MainWindow.document.forms["security_form"].elements["current_roles"].options
	var removeItemtext = ""
	 	for (i=0;i<removeItemtextobj.length;i++){
			if(removeItemtextobj.options[i].selected){
				//if (removeItemtextobj.options[i].text.indexOf("NESTED") == -1){
					Itemselected =true
					if (removeItemtext.length>0){
						removeItemtext=removeItemtext + "," + removeItemtextobj[i].text
					}else{
						removeItemtext=removeItemtextobj[i].text
					}
				//}
			}
		}
		MainWindow.document.forms["security_form"].elements["items_to_remove"].value=removeItemtext

}

function checkAddValues(){
	addItemtextobj= MainWindow.document.forms["security_form"].elements["roles"].options
	var addItemtext = ""
		for (i=0;i<addItemtextobj.length;i++){
			if(addItemtextobj.options[i].selected){
				Itemselected =true
				if(addItemtext.length>0){
					addItemtext=addItemtext + "," + addItemtextobj[i].text
				}else{
					addItemtext=addItemtextobj[i].text
				}
			}
			
		}
		MainWindow.document.forms["security_form"].elements["items_to_add"].value=addItemtext
		
		
}

function addCurrentList(){
	var Itemselected = false
	var addItemtextobj = MainWindow.document.forms["security_form"].elements["items_to_add"].value
	if (addItemtextobj.length>0){
		Itemselected =true
		
		var addItemtext_array =addItemtextobj.split(",")
			for (q=0;q<addItemtext_array.length;q++){
				var addItemtext=addItemtext_array[q]
				w_avail_list=removeFromList(addItemtext, w_avail_list)
				w_current_list=addToList(addItemtext, w_current_list)			
			}
	}
	if (Itemselected == false){
		alert("Please make a selection in the Current Roles and Users list")
	}else{
		MainWindow.document.forms["admin_form"].elements["roles_hidden"].value = w_avail_list
		MainWindow.document.forms["admin_form"].elements["current_roles_hidden"].value = w_current_list
		//setCookie("current_roles_hidden", w_current_list, 2)
		//setCookie("roles_hidden", w_avail_list, 2)
		fill_lists()
		MainWindow.document.forms["security_form"].elements["items_to_add"].value=""
	}
}

function removeCurrentList(){
	var Itemselected = false
	var removeItemtextobj = MainWindow.document.forms["security_form"].elements["items_to_remove"].value
		if (removeItemtextobj.length>0){
			Itemselected =true
			removeItemtext_array =removeItemtextobj.split(",")
			for (p=0;p<removeItemtext_array.length;p++){
				var removeItemtext=removeItemtext_array[p]
				if (removeItemtext.indexOf("NESTED") ==-1){
					w_avail_list=addToList(removeItemtext, w_avail_list)
					w_current_list=removeFromList(removeItemtext, w_current_list)	
				}else{
				alert("Nested privileges must be removed form the role that has the direct grant. If this role is outside of biosar then it cannot be administered through this interface.")
					}	
			}
		}
		if (Itemselected == false){
			alert("Please make a selection in the Available Roles and Users list")
		}else{
			MainWindow.document.forms["admin_form"].elements["roles_hidden"].value = w_avail_list
			MainWindow.document.forms["admin_form"].elements["current_roles_hidden"].value = w_current_list
			//setCookie("current_roles_hidden", w_current_list, 2)
			//setCookie("roles_hidden", w_avail_list, 2)
			fill_lists()
			MainWindow.document.forms["security_form"].elements["items_to_remove"].value=""
		}
	
}

function helpMsg(topic){
	if (topic=="enable_excel"){
		alert("Turns on the Display in Excel button in list and detail view. Clicking the button sends the result set to your client MS Excel application. ChemDraw for Excel version 14.x must be installed on your machine. To display the structures once in Excel,  select the structure column and choose Show Picture in the ChemDraw for Excel toolbar.")
	}
	if (topic=="enable_bioviz"){
		alert("Turns on the Display in BioViz button in list and detail view. Clicking the button sends the result set to your client CS ChemFinder BioViz application.  This feature requires that you have ChemFinder version 14 or later installed on your machine.")
	}
	if (topic=="format_mask"){
		alert("Use a valid Oracle number or date format mask.\rFor example:\rNumber with 2 decimals:999999D99\rEuropean date format: DD/MM/YYYY\r\rSee the Administrator Guide for more details.")
	}
	if (topic=="hyperlink"){
		alert("There are 3 main combinations allowed:\rURL and Link Text blank: Use if the full URL is stored in the database.\r\rURL supplied and Link Text left blank:  This will display the value of the column as a link to the url you have supplied.\r\r example URLs:\r http://mywebsite.com/mypage.asp?regid=<REGDB.VW_REGISTRYNUMBER.REGID> \r\r/biosar_browser/default.asp?dataaction=search&dbname=biosar_browser&metadata_directive=blind_gui&formgroup=1020&REGDB.REG_NUMBERS.REG_ID==<REGDB.VW_REGISTRYNUMBER.REGID>&output_type=FRAMES\r\r**Note** You can specify data from the row to be passed by putting the full field name between <>.  In the above example the form is based on a the view vw_registrynumber, so to pass the row value for regid you specify the full field name REGDB.VW_REGISTRYNUMBER.REGID \r\rURL and Link Text supplied: This will give you a link that will have the supplied text and go to the URL.\r example Link Text: Link To BioSAR")
	}	
}
function EchoItemAddedWithRefreshForm(nodeID, treeItemID){
	alert ('Item added');
	var formgroup ="<%=request("formgroup_id")%>"
	tabPost(3,formgroup)
}

function EchoItemMovedWithRefreshForm(nodeID, treeItemID){
	alert ('Item moved');
	var formgroup ="<%=request("formgroup_id")%>"
	tabPost(3,formgroup)
	
}


function EchoItemRemovedWithRefreshForm(ItemID){
	alert ('Item removed');
	var formgroup ="<%=request("formgroup_id")%>"
	tabPost(3,formgroup)
	
}

function EchoItemAddedWithRefreshTable(nodeID, treeItemID){
	alert ('Item added');
	var table ="<%=request("table_id")%>"
	tabPostTable(2,table)
}

function EchoItemMovedWithRefreshTable(nodeID, treeItemID){
	alert ('Item moved');
	var table ="<%=request("table_id")%>"
	tabPostTable(2,table)
	
}


function EchoItemRemovedWithRefreshTable(ItemID){
	alert ('Item removed');
	var table ="<%=request("table_id")%>"
	tabPostTable(2,table)
	
}
function setIntegrationCookie(sType){
	if (document.forms["integration_form"]){
		if (sType=="excel"){
	
			if (document.forms['integration_form'].elements['enable_excel_cb'].checked ==  true){
				document.forms['admin_form'].elements['enable_excel'].value  = '1'
				
				
			}
			if (document.forms['integration_form'].elements['enable_excel_cb'].checked ==  false){
				document.forms['admin_form'].elements['enable_excel'].value = '0'
				
				
			}
		}
		if (sType=="bioviz"){
			if (document.forms['integration_form'].elements['enable_bioviz_cb'].checked==  true){
				document.forms['admin_form'].elements['enable_bioviz'].value = '1'
				
			}
			if (document.forms['integration_form'].elements['enable_bioviz_cb'].checked == false){
				document.forms['admin_form'].elements['enable_bioviz'].value = '0'
				
			}
		}
	}
	
}
function setTempIntegrationCookie(sType){
	if (MainWindow.document.forms["integration_form"]){

		if (sType=="excel"){
	
			document.forms['template_form'].elements['enable_excel'].value  = document.forms['admin_form'].elements['enable_excel'].value 

		}
		if (sType=="bioviz"){
			document.forms['template_form'].elements['enable_bioviz'].value  = document.forms['admin_form'].elements['enable_bioviz'].value 
			
		}
	}
	
}

function doOrganizeForms(){
	var manageFormsHelp = "Select a form category or form and choose an action from the menu that appears."
	OpenDialog3('/biosar_browser/source/treeview.asp?dbname=biosar_browser&qshelp='+ manageFormsHelp + '&TreeID=2&TreeTypeID=<%=Session("PRIVATE_CATEGORY_TREE_TYPE_ID")%>&ItemTypeID=2&ClearNodes=1&TreeMode=manage_tree&ShowItems=1&qsrelay=ShowItems,qshelp','MySelectItemDiag',3)
	
}

function doOrganizePublicForms(){
	var manageFormsHelp = "Select a public form category or form and choose an action from the menu that appears."
	OpenDialog3('/biosar_browser/source/treeview.asp?dbname=biosar_browser&qshelp='+ manageFormsHelp + '&TreeID=3&TreeTypeID=2&ItemTypeID=2&ClearNodes=1&TreeMode=manage_tree&ShowItems=1&qsrelay=ShowItems,qshelp','MySelectItemDiag',3)
}
function doOrganizeProjects(){
	var manageProjectsHelp = "Select a project or table and choose an action from the menu that appears."
	OpenDialog3('/biosar_browser/source/treeview.asp?dbname=biosar_browser&qshelp='+ manageProjectsHelp + '&TreeID=4&TreeTypeID=1&ItemTypeID=1&ClearNodes=1&TreeMode=manage_tree&ShowItems=1&qsrelay=ShowItems,qshelp','MySelectItemDiag',3)
}

function tableChange(){
	//set cookies holding all column changes so far
	var tableChanges = getCookie("tableChanges")
	var displayNameField = "table_display_name"
	var displayNameEntry =document.forms["table_form"].elements[displayNameField].value
	var descriptionField = "table_description"
	var descriptionEntry = document.forms["table_form"].elements[descriptionField].value
	var fullEntry = displayNameEntry + "::CS::" +  descriptionEntry
	setCookie("TableChanges", fullEntry, 2)
	document.forms["admin_form"].elements[displayNameField].value=displayNameEntry
	document.forms["admin_form"].elements[descriptionField].value=descriptionEntry
}		


function resetChanges(){
	setCookie("columnChanges", "", 2)
	setCookie("TableChanges", "", 2)
}
		
function columnChange( colId){
	// DGB This cookie gets too big and blows up.
	// Changed fix CSBR-62449
	// do not save to cookie
	
	var columnChanges = getCookie("columnChanges")
	if ((columnChanges != "")&& (columnChanges != null)){
		if (!isDup(columnChanges, colId)){
			columnChanges = columnChanges + "," + colId
		}
	}else{
		columnChanges = colId
	}
	
	// DGB This cookie gets too big and blows up.
	// Do not save the cookie to fix CSBR-62449
	//setCookie("columnChanges", columnChanges, 2)
	
	//get all changeable values relavant to this column_id
	var colIDEntry = colId
	var primaryKeyField = "primary_key"
	
	if (document.forms["table_form"].elements[primaryKeyField]){
		if ((document.forms["table_form"].elements[primaryKeyField].length == 'undefined')||(document.forms["table_form"].elements[primaryKeyField].length == null)){
			var primaryKeyEntry = document.forms["table_form"].elements[primaryKeyField].checked
			
			}else{
				for (i=0;i<document.forms["table_form"].elements[primaryKeyField].length;i++){
					if(document.forms["table_form"].elements[primaryKeyField][i].value == colId){
							var primaryKeyEntry = document.forms["table_form"].elements[primaryKeyField][i].checked
							
					}
				}
			}
	}
	else{
		var primaryKeyEntry = false;
	}	
	
	if (primaryKeyEntry==true){
		var newPrimaryKeyEntry=colId + "Y"
	}else{
		var newPrimaryKeyEntry= colId + "N"
	}
	var isVisibleField = "column_is_visible" + colId
	
	var isVisibleEntry = document.forms["table_form"].elements[isVisibleField].checked
	
	if (isVisibleEntry==true){
		isVisibleEntry="Y"
	}else{
		isVisibleEntry="N"
	}
	var columnOrderField = "column_order" + colId
	var columnOrderEntry = document.forms["table_form"].elements[columnOrderField].value
	var columnDisplayNameField = "column_display_name" + colId
	var columnDisplayNameEntry = document.forms["table_form"].elements[columnDisplayNameField].value
	var columnDescriptionField = "column_description" + colId
	var columnDescriptionEntry = document.forms["table_form"].elements[columnDescriptionField].value
	var fullEntry = colIDEntry + "::CS::" + primaryKeyEntry + "::CS::" + isVisibleEntry + "::CS::" +  columnOrderEntry + "::CS::" +  columnDisplayNameEntry + "::CS::" +  columnDescriptionEntry + "::CS::" 
	
	
	// DGB This cookie gets too big and blows up.
	// Do not save the cookie to fix CSBR-62449
	//setCookie("columnChanges", columnChanges, 2)
	//setCookie("columnChanges" + colId, fullEntry, 2)

	//check to see if these field are in the admin_form
	//this is the form that does the post
	primaryKeyField= primaryKeyField + colId
	document.forms["admin_form"].elements[primaryKeyField].value=newPrimaryKeyEntry
	document.forms["admin_form"].elements[isVisibleField].value=isVisibleEntry
	document.forms["admin_form"].elements[columnOrderField].value=columnOrderEntry
	document.forms["admin_form"].elements[columnDisplayNameField].value=columnDisplayNameEntry
	document.forms["admin_form"].elements[columnDescriptionField].value=columnDescriptionEntry
		
}

function isDup(inputStr, newItem){
	//check that is is not already in the string
	var bItemFound= false
	var temp_array = inputStr.split(",")
	for (i=0;i<temp_array.length;i++){
		var temp_item = temp_array[i]
		if(newItem == temp_item){
			bItemFound = true
		}
	}
	if (bItemFound == true){
		return true
	}
	if (bItemFound == true){
		return false
	}	

}

function tabPost(id,formgroup_id){
	MainWindow.document.forms["admin_form"].action="user_tables.asp?TB="+ id + "&output_type=tab&dbname=biosar_browser&formgroup=base_form_group&formmode=admin&formgroup_id=" + formgroup_id
	MainWindow.document.forms["admin_form"].method = "post"
	MainWindow.document.forms["admin_form"].submit()
}
function tabPostTable(id,table_id){
	MainWindow.document.forms["admin_form"].action="admin_table_view.asp?TB="+ id + "&output_type=tab&dbname=biosar_browser&formgroup=base_form_group&formmode=admin&table_id=" + table_id
	MainWindow.document.forms["admin_form"].method = "post"
	MainWindow.document.forms["admin_form"].submit()
}

// This function was added to clear the format mask
// If the context of the format mask is blank then
// the value of the element is not posted and therefore
//  not processes into the DB
// Here we check for empty format mask values and replace
// them with a single blank character which will get posted.
function ClearFormatMask(){
	if (document.forms["admin_form"]){
		var e = document.forms["admin_form"].elements
		for (i=0;i<e.length;i++){
			if (e[i].name.indexOf("formatMask")==0){
				if (e[i].value.length == 0){
					e[i].value=" ";
				}
			}
		}
	}	
}


</script>