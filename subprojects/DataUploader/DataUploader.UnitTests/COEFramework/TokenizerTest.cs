using CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.COEFramework
{
    [TestFixture]
    public class TokenizerTest
    {
        #region "Test Split method"

        [Test]
        public void SplitTest()
        {
            SimpleSplitTest();
            IntegerIndexedPropertySplitTest();
            StringIndexedPropertySplitTest();
            NestedAndIndexedPropertySplitTest();
        }

        private void SimpleSplitTest()
        {
            string errorMessage = "Failed on simple split test";
            string simpleProperty = "Level1Property.Level2Property.Level3Property";

            string[] pieces = XmlTranslation.Tokenizer.Split(simpleProperty, Constants.SEPERATORS);

            Assert.AreEqual(5, pieces.Length, errorMessage);
            Assert.AreEqual("Level1Property", pieces[0], errorMessage);
            Assert.AreEqual(".", pieces[1], errorMessage);
            Assert.AreEqual("Level2Property", pieces[2], errorMessage);
            Assert.AreEqual(".", pieces[3], errorMessage);
            Assert.AreEqual("Level3Property", pieces[4], errorMessage);
        }

        private void IntegerIndexedPropertySplitTest()
        {
            string errorMessage = "Failed on integer indexed property split test";
            string indexedProperty = "IntegerIndexedProperty[0]";

            string[] pieces = XmlTranslation.Tokenizer.Split(indexedProperty, Constants.SEPERATORS);

            Assert.AreEqual(4, pieces.Length, errorMessage);
            Assert.AreEqual("IntegerIndexedProperty", pieces[0], errorMessage);
            Assert.AreEqual("[", pieces[1], errorMessage);
            Assert.AreEqual("0", pieces[2], errorMessage);
            Assert.AreEqual("]", pieces[3], errorMessage);
        }

        private void StringIndexedPropertySplitTest()
        {
            string errorMessage = "Failed on string indexed property split test";
            string indexedProperty = "StringIndexedProperty['somename']";

            string[] pieces = XmlTranslation.Tokenizer.Split(indexedProperty, Constants.SEPERATORS);

            Assert.AreEqual(6, pieces.Length, errorMessage);
            Assert.AreEqual("StringIndexedProperty", pieces[0], errorMessage);
            Assert.AreEqual("[", pieces[1], errorMessage);
            Assert.AreEqual("'", pieces[2], errorMessage);
            Assert.AreEqual("somename", pieces[3], errorMessage);
            Assert.AreEqual("'", pieces[4], errorMessage);
            Assert.AreEqual("]", pieces[5], errorMessage);
        }

        private void NestedAndIndexedPropertySplitTest()
        {
            string errorMessage = "Failed on nested and indexed property split test";
            string nestedAndIndexedPropertySplitTest = "NestedProperty.ChildProperty[0].ChildNestedProperty";

            string[] pieces = XmlTranslation.Tokenizer.Split(nestedAndIndexedPropertySplitTest, Constants.SEPERATORS);

            Assert.AreEqual(8, pieces.Length, errorMessage);
            Assert.AreEqual("NestedProperty", pieces[0], errorMessage);
            Assert.AreEqual(".", pieces[1], errorMessage);
            Assert.AreEqual("ChildProperty", pieces[2], errorMessage);
            Assert.AreEqual("[", pieces[3], errorMessage);
            Assert.AreEqual("0", pieces[4], errorMessage);
            Assert.AreEqual("]", pieces[5], errorMessage);
            Assert.AreEqual(".", pieces[6], errorMessage);
            Assert.AreEqual("ChildNestedProperty", pieces[7], errorMessage);
        }

        #endregion

        #region "Test GetNextToken method"

        [Test]
        public void GetNextTokenTest()
        {
            // The Split method test already covers different scenarios, so we only need to test
            // the most complicated case. The GetNextToken will use the token collection object internally
            // generated by Split method.
            string errorMessage = "Failed on GetNextToken test";
            string nestedAndIndexedPropertySplitTest = "NestedProperty.ChildProperty[0].ChildNestedProperty";

            XmlTranslation.Tokenizer tokenizer = new XmlTranslation.Tokenizer(nestedAndIndexedPropertySplitTest, Constants.SEPERATORS);

            Assert.AreEqual("NestedProperty", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual(".", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual("ChildProperty", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual("[", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual("0", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual("]", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual(".", tokenizer.GetNextToken(), errorMessage);
            Assert.AreEqual("ChildNestedProperty", tokenizer.GetNextToken(), errorMessage);
        }

        #endregion
    }
}
