<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToolBar.ascx.cs" Inherits="ToolBar" %>
<script type="text/javascript" language="javascript">
    function DisableBaseTableLink(count) {
        if (count <= '2') {
            var obj = '<%= BaseTableImageButton.ClientID %>';
            document.getElementById(obj + '_ActionButton').disabled = true;
            document.getElementById(obj + '_ActionImageButton').disabled = true;
        }
    }
</script>
<div style="text-align:center;float:left;" id="DivForMasterDV" runat="server">
    <asp:HiddenField ID="CurrentSchemaHidden" Value="" runat="server" />
    <COEManager:ImageButton id="AddSchemaImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="AddSchema" CausesValidation="false" OnClientClick="ShowModalFrame('AddSchema.aspx', 'Add Schema'); return false;" />
    <COEManager:ImageButton id="RemoveSchemaImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="Remove" OnClientClick="if(confirm('Are you sure you want to delete schema?')) { SetProgressLayerVisibility(true); return true; } else return false;" />
    <COEManager:ImageButton id="RefreshSchemaImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="Publish" CausesValidation="false" OnClientClick="ShowModalFrame('RefreshSchema.aspx', 'Refresh Schema'); return false;"/>
</div>
<div style="text-align:center;float:left;" id="DivForRegularDV" runat="server">
    <COEManager:ImageButton id="NameAndDescImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="CreateAlias" OnClientClick="SetProgressLayerVisibility(true); return true;" />
    <COEManager:ImageButton id="SecurityImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="EditRoleUsers" OnClientClick="SetProgressLayerVisibility(true); return true;" />
    <COEManager:ImageButton id="BaseTableImageButton" runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="UpdateTable" OnClientClick="SetProgressLayerVisibility(true); return true;" />
</div>
<div style="text-align:right;">
    <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" OnClientClick="return ConfirmCancel();" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
    <COEManager:ImageButton ID="DoneImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" OnClientClick="SetProgressLayerVisibility(true); return true;" />
</div>