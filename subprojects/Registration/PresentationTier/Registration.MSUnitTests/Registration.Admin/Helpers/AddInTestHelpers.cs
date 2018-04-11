using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RegistrationAdmin.Services.MSUnitTests.Helpers
{
    class AddInTestHelpers
    {
        public static string GetAttributeValue(XmlNode addInNode, string attributeName)
        {
            if (addInNode != null)
            {
                return addInNode.Attributes[attributeName].Value;
            }
            return string.Empty;
        }
        
    }
}
