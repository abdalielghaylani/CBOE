<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PreferencesPane.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls.PreferencesPane" %>
<div class="LeftPanelUC">
    <script language="javascript" type="text/javascript">
        function ValidateHitsPerPage(hitsTxtBoxId)
        {
            var validTxt = IsValidPositiveInteger(document.getElementById(hitsTxtBoxId).value);
            var errorMesg='Invalid hits per page.'
            
            if (validTxt && document.getElementById(hitsTxtBoxId).value > 100)
             {
                validTxt=false;
                errorMesg='Hits per page must be between 1 and 100.';
             }
            SetHitsErrorVisibility(!validTxt,errorMesg);
            return validTxt;
        }
        function SetHitsErrorVisibility(visible,errorMesg)
        {
            document.getElementById('InvalidHitsPerPageDiv').style.display = visible == true ? 'block' : 'none';
            document.getElementById('InvalidHitsPerPageDiv').innerHTML=errorMesg;
        }
    </script>
    <div class="QueryManagementWrapper">
        <div class="QueryManagementUC" style="border: solid thin Silver; margin-top:4px;">
            <div>
                <div class="Button2" style="float:left;">
                    <div class="Left">
                    </div>
                    <asp:LinkButton ID="SaveLinkButton" runat="server" CssClass="LinkButton" Text="Save" CausesValidation="false" OnClick="YesButton_Click" />
                    <div class="Right">
                    </div>
                </div>
                <div class="Button2">
                    <div class="Left">
                    </div>
                    <asp:LinkButton ID="DefaultsLinkButton" runat="server" CssClass="LinkButton" Text="Restore Defaults" CausesValidation="false" OnClick="YesButton_Click"  />
                    <div class="Right">
                    </div>
                </div>
            </div>
            <div class="LeftPanelRow">
                <div id="HitsPerPageTD" runat="server" style="float:left;"></div>
                <div style="float:right;"><asp:TextBox ID="HitsPerPageTextBox" runat="server" MaxLength="3" CssClass="HitsPerPageTextBox"/></div>
            </div>
            <div class="LeftPanelRow">
                <div id="FilterChildDataTD" runat="server" style="float:left;"></div>
                <div style="float:right;"><asp:CheckBox id="FilterChildDataCheckBox" runat="server" CssClass="PreferencesCheckBox" /></div>
            </div>
            <div class="LeftPanelRow">
                    <div id="HighlightStructuresTD" runat="server" style="float:left;"></div>
                    <div style="float:right;"><asp:CheckBox id="HighlightStructuresCheckBox" runat="server" CssClass="PreferencesCheckBox" /></div>
            </div>
            <div class="LeftPanelRow" style="border-bottom: solid 1px Silver;"></div>
            <div class="LeftPanelRow">
                <div id="SkipListTD" runat="server" style="float:left;"></div>
                <div style="float:right;"><asp:CheckBox id="SkipListCheckBox" runat="server" CssClass="PreferencesCheckBox" /></div>
            </div>
            <div class="LeftPanelRow">
                <div style="text-align:left" id="ListsTD" runat="server"></div>
                <div style="text-align:right;"><asp:DropDownList ID="ListsFormDropDown" runat="server" Width="90%" /></div>
            </div>
            <div class="LeftPanelRow">
                <div style="text-align:left" id="DetailsTD" runat="server"></div>
                <div style="text-align:right"><asp:DropDownList ID="DetailsFormDropDown" runat="server" Width="90%" /></div>
            </div>
        </div>
        <div id="InvalidHitsPerPageDiv" style="display:none;color:Red;margin-top:6px;"></div>
        
        <div id="ConfirmationYUIPanel" style="text-align:center;" runat="server">
            <div class="hd" id="header" runat="server"></div>
            <div class="bd" id="body" runat="server">
                <div style="padding:10px;"><asp:Label ID="ConfirmationLabel" runat="server" /></div>
                <div style="display:inline;padding:3px;"><asp:Button ID="YesButton" runat="server" OnClick="YesButton_Click" Width="60px" /></div>
                <div style="display:inline;padding:3px;"><asp:Button ID="NoButton" runat="server" OnClick="NoButton_Click" Width="60px" /></div>
                <div><asp:HiddenField ID="ActionField" Value="" runat="server" /></div>
            </div>
            <div class="ft" id="footer" runat="server">
            </div>
        </div>
    </div>
</div>