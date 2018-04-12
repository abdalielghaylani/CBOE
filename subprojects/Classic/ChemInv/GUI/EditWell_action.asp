<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetWellAttributes.asp"-->
<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

Dim httpResponse
Dim FormData
Dim ServerName
Dim Conn

'Response.Write "test"
'Response.End

ServerName = Application("InvServerName")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
'FormData = Request.Form & Credentials
QueryString = ""
ValuePairs = ""
fieldList = ""

wFieldList = Session("wFieldList")

'Response.Write wFieldList & "=wFieldList<BR>"
'for each item in Request.Form
'	Response.Write item & " = " & Request.Form(item) & "<BR>"
'	Response.Write Session("w" & mid(item,2)) & " = w" &  mid(item,2) & "<BR>"
'next
'Response.End
arrFields = split(wFieldList, ",")
For i = 0 to ubound(arrFields)
	field = arrFields(i)
	'Response.Write "'"& field & "':" & (len(Session(field)) > 0) & "<BR>"	
	if len(Session(field)) > 0 then
		if InStr("iContainerIDs,multiscan,multiSelect,fieldList,wCompoundCount,wParentCount", field) = 0 then
			Select case lcase(field)
				'number fields
				case "wwell_format_id_fk","wqty_unit_fk","wqty_initial","wqty_remaining","wweight_unit_fk","wweight","wconc_unit_fk","wconcentration","wcompound_id_fk","wsolvent_id_fk","wreg_id_fk","wbatch_number_fk","wsolvent_volume","wsolvent_volume_unit_id_fk","wsolution_volume"
					ValuePairs = ValuePairs & mid(field,2) & "%3D" & Session(field) & "::"
				'text fields
				case "wfield_1","wfield_2","wfield_3","wfield_4","wfield_5"
					ValuePairs = ValuePairs & mid(field,2) & "%3D'" & server.URLEncode(replace(Session(field),"'","''")) & "'::"
				'date fields
				case "wdate_1","wdate_2"
					ValuePairs = ValuePairs & mid(field,2) & "%3D TO_DATE('" & Session(field) & "','" & Application("DATE_FORMAT_STRING") & "')::"
				'don't update the field
				case else
			end select
			fieldList = fieldList & field & ","
		end if
	else
	'set empty fields to null
		if InStr("iContainerIDs,multiscan,multiSelect,fieldList", field) = 0 then
			Select case lcase(field)
				'number fields
				case "wwell_format_id_fk","wqty_unit_fk","wqty_initial","wqty_remaining","wweight_unit_fk","wweight","wconc_unit_fk","wconcentration","wsolvent_id_fk","wsolvent_volume","wsolvent_volume_unit_id_fk","wsolution_volume","wcompound_id_fk","wreg_id_fk","wbatch_number_fk"
					if lcase(field)="wcompound_id_fk" then  'Avoid deletion of the compounds of a mixture well.
					    if cint(Session("wCompoundCount")) <=1 then ValuePairs = ValuePairs & mid(field,2) & "%3DNULL::"
					else
						ValuePairs = ValuePairs & mid(field,2) & "%3DNULL::"
					end if 	
				'text fields
				case "wfield_1","wfield_2","wfield_3","wfield_4","wfield_5"
					ValuePairs = ValuePairs & mid(field,2) & "%3DNULL::"
				'date fields
				case "wdate_1","wdate_2"
					ValuePairs = ValuePairs & mid(field,2) & "%3DNULL::"
			end select
			fieldList = fieldList & field & ","
			'Response.Write field & "=field<BR>"
			'Response.Write ValuePairs & "<BR>"
		end if
	end if
next

'Response.Write "<BR>fieldList=" & fieldList & "<BR>"
'Response.Write "<BR>valuePairs=" & ValuePairs & "<BR>"
'Response.End

QueryString = QueryString & "WellIDs=" & Request("iWellIDs")
QueryString = QueryString & "&ValuePairs=" & Left(ValuePairs, len(ValuePairs) - 2)
'Response.Write "<BR>" & QueryString & "<BR>"
'Response.End
QueryString = QueryString & Credentials

'Update the wells
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/updateWell.asp", "ChemInv", QueryString)
out = httpResponse
'Response.Write out
'Response.End
httpResponse1 = CShttpRequest2("POST", ServerName, "/cheminv/api/SetAggregatedPlateData.asp", "ChemInv", "PlateIDs=" & Session("plPlate_ID")& Credentials)
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update a Well</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<SCRIPT LANGUAGE="javascript">
function SelectLocationNodeFromEditWell(bClearNodes, LocationID, RemoveLocationID, openNodes, SelectContainer, IsPlateView, SelectWell){
	//alert(openNodes)
	
	var url = "/cheminv/cheminv/BrowseTree.asp?" + "ClearNodes=" + bClearNodes + "&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&GotoNode=" + LocationID +  "&sNode=" + LocationID + "&SelectContainer=" + SelectContainer + "&SelectWell=" + SelectWell;
	if (RemoveLocationID){
		url += "&RemoveNode=" + RemoveLocationID;
	}
	
	if (openNodes){
		url += openNodes;
	}
	url += "&Exp=Y#" + LocationID;

	//this is the line that is diff for well editing
	var thetop = opener.opener.parent.parent;	
	if (IsPlateView) {
		thetop = thetop.parent;
	}
	if (thetop.name == "main"){
		// Tree is in search results mode
		var theTreeFrame =  thetop.mainFrame;
		if (theTreeFrame) theTreeFrame.location.reload();
	}
	else if (thetop.TreeFrame){
		//Tree is in Browse mode
		var theTreeFrame = thetop.TreeFrame;
		tempURL = theTreeFrame.location.pathname + theTreeFrame.location.search + theTreeFrame.location.hash 
		theTreeFrame.location.href = url;
		if (tempURL == url){
			if (theTreeFrame) theTreeFrame.location.reload();
		}
	}
	else{
		opener.location.reload();
	}
}
</SCRIPT>

<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE>
			<% 
			If isNumeric(httpResponse) then
				If CLng(httpResponse) = 0 then
					Response.Write "<center><SPAN class=""GuiFeedback"">Well has been updated</SPAN></center>"
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNodeFromEditWell(0, " & Session("plLocation_ID_FK") & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & Session("plPlate_ID") & ",1," & Request("iWellIDs") & ");opener.close(); window.close();</SCRIPT>" 
					Response.Write "<SCRIPT LANGUAGE=javascript></SCRIPT>" 
				else				
					Response.Write "<center><P><CODE>" & Application(httpResponse) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback"">Well could not be updated</SPAN>"
					Response.Write "<P><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				End if
			Else
					Response.Write "<P><CODE>Oracle Error: " & httpResponse & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End if
			%>
		</TD>
	</TR>
</TABLE>
</Body>
