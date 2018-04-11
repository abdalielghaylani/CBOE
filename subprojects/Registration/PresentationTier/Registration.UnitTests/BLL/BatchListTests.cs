using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    [TestFixture]
    public class BatchListTests
    {

        #region XML

        private const string updateBatchListXml_PropertyChanged = @"<MultiCompoundRegistryRecord SameBatchesIdentity=""True"" ActiveRLS=""Off"" IsEditable=""True"">  
  <BatchList>
    <Batch>
      <BatchID>2</BatchID>
      <BatchNumber>1</BatchNumber>
      <FullRegNumber>AB-000002/01</FullRegNumber>
      <DateCreated>2011-07-22 02:00:02</DateCreated>
      <PersonCreated displayName=""T5_85"">15</PersonCreated>
      <PersonRegistered displayName=""T5_85"">15</PersonRegistered>
      <DateLastModified>2011-08-05 03:11:22</DateLastModified>
      <StatusID/>
      <ProjectList/>
      <IdentifierList/>
      <PropertyList>
        <Property name=""SCIENTIST_ID"" pickListDomainID=""3"" pickListDisplayValue=""T5_85"">15</Property>
        <Property name=""CREATION_DATE"">2011-07-22 12:00:00</Property>
        <Property name=""PURITY"">45</Property>
        <Property name=""BATCH_COMMENT"">changed batch comments</Property>
        <Property name=""STORAGE_REQ_AND_WARNINGS"">src</Property>
        <Property name=""FORMULA_WEIGHT"">254.45126</Property>
        <Property name=""BATCH_FORMULA"">C14H26·1C3H8O</Property>
        <Property name=""PERCENT_ACTIVE"">76.38</Property>
        <Property name=""NOTEBOOK_TEXT""/>
        <Property name=""AMOUNT""/>
        <Property name=""AMOUNT_UNITS""/>
        <Property name=""APPEARANCE""/>
        <Property name=""PURITY_COMMENTS""/>
        <Property name=""SAMPLEID""/>
        <Property name=""SOLUBILITY""/>
      </PropertyList>
      <BatchComponentList>
        <BatchComponent>
          <ID>2</ID>
          <BatchID>2</BatchID>
          <CompoundID>2</CompoundID>
          <MixtureComponentID>2</MixtureComponentID>
          <ComponentIndex>-2</ComponentIndex>
          <PropertyList>
            <Property name=""PERCENTAGE""/>
          </PropertyList>
          <BatchComponentFragmentList>
            <BatchComponentFragment>
              <ID>3</ID>
              <CompoundFragmentID>3</CompoundFragmentID>
              <FragmentID>343</FragmentID>
              <Equivalents>1</Equivalents>
            </BatchComponentFragment>
          </BatchComponentFragmentList>
        </BatchComponent>
      </BatchComponentList>
    </Batch>
  </BatchList>
</MultiCompoundRegistryRecord>";

        #endregion

        private string originalXml = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }


        [Test]
        public void UpdateFromXml_Should_Update_BatchListLevel_PropertyChanged()
        {
            RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000002");

            Assert.IsNotNull(record);

            if (string.IsNullOrEmpty(originalXml))
                originalXml = record.Xml;

            record.UpdateFromXml(updateBatchListXml_PropertyChanged);

            record.Save();

            RegistryRecord updatedRecord = RegistryRecord.GetRegistryRecord("AB-000002");

            string expectedCommnents = "changed batch comments";

            Assert.AreEqual(expectedCommnents, updatedRecord.BatchList[0].PropertyList["BATCH_COMMENT"].Value);

            if (!string.IsNullOrEmpty(originalXml))
            {
                updatedRecord.Xml = originalXml;
                updatedRecord.Save();
            }
        }


        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
