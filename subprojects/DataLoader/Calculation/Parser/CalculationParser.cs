using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using CambridgeSoft.COE.DataLoader.Calculation.Execution;

namespace CambridgeSoft.COE.DataLoader.Calculation.Parser
{
    public class CalculationParser : CalculationExecution
    {

        /********************************************************************************
         * 
        *********************************************************************************/
        public CalculationParser()
        {
            ObjectAddOperators(new BuiltinOperators());
            dictOperators.Add(".", new Operator(".", 0, false, 0, new List<Function>(new Function[] {
                new Function(null, ".", new DataType[] { }),
            })));
            dictOperators.Add("(", new Operator("(", 0, false, 0, new List<Function>(new Function[] {
                new Function(null, "(", new DataType[] { }),
            })));
            dictOperators.Add(")", new Operator(")", 0, true, 0, new List<Function>(new Function[] {
                new Function(null, ")", new DataType[] { }),
            })));
            dictOperators.Add("{", new Operator("{", 0, false, 0, new List<Function>(new Function[] {
                new Function(null, "{", new DataType[] { }),
            })));
            dictOperators.Add("}", new Operator("}", 0, true, 0, new List<Function>(new Function[] {
                new Function(null, "}", new DataType[] { }),
            })));
            dictOperators.Add(",", new Operator(",", 1, true, 2, new List<Function>(new Function[] {
                new Function(null, ",", new DataType[] { DataType.Boolean, DataType.Any, DataType.Boolean }),
                new Function(null, ",", new DataType[] { DataType.Decimal, DataType.Any, DataType.Decimal }),
                new Function(null, ",", new DataType[] { DataType.Integer,  DataType.Any, DataType.Integer }),
                new Function(null, ",", new DataType[] { DataType.String,  DataType.Any, DataType.String }),
            })));
            ObjectAddConversions(new BuiltinConversions());
            ObjectAddFunctions(new BuiltinFunctions());
            return;
        }
        /********************************************************************************
         * 
        *********************************************************************************/
        [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple=false)]
        public class OperatorInfo : System.Attribute
        {
            public string strToken;
            public int nPrecedence;
            public bool bLeftAssociative;
            public OperatorInfo(string vstrToken, int vnPrecedence, bool vbLeftAssociative)
            {
                strToken = vstrToken;
                nPrecedence = vnPrecedence;
                bLeftAssociative = vbLeftAssociative;
                return;
            }
        } // OperatorInfo
        /********************************************************************************
         ********************************************************************************
         * New stuff
         ********************************************************************************
        ********************************************************************************/
        private enum ParseState { Data, Opcode, Name };
        private Tokenizer oTokenizer = new Tokenizer();
        private List<Opcodes> opcodesList = null;
        private Stack<List<Opcodes>> opcodesListStack = new Stack<List<Opcodes>>();
        private List<Operator> operatorList = null;
        private Stack<List<Operator>> operatorListStack = new Stack<List<Operator>>();
        private List<int> operatorColumnList = null;
        private Stack<List<int>> operatorColumnListStack = new Stack<List<int>>();
        private List<Function> signature = null;
        private Stack<List<Function>> signatureStack = new Stack<List<Function>>();
        private int arguments;
        private Stack<int> argumentsStack = new Stack<int>();
        private ParseState eParseState;
        private string strFalse = false.ToString().ToUpper();
        private string strTrue = true.ToString().ToUpper();
        private bool CalculationAdd(string strCalculationName, DataType dtReturn, string strSource)
        {
            strError = "";
            strErrorSource = "";
            nErrorColumn = -1;
            cErrorColumns = 0;
            if (dictCalculations.ContainsKey(strCalculationName)) dictCalculations.Remove(strCalculationName);
            //
            bool bTrace = false;
            oTokenizer.Source = "(" + strSource + ")";
            opcodesListStack.Clear();
            opcodesList = new List<Opcodes>();
            operatorListStack.Clear();
            operatorColumnList = new List<int>();
            operatorColumnListStack.Clear();
            signatureStack.Clear();
            signature = new List<Function>(); signature.Add(new Function(null, "", new DataType[] { DataType.Any, DataType.Any}));
            arguments = 0;
            //
            string strToken = "";
            Tokenizer.TokenClass eClass = Tokenizer.TokenClass.Empty;
            int nStartingPosition = 0;
            //
            eParseState = ParseState.Data;
            do
            {
                oTokenizer.ReadToken(ref strToken, ref eClass, ref nStartingPosition);
                int nColumn = nStartingPosition - 1;   // -1 because of the extra () we put on
                int cColumns = strToken.Length;
                if (bTrace) Console.WriteLine("@" + nStartingPosition + " " + strToken + " (" + eClass + ")");
                DataType dtThis = TokenClassGetDataType(eClass);
                switch (eClass)
                {
                    case Tokenizer.TokenClass.Char:
                        {
                            if (eParseState != ParseState.Data)
                            {
                                strError = "Data not expected";
                                break;
                            }
                            opcodesList.Add(new Opcodes(dtThis, new Opcode(OpcodeType.PushC, strToken, nColumn, cColumns)));
                            eParseState = ParseState.Opcode;
                            break;
                        } // Char
                    case Tokenizer.TokenClass.Decimal:
                        {
                            if (eParseState != ParseState.Data)
                            {
                                strError = "Data not expected";
                                break;
                            }
                            if ((operatorList.Count > 0) && operatorList[operatorList.Count - 1].opToken == "U-")
                            {
                                // Optimize by transferring the unary minus to in front of the decimal
                                operatorList.RemoveAt(operatorList.Count - 1);
                                strToken = "-" + strToken;
                            }
                            opcodesList.Add(new Opcodes(dtThis, new Opcode(OpcodeType.PushD, strToken, nColumn, cColumns)));
                            eParseState = ParseState.Opcode;
                            break;
                        } // Decimal
                    case Tokenizer.TokenClass.Integer:
                        {
                            if (eParseState != ParseState.Data)
                            {
                                strError = "Data not expected";
                                break;
                            }
                            if ((operatorList.Count > 0) && operatorList[operatorList.Count - 1].opToken == "U-")
                            {
                                // Optimize by transferring the unary minus to in front of the integer
                                operatorList.RemoveAt(operatorList.Count - 1);
                                strToken = "-" + strToken;
                            }
                            opcodesList.Add(new Opcodes(dtThis, new Opcode(OpcodeType.PushI, strToken, nColumn, cColumns)));
                            eParseState = ParseState.Opcode;
                            break;
                        } // Integer
                    case Tokenizer.TokenClass.String:
                        {
                            if (eParseState != ParseState.Data)
                            {
                                strError = "Data not expected";
                                break;
                            }
                            opcodesList.Add(new Opcodes(dtThis, new Opcode(OpcodeType.PushS, strToken, nColumn, cColumns)));
                            eParseState = ParseState.Opcode;
                            break;
                        } // String
                    case Tokenizer.TokenClass.Error:
                        {
                            strError = "Erroneous token";
                            break;
                        }
                    case Tokenizer.TokenClass.Illegal:
                        {
                            strError = "Illegal token";
                            break;
                        }
                    case Tokenizer.TokenClass.Indirect:
                        {
                            if (eParseState != ParseState.Data)
                            {
                                strError = "Field not expected";
                                break;
                            }
                            string strField = strToken.Substring(1, strToken.Length - 2);  // Remove []
                            if (dictFields.ContainsKey(strField.ToUpper()) == false)
                            {
                                strError = "Field not recognized";
                                break;
                            }
                            opcodesList.Add(new Opcodes(dictFields[strField.ToUpper()].dtColumn, new Opcode(OpcodeType.PushF, dictFields[strField.ToUpper()].ColumnName, nColumn, cColumns, dictFields[strField.ToUpper()].dtColumn)));
                            eParseState = ParseState.Opcode;
                            break;
                        } // Indirect
                    case Tokenizer.TokenClass.Name:
                        {
                            if (eParseState == ParseState.Name)
                            {
                                if (dictFunctions.ContainsKey(strToken.ToUpper()) == false)
                                {
                                    strError = "Function not recognized";
                                    break;
                                }
                                Opcodes codeDot = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                Opcodes codeTarget = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                DataType dtTarget = codeTarget.opDataType;
                                int nFunction = 0;  //+ Use function #0 but this may be replaced later
                                List<Function> functions = dictFunctions[strToken.ToUpper()];
                                for (nFunction = 0; nFunction < functions.Count; nFunction++)
                                {
                                    Type tTarget = functions[nFunction].tTarget;
                                    if (tTarget == null) continue;
                                    if (TypeGetDataType(tTarget.ToString()) == dtTarget)
                                    {
                                        break;
                                    };
                                }
                                if (nFunction >= functions.Count)
                                {
                                    strError = "Wrong object type for function";
                                    break;
                                }
                                opcodesList.Add(new Opcodes(dictFunctions[strToken.ToUpper()][nFunction].dtSignature[0], new Opcodes(), new Opcode(OpcodeType.PendingCallM, strToken, nColumn, cColumns, dictFunctions[strToken.ToUpper()][nFunction]), codeTarget));
                                eParseState = ParseState.Opcode;
                            }
                            else if (eParseState == ParseState.Data)
                            {
                                // Special handling of True and False that look like strings to the tokenizer
                                if ((strToken.ToUpper() == strTrue) || (strToken.ToUpper() == strFalse))
                                {
                                    opcodesList.Add(new Opcodes(DataType.Boolean, new Opcode(OpcodeType.PushB, strToken, nColumn, cColumns)));
                                    eParseState = ParseState.Opcode;    // ??? argument check
                                    break;
                                }
                                // The token could also be a field without the [] braces
                                if (dictFields.ContainsKey(strToken.ToUpper()))
                                {
                                    opcodesList.Add(new Opcodes(dictFields[strToken.ToUpper()].dtColumn, new Opcode(OpcodeType.PushF, dictFields[strToken.ToUpper()].ColumnName, nColumn, cColumns, dictFields[strToken.ToUpper()].dtColumn)));
                                    eParseState = ParseState.Opcode;
                                    break;
                                }
                                if (dictFunctions.ContainsKey(strToken.ToUpper()) == false)
                                {
                                    strError = "Function not recognized";
                                    break;
                                }
                                int nFunction = 0;  //+ Use function #0 but this may be replaced later
                                List<Function> functions = dictFunctions[strToken.ToUpper()];
                                for (nFunction = 0; nFunction < functions.Count; nFunction++)
                                {
                                    if ((functions[nFunction].tTarget == null) || (functions[nFunction].oTarget != null))
                                    {
                                        break;
                                    };
                                }
                                if (nFunction >= functions.Count)
                                {
                                    strError = "Object required for function";
                                    break;
                                }
                                opcodesList.Add(new Opcodes(dictFunctions[strToken.ToUpper()][nFunction].dtSignature[0], new Opcode(OpcodeType.PendingCall, strToken, nColumn, cColumns, dictFunctions[strToken.ToUpper()][nFunction])));
                                eParseState = ParseState.Opcode;
                            }
                            else
                            {
                                strError = "Function not expected";
                            }
                            break;
                        } // Name
                    case Tokenizer.TokenClass.Operator:
                        {
                            /********************************************************************************
                             * Handle Unary operators right away.
                             * OK since we know a priori that they have the highest precedence.
                            ********************************************************************************/
                            if (eParseState == ParseState.Data)
                            {
                                if (dictOperators.ContainsKey("U" + strToken))
                                {
                                    if (strToken == "+")
                                    {
                                        // Optimizing out unary plus
                                        break;  // Done with unary plus
                                    }
                                    if ((strToken == "-") && (operatorList.Count > 0))
                                    {
                                        Operator operatorTop = operatorList[operatorList.Count - 1];
                                        if (operatorTop.opToken == "U-")
                                        {
                                            // Optimize out back-to-back unary minus
                                            operatorList.RemoveAt(operatorList.Count - 1);
                                            break;  // Done with -- 
                                        }
                                    }
                                    Operator operatorUnary = dictOperators["U" + strToken];
                                    operatorList.Add(operatorUnary); operatorColumnList.Add(nColumn);
                                    break; // Done with other unary operators
                                }
                            } // if (eParseState == ParseState.Data)

                            /********************************************************************************
                             * Now make sure it's an operator; and expected
                            ********************************************************************************/
                            if (dictOperators.ContainsKey(strToken) == false)
                            {
                                strError = "No such operator";
                                break;  // Error
                            }
                            Operator operatorThis = dictOperators[strToken];
                            if (eParseState == ParseState.Data)
                            {
                                if ((strToken != "(") && (strToken != "{") && (strToken != ")") && (strToken != "}")) // Exceptions
                                {
                                    strError = "Operator not expected";
                                    break;  // Error
                                }
                            }
                            else if (eParseState != ParseState.Opcode)
                            {
                                strError = "Operator not expected";
                                break;  // Error
                            }

                            /********************************************************************************
                             * Handle dot
                            ********************************************************************************/
                            if (operatorThis.opToken == ".")
                            {
                                if (eParseState != ParseState.Opcode)
                                {
                                    strError = "Dot not expected.";
                                    break;
                                }
                                opcodesList.Add(new Opcodes(DataType.Any, new Opcode(OpcodeType.Dot, "", nColumn, 1)));
                                eParseState = ParseState.Name;
                                break;  // Done with dot, expecting Name
                            }

                            /********************************************************************************
                             * Handle left parenthesis.
                            ********************************************************************************/
                            if (operatorThis.opToken == "(")
                            {
                                // Left parenthesis
                                if (eParseState != ParseState.Opcode)
                                {
                                    // Normal left parenthesis is treated like an unnamed function with one parameter
                                    opcodesListStack.Push(opcodesList);
                                    opcodesList = new List<Opcodes>();
                                    opcodesList.Add(new Opcodes(DataType.Any, new Opcode(OpcodeType.PendingLParen, "", nColumn, cColumns)));
                                    signatureStack.Push(signature);
                                    signature = new List<Function>(); signature.Add(new Function(null, "", new DataType[] { DataType.Any, DataType.Any }));
                                    argumentsStack.Push(arguments);
                                    arguments = 0;
                                }
                                else
                                {
                                    // Left parenthesis after a function name
                                    Opcodes codeFragment = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                    bool bCallM = (codeFragment.opProgram[0].eOpcodeType == OpcodeType.PendingCallM);
                                    DataType dtTarget = DataType.Error;
                                    if (bCallM)
                                    {
                                        if (codeFragment.opProgram[1].function.dtSignature != null)
                                        {
                                            dtTarget = codeFragment.opProgram[1].function.dtSignature[0];
                                        }
                                        else
                                        {
                                            dtTarget = TypeGetDataType(codeFragment.opProgram[1].runtimeObject.GetType().ToString());
                                        }
                                    }
                                    opcodesListStack.Push(opcodesList);
                                    opcodesList = new List<Opcodes>();
                                    opcodesList.Add(codeFragment);
                                    signatureStack.Push(signature);
                                    // signature actually needs to be a copy of the List<Function>
                                    signature = new List<Function>();
                                    if (dictFunctions.ContainsKey(codeFragment.opProgram[0].strToken.ToUpper()) == false)
                                    {
                                        strError = "Opening parenthesis not expected";
                                        break;
                                    }
                                    foreach (Function function in dictFunctions[codeFragment.opProgram[0].strToken.ToUpper()])
                                    {
                                        if (bCallM)
                                        {
                                            if (function.tTarget == null) continue;
                                            if (TypeGetDataType(function.tTarget.ToString()) == dtTarget)
                                            {
                                                signature.Add(function);
                                            }
                                        }
                                        else
                                        {
                                            if ((function.tTarget == null) || (function.oTarget != null))
                                            {
                                                signature.Add(function);
                                            }
                                        }
                                    } // foreach (Function function in dictFunctions[codeFragment.opProgram[0].strToken.ToUpper()])
                                    argumentsStack.Push(arguments);
                                    arguments = 0;
                                }
                                operatorListStack.Push(operatorList); operatorList = new List<Operator>();
                                operatorColumnListStack.Push(operatorColumnList); operatorColumnList = new List<int>();
                                operatorList.Add(operatorThis); operatorColumnList.Add(nColumn);
                                eParseState = ParseState.Data;
                                break;  // Done with left parenthesis
                            } // if (operatorThis.opToken == "(")

                            /********************************************************************************
                             * Handle left curly brace
                            ********************************************************************************/
                            if (operatorThis.opToken == "{")
                            {
                                // Left curly brace
                                if (eParseState == ParseState.Opcode)
                                {
                                    strError = "Opening curly brace not expected";
                                    break;  // Error
                                }
                                if ((opcodesList[0].opProgram[0].eOpcodeType != OpcodeType.PendingCall) && (opcodesList[0].opProgram[0].eOpcodeType != OpcodeType.PendingCallM))
                                {
                                    strError = "Opening curly brace not expected";
                                    break;  // Error
                                }
                                {
                                    List<Function> revised = new List<Function>();
                                    foreach (Function function in signature)
                                    {
                                        DataType thisDataType = function.dtSignature[1 + arguments];
                                        if (thisDataType.ToString().EndsWith("Array"))
                                        {
                                            revised.Add(function);
                                        }
                                    }
                                    signature = revised;    // drop signatures that do not have an array in this position
                                }
                                if (signature.Count == 0)
                                {
                                    strError = "Opening curly brace not expected";
                                    break;  // Error
                                }
                                // Left curly brace implied function name (Array)
                                opcodesListStack.Push(opcodesList);
                                opcodesList = new List<Opcodes>();
                                opcodesList.Add(new Opcodes(DataType.Any, new Opcode(OpcodeType.PendingArray, "", nColumn, cColumns)));
                                signatureStack.Push(signature);
                                List<Function> singleSignature = new List<Function>();
                                foreach (Function function in signature)
                                {
                                    DataType arrayDataType = function.dtSignature[1 + arguments];
                                    string strArrayDataType = arrayDataType.ToString();
                                    string strSingleDataType = strArrayDataType.Substring(0, strArrayDataType.Length - "Array".Length);
                                    DataType singleDataType = (DataType)Enum.Parse(typeof(DataType), strSingleDataType);
                                    singleSignature.Add(new Function(function.strType, function.strMethod, new DataType[] { arrayDataType, singleDataType }));
                                }
                                signature = singleSignature;
                                argumentsStack.Push(arguments);
                                arguments = 1;
                                operatorListStack.Push(operatorList); operatorList = new List<Operator>();
                                operatorColumnListStack.Push(operatorColumnList); operatorColumnList = new List<int>();
                                operatorList.Add(operatorThis); operatorColumnList.Add(nColumn);
                                eParseState = ParseState.Data;
                                break;  // Done with left curly brace
                            } // if (operatorThis.opToken == "{")

                            /********************************************************************************
                             * Allow function calls with no parentheses if no arguments is legal
                            ********************************************************************************/
                            if (opcodesList.Count >= 2)
                            {
                                if (
                                    ((opcodesList[opcodesList.Count - 1].opProgram[0].eOpcodeType == OpcodeType.PendingCall) && (opcodesList[opcodesList.Count - 1].opProgram.Count == 1)) ||
                                    ((opcodesList[opcodesList.Count - 1].opProgram[0].eOpcodeType == OpcodeType.PendingCallM) && (opcodesList[opcodesList.Count - 1].opProgram.Count == 2))
                                )
                                {
                                    {
                                        // Left parenthesis after a function name
                                        Opcodes codeFragment = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                        bool bCallM = (codeFragment.opProgram[0].eOpcodeType == OpcodeType.PendingCallM);
                                        DataType dtTarget = DataType.Error;
                                        if (bCallM)
                                        {
                                            if (codeFragment.opProgram[1].function.dtSignature != null)
                                            {
                                                dtTarget = codeFragment.opProgram[1].function.dtSignature[0];
                                            }
                                            else
                                            {
                                                dtTarget = TypeGetDataType(codeFragment.opProgram[1].runtimeObject.GetType().ToString());
                                            }
                                        }
                                        opcodesListStack.Push(opcodesList);
                                        opcodesList = new List<Opcodes>();
                                        opcodesList.Add(codeFragment);
                                        signatureStack.Push(signature);
                                        // signature actually needs to be a copy of the List<Function>
                                        signature = new List<Function>();
                                        foreach (Function function in dictFunctions[codeFragment.opProgram[0].strToken.ToUpper()])
                                        {
                                            if (bCallM)
                                            {
                                                if (function.tTarget == null) continue;
                                                if (TypeGetDataType(function.tTarget.ToString()) == dtTarget)
                                                {
                                                    signature.Add(function);
                                                }
                                            }
                                            else
                                            {
                                                if ((function.tTarget == null) || (function.oTarget != null))
                                                {
                                                    signature.Add(function);
                                                }
                                            }
                                        }
                                        if (signature.Count != 1)
                                        {
                                            strError = "Left parenthesis expected.";
                                            break;
                                        }
                                        argumentsStack.Push(arguments);
                                        arguments = 0;
                                        operatorListStack.Push(operatorList); operatorList = new List<Operator>();
                                        operatorColumnListStack.Push(operatorColumnList); operatorColumnList = new List<int>();
                                        operatorList.Add(operatorThis); operatorColumnList.Add(nColumn);
                                    }
                                    {
                                        // Pop up a level as a result of the closing parenthesis or curly brace
                                        Operator opcode = operatorList[operatorList.Count - 1]; operatorList.RemoveAt(operatorList.Count - 1);
                                        int nOperatorColumn = operatorColumnList[operatorColumnList.Count - 1]; operatorColumnList.RemoveAt(operatorColumnList.Count - 1);
                                        operatorList = operatorListStack.Pop();
                                        operatorColumnList = operatorColumnListStack.Pop();
                                        Opcodes codeFragment = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                        Opcodes codeFunction;
                                        {
                                            // Empty parenthesis
                                            codeFunction = codeFragment;
                                            codeFragment = new Opcodes();
                                        }
                                        opcodesList = opcodesListStack.Pop();
                                        // True function call
                                        {
                                            Function function = codeFunction.opProgram[0].function;
                                            if ((function.tTarget != null) && (function.oTarget == null))
                                            {
                                                Opcode opcodeCall = codeFunction.opProgram[0];
                                                codeFunction.opProgram.RemoveAt(0);
                                                opcodeCall.eOpcodeType = OpcodeType.CallM;
                                                opcodeCall.function = signature[0];
                                                codeFragment.opDataType = signature[0].dtSignature[0];
                                                if (codeFragment.opProgram == null)
                                                {
                                                    codeFragment.opProgram = new List<Opcode>();
                                                }
                                                codeFragment.opProgram.AddRange(codeFunction.opProgram);
                                                codeFragment.opProgram.Add(opcodeCall);
                                                opcodesList.Add(codeFragment);
                                            }
                                            else
                                            {
                                                opcodesList.Add(new Opcodes(signature[0].dtSignature[0], codeFragment, new Opcode(OpcodeType.Call, codeFunction.opProgram[0].strToken, codeFunction.opProgram[0].nColumn, codeFunction.opProgram[0].cColumns, signature[0])));
                                            }
                                        }
                                        if (bTrace) Console.WriteLine("Emit:" + opcodesList[opcodesList.Count - 1]);
                                        signature = signatureStack.Pop();
                                        arguments = argumentsStack.Pop();
                                    }
                                }
                            } // if (opcodesList.Count >= 2) ...

                            if (eParseState == ParseState.Opcode)
                            {
                                /********************************************************************************
                                 * Clear out unary operators
                                ********************************************************************************/
                                while ((operatorList.Count > 0) && (operatorList[operatorList.Count - 1].opOperands == 1))
                                {
                                    Operator opcode = operatorList[operatorList.Count - 1]; operatorList.RemoveAt(operatorList.Count - 1);
                                    int nOperatorColumn = operatorColumnList[operatorColumnList.Count - 1]; operatorColumnList.RemoveAt(operatorColumnList.Count - 1);
                                    Opcodes codeFragment = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                    Function function = LookupFunction(opcode.opFunctions, new DataType[] { codeFragment.opDataType });
                                    opcodesList.Add(new Opcodes(codeFragment.opDataType, codeFragment, new Opcode(OpcodeType.Unary, opcode.opToken.Substring(1), nOperatorColumn, opcode.opToken.Length - 1, function)));
                                    if (bTrace) Console.WriteLine("Emit:" + opcodesList[opcodesList.Count - 1]);
                                }

                                /********************************************************************************
                                 * Clear out binary operators based on precedence and associativity
                                ********************************************************************************/
                                while ((operatorList.Count > 0) && (operatorList[operatorList.Count - 1].opOperands == 2))
                                {
                                    Operator operatorTop = operatorList[operatorList.Count - 1];
                                    int nOperatorTopColumn = operatorColumnList[operatorColumnList.Count - 1];
                                    if (
                                        (operatorThis.opPredence < operatorTop.opPredence) ||
                                        ((operatorThis.opPredence == operatorTop.opPredence) && operatorThis.opLeftAssociative)
                                    )
                                    {
                                        Operator opcode = operatorList[operatorList.Count - 1]; operatorList.RemoveAt(operatorList.Count - 1);
                                        int nOperatorColumn = operatorColumnList[operatorColumnList.Count - 1]; operatorColumnList.RemoveAt(operatorColumnList.Count - 1);
                                        Opcodes codeFragmentRht = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                        Opcodes codeFragmentLft = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                        Function function;
                                        DataType dt;
                                        if (opcode.opToken == ",")
                                        {
                                            // Comma is a special case
                                            function = new Function();
                                            dt = codeFragmentRht.opDataType;
                                            opcodesList.Add(new Opcodes(dt, codeFragmentLft, new Opcode(OpcodeType.Comma, "", nOperatorColumn, 1, function), codeFragmentRht));
                                        }
                                        else
                                        {
                                            // Ordinary binary operator, not a comma
                                            function = LookupFunction(opcode.opFunctions, new DataType[] { codeFragmentLft.opDataType, codeFragmentRht.opDataType });
                                            if (function.dtSignature == null)
                                            {
                                                // Not a perfect match
                                                Operator[] conversion = new Operator[0];
                                                function = LookupFunctionWithConversion(opcode.opFunctions, new DataType[] { codeFragmentLft.opDataType, codeFragmentRht.opDataType }, ref conversion);
                                                if (function.dtSignature == null)
                                                {
                                                    strError = "Data type mismatch";
                                                    Opcode topData = codeFragmentRht.opProgram[codeFragmentRht.opProgram.Count - 1];
                                                    nColumn = topData.nColumn;
                                                    cColumns = topData.cColumns;
                                                    break;
                                                }
                                                // Apply conversion(s)
                                                dt = function.dtSignature[0];
                                                if (conversion[0].opFunctions != null)
                                                {
                                                    codeFragmentLft = new Opcodes(conversion[0].opFunctions[0].dtSignature[0], codeFragmentLft, new Opcode(OpcodeType.Convert, conversion[0].opToken, nOperatorColumn, 0, conversion[0].opFunctions[0]));
                                                }
                                                if (conversion[1].opFunctions != null)
                                                {
                                                    codeFragmentRht = new Opcodes(conversion[1].opFunctions[0].dtSignature[0], codeFragmentRht, new Opcode(OpcodeType.Convert, conversion[1].opToken, nOperatorColumn, 0, conversion[1].opFunctions[0]));
                                                }
                                            }
                                            else
                                            {
                                                // A perfect match
                                                dt = function.dtSignature[0];
                                            }
                                            opcodesList.Add(new Opcodes(dt, codeFragmentLft, codeFragmentRht, new Opcode(OpcodeType.Binary, opcode.opToken, nOperatorColumn, opcode.opToken.Length, function)));
                                        }
                                        if (bTrace) Console.WriteLine("Emit:" + opcodesList[opcodesList.Count - 1]);
                                    }
                                    else
                                    {
                                        break;  // Done clearing out binary operators
                                    }
                                } // while ((operatorList.Count > 0) && (operatorList[operatorList.Count - 1].opOperands == 2))
                            } // if (eParseState == ParseState.Opcode)
                            if (strError != "") break; // Propogate error

                            /********************************************************************************
                             * Do argument count and signature checking.
                            ********************************************************************************/
                            if ((operatorThis.opToken == ",") || (operatorThis.opToken == ")") || (operatorThis.opToken == "}"))
                            {
                                bool bArgument = (opcodesList[0].opProgram[0].eOpcodeType == OpcodeType.PendingCall) || (opcodesList[0].opProgram[0].eOpcodeType == OpcodeType.PendingCallM) || (opcodesList[0].opProgram[0].eOpcodeType == OpcodeType.PendingLParen);
                                bArgument &= (opcodesList[opcodesList.Count - 1].opProgram[0].eOpcodeType != OpcodeType.PendingCall);   // zero arguments don't get checked here
                                bArgument &= (opcodesList[opcodesList.Count - 1].opProgram[0].eOpcodeType != OpcodeType.PendingCallM);   // zero arguments don't get checked here
                                if (bArgument)
                                {
                                    arguments++;
                                    {
                                        List<Function> revised = new List<Function>();
                                        foreach (Function function in signature)
                                        {
                                            if ((function.dtSignature.Length - 1) >= arguments)
                                            {
                                                revised.Add(function);
                                            }
                                        }
                                        signature = revised;    // drop signatures that are not long enough
                                    }
                                    if (signature.Count == 0)
                                    {
                                        strError = "Too many arguments";
                                        break;
                                    }
                                }
                                Opcodes codeFragment = opcodesList[opcodesList.Count - 1];
                                {
                                    // Check for perfect match
                                    bool bMatch = false;
                                    {
                                        List<Function> revised = new List<Function>();
                                        foreach (Function function in signature)
                                        {
                                            if (function.dtSignature[arguments] == codeFragment.opDataType)
                                            {
                                                bMatch = true;
                                                revised.Add(function);
                                            }
                                        }
                                        if (bMatch)
                                        {
                                            signature = revised;    // keep only the prefect matches
                                        }
                                    }
                                    if (bMatch == false)
                                    {
                                        // No perfect matches. Look for conversion or DataType.Any
                                        {
                                            List<Function> revised = new List<Function>();
                                            foreach (Function function in signature)
                                            {
                                                if (function.dtSignature[arguments] == DataType.Any)
                                                {
                                                    // Copy signatures that are DataType.Any
                                                    revised.Add(function);
                                                }
                                                else if (bMatch)
                                                {
                                                    // Copy signatures that match the sucessful conversion
                                                    if (function.dtSignature[arguments] == codeFragment.opDataType)
                                                    {
                                                        revised.Add(function);
                                                    }
                                                }
                                                else
                                                {
                                                    // Look for a conversion
                                                    Operator conversion = new Operator();
                                                    Function conversionFunction = LookupFunctionWithConversion(function.dtSignature[arguments], codeFragment.opDataType, ref conversion);
                                                    if (conversionFunction.dtSignature != null)
                                                    {
                                                        // Found a conversion, apply it.
                                                        bMatch = true;
                                                        codeFragment = new Opcodes(conversion.opFunctions[0].dtSignature[0], codeFragment, new Opcode(OpcodeType.Convert, conversion.opToken, nColumn, cColumns, conversion.opFunctions[0]));
                                                        opcodesList[opcodesList.Count - 1] = codeFragment;
                                                        revised.Add(function);
                                                    }
                                                }
                                            }
                                            if (revised.Count == 0)
                                            {
                                                // Not OK. Could not convert
                                                strError = "Data type mismatch; expected ";
                                                List<string> listExpected = new List<string>();
                                                foreach (Function function in signature)
                                                {
                                                    string strExpected = function.dtSignature[arguments].ToString();
                                                    if (listExpected.Contains(strExpected) == false)
                                                    {
                                                        listExpected.Add(strExpected);
                                                    }
                                                    listExpected.Sort();
                                                }
                                                if (listExpected.Count == 1)
                                                {
                                                    strError += listExpected[0];
                                                }
                                                else
                                                {
                                                    for (int nExpected = 0; nExpected < listExpected.Count - 2; nExpected++)
                                                    {
                                                        strError += listExpected[nExpected] + ", ";
                                                    }
                                                    strError += listExpected[listExpected.Count - 2] + " or ";
                                                    strError += listExpected[listExpected.Count - 1];
                                                }
                                                Opcode topData = codeFragment.opProgram[codeFragment.opProgram.Count - 1];
                                                nColumn = topData.nColumn;
                                                cColumns = topData.cColumns;
                                                break;
                                            }
                                            signature = revised;    // keep only the DataType.Any matches
                                        }
                                    } // if (bMatch == false)
                                }
                                if (bArgument)
                                {
                                    {
                                        List<Function> revised = new List<Function>();
                                        foreach (Function function in signature)
                                        {
                                            if ((function.dtSignature.Length - 1) >= arguments)
                                            {
                                                revised.Add(function);
                                            }
                                        }
                                        signature = revised;    // drop signatures that are not long enough
                                    }
                                    if (signature.Count == 0)
                                    {
                                        strError = "Too many arguments";
                                        break;
                                    }
                                }
                            } // if ((operatorThis.opToken == ",") || (operatorThis.opToken == ")") || (operatorThis.opToken == "}"))

                            /********************************************************************************
                             * Pop level on closing parenthesis or brace
                            ********************************************************************************/
                            if ((operatorThis.opToken == ")") || (operatorThis.opToken == "}"))
                            {
                                bool bArgument = (opcodesList[0].opProgram[0].eOpcodeType == OpcodeType.PendingCall) || (opcodesList[0].opProgram[0].eOpcodeType == OpcodeType.PendingCallM) || (opcodesList[0].opProgram[0].eOpcodeType == OpcodeType.PendingLParen);
                                if ((bArgument && (operatorThis.opToken == "}")) || (!bArgument && (operatorThis.opToken == ")")))
                                {
                                    strError = (operatorThis.opToken == "}") ? "Closing curly brace not expected" : "Closing parenthesis not expected";
                                    if (nColumn >= strSource.Length) nColumn = strSource.Length - 1;
                                    break;
                                }
                                // Right parenthesis or right curly brace
                                if (eParseState != ParseState.Opcode)
                                {
                                    if (signature[0].dtSignature.Length == 1)   //++ [0]
                                    {
                                        // OK, an empty argument list is expected
                                    }
                                    else
                                    {
                                        // Not OK. One of two error conditions.
                                        if (nColumn < strSource.Length)
                                        {
                                            strError = (bArgument) ? "Closing parenthesis not expected" : "Closing curly brace not expected";
                                        }
                                        else
                                        {
                                            strError = "Unexpected end of line";
                                        }
                                        break;
                                    }
                                }
                                if (opcodesListStack.Count == 0)
                                {
                                    // Not OK.
                                    strError = "Too many closing parentheses";
                                    nColumn--; // This is detected one column too late
                                    break;
                                }
                                {
                                    // Pop up a level as a result of the closing parenthesis or curly brace
                                    Operator opcode = operatorList[operatorList.Count - 1]; operatorList.RemoveAt(operatorList.Count - 1);
                                    int nOperatorColumn = operatorColumnList[operatorColumnList.Count - 1]; operatorColumnList.RemoveAt(operatorColumnList.Count - 1);
                                    operatorList = operatorListStack.Pop();
                                    operatorColumnList = operatorColumnListStack.Pop();
                                    Opcodes codeFragment = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                    Opcodes codeFunction;
                                    if ((codeFragment.opProgram[0].eOpcodeType != OpcodeType.PendingCall) && (codeFragment.opProgram[0].eOpcodeType != OpcodeType.PendingCallM) && (codeFragment.opProgram[0].eOpcodeType != OpcodeType.PendingArray)) // ???
                                    {
                                        // Non-empty parentheses
                                        codeFunction = opcodesList[opcodesList.Count - 1]; opcodesList.RemoveAt(opcodesList.Count - 1);
                                    }
                                    else
                                    {
                                        // Empty parenthesis
                                        codeFunction = codeFragment;
                                        codeFragment = new Opcodes();
                                    }
                                    opcodesList = opcodesListStack.Pop();
                                    if (bArgument)
                                    {
                                        List<Function> revised = new List<Function>();
                                        foreach (Function function in signature)
                                        {
                                            if ((function.dtSignature.Length - 1) == arguments)
                                            {
                                                revised.Add(function);
                                            }
                                        }
                                        signature = revised;    // drop signatures that are not exactly the right length
                                    }
                                    if (signature.Count == 0)
                                    {
                                        strError = "Too few arguments";
                                        break;
                                    }
                                    // !!! WJC signature.Count > 1 is OOPS ???
                                    if (codeFunction.opProgram[0].strToken == "")   // Should be on eOpcodeType
                                    {
                                        if (codeFunction.opProgram[0].eOpcodeType == OpcodeType.PendingArray)
                                        {
                                            // PushI codeFragment.opProgram.Length after codeFragment
                                            int cElements = (codeFragment.opProgram.Count + 1) / 2;
                                            codeFragment = new Opcodes(DataType.Any, codeFragment, new Opcode(OpcodeType.PushI, cElements.ToString(), -1, -1));
                                            // OpcodeType.Array after codeFragment
                                            // return  DataType from signature[0]
                                            opcodesList.Add(new Opcodes(signature[0].dtSignature[0], codeFragment, new Opcode(OpcodeType.Array, codeFunction.opProgram[0].strToken, codeFunction.opProgram[0].nColumn, codeFunction.opProgram[0].cColumns, signature[0])));
                                        }
                                        else
                                        {
                                            // Normal left parenthesis
                                            opcodesList.Add(codeFragment);
                                        }
                                        if (bTrace) Console.WriteLine("Emit:" + opcodesList[opcodesList.Count - 1]);
                                    }
                                    else
                                    {
                                        // True function call
                                        {
                                            Function function = codeFunction.opProgram[0].function;
                                            if ((function.tTarget != null) && (function.oTarget == null))
                                            {
                                                Opcode opcodeCall = codeFunction.opProgram[0];
                                                codeFunction.opProgram.RemoveAt(0);
                                                opcodeCall.eOpcodeType = OpcodeType.CallM;
                                                opcodeCall.function = signature[0];
                                                codeFragment.opDataType = signature[0].dtSignature[0];
                                                if (codeFragment.opProgram == null)
                                                {
                                                    codeFragment.opProgram = new List<Opcode>();
                                                }
                                                codeFragment.opProgram.AddRange(codeFunction.opProgram);
                                                codeFragment.opProgram.Add(opcodeCall);
                                                opcodesList.Add(codeFragment);
                                            }
                                            else
                                            {
                                                opcodesList.Add(new Opcodes(signature[0].dtSignature[0], codeFragment, new Opcode(OpcodeType.Call, codeFunction.opProgram[0].strToken, codeFunction.opProgram[0].nColumn, codeFunction.opProgram[0].cColumns, signature[0])));
                                            }
                                        }
                                        if (bTrace) Console.WriteLine("Emit:" + opcodesList[opcodesList.Count - 1]);
                                    }
                                    signature = signatureStack.Pop();
                                    arguments = argumentsStack.Pop();
                                }
                                eParseState = ParseState.Opcode;
                                break;
                            } // ")"

                            operatorList.Add(operatorThis); operatorColumnList.Add(nColumn);
                            eParseState = ParseState.Data;
                            break;
                        } // Operator
                    case Tokenizer.TokenClass.Whitespace:
                        {
                            // TODO may matter with unary opcodes
                            break;
                        } // Whitespace
                    case Tokenizer.TokenClass.Empty:
                        {
                            // Check for uncloded parentheses
                            if (opcodesListStack.Count != 0)
                            {
                                strError = "Too few closing parentheses";
                                break;
                            }
                            if (opcodesList.Count != 1)
                            {
                                strError = "Too much data is on the final stack!";
                                break;
                            }
                            // Final convert if necessary
                            if ((dtReturn != DataType.Any) && (dtReturn != opcodesList[0].opDataType))
                            {
                                Operator conversion = new Operator();
                                LookupFunctionWithConversion(dtReturn, opcodesList[0].opDataType, ref conversion);
                                if (conversion.opFunctions == null)
                                {
                                    strError = "Unable to convert to the required type";
                                    break;
                                }
                                opcodesList[0] = new Opcodes(conversion.opFunctions[0].dtSignature[0], opcodesList[0], new Opcode(OpcodeType.Convert, conversion.opToken, nColumn, 0, conversion.opFunctions[0]));
                            }
                            if (dictCalculations.ContainsKey(strCalculationName)) dictCalculations.Remove(strCalculationName);
                            dictCalculations.Add(strCalculationName, new Statement(strSource, opcodesList[0]));
                            if (bTrace) Console.Write(CalculationDisassemble(strCalculationName));
                            break;
                        } // EOS
                    default:
                        {
                            strError = "Unknown token class";
                            break;
                        }
                } // switch (eClass)
                if (strError.Length > 0)
                {
                    strErrorSource = strSource;
                    nErrorColumn = (nColumn <= strSource.Length) ? nColumn : strSource.Length;
                    cErrorColumns = (cColumns > 0) ? cColumns : 1; // End of line
                    break;
                }
            } while (strToken.Length > 0);
            return (strError.Length > 0);
        } // CalculationAdd()
        public bool CalculationAdd(string strCalculationName, Object oReturn, string strSource)
        {
            DataType dtReturn = (oReturn != null) ? TypeGetDataType(oReturn.GetType().FullName) : DataType.Any;
            if (dtReturn == DataType.Error)
            {
                strError = "Unsupported return data type";
            }
            else
            {
                CalculationAdd(strCalculationName, dtReturn, strSource);
            }
            return (strError.Length > 0);
        } // CalculationAdd()

        public string CalculationDisassemble(string strCalculationName)
        {
            string strRet = "";
            if (dictCalculations.ContainsKey(strCalculationName))
            {
                List<Opcode> listOpcode = dictCalculations[strCalculationName].opcodes.opProgram;
                foreach (Opcode opcode in listOpcode)
                {
                    switch (opcode.eOpcodeType)
                    {
                        case OpcodeType.Binary:
                        case OpcodeType.Call:
                        case OpcodeType.Comma:
                        case OpcodeType.Convert:
                        case OpcodeType.PushB:
                        case OpcodeType.PushC:
                        case OpcodeType.PushD:
                        case OpcodeType.PushF:
                        case OpcodeType.PushI:
                        case OpcodeType.PushS:
                        case OpcodeType.Unary:
                            {
                                strRet += "\t" + opcode.eOpcodeType + "\t" + ((opcode.eOpcodeType == OpcodeType.PushF) ? "[" : "") + opcode.strToken + ((opcode.eOpcodeType == OpcodeType.PushF) ? "]" : "");
                                if (opcode.function.strMethod != null)
                                {
                                    strRet += "\t" + opcode.function.strType + "::" + opcode.function.strMethod;
                                }
                                strRet += "\n";
                                break;
                            }
                        default:
                            {
                                strRet += "\t" + opcode.eOpcodeType + "\t" + opcode.strToken + "\n";
                                break;
                            }
                    } // switch (opcode.eOpcodeType)
                } // foreach (Opcode opcode in listOpcode)
                strRet += "\t" + "RET" + "\t//" + dictCalculations[strCalculationName].opcodes.opDataType.ToString() + "\n";
            } // if (dictCalculations.ContainsKey(strCalculationName))
            return strRet;
        } // CalculationDisassemble()

        // Accummulate count of field usage
        private void StatementIncrementFieldCounts(Statement statement, ref Dictionary<string, int> rdict)
        {
            List<Opcode> listOpcode = statement.opcodes.opProgram;
            foreach (Opcode opcode in listOpcode)
            {
                if (opcode.eOpcodeType == OpcodeType.PushF)
                {
                    string strField = opcode.strToken;
                    if (rdict.ContainsKey(strField) == false) rdict.Add(strField, 0);
                    rdict[strField]++;
                }
            }
            return;
        } // StatementIncrementFieldCounts()

        // Count of field usage for one calculation
        public Dictionary<string, int> CalculationGetFieldCounts(string strCalculationName)
        {
            Dictionary<string, int> dictRet = new Dictionary<string, int>();
            if (dictCalculations.ContainsKey(strCalculationName))
            {
                StatementIncrementFieldCounts(dictCalculations[strCalculationName], ref dictRet);
            }
            return dictRet;
        } // CalculationGetFieldCounts()
        public void CalculationIncrementFieldCounts(string strCalculationName, ref Dictionary<string, int> rdict)
        {
            if (dictCalculations.ContainsKey(strCalculationName))
            {
                StatementIncrementFieldCounts(dictCalculations[strCalculationName], ref rdict);
            }
            return;
        } // CalculationIncrementFieldCounts()

        // Count of field usage for all calculations
        public Dictionary<string, int> GetFieldCounts()
        {
            Dictionary<string, int> dictRet = new Dictionary<string, int>();
            foreach (KeyValuePair<string, Statement> kvp in dictCalculations)
            {
                StatementIncrementFieldCounts(kvp.Value, ref dictRet);
            }
            return dictRet;
        } // GetFieldCounts()

        public bool CalculationRemove(string strCalculationName)
        {
            bool bRet = dictCalculations.ContainsKey(strCalculationName) == false;  // true on error
            if (bRet == false)
            {
                dictCalculations.Remove(strCalculationName);
            }
            return bRet;
        } // CalculationRemove()

        public string CalculationUnparse(string strCalculationName, bool bWhitespace, bool bShowConvert, string strConvertLft, string strConvertRht)
        {
            string strRet = "";
            string strWhitespace = (bWhitespace) ? " " : "";
            if (dictCalculations.ContainsKey(strCalculationName))
            {
                Stack<string> strExpression = new Stack<string>();
                Stack<int> nPrecedence = new Stack<int>();
                List<Opcode> listOpcode = dictCalculations[strCalculationName].opcodes.opProgram;
                foreach (Opcode opcode in listOpcode)
                {
                    switch (opcode.eOpcodeType)
                    {
                        case OpcodeType.PushB:
                        case OpcodeType.PushC:
                        case OpcodeType.PushD:
                        case OpcodeType.PushF:
                        case OpcodeType.PushI:
                        case OpcodeType.PushS:
                            {
                                strExpression.Push(((opcode.eOpcodeType == OpcodeType.PushF) ? "[" : "") + opcode.strToken + ((opcode.eOpcodeType == OpcodeType.PushF) ? "]" : ""));
                                nPrecedence.Push(15);
                                break;
                            }
                        case OpcodeType.Unary:
                            {
                                int nop = dictOperators["U" + opcode.strToken].opPredence;
                                string strE1 = strExpression.Pop();
                                int n1 = nPrecedence.Pop();
                                string strE = opcode.strToken + strE1;
                                strExpression.Push(strE);
                                nPrecedence.Push(nop);
                                break;
                            }
                        case OpcodeType.Binary:
                            {
                                int nop = dictOperators[opcode.strToken].opPredence;
                                string strE2 = strExpression.Pop();
                                {
                                    int n2 = nPrecedence.Pop();
                                    if (n2 < nop)
                                    {
                                        strE2 = "(" + strE2 + ")";
                                    }
                                }
                                string strE1 = strExpression.Pop();
                                {
                                    int n1 = nPrecedence.Pop();
                                    if (n1 < nop)
                                    {
                                        strE1 = "(" + strE1 + ")";
                                    }
                                }
                                string strE = strE1 + strWhitespace + opcode.strToken + strWhitespace + strE2;
                                strExpression.Push(strE);
                                nPrecedence.Push(nop);
                                break;
                            }
                        case OpcodeType.Comma:
                            {
                                string strE1 = strExpression.Pop();
                                nPrecedence.Pop();
                                string strE = strE1 + "," + strWhitespace;
                                strExpression.Push(strE);
                                nPrecedence.Push(15);
                                break;
                            }
                        case OpcodeType.Call:
                            {
                                string strE1;
                                if (opcode.function.dtSignature.Length > 1)
                                {
                                    strE1 = strExpression.Pop();
                                    nPrecedence.Pop();
                                    if (opcode.function.dtSignature.Length > 2)
                                    {
                                        string strE2 = strExpression.Pop();
                                        nPrecedence.Pop();
                                        strE1 = strE2 + strE1;
                                    }
                                }
                                else
                                {
                                    strE1 = "";
                                }
                                string strE = opcode.strToken + "(" + strE1 + ")";
                                strExpression.Push(strE);
                                nPrecedence.Push(15);
                                break;
                            }
                        case OpcodeType.CallM:
                            {
                                string strT;
                                strT = strExpression.Pop();
                                nPrecedence.Pop();
                                string strE1;
                                if (opcode.function.dtSignature.Length > 1)
                                {
                                    strE1 = strExpression.Pop();
                                    nPrecedence.Pop();
                                    if (opcode.function.dtSignature.Length > 2)
                                    {
                                        string strE2 = strExpression.Pop();
                                        nPrecedence.Pop();
                                        strE1 = strE2 + strE1;
                                    }
                                }
                                else
                                {
                                    strE1 = "";
                                }
                                string strE = strT + "." + opcode.strToken + "(" + strE1 + ")";
                                strExpression.Push(strE);
                                nPrecedence.Push(15);
                                break;
                            }
                        case OpcodeType.Array:
                            {
                                string strE1 = "";
                                int cElements = Convert.ToInt32(strExpression.Pop());
                                while (cElements-- > 0)
                                {
                                    strE1 = strExpression.Pop() + strE1;
                                }
                                string strE = opcode.strToken + "{" + strE1 + "}";
                                strExpression.Push(strE);
                                nPrecedence.Push(15);
                                break;
                            }
                        case OpcodeType.Convert:
                            {
                                if (bShowConvert)
                                {
                                    string strE1 = strExpression.Pop();
                                    nPrecedence.Pop();
                                    string strE = strConvertLft + opcode.strToken + "(" + strConvertRht + strE1 + strConvertLft + ")" + strConvertRht;
                                    strExpression.Push(strE);
                                    nPrecedence.Push(15);
                                }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    } // switch (opcode.eOpcodeType)
                } // foreach (Opcode opcode in listOpcode)
                strRet = strExpression.Pop();
            } // if ((nCalc >= 0) && (nCalc < listCalculations.Count))
            return strRet;
        } // CalculationUnparse()
        public string CalculationUnparse(string strCalculationName)
        {
            return CalculationUnparse(strCalculationName, true, true, "", "");
        } // CalculationUnparse()


        /*================================================================================
         * DataType
         ================================================================================*/

        private static DataType TokenClassGetDataType(Tokenizer.TokenClass eClass)
        {
            DataType dtRet = DataType.Error;
            switch (eClass)
            {
                case Tokenizer.TokenClass.Char:
                    dtRet = DataType.Char;
                    break;
                case Tokenizer.TokenClass.Decimal:
                    dtRet = DataType.Decimal;
                    break;
                case Tokenizer.TokenClass.Integer:
                    dtRet = DataType.Integer;
                    break;
                case Tokenizer.TokenClass.String:
                    dtRet = DataType.String;
                    break;
                default:
                    dtRet = DataType.Error;
                    break;
            }
            return dtRet;
        } // TokenClassGetDataType()

        private Function LookupFunction(List<Function> vFunctions, DataType[] vOperands)
        {
            Function functionRet = new Function();
            foreach (Function aFunction in vFunctions)
            {
                DataType[] aSignature = aFunction.dtSignature;
                if (aSignature.Length - 1 != vOperands.Length) continue;
                int nOperand;
                for (nOperand = 0; nOperand < vOperands.Length; nOperand++)
                {
                    if (aSignature[1 + nOperand] == DataType.Any)
                    {
                        continue;
                    }
                    if (aSignature[1 + nOperand] != vOperands[nOperand])
                    {
                        break;
                    }
                }
                if (nOperand < vOperands.Length) continue;
                functionRet = aFunction;
                break;
            } // foreach (DataType[] aSignature in vSignatures)
            return functionRet;
        } // LookupFunction()

        /*================================================================================
         * Operator
         ================================================================================*/
        private struct Operator
        {
            public string opToken;
            public int opPredence;
            public bool opLeftAssociative;
            public int opOperands;
            public List<Function> opFunctions;
            public Operator(string strToken, int nPrecedence, bool bLeftAssociative, int nOperands, List<Function> listFunctions)
            {
                opToken = strToken;
                opPredence = nPrecedence;
                opLeftAssociative = bLeftAssociative;
                opOperands = nOperands;
                opFunctions = listFunctions;
                return;
            }
        } // struct Operator

        private Dictionary<string, Operator> dictOperators = new Dictionary<string, Operator>();
        public bool ObjectAddOperators(Object vObject)
        {
            strError = "";
            Type tObject = vObject.GetType();
            MemberInfo[] mis = tObject.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, null, null);
            foreach (MemberInfo mi in mis)
            {
                string strFunctionName = "";
                DataType[] dtArguments = new DataType[0];
                if (DeclarationDecode(mi.ToString(), ref strFunctionName, ref dtArguments))
                {
                    break;
                }
                // dictOperators.Add ...
                OperatorInfo oi = (OperatorInfo)System.Attribute.GetCustomAttribute(mi, typeof(OperatorInfo));
                if (oi == null)
                {
                    continue;   // Silent failure ???
                }
                string strToken = oi.strToken;
                int nPrecedence = oi.nPrecedence;
                bool bLeftAssociative = oi.bLeftAssociative;
                if (dictOperators.ContainsKey(strToken) == false)
                {
                    dictOperators.Add(strToken, new Operator(strToken, nPrecedence, bLeftAssociative, dtArguments.Length - 1, new List<Function>()));
                }
                dictOperators[strToken].opFunctions.Add(new Function(tObject.FullName, strFunctionName, dtArguments));
            } // foreach (MemberInfo mi in mis)
            return (strError.Length > 0);
        } // ObjectAddOperators()

        /*================================================================================
         * Field
         ================================================================================*/
        private struct Field
        {
            public string ColumnName;
            public DataType dtColumn;
            public string TableName;
            public string DataSetName;
            public Field(string vColumnName, DataType vdtColumn, string vTableName, string vDataSetName)
            {
                ColumnName = vColumnName;
                dtColumn = vdtColumn;
                TableName = vTableName;
                DataSetName = vDataSetName;
                return;
            }
        } // struct Field

        private Dictionary<string, Field> dictFields = new Dictionary<string, Field>();
        public string Fields
        {
            get
            {
                string xmlRet = "";
                xmlRet += "<fields>" + "\n";
                foreach (KeyValuePair<string, Field> kvp in dictFields)
                {
                    Field oField = kvp.Value;
                    xmlRet += " " + "<field" + "\n";
                    xmlRet += " name='" + oField.ColumnName + "'";
                    xmlRet += " type='" + oField.dtColumn.ToString() + "'";
                    xmlRet += " table='" + oField.TableName + "'";
                    xmlRet += " dataset='" + oField.DataSetName + "'";
                    ;
                    xmlRet += "/>" + "\n";
                }
                xmlRet += "</fields>" + "\n";
                return xmlRet;
            }
        } // Fields
        public bool DataViewManagerAddFields(DataViewManager vDataViewManager)
        {
            strError = "";
            string strDataSetName = vDataViewManager.DataSet.DataSetName;
            foreach (DataTable oDataTable in vDataViewManager.DataSet.Tables)
            {
                string strTableName = oDataTable.TableName;
                foreach (DataColumn oDataColumn in oDataTable.Columns)
                {
                    string strCaption = oDataColumn.Caption;
                    string strDataType = oDataColumn.DataType.ToString();
                    DataType dtDataType = TypeGetDataType(strDataType);
                    if (dtDataType == DataType.Error)
                    {
                        continue;   // ? ignore fields we cannot reach
                    }
                    dictFields.Add(strCaption.ToUpper(), new Field(strCaption, dtDataType, strTableName, strDataSetName));
                } // foreach (DataColumn oDataColumn in oDataTable.Columns)
            } // foreach (DataTable oDataTable in vDataViewManager.DataSet.Tables)
            return (strError.Length > 0);
        } // DataViewManagerAddFields()

        /*================================================================================
         * Function
         ================================================================================*/

#if OBSOLETE
        private Dictionary<string, Function> dictConversions = new Dictionary<string, Function>();
#endif
        private Dictionary<string, List<Function>> dictConversions = new Dictionary<string, List<Function>>();
        private bool DeclarationDecode(string vstrDeclaration, ref string rstrFunctionName, ref DataType[] rdtArguments)
        {
            strError = "";
            do
            {
                string strReturnType;
                string[] strArguments;
                {
                    string[] strSplit;
                    strSplit = vstrDeclaration.Split(new char[] { ' ' }, 2);
                    strReturnType = strSplit[0];
                    strSplit = strSplit[1].Split(new char[] { '(' }, 2);
                    rstrFunctionName = strSplit[0];
                    strSplit[1] = strSplit[1].Substring(0, strSplit[1].Length - 1);
                    if (strSplit[1].Length > 0)
                    {
                        strArguments = strSplit[1].Split(',');
                    }
                    else
                    {
                        strArguments = new string[0];
                    }
                }
                rdtArguments = new DataType[1 + strArguments.Length];
                rdtArguments[0] = TypeGetDataType(strReturnType);
                if (rdtArguments[0] == DataType.Error)
                {
                    strError = "Return type not supported";
                    break;
                }
                for (int nArgument = 0; nArgument < strArguments.Length; nArgument++)
                {
                    strArguments[nArgument] = strArguments[nArgument].Trim();
                    rdtArguments[1 + nArgument] = TypeGetDataType(strArguments[nArgument]);
                    if (rdtArguments[1 + nArgument] == DataType.Error)
                    {
                        strError = "Argument type not supported";
                        break;
                    }
                }
            } while (false);
            return (strError.Length > 0);
        } // DeclarationDecode()

        private bool ObjectAddConversions(Object vObject)
        {
            strError = "";
            Type tObject = vObject.GetType();
            MemberInfo[] mis = tObject.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, null, null);
            foreach (MemberInfo mi in mis)
            {
                string strFunctionName = "";
                DataType[] dtArguments = new DataType[0];
                if (DeclarationDecode(mi.ToString(), ref strFunctionName, ref dtArguments))
                {
                    break;
                }
#if OBSOLETE
                dictConversions.Add(strFunctionName, new Function(tObject.FullName, strFunctionName, dtArguments));
#endif
                if (dictConversions.ContainsKey(strFunctionName) == false)
                {
                    dictConversions.Add(strFunctionName, new List<Function>());
                }
                dictConversions[strFunctionName].Add(new Function(tObject.AssemblyQualifiedName, strFunctionName, dtArguments));
            } // foreach (MemberInfo mi in mis)
            return (strError.Length > 0);
        } // ObjectAddConversions()

        private Dictionary<string, List<Function>> dictFunctions = new Dictionary<string, List<Function>>();
        public string Functions
        {
            get
            {
                string xmlRet = "";
                xmlRet += "<functions>" + "\n";
                foreach (KeyValuePair<string, List<Function>> kvp in dictFunctions)
                {
                    foreach (Function function in kvp.Value)
                    {
                        xmlRet += " " + "<function";
                        xmlRet += " method='" + function.strMethod + "'";
                        xmlRet += " returns='" + function.dtSignature[0].ToString() + "'";
                        if (function.tTarget != null) {
                            xmlRet += " target='" + function.tTarget.ToString() + "'";
                        }
                        xmlRet += " type='" + function.strType + "'";
                        xmlRet += ">" + "\n";
                        xmlRet += "  " + "<arguments";
                        xmlRet += " count='" + (function.dtSignature.Length - 1) + "'";
                        if (function.dtSignature.Length > 1)
                        {
                            xmlRet += ">" + "\n";
                            for (int nArg = 1; nArg < function.dtSignature.Length; nArg++)
                            {
                                xmlRet += "   " + "<argument>" + "\n";
                                xmlRet += "    " + function.dtSignature[nArg].ToString() + "\n";
                                xmlRet += "   " + "</argument>" + "\n";
                            }
                            xmlRet += "  " + "</arguments>" + "\n";
                        }
                        else
                        {
                            xmlRet += "/>" + "\n";
                        }
                        xmlRet += " " + "</function>" + "\n";
                    } // foreach (Function function in kvp.Value)
                } // foreach (KeyValuePair<string, List<Function>> kvp in dictFunctions)
                xmlRet += "</functions>" + "\n";
                return xmlRet;
            } // get
        } // Functions
        public bool ObjectAddFunctions(Object vObject)
        {
            strError = "";
            Type tObject = vObject.GetType();
            return TargetTypeAddFunctions(vObject, tObject);
        } // ObjectAddFunctions()

        private bool TargetTypeAddFunctions(Object voTarget, Type vtTarget)
        {
            MethodInfo[] mis = vtTarget.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo mi in mis)
            {
                string strFunctionName = "";
                DataType[] dtArguments = new DataType[0];
                if (DeclarationDecode(mi.ToString(), ref strFunctionName, ref dtArguments))
                {
                    continue;
                }
                if (dictFunctions.ContainsKey(strFunctionName.ToUpper()) == false)
                {
                    dictFunctions.Add(strFunctionName.ToUpper(), new List<Function>());
                }
                // Should probably check for duplicate functions before adding
                if (mi.IsStatic)
                {
                    dictFunctions[strFunctionName.ToUpper()].Add(new Function(vtTarget.AssemblyQualifiedName, strFunctionName, dtArguments));
                }
                else
                {
                    if (voTarget == null)
                    {
                        DataType dt = TypeGetDataType(vtTarget.ToString());
                        if (dt == DataType.Error)
                        {
                            continue;   // Requires an unsupported data type
                        }
                    }
                    dictFunctions[strFunctionName.ToUpper()].Add(new Function(vtTarget.AssemblyQualifiedName, strFunctionName, voTarget, vtTarget, dtArguments));
                }
            }
            return (strError.Length > 0);
        } // TypeAddFunctions(

        public bool TypeAddFunctions(Type vType)
        {
            strError = "";
            return TargetTypeAddFunctions(null, vType);
        } // TypeAddFunctions()

        private Function LookupFunctionWithConversion(DataType vDesired, DataType vHave, ref Operator vOperator)
        {
            Function functionRet = new Function();
            vOperator = new Operator();
            foreach (KeyValuePair<string, List<Function>> kvp in dictConversions)
            {
                foreach (Function function in kvp.Value)
                {
                    if (
                        (function.dtSignature[0] == vDesired) &&
                        (function.dtSignature[1] == vHave)
                    )
                    {
                        // "Fake" Operator
                        vOperator = new Operator(kvp.Key, 0, true, 1, new List<Function>(new Function[] { new Function(function.strType, function.strMethod, function.dtSignature) }));
                        functionRet = function;
                        break;
                    }
                }
                if (functionRet.strMethod != null)
                {
                    break;
                }
            }
            return functionRet;
        } // LookupFunctionWithConversion()

        private Function LookupFunctionWithConversion(List<Function> vFunctions, DataType[] vOperands, ref Operator[] vOperators)
        {
            Function functionRet = new Function();
            vOperators = new Operator[vOperands.Length];
            foreach (Function aFunction in vFunctions)
            {
                DataType[] aSignature = aFunction.dtSignature;
                if (aSignature.Length - 1 != vOperands.Length) continue;
                int nOperand;
                for (nOperand = 0; nOperand < vOperands.Length; nOperand++)
                {
                    if (aSignature[1 + nOperand] != vOperands[nOperand])
                    {
                        vOperators[nOperand] = new Operator();
                        Function testFunction = LookupFunctionWithConversion(aSignature[1 + nOperand], vOperands[nOperand], ref vOperators[nOperand]);
                        if (vOperators[nOperand].opFunctions == null)
                        {
                            break;
                        }
                    }
                }
                if (nOperand < vOperands.Length) continue;
                functionRet = aFunction;
                break;
            } // foreach (DataType[] aSignature in vSignatures)
            return functionRet;
        } // LookupFunctionWithConversion()

    } // class CalculationParser
}
