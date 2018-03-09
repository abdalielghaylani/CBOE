using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common

{
    public class NotificationMessage
    {

      private string message;
      private string messageID;

      public string Message
      {
          get { return message; }
          set { message = value; }
      }


        public string MessageID
        {
            get { return messageID; }
            set { messageID = value; }
        }
	

    }
}
