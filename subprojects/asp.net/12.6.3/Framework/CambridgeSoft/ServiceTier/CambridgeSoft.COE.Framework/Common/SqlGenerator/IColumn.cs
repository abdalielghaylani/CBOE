using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator {
    /// <summary>
    /// Interface that defines a DataBase column and it's properties. This class is implemented by Field and Lookup classes.
    /// </summary>
	public interface IColumn {
        /// <summary>
        /// Identifier of the database column
        /// </summary>
		int FieldId {
            get;
            set;
        }

        /// <summary>
        /// Source owner of the column
        /// </summary>
		ISource Table {
            get;
            set;
        }

        /// <summary>
        /// Mime type of the column
        /// </summary>
        COEDataView.MimeTypes MimeType {
            get;
            set;
        }

        /// <summary>
        /// returns the name of the column into a string
        /// </summary>
        /// <returns>the string that contains the name</returns>
		string GetFullyQualifiedNameString();
		string GetNameString();
    }
}
