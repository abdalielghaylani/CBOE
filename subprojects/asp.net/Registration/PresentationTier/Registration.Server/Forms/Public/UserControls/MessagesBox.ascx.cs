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
using System.Drawing;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace PerkinElmer.COE.Registration.Server.Forms.Public.UserControls
{
    public enum MessageBoxIcons
    {
        None, Asteriks, Exclamation, Question
    }

    public enum MessageBoxStyle
    {
        Blue, Violet   //other style enumerations here..
    }

    public partial class MessagesBox : System.Web.UI.UserControl
    {
        public string Title
        {
            get
            {
                return lblTitle.Text;
            }
        }

        public void Show(string Message, string Caption)
        {
            Show(Message, Caption, null, "OK", null, MessageBoxIcons.None, MessageBoxStyle.Blue);

        }
        public void Show(string Message, string Caption, string Btn1Text, string Btn2Text, string Btn3Text)
        {
            Show(Message, Caption, Btn1Text, Btn2Text, Btn3Text, MessageBoxIcons.None, MessageBoxStyle.Blue);
        }
        public void Show(string Message, string Caption, string Btn1Text, string Btn2Text, string Btn3Text, MessageBoxIcons m)
        {
            Show(Message, Caption, Btn1Text, Btn2Text, Btn3Text, m, MessageBoxStyle.Blue);
        }
        public void Show(string Message, string Caption, string Btn1Text, string Btn2Text, string Btn3Text, MessageBoxIcons m, MessageBoxStyle s)
        {
            Page.ClientScript.RegisterStartupScript(typeof(MessagesBox), "calis", "showPopWin('" + Caption + "', null,'" + this.ID + "', '" + this.popupTitleBar.ClientID + "');", true);
            lblMessage.Text = Message;
            lblTitle.Text = Caption;

            InitializeStyle(s);
            InitializeButtons(Btn1Text, Btn2Text, Btn3Text);
            InitializeIcon(m);
        }

        #region GUIShell variables

        RegistrationMaster _masterPage = null;

        #endregion

        #region Visual Initializations
        private void InitializeButtons(string Btn1Text, string Btn2Text, string Btn3Text)
        {



            btn1.Visible = !((Btn1Text == "") || (Btn1Text == null));
            btn2.Visible = !((Btn2Text == "") || (Btn2Text == null));
            btn3.Visible = !((Btn3Text == "") || (Btn3Text == null));



            btn1.Text = Btn1Text;
            btn2.Text = Btn2Text;
            btn3.Text = Btn3Text;




        }
        private void InitializeIcon(MessageBoxIcons m)
        {
            switch (m)
            {
                case MessageBoxIcons.None:
                    {
                        iIcon.Visible = false;
                        break;
                    }
                case MessageBoxIcons.Asteriks:
                    {
                        iIcon.Visible = true;
                        iIcon.ImageUrl = "./Images/bilgi.gif";
                        break;
                    }
                case MessageBoxIcons.Exclamation:
                    {
                        iIcon.Visible = true;
                        iIcon.ImageUrl = "./Images/uyari.gif";
                        break;
                    }
                case MessageBoxIcons.Question:
                    {
                        iIcon.Visible = true;
                        iIcon.ImageUrl = "./Images/soru.gif";
                        break;
                    }

            }
        }
        private void InitializeStyle(MessageBoxStyle m)
        {
            HtmlGenericControl p;
            HtmlTable t;

            p = (HtmlGenericControl)FindControl("popupContainer");
            p.Style.Add("Border", "transparent 0px solid");
            p.Visible = true;



            p = (HtmlGenericControl)FindControl("popupMask");
            p.Visible = true;

            t = (HtmlTable)FindControl("DesignTable");
            t.Visible = false;


            //other style definitions here...
            switch (m)
            {
                case MessageBoxStyle.Blue:


                    p = (HtmlGenericControl)FindControl("popupInner");
                    p.Style.Add("BORDER-RIGHT", "#b9c9ef 2px solid");
                    p.Style.Add("BORDER-TOP", "#b9c9ef 2px solid");
                    p.Style.Add("BORDER-LEFT", "#b9c9ef 2px solid");
                    p.Style.Add("BORDER-BOTTOM", "#b9c9ef 2px solid");
                    p.Style.Add("BACKGROUND-COLOR", "#000000");


                    p = (HtmlGenericControl)FindControl("popupTitleBar");
                    p.Style.Add("border-bottom", "#B9C9EF 1px solid");
                    p.Style.Add("background-color", "#E0E9F8");
                    p.Style.Add("color", "#1F336B");

                    t = (HtmlTable)FindControl("MainTable");
                    t.Style.Add("FILTER", "progid:DXImageTransform.Microsoft.Gradient(startColorStr='#E0E9F8', endColorStr='#ebeced', gradientType='0')");

                    btn1.ForeColor = Color.FromArgb(31, 51, 107);
                    btn2.ForeColor = Color.FromArgb(31, 51, 107);
                    btn3.ForeColor = Color.FromArgb(31, 51, 107);

                    btn1.BackColor = Color.FromArgb(224, 233, 248);
                    btn2.BackColor = Color.FromArgb(224, 233, 248);
                    btn3.BackColor = Color.FromArgb(224, 233, 248);

                    btn1.BorderColor = Color.FromArgb(185, 201, 239);
                    btn2.BorderColor = Color.FromArgb(185, 201, 239);
                    btn3.BorderColor = Color.FromArgb(185, 201, 239);
                    break;
                case MessageBoxStyle.Violet:

                    p = (HtmlGenericControl)FindControl("popupInner");
                    p.Style.Add("BORDER-RIGHT", "#400080 2px solid");
                    p.Style.Add("BORDER-TOP", "#400080 2px solid");
                    p.Style.Add("BORDER-LEFT", "#400080 2px solid");
                    p.Style.Add("BORDER-BOTTOM", "#400080 2px solid");
                    p.Style.Add("BACKGROUND-COLOR", "#000000");


                    p = (HtmlGenericControl)FindControl("popupTitleBar");
                    p.Style.Add("border-bottom", "#400080 1px solid");
                    p.Style.Add("background-color", "#D2C8DC");
                    p.Style.Add("color", "#1F336B");

                    t = (HtmlTable)FindControl("MainTable");
                    t.Style.Add("FILTER", "progid:DXImageTransform.Microsoft.Gradient(startColorStr='#D2C8DC', endColorStr='#FFFFFF', gradientType='0')");

                    btn1.ForeColor = Color.FromArgb(31, 51, 107);
                    btn2.ForeColor = Color.FromArgb(31, 51, 107);
                    btn3.ForeColor = Color.FromArgb(31, 51, 107);

                    btn1.BackColor = Color.FromArgb(210, 200, 220);
                    btn2.BackColor = Color.FromArgb(210, 200, 220);
                    btn3.BackColor = Color.FromArgb(210, 200, 220);

                    btn1.BorderColor = Color.FromArgb(64, 00, 128);
                    btn2.BorderColor = Color.FromArgb(64, 00, 128);
                    btn3.BorderColor = Color.FromArgb(64, 00, 128);
                    break;


            }
        }
        #endregion


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //

            InitializeComponent();
            if (this.Page.Master is RegistrationMaster)
            {
                _masterPage = (RegistrationMaster)this.Page.Master;
            }
            base.OnInit(e);
        }

        void Mbox_PreRender(object sender, EventArgs e)
        {
            lblMessage.Text = "dizayn...";
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblMessage.Text = "dizayn...";

        }
        #endregion

        #region Events & Raisings
        public event EventHandler Btn1Clicked;
        public event EventHandler Btn2Clicked;
        public event EventHandler Btn3Clicked;
        protected void btn1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Btn1Clicked != null)
                    Btn1Clicked(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        protected void btn2_Click(object sender, EventArgs e)
        {
            try
            {
                if (Btn2Clicked != null)
                    Btn2Clicked(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        protected void btn3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Btn3Clicked != null)
                    Btn3Clicked(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleUIException(exception);
                _masterPage.DisplayErrorMessage(exception, false);
            }
        }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeStyle(MessageBoxStyle.Blue);
        }
    }
}