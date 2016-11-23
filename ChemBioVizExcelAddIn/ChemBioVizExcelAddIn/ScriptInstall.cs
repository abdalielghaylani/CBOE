using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.Configuration;
using System.Diagnostics; 

namespace DataBaseScript
{
    [RunInstaller(true)]
    public class ScriptInstall : Installer
    {
      
        public ScriptInstall()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }
        
      
        protected override void OnAfterInstall(IDictionary savedState)
        {  
            try
            {
                ExecuteBat();
            }
            catch (Exception ex)
            {
                throw new InstallException("Cannot set the security policy." + " --", ex);
            }
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            try
            {
                ExecuteBat();
            }
            catch (Exception ex)
            {
                throw new InstallException("Cannot remove the security policy." + " --", ex);
            }
        }

        private void ExecuteBat()
        {

            try
            {
                string AssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().Replace(Context.Parameters["AssemblyName"], "") + Context.Parameters["BatchFileName"];
                //System.Windows.Forms.MessageBox.Show(AssemblyPath);

                Process proc = new Process();
                proc.StartInfo.FileName = AssemblyPath;
                proc.StartInfo.Arguments = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().Replace(Context.Parameters["AssemblyName"], "");
                //System.Windows.Forms.MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString().Replace("ChemBioVizExcelAddIn.dll", ""));
                proc.StartInfo.RedirectStandardError = false;
                proc.StartInfo.RedirectStandardOutput = false;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit();
                proc.Close();
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       

    }
    
}
