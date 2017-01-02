using System;
using System.Configuration;
using CambridgeSoft.ChemFlow;
using System.IO;

public class PyProxyHelper 
{
    public PyProxyHelper()
    {
        PythonProxy.Engine.Logging.Disabled = true;
        PythonProxy.Engine.Logging.Folder = "";
        PythonProxy.Engine.Logging.ExpireDays = 10;
        PythonProxy.Engine.Logging.Debug = true;
        PythonProxy.Engine.SetPythonVerion("2.5");
    }

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

   
    public int BatchExecute(string code, string[] inputVars, string[] inputs, string[] outputVars,
        out object[] outputs, out string[] errors)
    {
        outputs = null;
        errors = null;
        if (!VerifyInputs(inputVars, inputs))
        {
            SaveLog("Invalid input vars");
            return 0;
        }

        PythonProxy.Engine engine = null;
        engine = new PythonProxy.Engine("");
        if (engine == null)
        {
            SaveLog("Cannot initialize Python!");
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

    void SaveLog(string msg)
    {
        PythonProxy.Engine.Logging.Add("", Log.Level.Error, msg);
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
}
