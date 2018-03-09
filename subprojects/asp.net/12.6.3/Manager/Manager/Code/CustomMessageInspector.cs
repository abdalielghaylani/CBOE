// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomMessageInspector.cs" company="PerkinElmer Inc.">
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
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Web;

    /// <summary>
    /// The message inspector class for adding XSRF token in request header
    /// </summary>
    public class CustomMessageInspector : IClientMessageInspector
    {
        private SpotfireServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMessageInspector" /> class.
        /// </summary>
        /// <param name="server">The Spotfire server to be accessed</param>
        public CustomMessageInspector(SpotfireServer server)
            : base()
        {
            this.server = server;
        }

        /// <summary>
        /// Add XSRF token and cookie before sending web service request
        /// </summary>
        /// <param name="request">the web service request</param>
        /// <param name="channel">the client channel</param>
        /// <returns>The object that is returned as the correlationState argument.</returns>
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty property;

            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                property = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            }
            else
            {
                property = new HttpRequestMessageProperty();
                request.Properties.Add(HttpRequestMessageProperty.Name, property);
            }

            property.Headers["X-XSRF-TOKEN"] = server.XsrfToken;
            property.Headers["Cookie"] = server.Cookie;

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is
        /// received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            // throw new NotImplementedException();
        }
    }
}