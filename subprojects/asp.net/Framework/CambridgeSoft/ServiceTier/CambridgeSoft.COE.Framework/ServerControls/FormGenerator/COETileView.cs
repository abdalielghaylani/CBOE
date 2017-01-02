using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.ServerControls.TileView;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public class COETileView : TileView, ICOEGenerableControl {
        #region ICOEGenerableControl Members

        public object GetData() {
            return (System.Data.DataSet) DataSource;
        }

        public void PutData(object data) {
            DataSource = data;
            this.DataBind();
        }

        public void LoadFromXml(string xmlDataAsString) {
            base.LoadFromXml(xmlDataAsString);
        }

        public string DefaultValue {
            get {
                return "";
            }
            set {
                ;
            }
        }

        #endregion
    }
}
