using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.ReadKey();
//            TestLocal();
//            TestLocal2();
            TestWebService();
//            TestWebService2();
        }

        public static void TestWebService()
        {
            string code = "from ChemScript import *\nm=Mol.loadData('CCCC', 'smiles')\nr=m.formula()\nw=m.weight()";

            PyEngine.Service pe = new Test.PyEngine.Service();

            object ret;
            string error;
            bool f = pe.Execute(code, "r", out ret, out error);
            if (f)
            {
            }
        }

        public static void TestWebService2()
        {
            string code = "from ChemScript import *\nm=Mol.loadData(cdx, 'cdx')\nr=m.formula()\nw=m.weight()";

            PyEngine.Service pe = new Test.PyEngine.Service();

            string[] inputvars = new string[] { "cdx" };
            string[] inputs = new string[] { @"VmpDRDAxMDAEAwIBAAAAAAAAAAAAAACAAQAAAAMAOwAAAENTIENEWCBkcml2ZXIgMi4wIFtm
b3IgQ2hlbURyYXcgOF0gLyBOb3YgMDYgMTM6MzI6MDQgMjAwNgQCEABbZScAW2UnAAkYOwDi
v40AAQkIAAAAAAAAAAAAAgkIABpIsQDz7wMBBQgEAAAAHgAAAzIACAD///////8AAAAAAAD/
/wAAAAAAAAAA////////AAAAAP//AAAAAP///////wAA//8BgAIAAAAEAhAAW2UnAFtlJwAJ
GDsA4r+NAAOAAwAAAASABAAAAAACCABbZScAW2UnADsEAQAAPAQBAAA5BAMAAAAwAAAEgAUA
AAAAAggACRg7AIiDSQA7BAEAADwEAQAAOQQDAAAAMQAABIAGAAAAAAIIAFtlJwC1oWsAOwQB
AAA8BAEAADkEAwAAADIAAASABwAAAAACCAAJGDsA4r+NADsEAQAAPAQBAAA5BAMAAAAzAAAF
gAgAAAAEBgQABAAAAAUGBAAFAAAAAAAFgAkAAAAEBgQABQAAAAUGBAAGAAAAAAAFgAoAAAAE
BgQABgAAAAUGBAAHAAAAAAAAAAAAAAAAAA==" };
            string[] outputvars = new string[]{"r", "w", "m"};

            string outputs;
            string errors;
            int n = pe.SingleExecute(code, inputvars, inputs, outputvars, out outputs, out errors);
            if (n > 0)
            {
            }
        }

        public static void TestLocal()
        {
            string code = "from ChemScript import *\nm=Mol.loadData('CCCC')\nr=m.formula()\nw=m.weight()";

            object ret;
            string error;

            T.Service s = new T.Service();
            bool f = s.Execute(code, "m", out ret, out error);
            if (!f)
            {
                Console.WriteLine(error);
            }
        }

        public static void TestLocal2()
        {
            string code = "from ChemScript import *\nm=Mol.loadData(smiles)\nr=m.formula()\nw=m.weight()";

            T.Service s = new T.Service();

            string[] inputvars = new string[] { "smiles" };
            string[] inputs = new string[] { "CCCCC", "????", "CC=O" };
            string[] outputvars = new string[] { "r", "w", "m" };

            object[] outputs;
            string[] errors;
            int n = s.BatchExecute(code, inputvars, inputs, outputvars, out outputs, out errors);
            if (n > 0)
            {
            }
        }
    }
}
