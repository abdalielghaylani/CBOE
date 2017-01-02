using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;

namespace CambridgeSoft.COE.Framework.Common.Compression {
    public class GZip {
        /// <summary>
        /// Compress a given string and returns it as a byte array encoded as specified by the Encoding parameter.
        /// </summary>
        /// <param name="flatString">Flat string to be compressed.</param>
        /// <param name="encoding">The returning encoding.</param>
        /// <returns>A compressed byte array.</returns>
        public static byte[] CompressString(string flatString, Encoding encoding) {
            byte[] inbyt = encoding.GetBytes(flatString);
            MemoryStream objStream = new MemoryStream();
            GZipStream objZS = new GZipStream(objStream, System.IO.Compression.CompressionMode.Compress);
            objZS.Write(inbyt, 0, inbyt.Length);
            objZS.Flush();
            objZS.Close();
            return objStream.ToArray();
        }

        private static byte[] CompressDataSet(DataSet ds) {
            ds.RemotingFormat = SerializationFormat.Binary;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, ds);
            byte[] inbyt = ms.ToArray();
            System.IO.MemoryStream objStream = new MemoryStream();
            System.IO.Compression.GZipStream objZS = new System.IO.Compression.GZipStream(objStream, System.IO.Compression.CompressionMode.Compress);
            objZS.Write(inbyt, 0, inbyt.Length);
            objZS.Flush();
            objZS.Close();
            return objStream.ToArray();
        }

        /// <summary>
        /// Decompress a string that is a base 64 string and returns it encoded as specified by the Encoding parameter.
        /// </summary>
        /// <param name="compressedBase64String">Compressed string.</param>
        /// <param name="encoding">The returning encoding.</param>
        /// <returns>The result uncompressed string.</returns>
        public static string DecompressString(string compressedBase64String) {
            System.IO.MemoryStream objStream = new MemoryStream(Convert.FromBase64String(compressedBase64String));
            byte[] outByt = new byte[objStream.Length];
            System.IO.Compression.GZipStream objZS = new System.IO.Compression.GZipStream(objStream, System.IO.Compression.CompressionMode.Decompress);
            StringBuilder unCompressedXml = new StringBuilder();
            int length = 0;
            while((length = objZS.Read(outByt, 0, outByt.Length)) > 0) {
                for(int index = 0; index < length; index++) {
                    unCompressedXml.Append(Convert.ToChar(outByt[index]));
                }
            }

            objZS.Flush();
            objZS.Close();

            return unCompressedXml.ToString();
        }

    }
}
