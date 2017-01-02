using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemBioViz.NET.Utilities
{
    public class KeyGenerator
    {
        private int key = 0;

        public String GetKey()
        {
            key++;
            return Convert.ToString(key);
        }
    }
}
