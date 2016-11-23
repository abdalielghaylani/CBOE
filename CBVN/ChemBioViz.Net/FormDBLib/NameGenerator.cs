using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormDBLib
{
    public class NameGenerator
    {
        #region Variables
        private List<String> m_namelist;
        private List<String> m_namesUsed;
        private Random m_random;
        #endregion

        #region Constructors
        // returns strings from a list in random order
        public NameGenerator(List<String> namelist)
        {
            m_namelist = namelist;
            m_namesUsed = new List<String>();
            m_random = new Random();
        }
        #endregion

        #region Methods
        public String GetNextName()
        {
            // get a random name from a given set
            String s = "";
            int nNames = m_namelist.Count;
            while (true)
            {
                int iRandIndex = m_random.Next(0, nNames);
                s = m_namelist[iRandIndex];
                if (!m_namesUsed.Contains(s))
                {
                    m_namesUsed.Add(s);
                    break;
                }
            }
            // if all names have been used, start over
            if (m_namesUsed.Count >= nNames)
                m_namesUsed.Clear();

            return s;
        }
        #endregion
    }
}
