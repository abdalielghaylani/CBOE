The recommended way of running the patching tool is without parameter. 

COEPatcher.exe


That would prompt the user for the following input:

Oracle Instance (orcl):
Oracle User (system):
Oracle Password: 
Current Version (11.0.1):


Only in particular cases when you want to override the list of patches to run you may use the patcher with parameters. For instance
for running only the list of patches targeted as 11.0.2 you might use:

COEPatcher.exe -i orcl -u system -p manager2 -l .\PatchLists\11.0.2.txt

If you have changed the default oracle instance or the system password please reflect that into the command line parameters.

If you want to get more info about the parameters run :

COEPatcher.exe -h
