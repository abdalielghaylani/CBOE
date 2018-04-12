using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.NCDS_DataLoader.Common
{
    #region MappingTypeGeneric
    /// <summary>
    /// _mtGeneric field type
    /// </summary>
    struct MappingTypeGeneric
    {
        // enum
        /// <summary>
        /// _mtGeneric field types
        /// </summary>
        public enum GenericType
        {
            /// <summary>
            /// Unknown
            /// </summary>
            Error,
            /// <summary>
            /// Binary
            /// </summary>
            Binary,
            /// <summary>
            /// Boolean
            /// </summary>
            Boolean,
            /// <summary>
            /// Date
            /// </summary>
            Date,
            /// <summary>
            /// Number than may have a fractional part
            /// </summary>
            Decimal,
            /// <summary>
            /// Integral number
            /// </summary>
            Integer,
            /// <summary>
            /// String
            /// </summary>
            String,
            /// <summary>
            /// Structure
            /// </summary>
            Structure,
            /// <summary>
            /// Structure
            /// </summary>
            Time,
        };
        // data
        private GenericType eGenericType;
        // statics
        static public string[] BasicNames()
        {
            return Enum.GetNames(typeof(MappingTypeGeneric.GenericType));
        }
        static public bool CanAssign(MappingTypeGeneric mtFrom, MappingTypeGeneric mtTo)
        {
            bool bRet;
            GenericType eFrom = mtFrom.eGenericType;
            GenericType eTo = mtTo.eGenericType;
            do
            {
                if ((eFrom == GenericType.Error) || (eTo == GenericType.Error))
                {
                    bRet = false;
                    break;  // Cannot assign from or to Error
                }
                if (eFrom == eTo)
                {
                    bRet = true;
                    break;  // Can assign anything (except Error) to itself
                }
                if ((eFrom == GenericType.Structure) && (eTo == GenericType.Binary))
                {
                    bRet = true;
                    break;  // Can assign Structure to Binary
                }
                if ((eFrom == GenericType.Binary) || (eTo == GenericType.Binary))
                {
                    bRet = false;
                    break;  // Can only assign Binary to Binary (except as above)
                }
                if (eTo == GenericType.String)
                {
                    bRet = true;
                    break;  // Can assign anything (except Error and Binary) to String
                }
                if ((eFrom == GenericType.Integer) && (eTo == GenericType.Decimal))
                {
                    bRet = true;
                    break;  // Can assign Integer to Decimal
                }
                bRet = false;
            } while (false);
            return bRet;
        } // CanAssign()
        static public bool CanParse(MappingTypeGeneric mtThis, string vstrThis)
        {
            bool bRet = true;
            try
            {
                switch (mtThis.eGenericType)
                {
                    case GenericType.Boolean:
                        {
                            bool.Parse(vstrThis);
                            bRet = true;
                            break;
                        }
                    case GenericType.Integer:
                        {
                            Int32.Parse(vstrThis);
                            bRet = true;
                            break;
                        }
                    case GenericType.Date:
                        {
                            DateTime dt = DateTime.Parse(vstrThis);
                            bRet = true;
                            break;
                        }
                    case GenericType.Decimal:
                        {
                            double d = double.Parse(vstrThis);
                            bRet = true;
                            break;
                        }
                    case GenericType.String:
                        {
                            bRet = true;
                            break;
                        }
                    case GenericType.Time:
                        {
                            DateTime dt = DateTime.Parse(vstrThis, System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                            bRet = ((dt.Year == 1) && (dt.Month == 1) && (dt.Day == 1));
                            break;
                        }
                    case GenericType.Binary:
                    case GenericType.Error:
                    case GenericType.Structure:
                        {
                            bRet = false;   // false for now
                            break;
                        }
                } // switch (mtThis.eGenericType)
            }
            catch
            {
                bRet = false;   // false on any exception
            }
            return bRet;
        } // CanParse()
        // Properties
        public string TypeName
        {
            get
            {
                return Enum.GetName(typeof(GenericType), eGenericType);
            }
        }
        public GenericType TypeValue
        {
            get
            {
                return eGenericType;
            }
            set
            {
                eGenericType = value;
            }
        }
        // Constructor
        public MappingTypeGeneric(string strGenericType)
        {
            // Map strGenericType to eGenericType which match eGenericType enum values
            eGenericType = GenericType.Error;
            try
            {
                eGenericType = (GenericType)Enum.Parse(typeof(GenericType), strGenericType, false);
            }
            catch (ArgumentException)
            {
                ;   // Ignore ArgumentError
            }
            return;
        } // MappingTypeGeneric()
    } // MappingTypeGeneric
    #endregion

    #region MappingTypeDb
    /// <summary>
    /// Database field type
    /// </summary>
    struct MappingTypeDb
    {
        #region enum
        // enum
#if MDB_DOC
Access          OleDbType
------          ---------
AutoNumber      Integer
Byte            UnsignedTinyInt
Currency        Currency
DateTime        Date
Decimal         Numeric
Double          Double
Hyperlink       WChar
Integer         SmallInt
LongInteger     Integer
Memo            WChar
Number          Integer
OLEObject       Binary
ReplicationID   Guid
Single          Single
Text            WChar
YesNo           Boolean
#endif
        /// <summary>
        /// Database field types
        /// </summary>
        enum DbType
        {
            /// <summary>
            /// Unknown
            /// </summary>
            Error,
            /// <summary>
            /// MDB OLE Object
            /// </summary>
            Binary,
            /// <summary>
            /// MDB Yes/No
            /// </summary>
            Boolean,
            /// <summary>
            /// MDB Currency
            /// </summary>
            Currency,
            /// <summary>
            /// MDB Date/Time
            /// </summary>
            Date,
            /// <summary>
            /// MDB Number (Double)
            /// </summary>
            Double,
            /// <summary>
            /// MDB Number (Replication ID)
            /// </summary>
            Guid,
            /// <summary>
            /// MDB Number (long)
            /// </summary>
            Integer,
            /// <summary>
            /// MDB Number (Long Integer) or AutoNumber
            /// </summary>
            Numeric,
            /// <summary>
            /// MDB Number (Single)
            /// </summary>
            Single,
            /// <summary>
            /// MDB Number (Integer)
            /// </summary>
            SmallInt,
            /// <summary>
            ///  MDB Number (Byte)
            /// </summary>
            UnsignedTinyInt,
            /// <summary>
            /// MDB Text or Memo or Hyperlink
            /// </summary>
            WChar,
            /// <summary>
            /// C# System.Boolean
            /// </summary>
            System_Boolean,
            /// <summary>
            /// C# System.Byte[]
            /// </summary>
            System_Byte_Array,
            /// <summary>
            /// C# System.DateTime
            /// </summary>
            System_DateTime,
            /// <summary>
            /// C# System.Double
            /// </summary>
            System_Double,
            /// <summary>
            /// C# System.Int16
            /// </summary>
            System_Int16,
            /// <summary>
            /// C# System.Int32
            /// </summary>
            System_Int32,
            /// <summary>
            /// C# System.String
            /// </summary>
            System_String,
            /// <summary>
            /// Special case of System_String which we are pretty sure is a Base64 CDX
            /// </summary>
            Base64Cdx,
            /// <summary>
            /// Special case of System.DateTime that is only a Time
            /// </summary>
            Time,
        };
        #endregion
        #region data
        private DbType _eDbType;
        private MappingTypeGeneric _mtGeneric;
        #endregion
        #region properties
        public string BasicTypeName
        {
            get
            {
                return _mtGeneric.TypeName;
            }
        } // BasicTypeName
        public MappingTypeGeneric.GenericType BasicTypeValue
        {
            get
            {
                return _mtGeneric.TypeValue;
            }
        } // BasicTypeValue
        #endregion
        #region constructors
        public MappingTypeDb(string strDbType)
        {
            // Map strDbType to DbType which match DbType enum values (with some simple replacements)
            _eDbType = DbType.Error;
            try
            {
                string strDbTypeLegal = strDbType;
                strDbTypeLegal = strDbTypeLegal.Replace('.', '_');
                strDbTypeLegal = strDbTypeLegal.Replace("[]", "_Array");
                if (strDbTypeLegal == "null") strDbTypeLegal = "System_String"; // WJC TODO DEBUG THIS
                _eDbType = (DbType)Enum.Parse(_eDbType.GetType(), strDbTypeLegal, false);
            }
            catch (ArgumentException)
            {
                ;   // Ignore ArgumentError
            }
            // Map DbType to GenericType
            switch (_eDbType)
            {
                case DbType.Binary: { _mtGeneric = new MappingTypeGeneric("Binary"); break; }
                case DbType.Boolean: { _mtGeneric = new MappingTypeGeneric("Boolean"); break; }
                case DbType.Currency: { _mtGeneric = new MappingTypeGeneric("Decimal"); break; }
                case DbType.Date: { _mtGeneric = new MappingTypeGeneric("Date"); break; }
                case DbType.Double: { _mtGeneric = new MappingTypeGeneric("Decimal"); break; }
                case DbType.Guid: { _mtGeneric = new MappingTypeGeneric("String"); break; }
                case DbType.Integer: { _mtGeneric = new MappingTypeGeneric("Integer"); break; }
                case DbType.Numeric: { _mtGeneric = new MappingTypeGeneric("Integer"); break; }
                case DbType.Single: { _mtGeneric = new MappingTypeGeneric("Decimal"); break; }
                case DbType.SmallInt: { _mtGeneric = new MappingTypeGeneric("Integer"); break; }
                case DbType.UnsignedTinyInt: { _mtGeneric = new MappingTypeGeneric("Integer"); break; }
                case DbType.WChar: { _mtGeneric = new MappingTypeGeneric("String"); break; }
                case DbType.System_Boolean: { _mtGeneric = new MappingTypeGeneric("Boolean"); break; }
                case DbType.System_Byte_Array: { _mtGeneric = new MappingTypeGeneric("Binary"); break; }
                case DbType.System_DateTime: { _mtGeneric = new MappingTypeGeneric("Date"); break; }
                case DbType.System_Double: { _mtGeneric = new MappingTypeGeneric("Decimal"); break; }
                case DbType.System_Int16: { _mtGeneric = new MappingTypeGeneric("Integer"); break; }
                case DbType.System_Int32: { _mtGeneric = new MappingTypeGeneric("Integer"); break; }
                case DbType.System_String: { _mtGeneric = new MappingTypeGeneric("String"); break; }
                case DbType.Base64Cdx: { _mtGeneric = new MappingTypeGeneric("Structure"); break; }
                case DbType.Time: { _mtGeneric = new MappingTypeGeneric("Time"); break; }
                default: { _mtGeneric = new MappingTypeGeneric("Error"); break; }
            } // switch (_eDbType)
            return;
        } // MappingTypeDb()
        #endregion
        #region methods
        static public bool CanAssign(MappingTypeDb mtFrom, MappingTypeDb mtTo)
        {
            return MappingTypeGeneric.CanAssign(mtFrom._mtGeneric, mtTo._mtGeneric);
        }
        #endregion
    } // MappingTypeDb
    #endregion
}
