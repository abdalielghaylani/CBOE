ACCEPT dumpFile CHAR DEFAULT 'sql\chemacxdb8.dmp' PROMPT 'Enter the path to the dmp file (sql\chemacxdb8.dmp):'

host imp &&schemaName/&&schemaPass@&&serverName file='&&dumpFile' ignore=yes full=yes log=log_import_chemacx_data.txt
