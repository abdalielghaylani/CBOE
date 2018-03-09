// -----------------------------------------------------------------------
// <copyright file="MathsHelperTestCase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CambridgeSoft.COE.Framework.NUnitTest.SQLGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MathsHelper
    {
        public MathsHelper() { }
        public int Add(int a, int b)
        {
            int x = a + b;
            return x;
        }

        public int Subtract(int a, int b)
        {
            int x = a - b;
            return x;
        }

    }
}
