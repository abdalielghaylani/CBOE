using System.IO;    // MemoryStream
using System.Text;  // Encoding
using System.Xml;   // Xml...

namespace CambridgeSoft.COE.ConfigLoader
{
    /// <summary>
    /// Allows easy use of <c>XmlTextWriter</c> to get the resulting XML as a string rather than writing to a file
    /// </summary>
    public class COEXmlTextWriter : XmlTextWriter
    {
        // Constructor
        public COEXmlTextWriter()
            : base(new MemoryStream(), Encoding.UTF8)
        {
            return;
        }
        
        //Properties
        public string XmlString
        {
            get
            {
                Flush();
                return MemoryStreamGetString((MemoryStream)BaseStream);
            }
        }

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
        }

    }
}
