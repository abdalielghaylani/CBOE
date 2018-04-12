using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;
using System.Threading;
using CambridgeSoft.ChemFlow;

namespace PythonProxy
{
    public class Engine
    {
        static ServiceLog serviceLog = new ServiceLog();

        PythonEngine pyEngine = null;
        string ip = null;

        public Engine(string ip)
        {
            this.ip = ip;
        }

        ~Engine()
        {
            //            Dispose();
        }

        public static void SetPythonVerion(string ver)
        {
            if (ver == "2.5")
                Runtime.PythonVersion = Runtime.Version.Python25;
            else
                Runtime.PythonVersion = Runtime.Version.Python24;
        }

        public static ServiceLog Logging
        {
            get { return serviceLog; }
        }

        PythonEngine PyEngine
        {
            get
            {
                if (!PythonEngine.IsInitialized)
                {
                    try
                    {
                        serviceLog.Add(ip, Log.Level.Regular, "Init Python [" + Runtime.PythonVersion + "] ...");
                        PythonEngine.Initialize();
                    }
                    catch (Exception e)
                    {
                        serviceLog.Add(ip, Log.Level.Crash, "Init Python: " + e.Message);
                    }
                }

                if (pyEngine == null)
                    pyEngine = new PythonEngine();

                return pyEngine;
            }
        }

        void Dispose()
        {
            if (PythonEngine.IsInitialized)
            {
                serviceLog.Add(ip, Log.Level.Regular, "Shutdown Python ...");
                try
                {
                    PythonEngine.Shutdown();
                }
                catch (Exception e)
                {
                    serviceLog.Add(ip, Log.Level.Crash, "Shutdown Python: " + e.Message);
                }
            }
        }

        public bool Run(string code, out string err)
        {
            return RunString(code, out err) != null;
        }

        PyObject RunString(string code, out string err)
        {
            err = null;

            try
            {
                if (serviceLog.Debug)
                    serviceLog.Add(ip, Log.Level.Regular, "Run:\n------------\n" + code + "\n-----------");

                PyObject ret = PyEngine.RunString(code);

                if (serviceLog.Debug)
                    serviceLog.Add(ip, Log.Level.Regular, "Return: " + ret);

                return ret;
            }
            catch (Exception e)
            {
                err = e.Message;
                serviceLog.Add(ip, Log.Level.Error, "RunString: " + err);
                return null;
            }
        }

        public string GetVariable(string varname)
        {
            try
            {
                PyObject ret = PyEngine.GetLocalVariable(varname);
                if (serviceLog.Debug)
                    serviceLog.Add(ip, Log.Level.Regular, "GetVar: [" + varname + "]=" + ret);

                if (ret == null)
                    return null;

                string type = ret.GetPythonType() + "";
                if (type.StartsWith("<class 'ChemScript") && type.EndsWith(".MolPtr'>"))
                    return ConvertCDX(varname);
                else if (type == "<type 'NoneType'>")
                    return null;

                return ret.ToString();
            }
            catch (Exception e)
            {
                serviceLog.Add(ip, Log.Level.Error, "GetVar: " + e.Message);
                return null;
            }
        }

        public void SetVariable(string varname, string value)
        {
            try
            {
                PyEngine.SetLocalVariable(varname, value);

                if (serviceLog.Debug)
                    serviceLog.Add(ip, Log.Level.Regular, "SetVar: [" + varname + "]=" + value);
            }
            catch (Exception e)
            {
                serviceLog.Add(ip, Log.Level.Error, "SetVar: " + e.Message);
            }
        }

        string ConvertCDX(string varname)
        {
            const string tempCdx = "__cdx_";

            try
            {
                string code = tempCdx + "=None\n" + tempCdx + "=" + varname + ".writeData('cdx', True)";
                string err;
                RunString(code, out err);
            }
            catch (Exception e)
            {
                serviceLog.Add(ip, Log.Level.Error, "ConvertCDX: " + e.Message);
                return null;
            }

            return GetVariable(tempCdx);
        }
    }
}
