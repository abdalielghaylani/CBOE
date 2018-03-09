using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
	class CSBR132544: BugFixBaseCommand
	{
        /// <summary>
        ///Patcher to update COEFrameworkConfig.xml. Fix to CSBR132544 and  CSBR143431 issues  by disallowing special charcters in sequence table editor screen
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="dataviews"></param>
        /// <param name="configurations"></param>
        /// <param name="objectConfig"></param>
        /// <param name="frameworkConfig"></param>
        /// <returns></returns>
        
        public override List<string> Fix(List<XmlDocument> forms, List<XmlDocument> dataviews, List<XmlDocument> configurations, XmlDocument objectConfig, XmlDocument frameworkConfig)
        {
            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            # region Prefix Custom validation 
            XmlNode prefixValidation = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='PREFIX']/validationRule");

            if (prefixValidation != null)
            {
                if (prefixValidation.SelectSingleNode("add[@name='custom']") == null)
                {
                    XmlNode prefixCusValidation = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    XmlAttribute errmsg = frameworkConfig.CreateAttribute("errorMessage");
                    name.Value = "custom";
                    errmsg.Value = "The characters &lt;,&gt;,&amp;,&quot;,?,+,=,',#,%  are not allowed as a prefix";
                    prefixCusValidation.Attributes.Append(name);
                    prefixCusValidation.Attributes.Append(errmsg);
                    prefixCusValidation.InnerXml = @"
                    <parameter>
                        <add name=""clientScript"" value=""var patt=new RegExp(&quot;[&lt;&gt;,&amp;,\&quot;,\',?,+,=,#,%]&quot;);arguments.IsValid =!patt.test(document.getElementById(source.controltovalidate).value);""></add>
                    </parameter>
                    ";
                    prefixValidation.AppendChild(prefixCusValidation);
                    messages.Add("Custom validation added for Prefix on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Custom validation already present for Prefix on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Prefix validations not available on sequence table editor");
            }

            #endregion

            # region Prefix delimiter Custom validation
            XmlNode prefixdelimValidation = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='PREFIXDELIMITER']/validationRule");

            if (prefixdelimValidation != null)
            {
                if (prefixdelimValidation.SelectSingleNode("add[@name='custom']") == null)
                {
                    XmlNode prefixDelimCusValidation = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    XmlAttribute errmsg = frameworkConfig.CreateAttribute("errorMessage");
                    name.Value = "custom";
                    errmsg.Value = "The characters &lt;,&gt;,&amp;,&quot;,?,+,=,',#,%  are not allowed as a prefix delimiter";
                    prefixDelimCusValidation.Attributes.Append(name);
                    prefixDelimCusValidation.Attributes.Append(errmsg);
                    prefixDelimCusValidation.InnerXml = @"
                    <parameter>
                        <add name=""clientScript"" value=""var patt=new RegExp(&quot;[&lt;&gt;,&amp;,\&quot;,\',?,+,=,#,%]&quot;);arguments.IsValid =!patt.test(document.getElementById(source.controltovalidate).value);""></add>
                    </parameter>
                    ";
                    prefixdelimValidation.AppendChild(prefixDelimCusValidation);
                    messages.Add("Custom validation added for Prefix delimiter on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Custom validation already present for Prefix delimiter on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("Prefix delimiter validations not available on sequence table editor");
            }

            #endregion

            # region suffix delimiter Custom validation
            XmlNode suffixdelimValidation = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='SUFFIXDELIMITER']/validationRule");

            if (suffixdelimValidation != null)
            {
                if (suffixdelimValidation.SelectSingleNode("add[@name='custom']") == null)
                {
                    XmlNode suffixDelimCusValidation = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    XmlAttribute errmsg = frameworkConfig.CreateAttribute("errorMessage");
                    name.Value = "custom";
                    errmsg.Value = "The characters &lt;,&gt;,&amp;,&quot;,?,+,=,',#,%  are not allowed as a suffix delimiter";
                    suffixDelimCusValidation.Attributes.Append(name);
                    suffixDelimCusValidation.Attributes.Append(errmsg);
                    suffixDelimCusValidation.InnerXml = @"
                    <parameter>
                        <add name=""clientScript"" value=""var patt=new RegExp(&quot;[&lt;&gt;,&amp;,\&quot;,\',?,+,=,#,%]&quot;);arguments.IsValid =!patt.test(document.getElementById(source.controltovalidate).value);""></add>
                    </parameter>
                    ";
                    suffixdelimValidation.AppendChild(suffixDelimCusValidation);
                    messages.Add("Custom validation added for suffix delimiter on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Custom validation already present for suffix delimiter on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("suffix delimiter validations not available on sequence table editor");
            }

            #endregion

            # region batch delimiter Custom validation
            XmlNode batchdelimValidation = frameworkConfig.SelectSingleNode("//coeConfiguration/applications/add[@name='REGISTRATION']/tableEditor/add[@name='VW_SEQUENCE']/tableEditorData/add[@name='BATCHDELIMITER']/validationRule");

            if (batchdelimValidation != null)
            {
                if (batchdelimValidation.SelectSingleNode("add[@name='custom']") == null)
                {
                    XmlNode batchDelimCusValidation = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    XmlAttribute errmsg = frameworkConfig.CreateAttribute("errorMessage");
                    name.Value = "custom";
                    errmsg.Value = "The characters &lt;,&gt;,&amp;,&quot;,?,+,=,',#,%  are not allowed as a batch delimiter";
                    batchDelimCusValidation.Attributes.Append(name);
                    batchDelimCusValidation.Attributes.Append(errmsg);
                    batchDelimCusValidation.InnerXml = @"
                    <parameter>
                        <add name=""clientScript"" value=""var patt=new RegExp(&quot;[&lt;&gt;,&amp;,\&quot;,\',?,+,=,#,%]&quot;);arguments.IsValid =!patt.test(document.getElementById(source.controltovalidate).value);""></add>
                    </parameter>
                    ";
                    batchdelimValidation.AppendChild(batchDelimCusValidation);
                    messages.Add("Custom validation added for batch delimiter on sequence table");
                }
                else
                {
                    errorsInPatch = true;
                    messages.Add("Custom validation already present for batch delimiter on sequence table");
                }
            }
            else
            {
                errorsInPatch = true;
                messages.Add("batch delimiter validations not available on sequence table editor");
            }

            #endregion
            
             
            if (!errorsInPatch && messages.Count != 0)
            {
                messages.Add("process succeed!");
            }
            else
            {
                messages.Add("failed to patch.");
            }

            return messages;
        }
	}
}
