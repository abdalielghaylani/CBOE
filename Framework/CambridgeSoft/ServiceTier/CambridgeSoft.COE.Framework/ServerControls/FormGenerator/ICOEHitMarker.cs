using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    public interface ICOEHitMarker
    {
        event MarkingHitHandler MarkingHit;

        string ColumnIDValue { get; set; }

        string ColumnIDBindingExpression { get; set; }
    }

    public delegate void MarkingHitHandler(object sender, MarkHitEventArgs eventArgs);
    
    public delegate void MarkAllHitsHandler(object sender, MarkAllHitsEventArgs eventArgs);

    #region MarkHitEventArgs Class
    public class MarkHitEventArgs : EventArgs
    {
        private string _columnIDValue;
        private bool _marked;
        private string _columnIDBindingExpression;
        
        public string ColumnIDValue
        {
            get { return _columnIDValue; }
        }

        public bool Marked
        {
            get { return _marked; }
        }

        public string ColumnIDBindingExpression
        {
            get { return _columnIDBindingExpression; }
        }

        public MarkHitEventArgs(string columnIDValue, bool marked)
        {
            _columnIDValue = columnIDValue;
            _marked = marked;
        }

        public MarkHitEventArgs(string columnIDValue, bool marked, string columnIDBindingExpression)
        {
            _columnIDValue = columnIDValue;
            _marked = marked;
            _columnIDBindingExpression = columnIDBindingExpression;
        }
    }

    public class MarkAllHitsEventArgs : EventArgs
    {
        private bool _marked;

        public bool Marked
        {
            get
            {
                return _marked;
            }
        }

        public MarkAllHitsEventArgs(bool marked)
        {
            _marked = marked;
        }
    }
    #endregion
}
