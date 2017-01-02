using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ChemControls
{
    public class FormCommandsHandler
    {
        GridCommandsHandler gridCommandsHandler;

        public FormCommandsHandler()
        {
            gridCommandsHandler = new GridCommandsHandler();
        }

        ChemDataGrid GetChemDataGrid(Control c)
        {
            Type type = c.GetType();
            PropertyInfo pInfo = type.GetProperty("ChemDataGrid");
            if (pInfo == null)
                return null;
            MethodInfo mInfo = pInfo.GetGetMethod();
            if (mInfo == null)
                return null;
            return mInfo.Invoke(c, null) as ChemDataGrid;
        }

        //        public void UpdateGridView(UltraToolbarsManager manager, Panel p) { ChemDataGrid g = GetChemDataGrid(p); if (g != null) gridCommandsHandler.UpdateGridView(manager, g); }
        //        public void UpdateCardView(UltraToolbarsManager manager, Panel p) { ChemDataGrid g = GetChemDataGrid(p); if (g != null) gridCommandsHandler.UpdateCardView(manager, g); }
    }
}
