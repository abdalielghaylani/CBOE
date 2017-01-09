<%@ Control Language="C#" AutoEventWireup="true" Codebehind="NavigationPanel.ascx.cs"
    Inherits="PerkinElmer.CBOE.Registration.Client.Forms.RegistrationAdmin.UserControls.NavigationPanel" %>
<ul class="UlTabsHeader">
    <li class="LiFirstTabsHeader"></li>
    <li id="LiPropertyList" class="SelectedTabsHeader" runat="server">
        <div class="LeftCornerTabsHeader">
        </div>
        <asp:LinkButton CssClass="ATabsHeader" ID="LinkButtonPropertyList" runat="server"
            OnClientClick="CleanFields()" OnClick="LinkButton_PropertyList_Click" CausesValidation="false">Registry</asp:LinkButton>
        <div class="RigthCornerTabsHeader">
        </div>
    </li>
    <li id="LiCompound" class="LiTabsHeader" runat="server">
        <div class="LeftCornerTabsHeader">
        </div>
        <asp:LinkButton CssClass="ATabsHeader" ID="LinkButtonCompound" runat="server" OnClientClick="CleanFields()"
            OnClick="LinkButton_Compound_Click" CausesValidation="false">Component</asp:LinkButton>
        <div class="RigthCornerTabsHeader">
        </div>
    </li>
    
    <li id="LiStructure" class="LiTabsHeader" runat="server">
        <div class="LeftCornerTabsHeader">
        </div>
        <asp:LinkButton CssClass="ATabsHeader" ID="LinkButtonStructure" runat="server" OnClientClick="CleanFields()"
            OnClick="LinkButton_Structure_Click" CausesValidation="false">Base Fragment</asp:LinkButton>
        <div class="RigthCornerTabsHeader">
        </div>
    </li>
    
    <li id="LiBatch" class="LiTabsHeader" runat="server">
        <div class="LeftCornerTabsHeader">
        </div>
        <asp:LinkButton CssClass="ATabsHeader" ID="LinkButtonBatch" runat="server" OnClientClick="CleanFields()"
            OnClick="LinkButton_Batch_Click" CausesValidation="false">Batch</asp:LinkButton>
        <div class="RigthCornerTabsHeader">
        </div>
    </li>
    <li id="LiBatchComponent" class="LiTabsHeader" runat="server">
        <div class="LeftCornerTabsHeader">
        </div>
        <asp:LinkButton CssClass="ATabsHeader" ID="LinkButtonBatchComponent" OnClientClick="CleanFields()"
            runat="server" OnClick="LinkButton_BatchComponent_Click1" CausesValidation="false">Batch Component</asp:LinkButton>
        <div class="RigthCornerTabsHeader">
        </div>
    </li>
    <li class="LiLastTabsHeader"></li>
</ul>
