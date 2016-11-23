using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.RegistrationAdminWebApp;
using CambridgeSoft.COE.Registration.Services;
using CambridgeSoft.COE.Registration.Services.Types;
using CambridgeSoft.COE.Framework.Common;

namespace CambridgeSoft.COE.Registration.UnitTests.WebService
{
    [TestFixture]
    public class COERegistrationServicesTests
    {
        private string originalXml = string.Empty;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");

            RegistryRecord naproxen =
                Helpers.Registration.SubmitSampleCompound(Helpers.Registration.NAPROXEN_SMILES, Helpers.Registration.REGNUM1, false);
            naproxen.Dispose();

            RegistryRecord naproxen2 =
                Helpers.Registration.SubmitSampleCompound(Helpers.Registration.NAPROXEN_SMILES, Helpers.Registration.REGNUM2, false);
            naproxen2.Dispose();
        }

        [Test]
        public void CanAddNewBatch()
        {
            RegistryRecord record = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);
            int batchCount = record.BatchList.Count;

            COERegistrationServices services = new COERegistrationServices();
            services.AddBatch(Helpers.Registration.REGNUM1, string.Format(addBatchXml, COEUser.ID));

            RegistryRecord recordModified = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);
            int batchCountModified = recordModified.BatchList.Count;

            Assert.AreEqual(batchCount + 1, batchCountModified);
        }

        [Test]
        public void CanAddComponent()
        {
            COERegistrationServices services = new COERegistrationServices();

            RegistryRecord record = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);

            //if (string.IsNullOrEmpty(originalXml))
            //    originalXml = record.Xml;

            int componentCount = record.ComponentList.Count;

            services.AddComponent(Helpers.Registration.REGNUM1, string.Format(componentXml, COEUser.ID));

            RegistryRecord recordModified = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);

            int componentCountModified = recordModified.ComponentList.Count;

            Assert.AreEqual(componentCount + 1, componentCountModified);

        }

        [Test]
        public void CanDeleteBatch()
        {
            COERegistrationServices services = new COERegistrationServices();

            RegistryRecord record = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);
            // Check to see if CanAddNewBatch has been called
            if (record.BatchList.Count == 1)
            {
                // Add a new Batch if there's currently only 1 Batch
                services.AddBatch(Helpers.Registration.REGNUM1, string.Format(addBatchXml, COEUser.ID));
            }

            record = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);
            int batchCount = record.BatchList.Count;

            services.DeleteBatch(Helpers.Registration.REGNUM1, record.BatchList[1].ID);

            RegistryRecord recordModified = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);

            Assert.AreEqual(batchCount - 1, recordModified.BatchList.Count);
        }

        [Test]
        public void CanDeleteComponent()
        {

            COERegistrationServices services = new COERegistrationServices();

            RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000001");

            if (string.IsNullOrEmpty(originalXml))
                originalXml = record.Xml;

            int componentCount = record.ComponentList.Count;

            services.DeleteComponent("AB-000001", -1);

            RegistryRecord recordModified = RegistryRecord.GetRegistryRecord("AB-000001");

            Assert.AreEqual(componentCount - 1, recordModified.ComponentList.Count);

            if (!string.IsNullOrEmpty(originalXml))
            {
                recordModified.Xml = originalXml;
                recordModified.Save();
            }
        }

        [Test]
        public void CanUpdateBatch()
        {
            COERegistrationServices services = new COERegistrationServices();

            services.UpdateBatch(Helpers.Registration.REGNUM1, updateBatchXml);

            RegistryRecord recordModified = RegistryRecord.GetRegistryRecord(Helpers.Registration.REGNUM1);

            //Do some comparation
            Assert.AreEqual("warnings", recordModified.BatchList[0].PropertyList["STORAGE_REQ_AND_WARNINGS"].Value);
        }

        [Test]
        public void CanUpdateComponent()
        {
            COERegistrationServices services = new COERegistrationServices();

            RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000001");

            if (string.IsNullOrEmpty(originalXml))
                originalXml = record.Xml;

            services.UpdateComponent("AB-000001", "");

            RegistryRecord recordModified = RegistryRecord.GetRegistryRecord("AB-000001");

            Assert.AreEqual("new strc commencts", recordModified.ComponentList[0].Compound.BaseFragment.Structure.PropertyList["STRUCT_COMMENTS"].Value);

            if (!string.IsNullOrEmpty(originalXml))
            {
                recordModified.Xml = originalXml;
                recordModified.Save();
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Helpers.Registration.DeleteRegisteredRecord(Helpers.Registration.REGNUM1);
            Helpers.Registration.DeleteRegisteredRecord(Helpers.Registration.REGNUM2);

            Helpers.Authentication.Logoff();
        }


        #region XML

        #region Add_Batch_Xml
        private const string addBatchXml = @"<Batch>
  <BatchID>0</BatchID>
  <BatchNumber>0</BatchNumber>
  <FullRegNumber></FullRegNumber>
  <DateCreated>0001/01/01 12:00:00</DateCreated>
  <PersonCreated>{0}</PersonCreated>
  <PersonRegistered>{0}</PersonRegistered>
  <DateLastModified>0001/01/01 12:00:00</DateLastModified>
  <StatusID>3</StatusID>
  <PropertyList>
    <Property name=""BATCH_PREFIX"" type=""PICKLISTDOMAIN"" pickListDomainID=""4""></Property>
    <Property name=""SALTANDBATCHSUFFIX"" type=""TEXT""></Property>
    <Property name=""SCIENTIST_ID"" type=""PICKLISTDOMAIN"" pickListDomainID=""3"">32</Property>
    <Property name=""CREATION_DATE"" type=""DATE"">2011/09/01 12:00:00</Property>
    <Property name=""NOTEBOOK_TEXT"" type=""TEXT""></Property>
    <Property name=""AMOUNT"" type=""NUMBER""></Property>
    <Property name=""AMOUNT_UNITS"" type=""PICKLISTDOMAIN"" pickListDomainID=""2""></Property>
    <Property name=""APPEARANCE"" type=""TEXT""></Property>
    <Property name=""PURITY"" type=""NUMBER""></Property>
    <Property name=""PURITY_COMMENTS"" type=""TEXT""></Property>
    <Property name=""SAMPLEID"" type=""TEXT""></Property>
    <Property name=""SOLUBILITY"" type=""TEXT""></Property>
    <Property name=""BATCH_COMMENT"" type=""TEXT"">batch comm</Property>
    <Property name=""STORAGE_REQ_AND_WARNINGS"" type=""TEXT""></Property>
    <Property name=""FORMULA_WEIGHT"" type=""NUMBER"">30.06904</Property>
    <Property name=""BATCH_FORMULA"" type=""TEXT"">C2H6</Property>
    <Property name=""PERCENT_ACTIVE"" type=""NUMBER"">100</Property>
  </PropertyList>
  <IdentifierList></IdentifierList>
  <BatchComponentList>
    <BatchComponent >
      <ID>0</ID>
      <BatchID>0</BatchID>
      <MixtureComponentID>0</MixtureComponentID>
      <CompoundID>0</CompoundID>
      <ComponentIndex>0</ComponentIndex>
      <OrderIndex>1</OrderIndex>
      <PropertyList>
        <Property name=""PERCENTAGE"" type=""NUMBER""></Property>
      </PropertyList>
      <BatchComponentFragmentList></BatchComponentFragmentList>
    </BatchComponent>
  </BatchComponentList>
</Batch>";
        #endregion

        #region Update_Batch_Xml
        private string updateBatchXml = @"<Batch>
      <BatchID>1</BatchID>
      <BatchNumber>1</BatchNumber>
      <FullRegNumber>TT-000001/01</FullRegNumber>
      <DateCreated>2011/08/31 07:18:04</DateCreated>
      <PersonCreated>32</PersonCreated>
      <PersonRegistered>32</PersonRegistered>
      <DateLastModified>2011/08/31 07:18:04</DateLastModified>
      <StatusID>3</StatusID>
      <PropertyList>
        <Property name=""BATCH_PREFIX"" type=""PICKLISTDOMAIN"" pickListDomainID=""4"" insert=""yes""></Property>
        <Property name=""SALTANDBATCHSUFFIX"" type=""TEXT"" insert=""yes""></Property>
        <Property name=""SCIENTIST_ID"" type=""PICKLISTDOMAIN"" pickListDomainID=""3"" insert=""yes"">32</Property>
        <Property name=""CREATION_DATE"" type=""DATE"" insert=""yes"" update=""yes"">2011/09/01 12:00:00</Property>
        <Property name=""NOTEBOOK_TEXT"" type=""TEXT"" insert=""yes""></Property>
        <Property name=""AMOUNT"" type=""NUMBER"" insert=""yes""></Property>
        <Property name=""AMOUNT_UNITS"" type=""PICKLISTDOMAIN"" pickListDomainID=""2"" insert=""yes""></Property>
        <Property name=""APPEARANCE"" type=""TEXT"" insert=""yes""></Property>
        <Property name=""PURITY"" type=""NUMBER"" insert=""yes""></Property>
        <Property name=""PURITY_COMMENTS"" type=""TEXT"" insert=""yes""></Property>
        <Property name=""SAMPLEID"" type=""TEXT"" insert=""yes""></Property>
        <Property name=""SOLUBILITY"" type=""TEXT"" insert=""yes""></Property>
        <Property name=""BATCH_COMMENT"" type=""TEXT"" insert=""yes"">batch comm</Property>
        <Property name=""STORAGE_REQ_AND_WARNINGS"" type=""TEXT"" insert=""yes"" update=""yes"">warnings</Property>
        <Property name=""FORMULA_WEIGHT"" type=""NUMBER"" insert=""yes"">30.06904</Property>
        <Property name=""BATCH_FORMULA"" type=""TEXT"" insert=""yes"">C2H6</Property>
        <Property name=""PERCENT_ACTIVE"" type=""NUMBER"" insert=""yes"">100</Property>
      </PropertyList>
      <IdentifierList></IdentifierList>
      <BatchComponentList>
        <BatchComponent >
          <ID>1</ID>
          <BatchID>1</BatchID>
          <MixtureComponentID>1</MixtureComponentID>
          <CompoundID>1</CompoundID>
          <ComponentIndex>-1</ComponentIndex>
          <OrderIndex>1</OrderIndex>
          <PropertyList>
            <Property name=""PERCENTAGE"" type=""NUMBER"" insert=""yes""></Property>
          </PropertyList>
          <BatchComponentFragmentList></BatchComponentFragmentList>
        </BatchComponent>
      </BatchComponentList>
    </Batch>";
        #endregion

        private const string componentXml = @"<ComponentList>
    <Component >
	<ID>0</ID>
	<ComponentIndex>0</ComponentIndex>
	<Percentage>0</Percentage>
	<Compound>
	  <CompoundID>-1</CompoundID>
	  <DateCreated>0001/01/01 12:00:00</DateCreated>
	  <DateLastModified>0001/01/01 12:00:00</DateLastModified>
	  <PersonCreated>{0}</PersonCreated>
	  <Tag></Tag>
	  <PersonRegistered>15</PersonRegistered>
	  <RegNumber><RegID>0</RegID><SequenceNumber>0</SequenceNumber><SequenceID>2</SequenceID><RegNumber></RegNumber></RegNumber>
	  <BaseFragment><ID>0</ID><Structure><StructureID>0</StructureID><StructureFormat></StructureFormat><DrawingType>0</DrawingType>
<Structure molWeight=""0"" >VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAFQAAAENoZW1EcmF3IDEyLjAu
Mi45MTgIABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEADkZioAAIBOAMXMPQBMLLgA
AQkIAABAVgAAQAIAAgkIAAAAAAAAAAAADQgBAAEIBwEAAToEAQABOwQBAABFBAEA
ATwEAQAADAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMA
CwgIAAQAAADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQA
AAAeAAQIAgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAA
ACQAAAAkAAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8A
AAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBB
cmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGADQAAAAQCEAAAAAAAAAAAABsZ3QIA
gFQCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAMAAAAEAhAA5GYq
AACATgDFzD0ATCy4AASAAgAAAAACCAAAAC0AAMBOAAoAAgABADcEAQABAAAEgAQA
AAAAAggAAAA8ABO7aAAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAAAALQAmtoIACgAC
AAUANwQBAAEAAASACAAAAAACCAAAADwAObGcAAoAAgAHADcEAQABAAAEgAoAAAAA
AggA//8sAEystgAKAAIACQA3BAEAAQAABYAFAAAACgACAAQABAYEAAIAAAAFBgQA
BAAAAAoGAQABAAAFgAcAAAAKAAIABgAEBgQABAAAAAUGBAAGAAAACgYBAAEAAAWA
CQAAAAoAAgAIAAQGBAAGAAAABQYEAAgAAAABBgIABgAKBgEAAQAABYALAAAACgAC
AAoABAYEAAgAAAAFBgQACgAAAAEGAgAGAAoGAQABAAAAAAAAAAAAAA==
</Structure><UseNormalization>T</UseNormalization>
<NormalizedStructure>VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAFQAAAENoZW1EcmF3IDEyLjAu
Mi45MTgIABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEADkZioAAIBOAMXMPQBMLLgA
AQkIAABAVgAAQAIAAgkIAAAAAAAAAAAADQgBAAEIBwEAAToEAQABOwQBAABFBAEA
ATwEAQAADAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMA
CwgIAAQAAADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQA
AAAeAAQIAgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAA
ACQAAAAkAAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8A
AAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBB
cmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGADQAAAAQCEAAAAAAAAAAAABsZ3QIA
gFQCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAMAAAAEAhAA5GYq
AACATgDFzD0ATCy4AASAAgAAAAACCAAAAC0AAMBOAAoAAgABADcEAQABAAAEgAQA
AAAAAggAAAA8ABO7aAAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAAAALQAmtoIACgAC
AAUANwQBAAEAAASACAAAAAACCAAAADwAObGcAAoAAgAHADcEAQABAAAEgAoAAAAA
AggA//8sAEystgAKAAIACQA3BAEAAQAABYAFAAAACgACAAQABAYEAAIAAAAFBgQA
BAAAAAoGAQABAAAFgAcAAAAKAAIABgAEBgQABAAAAAUGBAAGAAAACgYBAAEAAAWA
CQAAAAoAAgAIAAQGBAAGAAAABQYEAAgAAAABBgIABgAKBgEAAQAABYALAAAACgAC
AAoABAYEAAgAAAAFBgQACgAAAAEGAgAGAAoGAQABAAAAAAAAAAAAAA==
</NormalizedStructure>
      <PropertyList><Property name=""STRUCT_COMMENTS"" type=""TEXT"">scccccccccc</Property><Property name=""STRUCT_NAME"" type=""TEXT"">pentane</Property></PropertyList>
	  <IdentifierList></IdentifierList>
	  </Structure>
	  </BaseFragment>
	  <FragmentList><Fragment><CompoundFragmentID>0</CompoundFragmentID><FragmentID>67</FragmentID><Structure><StructureID>0</StructureID><StructureFormat></StructureFormat><DrawingType>0</DrawingType><Structure molWeight=""0"" ></Structure><UseNormalization>T</UseNormalization><NormalizedStructure></NormalizedStructure></Structure></Fragment><Fragment><CompoundFragmentID>0</CompoundFragmentID><FragmentID>1214</FragmentID><Structure><StructureID>0</StructureID><StructureFormat></StructureFormat><DrawingType>0</DrawingType><Structure molWeight=""0"" ></Structure><UseNormalization>T</UseNormalization><NormalizedStructure></NormalizedStructure></Structure></Fragment></FragmentList>
	  <PropertyList><Property name=""CMP_COMMENTS"" type=""TEXT"">ccccccccccccccccccc</Property><Property name=""STRUCTURE_COMMENTS_TXT"" type=""TEXT"">scccccccccc</Property></PropertyList>
	  <IdentifierList></IdentifierList>
	</Compound>
   </Component>
 </ComponentList>";


        private const string AB01ComponentXml = @"<ComponentList>
    <Component>
      <ID/>
      <ComponentIndex>-1</ComponentIndex>
      <Compound>
        <CompoundID>1</CompoundID>
        <DateCreated>2011-07-22 01:57:31</DateCreated>
        <PersonCreated>15</PersonCreated>
        <PersonRegistered>15</PersonRegistered>
        <DateLastModified>2011-08-04 06:51:00</DateLastModified>
        <Tag/>
        <PropertyList>
          <Property name=""CMP_COMMENTS"">cc</Property>
          <Property name=""STRUCTURE_COMMENTS_TXT"">sc</Property>
        </PropertyList>
        <RegNumber>
          <RegID>2</RegID>
          <SequenceNumber>1</SequenceNumber>
          <RegNumber>C000001</RegNumber>
          <SequenceID>2</SequenceID>
        </RegNumber>
        <BaseFragment>
          <Structure>
            <StructureID>4</StructureID>
            <StructureFormat/>
            <Structure molWeight=""222.4094"" formula=""C16H30"">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
AAAABQYEABoAAAAKBgEAAQAAAAAAAAAAAAA=
</Structure>
            <NormalizedStructure>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
AAAABQYEABoAAAAKBgEAAQAAAAAAAAAAAAA=
</NormalizedStructure>
            <UseNormalization>T</UseNormalization>
            <DrawingType>0</DrawingType>
            <PropertyList>
              <Property name=""STRUCT_COMMENTS"">new strc commencts</Property>
              <Property name=""STRUCT_NAME"">cyclohexane compound with decahydronaphthalene (1:1)</Property>
            </PropertyList>
            <IdentifierList/>
          </Structure>
        </BaseFragment>
        <FragmentList>
          <Fragment>
            <CompoundFragmentID>1</CompoundFragmentID>
            <FragmentID>966</FragmentID>
            <Code>966</Code>
            <Name>Chloroform </Name>
            <FragmentTypeID lookupTable=""FragmentType"" lookupField=""ID"" displayField=""Description"" displayValue=""Solvate"">2</FragmentTypeID>
            <DateCreated>2009-06-30 02:02:34</DateCreated>
            <DateLastModified>2009-06-30 02:02:34</DateLastModified>
            <Equivalents/>
            <Structure>
              <StructureFormat/>
              <Structure molWeight=""119.37764"" formula=""CHCl3"">
        VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAGAAAAENzQ2FydHJpZGdlIDEx
        LjAuMC4xNjUAAw4AAgD///////8AAAAAAAABgAAAAAADgAoAAAAEgAEAAAACBAIA
        EQAAAASAAgAAAAAABIADAAAAAgQCABEAAAAEgAQAAAACBAIAEQAAAAWABgAAAAQG
        BAABAAAABQYEAAIAAAAAAAWABwAAAAQGBAACAAAABQYEAAMAAAAAAAWACAAAAAQG
        BAACAAAABQYEAAQAAAAAAAAAAAAAAAAA
      </Structure>
            </Structure>
          </Fragment>
          <Fragment>
            <CompoundFragmentID>2</CompoundFragmentID>
            <FragmentID>23</FragmentID>
            <Code>23</Code>
            <Name>4-Methylbenzenesulphonic acid salt</Name>
            <FragmentTypeID lookupTable=""FragmentType"" lookupField=""ID"" displayField=""Description"" displayValue=""Salt"">1</FragmentTypeID>
            <DateCreated>2009-06-30 02:02:31</DateCreated>
            <DateLastModified>2009-06-30 02:02:31</DateLastModified>
            <Equivalents/>
            <Structure>
              <StructureFormat/>
              <Structure molWeight=""172.20162"" formula=""C7H8O3S"">
        VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAGAAAAENzQ2FydHJpZGdlIDEx
        LjAuMC4xNjUAAw4AAgD///////8AAAAAAAABgAAAAAADgBkAAAAEgAEAAAAAAASA
        AgAAAAAABIADAAAAAAAEgAQAAAAAAASABQAAAAAABIAGAAAAAAAEgAcAAAAAAASA
        CAAAAAIEAgAQAAAABIAJAAAAAgQCAAgAAAAEgAoAAAACBAIACAAAAASACwAAAAIE
        AgAIAAAABYANAAAABAYEAAEAAAAFBgQAAgAAAAAABYAOAAAABAYEAAIAAAAFBgQA
        AwAAAAAGAgCAAAEGAgABAAIGAgABAAAABYAPAAAABAYEAAMAAAAFBgQABAAAAAAG
        AgCAAAEGAgABAAIGAgABAAAABYAQAAAABAYEAAQAAAAFBgQABQAAAAAGAgCAAAEG
        AgABAAIGAgABAAAABYARAAAABAYEAAUAAAAFBgQABgAAAAAGAgCAAAEGAgABAAIG
        AgABAAAABYASAAAABAYEAAYAAAAFBgQABwAAAAAGAgCAAAEGAgABAAIGAgABAAAA
        BYATAAAABAYEAAIAAAAFBgQABwAAAAAGAgCAAAEGAgABAAIGAgABAAAABYAUAAAA
        BAYEAAUAAAAFBgQACAAAAAAABYAVAAAABAYEAAgAAAAFBgQACQAAAAAGAgACAAAA
        BYAWAAAABAYEAAgAAAAFBgQACgAAAAAGAgACAAAABYAXAAAABAYEAAgAAAAFBgQA
        CwAAAAAAAAAAAAAAAAA=
      </Structure>
            </Structure>
          </Fragment>
        </FragmentList>
        <IdentifierList/>
      </Compound>
    </Component>
  </ComponentList>";
        #endregion

    }
}
