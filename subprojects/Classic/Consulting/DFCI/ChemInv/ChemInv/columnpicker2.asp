<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/manage_queries.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/manage_user_settings_vbs.asp"-->
<%
Dim fieldArray
Dim Conn
Dim RS
bUseDefault = Request.Form("UseDefault")
bSaveColDef = Request.Form("SaveColDef")
bSetWidth = Request.Form("SetWidth")
ColDefStr = Request.Form("ColDef")

ArrayID = Request.QueryString("ArrayID")
PageSize = Request.Form("PageSize")
isSearch = Request.QueryString("isSearch")
showplates = Request.QueryString("showplates")
showWells = Request.QueryString("showWells")
showBatches = Request.QueryString("showBatches")
showRequests = Request.QueryString("showRequests")
showRackSummary = Request.QueryString("showRackSummary")
showReservations = Request.QueryString("showReservations")
if showplates = "" then showplates = false
if showwells = "" then showwells = false
if showBatches = "" then showbatches = false
if showRequests = "" then showRequests = false
if showRackSummary = "" then showRackSummary = false
if showReservations = "" then showReservations = false
entity = "Container"
if showplates then
	showType = "showplates"
	entity = "Plate"
elseif showWells then
	showType = "showWells"
	entity = "Well"
elseif showBatches then
	showType = "showBatches"
	entity = "Batch"
elseif showRequests then
	showType = "showRequests"
	entity = "Request"
elseif showRackSummary then
	showType = "showRackSummary"
	entity = "DisplayRack"
elseif showReservations then
	showType = "showReservations"
	entity = "Reservation"
end if
dbkey = Request.QueryString("dbkey")
formgroup = Request.QueryString("formgroup")

if PageSize <> "" then 
	Session("ScreenReportPageSize" & ArrayID) =  PageSize
	Session("UserNumListView" & dbkey) = PageSize
	Session("bColChooserUpdate") = "true"
	'Response.Write Session("ScreenReportPageSize" & ArrayID) & "##" & ArrayID
	'Response.End

	'-- if this is in a search page update the core user pref
	if isSearch = "1" then 
		User_ID = getUserSettingsID(dbkey, formgroup)
		Setting_Name = Ucase("USERNUMLISTVIEW" & dbkey & formgroup)
		'updateCurrentSettings dbkey, formgroup, Setting_Name, User_ID, PageSize 		
		currentSearchPrefs = selectCurrentSettings(dbkey, formgroup, User_ID, Setting_Name)
		on error resume next
		if currentSearchPrefs = -1 then
			insertCurrentSettings dbkey, formgroup, Setting_Name, User_ID, PageSize 
		else
			updateCurrentSettings dbkey, formgroup, Setting_Name, User_ID, PageSize 
		end if
	end if
	rc= WriteUserProperty(Session("UserNameCheminv"), "ScreenReportPageSize" & ArrayID , PageSize)
End if

if Len(ColDefStr) > 0 then
	if Len(bUseDefault) > 0 then ColDefStr = Application(entity & "ReportColDef" & ArrayID)
	rc= WriteUserProperty(Session("UserNameCheminv"), entity & "ChemInvFA" & ArrayID , ColDefstr)
	fieldArray = GetFieldArray(ColDefStr, Application(entity & "FieldMap"))
	Session(entity & "ReportFieldArray" & ArrayID) = fieldArray
	if (Len(bSaveColDef) > 0) AND bUseDefault=""  then
		Response.Write "<script language=JavaScript>opener.focus();opener.location.reload();window.close();</script>"
	End if
End if

%>
<html>
<head>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--
window.focus();

// DGB removed unused functions

// Add the selected items from the source to destination list
function addSrcToDestList() {
	destList = window.document.forms[0].destList;
	srcList = window.document.forms[0].srcList; 
	var len = destList.length;
	for(var i = 0; i < srcList.length; i++) {
		if ((srcList.options[i] != null) && (srcList.options[i].selected)) {
			//Check if this value already exist in the destList or not
			//if not then add it otherwise do not add it.
			var found = false;
			for(var count = 0; count < len; count++) {
				if (destList.options[count] != null) {
					if (srcList.options[i].text == destList.options[count].text) {
						found = true;
						break;
					}
			   }
			}
			if (found != true) {
				destList.options[len] = new Option(srcList.options[i].text, srcList.options[i].value); 
				len++;
			}
		}
	}
	deleteFromsrcList();
}

// Add the selected items from the dest to source list
function addDestToSrcList() {
	destList = window.document.forms[0].destList;
	srcList = window.document.forms[0].srcList; 
	var len = srcList.length;
	for(var i = 0; i < destList.length; i++) {
		if ((destList.options[i] != null) && (destList.options[i].selected)) {
			//Check if this value already exist in the source or not
			//if not then add it otherwise do not add it.
			var found = false;
			for(var count = 0; count < len; count++) {
				if (srcList.options[count] != null) {
				if (destList.options[i].text == srcList.options[count].text) {
				found = true;
				break;
				      }
				   }
			}
			if (found != true) {
				srcList.options[len] = new Option(destList.options[i].text, destList.options[i].value); 
				len++;
			}
		}
	}
	deleteFromDestList();
}


// Deletes from the destination list.
function deleteFromDestList() {
var destList  = window.document.forms[0].destList;
var len = destList.options.length;
for(var i = (len-1); i >= 0; i--) {
if ((destList.options[i] != null) && (destList.options[i].selected == true)) {
destList.options[i] = null;
      }
   }
}

// Deletes from the source list.
function deleteFromsrcList() {
var destList  = window.document.forms[0].srcList;
var len = srcList.options.length;
for(var i = (len-1); i >= 0; i--) {
if ((srcList.options[i] != null) && (srcList.options[i].selected == true)) {
srcList.options[i] = null;
      }
   }
}

function move(index,to) {
var list = window.document.forms[0].destList;
var total = list.options.length-1;
//for (i= 0; i< total +1; i++){alert(i + "-" + list.options[i].value + "-" + list.options[i].text)};
if (index == -1) return false;
if (to == +1 && index == total) return false;
if (to == -1 && index == 0) return false;
var items = new Array;
var values = new Array;
for (i = total; i >= 0; i--) {
items[i] = list.options[i].text;
values[i] = list.options[i].value;
}
for (i = total; i >= 0; i--) {
if (index == i) {
list.options[i + to] = new Option(items[i],values[i], 0, 1);
list.options[i] = new Option(items[i + to], values[i + to]);
i--;
}
else {
list.options[i] = new Option(items[i], values[i]);
   }
}
list.focus();
}

function FillColDef() {
	if (!isPositiveNumber(document.forms[0].PageSize.value) || document.forms[0].PageSize.value < 1){
		document.forms[0].PageSize.value = '';
		document.forms[0].PageSize.focus();
		document.forms[0].PageSize.select();
		alert("The number of items per page must be a positive number greater than zero.");
		return false;
	}	
	var list = window.document.forms[0].destList;
	var theList = "";
	for (var i = 0; i < list.options.length; i++) { 
		theList += list.options[i].value;
		if (i != list.options.length-1) theList += "|";
	}
	document.forms[0].ColDef.value = theList;
 }

function FillColWidths(){
	var bWriteError = false;
	theList = "";
	<%
		fieldArray = Session(entity & "ReportFieldArray" & ArrayID)		
		response.write vbcrlf
		For i = 0 to Ubound(fieldArray) 
			if NOT (Cstr(fieldArray(i,1)) = "RegBatchID" AND Application("RegServerName") = "NULL")  then
				response.write(vbtab & "if (!isPositiveNumber(document.forms[0].col_" & fieldArray(i,3) & ".value) || document.forms[0].col_" & fieldArray(i,3) & ".value < 1){" & vbcrlf)
					response.write(vbtab & vbtab & "alert(""The value for each column width must be a positive number greater than zero."");" & vbcrlf)
					response.write(vbtab & vbtab & "document.forms[0].col_" & fieldArray(i,3) & ".focus();")
					response.write(vbtab & vbtab & "document.forms[0].col_" & fieldArray(i,3) & ".select();")
					response.write(vbtab & vbtab & "return false;" & vbcrlf)
				response.write(vbtab & "} else {" & vbcrlf)
					response.write vbtab & vbtab & "theList= theList + """ & fieldArray(i,3) & """ + "":"" + document.forms[0].col_" & fieldArray(i,3) & ".value + ""|""; " & vbcrlf
				response.write(vbtab & "}" & vbcrlf & vbcrlf)
			end if
		Next
	%>
	theList = theList.substring(0,theList.length-1)
	document.forms[0].ColDef.value = theList;
}

function Validate() {

		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
<%
		fieldArray = Session(entity & "ReportFieldArray" & ArrayID)		
		For i = 0 to Ubound(fieldArray)
			if NOT (Cstr(fieldArray(i,1)) = "RegBatchID" AND Application("RegServerName") = "NULL")  then
				Response.Write vbtab & "if (!isPositiveNumber(document.forms[0].col_" & fieldArray(i,3) & ".value)) {" & vbcrlf
					Response.Write vbtab & vbtab & "errmsg = errmsg + ""- Width for " & fieldArray(i,1) & " must be a positive number.\r"";" & vbcrlf
					Response.Write vbtab & vbtab & "bWriteError = true;" & vbcrlf
				Response.Write vbtab & "}" & vbcrlf
			end if
		Next
%>

		if (bWriteError){
			alert(errmsg);
		}
		else{
			FillColWidths(); 
			document.forms[0].SaveColDef.value='true'; 
			document.forms[0].submit();
		}
}
//-->
</script>
</head>
<body>
<center>
<form method="POST">
<input type="hidden" name="UseDefault">
<input type="hidden" name="ColDef">
<input type="hidden" name="SaveColDef">
<input type="hidden" name="SetWidth">

<%if Len(bSetWidth) > 0 then%>
	<table bgcolor="#C0C0C0" border="0">
		<tr>
			<td colspan="3" bgcolor="#C0C0C0" align="center"><span class="GuiFeedback">Enter width (in characters) for each column</span></td>
		</tr>
		<%
				fieldArray = Session(entity & "ReportFieldArray" & ArrayID)		
				For i = 0 to Ubound(fieldArray)
					'Response.Write			fieldArray(i,1) & ":" & fieldArray(i,2) & ":" & fieldArray(i,3) & ":" & i & "<BR>"				
					if NOT (Cstr(fieldArray(i,1)) = "RegBatchID" AND Application("RegServerName") = "NULL")  then
						Response.Write "<tr>"
						Response.Write "	<td align=right>"
						Response.Write			fieldArray(i,1) & ":"
						Response.Write "	</td>"
						Response.Write "	<td align=left>"
						Response.Write			"<INPUT size=3 type=text name=""col_" & fieldArray(i,3) & """ value=""" & fieldArray(i,2) & """>" 
						Response.Write "	</td>"
						Response.Write "<td width=150></td>"
						Response.Write "</tr>"
					end if
				Next
				
		%>
		<tr>
			<td colspan="3" align="right">
				<input type="button" value="Cancel" onClick="document.location.href='columnpicker2.asp?ArrayID=<%=ArrayID%>&amp;<%=showType%>=true'">
				<input type="button" value="Save" onClick="Validate()" id="button3" name="button3">
			</td>
		</tr>
	</table>
<%Else%>
	<table bgcolor="#C0C0C0">
		<tr>
			<td colspan="4" bgcolor="#C0C0C0" align="center"><span class="GuiFeedback">Choose Columns for <%=entity%> List</span></td>
		</tr>
		<tr>
			<td bgcolor="#C0C0C0" width="74">Available:</td>
			<td bgcolor="#C0C0C0"> </td>
			<td bgcolor="#C0C0C0" width="69">Selected:</td>
			<td bgcolor="#C0C0C0"> </td>
		</tr>
		<tr>
			<td bgcolor="#C0C0C0" width="85">
				<select size="6" name="srcList" multiple>
					<%
					
					fieldArray = Application(entity & "FieldMap")
					
					'DGB only display the fields that are not already selected
					FAstr = "|" &GetUserProperty(Session("UserNameCheminv"),entity & "ChemInvFA" & ArrayID)
					For i = 0 to Ubound(fieldArray)
						if NOT (Cstr(fieldArray(i,1)) = "RegBatchID" AND Application("RegServerName") = "NULL") and NOT (Cstr(fieldArray(i,1)) = "Container ID") then
							if NOT InStr(1,FAstr, "|" & i & ":") > 0 and fieldArray(i,2) <> "" then
								Response.Write "<OPTION value=""" & i &  ":" & fieldArray(i,2) & """>" & fieldArray(i,1)
							end if
						end if
					Next
					%>
					</select>
			</td>
			<td bgcolor="#C0C0C0" width="74" align="center">
				<a href="#" onClick="javascript:addSrcToDestList();return false"><img border="0" src="/cheminv/graphics/right_arrow.gif"></a>
				<br><br>
				<a href="#" onclick="javascript:addDestToSrcList();return false"><img border="0" src="/cheminv/graphics/left_arrow.gif"></a>
			</td>
			<td bgcolor="#C0C0C0" width="69">
				<select size="6" name="destList" multiple>
				<%
				
				fieldArray = Session(entity & "ReportFieldArray" & ArrayID)		
				
				For i = 0 to Ubound(fieldArray)
					Response.Write "<OPTION value=""" & fieldArray(i,3) & ":" & fieldArray(i,2) & """>" & fieldArray(i,1)
				Next
				%>
				</select>
			</td>
			<td valign="middle">
				<a href="#" onClick="move(document.forms[0].destList.selectedIndex,-1);return false"><img border="0" src="/cheminv/graphics/up_arrow.gif"></a><br>
				<a href="#" onClick="move(document.forms[0].destList.selectedIndex,+1);return false"><img border="0" src="/cheminv/graphics/down_arrow.gif"></a>
			</td>
		</tr>
		<% if lCase(entity) <> "request" and lCase(entity) <> "displayrack" and lCase(entity) <> "reservation" then %>
		<tr>
			<td align="left" colspan="4"><%if lCase(entity) = "batch" then Response.write(entity & "es") else Response.write(entity & "s") end if%> per page:&nbsp;<input type="text" size="4" name="PageSize" value="<%=Session("ScreenReportPageSize" & ArrayID)%>"></td>
		</tr>
		<%else%>
		<input type="hidden" name="PageSize" value=1>
		<% end if %>
		<tr>
			<td colspan="4" align="center">
				<input type="button" value="Cancel" onClick="window.close()">
				<input type="button" value="Use Default" onClick="FillColDef(); document.forms[0].UseDefault.value='true'; submit()">
				<input type="button" value="Save" onClick="FillColDef(); document.forms[0].SaveColDef.value='true'; submit()">
				<input type="button" value="Set Widths" onClick="FillColDef(); document.forms[0].SetWidth.value='true'; submit()">
			</td>
		</tr>
	</table>
	
		
<%End if%>	
<br>
<table width="330">
	<tr>
		<td bgcolor="#FFFFFF" colspan="4">
			<font size="1"><b>Note:</b>&nbsp;Column selection and layout is for screen display only.  The layout of printed reports is controlled by report templates on the server.  Please contact your ChemInv administrator for customized report layouts.</font>
		</td>
	</tr>
</table>
</center>
</form>
</body>
</html>
