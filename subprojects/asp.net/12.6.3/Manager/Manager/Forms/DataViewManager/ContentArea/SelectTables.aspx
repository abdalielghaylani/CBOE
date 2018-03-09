<%@ Page Language="C#" AutoEventWireup="true" Inherits="Forms_DataViewManager_ContentArea_SelectTables" Codebehind="SelectTables.aspx.cs" MasterPageFile="~/Forms/Master/DataViewManager.Master" %>
<%@ MasterType VirtualPath="~/Forms/Master/DataViewManager.master" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/SelectTables.ascx" TagName="SelectTables" TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/TableSummary.ascx" TagName="TableSummary" TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/DuplicateTable.ascx" TagName="DuplicateTable" TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/SchemaSummary.ascx" TagName="SchemaSummary" TagPrefix="cm" %>
<%@ Register Src="~/Forms/DataViewManager/UserControls/ToolBar.ascx" TagName="Toolbar" TagPrefix="cm" %>

<asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server" >
    <style type="text/css">
        .BackgroundVisible
        {
            position: fixed;
            display: block;
            src: 'nothing.txt';
            scrolling: no;
            z-index: 300;
            width: 100%;
            height: 100%;
            top: 0px;
            left: 0px;
            filter: alpha(opacity=40);
            -ms-filter: "alpha(opacity=40)";
            opacity: .4;
        }
        .BackgroundHidden
        {
            display: none;
        }
    </style>
    <script type="text/JavaScript" language="JavaScript">
        function pageLoad()
        {
            var manager = Sys.WebForms.PageRequestManager.getInstance();
           manager.add_endRequest(endRequest);
           manager.add_beginRequest(OnBeginRequest);
        }
        function OnBeginRequest(sender, args)
        {
            var manager = Sys.WebForms.PageRequestManager.getInstance();
            $get('ParentDiv').className = 'BackgroundVisible';   
        }
        function endRequest(sender, args)
        {
           $get('ParentDiv').className ='BackgroundHidden';
        }
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" ScriptMode="Release"/>
    <iframe id="ParentDiv" class="BackgroundHidden" frameborder="0" scrolling="no"></iframe>
    <table class="PagesContentTable">
        <tr>
            <td align="center" colspan="2">
                <asp:UpdatePanel ID="ErrorAreaUpdatePanel" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <COEManager:ErrorArea ID="ErrorAreaUserControl" runat="server" Visible="false" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr valign="top">
            <td align="left">
                <asp:UpdatePanel ID="TableListUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="BaseTable" style="width:400px">
                            <cm:SelectTables id="SelectTablesUserControl" runat="server"></cm:SelectTables>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="TableSummary" EventName="TableDeleted" />
                        <asp:AsyncPostBackTrigger ControlID="TableSummary" EventName="TableDuplicated" />
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeChecked" />
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeCollapsed" />
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeExpanded" />
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="SortApplied" />
                        <asp:AsyncPostBackTrigger ControlID="Toolbar" EventName="SchemaRemoved" />
                        <asp:AsyncPostBackTrigger ControlID="DuplicateTable" EventName="Duplicate" />
                        <asp:AsyncPostBackTrigger ControlID="DuplicateTable" EventName="Cancel" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="right">
                <asp:UpdatePanel ID="ConfirmationAreaUpdatePanel" runat="server" UpdateMode="Always" RenderMode="Inline">
                    <ContentTemplate>
                        <COEManager:ConfirmationArea ID="ConfirmationAreaUserControl" runat="server" Visible="false" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="BaseTable" style="width:558px">
                            <cm:Toolbar ID="Toolbar" runat="server" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeSelected" />
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeChecked" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="ContentUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="BaseTable" style="width:558px;text-align:left;">
                            <cm:TableSummary ID="TableSummary" runat="server" visible="false"/>
                            <cm:SchemaSummary ID="SchemaSummary" runat="server" visible="false"/>
                            <cm:DuplicateTable ID="DuplicateTable" runat="server" Visible="false" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeSelected" />
                        <asp:AsyncPostBackTrigger ControlID="SelectTablesUserControl" EventName="NodeChecked" />
                        <asp:AsyncPostBackTrigger ControlID="Toolbar" EventName="AddSchema" />
                        <asp:AsyncPostBackTrigger ControlID="Toolbar" EventName="SchemaRemoved" />
                        <asp:AsyncPostBackTrigger ControlID="TableSummary" EventName="TableDeleted" />
                    </Triggers>
                </asp:UpdatePanel>
                <div class="BaseTable" style="width:558px">
                    <asp:UpdatePanel ID="SubmitButtonUpdatePanel" runat="server" UpdateMode="always">
                        <ContentTemplate>
                            <COEManager:ImageButton ID="CancelImageButton" runat="server" TypeOfButton="Cancel" ButtonMode="ImgAndTxt" OnClientClick="return ConfirmCancel();" CausesValidation="false" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" />
                            <COEManager:ImageButton ID="DoneImageButton" runat="server" TypeOfButton="Submit" ButtonMode="ImgAndTxt" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
            <td>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false" DisplayAfter="20">
                    <ProgressTemplate>
                        <div id="UpdateProgressDiv" style="z-index:340;">
                            <img id="ProgressImage" alt="Processing" src="../../../App_Themes/Common/Images/searching.gif" style="position:absolute;top:72px;left:762px;z-index:340;"/>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
</asp:Content>
