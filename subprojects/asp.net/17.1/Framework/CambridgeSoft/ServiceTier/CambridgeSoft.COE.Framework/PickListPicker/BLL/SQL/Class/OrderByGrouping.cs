using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CambridgeSoft.COE.Framework.COEPickListPickerService
{
    /// <summary>
    /// Class for building order by grouping used in PickListDomain tables.
    /// </summary>
    public class OrderByGrouping : IOrderByGrouping
    {
        #region ENUM PRIVATE
        /// <summary>
        /// Enum check .
        /// </summary>
        private enum Check
        {
            Count, NoCount, Empty, NotEmpty, None
        }
        #endregion

        #region CONSTANTS  PRIVATE
        private const string ORDER = "ORDER BY";
        private const string ASC = "ASC";
        private const string DESC = "DESC";
        #endregion

        #region Variables PRIVATE
        private string _constructedQuery = string.Empty;
        private string _conditionClause = string.Empty;
        private string _orderClause = string.Empty;
        private List<string> _sqlFilterList;
        private List<string> _orderClauseList;
        private string _orderByQuery = string.Empty;
        private string _sortOrder = string.Empty;
        #endregion

        #region Properties PRIVATE
        /// <summary>
        /// Any filter statement (order by clause) required to derive the proper subset
        /// of records serving as this picklist's item-set.
        /// </summary>
        private List<string> SqlFilter
        {
            get
            {
                if (_sqlFilterList == null)
                    _sqlFilterList = new List<string>();
                return _sqlFilterList;
            }
            set
            {
                _sqlFilterList = value;
            }
        }

        /// <summary>
        /// Contains columns used in order by grouping
        /// </summary>
        public List<string> OrderClauseList
        {
            get
            {
                if (_orderClauseList == null)
                    _orderClauseList = new List<string>();
                return _orderClauseList;
            }
            set
            {
                _orderClauseList = value;
            }
        }
        
        private string OrderClause
        {
            get
            {
                return _orderClause;
            }
            set
            {
                _orderClause = value;
                if (!string.IsNullOrEmpty(_orderClause))
                    OrderClauseList.Add(_orderClause);
            }
        }

        public string OrderByQuery
        {
            get
            {
                return _orderByQuery;
            }
            set
            {
                _orderByQuery = value;
            }
        }

        private string SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }
        }
        #endregion

        #region Properties PUBLIC

        /// <summary>
        /// Implements ISqlBuilder
        /// </summary>
        public string GetQuery
        {
            get
            {
                this.BuildQuery();
                return _constructedQuery;
            }
        }

        #endregion

        #region Methods Private
        /// <summary>
        /// Get the columns used in order by statement
        /// <param name="filter">the filter to look in</param>
        /// </summary>
        private string GetORDERBYConditions(string filter)
        {
            string retVal = string.Empty;
            if (filter.ToUpper().Contains(ORDER))
            {
                retVal = GetToken(filter, ORDER, "");
                if (retVal.ToUpper().Contains(ASC))
                {
                    _sortOrder = ASC;
                    retVal = retVal.ToUpper().Replace(ASC, "");
                }
                if (retVal.ToUpper().Contains(DESC))
                {
                    _sortOrder = DESC;
                    retVal = retVal.ToUpper().Replace(DESC, "");
                }
            }
            return retVal;
        }

        /// <summary>
        /// Get the join sql, with performing valid check 
        /// <param name="joinWith">the seperator to join array list</param>
        /// <param name="ToJoin">the array to join</param>
        /// <param name="check">the validation to join</param>
        /// <returns>Constrcted string using array , seperator</returns>
        /// </summary>
        private string JOIN(string joinWith, object ToJoin, Check check)
        {
            Type type = ToJoin.GetType();
            string retVal = string.Empty;

            switch (type.Name.ToUpper())
            {
                case "LIST`1":
                    List<string> arrayToJoin = (List<string>)ToJoin;
                    switch (check)
                    {
                        case Check.Count:
                            if (arrayToJoin.Count == 1)
                                joinWith = "";
                            retVal = string.Join(joinWith, arrayToJoin.ToArray());
                            break;
                        case Check.NoCount:
                            retVal = string.Join(joinWith, arrayToJoin.ToArray());
                            break;
                    }
                    break;
                case "STRING":
                    string stringToJoin = (string)ToJoin;
                    switch (check)
                    {
                        case Check.NotEmpty:
                            if (!string.IsNullOrEmpty(stringToJoin))
                                retVal = joinWith + stringToJoin;
                            break;
                        case Check.Empty:
                            retVal = joinWith + stringToJoin;
                            break;
                    }
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Split the conditions in to whereClause and Grouping. 
        /// </summary>
        private void SetClause()
        {
            foreach (string filter in SqlFilter)
            {
                OrderClause = GetORDERBYConditions(filter);
            }

            OrderByQuery = JOIN(",", OrderClauseList, Check.Count);
            OrderByQuery = JOIN(" " + ORDER, OrderByQuery, Check.NotEmpty);
        }

        /// <summary>
        /// Build select statement query. 
        /// </summary>
        private void BuildQuery()
        {
            _constructedQuery = OrderByQuery + SortOrder;
        }

        /// <summary>
        /// Gets the valid token in between start and end . 
        /// <param name="token">the token on which seperation to be performed</param>
        /// <param name="start">the starting split range</param>
        /// <param name="end">the end split range</param>
        /// <returns>The available string in given range</returns>
        /// </summary>
        private string GetToken(string token, string start, string end)
        {
            string retVal = string.Empty;
            retVal = token;
            if (start == string.Empty)
            {
                start = "$$SQLSTART$$ ";
                token = start + token;
            }
            if (end == string.Empty)
            {
                end = " $$SQLEND$$";
                token = token + end;
            }
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end), RegexOptions.IgnoreCase);
            MatchCollection matches = r.Matches(token);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                foreach (Group group in groups)
                    retVal = group.Value;
            }
            return retVal;
        }
        #endregion

        #region Constructors PRIVATE
        private OrderByGrouping(List<string> sqlFilter)
        {
            this.SqlFilter.Clear();
            this.SqlFilter.AddRange(sqlFilter);
        }
        #endregion

        #region Constructors PUBLIC
        public OrderByGrouping(IOrderByGrouping orderByGrouping)
            : this(orderByGrouping.OrderClauseList)
        {
            this.SetClause();
        }
        #endregion
    }
}
