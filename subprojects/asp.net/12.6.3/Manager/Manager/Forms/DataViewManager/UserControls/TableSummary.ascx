<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TableSummary.ascx.cs" Inherits="TableSummary" %>
<style type="text/css">
.TableTextBox{font-size: 10px;font-family: Verdana;color: #000066;background-color:#F1F1F1;height: 15px;width: 200px;border-top: #C0C0C0 1px solid;border-left: #C0C0C0 1px solid;border-right: #C0C0C0 1px solid;border-bottom: #C0C0C0 1px solid;}
.ImageButtonHolder
{
    margin-right:6px;
    float:left;
}
</style>
<asp:HiddenField ID="SelectedTableIDHidden" Value="-1" runat="server" />
<table id="ShowDetailsTable" runat="server">
    <tr valign="top">
        <td colspan="2">
            <asp:Label runat="server" ID="TableDetailTitle" SkinID="Title"></asp:Label>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="NameTitleLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <asp:TextBox ID="NameTextBox" runat="server" ReadOnly="true" CssClass="TableTextBox" />
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="AliasTitleLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <asp:TextBox ID="AliasTextBox" runat="server" MaxLength="100" ReadOnly="true" CssClass="TableTextBox" />
        </td>
    </tr>
    <tr style="height:40px;">
        <td></td>
        <td align="center" valign="bottom">
            <div class="ImageButtonHolder">
                <COEManager:ImageButton id="EditTableImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="UpdateTable" OnClientClick="SetProgressLayerVisibility(true); return true;" />
            </div>
            <div class="ImageButtonHolder">
                <COEManager:ImageButton id="DuplicateTableImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="CreateAlias" OnClientClick="ShowDuplicateTable(true); return false;" />
            </div>
            <div class="ImageButtonHolder">
                <COEManager:ImageButton id="EditTagsImageButton" runat="server" ButtonMode="ImgAndTxt" TypeOfButton="UpdateTable" OnClientClick="SetProgressLayerVisibility(true); return true;" />
            </div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="PrimaryKeysLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('TableSummaryPKTbl');" id="PkContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="TableSummaryPKTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="RelationshipsLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('TableSummaryRelTbl');" id="RelationshipsContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="TableSummaryRelTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="LookupsLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('TableSummaryLookupTbl');" id="LookupsContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="TableSummaryLookupTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="TagsLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('TableSummaryTagsTbl');" id="Div1" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="TableSummaryTagsTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="IndexLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('TableSummaryIndexesTbl');" id="IndexesContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="TableSummaryIndexesTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
</table>