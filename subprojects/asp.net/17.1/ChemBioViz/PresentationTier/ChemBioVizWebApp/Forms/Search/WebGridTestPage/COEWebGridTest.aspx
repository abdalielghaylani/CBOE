<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="COEWebGridTest.aspx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Search.WebGridTestPage.COEWebGridTest" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="COECntrl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        &nbsp;</div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <coecntrl:coewebgrid id="COEWebGrid1" runat="server"></coecntrl:coewebgrid>
    </form>
</body>
</html>
