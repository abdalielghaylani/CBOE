using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Deployment.Application;

namespace CambridgeSoft.COE.Framework.ServerControls.Login
{
    public class Arguments
    {
        #region Variables
        private string[] _arguments;
        #endregion

        #region Methods
        #region Constructors
        public Arguments()
        {

        }
        
        public Arguments(string[] arguments) : this()
        {
            this.SetArguments(arguments);
        }
        #endregion

        #region public Methods
        public void SetArguments(string[] arguments)
        {
            _arguments = arguments;
        }

        public string this[string argumentName]
        {
            get 
            {
                if (_arguments != null)
                {
                    foreach (string currentArgument in this._arguments)
                        if (currentArgument.Equals(argumentName) ||
                            currentArgument.StartsWith(argumentName + "=") ||
                            currentArgument.StartsWith("-" + argumentName + "="))
                        {
                            if (currentArgument.Contains("="))
                                return currentArgument.Substring(currentArgument.LastIndexOf("=") + 1);
                            else
                                return "";
                        }
                }

                return null;
            }
        }
        
        public static string[] GetArgsFromQueryString(string[] args0)
        {
            // get command-line args from http deployment (for click-once)
            // from http://weblogs.asp.net/marianor/archive/2008/03/08/simulating-command-line-parameters-in-click-once-applications.aspx
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.ActivationUri != null)
            {
                string query = HttpUtility.UrlDecode(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
                if (!string.IsNullOrEmpty(query) && query.StartsWith("?"))
                {
                    string[] arguments = query.Substring(1).Split('&');
                    string[] commandLineArgs = new string[arguments.Length + 1];
                    commandLineArgs[0] = Environment.GetCommandLineArgs()[0];
                    arguments.CopyTo(commandLineArgs, 1);
                    return commandLineArgs;
                }
            }
            return args0;
        }

        public override string ToString()
        {
            if (_arguments != null)
            {
                StringBuilder builder = new StringBuilder();

                foreach (string currentString in this._arguments)
                {
                    builder.Append(currentString + "\n");
                }

                return builder.ToString();
            }
            
            return string.Empty;
        }
        #endregion
        #endregion
    }
}
