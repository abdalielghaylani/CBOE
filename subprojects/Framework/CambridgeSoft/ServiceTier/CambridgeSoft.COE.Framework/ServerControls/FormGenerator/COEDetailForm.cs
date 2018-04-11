using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public class COEDetailForm : COEForm {
        #region Variables
        private  COEFormMode _currentMode = COEFormMode.ViewMode;
        #endregion

        #region Properties
        public COEFormMode CurrentMode {
            get { return _currentMode; }
            set { _currentMode = value; }
        }
        #endregion

        #region Enumerations
        public enum COEFormMode {
            AddMode = 1,
            EditMode = 2,
            ViewMode = 3
        }
        #endregion
    }
}
