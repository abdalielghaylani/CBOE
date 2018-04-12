<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<%
Dim Conn
Dim Cmd
Dim RS
Dim bNoLocations
Dim bNoContainers
Dim SelectContainer
Dim SelectWell
Dim CurrentContainer
Dim SortbyFieldName
Dim SortDirectionTxt
Dim SortDirection
Dim showPlates
Dim headerTxt

bDebugPrint = false
bNoLocations = true
bNoContainers = false
ContainerCount = 0
nameLenght =10

Response.ExpiresAbsolute = Now()

LocationID = Request.QueryString("LocationID")
showPlates = Request("showplates")
QS = Request.QueryString
QS = Replace(QS, "view=" & Request.QueryString("View") & "&" ,"")
QS = Replace(QS, "&multiSelect=" & Request.QueryString("multiSelect"),"")
QS = Replace(QS, "multiSelect=" & Request.QueryString("multiSelect") & "&","")
QS = Replace(QS, "&reconcile=" & Request.QueryString("reconcile"),"")
QS = Replace(QS, "&showInList=" & Request.QueryString("showInList"),"")
QS = Replace(QS, "&showplates=" & showPlates,"")
QS = Replace(QS, "&SortByFieldName=" & Request.QueryString("SortByFieldName"),"")
lView = Request.QueryString("view")

If lView = "" Then lView = 3
MultiSelect = Request.QueryString("multiSelect")
if MultiSelect = "0" then Session("bMultiSelect") = false
reconcile = Request.QueryString("reconcile")
if reconcile = "1" then
	reconcile = true 
	Session("TabFrameURL") = "reconcile.asp"
else
	reconcile = false
	Session("TabFrameURL") = ""
end if
if reconcile = "0" then Session("bReconcile") = false

if LocationID >=0  then Session("CurrentLocationID") = LocationID
CompoundID = Request.QueryString("CompoundID")
RegID = Request.QueryString("RegID")
ACXID = Request.QueryString("ACXID")
ContainerID = Request.QueryString("ContainerID")
PlateID = Request.QueryString("PlateID")
If ContainerID = "" then ContainerID = 0
LocationCount = Request.QueryString("LocationCount")
LocationName = Request.QueryString("LocationName")
if Len(LocationName) > 0 then Session("CurrentLocationName") = LocationName
SelectContainer = Request.QueryString("SelectContainer")
if Session("SelectWell") <> Session("lastSelectedWell") then
	SelectWell = Session("SelectWell")
	'Session("lastSelectedWell") = SelectWell
else
	Session("SelectWell") = "0"
end if
CurrentContainerID = Session("CurrentContainerID")
If CurrentContainerID = "" then CurrentContainerID = 0
if SelectContainer = "" OR SelectContainer="undefined" then SelectContainer = 0
If SelectWell = "" OR SelectWell="undefined" OR IsEmpty(SelectWell) then 
	SelectWell = 0
else
	Session("sPlateTab") = "PlateViewer"
end if
if Len(LocationID) >0 then LocIDParam= "LocationID=" & LocationID & "&"
showInList = lcase(Request("showInList"))
if showPlates = "1" then showInList = "plates"
if showInList = "" then 
	showInList = Session("showInList")
Else
	Session("showInList") = showInList
End if
If showInList = "" then showInList = "containers"

if Application("PLATES_ENABLED") AND LocationCount = "" then
		Call GetInvCommand("SQLText", adCmdText)
		Cmd.CommandText = "SELECT count(container_id) AS ContainerCount from " & Application("CHEMINV_USERNAME") & ".inv_containers where location_id_fk=?"
		Cmd.Parameters.Append Cmd.CreateParameter(, 131, 1, 0, LocationID)
		Set RS = Cmd.Execute
		ContainerCount = CLng(RS("ContainerCount").Value)
		Cmd.CommandText = "SELECT count(plate_id) AS PlateCount from " & Application("CHEMINV_USERNAME") & ".inv_plates where location_id_fk=? AND plate_type_id_fk <> -1"
		Set RS = Cmd.Execute
		PlateCount = CLng(RS("PlateCount").Value)	
		if ContainerCount = 0 and PlateCount > 0 then
			showInList = "plates"
		end if	
		if PlateCount = 0 and ContainerCount > 0 then
			showInList = "containers"
		end if
		cLink = ""
		if showInList = "containers" AND ContainerCount > 0 then
			cLink = ContainerCount & "&nbsp;" & Pluralize("container", ContainerCount)
		elseif ContainerCount > 0 then
			cLink = ContainerCount & "&nbsp;" & "<a class=""menuLink"" href=""BuildList.asp?view=" & Request.QueryString("View") & "&" & QS & "&showInList=containers"">" & Pluralize("container", ContainerCount) & "</a>"
		end if
		pLink = ""
		if showInList = "plates" AND PlateCount > 0 then
			pLink = PlateCount & "&nbsp;" & Pluralize("plate", PlateCount)
		elseif PlateCount > 0 then
			pLink = PlateCount & "&nbsp;" & "<a class=""menuLink"" href=""BuildList.asp?view=" & Request.QueryString("View") & "&" & QS & "&showInList=plates"">" & Pluralize("plate", PlateCount) & "</a>"
		end if	
		
		headerTxt =  cLink
		if headerTxt <> "" AND pLink <> "" then headerTxt = headerTxt & " and "
		headerTxt = headerTxt & pLink
		if headerTxt <> "" then headerTxt = "Found " & headerTxt
		if PlateCount + ContainerCount = 0 then showInList = "" 
End if
if showplates = "1" then showInList = "plates"

Function Pluralize(str, count)
	if count = 1 then 
		Pluralize =  str
	else
		Pluralize = str & "s"
	end if		
End function

if showInList = "" then showInList = "containers"
Session("showInList") = showInList
if showInList = "plates" then
	if MultiSelect = "1" or Session("bMultiSelect") = true then 
		Session("bMultiSelect") = true
		Session("TabFrameURL") = "multiselect_plate.asp"
		if instr(QS,"multiSelect=") = 0 then QS = QS & "&multiSelect=1"
	End if
%> 
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/BuildPlateList.asp" -->
<%Elseif showInList = "containers" then
	if MultiSelect = "1" or Session("bMultiSelect") = true then 
		Session("bMultiSelect") = true
		Session("TabFrameURL") = "multiselect.asp"
		if instr(QS,"multiSelect=") = 0 then QS = QS & "&multiSelect=1"
	End if	
%>
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/BuildContainerList.asp" -->
<%else
	Response.Write "<BR><BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP><span class=""GuiFeedback"">There are no items to display at this location.</span></TD></TR></TABLE>"
	Response.Write "<SCRIPT language=javascript>if (parent.TabFrame) parent.TabFrame.location.href = '/cheminv/cheminv/SelectContainerMsg.asp?entity=none';</SCRIPT>"

End if	%>
<%

%>



