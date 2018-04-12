<!-- #INCLUDE FILE="header.asp" -->
<SCRIPT LANGUAGE=javascript>
	function uncheckAll(field){
		if (field){
			for (i = 0; i < field.length; i++){
				field[i].checked = false ;
			}
		}
	}
	
	function checkAll(field){
		if (field){
			for (i = 0; i < field.length; i++){
				field[i].checked = true ;
			}
		}
	}

	window.onload = function(){loadframes()}
	var navbar_add_on = '&page_name=refresh_forms&formgroup_id=&referer_page=';
	function loadframes(){
		
		loadUserInfoFrame()
		//jhs 10/1/2007
		//make it work with new method via session
		//go up a few lines to see session string build
		//loadNavBarFrame('&page_name=refresh_forms&formgroup_id=&referer_page=')
		}
	
	function getCheckedValues(field){
		var o="";
		if (field){
			for (i = 0; i < field.length; i++){
				o += bool2int(field[i].checked) +"," ;
			}
			o = o.substring(0, o.length-1)
			return(o);
		}
	}
	
	function bool2int(bool){
		return bool ? 1: 0;
	}
	
	function doIt(){
		form1.enable_BioViz_list.value = getCheckedValues(form1.enable_BioViz);
		form1.enable_Excel_list.value = getCheckedValues(form1.enable_Excel);
		form1.refresh_list.value = getCheckedValues(form1.refresh);
		form1.submit();
	}
	
</SCRIPT>

<%


'Set oConn = SysConnection

Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")

sql = "select formgroup_id,formgroup_name,user_id, is_public,description,bioviz, excel  from db_formgroup t order by user_id, formgroup_name"
				
Set oRS = uConn.Execute(sql)

if oRS.EOF and oRS.BOF then
	response.write "no forms found"
	Response.End
else
	Response.Write "<form name=form1 method=post action=""user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&action=refresh_form"">" & vbCrLf
	Response.Write "<table>"
	Response.Write "<tr>" & vbCrLf
	Response.write "	<th>Form Name</th>" & vbCrLf 
	Response.write "	<th>Description</th>" & vbCrLf
	Response.write "	<th>Owner</th>" & vbCrLf
	Response.write "	<th>Public</th>" & vbCrLf
	Response.write "	<th>BioViz?&nbsp;" & getCheckAll("form1.enable_BioViz") &  "</th>" & vbCrLf
	Response.write "	<th>Excel?&nbsp;" & getCheckAll("form1.enable_Excel") &  "</th>" & vbCrLf
	Response.write "	<th>Refresh?&nbsp;" & getCheckAll("form1.refresh") &  "</th>" & vbCrLf
	Response.Write "</tr>" & vbCrLf
	do while NOT oRS.EOF
		fgID = oRS("formgroup_id").value
		fgName = oRS("formgroup_name").value
		fgDesc = oRS("description").value
		fgUserID = oRS("user_id").value
		isBioViz = oRS("bioviz").value
		isExcel =  oRS("excel").value
		isPublic = oRS("is_public").value
		fgIDList = fgIDList & fgID & ","
		Response.Write "<tr>" & vbCrLf
		Response.write "	<td>" & fgName & "</td>" & vbCrLf 
		Response.write "	<td>" & fgDesc & "</td>" & vbCrLf
		Response.write "	<td>" & fgUserID & "</td>" & vbCrLf
		Response.write "	<td>" & getYesNo(isPublic) & "</td>" & vbCrLf
		Response.write "	<td>" & getCheckBox("enable_BioViz", isBioViz,Application("BIOVIZ_INTEGRATION")=1) & "</td>" & vbCrLf
		Response.write "	<td>" & getCheckBox("enable_Excel", isExcel, Application("EXCEL_INTEGRATION")=1) & "</td>" & vbCrLf
		Response.write "	<td>" & getCheckBox("refresh", 0,true) & "</td>" & vbCrLf
		Response.Write "</tr>" & vbCrLf
		
		oRS.MoveNext
	loop
	fgIDList = left(fgIDList, len(fgIDList)-1)
	Response.Write "<input type=""hidden"" name=""formgroup_id_list"" value=""" & fgIDList & """>" & vbCrLf
	Response.Write "<input type=""hidden"" name=""enable_BioViz_list"">" & vbCrLf
	Response.Write "<input type=""hidden"" name=""enable_Excel_list"">" & vbCrLf
	Response.Write "<input type=""hidden"" name=""refresh_list"">" & vbCrLf

	Response.Write "</table>"
	Response.Write "</form>"
end if

Function getCheckBox(name, val, enabled)
	dim o
	if val then checked = " checked " 
	if NOT enabled then disabled = " disabled " 
	o= "&nbsp;&nbsp;<input " & disabled & checked &  "name=""" & name & """ type=""checkbox"" value=""" & val & """>"
	getCheckBox = o
End function

Function getYesNo(val)
	dim o
	if val = "1" or val = "Y" or val = "YES" then 
		o = "Yes"
	else
		o = "No"
	end if	 
	getYesNo = o
End function

Function getCheckAll(fieldName)
	Dim o
	
	o = "<a class=MenuLink href=""javascript:void(0)"" onClick=""uncheckAll(" & fieldName & ")""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border=""0""></a></span><span title=""check all fields in this column""><a class=""MenuLink"" href=""javascript:void(0)""  onClick=""checkAll(" & fieldName & ")""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border=""0""></a>"
	getCheckAll = o
End Function



%>
<form name="cancel_form" method="post" action="user_forms.asp?formgroup=base_form_group&formgroup_id=&dbname=biosar_browser&cancel=true">
	<input type="hidden" name="cancel" value>
</form>
</body>
