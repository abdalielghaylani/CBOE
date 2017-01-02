using System;
using System.Collections.Generic;
using System.Text;

namespace FormDBLib
{
    public class CBVEventArgs : EventArgs
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
        public CBVEventArgs(string parameter) : base()
        {
            this.parameter = parameter;
        }
        #endregion

    }
}
