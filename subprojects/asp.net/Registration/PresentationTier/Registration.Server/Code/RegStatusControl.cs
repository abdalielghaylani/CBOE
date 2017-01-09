using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Registration.Services.Types;
using System;
using System.Web.UI;

namespace CambridgeSoft.COE.Registration
{
    public class RegStatusControl : COEStateControl, IPostBackEventHandler
    {
        #region Constants
        private const string CAN_SET_APPROVED_FLAG = "SET_APPROVED_FLAG";
        private const string CAN_TOGGLE_APPROVED_FLAG = "TOGGLE_APPROVED_FLAG";
        private const string CAN_SET_LOCKED_FLAG = "SET_LOCKED_FLAG";
        private const string CAN_TOGGLE_LOCKED_FLAG = "TOGGLE_LOCKED_FLAG";
        #endregion

        public event EventHandler Click;

        #region Properties
        private RegistryRecord RegistryRecord
        {
            get { return (RegistryRecord)Page.Session[PerkinElmer.COE.Registration.Server.Constants.MultiCompoundObject_Session]; }
        }
        #endregion

        public override string Value
        {
            get
            {
                return this.RegistryRecord.Status.ToString();
            }
            set
            {
                base.Value = value;
            }
        }
        public string ValueToSave
        {
            get
            {
                if (ViewState["RegStausValue"] != null)
                    return (string)ViewState["RegStausValue"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["RegStausValue"] = value;
            }
        }
        #region Overriden LifeCycle Methods
        protected override void OnInit(System.EventArgs e)
        {
            this.Click += new EventHandler(RegStatusControl_ImageClick);
            base.OnInit(e);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            this.CurrentDisplayType = DisplayType.ImageButton;
            if (this.Enabled)
            {
                //Check the Permissions
                if (this.Value == "1" || this.Value == RegistryStatus.Submitted.ToString())
                {
                    if (!COEPrincipal.HasPrivilege(CAN_SET_APPROVED_FLAG, string.Empty))
                        this.Enabled = false;
                }
                else if (this.Value == "2" || this.Value == RegistryStatus.Approved.ToString())
                {
                    if (!COEPrincipal.HasPrivilege(CAN_TOGGLE_APPROVED_FLAG, string.Empty))
                        this.Enabled = false;
                }
                else if (this.Value == "3" || this.Value == RegistryStatus.Registered.ToString())
                {
                    if (!COEPrincipal.HasPrivilege(CAN_SET_LOCKED_FLAG, string.Empty))
                        this.Enabled = false;
                }
                else if (this.Value == "4" || this.Value == RegistryStatus.Locked.ToString())
                {
                    if (!COEPrincipal.HasPrivilege(CAN_TOGGLE_LOCKED_FLAG, string.Empty))
                        this.Enabled = false;
                }
                this.ValueToSave = this.Value;
            }
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "5px");
            base.Render(writer);
        }
        #endregion

        #region Event Handlers
        void RegStatusControl_ImageClick(object sender, EventArgs e)
        {
            if (this.RegistryRecord != null)
            {
                if (this.ValueToSave == "1" || this.ValueToSave == RegistryStatus.Submitted.ToString())
                    this.RegistryRecord.Status = RegistryStatus.Approved;
                else if (this.ValueToSave == "2" || this.ValueToSave == RegistryStatus.Approved.ToString())
                    this.RegistryRecord.Status = RegistryStatus.Submitted;
                else if (this.ValueToSave == "3" || this.ValueToSave == RegistryStatus.Registered.ToString())
                    this.RegistryRecord.Status = RegistryStatus.Locked;
                else if (this.ValueToSave == "4" || this.ValueToSave == RegistryStatus.Locked.ToString())
                    this.RegistryRecord.Status = RegistryStatus.Registered;
                if (this.RegistryRecord.Status == RegistryStatus.Locked || this.RegistryRecord.Status == RegistryStatus.Registered)
                {
                    this.RegistryRecord.SetLockStatus();
                }
                else
                {
                    this.RegistryRecord.SetApprovalStatus();
                }
                this.Value = this.RegistryRecord.Status.ToString();

                Page.DataBind();
            }
        }
        #endregion

        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "Click" && Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
