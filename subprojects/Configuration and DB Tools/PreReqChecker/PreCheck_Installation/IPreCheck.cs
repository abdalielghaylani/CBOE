using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PreCheck_Installation
{
    interface IPreCheck
    {
        #region Properties
        string PreCheckName
         {
              get;
              set;
         }
        Hashtable PreCheckSubList
        {
            get;
            set;
        }
        Hashtable Status
        {
            get;
            set;
        }
        Hashtable ExpectedValueList
        {
            get;
            set;
        }
        Hashtable ActualValue
        {
              get;
              set;
         }
        Hashtable Message
        {
            get;
            set;
        }
        #endregion

        #region Methods
        void AssignPreCheckSubList();
        void AssignExpectedList();
        void GetActualResult();
        void CompareExpectedResult();
        void DisplayMessage();
        #endregion

    }
}
