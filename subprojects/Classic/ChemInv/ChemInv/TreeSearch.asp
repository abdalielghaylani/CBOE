<%
dbkey = "ChemInv"
ServerName = Request.ServerVariables("Server_Name")

BatchField1 = Request("Batch_Field_1")
BatchField2 = Request("Batch_Field_2")
BatchField3 = Request("Batch_Field_3")
ContainerSize = Request("ContainerSize")
OpenPositions = Request("OpenPositions")
RestrictSize = Request("RestrictSize")
Submit = Request("Submit")
numSamples = Request("numSamples")
contSize = Request("contSize")
showResults = Request("showResults")
treeId = Request("TreeID")
BatchType=Request("Batch_Type")
if not IsEmpty(Application("DEFAULT_SAMPLE_UOM")) then
	UOMAbv = Application("DEFAULT_SAMPLE_UOM")
else
	UOMAbv = Request("UOMAbv")
	if isEmpty(UOMAbv) then UOMAbv = "1=ml" 
end if

arrUOMValue = split(Request("UOM"),"=")
if Request("UOM") <> "" then
	ContainerSizeUOM = arrUOMValue(0)
	UOMAbv = Request("UOM")
end if


'-- do the search
if showResults = "true" then
    
    '-- Show sample requests
    if BatchField1 = "" then BatchField1 = null
    if BatchField2 = "" then BatchField2 = null
    if BatchField3 = "" then BatchField3 = null
    if OpenPositions = "" then OpenPositions = null
    if ContainerSize = "" then ContainerSize = null
    if RestrictSize = "" then RestrictSize = null
    if ContainerSizeUOM = "" then ContainerSizeUOM = null

    'Response.Write(Request.Form)

    RequestTypeID = 1
    Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".RACKS.SEARCHRACKS(?,?,?,?,?,?,?,?)}", adCmdText)	
    Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHFIELD1",200, 1, 30, BatchField1)
    Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHFIELD2",200, 1, 30, BatchField2)
    Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHFIELD3",200, 1, 30, BatchField3)
    Cmd.Parameters.Append Cmd.CreateParameter("P_OPENPOSITIONS",131, 1, 0, OpenPositions)
    Cmd.Parameters.Append Cmd.CreateParameter("P_CONTAINERSIZE",131, 1, 0, ContainerSize)
    Cmd.Parameters.Append Cmd.CreateParameter("P_RESTRICTSIZE",200, 1, 10, RestrictSize)
    Cmd.Parameters.Append Cmd.CreateParameter("P_CONTAINERSIZEUOM",131, 1, 0, ContainerSizeUOM)
    Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHTYPE",131, 1, 0, BatchType)
    Cmd.Properties ("PLSQLRSet") = TRUE  
	'For each p in Cmd.Parameters
	'	Response.Write p.name & " = " & p.value & "<BR>"
	'Next
    Set RS = Cmd.Execute
end if

%>
<script type="text/javascript" language="JavaScript">
    //function to change the labels of batch fields
    function ResetBatchFielslabels(batchTypeid) {
        var TempBatchStr = document.getElementById('FieldStringBatch' + batchTypeid);
        if (TempBatchStr) {
            if (TempBatchStr.value == '') {
                for (i = 1; i <= 3; i++) {
                    var BatchFld = document.getElementById("BatchFld" + i);
                    if (BatchFld) {
                        BatchFld.style.display = 'none';
                    }
                    var batch_field = document.getElementById("batch_field_" + i);
                    if (batch_field) {
                        batch_field.value = '';
                    }
                    var reg_number = document.getElementById("reg_number");
                    if (reg_number) {
                        reg_number.value = '';
                    }
                }
            }
            else {
                var arrBatchValues = TempBatchStr.value.split("|");
                for (i = 0; i < arrBatchValues.length; i++) {
                    var dispVal = arrBatchValues[i].split(',')[0];
                    var batchField = arrBatchValues[i].split(',')[1];
                    var BatchFieldDisplay = document.getElementById("BatchFieldDisplay" + batchField);
                    if (BatchFieldDisplay) {
                        BatchFieldDisplay.innerHTML = dispVal + ":";
                    }
                    var BatchFld = document.getElementById("BatchFld" + batchField);
                    if (BatchFld) {
                        BatchFld.style.display = 'block';
                    }
                    var batch_field = document.getElementById("batch_field_" + batchField);
                    if (batch_field) {
                        batch_field.value = '';
                        var FieldName = arrBatchValues[i].split(",")[2];
                        if (FieldName == "REG_ID_FK") {
                            batch_field.title = 'Example AB-000001';
                        }
                        else {
                            batch_field.title = '';
                        }
                    }
                }
                for (i = arrBatchValues.length + 1; i <= 3; i++) {
                    var BatchFld = document.getElementById("BatchFld" + i);
                    if (BatchFld) {
                        BatchFld.style.display = 'none';
                    }
                    var batch_field = document.getElementById("batch_field_" + i);
                    if (batch_field) {
                        batch_field.value = '';
                    }
                }
            }
        }
    }				
</script>

<form name="form1" action="BrowseTree.asp?<%=Request.Querystring%>" method="POST">
<input type="hidden" name="showResults" value="true" />
<input type="hidden" name="treeId" value="<%=treeId%>" />
<div style="margin: -25px 0px 0x -5px; padding: 15px 0px 0px 25px;">

		<br />
		<br />
	<% if showResults <> "true" then %>
		<span class="guifeedback">Search Criteria</span>
	<% elseif showResults = "true" then %>
		<span class="guifeedback">Search Results</span>
	<% end if %>
	
<% 
if showResults <> "true" then 
%>	
	<br />
    <table>
        <tr>
        <% if cint(Application("NumBatchTypes")) >=1 then %>
           <span title="Select a batch Type">Batch type:</span><br>
			   <%= ShowSelectBox3("batch_type", "", "SELECT ID AS Value, Batch_type AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_batch_type where id in (select distinct batch_type_id_fk from "& Application("CHEMINV_USERNAME") & ".inv_container_batch_fields) ORDER BY Batch_order ASC", 30, "", "","ResetBatchFielslabels(this.value);")%>
                <tr id="BatchFld1">
		        		<td>
							<span id="BatchFieldDisplay1" style="display:block">BatchFieldDisplay1:</span>
                            <input type="text" size="10" name="batch_field_1" id="batch_field_1" />
						</td>
					</tr>
                    <tr id="BatchFld2">
		        		<td>
							<span id="BatchFieldDisplay2" style="display:block">BatchFieldDisplay2:</span>
							 <input type="text" size="10" name="batch_field_2" id="batch_field_2" />
						</td>
					</tr>
                    <tr id="BatchFld3">
		        		<td>
							<span id="BatchFieldDisplay3" style="display:block">BatchFieldDisplay3:</span>
							 <input type="text" size="10" name="batch_field_3" id="batch_field_3" />
						</td>
					</tr>
                    <% Call GetInvConnection()
					SQL = "SELECT FIELD_NAME,DISPLAY_NAME, SORT_ORDER, BATCH_TYPE_ID_FK FROM " & Application("CHEMINV_USERNAME") & ".inv_container_batch_fields ORDER BY SORT_ORDER"
					Set Cmd = GetCommand(Conn, SQL, adCmdText)
					Set RS = Cmd.Execute
					'count=1
					FieldStringBatch1=""
					FieldStringBatch2=""
					FieldStringBatch3=""
					if not (rs.eof and rs.bof) then
					do while not rs.eof 
					 
					if RS("BATCH_TYPE_ID_FK")="1" then 
					     if FieldStringBatch1="" then 
					         FieldStringBatch1= RS("DISPLAY_NAME") & "," & RS("SORT_ORDER") & "," & RS("FIELD_NAME")
					     else    
					        FieldStringBatch1= FieldStringBatch1 & "|" & RS("DISPLAY_NAME") & "," & RS("SORT_ORDER") & "," & RS("FIELD_NAME")
					     end if   
					elseif RS("BATCH_TYPE_ID_FK")="2" then 
					    if FieldStringBatch2="" then 
					         FieldStringBatch2= RS("DISPLAY_NAME") & "," & RS("SORT_ORDER") & "," & RS("FIELD_NAME")
					     else    
					        FieldStringBatch2= FieldStringBatch2 & "|" & RS("DISPLAY_NAME") & "," & RS("SORT_ORDER") & "," & RS("FIELD_NAME")
					     end if   
					else 
					    if FieldStringBatch3="" then 
					         FieldStringBatch3= RS("DISPLAY_NAME") & "," & RS("SORT_ORDER") & "," & RS("FIELD_NAME")
					     else    
					        FieldStringBatch3= FieldStringBatch3 & "|" & RS("DISPLAY_NAME") & "," & RS("SORT_ORDER") & "," & RS("FIELD_NAME")
					     end if   
					 end if%>
					
					<%
					RS.MoveNext
					Loop
					End If%> 	
					<input Type="hidden" name="FieldStringBatch1" id="FieldStringBatch1" value="<%=FieldStringBatch1%>" />
					<input Type="hidden" name="FieldStringBatch2" id="FieldStringBatch2" value="<%=FieldStringBatch2%>" />
					<input Type="hidden" name="FieldStringBatch3" id="FieldStringBatch3" value="<%=FieldStringBatch3%>" />
                    <script language="javascript"> ResetBatchFielslabels(1);</script> 
                    <%end if  %>
            <td>
                
	            Number of open positions:<br />
	            <input type="text" size="3" name="OpenPositions" id="OpenPositions" value="<%=OpenPositions%>" /><br />
	            Container Size:<br />
	            <table cellpadding="0" cellspacing="0"><tr><td>
		            <input type="text" size="3" name="ContainerSize" id="ContainerSize" value="<%=ContainerSize%>" />&nbsp;
	            </td><td>
	            <%Response.write ShowPickList2("", "UOM", UOMAbv,"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC","300"," ","","") & vbcrlf%>
	            </td></tr></table>
            <br />
	        <input type="Submit" onclick="Validate(); return false;" name="Submit" value="Submit" />
            </td>
        </tr>
    </table>
<!--
    	<a href="BrowseTree.asp?BatchField1=<%=BatchField1%>&BatchField2=<%=BatchField2%>&BatchField3=<%=BatchField3%>&ContainerSize=<%=ContainerSize%>&OpenPositions=<%=OpenPositions%>&FormStep=&RestrictSize=<%=RestrictSize%>&TB=Search">Refine</a>
        &nbsp;|&nbsp;
    	<a href="BrowseTree.asp?TB=Search">New Search</a>
-->    	
<% 
elseif showResults = "true" then
    response.Write "<br /><br />"
    if (RS.EOF and RS.BOF) then
	    Response.Write ("<span class=""GUIFeedback"" style=""display: block; width: 100%; padding: 5px;"">No Racks matched your search criteria.</span>")
    else
	    Response.Write("<table cellpadding=""3"" cellspacing=""5"" style=""border-bottom: 1px solid #ccc;"">")
	    Response.write("<tr><td colspan=""4"">")
    	%>
    	<a href="BrowseTree.asp?TreeID=<%=TreeID%>&BatchField1=<%=BatchField1%>&BatchField2=<%=BatchField2%>&BatchField3=<%=BatchField3%>&ContainerSize=<%=ContainerSize%>&FormStep=&RestrictSize=<%=RestrictSize%>&TB=Search&UOM=<%=request("UOM")%>">Refine</a>
        &nbsp;|&nbsp;
    	<a href="BrowseTree.asp?TreeID=<%=TreeID%>&TB=Search">New Search</a>
        <%
	    Response.Write("</td></tr>")
	    Response.write("<tr><td colspan=""4"">")
	    Response.Write("<strong>Search Criteria:</strong> ")
	    criteria = "  "
	    if BatchField1 <> "" then criteria = criteria & "Batch Field 1: " & BatchField1 & ", "
	    if BatchField2 <> "" then criteria = criteria & "Batch Field 2: " & BatchField2 & ", "
	    if OpenPositions <> "" then criteria = criteria & "Open Positions: " & OpenPositions & ", "
	    if ContainerSize <> "" then criteria = criteria & "ContainerSize: " & ContainerSize & "mg, "
	    criteria = trim(left(criteria,len(criteria)-2))
	    response.Write criteria
	    Response.Write("</td></tr>")
	    Response.write("<tr><th>Rack Name</th><th>Open Pos.</th><th>Filled Pos.</th><th>Location</th></tr>")
	    url = "BrowseTree.asp?TB=Browse&isRack=true&ClearNodes=0&TreeID=" & TreeID & "&MaybeLocSearch=" & MaybeLocSearch & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&LocationPickerID=" & LocationPickerID & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&GotoNode=0"
	    'BrowseTree.asp?isRack=true&snode="& RS("Location_ID") &"&ClearNodes=0&TB=Browse&TreeID=" & treeId
	    While NOT RS.EOF
	    
		    'Response.write("<tr><td valign=""middle""><a href=""BrowseTree.asp?isRack=true&snode="& RS("Location_ID") &"&ClearNodes=0&TB=Browse&TreeID=" & treeId & """><img src=""/cheminv/images/treeview/rack_closed.gif"" border=""0"" />&nbsp;" & RS("location_name") & "</a></td>")
		    Response.write("<tr><td valign=""middle""><a href=""" & url & "&snode=" & RS("Location_ID") & """><img src=""/cheminv/images/treeview/rack_closed.gif"" border=""0"" />&nbsp;" & RS("location_name") & "</a></td>")
		    Response.Write("<td align=""right"">" & RS("OpenPositions") & "</td>")
		    Response.Write("<td align=""right"">" & RS("FilledPositions") & "</td>")
		    Response.Write("<td>" & TruncateInSpan(RS("LocationPathFull"), 35, "")  & "</td></tr>")
		    'Response.Write("<td><a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewSimpleRackLayout.asp?rackid=" & RS("location_id") & "&locationid=" & RS("location_id") & "&containerid=&RackPath=', 'Diag1', 2); return false;"">" & RS("LocationPath") & "</a></td></tr>")
		    'Response.Write("<td><a class=""MenuLink"" href=""#"" title=""" & RS("LocationPathFull") & """>" & RS("LocationPath") & "</a></td></tr>")
	    RS.MoveNext
	    Wend
	    RS.Close
	    Set RS = nothing
	    Set Conn = nothing
	    Response.Write("</table>")
    end if 
end if
%>
</div>
  <script language="javascript">
  function Validate()
  {
   if ((document.form1.OpenPositions.value > 0) || (document.form1.OpenPositions.value=="")) {
	 document.form1.submit();
	}
	else{
		alert("- Number of open positions must be a positive number.\r");
		return false;
				}
  }
  </script>
</form>

