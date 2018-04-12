using System.IO;    // MemoryStream
using System.Text;  // Encoding
using System.Xml;   // Xml...

namespace CambridgeSoft.COE.DataLoader.Common
{
    /// <summary>
    /// Allows easy use of <c>XmlTextWriter</c> to get the resulting XML as a string rather than writing to a file
    /// </summary>
    public class COEXmlTextWriter : XmlTextWriter
    {
        public string XmlString
        {
            get
            {
                Flush();
                return MemoryStreamGetString((MemoryStream)BaseStream);
            }
        }
        // Constructor
        public COEXmlTextWriter() : base(new MemoryStream(), Encoding.UTF8)
        {
            return;
        } // COEXmlTextWriter()
        // Methods
        static private string MemoryStreamGetString(MemoryStream oMemoryStream)
        {
            string xmlRet;
            byte[] byteXml;
            int cBytes;
            // GetBuffer is more effecient than ToArray
            if (oMemoryStream.CanWrite)
            {
                byteXml = oMemoryStream.GetBuffer();
                cBytes = (int)oMemoryStream.Length;
            }
            else
            {
                byteXml = oMemoryStream.ToArray();
                cBytes = byteXml.Length;
            }
            byteXml = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), byteXml, 3, cBytes - 3);
            xmlRet = Encoding.UTF8.GetString(byteXml);
            return xmlRet;
        } // MemoryStreamGetString()
        static public string Pretty(string xmlUgly)
        {
            string xmlPretty = "";
            char[] chOuterXml = xmlUgly.ToCharArray();
            int nIndent = 0;
            bool bBOL = true;
            for (int n = 0; n < xmlUgly.Length; n++)
            {
                char chThis = chOuterXml[n];
                if (chThis == '<')
                {
                    if (bBOL == false)
                    {
                        xmlPretty += '\r'; bBOL = true;
                    }
                    char chNext = chOuterXml[n + 1];
                    if (chNext == '/')
                    {
                        nIndent--;
                        xmlPretty += new string(' ', nIndent); bBOL = false;
                    }
                    else
                    {
                        xmlPretty += new string(' ', nIndent); bBOL = false;
                        nIndent++;
                    }
                }
                else if (chThis == '/')
                {
                    char chNext = chOuterXml[n + 1];
                    if (chNext == '>')
                    {
                        nIndent--;
                    }
                }
                else if (chThis < ' ')
                {
                    continue;
                }
                else if ((chThis == ' ') && bBOL)
                {
                    continue;
                }
                if (bBOL)
                {
                    xmlPretty += new string(' ', nIndent); bBOL = false;
                }
                xmlPretty += chThis;
                if (chThis == '>')
                {
                    xmlPretty += "\r"; bBOL = true;
                }
            } // for (int n = 0; n < xmlUgly.Length; n++)
            return xmlPretty;
        } // Pretty()
        static public string XmlDocumentGetString(XmlDocument oXmlDocument)
        {
            string xmlRet;
            MemoryStream oMemoryStream = new MemoryStream();
            oXmlDocument.Save(oMemoryStream);
            xmlRet = MemoryStreamGetString(oMemoryStream);
            oMemoryStream.Close();
            return xmlRet;
        } // XmlDocumentGetString()
    }
}
