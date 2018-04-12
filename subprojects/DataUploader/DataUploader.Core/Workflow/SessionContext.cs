using System.Collections.Generic;

namespace CambridgeSoft.COE.DataLoader.Core.Workflow
{
    /// <summary>
    /// A dictionary containing all information needed for this specific loading session.
    /// </summary>
    public class SessionContext
    {
        private IDictionary<string, object> _items = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SessionContext()
        {
            _items = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets a specific session context item.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <returns>The value of the item</returns>
        public object this[string key]
        {
            get
            {
                return _items[key];
            }
            set
            {
                _items[key] = value;
            }
        }
    }
}
