using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SharedLib
{
    public interface IChemDataProvider
    {
        bool IsFinished();          // returns true if all the records have been retrieved
        DataSet GetSchema();        // gets a schema out of the data to come
        void Start();               // starts the retrieval
        int GetRecordCount();       // gets the record count for the base table
        bool MoveTo(int index);     // ensures we have data for given record
        int GetPageRow(int index);  // returns row within current page corresponding to index in full list
    }
}
