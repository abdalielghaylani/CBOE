<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest = "false" CodeFile="COEFormManagerTest.aspx.cs" Inherits="COEFormManagerTest" %>

<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormManager"
    TagPrefix="COECntrl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>COEFormManger Server Control Sample Application</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <coecntrl:coeformmanager id="COEFormManager1" runat="server" height="300px" width="400px"></coecntrl:coeformmanager>
    
    </div>
    </form>
</body>
</html>
