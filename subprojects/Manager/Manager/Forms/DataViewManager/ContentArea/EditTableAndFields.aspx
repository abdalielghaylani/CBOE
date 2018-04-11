<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_EditTableAndFields" Codebehind="EditTableAndFields.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/EditTableAndFields.ascx" TagName="EditTableAndFields" TagPrefix="uc1" %>
<asp:Content Id="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="Server">
<iframe id="ParentDiv" class="BackgroundHidden" frameborder="0" scrolling="no"></iframe>
    <script language="javascript" type="text/javascript">
    function SetProgressLayerVisibility(visible)
    {
        if(visible)
        {
            document.getElementById('UpdateProgressDiv').style.display = 'block';
            document.getElementById('ParentDiv').className = 'BackgroundVisible';
        }
        else
        {
            document.getElementById('UpdateProgressDiv').style.display = 'none';
            document.getElementById('ParentDiv').className ='BackgroundHidden';
        }
    }
    </script>
    <table class="PagesContentTable">
        <tr>
            <td align="center">
                <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <uc1:EditTableAndFields id="EditTableAndFieldsUserControl" runat="server">
                </uc1:EditTableAndFields>
            </td>
            <td>
                <div id="UpdateProgressDiv" style="z-index:340;display:none;">
                    <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif" style="position:absolute;top:72px;left:762px;z-index:340;"/>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

