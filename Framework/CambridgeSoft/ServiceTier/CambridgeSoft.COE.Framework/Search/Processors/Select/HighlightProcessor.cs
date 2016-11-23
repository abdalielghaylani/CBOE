using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COELoggingService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems;
using CambridgeSoft.COE.Framework.Common.SqlGenerator;
using System.Data;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries;
using CambridgeSoft.COE.Framework.Properties;
using CambridgeSoft.COE.Framework.Caching;

namespace CambridgeSoft.COE.Framework.COESearchService.Processors
{
	/// <summary>
	/// A dummy processor for Criteria that don't need specific work. This class follows the template pattern along with search service GetHitList Method
	/// </summary>
	internal class HighlightProcessor : SelectProcessor
	{
		#region Variables
		private XmlNode xmlNode;
		private ResultsCriteria.IResultsCriteriaBase item;
        private SearchCriteria.SearchCriteriaItem scToAppend;
        //private int _hitListID;
        private PagingInfo _pagingInfo;
        private int _hitListTableID;
        private string _tempTableName;
        private SearchCriteria scToModify;
       
        [NonSerialized]
         static COELog _coeLog = COELog.GetSingleton("COESearch");

		#endregion
        
        #region Properties
        /// <summary>
        /// Temporary table name to be used for inserting molecules in the preprocess step, in the case of using CSCartridge.
        /// </summary>
        internal string TempTableName
        {
         get
         {
             return this._tempTableName;
         }
         set
         {
             this._tempTableName = value;
         }
        }

        #endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of Generic Processor with an xmlNode of the SearchCriteria XML.
		/// </summary>
		/// <param name="xmlNode">The node of a SearchCriteriaItem.</param>
		internal HighlightProcessor(XmlNode xmlNode) {
			this.xmlNode = xmlNode;
		}

		/// <summary>
		/// Creates a new instance of Generic Processor with a given SearchCriteria Item
		/// </summary>
		/// <param name="item">The SearchCriteriaItem that is to be handled by this Processor</param>
        internal HighlightProcessor(ResultsCriteria.IResultsCriteriaBase item)
        {
			this.item = item;
            this._tempTableName = ConfigurationUtilities.GetChemEngineSchema(DBMSType.ORACLE) + ".TEMPQUERIES";
		}

        /// <summary>
		/// Creates a new instance of Generic Processor with a given SearchCriteria Item
		/// </summary>
		/// <param name="item">The SearchCriteriaItem that is to be handled by this Processor</param>
        internal HighlightProcessor(ResultsCriteria.IResultsCriteriaBase item, PagingInfo pagingInfo, string tempTableName, SearchCriteria searchCriteria)
        {
            _pagingInfo = pagingInfo;
            _tempTableName = tempTableName;
			this.item = item;
            scToModify = searchCriteria;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Template method Preprocess - performs the required setup steps for executing this command: Nothing in this case.
		/// </summary>
		/// <param name="searchDAL">Search service DAL to be used if required.</param>
        public override void PreProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) 
        {
            //if ((AppDomain.CurrentDomain.GetData("cartridgeVersion") != null) &&  (((int[])AppDomain.CurrentDomain.GetData("cartridgeVersion"))[0] >= 13))
            //{return;}

            Int32 cartMajor = Convert.ToInt32(ConfigurationUtilities.GetCartridgeMajorVersion(DBMSType.ORACLE));
            if (cartMajor < 13)
            { return; }

            string appName = COEAppName.Get();
            if ((appName != string.Empty) && (ConfigurationUtilities.GetApplicationData(appName).SaveQueryHistory.ToLower() == "yes"))
            {
                //TODO....As of now if we aren't storing the searches then this is useless...             
                //Use the hitlist Information to figureout what Search Criteria Table and id to use
                COEHitListService.COEHitListBO hlBO = COEHitListService.COEHitListBO.Get(_pagingInfo.HitListType, _pagingInfo.HitListID);

                //for some reason you couldn't get the hitlist
                if (hlBO == null || hlBO.HitListID < 0)
                    return;

                try
                {
                    SearchCriteria criteria = null;
                    if (hlBO.SearchCriteriaID > 0)
                    {
                        COESearchCriteriaService.COESearchCriteriaBO searchCriteriaBO = COESearchCriteriaService.COESearchCriteriaBO.Get(hlBO.SearchCriteriaType, hlBO.SearchCriteriaID);

                        if (searchCriteriaBO != null)
                            criteria = searchCriteriaBO.SearchCriteria;
                    }
                    //These lines can be uncommented if you whish to allow caching of search criteria without storing it to the database.
                    //If you uncomment these lines comment out the configuration check above
                    //else
                    //{
                    //    criteria = LocalCache.Get(hlBO.ID.ToString() + "_" + hlBO.HitListType.ToString(), typeof(SearchCriteria)) as SearchCriteria;
                    //}

                    if (criteria != null)
                    {
                        ResultsCriteria.HighlightedStructure rc = (ResultsCriteria.HighlightedStructure)item;

                        foreach (SearchCriteria.SearchCriteriaItem sci in criteria.Items)
                        {
                            //check field ids first because its cheap
                            if (rc.Id == sci.FieldId)
                            {
                                //if they match then see if it was a structurecritiera (not molwt or formula or other special structure thing)
                                if (sci.Criterium.GetType().Name == "StructureCriteria")
                                {
                                    scToAppend = sci;
                                }
                            }

                        }
                        //insert structure since we have the DAL
                        if (scToAppend != null)
                        {
                            InsertStructToTemp(searchDAL);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //System
                }
            }
		}

		/// <summary>
		/// If a structure was identified for highlighting then higlight it.
        /// If no structure was highlighted then set Highlight=false to be handled in SelectClauseStructureHighlight
		/// </summary>
		/// <returns></returns>
        public override void Process(ResultsCriteria.IResultsCriteriaBase item)
        {
            System.Diagnostics.Debug.WriteLine("Executed StructureHighlight Process");

            //if you have identified that you have structure info then add it to the searchcriteria
            if (scToAppend != null)
            {
                //turn on highlighting
                ((ResultsCriteria.HighlightedStructure)item).Highlight = true;

                //add a string that has the correct clause
                ((ResultsCriteria.HighlightedStructure)item).MoleculeContainsClause = GetStructureSelect();
                ((ResultsCriteria.HighlightedStructure)item).MoleculeContainsOptions = GetMoleculeContainsOptions();

                //the following commented code could be used to create a filtering criteria including highlighting. 
                //However at this time it is not what we want. I am leaving it in as we may at some point want 
                //to put this back in
                //SearchCriteria.StructureCriteria struccrit = (SearchCriteria.StructureCriteria)scToAppend.Criterium;
                //struccrit.Highlight = SearchCriteria.COEBoolean.Yes;

                //SearchCriteria.SearchCriteriaItem sci = new SearchCriteria.SearchCriteriaItem();
                //sci.Criterium = struccrit;
                //sci.TableId = scToAppend.TableId;
                //sci.FieldId = scToAppend.FieldId;
                //sci.Criterium = struccrit;
                //scToModify.Items.Add(sci);
            }
            else
            {
                //tell the where clause item that we don't have anything to highlight
                //SelectClauseStructureHighlight will create regular Field instead
                ((ResultsCriteria.HighlightedStructure)item).Highlight = false;
            }
		}


        /// <summary>
        /// Molecules the contains options.
        /// Unfortunately this is basically copied from WhereClauseStructure Class
        /// And modified to use properies of append criteria
        /// </summary>
        /// <returns></returns>
        private string GetMoleculeContainsOptions()
        {
            //SearchCriteria.StructureCriteria sc = (SearchCriteria.StructureCriteria)scToAppend;
            //SearchCriteria.StructureCriteria sc = scToAppend.GetSearchCriteriaItem(scToAppend.ID);

            SearchCriteria.StructureCriteria sc = (SearchCriteria.StructureCriteria)scToAppend.Criterium;
            

            StringBuilder builder = new StringBuilder(string.Empty);
            builder.Append("absoluteHitsRel=" + GetStringFromBool(sc.AbsoluteHitsRel));
			builder.Append(", relativeTetStereo=" + GetStringFromBool(sc.RelativeTetStereo));
			builder.Append(", tetrahedralStereo=" + sc.TetrahedralStereo);
			builder.Append(", reactionCenter=" + GetStringFromBool(sc.ReactionCenter));
            builder.Append(", full=" + GetStringFromBool(sc.FullSearch));
            builder.Append(", tautomer=" + GetStringFromBool(sc.Tautometer));
            builder.Append(", fragmentsOverlap=" + GetStringFromBool(sc.FragmentsOverlap));
            builder.Append(", permitExtraneousFragmentsIfRXN=" + GetStringFromBool(sc.PermitExtraneousFragmentsIfRXN));
            builder.Append(", permitExtraneousFragments=" + GetStringFromBool(sc.PermitExtraneousFragments));
            builder.Append(", doubleBondStereo=" + GetStringFromBool(sc.DoubleBondStereo));
            builder.Append(", hitAnyChargeHetero=" + GetStringFromBool(sc.HitAnyChargeHetero));
            builder.Append(", identity=" + GetStringFromBool(sc.Identity));
            builder.Append(", hitAnyChargeCarbon=" + GetStringFromBool(sc.HitAnyChargeCarbon));
			//If we remove the following two lines, we allow substructure highlight when similarity is chosen
			builder.Append(", simThreshold=" + sc.SimThreshold);
            builder.Append(", similar=" + GetStringFromBool(sc.Similar));
            

            //builder.Append(", ignoreImplicitH=" + GetStringFromBool(ignoreImplicitHydrogens));
			return builder.ToString();
		}

        /// <summary>
        /// Gets the structure select.
        /// Unfortunately this is basically copied from StructureProcessor
        /// </summary>
        /// <returns></returns>
        private string GetStructureSelect()
        {
            SearchCriteria.StructureCriteria sc = (SearchCriteria.StructureCriteria)scToAppend.Criterium;
            int primaryKey = 1;

            Query tempSelect = new Query();
            tempSelect.SetMainTable(new Table(TempTableName));

            SelectClauseField queryColumn = new SelectClauseField();
            queryColumn.DataField = new Field("QUERY", DbType.String);

            WhereClauseEqual primaryKeyConstraint = new WhereClauseEqual();
            primaryKeyConstraint.DataField = new Field("ID", DbType.Int32);
            primaryKeyConstraint.Val = new Value(primaryKey.ToString(), DbType.Int32);

            tempSelect.AddSelectItem(queryColumn);
            tempSelect.AddWhereItem(primaryKeyConstraint);

            tempSelect.EncloseInParenthesis = false;
            tempSelect.UseParameters = false;
            string tempSelectString = tempSelect.ToString(); 

            return tempSelectString;
        }

		private string GetStringFromBool(CambridgeSoft.COE.Framework.Common.SearchCriteria.COEBoolean value) {
            if (value == CambridgeSoft.COE.Framework.Common.SearchCriteria.COEBoolean.Yes)
				return "YES";
			else
				return "NO";
		}

        private void InsertStructToTemp(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL)
        {
            int primaryKey = 1;
            SearchCriteria.StructureCriteria sc = (SearchCriteria.StructureCriteria)scToAppend.Criterium;

            Insert insert = new Insert();
            insert.MainTable = new Table(TempTableName);
            insert.Fields.Add(new Field("QUERY", DbType.String));
            insert.Fields.Add(new Field("ID", DbType.Int32));

            insert.ParamValues.Add(new Value(sc.Structure, DbType.String));
            insert.ParamValues.Add(new Value(primaryKey.ToString(), DbType.Int32));

            if (searchDAL.ExecuteNonQuery(insert) != 1)
                throw new Exception("Error when trying to insert temporary molecule - Highlight Processor");
        }

		/// <summary>
		/// Template method PostProcess - No need for clean up.
		/// </summary>
		/// <param name="searchDAL"></param>
        public override void PostProcess(CambridgeSoft.COE.Framework.COESearchService.DAL searchDAL) { }
		#endregion
	}
}
