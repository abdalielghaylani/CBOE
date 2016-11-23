To change the RLS mode unzip the files to a folder and run the following command from a command prompt:

RLSConfigurationTool.exe -v="Registry Level Projects" -i="orcl" -u="system" -p="manager2"

That will change the ActiveRLS setting to "Registry Level Projects", if the ActiveRLS setting needs to be changed to "Batch Level Projects" or "Off" just replace the -v parameter's value as needed.

If you have changed the default oracle instance or the system password please reflect that into the command line parameters.