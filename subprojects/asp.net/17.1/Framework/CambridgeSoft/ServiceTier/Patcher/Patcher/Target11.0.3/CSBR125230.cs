using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    /// <summary>
    /// CSBR-125230: The COE:CSSClass of all the properties get lost when you change the CSS class of any property
    /// 
    /// Steps:
    /// 1) Login as t5_85
    /// 2) Go to Customize registration
    /// 3) Go to Form customization
    /// 4) Change the CSS class of any property
    /// 5) Go to submit new record
    /// 
    /// BUG: All the properties lost its COE:CSSClass. If you see on the database the table coedb.coeforms the form 4010, you will see that all the COE:CSSClass are empty. The bug is only reproducible when you modify  the class of a property, not when you create a property.
    /// </summary>
	public class CSBR125230 : BugFixBaseCommand
	{
        /// <summary>
        /// Manual steps to fix:
        /// 
        /// Open customize registration and edit form 4010 (submit)
        /// Look for &lt;cssClass&gt;Std25x40&lt;/cssClass&gt; and replace with &lt;cssClass&gt;Std20x40&lt;/cssClass&gt;.
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

            foreach (XmlDocument doc in forms)
            {
                string id = doc.DocumentElement.Attributes["id"] == null ? string.Empty : doc.DocumentElement.Attributes["id"].Value;
                XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("COE", "COE.FormGroup");
                if (id == "4010")
                {
                    //Prefix
                    XmlNodeList prefixes = doc.SelectNodes("//COE:formElement[@name='Prefix']", manager);
                    if (prefixes.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no prefix in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode prefix in prefixes)
                        {
                            XmlNode cssClass = prefix.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.Trim().ToUpper().Equals("Std20x40".Trim().ToUpper()))
                                {
                                    messages.Add("WARNING: prefix was already patched");
                                }
                                else if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR: prefix did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated prefix css class");
                                }
                            }
                            else
                            {
                                messages.Add("WARNING: prefix did not contain cssClass");
                            }
                        }
                    }

                    //Component ID
                    XmlNodeList componentIds = doc.SelectNodes("//COE:formElement[@name='Component ID']", manager);
                    if (componentIds.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no Component ID in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode componentId in componentIds)
                        {
                            XmlNode cssClass = componentId.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.ToUpper().Trim().Equals("Std20x40".ToUpper().Trim()))
                                {
                                    messages.Add("WARNING: Component ID was already patched");
                                }
                                if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR: Component ID did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated Component ID css class");
                                }
                            }
                            else 
                            {
                                messages.Add("ERROR: Successfuly updated Component ID css class");
                                errorsInPatch = true;
                            }
                        }
                    }

                    //Formula
                    XmlNodeList formulas = doc.SelectNodes("//COE:formElement[@name='Formula']", manager);
                    if (formulas.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no Formula in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode formula in formulas)
                        {
                            XmlNode cssClass = formula.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.ToUpper().Trim().Equals("Std20x40".ToUpper().Trim()))
                                {
                                    messages.Add("WARNING: Formula was already patched");
                                }
                                else if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR: Formula did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated Formula css class");
                                }
                            }
                            else
                            {
                                messages.Add("ERROR: formula does not contain cssClass");
                                errorsInPatch = false;
                            }
                        }
                    }

                    //MW
                    XmlNodeList molWeights = doc.SelectNodes("//COE:formElement[@name='MW']", manager);
                    if (molWeights.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no MW in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode molWeight in molWeights)
                        {
                            XmlNode cssClass = molWeight.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.ToUpper().Trim().Equals("Std20x40".ToUpper().Trim()))
                                {
                                    messages.Add("WARNING: MW was already patched");
                                }
                                else if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR:MW did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated MW css class");
                                }
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ERROR: MW did not contain cssClass");
                            }
                        }
                    }

                    //FORMULA_WEIGHT
                    XmlNodeList formulaWeights = doc.SelectNodes("//COE:formElement[@name='FORMULA_WEIGHT']", manager);
                    if (formulaWeights.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no FORMULA_WEIGHT in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode formulaWeight in formulaWeights)
                        {
                            XmlNode cssClass = formulaWeight.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.ToUpper().Trim().Equals("Std20x40".ToUpper().Trim()))
                                {
                                    messages.Add("WARNING: FORMULA_WEIGHT was already patched");
                                }
                                else if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR: FORMULA_WEIGHT did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated FORMULA_WEIGHT css class");
                                }
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ERROR: FORMULA_WEIGHT was not patched");
                            }
                        }
                    }

                    //BATCH_FORMULA
                    XmlNodeList batchFormulas = doc.SelectNodes("//COE:formElement[@name='BATCH_FORMULA']", manager);
                    if (batchFormulas.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no BATCH_FORMULA in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode batchFormula in batchFormulas)
                        {
                            XmlNode cssClass = batchFormula.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.ToUpper().Trim().Equals("Std20x40".ToUpper().Trim()))
                                {
                                    messages.Add("WARNING: BATCH_FORMULA was already patched");
                                }
                                else if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR: BATCH_FORMULA did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated BATCH_FORMULA css class");
                                }
                            }
                            else
                            {
                                messages.Add("ERROR: BATCH_FORMULA did not have cssClass");
                                errorsInPatch = true;
                            }
                        }
                    }

                    //PERCENT_ACTIVE
                    XmlNodeList percentActives = doc.SelectNodes("//COE:formElement[@name='PERCENT_ACTIVE']", manager);
                    if (percentActives.Count < 1)
                    {
                        errorsInPatch = true;
                        messages.Add("There was no PERCENT_ACTIVE in coeform 4010 to be modified");
                    }
                    else
                    {
                        foreach (XmlNode percentActive in percentActives)
                        {
                            XmlNode cssClass = percentActive.SelectSingleNode("./COE:displayInfo/COE:cssClass", manager);
                            if (cssClass != null)
                            {
                                if (cssClass.InnerText.ToUpper().Trim().Equals("Std20x40".ToUpper().Trim()))
                                {
                                    messages.Add("WARNING: PERCENT_ACTIVE was already patched");
                                }
                                else if (cssClass.InnerText != "Std25x40")
                                {
                                    errorsInPatch = true;
                                    messages.Add("ERROR: PERCENT_ACTIVE did not have the expected value and was not modified");
                                }
                                else
                                {
                                    cssClass.InnerText = "Std20x40";
                                    messages.Add("Successfuly updated PERCENT_ACTIVE css class");
                                }
                            }
                            else
                            {
                                errorsInPatch = true;
                                messages.Add("ERROR: PERCENT_ACTIVE did not have cssClass");
                            }
                        }
                    }
                }
            }
            if (!errorsInPatch)
            {
                messages.Add("CSBR125230 was successfully patched");
            }
            else
                messages.Add("CSBR125230 was patched with errors");
            return messages;
        }
    }
}
