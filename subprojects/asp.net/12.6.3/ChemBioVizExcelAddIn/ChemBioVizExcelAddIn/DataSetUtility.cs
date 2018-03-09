using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ChemBioVizExcelAddIn
{
    class DataSetUtility
    {
        # region _  Variables  _

        public DataSet ds;

        #endregion

        #region _  Constructor  _

        public DataSetUtility(ref DataSet DataSet)
        {
            ds = DataSet;
        }
        public DataSetUtility()
        {
            ds = null;
        }

        #endregion

        #region _  Create Relation between DataTables _

        public void CreateRelation(string Relation, int FirstTableId, int FirstColumnId, int SecondTableId, int SecondColumnId)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableId].Columns[FirstColumnId], ds.Tables[SecondTableId].Columns[SecondColumnId]);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
            }
        }

        public void CreateRelation(string Relation, int FirstTableId, string FirstColumnName, int SecondTableId, string SecondColumnName)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableId].Columns[FirstColumnName], ds.Tables[SecondTableId].Columns[SecondColumnName]);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
            }
        }

        public void CreateRelation(string Relation, string FirstTableName, int FirstColumnId, string SecondTableName, int SecondColumnId)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableName].Columns[FirstColumnId], ds.Tables[SecondTableName].Columns[SecondColumnId]);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
            }
        }

        public void CreateRelation(string Relation, string FirstTableName, string FirstColumnName, string SecondTableName, string SecondColumnName)
        {
            try
            {
                ds.Relations.Add(Relation, ds.Tables[FirstTableName].Columns[FirstColumnName], ds.Tables[SecondTableName].Columns[SecondColumnName]);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
            }
        }

        #endregion


    }
}
