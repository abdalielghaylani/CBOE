<%@ Page Language="C#" AutoEventWireup="true" CodeFile="COEHitlistManagerTest.aspx.cs" Inherits="COEHitlistManagerTest" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEHitlistManager"
    TagPrefix="COECntrl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>COEHitListManager Server Control Sample Application</title>
<script language="javascript" type="text/javascript">
// <!CDATA[



// ]]>
</script>
</head>
<body text="#0e00c0">
    <form id="form1" runat="server">
    <div>
   <!--       Application Name:
      <asp:DropDownList ID="drpOwnersList" runat="server" Width="114px">
        </asp:DropDownList><br /> -->
        Current
        HitList Type:<asp:DropDownList ID="drpHitlistType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpHitlistType_SelectedIndexChanged" Width="137px">
            <asp:ListItem>TEMP</asp:ListItem>
            <asp:ListItem>SAVED</asp:ListItem>
        </asp:DropDownList><br />
        Current
        HitListID:&nbsp;&nbsp;
        <asp:DropDownList ID="drpHitlistID" runat="server" Width="141px">
        </asp:DropDownList><br />
        <br />
        <asp:Button ID="btnOK" runat="server" Text="Show HitlistManager" OnClick="btnOK_Click" />
      
    </form>
</body>
</html>
