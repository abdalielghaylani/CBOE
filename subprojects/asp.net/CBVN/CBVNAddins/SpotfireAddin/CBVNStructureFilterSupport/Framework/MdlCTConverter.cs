// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MdlCTConverter.cs" company="PerkinElmer Inc.">
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
    using System.IO;
    using System.Text;

    /// <summary>
    /// A converter from and to the MDL CT Format.
    /// </summary>
    internal static class MdlCTConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts the BLOB to CT data.
        /// </summary>
        /// <param name="blob">The BLOB value.</param>
        /// <returns>The CT data.</returns>
        public static byte[] ConvertBlobToCTData(BlobValue blob)
        {
            using (MemoryStream target = new MemoryStream())
            {
                WriteBlobToCTStream(blob, target);
                byte[] bytes = target.ToArray();
                return bytes;
            }
        }

        /// <summary>
        /// Converts the file to CT data.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The CT data.</returns>
        public static byte[] ConvertFileToCTData(string path, Encoding encoding)
        {
            using (FileStream source = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (MemoryStream target = new MemoryStream())
            {
                CopyStreamToCTStream(source, target, encoding);
                return target.ToArray();
            }
        }

        /// <summary>
        /// Converts the string to CT data.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The CT data.</returns>
        public static byte[] ConvertStringToCTData(string data, Encoding encoding)
        {
            using (MemoryStream target = new MemoryStream())
            {
                WriteStringToCTStream(data, encoding, target);
                return target.ToArray();
            }
        }

        /// <summary>
        /// Copies the CT file to file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="codepage">The codepage.</param>
        public static void CopyCTFileToFile(string source, string target, int codepage)
        {
            CopyCTFileToFile(source, target, Encoding.GetEncoding(codepage));
        }

        /// <summary>
        /// Copies the CT file to file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="encoding">The encoding.</param>
        public static void CopyCTFileToFile(string source, string target, Encoding encoding)
        {
            using (FileStream input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream output = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                CopyCTStreamToStream(input, output, encoding);
            }
        }

        /// <summary>
        /// Copies the file to CT file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="codepage">The codepage.</param>
        public static void CopyFileToCTFile(string source, string target, int codepage)
        {
            CopyFileToCTFile(source, target, Encoding.GetEncoding(codepage));
        }

        /// <summary>
        /// Copies the file to CT file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="encoding">The encoding.</param>
        public static void CopyFileToCTFile(string source, string target, Encoding encoding)
        {
            using (FileStream input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream output = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                CopyStreamToCTStream(input, output, encoding);
            }
        }

        /// <summary>
        /// Creates the BLOB from CT data.
        /// </summary>
        /// <param name="data">The byte data.</param>
        /// <param name="codepage">The codepage.</param>
        /// <returns>The blob value.</returns>
        public static BlobValue CreateBlobFromCTData(byte[] data, int codepage)
        {
            return CreateBlobFromCTData(data, Encoding.GetEncoding(codepage));
        }

        /// <summary>
        /// Creates the BLOB from CT data.
        /// </summary>
        /// <param name="data">The byte data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The blob value.</returns>
        public static BlobValue CreateBlobFromCTData(byte[] data, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream(data, false))
            {
                return CreateBlobFromCTStream(stream, encoding);
            }
        }

        /// <summary>
        /// Creates the BLOB from CT file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="codepage">The codepage.</param>
        /// <returns>The blob value.</returns>
        public static BlobValue CreateBlobFromCTFile(string path, int codepage)
        {
            return CreateBlobFromCTFile(path, Encoding.GetEncoding(codepage));
        }

        /// <summary>
        /// Creates the BLOB from CT file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The blob value.</returns>
        public static BlobValue CreateBlobFromCTFile(string path, Encoding encoding)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return CreateBlobFromCTStream(stream, encoding);
            }
        }

        /// <summary>
        /// Creates the BLOB from CT stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The blob value.</returns>
        public static BlobValue CreateBlobFromCTStream(Stream stream, Encoding encoding)
        {
            using (MemoryStream target = new MemoryStream())
            {
                CopyCTStreamToStream(stream, target, encoding);
                return new BlobValue(target.ToArray(), encoding);
            }
        }

        /// <summary>
        /// Writes the BLOB to CT file.
        /// </summary>
        /// <param name="blob">The BLOB value.</param>
        /// <param name="path">The path to the file.</param>
        public static void WriteBlobToCTFile(BlobValue blob, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                WriteBlobToCTStream(blob, stream);
            }
        }

        /// <summary>
        /// Writes the BLOB to CT stream.
        /// </summary>
        /// <param name="blob">The BLOB value.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteBlobToCTStream(BlobValue blob, Stream stream)
        {
            WriteDataToCTStream(blob.Bytes, blob.Encoding, stream);
        }

        /// <summary>
        /// Writes the data to CT stream.
        /// </summary>
        /// <param name="data">The byte data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteDataToCTStream(byte[] data, Encoding encoding, Stream stream)
        {
            using (MemoryStream source = new MemoryStream(data, false))
            {
                CopyStreamToCTStream(source, stream, encoding);
            }
        }

        /// <summary>
        /// Writes the string to CT stream.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="stream">The stream.</param>
        public static void WriteStringToCTStream(string data, Encoding encoding, Stream stream)
        {
            WriteDataToCTStream(encoding.GetBytes(data), encoding, stream);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies the CT stream to stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="encoding">The encoding.</param>
        private static void CopyCTStreamToStream(Stream source, Stream target, Encoding encoding)
        {
            byte[] lineSeparator = encoding.GetBytes("\r\n");

            byte[] buffer = new byte[Byte.MaxValue];
            while (true)
            {
                int length = source.ReadByte();
                if (length == -1)
                {
                    break;
                }

                if (length > 0)
                {
                    if (source.Read(buffer, 0, length) != length)
                    {
                        throw new FormatException("Invalid data format.");
                    }

                    target.Write(buffer, 0, length);
                }

                target.Write(lineSeparator, 0, lineSeparator.Length);
            }
        }

        /// <summary>
        /// Copies the stream to CT stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="encoding">The encoding.</param>
        private static void CopyStreamToCTStream(Stream source, Stream target, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(source, encoding))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    byte[] data = encoding.GetBytes(line);
                    if (data.Length > Byte.MaxValue)
                    {
                        throw new ArgumentException("Invalid format, at least one line is too long.");
                    }

                    target.WriteByte((byte)data.Length);
                    target.Write(data, 0, data.Length);
                }
            }
        }

        #endregion
    }
}