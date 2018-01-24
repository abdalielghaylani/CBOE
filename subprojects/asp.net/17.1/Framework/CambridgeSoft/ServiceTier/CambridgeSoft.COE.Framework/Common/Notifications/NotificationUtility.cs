using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="messageID"></param>
        /// <param name="message"></param>
        public static void AddNotification(ref Notification notification, string messageID, string message)
        {
            NotificationMessage newMessage = new NotificationMessage();
            newMessage.MessageID = messageID;
            newMessage.Message = message;

            notification.Notifications.Add(newMessage);
        }


        public static void FormatAsSoapException(Notification notification, ref XmlNode node, ref XmlDocument doc )
        {
            for (int i = 0; i < notification.Notifications.Count; i++)
            {
                // Build specific details for the SoapException.
                // Add first child of detail XML element.
                System.Xml.XmlNode details =
                  doc.CreateNode(XmlNodeType.Element, "Notifications" + i,
                                 "http://cambridgesoft.com/");
                System.Xml.XmlNode detailsChild =
                  doc.CreateNode(XmlNodeType.Element, "childOfNotifications" + i,
                                 "http://cambridgesoft.com/");

                XmlAttribute attr = doc.CreateAttribute("t", "message",
                                   "http://cambridgesoft.com/");
                attr.Value = ((NotificationMessage)notification.Notifications[i]).Message;

                details.AppendChild(detailsChild);


                // Append child elements to the detail node.
                node.AppendChild(details);
            }
            
        }
    }

    
}
