DROP index mx;

CREATE INDEX MX ON substance(base64_cdx) INDEXTYPE IS CsCartridge.MoleculeIndexType
PARAMETERS('SKIP_POPULATING=YES');
commit;


ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum < 100;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;

ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 1 AND 50000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 50000 AND 100000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 100000 AND 150000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 150000 AND 200000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 200000 AND 250000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 250000 AND 300000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
ALTER INDEX mx PARAMETERS('suspend');
UPDATE substance SET base64_cdx = base64_cdx where csnum between 300000 AND 350000;
ALTER INDEX mx PARAMETERS('resume');
COMMIT;
