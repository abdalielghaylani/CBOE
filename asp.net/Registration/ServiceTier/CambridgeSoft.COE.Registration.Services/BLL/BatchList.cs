using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Csla;
using Csla.Data;
using Csla.Validation;

using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Validation;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class BatchList : BusinessListBase<BatchList, Batch>
    {

        [NonSerialized]
        private FilteredBindingList<Batch> _filteredList;

        [COEUserActionDescription("GetBatchListBrokenRules")]
        public void GetBrokenRulesDescription(List<BrokenRuleDescription> brokenRules)
        {
            try
            {
                foreach (Batch currentBatch in this)
                    currentBatch.GetBrokenRulesDescription(brokenRules);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
            }
        }

        [COEUserActionDescription("FilterBatchList")]
        public BatchList GetBatchesById(int id)
        {
            try
            {
                if (_filteredList == null) _filteredList = new FilteredBindingList<Batch>(this);
                _filteredList.RemoveFilter();//Just in case a previous filter has been applied
                _filteredList.FilterProvider = new Csla.FilterProvider(Filters.GetByID);
                _filteredList.ApplyFilter(String.Empty, id);

                if (_filteredList.Count > 0)
                {
                    foreach (Batch currentBatch in this)
                    {
                        if (currentBatch.ID != ((Batch)_filteredList[0]).ID)
                            this.Remove(currentBatch);
                    }
                }
                return this;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("GetBatchFromList")]
        public Batch GetBatchById(int id)
        {
            try
            {
                foreach (Batch currentBatch in this)
                {
                    if (currentBatch.ID == id)
                        return currentBatch;
                }
                return null;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }



        /// <summary>
        /// Gets the batch from full reg num.
        /// </summary>
        /// <param name="fullRegNumber">The full reg number.</param>
        /// <returns>The information for the batch that corresponds to the Full Reg Number that was passed in</returns>
        [COEUserActionDescription("GetBatchFromList")]
        public Batch GetBatchFromFullRegNum(string fullRegNumber)
        {
            try
            {
                foreach (Batch currentBatch in this)
                {
                    if (currentBatch.FullRegNumber.ToLower() == fullRegNumber.ToLower())
                        return currentBatch;
                }
                return null;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        /// <summary>
        /// Method to compare two objects and know the index inside the collection.
        /// IndexOf was not overloaded because is used for the removeAt call.
        /// </summary>
        /// <param name="component">Component to compare</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("GetBatchIndex")]
        public int GetIndex(Batch batch)
        {
            int retVal = 0;
            try
            {
                if (this.Contains(batch))
                {
                    foreach (Batch currentBatch in this)
                    {
                        retVal++;
                        if (batch.UniqueID == currentBatch.UniqueID)
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

        private class Filters
        {
            public static bool GetByID(object item, object filter)
            {
                int id = 0;
                Batch batch = null;
                //Filter is a int
                if (filter is int)
                    id = Convert.ToInt32((string)filter.ToString());
                //assume item is the object it self. (See string.empty parameter)
                if (item is Batch)
                {
                    batch = ((Batch)item);
                    if (batch != null && batch.ID == id) //Coverity Fix - CID 11694 
                        return true;
                }
                return false;
            }
        }

        internal static BatchList NewBatchList()
        {
            return new BatchList();
        }


        /// <summary>        
        /// Initialize new object
        /// </summary>
        /// <param name="xml">Xml that contain all batchs information</param>
        /// <param name="isNew">Defines if the object is new</param>
        /// <param name="isClean">Defines if the object is clean</param>
        /// <param name="sbi">Defines if SameBatchIdentity property is enable</param>
        /// <param name="regNum">Registry Number that contain new object</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("CreateBatchList")]
        public static BatchList NewBatchList(string xml, bool isNew, bool isClean,bool sbi,string regNum)
        {
            try
            {
                BatchList batchList = new BatchList(xml, isNew, isClean,sbi,regNum);
                return batchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateBatchList")]
        public static BatchList NewBatchList(string xml, bool isNew, bool isClean, bool sbi, string regNum, RLSStatus rlsStatus)
        {
            try
            {
                BatchList batchList = new BatchList(xml, isNew, isClean, sbi, regNum, rlsStatus);
                return batchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        [COEUserActionDescription("CreateBatchList")]
        public static BatchList NewBatchList(string xml, bool isNew, bool isClean)
        {
            try
            {
                BatchList batchList = new BatchList(xml, isNew, isClean);
                return batchList;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private BatchList()
        {
        }

        private BatchList(string xml, bool isNew, bool isClean)                       
            :this(xml,isNew,isClean,false,string.Empty)
        {
        }

        private BatchList(string xml, bool isNew, bool isClean,bool sbi,string regNum) {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));

            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("BatchList/Batch");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Batch.NewBatch(xIterator.Current.OuterXml, isNew, isClean,sbi,regNum));
                } while (xIterator.Current.MoveToNext());
            }
        }

        private BatchList(string xml, bool isNew, bool isClean, bool sbi, string regNum, RLSStatus rlsStatus)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));

            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("BatchList/Batch");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Batch.NewBatch(xIterator.Current.OuterXml, isNew, isClean, sbi, regNum, rlsStatus));
                } while (xIterator.Current.MoveToNext());
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            StringBuilder builder = new StringBuilder("");

            builder.Append("<BatchList>");

            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i].UpdateSelf(addCRUDattributes));
            }

            builder.Append("</BatchList>");

            return builder.ToString();
        }


        internal void UpdateFromXml(XmlNode incomingNode)
        {
            XmlNodeList batchIDNodeList = incomingNode.SelectNodes("//BatchList/Batch/BatchID");

            foreach (XmlNode batchIDNode in batchIDNodeList)
            {
                int batchID = int.Parse(batchIDNode.InnerText);  
                //look through each of the batches in the registry record and update if there is a match
                foreach (Batch batch in this)
                {
                    if (batch.ID == batchID)
                        //get the batch node and send to update of batch
                        batch.UpdateFromXml(batchIDNode.ParentNode);
                }
               
            }

        }

        internal void UpdateUserPreference(XmlNode incomingNode)
        {
            XmlNodeList batchIDNodeList = incomingNode.SelectNodes("//BatchList/Batch/BatchID");

            foreach (XmlNode batchIDNode in batchIDNodeList)
            {
                int batchID = int.Parse(batchIDNode.InnerText);
                //look through each of the batches in the registry record and update if there is a match
                foreach (Batch batch in this)
                {
                    if (batch.ID == batchID)
                        //get the batch node and send to update of batch
                        batch.UpdateUserPreference(batchIDNode.ParentNode);
                }

            }

        }
    }
}
