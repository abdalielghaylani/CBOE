-- start spooling constraints name
ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'

connect chemacxdb/oracle@&&serverName 
spool ON
spool temp_spooled_drops.sql

select 'ALTER TABLE '||TABLE_NAME||' DROP CONSTRAINT '||constraint_name||';' as "--Statement"
from all_constraints
where owner='CHEMACXDB'
and table_name not in ('DB_QUERY',
                       'DB_QUERY_ITEM',
                       'SHOPPINGCART',
                       'USERHITLIST',
                       'USERHITLISTID',
                       'USER_SETTINGS',
                       'CSDOHITLIST',
                       'CSDOHITLISTID');


-- Start spooling indexes

select 'DROP INDEX '||index_NAME||';' as "--Statement"
from all_indexes
where table_owner='CHEMACXDB'
and table_name not in ('DB_QUERY',
                       'DB_QUERY_ITEM',
                       'SHOPPINGCART',
                       'USERHITLIST',
                       'USERHITLISTID',
                       'USER_SETTINGS',
                       'CSDOHITLIST',
                       'CSDOHITLISTID');

spool off

spool on
spool log_update_chemacxdb.txt

@temp_spooled_drops.sql

drop index MX force;

-- Trancate Tables
TRUNCATE TABLE ACX_SYNONYM;
TRUNCATE TABLE CSDOHITLIST;
TRUNCATE TABLE CSDOHITLISTID;
TRUNCATE TABLE GLOBALS;
TRUNCATE TABLE MSDX;
TRUNCATE TABLE PACKAGE;
TRUNCATE TABLE PACKAGESIZECONVERSION;
TRUNCATE TABLE PRODUCT;
TRUNCATE TABLE PROPERTYALPHA;
TRUNCATE TABLE PROPERTYCLASSALPHA;
TRUNCATE TABLE SUBSTANCE;
TRUNCATE TABLE SUPPLIER;
TRUNCATE TABLE SUPPLIERADDR;
TRUNCATE TABLE SUPPLIERPHONEID;
TRUNCATE TABLE VERSION;

-- End of Truncate tables

-- Start Import


ACCEPT dumpFile CHAR DEFAULT 'chemacxdb92.dmp' PROMPT 'Enter the path to the dmp file (chemacxdb92.dmp):'


host imp chemacxdb/oracle@&&serverName file='&&dumpFile' IGNORE=Y TABLES=(ACX_SYNONYM, GLOBALS, MSDX, PACKAGE, PACKAGESIZECONVERSION, PRODUCT, PROPERTYALPHA, PROPERTYCLASSALPHA, SUBSTANCE, SUPPLIER, SUPPLIERADDR, SUPPLIERPHONEID, TEMP_IDS, USER_SETTINGS, VERSION) FULL=N log=log_import.log

--create CS_Cartridge index

drop index MX force;

create index mx on chemacxdb.substance(base64_cdx) INDEXTYPE IS CsCartridge.MoleculeIndexType
	PARAMETERS('TABLESPACE=T_chemacxdb_CSCART');

ALTER INDEX mx PARAMETERS('SYNCHRONIZE');

ANALYZE INDEX mx COMPUTE STATISTICS;


@check_import_chemacx.sql

spool off
prompt logged session to: update_chemacxdb.log

