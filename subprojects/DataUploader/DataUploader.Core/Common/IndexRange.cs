using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

/*
 * Virtually every action taken against a data-source file will involve the use of
 * a collection of record indices.
 * 
 * The session context is initialized with a IndexRange, but internally it maintains
 * its active index list as SourceDataIndices.
 * */

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Maintains the a range of source record indices being operated on in the current context.
    /// </summary>
    [Serializable]
    public class IndexRange
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public IndexRange() { }

        /// <summary>
        /// Constructor accepting values expected to create a valid range.
        /// </summary>
        /// <param name="startsAt">the beginning of the range</param>
        /// <param name="finishesAt">the end of the range</param>
        public IndexRange(int startsAt, int finishesAt)
        {
            this._rangeBegin = startsAt;
            this._rangeEnd = finishesAt;
        }

        int _rangeBegin = -1;
        /// <summary>
        /// The caller can set this to -1 to inactivate the range, regardless of the RangeEnd
        /// value.
        /// </summary>
        public int RangeBegin
        {
            get { return _rangeBegin;  }
            set 
            {
                if (value <= _rangeEnd && value >= -1)
                    _rangeBegin = value;
            }
        }
        
        int _rangeEnd = int.MaxValue;
        public int RangeEnd
        {
            get { return _rangeEnd; }
            set 
            {
                if (value >= _rangeBegin)
                    _rangeEnd = value;
            }
        }

        /// <summary>
        /// An index-range is considered active unless specifically inactivated
        /// by virtue of setting the start of the range to -1.
        /// </summary>
        public bool IsInactive
        {
            get { return (_rangeBegin == -1); }
        }

        /// <summary>
        /// Converts an index range into an index list.
        /// </summary>
        /// <returns></returns>
        public IndexList ToIndexList()
        {
            IndexList indices = new IndexList(this);
            return indices;
        }

        /// <summary>
        /// Converts an index range into an index list, trimming the index list
        /// at a specified minimum and maximum values.
        /// </summary>
        /// <param name="lowerIndexLimit">the lower limit used to adjust the range's Begin</param>
        /// <param name="upperIndexLimit">the upper limit used to adjust the range End</param>
        /// <returns></returns>
        public IndexList ToIndexList(int lowerIndexLimit, int upperIndexLimit)
        {
            IndexRange adjustedIndexRange = new IndexRange();
            adjustedIndexRange.RangeBegin = System.Math.Max(lowerIndexLimit, this.RangeBegin);
            adjustedIndexRange.RangeEnd = System.Math.Min(upperIndexLimit, this.RangeEnd);

            IndexList indices = new IndexList(adjustedIndexRange);
            return indices;
        }

    }

    /// <summary>
    /// Simple wrapper for 
    /// </summary>
    [Serializable]
    public class IndexRanges : SortedDictionary<int, IndexRange>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public IndexRanges() { }

        /// <summary>
        /// Converts one or more index ranges into an index list.
        /// </summary>
        /// <returns></returns>
        public IndexList ToIndexList()
        {
            IndexList indices = new IndexList(this);
            return indices;
        }

        /// <summary>
        /// Ensures that overlapping ranges are rectified by recreating the ranges.
        /// </summary>
        public void NormalizeRanges()
        {
            IndexList indices = this.ToIndexList();
            IndexRanges newRanges = indices.ToIndexRanges();
            this.Clear();
            foreach (IndexRange range in newRanges.Values)
                this.Add(this.Count, range);
        }
    }
}
