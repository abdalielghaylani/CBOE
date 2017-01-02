using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Registration;

/*
NEXT_IN_SEQUENCE: NUMBER(8)
REGNUMBER_LENGTH: NUMBER(3), but should be NUMBER(1)
*/

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    public class NumericSequence : RegistrationBusinessBase<NumericSequence>
    {
        #region [Properties]

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private RegNumberPadding _regPadding;
        public RegNumberPadding RegPadding
        {
            get { return _regPadding; }
            set { _regPadding = value; }
        }

        #endregion

        public enum RegNumberPadding
        {
            None = -1,
            FourDigits = 4,
            FiveDigits = 5,
            SixDigits = 6,
            SevenDigits = 7,
            EightDigits = 8,
            NineDigits = 9
        }

    }
}
