using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.IO;

namespace CambridgeSoft.COE.Security.Services.Utlities
{
	public static class Encryptor
	{
		/// <summary>
		/// Converts the specified byte array to a string of hex characters representing the array
		/// </summary>
		/// <param name="value">An array of bytes to be converted</param>
		/// <returns>A string of hexidecimal characters</returns>
		public static string Encrypt(string p)
		{
			//byte[] encryptedPass = HexConverter.ToByteArray(p);
			byte[] iv = { 218, 123, 211, 4, 145, 147, 132, 228, 121, 39, 222, 60, 158, 10, 229, 62 };
			byte[] keyPass = UnicodeConverter.ToByteArray("LdapPass");
			
			byte[] toEncryptPass;
            using(RijndaelManaged myRijndael = new RijndaelManaged())
            {
			//Get a decryptor that uses the same key and IV as the encryptor.                
                using (ICryptoTransform encryptorPass = myRijndael.CreateEncryptor(keyPass, iv))
                {

                    //Encrypt the data.                
                    using (MemoryStream msEncryptPass = new MemoryStream())
                    {
                        using (CryptoStream csEncryptPass = new CryptoStream(msEncryptPass, encryptorPass, CryptoStreamMode.Write))
                        {

                            //Convert the data to a byte array.               
                            toEncryptPass = UnicodeConverter.ToByteArray(p);

                            //Write all data to the crypto stream and flush it.              
                            csEncryptPass.Write(toEncryptPass, 0, toEncryptPass.Length);
                            csEncryptPass.FlushFinalBlock();


                            //Get encrypted array of bytes.              
                            byte[] encryptedPass = msEncryptPass.ToArray();


                            return HexConverter.ToHexString(encryptedPass);
                        }
                    }
                }
            }
		}

		/// <summary>
		/// Decrypts the specified p.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <returns></returns>
		public static string Decrypt(string p)
		{
			if (p == null) return null;
			byte[] encrypted = HexConverter.ToByteArray(p);
			byte[] iv = { 218, 123, 211, 4, 145, 147, 132, 228, 121, 39, 222, 60, 158, 10, 229, 62 };
			byte[] key = UnicodeConverter.ToByteArray("LdapPass");
            using (System.Security.Cryptography.RijndaelManaged myRijndael = new System.Security.Cryptography.RijndaelManaged())
            {

                //Get a decryptor that uses the same key and IV as the encryptor.
                using (System.Security.Cryptography.ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, iv))
                {

                    //Now decrypt the previously encrypted message using the decryptor
                    // obtained in the above step.
                    using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                    {
                        using (System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            byte[] fromEncrypt = new byte[encrypted.Length];
                            int numRead = csDecrypt.Read(fromEncrypt, 0, (int)encrypted.Length);

                            byte[] targetDecrypt = new byte[numRead];
                            Array.Copy(fromEncrypt, 0, targetDecrypt, 0, numRead);
                            return UnicodeConverter.ToUnicodeString(targetDecrypt);
                        }
                    }
                }
            }
		}

		public static bool IsEncrypted(string p)
		{
			try
			{
				p = Decrypt(p);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

	}
}
