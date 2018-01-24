using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;
using System.Xml;
using CambridgeSoft.COE.Registration.Access;

namespace CambridgeSoft.COE.Registration.UnitTests.Helpers
{
    /// <summary>
    /// Facade by which Unit Tests can interact with the Registration database.
    /// </summary>
    public class Registration
    {
        public const string REGNUM1 = "TT-000001";
        public const string REGNUM2 = "TT-000002";
        public const string NAPROXEN_SMILES = "O=C([C@H](C1=CC2=CC=C(OC)C=C2C=C1)C)[O-]";
        public const string NAPROXEN_SUBSTRUCTURE_SMILES = "C1=CC2=CC=C(OC)C=C2C=C1";

        private static RegistrationOracleDAL _regDal = null;
        private static RegistrationOracleDAL RegDal
        {
            get
            {
                if (_regDal == null)
                    DalUtils.GetRegistrationDAL(ref _regDal, Constants.SERVICENAME);
                return _regDal;
            }
        }

        public static RegistryRecord SubmitSampleCompound(string smilesStructure, string regNumber, bool temporary)
        {
            RegistryRecord result = null;
            //if this exists, return the existing record
            try
            {
                result = RegistryRecord.GetRegistryRecord(regNumber);
                return result;
            }
            catch(Exception ex)
            {

            }

            RegistryRecord rec = RegistryRecord.NewRegistryRecord();
            rec.RegNumber.RegNum = regNumber;
            rec.ComponentList[0].Compound.BaseFragment.Structure.Value = smilesStructure;
            rec.ComponentList[0].Compound.BaseFragment.Structure.Format = "chemical/x-smiles";

            rec.UpdateXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(rec.Xml);

            try
            {
                if (temporary)
                    result = rec.Save(DuplicateCheck.None);
                else
                    result = rec.Register(DuplicateAction.Duplicate);

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public static void DeleteRegisteredRecord(string regNumber)
        {
            RegDal.DeleteRegistryRecord(regNumber);
        }
    }
}
