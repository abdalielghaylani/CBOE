<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<% 
dim Cmd
dim RS
function GetFieldType( strField )
    colPos = instrrev( strField, ":" )
	fieldType = mid( strField, colPos + 1 )
	if instr(fieldType,"|")>0 then 
	    TempFields = split(fieldType,"|")
	    fieldType = TempFields(0)
	end if  
	strType = ""
	select case lcase( fieldType )
		'numeric
		case "16","2","3","20","17","18","29","21","4","5","6","14","131","11","139","number"
			strType = "numeric"
		'text
		case "12","8","129","200","201","130","202","203","varchar2","char","nchar"
			strType = "text"
		'date
		case "7","133","date"
			strType = "date"
	end select
	GetFieldType = strType
end function

Dim Conn
Dim aPKColumns
Dim pkIDs
Dim pkID
Dim fieldType
action = Request("action")
TableName = Request("TableName")
IDColumnName = Request("IDColumnName")
dbTableName = Request("dbTableName")
pkColumnName = Request("pkColumnName")

'for each key in Request.QueryString
'	Response.Write key & "=" & request(key) & "<BR>"
'next

if ( action = "edit" ) or ( action = "delete" ) then
    ' Grab the PK value(s) and add any special characters if required.
    ' pkID is a temporary variable, read in from the query string.  pkIDs is the concatenated
    ' version of all the primary keys together, which is required by the API asp files
    ' for submission to Oracle when editing or deleting a record.
    aPKColumns = split( pkColumnName, "," )
    pkIDs = ""
    for i = 0 to ubound( aPKColumns )
        if( i <> 0 ) then
            ' Database functions to manipulate these records expect PK values to be colon-delimited
            pkIDs = pkIDs & ":"
        end if 
        pkID = Request( aPKColumns(i) )
        fieldType = GetFieldType( pkID )
        pkID = left( pkID, instrrev( pkID, ":" ) - 1 )
        if instr(pkID,":")>0 then 
          pkID = split(pkID,":")(0)
        end if 
        if( fieldType = "text" ) then
            pkIDs = pkIDs & "'" & pkID & "'"
        else
            pkIDs = pkIDs & pkID
        end if
    next
end if

'Response.Write "pkIDs= " & pkIDs & "<br>"
'Response.End

if action = "edit" then
	bEnabled = true
	actionText = "Edit this information."
elseif action = "create" then
	bEnabled = true
	actionText = "Enter information."
else
	bEnabled = false
	actionText = "Delete this row of information?"
end if

'for each key in Request.QueryString
	'Response.Write key & "=" & request(key) & "<BR>"
'next

numericFields = " "
textFields = " "
dateFields = " "

fieldsNotShown = "RID,CREATOR,TIMESTAMP,TableName,dbTableName,action,pkColumnName,IDColumnName," & IDColumnName

' If the table is inv_project_job_info or inv_owners, then the primary key is editable by the user
' and needs to be displayed here.  Otherwise, do not display the primary key.

if ( instr( "inv_project_job_info,inv_owners", lcase( dbTableName ) ) = 0 ) then
    fieldsNotShown = fieldsNotShown & "," & pkColumnName
end if

if ( instr( "inv_suppliers", lcase( dbTableName ) ) > 0 ) then
    fieldsNotShown = fieldsNotShown & ",SUPPLIER_ADDRESS_ID_FK"
end if

' build list of numeric, text, and date fields
For each oField in Request.QueryString
	if instr(fieldsNotShown,oField) = 0 then
		fieldType = GetFieldType( Request( oField ) )
		select case lcase( fieldType )
		    'numeric
		    case "numeric"
			    numericFields = numericFields & oField & ","
		    'text
		    case "text"
			    textFields = textFields & oField & ","
		    'date
		    case "date"
			    dateFields = dateFields & dateFields & ","
	    end select	
	        tempFieldType= split(Request( oField ),"|")
	        TempString= lcase( fieldType ) & "|" & tempFieldType(1) & "|" &  tempFieldType(2)
	        FieldString = FieldString &	oField & "|" & TempString  & "::"
	 end if
next

numericFields = trim(left(numericFields,len(numericFields)-1))
textFields = trim(left(textFields,len(textFields)-1))
dateFields = trim(left(dateFields,len(dateFields)-1))
FieldString = trim(left(FieldString,len(FieldString)-2))

'Response.Write numericFields & "=numericFields<BR>"
'Response.Write textFields & "=textFields<BR>"
'Response.Write dateFields & "=dateFields<BR>"
'Response.End

%>
<html>
<head>
	<title><%=Application("appTitle")%> -- Edit or Delete a Table Row</title>
	<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
	<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
	<SCRIPT LANGUAGE="">
	// Validates container attributes
	function ValidateRow(strMode){	
		var bWriteError = false;
		var MainForm = document.form1;
		var errmsg = "Please fix the following problems:\r";
		strMode = strMode.toLowerCase();
		
		if( strMode == "edit" || strMode == "create") {
			<%
			arrNumericFields = split(numericFields,",")
			for i = 0 to ubound(arrNumericFields)
				field = arrNumericFields(i)
				Response.Write "if (document.form1." & field & ".value.length > 0) {"
				Response.Write "if (!isNumber(document.form1." & field & ".value)) {"
				Response.Write "errmsg = errmsg + ""- " & field & " must be a number.\r"";"
				Response.Write "bWriteError = true;}}"
			next		

			arrDateFields = split(dateFields,",")
			for i = 0 to ubound(arrDateFields)
				field = arrNumericFields(i)
				Response.Write "if (document.form1." & field & ".value.length > 0) {"
				Response.Write "if (!isDate(document.form1." & field & ".value)) {"
				Response.Write "errmsg = errmsg + ""- " & field & " must be in MM/DD/YYYY format.\r"";"
				Response.Write "bWriteError = true;}}" 
			next
			ArrFields = split(FieldString,"::")
			for each field in ArrFields
			    ArrfieldValue = split(field,"|")
			    if ArrfieldValue(3)="N" then
				    Response.Write "if (trim(document.form1." & ArrfieldValue(0) & ".value).length == 0) {" 
                    Response.Write "errmsg = errmsg + ""- " & ArrfieldValue(0) & " cannot be blank .\r"";" 
				    Response.Write "bWriteError = true;}" 
					'To fix CSBR: 132292
					' Don't allow | character, since its used as a delimiter here
					' Only add the error msg if not added
				    Response.Write "if (document.form1." & ArrfieldValue(0) & ".value.indexOf('|') >= 0) {"
					Response.Write "if (errmsg.indexOf('|') < 0){"
                    Response.Write "errmsg = errmsg + ""- " & ArrfieldValue(0) & " cannot contain | .\r"";}" 
				    Response.Write "bWriteError = true;}" 
				end if     
                if ArrfieldValue(1)="text" then
                    Response.Write "if (document.form1." & ArrfieldValue(0) & ".value.length > " & cint(ArrfieldValue(2)) & ") {" 
                    Response.Write "errmsg = errmsg + ""- " & ArrfieldValue(0) & " length must be less or equal to " & cint(ArrfieldValue(2)) & "  .\r"";" 
				    Response.Write "bWriteError = true;}" 
					'To fix CSBR: 132292
					' Don't allow | character, since its used as a delimiter here
					' Only add the error msg if not added
				    Response.Write "if (document.form1." & ArrfieldValue(0) & ".value.indexOf('|') >= 0) {"
					Response.Write "if (errmsg.indexOf('|') < 0){"
                    Response.Write "errmsg = errmsg + ""- " & ArrfieldValue(0) & " cannot contain | .\r"";}" 
				    Response.Write "bWriteError = true;}" 
				elseif ArrfieldValue(1)="numeric" then 
				    ArrDataVal = split(ArrfieldValue(2),",")
   				    if ArrDataVal(0)<> "" and not isNull(ArrDataVal(0)) Then
				        maxval = ArrDataVal(0) + 1
				        DifferenceVal = cint(ArrDataVal(0))- cint(ArrDataVal(1))
				        if ArrDataVal(0)>0 then 
				            Response.Write "if ((document.form1." & ArrfieldValue(0) & ".value.indexOf('.') ==-1 && document.form1." & ArrfieldValue(0) & ".value.length > " & cint(DifferenceVal) & ") || (document.form1." & ArrfieldValue(0) & ".value.indexOf('.') !=-1 && document.form1." & ArrfieldValue(0) & ".value.indexOf('.') > " & cint(DifferenceVal) & ")) {" 
                            Response.Write "errmsg = errmsg + ""- " & ArrfieldValue(0) & " Data type is Number(" & ArrDataVal(0) & "," & ArrDataVal(1) & ") .\r"";" 
				            Response.Write "bWriteError = true;}" 
				        end if 
				    end if    
				end if     
			next 
			%>
		}
		// Report problems
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else
		{		    
			if( strMode == "delete" )
			{
			    MainForm.action = "DeleteTableRow_action.asp";
			}
			else if( strMode == "create" )
			{
				MainForm.action = "CreateTableRow_action.asp";				
			}
			else
			{
			    MainForm.action = "EditTableRow_action.asp";				
			}
			MainForm.submit();
		}
	}
// Removes leading whitespaces
function trim(str)
{  while(str.charAt(0) == (" ") )
  {  str = str.substring(1);
  }
  while(str.charAt(str.length-1) == " " )
  {  str = str.substring(0,str.length-1);
  }
  return str;
}

	</SCRIPT>	
</head>
<body onload="javascript:SetToFirstControl();">

<center>
<form name="form1" method="POST">
<INPUT TYPE="hidden" NAME="TableName" VALUE="<%=TableName%>">
<INPUT TYPE="hidden" NAME="dbTableName" VALUE="<%=dbTableName%>">
<INPUT TYPE="hidden" NAME="pkColumnName" VALUE="<%=pkColumnName%>">
<%
if action="create" then
	
	Response.Write "<INPUT TYPE=""hidden"" NAME=""IDColumnName"" VALUE=""" & IDColumnName & """>"
	if instr(TableName,"(") > 0 then
		IDValue = Request(IDColumnName)
		colPos = instr(IDValue,":")
		fieldValue = left(IDValue,colPos - 1)
		fieldType = mid(IDValue,colPos + 1)
		Response.Write "<INPUT TYPE=""hidden"" NAME=""IDValue"" VALUE=""" & fieldValue & """>"
	end if
end if
%>
<INPUT TYPE="hidden" NAME="pkIDs" VALUE="<%=pkIDs%>">
<INPUT TYPE="hidden" NAME="numericFields" VALUE="<%=numericFields%>">
<INPUT TYPE="hidden" NAME="textFields" VALUE="<%=textFields%>">
<INPUT TYPE="hidden" NAME="dateFields" VALUE="<%=dateFields%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=actionText%></span><br><br>
		</td>
	</tr>
	<%
	Response.Write BuildForm(bEnabled)
	%>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRow('<%=action%>'); return false;"><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21" id=image1 name=image1></a></td>
	</tr>	

</table>	
</form>
</center>
</body>
</html>
<%
Function BuildForm(bEnabled)
	if not bEnabled then
		enableString = "CLASS=""GrayedText"" READONLY"
	else
		enableString = ""
	end if
	theForm = ""
    For each oItem in Request.QueryString
	    if instr(fieldsNotShown,oItem) = 0 then
		    qsValue = Request(oItem)
		    colPos = instrrev(qsValue,":")
		    fieldValue = left(qsValue,colPos - 1)
		    if(lcase(action)="edit") or (lcase(action)="delete") then
		         fieldValue1 = split(fieldValue,":")
                 if instr(fieldValue,":")>0 then 
                    fieldValue = split(fieldValue,":")(0)
                 else
                    fieldValue=fieldValue1(0)
                 end if 
		    end if
		    fieldType = mid(qsValue,colPos + 1)
		' to fix CSBR - 146602; Fetching all the Compound_ids into the dropdown will result in timeout at the customer with lot of records
		' Immediate fix is to make this field a text box and type the value instead of a dropdown.
		    if Ucase(Right(oItem,2))="FK" and Ucase(oItem)<>"GRID_FORMAT_TYPE_FK" and Ucase(dbTableName) <> "INV_SOLVENTS" then    
		    theForm = theForm & "<TR><TD ALIGN=""right"" NOWRAP>" & oItem & ":</TD>" 
		    theForm = theForm & GetFKValues(oItem,fieldValue,dbTableName,Application("CHEMINV_USERNAME"))
		    else 
		    theForm = theForm & "<TR><TD ALIGN=""right"" NOWRAP>" & oItem & ":</TD><TD><INPUT TYPE=""text"" NAME=""" & oItem & """ VALUE=""" & fieldValue & """" & enableString & "></TD></TR>"
		    end if
	    end if
	next
	
	BuildForm = theForm
end function

FUNCTION GetFKValues(Field,l_fieldValue,dbTableName,Owner)
Str=""
theValue=""
GetInvConnection()
Set Cmd = GetCommand(Conn, Application("CHEMINV_USERNAME") & ".GUIUTILS.GETFKReference", 4)
Cmd.Parameters.Append Cmd.CreateParameter("pColumnName", 200, 1,4000, Field)
Cmd.Parameters.Append Cmd.CreateParameter("pTableName", 200, 1,255, dbTableName)
Cmd.Parameters.Append Cmd.CreateParameter("pOwner", 200, 1,255, Owner)

Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE

Str=Str & "<TD><Select name=""" & Field & """>"&_
           "<option value=""""></option>"
DO WHILE NOT rs.eof
    strSelected = ""
    theValue = RS("Value").value
    if(l_fieldValue=Cstr(theValue))then 
        Str=Str & "<option selected VALUE=""" & theValue & """>" & theValue &"</option>"
    else
        Str=Str & "<option  VALUE=""" & theValue & """>" & theValue &"</option>"
    end if    
    rs.movenext
LOOP
GetFKValues=Str & "</select>"

end function

%>
