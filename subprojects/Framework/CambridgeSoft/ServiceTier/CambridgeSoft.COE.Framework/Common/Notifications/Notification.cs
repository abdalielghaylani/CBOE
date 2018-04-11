using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class Notification
    {
        private ArrayList notifications= new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        public ArrayList Notifications
        {
            get { return notifications; }
            set { notifications = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasMessages
        {
            get { return 0 != Notifications.Count; }
        }

       

    }
}
