<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuItem.ascx.cs" Inherits="DocManagerWeb.Forms.Master.UserControls.MenuItem" %>

<div style="float:left">
    <div class="MenuButton" id="MainButtonContainer" runat="server">
       <asp:Button ID="SubmitDocument" runat="server" Text="Submit Another Document" /> 
       <asp:Button ID="SearchDocuments" runat="server" Text="Search Documents" /> 
       <asp:Button ID="BatchSubmission" runat="server" Text="Batch Submission" /> 
       <asp:Button ID="RecentActivities" runat="server" Text="Recent Activities" /> 
       <asp:Button ID="MainMenu" runat="server" Text="Main Menu" /> 
       <asp:Button ID="Home" runat="server" Text="Home" /> 
    </div>
    <div style="display: none;" class="MenuItemContainer" id="MenuItems" runat="server"/>
</div>
