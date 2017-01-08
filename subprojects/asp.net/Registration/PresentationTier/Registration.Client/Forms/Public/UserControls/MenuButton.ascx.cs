using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.CBOE.Registration.Client.Forms.Public.UserControls
{
    public partial class MenuButton : System.Web.UI.UserControl
    {
        #region GUIShell variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Private variables

        bool _validateAddComponent = false;

        #endregion

        #region Event Handlers
        public event EventHandler Click;
        public event CommandEventHandler Command;
        #endregion

        #region Properties
        public string Text
        {
            get { return this.SplitButtonLink.Text; }
            set { this.SplitButtonLink.Text = value; }
        }
        public string LeftImageURL
        {
            get { return this.LeftImage.Src; }
            set { this.LeftImage.Src = value; }
        }
        public string DropDownImageURL
        {
            get { return this.DropDownImage.Src; }
            set { this.DropDownImage.Src = value; }
        }
        public bool CausesValidation
        {
            get { return this.SplitButtonLink.CausesValidation; }
            set { this.SplitButtonLink.CausesValidation = value; }
        }
        public bool EnableMenu
        {
            get 
            {
                if(ViewState["EnableMenu"] != null)
                    return (bool) ViewState["EnableMenu"];
                else
                    return true;
            }
            set 
            {
                ViewState["EnableMenu"] = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.SplitButtonLink.Enabled;
            }
            set
            {
                this.SplitButtonLink.Enabled = value;
                this.MainButtonContainer.Disabled = !value;
            }
        }

        public string OnClientClick
        {
            get
            {
                if(ViewState["OnClientClick"] != null)
                    return (string) ViewState["OnClientClick"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["OnClientClick"] = value;
            }
        }
                
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<MenuItem> MenuItemList
        {
            get
            {
                if(ViewState["MenuItem"] != null)
                    return (List<MenuItem>) ViewState["MenuItem"];
                else
                    return new List<MenuItem>();
            }
            set
            {
                ViewState["MenuItem"] = value;
            }
        }

        public string MainButtonContainerClientID
        {
            get { return this.MainButtonContainer.ClientID; }
        }

        public bool ValidateAddComponent
        {
            get { return this._validateAddComponent; }
            set { this._validateAddComponent = value; }
        }

        #endregion

        #region Lyfe Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if(!Page.IsPostBack)
                    this.SetControlsAttributes();

                if(!EnableMenu)
                {
                    if(this.Enabled)
                    {
                        this.SplitButtonLink.Click += new EventHandler(SplitButtonLink_Click);
                    }
                    this.DropDownImage.Visible = false;
                }
                else
                {
                    if (this.ValidateAddComponent)
                    {
                        foreach (MenuItem menuControl in this.MenuItemList)
                        {
                            if (menuControl.Text == Resources.Resource.AddComponent_Button_Text)
                            {
                                menuControl.OnClientClick = "if (ValidateMenuItemClick(\"AddComponent\")==false){return false;};";
                            }
                        }
                    }
                    if (!Page.ClientScript.IsStartupScriptRegistered(typeof(MenuButton), "ShowHideMenu"))
                    {
                        string script = @"<script language='javascript' type='text/javascript'>
                        function ValidateMenuItemClick(menuItem) {
                            var retVal = false;
                            switch (menuItem) {
                                case 'AddComponent':
                                    if (CheckPageValidatorForMenus())
                                        retVal = true;
                                    break;
                            }
                            return retVal;
                        }
                        function ShowMenu(itemsContainerId, linkToFocusId)
                        {  
                            var menu = document.getElementById(itemsContainerId)
                            if(menu.style.display == 'none') {
                                menu.style.display = 'block';
                                var link = document.getElementById(linkToFocusId);
                                link.focus();
                            }
                            else {
                                menu.style.display = 'none';
                            }
                            return false;
                        }
                        function HideMenu(itemsContainerId)
                        {
                            var menu = document.getElementById(itemsContainerId)
                            if(menu.style.display != 'none') {
                                menu.style.display = 'none';
                            }
                            return false;
                        }
                    </script>
                            ";
                        Page.ClientScript.RegisterStartupScript(typeof(MenuButton), "ShowHideMenu", script, false);
                    }
                    this.SplitButtonLink.Attributes.Add("onclick", "return false;");
                    this.SplitButtonLink.Attributes.Add("onblur", "window.setTimeout(\"HideMenu('" + this.MenuItems.ClientID + "')\", 300);");
                    this.MainButtonContainer.Attributes.Add("onclick", "ShowMenu('" + this.MenuItems.ClientID + "', '" + this.SplitButtonLink.ClientID + @"');");
                    this.CreateMenuItems();
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.Page.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Page.Master;
            }
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!this.Enabled)
                this.MainButtonContainer.Attributes.Add("onclick", "return false;");

            else if (!EnableMenu)
            {
                string onClickString = string.Empty;
                if (!string.IsNullOrEmpty(this.OnClientClick))
                    onClickString = this.OnClientClick;
                else
                    onClickString = Page.ClientScript.GetPostBackClientHyperlink(this.SplitButtonLink, "");

                this.MainButtonContainer.Attributes.Add("onclick", onClickString);
            }
        }
        #endregion

        #region Event Handlers
        protected void SplitButtonLink_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.EnableMenu)
                {
                    if(Command != null)
                    {
                        CommandEventArgs eventArgs = new CommandEventArgs("Click", new string[] { SplitButtonLink.Text });
                        Command(sender, eventArgs);
                    }
                }
                else
                {
                    if(Click != null)
                        Click(sender, e);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }

        void MenuItem_Command(object sender, CommandEventArgs e)
        {
            if(Command != null)
            {
                Command(sender, e);
            }
        }

        
        #endregion

        #region Private Methods
        private void SetControlsAttributes()
        {

        }

        private void CreateMenuItems()
        {
            foreach(MenuItem menuItem in this.MenuItemList)
            {
                this.MenuItems.Controls.Add(menuItem);
                if(menuItem.Type == MenuItem.MenuItemType.RegularItem)
                    menuItem.Command += new CommandEventHandler(MenuItem_Command);
            }
        }

        #endregion

        #region Internal Methods
        internal string GetButtonUniqueID()
        {
            if(!this.EnableMenu && this.Enabled)
            {
                return this.SplitButtonLink.UniqueID;
            }
            else
                return this.UniqueID;
        }
        #endregion

        #region public Methods
        public void DisableControl()
        {
            this.MainButtonContainer.Disabled = true;
            this.SplitButtonLink.Enabled = false;
            this.MainButtonContainer.Attributes.Add("onclick", "return false;");
         }
        #endregion
    }
}