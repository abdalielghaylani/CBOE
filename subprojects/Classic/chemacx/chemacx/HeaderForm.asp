<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
	<title>Untitled</title>
	
<script language="JavaScript">
function SaveHeaderCookie(){
var ohc;
var dd= ":";
var dq = String.fromCharCode(34);
var cc = ",";
var ohl ="ACXSUPID,SUPPLIER,QTY,UNIT,CATNUM,DESCRIPTION,UPRICE,SUBTOT,S1,S2,DATE,DATEREQ,NAME,c1,c2,c3,c4,c5,c6,c7,c8,c9,c10,c11,c12,c13,c14,c15,c16,EXPDATE,d1,d2,d3,d4,e1,e2,e3,e4,e5,e6,p1,p2,p3,p4,p5,p6,PHONEAC,PHONE,FAXAC,FAX,NAME,ADDR1,ADDR2,ADDR3,CITY,STATE,ZIP"  
ohl += String.fromCharCode(10,13);
ohl += ",,,,,,,,"
with (document.SAFAX){
//digitize the cc and cost center fields
var ccnum = cardnum.value;
ccnum += "                ";
ccnum = ccnum.slice(0,16);
cardnumArray = ccnum.split("");
var deptcode= dcode.value;
deptcode += "    ";
deptcode = deptcode.slice(0,4);
deptcodeArray = deptcode.split("");
var expnscode= ecode.value;
expnscode += "      ";
expnscode = expnscode.slice(0,6);
expnscodeArray = expnscode.split("");
var prjcode= pcode.value;
prjcode += "      ";
prjcode = prjcode.slice(0,6);
prjcodeArray = prjcode.split("");

if (service[0].checked)
{ohc = "X::";
ohl +=  dq + "X" + dq + cc + dq + dq +cc;}
else
{ohc = ":X:";
ohl += dq + dq + cc + dq + "X" + dq +cc;}

ohc += expectdate.value + dd;
ohc += reqdate.value + dd;
ohc += Name.value + dd;
ohc += cardnum.value + dd;
ohc += expirdate.value + dd;
ohc += dcode.value + dd;
ohc += ecode.value + dd;
ohc += pcode.value + dd;
ohc += phoneac.value + dd;
ohc += phone.value + dd;
ohc += faxac.value + dd;
ohc += fax.value + dd;
ohc += shipto.value + dd;
ohc += addr1.value + dd;
ohc += addr2.value + dd;
ohc += addr3.value + dd;
ohc += city.value + dd;
ohc += state.value + dd;
ohc += zipcode.value;

ohl += dq +  expectdate.value +  dq + cc;
ohl += dq +  reqdate.value +  dq + cc;
ohl += dq +  Name.value +  dq + cc;
for (var i=0; i<16; i++)
{ohl += dq + cardnumArray[i] + dq + cc;}
ohl += dq +  expirdate.value +  dq + cc;
for (var i=0; i<4; i++)
{ohl += dq + deptcodeArray[i] + dq + cc;}
for (var i=0; i<6; i++)
{ohl += dq + expnscodeArray[i] + dq + cc;}
for (var i=0; i<6; i++)
{ohl += dq + prjcodeArray[i] + dq + cc;}
ohl += dq +  phoneac.value +  dq + cc;
ohl += dq +  phone.value +  dq + cc;
ohl += dq +  faxac.value +  dq + cc;
ohl += dq +  fax.value +  dq + cc;
ohl += dq +  shipto.value +  dq + cc;
ohl += dq +  addr1.value +  dq + cc;
ohl += dq +  addr2.value +  dq + cc;
ohl += dq +  addr3.value +  dq + cc;
ohl += dq +  city.value +  dq + cc;
ohl += dq +  state.value +  dq + cc;
ohl += dq +  zipcode.value + dq +  String.fromCharCode(13,10);
}
//alert(ohl);
tenyear = new Date();
tenyear.setFullYear(tenyear.getFullYear() +10);
wholecookie= "acxorderheader=" + escape(ohc) + "; expires=" + tenyear.toGMTString()+ "; path=/chemacx";  
document.cookie =  wholecookie;
document.SAFAX.headerlist.value= ohl;
document.SAFAX.target = "_new" 
document.SAFAX.submit();
}

function ReadHeaderCookie(){
var allcookies = document.cookie;
//alert(allcookies);
var  pos = allcookies.indexOf("acxorderheader=");
if (pos != -1){
var start = pos+15;
var end = allcookies.indexOf(";",start);  //the next semicolon ends the cookie
if (end == -1) end=allcookies.length;
var rohc = allcookies.substring(start,end);
rohc = unescape(rohc);

if (rohc.length){
var headerarray = rohc.split(":");
with (document.SAFAX){
if (headerarray[0] == "X") {service[0].checked = true;} 
else {service[1].checked = true;}
expectdate.value = headerarray[2];
 reqdate.value = headerarray[3];
 Name.value = headerarray[4];
 cardnum.value = headerarray[5];
 expirdate.value = headerarray[6];
 dcode.value = headerarray[7];
 ecode.value = headerarray[8];
 pcode.value = headerarray[9];
 phoneac.value = headerarray[10];
 phone.value = headerarray[11];
 faxac.value = headerarray[12];
 fax.value = headerarray[13];
 shipto.value = headerarray[14];
 addr1.value = headerarray[15];
 addr2.value = headerarray[16];
 addr3.value = headerarray[17];
 city.value = headerarray[18];
 state.value = headerarray[19];
 zipcode.value = headerarray[20];
}
}
}
else {alert("No personal information saved on your computer.  Please fill the form.");}
}
</script>	
	
	
</head>

<body onload="ReadHeaderCookie()">
<form name="SAFAX" action="SaveHeader.asp" method="POST">
<h2 align="center">ChemACX Requisitioner Information</h2>
<div align="center">
<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
<tr>
    <td><input type="radio" name="service" value="X" checked> <b>Routine</b></td>
    <td><input type="radio" name="service" value="X"> <b>Rush</b></td>
</tr>
</table>
<br>
<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
<tr>
    <td>Order date:</td>
    <td><input type="text" size="10" name="expectdate"></td>
	<td>Date required:</td>
    <td><input type="text" size="10" name="reqdate"></td>
</tr>
</table>
<br>
<b>CARDHOLDER INFORMATION</b>
<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
<tr>
    <td align="right">Cardholder Name:</td>
    <td><input type="text" size="50" name="Name"></td>
</tr>
<tr>
    <td align="right">Corp. Card#:</td>
	<td><input type="text" size="16" name="cardnum" maxlength="16">&nbsp;<font face size="-1">(No spaces or dashes)</font></td>
</tr>
<tr>
    <td align="right">Expiration Date:</td>
    <td><input type="text" size="7" name="expirdate">&nbsp;<font face size="-1">(mm/yyyy)</font></td>
</tr>
<tr>
    <td align="right">Expense Code:</td>
    <td><input type="text" size="4" maxlength="4" name="dcode">-
	<input type="text" size="6" maxlength="6" name="ecode">-
	<input type="text" size="6" maxlength="6" name="pcode">&nbsp;<font face size="-1">(department - expense - project)</font></td>
</tr>
<tr>
    <td align="right">Phone:</td>
    <td><input type="text" size="3" name="phoneac">
	<input type="text" size="7" name="phone"></td>
</tr>
<tr>
    <td align="right">Fax:</td>
    <td><input type="text" size="3" name="faxac">
	<input type="text" size="7" name="fax"></td>
</tr>
</table>
<br>
<b>SHIPPING ADDRESS</b>
<table cellspacing="0" cellpadding="2" border="2" bordercolor="#4A5AA9">
<tr>
    <td align="right">Name:</td>
    <td><input type="text" size="30" name="shipto"></td>
</tr>
<tr>
    <td align="right">Address 1:</td>
    <td><input type="text" size="30" name="addr1"></td>
</tr>
<tr>
    <td align="right">Room#:</td>
    <td><input type="text" size="30" name="addr2"></td>
</tr>
<tr>
    <td align="right">&nbsp;</td>
    <td><input type="text" size="30" name="addr3"></td>
</tr>
<tr>
    <td align="right">City:</td>
    <td><input type="text" size="30" name="city"></td>
</tr>
<tr>
    <td align="right">State:</td>
    <td><input type="text" size="2" name="state"></td>
</tr>
<tr>
    <td align="right">Zip Code:</td>
    <td><input type="text" size="10" name="zipcode"></td>
</tr>
</table>
<br>
<input type="hidden" name="headerlist">
<input type="hidden" name="itemslist" value="<%=request.form("CSVcart") %>">
<!--- <input type="hidden" name="isSigma" value="<%=request.form("isSigma") %>"> --->
<a href="Close" onclick="parent.opener.focus(); parent.close(); return false"><img src="<%=Application("NavButtonGifPath")%>close_btn.gif" alt="Close the shopping cart window" border="0"></a>
<a href="Next" onclick="SaveHeaderCookie();return false"><img src="<%=Application("NavButtonGifPath")%>export_word_btn.gif" alt="Export to Word Template" border="0"></a> </div>
</form> 


</body>
</html>

