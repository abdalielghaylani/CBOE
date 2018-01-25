using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using CambridgeSoft.COE.Registration.Access;
using CambridgeSoft.COE.Registration.Services.Types;

namespace PerkinElmer.COE.Registration.Server.Code
{
    public static class Extensions
    {
        #region Utils - set or get private members/private methods via Reflection
        private static void SetPrivateVariable(object obj, string variableName, object value)
        {
            obj.GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance).SetValue(obj, value);
        }

        private static void CallPrivateMethod(object obj, string methodName)
        {
            obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(obj, null);
        }

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

        #endregion

        #region UpdateFromXmlEx RegistryRecord
        public static void UpdateFromXmlEx(this RegistryRecord record, string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var rootNode = doc.DocumentElement;

            // RegistryRecord itself only update properties that are allowed to be updated not auto-generated fields like person created
            var node = rootNode.SelectSingleNode("SubmissionComments");
            if (node != null && !string.IsNullOrEmpty(node.InnerText) && (record.SubmissionComments != node.InnerText))
                record.SubmissionComments = node.InnerText;

            /* TODO: RegNumber is not updating! */
            if (string.IsNullOrEmpty(record.RegNumber.RegNum))
            {
                SetPrivateVariable(record, "_regNumber", RegNumber.NewRegNumber(rootNode.SelectSingleNode("RegNumber").OuterXml, true));
                CallPrivateMethod(record.RegNumber, "MarkDirty");
                CallPrivateMethod(record, "MarkDirty");
            }

            record.PropertyList.UpdateFromXmlEx(rootNode.SelectSingleNode("PropertyList"));
            record.ProjectList.UpdateFromXmlEx(rootNode.SelectSingleNode("ProjectList"));
            record.IdentifierList.UpdateFromXmlEx(rootNode.SelectSingleNode("IdentifierList"));
            record.BatchList.UpdateFromXmlEx(rootNode.SelectSingleNode("BatchList"));
            record.ComponentList.UpdateFromXmlEx(rootNode.SelectSingleNode("ComponentList"));
        }

        #endregion

        #region PropertyList

        public static void UpdateFromXmlEx(this PropertyList list, XmlNode dataNode)
        {
            if (dataNode == null) return;

            foreach (Property property in list)
            {
                var propertyNode = dataNode.SelectSingleNode(string.Format("Property[@name='{0}']", property.Name));
                if (propertyNode != null)
                {
                    property.UpdateFromXmlEx(propertyNode);
                }
            }
        }

        public static void UpdateFromXmlEx(this Property property, XmlNode dataNode)
        {
            if (dataNode == null) return;

            if (!property.Value.Equals(dataNode.InnerText))
            {
                property.Value = dataNode.InnerText;
            }
        }

        #endregion

        #region ProjectList
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

        #endregion ProjectList

        #region IdentifierList
        public static void UpdateFromXmlEx(this IdentifierList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
            XmlNodeList nodes = dataNode.SelectNodes("Identifier");

            var newIemsToAdd = new List<Identifier>();
            foreach (XmlNode node in nodes)
            {
                Identifier identifier = Identifier.NewIdentifier(node.OuterXml, true, true);
                // Compare the unique id for the identifier entry and input value with existing entries to see if there is a match.
                // If there is, keep the data. Otherwise assume it is new and add it.
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

            var itemsToRemove = new List<int>();
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
                    itemsToRemove.Insert(0, index);
                index++;
            }

            foreach (var itemIndex in itemsToRemove)
            {
                RemoveItem(list, itemIndex);
            }

            foreach (Identifier identifer in newIemsToAdd)
            {
                list.Add(identifer);
            }
        }

        #endregion

        #region BatchList

        public static void UpdateFromXmlEx(this BatchList list, XmlNode dataNode)
        {
            if (dataNode == null) return;

            XmlNodeList batchNodeList = dataNode.SelectNodes("Batch");

            foreach (XmlNode batchNode in batchNodeList)
            {
                XmlNode batchIdNode = batchNode.SelectSingleNode("BatchID");
                int batchID = int.Parse(batchIdNode.InnerText);
                // look through each of the batches in the registry record and update if there is a match
                foreach (Batch batch in list)
                {
                    if (batch.ID == batchID)
                    {
                        batch.UpdateFromXmlEx(batchNode);
                    }
                }
            }
        }

        public static void UpdateFromXmlEx(this Batch batch, XmlNode dataNode)
        {
            XmlNode projectListNode = dataNode.SelectSingleNode("ProjectList");
            if (projectListNode != null)
                batch.ProjectList.UpdateFromXmlEx(projectListNode);

            XmlNode identifierListNode = dataNode.SelectSingleNode("IdentifierList");
            batch.IdentifierList.UpdateFromXmlEx(identifierListNode);

            XmlNode propertyListNode = dataNode.SelectSingleNode("PropertyList");
            batch.PropertyList.UpdateFromXmlEx(propertyListNode);

            XmlNode batchComponentListNode = dataNode.SelectSingleNode("BatchComponentList");
            batch.BatchComponentList.UpdateFromXmlEx(batchComponentListNode);
        }

        public static void UpdateFromXmlEx(this BatchComponentList list, XmlNode dataNode)
        {
            XmlNodeList batchIDNodeList = dataNode.SelectNodes("BatchComponent/BatchID");
            foreach (XmlNode batchIDNode in batchIDNodeList)
            {
                int batchID = int.Parse(batchIDNode.InnerText);

                foreach (BatchComponent batchComponent in list)
                {
                    if (batchComponent.BatchID == batchID)
                    {
                        batchComponent.UpdateFromXmlEx(batchIDNode.ParentNode);
                    }
                }
            }
        }

        public static void UpdateFromXmlEx(this BatchComponent batchComponent, XmlNode dataNode)
        {
            XmlNode propertyListNode = dataNode.SelectSingleNode("PropertyList");
            batchComponent.PropertyList.UpdateFromXmlEx(propertyListNode);
            XmlNode batchcomponentFragmentListNode = dataNode.SelectSingleNode("BatchComponentFragmentList");
            batchComponent.BatchComponentFragmentList.UpdateFromXmlEx(batchcomponentFragmentListNode);
        }

        public static void UpdateFromXmlEx(this BatchComponentFragmentList list, XmlNode dataNode)
        {
            XmlNodeList batchcomponentfragmentNodeList = dataNode.SelectNodes("BatchComponentFragment");

            var itemsToRemove = new List<int>();
            int index = 0;
            foreach (BatchComponentFragment batchcmpFrg in list)
            {
                bool foundFragment = false;
                foreach (XmlNode batchcomponentfragmentNode in batchcomponentfragmentNodeList)
                {
                    BatchComponentFragment batchcomponentfragment = BatchComponentFragment.NewBatchComponentFragment(batchcomponentfragmentNode.OuterXml, true, true);

                    if (batchcomponentfragment.UniqueID == batchcmpFrg.UniqueID)
                    {
                        foundFragment = true;
                        break;
                    }
                }

                if (!foundFragment)
                    itemsToRemove.Insert(0, index);

                index++;
            }

            foreach (var itemIndex in itemsToRemove)
            {
                RemoveItem(list, itemIndex);
            }

            foreach (XmlNode batchcomponentfragmentNode in batchcomponentfragmentNodeList)
            {
                BatchComponentFragment batchcomponentfragment = BatchComponentFragment.NewBatchComponentFragment(batchcomponentfragmentNode.OuterXml, true, true);
                bool foundFragment = false;
                foreach (BatchComponentFragment batchcmpFrg in list)
                {
                    if (batchcomponentfragment.UniqueID == batchcmpFrg.UniqueID)
                    {
                        UpdateFromXml(batchcmpFrg, batchcomponentfragmentNode);
                        foundFragment = true;
                        break;
                    }
                }
                if (!foundFragment)
                {
                    // Mark BatchComponentFragmentList as Dirty to fix
                    // CBOE-6398 Update Record: Getting error when add a fragment to the registry record which has more than one batch (SBI-True)                   
                    CallPrivateMethod(batchcomponentfragment, "MarkDirty");
                    list.Add(batchcomponentfragment);
                }
            }
        }

        #endregion

        #region Component List
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
            if (dataNode == null) return;
            // Update Percentage
            var percentageNode = dataNode.SelectSingleNode("Percentage");
            double percetage;
            if (percentageNode != null && double.TryParse(percentageNode.InnerText, out percetage))
            {
                component.Percentage = percetage;
            }

            // Update compound
            XmlNode compoundNode = dataNode.SelectSingleNode("Compound");
            component.Compound.UpdateFromXmlEx(compoundNode);
        }

        public static void UpdateFromXmlEx(this Compound compound, XmlNode dataNode)
        {
            if (dataNode == null) return;
            // Validate
            var regNumberNode = dataNode.SelectSingleNode("RegNumber/RegNumber");
            if (regNumberNode == null || !regNumberNode.InnerText.Equals(compound.RegNumber.RegNum, System.StringComparison.OrdinalIgnoreCase))
                throw new Exception("The compound ID is invalid");

            // Update
            var lastModifiedDate = DateTime.Now;
            var node = dataNode.SelectSingleNode("DateLastModified");
            if (node != null)
            {
                DateTime.TryParse(node.InnerText, out lastModifiedDate);
                compound.DateLastModified = lastModifiedDate;
            }
            compound.IdentifierList.UpdateFromXmlEx(dataNode.SelectSingleNode("IdentifierList"));
            compound.BaseFragment.UpdateFromXmlEx(dataNode.SelectSingleNode("BaseFragment"));
            compound.FragmentList.UpdateFromXmlEx(dataNode.SelectSingleNode("FragmentList"));
            compound.CompoundFragmentList.UpdateFromXmlEx(dataNode.SelectSingleNode("FragmentList"));
            compound.PropertyList.UpdateFromXmlEx(dataNode.SelectSingleNode("PropertyList"));
            compound.ProjectsList.UpdateFromXmlEx(dataNode.SelectSingleNode("ProjectList"));
        }

        public static void UpdateFromXmlEx(this BaseFragment baseFragment, XmlNode dataNode)
        {
            if (dataNode == null) return;
            baseFragment.Structure.UpdateFromXmlEx(dataNode.SelectSingleNode("Structure"));
        }

        public static void UpdateFromXmlEx(this FragmentList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
        }

        public static void UpdateFromXmlEx(this CompoundFragmentList list, XmlNode dataNode)
        {
            if (dataNode == null) return;
        }

        public static void UpdateFromXmlEx(this Structure structure, XmlNode dataNode)
        {
            if (dataNode == null) return;
            var idNode = dataNode.SelectSingleNode("StructureID");
            int id;
            if (idNode != null && int.TryParse(idNode.InnerText, out id) && id == structure.ID)
            {
                int drawingType;
                var node = dataNode.SelectSingleNode("DrawingType");
                if (node != null && int.TryParse(node.InnerText, out drawingType))
                {
                    if (!structure.DrawingType.Equals((DrawingType)drawingType))
                    {
                        structure.DrawingType = (DrawingType)drawingType;
                    }
                }
                var structureElement = dataNode.SelectSingleNode("Structure") as XmlElement;
                if (structureElement != null)
                {
                    if (!structure.Value.Equals(structureElement.InnerText))
                    {
                        structure.Value = structureElement.InnerText;
                    }
                    if (structureElement.HasAttribute("formula"))
                    {
                        if (!structure.Formula.Equals(structureElement.GetAttribute("formula")))
                        {
                            structure.Formula = structureElement.GetAttribute("formula");
                        }
                    }
                    if (structureElement.HasAttribute("molWeight"))
                    {
                        var molWeightStr = structureElement.GetAttribute("molWeight");
                        double molWeight;
                        if (double.TryParse(molWeightStr, out molWeight))
                        {
                            if (!structure.MolWeight.Equals(molWeight))
                            {
                                structure.MolWeight = molWeight;
                            }
                        }
                    }
                    node = structureElement.SelectSingleNode("Structure/validationRuleList");
                    if (node != null)
                        SetPrivateVariable(structure, "_validationRulesXml", node.OuterXml);

                    node = structureElement.SelectSingleNode("NormalizedStructure");
                    if (node != null && !string.IsNullOrEmpty(node.InnerText))
                    {
                        if (!structure.NormalizedStructure.Equals(node.InnerText))
                        {
                            structure.NormalizedStructure = node.InnerText;
                        }
                    }
                    // If Value is null, put the normalized structure here.
                    //  This issue is occurring when a new component is added to a temporary record.
                    if (string.IsNullOrEmpty(structure.Value))
                    {
                        if (!structure.Value.Equals(structure.NormalizedStructure))
                        {
                            structure.Value = structure.NormalizedStructure;
                        }
                    }

                    node = structureElement.SelectSingleNode("UseNormalization");
                    COEDALBoolean useNormalization;
                    if (node != null && Enum.TryParse<COEDALBoolean>(node.InnerText, out useNormalization))
                        structure.UseNormalizedStructure = useNormalization == COEDALBoolean.T;
                }
                structure.PropertyList.UpdateFromXmlEx(dataNode.SelectSingleNode("PropertyList"));
                structure.IdentifierList.UpdateFromXmlEx(dataNode.SelectSingleNode("IdentifierList"));
            }
        }

        #endregion

        #region registry record
        public static void FixBatchesFragmentsEx(this RegistryRecord registryRecord)
        {
            registryRecord.UpdateFragments();

            // In case fragments has been updated, we need to modify the other batches if the flag sameIdentity = true.
            if (registryRecord.SameBatchesIdentity)
            {
                if (registryRecord.BatchList.Count > 0)
                {
                    // We assume the first batch is the one that has the correct values for the fragment sections. Notice that in RegGUI only values of batch 0 are editable (if SBI = true).
                    foreach (BatchComponent batchComp in registryRecord.BatchList[0].BatchComponentList)
                    {
                        // Overwrite values of other batches.
                        for (int i = 1; i < registryRecord.BatchList.Count; i++)
                        {
                            registryRecord.BatchList[i].BatchComponentList[batchComp.OrderIndex - 1].BatchComponentFragmentList = batchComp.BatchComponentFragmentList;
                        }
                    }
                }
            }
        }
        #endregion
    }
}