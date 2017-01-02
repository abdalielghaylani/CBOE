// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobValue.cs" company="PerkinElmer Inc.">
//   Copyright © 2011 - 2011 PerkinElmer Inc., 
// 100 CambridgePark Drive, Cambridge, MA 02140. 
// All rights reserved. 
// This software is the confidential and proprietary information 
// of PerkinElmer Inc. ("Confidential Information"). You shall not 
// disclose such Confidential Information and may not use it in any way, 
// absent an express written license agreement between you and PerkinElmer Inc. 
// that authorizes such use.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CBVNStructureFilterSupport.Framework
{
    using System;
    using System.Text;

    /// <summary>
    /// A blob value (consisting of a byte array and an encoding).
    /// </summary>
    internal class BlobValue
    {
        #region Constants and Fields

        /// <summary>
        /// The bytes that make up the blob.
        /// </summary>
        private readonly byte[] bytes;

        /// <summary>
        /// The encoding that should be used to interpret the blob.
        /// </summary>
        private readonly Encoding encoding;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobValue"/> class.
        /// </summary>
        /// <param name="bytes">The byte data.</param>
        /// <param name="encoding">The encoding.</param>
        public BlobValue(byte[] bytes, Encoding encoding)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            this.bytes = bytes;
            this.encoding = encoding;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public byte[] Bytes
        {
            get
            {
                return this.bytes;
            }
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a blob value from a string.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The corresponding blob value.</returns>
        public static BlobValue CreateFromString(string data, Encoding encoding)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            return new BlobValue(encoding.GetBytes(data), encoding);
        }

        #endregion
    }
}