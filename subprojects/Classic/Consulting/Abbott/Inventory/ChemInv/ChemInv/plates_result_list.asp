<%@ LANGUAGE="VBScript" %>
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
	<head>
		<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
		<title><%=Application("appTitle")%> -- Results-List View</title>
		<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
		<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
		<script language="JavaScript">
			top.bannerFrame.theMainFrame = <%=Application("mainwindow")%>
			
			///////////////////////////////////////////////////////////////////////////////
			// Opens up a dialog box
			//
			function OpenDialog(url, name, width, height, topPos, leftPos)
			{
					var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable,width=" + width + ",height=" + height + ",screenX=" + leftPos + ",screenY=" + topPos + ",left=" + leftPos + ",top=" + topPos;
					DialogWindow = window.open(url,name,attribs);
			}
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
				var strURL = "http://" + serverName + "/cheminv/cheminv/MultiSelect.asp?selectChckBox=" + allChckBoxValues + "&removeList=" + removeList + "&dictType=plate";	
				if (bClear) strURL += "&clear=True";
				//alert(strURL);
				//alert(document.form1.removeList.value + ":removeList");
				//errmsg = errmsg + "- " + strURL + "\r";
				//bWriteError = true;
				var httpResponse = JsHTTPGet(strURL);
				//alert(httpResponse);
				updateNumPlates(httpResponse);
				document.form1.removeList.value = "";
			}
			
			function updateNumPlates(num) {
				var text;
				
				if (num == 1) {
					text = num + " Plate Selected";
				}
				else {
					text = num + " Plates Selected";
				}
				
				document.all.plateNumSpan.innerHTML = text;
			}			
			
		</SCRIPT>
		
		<STYLE>
			A {Text-Decoration: none;}
			.ListView {color:#000000; font-size:8pt; font-family: Lucida Console}
			A.ListView:LINK	{Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
			A.ListView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
			A.ListView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial
		</STYLE>
	</head>
	<!--#INCLUDE FILE="../source/secure_nav.asp"-->
	<!--#INCLUDE FILE="../source/app_js.js"-->
	<!--#INCLUDE FILE="../source/app_vbs.asp"-->
	<body bgcolor="#FFFFFF">
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
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

showPlates = "1"
QS = Request.QueryString
if Request("multiSelect") = 1 and Request("clear") <> 1 then
	Session("bMultiSelect") = true
elseif Request("clear") = 1 then
	Session("bMultiSelect") = false
	QS = Replace(QS, "multiSelect=" & Request.QueryString("multiselect") & "&" ,"")
	QS = Replace(QS, "clear=" & Request.QueryString("clear") & "&" ,"")
end if

bdebugPrint = false
bHitsFound = true

if Not Session("fEmptyRecordset" & dbkey & formgroup) = True  then
  listItemNumber = 0
  HitsFound = true 
Else
	bHitsFound = false
end if

If bHitsFound then
	QS = Replace(QS, "view=" & Request.QueryString("View") & "&" ,"")
	nameLength =10
	lView = Request.QueryString("view")
	If lView = "" Then lView = 3

	Call GetSortfield()

	ImagePath = "../images/listview"
	Loc_Icon_lrg_clsd = "icon_clsdfold32.gif"
	Loc_Icon_sml_clsd = "icon_clsdfold.gif"
	Loc_Icon_sml_open = "icon_openfold.gif"
	Con_Icon_lrg_clsd = "Plate_icon_16.gif"
	Con_Icon_sml_clsd = "Plate_icon_16.gif"
	Con_Icon_lrg_open = "Plate_icon_16.gif"
	Con_Icon_sml_open = "Plate_icon_16.gif"
	Icon_Open = Con_Icon_lrg_open
	Icon_Clsd = Con_Icon_lrg_clsd

	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("PlateReportFieldArray1")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"PlateChemInvFA1")
		If IsNull(ColDefStr) OR ColDefStr = "" then 
			' Default column definition
			ColDefstr= Application("PlateReportColDef1")
			rc= WriteUserProperty(Session("UserNameCheminv"), "PlateChemInvFA1", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("PlateFieldMap"))
		Session("PlateReportFieldArray1") = fieldArray
	Else
		'ReDim fieldArray(Ubound(Session("ContainerReportFieldArray1")),3)
		fieldArray = Session("PlateReportFieldArray1")
	End if
	%>
	<div align="left" id="header" style="POSITION:Absolute;top:0;left:0;background-color:white;visibility:visible;z-index:2">
		<TABLE cellspacing=0 cellpadding=0 width=600>	
			<TR>
				<td align=center COLSPAN="2">
					<Span class="GuiFeedback" id="mySpan">&nbsp;</Span>
				</td>
			</TR>

			<%
			if Session("bMultiSelect") then
				ListPositionHeight = 75
			%>
			<TR>
				<td align=center COLSPAN="2">
					<A CLASS="MenuLink" HREF="#" onclick="CheckAll(true); return false">Select All</A>
					|
					<A CLASS="MenuLink" HREF="#" onclick="CheckAll(false); return false">Clear All</A>
					|
					<!--<a class="MenuLink" HREF="/cheminv/gui/<%=Session("TabFrameURL")%>?containerCount=0&clear=1" target="TabFrame">Cancel MultiSelect</a>-->					
					<a class="MenuLink" HREF="plates_result_list.asp?view=3&clear=1&<%=Replace(QS, "multiSelect=1&","")%>">Cancel MultiSelect</a>
				</td>
			</tr>
			<tr>
				<TD ALIGN="LEFT" VALIGN="top" NOWRAP><SPAN CLASS="GuiFeedback" ID="plateNumSpan"><%if Session("bMultiSelect") then Response.Write"<SCRIPT LANGUAGE=""javascript"">updateNumPlates(" & plate_multiSelect_dict.count & ");</SCRIPT>"%></SPAN></TD>
				<td align="center" valign="top" nowrap>
					<%If Session("INV_CREATE_PLATE" & dbkey) then%>	
						<a class="MenuLink" HREF="#" onclick="OpenDialog2('/Cheminv/GUI/ReformatPlate.asp?multiSelect=true', 'Diag', 1); return false">Reformat Plates</a>
						|
					<%end if%>
					<%If Session("INV_CREATE_PLATE" & dbkey) then%>	
						<a class="MenuLink" HREF="#" onclick="OpenDialog2('/Cheminv/GUI/ReformatPlate.asp?multiSelect=true&pageAction=daughter', 'Diag', 1); return false">Daughter Plates</a>
						|
					<%end if%>
					<%If Session("INV_EDIT_PLATE" & dbkey) then%>	
						<a class="MenuLink" HREF="#" onclick="OpenDialog2('/Cheminv/GUI/UpdatePlate.asp?multiSelect=true&action=edit', 'Diag', 1); return false">Update Plates</a>
						|
					<%end if%>
					<%If Session("INV_MOVE_PLATE" & dbkey) then%>	
						<a class="MenuLink" HREF="#" onclick="OpenDialog2('/Cheminv/GUI/MovePlate.asp?multiSelect=true', 'Diag', 1); return false">Move Plates</a>
						|
					<%end if%>
					<%If Session("INV_RETIRE_PLATE" & dbkey) then%>
						<a class="MenuLink" HREF="#" onclick="OpenDialog2('/Cheminv/GUI/RetirePlate.asp?multiSelect=true', 'Diag', 1); return false">Retire Plates</a>
						|
					<%End if%>
					<%If Session("INV_DELETE_PLATE" & dbkey) then%>	
						<a class="MenuLink" HREF="#" onclick="OpenDialog2('/Cheminv/GUI/DeletePlate.asp?multiSelect=true', 'Diag', 1); return false">Delete Plates</a>		
					<%End if%>						
				</td>
			</tr>
			<%
			Else
				ListPositionHeight = 55
			%>
			<TR>
				<td align=center>
					<A CLASS="MenuLink" HREF="plates_result_list.asp?view=3&multiSelect=1&<%=Replace(QS, "clear=1&","")%>" >Multi Select</A>
					|
					<A CLASS="MenuLink" HREF="plates_result_list.asp?view=0&<%=QS%>">Large Icons</A>
					|
					<A CLASS="MenuLink" HREF="plates_result_list.asp?view=1&<%=QS%>" >Small Icons</A>
					<!--
					|
					<A CLASS="MenuLink" HREF="plates_result_list.asp?view=2&<%=QS%>" >List</A>
					-->
					|
					<A CLASS="MenuLink" HREF="plates_result_list.asp?view=3&<%=QS%>" >Details</A>
					|
					<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('columnPicker2.asp?ArrayID=1&showplates=1', 'Diag', 550, 450, 200,200); return false">Column Chooser</a>
					|
					<A CLASS="MenuLink" HREF="/cheminv/Gui/CreateReport_frset.asp?HitlistID=<%=Session("hitlistID" & "ChemInv" & formgroup)%>&ShowInList=plates" target="RPT">Print Report</A>
				</td>
			</tr>
			<%End if%>
		</table>
	<% 
		if lview = 3 then
			nameLength = 50
			Icon_Open = Con_Icon_sml_open
			Icon_Clsd = Con_Icon_sml_clsd
			WriteTableHeaderFromFieldArray fieldArray, Application("PlateFieldMap"),0
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
		BuildListViewHeaderFromFieldArray fieldArray, Application("PlateFieldMap"), 0
	End if	
	BaseIDList = ""
End if
%>	
<!--#INCLUDE VIRTUAL ="/cfserverasp/source/recordset_vbs.asp"-->
<%
	BaseIDList =  BaseIDList  &  "," & BaseID
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/PlateSQL.asp"-->
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
	if end_index = 1 then Response.Redirect "/cheminv/cheminv/cheminv_form_frset.asp?formgroup=" & formgroup & "&dbname=cheminv&formmode=edit" & "&unique_id=" & BaseRunningIndex
		
	' DGB Join to csdohitlist and Upper the sort field name
	SQL = Replace(SQL, " p , ", " p, " & Application("CHEMINV_USERNAME") & ".CSDOHitlist, ")
		SQL= SQL & _ 
			  "p.ROWID = " & Application("CHEMINV_USERNAME") & ".CSDOHitlist.ID " & _
			"AND " & _ 
			  Application("CHEMINV_USERNAME") & ".CSDOHitlist.HitlistID = ?"  & _
			" ORDER BY Upper(" & SortbyFieldName & ") " & SortDirectionTxt  
	
	' DGB Execute via command
	'Response.Write(SQL)
	'Response.ENd
	Call GetInvCommand("placeholdertext", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, Session("hitlistID" & "ChemInv" & formgroup))
	if bdebugPrint then
		Response.Write "<BR><BR><BR><BR>" & SQL & "<BR>"
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
	
	
	Set RS= Server.CreateObject("ADODB.Recordset")
	RS.cursorlocation= aduseclient
	RS.cachesize=5
	RS.open Cmd
	TotalContainers = RS.RecordCount
	if NOT (RS.BOF AND RS.EOF) then 
		RS.movefirst
		RS.pagesize=mypagesize
		maxcount=cint(RS.pagecount)
		RS.absolutepage=mypage
	End if
	ContainerCount =0
	if maxCount > 1 then
		response.write PageNavBar2()
	end if
	If RS.EOF AND RS.BOF then bNoContainers = True
		
	aindex = 0
	basearray = Session("Base_RS" & dbkey & formgroup)	
	Do While Not RS.EOF AND ContainerCount < RS.pagesize
	
		Barcode = RS("Plate_Barcode")
		plateID = RS("plate_ID")
		' Resort the basearray
		basearray(0,aindex) = plateID
		Session("Base_RS" & dbkey & formgroup)(aindex) = plateID
		if Session("bMultiSelect") then
			chckedStr = ""
			if multiSelect_dict.Exists(CStr(plateID)) then chckedStr = "checked"
			'chkboxStr = "<span id=""_" & Barcode & """><input type=checkbox name=""selectChckBox"" " & chckedStr &  " value=""" & ContainerID & """ onclick=""Removals('" & ContainerID & "', this.checked);SelectMarked();""" & disableChkBoxes &  "></span>" & Barcode
			chkboxStr = "<span id=""_" & Barcode & """><input type=checkbox name=""selectChckBox"" " & chckedStr &  " value=""" & plateID & """ onclick=""Removals('" & plateID & "', this.checked);updateDict();""" & disableChkBoxes &  "></span>" & Barcode
			Set Listx = ListView.ListItems.Add(, , chkboxStr, Con_Icon_lrg_clsd, , , , Container_Name)
		Else
			Set Listx = ListView.ListItems.Add(, , Barcode, Con_Icon_lrg_clsd, Con_Icon_sml_clsd, , , Barcode)
			'Set Listx = ListView.ListItems.Add(, , plateID, Con_Icon_lrg_clsd, Con_Icon_sml_clsd, , , plateID)
			Listx.URL = "javascript:goFormView('/cheminv/cheminv/cheminv_form_frset.asp?formgroup=" & formgroup &  "&dbname=cheminv&formmode=edit&unique_id=" & aindex + 1 &  "' ,'" & aindex + 1 & "')"
			'Listx.DHTML = "id=""e" & ContainerID & """ onclick=""SelectContainer(this, " & ContainerID & ", " & RS("Location_ID_FK") & "); return false;"" onmouseover=""javascript:this.style.cursor='hand'"" onmouseout=""javascript:this.style.cursor='default'"""
			
			'Set Listx = ListView.ListItems.Add(, , Barcode, Con_Icon_lrg_clsd, Con_Icon_sml_clsd, , , RS("Container_Name"))
			'Listx.URL = "javascript:goFormView('/cheminv/cheminv/cheminv_form_frset.asp?formgroup=" & formgroup &  "&dbname=cheminv&formmode=edit&unique_id=" & aindex + 1 &  "' ,'" & aindex + 1 & "')"
			''Listx.DHTML = "onKeyPress=""javascript:alert('test');"""

		End if

		if lview = 3 then
			AddListViewSubItemsFromFieldArray(fieldArray)
		End if
		RS.MoveNext
		ContainerCount = ContainerCount + 1 
		aindex = aindex + 1
	Loop
	Session("Base_RS" & dbkey & formgroup) = basearray		
	CloseRS(BaseRS)
	CloseConn(DataConn)
	
	%>
	<div align="left" id="report" style="POSITION:Absolute;top:<%=ListPositionHeight%>;left:0;  z-index:1">
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
		url = "plates_result_list.asp?formgroup=" + "<%=formgroup%>" + "&formmode=" + "<%=formmode%>" + "&dbname=cheminv" + "&BaseCurrentIndex=" + "<%=current_index%>" + "&BaseRecordCount=" + "<%=totalrecords%>" + "&dbsearchnames=cheminv" + "&SortByFieldName=" + fieldName;
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
				document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>";
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
