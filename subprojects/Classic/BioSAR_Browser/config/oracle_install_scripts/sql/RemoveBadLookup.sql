connect &&InstallUser/&&sysPass@&&serverName;
-- Remove bad lookup for solvate name
update BIOSARDB.DB_COLUMN
set LOOKUP_TABLE_ID = null,
LOOKUP_COLUMN_ID = null,
LOOKUP_COLUMN_DISPLAY = null,
LOOKUP_JOIN_TYPE = null,
LOOKUP_SORT_DIRECT = null
where table_id = (select table_id from BIOSARDB.DB_TABLE
where table_name = 'REGDB.BATCHES') and column_name = 'SOLVATE_NAME';

commit;
