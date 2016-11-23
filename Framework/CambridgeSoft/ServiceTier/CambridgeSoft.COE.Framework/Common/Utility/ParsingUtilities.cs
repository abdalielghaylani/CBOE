using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CambridgeSoft.COE.Framework.Common
{
    public class ParsingUtilities
    {
        /// <summary>
        /// Parses a list of items based on the type.
        /// If type is txt then the separator between items is the end of line, if csv the separator is a comma and if sdf its $$$$
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileType">Allowed types are txt | csv | sdf</param>
        /// <returns>The item list contained in the stream</returns>
        public static List<string> ParseList(System.IO.Stream stream, string fileType)
        {
            List<string> list = new List<string>();
            using (StreamReader sr = new StreamReader(stream))
            {
                if (fileType.ToLower() == "txt")
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            list.Add(line);
                        }
                    }
                }
                else if (fileType.ToLower() == "sdf")
                {
                    string[] mols = sr.ReadToEnd().TrimEnd(null).Split(new string[] { "$$$$\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < mols.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(mols[i]))
                        {
                            Int32 indexMEND = mols[i].IndexOf("M  END");

                            if (indexMEND > 0)
                            {
                                mols[i] = mols[i].Substring(0, indexMEND + 6).TrimEnd();
                            }
                            list.Add(System.Web.HttpUtility.HtmlEncode(mols[i]));
                        }
                    }
                }
                else if (fileType.ToLower() == "csv")
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] values = line.Split(',');
                            list.AddRange(values);
                        }
                    }
                }
            }
            return list;
        }
    }
}
