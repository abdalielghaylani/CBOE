using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems {
    public class WhereClauseStartsWith : WhereClauseLike {
        #region Constructors
        public WhereClauseStartsWith()
            : base() {
            this.dataField = new Field();
            this.val = new Value();
        }
        #endregion

        #region Properties
        public override bool FullWordSearch {
            get {
                return false;
            }
            set { }
        }

        public override bool NormalizeChemicalName {
            get {
                return false;
            }
            set { }
        }

        public override SearchCriteria.Positions WildCardPosition {
            get {
                return SearchCriteria.Positions.Right;
            }
            set { }
        }
        #endregion
    }
}
