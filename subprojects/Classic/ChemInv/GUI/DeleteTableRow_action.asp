<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
Dim httpResponse
Dim ServerName
Dim Credentials
Dim FormData
Dim bDebugPrint

bDebugPrint = false

ServerName = Request.ServerVariables("Server_Name")
Credentials = "&CSUserName=" & Server.URLEncode(Session("UserName" & "cheminv")) & "&CSUSerID=" & Server.URLEncode(Session("UserID" & "cheminv"))
'FormData = Request.Form & Credentials
QueryString = ""
ValuePairs = ""
Tablename=Request("dbTableName")
QueryString = QueryString & "TableName=" & Request("dbTableName")
QueryString = QueryString & "&pkColumnName=" & Request("pkColumnName")
QueryString = QueryString & "&pkIDs=" & Server.URLEncode(Request("pkIDs")) 'CBOE-299 SJ To retain the space character in the case of Owner Id
'Response.Write "<BR>" & QueryString
'Response.End
QueryString = QueryString & Credentials

'Update the containers
httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/DeleteTableRow.asp", "ChemInv", QueryString)
out = httpResponse

Call ManageTable_Event(tableName, "delete") ' WJC

'Response.Write out
'Response.End
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete a Table Row</title>
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
			Elseif InStr(Left(out,18),"|") then
				containerList = out
				out = left(out,InStr(out,"|")-1)
				theAction = "SelectContainer"
			
			'Response.Write(theAction)
			Else
				theAction = "WriteOtherError"
			
			End if

			if trim(ucase(out)) = "CHILD DATA EXISTS FOR THIS ROW.  IT COULD NOT BE DELETED." then  
			    theAction="CustomMsg"
			 end if
			'Response.Write("<br>action:"&theAction&":"&len(out))
			Select Case theAction
				Case "Success"
					Response.Write "<script language=javascript>"
					Response.Write	"if (opener){opener.location = 'manageTables.asp?TableName=" & Request("TableName") & "'; opener.focus();}"
					Response.Write "</script>"
					Response.write "<Script language=javascript>window.close();</script>"		
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<SPAN class=""GuiFeedback""><center>Table row could not be deleted.</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "CustomMsg"
				Response.Write "<P><CODE>Oracle Error: " & CustomErrorMsg(Tablename) & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					Response.end
			End Select
			
 FUNCTION CustomErrorMsg(Tablename)
    CustomErrorMsg="Some <objects> having refrence of this <field>, It could not be deleted."
    Select Case UCase(Tablename)
        Case UCase("inv_barcode_desc")
            Obj= "Plates/Containers" 
            Field = "Barcode"
        Case UCase("inv_container_status")
             Obj= "Containers" 
             Field = "Container Status"
        Case UCase("inv_container_types")
             Obj= "Containers" 
             Field = "Container Type"
        Case UCase("inv_country")
             Obj= "Containers" 
             Field = "Country"
        Case UCase("inv_grid_format")
             Obj= "Plates/Locations" 
             Field = "Grid Format"
        Case UCase("inv_location_types")
             Obj= "Locations" 
             Field = "Location Type"
        Case UCase("inv_owners")
             Obj= "Plates/Containers" 
             Field = "Owner"
        Case UCase("inv_physical_state")
             Obj= "Plates/Containers" 
             Field = "Physical State"
        Case UCase("inv_project_job_info")
             Obj= "Containers" 
             Field = "Project Job Info"
        Case UCase("inv_reporttypes")
             Obj= "Reports" 
             Field = "Report"
        Case UCase("inv_reservation_types")
             Obj= "Containers" 
             Field = "Reservation Types"
        Case UCase("inv_solvents")
             Obj= "Plates/Containers" 
             Field = "Solvent"
        Case UCase("inv_states")
             Obj= "Plates/Containers" 
             Field = "State"
        Case UCase("inv_suppliers")
             Obj= "Containers" 
             Field = "Supplier"
        Case UCase("inv_units")
             Obj= "Containers/Plates" 
             Field = "Unit"
        Case UCase("inv_unit_types")
             Obj= "Containers/Plates" 
             Field = "Unit Type"
    End Select
    CustomErrorMsg = replace(CustomErrorMsg,"<objects>", Obj)
    CustomErrorMsg = replace(CustomErrorMsg,"<field>", Field)
END FUNCTION
			
%>
		</TD>
	</TR>
</TABLE>
</Body>			
