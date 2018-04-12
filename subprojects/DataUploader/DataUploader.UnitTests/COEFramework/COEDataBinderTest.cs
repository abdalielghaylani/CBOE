using System;
using System.Reflection;

using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;

using NUnit.Framework;
using CambridgeSoft.COE.DataLoader.Core.DataMapping;

namespace CambridgeSoft.COE.UnitTests.COEFramework
{
    [TestFixture]
    public class COEDataBinderTest
    {

        #region "Test RetrieveProperty and SetProperty methods"

        [Test]
        public void RetrieveAndSetPropertyTest()
        {
            TestCanRetrieveAndSetSimpleProperty();
            TestCanRetrieveAndSetNestedProperty();
            TestCanRetrieveAndSetIntegerIndexedProperty();
            TestCanRetrieveAndSetStringIndexedProperty();
            TestCanRetrieveAndSetNestedAndIndexedProperty();
            TestCanRetrieveAndSetArrayElement();
            TestCanRetrieveThis();
        }

        private void TestCanRetrieveAndSetSimpleProperty()
        {
            string bindingExpression = "RegNum";
            string retrieveErrorMessage = "Failed to retrieve simple property";
            string setErrorMessage = "Failed to set simple property";
            string oldValue = "AB-00001";
            string newValue = "NEW-00001";

            MockRegistryRecord registryRecord = new MockRegistryRecord();
            registryRecord.RegNum = oldValue;
            COEDataBinder dataBinder = new COEDataBinder(registryRecord);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);
        }

        private void TestCanRetrieveAndSetNestedProperty()
        {
            string bindingExpression = "BatchList.Description";
            string retrieveErrorMessage = "Failed to retrieve nested property";
            string setErrorMessage = "Failed to set nested property";
            string oldValue = "Initial batch list";
            string newValue = "Refined batch list";

            MockRegistryRecord registryRecord = new MockRegistryRecord();
            registryRecord.BatchList.Description = oldValue;
            COEDataBinder dataBinder = new COEDataBinder(registryRecord);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);
        }

        private void TestCanRetrieveAndSetIntegerIndexedProperty()
        {
            string bindingExpression = "this[0].DateCreated";
            string retrieveErrorMessage = "Failed to retrieve integer indexed property";
            string setErrorMessage = "Failed to set integer indexed property";
            DateTime oldValue = DateTime.Now;
            DateTime newValue = DateTime.Now.AddDays(1);

            MockBatchList batchList = new MockBatchList();
            // Comment out the following 2 lines to test the case when the element with requested indexor doesn't exist already.
            batchList.Add(new MockBatch());
            batchList[0].DateCreated = oldValue;
            COEDataBinder dataBinder = new COEDataBinder(batchList);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);
        }

        private void TestCanRetrieveAndSetStringIndexedProperty()
        {
            string bindingExpression = "this['Structure'].Value";
            string retrieveErrorMessage = "Failed to retrieve string indexed property";
            string setErrorMessage = "Failed to set string indexed property";
            string oldValue = "Benzene";
            string newValue = "Ethanol";

            MockPropertyList propertyList = new MockPropertyList();
            propertyList.Add(new MockProperty("Structure"));
            propertyList[0].Value = oldValue;

            COEDataBinder dataBinder = new COEDataBinder(propertyList);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);
        }

        private void TestCanRetrieveAndSetNestedAndIndexedProperty()
        {
            string bindingExpression = "BatchList[0].PropertyList['Project'].Value";
            string retrieveErrorMessage = "Failed to retrieve nested and indexed property";
            string setErrorMessage = "Failed to set nested and indexed property";
            string oldValue = "AB";
            string newValue = "NewProject";

            MockRegistryRecord registryRecord = new MockRegistryRecord();
            registryRecord.BatchList.Add(new MockBatch());
            registryRecord.BatchList[0].PropertyList.Add(new MockProperty("Project"));
            registryRecord.BatchList[0].PropertyList[0].Value = oldValue;

            COEDataBinder dataBinder = new COEDataBinder(registryRecord);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);
        }

        private void TestCanRetrieveAndSetArrayElement()
        {
            string bindingExpression = "this[0][0]";
            string retrieveErrorMessage = "Failed to retrieve array element";
            string setErrorMessage = "Failed to set array element";
            string oldValue = "Old Scientist";
            string newValue = "New Scientist";

            MockBatchList batchList = new MockBatchList();
            batchList.Add(new MockBatch());
            batchList[0][0] = oldValue;

            COEDataBinder dataBinder = new COEDataBinder(batchList);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);

            bindingExpression = "BatchList[0][0]";
            MockRegistryRecord registryRecord = new MockRegistryRecord();
            registryRecord.BatchList.Add(new MockBatch());
            registryRecord.BatchList[0][0] = oldValue;

            dataBinder = new COEDataBinder(registryRecord);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);

            bindingExpression = "NameHistory[0]";
            registryRecord = new MockRegistryRecord();
            registryRecord.NameHistory[0] = oldValue;
            dataBinder = new COEDataBinder(registryRecord);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);

            dataBinder.SetProperty(bindingExpression, newValue);

            Assert.AreEqual(newValue, dataBinder.RetrieveProperty(bindingExpression), setErrorMessage);
        }

        private void TestCanRetrieveThis()
        {
            string bindingExpression = "this";
            string retrieveErrorMessage = "Failed to retrieve 'this'";
            MockRegistryRecord oldValue = new MockRegistryRecord();
            oldValue.RegNum = "AB-00001";

            COEDataBinder dataBinder = new COEDataBinder(oldValue);

            Assert.AreEqual(oldValue, dataBinder.RetrieveProperty(bindingExpression), retrieveErrorMessage);
        }

        #endregion

    }
}
