using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems {
    public interface IWhereClauseParser {
        WhereClauseItem CreateInstance(XmlNode criteriaXmlNode, Field dataField);
    }
}
