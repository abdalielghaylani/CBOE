using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.DataLoaderGUI.Common
{
    /// <summary>
    /// Utility class used to help store and retrieve extra information in controls (should be COEItemPair)
    /// </summary>
    /// <typeparam name="T">Type holding the extra information</typeparam>
    public class COEItemPair<T>
    {
        #region data
        private string _Key;
        private T _Value;
        #endregion

        #region properties
        /// <summary>
        /// Expose the value
        /// </summary>
        public T Value
        {
            get
            {
                return _Value;
            }
        } // Value
        #endregion

        #region constructors
        /// <summary>
        /// Useful to store multiple typed values in UI properies such as a Tag
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="nValue"></param>
        public COEItemPair(string strKey, T nValue)
        {
            _Key = strKey;
            _Value = nValue;
            return;
        } // COEItemPair()
        #endregion

        #region methods
        /// <summary>
        /// Override ToString so the key appears visually in the UI
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _Key;
        } // ToString()
        #endregion
    } // class COEItemPair
}
