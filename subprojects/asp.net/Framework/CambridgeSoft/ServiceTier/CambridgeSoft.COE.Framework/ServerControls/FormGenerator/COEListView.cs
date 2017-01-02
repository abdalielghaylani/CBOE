using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Controls.COEWebGrid;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    public class COEListView : COEGrid.COEUltraGridView, ICOEGenerableControl
    {
        #region ICOEGenerableControl Members

        public object GetData()
        {
            return (System.Data.DataSet)DataSource;
        }

        public void PutData(object data)
        {
            DataSource = data;
            this.DataBind();
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            base.LoadFromXml(xmlDataAsString);
        }

        public string DefaultValue
        {
            get
            {
                return "";
            }
            set
            {
                ;
            }
        }

        #endregion

    }
}
