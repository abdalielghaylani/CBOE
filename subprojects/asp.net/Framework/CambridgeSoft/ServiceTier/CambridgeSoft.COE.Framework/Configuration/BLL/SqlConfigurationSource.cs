using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using System.Data.Common;
using System.Reflection;
using System.Configuration;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using System.Data;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.Framework.COELoggingService;


namespace CambridgeSoft.COE.Framework.COEConfigurationService
{
    public class SqlConfigurationSource : IConfigurationSource
    {
        #region Variables
        private DAL dal;
        private Dictionary<string, ConfigurationSection> cachedSections;
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEConfiguration");

        #endregion


        #region Constructors
        public SqlConfigurationSource(CambridgeSoft.COE.Framework.COEConfigurationService.DAL DAL)
        {
            this.dal = DAL;
            this.cachedSections = new Dictionary<string, ConfigurationSection>();
        }
        #endregion

        #region IConfigurationSource Members

        public void Add(IConfigurationParameter saveParameter, string sectionName, System.Configuration.ConfigurationSection configurationSection)
        {
            try
            {
                string sectionType = configurationSection.GetType().AssemblyQualifiedName;

                MethodInfo info = configurationSection.GetType().GetMethod("SerializeSection", BindingFlags.NonPublic | BindingFlags.Instance);
                string sectionXml = (string)info.Invoke(configurationSection, new object[] { configurationSection, sectionName, ConfigurationSaveMode.Modified });

                this.dal.AddSection(sectionName, sectionType, sectionXml);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void AddSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            return;
        }

        public System.Configuration.ConfigurationSection GetSection(string sectionName)
        {
            if (cachedSections.ContainsKey(sectionName))
            {
                return cachedSections[sectionName];
            }
            else
            {

                ConfigurationSection configSection = null;
                string sectionXmlStr = string.Empty;
                string sectionTypeClassName = string.Empty;

                sectionXmlStr = this.dal.GetSection(sectionName, ref sectionTypeClassName);

                if (!string.IsNullOrEmpty(sectionXmlStr))
                {
                    Type sectionManagerType = Type.GetType(sectionTypeClassName);
                    configSection = (ConfigurationSection)Activator.CreateInstance(sectionManagerType);

                    StringReader stringReader = new StringReader(sectionXmlStr);

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.CloseInput = true;
                    settings.IgnoreComments = true;
                    settings.IgnoreWhitespace = true;

                    XmlReader reader = XmlReader.Create(stringReader, settings);

                    MethodInfo info = configSection.GetType().GetMethod("DeserializeSection", BindingFlags.NonPublic | BindingFlags.Instance);
                    info.Invoke(configSection, new object[] { reader });
                    reader.Close();
                    this.cachedSections.Add(sectionName, configSection);

                }


                return configSection;
            }
        }

        private Query GetSelectStatement(string sectionName)
        {
            Query query = new Query();
            try
            {
                query.SetMainTable(new Table("Configuration"));


                Field sectionNameField = new Field("SectionName", System.Data.DbType.String);
                SelectClauseField sectionNameSelectField = new SelectClauseField(sectionNameField);
                SelectClauseField sectionXmlSelectField = new SelectClauseField(new Field("SectionXml", System.Data.DbType.String));
                SelectClauseField sectionTypeSelectField = new SelectClauseField(new Field("SectionType", System.Data.DbType.String));
                SelectClauseField sectionTimeStampSelectField = new SelectClauseField(new Field("TimeStamp", System.Data.DbType.DateTime));

                query.AddSelectItem(sectionNameSelectField);
                query.AddSelectItem(sectionXmlSelectField);
                query.AddSelectItem(sectionTypeSelectField);
                query.AddSelectItem(sectionTimeStampSelectField);

                WhereClauseEqual equalConstraint = new WhereClauseEqual();
                equalConstraint.DataField = sectionNameField;
                equalConstraint.Val = new Value(sectionName, System.Data.DbType.String);
                equalConstraint.TrimPosition = SearchCriteria.Positions.Both;
                query.AddWhereItem(equalConstraint);
                return query;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public void Remove(IConfigurationParameter removeParameter, string sectionName)
        {
            this.dal.RemoveSection(sectionName);
        }

        public void RemoveSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            return;
        }

        #endregion
    }
}
