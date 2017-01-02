<%@ Control Language="C#" AutoEventWireup="true" Codebehind="RecPanel.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls.RecPanel" %>
<div id="RecPanelYUIPanel" style="text-align: top; display: none;" runat="server">
    <div class="MenuItemContainer">
        <asp:Panel runat="server" ID="MenuItemContainerPanel" DefaultButton="OKLinkRecButton">
            <div style="width: 275px; margin-bottom: 30px; margin-top: 5px; margin-left: 10px;">
                <div>
                    <div style="float: left;">
                        <asp:Label ID="DisplayRecLabel" runat="server" Text="Please Enter Rec #"></asp:Label>
                    </div>
                    <div style="float: left;">
                        <asp:TextBox ID="EnterRecNoText" runat="server"></asp:TextBox>
                    </div>
                </div>
                <br style="clear: both;" />
                <div style="margin: 0px; padding: 5px 10px 0px 0px; float: right; width:100px;">
                    <div class="Button3" style="float:left;">
                        <div class="Left">
                        </div>
                        <asp:LinkButton ID="OKLinkRecButton" runat="server" CssClass="LinkButton" OnClick="SaveButton_Click"
                            Text="Ok" />
                        <div class="Right">
                        </div>
                    </div>
                    <div class="Button3" style="float:right;">
                        <div class="Left">
                        </div>
                        <asp:LinkButton ID="CancelLinkRecButton" runat="server" CssClass="LinkButton" Text="Cancel"
                            CausesValidation="false" />
                        <div class="Right">
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</div>
