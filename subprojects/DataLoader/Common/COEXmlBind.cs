using System;
using System.Reflection;
using System.Xml;

namespace CambridgeSoft.COE.DataLoader.Common
{
    /// <summary>
    /// Load and store XML attribute values bound to class members
    /// </summary>
    public class COEXmlBind
    {
        /// <summary>
        /// Load values from the object into the XML
        /// </summary>
        /// <param name="XmlObject"></param>
        /// <param name="MemberAttribute"></param>
        /// <param name="ValueAttribute"></param>
        /// <param name="SourceObject"></param>
        public static void LoadValues(XmlDocument XmlObject, string MemberAttribute, string ValueAttribute, Object SourceObject)
        {
            Type t = SourceObject.GetType();
            XmlNode oXmlNodeRoot = XmlObject.DocumentElement;
            //Coverity fix - CID 19592
            if (oXmlNodeRoot != null)
            {
                XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::*[attribute::" + MemberAttribute + "]");
                foreach (XmlNode oXmlNode in oXmlNodeList)
                {
                    string strMember = oXmlNode.Attributes[MemberAttribute].Value;
                    Object oValue = null;
                    try
                    {
                        oValue = t.InvokeMember(
                            strMember,
                            BindingFlags.DeclaredOnly
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.GetField,
                            null,
                            SourceObject,
                            null
                        );
                    }
                    catch
                    {
    #if DEBUG
                        throw;
    #endif
                    }
                    if (oValue != null)
                    {
                        if (oValue.GetType().IsEnum)    // Convert Enum to it's underlying integer value
                        {
                            oValue = (Int32)Enum.Parse(oValue.GetType(), oValue.ToString());
                        }
                        string strValue = oValue.ToString();
                        XmlAttribute oXmlAttribute = oXmlNode.Attributes[ValueAttribute];
                        if (oXmlAttribute == null)
                        {
                            oXmlAttribute = XmlObject.CreateAttribute(ValueAttribute);
                            oXmlNode.Attributes.Append(oXmlAttribute);
                        }
                        oXmlAttribute.Value = strValue;
                    }
                } // foreach (XmlNode oXmlNode in oXmlNodeList)
            }
            return;
        } // LoadValues()
        /// <summary>
        /// Overloaded. See above
        /// </summary>
        /// <param name="XmlString"></param>
        /// <param name="MemberAttribute"></param>
        /// <param name="ValueAttribute"></param>
        /// <param name="SourceObject"></param>
        /// <returns></returns>
        public static string LoadValues(string XmlString, string MemberAttribute, string ValueAttribute, Object SourceObject)
        {
            XmlDocument XmlObject = new XmlDocument();
            try
            {
                XmlObject.LoadXml(XmlString);
                LoadValues(XmlObject, MemberAttribute, ValueAttribute, SourceObject);
                XmlString = XmlObject.OuterXml;
            }
            catch
            {
                ;
            }
            return XmlString;
        } // LoadValues()
        /// <summary>
        /// Store values to the object from the XML
        /// </summary>
        /// <param name="XmlObject"></param>
        /// <param name="MemberAttribute"></param>
        /// <param name="ValueAttribute"></param>
        /// <param name="DestinationObject"></param>
        public static void StoreValues(XmlDocument XmlObject, string MemberAttribute, string ValueAttribute, Object DestinationObject)
        {
            Type t = DestinationObject.GetType();
            Object[] oValue = new Object[1];
            XmlNode oXmlNodeRoot = XmlObject.DocumentElement;
            //Coverity fix - CID 19593
            if (oXmlNodeRoot != null)
            {
                XmlNodeList oXmlNodeList = oXmlNodeRoot.SelectNodes("descendant::*[attribute::" + MemberAttribute + "]");
                foreach (XmlNode oXmlNode in oXmlNodeList)
                {
                    string strMember = oXmlNode.Attributes[MemberAttribute].Value;
                    XmlAttribute oXmlAttribute = oXmlNode.Attributes[ValueAttribute];
                    if (oXmlAttribute == null)
                    {
                        continue;   // Really shouldn't happen ?
                    }
                    string strValue = oXmlAttribute.Value;
                    oValue[0] = Convert.ToInt32(strValue);
                    try
                    {
                        MemberInfo[] matchedMembers = t.GetMember(strMember, BindingFlags.DeclaredOnly
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.SetField
                        );

                        foreach(MemberInfo mi in matchedMembers)
                        {
                            if (mi is PropertyInfo)
                            {
                                PropertyInfo pi = (PropertyInfo)mi;
                                pi.SetValue(DestinationObject, oValue[0], null);
                            }
                            else if (mi is FieldInfo)
                            {
                                FieldInfo fi = (FieldInfo)mi;
                                fi.SetValue(DestinationObject, oValue[0]);
                            }
                            else
                            {
                                t.InvokeMember(
                                    strMember,
                                    BindingFlags.DeclaredOnly
                                    | BindingFlags.Public
                                    | BindingFlags.NonPublic
                                    | BindingFlags.Instance
                                    | BindingFlags.SetField,
                                    null,
                                    DestinationObject,
                                    oValue
                                );
                            }
                        }
                    }
                    catch
                    {
    #if DEBUG
                        throw;
    #endif
                    }
                } // foreach (XmlNode oXmlNode in oXmlNodeList)
            }
            return;
        } // StoreValues()
        /// <summary>
        /// Overloaded. See above
        /// </summary>
        /// <param name="XmlString"></param>
        /// <param name="MemberAttribute"></param>
        /// <param name="ValueAttribute"></param>
        /// <param name="DestinationObject"></param>
        /// <returns></returns>
        public static string StoreValues(string XmlString, string MemberAttribute, string ValueAttribute, Object DestinationObject)
        {
            XmlDocument XmlObject = new XmlDocument();
            XmlObject.LoadXml(XmlString);
            StoreValues(XmlObject, MemberAttribute, ValueAttribute, DestinationObject);
            return XmlObject.OuterXml;
        } // StoreValues()
    }
}
