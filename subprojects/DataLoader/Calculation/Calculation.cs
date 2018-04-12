using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Calculation
{
    public class Calculation
    {
        /********************************************************************************
         * 
        *********************************************************************************/
        protected string strError;
        public string Error
        {
            get
            {
                return strError;
            }
        } // Error
        protected string strErrorSource;
        public string ErrorSource
        {
            get
            {
                return strErrorSource;
            }
        }
        protected int nErrorColumn;
        public int ErrorColumn
        {
            get
            {
                return nErrorColumn;
            }
        }
        protected int cErrorColumns;
        public int ErrorColumns
        {
            get
            {
                return cErrorColumns;
            }
        }
        /*================================================================================
         * DataType
         ================================================================================*/
        protected enum DataType {
            Error, Any,
            Boolean, DateTime, Decimal, Integer, String, Char,
            BooleanArray, DecimalArray, IntegerArray, StringArray, CharArray,
        };

        protected static object DataTypeGetObject(DataType dt)
        {
            object oRet = null;
            switch (dt)
            {
                case DataType.Boolean: oRet = new Boolean(); break;
                case DataType.Char: oRet = new Char(); break;
                case DataType.DateTime: oRet = new DateTime(); break;
                case DataType.Decimal: oRet = new Double(); break;
                case DataType.Integer: oRet = new Int32(); break;
                case DataType.String: oRet = new String(new char[] { '\0' }); break;
                default: oRet = new object(); break;
            } // switch (dt)
            return oRet;
        } // DataTypeGetObject

        protected static DataType TypeGetDataType(string strType)
        {
            DataType dtRet = DataType.Error;
            if (strType.StartsWith("System."))
            {
                strType = strType.Substring("System.".Length);
            }
            bool bArray = strType.EndsWith("[]");
            if (bArray)
            {
                strType = strType.Substring(0, strType.Length - 2); // WJC comment this out to disable arrays
            }
            switch (strType)
            {
                case "Boolean":
                    {
                        dtRet = (bArray) ? DataType.BooleanArray : DataType.Boolean;
                        break;
                    }
                case "Char":
                    {
                        dtRet = (bArray) ? DataType.CharArray : DataType.Char;
                        break;
                    }
                case "DateTime":
                    {
                        dtRet = DataType.DateTime;
                        break;
                    }
                case "Double":
                    {
                        dtRet = (bArray) ? DataType.DecimalArray : DataType.Decimal;
                        break;
                    }
                case "Int32":
                    {
                        dtRet = (bArray) ? DataType.IntegerArray : DataType.Integer;
                        break;
                    }
                case "String":
                    {
                        dtRet = (bArray) ? DataType.StringArray : DataType.String;
                        break;
                    }
                default:
                    {
                        dtRet = DataType.Error;
                        break;
                    }
            }
            return dtRet;
        } // TypeGetDataType()

        /*================================================================================
         * Function
         ================================================================================*/
        protected struct Function
        {
            public string strType;
            public string strMethod;
            public Object oTarget;
            public Type tTarget;
            public DataType[] dtSignature;
            public Function(string vstrType, string vstrMethod, DataType[] vdtSignature)
            {
                strType = vstrType;
                strMethod = vstrMethod;
                oTarget = null;
                tTarget = null;
                dtSignature = vdtSignature;
                return;
            } // Function()
            public Function(string vstrType, string vstrMethod, Object voTarget, Type vtTarget, DataType[] vdtSignature)
            {
                strType = vstrType;
                strMethod = vstrMethod;
                oTarget = voTarget;
                tTarget = vtTarget;
                dtSignature = vdtSignature;
                return;
            } // Function()
        } // struct Function

        /*================================================================================
         * Opcode/Opcodes
         ================================================================================*/
        protected enum OpcodeType
        {
            Array, Binary, Call, CallM, Comma, Convert, Dot, PushB, PushC, PushD, PushF, PushI, PushS, Unary,
            PendingArray, PendingCall, PendingCallM, PendingLParen,
        };
        protected struct Opcode
        {
            public OpcodeType eOpcodeType;
            public string strToken;
            public int nColumn;
            public int cColumns;
            public Function function;
            public Object runtimeObject;
            public Opcode(OpcodeType veOpcodeType, string vstrToken, int vnColumn, int vcColumns, Function vfunction)
            {
                eOpcodeType = veOpcodeType;
                strToken = vstrToken;
                nColumn = vnColumn;
                cColumns = vcColumns;
                function = vfunction;
                runtimeObject = new Object();
                return;
            }
            public Opcode(OpcodeType veOpcodeType, string vstrToken, int vnColumn, int vcColumns, DataType dtToken)
            {
                eOpcodeType = veOpcodeType;
                strToken = vstrToken;
                nColumn = vnColumn;
                cColumns = vcColumns;
                function = new Function();
                runtimeObject = DataTypeGetObject(dtToken);
                return;
            }
            public Opcode(OpcodeType veOpcodeType, string vstrToken, int vnColumn, int vcColumns)
            {
                eOpcodeType = veOpcodeType;
                strToken = vstrToken;
                nColumn = vnColumn;
                cColumns = vcColumns;
                function = new Function();
                runtimeObject = new Object();
                ComputeRuntimeObject();
                return;
            }
            public void ComputeRuntimeObject()
            {
                switch (eOpcodeType)
                {
                    case OpcodeType.PushB:
                        {
                            runtimeObject = Convert.ToBoolean(strToken);
                            break;
                        }
                    case OpcodeType.PushC:
                        {
                            runtimeObject = Convert.ToChar(strToken.Substring(1,1));
                            break;
                        }
                    case OpcodeType.PushD:
                        {
                            runtimeObject = Convert.ToDouble(strToken);
                            break;
                        }
                    case OpcodeType.PushI:
                        {
                            runtimeObject = Convert.ToInt32(strToken);
                            break;
                        }
                    case OpcodeType.PushS:
                        {
                            string strClear = strToken.Substring(1, strToken.Length - 2);
                            strClear = strClear.Replace("\\\\", "\xFF");
                            strClear = strClear.Replace("\\", "");
                            strClear = strClear.Replace("\xFF", "\\");
                            runtimeObject = strClear;
                            break;
                        }
                    default:
                        {
                            runtimeObject = new Object();
                            break;
                        }
                } // switch (opcode.eOpcodeType)
                return;
            }
        } // struct Opcode

        protected struct Opcodes
        {
            public DataType opDataType;
            public List<Opcode> opProgram;
            public Opcodes(DataType dtDataType, Opcode oOpcode)
            {
                opDataType = dtDataType;
                opProgram = new List<Opcode>();
                opProgram.Add(oOpcode);
                return;
            }
            public Opcodes(DataType dtDataType, Opcodes oOpcode1, Opcode oOpcode)
            {
                opDataType = dtDataType;
                opProgram = new List<Opcode>();
                if (oOpcode1.opProgram != null) opProgram.AddRange(oOpcode1.opProgram);
                opProgram.Add(oOpcode);
                return;
            }
            public Opcodes(DataType dtDataType, Opcodes oOpcode1, Opcodes oOpcode2, Opcode oOpcode)
            {
                opDataType = dtDataType;
                opProgram = new List<Opcode>();
                opProgram.AddRange(oOpcode1.opProgram);
                opProgram.AddRange(oOpcode2.opProgram);
                opProgram.Add(oOpcode);
                return;
            }
            public Opcodes(DataType dtDataType, Opcodes oOpcode1, Opcode oOpcode, Opcodes oOpcode2)
            {
                opDataType = dtDataType;
                opProgram = new List<Opcode>();
                if (oOpcode1.opProgram != null) opProgram.AddRange(oOpcode1.opProgram);
                opProgram.Add(oOpcode);
                opProgram.AddRange(oOpcode2.opProgram);
                return;
            }
        } // struct Opcodes

        /*================================================================================
         * Statement
         ================================================================================*/
        protected struct Statement
        {
            public string strSource;
            public Opcodes opcodes;
            public Statement(string vstrSource, Opcodes vOpcodes)
            {
                strSource = vstrSource;
                opcodes = vOpcodes;
                return;
            }
        }

        /*================================================================================
         * listCalculations
         ================================================================================*/
        protected Dictionary<string, Statement> dictCalculations = new Dictionary<string, Statement>();
        public string GetCalculation(string strCalculationName)
        {
            Opcodes thisCalculation = dictCalculations[strCalculationName].opcodes;
            COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
            oCOEXmlTextWriter.WriteStartElement("Calculation");
            oCOEXmlTextWriter.WriteStartElement("Name");
            oCOEXmlTextWriter.WriteString(strCalculationName);
            oCOEXmlTextWriter.WriteEndElement();   // Name
            oCOEXmlTextWriter.WriteStartElement("Source");
            oCOEXmlTextWriter.WriteString(dictCalculations[strCalculationName].strSource);
            oCOEXmlTextWriter.WriteEndElement();   // Source
            oCOEXmlTextWriter.WriteStartElement("ReturnType");
            oCOEXmlTextWriter.WriteString(thisCalculation.opDataType.ToString());
            oCOEXmlTextWriter.WriteEndElement();   // ReturnType
            oCOEXmlTextWriter.WriteStartElement("Opcodes");
            oCOEXmlTextWriter.WriteAttributeString("count", thisCalculation.opProgram.Count.ToString());
            foreach (Opcode opcode in thisCalculation.opProgram)
            {
                oCOEXmlTextWriter.WriteStartElement("Opcode");
                oCOEXmlTextWriter.WriteStartElement("OpcodeType");
                oCOEXmlTextWriter.WriteString(opcode.eOpcodeType.ToString());
                oCOEXmlTextWriter.WriteEndElement();   // OpcodeType
                oCOEXmlTextWriter.WriteStartElement("Token");
                oCOEXmlTextWriter.WriteString(opcode.strToken);
                oCOEXmlTextWriter.WriteEndElement();   // Token
                if (opcode.function.dtSignature != null)
                {
                    oCOEXmlTextWriter.WriteStartElement("Function");
                    oCOEXmlTextWriter.WriteStartElement("Signature");
                    oCOEXmlTextWriter.WriteAttributeString("count", opcode.function.dtSignature.Length.ToString());
                    foreach (DataType dt in opcode.function.dtSignature)
                    {
                        oCOEXmlTextWriter.WriteStartElement("Argument");
                        oCOEXmlTextWriter.WriteString(dt.ToString());
                        oCOEXmlTextWriter.WriteEndElement();   // Argument
                    }
                    oCOEXmlTextWriter.WriteEndElement();   // Signature
                    oCOEXmlTextWriter.WriteStartElement("Type");
                    oCOEXmlTextWriter.WriteString(opcode.function.strType);
                    oCOEXmlTextWriter.WriteEndElement();   // Type
                    oCOEXmlTextWriter.WriteStartElement("Method");
                    oCOEXmlTextWriter.WriteString(opcode.function.strMethod);
                    oCOEXmlTextWriter.WriteEndElement();   // Method
                    oCOEXmlTextWriter.WriteEndElement();   // Function
                } // if (opcode.function.dtSignature != null)
                oCOEXmlTextWriter.WriteStartElement("Column");
                oCOEXmlTextWriter.WriteString(opcode.nColumn.ToString());
                oCOEXmlTextWriter.WriteEndElement();   // Column
                oCOEXmlTextWriter.WriteStartElement("Columns");
                oCOEXmlTextWriter.WriteString(opcode.cColumns.ToString());
                oCOEXmlTextWriter.WriteEndElement();   // opcode.cColumns
                oCOEXmlTextWriter.WriteEndElement();   // Opcode
            } // foreach (Opcode opcode in thisCalculation.opProgram)
            oCOEXmlTextWriter.WriteEndElement();   // Opcodes
            oCOEXmlTextWriter.WriteEndElement();   // Calculation
            string xml = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
            oCOEXmlTextWriter.Close();
            return xml;
        } // GetCalculation()
        /********************************************************************************
         * CalculationSet
        *********************************************************************************/
        public void CalculationSet(string xml)
        {
            Opcodes opcodes = new Opcodes();
            opcodes.opProgram = new List<Opcode>();
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.LoadXml(xml);
            XmlNode oXmlNodeCalculation = oXmlDocument.DocumentElement;
            //Coverity fix - CID 19591
            if (oXmlNodeCalculation != null)
            {
                XmlNode oXmlNodeName = oXmlNodeCalculation.FirstChild;
                string strCalculationName = oXmlNodeName.InnerText;
                XmlNode oXmlNodeSource = oXmlNodeName.NextSibling;
                string strSource = oXmlNodeSource.InnerText;
                XmlNode oXmlNodeReturnType = oXmlNodeSource.NextSibling;
                opcodes.opDataType = (DataType)Enum.Parse(typeof(DataType), oXmlNodeReturnType.InnerText);
                XmlNode oXmlNodeOpcodes = oXmlNodeReturnType.NextSibling;
                foreach (XmlNode oXmlNodeOpcode in oXmlNodeOpcodes)
                {
                    Opcode opcode = new Opcode();
                    XmlNode oXmlNodeOpcodeType = oXmlNodeOpcode.FirstChild;
                    opcode.eOpcodeType = (OpcodeType)Enum.Parse(typeof(OpcodeType), oXmlNodeOpcodeType.InnerText);
                    XmlNode oXmlNodeToken = oXmlNodeOpcodeType.NextSibling;
                    opcode.strToken = oXmlNodeToken.InnerText;
                    XmlNode oXmlNodeFunction = oXmlNodeToken.NextSibling;
                    XmlNode oXmlNodeColumn;
                    if (oXmlNodeFunction.Name == "Function")
                    {
                        XmlNode oXmlNodeSignature = oXmlNodeFunction.FirstChild;
                        XmlAttribute oXmlAttribute = oXmlNodeSignature.Attributes["count"];
                        opcode.function.dtSignature = new DataType[Convert.ToInt32(oXmlAttribute.Value)];
                        XmlNode oXmlNodeArgument = oXmlNodeSignature.FirstChild;
                        for (int nSignature = 0; nSignature < opcode.function.dtSignature.Length; nSignature++)
                        {
                            opcode.function.dtSignature[nSignature] = (DataType)Enum.Parse(typeof(DataType), oXmlNodeArgument.InnerText);
                            oXmlNodeArgument = oXmlNodeArgument.NextSibling;
                        }
                        XmlNode oXmlNodeType = oXmlNodeSignature.NextSibling;
                        opcode.function.strType = oXmlNodeType.InnerText;
                        XmlNode oXmlNodeMethod = oXmlNodeType.NextSibling;
                        opcode.function.strMethod = oXmlNodeMethod.InnerText;
                        oXmlNodeColumn = oXmlNodeFunction.NextSibling;
                    }
                    else
                    {
                        oXmlNodeColumn = oXmlNodeFunction;
                    }
                    opcode.nColumn = Convert.ToInt32(oXmlNodeColumn.InnerText);
                    XmlNode oXmlNodeColumns = oXmlNodeColumn.NextSibling;
                    opcode.cColumns = Convert.ToInt32(oXmlNodeColumns.InnerText);
                    opcode.ComputeRuntimeObject();
                    opcodes.opProgram.Add(opcode);
                }
                if (dictCalculations.ContainsKey(strCalculationName)) dictCalculations.Remove(strCalculationName);
                dictCalculations.Add(strCalculationName, new Statement(strSource, opcodes));
            }
            return;
        } // CalculationSet()
    } // class Calculation
}
