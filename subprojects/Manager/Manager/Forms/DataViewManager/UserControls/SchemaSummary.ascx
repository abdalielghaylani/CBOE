<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchemaSummary.ascx.cs" Inherits="SchemaSummary" %>

<style type="text/css">
.SchemaTextBox{font-size: 10px;font-family: Verdana;color: #000066;background-color:#F1F1F1;height:15px;width:200px;border-top: #C0C0C0 1px solid;border-left: #C0C0C0 1px solid;border-right: #C0C0C0 1px solid;border-bottom: #C0C0C0 1px solid;}
</style>
<table id="ShowDetailsTable" runat="server"> 
    <tr valign="top">
        <td colspan="2">
            <asp:Label runat="server" ID="SchemaDetailTitle" SkinID="Title"></asp:Label>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="NameTitleLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div id="SchemaNameDiv" style="width:495px;float:left;">
                <asp:TextBox ID="SchemaNameTextBox" runat="server" ReadOnly="true" CssClass="SchemaTextBox"></asp:TextBox>
            </div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="PrimaryKeysLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('SchemaSummaryPKTbl');" id="PkContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="SchemaSummaryPKTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="RelationshipsLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('SchemaSummaryRelTbl');" id="RelationshipsContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="SchemaSummaryRelTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
    <tr valign="top">
        <td>
            <asp:Label runat="server" ID="LookupsLabel" SkinID="Text"></asp:Label>
        </td>
        <td align="left">
            <div onclick="ToggleVisibility('SchemaSummaryLookupTbl');" id="LookupsContainerHeader" runat="server" class="CollapsablePanelExpanded"></div>
            <div id="SchemaSummaryLookupTbl" style="width:495px;float:left;"></div>
        </td>
    </tr>
</table>