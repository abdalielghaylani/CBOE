<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
ContainerID = Session("ContainerID")
ContainerName = Session("ContainerName")
ReservationID = Request("ReservationID")
isDelete = false
If ReservationID = "" then
	Caption = "Add a Reservation to this container."
	formAction = "action=create"
	ReservationUserID = Session("UserNameChemInv")
	ReservationType = "0"
	QtyReserved = Request("QtyAvailable")
Else
	if Lcase(Request("action")) = "delete" then
		Caption = "Are you sure you want to delete this reservation?"
		formAction = "action=delete"
		isDelete = true
	Else
		Caption = "Edit this Reservation"
		formAction = "action=edit"
		ReservationUserID = Request("ReservationUserID")
		ReservationTypeID = Request("ReservationTypeID")
		QtyReserved = Request("QtyReserved")
	End if
End if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create a New Inventory Reservation</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--
	window.focus();
	function ValidateReservation(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Quantity reserved is required
		if (document.form1.QtyReserved){
			if (document.form1.QtyReserved.value.length == 0) {
				errmsg = errmsg + "- Quantity reserved is required.\r";
				bWriteError = true;
			}
			else{
				if (!isPositiveNumber(document.form1.QtyReserved.value)&&(document.form1.QtyReserved.value != 0)){
				errmsg = errmsg + "- Quantity reserved must be a positive number.\r";
				bWriteError = true;
			}
			}
		
			// Quantity reserved if present must be a number
			if (!isNumber(document.form1.QtyReserved.value)){
				errmsg = errmsg + "- Quantity reserved must be a number.\r";
				bWriteError = true;
			}
		}		
		if (bWriteError){
			alert(errmsg);	
		}
		else{
			document.form1.submit();
		}
	}
//-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="Reserve_action.asp?<%=formAction%>" method="POST">
<input TYPE="Hidden" Name="ContainerID" value="<%=ContainerID%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container Name:
		</td>
		<td>
			<input TYPE="tetx" SIZE="30" Maxlength="50" onfocus="blur()" VALUE="<%=ContainerName%>" disabled>
		</td>
	</tr>
	<%if ReservationID > 0 then%>
	<tr>
		<td align="right" nowrap>
			Reservation ID:
		</td>
		<td>
			<input TYPE="tetx" SIZE="30" Maxlength="50" onfocus="blur()" VALUE="<%=ReservationID%>" disabled>
			<input TYPE="Hidden" Name="ReservationID" value="<%=ReservationID%>">
		</td>
	</tr>
	<%End if%>
<%If NOT isDelete then%>
	<tr>
		<td align="right" nowrap>
			User ID:
		</td>
		<td>
			<%=ShowSelectBox2("ReservationUserID", Ucase(ReservationUserID), "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 27, "", "")%>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Quantity (<%=Request("UOMAbv")%>):</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="20" Maxlength="50" NAME="QtyReserved" value="<%=QtyReserved%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Reservation Type:
		</td>
		<td>
			<%=ShowSelectBox2("ReservationTypeID", ReservationTypeID, "SELECT Reservation_Type_ID AS Value, Reservation_Type_Name AS DisplayText FROM inv_Reservation_Types ORDER BY lower(Reservation_Type_Name) ASC", 27, "", "")%>
		</td>
	</tr>
<%End if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateReservation(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</center>
</body>
</html>
