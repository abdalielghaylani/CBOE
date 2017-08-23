using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class Extensions
    {
        private static void UpdateFromXml(object obj, XmlNode node)
        {
            var args = new object[] { node };
            obj.GetType().GetMethod("UpdateFromXml", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, args);
        }

        private static void RemoveItem(object obj, int index)
        {
            var args = new object[] { index };
            obj.GetType().GetMethod("RemoveItem", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, args);
        }

        public static void UpdateFromXmlEx(this RegistryRecord record, string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var rootNode = doc.DocumentElement;

            // RegistryRecord itself only update properties that are allowed to be updated not auto-generated fields like person created
            var matchingChild = rootNode.SelectSingleNode("SubmissionComments");
            if (matchingChild != null && !string.IsNullOrEmpty(matchingChild.InnerText) && (record.SubmissionComments != matchingChild.InnerText))
                record.SubmissionComments = matchingChild.InnerText;

            record.PropertyList.UpdateFromXmlEx(rootNode.SelectSingleNode("PropertyList"));
            record.ProjectList.UpdateFromXmlEx(rootNode.SelectSingleNode("ProjectList"));
            record.IdentifierList.UpdateFromXmlEx(rootNode.SelectSingleNode("IdentifierList"));
            record.BatchList.UpdateFromXmlEx(rootNode.SelectSingleNode("BatchList"));
            record.ComponentList.UpdateFromXmlEx(rootNode.SelectSingleNode("ComponentList"));
        }

        public static void UpdateFromXmlEx(this PropertyList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
            UpdateFromXml(list, dataNode);
        }

        public static void UpdateFromXmlEx(this ProjectList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
            var nodes = dataNode.SelectNodes("Project");
            var itemsToRemove = new List<int>();
            var index = 0;
            foreach (var p in list)
            {
                XmlNode matchingChild = dataNode.SelectSingleNode(string.Format("Project[ID='{0}']", p.ID));
                // If a node matches this ID, we should update the matching object.
                if (matchingChild != null)
                    UpdateFromXml(p, matchingChild);
                else
                    itemsToRemove.Insert(0, index);
                ++index;
            }
            foreach (var itemIndex in itemsToRemove)
            {
                RemoveItem(list, itemIndex);
            }
            foreach (XmlNode node in nodes)
            {
                XmlNode idNode = node.SelectSingleNode("ID");
                if (idNode == null || idNode.InnerText == "0" || idNode.InnerText == string.Empty)
                {
                    list.Add(Project.NewProject(node.OuterXml, false, true));
                }
            }
        }

        public static void UpdateFromXmlEx(this IdentifierList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
            XmlNodeList nodes = dataNode.SelectNodes("Identifier");

            var newIemsToAdd = new List<Identifier>();
            var itemsToRemove = new List<int>();
            foreach (XmlNode node in nodes)
            {
                Identifier identifier = Identifier.NewIdentifier(node.OuterXml, true, true);
                // look at the unique id for the identifier entry and input value, see if there is a match. If there is, keep the data
                // otherwise assume it is new and add it
                bool identifierFound = false;
                foreach (Identifier idCurrent in list)
                {
                    if (idCurrent.UniqueID == identifier.UniqueID && idCurrent.InputText.Equals(identifier.InputText))
                    {
                        idCurrent.InputText = identifier.InputText;
                        identifierFound = true;
                        break;
                    }
                }

                if (!identifierFound)
                    newIemsToAdd.Add(identifier);
            }

            int index = 0;
            foreach (Identifier idCurrent in list)
            {
                bool identifierFound = false;
                foreach (XmlNode node in nodes)
                {
                    Identifier identifier = Identifier.NewIdentifier(node.OuterXml, true, true);
                    if ((idCurrent.UniqueID == identifier.UniqueID) && idCurrent.InputText.Equals(identifier.InputText))
                    {
                        identifierFound = true;
                        break;
                    }
                }

                if (!identifierFound)
                    itemsToRemove.Add(index);
                index++;
            }

            foreach (var itemIndex in itemsToRemove)
            {
                RemoveItem(list, itemIndex);
            }

            foreach (Identifier identifer in newIemsToAdd)
                list.Add(identifer);
        }

        public static void UpdateFromXmlEx(this BatchList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
            UpdateFromXml(list, dataNode);
        }

        public static void UpdateFromXmlEx(this ComponentList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
            var componentNodes = dataNode.SelectNodes("Component");
            foreach (XmlNode componentNode in componentNodes)
            {
                var componentIndexNode = componentNode.SelectSingleNode("ComponentIndex");
                int componentIndex;
                if (componentIndexNode == null || !int.TryParse(componentIndexNode.InnerText, out componentIndex))
                    continue;
                foreach (var comp in list)
                {
                    if (comp.ComponentIndex == componentIndex)
                    {
                        comp.UpdateFromXmlEx(componentNode);
                        break;
                    }
                }
            }
        }

        public static void UpdateFromXmlEx(this Component component, XmlNode dataNode)
        {
            UpdateFromXml(component, dataNode);
        }
    }
}