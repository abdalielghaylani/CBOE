<%@ LANGUAGE=VBScript %>
<%	Response.expires=0
Response.Buffer = true
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
%>
<html>

<head>
<script language= "javascript">
function doFormSubmit(theFormName){
var myPost = document.forms[theFormName].post_test.value
document.forms[theFormName].method ="post"
document.forms[theFormName].action = "<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?" + myPost
document.forms[theFormName].submit()
}

function submitThisRegNumber1(theFormName){
var myPost = document.forms[theFormName].post_test.value
document.forms[theFormName].method ="post"
document.forms[theFormName].action = "<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=search&user_id=t5_84&user_pwd=t5_84&return_fields=structures.cpd_internal_id&return_structure=true&return_format=gif&return_embedded=true&reg_numbers.reg_number=" + myPost
document.forms[theFormName].submit()
}
function submitThisRegNumber2(theFormName){
var myPost = document.forms[theFormName].post_test.value
document.forms[theFormName].method ="post"
document.forms[theFormName].action = "<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=search&user_id=t5_84&user_pwd=t5_84&return_fields=structures.cpd_internal_id&return_structure=true&return_format=gif&return_embedded=false&reg_numbers.reg_number=" + myPost
document.forms[theFormName].submit()
}
function submitThisRegNumber3(theFormName){
var myPost = document.forms[theFormName].post_test.value
document.forms[theFormName].method ="post"
document.forms[theFormName].action = "<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=search&user_id=t5_84&user_pwd=t5_84&return_fields=alt_ids.identifier,alt_ids.identifier_type&reg_numbers.reg_number=" + myPost
document.forms[theFormName].submit()
}



</script>
<title>Generic POST Interface - Test Page</title>
</head>
<body <%=Application("BODY_BACKGROUND")%>>
<H3>POST Request Examples</H3>
<p>
<p>
Post action: <a href="<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp"><%=Application("AppPathHTTP")%>/reg/reg_post_action.asp</a>
<p>
NOTE: When posting from outside of the chem_reg directory, you need to add http://your_server_name to the post action.
<p>
Below are the Name/Value pairs which are relevant: These can be posted as form elements, either
hidden or user input. In the examples they are shown as a text string as
they would appear in the actual http request. They have been posted as a html
encoded query string. This is done for example purposes only. We suggest that you use
hidden fields/user input fields.</p>
<p><H3>reg_method= SEARCH</H3>
<p>user_id = valid user name within oracle system (here we are using "t5_84")</p>
<p>user_pwd = valid user password within oracle system (here were are using "t5_84")</p>
<p>return_structure = TRUE|FALSE : indicates you want to return the structure.  The default is FALSE.</p>
<p>return_fields = comma delimited list of the fields you want to return.  If you are returning a structure, you must enter
structures.cpd_internal_id&nbsp;</p>
<p>return_format = CDX|GIF : indicates the format in which to return the structure - if none is specified CDX is returned</p>
<p>return_embedded = TRUE|FALSE : indicates that the structure should be returned within the html embed or source tags. or to just send the file path. If none is specified you will get the embedded form.&nbsp;</p>
<p>Any data that is part of the criteria for the where clause is entered as a
name in the form TABLENAME.FIELDNAME such as that shown below:</p>
<p>In the examples below you are using the registration number to
uniquely identify the structure you are trying to retrieve. However, you can use
as many fields as you want as part of the search criteria, they all become
&quot;AND&quot;-ed in the constructed SQL where clause. Fields that can be used are in the
<%=Application("appkey")%>/config/reg.ini file in the section
&quot;base_form_group&quot;.&nbsp;&nbsp;</p>
<p>NOTE: The <A HREF="/<%=Application("appkey")%>/Reg/chemreg_postrequest.asp">chemreg_postrequest.asp</A> file contains examples for sending a structure along with the request.
</p>
<P><STRONG>Example 1: Embedded GIF</STRONG></p>
<p>The structure is returned as a GIF by searching on registration number. The return value will consist of a fully constructed &lt;IMG&gt; tag.</P>
<p>Get the structure for registration number you enter </p>
<p>
<form name="Form1" METHOD="POST" ACTION="">
<input TYPE="Button" VALUE="Submit" ONCLICK="submitThisRegNumber1('Form1')">
<input type = "text" name="post_test" value = "">

<p>Enter a registration number and  click the Submit button. The post will look like the following (where the reg_number will reflect what you have entered)</p>
<p><font face="Courier" size="2" color="Navy"><%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=search&user_id=t5_84&user_pwd=t5_84&return_fields=structures.cpd_internal_id&return_structure=true&return_format=gif&return_embedded=true and & reg_number= "the reg number you entered above"</font></p>
</form>

<p><STRONG>Example 2: Non-embedded GIF</STRONG></p>
<p>The path to the GIF file corresponding to the registration number is returned.</p>
<p>
<form name="Form2" METHOD="POST" ACTION="">
<input TYPE="Button" VALUE="Submit" ONCLICK="submitThisRegNumber2('Form2')">
<input type = "text" name="post_test" value = "">
<p>Enter a registration number and  click the Submit button. The post will look like the following (where the reg_number will reflect what you have entered)</p>
<p><font face="Courier" size="2" color="Navy"><%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=search&user_id=t5_84&user_pwd=t5_84&return_fields=structures.cpd_internal_id&return_structure=true&return_format=gif&return_embedded=false and & reg_number= "the reg number you entered above"</font></p>
</form>

<p><STRONG>Example 3: Return identifiers</STRONG></p>
<p>This returns Identifiers from the Alt_IDSs table (Name, CAS Numbers, Synonyms) for a given registration number.</p>
<p>Get identifiers corresponding to the registration numbers less then 31</p>
<p>
<form name="Form3" METHOD="POST" ACTION="">
<input TYPE="Button" VALUE="Submit" ONCLICK="submitThisRegNumber3('Form3')">
<input type = "text" name="post_test" value = "">
<p>This is information posted when you click the Submit button:</p>
<p><font face="Courier" size="2" color="Navy"><%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=search&user_id=t5_84&user_pwd=t5_84&&return_fields=alt_ids.identifier,alt_ids.identifier_type"</font></p>
</form>
<p>
<p><STRONG>Example 4: Enter Query String</STRONG></p>

<form name="Form4" METHOD="POST" ACTION="">
<input TYPE="Button" VALUE="Submit" ONCLICK="doFormSubmit('Form4')" >
<p>Enter a query string (information after the "?" only) and click Submit:</p>
<input type = "text" name ="post_test" value = "" size = "200">
</form>
</body>
