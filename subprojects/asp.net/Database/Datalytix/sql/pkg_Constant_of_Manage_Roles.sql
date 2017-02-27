COLUMN X NOPRINT NEW_VALUE BioSar_r
SELECT case when COUNT(*)=0 then 'TRUE' else 'FALSE' end as X
        FROM dba_tables
       WHERE table_name = UPPER ('BIOSAR_BROWSER_PRIVILEGES');

CREATE OR REPLACE PACKAGE &&schemaName..Constant_of_Manage_Roles AS	 
	--  !!!!!!!!!!!!this package must be created before Manage_Roles package!!!!!!!!!!
	-- this package is used for correct working Manage_Roles package,  BioSar_roles depends on customer has BioSar/Datalytix or not. 
	-- Please, don't delete this package!!!
   -- if we have biosar_browser_privileges table BioSar_roles must be FALSE 
   -- if we don't have biosar_browser_privileges table BioSar_roles must be TRUE
   
   BioSar_roles constant boolean := &&BioSar_r;
end;
/	   
show errors
