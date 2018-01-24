using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Patcher
{
    public class CSBR133423 : BugFixBaseCommand
	{
        public override List<string> Fix(
            List<XmlDocument> forms
            , List<XmlDocument> dataviews
            , List<XmlDocument> configurations
            , XmlDocument objectConfig
            , XmlDocument frameworkConfig
            )
        {
            /* The goal here is to alter an existing add-in's configuration to match a fundamentally
             * differnt implementation.
             * 
                <AddIn assembly="CambridgeSoft.COE.Registration.RegistrationAddins, Version=13.0.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc" class="CambridgeSoft.COE.Registration.Services.RegistrationAddins.FindDuplicatesAddIn" friendlyName="Find Custom Field Duplicates" required="no" enabled="no">
			        <Event eventName="Registering" eventHandler="OnRegisteringHandler"/>
			        <AddInConfiguration>
				        <Behavior>CompoundProperty</Behavior>
				        <DataViewID>4008</DataViewID>
				        <PropertyName>CUSTOM_PROPERTY_NAME</PropertyName>
				        <SearchCriteria>COMPOUND.CUSTOM_PROPERTY_NAME equals 'Compound_Property_Value'</SearchCriteria>
				        <ResultFields>COMPOUND.COMPOUNDID|MIXTURE.REGNUMBER</ResultFields>
			        </AddInConfiguration>
		        </AddIn>
             *
             * THis configuration becomes totally obselete, as this functionality's settings
             * cover a separate feature; pre-load matching of existing components.
             * 
             * Any pre-existing configuration should be migrated.
             */

            List<string> messages = new List<string>();
            bool errorsInPatch = false;

            string componentProperty = null;
            int regConfigIndex = -1;
            XmlDocument addinsConfig = new XmlDocument();
            addinsConfig.LoadXml(objectConfig.OuterXml);
            XmlDocument regConfig = null;

            try
            {
                foreach (XmlDocument config in configurations)
                {
                    if (config.SelectSingleNode("//applicationSettings[@name='Reg App Settings']") != null)
                    {
                        regConfig = new XmlDocument();
                        regConfig.LoadXml(config.OuterXml);
                        regConfigIndex = configurations.IndexOf(config);
                        break;
                    }
                }

                bool currentlyEnabled = false;
                string addInConfigXpath = "//AddIn[@class='CambridgeSoft.COE.Registration.Services.RegistrationAddins.FindDuplicatesAddIn']";

                XmlNode addInNode = addinsConfig.SelectSingleNode(addInConfigXpath);
                if (addInNode != null)
                {
                    XmlNode addInConfiguration = addInNode.SelectSingleNode("AddInConfiguration");
                    if (addInConfiguration != null)
                    {
                        XmlNode addInBehaviorNode = addInConfiguration.SelectSingleNode("Behavior");
                        if (addInBehaviorNode != null)
                        {
                            //only propagate a setting at the Component level
                            if (addInBehaviorNode.InnerXml == "CompoundProperty")
                            {
                                //extract any pre-existing component property name...
                                XmlNode addInPropertyNode = addInConfiguration.SelectSingleNode("PropertyName");
                                if (addInPropertyNode != null)
                                {
                                    //...bypassing if the component property is empty or the 'default' value
                                    string text = addInPropertyNode.InnerXml;
                                    if (!string.IsNullOrEmpty(text) && !text.Equals("Compound_Property_Value"))
                                        componentProperty = text;
                                }
                            }
                        }
                    }

                    //always wipe out existing settings as they are no longer used
                    if (addInConfiguration != null)
                        addInConfiguration.InnerXml = "";
                }
                //Coverity fix - CID 19418
                if (!string.IsNullOrEmpty(componentProperty) && regConfig != null)
                {
                    //apply any pre-configured component property to the default settings
                    string settingValueXpath = "//add[@name='ENHANCED_DUPLICATE_SCAN']/settings/add[@name='Value']";
                    XmlNode settingValueNode = regConfig.SelectSingleNode(settingValueXpath);
                    if (settingValueNode != null)
                    {
                        settingValueNode.Attributes["value"].Value = componentProperty;
                        messages.Add(
                            string.Format("Compound-level property '{0}' was migrated to the system settings for no-structure matching.", componentProperty)
                        );
                    }
                    else
                        messages.Add("No pre-existing compound-level property setting was found.");
                }
                else
                    messages.Add("No pre-existing compound-level property setting was found.");

            }
            catch (Exception ex)
            {
                errorsInPatch = true;
                messages.Add(ex.Message);
            }
            finally
            {
                if (!errorsInPatch && regConfig != null)
                {
                    //apply the changes for these XMLs
                    objectConfig = addinsConfig;
                    configurations[regConfigIndex] = regConfig;
                    /* for debugging only
                    string xpath = "//add[@name='ENHANCED_DUPLICATE_SCAN']/settings/add[@name='Value']/@value";
                    XmlNode n = configurations[regConfigIndex].SelectSingleNode(xpath);
                    string val = n.Value; */

                    messages.Add("CSBR-133423 was successfully patched");
                }
                else
                    messages.Add("CSBR-133423 patch was aborted due to errors");
            }

            return messages;
        }
    }
}
