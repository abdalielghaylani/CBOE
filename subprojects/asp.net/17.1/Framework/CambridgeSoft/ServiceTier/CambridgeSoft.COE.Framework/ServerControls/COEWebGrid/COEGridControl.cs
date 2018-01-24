using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.Xml;
using System.Reflection;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Data;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    [ToolboxData("<{0}:COEWebGrid runat=server></{0}:COEGridViewTestm>")]
    public class COEWebGrid : COEGrid.COEUltraGridView, ICOEGenerableControl, ICOEFullDatasource, ICOEHitMarker
    {
        public COEWebGrid()
        {

        }

        #region ICOEGenerableControl Members

        public new object GetData()
        {
            DataTable result = new DataTable();
            result = this.DataSource as DataTable;
            return result;
        }

        public void PutData(object data)
        {
            base.DataSource = data;
            base.CreateChildControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.MarkingHit += new MarkingHitHandler(COEWebGrid_MarkingHit);
        }

        void COEWebGrid_MarkingHit(object sender, MarkHitEventArgs eventArgs)
        {
            if (MarkingHit != null)
                MarkingHit(sender, eventArgs);
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            base.LoadFromXml(xmlDataAsString);
        }

        public string DefaultValue
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
        #endregion

        #region ICOEFullDatasource Members

        public object FullDatasource
        {
            set { base.FullDatasource = value; }
            get { return base.FullDatasource; }
        }
        #endregion

        #region ICOEHitMarker Members

        public event MarkingHitHandler MarkingHit;

        public string ColumnIDValue
        {
            get
            { return string.Empty;}
            set
            {}
        }

        public string ColumnIDBindingExpression
        {
            get
            { return string.Empty; }
            set
            { }
        }

        #endregion
    }
}
