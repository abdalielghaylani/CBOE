<%Response.Expires = -1
if Not Session("UserValidated" & dbkey) = 1 then
	'response.redirect "../../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if

dbkey = "docmanager"
%>
<%'SYAN modified on 1/16/2006 to fix CSBR-54770%>

<!--#INCLUDE VIRTUAL="/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'End of SYAN modification%>

<!--#INCLUDE VIRTUAL="/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL="/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL="/cfserverasp/source/form_val_js.js"-->
<%
StoreASPSessionID()




%>
<script language="javascript">

function MaxLengthValidation(object, MaxLength){ 

        if(object.value.length > MaxLength) 
	{           
          object.value = object.value.substr(0, MaxLength); 
          return false; 
        } 
      
}

function updateSelectedValue(theform,elm){
	
	var theFieldname = elm.name.replace("list", "")
	var theField = theform.elements[theFieldname]
	var theSelectBox = elm.options[elm.selectedIndex]
	theField.value = theSelectBox.value
	//alert(theField.name + "=" + theField.value);
	//elm.focus()
	//window.focus()
	return true

}

function PopUpDate_override(strControl,fullsourcepath,formname)
{

	var fullsourcepath = fullsourcepath.replace('cows_input_form','submitForm');

	var formname = 'submitForm';
	var formName = 'submitForm';
	//SYAN added 12/15/2003 to fix CSBR-35466
	//append date_format to fullsourcepath. This fix is made this way to avoid 
	//change of function signature. Yuk.
	var quesPos = fullsourcepath.indexOf('?')
	var param;
	if (fullsourcepath.indexOf('?') > 0) {
		param = fullsourcepath.substring(quesPos + 1, fullsourcepath.length);
		fullsourcepath = fullsourcepath.substring(0, quesPos);
	}

	//End of SYAN modification
	
	var browserNetscape = "<%=strTrueFalse(detectNetscape())%>"
	var strURL = fullsourcepath + "?CTRL=" + strControl;

	if (formname !=null){
		
		var strCurDate = "document.forms['" + formname + "'].elements['" + strControl + "'].value";
		
	}
	else{
		var strCurDate = document.forms["cows_input_form"].elements[strControl].value;
	}
	
	
	if ((strCurDate != null) && (strCurDate != "undefined")) {
		if (strCurDate.length > 0){
			strURL += "&INIT=" + strCurDate;
		}
	}
	
	//SYAN added 12/15/2003 to fix CSBR-35466
	if (param != null) {
		strURL = strURL + '&' + param
	}
	//End of SYAN modification
	var windowDatePicker = ""
	if (windowDatePicker.name == null){
	
		if (browserNetscape.toLowerCase() == "true"){
			windowDatePicker = window.open(strURL,"dp","toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0," + "width=190,height=200");
			windowDatePicker.focus();
			}
		else{
			windowDatePicker = window.open(strURL,"dp","toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0," + "width=190,height=200,left=" + (window.event.screenX - 190) + ",top=" + (window.event.screenY + 20));
			windowDatePicker.focus();
		}
	}
	else{
		windowDatePicker.focus()
	}
	//jhs 1/4/2007 - we don't have this available and ie 5 is no longer supported anyway
	//if (Version.indexOf("MSIE 5.0") != -1){
		date_picker_used = true
	//}
	//return false
}

</script>

<html>
<head>
<title>Document submission</title>

</head>

<body background="<%=Application("UserWindowBackground")%>">
<form method="post" name="submitForm" action="submitFeedback.asp?<%=Request.QueryString%>" target="_top">

	<!---JHS added 4/9/2003--->
	<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
		<!-- The table for the banner. -->
		<tr>

			<td valign="top" width="300">
				<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
			</td>

			<td>
					<font face="Arial" color="#0099FF" size="4"><i>
						Add Document Details
					</i></font>
			</td>
		</tr>
	</table>
	<!---JHS added 4/9/2003 end--->
	
<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="100"></td>
		<td width="580">
			<!---JHS commented out 4/9/2003 able border="0">				<tr>					<td colspan="2"><img src="/docmanager/docmanager/gifs/banner.gif"></td>				</tr>			</table--->
			
			<table border="0">
				<tr><td colspan="2" align="left">
						<a href="/docmanager/docmanager/src/locateDocs.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/submitAnotherDoc.gif" border="0"></a>
						<a href="/<%=Application("appkey")%>/inputtoggle.asp?formgroup=base_form_group&amp;dbname=<%=dbkey%>" target="_top"><img src="/<%=Application("appkey")%>/graphics/searchdoc_btn.gif" border="0"></a>
						<%if session("BATCHLOAD_DOCS" & dbkey) then%>
						<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>
						<%end if%>
						<%if session("VIEW_HISTORY" & dbkey) then%>
						<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>
						<%end if%>
						<a href="/docmanager/docmanager/mainpage.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/mainmenu.gif" border="0"></a>
						<a href="/cs_security/home.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/home_oval_btn.gif" border="0"></a>
					</td>
				</tr>
			</table>
			
			<table width="560">
				<tr>
					<td align="right" width="450"><font color="#000099">You are logged in as <font color="#990000"><%=Session("UserName" & Application("appkey"))%></font></font></td>
					<td align="center"><a href="/<%=Application("appkey")%>/logoff.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a></td>
				</tr>
			</table>

			<table>
				<tr><td height="0"></td></tr>
			    <tr>		<!---JHS 1/9/2008 -Removed last modified date because it is useless--->
					<td><font>Information of <%=Session("fileName")%> (<%=Session("docSize")%> kb) 
								shown below will be displayed as properties of the document. Make changes if necessary before submission.
						</font>
					</td>
				</tr>
			</table>

			<table border="0">
				<tr><td><table>
							<tr><td>Title:</td>
								<td><input size="60" name="title" value="<%=Session("docTitle")%>" maxlength="1000"></td>
							</tr>
							
							<tr><td>Author(s):</td>
								<td><input size="60" name="author" value="<%=Session("docAuthor")%>" maxlength="500"></td>
							</tr>
							
							<tr><td valign="top">Comments:</td>
								<td><textarea cols="45" rows="3" name="comments" maxlength="2000" onkeyup="MaxLengthValidation(this, 2000)" onblur="MaxLengthValidation(this, 2000)"><%=Session("docComments")%></textarea></td>
							</tr>

							<%If Application("optional_fieldMAIN_AUTHOR") then%>
								<tr><td>Main Author:</td>
								<td><input size="60" name="MAIN_AUTHOR" value="" maxlength="60"></td>
							</tr>
							<%else%>
								<input size="60" name="MAIN_AUTHOR" type="hidden" value="" maxlength="60">
							<%End If%>

							<%If Application("optional_fieldREPORT_NUMBER") then%>
								<tr><td>Report Number:</td>
								<td><input size="60" name="REPORT_NUMBER" value="" maxlength="12"></td>
							</tr>
							<%else%>
								<input size="60" name="REPORT_NUMBER" type="hidden" value="" maxlength="12">
							<%End If%>	

							<%If Application("optional_fieldSTATUS") then
								DocTypeVal = 1
								DocTypeText = ""

						
							%>
								<tr><td>Status:</td>
								<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.STATUS", Application("STATUS_LIST"), "", DocTypeText, 1, true, "value", "0"%></td>
							</tr>
							<%else%>
								<input size="60" name="DOCMGR_DOCUMENTS.STATUS" type="hidden" value="">
							<%End If%>	
														
							<%If Application("optional_fieldWRITER") then%>
								<tr><td>Writer:</td>
								<td><input size="60" name="WRITER" value="" maxlength="60"></td>
							</tr>
							<%else%>
								<input size="60" name="WRITER" type="hidden" value="" maxlength="60">
							<%End If%>	
							
							<%If Application("optional_fieldABSTRACT") then%>
								<tr><td>Abstract:</td>
								<td><textarea cols="45" rows="3" name="ABSTRACT"></textarea></td>
							</tr>
							<%else%>
								<input size="60" name="ABSTRACT" type="hidden" value="">
							<%End If%>		
							<%

							If Application("optional_fieldDOCUMENT_DATE") then%>
								<tr><td>Document date <font size=-1>(<%=Application("DATE_FORMAT_DISPLAY")%>)</font>:</td>
								<td><%'ShowInputField dbkey, formgroup, "DOCMGR_DOCUMENTS.DOCUMENT_DATE", "DATE_PICKER:8", "15"%>
								<%if Application("Date_format") <> "" then
									dformat = Application("Date_format")
								 else
									dformat = "8"
								 end if
								 %>
								<!---br /---><input type="text" name="DOCMGR_DOCUMENTS.DOCUMENT_DATE" size="15" value="" onChange="isValid(this, <%=dformat%>)"><a href="javascript:void(0);" onclick="PopUpDate_override(&quot;DOCMGR_DOCUMENTS.DOCUMENT_DATE&quot;,&quot;/cfserverasp/source/month_view1.asp?date_format=<%=dformat%>&amp;formName=submitForm&quot;,&quot;submitForm&quot;);return false;"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>
								</td>
							</tr>

							<%else%>
								<input size="60" name="DOCMGR_DOCUMENTS.DOCUMENT_DATE" type="hidden" value="">
							<%End If%>	
							
							<%If Application("optional_fieldDOCUMENT_CLASS") then
								DocTypeVal = 1
								DocTypeText = ""
							%>
								<tr><td>Document Class:</td>
								<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.DOCUMENT_CLASS", Application("DOCUMENT_CLASS_LIST"), "", DocTypeText, 1, true, "value", "0"%></td>
							</tr>
							<%else%>
								<input size="60" name="DOCMGR_DOCUMENTS.DOCUMENT_CLASS" type="hidden" value="">
							<%End If%>	

							<%If Application("optional_fieldSEC_DOC_CAT") then
								DocTypeVal = 1
								DocTypeText = ""
							%>
								<tr><td>Security Document Category:</td>
								<td><%ShowLookUpList dbkey, formgroup, null, "DOCMGR_DOCUMENTS.SEC_DOC_CAT", Application("SEC_DOC_CAT_LIST"), "", DocTypeText, 1, true, "value", "0"%></td>
							</tr>
							<%else%>
								<input size="60" name="DOCMGR_DOCUMENTS.SEC_DOC_CAT" type="hidden" value="">
							<%End If%>	
																											
							<tr><td></td>
								<td align="right">
									<input type="submit" value="Submit This Document">
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</form>
</body>
</html>
<%
'clear up the attribute so next document wouldn't show the old information
Session("docTitle") = ""
Session("docAuthor") = ""
Session("docLastModified") = ""
Session("docComments") = ""
%>	
