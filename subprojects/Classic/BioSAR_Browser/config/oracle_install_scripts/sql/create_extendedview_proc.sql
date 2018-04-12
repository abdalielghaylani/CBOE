




--Copyright 1999-2005 CambridgeSoft Corporation. All rights reserved


spool ON
spool log_create_extended_view_proc.txt


SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

DEFINE BASchemaName = BIOASSAYHTS
DEFINE BASchemaPass = ORACLE
DEFINE REGSchemaName = REGDB
DEFINE REGSChemaPass = ORACLE


ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):'

Connect &&InstallUser/&&sysPass@&&serverName
grant create view to &&BASchemaName;

Connect &&REGSchemaName/&&REGSChemaPass@&&serverName
grant select on regdb.reg_numbers to &&BASchemaName with grant option;
grant select on regdb.batches to &&BASchemaName with grant option;


Connect &&BASchemaName /&&BASchemaPass@&&serverName

CREATE OR REPLACE
procedure CreateExtendedView(	pBaseName in varchar2, 
								pBaseDisplayName in varchar2, 
								oNewName out varchar2, 
								oNewDisplayName out varchar2,
								oKeyField out varchar2) as
	vSql varchar2(2000);
    vPrefix varchar2(10):='KSID_';
    
    junk boolean;
    
begin
	
	oKeyField :='CSMNUM';
	
	-- Check that the Base View contains the key field
	vSql:= 'SELECT 1 FROM user_tab_cols  
				WHERE table_name= :1  
				AND column_name= :2 '; 
	execute immediate vSql
		into junk
		using pBaseName, oKeyField;
	
	-- Set the New Names
	oNewName := vPrefix || pBaseName;
	oNewDisplayName:= vPrefix || pBaseDisplayName;
	
	-- Create the extended view
	vSql:= 'CREATE OR REPLACE VIEW ' || oNewName || ' AS  
				SELECT r.reg_number, b.batch_number, v.*
					FROM '|| pBaseName || ' v, regdb.reg_numbers r, regdb.batches b
					WHERE v.'|| oKeyField ||' = b.field_1
					AND r.reg_id = b.reg_internal_id';

     --raise_application_error(-20000, vSQL);
     execute immediate vSql;
     execute immediate 'grant select on '|| oNewName || ' to biosar_browser_admin';
     execute immediate 'grant select on '|| oNewName || ' to biosar_user';	
exception
	when no_data_found then
		raise_application_error(-20002, '# Extended view could not be created. A field named '
											|| oKeyField ||' is required to create the extended view.  Only the standard view has been exposed.#');    
end CreateExtendedView;
/


spool off;
exit