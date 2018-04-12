<%@ LANGUAGE=VBScript %>
<%	Response.expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"%>
<html>

<head>

<meta NAME="GENERATOR" Content="Microsoft FrontPage 4.0">
<title>Generic POST Test Interface</title>
</head>

<body  <%=Application("BODY_BACKGROUND")%>>



<STRONG>Post Example for retrieving information about oracle database via select statements.

</STRONG>Below is simple example info about the data returned from an sql statement.
<p>Application parameters:</p>
<p>&nbsp;</p>
<p>REG_METHOD = db_info</p>
<p>REG_PARAMETER = AUTHENTICATE or Leave blank</p>

<p>USER_ID= valid user id.&nbsp; </p>
<p>USER_ID= valid user password.&nbsp;</p>
<p>SQL_REQUEST = any valid select statement.&nbsp; UPDATE, DELETE, AND INSERT
statements&nbsp; not supported.&nbsp; This is an INFO only method.</p>
<p>RETURN_PROPS = the properties of each field to return.&nbsp; Enter as a comma delimited
list.&nbsp; Leave empty and defaults to value. possible values
are (name|numericscale|precision|size|type|value)</p>
<p>response is&nbsp; delineated by &quot;:&quot; rows by &quot;::&quot;</p>
<p>&nbsp;</p>
<form name="name4" METHOD="POST" ACTION="<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp" id=form4 name=form4>



<TABLE WIDTH=75% BORDER=1 CELLSPACING=1 CELLPADDING=1>
	<TR>
		<TD>
REG_METHOD&nbsp;</TD>
		<TD><input type = "text" value = "DB_INFO" name = "REG_METHOD" size = "10">
        </TD>
		
	</TR>
	<TR>
		<TD>
REG_PARAMETER&nbsp;</TD>
		<TD><input type = "text" value = "" name = "REG_PARAMETER" size = "10">
        </TD>
		<TD>Use AUTHENTICATE if you want check user_id/userpassord.  Otherwise leave blank&nbsp;</TD>
	</TR>
	<TR>
		<TD>
USER_ID&nbsp;&nbsp;</TD>
		<TD> <input type = "text" value = "T5_85" name = "USER_ID" size = "10">
        </TD>
		<TD>
	</TR>
	<TR>
		<TD>
USER_PWD</TD>
		<TD><input type = "text" value = "T5_85" name = "USER_PWD" size = "10">


        </TD>
		<TD>USE SYSTEM/MANAGER for info related to other system tables</TD>
	</TR>
	<TR>
		<TD>


SQL_REQUEST&nbsp;</TD>
		<TD colspan="2"><input type = "text" value = "select * from tab" name = "sql_request" size = "50">
        </TD>
	</TR>
	<TR>
		<TD>&nbsp;RETURN_PROPS</TD>
		<TD colspan="2"><input type = "text" value = "NAME,SIZE,TYPE,VALUE" name = "return_props" size = "80">

        </TD>
	</TR>
	<TR>
		<TD><input TYPE="Submit" VALUE="Submit" ONCLICK="submit(this.form)" id=Submit3 name=Submit3>

        </TD>
		<TD></TD>
		<TD></TD>
	</TR>
</TABLE>
<br>&nbsp;
<br>

To get information about the fields in each of the tables found from select * from tab, use the following (shown here for ALT_IDS):
<br>select column_name,data_type from user_tab_columns where table_name = 'ALT_IDS' <br>
&nbsp; This is information posted when you click the Submit button:
Post Action and Post name/value pairs information (in concatenated form):"<%=Application("AppPathHTTP")%>/reg/reg_post_action.asp?reg_method=dbinfo&user_id=regdb&user_pwd=oracle&sql_request=select * from tab&return_props=name,numericscale,precision,size,type,value"
returns info from select statement as indicated in return_props. If return_props is empty then the value is returned by default.
</form>



</body>
