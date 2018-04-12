using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using CambridgeSoft.ChemFlow;
using System.IO;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public Service()
    {
        PythonProxy.Engine.Logging.Disabled = (ConfigurationManager.AppSettings["log"] + "").ToLower() != "on";
        PythonProxy.Engine.Logging.Folder = ConfigurationManager.AppSettings["logfolder"];
        PythonProxy.Engine.Logging.ExpireDays = ToInt(ConfigurationManager.AppSettings["logdays"]);
        PythonProxy.Engine.Logging.Debug = (ConfigurationManager.AppSettings["debug"] + "").ToLower() == "on";
        PythonProxy.Engine.SetPythonVerion(ConfigurationManager.AppSettings["pythoncoreversion"]);
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

    [WebMethod]
    public string Version()
    {
        return "CambridgeSoft Python WebService 1.52 (" + PythonProxy.Engine.PythonVersion + ")";
    }

    [WebMethod]
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

    [WebMethod]
    public int BatchExecute(string code, string[] inputVars, string[] inputs, string[] outputVars,
        out object[] outputs, out string[] errors)
    {
        int n = 0;
        outputs = null;
        errors = null;

        Application.Lock();
        while ((Application["lock"] + "") == "locked")
        {
            Application.UnLock();
            System.Threading.Thread.Sleep(50);
            Application.Lock();
        }

        Application["lock"] = "locked";

        try
        {
            n = ExecuteIt(code, inputVars, inputs, outputVars, out outputs, out errors);
        }
        catch (Exception e)
        {
            if (errors == null)
            {
                errors = new string[0];
                errors[0] = e.Message;
            }
        }

        Application["lock"] = "";
        Application.UnLock();
        return n;
    }

    int ExecuteIt(string code, string[] inputVars, string[] inputs, string[] outputVars,
        out object[] outputs, out string[] errors)
    {
        outputs = null;
        errors = null;
        if (!VerifyInputs(inputVars, inputs))
        {
            SaveLog("Invalid input vars");
            return 0;
        }

        int n = 0;

        using (PythonProxy.Engine engine = new PythonProxy.Engine(Context.Request.UserHostName))
        {
            if (engine == null)
            {
                SaveLog("Cannot initialize Python!");
                return 0;
            }

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

            engine.Dispose();
        }

        return n;
    }

    [WebMethod]
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

            outputs += kSeprator;
            outputs += PythonProxy.Engine.Logging.Log.LogEntry;

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
        PythonProxy.Engine.Logging.Add(Context.Request.UserHostName, Log.Level.Error, msg);
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
