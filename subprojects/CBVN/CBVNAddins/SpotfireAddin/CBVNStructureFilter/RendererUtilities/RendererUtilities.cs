using System;
using System.Reflection;
using Spotfire.Dxp.Framework.DocumentModel;
using Spotfire.Dxp.Framework.Preferences;
using Spotfire.Dxp.Framework.ApplicationModel;
using System.Collections.Generic;
using CBVNStructureFilterSupport.ChemDraw;

namespace CBVNStructureFilter
{
    internal class RendererUtilities
    {
        /// <summary>
        /// Get the global settings
        /// </summary>
        /// <param name="context">INodeContext from which services can be got</param>
        /// <param name="needMatchContentType">needMatchContentType</param>
        /// <param name="contentType">contentType</param>
        /// <param name="typeIdentifier">typeIdentifier</param>
        /// <returns>return the model which matches the contentType and typeIdentifier</returns>
        public static Dictionary<string, StructureColumnRendererModel> GetGlobalRenderSettings(INodeContext context, bool needMatchContentType, string contentType, TypeIdentifier typeIdentifier)
        {
            Dictionary<string, StructureColumnRendererModel> dictionary = new Dictionary<string, StructureColumnRendererModel>();
            StructureColumnRendererModel model = null;

            if (context != null)
            {
                PreferenceManager preferenceManager = context.GetService<PreferenceManager>();

                ApplicationThread applicationThread = context.GetService<ApplicationThread>();

                applicationThread.Invoke(delegate
                {
                    try
                    {
                        Assembly assembly = Assembly.GetAssembly(typeof(Spotfire.Dxp.Application.Document));
                        if (assembly != null)
                        {
                            Type type = assembly.GetType("Spotfire.Dxp.Application.ApplicationPreferences");
                            if (type != null)
                            {
                                MethodInfo getPreferenceMethod = preferenceManager.GetType().GetMethod("GetPreference");
                                if (getPreferenceMethod != null)
                                {
                                    getPreferenceMethod = getPreferenceMethod.MakeGenericMethod(type);
                                    if (getPreferenceMethod != null)
                                    {
                                        object preferenceValue = getPreferenceMethod.Invoke(preferenceManager, null);
                                        if (preferenceValue != null)
                                        {
                                            PropertyInfo renderersProperty = preferenceValue.GetType().GetProperty("DefaultValueRenderers");
                                            if (renderersProperty != null)
                                            {
                                                object renderersValue = renderersProperty.GetValue(preferenceValue, null);
                                                if (renderersValue != null)
                                                {
                                                    PropertyInfo countProperty = renderersValue.GetType().GetProperty("Count");
                                                    PropertyInfo itemProperty = renderersValue.GetType().GetProperty("Item");
                                                    if (countProperty != null && itemProperty != null)
                                                    {
                                                        int? countValue = countProperty.GetValue(renderersValue, null) as int?;
                                                        if (countValue != null)
                                                        {
                                                            for (int i = 0; i < countValue.Value; i++)
                                                            {
                                                                object rendererValue = itemProperty.GetValue(renderersValue, new object[] { i });
                                                                if (rendererValue != null)
                                                                {
                                                                    PropertyInfo contentTypeProperty = rendererValue.GetType().GetProperty("ContentType");
                                                                    PropertyInfo settingsProperty = rendererValue.GetType().GetProperty("Settings");
                                                                    PropertyInfo typeIdentifierProperty = rendererValue.GetType().GetProperty("TypeIdentifier");
                                                                    if (contentTypeProperty != null && settingsProperty != null && typeIdentifierProperty != null)
                                                                    {
                                                                        string contentTypeValue = contentTypeProperty.GetValue(rendererValue, null) as string;
                                                                        TypeIdentifier typeIdentifierValue = typeIdentifierProperty.GetValue(rendererValue, null) as TypeIdentifier;
                                                                        if (contentTypeValue != null && typeIdentifier != null)
                                                                        {
                                                                            if ((!needMatchContentType || contentTypeValue.Equals(contentType)) && typeIdentifierValue.Equals(typeIdentifier))
                                                                            {
                                                                                object theobject = settingsProperty.GetValue(rendererValue, null);
                                                                                model = GetDataFromLDStructureColumnRendererModelClass(theobject);
                                                                                dictionary.Add(contentTypeValue, model);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                });
            }

            return dictionary;
        }

        /// <summary>
        /// Get the merged rendering settings
        /// </summary>
        /// <param name="context">INodeContext from which services can be got</param>
        /// <param name="contentType">contentType</param>
        /// <param name="typeIdentifier">TypeIdentifier</param>
        /// <param name="customizedRenderSettings">customized settings</param>
        /// <returns>return customized settings if it is not default, return global settings otherwise</returns>
        public static string GetNeededRenderSettings(INodeContext context, string contentType, TypeIdentifier typeIdentifier, string customizedRenderSettings)
        {
            if (string.IsNullOrEmpty(customizedRenderSettings) || customizedRenderSettings.Equals(ChemDrawUtilities.DefaultRenderSettingsString))
            {
                if (contentType != null)
                {
                    Dictionary<string, StructureColumnRendererModel> dictionary = RendererUtilities.GetGlobalRenderSettings(context, true, contentType, typeIdentifier);
                    if (dictionary != null && dictionary.ContainsKey(contentType))
                    {
                        StructureColumnRendererModel globalModel = dictionary[contentType];
                        if (globalModel != null)
                        {
                            customizedRenderSettings = globalModel.ChemDrawRenderSettings;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(customizedRenderSettings))
            {
                customizedRenderSettings = ChemDrawUtilities.DefaultRenderSettingsString;
            }
            return customizedRenderSettings;
        }


        /// <summary>
        /// When Default Renderer is saved in Spotfire using Tools > Options > Renderers >. It get saved with Spotfire.Dxp.LeadDiscovery.RendererUtilities.StructureColumnRendererModel class object. 
        /// When Default renderer gets loaded into the Datalytix it failes to covert Spotfire.Dxp.LeadDiscovery.RendererUtilities.StructureColumnRendererModel class object into CBVNStructureFilter.StructureColumnRendererModel class object.
        /// To fix this issue below method is required.
        /// Created StructureColumnRendererModel object and set values from Lead Discovery StructureColumnRendererModel class object.
        /// </summary>
        /// <param name="theLDStructureColumnRendererModelObject">Lead Discovery StructureColumnRendererModel class object</param>
        /// <returns>Local StructureColumnRendererModel class object</returns>
        private static StructureColumnRendererModel GetDataFromLDStructureColumnRendererModelClass(object theLDStructureColumnRendererModelObject)
        {
            StructureColumnRendererModel theStructureColumnRendererModel = new StructureColumnRendererModel();
            try
            {
                if (theLDStructureColumnRendererModelObject != null)
                {
                    foreach (PropertyInfo thePropertyInfo in theLDStructureColumnRendererModelObject.GetType().GetProperties())
                    {
                        switch (thePropertyInfo.Name)
                        {
                            case "ChemDrawRenderSettings":
                                theStructureColumnRendererModel.ChemDrawRenderSettings = Convert.ToString(thePropertyInfo.GetValue(theLDStructureColumnRendererModelObject, null));
                                break;
                            case "ChemDrawStyleFileName":
                                theStructureColumnRendererModel.ChemDrawStyleFileName = Convert.ToString(thePropertyInfo.GetValue(theLDStructureColumnRendererModelObject, null));
                                break;
                            case "ShowHydrogens":
                                theStructureColumnRendererModel.ShowHydrogens = Convert.ToString(thePropertyInfo.GetValue(theLDStructureColumnRendererModelObject, null));
                                break;
                            case "StringType":
                                theStructureColumnRendererModel.StringType = (StructureStringType)(thePropertyInfo.GetValue(theLDStructureColumnRendererModelObject, null));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return theStructureColumnRendererModel;
        }
    }
}
