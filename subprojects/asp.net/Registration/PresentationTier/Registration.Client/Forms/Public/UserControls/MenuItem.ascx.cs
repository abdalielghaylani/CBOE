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
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.CBOE.Registration.Client.Forms.Public.UserControls
{
    public partial class MenuItem : System.Web.UI.UserControl
    {
        #region GUIShell variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Public Events
        public event CommandEventHandler Command;
        #endregion

        #region Properties
        public string OnClientClick
        {
            get
            {
                if (ViewState["OnClientClick"] == null)
                    ViewState["OnClientClick"] = string.Empty;

                return (string)ViewState["OnClientClick"];
            }
            set
            {
                ViewState["OnClientClick"] = value;
            }
        }

        public string Text
        {
            get
            {
                if(ViewState["Text"] != null)
                    return (string) ViewState["Text"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["Text"] = value;
            }
        }
        
        public string CommandName
        {
            get
            {
                if(ViewState["CommandName"] != null)
                    return (string) ViewState["CommandName"];
                else
                    return string.Empty;
            }
            set
            {
                if(value != null)
                    this.ID = value.Replace(" ", "");

                ViewState["CommandName"] = value;
            }
        }

        public bool Enabled
        {
            get { return this.MenuItemLinkButton.Enabled; }
            set
            {
                this.MenuItemDiv.Disabled = !value;
                this.MenuItemLinkButton.Enabled = value;
            }
        }

        internal string SimpleSavePanelClientID
        {
            get
            {
                if(ViewState["SimpleSavePanelClientID"] != null)
                    return (string) ViewState["SimpleSavePanelClientID"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["SimpleSavePanelClientID"] = value;
            }
        }

        public MenuItemType Type
        {
            get
            {
                if(ViewState["Type"] != null)
                    return (MenuItemType) ViewState["Type"];
                else
                    return MenuItemType.RegularItem;
            }
            set
            {
                ViewState["Type"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether validation is performed 
        /// when the MenuItemLinkButton control is clicked.
        /// </summary>
        public bool CausesValidation
        {
            get { return this.MenuItemLinkButton.CausesValidation; }
            set { this.MenuItemLinkButton.CausesValidation = value; }
        }
        #endregion

        #region Page Life Cycle Events
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(this.Enabled)
                this.MenuItemLinkButton.Click += new EventHandler(MenuItemLinkButton_Click);
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
            this.MenuItemLinkButton.Text = this.Text;
            this.MenuItemLinkButton.Attributes.Add("onclick", "return false;");

            if(this.Type == MenuItemType.RegularItem)
            {
                if(this.Enabled)
                {
                    this.MenuItemDiv.Attributes.Add("onclick", this.OnClientClick + Page.ClientScript.GetPostBackClientHyperlink(this.MenuItemLinkButton, "").Replace("javascript:", ""));
                }
                else
                    this.MenuItemDiv.Attributes.Add("onclick", "return false;");
            }
            else
            {
                this.MenuItemDiv.Attributes.Add("onclick", "SetSimpleSavePanelVisibility('" + this.SimpleSavePanelClientID + "', true); return false;");
            }
            base.OnPreRender(e);
        }
        #endregion

        #region Event Handlers
        void MenuItemLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(Command != null)
                {
                    CommandEventArgs eventArgs = new CommandEventArgs(this.CommandName, string.Empty);
                    Command(sender, eventArgs);
                }
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        #endregion

        [System.ComponentModel.DefaultValue(1)]
        public enum MenuItemType
        {
            RegularItem = 1,
            SimplePanelDisplayer = 2
        }
    }
}
