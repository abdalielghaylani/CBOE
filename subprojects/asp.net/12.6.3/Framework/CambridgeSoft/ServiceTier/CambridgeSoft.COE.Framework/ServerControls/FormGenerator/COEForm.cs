using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public class COEForm : CompositeDataBoundControl {
        #region Variables
        private List<COEFormGenerator> _formGenerators;
        #endregion

        #region Properties
        public List<COEFormGenerator> FormGenerators {
            get {
                return _formGenerators;
            }
            set {
                _formGenerators = value;
            }
        }
        #endregion

        #region LifeCycle Events
        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding) {
            foreach(COEFormGenerator formGenerator in FormGenerators) {
                this.Controls.Add(formGenerator);
                if(!string.IsNullOrEmpty(this.DataSourceID))
                    formGenerator.DataSourceID = this.DataSourceID;
                else
                    formGenerator.DataSource = this.DataSource;
            }
            return FormGenerators.Count;
        }
        #endregion
    }
}
