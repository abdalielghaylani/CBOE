using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.Registration.Services.Types;

using NUnit.Framework;

namespace CambridgeSoft.COE.Registration.UnitTests
{
    [TestFixture]
    public class Using_AutoMapper
    {
        [SetUp]
        public void SetUp()
        {
            Helpers.Authentication.Logon("T5_85", "T5_85");
        }

        [Test]
        public void MapFromOneRegistrationToAnother()
        {

            //create a property 'donor'
            RegistryRecord nr = RegistryRecord.GetRegistryRecord("AB-000001");
            //make some modifications to be mapped into the recipient
            //nr.ComponentList[0].Compound.PropertyList["CMP_COMMENTS"].Value = "AutoMapper test";
            nr.ComponentList[0].Compound.AddIdentifier("Synonym", "Garbage543");
            nr.UpdateXml();

            //create a property 'recipient'
            RegistryRecord rr = RegistryRecord.GetRegistryRecord("AB-000001");

            //map the modifications into the recipient
            AutoMapper.Mapper.CreateMap<RegistryRecord, RegistryRecord>();
            //AutoMapper.Mapper.CreateMap<ComponentList, ComponentList>();
            AutoMapper.Mapper.CreateMap<Component, Component>();
            AutoMapper.Mapper.CreateMap<Compound, Compound>();
            //AutoMapper.Mapper.CreateMap<IdentifierList, IdentifierList>();
            AutoMapper.Mapper.CreateMap<Identifier, Identifier>();

            try
            {
                //AutoMapper.Mapper.Map<RegistryRecord, RegistryRecord>(nr, rr);
                AutoMapper.Mapper.Map<IdentifierList, IdentifierList>(
                    nr.ComponentList[0].Compound.IdentifierList,
                    rr.ComponentList[0].Compound.IdentifierList
                );

                Assert.IsTrue(rr.IsDirty, "The recipient record has not been marked as dirty!");
                Assert.Greater(rr.ComponentList[0].Compound.IdentifierList.Count, 0, "Identifier not added!");
            }
            catch (Exception ex)
            {

            }

        }

        [TearDown]
        public void TearDown()
        {
            Helpers.Authentication.Logoff();
        }
    }
}
