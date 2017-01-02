using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.UnitTests.BLL
{
    [TestFixture]
    public class RegistryRecordTest
    {
        private string updatedRegistryRecordXml = @"
<MultiCompoundRegistryRecord SameBatchesIdentity=""True"" ActiveRLS=""Off"" IsEditable=""True""><ID>1</ID><DateCreated>2011-07-29 03:52:46</DateCreated><DateLastModified>2011-07-29 03:52:46</DateLastModified><PersonCreated>9</PersonCreated><StructureAggregation>VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAFQAAAENoZW1EcmF3IDEyLjAu
Mi45MTgIABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEAAyLBIA7ERhAM1TTwATO5YA
AQkIAAAAAAAAAAAAAgkIAAAA4QAAgAYBDQgBAAEIBwEAAToEAQABOwQBAABFBAEA
ATwEAQAADAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMA
CwgIAAQAAADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQA
AAAeAAQIAgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAA
ACQAAAAkAAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8A
AAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBB
cmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGAFQAAAAQCEAAAAAAAAAAAAAAA0AIA
ABwCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAAMiwS
AOxEYQDNU08AEzuWAASAAgAAAAACCAAAwCEA7MRhAAoAAgABADcEAQABAAAEgAQA
AAAAAggAAMA/AOzEYQAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADATgAAwHsACgAC
AAUANwQBAAEAAASACAAAAAACCAAAwD8AE7uVAAoAAgAHADcEAQABAAAEgAoAAAAA
AggAAMAhABO7lQAKAAIACQA3BAEAAQAABIAMAAAAAAIIAADAEgAAwHsACgACAAsA
NwQBAAEAAAWADgAAAAoAAgANAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAPAAAA
CgACAA4ABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgBAAAAAKAAIADwAEBgQABgAA
AAUGBAAIAAAACgYBAAEAAAWAEQAAAAoAAgAQAAQGBAAIAAAABQYEAAoAAAAKBgEA
AQAABYASAAAACgACABEABAYEAAoAAAAFBgQADAAAAAoGAQABAAAFgBMAAAAKAAIA
EgAEBgQADAAAAAUGBAACAAAACgYBAAEAAAAAAAAAAAAA
</StructureAggregation><Approved /><StatusID>3</StatusID><PropertyList><Property name=""REG_COMMENTS"" friendlyName=""REG_COMMENTS"" type=""TEXT"" precision=""200"" sortOrder=""0""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList>asdf</Property></PropertyList><RegNumber><RegID>1</RegID><SequenceNumber>1</SequenceNumber><RegNumber>AB-000001</RegNumber><SequenceID>1</SequenceID></RegNumber><IdentifierList /><ProjectList /><ComponentList><Component><ID /><ComponentIndex>-1</ComponentIndex><Compound><CompoundID>1</CompoundID><DateCreated>2011-07-29 03:52:46</DateCreated><PersonCreated>15</PersonCreated><PersonRegistered>15</PersonRegistered><DateLastModified>2011-07-29 03:52:46</DateLastModified><Tag /><PropertyList><Property name=""CMP_COMMENTS"" friendlyName=""CMP_COMMENTS"" type=""TEXT"" precision=""200"" sortOrder=""0""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList></Property><Property name=""STRUCTURE_COMMENTS_TXT"" friendlyName=""STRUCTURE_COMMENTS_TXT"" type=""TEXT"" precision=""200"" sortOrder=""1""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList></Property></PropertyList><RegNumber><RegID>2</RegID><SequenceNumber>1</SequenceNumber><RegNumber>C000001</RegNumber><SequenceID>2</SequenceID></RegNumber><BaseFragment><Structure><StructureID>4</StructureID><StructureFormat /><Structure molWeight=""84.15948"" formula=""C6H12"">VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAFQAAAENoZW1EcmF3IDEyLjAu
Mi45MTgIABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEAAyLBIA7ERhAM1TTwATO5YA
AQkIAAAAAAAAAAAAAgkIAAAA4QAAgAYBDQgBAAEIBwEAAToEAQABOwQBAABFBAEA
ATwEAQAADAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMA
CwgIAAQAAADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQA
AAAeAAQIAgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAA
ACQAAAAkAAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8A
AAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBB
cmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGAFQAAAAQCEAAAAAAAAAAAAAAA0AIA
ABwCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAAMiwS
AOxEYQDNU08AEzuWAASAAgAAAAACCAAAwCEA7MRhAAoAAgABADcEAQABAAAEgAQA
AAAAAggAAMA/AOzEYQAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADATgAAwHsACgAC
AAUANwQBAAEAAASACAAAAAACCAAAwD8AE7uVAAoAAgAHADcEAQABAAAEgAoAAAAA
AggAAMAhABO7lQAKAAIACQA3BAEAAQAABIAMAAAAAAIIAADAEgAAwHsACgACAAsA
NwQBAAEAAAWADgAAAAoAAgANAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAPAAAA
CgACAA4ABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgBAAAAAKAAIADwAEBgQABgAA
AAUGBAAIAAAACgYBAAEAAAWAEQAAAAoAAgAQAAQGBAAIAAAABQYEAAoAAAAKBgEA
AQAABYASAAAACgACABEABAYEAAoAAAAFBgQADAAAAAoGAQABAAAFgBMAAAAKAAIA
EgAEBgQADAAAAAUGBAACAAAACgYBAAEAAAAAAAAAAAAA
</Structure><NormalizedStructure>VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAAAAAAMAFQAAAENoZW1EcmF3IDEyLjAu
Mi45MTgIABMAAABVbnRpdGxlZCBEb2N1bWVudAQCEAAyLBIA7ERhAM1TTwATO5YA
AQkIAAAAAAAAAAAAAgkIAAAA4QAAgAYBDQgBAAEIBwEAAToEAQABOwQBAABFBAEA
ATwEAQAADAYBAAEPBgEAAQ0GAQAAQgQBAABDBAEAAEQEAQAACggIAAMAYADIAAMA
CwgIAAQAAADwAAMACQgEADOzAgAICAQAAAACAAcIBAAAAAEABggEAAAABAAFCAQA
AAAeAAQIAgB4AAMIBAAAAHgAIwgBAAUMCAEAACgIAQABKQgBAAEqCAEAAQIIEAAA
ACQAAAAkAAAAJAAAACQAAQMCAAAAAgMCAAEAAAMyAAgA////////AAAAAAAA//8A
AAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAEkAAAAAgADAOQEBQBB
cmlhbAQA5AQPAFRpbWVzIE5ldyBSb21hbgGAFQAAAAQCEAAAAAAAAAAAAAAA0AIA
ABwCFggEAAAAJAAYCAQAAAAkABkIAAAQCAIAAQAPCAIAAQADgAUAAAAEAhAAMiwS
AOxEYQDNU08AEzuWAASAAgAAAAACCAAAwCEA7MRhAAoAAgABADcEAQABAAAEgAQA
AAAAAggAAMA/AOzEYQAKAAIAAwA3BAEAAQAABIAGAAAAAAIIAADATgAAwHsACgAC
AAUANwQBAAEAAASACAAAAAACCAAAwD8AE7uVAAoAAgAHADcEAQABAAAEgAoAAAAA
AggAAMAhABO7lQAKAAIACQA3BAEAAQAABIAMAAAAAAIIAADAEgAAwHsACgACAAsA
NwQBAAEAAAWADgAAAAoAAgANAAQGBAACAAAABQYEAAQAAAAKBgEAAQAABYAPAAAA
CgACAA4ABAYEAAQAAAAFBgQABgAAAAoGAQABAAAFgBAAAAAKAAIADwAEBgQABgAA
AAUGBAAIAAAACgYBAAEAAAWAEQAAAAoAAgAQAAQGBAAIAAAABQYEAAoAAAAKBgEA
AQAABYASAAAACgACABEABAYEAAoAAAAFBgQADAAAAAoGAQABAAAFgBMAAAAKAAIA
EgAEBgQADAAAAAUGBAACAAAACgYBAAEAAAAAAAAAAAAA
</NormalizedStructure><UseNormalization>T</UseNormalization><DrawingType>0</DrawingType><PropertyList><Property name=""STRUCT_COMMENTS"" friendlyName=""STRUCT_COMMENTS"" type=""TEXT"" precision=""200"" sortOrder=""0""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList></Property><Property name=""STRUCT_NAME"" friendlyName=""STRUCT_NAME"" type=""TEXT"" precision=""2000"" sortOrder=""1""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 2000 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""2000"" /></params></validationRule></validationRuleList>cyclohexane</Property></PropertyList><IdentifierList /></Structure></BaseFragment><FragmentList /><IdentifierList /></Compound></Component></ComponentList><BatchList><Batch><BatchID>1</BatchID><BatchNumber>1</BatchNumber><FullRegNumber>AB-000001/01</FullRegNumber><DateCreated>2011-07-29 03:52:46</DateCreated><PersonCreated displayName=""T5_84"">9</PersonCreated><PersonRegistered displayName=""T5_85"">15</PersonRegistered><DateLastModified>2011-07-29 03:52:46</DateLastModified><StatusID>3</StatusID><ProjectList /><IdentifierList /><PropertyList><Property name=""SCIENTIST_ID"" friendlyName=""SCIENTIST_ID"" type=""PICKLISTDOMAIN"" precision="""" pickListDomainID=""3"" sortOrder=""0""><validationRuleList />15</Property><Property name=""CREATION_DATE"" friendlyName=""CREATION_DATE"" type=""DATE"" precision="""" sortOrder=""1""><validationRuleList />2011-07-29 12:00:00</Property><Property name=""NOTEBOOK_TEXT"" friendlyName=""NOTEBOOK_TEXT"" type=""TEXT"" precision=""100"" sortOrder=""2""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 100 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""100"" /></params></validationRule></validationRuleList></Property><Property name=""AMOUNT"" friendlyName=""AMOUNT"" type=""NUMBER"" precision=""14.6"" sortOrder=""3""><validationRuleList><validationRule validationRuleName=""float"" errorMessage=""This property can have at most 8 integer and 6 decimal digits""><params><param name=""integerPart"" value=""8"" /><param name=""decimalPart"" value=""6"" /></params></validationRule></validationRuleList></Property><Property name=""AMOUNT_UNITS"" friendlyName=""AMOUNT_UNITS"" type=""PICKLISTDOMAIN"" precision="""" pickListDomainID=""2"" sortOrder=""4""><validationRuleList /></Property><Property name=""APPEARANCE"" friendlyName=""APPEARANCE"" type=""TEXT"" precision=""50"" sortOrder=""5""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 50 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""50"" /></params></validationRule></validationRuleList></Property><Property name=""PURITY"" friendlyName=""PURITY"" type=""NUMBER"" precision=""5.2"" sortOrder=""6""><validationRuleList><validationRule validationRuleName=""float"" errorMessage=""This property can have at most 3 integer and 2 decimal digits""><params><param name=""integerPart"" value=""3"" /><param name=""decimalPart"" value=""2"" /></params></validationRule><validationRule validationRuleName=""numericRange""><params><param name=""min"" value=""0"" /><param name=""max"" value=""100"" /></params></validationRule></validationRuleList></Property><Property name=""PURITY_COMMENTS"" friendlyName=""PURITY_COMMENTS"" type=""TEXT"" precision=""50"" sortOrder=""7""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 50 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""50"" /></params></validationRule></validationRuleList></Property><Property name=""SAMPLEID"" friendlyName=""SAMPLEID"" type=""TEXT"" precision=""200"" sortOrder=""8""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 100 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""100"" /></params></validationRule></validationRuleList></Property><Property name=""SOLUBILITY"" friendlyName=""SOLUBILITY"" type=""TEXT"" precision=""100"" sortOrder=""9""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 100 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""100"" /></params></validationRule></validationRuleList></Property><Property name=""BATCH_COMMENT"" friendlyName=""BATCH_COMMENT"" type=""TEXT"" precision=""200"" sortOrder=""10""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList></Property><Property name=""STORAGE_REQ_AND_WARNINGS"" friendlyName=""STORAGE_REQ_AND_WARNINGS"" type=""TEXT"" precision=""200"" sortOrder=""11""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList></Property><Property name=""FORMULA_WEIGHT"" friendlyName=""FORMULA_WEIGHT"" type=""NUMBER"" precision=""14.6"" sortOrder=""12""><validationRuleList><validationRule validationRuleName=""float"" errorMessage=""This property can have at most 8 integer and 6 decimal digits""><params><param name=""integerPart"" value=""8"" /><param name=""decimalPart"" value=""6"" /></params></validationRule></validationRuleList>84.15948</Property><Property name=""BATCH_FORMULA"" friendlyName=""BATCH_FORMULA"" type=""TEXT"" precision=""200"" sortOrder=""13""><validationRuleList><validationRule validationRuleName=""textLength"" errorMessage=""The property value can have between 0 and 200 characters""><params><param name=""min"" value=""0"" /><param name=""max"" value=""200"" /></params></validationRule></validationRuleList>C6H12</Property><Property name=""PERCENT_ACTIVE"" friendlyName=""PERCENT_ACTIVE"" type=""NUMBER"" precision=""5.2"" sortOrder=""14""><validationRuleList><validationRule validationRuleName=""float"" errorMessage=""This property can have at most 3 integer and 2 decimal digits""><params><param name=""integerPart"" value=""3"" /><param name=""decimalPart"" value=""2"" /></params></validationRule><validationRule validationRuleName=""numericRange""><params><param name=""min"" value=""0"" /><param name=""max"" value=""100"" /></params></validationRule></validationRuleList>100</Property></PropertyList><BatchComponentList><BatchComponent><ID>1</ID><BatchID>1</BatchID><CompoundID>1</CompoundID><MixtureComponentID>1</MixtureComponentID><ComponentIndex>-1</ComponentIndex><PropertyList><Property name=""PERCENTAGE"" friendlyName=""PERCENTAGE"" type=""NUMBER"" precision=""5.2"" sortOrder=""0""><validationRuleList><validationRule validationRuleName=""float"" errorMessage=""This property can have at most 3 integer and 2 decimal digits""><params><param name=""integerPart"" value=""3"" /><param name=""decimalPart"" value=""2"" /></params></validationRule><validationRule validationRuleName=""numericRange""><params><param name=""min"" value=""0"" /><param name=""max"" value=""100"" /></params></validationRule></validationRuleList></Property></PropertyList><BatchComponentFragmentList /></BatchComponent></BatchComponentList></Batch></BatchList><AddIns><AddIn assembly=""CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc"" class=""CambridgeSoft.COE.Registration.Services.RegistrationAddins.NormalizedStructureAddIn"" friendlyName=""Structure Normalization"" required=""no"" enabled=""no""><Event eventName=""Updating"" eventHandler=""ApplyNormalization"" /><Event eventName=""Inserting"" eventHandler=""ApplyNormalization"" /><AddInConfiguration><ScriptFile>C:\Program Files\CambridgeSoft\ChemOfficeEnterprise12.1.0.0\Registration\PythonScripts\parentscript.py</ScriptFile><!--Commented <PythonWebServiceURL> to bypass soap
			<PythonWebServiceURL>http://localhost/PyEngine/Service.asmx</PythonWebServiceURL> --><StructuresIdsToAvoid>-2|-3|</StructuresIdsToAvoid></AddInConfiguration></AddIn><AddIn assembly=""CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc"" class=""CambridgeSoft.COE.Registration.Services.RegistrationAddins.StructureAggregationAddIn"" friendlyName=""Aggregate Structures"" required=""yes"" enabled=""yes""><Event eventName=""Inserting"" eventHandler=""OnInsertingHandler"" /><Event eventName=""Updating"" eventHandler=""OnUpdatingHandler"" /><AddInConfiguration><NumberOfColumns>4</NumberOfColumns></AddInConfiguration></AddIn><AddIn assembly=""CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc"" class=""CambridgeSoft.COE.Registration.Services.RegistrationAddins.FindDuplicatesAddIn"" friendlyName=""Find Custom Field Duplicates"" required=""no"" enabled=""no""><Event eventName=""Registering"" eventHandler=""OnRegisteringHandler"" /><AddInConfiguration /></AddIn><AddIn assembly=""CambridgeSoft.COE.Registration.RegistrationAddins, Version=12.1.0.0, Culture=neutral, PublicKeyToken=f435ba95da9797dc"" class=""CambridgeSoft.COE.Registration.Services.RegistrationAddins.ChemDrawPropertyCalculationAddIn"" friendlyName=""ChemDraw Calculations"" required=""no"" enabled=""yes""><Event eventName=""Inserting"" eventHandler=""Calculate"" /><Event eventName=""Updating"" eventHandler=""Calculate"" /><AddInConfiguration><Calculations><Calculation><Behavior>BatchProperty</Behavior><Transformation>MolecularFormula</Transformation><PropertyName>BATCH_FORMULA</PropertyName><DefaultText>NA</DefaultText></Calculation><Calculation><Behavior>BatchProperty</Behavior><Transformation>FormulaWeight</Transformation><PropertyName>FORMULA_WEIGHT</PropertyName><DefaultText /></Calculation><Calculation><Behavior>BatchProperty</Behavior><Transformation>PercentActive</Transformation><PropertyName>PERCENT_ACTIVE</PropertyName><DefaultText /></Calculation><Calculation><Behavior>StructureProperty</Behavior><Transformation>FindNameByStructure</Transformation><PropertyName>STRUCT_NAME</PropertyName><DefaultText>NA</DefaultText></Calculation></Calculations></AddInConfiguration></AddIn></AddIns></MultiCompoundRegistryRecord>
";

        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        [Test]
        public void UpdateFromXml_Should_Update_Registry_Level()
        {
            RegistryRecord record = RegistryRecord.GetRegistryRecord("AB-000001");

            record.UpdateFromXml(updatedRegistryRecordXml);

            record.Save();

            record = RegistryRecord.GetRegistryRecord("AB-000001");

            Assert.AreEqual(9, record.PersonCreated);
        }
    }
}
