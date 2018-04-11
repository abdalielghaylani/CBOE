using System;

namespace CambridgeSoft.COE.Framework.COESearchService {
    /// <summary>
    /// ResultPageInfo contains information about a page of results requested or received from the server.
    /// </summary>
    public class ResultPageInfo {
        private int _resultSetID;
        private int _pageSize;
        private int _start;
        private int _end;


        /// <summary>
        /// Initializes a new instance of the <see cref="ResultPageInfo"/> class.
        /// </summary>
        public ResultPageInfo() {
            _resultSetID = 0;
            _pageSize = int.MaxValue - 1;
            _start = 1;
            _end = int.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultPageInfo"/> class.
        /// </summary>
        /// <param name="resultSetID">The ResultID</param>
        /// <param name="PageSize">Size of the page</param>
        /// <param name="Start">The starting index of the page</param>
        /// <param name="End">The last index of the page</param>
        public ResultPageInfo(int resultSetID, int PageSize, int Start, int End) {
            _resultSetID = resultSetID;
            _pageSize = PageSize;
            _start = Start;
            _end = End;
        }

        /// <summary>
        /// Gets or sets the result set ID.
        /// </summary>
        /// <value>The result set ID .</value>
        public int ResultSetID {
            get { return _resultSetID; }
            set { _resultSetID = value; }
        }


        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page - number of records requested or returned in the dataresult.</value>
        public int PageSize {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The starting record (index) of a data result. </value>
        public int Start {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The ending record (index) of a data result.</value>
        public int End {
            get { return _end; }
            set { _end = value; }
        }

    }
}
