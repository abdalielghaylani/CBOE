using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.Common.Messaging;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public class COEFormCollection {
        #region Variables
        private CambridgeSoft.COE.Framework.Common.Messaging.FormGroup.Display displays;
        private int _currentFormIndex;
        private FormGroup.DisplayCollection _displayCollection;
        private FormGroup.DisplayMode _currentDisplayMode;
        #endregion

        #region Properties
        public FormGroup.DisplayCollection Displays
        {
            get
            {
                if (_displayCollection == null)
                    _displayCollection = new FormGroup.DisplayCollection();

                return _displayCollection;
            }
            set
            {
                _displayCollection = value;
            }
        }

        public int CurrentFormIndex {
            get { return _currentFormIndex; }
            set {
                _currentFormIndex = Math.Max(0, Math.Min(value, Displays.Displays.Count - 1));
            }
        }

        public FormGroup.DisplayMode CurrentDisplayMode
        {
            get
            {
                return _currentDisplayMode;
            }
            set
            {
                    _currentDisplayMode = value;
            }
        }
        #endregion

        #region Methods
        internal void LoadControlState(object savedState)
        {
            object[] savedStateArray = (object[])savedState;
            int currentIndex = 0;

            this.CurrentFormIndex = (int)(savedStateArray[currentIndex++]);
            this.CurrentDisplayMode = (FormGroup.DisplayMode)(savedStateArray[currentIndex++]);

        }

        internal object SaveControlState()
        {
            return new object[] { CurrentFormIndex, CurrentDisplayMode };
        }

        public COEFormCollection(FormGroup.DisplayCollection displayCollection)
        {
            this._displayCollection = displayCollection;
            this.CurrentFormIndex = 0;
        }

        public FormGroup.Display this[int index]
        {
            get
            {
                if(index >= 0 && index < _displayCollection.Displays.Count)
                    return this._displayCollection[index];
                
                return null;
            }
            set
            {
                if (index >= 0 && index < _displayCollection.Displays.Count)
                    this._displayCollection[index] = value;
            }
        }
        
        public FormGroup.Display GetCurrent()
        {
            return this[this.CurrentFormIndex];
        }

        #endregion
    }
}
