<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<%
Dim Conn
Dim ShapeConn
Dim Cmd
Dim RS
Dim bNoWells
Dim SelectWell
Dim SortbyFieldName
Dim SortDirectionTxt
Dim SortDirection
Dim headerTxt

bDebugPrint = false
ContainerCount = 0
nameLength =10
ShowWells = true

Response.ExpiresAbsolute = Now()

WellList = Request("WellList")
lView = Request.QueryString("view")
If lView = "" Then lView = 3


bWells = false
if len(WellList)>0 then bWells = true

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
If NOT IsArray(Session("WellReportFieldArray1")) then
	ColDefStr = GetUserProperty(Session("UserNameCheminv"),"WellChemInvFA1")
	If ColDefStr = "" OR IsNull(ColDefStr) then 
		' Default column definition
		ColDefstr= Application("WellReportColDef1")
		rc= WriteUserProperty(Session("UserNameCheminv"), "WellChemInvFA1", ColDefstr)
	End if
	fieldArray = GetFieldArray(colDefstr, Application("WellFieldMap"))
	Session("WellReportFieldArray1") = fieldArray
Else
	fieldArray = Session("WellReportFieldArray1")
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
			var WellList = '<%=WellList%>';
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
				self.location.href = "BuildWellList.asp?WellList=" + WellList;
			}
		</SCRIPT>
</HEAD>
<BODY TOPMARGIN=0 LEFTMARGIN=0 BGCOLOR="#FFFFFF">
<form name="form1" method="POST">
<input type="hidden" name="WellList" VALUE="<%=WellList%>">
<div align="left" id="header" style="POSITION:Absolute;top:0;left:0;background-color:white;visibility:visible;z-index:2">
	<TABLE cellspacing=0 cellpadding=0>	
		<TR>
			<td align=left nowrap>
				<Span id="mySpan">&nbsp;</Span>
			</td>
			<td align="right" nowrap>
				<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('../cheminv/columnPicker2.asp?ArrayID=1&showWells=true', 'CCDiag', 4); return false">Column Chooser</a>
					|
				<a class="MenuLink" href="#" title="Click to export to text file" onclick="OpenDialog('ExportToFileFrame.asp?WellList=<%=WellList%>', 'ExpDiag', 4); return false">Export to File</a>
			</td>
		</tr>
	</table>	
<% 
	Icon_Open = Con_Icon_sml_open
	Icon_Clsd = Con_Icon_sml_clsd
	WriteTableHeaderFromFieldArray2 fieldArray, Application("WellFieldMap"), 0, 7.91
	nameLength = 50
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
<%if bWells then%>
	
<%
Set ListView = Server.CreateObject("VASPLV.ASPListView")

	ListView.Class = "ListView"
	ListView.ImagePath = ImagePath
	ListView.View = lView
	ListView.Width = 730
	ListView.Height = 80
	ListView.Gridlines = True
	ListView.ColumnHeaderForeColor = ""
	ListView.ColumnHeaderBackColor = ""
	ListView.HideColumnHeaders = True
	ListView.LabelWrap = False
	ListView.BackColor = ""
	ListView.Picture = ""
	ListView.Licensekey = "1C71-2CE6-A49C"

	BuildListViewHeaderFromFieldArray fieldArray, Application("WellFieldMap"), 0
	RowCount = 0
' The include file below populates SQL with the SQL string needed to fetch well report data
%>	
<!--#INCLUDE VIRTUAL = "/cheminv/gui/WellSQL.asp"-->
<%		
	SQL = SQL & "w.well_id in (" & WellList & ")"
	ParentWellSQL = ParentWellSQL & " AND w.well_id in (SELECT wp.parent_well_id_fk FROM inv_well_parent wp, inv_wells w WHERE w.well_id = wp.child_well_id_fk and w.well_id in (" & WellList &"))"
	WellCompoundSQL = WellCompoundSQL & " well_id_fk in (Select Well_id from inv_wells where well_id in (" & WellList & "))"
	'WellCompoundSQL = WellCompoundSQL2 & " well_id_fk in (Select Well_id from inv_wells where well_id in (" & WellList & "))"
	GetInvShapeConnection()
	'WellCompoundSQL = "select * from inv_well_compounds wc"

	Shapestr = "SHAPE {" & SQL & "}" & _
				" APPEND ((SHAPE {SELECT wp.parent_well_id_fk, wp.child_well_id_fk FROM inv_well_parent wp, inv_wells w where child_well_id_fk = well_id and well_id in (Select Well_id from inv_wells where well_id in (" & WellList & "))} as rsWellParent" & _
					" APPEND({" & ParentWellSQL & "} as rsParent" & _               
					" RELATE parent_well_id_fk to well_id))" & _
				" RELATE well_id TO child_well_id_fk) as rsWellParent, " & _
				" ({" & WellCompoundSQL & "} RELATE well_id TO well_id_fk) as rsCompound"
				'" APPEND ({SELECT * from inv_well_parent where child_well_id_fk in (" & WellList & ")} RELATE well_id to child_well_id_fk) as rsParent," & _
				'" ({SELECT * FROM inv_plates} RELATE plate_id_fk to plate_id) as rsPlate"

	'Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & shapestr
	'Response.End                
	Set ShapeCmd = Server.CreateObject("ADODB.Command")
	ShapeCmd.ActiveConnection = ShapeConn
	ShapeCmd.commandtext = Shapestr
	
	If bDegbugPrint then
		Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & SQL
		Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & ParentWellSQL
		Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & WellCompoundSQL
		Response.end	
	End if
	
	mypage=request.querystring("whichpage")
	If mypage="" then
		mypage=1
	end if
		mypagesize = Session("ScreenReportPageSize3")
		If mypagesize="" then
			mypagesize = GetUserProperty(Session("UserNameCheminv"),"ScreenReportPageSize3")
			if mypagesize = "" or IsNull(mypagesize) then
				mypagesize= 100
				rc= WriteUserProperty(Session("UserNameCheminv"), "ScreenReportPageSize3" , mypagesize)
			End if
			Session("ScreenReportPageSize3") = mypagesize
		End if
	end if
	Set RS= Server.CreateObject("ADODB.Recordset")
	RS.cursorlocation= aduseclient
	RS.cachesize=5
	'ShapeConn.Execute("Alter session set sql_trace = true")
	' The shape recordset is obtained by executing on the command object
	RS.open ShapeCmd
	'ShapeConn.Execute("Alter session set sql_trace = false")
	
	TotalWells = RS.RecordCount
	if NOT (RS.BOF AND RS.EOF) then 
		RS.movefirst
		RS.pagesize=mypagesize
		maxcount=cint(RS.pagecount)
		RS.absolutepage=mypage
	End if
	WellCount =0
	'Response.Write TotalWells
	'Response.End
	
	If RS.EOF AND RS.BOF then bNoWells = True

	Do While Not RS.EOF AND WellCount < RS.pagesize
		WellID = RS("Well_ID")
		PlateBarcode = RS("Plate_Barcode")

		Set Listx = ListView.ListItems.Add(, , WellID, ,, , , WellID)
		'Listx.URL="View well details"
		Listx.URL = "/cheminv/cheminv/BrowseInventory_frset.asp?PostRelay=1&GotoNode=" & RS("location_id_fk") & "&SelectContainer=" & RS("plate_id_fk") & "&SelectWell=" & WellID & "&ClearNodes=0"

		'get parent well information
		Set rsWellParent = RS("rsWellParent").Value
		arrParentWellInfo = split(GetParentWellInfoString(rsWellParent,"<BR>"),"|")
		ParentWellLinks = arrParentWellInfo(0)
		ParentPlateLinks = arrParentWellInfo(1)
		
		Set rsCompound = RS("rsCompound").Value
		
		'Response.Write "<BR><BR><BR><BR><BR><BR><BR><BR><BR><BR>" & ParentWellLinks & ":test"
		'Response.End
		For i=0 to Ubound(fieldArray)
			fn = Cstr(fieldArray(i,0))
			if inStr(1,fn,"inv_compounds.") > 0 then
				fn = Replace(fn, "inv_compounds.", "")
			end if
			'Response.Write fn & ":" & RS(fn)
			
			select case lcase(fn)
				case "compound_id_fk"
					compoundIDText = ""
					'Response.Write "<BR><BR><BR><BR><BR><BR>" & rsCompound.BOF & ":" & rsCompound.EOF
					if rsCompound.EOF and not rsCompound.EOF then rsCompound.MoveFirst
					While not rsCompound.EOF 
						compoundIDText = compoundIDText & rsCompound("compound_id_fk") & "<BR>"
						rsCompound.movenext
					Wend
					Listx.ListSubItems.Add , , compoundIDText
				case "reg_batch_id"
					regBatchIDText = ""
					if rsCompound.EOF and not rsCompound.EOF then rsCompound.MoveFirst
					While not rsCompound.EOF 
						compoundIDText = compoundIDText & rsCompound("compound_id_fk") & "<BR>"
						rsCompound.movenext
					Wend
					Listx.ListSubItems.Add , ,regBatchIDText
				case "parent_well_names"
					Listx.ListSubItems.Add , ,ParentWellLinks
				case "parent_plate_barcodes"
					Listx.ListSubItems.Add , ,ParentPlateLinks
				case else
					Listx.ListSubItems.Add , ,TruncateInSpan(RS(fn), CInt(fieldArray(i,2)),"") 	
			end select
		Next

		RS.MoveNext
	Loop
		
	RS.Close
	Set RS = Nothing
	ShapeConn.Close
	Set Conn = Nothing
%>	 

<div align="left" id="report" style="POSITION:Absolute;top:55;left:2;  z-index:1">
<%
	ListView.Show
	ListView.Unload
	Set ListView = Nothing
%>
</div>
</form>

</body>
</HTML>
