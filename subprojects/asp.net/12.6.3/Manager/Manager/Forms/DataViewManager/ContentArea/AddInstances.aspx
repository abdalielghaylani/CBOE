<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddInstances.aspx.cs" Inherits="Manager.Forms.DataViewManager.ContentArea.AddInstances" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/ManageDbInstance.ascx" TagPrefix="COEManager" TagName="ManageDbInstance" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<head id="Head1" runat="server">
    <title>Add Instance</title>
</head>
<body>
    <form runat="server">
    <table>
        <tr>
            <td align="left">
                <div style="width:100%;">
                    <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ManageDbInstance runat="server" id="DbManager"  />
            </td>
        </tr>
        </table>
    </form>
</body>
