using System;
using System.Collections.Generic;
using System.Text;

using CambridgeSoft.COE.DataLoader.Core;

using NUnit.Framework;

namespace CambridgeSoft.COE.UnitTests.DataLoader.Core
{
    [TestFixture()]
    public class RecordIndexTests
    {
        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void ConstructMasterRangeNoValues()
        {
            IndexRange sdr = new IndexRange();
            Assert.AreEqual(sdr.RangeBegin, -1, "The range start was not defaulted properly.");
            Assert.AreEqual(sdr.RangeEnd, int.MaxValue, "The range end was not defaulted properly.");
            Assert.AreEqual(sdr.IsInactive, true, "The range was inappropriately activated.");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void ConstructMasterRangeWithValues()
        {
            int start = 5;
            int finish = 25;

            IndexRange sdr = new IndexRange(start, finish);
            Assert.AreEqual((sdr.RangeEnd - sdr.RangeBegin), (finish - start)
                , "The expected range was not created properly"
            );
            Assert.AreNotEqual(sdr.RangeBegin, -1, "The start of the range was not set");
            Assert.AreNotEqual(sdr.RangeEnd, int.MaxValue, "The end of the range was not set");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void ConstructSourceDataIndices()
        {
            int start = 9;
            int finish = 209;

            IndexRange sdr = new IndexRange(start, finish);
            IndexList indexList = new IndexList(sdr);

            Assert.IsNotEmpty(indexList, "The indices were not added by the constructor");
            Assert.AreEqual(201, indexList.Count, "The list was not calculated to be inclusive");
            Assert.AreEqual(indexList.IndexOf(start), 0, "Could not find start of range");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void GenerateRangeListDefaultValues()
        {
            IndexList indexList = new IndexList(new IndexRange());
            IndexRanges actionRanges = indexList.ToIndexRanges();
            string err = "An 'action range' was created from an uninitialized IndexRange";

            Assert.AreEqual(actionRanges.Count, 0, err);
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void GenerateRangeListWithValues()
        {
            //Create indices where #2, 6 and 7 are all omitted
            int[] interestingIndexNumbers = new int[] {1,3,4,5,8,9,10};

            IndexList indexList = new IndexList(interestingIndexNumbers);
            IndexRanges actionRanges = indexList.ToIndexRanges();

            Assert.AreEqual(actionRanges.Count, 3, "Ranges calculated correctly");
            Assert.AreEqual(actionRanges[0].RangeBegin, 1, "First range begins with the correct index");
            Assert.AreEqual(actionRanges[0].RangeEnd, 1, "First range ends with the correct index");
            Assert.AreEqual(actionRanges[1].RangeBegin, 3, "Second range begins with the correct index");
            Assert.AreEqual(actionRanges[1].RangeEnd, 5, "Second range ends with the correct index");
            Assert.AreEqual(actionRanges[2].RangeBegin, 8, "Third/final range begins with the correct index");
            Assert.AreEqual(actionRanges[2].RangeEnd, 10, "Third/final range ends with the correct index");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void RecreateRangeListAfterApplyingExclusions()
        {
            IndexRange sdr = new IndexRange(455, 1500);
            IndexList indexList = new IndexList(sdr);
            IndexRanges actionRanges = indexList.ToIndexRanges();

            //create the single range
            Assert.AreEqual(actionRanges.Count, 1, "Ranges calculated correctly");

            //eliminate some data
            indexList.Remove(993);
            indexList.Remove(994);
            indexList.Remove(1204);
            indexList.Remove(1205);
            indexList.Remove(1206);

            IndexRanges newActionRanges = indexList.ToIndexRanges();

            Assert.AreEqual(newActionRanges.Count, 3, "Ranges calculated correctly");
            Assert.AreEqual(newActionRanges[1].RangeBegin, 995, "Ranges calculated incorrectly");
            Assert.AreEqual(newActionRanges[1].RangeEnd, 1203, "Ranges calculated incorrectly");
        }

        [Test()]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void EnsureRangeNormalization()
        {
            //provide some overlapping index ranges
            IndexRanges ranges = new IndexRanges();
            ranges.Add(0, new IndexRange(455, 653));
            ranges.Add(1, new IndexRange(200, 511));
            ranges.Add(2, new IndexRange(12, 98));
            ranges.NormalizeRanges();

            //The union of these values should yield two index ranges
            //  12- 98, inclusive
            // 200-653, inclusive

            Assert.AreEqual(ranges.Count, 2, "Index ranges were calculated incorrectly");
            Assert.AreEqual(ranges[0].RangeBegin, 12, "Index range 1 has an invalid starting value");
            Assert.AreEqual(ranges[0].RangeEnd, 98, "Index range 1 has an invalid starting value");
            Assert.AreEqual(ranges[1].RangeBegin, 200, "Index range 2 has an invalid starting value");
            Assert.AreEqual(ranges[1].RangeEnd, 653, "Index range 1 has an invalid starting value");
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void RecordCountLimitWorks()
        {
            IndexRange originalRange = new IndexRange(1, 33);
            int chunkSize = 10;
            int recordCount = 25;

            IndexRanges splitRanges = originalRange.ToIndexList(1, recordCount).ToIndexRanges(chunkSize);

            Assert.AreEqual(3, splitRanges.Count);
            Assert.AreEqual(1, splitRanges[0].RangeBegin);
            Assert.AreEqual(10, splitRanges[0].RangeEnd);
            Assert.AreEqual(11, splitRanges[1].RangeBegin);
            Assert.AreEqual(20, splitRanges[1].RangeEnd);
            Assert.AreEqual(21, splitRanges[2].RangeBegin);
            Assert.AreEqual(recordCount, splitRanges[2].RangeEnd);
        }

        [Test]
        [Category(UnitUtils.NO_DEPENDENCY_CATEGORY)]
        public void ZeroBaseWorks()
        {
            IndexRange originalRange = new IndexRange(0, 20);

            int chunkSize = 15;

            IndexRanges splitRange = originalRange.ToIndexList().ToIndexRanges(chunkSize);
        }
    }
}
