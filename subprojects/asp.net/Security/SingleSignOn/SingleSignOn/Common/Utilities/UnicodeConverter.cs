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
	/// Static methods for converting between a unicode string and a byte array.
	/// </summary>
	public static class UnicodeConverter
	{
		/// <summary>
		/// Converts the specified byte array to a string of hex characters representing the array
		/// </summary>
		/// <param name="value">An array of bytes to be converted</param>
		/// <returns>A string of unicode characters</returns>
		public static string ToUnicodeString(byte[] value)
		{
			string s = "";
			short i;

			// The URI HexEscape method prepends a '%' before the hexidecimal character.
			i = -1;
			foreach (byte b in value)
			{
				if (i == -1)
					i = (short)(b * 256);
				else
				{
					i += (short)b;
					s = s + (char)i;
					i = -1;
				}
			}

			return s;
		}

		/// <summary>
		/// Converts the specified string of hex characters into an array of bytes.
		/// </summary>
		/// <param name="value">A string of two character hexadecimal numbers</param>
		/// <returns>An array of bytes</returns>
		public static byte[] ToByteArray(string value)
		{
			byte[] bArray = new byte[value.Length * 2];
			char[] c = value.ToCharArray();
			int i;
			int j;

			for (i = 0, j = 0; i < value.Length; i++, j += 2)
			{
				bArray[j] = (byte)((int)c[i] / 256);
				bArray[j + 1] = (byte)((int)c[i] & 255);
			}

			return bArray;
		}
	}
}
