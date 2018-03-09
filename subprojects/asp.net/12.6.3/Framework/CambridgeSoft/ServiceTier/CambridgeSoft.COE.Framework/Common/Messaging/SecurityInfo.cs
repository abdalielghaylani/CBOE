using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CambridgeSoft.COE.Framework.Common {
    /// <summary>
    /// The security info messaging type.
    /// </summary>
    [XmlRoot(ElementName = "SecurityInfo", IsNullable = false, Namespace = "COE.SecurityInfo"), Serializable]
    public class SecurityInfo {

        #region Variables
        private string identityToken;
        private string userName;
        private string password;
        private const string xmlNS = "COE.SecurityInfo";
        #endregion

        #region Properties
        /// <summary>
        /// The identity token.
        /// </summary>
        [XmlElement(ElementName = "IdentityToken", DataType = "string")]
        public string IdentityToken {
            get { return this.identityToken; }
            set { this.identityToken = value; }
        }

        /// <summary>
        /// The user name.
        /// </summary>
        [XmlElement(ElementName = "UserName", DataType = "string")]
        public string UserName {
            get { return this.userName; }
            set { this.userName = value; }
        }

        /// <summary>
        /// The password.
        /// </summary>
        [XmlElement(ElementName = "Password", DataType = "string")]
        public string Password {
            get { return this.password; }
            set { this.password = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public SecurityInfo() {
            this.identityToken = string.Empty;
            this.password = string.Empty;
            this.userName = string.Empty;
        }
        /// <summary>
        /// Initializes its members to the desired values.
        /// </summary>
        /// <param name="identityToken">The identity token.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public SecurityInfo(string identityToken, string userName, string password) {
            this.IdentityToken = identityToken;
            this.UserName = userName;
            this.Password = password;
        }

        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="doc">The xml representation.</param>
        public SecurityInfo(XmlDocument doc) {
            this.GetFromXml(doc);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="doc">The xml representation.</param>
        public void GetFromXml(XmlDocument doc) {
            foreach(XmlNode currentNode in doc.DocumentElement.ChildNodes) {
                if(currentNode.NodeType == XmlNodeType.Element) {
                    switch(currentNode.Name.Trim().ToLower()) {
                        case "identitytoken":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.identityToken = currentNode.InnerText;
                            break;
                        case "username":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.userName = currentNode.InnerText;
                            break;
                        case "password":
                            if(currentNode.InnerText.ToString().Trim() != string.Empty)
                                this.password = currentNode.InnerText;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes its members from its xml representation.
        /// </summary>
        /// <param name="xml">The xml representation as string.</param>
        public void GetFromXml(string xml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            this.GetFromXml(doc);
        }

        /// <summary>
        /// Builds its xml representation as string .
        /// </summary>
        /// <returns>The xml representation as string.</returns>
        public override string ToString() {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<SecurityInfo xmlns=\"" + xmlNS + "\">");
            builder.Append("<IdentityToken>");
            builder.Append(this.identityToken);
            builder.Append("</IdentityToken>");
            builder.Append("<UserName>");
            builder.Append(this.userName);
            builder.Append("</UserName>");
            builder.Append("<Password>");
            builder.Append(this.password);
            builder.Append("</Password>");
            builder.Append("</SecurityInfo>");

            return builder.ToString();
        }
        #endregion
    }
}
