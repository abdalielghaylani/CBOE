<%
showPlates = 1
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
If NOT IsArray(Session("PlateReportFieldArray2")) then
	ColDefStr = GetUserProperty(Session("UserNameCheminv"),"PlateChemInvFA2")
	If ColDefStr = "" OR IsNull(ColDefStr) then 
		' Default column definition
		ColDefstr= Application("PlateReportColDef2")
		rc= WriteUserProperty(Session("UserNameCheminv"), "PlateChemInvFA2", ColDefstr)
	End if
	fieldArray = GetFieldArray(colDefstr, Application("PlateFieldMap"))
	Session("PlateReportFieldArray2") = fieldArray
Else
	fieldArray = Session("PlateReportFieldArray2")
End if
%>

<HTML>
	<HEAD>
	<SCRIPT LANGUAGE=javascript src="/cheminv/utils.js"></SCRIPT>
	<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
		<STYLE>
			A {Text-Decoration: none;}
			.ListView {color:#000000; font-size:8pt; font-family: Lucida Console}
			A.ListView:LINK	{Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
			A.ListView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: arial}
			A.ListView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial
		</STYLE>
		<SCRIPT LANGUAGE=JAVASCRIPT>
			var PrevFolderImage
			var PrevLink
			var openNodes = "<%=Session("TreeViewOpenNodes1")%>";
			///////////////////////////////////////////////////////////////////////////////
			//
			//
			function OpenFolder(elm, ImageOpen, ImageClsd)
			{
				elm.firstChild.src = ImageOpen;
				elm.style.color = "black"
				elm.style.fontWeight = "bold"
				if ((typeof(PrevFolderImage)== "object") && (PrevFolderImage != elm.firstChild) )
				{
					PrevFolderImage.src = ImageClsd;
					PrevLink.style.color = "#4682b4";
					PrevLink.style.fontWeight = ""
				}
				PrevFolderImage = elm.firstChild;
				PrevLink = elm;
			}
			///////////////////////////////////////////////////////////////////////////////
			//
			//
			function RefreshList(LocationID)
			{
				self.location.href = "BuildList.asp?LocationID=" + LocationID;
			}
			///////////////////////////////////////////////////////////////////////////////
			//
			//
			function RefreshLocationBar(LocationPath){
				if(top.bannerFrame){
					if (top.bannerFrame.LocationBar.LocationBox) {
						top.bannerFrame.LocationBar.LocationBox.value = LocationPath;
					}
				}
			}
		</SCRIPT>
</HEAD>
<BODY TOPMARGIN=0 LEFTMARGIN=0 BGCOLOR="#FFFFFF" onunload="top.window.defaultStatus=''">

<bgsound src="" id="snd1">
<form name="form1" method="POST" target="TabFrame">
<input type="hidden" name="SelectWell" VALUE="<%=SelectWell%>">
<input type="hidden" name="removeList">
<div align="left" id="header" style="POSITION:Absolute;top:0;left:0;background-color:white;visibility:visible;z-index:2">
	<TABLE cellspacing=0 cellpadding=0>	
		<TR>
			<td align=left nowrap>
				<Span id="mySpan">&nbsp;</Span>
			</td>
			<td align="right" nowrap>
			<%if Application("UseCustomListFrameLinks") then%>
				<!--#INCLUDE VIRTUAL = "/cheminv/custom/cheminv/list_frame_links.asp"-->
			<%else%>
				<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/list_frame_links.asp"-->
			<%end if%>	
			</td>
		</tr>
	</table>	
<% 
if lview > 0 then
	Icon_Open = Con_Icon_sml_open
	Icon_Clsd = Con_Icon_sml_clsd
End if
if lview = 3 then 
	WriteTableHeaderFromFieldArray fieldArray, Application("PlateFieldMap"), 0
	nameLenght = 50
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
Set ListView = Server.CreateObject("VASPLV.ASPListView")

	ListView.Class = "ListView"
	ListView.ImagePath = ImagePath
	ListView.View = lView
	ListView.Width = 730
	ListView.Height = 80
	ListView.Gridlines = False
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

if Not IsEmpty(LocationID) then
	Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GETLOCATIONPATH", adCmdStoredProc)	
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 2000, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pLocationID", adnumeric, 1, 0, LocationID)
	Cmd.Execute()
	
	LocationPath = Trim(Cmd.Parameters("RETURN_VALUE"))
	'LocationPath = Left(LocationPath, Len(LocationPath)-1) 'remove the backslash
	'Response.Write "<BR><BR><BR>" & LocationPath
	'Response.end
	Response.Write "<script language=javascript>RefreshLocationBar(""" & Replace(LocationPath, "\","\\") & """);</script>"
	Session("CurrentLocationName") = LocationPath
End if

if (IsEmpty(LocationID) OR Len(LocationID)=0)AND IsEmpty(CompoundID) AND IsEmpty(PlateID)AND IsEmpty(RegID) AND IsEmpty(ACXID) then
	if reconcile then
		msg = "Scan barcode or browse to the location to reconcile."
	Else
		msg = "Select a location from the left frame"
	end if
	Response.Write "<BR><BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP><span class=""GuiFeedback"">" & msg & "</span></TD></TR></TABLE>"
Else
	RowCount = 0
	cPlural = "s"
	slPlural = "s"
' The include file below populates SQL with the SQL string needed to fetch palte report data
%>	
<!--#INCLUDE VIRTUAL = "/cheminv/gui/PlateSQL.asp"-->
<%		
	Call GetInvCommand("SQLText", adCmdText)
	If CLng(PlateID) > 0 then
		SQL = SQL & " p.Plate_ID=?"
		Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, PlateID)
	Else
		if Len(LocationID) > 0 then
			SQL = SQL & " p.Location_ID_FK =?"
			Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, LocationID)	
		End if
		if Len(CompoundID) > 0 then
			SQL = Replace(SQL, " p ", " p , " & schemaName & ".inv_wells w ")
			SQL = SQL & " p.plate_id = w.plate_id_fk AND w.Compound_ID_FK = ?"
			Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, CompoundID)
		End if
		if Len(RegID) > 0 then
			SQL = SQL & " AND Inv_Containers.Reg_ID_FK = ?"
			Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, RegID)
		End if
		if Len(ACXID) > 0 then
			SQL = SQL & " AND Inv_Compounds.ACX_ID = ?"
			Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 20, ACXID)
		End if
	End if
	'DGB Uppercase the sorting field
	SQL = SQL & " ORDER BY Upper(" & SortbyFieldName & ") " & SortDirectionTxt
	Cmd.CommandText = SQL
	
	If bDebugPrint then
		Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & SQL
		Response.end	
	End if
	
	mypage=request.querystring("whichpage")
	If mypage="" then
		mypage=1
	end if
	'!DGB! make the pagesize huge when roconciling containers
	if reconcile then
		mypagesize = 10000
	else	
		mypagesize = Session("ScreenReportPageSize2")
		If mypagesize="" then
			mypagesize = GetUserProperty(Session("UserNameCheminv"),"ScreenReportPageSize2")
			if mypagesize = "" or IsNull(mypagesize) then
				mypagesize= 100
				rc= WriteUserProperty(Session("UserNameCheminv"), "ScreenReportPageSize2" , mypagesize)
			End if
			Session("ScreenReportPageSize2") = mypagesize
		End if
	end if
	Set RS= Server.CreateObject("ADODB.Recordset")
	RS.cursorlocation= aduseclient
	RS.cachesize=5
	RS.open Cmd
	TotalContainers = RS.RecordCount
	if NOT (RS.BOF AND RS.EOF) then 
		'-- Jump to correct page of recordset for the selected plate
		if SelectContainer <> "" and SelectContainer <> 0 and request.querystring("whichpage") = "" then
			cntrid = SelectContainer
			containersList = RS.GetRows()
			cntrpage = 1
			cntrpagectr = 0
			For i = 0 to uBound(containersList,2)
				tValue=cStr(containersList(0,i))
				If cStr(cntrid) = tValue then
					'If (i/cntrpage)=mypagesize then cntrpage = cntrpage + 1 end if 
					cntrpage = (i\mypagesize)+1
					exit for
				else
					cntrpagectr = cntrpagectr + 1
					if cntrpagectr = mypagesize then
						cntrpage = cntrpage + 1
						cntrpagectr = 0
					end if
				end if
			Next
			mypage = cntrpage
		end if

		RS.movefirst
		RS.pagesize=mypagesize
		maxcount=cint(RS.pagecount)
		RS.absolutepage=mypage
	End if
	ContainerCount =0
	
	if maxCount > 1 then
		response.write PageNavBar()
	end if
	If RS.EOF AND RS.BOF then bNoContainers = True

	Do While Not RS.EOF AND ContainerCount < RS.pagesize
		PlateID = RS("Plate_ID")
		PlateBarcode = RS("Plate_Barcode")
		Refresh = "True"
		If cLng(PlateID) = cLng(SelectContainer) AND SelectWell <> 0 then Refresh = "False"
		
		if Session("bMultiSelect") OR reconcile then
			chckedStr = ""
			bDisableChkBoxes = Application("DisableCheckBoxesDuringRectification") AND reconcile
			if bDisableChkBoxes then disableChkBoxes = " disabled "
			if plate_multiSelect_dict.Exists(CStr(PlateID)) then chckedStr = "checked"
			chkboxStr = "<span id=""_" & RS("Plate_Barcode") & """><input type=checkbox name=""selectChckBox"" " & chckedStr &  " value=""" & PlateID & """ onclick=""Removals('" & PlateID & "', this.checked);SelectMarked();""" & disableChkBoxes &  "></span>" & PlateBarcode
			Set Listx = ListView.ListItems.Add(, , chkboxStr, Con_Icon_lrg_clsd, , , "TabFrame", RS("Plate_Name"))
		Else
			Set Listx = ListView.ListItems.Add(, , PlateBarcode, Con_Icon_lrg_clsd, Con_Icon_sml_clsd, , "TabFrame", PlateBarcode)
			Listx.URL="View plate details"
			Listx.DHTML = "id=""e" & PlateID & """ onclick=""SelectPlate(this, " & PlateID & ", " & RS("Location_ID_FK") & ",'" & Refresh & "'); return false;"" onmouseover=""javascript:this.style.cursor='hand'"" onmouseout=""javascript:this.style.cursor='default'"" name=""" & PlateID & """"
			Listx.Target = "_self"
		End if
		if lview = 3 then
			Call AddListViewSubItemsFromFieldArray(fieldArray)
		End if
		
		' Select the current plate by default
		if (SelectContainer = 0) AND (TotalContainers = 1) then SelectContainer = CLng(PlateID) 

		RS.MoveNext
		ContainerCount = ContainerCount + 1 	
	Loop
		
	if ContainerCount = 1 then cPlural = ""

	RS.Close
	Set RS = Nothing
	Conn.Close
	Set Conn = Nothing
		
	If IsEmpty(LocationCount)OR Len(LocationCount) = 0 then 
		If (bNoLocations AND bNoContainers)OR (NOT IsEmpty(LocationCount) AND LocationCount = "" ) then 
			if NOT (Session("bMultiSelect") OR Reconcile) then
				Response.Write "<BR><BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP><span class=""GuiFeedback"">There are no plates to display at this location.</span></TD></TR></TABLE>"
				Response.Write "<SCRIPT language=javascript>if (parent.TabFrame) parent.TabFrame.location.href = '/cheminv/cheminv/SelectContainerMsg.asp?entity=plate';</SCRIPT>"
				Response.end
				strTemp = ""
			end if
		Else
			strTemp = "Found " & totalContainers & " plate" & cplural
		End if
	Else		
		lPlural = "s"
		if CLng(LocationCount) = 1 then lPlural = ""
		strTemp = "Found " & TotalContainers & " plate" & cplural &  " in " & LocationCount & " location" & lplural & "."
	End if

	if ContainerCount > 0 then showMultiSelect = "if (document.all.multiSelectLink) document.all.multiSelectLink.style.visibility='visible';"
	' Autoload a container into the tab frame
	Response.Write "<SCRIPT LANGUAGE=javascript>top.window.defaultStatus ='" & strTemp & "';" & showMultiSelect & "</SCRIPT>"
	Response.Write "<Script language=javascript for=window event=onload>"
	if ContainerCount = 1 OR CLng(SelectContainer)>0 then
		if CLng(SelectContainer)>0 then ContainerID = SelectContainer
		theLink = "document.anchors(""e" & CLng(ContainerID) & """)"
		response.write "if (" & theLink & ") " & theLink & ".click()"
	Else
		if NOT (Session("bMultiSelect") OR reconcile) then response.write "if (parent.TabFrame) parent.frames['TabFrame'].location.href = 'SelectContainerMsg.asp?entity=plate';"
	End if
	Response.Write "</SCRIPT>"
%>	 

<div align="left" id="report" style="POSITION:Absolute;top:55;left:2;  z-index:1">
	<%
	ListView.Show

	ListView.Unload
	Set ListView = Nothing
End if
%>
</div>
<SCRIPT LANGUAGE=javascript>
function SelectPlate(elm, PlateID, locationID, Refresh){
	var ImageOpen = "<%=ImagePath%>/<%=Icon_Open%>";
	var ImageClsd = "<%=ImagePath%>/<%=Icon_Clsd%>";
	OpenFolder(elm, ImageOpen, ImageClsd);
	if (parent.TabFrame) parent.TabFrame.location.href = "/cheminv/gui/ViewPlate.asp?PlateID=" + PlateID + "&refresh=" + Refresh;
}

function SortReport(fieldName){
	var url = document.location.href;
	//url = "BuildList.asp?showplates=1&<%=LocIDParam%>CompoundID=<%=CompoundID%>&PlateID=<%=PlateID%>&LocationName=<%=LocationName%>&SortByFieldName=" + fieldName;
	url = "BuildList.asp?<%=QS%>&SortByFieldName=" + fieldName;
	document.location.href = url;
}
</SCRIPT>
</form>
</BODY>
</HTML>
<SCRIPT LANGUAGE=javascript>
	//refresh tab frame
	TabFrameURL = "<%=Session("TabFrameURL")%>";
	if (TabFrameURL != "") {
		document.form1.action="/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
		document.form1.submit();
	}
	function SelectMarked(){
				var reml = document.form1.removeList;
				var len = reml.value.length
				if (reml.value.substring(len -1 , len) == ","){
					reml.value = reml.value.substring(0, len - 1);
				}
				//alert(reml.value);
				document.form1.action = "/Cheminv/GUI/<%=Session("TabFrameURL")%>?containerCount=<%=totalContainers%>&showInList=<%=showInList%>";
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
				SelectMarked();	
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

<!--MCD: this variable is checked by reconcile.asp.  Reconcile calls SelectMarked() to
         get the number of containers in the location -->
<script LANGUAGE="JScript">
//scroll to the selected container
document.location = document.location + "#<%=SelectContainer%>";
//scroll down the length of the header
window.scrollBy(0,-(document.all.header.scrollHeight));
//show the header
slide();
var finishedLoading = true;
</script>
<!--MCD: end changes -->

