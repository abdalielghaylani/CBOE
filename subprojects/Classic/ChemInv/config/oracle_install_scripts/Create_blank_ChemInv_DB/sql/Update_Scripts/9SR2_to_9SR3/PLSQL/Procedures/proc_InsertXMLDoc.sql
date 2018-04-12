CREATE OR REPLACE PROCEDURE "&&SchemaName"."INSERTXMLDOC"(
	theDir varchar2,
	theFile varchar2,
	name varchar2 := NULL,
	typeID inv_xmldocs.xmldoc_type_id_fk%TYPE)  
IS
theBFile	BFILE;
theCLOB		CLOB;
theDocName	varchar2(200) := NVL(name,theFile);

BEGIN                             
	INSERT INTO INV_XMLDOCS (XMLDOC, NAME, XMLDOC_TYPE_ID_FK) VALUES (empty_clob(), theDocName, typeID) RETURNING XMLDOC INTO theCLOB;
	theBFile := BFileName(theDir, theFile);
	dbms_lob.fileOpen(theBFile);
	dbms_lob.loadFromFile(dest_lob => theCLOB,
		src_bfile => theBFile,
		amount => dbms_lob.getLength(theBFile));
	dbms_lob.fileClose(theBFile);
END;
/
show errors;
