using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;
using System.Diagnostics;

namespace CambridgeSoft.COE.Registration.Services.RegistrationAddins
{
    public class PythonRuntimeWrapper : IDisposable
    {
        #region SingletonPattern
        private static PythonRuntimeWrapper _instance;
        private static readonly object _padlock = new object();

        public static PythonRuntimeWrapper GetInstance()
        {
            lock (_padlock)
            {
                if (_instance == null)
                    _instance = new PythonRuntimeWrapper();
                return _instance;
            }
        }
        #endregion

        #region Properties
        private PythonEngine _pythonEngine;

        private PythonEngine Engine
        {
            get
            {
                this.Initialize();

                if (_pythonEngine == null)
                    _pythonEngine = new PythonEngine();

                return _pythonEngine;
            }
        }

        public void Dispose()
        {
            ShutDown();
        }
        #endregion

        #region Methods
        public PythonRuntimeWrapper()
        {
            Runtime.PythonVersion = Runtime.Version.Python25;
            Initialize();
        }

        public void Initialize()
        {
            if (!PythonEngine.IsInitialized)
            {
                PythonEngine.Initialize();
            }
        }

        public void ShutDown()
        {
            if (PythonEngine.IsInitialized)
            {
                this._pythonEngine = null;
                //PythonEngine.Shutdown();
            }
        }


        public string Home
        {
            set
            {
                PythonEngine.PythonHome = value;
            }
            get
            {
                return PythonEngine.PythonHome;
            }
        }

        public void SetInputVariable(string name, string value)
        {
            PyString pyString = new PyString(value);

            this.Initialize();
            //IntPtr gs = PythonEngine.AcquireLock();
            this.Engine.SetLocalVariable(name, pyString.ToString());
            //PythonEngine.ReleaseLock(gs);
        }

        public string GetOutputVariable(string name)
        {
            this.Initialize();
            //IntPtr gs = PythonEngine.AcquireLock();
            PyObject pythonObject = this.Engine.GetLocalVariable(name);
            //PythonEngine.ReleaseLock(gs);

            return pythonObject.ToString();
        }

        public void Execute(string script)
        {
            script = script.Replace("\r\n", "\n");

            this.Initialize();
            //IntPtr gs = PythonEngine.AcquireLock();
            PyObject returnValue = this.Engine.RunString(script);
            //PythonEngine.ReleaseLock(gs);
        }

        public object AcquireLock()
        {
            this.Initialize();
            return PythonEngine.AcquireLock();
        }

        public void ReleaseLock(object gs)
        {
            PythonEngine.ReleaseLock((IntPtr)gs);
        }
        #endregion
    }
}
