-- Create structure index using version 3 of CS Cartridge
connect system/manager2@dgb2
alter user cscartridge quota unlimited on t_chemacxdb_cscart;
connect chemacxdb/oracle@dgb2
drop index mx force;

create index mx on substance(base64_cdx) INDEXTYPE IS CsCartridge.MoleculeIndexType
	PARAMETERS('TABLESPACE=T_CHEMACXDB_CSCART');

ALTER INDEX mx PARAMETERS('SYNCHRONIZE');

ANALYZE INDEX mx COMPUTE STATISTICS;

