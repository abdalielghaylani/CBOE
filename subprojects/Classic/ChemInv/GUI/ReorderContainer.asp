<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
 
%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetReorderContainerAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<%
	'The variable used to set the "On Order Location ID" is hard-coded as LocationID in guiUtils.asp, so we set
	'it here to be the "On Order" location.
	LocationID = 1
	Dim FormData
	Dim bDebugPrint
	Dim bReorderPossible
	Dim CatNum
	Dim SupplierID
	
	Sub AddNameValuePair(byref sOut, byval sName, byval sVal)
	    dim maybeAmp
	    maybeAmp = "&"
	    if len(sOut) = 0 then maybeAmp = ""
	    sOut = sOut & maybeAmp & sName & "=" & sVal  
    end sub
    
    Sub AddItemToList(byref v, byVal str)
	    dim maybeComma
	    maybeComma = ","
	    if len(v) = 0 then maybeComma = ""
	    v = v & maybeComma & str
    End sub
    
    bDebugPrint = false
    bReorderPossible = true
    
    CatNum = Request("CatNum")
    SupplierID = Request("SupplierID")
    
    if (SupplierID="") then 
		bReorderPossible = false
    else 		
	    if (CatNum = "") OR (SupplierID =0) then
	        bReorderPossible = false
	    end if
    end if
	
	' Custom work for Millennium, not enabled for E10
	'if Application("BYPASS_ORDERING_SCREENS") and bReorderPossible then	 
	if false then   
	    AddNameValuePair FormData, "DeliveryLocationID", DeliveryLocationID
	    AddNameValuePair FormData, "ContainerID", ContainerID
	    AddNameValuePair FormData, "ContainerName", ContainerName
	    AddNameValuePair FormData, "Comments", Comments
	    AddNameValuePair FormData, "OwnerID", OwnerID
	    AddNameValuePair FormData, "CurrentUserID", CurrentUserID
	    AddNameValuePair FormData, "NumCopies", NumCopies	    
	    AddNameValuePair FormData, "Project", Project
	    AddNameValuePair FormData, "Job", Job
	    AddNameValuePair FormData, "RushOrder", RushOrder
	    AddNameValuePair FormData, "DueDate", DueDate
	    AddNameValuePair FormData, "ReorderReason", ReorderReason
	    AddNameValuePair FormData, "ReorderReasonOther", ""
	    	    
	    ServerName= Request.ServerVariables("Server_Name")
	    pUserAgent= "CShttpRequest3"
	    pSendTimeout= 30 * 1000
	    pReceiveTimout= 120 * 1000

	    AddNameValuePair FormData, "sid", session.SessionID
	    AddNameValuePair FormData, "CSUserName", Session("UserName" & "cheminv")
	    AddNameValuePair FormData, "CSUSerID", Session("UserID" & "cheminv")
	    pTarget= "/cheminv/api/ReorderContainer.asp" 
	    
	    httpResponse = CShttpRequest3("POST", ServerName, pTarget, pUserAgent, FormData, pSendTimeout, pReceiveTimout)
	    if InStr(httpResponse,"ORA-")> 0 OR bDebugPrint then
		    response.write httpResponse 
		    response.end
		else
            Session("CurrentContainerID") = httpResponse
            server.transfer("./ReorderContainer_bypass.asp")
	    end if    
	    
	    Response.End
	end if      ' if Application("BYPASS_ORDERING_SCREENS") then
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reorder a Container</title>

<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();

	// Validates container attributes
	function ValidateContainer(){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		//Populate hidden variables
		!document.form1.iContainerName ? document.form1.ContainerName.value="<%=ContainerName%>" : document.form1.ContainerName.value = document.form1.iContainerName.value;
		!document.form1.iComments ? document.form1.Comments.value=document.form1.Comments.value : document.form1.Comments.value = document.form1.iComments.value;
		!document.form1.iOwnerID ? document.form1.OwnerID.value="<%=OwnerID%>" : document.form1.OwnerID.value = document.form1.iOwnerID.value;		
		!document.form1.iCurrentUserID ? document.form1.CurrentUserID.value="<%=CurrentUserID%>" : document.form1.CurrentUserID.value = document.form1.iCurrentUserID.value;		
		!document.form1.iDeliveryLocationID ? document.form1.DeliveryLocationID.value="<%=DeliveryLocationID%>" : document.form1.DeliveryLocationID.value = document.form1.iDeliveryLocationID.value;
		!document.form1.iNumCopies ? document.form1.NumCopies.value="<%=NumCopies%>" : document.form1.NumCopies.value = document.form1.iNumCopies.value;
		!document.form1.iProject ? document.form1.Project.value="<%=Project%>" : document.form1.Project.value = document.form1.iProject.value;	
		!document.form1.iJob ? document.form1.Job.value="<%=Job%>" : document.form1.Job.value = document.form1.iJob.value;	
		!document.form1.iRushOrder ? document.form1.RushOrder.value="<%=RushOrder%>" : document.form1.RushOrder.value = document.form1.iRushOrder.value;	
		!document.form1.iDueDate ? document.form1.DueDate.value="<%=DueDate%>" : document.form1.DueDate.value = document.form1.iDueDate.value;	
		!document.form1.iReorderReason ? document.form1.ReorderReason.value="<%=ReorderReason%>" : document.form1.ReorderReason.value = document.form1.iReorderReason.value;	
		!document.form1.iReorderReasonOther ? document.form1.ReorderReasonOther.value=document.form1.ReorderReasonOther.value : document.form1.ReorderReasonOther.value = document.form1.iReorderReasonOther.value;
		
		//DeliveryLocationID is required
		
		if (document.form1.DeliveryLocationID.value.length == 0) {
			errmsg = errmsg + "- Delivery Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// DeliveryLocationID must be a positive number
			if (!isPositiveNumber(document.form1.DeliveryLocationID.value)){
				errmsg = errmsg + "- Delivery Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	

		// NumCopies is required
		if (document.form1.NumCopies.value.length == 0) {
			errmsg = errmsg + "- Number of Copies is required.\r";
			bWriteError = true;
		}
		else{
			// NumCopies must be a number
			if (!isPositiveNumber(document.form1.NumCopies.value)){
				errmsg = errmsg + "- Number of copies must be a positive number.\r";
				bWriteError = true;
			}
			else {
				if (document.form1.NumCopies.value > 399){
					errmsg = errmsg + "- Number of copies must be less than 400.\r";
					bWriteError = true;	
				}
			}
		}

		// Project is required.
		if (document.form1.Project.value.length == 0){
			errmsg = errmsg + "- The Project is required.\r";
			bWriteError = true;
		}
		// Job is required.
		if (document.form1.Job.value.length == 0){
			errmsg = errmsg + "- The Job is required.\r";
			bWriteError = true;
		}
		
		// The due date must be present.
		if (document.form1.DueDate.value.length == 0){
			errmsg = errmsg + "- The Due Date is required.\r";
			bWriteError = true;
		}

		// Due Date must be a date
		if (document.form1.DueDate.value.length > 0 && !isDate(document.form1.DueDate.value)){
			errmsg = errmsg + "- The Due Date must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}

		// Due Date must be at least tomorrow
	    var d, dueDate;
	    dueDate = getDate(document.form1.DueDate.value);
	    d = new Date();
	    d.setHours(0,0,0,0);		// set time to 00:00:00.000
	    d.setDate(d.getDate()+1);	// set date to "tomorrow"
	    
		if (document.form1.DueDate.value.length > 0 && isDate(document.form1.DueDate.value) && Date.parse(dueDate) < Date.parse(d)) {
			errmsg = errmsg + "- The Due Date must be at least tomorrow.\r";
			bWriteError = true;
		}

		// Reorder Reason may be required.
		if (<%=Session("Hassle")%> == 1) {
			if (document.form1.ReorderReason.value.length == 0){
				errmsg = errmsg + "- The Reorder Reason is required.\r";
				bWriteError = true;
			}
			if (document.form1.ReorderReason.value == 3 && document.form1.ReorderReasonOther.value.length == 0) {
				errmsg = errmsg + "- The Reorder Reason is required.\r";
				bWriteError = true;
			}
		}

		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			var bcontinue = true;
			
			if (bcontinue) document.form1.submit();
		}
	}
	 
	function postDataFunction(newFocus) {
		document.form1.action = "ReorderContainer.asp?GetData=form&setFocus=" + newFocus
		document.form1.submit()
	}
//-->
</script>

</head>
<body>
<%
if not bReorderPossible then 
	Response.Write "<CENTER><BR><BR><BR><BR><BR><BR><BR><span class=GUIFeedback>Cannot reorder container because it's missing supplier ID and/or catalog number.</span><BR><br>"
	Response.Write "<a HREF=""#"" onclick=""window.close(); return false;""><img SRC=""/ChemInv/graphics/ok_dialog_btn.gif"" border=0></a></CENTER></body>"
	Response.end
End if 	
%>
<form name="form1" method="POST" action="ReorderContainer_action.asp">
<input TYPE="hidden" NAME="ContainerID" Value="<%=ContainerID%>">
<input TYPE="hidden" NAME="DeliveryLocationID" Value> 
<input TYPE="hidden" NAME="ContainerName" Value>
<input TYPE="hidden" NAME="Comments" Value="<%=Comments%>">
<input TYPE="hidden" NAME="OwnerID" Value>
<input TYPE="hidden" NAME="CurrentUserID" Value>
<input TYPE="hidden" NAME="NumCopies" Value>
<input TYPE="hidden" NAME="Project" Value>
<input TYPE="hidden" NAME="Job" Value>
<input TYPE="hidden" NAME="CatNum" Value="<%=CatNum%>">
<input TYPE="hidden" NAME="SupplierID" Value="<%=SupplierID%>">
<input TYPE="hidden" NAME="RushOrder" Value="<%=RushOrder%>">
<input TYPE="hidden" NAME="DueDate" Value>
<input TYPE="hidden" NAME="ReorderReason" Value>
<input TYPE="hidden" NAME="ReorderReasonOther" Value="<%=ReorderReasonOther%>">

<table border="0" cellspacing="0" cellpadding="0" width="700">
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Delivery loc ID: <%=GetBarcodeIcon()%></span>
		</td>
		<td colspan="3">
			<%ShowLocationPicker2 "document.form1", "iDeliveryLocationID", "lpDeliveryLocationBarCode", "lpDeliveryLocationName", 10, 49, false, DeliveryLocationID%> 
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			On Order loc ID: <%=GetBarcodeIcon()%>
		</td>
		<td colspan="3">
			<%ShowReadOnlyLocationWithoutPicker "document.form1", "iLocationID", "lpLocationBarCode", "lpLocationName", 10, 49, false%> 
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Container name:</span>
		</td>
		<td colspan="3">
			<input type="text" name="iContainerName" size="70" value="<%=ContainerName%>">
		</td>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Number of Bottles:", "NumCopies", 5, "", False, True)%>
		<td></td><td></td>
	</tr>
	<tr height="25">
		<%= ShowPickList("<SPAN class=required>Container Status:</span>", "iContainerStatusID", ContainerStatusID,"SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_Container_Status where Container_Status_ID = 4")%>
	</tr>
<!--	<tr height="25">		<%= ShowPickList("Owner:", "iOwnerID", OwnerID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>	</tr>-->
	<tr height="25">
		<%=ShowPickList("Current user:", "iCurrentUserID", CurrentUserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span title="Pick an option from the list">Owner:</span>
		</td>
		<td>
			<%=ShowSelectBox2("iOwnerID", OwnerID, "SELECT owner_ID AS Value, description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners ORDER BY lower(description) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
	</tr>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Comments:
		</td>
		<td valign="top">
			<textarea rows="8" cols="60" name="iComments" wrap="hard"><%=Comments%></textarea>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Project:</span>
		</td>
		<td>
		<%=ShowSelectBox3("iProject", Project, "SELECT DISTINCT project_no AS Value, project_description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_Project_Job_Info ORDER BY UPPER(project_description)", 27, RepeatString(43, "&nbsp;"), "", "postDataFunction('iJob');")%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Job:</span>
		</td>
		<td>
		<%=ShowSelectBox2("iJob", Job, "SELECT job_no AS Value, job_description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_Project_Job_Info WHERE project_no = '" & Project & "' ORDER BY UPPER(job_description)", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150"><span class="required">Due Date:</span></td>
		<td><%call ShowInputField("", "", "iDueDate:form1:" & DueDate , "DATE_PICKER:TEXT", "15")%></td>
		<!--<td><input type="text" name="iDueDate" size="15" value="<%=DueDate%>"><a href onclick="return PopUpDate(&quot;iDueDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a></td>-->
	</tr>
	<tr height="25">
		<td>&nbsp;</td><td><input type="checkbox" name="rushOrder_cb" onclick="document.form1.RushOrder.value = this.checked;">Rush order<br></td>
		<script language="Javascript">if (document.form1.RushOrder.value == "true") document.form1.rushOrder_cb.click(); </script>
		<td></td><td></td>
	</tr>
	<%
		If Session("Hassle") = 1 Then
	%>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Reorder Reason:</span>
		</td>
		<td>
		<%=ShowSelectBox3("iReorderReason", ReorderReason, "SELECT DISTINCT container_order_reason_id AS Value, name AS DisplayText, sort_order FROM " & Application("CHEMINV_USERNAME") & ".inv_Container_Order_Reason ORDER BY sort_order ASC", 27, RepeatString(43, "&nbsp;"), "", "postDataFunction('iReorderReasonOther');")%>
		</td>
	</tr>
	<%
			If Session("ReorderReason") = "3" Then
	%>
	<tr height="50">
		<td align="right" valign="top" nowrap>
			Reason (other):
		</td>
		<td valign="top">
			<textarea rows="2" cols="60" name="iReorderReasonOther" wrap="hard"><%=ReorderReasonOther%></textarea>
		</td>
	</tr>
	<%
			End If
		End If
	%>
	<tr>
		<td colspan="4" align="right" height="20" valign="bottom"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/ChemInv/graphics/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
				<a HREF="#" onclick="ValidateContainer(); return false;"><img SRC="/ChemInv/graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>	
</form>
</body>
</html>
<%If Not IsEmpty(Request("setFocus")) Then%>
<script language="JavaScript">
<!--Hide JavaScript
	var a = document.all.item("<%=Request("setFocus")%>");
	if (a!=null) {
	    document.all.<%=Request("setFocus")%>.focus();
	}
//-->
</script>
<%End If%>
