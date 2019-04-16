using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;


namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// Common utilities class.
    /// </summary>
    public class Utilities
    {
        private const string DEFAULTKEY = "COEFramework";
        /// <summary>
        /// Returns the path to the specified project. It exctracts this information from the corresponding assembly.
        /// </summary>
        /// <param name="projectName">The name of the project whose path must be retrieved. It must match the assembly name</param>
        /// <returns>The project base path.</returns>
        public static string GetProjectBasePath(string projectName)
        {

            Assembly asm = Assembly.Load(projectName);
            string pathToAsm = asm.Location;
            string separatorString = @"\servicetier\";
            int separatorIndex = pathToAsm.ToLower().IndexOf(separatorString);
            string absolutePath = pathToAsm.Substring(0, separatorIndex + separatorString.Length);
            string projectBasePath = absolutePath + projectName;
            return projectBasePath;
        }

        /// <summary>
        /// Returs the left part of a string with a length of <paramref name="iLen"/>.
        /// </summary>
        /// <param name="strParam">The base string.</param>
        /// <param name="iLen">The length</param>
        /// <returns>The left part of a string</returns>
        public static String Left(String strParam, int iLen)
        {
            if (iLen > 0 && iLen < strParam.Length)
                return strParam.Substring(0, iLen);
            else
                return strParam;
        }

        /// <summary>
        /// Compress a plain text using GZip method.
        /// </summary>
        /// <param name="plainText">Text to compress</param>
        /// <returns>GZip compressed byte array</returns>
        public static byte[] GZipCompress(string plainText)
        {
            byte[] inputBuffer = Encoding.UTF8.GetBytes(plainText);

            MemoryStream outputStream = new MemoryStream();
            GZipStream stream = new GZipStream(outputStream, CompressionMode.Compress, true);
            stream.Write(inputBuffer, 0, inputBuffer.Length);
            stream.Close();

            return outputStream.ToArray();
        }

        /// <summary>
        /// Decompress a text using the GZip method.
        /// </summary>
        /// <param name="encodedText">GZip compressed byte array</param>
        /// <returns>Plain text</returns>
        public static string GZipDecompress(byte[] encodedText)
        {
            MemoryStream inputStream = new MemoryStream(encodedText);

            byte[] decompressedChunk = new byte[1000];
            string plainText = string.Empty;

            int bytesRead = 0;
            using (GZipStream stream = new GZipStream(inputStream, CompressionMode.Decompress))  //Coverity Fix CID 20288 ASV
            {
                do
                {
                    bytesRead = stream.Read(decompressedChunk, 0, decompressedChunk.Length);
                    string xmlChunk = Encoding.UTF8.GetString(decompressedChunk, 0, bytesRead);
                    plainText += xmlChunk;
                } while (bytesRead != 0);
            }
            return plainText;
        }

        public static bool SimulationMode()
        {
            if (Csla.ApplicationContext.GlobalContext["SimulationMode"] == null)
            {
                if (ConfigurationManager.AppSettings["SimulationMode"] != null)
                    Csla.ApplicationContext.GlobalContext["SimulationMode"] = bool.Parse(ConfigurationManager.AppSettings.Get("SimulationMode"));
                else
                    Csla.ApplicationContext.GlobalContext["SimulationMode"] = false;
            }

            return (bool)Csla.ApplicationContext.GlobalContext["SimulationMode"];
        }

        public static void RefreshSimulationMode()
        {
            if (ConfigurationManager.AppSettings["SimulationMode"] != null)
                Csla.ApplicationContext.GlobalContext["SimulationMode"] = bool.Parse(ConfigurationManager.AppSettings.Get("SimulationMode"));
            else
                Csla.ApplicationContext.GlobalContext["SimulationMode"] = false;
        }

        public static Object XmlDeserialize(string xml, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(new StringReader(xml));
        }

        /// <summary>
        /// Generic version of the XmlDeserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xml)
        {
            
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            XmlSerializer serializer = null;
            if (!string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI))
                serializer = new XmlSerializer(typeof(T), doc.DocumentElement.NamespaceURI);
            else
                serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(new StringReader(xml));
        }

        public static string XmlSerialize(Object source)
        {
            XmlSerializer serializer = new XmlSerializer(source.GetType());

            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, source);
            stringWriter.Flush();

            return stringWriter.ToString();
        }

        public static Object Deserialize(string xml)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            Byte[] buffer = Convert.FromBase64String(xml);

            MemoryStream stream = new MemoryStream(buffer);

            return binaryFormatter.Deserialize(stream);
        }

        public static string Serialize(Object source)
        {
            MemoryStream stream = new MemoryStream();

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(stream, source);


            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        /// Returns the hash as an array of 16 bytes. 
        /// Note that some MD5 implementations produce a 32-character, hexadecimal-formatted hash. To interoperate with such implementations, use <see cref="GetMD5Hash"/> function instead.
        /// </summary>
        /// <param name="value">String to compute its hash</param>
        /// <returns>The hash as an Array of 16 bytes</returns>
        public static byte[] ComputeMD5(string value)
        {
            using (MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider())   // Coverity Fix CID - 11813
            {
                byte[] byteValue = UTF32Encoding.UTF8.GetBytes(value.ToCharArray());
                return md5provider.ComputeHash(byteValue);
            }
        }

        /// <summary>
        /// Computes the MD5 hash value of a string and returns the hash as a 32-character, hexadecimal-formatted string. 
        /// The hash string created by this funciton is compatible with any MD5 hash function (on any platform) that creates a 32-character, hexadecimal-formatted hash string
        /// </summary>
        /// <param name="input">String to be hashed</param>
        /// <returns>Hashed string</returns>
        public static string GetMD5Hash(string input)
        {            
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Create a new instance of the MD5CryptoServiceProvider object.
            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())     // Coverity Fix CID - 11815 
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


        public static string EncryptRijndael(string plainText, string key)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(plainText))
            {
                byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(plainText);

                //Salt is created for additional degree of disorder in encrypted key
                byte[] Salt = //new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }; 
                    System.Text.Encoding.Unicode.GetBytes(key.Length.ToString());

                //This class uses an extension of the PBKDF1 algorithm defined 
                //in the PKCS#5 v2.0 standard to derive bytes suitable 
                //for use as key material from a password. 
                //The standard is documented in IETF RRC 2898.
                //Coverity Fix CID 22965
                using (PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt))
                {

                    Rijndael rijmenDaemen = Rijndael.Create();

                    rijmenDaemen.Key = SecretKey.GetBytes(32);
                    rijmenDaemen.IV = SecretKey.GetBytes(16);


                    using (MemoryStream encryptedStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, rijmenDaemen.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(PlainText, 0, PlainText.Length);
                        }
                        response = Convert.ToBase64String(encryptedStream.ToArray());
                    }
                }
            }
            return response;
        }

        public static string EncryptRijndael(string plainText)
        {
            return EncryptRijndael(plainText, DEFAULTKEY);
        }

        public static string Encrypt(string plainText, string password)
        {
            String cipherText = String.Empty;

            // Encode the passed plain text string into Unicode byte stream
            byte[] plainTextByte = new UnicodeEncoding().GetBytes(plainText);

            // Turn the password into Key and IV
            //Coverity Fix CID 22964
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
            {

                // Create a memory stream to which CryptoStream will write the cipher text
                using (MemoryStream cipherStream = new MemoryStream())
                {

                    Rijndael m_rj = Rijndael.Create();
                    m_rj.Key = pdb.GetBytes(32);
                    m_rj.IV = pdb.GetBytes(16);


                    // Create a CryptoStream in Write Mode; initialise withe the Rijndael's Encryptor ICryptoTransform
                    using (CryptoStream crypStream = new CryptoStream(cipherStream, m_rj.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Write the plaintext byte stream to CryptoStream
                        crypStream.Write(plainTextByte, 0, plainTextByte.Length);
                    }

                    // Extract the ciphertext byte stream and close the MemoryStream
                    byte[] cipherTextByte = cipherStream.ToArray();

                    // Encode the ciphertext byte into Unicode string
                    cipherText = Convert.ToBase64String(cipherTextByte);
                }
            }
            return cipherText;
        }

        public static string DecryptRijndael(string encryptedText, string key)
        {
            if (!string.IsNullOrEmpty(encryptedText))
            {
                byte[] PlainText = Convert.FromBase64String(encryptedText);

                //Salt is created for additional degree of disorder in encrypted key
                byte[] Salt = System.Text.Encoding.Unicode.GetBytes(key.Length.ToString());

                //This class uses an extension of the PBKDF1 algorithm defined 
                //in the PKCS#5 v2.0 standard to derive bytes suitable 
                //for use as key material from a password. 
                //The standard is documented in IETF RRC 2898.
                using (PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt))
                {
                    //Coverity Fix CID 13996
                    using (Rijndael rijmenDaemen = Rijndael.Create())
                    {

                        rijmenDaemen.Key = SecretKey.GetBytes(32);
                        rijmenDaemen.IV = SecretKey.GetBytes(16);

                        using (MemoryStream decryptedStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(decryptedStream, rijmenDaemen.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(PlainText, 0, PlainText.Length);
                            }
                            return Encoding.Unicode.GetString(decryptedStream.ToArray());
                        }
                    }
                }
            }
            return string.Empty;
        }


        public static string DecryptRijndael(string encryptedText)
        {
            return DecryptRijndael(encryptedText, DEFAULTKEY);
        }

        /// <summary>
        /// Indicates if the given text was encrypted with Rijndael algorithm and the given key
        /// </summary>
        /// <param name="text">The value</param>
        /// <param name="key">The key used for the original encryption</param>
        /// <returns>True if encrypted with this class</returns>
        public static bool IsRijndaelEncrypted(string text, string key)
        {
            try
            {
                text = DecryptRijndael(text, key);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates if the given text was encrypted with Rijndael algorithm and the default key
        /// </summary>
        /// <param name="text">The value</param>
        /// <returns>True if encrypted with this class</returns>
        public static bool IsRijndaelEncrypted(string text)
        {
            try
            {
                text = DecryptRijndael(text, DEFAULTKEY);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Pretty-prints an xml string.
        /// </summary>
        /// <param name="xmlString">A raw string representation of an xml document/node</param>
        /// <returns>A string representation of the formatted xml document/node</returns>
        public static string FormatXmlString(string xmlString)
        {
            string formattedXml = xmlString;

            MemoryStream memStream = new MemoryStream();
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(memStream, System.Text.Encoding.Unicode);
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            try
            {
                doc.LoadXml(xmlString);
                writer.Formatting = System.Xml.Formatting.Indented;

                doc.WriteContentTo(writer);
                writer.Flush();
                memStream.Flush();

                //re-position the memorystream cursor
                memStream.Position = 0;
                // Coverity Fix CID - 11814
                using (StreamReader reader = new StreamReader(memStream))
                {
                    formattedXml = reader.ReadToEnd();
                }

                memStream.Close();
                writer.Close();
            }
            catch (System.Xml.XmlException)
            {
                formattedXml = "There was a problem formatting the xml document string." + "\n\n" + formattedXml;
            }

            return formattedXml;
        }

        /// <summary>
        /// Given a numerical expression returns a list of values comma separated.
        /// expression is clause;clause;...
        /// where clause is num,num,num-num,... or start:incr:end
        /// example: "1-7;3:.25:10;1,5,9;4-7"
        /// </summary>
        /// <param name="expression">expression is clause;clause;...</param>
        /// <param name="bIntegers">Atr the numbers integers or not</param>
        /// <returns>A list of values separated by commas.</returns>
        public static string NumExpressionToString(string expression, bool bIntegers)
        {
            // expression is clause;clause;...
            // where clause is num,num,num-num,... or start:incr:end
            // example: "1-7;3:.25:10;1,5,9;4-7"

            string s = string.Empty, format = bIntegers ? "F0" : "N";
            List<List<double>> results = ParseExpression(expression);
            foreach (List<double> list in results)
            {
                foreach (double f in list)
                {
                    if (!string.IsNullOrEmpty(s))
                        s += ",";
                    s += f.ToString(format);
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public static List<List<double>> ParseExpression(string expression)
        {
            // from http://stackoverflow.com/questions/707508/c-string-convention-parsing
            // "x-y" is the same as "x:1:y" so simplify the expression...

            List<List<double>> results = new List<List<double>>();
            string partExpression = string.Empty;
            foreach (string part in expression.Split(';')) // Split the strings (Example -1--7;3:.25:10;1,-5,9;4-7;-1-7)
            {
                partExpression = part;
                //if the string does not contains comma then need to check as follows for such string parts (example -5--1, -5-1,5-1)

                if (partExpression.IndexOf(",") < 0) //Fixed CSBR-152027
                {
                    if (partExpression.IndexOf("--") > 0) // -5--1 (from -5 to -1)
                        partExpression = partExpression.Replace("--", "/-");
                    else
                    {
                        if (partExpression.StartsWith("-"))  // -5-1 (from -5 to 1)
                        {
                            partExpression = partExpression.Trim('-').Replace('-', '/');
                            partExpression = '-' + partExpression;
                        }
                        else
                            partExpression = partExpression.Trim('-').Replace('-', '/'); // 1-3 (from 1 to 3 equivalent to 1,2,3) 
                    }
                    partExpression = partExpression.Replace("/", ":1:");
                }

                results.Add(ParseSubExpression(partExpression));
            }
            return results;
        }
        //---------------------------------------------------------------------
        private static List<double> ParseSubExpression(string part)
        {
            List<double> results = new List<double>();

            // If this is a set of numbers... 
            if (part.IndexOf(',') != -1)
                // Then add each member of the set... 
                foreach (string a in part.Split(','))
                    results.AddRange(ParseSubExpression(a));
            // If this is a range that needs to be computed... 
            else if (part.IndexOf(":") != -1)
            {
                // Parse out the range parameters... 
                string[] parts = part.Split(':');
                double start = double.Parse(parts[0]);
                double increment = double.Parse(parts[1]);
                double end = double.Parse(parts[2]);

                // Evaluate the range... 
                for (double i = start; i <= end; i += increment)
                    results.Add(i);
            }
            else
                results.Add(double.Parse(part));

            return results;
        }

        /// <summary>
        /// Split a string by length.
        /// </summary>
        /// <param name="str">
        /// The string to be split.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// A list contains fixed length strings.
        /// </returns>
        public static List<string> SplitStringByLength(string str, int length)
        {
            var ret = new List<string>();

            for (var idx = 0; idx < str.Length; idx += length)
            {
                ret.Add(idx + length < str.Length ? str.Substring(idx, length) : str.Substring(idx));
            }

            return ret;
        }

        /// <summary>
        /// Get password from a Oracle connection string
        /// </summary>
        /// <param name="connstr">the given connection string</param>
        /// <returns>the password parsed or empty</returns>
        public static string GetPwdFromOracleConnStr(string connstr)
        {
            // locate the position of password section
            const string Title = "password";
            var pos = connstr.ToLower().IndexOf(Title, StringComparison.Ordinal);
            if (pos != -1)
            {
                // fetch the passworkd
                var substr = connstr.Substring(pos + Title.Length).TrimStart();
                if (substr[0] == '=')
                {
                    // skip "="
                    substr = substr.Substring(1).TrimStart();
                    // find section end position
                    pos = substr.IndexOf(";", StringComparison.Ordinal);
                    if (pos != -1)
                    {
                        substr = substr.Substring(0, pos);
                    }
                    return substr.TrimEnd();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Indicates if the given text was encrypted based on the FIPS enabled flag and the default key, 
        /// Rijndael algorithm for FIPSEnabled = false, AES for FIPSEnabled = true
        /// </summary>
        /// <param name="fipsEnabled">The FIPS enabled flag</param>
        /// <param name="text">The value</param>
        /// <returns>True if encrypted with this class</returns>
        public static bool IsEncrypted(bool fipsEnabled, string text)
        {
            if (fipsEnabled)
            {
                return IsAESEncrypted(text);
            }
            else
            {
                return IsRijndaelEncrypted(text);
            }
        }

        public static string Encrypt(bool fipsEnabled, string text)
        {
            if (fipsEnabled)
            {
                return EncryptAES(text);
            }
            else
            {
                return EncryptRijndael(text);
            }
        }

        public static string Decrypt(bool fipsEnabled, string text)
        {
            if (fipsEnabled)
            {
                return DecryptAES(text);
            }
            else
            {
                return DecryptRijndael(text);
            }
        }

        /// <summary>
        /// Indicates if the given text was encrypted with AES algorithm and the default key
        /// </summary>
        /// <param name="text">The value</param>
        /// <returns>True if encrypted with this class</returns>
        public static bool IsAESEncrypted(string text)
        {
            try
            {
                text = DecryptAES(text, DEFAULTKEY);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string DecryptAES(string encryptedText)
        {
            return DecryptAES(encryptedText, DEFAULTKEY);
        }

        public static string DecryptAES(string encryptedText, string key)
        {
            if (!string.IsNullOrEmpty(encryptedText))
            {
                byte[] PlainText = Convert.FromBase64String(encryptedText);

                //Salt is created for additional degree of disorder in encrypted key
                byte[] Salt = System.Text.Encoding.Unicode.GetBytes(key.Length.ToString());

                //This class uses an extension of the PBKDF1 algorithm defined 
                //in the PKCS#5 v2.0 standard to derive bytes suitable 
                //for use as key material from a password. 
                //The standard is documented in IETF RRC 2898.
                using (PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt))
                {
                    AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider();
                    var decryptor = aesCryptoProvider.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(decryptedStream, decryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(PlainText, 0, PlainText.Length);
                        }
                        return Encoding.Unicode.GetString(decryptedStream.ToArray());
                    }
                }
            }
            return string.Empty;
        }

        public static string EncryptAES(string plainText, string key)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(plainText))
            {
                byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(plainText);

                //Salt is created for additional degree of disorder in encrypted key
                byte[] Salt = //new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }; 
                    System.Text.Encoding.Unicode.GetBytes(key.Length.ToString());

                //This class uses an extension of the PBKDF1 algorithm defined 
                //in the PKCS#5 v2.0 standard to derive bytes suitable 
                //for use as key material from a password. 
                //The standard is documented in IETF RRC 2898.
                //Coverity Fix CID 22965
                using (PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt))
                {
                    AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider();
                    var encryptor = aesCryptoProvider.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                    using (MemoryStream encryptedStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(PlainText, 0, PlainText.Length);
                        }
                        response = Convert.ToBase64String(encryptedStream.ToArray());
                    }
                }
            }
            return response;
        }

        public static string EncryptAES(string plainText)
        {
            return EncryptAES(plainText, DEFAULTKEY);
        }
    }
}

