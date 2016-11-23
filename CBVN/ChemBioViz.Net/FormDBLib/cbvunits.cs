using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace CBVUtilities
{
    public class CBVUnitsManager
    {
        //---------------------------------------------------------------------
        public struct UnitsDef
        {
            public String m_symbol;
            public double m_factor;
            public double m_offset;
            public String m_groupName;
        }
        //---------------------------------------------------------------------
        #region Variables
        private List<UnitsDef> m_unitsDict;
        #endregion

        #region Constructor
        //---------------------------------------------------------------------
        public CBVUnitsManager(String sResourceXml)
        {
            // obtain xml by calling ChemBioVizForm.GetResourceXmlString()
            if (!String.IsNullOrEmpty(sResourceXml))
                Load(sResourceXml);            
        }
        //---------------------------------------------------------------------
        #endregion
        
        #region Methods
        //---------------------------------------------------------------------
        public void Load(String sXml)
        {
            m_unitsDict = new List<UnitsDef>();
            XmlNode root = CBVUtil.LoadXmlString(sXml);
            foreach (XmlNode groupNode in root.ChildNodes)
            {
                if (groupNode.NodeType == XmlNodeType.Whitespace) continue;
                String groupName = CBVUtil.GetStrAttrib(groupNode, "type");
                foreach (XmlNode unitNode in groupNode.ChildNodes)
                {
                    if (unitNode.NodeType == XmlNodeType.Whitespace) continue;
                    UnitsDef udef = new UnitsDef();
                    udef.m_groupName = groupName;
                    udef.m_symbol = CBVUtil.GetStrAttrib(unitNode, "name");
                    udef.m_factor = CBVUtil.StrToDbl(CBVUtil.GetStrAttrib(unitNode, "factor"));
                    udef.m_offset = CBVUtil.StrToDbl(CBVUtil.GetStrAttrib(unitNode, "offset"));
                    m_unitsDict.Add(udef);
                }
            }
        }
        //---------------------------------------------------------------------
        public int FindInComboList(String s)
        {
            // search by symbol and return index; for selecting in combo box
            if (!String.IsNullOrEmpty(s))
            {
                for (int i = 0; i < m_unitsDict.Count; ++i)
                    if (CBVUtil.Eqstrs(s, m_unitsDict[i].m_symbol))
                        return i + 1;   // offset because first item is blank
            }
            return -1;
        }
        //---------------------------------------------------------------------
        public List<String> GetComboList()
        {
            List<String> items = new List<String>();
            items.Add("");
            foreach (UnitsDef udef in m_unitsDict)
            {
                String sDisplay = String.Format("[{0}]: {1}", udef.m_groupName, udef.m_symbol);
                items.Add(sDisplay);
            }
            return items;
        }
        //---------------------------------------------------------------------
        public List<String> GetComboList(String sTargetUnits)
        {
            // return list of units of same group as target
            List<String> items = new List<String>();
            UnitsDef udefTarget = new UnitsDef();
            if (FindBySymbol(sTargetUnits, ref udefTarget))
            {
                foreach (UnitsDef udef in m_unitsDict)
                {
                    if (CBVUtil.Eqstrs(udef.m_groupName, udefTarget.m_groupName))
                        items.Add(udef.m_symbol);
                }
            }
            return items;
        }
        //---------------------------------------------------------------------
        public bool FindBySymbol(String symbol, ref UnitsDef retUdef)
        {
            foreach (UnitsDef udef in m_unitsDict)
            {
                if (udef.m_symbol.Equals(symbol))
                {
                    retUdef = udef;
                    return true;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        private String GetSourceUnitsName(String s, String sSelectedUnits)
        {
            // if s contains units, return those, otherwise use selected
            int iFinal = CBVUtil.FindFinalTokenPos(s, ' ');
            String sUnits = (iFinal >= 0) ? s.Substring(iFinal) : sSelectedUnits;
            return sUnits;
        }
        //---------------------------------------------------------------------
        private bool ParseUnits(ref String s, ref String sUnits)
        {
            // if final token of string is valid units string, return the two parts
            int iFinal = CBVUtil.FindFinalTokenPos(s, ' ');
            if (iFinal >= 0)
            {
                String sFinal = s.Substring(iFinal);
                UnitsDef udef = new UnitsDef();
                if (FindBySymbol(sFinal, ref udef))
                {
                    sUnits = udef.m_symbol;
                    s = s.Substring(0, iFinal - 1);
                    return true;
                }
            }
            return false;
       }
        //---------------------------------------------------------------------
        public String TranslateString(String sRaw, String targetUnits, String sSelectedUnits, ref String sDisplay)
        {
            // given a string with values and units, return modified string with values scaled
            // ex: "20-30 mg" with target units "g" => "0.02-0.03"
            // CSBR-132401: sSelectedUnits is from units combo .. use this if not overridden in sRaw

            String s = sRaw, sUnits = String.Empty;
            sDisplay = s;

            if (ParseUnits(ref s, ref sUnits)) {
                ;
            }
            else if (!String.IsNullOrEmpty(sSelectedUnits) && !CBVUtil.Eqstrs(targetUnits, sSelectedUnits)) {
                sUnits = sSelectedUnits;
                sDisplay = String.Concat(s, " ", sUnits);
            }
            else {
                return s;
            }

            String sOut = s;
            UnitsDef udef = new UnitsDef(), targetUdef = new UnitsDef();
            if (FindBySymbol(sUnits, ref udef) && FindBySymbol(targetUnits, ref targetUdef))
            {
                double dTotFactor = targetUdef.m_factor / udef.m_factor;
                double dTotOffset = targetUdef.m_factor * (udef.m_offset - targetUdef.m_offset);

                sOut = CBVUtil.ScaleNumerics(s, dTotFactor, dTotOffset);
                Debug.WriteLine(String.Format("Input={0} Output={1} Display={2}", sRaw, sOut, sDisplay));
            }
            return sOut;
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
