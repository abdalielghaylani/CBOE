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

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls
{
    public partial class MenuItem : System.Web.UI.UserControl
    {
        #region Public Events
        public event CommandEventHandler Command;
        #endregion

        #region Properties
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
                ViewState["CommandName"] = value;
            }
        }

        public string CommandArgument
        {
            get
            {
                if (ViewState["CommandArgument"] != null)
                    return ViewState["CommandArgument"] as string;
                else
                    return string.Empty;
            }
            set
            {
                ViewState["CommandArgument"] = value;
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

        public string OnClientClick
        {
            get
            {
                if(ViewState["OnClientClick"] == null)
                    ViewState["OnClientClick"] = string.Empty;

                return ViewState["OnClientClick"] as string;
            }
            set
            {
                ViewState["OnClientClick"] = value;
            }
        }
        #endregion

        #region Page Life Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MenuItemLinkButton.Text = this.Text;

            this.MenuItemLinkButton.Attributes.Add("onclick", "return false");

            if(this.Type == MenuItemType.RegularItem)
            {
                if(this.Enabled)
                {
                    this.MenuItemDiv.Attributes.Add("onclick", this.OnClientClick + " " + Page.ClientScript.GetPostBackClientHyperlink(this.MenuItemLinkButton, ""));
                }

                this.MenuItemLinkButton.Click += new EventHandler(MenuItemLinkButton_Click);
            }
            else
            {
                this.MenuItemDiv.Attributes.Add("onclick", "SetSimpleSavePanelVisibility('" + this.SimpleSavePanelClientID + "', true); return false;");
            }
        }
        #endregion

        #region Event Handlers
        void MenuItemLinkButton_Click(object sender, EventArgs e)
        {
            if(Command != null)
            {
                CommandEventArgs eventArgs = new CommandEventArgs(this.CommandName, this.CommandArgument);
                Command(sender, eventArgs);
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