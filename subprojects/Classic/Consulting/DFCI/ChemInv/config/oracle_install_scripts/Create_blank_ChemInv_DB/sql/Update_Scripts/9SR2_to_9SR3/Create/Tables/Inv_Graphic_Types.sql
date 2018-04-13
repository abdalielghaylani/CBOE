CREATE TABLE "INV_GRAPHIC_TYPES"(
	"GRAPHIC_TYPE_ID" NUMBER NOT NULL,
	"GRAPHIC_TYPE_NAME" VARCHAR2(50),
    CONSTRAINT "INV_GRAPHIC_TYPE_PK"
		PRIMARY KEY("GRAPHIC_TYPE_ID") USING INDEX TABLESPACE &&indexTableSpaceName
	);
	
CREATE SEQUENCE SEQ_INV_GRAPHIC_TYPES INCREMENT BY 1 START WITH 1;


CREATE OR REPLACE TRIGGER "TRG_INV_GRAPHIC_TYPES_ID"
    BEFORE INSERT
    ON "INV_GRAPHIC_TYPES"
    FOR EACH ROW
BEGIN
	if :new.graphic_type_id is null then
		select SEQ_INV_GRAPHIC_TYPES.nextval into :new.graphic_type_id from dual;
	end if;
END;
/