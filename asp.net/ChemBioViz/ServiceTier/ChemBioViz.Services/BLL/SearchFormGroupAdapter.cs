using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService
{
    public class SearchFormGroupAdapter
    {
        public static SearchCriteria GetSearchCriteria(FormGroup.Display display)
        {
            SearchCriteria criteria = new SearchCriteria();
            XmlSerializer ser = new XmlSerializer(typeof(FormGroup.QueryDisplay));
            XmlDocument doc = new XmlDocument();
            StringWriter stringWriter = new StringWriter();
            ser.Serialize(stringWriter, display);
            stringWriter.Flush();
            doc.LoadXml(stringWriter.ToString());
            stringWriter.Close();

            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("COE", "COE.FormGroup");

            XmlNodeList theXmlNodeList = doc.SelectNodes("//COE:searchCriteriaItem", manager);
            if (theXmlNodeList != null) //Coverity Fix 
            {
                foreach (XmlNode node in theXmlNodeList)
                {
                    SearchCriteria.SearchCriteriaItem item = new SearchCriteria.SearchCriteriaItem(node);
                    criteria.Items.Add(item);
                }
            }
            /*foreach (FormGroup.Form form in display.Forms)
            {
                foreach (FormGroup.FormElement element in form.LayoutInfo)
                {
                    if (element.SearchCriteriaItem != null)
                    {
                        SearchCriteria.SearchCriteriaItem newItem = SearchCriteria.SearchCriteriaItem.GetSearchCriteriaItem(CambridgeSoft.COE.Framework.Common.Utilities.XmlSerialize(element.SearchCriteriaItem));
                        newItem.Modifier = element.BindingExpression;
                        criteria.Items.Add(newItem);
                    }
                }
            }*/

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Search Criteria:");
            System.Diagnostics.Debug.WriteLine(criteria.ToString());
#endif

            return criteria;
        }

        public static ResultsCriteria GetResultsCriteria(FormGroup.Display display)
        {
            if (display is FormGroup.DetailsDisplay)
                return ((FormGroup.DetailsDisplay)display).ResultsCriteria;
            else
                return ((FormGroup.ListDisplay)display).ResultsCriteria;
        }

        public static COEDataView GetDataView(int dataViewId)
        {
            COEDataViewBO dataViewService = COEDataViewBO.Get(dataViewId);

            if (dataViewService != null)
                return dataViewService.COEDataView;

            return null;
        }

        public static void PopulateSearchCriteria(SearchCriteria searchCriteria, int searchCriteriaId, string searchCriteriaValue)
        {
            foreach (SearchCriteria.SearchCriteriaItem currentSearchCriteriaItem in searchCriteria.Items)
            {
                if (currentSearchCriteriaItem.ID == searchCriteriaId)
                {
                    currentSearchCriteriaItem.Criterium.Value = searchCriteriaValue;
                }
            }
        }

        public static void PopulateSearchCriteria(SearchCriteria searchCriteria, SearchCriteria.SearchCriteriaItem item)
        {
            foreach (SearchCriteria.SearchCriteriaItem currentSearchCriteriaItem in searchCriteria.Items)
            {
                if (currentSearchCriteriaItem.ID == item.ID)
                {
                    currentSearchCriteriaItem.Criterium = item.Criterium;
                }
            }
        }

        public static SearchCriteria GetSearchCriteria(FormGroup.Display display, int searchCriteriaId, string searchCriteriaValue)
        {
            SearchCriteria criteria = new SearchCriteria();

            foreach (FormGroup.Form form in display.Forms)
            {
                foreach (FormGroup.FormElement element in form.LayoutInfo)
                {
                    if (element.SearchCriteriaItem != null && element.SearchCriteriaItem.ID == searchCriteriaId)
                    {
                        SearchCriteria.SearchCriteriaItem newItem = SearchCriteria.SearchCriteriaItem.GetSearchCriteriaItem(CambridgeSoft.COE.Framework.Common.Utilities.XmlSerialize(element.SearchCriteriaItem));
                        newItem.Modifier = element.BindingExpression;
                        newItem.Criterium.Value = searchCriteriaValue;
                        criteria.Items.Add(newItem);
                        break;
                    }
                }
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Search Criteria:");
            System.Diagnostics.Debug.WriteLine(criteria.ToString());
#endif

            return criteria;
        }
    }
}
