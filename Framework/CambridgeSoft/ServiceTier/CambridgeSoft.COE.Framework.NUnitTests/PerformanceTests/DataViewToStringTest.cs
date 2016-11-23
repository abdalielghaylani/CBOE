using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using CambridgeSoft.COE.Framework.Common;
using System.IO;
using System.Xml;
using CambridgeSoft.COE.Framework.Caching;
using CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData;
namespace CambridgeSoft.COE.Framework.NUnitTests
{
    /// <summary>
    /// Summary description for DataViewToStringTest
    /// </summary>
    [TestFixture]
    public class DataViewToStringTest
    {
        COEDataView _bigDV;
        COEDataView _alternateBigDV;
        public DataViewToStringTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Documents and Settings\fgaldeano\Desktop\BigDataview.xml");
            _bigDV = new COEDataView(doc);

            doc.Load(@"C:\Documents and Settings\fgaldeano\Desktop\AlternateBigDV.xml");
            _alternateBigDV = new COEDataView(doc);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [TestFixtureSetUp]
        // public static void MyClassInitialize() { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [TestFixtureTearDown]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [SetUp]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TearDown]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void ToStringTest()
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
            string str = _bigDV.ToString();
            System.Diagnostics.Debug.WriteLine("After tostring");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
            string key = Utilities.GetMD5Hash(str);
            System.Diagnostics.Debug.WriteLine("After md5");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);

            str = _alternateBigDV.ToString();
            System.Diagnostics.Debug.WriteLine("After tostring");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
            key = Utilities.GetMD5Hash(str);
            System.Diagnostics.Debug.WriteLine("After md5");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);


            str = Utilities.Serialize(_bigDV); ;
            System.Diagnostics.Debug.WriteLine("After tostring");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
            key = Utilities.GetMD5Hash(str);
            System.Diagnostics.Debug.WriteLine("After md5");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);

            str = Utilities.Serialize(_alternateBigDV); ;
            System.Diagnostics.Debug.WriteLine("After tostring");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
            key = Utilities.GetMD5Hash(str);
            System.Diagnostics.Debug.WriteLine("After md5");
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
        }

        [Test]
        public void GetMetadataDataviewTest()
        {
            DataView dv = null;

            for (int i = 0; i < 10; i++)
                dv = GetMetadataDataview(_bigDV);

            for (int i = 0; i < 10; i++)
                dv = GetMetadataDataview(_alternateBigDV);

            for (int i = 0; i < 10; i++)
                dv = new DataView(_bigDV);

            for (int i = 0; i < 10; i++)
                dv = new DataView(_alternateBigDV);
        }

        private DataView GetMetadataDataview(COEDataView dataView)
        {
           DataView result = null;
            string key = dataView.GetHashCode().ToString();
            result = LocalCache.Get(key, typeof(DataView)) as DataView;
            if (result == null)
            {
                result = new DataView(dataView);
                LocalCache.Add(key, typeof(DataView), result, LocalCache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), COECacheItemPriority.Normal);
            }
            return result;
        }
    }
}
