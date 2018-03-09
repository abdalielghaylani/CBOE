using System;

namespace CambridgeSoft.COE.Framework.COESearchService {
    /// <summary>
    /// ResultSetInfo contains information about a result set that has been cached on the server by a previous call to the DoSearch() method.
    /// </summary>
    public class ResultSetInfo {
        private int _id;
        private int _estimatedCount;
        private int _currentCount;
        private int _totalCount; //null until done
        private bool _isComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSetInfo"/> class.
        /// </summary>
        public ResultSetInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSetInfo"/> class.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="EstimatedCount">The estimated count.</param>
        /// <param name="CurrentCount">The current count.</param>
        /// <param name="TotalCount">The total count.</param>
        public ResultSetInfo(int ID, int EstimatedCount, int CurrentCount, int TotalCount) {
            _id = ID;
            _estimatedCount = EstimatedCount;
            _currentCount = CurrentCount;
            _totalCount = TotalCount;
            _isComplete = CurrentCount == TotalCount;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The handle to result set.  The ID that can be used to get more hits using GetDataPage.</value>
        public int ID {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets the estimated count.
        /// </summary>
        /// <value>The estimated number of records in the result set.  An estimated hit count may be provided ahead of the actual count being defined at the end of the search. Note this feature is not yet available in the COEFramework.</value>
        public int EstimatedCount {
            get { return _estimatedCount; }
            set { _estimatedCount = value; }
        }

        /// <summary>
        /// Gets or sets the current count.
        /// </summary>
        /// <value>The current number of records that have been identified in the result set.
        /// Will change until the query has been completed.
        /// </value>
        public int CurrentCount {
            get { return _currentCount; }
            set { _currentCount = value; }
        }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>The total count of records in the result set. This will be null until the query has finished.</value>
        public int TotalCount {
            get { return _totalCount; }
            set { _totalCount = value; }
        }

        /// <summary>
        /// Gets or sets if the partial search is complete.
        /// </summary>
        /// <value>True if the partial search is complete, false otherwise</value>
        public bool IsComplete {
            get { return _isComplete; }
            set { _isComplete = value; }
        }
    }
}
