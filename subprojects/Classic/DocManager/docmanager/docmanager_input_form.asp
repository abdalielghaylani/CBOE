<%@ LANGUAGE=VBScript %>


<%response.expires = 0%>
<%'Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved
dbkey=Request("dbname")
if Not Session("UserValidated" & dbkey) = 1 then
	response.redirect "../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if%>

<% 
Dim optcount 
optcount=0

sub checkOpenTR
	if optcount = 0 then
		Response.Write "<tr>"
	End if	
end sub

sub checkCloseTR
	if optcount = 1 then
		Response.Write "</tr>"
		optcount = 0
	Else
		optcount = 1
	End if	
end sub

%>

<html>
<head>

<script language="javascript">
    function doSortHidden()
    {
        var sortval = document.cows_input_form.sortcol[document.cows_input_form.sortcol.selectedIndex].value; 
        var sortdir = document.cows_input_form.ascdesc[document.cows_input_form.ascdesc.selectedIndex].value; 
        
        document.cows_input_form.order_by.value = "" + sortval + " " + sortdir;
    }    
</script>


<!--#INCLUDE VIRTUAL="/cfserverasp/source/cows_func_js.asp"-->
<title>Document Manager - Input Form</title>
</head>

<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->


<!--#INCLUDE VIRTUAL="/cfserverasp/source/header_vbs.asp"-->

<input type = "hidden" name = "CSDO_TRANS_STRUC_KEY" value = "DOCID">
<BODY background="<%=Application("UserWindowBackground")%>">


<table border="0" cellspacing="0" cellpadding="0">

<%'=Request.ServerVariables("HTTP_USER_AGENT")%>
	
<%
'JHS added 4/02/03
if session("showselect") then
%>
<tr><td width="500">
<%
	Response.Write "<span style=""align:left"">To add a link to " & Session("LinkFieldName") & " " & Session("extLinkID") & " for " & Session("extAppName") & ", perform a search to find the document and then click the ""Add Link"" button.</span>" 
	Response.Write "<br><br>"
	
%>
</td></tr>
<%
end if	
'JHS added 4/02/03 end

DocTypeList = "DOC:DOC,XLS:XLS,PPT:PPT,PDF:PDF,TXT:TXT"
DocTypeVal = 1
DocTypeText = ""

if CBool(Application("RLS")) = true then 
	sql = "SELECT PROJECT_INTERNAL_ID, PROJECT_NAME " & _
			"FROM REGDB.PROJECTS, REGDB.PEOPLE_PROJECT, CS_SECURITY.PEOPLE " & _
			"WHERE REGDB.PEOPLE_PROJECT.PROJECT_ID = REGDB.PROJECTS.PROJECT_INTERNAL_ID " & _
				"AND CS_SECURITY.PEOPLE.PERSON_ID = REGDB.PEOPLE_PROJECT.PERSON_ID " & _
				"AND CS_SECURITY.PEOPLE.USER_ID = '" & UCase(Session("UserName" & Application("appkey"))) & "'"
else
	sql = "SELECT PROJECT_INTERNAL_ID, PROJECT_NAME " & _
			"FROM REGDB.PROJECTS"
end if
		
ConnStr = "FILE NAME=" & Application("UDLPath") & ";User ID=" & Session("UserName" & dbkey) & ";Password=" & Session("UserID" & dbkey)
Set cnn = Server.CreateObject("ADODB.Connection")
cnn.Open ConnStr
set projectsRS = Server.CreateObject("ADODB.Recordset")

'SYAN modified on 2/22/2006 to fix CSBR-64263
err.clear
on error resume next
projectsRS.Open sql, cnn

projectPriv = true
if InStr(err.Description, "table or view does not exist") > 0 then
	projectPriv = false
	'Response.Write "You need to be granted a Chemistry Registration role first for this page contains Projects information in Registration."
end if
'End of SYAN modification
%>
			

<tr><td>
<table border="1" bordercolor="#000099" width="650">
                <tr><td colspan="4" align="right">
                    <input type="hidden" name="order_by" value="DOCMGR_DOCUMENTS.DATE_SUBMITTED DESC" />
                   <font size="1" color="#000099"><b>Sort by:</b></font>
                    <select name="sortcol" onchange="doSortHidden()">
                        <option value="DOCMGR_DOCUMENTS.DATE_SUBMITTED">Date Submitted</option>
                        <option value="DOCMGR_DOCUMENTS.AUTHOR">Author</option>
                        <option value="DOCMGR_DOCUMENTS.DOCNAME">File Name</option>
                        <option value="DOCMGR_DOCUMENTS.DOCTYPE">Document Type</option>
                        <option value="DOCMGR_DOCUMENTS.TITLE">Title</option>
                        
											
				<%If Application("optional_fieldDOCUMENT_CLASS") then%>
					<option value="DOCMGR_DOCUMENTS.DOCUMENT_CLASS">Document Class</option>
				<%End If%>
				<%If Application("optional_fieldDOCUMENT_DATE") then%>
					<option value="DOCMGR_DOCUMENTS.DOCUMENT_DATE">Document Date</option>
				<%End If%>	
				<%If Application("optional_fieldMAIN_AUTHOR") then%>
					<option value="DOCMGR_DOCUMENTS.MAIN_AUTHOR">Main Author</option>
				<%End If%>
				<%If Application("optional_fieldREPORT_NUMBER") then%>
					<option value="DOCMGR_DOCUMENTS.REPORT_NUMBER">Report Number</option>
				<%End If%>
				<%If Application("optional_fieldSEC_DOC_CAT") then%>
					<option value="DOCMGR_DOCUMENTS.SEC_DOC_CAT">Security Category</option>
				<%End If%>	
				<%If Application("optional_fieldSTATUS") then%>
					<option value="DOCMGR_DOCUMENTS.STATUS">Status</option>			
			    <%End If%>
				<%If Application("optional_fieldWRITER") then%>
					<option value="DOCMGR_DOCUMENTS.WRITER">Writer</option>			
				<%End If%>	                        
                        
                        
                        
                        
                        
                        
                        
                        
                    </select>
                    <select name="ascdesc" onchange="doSortHidden()">
                        <option value="ASC">ASC</option>
                        <option value="DESC" selected>DESC</option>
                    </select>
                </td></tr>
				<tr><td colspan="4"><font color="#000099"><b>Full-Text: </b></font><font size="2" color="#000099">(Exact word or phrases; x*; x AND y; x OR y; x NOT y; ABOUT(x))</font>
					</td>
				</tr>

				<tr><td colspan="4"><%'this is to search for long raw data DOCMGR_DOCUMENTS.DOC field,
						'but the field given here is the returning field for build subquery
						'in search_func_vbs.asp
						ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.DOCID", 25, "70"%>
						<input type="hidden" name="INDEX_FIELD" value="DOCMGR_DOCUMENTS.DOC">
					</td>
					<td>
					</td>
				</tr>
				
				<!--tr><td><font color="#000099"><b>SEP Number:</b></font>
					</td>
					
					<td><%'ShowInputField dbkey, formgroup, "DOCMGR_EXTERNAL_LINKS.LINKID", 25, "25"%>
					</td>
					
					<td><font color="#000099"><b>Project:</b></font>
					</td>
					
					<td><%if projectPriv = true then%>
							<%if not (projectsRS.BOF and projectsRS.EOF) then%>
							<table>
								<tr>
									<td>
										<select name="DOCMGR_DOCUMENTS.REG_RLS_PROJECT_IDlist" onChange="updateSelectedValue(this.form,this)" size ="1">
										<option>-- Select one --</option>
										<%	projectsRS.MoveFirst
											while not projectsRS.EOF
										%>
											<option value="<%=projectsRS("PROJECT_INTERNAL_ID")%>"><%=projectsRS("PROJECT_NAME")%></option>
											<%projectsRS.MoveNext
											wend%>
										</select>
										<input type="hidden" name="DOCMGR_DOCUMENTS.REG_RLS_PROJECT_ID" value="">			
									</td>
								</tr>
							</table>
						<%
							projectsRS.Close
							Set projectsRS = nothing
						end if
					else%>
						<table>
							<tr>
								<td>
									<a href="#" onclick="alert('You need to be granted a Chemistry Registration Role first to search by projects information. Please ask the administrators to grant you the role and log back in to try again.'); return false;">Insufficient Privileges</a>
								</td>
							</tr>
						</table>
					<%end if%>

					</td>
				</tr-->
				
				<tr><td><font color="#000099"><b>File Type:</b></td>
					<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.DOCTYPE", DocTypeList, "", DocTypeText, 1, true, "value", "0"%></td>				
					<td><font color="#000099"><b>File Name:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.DOCNAME", 25, "25"%></td>
				</tr>
				
				<tr><td><font color="#000099"><b>Title:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.TITLE", 25, "25"%></td>				
					<td><font color="#000099"><b>Author:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.AUTHOR", 25, "25"%></td>
				</tr>
				
				
				<% 
				'REPORT_NUMBER,MAIN_AUTHOR,STATUS,WRITER,DOCUMENT_DATE,ABSTRACT
				'For now hard code optional field display
				
				If Application("optional_fieldREPORT_NUMBER") then
					checkOpenTR%>
					<td><font color="#000099"><b>Report Number:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.REPORT_NUMBER", 25, "25"%></td>				
					<%checkCloseTR
				End If%>
				
				<%If Application("optional_fieldMAIN_AUTHOR") then
					checkOpenTR%>
					<td><font color="#000099"><b>Main Author:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.MAIN_AUTHOR", 25, "25"%></td>				
					<%checkCloseTR
				End If%>
				
				<%If Application("optional_fieldSTATUS") then
					checkOpenTR%>
					<td><font color="#000099"><b>Status:</b></td>			
					<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.STATUS", Application("STATUS_LIST"), "", DocTypeText, 1, true, "value", "0"%></td>						
					<%checkCloseTR
				End If%>
				
				<%If Application("optional_fieldWRITER") then
					checkOpenTR%>
					<td><font color="#000099"><b>Writer:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.WRITER", 25, "25"%></td>				
					<%checkCloseTR
				End If%>				
				<%If Application("optional_fieldABSTRACT") then
					checkOpenTR%>
					<td><font color="#000099"><b>Abstract:</b></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.ABSTRACT", 25, "25"%></td>				
					<%checkCloseTR
				End If%>									

				<%If Application("optional_fieldDOCUMENT_DATE") then
					checkOpenTR%>
					<td><font color="#000099"><b>Document Date:</b></font><br /> <font  color="#000099" size="-2">(<%=Application("DATE_FORMAT_DISPLAY") %>)</font></td>
					<td><%ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.DOCUMENT_DATE", "DATE_PICKER:8", "15"%></td>				
					<%checkCloseTR
				End If%>	

				<%If Application("optional_fieldDOCUMENT_CLASS") then
					checkOpenTR%>
					<td><font color="#000099"><b>Document Class:</b></td>			
					<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.DOCUMENT_CLASS", Application("DOCUMENT_CLASS_LIST"), "", DocTypeText, 1, true, "value", "0"%></td>						
					<%checkCloseTR
				End If%>
				<%If Application("optional_fieldSEC_DOC_CAT") then
					checkOpenTR%>
					<td><font color="#000099"><b>Security Document Category:</b></td>			
					<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.SEC_DOC_CAT", Application("SEC_DOC_CAT_LIST"), "", DocTypeText, 1, true, "value", "0"%></td>						
					<%checkCloseTR
				End If%>					
								
				<%
				if optcount = 1 then
					Response.Write "<td>&nbsp;</td><td>&nbsp;</td></tr>"
					optcount = 0
				Else
					optcount = 1
				End if	
				%>
								
				<tr><td colspan="4"><table cellpadding="0" cellspacing="0" border="0">
							<tr><td valign="top">
									<select name="DOCMGR_STRUCTURES.STRUCTURE.sstype" size="1">
										<option selected value="0">Substructure</option>
										<option value="1">Exact Structure</option>
										<option value="3">Tanimoto Similarity</option>
									</select>
								</td>
							</tr>
											
							<tr><td><!--table border="1">
										<tr><td--><!--embed src="/CFWTEMP/docmanager/docmanagerTemp/mt.cdx" border="0" width="470" height="302" id="4" name="CDX" type="chemical/x-cdx">
												<Input type="hidden" name = "DOCMGR_STRUCTURES.STRUCTURE" value=""-->
												<%ShowStrucInputField  dbkey, formgroup, "DOCMGR_STRUCTURES.STRUCTURE", "4", "470", "302", "", ""%>									
											<!--/td>
										</tr>
									</table-->
								</td>
							</tr>
						</table>
					</td>
				</tr>
				
				<!--tr><td>
						<table border="0" cellpadding="0">
							<tr><td>
									<font face="MS Sans Serif" size="1" color="#000000"><font face="Arial" size="2" color="#000000"><%ShowStrucInputField  dbkey, formgroup,"DOCMGR_STRUCTURES.STRUCTURE","4","470","302", "AllOptions", "SelectList"%></font></font>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				
				<tr>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>MOL_ID</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.MOL_ID",1,"32"%></font></td>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DOCID</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.DOCID",1,"32"%></font></td>
				</tr>
				
				
				<tr>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>Formula</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.FORMULA",4,"32"%></font></td>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>MolWeight</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_STRUCTURES.MOLWEIGHT",0,"31.2"%></font></td>
				</tr>
				
				<tr>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>TITLE</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.TITLE",0,"64"%></font></td>
				</tr>
				
				<tr>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DOCLOCATION</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DOCLOCATION",0,"64"%></font></td>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DOCNAME</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DOCNAME",0,"32"%></font></td>
				</tr>
				
				<tr>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>AUTHOR</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.AUTHOR",0,"32"%></font></td>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>SUBMITTER</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.SUBMITTER",0,"32"%></font></td>
				</tr>
				
				<tr>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>SUBMITTER_COMMENTS</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.SUBMITTER_COMMENTS",0,"64"%></font></td>
					<td nowrap valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><strong>DATE_SUBMITTED</strong></font></td><td valign = "top"><font face="MS Sans Serif" size="1" color="#000000"><%ShowInputField dbkey, formgroup,"DOCMGR_DOCUMENTS.DATE_SUBMITTED",8,"32"%></font></td>
				</tr-->
			</table>
		</td>
	</tr>
</table>

<!--this field is to add an input field, so when enter key hit, the form won't submit-->


<!--#INCLUDE VIRTUAL="/cfserverasp/source/input_form_footer_vbs.asp"-->

</body>
</html>
