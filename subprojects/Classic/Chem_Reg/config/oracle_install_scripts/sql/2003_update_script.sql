--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

--run this script to update the cartridge version of ChemREG.  THIS IS FOR NEW INSTALLATION ONLY. THE BASE64 COLUMNS ARE DROPPED>




Connect &&schemaName/&&schemaPass@&&serverName

Drop Index regdb.mx;

ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES"
    SET UNUSED ("BASE64_CDX") CASCADE CONSTRAINTS;

ALTER TABLE "REGDB"."TEMPORARY_STRUCTURES"
    ADD("BASE64_CDX" CLOB);

ALTER TABLE "REGDB"."STRUCTURES"
    SET UNUSED ("BASE64_CDX") CASCADE CONSTRAINTS;

ALTER TABLE "REGDB"."STRUCTURES"
    ADD("BASE64_CDX" CLOB);


create index regdb.mx on regdb.structures(base64_cdx) indextype is cscartridge.moleculeindextype;
create index regdb.mx2 on regdb.temporary_structures(base64_cdx) indextype is cscartridge.moleculeindextype;


---create sequnce for molids and triggers for structures and temporary_structures tables

--DROP SEQUENCE MOLID_SEQ;
CREATE SEQUENCE MOLID_SEQ INCREMENT BY 1 START WITH 1;


-- Create table level triggers for structures.
create or replace trigger TRG_MOL_ID
BEFORE INSERT ON STRUCTURES
FOR EACH ROW

BEGIN
SELECT MOLID_SEQ.NEXTVAL INTO :NEW.MOL_ID  FROM DUAL;
END;
/





-- Create table level triggers for temporary_structures table
create or replace trigger TRG_TEMP_MOL_ID
BEFORE INSERT ON TEMPORARY_STRUCTURES
FOR EACH ROW

BEGIN
SELECT MOLID_SEQ.NEXTVAL INTO :NEW.MOL_ID  FROM DUAL;
END;
/

commit;