<%@ Control Language="C#" AutoEventWireup="true" Inherits="EditUserUC" Codebehind="EditUser.ascx.cs" %>
<%@ Register Assembly="Csla" Namespace="Csla.Web" TagPrefix="cc1" %>
<asp:panel id="Panel1" runat="server" height="580px">
    <asp:UpdatePanel ID="UserDetailsViewUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:DetailsView id="DetailsView1" SkinID="SecurityDetailView3" OnDataBound="DetailsView_Load"  runat="server" DataSourceID="COEUserBODataSource" DataKeyNames="UserID" AutoGenerateRows="False" DefaultMode="Edit" GridLines="None" Width="600px">
                <Fields>
                    <asp:BoundField DataField="PersonID" HeaderText="PersonID" ReadOnly="True" SortExpression="PersonID" Visible= "False" ItemStyle-Width="200px"></asp:BoundField>
                    <asp:TemplateField HeaderStyle-ForeColor="Red" HeaderText="User Name" ItemStyle-Width="400px">
                        <ItemTemplate>
                            <asp:TextBox runat="server" ID="UserID" CssClass="UserTextBox" Text='<%# Bind("UserID")%>'></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="UserIDRequiredField" SkinID="ReqField" ControlToValidate="UserID" ErrorMessage="User Name Required"></asp:RequiredFieldValidator>
                            <asp:LinkButton ID="SelectOracleUserLinkButton"   runat="server" OnClick="SelectOracleUserButton_Click" OnClientClick="javascript:showAddOracleUserPanel()" CausesValidation="false" CssClass="OracleUserLink">Select Oracle User</asp:LinkButton>
                            <asp:CustomValidator runat="server" ID="LDAPUserCustomValidator" OnServerValidate="LDAPUser_ServerValidate" ValidationGroup="LDAPUserValidationGroup" />
                            <nobr><asp:Button ID="GetLDAPUser"  SkinID="LDAPButton" runat="server"  UseSubmitBehavior="true" OnClick="LDAPButton_Clicked" CausesValidation="true" ValidationGroup="LDAPUserValidationGroup"></asp:Button></nobr>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField  HeaderText="User Code" HeaderStyle-CssClass="HeaderLabels">
                        <ItemTemplate>
                          <asp:TextBox runat="server" ID="UserCode" CssClass="UserTextBox" Text='<%# Bind("UserCode")%>'  ></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField   HeaderStyle-ForeColor="Red" HeaderText="Password">
                        <ItemTemplate >
                            <asp:TextBox ID="Password" TextMode="Password" CssClass="UserTextBox" Text='<%# Bind("Password")%>' runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="PasswordRequiredField" SkinID="ReqField" ControlToValidate="Password" ErrorMessage="Password Required"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField >
                    <asp:TemplateField  HeaderStyle-ForeColor="Red"  HeaderText="Confirm Password" AccessibleHeaderText="ConfirmPassword" >
                        <ItemTemplate>
                            <asp:TextBox ID="ConfirmPassword" TextMode="Password" Text='<%# Bind("ConfirmPassword")%>' runat="server" CssClass="UserTextBox"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="ConfirmPasswordRequiredField" SkinID="ReqField" ControlToValidate="ConfirmPassword"  ErrorMessage="Password Confirm Required"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField >
                    <asp:TemplateField HeaderText="First Name">
                        <ItemTemplate >
                          <asp:TextBox runat="server" ID="FirstName" CssClass="UserTextBox"  Text='<%# Bind("FirstName")%>'  ></asp:TextBox>
                          
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField  HeaderText="Middle Name">
                        <ItemTemplate >
                          <asp:TextBox runat="server" ID="MiddleName"  Text='<%# Bind("MiddleName")%>'  CssClass="UserTextBox"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-ForeColor="Red" HeaderText="Last Name">
                        <ItemTemplate >
                          <asp:TextBox runat="server" ID="LastName"  CssClass="UserTextBox"  Text='<%# Bind("LastName")%>'  ></asp:TextBox>
                          <asp:RequiredFieldValidator runat="server" ID="LastNameRequiredField" SkinID="ReqField" ControlToValidate="LastName" ErrorMessage="Last Name Required"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" Visible="false"></asp:BoundField>
                    <asp:BoundField DataField="Department" HeaderText="Department" SortExpression="Department" Visible="false"></asp:BoundField>
                    <asp:TemplateField HeaderText="Telephone" SortExpression="Telephone">
                        <EditItemTemplate>
                            <asp:TextBox ID="Telephone" runat="server" Text='<%# Bind("Telephone") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="Telephone" runat="server" Text='<%# Bind("Telephone") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Telephone") %>'></asp:Label>
                        </ItemTemplate>
                        <ControlStyle CssClass="UserTextBox" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Email">
                        <ItemTemplate >
                          <asp:TextBox runat="server" ID="Email"  CssClass="UserTextBox"  Text='<%# Bind("Email")%>'  ></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Address" SortExpression="Address">
                        <EditItemTemplate>
                            <asp:TextBox ID="Address" runat="server" Text='<%# Bind("Address") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="Address" runat="server" Text='<%# Bind("Address") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Address") %>'></asp:Label>
                        </ItemTemplate>
                        <ControlStyle CssClass="UserTextBox" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Supervisor">
                        <EditItemTemplate>
                              <asp:DropDownList ID="SupervisorDropDownList1" runat="server" CssClass="EditUserDropDownList" 
                                DataSourceID="SupervisorDataSource"
                                DataTextField="Value" DataValueField="Key" 
                                SelectedValue='<%# Bind("SupervisorID") %>'>
                              </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                              <asp:DropDownList ID="SupervisorDropDownList2" runat="server" CssClass="EditUserDropDownList" 
                                DataSourceID="SupervisorDataSource"
                                DataTextField="Value" DataValueField="Key" 
                                Enabled="False" SelectedValue='<%# Bind("SupervisorID") %>'>
                              </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Site">
                        <EditItemTemplate>
                              <asp:DropDownList ID="SiteDropDownList1" runat="server" CssClass="EditUserDropDownList" 
                                DataSourceID="SiteDataSource"
                                DataTextField="Value" DataValueField="Key" 
                                SelectedValue='<%# Bind("SiteID") %>'>
                              </asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                              <asp:DropDownList ID="SiteDropDownList2" runat="server" 
                                DataSourceID="SiteDataSource"
                                DataTextField="Value" DataValueField="Key" 
                                Enabled="False" SelectedValue='<%# Bind("SiteID") %>'>
                              </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <ItemTemplate >
                          <asp:Checkbox runat="server" ID="Active" Checked='<%# Bind("Active")%>' ></asp:Checkbox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="HiddenHeader">
                        <ItemTemplate>
                            <asp:UpdatePanel ID="EditUserUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <table class="EditUserTable">
                                        <tr>
                                            <td>
                                                <br /><asp:Label ID="List1Label" Text="Available Roles" runat="server" CssClass="ListLabel"></asp:Label><br />
                                                <asp:ListBox ID="LeftList" Rows="5" DataSource='<%# Bind("AvailableRoles")%>' DataTextField= "RoleName" DataValueField= "RoleID" SelectionMode="Multiple"    runat="server" CssClass="UserListBox"></asp:ListBox>
                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td><asp:Button ID="MoveFromLeftListToRightList" Text=">>" runat="server" BackColor="#000063" ForeColor="#ffffff" Font-Bold="true" Width="50px" OnClientClick="javascript:isSelectingRoles=true;Page_ValidationActive=false;ValidationSummaryOnSubmit = original_ValidationSummaryOnSubmit;validationActive=false;fnResetError();" /></td>
                                                   </tr>
                                                    <tr>
                                                        <td><asp:Button ID="MoveFromRightListToLeftList" Text="<<" runat="server" BackColor="#000063" ForeColor="#ffffff" Font-Bold="true" Width="50px" OnClientClick="javascript:isSelectingRoles=true;Page_ValidationActive=false;ValidationSummaryOnSubmit = original_ValidationSummaryOnSubmit;validationActive=false;fnResetError();" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td>
                                                <br />
                                                <asp:Label ID="List2Label" Text="Current Roles" runat="server" CssClass="ListLabel"></asp:Label>
                                                <br />
                                                <asp:ListBox ID="RightList" Rows="5" DataSource='<%# Bind("Roles")%>'  DataTextField= "RoleName" DataValueField= "RoleID" SelectionMode="Multiple"  runat="server" CssClass="UserListBox"></asp:ListBox>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <div class="ManageUserButtonsHolder">
                                <COEManager:ImageButton ID="SaveButton" OnClientClick="fnSetError();"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Save" ButtonCssClass="ManageUserSave"></COEManager:ImageButton>
                                <COEManager:ImageButton ID="CancelButton"  runat="server"  ButtonMode="ImgAndTxt" TypeOfButton="Cancel" ButtonCssClass="ManageUserCancel" ImageURL="../../../App_Themes/Common/Images/Cancel.png" HoverImageURL="../../../App_Themes/Common/Images/Cancel.png" CausesValidation="false" ></COEManager:ImageButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            <FooterTemplate>
            </FooterTemplate>
            </asp:DetailsView>
            <div id="serverErrorPanel" visible="false">
                <div class="hd">
                </div>
                <div class="bd">
                    <asp:Label ID="ErrorMessage" runat="server"></asp:Label>
                </div>
                <div class="ft">
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="ActivateUserConfimationUP" ChildrenAsTriggers="false" UpdateMode="conditional" runat="server">
        <ContentTemplate>
            <asp:Panel ID="EnableUserPanel" runat="server" Visible="false">
                <div id="UserConfirmationPanel">
                    <div class="bd" style="text-align:center;" >
                        <asp:Label ID="UserAlreadyExistLabel" runat="server" CssClass="ActivateUserLabel" /><br />
                        <asp:Button ID="OKActivationButton" runat="server" CssClass="ActivateUserButton" CausesValidation="false" />
                        <asp:Button ID="CancelActivationButton" runat="server" CssClass="ActivateUserButton" CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:panel>
<div id="container_userlist" >
    <div visible="false" id="addOracleUserPanel">
        <div visible="false" class="OracleUserLink">Add User From Oracle</div>
        <div visible="false" class="bd">
          <asp:UpdatePanel ID="OracleUserAddUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
          <ContentTemplate>
            <table>
                <tr>
                    <td align="center"><asp:ListBox  SkinID="YUIPanelListBox"  AutoPostBack="true" OnSelectedIndexChanged="UsersAddListBox_IndexChange" visible="false" runat="server" Rows="20" Width="150" ID="UsersAddListBox" SelectionMode="Single" DataSourceID="DBMSUserListDataSource" DataTextField="Value" DataValueField = "Key"></asp:ListBox><br /></td>
                </tr>
            </table>
         </ContentTemplate>
         </asp:UpdatePanel>
        </div>
    </div>
</div>
<script language="javascript" type="text/javascript">
YAHOO.namespace("ChemOfficeEnterprise.EditUser");
function initUserConfirmationPanel() {
    var visibleVar = (typeof (document.getElementById('<%= EnableUserPanel.ClientID %>')) != 'undefined' && document.getElementById('ctl00_ctl00_ContentPlaceHolder_ContentPlaceHolder_EditUserControl_EnableUserPanel') != null);
    YAHOO.ChemOfficeEnterprise.EditUser.ConfirmationPanel = new YAHOO.widget.Panel('UserConfirmationPanel', { width: '350px', fixedcenter: true, visible: visibleVar, draggable: false, close: false, modal: true, constraintoviewport: true, zIndex: 17000 });
    YAHOO.ChemOfficeEnterprise.EditUser.ConfirmationPanel.render(document.forms[0]);
}
var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_pageLoaded(initUserConfirmationPanel);
</script>
<script language="javascript" type="text/javascript">
var isSelectingRoles = false;
function VerifyValidation() {
    if (Page_IsValid==false && validationActive==true)
    {
        ValidationSummaryOnSubmitEx();
    }
    if (isSelectingRoles==true)
    {
        EnableValidators();
    }
}
Sys.Application.add_load( VerifyValidation );
</script>
<script type="text/javascript">
    YAHOO.namespace('ChemOfficeEnterprise.Errors');
    var obj = true;
function initErrorsPanel() 
{
	YAHOO.ChemOfficeEnterprise.Errors.panel1 = new YAHOO.widget.Panel('serverErrorPanel', { width:'500px', visible:false, constraintoviewport:true, modal:true, fixedcenter:true } );
	YAHOO.ChemOfficeEnterprise.Errors.panel1.render();
}

function showErrorPanel() {    
    YAHOO.ChemOfficeEnterprise.Errors.panel1.show();
}

function EndRequestHandler(sender, args) {
    if (document.getElementById('<%= ErrorMessage.ClientID %>').innerHTML != "") {
    if(obj==true)
        showErrorPanel();
    }
}
function fnResetError() {
    obj = false;
    return true;
}
function fnSetError() {
    obj = true;
    return true;
}

Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initErrorsPanel); 
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler); 
</script>
<cc1:CslaDataSource id="COEUserBODataSource"  OnInsertObject="COEUserBODataSource_InsertObject" OnSelectObject="COEUserBODataSource_SelectObject"  OnUpdateObject="COEUserBODataSource_UpdateObject" runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.COEUserBO" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="SupervisorDataSource" OnSelectObject="SupervisorDataSource_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.SupervisorList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="SiteDataSource" OnSelectObject="SiteDataSource_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.SiteList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource> 
<cc1:CslaDataSource id="DBMSUserListDataSource" OnSelectObject="DBMSUserList_SelectObject"   runat="server" TypeSupportsSorting="False" TypeSupportsPaging="False" TypeName="CambridgeSoft.COE.Framework.COESecurityService.UserList" TypeAssemblyName="CambridgeSoft.COE.Framework"></cc1:CslaDataSource>
