using System;

namespace CambridgeSoft.COE.Framework.COESearchService {
    /// <summary>
    /// Input parameters required to perform a search.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The search filters are specified by adding search expressions to the <see cref="FieldCriteria"/> list. Additionally a domain could be specified by setting the <see cref="DomainFieldName"/> and the list of values that define the <see cref="Domain"/>.
    /// </para>
    /// <para>
    /// If a STRUCTURE search is being part of the filter, it is possible to indicate its options through the <see cref="SearchOptions"/> attribute to have better control of it.
    /// </para>
    /// <para>
    /// Is in this object also where the user can specify, using the <see cref="ReturnPartialResults"/> attribute, if the search would be performed in a partial form, which can improve response times in certain circumstances.
    /// </para>
    /// </remarks>
    public class SearchInput {
        private string[] _fieldCriteria;
        private string[] _domain;
        private string _domainFieldName;
        private string[] _searchOptions;
        private bool _returnPartialResults;
        private bool _returnSimiliarityScores;
        private bool _avoidHitList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchInput"/> class.
        /// </summary>
        public SearchInput()
        {
            _fieldCriteria = new string[0];
            _domain = new string[0];
            _searchOptions = new string[0];
            _returnPartialResults = false;
            _domainFieldName = string.Empty;
            _returnSimiliarityScores = false;
            _avoidHitList = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchInput"/> class.
        /// </summary>
        /// <param name="fieldCriteria">The field criteria.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="domainFieldName">The fieldName that defines the domain.</param>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="returnPartialResults">The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server.  The limit can be configured differently for "narrow" resultsets (1 or 2 integer fields) than for "wide" multi field/fieldtype result sets.  The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.</param>
        public SearchInput(string[] fieldCriteria, string[] domain, string domainFieldName, string[] searchOptions, bool returnPartialResults)
            : this()
        {
            _fieldCriteria = fieldCriteria;
            _domain = domain;
            _domainFieldName = domainFieldName;
            _searchOptions = searchOptions;
            _returnPartialResults = returnPartialResults;
            _returnSimiliarityScores = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchInput"/> class.
        /// </summary>
        /// <param name="fieldCriteria">The field criteria.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="domainFieldName">The fieldName that defines the domain.</param>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="returnPartialResults">The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server.  The limit can be configured differently for "narrow" resultsets (1 or 2 integer fields) than for "wide" multi field/fieldtype result sets.  The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.</param>
        public SearchInput(string[] fieldCriteria, string[] domain, string domainFieldName, string[] searchOptions, bool returnPartialResults, bool returnSimilarityScores)
            : this()
        {
            _fieldCriteria = fieldCriteria;
            _domain = domain;
            _domainFieldName = domainFieldName;
            _searchOptions = searchOptions;
            _returnPartialResults = returnPartialResults;
            _returnSimiliarityScores = false;
            _returnSimiliarityScores = returnSimilarityScores;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchInput"/> class.
        /// </summary>
        /// <param name="fieldCriteria">The field criteria.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="domainFieldName">The fieldName that defines the domain.</param>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="returnPartialResults">The query will proceed to completion before any results are returned.  The requested page size (or a smaller page) will be returned to the caller upon completion of the query.  More pages can be returned with subsequent calls to GetDataPage() method.  The reasons for receiveing smaller than requested page size are:  1) query generated insufficient hits, 2) size of the resultset exceeds limits set on the server.  The limit can be configured differently for "narrow" resultsets (1 or 2 integer fields) than for "wide" multi field/fieldtype result sets.  The caller can ascertain why a smaller page size was returned by comparing the ResultSetInfo.TotalCount to the returned ResultPageInfo.PageSize.  ResultSetInfo.CurrentCount will always match ResultSetInfo.TotalCount.</param>
        /// <param name="returnSimilarityScores">Indicates if similarity scores should be returned when possible</param>
        /// <param name="avoidHitList">Indicates if HitList insertions should be avoided</param>
        public SearchInput(string[] fieldCriteria, string[] domain, string domainFieldName, string[] searchOptions, bool returnPartialResults, bool returnSimilarityScores, bool avoidHitList)
            : this()
        {
            _fieldCriteria = fieldCriteria;
            _domain = domain;
            _domainFieldName = domainFieldName;
            _searchOptions = searchOptions;
            _returnPartialResults = returnPartialResults;
            _returnSimiliarityScores = false;
            _returnSimiliarityScores = returnSimilarityScores;
            _avoidHitList = avoidHitList;
        }

        /// <summary>
        /// Gets or sets the field criteria.
        /// </summary>
        /// <value>Field Criteria array consisting of Name Operator Value</value>
        /// <remarks>
        /// <para>
        /// Supported operators
        /// [TODO:] Further specify the operators to include numerical range, wild card text, case sensitivity, list search, and other field criteria supported by the COEFramework.
        /// </para>
        /// 	<list type="table">
        /// 		<listheader><term>Operator</term><description>Description</description></listheader>
        /// 		<item><term>EXACT</term><description>Structure Field Only</description></item>
        /// 		<item><term>SUBSTRUCTURE</term><description>Structure Field Only</description></item>
        /// 		<item><term>SIMILARITY</term><description>Structure Field Only: If similarity is specified SIMTHRESHOLD should be defined in search options</description></item>
        ///         <item><term>FULL</term><description>Structure Field Only</description></item>
        ///         <item><term>IDENTITY</term><description>Structure Field Only</description></item>
		/// 		<item><term>MOLWEIGHT</term><description>Structure field only</description></item>
		///			<item><term>Equals</term><description>Text - Case sensitive</description></item>
		///			<item><term>equals</term><description>Text - Case insensitive</description></item>
		/// 		<item><term>Contains</term><description>Text - Case sensitive</description></item>
		///			<item><term>contains</term><description>Text - Case insensitive</description></item>
		/// 	    <item><term>StartsWith</term><description>Text - Case sensitive</description></item>
		///			<item><term>startsWith</term><description>Text - Case insensitive</description></item>
		/// 	    <item><term>EndWith</term><description>Text - Case sensitive</description></item>
		///			<item><term>endWith</term><description>Text - Case insensitive</description></item>
		///			<item><term>NotContains</term><description>Text - Case sensitive</description></item>
		///			<item><term>notContains</term><description>Text - Case insensitive</description></item>
		///			<item><term>=</term><description>Number</description></item>
        /// 		<item><term>!=</term><description>Number</description></item>
        /// 		<item><term>&lt;&gt;</term><description>Number</description></item>
        /// 		<item><term>&gt;</term><description>Number</description></item>
        /// 		<item><term>&lt;</term><description>Number</description></item>
        /// 		<item><term>BETWEEN</term><description>Number</description></item>
		///			<item><term></term></item>
        /// 	</list>
        /// </remarks>
		/// <para>
		///		OR searching can be performed in the following manner:
		///		TABLEALIAS.FIELDALIAS OPERATOR VALUE [{OR OPERATOR VALUE}]
		///		[ : optional
		///		{ : denotes repetition
		/// </para>
        /// <example>
        /// 	<code>
        /// //ex. string criteria 
		/// //case sensitive search
		/// SUBSTANCE.CHEMICALNAME Equals benzene
		/// 
		/// //case insensitive search
		/// SUBSTANCE.CHEMICALNAME equals benzene
        /// 
        /// //ex. number criteria
		/// SUBSTANCE.CSNUM &gt; 100
		/// SUBSTANCE.CSNUMCSNUM BETWEEN 100 AND 110
        /// 
        /// //ex. structure criteria
        /// SUBSTANCE.STRUCTURE EXACT c1ccccc1
		/// 
		/// //ex. molweight range search
		/// SUBSTANCE.STRUCTURE MOLWEIGHT 50-51
		/// 
		/// //ex. OR searching
		/// SUBSTANCE.CSNUM =6833 OR =6836
        /// </code>
        /// </example>
        public string[] FieldCriteria {
            get { return _fieldCriteria; }
            set { _fieldCriteria = value; }
        }

        /// <summary>
        /// Gets or sets the domain to limit searching.  The domain is defined by an array of values for the field specified in <see cref="DomainFieldName"/> attribute.
        /// [TODO:] Extend to also allow a ResultSet.ID to define the domain.
        /// </summary>
        /// <value>A string array of primary keys</value>
        public string[] Domain {
            get { return _domain; }
            set { _domain = value; }
        }

        /// <summary>
        /// Specifies the field to be used while using Domain values.
        /// </summary>
        /// <value>The fieldName to be used while serching over a Domain.</value>
        public string DomainFieldName {
            get { return _domainFieldName; }
            set { _domainFieldName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether to return the first page of results as soon as they are ready or wait until the search has been completed.
        /// </summary>
        /// <value>
        /// When set to true first page of results are returned as soon as they are available.
        /// </value>
        public bool ReturnPartialResults {
            get { return _returnPartialResults; }
            set { _returnPartialResults = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wether to return the list of similarity scores when a similarity search is performed.
        /// </summary>
        /// <value>
        /// When set to true the list of similarity scores would be populated.
        /// </value>
        public bool ReturnSimilarityScores {
            get { return _returnSimiliarityScores; }
            set { _returnSimiliarityScores = value; }
        }

        /// <summary>
        /// Gets or sets the search options.
        /// Search Options are passed as Name=Value
        /// </summary>
        /// <value>
        /// <list type="table">
        /// <listheader>Supported Structure Search Options:
        /// <term>Parameter</term><description>Values</description>
        /// </listheader>  
        /// <item><term>ABSOLUTEHITSREL</term><description>[YES | NO] - Default is NO</description></item>
        /// <item><term>COMPLETE</term><description>number - to limit the number of hits tested against atom by atom matching</description></item>
        /// <item><term>DOUBLEBONDSTEREO</term><description>[YES|NO] - double bond stereo matching. Default is YES.</description></item>
        /// <item><term>FRAGMENTSOVERLAP</term><description>[YES|NO] - fragments can overlap. Default is NO.</description></item>
        /// <item><term>FULL</term><description>[YES|NO] - to specify full structure search. Default is substructure search</description></item>
        /// <item><term>IDENTITY</term><description>[YES | NO] - Default is NO.  If IDENTITY is passed it overrides the searchType parameter.</description></item>
        /// <item><term>SIMILAR</term><description>[YES|NO] - similarity search. Default is NO.</description></item>
        /// <item><term>HITANYCHARGECARBON</term><description>[YES|NO] - hit any charged carbon. Default is YES.</description></item>
        /// <item><term>HITANYCHARGEHETERO</term><description>[YES|NO] - hit any charged hetero atom option. Default is YES.</description></item>
        /// <item><term>MASSMAX</term><description>number- to prescribe maximum molecular mass to matching records</description></item>
        /// <item><term>MASSMIN</term><description>number - to prescribe minimum molecular mass to matching records</description></item>
        /// <item><term>PERMITEXTRANEOUSFRAGMENTS</term><description>[YES |NO] - permit extraneous fragments. Default is NO.</description></item>
        /// <item><term>PERMITEXTRANEOUSFRAGMENTSIFRXN</term><description>[YES |NO] - permit extraneous fragments. Default is NO.</description></item>
        /// <item><term>REACTIONCENTER</term><description>[YES|NO] - to specify if reaction search can match only reaction centers. Default is YES.</description></item>
        /// <item><term>RELATIVETETSTEREO</term><description>[YES | NO] - Default is NO</description></item>
        /// <item><term>SIMTHRESHOLD</term><description>number - to set the similarity percentage when SIMILARITY=YES is set as well. Otherwise disregarded. Default value used if it is omitted) is 90 percent.</description></item>
        /// <item><term>TAUTOMER</term><description>[YES|NO] - Default is NO</description></item>
        /// <item><term>TETRAHEDRALSTEREO</term><description>[YES|NO|SAME|EITHER|ANY] - tetrahedral stereo matching. Default is YES.</description></item>
        /// <item><term>IGNOREIMPLICITH</term><description>[YES|NO] if set to YES implicit hydrogenes are ignored. Default is NO.</description></item>
        /// <item><term>LOOSEDELOCALIZATION</term><description>[YES|NO]. Default is NO.</description></item>
        /// </list>
        /// </value>
        public string[] SearchOptions {
            get { return _searchOptions; }
            set { _searchOptions = value; }
        }

        /// <summary>
        /// Gets or sets if the search should avoid inserting into hitlist table, that is, doing a browse like search.
        /// </summary>
        public bool AvoidHitList
        {
            get { return _avoidHitList; }
            set { _avoidHitList = value; }
        }
    }
}
