<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPreferencesPane.ascx.cs"
    Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls.SearchPreferencesPane" %>
<div class="LeftPanelUC">
        <table>
            <tr valign="top">
                <td id="HitAnyChargeTD" runat="server"></td>
                <td><asp:CheckBox ID="HitAnyChargeCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="ReactionQueryTD" runat="server"></td>
                <td><asp:CheckBox ID="ReactionQueryCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="HitAnyChargeOnCarbonTD" runat="server"></td>
                <td><asp:CheckBox ID="HitAnyChargeOnCarbonCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="PermitExtraneousFragmentsInFullTD" runat="server"></td>
                <td><asp:CheckBox ID="PermitExtraneousFragmentsInFullCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="PermitExtraneousFragmentsInReactionFullTD" runat="server"></td>
                <td><asp:CheckBox ID="PermitExtraneousFragmentsInReactionFullCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="FragmentsOverlapTD" runat="server"></td>
                <td><asp:CheckBox ID="FragmentsOverlapCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="TautomericTD" runat="server"></td>
                <td><asp:CheckBox ID="TautomericCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="IgnoreImplicitHydrogensTD" runat="server"></td>
                <td><asp:CheckBox ID="IgnoreImplicitHydrogensCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="SimilarityTD" runat="server"></td>
                <td><asp:TextBox ID="SimilarityTextBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="FullStructureSimilarityTD" runat="server"></td>
                <td><asp:CheckBox ID="FullStructureSimilarityCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="StereochemistryOptionsTD" runat="server" colspan="2"></td>
            </tr>
            <tr>
                <td id="MatchTetrahedralStereoTD" runat="server"></td>
                <td><asp:DropDownList ID="MatchTetrahedralStereoDropDown" runat="server" /></td>
            </tr>
            <tr>
                <td id="MatchDoubleBondStereoTD" runat="server"></td>
                <td><asp:CheckBox ID="MatchDoubleBondStereoCheckBox" runat="server" /></td>
            </tr>
            <tr>
                <td id="ThickBondsRelTD" runat="server"></td>
                <td><asp:CheckBox ID="ThickBondsRelCheckBox" runat="server" /></td>
            </tr>
        </table>
</div>
