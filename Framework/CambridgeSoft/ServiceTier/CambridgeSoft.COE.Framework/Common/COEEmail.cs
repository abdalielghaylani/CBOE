using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Class to work with email 
    /// </summary>
    public class COEEmail
    {
        #region Variables

        private string _host = string.Empty;
        private int _port = 25;
        private List<string> _to;
        private string _from = string.Empty;
        private string _subject = string.Empty;
        private string _body = string.Empty;
        private SmtpClient _smtpClient = new SmtpClient();
        private MailMessage _message = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the host (IP or host name).
        /// </summary>
        /// <value>The host that will be in charge of sending the email.</value>
        public string Host
        {
            set
            {
                _smtpClient.Host = value;
            }
            get
            {
                return _smtpClient.Host;
            }

        }

        /// <summary>
        /// Gets or sets the port number (by default 25).
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            set
            {
                _smtpClient.Port = value;
            }

            get
            {
                return _smtpClient.Port;
            }
        }

        /// <summary>
        /// Gets or sets the list of receipients.
        /// </summary>
        /// <value>To.</value>
        public List<string> To
        {
            set
            {
                _to = value;
            }
            get
            {
                return _to;
            }
        }

        /// <summary>
        /// Gets or sets who is sending the email message.
        /// </summary>
        /// <value>From who</value>
        public string From
        {
            get
            {
                return _message.From.Address;
            }
        }

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get
            {
                return _message.Subject;
            }
        }

        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        /// <value>The body.</value>
        public string Body
        {
            get
            {
                return _message.Body;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="COEEmail"/> class.
        /// </summary>
        private COEEmail()
        {
            if (_to == null)
                _to = new List<string>();
            if (_message == null)
                _message = new MailMessage();
            if (_smtpClient == null)
                _smtpClient = new SmtpClient();
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="COEEmail"/> class.
        /// </summary>
        /// <param name="host">The host name/ip</param>
        /// <param name="port">The port.</param>
        private COEEmail(string host, int port)
            : this()
        {
            this.Host = host;
            this.Port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="COEEmail"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="receivers">The list of receivers.</param>
        /// <param name="from">From who.</param>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <param name="isHtml">Indicates whether the message is html or plain</param>
        private COEEmail(string host, int port, List<string> receivers, string from, string subject, string body, bool isHtml)
            : this(host, port)
        {
            this.SetMessage(receivers, from, subject, body, isHtml);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the email object.
        /// </summary>
        /// <returns></returns>
        public static COEEmail CreateEmail()
        {
            return new COEEmail();
        }

        /// <summary>
        /// Creates the email object.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public static COEEmail CreateEmail(string host, int port)
        {
            return new COEEmail(host, port);
        }

        /// <summary>
        /// Creates the email object.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="receivers">The receivers.</param>
        /// <param name="from">The email's from.</param>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <param name="isHtml">Indicates whether the message is html or plain</param>
        /// <returns></returns>
        public static COEEmail CreateEmail(string host, int port, List<string> receivers, string from, string subject, string body, bool isHtml)
        {
            return new COEEmail(host, port, receivers, from, subject, body, isHtml);
        }

        /// <summary>
        /// Sets the message attributes.
        /// </summary>
        /// <param name="receivers">The list of receipients</param>
        /// <param name="from">The email's from.</param>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <param name="isHtml">Indicates whether the message is html or plain</param>
        public void SetMessage(List<string> receivers, string from, string subject, string body, bool isHtml)
        {
            foreach (string to in receivers)
                _message.To.Add(to);
            _message.From = new MailAddress(from);
            _message.Subject = subject;
            _message.Body = body;
            _message.IsBodyHtml = isHtml;
        }

        /// <summary>
        /// Sends the message to the given list of receipients.
        /// </summary>
        public void SendMessage()
        {
            _smtpClient.Send(_message);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="receivers">The list of receipients</param>
        /// <param name="from">The email's from.</param>
        /// <param name="subject">The email's subject.</param>
        /// <param name="body">The email's body.</param>
        /// <param name="isHtml">Indicates whether the message is html or plain</param>
        public void SendMessage(List<string> receivers, string from, string subject, string body, bool isHtml)
        {
            this.SetMessage(receivers, from, subject, body, isHtml);
            this.SendMessage();
        }

        /// <summary>
        /// Determines whether [is valid email] [the specified email] (syntactically).
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid email] [the specified email]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>It just checks for the email format (it's not checking using smtp handshake, etc)</remarks>
        public static bool IsValidEmail(string email)
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(email))
            {
                string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                      @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(strRegex);
                retVal = re.IsMatch(email);
            }
            return retVal;
        }

        #endregion
    }
}
