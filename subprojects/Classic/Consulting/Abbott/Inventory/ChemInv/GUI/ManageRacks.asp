<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Cmd
Dim Conn
Dim RS
dbkey = "ChemInv"
ServerName = Request.ServerVariables("Server_Name")

BatchField1 = Request("BatchField1")
BatchField2 = Request("BatchField2")
BatchField3 = Request("BatchField3")
ContainerSize = Request("ContainerSize")
OpenPositions = Request("OpenPositions")
RestrictSize = Request("RestrictSize")
Submit = Request("Submit")
numSamples = Request("numSamples")
contSize = Request("contSize")

if not IsEmpty(Application("DEFAULT_SAMPLE_UOM")) then
	UOMAbv = Application("DEFAULT_SAMPLE_UOM")
else
	UOMAbv = Request("UOMAbv")
	if isEmpty(UOMAbv) then UOMAbv = "1=ml" 
end if

'BatchField1 = "AB-005680"
'BatchField2 = "1"
'ContainerSize = 250
arrUOMValue = split(Request("UOM"),"=")
if Request("UOM") <> "" then
	ContainerSizeUOM = arrUOMValue(0)
end if
FormStep = Request("FormStep")

if FormStep = "" then 
	FormStep = 1
else
	FormStep = 2
end if

%>
<html>
<head>

<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: verdana}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: verdana}
</style>
<title><%=Application("appTitle")%> -- Search Inventory Racks</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/refreshGUI.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	
<% if FormStep = "1" then %>
	function Validate() {
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r\r";
		if (document.form1.BatchField1.value.length == 0 && document.form1.BatchField2.value.length == 0 && document.form1.OpenPositions.value.length == 0 && document.form1.ContainerSize.value.length == 0) {
			bWriteError = true;
			errmsg = errmsg + "Please enter at least one search criteria.\r";
		} else {
			if (document.form1.ContainerSize.value.length > 0 && !isNumber(document.form1.ContainerSize.value)){
				bWriteError = true;
				errmsg = errmsg + "- Container Size must be a positive number greater than zero.\r";
			}
			if (document.form1.OpenPositions.value.length > 0 && !isPositiveNumber(document.form1.OpenPositions.value)){
				bWriteError = true;
				errmsg = errmsg + "- Number of open positions must be a positive number greater than zero.\r";
			}
		}
		if (bWriteError){
			alert(errmsg);
		}else{
			document.form1.submit();
		}
	
	}
<% elseif FormStep = "2" then %>
	function addRacks(field) {
		var bChecked = false;
		for (i=0; i<field.length; i++) {
			if (field[i].checked){
				bSetValue = true;
				var tempValue = field[i].value.split(",")
				//alert(tempValue[0] + ":" + tempValue[1]);
				//value="8301" id="Coromega Rack\BKA3\Re-Aliquot Rack\ :: 46 open"
				for (j=0; j < opener.document.form1.SuggestedRackID.length; j++){			
					if (tempValue[0] == opener.document.form1.SuggestedRackID.options[j].value){
						bSetValue = false;
					}
					if (opener.document.form1.SuggestedRackID.options[j].value == "NULL"){
						opener.document.form1.SuggestedRackID.remove(j);
					}
				}
				if (bSetValue){
					addToList(opener.document.form1.SuggestedRackID, tempValue[1], tempValue[0]);
					bSetValue = true;
				}else{
					//alert("The selected, " + tempValue[1] + " already exists");
				}
			}
		}
	
		/*
		var hiddenInputs = "";
		var tmpStr = "";
		var tmpStrArr = "";
		for (k=0; k < opener.document.form1.SuggestedRackID.length; k++){			
			alert(opener.document.form1.SuggestedRackID.options[k].value);
			tmpStr = GetFirstOpenRackPosition(opener.document.form1.SuggestedRackID.options[k].value);
			//tmpStrArr = tmpStr.split("::");
			//alert(tmpStrArr[0] + ":" + tmpStrArr[1]);
			hiddenInputs = hiddenInputs+"<input type=\"text\" name=\"test\" value=\"t" + tmpStr + "\">";
			tmpStr = "";
		}
		changeContent(opener.document.all.hiddenInputs,'');
		changeContent(opener.document.all.hiddenInputs,hiddenInputs);
		<input type="hidden" name="Rack8301Def" value="8302::1">
		*/
	}
<% end if %>	

-->
</script>

</head>
<body>
<form name="form1" action="Manageracks.asp" method="POST">
<input type="hidden" name="FormStep" value="<%=FormStep%>" />

<div style="margin: -25px 0px 0x -5px; padding: 15px 0px 0px 25px;">
	<p>
	<span class="GuiFeedback" style="background-color: #dedede; padding: 5px; width: 100%;">Rack Managment</span><br /><br />

	<a class="MenuLink" <%if FormStep = 1 then Response.Write("style=""color: #666;""") %> href="ManageRacks.asp?BatchField1=<%=BatchField1%>&BatchField2=<%=BatchField2%>&BatchField3=<%=BatchField3%>&ContainerSize=<%=ContainerSize%>&OpenPositions=<%=OpenPositions%>&FormStep=&RestrictSize=<%=RestrictSize%>">Select Criteria</a> | 
	<% if FormStep = 1 then %>
		<span class="MenuLink">Search Results</span><br /><br />
	<% elseif FormStep = 2 then %>
		<a class="MenuLink" style="color: #666;" href="#">Search Results</a>
		<br /><br />
	<% end if %>
	
<% if FormStep = "2" then

'-- Show sample requests
if BatchField1 = "" then BatchField1 = null
if BatchField2 = "" then BatchField2 = null
if BatchField3 = "" then BatchField3 = null
if OpenPositions = "" then OpenPositions = null
if ContainerSize = "" then ContainerSize = null
if RestrictSize = "" then RestrictSize = null
if ContainerSizeUOM = "" then ContainerSizeUOM = null

'Response.Write(Request.Form)

RequestTypeID = 1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.SEARCHRACKS(?,?,?,?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHFIELD1",200, 1, 30, BatchField1)
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHFIELD2",200, 1, 30, BatchField2)
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHFIELD3",200, 1, 30, BatchField3)
Cmd.Parameters.Append Cmd.CreateParameter("P_OPENPOSITIONS",131, 1, 0, OpenPositions)
Cmd.Parameters.Append Cmd.CreateParameter("P_CONTAINERSIZE",131, 1, 0, ContainerSize)
Cmd.Parameters.Append Cmd.CreateParameter("P_RESTRICTSIZE",200, 1, 10, RestrictSize)
Cmd.Parameters.Append Cmd.CreateParameter("P_CONTAINERSIZEUOM",131, 1, 0, ContainerSizeUOM)
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute

if (RS.EOF and RS.BOF) then
	Response.Write ("<span class=""GUIFeedback"" style=""display: block; width: 100%; padding: 5px;"">No Racks matched your search criteria. <a href=""ManageRacks.asp?BatchField1=" & BatchField1 & "&BatchField2=" & BatchField2 & "&BatchField3=" & BatchField3 & "&ContainerSize=" & ContainerSize & "&OpenPositions=" & OpenPositions & "&FormStep=&RestrictSize=" & RestrictSize & """>Select New Criteria</a></span>")
else

	Response.Write("<table cellpadding=""3"" cellspacing=""5"" style=""border-bottom: 1px solid #ccc;"">")
	Response.write("<tr><td colspan=""4"">")
	Response.Write("<strong>Search Criteria:</strong> ")
	if BatchField1 <> "" then Response.Write("A-Code: " & BatchField1 & ", ")
	if BatchField2 <> "" then Response.Write("Lot Number: " & BatchField2 & ", ")
	if OpenPositions <> "" then Response.Write("Open Positions: " & OpenPositions & ", ")
	if ContainerSize <> "" then Response.Write("ContainerSize: " & ContainerSize & "mg, ")
	Response.Write("</td></tr>")
	Response.write("<tr><th>Rack Name</th><th>Open Pos.</th><th>Filled Pos.</th><th>Location</th></tr>")
	While NOT RS.EOF
		Response.write("<tr><td valign=""middle""><input type=""checkbox"" name=""RackID"" value=""" & RS("location_id") & "," & RS("LocationPath") & " :: " & RS("OpenPositions") & " open"" />&nbsp;<img src=""/cheminv/images/treeview/rack_closed.gif"" border=""0"" />&nbsp;" & RS("location_name") & "</td>")
		Response.Write("<td align=""right"">" & RS("OpenPositions") & "</td>")
		Response.Write("<td align=""right"">" & RS("FilledPositions") & "</td>")
		Response.Write("<td><a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewSimpleRackLayout.asp?rackid=" & RS("location_id") & "&locationid=" & RS("location_id") & "&containerid=&RackPath=', 'Diag1', 2); return false;"">" & RS("LocationPath") & "</a></td></tr>")
		'Response.Write("<td><a class=""MenuLink"" href=""#"" title=""" & RS("LocationPathFull") & """>" & RS("LocationPath") & "</a></td></tr>")
	RS.MoveNext
	Wend
	RS.Close
	Set RS = nothing
	Set Conn = nothing
	Response.Write("</table>")
	Response.write("<a class=""MenuLink"" href=""#"" onclick=""checkAll(document.form1.RackID)"">Check All</a> | <a class=""MenuLink"" href=""#"" onclick=""uncheckAll(document.form1.RackID)"">Uncheck All</a><br /><br />")
	Response.write("<a href=""#"" class=""MenuLink"" onclick=""addRacks(document.form1.RackID)""><img src=""/cheminv/graphics/sq_btn/add_selected_racks.gif"" border=""0"" /></a>&nbsp;<a href=""#"" class=""MenuLink"" onclick=""window.close()""><img src=""/cheminv/graphics/sq_btn/cancel_dialog_btn.gif"" border=""0"" /></a></p>")
	

end if

elseif FormStep = "1" then 
if RestrictSize = "on" then RestrictSize=" checked=""checked"""
%>	
<table><tr><td>
	A-Code:<br />
	<input type="text" size="25" name="BatchField1" value="<%=BatchField1%>" /><br />
	Lot Number:<br />
	<input type="text" size="25" name="BatchField2" value="<%=BatchField2%>" /><br />
	Number of open positions:<br />
	<input type="text" size="3" name="OpenPositions" value="<%=OpenPositions%>" /><br />
	Container Size:<br />
	<table cellpadding="0" cellspacing="0"><tr><td>
		<input type="text" size="3" name="ContainerSize" value="<%=ContainerSize%>" />&nbsp;
	</td><td>
	<%Response.write ShowPickList("", "UOM", UOMAbv,"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC") & vbcrlf%>
	</td></tr></table>
	
	<!--<input type="checkbox" name="RestrictSize"<%=RestrictSize%> /> Restrict results by container size<br /><br />--><br />
	<input type="Submit" onclick="Validate(); return false;" name="Submit" value="Submit" />
</td></tr></table>

<% end if %>

</div>

</form>

<script language="javascript">
	if (opener.document.form1.iRegID) {
		RegID = opener.document.form1.iRegID.value;
		if (RegID.length > 0) {
			if (document.form1.BatchField1) {
				document.form1.BatchField1.value = RegID;
			}
		}
	}
	if (opener.document.form1.iBatchNumber) {
		BatchNumber = opener.document.form1.iBatchNumber.value;
		if (BatchNumber.length > 0) {
			if (document.form1.BatchField2) {
				document.form1.BatchField2.value = BatchNumber;
			}
		}
	}
	numSamples = "<%=numSamples%>";
	if (numSamples.length > 0) {
		if (document.form1.OpenPositions) {
			document.form1.OpenPositions.value = numSamples;
		}
	}
	contSize = "<%=contSize%>";
	if (contSize.length > 0) {
		if (document.form1.ContainerSize) {
			document.form1.ContainerSize.value = contSize;
		}
	}
	
	
</script>

</body>
</html>