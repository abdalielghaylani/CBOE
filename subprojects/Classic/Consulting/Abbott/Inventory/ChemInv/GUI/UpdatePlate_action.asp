<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
QueryString = ""
ValuePairs = ""
For each field in Request.Form
	if InStr("iPlateIDs,multiscan,multiSelect,fieldList", field) = 0 then
		Select case lcase(field)
			case "ift_cycles","istatus_id_fk","ilibrary_id_fk","iconcentration","iconc_unit_fk","iqty_initial","iqty_remaining","iqty_unit_fk","iweight","iweight_unit_fk","isolvent_id_fk","isolvent_volume_unit_id_fk","iplate_type_id_fk"
				ValuePairs = ValuePairs & mid(field,2) & "%3D" & Request(field) & "::"
			case "ift_cycles_increment"
				ValuePairs = ValuePairs & "ft_cycles%3Dft_cycles" & server.URLEncode(Request(field)) & "::"	
			case "igroup_name","isupplier","ifield_1","ifield_2","ifield_3","ifield_4","ifield_5"
				ValuePairs = ValuePairs & mid(field,2) & "%3D'" & server.URLEncode(replace(Request(field),"'","''")) & "'::"
			case "idate_1","idate_2"
				ValuePairs = ValuePairs & mid(field,2) & "%3D TO_DATE('" & Request(field) & "','" & Application("DATE_FORMAT_STRING") & "')::"
		end select
	end if
next
QueryString = QueryString & "PlateIDs=" & Request("iPlateIDs")
QueryString = QueryString & "&ValuePairs=" & Left(ValuePairs, len(ValuePairs) - 2)
QueryString = QueryString & Credentials
'Response.Write "<BR>" & QueryString & "<BR>"
'Response.End

'Update the plates
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdatePlate.asp", "ChemInv", QueryString)
out = httpResponse

'Response.Write out & "=out<BR>"
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update an Inventory Plate</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align=center>
<%
			If isNumeric(out) then
				if Clng(out) = 0 then
					theAction = "Success"
				Else
					theAction = "WriteAPIError"
				End if
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
			Select Case theAction
				Case "Success"
					Session("bMultiSelect") = false
					plate_multiSelect_dict.RemoveAll()
					Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & Session("CurrentLocationID") & ", 0, '" & Session("TreeViewOpenNodes1") & "',0,1); opener.focus(); window.close();</SCRIPT>" 
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>Plate(s) could not be updated.</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
