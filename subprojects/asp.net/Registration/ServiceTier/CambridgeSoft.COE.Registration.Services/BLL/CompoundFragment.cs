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
    public class CompoundFragment : RegistrationBusinessBase<Component>
    {
        private Fragment _fragment;
        public Fragment Fragment
        {
            get
            {
                CanReadProperty(true);
                return _fragment;
            }
            set
            {
                CanWriteProperty(true);
                if (_fragment != value)
                {
                    _fragment = value;
                    PropertyHasChanged();
                }
            }
        }

        //[COEUserActionDescription("CreateNewComponent")]
        public static CompoundFragment NewCompoundFragment(string xml, bool isNew, bool isClean)
        {
            try
            {
                CompoundFragment target = new CompoundFragment(xml, isNew, isClean);

                if (isNew)
                    target.MarkNew();
                else
                    target.MarkOld();

                if (isClean)
                    target.MarkClean();
                else
                    target.MarkDirty();

                target.ValidationRules.CheckRules();
                return target;
            }
            catch (Exception exception)
            {
                COEExceptionDispatcher.HandleBLLException(exception);
                return null;
            }
        }

        public static CompoundFragment NewCompoundFragment(int fragmentId)
        {
            CompoundFragment result = new CompoundFragment();
            result.Fragment = Fragment.NewFragment(fragmentId, null, null, 0, null, 0, null);
            return result;
        }

        private CompoundFragment()
        {
            this.MarkAsChild();
        }

        private CompoundFragment(string xml, bool isNew, bool isClean)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();

            XPathNodeIterator xIterator = xNavigator.Select("Fragment/CompoundFragmentID");
            xIterator.MoveNext();
            int localId = 0;
            int.TryParse(xIterator.Current.Value, out localId);
            this.ID = localId;

            xIterator = xNavigator.Select("Fragment");
            xIterator.MoveNext();
            _fragment = Fragment.NewFragment(xIterator.Current.OuterXml, isNew, isClean);
        }

    }
}
