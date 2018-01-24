using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Commong Business Logic Layer utils.
    /// </summary>
    public class BLLUtils
    {
        /// <summary>
        /// Clear out old notifications since the Manager may be stored as a reference on the client
        /// </summary>
        /// <returns></returns>
        public  static Notification ResetNotifications(BLLBase managerObject)
        {
            // add to Notifications property
            Notification notifications = new Notification();

            //bind notifications to manager
            managerObject.Notifications = notifications;

            //return notification object
            return notifications;
        }    
    }
}
