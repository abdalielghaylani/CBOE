using System;
using CambridgeSoft.COE.Framework.Common;
using System.Collections.Generic;


/// <summary>
/// Summary description for SearchCriteriaHelper
/// </summary>
public class SearchCriteriaHelper {
    public static SearchCriteria BuildCriteriaForSubmittedCompounds(string tempNumber, string seqNumber, string chemistId) {
        SearchCriteria searchCriteria = new SearchCriteria();
        searchCriteria.SearchCriteriaID = 1;
        SearchCriteria.SearchCriteriaItem item;
        if(!string.IsNullOrEmpty(tempNumber)) {
            item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.NumericalCriteria criteria = new SearchCriteria.NumericalCriteria();
            criteria.InnerText = tempNumber.Trim();
            criteria.Operator = SearchCriteria.COEOperators.EQUAL;
            criteria.Trim = SearchCriteria.Positions.None;
            item.FieldId = 1;
            item.TableId = 1;
            item.Criterium = criteria;
            searchCriteria.Items.Add(item);
        }
        if (!string.IsNullOrEmpty(chemistId))
        {
            item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.NumericalCriteria criteria = new SearchCriteria.NumericalCriteria();
            criteria.InnerText = chemistId;
            criteria.Operator = SearchCriteria.COEOperators.EQUAL;
            criteria.Trim = SearchCriteria.Positions.None;
            item.FieldId = 3;
            item.TableId = 1;
            item.Criterium = criteria;
            searchCriteria.Items.Add(item);
        }
        return searchCriteria;
    }

    public static SearchCriteria BuildCriteriaForSubmitMixture(string registryNumber, string structureId)
    {
        SearchCriteria searchCriteria = new SearchCriteria();
        searchCriteria.SearchCriteriaID = 1;
        SearchCriteria.SearchCriteriaItem item;
        if (!string.IsNullOrEmpty(registryNumber))
        {
            item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.TextCriteria criteria = new SearchCriteria.TextCriteria();
            criteria.InnerText = registryNumber.Trim();
            criteria.Operator = SearchCriteria.COEOperators.LIKE;
            criteria.Trim = SearchCriteria.Positions.Both;
            criteria.CaseSensitive = SearchCriteria.COEBoolean.No;
            item.FieldId = 21;
            item.TableId = 2;
            item.Criterium = criteria;
            searchCriteria.Items.Add(item);
        }
        if(!string.IsNullOrEmpty(structureId)) {
            item = new SearchCriteria.SearchCriteriaItem();
            SearchCriteria.NumericalCriteria criteria = new SearchCriteria.NumericalCriteria();
            criteria.InnerText = structureId.Trim();
            criteria.Operator = SearchCriteria.COEOperators.EQUAL;
            criteria.Trim = SearchCriteria.Positions.None;
            item.FieldId = 12;
            item.TableId = 1;
            item.Criterium = criteria;
            searchCriteria.Items.Add(item);
        }

        return searchCriteria;
    }
}
