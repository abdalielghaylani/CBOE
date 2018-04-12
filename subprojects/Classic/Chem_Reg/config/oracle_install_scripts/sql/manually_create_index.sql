CONNECT regdb/oracle;

DROP INDEX mx FORCE;

CREATE INDEX mx ON structures(base64_cdx) INDEXTYPE is CsCartridge.MoleculeIndexType PARAMETERS('SKIP_POPULATING=yes,FULLEXACT=INDEX');

UPDATE structures SET base64_cdx = base64_cdx;

COMMIT;

DROP INDEX mx2 FORCE;

CREATE INDEX mx2 ON temporary_structures(base64_cdx) INDEXTYPE is CsCartridge.MoleculeIndexType PARAMETERS('SKIP_POPULATING=yes,FULLEXACT=INDEX');

UPDATE temporary_structures SET base64_cdx = base64_cdx;

COMMIT;


 



