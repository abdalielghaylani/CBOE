// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerException.cs" company="PerkinElmer Inc.">
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

namespace CambridgeSoft.COE.Framework.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Server related exception
    /// </summary>
    [Serializable]
    public class ServerException : Exception
    {
        /// <summary>
        /// detailed error information
        /// </summary>
        private readonly string details;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException" /> class.
        /// default constructor
        /// </summary>
        public ServerException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException" /> class.
        /// use message as parameter to initiate exception
        /// </summary>
        /// <param name="message">
        /// error message
        /// </param>
        public ServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException" /> class.
        /// use message as parameter to initiate exception
        /// </summary>
        /// <param name="message">
        /// error message
        /// </param>
        /// <param name="detailInfo">
        /// detailed error info
        /// </param>
        public ServerException(string message, string detailInfo) : base(message)
        {
            this.details = detailInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException" /> class.
        /// serialization constructor
        /// </summary>
        /// <param name="serializationInfo">
        /// serialization information
        /// </param>
        /// <param name="context">streaming context</param>
        public ServerException(
            System.Runtime.Serialization.SerializationInfo serializationInfo,
            System.Runtime.Serialization.StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Gets detail info property
        /// </summary>
        public string Details
        {
            get
            {
                return this.details;
            }
        }

        /// <summary>
        /// convert to String
        /// </summary>
        /// <returns>return the string format</returns>
        public override string ToString()
        {
            return this.details;
        }
    }
}
