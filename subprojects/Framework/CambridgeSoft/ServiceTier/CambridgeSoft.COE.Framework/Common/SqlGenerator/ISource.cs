using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator {
    /// <summary>
    /// Represents a container of columns.
    /// </summary>
	public interface ISource {
        /// <summary>
        /// Gets the alias of the source.
        /// </summary>
        /// <returns>The alias or string.Empty.</returns>
        string GetAlias();

		List<Value> ParamValues {
			get;
			set;
		}

        string ToString(List<Value> paramValues);
    }
}
