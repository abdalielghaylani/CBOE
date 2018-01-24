using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Schema;
using System.ComponentModel;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.Framework.Common.Utility;

namespace CambridgeSoft.COE.Framework.Common
{
    /// JumpTo
    /// <summary>
    /// Write something here.
    /// </summary>

    [Serializable]
    public class JumpTo
    {
        #region Variables
                
        public int FormId;
        public string FormName;
        private FormTypes formType;

        #endregion

        #region enums

        public enum FormTypes
        {
            NONE,
            Web,
            Rich
        };

        #endregion

        #region Properties

        public FormTypes FormType
        {
            get { return formType; }
            set { formType = value; }
        }

        /// <summary>
        /// Identifier of the source dataview default value = -1
        /// </summary>
        private int sourceDataviewId;

        public int SourceDataviewId
        {
            get { return sourceDataviewId; }
            set { sourceDataviewId = value; }
        }

        /// <summary>
        /// name of the source dataview
        /// </summary>
        private string sourceDataviewName;

        public string SourceDataviewName
        {
            get { return sourceDataviewName; }
            set { sourceDataviewName = value; }
        }

        /// <summary>
        /// identifier of the source table
        /// </summary>
        private int sourceTableId;

        public int SourceTableId
        {
            get { return sourceTableId; }
            set { sourceTableId = value; }
        }

        /// <summary>
        /// alias name of the source table
        /// </summary>
        private string sourceTablealias;

        public string SourceTablealias
        {
            get { return sourceTablealias; }
            set { sourceTablealias = value; }
        }

        /// <summary>
        /// identifier of the source field
        /// </summary>
        private int sourceFieldId;

        public int SourceFieldId
        {
            get { return sourceFieldId; }
            set { sourceFieldId = value; }
        }

        /// <summary>
        /// alias name of the source field
        /// </summary>
        private string sourceFieldAlias;

        public string SourceFieldAlias
        {
            get { return sourceFieldAlias; }
            set { sourceFieldAlias = value; }
        }

        /// <summary>
        /// identifier of the target dataview
        /// </summary>
        private int targetDataviewId;

        public int TargetDataviewId
        {
            get { return targetDataviewId; }
            set { targetDataviewId = value; }
        }

        /// <summary>
        /// name of the target dataview
        /// </summary>
        private string targetDataviewName;

        public string TargetDataviewName
        {
            get { return targetDataviewName; }
            set { targetDataviewName = value; }
        }

        /// <summary>
        /// identifier of the target table
        /// </summary>
        private int targetTableId;

        public int TargetTableId
        {
            get { return targetTableId; }
            set { targetTableId = value; }
        }

        /// <summary>
        /// alias name of the target table
        /// </summary>
        private string targetTableAlias;

        public string TargetTableAlias
        {
            get { return targetTableAlias; }
            set { targetTableAlias = value; }
        }

        /// <summary>
        /// identifier of the target field
        /// </summary>
        private int targetFieldId;

        public int TargetFieldId
        {
            get { return targetFieldId; }
            set { targetFieldId = value; }
        }

        /// <summary>
        /// alias name of the target field
        /// </summary>
        private string targetFieldAlias;

        public string TargetFieldAlias
        {
            get { return targetFieldAlias; }
            set { targetFieldAlias = value; }
        }

        #endregion

        #region Constructor

        public JumpTo()
        {
            //
            // TODO: Add constructor logic here
            //
            this.sourceDataviewId = -1;
            this.sourceDataviewName = string.Empty;
            this.sourceTableId = -1;
            this.sourceTablealias = string.Empty;
            this.sourceFieldId = -1;
            this.sourceFieldAlias = string.Empty;
            this.targetDataviewId = -1;
            this.targetDataviewName = string.Empty;
            this.targetTableId = -1;
            this.targetTableAlias = string.Empty;
            this.targetFieldId = -1;
            this.targetFieldAlias = string.Empty;
            this.FormId = 0;
            this.FormName = string.Empty;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Converts the current jumpto object to it's xml representation and returns it into a string
        /// </summary>
        /// <returns>the resulting string containing the xml representation of the jumpto object</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<JumpTo sourceDataviewId=\"" + sourceDataviewId + "\" sourceDataviewName=\"" + sourceDataviewName + "\" sourceTableId=\"" + sourceTableId + "\" sourceTableName=\"" + sourceTablealias + "\" sourceFieldId=\"" + sourceFieldId + "\" sourceFieldName=\"" + sourceFieldAlias + "\" tagetDataviewId=\"" + targetDataviewId + "\" tagetDataviewName=\"" + targetDataviewName + "\" targetTableId=\"" + targetTableId + "\" targetTableName=\"" + targetTableAlias + "\" targetFieldId=\"" + targetFieldId + "\" targetFieldName=\"" + targetFieldAlias + "\" targetFormId=\"" + FormId + "\" targetFormName=\"" + FormName + "\" targetFormType=\"" + FormType.ToString() + "\">");

            builder.Append("</JumpTo>");

            return builder.ToString();
        }

        #endregion
    }
}