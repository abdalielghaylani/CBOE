
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using CambridgeSoft.COE.Registration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.ExceptionHandling;
using Csla;
using Csla.Data;
using Csla.Validation;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class BatchComponentFragmentList : 
        BusinessListBase<BatchComponentFragmentList, BatchComponentFragment>
    {
        private bool _isTemporary;
        internal bool IsTemporary
        {
            get { return _isTemporary; }
            set { _isTemporary = value; }
        }

        /// <summary>
        /// Method to compare two objects and know the index inside the collection.
        /// IndexOf was not overloaded because is used for the removeAt call.
        /// </summary>
        /// <param name="component">Component to compare</param>
        /// <returns>Index of the given component inside the collection</returns>
        /// 
        [COEUserActionDescription("GetBatchComponentFragmentListIndex")]
        public int GetIndex(BatchComponentFragment component)
        {
            int retVal = 0;
            try
            {
                if(this.Contains(component))
                {
                    foreach(BatchComponentFragment currentComponent in this)
                    {
                        retVal++;
                        if(currentComponent.UniqueID == component.UniqueID)
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

        /// <summary>
        /// Fetches a child object by its respository identifier.
        /// </summary>
        /// <param name="childId">the ID of the individual BatchComponentFragment</param>
        /// <returns>either the matching BatchComponentFragment instance, or null if not found or if childId is zero</returns>
        internal BatchComponentFragment GetChildById(int childId)
        {
            BatchComponentFragment soughtItem = null;
            if (childId > 0)
            {
                foreach (BatchComponentFragment current in this)
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

        internal static BatchComponentFragmentList NewBatchComponentFragmentList()
        {
            return new BatchComponentFragmentList();
        }

        [COEUserActionDescription("CreateBatchComponentFragmentList")]
        public static BatchComponentFragmentList NewBatchComponentFragmentList(string xml, bool isNew, bool isClean)
        {
            try
            {
            return new BatchComponentFragmentList(xml, isNew, isClean);
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private BatchComponentFragmentList()
        {
        }

        private BatchComponentFragmentList(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("BatchComponentFragmentList/BatchComponentFragment");
            if(xIterator.MoveNext())
            {
                do
                {
                    this.Add(BatchComponentFragment.NewBatchComponentFragment(xIterator.Current.OuterXml, isNew, isClean));
                } while(xIterator.Current.MoveToNext());
            }
        }

        public void Clear(bool clearDeletedList)
        {
            this.Clear();
            if (clearDeletedList)
                this.DeletedList.Clear();
                
        }

        public void ClearDeletedList()
        {
            this.DeletedList.Clear();
        }

        /// <summary>
        /// Get the deleted list count of fragments.
        /// </summary>
        public List<BatchComponentFragment> GetDeletedList()
        {
            return this.DeletedList;
        }

        internal void MarkNew()
        {
            foreach(BatchComponentFragment currentBatchComponentFragment in this)
                currentBatchComponentFragment.MarkNew();
        }

        [COEUserActionDescription("GetFromBatchComponentFragmentList")]
        public BatchComponentFragment GetBatchComponentFragmentByFragmentID(int fragmentID)
        {
            try
            {
                foreach(BatchComponentFragment bcFragment in this)
                {
                    if(bcFragment.FragmentID == fragmentID)
                        return bcFragment;
                }
                return null;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        internal string UpdateSelf(bool addCRUDattributes)
        {
            bool showUpdateTag = false;

            // If any fragment is modified, or there's any new fragment,
            // we should set update tag in <BatchComponentFragmentList> node.
            foreach (BatchComponentFragment currentBCFragment in this)
            {
                if (currentBCFragment.IsDirty || currentBCFragment.IsNew)
                {
                    showUpdateTag = true;
                    break;
                }
            }

            // If there's any fragment deleted, we should also set update tag in <BatchComponentFragmentList> node.
            if (!showUpdateTag && this.DeletedList.Count > 0)
                showUpdateTag = true;

            StringBuilder builder = new StringBuilder("");
            builder.Append(string.Format("<BatchComponentFragmentList{0}>", (addCRUDattributes && showUpdateTag ? " update=\"yes\"" : string.Empty)));

            foreach (BatchComponentFragment currentBCFragment in this)
                builder.Append(currentBCFragment.UpdateSelf(addCRUDattributes));

            foreach (BatchComponentFragment currentDeletedBCFragment in this.DeletedList)
                //CSBR:143968 for the case where the record has never been committed to the database, but has identifiers that are
                //deleted such as loading a template, simply remove them from the xml
                if (!currentDeletedBCFragment.IsNew)
                {
                    builder.Append(currentDeletedBCFragment.UpdateSelf(true));
                }


            builder.Append("</BatchComponentFragmentList>");

            return builder.ToString();
        }
        
        internal void UpdateFromXml(XmlNode parentNode)
        {
            XmlNodeList batchcomponentfragmentNodeList = parentNode.SelectNodes("//BatchComponentFragment");
            foreach (XmlNode batchcomponentfragmentNode in batchcomponentfragmentNodeList)
            {
                int batchcmpfrgId = (batchcomponentfragmentNode.SelectSingleNode("ID") != null?int.Parse(batchcomponentfragmentNode.SelectSingleNode("ID").InnerText):-1);
                if (batchcmpfrgId>=0)
                {
                     foreach(BatchComponentFragment batchcmpFrg in this)
                     {
                         if (batchcmpFrg.ID == batchcmpfrgId)
                         {
                             batchcmpFrg.UpdateFromXml(batchcomponentfragmentNode);
                         }
                     }
                }
                else
                    this.Add(BatchComponentFragment.NewBatchComponentFragment(batchcomponentfragmentNode.OuterXml,true,true));
            }
        }
    }
}



