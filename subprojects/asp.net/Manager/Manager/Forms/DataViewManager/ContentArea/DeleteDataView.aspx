<%@ Page Language="C#" MasterPageFile="~/Forms/Master/DataViewManager.Master" AutoEventWireup="true" CodeBehind="DeleteDataView.aspx.cs" Inherits="DeleteDataView" Title="Delete a dataview" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.Master" %>
<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <script language="javascript" type="text/javascript">
        function ConfirmDelete() {
            //Bug Fixing- Jira ID: CBOE-405
            var dropDownCtrl = document.getElementById("<%=DataViewListDropDown.ClientID%>");
            var SelVal = dropDownCtrl.options[dropDownCtrl.selectedIndex].value;
            var btn = document.getElementById("ctl00_ctl00_ContentPlaceHolder_ContentPlaceHolder_OkImageButton_ActionButton");
            if (dropDownCtrl != null && SelVal != -1) {
                YAHOO.coemanager.dvManager.confirmationPanel.show();
                return false;
            }
            if (btn != null) {
                btn.click(); return false;
            }
        }

        YAHOO.namespace("coemanager.dvManager");
        function initConfirmationPanel() {
            YAHOO.coemanager.dvManager.confirmationPanel = new YAHOO.widget.Panel("confirmationPanel",
                {
                    close: true,
                    width: 300,
                    visible: false,
                    draggable: false,
                    constraintoviewport: true,
                    modal: true,
                    fixedcenter: true,
                    zIndex: 17000
                }
            );

            YAHOO.coemanager.dvManager.confirmationPanel.render(document.forms[0]);
        }
        YAHOO.util.Event.addListener(window, "load", initConfirmationPanel);
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
                <asp:DropDownList ID="DataViewListDropDown" runat="server" AppendDataBoundItems="true" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt"  ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png"/>
                <COEManager:ImageButton ID="DeleteImageButton" runat="server" TypeOfButton="Delete" ButtonMode="ImgAndTxt" OnClientClick="javascript:return ConfirmDelete();" />
            </td>
        </tr>
    </table>
    <div id="confirmationPanel" style="visibility:hidden;"> 
      <div class="hd" runat="server" id="HeaderDiv"></div>  
      <div class="bd" style="text-align:center;">
        <img src="../../../App_Themes/Blue/Images/Warning.png" alt="Warning" style="float:left;" />
        <p runat="server" id="WarningMessage" class="DeleteDataviewWarning"></p>
        <div style="clear:both;">
            <COEManager:ImageButton ID="AbortImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" OnClientClick="YAHOO.coemanager.dvManager.confirmationPanel.hide();return false;" />
            <COEManager:ImageButton ID="OkImageButton" runat="server" TypeOfButton="Save" ButtonMode="ImgAndTxt" ImageURL="../../../App_Themes/Common/Images/Done.png" HoverImageURL="../../../App_Themes/Common/Images/Done.png" />
        </div>
      </div>
    </div>
</asp:Content>