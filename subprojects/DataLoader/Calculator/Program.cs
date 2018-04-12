using System;
using System.Data;
#if SEPARATE
using CambridgeSoft.COE.DataLoader.Calculation.Execution;
#endif
using CambridgeSoft.COE.DataLoader.Calculation.Parser;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            CalculationParser oParser = new CalculationParser();
            oParser.TypeAddFunctions(Type.GetType("System.Boolean"));
            oParser.TypeAddFunctions(Type.GetType("System.Char"));
            oParser.TypeAddFunctions(Type.GetType("System.Convert"));
            oParser.TypeAddFunctions(Type.GetType("System.DateTime"));
            //oParser.TypeAddFunctions(Type.GetType("System.DBNull"));
            oParser.TypeAddFunctions(Type.GetType("System.Double"));
            //oParser.TypeAddFunctions(Type.GetType("System.Environment"));
            oParser.TypeAddFunctions(Type.GetType("System.Int32"));
            oParser.TypeAddFunctions(Type.GetType("System.Math"));
            oParser.TypeAddFunctions(Type.GetType("System.String"));
            Object oFoo = new MyFunctions();
            oParser.ObjectAddFunctions(oFoo);
            {
                string strFunctions = oParser.Functions;
                Console.WriteLine(strFunctions);
            }

#if SEPARATE
            CalculationExecution oExecution = new CalculationExecution();
#endif

            DataViewManager dvm = new MyData().MyDataViewManager();
            oParser.DataViewManagerAddFields(dvm);
#if SEPARATE
            oExecution.DataRowBind(dvm.DataSet.Tables[0].Rows[0], dvm.DataSet.Tables[0]);
#else
            oParser.DataRowBind(dvm.DataSet.Tables[0].Rows[0], dvm.DataSet.Tables[0]);
#endif
            string strCalculationName = "0";
            for (string strSource = Console.ReadLine(); strSource.Length > 0; strSource = Console.ReadLine())
            {
                strCalculationName = (Convert.ToInt32(strCalculationName) + 1).ToString();
                bool bFailed = oParser.CalculationAdd(strCalculationName, "", strSource);
                if (bFailed == false)
                {
                    string strDisassembly = oParser.CalculationDisassemble(strCalculationName);
                    Console.Write(strDisassembly);
                    string strUnparse = oParser.CalculationUnparse(strCalculationName);
                    Console.WriteLine(strUnparse);
#if SEPARATE
                    string xml = oParser.GetCalculation(strCalculationName);
                    oExecution.CalculationSet(xml);
                    oParser.CalculationSet(xml);
                    Object oResult = oExecution.CalculationExecute(strCalculationName);
#else
                    Object oResult = oParser.CalculationExecute(strCalculationName);
#endif
                    if (oResult != null)
                    {
                        Console.WriteLine(oResult.ToString());
                    }
                    else
                    {
                        Console.WriteLine(strSource);
#if SEPARATE
                        Exception ex = oExecution.CalculationException;
                        Console.WriteLine("".PadRight(oExecution.ErrorColumn) + "".PadRight(oExecution.ErrorColumns, '\xB0'));
#else
                        Exception ex = oParser.CalculationException;
                        Console.WriteLine("".PadRight(oParser.ErrorColumn) + "".PadRight(oParser.ErrorColumns, '\xB0'));
#endif
                        Console.WriteLine(ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine(ex.InnerException.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("".PadRight(oParser.ErrorColumn) + "".PadRight(oParser.ErrorColumns, '\xB0'));
                    Console.WriteLine(oParser.Error);
                }
            }
            return;
        }
    }
}
