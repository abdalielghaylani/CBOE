<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSchema.aspx.cs" Inherits="Manager.Forms.DataViewManager.ContentArea.AddSchema" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/AddSchemas.ascx" TagName="AddSchemas" TagPrefix="cm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Add Schema</title>    
    <script language="javascript" type="text/javascript" src="../../Public/JScripts/CommonScripts.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript">
    </script>
    <table>
        <tr>
            <td align="left">
                <div style="width:100%;">
                    <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="AddSchemasDiv" style="margin-top: 80px; margin-left:250px;">
                    <cm:AddSchemas ID="AddSchemasUserControl" runat="server" />
                </div>
            </td>
        </tr>
        </table>
    </form>
</body>
</html>
