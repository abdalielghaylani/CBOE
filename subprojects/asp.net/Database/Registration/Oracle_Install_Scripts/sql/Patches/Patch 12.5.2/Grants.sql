
prompt 
prompt Starting "Grants.sql"...
prompt 
	
-- SITES WORK FLOW 	
     Connect &&InstallUser/&&sysPass@&&serverName;
     GRANT  SELECT,INSERT,DELETE,UPDATE ON coedb.sites  TO REGDB;




