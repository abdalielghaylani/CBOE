using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CambridgeSoft.COE.Security.Services.Utlities
{
	/// <summary>
	/// Static methods for converting between a hex string and a byte array.
	/// </summary>
	public static class HexConverter
	{
		/// <summary>
		/// Converts the specified byte array to a string of hex characters representing the array
		/// </summary>
		/// <param name="value">An array of bytes to be converted</param>
		/// <returns>A string of hexidecimal characters</returns>
		public static string ToHexString(byte[] value)
		{
			string s = "";
			string h;

			// The URI HexEscape method prepends a '%' before the hexidecimal character.
			foreach (byte b in value)
			{
				h = Uri.HexEscape((char)b);
				s = s + h.Substring(h.Length - 2);
			}
			return s;
		}

		/// <summary>
		/// Convert the specified string of hex characters into an array of bytes.
		/// </summary>
		/// <param name="value">A string of two character hexadecimal numbers</param>
		/// <returns>An array of bytes</returns>
		public static byte[] ToByteArray(string value)
		{
			byte[] bArray = new byte[value.Length / 2];
			char[] c = value.ToCharArray();
			int i;
			int j;

			for (i = 0, j = 0; i < value.Length; ++j)
			{
				bArray[j] = (byte)(Uri.FromHex(c[i++]) * 16);
				bArray[j] += (byte)Uri.FromHex(c[i++]);
			}

			return bArray;
		}
	}
}
