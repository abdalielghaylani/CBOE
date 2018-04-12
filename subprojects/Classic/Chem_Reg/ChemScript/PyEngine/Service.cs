using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public Service()
    {
    }

    [WebMethod]
    public bool Execute(string code, string returnName, out object retval, out string error)
    {
        retval = null;
        error = null;

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
        outputs = null;
        errors = null;
        if (!VerifyInputs(inputVars, inputs))
            return 0;

        PyNet.Engine engine = new PyNet.Engine();

        int n = 0;
        if (inputVars == null || inputs.Length == 0)
        {
            outputs = new object[outputVars.Length];
            errors = new string[1];

            bool f = Execute(engine, code, out errors[0]);
            if (f)
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
                SetInput(engine, inputVars, inputs, i);

                bool f = Execute(engine, code, out errors[i]);
                if (f)
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

    bool VerifyInputs(string[] inputVars, string[] inputs)
    {
        return inputVars == null || inputs != null && (inputs.Length % inputVars.Length == 0);
    }

    void GetOutput(PyNet.Engine engine, string[] outputVars, object[] outputs, int row)
    {
        if (outputVars == null)
            return;

        int start = outputVars.Length * row;
        for (int i = 0; i < outputVars.Length; ++i)
            outputs[start + i] = engine.GetVariable(outputVars[i]);
    }

    void SetInput(PyNet.Engine engine, string[] inputVars, string[] inputs, int row)
    {
        if (inputVars == null)
            return;

        int start = inputVars.Length * row;
        for (int i = 0; i < inputVars.Length; ++i)
            engine.SetVariable(inputVars[i], inputs[start + i]);
    }

    bool Execute(PyNet.Engine engine, string code, out string error)
    {
        error = null;

        bool f = false;
        try
        {
            f = engine.Run(code);
        }
        catch (Exception e)
        {
            error = e.Message;
        }

        return f;
    }
}
