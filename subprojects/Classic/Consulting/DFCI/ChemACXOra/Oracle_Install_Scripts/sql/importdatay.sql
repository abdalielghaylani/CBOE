ACCEPT dumpFile CHAR DEFAULT 'chemacxdb92.dmp' PROMPT 'Enter the path to the dmp file (chemacxdb92.dmp):'

host imp &&schemaName/&&schemaPass@&&serverName file='&&dumpFile' ignore=yes full=yes log=log_import_chemacx_data.txt
