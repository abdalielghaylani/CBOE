using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
//
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Net;

namespace Util
{
    /// <summary>
    /// Summary description for XMLUtil
    /// </summary>
    public class XMLUtil
    {
        public XMLUtil()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }
    }
}
