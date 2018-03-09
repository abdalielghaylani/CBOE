using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemControls
{
    public class ChemControlsEventArgs : EventArgs
    {
        #region Variables
        private string parameter;
        #endregion

        #region Properties
        public string Parameter
        {
            get { return this.parameter; }
        }
        #endregion

        #region Constructors
        // You could add whatever constructor you want and pass your parameters to it
        public ChemControlsEventArgs(string parameter): base()
        {
            this.parameter = parameter;
        }
        #endregion

    }
}
