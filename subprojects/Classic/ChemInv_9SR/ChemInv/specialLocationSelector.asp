<%@ LANGUAGE="VBScript"%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
Dim RS1


if request("Postback") = "1" then
	rc= WriteUserProperty(Session("UserNameCheminv"), "invSpecialLocs" , Request("SpecialLocationList"))
	Response.Write "<SCRIPT LANGUAGE=javascript>window.close()</SCRIPT>"
	Response.end
else
	sql = "SELECT " & Application("CHEMINV_USERNAME") & ".GetLocationPath(Location_id) AS LocationPath, Location_ID FROM inv_locations WHERE location_id IN (" & Application("SPECIAL_LOCATIONS") & ") ORDER BY location_id ASC"
	'Response.Write sql
	'Response.end
	Call GetInvCommand(sql, 1)
	Set RS1 = Cmd.Execute
	invSpecialLocs = GetUserProperty(Session("UserNameCheminv"), "invSpecialLocs")
end if
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
	<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
	<SCRIPT LANGUAGE=javascript src="/cheminv/utils.js"></SCRIPT>
	<title>Special Inventory Locations</title>
	<SCRIPT LANGUAGE="javascript">
		focus();
		
		function SaveList(){
			var str = "";
			for (i=0; i < document.form1.elements.length ; i++){
			if (document.form1.elements[i].checked){
				if (str.length > 0) str = str + ",";
				str = str + document.form1.elements[i].value ;
				}
			}
			document.form1.SpecialLocationList.value = str;
			if (opener.cows_input_form.SpecialLocationList){
			 opener.cows_input_form.SpecialLocationList.value = str;
			 opener.SetLocationSQL(opener.cows_input_form.tempLocation_ID.value)
			}
			document.form1.submit();	
		}
		
		function CheckFromList(List){
			if (List.length > 0){
				var arr_temp = List.split(",");
				for (i=0;i< arr_temp.length; i++){
					//alert(arr_temp[i]);
					document.form1["specialLocations_" + arr_temp[i]].checked = true;
				}
			}		
		}
	</SCRIPT>	
</head>

<body bgcolor="#FFFFFF" onload="CheckFromList('<%=invSpecialLocs%>');">
<center>
<form name="form1" method="POST">
<table border=0 bgcolor=#FFFFFF>
	<tr>
		<th align=left nowrap>Select the locations to exclude from search:</th>
	</tr>
	<tr>
		<td><BR><BR></td>
	</tr>
	<% 
	if NOT (RS1.EOF AND RS1.BOF) then
		While Not RS1.EOF
		ID = RS1("Location_ID")
		lp = RS1("LocationPath")
		Response.Write  "<tr><td><input type=""checkbox"" name=""specialLocations_" & ID & """ value=""" & ID & """>" & Left(lp, len(lp)-1) & "</td></tr>"
		RS1.MoveNext
		Wend
	else
		Response.write "<tr><td><span class=""GUIFeedback"">No special locations found</span></td></tr>"
	end if
	%>
	<tr>
		<td  align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="SaveList(); return false"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>
<input type="hidden" name="SpecialLocationList">
<input type="hidden" name="PostBack" value="1">
</form>
</center>

</body>
</html>




