using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Xml.XPath;
using System.IO;

namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    [Serializable()]
	public class StructureList : BusinessListBase<StructureList, Structure>
    {
        #region Variables

        private string _id = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Identifier
        /// </summary>
        [System.ComponentModel.DataObjectField(true, true)]
        public string ID
        {
            get { return _id; }
        }

        #endregion

        #region Factory Methods

		public static StructureList NewStructureList(string xml)
        {
			return new StructureList(xml);
        }


        #endregion

        #region Xml methods

        /// <summary>
        /// Build this into the custom xml.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateSelf()
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append("<Structures>");
            for (int i = 0; i < this.Count; i++)
                builder.Append(this[i].UpdateSelf());
			builder.Append("</Structures>");
            return builder.ToString();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a ControlList by given xml.
        /// </summary>
        /// <param name="xml"></param>
        private StructureList(string xml)
            : this()
        {
            XPathDocument xDocument = new XPathDocument(new StringReader(xml));
            XPathNavigator xNavigator = xDocument.CreateNavigator();
            XPathNodeIterator xIterator = xNavigator.Select("Structures/Structure");

            if (xIterator.MoveNext())
            {
                do
                {
                    this.Add(Structure.NewStructure(xIterator.Current.OuterXml, false, true));
                } while (xIterator.Current.MoveToNext());
            }

            xIterator = xNavigator.Select("Structures/ID");
            if (xIterator.MoveNext())
                if (!string.IsNullOrEmpty(xIterator.Current.Value))
                    _id = xIterator.Current.Value;
        }

        private StructureList()
        {
        }
        #endregion


    }
}
