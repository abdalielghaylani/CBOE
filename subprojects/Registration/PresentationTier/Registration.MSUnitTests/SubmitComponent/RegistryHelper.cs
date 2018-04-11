using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CambridgeSoft.COE.Registration.Services.Types;
using System.Xml;

namespace CambridgeSoft.COE.Registration.MSUnitTests
{
    /// <summary>
    /// Helper class for Registry record
    /// </summary>
    public class RegistryHelper
    {
        #region Methdos

        /// <summary>
        /// Creating and filling RegistryRecord object
        /// </summary>
        /// <param name="Registration_Number">Registry record Number</param>
        /// <returns>RegistryRecord object</returns>
        public static RegistryRecord CreateRegistryObject(string Registration_Number)
        {
            RegistryRecord theRegistryRecord = null;
            try
            {
                theRegistryRecord = RegistryRecord.NewRegistryRecord();
                theRegistryRecord.RegNumber.RegNum = Registration_Number;
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Value = GlobalVariables.New_Structure;//NAPROXEN_SMILES;
                theRegistryRecord.ComponentList[0].Compound.BaseFragment.Structure.Format = "chemical/x-smiles";

                theRegistryRecord.UpdateXml();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(theRegistryRecord.Xml);
            }
            catch (Exception)
            {

            }
            return theRegistryRecord;
        }

        /// <summary>
        /// Loading Registry record from database
        /// </summary>
        /// <param name="searchCriteria">Search crieria could be REGID or REG Number</param>
        /// <param name="registryLoadType">Registry load type could be REGID or REG Number</param>
        /// <returns></returns>
        public static RegistryRecord LoadRegistryRecord(string searchCriteria, GlobalVariables.RegistryLoadType registryLoadType)
        {
            try
            {
                if (registryLoadType == GlobalVariables.RegistryLoadType.REGNUM)
                {
                    return RegistryRecord.GetRegistryRecord(searchCriteria);
                }
                else if (registryLoadType == GlobalVariables.RegistryLoadType.ID)
                {
                    return RegistryRecord.GetRegistryRecord(Convert.ToInt32(searchCriteria));
                }
            }
            catch
            {

            }
            return null;
        }

        #endregion
    }
}
