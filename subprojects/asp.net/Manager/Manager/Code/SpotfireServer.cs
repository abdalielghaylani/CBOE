// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpotfireServer.cs" company="PerkinElmer Inc.">
//   Copyright (c) 2013 PerkinElmer Inc.,
//   940 Winter Street, Waltham, MA 02451.
//   All rights reserved.
//   This software is the confidential and proprietary information
//   of PerkinElmer Inc. ("Confidential Information"). You shall not
//   disclose such Confidential Information and may not use it in any way,
//   absent an express written license agreement between you and PerkinElmer Inc.
//   that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Manager.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Xml.XPath;

    /// <summary>
    /// Spotfire server information
    /// </summary>
    public class SpotfireServer
    {
        private readonly Uri uri;
        
        private string cookie;
        private string xsrfToken;

        internal SpotfireServer(string path)
        {
            this.uri = new Uri(path);
            Update();
        }

        internal Version Version { get; set; }

        internal string XsrfToken
        {
            get { return this.xsrfToken; } 
        }

        internal string Cookie
        {
            get { return this.cookie; }
        }

        /// <summary>
        /// Update itself by download and parsing manifest file
        /// </summary>
        internal void Update()
        {
            Uri manifestUri = new Uri(uri, "/spotfire/manifest");
            WebRequest webRequest = HttpWebRequest.CreateDefault(manifestUri);

            using (WebResponse response = webRequest.GetResponse())
            {
                XPathDocument manifest = new XPathDocument(response.GetResponseStream());
                XPathNavigator pathNavigator = manifest.CreateNavigator();
                this.Version = ParseVersion(pathNavigator, new Version(0, 0));

                var setCookie = response.Headers["Set-Cookie"];

                this.cookie = ParseCookie(setCookie);
                this.xsrfToken = ParseXsrfToken(setCookie);
            }
        }

        private Version ParseVersion(XPathNavigator rootNavigator, System.Version defaultIfMissing)
        {
            XPathExpression versionPath = XPathExpression.Compile("/manifest/server-info/version");
            XPathNavigator pathNavigator = rootNavigator.SelectSingleNode(versionPath);
            Version version;

            if (pathNavigator != null && System.Version.TryParse(pathNavigator.Value, out version))
            {
                return version;
            }

            return defaultIfMissing;
        }

        private string ParseCookie(string setCookie)
        {
            int pos = setCookie.IndexOf("JSESSIONID=");
            string str = setCookie.Substring(pos);
            pos = str.IndexOf(";");

            string session = str.Substring(0, pos);
            string token = ParseXsrfToken(setCookie);

            return string.Format("{0}; XSRF-TOKEN={1}", session, token);
        }

        private string ParseXsrfToken(string setCookie)
        {
            int pos = setCookie.IndexOf("XSRF-TOKEN=") + 11;
            string str = setCookie.Substring(pos);
            pos = str.IndexOf(";");

            return str.Substring(0, pos);
        }
    }
}