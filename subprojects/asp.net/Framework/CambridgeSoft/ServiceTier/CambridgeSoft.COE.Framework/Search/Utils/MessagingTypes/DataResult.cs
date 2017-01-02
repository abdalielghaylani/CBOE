using System;


namespace CambridgeSoft.COE.Framework.COESearchService {

    /// <summary>
    /// DataResult contains data results from a calls to the Search class.
    /// </summary>
    /// <remarks>
    /// Typically a user would check the <typeparamref name="Status"/> to confirm the search succeded. Then would use the data contained in <typeparamref name="ResultSet"/> property, transforming it into a DataSet.
    /// </remarks>
    /// <example>
    /// <code language="C#">
    /// DataResult results = target.DoSearch(searchInput, resultFields, rpi);
    /// if(results.Status == "SUCCESS")
    /// {
    ///     DataSet ds = GetDataSetFromXml(results.ResultSet);
    ///     // Your business logic comes here to manipulate/display the information returned from the search.
    /// }
    /// </code>
    /// </example>
    public class DataResult {
        private string _status;
        private string _resultSet;
        private ResultSetInfo _resultSetInfo;
        private ResultPageInfo _resultPageInfo;
		private string _statusDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult"/> class.
        /// </summary>
        public DataResult() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="resultSet">The result set.</param>
        /// <param name="resultSetInfo">The result set info.</param>
        /// <param name="resultPageInfo">The result page info.</param>
        public DataResult(string status, string resultSet, ResultSetInfo resultSetInfo, ResultPageInfo resultPageInfo) {
            _status = status;
            _resultSet = resultSet;
            _resultSetInfo = resultSetInfo;
            _resultPageInfo = resultPageInfo;
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
        /// <remarks>
        /// <para>This string is an xml that can easily be converted to a DataSet using a method like:</para>
        /// <code language="C#">
        /// public static DataSet GetDataSetFromXml(string resultSetXml) {
        ///     DataSet ds = new DataSet();
        ///     if(!string.IsNullOrEmpty(resultSetXml)) {
        ///         using(System.IO.StringReader stringReader = new System.IO.StringReader(resultSetXml)) {
        ///             ds.ReadXml(stringReader);
        ///         }
        ///     }
        ///     return ds;
        /// }
        /// </code>
        /// </remarks>
        /// <value>
        /// An xml document returned as a string that contains the requested result set.
        /// </value>
        public string ResultSet {
            get { return _resultSet; }
            set { _resultSet = value; }
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


		/// <summary>
		/// Gets or sets the status details.
		/// </summary>
		/// <value>The status details.</value>
		public string StatusDetails
		{
			get { return _statusDetails; }
			set { _statusDetails = value; }
		}

    }
}
