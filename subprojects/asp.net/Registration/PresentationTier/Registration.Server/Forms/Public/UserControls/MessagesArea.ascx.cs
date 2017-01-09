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


namespace PerkinElmer.COE.Registration.Server.Forms.Public.UserControls
{
    [Themeable(true)]
    public partial class MessagesArea : System.Web.UI.UserControl
    {
        #region Properties

        public string AreaText
        {
            set { this.MessagesAreaLabel.Text = value; }
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        public override string SkinID
        {
            get
            {
                return base.SkinID;
            }
            set
            {
                base.SkinID = value;
            }
        }

        #endregion

        #region Events Handlers

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
