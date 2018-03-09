<%@ Control Language="C#" AutoEventWireup="true" Codebehind="QueryManagerPane.ascx.cs" Inherits="CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls.ManagerPane" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuButton.ascx" TagName="MenuButton" TagPrefix="ChemBioViz" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuItem.ascx" TagName="MenuItem" TagPrefix="ChemBioViz" %>

    <div class="LeftPanelUC">
    <script language="javascript" type="text/javascript">
        function ValidatePriorSubmit(ddlId, controlA, controlB)
        {
            var hasHitListSelected = ValidateDDL(ddlId);
            if(hasHitListSelected)
            {
                if(controlB != null)
                {
                    var validName = ValidateTextBox(controlA);
                    var validDescription = ValidateTextBox(controlB);
                    if(!validName)
                        document.getElementById('NoDDLSelectedDiv').innerHTML = 'Invalid Name.';
                    else
                        document.getElementById('NoDDLSelectedDiv').innerHTML = '';
                    if(!validDescription)
                    {
                        if(!validName)
                            document.getElementById('NoDDLSelectedDiv').innerHTML += '<br/>';
                    
                        document.getElementById('NoDDLSelectedDiv').innerHTML += 'Invalid Description.';
                    }
                    
                    var validSelection = (validName && validDescription);
                    
                    SetErrorVisibility(!validSelection);
                    return validSelection;
                }
                else if(controlA != null)
                {
                    var validRestoreType = ValidateRestoreType(controlA);
                    if(!validRestoreType)
                        document.getElementById('NoDDLSelectedDiv').innerHTML = 'No restore type selected.';
                    
                    SetErrorVisibility(!validRestoreType);
                    return validRestoreType;
                }
                else
                {
                    SetErrorVisibility(false);
                    return hasHitListSelected;
                }
            }
            else
            {
                document.getElementById('NoDDLSelectedDiv').innerHTML = 'No hitlist selected.';
                return false;
            }
        }
        
        function ValidateDDL(ddlId)
        {
            var ddl = document.getElementById(ddlId);
            var validSelection = false;
            for(i = 0; i < ddl.options.length; i++)
            {
                 if(ddl.options[i].selected)
                 {
                    validSelection = (ddl.options[i].value != '-1,TEMP');
                    break;
                 }
            }
            SetErrorVisibility(!validSelection);
            return validSelection;
        }
        
        function ValidateRestoreType(radioButton)
        {
            for(i = 0; i < radioButton.length; i++)
            {
                if(radioButton[i].checked)
                    return true;
            }
            return false;
        }
        
        function ValidateTextBox(control)
        {
            return (control.value.length > 0);
        }
        
        function SetErrorVisibility(visible)
        {
            if(visible)
            {
                document.getElementById('NoDDLSelectedDiv').style.display = 'block';
            }
            else
            {
                document.getElementById('NoDDLSelectedDiv').style.display = 'none';
            }
        }
        
        function ShowEditTab()
        {
            document.getElementById('EditContents').style.display = 'block';
            document.getElementById('EditTab').className = 'QueryManagementTabSelected';
            document.getElementById('RestoreContents').style.display = 'none';
            document.getElementById('RestoreTab').className = 'QueryManagementTab';
            document.getElementById('DeleteContents').style.display = 'none';
            document.getElementById('DeleteTab').className = 'QueryManagementTab';
            document.getElementById('<%# SelectedTabHiddenField.ClientID %>').value = 'EditContents';
            return false;
        }
        function ShowRestoreTab()
        {
            document.getElementById('EditContents').style.display = 'none';
            document.getElementById('EditTab').className = 'QueryManagementTab';
            document.getElementById('RestoreContents').style.display = 'block';
            document.getElementById('RestoreTab').className = 'QueryManagementTabSelected';
            document.getElementById('DeleteContents').style.display = 'none';
            document.getElementById('DeleteTab').className = 'QueryManagementTab';
            document.getElementById('<%# SelectedTabHiddenField.ClientID %>').value = 'RestoreContents';
            return false;
        }
        function ShowDeleteTab()
        {
            document.getElementById('EditContents').style.display = 'none';
            document.getElementById('EditTab').className = 'QueryManagementTab';
            document.getElementById('RestoreContents').style.display = 'none';
            document.getElementById('RestoreTab').className = 'QueryManagementTab';
            document.getElementById('DeleteContents').style.display = 'block';
            document.getElementById('DeleteTab').className = 'QueryManagementTabSelected';
            document.getElementById('<%# SelectedTabHiddenField.ClientID %>').value = 'DeleteContents';
            return false;
        }
        
        function ShowSelectedTab(tabId)
        {
            if(tabId == 'EditContents')
                ShowEditTab();
            else if(tabId == 'RestoreContents')
                ShowRestoreTab();
            else if(tabId == 'DeleteContents')
                ShowDeleteTab();
        }
        
        function ShowGroup(groupId)
        {
            document.getElementById('RecentContainer').style.display = (groupId == 'RecentContainer') ? 'block': 'none';
            document.getElementById('RecentTitle').className = (groupId == 'RecentContainer') ? 'QueryManagementTitleSelected': 'QueryManagementTitle';
            document.getElementById('SavedContainer').style.display = (groupId == 'SavedContainer') ? 'block': 'none';
            document.getElementById('SavedTitle').className = (groupId == 'SavedContainer') ? 'QueryManagementTitleSelected': 'QueryManagementTitle';
            document.getElementById('AdvancedContainer').style.display = (groupId == 'AdvancedContainer') ? 'block': 'none';
            document.getElementById('AdvancedTitle').className = (groupId == 'AdvancedContainer') ? 'QueryManagementTitleSelected': 'QueryManagementTitle';
            document.getElementById('<%# DefaultExpandedGroup.ClientID %>').value = groupId;
        }
        
        function InitDefaultGroup()
        {
            ShowGroup('<%# DefaultExpandedGroup.Value %>');
            ShowSelectedTab('<%# SelectedTabHiddenField.Value %>');
        }
        YAHOO.util.Event.addListener(window, 'load', InitDefaultGroup);
    </script>
    <div class="QueryManagementWrapper">
        <asp:HiddenField ID="DefaultExpandedGroup" Value="RecentContainer" runat="server" EnableViewState="true" />
        <div class="QueryManagementUC">
            <div id="RecentTitle" class="QueryManagementTitle" onclick="ShowGroup('RecentContainer');"><asp:Literal ID="RecentQueriesLiteral" runat="server" Text="<%$ Resources:Resource,RecentQueries_Label_Text%>" /></div>
            <div id="RecentContainer" style="display:none;" class="QueryManagementContainer">
                <asp:Repeater ID="RecentQueriesRepeater" runat="server">
                    <ItemTemplate>
                        <div>
                            <ChemBioViz:MenuButton id="RecentQueryItemMenuButton" runat="server" CausesValidation="false"
                                Text='<%#DataBinder.Eval(Container.DataItem, "Name") + " " + DataBinder.Eval(Container.DataItem, "NumHits") + " " + DataBinder.Eval(Container.DataItem, "DateCreated")%>' 
                                EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/RestoreQuery.png" MaxLength="20" OnCommand="Queries_ItemCommand">
                                <MenuItemList>
                                    <ChemBioViz:MenuItem Text="Restore Hitlist" CommandName="RestoreHitlist" />
                                    <ChemBioViz:MenuItem Text="Perform Query" CommandName="PerformQuery" />
                                    <ChemBioViz:MenuItem Text="Restore Query To Form" CommandName="RestoreQueryToForm" />
                                </MenuItemList>
                            </ChemBioViz:MenuButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div class="QueryManagementUC">
            <div id="SavedTitle" class="QueryManagementTitle" onclick="ShowGroup('SavedContainer');"><asp:Literal ID="SavedQueriesLiteral" runat="server" Text="<%$ Resources:Resource,SavedQueries_Label_Text%>" /></div>
            <div id="SavedContainer" style="display:none;" class="QueryManagementContainer">
                <asp:Repeater ID="SavedQueriesRepeater" runat="server">
                    <ItemTemplate>
                        <div>
                            <ChemBioViz:MenuButton id="SavedQueryItemMenuButton" runat="server" CausesValidation="false"
                                Text='<%#DataBinder.Eval(Container.DataItem, "Name") + " " + DataBinder.Eval(Container.DataItem, "NumHits") + " " + DataBinder.Eval(Container.DataItem, "DateCreated")%>' 
                                EnableMenu="true" LeftImageURL="~/App_Themes/Common/Images/RestoreQuery.png" MaxLength="20" OnCommand="Queries_ItemCommand">
                                <MenuItemList>
                                    <ChemBioViz:MenuItem Text="Restore Hitlist" CommandName="RestoreHitlist" />
                                    <ChemBioViz:MenuItem Text="Perform Query" CommandName="PerformQuery" />
                                    <ChemBioViz:MenuItem Text="Restore Query To Form" CommandName="RestoreQueryToForm" />
                                </MenuItemList>
                            </ChemBioViz:MenuButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div class="QueryManagementUC">
            <div id="AdvancedTitle" class="QueryManagementTitle" onclick="ShowGroup('AdvancedContainer');"><asp:Literal ID="ManageHitListsLiteral" runat="server" Text="<%$ Resources:Resource,ManageHitLists_Label_Text%>" /></div>
            <div id="AdvancedContainer" style="display:none;" class="QueryManagementContainer">
                <div style="margin-top:4px;margin-bottom:4px;">
                    <asp:Label ID="SelectHitlistLabel" runat="server" />
                    <asp:DropDownList ID="SelectHitlistDropDownList" runat="server" Width="190px" />
                </div>
                <div id="NoDDLSelectedDiv" style="display:none;color:Red;">No hitlist selected.</div>
                <div>
                    <asp:HiddenField ID="SelectedTabHiddenField" Value="None" runat="server" EnableViewState="true" />
                    <div id="TabsContainer" class="QueryManagementTabContainer">
                        <div id="EditTab" class="QueryManagementTab" onclick="ShowEditTab();"><a>Edit</a></div>
                        <div id="RestoreTab" class="QueryManagementTab" onclick="ShowRestoreTab();"><a>Restore</a></div>
                        <div id="DeleteTab" class="QueryManagementTab" onclick="ShowDeleteTab();"><a>Delete</a></div>
                    </div>
                    <div id="EditContents" class="QueryManagementContents" style="display:none;">
                        <div style="clear:both;">
                            <asp:Label ID="FillQueryPropertiesLabel" runat="server" CssClass="MessageBoldLabel" />
                        </div>
                        <table style="margin-top:15px;">
                            <tr>
                                <td><asp:Label ID="NameLabel" runat="server" /></td>
                                <td><asp:TextBox ID="NameTextBox" runat="server" Width="100px" /></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="DescriptionLabel" runat="server" /></td>
                                <td><asp:TextBox ID="DescriptionTextBox" runat="server" Width="100px" /></td>
                            </tr>
                            <tr>
                                <td colspan="2"><asp:CheckBox ID="IsPublicCheckBox" runat="server" /></td>
                            </tr>
                        </table>
                        <div class="Button2">
                            <div class="Left">
                            </div>
                            <asp:LinkButton ID="EditLinkButton" runat="server" OnClick="EditButton_Click" CssClass="LinkButton" CausesValidation="false" />
                            <div class="Right">
                            </div>
                        </div>
                        <div class="Button2">
                            <div class="Left">
                            </div>
                            <asp:LinkButton ID="CancelEditLinkButton" runat="server" OnClick="CancelButton_Click" CssClass="LinkButton" CausesValidation="false" />
                            <div class="Right">
                            </div>
                        </div>
                    </div>
                    <div id="RestoreContents" class="QueryManagementContents" style="display:none;">
                        <div style="clear:both;">
                            <asp:Label ID="RestoreTypeLabel" runat="server" CssClass="MessageBoldLabel" />
                        </div>
                        <asp:RadioButtonList ID="RestoreTypeRadioButtonList" runat="server">
                            <asp:ListItem Value="ReplaceHitlist"></asp:ListItem>
                            <asp:ListItem Value="IntersectHitlist"></asp:ListItem>
                            <asp:ListItem Value="SubstractHitlist"></asp:ListItem>
                            <asp:ListItem Value="UnionHitlist"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div class="Button2">
                            <div class="Left">
                            </div>
                            <asp:LinkButton ID="RestoreButton" runat="server" OnClick="RestoreButton_Click" CssClass="LinkButton" CausesValidation="false" />
                            <div class="Right">
                            </div>
                        </div>
                        <div class="Button2">
                            <div class="Left">
                            </div>
                            <asp:LinkButton ID="CancelRestoreLinkButton" runat="server" OnClick="CancelButton_Click" CssClass="LinkButton" CausesValidation="false" />
                            <div class="Right">
                            </div>
                        </div>
                    </div>
                    <div id="DeleteContents" class="QueryManagementContents" style="display:none;">
                        <div style="clear:both;">
                            <asp:Label ID="ConfirmDeleteLabel" runat="server" CssClass="MessageBoldLabel" />
                        </div>
                        <div class="Button2">
                            <div class="Left">
                            </div>
                            <asp:LinkButton ID="DeleteLinkButton" runat="server" OnClick="DeleteButton_Click" CssClass="LinkButton" CausesValidation="false" />
                            <div class="Right">
                            </div>
                        </div>
                        <div class="Button2">
                            <div class="Left">
                            </div>
                            <asp:LinkButton ID="CancelDeleteLinkButton" runat="server" OnClick="CancelButton_Click" CssClass="LinkButton" CausesValidation="false" />
                            <div class="Right">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
