using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using CambridgeSoft.COE.DataLoader.Calculation;

namespace CambridgeSoft.COE.DataLoader.Calculation.Execution
{
    public class CalculationExecution : Calculation
    {
        private Exception exCalculationException = null;
        public Exception CalculationException
        {
            get
            {
                return exCalculationException;
            }
        }
        /********************************************************************************
         * DataRow
        *********************************************************************************/
        private DataRow oDataRow;
        private Dictionary<string, string> _dictName = new Dictionary<string, string>();
        public bool DataRowBind(DataRow vDataRow, DataTable vDataTable)
        {
            strError = "";
            oDataRow = vDataRow;
            _dictName.Clear();
            foreach (DataColumn oDataColumn in vDataTable.Columns)
            {
                _dictName.Add(oDataColumn.Caption, oDataColumn.ColumnName);
            }
            return (strError.Length > 0);
        } // DataRowBind()
        /********************************************************************************
         * CalculationExecute
        *********************************************************************************/
        public Object CalculationExecute(string strCalculation)
        {
            Object oRet = null;
            exCalculationException = null;
            if (dictCalculations.ContainsKey(strCalculation))
            {
                Stack<Object> stackObjects = new Stack<object>();
                Object result;
                List<Opcode> listOpcode = dictCalculations[strCalculation].opcodes.opProgram;
                foreach (Opcode opcode in listOpcode)
                {
                    switch (opcode.eOpcodeType)
                    {
                        case OpcodeType.Comma:
                            {
                                break;
                            }
                        case OpcodeType.PushB:
                        case OpcodeType.PushC:
                        case OpcodeType.PushD:
                        case OpcodeType.PushI:
                        case OpcodeType.PushS:
                            {
                                stackObjects.Push(opcode.runtimeObject);
                                break;
                            }
                        case OpcodeType.Binary:
                            {
                                //Type target = Type.GetType(opcode.function.strType);
                                Object[] args = new Object[2];
                                args[1] = stackObjects.Pop();
                                args[0] = stackObjects.Pop();
                                try
                                {
                                    result = Type.GetType(opcode.function.strType).InvokeMember(opcode.function.strMethod, BindingFlags.InvokeMethod, null, null, args);
                                }
                                catch (Exception ex)
                                {
                                    exCalculationException = ex;
                                    result = ex;
                                }
                                stackObjects.Push(result);
                                break;
                            }
                        case OpcodeType.Array:
                            {
                                int cElements = (int)stackObjects.Pop();
                                switch (stackObjects.Peek().GetType().FullName)
                                {
                                    case "System.Boolean":
                                        {
                                            Boolean[] array = new Boolean[cElements];
                                            for (int nElement = cElements - 1; nElement >= 0; nElement--)
                                            {
                                                array[nElement] = (Boolean)stackObjects.Pop();
                                            }
                                            stackObjects.Push(array);
                                            break;
                                        }
                                    case "System.Char":
                                        {
                                            Char[] array = new Char[cElements];
                                            for (int nElement = cElements - 1; nElement >= 0; nElement--)
                                            {
                                                array[nElement] = (Char)stackObjects.Pop();
                                            }
                                            stackObjects.Push(array);
                                            break;
                                        }
                                    case "System.Double":
                                        {
                                            Double[] array = new Double[cElements];
                                            for (int nElement = cElements - 1; nElement >= 0; nElement--)
                                            {
                                                array[nElement] = (Double)stackObjects.Pop();
                                            }
                                            stackObjects.Push(array);
                                            break;
                                        }
                                    case "System.Int32":
                                        {
                                            Int32[] array = new Int32[cElements];
                                            for (int nElement = cElements - 1; nElement >= 0; nElement--)
                                            {
                                                array[nElement] = (Int32)stackObjects.Pop();
                                            }
                                            stackObjects.Push(array);
                                            break;
                                        }
                                    case "System.String":
                                        {
                                            string[] array = new string[cElements];
                                            for (int nElement = cElements - 1; nElement >= 0; nElement--)
                                            {
                                                array[nElement] = (string)stackObjects.Pop();
                                            }
                                            stackObjects.Push(array);
                                            break;
                                        }
                                    default:
                                        {
                                            Object[] array = new Object[cElements];
                                            for (int nElement = cElements - 1; nElement >= 0; nElement--)
                                            {
                                                array[nElement] = (Object)stackObjects.Pop();
                                            }
                                            stackObjects.Push(array);
                                            break;
                                        }
                                }
                                break;
                            }
                        case OpcodeType.Call:
                            {
                                Object oTarget = opcode.function.oTarget;
                                int cArgs = opcode.function.dtSignature.Length - 1;
                                Object[] args = new Object[cArgs];
                                int nNull = -1;
                                for (int nArg = cArgs - 1; nArg >= 0; nArg--)
                                {
                                    args[nArg] = stackObjects.Pop();
                                }
                                if (nNull == -1)
                                {
                                    try
                                    {
                                        result = Type.GetType(opcode.function.strType).InvokeMember(opcode.function.strMethod, BindingFlags.InvokeMethod, null, oTarget, args);
                                    }
                                    catch (Exception ex)
                                    {
                                        exCalculationException = ex;
                                        result = ex;
                                    }
                                    stackObjects.Push(result);
                                }
                                else
                                {
                                    stackObjects.Push(args[nNull]);
                                }
                                break;
                            } // case OpcodeType.Call
                        case OpcodeType.CallM:
                            {
                                Object oTarget = stackObjects.Pop();
                                int cArgs = opcode.function.dtSignature.Length - 1;
                                Object[] args = new Object[cArgs];
                                int nNull = -1;
                                for (int nArg = cArgs - 1; nArg >= 0; nArg--)
                                {
                                    args[nArg] = stackObjects.Pop();
                                }
                                if (nNull == -1)
                                {
                                    try
                                    {
                                        result = Type.GetType(opcode.function.strType).InvokeMember(opcode.function.strMethod, BindingFlags.InvokeMethod, null, oTarget, args);
                                    }
                                    catch (Exception ex)
                                    {
                                        exCalculationException = ex;
                                        result = ex;
                                    }
                                    stackObjects.Push(result);
                                }
                                else
                                {
                                    stackObjects.Push(args[nNull]);
                                }
                                break;
                            } // case OpcodeType.Call
                        case OpcodeType.Convert:
                        case OpcodeType.Unary:
                            {
                                //Type target = Type.GetType(opcode.function.strType);
                                Object[] args = new Object[1];
                                int nNull = -1;
                                args[0] = stackObjects.Pop();
                                if (nNull == -1)
                                {
                                    try
                                    {
                                        result = Type.GetType(opcode.function.strType).InvokeMember(opcode.function.strMethod, BindingFlags.InvokeMethod, null, null, args);
                                    }
                                    catch (Exception ex)
                                    {
                                        exCalculationException = ex;
                                        result = ex;
                                    }
                                    stackObjects.Push(result);
                                }
                                else
                                {
                                    stackObjects.Push(args[nNull]);
                                }
                                break;
                            }
                        case OpcodeType.PushF:
                            {
                                stackObjects.Push(oDataRow[_dictName[opcode.strToken]]);
                                break;
                            }
                        default:
                            {
                                throw new Exception("Unknown opcode");
                            }
                    } // switch (opcode.eOpcodeType)
                    if (exCalculationException != null)
                    {
                        strErrorSource = dictCalculations[strCalculation].strSource;
                        nErrorColumn = opcode.nColumn;
                        cErrorColumns = opcode.cColumns;
                        break;
                    }
                } // foreach (Opcode opcode in listOpcode)
                if (exCalculationException == null)
                {
                    result = stackObjects.Pop();
                    oRet = result;
                }
            } // if (dictCalculations.ContainsKey(strCalculation))
            return oRet;
        } // CalculationExecute()
    } // class CalculationExecution
}
