<%@ Page Language="C#" AutoEventWireup="true" Codebehind="RecentActivities.aspx.cs"
    Inherits="DocManagerWeb.Forms.ContentArea.RecentActivities" MasterPageFile="~/Forms/Master/DocManager.Master" %>

<%@ Register Src="~/Forms/Public/UserControls/MessagesArea.ascx" TagName="MessagesArea"
    TagPrefix="uc2" %>
<%@ Register Src="~/Forms/Public/UserControls/MessagesBox.ascx" TagName="MessageBox"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics2.WebUI.Misc.v11.1, Version=11.1.20111.1006, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="CambridgeSoft.COE.Framework" Namespace="CambridgeSoft.COE.Framework.Controls.COEFormGenerator"
    TagPrefix="cc2" %>
<%@ Register Assembly="Csla, Version=2.1.1.0, Culture=neutral, PublicKeyToken=93be5fdc093e4c30"
    Namespace="Csla.Web" TagPrefix="cc1" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuButton.ascx" TagName="MenuButton"
    TagPrefix="uc4" %>
<%@ Register Src="~/Forms/Public/UserControls/MenuItem.ascx" TagName="MenuItem" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

<script language="javascript" type="text/javascript">
    function CloseWindow() {
        self.close();
    }

    function numericFilter(txb) {
        txb.value = txb.value.replace(/[^\0-9]/ig, "");
    }
   
    </script>
    <table class="PagesContentTable" cellspacing="1" cellpadding="2">
        <tr class="PagesToolBar">
            <td class="ColumnContentHeader">
            </td>
            <td align="left" style="white-space: nowrap; padding-top: 10px;" class="MainButtonContainer">
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="CloseWindow()" />
            </td>
            <td align="right">
                <asp:Label ID="lblRecentActivities" runat="server" SkinID="PageTitleLabel" Text="Recent Activities" />
            </td>
        </tr>
        <tr id="MessagesAreaRow" runat="server" visible="false">
            <td colspan="3" align="center">
                <uc2:MessagesArea ID="MessagesArea1" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="margin-top: 45px; margin-left: 100px;">              
                <div style="width: 100%; height: auto; padding: 0; margin: 30px;">
                    <div style="width: 330px; min-height: 131px; height: auto; background-color: #CCC;">
                    <div style="width: 50%; height: auto; float: left; margin-Top: 6px; margin-left:6px;">
                        Documents submitted:
                        <br />
                        <br />
                            <div style="margin-Top: 3px;">
                                <asp:RadioButton ID="rbtnSubmitOpt1" runat="server" Text=" Today" GroupName="Days" Checked="true"/></div>
                                <asp:Panel ID="pnlDays" runat="server" DefaultButton="btnShowActivities">
                            <div style="margin-Top: 3px;">
                                <asp:RadioButton ID="rbtnSubmitOpt2" runat="server" Text=" In the last " GroupName="Days" />
                                 <asp:TextBox ID="txtSubmitDays" Text="2" Width="25px" runat="server" onKeyUp="numericFilter(this);" /> Days
                                </div>
                                </asp:Panel>                 
                       </div>            
                        <div style="width: 40%; height: auto; float: left; margin-Top: 6px;">
                            Sort List by
                            <br />
                            <br />
                            <div style="margin-Top: 3px;">
                                <asp:RadioButton ID="rbtnOrderByOpt1" runat="server" Text=" Submit time" GroupName="Sorting" Checked="true" OnCheckedChanged="SortOrderRadioButton_Checked" /></div>
                            <div style="margin-Top: 3px;">
                                <asp:RadioButton ID="rbtnOrderByOpt2" runat="server" Text=" Submitter" GroupName="Sorting" OnCheckedChanged="SortOrderRadioButton_Checked"  /></div>
                            <div style="margin-Top: 3px;">
                                <asp:RadioButton ID="rbtnOrderByOpt3" runat="server" Text=" Title" GroupName="Sorting" OnCheckedChanged="SortOrderRadioButton_Checked" /></div>
                            <div style="margin-Top: 3px;">
                                <asp:RadioButton ID="rbtnOrderByOpt4" runat="server" Text=" File Type" GroupName="Sorting" OnCheckedChanged="SortOrderRadioButton_Checked"  /></div>
                        </div>
                        <br style="clear: both" />
                        
                </div>
                    <div style="margin-top: 10px; height: auto; margin-right: 392px;" align="right"> 
            <asp:LinkButton ID="btnShowActivities"  runat="server" SkinId="ButtonBig" Text="Show Activities" OnClick="BtnShowActivities_Click" ForeColor="Blue"/>
            </div>
                    <div style="width: 100%; height: auto; margin-top: 20px;"> 
         <br />           
            <asp:Label ID="recCnt" runat="server"></asp:Label>&nbsp;Documents submitted.
        </div>
        <div>
            <br />
            <asp:LinkButton ID="LBtnFirst" runat="server"  OnClick="LBtnFirst_Click">First</asp:LinkButton>
            <asp:LinkButton ID="LBtnPrevious" runat="server"  OnClick="LBtnPrevious_Click">Previous</asp:LinkButton>
            <asp:LinkButton ID="LBtnNext" runat="server" OnClick="LBtnNext_Click">Next</asp:LinkButton>
            <asp:LinkButton ID="LBtnLast" runat="server"  OnClick="LBtnLast_Click">Last</asp:LinkButton>
            <asp:DropDownList ID="DDLPageSize" runat="server" OnSelectedIndexChanged="DDLPageSize_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="5" Text="5"></asp:ListItem>
                <asp:ListItem Value="10" Text="10"></asp:ListItem>
                <asp:ListItem Value="20" Text="20"></asp:ListItem>
            </asp:DropDownList>
            Total pages:&nbsp;<asp:Label ID="LblPageCount" runat="server"></asp:Label>
            Now viewing:&nbsp;<asp:Label ID="LblCurrentPage" runat="server"></asp:Label>
        </div>
                    <div style="width: 100%; height: auto; margin-top: 20px;">           
                        <asp:Repeater runat="server" id="rptNames" >
                            <HeaderTemplate>
                                <table border="1" cellspacing="2">            
                                    <tr style="height: 30px; background-color:#999;">
                                        <td>Title</td>
                                        <td>Author</td>
                                        <td>Submitted By</td>
                                        <td>File Type</td>
                                        <td>File Name</td>
                                        <td>Date Submitted</td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                  <tr style="height: 25px;">
                                    <td style="background-color: #ccc;"><a href="ViewDocument.aspx?DocId=<%# Eval("DocId") %>" style="color:#0000FF"><%# Eval("Title")%></a></td>
                                    <td style="background-color: #ccc;"><%# Eval("Author")%></td>
                                    <td style="background-color: #ccc;"><%# Eval("Submitter")%></td>
                                    <td style="background-color: #ccc;"><%# Eval("DocType")%></td>
                                    <td style="background-color: #ccc;"><%# Eval("DocName")%></td>
                                    <td style="background-color: #ccc;"><%# Eval("DateSubmitted","{0:dd-MMM-yyyy}")%></td>
                                  </tr>
                        </ItemTemplate>                        
                        <FooterTemplate>        
                        </table>
                        </FooterTemplate>
                        </asp:Repeater>
                      </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>


