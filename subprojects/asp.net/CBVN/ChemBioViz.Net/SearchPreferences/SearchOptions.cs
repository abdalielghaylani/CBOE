using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using System.Configuration;
using System.Xml.Serialization;

namespace SearchPreferences
{
    public class SearchOptions
    {
        #region Variables
        private static SearchOptions m_searchOptionsInstance = new SearchOptions();

        private SearchCriteria.StructureCriteria m_structureCriteria;
        private SearchCriteria.CSFormulaCriteria m_formulaCriteria;
        private SearchCriteria.CSMolWeightCriteria m_molWeightCriteria;
        private SearchCriteria.DateCriteria m_dateCriteria;
        private SearchCriteria.NumericalCriteria m_numericalCriteria;
        private SearchCriteria.TextCriteria m_textCriteria;
        private bool m_matchStereochemistry;
        #endregion

        #region Properties
        public static SearchOptions SearchOptionsInstance
        {
            get { return SearchOptions.m_searchOptionsInstance; }
            set { SearchOptions.m_searchOptionsInstance = value; }
        }
        public SearchCriteria.StructureCriteria StructureCriteria
        {
            get { return m_structureCriteria; }
            set { m_structureCriteria = value;}
        }
        public SearchCriteria.CSFormulaCriteria FormulaCriteria
        {
            get { return m_formulaCriteria; }
            set { m_formulaCriteria = value; }
        }
        public SearchCriteria.CSMolWeightCriteria MolWeightCriteria
        {
            get { return m_molWeightCriteria; }
            set { m_molWeightCriteria = value; }
        }
        public SearchCriteria.DateCriteria DateCriteria
        {
            get { return m_dateCriteria; }
            set { m_dateCriteria = value; }
        }
        public SearchCriteria.NumericalCriteria NumericalCriteria
        {
            get { return m_numericalCriteria; }
            set { m_numericalCriteria = value; }
        }
        public SearchCriteria.TextCriteria TextCriteria
        {
            get { return m_textCriteria; }
            set { m_textCriteria = value; }
        }
        public bool MatchStereochemistry
        {
            get { return m_matchStereochemistry; }
            set { m_matchStereochemistry = value; }
        }
        #endregion

        #region Constructor
        public SearchOptions()
        {
            m_structureCriteria = new SearchCriteria.StructureCriteria();
            m_formulaCriteria = new SearchCriteria.CSFormulaCriteria();
            m_molWeightCriteria = new SearchCriteria.CSMolWeightCriteria();
            m_dateCriteria = new SearchCriteria.DateCriteria();
            m_numericalCriteria = new SearchCriteria.NumericalCriteria();
            m_textCriteria = new SearchCriteria.TextCriteria();
        }
        #endregion
    }

}
