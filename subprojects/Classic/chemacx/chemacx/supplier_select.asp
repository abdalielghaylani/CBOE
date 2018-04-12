<SCRIPT LANGUAGE="JavaScript" SRC="wddx.js"></SCRIPT>
<SCRIPT LANGUAGE="JavaScript" SRC="wddxRsEx.js"></SCRIPT>
<!--#INCLUDE FILE="AdoToWddx.js"-->
<% 
'get connection string from application variable
connection_array = Application( "base_connection" & "chemacx")
ConnStr = connection_array(0) & "="  & connection_array(1)
'response.write ConnStr
Session("suppliersTosearch") = Replace(request.cookies("acxprefsuplist"), ":",",")

' variables from configuration ini file
tabWhereField = Application("tabWhereField")
tab1WhereValue = Application("tab1WhereValue")
tab3WhereValue = Application("tab3WhereValue")	




' Overwrite variables from configuration ini file
'tabWhereField = "SupplierType"
'tab1WhereValue = "1"
'tab3WhereValue = "2,3"	
	
SQLQuery = "SELECT Name, SupplierID FROM Supplier WHERE [Supplier].[" & tabWhereField & "] IN (" & tab1WhereValue & "," & tab3WhereValue &  ") ORDER BY Name"


'response.write "<BR>DBVERSION= " & Application("acxDbVersion") & "<BR>"

if Application("acxDbVersion")= "5.5" then
SQLQuery = "SELECT Name, SupplierID FROM Supplier ORDER BY Name"
end if

'response.write SQLQUERY
set SupSWRS = SqlToWddxRS(SQLQuery, ConnStr)
		
 ' use Js converter to transfer Prod RS to client side
Set MyConv = Server.CreateObject("WDDX.JSConverter.1")  
MyDynamicJSCode = MyConv.ConvertData(SupSWRS, "SupWRS")
response.write "<SCRIPT LANGUAGE=""JavaScript"">" & MyDynamicJSCode & "</SCRIPT>"
Set SupSWRS = nothing
set MyConv = nothing

%>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
	<title>Vendor Selector</title>
<script language="JavaScript1.1">
focus();
opener.<%=Application("mainwindow")%>.document.cows_input_form.limitSearch[0].checked = true;
opener.<%=Application("mainwindow")%>.writeLimitCookie('0',opener.<%=Application("mainwindow")%>.limitflag);
var list='';

SelectedWRS = new WddxRecordset;
SelectedWRS.addColumn("name");
SelectedWRS.addColumn("supplierid");


function FillSelect(selobj,WRSobj) {      
      selobj.options.length = 0; 
       
	 for (var i = 0; i < WRSobj.getRowCount(); i++) 
	 {  document.form1.NewOpt = new Option;
        document.form1.NewOpt.value = WRSobj.supplierid[i];
		document.form1.NewOpt.text = WRSobj.name[i];
        selobj.options[selobj.options.length] = document.form1.NewOpt;
		     
	 }
}

function MakeSelectionList(toObject,fromObject,toRS,fromRS,type) {

if (fromObject.options[0].value == 0) {
return false;}
var numofdels = 0;
for (var i=0; i< fromObject.length; i++) 
{
	if (fromObject.options[i].selected) 
	{
		toRS.addRows(1);
        toRS.supplierid[toRS.getRowCount()-1]= fromObject[i].value;
		toRS.name[toRS.getRowCount()-1]= fromObject[i].text;
		fromRS.deleteRow(i-numofdels);
		numofdels += 1;
		if (type == 'add')
		{
		list += fromObject.options[i].value + ":";
		}
		else if (type == 'rem')
		{
		var re = '/:*' + fromObject.options[i].value + '/';
		//alert (re); 
		list =	list.replace(eval(re),"");
		}
	}
}
//alert(list);
SupWRS.sort('name','asc');
SelectedWRS.sort('name','asc');


FillSelect(toObject,toRS);
FillSelect(fromObject,fromRS);
var emptymessage
<% if request.querystring("listType") =  0 then
emptymessage = "All ChemACX vendors"
%>
emptymessage = "All ChemACX vendors";
<%else
emptymessage = "No vendors selected"
%>
emptymessage = "No vendors selected";
<%end if%>
if (document.form1.selectedsup.length == 0)
{
document.form1.NewOpt = new Option;
document.form1.NewOpt.value = 0;
document.form1.NewOpt.text = emptymessage;                            
document.form1.selectedsup.options[0]= document.form1.NewOpt;
}
}

function SelectAll(){
for (var x=0; x< document.form1.selectedsup.options.length; x++){
		document.form1.selectedsup.options[x].selected=1;
	}
MakeSelectionList(document.form1.availsup, document.form1.selectedsup,SupWRS,SelectedWRS,'rem')
}

function SelectFromCookie(mode){
// mode 0 selects from cookie  mode 1 selects from prefered list
if (mode == 0) {
var cstart=11;
var cookiename="acxsuplist=";}
else if (mode == 1) {
var cstart=15;
var cookiename="acxprefsuplist=";
SelectAll();}
else {alert("Error in mode parameter or SelectFromCookie function");return false}
var allcookies = document.cookie;
//alert("all ="+ allcookies);
var  pos = allcookies.indexOf(cookiename);
if (pos != -1){
var start = pos+cstart;
var end = allcookies.indexOf(";",start);  
if (end == -1) end=allcookies.length;
var idlist = allcookies.substring(start,end);
//alert("supllierlist cookie= " + idlist);
}
else { 
//alert('suplist cookie not found');
var idlist="";}

idarray= idlist.split(":");
if (!idlist==""){
for (i in idarray){
	id= idarray[i];
	for (index=0; index<document.form1.availsup.options.length; index++)
	{
	if (document.form1.availsup.options[index].value == id)
	{document.form1.availsup.options[index].selected=1;}}
}
MakeSelectionList(document.form1.selectedsup, document.form1.availsup,SelectedWRS,SupWRS,'add')
}
}

function SaveList(){
list = list.substring(0, list.length-1);
if (list.substring(0,1) == ":") {list = list.substring(1,list.length-1);}
nextyear = new Date();
nextyear.setFullYear(nextyear.getFullYear() +1);
//alert(nextyear.toGMTString());
wholecookie= "acxsuplist=" + list + "; expires=" + nextyear.toGMTString() 
//alert(wholecookie);
document.cookie =  wholecookie; 
//list = list.replace(/:/g,",");
//alert(list);
//opener.document.cows_input_form["Product.SupplierID"].value = list;
opener.<%=Application("mainwindow")%>.location.reload();
}

function SavePreferedList(){
if (list.substring(0,1) == ":") {list = list.substring(1,list.length);}
list = list.substring(0, list.length-1);
nextyear = new Date();
nextyear.setFullYear(nextyear.getFullYear() +1);
wholecookie= "acxprefsuplist=" + list + "; expires=" + nextyear.toGMTString()
document.cookie =  wholecookie; 
commalist =  list.replace(/:/g,",");
opener.<%=Application("mainwindow")%>.document.cows_input_form["Product.SupplierID"].value= commalist;
window.location.reload();
if ((list.length != 0) && (opener.<%=Application("mainwindow")%>.limitflag)){
opener.<%=Application("mainwindow")%>.document.cows_input_form.limitSearch[1].checked=true;
opener.<%=Application("mainwindow")%>.writeLimitCookie('1',true);
}
}


</script>
</head>

<body onload="SupWRS.sort('name','asc');FillSelect(document.form1.availsup, SupWRS);SelectFromCookie(<%=request.querystring("listType")%>)">
<form name="form1">
<table border="0" align="center">
<tr>
<td rowspan="2"><h3><font face="Arial">Available ChemACX Vendors:</font></h3>
<select name=availsup size=20 multiple>
<option val=0>All ChemACX Vendors&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

</select></td>
<td valign="bottom"><input type=button value=">>>" onclick="MakeSelectionList(document.form1.selectedsup, document.form1.availsup,SelectedWRS,SupWRS,'add')"></td>
<td rowspan="2"><h3><font face="Arial">Currently Selected:</font></h3>
<select name=selectedsup size=20 multiple>
<option value=0><%=emptymessage %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</select></td>
</tr>
<tr>
<td valign="top"><input type=button value="<<<" onclick="MakeSelectionList(document.form1.availsup, document.form1.selectedsup,SupWRS,SelectedWRS,'rem')"></td>
</tr>
<tr>
<% if request.querystring("listType") =  0 then %>
<td>
<a onmouseover="status='Move all suppliers to the currently selected list';return true" onmouseout="status='';return true" href="JavaScript:void SelectAll();"><img src="/ChemACX/graphics/select_all_btn.gif" width="135" height="30" alt="" border="0"></a>
</td>
<td>
&nbsp;
</td>
<td>
<a onmouseover="status='Limit search to the currently selected suppliers';return true" onmouseout="status='';return true" href="JavaScript:void SaveList();void window.self.close();"><img src="/ChemACX/graphics/search_selected_supp_btn.gif" width="160" height="30" alt="" border="0"></a>
<%else%>
<td>
&nbsp;
</td>
<td>
&nbsp;
</td>
<td>
<a onmouseover="status='Save currently selected suppliers as your prefered list';return true" onmouseout="status='';return true" href="JavaScript:void SavePreferedList();void window.self.close();"><img src="/ChemACX/graphics/save_as_pref_list_btn.gif" width="135" height="30" alt="" border="0"></a>
<%end if%>
</td>
</tr>
</table>

<BR><BR>


</form>







</body>
</html>


