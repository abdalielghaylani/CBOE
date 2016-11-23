using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// Bug Description:
    /// The sequences table doesnot contain the NEXTINSEQUENCE field. The NEXTINSEQUENCE field should be set in default configuration.
    /// 
    /// Note: To add the NEXTINSEQUENCE field, we need to follow the steps mentioned in the attached document.
    /// 
    /// DGB Commented:
    /// There is a clear requirement for the administrator to reset the nextinsequence value after a legacy data load.  In the past we have addressed this by making the nextinsequence value editable in table editor, risking all the downside described by Jeff in note 2.
    /// 
    /// Exposing the nextinsequence value via the tableeditor configuration is trivial.  Adding validation rules that would paliate Jeff's concerns is difficult.
    /// 
    /// An alternate solution would be to provide a database procedure that resets the nextinsequence to follow the maxium used value.  Then we would need to provide a GUI element that would call that procedure to reset a given sequence.  We don't currently have a good place to "hang" such a GUI element.  I am therefore inclined to simply expose the nextinsquence value via tableeditor configuration.
    /// </summary>
	public class CSBR127941 : BugFixBaseCommand
	{
        /// <summary>
        /// DGB Exposed nextinsequence in table editor.  Used validators for:  Positive Integer, Required, Leess than 10 digits
        /// 
        /// Manual Fix:
        /// 
        /// Open coeframeworkconfig.xml look for &lt;add name="VW_SEQUENCE" inside the inner node tableEditorData, right after REGNUMBERLENGTH node add
        /// &lt;add name="NEXTINSEQUENCE" dataType="number"&gt;
		///     &lt;validationRule&gt;
		///         &lt;add name="positiveNumber" errorMessage="This field must be a positive number. "/&gt;
        ///     	&lt;add name="textLength" errorMessage="The length must be between 1 and 10. "&gt;
        ///     		&lt;parameter&gt;
		///					&lt;add name="min" value="1"/&gt;
		///				    &lt;add name="max" value="10"/&gt;
		///			    &lt;/parameter&gt;
		///			&lt;/add&gt;
		///		    &lt;add name="requiredField" errorMessage="This field is required"/&gt;
		///	    &lt;/validationRule&gt;
		///	&lt;/add&gt;
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
            string nextInSequenceStr = @"<validationRule>
									<add name=""positiveNumber"" errorMessage=""This field must be a positive number. ""/>
									<add name=""textLength"" errorMessage=""The length must be between 1 and 10. "">
										<parameter>
											<add name=""min"" value=""1""/>
											<add name=""max"" value=""10""/>
										</parameter>
									</add>
									<add name=""requiredField"" errorMessage=""This field is required""/>
								</validationRule>";
            XmlNode node = frameworkConfig.SelectSingleNode("//add[@name='NEXTINSEQUENCE']");
            if (node == null)
            {
                XmlNode regNumberLength = frameworkConfig.SelectSingleNode("//add[@name='VW_SEQUENCE']/tableEditorData/add[@name='REGNUMBERLENGTH']");
                if (regNumberLength == null)
                {
                    messages.Add("REGNUMBERLENGTH was not found");
                    errorsInPatch = true;
                }
                else
                {
                    XmlElement nextInSeq = frameworkConfig.CreateElement("add");
                    XmlAttribute name = frameworkConfig.CreateAttribute("name");
                    name.Value = "NEXTINSEQUENCE";
                    nextInSeq.Attributes.Append(name);
                    XmlAttribute dataType = frameworkConfig.CreateAttribute("dataType");
                    dataType.Value = "number";
                    nextInSeq.Attributes.Append(dataType);
                    nextInSeq.InnerXml = nextInSequenceStr;
                    regNumberLength.ParentNode.InsertAfter(nextInSeq, regNumberLength);
                    messages.Add("NEXTINSEQUENCE added to coeframeworkconfig.xml");
                }
            }
            else
            {
                messages.Add("NEXTINSEQUENCE was already added");
                errorsInPatch = true;
            }
            if (!errorsInPatch)
                messages.Add("CSBR127941 was successfully patched");
            else
                messages.Add("CSBR127941 was patched with errors");
            return messages;
        }
    }
}
