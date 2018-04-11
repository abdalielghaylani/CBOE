using System;

namespace CambridgeSoft.COE.Framework.COESearchService {

    /// <summary>
    /// DataListResult contains primarily, the primary key values of the records that matched a search using calls to <see cref="COESearch"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typically a user would check the <typeparamref name="Status"/> to confirm the search succeded. Then would use the data contained in <typeparamref name="ResultList"/> property.
    /// </para>
    /// <para>
    /// Additionally, if this result comes from a SIMILARITY search, a user could use the <see cref="SimilarityScores"/> attribute looking for some particular similarity score.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code language="C#">
    /// DataListResult results = target.DoSearch(searchInput, "SUBSTANCE.ACX_ID", rpi);
    /// if(results.Status == "SUCCESS")
    /// {
    ///     foreach(string pkMatch in results.ResultList)
    ///     {
    ///         // Your business logic comes here to manipulate/display the information returned from the search.
    ///     }
    /// }
    /// </code>
    /// </example>
    public class DataListResult {
        private string _status;
        private string[] _resultList;
        private ResultSetInfo _resultSetInfo;
        private ResultPageInfo _resultPageInfo;
        private string[] _similarityScores;
		private string _statusDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult"/> class.
        /// </summary>
        public DataListResult() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="resultList">The result list.</param>
        /// <param name="similarityScores">NULL for non-similarity searches. For similarity searches an array of scores of equal length to the result list.</param>
        /// <param name="resultSetInfo">The result set info.</param>
        /// <param name="resultPageInfo">The result page info.</param>
        public DataListResult(string status, string[] resultList, string[] similarityScores, ResultSetInfo resultSetInfo, ResultPageInfo resultPageInfo) {
            _status = status;
            _resultList = resultList;
            _similarityScores = similarityScores;
            _resultSetInfo = resultSetInfo;
            _resultPageInfo = resultPageInfo;
			_statusDetails = String.Empty;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>SUCCESS | FAILURE: Error Message</value>
        public string Status {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// Gets or sets the result set.
        /// </summary>
        /// <value>
        /// An string array 
        /// </value>
        public string[] ResultList {
            get { return _resultList; }
            set { _resultList = value; }
        }

        /// <summary>
        /// Gets or sets the similarity scores.
        /// </summary>
        /// <value>NULL for non-similarity searches. For similarity searches an array of scores of equal length to the result list.</value>
        public string[] SimilarityScores {
            get { return _similarityScores; }
            set { _similarityScores = value; }
        }

        /// <summary>
        /// Gets or sets the result set info.
        /// </summary>
        /// <value>The result set info. <see cref="ResultSetInfo"/></value>
        public ResultSetInfo resultSetInfo {
            get { return _resultSetInfo; }
            set { _resultSetInfo = value; }
        }

        /// <summary>
        /// Gets or sets the result page info.
        /// </summary>
        /// <value>The result page info. <see cref="ResultPageInfo"/></value>
        public ResultPageInfo resultPageInfo {
            get { return _resultPageInfo; }
            set { _resultPageInfo = value; }
        }

		public string StatusDetails
		{
			get { return _statusDetails; }
			set { _statusDetails = value; }
		}
    }
}
