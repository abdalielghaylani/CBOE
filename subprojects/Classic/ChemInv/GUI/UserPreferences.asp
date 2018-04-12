<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetUserPreferencesAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<%
Source = Request("Source")
if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%
Dim Conn
Dim cmd
UserPreferencesFieldsStr = Application("UserPreferencesFieldsStr")
FieldsArray = split(UserPreferencesFieldsStr,";")
if Application("ENABLE_OWNERSHIP")="TRUE" then
  if session("Principal_ID_FK") <>"" then
  PrincipalID=session("Principal_ID_FK")
  else
  PrincipalID="0"
  end if 
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Container Preferences</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/CalculateFromPlugin.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="JavaScript">
<!--Hide JavaScript
	window.focus();
function ValidateContainer(){
		var bWriteError = false;
		var bWriteWarning = false;
		var errmsg = "Please fix the following problems:\r\r";
		var warningmsg = "Please read the following warnings:\r\r";
        <% for each item in FieldsArray
            FieldArray = split(Application(item), ":")
            
            if FieldArray(4) = "1" and FieldArray(5) = "1" and FieldArray(0) = "1" Then%>
           	if (!isPositiveNumber(document.form1.<%=FieldArray(2)%>.value) && (document.form1.<%=FieldArray(2) %>.value != 0) && (document.form1.<%=FieldArray(2)%>.value != "")){
				errmsg = errmsg + "- <%=FieldArray(3)%> must be a positive number.\r";
				bWriteError = true;
			}
			var m = document.form1.<%=FieldArray(2)%>.value;
			if(m.toString().indexOf(",") != -1){
			errmsg = errmsg + "- <%=FieldArray(3)%> has wrong decimal operator.\r";
			bWriteError = true;
			}

            <% end if
            if FieldArray(4) = "2" and FieldArray(5) = "2" and FieldArray(0) = "1" Then%> 
        	if (document.form1.<%=FieldArray(2)%>.value.length > 0 && !isDate(document.form1.<%=FieldArray(2)%>.value)){
				errmsg = errmsg + "- <%=FieldArray(3)%> must be in " + dateFormatString + " format.\r";
				bWriteError = true;
			}
        
        <% end if 
        next%>
        if (bWriteError){
            alert(errmsg)
            return false;
         }
        document.form1.action = "UserPreferences_action.asp";
        document.form1.submit();
        }

  function ResetForm(StrForm) 
  {
    var frm_elements = form1.elements;
    for(i=0; i<frm_elements.length; i++)
    {   
        field_type = frm_elements[i].type.toLowerCase();   
         switch (field_type)   
          {    
          case "text":    
               frm_elements[i].value = "";       
               break; 
          case "textarea": 
               frm_elements[i].value = "";       
               break;     
          case "hidden":        
               if (frm_elements[i].name != 'tempCsUserName' && frm_elements[i].name != 'tempCsUserID' && frm_elements[i].name != 'CurrentUserID' && frm_elements[i].name != 'PageSource')
               frm_elements[i].value = "";       
               break;    
          case "select-one":
               frm_elements[i].value = "";       
               break;      
          default:        
              break;   
          }
    }
  }
-->
</script>
</head>
<body style="overflow:auto">
<%
if lcase(Source) = "eln" and Session("UserPreferenceLoginErrorMessage" & "cheminv") <> "" Then
	Response.write "<BR/><SPAN class=""GuiFeedback""><CENTER>" & Session("UserPreferenceLoginErrorMessage" & "cheminv") & "</CENTER></SPAN>"
	Response.End
end if
%>
<center>
<form name="form1" method="POST">
<input TYPE="hidden" NAME="ExpDate" Value>
<input TYPE="hidden" NAME="CurrentUserID" Value>
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" id="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>>
<input TYPE="hidden" NAME="PageSource" Value="<%=Source%>">
<span class="GuiFeedback">Please set your preference for container creation.</span><br><br>
<center><span class="GuiFeedback">Note: 	</span>  Red coloured fields indicate mandatory fields when Creating a container,<br />
 but they are for information only and not required here to save your user preference<br /><br /></center>
<table border="0">
	<% 
    for each item in FieldsArray
        FieldArray = split(Application(item), ":")
        if FieldArray(0) ="1" then 
           if lcase(trim(FieldArray(2))) = "principal_id_fk" then
                if Application("ENABLE_OWNERSHIP")="TRUE" then 
                    LocationAdmin =1
                %>
                    <input TYPE="hidden" NAME="OwnerShipGroupList" id="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
                    <input TYPE="hidden" NAME="OwnerShipUserList" id ="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
                    <input TYPE="hidden" NAME="Principal_ID_FK" id="PrincipalID" Value=<%=PrincipalID%>>
                    <input type="hidden" NAME="OwnershipType" id="OwnershipType" value="<%=OwnershipType%>" />
                    <tr><%Response.Write "<td colspan=""3"">" & iif((FieldArray(1) = "0" and FieldArray(0)= "1") ,FieldArray(3),"<span class=""required"">" & FieldArray(3) & "</span>") %>
                        <td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="getList(OwnerShipUserList.value,null);setPrincipalID(document.form1.Ownershiplst);"/>by user
		                <input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="getList(OwnerShipGroupList.value,null);setPrincipalID(document.form1.Ownershiplst);" />by Group</td></tr>
		                <tr><td colspan="3">&nbsp;</td><td ><SELECT id="Ownershiplst" onchange="setPrincipalID(this);" ><OPTION></OPTION></SELECT></td></tr></table></div></td>
	                </tr>
                <% end if
           else
                Response.Write "<td colspan=""3"">" & iif((FieldArray(1) = "0" and FieldArray(0)= "1") ,FieldArray(3),"<span class=""required"">" & FieldArray(3) & "</span>") & "</td>"
                if FieldArray(5) = "1"  then 'text box
                    Response.Write "<td><input type=""text"" name=" & trim(FieldArray(2)) & " size=""30"" value=""" & Session(trim(FieldArray(2))) & """></td>"
                elseif FieldArray(5) = "2" then ' date
                    Response.Write "<td>"
                    call ShowInputField("",  Session(trim(FieldArray(2))) , trim(FieldArray(2)) & ":form1:" & Session(trim(FieldArray(2)))  , "DATE_PICKER:TEXT", "15")
                    Response.Write "</td>"  
                elseif FieldArray(5) = "5"  then ' text area
                   Response.Write "<td><textarea name=" & trim(FieldArray(2)) & " size=""30"">" & Session(trim(FieldArray(2))) & "</textarea></td>"
                elseif FieldArray(5) = "3" and FieldArray(2) <> "CURRENT_USER_ID_FK" then ' dropdown
                    Response.Write  "<td>" & ShowSelectBox2(trim(FieldArray(2)), Session(trim(FieldArray(2))), FieldArray(6),255, RepeatString(43, "&nbsp;"), "") & "</td>"
                elseif FieldArray(5) = "3" and FieldArray(2) = "CURRENT_USER_ID_FK" then ' dropdown
                    Response.Write  "<td>" & ShowSelectBox2(trim(FieldArray(2)), iif(isnull(UserSettingXML) or trim(UserSettingXML) = "", Ucase(CurrentUserID),Session(trim(FieldArray(2)))), FieldArray(6),255, RepeatString(43, "&nbsp;"), "") & "</td>"
                elseif FieldArray(5) = "4"  then ' location
                    Response.Write "<td colspan=""3"">&nbsp;" & GetBarcodeIcon() & "&nbsp;"
                    authorityFunction =""
                    ShowLocationPicker7 "document.form1", "iLocationID", "lpLocationBarCode", "lpLocationName", 10, 49, false, authorityFunction, Session(trim(FieldArray(2)))
                    Response.Write "&nbsp;</td>"
                end if
                Response.Write "</tr>"  
            end if
        end if
    Next%>

	<tr>    
        <td>&nbsp;</td>  
		<td align="right"> <br />
			<a HREF="#" onclick="ResetForm(this.form);"><img SRC="/cheminv/graphics/sq_btn/clearall.gif" border="0" alt="ClearAll"></a>
            <%if lcase(Source) <> "eln" Then %>
                <a HREF="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
            <%End if %>
			<a HREF="#" onclick="ValidateContainer(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
<%if LocationAdmin =1 then%>
 <script language = "javascript">
  function setPrincipalID(element)
    {
        <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
        !document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
        <% end if %>
    }
  function setOwnership()
    {
        <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
        var type="";
        type=<%=PrincipalID%>;
        if(type!="")
        {
             document.getElementById("PrincipalID").value=document.getElementById("OwnershipType").value;
             var tempString ="|" + document.getElementById("OwnerShipUserList").value;
           if (tempString.indexOf("|" + type + ",")>=0)
           {
             getList(document.getElementById("OwnerShipUserList").value,type);
             document.getElementById("User_cb").checked=true;
           }
           else
           {
                getList(document.getElementById("OwnerShipGroupList").value,type);
                document.getElementById("Group_cb").checked=true;
           }
           document.form1.PrincipalID.value=type;
        }
  
        <% end if %>
     }
  setOwnership(); 
  </script>
<%end if %>	
</form>
</center>
</body>
</html>
