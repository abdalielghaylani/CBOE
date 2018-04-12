<%@ language="vbscript" %>
<%
	'SYAN added 1/14/2004 to fix CSBR-35466
	flipFrom = Request.QueryString("flipFrom")
	flipTo = Request.QueryString("flipTo")
	'End of SYAN modification
	
	'DJP added to support diff form names
	formName = Request.QueryString("formName")
%>
<script Language="jscript" runat="server">
	Response.Buffer=true; 

	var date_format = new String(Request.QueryString("date_format"));

	if (date_format == '') {
		date_format = 8;
	}
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->


<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 7.0">
</HEAD>
<BODY onload="DatePicker_onload()">
<script Language="JavaScript">
<!--

function DatePicker_onload(){
var strControl = document.FormX.hControl.value;
var strDate = document.FormX.hDate.value;
var objParent = window.opener;
var formName = "<%=formName%>";

if (strDate.length > 0){

	var currVal = objParent.document.forms[formName].elements[strControl].value 
	if (currVal.indexOf("-",currVal.length-1)!= -1){
		strDate = currVal + strDate
	}
	//SYAN added 12/15/2003 to fix CSBR-35466
	strDate = FormatDate(strDate, "<%=flipFrom%>", "<%=flipTo%>"); //this function is in /gui/dateFormat_js.asp
	//End of SYAN modification
	
	objParent.document.forms[formName].elements[strControl].value = strDate;
}
objParent.document.forms[formName].elements[strControl].focus()
close();
}

//-->
</script>

<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp" -->

<script Language="jscript" runat="server">
var strControl = Session("DateCtrl");
var strDate = new String(Request.QueryString("MV"));
</script>
<form  name="FormX">
<input name="hControl" type=hidden value="<% = strControl%>">
<input name="hDate" type=hidden value="<% = strDate%>">
</form>
</BODY>
</HTML>