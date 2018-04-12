<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
Dim CDD_DEBUG
CDD_DEBUG = false



%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<!--#INCLUDE FILE = "../../source/biosar_header.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../../source/app_js.js"-->
<!--#INCLUDE FILE="../../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->
<% 

Dim NumCriteria
NumCriteria = 20
Dim i
For i = 1 to NumCriteria%>
	<%if i = 1 then%>
		<%if detectIE() = true then %>
			<style>
			<!--
			#TEXTBOX_DIV_CRITERIA_<%=i%>{visibility:visible}
			#FORM_DIV_CRITERIA_<%=i%>{visibility:visible}
			#BUTTON_DIV_CRITERIA_<%=i%>{visibility:hidden}
			-->
			</style>

		<%else%>
			<style>
			<!--
			#TEXTBOX_DIV_CRITERIA_<%=i%>{position:relative;visibility:hidden}
			#FORM_DIV_CRITERIA_<%=i%>{position:relative;visibility:visible}
			#BUTTON_DIV_CRITERIA_<%=i%>{position:relative;visibility:hidden}
			
			-->
			</style>

		<%end if%>
	<%else%>
		<%if detectIE() = true then %>
			<style>
			<!--
			#TEXTBOX_DIV_CRITERIA_<%=i%>{visibility:hidden}
			#FORM_DIV_CRITERIA_<%=i%>{visibility:hidden}
			#BUTTON_DIV_CRITERIA_<%=i%>{visibility:hidden}
			-->
			</style>

		<%else%>
			<style>
			<!--
			#TEXTBOX_DIV_CRITERIA_<%=i%>{position:relative;visibility:hidden}
			#FORM_DIV_CRITERIA_<%=i%>{position:relative;visibility:visible}
			#BUTTON_DIV_CRITERIA_<%=i%>{position:relative;visibility:hidden}
			
			-->
			</style>

		<%end if%>
	
	<%end if%>
<%next%>
<script language = "javascript">



var table_field_array = new Array
var table_string = ""
//name constants for element names for each assay/criteria section
var FORMDIV_ELM = "FORM_DIV_CRITERIA_"
var FORM_ELM = "FORM_CRITERIA_"
var SELECTBOX1_ELM = "SELECT1_CRITERIA_"
var SELECTBOX2_ELM = "SELECT2_CRITERIA_"
var TEXTBOXDIV_ELM = "TEXTBOX_DIV_CRITERIA_"
var TEXTBOX_ELM = "TEXTBOX_CRITERIA_"
var BUTTONDIV_ELM = "BUTTON_DIV_CRITERIA_"
var MAINWINDOW =<%=Application("mainwindow")%>


//get data from formgroup session variable describing all tables and fields for lists
var myListChild ="<%=Session("Full_ChildTable_And_Fields" & dbkey & formgroup & "QUERY")%>"
//build associative array from list
var split1_array = myListChild.split("|")
for (i=0;i< split1_array.length;i++){
	var split2_array = split1_array[i].split(":::")
	table_field_array[i]  = split2_array[1]
	table_field_array[split2_array[1]] =  split2_array[2]
	if (table_string.length>0){
		table_string = table_string + "," + split2_array[1] + ":" + split2_array[0]
	}
	else{
		table_string =  split2_array[1] + ":" + split2_array[0]
		}
		
}

function buildTablesSelectBox(Name, thestring, bBlankLine, CriteriaNumber){
	
	if (bBlankLine == null){
		bBlankLine = true
	}

		var blankLine = '--------- Select Assay ---------'
		document.write ('<select name="' + Name + '" onChange="fillFieldsList(' + CriteriaNumber + ',this.options[this.selectedIndex].value)" size ="0">')
		if (bBlankLine == true){
			document.write ('<option selected>' + blankLine +'</option>')
		}
		if(!thestring == ""){

			temp = thestring.split(",")
			for (i=0;i<temp.length;i++){
				theItems = temp[i].split(":")
				document.write ("<option value=" + theItems[0] + ">")
				document.write (theItems[1])
				document.write ("</option>")
			}
		}
		document.write ("</select>")

}
function fillFieldsList(CriteriaNumber, theValue){
	var blankLine = '--------- Select Result ---------'
	var field_string = table_field_array[theValue]
	var theList = <%=Application("mainwindow")%>.document.forms[FORM_ELM + CriteriaNumber].elements[SELECTBOX2_ELM+ CriteriaNumber]
	theList.length = 0
	var temp = field_string.split(",")
	theList.options[0] = new Option(blankLine)
	var optionNumber = 1
	for (i=0;i<temp.length;i++){
		
			var temp2 = temp[i].split(":;:")
			var theOption = temp2[0]
			var theValue=temp2[1]
		var checkVal = MainWindow.document.forms["cows_input_form"].elements[temp2[0]].value
		
		if (checkVal.length == ""){	
			theList.options[optionNumber] = new Option(theOption)
			theList.options[optionNumber].value = theOption
			theList.options[optionNumber].text = theValue
			optionNumber = optionNumber + 1
		}
	}
	
	theList.selected = theList.options[0] 
	showInfo(BUTTONDIV_ELM + CriteriaNumber)

}

function fillFieldsList2(CriteriaNumber, theValue){
	var blankLine = '--------- Select Result ---------'
	var field_string = table_field_array[theValue]
	var theList = <%=Application("mainwindow")%>.document.forms[FORM_ELM + CriteriaNumber].elements[SELECTBOX2_ELM+ CriteriaNumber]
	theList.length = 0
	var temp = field_string.split(",")
		theList.options[0] = new Option(blankLine)
	
	for (i=0;i<temp.length;i++){
		
			var temp2 = temp[i].split(":;:")
			var theOption = temp2[0]
			var theValue=temp2[1]
			
			theList.options[i +1] = new Option(theOption)
			theList.options[i +1].value = theOption
			theList.options[i +1].text = theValue
		
	}
	theList.selected = theList.options[0] 
}


function buildFieldsSelectBox(Name, thestring, blankline, CriteriaNumber){
if (blankline == null){
	blankline = true
}
	
	var blankLine = '--------- Select Result ---------'
	document.write ('<select name="' + Name + '"  onChange = "setCriteriaField(' + CriteriaNumber +')" size ="0">')
	if (blankline == true){
	document.write ('<option selected>' + blankLine +'</option>')
	}
	if(!thestring == ""){

	temp = thestring.split(",")
	for (i=0;i<temp.length;i++){
		theItems = temp[i].split(":")
		document.write ("<option value=" + theItems[0] + ">")
		document.write (theItems[1])
		document.write ("</option>")
	}
	}
	document.write ("</select>")

}

function setCriteriaField(CriteriaNumber){
showInfo(TEXTBOXDIV_ELM + CriteriaNumber)

//var theField = <%=Application("mainwindow")%>.document.forms[theFieldName].elements[theFieldName]
//theField.onChange = "updateValue(&quot;" + theValue + "&quot;, this.value)"
}

function updateValue(theValue, CriteriaNumber){
	var theFieldName = document.forms[FORM_ELM + CriteriaNumber].elements[SELECTBOX2_ELM + CriteriaNumber].options[document.forms[FORM_ELM + CriteriaNumber].elements[SELECTBOX2_ELM + CriteriaNumber].selectedIndex].value
	
	if (theFieldName.length > 0){
		MainWindow.document.forms["cows_input_form"].elements[theFieldName].value = theValue
		}
	else{
		alert("please select and assay and result type before entereing criteria")}

	}


function showInfo(theValue) {
	ns4 = (document.layers)? true:false
	ie4 = (document.all)? true:false
	
	if (ns4){
		
		document.layers[theValue].visibility = "show"
		
	}else{
		if (ie4) {
			
			document.all[theValue].style.visibility = "visible"
		
		}
		else{
			
			document.layers[theValue].visibility = "show"
		}
	}
}

function showNextCriteria(CriteriaNumber) {
	var NextCriteria = CriteriaNumber + 1
	ns4 = (document.layers)? true:false
	ie4 = (document.all)? true:false
	
	if (ns4){
		
		document.layers[FORMDIV_ELM + NextCriteria].visibility = "show"
		document.layers[BUTTONDIV_ELM + CriteriaNumber].visibility = "hide"
		
	}else{
		if (ie4) {
			
			document.all[FORMDIV_ELM + NextCriteria].style.visibility = "visible"
			document.all[BUTTONDIV_ELM + CriteriaNumber].style.visibility = "hidden"
		
		}
		else{
			
			document.layers[FORMDIV_ELM + NextCriteria].visibility = "show"
			document.layers[BUTTONDIV_ELM + CriteriaNumber].visibility = "hide"
		}
	}
}

function showCriteria(CriteriaNumber) {
	var lastCriteria = CriteriaNumber-1

	ns4 = (document.layers)? true:false
	ie4 = (document.all)? true:false
	
	if (ns4){
		
		document.layers[FORMDIV_ELM + NextCriteria].visibility = "show"
		document.layers[BUTTONDIV_ELM + lastCriteria].visibility = "hide"
		
	}else{
		if (ie4) {
			document.all[FORMDIV_ELM + CriteriaNumber].style.visibility = "visible"
			document.all[BUTTONDIV_ELM + lastCriteria].style.visibility = "hidden"
		
		}
		else{
			document.layers[FORMDIV_ELM + CriteriaNumber].visibility = "show"
			document.layers[BUTTONDIV_ELM + lastCriteria].visibility = "hide"
		}
	}
}


function hideInfo(theValue) {
	ns4 = (document.layers)? true:false
	ie4 = (document.all)? true:false
	if (ns4){
		document.layers[theValue].visibility = "hide"
		
		}
	if (ie4) {
		
		document.all[theValue].style.visibility = "hidden"
		
		}
}

function checkRestoreQuery(){
var CriteriaNumber = 1
var special = "<%=request.querystring("special")%>"

	if (special.toUpperCase() == "EDIT_QUERY"){
	
		var table_string_array1 = table_string.split(",")
		var r
		
		for(r=0;r<table_string_array1.length;r++){
			var m
			var table_array2 = table_string_array1[r].split(":")
			var table_name=table_array2[0]
			var fieldname_string = table_field_array[table_name]
			var fieldname_string_array = fieldname_string.split(",")
			for(m=0;m<fieldname_string_array.length;m++){
				var fieldname_temp= fieldname_string_array[m]
				var fieldname_temp_array = fieldname_temp.split(":;:")
				var fieldname = fieldname_temp_array[0]
				var theValue = MainWindow.document.forms["cows_input_form"].elements[fieldname].value 
				
				if (theValue != ""){
					if (CriteriaNumber==1){
						showInfo(BUTTONDIV_ELM + CriteriaNumber)
						setSelect1_Selected(CriteriaNumber,table_name)
						setSelect2_Selected(CriteriaNumber,fieldname)
						setTextBoxValue(CriteriaNumber, theValue)
					}
					if  (CriteriaNumber>1){
						
						showCriteria(CriteriaNumber)
						setSelect1_Selected(CriteriaNumber,table_name)
						setSelect2_Selected(CriteriaNumber,fieldname)
						showInfo(TEXTBOXDIV_ELM + CriteriaNumber)
						setTextBoxValue(CriteriaNumber, theValue)
						showInfo(BUTTONDIV_ELM + CriteriaNumber)
						
					
					}
					CriteriaNumber = CriteriaNumber+1
				}
			}

		}

	}

}

function setSelect1_Selected(CriteriaNumber,theValue){
	var optionNumber = ""
	var myList = MainWindow.document.forms[FORM_ELM + CriteriaNumber].elements[SELECTBOX1_ELM + CriteriaNumber]
	for (i =0;i<myList.length;i++){
		if (theValue == myList.options[i].value){
			var optionNumber = i
		}
	}
	myList.selectedIndex = optionNumber
	fillFieldsList2(CriteriaNumber, theValue)
}

function setSelect2_Selected(CriteriaNumber,theValue){
	
	var optionNumber = ""
	var myList = MainWindow.document.forms[FORM_ELM + CriteriaNumber].elements[SELECTBOX2_ELM + CriteriaNumber]
	for (i =0;i<myList.length;i++){
		if (theValue == myList.options[i].value){
			var optionNumber = i
		}
	}
	
	myList.selectedIndex = optionNumber
	
}
function setTextBoxValue(CriteriaNumber, theValue){
	var theFieldName = document.forms[FORM_ELM + CriteriaNumber].elements[TEXTBOX_ELM + CriteriaNumber].value = theValue

}
</script>
 


<title>#DB_NAME Search/Refine Form</title>
</head>

<body <%=default_body%> >
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table border = "0" width="600">
<tr><td <%=td_header_bgcolor %>><font <%=font_header_caption%>><b>Compound Criteria</b></font></td></tr></table>

<table <%=table_main_L1%>>
<tr><tr>
<%
'use SHOW_ONLY_BASE to indicate that getItems should display only the base table fields and format all others as hidden within the cows_input_form
'This parameter also hides the base table name header so you can add your own
'code after the _footer generates the input boxes that update the hidden fields for purposes of searching via cows.
		GetItems formgroup, dbkey,  "QUERY", "SHOW_ONLY_BASE"%>
		<%Response.Write Session("LEFT" & "QUERY" & dbkey & formgroup)%>
</td></tr></table>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
<% 'assay form information must be after cows_input_form. The above _footer_vbs closes that form%>


<%For i = 1 to NumCriteria
if i = 1 then%>
<table border = "0" width="600">
<tr><td <%=td_header_bgcolor %>><font <%=font_header_caption%>><b>Assay Criteria</b></font></td></tr></table>
<%end if%>
<!--open form criteria div--><div id ="FORM_DIV_CRITERIA_<%=i%>"><form name="FORM_CRITERIA_<%=i%>">
<!--start div/form table-->
<table border = "0" width="600">
<tr>

<!--start select box 1 cell/table--><td width="200" align="left">
<table width="200"><tr><td><script language = "javascript">
buildTablesSelectBox("SELECT1_CRITERIA_<%=i%>",table_string,true,<%=i%>)
</script></td></tr></table></td><!--end select box 1 cell/table-->


<!--start select box 2 cell/table--><td width="200" align="left">
<table><tr><td>
<script language = "javascript">
buildFieldsSelectBox("SELECT2_CRITERIA_<%=i%>","",true,<%=i%>)
</script>
</td></tr></table></td>
<!--end select box 2 cell/table-->

<!--start text box cell/table-->
<td width="100" align="top">
<!--open text box  div--><div id="TEXTBOX_DIV_CRITERIA_<%=i%>">
<table><tr><td><input type = "text" name = "TEXTBOX_CRITERIA_<%=i%>" value = "" onBlur="updateValue(this.value,<%=i%>)">
</td></tr></table>
</div><!--close text box  div-->
</td>
<!--end text box cell/table-->

<!--start button cell/table-->
<td width="100" align="top">

<!--open button  div-->
<div id="BUTTON_DIV_CRITERIA_<%=i%>">
<table><tr><td><input type="button" name="Add Criteria" value="Add Criteria" onclick="showNextCriteria(<%=i%>)">
</td></tr></table>
</div>
<!--close button  div-->
</td>
<!--end button cell/table-->

</tr>
</table><!--start div/form table-->
</form>
</div><!--close form criteria div-->

<%next%>

<script language = "javascript">
checkRestoreQuery()
</script>
</body>
</html>
