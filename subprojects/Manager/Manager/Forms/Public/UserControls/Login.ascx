<%@ Control Language="C#"  EnableTheming="false" AutoEventWireup="true" Inherits="Forms_Public_UserControls_Login" Codebehind="Login.ascx.cs" %>

<div id="wrapper">
	<div id="brand"></div>
    <div id="content">
    	<div id="info">
        	<h1>Welcome to ChemBioOffice Enterprise</h1>
            <p class="maininfo">Please enter your Registered ID to access applications for managing compounds, users, and more.</p>
            <!--p class="maininfo">Not a Registered User? Contact width...</p-->
            <br />
            <p class="maininfo">PerkinElmer is a leading supplier of discovery, collaboration, and knowledge enterprise solutions, desktop software, scientific databases and consulting services to the pharmaceutical, biotechnology, and chemical industries. The Company provides:</p>
            <br />
            <div class="product">
            	<img src="../../../app_themes/<%="Login" + this.Page.StyleSheetTheme%>/images/bgDs.gif" alt="Desktop software" />
                <p><strong>Chemical Registration</strong>  is a customizable application that assigns corporate registration numbers for compounds based on each organization's business rules. </p>
            </div><!--product-->
            <div class="product">
            	<img src="../../../app_themes/<%="Login" + this.Page.StyleSheetTheme%>/images/bgLab.gif" alt="Desktop software" />
              <p><strong>Inventory Enterprise</strong>  is an application designed to manage the chemical and biological reagent tracking needs of laboratories and research centers in multiple contexts. </p>
            </div>
            <div class="product">
            	<img src="../../../app_themes/<%="Login" + this.Page.StyleSheetTheme%>/images/bgMi.gif" alt="Desktop software" />
              <p><strong>ChemACX</strong> is a database of available chemicals that provides a unified, up-to-date, structure-searchable source of chemicals from over 500 suppliers worldwide.  </p>
            </div>
            <div class="product">
            	<img src="../../../app_themes/<%="Login" + this.Page.StyleSheetTheme%>/images/bgKm.gif" alt="Desktop software" />
              <p><strong>E-Notebook</strong> provides a smooth, well-organized interface designed to replace paper laboratory notebooks.  </p>
            </div>
            <div class="clear"></div>
            <br />
            <p class="maininfo" style="color:#000000">
                Support:<a style="color:#000099;" href="http://www.perkinelmer.com/Informatics/support/contact" target="_blank">http://www.perkinelmer.com/Informatics/support/contact</a>
            </p>
            <div class="clear">

            </div>
      </div><!--info-->
      <div id="login">
            <asp:Panel  ID="loginPanel" runat="server" defaultButton="">
                <asp:Login OnLoginError="DisplayError" DisplayRememberMe="false" ID="Login1" LabelStyle-CssClass="label" LoginButtonStyle-CssClass="button" LoginButtonText="Log In" TitleTextStyle-CssClass="loginTitle" runat="server"  DestinationPageUrl="~/Forms/Public/ContentArea/Home.aspx" OnLoggedIn="OnLoggedInEvent" FailureTextStyle-CssClass="failureText" />
                <asp:ValidationSummary runat="server" id="loginvalidation" ValidationGroup="ctl00$ContentPlaceHolder$Login1$Login1" ShowSummary="false" ShowMessageBox="true" />
            </asp:Panel>  
            <p class="Version">CBOE Framework Version: <%=CambridgeSoft.COE.Framework.COEConfigurationService.ConfigurationUtilities.GetFrameworkFileVersion()%></p>
            <div style="clear:both;"><asp:Label runat="server" Visible="false" ID="InactivityMessage" SkinID="ErrorText" /></div>
        </div>
        <img class="brand" src="../../../app_themes/<%="Login" + this.Page.StyleSheetTheme%>/images/PKI_FTB_Logo_RGB_small.jpg" alt="PerkinElmer Inc." />
    </div><!--content-->

</div><!--wrapper-->