CREATE OR REPLACE PROCEDURE "&&SchemaName"."INSERTXSLT"(
	theDir varchar2,
	theFile varchar2,
	name varchar2 := NULL)  
IS
theBFile	BFILE;
theCLOB		CLOB;
theDocName	varchar2(200) := NVL(name,theFile);

BEGIN                             
	INSERT INTO INV_XSLTS (XSLT, XSLT_Name) VALUES (empty_clob(), theDocName) RETURNING XSLT INTO theCLOB;
	theBFile := BFileName(theDir, theFile);
	dbms_lob.fileOpen(theBFile);
	dbms_lob.loadFromFile(dest_lob => theCLOB,
		src_bfile => theBFile,
		amount => dbms_lob.getLength(theBFile));
	dbms_lob.fileClose(theBFile);
END;
/
show errors;
