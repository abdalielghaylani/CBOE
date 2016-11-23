using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.XPath;

using CambridgeSoft.COE.Registration.Services.Common;
using CambridgeSoft.COE.Framework.ExceptionHandling;

namespace CambridgeSoft.COE.Registration.Services.Types
{
    [Serializable()]
    public class CompoundFragmentList : RegistrationBusinessListBase<CompoundFragmentList, CompoundFragment>
    {
        //[COEUserActionDescription("CreateComponentList")]
        public static CompoundFragmentList NewCompoundFragmentList(string xml, bool isNew, bool isClean)
        {
            try
            {
                CompoundFragmentList list = new CompoundFragmentList(xml, isNew, isClean);
                return list;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        private CompoundFragmentList(string xml, bool isNew, bool isClean)
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("FragmentList/Fragment");
            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(CompoundFragment.NewCompoundFragment(xIterator.Current.OuterXml, isNew, isClean));
                } while (xIterator.Current.MoveToNext());
            }
        }

        /// <summary>
        /// For use by other members of this assembly.
        /// </summary>
        /// <returns></returns>
        internal static CompoundFragmentList NewCompoundFragmentList()
        {
            return new CompoundFragmentList();
        }

        private CompoundFragmentList() { }

        public CompoundFragment GetById(int compoundFragmentId)
        {
            CompoundFragment result = null;
            foreach (CompoundFragment cf in this)
            {
                if (cf.ID == compoundFragmentId)
                {
                    result = cf;
                    break;
                }
            }
            return result;
        }

    }
}
