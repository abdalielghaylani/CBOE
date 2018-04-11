using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR – 128771: 
    /// On the mixture duplicate resoluton page, the NO,MW,MF are null
    /// 
    /// Also reported in the same bug, in note #9:
    /// On the component duplicate resolution page, the MF and MW can display now, there are 2 new issues:
    /// 1.Should change the 'Compund' to 'Compound'
    /// 2.For the identifier part, the combox and textbox display unexpectedly
    /// </summary>
    public class CSBR128771 : BugFixBaseCommand
    {
        /// <summary>
        /// These were the manual steps to fix:
        /// On FormGroup 4013:
        /// 
        /// For the original bug:
        /// 1.	Search for &lt;formElement name="RegNum"&gt; (There is only one entry)
        /// 2.	Modify &lt;label&gt;Registry Number&lt;/label&gt; with  &lt;label&gt;Compound Id&lt;/label&gt;
        /// 
        /// For the bug reported in note #9:
        /// 1.	Search for the second COEForm id="0" entry
        /// 2.	Go to &lt;formDisplay&gt; node and replace the following xml:
        /// 
        /// &lt;height&gt;400px&lt;/height&gt;
        /// &lt;width&gt;730px&lt;/width&gt;
        /// 
        /// with:
        /// 
        /// &lt;layoutStyle&gt;flowLayout&lt;/layoutStyle&gt;
        /// 
        /// 3.	Search for ComponentRepeater  (there is only one entry) and remove the following xml:
        /// 
        /// &lt;top&gt; 0px&lt;/top&gt;
        /// &lt;left&gt;0px&lt;/left&gt;
        /// 
        /// 4.	In the same formElement go to &lt;configInfo&gt; &lt;fieldConfig&gt; &lt;coeForm id="1"&gt; &lt;formDisplay&gt; node and replace the following xml:
        /// 
        /// &lt;height&gt;300px&lt;/height&gt;
        /// &lt;width&gt;700px&lt;/width&gt;
        /// 
        /// with:
        /// 
        /// &lt;layoutStyle&gt;flowLayout&lt;/layoutStyle&gt;
        /// 
        /// 5.	Search for &lt;formElement name="RegNum"&gt; (there is only one entry) and remove the following xml:
        /// 
        /// &lt;top&gt;40px&lt;/top&gt;
        /// &lt;left&gt;250px&lt;/left&gt;
        ///      
        /// 6.	Search the next formElement &lt;formElement name="MF"&gt; ? &lt;displayInfo&gt; and remove the following xml:
        /// 
        /// &lt;top&gt;90px&lt;/top&gt;
        /// &lt;left&gt;250px&lt;/left&gt;
        /// 
        /// 7.	Search  the next formElement &lt;formElement name="MW"&gt; ? &lt;displayInfo&gt; and remove the following xml:
        /// 
        /// &lt;top&gt;140px&lt;/top&gt;
        /// &lt;left&gt;250px&lt;/left&gt;
        /// 
        /// 8.	Search the next formElement &lt;formElement name="Identifiers"&gt; ? &lt;displayInfo&gt; and remove the following xml:
        /// 
        /// &lt;top&gt;40px&lt;/top&gt;
        /// &lt;left&gt;450px&lt;/left&gt;
        /// 
        /// 9.	Search the next formElement (doesn´t have name but his id is “ResolveLinkButton") 	&lt;formElement&gt; ? &lt;displayInfo&gt; and replace the following xml:
        /// 
        /// &lt;top&gt;250px&lt;/top&gt;
        /// &lt;left&gt;250px&lt;/left&gt;
        /// 
        /// with:
        /// 
        /// &lt;style&gt;clear:both; margin-left:290px; width:100%&lt;/style&gt;
        /// 
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
            foreach(XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                if(id == "4013")
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("COE", "COE.FormGroup");
                    XmlNode formElementRegNum = doc.SelectSingleNode("//COE:formElement[@name='RegNum']", manager);
                    if(formElementRegNum == null)
                    {
                        messages.Add("There as no formElement with name RegNum");
                        errorsInPatch = true;
                    }
                    else
                    {
                        XmlNode label = formElementRegNum.SelectSingleNode("./COE:label", manager);
                        if(label == null || label.InnerText != "Registry Number")
                        {
                            messages.Add("The formElement with name RegNum has an unexpected label and thus was not modified");
                            errorsInPatch = true;
                        }
                        else
                        {
                            label.InnerText = "Compound Id";
                        }

                        XmlNode regNumDisplayInfo = formElementRegNum.SelectSingleNode("./COE:displayInfo", manager);
                        if(regNumDisplayInfo != null)
                        {
                            XmlNode top = regNumDisplayInfo.SelectSingleNode("./COE:top", manager);
                            XmlNode left = regNumDisplayInfo.SelectSingleNode("./COE:left", manager);
                            if(top != null)
                            {
                                regNumDisplayInfo.RemoveChild(top);
                                messages.Add("top removed from regnum's displayInfo");
                            }
                            if(left != null)
                            {
                                regNumDisplayInfo.RemoveChild(left);
                                messages.Add("left removed from regnum's displayInfo");
                            }
                        }
                    }

                    XmlNode coeForm = doc.SelectSingleNode("//COE:listForm[@id='0']/COE:coeForms/COE:coeForm[@id='0']", manager);
                    if(coeForm != null)
                    {
                        XmlNode formDisplay = coeForm.SelectSingleNode("./COE:formDisplay", manager);
                        if(formDisplay != null)
                        {
                            XmlNode height = formDisplay.SelectSingleNode("./COE:height", manager);
                            XmlNode width = formDisplay.SelectSingleNode("./COE:width", manager);
                            XmlNode layoutStyle = formDisplay.SelectSingleNode("./COE:layoutStyle", manager);
                            if(layoutStyle == null)
                            {
                                layoutStyle = doc.CreateNode(XmlNodeType.Element, "layoutStyle", formDisplay.NamespaceURI);
                                formDisplay.AppendChild(layoutStyle);
                                messages.Add("layoutStyle added to formDisplay");
                            }

                            layoutStyle.InnerText = "flowLayout";

                            if(height != null)
                            {
                                formDisplay.RemoveChild(height);
                                messages.Add("height removed from formDisplay");
                            }
                            if(width != null)
                            {
                                formDisplay.RemoveChild(width);
                                messages.Add("width removed from formDisplay");
                            }
                        }
                        else
                        {
                            messages.Add("formDisplay was not found in list form 0, coeform 0");
                            errorsInPatch = true;
                        }

                        XmlNode repeaterDisplayInfo = doc.SelectSingleNode("//COE:type[text()='CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEFormGeneratorRepeater']/..", manager);
                        if (repeaterDisplayInfo != null)
                        {
                            XmlNode top = repeaterDisplayInfo.SelectSingleNode("./COE:top", manager);
                            XmlNode left = repeaterDisplayInfo.SelectSingleNode("./COE:left", manager);
                            if (top != null)
                            {
                                repeaterDisplayInfo.RemoveChild(top);
                                messages.Add("top removed from reapeter's displayInfo");
                            }
                            if (left != null)
                            {
                                repeaterDisplayInfo.RemoveChild(left);
                                messages.Add("left removed from reapeter's displayInfo");
                            }

                            XmlNode coeFormInsideRepeater = repeaterDisplayInfo.SelectSingleNode("../COE:configInfo/COE:fieldConfig/COE:coeForm[@id='1']", manager);
                            if (coeFormInsideRepeater != null)
                            {
                                XmlNode formDisplayInRepeater = coeFormInsideRepeater.SelectSingleNode("./COE:formDisplay", manager);
                                if (formDisplayInRepeater != null)
                                {
                                    XmlNode height = formDisplayInRepeater.SelectSingleNode("./COE:height", manager);
                                    XmlNode width = formDisplayInRepeater.SelectSingleNode("./COE:width", manager);
                                    XmlNode layoutStyle = formDisplayInRepeater.SelectSingleNode("./COE:layoutStyle", manager);
                                    if (layoutStyle == null)
                                    {
                                        layoutStyle = doc.CreateNode(XmlNodeType.Element, "layoutStyle", formDisplayInRepeater.NamespaceURI);
                                        formDisplayInRepeater.AppendChild(layoutStyle);
                                        messages.Add("layoutStyle added to formDisplay inside repeater");
                                    }

                                    layoutStyle.InnerText = "flowLayout";

                                    if (height != null)
                                    {
                                        formDisplayInRepeater.RemoveChild(height);
                                        messages.Add("height removed from formDisplay inside repeater");
                                    }
                                    if (width != null)
                                    {
                                        formDisplayInRepeater.RemoveChild(width);
                                        messages.Add("width removed from formDisplay inside repeater");
                                    }
                                }
                                XmlNode mfDisplayInfo = coeFormInsideRepeater.SelectSingleNode("./COE:viewMode/COE:formElement[@name='MF']/COE:displayInfo", manager);
                                if (mfDisplayInfo != null)
                                {
                                    XmlNode top1 = mfDisplayInfo.SelectSingleNode("./COE:top", manager);
                                    XmlNode left1 = mfDisplayInfo.SelectSingleNode("./COE:left", manager);
                                    if (top1 != null)
                                    {
                                        mfDisplayInfo.RemoveChild(top1);
                                        messages.Add("top removed from MF's displayInfo");
                                    }
                                    if (left1 != null)
                                    {
                                        mfDisplayInfo.RemoveChild(left1);
                                        messages.Add("left removed from MF's displayInfo");
                                    }
                                }
                                XmlNode mwDisplayInfo = coeFormInsideRepeater.SelectSingleNode("./COE:viewMode/COE:formElement[@name='MW']/COE:displayInfo", manager);
                                if (mwDisplayInfo != null)
                                {
                                    XmlNode top2 = mwDisplayInfo.SelectSingleNode("./COE:top", manager);
                                    XmlNode left2 = mwDisplayInfo.SelectSingleNode("./COE:left", manager);
                                    if (top2 != null)
                                    {
                                        mwDisplayInfo.RemoveChild(top2);
                                        messages.Add("top removed from MW's displayInfo");
                                    }
                                    if (left2 != null)
                                    {
                                        mwDisplayInfo.RemoveChild(left2);
                                        messages.Add("left removed from MW's displayInfo");
                                    }
                                }
                                XmlNode identifiersDisplayInfo = coeFormInsideRepeater.SelectSingleNode("./COE:viewMode/COE:formElement[@name='Identifiers']/COE:displayInfo", manager);
                                if (identifiersDisplayInfo != null)
                                {
                                    XmlNode top3 = identifiersDisplayInfo.SelectSingleNode("./COE:top", manager);
                                    XmlNode left3 = identifiersDisplayInfo.SelectSingleNode("./COE:left", manager);
                                    if (top3 != null)
                                    {
                                        identifiersDisplayInfo.RemoveChild(top3);
                                        messages.Add("top removed from Identifiers' displayInfo");
                                    }
                                    if (left3 != null)
                                    {
                                        identifiersDisplayInfo.RemoveChild(left3);
                                        messages.Add("left removed from Identifiers' displayInfo");
                                    }
                                }
                                XmlNode resolveLinkDisplayInfo = doc.SelectSingleNode("//COE:Id[text()='ResolveLinkButton']/../COE:displayInfo", manager);
                                if (resolveLinkDisplayInfo != null)
                                {
                                    XmlNode top4 = resolveLinkDisplayInfo.SelectSingleNode("./COE:top", manager);
                                    XmlNode left4 = resolveLinkDisplayInfo.SelectSingleNode("./COE:left", manager);
                                    XmlNode style = resolveLinkDisplayInfo.SelectSingleNode("./COE:style", manager);
                                    if (style == null)
                                    {
                                        style = doc.CreateNode(XmlNodeType.Element, "style", resolveLinkDisplayInfo.NamespaceURI);
                                        resolveLinkDisplayInfo.AppendChild(style);
                                        messages.Add("style added to formDisplay inside ResolveLinkButton");
                                    }

                                    style.InnerText = "clear:both; margin-left:290px; width:100%";

                                    if (top4 != null)
                                    {
                                        resolveLinkDisplayInfo.RemoveChild(top4);
                                        messages.Add("top removed from ResolveLinkButton's displayInfo");
                                    }
                                    if (left4 != null)
                                    {
                                        resolveLinkDisplayInfo.RemoveChild(left4);
                                        messages.Add("left removed from ResolveLinkButton's displayInfo");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(!errorsInPatch)
                messages.Add("CSBR128771 was successfully patched");
            else
                messages.Add("CSBR128771 was patched with errors");
            return messages;
        }
    }
}
