using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
namespace PreCheck_Installation
{
    class Program
    {
         static string _txtFilePath=string.Empty;
         static string _xmlFilePath = string.Empty;

        static void Main(string[] args)
           {
               string userName = string.Empty;
               string password = string.Empty;
               string serviceName = string.Empty;
               string dbServerName = string.Empty;
               string cSFlag = string.Empty;
            Console.SetWindowSize(100, 50);
            Console.WriteLine("Enter the target Oracle service name:");
            serviceName = Console.ReadLine();
            if (serviceName == "")
            {
                serviceName="orcl";
            }
            Console.WriteLine("Enter the name of an Oracle account with system privileges <system>:");
            userName = Console.ReadLine();
            if (userName == "")
            {
                userName = "system";
            }
            Console.WriteLine("Enter the above Oracle account password <manager2> :");
            password = Console.ReadLine();
            if (password == "")
            {
                password = "manager2";
            }
            Console.WriteLine("Do you want to prechek CBOE Application Dependencies <Y/N> :");
            cSFlag  = Console.ReadLine().ToUpper();
            if (cSFlag == "Y")
            {
                Console.WriteLine("Enter the Database Server to connect <LocalHost> :");
                dbServerName = Console.ReadLine();
                if (dbServerName == "")
                {
                    dbServerName = "LocalHost";
                }
            
            }
             
            object objHW = Activator.CreateInstance(typeof(PreCheckHardware));
            Console.WriteLine("PreCheck for hardware process completed.");
            Console.WriteLine("\n");
            object objOS = Activator.CreateInstance(typeof(PreCheckOS));
            Console.WriteLine("PreCheck for Operating System process completed.");
            Console.WriteLine("\n");
            object objIISVersion = Activator.CreateInstance(typeof(PreCheckIIS));
            Console.WriteLine("PreCheck for Internet Information System process completed.");
            Console.WriteLine("\n");
            object objDotNetFWVersion = Activator.CreateInstance(typeof(PreCheckNetFW));
            Console.WriteLine("PreCheck for .Net Framework process completed.");
            Console.WriteLine("\n");
            Type oracleClientType=typeof(PreCheckOracleClient);
            object objOracleClient = Activator.CreateInstance(oracleClientType, new object[] { userName, password, serviceName});
            Console.WriteLine("PreCheck for Oracle Client process completed.");
            Console.WriteLine("\n");
            object objMDACVersion = Activator.CreateInstance(typeof(PreCheckMDAC));
            Console.WriteLine("PreCheck for MDAC process completed.");
            Console.WriteLine("\n");
            object objMSOfficeVersion = Activator.CreateInstance(typeof(PreCheckMSOffice));
            Console.WriteLine("PreCheck for MS Office process completed.");
            Console.WriteLine("\n");
            object objCBOEAppnDep =null;
            if (cSFlag == "Y")
            {
                Type cBOEDependenciesType = typeof(PreCheckCBOEDependency);
                 objCBOEAppnDep = Activator.CreateInstance(cBOEDependenciesType, new object[] { userName, password, serviceName, dbServerName });
                Console.WriteLine("PreCheck for CBOE Application Dependencies process completed.");
                Console.WriteLine("\n");
            }
            List<Object> objlst = new List<Object>();
            // adding the objects to objects list 
            objlst.Add(objHW);
            objlst.Add(objOS);
            objlst.Add(objOracleClient);
            objlst.Add(objDotNetFWVersion);
            objlst.Add(objIISVersion);
            objlst.Add(objMDACVersion);
            objlst.Add(objMSOfficeVersion);
            if (objCBOEAppnDep != null)
            {
                objlst.Add(objCBOEAppnDep);
            }
            GenenerateOutPuttxt objtxt = new GenenerateOutPuttxt();
            objtxt.ObjectList = objlst;
            string doc = objtxt.GetPreCheckXMLDoc;
            //Console.WriteLine(doc);
            GenerateTXTFile(doc);
            GenerateXMLFile(objlst);
            //GenerateExcelFile(objlst);
            Console.WriteLine("Path of Log file(.txt):"+_txtFilePath);
            Console.WriteLine("\n");
            Console.WriteLine("Path of Log file(.XML):"+_xmlFilePath );
            Console.CursorTop = 0;    
            Console.Read();
          
                    
        }

        static void GenerateTXTFile(string OutPut)
        {
            try
            {
                OutPut = OutPut.Replace("\n", Environment.NewLine);
                _txtFilePath  = System.Reflection.Assembly.GetExecutingAssembly().Location;
                _txtFilePath = _txtFilePath.Substring(0, _txtFilePath.Length - 25);
                System.IO.FileStream wFile;
                byte[] byteData = null;
                byteData = Encoding.ASCII.GetBytes(OutPut);
                _txtFilePath = _txtFilePath + "PreChecker.txt";
                wFile = new FileStream(_txtFilePath, FileMode.Append);
                wFile.Write(byteData, 0, byteData.Length);
                wFile.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        static void GenerateXMLFile(List<object> ObjList)
        {
            try
            {
                _xmlFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                _xmlFilePath = _xmlFilePath.Substring(0, _xmlFilePath.Length - 25);
                _xmlFilePath=_xmlFilePath + "PreChecker.xml";
                GenerateXML objXML = new GenerateXML();
                objXML.ObjectList = ObjList;
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc = objXML.GetPreCheckXMLDoc;
                XMLDoc.Save(_xmlFilePath);
                
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

         

    }
}

