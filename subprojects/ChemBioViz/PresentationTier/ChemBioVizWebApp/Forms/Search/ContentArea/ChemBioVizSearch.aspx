<%@ Page Language="C#" AutoEventWireup="True" Inherits="ChemBioVizSearch" Codebehind="ChemBioVizSearch.aspx.cs"
    EnableViewStateMac="true" MasterPageFile="~/Forms/Master/MasterPage.master" Async="true"
    EnableSessionState="True" ValidateRequest="false" EnableEventValidation="false" %>

    <%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
        <%@ Register Assembly="CambridgeSoft.COE.Framework, Version=18.1.0.0, Culture=neutral, PublicKeyToken=1e3754866626dfbf"
    Namespace="CambridgeSoft.COE.Framework.Controls" TagPrefix="cc2" %>
            <%@ Register Src="~/Forms/Search/UserControls/SavePanel.ascx" TagPrefix="ChemBioViz"
    TagName="SavePanel" %>
                <%@ Register Src="~/Forms/Public/UserControls/ImageButton.ascx" TagName="ImageButton"
    TagPrefix="ChemBioViz" %>
                    <%@ Register Src="~/Forms/Public/UserControls/MenuButton.ascx" TagName="MenuButton"
    TagPrefix="ChemBioViz" %>
                        <%@ Register Src="~/Forms/Public/UserControls/RecPanel.ascx" TagName="RecPanel" TagPrefix="RecPan" %>
                            <%@ Register Src="~/Forms/Public/UserControls/MenuItem.ascx" TagName="MenuItem" TagPrefix="ChemBioViz" %>

                                <asp:Content ID="PagesContent" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
                                    <asp:ScriptManager ID="MainScriptManager" AsyncPostBackTimeout="600" runat="server">
                                    </asp:ScriptManager>
                                    <asp:Timer ID="MainPageTimer" runat="server" Interval="1000">
                                    </asp:Timer>
                                    <div id="MenuesContainerDiv" style="margin: 0px; height: 100%;">
                                        <div runat="server" id="ErrorContainerDiv">
                                            <asp:Label ID="errorLabel" CssClass="PageTitleLabel" Style="color: red" runat="server"></asp:Label>
                                            <asp:Label ID="StackTraceLabel" CssClass="PageTitleLabel" Style="color: Red;" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div id="FormBrowserDiv" style="text-align: right">
                                        <asp:Label ID="FormsLabel" runat="server" Text="Available Forms" Visible="false"></asp:Label>
                                        <asp:DropDownList ID="FormsDropDownList" runat="server" OnSelectedIndexChanged="FormsDropDownList_SelectedIndexChanged" AutoPostBack="true" Visible="true">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="HeaderClass">
                                        <div style="float: left;">
                                            <asp:Label ID="TotalSearchableRecordsLabel" runat="server" />
                                        </div>
                                        <div style="float: right;">
                                            <div style="overflow: auto;">
                                                <asp:Label ID="MarkedCountLabel" Visible="false" runat="server" CssClass="ConfirmationMessage" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="ButtonsToolBar">
                                        <div style="float: right; margin-right: 70px;">
                                            <ChemBioViz:MenuButton ID="SearchQueryMenuButton" runat="server" CausesValidation="false" Text="Restore Query" EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/RestoreQuery.png">
                                                <MenuItemList>
                                                    <ChemBioViz:MenuItem Text="Restore Last Hitlist" CommandName="RestoreLastHitlist" />
                                                    <ChemBioViz:MenuItem Text="Perform Last Query" CommandName="PerformLastQuery" />
                                                    <ChemBioViz:MenuItem Text="Restore Last Query To Form" CommandName="RestoreLastQueryToForm" />
                                                </MenuItemList>
                                            </ChemBioViz:MenuButton>
                                        </div>
                                        <div class="MainButtonContainer">
                                            <%-- <asp:Panel ID="panExportpopup" runat="server" BorderColor="white" BorderWidth="1">
            </asp:Panel>--%>

                                                <ChemBioViz:ImageButton ID="SearchImageButton" runat="server" TypeOfButton="Search" ButtonMode="Txt" CausesValidation="true" />
                                                <ChemBioViz:ImageButton ID="ClearImageButton" runat="server" TypeOfButton="Clear" ButtonMode="Txt" CausesValidation="false" OnClientClick="return coe_clearForm();" UseSubmitBehavior="false" />
                                                <ChemBioViz:ImageButton ID="RetrieveAllImageButton" runat="server" TypeOfButton="RetrieveAll" ButtonMode="Txt" CausesValidation="false" />
                                                <ChemBioViz:MenuButton ID="NewQueryMenuButton" runat="server" CausesValidation="false" Text="New Query" EnableMenu="false" LeftImageURL="~/App_Themes/Common/Images/NewQuery.png" />
                                                <ChemBioViz:MenuButton ID="RefineImageMenuButton" runat="server" CausesValidation="false" Text="Refine" EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/Refine.png">
                                                    <MenuItemList>
                                                        <ChemBioViz:MenuItem Text="Edit Current Query" CommandName="EditCurrent" />
                                                        <ChemBioViz:MenuItem Text="Refine Over Current Records" CommandName="RefineOverHitList" />
                                                    </MenuItemList>
                                                </ChemBioViz:MenuButton>
                                                <ChemBioViz:MenuButton ID="QueryMenuButton" runat="server" CausesValidation="false" Text="Query" EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/Query.png">
                                                    <MenuItemList>
                                                        <ChemBioViz:MenuItem Text="Save" CommandName="SaveQuery" Type="SimplePanelDisplayer" />
                                                        <ChemBioViz:MenuItem Text="Advanced" CommandName="AdvancedQuery" />
                                                    </MenuItemList>
                                                </ChemBioViz:MenuButton>
                                                <ChemBioViz:MenuButton ID="MarkedMenuButton" runat="server" CausesValidation="false" Text="Marked" EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/Marked.png">
                                                    <MenuItemList>
                                                        <ChemBioViz:MenuItem Text="Save" CommandName="SaveMarked" Type="SimplePanelDisplayer" />
                                                        <ChemBioViz:MenuItem Text="Show Marked" CommandName="ShowMarked" />
                                                        <ChemBioViz:MenuItem Text="Show Search Results" CommandName="ShowAll" />
                                                    </MenuItemList>
                                                </ChemBioViz:MenuButton>
                                                <ChemBioViz:MenuButton ID="HitsMenuButton" runat="server" CausesValidation="false" Text="Return To Hits" EnableMenu="false" LeftImageURL="~/App_Themes/Common/Images/ReturnToHits.png" />
                                                <ChemBioViz:MenuButton ID="PrintMenuButton" runat="server" CausesValidation="false" Text="Print" EnableMenu="false" LeftImageURL="~/App_Themes/Common/Images/Print.png" OnClientClick="return PrintFormGenerator();" />
                                                <div style="border:1px solid white; width:65px;float:left;margin-top:5px;vertical-align:baseline;" onmouseover="this.style.border='1px solid #CCC'" ; onmouseout="this.style.border='1px solid white'">
                                                    <asp:ImageButton ID="imgExport" runat="server" ImageUrl="../../../App_Themes/Common/Images/Export.png" OnClientClick="javascript:return flipVisibility(this,'imgExport');" />
                                                    <asp:LinkButton ID="lnkbtnExport" runat="server" OnClientClick="javascript:return flipVisibility(this,'lnkbtnExport');" Font-Size="90%" ForeColor="Navy"> Export</asp:LinkButton>

                                                </div>
                                                <asp:DropDownList ID="GridSizeDropDownList" runat="server" OnSelectedIndexChanged="GridSizeDropDownList_SelectedIndexChanged" AutoPostBack="true" Visible="false" CssClass="HitsPerPage">
                                                    <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                                    <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                                    <asp:ListItem Value="25" Text="25"></asp:ListItem>
                                                    <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                                    <asp:ListItem Value="100" Text="100"></asp:ListItem>
                                                </asp:DropDownList><label id="LabelForGridSize" runat="server" class="HitsPerPage" style="padding-left: 1px; margin-left: 0px;"></label>
                                        </div>
                                        <div>
                                            <%--Div containing Export Server control--%>
                                                <asp:HiddenField runat="server" ID="expcontrolvisibility" />
                                                <div id="ExportControlDiv" style="display: none; position: absolute; top: 200px;
                left: 250px; z-index: 999;" runat="server">
                                                    <cc2:ExportControl ID="ExportControl" runat="server" />
                                                </div>
                                        </div>
                                    </div>
                                    <div>
                                        <div class="ActionLinksContainer" id="ActionLinksContainer" runat="server" />
                                    </div>
                                    <!-- DIV containing FormGroup -->
                                    <div class="FormGenContainer" style="position: absolute; width: 660px;">
                                        <asp:UpdatePanel runat="server" ID="MainUpdatePanel">
                                            <ContentTemplate>
                                                <div id="PagerDiv">
                                                    <cc2:PagerControl ID="CoePagerControl" runat="server" OnCurrentPageChanged="CoePagerControl_CurrentPageChanged" SkinID="COEPagerControl" />
                                                    <RecPan:RecPanel ID="RecNoPanel" runat="server" />
                                                </div>
                                                <asp:Label ID="MaxRecordsReachedLabel" Visible="false" runat="server" CssClass="MaxHitsText" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="MainPageTimer" EventName="Tick" />
                                                <asp:PostBackTrigger ControlID="CoePagerControl" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                        <asp:PlaceHolder ID="FormGeneratorHolder" runat="server"></asp:PlaceHolder>
                                        <!-- DIV containing NO Records Found Message Label -->
                                        <div style="overflow: auto;" id="NoRecordsFoundDiv" runat="server" visible="false">
                                            <asp:Label ID="NoRecordsFoundLabel" ForeColor="DarkRed" Visible="false" runat="server" Font-Size="15px" Font-Bold="true" />
                                        </div>
                                        <asp:UpdatePanel runat="server" ID="BottomPagerUpdatePanel">
                                            <ContentTemplate>
                                                <div id="BottomNavDiv" style="clear:both;">
                                                    <RecPan:RecPanel ID="RecNoPanelBottom" runat="server" />
                                                    <cc2:PagerControl ID="CoePagerControlBottom" runat="server" OnCurrentPageChanged="CoePagerControl_CurrentPageChanged" SkinID="COEPagerControl" />
                                                </div>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="MainPageTimer" EventName="Tick" />
                                                <asp:PostBackTrigger ControlID="CoePagerControlBottom" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- DIV containing ultraweb menu (ACTION Menu) -->
                                    <cc1:CslaDataSource ID="ChemBioVizSearchCSLADataSource" runat="server" OnSelectObject="CslaDataSource_SelectObject" OnUpdateObject="CslaDataSource_UpdateObject">
                                    </cc1:CslaDataSource>

                                    <script type="text/javascript" language="javascript">
                                        if (document.getElementById('<%= this.ExportControlDiv.ClientID %>').style.display == 'block') {
                                            HideChemDraws();
                                        } else {
                                            ShowChemDraws();
                                        }

                                        function ShowModalFrame(url, headerText) {
                                            if (typeof(InitModalIframeControl_<%= this.MoldalIFrame.ClientID %>) == 'function')
                                                InitModalIframeControl_<%= this.MoldalIFrame.ClientID %> (url, headerText, true);
                                        }

                                        function CloseModal(refresh) {
                                            if (typeof(CloseCOEModalIframe) == 'function')
                                                CloseCOEModalIframe();
                                        }
                                    </script>

                                    <cc2:COEModalIframe runat="server" BodyURL="" ID="MoldalIFrame" HeaderText="New Container" ModalPanelSettings="modal: true, context: ['MenuesContainerDiv', 'tl', 'bl'] "></cc2:COEModalIframe>
                                </asp:Content>