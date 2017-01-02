using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace FormDBLib
{
    public class Cryptography
    {
        #region Variables
        private Rijndael m_rj;    // Define the Cryptography Service Provider
        #endregion

        #region Methods
        /// <summary>
        ///  Encrypts a plain text with a given password
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Encrypt(string plainText, string password)
        {
            String cipherText = String.Empty;

            // Encode the passed plain text string into Unicode byte stream
            byte[] plainTextByte = new UnicodeEncoding().GetBytes(plainText);

            // Turn the password into Key and IV
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
            {

                // Create a memory stream to which CryptoStream will write the cipher text
                using (MemoryStream cipherStream = new MemoryStream())
                {

                    m_rj = Rijndael.Create();
                    //Coverity Bug Fix CID 10888 (Local Analysis)
                    if (m_rj != null)
                    {
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
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Decrypts a cipher text with the same password used to encrypt it before
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText, string password)
        {
            String plainText = String.Empty;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
            {

                // Create a memory stream from which CryptoStream will read the cipher text
                using (MemoryStream cipherTextStream = new MemoryStream())
                {
                    m_rj = Rijndael.Create();
                    //Coverity Bug Fix CID 10887 (Local Analysis)
                    if (m_rj != null)
                    {
                        m_rj.Key = pdb.GetBytes(32);
                        m_rj.IV = pdb.GetBytes(16);

                        // Create a CryptoStream and initialize with the Rijndael's Decryptor
                        using (CryptoStream crypStream = new CryptoStream(cipherTextStream, m_rj.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            // Write the cipher text byte stream to CryptoStream
                            crypStream.Write(cipherBytes, 0, cipherBytes.Length);
                        }
                        //  Extract the plaintext byte stream and close the Streams
                        byte[] plainByteText = cipherTextStream.ToArray();
                        plainText = System.Text.Encoding.Unicode.GetString(plainByteText);
                    }
                }
                return plainText;
            }
        }
        #endregion
    }
}
