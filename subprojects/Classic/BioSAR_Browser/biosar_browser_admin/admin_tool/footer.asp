
<%path = Request.ServerVariables("path_info")
page_name_temp = split(path, "/", -1)
count= UBound(page_name_temp)
page_name = replace(page_name_temp(count), ".asp", "")
formgroup_id =request("formgroup_id") 
return_to_form=request("return_to_form")



if not request("formgroup") = "base_form_group" then
	the_full_referer = Request.ServerVariables("HTTP_REFERER") 
	if inStr(the_full_referer, "?")>0 then
		temp = split(the_full_referer, "?", -1)
		temp2= split(temp(0), "/", -1)
		referer_page = temp2(ubound(temp2))
	else
		temp = split(the_full_referer, "/", -1)
		referer_page =  temp(ubound(temp))
	end if
	Session("return_page" & request("dbname") & request("formgroup") ) = the_full_referer
else
	Session("return_page" & request("dbname") & request("formgroup") ) = ""
end if
%>



<script language = "javascript">
	var page_name = "<%=page_name%>"
	//var navbar_add_on = "";
	if (page_name == "user_tables"){
		//jhs 11/1/2007 - try new method
		navbar_add_on = '&page_name=' + "<%=page_name%>" + "&formgroup_id=" + "<%=formgroup_id%>" + "&referer_page=" + "<%=referer_page%>";
		loadUserInfoFrame()
		//loadNavBarFrame('&page_name=' + "<%=page_name%>" + "&formgroup_id=" + "<%=formgroup_id%>" + "&referer_page=" + "<%=referer_page%>")
	}else{
	navbar_add_on = '&page_name=' + "<%=page_name%>" + "&formgroup_id=" + "<%=formgroup_id%>" + "&referer_page=" + "<%=referer_page%>";	
	window.onload = function(){loadframes()}
		function loadframes(){
			loadUserInfoFrame()
			//loadNavBarFrame('&page_name=' + "<%=page_name%>" + "&formgroup_id=" + "<%=formgroup_id%>" + "&referer_page=" + "<%=referer_page%>")
			}
	}
	

</script>

<form  method=post id=form1 name=cows_input_form>
</form>