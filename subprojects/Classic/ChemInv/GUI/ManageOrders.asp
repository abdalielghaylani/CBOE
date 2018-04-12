<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<html>
<head>
<title>Assign To Order</title>
<script type="text/javascript" language="javascript" src="/cheminv/choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>

<%
Dim Conn
Dim Cmd
Dim RS
Dim Action
Dim ContainerID
Dim SQL
Dim strSelected
Dim bOrders
Dim bOnOrder
Dim Count

Action = Request("action")
ContainerID = Request("ContainerID")
OrderID = Session("OrderID")

GetInvConnection()
bOnOrder = true

' Verify that this container isn't in a new or shipped order
SQL = "SELECT count(oc.container_id_fk) as Count FROM inv_order_containers oc, inv_orders o WHERE container_id_fk = ? AND order_id_fk = order_id AND order_status_id_fk not in (3,4)"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("AddContainerID", 5, 1, 0, ContainerID)
Set RS = server.CreateObject("ADODB.recordset")
RS.Open Cmd

Count = 0
if not (RS.BOF or RS.EOF) then Count = RS("Count")
if cint(Count) = 0 then
    SQL = "SELECT '[' || TIMESTAMP || '] ' || SHIP_TO_NAME AS DisplayText, ORDER_ID AS value FROM " &  Application("CHEMINV_USERNAME") & ".inv_orders where ORDER_STATUS_ID_FK in (1)"

    Set RS = Conn.Execute( SQL )
    if( not RS.EOF ) then
        bOrders = true
    else
        bOrders = false
    end if
    
    bOnOrder = false
end if

%>

<script type="text/javascript" language="JavaScript">
<!--
	window.focus();
	
	function CreateOrder()
	{
	    OpenDialog('/Cheminv/GUI/Order_list.asp?action=create&CreateType=AssignToOrder&clear=1', 'Diag2', 1);
	}
	
	function CreateOrderAndAddContainer()
	{
	    OpenDialog('/Cheminv/GUI/Order_list.asp?action=create&CreateType=AssignToOrder&clear=1&AddContainerID=<% = ContainerID %>', 'Diag2', 1);
	}

	function CreateOrderFinished()
	{
<%
    ' If no orders existed previously, we build a new one and automatically assign this
    ' container to the new order.  The user may also choose to create a new order even if others
    ' already exist.  In the former case, we simply close this window and exit.  In the latter
    ' case, we do not want to close, but rather refresh the current window.
	if( not bOrders and not bOnOrder ) then
%>
	    window.close();
<%
    else
%>
        window.location.reload();
<%
    end if
%>
	}
	
	function SelectOrder( OrderID )
	{
	    if( document.form1.OrderID )
	    {
	        var oOptions = document.form1.OrderID.options;
	        var bFound = false;
	        for( i = 0; i < oOptions.length && !bFound; i++ )
	        {
	            if( oOptions.item(i).value == OrderID )
	            {
	                document.form1.OrderID.selectedIndex = i;
	                bFound = true;
	            }
	        }
	    }
	}

	function Validate(){

		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		if( document.form1.OrderID.value == "NULL" )
		{
		    errmsg = errmsg + "The selected order is not valid.\rPlease choose a valid order or create a new one.";
		    bWriteError = true;
		}
		if (bWriteError)
		{
			alert(errmsg);
		}
		else
		{
			document.form1.submit();
		}
	}
<%
	if( not bOrders and not bOnOrder ) then
%>
    CreateOrderAndAddContainer();
<%
    end if
%>

//-->
</script>
</head>

<body>

<%
if( bOnOrder ) then
%>
<center>
<table border="0">
	<tr>
		<td colspan="3">
			<span class="GuiFeedback"><br><br><center>This container is already associated with an order.</center></span><br><br>
		</td>
	</tr>
	<tr>
	<td colspan="2" align="right">
        <a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0" alt="Close"></a>
    </td>
    </tr>
</table>
</center>
<%
else
%>
<center>
<form name="form1" action="ManageOrders_action.asp" method="post">
<input type="hidden" name="action" value="<%=action%>" />
<input type="hidden" name="ContainerID" value="<%=ContainerID%>" />

<table border="0">
	<tr>
		<td colspan="3">
			<span class="GuiFeedback"><br><br><center>Please choose an existing order to assign this 
<% if( action = "AssignContainer" ) then %>
			container
<% elseif( action = "AssignRequest" ) then %>
            request            
<% end if %>
			<br />
			or click on New to create a new order:</center></span><br><br>
		</td>
	</tr>
	<tr>
	<td align=right valign=top nowrap>
	    Order description:
	</td>
	<td>
	<% = BuildSelectBox(RS, "OrderID", "", 0, "", "", 1, false, "", "No new orders!") %>
	</td>
	<td>
	<a class="MenuLink" HREF="#" onclick="CreateOrder();return false;">New</a>
	</td>
	</tr>
	<tr>
	<td colspan="2" align="right">
	<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
    <a HREF="#" onclick="Validate('Create'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
	</td></tr>
</table>
</form>
</center>

<%
    if( OrderID <> "" ) then
%>
<script language=javascript>
    SelectOrder( <% = OrderID %> );
</script>
<%
    end if      ' if( OrderID <> "" ) then
end if      ' if( bOnOrder ) then
%>
</body>
</html>

