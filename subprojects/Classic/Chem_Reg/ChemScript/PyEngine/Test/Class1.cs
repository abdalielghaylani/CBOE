using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.ChemFlow;

namespace T
{
    public class Service
    {
        public Service()
        {
            //PythonProxy.Engine.Logging.Disabled = (ConfigurationManager.AppSettings["log"] + "").ToLower() != "on";
            //PythonProxy.Engine.Logging.Folder = ConfigurationManager.AppSettings["logfolder"];
            //PythonProxy.Engine.Logging.ExpireDays = ToInt(ConfigurationManager.AppSettings["logdays"]);
            //PythonProxy.Engine.Logging.Debug = (ConfigurationManager.AppSettings["debug"] + "").ToLower() == "on";
            PythonProxy.Engine.SetPythonVerion("2.5");
        }

        int ToInt(string s)
        {
            int n = 0;
            try
            {
                n = Convert.ToInt32(s);
            }
            catch
            {
                return 0;
            }

            return n;
        }

        //[WebMethod]
        public string Version()
        {
            return "CambridgeSoft Python WebService 1.1";
        }

        //[WebMethod]
        public bool Execute(string code, string returnName, out object retval, out string error)
        {
            retval = "";
            error = "";

            object[] output;
            string[] errors;
            int n = BatchExecute(code, null, null, new string[] { returnName }, out output, out errors);
            if (n == 1)
                retval = output[0];
            else
                error = errors != null ? errors[0] : null;

            return n == 1;
        }

        //[WebMethod]
        public int BatchExecute(string code, string[] inputVars, string[] inputs, string[] outputVars,
            out object[] outputs, out string[] errors)
        {
            outputs = null;
            errors = null;
            if (!VerifyInputs(inputVars, inputs))
            {
                PythonProxy.Engine.Logging.Add(null, Log.Level.Error, "Invalid input vars");
                return 0;
            }

            PythonProxy.Engine engine = null;
            engine = new PythonProxy.Engine(null);

            if (engine == null)
            {
                PythonProxy.Engine.Logging.Add(null, Log.Level.Crash, "Cannot initialize Python!");
                return 0;
            }

            int n = 0;
            if (inputVars == null || inputs.Length == 0)
            {
                outputs = new object[outputVars.Length];
                errors = new string[1];

                if (Execute(engine, code, out errors[0]))
                {
                    ++n;
                    GetOutput(engine, outputVars, outputs, 0);
                }
                else
                {
                    if (errors[0] == null)
                        errors[0] = "Unknown error!";
                }
            }
            else
            {
                outputs = new object[inputs.Length * outputVars.Length];
                errors = new string[inputs.Length];

                for (int i = 0; i < inputs.Length; ++i)
                {
                    if (!SetInput(engine, inputVars, inputs, i))
                        return 0;

                    if (Execute(engine, code, out errors[i]))
                    {
                        ++n;
                        GetOutput(engine, outputVars, outputs, i);
                    }
                    else
                    {
                        if (errors[i] == null)
                            errors[i] = "Unknown error!";
                    }
                }
            }

            return n;
        }

        //[WebMethod]
        public int SingleExecute(string code, string[] inputVars, string[] inputs, string[] outputVars,
            out string outputs, out string errors)
        {
            const string kSeprator = "|||||";
            outputs = errors = null;

            object[] outs;
            string[] errs;

            int n = BatchExecute(code, inputVars, inputs, outputVars, out outs, out errs);
            if (n > 0)
            {
                foreach (object s in outs)
                {
                    if (outputs != null)
                        outputs += kSeprator;
                    outputs += s;
                }

                foreach (string s in errs)
                {
                    if (errors != null)
                        errors += kSeprator;
                    errors += s;
                }
            }
            else
            {
                if (errs != null && errs.Length > 0)
                {
                    foreach (string s in errs)
                    {
                        if (errors != null)
                            errors += kSeprator;
                        errors += s;
                    }
                }
            }

            return n;
        }

        bool VerifyInputs(string[] inputVars, string[] inputs)
        {
            return inputVars == null || inputs != null && (inputs.Length % inputVars.Length == 0);
        }

        void GetOutput(PythonProxy.Engine engine, string[] outputVars, object[] outputs, int row)
        {
            if (outputVars == null)
                return;

            int start = outputVars.Length * row;
            for (int i = 0; i < outputVars.Length; ++i)
                outputs[start + i] = engine.GetVariable(outputVars[i]);
        }

        bool SetInput(PythonProxy.Engine engine, string[] inputVars, string[] inputs, int row)
        {
            if (inputVars == null)
                return true;

            int start = inputVars.Length * row;
            for (int i = 0; i < inputVars.Length; ++i)
                engine.SetVariable(inputVars[i], inputs[start + i]);

            return true;
        }

        bool Execute(PythonProxy.Engine engine, string code, out string error)
        {
            error = null;

            code = code.Replace("\\n", "\n");
            return engine.Run(code, out error);
        }
    }

}
