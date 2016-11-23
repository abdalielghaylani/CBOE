using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using Oracle.DataAccess.Client;
using System.Runtime.Serialization;
using System.Data;

namespace CambridgeSoft.COE.Framework.Caching
{
    /// <summary>
    /// Inherits from the base cache to allow Oracle notifications of changes.
    /// </summary>
    public class OracleCacheDependency : COECacheDependency
    {
        OracleCommand _cmd = null;
        OracleDependency _dependency = null;
        List<string> _rowIDs = new List<string>();

        /// <summary>
        /// Initializes an OracleCacheDependency with a given command.
        /// </summary>
        /// <param name="cmd">Sql that defines the rows and objects in database that are being cached</param>
        public OracleCacheDependency(OracleCommand cmd)
            : base()
        {
            _cmd = cmd;
            this.AssociateCommand();
        }

        /// <summary>
        /// The command itself needs to be executed to have oracle listen for changes, and also the OnChange event needs a listener associated.
        /// </summary>
        private void AssociateCommand()
        {
            // The oracle dependency is made to be deleted automatically after 2400 secs,
            // to only notificate the first time (we recreate the notification if it was not the row we were listening), 
            // and not to be persistent, which means is not restored after a shutdown

            _cmd.Notification = null;
            _cmd.AddToStatementCache = false; // To allow multiple dependencies on the same query (multiple users?)

            bool manageConnectionClosing = false;
            if (_cmd.Connection != null && _cmd.Connection.State != System.Data.ConnectionState.Open)
            {
                _cmd.Connection.Open();
                manageConnectionClosing = true;
            }

            _dependency = new OracleDependency(_cmd);
            _dependency.QueryBasedNotification = false;
            _cmd.Notification.Timeout = 2400;

            _dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

            if (_cmd.CommandText.ToLower().Contains("rowid"))
            {
                // Fix Coverity: CID-28978 Resource leak
                using (OracleDataReader reader = _cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _rowIDs.Add(reader["rowid"].ToString());
                    }
                }
            }
            else
                _cmd.ExecuteNonQuery();

            if(manageConnectionClosing)
                _cmd.Connection.Close();

            
        }

        /// <summary>
        /// This methods notifies the base class that a dependency change ocurred, based on certain oracle events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        void dependency_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            bool triggerNotification = false;
            
            if (eventArgs.Details.Rows.Count > 0)
            {
                foreach (DataRow row in eventArgs.Details.Rows)
                {
                    if ((_rowIDs.Count == 0 || _rowIDs.Contains(row["ROWID"].ToString())) && 
                        (eventArgs.Info == OracleNotificationInfo.Update || eventArgs.Info == OracleNotificationInfo.Delete))
                    {
                        triggerNotification = true;
                        break;
                    }
                }
            }

            if (triggerNotification)
            {
                NotifyDependencyChanged(sender, eventArgs as EventArgs);
            }
            else
            {
                //The notification was removed from db, but was not due to a change we wanted to listen, so lets re-create the notification
                this.AssociateCommand();
            }
        }
    }
}
