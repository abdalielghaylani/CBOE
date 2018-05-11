<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim RS
Dim Cmd
If Request.QueryString("Updatebtchfld")<>"" then %>
        <!--#INCLUDE VIRTUAL = "/cheminv/gui/GetBatchTypeFields.asp"-->
<%      
        Dim BatchName1
        Dim BatchName2
        Dim BatchName3
        GetBatchFiledName()
 Else 
        '-- get the current Grouping information
        Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".Batch.GetBatchFields()}", adCmdText)	
        Cmd.Properties ("PLSQLRSet") = TRUE  
        Set rs = Cmd.Execute
        Cmd.Properties ("PLSQLRSet") = FALSE

        while not rs.EOF 
	        if rs("sort_order") = "1" AND rs("batch_type_id_fk") = "1" then
		        field1 = rs("field_name")
		        displayName1 = rs("display_name")
	        end if
	        if rs("sort_order") = "2" AND rs("batch_type_id_fk") = "1" then
		        field2 = rs("field_name")
		        displayName2 = rs("display_name")
	        end if
	        if rs("sort_order") = "3" AND rs("batch_type_id_fk") = "1" then
		        field3 = rs("field_name")
		        displayName3 = rs("display_name")
	        end if
	        if rs("sort_order") = "1" AND rs("batch_type_id_fk") = "2" then
		        field1_2 = rs("field_name")
		        displayName1_2 = rs("display_name")
	        end if
	        if rs("sort_order") = "2" AND rs("batch_type_id_fk") = "2" then
		        field2_2 = rs("field_name")
		        displayName2_2 = rs("display_name")
	        end if
	        if rs("sort_order") = "3" AND rs("batch_type_id_fk") = "2" then
		        field3_2 = rs("field_name")
		        displayName3_2 = rs("display_name")
	        end if
	        if rs("sort_order") = "1" AND rs("batch_type_id_fk") = "3" then
		        field1_3 = rs("field_name")
		        displayName1_3 = rs("display_name")
	        end if
	        if rs("sort_order") = "2" AND rs("batch_type_id_fk") = "3" then
		        field2_3 = rs("field_name")
		        displayName2_3 = rs("display_name")
	        end if
	        if rs("sort_order") = "3" AND rs("batch_type_id_fk") = "3" then
		        field3_3 = rs("field_name")
		        displayName3_3 = rs("display_name")
	        end if
	        rs.MoveNext
        wend
        BatchNameArray= split(GetBatchingFieldNames(),";")
   if(UBound(BatchNameArray) > 0) then
        PrimaryBatchName=split(BatchNameArray(0),"|")(0)
        SecondaryBatchName=split(BatchNameArray(1),"|")(0)
        TertiaryBatchName=split(BatchNameArray(2),"|")(0)
    end if
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Grouping Fields</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function Validate(){
		var bWriteError = false;
		var errmsg = "";
		var bField1 = false;
		var bField2 = false;
		var bField3 = false;
		
		var bField1_2 = false;
		var bField2_2 = false;
		var bField3_2 = false;		
		
		var bField1_3 = false;
		var bField2_3 = false;
		var bField3_3 = false;		

		// clearing the form fields 
		if (document.form1.clear.checked)
		{
		  ClearFields();
		  document.form1.submit();
		  return;	 
		}
		
		var field1 = document.form1.field1.value;
		var field2 = document.form1.field2.value;
		var field3 = document.form1.field3.value;
		
		var field1_2 = document.form1.field1_2.value;
		var field2_2 = document.form1.field2_2.value;
		var field3_2 = document.form1.field3_2.value;
		
		var field1_3 = document.form1.field1_3.value;
		var field2_3 = document.form1.field2_3.value;
		var field3_3 = document.form1.field3_3.value;
		
		if (field1.length > 0)
			bField1 = true;
		if (field2.length > 0)
			bField2 = true;
		if (field3.length > 0)
			bField3 = true;
			
	    if (field1_2.length > 0)
			bField1_2 = true;
		if (field2_2.length > 0)
			bField2_2 = true;
		if (field3_2.length > 0)
			bField3_2 = true;

	    if (field1_3.length > 0)
			bField1_3 = true;
		if (field2_3.length > 0)
			bField2_3 = true;
		if (field3_3.length > 0)
			bField3_3 = true;
		
		// Clearing Primary Grouping fileds
		if (document.form1.clear1.checked)
		{
		 if ((bField1_2 || bField2_2 ||  bField3_2 ||  bField1_3 || bField2_3 || bField3_3 ) && !(document.form1.clear2.checked && document.form1.clear3.checked))
		 {
		     alert("- Can not Clear <%=PrimaryBatchName%> Grouping Field, Please remove <%=SecondaryBatchName%> and <%=TertiaryBatchName%> Grouping Fields first.\n")
		     return ;
		 }
		 else
		 {
		     document.form1.clear.checked=true;
		     document.form1.submit();
		      return;	
		 }
		}
		// Clearing Secondary Grouping fileds
		if (document.form1.clear2.checked)
		{
		 if (!document.form1.clear3.checked && (bField1_3 || bField2_3 || bField3_3))
		 {
		     alert("- Can not Clear <%=SecondaryBatchName%> Grouping Field, Please remove  <%=TertiaryBatchName%> Grouping Field first.\n")
		     return ;
		 }
		 else 
		 {
		      ClearFields2();
		      document.form1.submit();
		      return;
		  }	 
		}
		// Clearing Tertiary Grouping filed
		if (document.form1.clear3.checked)
		{
		  ClearFields3();
		  document.form1.submit();
		  return;
		}
		if (!document.form1.clear.checked && !bField1 && !bField2 && !bField3 && !bField1_2 && !bField2_2 && !bField3_2 && !bField1_3 && !bField2_3 && !bField3_3)
		{
		    
		    bWriteError = true
		    errmsg += "- Select atleast one Grouping Field .\n"
		}
		
		// Checking if some one selected  field2 and/or field3 and field 2 or field1 is empty.
		if ((!bField1) && (bField2 || bField3))
		{
		bWriteError = true
		errmsg += "- Select <%=PrimaryBatchName%> Grouping Field 1.\n"
		} 	
		if ((!bField2) && (bField3))
		{
		bWriteError = true
		errmsg += "- Select <%=PrimaryBatchName%> Grouping Field 2.\n"
		}
		
		
		//<%=SecondaryBatchName%> Grouping fields
		if ((!bField1) && (bField1_2 || bField2_2 || bField3_2))
		{
		bWriteError = true
		errmsg += "- Select <%=PrimaryBatchName%> Grouping Field 1.\n"
		} 
		
		if ((!bField1_2) && (bField2_2 || bField3_2))
		{
		bWriteError = true
		errmsg += "- Select <%=SecondaryBatchName%> Grouping Field 1.\n"
		} 
		
		if ((!bField2_2) && (bField3_2))
		{
		bWriteError = true
		errmsg += "- Select <%=SecondaryBatchName%> Grouping Field 2.\n"
		}	


	//Tertiary Grouping fields
		if ((!bField1) && (!bField2) && (bField1_3 || bField2_3 || bField3_3))
		{
		bWriteError = true
		errmsg += "- Select <%=PrimaryBatchName%> and <%=SecondaryBatchName%> Grouping Field 1.\n"
		} 
		
		if ((!bField1_3) && (bField2_3 || bField3_3))
		{
		bWriteError = true
		errmsg += "- Select <%=TertiaryBatchName%> Grouping Field 1.\n"
		} 
		
		if ((!bField2_3) && (bField3_3))
		{
		bWriteError = true
		errmsg += "- Select <%=TertiaryBatchName%> Grouping Field 2.\n"
		}	


		// Checking if some one selected a field in dropdown and not enterered display name 	
		if ((bField1) && (document.form1.displayName1.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=PrimaryBatchName%> Display Name1.\n"	
		}
		if ((bField2) && (document.form1.displayName2.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=PrimaryBatchName%> Display Name2.\n"	
		}
		if ((bField3) && (document.form1.displayName3.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=PrimaryBatchName%> Display Name3.\n"	
		}
		
		//<%=SecondaryBatchName%>
		if ((bField1_2) && (document.form1.displayName1_2.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=SecondaryBatchName%> Display Name1.\n"	
		}
		if ((bField2_2) && (document.form1.displayName2_2.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=SecondaryBatchName%> Display Name2.\n"	
		}
		if ((bField3_2) && (document.form1.displayName3_2.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=SecondaryBatchName%> Display Name3.\n"	
		}
		
				
		//Tertiary
		if ((bField1_3) && (document.form1.displayName1_3.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=TertiaryBatchName%> Display Name1.\n"	
		}
		if ((bField2_3) && (document.form1.displayName2_3.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=TertiaryBatchName%> Display Name2.\n"	
		}
		if ((bField3_3) && (document.form1.displayName3_3.value==""))
		{
		bWriteError = true
		errmsg += "- Enter <%=TertiaryBatchName%> Display Name3.\n"	
		}
		
		bWriteError = false;
		// if field1 is selected make sure it's not the same as field2 or field3
		if (bField1)
		{
			if (bField2) {
				if (field1 == field2) {
					bWriteError = true; 
				}
			}
			if (bField3) {
				if (field1 == field3) {
					bWriteError = true;
				}
			}
		}
		// if field2 is selected make sure it's not the same as field1 or field3
		if (bField2)
		{
			if (bField1) {
				if (field2 == field1) {
					bWriteError = true; 
				}
			}
			if (bField3) {
				if (field2 == field3) {
					bWriteError = true;
				}
			}
		}
		// if field3 is selected make sure it's not the same as field1 or field2
		if (bField3)
		{
			if (bField1) {
				if (field3 == field1) {
					bWriteError = true; 
				}
			}
			if (bField2) {
				if (field3 == field2) {
					bWriteError = true;
				}
			}
		}
		
		
		//<%=SecondaryBatchName%>
		
		if (bField1_2)
		{
			if (bField2_2) {
				if (field1_2 == field2_2) {
					bWriteError = true; 
				}
			}
			if (bField3_2) {
				if (field1_2 == field3_2) {
					bWriteError = true;
				}
			}
		}
		
		
		if (bField2_2)
		{
			if (bField1_2) {
				if (field2_2 == field1_2) {
					bWriteError = true; 
				}
			}
			if (bField3_2) {
				if (field2_2 == field3_2) {
					bWriteError = true;
				}
			}
		}
		
		if (bField3_2)
		{
			if (bField1_2) {
				if (field3_2 == field1_2) {
					bWriteError = true; 
				}
			}
			if (bField2_2) {
				if (field3_2 == field2_2) {
					bWriteError = true;
				}
			}
		}

		//Tertiary
		
		if (bField1_3)
		{
			if (bField2_3) {
				if (field1_3 == field2_3) {
					bWriteError = true; 
				}
			}
			if (bField3_3) {
				if (field1_3 == field3_3) {
					bWriteError = true;
				}
			}
		}
		if (bField2_3)
		{
			if (bField1_3) {
				if (field2_3 == field1_3) {
					bWriteError = true; 
				}
			}
			if (bField3_3) {
				if (field2_3 == field3_3) {
					bWriteError = true;
				}
			}
		}
		
		if (bField3_3)
		{
			if (bField1_3) {
				if (field3_3 == field1_3) {
					bWriteError = true; 
				}
			}
			if (bField2_3) {
				if (field3_3== field2_3) {
					bWriteError = true;
				}
			}
		}
		if (bWriteError){
			errmsg += "- Grouping Fields must be distinct."						
			
		}
		if(errmsg!="")
		{
		errmsg ="Please fix the following problems:\r" + errmsg;
		alert(errmsg);
		}
		 
		else{
			document.form1.submit();
		}
	}
// This function is created to clear all Grouping fields 
function ClearFields()
{
 document.form1.field1.value="";
 document.form1.field2.value="";
 document.form1.field3.value="";
 document.form1.displayName1.value=""
 document.form1.displayName2.value=""
 document.form1.displayName3.value=""
 document.form1.field1_2.value="";
 document.form1.field2_2.value="";
 document.form1.field3_2.value="";
 document.form1.displayName1_2.value=""
 document.form1.displayName2_2.value=""
 document.form1.displayName3_2.value=""
 document.form1.displayName1_3.value=""
 document.form1.displayName2_3.value=""
 document.form1.displayName3_3.value=""

}
// This function is created to clear secondary Grouping field
function ClearFields2()
{
 document.form1.field1_2.value="";
 document.form1.field2_2.value="";
 document.form1.field3_2.value="";
 document.form1.displayName1_2.value=""
 document.form1.displayName2_2.value=""
 document.form1.displayName3_2.value=""
 }
// This function is created to clear Tertiary Grouping field
function ClearFields3()
{
 document.form1.field1_3.value="";
 document.form1.field2_3value="";
 document.form1.field3_3.value="";
 document.form1.displayName1_3.value=""
 document.form1.displayName2_3.value=""
 document.form1.displayName3_3.value=""
 }
function ValidateBatchFields()   
        {   	
            var bWriteError = false;
            var errmsg = "";
            if (document.form1.BatchingField1.value==document.form1.BatchingField2.value==document.form1.BatchingField3.value)
            {
            bWriteError = true
            errmsg += "- Enter Grouping Fields must be distinct.\n"	
            }
            if (document.form1.BatchingField1.value==document.form1.BatchingField2.value)
            {
            bWriteError = true
            errmsg += "- Enter Grouping Fields 1,2 must be distinct .\n"	
            }
            if (document.form1.BatchingField1.value==document.form1.BatchingField3.value)
            {
            bWriteError = true
            errmsg += "- Enter Grouping Fields 1,3 must be distinct .\n"	
            }
            if (document.form1.BatchingField2.value==document.form1.BatchingField3.value)
            {
            bWriteError = true
            errmsg += "- Enter Grouping Fields 2,3 must be distinct .\n"	
            }
            if(document.form1.BatchingField1.value.length==0)
            {        
            bWriteError = true
            errmsg += "- Enter Grouping Field 1.\n"		
            }
            if(document.form1.BatchingField2.value.length==0)
            {        
            bWriteError = true
            errmsg += "- Enter Grouping Field 2.\n"		
            }
            if(document.form1.BatchingField3.value.length==0)
            {        
            bWriteError = true
            errmsg += "- Enter Grouping Field 3.\n"		
            }
            if(bWriteError)
            {
                alert(errmsg);
            }
            else
            {
                document.form1.action="UpdateBatchTypeFields_action.asp";
                document.form1.submit();
            }
        }  
-->
</script>

	
</head>
<body TOPMARGIN="0" LEFTMARGIN="5" BGCOLOR="#FFFFFF">
<br><br>
<form NAME="form1" METHOD="POST" action="ManageBatchFields_action.asp">
        <%if request.QueryString("Updatebtchfld")="" then %>
<table border="0" align="center" width="80%">
	<tr>
		<td colspan="4" align="center">
			<span class="GuiFeedback">
			Please select the database columns that will be used to define a container grouping.<br/><br/>
			</span>
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
			<span class="GuiFeedback">
			WARNING: 
			</span>
		</td>
		<td align="left" colspan="3">
			Changing group fields will delete all current grouping information (including group requests) and recalculate container grouping. This process may take a few minutes.<br/><br/>
		</td>
	</tr>
            <tr>
                <td colspan="4" align="right">
                    <a href="ManageBatchFields.asp?Updatebtchfld=update">Change Grouping Fields Name</a>&nbsp;
                </td>
            </tr>
	<tr>
	<td colspan=4><fieldset>
	<legend><span class="GuiFeedback"><%=PrimaryBatchName%></span></legend>
	
	<table border="0" align="center" width="100%">
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 1:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field1", field1, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName1" size="20" value="<%=displayName1%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
            Grouping Field 2:
		</td>
		<td>
			<%=ShowSelectBox2("field2", field2, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top">
			Display Name:
		</td>
		<td>
			<input type="text" name="displayName2" size="20" value="<%=displayName2%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
            Grouping Field 3:
		</td>
		<td>
			<%=ShowSelectBox2("field3", field3, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top">
			Display Name:
		</td>
		<td>
			<input type="text" name="displayName3" size="20" value="<%=displayName3%>">
		</td>
	</tr><tr>
        <td></td>
        <td></td>
        
        <td align="left" style="padding-left: 60px;" colspan="2"><input type="checkbox" name="clear1" value="1" />Clear <%=PrimaryBatchName%> Grouping Fields</td>	
	</tr></table></fieldset></td></tr>
	<tr>
		<td colspan=4><fieldset>
	<legend><span class="GuiFeedback"><%=SecondaryBatchName%></span></legend>
	
	<table border="0" align="center" width="100%">
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 1:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field1_2", field1_2, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName1_2" size="20" value="<%=displayName1_2%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 2:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field2_2", field2_2, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName2_2" size="20" value="<%=displayName2_2%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 3:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field3_2", field3_2, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName3_2" size="20" value="<%=displayName3_2%>">
		</td>
	</tr><tr>
        <td></td>
        <td></td>
        
        <td align="left" style="padding-left: 60px;" colspan="2"><input type="checkbox" name="clear2" value="1" />Clear <%=SecondaryBatchName%> Grouping Fields</td>	
	</tr></table></fieldset></td></tr>
	
<!--Thrid Grouping -->	
	<tr>
		<td colspan=4><fieldset>
	<legend><span class="GuiFeedback"><%=TertiaryBatchName%></span></legend>
	
	<table border="0" align="center" width="100%">
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 1:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field1_3", field1_3, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName1_3" size="20" value="<%=displayName1_3%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 2:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field2_3", field2_3, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName2_3" size="20" value="<%=displayName2_3%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" width="29%">
            Grouping Field 3:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field3_3", field3_3, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK', 'BATCH_ID2_FK', 'BATCH_ID3_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW','QTY_AVAILABLE','QTY_REMAINING') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName3_3" size="20" value="<%=displayName3_3%>">
		</td>
	</tr><tr>
        <td></td>
        <td></td>
        
        <td align="left" style="padding-left: 60px;" colspan="2"><input type="checkbox" name="clear3" value="1" />Clear <%=TertiaryBatchName%> Grouping Fields</td>	
	</tr></table></fieldset></td></tr>
	
	<tr>
        <td></td>
        <td></td>
       
        <td align="right" colspan="4"><input type="checkbox" name="clear" value="1" />Clear All Grouping Fields</td>	
	</tr>
	<tr>
	<td colspan="4" align="right">
			<a HREF="#" onclick="window.location.replace('/cheminv/gui/menu.asp');"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
	</td>
	</tr>
            <%else %>
           
            <tr>
            
                <center>
                    <fieldset style="width: 60%; height: 20%;">
                        <legend style="margin-top:150px"><span class="GuiFeedback">Grouping Fields</span></legend>
                        <table border="0" align="center" width="60%">                            
                                <tr>
                                    <td align="right" valign="top" style="height: 26px; width: 32%;">
                                         Grouping Field 1 :
                                    </td>
                                    <td style="width: 28%; height: 26px">
                                        <input name="BatchingField1" type="text" value="<%=BatchName1%>" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" valign="top" style="height: 26px; width: 32%;">
                                        Grouping Field 2:
                                    </td>
                                    <td style="width: 28%; height: 26px">
                                        <input name="BatchingField2" type="text" value="<%=BatchName2%>" /></td>
                                </tr>
                                <tr>
                                    <td align="right" valign="top" style="height: 26px; width: 32%;">
                                        Grouping Field 3:
                                    </td>
                                    <td style="width: 28%; height: 26px">
                                        <input name="BatchingField3" type="text" value="<%=BatchName3%>" /></td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2" style="height: 40px;">
                                        <a href="#" onClick="window.location.replace('/cheminv/gui/menu.asp');">
                                            <img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;&nbsp;
                                        <a href="#" onclick="ValidateBatchFields(); return false;">
                                            <img src="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
                                </tr>
                        </table>
                    </fieldset>
                </center>
            </tr>
            <%  end if  %>
          </table>
</form>
</body>
</html>
