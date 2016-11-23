using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Registration.Services.Properties;
using CambridgeSoft.COE.Registration.Validation;
using CambridgeSoft.COE.Framework.Common.Validation;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class BatchComponentList :
        BusinessListWithValidation<BatchComponentList, BatchComponent>
    {

        /// <summary>
        /// Method to compare two objects and know the index inside the collection.
        /// IndexOf was not overloaded because is used for the removeAt call.
        /// </summary>
        /// <param name="component">Component to compare</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("GetBatchComponentIndex")]
        public int GetIndex(BatchComponent component)
        {
            int retVal = 0;

            try
            {
                if (this.Contains(component))
                {
                    foreach (BatchComponent currentComponent in this)
                    {
                        retVal++;
                        if (currentComponent.UniqueID == component.UniqueID)
                            break;
                    }
                }
                return retVal;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return retVal;
            }
        }

        public override bool IsValid
        {
            [COEUserActionDescription("ValidateBatchComponentList")]
            get
            {
                try
                {
                    return base.IsValid && this.CheckBatchComponents();
                }
                catch (Exception exception)
                {
                    COEExceptionDispatcher.HandleBLLException(exception);
                    return false;
                }
            }
        }

        private bool CheckBatchComponents()
        {
            bool valid = true;
            foreach (BatchComponent currentBatchComponent in this)
            {
                valid &= currentBatchComponent.IsValid;
            }
            //Check that the total of percentages is > 0 and < 100.0
            return valid && this.CheckPercentage(); ;
        }

        [COEUserActionDescription("GetBatchComponentListBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                foreach (BatchComponent currentBatchComponent in this)
                    currentBatchComponent.GetBrokenRulesDescription(brokenRules);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        private bool CheckPercentage()
        {
            bool retVal = true;
            double sum = 0;
            foreach (BatchComponent currentBatchComponent in this)
            {
                foreach (Property currentProperty in currentBatchComponent.PropertyList)
                {
                    if (currentProperty.Name.ToUpper() == "PERCENTAGE" && !string.IsNullOrEmpty(currentProperty.Value))
                            sum += double.Parse(currentProperty.Value);
                }
            }
            if (sum < 0 || sum > 100.00)
            {
                retVal = false;
                throw new ValidationException(Resources.PercentagesInvalidSum);
            }
            return retVal;
        }

        internal static BatchComponentList NewBatchComponentList()
        {
            return new BatchComponentList();
        }

        [COEUserActionDescription("CreateBatchComponentList")]
        public static BatchComponentList NewBatchComponentList(string xml, bool isNew, bool isClean)
        {
            try
            {
                BatchComponentList batchComponentList = new BatchComponentList(xml, isNew, isClean);
                return batchComponentList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Fetches a child object by its respository identifier.
        /// </summary>
        /// <param name="childId">the ID of the individual BatchComponent</param>
        /// <returns>either the matching BatchComponent instance, or null if not found or if childId is zero</returns>
        internal BatchComponent GetChildById(int childId)
        {
            BatchComponent soughtItem = null;
            if (childId > 0)
            {
                foreach (BatchComponent current in this)
                {
                    if (current.ID == childId)
                    {
                        soughtItem = current;
                        break;
                    }
                }
            }
            return soughtItem;
        }

        private BatchComponentList()
        {
        }

        private BatchComponentList(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("BatchComponentList/BatchComponent");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(BatchComponent.NewBatchComponent(xIterator.Current.OuterXml, isNew, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<BatchComponentList>");

            for (int i = 0; i < this.DeletedList.Count; i++)
            {
                if (!this.DeletedList[i].IsNew)
                    builder.Append(this.DeletedList[i].UpdateSelf(true));
            }

            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i].UpdateSelf(addCRUDattributes));
            }

            builder.Append("</BatchComponentList>");
            return builder.ToString();
        }

        internal void ForgetDeleted()
        {
            this.DeletedList.Clear();
        }

        internal void UpdateFromXml(XmlNode parentNode)
        {
            XmlNodeList batchIDNodeList = parentNode.SelectNodes("//BatchComponent/BatchID");
            //update
            foreach (XmlNode batchIDNode in batchIDNodeList)
            {
                int batchID = int.Parse(batchIDNode.InnerText);
                ////add
                //if (batchID == 0)
                //{
                //    this.Add(BatchComponent.NewBatchComponent(batchIDNode.ParentNode.OuterXml, true, true));
                //}
                ////update
                //else
                //{
                    foreach (BatchComponent batchComponent in this)
                    {
                        if (batchComponent.BatchID == batchID)
                        {
                            batchComponent.UpdateFromXml(batchIDNode.ParentNode);
                        }
                    }
                //}
            }


            //delete

        }
    }
}
