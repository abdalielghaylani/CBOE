using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Core
{
    /// <summary>
    /// Container for a list of indices; lightweight reference for any indexed collection
    /// </summary>
    public class IndexList : List<int>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public IndexList() { }

        /// <summary>
        /// Create the new list using a pre-existing set of index-ranges.
        /// </summary>
        /// <param name="indexRanges"></param>
        public IndexList(IndexRange indexRange)
        {
            if (!indexRange.IsInactive)
                PopulateSelf(indexRange);
            this.Sort();
        }

        /// <summary>
        /// Create the new list using a pre-existing set of index-ranges.
        /// </summary>
        /// <param name="indexRanges"></param>
        public IndexList(List<IndexRange> indexRanges)
        {
            foreach (IndexRange indexRange in indexRanges)
            {
                if (!indexRange.IsInactive)
                    PopulateSelf(indexRange);
            }
            this.Sort();
        }

        /// <summary>
        /// Create the new list using a pre-existing set of index-ranges.
        /// </summary>
        /// <param name="indexRanges"></param>
        public IndexList(IndexRanges indexRanges)
        {
            foreach (IndexRange indexRange in indexRanges.Values)
            {
                if (!indexRange.IsInactive)
                    PopulateSelf(indexRange);
            }
            this.Sort();
        }

        /// <summary>
        /// Typical constructor, adopts (and optionally normalizes) a list of indices
        /// </summary>
        /// <param name="indices"></param>
        public IndexList(List<int> indices)
        {
            AddValues(indices);
            this.Sort();
        }

        /// <summary>
        /// Typical constructor, adopts and normalizes) a list of indices based on an [INDEX]
        /// column in a DataTable. Optionally, the caller can provide a row filter to extract
        /// only a subset of the [INDEX] column values.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="indexColumnName"></param>
        public IndexList(System.Data.DataTable table, string indexColumnName, string filter)
        {
            System.Data.DataView view = table.DefaultView;
            if (!string.IsNullOrEmpty(filter))
                view.RowFilter = filter;

            int[] indices = new int[view.Count];
            int indexer = 0;

            for (int viewRowIndex = 0; viewRowIndex < view.Count; viewRowIndex++)
            {
                System.Data.DataRowView viewRow = view[viewRowIndex];
                indices[indexer] = (int)viewRow[indexColumnName];
                indexer++;
            }
            this.AddValues(indices);
        }

        /// <summary>
        /// Typical constructor, adopts (and optionally normalizes) an array of indices
        /// </summary>
        /// <param name="indices"></param>
        public IndexList(int[] indices)
        {
            AddValues(indices);
            this.Sort();
        }

        private void PopulateSelf(IndexRange indexRange)
        {
            // Ensure the list is INCLUSIVE of both start and end values for the range
            int[] indices = new int[indexRange.RangeEnd - indexRange.RangeBegin + 1];
            for (int index = 0; index < indices.Length; index++)
            {
                indices[index] = indexRange.RangeBegin + index;
            }
            AddValues(indices);
        }

        public void AddValues(int[] indices)
        {
            //If we had LINQ ...
            //  IEnumerable<int> result = indices.Distinct();
            //  indices = Enumerable.ToArray(indices);
            //
            //But, since we're not upgrading to .Net 3.5 yet ...
            foreach (int indexNumber in indices)
                if (!this.Contains(indexNumber))
                    this.Add(indexNumber);
        }

        public void AddValues(List<int> indices)
        {
            int[] newIndices = indices.ToArray();
            AddValues(newIndices);
        }

        /// <summary>
        /// Converts the current index list into one or more index ranges.
        /// </summary>
        /// <returns></returns>
        public IndexRanges ToIndexRanges()
        {
            //ensure we sort this list!
            this.Sort();

            //create a container
            IndexRange range = null;
            IndexRanges ranges = new IndexRanges();

            for (int current = 0; current < this.Count; current++)
            {
                if (current == 0)
                {
                    range = new IndexRange();
                    range.RangeBegin = this[current];
                }

                //seek ahead one spot
                int next = current + 1;
                if ((next) <= this.Count - 1)
                {
                    if (this[next] != this[current] + 1)
                    {
                        range.RangeEnd = this[current];
                        ranges.Add(ranges.Count, range);

                        range = new IndexRange();
                        range.RangeBegin = this[next];
                    }
                }
                else
                {
                    range.RangeEnd = this[current];
                    ranges.Add(ranges.Count, range);
                }
            }

            return ranges;
        }

        public IndexRanges ToIndexRanges(int maxListSize)
        {
            //ensure we sort this list!
            this.Sort();

            //create a container
            IndexRange range = null;
            IndexRanges ranges = new IndexRanges();

            for (int current = 0; current < this.Count; current++)
            {
                if (current == 0)
                {
                    range = new IndexRange();
                    range.RangeBegin = this[current];
                }

                //seek ahead one spot
                int next = current + 1;
                if ((next) <= this.Count - 1)
                {
                    if (this[next] != this[current] + 1 || (current != 0 && next % maxListSize == 0))
                    {
                        range.RangeEnd = this[current];
                        ranges.Add(ranges.Count, range);

                        range = new IndexRange();
                        range.RangeBegin = this[next];
                    }
                }
                else
                {
                    range.RangeEnd = this[current];
                    ranges.Add(ranges.Count, range);
                }
            }

            return ranges;

        }
    }
}
