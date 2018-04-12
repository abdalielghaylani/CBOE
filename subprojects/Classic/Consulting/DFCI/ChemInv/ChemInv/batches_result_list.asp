<%@ LANGUAGE="VBScript" %>

<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
		<script type="text/javascript" language="javascript">
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
			
			///////////////////////////////////////////////////////////////////////////////
			// Opens up a dialog box
			//
			//function OpenDialog(url, name, width, height, topPos, leftPos)
			//{
			//		var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=" + width + ",height=" + height + ",screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
			//		DialogWindow = window.open(url,name,attribs);
			//}
			//-->
			
			document.onkeydown = keyDown;
			document.onkeyup = keyUp;
			var bCtrlDown = false;
			
			function keyDown() {
				var keycode = event.keyCode;
				var realkey = String.fromCharCode(event.keyCode);
				//alert("keycode: " + keycode + "\nrealkey: " + realkey)
				if (keycode == 17) {
					//var test = document.getElementById("1");
					bCtrlDown = true;
					//alert("test");
				}			
				if (bCtrlDown && keycode == 33) {
					//alert("ctrl-pgup detected!");
					document.location = "<%=NextPageURL()%>";
					//alert (document.form1.selectChckBox.value);

					return false;
				}
				if (bCtrlDown && keycode == 34) {
					//alert("ctrl-pgdn detected!");
					document.location = "<%=PrevPageURL()%>";
					return false;
				}
			}
			
			function keyUp() {
				var keycode = event.keyCode;
				if (keycode = 17) {
					bCtrlDown = false;
				}
			}

			function updateDict(bClear) {
				var allChckBoxValues = "";
				var removeList = document.form1.removeList.value;
				for (var i = 0; i < document.form1.selectChckBox.length; i++) {
					if (document.form1.selectChckBox[i].checked) allChckBoxValues += document.form1.selectChckBox[i].value + ",";
				}
				allChckBoxValues = allChckBoxValues.substr(0,(allChckBoxValues.length - 1));
				removeList = removeList.substr(0,(removeList.length - 1));
				var strURL = "http://" + serverName + "/cheminv/cheminv/MultiSelect.asp?selectChckBox=" + allChckBoxValues + "&removeList=" + removeList;	
				if (bClear) strURL += "&clear=True";
				//alert(strURL);
				//alert(document.form1.removeList.value + ":removeList");
				//errmsg = errmsg + "- " + strURL + "\r";
				//bWriteError = true;
				var httpResponse = JsHTTPGet(strURL);
				//alert(httpResponse);
				updateNumContainers(httpResponse);
				document.form1.removeList.value = "";
			}
			
			function updateNumContainers(num) {
				var text;
				
				if (num == 1) {
					text = num + " Batches Selected";
				}
				else {
					text = num + " Batches Selected";
				}
				
				document.all.containerNumSpan.innerHTML = text;
			}			
			
		</script>
<!--
		<STYLE>
			img { width:76px;height:15px;}
		</STYLE>
-->
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<body>
<%
Dim ListView
Dim fieldArray
Dim bdebugPrint
Dim SortbyFieldName
Dim SortDirectionTxt
Dim SortDirection
Dim bHitsFound
Dim Conn
Dim Cmd
Dim RS


if Request("SortDirectionTxt") <> "" then
	SortDirectionTxt = Request("SortDirectionTxt")
	Session("BatchSortDirection") = SortDirectionTxt
else
	if Session("BatchSortDirection") = "" then
	SortDirectionTxt = "asc"
	Session("BatchSortDirection") = SortDirectionTxt
	else
		SortDirectionTxt = Session("BatchSortDirection")
	end if
end if

QS = Request.QueryString

if Request("multiSelect") = 1 and Request("clear") <> 1 then
	Session("bMultiSelect") = true
elseif Request("clear") = 1 then
	Session("bMultiSelect") = false
	QS = Replace(QS, "multiSelect=" & Request.QueryString("multiselect") & "&" ,"")
	QS = Replace(QS, "clear=" & Request.QueryString("clear") & "&" ,"")
end if

if Session("bMultiSelect") then
	
end if

bdebugPrint = false
bHitsFound = true

if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
    listItemNumber = 0
    HitsFound = true 
Else
    bHitsFound = false
end if

showBatches = "1"
If bHitsFound then
	QS = Replace(QS, "view=" & Request.QueryString("View") & "&" ,"")
	nameLength =10
	lView = Request.QueryString("view")
	If lView = "" Then lView = 3

	'showbatches = true

	Call GetSortfield()

	ImagePath = "../images/listview"
	Loc_Icon_lrg_clsd = "icon_clsdfold32.gif"
	Loc_Icon_sml_clsd = "icon_clsdfold.gif"
	Loc_Icon_sml_open = "icon_openfold.gif"
	Con_Icon_lrg_clsd = "batch_request.gif"
	Con_Icon_sml_clsd = "batch_request.gif"
	Con_Icon_lrg_open = "batch_request.gif"
	Con_Icon_sml_open = "batch_request.gif"
	
	Icon_Open = Con_Icon_lrg_open
	Icon_Clsd = Con_Icon_lrg_clsd

	'-- Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("BatchReportFieldArray1")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"BatchChemInvFA1")
		If IsNull(ColDefStr) OR ColDefStr = "" then 
			'-- Default column definition
			ColDefstr= Application("BatchReportColDef1")
			rc = WriteUserProperty(Session("UserNameCheminv"), "BatchChemInvFA1", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("BatchFieldMap"))
		Session("BatchReportFieldArray1") = fieldArray
	Else
		' ReDim fieldArray(Ubound(Session("ContainerReportFieldArray1")),3)
		fieldArray = Session("BatchReportFieldArray1")
	End if
	%>

	<div align="left" id="header" style="POSITION:Absolute;top:0;left:0;background-color:white;visibility:visible;z-index:2">
		<TABLE cellspacing=0 cellpadding=0 width=600>	
			<TR>
				<td align=center COLSPAN="2">
					<Span class="GuiFeedback" id="mySpan">&nbsp;</Span>
				</td>
			</TR>

			<TR>
				<td align=center>
<!--				<A CLASS="MenuLink" HREF="batches_result_list.asp?view=3&multiSelect=1&<%=QS%>" >Multi Select</A>
					|-->
					<A CLASS="MenuLink" HREF="batches_result_list.asp?view=0&<%=QS%>" >Large Icons</A>
					|
					<A CLASS="MenuLink" HREF="batches_result_list.asp?view=1&<%=QS%>" >Small Icons</A>
<!--					|
					<A CLASS="MenuLink" HREF="batches_result_list.asp?view=2&<%=QS%>" >List</A>-->
					|
					<A CLASS="MenuLink" HREF="batches_result_list.asp?view=3&<%=QS%>" >Details</A>
					|
					<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('columnPicker2.asp?ArrayID=1&showbatches=1', 'Diag', 1); return false">Column Chooser</a>
					|
					<A CLASS="MenuLink" HREF="/cheminv/Gui/CreateReport_frset.asp?ShowInList=batches&HitlistID=<%=Session("hitlistID" & "ChemInv" & formgroup)%>" target="RPT">Print Report</A>
				</td>
			</tr>
		</table>
	<% 
		if lview = 3 then
			nameLength = 50
			Icon_Open = Con_Icon_sml_open
			Icon_Clsd = Con_Icon_sml_clsd
			'for i=0 to ubound(fieldArray)
			'	Response.Write(fieldArray(i,0) & ":" & fieldArray(i,1) & ":" & fieldArray(i,2) & "<br>")
			'next
			WriteTableHeaderFromFieldArray fieldArray, Application("BatchFieldMap"),0
		end if
	%>
	</div>
	<script FOR="window" EVENT="onscroll" LANGUAGE="JScript">
	slide();
	</script>
	<script language="javascript">
	function slide() {
		with(document.body){
			header.style.left=0;
			header.style.top=scrollTop;
		}
	}
	</script>
	<%
	' Create the listview object
	Set ListView = Server.CreateObject("VASPLV.ASPListView")

		ListView.Class = "ListView"
		ListView.ImagePath = ImagePath
		ListView.View = lView
		ListView.Width = 1700
		ListView.Height = 350
		ListView.Gridlines = false
		ListView.ColumnHeaderForeColor = ""
		ListView.ColumnHeaderBackColor = ""
		ListView.HideColumnHeaders = true
		ListView.LabelWrap = False
		ListView.BackColor = ""
		ListView.Picture = ""
		ListView.Licensekey = "1C71-2CE6-A49C"

	if lview = 3 then
		BuildListViewHeaderFromFieldArray fieldArray, Application("BatchFieldMap"),0
	End if	
	BaseIDList = ""
End if

%>	
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
	BaseIDList =  BaseIDList  &  "," & BaseID
%>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/BatchSQL.asp"-->
<form name="form1" method="POST" action="/cheminv/cheminv/MultiSelect.asp" target="_blank">
<input type="hidden" name="removeList">
<%

If bHitsFound then
	if BaseIDList <> "" then
		BaseIDList = Right(BaseIDList, Len(BaseIDList)-1)
	Else
		BaseIDList = "NULL"
	End if
	
	' If there is only one record to display then Go to form view
	'if end_index = 1 then Response.Redirect "/cheminv/cheminv/cheminv_form_frset.asp?formgroup=" & formgroup & "&dbname=cheminv&formmode=edit" & "&unique_id=" & BaseRunningIndex
		
	' DGB Join to csdohitlist and Upper the sort field name
	if SortbyFieldName = "BATCHID" then
		SortbyFieldText = "(" & SortbyFieldName & ")"
	else
		SortbyFieldText = "upper(" & SortbyFieldName & ")"
	end if
	SQL = Replace(SQL, "FROM " & vblf & "inv_container_batches", "FROM " & Application("CHEMINV_USERNAME") & ".CSDOHitlist, inv_container_batches")
	SQL = SQL & "AND " & _ 
			"inv_container_batches.ROWID = " & Application("CHEMINV_USERNAME") & ".CSDOHitlist.ID " & _
			"AND " & _ 
			Application("CHEMINV_USERNAME") & ".CSDOHitlist.HitlistID = ? " & _
			"GROUP BY BATCH_ID,unit_of_meas_id_fk,Batch_Status_ID_FK,inv_container_batches.batch_field_1 , inv_container_batches.batch_field_2 , inv_container_batches.batch_field_3 , inv_container_batches.field_1 , inv_container_batches.field_2 , inv_container_batches.field_3 , inv_container_batches.field_4 , inv_container_batches.field_5 , inv_container_batches.date_1 , inv_container_batches.date_2 " & vbcrlf & _
			"ORDER BY " & SortbyFieldText & " " & Session("BatchSortDirection")  
	
	'Response.Write "<BR/><BR/><BR/><BR/>"
	'Response.Write(SQL)		
	'Response.End			
	' DGB Execute via command

	Call GetChemInvCommand("placeholdertext", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, Session("hitlistID" & "ChemInv" & formgroup))
	if bdebugPrint then
		Response.Write "<br><br><br><br>" & SQL & "<br>"
		Response.Write "? = " & Session("hitlistID" & "ChemInv" & formgroup)
		Response.End
	End if
	Cmd.CommandText = SQL
	
	
	Dim mypage
	Dim mypagesize
	Dim maxcount
	mypage=request.querystring("whichpage")
	If mypage="" then
		mypage=1
	end if
		
	mypagesize = Session("ScreenReportPageSize1")
	If mypagesize="" then
		mypagesize = GetUserProperty(Session("UserNameCheminv"),"ScreenReportPageSize1")
		if mypagesize = "" or IsNull(mypagesize) then
			mypagesize= 100
			rc= WriteUserProperty(Session("UserNameCheminv"), "ScreenReportPageSize1" , mypagesize)
		End if
		Session("ScreenReportPageSize1") = mypagesize
	End if
	

	Set RS = Server.CreateObject("ADODB.Recordset")
	RS.cursorlocation= aduseclient
	RS.cachesize=5
	RS.open Cmd
	TotalBatches = RS.RecordCount
	
	if NOT (RS.BOF AND RS.EOF) then 
		RS.movefirst
		RS.pagesize=mypagesize
		maxcount=cint(RS.pagecount)
		RS.absolutepage=mypage
	End if
	BatchCount = 0
	if maxCount > 1 then
		response.write PageNavBar2()
	end if
	If RS.EOF AND RS.BOF then bNoBatches = True

	Dim BatchCount
	Dim aindex	
	aindex = 0
	basearray = Session("Base_RS" & dbkey & formgroup)
	
	Do While NOT RS.EOF 'AND BatchCount < RS.pagesize
	
		BatchID = RS("BatchID")
		BatchStatus = RS("BatchStatus")
		UOMAbbrv = RS("UOMAbbrv")
		Substance = RS("Substance")
		AmountRemaining = RS("AmountRemaining")
		AmountAvailable = RS("AmountAvailableNumber")
		AmountReserved = RS("AmountReserved")
		NumberOfOrganizations = RS("NumberOfOrganizations")
		bAllowReservation = true
		if cInt(NumberOfOrganizations) = 0 then bAllowReservation = false
		
		' Resort the basearray
		basearray(0,aindex) = BatchID
		
		Session("Base_RS" & dbkey & formgroup)(aindex) = BatchID
		'-- Allow/disallow Requests/Reservations based by batch status
		if BatchStatus = 1 then AllowBatchRequest = true else AllowBatchRequest = true end if
			
		'urlViewBatchDetails = "|&nbsp;<a href=""#"" class=""MenuLink"" onclick=""OpenDialog('/cheminv/gui/RequestBatch.asp?bAllowReservation=" & bAllowReservation & "&RequestType=reservation&batchid=" & BatchID & "&AmountAvailable=" & AmountAvailable & "&uomabbrv=" & UOMAbbrv & "&AllowBatchRequest=" & AllowBatchRequest & "', 'DiagRequest', 2);return false"" title=""Reserve Batch " & BatchID & """>Reserve</a>" & "&nbsp;<a href=""#"" class=""MenuLink"" onclick=""OpenDialog('/cheminv/gui/ViewBatchInfo.asp?BatchID=" & BatchID & "&ShowLinks=0&SortDirectionTxt=" & SortDirectionTxt & "','DiagBatchDetails',2);return false"" title=""Manage Batch Details for " & BatchID & """><img src=""/cheminv/images/listview/batch_request.gif"" border=""0"">" & BatchID & "</a>"
		urlViewBatchDetails = "|&nbsp;<a href=""#"" class=""MenuLink"" onclick=""OpenDialog('/cheminv/gui/ReserveBatch.asp?bAllowReservation=" & bAllowReservation & "&RequestType=reservation&batchid=" & BatchID & "&AmountAvailable=" & AmountAvailable & "&uomabbrv=" & UOMAbbrv & "&AllowBatchRequest=" & AllowBatchRequest & "', 'DiagRequest', 2);return false"" title=""Reserve Batch " & BatchID & """>Reserve</a>" & "&nbsp;<a href=""#"" class=""MenuLink"" onclick=""OpenDialog('/cheminv/gui/ViewBatchInfo.asp?BatchID=" & BatchID & "&ShowLinks=0&SortDirectionTxt=" & SortDirectionTxt & "','DiagBatchDetails',2);return false"" title=""Manage Batch Details for " & BatchID & """><img src=""/cheminv/images/listview/batch_request.gif"" border=""0"">" & BatchID & "</a>"
		Set Listx = ListView.ListItems.Add(, , "Request </a> " & urlViewBatchDetails, , , 30, cInt(30), "Request Batch " & BatchID)
		'Listx.URL = "#"" onclick=""OpenDialog('/cheminv/gui/RequestBatch.asp?RequestType=&batchid=" & BatchID & "&AmountAvailable=" & AmountAvailable & "&uomabbrv=" & UOMAbbrv & "&AllowBatchRequest=" & AllowBatchRequest & "', 'DiagRequest', 2); return false"
		Listx.URL = "#"" onclick=""OpenDialog('/cheminv/gui/RequestSample.asp?RequestType=&action=create&batchid=" & BatchID & "&QtyRequired=" & AmountAvailable & "&AmountAvailable=" & AmountAvailable & "&UOMAbv=" & UOMAbbrv & "&AllowBatchRequest=" & AllowBatchRequest & "', 'DiagRequest', 2); return false"
		'Set Listx = ListView.ListItems.Add(, , BatchID, , , , 20, "View Batch Details")
		'Listx.URL = "#"" onclick=""OpenDialog('/cheminv/gui/ViewBatchInfo.asp?BatchID=" & BatchID & "&ShowLinks=0','Diag',1);return false"

		if lview = 3 then
			AddListViewSubItemsFromFieldArray(fieldArray)
		End if
		
		BatchCount = BatchCount + 1 
		aindex = aindex + 1
	
		RS.MoveNext
	Loop
	

	Session("Base_RS" & dbkey & formgroup) = basearray		
	CloseRS(BaseRS)
	CloseConn(DataConn)
	
	%>

	<div align="left" id="report" style="position:Absolute;top:65;left:0;  z-index:1">
	<%
	ListView.Show
	ListView.Unload
	Set ListView = Nothing
	%>
	</div>
	<SCRIPT LANGUAGE=javascript>
	<!--
	function SortReport(fieldName){
		var url = document.location.href;
		var sortOrder = "<%=lcase(Session("BatchSortDirection"))%>";
		
		if (sortOrder == "asc"){
			sortOrder = "desc";
		}else if (sortOrder == "desc") {
			sortOrder = "asc";
		}
		url = "batches_result_list.asp?formgroup=" + "<%=formgroup%>" + "&formmode=" + "<%=formmode%>" + "&dbname=cheminv" + "&BaseCurrentIndex=" + "<%=current_index%>" + "&BaseRecordCount=" + "<%=totalrecords%>" + "&dbsearchnames=cheminv" + "&SortByFieldName=" + fieldName + "&SortDirectionTxt=" + sortOrder;
		document.location.href = url;
	}
	//-->
	</SCRIPT>
<%End if%>
	</form>
	</body>
</html>
<%

function NextPageURL()
	Dim qs
	qs = ""
	qs = qs & "&formgroup=" & formgroup & "&formmode=list&dbname=cheminv&PagingMove=previous_record"
	scriptname = request.servervariables("script_name")
	maxcount = Request("maxcount")
	
	currentPage = Request("whichpage")
	if currentPage = "" then 
		currentPage = 1
		nextPage = 2
	elseif currentPage = maxcount then
		nextPage = currentPage
	else
		nextPage = currentPage + 1
	end if
	URL = scriptname & "?whichpage=" & nextPage
	URL = URL & "&maxcount=" & maxcount
	URL = URL & qs
	NextPageURL = URL
end function

function PrevPageURL()
	Dim qs
	qs = ""
	qs = qs & "&formgroup=" & formgroup & "&formmode=list&dbname=cheminv&PagingMove=previous_record"
	scriptname = request.servervariables("script_name")
	maxcount = Request("maxcount")

	currentPage = Request("whichpage")
	if currentPage = "" then 
		currentPage = 1
		prevPage = 2
	elseif currentPage = 1 then
		prevPage = currentPage
	else
		prevPage = currentPage - 1
	end if
	URL= scriptname & "?whichpage=" & prevPage
	URL = URL & "&maxcount=" & maxcount
	URL = URL & qs
	PrevPageURL = URL

end function

%>

<SCRIPT LANGUAGE="javascript">

	function SelectMarked(){
				var reml = document.form1.removeList;
				var len = reml.value.length
				if (reml.value.substring(len -1 , len) == ","){
					reml.value = reml.value.substring(0, len - 1);
				}
				//alert(reml.value);
				document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?batchCount=<%=totalBatches%>";
				elm = document.form1.selectChckBox;
				<%if bDisableChkBoxes then%>
					//MCD: added check for case where there's only one element
					if (elm.length){
						for (i=0; i < elm.length ; i++){
							elm[i].disabled = false;
						}
					}
					else{
						elm.disabled = false;
					}
				<%end if%>
				document.form1.submit();
				<%if bDisableChkBoxes then%>
					//MCD: added check for case where there's only one element
					if (elm.length){
						for (i=0; i < elm.length ; i++){
							elm[i].disabled = true;
						}
					}
					else{
						elm.disabled = true;
					}
				<%end if%>
				reml.value =""; 
			}
			
			function CheckAll(bCheck){
				var cbObj = document.form1.selectChckBox;
				
				if (cbObj.length){
					for (i=0; i< cbObj.length; i++){
						if (cbObj[i].checked ^ bCheck){
							cbObj[i].checked = bCheck;
							if (!bCheck) Removals(cbObj[i].value, false);
						}
					}
				}
				else{
					if (cbObj.checked ^ bCheck){
							cbObj.checked = bCheck;
							if (!bCheck) Removals(cbObj.value, false);
						}
				}
				updateDict(!bCheck);	
			}
		
		
		
			function Removals(id, bRemove){
				var idc = id + ",";
				var reml = document.form1.removeList;
			
				if (bRemove){
					if (reml.value.indexOf(idc) >=0){
						reml.value = reml.value.replace(idc,"");
					}
				}
				else{
					if (reml.value.indexOf(idc) ==  -1){
						reml.value += id + ",";
					}
				}
				//alert(reml.value);				
			}
			function HappySound(){
				snd1.src='/cheminv/sounds/yes1.wav';
				return true;
			}
	
			function SadSound(){
				snd1.src='/cheminv/sounds/no1.wav';
			}
			
			function SadSound2(){
				snd1.src='/cheminv/sounds/no2.wav';
			}
			
</SCRIPT>


