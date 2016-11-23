-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--- Ask user for JChem Cartridge owner account, password and the basic role name.
ACCEPT jchem_cartridge_pwd CHAR DEFAULT 'manager2' PROMPT 'Enter the password of ChemAxon JChem Cartridge schema (manager2):' HIDE
ACCEPT jchem_cartridge_role CHAR DEFAULT 'JCC_BASIC_ROLE' PROMPT 'Enter the basic role name of the ChemAxon JChem Cartridge (JCC_BASIC_ROLE):'

prompt
prompt Enable ChemAxon JChem Cartridge in Datalytix
prompt

CONNECT &&jchem_cartridge_owner/&&jchem_cartridge_pwd@&&serverName

-- Grant permission for global user to access JChem cartridge.
GRANT &jchem_cartridge_role TO &globalSchemaName;
CALL privman_pkg.grants_on_jcobjs('&jchem_cartridge_owner','&globalSchemaName');

-- Grant permission for COEDB user to access JChem cartridge.
GRANT &jchem_cartridge_role TO &securitySchemaName;
CALL privman_pkg.grants_on_jcobjs('&jchem_cartridge_owner','&securitySchemaName');

COMMIT;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
@@create_VALID_CARTRIDGE_Table.sql
@@insert_cartridge_type.sql 3 'ChemAxon JChem Cartridge' &jchem_cartridge_owner
