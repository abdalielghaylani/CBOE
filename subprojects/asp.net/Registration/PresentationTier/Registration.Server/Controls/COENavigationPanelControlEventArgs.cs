using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace PerkinElmer.COE.Registration.Server.Controls
{
    /// <summary>
    /// This is a Custom EventArgs to handle our custom information 
    /// about the control which implements ICoeNavigationPanelControl
    /// </summary>
    [Serializable]
    public class COENavigationPanelControlEventArgs : EventArgs
    {
        #region Variables

        private string _controlID = String.Empty;
        private string _eventType = String.Empty;

        #endregion

        #region Methods

        public COENavigationPanelControlEventArgs()
        {

        }

        public COENavigationPanelControlEventArgs(string controlID, string eventType)
        {
            this._controlID = controlID;
            this._eventType = eventType;
        }

        #endregion

        #region Properties

        public string ControlId
        {
            get { return this._controlID; }
        }

        public string EventType
        {
            get { return this._eventType; }
        }

        #endregion
    }
}
